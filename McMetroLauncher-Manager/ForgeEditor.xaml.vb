Imports System.Net
Imports System.IO
Imports System.Xml.Linq
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Windows.Threading

Public Class ForgeEditor
    Public loadedindex As Integer
    Public NewForge As Boolean

    Sub Get_Versions()
        cb_versions.ItemsSource = Versions.Versions(Versions.releasetypes.release)
        If NewForge = True Then
            cb_versions.SelectedIndex = 0
        Else
            cb_versions.SelectedItem = Forge.versionAt(loadedindex)
        End If
    End Sub

    Sub Load_ModInfos()
        tb_build.Text = Forge.buildAt(loadedindex)
        tb_time.Text = Forge.timeAt(loadedindex)
        tb_downloadlink.Text = Forge.downloadlinkAt(loadedindex)
    End Sub

    'Private Sub btn_save_Click(sender As Object, e As RoutedEventArgs) Handles btn_save.Click
    '    'Überprüfen ob es eine gültige Uri ist:

    '    If tb_description.Text = Nothing Or tb_downloadlink.Text = Nothing Or tb_name.Text = Nothing Or tb_video.Text = Nothing Or tb_website.Text = Nothing Or tb_autor.Text = Nothing Then
    '        MessageBox.Show("Bitte fülle alle Felder aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
    '    Else
    '        Dim frgMod As New ForgeMod(tb_name.Text, tb_autor.Text, cb_versions.SelectedItem.ToString, tb_description.Text, tb_downloadlink.Text, tb_video.Text, tb_website.Text, lb_needed_mods.Items.Cast(Of String)().ToList)
    '        If NewMod = True Then
    '            'Shauen ob es bereits existiert
    '            If Mods.Get_Mods(cb_versions.SelectedItem.ToString).Contains(tb_name.Text.ToString) Then
    '                MessageBox.Show("Diese Mod existiert bereits!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
    '                Exit Sub
    '            Else
    '                Mods.Add(frgMod)
    '            End If
    '        Else
    '            'Mod bearbeiten
    '            Mods.Edit(loadedmodindex, loadedversion, frgMod)
    '        End If
    '    End If
    '    DialogResult = True
    '    Me.Close()
    'End Sub

    Private Sub ModEditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Get_Versions()
        If NewForge = False Then
            Load_ModInfos()
        End If
    End Sub

    Private Sub btn_save_Click(sender As Object, e As RoutedEventArgs) Handles btn_save.Click
        If tb_build.Text = Nothing Or tb_downloadlink.Text = Nothing Or tb_time.Text = Nothing Then
            MessageBox.Show("Bitte fülle alle Felder aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
        Else
            Dim frg As New ForgeEintrag(tb_build.Text, cb_versions.SelectedItem.ToString, tb_time.Text, tb_downloadlink.Text)
            If NewForge = True Then
                'Shauen ob es bereits existiert
                If Forge.Get_Forge.Select(Function(p) p.build).Contains(tb_build.Text) Then
                    MessageBox.Show("Dieses Forge Build existiert bereits!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
                    Exit Sub
                Else
                    Forge.Add(frg)
                End If
            Else
                'Mod bearbeiten
                Forge.Edit(loadedindex, frg)
            End If
        End If
        DialogResult = True
        Me.Close()
    End Sub
End Class
