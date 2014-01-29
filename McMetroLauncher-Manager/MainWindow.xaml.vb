Imports System.Net
Imports System.IO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Module GlobalInfos
    Public Webspace As String = "http://youyougabbo.square7.ch/"
    Public Versionsurl As String = "http://s3.amazonaws.com/Minecraft.Download/versions/versions.json"
    Public mcpfad As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\.minecraft"
    Public modsfile As String = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\newmodlist.json"
    Public librariespath As String = mcpfad & "\libraries"
    Public assetspath As String = mcpfad & "\assets"
    Public resourcesfile As String = mcpfad & "\cache\minecraft_resources.xml"
    Public launcher_profiles_json As String = mcpfad & "\launcher_profiles.json"
    Public outputjsonversions As String = mcpfad & "\cache\versions.json"
    Public versionsidlist As IList(Of String) = New List(Of String)
    Public versiontypelist As IList(Of String) = New List(Of String)
    Public lastversionID As String
    Public Delegate Sub WriteA(ByVal Text As String)
    Public modsfolder As String = mcpfad & "\mods"
    Public cachefolder As String = mcpfad & "\cache"
    Public downloadfilepath As String
    Public Versions As Versionslist

End Module

Public Class Manager
    WithEvents wc As New WebClient
    WithEvents wcversions As New WebClient

    Public Sub ModManager()
        Dim ModManager As New ModManager
        ModManager.Show()
        Me.Hide()
    End Sub

    Public Sub ForgeManager()
        Dim ForgeManager As New ForgeManager
        ForgeManager.Show()
        Me.Hide()
    End Sub

    Public Async Function Versions_Load() As Task
        Dim o As String = File.ReadAllText(outputjsonversions)
        GlobalInfos.Versions = Await JsonConvert.DeserializeObjectAsync(Of Versionslist)(o)

        If IO.Directory.Exists(mcpfad & "\versions") = True Then
            Dim list_versionsdirectories As IEnumerable(Of String) = IO.Directory.GetDirectories(mcpfad & "\versions")
            Dim list_versions As IList(Of String) = New List(Of String)
            For Each version As String In list_versionsdirectories
                Dim versionname As String = IO.Path.GetFileName(version)
                If GlobalInfos.Versions.versions.Select(Function(p) p.id).Contains(versionname) = False Then
                    list_versions.Add(versionname)
                End If
            Next
            For Each Version As String In list_versions
                If File.Exists(mcpfad & "\versions\" & Version & "\" & Version & ".jar") And File.Exists(mcpfad & "\versions\" & Version & "\" & Version & ".json") = True Then
                    Dim jo As JObject = JObject.Parse(File.ReadAllText(mcpfad & "\versions\" & Version & "\" & Version & ".json"))
                    If jo("id").ToString = Version Then
                        Dim versionitem As New Versionslist.Version() With {
                            .id = jo("id").ToString,
                            .type = jo("type").ToString,
                            .time = jo("time").ToString,
                            .releaseTime = jo("releaseTime").ToString}
                        GlobalInfos.Versions.versions.Add(versionitem)
                    Else
                        'Falsche id wurde gefunden
                    End If
                End If
            Next
        End If
    End Function

    Private Async Sub Manager_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Await Modifications.Load()
        Await Versions_Load()
        btn_forge.IsEnabled = False
        btn_mods.IsEnabled = False
        pb_download.IsIndeterminate = True
        pb_download.Visibility = Windows.Visibility.Visible
        If IO.Directory.Exists(cachefolder) = False Then
            IO.Directory.CreateDirectory(cachefolder)
        End If
        wcversions = New WebClient
        wcversions.DownloadFileAsync(New Uri(Versionsurl), outputjsonversions)
    End Sub

    Private Async Sub wcversions_DownloadFileCompleted(sender As Object, e As ComponentModel.AsyncCompletedEventArgs) Handles wcversions.DownloadFileCompleted
        If IO.File.Exists(modsfile) = False Then
            wc = New WebClient
            Await wc.DownloadFileTaskAsync(New Uri(Webspace & "\minecraft\launcher\modlist.json"), modsfile)
        End If
        Start()
    End Sub

    Private Sub Start()
        btn_forge.IsEnabled = True
        btn_mods.IsEnabled = True
        pb_download.IsIndeterminate = False
        pb_download.Visibility = Windows.Visibility.Hidden
    End Sub

End Class
