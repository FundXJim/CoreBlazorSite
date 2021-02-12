Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager
Imports XbrlX
Public Class Values
    Dim cos As Companies
    Property Values As List(Of Value)
    Property SECNums As List(Of SECNum)
    Public downLoadTot As Short
    Dim tkrTable As DataTable
    Dim filingsConn As SqlConnection
    Public errMsg As String
    Public Sub New()
    End Sub
    Public Sub New(cs As Companies)
        cos = cs
    End Sub
    Private Function openConnection() As Boolean
        filingsConn = New SqlConnection(ConnectionStrings("FilingsDb").ConnectionString)
        Try
            filingsConn.Open()
        Catch ex As Exception
            'Pick up filingsConn open exception
            Check.msg = Check.msg & " Db open fail " & ex.Message
            Check.Log()
            Return False
        End Try
        Return True
    End Function
    Sub loadTkrTable(tkrs As String) 'To be removed as now in Check.
        tkrTable = New DataTable()
        Dim tkrArr = Split(tkrs, ",")
        tkrTable.Columns.Add("Ticker", GetType(String))
        For i = 0 To tkrArr.Count - 1
            tkrTable.Rows.Add(tkrArr(i))
        Next
        'These tickers may not exist in the db so could be false count and downloads get falsely reduced. If none present then
        'no records get returned so this will get trapped and downloads will not change.
        downLoadTot = tkrArr.Count
    End Sub
  
    Public Function GetLatestStdValue(tkr As String, tag As String, rYr As Short) As Boolean

        Dim dataCmd As SqlCommand, dataRdr As SqlDataReader

        If Not openConnection() Then
            Return False
        End If


        'Stored procedure alt.
        'dataCmd = New SqlCommand("GetValues", filingsConn)
        dataCmd = New SqlCommand("GetLatestStdValues", filingsConn)
        dataCmd.CommandType = CommandType.StoredProcedure
        dataCmd.Parameters.AddWithValue("@Tkr", tkr)
        dataCmd.Parameters.AddWithValue("@Tag", tag)
        dataCmd.Parameters.AddWithValue("@RelYr", rYr)

        dataCmd.CommandTimeout = 1000 '600
        Try
            dataRdr = dataCmd.ExecuteReader()
        Catch ex As Exception
            Check.msg = Check.msg & " Db query fail " & ex.Message
            Check.Log()
            Return False
        End Try

        Values = New List(Of Value)


        Dim linkRoot As String = AppSettings("EdgarLinkRoot")
        Do While dataRdr.Read()
            Values.Add(New Value(dataRdr("CoName"), dataRdr("CamelTag"), dataRdr("StdTag"), dataRdr("EndDate") / 10000, dataRdr("EndDate"), dataRdr("days"), dataRdr("Value1")))
            'Values.Add(New Value(dataRdr(0), dataRdr(1), dataRdr(2) / 10000, dataRdr(2), dataRdr(3), dataRdr(4)))
        Loop
        dataRdr.Close()
        filingsConn.Close()

        Check.msg = "Ok(" & Values.Count & ")"
        Check.Log()
        Return True


    End Function
End Class
