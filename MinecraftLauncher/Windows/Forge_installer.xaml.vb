Imports System.Net
Imports Ionic.Zip
Imports MahApps.Metro.Controls.Dialogs
Imports MahApps.Metro
Imports System.IO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports McMetroLauncher.Forge

Class Forge_installer
    WithEvents wc As New WebClient
    Dim wcauto As New WebClient
    Private filename As String
    Dim build As ForgeBuild
    Dim stripmeta As Boolean = False
    Dim liblist As List(Of Library) = New List(Of Library)
    Dim libdownloadindex As Integer
    Dim libpath As FileInfo
    Dim controller As ProgressDialogController

    Public Sub Load_Forge()
        lst.Items.Clear()
        Dim list As IList(Of Forge.ForgeBuild) = New List(Of Forge.ForgeBuild)
        For Each item As Forge.ForgeBuild In Forge.ForgeList
            Dim lstitems As IList(Of String) = list.Select(Function(p) p.version).ToList
            If item.files.Select(Function(o) o.type).Contains("universal") AndAlso lstitems.Contains(item.version) = False Then
                list.Add(item)
            End If
        Next
        list = list.OrderByDescending(Function(p) p.build).ToList
        For Each item As Forge.ForgeBuild In list
            lst.Items.Add(item)
        Next
    End Sub

    Private Sub Forge_installer_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        Profiles.Get_Profiles()
    End Sub

    Private Sub ForgeManager_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Try
            Load_Forge()
            tb_mcpfad.Text = mcpfad.FullName
        Catch Ex As Exception
            MsgBox(Ex.Message & Environment.NewLine & Ex.StackTrace)
        End Try
    End Sub

    Private Async Sub btn_download_Click(sender As Object, e As RoutedEventArgs) Handles btn_download.Click
        If lst.SelectedIndex = -1 Then
            Await Me.ShowMessageAsync(Nothing, Application.Current.FindResource("ChooseForgeVersion").ToString, MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = Application.Current.FindResource("OK").ToString, .ColorScheme = MetroDialogColorScheme.Accented})
        ElseIf wc.IsBusy = True Then
            wc.CancelAsync()
        Else
            btn_download_auto.IsEnabled = False
            pb_download.IsIndeterminate = False
            forge_instructions.IsSelected = True
            Dim Legacyforgefile As Boolean = False
            If Forge.LegacyBuildList.Select(Function(p) p.version).Contains(build.version) Then
                Legacyforgefile = True
            End If
            Dim url As New Uri(String.Format("http://files.minecraftforge.net/maven/net/minecraftforge/forge/{1}-{0}{2}/forge-{1}-{0}{2}-installer.jar", build.version, build.mcversion, IIf(build.branch = Nothing, "", "-" & build.branch).ToString))
            If Legacyforgefile = True Then
                url = New Uri(String.Format("http://files.minecraftforge.net/minecraftforge/minecraftforge-installer-{1}-{0}.jar", build.version, build.mcversion))
            End If
            filename = IO.Path.Combine(cachefolder.FullName, String.Format("forge-{0}-{1}-installer.jar", build.mcversion, build.version))
            wc.DownloadFileAsync(url, filename)
            ForgeInstallerViewModel.Instance.installerdownloading = True
        End If
    End Sub

    Private Sub btn_copy_Click(sender As Object, e As RoutedEventArgs) Handles btn_copy.Click
        Clipboard.SetText(tb_mcpfad.Text)
    End Sub

    Private Sub wc_DownloadFileCompleted(sender As Object, e As ComponentModel.AsyncCompletedEventArgs) Handles wc.DownloadFileCompleted
        If e.Cancelled = True Then
            IO.File.Delete(filename)
        Else
            pb_download.IsIndeterminate = True
            Process.Start(filename)
        End If
        btn_download_auto.IsEnabled = True
        ForgeInstallerViewModel.Instance.installerdownloading = False
    End Sub

    Private Sub wc_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles wc.DownloadProgressChanged
        pb_download.Value = e.ProgressPercentage
    End Sub

    Private Async Sub btn_download_auto_Click(sender As Object, e As RoutedEventArgs) Handles btn_download_auto.Click
        Dim branch As String = IIf(build.branch = Nothing, "", "-" & build.branch).ToString
        Try
            If lst.SelectedIndex = -1 Then
                Await Me.ShowMessageAsync(Nothing, Application.Current.FindResource("ChooseForgeVersion").ToString, MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = Application.Current.FindResource("OK").ToString, .ColorScheme = MetroDialogColorScheme.Accented})
            Else
                stripmeta = False
                controller = Await Me.ShowProgressAsync(Application.Current.FindResource("InstallingForge").ToString, Application.Current.FindResource("PleaseWait").ToString, False, New MetroDialogSettings() With {.ColorScheme = MetroDialogColorScheme.Theme})
                Dim Legacyforge As Boolean = False
                If Forge.LegacyBuildList.Select(Function(p) p.version).Contains(build.version) Then
                    Legacyforge = True
                End If
                'Download Universal
                Dim extension As String = Forge.ForgeList.Where(Function(p) p.version = build.version).First.files.Where(Function(p) p.type = "universal").First.extension
                If extension = "zip" Then
                    stripmeta = True
                End If
                Dim url As New Uri(String.Format("http://files.minecraftforge.net/maven/net/minecraftforge/forge/{1}-{0}{2}/forge-{1}-{0}{2}-universal.{3}", build.version, build.mcversion, branch, extension))
                If Legacyforge = True Then
                    url = New Uri(String.Format("http://files.minecraftforge.net/minecraftforge/minecraftforge-universal-{1}-{0}.{2}", build.version, build.mcversion, extension))
                End If
                filename = IO.Path.Combine(cachefolder.FullName, String.Format("forge-{0}-{1}{2}-universal.jar", build.mcversion, build.version, branch))
                wcauto.DownloadFileAsync(url, filename)
                AddHandler wcauto.DownloadFileCompleted, AddressOf Install_Version
                AddHandler wcauto.DownloadProgressChanged, Sub(sender2 As Object, e2 As Net.DownloadProgressChangedEventArgs)
                                                               Dim progress As Double = e2.ProgressPercentage / 100 / 3
                                                               If progress <= 1 Then
                                                                   controller.SetProgress(progress)
                                                               End If
                                                           End Sub

            End If
        Catch ex As Exception
            MsgBox(ex.Message & Environment.NewLine & ex.StackTrace)
            controller.CloseAsync()
        End Try
    End Sub
    Async Sub Install_Version(sender As Object, e As ComponentModel.AsyncCompletedEventArgs)
        Try
            Dim branch As String = IIf(build.branch = Nothing, "", "-" & build.branch).ToString
            Dim versionsname As String = build.mcversion & "-Forge" & build.version & branch
            If e.Cancelled = False AndAlso e.Error Is Nothing Then
                Dim legacyinstallation As Boolean = False
                'Extract version.json and copy to versionsfolder
                Try
                    Using zip1 As ZipFile = ZipFile.Read(filename)
                        If zip1.Entries.Select(Function(p) p.FileName).Contains("version.json") Then
                            ' here, we extract every entry, but we could extract conditionally,
                            ' based on entry name, size, date, checkbox status, etc.   
                            For Each i As ZipEntry In zip1
                                If i.FileName = "version.json" Then
                                    i.Extract(IO.Path.GetDirectoryName(filename), ExtractExistingFileAction.OverwriteSilently)
                                    With New FileInfo(Path.Combine(versionsfolder.FullName, versionsname, versionsname & ".json"))
                                        If .Directory.Exists = False Then
                                            .Directory.Create()
                                        End If
                                        File.Copy(Path.Combine(Path.GetDirectoryName(filename), "version.json"), .FullName, True)
                                        Exit For
                                    End With
                                End If
                            Next
                        Else
                            legacyinstallation = True
                        End If
                    End Using
                Catch ex As ZipException
                    'ShowError
                    'Write("Fehler beim entpacken der natives: " & ex.Message, LogLevel.ERROR)
                End Try
                'Copy verisons.jar and download if doesn't exist
                With New FileInfo(Path.Combine(versionsfolder.FullName, build.mcversion, build.mcversion & ".jar"))
                    Dim jsonfile As New FileInfo(Path.Combine(versionsfolder.FullName, build.mcversion, build.mcversion & ".json"))
                    Dim output As String = .FullName
                    Dim jsonoutput As String = jsonfile.FullName
                    Dim VersionsURl As String = "https://s3.amazonaws.com/Minecraft.Download/versions/" & build.mcversion & "/" & build.mcversion
                    Try
                        If .Exists = False Then
                            'Download
                            wcauto = New WebClient
                            controller.SetMessage(Application.Current.FindResource("DownloadingVersion").ToString)
                            AddHandler wcauto.DownloadProgressChanged, Sub(sender2 As Object, e2 As Net.DownloadProgressChangedEventArgs)
                                                                           Dim progress As Double = 1 / 3 + e2.ProgressPercentage / 100 / 3
                                                                           If progress <= 1 Then
                                                                               controller.SetProgress(progress)
                                                                           End If
                                                                       End Sub
                            If .Directory.Exists = False Then
                                .Directory.Create()
                            End If
                            Await wcauto.DownloadFileTaskAsync(New Uri(VersionsURl & ".jar"), output)
                        End If
                        If jsonfile.Exists = False Then
                            If .Directory.Exists = False Then
                                .Directory.Create()
                            End If
                            Await New WebClient().DownloadFileTaskAsync(New Uri(VersionsURl & ".json"), jsonoutput)
                        End If
                    Catch ex As Exception
                        'Show Error
                    End Try
                    'Copy
                    controller.SetMessage(Application.Current.FindResource("InstallingVersion").ToString)
                    Dim targetpath As New FileInfo(Path.Combine(versionsfolder.FullName, versionsname, versionsname & ".jar"))
                    If targetpath.Directory.Exists = False Then
                        targetpath.Directory.Create()
                    End If
                    If stripmeta = True Then
                        CopyandStrip(New FileInfo(output), targetpath)
                    Else
                        File.Copy(output, targetpath.FullName, True)
                    End If
                    If legacyinstallation Then
                        'Patch jar with Forge:
                        Copytojar(New FileInfo(filename), targetpath)
                        'Copy json and change the id:
                        Dim targetoutput As String = Path.Combine(versionsfolder.FullName, versionsname, versionsname & ".json")
                        File.Copy(jsonoutput, targetoutput, True)
                        Dim text As String = File.ReadAllText(targetoutput)
                        Dim versioninfo As VersionInfo = Await JsonConvert.DeserializeObjectAsync(Of VersionInfo)(text)
                        versioninfo.id = versionsname
                        File.WriteAllText(targetoutput, Await JsonConvert.SerializeObjectAsync(versioninfo, Formatting.Indented))
                    End If
                    controller.SetProgress(2 / 3)
                    'Copy universal to corresponding library
                    Dim o As String = File.ReadAllText(Path.Combine(versionsfolder.FullName, versionsname, versionsname & ".json"))
                    'Converter because some Forge Versions(10.12.0.1054) has Double as minimumlauncherversion, expected integer. It rounds the double to an integer
                    Dim Versionsinfos As VersionInfo = Await JsonConvert.DeserializeObjectAsync(Of VersionInfo)(o, New JsonSerializerSettings() With {.NullValueHandling = NullValueHandling.Ignore, .Converters = New List(Of JsonConverter) From {New CustomIntConverter()}})
                    Dim libpath As String = Nothing
                    If Versionsinfos.libraries.Select(Function(p) p.name.Split(CChar(":"))(1)).Contains("forge") Then
                        libpath = String.Format(Path.Combine(librariesfolder.FullName, "net", "minecraftforge", "forge\{1}-{0}{2}\forge-{1}-{0}{2}.jar"), build.version, build.mcversion, branch)
                    Else
                        libpath = String.Format(Path.Combine(librariesfolder.FullName, "net", "minecraftforge", "{1}\{0}\{1}-{0}.jar"), build.version, "minecraftforge")
                    End If
                    With New FileInfo(libpath)
                        If .Directory.Exists = False Then
                            .Directory.Create()
                        End If
                        File.Copy(filename, .FullName, True)
                    End With
                    'Download other Libraries
                    liblist.Clear()
                    For Each item As Library In Versionsinfos.libraries.Where(Function(p) p.url <> Nothing)
                        If item.url.Contains("files.minecraftforge.net") AndAlso item.name.Split(CChar(":"))(1) <> "forge" AndAlso item.name.Split(CChar(":"))(1) <> "minecraftforge" Then
                            liblist.Add(item)
                        End If
                    Next
                    libdownloadindex = 0
                    Await Downloadlibs()
                End With
            ElseIf e.Error IsNot Nothing Then
                controller.SetMessage(Application.Current.FindResource("ErrorDownloading").ToString & ": " & e.Error.Message & Environment.NewLine & libpath.FullName)
                controller.SetCancelable(True)
                While controller.IsCanceled = False
                    Await Task.Delay(10)
                End While
                Await controller.CloseAsync()
            End If
        Catch ex As Exception
            MsgBox(ex.Message & Environment.NewLine & ex.StackTrace)
            controller.CloseAsync()
        End Try
    End Sub

    Async Function Downloadlibs() As Task
        If build.mcversion = "1.5.2" Then
            controller.SetMessage(Application.Current.FindResource("DownloadingRequiredLibraries").ToString)
            controller.SetIndeterminate()
            Try
                Await New WebClient().DownloadFileTaskAsync("http://files.minecraftforge.net/fmllibs/bcprov-jdk15on-148.jar.stash", Path.Combine(mcpfad.FullName, "lib", "bcprov-jdk15on-148.jar"))
                Await New WebClient().DownloadFileTaskAsync("http://files.minecraftforge.net/fmllibs/scala-library.jar.stash", Path.Combine(mcpfad.FullName, "lib", "scala-library.jar"))
            Catch ex As Exception
                MsgBox(ex.Message & Environment.NewLine & ex.StackTrace)
                File.Delete(Path.Combine(mcpfad.FullName, "lib", "bcprov-jdk15on-148.jar"))
                File.Delete(Path.Combine(mcpfad.FullName, "lib", "scala-library.jar"))
                controller.CloseAsync()
            End Try
        End If


        If libdownloadindex < liblist.Count Then
            Using a As New WebClient
                With liblist.ElementAt(libdownloadindex)
                    libpath = New FileInfo(IO.Path.Combine(librariesfolder.FullName, .path) & ".pack.xz")
                    Dim customurl As String = .url
                    Dim url As String = customurl & .path & ".pack.xz"
                    controller.SetMessage(Application.Current.FindResource("DownloadingLibrary").ToString & ": " & libpath.FullName)
                    If Not libpath.Directory.Exists Then
                        libpath.Directory.Create()
                    End If
                    a.DownloadFileAsync(New Uri(url), libpath.FullName)
                    AddHandler a.DownloadFileCompleted, AddressOf Downloadlibcompleted
                    AddHandler a.DownloadProgressChanged, Sub(sender2 As Object, e2 As Net.DownloadProgressChangedEventArgs)
                                                              Dim progress As Double = 2 / 3 + (libdownloadindex + e2.ProgressPercentage / 100) / 3 / liblist.Count
                                                              If progress <= 1 Then
                                                                  controller.SetProgress(progress)
                                                              End If
                                                          End Sub
                End With
            End Using
        Else
            'Install Finished
            Await controller.CloseAsync()
            Me.Hide()
            Dim branch As String = IIf(build.branch = Nothing, "", "-" & build.branch).ToString
            Dim profileedit As New Forge_ProfileEditor With {.Versionname = build.mcversion & "-Forge" & build.version & branch}
            profileedit.ShowDialog()
            Me.Close()
        End If
    End Function

    Sub CopyandStrip(sourcejar As FileInfo, targetjar As FileInfo)
        Dim input As ZipFile = New ZipFile(sourcejar.FullName)
        Dim output As ZipFile = New ZipFile
        Using fs As New FileStream(targetjar.FullName, FileMode.Create, FileAccess.ReadWrite)
            For Each e As ZipEntry In input.Entries
                If e.IsDirectory Then
                    Dim ms As New MemoryStream
                    e.Extract(ms)
                    ms.Seek(0, SeekOrigin.Begin)
                    output.AddEntry(e.FileName, ms)
                ElseIf e.FileName.StartsWith("META-INF") Then

                Else
                    Dim n As ZipEntry = e
                    n.SetEntryTimes(e.CreationTime, e.AccessedTime, e.ModifiedTime)
                    Dim ms As New MemoryStream
                    n.Extract(ms)
                    ms.Seek(0, SeekOrigin.Begin)
                    output.AddEntry(n.FileName, ms)
                End If
            Next
            output.Save(fs)
            fs.Close()
        End Using
    End Sub


    Sub Copytojar(sourcejar As FileInfo, targetjar As FileInfo)
        Dim input As ZipFile = ZipFile.Read(sourcejar.FullName)
        Dim output As ZipFile = ZipFile.Read(targetjar.FullName)
        For Each e As ZipEntry In input.Entries
            If e.IsDirectory = False Then
                Dim ms As New MemoryStream
                If output.Entries.Select(Function(p) p.FileName).Contains(e.FileName) Then
                    output.RemoveEntry(e.FileName)
                End If
                e.Extract(ms)
                ms.Seek(0, SeekOrigin.Begin)
                output.AddEntry(e.FileName, ms)
            End If
        Next
        output.Save()
    End Sub

    Async Sub Downloadlibcompleted(sender As Object, e As ComponentModel.AsyncCompletedEventArgs)
        If e.Cancelled = False AndAlso e.Error Is Nothing Then
            controller.SetMessage(Application.Current.FindResource("UnpackingLibrary").ToString & ": " & libpath.FullName)
            If Await Unpack.Unpack(libpath, libpath) = False Then
                controller.SetMessage(Application.Current.FindResource("ErrorUnpacking").ToString & ": " & libpath.FullName)
                controller.SetCancelable(True)
                While controller.IsCanceled = False
                    Await Task.Delay(10)
                End While
                Await controller.CloseAsync()
            Else
                libdownloadindex += 1
                Await Downloadlibs()
            End If
            'libpath.Delete()
        ElseIf e.Error IsNot Nothing Then
            controller.SetMessage(Application.Current.FindResource("ErrorDownloading").ToString & ": " & e.Error.Message & Environment.NewLine & libpath.FullName)
            controller.SetCancelable(True)
            While controller.IsCanceled = False
                Await Task.Delay(10)
            End While
            Await controller.CloseAsync()
        End If
    End Sub

    Private Sub lst_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lst.SelectionChanged
        build = DirectCast(lst.SelectedItem, Forge.ForgeBuild)
        If DirectCast(lst.SelectedItem, Forge.ForgeBuild).files.Select(Function(p) p.type).Contains("universal") Then
            If build.mcversion <> "1.6.1" AndAlso build.mcversion <> "1.6.2" Then
                btn_download_auto.IsEnabled = True
            Else
                btn_download_auto.IsEnabled = False
            End If
        Else
            btn_download_auto.IsEnabled = False
        End If
        If DirectCast(lst.SelectedItem, Forge.ForgeBuild).files.Select(Function(p) p.type).Contains("installer") Then
            btn_download.IsEnabled = True
        Else
            btn_download.IsEnabled = False
        End If
    End Sub
End Class
