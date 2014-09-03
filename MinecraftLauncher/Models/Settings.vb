Imports System.Xml
Imports System.IO
Imports Microsoft.Win32
Imports MahApps
Imports Newtonsoft.Json
Imports System.ComponentModel
Imports Newtonsoft.Json.Linq

Public Class Settings
    Private Shared SettingsFile As New FileInfo(Path.Combine(Applicationdata.FullName, "Settings.json"))
    Public Shared Settings As cls_Settings = New cls_Settings
    Public Shared Async Function Load() As Task
        If SettingsFile.Exists Then
            Dim valid As Boolean = True
            Try
                JContainer.Parse(File.ReadAllText(SettingsFile.FullName))
            Catch ex As Exception
                valid = False
            End Try
            If valid = True Then
                Await ReadSettings()
            Else
                SetStandard()
            End If
        Else
            SetStandard()
        End If
    End Function
    Public Shared Sub SetStandard()
        Settings.mcpfad = Nothing
        Settings.Accent = "Blue"
        Settings.Theme = "BaseLight"
        Settings.DirectJoin = False
        Settings.ServerAddress = Nothing
    End Sub
    Private Shared Async Function ReadSettings() As Task
        Dim text As String = File.ReadAllText(SettingsFile.FullName)
        Settings = Await JsonConvert.DeserializeObjectAsync(Of cls_Settings)(text, New JsonSerializerSettings() With {.DefaultValueHandling = DefaultValueHandling.Ignore, .NullValueHandling = NullValueHandling.Ignore})
    End Function
    Public Shared Sub Save()
        If Settings.mcpfad = mcpfad.FullName Then
            Settings.mcpfad = Nothing
        End If
        Dim text As String = JsonConvert.SerializeObject(Settings, Newtonsoft.Json.Formatting.Indented, New JsonSerializerSettings() With {.DefaultValueHandling = DefaultValueHandling.Ignore, .NullValueHandling = NullValueHandling.Ignore})
        File.WriteAllText(SettingsFile.FullName, text)
    End Sub
    Public Class cls_Settings
        Inherits PropertyChangedBase
        Private _mcpfad As String, _accent As String, _Theme As String, _ServerAddress As String, _DirectJoin As Boolean, _WindowState As WindowState, _JavaPath As String, _lstLanguages As IList(Of Language)
        Public Sub New()
            _lstLanguages = New List(Of Language) From {New Language("Deutsch", "/resources/languages/MSL.de-de.xaml", "de-de", New Uri("/resources/languages/icons/de.png", UriKind.Relative)), New Language("English", "/resources/languages/MSL.en-us.xaml", "en-us", New Uri("/resources/languages/icons/en.png", UriKind.Relative))}
        End Sub

        Private Sub LoadDefaultLanguage()
            Dim currentCultur = System.Threading.Thread.CurrentThread.CurrentCulture
            If currentCultur.TwoLetterISOLanguageName = "de" Then
                Me.ActivLanguage = lstLanguages(0)
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
            End Set
        End Property

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
    End Class

End Class