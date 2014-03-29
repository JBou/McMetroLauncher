Imports System.Net
Imports Ionic.Zip
Imports MahApps.Metro.Controls.Dialogs
Imports MahApps.Metro

Class Forge_installer
    WithEvents wc As New WebClient
    Private filename As String

    Public Sub Load_Forge()
        lst.Items.Clear()
        Dim list As IList(Of Forge.ForgeBuild) = New List(Of Forge.ForgeBuild)
        For Each item As Forge.ForgeBuild In Forge.ForgeList
            Dim lstitems As IList(Of String) = list.Select(Function(p) p.version).ToList
            If item.files.Select(Function(o) o.type).Contains("installer") And lstitems.Contains(item.version) = False Then
                list.Add(item)
            End If
        Next
        list = list.OrderByDescending(Function(p) p.build).ToList
        For Each item As Forge.ForgeBuild In list
            lst.Items.Add(item)
        Next
    End Sub

    Private Sub ForgeManager_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim theme = ThemeManager.DetectAppStyle(Application.Current)
        Dim appTheme = ThemeManager.GetAppTheme(Me.Name)
        If appTheme.Name = "BaseLight" Then
            btn_copy_image.Source = ImageConvert.GetImageStream(My.Resources.appbar_page_copy)
        Else
            btn_copy_image.Source = ImageConvert.GetImageStream(My.Resources.appbar_page_copy_dark)
        End If
        Load_Forge()
        tb_mcpfad.Text = mcpfad.FullName
    End Sub

    Private Async Sub btn_download_Click(sender As Object, e As RoutedEventArgs) Handles btn_download.Click
        If lst.SelectedIndex = -1 Then
            Await Me.ShowMessageAsync(Nothing, "Bitte wähle eine Forge Version Aus!", MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = "Ok", .ColorScheme = MetroDialogColorScheme.Accented})
        ElseIf wc.IsBusy = True Then
            wc.CancelAsync()
        Else
            forge_anleitung.IsSelected = True
            Dim version As String = DirectCast(lst.SelectedItem, Forge.ForgeBuild).version
            Dim mcversion As String = DirectCast(lst.SelectedItem, Forge.ForgeBuild).mcversion
            Dim Legacyforgefile As Boolean = False
            If Forge.LegacyBuildList.Select(Function(p) p.version).Contains(version) Then
                Legacyforgefile = True
            End If
            Dim url As Uri = Nothing
            If Legacyforgefile = True Then
                url = New Uri(String.Format("http://files.minecraftforge.net/minecraftforge/minecraftforge-installer-{0}-{1}.jar", mcversion, version))
            Else
                url = New Uri(String.Format("http://files.minecraftforge.net/maven/net/minecraftforge/forge/{0}-{1}/forge-{0}-{1}-installer.jar", mcversion, version))
            End If
            filename = IO.Path.Combine(cachefolder.FullName, String.Format("forge-{0}-{1}-installer.jar", mcversion, version))
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
