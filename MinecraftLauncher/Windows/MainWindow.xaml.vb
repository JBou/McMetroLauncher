#Region "Imports"
Imports System.Net
Imports System.IO
Imports System.Xml.Linq
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Globalization
Imports Ionic.Zip
Imports System.Net.Sockets
Imports MahApps.Metro
Imports System.Reflection
Imports System.Windows.Threading
Imports System.Text.RegularExpressions
Imports MahApps.Metro.Accent
Imports MahApps.Metro.Controls
Imports MahApps.Metro.Controls.Dialogs
Imports Microsoft.Win32
Imports Ookii.Dialogs.Wpf
Imports System.Xml
Imports fNbt
Imports System.Threading
Imports System.Security.Cryptography
Imports Craft.Net
Imports System.Threading.Tasks
Imports System.ComponentModel
Imports System.Windows.Media
Imports System
Imports System.Windows.Markup
Imports McMetroLauncher.Models
Imports System.Runtime.ExceptionServices
Imports System.Text
Imports System.Resources
Imports McMetroLauncher.JBou.Authentication
Imports McMetroLauncher.JBou.Authentication.Session
Imports System.Collections.ObjectModel
Imports System.Runtime.CompilerServices
Imports McMetroLauncher.Forge

#End Region

Public Class MainWindow
#Region "Variables"
    '****************Webclients*****************
    WithEvents wcmod As New System.Net.WebClient
    '*************Minecraft Prozess*************
    WithEvents mc As New Process
    '**************Mods Download****************
    Private modsdownloadingversion As String
    Private moddownloading As Boolean = False
    Private modsdownloadlist As IList(Of Modifications.Mod) = New List(Of Modifications.Mod)
    Private modsdownloadindex As Integer
    Private Modsfilename As String
    Private modslist As IList(Of Modifications.Mod)
    Private modsfolderPath As String
    '********************UI*********************
    Public controller As ProgressDialogController
    Public toolscontroller As ProgressDialogController
    '******************Others*******************
#End Region

#Region "Mainwindow Events"
    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        AddHandler ThemeManager.IsThemeChanged, AddressOf IsThemeChanged
        Me.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme
    End Sub

    Private Sub MainWindow_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        Try
            For i = 0 To startedversions.Count - 1
                If IO.Directory.Exists(startedversions.Item(i).ToString) = True Then
                    IO.Directory.Delete(startedversions.Item(i).ToString, True)
                End If
            Next
            If cachefolder.Exists = True Then
                cachefolder.Delete(True)
            End If
        Catch ex As Exception
        End Try
        Environment.Exit(Environment.ExitCode)
    End Sub

    Private Async Sub MainWindow_Loaded(sender As Object, e As EventArgs) Handles Me.Loaded
        'Set the Webbrowser Source:
        Webcontrol_news.WebSession = WebCore.CreateWebSession(New WebPreferences() With {.CustomCSS = Scrollbarcss})
        'wc_mod_video.WebSession = WebCore.CreateWebSession(New WebPreferences() With {.CustomCSS = Scrollbarcss})
        While Webcontrol_news.IsLoading = True
            Await Task.Delay(10)
        End While
        Webcontrol_news.Visibility = Windows.Visibility.Visible
        lbl_news_loading.Visibility = Windows.Visibility.Collapsed
        pb_news_loading.Visibility = Windows.Visibility.Collapsed
    End Sub
#End Region

    Sub IsThemeChanged(sender As Object, e As OnThemeChangedEventArgs)
        MainViewModel.Instance.Settings.Theme = e.AppTheme.Name
        MainViewModel.Instance.Settings.Accent = e.Accent.Name
        MainViewModel.Instance.Settings.Save()
    End Sub

    Private Sub ShowSettings(sender As Object, e As RoutedEventArgs)
        'Contrrols auf ihre Einstellungen setzen
        ToggleFlyout(0)
    End Sub

    Public Sub Open_Website()
        Process.Start(GlobalInfos.Website)
    End Sub

    Public Sub Open_Github()
        Process.Start(GlobalInfos.Github)
    End Sub

    Private Sub ToggleFlyout(index As Integer)
        Dim flyout As Flyout = CType(Me.Flyouts.Items(index), Controls.Flyout)
        If flyout Is Nothing Then
            Return
        End If

        If flyout.IsOpen = True Then
            flyout.IsOpen = False
        Else
            flyout.IsOpen = True
        End If
    End Sub

    Private Sub Help()
        'Zeige Hilfe
    End Sub

    ''' <summary>
    ''' Registriert ein benutzerdefiniertes URL-Protokoll für die Verwendung mit der
    ''' Windows-Shell, dem Internet Explorer und Office.
    ''' 
    ''' Beispiel für einen URL eines benutzerdefinierten URL-Protokolls:
    ''' 
    '''   rainbird://RemoteControl/OpenFridge/GetBeer
    ''' </summary>
    ''' <param name="protocolName">Name des Protokolls (z.B. "rainbird" für "rainbird://...")</param>
    ''' <param name="applicationPath">Vollständiger Dateisystem-Pfad zur EXE-Datei, die den URL bei Aufruf verarbeitet (Der komplette URL wird als Befehlszeilenparameter übergteben)</param>
    ''' <param name="description">Beschreibung (z.B. "URL:Rainbird Custom URL")</param>
    ''' +
    ''' 
    Public Sub RegisterURLProtocol(protocolName As String, applicationPath As String, description As String)
        ' Neuer Schlüssel für das gewünschte URL Protokoll erstellen
        Dim myKey As RegistryKey = Registry.ClassesRoot.CreateSubKey(protocolName)

        ' Protokoll zuweisen
        myKey.SetValue(Nothing, description)
        myKey.SetValue("URL Protocol", String.Empty)

        ' Shellwerte eintragen
        Registry.ClassesRoot.CreateSubKey(protocolName & "\Shell")
        Registry.ClassesRoot.CreateSubKey(protocolName & "\Shell\open")
        myKey = Registry.ClassesRoot.CreateSubKey(protocolName & "\Shell\open\command")

        ' Anwendung festlegen, die das URL-Protokoll behandelt
        myKey.SetValue(Nothing, Chr(34) & applicationPath & Chr(34) & " %1")
    End Sub

    Async Function Unzip() As Task
        Await Task.Run(New Action(Sub()
                                      Try
                                          Write(Application.Current.FindResource("UnpackingNatives").ToString)
                                          UnpackDirectory = Path.Combine(versionsfolder.FullName, Startinfos.Version.id, Startinfos.Version.id & "-natives-" & DateTime.Now.Ticks.ToString)
                                          If startedversions.Contains(UnpackDirectory) = False Then
                                              startedversions.Add(UnpackDirectory)
                                          End If
                                          For Each item In Startinfos.Versionsinfo.libraries.Where(Function(p) p.natives IsNot Nothing) 'TODO Resolve VersionsInfo
                                              With item
                                                  'Rules überprüfen
                                                  Dim allowdownload As Boolean = True
                                                  If .rules Is Nothing Then
                                                      allowdownload = True
                                                  Else
                                                      If .rules.Select(Function(p) p.action).Contains("allow") Then
                                                          If .rules.Where(Function(p) p.action = "allow").First.os IsNot Nothing Then
                                                              If .rules.Where(Function(p) p.action = "allow").First.os.name = "windows" Then
                                                                  allowdownload = True
                                                              Else
                                                                  allowdownload = False
                                                              End If
                                                          End If
                                                      ElseIf .rules.Select(Function(p) p.action).Contains("disallow") Then
                                                          If .rules.Where(Function(p) p.action = "disallow").First.os IsNot Nothing Then
                                                              If .rules.Where(Function(p) p.action = "disallow").First.os.name = "windows" Then
                                                                  allowdownload = False
                                                              Else
                                                                  allowdownload = True
                                                              End If
                                                          End If
                                                      End If
                                                  End If
                                                  If .natives IsNot Nothing AndAlso .natives.windows IsNot Nothing AndAlso allowdownload = True Then
                                                      Dim librarypath As New FileInfo(IO.Path.Combine(librariesfolder.FullName, .path.Replace("/", "\")))
                                                      If IO.Directory.Exists(librarypath.DirectoryName) = False Then
                                                          IO.Directory.CreateDirectory(librarypath.DirectoryName)
                                                      End If
                                                      Try
                                                          Using zip1 As ZipFile = ZipFile.Read(librarypath.FullName)
                                                              ' here, we extract every entry, but we could extract conditionally,
                                                              ' based on entry name, size, date, checkbox status, etc.   
                                                              For Each e As ZipEntry In zip1
                                                                  Dim ls As IList(Of String) = .extract.exclude
                                                                  For Each file As String In ls
                                                                      If e.FileName.StartsWith(file) = False Then
                                                                          e.Extract(UnpackDirectory, ExtractExistingFileAction.OverwriteSilently)
                                                                      End If
                                                                  Next
                                                              Next
                                                          End Using
                                                      Catch ex As ZipException
                                                          Write(Application.Current.FindResource("ErrorUnpackingNatives").ToString & ": " & ex.Message, LogLevel.ERROR)
                                                      End Try
                                                  End If
                                              End With
                                          Next
                                      Catch ex As Exception
                                          Write(Application.Current.FindResource("ErrorUnpackingNativesLibraryMissing").ToString & Environment.NewLine & ex.Message & Environment.NewLine & ex.StackTrace, LogLevel.ERROR)
                                      End Try
                                  End Sub))
    End Function

    Async Function Get_Startinfos() As Task 'TODO Move to Version?
        Await Unzip()
        Write(Application.Current.FindResource("GettingStartInfos").ToString)
        Dim mainClass As String = Startinfos.Versionsinfo.mainClass
        Dim minecraftArguments As List(Of String) = Startinfos.Versionsinfo.minecraftArguments.Split(Chr(32)).ToList
        Dim libraries As String = Nothing
        Dim gamedir As String = Nothing
        Dim argumentreplacements As List(Of String()) = New List(Of String())
        Dim natives As String = """" & UnpackDirectory & """"
        Dim javaargs As String = Nothing
        Dim height As String = Nothing
        Dim width As String = Nothing
        Dim Teil_Arguments As String = Nothing

        Await Task.Run(New Action(Async Sub()
                                      'TODO
                                      'Split by Space --> (Chr(32))
                                      If Startinfos.Versionsinfo Is Nothing Then
                                          Startinfos.Versionsinfo = Await MinecraftDownloadManager.ParseVersionsInfo(Startinfos.Version)
                                      End If
                                      Versionsjar = """" & Path.Combine(versionsfolder.FullName, Startinfos.Versionsinfo.getJar, Startinfos.Versionsinfo.getJar & ".jar") & """"
                                      For i = 0 To Startinfos.Versionsinfo.libraries.Count - 1
                                          Dim librarytemp As Library = Startinfos.Versionsinfo.libraries.Item(i)
                                          If librarytemp.natives Is Nothing OrElse librarytemp.natives.windows IsNot Nothing Then
                                              libraries &= """" & Path.Combine(librariesfolder.FullName, librarytemp.path.Replace("/", "\")) & """" & ";"
                                          End If
                                      Next

                                      If Startinfos.Profile.gameDir <> Nothing Then
                                          gamedir = Startinfos.Profile.gameDir
                                      Else
                                          gamedir = mcpfad.FullName
                                      End If

                                      If IO.Directory.Exists(gamedir) = False Then
                                          IO.Directory.CreateDirectory(gamedir)
                                      End If
                                      Dim assets_dir As String = assetspath.FullName
                                      Dim assets_index_name = If(Startinfos.Versionsinfo.assets, "legacy")
                                      Dim resourcesindex As resourcesindex = Await MinecraftDownloadManager.ParseResources(assets_index_name) 'TODO Don't Parse it again, just parsed when resources where downloaded
                                      If resourcesindex.virtual Then
                                          assets_dir = Path.Combine(assetspath.FullName, "virtual", assets_index_name)
                                      End If
                                      argumentreplacements.Add(New String() {"${auth_player_name}", Startinfos.Session.SelectedProfile.Name})
                                      argumentreplacements.Add(New String() {"${version_name}", Startinfos.Version.id})
                                      argumentreplacements.Add(New String() {"${game_directory}", """" & gamedir & """"})
                                      argumentreplacements.Add(New String() {"${game_assets}", """" & assets_dir & """"})
                                      argumentreplacements.Add(New String() {"${assets_root}", """" & assets_dir & """"})
                                      argumentreplacements.Add(New String() {"${assets_index_name}", assets_index_name})
                                      argumentreplacements.Add(New String() {"${user_properties}", New JObject().ToString})

                                      If Startinfos.Session.OnlineMode = True Then
                                          'session username
                                          argumentreplacements.Add(New String() {"${auth_uuid}", Startinfos.Session.SelectedProfile.Id})
                                          argumentreplacements.Add(New String() {"${auth_access_token}", Startinfos.Session.AccessToken})
                                          argumentreplacements.Add(New String() {"${auth_session}", "token:" & Startinfos.Session.AccessToken & ":" & Startinfos.Session.SelectedProfile.Id})
                                          Dim jo As New JObject
                                          For Each item As authenticationDatabase.Userproperty In Startinfos.Session.User.Properties
                                              jo.Add(New JProperty(item.name, item.value))
                                          Next
                                          argumentreplacements.Add(New String() {"${user_properties}", jo.ToString})
                                          'TODO:
                                          'argumentreplacements.Add(New String() {"${user_type}", "mojang/legacy"})
                                          'Vielleicht twitch token aus einstellungen, so kann man auch cracked streamen
                                      End If

                                      For Each item As String() In argumentreplacements
                                          For i = 0 To minecraftArguments.Count - 1
                                              minecraftArguments.Item(i) = minecraftArguments.Item(i).Replace(item(0), item(1))
                                          Next
                                      Next

                                      If Startinfos.Server.ServerAdress <> Nothing Then
                                          minecraftArguments.Add("--server")
                                          minecraftArguments.Add(Startinfos.Server.ServerAdress)
                                          If Startinfos.Server.ServerPort <> Nothing Then
                                              minecraftArguments.Add("--port")
                                              minecraftArguments.Add(Startinfos.Server.ServerPort)
                                          End If
                                      End If

                                      If Startinfos.Profile.javaArgs <> Nothing Then
                                          javaargs = Startinfos.Profile.javaArgs
                                      Else
                                          'javaargs = "-Xmx" & RamCheck() & "M"
                                          javaargs = "-Xmx" & "1024" & "M"
                                      End If

                                      If Startinfos.Profile.resolution.height <> Nothing AndAlso Startinfos.Profile.resolution.height <> "0" Then
                                          height = "--height " & Startinfos.Profile.resolution.height
                                      Else
                                          height = Nothing
                                      End If

                                      If Startinfos.Profile.resolution.width <> Nothing AndAlso Startinfos.Profile.resolution.width <> "0" Then
                                          width = "--width " & Startinfos.Profile.resolution.width
                                      Else
                                          width = Nothing
                                      End If

                                      Teil_Arguments = String.Join(Chr(32), mainClass, String.Join(Chr(32), minecraftArguments), height, width)
                                      'TODO
                                      Arguments = javaargs & " -Djava.library.path=" & natives & " -cp " & libraries & Versionsjar & " " & Teil_Arguments
                                      If Startinfos.IsStarting = True Then
                                          Start_MC_Process(Teil_Arguments)
                                      End If
                                  End Sub))
    End Function

#Region "MinecraftStart"

    Async Sub Start_MC_Process(Optional Teil_Arguments As String = Nothing)
        If Teil_Arguments = Nothing Then Teil_Arguments = Arguments
        Write(Application.Current.FindResource("StartingMinecraft").ToString & " (" & String.Format(Application.Current.FindResource("JavaArchitecture").ToString, Await GetJavaArch()) & "): " & Teil_Arguments)
        mc = New Process()
        With mc.StartInfo
            .FileName = Startcmd(Startinfos.Profile)
            .Arguments = Arguments
            ' Arbeitsverzeichnis setzen falls nötig
            .WorkingDirectory = mcpfad.FullName
            ' kein Window erzeugen
            .CreateNoWindow = True
            ' UseShellExecute auf false setzen
            .UseShellExecute = False
            ' StandardOutput von Console umleiten
            .RedirectStandardError = True
            .RedirectStandardOutput = True
        End With
        ' Prozess starten
        mc.Start()
        mc.BeginErrorReadLine()
        mc.BeginOutputReadLine()
        If Startinfos.IsStarting = True Then
            Startinfos.IsStarting = False
            Startinfos.Server.JustStarted = False
            Startinfos.Versionsinfo = Nothing
            Startinfos.Profile = Nothing
            Startinfos.Version = Nothing
        End If
    End Sub

    Public Sub StartfromServerlist()
        If Startinfos.Server.JustStarted = False Then
            If MainViewModel.Instance.Directjoinaddress.Contains(":") = False Then
                Startinfos.Server.ServerAdress = MainViewModel.Instance.Directjoinaddress
            Else
                Dim address As String() = MainViewModel.Instance.Directjoinaddress.Split(CChar(":"))
                Startinfos.Server.ServerAdress = address(0)
                Startinfos.Server.ServerPort = address(1)
            End If
            Startinfos.Server.JustStarted = True
        End If
    End Sub

    Public Async Sub StartMC()
        If Startinfos.IsStarting = True Then
            Await Me.ShowMessageAsync(Application.Current.FindResource("Attention").ToString, Application.Current.FindResource("MinecraftAlreadyStarting").ToString & "!", MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = Application.Current.FindResource("OK").ToString, .ColorScheme = MetroDialogColorScheme.Accented})
        ElseIf cb_profiles.SelectedIndex = -1 Then
            Await Me.ShowMessageAsync(Nothing, Application.Current.FindResource("PleaseSelectProfile").ToString & "!", MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = Application.Current.FindResource("OK").ToString, .ColorScheme = MetroDialogColorScheme.Accented})
            'ElseIf tb_username.Text = Nothing Then
            '    Await Me.ShowMessageAsync(Nothing, "Gib bitte einen Usernamen ein!", MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = "Ok", .ColorScheme = MetroDialogColorScheme.Accented})
        Else
            If Startinfos.Profile Is Nothing Then
                Startinfos.Profile = Await Profiles.FromName(MainViewModel.Instance.selectedProfile)
            End If
            Await Versions_Load()
            tabitem_console.IsSelected = True
            mc = New Process
            If Await Check_Account() Then
                If Startinfos.Server.JustStarted = False Then
                    If cb_direct_join.IsChecked = True Then
                        If MainViewModel.Instance.Directjoinaddress <> Nothing Then
                            If Startinfos.Server.JustStarted = False Then
                                If MainViewModel.Instance.Directjoinaddress.Contains(":") = False Then
                                    Startinfos.Server.ServerAdress = MainViewModel.Instance.Directjoinaddress
                                Else
                                    Dim address As String() = MainViewModel.Instance.Directjoinaddress.Split(CChar(":"))
                                    Startinfos.Server.ServerAdress = address(0)
                                    Startinfos.Server.ServerPort = address(1)
                                End If
                                Startinfos.Server.JustStarted = True
                            End If
                        End If
                    Else
                        Startinfos.Server.ServerAdress = Nothing
                    End If
                End If


                If Startinfos.Version Is Nothing Then
                    If Startinfos.Profile.lastVersionId <> Nothing Then
                        'TODO
                        If Versions.versions.Select(Function(p) p.id).Contains(Startinfos.Profile.lastVersionId) Then
                            Startinfos.Version = Versions.versions.Where(Function(p) p.id = Startinfos.Profile.lastVersionId).First
                        Else
                            Write(String.Format(Application.Current.FindResource("JarOrJsonNotFound").ToString, Startinfos.Profile.lastVersionId, Startinfos.Profile.lastVersionId) & Environment.NewLine & Application.Current.FindResource("ReinstallForgeIfStarted").ToString & "!", LogLevel.ERROR)
                            Startinfos.Profile = Nothing
                            Exit Sub
                        End If
                    Else
                        If Startinfos.Profile.allowedReleaseTypes Is Nothing Then
                            Startinfos.Profile.allowedReleaseTypes = New List(Of String)
                        End If
                        'Wenn snapshots aktiviert sind, dann index 0, sonst latestrelease
                        If Startinfos.Profile.allowedReleaseTypes.Count > 0 Then
                            If Startinfos.Profile.allowedReleaseTypes.Contains("snapshot") = True Then
                                'Version mit Index 0 auslesen
                                Startinfos.Version = Versions.latest_version
                            Else
                                Startinfos.Version = Versions.versions.Where(Function(p) p.id = Versions.latest.release).First
                            End If
                        Else
                            Startinfos.Version = Versions.versions.Where(Function(p) p.id = Versions.latest.release).First
                        End If
                    End If
                End If
                Clear()
                Startinfos.IsStarting = True
                If Await MinecraftDownloadManager.DownloadVersion(Startinfos.Version) Then
                    If Startinfos.Versionsinfo Is Nothing Then
                        Startinfos.Versionsinfo = Await MinecraftDownloadManager.ParseVersionsInfo(Startinfos.Version)
                        If Startinfos.Versionsinfo.minimumLauncherVersion > supportedLauncherVersion Then
                            Main.Write(Application.Current.FindResource("VersionNotSupported").ToString, MainWindow.LogLevel.ERROR)
                        End If
                    End If
                    Startinfos.Versionsinfo = Await Startinfos.Versionsinfo.resolve
                    If Await MinecraftDownloadManager.DownloadResources() Then
                        If Await MinecraftDownloadManager.DownloadLibraries(Startinfos.Versionsinfo) Then
                            Await Get_Startinfos()
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub btn_startMC_Click(sender As Object, e As RoutedEventArgs) Handles btn_startMC.Click
        StartMC()
    End Sub

    Private Sub mc_ErrorDataReceived(sender As Object, e As DataReceivedEventArgs) Handles mc.ErrorDataReceived
        Try
            Write(e.Data)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub p_OutputDataReceived(ByVal sender As Object, ByVal e As DataReceivedEventArgs) Handles mc.OutputDataReceived
        Try
            Write(e.Data)
        Catch ex As Exception
        End Try
    End Sub

#End Region

#Region "LOG"
    Public Sub Append(ByVal Line As String, rtb As RichTextBox, Optional Color As Brush = Nothing)
        Me.Dispatcher.Invoke(New Action(Sub()
                                            Dim tr As New TextRange(rtb.Document.ContentEnd, rtb.Document.ContentEnd)
                                            tr.Text = Line & Environment.NewLine
                                            If Color IsNot Nothing Then
                                                tr.ApplyPropertyValue(TextElement.ForegroundProperty, Color)
                                            End If
                                            rtb.ScrollToEnd()
                                        End Sub))
    End Sub
    ''' <summary>
    ''' Schreibt eine Zeile in die tb_ausgabe
    ''' </summary>
    ''' <param name="Line">Die Zeile, die geschrieben werden soll</param>
    ''' <param name="LogLevel">Das Level der Nachricht</param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Line As String, Optional LogLevel As LogLevel = LogLevel.INFO)
        Select Case LogLevel
            Case MainWindow.LogLevel.INFO
                Dispatcher.Invoke(New WriteA(AddressOf Append), Line, tb_ausgabe)
            Case MainWindow.LogLevel.WARNING
                Dispatcher.Invoke(New WriteColored(AddressOf Append), "[" & Application.Current.FindResource("Warning").ToString & "] " & Line, tb_ausgabe, Brushes.Orange)
            Case MainWindow.LogLevel.ERROR
                Dispatcher.Invoke(New WriteColored(AddressOf Append), "[" & Application.Current.FindResource("Error").ToString & "] " & Line, tb_ausgabe, Brushes.Red)
        End Select
        '-Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true

        'Error:

        '2014-04-08 16:06:25 [Schwerwiegend] [ForgeModLoader] Technical information: The class net.minecraft.client.ClientBrandRetriever should have been associated with the minecraft jar file,
        'and should have returned us a valid, intact minecraft jar location. This did not work. Either you have modified the minecraft jar file (if so run the forge installer again),
        'or you are using a base editing jar that is changing this class (and likely others too). If you REALLY want to run minecraft in this configuration,
        'add the flag -Dfml.ignoreInvalidMinecraftCertificates=true to the 'JVM settings' in your launcher profile.
        If Line.Contains("add the flag -Dfml.ignoreInvalidMinecraftCertificates=true to the 'JVM settings' in your launcher profile") Then
            Write(Application.Current.FindResource("ignoreInvalidMinecraftCertificatesMessage").ToString, MainWindow.LogLevel.ERROR)
        End If
    End Sub
    Public Enum LogLevel
        INFO
        WARNING
        [ERROR]
    End Enum
    Public Sub Clear()
        Me.Dispatcher.Invoke(New Action(Sub()
                                            tb_ausgabe.SelectAll()
                                            tb_ausgabe.Selection.Text = Environment.NewLine
                                        End Sub))
    End Sub
#End Region
    Public Shared Function Startcmd(profile As Profiles.Profile) As String
        If profile.javaDir <> Nothing Then
            Return profile.javaDir
        Else
            Return GetJavaPath()
        End If
    End Function
    Public Shared Async Function GetJavaVersionInformation() As Task(Of String)
        Try
            Dim profile As Profiles.Profile = Nothing
            If Startinfos.IsStarting = True Then
                profile = Startinfos.Profile
            Else
                profile = Await Profiles.FromName(MainViewModel.Instance.selectedProfile)
            End If
            Dim procStartInfo As New System.Diagnostics.ProcessStartInfo(Startcmd(profile), "-version")

            procStartInfo.RedirectStandardOutput = True
            procStartInfo.RedirectStandardError = True
            procStartInfo.UseShellExecute = False
            procStartInfo.CreateNoWindow = True
            Dim proc As System.Diagnostics.Process = New Process()
            proc.StartInfo = procStartInfo
            proc.Start()

            Return proc.StandardError.ReadToEnd()
        Catch objException As Exception
            Return Nothing
        End Try
    End Function
    Public Shared Async Function GetJavaArch() As Task(Of Integer)
        Dim version As String = Await GetJavaVersionInformation()
        If version.Contains("64-Bit") Then
            Return 64
        Else
            Return 32
        End If
    End Function

    Private Sub tb_ausgabe_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tb_ausgabe.TextChanged
        tb_ausgabe.ScrollToEnd()
    End Sub

    Private Sub btn_new_profile_Click(sender As Object, e As RoutedEventArgs) Handles btn_new_profile.Click
        Newprofile = True
        Dim frm_ProfileEditor As New ProfileEditor
        frm_ProfileEditor.ShowDialog()
    End Sub

    Private Sub btn_edit_profile_Click(sender As Object, e As RoutedEventArgs) Handles btn_edit_profile.Click
        Newprofile = False
        Dim frm_ProfileEditor As New ProfileEditor
        frm_ProfileEditor.ShowDialog()
    End Sub

    Private Async Sub btn_delete_profile_Click(sender As Object, e As RoutedEventArgs) Handles btn_delete_profile.Click
        If MainViewModel.Instance.Profiles.Count > 1 Then
            Profiles.Remove(MainViewModel.Instance.selectedProfile)
            MainViewModel.Instance.selectedProfile = MainViewModel.Instance.Profiles.First
            Profiles.Get_Profiles()
        Else
            Await Me.ShowMessageAsync(Application.Current.FindResource("Error").ToString, Application.Current.FindResource("CannotDeleteLastProfile").ToString & "!", MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = Application.Current.FindResource("OK").ToString, .ColorScheme = MetroDialogColorScheme.Accented})
        End If
    End Sub

#Region "Mods"
    Public Sub Load_ModVersions()
        cb_modversions.Items.Clear()
        Dim modversionslist As IList(Of String) = Modifications.List_all_Mod_Vesions.Reverse.ToList
        For Each Modversion As String In modversionslist
            cb_modversions.Items.Add(Modversion)
        Next
        cb_modversions.SelectedIndex = 0
    End Sub
    Private Sub Filter_Mods()
        Dim selectedmod As String = Nothing
        If lb_mods.SelectedItem IsNot Nothing Then selectedmod = DirectCast(lb_mods.SelectedItem, Modifications.Mod).name
        lb_mods.Items.Clear()
        Modifications.Check_installed(modsfolderPath)
        If cb_modversions.SelectedItem IsNot Nothing Then
            Dim mods_with_selectedversion As IList(Of Modifications.Mod) = Modifications.ModList.Where(Function(p) p.versions.Select(Function(i) i.version).Contains(cb_modversions.SelectedItem.ToString)).ToList
            For Each Moditem As Modifications.Mod In mods_with_selectedversion
                If Moditem.name.ToLower.Contains(tb_search_mods.Text.ToLower) = True Then
                    lb_mods.Items.Add(Moditem)
                End If
            Next
            If selectedmod <> Nothing Then
                If lb_mods.Items.Cast(Of Modifications.Mod).Select(Function(p) p.name).Contains(selectedmod) Then
                    lb_mods.SelectedItem = lb_mods.Items.Cast(Of Modifications.Mod).Where(Function(p) p.name = selectedmod).First
                Else
                    If lb_mods.Items.Count > 0 Then
                        lb_mods.SelectedIndex = 0
                    End If
                End If
            Else
                If lb_mods.Items.Count > 0 Then
                    lb_mods.SelectedIndex = 0
                End If
            End If
        End If
    End Sub
    Private Sub cb_modversions_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cb_modversions.SelectionChanged
        Filter_Mods()
        SelectedModVersion = cb_modversions.SelectedItem.ToString
    End Sub
    Private Sub tb_search_mods_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tb_search_mods.TextChanged
        Filter_Mods()
    End Sub
    Private Sub btn_website_Click(sender As Object, e As RoutedEventArgs) Handles btn_website.Click
        Process.Start(DirectCast(lb_mods.SelectedItem, Modifications.Mod).website)
    End Sub
    Private Sub btn_video_Click(sender As Object, e As RoutedEventArgs) Handles btn_video.Click
        Process.Start(DirectCast(lb_mods.SelectedItem, Modifications.Mod).video)
    End Sub
    Private Sub lb_mods_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lb_mods.SelectionChanged
        'Load_Modinfos
        If lb_mods.SelectedIndex <> -1 Then
            Dim selected As Modifications.Mod = DirectCast(lb_mods.SelectedItem, Modifications.Mod)
            If selected.versions.Where(Function(p) p.version = cb_modversions.SelectedItem.ToString).ToList.First.installed = True Then
                img_installed.Source = ImageConvert.GetImageStream(My.Resources.check_green)
                btn_list_delete_mod.IsEnabled = True
            Else
                img_installed.Source = Nothing
                btn_list_delete_mod.IsEnabled = False
            End If
            'cb_mods_description_language.Items.Clear()
            'For Each Language As String In selected.descriptions.Select(Function(p) p.id)
            '    cb_mods_description_language.Items.Add(Language)
            'Next

            'TODO: Move into Converter and set the first selected item to the selected language, instead of de:
            'If selected.descriptions.Select(Function(p) p.id).Contains("de") Then
            '    cb_mods_description_language.SelectedItem = cb_mods_description_language.Items.Cast(Of Modifications.Mod.Description).Where(Function(p) p.id = "de").First
            'ElseIf selected.descriptions.Select(Function(p) p.id).Contains("en") Then
            '    cb_mods_description_language.SelectedItem = cb_mods_description_language.Items.Cast(Of Modifications.Mod.Description).Where(Function(p) p.id = "en").First
            'Else
            '    cb_mods_description_language.SelectedItem = selected.descriptions.First.id
            'End If
            Select Case selected.type
                Case "forge"
                    lbl_type.Content = String.Format("{0}: {1} ({2}->{1})", Application.Current.FindResource("Prerequisite").ToString, Application.Current.FindResource("MinecraftForge").ToString, Application.Current.FindResource("Tools").ToString)
                Case "liteloader"
                    lbl_type.Content = String.Format("{0}: {1} ({2}->{1})", Application.Current.FindResource("Prerequisite").ToString, Application.Current.FindResource("LiteLoader").ToString, Application.Current.FindResource("Tools").ToString)
                Case Else
                    lbl_type.Content = Application.Current.FindResource("Typ").ToString & ": " & DirectCast(lb_mods.SelectedItem, Modifications.Mod).type
            End Select
        End If
    End Sub

    'Private Sub cb_mods_description_language_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cb_mods_description_language.SelectionChanged
    '    If lb_mods.SelectedIndex <> -1 Then
    '        If cb_mods_description_language.SelectedIndex <> -1 Then
    '            Dim selected As Modifications.Mod = DirectCast(lb_mods.SelectedItem, Modifications.Mod)
    '            tb_description.Text = selected.descriptions.Where(Function(p) p.id = cb_mods_description_language.SelectedItem.ToString).First.text
    '        End If
    '    End If
    'End Sub

    Private Async Function RefreshMods() As Task
        Try
            If rb_mods_profile.IsChecked = True Then
                Dim profile As Profiles.Profile = Await Profiles.FromName(cb_mods_profilename.SelectedItem.ToString)
                modsfolderPath = IO.Path.Combine(profile.gameDir, "mods")
            ElseIf rb_mods_folder.IsChecked = True Then
                modsfolderPath = tb_modsfolder.Text
            End If
            Dim selectedversion As String = cb_modversions.SelectedItem.ToString
            Dim selectedmod As Integer = lb_mods.SelectedIndex
            Dim SelectedItems As IList(Of String) = DirectCast(lb_mods.SelectedItems, IList(Of Modifications.Mod)).Select(Function(p) p.id).ToList
            Load_ModVersions()
            Filter_Mods()
            For i = 0 To lb_mods.Items.Count - 1
                If SelectedItems.Contains(DirectCast(lb_mods.Items.Item(i), Modifications.Mod).id) Then
                    lb_mods.SelectedItems.Add(lb_mods.Items.Item(i))
                End If
            Next
            cb_modversions.SelectedItem = selectedversion
            lb_mods.SelectedIndex = selectedmod
        Catch
        End Try
    End Function
    Private Sub btn_resetmodsfoler_Click(sender As Object, e As RoutedEventArgs) Handles btn_resetmodsfoler.Click
        tb_modsfolder.Text = modsfolder.FullName
    End Sub
    Private Sub btn_selectmodsfolder_Click(sender As Object, e As RoutedEventArgs) Handles btn_selectmodsfolder.Click
        Try
            If modsfolder.Exists = False Then
                modsfolder.Create()
            End If
        Catch
        End Try
        Dim fd As New VistaFolderBrowserDialog
        fd.UseDescriptionForTitle = True
        fd.Description = Application.Current.FindResource("SelectModsFolder").ToString
        fd.RootFolder = Environment.SpecialFolder.MyComputer
        fd.SelectedPath = modsfolder.FullName
        fd.ShowNewFolderButton = True
        If fd.ShowDialog = True Then
            tb_modsfolder.Text = fd.SelectedPath
        End If
    End Sub
    Private Sub tb_modsfolder_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tb_modsfolder.TextChanged
        Try
            If rb_mods_folder.IsChecked = True Then
                modsfolderPath = tb_modsfolder.Text
                Filter_Mods()
            End If
        Catch
        End Try
    End Sub
    Private Sub rb_mods_folder_Checked(sender As Object, e As RoutedEventArgs) Handles rb_mods_folder.Checked
        If tb_modsfolder IsNot Nothing AndAlso tb_modsfolder.Text <> Nothing Then
            modsfolderPath = tb_modsfolder.Text
            Filter_Mods()
        End If
    End Sub
    Private Async Sub rb_mods_profile_Checked(sender As Object, e As RoutedEventArgs) Handles rb_mods_profile.Checked
        If cb_mods_profilename.SelectedIndex <> -1 Then
            Dim profile As Profiles.Profile = Await Profiles.FromName(cb_mods_profilename.SelectedItem.ToString)
            If profile.gameDir = Nothing Then
                profile.gameDir = mcpfad.FullName
            End If
            modsfolderPath = IO.Path.Combine(profile.gameDir, "mods")
            Filter_Mods()
        End If
    End Sub
    Private Async Sub cb_mods_profilename_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cb_mods_profilename.SelectionChanged
        If rb_mods_profile.IsChecked = True Then
            If cb_mods_profilename.SelectedIndex <> -1 Then
                Dim profile As Profiles.Profile = Await Profiles.FromName(cb_mods_profilename.SelectedItem.ToString)
                If profile.gameDir = Nothing Then
                    profile.gameDir = mcpfad.FullName
                End If
                modsfolderPath = IO.Path.Combine(profile.gameDir, "mods")
                Filter_Mods()
            End If
        End If
    End Sub
    Private Async Sub btn_list_delete_mod_Click(sender As Object, e As RoutedEventArgs) Handles btn_list_delete_mod.Click
        Dim Version As Modifications.Mod.Version = DirectCast(lb_mods.SelectedItem, Modifications.Mod).versions.Where(Function(p) p.version = cb_modversions.SelectedItem.ToString).First
        Await Delete_Mod(Version)
    End Sub
    Public Async Function Delete_Mod(Version As Modifications.Mod.Version) As Task
        Dim capturedException As ExceptionDispatchInfo = Nothing
        For Each selectedmod As Modifications.Mod In lb_mods.SelectedItems
            Try
                Dim Struktur As String = Version.version & "\" & Version.version & "-" & selectedmod.id & "." & Version.extension
                If File.Exists(tb_modsfolder.Text & "\" & Struktur) = True Then
                    File.Delete(tb_modsfolder.Text & "\" & Struktur)
                End If
            Catch ex As Exception
                capturedException = ExceptionDispatchInfo.Capture(ex)
            End Try
            If capturedException IsNot Nothing Then
                Await Me.ShowMessageAsync(Application.Current.FindResource("Error").ToString, String.Format(Application.Current.FindResource("ErrorDeletingMod").ToString & "!", selectedmod.name))
            End If
        Next
        Dim selected As Integer = lb_mods.SelectedIndex
        Filter_Mods()
        lb_mods.SelectedIndex = selected

    End Function
    Private Async Sub btn_downloadmod_Click(sender As Object, e As RoutedEventArgs) Handles btn_downloadmod.Click
        If moddownloading = True Then
            'Await Me.ShowMessageAsync("Download läuft", "Eine Mod wird bereits heruntergeladen. Warte bitte, bis diese fertig ist!", MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = Application.Current.FindResource("OK").ToString, .ColorScheme = MetroDialogColorScheme.Accented})
        Else
            modsdownloadingversion = cb_modversions.SelectedItem.ToString
            btn_resetmodsfoler.IsEnabled = False
            btn_selectmodsfolder.IsEnabled = False
            btn_downloadmod.IsEnabled = False
            btn_list_delete_mod.IsEnabled = False
            cb_mods_profilename.IsEnabled = False
            rb_mods_folder.IsEnabled = False
            rb_mods_profile.IsEnabled = False
            moddownloading = True
            modsdownloadlist.Clear()
            For Each selectedmod As Modifications.Mod In lb_mods.SelectedItems
                If modsdownloadlist.Select(Function(p) p.id).Contains(selectedmod.id) = False Then
                    modsdownloadlist.Add(selectedmod)
                    For Each item As String In Modifications.Dependencies(selectedmod.id, modsdownloadingversion)
                        Dim moditem As Modifications.Mod = Modifications.ModList.Where(Function(p) p.id = item).First
                        If modsdownloadlist.Select(Function(p) p.id).Contains(moditem.id) = False Then
                            modsdownloadlist.Add(moditem)
                        End If
                    Next
                End If
            Next
            modsdownloadindex = 0

            controller = Await Me.ShowProgressAsync(Application.Current.FindResource("InstallingMods").ToString, Application.Current.FindResource("PleaseWait").ToString)
            'controller.SetCancelable(True)

            download_mod()
        End If
    End Sub
    Private Async Sub download_mod()
        If modsdownloadindex < modsdownloadlist.Count Then
            Dim url As New Uri(modsdownloadlist.Item(modsdownloadindex).versions.Where(Function(p) p.version = modsdownloadingversion).First.downloadlink)
            Dim progress As Double = modsdownloadindex / modsdownloadlist.Count
            controller.SetProgress(progress)
            controller.SetMessage(modsdownloadindex + 1 & " / " & modsdownloadlist.Count & " " & modsdownloadlist.Item(modsdownloadindex).name)
            Modsfilename = modsdownloadingversion & "\" & modsdownloadingversion & "-" & modsdownloadlist.Item(modsdownloadindex).id & "." & modsdownloadlist.Item(modsdownloadindex).versions.Where(Function(p) p.version = modsdownloadingversion).First.extension
            Dim capturedException As ExceptionDispatchInfo = Nothing
            Try
                If IO.Directory.Exists(IO.Path.GetDirectoryName(cachefolder.FullName & "\" & Modsfilename)) = False Then
                    IO.Directory.CreateDirectory((IO.Path.GetDirectoryName(cachefolder.FullName & "\" & Modsfilename)))
                End If
                wcmod.DownloadFileAsync(url, Path.Combine(cachefolder.FullName, Modsfilename))
            Catch ex As Exception
                capturedException = ExceptionDispatchInfo.Capture(ex)
                Exit Sub
            End Try
            If capturedException IsNot Nothing Then
                Await Mod_Download_finished(capturedException.SourceException.Message)
            End If
        Else
            Await Mod_Download_finished()
        End If
    End Sub
    Private Async Sub wcmod_DownloadFileCompleted(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs) Handles wcmod.DownloadFileCompleted
        If e.Error IsNot Nothing Then
            Await Mod_Download_finished(e.Error.Message)
            controller.SetCancelable(True)
            While controller.IsCanceled = False
                Await Task.Delay(10)
            End While
            Await controller.CloseAsync()
        ElseIf e.Cancelled = True Then
            'lbl_mods_status.Content = "Abgebrochen
            controller.SetMessage(Application.Current.FindResource("Canceled").ToString)
        Else
            Try
                Dim path As String = modsfolderPath & "\" & Modsfilename
                If IO.Directory.Exists(IO.Path.GetDirectoryName(path)) = False Then
                    IO.Directory.CreateDirectory((IO.Path.GetDirectoryName(path)))
                End If
                My.Computer.FileSystem.MoveFile(IO.Path.Combine(cachefolder.FullName, Modsfilename), path)
            Catch
            End Try
            modsdownloadindex += 1
            Dim selected As Integer = lb_mods.SelectedIndex
            Filter_Mods()
            lb_mods.SelectedIndex = selected
            download_mod()
        End If
    End Sub

    Private Async Function Mod_Download_finished(Optional Errormessage As String = Nothing) As Task
        If Errormessage = Nothing Then
            controller.SetMessage(Application.Current.FindResource("InstallingModsSuccessful").ToString)
            Await Task.Delay(1000)
            Await controller.CloseAsync()
        Else
            controller.SetMessage(Application.Current.FindResource("ErrorInstallingMods").ToString & ": " & Errormessage)
            controller.SetCancelable(True)
        End If
        moddownloading = False
        btn_resetmodsfoler.IsEnabled = True
        btn_selectmodsfolder.IsEnabled = True
        btn_downloadmod.IsEnabled = True
        btn_list_delete_mod.IsEnabled = True
        cb_mods_profilename.IsEnabled = True
        rb_mods_folder.IsEnabled = True
        rb_mods_profile.IsEnabled = True
        Dim selected As Integer = lb_mods.SelectedIndex
        Filter_Mods()
        lb_mods.SelectedIndex = selected
    End Function
    Private Sub wcmod_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles wcmod.DownloadProgressChanged
        If modsdownloadindex < modsdownloadlist.Count Then
            Dim progress As Double = modsdownloadindex / modsdownloadlist.Count + e.ProgressPercentage / 100 / modsdownloadlist.Count
            controller.SetProgress(progress)
        End If
    End Sub

#End Region

#Region "Server"

    Async Function Load_Servers() As Task
        servers_dat.Refresh()
        lb_servers.Items.Clear()
        If servers_dat.Exists = True Then
            lbl_no_servers.Visibility = Windows.Visibility.Collapsed
            MainViewModel.Instance.Servers = New ObservableCollection(Of ServerList.Server)
            Await ServerList.Load()
            If MainViewModel.Instance.Servers.Count = 0 Then
                lbl_no_servers.Visibility = Windows.Visibility.Visible
            Else
                lbl_no_servers.Visibility = Windows.Visibility.Collapsed
            End If
            '    Dim servers_nbtfile As New fNbt.NbtFile
            '    servers_nbtfile.LoadFromFile(servers_dat)
            '    Dim servers_tag As NbtList = servers_nbtfile.RootTag.Get(Of NbtList)("servers")
            '    Dim lol As NbtCompound = CType(servers_tag.Last, NbtCompound)
            '    servers.Servers.Clear()
            '    lb_servers.Items.Clear()
            '    'servers = servers_tag.Select(Function(p) New Server(p.Item("name").StringValue, p.Item("ip").StringValue, Convert.ToBoolean(p.Item("hideAddress").ByteValue), If(p.Item("icon").HasValue, ImageConvert.GetImageStream(ImageConvert.GetImageFromString(p.Item("icon").StringValue)), ImageConvert.GetImageStream(My.Resources.transparent64_64)))).ToList
            '    For Each item As NbtCompound In servers_tag
            '        Dim name As String = item.Item("name").StringValue
            '        Dim ip As String = item.Item("ip").StringValue
            '        Dim hideAddress As Boolean = Convert.ToBoolean(item.Item("hideAddress").ByteValue)
            '        Dim icon As String = Nothing
            '        If item.Tags.Select(Function(p) p.Name).Contains("icon") = False Then
            '            'Überarbeiten
            '            icon = Nothing
            '        Else
            '            icon = item.Item("icon").StringValue
            '        End If
            '        Dim server As New Server() With {
            '                .name = name,
            '                .ip = ip,
            '                .hideAddress = hideAddress,
            '                .icon = icon
            '            }
            '        servers.Servers.Add(server)
            '    Next
            For Each item As ServerList.Server In MainViewModel.Instance.Servers
                lb_servers.Items.Add(item)
            Next
            If lb_servers.SelectedIndex = -1 Then
                lb_servers.SelectedIndex = 0
            Else
                lb_servers.SelectedIndex = lb_servers.SelectedIndex
            End If
            '    'Icon tag ist enthalten?!?
        Else
            lbl_no_servers.Visibility = Windows.Visibility.Visible
            'Es existieren keine Server
            'MessageBox.Show("Die Server Datei existiert nicht!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
        End If
    End Function

    Sub Ping_servers()
        Dim fThread = New Thread(New ThreadStart(AddressOf ThreadProc))
        fThread.IsBackground = True
        fThread.Start()
    End Sub

    Private Sub ThreadProc()
        Try
            Parallel.For(0, MainViewModel.Instance.Servers.Count, Sub(b)
                                                                      CheckOnline(b)
                                                                  End Sub)
        Catch
        End Try
        If servers_dat.Exists AndAlso Not servers_dat.IsLocked Then
            ServerList.Save()
        End If
    End Sub

    Private Sub CheckOnline(ByVal i As Integer)
        Try
            'MessageBox.Show(String.Join(" | ", host, port))

            'Dim pinger As New OldServerPing(host, port, 0, 15000)
            'servers.Item(i).OldServerPing = pinger
            'Dispatcher.Invoke(New Action(Sub()
            '                                 Dim selected As Integer = lb_servers.SelectedIndex
            '                                 lb_servers.Items.RemoveAt(i)
            '                                 lb_servers.Items.Insert(i, servers.Servers.Item(i))
            '                                 lb_servers.SelectedIndex = selected
            '                             End Sub))
            MainViewModel.Instance.Servers.Item(i).DoPing()
            'MsgBox(servers.Item(i).ServerStatus.Players.MaxPlayers)
            'Dispatcher.Invoke(New Action(Sub()
            '                                 Dim selected As Integer = lb_servers.SelectedIndex
            '                                 lb_servers.Items.RemoveAt(i)
            '                                 lb_servers.Items.Insert(i, MainViewModel.Instance.Servers.Item(i))
            '                                 lb_servers.SelectedIndex = selected
            '                             End Sub))
        Catch null As ArgumentNullException
            'hostNameOrAddress ist null.
        Catch socket As SocketException
            'Beim Auflösen von hostNameOrAddress ist ein Fehler aufgetreten.
        Catch argument As ArgumentException
            'hostNameOrAddress ist keine gültige IP-Adresse.
        End Try

    End Sub

    Private Async Sub btn_refresh_servers_Click(sender As Object, e As RoutedEventArgs) Handles btn_refresh_servers.Click
        Await Load_Servers()
        Ping_servers()
    End Sub

    Private Async Sub btn_add_servers_Click(sender As Object, e As RoutedEventArgs) Handles btn_add_servers.Click
        Dim editor As New ServerEditor()
        Dim result As Boolean?
        editor.ShowDialog()
        result = editor.DialogResult
        If result = True Then
            Await Load_Servers()
            Ping_servers()
        End If
    End Sub

    Private Async Sub btn_edit_servers_Click(sender As Object, e As RoutedEventArgs) Handles btn_edit_servers.Click
        If lb_servers.SelectedIndex <> -1 Then
            Dim server As ServerList.Server = DirectCast(lb_servers.SelectedItem, ServerList.Server)
            Dim editor As New ServerEditor(lb_servers.SelectedIndex, server)
            Dim result As Boolean?
            editor.ShowDialog()
            result = editor.DialogResult
            If result = True Then
                Await Load_Servers()
                Ping_servers()
            End If
        End If
    End Sub

    Private Async Sub btn_delete_servers_Click(sender As Object, e As RoutedEventArgs) Handles btn_delete_servers.Click
        Dim selected As Integer = lb_servers.SelectedIndex
        If selected <> -1 Then
            Dim result As MessageDialogResult = Await Me.ShowMessageAsync(Application.Current.FindResource("DeleteServer").ToString, String.Format(Application.Current.FindResource("DeleteServerMessage").ToString, DirectCast(lb_servers.SelectedItem, ServerList.Server).name), MessageDialogStyle.AffirmativeAndNegative, New MetroDialogSettings() With {.AffirmativeButtonText = Application.Current.FindResource("Yes").ToString, .NegativeButtonText = Application.Current.FindResource("No").ToString, .ColorScheme = MetroDialogColorScheme.Accented})
            If result = MessageDialogResult.Affirmative Then
                lb_servers.Items.RemoveAt(selected)
                MainViewModel.Instance.Servers.RemoveAt(selected)
                ServerList.Save()
            End If
        End If
    End Sub

    Sub copy_server_address()
        If lb_servers.SelectedIndex <> -1 Then
            Clipboard.SetText(DirectCast(lb_servers.SelectedItem, ServerList.Server).ip)
        End If
    End Sub

    Sub join_server_from_list()
        If lb_servers.SelectedIndex <> -1 Then
            MainViewModel.Instance.Directjoinaddress = DirectCast(lb_servers.SelectedItem, ServerList.Server).ip
            cb_direct_join.IsChecked = True
            tabitem_Minecraft.IsSelected = True
        End If
    End Sub

    Async Function join_server_from_list_auto() As Task
        tabitem_Minecraft.IsSelected = True
        Dim selected As Object = lb_servers.SelectedItem
        Dim Threadbusy As Boolean = True
        Dim fThread = New Thread(New ThreadStart(Async Sub()
                                                     Try
                                                         'Dispatcher.Invoke(New Action(Async Function()
                                                         Dim ip As String = DirectCast(selected, ServerList.Server).ip
                                                         Dim Version As String = DirectCast(selected, ServerList.Server).ServerStatus.Version.Name
                                                         If Startinfos.Server.JustStarted = False Then
                                                             If ip.Contains(":") = False Then
                                                                 Startinfos.Server.ServerAdress = ip
                                                             Else
                                                                 Dim address As String() = ip.Split(CChar(":"))
                                                                 Startinfos.Server.ServerAdress = address(0)
                                                                 Startinfos.Server.ServerPort = address(1)
                                                             End If
                                                             Startinfos.Server.JustStarted = True
                                                         End If
                                                         Await Versions_Load()
                                                         '1.6.2-1.7.4

                                                         Dim check As Boolean = True
                                                         Dim Servers() As String = {"Spigot", "CraftBukkit", "BungeeCord"}
                                                         For Each item As String In Servers
                                                             If Version.Contains(item) Then
                                                                 Dim ver As String = Version.Replace(item & " ", "")
                                                                 Startinfos.Version = Versions.versions.Where(Function(p) p.id = ver).FirstOrDefault
                                                                 check = False
                                                                 Exit For
                                                             End If
                                                         Next
                                                         If check = True Then
                                                             If Version.Split(CChar(" ")).Count > 1 Then
                                                                 Startinfos.Version = Versions.versions.Where(Function(p) p.id = Version.Split(CChar(" ")).Last).FirstOrDefault
                                                             ElseIf Version.Contains("-") = True Then
                                                                 Startinfos.Version = Versions.versions.Where(Function(p) p.id = Version.Split(CChar("-"))(1)).FirstOrDefault
                                                             Else
                                                                 Startinfos.Version = Versions.versions.Where(Function(p) p.id = Version).FirstOrDefault
                                                             End If
                                                         Else
                                                             Startinfos.Version = Versions.versions.Where(Function(p) p.id = Version).FirstOrDefault
                                                         End If
                                                         Startinfos.Profile = New Profiles.Profile()
                                                         Threadbusy = False
                                                     Catch ex As Exception
                                                         Threadbusy = False
                                                     End Try
                                                 End Sub))
        fThread.IsBackground = True
        fThread.Start()
        While Threadbusy = True
            Await Task.Delay(10)
        End While
        StartMC()
    End Function

#End Region

    Private Sub cb_direct_join_Click(sender As Object, e As RoutedEventArgs) Handles cb_direct_join.Click
        MainViewModel.Instance.Settings.DirectJoin = cb_direct_join.IsChecked.Value
        MainViewModel.Instance.Settings.Save()
    End Sub

#Region "Tools"

    Private Sub forge_installer()
        Dim frg As New Forge_installer
        frg.Show()
    End Sub

    Sub liteloader_installer()
        Dim frg As New LiteLoader_installer
        frg.Show()
    End Sub

    Private Async Sub download_feedthebeast()
        toolscontroller = Await Me.ShowProgressAsync(Application.Current.FindResource("DownloadingFTB").ToString, Application.Current.FindResource("PleaseWait").ToString)
        Dim url As New Uri(Downloads.Downloadsjo("feedthebeast").Value(Of String)("url"))
        Dim filename As String = Downloads.Downloadsjo("feedthebeast").Value(Of String)("filename")
        Dim path As New FileInfo(IO.Path.Combine(mcpfad.FullName, "tools", filename))
        If path.Directory.Exists = False Then
            path.Directory.Create()
        End If
        Try
            btn_start_feedthebeast.IsEnabled = False
            'progressbar lädt herunter
            Dim a As New WebClient
            AddHandler a.DownloadProgressChanged, AddressOf Toolsdownloadpreogresschanged
            Await a.DownloadFileTaskAsync(url, path.FullName)
            Await toolscontroller.CloseAsync()
            btn_start_feedthebeast.IsEnabled = True
        Catch ex As Exception
            Try
                If path.Exists Then
                    path.Delete()
                End If
            Catch
            End Try
            btn_start_feedthebeast.IsEnabled = False
            Me.ShowMessageAsync("Fehler", ex.Message, MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = "Ok", .ColorScheme = MetroDialogColorScheme.Accented})
        End Try
    End Sub

    Sub Toolsdownloadpreogresschanged(sender As Object, e As DownloadProgressChangedEventArgs)
        toolscontroller.SetProgress(e.ProgressPercentage / 100)
    End Sub

    Private Sub start_feedthebeast()
        Dim filename As String = Downloads.Downloadsjo("feedthebeast").Value(Of String)("filename")
        Dim path As New FileInfo(IO.Path.Combine(mcpfad.FullName, "tools", filename))
        Process.Start(path.FullName)
    End Sub

    Private Async Sub download_techniclauncher()
        toolscontroller = Await Me.ShowProgressAsync(Application.Current.FindResource("DownloadingTechnicLauncher").ToString, Application.Current.FindResource("PleaseWait").ToString)
        Dim url As String = "http://launcher.technicpack.net/launcher/{0}/TechnicLauncher.jar"
        Dim filename As String = "TechnicLauncher.jar"
        Dim path As New FileInfo(IO.Path.Combine(mcpfad.FullName, "tools", filename))
        If path.Directory.Exists = False Then
            path.Directory.Create()
        End If
        Try
            btn_start_techniclauncher.IsEnabled = False
            'progressbar lädt herunter
            Dim laststablebuild As String = Await New WebClient().DownloadStringTaskAsync("http://build.technicpack.net/job/TechnicLauncher/Stable/buildNumber")
            url = String.Format(url, laststablebuild)
            Dim a As New WebClient
            AddHandler a.DownloadProgressChanged, AddressOf Toolsdownloadpreogresschanged
            Await a.DownloadFileTaskAsync(url, path.FullName)
            Await toolscontroller.CloseAsync()
            btn_start_techniclauncher.IsEnabled = True
        Catch ex As Exception
            Try
                If path.Exists Then
                    path.Delete()
                End If
            Catch
            End Try
            btn_start_techniclauncher.IsEnabled = True
            Me.ShowMessageAsync("Fehler", ex.Message, MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = "Ok", .ColorScheme = MetroDialogColorScheme.Accented})
        End Try
    End Sub

    Private Sub start_techniclauncher()
        Dim filename As String = "TechnicLauncher.jar"
        Dim path As New FileInfo(IO.Path.Combine(mcpfad.FullName, "tools", filename))
        Process.Start(path.FullName)
        'TechnicLauncher starten
    End Sub

    Sub Check_Tools_Downloaded()
        'For Each item As Stirng in
        'TechnicLauncher
        Dim technicfilename As String = "TechnicLauncher.jar"
        Dim feedthebeastfilename As String = Downloads.Downloadsjo("feedthebeast").Value(Of String)("filename").ToString
        If File.Exists(Path.Combine(mcpfad.FullName, "tools", technicfilename)) = True Then
            btn_start_techniclauncher.IsEnabled = True
        Else
            btn_start_techniclauncher.IsEnabled = False
        End If
        If File.Exists(Path.Combine(mcpfad.FullName, "tools", feedthebeastfilename)) = True Then
            btn_start_feedthebeast.IsEnabled = True
        Else
            btn_start_feedthebeast.IsEnabled = False
        End If
    End Sub

#End Region

    Private Sub MainWindow_StateChanged(sender As Object, e As EventArgs) Handles Me.StateChanged
        MainViewModel.Instance.Settings.WindowState = Me.WindowState
        MainViewModel.Instance.Settings.Save()
    End Sub

#Region "Auth"
    Private Async Sub cb_profiles_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cb_profiles.SelectionChanged
        Await Check_Account()
    End Sub

    ''' <summary>
    ''' Login with accesstoken. Returns true if successfully logged in, otherwise false.
    ''' </summary>
    ''' <returns>Returns true if successfully logged in, otherwise false.</returns>
    ''' <remarks></remarks>
    Async Function Check_Account() As Task(Of Boolean)
        If cb_profiles.SelectedIndex <> -1 Then
            Await authenticationDatabase.Load()
            Dim profile As Profiles.Profile = Await Profiles.FromName(MainViewModel.Instance.selectedProfile)
            If profile.playerUUID = Nothing Then
                LoginScreen.Open()
                TabControl_main.Visibility = Windows.Visibility.Collapsed
                Return False
            Else
                Dim capturedException As MinecraftAuthenticationException = Nothing
                'Login with access token
                Try
                    If authenticationDatabase.List.Select(Function(p) p.uuid.Replace("-", "")).Contains(profile.playerUUID) Then
                        Dim Account = authenticationDatabase.List.Where(Function(p) p.uuid.Replace("-", "") = profile.playerUUID).First
                        If Guid.TryParse(Account.userid, New Guid) Then
                            If Startinfos.Session Is Nothing Then
                                Startinfos.Session = New Session() With {.AccessToken = Account.accessToken,
                                                                         .ClientToken = authenticationDatabase.clientToken,
                                                                         .SelectedProfile = Nothing}
                            End If
                            If MainViewModel.Instance.Account Is Nothing OrElse Not MainViewModel.Instance.Account.uuid = Account.uuid Then
                                Startinfos.Session = New Session() With {.AccessToken = Account.accessToken,
                                      .ClientToken = authenticationDatabase.clientToken,
                                      .SelectedProfile = Nothing}
                                Write(Application.Current.FindResource("LoggingInWithAccessToken").ToString)
                                Await Startinfos.Session.Refresh()
                                authenticationDatabase.List.Where(Function(p) p.uuid.Replace("-", "") = profile.playerUUID).First.accessToken = Startinfos.Session.AccessToken
                                Await authenticationDatabase.Save()
                            Else
                                'Just logged in
                                Startinfos.Session.AccessToken = Account.accessToken
                                Startinfos.Session.ClientToken = authenticationDatabase.clientToken
                            End If
                        Else
                            Startinfos.Session = New Session() With {.ClientToken = authenticationDatabase.clientToken,
                                                                  .SelectedProfile = New Profile() With {.Name = Account.displayName}}
                        End If
                        'This comes last:
                        MainViewModel.Instance.Account = Account
                        Return True
                    Else
                        LoginScreen.Open()
                        TabControl_main.Visibility = Windows.Visibility.Collapsed
                        Return False
                    End If
                Catch ex As MinecraftAuthenticationException
                    capturedException = ex
                End Try
                If capturedException IsNot Nothing Then
                    Await Me.ShowMessageAsync(capturedException.Error, capturedException.ErrorMessage, MessageDialogStyle.Affirmative)
                    If capturedException.ErrorMessage = "Invalid token." Then
                        If MainViewModel.Instance.Account IsNot Nothing Then
                            Dim Account = MainViewModel.Instance.Account
                            authenticationDatabase.List.Remove(authenticationDatabase.List.Where(Function(p) p.uuid = Account.uuid).First)
                            Await authenticationDatabase.Save()
                        End If
                    End If
                    LoginScreen.Open()
                    TabControl_main.Visibility = Windows.Visibility.Collapsed
                End If
                Return False
                End If
        Else
                Return False
        End If
    End Function

    'Async Function ShowUsername_Avatar(Account As authenticationDatabase.Account) As Task
    '    lbl_Username.Text = Account.displayName
    '    lbl_user_state.Text = If(Guid.TryParse(Account.userid, New Guid), Application.Current.FindResource("Premium").ToString, Application.Current.FindResource("Cracked").ToString)
    '    lbl_user_state.Foreground = If(Guid.TryParse(Account.userid, New Guid), Brushes.Green, Brushes.Red)
    '    Try
    '        Dim WebRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create("https://minotar.net/avatar/" & Account.displayName & "/100"), HttpWebRequest)
    '        Using WebReponse As HttpWebResponse = DirectCast(Await WebRequest.GetResponseAsync, HttpWebResponse)
    '            Using stream As Stream = WebReponse.GetResponseStream
    '                img_avatar.Source = ImageConvert.GetImageStream(System.Drawing.Image.FromStream(stream))
    '            End Using
    '        End Using
    '    Catch ex As WebException
    '        'Failed to load Avatar
    '    End Try
    'End Function

    Private Async Sub btn_logout_Click(sender As Object, e As RoutedEventArgs) Handles btn_logout.Click
        'logout / invalidate session
        Dim capturedException As MinecraftAuthenticationException = Nothing
        Try
            Dim profile As Profiles.Profile = Await Profiles.FromName(MainViewModel.Instance.selectedProfile)
            profile.playerUUID = Nothing
            Await Profiles.Edit(MainViewModel.Instance.selectedProfile, profile)
            LoginScreen.Open()
            TabControl_main.Visibility = Windows.Visibility.Collapsed
        Catch ex As MinecraftAuthenticationException
            capturedException = ex
        End Try
        If capturedException IsNot Nothing Then
            Await Me.ShowMessageAsync(capturedException.Error, capturedException.ErrorMessage, MessageDialogStyle.Affirmative)
        End If
    End Sub

#End Region

    Sub OnDataUpdated(data As IList(Of Object), changedItem As Object, oldindex As Integer, newIndex As Integer)

        MainViewModel.Instance.Servers.Move(oldindex, newIndex)
        ServerList.Save()
        'MessageBox.Show(String.Join(Environment.NewLine, MainViewModel.Instance.Servers))
    End Sub
End Class