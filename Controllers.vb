Imports System.Net.Http
Imports System.Web.Http
Imports System.Web.HttpContext
Imports System.Web.Http.Cors
Imports XbrlX

'This defines the endpoints.
Namespace Controllers
    
    <RoutePrefix("api/v1/us/std/items")> 'This sets a default prefix for the controller. NOTE: We need the word items otherwise will read secions as tag.
    Public Class StdValuesController
        Inherits ApiController
        Dim currCallType As String
        Dim currUser As UsrLogin, currCos As Companies, currValues As Values
        Public Sub New() 'Think you always need a parameterless constructor even if empty.
            currUser = New UsrLogin
            currCos = New Companies
            currValues = New Values(currCos)
        End Sub
        'PeriodNo can now be specified relative now so 0, -1,-2 are all valid! You can also leave off the minus sign. Also "12" brings back 1 & 2 for a filing.
        <Route("{tag}/fiscal/yr/{periodNo:int?}/{cos?}/{tkn:int?}")>
        Public Function GetLatestStdYrVal(tag As String, Optional periodNo As Short = 0, Optional cos As String = "msft", Optional tkn As Integer = 0) As IHttpActionResult
            currCallType = "LatestStdYrVal"
            If Not Check.testParams(tkn, cos, 3000000, 1, periodNo, currCallType, tag, Nothing) Then
                Return BadRequest(Check.msg)
            End If

            If currValues.GetLatestStdValue(cos, tag, (-1 * periodNo) + 1) Then
                Return Ok(currValues.Values)
            Else
                Return BadRequest(Check.msg)
            End If

        End Function

    End Class
 
End Namespace