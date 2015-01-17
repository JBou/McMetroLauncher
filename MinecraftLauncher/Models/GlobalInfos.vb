Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Resources
Imports Microsoft.Win32
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports System.Windows.Media
Imports McMetroLauncher.JBou.Authentication
Imports MahApps.Metro
Imports System

Public Module GlobalInfos
#Region "Functions/Subs"
    Public Function Check_Updates() As Boolean
        If New Version(onlineversion) > New Version(AssemblyVersion) Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Async Function Versions_Load() As Task
        Dim o As String = File.ReadAllText(outputjsonversions.FullName)
        GlobalInfos.Versions = Await JsonConvert.DeserializeObjectAsync(Of Versionslist)(o)
        If versionsfolder.Exists = True Then
            Dim list_versionsdirectories As IEnumerable(Of String) = versionsfolder.GetDirectories.Select(Function(p) p.FullName)
            Dim list_versions As IList(Of String) = New List(Of String)
            For Each version As String In list_versionsdirectories
                Dim versionname As String = IO.Path.GetFileName(version)
                If GlobalInfos.Versions.versions.Select(Function(p) p.id).Contains(versionname) = False Then
                    list_versions.Add(versionname)
                End If
            Next
            For Each Version As String In list_versions
                If File.Exists(Path.Combine(versionsfolder.FullName, Version, Version & ".json")) Then
                    Dim jo As JObject = JObject.Parse(File.ReadAllText(Path.Combine(versionsfolder.FullName, Version, Version & ".json")))
                    If jo("id").ToString = Version Then
                        Dim versionitem As New Versionslist.Version() With {
                            .id = jo("id").ToString,
                            .type = jo("type").ToString,
                            .time = jo("time").ToString,
                            .releaseTime = jo("releaseTime").ToString,
                            .custom = True}
                        GlobalInfos.Versions.versions.Add(versionitem)
                    Else
                        'TODO: Falsche id wurde gefunden
                    End If
                End If
            Next
        End If
    End Function

    Public Function GetJavaHome() As String
        Dim environmentPath As String = Environment.GetEnvironmentVariable("JAVA_HOME")
        If environmentPath IsNot Nothing Then
            Return environmentPath
        End If
        Try
            Dim javaKey As String = "SOFTWARE\JavaSoft\Java Runtime Environment"
            Dim javakeyWow6432Node As String = "SOFTWARE\Wow6432Node\JavaSoft\Java Runtime Environment"
            Using baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(javaKey)
                If baseKey IsNot Nothing Then
                    Dim currentVersion As String = baseKey.GetValue("CurrentVersion").ToString()
                    Using homeKey = baseKey.OpenSubKey(currentVersion)
                        If homeKey IsNot Nothing Then
                            Return homeKey.GetValue("JavaHome").ToString()
                        End If
                    End Using
                End If
            End Using
            Using baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(javakeyWow6432Node)
                If baseKey IsNot Nothing Then
                    Dim currentVersion As String = baseKey.GetValue("CurrentVersion").ToString()
                    Using homeKey = baseKey.OpenSubKey(currentVersion)
                        If homeKey IsNot Nothing Then
                            Return homeKey.GetValue("JavaHome").ToString()
                        End If
                    End Using
                End If
            End Using
        Catch
        End Try
        Return Nothing
    End Function

    Public Function GetJavaPath() As String
        If Not MainViewModel.Instance.Settings.JavaPath = Nothing Then
            Return MainViewModel.Instance.Settings.JavaPath()
        End If
        If Not GetJavaHome() = Nothing Then
            Return Path.Combine(GetJavaHome(), "bin", "java.exe")
        End If
        Return Nothing
    End Function

#End Region
    Public Class Startinfos
        Public Class Server
            Private Shared m_serveradress As String
            Private Shared m_serverport As String
            Private Shared m_juststarted As Boolean
            Public Shared Property JustStarted As Boolean
                Get
                    Return m_juststarted
                End Get
                Set(value As Boolean)
                    m_juststarted = value
                End Set
            End Property
            Public Shared Property ServerAdress As String
                Get
                    Return m_serveradress
                End Get
                Set(value As String)
                    m_serveradress = value
                End Set
            End Property
            Public Shared Property ServerPort As String
                Get
                    Return m_serverport
                End Get
                Set(value As String)
                    m_serverport = value
                End Set
            End Property
        End Class
        Public Shared Property Profile As Profiles.Profile
            Get
                Return m_profile
            End Get
            Set(value As Profiles.Profile)
                m_profile = value
            End Set
        End Property

        Private Shared m_profile As Profiles.Profile
        Public Shared Property Version As Versionslist.Version
            Get
                Return m_version
            End Get
            Set(value As Versionslist.Version)
                m_version = value
            End Set
        End Property
        Private Shared m_version As Versionslist.Version
        Public Shared Property Versionsinfo As VersionInfo
            Get
                Return m_versionsinfo
            End Get
            Set(value As VersionInfo)
                m_versionsinfo = value
            End Set
        End Property
        Private Shared m_versionsinfo As VersionInfo
        Public Shared Property IsStarting As Boolean
            Get
                Return m_isstarting
            End Get
            Set(value As Boolean)
                m_isstarting = value
                If value = False Then
                    Version = Nothing
                End If
            End Set
        End Property
        Private Shared m_isstarting As Boolean
        Public Shared Property Session As Session
    End Class

    '--------supportedLauncherVersion---------
    Public Const supportedLauncherVersion As Integer = 16
    '--------------MainWindow------------------
    Public Property Main As New MainWindow
    '------------------------------------------
    Public ReadOnly Property AssemblyVersion As String
        Get
            Return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
        End Get
    End Property
    Public SelectedModVersion As String
    Public Versions As Versionslist = New Versionslist
    Public Website As String = "http://patzleiner.net/"
    Public Github As String = "https://github.com/JBou/McMetroLauncher"
    Public Appdata As New DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
    Public mcpfad As New DirectoryInfo(Path.Combine(Appdata.FullName, ".minecraft"))
    Public Applicationdata As New DirectoryInfo(Path.Combine(Appdata.FullName, "McMetroLauncher"))
    Public Applicationcache As New DirectoryInfo(IO.Path.Combine(Applicationdata.FullName, "cache"))
    Public cachefolder As New DirectoryInfo(Path.Combine(mcpfad.FullName, "cache"))
    Public versionsfolder As New DirectoryInfo(Path.Combine(mcpfad.FullName, "versions"))
    Public modsfolder As New DirectoryInfo(Path.Combine(mcpfad.FullName, "mods"))
    Public modsfile As New FileInfo(Path.Combine(Applicationcache.FullName, "modlist.json"))
    Public Forgefile As New FileInfo(Path.Combine(Applicationcache.FullName, "forge.json"))
    Public Legacyforgefile As New FileInfo(Path.Combine(Applicationcache.FullName, "legacyforge.json"))
    Public librariesfolder As New DirectoryInfo(Path.Combine(mcpfad.FullName, "libraries"))
    Public assetspath As New DirectoryInfo(Path.Combine(mcpfad.FullName, "assets"))
    Public launcher_profiles_json As New FileInfo(Path.Combine(mcpfad.FullName, "launcher_profiles.json"))
    Public servers_dat As New FileInfo(Path.Combine(mcpfad.FullName, "servers.dat"))
    Public Newprofile As Boolean
    Public outputjsonversions As New FileInfo(Path.Combine(Applicationcache.FullName, "versions.json"))
    Public ReadOnly Property versionsJSON(Version As Versionslist.Version) As String
        Get
            Dim versionid As String = Versions.versions.Where(Function(p) p.id = Version.id).First.id
            Return Path.Combine(versionsfolder.FullName, versionid, versionid & ".json")
        End Get
    End Property
    Public Versionsjar As String
    Public UnpackDirectory As String
    Public Arguments As String
    Public Delegate Sub WriteA(ByVal Text As String, rtb As RichTextBox)
    Public Delegate Sub WriteColored(ByVal Text As String, rtb As RichTextBox, Color As Brush)
    Public downloadfilepath As String
    Public onlineversion As String = Nothing
    Public changelog As String = Nothing
    Public resources_dir As New DirectoryInfo(Path.Combine(mcpfad.FullName, "resources"))
    Public librariesurl As String = "https://libraries.minecraft.net/"
    Public CommandLineArgs As String() = Environment.GetCommandLineArgs()
    Public ReadOnly Property indexesurl(index_name As String) As String
        Get
            Return "http://s3.amazonaws.com/Minecraft.Download/indexes/" & index_name & ".json"
        End Get
    End Property
    Public ReadOnly Property cacheindexesfile(index_name As String) As FileInfo
        Get
            Return New FileInfo(Path.Combine(cachefolder.FullName, "indexes/" & index_name & ".json"))
        End Get
    End Property
    Public ReadOnly Property indexesfile(index_name As String) As FileInfo
        Get
            Return New FileInfo(Path.Combine(assetspath.FullName, "indexes/" & index_name & ".json"))
        End Get
    End Property
    Public ReadOnly Property resourcefile(hash As String) As FileInfo
        Get
            Return New FileInfo(Path.Combine(assetspath.FullName, "objects/" & hash.Substring(0, 2) & "/" & hash))
        End Get
    End Property
    Public ReadOnly Property resourceurl(hash As String) As String
        Get
            Return "http://resources.download.minecraft.net/" & hash.Substring(0, 2) & "/" & hash
        End Get
    End Property
    Public ReadOnly YoutubeVideoRegex As Regex = New Regex("youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)", RegexOptions.IgnoreCase)
    Public Versionsurl As String = "http://s3.amazonaws.com/Minecraft.Download/versions/versions.json"
    Public modfileurl As String = Website & "download/modlist.json"
    Public versionurl As String = Website & "mcmetrolauncher/version.txt"
    Public changelogurl As String = Website & "mcmetrolauncher/changelog.txt"
    Public Forgeurl As String = "http://files.minecraftforge.net/maven/net/minecraftforge/forge/json"
    Public Legacyforgeurl As String = "http://files.minecraftforge.net/minecraftforge/json2"
    Public startedversions As IList(Of String) = New List(Of String)
    'http://www.joomlavision.com/customize-browser-scrollbars-css3/

    'http://ospa.arvat.org/gmail-style-scrollbar-using-webkit-and-css/
    Public Scrollbarcss As String = <![CDATA[
 ::-webkit-scrollbar{
    width:10px;
    height:10px;
    background-color:#fff;
    box-shadow: inset 1px 1px 0 rgba(0,0,0,.1),inset -1px -1px 0 rgba(0,0,0,.07);
}
::-webkit-scrollbar:hover{
    background-color:#eee;
}
::-webkit-resizer{
    -webkit-border-radius:4px;
    background-color:#666;
}
::-webkit-scrollbar-thumb{
    min-height:0.8em;
    min-width:0.8em;
    background-color: rgba(0, 0, 0, .2);
    box-shadow: inset 1px 1px 0 rgba(0,0,0,.1),inset -1px -1px 0 rgba(0,0,0,.07);
}
::-webkit-scrollbar-thumb:hover{
    background-color: #bbb;
}
::-webkit-scrollbar-thumb:active{
    background-color:#888;
}
    ]]>.Value
End Module