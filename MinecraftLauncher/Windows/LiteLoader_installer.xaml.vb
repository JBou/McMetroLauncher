Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports MahApps.Metro
Imports MahApps.Metro.Controls.Dialogs
Imports System.Net

Public Class LiteLoader_installer

    WithEvents wc As New WebClient
    Private filename As String

    Private Async Sub LiteLoader_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If LiteLoader.LiteLoaderList Is Nothing Then
            Await LiteLoader.Load()
        End If
        For Each item As LiteLoader.LiteloaderEintrag In LiteLoader.LiteLoaderList
            lst.Items.Add(item)
        Next
        tb_mcpfad.Text = mcpfad.FullName
    End Sub

    Private Async Sub btn_download_Click(sender As Object, e As RoutedEventArgs) Handles btn_download.Click
        If lst.SelectedIndex = -1 Then
            Await Me.ShowMessageAsync(Nothing, Application.Current.FindResource("ChooseLiteLoaderVersion").ToString, MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = Application.Current.FindResource("OK").ToString, .ColorScheme = MetroDialogColorScheme.Accented})
        ElseIf wc.IsBusy = True Then
            wc.CancelAsync()
        Else
            liteloader_instructions.IsSelected = True
            Dim url As New Uri(DirectCast(lst.SelectedItem, LiteLoader.LiteLoaderEintrag).DownloadLink)
            Dim ls As IList(Of String) = url.Segments
            filename = IO.Path.Combine(cachefolder.FullName, ls.Last)
            wc.DownloadFileAsync(url, filename)
            LiteLoaderInstallerViewModel.Instance.installerdownloading = True
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
        LiteLoaderInstallerViewModel.Instance.installerdownloading = False
    End Sub

    Private Sub wc_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles wc.DownloadProgressChanged
        pb_download.Value = e.ProgressPercentage
    End Sub

End Class