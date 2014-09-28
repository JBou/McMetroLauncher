Imports Exceptionless
Imports System
Imports System.IO
Imports System.Reflection
Imports System.Configuration

Class Application

    ' Ereignisse auf Anwendungsebene wie Startup, Exit und DispatcherUnhandledException
    ' können in dieser Datei verarbeitet werden.
    Private WithEvents myDomain As AppDomain = AppDomain.CurrentDomain

#If Not Debug Then
    Dim IsHandled As Boolean = False
    Private Sub Dispatcher_UnhandledExceptions(sender As Object, e As Windows.Threading.DispatcherUnhandledExceptionEventArgs)
        e.Handled = True
        OnExceptionOccurred(e.Exception)
    End Sub

    Private Sub AppDomain_UnhandledException(sender As Object, e As UnhandledExceptionEventArgs)
        OnExceptionOccurred(DirectCast(e.ExceptionObject, Exception))
    End Sub

    Private Sub Application_DispatcherUnhandledException(sender As Object, e As Windows.Threading.DispatcherUnhandledExceptionEventArgs) Handles Me.DispatcherUnhandledException
        e.Handled = True
        OnExceptionOccurred(e.Exception)
    End Sub

    Private Sub OnExceptionOccurred(ex As Exception)
        If Not IsHandled Then
            IsHandled = True
            Dim frm As New SubmitException() With {.Exception = ex}
            frm.ShowDialog()
        End If
    End Sub

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
        AddHandler MyDomain.UnhandledException, AddressOf AppDomain_UnhandledException
        AddHandler Dispatcher.UnhandledException, AddressOf Dispatcher_UnhandledExceptions
        ExceptionlessClient.Current.Register(False)
    End Sub
#End If

    'Private Function CurrentDomain_AssemblyResolve(sender As Object, args As ResolveEventArgs) As Assembly Handles myDomain.ReflectionOnlyAssemblyResolve
    '    Dim assemblyPath As New FileInfo(Path.Combine(Librariespath.FullName, New AssemblyName(args.Name).Name & ".dll"))
    '    If Not assemblyPath.Exists Then Return Nothing
    '    Return Assembly.LoadFrom(assemblyPath.FullName)
    'End Function

End Class
