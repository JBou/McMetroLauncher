Public Class Server
    Private _name As String, _ip As String, _hideAddress As Boolean, _icon As BitmapSource, _serverping As ServerPing
    Public Sub New(name As String, ip As String, hideAddress As Boolean, icon As BitmapSource)
        Me.name = name
        Me.ip = ip
        Me.hideAddress = hideAddress
        Me.icon = icon
        'If ip.Contains(":") = False Then
        '    ServerPing = New ServerPing(ip)
        'Else
        '    Dim doublepointindex As Integer = ip.IndexOf(":")
        '    ServerPing = New ServerPing(ip.Substring(0, doublepointindex + 1), CInt(ip.Substring(doublepointindex)))
        'End If
    End Sub
    Public Property name As String
        Get
            Return _name
        End Get
        Set(value As String)
            _name = value
        End Set
    End Property

    Public Property ip As String
        Get
            Return _ip
        End Get
        Set(value As String)
            _ip = value
        End Set
    End Property

    Public Property hideAddress As Boolean
        Get
            Return _hideAddress
        End Get
        Set(value As Boolean)
            _hideAddress = value
        End Set
    End Property

    Public Property ServerPing As ServerPing
        Get
            Return _serverping
        End Get
        Set(value As ServerPing)
            _serverping = value
        End Set
    End Property

    Public Property icon As BitmapSource
        Get
            Return _icon
        End Get
        Set(value As BitmapSource)
            _icon = value
        End Set
    End Property

End Class
