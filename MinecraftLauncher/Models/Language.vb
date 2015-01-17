Imports Newtonsoft.Json

Public Class Language
    Private _Name As String
    <JsonIgnore>
    Public Property Name() As String
        Get
            Return _Name
        End Get
        Set(ByVal value As String)
            _Name = value
        End Set
    End Property
    Private _Author As String
    <JsonIgnore>
    Public Property Author() As String
        Get
            Return _Author
        End Get
        Set(ByVal value As String)
            _Author = value
        End Set
    End Property
    Private _path As String
    <JsonIgnore>
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

    Public Sub New(Name As String, Author As String, Path As String, Code As String, Icon As Uri)
        Me.Name = Name
        Me.Author = Author
        Me.Path = Path
        Me.Code = Code
        Application.Current.Dispatcher.Invoke(Sub() Me.Icon = New BitmapImage(Icon))
    End Sub
End Class