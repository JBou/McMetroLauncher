Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Newtonsoft.Json.Linq
Imports System.Text.RegularExpressions
Imports System.Net
Imports MahApps.Metro
Imports System

Public Class MainViewModel
    Inherits PropertyChangedBase

#Region "Singleton & Constructor"
    Private Shared _Instance As MainViewModel
    Public Shared ReadOnly Property Instance As MainViewModel
        Get
            If _Instance Is Nothing Then _Instance = New MainViewModel
            Return _Instance
        End Get
    End Property

    Public Sub New()
        Check_RAM_CPU()
        Check_service_statuses()
    End Sub

#End Region

#Region "Properties"

    Private _Servers As New ObservableCollection(Of ServerList.Server)
    Public Property Servers As ObservableCollection(Of ServerList.Server)
        Get
            Return _Servers
        End Get
        Set(value As ObservableCollection(Of ServerList.Server))
            SetProperty(value, _Servers)
        End Set
    End Property
    Private _AppThemes As List(Of AppThemeMenuData) = ThemeManager.AppThemes.Select(Function(a) New AppThemeMenuData() With {.Name = a.Name, .BorderColorBrush = CType(a.Resources("BlackColorBrush"), Windows.Media.Brush), .ColorBrush = CType(a.Resources("WhiteColorBrush"), Windows.Media.Brush)}).ToList
    Public Property AppThemes As List(Of AppThemeMenuData)
        Get
            Return _AppThemes
        End Get
        Set(value As List(Of AppThemeMenuData))
            SetProperty(value, _AppThemes)
        End Set
    End Property
    Private _AccentColors As List(Of AccentColorMenuData) = ThemeManager.Accents.Select(Function(a) New AccentColorMenuData() With {.Name = a.Name, .ColorBrush = CType(a.Resources("AccentColorBrush"), Windows.Media.Brush)}).ToList()
    Public Property AccentColors As List(Of AccentColorMenuData)
        Get
            Return _AccentColors
        End Get
        Set(value As List(Of AccentColorMenuData))
            SetProperty(value, _AccentColors)
        End Set
    End Property
    Private _profiles As ObservableCollection(Of String) = New ObservableCollection(Of String)
    Public Property Profiles As ObservableCollection(Of String)
        Get
            Return _profiles
        End Get
        Set(value As ObservableCollection(Of String))
            SetProperty(value, _profiles)
        End Set
    End Property
    Private _selectedProfile As String
    Public Property selectedProfile As String
        Get
            Return _selectedProfile
        End Get
        Set(value As String)
            SetProperty(value, _selectedProfile)
            Dim o As String = IO.File.ReadAllText(launcher_profiles_json.FullName)
            Dim jo As JObject = JObject.Parse(o)
            jo("selectedProfile") = value
            IO.File.WriteAllText(launcher_profiles_json.FullName, jo.ToString)
        End Set
    End Property
    Private _Settings As Settings
    Public Property Settings() As Settings
        Get
            Return _Settings
        End Get
        Set(ByVal value As Settings)
            SetProperty(value, _Settings)
        End Set
    End Property
    'TODO: Move to Settings
    Private _Directjoinaddress As String
    Public Property Directjoinaddress As String
        Get
            Return _Directjoinaddress
        End Get
        Set(value As String)
            SetProperty(value, _Directjoinaddress)
            MainViewModel.Instance.Settings.ServerAddress = value
            Settings.Save()
        End Set
    End Property
    Private _Account As authenticationDatabase.Account
    Public Property Account As authenticationDatabase.Account
        Get
            Return _Account
        End Get
        Set(value As authenticationDatabase.Account)
            SetProperty(value, _Account)
        End Set
    End Property
#End Region

#Region "Methods"

    Public Async Function LoadSettings() As Task
        Settings = Await Settings.Load()
    End Function

#End Region

#Region "Infos"
    Private _servicestatuses As ObservableCollection(Of AccentColorMenuData) = New ObservableCollection(Of AccentColorMenuData)
    Public Property ServiceStatuses As ObservableCollection(Of AccentColorMenuData)
        Get
            Return _servicestatuses
        End Get
        Set(value As ObservableCollection(Of AccentColorMenuData))
            SetProperty(value, _servicestatuses)
        End Set
    End Property

    Public ReadOnly Property CPU As ObservableCollection(Of CPU)
        Get
            Return _cpu
        End Get
    End Property

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
            Dim ls As New List(Of AccentColorMenuData)
            Try
                Dim status As String = wc.DownloadString("http://status.mojang.com/check")
                Dim ja As JArray = JArray.Parse(status)
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
            Catch ex As WebException
                ls.Add(New AccentColorMenuData() With {.Name = Application.Current.FindResource("ErrorLoading").ToString, .ColorBrush = CType(ThemeManager.Accents.Where(Function(p) p.Name = "Red").First.Resources("AccentColorBrush"), System.Windows.Media.Brush)})
                'TODO: Failed to get Service Statuses
            End Try
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