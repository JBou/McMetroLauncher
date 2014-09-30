Imports System.IO
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json

Public Class authenticationDatabase
    Private Shared m_List As IList(Of Account) = New List(Of Account)
    Public Shared Property List As IList(Of Account)
        Get
            Return m_List
        End Get
        Set(value As IList(Of Account))
            m_List = value
        End Set
    End Property

    Private Shared m_clientToken As String
    Public Shared Property clientToken As String
        Get
            If m_clientToken = Nothing Then m_clientToken = Guid.NewGuid().ToString()
            Return m_clientToken
        End Get
        Set(value As String)
            m_clientToken = value
        End Set
    End Property

    Public Shared Async Function Load() As Task
        Await LoadFrom(launcher_profiles_json)
    End Function

    Public Shared Async Function LoadFrom(launcher_profiles As FileInfo) As Task
        List.Clear()
        Dim o As String = File.ReadAllText(launcher_profiles.FullName)
        Dim jo As JObject = JObject.Parse(o)
        clientToken = If(jo.Properties.Select(Function(p) p.Name).Contains("clientToken"), jo("clientToken").ToString, Guid.NewGuid.ToString)
        If jo.Properties.Select(Function(p) p.Name).Contains("authenticationDatabase") Then
            For Each item As String In jo.Value(Of JObject)("authenticationDatabase").Properties.Select(Function(p) p.Value.ToString)
                Try
                    Dim account As Account = Await JsonConvert.DeserializeObjectAsync(Of Account)(item)
                    'Nur wenn alle properties existieren hinzufügen:
                    List.Add(account)
                Catch ex As JsonException
                    'TODO: Cannot load Account, Invalid format (uuid?)
                End Try
            Next
        End If
    End Function

    Public Shared Async Function Save() As Task
        Await SaveTo(launcher_profiles_json)
    End Function

    Public Shared Async Function SaveTo(launcher_profiles As FileInfo) As Task
        Dim o As String = File.ReadAllText(launcher_profiles.FullName)
        Dim jo As JObject = JObject.Parse(o)
        jo("clientToken") = clientToken
        Dim accountlist As New JObject
        For Each item As Account In List
            If accountlist.Properties.Select(Function(p) p.Name).Contains(item.uuid.Replace("-", "")) = False Then
                Dim account As String = Await JsonConvert.SerializeObjectAsync(item, Formatting.Indented, New JsonSerializerSettings() With {.NullValueHandling = NullValueHandling.Ignore})
                accountlist.Add(New JProperty(item.uuid.Replace("-", ""), JObject.Parse(account)))
            End If
        Next
        jo("authenticationDatabase") = accountlist
        File.WriteAllText(launcher_profiles.FullName, jo.ToString(Formatting.Indented))
    End Function


    Public Class Account
        Inherits PropertyChangedBase

        Private _userProperties As IList(Of Userproperty)
        <JsonProperty("userProperties")>
        Public Property userProperties As IList(Of Userproperty)
            Get
                Return _userProperties
            End Get
            Set(value As IList(Of Userproperty))
                SetProperty(value, _userProperties)
            End Set
        End Property
        Private _username As String
        <JsonProperty("username")>
        Public Property username As String
            Get
                Return _username
            End Get
            Set(value As String)
                SetProperty(value, _username)
            End Set
        End Property
        Private _accessToken As String
        <JsonProperty("accessToken")>
        Public Property accessToken As String
            Get
                Return _accessToken
            End Get
            Set(value As String)
                SetProperty(value, _accessToken)
            End Set
        End Property
        Private _userid As String
        <JsonProperty("userid")>
        Public Property userid As String
            Get
                Return _userid
            End Get
            Set(value As String)
                SetProperty(value, _userid)
            End Set
        End Property
        'todo: make in the get a guid.parse
        Private _uuid As String
        <JsonProperty("uuid")>
        Public Property uuid As String
            Get
                Return _uuid
            End Get
            Set(value As String)
                SetProperty(Guid.Parse(value).ToString, _uuid)
            End Set
        End Property
        Private _displayName As String
        <JsonProperty("displayName")>
        Public Property displayName As String
            Get
                Return _displayName
            End Get
            Set(value As String)
                SetProperty(value, _displayName)
            End Set
        End Property
    End Class

    Public Class Userproperty
        Inherits PropertyChangedBase
        Private _name As String
        <JsonProperty("name")>
        Public Property name As String
            Get
                Return _name
            End Get
            Set(value As String)
                SetProperty(value, _name)
            End Set
        End Property
        Private _value As String
        <JsonProperty("value")>
        Public Property value As String
            Get
                Return _value
            End Get
            Set(value As String)
                SetProperty(value, _value)
            End Set
        End Property
    End Class
End Class
