Imports System.Xml
Imports System.IO
Imports Microsoft.Win32
Imports MahApps
Imports Newtonsoft.Json
Imports System.ComponentModel
Imports Newtonsoft.Json.Linq

Public Class Settings
    Inherits PropertyChangedBase
    Private Shared SettingsFile As New FileInfo(Path.Combine(Applicationdata.FullName, "Settings.json"))
    Public Shared Async Function Load() As Task(Of Settings)
        If SettingsFile.Exists Then
            Dim valid As Boolean = True
            Try
                JContainer.Parse(File.ReadAllText(SettingsFile.FullName))
            Catch ex As Exception
                valid = False
            End Try
            If valid = True Then
                Return Await ReadSettings()
            End If
        End If
        Return New Settings
    End Function

    Private Shared Async Function ReadSettings() As Task(Of Settings)
        Dim text As String = File.ReadAllText(SettingsFile.FullName)
        Dim settings As Settings = Await JsonConvert.DeserializeObjectAsync(Of Settings)(text, New JsonSerializerSettings() With {.DefaultValueHandling = DefaultValueHandling.Ignore, .NullValueHandling = NullValueHandling.Ignore})
        settings.ActivLanguage = settings.lstLanguages.Where(Function(p) p.Code = settings.ActivLanguage.Code).First
        Return settings
    End Function
    Public Function Save() As Boolean
        If mcpfad = GlobalInfos.mcpfad.FullName Then
            mcpfad = Nothing
        End If
        Dim text As String = JsonConvert.SerializeObject(Me, Newtonsoft.Json.Formatting.Indented, New JsonSerializerSettings() With {.DefaultValueHandling = DefaultValueHandling.Ignore, .NullValueHandling = NullValueHandling.Ignore})
        Try
            If Not SettingsFile.Directory.Exists Then
                SettingsFile.Directory.Create()
            End If
            File.WriteAllText(SettingsFile.FullName, text)
        Catch ex As IOException
            Return False
        End Try
        Return True
    End Function

#Region "Properties"
    Private _mcpfad As String, _accent As String, _Theme As String, _ServerAddress As String, _DirectJoin As Boolean, _WindowState As WindowState, _JavaPath As String
    Public Sub New()
        _lstLanguages = New List(Of Language) From {New Language("Deutsch", "JBou", "/resources/languages/mcml.de-de.xaml", "de-de", New Uri("/resources/languages/icons/de.png", UriKind.Relative)),
                                                    New Language("English", "JBou, ep9869", "/resources/languages/mcml.en-us.xaml", "en-us", New Uri("/resources/languages/icons/en.png", UriKind.Relative)),
                                                    New Language("tiếng Việt", "sdvn", "/resources/languages/mcml.vn-vn.xaml", "vn-vn", New Uri("/resources/languages/icons/vn.png", UriKind.Relative))}
        LoadDefaultLanguage()
        mcpfad = Nothing
        Accent = "Blue"
        Theme = "BaseLight"
        ServerAddress = Nothing
        DirectJoin = False
        JavaPath = Nothing
    End Sub

    Private Sub LoadDefaultLanguage()
        Dim currentCultur = System.Threading.Thread.CurrentThread.CurrentCulture
        If currentCultur.TwoLetterISOLanguageName = "de" Then
            Me.ActivLanguage = lstLanguages(0)
        ElseIf currentCultur.TwoLetterISOLanguageName = "vn" Then
            Me.ActivLanguage = lstLanguages(2)
        Else
            Me.ActivLanguage = lstLanguages(1)
        End If
    End Sub

    Private _lastLanguage As ResourceDictionary
    Private _ActivLanguage As Language

    Public Property ActivLanguage() As Language
        Get
            Return _ActivLanguage
        End Get
        Set(ByVal value As Language)
            SetProperty(value, _ActivLanguage)
            Dim dic = New ResourceDictionary() With {.Source = New Uri(value.Path, UriKind.Relative)}
            If _lastLanguage IsNot Nothing Then
                Application.Current.Resources.Remove(_lastLanguage)
            End If
            Application.Current.Resources.MergedDictionaries.Add(dic)
            _lastLanguage = dic
            Me.Save()
        End Set
    End Property
    Private _lstLanguages As IList(Of Language)
    <JsonIgnore>
    Public ReadOnly Property lstLanguages As IList(Of Language)
        Get
            Return _lstLanguages
        End Get
    End Property
    Public Property mcpfad As String
        Get
            Return _mcpfad
        End Get
        Set(value As String)
            _mcpfad = value
        End Set
    End Property
    Public Property JavaPath As String
        Get
            Return _JavaPath
        End Get
        Set(value As String)
            _JavaPath = value
        End Set
    End Property
    <DefaultValue("Blue")>
    Public Property Accent As String
        Get
            Return _accent
        End Get
        Set(value As String)
            _accent = value
        End Set
    End Property
    <DefaultValue("Light")>
    Public Property Theme As String
        Get
            Return _Theme
        End Get
        Set(value As String)
            _Theme = value
        End Set
    End Property
    <DefaultValue("")>
    Public Property ServerAddress As String
        Get
            Return _ServerAddress
        End Get
        Set(value As String)
            _ServerAddress = value
        End Set
    End Property
    <DefaultValue(False)>
    Public Property DirectJoin As Boolean
        Get
            Return _DirectJoin
        End Get
        Set(value As Boolean)
            _DirectJoin = value
        End Set
    End Property
    <DefaultValue(WindowState.Normal)>
    Public Property WindowState As WindowState
        Get
            Return _WindowState
        End Get
        Set(value As WindowState)
            _WindowState = value
        End Set
    End Property

#End Region

End Class