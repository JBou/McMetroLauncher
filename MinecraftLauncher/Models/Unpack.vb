Imports System.IO

Public Class Unpack
    Public Shared Property Errormessage As String
    Public Shared Async Function Unpack(input As FileInfo, output As FileInfo) As Task(Of Boolean)
        Errormessage = Nothing
        Dim args As String = String.Join(" ", "-jar", "tools/Pack_xz_Extractor.jar", """" & input.FullName & """", """" & output.FullName & """")
        If input.Exists = True Then
            Dim result As Boolean = Await Task.Run(Function() As Boolean
                                                       Dim p As New Process
                                                       With p.StartInfo
                                                           .FileName = GetJavaPath()
                                                           .Arguments = args
                                                           ' Arbeitsverzeichnis setzen falls nötig
                                                           .WorkingDirectory = ""
                                                           ' kein Window erzeugen
                                                           .CreateNoWindow = True
                                                           ' UseShellExecute auf false setzen
                                                           .UseShellExecute = False
                                                       End With
                                                       AddHandler p.ErrorDataReceived, AddressOf ErrorDataReceived
                                                       AddHandler p.OutputDataReceived, AddressOf OutputDataReceived
                                                       ' Prozess starten
                                                       p.Start()
                                                       p.WaitForExit()
                                                       If p.ExitCode = 0 Then
                                                           Errormessage = Nothing
                                                           Return True
                                                       Else
                                                           'Errormessage = p.StandardError.ReadToEnd
                                                           Return False
                                                       End If
                                                   End Function)
            Return result
        Else
            Errormessage = "The Inputfile does not exist"
            Return False
        End If
    End Function
    Private Shared Sub ErrorDataReceived(sender As Object, e As DataReceivedEventArgs)
        Try
            Errormessage.Insert(Errormessage.Length, e.Data)
        Catch ex As Exception
        End Try
    End Sub

    Private Shared Sub OutputDataReceived(ByVal sender As Object, ByVal e As DataReceivedEventArgs)
        Try
            Errormessage.Insert(Errormessage.Length, e.Data)
        Catch ex As Exception
        End Try
    End Sub

End Class
