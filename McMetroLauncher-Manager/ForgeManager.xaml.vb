Public Class ForgeManager

    Private Sub btn_add_Click(sender As Object, e As RoutedEventArgs) Handles btn_add.Click
        Dim frm_Forgeeditor As New ForgeEditor
        Dim result As Boolean?
        frm_Forgeeditor.ShowDialog()
        result = frm_Forgeeditor.DialogResult
        If result = True Then
            Load_Forge()
        End If
    End Sub

    Public Sub Load_Forge()
        lst.Items.Clear()
        For Each item As Forge.Forgeeintrag In Forge.ForgeList
            lst.Items.Add(item)
        Next
    End Sub

    Private Sub ForgeManager_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        Dim main As New Manager
        main.Show()
    End Sub

    Private Sub ForgeManager_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Load_Forge()
    End Sub

    Private Async Sub btn_delete_Click(sender As Object, e As RoutedEventArgs) Handles btn_delete.Click
        If lst.SelectedIndex = -1 Then
            MessageBox.Show("Bitte wähle eine Forge Version Aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error)
        Else
            Forge.ForgeList.RemoveAt(lst.SelectedIndex)
            Await Forge.SavetoFile(modsfile)
            Load_Forge()
        End If
    End Sub

    Private Sub btn_edit_Click(sender As Object, e As RoutedEventArgs) Handles btn_edit.Click
        If lst.SelectedIndex = -1 Then
            MessageBox.Show("Bitte wähle eine Forge Version Aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error)
        Else
            Dim frm_Forgeeditor As New ForgeEditor(DirectCast(lst.SelectedItem, Forge.Forgeeintrag), lst.SelectedIndex)
            Dim result As Boolean?
            frm_Forgeeditor.ShowDialog()
            result = frm_Forgeeditor.DialogResult
            If result = True Then
                Load_Forge()
            End If
        End If
    End Sub
End Class
