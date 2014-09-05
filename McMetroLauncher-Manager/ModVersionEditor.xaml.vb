Imports System.Net
Imports System.IO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Windows.Threading

Public Class ModVersionEditor
    Private loadedindex As Integer
    Private NewVersion As Boolean
    Private modVersion As Modifications.Mod.Version

    Sub Get_Versions()
        For Each item As Versionslist.Version In Versions.versions.Where(Function(p) p.type = "release")
            cb_versions.Items.Add(item.id)
        Next
        If NewVersion = True Then
            cb_versions.SelectedIndex = 0
        Else
            cb_versions.SelectedItem = Versions.versions.Where(Function(p) p.id = modVersion.version).First.id
        End If
    End Sub
    Sub Get_Dependencies()
        cb_dependencies.Items.Clear()
        Dim idlist As IList(Of String) = Modifications.ModList.Where(Function(p) p.versions.Select(Function(v) v.version).Contains(cb_versions.SelectedItem.ToString)).Select(Function(i) i.id).ToList
        If idlist.Count = 0 Then
            btn_add_dependecies.IsEnabled = False
            btn_delete_dependecies.IsEnabled = False
        Else
            For Each item As String In idlist
                cb_dependencies.Items.Add(item)
            Next
            btn_add_dependecies.IsEnabled = True
            btn_delete_dependecies.IsEnabled = True
        End If
        cb_dependencies.SelectedIndex = 0
        If lb_depedencies.Items.Count > 0 Then
            lb_depedencies.SelectedIndex = 0
        End If
    End Sub

    Sub Load_VersionInfos()
        lb_depedencies.Items.Clear()
        tb_downloadlink.Text = modVersion.downloadlink
        lb_depedencies.Items.Clear()
        cb_extension.SelectedItem = modVersion.extension
    End Sub

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Me.NewVersion = True
    End Sub

    Public Sub New(ModVersion As Modifications.Mod.Version, loadedindex As Integer)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Me.NewVersion = False
        Me.modVersion = ModVersion
        Me.loadedindex = loadedindex
    End Sub

    Private Sub ModEditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Get_Versions()
        Get_Dependencies()
        For Each item As String In extensions
            cb_extension.Items.Add(item)
        Next
        If NewVersion = False Then
            Load_VersionInfos()
            If modVersion.dependencies IsNot Nothing Then
                For Each dependency As String In modVersion.dependencies
                    lb_depedencies.Items.Add(dependency)
                Next
            End If
        Else
            cb_extension.SelectedIndex = 0
        End If
    End Sub

    Private Sub btn_save_Click(sender As Object, e As RoutedEventArgs) Handles btn_save.Click
        If cb_versions.SelectedIndex = -1 OrElse tb_downloadlink.Text = Nothing Then
            MessageBox.Show("Bitte fülle alle Felder aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
        Else
            modVersion = New Modifications.Mod.Version With {
                .version = cb_versions.SelectedItem.ToString,
                .downloadlink = tb_downloadlink.Text,
                .dependencies = lb_depedencies.Items.Cast(Of String)().ToList,
                .extension = cb_extension.SelectedItem.ToString}
            If NewVersion = True Then
                'Shauen ob es bereits existiert
                If Moditem.versions.Select(Function(p) p.version).Contains(cb_versions.SelectedItem.ToString) Then
                    MessageBox.Show("Dieses Mod Version existiert bereits!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Information)
                    Exit Sub
                Else
                    Moditem.versions.Add(modVersion)
                End If
            Else
                Moditem.versions.Item(loadedindex) = modVersion
            End If
            DialogResult = True
            Me.Close()
        End If
    End Sub

    Private Sub cb_versions_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cb_versions.SelectionChanged
        lb_depedencies.Items.Clear()
        Get_Dependencies()
    End Sub

    Private Sub btn_add_dependecies_Click(sender As Object, e As RoutedEventArgs) Handles btn_add_dependecies.Click
        If lb_depedencies.Items.Contains(cb_dependencies.SelectedItem.ToString) = False Then
            lb_depedencies.Items.Add(cb_dependencies.SelectedItem.ToString)
        End If
        If lb_depedencies.Items.Count > 0 Then
            lb_depedencies.SelectedIndex = 0
        End If
    End Sub

    Private Sub btn_delete_dependecies_Click(sender As Object, e As RoutedEventArgs) Handles btn_delete_dependecies.Click
        If lb_depedencies.SelectedIndex = -1 Then
            MessageBox.Show("Bitte wähle eine Abhängigkeit aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error)
        Else
            lb_depedencies.Items.RemoveAt(lb_depedencies.SelectedIndex)
        End If
        If lb_depedencies.Items.Count > 0 Then
            lb_depedencies.SelectedIndex = 0
        End If
    End Sub
End Class
