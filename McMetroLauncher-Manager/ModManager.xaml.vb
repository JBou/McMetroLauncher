Public Class ModManager

    Private Sub ModManager_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        Dim main As New Manager
        main.Show()
    End Sub

    Private Sub ModManager_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Load_Versions()
    End Sub

    Sub Load_Versions()
        cb_modversions.ItemsSource = Mods.Get_ModVersions
        cb_modversions.SelectedIndex = 0
    End Sub

    Sub Load_Mods()
        lb_mods.Items.Clear()
        For Each item As ForgeMod In Mods.Get_Mods(cb_modversions.SelectedItem.ToString, modsfolder)
            lb_mods.Items.Add(item)
        Next
    End Sub

    Private Sub cb_modversions_ContextMenuClosing(sender As Object, e As ContextMenuEventArgs) Handles cb_modversions.ContextMenuClosing
        Load_Mods()
    End Sub

    Private Sub cb_modversions_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cb_modversions.SelectionChanged
        Load_Mods()
        lb_mods.SelectedIndex = 0
    End Sub

    Private Sub btn_add_Click(sender As Object, e As RoutedEventArgs) Handles btn_add.Click
        Dim ModsEditor As New ModEditor
        Dim result As Boolean?
        ModsEditor.loadedmodindex = Nothing
        ModsEditor.loadedversion = Nothing
        ModsEditor.NewMod = True
        ModsEditor.versionsvisibility(True)
        ModsEditor.ShowDialog()
        result = ModsEditor.DialogResult
        If result = True Then
            Load_Versions()
            Load_Mods()
        End If
    End Sub

    Private Sub btn_edit_Click(sender As Object, e As RoutedEventArgs) Handles btn_edit.Click
        Dim ModsEditor As New ModEditor
        Dim result As Boolean?
        ModsEditor.loadedmodindex = Mods.Name(DirectCast(lb_mods.SelectedItem, ForgeMod).name, cb_modversions.SelectedItem.ToString)
        ModsEditor.loadedversion = cb_modversions.SelectedItem.ToString
        ModsEditor.NewMod = False
        ModsEditor.versionsvisibility(False)
        ModsEditor.ShowDialog()
        result = ModsEditor.DialogResult
        If result = True Then
            Load_Versions()
        End If
    End Sub

    Private Sub lb_mods_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lb_mods.SelectionChanged
        'Load_Modinfos
        If lb_mods.SelectedIndex <> -1 Then
            lbl_name.Content = DirectCast(lb_mods.SelectedItem, ForgeMod).name
            tb_description.Text = DirectCast(lb_mods.SelectedItem, ForgeMod).description
            lbl_website.Content = DirectCast(lb_mods.SelectedItem, ForgeMod).Website
            lbl_video.Content = DirectCast(lb_mods.SelectedItem, ForgeMod).video
            lbl_downloadlink.Content = DirectCast(lb_mods.SelectedItem, ForgeMod).Downloadlink
            lbl_autor.Content = DirectCast(lb_mods.SelectedItem, ForgeMod).autor
            lb_needed_mods.Items.Clear()
            For Each item As String In DirectCast(lb_mods.SelectedItem, ForgeMod).needed_mods
                lb_needed_mods.Items.Add(item)
            Next
            lbl_type.Content = "Type: " & DirectCast(lb_mods.SelectedItem, ForgeMod).type
        End If
        'If lb_mods.SelectedIndex <> -1 Then
        '    lbl_name.Content = Mods.NameAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
        '    tb_description.Text = Mods.descriptionAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
        '    lbl_website.Content = Mods.websiteAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
        '    lbl_video.Content = Mods.videoAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
        '    lbl_downloadlink.Content = Mods.downloadlinkAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
        '    lbl_autor.Content = Mods.AutorAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
        '    lb_needed_mods.Items.Clear()
        '    For Each item As String In Mods.needed_modsAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
        '        lb_needed_mods.Items.Add(item)
        '    Next
        'End If
    End Sub

    Private Sub btn_delete_Click(sender As Object, e As RoutedEventArgs) Handles btn_delete.Click
        Mods.RemoveAt(cb_modversions.SelectedItem.ToString, Mods.Name(DirectCast(lb_mods.SelectedItem, ForgeMod).name, cb_modversions.SelectedItem.ToString))
        Load_Versions()
        Load_Mods()
    End Sub

End Class
