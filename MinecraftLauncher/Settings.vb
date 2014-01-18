Imports System.Xml
Imports System.IO
Imports Microsoft.Win32
Imports MahApps

Public Class Settings
    Private Shared SettingsFile As New FileInfo(Path.Combine(Applicationdata.FullName, "Settings.xml"))

    Private Shared _Username As String, _mcpfad As String, _accent As String, _Theme As String, _ServerAddress As String, _DirectJoin As Boolean

    Public Shared Property mcpfad As String
        Get
            Return _mcpfad
        End Get
        Set(value As String)
            _mcpfad = value
        End Set
    End Property

    Public Shared Property Accent As String
        Get
            Return _accent
        End Get
        Set(value As String)
            _accent = value
        End Set
    End Property

    Public Shared Property Theme As String
        Get
            Return _Theme
        End Get
        Set(value As String)
            _Theme = value
        End Set
    End Property

    Public Shared Property ServerAddress As String
        Get
            Return _ServerAddress
        End Get
        Set(value As String)
            _ServerAddress = value
        End Set
    End Property

    Public Shared Property DirectJoin As Boolean
        Get
            Return _DirectJoin
        End Get
        Set(value As Boolean)
            _DirectJoin = value
        End Set
    End Property

    Public Shared Property Username As String
        Get
            Return _Username
        End Get
        Set(value As String)
            _Username = value
        End Set
    End Property

    Public Shared Sub Load()
        If SettingsFile.Exists Then
            ReadSettings()
        Else
            Username = Nothing
            mcpfad = mcpfad
            Accent = "Blue"
            Theme = "Light"
        End If
    End Sub

    Private Shared Sub ReadSettings()
        Dim document As XmlReader = New XmlTextReader(SettingsFile.FullName)

        While (document.Read())
            Dim type As XmlNodeType = document.NodeType

            If type = XmlNodeType.Element Then

                Select Case document.Name
                    Case "Username"
                        Username = document.ReadInnerXml
                    Case "mcpfad"
                        mcpfad = document.ReadInnerXml
                    Case "Accent"
                        Accent = document.ReadInnerXml
                    Case "Theme"
                        Theme = document.ReadInnerXml
                    Case "ServerAddress"
                        ServerAddress = document.ReadInnerXml
                    Case "DirectJoin"
                        DirectJoin = Convert.ToBoolean(document.ReadInnerXml)
                End Select

            End If
        End While
    End Sub

    Public Shared Sub Save()
        Try

            If SettingsFile.Exists = False Then
                SettingsFile.Directory.Create()
            End If

            Dim Settings As New XmlWriterSettings()

            Settings.Indent = True

            Dim XmlWrt As XmlWriter = XmlWriter.Create(SettingsFile.FullName, Settings)

            With XmlWrt

                .WriteStartDocument()

                .WriteComment("Einstellungen vom McMetroLauncher")
                .WriteComment("Bitte nicht manuell bearbeiten")

                .WriteStartElement("Settings")

                .WriteStartElement("Username")
                .WriteString(Username)
                .WriteEndElement()

                .WriteStartElement("mcpfad")
                .WriteString(mcpfad)
                .WriteEndElement()

                .WriteStartElement("Accent")
                .WriteString(Accent)
                .WriteEndElement()

                .WriteStartElement("Theme")
                .WriteString(Theme)
                .WriteEndElement()

                .WriteStartElement("ServerAddress")
                .WriteString(ServerAddress)
                .WriteEndElement()

                .WriteStartElement("DirectJoin")
                .WriteString(DirectJoin.ToString)
                .WriteEndElement()

                .WriteEndElement()

                .WriteEndDocument()
                .Close()

            End With
        Catch

        End Try
    End Sub

End Class