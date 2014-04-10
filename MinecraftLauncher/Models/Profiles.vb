﻿Imports System.Net
Imports System.IO
Imports System.Xml.Linq
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Globalization
Imports Ionic.Zip
Imports System.Net.Sockets
Imports System.ComponentModel

Public Class Profiles
    Public Shared o As String
    Public Shared profilesjo As JObject

    Public Shared Sub Load()
        o = File.ReadAllText(launcher_profiles_json.FullName)
        profilesjo = JObject.Parse(o)
    End Sub

    Public Shared Async Function FromName(profilename As String) As Task(Of Profile)
        Load()
        Dim profile As Profile = Await JsonConvert.DeserializeObjectAsync(Of Profile)(profilesjo("profiles")(profilename).ToString)
        Return profile
    End Function

    Public Shared Function List() As IList(Of String)
        Load()
        Return profilesjo.Value(Of JObject)("profiles").Properties.Select(Function(p) p.Name).ToList()
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
        If Profile.resolution IsNot Nothing Then
            If Profile.resolution.height = Nothing Then
                Profile.resolution.height = "480"
            End If
            If Profile.resolution.width = Nothing Then
                Profile.resolution.width = "854"
            End If
            Profileproperties.Add(New JProperty("resolution", New JObject(New JProperty("width", Profile.resolution.width), New JProperty("height", Profile.resolution.height))))
        End If
        If Profile.allowedReleaseTypes IsNot Nothing Then
            Profile.allowedReleaseTypes.Insert(0, "release")
            Profileproperties.Add(New JProperty("allowedReleaseTypes", New JArray(Profile.allowedReleaseTypes.Select(Function(p) p.ToString))))
        End If
        profilesjo.Value(Of JObject)("profiles").Add(New JProperty(Profile.name, Profileproperties))
        profilesjo("selectedProfile") = Profile.name
        File.WriteAllText(launcher_profiles_json.FullName, profilesjo.ToString)
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
        If Profile.resolution IsNot Nothing Then
            If Profile.resolution.height = Nothing Then
                Profile.resolution.height = "480"
            End If
            If Profile.resolution.width = Nothing Then
                Profile.resolution.width = "854"
            End If
            Profileproperties.Add(New JProperty("resolution", New JObject(New JProperty("width", Profile.resolution.width), New JProperty("height", Profile.resolution.height))))
        End If
        If Profile.allowedReleaseTypes IsNot Nothing Then
            Profile.allowedReleaseTypes.Insert(0, "release")
            Profileproperties.Add(New JProperty("allowedReleaseTypes", New JArray(Profile.allowedReleaseTypes.Select(Function(p) p.ToString))))
        End If
        profilesjo.Value(Of JObject)("profiles").Property(editprofilename).Replace(New JProperty(Profile.name, Profileproperties))
        profilesjo("selectedProfile") = Profile.name
        File.WriteAllText(launcher_profiles_json.FullName, profilesjo.ToString)
    End Sub

    Public Shared Sub Remove(profilename As String)
        Load()
        profilesjo.Value(Of JObject)("profiles").Property(profilename).Remove()
        IO.File.WriteAllText(launcher_profiles_json.FullName, profilesjo.ToString)
    End Sub

    Public Shared Sub Get_Profiles()
        Profiles.Load()
        Dim jo As JObject = Profiles.profilesjo
        ViewModel.Profiles.Clear()
        For Each Profile As String In Profiles.List
            ViewModel.Profiles.Add(Profile)
        Next
        If jo.Properties.Select(Function(p) p.Name).Contains("selectedProfile") = True Then
            ViewModel.selectedprofile = jo("selectedProfile").ToString
        Else
            jo.Add(New JProperty("selectedProfile", ViewModel.Profiles.First))
            ViewModel.selectedprofile = ViewModel.Profiles.First
        End If
        If Profiles.List.Count = 0 Then
            'StandartProfile schreiben
            Dim standartprofile As New JObject(
            New JProperty("profiles",
                New JObject(
                    New JProperty("Default",
                        New JObject(
                            New JProperty("name", "Default"))))),
            New JProperty("selectedProfile", "Default"))
            IO.File.WriteAllText(launcher_profiles_json.FullName, standartprofile.ToString)
            Get_Profiles()
        End If
    End Sub

    Public Class Profile
        Private _name As String, _gameDir As String, _lastVersionId As String, _javaDir As String,
            _javaArgs As String, _resolution As cls_Resolution, _allowedReleaseTypes As IList(Of String),
            _playerUUID As String, _launcherVisibilityOnGameClose As String

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

        Public Property resolution As cls_Resolution
            Get
                Return _resolution
            End Get
            Set(value As cls_Resolution)
                _resolution = value
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
            resolution = New cls_Resolution
        End Sub

        Public Class cls_Resolution
            Private m_height, m_width As String
            Public Property height As String
                Get
                    Return m_height
                End Get
                Set(value As String)
                    m_height = value
                End Set
            End Property
            Public Property width As String
                Get
                    Return m_width
                End Get
                Set(value As String)
                    m_width = value
                End Set
            End Property
            Public Sub New()

            End Sub
        End Class
    End Class
End Class
