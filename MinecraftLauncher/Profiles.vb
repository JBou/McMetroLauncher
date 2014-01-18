Imports System.Net
Imports System.IO
Imports System.Xml.Linq
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Globalization
Imports Ionic.Zip
Imports System.Net.Sockets

Public Class Profiles
    Public Shared o As String
    Public Shared profilesjo As JObject

    Public Shared Sub Load()
        o = File.ReadAllText(launcher_profiles_json)
        profilesjo = JObject.Parse(o)
    End Sub

    Public Shared Function PropertyList(profilename As String) As IList(Of String)
        Load()
        Return profilesjo("profiles").Value(Of JObject)(profilename).Properties.Select(Function(p) p.Name).ToList()
    End Function

    Public Shared Function List() As IList(Of String)
        Load()
        Return profilesjo.Value(Of JObject)("profiles").Properties.Select(Function(p) p.Name).ToList()
    End Function

    Public Shared Function name(profilename As String) As String
        Load()
        If PropertyList(profilename).Contains("name") = True Then
            Return profilesjo("profiles")(profilename)("name").ToString
        Else
            Return Nothing
        End If
    End Function

    Public Shared Function gameDir(profilename As String) As String
        Load()
        If PropertyList(profilename).Contains("gameDir") = True Then
            Return profilesjo("profiles")(profilename)("gameDir").ToString
        Else
            Return Nothing
        End If
    End Function

    Public Shared Function lastVersionId(profilename As String) As String
        Load()
        If PropertyList(profilename).Contains("lastVersionId") = True Then
            Return profilesjo("profiles")(profilename)("lastVersionId").ToString
        Else
            Return Nothing
        End If
    End Function

    Public Shared Function launcherVisibilityOnGameClose(profilename As String) As String
        Load()
        If PropertyList(profilename).Contains("launcherVisibilityOnGameClose") = True Then
            Return profilesjo("profiles")(profilename)("launcherVisibilityOnGameClose").ToString
        Else
            Return Nothing
        End If
    End Function

    Public Shared Function playerUUID(profilename As String) As String
        Load()
        If PropertyList(profilename).Contains("playerUUID") = True Then
            Return profilesjo("profiles")(profilename)("playerUUID").ToString
        Else
            Return Nothing
        End If
    End Function

    Public Shared Function allowedReleaseTypes(profilename As String) As IList(Of String)
        Load()
        If PropertyList(profilename).Contains("allowedReleaseTypes") = True Then
            Dim ja As JArray = profilesjo("profiles")(profilename).Value(Of JArray)("allowedReleaseTypes")
            Dim versionsinfo As IList(Of String) = ja.Values(Of String).ToList
            Return versionsinfo
        Else
            Return New List(Of String)
        End If
    End Function

    Public Shared Function javaDir(profilename As String) As String
        Load()
        If PropertyList(profilename).Contains("javaDir") = True Then
            Return profilesjo("profiles")(profilename)("javaDir").ToString
        Else
            Return Nothing
        End If
    End Function

    Public Shared Function javaArgs(profilename As String) As String
        Load()
        If PropertyList(profilename).Contains("javaArgs") = True Then
            Return profilesjo("profiles")(profilename)("javaArgs").ToString
        Else
            Return Nothing
        End If
    End Function

    Public Shared Function resolution_width(profilename As String) As String
        Load()
        If PropertyList(profilename).Contains("resolution") = True Then
            Return profilesjo("profiles")(profilename)("resolution")("width").ToString
        Else
            Return Nothing
        End If
    End Function

    Public Shared Function resolution_height(profilename As String) As String
        Load()
        If PropertyList(profilename).Contains("resolution") = True Then
            Return profilesjo("profiles")(profilename)("resolution")("height").ToString
        Else
            Return Nothing
        End If
    End Function

    Public Shared Sub Add(ByVal Profile As Profile)
        Load()
        Dim Profileproperties As JObject = New JObject(New JProperty("name", Profile.name))
        If Profile.gameDir <> Nothing Then
            Profileproperties.Add(New JProperty("gameDir", Profile.gameDir))
        End If
        If Profile.lastVersionId <> Nothing Then
            Profileproperties.Add(New JProperty("lastVersionId", Profile.lastVersionId))
        End If
        If Profile.javaDir <> Nothing Then
            Profileproperties.Add(New JProperty("javaDir", Profile.javaDir))
        End If
        If Profile.javaArgs <> Nothing Then
            Profileproperties.Add(New JProperty("javaArgs", Profile.javaArgs))
        End If
        If Profile.resolution_height <> Nothing And Profile.resolution_width <> Nothing Then
            Profileproperties.Add(New JProperty("resolution", New JObject(New JProperty("width", Profile.resolution_width), New JProperty("height", Profile.resolution_height))))
        End If
        If Profile.allowedReleaseTypes IsNot Nothing Then
            Profile.allowedReleaseTypes.Insert(0, "release")
            Profileproperties.Add(New JProperty("allowedReleaseTypes", New JArray(Profile.allowedReleaseTypes.Select(Function(p) p.ToString))))
        End If
        profilesjo.Value(Of JObject)("profiles").Add(New JProperty(Profile.name, Profileproperties))
        profilesjo("selectedProfile") = Profile.name
        File.WriteAllText(launcher_profiles_json, profilesjo.ToString)
    End Sub

    Public Shared Sub Edit(editprofilename As String, Profile As Profile)
        Load()
        Dim Profileproperties As JObject = New JObject(New JProperty("name", Profile.name))
        If Profile.gameDir <> Nothing Then
            Profileproperties.Add(New JProperty("gameDir", Profile.gameDir))
        End If
        If Profile.lastVersionId <> Nothing Then
            Profileproperties.Add(New JProperty("lastVersionId", Profile.lastVersionId))
        End If
        If Profile.javaDir <> Nothing Then
            Profileproperties.Add(New JProperty("javaDir", Profile.javaDir))
        End If
        If Profile.javaArgs <> Nothing Then
            Profileproperties.Add(New JProperty("javaArgs", Profile.javaArgs))
        End If
        If Profile.resolution_height <> Nothing And Profile.resolution_width <> Nothing Then
            Profileproperties.Add(New JProperty("resolution", New JObject(New JProperty("width", Profile.resolution_width), New JProperty("height", Profile.resolution_height))))
        End If
        If Profile.allowedReleaseTypes IsNot Nothing Then
            Profile.allowedReleaseTypes.Insert(0, "release")
            Profileproperties.Add(New JProperty("allowedReleaseTypes", New JArray(Profile.allowedReleaseTypes.Select(Function(p) p.ToString))))
        End If
        profilesjo.Value(Of JObject)("profiles").Property(editprofilename).Replace(New JProperty(Profile.name, Profileproperties))
        profilesjo("selectedProfile") = Profile.name
        File.WriteAllText(launcher_profiles_json, profilesjo.ToString)
    End Sub

    Public Shared Sub Remove(profilename As String)
        Load()
        profilesjo.Value(Of JObject)("profiles").Property(profilename).Remove()
        IO.File.WriteAllText(launcher_profiles_json, profilesjo.ToString)
    End Sub

    Public Class Profile
        Private _name As String, _gameDir As String, _lastVersionId As String, _javaDir As String,
            _javaArgs As String, _resolution_width As String, _resolution_height As String, _allowedReleaseTypes As IList(Of String), _playerUUID As String, _launcherVisibilityOnGameClose As String

        Public Property name As String
            Get
                Return _name
            End Get
            Set(value As String)
                _name = value
            End Set
        End Property

        Public Property gameDir As String
            Get
                Return _gameDir
            End Get
            Set(value As String)
                _gameDir = value
            End Set
        End Property
        Public Property launcherVisibilityOnGameClose As String
            Get
                Return _launcherVisibilityOnGameClose
            End Get
            Set(value As String)
                _launcherVisibilityOnGameClose = value
            End Set
        End Property

        Public Property playerUUID As String
            Get
                Return _playerUUID
            End Get
            Set(value As String)
                _playerUUID = value
            End Set
        End Property

        Public Property lastVersionId As String
            Get
                Return _lastVersionId
            End Get
            Set(value As String)
                _lastVersionId = value
            End Set
        End Property

        Public Property javaDir As String
            Get
                Return _javaDir
            End Get
            Set(value As String)
                _javaDir = value
            End Set
        End Property

        Public Property javaArgs As String
            Get
                Return _javaArgs
            End Get
            Set(value As String)
                _javaArgs = value
            End Set
        End Property

        Public Property resolution_width As String
            Get
                Return _resolution_width
            End Get
            Set(value As String)
                _resolution_width = value
            End Set
        End Property

        Public Property resolution_height As String
            Get
                Return _resolution_height
            End Get
            Set(value As String)
                _resolution_height = value
            End Set
        End Property


        Public Property allowedReleaseTypes As IList(Of String)
            Get
                Return _allowedReleaseTypes
            End Get
            Set(value As IList(Of String))
                _allowedReleaseTypes = value
            End Set
        End Property

        Public Sub New()
        End Sub
    End Class
End Class
