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
Imports McMetroLauncher.JBou.Authentication.Session
Imports McMetroLauncher.JBou.Authentication

Public Class SplashScreen
    Dim dlversion As New WebClient
    Dim dlchangelog As New WebClient
    Dim dlversionsjson As New WebClient
    Dim dlmodsfile As New WebClient
    Dim dlforgefile As New WebClient
    Dim dllegacyforgefile As New WebClient


    Public Shared Starting As Boolean = True
    Public Async Function internetconnection() As Task(Of Boolean)
        Try
            Using client As New WebClient
                Using stream As Stream = Await client.OpenReadTaskAsync("http://www.google.com")
                    stream.Close()
                    Return True
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub SplashScreen_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        dlversion.CancelAsync()
        dlchangelog.CancelAsync()
        dlversionsjson.CancelAsync()
        dlmodsfile.CancelAsync()
        dlforgefile.CancelAsync()
        dllegacyforgefile.CancelAsync()
    End Sub

    Private Async Sub SplashScreen_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Try
            Starting = True
            Await Settings.Load()
            If Settings.Settings.Accent <> Nothing And ThemeManager.Accents.Select(Function(p) p.Name).Contains(Settings.Settings.Accent) Then
                Dim theme = ThemeManager.DetectAppStyle(Application.Current)
                Dim accent = ThemeManager.Accents.Where(Function(p) p.Name = Settings.Settings.Accent).FirstOrDefault
                If accent Is Nothing Then accent = ThemeManager.Accents.First
                ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1)
            End If
            If Settings.Settings.Theme <> Nothing And ThemeManager.AppThemes.Select(Function(p) p.Name).Contains(Settings.Settings.Theme) Then
                Dim theme = ThemeManager.DetectAppStyle(Application.Current)
                Dim appTheme = ThemeManager.AppThemes.Where(Function(p) p.Name = Settings.Settings.Theme).FirstOrDefault
                If appTheme Is Nothing Then appTheme = ThemeManager.AppThemes.First
                ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, appTheme)
            End If

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


            If Await internetconnection() = True Then
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
                dlversion.DownloadStringAsync(New Uri(versionurl))
                AddHandler dlversion.DownloadStringCompleted, AddressOf downloadchangelog
                AddHandler dlversion.DownloadProgressChanged, AddressOf dlprogresschanged
            Else
                lbl_statustitle.Content = "Fehler"
                lbl_status.Content = "Bitte überprüfe deine Internetverbindung!"
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
        End Try
    End Sub

    Private Sub downloadchangelog(sender As Object, e As DownloadStringCompletedEventArgs)
        If e.Cancelled = False And e.Error Is Nothing Then
            Try
                onlineversion = e.Result
                dlchangelog.DownloadStringAsync(New Uri(changelogurl))
                AddHandler dlchangelog.DownloadStringCompleted, AddressOf downloadversionsjson
                AddHandler dlchangelog.DownloadProgressChanged, AddressOf dlprogresschanged
            Catch ex As Exception
                MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
            End Try
        ElseIf e.Cancelled = False And e.Error IsNot Nothing Then
            MessageBox.Show("Ein Fehler ist aufgetreten: " & Environment.NewLine & e.Error.Message & Environment.NewLine & e.Error.StackTrace)
        End If
    End Sub

    Private Sub downloadversionsjson(sender As Object, e As DownloadStringCompletedEventArgs)
        If e.Cancelled = False And e.Error Is Nothing Then
            Try
                changelog = e.Result
                If Check_Updates() = True Then
                    lbl_status.Content = "Update gefunden"
                    Dim updater As New Updater
                    updater.Show()
                    Me.Close()
                Else
                    lbl_status.Content = "Lade Versions-Liste herunter"
                    dlversionsjson.DownloadFileAsync(New Uri(Versionsurl), outputjsonversions.FullName)
                    AddHandler dlversionsjson.DownloadFileCompleted, AddressOf downloadmodsfile
                    AddHandler dlversionsjson.DownloadProgressChanged, AddressOf dlprogresschanged
                End If
            Catch ex As Exception
                MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
            End Try
        ElseIf e.Cancelled = False And e.Error IsNot Nothing Then
            MessageBox.Show("Ein Fehler ist aufgetreten: " & Environment.NewLine & e.Error.Message & Environment.NewLine & e.Error.StackTrace)
        End If
    End Sub

    Private Async Sub downloadmodsfile(sender As Object, e As ComponentModel.AsyncCompletedEventArgs)
        If e.Cancelled = False And e.Error Is Nothing Then
            Try
                Await Versions_Load()
                lbl_status.Content = "Lade Mod-Liste herunter"
                dlmodsfile.DownloadFileAsync(New Uri(modfileurl), modsfile.FullName)
                AddHandler dlmodsfile.DownloadFileCompleted, AddressOf downloadlegacyforgefile
                AddHandler dlmodsfile.DownloadProgressChanged, AddressOf dlprogresschanged
            Catch ex As Exception
                MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
            End Try
        ElseIf e.Cancelled = False And e.Error IsNot Nothing Then
            MessageBox.Show("Ein Fehler ist aufgetreten: " & Environment.NewLine & e.Error.Message & Environment.NewLine & e.Error.StackTrace)
        End If
    End Sub

    Private Sub downloadlegacyforgefile(sender As Object, e As ComponentModel.AsyncCompletedEventArgs)
        If e.Cancelled = False And e.Error Is Nothing Then
            Dim valid As Boolean = True
            Try
                JContainer.Parse(File.ReadAllText(Legacyforgefile.FullName))
            Catch ex As Exception
                valid = False
            End Try
            If valid = True Then
                Downloadforgefile()
            Else
                Try
                    lbl_status.Content = "Lade Forge-Build-Liste herunter"
                    dlforgefile.DownloadFileAsync(New Uri(Legacyforgeurl), Legacyforgefile.FullName)
                    AddHandler dlforgefile.DownloadFileCompleted, AddressOf downloadlegacyforgefilefinfished
                    AddHandler dlforgefile.DownloadProgressChanged, AddressOf dlprogresschanged
                Catch ex As Exception
                    MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
                End Try
            End If
        ElseIf e.Cancelled = False And e.Error IsNot Nothing Then
            MessageBox.Show("Ein Fehler ist aufgetreten: " & Environment.NewLine & e.Error.Message & Environment.NewLine & e.Error.StackTrace)
        End If
    End Sub

    Private Sub downloadlegacyforgefilefinfished(sender As Object, e As ComponentModel.AsyncCompletedEventArgs)
        If e.Cancelled = False And e.Error Is Nothing Then
            Downloadforgefile()
        ElseIf e.Cancelled = False And e.Error IsNot Nothing Then
            MessageBox.Show("Ein Fehler ist aufgetreten: " & Environment.NewLine & e.Error.Message & Environment.NewLine & e.Error.StackTrace)
        End If
    End Sub

    Sub Downloadforgefile()
        Try
            dllegacyforgefile.DownloadFileAsync(New Uri(Forgeurl), Forgefile.FullName)
            AddHandler dllegacyforgefile.DownloadFileCompleted, AddressOf DownloadsFinished
            AddHandler dllegacyforgefile.DownloadProgressChanged, AddressOf dlprogresschanged
        Catch ex As Exception
            MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
        End Try
    End Sub

    Private Async Sub DownloadsFinished(sender As Object, e As ComponentModel.AsyncCompletedEventArgs)
        If e.Cancelled = False And e.Error Is Nothing Then
            Try
                lbl_status.Content = "Launcher startet..."
                Await Modifications.Load()
                Await Forge.Load()
                Await LiteLoader.Load()
                Downloads.Load()
                Await Start()
            Catch ex As Exception
                MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
            End Try
        ElseIf e.Cancelled = False And e.Error IsNot Nothing Then
            MessageBox.Show("Ein Fehler ist aufgetreten: " & Environment.NewLine & e.Error.Message & Environment.NewLine & e.Error.StackTrace)
        End If
    End Sub

    Async Function Start() As Task
        Try
            ShowWindowCommandsOnTop = False
            If Settings.Settings.WindowState <> Windows.WindowState.Minimized Then
                Main.WindowState = Settings.Settings.WindowState
            End If
            Main.Webcontrol_news.Visibility = Windows.Visibility.Collapsed
            Main.tb_modsfolder.Text = modsfolder.FullName
            Await Main.Load_ModVersions()
            Profiles.Get_Profiles()
            Main.cb_direct_join.IsChecked = Settings.Settings.DirectJoin
            Main.tb_server_address.Text = Settings.Settings.ServerAddress
            'Main.tb_username.Text = Settings.Settings.Username
            Await Main.Load_Servers()
            Main.Ping_servers()
            Main.Check_Tools_Downloaded()
            Await Check_Account()
            Starting = False
        Catch ex As Exception
            MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
        End Try
    End Function

    Sub dlprogresschanged(sender As Object, e As DownloadProgressChangedEventArgs)
        pb_download.Value = e.ProgressPercentage
    End Sub


#Region "Auth"

    Async Function Check_Account() As Task
        Await authenticationDatabase.Load()
        Dim profile As Profiles.Profile = Await Profiles.FromName(ViewModel.selectedprofile)
        If profile.playerUUID = Nothing Then
            'Show Login
            Dim loginscreen As New Login
            loginscreen.Show()
        Else
            Dim capturedException As MinecraftAuthenticationException = Nothing
            'Login with access token
            Try
                If authenticationDatabase.List.Select(Function(p) p.uuid.Replace("-", "")).Contains(profile.playerUUID) Then
                    lbl_status.Content = "Anmelden mit access token"
                    Dim Account = authenticationDatabase.List.Where(Function(p) p.uuid.Replace("-", "") = profile.playerUUID).First
                    Dim session = New Session() With {.AccessToken = Account.accessToken,
                                                      .ClientToken = authenticationDatabase.clientToken,
                                                      .SelectedProfile = Nothing}
                    Await session.Refresh
                    Main.lbl_Username.Content = "Willkommen, " & Account.displayName
                    authenticationDatabase.List.Where(Function(p) p.uuid.Replace("-", "") = profile.playerUUID).First.accessToken = session.AccessToken
                    Await authenticationDatabase.Save()
                Else
                    'Show Login
                    Dim loginscreen As New Login
                    loginscreen.Show()
                    Me.Close()
                    Exit Function
                End If
            Catch ex As MinecraftAuthenticationException
                capturedException = ex
            End Try
            If capturedException IsNot Nothing Then
                Await Me.ShowMessageAsync(capturedException.Error, capturedException.ErrorMessage, MessageDialogStyle.Affirmative)
                'Show Login
                Dim loginscreen As New Login
                loginscreen.Show()
            Else
                Main.Show()
            End If
        End If
        Me.Close()
    End Function

#End Region
End Class