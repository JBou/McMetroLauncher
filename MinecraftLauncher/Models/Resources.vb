Public Class resourcesindex
    Dim _virtual As Boolean, _objects As IList(Of resourcesindexobject)
    Public Property virtual As Boolean
        Get
            Return _virtual
        End Get
        Set(value As Boolean)
            _virtual = value
        End Set
    End Property

    Public Property objects As IList(Of resourcesindexobject)
        Get
            Return _objects
        End Get
        Set(value As IList(Of resourcesindexobject))
            _objects = value
        End Set
    End Property

    Public Sub New(virtual As Boolean, objects As IList(Of resourcesindexobject))
        Me.virtual = virtual
        Me.objects = objects
    End Sub

End Class

Public Class resourcesindexobject
    Dim _key As String, _hash As String, _size As Integer

    Public Property key As String
        Get
            Return _key
        End Get
        Set(value As String)
            _key = value
        End Set
    End Property

    Public Property hash As String
        Get
            Return _hash
        End Get
        Set(value As String)
            _hash = value
        End Set
    End Property

    Public Property size As Integer
        Get
            Return _size
        End Get
        Set(value As Integer)
            _size = value
        End Set
    End Property

    Public Sub New(key As String, hash As String, size As Integer)
        Me.key = key
        Me.hash = hash
        Me.size = size
    End Sub

End Class