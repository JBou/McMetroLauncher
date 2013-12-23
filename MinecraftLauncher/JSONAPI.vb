'Imports tman0.JsonAPI2
'Imports System.IO
'Imports System.Net
'Imports System.Windows.Threading

'Class cs_JSONAPI

'    WithEvents JSONAPI As JSONAPI

'    Private Shared Sub Main(args As String())
'        Dim j = New JSONAPI("localhost", 20059, "admin", "password")
'        j.Connect()
'        j.Subscribe("console")
'        While j.Connected = True
'            Dim line = Console.ReadLine()
'            j.Call("runConsoleCommand", Nothing, line)
'        End While
'    End Sub
'End Class
