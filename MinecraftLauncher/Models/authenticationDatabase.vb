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
        clientToken = jo("clientToken").ToString
        If jo.Properties.Select(Function(p) p.Name).Contains("authenticationDatabase") Then
            For Each item As String In jo.Value(Of JObject)("authenticationDatabase").Properties.Select(Function(p) p.Value.ToString)
                Dim account As Account = Await JsonConvert.DeserializeObjectAsync(Of Account)(item)
                'Nur wenn alle properties existieren hinzufügen:
                List.Add(account)
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
            Dim account As String = Await JsonConvert.SerializeObjectAsync(item, Formatting.Indented, New JsonSerializerSettings() With {.NullValueHandling = NullValueHandling.Ignore})
            accountlist.Add(New JProperty(item.uuid.Replace("-", ""), JObject.Parse(account)))
        Next
        jo("authenticationDatabase") = accountlist
        File.WriteAllText(launcher_profiles.FullName, jo.ToString(Formatting.Indented))
    End Function


    Public Class Account

        Public Property userProperties As IList(Of Userproperty)
        Public Property username As String
        Public Property accessToken As String
        Public Property userid As String
        'todo: make in the get a guid.parse
        Private m_uuid As String
        Public Property uuid As String
            Get
                Return m_uuid
            End Get
            Set(value As String)
                m_uuid = Guid.Parse(value).ToString
            End Set
        End Property
        Public Property displayName As String
    End Class

    Public Class Userproperty
        Public Property name As String
        Public Property value As String
    End Class
End Class
