Imports MahApps.Metro
Imports System.Windows.Media
Imports McMetroLauncher.Models

Public Class AccentColorMenuData
    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(value As String)
            m_Name = value
        End Set
    End Property
    Private m_Name As String
    Public Property BorderColorBrush() As Brush
        Get
            Return m_BorderColorBrush
        End Get
        Set(value As Brush)
            m_BorderColorBrush = value
        End Set
    End Property
    Private m_BorderColorBrush As Brush
    Public Property ColorBrush() As Brush
        Get
            Return m_ColorBrush
        End Get
        Set(value As Brush)
            m_ColorBrush = value
        End Set
    End Property
    Private m_ColorBrush As Brush

    Private m_changeAccentCommand As ICommand

    Public ReadOnly Property ChangeAccentCommand() As ICommand
        Get
            Return If(Me.m_changeAccentCommand, (InlineAssignHelper(m_changeAccentCommand, New SimpleCommand() With { _
                .CanExecuteDelegate = Function(x) True, _
                .ExecuteDelegate = Sub() DoChangeTheme() _
            })))
        End Get
    End Property

    Protected Overridable Sub DoChangeTheme()
        Dim theme = ThemeManager.DetectAppStyle(Application.Current)
        Dim accent = ThemeManager.GetAccent(Me.Name)
        ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1)
    End Sub
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function
End Class

Public Class AppThemeMenuData
    Inherits AccentColorMenuData
    Protected Overrides Sub DoChangeTheme()
        Dim theme = ThemeManager.DetectAppStyle(Application.Current)
        Dim appTheme = ThemeManager.GetAppTheme(Me.Name)
        ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, appTheme)
    End Sub
End Class