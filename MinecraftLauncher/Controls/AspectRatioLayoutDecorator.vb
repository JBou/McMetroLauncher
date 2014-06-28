Imports System.Windows
Namespace JBou.Controls
    Public Class AspectRatioLayoutDecorator
        Inherits Decorator
        Public Shared ReadOnly AspectRatioProperty As DependencyProperty = DependencyProperty.Register("AspectRatio",
                                                                                                       GetType(Double),
                                                                                                       GetType(AspectRatioLayoutDecorator),
                                                                                                       New FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsMeasure),
                                                                                                       New ValidateValueCallback(AddressOf ValidateAspectRatio))

        Private Shared Function ValidateAspectRatio(value As Object) As Boolean
            If Not (TypeOf value Is Double) Then
                Return False
            End If

            Dim aspectRatio = CDbl(value)
            Return aspectRatio > 0 AndAlso Not Double.IsInfinity(aspectRatio) AndAlso Not Double.IsNaN(aspectRatio)
        End Function

        Public Property AspectRatio() As Double
            Get
                Return CDbl(GetValue(AspectRatioProperty))
            End Get
            Set(value As Double)
                SetValue(AspectRatioProperty, value)
            End Set
        End Property

        Protected Overrides Function MeasureOverride(constraint As Size) As Size
            If Child IsNot Nothing Then
                constraint = SizeToRatio(constraint, False)
                Child.Measure(constraint)

                If Double.IsInfinity(constraint.Width) OrElse Double.IsInfinity(constraint.Height) Then
                    Return SizeToRatio(Child.DesiredSize, True)
                End If

                Return constraint
            End If

            ' we don't have a child, so we don't need any space
            Return New Size(0, 0)
        End Function

        Public Function SizeToRatio(size As Size, expand As Boolean) As Size
            Dim ratio As Double = AspectRatio

            Dim height As Double = size.Width / ratio
            Dim width As Double = size.Height * ratio

            If expand Then
                width = Math.Max(width, size.Width)
                height = Math.Max(height, size.Height)
            Else
                width = Math.Min(width, size.Width)
                height = Math.Min(height, size.Height)
            End If

            Return New Size(width, height)
        End Function

        Protected Overrides Function ArrangeOverride(arrangeSize As Size) As Size
            If Child IsNot Nothing Then
                Dim newSize = SizeToRatio(arrangeSize, False)

                Dim widthDelta As Double = arrangeSize.Width - newSize.Width
                Dim heightDelta As Double = arrangeSize.Height - newSize.Height

                Dim top As Double = 0
                Dim left As Double = 0

                If Not Double.IsNaN(widthDelta) AndAlso Not Double.IsInfinity(widthDelta) Then
                    left = widthDelta / 2
                End If

                If Not Double.IsNaN(heightDelta) AndAlso Not Double.IsInfinity(heightDelta) Then
                    top = heightDelta / 2
                End If

                Dim finalRect = New Rect(New Point(left, top), newSize)
                Child.Arrange(finalRect)
            End If

            Return arrangeSize
        End Function
    End Class
End Namespace