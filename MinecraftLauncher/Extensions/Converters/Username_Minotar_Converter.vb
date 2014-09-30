Imports System.Text.RegularExpressions

Public Class Username_Minotar_Converter
    Implements System.Windows.Data.IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim username As String = TryCast(value, String)
        Dim resolution As String = TryCast(parameter, String)
        Return String.Format("https://minotar.net/avatar/{0}/{1}", username, resolution)
        Return Nothing
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function

End Class