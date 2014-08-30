Imports System.ComponentModel
Imports Exceptionless

Public Class SubmitException
    Implements INotifyPropertyChanged

    Private _exception As Exception
    Public Property Exception As Exception
        Get
            Return _exception
        End Get
        Set(value As Exception)
            If value IsNot _exception Then
                _exception = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Exception"))
            End If
        End Set
    End Property

    Private _txtNote As String
    Public Property txtNote() As String
        Get
            Return _txtNote
        End Get
        Set(ByVal value As String)
            If value <> _txtNote Then
                _txtNote = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("txtNote"))
            End If
        End Set
    End Property

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Private _SendReport As RelayCommand
    Public ReadOnly Property SendReport As RelayCommand
        Get
            If _SendReport Is Nothing Then _SendReport = New RelayCommand(Sub(parameter As Object)
                                                                              If Not String.IsNullOrWhiteSpace(txtNote) Then Exception.ToExceptionless().SetUserDescription(txtNote).Submit() Else Exception.ToExceptionless().Submit()
                                                                              ExceptionlessClient.Current.ProcessQueue()
                                                                              Application.Current.Shutdown()
                                                                          End Sub)
            Return _SendReport
        End Get
    End Property

    Private _JustClose As RelayCommand
    Public ReadOnly Property JustClose As RelayCommand
        Get
            If _JustClose Is Nothing Then _JustClose = New RelayCommand(Sub(parameter As Object)
                                                                            Application.Current.Shutdown()
                                                                        End Sub)
            Return _JustClose
        End Get
    End Property
End Class