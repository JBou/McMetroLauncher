Public Class UpdaterViewModel
    Inherits PropertyChangedBase

#Region "Singleton & Constructor"
    Private Shared _Instance As UpdaterViewModel
    Public Shared ReadOnly Property Instance As UpdaterViewModel
        Get
            If _Instance Is Nothing Then _Instance = New UpdaterViewModel
            Return _Instance
        End Get
    End Property

    Public Sub New()
    End Sub

#End Region

#Region "Properties"
    Private _installerdownloading As Boolean
    Public Property installerdownloading As Boolean
        Get
            Return _installerdownloading
        End Get
        Set(value As Boolean)
            SetProperty(value, _installerdownloading)
        End Set
    End Property
#End Region

End Class
