Imports System.Windows.Input

Namespace Models
    Public Class SimpleCommand
        Implements ICommand
        Public CanExecuteDelegate As Predicate(Of Object)
        Public ExecuteDelegate As Action(Of Object)

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            If CanExecuteDelegate IsNot Nothing Then
                Return CanExecuteDelegate(parameter)
            End If
            Return True ' if there is no can execute default to true
        End Function

        Public Custom Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged
            AddHandler(ByVal value As EventHandler)
                If CanExecuteDelegate IsNot Nothing Then
                    AddHandler CommandManager.RequerySuggested, value
                End If
            End AddHandler

            RemoveHandler(ByVal value As EventHandler)
                If CanExecuteDelegate IsNot Nothing Then
                    RemoveHandler CommandManager.RequerySuggested, value
                End If
            End RemoveHandler

            RaiseEvent(ByVal sender As Object, ByVal e As System.EventArgs)
                If CanExecuteDelegate IsNot Nothing Then
                    CommandManager.InvalidateRequerySuggested()
                End If
            End RaiseEvent
        End Event
        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            If ExecuteDelegate <> Nothing Then
                ExecuteDelegate(parameter)
            End If
        End Sub
    End Class

End Namespace