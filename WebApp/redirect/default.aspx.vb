
Partial Class _default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '-- 67.205.109.85
        Select Case D.Server.Info.DomainName
            Case "akrodepot.com" : D.Server.Redirect.DomainMoved("depot.akro.ca")
            Case "courbec.com" : D.Server.Redirect.PageMovedPermanentlyToWWW()
            Case "exelinternational.com" : D.Server.Redirect.PageMovedPermanentlyToWWW()
            Case "exel.qc.ca" : D.Server.Redirect.DomainMoved("www.exelinternational.com")
            Case "jonxion.ca" : D.Server.Redirect.PageMovedPermanentlyToWWW()
            Case "lugital.com" : D.Server.Redirect.PageMovedPermanentlyToWWW()
            Case "msbexpertise.com" : D.Server.Redirect.PageMovedPermanentlyToWWW()
            Case "sfpodium.ca" : D.Server.Redirect.PageMovedPermanentlyToWWW()
            Case "msbdesign.ca" : D.Server.Redirect.KeepReferer("http://www.msbexpertise.com/")
            Case "m.axiom19.com" : D.Server.Redirect.KeepReferer("http://www.axiom19.com/mobile")
            Case "vivenda.net" : D.Server.Redirect.DomainMoved("www.vivenda.ca") 'D.Server.Redirect.KeepReferer("http://www.vivenda.ca")
            Case "montrealparty.com", "barhop.ca", "december31.ca" : D.Server.Redirect.DomainMoved("montrealparty.com")
            Case "beachpartyckoi.ca" : D.Server.Redirect.DomainMoved("beachpartyckoi.com")
            Case "arminvanbuurenmontreal.com", "festivalsundance.ca", "festivalsundance.com", "sundancefestival.ca", "partyalaplage.ca", "partyalaplage.com", "plagefamiliale.ca", "plagefamiliale.com" : D.Server.Redirect.DomainMoved("beachclub.com")
            Case Else
                If D.Server.Info.DomainName.Contains("webmail.") Then
                    Response.Redirect("http://mail.google.com/a/" & D.Server.Info.DomainName.ToString.Replace("webmail.", ""))
                Else
                    Response.Write("Not Redirected")
                    Try
                        Dim pWriter As New IO.StreamWriter(Request.PhysicalApplicationPath & "redirect.log", True)
                        pWriter.WriteLine(Request.ServerVariables("HTTP_HOST") & vbTab & Request.ServerVariables("QUERY_STRING") & vbTab & Request.ServerVariables("HTTP_REFERER"))
                        pWriter.Close()
                    Catch ex As Exception
                    End Try
                End If
        End Select
    End Sub

End Class

Namespace D
    Public Class Server
#Region "CONTEXT"
        Public Shared ReadOnly Property Response() As System.Web.HttpResponse
            Get
                Return HttpContext.Current.Response
            End Get
        End Property

        Public Shared ReadOnly Property Request() As System.Web.HttpRequest
            Get
                Return HttpContext.Current.Request
            End Get
        End Property


        Public Shared ReadOnly Property Cache() As System.Web.Caching.Cache
            Get
                Return HttpContext.Current.Cache
            End Get
        End Property

        Public Shared ReadOnly Property Session() As System.Web.SessionState.HttpSessionState
            Get
                Return HttpContext.Current.Session
            End Get
        End Property

        Public Shared ReadOnly Property IsDevelopment() As Boolean
            Get
                If Request.ServerVariables("SERVER_NAME") = "localhost" Then Return True
                Return False
            End Get
        End Property
#End Region
#Region "INFO"
        Public Class Info

            ''' <summary>Return Domain name as long i.e.: secure.host.com</summary>
            Public Shared Function DomainName() As String
                Return Request.ServerVariables("SERVER_NAME").ToLower.Replace("www.", "")
            End Function

            ''' <summary>Return all Domain host i.e.: Array(0) = "com", Array(1) = "lugital"</summary>
            Public Shared Function DomainHosts() As Generic.List(Of String)
                Dim pDomainHostList As Generic.List(Of String) = New Generic.List(Of String)
                Dim pServer As String = Info.DomainName()
                If InStr(pServer, ".") = 0 Then pServer = pServer & "." & pServer & "." & pServer & "." & pServer & "." & pServer
                Dim pHost As Array = Split(pServer, ".")
                For idx As Integer = UBound(pHost) To 0 Step -1
                    pDomainHostList.Add(pHost(idx))
                Next
                pDomainHostList.Add("") : pDomainHostList.Add("") : pDomainHostList.Add("") : pDomainHostList.Add("") '-- SECURITY
                Return pDomainHostList
            End Function

            ''' <summary>Return URL Decoded Query string without the ?"</summary>
            Public Shared ReadOnly Property QueryString() As String
                Get
                    Return HttpUtility.UrlDecode(Request.ServerVariables("QUERY_STRING"))
                End Get
            End Property

            Public Shared Function Referer() As String
                Return Request.ServerVariables("HTTP_REFERER")
            End Function

            ''' <summary>Return IP address of the remote computer / client</summary>
            Public Shared Function RemoteIP() As String
                Return Request.ServerVariables("REMOTE_ADDR")
            End Function

            Public Shared Function Language() As String
                Dim pLanguage As String = "en"
                Try
                    If D.Server.Request.UserLanguages(0).ToLower.Contains("fr") Then pLanguage = "fr"
                Catch ex As Exception
                    pLanguage = "en"
                End Try
                Return pLanguage
            End Function
        End Class
#End Region '"INFO"
#Region "URL"
        Public Class URL
            ''' <summary>Return: https://secure.devless.com:80/vdir/ or http://devless.com/</summary>
            Public Shared ReadOnly Property Site() As String
                Get
                    Dim pSiteURL As String = ""
                    pSiteURL += LCase(Request.ServerVariables("SERVER_PROTOCOL"))
                    pSiteURL = Left(pSiteURL, InStrRev(pSiteURL, "/") - 1) & "://"
                    pSiteURL += Request.ServerVariables("HTTP_HOST")
                    pSiteURL += Request.ServerVariables("URL")
                    pSiteURL = Left(pSiteURL, InStrRev(pSiteURL, "/"))
                    Return HttpUtility.UrlDecode(pSiteURL)
                End Get
            End Property

            ''' <summary>Return: https://secure.devless.com:80/vdir/?tag=Home-FR or http://devless.com/?tag=Home-FR</summary>
            Public Shared Function Page(Optional ByVal inExcludeFromQueryString As String = "") As String
                Dim pQueryString As String = Info.QueryString
                If pQueryString <> "" Then pQueryString = "?" & HttpUtility.UrlEncode(pQueryString).Replace(HttpUtility.UrlEncode("&"), "&").Replace(HttpUtility.UrlEncode("="), "=")
                If inExcludeFromQueryString <> "" And pQueryString.Contains(inExcludeFromQueryString) Then pQueryString = ""
                Return URL.Site & pQueryString
            End Function

        End Class

#End Region '"URL"
#Region "REDIRECT"
        Public Class Redirect
            Public Shared Sub DomainAlias(ByVal inOfficalDomain As String)
                Dim pURL As String = "http://" & inOfficalDomain
                PageMovedPermanently(pURL)
            End Sub

            Public Shared Sub DomainMoved(ByVal inNewDomain As String)
                Dim pURL As String = "http://" & inNewDomain
                Dim pQueryString As String = Info.QueryString
                If pQueryString <> "" Then pURL += "/?" & pQueryString
                PageMovedPermanently(pURL)
            End Sub

            Public Shared Sub PageNotFound()
                Response.Clear()
                Response.Status = "404 Not Found"
                Response.Write("404 Not Found")
                Response.End()
            End Sub

            Public Shared Sub PageMovedPermanently(ByVal inNewPageFullURL As String)
                Response.Clear()
                Response.Status = "301 Moved Permanently"
                Response.AddHeader("Location", inNewPageFullURL)
                Response.End()
            End Sub

            Public Shared Sub PageMovedPermanentlyToWWW()
                Dim pPageURL As String = URL.Page
                If Not pPageURL.Contains("www.") Then pPageURL = pPageURL.Replace("http://", "http://www.")
                PageMovedPermanently(pPageURL)
            End Sub

            Public Shared Sub KeepReferer(ByVal inAbsoluteURL As String, Optional ByVal inMethod As String = "GET")
                If Not inAbsoluteURL.Contains("?") And Server.Info.QueryString <> "" Then inAbsoluteURL += "?" & Server.Info.QueryString
                Response.Clear()
                Response.Write("<meta http-equiv=""refresh"" content=""2; url=""" & inAbsoluteURL & """><form id=""redirect"" action=""" & inAbsoluteURL & """ method=""" & inMethod & """></form><script>document.getElementById(""redirect"").submit();</script><a href=""" & inAbsoluteURL & """>" & inAbsoluteURL & "</a>")
                Response.End()
            End Sub
        End Class
#End Region '"REDIRECT"

    End Class ' Server END
End Namespace