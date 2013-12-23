Imports Newtonsoft.Json.Linq

Public Class Forge

    Public Shared Function Get_Forge() As IList(Of ForgeEintrag)
        Dim list As IList(Of ForgeEintrag) = New List(Of ForgeEintrag)
        Dim builds As IList(Of String) = Mods.modsjo("forge").Value(Of JArray).Select(Function(p) p("build").ToString).ToList
        Dim versions As IList(Of String) = Mods.modsjo("forge").Value(Of JArray).Select(Function(p) p("version").ToString).ToList
        Dim time As IList(Of String) = Mods.modsjo("forge").Value(Of JArray).Select(Function(p) p("time").ToString).ToList
        Dim downloadlinks As IList(Of String) = Mods.modsjo("forge").Value(Of JArray).Select(Function(p) p("downloadlink").ToString).ToList

        For i = 0 To builds.Count - 1
            Dim frg As New ForgeEintrag(builds.Item(i).ToString, versions.Item(i).ToString, time.Item(i).ToString, downloadlinks.Item(i).ToString)
            list.Add(frg)
        Next
        Return list
    End Function

    Public Shared Function buildAt(index As Integer) As String
        Dim forgeinfo As String = Mods.modsjo("forge").ElementAt(index).Value(Of String)("build").ToString
        Return forgeinfo
    End Function

    Public Shared Function versionAt(index As Integer) As String
        Dim forgeinfo As String = Mods.modsjo("forge").ElementAt(index).Value(Of String)("version").ToString
        Return forgeinfo
    End Function

    Public Shared Function timeAt(index As Integer) As String
        Dim forgeinfo As String = Mods.modsjo("forge").ElementAt(index).Value(Of String)("time").ToString
        Return forgeinfo
    End Function

    Public Shared Function downloadlinkAt(index As Integer) As String
        Dim forgeinfo As String = Mods.modsjo("forge").ElementAt(index).Value(Of String)("downloadlink").ToString
        Return forgeinfo
    End Function

    Public Shared Function BuildIndex(Build As String) As Integer
        Dim forgearray As JArray = CType(Mods.modsjo("forge"), JArray)
        For i = 0 To forgearray.Count - 1

            Dim getforgeBuild As String = buildAt(i)
            If getforgeBuild = Build Then
                Return i
                Exit Function
            End If
        Next
        Throw New InvalidOperationException("Das angegebene ForgeBuild wurde nicht gefunden")
    End Function

    Public Shared Sub Add(Forge As ForgeEintrag)
        Dim Forgeproperties As JObject = New JObject
        Forgeproperties.Add(New JProperty("build", Forge.build))
        Forgeproperties.Add(New JProperty("version", Forge.version))
        Forgeproperties.Add(New JProperty("time", Forge.time))
        Forgeproperties.Add(New JProperty("downloadlink", Forge.downloadLink))
        Mods.modsjo.Value(Of JArray)("forge").Add(Forgeproperties)
        IO.File.WriteAllText(modsfile, Mods.modsjo.ToString)
    End Sub

    Public Shared Sub Edit(ByVal editedforgeindex As Integer, Forge As ForgeEintrag)
        Mods.modsjo.Value(Of JArray)("forge").RemoveAt(editedforgeindex)
        'Mods.modsjo.Value(Of JArray)("forge").OrderByDescending(Function(p) p("name").ToString)
        IO.File.WriteAllText(modsfile, Mods.modsjo.ToString)
        Add(Forge)
    End Sub

    Public Shared Sub RemoveAt(ByVal index As Integer)
        Mods.modsjo.Value(Of JArray)("forge").RemoveAt(index)
        IO.File.WriteAllText(modsfile, Mods.modsjo.ToString)
        Get_Forge()
    End Sub

End Class

Public Class ForgeEintrag
    Private _build As String, _downloadlink As String, _version As String, _time As String

    Public Property time As String
        Get
            Return _time
        End Get
        Set(value As String)
            _time = value
        End Set
    End Property

    Public Property build As String
        Get
            Return _build
        End Get
        Set(value As String)
            _build = value
        End Set
    End Property

    Public Property downloadLink As String
        Get
            Return _downloadlink
        End Get
        Set(value As String)
            _downloadlink = value
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

    Public Sub New(build As String, version As String, time As String, dl As String)
        Me.build = build
        Me.downloadLink = dl
        Me.version = version
        Me.time = time
    End Sub
End Class