Public Class Login




    Private Sub Hyperlink_RequestNavigate(sender As Object, e As RequestNavigateEventArgs)
        Process.Start(New ProcessStartInfo(e.Uri.AbsoluteUri))
        e.Handled = True
    End Sub
    Private Sub open_register()
        Process.Start("https://account.mojang.com/register")
    End Sub
End Class
