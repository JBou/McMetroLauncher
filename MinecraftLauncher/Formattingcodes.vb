' Copyright 2009, 2010, 2011 Matvei Stefarov <me@matvei.org>
Imports System.Collections.Generic
Imports System.Text


Public Class FormattingCodes
    Public Shared Colorcodes As String() = {"§0", "§1", "§2", "§3", "§4", "§5", "§6", "§7", "§8", "§9", "§a", "§b", "§c", "§d", "§e", "§f"}
    Public Shared Formattingcodes As String() = {"§k", "§l", "§m", "§n", "§n", "§o", "§r"}
    Public Shared MotD_new_line As String = "\n"
End Class

''' <summary>
''' Static class with definitions of Minecraft color codes, parsers/converters, and utilities.
''' </summary>
''' 
Public NotInheritable Class ColorCodes
    Private Sub New()
    End Sub
    Public Const Black As String = "&0",
        Navy As String = "&1",
        Green As String = "&2",
        Teal As String = "&3",
        Maroon As String = "&4",
        Purple As String = "&5",
        Olive As String = "&6",
        Silver As String = "&7",
        Gray As String = "&8",
        Blue As String = "&9",
        Lime As String = "&a",
        Aqua As String = "&b",
        Red As String = "&c",
        Magenta As String = "&d",
        Yellow As String = "&e",
        White As String = "&f"

    ' User-defined color assignments. Set by Config.ApplyConfig.
    Public Shared Sys As String, Help As String, Say As String, Announcement As String, PM As String, IRC As String, _
        [Me] As String, Warning As String

    ' Defaults for user-defined colors.
    Public Const SysDefault As String = Yellow, HelpDefault As String = Lime, SayDefault As String = Green, AnnouncementDefault As String = Green, PMDefault As String = Aqua, IRCDefault As String = Purple, _
        MeDefault As String = Purple, WarningDefault As String = Red

    Public Shared ReadOnly ColorNames As New SortedList(Of Char, String)() From { _
        {"0"c, "black"},
        {"1"c, "navy"},
        {"2"c, "green"},
        {"3"c, "teal"},
        {"4"c, "maroon"},
        {"5"c, "purple"},
        {"6"c, "olive"},
        {"7"c, "silver"},
        {"8"c, "gray"},
        {"9"c, "blue"},
        {"a"c, "lime"},
        {"b"c, "aqua"},
        {"c"c, "red"},
        {"d"c, "magenta"},
        {"e"c, "yellow"},
        {"f"c, "white"}
    }


    ''' <summary> Gets color name for hex color code. </summary>
    ''' <param name="code">Hexadecimal color code (between '0' and 'f')</param>
    ''' <returns>Lowercase color name</returns>
    Public Shared Function GetName(code As Char) As String
        code = [Char].ToLower(code)
        If IsValidColorCode(code) Then
            Return ColorNames(code)
        End If
        Dim color As String = Parse(code)
        If color Is Nothing Then
            Return Nothing
        End If
        Return ColorNames(color(1))
    End Function


    ''' <summary> Gets color name for a numeric color code. </summary>
    ''' <param name="index"> Ordinal numeric color code (between 0 and 15) </param>
    ''' <returns> Lowercase color name. If input is out of range, returns null. </returns>
    Public Shared Function GetName(index As Integer) As String
        If index >= 0 AndAlso index <= 15 Then
            Return ColorNames.Values(index)
        Else
            Return Nothing
        End If
    End Function


    ''' <summary> Gets color name for a string representation of a color. </summary>
    ''' <param name="color"> Any parsable string representation of a color. </param>
    ''' <returns> Lowercase color name.
    ''' If input is an empty string, returns empty string.
    ''' If input is null or cannot be parsed, returns null. </returns>
    Public Shared Function GetName(color As String) As String
        If color Is Nothing Then
            Return Nothing
        ElseIf color.Length = 0 Then
            Return ""
        Else
            Dim parsedColor As String = Parse(color)
            If parsedColor Is Nothing Then
                Return Nothing
            Else
                Return GetName(parsedColor(1))
            End If
        End If
    End Function



    ''' <summary> Parses a string to a format readable by Minecraft clients. 
    ''' an accept color names and color codes (with or without the ampersand). </summary>
    ''' <param name="code"> Color code character </param>
    ''' <returns> Two-character color string, readable by Minecraft client.
    ''' If input is null or cannot be parsed, returns null. </returns>
    Public Shared Function Parse(code As Char) As String
        code = Char.ToLower(code)
        If IsValidColorCode(code) Then
            Return "&" & code
        Else
            Select Case code
                Case "s"c
                    Return Sys
                Case "y"c
                    Return Say
                Case "p"c
                    Return PM
                Case "r"c
                    Return Announcement
                Case "h"c
                    Return Help
                Case "w"c
                    Return Warning
                Case "m"c
                    Return [Me]
                Case "i"c
                    Return IRC
                Case Else
                    Return Nothing
            End Select
        End If
    End Function


    ''' <summary> Parses a numeric color code to a string readable by Minecraft clients </summary>
    ''' <param name="index"> Ordinal numeric color code (between 0 and 15) </param>
    ''' <returns> Two-character color string, readable by Minecraft client.
    ''' If input cannot be parsed, returns null. </returns>
    Public Shared Function Parse(index As Integer) As String
        If index >= 0 AndAlso index <= 15 Then
            Return "&" & ColorNames.Keys(index)
        Else
            Return Nothing
        End If
    End Function


    ''' <summary> Parses a string to a format readable by Minecraft clients. 
    ''' an accept color names and color codes (with or without the ampersand). </summary>
    ''' <param name="color"> Ordinal numeric color code (between 0 and 15) </param>
    ''' <returns> Two-character color string, readable by Minecraft client.
    ''' If input is an empty string, returns empty string.
    ''' If input is null or cannot be parsed, returns null. </returns>
    Public Shared Function Parse(color As String) As String
        If color Is Nothing Then
            Return Nothing
        End If
        color = color.ToLower()
        Select Case color.Length
            Case 2
                If color(0) = "&"c AndAlso IsValidColorCode(color(1)) Then
                    Return color
                End If
                Exit Select

            Case 1
                Return Parse(color(0))

            Case 0
                Return ""
        End Select
        If ColorNames.ContainsValue(color) Then
            Return "&" & ColorNames.Keys(ColorNames.IndexOfValue(color))
        Else
            Return Nothing
        End If
    End Function


    Public Shared Function ParseToIndex(color As String) As Integer
        color = color.ToLower()
        If color.Length = 2 AndAlso color(0) = "&"c Then
            If ColorNames.ContainsKey(color(1)) Then
                Return ColorNames.IndexOfKey(color(1))
            Else
                Select Case color
                    Case "&s"
                        Return ColorNames.IndexOfKey(Sys(1))
                    Case "&y"
                        Return ColorNames.IndexOfKey(Say(1))
                    Case "&p"
                        Return ColorNames.IndexOfKey(PM(1))
                    Case "&r"
                        Return ColorNames.IndexOfKey(Announcement(1))
                    Case "&h"
                        Return ColorNames.IndexOfKey(Help(1))
                    Case "&w"
                        Return ColorNames.IndexOfKey(Warning(1))
                    Case "&m"
                        Return ColorNames.IndexOfKey([Me](1))
                    Case "&i"
                        Return ColorNames.IndexOfKey(IRC(1))
                    Case Else
                        Return 15
                End Select
            End If
        ElseIf ColorNames.ContainsValue(color) Then
            Return ColorNames.IndexOfValue(color)
        Else
            ' white
            Return 15
        End If
    End Function



    ''' <summary>
    ''' Checks whether a color code is valid (checks if it's hexadecimal char).
    ''' </summary>
    ''' <returns>True is char is valid, otherwise false</returns>
    Public Shared Function IsValidColorCode(code As Char) As Boolean
        Return (code >= "0"c AndAlso code <= "9"c) OrElse (code >= "a"c AndAlso code <= "f"c) OrElse (code >= "A"c AndAlso code <= "F"c)
    End Function


    Public Shared Function ReplacePercentCodes(message As String) As String
        Dim sb As New StringBuilder(message)
        sb.Replace("%0", "&0")
        sb.Replace("%1", "&1")
        sb.Replace("%2", "&2")
        sb.Replace("%3", "&3")
        sb.Replace("%4", "&4")
        sb.Replace("%5", "&5")
        sb.Replace("%6", "&6")
        sb.Replace("%7", "&7")
        sb.Replace("%8", "&8")
        sb.Replace("%9", "&9")
        sb.Replace("%a", "&a")
        sb.Replace("%b", "&b")
        sb.Replace("%c", "&c")
        sb.Replace("%d", "&d")
        sb.Replace("%e", "&e")
        sb.Replace("%f", "&f")
        sb.Replace("%A", "&a")
        sb.Replace("%B", "&b")
        sb.Replace("%C", "&c")
        sb.Replace("%D", "&d")
        sb.Replace("%E", "&e")
        sb.Replace("%F", "&f")
        Return sb.ToString()
    End Function


    'Public Shared Function SubstituteSpecialColors(input As String) As String
    '    Dim sb As New StringBuilder(input)
    '    For i As Integer = sb.Length - 1 To 1 Step -1
    '        If sb(i - 1) = "&"c Then
    '            Select Case [Char].ToLower(sb(i))
    '                Case "s"c
    '                    sb(i) = Sys(1)
    '                    Exit Select
    '                Case "y"c
    '                    sb(i) = Say(1)
    '                    Exit Select
    '                Case "p"c
    '                    sb(i) = PM(1)
    '                    Exit Select
    '                Case "r"c
    '                    sb(i) = Announcement(1)
    '                    Exit Select
    '                Case "h"c
    '                    sb(i) = Help(1)
    '                    Exit Select
    '                Case "w"c
    '                    sb(i) = Warning(1)
    '                    Exit Select
    '                Case "m"c
    '                    sb(i) = [Me](1)
    '                    Exit Select
    '                Case "i"c
    '                    sb(i) = IRC(1)
    '                    Exit Select
    '                Case Else
    '                    If IsValidColorCode(sb(i)) Then
    '				Continue Select
    '                    Else
    '                        sb.Remove(i - 1, 1)
    '                    End If
    '                    Exit Select
    '            End Select
    '        End If
    '    Next
    '    Return sb.ToString()
    'End Function

#Region "IRC Colors"

    Public Const IRCReset As String = ChrW(3) & ChrW(15)
    Public Const IRCBold As String = ChrW(2)

    Shared ReadOnly MinecraftToIRCColors As New Dictionary(Of String, IRCColor)() From { _
        {White, IRCColor.White}, _
        {Black, IRCColor.Black}, _
        {Navy, IRCColor.Navy}, _
        {Green, IRCColor.Green}, _
        {Red, IRCColor.Red}, _
        {Maroon, IRCColor.Maroon}, _
        {Purple, IRCColor.Purple}, _
        {Olive, IRCColor.Olive}, _
        {Yellow, IRCColor.Yellow}, _
        {Lime, IRCColor.Lime}, _
        {Teal, IRCColor.Teal}, _
        {Aqua, IRCColor.Aqua}, _
        {Blue, IRCColor.Blue}, _
        {Magenta, IRCColor.Magenta}, _
        {Gray, IRCColor.Gray}, _
        {Silver, IRCColor.Silver} _
    }


    Public Shared Function EscapeAmpersands(input As String) As String
        Return input.Replace("&", "&&")
    End Function


    'Public Shared Function ToIRCColorCodes(input As String) As String
    '    Dim sb As New StringBuilder(SubstituteSpecialColors(input))

    '    For Each code As KeyValuePair(Of String, IRCColor) In MinecraftToIRCColors
    '        sb.Replace(code.Key, ChrW(3) & CInt(code.Value).ToString().PadLeft(2, "0"c))
    '    Next
    '    Return sb.ToString()
    'End Function

#End Region
End Class


Enum IRCColor
    White = 0
    Black
    Navy
    Green
    Red
    Maroon
    Purple
    Olive
    Yellow
    Lime
    Teal
    Aqua
    Blue
    Magenta
    Gray
    Silver
End Enum