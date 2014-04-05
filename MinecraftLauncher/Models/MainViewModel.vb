Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public Class MainViewModel
    Private ReadOnly _ram As ObservableCollection(Of Ram) = New ObservableCollection(Of Ram)
    Public ReadOnly Property Ram As ObservableCollection(Of Ram)
        Get
            Return _ram
        End Get
    End Property
    Private ReadOnly _cpu As ObservableCollection(Of CPU) = New ObservableCollection(Of CPU)
    Public ReadOnly Property CPU As ObservableCollection(Of CPU)
        Get
            Return _cpu
        End Get
    End Property
    Private CPUCounter As PerformanceCounter = New PerformanceCounter("Processor", "% Processor Time", "_Total")
    Private MemCounter As PerformanceCounter = New PerformanceCounter("Memory", "Available MBytes")
    Private totalram As ULong = My.Computer.Info.TotalPhysicalMemory()


    Public Sub New()
        Check_RAM_CPU()
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
End Class

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
