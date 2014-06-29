Imports System.Runtime.CompilerServices
Imports System.IO

Public Module ExtensionMethods

    <Extension()>
    Public Function IsLocked(ByVal file As FileInfo) As Boolean
        Dim stream As FileStream = Nothing

        Try
            stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None)
        Catch generatedExceptionName As IOException
            'the file is unavailable because it is:
            'still being written to
            'or being processed by another thread
            'or does not exist (has already been processed)
            Return True
        Finally
            If stream IsNot Nothing Then
                stream.Close()
            End If
        End Try

        'file is not locked
        Return False
    End Function

End Module
