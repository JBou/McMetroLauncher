Public Class ValueConverterGroup
    Inherits List(Of IValueConverter)
    Implements IValueConverter
#Region "IValueConverter Members"

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Return Me.Aggregate(value, Function(current, converter) converter.Convert(current, targetType, parameter, culture))
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function

#End Region
End Class
