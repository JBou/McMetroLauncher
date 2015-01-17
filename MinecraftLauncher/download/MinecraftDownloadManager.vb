Imports System.IO
Imports System.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports McMetroLauncher.Forge
Imports System.Runtime.ExceptionServices

Public Class MinecraftDownloadManager

    Private Shared WithEvents wcversionsdownload As New System.Net.WebClient
    Private Shared WithEvents wcversionsjsondownload As New System.Net.WebClient
    Private Shared WithEvents wclibraries As New System.Net.WebClient
    Private Shared WithEvents wcresources As New System.Net.WebClient

#Region "Versions"

    Private Shared Async Function DownloadJar(resolved As VersionInfo) As Task(Of Boolean)
        Return Await Task.Run(Async Function()
                                  Dim VersionsURl As String = "https://s3.amazonaws.com/Minecraft.Download/versions/" & resolved.getJar & "/" & resolved.getJar & ".jar"
                                  Dim Outputfile As New FileInfo(Path.Combine(versionsfolder.FullName, resolved.getJar, resolved.getJar & ".jar"))
                                  Dim CacheOutputfile As New FileInfo(Path.Combine(cachefolder.FullName, "versions", resolved.getJar, resolved.getJar & ".jar"))

                                  Main.Write(Application.Current.FindResource("CheckVersionUpToDate").ToString)
                                  Dim todownload As Boolean = False
                                  If Outputfile.Exists = False Then
                                      todownload = True
                                  Else
                                      If Versions.versions.Where(Function(p) p.custom = False).Select(Function(p) p.id).Contains(resolved.id) Then
                                          Try
                                              Dim Request As HttpWebRequest = DirectCast(WebRequest.Create(VersionsURl), HttpWebRequest)
                                              Request.Timeout = 5000
                                              Dim Response As WebResponse = Await Request.GetResponseAsync
                                              Dim etag As String = Response.Headers("ETag")
                                              Response.Close()
                                              Dim md5 As String = Helpers.MD5FileHash(Outputfile.FullName).ToLower
                                              If etag <> Chr(34) & md5 & Chr(34) Then
                                                  todownload = True
                                              End If
                                          Catch
                                              todownload = False
                                          End Try
                                      Else
                                          todownload = False
                                      End If
                                  End If
                                  If todownload = True Then
                                      Main.Write(String.Format(Application.Current.FindResource("DownloadingMinecraftVersion").ToString, resolved.getJar))
                                      Try
                                          If CacheOutputfile.Directory.Exists = False Then
                                              CacheOutputfile.Directory.Create()
                                          End If
                                          wcversionsdownload = New WebClient
                                          Await wcversionsdownload.DownloadFileTaskAsync(New Uri(VersionsURl), CacheOutputfile.FullName)

                                          If Outputfile.Exists Then
                                              Outputfile.Delete()
                                          End If
                                          If Outputfile.Directory.Exists = False Then
                                              Outputfile.Directory.Create()
                                          End If
                                          CacheOutputfile.MoveTo(Outputfile.FullName)
                                      Catch ex As Exception
                                          Main.Write(String.Format(Application.Current.FindResource("ErrorDownloadingMinecraftVersion").ToString, resolved.getJar) & ":" & Environment.NewLine & ex.Message & Environment.NewLine & ex.StackTrace, MainWindow.LogLevel.ERROR)
                                          Return False
                                      End Try
                                  End If
                                  Return True
                              End Function)
    End Function

    Private Shared Sub wcversionsdownload_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles wcversionsdownload.DownloadProgressChanged
        MainWindowViewModel.Instance.pb_download_Value = e.ProgressPercentage
    End Sub

    Public Shared Async Function DownloadVersion(version As Versionslist.Version) As Task(Of Boolean)
        Dim VersionsJSONURL As String = "https://s3.amazonaws.com/Minecraft.Download/versions/" & version.id & "/" & version.id & ".json"
        Dim OutputfileJSON As New FileInfo(Path.Combine(versionsfolder.FullName, version.id, version.id & ".json"))
        Dim CacheOutputfileJSON As New FileInfo(Path.Combine(cachefolder.FullName, "versions", version.id, version.id & ".json"))
        Dim Request As HttpWebRequest = DirectCast(WebRequest.Create(VersionsJSONURL), HttpWebRequest)
        Request.Timeout = 5000
        'Json File download
        Dim jsontodownload As Boolean = False
        If OutputfileJSON.Exists = False Then
            jsontodownload = True
        Else
            If Versions.versions.Where(Function(p) p.custom = False).Select(Function(p) p.id).Contains(version.id) Then
                Try
                    Dim Response As WebResponse = Await Request.GetResponseAsync
                    Dim etag As String = Response.Headers("ETag")
                    Response.Close()
                    Dim md5 As String = Helpers.MD5FileHash(OutputfileJSON.FullName).ToLower
                    If etag <> Chr(34) & md5 & Chr(34) Then
                        jsontodownload = True
                    End If
                Catch
                    jsontodownload = False
                End Try
            Else
                jsontodownload = False
            End If
        End If
        If jsontodownload = True Then
            If CacheOutputfileJSON.Directory.Exists = False Then
                CacheOutputfileJSON.Directory.Create()
            End If
            If CacheOutputfileJSON.Exists Then
                CacheOutputfileJSON.Delete()
            End If
            Try
                wcversionsjsondownload = New WebClient
                Await wcversionsjsondownload.DownloadFileTaskAsync(New Uri(VersionsJSONURL), CacheOutputfileJSON.FullName)
                If OutputfileJSON.Exists Then
                    OutputfileJSON.Delete()
                End If
                If OutputfileJSON.Directory.Exists = False Then
                    OutputfileJSON.Directory.Create()
                End If
                CacheOutputfileJSON.MoveTo(OutputfileJSON.FullName)
            Catch ex As Exception
                Main.Write(String.Format(Application.Current.FindResource("ErrorDownloadingMinecraftVersion").ToString, version.id) & ":" & Environment.NewLine & ex.Message & Environment.NewLine & ex.StackTrace, MainWindow.LogLevel.ERROR)
                Return False
            End Try
        End If
        If Await DownloadJar(Await ParseVersionsInfo(version)) Then
            Return True
        End If
        Return False
    End Function

    Public Shared Async Function ParseVersionsInfo(Version As Versionslist.Version) As Task(Of VersionInfo)
        Dim o As String = File.ReadAllText(versionsJSON(Version))
        Dim javaarch As Integer = Await MainWindow.GetJavaArch()
        o = o.Replace("${arch}", javaarch.ToString)
        'Converter because some Forge Versions(10.12.0.1054) has Double as minimumlauncherversion, expected integer. It rounds the double to an integer
        Return Await JsonConvert.DeserializeObjectAsync(Of VersionInfo)(o, New JsonSerializerSettings() With {.NullValueHandling = NullValueHandling.Ignore, .Converters = New List(Of JsonConverter) From {New CustomIntConverter()}})
    End Function

#End Region

#Region "Libraries"

    Public Shared Async Function DownloadLibraries(versionInfo As VersionInfo) As Task(Of Boolean)
        'http://wiki.vg/Game_Files
        Return Await StartLibrariesDownload(0, 1, 0, versionInfo) 'TODO resolve
    End Function

    Private Shared Async Function StartLibrariesDownload(index As Integer, dltry As Integer, failures As Integer, resolved As VersionInfo) As Task(Of Boolean)
        MainWindowViewModel.Instance.pb_download_Value = index / resolved.libraries.Count * 100
        If index < resolved.libraries.Count Then
            Try
                Dim Currentlibrary As Library = resolved.libraries.Item(index)
                Dim allowdownload As Boolean = True
                If Currentlibrary.rules Is Nothing Then
                    allowdownload = True
                Else
                    If Currentlibrary.rules.Select(Function(p) p.action).Contains("allow") Then
                        If Currentlibrary.rules.Where(Function(p) p.action = "allow").First.os IsNot Nothing Then
                            If Currentlibrary.rules.Where(Function(p) p.action = "allow").First.os.name = "windows" Then
                                allowdownload = True
                            Else
                                allowdownload = False
                            End If
                        End If
                    ElseIf Currentlibrary.rules.Select(Function(p) p.action).Contains("disallow") Then
                        If Currentlibrary.rules.Where(Function(p) p.action = "disallow").First.os IsNot Nothing Then
                            If Currentlibrary.rules.Where(Function(p) p.action = "disallow").First.os.name = "windows" Then
                                allowdownload = False
                            Else
                                allowdownload = True
                            End If
                        End If
                    End If
                End If
                If allowdownload = True Then
                    Dim todownload As Boolean = True
                    Dim url As String = Nothing
                    If resolved.libraries.ElementAt(index).url = Nothing Then
                        url = librariesurl & Currentlibrary.path
                    Else
                        Dim customurl As String = resolved.libraries.ElementAt(index).url
                        url = customurl & Currentlibrary.path
                    End If
                    Dim librarypath As New FileInfo(IO.Path.Combine(librariesfolder.FullName, Currentlibrary.path))
                    Dim a As New WebClient()
                    If librarypath.Directory.Exists = False Then
                        librarypath.Directory.Create()
                    End If
                    Dim sha1filehashPath As FileInfo = New FileInfo(librarypath.FullName & ".sha1")
                    Dim hashexisted As Boolean = True
                    Dim Currentlibrarysha1 As String
                    If Not sha1filehashPath.Exists Then
                        hashexisted = False
                        Try
                            Await a.DownloadFileTaskAsync(New Uri(url & ".sha1"), sha1filehashPath.FullName)
                        Catch e As Exception
                            Currentlibrarysha1 = Nothing
                        End Try
                    End If
                    Currentlibrarysha1 = File.ReadAllText(sha1filehashPath.FullName)
                    If librarypath.Exists Then
                        If String.IsNullOrWhiteSpace(Currentlibrarysha1) Then
                            todownload = False
                        Else
                            If Helpers.SHA1FileHash(librarypath.FullName).ToLower = Currentlibrarysha1 Then
                                todownload = False
                            Else
                                todownload = True
                            End If
                        End If
                    Else
                        todownload = True
                    End If
                    If todownload = True Then
                        'Download
                        If librarypath.Directory.Exists = False Then
                            librarypath.Directory.Create()
                        End If
                        'Falls es ein MinecraftForge Build ist, url ändern
                        Dim downloadforgelib As Boolean = False
                        Dim forgeuniversal As Boolean = False
                        Dim version As String = Currentlibrary.name.Split(CChar(":"))(2)
                        'legacy = "minecraftforge", new versions = "forge"
                        If Currentlibrary.name.Split(CChar(":"))(1) = "minecraftforge" AndAlso Forge.ForgeList.Select(Function(p) p.version).Contains(version) OrElse Currentlibrary.name.Split(CChar(":"))(1) = "forge" AndAlso Forge.ForgeList.Select(Function(p) p.mcversion & "-" & p.version & IIf(p.branch = Nothing, "", "-" & p.branch).ToString).Contains(version) Then
                            Dim build As ForgeBuild
                            If Currentlibrary.name.Split(CChar(":"))(1) = "minecraftforge" Then
                                build = Forge.ForgeList.Where(Function(p) p.version = version).First
                            Else
                                build = Forge.ForgeList.Where(Function(p) (p.mcversion & "-" & p.version & IIf(p.branch = Nothing, "", "-" & p.branch).ToString) = version).First
                            End If
                            'Buildlist durchsuchen
                            downloadforgelib = True
                            forgeuniversal = True
                            Dim branch As String = IIf(build.branch = Nothing, "", "-" & build.branch).ToString
                            If Forge.LegacyBuildList.Select(Function(p) p.version).Contains(version) Then
                                url = String.Format("http://files.minecraftforge.net/minecraftforge/minecraftforge-universal-{1}-{0}.jar", version, build.mcversion)
                            Else
                                If Currentlibrary.name.Split(CChar(":"))(1) = "minecraftforge" Then
                                    url = String.Format("http://files.minecraftforge.net/maven/net/minecraftforge/forge/{1}-{0}{2}/forge-{1}-{0}{2}-universal.jar", version, build.mcversion, branch)
                                Else
                                    url = String.Format("http://files.minecraftforge.net/maven/net/minecraftforge/forge/{0}/forge-{0}-universal.jar", version)
                                End If
                            End If
                        End If
                        Dim outputfile As String = librarypath.FullName
                        'Auser bei forge universal lib
                        Dim Tounpack = False
                        If url.Contains("files.minecraftforge.net") AndAlso forgeuniversal = False Then
                            downloadforgelib = True
                            Tounpack = True
                            url = url.Insert(url.Length, ".pack.xz")
                            outputfile = outputfile.Insert(outputfile.Length, ".pack.xz")
                            Main.Write(Application.Current.FindResource("DownloadingLibraryFromForgeServer").ToString & " (" & Application.Current.FindResource("Try").ToString & " " & dltry & "): " & librarypath.FullName)
                        Else
                            If forgeuniversal = True Then
                                Main.Write(Application.Current.FindResource("DownloadingMinecraftForgeAutomatically").ToString & " (" & Application.Current.FindResource("Try").ToString & " " & dltry & "): " & librarypath.FullName)
                            End If
                        End If
                        If downloadforgelib = False Then
                            Main.Write(Application.Current.FindResource("DownloadingLibrary").ToString & " (" & Application.Current.FindResource("Try").ToString & " " & dltry & "): " & outputfile)
                        End If
                        wclibraries = New WebClient
                        AddHandler wclibraries.DownloadProgressChanged, Sub(sender, e) wclibraries_DownloadProgressChanged(sender, e, index, resolved)
                        Dim capturedException As ExceptionDispatchInfo = Nothing
                        Try
                            Await wclibraries.DownloadFileTaskAsync(New Uri(url), outputfile)
                        Catch ex As Exception
                            Main.Write(Application.Current.FindResource("ErrorDownloadingLibrary").ToString & ": " & ex.Message & Environment.NewLine & Application.Current.FindResource("ReinstallForgeIfStarted").ToString & "!", MainWindow.LogLevel.ERROR)
                            index += 1
                            dltry = 1
                            failures += 1
                            Try
                                File.Delete(Path.Combine(librariesfolder.FullName, Currentlibrary.path))
                            Catch
                                Main.Write(Application.Current.FindResource("ErrorDeletingLibrary").ToString & "!", MainWindow.LogLevel.WARNING)
                            End Try
                            capturedException = ExceptionDispatchInfo.Capture(ex)
                        End Try
                        If capturedException IsNot Nothing Then
                            Return Await StartLibrariesDownload(index, dltry, failures, resolved)
                        End If
                        'TODO
                        '------------------------- Download Finished ---------------------------

                        Dim libpath As String = Path.Combine(librariesfolder.FullName, Currentlibrary.path)
                        If Tounpack = True Then
                            Dim input As New FileInfo(libpath & ".pack.xz")
                            Dim output As New FileInfo(libpath)
                            Main.Write(Application.Current.FindResource("UnpackingLibrary").ToString & ": " & libpath)
                            If Await Unpack.Unpack(input, output) = False Then
                                'ShowError
                                Main.Write(Application.Current.FindResource("ErrorUnpackingLibrary").ToString & ": " & libpath)
                            End If
                            input.Delete()
                        End If
                        If dltry > 3 Then
                            failures += 1
                            Main.Write(Application.Current.FindResource("DownloadCanceledTooManyAttempts").ToString & "!" & Environment.NewLine & Application.Current.FindResource("ReinstallForgeIfStarted").ToString & "!", MainWindow.LogLevel.ERROR)
                            Return False
                        Else
                            If Tounpack = True Then
                                Tounpack = False
                                'Nächste Library Downloaden
                                index += 1
                                dltry = 1
                                Main.Write(Application.Current.FindResource("DownloadingLibrarySuccessful").ToString)
                            Else
                                'Hash überprüfen
                                If Helpers.SHA1FileHash(libpath).ToLower = Currentlibrarysha1 Then
                                    'Nächste Library Downloaden
                                    index += 1
                                    dltry = 1
                                    Main.Write(Application.Current.FindResource("DownloadingLibrarySuccessfulAndChecksumMatched").ToString)
                                Else
                                    'Library erneut herunterladen, Versuch erhöhen:
                                    dltry += 1
                                End If
                            End If
                            Return Await StartLibrariesDownload(index, dltry, failures, resolved)
                        End If

                        '-----------------------------------------------------------------------
                    Else
                        If String.IsNullOrWhiteSpace(Currentlibrarysha1) Then
                            Main.Write(Application.Current.FindResource("LibraryExistsNotChecksumMatched").ToString & ": " & librarypath.FullName, MainWindow.LogLevel.WARNING)
                        Else
                            If hashexisted Then
                                Main.Write(Application.Current.FindResource("LocalFileMatchesLocalChecksum").ToString & ": " & librarypath.FullName)
                            Else
                                Main.Write(Application.Current.FindResource("LibraryAlreadyExists").ToString & ": " & librarypath.FullName)
                            End If
                        End If
                        index += 1
                        Return Await StartLibrariesDownload(index, dltry, failures, resolved)
                    End If
                Else
                    index += 1
                    Return Await StartLibrariesDownload(index, dltry, failures, resolved)
                End If
            Catch Ex As Exception
                Main.Write(Ex.Message & Environment.NewLine & Ex.StackTrace, MainWindow.LogLevel.ERROR)
            End Try
        Else
            'Downloads finished
            Return True
        End If
        Return False
    End Function

    Private Shared Sub wclibraries_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs, index As Integer, resolved As VersionInfo)
        If index < resolved.libraries.Count Then
            MainWindowViewModel.Instance.pb_download_Value = (index / resolved.libraries.Count) * 100 + e.ProgressPercentage / resolved.libraries.Count
        End If
    End Sub

#End Region

#Region "Resources"

    Public Shared Async Function DownloadResources() As Task(Of Boolean)
        Main.Write(Application.Current.FindResource("DownloadingResources").ToString)
        If Startinfos.Versionsinfo Is Nothing Then
            Startinfos.Versionsinfo = Await MinecraftDownloadManager.ParseVersionsInfo(Startinfos.Version)
        End If
        Dim assets_index_name = If(Startinfos.Versionsinfo.assets, "legacy")
        MainWindowViewModel.Instance.pb_download_IsIndeterminate = True
        If resources_dir.Exists = False Then
            resources_dir.Create()
        End If
        If indexesfile(assets_index_name).Exists = False Then
            Main.Write(Application.Current.FindResource("DownloadingResourcesIndexes").ToString)
            If cacheindexesfile(assets_index_name).Directory.Exists = False Then
                cacheindexesfile(assets_index_name).Directory.Create()
            End If
            Dim wcindexes As New WebClient
            Try
                Await wcindexes.DownloadFileTaskAsync(New Uri(indexesurl(assets_index_name)), cacheindexesfile(assets_index_name).FullName)
                If indexesfile(assets_index_name).Directory.Exists = False Then
                    indexesfile(assets_index_name).Directory.Create()
                End If
                If indexesfile(assets_index_name).Exists Then
                    indexesfile(assets_index_name).Delete()
                End If
                cacheindexesfile(assets_index_name).CopyTo(indexesfile(assets_index_name).FullName, True)
                MainWindowViewModel.Instance.pb_download_IsIndeterminate = False
                Return Await StartResourcesDownload(0, 1, Await ParseResources(assets_index_name), assets_index_name)
            Catch
                'TODO Error Downloading index file
            End Try
        Else
            MainWindowViewModel.Instance.pb_download_IsIndeterminate = False
            Return Await StartResourcesDownload(0, 1, Await ParseResources(assets_index_name), assets_index_name) 'Maybe await is not weorking there
        End If
        Return False
    End Function

    Public Shared Async Function ParseResources(assets_index_name As String) As Task(Of resourcesindex)
        MainWindowViewModel.Instance.pb_download_IsIndeterminate = False
        Dim virtual As Boolean = False
        Dim indexesobjects As IList(Of resourcesindexobject) = New List(Of resourcesindexobject)
        Await Task.Run(New Action(Sub()
                                      Dim indexjo As JObject = JObject.Parse(File.ReadAllText(indexesfile(assets_index_name).FullName))
                                      If indexjo.Properties.Select(Function(p) p.Name).Contains("virtual") Then
                                          virtual = Convert.ToBoolean(indexjo("virtual").ToString)
                                      End If
                                      For i = 0 To indexjo("objects").Values.Count - 1
                                          Dim keys As List(Of JProperty) = indexjo.Value(Of JObject)("objects").Properties.ToList
                                          Dim key As String = keys.Item(i).Name
                                          'indexjo.Values(Of JProperty)("objects").ElementAt(i).ToString()
                                          Dim hash As String = keys.Item(i).Value.Value(Of String)("hash")
                                          Dim size As Integer = CInt(keys.Item(i).Value.Value(Of String)("size"))
                                          Dim item As New resourcesindexobject(key, hash, size)
                                          indexesobjects.Add(item)
                                      Next
                                  End Sub))
        Return New resourcesindex(virtual, indexesobjects)
    End Function

    Private Shared Async Function StartResourcesDownload(index As Integer, dltry As Integer, resourcesindexes As resourcesindex, assets_index_name As String) As Task(Of Boolean)
        If index < resourcesindexes.objects.Count Then
            MainWindowViewModel.Instance.pb_download_Value = index / (resourcesindexes.objects.Count - 1) * 100
            Dim currentresourcesobject As resourcesindexobject = resourcesindexes.objects.Item(index)
            Dim resource As New FileInfo(resourcefile(currentresourcesobject.hash).FullName.Replace("/", "\"))
            Dim todownload As Boolean = True
            If resource.Exists Then
                'Hash überprüfen
                If Helpers.SHA1FileHash(resource.FullName).ToLower = currentresourcesobject.hash Then
                    'Diese Resource überspringen
                    todownload = False
                Else
                    'Diese Resource Downloaden
                    todownload = True
                End If
            Else
                todownload = True
            End If
            If todownload = True Then
                If resource.Directory.Exists = False Then
                    resource.Directory.Create()
                End If
                Main.Write(Application.Current.FindResource("DownloadingResource").ToString & " (" & Application.Current.FindResource("Try").ToString & " " & dltry & "): " & resource.FullName)
                wcresources = New WebClient
                Try
                    Await wcresources.DownloadFileTaskAsync(New Uri(resourceurl(currentresourcesobject.hash)), resource.FullName)
                Catch
                    'TODO Failed Look at Github old version
                End Try

                '------------------------- Download Finished ---------------------------

                If dltry > 3 Then
                    Main.Write(Application.Current.FindResource("DownloadCanceledTooManyAttempts").ToString & "!", MainWindow.LogLevel.ERROR)
                    Return False
                    Exit Function
                Else
                    'Hash überprüfen
                    If Helpers.SHA1FileHash(resourcefile(currentresourcesobject.hash).FullName).ToLower = currentresourcesobject.hash Then
                        'Nächste Resource Downloaden
                        index += 1
                        dltry = 1
                        Main.Write(Application.Current.FindResource("DownloadingResourceSuccessful").ToString)
                    Else
                        'Resource erneut herunterladen, Versuch erhöhen:
                        dltry += 1
                    End If
                    Return Await StartResourcesDownload(index, dltry, resourcesindexes, assets_index_name)
                End If

                '-----------------------------------------------------------------------
            Else
                index += 1
                Return Await StartResourcesDownload(index, dltry, resourcesindexes, assets_index_name)
            End If
        Else
            'Downloads finished
            If resourcesindexes.virtual = True Then
                'Alle keys in den ordner :"virtual\legacy" kopieren
                Main.Write(String.Format(Application.Current.FindResource("CreateVirtualResources").ToString, assetspath.FullName))
                For Each item As resourcesindexobject In resourcesindexes.objects
                    Dim destination As New FileInfo(Path.Combine(assetspath.FullName, "virtual", assets_index_name, item.key.Replace("/", "\")))
                    If destination.Directory.Exists = False Then
                        destination.Directory.Create()
                    End If
                    Try
                        If destination.Exists = True Then
                            destination.Delete()
                        End If
                        resourcefile(item.hash).CopyTo(destination.FullName)
                    Catch ex As Exception
                    End Try
                Next
            End If
            Return True
        End If
        Return False
    End Function

    Private Shared Sub wc_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles wcresources.DownloadProgressChanged, wcversionsdownload.DownloadProgressChanged, wclibraries.DownloadProgressChanged
        Dim totalbytes As Double = e.TotalBytesToReceive / 1000
        Dim bytes As Double = e.BytesReceived / 1000
        Dim Einheit As String = "KB"
        If totalbytes >= 1000 Then
            totalbytes = e.TotalBytesToReceive / 1000000
            bytes = e.BytesReceived / 1000000
            Einheit = "MB"
        End If
        MainWindowViewModel.Instance.lbl_downloadstatus_Content = String.Format(Application.Current.FindResource("DownloadProgressFormat").ToString, e.ProgressPercentage, Math.Round(bytes, 2), Einheit, Math.Round(totalbytes, 2))
    End Sub

#End Region

End Class
