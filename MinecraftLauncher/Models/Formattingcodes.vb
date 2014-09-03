Imports System.Windows.Media
Imports System.Drawing.Text
Imports System.Runtime.InteropServices
Imports System.IO

Public Class FormattingCodes
    Public Shared Function ParseFormattedtext(text As String) As IList(Of FormattedText)
        'http://minecraft.gamepedia.com/Formatting_codes
        Dim Color As Color = Colors.Black
        Dim Obfuscated As Boolean = False
        Dim Bold As Boolean = False
        Dim Strikethrough As Boolean = False
        Dim Underline As Boolean = False
        Dim Italic As Boolean = False
        Dim FormattedTextlist As IList(Of FormattedText) = New List(Of FormattedText)
        For i = 0 To text.Length - 1
            If i > 1 Then
                If text.ElementAt(i - 2) = "§" Then
                    Dim substring As String = text.Substring(i - 2, 2)
                    If Colorcodes.Select(Function(p) p.Code).Contains(substring) Then
                        Color = Colorcodes.Where(Function(p) p.Code = substring).First.Color
                        'If a color code is used after a formatting code, the formatting code will be disabled beyond the color code point. 
                        'For example, §cX§nY displays as XY, whereas §nX§cY displays as XY. 
                        'Therefore, when using a color code in tandem with a formatting code, ensure the color code is used first and reuse the formatting code when changing colors.
                        Obfuscated = False
                        Bold = False
                        Strikethrough = False
                        Underline = False
                        Italic = False
                    ElseIf substring = "§r" Then
                        Color = Colors.Black
                        Obfuscated = False
                        Bold = False
                        Strikethrough = False
                        Underline = False
                        Italic = False
                    ElseIf Formattingcodes.Select(Function(p) p.Code).Contains(substring) Then
                        Select Case substring
                            Case "§k"
                                Obfuscated = True
                            Case "§l"
                                Bold = True
                            Case "§m"
                                Strikethrough = True
                            Case "§n"
                                Underline = True
                            Case "§o"
                                Italic = True
                        End Select
                    End If
                End If
            End If
            FormattedTextlist.Add(New FormattedText() With {.Bold = Bold, .Color = Color, .Italic = Italic, .Obfuscated = Obfuscated, .Strikethrough = Strikethrough, .Text = text.ElementAt(i), .Underline = Underline})
        Next
        'Color- und  löschen:
        Dim index As Integer = 0
        Do Until index > FormattedTextlist.Count - 2
            Dim str1 As String = FormattedTextlist.ElementAt(index).Text
            Dim str2 As String = FormattedTextlist.ElementAt(index + 1).Text
            Dim substring As String = String.Join(Nothing, str1, str2)
            If Formattingcodes.Select(Function(p) p.Code).Contains(substring) = True OrElse Colorcodes.Select(Function(p) p.Code).Contains(substring) = True Then
                FormattedTextlist.RemoveAt(index)
                FormattedTextlist.RemoveAt(index)
            Else
                index += 1
            End If
        Loop
        Return FormattedTextlist
    End Function
    Public Shared Function MinecraftText2Document(text As String) As FlowDocument
        Dim document As FlowDocument = New FlowDocument()
        Dim FormattedTextlist As IList(Of FormattingCodes.FormattedText) = ParseFormattedtext(text)
        ' Dim stackpanel As New StackPanel() With {.Orientation = Orientation.Horizontal}
        Dim stackpanel As New WrapPanel
        For Each item As FormattingCodes.FormattedText In FormattedTextlist
            'Whitespace underline does'nt work. So replace it with no-break-space, then it works
            item.Text = item.Text.Replace(Chr(32), Chr(160))
            Dim tblock As New TextBlock() With {.Text = item.Text, .Foreground = New SolidColorBrush(item.Color)}
            'Formatted Text don't looks good with this Font:
            'tblock.FontFamily = New FontFamily("Minecraftia")
            'tblock.FontFamily = New FontFamily("Lucida Sans")
            tblock.FontFamily = New FontFamily("ProFontWindows")
            If item.Obfuscated = True Then tblock.BeginAnimation(TextBlock.TextProperty, New JBou.Animations.CryptedTextAnimation() With {.Text = item.Text, .Length = item.Text.Length, .Duration = Duration.Forever})
            If item.Bold = True Then tblock.FontWeight = FontWeights.Bold
            If item.Strikethrough = True Then tblock.TextDecorations.Add(TextDecorations.Strikethrough)
            If item.Underline = True Then tblock.TextDecorations.Add(TextDecorations.Underline)
            If item.Italic = True Then tblock.FontStyle = FontStyles.Italic
            stackpanel.Children.Add(tblock)
        Next
        document.Blocks.Add(New BlockUIContainer(stackpanel))
        Return document
    End Function

    Public Shared Colorcodes As IList(Of ColorCode) = New List(Of ColorCode) From
    {
        New ColorCode() With {.Name = "Black", .Color = DirectCast(ColorConverter.ConvertFromString("#000000"), Color), .Code = "§0"},
        New ColorCode() With {.Name = "Dark Blue", .Color = DirectCast(ColorConverter.ConvertFromString("#0000AA"), Color), .Code = "§1"},
        New ColorCode() With {.Name = "Dark Green", .Color = DirectCast(ColorConverter.ConvertFromString("#00AA00"), Color), .Code = "§2"},
        New ColorCode() With {.Name = "Dark Aqua", .Color = DirectCast(ColorConverter.ConvertFromString("#00AAAA"), Color), .Code = "§3"},
        New ColorCode() With {.Name = "Dark Red", .Color = DirectCast(ColorConverter.ConvertFromString("#AA0000"), Color), .Code = "§4"},
        New ColorCode() With {.Name = "Dark Purple", .Color = DirectCast(ColorConverter.ConvertFromString("#AA00AA"), Color), .Code = "§5"},
        New ColorCode() With {.Name = "Gold", .Color = DirectCast(ColorConverter.ConvertFromString("#FFAA00"), Color), .Code = "§6"},
        New ColorCode() With {.Name = "Gray", .Color = DirectCast(ColorConverter.ConvertFromString("#AAAAAA"), Color), .Code = "§7"},
        New ColorCode() With {.Name = "Dark Gray", .Color = DirectCast(ColorConverter.ConvertFromString("#555555"), Color), .Code = "§8"},
        New ColorCode() With {.Name = "Blue", .Color = DirectCast(ColorConverter.ConvertFromString("#5555FF"), Color), .Code = "§9"},
        New ColorCode() With {.Name = "Green", .Color = DirectCast(ColorConverter.ConvertFromString("#55FF55"), Color), .Code = "§a"},
        New ColorCode() With {.Name = "Aqua", .Color = DirectCast(ColorConverter.ConvertFromString("#55FFFF"), Color), .Code = "§b"},
        New ColorCode() With {.Name = "Red", .Color = DirectCast(ColorConverter.ConvertFromString("#FF5555"), Color), .Code = "§c"},
        New ColorCode() With {.Name = "Light Purple", .Color = DirectCast(ColorConverter.ConvertFromString("#FF55FF"), Color), .Code = "§d"},
        New ColorCode() With {.Name = "Yellow", .Color = DirectCast(ColorConverter.ConvertFromString("#FFFF55"), Color), .Code = "§e"},
        New ColorCode() With {.Name = "White", .Color = DirectCast(ColorConverter.ConvertFromString("#FFFFFF"), Color), .Code = "§f"}
    }
    Public Shared Formattingcodes As IList(Of FormattingCode) = New List(Of FormattingCode) From
{
    New FormattingCode() With {.Name = "Obfuscated", .Code = "§k"},
    New FormattingCode() With {.Name = "Bold", .Code = "§l"},
    New FormattingCode() With {.Name = "Strikethrough", .Code = "§m"},
    New FormattingCode() With {.Name = "Underline", .Code = "§n"},
    New FormattingCode() With {.Name = "Italic", .Code = "§o"},
    New FormattingCode() With {.Name = "Reset", .Code = "§r"}
}
    Public Shared MotD_new_line As String = "\n"

    Public Class FormattedText
        Private m_text As String,
            m_color As Color,
            m_Obfuscated As Boolean,
            m_Bold As Boolean,
            m_Strikethrough As Boolean,
            m_Underline As Boolean,
            m_Italic As Boolean

        Public Property Text As String
            Get
                Return m_text
            End Get
            Set(value As String)
                m_text = value
            End Set
        End Property

        Public Property Color As Color
            Get
                Return m_color
            End Get
            Set(value As Color)
                m_color = value
            End Set
        End Property
        Public Property Obfuscated As Boolean
            Get
                Return m_Obfuscated
            End Get
            Set(value As Boolean)
                m_Obfuscated = value
            End Set
        End Property
        Public Property Bold As Boolean
            Get
                Return m_Bold
            End Get
            Set(value As Boolean)
                m_Bold = value
            End Set
        End Property
        Public Property Strikethrough As Boolean
            Get
                Return m_Strikethrough
            End Get
            Set(value As Boolean)
                m_Strikethrough = value
            End Set
        End Property
        Public Property Underline As Boolean
            Get
                Return m_Underline
            End Get
            Set(value As Boolean)
                m_Underline = value
            End Set
        End Property
        Public Property Italic As Boolean
            Get
                Return m_Italic
            End Get
            Set(value As Boolean)
                m_Italic = value
            End Set
        End Property

        Public Sub New()

        End Sub

    End Class
    Public Class ColorCode
        Private m_name As String,
            m_color As Color,
            m_Code As String

        Public Property Name As String
            Get
                Return m_name
            End Get
            Set(value As String)
                m_name = value
            End Set
        End Property

        Public Property Color As Color
            Get
                Return m_color
            End Get
            Set(value As Color)
                m_color = value
            End Set
        End Property
        Public Property Code As String
            Get
                Return m_Code
            End Get
            Set(value As String)
                m_Code = value
            End Set
        End Property

        Public Sub New()

        End Sub

    End Class
    Public Class FormattingCode
        Private m_name As String,
            m_Code As String

        Public Property Name As String
            Get
                Return m_name
            End Get
            Set(value As String)
                m_name = value
            End Set
        End Property

        Public Property Code As String
            Get
                Return m_Code
            End Get
            Set(value As String)
                m_Code = value
            End Set
        End Property

        Public Sub New()

        End Sub

    End Class
End Class