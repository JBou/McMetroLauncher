Imports System.ComponentModel
Imports System.Text.RegularExpressions

Public Class LoginViewModel
    Inherits PropertyChangedBase
    Implements IDataErrorInfo

#Region "Singleton & Constructor"
    Private Shared _Instance As LoginViewModel
    Public Shared ReadOnly Property Instance As LoginViewModel
        Get
            If _Instance Is Nothing Then _Instance = New LoginViewModel
            Return _Instance
        End Get
    End Property

    Public Sub New()
    End Sub

#End Region

#Region "Properties"
    Public _username As String
    Public Property Username As String
        Get
            Return _username
        End Get
        Set(value As String)
            SetProperty(value, _username)
        End Set
    End Property
    Public _onlineMode As Boolean = True
    Public Property OnlineMode As Boolean
        Get
            Return _onlineMode
        End Get
        Set(value As Boolean)
            SetProperty(value, _onlineMode)
        End Set
    End Property

#End Region

#Region "Error"
    Public ReadOnly Property [Error]() As String Implements IDataErrorInfo.Error
        Get
            Return String.Empty
        End Get
    End Property

    Default Public ReadOnly Property Item(columnName As String) As String Implements IDataErrorInfo.Item
        Get
            If columnName = "Username" Then
                If String.IsNullOrWhiteSpace(Username) Then
                    Return Application.Current.FindResource("UsernameMissing").ToString
                ElseIf Not OnlineMode Then
                    'Username Validation
                    If Username.Length < 3 OrElse Username.Length > 16 Then
                        Return "3 - 16 " & Application.Current.FindResource("Characters").ToString
                    Else
                        Dim regex As New Regex("^[A-Za-z0-9_-]{2,16}$")
                        Dim match As Match = regex.Match(Username)

                        If Not match.Success Then
                            Return Application.Current.FindResource("NoSpecialChars").ToString
                        End If
                    End If
                End If
            End If
            Return String.Empty
        End Get
    End Property

#End Region

End Class
