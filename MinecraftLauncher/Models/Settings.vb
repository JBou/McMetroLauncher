Imports System.Xml
Imports System.IO
Imports Microsoft.Win32
Imports MahApps
Imports Newtonsoft.Json
Imports System.ComponentModel
Imports Newtonsoft.Json.Linq

Public Class Settings
    Private Shared SettingsFile As New FileInfo(Path.Combine(Applicationdata.FullName, "Settings.xml"))
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
        Settings.Theme = "Light"
        Settings.DirectJoin = False
        Settings.ServerAddress = Nothing
    End Sub
    Private Shared Async Function ReadSettings() As Task
        Dim text As String = File.ReadAllText(SettingsFile.FullName)
        Settings = Await JsonConvert.DeserializeObjectAsync(Of cls_Settings)(text, New JsonSerializerSettings() With {.DefaultValueHandling = DefaultValueHandling.Ignore, .NullValueHandling = NullValueHandling.Ignore})


        'Dim document As XmlReader = New XmlTextReader(SettingsFile.FullName)

        'While (document.Read())
        '    Dim type As XmlNodeType = document.NodeType

        '    If type = XmlNodeType.Element Then

        '        Select Case document.Name
        '            Case "Username"
        '                Username = document.ReadInnerXml
        '            Case "mcpfad"
        '                mcpfad = document.ReadInnerXml
        '            Case "Accent"
        '                Accent = document.ReadInnerXml
        '            Case "Theme"
        '                Theme = document.ReadInnerXml
        '            Case "ServerAddress"
        '                ServerAddress = document.ReadInnerXml
        '            Case "DirectJoin"
        '                DirectJoin = Convert.ToBoolean(document.ReadInnerXml)
        '        End Select

        '    End If
        'End While
    End Function
    Public Shared Async Function Save() As Task
        If Settings.mcpfad = mcpfad.FullName Then
            Settings.mcpfad = Nothing
        End If
        Dim text As String = Await JsonConvert.SerializeObjectAsync(Settings, Newtonsoft.Json.Formatting.Indented, New JsonSerializerSettings() With {.DefaultValueHandling = DefaultValueHandling.Ignore, .NullValueHandling = NullValueHandling.Ignore})
        File.WriteAllText(SettingsFile.FullName, text)

        'Try

        '    If SettingsFile.Directory.Exists = False Then
        '        SettingsFile.Directory.Create()
        '    End If
        '    Dim s As String = Await JsonConvert.SerializeObjectAsync(Settings, Newtonsoft.Json.Formatting.Indented)
        '    Dim Settings As New XmlWriterSettings()

        '    Settings.Indent = True

        '    Dim XmlWrt As XmlWriter = XmlWriter.Create(SettingsFile.FullName, Settings)

        '    With XmlWrt

        '        .WriteStartDocument()

        '        .WriteComment("Einstellungen vom McMetroLauncher")
        '        .WriteComment("Bitte nicht manuell bearbeiten")

        '        .WriteStartElement("Settings")

        '        .WriteStartElement("Username")
        '        .WriteString(Username)
        '        .WriteEndElement()

        '        .WriteStartElement("mcpfad")
        '        .WriteString(mcpfad)
        '        .WriteEndElement()

        '        .WriteStartElement("Accent")
        '        .WriteString(Accent)
        '        .WriteEndElement()

        '        .WriteStartElement("Theme")
        '        .WriteString(Theme)
        '        .WriteEndElement()

        '        .WriteStartElement("ServerAddress")
        '        .WriteString(ServerAddress)
        '        .WriteEndElement()

        '        .WriteStartElement("DirectJoin")
        '        .WriteString(DirectJoin.ToString)
        '        .WriteEndElement()

        '        .WriteEndElement()

        '        .WriteEndDocument()
        '        .Close()

        '    End With
        'Catch

        'End Try
    End Function
    Public Class cls_Settings
        Private _Username As String, _mcpfad As String, _accent As String, _Theme As String, _ServerAddress As String, _DirectJoin As Boolean, _WindowState As WindowState
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
        <DefaultValue("")>
        Public Property Username As String
            Get
                Return _Username
            End Get
            Set(value As String)
                _Username = value
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