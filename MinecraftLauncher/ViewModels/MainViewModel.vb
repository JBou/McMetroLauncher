Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Newtonsoft.Json.Linq
Imports System.Text.RegularExpressions
Imports System.Net
Imports MahApps.Metro

Public Class MainViewModel
    Implements INotifyPropertyChanged, IDataErrorInfo
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    ''' <summary>
    ''' Raises the PropertyChanged event if needed.
    ''' </summary>
    ''' <param name="propertyName">The name of the property that changed.</param>
    Protected Overridable Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = "")
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Private m_Servers As New ObservableCollection(Of ServerList.Server)
    Public Property Servers As ObservableCollection(Of ServerList.Server)
        Get
            Return m_Servers
        End Get
        Set(value As ObservableCollection(Of ServerList.Server))
            m_Servers = value
            OnPropertyChanged("Servers")
        End Set
    End Property

    Public Property AppThemes As List(Of AppThemeMenuData)
        Get
            Return GlobalInfos.AppThemes
        End Get
        Set(value As List(Of AppThemeMenuData))
            GlobalInfos.AppThemes = value
            OnPropertyChanged("AppThemes")
        End Set
    End Property

    Public Property AccentColors As List(Of AccentColorMenuData)
        Get
            Return GlobalInfos.AccentColors
        End Get
        Set(value As List(Of AccentColorMenuData))
            GlobalInfos.AccentColors = value
            OnPropertyChanged("AccentColors")
        End Set
    End Property

    Public Property Profiles As ObservableCollection(Of String)
        Get
            Return _profiles
        End Get
        Set(value As ObservableCollection(Of String))
            _profiles = value
            OnPropertyChanged("Profiles")
        End Set
    End Property
    Public Property selectedprofile As String
        Get
            Return McMetroLauncher.Profiles.selectedprofile
        End Get
        Set(value As String)
            McMetroLauncher.Profiles.selectedprofile = value
            OnPropertyChanged("selectedprofile")
        End Set
    End Property

#Region "Minecraft"
    Private _Directjoinaddress As String
    Public Property Directjoinaddress As String
        Get
            Return _Directjoinaddress
        End Get
        Set(value As String)
            _Directjoinaddress = value
            OnPropertyChanged("Directjoinaddress")
            Settings.Settings.ServerAddress = value
            Settings.Save()
        End Set
    End Property
#End Region

#Region "Login"
    Public _username As String
    Public Property Username As String
        Get
            Return _username
        End Get
        Set(value As String)
            If Equals(value, Username) Then
                Return
            Else
                _username = value
                OnPropertyChanged("Username")
            End If
        End Set
    End Property
    Public _onlineMode As Boolean = True
    Public Property OnlineMode As Boolean
        Get
            Return _onlineMode
        End Get
        Set(value As Boolean)
            If Equals(value, Username) Then
                Return
            Else
                _onlineMode = value

                OnPropertyChanged("OnlineMode")
            End If
        End Set
    End Property

#End Region

#Region "Error"
    Public ReadOnly Property [Error]() As String Implements IDataErrorInfo.Error
        Get
            Return String.Empty
        End Get
    End Property

    Default Public ReadOnly Property Item(columnName As String) As String Implements IDataErrorInfo.Item
        Get
            If columnName = "Username" Then
                If String.IsNullOrWhiteSpace(Username) Then
                    Return "Gib einen Benutzernamen ein"
                ElseIf Not OnlineMode Then
                    'Username Validation
                    If Username.Length < 3 Or Username.Length > 16 Then
                        Return "3 - 16 Zeichen"
                    Else
                        Dim regex As New Regex("^[A-Za-z0-9_-]{2,16}$")
                        Dim match As Match = regex.Match(Username)

                        If Not match.Success Then
                            Return "Nur Buchstaben, Zahlen, Binde- und Unterstriche"
                        End If
                    End If
                End If
            End If
            Return String.Empty
        End Get
    End Property

#End Region

    Public Sub New()
        Check_RAM_CPU()
        Check_service_statuses()
    End Sub

#Region "Infos"
    Private _servicestatuses As ObservableCollection(Of AccentColorMenuData) = New ObservableCollection(Of AccentColorMenuData)
    Public Property ServiceStatuses As ObservableCollection(Of AccentColorMenuData)
        Get
            Return _servicestatuses
        End Get
        Set(value As ObservableCollection(Of AccentColorMenuData))
            _servicestatuses = value
            OnPropertyChanged("ServiceStatuses")
        End Set
    End Property

    Public ReadOnly Property CPU As ObservableCollection(Of CPU)
        Get
            Return _cpu
        End Get
    End Property
    Private _profiles As ObservableCollection(Of String) = New ObservableCollection(Of String)

    Private ReadOnly _ram As ObservableCollection(Of Ram) = New ObservableCollection(Of Ram)
    Public ReadOnly Property Ram As ObservableCollection(Of Ram)
        Get
            Return _ram
        End Get
    End Property

    Private ReadOnly _cpu As ObservableCollection(Of CPU) = New ObservableCollection(Of CPU)
    Private CPUCounter As PerformanceCounter = New PerformanceCounter("Processor", "% Processor Time", "_Total")
    Private MemCounter As PerformanceCounter = New PerformanceCounter("Memory", "Available MBytes")
    Private totalram As ULong = My.Computer.Info.TotalPhysicalMemory()

    Public Sub Check_service_statuses()
        Using wc As New WebClient
            Dim status As String = wc.DownloadString("http://status.mojang.com/check")
            Dim ja As JArray = JArray.Parse(status)
            Dim ls As New List(Of AccentColorMenuData)
            For Each Item As JObject In ja
                Dim name As String = Item.Properties.Select(Function(p) p.Name).First
                Dim value As String = Item.Value(Of String)(name)
                Dim service As New AccentColorMenuData() With {.Name = name}
                If value.ToString = "green" Then
                    service.ColorBrush = CType(ThemeManager.Accents.Where(Function(p) p.Name = "Green").First.Resources("AccentColorBrush"), System.Windows.Media.Brush)
                ElseIf value.ToString = "yellow" Then
                    service.ColorBrush = CType(ThemeManager.Accents.Where(Function(p) p.Name = "Yellow").First.Resources("AccentColorBrush"), System.Windows.Media.Brush)
                Else
                    service.ColorBrush = CType(ThemeManager.Accents.Where(Function(p) p.Name = "Red").First.Resources("AccentColorBrush"), System.Windows.Media.Brush)
                End If
                ls.Add(service)
            Next
            ServiceStatuses = New ObservableCollection(Of AccentColorMenuData)(ls)
        End Using
    End Sub

    Sub Check_RAM_CPU()
        Dim totalram As ULong = My.Computer.Info.TotalPhysicalMemory()
        Dim avaiableram As ULong = My.Computer.Info.AvailablePhysicalMemory
        Dim usedram As ULong = totalram - avaiableram
        Dim percentage As Integer = CInt(usedram / totalram * 100)
        _cpu.Add(New CPU() With {.Name = "", .Count = CInt(CPUCounter.NextValue)})
        _ram.Add(New Ram() With {.Name = "", .Count = percentage})



        Dim dispatcherTimer As System.Windows.Threading.DispatcherTimer = New System.Windows.Threading.DispatcherTimer()
        AddHandler dispatcherTimer.Tick, AddressOf dispatcherTimer_Tick
        dispatcherTimer.Interval = New TimeSpan(0, 0, 1)
        dispatcherTimer.Start()
    End Sub

    Private Sub dispatcherTimer_Tick(sender As Object, e As EventArgs)
        Dim totalram As ULong = My.Computer.Info.TotalPhysicalMemory()
        Dim avaiableram As ULong = My.Computer.Info.AvailablePhysicalMemory
        Dim usedram As ULong = totalram - avaiableram
        Dim percentage As Integer = CInt(usedram / totalram * 100)
        _cpu.Item(0).Count = CInt(CPUCounter.NextValue)
        _ram.Item(0).Count = percentage
        '_ram.Item(0).Count = CInt(theMemCounter.NextValue)
    End Sub

#End Region

End Class

#Region "Classes"
Public Class Ram
    Implements INotifyPropertyChanged
    Private _name As String = String.Empty
    Private _count As Integer = 0

    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(value As String)
            _name = value
            NotifyPropertyChanged("Name")
        End Set
    End Property

    Public Property Count() As Integer
        Get
            Return _count
        End Get
        Set(value As Integer)
            _count = value
            NotifyPropertyChanged("Count")
        End Set
    End Property


    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Private Sub NotifyPropertyChanged(<CallerMemberName> Optional propertyName As [String] = "")
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class
Public Class CPU
    Implements INotifyPropertyChanged
    Private _name As String = String.Empty
    Private _count As Integer = 0

    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(value As String)
            _name = value
            NotifyPropertyChanged("Name")
        End Set
    End Property

    Public Property Count() As Integer
        Get
            Return _count
        End Get
        Set(value As Integer)
            _count = value
            NotifyPropertyChanged("Count")
        End Set
    End Property


    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Private Sub NotifyPropertyChanged(<CallerMemberName> Optional propertyName As [String] = "")
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class

#End Region