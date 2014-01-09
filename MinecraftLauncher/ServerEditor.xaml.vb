Imports fNbt

Public Class ServerEditor
    Private Newserver As Boolean = False
    Private serversindex As Integer = Nothing

    Public Sub New(serversindex As Integer, name As String, ip As String)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

        Newserver = False
        tb_name.Text = name
        tb_address.Text = ip
        Me.serversindex = serversindex
    End Sub

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

        Newserver = True
        tb_name.Text = Nothing
        tb_address.Text = Nothing
    End Sub

    Private Sub Save()
        Dim nbtserverfile As New NbtFile
        nbtserverfile.LoadFromFile(servers_dat)
        Dim index As Integer = Nothing
        If Newserver = True Then
            index = nbtserverfile.RootTag.Get(Of NbtList)("servers").Count
        Else
            index = serversindex
            nbtserverfile.RootTag.Get(Of NbtList)("servers").RemoveAt(serversindex)
        End If
        Dim tag_name As New NbtString("name", tb_name.Text)
        Dim tag_ip As New NbtString("ip", tb_address.Text)
        Dim tag_hide As New NbtByte("hideAddress", 0)
        Dim item As IList(Of NbtTag) = New List(Of NbtTag)
        item.Add(tag_name)
        item.Add(tag_ip)
        item.Add(tag_hide)
        nbtserverfile.RootTag.Get(Of NbtList)("servers").Insert(index, New NbtCompound(item.AsEnumerable))
        nbtserverfile.SaveToFile(servers_dat, NbtCompression.None)
        DialogResult = True
        Me.Close()
    End Sub


End Class
