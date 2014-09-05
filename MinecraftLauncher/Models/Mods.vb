Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.IO


Public Structure Modifications
    Public Shared ModList As IList(Of Modifications.Mod)
    Private Shared list_dependencies As IList(Of String) = New List(Of String)

    Public Shared Async Function SavetoFile(filename As String) As Task
        If ModList Is Nothing Then
            Exit Function
        End If
        ModList = ModList.OrderBy(Function(p) p.name).ToList
        For i As Integer = 0 To ModList.Count - 1
            ModList.Item(i).versions = ModList.Item(i).versions.OrderByDescending(Function(p) p.version).ToList()
            ModList.Item(i).descriptions = ModList.Item(i).descriptions.OrderBy(Function(p) p.id).ToList()
            For a As Integer = 0 To ModList.Item(i).versions.Count - 1
                If ModList.Item(i).versions.Item(a).dependencies IsNot Nothing Then
                    ModList.Item(i).versions.Item(a).dependencies = ModList.Item(i).versions.Item(a).dependencies.OrderBy(Function(p) p.ToString).ToList
                End If
            Next
        Next
        Dim strModList As JArray = JArray.Parse(Await JsonConvert.SerializeObjectAsync(ModList, Formatting.Indented, New JsonSerializerSettings() With {.NullValueHandling = NullValueHandling.Ignore, .DefaultValueHandling = DefaultValueHandling.Ignore}))
        Dim o As String = File.ReadAllText(filename)
        Dim jo As JObject = JObject.Parse(o)
        jo("mods") = strModList
        File.WriteAllText(filename, jo.ToString)
    End Function

    Public Shared Async Function Load() As Task
        Dim o As String = File.ReadAllText(modsfile.FullName)
        Dim jo As JObject = JObject.Parse(o)
        ModList = Await JsonConvert.DeserializeObjectAsync(Of IList(Of Modifications.Mod))(jo("mods").ToString)
    End Function

    Public Shared Sub Check_installed(modsfolder As String)
        For i = 0 To ModList.Count - 1
            With ModList.Item(i)
                Dim versions As IList(Of Modifications.Mod.Version) = New List(Of Modifications.Mod.Version)
                For Each version As Modifications.Mod.Version In .versions
                    Dim filename As String = Nothing
                    If version.version >= "1.6.4" Then
                        filename = version.version & "\" & version.version & "-" & .id & "." & version.extension
                    Else
                        filename = version.version & "-" & .id & "." & version.extension
                    End If
                    If File.Exists(Path.Combine(modsfolder, filename)) Then
                        version.installed = True
                    Else
                        version.installed = False
                    End If
                    versions.Add(version)
                Next
                ModList.Item(i).versions = versions
            End With
        Next
    End Sub

    Public Shared Function List_all_Mod_Vesions() As IList(Of String)
        Dim list As IList(Of String) = New List(Of String)
        For Each item As [Mod] In ModList
            For Each version As String In item.versions.Select(Function(p) p.version.ToString)
                If list.Contains(version) = False Then
                    list.Add(version)
                End If
            Next
        Next
        Return list.OrderBy(Function(p) New Version(p)).ToList
    End Function

    Public Shared Function Dependencies(ByVal modid As String, version As String) As IList(Of String)
        list_dependencies.Clear()
        Get_dependencies(modid, version)
        If list_dependencies.Count > 0 Then
            Return list_dependencies
        Else
            Return New List(Of String)
        End If
    End Function

    Private Shared Sub Get_dependencies(ByVal mod_id As String, version As String)
        'Dim ls As IList(Of String) = Mods.needed_modsAt(Version, Mods.Name(Modname, Version))
        If ModList.Where(Function(p) p.id = mod_id).First.versions.Where(Function(p) p.version = version).First.dependencies IsNot Nothing Then
            Dim ls As IList(Of String) = ModList.Where(Function(p) p.id = mod_id).First.versions.Where(Function(p) p.version = version).First.dependencies
            If ls.Count > 0 Then
                For Each item As String In ls
                    If list_dependencies.Contains(item) = False Then
                        list_dependencies.Add(item)
                    End If
                    Get_dependencies(item, version)
                Next
            End If
        End If
    End Sub

    Public Class [Mod]
        Public Sub New()
            Me.descriptions = New List(Of Description)
            Me.versions = New List(Of Version)
        End Sub

        Public Property name() As String
            Get
                Return m_name
            End Get
            Set(value As String)
                m_name = value
            End Set
        End Property
        Private m_name As String
        Public Property authors() As IList(Of String)
            Get
                Return m_authors
            End Get
            Set(value As IList(Of String))
                m_authors = value
            End Set
        End Property
        Private m_authors As IList(Of String)
        Public Property descriptions() As IList(Of Description)
            Get
                Return m_description
            End Get
            Set(value As IList(Of Description))
                m_description = value
            End Set
        End Property
        Private m_description As IList(Of Description)
        Public Property versions() As IList(Of Version)
            Get
                Return m_versions
            End Get
            Set(value As IList(Of Version))
                m_versions = value
            End Set
        End Property
        Private m_versions As IList(Of Version)
        Public Property video() As String
            Get
                Return m_video
            End Get
            Set(value As String)
                m_video = value
            End Set
        End Property
        Private m_video As String
        Public Property website() As String
            Get
                Return m_website
            End Get
            Set(value As String)
                m_website = value
            End Set
        End Property
        Private m_website As String
        Public Property id() As String
            Get
                Return m_id
            End Get
            Set(value As String)
                m_id = value
            End Set
        End Property
        Private m_id As String
        Public Property type() As String
            Get
                Return m_type
            End Get
            Set(value As String)
                m_type = value
            End Set
        End Property
        Private m_type As String
        Public Class Description
            Public Sub New()

            End Sub
            Public Property id() As String
                Get
                    Return m_id
                End Get
                Set(value As String)
                    m_id = value
                End Set
            End Property
            Private m_id As String
            Public Property text() As String
                Get
                    Return m_text
                End Get
                Set(value As String)
                    m_text = value
                End Set
            End Property
            Private m_text As String
        End Class
        Public Class Version
            Public Sub New()

            End Sub
            Public Property version() As String
                Get
                    Return m_version
                End Get
                Set(value As String)
                    m_version = value
                End Set
            End Property
            Private m_version As String
            Public Property dependencies() As IList(Of String)
                Get
                    Return m_dependencies
                End Get
                Set(value As IList(Of String))
                    m_dependencies = value
                End Set
            End Property
            Private m_dependencies As IList(Of String)
            Public Property downloadlink() As String
                Get
                    Return m_downloadlink
                End Get
                Set(value As String)
                    m_downloadlink = value
                End Set
            End Property
            Private m_downloadlink As String
            Public Property extension() As String
                Get
                    Return m_extension
                End Get
                Set(value As String)
                    m_extension = value
                End Set
            End Property
            Private m_extension As String
            <JsonIgnore>
            Public Property installed() As Boolean
                Get
                    Return m_installed
                End Get
                Set(value As Boolean)
                    m_installed = value
                End Set
            End Property
            Private m_installed As Boolean
        End Class
    End Class
End Structure

Public Structure LiteLoader

    Public Shared LiteLoaderList As IList(Of LiteloaderEintrag)

    Public Shared Async Function SavetoFile(Filename As String) As Task
        If LiteLoaderList Is Nothing Then
            Exit Function
        End If
        LiteLoaderList = LiteLoaderList.OrderByDescending(Function(p) p.version).ToList
        Dim strModList As String = Await JsonConvert.SerializeObjectAsync(LiteLoaderList)
        Dim o As String = File.ReadAllText(Filename)
        Dim jo As JObject = JObject.Parse(o)
        jo("mods") = strModList
        File.WriteAllText(Filename, jo.ToString)
    End Function

    Public Shared Async Function Load() As Task
        Dim o As String = File.ReadAllText(modsfile.FullName)
        Dim jo As JObject = JObject.Parse(o)
        LiteLoaderList = Await JsonConvert.DeserializeObjectAsync(Of IList(Of LiteloaderEintrag))(jo("liteloader").ToString)
    End Function

    Public Class LiteloaderEintrag
        Public Sub New()

        End Sub
        Public Property version() As String
            Get
                Return m_version
            End Get
            Set(value As String)
                m_version = value
            End Set
        End Property
        Private m_version As String
        Public Property downloadlink() As String
            Get
                Return m_downloadlink
            End Get
            Set(value As String)
                m_downloadlink = value
            End Set
        End Property
        Private m_downloadlink As String
    End Class


End Structure

Public Structure Downloads

    Public Shared Downloadsjo As JObject
    Public Shared Sub Load()
        Dim o As String = File.ReadAllText(modsfile.FullName)
        Dim jo As JObject = JObject.Parse(o)
        Downloadsjo = JObject.Parse(jo("downloads").ToString)
    End Sub

End Structure