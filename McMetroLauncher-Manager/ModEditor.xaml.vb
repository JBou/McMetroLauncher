Imports System.Net
Imports System.IO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Windows.Threading

Public Class ModEditor
    Private Modindex As Integer
    Private NewMod As Boolean
    Private saved As Boolean

    Public Sub New()
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Me.NewMod = True
        GlobalInfos.Moditem = New Modifications.Mod
    End Sub
    Public Sub New(Moditem As Modifications.Mod, Modindex As Integer)
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Me.Modindex = Modindex
        GlobalInfos.Moditem = Moditem
        Me.NewMod = False
    End Sub

    Sub Get_Versions()
        lb_versions.Items.Clear()
        For Each item As Modifications.Mod.Version In Moditem.versions
            lb_versions.Items.Add(item)
        Next
        If lb_versions.SelectedIndex = -1 Then
            lb_versions.SelectedIndex = 0
        End If
    End Sub

    Private Sub tb_description_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tb_description.TextChanged
        If tb_description.Text = Nothing Then
            If Moditem.descriptions.Select(Function(p) p.id).Contains(cb_description.SelectedItem.ToString) Then
                Moditem.descriptions.Remove(Moditem.descriptions.Where(Function(p) p.id = cb_description.SelectedItem.ToString).First)
            End If
        Else
            If Moditem.descriptions.Select(Function(p) p.id).Contains(cb_description.SelectedItem.ToString) Then
                For i = 0 To Moditem.descriptions.Count - 1
                    If Moditem.descriptions.Item(i).id = cb_description.SelectedItem.ToString Then
                        Moditem.descriptions.Item(i).text = tb_description.Text
                    End If
                Next
            Else
                Moditem.descriptions.Add(New Modifications.Mod.Description() With {.id = cb_description.SelectedItem.ToString, .text = tb_description.Text})
            End If
        End If
    End Sub

    Sub Load_ModInfos()
        tb_name.Text = Moditem.name
        tb_website.Text = Moditem.website
        tb_video.Text = Moditem.video
        tb_authors.Text = String.Join(", ", Moditem.authors)
        tb_id.Text = Moditem.id
        cb_type.SelectedItem = Moditem.type
        Get_Versions()
    End Sub

    Private Sub ModEditor_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        If saved = False Then
            Dim result As MessageBoxResult = MessageBox.Show("Beenden ohne zu spechern?", "Achtung", MessageBoxButton.YesNoCancel, MessageBoxImage.Information)
            If result = MessageBoxResult.No OrElse result = MessageBoxResult.Cancel Then
                e.Cancel = True
            End If
        End If
    End Sub


    Private Sub ModEditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        For Each item As String In types
            cb_type.Items.Add(item)
        Next
        For Each item As String In languages
            cb_description.Items.Add(item)
        Next
        cb_description.SelectedIndex = 0
        If NewMod = True Then
            cb_type.SelectedIndex = 0
        Else
            Load_ModInfos()
        End If
    End Sub

    Private Sub cb_description_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cb_description.SelectionChanged
        With Moditem.descriptions.Where(Function(p) p.id = cb_description.SelectedItem.ToString)
            If .Count = 0 Then
                tb_description.Text = Nothing
            Else
                tb_description.Text = .First.text
            End If
        End With
    End Sub

    Private Sub tb_author_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tb_name.TextChanged
        Moditem.name = tb_name.Text
    End Sub

    Private Sub cb_extension_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cb_type.SelectionChanged
        If cb_type.SelectedIndex <> -1 Then
            Moditem.type = cb_type.SelectedItem.ToString
        End If
    End Sub

    Private Sub tb_author_TextChanged_1(sender As Object, e As TextChangedEventArgs) Handles tb_authors.TextChanged
        Moditem.authors = tb_authors.Text.Split(CChar(", ")).ToList
    End Sub

    Private Sub tb_id_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tb_id.TextChanged
        Moditem.id = tb_id.Text
    End Sub

    Private Sub tb_website_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tb_website.TextChanged
        Moditem.website = tb_website.Text
    End Sub

    Private Sub tb_video_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tb_video.TextChanged
        Moditem.video = tb_video.Text
    End Sub

    Private Sub btn_add_needed_mod_Click(sender As Object, e As RoutedEventArgs) Handles btn_add_dependencies.Click
        Dim frm_ModVersionEditor As New ModVersionEditor()
        Dim result As Boolean?
        frm_ModVersionEditor.ShowDialog()
        result = frm_ModVersionEditor.DialogResult
        If result = True Then
            Get_Versions()
        End If
        If lb_versions.Items.Count > 0 Then
            lb_versions.SelectedIndex = 0
        End If
    End Sub

    Private Sub btn_edit_dependencies_Click(sender As Object, e As RoutedEventArgs) Handles btn_edit_dependencies.Click
        If lb_versions.SelectedIndex = -1 Then
            MessageBox.Show("Bitte wähle eine Mod Version aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error)
        Else
            Dim frm_ModVersionEditor As New ModVersionEditor(DirectCast(lb_versions.SelectedItem, Modifications.Mod.Version), lb_versions.SelectedIndex)
            Dim result As Boolean?
            frm_ModVersionEditor.ShowDialog()
            result = frm_ModVersionEditor.DialogResult
            If result = True Then
                Get_Versions()
            End If
        End If
        If lb_versions.Items.Count > 0 Then
            lb_versions.SelectedIndex = 0
        End If
    End Sub
    Private Sub btn_delete_needed_mod_Click(sender As Object, e As RoutedEventArgs) Handles btn_delete_dependencies.Click
        If lb_versions.SelectedIndex = -1 Then
            MessageBox.Show("Bitte wähle eine Mod Version aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error)
        Else
            Moditem.versions.Remove(DirectCast(lb_versions.SelectedItem, Modifications.Mod.Version))
            Get_Versions()
        End If
        If lb_versions.Items.Count > 0 Then
            lb_versions.SelectedIndex = 0
        End If
    End Sub

    Private Async Sub btn_save_Click(sender As Object, e As RoutedEventArgs) Handles btn_save.Click
        Dim allinformation As Boolean = True
        If tb_name.Text = Nothing OrElse tb_authors.Text = Nothing OrElse tb_id.Text = Nothing OrElse tb_video.Text = Nothing OrElse tb_website.Text = Nothing OrElse cb_type.SelectedIndex = -1 Then
            allinformation = False
        End If
        If lb_versions.Items.Count = 0 Then
            MessageBox.Show("Bitte füge eine Version hinzu!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
        ElseIf Moditem.descriptions.Count = 0 Then
            MessageBox.Show("Bitte füge eine Beschreibung hinzu!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
        ElseIf allinformation = False Then
            MessageBox.Show("Bitte fülle alle Felder aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
        Else
            Dim Modification As New Modifications.Mod() With {
                .descriptions = Moditem.descriptions,
                .authors = tb_authors.Text.Split(CChar(", ")).ToList,
                .id = tb_id.Text,
                .name = tb_name.Text,
                .type = cb_type.SelectedItem.ToString,
                .versions = lb_versions.Items.Cast(Of Modifications.Mod.Version)().ToList,
                .video = tb_video.Text,
                .website = tb_website.Text}
            If NewMod = True Then
                'Shauen ob es bereits existiert
                If Modifications.ModList.Select(Function(p) p.id).Contains(Modification.id) Then
                    MessageBox.Show("Diese Mod existiert bereits!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
                    Exit Sub
                Else
                    Modifications.ModList.Add(Modification)
                End If
            Else
                Modifications.ModList.Item(Modindex) = Modification
            End If
            Await Modifications.SavetoFile(New FileInfo(modsfile))
            saved = True
            Me.DialogResult = True
            Me.Close()
        End If
    End Sub
End Class


'    Public loadedmodindex As Integer
'    Public loadedversion As String
'    Public NewMod As Boolean
'    Private NoMods As String = "Es existieren derzeit keine anderen Mods"

'    Sub Get_Mods()
'        Try
'            Dim ls As IList(Of String)
'            If Mods.Get_ModVersions.Contains(cb_versions.SelectedItem.ToString) = True Then
'                ls = Mods.Get_Mods(cb_versions.SelectedItem.ToString, modsfolder).Select(Function(p) p.name.ToString).ToList
'                If NewMod = False Then
'                    ls.Remove(Mods.NameAt(loadedversion, loadedmodindex))
'                End If
'                If ls.Count = 0 Then
'                    ls.Add(NoMods)
'                End If
'            Else
'                ls = New List(Of String)
'                ls.Add(NoMods)
'            End If
'            cb_needed_mods.ItemsSource = ls
'            cb_needed_mods.SelectedIndex = 0
'        Catch
'        End Try
'    End Sub

'    Sub Get_Versions()
'        For Each item As Versionslist.Version In Versions.versions.Where(Function(p) p.type = "release")
'            cb_versions.Items.Add(item.id)
'        Next
'        If NewMod = True Then
'            cb_versions.SelectedIndex = 0
'        Else
'            cb_versions.SelectedItem = loadedversion
'        End If
'    End Sub

'    Sub Load_ModInfos()
'        tb_name.Text = Mods.NameAt(loadedversion, loadedmodindex)
'        tb_website.Text = Mods.websiteAt(loadedversion, loadedmodindex)
'        tb_video.Text = Mods.videoAt(loadedversion, loadedmodindex)
'        tb_downloadlink.Text = Mods.downloadlinkAt(loadedversion, loadedmodindex)
'        tb_description.Text = Mods.descriptionAt(loadedversion, loadedmodindex)
'        tb_author.Text = Mods.authorAt(loadedversion, loadedmodindex)
'        tb_filename.Text = Mods.idAt(loadedversion, loadedmodindex)
'        For Each item As String In Mods.needed_modsAt(loadedversion, loadedmodindex)
'            lb_needed_mods.Items.Add(item)
'        Next
'        cb_extension.SelectedItem = Mods.extensionAt(loadedversion, loadedmodindex)
'        cb_type.SelectedItem = Mods.typeAt(loadedversion, loadedmodindex)
'    End Sub

'    Private Sub btn_save_Click(sender As Object, e As RoutedEventArgs) Handles btn_save.Click
'        'Überprüfen ob es eine gültige Uri ist:

'        If tb_description.Text = Nothing Or tb_downloadlink.Text = Nothing Or tb_name.Text = Nothing Or tb_video.Text = Nothing Or tb_website.Text = Nothing Or tb_author.Text = Nothing Then
'            MessageBox.Show("Bitte fülle alle Felder aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
'        Else
'            Dim frgMod As New ForgeMod(tb_name.Text, tb_author.Text, cb_versions.SelectedItem.ToString, tb_description.Text, tb_downloadlink.Text, tb_video.Text, tb_website.Text, tb_filename.Text, cb_extension.SelectedItem.ToString, cb_type.SelectedItem.ToString, lb_needed_mods.Items.Cast(Of String)().ToList, False)
'            If NewMod = True Then
'                'Shauen ob es bereits existiert
'                If Mods.Get_Mods(cb_versions.SelectedItem.ToString, modsfolder).Select(Function(p) p.name.ToString).Contains(tb_name.Text.ToString) Then
'                    MessageBox.Show("Diese Mod existiert bereits!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
'                    Exit Sub
'                Else
'                    Mods.Add(frgMod)
'                End If
'            Else
'                'Mod bearbeiten
'                Mods.Edit(loadedmodindex, loadedversion, frgMod)
'            End If
'            DialogResult = True
'            Me.Close()
'        End If
'    End Sub

'    Public Sub versionsvisibility(ByVal Visibility As Boolean)
'        If Visibility = True Then
'            cb_versions.Visibility = Windows.Visibility.Visible
'            lbl_version.Visibility = Windows.Visibility.Hidden
'        Else
'            cb_versions.Visibility = Windows.Visibility.Hidden
'            lbl_version.Visibility = Windows.Visibility.Visible
'        End If
'    End Sub

'    Private Sub btn_add_needed_mod_Click(sender As Object, e As RoutedEventArgs) Handles btn_add_needed_mod.Click
'        If cb_needed_mods.SelectedItem.ToString <> NoMods Then
'            If lb_needed_mods.Items.Contains(cb_needed_mods.SelectedItem.ToString) = False Then
'                lb_needed_mods.Items.Add(cb_needed_mods.SelectedItem.ToString)
'            End If
'        End If
'    End Sub

'    Private Sub btn_delete_needed_mod_Click(sender As Object, e As RoutedEventArgs) Handles btn_delete_needed_mod.Click
'        If lb_needed_mods.Items.Contains(cb_needed_mods.SelectedItem.ToString) = True Then
'            lb_needed_mods.Items.Remove(cb_needed_mods.SelectedItem.ToString)
'        End If
'    End Sub

'    Private Sub ModEditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
'        cb_extension.Items.Add("jar")
'        cb_extension.Items.Add("zip")
'        cb_extension.Items.Add("litemod")
'        cb_type.Items.Add("forge")
'        cb_type.Items.Add("liteloader")
'        Get_Versions()
'        If NewMod = True Then
'            cb_versions.SelectedIndex = 0
'            cb_extension.SelectedIndex = 0
'            cb_type.SelectedIndex = 0
'        Else
'            Load_ModInfos()
'            lbl_version.Content = loadedversion
'        End If
'        Get_Mods()
'    End Sub

'    Private Sub cb_versions_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cb_versions.SelectionChanged
'        lb_needed_mods.Items.Clear()
'        Get_Mods()
'    End Sub

'    'Public Sub New(ByVal loadedmodname As String, loadedversion As String, ForgeMod As ForgeMod)

'    '    ' Dieser Aufruf ist für den Designer erforderlich.
'    '    InitializeComponent()

'    '    ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

'    '    Me.loadedversion = loadedversion

'    'End Sub