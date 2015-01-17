Imports System.Net
Imports System.IO
Imports System.Xml.Linq
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Ookii.Dialogs.Wpf
Imports MahApps.Metro
Imports MahApps.Metro.Controls
Imports MahApps.Metro.Controls.Dialogs
Imports McMetroLauncher.Profiles

Public Class ProfileEditor
    Private loadedprofile As String

    Sub Get_Versions()
        Dim selectedid As String = Nothing
        If cb_versions.SelectedIndex <> -1 Then
            selectedid = DirectCast(cb_versions.SelectedItem, Versionslist.Version).id
        End If
        cb_versions.Items.Clear()
        cb_versions.Items.Add(New Versionslist.Version() With {.type = Application.Current.FindResource("LatestVersion").ToString})
        For Each item As Versionslist.Version In Versions.versions
            If item.type = "release" Then
                cb_versions.Items.Add(item)
            ElseIf item.type = "snapshot" Then
                If cb_snapshots.IsChecked = True Then
                    cb_versions.Items.Add(item)
                End If
            ElseIf item.type = "old_beta" Then
                If cb_old_beta.IsChecked = True Then
                    cb_versions.Items.Add(item)
                End If
            ElseIf item.type = "old_alpha" Then
                If cb_old_alpha.IsChecked = True Then
                    cb_versions.Items.Add(item)
                End If
            End If
        Next
        If selectedid = Nothing Then
            cb_versions.SelectedIndex = 0
        Else
            cb_versions.SelectedItem = cb_versions.Items.OfType(Of Versionslist.Version).ToList.Where(Function(p) p.id = selectedid).FirstOrDefault
        End If
    End Sub

    Async Function Load_ProfileInfos() As Task
        Dim profile As Profile = Await Profiles.FromName(loadedprofile)
        If profile.name = Nothing Then
            tb_profile_name.Text = mcpfad.FullName
        Else
            tb_profile_name.Text = profile.name
        End If
        If profile.resolution IsNot Nothing Then
            If profile.resolution.height = Nothing Then
                tb_res_height.Text = "480"
            Else
                tb_res_height.Text = profile.resolution.height
                cb_resolution.IsChecked = True
            End If
            If profile.resolution.width = Nothing Then
                tb_res_width.Text = "854"
            Else
                tb_res_width.Text = profile.resolution.width
                cb_resolution.IsChecked = True
            End If
        End If
        If profile.gameDir = Nothing Then
            tb_gameDir.Text = mcpfad.FullName
        Else
            tb_gameDir.Text = profile.gameDir
            cb_game_directory.IsChecked = True
        End If
        If profile.allowedReleaseTypes IsNot Nothing Then
            If profile.allowedReleaseTypes.Count > 0 Then
                If profile.allowedReleaseTypes.Contains("snapshot") = True Then
                    cb_snapshots.IsChecked = True
                End If
                If profile.allowedReleaseTypes.Contains("old_beta") = True Then
                    cb_old_beta.IsChecked = True
                End If
                If profile.allowedReleaseTypes.Contains("old_alpha") = True Then
                    cb_old_alpha.IsChecked = True
                End If
            End If
        End If
        Get_Versions()
        If cb_versions.Items.OfType(Of Versionslist.Version).ToList.Select(Function(p) p.id).Contains(profile.lastVersionId) Then
            cb_versions.SelectedItem = cb_versions.Items.OfType(Of Versionslist.Version).ToList.Where(Function(p) p.id = profile.lastVersionId).FirstOrDefault
        Else
            cb_versions.SelectedIndex = 0
        End If
        If profile.javaDir = Nothing Then
            tb_java_executable.Text = MainWindow.Startcmd(Await Profiles.FromName(loadedprofile))
        Else
            tb_java_executable.Text = profile.javaDir
            cb_java_path.IsChecked = True
        End If
        If profile.javaArgs = Nothing Then
            tb_java_arguments.Text = "-Xmx1G"
        Else
            tb_java_arguments.Text = profile.javaArgs
            cb_java_arguments.IsChecked = True
        End If

    End Function

    Async Function StandardValues() As Task
        tb_gameDir.Text = mcpfad.FullName
        tb_res_height.Text = "480"
        tb_res_width.Text = "854"
        tb_java_executable.Text = MainWindow.Startcmd(Await Profiles.FromName(loadedprofile))
        tb_java_arguments.Text = "-Xmx1G"
    End Function

    Private Sub tb_res_PreviewTextInput(ByVal sender As System.Object, ByVal e As System.Windows.Input.TextCompositionEventArgs) Handles tb_res_height.PreviewTextInput, tb_res_width.PreviewTextInput
        If Not Char.IsNumber(CChar(e.Text)) Then e.Handled = True
    End Sub

    Private Async Sub ProfileEditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Try
            Await Versions_Load()
            Get_Versions()
            loadedprofile = MainViewModel.Instance.selectedprofile
            If Newprofile = True Then
                Await StandardValues()
            Else
                Await Load_ProfileInfos()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message & Environment.NewLine & ex.StackTrace)
        End Try
    End Sub

    Private Async Sub btn_save_Click(sender As Object, e As RoutedEventArgs) Handles btn_save.Click

        If tb_profile_name.Text = Nothing Then
            Await Me.ShowMessageAsync(Application.Current.FindResource("MissingProfileName").ToString, Application.Current.FindResource("EnterProfileName").ToString, MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = Application.Current.FindResource("OK").ToString, .ColorScheme = MetroDialogColorScheme.Accented})
            Exit Sub
        Else
            Dim name As String = tb_profile_name.Text
            Dim gameDir As String = Nothing
            Dim lastVersionId As String = Nothing
            Dim javaDir As String = Nothing
            Dim javaArgs As String = Nothing
            Dim resolution_width As String = Nothing
            Dim resolution_height As String = Nothing
            Dim allowedReleaseTypes As IList(Of String) = New List(Of String)

            If cb_game_directory.IsChecked = True Then
                gameDir = tb_gameDir.Text
            End If

            If cb_versions.SelectedIndex <> 0 Then
                If cb_versions.SelectedIndex = 0 Then
                    lastVersionId = Nothing
                Else
                    lastVersionId = DirectCast(cb_versions.SelectedItem, Versionslist.Version).id
                End If
            End If

            If cb_java_path.IsChecked = True Then
                javaDir = tb_java_executable.Text
            End If
            If cb_java_arguments.IsChecked = True Then
                javaArgs = tb_java_arguments.Text
            End If
            If cb_resolution.IsChecked = True Then
                If tb_res_height.Text = Nothing OrElse tb_res_width.Text = Nothing Then
                    Await Me.ShowMessageAsync(Application.Current.FindResource("Error").ToString, Application.Current.FindResource("EnterValidResolution").ToString, MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = Application.Current.FindResource("OK").ToString, .ColorScheme = MetroDialogColorScheme.Accented})
                    Exit Sub
                End If
                resolution_width = tb_res_width.Text
                resolution_height = tb_res_height.Text
            End If

            If cb_snapshots.IsChecked OrElse cb_old_beta.IsChecked OrElse cb_old_alpha.IsChecked Then
                If cb_snapshots.IsChecked = True Then
                    allowedReleaseTypes.Add("snapshot")
                End If
                If cb_old_beta.IsChecked = True Then
                    allowedReleaseTypes.Add("old_beta")
                End If
                If cb_old_alpha.IsChecked = True Then
                    allowedReleaseTypes.Add("old_alpha")
                End If
                'Release wird in der ProfileClass automatisch eingefügt, falls die liste nicht nothing ist
            Else
                allowedReleaseTypes = Nothing
            End If

            Dim res As New Profiles.Profile.cls_Resolution With {
                                    .height = resolution_height,
                                    .width = resolution_width
                }
            If resolution_height = Nothing AndAlso resolution_width = Nothing Then
                res = Nothing
            End If

            Dim prof As New Profiles.Profile() With {
                .name = name,
                .gameDir = gameDir,
                .resolution = res,
                .lastVersionId = lastVersionId,
                .javaDir = javaDir,
                .javaArgs = javaArgs,
                .allowedReleaseTypes = allowedReleaseTypes
                }

            If Newprofile = True Then
                If Profiles.List.Contains(tb_profile_name.Text) = True Then
                    Await Me.ShowMessageAsync(Application.Current.FindResource("Error").ToString, Application.Current.FindResource("ProfileExists").ToString, MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = "Ok", .ColorScheme = MetroDialogColorScheme.Accented})
                Else
                    Await Profiles.Add(prof)
                End If

            Else
                Dim oldprofile As Profiles.Profile = Await Profiles.FromName(loadedprofile)
                prof.playerUUID = oldprofile.playerUUID
                If Profiles.List.Contains(tb_profile_name.Text.ToString) AndAlso tb_profile_name.Text.ToString <> loadedprofile Then
                    Await Me.ShowMessageAsync(Application.Current.FindResource("Error").ToString, Application.Current.FindResource("ProfileExists").ToString, MessageDialogStyle.Affirmative, New MetroDialogSettings() With {.AffirmativeButtonText = "Ok", .ColorScheme = MetroDialogColorScheme.Accented})
                Else
                    Await Profiles.Edit(loadedprofile, prof)
                End If
            End If

            Profiles.Get_Profiles()
            Me.Close()
        End If
    End Sub

    Private Sub btn_selectgamedir_Click(sender As Object, e As RoutedEventArgs) Handles btn_selectgamedir.Click
        Dim fd As New VistaFolderBrowserDialog
        fd.Description = Application.Current.FindResource("SelectGameDirectory").ToString
        fd.RootFolder = Environment.SpecialFolder.ApplicationData
        fd.SelectedPath = mcpfad.FullName
        fd.ShowNewFolderButton = True
        If fd.ShowDialog = True Then
            tb_gameDir.Text = fd.SelectedPath
        End If
    End Sub

    Private Sub btn_selectjavadir_Click(sender As Object, e As RoutedEventArgs) Handles btn_selectjavadir.Click
        Dim fd As New VistaOpenFileDialog
        fd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
        If tb_java_executable.Text = Nothing Then
            fd.FileName = Nothing
        Else
            fd.FileName = tb_java_executable.Text
        End If
        fd.Multiselect = False
        fd.CheckFileExists = True
        fd.CheckPathExists = True
        fd.Filter = String.Format("{0}|*.exe", Application.Current.FindResource("ExectuableFiles").ToString())
        fd.Title = Application.Current.FindResource("OFDJavaPathTitle").ToString
        If fd.ShowDialog = True Then
            tb_java_executable.Text = fd.FileName
        End If
    End Sub

    Private Sub Open_gameDir()
        Dim p As New Process
        With p.StartInfo
            .FileName = "explorer.exe"
            .Arguments = tb_gameDir.Text
        End With
        p.Start()
    End Sub

    Private Sub cb_checked_unchecked(sender As Object, e As RoutedEventArgs) Handles cb_snapshots.Checked, cb_old_beta.Checked, cb_old_alpha.Checked, cb_snapshots.Unchecked, cb_old_beta.Unchecked, cb_old_alpha.Unchecked
        Get_Versions()
        If cb_versions.SelectedIndex = -1 Then
            cb_versions.SelectedIndex = 0
        End If
    End Sub

    Private Async Sub cb_old_alpha_beta_PreviewMouseDown(sender As Object, e As RoutedEventArgs) Handles cb_old_beta.PreviewMouseDown, cb_old_alpha.PreviewMouseDown, cb_snapshots.PreviewMouseDown
        If Not DirectCast(sender, CheckBox).IsChecked Then
            If sender Is cb_old_beta OrElse sender Is cb_old_alpha OrElse sender Is cb_snapshots Then
                e.Handled = True
                Dim msgtext As String = Application.Current.FindResource("OldVersionsMessage").ToString
                If sender Is cb_snapshots Then msgtext = Application.Current.FindResource("SnapshotVersionsMessage").ToString
                Dim result As MessageDialogResult = Await Me.ShowMessageAsync(Application.Current.FindResource("Attention").ToString, msgtext, MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, New MetroDialogSettings() With {.AffirmativeButtonText = Application.Current.FindResource("Yes").ToString, .NegativeButtonText = Application.Current.FindResource("No").ToString, .FirstAuxiliaryButtonText = Application.Current.FindResource("Cancel").ToString, .ColorScheme = MetroDialogColorScheme.Accented, .AnimateShow = True, .AnimateHide = True})

                If result = MessageDialogResult.Affirmative Then
                    Get_Versions()
                    DirectCast(sender, CheckBox).IsChecked = True
                Else
                    DirectCast(sender, CheckBox).IsChecked = False
                End If
            End If
        End If
    End Sub
End Class
