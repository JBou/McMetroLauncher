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
Imports Craft.Net.Client
Imports System.Threading.Tasks
Imports System.ComponentModel
Imports System.Windows.Media

#End Region
Public Module GlobalInfos
#Region "Functions/Subs"
    Public Function Check_Updates() As Boolean
        If onlineversion > AssemblyVersion Then
            Return True
        Else
            Return False
        End If
    End Function
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
    Public Function GetJavaPath() As String
        Dim environmentPath As String = Environment.GetEnvironmentVariable("JAVA_HOME")
        If environmentPath IsNot Nothing Then
            Return environmentPath
        End If
        Dim javaKey As String = "SOFTWARE\JavaSoft\Java Runtime Environment"
        Dim javakeyWow6432Node As String = "SOFTWARE\Wow6432Node\JavaSoft\Java Runtime Environment"
        Using baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(javaKey)
            If baseKey IsNot Nothing Then
                Dim currentVersion As String = baseKey.GetValue("CurrentVersion").ToString()
                Using homeKey = baseKey.OpenSubKey(currentVersion)
                    If homeKey IsNot Nothing Then
                        Return homeKey.GetValue("JavaHome").ToString()
                    End If
                End Using
            End If
        End Using
        Using baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(javakeyWow6432Node)
            If baseKey IsNot Nothing Then
                Dim currentVersion As String = baseKey.GetValue("CurrentVersion").ToString()
                Using homeKey = baseKey.OpenSubKey(currentVersion)
                    If homeKey IsNot Nothing Then
                        Return homeKey.GetValue("JavaHome").ToString()
                    End If
                End Using
            End If
        End Using
        Return Nothing
    End Function
#End Region
    Public Class Startinfos
        Public Class Server
            Private Shared m_serveradress As String
            Private Shared m_serverport As String
            Private Shared m_juststarted As Boolean
            Public Shared Property JustStarted As Boolean
                Get
                    Return m_juststarted
                End Get
                Set(value As Boolean)
                    m_juststarted = value
                End Set
            End Property
            Public Shared Property ServerAdress As String
                Get
                    Return m_serveradress
                End Get
                Set(value As String)
                    m_serveradress = value
                End Set
            End Property
            Public Shared Property ServerPort As String
                Get
                    Return m_serverport
                End Get
                Set(value As String)
                    m_serverport = value
                End Set
            End Property
        End Class
        Public Shared Property Profile As Profiles.Profile
            Get
                Return m_profile
            End Get
            Set(value As Profiles.Profile)
                m_profile = value
            End Set
        End Property

        Private Shared m_profile As Profiles.Profile
        Public Shared Property Version As Versionslist.Version
            Get
                Return m_version
            End Get
            Set(value As Versionslist.Version)
                m_version = value
            End Set
        End Property
        Private Shared m_version As Versionslist.Version
        Public Shared Property Versionsinfo As VersionsInfo
            Get
                Return m_versionsinfo
            End Get
            Set(value As VersionsInfo)
                m_versionsinfo = value
            End Set
        End Property
        Private Shared m_versionsinfo As VersionsInfo
        Public Shared Property IsStarting As Boolean
            Get
                Return m_isstarting
            End Get
            Set(value As Boolean)
                m_isstarting = value
            End Set
        End Property
        Private Shared m_isstarting As Boolean

    End Class

    Public AccentColors As List(Of AccentColorMenuData)
    Public ReadOnly Property AssemblyVersion As String
        Get
            Return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
        End Get
    End Property
    Public SelectedModVersion As String
    Public Versions As Versionslist
    Public LastLogin As LastLogin
    Public Session As Session
    Public Website As String = "http://patzleiner.net/"
    Public Appdata As New DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
    Public mcpfad As String = Appdata.FullName & "\.minecraft"
    Public modsfile As String = mcpfad & "\cache\modlist.json"
    Public librariesfolder As String = mcpfad & "\libraries"
    Public assetspath As String = mcpfad & "\assets"
    Public launcher_profiles_json As String = mcpfad & "\launcher_profiles.json"
    Public servers_dat As String = mcpfad & "\servers.dat"
    Public Newprofile As Boolean
    Public outputjsonversions As String = mcpfad & "\cache\versions.json"
    Public ReadOnly Property versionsJSON(Version As Versionslist.Version) As String
        Get
            Return mcpfad & "\versions\" & Versions.versions.Where(Function(p) p.id = Version.id).First.id & "\" & Versions.versions.Where(Function(p) p.id = Version.id).First.id & ".json"
        End Get
    End Property
    Public Versionsjar As String
    Public UnpackDirectory As String
    Public Arguments As String
    Public Delegate Sub WriteA(ByVal Text As String)
    Public Delegate Sub WriteColored(ByVal Text As String, Color As Brush)
    Public cachefolder As String = mcpfad & "\cache"
    Public modsfolder As String = mcpfad & "\mods"
    Public downloadfilepath As String
    Public servers As ServerList = New ServerList
    Public Applicationdata As New DirectoryInfo(Path.Combine(Appdata.FullName, "McMetroLauncher"))
    Public Applicationcache As New DirectoryInfo(IO.Path.Combine(Applicationdata.FullName, "cache"))
    Public onlineversion As String = Nothing
    Public changelog As String = Nothing
    Public resources_dir As New DirectoryInfo(Path.Combine(mcpfad, "resources"))
    Public librariesurl As String = "https://libraries.minecraft.net/"
    Public selectedprofile As String
    Public ReadOnly Property indexesurl(assets_index_name As String) As String
        Get
            Return "http://s3.amazonaws.com/Minecraft.Download/indexes/" & assets_index_name & ".json"
        End Get
    End Property
    Public ReadOnly Property cacheindexesfile(assets_index_name As String) As FileInfo
        Get
            Return New FileInfo(Path.Combine(cachefolder, "indexes/" & assets_index_name & ".json"))
        End Get
    End Property
    Public ReadOnly Property indexesfile(assets_index_name As String) As FileInfo
        Get
            Return New FileInfo(Path.Combine(assetspath, "indexes/" & assets_index_name & ".json"))
        End Get
    End Property
    Public ReadOnly Property resourcefile(hash As String) As FileInfo
        Get
            Return New FileInfo(Path.Combine(assetspath, "objects/" & hash.Substring(0, 2) & "/" & hash))
        End Get
    End Property
    Public ReadOnly Property resourceurl(hash As String) As String
        Get
            Return "http://resources.download.minecraft.net/" & hash.Substring(0, 2) & "/" & hash
        End Get
    End Property

    Public Versionsurl As String = "http://s3.amazonaws.com/Minecraft.Download/versions/versions.json"
    Public modfileurl As String = Website & "download/modlist.json"
    Public versionurl As String = Website & "mcmetrolauncher/version.txt"
    Public changelogurl As String = Website & "mcmetrolauncher/changelog.txt"

End Module

Public Class AccentColorMenuData
    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(value As String)
            m_Name = value
        End Set
    End Property
    Private m_Name As String
    Public Property ColorBrush() As SolidColorBrush
        Get
            Return m_ColorBrush
        End Get
        Set(value As SolidColorBrush)
            m_ColorBrush = value
        End Set
    End Property
    Private m_ColorBrush As SolidColorBrush


    Private Sub ChangeAccent(sender As Object)
        Dim theme = ThemeManager.DetectTheme(Application.Current)
        Dim accent = ThemeManager.DefaultAccents.First(Function(x) x.Name = Me.Name)
        ThemeManager.ChangeTheme(Application.Current, accent, theme.Item1)
    End Sub

End Class

Public Class MainWindow
#Region "Variables"
    '****************Webclients*****************
    WithEvents wcresources As New System.Net.WebClient ' Für das WebClient steuerelement mit Events z.b. DownloadProgressChanged... 
    WithEvents wcversionsdownload As New System.Net.WebClient
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
    '******************Assets*******************
    Private assets_index_name As String
    '******************Others*******************
#End Region

    Sub ThemeLight()
        Dim theme = ThemeManager.DetectTheme(Application.Current)
        ThemeManager.ChangeTheme(Application.Current, theme.Item2, MahApps.Metro.Theme.Light)
        Settings.Theme = "Light"
        Settings.Save()
        btn_refresh_image.Source = ImageConvert.GetImageStream(My.Resources.appbar_refresh)
        btn_list_delete_mod_image.Source = ImageConvert.GetImageStream(My.Resources.appbar_delete)
        DirectCast(Me.Resources("serverlistcontext_copy_image"), Windows.Controls.Image).Source = ImageConvert.GetImageStream(My.Resources.appbar_page_copy)
        DirectCast(Me.Resources("serverlistcontext_direct_join_image"), Windows.Controls.Image).Source = ImageConvert.GetImageStream(My.Resources.appbar_control_play)
        DirectCast(Me.Resources("serverlistcontext_direct_join_image2"), Windows.Controls.Image).Source = ImageConvert.GetImageStream(My.Resources.appbar_control_play)
    End Sub

    Sub ThemeDark()
        Dim theme = ThemeManager.DetectTheme(Application.Current)
        ThemeManager.ChangeTheme(Application.Current, theme.Item2, MahApps.Metro.Theme.Dark)
        Settings.Theme = "Dark"
        Settings.Save()
        btn_refresh_image.Source = ImageConvert.GetImageStream(My.Resources.appbar_refresh_dark)
        btn_list_delete_mod_image.Source = ImageConvert.GetImageStream(My.Resources.appbar_delete_dark)
        DirectCast(Me.Resources("serverlistcontext_copy_image"), Windows.Controls.Image).Source = ImageConvert.GetImageStream(My.Resources.appbar_page_copy_dark)
        DirectCast(Me.Resources("serverlistcontext_direct_join_image"), Windows.Controls.Image).Source = ImageConvert.GetImageStream(My.Resources.appbar_control_play_dark)
    End Sub

    Private Sub OnMenuItemClicked(sender As Object, e As RoutedEventArgs)
        Dim item As MenuItem = TryCast(e.OriginalSource, MenuItem)
        ' Handle the menu item click here
        If item IsNot Nothing Then
            ChangeAccent(item.Header.ToString)
        End If
    End Sub

    Sub ChangeAccent(accentname As String)
        Dim theme = ThemeManager.DetectTheme(Application.Current)
        If ThemeManager.DefaultAccents.Select(Function(x) x.Name).Contains(accentname) Then
            Dim accent = ThemeManager.DefaultAccents.First(Function(x) x.Name = accentname)
            ThemeManager.ChangeTheme(Application.Current, accent, theme.Item1)
        End If
        Settings.Accent = accentname
        Settings.Save()
    End Sub

    Private Sub ShowSettings(sender As Object, e As RoutedEventArgs)
        'Contrrols auf ihre Einstellungen setzen
        ToggleFlyout(0)
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

    Private Sub MainWindow_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        'My.Settings.Username = tb_username.Text.ToString
        'My.Settings.Ram = cb_ram.SelectedItem.ToString
        'My.Settings.Save()
        Try
            For i = 0 To lb_startedversions.Items.Count - 1
                If IO.Directory.Exists(lb_startedversions.Items.Item(i).ToString) = True Then
                    IO.Directory.Delete(lb_startedversions.Items.Item(i).ToString, True)
                End If
            Next

            wcresources.CancelAsync()
            wcversionsdownload.CancelAsync()
            wcindexes.CancelAsync()
            wcversionsstring.CancelAsync()
            wc_libraries.CancelAsync()

            If IO.Directory.Exists(mcpfad & "\cache") = True Then
                IO.Directory.Delete(mcpfad & "\cache", True)
            End If

        Catch ex As Exception
        End Try
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

    Public Sub ChangeAccent()
        Dim theme = ThemeManager.DetectTheme(Application.Current)
        Dim accent = ThemeManager.DefaultAccents.First(Function(x) x.Name = Me.Name)
        ThemeManager.ChangeTheme(Application.Current, accent, theme.Item1)
    End Sub

    Sub Get_Profiles()
        Profiles.Load()
        Dim jo As JObject = Profiles.profilesjo
        Dim i As Integer = 0
        Profiles.Load()
        Dim selectedprofile As String = jo("selectedProfile").ToString
        cb_profiles.Items.Clear()
        cb_mods_profilename.Items.Clear()
        For Each Profile As String In Profiles.List
            cb_profiles.Items.Add(Profile)
            cb_mods_profilename.Items.Add(Profile)
        Next
        If jo.Properties.Select(Function(p) p.Name).Contains("selectedProfile") = True Then
            cb_profiles.SelectedItem = selectedprofile
            cb_mods_profilename.SelectedItem = selectedprofile
        Else
            jo.Add(New JProperty("selectedProfile"))
            cb_profiles.SelectedIndex = 0
            cb_mods_profilename.SelectedIndex = 0
        End If

        If Profiles.List.Count = 0 Then
            'StandartProfile schreiben
            Dim standartprofile As New JObject(
            New JProperty("profiles",
                New JObject(
                    New JProperty("Default",
                        New JObject(
                            New JProperty("name", "Default"))))),
            New JProperty("selectedProfile", "Default"))
            IO.File.WriteAllText(launcher_profiles_json, standartprofile.ToString)

            Get_Profiles()

        End If

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
        pb_download.IsIndeterminate = True
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
        Write("Lade Resourcen herunter")
        Dim indexjo As JObject = JObject.Parse(File.ReadAllText(indexesfile(assets_index_name).FullName))
        Dim indexesobjects As IList(Of resourcesindexobject) = New List(Of resourcesindexobject)
        Dim virtual As Boolean
        If indexjo.Properties.Select(Function(p) p.Name).Contains("virtual") = False Then
            virtual = False
        Else
            virtual = Convert.ToBoolean(indexjo("virtual"))
        End If
        Dim Parsethreadfinished As Boolean = False
        Dim fThread = New Thread(New ThreadStart(Sub()
                                                     For i = 0 To indexjo("objects").Values.Count - 1
                                                         Dim keys As List(Of JProperty) = indexjo.Value(Of JObject)("objects").Properties.ToList
                                                         Dim key As String = keys.Item(i).Name
                                                         'indexjo.Values(Of JProperty)("objects").ElementAt(i).ToString()
                                                         Dim hash As String = keys.Item(i).Value.Value(Of String)("hash")
                                                         Dim size As Integer = CInt(keys.Item(i).Value.Value(Of String)("size"))
                                                         Dim item As New resourcesindexobject(key, hash, size)
                                                         indexesobjects.Add(item)
                                                     Next
                                                     Parsethreadfinished = True
                                                 End Sub))
        fThread.Start()
        While Parsethreadfinished = False
            Await Task.Delay(10)
        End While
        resourcesindexes = New resourcesindex(virtual, indexesobjects)
        pb_download.Maximum = resourcesindexes.objects.Count
        resourcesdownloadindex = 0
        resourcesdownloadtry = 1
        resourcesdownloading = True
        DownloadResources()
    End Function

    Sub DownloadResources()
        pb_download.Value = resourcesdownloadindex
        If resourcesdownloadindex < resourcesindexes.objects.Count Then
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
                Write("---Der Download wurde aufgrund zu vieler Fehlversuche abgebrochen!")
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
                Dim destination As New FileInfo(Path.Combine(assetspath, "virtual", assets_index_name, item.key.Replace("/", "\")))
                If destination.Directory.Exists = False Then
                    destination.Directory.Create()
                End If
                Try
                    If destination.Exists = True Then
                        destination.Delete()
                    End If
                    resourcefile(item.hash).CopyTo(destination.FullName)
                Catch
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
        lbl_downloadstatus.Content = String.Format("{0}% - {1} {2} von {3} {4} heruntergeladen", e.ProgressPercentage, Math.Round(bytes, 2), Einheit, Math.Round(totalbytes, 2), Einheit)
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

    Async Sub Download_Version()
        Dim versionid As String = Startinfos.Version.id
        Dim VersionsURl As String = "https://s3.amazonaws.com/Minecraft.Download/versions/" & versionid & "/" & versionid & ".jar"
        Dim VersionsJSONURL As String = "https://s3.amazonaws.com/Minecraft.Download/versions/" & versionid & "/" & versionid & ".json"
        Dim Outputfile As String = mcpfad & "\versions\" & versionid & "\" & versionid & ".jar"
        Dim CacheOutputfile As String = mcpfad & "\cache\versions\" & versionid & "\" & versionid & ".jar"
        Dim OutputfileJSON As String = mcpfad & "\versions\" & versionid & "\" & versionid & ".json"
        Dim CacheOutputfileJSON As String = mcpfad & "\cache\versions\" & versionid & "\" & versionid & ".json"
        Dim CacheDirectoryname As String = IO.Path.GetDirectoryName(CacheOutputfile)
        Dim Directoryname As String = IO.Path.GetDirectoryName(Outputfile)
        If IO.File.Exists(Outputfile) = False Then

            If IO.Directory.Exists(CacheDirectoryname) = False Then
                IO.Directory.CreateDirectory(CacheDirectoryname)
            End If

            Write("Lade Minecraft Version " & versionid & " herunter")
            Try
                Await wcversionsdownload.DownloadFileTaskAsync(New Uri(VersionsURl), CacheOutputfile)
            Catch ex As Exception
                Write("Fehler beim herunterladen von Minecraft " & versionid & " :" & vbNewLine & ex.Message & vbNewLine)
            End Try
            If IO.Directory.Exists(Directoryname) = False Then
                IO.Directory.CreateDirectory(Directoryname)
            End If

            IO.File.Move(CacheOutputfile, Outputfile)

        End If

        If IO.File.Exists(OutputfileJSON) = False Then


            If IO.Directory.Exists(CacheDirectoryname) = False Then
                IO.Directory.CreateDirectory(CacheDirectoryname)
            End If
            pb_download.Maximum = 100
            Await wcversionsdownload.DownloadFileTaskAsync(New Uri(VersionsJSONURL), CacheOutputfileJSON)

            If IO.Directory.Exists(Directoryname) = False Then
                IO.Directory.CreateDirectory(Directoryname)
            End If

            IO.File.Move(CacheOutputfileJSON, OutputfileJSON)
        End If
        If Startinfos.Versionsinfo Is Nothing Then
            Await Parse_VersionsInfo(Startinfos.Version)
        End If
        If Startinfos.IsStarting = True Then
            Download_Resources()
        End If
    End Sub

    Function Login(username As String, password As String) As Session
        Dim session As Session = Client.Session.DoLogin(username, password)
        Return session
    End Function

    Async Sub Download_Libraries()
        If Startinfos.Versionsinfo Is Nothing Then
            Await Parse_VersionsInfo(Startinfos.Version)
        End If
        'http://wiki.vg/Game_Files
        pb_download.Maximum = Startinfos.Versionsinfo.libraries.Count
        librariesdownloadindex = 0
        librariesdownloadtry = 1
        librariesdownloading = True
        librariesdownloadfailures = 0
        DownloadLibraries()
    End Sub

    Async Function Parse_VersionsInfo(Version As Versionslist.Version) As Task
        Dim o As String = File.ReadAllText(versionsJSON(Version))
        o = o.Replace("${arch}", GetJavaArch.ToString)
        Startinfos.Versionsinfo = Await JsonConvert.DeserializeObjectAsync(Of VersionsInfo)(o)
        Startinfos.Versionsinfo.JObject = JObject.Parse(o)
    End Function

    Async Sub DownloadLibraries()
        Try
            pb_download.Value = librariesdownloadindex
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
                    If Startinfos.Versionsinfo.JObject("libraries").Item(librariesdownloadindex).Value(Of JObject).Properties.Select(Function(p) p.Name).Contains("url") = False Then
                        url = librariesurl & Currentlibrary.path
                    Else
                        url = Startinfos.Versionsinfo.JObject("libraries").Item(librariesdownloadindex).Value(Of String)("url") & Currentlibrary.path
                    End If
                    Dim librarypath As New FileInfo(IO.Path.Combine(librariesfolder, Currentlibrary.path.Replace("/", "\")))

                    'libraryurl & ".sha1" enthält hash
                    Dim a As New WebClient()
                    'Hash herunterladen
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
                        If SHA1FileHash(librarypath.FullName).ToLower = Currentlibrarysha1 Then
                            todownload = False
                        Else
                            todownload = True
                        End If
                    Else
                        '*********************************************************Wenn library nicht existiert und library url ist files.minecraftforge.net, dann Meldung zum erneuten installireren von Forge zeigen.
                        todownload = True
                    End If
                    If todownload = True Then
                        'Download
                        If librarypath.Directory.Exists = False Then
                            librarypath.Directory.Create()
                        End If
                        If Currentlibrarysha1.Length > 0 Then
                            wc_libraries.DownloadFileAsync(New Uri(url), librarypath.FullName)
                            Write("Library wird heruntergeladen (Versuch " & librariesdownloadtry & "): " & librarypath.FullName)
                        Else
                            If librarypath.Exists Then
                                Write("Library konnte nicht auf Hash überprüft werden und wird übersprungen, in der Annahme, dass die lokale Datei gut ist: " & librarypath.FullName)
                            Else
                                Write("---Library konnte nicht heruntergeladen werden: " & librarypath.FullName & Environment.NewLine & "---Falls du gerade Forge gestartet hast, installiere es erneut!")
                            End If
                            librariesdownloadindex += 1
                            DownloadLibraries()
                        End If
                    Else
                        Write("Library existiert bereits: " & librarypath.FullName)
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
            Write("Fehler:" & Ex.Message)
        End Try
    End Sub

    Private Sub wc_libraries_DownloadFileCompleted(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs) Handles wc_libraries.DownloadFileCompleted
        If e.Error IsNot Nothing Then
            Write("---Library konnte nicht heruntergeladen werden: " & e.Error.Message & Environment.NewLine & "---Falls du gerade Forge gestartet hast, installiere es erneut!")
            librariesdownloadindex += 1
            librariesdownloadtry = 1
            librariesdownloadfailures += 1
            DownloadLibraries()
        End If
        If e.Cancelled = False And e.Error Is Nothing Then
            If librariesdownloadtry > 3 Then
                librariesdownloadfailures += 1
                Write("---Der Download wurde aufgrund zu vieler Fehlversuche abgebrochen!---" & Environment.NewLine & "---Falls du gerade Forge gestartet hast, installiere es erneut!")
                Startinfos.IsStarting = False
                librariesdownloading = False
                Exit Sub
            Else
                'Hash überprüfen
                Dim path As String = IO.Path.Combine(librariesfolder, Currentlibrary.path)
                If SHA1FileHash(path).ToLower = Currentlibrarysha1 Then
                    'Nächste Library Downloaden
                    librariesdownloadindex += 1
                    librariesdownloadtry = 1
                    Write("Library erfolgreich heruntergeladen und Hash verglichen")
                Else
                    'Library erneut heruntergeladen, Versuch erhöhen:
                    librariesdownloadtry += 1
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

    Sub Unzip()
        Try
            Write("Natives werden entpackt")
            UnpackDirectory = Path.Combine(mcpfad, "versions", Startinfos.Version.id, Startinfos.Version.id & "-natives-" & DateTime.Now.Ticks.ToString)
            If lb_startedversions.Items.Contains(UnpackDirectory) = False Then
                lb_startedversions.Items.Add(UnpackDirectory)
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
                            Dim librarypath As New FileInfo(IO.Path.Combine(librariesfolder, .path.Replace("/", "\")))
                            If IO.Directory.Exists(librarypath.DirectoryName) = False Then
                                IO.Directory.CreateDirectory(librarypath.DirectoryName)
                            End If
                            Try
                                Using zip1 As ZipFile = ZipFile.Read(librarypath.FullName)
                                    Dim e As ZipEntry
                                    ' here, we extract every entry, but we could extract conditionally,
                                    ' based on entry name, size, date, checkbox status, etc.   
                                    For Each e In zip1
                                        Dim ls As IList(Of String) = .extract.exclude
                                        For Each file As String In ls
                                            If e.FileName.StartsWith(file) = False Then
                                                e.Extract(UnpackDirectory, ExtractExistingFileAction.OverwriteSilently)
                                            End If
                                        Next
                                    Next
                                End Using
                            Catch ex As ZipException
                                Write("Fehler beim entpacken der natives: " & ex.Message)
                            End Try
                        End If
                    End If
                End With
            Next
        Catch ex As Exception
            Write("Fehler beim entpacken der natives. Wahrscheinlich wurde die erforderliche Library nicht heruntergeladen")
        End Try
    End Sub

    Async Sub Get_Startinfos()
        Write("Startinfos werden ausgelesen")
        Unzip()

        Dim mainClass As String = Startinfos.Versionsinfo.mainClass
        Dim minecraftArguments As List(Of String) = Startinfos.Versionsinfo.minecraftArguments.Split(Chr(32)).ToList
        Dim libraries As String = Nothing
        Dim gamedir As String = Nothing
        Dim argumentreplacements As List(Of String()) = New List(Of String())
        Dim natives As String = UnpackDirectory
        Dim javaargs As String = Nothing
        Dim height As String = Nothing
        Dim width As String = Nothing


        Dim Parsethreadfinished As Boolean = False
        Dim fThread = New Thread(New ThreadStart(Async Sub()
                                                     Versionsjar = mcpfad & "\versions\" & Startinfos.Version.id & "\" & Startinfos.Version.id & ".jar"
                                                     'Split by Space --> (Chr(32))
                                                     If Startinfos.Versionsinfo Is Nothing Then
                                                         Await Parse_VersionsInfo(Startinfos.Version)
                                                     End If
                                                     For i = 0 To Startinfos.Versionsinfo.libraries.Count - 1
                                                         Dim librarytemp As Library = Startinfos.Versionsinfo.libraries.Item(i)
                                                         If librarytemp.natives Is Nothing Then
                                                             libraries &= Path.Combine(librariesfolder, librarytemp.path.Replace("/", "\") & ";")
                                                         Else
                                                             If librarytemp.natives.windows IsNot Nothing Then
                                                                 libraries &= Path.Combine(librariesfolder, librarytemp.path.Replace("/", "\") & ";")
                                                             End If
                                                         End If
                                                     Next

                                                     If Startinfos.Profile.gameDir <> Nothing Then
                                                         gamedir = Startinfos.Profile.gameDir
                                                     Else
                                                         gamedir = mcpfad
                                                     End If

                                                     If IO.Directory.Exists(gamedir) = False Then
                                                         IO.Directory.CreateDirectory(gamedir)
                                                     End If
                                                     Dim assets_dir As String = assetspath
                                                     If resourcesindexes.virtual = True Then
                                                         assets_dir = Path.Combine(assetspath, "virtual", assets_index_name)
                                                     End If
                                                     argumentreplacements.Add(New String() {"${auth_player_name}", Settings.Username})
                                                     argumentreplacements.Add(New String() {"${version_name}", Startinfos.Version.id})
                                                     argumentreplacements.Add(New String() {"${game_directory}", gamedir})
                                                     argumentreplacements.Add(New String() {"${game_assets}", assets_dir})
                                                     argumentreplacements.Add(New String() {"${assets_root}", assets_dir})
                                                     argumentreplacements.Add(New String() {"${assets_index_name}", assets_index_name})
                                                     argumentreplacements.Add(New String() {"${user_properties}", New JObject().ToString})

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

                                                     If Startinfos.Profile.resolution.height <> Nothing Then
                                                         Height = " --height " & Startinfos.Profile.resolution.height
                                                     Else
                                                         Height = Nothing
                                                     End If

                                                     If Startinfos.Profile.resolution.width <> Nothing Then
                                                         Width = " --width " & Startinfos.Profile.resolution.width
                                                     Else
                                                         Width = Nothing
                                                     End If

                                                     Arguments = javaargs & " -Djava.library.path=" & Natives & " -cp " & libraries & Versionsjar & " " & mainClass & " " & String.Join(Chr(32), minecraftArguments) & Height & Width
                                                     Parsethreadfinished = True
                                                 End Sub))
        fThread.Start()
        While Parsethreadfinished = False
            Await Task.Delay(10)
            Write("True")
        End While
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
        Write("Starte Minecraft (Java " & GetJavaArch.ToString & " Bit): " & Teil_Arguments)
        mc = New Process()
        With mc.StartInfo
            .FileName = Startcmd(Startinfos.Profile)
            .Arguments = Arguments
            ' Arbeitsverzeichnis setzen falls nötig
            .WorkingDirectory = ""
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
            If tb_server_address.Text.Contains(":") = False Then
                Startinfos.Server.ServerAdress = tb_server_address.Text
            Else
                Dim address As String() = tb_server_address.Text.Split(CChar(":"))
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
        ElseIf tb_username.Text = Nothing Then
            Await Me.ShowMessageAsync(Nothing, "Gib bitte einen Usernamen ein!", MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = "Ok", .ColorScheme = MetroDialogColorScheme.Accented})
        Else
            If Startinfos.Profile Is Nothing Then
                Startinfos.Profile = Await Profiles.FromName(selectedprofile)
            End If
            'If cb_online_mode.IsChecked = True Then
            '    If pb_Password.Password = Nothing Then
            '        MessageBox.Show("Gib ein Password ein!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
            '        Exit Sub
            'Else
            '    Try
            '        Client.LastLogin.SetLastLogin(New LastLogin() With {
            '                                      .Username = tb_username.Text,
            '                                      .Password = pb_Password.Password
            '                                         })
            '        Session = Login(tb_username.Text, pb_Password.Password)
            '    Catch ex As Client.Session.MinecraftAuthenticationException
            '        'Auf Deutsch übersetzen
            '        MessageBox.Show(ex.ErrorMessage)
            '        Exit Sub
            '    End Try
            '    End If
            'End If
            If Startinfos.Server.JustStarted = False Then
                If cb_direct_join.IsChecked = True Then
                    If tb_server_address.Text <> Nothing Then
                        If Startinfos.Server.JustStarted = False Then
                            If tb_server_address.Text.Contains(":") = False Then
                                Startinfos.Server.ServerAdress = tb_server_address.Text
                            Else
                                Dim address As String() = tb_server_address.Text.Split(CChar(":"))
                                Startinfos.Server.ServerAdress = address(0)
                                Startinfos.Server.ServerPort = address(1)
                            End If
                            Startinfos.Server.JustStarted = True
                        End If
                    End If
                Else
                    Startinfos.Server.ServerAdress = Nothing
                    Startinfos.Server.ServerPort = Nothing
                End If
            End If


            If Startinfos.Version Is Nothing Then
                If Startinfos.Profile.lastVersionId <> Nothing Then
                    Startinfos.Version = Versions.versions.Where(Function(p) p.id = Startinfos.Profile.lastVersionId).First
                Else
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


    Public Sub Append(ByVal Line As String, Color As Brush)
        Me.Dispatcher.Invoke(New Action(Sub()
                                            Dim tr As New TextRange(tb_ausgabe.Document.ContentEnd, tb_ausgabe.Document.ContentEnd)
                                            tr.Text = Line & Environment.NewLine
                                            tr.ApplyPropertyValue(TextElement.ForegroundProperty, Color)
                                            tb_ausgabe.ScrollToEnd()
                                        End Sub))
    End Sub
    Public Sub Append(ByVal Line As String)
        Me.Dispatcher.Invoke(New Action(Sub()
                                            Dim tr As New TextRange(tb_ausgabe.Document.ContentEnd, tb_ausgabe.Document.ContentEnd)
                                            tr.Text = Line & Environment.NewLine
                                            tb_ausgabe.ScrollToEnd()
                                        End Sub))
    End Sub

    ''' <summary>
    ''' Schreibt eine Zeile und anschleißend eine Neue Zeile ind die tb_ausgabe
    ''' </summary>
    ''' <param name="Line">Die Zeile, die geschrieben werden soll</param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Line As String)
        Dispatcher.Invoke(New WriteA(AddressOf Append), Line)
    End Sub

    Public Sub WriteError(ByVal Line As String)
        Dispatcher.Invoke(New WriteColored(AddressOf Append), "[ERROR] " & Line, Brushes.Red)
    End Sub

    Public Sub Clear()
        Me.Dispatcher.Invoke(New Action(Sub()
                                            tb_ausgabe.SelectAll()
                                            tb_ausgabe.Selection.Text = Environment.NewLine
                                        End Sub))
    End Sub

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
                profile = Await Profiles.FromName(selectedprofile)
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

    Public Function RamCheck() As Integer
        'If GetJavaPath() = "64" Then
        Dim cbram_selecteditem As String = cb_ram.SelectedItem.ToString()
        Dim ram_i As Integer = Val(cbram_selecteditem.First)
        Return ram_i * 1024
        'Else
        'Return 1024
        'End If
    End Function

    Private Sub wcversionsdownload_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles wcversionsdownload.DownloadProgressChanged
        pb_download.Value = e.ProgressPercentage
    End Sub

    Private Sub tb_ausgabe_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tb_ausgabe.TextChanged
        tb_ausgabe.ScrollToEnd()
    End Sub

    Private Sub btn_new_profile_Click(sender As Object, e As RoutedEventArgs) Handles btn_new_profile.Click
        Dim result As Boolean?
        Newprofile = True
        Dim frm_ProfileEditor As New ProfileEditor
        Dim Profiles As New Profiles
        frm_ProfileEditor.ShowDialog()
        result = frm_ProfileEditor.DialogResult
        If result = True Then
            Get_Profiles()
        End If

    End Sub

    Private Sub btn_edit_profile_Click(sender As Object, e As RoutedEventArgs) Handles btn_edit_profile.Click
        Dim result As Boolean?
        Newprofile = False
        Dim frm_ProfileEditor As New ProfileEditor
        Dim Profiles As New Profiles
        frm_ProfileEditor.ShowDialog()
        result = frm_ProfileEditor.DialogResult
        If result = True Then
            Get_Profiles()
        End If
    End Sub

    Private Sub cb_profiles_DropDownClosed(sender As Object, e As EventArgs) Handles cb_profiles.DropDownClosed
        Get_Profiles()
    End Sub

    Private Sub cb_profiles_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cb_profiles.SelectionChanged
        Try
            selectedprofile = cb_profiles.SelectedItem.ToString
            Dim o As String = IO.File.ReadAllText(launcher_profiles_json)
            Dim jo As JObject = JObject.Parse(o)
            jo("selectedProfile") = selectedprofile
            IO.File.WriteAllText(launcher_profiles_json, jo.ToString)
            cb_mods_profilename.SelectedItem = cb_profiles.SelectedItem
        Catch
        End Try
    End Sub

    Private Async Sub btn_delete_profile_Click(sender As Object, e As RoutedEventArgs) Handles btn_delete_profile.Click
        If cb_profiles.Items.Count > 1 Then
            Profiles.Remove(cb_profiles.SelectedItem.ToString)
            cb_profiles.SelectedIndex = 0
            Get_Profiles()
        Else
            Await Me.ShowMessageAsync("Fehler", "Das letzte Profil kann nicht gelöscht werden!", MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = "Ok", .ColorScheme = MetroDialogColorScheme.Accented})
        End If
    End Sub

    Private Sub tb_username_KeyDown(sender As Object, e As Input.KeyEventArgs) Handles tb_username.KeyDown
        If e.Key = Key.Enter Or e.Key = Key.Return Then
            'Deine Aktionen
            StartMC()
        End If
    End Sub

    Private Sub tb_username_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tb_username.TextChanged
        Try
            Settings.Username = tb_username.Text
            Settings.Save()
        Catch
        End Try
        'My.Settings.Username = tb_username.Text.ToString
        'My.Settings.Save()
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
        lb_mods.Items.Clear()
        Modifications.Check_installed(modsfolderPath)
        Dim mods_with_selectedversion As IList(Of Modifications.Mod) = Modifications.ModList.Where(Function(p) p.versions.Select(Function(i) i.version).Contains(cb_modversions.SelectedItem.ToString)).ToList
        For Each Moditem As Modifications.Mod In mods_with_selectedversion
            If Moditem.name.ToLower.Contains(tb_search_mods.Text.ToLower) = True Then
                lb_mods.Items.Add(Moditem)
            End If
        Next
        If lb_mods.Items.Count >= 1 Then
            lb_mods.SelectedIndex = 0
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
        End If


        'If lb_mods.SelectedIndex <> -1 Then
        '    lbl_name.Content = Mods.NameAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
        '    tb_description.Text = Mods.descriptionAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
        '    mod_website = Mods.websiteAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
        '    mod_video = Mods.videoAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
        '    mod_downloadlink = Mods.downloadlinkAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
        'End If
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
        tb_modsfolder.Text = modsfolder
    End Sub
    Private Sub btn_selectmodsfolder_Click(sender As Object, e As RoutedEventArgs) Handles btn_selectmodsfolder.Click
        Try
            If Directory.Exists(modsfolder) = False Then
                Directory.CreateDirectory(modsfolder)
            End If
        Catch
        End Try
        Dim fd As New VistaFolderBrowserDialog
        fd.UseDescriptionForTitle = True
        fd.Description = "Mods Ordner auswählen"
        fd.RootFolder = Environment.SpecialFolder.MyComputer
        fd.SelectedPath = modsfolder
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
                profile.gameDir = mcpfad
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
                    profile.gameDir = mcpfad
                End If
                modsfolderPath = IO.Path.Combine(profile.gameDir, "mods")
                Filter_Mods()
            End If
        End If
    End Sub
    Private Sub btn_list_delete_mod_Click(sender As Object, e As RoutedEventArgs) Handles btn_list_delete_mod.Click
        Dim Version As String = DirectCast(lb_mods.SelectedItem, Modifications.Mod).versions.Where(Function(p) p.version = cb_modversions.SelectedItem.ToString).First.version
        Delete_Mod(Version)
    End Sub
    Public Sub Delete_Mod(Version As String)
        Dim Struktur As String
        If Version >= "1.6.4" = True Then
            Struktur = Version & "\" & Version & "-" & DirectCast(lb_mods.SelectedItem, Modifications.Mod).id & "." & DirectCast(lb_mods.SelectedItem, Modifications.Mod).extension
        Else
            Struktur = Version & "-" & DirectCast(lb_mods.SelectedItem, Modifications.Mod).id & "." & DirectCast(lb_mods.SelectedItem, Modifications.Mod).extension
        End If
        If File.Exists(tb_modsfolder.Text & "\" & Struktur) = True Then
            File.Delete(tb_modsfolder.Text & "\" & Struktur)
        End If
        Dim selected As Integer = lb_mods.SelectedIndex
        Filter_Mods()
        lb_mods.SelectedIndex = selected
    End Sub
    Private Async Sub btn_downloadmod_Click(sender As Object, e As RoutedEventArgs) Handles btn_downloadmod.Click
        If moddownloading = True Then
            Await Me.ShowMessageAsync("Download läuft", "Eine Mod wird bereits heruntergeladen. Warte bitte, bis diese fertig ist!", MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = "Ok", .ColorScheme = MetroDialogColorScheme.Accented})
        Else
            modsdownloadingversion = cb_modversions.SelectedItem.ToString
            btn_resetmodsfoler.IsEnabled = False
            btn_selectmodsfolder.IsEnabled = False
            btn_refresh.IsEnabled = False
            btn_downloadmod.IsEnabled = False
            moddownloading = True
            lbl_mods_status.Content = Nothing
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
            lbl_mods_status.Content = modsdownloadindex + 1 & " / " & modsdownloadlist.Count & " " & modsdownloadlist.Item(modsdownloadindex).name
            If modsdownloadingversion >= "1.6.4" = True Then
                Modsfilename = modsdownloadingversion & "\" & modsdownloadingversion & "-" & modsdownloadlist.Item(modsdownloadindex).id & "." & modsdownloadlist.Item(modsdownloadindex).extension
            Else
                Modsfilename = modsdownloadingversion & "-" & modsdownloadlist.Item(modsdownloadindex).id & "." & modsdownloadlist.Item(modsdownloadindex).extension
            End If
            Try
                If IO.Directory.Exists(IO.Path.GetDirectoryName(cachefolder & "\" & Modsfilename)) = False Then
                    IO.Directory.CreateDirectory((IO.Path.GetDirectoryName(cachefolder & "\" & Modsfilename)))
                End If
                wcmod.DownloadFileAsync(url, cachefolder & "\" & Modsfilename)
            Catch ex As Exception
                lbl_mods_status.Content = ex.Message
                Me.ShowMessageAsync("Fehler", ex.Message, MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = "Ok", .ColorScheme = MetroDialogColorScheme.Accented})
                Mod_Download_finished()
                Exit Sub
            End Try
            'End If
            modsdownloadindex += 1
            Dim selected As Integer = lb_mods.SelectedIndex
            Filter_Mods()
            lb_mods.SelectedIndex = selected
        Else
            lbl_mods_status.Content = "Erfolgreich installiert"
            Mod_Download_finished()
        End If
    End Sub
    Private Sub wcmod_DownloadFileCompleted(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs) Handles wcmod.DownloadFileCompleted
        If e.Cancelled = True Then
            lbl_mods_status.Content = "Abgebrochen"
            Mod_Download_finished()
        Else
            Try
                Dim path As String = modsfolderPath & "\" & Modsfilename
                If IO.Directory.Exists(IO.Path.GetDirectoryName(path)) = False Then
                    IO.Directory.CreateDirectory((IO.Path.GetDirectoryName(path)))
                End If
                My.Computer.FileSystem.MoveFile(cachefolder & "\" & Modsfilename, path)
            Catch
            End Try
            download_mod()
        End If
    End Sub
    Private Sub Mod_Download_finished()
        moddownloading = False
        btn_resetmodsfoler.IsEnabled = True
        btn_selectmodsfolder.IsEnabled = True
        btn_refresh.IsEnabled = True
        btn_downloadmod.IsEnabled = True
        Dim selected As Integer = lb_mods.SelectedIndex
        Filter_Mods()
        lb_mods.SelectedIndex = selected
    End Sub
    Private Sub wcmod_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles wcmod.DownloadProgressChanged
        ' Do all the ui thread updates here
        Me.Dispatcher.Invoke(
            DispatcherPriority.Normal,
            New Action(Sub()

                           ' Do all the ui thread updates here
                           pb_mods_download.Value = e.ProgressPercentage

                       End Sub))
    End Sub

#End Region

#Region "Server"

    Sub Load_Servers()
        lb_servers.Items.Clear()
        If File.Exists(servers_dat) = True Then
            lbl_no_servers.Visibility = Windows.Visibility.Collapsed
            servers = New ServerList()
            servers.Load()
            If servers.Servers.Count = 0 Then
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
            For Each item As ServerList.Server In servers.Servers
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
    End Sub

    Sub Ping_servers()
        Dim fThread = New Thread(New ThreadStart(AddressOf ThreadProc))
        fThread.IsBackground = True
        fThread.Start()
    End Sub

    Private Sub ThreadProc()
        Try
            Parallel.For(0, servers.Servers.Count, Sub(b)
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
            servers.Servers.Item(i).DoPing()
            'MsgBox(servers.Item(i).ServerStatus.Players.MaxPlayers)
            Dispatcher.Invoke(New Action(Sub()
                                             Dim selected As Integer = lb_servers.SelectedIndex
                                             lb_servers.Items.RemoveAt(i)
                                             lb_servers.Items.Insert(i, servers.Servers.Item(i))
                                             lb_servers.SelectedIndex = selected
                                         End Sub))
            servers.Save()
        Catch null As ArgumentNullException
            'hostNameOrAddress ist null.
        Catch socket As SocketException
            'Beim Auflösen von hostNameOrAddress ist ein Fehler aufgetreten.
        Catch argument As ArgumentException
            'hostNameOrAddress ist keine gültige IP-Adresse.
        End Try

    End Sub

    Private Sub btn_refresh_servers_Click(sender As Object, e As RoutedEventArgs) Handles btn_refresh_servers.Click
        Load_Servers()
        Ping_servers()
    End Sub

    Private Sub btn_add_servers_Click(sender As Object, e As RoutedEventArgs) Handles btn_add_servers.Click
        Dim editor As New ServerEditor()
        Dim result As Boolean?
        editor.ShowDialog()
        result = editor.DialogResult
        If result = True Then
            Load_Servers()
            Ping_servers()
        End If
    End Sub

    Private Sub btn_edit_servers_Click(sender As Object, e As RoutedEventArgs) Handles btn_edit_servers.Click
        If lb_servers.SelectedIndex <> -1 Then
            Dim server As ServerList.Server = DirectCast(lb_servers.SelectedItem, ServerList.Server)
            Dim editor As New ServerEditor(lb_servers.SelectedIndex, server)
            Dim result As Boolean?
            editor.ShowDialog()
            result = editor.DialogResult
            If result = True Then
                Load_Servers()
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
                servers.Servers.RemoveAt(selected)
                servers.Save()
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
            tb_server_address.Text = DirectCast(lb_servers.SelectedItem, ServerList.Server).ip
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
                                                         If Version.Contains("-") = True Then
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

    Private Sub btn_update_Click(sender As Object, e As RoutedEventArgs) Handles btn_update.Click
        If onlineversion > AssemblyVersion Then
            Dim Updater As New Updater
            Updater.Show()
        Else
            lbl_noupdatefound.Text = "Du besitzt die neueste Version"
        End If
    End Sub

    Private Sub cb_direct_join_Click(sender As Object, e As RoutedEventArgs) Handles cb_direct_join.Click
        Settings.DirectJoin = cb_direct_join.IsChecked.Value
        Settings.Save()
    End Sub

    Private Sub tb_server_address_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tb_server_address.TextChanged
        Settings.ServerAddress = tb_server_address.Text
        Settings.Save()
    End Sub
#Region "Tools"

    Private Sub forge_installer()
        Dim frg As New Forge_installer
        frg.ShowDialog()
    End Sub

    Sub liteloader_installer()
        Dim frg As New LiteLoader_installer
        frg.Show()
    End Sub

    Private Async Sub download_feedthebeast()
        'Zuerst die Website auslesen, um den neuesten Link zu bekommen
        'Dim str As String = Await New WebClient().DownloadStringTaskAsync("")
        Dim url As New Uri(Downloads.Downloadsjo("feedthebeast").Value(Of String)("url"))
        Dim filename As String = Downloads.Downloadsjo("feedthebeast").Value(Of String)("filename")
        Dim path As New FileInfo(IO.Path.Combine(mcpfad, "tools", filename))
        If path.Directory.Exists = False Then
            path.Directory.Create()
        End If
        Try
            btn_start_feedthebeast.IsEnabled = False
            'progressbar lädt herunter
            Await New WebClient().DownloadFileTaskAsync(url, path.FullName)
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

    Private Sub start_feedthebeast()
        Dim filename As String = Downloads.Downloadsjo("feedthebeast").Value(Of String)("filename")
        Dim path As New FileInfo(IO.Path.Combine(mcpfad, "tools", filename))
        Process.Start(path.FullName)
        'TechnicLauncher starten
    End Sub

    Private Async Sub download_techniclauncher()
        'Zuerst die Website auslesen, um den neuesten Link zu bekommen
        'Dim str As String = Await New WebClient().DownloadStringTaskAsync("")
        Dim url As New Uri(Downloads.Downloadsjo("techniclauncher").Value(Of String)("url"))
        Dim filename As String = Downloads.Downloadsjo("techniclauncher").Value(Of String)("filename")
        Dim path As New FileInfo(IO.Path.Combine(mcpfad, "tools", filename))
        If path.Directory.Exists = False Then
            path.Directory.Create()
        End If
        Try
            btn_start_techniclauncher.IsEnabled = False
            'progressbar lädt herunter
            Await New WebClient().DownloadFileTaskAsync(url, path.FullName)
            btn_start_techniclauncher.IsEnabled = True
        Catch ex As Exception
            Try
                If path.Exists Then
                    path.Delete()
                End If
            Catch
            End Try
            btn_start_techniclauncher.IsEnabled = False
            Me.ShowMessageAsync("Fehler", ex.Message, MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = "Ok", .ColorScheme = MetroDialogColorScheme.Accented})
        End Try
    End Sub

    Private Sub start_techniclauncher()
        Dim filename As String = Downloads.Downloadsjo("techniclauncher").Value(Of String)("filename")
        Dim path As New FileInfo(IO.Path.Combine(mcpfad, "tools", filename))
        Process.Start(path.FullName)
        'TechnicLauncher starten
    End Sub

    Sub Check_Tools_Downloaded()
        'For Each item As Stirng in
        'TechnicLauncher
        Dim technicfilename As String = Downloads.Downloadsjo("techniclauncher").Value(Of String)("filename").ToString
        Dim feedthebeastfilename As String = Downloads.Downloadsjo("feedthebeast").Value(Of String)("filename").ToString
        If File.Exists(Path.Combine(mcpfad, "tools", technicfilename)) = True Then
            btn_start_techniclauncher.IsEnabled = True
        Else
            btn_start_techniclauncher.IsEnabled = False
        End If
        If File.Exists(Path.Combine(mcpfad, "tools", feedthebeastfilename)) = True Then
            btn_start_feedthebeast.IsEnabled = True
        Else
            btn_start_feedthebeast.IsEnabled = False
        End If
    End Sub

#End Region

End Class

#Region "Converters"
Public Structure ImageConvert
    ''' <summary>
    ''' Konvertiert ein Bild in einen Base64-String
    ''' </summary>
    ''' <param name="image">
    ''' Zu konvertierendes Bild
    ''' </param>
    ''' <returns>
    ''' Base64 Repräsentation des Bildes
    ''' </returns>
    Public Shared Function GetStringFromImage(image As System.Drawing.Image) As String
        If image IsNot Nothing Then
            Dim ic As New ImageConverter()
            Dim buffer As Byte() = DirectCast(ic.ConvertTo(image, GetType(Byte())), Byte())
            Return Convert.ToBase64String(buffer, Base64FormattingOptions.InsertLineBreaks)
        Else
            Return Nothing
        End If
    End Function
    '---------------------------------------------------------------------
    ''' <summary>
    ''' Konvertiert einen Base64-String zu einem Bild
    ''' </summary>
    ''' <param name="base64String">
    ''' Zu konvertierender String
    ''' </param>
    ''' <returns>
    ''' Bild das aus dem String erzeugt wird
    ''' </returns>
    Public Shared Function GetImageFromString(base64String As String) As System.Drawing.Image
        Dim buffer As Byte() = Convert.FromBase64String(base64String)

        If buffer IsNot Nothing Then
            Dim ic As New ImageConverter()
            Return TryCast(ic.ConvertFrom(buffer), System.Drawing.Image)
        Else
            Return Nothing
        End If
    End Function

    Public Shared Function GetImageStream(Image As System.Drawing.Image) As BitmapSource
        Dim ms As New MemoryStream()
        Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
        ms.Position = 0
        Dim bi As New BitmapImage()
        bi.BeginInit()
        bi.StreamSource = ms
        bi.EndInit()

        Return bi
    End Function

End Structure

Public Class Base64ImageConverter
    Implements System.Windows.Data.IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Dim s As String = TryCast(value, String)

        If s Is Nothing Then Return Nothing

        Dim bi As New BitmapImage()

        bi.BeginInit()
        bi.StreamSource = New MemoryStream(System.Convert.FromBase64String(s))
        bi.EndInit()

        Return bi
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function

End Class

Public Class FormattingcodesStringConverter
    Implements System.Windows.Data.IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Dim s As String = TryCast(value, String)

        If s Is Nothing Then Return Nothing

        For Each item As String In FormattingCodes.Colorcodes
            s = s.Replace(item, Nothing)
        Next
        For Each item As String In FormattingCodes.Formattingcodes
            s = s.Replace(item, Nothing)
        Next
        'Neue Zeile für MotD -> "/n"

        Return s
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function

End Class

''' <summary>
''' Convertiert eine IList(Of ServerStatus.Playerlist.Player) zu einem String, getrennt durch neue Zeilen
''' </summary>
''' <remarks></remarks>
Public Class Playerlist_Namesstring_Converter
    Implements System.Windows.Data.IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Dim s As IList(Of ServerStatus.PlayerList.Player) = TryCast(value, IList(Of ServerStatus.PlayerList.Player))
        If s Is Nothing Then Return Nothing
        Dim playernames As IList(Of String) = s.Select(Function(p) p.Name).ToList
        Dim returnstring As String = String.Join(Environment.NewLine, playernames)
        Return returnstring
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function

End Class
Public Class MODS_installed_imageConverter
    Implements System.Windows.Data.IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Dim s As IList(Of Modifications.Mod.Version) = TryCast(value, IList(Of Modifications.Mod.Version))
        If s Is Nothing Then Return Nothing
        Dim r As BitmapSource
        If s.Where(Function(p) p.version = SelectedModVersion).First.installed = True Then
            r = ImageConvert.GetImageStream(My.Resources.check_green)
        Else
            r = Nothing
        End If
        Return r
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function

End Class

#End Region