Imports System.Net
Imports System.IO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Module GlobalInfos
    Public Website As String = "http://patzleiner.net/"
    Public Versionsurl As String = "http://s3.amazonaws.com/Minecraft.Download/versions/versions.json"
    Public mcpfad As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\.minecraft"
    Public modsfile As String = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\modlist.json"
    Public outputjsonversions As String = mcpfad & "\cache\versions.json"
    Public modsfolder As String = mcpfad & "\mods"
    Public cachefolder As String = mcpfad & "\cache"
    Public modfileurl As String = Website & "download/modlist.json"
    Public downloadfilepath As String
    Public Versions As Versionslist
    Public Moditem As Modifications.Mod
    Public extensions As String() = {"jar", "zip", "litemod"}
    Public types As String() = {"forge", "liteloader"}
    Public languages As String() = {"en", "de"}
End Module

Public Class Manager
    Dim wc As New WebClient()
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
    End Function

    Private Sub Manager_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        wc.CancelAsync()
        Application.Current.Shutdown()
    End Sub

    Private Async Sub Manager_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        btn_forge.IsEnabled = False
        btn_mods.IsEnabled = False
        pb_download.IsIndeterminate = True
        pb_download.Visibility = Windows.Visibility.Visible
        If IO.Directory.Exists(cachefolder) = False Then
            IO.Directory.CreateDirectory(cachefolder)
        End If
        wc = New WebClient()
        Await wc.DownloadFileTaskAsync(New Uri(Versionsurl), outputjsonversions)
        If IO.File.Exists(modsfile) = False Then
            wc = New WebClient()
            Await wc.DownloadFileTaskAsync(New Uri(modfileurl), modsfile)
        End If
        Await Start()
    End Sub

    Private Async Function Start() As Task
        Await Modifications.Load()
        'Await Forge.Load
        Await LiteLoader.Load
        Downloads.Load()
        Await Versions_Load()
        'btn_forge.IsEnabled = True
        btn_mods.IsEnabled = True
        pb_download.IsIndeterminate = False
        pb_download.Visibility = Windows.Visibility.Hidden
    End Function

End Class
