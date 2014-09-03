Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.ComponentModel

Namespace JBou.Controls
    Public Class ArrangePanel
        Inherits Panel
        Private _draggingObject As UIElement
        Private _delta As Vector
        Private _startPosition As Point
        Private ReadOnly _strategy As ILayoutStrategy = New TableLayoutStrategy()

        Protected Overrides Sub OnPreviewMouseLeftButtonDown(e As MouseButtonEventArgs)
            StartReordering(e)
        End Sub

        Protected Overrides Sub OnPreviewMouseMove(e As MouseEventArgs)
            If _draggingObject IsNot Nothing Then
                If e.LeftButton = MouseButtonState.Released Then
                    StopReordering()
                Else
                    DoReordering(e)
                End If
            End If
        End Sub

        Protected Overrides Sub OnMouseLeave(e As MouseEventArgs)
            'Doesn't work really good
            StopReordering()
            MyBase.OnMouseLeave(e)
        End Sub

        Protected Overrides Sub OnMouseLeftButtonUp(e As MouseButtonEventArgs)
            StopReordering()
        End Sub

        Private Sub StartReordering(e As MouseEventArgs)
            _startPosition = e.GetPosition(Me)
            _draggingObject = GetMyChildOfUiElement(DirectCast(e.OriginalSource, UIElement))
            _draggingObject.SetValue(ZIndexProperty, 100)
            Dim position = GetPosition(_draggingObject)
            _delta = position.TopLeft - _startPosition
            _draggingObject.BeginAnimation(PositionProperty, Nothing)
            SetPosition(_draggingObject, position)
        End Sub

        Private Sub DoReordering(e As MouseEventArgs)
            e.Handled = True
            Dim mousePosition As Point = e.GetPosition(Me)
            Dim index = _strategy.GetIndex(mousePosition)
            SetOrder(_draggingObject, index)
            Dim topLeft = mousePosition + _delta
            Dim newPosition = New Rect(topLeft, GetPosition(_draggingObject).Size)
            If ColumnCount = 1 AndAlso OnlyVerticalAnimation = True Then
                Dim pos As Point = topLeft
                pos.X = GetPosition(_draggingObject).X
                newPosition = New Rect(pos, GetPosition(_draggingObject).Size)
            End If
            SetPosition(_draggingObject, newPosition)
        End Sub

        Private Sub StopReordering()
            If _draggingObject Is Nothing Then
                Return
            End If

            _draggingObject.ClearValue(ZIndexProperty)
            InvalidateMeasure()
            AnimateToPosition(_draggingObject, GetDesiredPosition(_draggingObject))
            _draggingObject = Nothing
        End Sub

        Private Function GetMyChildOfUiElement(e As UIElement) As UIElement
            Dim obj = e
            Dim parent = DirectCast(VisualTreeHelper.GetParent(obj), UIElement)
            While parent IsNot Nothing AndAlso parent IsNot Me
                obj = parent
                parent = DirectCast(VisualTreeHelper.GetParent(obj), UIElement)
            End While
            Return obj
        End Function

        Protected Overrides Function MeasureOverride(availableSize As Size) As Size
            InitializeEmptyOrder()
            If _draggingObject IsNot Nothing Then
                ReorderOthers()
            End If

            Dim measures = MeasureChildren()


            If ColumnCount = Integer.MaxValue Then
                _strategy.Calculate(availableSize, measures)
            Else
                _strategy.Calculate(availableSize, measures, CInt(GetValue(ColumnCountProperty)))
            End If

            Dim index = -1
            For Each child In Children.OfType(Of UIElement)().OrderBy(AddressOf GetOrder)
                index += 1
                If child Is _draggingObject Then
                    Continue For
                End If
                SetDesiredPosition(child, _strategy.GetPosition(index))
            Next
            Dim resultsize As Size = _strategy.ResultSize
            If Double.IsNaN(_strategy.ResultSize.Width) OrElse Double.IsInfinity(_strategy.ResultSize.Width) Then
                resultsize.Width = Me.RenderSize.Width
            End If
            Return resultsize
        End Function

        Protected Overrides Function ArrangeOverride(finalSize As Size) As Size
            For Each child In Children.OfType(Of UIElement)().OrderBy(AddressOf GetOrder)
                'If stretch then width = acailablesize ??
                Dim position = GetPosition(child)
                If Double.IsNaN(position.Top) Then
                    position = GetDesiredPosition(child)
                End If
                If Double.IsNaN(_strategy.ResultSize.Width) OrElse Double.IsInfinity(_strategy.ResultSize.Width) Then
                    position.Width = finalSize.Width
                End If
                child.Arrange(position)
            Next
            Return _strategy.ResultSize
        End Function


        Private Function MeasureChildren() As Size()
            If _measures Is Nothing OrElse Children.Count <> _measures.Length Then
                _measures = New Size(Children.Count - 1) {}

                Dim infinitSize = New Size(Double.PositiveInfinity, Double.PositiveInfinity)

                For Each child As UIElement In Children
                    child.Measure(infinitSize)
                Next


                Dim i = 0
                For Each _Measure In Children.OfType(Of UIElement)().OrderBy(AddressOf GetOrder).[Select](Function(ch) ch.DesiredSize)
                    _measures(i) = _Measure
                    i += 1
                Next
            End If
            Return _measures
        End Function

        Private Sub ReorderOthers()
            Dim s = GetOrder(_draggingObject)
            Dim i = 0
            For Each child In Children.OfType(Of UIElement)().OrderBy(AddressOf GetOrder)
                If i = s Then
                    i += 1
                End If
                If child Is _draggingObject Then
                    Continue For
                End If
                Dim current = GetOrder(child)
                If i <> current Then
                    SetOrder(child, i)
                End If
                i += 1
            Next
        End Sub

        Private Sub InitializeEmptyOrder()
            If Children.Count > 0 Then
                Dim [next] = Children.OfType(Of UIElement)().Max(Function(ch) GetOrder(ch)) + 1
                For Each child In Children.OfType(Of UIElement)().Where(Function(_child) GetOrder(_child) = -1)
                    SetOrder(child, [next])
                    [next] += 1
                Next
            End If
        End Sub


        Public Shared ReadOnly OrderProperty As DependencyProperty
        Public Shared ReadOnly PositionProperty As DependencyProperty
        Public Shared ReadOnly DesiredPositionProperty As DependencyProperty
        Public Shared ColumnCountProperty As DependencyProperty
        Public Shared OnlyVerticalAnimationProperty As DependencyProperty
        Private _measures As Size()

        Shared Sub New()
            PositionProperty = DependencyProperty.RegisterAttached("Position", GetType(Rect), GetType(ArrangePanel), New FrameworkPropertyMetadata(New Rect(Double.NaN, Double.NaN, Double.NaN, Double.NaN), FrameworkPropertyMetadataOptions.AffectsParentArrange))
            DesiredPositionProperty = DependencyProperty.RegisterAttached("DesiredPosition", GetType(Rect), GetType(ArrangePanel), New FrameworkPropertyMetadata(New Rect(Double.NaN, Double.NaN, Double.NaN, Double.NaN), AddressOf OnDesiredPositionChanged))
            OrderProperty = DependencyProperty.RegisterAttached("Order", GetType(Integer), GetType(ArrangePanel), New FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsParentMeasure))
            ColumnCountProperty = DependencyProperty.Register("ColumnCount", GetType(Integer), GetType(ArrangePanel), New FrameworkPropertyMetadata(Integer.MaxValue))
            OnlyVerticalAnimationProperty = DependencyProperty.Register("OnlyVerticalAnimation", GetType(Boolean), GetType(ArrangePanel), New FrameworkPropertyMetadata(False))
        End Sub

        Public Property ColumnCount As Integer
            Get
                Return DirectCast(GetValue(ColumnCountProperty), Integer)
            End Get
            Set(value As Integer)
                SetValue(ColumnCountProperty, value)
            End Set
        End Property

        Public Property OnlyVerticalAnimation As Boolean
            Get
                Return DirectCast(GetValue(OnlyVerticalAnimationProperty), Boolean)
            End Get
            Set(value As Boolean)
                SetValue(OnlyVerticalAnimationProperty, value)
            End Set
        End Property

        Protected Overrides Sub OnVisualChildrenChanged(visualAdded As DependencyObject, visualRemoved As DependencyObject)
            Dim startdouble As Double = 0
            Dim enddouble As Double = 1
            If visualAdded IsNot Nothing Then
                'Add Animation
                startdouble = 0
                enddouble = 1
            End If
            If visualRemoved IsNot Nothing Then
                'Remove Animation
                startdouble = 1
                enddouble = 0
            End If
            Dim control As FrameworkElement = DirectCast(visualAdded, FrameworkElement)
            control.BeginAnimation(OpacityProperty, New DoubleAnimation(startdouble, enddouble, New Duration(TimeSpan.FromMilliseconds(200))))

            Dim tg As New TransformGroup()
            tg.Children.Add(New ScaleTransform(0, 0))
            control.RenderTransform = tg
            control.RenderTransformOrigin = New Point(0.5, 0.5)
            Dim sct As ScaleTransform = DirectCast(DirectCast(control.RenderTransform, TransformGroup).Children(0), ScaleTransform)
            Dim scaleanimation As New DoubleAnimation(startdouble, enddouble, New Duration(TimeSpan.FromMilliseconds(200)))
            sct.BeginAnimation(ScaleTransform.ScaleXProperty, scaleanimation)
            sct.BeginAnimation(ScaleTransform.ScaleYProperty, scaleanimation)
        End Sub

        Private Shared Sub OnDesiredPositionChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim desiredPosition = DirectCast(e.NewValue, Rect)
            AnimateToPosition(d, desiredPosition)
        End Sub

        Private Shared Sub AnimateToPosition(d As DependencyObject, desiredPosition As Rect)
            Dim position = GetPosition(d)
            If Double.IsNaN(position.X) Then
                SetPosition(d, desiredPosition)
                Return
            End If
            Dim pos1 As Double = (desiredPosition.TopLeft - position.TopLeft).Length
            Dim pos2 As Double = (desiredPosition.BottomRight - position.BottomRight).Length
            Dim distance = Math.Max(If(Double.IsNaN(pos1), 0, pos1), If(Double.IsNaN(pos2), 0, pos2))

            Dim animationTime = TimeSpan.FromMilliseconds(distance * 5)
            Dim animation = New RectAnimation(position, desiredPosition, New Duration(animationTime))
            animation.DecelerationRatio = 1
            DirectCast(d, UIElement).BeginAnimation(PositionProperty, animation)
        End Sub

        Public Shared Function GetOrder(obj As DependencyObject) As Integer
            Return CInt(obj.GetValue(OrderProperty))
        End Function

        Public Shared Sub SetOrder(obj As DependencyObject, value As Integer)
            obj.SetValue(OrderProperty, value)
        End Sub

        Public Shared Function GetPosition(obj As DependencyObject) As Rect
            Return DirectCast(obj.GetValue(PositionProperty), Rect)
        End Function

        Public Shared Sub SetPosition(obj As DependencyObject, value As Rect)
            obj.SetValue(PositionProperty, value)
        End Sub

        Public Shared Function GetDesiredPosition(obj As DependencyObject) As Rect
            Return DirectCast(obj.GetValue(DesiredPositionProperty), Rect)
        End Function

        Public Shared Sub SetDesiredPosition(obj As DependencyObject, value As Rect)
            obj.SetValue(DesiredPositionProperty, value)
        End Sub

        Public Enum EasingFunctions
            Normal
            BackEase
            BounceEase
            CircleEase
            CubicEase
            ElasticEase
            ExponentialEase
            PowerEase
            QuadraticEase
            QuarticEase
            QuinticEase
            SineEase
        End Enum

        Public Shared Function GetEasingFunction(Name As String) As EasingFunctionBase
            Select Case Name
                Case "Normal"
                    Return Nothing
                Case "BackEase"
                    Return New BackEase
                Case "BounceEase"
                    Return New BounceEase
                Case "CircleEase"
                    Return New CircleEase
                Case "CubicEase"
                    Return New CubicEase
                Case "ElasticEase"
                    Return New ElasticEase
                Case "ExponentialEase"
                    Return New ExponentialEase
                Case "PowerEase"
                    Return New PowerEase
                Case "QuadraticEase"
                    Return New QuadraticEase
                Case "QuarticEase"
                    Return New QuarticEase
                Case "QuinticEase"
                    Return New QuinticEase
                Case "SineEase"
                    Return New SineEase
                Case Else
                    Return Nothing
            End Select
        End Function
    End Class
End Namespace