Imports System.Xml
Imports System.IO
Imports Microsoft.Win32
Imports MahApps
Imports Newtonsoft.Json
Imports System.ComponentModel
Imports Newtonsoft.Json.Linq
Imports System.Threading

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
        settings.ChangeLanguage()
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
        Languages = New List(Of Language) From {New Language("Deutsch", "JBou", "/resources/languages/mcml.de-de.xaml", "de", New Uri("/resources/languages/icons/de.png", UriKind.Relative)),
                                                    New Language("English", "JBou, ep9869", "/resources/languages/mcml.en-us.xaml", "en", New Uri("/resources/languages/icons/en.png", UriKind.Relative)),
                                                    New Language("español", "PromGames", "/resources/languages/mcml.es-es.xaml", "es", New Uri("/resources/languages/icons/es.png", UriKind.Relative)),
                                                    New Language("tiếng Việt", "sdvn", "/resources/languages/mcml.vn-vn.xaml", "vn", New Uri("/resources/languages/icons/vn.png", UriKind.Relative))}
        LoadDefaultLanguage()
        mcpfad = Nothing
        Accent = "Blue"
        Theme = "BaseLight"
        ServerAddress = Nothing
        DirectJoin = False
        JavaPath = Nothing
    End Sub

    Private Sub LoadDefaultLanguage()
        Dim lang As Language = Languages.FirstOrDefault(Function(x) x.Code = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName)
        Me.Language = If(lang Is Nothing, "en", lang.Code)
    End Sub

    Public Sub ChangeLanguage()
        Dim dic = New ResourceDictionary() With {.Source = New Uri(Languages.First(Function(x) x.Code = Language).Path, UriKind.Relative)}
        If _lastLanguage IsNot Nothing Then
            Application.Current.Resources.Remove(_lastLanguage)
        End If
        Application.Current.Resources.MergedDictionaries.Add(dic)
        _lastLanguage = dic
    End Sub

    Private _lastLanguage As ResourceDictionary
    Private _Language As String

    Public Property Language() As String
        Get
            Return _Language
        End Get
        Set(ByVal value As String)
            SetProperty(value, _Language)
            ChangeLanguage()
            Me.Save()
        End Set
    End Property
    Private _Languages As IList(Of Language)
    <JsonIgnore>
    Public Property Languages As IList(Of Language)
        Get
            Return _Languages
        End Get
        Set(value As IList(Of Language))
            _Languages = value
        End Set
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