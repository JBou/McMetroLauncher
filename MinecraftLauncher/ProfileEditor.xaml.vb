Imports System.Net
Imports System.IO
Imports System.Xml.Linq
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Ookii.Dialogs.Wpf
Imports MahApps.Metro
Imports MahApps.Metro.Controls
Imports MahApps.Metro.Controls.Dialogs

Public Class ProfileEditor
    Private loadedprofile As String

    Sub Get_Versions()
        'Wenn index nicht - ist
        Dim selectedid As String = Nothing
        If cb_versions.SelectedIndex <> -1 Then
            selectedid = DirectCast(cb_versions.SelectedItem, Versionslist.Version).id
        End If
        cb_versions.Items.Clear()
        cb_versions.Items.Add(New Versionslist.Version() With {.type = "Neueste Version"})
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

    Sub Load_ProfileInfos()
        If Profiles.name(loadedprofile) = Nothing Then
            tb_profile_name.Text = mcpfad
        Else
            tb_profile_name.Text = Profiles.name(loadedprofile)
        End If
        If Profiles.resolution_height(loadedprofile) = Nothing Then
            tb_res_height.Text = "480"
        Else
            tb_res_height.Text = Profiles.resolution_height(loadedprofile)
            cb_resolution.IsChecked = True
        End If
        If Profiles.resolution_width(loadedprofile) = Nothing Then
            tb_res_width.Text = "854"
        Else
            tb_res_width.Text = Profiles.resolution_width(loadedprofile)
            cb_resolution.IsChecked = True
        End If
        If Profiles.gameDir(loadedprofile) = Nothing Then
            tb_gameDir.Text = mcpfad
        Else
            tb_gameDir.Text = Profiles.gameDir(loadedprofile)
            cb_game_directory.IsChecked = True
        End If
        If Profiles.allowedReleaseTypes(loadedprofile).Count > 0 Then
            If Profiles.allowedReleaseTypes(loadedprofile).Contains("snapshot") = True Then
                cb_snapshots.IsChecked = True
            End If
            If Profiles.allowedReleaseTypes(loadedprofile).Contains("cb_old_beta") = True Then
                cb_old_beta.IsChecked = True
            End If
            If Profiles.allowedReleaseTypes(loadedprofile).Contains("cb_old_alpha") = True Then
                cb_old_alpha.IsChecked = True
            End If
        End If
        Get_Versions()
        cb_versions.SelectedItem = cb_versions.Items.OfType(Of Versionslist.Version).ToList.Where(Function(p) p.id = Profiles.lastVersionId(loadedprofile)).FirstOrDefault
        If Profiles.javaDir(loadedprofile) = Nothing Then
            tb_java_executable.Text = MainWindow.Startcmd
        Else
            tb_java_executable.Text = Profiles.javaDir(loadedprofile)
            cb_java_path.IsChecked = True
        End If
        If Profiles.javaArgs(loadedprofile) = Nothing Then
            tb_java_arguments.Text = "-Xmx1G"
        Else
            tb_java_arguments.Text = Profiles.javaArgs(loadedprofile)
            cb_java_arguments.IsChecked = True
        End If

    End Sub

    Sub StandardValues()
        tb_gameDir.Text = mcpfad
        tb_res_height.Text = "480"
        tb_res_width.Text = "854"
        tb_java_executable.Text = MainWindow.Startcmd
        tb_java_arguments.Text = "-Xmx1G"

    End Sub

    Private Sub tb_res_PreviewTextInput(ByVal sender As System.Object, ByVal e As System.Windows.Input.TextCompositionEventArgs) Handles tb_res_height.PreviewTextInput, tb_res_width.PreviewTextInput
        If Not Char.IsNumber(CChar(e.Text)) Then e.Handled = True
    End Sub

    Private Sub cb_Click(sender As Object, e As RoutedEventArgs) Handles cb_game_directory.Checked, cb_game_directory.Unchecked, cb_java_arguments.Checked, cb_java_arguments.Unchecked, cb_java_path.Checked, cb_java_path.Unchecked, cb_resolution.Checked, cb_resolution.Unchecked
        Check_cb_Status()
    End Sub

    Sub Check_cb_Status()
        If cb_game_directory.IsChecked = True Then
            tb_gameDir.IsEnabled = True
            btn_selectgamedir.IsEnabled = True
        Else
            tb_gameDir.IsEnabled = False
            btn_selectgamedir.IsEnabled = False
        End If

        If cb_java_path.IsChecked = True Then
            tb_java_executable.IsEnabled = True
        Else
            tb_java_executable.IsEnabled = False
        End If

        If cb_java_arguments.IsChecked = True Then
            tb_java_arguments.IsEnabled = True
        Else
            tb_java_arguments.IsEnabled = False
        End If

        If cb_resolution.IsChecked = True Then
            tb_res_height.IsEnabled = True
            tb_res_width.IsEnabled = True
        Else
            tb_res_height.IsEnabled = False
            tb_res_width.IsEnabled = False
        End If
    End Sub

    Private Sub ProfileEditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Get_Versions()
        loadedprofile = selectedprofile
        If Newprofile = True Then
            StandardValues()
        Else
            Load_ProfileInfos()
        End If
        Check_cb_Status()
    End Sub

    'Private Async Sub cb_snapshots_Click(sender As Object, e As RoutedEventArgs) Handles cb_snapshots.Click, cb_old_beta.Click, cb_old_alpha.Click
    '    If DirectCast(sender, CheckBox).IsChecked = True Then
    '        If sender Is cb_old_beta Or sender Is cb_old_alpha Then
    '            Dim msgtext As String = "Diese Versionen sind sehr veraltet und können unstabil sein. Alle Fehler, Abstürze, fehlende Funktionen oder andere Defekte die du finden könnstest werden in diesen Versionen nicht mehr behoben." & Environment.NewLine & "Es wird stark empfohlen, dass du diese Versionen in einem separatem Verzeichniss spielst, um Datenverlust zu vermeiden. Wir sind nicht verantwortlich für den Schaden an deinen Daten!" & Environment.NewLine & Environment.NewLine & "Bist du dir sicher, dass du fortsetzen möchstest?"
    '            Dim result As MessageDialogResult = Await Me.ShowMessageAsync("Achtung", msgtext, MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, New MetroDialogSettings() With {.AffirmativeButtonText = "Ja", .NegativeButtonText = "Nein", .FirstAuxiliaryButtonText = "Abbrechen", .ColorScheme = MetroDialogColorScheme.Accented, .UseAnimations = True})

    '            If result = MessageDialogResult.Affirmative Then
    '                Get_Versions()
    '                DirectCast(sender, CheckBox).IsChecked = True
    '            Else
    '                DirectCast(sender, CheckBox).IsChecked = False
    '            End If
    '        Else
    '            Get_Versions()
    '        End If
    '    Else
    '        Get_Versions()
    '    End If
    '    If cb_versions.SelectedIndex = -1 Then
    '        cb_versions.SelectedIndex = 0
    '    End If
    'End Sub

    Private Sub btn_save_Click(sender As Object, e As RoutedEventArgs) Handles btn_save.Click

        If tb_profile_name.Text = Nothing Then
            MessageBox.Show("Geben Sie bitte einen Profil Namen ein!", "Namen eingeben", MessageBoxButton.OK, MessageBoxImage.Information)
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
                If tb_res_height.Text = Nothing Or tb_res_width.Text = Nothing Then
                    MessageBox.Show("Bitte geben Sie eine gültige Auflösung ein!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
                    Exit Sub
                End If
                resolution_width = tb_res_width.Text
                resolution_height = tb_res_height.Text
            End If

            If cb_snapshots.IsChecked Or cb_old_beta.IsChecked Or cb_old_alpha.IsChecked Then
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

            Dim prof As New Profile(name, gameDir, lastVersionId, javaDir, javaArgs, resolution_width, resolution_height, allowedReleaseTypes)

            If Newprofile = True Then
                If Profiles.List.Contains(tb_profile_name.Text) = True Then
                    MessageBox.Show("Dieses Profil existiert bereits!", "Profil existiert bereits", MessageBoxButton.OK, MessageBoxImage.Information)
                Else
                    Profiles.Add(prof)
                End If

            Else
                If Profiles.List.Contains(tb_profile_name.Text.ToString) And tb_profile_name.Text.ToString <> loadedprofile Then
                    MessageBox.Show("Dieses Profil existiert bereits!", "Profil existiert bereits", MessageBoxButton.OK, MessageBoxImage.Information)
                Else
                    Profiles.Edit(loadedprofile, prof)
                End If
            End If

            Me.DialogResult = True
            Me.Close()
            End If
    End Sub

    Private Sub btn_selectgamedir_Click(sender As Object, e As RoutedEventArgs) Handles btn_selectgamedir.Click
        Dim fd As New VistaFolderBrowserDialog
        fd.Description = "Spiel Pfad auswählen"
        fd.RootFolder = Environment.SpecialFolder.MyComputer
        fd.SelectedPath = modsfolder
        fd.ShowNewFolderButton = True
        If fd.ShowDialog = True Then
            tb_gameDir.Text = fd.SelectedPath
        End If
    End Sub

    Private Sub btn_selectjavadir_Click(sender As Object, e As RoutedEventArgs) Handles btn_selectjavadir.Click
        Dim fd As New VistaOpenFileDialog
        If tb_java_executable.Text = Nothing Then
            fd.FileName = Nothing
        Else
            fd.FileName = tb_java_executable.Text
        End If
        fd.Multiselect = False
        fd.DefaultExt = ".exe"
        fd.Title = "Java Datei auswählen"
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

    Private Async Sub cb_old_alpha_PreviewMouseDown(sender As Object, e As MouseButtonEventArgs) Handles cb_snapshots.PreviewMouseDown, cb_old_beta.PreviewMouseDown, cb_old_alpha.PreviewMouseDown
        If DirectCast(sender, CheckBox).IsChecked = False Then
            If sender Is cb_old_beta Or sender Is cb_old_alpha Then
                Dim msgtext As String = "Diese Versionen sind sehr veraltet und können unstabil sein. Alle Fehler, Abstürze, fehlende Funktionen oder andere Defekte die du finden könnstest werden in diesen Versionen nicht mehr behoben." & Environment.NewLine & "Es wird stark empfohlen, dass du diese Versionen in einem separatem Verzeichniss spielst, um Datenverlust zu vermeiden. Wir sind nicht verantwortlich für den Schaden an deinen Daten!" & Environment.NewLine & Environment.NewLine & "Bist du dir sicher, dass du fortsetzen möchstest?"
                Dim result As MessageDialogResult = Await Me.ShowMessageAsync("Achtung", msgtext, MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, New MetroDialogSettings() With {.AffirmativeButtonText = "Ja", .NegativeButtonText = "Nein", .FirstAuxiliaryButtonText = "Abbrechen", .ColorScheme = MetroDialogColorScheme.Accented, .UseAnimations = True})

                If result = MessageDialogResult.Affirmative Then
                    Get_Versions()
                    DirectCast(sender, CheckBox).IsChecked = True
                Else
                    DirectCast(sender, CheckBox).IsChecked = False
                End If
            Else
                Get_Versions()
            End If
        Else
            Get_Versions()
        End If
        If cb_versions.SelectedIndex = -1 Then
            cb_versions.SelectedIndex = 0
        End If
    End Sub
End Class
