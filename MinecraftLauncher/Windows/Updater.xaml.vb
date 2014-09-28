Imports System
Imports System.Net

Class Updater

    Private WithEvents wc As New WebClient
    Dim Installer As String = IO.Path.Combine(Applicationcache.FullName, "McMetroLauncher.msi")

    Private ReadOnly Property AssemblyVersion As String
        Get
            Return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
        End Get
    End Property

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

        lblCurrentVersion.Text = AssemblyVersion
        lblNewVersion.Text = onlineversion
        wc_Changelog.WebSession = WebCore.CreateWebSession(New WebPreferences() With {.CustomCSS = Scrollbarcss})
    End Sub

    Private Sub btn_Click(sender As Object, e As RoutedEventArgs)
        'If updController.isUpdateDownloaderBusy Then
        '    updController.cancelUpdateDownload()
        '    btn.Content = "Update Downloaden"
        'Else
        '    updController.downloadUpdates()
        '    btn.Content = "Abbrechen"
        'End If

        If UpdaterViewModel.Instance.installerdownloading Then
            wc.CancelAsync()
        Else
            wc = New WebClient
            If IO.Directory.Exists(IO.Path.GetDirectoryName(Installer)) = False Then
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(Installer))
            End If
            wc.DownloadFileAsync(New Uri("http://patzleiner.net/download/McMetroLauncher.msi"), Installer)
            UpdaterViewModel.Instance.installerdownloading = True
        End If
    End Sub

    Private Sub wc_DownloadFileCompleted(sender As Object, e As ComponentModel.AsyncCompletedEventArgs) Handles wc.DownloadFileCompleted
        If e.Cancelled = False Then
            ' Schliessen und starten
            Process.Start(Installer)
            Application.Current.Shutdown()
        End If
        UpdaterViewModel.Instance.installerdownloading = False
    End Sub

    Private Sub wc_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles wc.DownloadProgressChanged
        prg.Value = e.ProgressPercentage
    End Sub

    Private Sub wc_Changelog_Loaded(sender As Object, e As RoutedEventArgs) Handles wc_Changelog.Loaded
        Dim markdown As New MarkdownSharp.Markdown
        Dim html As String = markdown.Transform(changelog)
        wc_Changelog.LoadHTML(html)
    End Sub

End Class
