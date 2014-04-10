Imports System.Text
Imports System.Windows
Imports System.Windows.Media.Animation

Namespace JBou.Animations
    Class CryptedTextAnimation
        Inherits AnimationTimeline
        Shared Sub New()
            LengthProperty = DependencyProperty.Register("Length", GetType(Integer), GetType(CryptedTextAnimation))
            TextProperty = DependencyProperty.Register("Text", GetType(String), GetType(CryptedTextAnimation))
        End Sub
        Public Shared ReadOnly LengthProperty As DependencyProperty
        Public Shared ReadOnly TextProperty As DependencyProperty
        Public Shared randomizer As New Random(Environment.TickCount)
        Public Property Length() As Integer
            Get
                Return CInt(GetValue(CryptedTextAnimation.LengthProperty))
            End Get
            Set(value As Integer)
                SetValue(CryptedTextAnimation.LengthProperty, value)
            End Set
        End Property
        Public Property Text() As String
            Get
                Return CStr(GetValue(CryptedTextAnimation.TextProperty))
            End Get
            Set(value As String)
                SetValue(CryptedTextAnimation.TextProperty, value)
            End Set
        End Property
        Private Const CHARACTERS As String = "abcdeghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.,-:;_!§$%&/()=?{[]}\<>|#*~^°"

        Private Const bigCHARACTERS As String = "abcdeghjmnopqrsuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789"
        '+#\-/()<>€@$^&%=?
        Private Const lenght4Chars As String = "k{}f"
        Private Const narrowCHARACTERS As String = "iltI.,;:!|"
        Private previousString As String = String.Empty

        Public Overrides ReadOnly Property TargetPropertyType() As Type
            Get
                Return GetType(String)
            End Get
        End Property

        Protected Overrides Function CreateInstanceCore() As System.Windows.Freezable
            Return New CryptedTextAnimation()
        End Function

        Public Overrides Function GetCurrentValue(defaultOriginValue As Object, defaultDestinationValue As Object, animationClock As AnimationClock) As Object
            If animationClock.CurrentTime.Value.TotalSeconds Mod 0.5 < 0.5 Then
                Dim chars As String = CHARACTERS
                'Nur bei Mincraftia Font
                'If lenght4Chars.Contains(Text) Then
                '    chars = lenght4Chars
                'ElseIf narrowCHARACTERS.Contains(Text) Then
                '    chars = narrowCHARACTERS
                'End If
                Dim str As New StringBuilder(Length)
                For i As Integer = 0 To Length - 1
                    str.Append(chars(randomizer.Next(chars.Length)))
                Next
                previousString = str.ToString()
            End If
            Return previousString
        End Function
    End Class
End Namespace
