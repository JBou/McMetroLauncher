Imports System.Net
Imports System.IO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Windows.Threading

Public Class ForgeEditor
    Private loadedindex As Integer
    Private NewForge As Boolean
    Private ForgeEintrag As Forge.Forgeeintrag

    Sub Get_Versions()
        For Each item As Versionslist.Version In Versions.versions.Where(Function(p) p.type = "release")
            cb_versions.Items.Add(item.id)
        Next
        If NewForge = True Then
            cb_versions.SelectedIndex = 0
        Else
            cb_versions.SelectedItem = Versions.versions.Where(Function(p) p.id = ForgeEintrag.version).First.id
        End If
    End Sub

    Sub Load_ModInfos()
        tb_build.Text = ForgeEintrag.build.ToString
        tb_time.Text = ForgeEintrag.time
        tb_downloadlink.Text = ForgeEintrag.downloadlink
    End Sub

    'Private Sub btn_save_Click(sender As Object, e As RoutedEventArgs) Handles btn_save.Click
    '    'Überprüfen ob es eine gültige Uri ist:

    '    If tb_description.Text = Nothing Or tb_downloadlink.Text = Nothing Or tb_name.Text = Nothing Or tb_video.Text = Nothing Or tb_website.Text = Nothing Or tb_author.Text = Nothing Then
    '        MessageBox.Show("Bitte fülle alle Felder aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
    '    Else
    '        Dim frgMod As New ForgeMod(tb_name.Text, tb_author.Text, cb_versions.SelectedItem.ToString, tb_description.Text, tb_downloadlink.Text, tb_video.Text, tb_website.Text, lb_needed_mods.Items.Cast(Of String)().ToList)
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

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Me.NewForge = True
    End Sub

    Public Sub New(ForgeEintrag As Forge.Forgeeintrag, loadedindex As Integer)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Me.NewForge = False
        Me.ForgeEintrag = ForgeEintrag
        Me.loadedindex = loadedindex
    End Sub

    Private Sub ModEditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Get_Versions()
        If NewForge = False Then
            Load_ModInfos()
        End If
    End Sub

    Private Async Sub btn_save_Click(sender As Object, e As RoutedEventArgs) Handles btn_save.Click
        If tb_build.Text = Nothing OrElse tb_downloadlink.Text = Nothing OrElse tb_time.Text = Nothing Then
            MessageBox.Show("Bitte fülle alle Felder aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
        Else
            Dim frg As New Forge.Forgeeintrag() With {
                .build = tb_build.Text,
                .version = cb_versions.SelectedItem.ToString,
                .time = tb_time.Text,
                .downloadlink = tb_downloadlink.Text}
            If NewForge = True Then
                'Shauen ob es bereits existiert
                If Forge.ForgeList.Select(Function(p) p.build).Contains(tb_build.Text) Then
                    MessageBox.Show("Dieses Forge Build existiert bereits!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
                    Exit Sub
                Else
                    Forge.ForgeList.Add(frg)
                End If
            Else
                Forge.ForgeList.Item(loadedindex) = frg
            End If
            Await Forge.SavetoFile(modsfile)
            DialogResult = True
            Me.Close()
        End If
    End Sub
End Class
