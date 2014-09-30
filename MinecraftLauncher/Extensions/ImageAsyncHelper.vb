Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Net
Imports System.Windows.Controls
Imports System.Windows.Data

Namespace Extensions
    Public Class ImageAsyncHelper
        Inherits DependencyObject
        Public Shared Function GetSourceUri(obj As DependencyObject) As Uri
            Return DirectCast(obj.GetValue(SourceUriProperty), Uri)
        End Function
        Public Shared Sub SetSourceUri(obj As DependencyObject, value As Uri)
            obj.SetValue(SourceUriProperty, value)
        End Sub
        Public Shared ReadOnly SourceUriProperty As DependencyProperty = DependencyProperty.RegisterAttached("SourceUri", GetType(Uri), GetType(ImageAsyncHelper), New PropertyMetadata() With {
            .PropertyChangedCallback = Sub(obj, e)
                                           DirectCast(obj, Image).SetBinding(Image.SourceProperty, New Binding("VerifiedUri") With {
                                               .Source = New ImageAsyncHelper() With {
                                                   .GivenUri = DirectCast(e.NewValue, Uri)
                                               },
                                               .IsAsync = True
                                           })

                                       End Sub
        })

        Private GivenUri As Uri
        Public ReadOnly Property VerifiedUri() As Uri
            Get
                Try
                    Dns.GetHostEntry(GivenUri.DnsSafeHost)
                    Return GivenUri
                Catch generatedExceptionName As Exception
                    Return Nothing

                End Try
            End Get
        End Property
    End Class
End Namespace
