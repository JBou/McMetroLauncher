Imports McMetroLauncher.JBou.Authentication
Imports McMetroLauncher.JBou.Authentication.Session
Imports System.Runtime.ExceptionServices
Imports MahApps.Metro.Controls.Dialogs

Public Class Login
    Public Session As Session

    Public Sub New()
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Load_Accounts()
    End Sub
    Sub Load_Accounts()
        cb_existing_users.Items.Clear()
        If authenticationDatabase.List.Count > 0 Then
            For Each item As authenticationDatabase.Account In authenticationDatabase.List
                cb_existing_users.Items.Add(item)
            Next
            cb_existing_users.SelectedIndex = 0
        Else
            cb_existing_users.Visibility = System.Windows.Visibility.Collapsed
            btn_play.Visibility = System.Windows.Visibility.Collapsed
            btn_logout.Visibility = System.Windows.Visibility.Collapsed
        End If
    End Sub

    Private Sub Hyperlink_RequestNavigate(sender As Object, e As RequestNavigateEventArgs)
        Process.Start(New ProcessStartInfo(e.Uri.AbsoluteUri))
        e.Handled = True
    End Sub

    Private Sub open_register()
        Process.Start("https://account.mojang.com/register")
    End Sub

    Private Async Sub btn_play_Click(sender As Object, e As RoutedEventArgs) Handles btn_play.Click
        Dim capturedException As MinecraftAuthenticationException = Nothing
        'Login with access token
        Try
            Dim Account = DirectCast(cb_existing_users.SelectedItem, authenticationDatabase.Account)
            Dim session = New Session() With {.AccessToken = Account.accessToken,
                                              .ClientToken = authenticationDatabase.clientToken,
                                              .SelectedProfile = Nothing}
            Await session.Refresh
            authenticationDatabase.List.Where(Function(p) p.uuid = Account.uuid).First.accessToken = session.AccessToken
            Await authenticationDatabase.Save()
            Dim profile As Profiles.Profile = Await Profiles.FromName(ViewModel.selectedprofile)
            profile.playerUUID = session.SelectedProfile.Id
            Await Profiles.Edit(ViewModel.selectedprofile, profile)
            Main.lbl_Username.Content = "Willkommen, " & session.SelectedProfile.Name
            Main.Show()
            Me.Close()
        Catch ex As MinecraftAuthenticationException
            capturedException = ex
        End Try
        If capturedException IsNot Nothing Then
            Await Me.ShowMessageAsync(capturedException.Error, capturedException.ErrorMessage, MessageDialogStyle.Affirmative)
            If capturedException.ErrorMessage = "Invalid token." Then
                Dim Account = DirectCast(cb_existing_users.SelectedItem, authenticationDatabase.Account)
                authenticationDatabase.List.Remove(authenticationDatabase.List.Where(Function(p) p.uuid = Account.uuid).First)
                Await authenticationDatabase.Save()
                Load_Accounts()
            End If
        End If
    End Sub

    Private Async Sub btn_login_Click(sender As Object, e As RoutedEventArgs) Handles btn_login.Click
        Dim capturedException As MinecraftAuthenticationException = Nothing
        'login with username & password
        Try
            Session = Await JBou.Authentication.Session.DoLogin(tb_username.Text, pb_password.Password)
            authenticationDatabase.clientToken = Session.ClientToken
            If authenticationDatabase.List.Select(Function(p) p.uuid).Contains(Session.SelectedProfile.Id) Then
                authenticationDatabase.List.Remove(authenticationDatabase.List.Where(Function(p) p.uuid = Session.SelectedProfile.Id).First)
            End If
            authenticationDatabase.List.Add(Session.ToAccount)
            Await authenticationDatabase.Save()
            Dim profile As Profiles.Profile = Await Profiles.FromName(ViewModel.selectedprofile)
            profile.playerUUID = Session.SelectedProfile.Id
            Await Profiles.Edit(ViewModel.selectedprofile, profile)
            Main.lbl_Username.Content = "Willkommen, " & Session.SelectedProfile.Name
            Main.Show()
            Me.Close()
        Catch ex As MinecraftAuthenticationException
            capturedException = ex
        End Try
        If capturedException IsNot Nothing Then
            Await Me.ShowMessageAsync(capturedException.Error, capturedException.ErrorMessage, MessageDialogStyle.Affirmative)
        End If
    End Sub

    Private Async Sub btn_logout_Click(sender As Object, e As RoutedEventArgs) Handles btn_logout.Click
        'logout / invalidate session
        Dim capturedException As MinecraftAuthenticationException = Nothing
        Try
            Dim Account = DirectCast(cb_existing_users.SelectedItem, authenticationDatabase.Account)
            Dim session = New Session() With {.AccessToken = Account.accessToken,
                                              .ClientToken = authenticationDatabase.clientToken,
                                              .SelectedProfile = Nothing}
            Await session.Invalidate()
            authenticationDatabase.List.Remove(authenticationDatabase.List.Where(Function(p) p.uuid = Account.uuid).First)
            Await authenticationDatabase.Save()
            Load_Accounts()
        Catch ex As MinecraftAuthenticationException
            capturedException = ex
        End Try
        If capturedException IsNot Nothing Then
            Await Me.ShowMessageAsync(capturedException.Error, capturedException.ErrorMessage, MessageDialogStyle.Affirmative)
        End If
    End Sub
End Class
