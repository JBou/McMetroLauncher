Public Class ButtonHelper
    Public Shared Function GetDialogResult(obj As DependencyObject) As System.Nullable(Of Boolean)
        Return CType(obj.GetValue(DialogResultProperty), System.Nullable(Of Boolean))
    End Function
    Public Shared Sub SetDialogResult(obj As DependencyObject, value As System.Nullable(Of Boolean))
        obj.SetValue(DialogResultProperty, value)
    End Sub
    Public Shared ReadOnly DialogResultProperty As DependencyProperty = DependencyProperty.RegisterAttached("DialogResult",
                                                                                                            GetType(System.Nullable(Of Boolean)), GetType(ButtonHelper),
                                                                                                            New UIPropertyMetadata() With {.PropertyChangedCallback = Sub(obj, e)
                                                                                                                                                                          Dim button As Button = TryCast(obj, Button)
                                                                                                                                                                          If button Is Nothing Then
                                                                                                                                                                              Throw New InvalidOperationException("Can only use ButtonHelper.DialogResult on a Button control")
                                                                                                                                                                          End If
                                                                                                                                                                          AddHandler button.Click, Sub(sender, e2)
                                                                                                                                                                                                       Window.GetWindow(Button).DialogResult = GetDialogResult(Button)
                                                                                                                                                                                                   End Sub

                                                                                                                                                                      End Sub})
End Class