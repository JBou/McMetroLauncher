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

#End Region

Public Class MainWindow
#Region "Variables"
    '****************Webclients*****************
    WithEvents wcresources As New System.Net.WebClient ' Für das WebClient steuerelement mit Events z.b. DownloadProgressChanged... 
    WithEvents wcversionsdownload As New System.Net.WebClient
    WithEvents wcversionsjsondownload As New System.Net.WebClient
    WithEvents wcindexes As New System.Net.WebClient
    WithEvents wcversionsstring As New System.Net.WebClient
    WithEvents wc_libraries As New System.Net.WebClient
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
    '************Resources Download*************
    Private resourcesdownloading As Boolean
    Private resourcesindexes As resourcesindex
    Private resourcesdownloadindex As Integer
    Private resourcesdownloadtry As Integer
    Private currentresourcesobject As resourcesindexobject
    '************Libraries Download*************
    Private librariesdownloading As Boolean
    Private librariesdownloadtry As Integer
    Private librariesdownloadindex As Integer
    Private librariesdownloadfailures As Integer
    Private Currentlibrary As Library
    Private Currentlibrarysha1 As String
    Private Tounpack As Boolean
    Private downloadforgelib As Boolean
    '******************Assets*******************
    Private assets_index_name As String
    '********************UI*********************
    Public controller As ProgressDialogController
    Public toolscontroller As ProgressDialogController
    '******************Others*******************
#End Region

#Region "GUI Update"
    Public Delegate Sub refresh_pb_download_Value(Value As Double)
    Public Delegate Sub refresh_pb_download_IsIndeterminate(Value As Boolean)
    Public Delegate Sub refresh_lbl_downloadstatus_Content(Value As String)

    Public Sub pb_download_Value(Value As Double)
        pb_download.Dispatcher.Invoke(New refresh_pb_download_Value(AddressOf set_pb_download_value), Value)
    End Sub
    Public Sub pb_download_IsIndeterminate(Value As Boolean)
        pb_download.Dispatcher.Invoke(New refresh_pb_download_IsIndeterminate(AddressOf set_pb_download_IsIndeterminate), Value)
    End Sub
    Public Sub lbl_downloadstatus_Content(Value As String)
        lbl_downloadstatus.Dispatcher.Invoke(New refresh_lbl_downloadstatus_Content(AddressOf set_lbl_downloadstatus_Content), Value)
    End Sub

    Private Async Function set_pb_download_value(Value As Double) As Task
        Await Task.Factory.StartNew(Sub()
                                        pb_download.Value = Value
                                    End Sub, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext())
    End Function
    Private Async Function set_pb_download_IsIndeterminate(Value As Boolean) As Task
        Await Task.Factory.StartNew(Sub()
                                        pb_download.IsIndeterminate = Value
                                    End Sub, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext())
    End Function
    Private Async Function set_lbl_downloadstatus_Content(Value As String) As Task
        Await Task.Factory.StartNew(Sub()
                                        lbl_downloadstatus.Content = Value
                                    End Sub, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext())
    End Function

#End Region

#Region "Mainwindow Events"
    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Me.DataContext = ViewModel
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

            wcresources.CancelAsync()
            wcversionsdownload.CancelAsync()
            wcindexes.CancelAsync()
            wcversionsstring.CancelAsync()
            wc_libraries.CancelAsync()

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
        If lb_mods.SelectedIndex <> -1 Then
            Dim youtubeMatch As Match = YoutubeVideoRegex.Match(DirectCast(lb_mods.SelectedItem, Modifications.Mod).video)
            Dim id As String = String.Empty
            If youtubeMatch.Success Then
                id = youtubeMatch.Groups(1).Value
                wc_mod_video.Source = New Uri("http://www.youtube.com/embed/" & id)
            End If
        End If
        While Webcontrol_news.IsLoading = True
            Await Task.Delay(10)
        End While
        Webcontrol_news.Visibility = Windows.Visibility.Visible
        lbl_news_loading.Visibility = Windows.Visibility.Collapsed
        pb_news_loading.Visibility = Windows.Visibility.Collapsed
    End Sub
#End Region

    Sub IsThemeChanged(sender As Object, e As OnThemeChangedEventArgs)
        Settings.Settings.Theme = e.AppTheme.Name
        Settings.Settings.Accent = e.Accent.Name
        Settings.Save()
    End Sub

    Private Sub ShowSettings(sender As Object, e As RoutedEventArgs)
        'Contrrols auf ihre Einstellungen setzen
        ToggleFlyout(0)
    End Sub

    Public Sub Open_Website()
        Process.Start("http://patzleiner.net")
    End Sub

    Public Sub Open_Github()
        Process.Start("https://github.com/JBou/McMetroLauncher")
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

    Async Sub Download_Resources()
        If Startinfos.Versionsinfo Is Nothing Then
            Await Parse_VersionsInfo(Startinfos.Version)
        End If
        If Startinfos.Versionsinfo.assets <> Nothing Then
            assets_index_name = Startinfos.Versionsinfo.assets
        Else
            assets_index_name = "legacy"
        End If
        resourcesdownloading = True
        pb_download_IsIndeterminate(True)
        If resources_dir.Exists = False Then
            resources_dir.Create()
        End If
        If indexesfile(assets_index_name).Exists = False Then
            Write("Lade Resourcen-Liste herunter")
            AddHandler wcindexes.DownloadFileCompleted, AddressOf downloadindexesfinished
            If cacheindexesfile(assets_index_name).Directory.Exists = False Then
                cacheindexesfile(assets_index_name).Directory.Create()
            End If
            wcindexes.DownloadFileAsync(New Uri(indexesurl(assets_index_name)), cacheindexesfile(assets_index_name).FullName)
        Else
            Await Parse_Resources()
        End If
        pb_download_IsIndeterminate(False)
    End Sub

    Async Sub downloadindexesfinished(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs)
        Try
            If indexesfile(assets_index_name).Directory.Exists = False Then
                indexesfile(assets_index_name).Directory.Create()
            End If
            If indexesfile(assets_index_name).Exists Then
                indexesfile(assets_index_name).Delete()
            End If
            cacheindexesfile(assets_index_name).CopyTo(indexesfile(assets_index_name).FullName, True)
        Catch
        End Try
        Await Parse_Resources()
    End Sub

    Async Function Parse_Resources() As Task
        pb_download.IsIndeterminate = False
        Await Task.Run(New Action(Sub()
                                      Write("Lade Resourcen herunter")
                                      Dim indexjo As JObject = JObject.Parse(File.ReadAllText(indexesfile(assets_index_name).FullName))
                                      Dim indexesobjects As IList(Of resourcesindexobject) = New List(Of resourcesindexobject)
                                      Dim virtual As Boolean
                                      If indexjo.Properties.Select(Function(p) p.Name).Contains("virtual") = False Then
                                          virtual = False
                                      Else
                                          virtual = Convert.ToBoolean(indexjo("virtual"))
                                      End If
                                      For i = 0 To indexjo("objects").Values.Count - 1
                                          Dim keys As List(Of JProperty) = indexjo.Value(Of JObject)("objects").Properties.ToList
                                          Dim key As String = keys.Item(i).Name
                                          'indexjo.Values(Of JProperty)("objects").ElementAt(i).ToString()
                                          Dim hash As String = keys.Item(i).Value.Value(Of String)("hash")
                                          Dim size As Integer = CInt(keys.Item(i).Value.Value(Of String)("size"))
                                          Dim item As New resourcesindexobject(key, hash, size)
                                          indexesobjects.Add(item)
                                      Next
                                      resourcesindexes = New resourcesindex(virtual, indexesobjects)
                                      resourcesdownloadindex = 0
                                      resourcesdownloadtry = 1
                                      resourcesdownloading = True
                                      DownloadResources()
                                  End Sub))
    End Function

    Sub DownloadResources()
        If resourcesdownloadindex < resourcesindexes.objects.Count Then
            pb_download_Value(resourcesdownloadindex / resourcesindexes.objects.Count - 1 * 100)
            currentresourcesobject = resourcesindexes.objects.Item(resourcesdownloadindex)
            Dim resource As New FileInfo(resourcefile(currentresourcesobject.hash).FullName.Replace("/", "\"))
            Dim todownload As Boolean = True
            If resource.Exists Then
                'Hash überprüfen
                If SHA1FileHash(resource.FullName).ToLower = currentresourcesobject.hash Then
                    'Diese Resource überspringen
                    todownload = False
                Else
                    'Diese Resource Downloaden
                    todownload = True
                End If
            Else
                todownload = True
            End If
            If todownload = True Then
                If resource.Directory.Exists = False Then
                    resource.Directory.Create()
                End If
                wcresources.DownloadFileAsync(New Uri(resourceurl(currentresourcesobject.hash)), resource.FullName)
                Write("Resource wird heruntergeladen (Versuch " & resourcesdownloadtry & "): " & resource.FullName)
            Else
                resourcesdownloadindex += 1
                DownloadResources()
            End If
        Else
            'Downloads fertig
            DownloadResourcesfinished()
        End If
    End Sub

    Private Sub wcresources_DownloadFileCompleted(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs) Handles wcresources.DownloadFileCompleted
        If e.Cancelled = False And e.Error Is Nothing Then
            If resourcesdownloadtry > 3 Then
                Write("Der Download wurde aufgrund zu vieler Fehlversuche abgebrochen!", LogLevel.ERROR)
                Startinfos.IsStarting = False
                resourcesdownloading = False
                Exit Sub
            Else
                'Hash überprüfen
                If SHA1FileHash(resourcefile(currentresourcesobject.hash).FullName).ToLower = currentresourcesobject.hash Then
                    'Nächste Resource Downloaden
                    resourcesdownloadindex += 1
                    resourcesdownloadtry = 1
                    Write("Resource erfolgreich heruntergeladen und Hash verglichen")
                Else
                    'Resource erneut heruntergeladen, Versuch erhöhen:
                    resourcesdownloadtry += 1
                End If
                DownloadResources()
            End If
        End If
    End Sub

    Sub DownloadResourcesfinished()
        resourcesdownloading = False
        If resourcesindexes.virtual = True Then
            'Alle keys in den ordner :"virtual\legacy" kopieren
            Write("Virtuelle Resourcen werden erstellt")
            For Each item As resourcesindexobject In resourcesindexes.objects
                Dim destination As New FileInfo(Path.Combine(assetspath.FullName, "virtual", assets_index_name, item.key.Replace("/", "\")))
                If destination.Directory.Exists = False Then
                    destination.Directory.Create()
                End If
                Try
                    If destination.Exists = True Then
                        destination.Delete()
                    End If
                    resourcefile(item.hash).CopyTo(destination.FullName)
                Catch ex As Exception
                End Try
            Next
        End If
        If Startinfos.IsStarting = True Then
            Download_Libraries()
        End If
    End Sub

    Private Sub wc_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles wcresources.DownloadProgressChanged, wc_libraries.DownloadProgressChanged, wcversionsdownload.DownloadProgressChanged
        Dim totalbytes As Double = e.TotalBytesToReceive / 1000
        Dim bytes As Double = e.BytesReceived / 1000
        Dim Einheit As String = "KB"
        If totalbytes >= 1000 Then
            totalbytes = e.TotalBytesToReceive / 1000000
            bytes = e.BytesReceived / 1000000
            Einheit = "MB"
        End If
        'lbl_downloadstatus_Content(String.Format("{0}% - {1} {2} von {3} {4} heruntergeladen", e.ProgressPercentage, Math.Round(bytes, 2), Einheit, Math.Round(totalbytes, 2), Einheit))
    End Sub

    Private Sub wc_libraries_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles wc_libraries.DownloadProgressChanged
        If librariesdownloadindex < Startinfos.Versionsinfo.libraries.Count Then
            pb_download_Value((librariesdownloadindex / Startinfos.Versionsinfo.libraries.Count) * 100 + e.ProgressPercentage / Startinfos.Versionsinfo.libraries.Count)
        End If
    End Sub

    Public Function SHA1FileHash(ByVal sFile As String) As String
        Dim SHA1 As New SHA1CryptoServiceProvider
        Dim Hash As Byte()
        Dim Result As String = Nothing

        Dim FN As New FileStream(sFile, FileMode.Open, FileAccess.Read, FileShare.Read, 8192)
        SHA1.ComputeHash(FN)
        FN.Close()

        Hash = SHA1.Hash
        Result = Strings.Replace(BitConverter.ToString(Hash), "-", "")
        Return Result
    End Function

    Public Function MD5FileHash(ByVal sFile As String) As String
        Dim MD5 As New MD5CryptoServiceProvider
        Dim Hash As Byte()
        Dim Result As String = ""
        Dim Tmp As String = ""

        Dim FN As New FileStream(sFile, FileMode.Open, FileAccess.Read, FileShare.Read, 8192)
        MD5.ComputeHash(FN)
        FN.Close()

        Hash = MD5.Hash
        Result = Strings.Replace(BitConverter.ToString(Hash), "-", "")
        Return Result
    End Function

    Async Sub Download_Version()
        Dim versionid As String = Startinfos.Version.id
        Dim VersionsURl As String = "https://s3.amazonaws.com/Minecraft.Download/versions/" & versionid & "/" & versionid & ".jar"
        Dim Outputfile As New FileInfo(Path.Combine(versionsfolder.FullName, versionid, versionid & ".jar"))
        Dim CacheOutputfile As New FileInfo(Path.Combine(cachefolder.FullName, "versions", versionid, versionid & ".jar"))

        'Jar File download
        Write("Prüfe, ob die Version aktuell ist")
        Dim todownload As Boolean = False
        If Outputfile.Exists = False Then
            todownload = True
        Else
            If Versions.versions.Where(Function(p) p.custom = False).Select(Function(p) p.id).Contains(versionid) Then
                Try
                    Dim Request As HttpWebRequest = DirectCast(WebRequest.Create(VersionsURl), HttpWebRequest)
                    Request.Timeout = 5000
                    Dim Response As WebResponse = Await Request.GetResponseAsync
                    Dim etag As String = Response.Headers("ETag")
                    Response.Close()
                    Dim md5 As String = MD5FileHash(Outputfile.FullName).ToLower
                    If etag <> Chr(34) & md5 & Chr(34) Then
                        todownload = True
                    End If
                Catch
                    todownload = False
                End Try
            Else
                todownload = False
            End If
        End If
        If todownload = True Then
            Write("Lade Minecraft Version " & versionid & " herunter")
            Try
                If CacheOutputfile.Directory.Exists = False Then
                    CacheOutputfile.Directory.Create()
                End If
                wcversionsdownload = New WebClient
                wcversionsdownload.DownloadFileAsync(New Uri(VersionsURl), CacheOutputfile.FullName)
                AddHandler wcversionsdownload.DownloadFileCompleted, AddressOf versiondownloadfinished
            Catch ex As Exception
                Write("Fehler beim herunterladen von Minecraft " & versionid & " :" & Environment.NewLine & ex.Message, LogLevel.ERROR)
                Startinfos.IsStarting = False
            End Try
        Else
            Await downloadversionsjson()
        End If
    End Sub

    Public Async Sub versiondownloadfinished(sender As Object, e As ComponentModel.AsyncCompletedEventArgs)
        Dim versionid As String = Startinfos.Version.id
        Dim Outputfile As New FileInfo(Path.Combine(versionsfolder.FullName, versionid, versionid & ".jar"))
        Dim CacheOutputfile As New FileInfo(Path.Combine(cachefolder.FullName, "versions", versionid, versionid & ".jar"))

        If e.Error Is Nothing And e.Cancelled = False Then
            If Outputfile.Exists Then
                Outputfile.Delete()
            End If
            If Outputfile.Directory.Exists = False Then
                Outputfile.Directory.Create()
            End If
            CacheOutputfile.MoveTo(Outputfile.FullName)
            Await downloadversionsjson()
        Else
            Write("Fehler beim herunterladen von Minecraft " & Startinfos.Version.id & " :" & Environment.NewLine & e.Error.Message, LogLevel.ERROR)
            Startinfos.IsStarting = False
        End If
    End Sub

    Async Function downloadversionsjson() As Task
        Dim versionid As String = Startinfos.Version.id
        Dim VersionsJSONURL As String = "https://s3.amazonaws.com/Minecraft.Download/versions/" & versionid & "/" & versionid & ".json"
        Dim OutputfileJSON As New FileInfo(Path.Combine(versionsfolder.FullName, versionid, versionid & ".json"))
        Dim CacheOutputfileJSON As New FileInfo(Path.Combine(cachefolder.FullName, "versions", versionid, versionid & ".json"))
        Dim Request As HttpWebRequest = DirectCast(WebRequest.Create(VersionsJSONURL), HttpWebRequest)
        Request.Timeout = 5000
        'Json File download
        Dim jsontodownload As Boolean = False
        If OutputfileJSON.Exists = False Then
            jsontodownload = True
        Else
            If Versions.versions.Where(Function(p) p.custom = False).Select(Function(p) p.id).Contains(versionid) Then
                Try
                    Dim Response As WebResponse = Await Request.GetResponseAsync
                    Dim etag As String = Response.Headers("ETag")
                    Response.Close()
                    Dim md5 As String = MD5FileHash(OutputfileJSON.FullName).ToLower
                    If etag <> Chr(34) & md5 & Chr(34) Then
                        jsontodownload = True
                    End If
                Catch
                    jsontodownload = False
                End Try
            Else
                jsontodownload = False
            End If
        End If
        If jsontodownload = True Then
            If CacheOutputfileJSON.Directory.Exists = False Then
                CacheOutputfileJSON.Directory.Create()
            End If
            If CacheOutputfileJSON.Exists Then
                CacheOutputfileJSON.Delete()
            End If
            Try
                wcversionsjsondownload = New WebClient
                wcversionsjsondownload.DownloadFileAsync(New Uri(VersionsJSONURL), CacheOutputfileJSON.FullName)
                AddHandler wcversionsjsondownload.DownloadFileCompleted, AddressOf jsondownloadfinished
            Catch ex As Exception
                Write("Fehler beim herunterladen von Minecraft " & versionid & " :" & Environment.NewLine & ex.Message, LogLevel.ERROR)
                Startinfos.IsStarting = False
            End Try
        Else
            Await Versiondownloadfinished_start()
        End If
    End Function

    Public Async Sub jsondownloadfinished(sender As Object, e As ComponentModel.AsyncCompletedEventArgs)
        Dim versionid As String = Startinfos.Version.id
        Dim OutputfileJSON As New FileInfo(Path.Combine(versionsfolder.FullName, versionid, versionid & ".json"))
        Dim CacheOutputfileJSON As New FileInfo(Path.Combine(cachefolder.FullName, "versions", versionid, versionid & ".json"))
        If e.Error Is Nothing And e.Cancelled = False Then
            If OutputfileJSON.Exists Then
                OutputfileJSON.Delete()
            End If
            If OutputfileJSON.Directory.Exists = False Then
                OutputfileJSON.Directory.Create()
            End If
            CacheOutputfileJSON.MoveTo(OutputfileJSON.FullName)
            Await Versiondownloadfinished_start()
        ElseIf e.Error IsNot Nothing Then
            Write("Ein Fehler ist beim herunterladen der Version aufgetreten: " & e.Error.Message, LogLevel.ERROR)
            Startinfos.IsStarting = False
        End If
    End Sub

    Async Function Versiondownloadfinished_start() As Task
        'Start 
        If Startinfos.Versionsinfo Is Nothing Then
            Await Parse_VersionsInfo(Startinfos.Version)
            If Startinfos.Versionsinfo.minimumLauncherVersion > supportedLauncherVersion Then
                Write("Diese Minecraft Version wird vom Launcher noch nicht vollständig unterstützt. Es könnte zu Fehlern kommen!", LogLevel.ERROR)
            End If
        End If
        If Startinfos.IsStarting = True Then
            Download_Resources()
        End If
    End Function

    Async Sub Download_Libraries()
        If Startinfos.Versionsinfo Is Nothing Then
            Await Parse_VersionsInfo(Startinfos.Version)
        End If
        'http://wiki.vg/Game_Files
        librariesdownloadindex = 0
        librariesdownloadtry = 1
        librariesdownloading = True
        librariesdownloadfailures = 0
        DownloadLibraries()
    End Sub

    Async Function Parse_VersionsInfo(Version As Versionslist.Version) As Task
        Dim o As String = File.ReadAllText(versionsJSON(Version))
        Dim javaarch As Integer = Await GetJavaArch()
        o = o.Replace("${arch}", javaarch.ToString)
        'Converter because some Forge Versions(10.12.0.1054) has Double as minimumlauncherversion, expected integer. It rounds the double to an integer
        Startinfos.Versionsinfo = Await JsonConvert.DeserializeObjectAsync(Of VersionsInfo)(o, New JsonSerializerSettings() With {.NullValueHandling = NullValueHandling.Ignore, .Converters = New List(Of JsonConverter) From {New CustomIntConverter()}})
        Startinfos.Versionsinfo.JObject = JObject.Parse(o)
    End Function

    Async Sub DownloadLibraries()
        Try
            pb_download_Value(librariesdownloadindex / Startinfos.Versionsinfo.libraries.Count * 100)
            If librariesdownloadindex < Startinfos.Versionsinfo.libraries.Count Then
                Currentlibrary = Startinfos.Versionsinfo.libraries.Item(librariesdownloadindex)
                Dim allowdownload As Boolean = True
                If Currentlibrary.rules Is Nothing Then
                    allowdownload = True
                Else
                    If Currentlibrary.rules.Select(Function(p) p.action).Contains("allow") Then
                        If Currentlibrary.rules.Where(Function(p) p.action = "allow").First.os IsNot Nothing Then
                            If Currentlibrary.rules.Where(Function(p) p.action = "allow").First.os.name = "windows" Then
                                allowdownload = True
                            Else
                                allowdownload = False
                            End If
                        End If
                    ElseIf Currentlibrary.rules.Select(Function(p) p.action).Contains("disallow") Then
                        If Currentlibrary.rules.Where(Function(p) p.action = "disallow").First.os IsNot Nothing Then
                            If Currentlibrary.rules.Where(Function(p) p.action = "disallow").First.os.name = "windows" Then
                                allowdownload = False
                            Else
                                allowdownload = True
                            End If
                        End If
                    End If
                End If
                If allowdownload = True Then
                    Dim todownload As Boolean = True
                    Dim url As String = Nothing
                    If Startinfos.Versionsinfo.libraries.ElementAt(librariesdownloadindex).url = Nothing Then
                        url = librariesurl & Currentlibrary.path
                    Else
                        Dim customurl As String = Startinfos.Versionsinfo.libraries.ElementAt(librariesdownloadindex).url
                        url = customurl & Currentlibrary.path
                    End If
                    Dim librarypath As New FileInfo(IO.Path.Combine(librariesfolder.FullName, Currentlibrary.path))
                    Dim a As New WebClient()
                    If librarypath.Directory.Exists = False Then
                        librarypath.Directory.Create()
                    End If
                    Try
                        Await a.DownloadFileTaskAsync(New Uri(url & ".sha1"), librarypath.FullName & ".sha1")
                    Catch e As Exception
                        Currentlibrarysha1 = Nothing
                    End Try
                    Currentlibrarysha1 = File.ReadAllText(librarypath.FullName & ".sha1")
                    If librarypath.Exists Then
                        If String.IsNullOrWhiteSpace(Currentlibrarysha1) Then
                            todownload = False
                        Else
                            If SHA1FileHash(librarypath.FullName).ToLower = Currentlibrarysha1 Then
                                todownload = False
                            Else
                                todownload = True
                            End If
                        End If
                    Else
                        todownload = True
                    End If
                    If todownload = True Then
                        'Download
                        If librarypath.Directory.Exists = False Then
                            librarypath.Directory.Create()
                        End If
                        'Falls es ein MinecraftForge Build ist, url ändern
                        downloadforgelib = False
                        Dim forgeuniversal As Boolean = False
                        Dim version As String = Currentlibrary.name.Split(CChar(":"))(2)
                        'legacy = "minecraftforge", new versions = "forge"
                        If Currentlibrary.name.Split(CChar(":"))(1) = "minecraftforge" Or Currentlibrary.name.Split(CChar(":"))(1) = "forge" And Forge.ForgeList.Select(Function(p) p.version).Contains(version) Then
                            Dim mcversion As String = Forge.ForgeList.Where(Function(p) p.version = version).First.mcversion
                            'Buildlist durchsuchen
                            downloadforgelib = True
                            forgeuniversal = True
                            If Forge.LegacyBuildList.Select(Function(p) p.version).Contains(version) Then
                                url = String.Format("http://files.minecraftforge.net/minecraftforge/minecraftforge-universal-{1}-{0}.jar", version, mcversion)
                            Else
                                url = String.Format("http://files.minecraftforge.net/maven/net/minecraftforge/forge/{1}-{0}/forge-{1}-{0}-universal.jar", version, mcversion)
                            End If
                        End If
                        Dim outputfile As String = librarypath.FullName
                        'Auser bei forge universal lib
                        If url.Contains("files.minecraftforge.net") And forgeuniversal = False Then
                            downloadforgelib = True
                            Tounpack = True
                            url = url.Insert(url.Length, ".pack.xz")
                            outputfile = outputfile.Insert(outputfile.Length, ".pack.xz")
                            Write("Library wird von den Forge Servern heruntergeladen (Versuch " & librariesdownloadtry & "): " & librarypath.FullName)
                        Else
                            If forgeuniversal = True Then
                                Write("Minecraft Forge Library wird automatisch heruntergeladen (Versuch " & librariesdownloadtry & "): " & librarypath.FullName)
                            Else
                                Write("Versuche Library von alternativer Quelle herunterzuladen herunterzuladen (Versuch " & librariesdownloadtry & "): " & librarypath.FullName)
                            End If
                        End If
                        If downloadforgelib = False Then
                            Write("Library wird heruntergeladen (Versuch " & librariesdownloadtry & "): " & outputfile)
                        End If
                        wc_libraries.DownloadFileAsync(New Uri(url), outputfile)
                    Else
                        If String.IsNullOrWhiteSpace(Currentlibrarysha1) Then
                            Write("Library konnte nicht auf Hash überprüft werden und wird übersprungen, in der Annahme, dass die lokale Datei gut ist: " & librarypath.FullName, LogLevel.WARNING)
                        Else
                            Write("Library existiert bereits: " & librarypath.FullName)
                        End If
                        librariesdownloadindex += 1
                        DownloadLibraries()
                    End If
                Else
                    librariesdownloadindex += 1
                    DownloadLibraries()
                End If
            Else
                'Downloads fertig
                DownloadLibrariesfinished()
            End If
        Catch Ex As Exception
            Write(Ex.Message & Environment.NewLine & Ex.StackTrace, LogLevel.ERROR)
            Startinfos.IsStarting = False
        End Try
    End Sub

    Private Async Sub wc_libraries_DownloadFileCompleted(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs) Handles wc_libraries.DownloadFileCompleted
        If e.Error IsNot Nothing Then
            Write("Library konnte nicht heruntergeladen werden: " & e.Error.Message & Environment.NewLine & "Falls du gerade Forge gestartet hast, installiere es erneut!", LogLevel.ERROR)
            librariesdownloadindex += 1
            librariesdownloadtry = 1
            librariesdownloadfailures += 1
            Try
                File.Delete(Path.Combine(librariesfolder.FullName, Currentlibrary.path))
            Catch Ex As Exception
                Write("Ein Fehler ist beim Löschen einer Library aufgetreten!", LogLevel.WARNING)
            End Try
            DownloadLibraries()
        End If
        If e.Cancelled = False And e.Error Is Nothing Then
            Dim libpath As String = Path.Combine(librariesfolder.FullName, Currentlibrary.path)
            If Tounpack = True Then
                Dim input As New FileInfo(libpath & ".pack.xz")
                Dim output As New FileInfo(libpath)
                Write("Library wird entpackt: " & libpath)
                If Await Unpack.Unpack(input, output) = False Then
                    'ShowError
                    Write("Fehler beim entpacken von: " & libpath)
                End If
                input.Delete()
            End If
            If librariesdownloadtry > 3 Then
                librariesdownloadfailures += 1
                Write("Der Download wurde aufgrund zu vieler Fehlversuche abgebrochen!" & Environment.NewLine & "Falls du gerade Forge gestartet hast, installiere es erneut!", LogLevel.ERROR)
                Startinfos.IsStarting = False
                librariesdownloading = False
            Else
                If Tounpack = True Then
                    Tounpack = False
                    'Nächste Library Downloaden
                    librariesdownloadindex += 1
                    librariesdownloadtry = 1
                    Write("Library erfolgreich heruntergeladen")
                Else
                    'Hash überprüfen
                    If SHA1FileHash(libpath).ToLower = Currentlibrarysha1 Then
                        'Nächste Library Downloaden
                        librariesdownloadindex += 1
                        librariesdownloadtry = 1
                        Write("Library erfolgreich heruntergeladen und Hash verglichen")
                    Else
                        'Library erneut heruntergeladen, Versuch erhöhen:
                        librariesdownloadtry += 1
                    End If
                End If
                DownloadLibraries()
            End If
        End If
    End Sub

    Sub DownloadLibrariesfinished()
        librariesdownloading = False
        If Startinfos.IsStarting = True Then
            Get_Startinfos()
        End If
    End Sub

    Async Function Unzip() As Task
        Await Task.Run(New Action(Sub()
                                      Try
                                          Write("Natives werden entpackt")
                                          UnpackDirectory = Path.Combine(versionsfolder.FullName, Startinfos.Version.id, Startinfos.Version.id & "-natives-" & DateTime.Now.Ticks.ToString)
                                          If startedversions.Contains(UnpackDirectory) = False Then
                                              startedversions.Add(UnpackDirectory)
                                          End If
                                          For Each item In Startinfos.Versionsinfo.libraries.Where(Function(p) p.natives IsNot Nothing)
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
                                                  If .natives IsNot Nothing And allowdownload = True Then
                                                      If .natives.windows <> Nothing Then
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
                                                              Write("Fehler beim entpacken der natives: " & ex.Message, LogLevel.ERROR)
                                                          End Try
                                                      End If
                                                  End If
                                              End With
                                          Next
                                      Catch ex As Exception
                                          Write("Fehler beim entpacken der natives. Wahrscheinlich wurde die erforderliche Library nicht heruntergeladen" & Environment.NewLine & ex.Message, LogLevel.ERROR)
                                      End Try
                                  End Sub))
    End Function

    Async Sub Get_Startinfos()
        Await Unzip()
        Write("Startinfos werden ausgelesen")
        Dim mainClass As String = Startinfos.Versionsinfo.mainClass
        Dim minecraftArguments As List(Of String) = Startinfos.Versionsinfo.minecraftArguments.Split(Chr(32)).ToList
        Dim libraries As String = Nothing
        Dim gamedir As String = Nothing
        Dim argumentreplacements As List(Of String()) = New List(Of String())
        Dim natives As String = UnpackDirectory
        Dim javaargs As String = Nothing
        Dim height As String = Nothing
        Dim width As String = Nothing


        Await Task.Run(New Action(Async Sub()
                                      Versionsjar = Path.Combine(versionsfolder.FullName, Startinfos.Version.id, Startinfos.Version.id & ".jar")
                                      'Split by Space --> (Chr(32))
                                      If Startinfos.Versionsinfo Is Nothing Then
                                          Await Parse_VersionsInfo(Startinfos.Version)
                                      End If
                                      For i = 0 To Startinfos.Versionsinfo.libraries.Count - 1
                                          Dim librarytemp As Library = Startinfos.Versionsinfo.libraries.Item(i)
                                          If librarytemp.natives Is Nothing Then
                                              libraries &= Path.Combine(librariesfolder.FullName, librarytemp.path.Replace("/", "\") & ";")
                                          Else
                                              If librarytemp.natives.windows IsNot Nothing Then
                                                  libraries &= Path.Combine(librariesfolder.FullName, librarytemp.path.Replace("/", "\") & ";")
                                              End If
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
                                      If resourcesindexes.virtual = True Then
                                          assets_dir = Path.Combine(assetspath.FullName, "virtual", assets_index_name)
                                      End If
                                      argumentreplacements.Add(New String() {"${auth_player_name}", Startinfos.Session.SelectedProfile.Name})
                                      argumentreplacements.Add(New String() {"${version_name}", Startinfos.Version.id})
                                      argumentreplacements.Add(New String() {"${game_directory}", gamedir})
                                      argumentreplacements.Add(New String() {"${game_assets}", assets_dir})
                                      argumentreplacements.Add(New String() {"${assets_root}", assets_dir})
                                      argumentreplacements.Add(New String() {"${assets_index_name}", assets_index_name})
                                      argumentreplacements.Add(New String() {"${user_properties}", New JObject().ToString})

                                      If Startinfos.Session.OnlineMode = True Then
                                          'session username
                                          argumentreplacements.Add(New String() {"${auth_uuid}", Startinfos.Session.SelectedProfile.Id})
                                          argumentreplacements.Add(New String() {"${auth_access_token}", Startinfos.Session.AccessToken})
                                          argumentreplacements.Add(New String() {"${auth_session}", "token:" & Startinfos.Session.AccessToken & ":" & Startinfos.Session.SelectedProfile.Id})
                                          Dim jo As New JObject
                                          For Each item As authenticationDatabase.Userproperty In Startinfos.Session.User.properties
                                              jo.Add(New JProperty(item.name, item.value))
                                          Next
                                          argumentreplacements.Add(New String() {"${user_properties}", jo.ToString})
                                          'TODO:
                                          'argumentreplacements.Add(New String() {"${user_type}", "mojang/legacy"})
                                          'Else
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

                                      If Startinfos.Profile.resolution.height <> Nothing And Startinfos.Profile.resolution.height <> "0" Then
                                          height = " --height " & Startinfos.Profile.resolution.height
                                      Else
                                          height = Nothing
                                      End If

                                      If Startinfos.Profile.resolution.width <> Nothing And Startinfos.Profile.resolution.width <> "0" Then
                                          width = " --width " & Startinfos.Profile.resolution.width
                                      Else
                                          width = Nothing
                                      End If

                                      Arguments = javaargs & " -Djava.library.path=" & natives & " -cp " & libraries & Versionsjar & " " & mainClass & " " & String.Join(Chr(32), minecraftArguments) & height & width
                                  End Sub))
        'StartArgumente und mainclass ... von JSON IO.File
        'Überprüfen, ob username eingegeben wurde!
        'Libraries zum Start hinzufügen!
        If Startinfos.IsStarting = True Then
            Start_MC_Process(mainClass & " " & String.Join(Chr(32), minecraftArguments) & height & width)
        End If
    End Sub

    Async Sub Start_MC_Process(Optional Teil_Arguments As String = Nothing)
        ' Anwendungspfad setzen -> hier liegt es im Anwendungsordner
        If Startcmd(Startinfos.Profile) = Nothing Then
            Dim result As MessageDialogResult = Await Me.ShowMessageAsync("Java nicht vorhanden", "Du musst Java installieren, um Minecraft zu spielen. Jetzt herunterladen?", MessageDialogStyle.AffirmativeAndNegative)
            If result = MessageDialogResult.Affirmative Then
                Process.Start("http://java.com/de/download")
            End If
            Exit Sub
        End If
        If Teil_Arguments = Nothing Then Teil_Arguments = Arguments
        Write("Starte Minecraft (Java " & Await GetJavaArch() & " Bit): " & Teil_Arguments)
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
            .RedirectStandardInput = True
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
            If ViewModel.Directjoinaddress.Contains(":") = False Then
                Startinfos.Server.ServerAdress = ViewModel.Directjoinaddress
            Else
                Dim address As String() = ViewModel.Directjoinaddress.Split(CChar(":"))
                Startinfos.Server.ServerAdress = address(0)
                Startinfos.Server.ServerPort = address(1)
            End If
            Startinfos.Server.JustStarted = True
        End If
    End Sub

    Public Async Sub StartMC()
        If Startinfos.IsStarting = True Then
            Await Me.ShowMessageAsync("Achtung", "Minecraft wird bereits gestartet!", MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = "Ok", .ColorScheme = MetroDialogColorScheme.Accented})
        ElseIf cb_profiles.SelectedIndex = -1 Then
            Await Me.ShowMessageAsync(Nothing, "Wähle bitte ein Profil aus!", MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = "Ok", .ColorScheme = MetroDialogColorScheme.Accented})
            'ElseIf tb_username.Text = Nothing Then
            '    Await Me.ShowMessageAsync(Nothing, "Gib bitte einen Usernamen ein!", MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = "Ok", .ColorScheme = MetroDialogColorScheme.Accented})
        Else
            If Startinfos.Profile Is Nothing Then
                Startinfos.Profile = Await Profiles.FromName(ViewModel.selectedprofile)
            End If
            Await Versions_Load()
            tabitem_console.IsSelected = True
            mc = New Process
            If Await Check_Account() Then
                If Startinfos.Server.JustStarted = False Then
                    If cb_direct_join.IsChecked = True Then
                        If ViewModel.Directjoinaddress <> Nothing Then
                            If Startinfos.Server.JustStarted = False Then
                                If ViewModel.Directjoinaddress.Contains(":") = False Then
                                    Startinfos.Server.ServerAdress = ViewModel.Directjoinaddress
                                Else
                                    Dim address As String() = ViewModel.Directjoinaddress.Split(CChar(":"))
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
                        If Versions.versions.Select(Function(p) p.id).Contains(Startinfos.Profile.lastVersionId) Then
                            Startinfos.Version = Versions.versions.Where(Function(p) p.id = Startinfos.Profile.lastVersionId).First
                        Else
                            Startinfos.Profile = Nothing
                            Write(Startinfos.Profile.lastVersionId & ".jar und/oder " & Startinfos.Profile.lastVersionId & ".json existiert nicht" & Environment.NewLine & "---Wähle eine andere Version aus oder, falls du gerade Forge gestartet hast, installiere es erneut!", LogLevel.ERROR)
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
                Download_Version()
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
                Dispatcher.Invoke(New WriteColored(AddressOf Append), "[WARNING] " & Line, tb_ausgabe, Brushes.Orange)
            Case MainWindow.LogLevel.ERROR
                Dispatcher.Invoke(New WriteColored(AddressOf Append), "[ERROR] " & Line, tb_ausgabe, Brushes.Red)
        End Select
        '-Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true

        'Error:

        '2014-04-08 16:06:25 [Schwerwiegend] [ForgeModLoader] Technical information: The class net.minecraft.client.ClientBrandRetriever should have been associated with the minecraft jar file,
        'and should have returned us a valid, intact minecraft jar location. This did not work. Either you have modified the minecraft jar file (if so run the forge installer again),
        'or you are using a base editing jar that is changing this class (and likely others too). If you REALLY want to run minecraft in this configuration,
        'add the flag -Dfml.ignoreInvalidMinecraftCertificates=true to the 'JVM settings' in your launcher profile.
        If Line.Contains("add the flag -Dfml.ignoreInvalidMinecraftCertificates=true to the 'JVM settings' in your launcher profile") Then
            Write("Ein Fehler ist aufgetreten. Bitte versuche, im ProfileEditor bei den ""JVM Argumenten"" folgendes hinzuzufügen: ""-Dfml.ignoreInvalidMinecraftCertificates=true""", MainWindow.LogLevel.ERROR)
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
            Return Path.Combine(GetJavaPath(), "bin", "java.exe")
        End If
    End Function
    Public Shared Async Function GetJavaVersionInformation() As Task(Of String)
        Try
            Dim profile As Profiles.Profile = Nothing
            If Startinfos.IsStarting = True Then
                profile = Startinfos.Profile
            Else
                profile = Await Profiles.FromName(ViewModel.selectedprofile)
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

    Private Sub wcversionsdownload_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles wcversionsdownload.DownloadProgressChanged
        pb_download_Value(e.ProgressPercentage)
    End Sub

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
        If ViewModel.Profiles.Count > 1 Then
            Profiles.Remove(ViewModel.selectedprofile)
            ViewModel.selectedprofile = ViewModel.Profiles.First
            Profiles.Get_Profiles()
        Else
            Await Me.ShowMessageAsync("Fehler", "Das letzte Profil kann nicht gelöscht werden!", MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = "Ok", .ColorScheme = MetroDialogColorScheme.Accented})
        End If
    End Sub

#Region "Mods"
    Public Async Function Load_ModVersions() As Task
        cb_modversions.Items.Clear()
        Dim modversionslist As IList(Of String) = Await Modifications.List_all_Mod_Vesions
        For Each Modversion As String In modversionslist
            cb_modversions.Items.Add(Modversion)
        Next
        cb_modversions.SelectedIndex = 0
    End Function
    Private Sub Filter_Mods()
        Dim selectedmod As String = Nothing
        If lb_mods.SelectedItem IsNot Nothing Then selectedmod = DirectCast(lb_mods.SelectedItem, Modifications.Mod).name
        lb_mods.Items.Clear()
        Modifications.Check_installed(modsfolderPath)
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
            If lb_mods.SelectedItems.Count > 1 Then
                btn_downloadmod.Content = lb_mods.SelectedItems.Count & " Installieren"
            Else
                btn_downloadmod.Content = "Installieren"
            End If
            lbl_name.Content = selected.name
            lbl_autor.Content = selected.autor
            cb_mods_description_language.Items.Clear()
            For Each Language As String In selected.descriptions.Select(Function(p) p.id)
                cb_mods_description_language.Items.Add(Language)
            Next
            If selected.descriptions.Select(Function(p) p.id).Contains("de") Then
                cb_mods_description_language.SelectedItem = "de"
            ElseIf selected.descriptions.Select(Function(p) p.id).Contains("en") Then
                cb_mods_description_language.SelectedItem = "en"
            Else
                cb_mods_description_language.SelectedItem = selected.descriptions.First.id
            End If
            Select Case selected.type
                Case "forge"
                    lbl_type.Content = "Vorraussetzung: Minecraft Forge (Tools->Forge)"
                Case "liteloader"
                    lbl_type.Content = "Vorraussetzung: LiteLoader (Tools->LiteLoader)"
                Case Else
                    lbl_type.Content = "Type: " & DirectCast(lb_mods.SelectedItem, Modifications.Mod).type
            End Select
            Dim oldsource As String = Nothing
            If wc_mod_video.Source <> Nothing Then
                oldsource = wc_mod_video.Source.ToString
            End If
            Dim youtubeMatch As Match = YoutubeVideoRegex.Match(DirectCast(lb_mods.SelectedItem, Modifications.Mod).video)
            Dim id As String = String.Empty
            If youtubeMatch.Success Then
                id = youtubeMatch.Groups(1).Value
                If oldsource <> id Then
                    wc_mod_video.Source = New Uri("http://www.youtube.com/embed/" & id)
                End If
            End If
        End If
    End Sub
    Private Sub cb_mods_description_language_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cb_mods_description_language.SelectionChanged
        If lb_mods.SelectedIndex <> -1 Then
            If cb_mods_description_language.SelectedIndex <> -1 Then
                Dim selected As Modifications.Mod = DirectCast(lb_mods.SelectedItem, Modifications.Mod)
                tb_description.Text = selected.descriptions.Where(Function(p) p.id = cb_mods_description_language.SelectedItem.ToString).First.text
            End If
        End If
    End Sub
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
            Await Load_ModVersions()
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
        fd.Description = "Mods Ordner auswählen"
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
        If tb_modsfolder.Text <> Nothing Then
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
        Dim Version As String = DirectCast(lb_mods.SelectedItem, Modifications.Mod).versions.Where(Function(p) p.version = cb_modversions.SelectedItem.ToString).First.version
        Await Delete_Mod(Version)
    End Sub
    Public Async Function Delete_Mod(Version As String) As Task
        Dim capturedException As ExceptionDispatchInfo = Nothing
        For Each selectedmod As Modifications.Mod In lb_mods.SelectedItems
            Try
                Dim Struktur As String = Version & "\" & Version & "-" & selectedmod.id & "." & selectedmod.extension
                If File.Exists(tb_modsfolder.Text & "\" & Struktur) = True Then
                    File.Delete(tb_modsfolder.Text & "\" & Struktur)
                End If
            Catch ex As Exception
                capturedException = ExceptionDispatchInfo.Capture(ex)
            End Try
            If capturedException IsNot Nothing Then
                Await Me.ShowMessageAsync("Fehler", "Mod konnte nicht gelöscht werden: " & selectedmod.name & Environment.NewLine & "Falls der Mod in Minecraft geöffnet ist, schließe Minecraft zuerst!")
            End If
        Next
        Dim selected As Integer = lb_mods.SelectedIndex
        Filter_Mods()
        lb_mods.SelectedIndex = selected

    End Function
    Private Async Sub btn_downloadmod_Click(sender As Object, e As RoutedEventArgs) Handles btn_downloadmod.Click
        If moddownloading = True Then
            Await Me.ShowMessageAsync("Download läuft", "Eine Mod wird bereits heruntergeladen. Warte bitte, bis diese fertig ist!", MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = "Ok", .ColorScheme = MetroDialogColorScheme.Accented})
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

            controller = Await Me.ShowProgressAsync("Mods werden installiert", "Bitte warten")
            'controller.SetCancelable(True)

            download_mod()
        End If
    End Sub
    Private Async Sub download_mod()
        If modsdownloadindex < modsdownloadlist.Count Then
            If modsfolderPath.Contains(IO.Path.GetInvalidPathChars) = True Then
                Await Me.ShowMessageAsync("Fehler", "Der Pfad des Mods Ordners enthält ungültige Zeichen", MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = "Ok", .ColorScheme = MetroDialogColorScheme.Accented})
                Exit Sub
            End If
            Dim url As New Uri(modsdownloadlist.Item(modsdownloadindex).versions.Where(Function(p) p.version = modsdownloadingversion).First.downloadlink)
            Dim progress As Double = modsdownloadindex / modsdownloadlist.Count
            controller.SetProgress(progress)
            controller.SetMessage(modsdownloadindex + 1 & " / " & modsdownloadlist.Count & " " & modsdownloadlist.Item(modsdownloadindex).name)
            Modsfilename = modsdownloadingversion & "\" & modsdownloadingversion & "-" & modsdownloadlist.Item(modsdownloadindex).id & "." & modsdownloadlist.Item(modsdownloadindex).extension
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
            controller.SetMessage("Abgebrochen")
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
            controller.SetMessage("Mods erfolgreich Installiert")
            Await Task.Delay(1000)
            Await controller.CloseAsync()
        Else
            controller.SetMessage("Fehler beim Installieren der Mods: " & Errormessage)
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
        lb_servers.Items.Clear()
        servers_dat.Refresh()
        If servers_dat.Exists = True Then
            lbl_no_servers.Visibility = Windows.Visibility.Collapsed
            ViewModel.Servers = New ServerList()
            Await ViewModel.Servers.Load()
            If ViewModel.Servers.Servers.Count = 0 Then
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
            For Each item As ServerList.Server In ViewModel.Servers.Servers
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
            Parallel.For(0, ViewModel.Servers.Servers.Count, Sub(b)
                                                                 CheckOnline(b)
                                                             End Sub)
        Catch
        End Try
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
            ViewModel.Servers.Servers.Item(i).DoPing()
            'MsgBox(servers.Item(i).ServerStatus.Players.MaxPlayers)
            Dispatcher.Invoke(New Action(Sub()
                                             Dim selected As Integer = lb_servers.SelectedIndex
                                             lb_servers.Items.RemoveAt(i)
                                             lb_servers.Items.Insert(i, ViewModel.Servers.Servers.Item(i))
                                             lb_servers.SelectedIndex = selected
                                         End Sub))
            ViewModel.Servers.Save()
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
            Dim result As MessageDialogResult = Await Me.ShowMessageAsync("Server löschen", "Bist du dir sicher, dass du den Server " & Chr(34) & DirectCast(lb_servers.SelectedItem, ServerList.Server).name & Chr(34) & " entgültig löschen willst?", MessageDialogStyle.AffirmativeAndNegative, New MetroDialogSettings() With {.AffirmativeButtonText = "Ja", .NegativeButtonText = "Nein", .ColorScheme = MetroDialogColorScheme.Accented})
            If result = MessageDialogResult.Affirmative Then
                lb_servers.Items.RemoveAt(selected)
                ViewModel.Servers.Servers.RemoveAt(selected)
                ViewModel.Servers.Save()
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
            ViewModel.Directjoinaddress = DirectCast(lb_servers.SelectedItem, ServerList.Server).ip
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

                                                         If Version.Contains("Spigot") Then
                                                             Dim ver As String = Version.Replace("Spigot ", "")
                                                             Startinfos.Version = Versions.versions.Where(Function(p) p.id = ver).FirstOrDefault
                                                         ElseIf Version.Contains("CraftBukkit") Then
                                                             Dim ver As String = Version.Replace("CraftBukkit ", "")
                                                             Startinfos.Version = Versions.versions.Where(Function(p) p.id = ver).FirstOrDefault
                                                         ElseIf Version.Contains("-") = True Then
                                                             Startinfos.Version = Versions.versions.Where(Function(p) p.id = Version.Split(CChar("-"))(1)).FirstOrDefault
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
        Settings.Settings.DirectJoin = cb_direct_join.IsChecked.Value
        Settings.Save()
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
        toolscontroller = Await Me.ShowProgressAsync("Feed the Beast wird heruntergeladen", "Bitte warten")
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
        toolscontroller = Await Me.ShowProgressAsync("Technic Launcher wird heruntergeladen", "Bitte warten")
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
        Settings.Settings.WindowState = Me.WindowState
        Settings.Save()
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
            Dim profile As Profiles.Profile = Await Profiles.FromName(ViewModel.selectedprofile)
            If profile.playerUUID = Nothing Then
                LoginScreen.Open()
                TabControl_main.Visibility = Windows.Visibility.Collapsed
                Return False
            Else
                Dim capturedException As MinecraftAuthenticationException = Nothing
                'Login with access token
                Try
                    If authenticationDatabase.List.Select(Function(p) p.uuid.Replace("-", "")).Contains(profile.playerUUID) Then
                        Write("Anmelden mit access token")
                        Dim Account = authenticationDatabase.List.Where(Function(p) p.uuid.Replace("-", "") = profile.playerUUID).First
                        If Guid.TryParse(Account.userid, New Guid) Then
                            Startinfos.Session = New Session() With {.AccessToken = Account.accessToken,
                                                                  .ClientToken = authenticationDatabase.clientToken,
                                                                  .SelectedProfile = Nothing}
                            Await Startinfos.Session.Refresh()
                            authenticationDatabase.List.Where(Function(p) p.uuid.Replace("-", "") = profile.playerUUID).First.accessToken = Startinfos.Session.AccessToken
                            Await authenticationDatabase.Save()
                        Else
                            Startinfos.Session = New Session() With {.ClientToken = authenticationDatabase.clientToken,
                                                                  .SelectedProfile = New Profile() With {.Name = Account.displayName}}
                        End If
                        'This comes last:
                        Await ShowUsername_Avatar(Account)
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
                    LoginScreen.Open()
                    TabControl_main.Visibility = Windows.Visibility.Collapsed
                End If
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Async Function ShowUsername_Avatar(Account As authenticationDatabase.Account) As Task
        lbl_Username.Content = Account.displayName
        lbl_user_state.Content = If(Guid.TryParse(Account.userid, New Guid), "Premium", "Cracked")
        lbl_user_state.Foreground = If(Guid.TryParse(Account.userid, New Guid), Brushes.Green, Brushes.Red)
        Dim WebRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create("https://minotar.net/helm/" & Account.displayName & "/100"), HttpWebRequest)
        Using WebReponse As HttpWebResponse = DirectCast(Await WebRequest.GetResponseAsync, HttpWebResponse)
            Using stream As Stream = WebReponse.GetResponseStream
                img_avatar.Source = ImageConvert.GetImageStream(System.Drawing.Image.FromStream(stream))
            End Using
        End Using
    End Function

    Private Async Sub btn_logout_Click(sender As Object, e As RoutedEventArgs) Handles btn_logout.Click
        'logout / invalidate session
        Dim capturedException As MinecraftAuthenticationException = Nothing
        Try
            Dim profile As Profiles.Profile = Await Profiles.FromName(ViewModel.selectedprofile)
            profile.playerUUID = Nothing
            Await Profiles.Edit(ViewModel.selectedprofile, profile)
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
End Class