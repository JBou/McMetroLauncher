Imports System.Net
Imports Ionic.Zip

Class Forge_installer
    Private List As New List(Of ForgeEintrag)
    WithEvents wc As New WebClient
    Private filename As String
    Private profilename As String
    Private version As String
    Dim build As String

    Public Sub Load_Forge()
        lst.Items.Clear()
        List.Clear()
        For Each item As ForgeEintrag In Forge.Get_Forge
            'MessageBox.Show(String.Join(" | ", item.build, item.version, item.time, item.downloadLink))
            lst.Items.Add(item)
            List.Add(item)
        Next
    End Sub

    Private Sub ForgeManager_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Load_Forge()
    End Sub

    Private Sub btn_download_Click(sender As Object, e As RoutedEventArgs) Handles btn_download.Click
        If lst.SelectedIndex = -1 Then
            MessageBox.Show("Bitte wähle eine Forge Version Aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error)
        ElseIf wc.IsBusy = True Then
            wc.CancelAsync()
        Else
            Dim url As New Uri(DirectCast(lst.SelectedItem, ForgeEintrag).downloadLink)
            Dim ls As IList(Of String) = url.Segments
            filename = cachefolder & "\" & ls.Last
            wc.DownloadFileAsync(url, filename)
            btn_download.Content = "Abbrechen"

            'Dim dateiendung As String = "zip"
            'version = DirectCast(lst.SelectedItem, ForgeEintrag).version
            'build = DirectCast(lst.SelectedItem, ForgeEintrag).build
            'profilename = String.Format("{1}-Forge{2}", version, build)
            'filename = String.Format("{1}\{2}.{3}", cachefolder, profilename, dateiendung)
            'wc.DownloadFileAsync(New Uri(url), filename)
            ''Installieren
        End If
    End Sub

    Private Sub wc_DownloadFileCompleted(sender As Object, e As ComponentModel.AsyncCompletedEventArgs) Handles wc.DownloadFileCompleted
        If e.Cancelled = True Then
            IO.File.Delete(filename)
        Else
            pb_download.IsIndeterminate = True
            Process.Start(filename)
            ''Entpacken... und installieren
            'UnzipForge(filename)
            ''Profiles hinzufügen
            'Dim profile As New Profile(profilename, Nothing, version, Nothing, Nothing, Nothing, Nothing, Nothing)
            'If Profiles.List.Contains(profilename) = True Then
            '    Profiles.Edit(profilename, profile)
            'Else
            '    Profiles.Add(profile)
            'End If
            'pb_download.IsIndeterminate = False
            'pb_download.Value = pb_download.Maximum
            'downloading = False
        End If
        btn_download.Content = "Herunterladen und Installieren"
    End Sub

    Private Sub wc_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles wc.DownloadProgressChanged
        pb_download.Value = e.ProgressPercentage
    End Sub

    Private Sub UnzipForge(file As String)
        Dim UnpackDirectory As String = mcpfad
        Using zip1 As Zipfile = ZipFile.Read(file)
            Dim e As ZipEntry
            ' here, we extract every entry, but we could extract conditionally,
            ' based on entry name, size, date, checkbox status, etc.   
            For Each e In zip1
                e.Extract(UnpackDirectory, ExtractExistingFileAction.OverwriteSilently)
            Next
        End Using
    End Sub
End Class
