Imports System.Net
Imports Ionic.Zip
Imports MahApps.Metro.Controls.Dialogs
Imports MahApps.Metro

Class Forge_installer
    WithEvents wc As New WebClient
    Private filename As String

    Public Sub Load_Forge()
        lst.Items.Clear()
        For Each item As Forge.Forgeeintrag In Forge.ForgeList
            lst.Items.Add(item)
        Next
    End Sub

    Private Sub ForgeManager_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If ThemeManager.DetectTheme(Application.Current).Item1 = Theme.Light Then
            btn_copy_image.Source = ImageConvert.GetImageStream(My.Resources.appbar_page_copy)
        Else
            btn_copy_image.Source = ImageConvert.GetImageStream(My.Resources.appbar_page_copy_dark)
        End If
        Load_Forge()
        tb_mcpfad.Text = mcpfad
    End Sub

    Private Async Sub btn_download_Click(sender As Object, e As RoutedEventArgs) Handles btn_download.Click
        If lst.SelectedIndex = -1 Then
            Await Me.ShowMessageAsync(Nothing, "Bitte wähle eine Forge Version Aus!", MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = "Ok", .ColorScheme = MetroDialogColorScheme.Accented})
        ElseIf wc.IsBusy = True Then
            wc.CancelAsync()
        Else
            forge_anleitung.IsSelected = True
            Dim url As New Uri(DirectCast(lst.SelectedItem, Forge.Forgeeintrag).downloadlink)
            Dim ls As IList(Of String) = url.Segments
            filename = cachefolder & "\" & ls.Last
            wc.DownloadFileAsync(url, filename)
            btn_download.Content = "Abbrechen"
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
        btn_download.Content = "Herunterladen und Installieren"
    End Sub

    Private Sub wc_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles wc.DownloadProgressChanged
        pb_download.Value = e.ProgressPercentage
    End Sub
End Class
