Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.IO

Public Structure Forge
    Public Shared BuildList As IList(Of ForgeBuild) = New List(Of ForgeBuild)
    Public Shared LegacyBuildList As IList(Of ForgeBuild) = New List(Of ForgeBuild)
    Public Shared ReadOnly Property ForgeList As IList(Of ForgeBuild)
        Get
            Dim ls As IList(Of ForgeBuild) = New List(Of ForgeBuild)
            For Each item As ForgeBuild In LegacyBuildList
                If ls.Select(Function(p) p.build).Contains(item.build) = False Then
                    ls.Add(item)
                End If
            Next
            For Each item As ForgeBuild In BuildList
                If ls.Select(Function(p) p.build).Contains(item.build) = False Then
                    ls.Add(item)
                End If
            Next
            Return ls
        End Get
    End Property
    Private Shared Async Function LoadBuilds() As Task
        BuildList.Clear()
        Dim jo As JObject = JObject.Parse(File.ReadAllText(Forgefile.FullName))
        For Each item As JProperty In jo("number")
            Dim build As ForgeBuild = Await JsonConvert.DeserializeObjectAsync(Of ForgeBuild)(item.Value.ToString)
            For Each file As JArray In item.Value.Value(Of JArray)("files")
                Dim forgefile As New ForgeBuild.ForgeFileitem
                forgefile.extension = file.Value(Of String)(0)
                forgefile.type = file.Value(Of String)(1)
                forgefile.hash = file.Value(Of String)(2)
                build.files.Add(forgefile)
            Next
            BuildList.Add(build)
        Next
    End Function
    Private Shared Async Function LoadLegacyBuilds() As Task
        LegacyBuildList.Clear()
        Dim jo As JObject = JObject.Parse(File.ReadAllText(Legacyforgefile.FullName))
        For Each item As JProperty In jo("number")
            Dim build As ForgeBuild = Await JsonConvert.DeserializeObjectAsync(Of ForgeBuild)(item.Value.ToString)
            For Each file As JObject In item.Value.Value(Of JArray)("files")
                Dim forgefile As New ForgeBuild.ForgeFileitem
                forgefile.extension = file.Value(Of String)("ext")
                forgefile.type = file.Value(Of String)("type")
                forgefile.hash = Nothing
                build.files.Add(forgefile)
            Next
            LegacyBuildList.Add(build)
        Next
    End Function

    Public Shared Async Function Load() As Task
        Await LoadBuilds()
        Await LoadLegacyBuilds()
    End Function

    Public Class ForgeBuild
        Public Property branch As String
        Public Property build As Integer
        <JsonIgnore>
        Public Property files As IList(Of ForgeFileitem)
        Public Property mcversion As String
        Public Property modified As Single
        Public Property version As String
        Public Sub New()
            files = New List(Of ForgeFileitem)
        End Sub
        Public Class ForgeFileitem
            Public Property extension As String
            Public Property type As String
            Public Property hash As String
            Public Sub New()

            End Sub
        End Class
    End Class

End Structure