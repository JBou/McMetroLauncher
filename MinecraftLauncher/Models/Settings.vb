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
    Public Shared Async Function Save() As Task
        If Settings.mcpfad = mcpfad.FullName Then
            Settings.mcpfad = Nothing
        End If
        Dim text As String = Await JsonConvert.SerializeObjectAsync(Settings, Newtonsoft.Json.Formatting.Indented, New JsonSerializerSettings() With {.DefaultValueHandling = DefaultValueHandling.Ignore, .NullValueHandling = NullValueHandling.Ignore})
        File.WriteAllText(SettingsFile.FullName, text)
    End Function
    Public Class cls_Settings
        Private _mcpfad As String, _accent As String, _Theme As String, _ServerAddress As String, _DirectJoin As Boolean, _WindowState As WindowState
        Public Sub New()

        End Sub
        Public Property mcpfad As String
            Get
                Return _mcpfad
            End Get
            Set(value As String)
                _mcpfad = value
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