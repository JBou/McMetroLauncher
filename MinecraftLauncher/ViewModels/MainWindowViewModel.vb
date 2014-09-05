Public Class MainWindowViewModel
    Inherits PropertyChangedBase

#Region "Singleton & Constructor"
    Private Shared _Instance As MainWindowViewModel
    Public Shared ReadOnly Property Instance As MainWindowViewModel
        Get
            If _Instance Is Nothing Then _Instance = New MainWindowViewModel
            Return _Instance
        End Get
    End Property

    Public Sub New()
    End Sub

#End Region

#Region "Properties"
    Private _pb_download_Value As Double
    Private _pb_download_IsIndeterminate As Boolean
    Private _lbl_downloadstatus_Content As String
    Public Property pb_download_Value As Double
        Get
            Return _pb_download_Value
        End Get
        Set(value As Double)
            SetProperty(value, _pb_download_Value)
        End Set
    End Property
    Public Property pb_download_IsIndeterminate As Boolean
        Get
            Return _pb_download_IsIndeterminate
        End Get
        Set(value As Boolean)
            SetProperty(value, _pb_download_IsIndeterminate)
        End Set
    End Property
    Public Property lbl_downloadstatus_Content As String
        Get
            Return _lbl_downloadstatus_Content
        End Get
        Set(value As String)
            SetProperty(value, _lbl_downloadstatus_Content)
        End Set
    End Property
#End Region

End Class
