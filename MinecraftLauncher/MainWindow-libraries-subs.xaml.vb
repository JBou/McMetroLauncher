''Imports System.Net
''Imports System.IO
''Imports System.Data
''Imports System.Xml.Linq
''Imports Newtonsoft.Json
''Imports Newtonsoft.Json.Linq
''Imports System.Globalization
''Imports Ionic.Zip
''Imports System.Net.Sockets
''Imports MahApps.Metro
''Imports McMetroLauncher.ProfileEditor
''Imports System.Reflection
''Imports System.Windows.Forms
''Imports System.Windows.Threading
''Imports System.Text.RegularExpressions
''Imports MahApps.Metro.Accent
''Imports MahApps.Metro.Controls
''Imports System.Windows.Input
''Imports Microsoft.Win32
''Imports Ookii.Dialogs.Wpf
''Imports System.Xml

''Module GlobaleVariablenTest
''    Public Webspace As String = "http://youyougabbo.square7.ch/"
''    Public Website As String = "http://patzleiner.net"
''    Public Appdata As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
''    Public mcpfad As String = Appdata & "\.minecraft"
''    Public modsfile As String = mcpfad & "\cache\modlist.json"
''    Public librariespath As String = mcpfad & "\libraries"
''    Public assetspath As String = mcpfad & "\assets"
''    Public resourcesfile As String = mcpfad & "\cache\minecraft_resources.xml"
''    Public launcher_profiles_json As String = mcpfad & "\launcher_profiles.json"
''    Public Modsurl As String = "http://youyougabbo.square7.ch/minecraft/mods/"
''    Public Newprofile As Boolean
''    Public selectedprofile As String
''    Public outputjsonversions As String = mcpfad & "\cache\versions.json"
''    Public versionsJSON As String
''    Public Versionsjar As String
''    Public UnpackDirectory As String
''    Public Arguments As String
''    Public serverip As String = "gabrielpatzleiner.tk"
''    Public serverport As Integer = 25565
''    Public JSONAPIpassword As String = Nothing
''    Public JSONAPIsalt As String = Nothing
''    Public JSONAPIport As Integer = 20059
''Public JSONAPI_RTKport As Integer = 25561
''Public IsStarting As Boolean
''Public versionsidlist As IList(Of String) = New List(Of String)
''Public versiontypelist As IList(Of String) = New List(Of String)
''Public lastversionID As String
''Public Delegate Sub WriteA(ByVal Text As String)
''Public modsfolder As String = mcpfad & "\mods"
''Public cachefolder As String = mcpfad & "\cache"
''Public downloadfilepath As String

''End Module

'Public Class MainWindowTest

'    WithEvents wcresources As New System.Net.WebClient ' Für das WebClient steuerelement mit Events z.b. DownloadProgressChanged... 
'    WithEvents wcversionsdownload As New System.Net.WebClient
'    WithEvents wcresources_xml As New System.Net.WebClient
'    WithEvents wcversionsstring As New System.Net.WebClient
'    WithEvents wc_libraries As New System.Net.WebClient
'    WithEvents wcmod As New System.Net.WebClient
'    WithEvents mc As New Process
'    Private moddownloading As Boolean = False
'    Private downloadlist As IList(Of ForgeMod) = New List(Of ForgeMod)
'    Private downloadindex As Integer
'    Private Modsfilename As String
'    Private modslist As IList(Of ForgeMod)
'    Public WithEvents updController As New updateSystemDotNet.updateController
'    Dim librariesDownloadindex As Integer = 0

'    Private Sub UpdateControllerInitalisieren()
'        updController.updateUrl = "http://patzleiner.net/mcmetrolauncher/updatesystem/"
'        updController.projectId = "5fa10b5a-f769-498e-bb55-3a06f20e2c5e"
'        updController.publicKey = "<RSAKeyValue><Modulus>pesqJDkvV0z870bUwSIJlGs0mkk2lFmvtRYrQu991v5daNhRUsUTiKxl7vKipJYKQJ/bw1LfF5fd6ntSjcsMAR5dKQFmeS5jaz0R6oJg9qkI1ZygUJ1qb1oC0NpsruaCp9oe5MTOjEsPVfX4TELfhyIurp7AxiihHXJysjltPWqxZtMXs1OIVxnxzNDQ16T02m2Gve/F/hY4hXjiwgLRYZN/nwLghbxnljlfflbOVsvaWSC4Rw3YyPIveLg2kiTUcqoN3tBlKOQ5YDbPIzkOSt6TOlyGnDQFG1ClyzhyUMCWGhtLE1/QRytrs7SwzG+tB+ygPcqFkLf0FT6az3nylbsurpRARLM0C9K7XWN39yzfCcOHHTwbP0GxRc4x9WqsMKmJK1ofwe99Lf+0gZ4IPXoc2U4n+sOW18pDPOMSdddz9zblPia2hdSGgJ9z9cuQ3f2RLFQh1QDGzgvxwqLt+Sy0Km3LFDb3yoIq3HfVvuz9lVCz0SsLV/YOB/l4EYonC1nKCHvHXI68/XsZ0WKd0ZyQ/X2LeTENH0bF7DQc/shC5iTNjHuownP/Reo0YnzR0tKNyt7i7M+zAf+V4snt6ykxmXy7CauOll7u0AVnwkJ5lkNH2VG51nCc2LdvMzUhdiKAlTSc27LXTXPKz0vQL13PjbhUlHiENaYR8888ac0=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"
'        updController.releaseFilter.checkForFinal = True
'        updController.releaseFilter.checkForBeta = True
'        updController.releaseFilter.checkForAlpha = False
'        updController.restartApplication = True
'        updController.autoCloseHostApplication = True
'        updController.retrieveHostVersion = True
'    End Sub

'    Sub Check_Updates()
'        UpdateControllerInitalisieren()
'        updController.checkForUpdatesAsync()
'        'updController.updateInteractive()
'    End Sub

'    Private Sub updController_updateFound(sender As Object, e As updateSystemDotNet.appEventArgs.updateFoundEventArgs) Handles updController.updateFound
'        Dim updater As New Updater
'        updater.ShowDialog()
'    End Sub

'    Private Sub updController_checkForUpdatesCompleted(sender As Object, e As updateSystemDotNet.appEventArgs.checkForUpdatesCompletedEventArgs) Handles updController.checkForUpdatesCompleted
'        If e.Cancelled AndAlso e.Error IsNot Nothing Then
'            MessageBox.Show(e.Error.ToString, "Fehler!")
'            Return
'        End If
'    End Sub

'    Private Sub ThemeLight(sender As Object, e As RoutedEventArgs)
'        Dim theme = ThemeManager.DetectTheme(Application.Current)
'        ThemeManager.ChangeTheme(Application.Current, theme.Item2, MahApps.Metro.Theme.Light)
'    End Sub

'    Private Sub ThemeDark(sender As Object, e As RoutedEventArgs)
'        Dim theme = ThemeManager.DetectTheme(Application.Current)
'        ThemeManager.ChangeTheme(Application.Current, theme.Item2, MahApps.Metro.Theme.Dark)
'    End Sub

'    Private Sub ShowSettings(sender As Object, e As RoutedEventArgs)
'        'Contrrols auf ihre Einstellungen setzen
'        ToggleFlyout(0)
'    End Sub

'    Private Sub ToggleFlyout(index As Integer)
'        Dim flyout As Flyout = CType(Me.Flyouts.Items(index), Controls.Flyout)
'        If flyout Is Nothing Then
'            Return
'        End If

'        If flyout.IsOpen = True Then
'            flyout.IsOpen = False
'        Else
'            flyout.IsOpen = True
'        End If
'    End Sub

'    Private Sub Help()
'        'Zeige Hilfe
'    End Sub

'    Private Sub MainWindow_Closed(sender As Object, e As EventArgs) Handles Me.Closed
'        'My.Settings.Username = tb_username.Text.ToString
'        'My.Settings.Ram = cb_ram.SelectedItem.ToString
'        'My.Settings.Save()
'        Try
'            For i = 0 To lb_startedversions.Items.Count - 1
'                If IO.Directory.Exists(mcpfad & "\versions\" & lb_startedversions.Items.Item(i).ToString & "\" & lb_startedversions.Items.Item(i).ToString & "-natives") = True Then
'                    IO.Directory.Delete(mcpfad & "\versions\" & lb_startedversions.Items.Item(i).ToString & "\" & lb_startedversions.Items.Item(i).ToString & "-natives", True)
'                End If
'            Next

'            wcresources.CancelAsync()
'            wcversionsdownload.CancelAsync()
'            wcresources_xml.CancelAsync()
'            wcversionsstring.CancelAsync()
'            wc_libraries.CancelAsync()

'            If IO.Directory.Exists(mcpfad & "\cache") = True Then
'                IO.Directory.Delete(mcpfad & "\cache", True)
'            End If

'        Catch ex As Exception
'        End Try
'    End Sub

'    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
'        Check_Updates()
'        'AccentColors = ThemeManager.DefaultAccents.Select(Function(p) p.Name)
'        Me.ShowWindowCommandsOnTop = False
'        tb_modsfolder.Text = modsfolder
'        Load_ModVersions()
'        'tb_username.Text = My.Settings.Username
'        Get_Profiles()
'        Settings.Load()
'        tb_username.Text = Settings.Username
'        For i = 1 To 8
'            cb_ram.Items.Add(i & " GB")
'        Next

'        'If My.Settings.Ram = Nothing Then
'        '    cb_ram.SelectedIndex = 0
'        'Else
'        '    cb_ram.SelectedItem = My.Settings.Ram
'        'End If

'        'If CheckServerStatus() = True Then
'        '    lbl_Serverstatus.Content = "Server: Online"
'        'Else
'        '    lbl_Serverstatus.Content = "Server: Offline"
'        'End If

'    End Sub

'    Sub Get_Profiles()
'        Profiles.Load()
'        Dim jo As JObject = Profiles.profilesjo
'        Dim i As Integer = 0
'        Profiles.Load()
'        Dim selectedprofile As String = jo("selectedProfile").ToString
'        cb_profiles.Items.Clear()
'        For Each Profile As String In Profiles.List
'            cb_profiles.Items.Add(Profile)
'        Next
'        If jo.Properties.Select(Function(p) p.Name).Contains("selectedProfile") = True Then
'            cb_profiles.SelectedItem = selectedprofile
'        Else
'            jo.Add(New JProperty("selectedProfile"))
'            cb_profiles.SelectedIndex = 0
'        End If

'        If Profiles.List.Count = 0 Then
'            'StandartProfile schreiben
'            Dim standartprofile As New JObject(
'            New JProperty("profiles",
'                New JObject(
'                    New JProperty("Default",
'                        New JObject(
'                            New JProperty("name", "Default"))))),
'            New JProperty("selectedProfile", "Default"))
'            IO.File.WriteAllText(launcher_profiles_json, standartprofile.ToString)

'            Get_Profiles()

'        End If

'    End Sub

'    Sub Download_Resources()

'        If wcresources.IsBusy Or wcversionsdownload.IsBusy Then
'            MessageBox.Show("Download läuft!", MessageBoxStyle.Information, "Achtung")
'        Else

'            'Auslesen

'            Try

'                Dim doc As New Xml.XmlDocument

'                doc.Load(resourcesfile)

'                Dim xmlTitles As Xml.XmlNodeList = doc.GetElementsByTagName("Key")

'                'Dim myNode As XmlNode = doc.SelectSingleNode("//Personen") ' Geht zum gewünschten Hauptknoten
'                'myNode = myNode.SelectSingleNode("Person")                      ' Geht weiter zum angegebenen Knoten
'                'TextBox1.Text = myNode.Attributes("Titel").Value                ' Holt sich den Wert des Attributs

'                'In Listbox eintragen

'                For i As Integer = 0 To xmlTitles.Count - 1
'                    If xmlTitles(i).FirstChild.Value.EndsWith("/") = False Then

'                        Listbox1.Items.Add(xmlTitles(i).FirstChild.Value)

'                    End If
'                Next

'            Catch ex As Exception
'                Dispatcher.Invoke(New WriteA(AddressOf Write), ex.Message & vbNewLine)
'            End Try

'            'Download

'            pb_download.Maximum = Listbox1.Items.Count
'            pb_download.Value = 0
'            For i = 0 To Listbox1.Items.Count - 1
'                Dim NORM As New System.Net.WebClient
'                Dim Inputfile As String = "https://s3.amazonaws.com/Minecraft.Resources/" & Listbox1.Items.Item(i).ToString
'                Dim Outputfile As String = mcpfad & "\assets\" & Listbox1.Items.Item(i).ToString
'                Dim CacheOutputfile As String = mcpfad & "\cache\" & Listbox1.Items.Item(i).ToString
'                Dim Directoryname As String = IO.Path.GetDirectoryName(Outputfile)
'                Dim CacheDirectoryname As String = IO.Path.GetDirectoryName(CacheOutputfile)

'                Try
'                    If IO.File.Exists(Outputfile) = False Then

'                        If IO.Directory.Exists(CacheDirectoryname) = False Then
'                            IO.Directory.CreateDirectory(CacheDirectoryname)
'                        End If
'                        Dispatcher.Invoke(New WriteA(AddressOf Write), "Lade " & Listbox1.Items.Item(i).ToString & " herunter" & vbNewLine)
'                        NORM.DownloadFileAsync(New Uri(Inputfile), CacheOutputfile)
'                        While NORM.IsBusy
'                            DoEvents()
'                        End While

'                        If IO.Directory.Exists(Directoryname) = False Then
'                            IO.Directory.CreateDirectory(Directoryname)
'                        End If

'                        IO.File.Move(CacheOutputfile, Outputfile)
'                    End If
'                    pb_download.Value = i + 1
'                Catch ex As Exception
'                    Dispatcher.Invoke(New WriteA(AddressOf Write), "Fehler beim herunterladen von " & Listbox1.Items.Item(i).ToString & " :" & vbNewLine & ex.Message & vbNewLine)
'                End Try
'            Next
'        End If
'    End Sub

'    Sub Download_Version()
'        If wcresources.IsBusy Or wcversionsdownload.IsBusy Then
'            MessageBox.Show("Download läuft!", MessageBoxStyle.Information, "Achtung")
'        Else

'            Dim VersionsURl As String = "https://s3.amazonaws.com/Minecraft.Download/versions/" & lastversionID & "/" & lastversionID & ".jar"
'            Dim VersionsJSON As String = "https://s3.amazonaws.com/Minecraft.Download/versions/" & lastversionID & "/" & lastversionID & ".json"
'            Dim Outputfile As String = mcpfad & "\versions\" & lastversionID & "\" & lastversionID & ".jar"
'            Dim CacheOutputfile As String = mcpfad & "\cache\versions\" & lastversionID & "\" & lastversionID & ".jar"
'            Dim OutputfileJSON As String = mcpfad & "\versions\" & lastversionID & "\" & lastversionID & ".json"
'            Dim CacheOutputfileJSON As String = mcpfad & "\cache\versions\" & lastversionID & "\" & lastversionID & ".json"
'            Dim CacheDirectoryname As String = IO.Path.GetDirectoryName(CacheOutputfile)
'            Dim Directoryname As String = IO.Path.GetDirectoryName(Outputfile)
'            If IO.File.Exists(Outputfile) = False Then

'                If IO.Directory.Exists(CacheDirectoryname) = False Then
'                    IO.Directory.CreateDirectory(CacheDirectoryname)
'                End If

'                Dispatcher.Invoke(New WriteA(AddressOf Write), "Lade Minecraft Version " & lastversionID & " herunter" & Environment.NewLine)
'                Try
'                    wcversionsdownload.DownloadFileAsync(New Uri(VersionsURl), CacheOutputfile)
'                Catch ex As Exception
'                    Dispatcher.Invoke(New WriteA(AddressOf Write), "Fehler beim herunterladen von Minecraft " & lastversionID & " :" & vbNewLine & ex.Message & vbNewLine)
'                End Try

'                While wcversionsdownload.IsBusy
'                    DoEvents()
'                End While

'                If IO.Directory.Exists(Directoryname) = False Then
'                    IO.Directory.CreateDirectory(Directoryname)
'                End If

'                IO.File.Move(CacheOutputfile, Outputfile)

'            End If

'            If IO.File.Exists(OutputfileJSON) = False Then


'                If IO.Directory.Exists(CacheDirectoryname) = False Then
'                    IO.Directory.CreateDirectory(CacheDirectoryname)
'                End If

'                wcversionsdownload.DownloadFileAsync(New Uri(VersionsJSON), CacheOutputfileJSON)
'                While wcversionsdownload.IsBusy
'                    DoEvents()
'                End While

'                If IO.Directory.Exists(Directoryname) = False Then
'                    IO.Directory.CreateDirectory(Directoryname)
'                End If

'                IO.File.Move(CacheOutputfileJSON, OutputfileJSON)

'            End If

'        End If
'    End Sub

'    Sub DoEvents()
'        Me.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
'          New Action(Sub()

'                     End Sub))

'    End Sub

'    Sub Start_MC()
'        ' Anwendungspfad setzen -> hier liegt es im Anwendungsordner
'        If Startcmd() = Nothing Then
'            Dim result As MessageBoxResult = CType(MessageBox.Show("Du musst Java installieren, um Minecraft zu spielen. Jetzt herunterladen?", "Java fehlt", MessageBoxButton.OKCancel), MessageBoxResult)

'            If result = MessageBoxResult.OK Then
'                Process.Start("http://java.com/de/download")
'            End If
'            Exit Sub
'        End If

'        Dispatcher.Invoke(New WriteA(AddressOf Write), "Starte Minecraft: " & Startcmd() & vbNewLine & Arguments & vbNewLine)

'        mc = New Process()
'        With mc.StartInfo
'            .FileName = Startcmd()
'            .Arguments = Arguments
'            ' Arbeitsverzeichnis setzen falls nötig
'            .WorkingDirectory = ""
'            ' kein Window erzeugen
'            .CreateNoWindow = True
'            ' UseShellExecute auf falsch setzen
'            .UseShellExecute = False
'            ' StandardOutput von Console umleiten
'            .RedirectStandardError = True
'            .RedirectStandardInput = True
'            .RedirectStandardOutput = True
'        End With
'        ' Prozess starten
'        mc.Start()

'        mc.BeginErrorReadLine()
'        mc.BeginOutputReadLine()
'    End Sub

'    Sub Get_Libraries()
'        versionsJSON = mcpfad & "\versions\" & lastversionID & "\" & lastversionID & ".json"
'        Dim o As String = IO.File.ReadAllText(versionsJSON)
'        Dim jo As JObject = JObject.Parse(o)
'        Dim i As Integer = 0
'        Dim library As String = CStr(jo.SelectToken("libraries[" & i & "].name"))
'        Dim rulesallow As String = CStr((jo.SelectToken("libraries[" & i & "].rules[0].os.name")))
'        Dim rulesdisallow As String = CStr((jo.SelectToken("libraries[" & i & "].rules[1].os.name")))
'        Dim windows_natives As String = CStr((jo.SelectToken("libraries[" & i & "].natives.windows")))
'        Dim url As String = CStr((jo.SelectToken("libraries[" & i & "].url")))
'        Dim extract As JObject = CType(jo.SelectToken("libraries[" & i & "].extract"), JObject)
'        Dim librariesrootURL As String = "https://s3.amazonaws.com/Minecraft.Download/libraries/"

'        'ist ein JArray, kann es aber nicht auslesen :(
'        'Dim exclude As String = CStr(jo.SelectToken("libraries[" & i & "].extract.exclude"))


'        Dispatcher.Invoke(New WriteA(AddressOf Write), "Libraries werden heruntergeladen" & Environment.NewLine)

'        lb_libraries.Items.Clear()
'        lb_libraries_extract.Items.Clear()

'        For i = 0 To jo("libraries").Count - 1

'            If rulesdisallow IsNot "windows" Then
'                If rulesallow Is Nothing Or rulesallow Is "windows" Then

'                    ' String to search in.
'                    Dim SearchString As String = library
'                    ' Search for "P".
'                    Dim SearchChar As String = ":"

'                    Dim doublepointindex As Integer
'                    ' A textual comparison starting at position 4. Returns 6.
'                    doublepointindex = InStr(1, SearchString, SearchChar, CompareMethod.Text)

'                    Dim filenametemp1 As String = Mid(library, 1, doublepointindex - 1) & "/"
'                    filenametemp1 = filenametemp1.Replace(".", "/")

'                    Dim filenametemp2 As String = Mid(library, doublepointindex + 1, library.Length) & "/"
'                    filenametemp2 = filenametemp2.Replace(":", "/")

'                    Dim filenametemp3 As String = Mid(library, doublepointindex + 1, library.Length)
'                    filenametemp3 = filenametemp3.Replace(":", "-")

'                    If windows_natives IsNot Nothing Then
'                        windows_natives = "-" & windows_natives
'                    End If

'                    If url = Nothing Then
'                        librariesrootURL = "https://s3.amazonaws.com/Minecraft.Download/libraries/"
'                    Else
'                        librariesrootURL = url.ToString
'                    End If

'                    Dim libraryfiles As String = filenametemp1 & filenametemp2 & filenametemp3 & windows_natives & ".jar"


'                    'MessageBox.Show(rulesallow & "-" & rulesdisallow & "-" & libraryfiles)
'                    lb_libraries_url.Items.Add(librariesrootURL)
'                    lb_libraries.Items.Add(libraryfiles)

'                    If extract IsNot Nothing Then
'                        lb_libraries_extract.Items.Add(libraryfiles)
'                    End If

'                End If
'            End If

'            library = CStr(jo.SelectToken("libraries[" & i & "].name"))
'            rulesallow = CStr((jo.SelectToken("libraries[" & i & "].rules[0].os.name")))
'            rulesdisallow = CStr((jo.SelectToken("libraries[" & i & "].rules[1].os.name")))
'            windows_natives = CStr((jo.SelectToken("libraries[" & i & "].natives.windows")))
'            extract = CType(jo.SelectToken("libraries[" & i & "].extract"), JObject)
'            url = CStr((jo.SelectToken("libraries[" & i & "].url")))
'            'exclude = CStr(jo.SelectToken("libraries[" & i & "].extract.exclude"))

'        Next

'        'Download
'        pb_download.Maximum = lb_libraries.Items.Count
'        pb_download.Value = 0
'        librariesDownloadindex = 0
'        Download_Libraries()

'    End Sub

'    Sub Download_Libraries()
'        If librariesDownloadindex <= lb_libraries.Items.Count Then
'            Dim Inputfile As String = lb_libraries_url.Items.Item(librariesDownloadindex).ToString & lb_libraries.Items.Item(librariesDownloadindex).ToString
'            Dim Outputfile As String = mcpfad & "\libraries\" & lb_libraries.Items.Item(librariesDownloadindex).ToString
'            Dim CacheOutputfile As String = mcpfad & "\cache\" & lb_libraries.Items.Item(librariesDownloadindex).ToString
'            Dim Directoryname As String = IO.Path.GetDirectoryName(Outputfile)
'            Dim CacheDirectoryname As String = IO.Path.GetDirectoryName(CacheOutputfile)

'            Try
'                If IO.File.Exists(Outputfile) = False Then
'                    If IO.Directory.Exists(CacheDirectoryname) = False Then
'                        IO.Directory.CreateDirectory(CacheDirectoryname)
'                    End If
'                    Dispatcher.Invoke(New WriteA(AddressOf Write), "Lade " & Inputfile & " herunter" & Environment.NewLine)
'                    Try
'                        AddHandler wc_libraries.DownloadFileCompleted, AddressOf wc_libraries_DownloadFileCompleted
'                        wc_libraries.DownloadFileAsync(New Uri(Inputfile), CacheOutputfile)
'                    Catch ex As Exception
'                        Dispatcher.Invoke(New WriteA(AddressOf Write), "Fehler beim herunterladen von " & Inputfile & " :" & vbNewLine & ex.Message & vbNewLine)
'                    End Try
'                Else
'                    pb_download.Value += 1
'                    librariesDownloadindex += 1
'                    Download_Libraries()
'                End If
'            Catch ex As Exception
'                MessageBox.Show(ex.Message)
'            End Try
'        Else
'            MessageBox.Show(lb_libraries.Items.Count & librariesDownloadindex)
'            Unzip()
'        End If

'    End Sub

'    Private Sub wc_libraries_DownloadFileCompleted(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs) Handles wc_libraries.DownloadFileCompleted
'        Dim Inputfile As String = lb_libraries_url.Items.Item(librariesDownloadindex).ToString & lb_libraries.Items.Item(librariesDownloadindex).ToString
'        Dim Outputfile As String = mcpfad & "\libraries\" & lb_libraries.Items.Item(librariesDownloadindex).ToString
'        Dim CacheOutputfile As String = mcpfad & "\cache\" & lb_libraries.Items.Item(librariesDownloadindex).ToString
'        Dim Directoryname As String = IO.Path.GetDirectoryName(Outputfile)
'        Dim CacheDirectoryname As String = IO.Path.GetDirectoryName(CacheOutputfile)

'        If IO.Directory.Exists(Directoryname) = False Then
'            IO.Directory.CreateDirectory(Directoryname)
'        End If
'        Try
'            IO.File.Move(CacheOutputfile, Outputfile)
'        Catch
'        End Try
'        pb_download.Value += 1
'        librariesDownloadindex += 1
'        Download_Libraries()
'    End Sub

'    Sub Unzip()

'        Dispatcher.Invoke(New WriteA(AddressOf Write), "Natives werden entpackt" & Environment.NewLine)

'        UnpackDirectory = mcpfad & "\versions\" & lastversionID & "\" & lastversionID & "-natives"

'        For i = 0 To lb_libraries_extract.Items.Count - 1

'            Dim ZipToUnpack As String = librariespath & "\" & lb_libraries_extract.Items.Item(i).ToString
'            Dim Directoryname As String = IO.Path.GetDirectoryName(UnpackDirectory)

'            If IO.Directory.Exists(Directoryname) = False Then
'                IO.Directory.CreateDirectory(Directoryname)
'            End If

'            Using zip1 As ZipFile = ZipFile.Read(ZipToUnpack)
'                Dim e As ZipEntry
'                ' here, we extract every entry, but we could extract conditionally,
'                ' based on entry name, size, date, checkbox status, etc.   
'                For Each e In zip1
'                    Try 'Kein Zugriff, da diese MC Version bereits läuft
'                        e.Extract(UnpackDirectory, ExtractExistingFileAction.OverwriteSilently)
'                    Catch
'                    End Try
'                Next
'            End Using
'        Next

'        If IO.Directory.Exists(UnpackDirectory & "\META-INF") = True Then
'            IO.Directory.Delete(UnpackDirectory & "\META-INF", True)
'        End If

'        Finish_Start()

'    End Sub

'    Sub Get_Startinfos()
'        Dispatcher.Invoke(New WriteA(AddressOf Write), "Startinfos werden ausgelesen" & Environment.NewLine)
'        Versionsjar = mcpfad & "\versions\" & lastversionID & "\" & lastversionID & ".jar"
'        Dim o As String = IO.File.ReadAllText(versionsJSON)
'        Dim jo As JObject = JObject.Parse(o)
'        Dim mainClass As String = CStr((jo.SelectToken("mainClass")))
'        Dim minecraftArguments As String = CStr((jo.SelectToken("minecraftArguments")))
'        Dim libraries As String = Nothing
'        Dim gamedir As String

'        For i = 0 To lb_libraries.Items.Count - 1
'            Dim librarytemp As String = lb_libraries.Items.Item(i).ToString
'            libraries = libraries & librariespath & "\" & librarytemp.Replace("/", "\") & ";"
'        Next

'        If Profiles.gameDir(selectedprofile) <> Nothing Then
'            gamedir = Profiles.gameDir(selectedprofile)
'        Else
'            gamedir = mcpfad
'        End If

'        If IO.Directory.Exists(gamedir) = False Then
'            IO.Directory.CreateDirectory(gamedir)
'        End If

'        minecraftArguments = minecraftArguments.Replace("${auth_player_name}", tb_username.Text)
'        minecraftArguments = minecraftArguments.Replace("${version_name}", lastversionID)
'        minecraftArguments = minecraftArguments.Replace("${game_directory}", gamedir)
'        minecraftArguments = minecraftArguments.Replace("${game_assets}", assetspath)

'        Dim natives As String = UnpackDirectory

'        Dim javaargs As String
'        Dim height As String
'        Dim width As String

'        If Profiles.javaArgs(selectedprofile) <> Nothing Then
'            javaargs = Profiles.javaArgs(selectedprofile)
'        Else
'            'javaargs = "-Xmx" & RamCheck() & "M"
'            javaargs = "-Xmx" & "1024" & "M"
'        End If

'        If Profiles.resolution_height(selectedprofile) <> Nothing Then
'            height = " --height " & Profiles.resolution_height(selectedprofile)
'        Else
'            height = Nothing
'        End If

'        If Profiles.resolution_width(selectedprofile) <> Nothing Then
'            width = " --width " & Profiles.resolution_width(selectedprofile)
'        Else
'            width = Nothing
'        End If

'        Arguments = javaargs & " -Djava.library.path=" & natives & " -cp " & libraries & Versionsjar & " " & mainClass & " " & minecraftArguments & height & width

'        'MessageBox.Show(Arguments)

'        'StartArgumente und mainclass ... von JSON IO.File
'        'Überprüfen, ob username eingegeben wurde!
'        'Libraries zum Start hinzufügen!

'    End Sub

'    Public Sub StartMC()

'        If IsStarting = True Then
'            MessageBox.Show("Minecraft wird bereits gestartet!", MessageBoxStyle.Information, "Achtung")
'        ElseIf cb_profiles.SelectedIndex = -1 Then
'            MessageBox.Show("Wähle bitte ein Profil auswählen!", MessageBoxStyle.Information, "Achtung")
'        ElseIf tb_username.Text = Nothing Then
'            MessageBox.Show("Gib einen Usernamen ein!", MessageBoxStyle.Information, "Fehler")
'        Else

'            If Profiles.lastVersionId(selectedprofile) <> Nothing Then
'                lastversionID = Profiles.lastVersionId(selectedprofile)
'            Else
'                'Wenn snapshots aktiviert sind, dann index 0, sonst latestrelease
'                If Profiles.allowedReleaseTypes(selectedprofile).Count > 0 Then
'                    If Profiles.allowedReleaseTypes(selectedprofile).Contains("snapshot") = True Then
'                        'Version mit Index 0 auslesen
'                        lastversionID = Versions.latest
'                    Else
'                        lastversionID = Versions.latestrelease
'                    End If
'                Else
'                    lastversionID = Versions.latestrelease
'                End If
'            End If

'            tb_ausgabe.Clear()
'            IsStarting = True
'            Try
'                If IO.Directory.Exists(mcpfad & "\versions\" & lastversionID & "\" & lastversionID & "-natives") = True Then
'                    IO.Directory.Delete(mcpfad & "\versions\" & lastversionID & "\" & lastversionID & "-natives", True)
'                End If
'            Catch
'            End Try

'            Download_Resources()
'            pb_download.Maximum = 100
'            Download_Version()
'            Get_Libraries()
'        End If
'    End Sub

'    Sub Finish_Start()
'        Get_Startinfos()
'        Start_MC()
'        IsStarting = False
'        If lb_startedversions.Items.Contains(lastversionID) = False Then
'            lb_startedversions.Items.Add(lastversionID)
'        End If
'    End Sub

'    Private Sub btn_startMC_Click(sender As Object, e As RoutedEventArgs) Handles btn_startMC.Click
'        StartMC()
'    End Sub

'    Private Sub p_OutputDataReceived(ByVal sender As Object, ByVal e As DataReceivedEventArgs) Handles mc.OutputDataReceived
'        Try
'            Dispatcher.Invoke(New WriteA(AddressOf Write), e.Data & Environment.NewLine)
'        Catch ex As Exception
'        End Try
'    End Sub

'    Private Sub p_ErrorDataReceived(ByVal sender As Object, ByVal e As DataReceivedEventArgs) Handles mc.ErrorDataReceived
'        Try
'            Dispatcher.Invoke(New WriteA(AddressOf Write), e.Data & Environment.NewLine)
'        Catch ex As Exception
'        End Try
'    End Sub

'    Public Sub Write(ByVal Line As String)
'        'Wenn zu viel Text in der Textbox ist, dann den obersten löschen
'        If tb_ausgabe.LineCount >= 700 Then
'        End If

'        Me.Dispatcher.Invoke(
'    DispatcherPriority.Normal,
'    New Action(Sub()

'                   ' Do all the ui thread updates here
'                   'tb_ausgabe.Text &= Line
'                   tb_ausgabe.AppendText(Line)
'                   tb_ausgabe.ScrollToEnd()

'               End Sub))
'        'Log schreiben

'    End Sub

'    Public Shared Function CheckServerStatus() As Boolean
'        Dim MinecraftServer As New TcpClient
'        Try
'            MinecraftServer.Connect(serverip, serverport)
'            Return True
'        Catch Ex As Exception
'            Return False
'        End Try
'    End Function

'    'Public Shared Function JavaCheck() As String

'    '    If IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) & "\Java\jre7") = True Then
'    '        Return "64"
'    '    ElseIf IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) & "\Java\jre7") = True Then
'    '        Return "32"
'    '    Else
'    '        Return Nothing
'    '    End If

'    'End Function

'    'Public Shared Function JavaPath() As String
'    '    If JavaCheck() = "64" Then
'    '        Return "C:\Program Files\Java\jre7\bin\javaw.exe"
'    '    Else
'    '        Return "C:\Program Files (x86)\Java\jre7\bin\javaw.exe"
'    '    End If
'    'End Function

'    Public Shared Function Startcmd() As String
'        If Profiles.javaDir(selectedprofile) <> Nothing Then
'            Return Profiles.javaDir(selectedprofile)
'        Else
'            Return GetJavaPath()
'        End If
'    End Function

'    Public Shared Function GetJavaPath() As String
'        Dim reg As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall")
'        For Each item As String In reg.GetSubKeyNames

'            Dim regkey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall" & "\" & item)

'            Try 'Das Try, weil Fehler fliegen wenn die Value nichts ist. Kann man auch mit If lösen...
'                If regkey.GetValueNames.Contains("Contact") = True Then
'                    If regkey.GetValue("Contact").ToString = "http://java.com" Then
'                        If regkey.GetValueNames.Contains("InstallLocation") = True Then
'                            Return Path.Combine(regkey.GetValue("InstallLocation").ToString, "bin", "java.exe")
'                        End If
'                    End If
'                End If
'            Catch ex As Exception

'            End Try

'        Next
'        Return Nothing
'    End Function

'    Public Function RamCheck() As Integer
'        'If GetJavaPath() = "64" Then
'        Dim cbram_selecteditem As String = cb_ram.SelectedItem.ToString()
'        Dim ram_i As Integer = Val(cbram_selecteditem.First)
'        Return ram_i * 1024
'        'Else
'        'Return 1024
'        'End If
'    End Function

'    Private Sub wcversionsdownload_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles wcversionsdownload.DownloadProgressChanged
'        pb_download.Value = e.ProgressPercentage
'    End Sub

'    Private Sub tb_ausgabe_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tb_ausgabe.TextChanged
'        tb_ausgabe.ScrollToEnd()
'    End Sub

'    Private Sub TabControl_main_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles TabControl_main.SelectionChanged
'        tb_ausgabe.ScrollToEnd()
'    End Sub

'    Private Sub btn_new_profile_Click(sender As Object, e As RoutedEventArgs) Handles btn_new_profile.Click
'        Dim result As Boolean?
'        Newprofile = True
'        Dim frm_ProfileEditor As New ProfileEditor
'        Dim Profiles As New Profiles
'        frm_ProfileEditor.ShowDialog()
'        result = frm_ProfileEditor.DialogResult
'        If result = True Then
'            Get_Profiles()
'        End If

'    End Sub

'    Private Sub btn_edit_profile_Click(sender As Object, e As RoutedEventArgs) Handles btn_edit_profile.Click
'        Dim result As Boolean?
'        Newprofile = False
'        Dim frm_ProfileEditor As New ProfileEditor
'        Dim Profiles As New Profiles
'        frm_ProfileEditor.ShowDialog()
'        result = frm_ProfileEditor.DialogResult
'        If result = True Then
'            Get_Profiles()
'        End If
'    End Sub

'    Private Sub cb_profiles_DropDownClosed(sender As Object, e As EventArgs) Handles cb_profiles.DropDownClosed
'        Get_Profiles()
'    End Sub

'    Private Sub cb_profiles_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cb_profiles.SelectionChanged
'        Try
'            selectedprofile = cb_profiles.SelectedItem.ToString
'            Dim o As String = IO.File.ReadAllText(launcher_profiles_json)
'            Dim jo As JObject = JObject.Parse(o)
'            jo("selectedProfile") = selectedprofile
'            IO.File.WriteAllText(launcher_profiles_json, jo.ToString)

'        Catch
'        End Try
'    End Sub

'    Private Sub btn_delete_profile_Click(sender As Object, e As RoutedEventArgs) Handles btn_delete_profile.Click
'        If cb_profiles.Items.Count > 1 Then
'            Profiles.Remove(cb_profiles.SelectedItem.ToString)
'            cb_profiles.SelectedIndex = 0
'            Get_Profiles()
'        Else
'            System.Windows.MessageBox.Show("Das letzte Profil kann nicht gelöscht werden!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Exclamation)
'        End If
'    End Sub

'    Private Sub btn_downloadmod_Click(sender As Object, e As RoutedEventArgs) Handles btn_downloadmod.Click
'        If moddownloading = True Then
'            MessageBox.Show("Eine Mod wird bereits heruntergeladen. Warte bitte, bis diese fertig ist!", "Download läuft", MessageBoxButton.OK, MessageBoxImage.Information)
'        Else
'            btn_resetmodsfoler.IsEnabled = False
'            btn_selectmodsfolder.IsEnabled = False
'            btn_refresh.IsEnabled = False
'            btn_downloadmod.IsEnabled = False
'            moddownloading = True
'            lbl_mods_status.Content = Nothing
'            downloadlist.Clear()
'            For Each selectedmod As ForgeMod In lb_mods.SelectedItems
'                downloadlist.Add(selectedmod)
'                For Each item As String In Mods.All_Needed_Mods(selectedmod.name, cb_modversions.SelectedItem.ToString)
'                    Dim moditem As ForgeMod = modslist.Where(Function(p) p.name = item).First
'                    If downloadlist.Contains(moditem) = False Then
'                        downloadlist.Add(moditem)
'                    End If
'                Next
'            Next
'            downloadindex = 0
'            download_mod()
'        End If
'    End Sub

'    Private Sub download_mod()
'        If downloadindex < downloadlist.Count Then
'            If tb_modsfolder.Text.Contains(IO.Path.GetInvalidPathChars) = True Then
'                MessageBox.Show("Der Pfad des Mods Ordner enthält ungültige Zeichen", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error)
'            Else
'                Dim Version As String = downloadlist.Item(downloadindex).version
'                Dim path As String = tb_modsfolder.Text
'                Dim name As String = downloadlist.Item(downloadindex).name
'                Dim url As New Uri(downloadlist.Item(downloadindex).downloadlink)
'                lbl_mods_status.Content = downloadindex + 1 & " / " & downloadlist.Count & " " & name
'                If Version >= "1.6.4" = True Then
'                    Modsfilename = Version & "\" & downloadlist.Item(downloadindex).id & "." & downloadlist.Item(downloadindex).extension
'                Else
'                    Modsfilename = Version & "-" & downloadlist.Item(downloadindex).id & "." & downloadlist.Item(downloadindex).extension
'                End If
'                'If url.Host = "mega.co.nz" Then
'                '    'AddHandler Megalib.DownloadProgress, AddressOf DownloadProgress
'                '    'AddHandler Megalib.DownloadFinished, AddressOf DownloadModFinished
'                '    'Megalib.download_url(Mods.downloadlinkAt(Version, Mods.Name(name, Version)), path)
'                'Else
'                Try
'                    If IO.Directory.Exists(IO.Path.GetDirectoryName(cachefolder & "\" & Modsfilename)) = False Then
'                        IO.Directory.CreateDirectory((IO.Path.GetDirectoryName(cachefolder & "\" & Modsfilename)))
'                    End If
'                    wcmod.DownloadFileAsync(url, cachefolder & "\" & Modsfilename)
'                Catch ex As Exception
'                    lbl_mods_status.Content = ex.Message
'                    MessageBox.Show(ex.Message)
'                    Mod_Download_finished()
'                    Exit Sub
'                End Try
'                'End If
'                downloadindex += 1
'                Dim selected As Integer = lb_mods.SelectedIndex
'                Load_Mods()
'                lb_mods.SelectedIndex = selected
'            End If
'        Else
'            lbl_mods_status.Content = "Erfolgreich installiert"
'            Mod_Download_finished()
'        End If
'    End Sub

'    Private Sub wcmod_DownloadFileCompleted(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs) Handles wcmod.DownloadFileCompleted
'        If e.Cancelled = True Then
'            lbl_mods_status.Content = e.Error
'            MessageBox.Show(e.Error)
'            Mod_Download_finished()
'        Else
'            Try
'                Dim path As String = tb_modsfolder.Text & "\" & Modsfilename
'                If IO.Directory.Exists(IO.Path.GetDirectoryName(path)) = False Then
'                    IO.Directory.CreateDirectory((IO.Path.GetDirectoryName(path)))
'                End If
'                My.Computer.FileSystem.MoveFile(cachefolder & "\" & Modsfilename, path)
'            Catch
'            End Try
'            download_mod()
'        End If
'    End Sub

'    Private Sub Mod_Download_finished()
'        moddownloading = False
'        btn_resetmodsfoler.IsEnabled = True
'        btn_selectmodsfolder.IsEnabled = True
'        btn_refresh.IsEnabled = True
'        btn_downloadmod.IsEnabled = True
'        Dim selected As Integer = lb_mods.SelectedIndex
'        Load_Mods()
'        lb_mods.SelectedIndex = selected
'    End Sub

'    Public Sub Load_ModVersions()
'        cb_modversions.Items.Clear()
'        Dim modversionslist As IList(Of String) = Mods.Get_ModVersions
'        For Each Modversion As String In modversionslist
'            cb_modversions.Items.Add(Modversion)
'        Next
'        cb_modversions.SelectedIndex = 0
'    End Sub

'    Public Sub Load_Mods()
'        Try
'            lb_mods.Items.Clear()
'            modslist = Mods.Get_Mods(cb_modversions.SelectedItem.ToString, tb_modsfolder.Text)
'            Filter_Mods()
'        Catch
'        End Try
'    End Sub

'    Private Sub Filter_Mods()
'        lb_mods.Items.Clear()
'        For Each Moditem As ForgeMod In modslist
'            If Moditem.name.ToLower.Contains(tb_search_mods.Text.ToLower) = True Then
'                lb_mods.Items.Add(Moditem)
'            End If
'        Next
'        If lb_mods.Items.Count >= 1 Then
'            lb_mods.SelectedIndex = 0
'        End If
'    End Sub

'    Private Sub cb_modversions_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cb_modversions.SelectionChanged
'        Load_Mods()
'    End Sub

'    Private Sub lb_mods_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lb_mods.SelectionChanged
'        'Load_Modinfos
'        If lb_mods.SelectedIndex <> -1 Then
'            'If DirectCast(lb_mods.SelectedItem, ForgeMod).installed = True Then
'            '    Dim c As New ImageSourceConverter()
'            '    img_installed.Source = CType(c.ConvertFrom(My.Resources.check_green), ImageSource)
'            'Else
'            '    img_installed.Source = Nothing
'            'End If
'            If lb_mods.SelectedItems.Count > 1 Then
'                btn_downloadmod.Content = lb_mods.SelectedItems.Count & " Installieren"
'            Else
'                btn_downloadmod.Content = "Installieren"
'            End If
'            lbl_name.Content = DirectCast(lb_mods.SelectedItem, ForgeMod).name
'            lbl_autor.Content = DirectCast(lb_mods.SelectedItem, ForgeMod).autor
'            tb_description.Text = DirectCast(lb_mods.SelectedItem, ForgeMod).description
'            If DirectCast(lb_mods.SelectedItem, ForgeMod).installed = True Then
'                img_installed.Visibility = Windows.Visibility.Visible
'                btn_list_delete_mod.IsEnabled = True
'            Else
'                btn_list_delete_mod.IsEnabled = False
'                img_installed.Visibility = Windows.Visibility.Hidden
'            End If
'        End If


'        'If lb_mods.SelectedIndex <> -1 Then
'        '    lbl_name.Content = Mods.NameAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
'        '    tb_description.Text = Mods.descriptionAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
'        '    mod_website = Mods.websiteAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
'        '    mod_video = Mods.videoAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
'        '    mod_downloadlink = Mods.downloadlinkAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
'        'End If
'    End Sub

'    'Public Sub DownloadSongStarted(ByVal sender As Object, ByVal e As MegaLibrary.Mega.DownloadStartedEventArgs)

'    '    StopPlayback()
'    '    Dim source As IWaveSource = Nothing
'    '    Try
'    '        source = CodecFactory.Instance.GetCodec(modsfolder & "\" & e.filename)

'    '    Catch ex As Exception
'    '        MessageBox.Show("Dateityp wird nicht unterstützt.")
'    '        Return
'    '    End Try

'    '    soundSource = source

'    '    soundOut = New DirectSoundOut()
'    '    soundOut.Initialize(source)
'    '    soundOut.Play()

'    'End Sub

'    'Private Sub StopPlayback()
'    '    If soundOut IsNot Nothing Then
'    '        soundOut.Stop()
'    '        soundOut.Dispose()
'    '        soundOut = Nothing
'    '    End If
'    '    If soundSource IsNot Nothing Then
'    '        soundSource.Dispose()
'    '    End If
'    'End Sub

'    'Private Sub PlayButton_Click(sender As Object, e As EventArgs) Handles btn_play.Click
'    '    If moddownloading = True Then
'    '        MessageBox.Show("Ein Song wird bereits heruntergeladen. Warte bitte, bis diese fertig ist!", "Download läuft", MessageBoxButton.OK, MessageBoxImage.Information)
'    '    Else
'    '        moddownloading = True
'    '        pb_mods_download.Value = 0
'    '        AddHandler Megalib.DownloadProgress, AddressOf DownloadProgress
'    '        AddHandler Megalib.DownloadFinished, AddressOf DownloadModFinished
'    '        'AddHandler Megalib.DownloadStarted, AddressOf DownloadSongStarted
'    '        Megalib.download_url(tb_downloadlink.Text, modsfolder)
'    '    End If

'    'End Sub

'    Private Sub wcmod_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles wcmod.DownloadProgressChanged
'        ' Do all the ui thread updates here
'        Me.Dispatcher.Invoke(
'            DispatcherPriority.Normal,
'            New Action(Sub()

'                           ' Do all the ui thread updates here
'                           pb_mods_download.Value = e.ProgressPercentage

'                       End Sub))
'    End Sub

'    Private Sub btn_website_Click(sender As Object, e As RoutedEventArgs) Handles btn_website.Click
'        Process.Start(DirectCast(lb_mods.SelectedItem, ForgeMod).website)
'    End Sub

'    Private Sub btn_video_Click(sender As Object, e As RoutedEventArgs) Handles btn_video.Click
'        Process.Start(DirectCast(lb_mods.SelectedItem, ForgeMod).video)
'    End Sub

'    Private Sub btn_installforge_Click(sender As Object, e As RoutedEventArgs) Handles btn_installforge.Click
'        Dim frg As New Forge_installer
'        frg.ShowDialog()
'    End Sub

'    Private Sub tb_username_KeyDown(sender As Object, e As Input.KeyEventArgs) Handles tb_username.KeyDown
'        If e.Key = Key.Enter Or e.Key = Key.Return Then
'            'Deine Aktionen
'            StartMC()
'        End If
'    End Sub

'    Private Sub tb_username_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tb_username.TextChanged
'        Try
'            Settings.Username = tb_username.Text
'            Settings.Save()
'        Catch
'        End Try
'        'My.Settings.Username = tb_username.Text.ToString
'        'My.Settings.Save()
'    End Sub

'    Private Sub btn_resetmodsfoler_Click(sender As Object, e As RoutedEventArgs) Handles btn_resetmodsfoler.Click
'        tb_modsfolder.Text = modsfolder
'    End Sub

'    Private Sub btn_selectmodsfolder_Click(sender As Object, e As RoutedEventArgs) Handles btn_selectmodsfolder.Click
'        Dim fd As New VistaFolderBrowserDialog
'        fd.UseDescriptionForTitle = True
'        fd.Description = "Mods Ordner auswählen"
'        fd.RootFolder = Environment.SpecialFolder.MyComputer
'        fd.SelectedPath = modsfolder
'        fd.ShowNewFolderButton = True
'        If fd.ShowDialog = True Then
'            tb_modsfolder.Text = fd.SelectedPath
'        End If
'    End Sub

'    Private Sub RefreshMods()
'        'For Each item As ForgeMod In modslist
'        '    Dim Struktur As String
'        '    Dim Version As String
'        '    Version = cb_versions.SelectedItem.ToString
'        '    If Version >= "1.6.4" = True Then
'        '        Struktur = Version & "\" & item.id & "." & item.extension
'        '    Else
'        '        Struktur = Version & "-" & item.id & "." & item.extension
'        '    End If
'        '    If File.Exists(modsfolder & "\" & Struktur) = True Then
'        '        item.installed = True
'        '    Else
'        '        item.installed = False
'        '    End If
'        'Next

'        'For Each item As ForgeMod In lb_mods.Items
'        '    Dim Struktur As String
'        '    Dim Version As String
'        '    Version = cb_versions.SelectedItem.ToString
'        '    If Version >= "1.6.4" = True Then
'        '        Struktur = Version & "\" & item.id & "." & item.extension
'        '    Else
'        '        Struktur = Version & "-" & item.id & "." & item.extension
'        '    End If
'        '    If File.Exists(modsfolder & "\" & Struktur) = True Then
'        '        item.installed = True
'        '    Else
'        '        item.installed = False
'        '    End If
'        'Next
'        Try
'            Dim selectedversion As String = cb_modversions.SelectedItem.ToString
'            Dim selectedmod As Integer = lb_mods.SelectedIndex
'            Dim SelectedItems As IList = lb_mods.SelectedItems
'            Load_ModVersions()
'            'Funktioniert nicht
'            For Each item In SelectedItems
'                lb_mods.SelectedItems.Add(item)
'            Next
'            cb_modversions.SelectedItem = selectedversion
'            lb_mods.SelectedIndex = selectedmod
'        Catch
'        End Try
'    End Sub

'    Private Sub tb_search_mods_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tb_search_mods.TextChanged
'        Filter_Mods()
'    End Sub

'    Private Sub btn_list_delete_mod_Click(sender As Object, e As RoutedEventArgs) Handles btn_list_delete_mod.Click
'        Dim Version As String = DirectCast(lb_mods.SelectedItem, ForgeMod).version
'        Dim Struktur As String
'        If Version >= "1.6.4" = True Then
'            Struktur = Version & "\" & DirectCast(lb_mods.SelectedItem, ForgeMod).id & "." & DirectCast(lb_mods.SelectedItem, ForgeMod).extension
'        Else
'            Struktur = Version & "-" & DirectCast(lb_mods.SelectedItem, ForgeMod).id & "." & DirectCast(lb_mods.SelectedItem, ForgeMod).extension
'        End If
'        If File.Exists(tb_modsfolder.Text & "\" & Struktur) = True Then
'            File.Delete(tb_modsfolder.Text & "\" & Struktur)
'        End If
'        Dim selected As Integer = lb_mods.SelectedIndex
'        Load_Mods()
'        lb_mods.SelectedIndex = selected
'    End Sub

'    Private Sub tb_modsfolder_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tb_modsfolder.TextChanged
'        RefreshMods()
'    End Sub

'End Class