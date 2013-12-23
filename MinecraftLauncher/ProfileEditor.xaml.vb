Imports System.Net
Imports System.IO
Imports System.Xml.Linq
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Ookii.Dialogs.Wpf

Public Class ProfileEditor
    Private loadedprofile As String

    Sub Get_Versions()
        Dim o As String = File.ReadAllText(outputjsonversions)
        Dim jo As JObject = JObject.Parse(o)
        Dim i As Integer = 0
        Dim versionid As String = jo("versions").ElementAt(i).Value(Of String)("id").ToString
        'Dim versionid As String = CStr(jo.SelectToken("versions[" & i & "].id"))
        Dim versiontype As String = jo("versions").ElementAt(i).Value(Of String)("type").ToString
        'Dim versiontype As String = CStr(jo.SelectToken("versions[" & i & "].type"))
        cb_versions.Items.Clear()
        cb_versions.Items.Add("Neueste Version")

        For i = 0 To versionsidlist.Count - 1
            If versiontypelist.Item(i).ToString = "release" Then
                cb_versions.Items.Add(versiontypelist.Item(i).ToString & " " & versionsidlist.Item(i).ToString)
            ElseIf cb_snapshots.IsChecked = True Then
                If versiontypelist.Item(i).ToString = "snapshot" Then
                    cb_versions.Items.Add(versiontypelist.Item(i).ToString & " " & versionsidlist.Item(i).ToString)
                End If
                'Andere Versionen hier einfügen
            End If
        Next

        If IO.Directory.Exists(mcpfad & "\versions") = True Then
            Dim list_versionsdirectories As IEnumerable(Of String) = IO.Directory.GetDirectories(mcpfad & "\versions")
            Dim list_versions As IList(Of String) = New List(Of String)
            For Each version As String In list_versionsdirectories
                Dim versionname As String = IO.Path.GetFileName(version)
                If versionsidlist.Contains(versionname) = False Then
                    list_versions.Add(versionname)
                End If
            Next
            For Each Version As String In list_versions
                If File.Exists(mcpfad & "\versions\" & Version & "\" & Version & ".jar") And File.Exists(mcpfad & "\versions\" & Version & "\" & Version & ".json") = True Then
                    cb_versions.Items.Add("release " & Version)
                End If
            Next
        End If
        'Profiles, die nur lokal existieren in die Combobox eintragen

        If cb_versions.SelectedIndex = -1 Then
            cb_versions.SelectedIndex = 0
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
        End If
        Get_Versions()
        cb_versions.SelectedItem = Load_Versiontype() & " " & Profiles.lastVersionId(loadedprofile)
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

    Private Function Load_Versiontype() As String
        Dim outputjsonversions As String = mcpfad & "\cache\versions.json"
        Dim o As String = File.ReadAllText(outputjsonversions)
        Dim jo As JObject = JObject.Parse(o)
        Dim i As Integer = 0
        Dim versionid As String = CStr(jo.SelectToken("versions[" & i & "].id"))
        Dim versiontype As String = CStr(jo.SelectToken("versions[" & i & "].type"))

        Do Until versionid = Nothing
            If versionid = Profiles.lastVersionId(loadedprofile) Then
                Return versiontype
            End If
            i = i + 1
            versionid = CStr(jo.SelectToken("versions[" & i & "].id"))
            versiontype = CStr(jo.SelectToken("versions[" & i & "].type"))
        Loop
        '*Keine übereinstimmung
        Return "release"
    End Function

    Sub StandartValues()
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
            StandartValues()
        Else
            Load_ProfileInfos()
        End If
        Check_cb_Status()
    End Sub

    Private Sub cb_snapshots_Click(sender As Object, e As RoutedEventArgs) Handles cb_snapshots.Click
        Get_Versions()
    End Sub

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
                Dim selectedversion As String = cb_versions.SelectedItem.ToString
                ' String to search in.
                Dim SearchString As String = selectedversion
                ' Search for "P".
                Dim SearchChar As String = " "
                Dim leertastenindex As Integer
                ' A textual comparison starting at position 4. Returns 6.
                leertastenindex = InStr(1, SearchString, SearchChar, CompareMethod.Text)
                selectedversion = Mid(selectedversion, leertastenindex + 1, selectedversion.Length)
                lastVersionId = selectedversion
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

            If cb_snapshots.IsChecked Then 'Or cb_alpha.IsChecked...
                If cb_snapshots.IsChecked = True Then
                    allowedReleaseTypes.Add("snapshot")
                End If
                'If cb_alpha.IsChecked = True Then
                '    allowedReleaseTypes.Add("old_alpha")
                'End If

                'Release wird in der ProfileClass automatisch eingefügt, falls die liste nicht nothing ist

                'Andere Typen z.B. alpha einfügen
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

End Class
