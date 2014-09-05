Imports System.Drawing
Imports System.IO
Imports System.Windows.Media

Public Class ModManager

    Private Sub ModManager_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        Dim main As New Manager
        main.Show()
    End Sub

    Private Async Sub ModManager_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Await Load_Mods()
    End Sub

    Private Async Function Load_Mods() As Task
        lb_mods.Items.Clear()
        Await Modifications.Load()
        For Each item As Modifications.Mod In Modifications.ModList
            lb_mods.Items.Add(item)
        Next
        lb_mods.SelectedIndex = 0
        'Für jede version eine Spalte hinzufügen
        For Each item As String In Modifications.List_all_Mod_Vesions.Reverse.ToList
            If DirectCast(lb_mods.View, GridView).Columns.Select(Function(p) p.Header).Contains(item) = False Then
                'Declare Binding
                Dim binding As New Binding
                binding.Path = New PropertyPath("versions")
                binding.Converter = New Versions_Image_Converter
                binding.ConverterParameter = item
                'Declare Celltemplate
                Dim Img = New FrameworkElementFactory(GetType(Controls.Image))
                Img.Name = "img"
                Img.SetBinding(Controls.Image.SourceProperty, New Binding("versions") With {.Converter = New Versions_Image_Converter, .ConverterParameter = item})
                Img.SetValue(Controls.Image.HeightProperty, 25.0)
                Img.SetValue(Controls.Image.MarginProperty, New Thickness(0.0))
                Dim DataTemplate As New DataTemplate() With {.VisualTree = Img}
                DirectCast(lb_mods.View, GridView).Columns.Add(New GridViewColumn() With {.Header = item, .CellTemplate = DataTemplate, .Width = 35})
            End If
        Next
    End Function

    'Private Async Sub btn_add_Click(sender As Object, e As RoutedEventArgs) Handles btn_add.Click
    '    Dim ModsEditor As New ModEditor
    '    Dim result As Boolean?
    '    ModsEditor.loadedmodindex = Nothing
    '    ModsEditor.loadedversion = Nothing
    '    ModsEditor.NewMod = True
    '    ModsEditor.versionsvisibility(True)
    '    ModsEditor.ShowDialog()
    '    result = ModsEditor.DialogResult
    '    If result = True Then
    '        Await Load_Mods()
    '    End If
    'End Sub

    '    Private Async Sub btn_edit_Click(sender As Object, e As RoutedEventArgs) Handles btn_edit.Click
    '        Dim ModsEditor As New ModEditor
    '        Dim result As Boolean?
    '        ModsEditor.loadedmodindex = Mods.Name(DirectCast(lb_mods.SelectedItem, ForgeMod).name, cb_modversions.SelectedItem.ToString)
    '        ModsEditor.loadedversion = cb_modversions.SelectedItem.ToString
    '        ModsEditor.NewMod = False
    '        ModsEditor.versionsvisibility(False)
    '        ModsEditor.ShowDialog()
    '        result = ModsEditor.DialogResult
    '        If result = True Then
    '            Await Load_Mods()
    '        End If
    '    End Sub

    '    Private Sub lb_mods_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lb_mods.SelectionChanged
    '        'Load_Modinfos
    '        If lb_mods.SelectedIndex <> -1 Then
    '            lbl_name.Content = DirectCast(lb_mods.SelectedItem, ForgeMod).name
    '            tb_description.Text = DirectCast(lb_mods.SelectedItem, ForgeMod).description
    '            lbl_website.Content = DirectCast(lb_mods.SelectedItem, ForgeMod).Website
    '            lbl_video.Content = DirectCast(lb_mods.SelectedItem, ForgeMod).video
    '            lbl_downloadlink.Content = DirectCast(lb_mods.SelectedItem, ForgeMod).Downloadlink
    '            lbl_author.Content = DirectCast(lb_mods.SelectedItem, ForgeMod).author
    '            lb_needed_mods.Items.Clear()
    '            For Each item As String In DirectCast(lb_mods.SelectedItem, ForgeMod).needed_mods
    '                lb_needed_mods.Items.Add(item)
    '            Next
    '            lbl_type.Content = "Type: " & DirectCast(lb_mods.SelectedItem, ForgeMod).type
    '        End If
    '        'If lb_mods.SelectedIndex <> -1 Then
    '        '    lbl_name.Content = Mods.NameAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
    '        '    tb_description.Text = Mods.descriptionAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
    '        '    lbl_website.Content = Mods.websiteAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
    '        '    lbl_video.Content = Mods.videoAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
    '        '    lbl_downloadlink.Content = Mods.downloadlinkAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
    '        '    lbl_author.Content = Mods.authorAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
    '        '    lb_needed_mods.Items.Clear()
    '        '    For Each item As String In Mods.needed_modsAt(cb_modversions.SelectedItem.ToString, lb_mods.SelectedIndex)
    '        '        lb_needed_mods.Items.Add(item)
    '        '    Next
    '        'End If
    '    End Sub

    '    Private Sub btn_delete_Click(sender As Object, e As RoutedEventArgs) Handles btn_delete.Click
    '        Mods.RemoveAt(cb_modversions.SelectedItem.ToString, Mods.Name(DirectCast(lb_mods.SelectedItem, ForgeMod).name, cb_modversions.SelectedItem.ToString))
    '        Load_Versions()
    '        Load_Mods()
    '    End Sub

    Private Sub lb_mods_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lb_mods.SelectionChanged
        If lb_mods.SelectedIndex <> -1 Then
            lbl_name.Content = DirectCast(lb_mods.SelectedItem, Modifications.Mod).name
            'tb_description.Text = DirectCast(lb_mods.SelectedItem, ForgeMod).description
            lbl_website.Content = DirectCast(lb_mods.SelectedItem, Modifications.Mod).website
            lbl_video.Content = DirectCast(lb_mods.SelectedItem, Modifications.Mod).video
            lbl_authors.Content = String.Join(", ", DirectCast(lb_mods.SelectedItem, Modifications.Mod).authors)
            lbl_ID.Content = DirectCast(lb_mods.SelectedItem, Modifications.Mod).id
            lbl_type.Content = DirectCast(lb_mods.SelectedItem, Modifications.Mod).type
            lb_versions.Items.Clear()
            For Each item As Modifications.Mod.Version In DirectCast(lb_mods.SelectedItem, Modifications.Mod).versions
                lb_versions.Items.Add(item)
            Next
            lb_versions.SelectedIndex = 0
            cb_descriptions.Items.Clear()
            For Each item As Modifications.Mod.Description In DirectCast(lb_mods.SelectedItem, Modifications.Mod).descriptions
                cb_descriptions.Items.Add(item)
            Next
            cb_descriptions.SelectedIndex = 0
        End If
    End Sub

    Private Sub cb_descriptions_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cb_descriptions.SelectionChanged
        If cb_descriptions.SelectedIndex <> -1 Then
            tb_description.Text = DirectCast(cb_descriptions.SelectedItem, Modifications.Mod.Description).text
        End If
    End Sub

    Private Async Sub btn_edit_Click(sender As Object, e As RoutedEventArgs) Handles btn_edit.Click
        If lb_mods.SelectedIndex = -1 Then
            MessageBox.Show("Bitte wähle eine Mod aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error)
        Else
            Dim frm_ModEditor As New ModEditor(DirectCast(lb_mods.SelectedItem, Modifications.Mod), lb_mods.SelectedIndex)
            Dim result As Boolean?
            frm_ModEditor.ShowDialog()
            result = frm_ModEditor.DialogResult
            If result = True Then
                Await Load_Mods()
            End If
        End If
    End Sub

    Private Async Sub btn_add_Click(sender As Object, e As RoutedEventArgs) Handles btn_add.Click
        If lb_mods.SelectedIndex = -1 Then
            MessageBox.Show("Bitte wähle eine Mod aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error)
        Else
            Dim frm_ModEditor As New ModEditor()
            Dim result As Boolean?
            frm_ModEditor.ShowDialog()
            result = frm_ModEditor.DialogResult
            If result = True Then
                Await Load_Mods()
            End If
        End If
    End Sub

    Private Async Sub btn_delete_Click(sender As Object, e As RoutedEventArgs) Handles btn_delete.Click
        If lb_mods.SelectedIndex = -1 Then
            MessageBox.Show("Bitte wähle eine Mod aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error)
        Else
            Dim moditem As Modifications.Mod = DirectCast(lb_mods.SelectedItem, Modifications.Mod)
            Dim result As MessageBoxResult = MessageBox.Show("Möchtest du wirklich die Mod " & Chr(36) & moditem.name & Chr(36) & " löschen?", "Achtung", MessageBoxButton.YesNoCancel, MessageBoxImage.Information)
            If result = MessageBoxResult.Yes Then
                Modifications.ModList.Remove(Modifications.ModList.Where(Function(p) p.id = moditem.id).First)
                Await Modifications.SavetoFile(New FileInfo(modsfile))
            End If
        End If
    End Sub
End Class


Public Class Dependencies_String_Converter
    Implements System.Windows.Data.IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Dim s As IList(Of String) = TryCast(value, IList(Of String))
        If s Is Nothing OrElse Not s.Count > 0 Then Return Nothing
        Dim dependecies As String = String.Join(Environment.NewLine, s)
        Dim returnstring As String = String.Join(Environment.NewLine, "Dependencies:", dependecies)
        Return returnstring
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function

End Class

Public Class Versions_Image_Converter
    Implements System.Windows.Data.IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Dim s As IList(Of Modifications.Mod.Version) = TryCast(value, IList(Of Modifications.Mod.Version))
        If s Is Nothing Then Return Nothing
        Dim param As String = TryCast(parameter, String)
        If s.Select(Function(p) p.version).Contains(param) Then
            Return ImageConvert.GetImageStream(My.Resources.check_green)
        Else
            Return Nothing
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function

End Class

Public Structure ImageConvert
    Public Shared Function GetImageStream(Image As System.Drawing.Image) As BitmapSource
        Dim ms As New MemoryStream()
        Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
        ms.Position = 0
        Dim bi As New BitmapImage()
        bi.BeginInit()
        bi.StreamSource = ms
        bi.EndInit()

        Return bi
    End Function
End Structure