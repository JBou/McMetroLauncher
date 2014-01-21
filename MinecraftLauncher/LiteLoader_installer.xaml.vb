Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports MahApps.Metro
Imports MahApps.Metro.Controls.Dialogs
Imports System.Net

Public Class LiteLoader_installer

    WithEvents wc As New WebClient
    Private filename As String

    Private Async Sub ForgeManager_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If ThemeManager.DetectTheme(Application.Current).Item1 = Theme.Light Then
            btn_copy_image.Source = ImageConvert.GetImageStream(My.Resources.appbar_page_copy)
        Else
            btn_copy_image.Source = ImageConvert.GetImageStream(My.Resources.appbar_page_copy_dark)
        End If
        If LiteLoader.List Is Nothing Then
            Await LiteLoader.Load()
        End If
        For Each item As LiteLoader.LiteLoaderEintrag In LiteLoader.List
            lst.Items.Add(item)
        Next
        tb_mcpfad.Text = mcpfad
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

Public Class LiteLoader
    Public Shared List As IList(Of LiteLoader.LiteLoaderEintrag)
    Public Shared Async Function Load() As Task
        If Mods.modsjo Is Nothing Then
            Mods.Load()
        End If
        If Mods.modsjo.Properties.Select(Function(p) p.Name).Contains("liteloader") Then
            List = Await JsonConvert.DeserializeObjectAsync(Of IList(Of LiteLoader.LiteLoaderEintrag))(Mods.modsjo("liteloader").ToString)
        Else
            List = New List(Of LiteLoader.LiteLoaderEintrag)
        End If
    End Function
    Public Class LiteLoaderEintrag
        Private m_version As String, m_downloadlink As String
        <JsonProperty("version")>
        Public Property Version As String
            Get
                Return m_version
            End Get
            Set(value As String)
                m_version = value
            End Set
        End Property
        <JsonProperty("downloadlink")>
        Public Property DownloadLink As String
            Get
                Return m_downloadlink
            End Get
            Set(value As String)
                m_downloadlink = value
            End Set
        End Property

        Public Sub New()

        End Sub

        Public Sub New(Version As String, Downloadlink As String)
            Me.Version = Version
            Me.DownloadLink = Downloadlink
        End Sub

    End Class
End Class