Imports MahApps.Metro.Controls.Dialogs
Imports MahApps.Metro

Public Class Forge_ProfileEditor
    Public Versionname As String = Nothing

    Private Sub Forge_ProfileEditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        lbl_header.Content = Versionname
        tb_newprofilename.Text = Versionname
    End Sub

    Private Async Sub btn_save_Click(sender As Object, e As RoutedEventArgs) Handles btn_save.Click
        If rb_existingprofile.IsChecked Then
            Dim selectedprofile As Profiles.Profile = Await Profiles.FromName(cb_profiles.SelectedItem.ToString)
            selectedprofile.lastVersionId = Versionname
            Await Profiles.Edit(selectedprofile.name, selectedprofile)
            'Profiles.Get_Profiles()
            Me.Close()
        Else
            If Profiles.List.Contains(tb_newprofilename.Text) Then
                Await Me.ShowMessageAsync(Application.Current.FindResource("Error").ToString, Application.Current.FindResource("ProfileExists").ToString & ": " & tb_newprofilename.Text)
            Else
                Await Profiles.Add(New Profiles.Profile With {.name = tb_newprofilename.Text, .lastVersionId = Versionname})
                Profiles.Get_Profiles()
                Me.Close()
            End If
        End If
    End Sub

    Private Sub btn_skip_Click(sender As Object, e As RoutedEventArgs) Handles btn_skip.Click
        Me.Close()
    End Sub

End Class
