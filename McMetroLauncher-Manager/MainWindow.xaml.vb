Imports System.Net

Module GlobaleVariablen
    Public Webspace As String = "http://youyougabbo.square7.ch/"
    Public Versionsurl As String = "http://s3.amazonaws.com/Minecraft.Download/versions/versions.json"
    Public mcpfad As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\.minecraft"
    Public modsfile As String = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\modlist.json"
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

    Private Sub Manager_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Mods.Load()
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

    Private Sub wcversions_DownloadFileCompleted(sender As Object, e As ComponentModel.AsyncCompletedEventArgs) Handles wcversions.DownloadFileCompleted
        If IO.File.Exists(modsfile) = False Then
            wc = New WebClient
            wc.DownloadFileAsync(New Uri(Webspace & "\minecraft\launcher\modlist.json"), modsfile)
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
