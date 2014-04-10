Imports System.IO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class Versionslist
    Public Sub New()
        versions = New List(Of Version)
        latest = New Latestversion
    End Sub
    Public ReadOnly Property latest_version() As Version
        Get
            If versions.Count > 0 Then
                Return versions.First
            Else
                Return Nothing
            End If
        End Get
    End Property
    Public Property latest() As Latestversion
        Get
            Return m_latest
        End Get
        Set(value As Latestversion)
            m_latest = value
        End Set
    End Property
    Private m_latest As Latestversion
    Public Property versions() As List(Of Version)
        Get
            Return m_versions
        End Get
        Set(value As List(Of Version))
            m_versions = value
        End Set
    End Property
    Private m_versions As List(Of Version)
    Public Class Latestversion
        Public Sub New()

        End Sub
        Public Property snapshot() As String
            Get
                Return m_snapshot
            End Get
            Set(value As String)
                m_snapshot = value
            End Set
        End Property
        Private m_snapshot As String
        Public Property release() As String
            Get
                Return m_release
            End Get
            Set(value As String)
                m_release = value
            End Set
        End Property
        Private m_release As String
    End Class
    Public Class Version
        Public Sub New()
        End Sub
        Public Property id() As String
            Get
                Return m_id
            End Get
            Set(value As String)
                m_id = value
            End Set
        End Property
        Private m_id As String
        Public Property time() As String
            Get
                Return m_time
            End Get
            Set(value As String)
                m_time = value
            End Set
        End Property
        Private m_time As String
        Public Property releaseTime() As String
            Get
                Return m_releaseTime
            End Get
            Set(value As String)
                m_releaseTime = value
            End Set
        End Property
        Private m_releaseTime As String
        Public Property type() As String
            Get
                Return m_type
            End Get
            Set(value As String)
                m_type = value
            End Set
        End Property
        Private m_type As String
        Public Property custom As Boolean
            Get
                Return m_custom
            End Get
            Set(value As Boolean)
                m_custom = value
            End Set
        End Property
        Private m_custom As Boolean
    End Class

End Class
