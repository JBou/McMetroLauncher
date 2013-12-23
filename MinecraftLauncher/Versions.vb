Imports System.Net
Imports System.IO
Imports System.Xml.Linq
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Globalization
Imports System.Net.Sockets

Public Class Versions
    Public Shared o As String
    Public Shared versionsjo As JObject

    Public Shared Sub Load()
        o = File.ReadAllText(outputjsonversions)
        versionsjo = JObject.Parse(o)
        versionsidlist = versionsjo("versions").Select(Function(p) p("id").ToString).ToList()
        versiontypelist = versionsjo("versions").Select(Function(p) p("type").ToString).ToList()
    End Sub

    Public Shared Function latestrelease() As String
        Load()
        Return versionsjo("latest")("release").ToString
    End Function

    Public Shared Function latestsnapshot() As String
        Load()
        Return versionsjo("latest")("snapshot").ToString
    End Function

    Public Shared Function latest() As String
        Load()
        Return versionsjo("versions")(0)("id").ToString
    End Function

    Public Shared Function Versions(version As releasetypes) As IList(Of String)
        Load()
        Dim versionslist As New List(Of String)
        If version = releasetypes.all Then
            Return versionsidlist
        Else
            For i = 0 To versionsidlist.Count - 1
                If versiontypelist.Item(i).ToString = version.ToString Then
                    versionslist.Add(versionsidlist.Item(i).ToString)
                End If
            Next
            Return versionslist
        End If
    End Function

    Public Enum releasetypes
        all
        release
        snapshot
        old_beta
        old_alpha
    End Enum

End Class
