Imports Craft.Net
Imports System.Net
Imports Craft.Net.Networking
Imports System.Net.Sockets
Imports Craft.Net.Common
Imports fNbt
Imports Newtonsoft.Json
Imports System.ComponentModel
Imports System.Collections.ObjectModel

Public Class ServerList
    Implements INotifyPropertyChanged

    Shared Sub New()
        MainViewModel.Instance.Servers = New ObservableCollection(Of Server)
    End Sub

    Public Shared Sub Save()
        SaveTo(servers_dat.FullName)
    End Sub

    Public Shared Sub SaveTo(file As String)
        Dim nbt = New NbtFile()
        nbt.RootTag = New NbtCompound("")
        Dim list = New NbtList("servers", NbtTagType.Compound)
        For Each server As Server In MainViewModel.Instance.Servers
            Dim compound = New NbtCompound()
            compound.Add(New NbtString("name", server.name))
            compound.Add(New NbtString("ip", server.ip))
            If server.icon <> Nothing Then
                compound.Add(New NbtString("icon", server.icon))
            End If
            compound.Add(New NbtByte("hideAddress", CByte(If(server.hideAddress, 1, 0))))
            compound.Add(New NbtByte("acceptTextures", CByte(If(server.AcceptTextures, 1, 0))))
            list.Add(compound)
        Next
        nbt.RootTag.Add(list)
        nbt.SaveToFile(file, NbtCompression.None)
    End Sub

    Public Shared Async Function Load() As Task
        Await LoadFrom(servers_dat.FullName)
    End Function

    Public Shared Async Function LoadFrom(file As String) As Task
        Await Task.Run(Sub()
                           If IO.File.Exists(file) Then
                               Dim nbt = New NbtFile(file)
                               Dim ls As New ObservableCollection(Of Server)
                               For Each server As NbtCompound In TryCast(nbt.RootTag("servers"), NbtList)
                                   Dim entry As New Server()
                                   If server.Contains("name") Then
                                       entry.name = server("name").StringValue
                                   End If
                                   If server.Contains("ip") Then
                                       entry.ip = server("ip").StringValue
                                   End If
                                   If server.Contains("hideAddress") Then
                                       entry.hideAddress = server("hideAddress").ByteValue = 1
                                   End If
                                   If server.Contains("acceptTextures") Then
                                       entry.AcceptTextures = server("acceptTextures").ByteValue = 1
                                   End If
                                   If server.Contains("icon") Then
                                       entry.icon = server("icon").StringValue
                                   End If
                                   ls.Add(entry)
                               Next
                               Application.Current.Dispatcher.Invoke((Sub()
                                                                          MainViewModel.Instance.Servers = ls
                                                                      End Sub))
                           Else
                               MainViewModel.Instance.Servers = New ObservableCollection(Of Server)
                           End If
                       End Sub)

    End Function

    Public Class Server
        Implements INotifyPropertyChanged
        Private _name As String, _ip As String, _hideAddress As Boolean, _icon As String, _serverping As ServerStatus, _AcceptTextures As Boolean
        Public Sub New(name As String, ip As String, hideAddress As Boolean, icon As String, AcceptTextures As Boolean)
            Me.name = name
            Me.ip = ip
            Me.hideAddress = hideAddress
            Me.icon = icon
            Me.AcceptTextures = AcceptTextures
            'If ip.Contains(":") = False Then
            '    ServerPing = New ServerPing(ip)
            'Else
            '    Dim doublepointindex As Integer = ip.IndexOf(":")
            '    ServerPing = New ServerPing(ip.Substring(0, doublepointindex + 1), CInt(ip.Substring(doublepointindex)))
            'End If
        End Sub

        Public Sub New()

        End Sub
        Public Property name As String
            Get
                Return _name
            End Get
            Set(value As String)
                _name = value
                OnPropertyChanged("name")
            End Set
        End Property
        Public Property ip As String
            Get
                Return _ip
            End Get
            Set(value As String)
                _ip = value
                OnPropertyChanged("ip")
            End Set
        End Property

        Public Property hideAddress As Boolean
            Get
                Return _hideAddress
            End Get
            Set(value As Boolean)
                _hideAddress = value
                OnPropertyChanged("hideAddress")
            End Set
        End Property

        Public Property AcceptTextures As Boolean
            Get
                Return _AcceptTextures
            End Get
            Set(value As Boolean)
                _AcceptTextures = value
                OnPropertyChanged("AcceptTextures")
            End Set
        End Property

        Public Property ServerStatus As ServerStatus
            Get
                Return _serverping
            End Get
            Set(value As ServerStatus)
                _serverping = value
                OnPropertyChanged("ServerStatus")
            End Set
        End Property

        Public Property icon As String
            Get
                Return _icon
            End Get
            Set(value As String)
                _icon = value
                OnPropertyChanged("icon")
            End Set
        End Property

        Public Sub DoPing()
            Dim client = New TcpClient
            Dim host As String = Nothing
            Dim port As Integer = Nothing
            client.ReceiveTimeout = 5000
            client.SendTimeout = 5000
            If ip.Contains(":") = True Then
                Dim address() As String = ip.Split(CChar(":"))
                port = CInt(address(1))
                host = address(0)
            Else
                host = ip
                port = 25565
            End If
            Try
                Dim ips As IPAddress() = Dns.GetHostAddresses(host)
                Dim endpoint As IPEndPoint = New IPEndPoint(ips.First(Function(p) p.AddressFamily = AddressFamily.InterNetwork), port)
                client.Connect(endpoint)
                Dim manager = New NetworkManager(client.GetStream)
                manager.WritePacket(New HandshakePacket(
                                    NetworkManager.ProtocolVersion,
                                    endpoint.Address.ToString,
                                    CType(endpoint.Port, System.UInt16),
                                    NetworkMode.Status), PacketDirection.Serverbound)
                manager.WritePacket(New StatusRequestPacket, PacketDirection.Serverbound)
                Dim _response = manager.ReadPacket(PacketDirection.Clientbound)
                If Not TypeOf _response Is StatusResponsePacket Then
                    client.Close()
                    Throw New InvalidOperationException("Server returned invalid ping response")
                End If
                Dim response = CType(_response, StatusResponsePacket)
                Dim sent = DateTime.Now
                manager.WritePacket(New StatusPingPacket(sent.Ticks), PacketDirection.Serverbound)
                Dim _pong = manager.ReadPacket(PacketDirection.Clientbound)
                If Not (TypeOf _pong Is StatusPingPacket) Then
                    client.Close()
                    Throw New InvalidOperationException("Server returned invalid ping response")
                End If
                client.Close()
                Dim pong = CType(_pong, StatusPingPacket)
                Dim time = New DateTime(pong.Time)
                response.Status.Latency = time - sent


                Dim Status As New ServerStatus
                Status.Description = response.Status.Description
                Status.Icon = response.Status.Icon
                Status.Latency = response.Status.Latency
                Status.Players = response.Status.Players
                Status.Version = response.Status.Version
                Status.Online = True
                Dim cleanPath As String = response.Status.Icon
                If Status.Icon IsNot Nothing Then
                    Dim removeString As String = "data:image/png;base64,"
                    If Status.Icon.StartsWith(removeString) Then
                        Dim sourcestring As String = cleanPath
                        Dim index As Integer = sourcestring.IndexOf(removeString)
                        cleanPath = If((index < 0), sourcestring, sourcestring.Remove(index, removeString.Length))
                    End If
                End If
                Status.Icon = cleanPath
                icon = cleanPath
                Me.ServerStatus = Status
            Catch socketex As SocketException
                client.Close()
                ServerStatus = New ServerStatus
                ServerStatus.Online = False
            Catch Ex As Exception
                client.Close()
                ServerStatus = New ServerStatus
                ServerStatus.Online = False
            End Try
        End Sub

        Public Overrides Function ToString() As String
            Return name
        End Function

#Region "PropertyChanged"
        Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

        ''' <summary>
        ''' Raises the PropertyChanged event if needed.
        ''' </summary>
        ''' <param name="propertyName">The name of the property that changed.</param>
        Protected Overridable Sub OnPropertyChanged(propertyName As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
#End Region
    End Class

    Public Class ServerStatus
        Inherits Craft.Net.ServerStatus
        Implements INotifyPropertyChanged

        Private m_online As Boolean
        <JsonIgnore>
        Public Property Online As Boolean
            Get
                Return m_online
            End Get
            Set(value As Boolean)
                m_online = value
                OnPropertyChanged("Online")
            End Set
        End Property

#Region "PropertyChanged"
        Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

        ''' <summary>
        ''' Raises the PropertyChanged event if needed.
        ''' </summary>
        ''' <param name="propertyName">The name of the property that changed.</param>
        Protected Overridable Sub OnPropertyChanged(propertyName As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
#End Region

    End Class

#Region "PropertyChanged"
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    ''' <summary>
    ''' Raises the PropertyChanged event if needed.
    ''' </summary>
    ''' <param name="propertyName">The name of the property that changed.</param>
    Protected Overridable Sub OnPropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
#End Region
End Class
