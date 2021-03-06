USE [FilingsDbL15]
GO
/****** Object:  StoredProcedure [dbo].[GetStdSeriesSectValsByName]    Script Date: 02/12/2021 15:57:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetStdSeriesSectValsByName] @Tkrs Tickers READONLY, @endDate INT, @begDate INT, @periodNo1 SMALLINT, @periodNo2 SMALLINT, @name Char(4), @schemaId smallInt
As

--The point of this query is to create/include a master order and label so as to aid the display statements from different years as one
--giant table spanning filings. This is accomplished by seperating out the preRelIds (orders) into a table to set a Series order worked out as follows:
--StdId (used as the partition column - same as DictId for now), then by Latest Filing, PreRelId.
--This is done via an inner join in the final table.
--Note: we don't use these Series values here. You can deploy them once dara has been pivoted to or aggreagated into single rows to set the order.
--Now you can select period 1 and period 2 for a filing rather than having to choose between the two. You can still just choose one by setting both values the same.

Declare @ReqFilings ReqFilingsYrRnk --Using a defined Table Type (look under Types to see it)
INSERT INTO @ReqFilings
SELECT e.Ticker, e.CoName, e.CIK,  f.FilingId, f.EndDate, f.AccessionNo, DENSE_RANK() OVER(PARTITION BY e.Ticker ORDER BY EndDate Desc)[FilingYear] from Filings as f
	Inner Join Filers as e on e.FilerId=f.FilerId	
WHERE e.Ticker IN (SELECT * FROM @Tkrs) and f.FormNo = 4 and f.EndDate<=@endDate And f.EndDate>@begDate  /* And p.endDate>=20130825 and p.endDate<20140405 and days >360 and days<370*/

IF OBJECT_ID('tempdb..#SectPreRels', 'U') IS NOT NULL
Drop Table #SectPreRels
SELECT rf.CoName, rf.Ticker, rf.FilingYear, rf.EndDate, rf.CIK, pr.FilingId, rf.AccessionNo, s.FilingSectionId, pr.PreRelId, pr.DictId, sd.StdId, i.ItemSchema, i.CamelTag, t.String1[Label], i.Abstract, pr.isNegLbl, aa.Mnemonic into #SectPreRels from FilingsDbL15Text.dbo.FilingPreRels as pr  
		inner join @ReqFilings as rf on rf.FilingId = pr.FilingId
		inner join FilingsDbL15Text.dbo.ItemDictionary as i on pr.DictId = i.DictId
		inner join FilingsDbL15Text.dbo.FilingSections as s on s.FilingSectionId = pr.FilingSectionId and s.FilingId = pr.FilingId
		inner join StdAliases as a on a.DictId = s.DictId
		inner join StdAliases as aa on aa.StdDictId = a.StdDictId
		full outer join StdDict as sd on sd.DictId = pr.DictId and sd.StdId < 300000000000
		left outer join FilingsDbL15Text.dbo.StringDictionary as t on pr.LabelId = t.StringId  --Left outer join ensures we still get the preRels even if there is no corresponding string (LabelId = Null) e.g preRel 87 for filingId = 145481 (HD View 360)
	 WHERE /*pr.FilingId in (Select FilingId from #ReqFilings)*/ /*pr.FilingSectionId = 2*/ aa.Mnemonic = @name and i.Abstract = 0 and i.CamelTag not like '%Abstract' --Removes Axis and domain tags etc) as pr

	--Select * From (
	--SELECT CamelTag, DictId, EndDate, Label, PreRelId [SeriesOrder]
	--	RANK () OVER (PARTITION BY StdId ORDER BY EndDate Desc, PreRelId) as Rnk
	--from #SectPreRels) as rkd
	--where rkd.Rnk = 1

--Using distinct as we've got duplicate values being saved when loading filings!!
SELECT distinct sp.CoName, sp.Ticker, sp.CIK, sp.AccessionNo, sp.FilingYear, sp.EndDate, sp.FilingSectionId, sdp.Position[StdPosition], co.SeriesOrder, sp.PreRelId, sp.ItemSchema, sp.CamelTag, sdc.CamelTag[StdCamelTag], sp.DictId, sdp.StdDictId, sp.StdId, sp.Abstract, sp.isNegLbl, co.Label[SeriesName], sp.Label[Name], sv.endDate[ValueDate], sv.days, sv.Value1 from #SectPreRels as sp
		-- this links in to the Std components.
		left outer join StdPreRels as sdp on sp.DictId = sdp.DictId and sp.Mnemonic = sdp.SectCode and sp.Ticker = sdp.Ticker and sdp.SchemaId = @schemaId
		inner join --this bit is no longer essential in this query, SeriesOrder is superseded now by StdPosition in how we use this.
		   (Select * From (
			SELECT CamelTag, DictId, EndDate, Label, PreRelId [SeriesOrder],
				RANK () OVER (PARTITION BY DictId ORDER BY EndDate Desc, PreRelId) as Rnk
			from #SectPreRels) as rkd
			where rkd.Rnk = 1
			) as co on sp.DictId = co.DictId
		left outer join --This connects up the values
		   (SELECT v.FilingId, v.DictId, dur.PerRefId, dur.endDate, p.PPId, dur.days, v.Value1 from ItemValues as v
				inner join @ReqFilings as rf on rf.FilingId = v.FilingId				
				inner join PrincipalPeriods as p on (p.durPerRefId = v.PerRefId or p.instPerRefId = v.PerRefId) and p.FilingId = v.FilingId
				Inner Join PerRefs as dur on dur.PerRefId = p.durPerRefId    
			WHERE v.ValueType <= 2 and v.SegId = 0 and p.PPId in (@PeriodNo1,@PeriodNo2) --v.FilingId in (SELECT FilingId FROM @fIDs)
			) as sv on sp.DictId = sv.DictId and sp.FilingId = sv.FilingId
		left outer join StdDict as sdc on sdc.DictId = sdp.StdDictId --This just gets the CamelTag corresponding to the StdTag.
--where (pr.Abstract = 0 or pr.CamelTag like '%Abstract') --May be quicker placed here - no evidence of this yet.
--Group by 
--where sp.DictId is not NULL
ORDER by sp.Ticker, sp.FilingYear, sp.PreRelId --, sp.FilingId desc