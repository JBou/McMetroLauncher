Imports Newtonsoft.Json

Public Class Language
    Private _Name As String
    <JsonIgnore> _
    Public Property Name() As String
        Get
            Return _Name
        End Get
        Set(ByVal value As String)
            _Name = value
        End Set
    End Property

    Private _path As String
    Public Property Path() As String
        Get
            Return _path
        End Get
        Set(ByVal value As String)
            _path = value
        End Set
    End Property

    Private _code As String
    Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            _code = value
        End Set
    End Property

    Private _icon As BitmapSource
    <JsonIgnore>
    Public Property Icon() As BitmapSource
        Get
            Return _icon
        End Get
        Set(ByVal value As BitmapSource)
            _icon = value
        End Set
    End Property

    Public Sub New()
    End Sub

    Public Sub New(Name As String, Path As String, Code As String, Icon As Uri)
        Me.Name = Name
        Me.Path = Path
        Me.Code = Code
        Application.Current.Dispatcher.Invoke(Sub() Me.Icon = New BitmapImage(Icon))
    End Sub
End Class