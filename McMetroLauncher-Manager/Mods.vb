Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.IO

Public Class Mods
    Public Shared o As String
    Public Shared modsjo As JObject
    Private Shared list_needed As IList(Of String) = New List(Of String)

    Public Shared Sub Load()

        Dim standartprofile As New JObject(
                New JProperty("forgemods",
                    New JObject))
        If IO.File.Exists(modsfile) = False Then
            o = Nothing
        Else
            o = File.ReadAllText(modsfile)
        End If
        If o = Nothing Then
            'StandartProfile schreiben
            File.WriteAllText(modsfile, standartprofile.ToString)
        End If
        o = File.ReadAllText(modsfile)
        modsjo = JObject.Parse(o)

    End Sub

    Public Shared Function Get_Mods(ByVal version As String, modsfolder As String) As IList(Of ForgeMod)
        If modsjo("forgemods").HasValues = True Then
            Dim list As IList(Of ForgeMod) = New List(Of ForgeMod)
            If modsjo.Value(Of JObject)("forgemods").Properties.Select(Function(p) p.Name).Contains(version) Then
                For i = 0 To modsjo("forgemods")(version).Value(Of JArray).Count - 1
                    Dim installed As Boolean
                    Dim Struktur As String
                    Dim extension As String = Mods.extensionAt(version, i)
                    If version >= "1.6.4" = True Then
                        Struktur = version & "\" & Mods.idAt(version, i) & "." & extension
                    Else
                        Struktur = version & "-" & Mods.idAt(version, i) & "." & extension
                    End If
                    If File.Exists(modsfolder & "\" & Struktur) = True Then
                        installed = True
                    Else
                        installed = False
                    End If
                    list.Add(New ForgeMod(Mods.NameAt(version, i), Mods.AutorAt(version, i), version, Mods.descriptionAt(version, i), Mods.downloadlinkAt(version, i), Mods.videoAt(version, i), Mods.websiteAt(version, i), Mods.idAt(version, i), extension, Mods.needed_modsAt(version, i), installed))
                Next
            End If
            Return list
        Else
            Return New List(Of ForgeMod)
        End If

    End Function

    Public Shared Function NameAt(ByVal version As String, index As Integer) As String
        Dim versionsinfo As String = modsjo("forgemods")(version).ElementAt(index).Value(Of String)("name").ToString
        Return versionsinfo
    End Function

    Public Shared Function AutorAt(ByVal version As String, index As Integer) As String
        Dim versionsinfo As String = modsjo("forgemods")(version).ElementAt(index).Value(Of String)("autor").ToString
        Return versionsinfo
    End Function

    Public Shared Function descriptionAt(ByVal version As String, index As Integer) As String
        Dim versionsinfo As String = modsjo("forgemods")(version).ElementAt(index).Value(Of String)("description").ToString
        Return versionsinfo
    End Function

    Public Shared Function websiteAt(ByVal version As String, index As Integer) As String
        Dim versionsinfo As String = modsjo("forgemods")(version).ElementAt(index).Value(Of String)("website").ToString
        Return versionsinfo
    End Function

    Public Shared Function downloadlinkAt(ByVal version As String, index As Integer) As String
        Dim versionsinfo As String = modsjo("forgemods")(version).ElementAt(index).Value(Of String)("downloadlink").ToString
        Return versionsinfo
    End Function

    Public Shared Function idAt(ByVal version As String, index As Integer) As String
        Dim versionsinfo As String = modsjo("forgemods")(version).ElementAt(index).Value(Of String)("id").ToString
        Return versionsinfo
    End Function

    Public Shared Function extensionAt(ByVal version As String, index As Integer) As String
        Dim versionsinfo As String = modsjo("forgemods")(version).ElementAt(index).Value(Of String)("extension").ToString
        Return versionsinfo
    End Function

    Public Shared Function videoAt(ByVal version As String, index As Integer) As String
        Dim versionsinfo As String = modsjo("forgemods")(version).ElementAt(index).Value(Of String)("video").ToString
        Return versionsinfo
    End Function

    Public Shared Function Get_ModVersions() As IList(Of String)
        Dim modtoken As JObject = modsjo("forgemods").Value(Of JObject)()
        Dim modversions As List(Of String) = modtoken.Properties().ToList.Select(Function(p) p.Name).ToList()
        Return modversions
    End Function

    Public Shared Sub Add(ByVal ForgeMod As ForgeMod)
        Dim modtoken As JObject = modsjo("forgemods").Value(Of JObject)()
        Dim modversions As List(Of String) = modtoken.Properties().[Select](Function(p) p.Name).ToList
        Dim modarray As JArray
        If modversions.Contains(ForgeMod.version) = True Then
            modarray = CType(modsjo("forgemods")(ForgeMod.version), JArray)
        Else
            modarray = New JArray
        End If
        Dim modproperties As JObject = New JObject

        modproperties.Add(New JProperty("name", ForgeMod.name))
        modproperties.Add(New JProperty("autor", ForgeMod.autor))
        modproperties.Add(New JProperty("description", ForgeMod.description))
        modproperties.Add(New JProperty("downloadlink", ForgeMod.Downloadlink))
        modproperties.Add(New JProperty("video", ForgeMod.video))
        modproperties.Add(New JProperty("website", ForgeMod.Website))
        modproperties.Add(New JProperty("id", ForgeMod.id))
        modproperties.Add(New JProperty("extension", ForgeMod.extension))

        If ForgeMod.needed_mods.Count > 0 Then
            modproperties.Add(New JProperty("needed_mods", New JArray(ForgeMod.needed_mods.Select(Function(p) p.ToString))))
        End If

        modarray.Add(modproperties)
        Dim versiontoken As JProperty = New JProperty(ForgeMod.version, modarray)
        If modversions.Contains(ForgeMod.version) = False Then
            modtoken.Add(versiontoken)
        Else
            modtoken(ForgeMod.version).Replace(modarray)
        End If
        modsjo("forgemods").Replace(modtoken)
        File.WriteAllText(modsfile, modsjo.ToString)
    End Sub

    Public Shared Sub Edit(ByVal editedmodindex As Integer, editedmodversion As String, ForgeMod As ForgeMod)
        modsjo("forgemods").Value(Of JArray)(editedmodversion).RemoveAt(editedmodindex)
        IO.File.WriteAllText(modsfile, modsjo.ToString)
        Add(ForgeMod)

        'Dim modproperties As JObject = New JObject
        'Dim modarray As JArray = CType(modsjo("forgemods")(editedmodversion), JArray)
        'modproperties.Add(New JProperty("name", ForgeMod.name))
        'modproperties.Add(New JProperty("autor", ForgeMod.autor))
        'modproperties.Add(New JProperty("description", ForgeMod.description))
        'modproperties.Add(New JProperty("downloadlink", ForgeMod.Downloadlink))
        'modproperties.Add(New JProperty("video", ForgeMod.video))
        'modproperties.Add(New JProperty("website", ForgeMod.Website))
        'If ForgeMod.needed_mods.Count > 0 Then
        '    modproperties.Add(New JProperty("needed_mods", New JArray(ForgeMod.needed_mods)))
        'End If
        'modarray.RemoveAt(editedmodindex)
        'modarray.Add(modproperties)
        ''modarray.OrderByDescending(Function(p) p("name").ToString)
        'Dim versionstoken As New JProperty(editedmodversion, modarray)
        'modsjo("forgemods").Value(Of JObject).Property(editedmodversion).Replace(versionstoken)
        'File.WriteAllText(modsfile, modsjo.ToString)
    End Sub

    Public Shared Function Name(ByVal Modname As String, Version As String) As Integer
        Dim modarray As JArray = CType(modsjo("forgemods")(Version), JArray)
        For i = 0 To modarray.Count - 1

            Dim getmodname As String = NameAt(Version, i)
            If getmodname = Modname Then
                Return i
                Exit Function
            End If
        Next
        Throw New InvalidOperationException("Der angegebene ModName wurde nicht gefunden")
    End Function

    Public Shared Sub RemoveAt(ByVal modversion As String, modindex As Integer)
        modsjo("forgemods").Value(Of JArray)(modversion).RemoveAt(modindex)
        If modsjo("forgemods")(modversion).Count = 0 Then
            modsjo.Value(Of JObject)("forgemods").Property(modversion).Remove()
        End If
        File.WriteAllText(modsfile, modsjo.ToString)
        Get_ModVersions()
    End Sub

    Public Shared Sub Order()
        modsjo.Value(Of JObject)("forgemods").Properties.OrderByDescending(Function(p) p.Name.ToString)
        For Each version As String In Get_ModVersions()
            modsjo.Value(Of JObject)("forgemods").Property(version).OrderByDescending(Function(p) p("name").ToString)
        Next
        MessageBox.Show(modsjo.ToString)
    End Sub

    Public Shared Function All_Needed_Mods(ByVal Modname As String, Version As String) As IList(Of String)
        list_needed.Clear()
        Get_needed_Mods(Modname, Version)
        If list_needed.Count > 0 Then
            Return list_needed
        Else
            Return New List(Of String)
        End If
    End Function

    Private Shared Sub Get_needed_Mods(ByVal Modname As String, Version As String)
        Dim ls As IList(Of String) = Mods.needed_modsAt(Version, Mods.Name(Modname, Version))
        If ls.Count > 0 Then
            For Each item As String In ls
                If list_needed.Contains(item) = False Then
                    list_needed.Add(item)
                End If
                Get_needed_Mods(item, Version)
            Next
        End If
    End Sub

    Public Shared Function needed_modsAt(ByVal version As String, index As Integer) As IList(Of String)
        Dim list As IList(Of String) = modsjo("forgemods")(version).ElementAt(index).Value(Of JObject)().Properties().Select(Function(p) p.Name).ToList
        If list.Contains("needed_mods") = True Then
            Dim ja As JArray = modsjo("forgemods")(version).ElementAt(index).Value(Of JArray)("needed_mods")
            Dim versionsinfo As IList(Of String) = ja.Values(Of String).ToList
            Return versionsinfo
        Else
            Return New List(Of String)
        End If
    End Function

    Public Shared Function modfolder_filenames() As IList(Of String)
        If IO.Directory.Exists(modsfolder) = True Then
            MessageBox.Show(String.Join(" | ", IO.Directory.EnumerateFiles(modsfolder)))
            MessageBox.Show(String.Join(" | ", IO.Directory.GetFiles(modsfolder)))
            Return IO.Directory.EnumerateFiles(modsfolder).ToList
        Else
            Return Nothing
        End If
    End Function
End Class

Public Class ForgeMod
    Private _name As String, _autor As String, _version As String, _video As String, _description As String, _downloadlink As String, _website As String, _id As String, _extension As String, _needed_mods As IList(Of String), _installed As Boolean

    Public Property name As String
        Get
            Return _name
        End Get
        Set(value As String)
            _name = value
        End Set
    End Property

    Public Property installed As Boolean
        Get
            Return _installed
        End Get
        Set(value As Boolean)
            _installed = value
        End Set
    End Property

    Public Property autor As String
        Get
            Return _autor
        End Get
        Set(value As String)
            _autor = value
        End Set
    End Property

    Public Property description As String
        Get
            Return _description
        End Get
        Set(value As String)
            _description = value
        End Set
    End Property

    Public Property video As String
        Get
            Return _video
        End Get
        Set(value As String)
            _video = value
        End Set
    End Property

    Public Property version As String
        Get
            Return _version
        End Get
        Set(value As String)
            _version = value
        End Set
    End Property

    Public Property downloadlink As String
        Get
            Return _downloadlink
        End Get
        Set(value As String)
            _downloadlink = value
        End Set
    End Property

    Public Property id As String
        Get
            Return _id
        End Get
        Set(value As String)
            _id = value
        End Set
    End Property

    Public Property extension As String
        Get
            Return _extension
        End Get
        Set(value As String)
            _extension = value
        End Set
    End Property

    Public Property website As String
        Get
            Return _website
        End Get
        Set(value As String)
            _website = value
        End Set
    End Property


    Public Property needed_mods As IList(Of String)
        Get
            Return _needed_mods
        End Get
        Set(value As IList(Of String))
            _needed_mods = value
        End Set
    End Property

    Public Sub New(name As String, autor As String, version As String, description As String, downloadlink As String, video As String, website As String, id As String, extension As String, needed_mods As IList(Of String), installed As Boolean)
        Me.name = name
        Me.description = description
        Me.downloadlink = downloadlink
        Me.video = video
        Me.version = version
        Me.website = website
        Me.id = id
        Me.extension = extension
        Me.needed_mods = needed_mods
        Me.autor = autor
        Me.installed = installed
    End Sub
End Class
