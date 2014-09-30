Imports McMetroLauncher.JBou.Authentication
Imports McMetroLauncher.JBou.Authentication.Session
Imports System.Runtime.ExceptionServices
Imports MahApps.Metro.Controls.Dialogs
Imports MahApps.Metro
Imports System.ComponentModel
Imports System.Text.RegularExpressions

Public Class Login

    Public Session As Session

    Sub Open()
        Controls.TextboxHelper.SetIsWaitingForData(tb_username, False)
        Visibility = System.Windows.Visibility.Visible
        Load_Accounts()
        tb_username.Text = Nothing
        pb_password.Password = Nothing
        cb_online_mode.IsChecked = True
        UpdateValidation()
    End Sub

    Private Sub Login_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Load_Accounts()
    End Sub

    Sub Load_Accounts()
        cb_existing_users.Items.Clear()
        If authenticationDatabase.List.Count > 0 Then
            For Each item As authenticationDatabase.Account In authenticationDatabase.List
                cb_existing_users.Items.Add(item)
            Next
            cb_existing_users.SelectedIndex = 0
            gb_existingaccount.Visibility = System.Windows.Visibility.Visible
        Else
            gb_existingaccount.Visibility = System.Windows.Visibility.Collapsed
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
            If Guid.TryParse(Account.userid, New Guid) Then
                Dim session = New Session() With {.AccessToken = Account.accessToken,
                                                    .ClientToken = authenticationDatabase.clientToken,
                                                    .SelectedProfile = Nothing}
                Dim profile As Profiles.Profile = Await Profiles.FromName(MainViewModel.Instance.selectedProfile)
                profile.playerUUID = Account.uuid.Replace("-", "")
                If MainViewModel.Instance.Account Is Nothing OrElse Not MainViewModel.Instance.Account.uuid = Account.uuid Then
                    Await session.Refresh
                    authenticationDatabase.List.Where(Function(p) p.uuid = Account.uuid).First.accessToken = session.AccessToken
                    Await authenticationDatabase.Save()
                    profile.playerUUID = session.SelectedProfile.Id
                    MainViewModel.Instance.Account = session.ToAccount
                End If
                Await Profiles.Edit(MainViewModel.Instance.selectedProfile, profile)
            Else
                Dim profile As Profiles.Profile = Await Profiles.FromName(MainViewModel.Instance.selectedProfile)
                profile.playerUUID = Account.uuid.Replace("-", "")
                Await Profiles.Edit(MainViewModel.Instance.selectedProfile, profile)
                MainViewModel.Instance.Account = Account
            End If
            Main.Show()
            Me.Visibility = System.Windows.Visibility.Collapsed
            Main.TabControl_main.Visibility = System.Windows.Visibility.Visible
        Catch ex As MinecraftAuthenticationException
            capturedException = ex
        End Try
        If capturedException IsNot Nothing Then
            Await DirectCast(MainWindow.GetWindow(Me), MainWindow).ShowMessageAsync(capturedException.Error, capturedException.ErrorMessage, MessageDialogStyle.Affirmative)
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
            If Not tb_username.GetBindingExpression(TextBox.TextProperty).HasError Then
                Controls.TextboxHelper.SetIsWaitingForData(tb_username, False)
                If cb_online_mode.IsChecked Then
                    Session = Await JBou.Authentication.Session.DoLogin(tb_username.Text, pb_password.Password)
                    authenticationDatabase.clientToken = Session.ClientToken
                    If authenticationDatabase.List.Select(Function(p) p.uuid.Replace("-", "")).Contains(Session.SelectedProfile.Id) Then
                        authenticationDatabase.List.Remove(authenticationDatabase.List.Where(Function(p) p.uuid.Replace("-", "") = Session.SelectedProfile.Id).First)
                    End If
                    authenticationDatabase.List.Add(Session.ToAccount)
                    Await authenticationDatabase.Save()
                    Dim profile As Profiles.Profile = Await Profiles.FromName(MainViewModel.Instance.selectedprofile)
                    profile.playerUUID = Session.SelectedProfile.Id
                    Await Profiles.Edit(MainViewModel.Instance.selectedprofile, profile)
                    MainViewModel.Instance.Account = Session.ToAccount
                Else
                    If authenticationDatabase.List.Select(Function(p) p.userid).Contains(tb_username.Text) Then
                        authenticationDatabase.List.Remove(authenticationDatabase.List.Where(Function(p) p.userid = tb_username.Text).First)
                    End If
                    Dim account As New authenticationDatabase.Account() With {
                                                    .displayName = tb_username.Text,
                                                    .username = tb_username.Text,
                                                    .uuid = Guid.NewGuid.ToString,
                                                    .userid = tb_username.Text}
                    authenticationDatabase.List.Add(account)
                    Await authenticationDatabase.Save()
                    Dim profile As Profiles.Profile = Await Profiles.FromName(MainViewModel.Instance.selectedprofile)
                    profile.playerUUID = account.uuid.Replace("-", "")
                    Await Profiles.Edit(MainViewModel.Instance.selectedprofile, profile)
                    MainViewModel.Instance.Account = account
                End If
                Main.Show()
                Me.Visibility = System.Windows.Visibility.Collapsed
                Main.TabControl_main.Visibility = System.Windows.Visibility.Visible
            End If
        Catch ex As MinecraftAuthenticationException
            capturedException = ex
        End Try
        If capturedException IsNot Nothing Then
            Await DirectCast(MainWindow.GetWindow(Me), MainWindow).ShowMessageAsync(capturedException.Error, capturedException.ErrorMessage, MessageDialogStyle.Affirmative)
        End If
    End Sub

    Private Async Sub btn_logout_Click(sender As Object, e As RoutedEventArgs) Handles btn_logout.Click
        'logout / invalidate session
        Dim capturedException As MinecraftAuthenticationException = Nothing
        Try
            Dim Account = DirectCast(cb_existing_users.SelectedItem, authenticationDatabase.Account)
            If Guid.TryParse(Account.userid, New Guid) Then
                Dim session = New Session() With {.AccessToken = Account.accessToken,
                                                  .ClientToken = authenticationDatabase.clientToken,
                                                  .SelectedProfile = Nothing}
                Await session.Invalidate()
            End If
            authenticationDatabase.List.Remove(authenticationDatabase.List.Where(Function(p) p.uuid = Account.uuid).First)
            Await authenticationDatabase.Save()
            Load_Accounts()
        Catch ex As MinecraftAuthenticationException
            capturedException = ex
        End Try
        If capturedException IsNot Nothing Then
            Await DirectCast(MainWindow.GetWindow(Me), MainWindow).ShowMessageAsync(capturedException.Error, capturedException.ErrorMessage, MessageDialogStyle.Affirmative)
        End If
    End Sub

    Private Sub UpdateValidation()
        If tb_username IsNot Nothing Then
            tb_username.GetBindingExpression(TextBox.TextProperty).UpdateSource()
        End If
    End Sub
End Class