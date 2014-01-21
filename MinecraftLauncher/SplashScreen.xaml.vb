Imports System.Net
Imports System.IO
Imports Newtonsoft.Json.Linq
Imports System.Text
Imports Newtonsoft.Json
Imports System.Threading
Imports MahApps.Metro

Public Class SplashScreen
    WithEvents wcversionsstring As New WebClient
    WithEvents wcmodlist As New WebClient
    WithEvents wcupdate As New WebClient
    WithEvents wcversion As New WebClient
    WithEvents wcchangelog As New WebClient


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
                Try
                    lbl_status.Content = "Prüfe auf Updates"
                    wcversion.DownloadStringAsync(New Uri(versionurl))
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                End Try
            Catch
            End Try
        Else
        lbl_statustitle.Content = "Fehler"
        lbl_status.Content = "Bitte überprüfe deine Internetverbindung!"
        End If
    End Sub

    Private Sub wcversion_DownloadFileCompleted(sender As Object, e As DownloadStringCompletedEventArgs) Handles wcversion.DownloadStringCompleted
        onlineversion = e.Result
        Try
            wcchangelog.DownloadStringAsync(New Uri(changelogurl))
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub wcchangelog_DownloadFileCompleted(sender As Object, e As DownloadStringCompletedEventArgs) Handles wcchangelog.DownloadStringCompleted
        changelog = e.Result
        If Check_Updates() = True Then
            Dim updater As New Updater
            updater.Show()
        Else
            Try
                wcversionsstring.DownloadFileAsync(New Uri(Versionsurl), outputjsonversions)
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        End If
    End Sub

    Private Async Sub wcversionsstring_DownloadFileCompleted(sender As Object, e As ComponentModel.AsyncCompletedEventArgs) Handles wcversionsstring.DownloadFileCompleted
        Await Versions_Load()
        lbl_status.Content = "Lade Mod-Liste herunter"
        wcmodlist.DownloadFileAsync(New Uri(modfileurl), modsfile)
    End Sub

    Private Async Sub wcmodlist_DownloadFileCompleted(sender As Object, e As ComponentModel.AsyncCompletedEventArgs) Handles wcmodlist.DownloadFileCompleted
        Try
            Mods.Load()
            Await Forge.Load()
            Await LiteLoader.Load()
            Start()
        Catch
        End Try
    End Sub

    Sub Start()
        Dim fThread = New Thread(New ThreadStart(AddressOf StartThread))
        fThread.IsBackground = True
        fThread.Start()
    End Sub

    Sub StartThread()
        Dispatcher.Invoke(New Action(Sub()
                                         'AccentColors = ThemeManager.DefaultAccents.Select(Function(p) p.Name)
                                         ' create accent color menu items for the demo
                                         AccentColors = ThemeManager.DefaultAccents.Select(Function(a) New AccentColorMenuData() With { _
                                                 .Name = a.Name,
                                                 .ColorBrush = New SolidColorBrush(CType(Windows.Media.ColorConverter.ConvertFromString(a.Resources("AccentColorBrush").ToString), System.Windows.Media.Color))
                                         }).ToList
                                         ShowWindowCommandsOnTop = False
                                         Try
                                             Dim Main As New MainWindow
                                             Me.Hide()


                                             Main.tb_modsfolder.Text = modsfolder
                                             Main.Load_ModVersions()
                                             Main.Get_Profiles()
                                             Main.Menuitem_accent.ItemsSource = AccentColors
                                             Settings.Load()
                                             Main.cb_direct_join.IsChecked = Settings.DirectJoin
                                             Main.tb_server_address.Text = Settings.ServerAddress
                                             Main.tb_username.Text = Settings.Username
                                             If Settings.Accent <> Nothing Then
                                                 Main.ChangeAccent(Settings.Accent)
                                             End If
                                             If Settings.Theme = "Dark" Then
                                                 Main.ThemeDark()
                                             Else
                                                 Main.ThemeLight()
                                             End If
                                             'LastLogin = Client.LastLogin.GetLastLogin
                                             'If LastLogin IsNot Nothing Then
                                             '    If LastLogin.Username <> Nothing Then
                                             '        tb_username.Text = LastLogin.Username
                                             '    End If
                                             '    If LastLogin.Password <> Nothing Then
                                             '        pb_Password.Password = LastLogin.Password
                                             '    End If
                                             'End If
                                             Main.Load_Servers()
                                             Main.Ping_servers()
                                             Main.Check_Tools_Downloaded()


                                             Main.InitializeComponent()
                                             'Finally Show The MainWindow
                                             Main.Show()
                                             Me.Close()
                                         Catch ex As Exception
                                             MessageBox.Show(ex.Message)
                                         End Try
                                     End Sub))
    End Sub

End Class

