Imports Newtonsoft.Json
Imports System.Net
Imports System.IO
Imports System.Runtime.Serialization
Imports Newtonsoft.Json.Linq

Namespace JBou.Authentication
    Public Class Session
        Inherits Craft.Net.Client.Session

        Public Shared Shadows Async Function DoLogin(username As String, password As String) As Task(Of Session)
            Return Await Task.Run(Function()
                                      Dim serializer = New JsonSerializer()
                                      Try
                                          Dim request = DirectCast(WebRequest.Create("https://authserver.mojang.com/authenticate"), HttpWebRequest)
                                          request.ContentType = "application/json"
                                          request.Method = "POST"
                                          Dim token = Guid.NewGuid().ToString()
                                          Dim blob = New Session.AuthenticationBlob(username, password, token)
                                          Dim json = JsonConvert.SerializeObject(blob)
                                          Dim stream = request.GetRequestStream()
                                          Using writer = New StreamWriter(stream)
                                              writer.Write(json)
                                          End Using
                                          Dim response = request.GetResponse()
                                          stream = response.GetResponseStream()
                                          Dim session = serializer.Deserialize(Of Session)(New JsonTextReader(New StreamReader(stream)))
                                          session.UserName = username
                                          Return session
                                      Catch e As WebException
                                          Dim stream = e.Response.GetResponseStream()
                                          Dim json = New StreamReader(stream).ReadToEnd()
                                          stream.Close()
                                          Dim jo = JObject.Parse(json)
                                          Throw New MinecraftAuthenticationException() With {.Error = jo.Value(Of String)("error"), .ErrorMessage = jo.Value(Of String)("errorMessage"), .Cause = jo.Value(Of String)("cause")}
                                      End Try
                                  End Function)
        End Function

        Public Sub New(userName As String)
            MyBase.New(userName)
            AvailableProfiles = New Profile() {New Profile() With {
                 .Name = userName,
                 .Id = Guid.NewGuid().ToString()
            }}
            SelectedProfile = AvailableProfiles(0)
        End Sub
        Public Sub New()
            ' TODO: Complete member initialization 
            MyBase.New(Nothing)
        End Sub


        ''' <summary>
        ''' Refreshes this session, so it may be used again. You will need to re-save it if you've
        ''' saved it to disk.
        ''' </summary>
        Public Shadows Async Function Refresh() As Task
            Await Task.Run(Sub()
                               If Not OnlineMode Then
                                   Throw New InvalidOperationException("This is an offline-mode session.")
                               End If
                               Dim serializer = New JsonSerializer()
                               Try
                                   Dim request = DirectCast(WebRequest.Create("https://authserver.mojang.com/refresh"), HttpWebRequest)
                                   request.ContentType = "application/json"
                                   request.Method = "POST"
                                   Dim blob = New RefreshBlob(Me)
                                   Dim stream = request.GetRequestStream()
                                   Dim json As String = JsonConvert.SerializeObject(blob, Formatting.Indented, New JsonSerializerSettings() With {.NullValueHandling = NullValueHandling.Ignore})
                                   Using writer = New StreamWriter(stream)
                                       writer.Write(json)
                                   End Using
                                   stream.Close()
                                   Dim response = request.GetResponse()
                                   stream = response.GetResponseStream()
                                   Dim jsonresponse = New StreamReader(stream).ReadToEnd()
                                   stream.Close()
                                   Dim sessionblob = JsonConvert.DeserializeObject(Of Session)(jsonresponse)
                                   Me.AccessToken = sessionblob.AccessToken
                                   Me.ClientToken = sessionblob.ClientToken
                                   Me.User = sessionblob.User
                                   Me.UserName = sessionblob.SelectedProfile.Name
                                   Me.SelectedProfile = sessionblob.SelectedProfile
                                   ' TODO: Add profile to available profiles if need be
                               Catch e As WebException
                                   Dim stream = e.Response.GetResponseStream()
                                   Dim json = New StreamReader(stream).ReadToEnd()
                                   stream.Close()
                                   Dim jo = JObject.Parse(json)
                                   Throw New MinecraftAuthenticationException() With {.Error = jo.Value(Of String)("error"), .ErrorMessage = jo.Value(Of String)("errorMessage"), .Cause = jo.Value(Of String)("cause")}
                               End Try
                           End Sub)
        End Function

        Public Shadows Async Function Invalidate() As Task
            Await Task.Run(Sub()
                               If Not OnlineMode Then
                                   Throw New InvalidOperationException("This is an offline-mode session.")
                               End If
                               Dim serializer = New JsonSerializer()
                               serializer.NullValueHandling = NullValueHandling.Ignore
                               Try
                                   Dim request = DirectCast(WebRequest.Create("https://authserver.mojang.com/invalidate"), HttpWebRequest)
                                   request.ContentType = "application/json"
                                   request.Method = "POST"
                                   Dim blob = New InvalidateBlob(Me)
                                   Dim stream = request.GetRequestStream()
                                   serializer.Serialize(New StreamWriter(stream), blob)
                                   stream.Close()
                                   Me.AccessToken = Nothing
                                   Me.AvailableProfiles = Nothing
                                   Me.SelectedProfile = New Profile
                               Catch e As WebException
                                   Dim stream = e.Response.GetResponseStream()
                                   Dim json = New StreamReader(stream).ReadToEnd()
                                   stream.Close()
                                   Dim jo = JObject.Parse(json)
                                   Throw New MinecraftAuthenticationException() With {.Error = jo.Value(Of String)("error"), .ErrorMessage = jo.Value(Of String)("errorMessage"), .Cause = jo.Value(Of String)("cause")}
                               End Try
                           End Sub)
        End Function

        Public Shadows Class MinecraftAuthenticationException
            Inherits Craft.Net.Client.Session.MinecraftAuthenticationException

            Public Sub New()

            End Sub

        End Class

        Private Class AuthenticationBlob
            Public Sub New(username As String, password As String, token As String)
                Me.Username = username
                Me.Password = password
                Me.ClientToken = token
                Me.Agent = New AgentBlob()
                Me.requestUser = True
            End Sub

            Public Class AgentBlob
                Public Sub New()
                    ' TODO: Update if needed, per https://twitter.com/sircmpwn/status/365306166638690304
                    Name = "Minecraft"
                    Version = 1
                End Sub

                <JsonProperty("name")> _
                Public Property Name() As String
                    Get
                        Return m_Name
                    End Get
                    Set(value As String)
                        m_Name = value
                    End Set
                End Property
                Private m_Name As String
                <JsonProperty("version")> _
                Public Property Version() As Integer
                    Get
                        Return m_Version
                    End Get
                    Set(value As Integer)
                        m_Version = value
                    End Set
                End Property
                Private m_Version As Integer
            End Class

            <JsonProperty("agent")> _
            Public Property Agent() As AgentBlob
                Get
                    Return m_Agent
                End Get
                Set(value As AgentBlob)
                    m_Agent = value
                End Set
            End Property
            Private m_Agent As AgentBlob
            <JsonProperty("username")> _
            Public Property Username() As String
                Get
                    Return m_Username
                End Get
                Set(value As String)
                    m_Username = value
                End Set
            End Property
            Private m_Username As String
            <JsonProperty("password")> _
            Public Property Password() As String
                Get
                    Return m_Password
                End Get
                Set(value As String)
                    m_Password = value
                End Set
            End Property
            Private m_Password As String
            <JsonProperty("clientToken")> _
            Public Property ClientToken() As String
                Get
                    Return m_ClientToken
                End Get
                Set(value As String)
                    m_ClientToken = value
                End Set
            End Property
            Private m_ClientToken As String
            <JsonProperty("requestUser")> _
            Public Property requestUser() As Boolean
                Get
                    Return m_requestUser
                End Get
                Set(value As Boolean)
                    m_requestUser = value
                End Set
            End Property
            Private m_requestUser As Boolean
        End Class

        Private Class RefreshBlob
            Public Sub New(session As Session)
                AccessToken = session.AccessToken
                ClientToken = session.ClientToken
                SelectedProfile = session.SelectedProfile
                Me.requestUser = True
            End Sub

            <JsonProperty("accessToken")> _
            Public Property AccessToken() As String
                Get
                    Return m_AccessToken
                End Get
                Set(value As String)
                    m_AccessToken = value
                End Set
            End Property
            Private m_AccessToken As String
            <JsonProperty("clientToken")> _
            Public Property ClientToken() As String
                Get
                    Return m_ClientToken
                End Get
                Set(value As String)
                    m_ClientToken = value
                End Set
            End Property
            Private m_ClientToken As String
            <JsonProperty("selectedProfile")> _
            Public Property SelectedProfile() As Profile
                Get
                    Return m_SelectedProfile
                End Get
                Set(value As Profile)
                    m_SelectedProfile = value
                End Set
            End Property
            Private m_SelectedProfile As Profile
            <JsonProperty("requestUser")> _
            Public Property requestUser() As Boolean
                Get
                    Return m_requestUser
                End Get
                Set(value As Boolean)
                    m_requestUser = value
                End Set
            End Property
            Private m_requestUser As Boolean
        End Class

        Private Class InvalidateBlob
            Public Sub New(session As Session)
                AccessToken = session.AccessToken
                ClientToken = session.ClientToken
            End Sub

            <JsonProperty("accessToken")> _
            Public Property AccessToken() As String
                Get
                    Return m_AccessToken
                End Get
                Set(value As String)
                    m_AccessToken = value
                End Set
            End Property
            Private m_AccessToken As String
            <JsonProperty("clientToken")> _
            Public Property ClientToken() As String
                Get
                    Return m_ClientToken
                End Get
                Set(value As String)
                    m_ClientToken = value
                End Set
            End Property
            Private m_ClientToken As String
        End Class

        <JsonProperty("user")>
        Public Shadows Property User As MojangUser
            Get
                Return m_User
            End Get
            Set(value As MojangUser)
                m_User = value
            End Set
        End Property
        Private m_User As MojangUser

        Public Function ToAccount() As authenticationDatabase.Account
            Return New authenticationDatabase.Account() With {.accessToken = AccessToken,
                                                                                       .displayName = SelectedProfile.Name,
                                                                                       .userid = User.id,
                                                                                       .username = UserName,
                                                                                       .uuid = SelectedProfile.Id,
                                                                                       .userProperties = User.properties}
        End Function

        Public Shadows Class MojangUser
            <JsonProperty("id")>
            Public Property Id As String
            <JsonProperty("properties")>
            Public Property Properties() As IList(Of authenticationDatabase.Userproperty)

        End Class
    End Class
End Namespace