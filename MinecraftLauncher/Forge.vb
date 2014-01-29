'Imports System.IO
'Imports Newtonsoft.Json.Linq
'Imports Newtonsoft.Json

'Public Class Forge

'    Public Shared Forgelist As IList(Of ForgeEintrag) = New List(Of ForgeEintrag)

'    Public Shared Async Function Load() As Task
'        Dim o As String = File.ReadAllText(modsfile)
'        Dim jo As JObject = JObject.Parse(o)
'        Forgelist = Await JsonConvert.DeserializeObjectAsync(Of IList(Of ForgeEintrag))(jo("forge").ToString)
'        Forgelist = Forgelist.OrderByDescending(Function(p) p.version).ToList
'    End Function

'    Public Shared Async Function Add(Forge As ForgeEintrag) As Task
'        Dim Forgeproperties As JObject = New JObject
'        Forgeproperties.Add(New JProperty("build", Forge.build))
'        Forgeproperties.Add(New JProperty("version", Forge.version))
'        Forgeproperties.Add(New JProperty("time", Forge.time))
'        Forgeproperties.Add(New JProperty("downloadlink", Forge.downloadLink))
'        Mods.modsjo.Value(Of JArray)("forge").Add(Forgeproperties)
'        Mods.modsjo = JObject.Parse(Mods.modsjo.Value(Of JArray)("forge").OrderByDescending(Function(p) p("version").ToString).ToString)
'        IO.File.WriteAllText(modsfile, Mods.modsjo.ToString)
'        Await Load()
'    End Function

'    Public Shared Async Function Edit(ByVal editedforgeindex As Integer, Forge As ForgeEintrag) As Task
'        Mods.modsjo.Value(Of JArray)("forge").RemoveAt(editedforgeindex)
'        'Mods.modsjo.Value(Of JArray)("forge").OrderByDescending(Function(p) p("name").ToString)
'        IO.File.WriteAllText(modsfile, Mods.modsjo.ToString)
'        Await Add(Forge)
'    End Function

'    Public Shared Async Function RemoveAt(ByVal index As Integer) As Task
'        Mods.modsjo.Value(Of JArray)("forge").RemoveAt(index)
'        IO.File.WriteAllText(modsfile, Mods.modsjo.ToString)
'        Await Load()
'    End Function

'End Class

'Public Class ForgeEintrag
'    Private _build As String, _downloadlink As String, _version As String, _time As String

'    Public Property time As String
'        Get
'            Return _time
'        End Get
'        Set(value As String)
'            _time = value
'        End Set
'    End Property

'    Public Property build As String
'        Get
'            Return _build
'        End Get
'        Set(value As String)
'            _build = value
'        End Set
'    End Property

'    Public Property downloadLink As String
'        Get
'            Return _downloadlink
'        End Get
'        Set(value As String)
'            _downloadlink = value
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

'    Public Sub New(build As String, version As String, time As String, dl As String)
'        Me.build = build
'        Me.downloadLink = dl
'        Me.version = version
'        Me.time = time
'    End Sub
'End Class