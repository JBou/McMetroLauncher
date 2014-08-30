Imports System.ComponentModel
Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Runtime.CompilerServices
<Serializable> _
Public Class PropertyChangedBase
    Implements INotifyPropertyChanged
    <NonSerialized> _
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Function SetProperty(Of T)(value As T, ByRef field As T, [property] As Expression(Of Func(Of Object))) As Boolean
        Return SetProperty(value, field, GetPropertyName([property]))
    End Function
    Public Function SetProperty(Of T)(value As T, ByRef field As T, <CallerMemberName> Optional propertyName As String = Nothing) As Boolean
        If field Is Nothing OrElse Not field.Equals(value) Then
            field = value
            OnPropertyChanged(propertyName)
            Return True
        End If
        Return False
    End Function
    Public Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
    Public Sub OnPropertyChanged([property] As Expression(Of Func(Of Object)))
        OnPropertyChanged(GetPropertyName([property]))
    End Sub
    Public Function GetPropertyName([property] As Expression(Of Func(Of Object))) As String
        Dim lambda = TryCast([property], LambdaExpression)
        Dim memberExpression As MemberExpression
        If TypeOf lambda.Body Is UnaryExpression Then
            Dim unaryExpression = TryCast(lambda.Body, UnaryExpression)
            memberExpression = TryCast(unaryExpression.Operand, MemberExpression)
        Else
            memberExpression = TryCast(lambda.Body, MemberExpression)
        End If
        If memberExpression IsNot Nothing Then
            Dim constantExpression = TryCast(memberExpression.Expression, ConstantExpression)
            Dim propertyInfo = TryCast(memberExpression.Member, PropertyInfo)
            If propertyInfo IsNot Nothing Then
                Return propertyInfo.Name
            End If
        End If
        Return [String].Empty
    End Function
End Class