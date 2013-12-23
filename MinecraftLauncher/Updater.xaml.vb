Imports System
Imports System.Net

Class Updater

    Private WithEvents updController As New updateSystemDotNet.updateController()
    Private WithEvents wc As New WebClient
    Dim Installer As String = IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "cache", "McMetroLauncher.msi")

    Private ReadOnly Property AssemblyVersion As String
        Get
            Return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
        End Get
    End Property

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        UpdateControllerInitalisieren()
        lblCurrentVersion.Content = "Aktuelle Version: " & AssemblyVersion

        updController.checkForUpdatesAsync()
    End Sub

    Private Sub btn_Click(sender As Object, e As RoutedEventArgs)
        'If updController.isUpdateDownloaderBusy Then
        '    updController.cancelUpdateDownload()
        '    btn.Content = "Update Downloaden"
        'Else
        '    updController.downloadUpdates()
        '    btn.Content = "Abbrechen"
        'End If

        If wc.IsBusy Then
            wc.CancelAsync()
            btn.Content = "Update Downloaden"
        Else
            wc = New WebClient
            If IO.Directory.Exists(IO.Path.GetDirectoryName(Installer)) = False Then
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(Installer))
            End If
            wc.DownloadFileAsync(New Uri("http://patzleiner.net/download/McMetroLauncher.msi"), Installer)
            btn.Content = "Abbrechen"
        End If
    End Sub

    Private Sub UpdateControllerInitalisieren()
        updController.updateUrl = "http://patzleiner.net/mcmetrolauncher/updatesystem/"
        updController.projectId = "5fa10b5a-f769-498e-bb55-3a06f20e2c5e"
        updController.publicKey = "<RSAKeyValue><Modulus>pesqJDkvV0z870bUwSIJlGs0mkk2lFmvtRYrQu991v5daNhRUsUTiKxl7vKipJYKQJ/bw1LfF5fd6ntSjcsMAR5dKQFmeS5jaz0R6oJg9qkI1ZygUJ1qb1oC0NpsruaCp9oe5MTOjEsPVfX4TELfhyIurp7AxiihHXJysjltPWqxZtMXs1OIVxnxzNDQ16T02m2Gve/F/hY4hXjiwgLRYZN/nwLghbxnljlfflbOVsvaWSC4Rw3YyPIveLg2kiTUcqoN3tBlKOQ5YDbPIzkOSt6TOlyGnDQFG1ClyzhyUMCWGhtLE1/QRytrs7SwzG+tB+ygPcqFkLf0FT6az3nylbsurpRARLM0C9K7XWN39yzfCcOHHTwbP0GxRc4x9WqsMKmJK1ofwe99Lf+0gZ4IPXoc2U4n+sOW18pDPOMSdddz9zblPia2hdSGgJ9z9cuQ3f2RLFQh1QDGzgvxwqLt+Sy0Km3LFDb3yoIq3HfVvuz9lVCz0SsLV/YOB/l4EYonC1nKCHvHXI68/XsZ0WKd0ZyQ/X2LeTENH0bF7DQc/shC5iTNjHuownP/Reo0YnzR0tKNyt7i7M+zAf+V4snt6ykxmXy7CauOll7u0AVnwkJ5lkNH2VG51nCc2LdvMzUhdiKAlTSc27LXTXPKz0vQL13PjbhUlHiENaYR8888ac0=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"
        updController.releaseFilter.checkForFinal = True
        updController.releaseFilter.checkForBeta = True
        updController.releaseFilter.checkForAlpha = False
        updController.restartApplication = True
        updController.retrieveHostVersion = True
    End Sub

    Private Sub updController_checkForUpdatesCompleted(sender As Object, e As updateSystemDotNet.appEventArgs.checkForUpdatesCompletedEventArgs) Handles updController.checkForUpdatesCompleted
        If e.Cancelled AndAlso e.Error IsNot Nothing Then
            MessageBox.Show(e.Error.ToString, "Fehler!")
            Return
        End If
        If Not e.Result Then
            btn.Content = "Keine Updates gefunden"
            lblNewestVersion.Content = "Neuste Version: " & AssemblyVersion
        End If
        prg.IsIndeterminate = False
    End Sub

    Private Sub updController_downloadUpdatesCompleted(sender As Object, e As ComponentModel.AsyncCompletedEventArgs) Handles updController.downloadUpdatesCompleted
        updController.applyUpdate()
        Application.Current.Shutdown()
    End Sub

    Private Sub updController_downloadUpdatesProgressChanged(sender As Object, e As updateSystemDotNet.appEventArgs.downloadUpdatesProgressChangedEventArgs) Handles updController.downloadUpdatesProgressChanged
        prg.Value = e.ProgressPercentage
    End Sub

    Private Sub updController_updateFound(sender As Object, e As updateSystemDotNet.appEventArgs.updateFoundEventArgs) Handles updController.updateFound
        btn.Content = "Update downloaden"
        lblNewestVersion.Content = "Neuste Version: " & updController.currentUpdateResult.newUpdatePackages(updController.currentUpdateResult.newUpdatePackages.Count - 1).releaseInfo.Version
        Dim size As Long = 0
        Dim sb As New Text.StringBuilder
        For Each package As updateSystemDotNet.Core.Types.updatePackage In updController.currentUpdateResult.newUpdatePackages
            sb.Append("Änderungen in der Version " & package.releaseInfo.Version & " (Veröffentlicht am: " & package.ReleaseDate.Remove(package.ReleaseDate.Length - 9) & ")" & _
                      Environment.NewLine & "------------------------------" & Environment.NewLine & updController.currentUpdateResult.Changelogs(package).germanChanges & Environment.NewLine)
            size += package.packageSize
        Next

        Dim roundsize As Double
        roundsize = size / 1024
        roundsize = roundsize / 1024
        'lblSize.Content = Math.Round(roundsize, 2).ToString & " MB"
        txtChangelog.Text = sb.ToString()
    End Sub

    Private Sub MainWindow_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        updController.Dispose()
    End Sub

    Private Sub wc_DownloadFileCompleted(sender As Object, e As ComponentModel.AsyncCompletedEventArgs) Handles wc.DownloadFileCompleted
        ' Schliessen und starten
        Process.Start(Installer)
        Application.Current.Shutdown()
    End Sub

    Private Sub wc_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles wc.DownloadProgressChanged
        prg.Value = e.ProgressPercentage
    End Sub

End Class
