Imports System.Windows.Input
Imports System.Threading

Public Class SimpleCommand
    Implements ICommand

    Public CanExecuteDelegate As Func(Of Object, Boolean)
    Public ExecuteDelegate As Action(Of Object)

    Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
        If CanExecuteDelegate IsNot Nothing Then
            Return CanExecuteDelegate(parameter)
        End If
        Return True
        ' if there is no can execute default to true
    End Function

    Public Custom Event CanExecuteChanged As EventHandler _
                     Implements ICommand.CanExecuteChanged
        AddHandler(ByVal value As EventHandler)
            AddHandler CommandManager.RequerySuggested, value
        End AddHandler

        RemoveHandler(ByVal value As EventHandler)
            RemoveHandler CommandManager.RequerySuggested, value
        End RemoveHandler

        RaiseEvent(ByVal sender As Object, ByVal e As System.EventArgs)
            CanExecute(sender)
        End RaiseEvent
    End Event

    Public Sub Execute(ByVal parameter As Object) Implements ICommand.Execute
        ExecuteDelegate(parameter)
    End Sub
End Class
