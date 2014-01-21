Imports System.Net
Imports System.IO
Imports System.Xml.Linq
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Windows.Threading

Public Class ModEditor
    Public loadedmodindex As Integer
    Public loadedversion As String
    Public NewMod As Boolean
    Private NoMods As String = "Es existieren derzeit keine anderen Mods"

    Sub Get_Mods()
        Try
            Dim ls As IList(Of String)
            If Mods.Get_ModVersions.Contains(cb_versions.SelectedItem.ToString) = True Then
                ls = Mods.Get_Mods(cb_versions.SelectedItem.ToString, modsfolder).Select(Function(p) p.name.ToString).ToList
                If NewMod = False Then
                    ls.Remove(Mods.NameAt(loadedversion, loadedmodindex))
                End If
                If ls.Count = 0 Then
                    ls.Add(NoMods)
                End If
            Else
                ls = New List(Of String)
                ls.Add(NoMods)
            End If
            cb_needed_mods.ItemsSource = ls
            cb_needed_mods.SelectedIndex = 0
        Catch
        End Try
    End Sub

    Sub Get_Versions()
        cb_versions.ItemsSource = Versions.Versions(Versions.releasetypes.release)
        If NewMod = True Then
            cb_versions.SelectedIndex = 0
        Else
            cb_versions.SelectedItem = loadedversion
        End If
    End Sub

    Sub Load_ModInfos()
        tb_name.Text = Mods.NameAt(loadedversion, loadedmodindex)
        tb_website.Text = Mods.websiteAt(loadedversion, loadedmodindex)
        tb_video.Text = Mods.videoAt(loadedversion, loadedmodindex)
        tb_downloadlink.Text = Mods.downloadlinkAt(loadedversion, loadedmodindex)
        tb_description.Text = Mods.descriptionAt(loadedversion, loadedmodindex)
        tb_autor.Text = Mods.AutorAt(loadedversion, loadedmodindex)
        tb_filename.Text = Mods.idAt(loadedversion, loadedmodindex)
        For Each item As String In Mods.needed_modsAt(loadedversion, loadedmodindex)
            lb_needed_mods.Items.Add(item)
        Next
        cb_extension.SelectedItem = Mods.extensionAt(loadedversion, loadedmodindex)
        cb_type.SelectedItem = Mods.typeAt(loadedversion, loadedmodindex)
    End Sub

    Private Sub btn_save_Click(sender As Object, e As RoutedEventArgs) Handles btn_save.Click
        'Überprüfen ob es eine gültige Uri ist:

        If tb_description.Text = Nothing Or tb_downloadlink.Text = Nothing Or tb_name.Text = Nothing Or tb_video.Text = Nothing Or tb_website.Text = Nothing Or tb_autor.Text = Nothing Then
            MessageBox.Show("Bitte fülle alle Felder aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
        Else
            Dim frgMod As New ForgeMod(tb_name.Text, tb_autor.Text, cb_versions.SelectedItem.ToString, tb_description.Text, tb_downloadlink.Text, tb_video.Text, tb_website.Text, tb_filename.Text, cb_extension.SelectedItem.ToString, cb_type.SelectedItem.ToString, lb_needed_mods.Items.Cast(Of String)().ToList, False)
            If NewMod = True Then
                'Shauen ob es bereits existiert
                If Mods.Get_Mods(cb_versions.SelectedItem.ToString, modsfolder).Select(Function(p) p.name.ToString).Contains(tb_name.Text.ToString) Then
                    MessageBox.Show("Diese Mod existiert bereits!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
                    Exit Sub
                Else
                    Mods.Add(frgMod)
                End If
            Else
                'Mod bearbeiten
                Mods.Edit(loadedmodindex, loadedversion, frgMod)
            End If
            DialogResult = True
            Me.Close()
        End If
    End Sub

    Public Sub versionsvisibility(ByVal Visibility As Boolean)
        If Visibility = True Then
            cb_versions.Visibility = Windows.Visibility.Visible
            lbl_version.Visibility = Windows.Visibility.Hidden
        Else
            cb_versions.Visibility = Windows.Visibility.Hidden
            lbl_version.Visibility = Windows.Visibility.Visible
        End If
    End Sub

    Private Sub btn_add_needed_mod_Click(sender As Object, e As RoutedEventArgs) Handles btn_add_needed_mod.Click
        If cb_needed_mods.SelectedItem.ToString <> NoMods Then
            If lb_needed_mods.Items.Contains(cb_needed_mods.SelectedItem.ToString) = False Then
                lb_needed_mods.Items.Add(cb_needed_mods.SelectedItem.ToString)
            End If
        End If
    End Sub

    Private Sub btn_delete_needed_mod_Click(sender As Object, e As RoutedEventArgs) Handles btn_delete_needed_mod.Click
        If lb_needed_mods.Items.Contains(cb_needed_mods.SelectedItem.ToString) = True Then
            lb_needed_mods.Items.Remove(cb_needed_mods.SelectedItem.ToString)
        End If
    End Sub

    Private Sub ModEditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        cb_extension.Items.Add("jar")
        cb_extension.Items.Add("zip")
        cb_extension.Items.Add("litemod")
        cb_type.Items.Add("forge")
        cb_type.Items.Add("liteloader")
        Get_Versions()
        If NewMod = True Then
            cb_versions.SelectedIndex = 0
            cb_extension.SelectedIndex = 0
            cb_type.SelectedIndex = 0
        Else
            Load_ModInfos()
            lbl_version.Content = loadedversion
        End If
        Get_Mods()
    End Sub

    Private Sub cb_versions_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cb_versions.SelectionChanged
        lb_needed_mods.Items.Clear()
        Get_Mods()
    End Sub

    'Public Sub New(ByVal loadedmodname As String, loadedversion As String, ForgeMod As ForgeMod)

    '    ' Dieser Aufruf ist für den Designer erforderlich.
    '    InitializeComponent()

    '    ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

    '    Me.loadedversion = loadedversion

    'End Sub

End Class
