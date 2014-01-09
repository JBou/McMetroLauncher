Imports System.Net
Imports System.IO
Imports System.Data
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
Imports System.Windows.Input
Imports Microsoft.Win32
Imports Ookii.Dialogs.Wpf
Imports System.Xml
Imports fNbt
Imports System.Threading
Imports System.Security.Cryptography

Module GlobaleVariablen
    Public Website As String = "http://patzleiner.net"
    Public Appdata As New DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
    Public mcpfad As String = Appdata.FullName & "\.minecraft"
    Public modsfile As String = mcpfad & "\cache\modlist.json"
    Public librariesfolder As String = mcpfad & "\libraries"
    Public assetspath As String = mcpfad & "\assets"
    Public launcher_profiles_json As String = mcpfad & "\launcher_profiles.json"
    Public servers_dat As String = mcpfad & "\servers.dat"
    Public Modsurl As String = "http://youyougabbo.square7.ch/minecraft/mods/"
    Public Newprofile As Boolean
    Public selectedprofile As String
    Public outputjsonversions As String = mcpfad & "\cache\versions.json"
    Public ReadOnly Property versionsJSON As String
        Get
            Return mcpfad & "\versions\" & lastversionID & "\" & lastversionID & ".json"
        End Get
    End Property
    Public Versionsjar As String
    Public UnpackDirectory As String
    Public Arguments As String
    Public serverip As String = "gabrielpatzleiner.tk"
    Public serverport As Integer = 25565
    Public JSONAPIpassword As String = Nothing
    Public JSONAPIsalt As String = Nothing
    Public JSONAPIport As Integer = 20059
    Public JSONAPI_RTKport As Integer = 25561
    Public IsStarting As Boolean
    Public versionsidlist As IList(Of String) = New List(Of String)
    Public versiontypelist As IList(Of String) = New List(Of String)
    Public lastversionID As String
    Public Delegate Sub WriteA(ByVal Text As String)
    Public modsfolder As String = mcpfad & "\mods"
    Public cachefolder As String = mcpfad & "\cache"
    Public downloadfilepath As String
    Public servers As IList(Of Server) = New List(Of Server)
    Public Applicationdata As New DirectoryInfo(Path.Combine(Appdata.FullName, "McMetroLauncher"))
    Public Applicationcache As New DirectoryInfo(IO.Path.Combine(Applicationdata.FullName, "cache"))
    Public onlineversion As String = Nothing
    Public changelog As String = Nothing
    Public resources_dir As New DirectoryInfo(Path.Combine(mcpfad, "resources"))
    Public librariesurl As String = "https://libraries.minecraft.net/"

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
    Public modfileurl As String = Website & "/download/modlist.json"
    Public versionurl As String = Website & "/mcmetrolauncher/version.txt"
    Public changelogurl As String = Website & "/mcmetrolauncher/changelog.txt"

End Module

Public Class AccentColorMenuData
    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(value As String)
            m_Name = Value
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
    Private moddownloading As Boolean = False
    Private modsdownloadlist As IList(Of ForgeMod) = New List(Of ForgeMod)
    Private modsdownloadindex As Integer
    Private Modsfilename As String
    Private modslist As IList(Of ForgeMod)
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
    '***************VersionsInfo****************
    Public VersionsInfos As VersionsInfo
    '******************Others*******************
    Private AccentColors As List(Of AccentColorMenuData)

    Private ReadOnly Property AssemblyVersion As String
        Get
            Return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
        End Get
    End Property

    Sub Check_Updates()
        If onlineversion > AssemblyVersion Then
            Dim Updater As New Updater
            Updater.Show()
        End If
    End Sub

    Private Sub ThemeLight()
        Dim theme = ThemeManager.DetectTheme(Application.Current)
        ThemeManager.ChangeTheme(Application.Current, theme.Item2, MahApps.Metro.Theme.Light)
        Settings.Theme = "Light"
        Settings.Save()
        btn_refresh_image.Source = ImageConvert.GetImageStream(My.Resources.appbar_refresh_light)
        btn_list_delete_mod_image.Source = ImageConvert.GetImageStream(My.Resources.appbar_delete_light)
    End Sub

    Private Sub ThemeDark()
        Dim theme = ThemeManager.DetectTheme(Application.Current)
        ThemeManager.ChangeTheme(Application.Current, theme.Item2, MahApps.Metro.Theme.Dark)
        Settings.Theme = "Dark"
        Settings.Save()
        btn_refresh_image.Source = ImageConvert.GetImageStream(My.Resources.appbar_refresh_dark)
        btn_list_delete_mod_image.Source = ImageConvert.GetImageStream(My.Resources.appbar_delete_dark)
    End Sub

    Private Sub OnMenuItemClicked(sender As Object, e As RoutedEventArgs)
        Dim item As MenuItem = TryCast(e.OriginalSource, MenuItem)
        ' Handle the menu item click here
        If item IsNot Nothing Then
            ChangeAccent(item.Header.ToString)
        End If
    End Sub

    Private Sub ChangeAccent(accentname As String)
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

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Check_Updates()
        'AccentColors = ThemeManager.DefaultAccents.Select(Function(p) p.Name)
        ' create accent color menu items for the demo
        AccentColors = ThemeManager.DefaultAccents.Select(Function(a) New AccentColorMenuData() With { _
                .Name = a.Name,
                .ColorBrush = New SolidColorBrush(CType(Windows.Media.ColorConverter.ConvertFromString(a.Resources("AccentColorBrush").ToString), System.Windows.Media.Color))
        }).ToList
        ShowWindowCommandsOnTop = False
        Menuitem_accent.ItemsSource = AccentColors
        tb_modsfolder.Text = modsfolder
        Load_ModVersions()
        'tb_username.Text = My.Settings.Username
        Get_Profiles()
        Settings.Load()
        tb_username.Text = Settings.Username
        If Settings.Accent <> Nothing Then
            ChangeAccent(Settings.Accent)
        End If
        If Settings.Theme = "Dark" Then
            ThemeDark()
        Else
            ThemeLight()
        End If
        Load_Servers()
        Ping_servers()
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
        For Each Profile As String In Profiles.List
            cb_profiles.Items.Add(Profile)
        Next
        If jo.Properties.Select(Function(p) p.Name).Contains("selectedProfile") = True Then
            cb_profiles.SelectedItem = selectedprofile
        Else
            jo.Add(New JProperty("selectedProfile"))
            cb_profiles.SelectedIndex = 0
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

    Sub Download_Resources()
        Dim jo As JObject = JObject.Parse(IO.File.ReadAllText(versionsJSON))
        If jo.Properties.Select(Function(p) p.Name).Contains("assets") = True Then
            assets_index_name = jo.Value(Of String)("assets")
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
            Parse_Resources()
        End If
    End Sub

    Sub downloadindexesfinished(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs)
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
        Parse_Resources()
    End Sub

    Sub Parse_Resources()
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
        pb_download.Maximum = resourcesindexes.objects.Count
        resourcesdownloadindex = 0
        resourcesdownloadtry = 1
        resourcesdownloading = True
        DownloadResources()
    End Sub

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
                IsStarting = False
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
        If IsStarting = True Then
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

    'Sub Download_Resources()

    'If wcresources.IsBusy Or wcversionsdownload.IsBusy Then
    '    MessageBox.Show("Download läuft!", MessageBox.Show.ShowStyle.Information, "Achtung")
    'Else

    '    'Auslesen

    '    Try

    '        Dim doc As New Xml.XmlDocument

    '        doc.Load(resourcesfile)

    '        Dim xmlTitles As Xml.XmlNodeList = doc.GetElementsByTagName("Key")

    '        'Dim myNode As XmlNode = doc.SelectSingleNode("//Personen") ' Geht zum gewünschten Hauptknoten
    '        'myNode = myNode.SelectSingleNode("Person")                      ' Geht weiter zum angegebenen Knoten
    '        'TextBox1.Text = myNode.Attributes("Titel").Value                ' Holt sich den Wert des Attributs

    '        'In Listbox eintragen

    '        For i As Integer = 0 To xmlTitles.Count - 1
    '            If xmlTitles(i).FirstChild.Value.EndsWith("/") = False Then

    '                Listbox1.Items.Add(xmlTitles(i).FirstChild.Value)

    '            End If
    '        Next

    '    Catch ex As Exception
    '        Dispatcher.Invoke(New WriteA(AddressOf Write), ex.Message & vbNewLine)
    '    End Try

    '    'Download

    '    pb_download.Maximum = Listbox1.Items.Count
    '    pb_download.Value = 0
    '    For i = 0 To Listbox1.Items.Count - 1
    '        Dim Inputfile As String = resourcesurl & "/" & Listbox1.Items.Item(i).ToString
    '        Dim Outputfile As String = resources_dir & "\" & Listbox1.Items.Item(i).ToString
    '        Dim CacheOutputfile As String = mcpfad & "\cache\" & Listbox1.Items.Item(i).ToString
    '        Dim Directoryname As String = IO.Path.GetDirectoryName(Outputfile)
    '        Dim CacheDirectoryname As String = IO.Path.GetDirectoryName(CacheOutputfile)

    '        Try
    '            If IO.File.Exists(Outputfile) = False Then

    '                If IO.Directory.Exists(CacheDirectoryname) = False Then
    '                    IO.Directory.CreateDirectory(CacheDirectoryname)
    '                End If
    '                wcresources = New WebClient
    '                Dispatcher.Invoke(New WriteA(AddressOf Write), "Lade " & Listbox1.Items.Item(i).ToString & " herunter" & vbNewLine)
    '                Dispatcher.Invoke(New WriteA(AddressOf Write), Inputfile & vbNewLine)
    '                wcresources.DownloadFileAsync(New Uri(Inputfile), CacheOutputfile)
    '                While wcresources.IsBusy
    '                    DoEvents()
    '                End While

    '                If IO.Directory.Exists(Directoryname) = False Then
    '                    IO.Directory.CreateDirectory(Directoryname)
    '                End If

    '                IO.File.Move(CacheOutputfile, Outputfile)
    '            End If
    '            pb_download.Value = i + 1
    '        Catch ex As Exception
    '            Dispatcher.Invoke(New WriteA(AddressOf Write), "Fehler beim herunterladen von " & Listbox1.Items.Item(i).ToString & " :" & vbNewLine & ex.Message & vbNewLine)
    '        End Try
    '    Next
    'End If
    'End Sub

    Sub Download_Version()
        If wcresources.IsBusy Or wcversionsdownload.IsBusy Then
            MessageBox.Show("Download läuft!", "Achtung", MessageBoxButton.OK, MessageBoxImage.Information)
        Else

            Dim VersionsURl As String = "https://s3.amazonaws.com/Minecraft.Download/versions/" & lastversionID & "/" & lastversionID & ".jar"
            Dim VersionsJSONURL As String = "https://s3.amazonaws.com/Minecraft.Download/versions/" & lastversionID & "/" & lastversionID & ".json"
            Dim Outputfile As String = mcpfad & "\versions\" & lastversionID & "\" & lastversionID & ".jar"
            Dim CacheOutputfile As String = mcpfad & "\cache\versions\" & lastversionID & "\" & lastversionID & ".jar"
            Dim OutputfileJSON As String = mcpfad & "\versions\" & lastversionID & "\" & lastversionID & ".json"
            Dim CacheOutputfileJSON As String = mcpfad & "\cache\versions\" & lastversionID & "\" & lastversionID & ".json"
            Dim CacheDirectoryname As String = IO.Path.GetDirectoryName(CacheOutputfile)
            Dim Directoryname As String = IO.Path.GetDirectoryName(Outputfile)
            If IO.File.Exists(Outputfile) = False Then

                If IO.Directory.Exists(CacheDirectoryname) = False Then
                    IO.Directory.CreateDirectory(CacheDirectoryname)
                End If

                Write("Lade Minecraft Version " & lastversionID & " herunter")
                Try
                    wcversionsdownload.DownloadFileAsync(New Uri(VersionsURl), CacheOutputfile)
                Catch ex As Exception
                    Write("Fehler beim herunterladen von Minecraft " & lastversionID & " :" & vbNewLine & ex.Message & vbNewLine)
                End Try

                While wcversionsdownload.IsBusy
                    DoEvents()
                End While

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
                wcversionsdownload.DownloadFileAsync(New Uri(VersionsJSONURL), CacheOutputfileJSON)
                While wcversionsdownload.IsBusy
                    DoEvents()
                End While

                If IO.Directory.Exists(Directoryname) = False Then
                    IO.Directory.CreateDirectory(Directoryname)
                End If

                IO.File.Move(CacheOutputfileJSON, OutputfileJSON)
            End If
            Parse_VersionsInfo()
        End If
    End Sub

    Sub DoEvents()
        Me.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
            New Action(Sub()

                       End Sub))

    End Sub

    Sub Start_MC()
        ' Anwendungspfad setzen -> hier liegt es im Anwendungsordner
        If Startcmd() = Nothing Then
            Dim result As MessageBoxResult = MessageBox.Show("Du musst Java installieren, um Minecraft zu spielen. Jetzt herunterladen?", "Java fehlt", MessageBoxButton.OKCancel)

            If result = MessageBoxResult.OK Then
                Process.Start("http://java.com/de/download")
            End If
            Exit Sub
        End If

        Write("Starte Minecraft: " & Startcmd() & Environment.NewLine & Arguments)
        Write("Java " & GetJavaArch.ToString & " Bit")
        mc = New Process()
        With mc.StartInfo
            .FileName = Startcmd()
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
    End Sub

    Public Function GetJavaVersionInformation() As String
        Try
            Dim procStartInfo As New System.Diagnostics.ProcessStartInfo(Startcmd, "-version")

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

    Public Function GetJavaArch() As Integer

        If GetJavaVersionInformation.Contains("64-Bit") Then
            Return 64
        Else
            Return 32
        End If
    End Function

    Sub Download_Libraries()
        If VersionsInfos Is Nothing Then
            Parse_VersionsInfo()
        End If
        'http://wiki.vg/Game_Files
        pb_download.Maximum = VersionsInfos.libraries.Count
        librariesdownloadindex = 0
        librariesdownloadtry = 1
        librariesdownloading = True
        librariesdownloadfailures = 0
        DownloadLibraries()
    End Sub

    Sub Parse_VersionsInfo()
        Dim o As String = File.ReadAllText(versionsJSON)
        If o.Contains("${arch}") Then
            o = o.Replace("${arch}", GetJavaArch.ToString)
        End If
        VersionsInfos = JsonConvert.DeserializeObject(Of VersionsInfo)(o)
        VersionsInfos.JObject = JObject.Parse(o)
    End Sub

    Sub DownloadLibraries()
        pb_download.Value = librariesdownloadindex
        If librariesdownloadindex < VersionsInfos.libraries.Count Then
            Currentlibrary = VersionsInfos.libraries.Item(librariesdownloadindex)
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
                If VersionsInfos.JObject("libraries").Item(librariesdownloadindex).Value(Of JObject).Properties.Select(Function(p) p.Name).Contains("url") = False Then
                    url = librariesurl & Currentlibrary.path
                Else
                    url = VersionsInfos.JObject("libraries").Item(librariesdownloadindex).Value(Of String)("url") & Currentlibrary.path
                End If
                Dim librarypath As New FileInfo(IO.Path.Combine(librariesfolder, Currentlibrary.path.Replace("/", "\")))

                'libraryurl & ".sha1" enthält hash
                Dim a As New WebClient()
                'Hash herunterladen
                If librarypath.Directory.Exists = False Then
                    librarypath.Directory.Create()
                End If
                a.DownloadFileAsync(New Uri(url & ".sha1"), librarypath.FullName & ".sha1")
                While a.IsBusy
                    DoEvents()
                End While
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
                        MsgBox(Currentlibrarysha1)
                        wc_libraries.DownloadFileAsync(New Uri(url), librarypath.FullName)
                        Write("Library wird heruntergeladen (Versuch " & librariesdownloadtry & "): " & librarypath.FullName)
                    Else
                        If librarypath.Exists Then
                            Write("Library konnte nicht auf Hash überprüft werden und wird übersprungen, in der Annahme, dass die lokale Datei gut ist: " & librarypath.FullName)
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
    End Sub

    Private Sub wc_libraries_DownloadFileCompleted(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs) Handles wc_libraries.DownloadFileCompleted
        If e.Error IsNot Nothing Then
            Write("Library konnte nicht heruntergeladen werden: " & e.Error.Message)
            librariesdownloadindex += 1
            librariesdownloadtry = 1
            librariesdownloadfailures += 1
            DownloadLibraries()
        End If
        If e.Cancelled = False And e.Error Is Nothing Then
            If librariesdownloadtry > 3 Then
                librariesdownloadfailures += 1
                Write("---Der Download wurde aufgrund zu vieler Fehlversuche abgebrochen!")
                IsStarting = False
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
        If IsStarting = True Then
            Get_Startinfos()
            Start_MC()
            IsStarting = False
        End If
    End Sub

    Sub Get_Libraries()
        Dim o As String = IO.File.ReadAllText(versionsJSON)
        Dim jo As JObject = JObject.Parse(o)
        Dim i As Integer = 0
        Dim library As String = CStr(jo.SelectToken("libraries[" & i & "].name"))
        Dim rulesallow As String = CStr((jo.SelectToken("libraries[" & i & "].rules[0].os.name")))
        Dim rulesdisallow As String = CStr((jo.SelectToken("libraries[" & i & "].rules[1].os.name")))
        Dim windows_natives As String = CStr((jo.SelectToken("libraries[" & i & "].natives.windows")))
        Dim url As String = CStr((jo.SelectToken("libraries[" & i & "].url")))
        Dim extract As JObject = CType(jo.SelectToken("libraries[" & i & "].extract"), JObject)
        Dim librariesrootURL As String = librariesURL

        'ist ein JArray, kann es aber nicht auslesen :(
        'Dim exclude As String = CStr(jo.SelectToken("libraries[" & i & "].extract.exclude"))


        Write("Libraries werden heruntergeladen")

        lb_libraries.Items.Clear()
        lb_libraries_extract.Items.Clear()

        For i = 0 To jo("libraries").Count - 1
            If rulesdisallow IsNot "windows" Then
                If rulesallow Is Nothing Or rulesallow Is "windows" Then
                    ' String to search in.
                    Dim SearchString As String = library
                    ' Search for "P".
                    Dim SearchChar As String = ":"

                    Dim doublepointindex As Integer
                    ' A textual comparison starting at position 4. Returns 6.
                    doublepointindex = InStr(1, SearchString, SearchChar, CompareMethod.Text)

                    Dim filenametemp1 As String = Mid(library, 1, doublepointindex - 1) & "/"
                    filenametemp1 = filenametemp1.Replace(".", "/")

                    Dim filenametemp2 As String = Mid(library, doublepointindex + 1, library.Length) & "/"
                    filenametemp2 = filenametemp2.Replace(":", "/")

                    Dim filenametemp3 As String = Mid(library, doublepointindex + 1, library.Length)
                    filenametemp3 = filenametemp3.Replace(":", "-")

                    If windows_natives IsNot Nothing Then
                        windows_natives = "-" & windows_natives
                    End If

                    If url <> Nothing Then
                        librariesrootURL = url.ToString
                    Else
                        librariesrootURL = librariesURL
                    End If

                    Dim libraryfiles As String = filenametemp1 & filenametemp2 & filenametemp3 & windows_natives & ".jar"
                    If libraryfiles.Contains("${arch}") = True Then
                        libraryfiles = libraryfiles.Replace("${arch}", GetJavaArch.ToString)
                    End If

                    'MessageBox.Show(rulesallow & "-" & rulesdisallow & "-" & libraryfiles)
                    lb_libraries_url.Items.Add(librariesrootURL)
                    lb_libraries.Items.Add(libraryfiles)

                    If extract IsNot Nothing Then
                        lb_libraries_extract.Items.Add(libraryfiles)
                    End If

                End If
            End If

            library = CStr(jo.SelectToken("libraries[" & i & "].name"))
            rulesallow = CStr((jo.SelectToken("libraries[" & i & "].rules[0].os.name")))
            rulesdisallow = CStr((jo.SelectToken("libraries[" & i & "].rules[1].os.name")))
            windows_natives = CStr((jo.SelectToken("libraries[" & i & "].natives.windows")))
            extract = CType(jo.SelectToken("libraries[" & i & "].extract"), JObject)
            url = CStr((jo.SelectToken("libraries[" & i & "].url")))
            'exclude = CStr(jo.SelectToken("libraries[" & i & "].extract.exclude"))

        Next

        'Download
        pb_download.Maximum = lb_libraries.Items.Count
        pb_download.Value = 0
        For i = 0 To lb_libraries.Items.Count - 1
            Dim Inputfile As String = lb_libraries_url.Items.Item(i).ToString & lb_libraries.Items.Item(i).ToString
            Dim Outputfile As String = mcpfad & "\libraries\" & lb_libraries.Items.Item(i).ToString
            Dim CacheOutputfile As String = mcpfad & "\cache\" & lb_libraries.Items.Item(i).ToString
            Dim Directoryname As String = IO.Path.GetDirectoryName(Outputfile)
            Dim CacheDirectoryname As String = IO.Path.GetDirectoryName(CacheOutputfile)

            Try

                If IO.File.Exists(Outputfile) = False Then

                    If IO.Directory.Exists(CacheDirectoryname) = False Then
                        IO.Directory.CreateDirectory(CacheDirectoryname)
                    End If
                    Write("Lade " & Inputfile & " herunter")
                    Try
                        wc_libraries.DownloadFileAsync(New Uri(Inputfile), CacheOutputfile)

                        While wc_libraries.IsBusy
                            DoEvents()
                        End While

                        If IO.Directory.Exists(Directoryname) = False Then
                            IO.Directory.CreateDirectory(Directoryname)
                        End If
                        IO.File.Move(CacheOutputfile, Outputfile)
                    Catch ex As Exception
                        Write("Fehler beim herunterladen von " & Inputfile & " :" & vbNewLine & ex.Message & vbNewLine)
                    End Try

                End If

                pb_download.Value = pb_download.Value + 1
            Catch ex As Exception
                MessageBox.Show(ex.Message)

            End Try
        Next



    End Sub

    Sub Unzip()

        Write("Natives werden entpackt")
        UnpackDirectory = Path.Combine(mcpfad, "versions", lastversionID, lastversionID & "-natives-" & DateTime.Now.Ticks.ToString)
        If lb_startedversions.Items.Contains(UnpackDirectory) = False Then
            lb_startedversions.Items.Add(UnpackDirectory)
        End If
        For Each item In VersionsInfos.libraries.Where(Function(p) p.natives IsNot Nothing)
            With item
                If .natives IsNot Nothing Then
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

    End Sub

    Sub Get_Startinfos()
        Write("Startinfos werden ausgelesen")
        Unzip()
        Versionsjar = mcpfad & "\versions\" & lastversionID & "\" & lastversionID & ".jar"
        Dim o As String = IO.File.ReadAllText(versionsJSON)
        Dim jo As JObject = JObject.Parse(o)
        Dim mainClass As String = (jo.SelectToken("mainClass")).ToString
        Dim minecraftArguments As String = (jo.SelectToken("minecraftArguments")).ToString
        Dim libraries As String = Nothing
        Dim gamedir As String
        Parse_VersionsInfo()
        For i = 0 To VersionsInfos.libraries.Count - 1
            Dim librarytemp As Library = VersionsInfos.libraries.Item(i)
            If librarytemp.natives Is Nothing Then
                libraries &= Path.Combine(librariesfolder, librarytemp.path.Replace("/", "\") & ";")
            Else
                If librarytemp.natives.windows IsNot Nothing Then
                    libraries &= Path.Combine(librariesfolder, librarytemp.path.Replace("/", "\") & ";")
                End If
            End If
        Next

        If Profiles.gameDir(selectedprofile) <> Nothing Then
            gamedir = Profiles.gameDir(selectedprofile)
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

        minecraftArguments = minecraftArguments.Replace("${auth_player_name}", tb_username.Text)
        minecraftArguments = minecraftArguments.Replace("${version_name}", lastversionID)
        minecraftArguments = minecraftArguments.Replace("${game_directory}", gamedir)
        minecraftArguments = minecraftArguments.Replace("${game_assets}", assets_dir)
        minecraftArguments = minecraftArguments.Replace("${assets_root}", assets_dir)
        minecraftArguments = minecraftArguments.Replace("${assets_index_name}", assets_index_name)
        minecraftArguments = minecraftArguments.Replace("${user_properties}", New JObject().ToString)

        Dim natives As String = UnpackDirectory

        Dim javaargs As String
        Dim height As String
        Dim width As String

        If Profiles.javaArgs(selectedprofile) <> Nothing Then
            javaargs = Profiles.javaArgs(selectedprofile)
        Else
            'javaargs = "-Xmx" & RamCheck() & "M"
            javaargs = "-Xmx" & "1024" & "M"
        End If

        If Profiles.resolution_height(selectedprofile) <> Nothing Then
            height = " --height " & Profiles.resolution_height(selectedprofile)
        Else
            height = Nothing
        End If

        If Profiles.resolution_width(selectedprofile) <> Nothing Then
            width = " --width " & Profiles.resolution_width(selectedprofile)
        Else
            width = Nothing
        End If

        Arguments = javaargs & " -Djava.library.path=" & natives & " -cp " & libraries & Versionsjar & " " & mainClass & " " & minecraftArguments & height & width

        'MessageBox.Show(Arguments)

        'StartArgumente und mainclass ... von JSON IO.File
        'Überprüfen, ob username eingegeben wurde!
        'Libraries zum Start hinzufügen!

    End Sub

    Public Sub StartMC()

        If IsStarting = True Then
            MessageBox.Show("Minecraft wird bereits gestartet!", "Achtung", MessageBoxButton.OK, MessageBoxImage.Information)
        ElseIf cb_profiles.SelectedIndex = -1 Then
            MessageBox.Show("Wähle bitte ein Profil auswählen!", "Achtung", MessageBoxButton.OK, MessageBoxImage.Information)
        ElseIf tb_username.Text = Nothing Then
            MessageBox.Show("Gib einen Usernamen ein!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
        Else
            If Profiles.lastVersionId(selectedprofile) <> Nothing Then
                lastversionID = Profiles.lastVersionId(selectedprofile)
            Else
                'Wenn snapshots aktiviert sind, dann index 0, sonst latestrelease
                If Profiles.allowedReleaseTypes(selectedprofile).Count > 0 Then
                    If Profiles.allowedReleaseTypes(selectedprofile).Contains("snapshot") = True Then
                        'Version mit Index 0 auslesen
                        lastversionID = Versions.latest
                    Else
                        lastversionID = Versions.latestrelease
                    End If
                Else
                    lastversionID = Versions.latestrelease
                End If
            End If

            tb_ausgabe.Clear()
            IsStarting = True
            Try
                If IO.Directory.Exists(mcpfad & "\versions\" & lastversionID & "\" & lastversionID & "-natives") = True Then
                    IO.Directory.Delete(mcpfad & "\versions\" & lastversionID & "\" & lastversionID & "-natives", True)
                End If
            Catch
            End Try
            Download_Version()
            'Zuerst die Version wegen den Resources key
            Download_Resources()
        End If
    End Sub

    Private Sub btn_startMC_Click(sender As Object, e As RoutedEventArgs) Handles btn_startMC.Click
        StartMC()
    End Sub

    Private Sub p_OutputDataReceived(ByVal sender As Object, ByVal e As DataReceivedEventArgs) Handles mc.OutputDataReceived
        Try
            Write(e.Data)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub p_ErrorDataReceived(ByVal sender As Object, ByVal e As DataReceivedEventArgs) Handles mc.ErrorDataReceived
        Try
            Write(e.Data)
        Catch ex As Exception
        End Try
    End Sub

    Public Sub Append(ByVal Line As String)
        'Wenn zu viel Text in der Textbox ist, dann den obersten löschen
        If tb_ausgabe.LineCount >= 700 Then
        End If

        Me.Dispatcher.Invoke(
    DispatcherPriority.Send,
    New Action(Sub()

                   ' Do all the ui thread updates here
                   'tb_ausgabe.Text &= Line
                   tb_ausgabe.AppendText(Line)
                   tb_ausgabe.ScrollToEnd()

               End Sub))
        'Log schreiben

    End Sub

    ''' <summary>
    ''' Schreibt eine Zeile und anschleißend eine Neue Zeile ind die tb_ausgabe
    ''' </summary>
    ''' <param name="Line">Die Zeile, die geschrieben werden soll</param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Line As String)

        Dispatcher.Invoke(New WriteA(AddressOf Append), Line & Environment.NewLine)

    End Sub

    Public Shared Function CheckServerStatus() As Boolean
        Dim MinecraftServer As New TcpClient
        Try
            MinecraftServer.Connect(serverip, serverport)
            Return True
        Catch Ex As Exception
            Return False
        End Try
    End Function

    'Public Shared Function JavaCheck() As String

    '    If IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) & "\Java\jre7") = True Then
    '        Return "64"
    '    ElseIf IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) & "\Java\jre7") = True Then
    '        Return "32"
    '    Else
    '        Return Nothing
    '    End If

    'End Function

    'Public Shared Function JavaPath() As String
    '    If JavaCheck() = "64" Then
    '        Return "C:\Program Files\Java\jre7\bin\javaw.exe"
    '    Else
    '        Return "C:\Program Files (x86)\Java\jre7\bin\javaw.exe"
    '    End If
    'End Function

    Public Shared Function Startcmd() As String
        If Profiles.javaDir(selectedprofile) <> Nothing Then
            Return Profiles.javaDir(selectedprofile)
        Else
            Return Path.Combine(GetJavaPath(), "bin", "java.exe")
        End If
    End Function

    Public Shared Function GetJavaPath() As String
        Dim javaKey As String = "SOFTWARE\JavaSoft\Java Runtime Environment"
        Using baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(javaKey)
            Dim currentVersion As String = baseKey.GetValue("CurrentVersion").ToString()
            Using homeKey = baseKey.OpenSubKey(currentVersion)
                Return homeKey.GetValue("JavaHome").ToString()
            End Using
        End Using
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

    Private Sub TabControl_main_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles TabControl_main.SelectionChanged
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

        Catch
        End Try
    End Sub

    Private Sub btn_delete_profile_Click(sender As Object, e As RoutedEventArgs) Handles btn_delete_profile.Click
        If cb_profiles.Items.Count > 1 Then
            Profiles.Remove(cb_profiles.SelectedItem.ToString)
            cb_profiles.SelectedIndex = 0
            Get_Profiles()
        Else
            System.Windows.MessageBox.Show("Das letzte Profil kann nicht gelöscht werden!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Exclamation)
        End If
    End Sub

    Private Sub btn_downloadmod_Click(sender As Object, e As RoutedEventArgs) Handles btn_downloadmod.Click
        If moddownloading = True Then
            MessageBox.Show("Eine Mod wird bereits heruntergeladen. Warte bitte, bis diese fertig ist!", "Download läuft", MessageBoxButton.OK, MessageBoxImage.Information)
        Else
            btn_resetmodsfoler.IsEnabled = False
            btn_selectmodsfolder.IsEnabled = False
            btn_refresh.IsEnabled = False
            btn_downloadmod.IsEnabled = False
            moddownloading = True
            lbl_mods_status.Content = Nothing
            modsdownloadlist.Clear()
            For Each selectedmod As ForgeMod In lb_mods.SelectedItems
                modsdownloadlist.Add(selectedmod)
                For Each item As String In Mods.All_Needed_Mods(selectedmod.name, cb_modversions.SelectedItem.ToString)
                    Dim moditem As ForgeMod = modslist.Where(Function(p) p.name = item).First
                    If modsdownloadlist.Contains(moditem) = False Then
                        modsdownloadlist.Add(moditem)
                    End If
                Next
            Next
            modsdownloadindex = 0
            download_mod()
        End If
    End Sub

    Private Sub download_mod()
        If modsdownloadindex < modsdownloadlist.Count Then
            If tb_modsfolder.Text.Contains(IO.Path.GetInvalidPathChars) = True Then
                MessageBox.Show("Der Pfad des Mods Ordner enthält ungültige Zeichen", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error)
            Else
                Dim Version As String = modsdownloadlist.Item(modsdownloadindex).version
                Dim path As String = tb_modsfolder.Text
                Dim name As String = modsdownloadlist.Item(modsdownloadindex).name
                Dim url As New Uri(modsdownloadlist.Item(modsdownloadindex).downloadlink)
                lbl_mods_status.Content = modsdownloadindex + 1 & " / " & modsdownloadlist.Count & " " & name
                If Version >= "1.6.4" = True Then
                    Modsfilename = Version & "\" & modsdownloadlist.Item(modsdownloadindex).id & "." & modsdownloadlist.Item(modsdownloadindex).extension
                Else
                    Modsfilename = Version & "-" & modsdownloadlist.Item(modsdownloadindex).id & "." & modsdownloadlist.Item(modsdownloadindex).extension
                End If
                'If url.Host = "mega.co.nz" Then
                '    'AddHandler Megalib.DownloadProgress, AddressOf DownloadProgress
                '    'AddHandler Megalib.DownloadFinished, AddressOf DownloadModFinished
                '    'Megalib.download_url(Mods.downloadlinkAt(Version, Mods.Name(name, Version)), path)
                'Else
                Try
                    If IO.Directory.Exists(IO.Path.GetDirectoryName(cachefolder & "\" & Modsfilename)) = False Then
                        IO.Directory.CreateDirectory((IO.Path.GetDirectoryName(cachefolder & "\" & Modsfilename)))
                    End If
                    wcmod.DownloadFileAsync(url, cachefolder & "\" & Modsfilename)
                Catch ex As Exception
                    lbl_mods_status.Content = ex.Message
                    MessageBox.Show(ex.Message)
                    Mod_Download_finished()
                    Exit Sub
                End Try
                'End If
                modsdownloadindex += 1
                Dim selected As Integer = lb_mods.SelectedIndex
                Load_Mods()
                lb_mods.SelectedIndex = selected
            End If
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
                Dim path As String = tb_modsfolder.Text & "\" & Modsfilename
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
        Load_Mods()
        lb_mods.SelectedIndex = selected
    End Sub

    Public Sub Load_ModVersions()
        cb_modversions.Items.Clear()
        Dim modversionslist As IList(Of String) = Mods.Get_ModVersions
        For Each Modversion As String In modversionslist
            cb_modversions.Items.Add(Modversion)
        Next
        cb_modversions.SelectedIndex = 0
    End Sub

    Public Sub Load_Mods()
        Try
            lb_mods.Items.Clear()
            modslist = Mods.Get_Mods(cb_modversions.SelectedItem.ToString, tb_modsfolder.Text)
            Filter_Mods()
        Catch
        End Try
    End Sub

    Private Sub Filter_Mods()
        lb_mods.Items.Clear()
        For Each Moditem As ForgeMod In modslist
            If Moditem.name.ToLower.Contains(tb_search_mods.Text.ToLower) = True Then
                lb_mods.Items.Add(Moditem)
            End If
        Next
        If lb_mods.Items.Count >= 1 Then
            lb_mods.SelectedIndex = 0
        End If
    End Sub

    Private Sub cb_modversions_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cb_modversions.SelectionChanged
        Load_Mods()
    End Sub

    Private Sub lb_mods_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lb_mods.SelectionChanged
        'Load_Modinfos
        If lb_mods.SelectedIndex <> -1 Then
            'If DirectCast(lb_mods.SelectedItem, ForgeMod).installed = True Then
            '    Dim c As New ImageSourceConverter()
            '    img_installed.Source = CType(c.ConvertFrom(My.Resources.check_green), ImageSource)
            'Else
            '    img_installed.Source = Nothing
            'End If
            If lb_mods.SelectedItems.Count > 1 Then
                btn_downloadmod.Content = lb_mods.SelectedItems.Count & " Installieren"
            Else
                btn_downloadmod.Content = "Installieren"
            End If
            lbl_name.Content = DirectCast(lb_mods.SelectedItem, ForgeMod).name
            lbl_autor.Content = DirectCast(lb_mods.SelectedItem, ForgeMod).autor
            tb_description.Text = DirectCast(lb_mods.SelectedItem, ForgeMod).description
            If DirectCast(lb_mods.SelectedItem, ForgeMod).installed = True Then
                img_installed.Visibility = Windows.Visibility.Visible
                btn_list_delete_mod.IsEnabled = True
            Else
                btn_list_delete_mod.IsEnabled = False
                img_installed.Visibility = Windows.Visibility.Hidden
            End If
        End If


        'If lb_mods.SelectedIndex <> -1 Then
        '    lbl_name.Content = Mods.NameAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
        '    tb_description.Text = Mods.descriptionAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
        '    mod_website = Mods.websiteAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
        '    mod_video = Mods.videoAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
        '    mod_downloadlink = Mods.downloadlinkAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
        'End If
    End Sub

    'Public Sub DownloadSongStarted(ByVal sender As Object, ByVal e As MegaLibrary.Mega.DownloadStartedEventArgs)

    '    StopPlayback()
    '    Dim source As IWaveSource = Nothing
    '    Try
    '        source = CodecFactory.Instance.GetCodec(modsfolder & "\" & e.filename)

    '    Catch ex As Exception
    '        MessageBox.Show("Dateityp wird nicht unterstützt.")
    '        Return
    '    End Try

    '    soundSource = source

    '    soundOut = New DirectSoundOut()
    '    soundOut.Initialize(source)
    '    soundOut.Play()

    'End Sub

    'Private Sub StopPlayback()
    '    If soundOut IsNot Nothing Then
    '        soundOut.Stop()
    '        soundOut.Dispose()
    '        soundOut = Nothing
    '    End If
    '    If soundSource IsNot Nothing Then
    '        soundSource.Dispose()
    '    End If
    'End Sub

    'Private Sub PlayButton_Click(sender As Object, e As EventArgs) Handles btn_play.Click
    '    If moddownloading = True Then
    '        MessageBox.Show("Ein Song wird bereits heruntergeladen. Warte bitte, bis diese fertig ist!", "Download läuft", MessageBoxButton.OK, MessageBoxImage.Information)
    '    Else
    '        moddownloading = True
    '        pb_mods_download.Value = 0
    '        AddHandler Megalib.DownloadProgress, AddressOf DownloadProgress
    '        AddHandler Megalib.DownloadFinished, AddressOf DownloadModFinished
    '        'AddHandler Megalib.DownloadStarted, AddressOf DownloadSongStarted
    '        Megalib.download_url(tb_downloadlink.Text, modsfolder)
    '    End If

    'End Sub

    Private Sub wcmod_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles wcmod.DownloadProgressChanged
        ' Do all the ui thread updates here
        Me.Dispatcher.Invoke(
            DispatcherPriority.Normal,
            New Action(Sub()

                           ' Do all the ui thread updates here
                           pb_mods_download.Value = e.ProgressPercentage

                       End Sub))
    End Sub

    Private Sub btn_website_Click(sender As Object, e As RoutedEventArgs) Handles btn_website.Click
        Process.Start(DirectCast(lb_mods.SelectedItem, ForgeMod).website)
    End Sub

    Private Sub btn_video_Click(sender As Object, e As RoutedEventArgs) Handles btn_video.Click
        Process.Start(DirectCast(lb_mods.SelectedItem, ForgeMod).video)
    End Sub

    Private Sub forge_installer()
        Dim frg As New Forge_installer
        frg.ShowDialog()
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

    Private Sub btn_resetmodsfoler_Click(sender As Object, e As RoutedEventArgs) Handles btn_resetmodsfoler.Click
        tb_modsfolder.Text = modsfolder
    End Sub

    Private Sub btn_selectmodsfolder_Click(sender As Object, e As RoutedEventArgs) Handles btn_selectmodsfolder.Click
        If Directory.Exists(modsfolder) = False Then
            Directory.CreateDirectory(modsfolder)
        End If
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

    Private Sub RefreshMods()
        'For Each item As ForgeMod In modslist
        '    Dim Struktur As String
        '    Dim Version As String
        '    Version = cb_versions.SelectedItem.ToString
        '    If Version >= "1.6.4" = True Then
        '        Struktur = Version & "\" & item.id & "." & item.extension
        '    Else
        '        Struktur = Version & "-" & item.id & "." & item.extension
        '    End If
        '    If File.Exists(modsfolder & "\" & Struktur) = True Then
        '        item.installed = True
        '    Else
        '        item.installed = False
        '    End If
        'Next

        'For Each item As ForgeMod In lb_mods.Items
        '    Dim Struktur As String
        '    Dim Version As String
        '    Version = cb_versions.SelectedItem.ToString
        '    If Version >= "1.6.4" = True Then
        '        Struktur = Version & "\" & item.id & "." & item.extension
        '    Else
        '        Struktur = Version & "-" & item.id & "." & item.extension
        '    End If
        '    If File.Exists(modsfolder & "\" & Struktur) = True Then
        '        item.installed = True
        '    Else
        '        item.installed = False
        '    End If
        'Next
        Try
            Dim selectedversion As String = cb_modversions.SelectedItem.ToString
            Dim selectedmod As Integer = lb_mods.SelectedIndex
            Dim SelectedItems As IList = lb_mods.SelectedItems
            Load_ModVersions()
            'Funktioniert nicht
            For Each item In SelectedItems
                lb_mods.SelectedItems.Add(item)
            Next
            cb_modversions.SelectedItem = selectedversion
            lb_mods.SelectedIndex = selectedmod
        Catch
        End Try
    End Sub

    Private Sub tb_search_mods_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tb_search_mods.TextChanged
        Filter_Mods()
    End Sub

    Private Sub btn_list_delete_mod_Click(sender As Object, e As RoutedEventArgs) Handles btn_list_delete_mod.Click
        Dim Version As String = DirectCast(lb_mods.SelectedItem, ForgeMod).version
        Dim Struktur As String
        If Version >= "1.6.4" = True Then
            Struktur = Version & "\" & DirectCast(lb_mods.SelectedItem, ForgeMod).id & "." & DirectCast(lb_mods.SelectedItem, ForgeMod).extension
        Else
            Struktur = Version & "-" & DirectCast(lb_mods.SelectedItem, ForgeMod).id & "." & DirectCast(lb_mods.SelectedItem, ForgeMod).extension
        End If
        If File.Exists(tb_modsfolder.Text & "\" & Struktur) = True Then
            File.Delete(tb_modsfolder.Text & "\" & Struktur)
        End If
        Dim selected As Integer = lb_mods.SelectedIndex
        Load_Mods()
        lb_mods.SelectedIndex = selected
    End Sub

    Private Sub tb_modsfolder_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tb_modsfolder.TextChanged
        RefreshMods()
    End Sub

#Region "Server"

    Public Sub Load_Servers()
        If File.Exists(servers_dat) = True Then
            Dim selected As Integer = lb_servers.SelectedIndex
            Dim servers_nbtfile As New fNbt.NbtFile
            servers_nbtfile.LoadFromFile(servers_dat)
            Dim servers_tag As NbtList = servers_nbtfile.RootTag.Get(Of NbtList)("servers")
            Dim lol As NbtCompound = CType(servers_tag.Last, NbtCompound)
            servers.Clear()
            lb_servers.Items.Clear()
            'servers = servers_tag.Select(Function(p) New Server(p.Item("name").StringValue, p.Item("ip").StringValue, Convert.ToBoolean(p.Item("hideAddress").ByteValue), If(p.Item("icon").HasValue, ImageConvert.GetImageStream(ImageConvert.GetImageFromString(p.Item("icon").StringValue)), ImageConvert.GetImageStream(My.Resources.transparent64_64)))).ToList
            For Each item As NbtCompound In servers_tag
                Dim name As String = item.Item("name").StringValue
                Dim ip As String = item.Item("ip").StringValue
                Dim hideAddress As Boolean = Convert.ToBoolean(item.Item("hideAddress").ByteValue)
                Dim icon As BitmapSource = Nothing
                If item.Tags.Select(Function(p) p.Name).Contains("icon") = False Then
                    'Überarbeiten
                    icon = ImageConvert.GetImageStream(My.Resources.transparent64_64)
                Else
                    icon = ImageConvert.GetImageStream(ImageConvert.GetImageFromString(item.Item("icon").StringValue))
                End If
                Dim server As New Server(name, ip, hideAddress, icon)
                servers.Add(server)
            Next
            For Each item As Server In servers
                lb_servers.Items.Add(item)
            Next
            If selected = -1 Then
                lb_servers.SelectedIndex = 0
            Else
                lb_servers.SelectedIndex = selected
            End If
            'Icon tag ist enthalten?!?
        Else
            MessageBox.Show("Die Server Datei existiert nicht!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
        End If
    End Sub

    Public Function Base64ToImage(base64String As String) As System.Drawing.Image
        ' Convert Base64 String to byte[]
        Dim imageBytes As Byte() = Convert.FromBase64String(base64String)
        Dim ms As New MemoryStream(imageBytes, 0, imageBytes.Length)

        ' Convert byte[] to Image
        ms.Write(imageBytes, 0, imageBytes.Length)
        Dim image__1 As System.Drawing.Image = System.Drawing.Image.FromStream(ms, True)
        Return image__1
    End Function

    Public Function ImageToBase64(image As System.Drawing.Image, format As System.Drawing.Imaging.ImageFormat) As String
        Using ms As New MemoryStream()
            ' Convert Image to byte[]
            image.Save(ms, format)
            Dim imageBytes As Byte() = ms.ToArray()

            ' Convert byte[] to Base64 String
            Dim base64String As String = Convert.ToBase64String(imageBytes)
            Return base64String
        End Using
    End Function

    Private Sub Ping_servers()

        Dim fThread = New Thread(New ThreadStart(AddressOf ThreadProc))
        fThread.IsBackground = True
        fThread.Start()

    End Sub

    Private Sub ThreadProc()
        Parallel.For(0, servers.Count - 1, Sub(b)
                                               CheckOnline(b)
                                           End Sub)
    End Sub

    Private Sub CheckOnline(ByVal i As Integer)
        Try
            Dim host As String = Nothing
            Dim port As Integer = Nothing
            If servers.Item(i).ip.Contains(":") = True Then
                Dim dpindex As Integer = servers.Item(i).ip.IndexOf(":")
                port = CInt(servers.Item(i).ip.Substring(dpindex + 1))
                host = servers.Item(i).ip.Substring(0, dpindex)
            Else
                host = servers.Item(i).ip
                port = 25565
            End If
            'MessageBox.Show(String.Join(" | ", host, port))

            Dim pinger As New ServerPing(host, port, 0, 15000)
            servers.Item(i).ServerPing = pinger
            Dispatcher.Invoke(New Action(Sub()
                                             Dim selected As Integer = lb_servers.SelectedIndex
                                             lb_servers.Items.RemoveAt(i)
                                             lb_servers.Items.Insert(i, servers.Item(i))
                                             lb_servers.SelectedIndex = selected
                                         End Sub))
        Catch
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
            Dim editor As New ServerEditor(lb_servers.SelectedIndex, DirectCast(lb_servers.SelectedItem, Server).name, DirectCast(lb_servers.SelectedItem, Server).ip)
            Dim result As Boolean?
            editor.ShowDialog()
            result = editor.DialogResult
            If result = True Then
                Load_Servers()
                Ping_servers()
            End If
        End If
    End Sub

    Private Sub btn_delete_servers_Click(sender As Object, e As RoutedEventArgs) Handles btn_delete_servers.Click
        If lb_servers.SelectedIndex <> -1 Then
            If MessageBox.Show("Bist du dir sicher, dass du den Server " & DirectCast(lb_servers.SelectedItem, Server).name & " entgültig löschen willst?", "Server löschen", MessageBoxButton.YesNo, MessageBoxImage.Information) = MessageBoxResult.Yes Then
                Dim nbtserverfile As New NbtFile
                nbtserverfile.LoadFromFile(servers_dat)
                nbtserverfile.RootTag.Get(Of NbtList)("servers").RemoveAt(lb_servers.SelectedIndex)
                nbtserverfile.SaveToFile(servers_dat, NbtCompression.None)
                Load_Servers()
                Ping_servers()
            End If
        End If
    End Sub




#End Region

    Private Sub btn_update_Click(sender As Object, e As RoutedEventArgs) Handles btn_update.Click
        If onlineversion > AssemblyVersion Then
            Dim Updater As New Updater
            Updater.Show()
        Else
            lbl_noupdatefound.Text = "Du besitzt die neueste Version"
        End If
    End Sub

End Class

Public Class ImageConvert
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
End Class