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
Imports System.Collections.Specialized
Imports System.Web
Imports Exceptionless

Public Class SplashScreen
    Dim dlversion As New WebClient
    Dim dlchangelog As New WebClient
    Dim dlversionsjson As New WebClient
    Dim dlmodsfile As New WebClient
    Dim dlforgefile As New WebClient
    Dim dllegacyforgefile As New WebClient


    Public Async Function internetconnection() As Task(Of Boolean)
        Try
            Using client = New WebClient()
                Using stream = Await client.OpenReadTaskAsync("http://www.google.com")
                    Return True
                End Using
            End Using
        Catch
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
        If Not Applicationdata.Exists Then
            Applicationdata.Create()
        End If
        If Applicationcache.Exists = False Then
            Applicationcache.Create()
        End If
        Await MainViewModel.Instance.LoadSettings()
        If MainViewModel.Instance.Settings.Accent <> Nothing AndAlso ThemeManager.Accents.Select(Function(p) p.Name).Contains(MainViewModel.Instance.Settings.Accent) Then
            Dim theme = ThemeManager.DetectAppStyle(Application.Current)
            Dim accent = ThemeManager.Accents.Where(Function(p) p.Name = MainViewModel.Instance.Settings.Accent).FirstOrDefault
            If accent Is Nothing Then accent = ThemeManager.Accents.First
            ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1)
        End If
        If MainViewModel.Instance.Settings.Theme <> Nothing AndAlso ThemeManager.AppThemes.Select(Function(p) p.Name).Contains(MainViewModel.Instance.Settings.Theme) Then
            Dim theme = ThemeManager.DetectAppStyle(Application.Current)
            Dim appTheme = ThemeManager.AppThemes.Where(Function(p) p.Name = MainViewModel.Instance.Settings.Theme).FirstOrDefault
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

        lbl_Version.Text = sVersion
        Dim attributes As Object() = Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyCopyrightAttribute), False)
        If attributes.Length > 0 Then
            Dim CopyrightAttribute As AssemblyCopyrightAttribute = DirectCast(attributes(0), AssemblyCopyrightAttribute)
            If CopyrightAttribute.Copyright <> "" Then
                lbl_copyright.Text = CopyrightAttribute.Copyright
            End If
        End If

        If Await internetconnection() = True Then
            If GetJavaPath() = Nothing OrElse New FileInfo(GetJavaPath()).Exists = False Then
                MainViewModel.Instance.Settings.JavaPath = Nothing
                MainViewModel.Instance.Settings.Save()
                Dim result As New frmGetJavaPath()
                result.ShowDialog()
                If result.DialogResult = True Then
                    MainViewModel.Instance.Settings.JavaPath = result.JavaPath
                    MainViewModel.Instance.Settings.Save()
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
            lbl_status.Content = Application.Current.FindResource("CheckingForUpdates").ToString
            dlversion.DownloadStringAsync(New Uri(versionurl))
            AddHandler dlversion.DownloadStringCompleted, AddressOf downloadchangelog
            AddHandler dlversion.DownloadProgressChanged, AddressOf dlprogresschanged
        Else
            lbl_statustitle.Content = Application.Current.FindResource("Error").ToString
            lbl_status.Content = Application.Current.FindResource("CheckInternetConnection").ToString & "!"
        End If
    End Sub

    Private Sub downloadchangelog(sender As Object, e As DownloadStringCompletedEventArgs)
        If e.Cancelled = False AndAlso e.Error Is Nothing Then
            Try
                onlineversion = e.Result
                dlchangelog.DownloadStringAsync(New Uri(changelogurl))
                AddHandler dlchangelog.DownloadStringCompleted, AddressOf downloadversionsjson
                AddHandler dlchangelog.DownloadProgressChanged, AddressOf dlprogresschanged
            Catch ex As Exception
                MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
            End Try
        ElseIf e.Cancelled = False AndAlso e.Error IsNot Nothing Then
            MessageBox.Show(Application.Current.FindResource("Error").ToString & ": " & Environment.NewLine & e.Error.Message & Environment.NewLine & e.Error.StackTrace)
        End If
    End Sub

    Private Sub downloadversionsjson(sender As Object, e As DownloadStringCompletedEventArgs)
        If e.Cancelled = False AndAlso e.Error Is Nothing Then
            Try
                changelog = e.Result
                If Check_Updates() = True Then
                    lbl_status.Content = Application.Current.FindResource("Error").ToString
                    Dim updater As New Updater
                    updater.Show()
                    Me.Close()
                Else
                    lbl_status.Content = Application.Current.FindResource("MsgDownloadingVersions").ToString
                    dlversionsjson.DownloadFileAsync(New Uri(Versionsurl), outputjsonversions.FullName)
                    AddHandler dlversionsjson.DownloadFileCompleted, AddressOf downloadmodsfile
                    AddHandler dlversionsjson.DownloadProgressChanged, AddressOf dlprogresschanged
                End If
            Catch ex As Exception
                MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
            End Try
        ElseIf e.Cancelled = False AndAlso e.Error IsNot Nothing Then
            MessageBox.Show(Application.Current.FindResource("Error").ToString & ": " & Environment.NewLine & e.Error.Message & Environment.NewLine & e.Error.StackTrace)
        End If
    End Sub

    Private Async Sub downloadmodsfile(sender As Object, e As ComponentModel.AsyncCompletedEventArgs)
        If e.Cancelled = False AndAlso e.Error Is Nothing Then
            Try
                Await Versions_Load()
                lbl_status.Content = Application.Current.FindResource("MsgDownloadingModlist").ToString
                dlmodsfile.DownloadFileAsync(New Uri(modfileurl), modsfile.FullName)
                AddHandler dlmodsfile.DownloadFileCompleted, AddressOf downloadlegacyforgefile
                AddHandler dlmodsfile.DownloadProgressChanged, AddressOf dlprogresschanged
            Catch ex As Exception
                MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
            End Try
        ElseIf e.Cancelled = False AndAlso e.Error IsNot Nothing Then
            MessageBox.Show(Application.Current.FindResource("Error").ToString & ": " & Environment.NewLine & e.Error.Message & Environment.NewLine & e.Error.StackTrace)
        End If
    End Sub

    Private Sub downloadlegacyforgefile(sender As Object, e As ComponentModel.AsyncCompletedEventArgs)
        If e.Cancelled = False AndAlso e.Error Is Nothing Then
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
                    lbl_status.Content = Application.Current.FindResource("MsgDownloadingForgeBuilds").ToString
                    dlforgefile.DownloadFileAsync(New Uri(Legacyforgeurl), Legacyforgefile.FullName)
                    AddHandler dlforgefile.DownloadFileCompleted, AddressOf downloadlegacyforgefilefinfished
                    AddHandler dlforgefile.DownloadProgressChanged, AddressOf dlprogresschanged
                Catch ex As Exception
                    MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
                End Try
            End If
        ElseIf e.Cancelled = False AndAlso e.Error IsNot Nothing Then
            MessageBox.Show(Application.Current.FindResource("Error").ToString & ": " & Environment.NewLine & e.Error.Message & Environment.NewLine & e.Error.StackTrace)
        End If
    End Sub

    Private Sub downloadlegacyforgefilefinfished(sender As Object, e As ComponentModel.AsyncCompletedEventArgs)
        If e.Cancelled = False AndAlso e.Error Is Nothing Then
            Downloadforgefile()
        ElseIf e.Cancelled = False AndAlso e.Error IsNot Nothing Then
            MessageBox.Show(Application.Current.FindResource("Error").ToString & ": " & Environment.NewLine & e.Error.Message & Environment.NewLine & e.Error.StackTrace)
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
        If e.Cancelled = False AndAlso e.Error Is Nothing Then
            lbl_status.Content = Application.Current.FindResource("LauncherStarting").ToString
            Await ServerList.Load
            Await authenticationDatabase.Load()
            Await Modifications.Load()
            Await Forge.Load()
            Await LiteLoader.Load()
            Downloads.Load()
            Await Start()
        ElseIf e.Cancelled = False AndAlso e.Error IsNot Nothing Then
            MessageBox.Show(Application.Current.FindResource("Error").ToString & ": " & Environment.NewLine & e.Error.Message & Environment.NewLine & e.Error.StackTrace)
        End If
    End Sub

    Async Function Start() As Task
        If MainViewModel.Instance.Settings.WindowState <> Windows.WindowState.Minimized Then
            Main.WindowState = MainViewModel.Instance.Settings.WindowState
        End If
        Main.Webcontrol_news.Visibility = Windows.Visibility.Collapsed
        Main.tb_modsfolder.Text = modsfolder.FullName
        Main.Load_ModVersions()
        Profiles.Get_Profiles()
        Main.cb_direct_join.IsChecked = MainViewModel.Instance.Settings.DirectJoin
        MainViewModel.Instance.Directjoinaddress = MainViewModel.Instance.Settings.ServerAddress
        Try
            If CommandLineArgs.Count > 1 Then
                Dim url As New Uri(CommandLineArgs(1))
                'Console.WriteLine("Protocol: {0}", url.Scheme)
                'Console.WriteLine("Host: {0}", url.Host)
                'Console.WriteLine("Path: {0}", HttpUtility.UrlDecode(url.AbsolutePath))
                'Console.WriteLine("Query: {0}", url.Query)
                'Dim Parms As NameValueCollection = HttpUtility.ParseQueryString(url.Query)
                'Console.WriteLine("Parms: {0}", Parms.Count)
                'For Each x As String In Parms.AllKeys
                '    Console.WriteLine(vbTab & "Parm: {0} = {1}", x, Parms(x))
                'Next
                If url.Host = "join" Then
                    If url.Segments.Count > 1 Then
                        MainViewModel.Instance.Directjoinaddress = url.Segments.ElementAt(1)
                        Main.cb_direct_join.IsChecked = True
                    End If
                End If
                If url.Host = "mods" Then
                    If url.Segments.Count > 2 Then
                        If url.Segments.ElementAt(1).Replace("/", "") = "show" Then
                            Main.cb_modversions.SelectedItem = Main.cb_modversions.Items.Cast(Of String).Where(Function(p) p = url.Segments.ElementAt(2).Replace("/", "")).First
                            If url.Segments.Count > 3 Then
                                Main.lb_mods.SelectedItem = Main.lb_mods.Items.Cast(Of Modifications.Mod).Where(Function(p) p.id = url.Segments.ElementAt(3).Replace("/", "")).First
                            End If
                            Main.tabitem_Mods.IsSelected = True
                        End If
                    End If
                End If
            End If
        Catch
        End Try
        Await Main.Load_Servers()
        Main.Ping_servers()
        Main.Check_Tools_Downloaded()
        Main.Show()
        Me.Close()
    End Function

    Sub dlprogresschanged(sender As Object, e As DownloadProgressChangedEventArgs)
        pb_download.Value = e.ProgressPercentage
    End Sub

End Class