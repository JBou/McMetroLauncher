Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.IO


Public Structure Modifications
    Public Shared ModList As IList(Of Modifications.Mod)
    Private Shared list_dependencies As IList(Of String) = New List(Of String)

    Public Shared Async Function SavetoFile(filename As FileInfo) As Task
        If ModList Is Nothing Then
            Exit Function
        End If
        ModList = ModList.OrderBy(Function(p) p.name).ToList
        For i As Integer = 0 To ModList.Count - 1
            ModList.Item(i).versions = ModList.Item(i).versions.OrderByDescending(Function(p) New Version(p.version)).ToList()
            ModList.Item(i).descriptions = ModList.Item(i).descriptions.OrderBy(Function(p) p.id).ToList()
            For a As Integer = 0 To ModList.Item(i).versions.Count - 1
                If ModList.Item(i).versions.Item(a).dependencies IsNot Nothing Then
                    ModList.Item(i).versions.Item(a).dependencies = ModList.Item(i).versions.Item(a).dependencies.OrderBy(Function(p) p).ToList
                End If
            Next
        Next
        Dim strModList As JArray = JArray.Parse(Await JsonConvert.SerializeObjectAsync(ModList, Formatting.Indented, New JsonSerializerSettings() With {.NullValueHandling = NullValueHandling.Ignore, .DefaultValueHandling = DefaultValueHandling.Ignore}))
        Dim o As String = File.ReadAllText(filename.FullName)
        Dim jo As JObject = JObject.Parse(o)
        jo("mods") = strModList
        File.WriteAllText(filename.FullName, jo.ToString)
        Dim s As String = String.Concat(Path.GetDirectoryName(filename.FullName), Path.DirectorySeparatorChar, Path.GetFileNameWithoutExtension(filename.FullName), ".min", Path.GetExtension(filename.FullName))
        File.WriteAllText(s, jo.ToString(Formatting.None))
    End Function

    Public Shared Async Function Load() As Task
        Dim o As String = File.ReadAllText(modsfile)
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

Public Structure Forge

    Public Shared ForgeList As IList(Of Forgeeintrag)

    Public Shared Async Function SavetoFile(Filename As String) As Task
        If ForgeList Is Nothing Then
            Exit Function
        End If
        ForgeList = ForgeList.OrderByDescending(Function(p) New Version(p.build)).ToList
        Dim strModList As JArray = JArray.Parse(Await JsonConvert.SerializeObjectAsync(ForgeList))
        Dim o As String = File.ReadAllText(Filename)
        Dim jo As JObject = JObject.Parse(o)
        jo("forge") = strModList
        File.WriteAllText(Filename, jo.ToString)
    End Function

    Public Shared Async Function Load() As Task
        Dim o As String = File.ReadAllText(modsfile)
        Dim jo As JObject = JObject.Parse(o)
        ForgeList = Await JsonConvert.DeserializeObjectAsync(Of IList(Of Forgeeintrag))(jo("forge").ToString)
    End Function

    Public Class Forgeeintrag
        Public Sub New()

        End Sub
        Public Property build() As String
            Get
                Return m_build
            End Get
            Set(value As String)
                m_build = value
            End Set
        End Property
        Private m_build As String
        Public Property version() As String
            Get
                Return m_version
            End Get
            Set(value As String)
                m_version = value
            End Set
        End Property
        Private m_version As String
        Public Property time() As String
            Get
                Return m_time
            End Get
            Set(value As String)
                m_time = value
            End Set
        End Property
        Private m_time As String
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
        Dim o As String = File.ReadAllText(modsfile)
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
        Dim o As String = File.ReadAllText(modsfile)
        Dim jo As JObject = JObject.Parse(o)
        Downloadsjo = JObject.Parse(jo("downloads").ToString)
    End Sub

End Structure

'Public Class Mods
'    Public Shared o As String
'    Public Shared modsjo As JObject
'    Private Shared list_needed As IList(Of String) = New List(Of String)

'    Public Shared Sub Load()

'        Dim standartprofile As New JObject(
'                New JProperty("mods",
'                    New JObject))
'        If IO.File.Exists(modsfile) = False Then
'            o = Nothing
'        Else
'            o = File.ReadAllText(modsfile)
'        End If
'        If o = Nothing Then
'            'StandartProfile schreiben
'            File.WriteAllText(modsfile, standartprofile.ToString)
'        End If
'        o = File.ReadAllText(modsfile)
'        modsjo = JObject.Parse(o)

'    End Sub

'    Public Shared Function Get_Mods(ByVal version As String, modsfolder As String) As IList(Of ForgeMod)
'        If modsjo("mods").HasValues = True Then
'            Dim list As IList(Of ForgeMod) = New List(Of ForgeMod)
'            For i = 0 To modsjo("mods")(version).Value(Of JArray).Count - 1
'                Dim installed As Boolean
'                Dim Struktur As String
'                Dim extension As String = Mods.extensionAt(version, i)
'                If version >= "1.6.4" = True Then
'                    Struktur = version & "\" & Mods.idAt(version, i) & "." & extension
'                Else
'                    Struktur = version & "-" & Mods.idAt(version, i) & "." & extension
'                End If
'                If File.Exists(modsfolder & "\" & Struktur) = True Then
'                    installed = True
'                Else
'                    installed = False
'                End If
'                list.Add(New ForgeMod(Mods.NameAt(version, i), Mods.authorAt(version, i), version, Mods.descriptionAt(version, i), Mods.downloadlinkAt(version, i), Mods.videoAt(version, i), Mods.websiteAt(version, i), Mods.idAt(version, i), extension,Mods.typeAt(version, i), Mods.needed_modsAt(version, i), installed))
'            Next
'            list = list.OrderBy(Function(p) p.name).ToList
'            Return list
'        Else
'            Return New List(Of ForgeMod)
'        End If

'    End Function

'    Public Shared Function NameAt(ByVal version As String, index As Integer) As String
'        Dim versionsinfo As String = modsjo("mods")(version).ElementAt(index).Value(Of String)("name").ToString
'        Return versionsinfo
'    End Function

'    Public Shared Function authorAt(ByVal version As String, index As Integer) As String
'        Dim versionsinfo As String = modsjo("mods")(version).ElementAt(index).Value(Of String)("author").ToString
'        Return versionsinfo
'    End Function

'    Public Shared Function descriptionAt(ByVal version As String, index As Integer) As String
'        Dim versionsinfo As String = modsjo("mods")(version).ElementAt(index).Value(Of String)("description").ToString
'        Return versionsinfo
'    End Function

'    Public Shared Function websiteAt(ByVal version As String, index As Integer) As String
'        Dim versionsinfo As String = modsjo("mods")(version).ElementAt(index).Value(Of String)("website").ToString
'        Return versionsinfo
'    End Function

'    Public Shared Function downloadlinkAt(ByVal version As String, index As Integer) As String
'        Dim versionsinfo As String = modsjo("mods")(version).ElementAt(index).Value(Of String)("downloadlink").ToString
'        Return versionsinfo
'    End Function

'    Public Shared Function idAt(ByVal version As String, index As Integer) As String
'        Dim versionsinfo As String = modsjo("mods")(version).ElementAt(index).Value(Of String)("id").ToString
'        Return versionsinfo
'    End Function

'    Public Shared Function extensionAt(ByVal version As String, index As Integer) As String
'        Dim versionsinfo As String = modsjo("mods")(version).ElementAt(index).Value(Of String)("extension").ToString
'        Return versionsinfo
'    End Function

'    Public Shared Function typeAt(ByVal version As String, index As Integer) As String
'        Dim versionsinfo As String = modsjo("mods")(version).ElementAt(index).Value(Of String)("type").ToString
'        Return versionsinfo
'    End Function

'    Public Shared Function videoAt(ByVal version As String, index As Integer) As String
'        Dim versionsinfo As String = modsjo("mods")(version).ElementAt(index).Value(Of String)("video").ToString
'        Return versionsinfo
'    End Function

'    Public Shared Function Get_ModVersions() As IList(Of String)
'        Dim modtoken As JObject = modsjo("mods").Value(Of JObject)()
'        Dim modversions As List(Of String) = modtoken.Properties().ToList.Select(Function(p) p.Name).ToList()
'        Return modversions
'    End Function

'    Public Shared Sub Add(ByVal ForgeMod As ForgeMod)
'        Dim modtoken As JObject = modsjo("mods").Value(Of JObject)()
'        Dim modversions As List(Of String) = modtoken.Properties().[Select](Function(p) p.Name).ToList
'        Dim modarray As JArray
'        If modversions.Contains(ForgeMod.version) = True Then
'            modarray = CType(modsjo("mods")(ForgeMod.version), JArray)
'        Else
'            modarray = New JArray
'        End If
'        Dim modproperties As JObject = New JObject

'        modproperties.Add(New JProperty("name", ForgeMod.name))
'        modproperties.Add(New JProperty("author", ForgeMod.author))
'        modproperties.Add(New JProperty("description", ForgeMod.description))
'        modproperties.Add(New JProperty("downloadlink", ForgeMod.Downloadlink))
'        modproperties.Add(New JProperty("video", ForgeMod.video))
'        modproperties.Add(New JProperty("website", ForgeMod.Website))
'        modproperties.Add(New JProperty("id", ForgeMod.id))
'        modproperties.Add(New JProperty("extension", ForgeMod.extension))
'        modproperties.Add(New JProperty("type", ForgeMod.type))

'        If ForgeMod.needed_mods.Count > 0 Then
'            modproperties.Add(New JProperty("needed_mods", New JArray(ForgeMod.needed_mods.Select(Function(p) p.ToString))))
'        End If

'        modarray.Add(modproperties)
'        Dim versiontoken As JProperty = New JProperty(ForgeMod.version, modarray)
'        If modversions.Contains(ForgeMod.version) = False Then
'            modtoken.Add(versiontoken)
'        Else
'            modtoken(ForgeMod.version).Replace(modarray)
'        End If
'        modsjo("mods").Replace(modtoken)
'        Mods.modsjo = JObject.Parse(Mods.modsjo.Value(Of JArray)("mods").OrderBy(Function(p) p("name").ToString).ToString)
'        File.WriteAllText(modsfile, modsjo.ToString)
'    End Sub

'    Public Shared Sub Edit(ByVal editedmodindex As Integer, editedmodversion As String, ForgeMod As ForgeMod)
'        modsjo("mods").Value(Of JArray)(editedmodversion).RemoveAt(editedmodindex)
'        IO.File.WriteAllText(modsfile, modsjo.ToString)
'        Add(ForgeMod)

'        'Dim modproperties As JObject = New JObject
'        'Dim modarray As JArray = CType(modsjo("mods")(editedmodversion), JArray)
'        'modproperties.Add(New JProperty("name", ForgeMod.name))
'        'modproperties.Add(New JProperty("author", ForgeMod.author))
'        'modproperties.Add(New JProperty("description", ForgeMod.description))
'        'modproperties.Add(New JProperty("downloadlink", ForgeMod.Downloadlink))
'        'modproperties.Add(New JProperty("video", ForgeMod.video))
'        'modproperties.Add(New JProperty("website", ForgeMod.Website))
'        'If ForgeMod.needed_mods.Count > 0 Then
'        '    modproperties.Add(New JProperty("needed_mods", New JArray(ForgeMod.needed_mods)))
'        'End If
'        'modarray.RemoveAt(editedmodindex)
'        'modarray.Add(modproperties)
'        ''modarray.OrderByDescending(Function(p) p("name").ToString)
'        'Dim versionstoken As New JProperty(editedmodversion, modarray)
'        'modsjo("mods").Value(Of JObject).Property(editedmodversion).Replace(versionstoken)
'        'File.WriteAllText(modsfile, modsjo.ToString)
'    End Sub

'    Public Shared Function Name(ByVal Modname As String, Version As String) As Integer
'        Dim modarray As JArray = CType(modsjo("mods")(Version), JArray)
'        For i = 0 To modarray.Count - 1

'            Dim getmodname As String = NameAt(Version, i)
'            If getmodname = Modname Then
'                Return i
'                Exit Function
'            End If
'        Next
'        Throw New InvalidOperationException("Der angegebene ModName wurde nicht gefunden")
'    End Function

'    Public Shared Sub RemoveAt(ByVal modversion As String, modindex As Integer)
'        modsjo("mods").Value(Of JArray)(modversion).RemoveAt(modindex)
'        If modsjo("mods")(modversion).Count = 0 Then
'            modsjo.Value(Of JObject)("mods").Property(modversion).Remove()
'        End If
'        File.WriteAllText(modsfile, modsjo.ToString)
'        Get_ModVersions()
'    End Sub

'    Public Shared Sub Order()
'        modsjo.Value(Of JObject)("mods").Properties.OrderByDescending(Function(p) p.Name.ToString)
'        For Each version As String In Get_ModVersions()
'            modsjo.Value(Of JObject)("mods").Property(version).OrderByDescending(Function(p) p("name").ToString)
'        Next
'        'MessageBox.Show(modsjo.ToString)
'    End Sub

'    Public Shared Function All_Needed_Mods(ByVal Modname As String, Version As String) As IList(Of String)
'        list_needed.Clear()
'        Get_needed_Mods(Modname, Version)
'        If list_needed.Count > 0 Then
'            Return list_needed
'        Else
'            Return New List(Of String)
'        End If
'    End Function

'    Private Shared Sub Get_needed_Mods(ByVal Modname As String, Version As String)
'        Dim ls As IList(Of String) = Mods.needed_modsAt(Version, Mods.Name(Modname, Version))
'        If ls.Count > 0 Then
'            For Each item As String In ls
'                If list_needed.Contains(item) = False Then
'                    list_needed.Add(item)
'                End If
'                Get_needed_Mods(item, Version)
'            Next
'        End If
'    End Sub

'    Public Shared Function needed_modsAt(ByVal version As String, index As Integer) As IList(Of String)
'        Dim list As IList(Of String) = modsjo("mods")(version).ElementAt(index).Value(Of JObject)().Properties().Select(Function(p) p.Name).ToList
'        If list.Contains("needed_mods") = True Then
'            Dim ja As JArray = modsjo("mods")(version).ElementAt(index).Value(Of JArray)("needed_mods")
'            Dim versionsinfo As IList(Of String) = ja.Values(Of String).ToList
'            Return versionsinfo
'        Else
'            Return New List(Of String)
'        End If
'    End Function

'    Public Shared Function modfolder_filenames() As IList(Of String)
'        If IO.Directory.Exists(modsfolder) = True Then
'            'MessageBox.Show(String.Join(" | ", IO.Directory.EnumerateFiles(modsfolder)))
'            'MessageBox.Show(String.Join(" | ", IO.Directory.GetFiles(modsfolder)))
'            Return IO.Directory.EnumerateFiles(modsfolder).ToList
'        Else
'            Return Nothing
'        End If
'    End Function
'End Class

'Public Class ForgeMod
'    Private _name As String, _author As String, _version As String, _video As String, _description As String, _downloadlink As String, _website As String, _id As String, _extension As String, _type As String, _needed_mods As IList(Of String), _installed As Boolean

'    Public Property name As String
'        Get
'            Return _name
'        End Get
'        Set(value As String)
'            _name = value
'        End Set
'    End Property

'    Public Property installed As Boolean
'        Get
'            Return _installed
'        End Get
'        Set(value As Boolean)
'            _installed = value
'        End Set
'    End Property

'    Public Property author As String
'        Get
'            Return _author
'        End Get
'        Set(value As String)
'            _author = value
'        End Set
'    End Property

'    Public Property description As String
'        Get
'            Return _description
'        End Get
'        Set(value As String)
'            _description = value
'        End Set
'    End Property

'    Public Property video As String
'        Get
'            Return _video
'        End Get
'        Set(value As String)
'            _video = value
'        End Set
'    End Property

'    Public Property version As String
'        Get
'            Return _version
'        End Get
'        Set(value As String)
'            _version = value
'        End Set
'    End Property

'    Public Property downloadlink As String
'        Get
'            Return _downloadlink
'        End Get
'        Set(value As String)
'            _downloadlink = value
'        End Set
'    End Property

'    Public Property id As String
'        Get
'            Return _id
'        End Get
'        Set(value As String)
'            _id = value
'        End Set
'    End Property

'    Public Property extension As String
'        Get
'            Return _extension
'        End Get
'        Set(value As String)
'            _extension = value
'        End Set
'    End Property

'    Public Property type As String
'        Get
'            Return _type
'        End Get
'        Set(value As String)
'            _type = value
'        End Set
'    End Property

'    Public Property website As String
'        Get
'            Return _website
'        End Get
'        Set(value As String)
'            _website = value
'        End Set
'    End Property


'    Public Property needed_mods As IList(Of String)
'        Get
'            Return _needed_mods
'        End Get
'        Set(value As IList(Of String))
'            _needed_mods = value
'        End Set
'    End Property

'    Public Sub New(name As String, author As String, version As String, description As String, downloadlink As String, video As String, website As String, id As String, extension As String, type As String, needed_mods As IList(Of String), installed As Boolean)
'        Me.name = name
'        Me.description = description
'        Me.downloadlink = downloadlink
'        Me.video = video
'        Me.version = version
'        Me.website = website
'        Me.id = id
'        Me.extension = extension
'        Me.type = type
'        Me.needed_mods = needed_mods
'        Me.author = author
'        Me.installed = installed
'    End Sub
'End Class
