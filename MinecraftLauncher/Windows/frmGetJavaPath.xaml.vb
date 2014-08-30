Public Class frmGetJavaPath
    Implements ComponentModel.INotifyPropertyChanged

    Private _JavaPath As String
    Public Property JavaPath() As String
        Get
            Return _JavaPath
        End Get
        Set(ByVal value As String)
            If value <> _JavaPath Then
                _JavaPath = value
                myPropertyChanged("JavaPath")
            End If
        End Set
    End Property

    Private _ChoosePath As RelayCommand
    Public ReadOnly Property ChoosePath As RelayCommand
        Get
            If _ChoosePath Is Nothing Then _ChoosePath = New RelayCommand(Sub(parameter As Object)
                                                                              Dim ofd As New Microsoft.Win32.OpenFileDialog()
                                                                              With ofd
                                                                                  .CheckFileExists = True
                                                                                  .CheckPathExists = True
                                                                                  .InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
                                                                                  .Multiselect = False
                                                                                  .Title = Application.Current.FindResource("OFDJavaPathTitle").ToString()
                                                                                  .Filter = String.Format("{0}|*.exe", Application.Current.FindResource("ExectuableFiles").ToString())
                                                                                  If .ShowDialog() Then
                                                                                      JavaPath = .FileName
                                                                                  End If
                                                                              End With
                                                                          End Sub)
            Return _ChoosePath
        End Get
    End Property
    Private _DownloadJava As RelayCommand
    Public ReadOnly Property DownloadJava As RelayCommand
        Get
            If _DownloadJava Is Nothing Then _DownloadJava = New RelayCommand(Sub(parameter As Object)
                                                                                  Process.Start("http://java.com/de/download")
                                                                              End Sub)
            Return _DownloadJava
        End Get
    End Property

#Region "INotifyPropertyChanged"
    Private Sub myPropertyChanged(propertyname As String)
        RaiseEvent PropertyChanged(Me, New ComponentModel.PropertyChangedEventArgs(propertyname))
    End Sub
    Public Event PropertyChanged(sender As Object, e As ComponentModel.PropertyChangedEventArgs) Implements ComponentModel.INotifyPropertyChanged.PropertyChanged
#End Region
End Class