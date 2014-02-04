Imports System.Collections.Generic
Imports System.Linq
Imports System.Net.Sockets
Imports System.Text
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json
Imports System.Drawing
Imports System.Windows.Media.Imaging
Imports System.IO

Public Class OldServerPing
    Const NonNumericRegex As String = "[^.0-9]"

    ''' <summary>
    ''' A class to ping a Minecraft server for its description, player count and maximum player count.
    ''' </summary>
    ''' <param name="host">Address of server to ping.</param>
    ''' <param name="port">Port on which the Minecraft server sits.</param>
    ''' <param name="sendTimeout">How long to wait when sending the ping (in milliseconds).</param>
    ''' <param name="recieveTimeout">How long to wait when reciving the ping response (in milliseconds).</param>
    Public Sub New(host As String, Optional port As Integer = 25565, Optional sendTimeout As Integer = 0, Optional recieveTimeout As Integer = 30000)
        Me.HostName = host
        Me.Port = port
        Me.SendTimeout = sendTimeout
        Me.RecieveTimeout = recieveTimeout

        Refresh()
    End Sub


    ''' <summary>
    ''' Refresh the data about this server.
    ''' </summary>
    Public Sub Refresh()
        'Set up our TCP Client
        Dim tcpClient As New TcpClient()

        'Set the timeouts
        tcpClient.SendTimeout = SendTimeout
        tcpClient.ReceiveTimeout = RecieveTimeout

        'Reset values
        Online = False
        PlayerCount = 0

        Try
            tcpClient.Connect(HostName, Port)

            Dim stream As NetworkStream = tcpClient.GetStream()

            'Send server list ping
            stream.Write(New Byte() {&HFE}, 0, 1)

            'Read data into buffer
            Dim buffer As Byte() = New Byte(255) {}
            While tcpClient.Connected
                Dim readBytes As Integer = stream.Read(buffer, 0, 256)
                If readBytes = 0 Then
                    Exit While
                End If
            End While

            'Check we've got a 0xFF Disconnect
            If buffer(0) <> 255 Then
                Online = False
            Else
                'Server is online
                Online = True

                'Remove the packet ident (0xFF) and the short containing the length of the string
                buffer = buffer.Skip(3).ToArray()

                'Decode UCS-2 string
                Dim str As String = System.Text.Encoding.BigEndianUnicode.GetString(buffer)

                'Split into array
                Dim parts As String() = str.Split("§"c)

                'Set values
                Description = parts(0).Trim()
                PlayerCount = Integer.Parse(Regex.Replace(parts(1), "[^.0-9]", ""))
                MaxPlayerCount = Integer.Parse(Regex.Replace(parts(2), "[^.0-9]", ""))
            End If
            'Online is already set to false.
            'We assume the server is offline if any errors occur.
        Catch
        Finally
            'close the client
            tcpClient.Close()
        End Try
    End Sub

    ''' <summary>
    ''' The port being pinged.
    ''' </summary>
    Public Property Port() As Integer
        Get
            Return m_Port
        End Get
        Set(value As Integer)
            m_Port = value
        End Set
    End Property
    Private m_Port As Integer

    ''' <summary>
    ''' The hostname being tested.
    ''' </summary>
    Public Property HostName As String
        Get
            Return m_HostName
        End Get
        Set(value As String)
            m_HostName = value
        End Set
    End Property
    Private m_HostName As String

    Public Property RecieveTimeout As Integer
        Get
            Return m_RecieveTimeout
        End Get
        Set(value As Integer)
            m_RecieveTimeout = value
        End Set
    End Property
    Private m_RecieveTimeout As Integer
    Public Property SendTimeout As Integer
        Get
            Return m_SendTimeout
        End Get
        Set(value As Integer)
            m_SendTimeout = value
        End Set
    End Property
    Private m_SendTimeout As Integer


    ''' <summary>
    ''' Whether we can reach the server or not.
    ''' </summary>
    Public Property Online As Boolean
        Get
            Return m_Online
        End Get
        Set(value As Boolean)
            m_Online = value
        End Set
    End Property
    Private m_Online As Boolean

    ''' <summary>
    ''' The server's MOTD.
    ''' This will always be null if Online has never been true. This information is kept after a Refresh.
    ''' </summary>
    Public Property Description As String
        Get
            Return m_Description
        End Get
        Set(value As String)
            m_Description = value
        End Set
    End Property
    Private m_Description As String

    ''' <summary>
    ''' Number of online players.
    ''' This will always be 0 if Online is false.
    ''' </summary>
    Public Property PlayerCount As Integer
        Get
            Return m_PlayerCount
        End Get
        Set(value As Integer)
            m_PlayerCount = value
        End Set
    End Property
    Private m_PlayerCount As Integer

    ''' <summary>
    ''' Maximum number of players for this server.
    ''' This will always be 0 if Online has never been true.  This information is kept after a Refresh.
    ''' </summary>
    Public Property MaxPlayerCount As Integer
        Get
            Return m_MaxPlayerCount
        End Get
        Set(value As Integer)
            m_MaxPlayerCount = value
        End Set
    End Property
    Private m_MaxPlayerCount As Integer
End Class