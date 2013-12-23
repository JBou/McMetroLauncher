Imports System.Net
Imports System.IO
Imports Newtonsoft.Json.Linq
Imports updateSystemDotNet.updateController

Public Class SplashScreen
    WithEvents wcversionsstring As New WebClient
    WithEvents wcresources_xml As New WebClient
    WithEvents wcmodlist As New WebClient
    WithEvents wcupdate As New WebClient
    WithEvents wcversion As New WebClient
    WithEvents wcchangelog As New WebClient

    Dim resourcesurl As String = "https://s3.amazonaws.com/Minecraft.Resources"
    Dim Versionsurl As String = "http://s3.amazonaws.com/Minecraft.Download/versions/versions.json"
    Dim modfileurl As String = Website & "/download/modlist.json"
    Dim versionurl As String = Website & "/mcmetrolauncher/version.txt"
    Dim changelogurl As String = Website & "/mcmetrolauncher/changelog.txt"


    Function internetconnection() As Boolean
        Try
            My.Computer.Network.Ping("www.google.com")
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    Private Sub SplashScreen_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        Dim oAssembly As System.Reflection.AssemblyName = _
  System.Reflection.Assembly.GetExecutingAssembly().GetName
        ' Versionsnummer
        Dim sVersion As String = oAssembly.Version.ToString()

        ' Haupt-Versionsnummer
        Dim sMajor As String = oAssembly.Version.Major.ToString()
        ' Neben-Versionsnummern
        Dim sMinor As String = oAssembly.Version.Minor.ToString()
        ' Build-Nr.
        Dim sBuild As String = oAssembly.Version.Build.ToString()

        lbl_Version.Content = "Version " & sVersion
        If internetconnection() = True Then
        lbl_status.Content = "Lade Versions-Liste herunter"
        If My.Computer.FileSystem.DirectoryExists(mcpfad & "\cache") = False Then
            IO.Directory.CreateDirectory(mcpfad & "\cache")
        End If

        Dim standartprofile As New JObject(
New JProperty("profiles",
New JObject(
    New JProperty("Default",
        New JObject(
            New JProperty("name", "Default"))))),
New JProperty("selectedProfile", "Default"))
        Dim o As String
        If IO.File.Exists(launcher_profiles_json) = False Then
            o = Nothing
        Else
            o = File.ReadAllText(launcher_profiles_json)
        End If
        If o = Nothing Then
            'StandartProfile schreiben
            File.WriteAllText(launcher_profiles_json, standartprofile.ToString)
        End If

        Try
            wcversionsstring.DownloadFileAsync(New Uri(Versionsurl), outputjsonversions)
        Catch
        End Try
        Else
        lbl_statustitle.Content = "Fehler"
        lbl_status.Content = "Bitte überprüfe deine Internetverbindung!"
        End If

    End Sub

    Private Sub wcversionsstring_DownloadFileCompleted(sender As Object, e As ComponentModel.AsyncCompletedEventArgs) Handles wcversionsstring.DownloadFileCompleted
        Try
            lbl_status.Content = "Lade Resourcen-Liste herunter"
            wcresources_xml.DownloadFileAsync(New Uri(resourcesurl), resourcesfile)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub wcresources_xml_DownloadFileCompleted(sender As Object, e As ComponentModel.AsyncCompletedEventArgs) Handles wcresources_xml.DownloadFileCompleted
        Try
            lbl_status.Content = "Lade Mod-Liste herunter"
            wcmodlist.DownloadFileAsync(New Uri(modfileurl), modsfile)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub wcmodlist_DownloadFileCompleted(sender As Object, e As ComponentModel.AsyncCompletedEventArgs) Handles wcmodlist.DownloadFileCompleted
        Try
            wcversion.DownloadFileAsync(New Uri(versionurl), onlineversionfile)
            lbl_status.Content = "Prüfe auf Updates"
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub wcversion_DownloadFileCompleted(sender As Object, e As ComponentModel.AsyncCompletedEventArgs) Handles wcversion.DownloadFileCompleted
        Try
            wcchangelog.DownloadFileAsync(New Uri(changelogurl), changelogfile)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Sub Start()
        Try
            Versions.Load()
            Mods.Load()
            Dim MainWindow As New MainWindow
            MainWindow.Show()
            Me.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub wcchangelog_DownloadFileCompleted(sender As Object, e As ComponentModel.AsyncCompletedEventArgs) Handles wcchangelog.DownloadFileCompleted
        Start()
    End Sub
End Class

