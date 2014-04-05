Imports System.Net
Imports System.IO
Imports Newtonsoft.Json.Linq
Imports System.Text
Imports Newtonsoft.Json
Imports System.Threading
Imports MahApps.Metro
Imports MahApps.Metro.Controls.Dialogs
Imports System
Imports System.Reflection
Imports System.Windows.Threading

Public Class SplashScreen

    Public Function internetconnection() As Boolean
        Try
            My.Computer.Network.Ping("www.google.com")
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Async Sub SplashScreen_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Try
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
            Dim attributes As Object() = Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyCopyrightAttribute), False)
            If attributes.Length > 0 Then
                Dim CopyrightAttribute As AssemblyCopyrightAttribute = DirectCast(attributes(0), AssemblyCopyrightAttribute)
                If CopyrightAttribute.Copyright <> "" Then
                    lbl_copyright.Content = CopyrightAttribute.Copyright
                End If
            End If
            If Applicationdata.Exists = False Then
                Applicationdata.Create()
            End If
            If Applicationcache.Exists = False Then
                Applicationcache.Create()
            End If


            If internetconnection() = True Then
                If GetJavaPath() = Nothing OrElse New FileInfo(Path.Combine(GetJavaPath(), "bin", "java.exe")).Exists = False Then
                    Dim result As MessageDialogResult = Await ShowMessageAsync("Java nicht vorhanden", "Du musst Java installieren, um den McMetroLauncher und Minecraft nutzen zu können." & Environment.NewLine & "Ansonsten werden einige Funktionen nicht funktionieren!!" & Environment.NewLine & "Jetzt herunterladen?", MessageDialogStyle.AffirmativeAndNegative)
                    If result = MessageDialogResult.Affirmative Then
                        Process.Start("http://java.com/de/download")
                    Else
                        Application.Current.Shutdown()
                    End If
                End If

                If cachefolder.Exists = False Then
                    cachefolder.Create()
                End If
                Dim standartprofile As New JObject(
                    New JProperty("profiles",
                    New JObject(
                        New JProperty("Default",
                            New JObject(
                                New JProperty("name", "Default"))))),
                    New JProperty("selectedProfile", "Default"))
                Dim o As String
                If launcher_profiles_json.Exists = False Then
                    o = Nothing
                Else
                    o = File.ReadAllText(launcher_profiles_json.FullName)
                End If
                If o = Nothing Then
                    'StandartProfile schreiben
                    File.WriteAllText(launcher_profiles_json.FullName, standartprofile.ToString)
                End If
                lbl_status.Content = "Prüfe auf Updates"
                Dim dl As New WebClient()
                dl.DownloadStringAsync(New Uri(versionurl))
                AddHandler dl.DownloadStringCompleted, AddressOf downloadchangelog
                AddHandler dl.DownloadProgressChanged, AddressOf dlprogresschanged
            Else
                lbl_statustitle.Content = "Fehler"
                lbl_status.Content = "Bitte überprüfe deine Internetverbindung!"
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
        End Try
    End Sub

    Private Sub downloadchangelog(sender As Object, e As DownloadStringCompletedEventArgs)
        Try
            onlineversion = e.Result
            Dim dl As New WebClient()
            dl.DownloadStringAsync(New Uri(changelogurl))
            AddHandler dl.DownloadStringCompleted, AddressOf downloadversionsjson
            AddHandler dl.DownloadProgressChanged, AddressOf dlprogresschanged
        Catch ex As Exception
            MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
        End Try
    End Sub

    Private Sub downloadversionsjson(sender As Object, e As DownloadStringCompletedEventArgs)
        Try
            changelog = e.Result
            If Check_Updates() = True Then
                lbl_status.Content = "Update gefunden"
                Dim updater As New Updater
                updater.Show()
                Me.Close()
            Else
                lbl_status.Content = "Lade Versions-Liste herunter"
                Dim dl As New WebClient()
                dl.DownloadFileAsync(New Uri(Versionsurl), outputjsonversions.FullName)
                AddHandler dl.DownloadFileCompleted, AddressOf downloadmodsfile
                AddHandler dl.DownloadProgressChanged, AddressOf dlprogresschanged
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
        End Try
    End Sub

    Private Sub downloadawesomium(sender As Object, e As DownloadStringCompletedEventArgs)
        Try
            changelog = e.Result
            If Check_Updates() = True Then
                lbl_status.Content = "Update gefunden"
                Dim updater As New Updater
                updater.Show()
            Else
                lbl_status.Content = "Lade Versions-Liste herunter"
                Dim dl As New WebClient()
                dl.DownloadFileAsync(New Uri(Versionsurl), outputjsonversions.FullName)
                AddHandler dl.DownloadFileCompleted, AddressOf downloadmodsfile
                AddHandler dl.DownloadProgressChanged, AddressOf dlprogresschanged
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
        End Try
    End Sub

    Private Async Sub downloadmodsfile(sender As Object, e As ComponentModel.AsyncCompletedEventArgs)
        Try
            Await Versions_Load()
            lbl_status.Content = "Lade Mod-Liste herunter"
            Dim dl As New WebClient()
            dl.DownloadFileAsync(New Uri(modfileurl), modsfile.FullName)
            AddHandler dl.DownloadFileCompleted, AddressOf downloadlegacyforgefile
            AddHandler dl.DownloadProgressChanged, AddressOf dlprogresschanged
        Catch ex As Exception
            MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
        End Try
    End Sub

    Private Sub downloadlegacyforgefile(sender As Object, e As ComponentModel.AsyncCompletedEventArgs)
        Try
            lbl_status.Content = "Lade Forge-Build-Liste herunter"
            Dim dl As New WebClient
            dl.DownloadFileAsync(New Uri(Legacyforgeurl), Legacyforgefile.FullName)
            AddHandler dl.DownloadFileCompleted, AddressOf downloadforgefile
            AddHandler dl.DownloadProgressChanged, AddressOf dlprogresschanged

        Catch ex As Exception
            MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
        End Try
    End Sub

    Private Sub downloadforgefile(sender As Object, e As ComponentModel.AsyncCompletedEventArgs)
        Try
            Dim dl As New WebClient()
            dl.DownloadFileAsync(New Uri(Forgeurl), Forgefile.FullName)
            AddHandler dl.DownloadFileCompleted, AddressOf DownloadsFinished
            AddHandler dl.DownloadProgressChanged, AddressOf dlprogresschanged
        Catch ex As Exception
            MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
        End Try
    End Sub

    Private Async Sub DownloadsFinished(sender As Object, e As ComponentModel.AsyncCompletedEventArgs)
        Try
            Await Modifications.Load()
            Await Forge.Load()
            Await LiteLoader.Load()
            Downloads.Load()
            Await Start()
        Catch ex As Exception
            MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
        End Try
    End Sub

    Async Function Start() As Task
        Await Dispatcher.InvokeAsync(New Action(Async Function()
                                                    Try
                                                        ShowWindowCommandsOnTop = False
                                                        Await Settings.Load()
                                                        ' create accent color menu items
                                                        AccentColors = ThemeManager.Accents.Select(Function(a) New AccentColorMenuData() With {.Name = a.Name, .ColorBrush = CType(a.Resources("AccentColorBrush"), Windows.Media.Brush)}).ToList()
                                                        'create metro theme color menu items
                                                        AppThemes = ThemeManager.AppThemes.Select(Function(a) New AppThemeMenuData() With {.Name = a.Name, .BorderColorBrush = CType(a.Resources("BlackColorBrush"), Windows.Media.Brush), .ColorBrush = CType(a.Resources("WhiteColorBrush"), Windows.Media.Brush)}).ToList
                                                        Dim Main As New MainWindow
                                                        If Settings.Settings.WindowState <> Windows.WindowState.Minimized Then
                                                            Main.WindowState = Settings.Settings.WindowState
                                                        End If
                                                        Main.Webcontrol_news.Visibility = Windows.Visibility.Collapsed
                                                        Main.tb_modsfolder.Text = modsfolder.FullName
                                                        Await Main.Load_ModVersions()
                                                        Main.Get_Profiles()
                                                        Main.Menuitem_accent.ItemsSource = AccentColors
                                                        Main.Menuitem_theme.ItemsSource = AppThemes
                                                        Main.cb_direct_join.IsChecked = Settings.Settings.DirectJoin
                                                        Main.tb_server_address.Text = Settings.Settings.ServerAddress
                                                        Main.tb_username.Text = Settings.Settings.Username
                                                        'LastLogin = Client.LastLogin.GetLastLogin
                                                        'If LastLogin IsNot Nothing Then
                                                        '    If LastLogin.Username <> Nothing Then
                                                        '        tb_username.Text = LastLogin.Username
                                                        '    End If
                                                        '    If LastLogin.Password <> Nothing Then
                                                        '        pb_Password.Password = LastLogin.Password
                                                        '    End If
                                                        'End If
                                                        Await Main.Load_Servers()
                                                        Main.Ping_servers()
                                                        Main.Check_Tools_Downloaded()
                                                        Me.Hide()
                                                        If Settings.Settings.Accent <> Nothing Then
                                                            Dim theme = ThemeManager.DetectAppStyle(Application.Current)
                                                            Dim accent = ThemeManager.Accents.Where(Function(p) p.Name = Settings.Settings.Accent).FirstOrDefault
                                                            If accent Is Nothing Then accent = ThemeManager.Accents.First
                                                            ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1)
                                                        End If
                                                        If Settings.Settings.Theme <> Nothing Then
                                                            Dim theme = ThemeManager.DetectAppStyle(Application.Current)
                                                            Dim appTheme = ThemeManager.AppThemes.Where(Function(p) p.Name = Settings.Settings.Theme).FirstOrDefault
                                                            If appTheme Is Nothing Then appTheme = ThemeManager.AppThemes.First
                                                            ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, appTheme)
                                                        End If
                                                        'Finally Show The MainWindow
                                                        Main.Show()
                                                        Me.Close()
                                                    Catch ex As Exception
                                                        MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
                                                    End Try
                                                End Function))
    End Function

    Sub dlprogresschanged(sender As Object, e As DownloadProgressChangedEventArgs)
        pb_download.Value = e.ProgressPercentage
    End Sub

End Class