Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports MahApps.Metro
Imports MahApps.Metro.Controls.Dialogs
Imports System.Net

Public Class LiteLoader_installer

    WithEvents wc As New WebClient
    Private filename As String

    Private Async Sub ForgeManager_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim theme = ThemeManager.DetectAppStyle(Application.Current)
        Dim appTheme = ThemeManager.GetAppTheme(Me.Name)
        If appTheme.Name = "BaseLight" Then
            btn_copy_image.Source = ImageConvert.GetImageStream(My.Resources.appbar_page_copy)
        Else
            btn_copy_image.Source = ImageConvert.GetImageStream(My.Resources.appbar_page_copy_dark)
        End If
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
            Await Me.ShowMessageAsync(Nothing, "Bitte wähle eine LiteLoader Version Aus!", MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = "Ok", .ColorScheme = MetroDialogColorScheme.Accented})
        ElseIf wc.IsBusy = True Then
            wc.CancelAsync()
        Else
            forge_anleitung.IsSelected = True
            Dim url As New Uri(DirectCast(lst.SelectedItem, LiteLoader.LiteLoaderEintrag).DownloadLink)
            Dim ls As IList(Of String) = url.Segments
            filename = IO.Path.Combine(cachefolder.FullName, ls.Last)
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

    Private Sub btn_copy_Click(sender As Object, e As RoutedEventArgs) Handles btn_copy.Click
        Clipboard.SetText(tb_mcpfad.Text)
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

End Class