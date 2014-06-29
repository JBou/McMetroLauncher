#Region "File Header"

' -------------------------------------------------------------------------------
' 
' This file is part of the WPFSpark project: http://wpfspark.codeplex.com/
'
' Author: Ratish Philip
' 
' WPFSpark v1.1
'
' -------------------------------------------------------------------------------

#End Region

Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media.Animation

Namespace JBou.Controls
    ''' <summary>
    ''' Interactions for the FluidWrapPanel
    ''' </summary>
    Public Class FluidWrapPanel
        Inherits Panel
#Region "Constants"

        Private Const NORMAL_SCALE As Double = 1.0
        Private Const DRAG_SCALE_DEFAULT As Double = 1.3
        Private Const NORMAL_OPACITY As Double = 1.0
        Private Const DRAG_OPACITY_DEFAULT As Double = 0.6
        Private Const OPACITY_MIN As Double = 0.1
        Private Const Z_INDEX_NORMAL As Int32 = 0
        Private Const Z_INDEX_INTERMEDIATE As Int32 = 1
        Private Const Z_INDEX_DRAG As Int32 = 10
        Private Shared DEFAULT_ANIMATION_TIME_WITHOUT_EASING As TimeSpan = TimeSpan.FromMilliseconds(200)
        Private Shared DEFAULT_ANIMATION_TIME_WITH_EASING As TimeSpan = TimeSpan.FromMilliseconds(400)
        Private Shared FIRST_TIME_ANIMATION_DURATION As TimeSpan = TimeSpan.FromMilliseconds(320)

#End Region

#Region "Fields"

        Private dragStartPoint As New Point()
        Private oldIndex As Integer
        Private dragElement As UIElement = Nothing
        Private lastDragElement As UIElement = Nothing
        Private fluidElements As List(Of UIElement) = Nothing
        Private layoutManager As FluidLayoutManager = Nothing
        Private isInitializeArrangeRequired As Boolean = False
        Private _parentlistbox As ListBox
        Private updatedata As Boolean = True

#End Region

#Region "Dependency Properties"

#Region "DragEasing"

        ''' <summary>
        ''' DragEasing Dependency Property
        ''' </summary>
        Public Shared ReadOnly DragEasingProperty As DependencyProperty = DependencyProperty.Register("DragEasing", GetType(EasingFunctionBase), GetType(FluidWrapPanel), New FrameworkPropertyMetadata((New PropertyChangedCallback(AddressOf OnDragEasingChanged))))

        ''' <summary>
        ''' Gets or sets the DragEasing property. This dependency property 
        ''' indicates the Easing function to be used when the user stops dragging the child and releases it.
        ''' </summary>
        Public Property DragEasing() As EasingFunctionBase
            Get
                Return DirectCast(GetValue(DragEasingProperty), EasingFunctionBase)
            End Get
            Set(value As EasingFunctionBase)
                SetValue(DragEasingProperty, Value)
            End Set
        End Property

        ''' <summary>
        ''' Handles changes to the DragEasing property.
        ''' </summary>
        ''' <param name="d">FluidWrapPanel</param>
        ''' <param name="e">DependencyProperty changed event arguments</param>
        Private Shared Sub OnDragEasingChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim panel As FluidWrapPanel = DirectCast(d, FluidWrapPanel)
            Dim oldDragEasing As EasingFunctionBase = DirectCast(e.OldValue, EasingFunctionBase)
            Dim newDragEasing As EasingFunctionBase = panel.DragEasing
            panel.OnDragEasingChanged(oldDragEasing, newDragEasing)
        End Sub

        ''' <summary>
        ''' Provides derived classes an opportunity to handle changes to the DragEasing property.
        ''' </summary>
        ''' <param name="oldDragEasing">Old Value</param>
        ''' <param name="newDragEasing">New Value</param>
        Protected Overridable Sub OnDragEasingChanged(oldDragEasing As EasingFunctionBase, newDragEasing As EasingFunctionBase)

        End Sub

#End Region

#Region "DragOpacity"

        ''' <summary>
        ''' DragOpacity Dependency Property
        ''' </summary>
        Public Shared ReadOnly DragOpacityProperty As DependencyProperty = DependencyProperty.Register("DragOpacity", GetType(Double), GetType(FluidWrapPanel), New FrameworkPropertyMetadata(DRAG_OPACITY_DEFAULT, New PropertyChangedCallback(AddressOf OnDragOpacityChanged), New CoerceValueCallback(AddressOf CoerceDragOpacity)))

        ''' <summary>
        ''' Gets or sets the DragOpacity property. This dependency property 
        ''' indicates the opacity of the child being dragged.
        ''' </summary>
        Public Property DragOpacity() As Double
            Get
                Return CDbl(GetValue(DragOpacityProperty))
            End Get
            Set(value As Double)
                SetValue(DragOpacityProperty, Value)
            End Set
        End Property


        ''' <summary>
        ''' Coerces the FluidDrag Opacity to an acceptable value
        ''' </summary>
        ''' <param name="d">Dependency Object</param>
        ''' <param name="value">Value</param>
        ''' <returns>Coerced Value</returns>
        Private Shared Function CoerceDragOpacity(d As DependencyObject, value As Object) As Object
            Dim opacity As Double = CDbl(value)

            If opacity < OPACITY_MIN Then
                opacity = OPACITY_MIN
            ElseIf opacity > NORMAL_OPACITY Then
                opacity = NORMAL_OPACITY
            End If

            Return opacity
        End Function

        ''' <summary>
        ''' Handles changes to the DragOpacity property.
        ''' </summary>
        ''' <param name="d">FluidWrapPanel</param>
        ''' <param name="e">DependencyProperty changed event arguments</param>
        Private Shared Sub OnDragOpacityChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim panel As FluidWrapPanel = DirectCast(d, FluidWrapPanel)
            Dim oldDragOpacity As Double = CDbl(e.OldValue)
            Dim newDragOpacity As Double = panel.DragOpacity
            panel.OnDragOpacityChanged(oldDragOpacity, newDragOpacity)
        End Sub

        ''' <summary>
        ''' Provides derived classes an opportunity to handle changes to the DragOpacity property.
        ''' </summary>
        ''' <param name="oldDragOpacity">Old Value</param>
        ''' <param name="newDragOpacity">New Value</param>
        Protected Overridable Sub OnDragOpacityChanged(oldDragOpacity As Double, newDragOpacity As Double)

        End Sub

#End Region

#Region "DragScale"

        ''' <summary>
        ''' DragScale Dependency Property
        ''' </summary>
        Public Shared ReadOnly DragScaleProperty As DependencyProperty = DependencyProperty.Register("DragScale", GetType(Double), GetType(FluidWrapPanel), New FrameworkPropertyMetadata(DRAG_SCALE_DEFAULT, New PropertyChangedCallback(AddressOf OnDragScaleChanged)))

        ''' <summary>
        ''' Gets or sets the DragScale property. This dependency property 
        ''' indicates the factor by which the child should be scaled when it is dragged.
        ''' </summary>
        Public Property DragScale() As Double
            Get
                Return CDbl(GetValue(DragScaleProperty))
            End Get
            Set(value As Double)
                SetValue(DragScaleProperty, Value)
            End Set
        End Property

        ''' <summary>
        ''' Handles changes to the DragScale property.
        ''' </summary>
        ''' <param name="d">FluidWrapPanel</param>
        ''' <param name="e">DependencyProperty changed event arguments</param>
        Private Shared Sub OnDragScaleChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim panel As FluidWrapPanel = DirectCast(d, FluidWrapPanel)
            Dim oldDragScale As Double = CDbl(e.OldValue)
            Dim newDragScale As Double = panel.DragScale
            panel.OnDragScaleChanged(oldDragScale, newDragScale)
        End Sub

        ''' <summary>
        ''' Provides derived classes an opportunity to handle changes to the DragScale property.
        ''' </summary>
        ''' <param name="oldDragScale">Old Value</param>
        ''' <param name="newDragScale">New Value</param>
        Protected Overridable Sub OnDragScaleChanged(oldDragScale As Double, newDragScale As Double)

        End Sub

#End Region

#Region "ElementEasing"

        ''' <summary>
        ''' ElementEasing Dependency Property
        ''' </summary>
        Public Shared ReadOnly ElementEasingProperty As DependencyProperty = DependencyProperty.Register("ElementEasing", GetType(EasingFunctionBase), GetType(FluidWrapPanel), New FrameworkPropertyMetadata((New PropertyChangedCallback(AddressOf OnElementEasingChanged))))

        ''' <summary>
        ''' Gets or sets the ElementEasing property. This dependency property 
        ''' indicates the Easing Function to be used when the elements are rearranged.
        ''' </summary>
        Public Property ElementEasing() As EasingFunctionBase
            Get
                Return DirectCast(GetValue(ElementEasingProperty), EasingFunctionBase)
            End Get
            Set(value As EasingFunctionBase)
                SetValue(ElementEasingProperty, Value)
            End Set
        End Property

        ''' <summary>
        ''' Handles changes to the ElementEasing property.
        ''' </summary>
        ''' <param name="d">FluidWrapPanel</param>
        ''' <param name="e">DependencyProperty changed event arguments</param>
        Private Shared Sub OnElementEasingChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim panel As FluidWrapPanel = DirectCast(d, FluidWrapPanel)
            Dim oldElementEasing As EasingFunctionBase = DirectCast(e.OldValue, EasingFunctionBase)
            Dim newElementEasing As EasingFunctionBase = panel.ElementEasing
            panel.OnElementEasingChanged(oldElementEasing, newElementEasing)
        End Sub

        ''' <summary>
        ''' Provides derived classes an opportunity to handle changes to the ElementEasing property.
        ''' </summary>
        ''' <param name="oldElementEasing">Old Value</param>
        ''' <param name="newElementEasing">New Value</param>
        ''' 
        Protected Overridable Sub OnElementEasingChanged(oldElementEasing As EasingFunctionBase, newElementEasing As EasingFunctionBase)

        End Sub

#End Region

#Region "IsComposing"

        ''' <summary>
        ''' IsComposing Dependency Property
        ''' </summary>
        Public Shared ReadOnly IsComposingProperty As DependencyProperty = DependencyProperty.Register("IsComposing", GetType(Boolean), GetType(FluidWrapPanel), New FrameworkPropertyMetadata((New PropertyChangedCallback(AddressOf OnIsComposingChanged))))

        ''' <summary>
        ''' Gets or sets the IsComposing property. This dependency property 
        ''' indicates if the FluidWrapPanel is in Composing mode.
        ''' </summary>
        Public Property IsComposing() As Boolean
            Get
                Return CBool(GetValue(IsComposingProperty))
            End Get
            Set(value As Boolean)
                SetValue(IsComposingProperty, Value)
            End Set
        End Property

        ''' <summary>
        ''' Handles changes to the IsComposing property.
        ''' </summary>
        ''' <param name="d">FluidWrapPanel</param>
        ''' <param name="e">DependencyProperty changed event arguments</param>
        Private Shared Sub OnIsComposingChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim panel As FluidWrapPanel = DirectCast(d, FluidWrapPanel)
            Dim oldIsComposing As Boolean = CBool(e.OldValue)
            Dim newIsComposing As Boolean = panel.IsComposing
            panel.OnIsComposingChanged(oldIsComposing, newIsComposing)
        End Sub

        ''' <summary>
        ''' Provides derived classes an opportunity to handle changes to the IsComposing property.
        ''' </summary>
        ''' <param name="oldIsComposing">Old Value</param>
        ''' <param name="newIsComposing">New Value</param>
        Protected Overridable Sub OnIsComposingChanged(oldIsComposing As Boolean, newIsComposing As Boolean)

        End Sub

#End Region

#Region "ItemHeight"

        ''' <summary>
        ''' ItemHeight Dependency Property
        ''' </summary>
        Public Shared ReadOnly ItemHeightProperty As DependencyProperty = DependencyProperty.Register("ItemHeight", GetType(Double), GetType(FluidWrapPanel), New FrameworkPropertyMetadata(0.0, New PropertyChangedCallback(AddressOf OnItemHeightChanged)))

        ''' <summary>
        ''' Gets or sets the ItemHeight property. This dependency property 
        ''' indicates the height of each item.
        ''' </summary>
        Public Property ItemHeight() As Double
            Get
                Return CDbl(GetValue(ItemHeightProperty))
            End Get
            Set(value As Double)
                SetValue(ItemHeightProperty, Value)
            End Set
        End Property

        ''' <summary>
        ''' Handles changes to the ItemHeight property.
        ''' </summary>
        ''' <param name="d">FluidWrapPanel</param>
        ''' <param name="e">DependencyProperty changed event arguments</param>
        Private Shared Sub OnItemHeightChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim fwPanel As FluidWrapPanel = DirectCast(d, FluidWrapPanel)
            Dim oldItemHeight As Double = CDbl(e.OldValue)
            Dim newItemHeight As Double = fwPanel.ItemHeight
            fwPanel.OnItemHeightChanged(oldItemHeight, newItemHeight)
        End Sub

        ''' <summary>
        ''' Provides derived classes an opportunity to handle changes to the ItemHeight property.
        ''' </summary>
        ''' <param name="oldItemHeight">Old Value</param>
        ''' <param name="newItemHeight">New Value</param>
        Protected Sub OnItemHeightChanged(oldItemHeight As Double, newItemHeight As Double)

        End Sub

#End Region

#Region "ItemWidth"

        ''' <summary>
        ''' ItemWidth Dependency Property
        ''' </summary>
        Public Shared ReadOnly ItemWidthProperty As DependencyProperty = DependencyProperty.Register("ItemWidth", GetType(Double), GetType(FluidWrapPanel), New FrameworkPropertyMetadata(0.0, New PropertyChangedCallback(AddressOf OnItemWidthChanged)))

        ''' <summary>
        ''' Gets or sets the ItemWidth property. This dependency property 
        ''' indicates the width of each item.
        ''' </summary>
        Public Property ItemWidth() As Double
            Get
                Return CDbl(GetValue(ItemWidthProperty))
            End Get
            Set(value As Double)
                SetValue(ItemWidthProperty, Value)
            End Set
        End Property

        ''' <summary>
        ''' Handles changes to the ItemWidth property.
        ''' </summary>
        ''' <param name="d">FluidWrapPanel</param>
        ''' <param name="e">DependencyProperty changed event arguments</param>
        Private Shared Sub OnItemWidthChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim fwPanel As FluidWrapPanel = DirectCast(d, FluidWrapPanel)
            Dim oldItemWidth As Double = CDbl(e.OldValue)
            Dim newItemWidth As Double = fwPanel.ItemWidth
            fwPanel.OnItemWidthChanged(oldItemWidth, newItemWidth)
        End Sub

        ''' <summary>
        ''' Provides derived classes an opportunity to handle changes to the ItemWidth property.
        ''' </summary>
        ''' <param name="oldItemWidth">Old Value</param>
        ''' <param name="newItemWidth">New Value</param>
        Protected Sub OnItemWidthChanged(oldItemWidth As Double, newItemWidth As Double)

        End Sub

#End Region

#Region "CellsPerLine"

        ''' <summary>
        ''' CellsPerLine Dependency Property
        ''' </summary>
        Public Shared ReadOnly CellsPerLineProperty As DependencyProperty = DependencyProperty.Register("CellsPerLine", GetType(Integer), GetType(FluidWrapPanel), New FrameworkPropertyMetadata(0, New PropertyChangedCallback(AddressOf OnCellsPerLineChanged)))

        ''' <summary>
        ''' Gets or sets the CellsPerLine property. This dependency property 
        ''' indicates the amount of cells per line.
        ''' </summary>
        Public Property CellsPerLine() As Integer
            Get
                Return CInt(GetValue(CellsPerLineProperty))
            End Get
            Set(value As Integer)
                SetValue(CellsPerLineProperty, Value)
            End Set
        End Property

        ''' <summary>
        ''' Handles changes to the CellsPerLine property.
        ''' </summary>
        ''' <param name="d">FluidWrapPanel</param>
        ''' <param name="e">DependencyProperty changed event arguments</param>
        Private Shared Sub OnCellsPerLineChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim fwPanel As FluidWrapPanel = DirectCast(d, FluidWrapPanel)
            Dim oldCellsPerLine As Integer = CInt(e.OldValue)
            Dim newCellsPerLine As Integer = fwPanel.CellsPerLine
            fwPanel.OnCellsPerLineChanged(oldCellsPerLine, newCellsPerLine)
        End Sub

        ''' <summary>
        ''' Provides derived classes an opportunity to handle changes to the CellsPerLine property.
        ''' </summary>
        ''' <param name="oldCellsPerLine">Old Value</param>
        ''' <param name="newCellsPerLine">New Value</param>
        Protected Sub OnCellsPerLineChanged(oldCellsPerLine As Double, newCellsPerLine As Double)

        End Sub

#End Region

#Region "ItemsSource"

        ''' <summary>
        ''' ItemsSource Dependency Property
        ''' </summary>
        Public Shared ReadOnly ItemsSourceProperty As DependencyProperty = DependencyProperty.Register("ItemsSource", GetType(IEnumerable), GetType(FluidWrapPanel), New FrameworkPropertyMetadata(New PropertyChangedCallback(AddressOf OnItemsSourceChanged)))

        ''' <summary>
        ''' Gets or sets the ItemsSource property. This dependency property 
        ''' indicates the bindable collection.
        ''' </summary>
        Public Property ItemsSource() As IEnumerable
            Get
                Return DirectCast(GetValue(ItemsSourceProperty), ObservableCollection(Of UIElement))
            End Get
            Set(value As IEnumerable)
                SetValue(ItemsSourceProperty, value)
            End Set
        End Property

        ''' <summary>
        ''' Handles changes to the ItemsSource property.
        ''' </summary>
        ''' <param name="d">FluidWrapPanel</param>
        ''' <param name="e">DependencyProperty changed event arguments</param>
        Private Shared Sub OnItemsSourceChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim panel As FluidWrapPanel = DirectCast(d, FluidWrapPanel)
            Dim oldItemsSource As IEnumerable = DirectCast(e.OldValue, ObservableCollection(Of UIElement))
            Dim newItemsSource As IEnumerable = panel.ItemsSource
            panel.OnItemsSourceChanged(oldItemsSource, newItemsSource)
        End Sub

        ''' <summary>
        ''' Provides derived classes an opportunity to handle changes to the ItemsSource property.
        ''' </summary>
        ''' <param name="oldItemsSource">Old Value</param>
        ''' <param name="newItemsSource">New Value</param>
        Protected Sub OnItemsSourceChanged(oldItemsSource As IEnumerable, newItemsSource As IEnumerable)
            ' Clear the previous items in the Children property
            Me.ClearItemsSource()

            ' Add the new children
            For Each child As UIElement In newItemsSource
                Children.Add(child)
            Next

            isInitializeArrangeRequired = True

            InvalidateVisual()
        End Sub

#End Region

#Region "Orientation"

        ''' <summary>
        ''' Orientation Dependency Property
        ''' </summary>
        Public Shared ReadOnly OrientationProperty As DependencyProperty = DependencyProperty.Register("Orientation", GetType(Orientation), GetType(FluidWrapPanel), New FrameworkPropertyMetadata(Orientation.Horizontal, New PropertyChangedCallback(AddressOf OnOrientationChanged)))

        ''' <summary>
        ''' Gets or sets the Orientation property. This dependency property 
        ''' indicates the orientation of arrangement of items in the panel.
        ''' </summary>
        Public Property Orientation() As Orientation
            Get
                Return DirectCast(GetValue(OrientationProperty), Orientation)
            End Get
            Set(value As Orientation)
                SetValue(OrientationProperty, Value)
            End Set
        End Property

        ''' <summary>
        ''' Handles changes to the Orientation property.
        ''' </summary>
        ''' <param name="d">FluidWrapPanel</param>
        ''' <param name="e">DependencyProperty changed event arguments</param>
        Private Shared Sub OnOrientationChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim panel As FluidWrapPanel = DirectCast(d, FluidWrapPanel)
            Dim oldOrientation As Orientation = DirectCast(e.OldValue, Orientation)
            Dim newOrientation As Orientation = panel.Orientation
            panel.OnOrientationChanged(oldOrientation, newOrientation)
        End Sub

        ''' <summary>
        ''' Provides derived classes an opportunity to handle changes to the Orientation property.
        ''' </summary>
        ''' <param name="oldOrientation">Old Value</param>
        ''' <param name="newOrientation">New Value</param>
        Protected Overridable Sub OnOrientationChanged(oldOrientation As Orientation, newOrientation As Orientation)
            InvalidateVisual()
        End Sub

#End Region

#End Region

#Region "Overrides"

        ''' <summary>
        ''' Override for the Measure Layout Phase
        ''' </summary>
        ''' <param name="availableSize">Available Size</param>
        ''' <returns>Size required by the panel</returns>
        Protected Overrides Function MeasureOverride(availableSize As Size) As Size
            Dim availableItemSize As New Size([Double].PositiveInfinity, [Double].PositiveInfinity)
            Dim rowWidth As Double = 0.0
            Dim maxRowHeight As Double = 0.0
            Dim colHeight As Double = 0.0
            Dim maxColWidth As Double = 0.0
            Dim totalColumnWidth As Double = 0.0
            Dim totalRowHeight As Double = 0.0

            ' Iterate through all the UIElements in the Children collection
            For i As Integer = 0 To InternalChildren.Count - 1
                Dim child As UIElement = InternalChildren(i)
                Dim behaviors = System.Windows.Interactivity.Interaction.GetBehaviors(child)
                If behaviors.Contains(New FluidMouseDragBehavior()) = False Then
                    behaviors.Add(New FluidMouseDragBehavior())
                End If
                If child IsNot Nothing Then
                    ' Ask the child how much size it needs
                    child.Measure(availableItemSize)
                    fluidElements.RemoveAll(Function(p) Not InternalChildren.Contains(p))
                    ' Check if the child is already added to the fluidElements collection
                    If Not fluidElements.Contains(child) Then
                        AddChildToFluidElements(child)
                    End If
            If Me.Orientation = Orientation.Horizontal Then
                ' Will the child fit in the current row?
                If rowWidth + child.DesiredSize.Width > availableSize.Width Then
                    ' Wrap to next row
                    totalRowHeight += maxRowHeight

                    ' Is the current row width greater than the previous row widths
                    If rowWidth > totalColumnWidth Then
                        totalColumnWidth = rowWidth
                    End If

                    rowWidth = 0.0
                    maxRowHeight = 0.0
                End If

                rowWidth += child.DesiredSize.Width
                If child.DesiredSize.Height > maxRowHeight Then
                    maxRowHeight = child.DesiredSize.Height
                End If
            Else
                ' Vertical Orientation
                ' Will the child fit in the current column?
                If colHeight + child.DesiredSize.Height > availableSize.Height Then
                    ' Wrap to next column
                    totalColumnWidth += maxColWidth

                    ' Is the current column height greater than the previous column heights
                    If colHeight > totalRowHeight Then
                        totalRowHeight = colHeight
                    End If

                    colHeight = 0.0
                    maxColWidth = 0.0
                End If

                colHeight += child.DesiredSize.Height
                If child.DesiredSize.Width > maxColWidth Then
                    maxColWidth = child.DesiredSize.Width
                End If
            End If
                End If
            Next

            If Me.Orientation = Orientation.Horizontal Then
                ' Add the height of the last row
                totalRowHeight += maxRowHeight
                ' If there is only one row, take its width as the total width
                If totalColumnWidth = 0.0 Then
                    totalColumnWidth = rowWidth
                End If
            Else
                ' Add the width of the last column
                totalColumnWidth += maxColWidth
                ' If there is only one column, take its height as the total height
                If totalRowHeight = 0.0 Then
                    totalRowHeight = colHeight
                End If
            End If

            Dim resultSize As New Size(totalColumnWidth, totalRowHeight)

            Return resultSize
        End Function

        ''' <summary>
        ''' Override for the Arrange Layout Phase
        ''' </summary>
        ''' <param name="finalSize">Available size provided by the FluidWrapPanel</param>
        ''' <returns>Size taken up by the Panel</returns>
        Protected Overrides Function ArrangeOverride(finalSize As Size) As Size
            If layoutManager Is Nothing Then
                layoutManager = New FluidLayoutManager()
            End If

            Dim maxheight As Double = 1
            Dim maxwidth As Double = 1
            For Each child As UIElement In InternalChildren
                maxheight = Math.Max(maxheight, child.DesiredSize.Height)
                maxwidth = Math.Max(maxwidth, child.DesiredSize.Width)
            Next

            'Stretch items if listboxitem = Stretch


            ' Initialize the LayoutManager
            layoutManager.Initialize(finalSize.Width, finalSize.Height, If((ItemWidth <> 0), ItemWidth, maxwidth), If((ItemHeight <> 0), ItemHeight, maxheight), Orientation, CellsPerLine)

            Dim isEasingRequired As Boolean = Not isInitializeArrangeRequired

            ' If the children are newly added, then set their initial location before the panel loads
            If (isInitializeArrangeRequired) AndAlso (Me.Children.Count > 0) Then
                InitializeArrange()
                isInitializeArrangeRequired = False
            End If

            ' Update the Layout
            UpdateFluidLayout(isEasingRequired)

            ' Return the size taken up by the Panel's Children
            Return layoutManager.GetArrangedSize(fluidElements.Count, finalSize)
        End Function

#End Region

#Region "Construction / Initialization"

        ''' <summary>
        ''' Ctor
        ''' </summary>
        Public Sub New()
            fluidElements = New List(Of UIElement)()
            layoutManager = New FluidLayoutManager()
            isInitializeArrangeRequired = True
        End Sub

#End Region

#Region "Helpers"

        ''' <summary>
        ''' Adds the child to the fluidElements collection and initializes its RenderTransform.
        ''' </summary>
        ''' <param name="child">UIElement</param>
        Private Sub AddChildToFluidElements(child As UIElement)
            ' Add the child to the fluidElements collection
            fluidElements.Add(child)
            ' Initialize its RenderTransform
            child.RenderTransform = layoutManager.CreateTransform(-ItemWidth, -ItemHeight, NORMAL_SCALE, NORMAL_SCALE)
        End Sub

        ''' <summary>
        ''' Intializes the arrangement of the children
        ''' </summary>
        Private Sub InitializeArrange()
            For Each child As UIElement In fluidElements
                ' Get the child's index in the fluidElements
                Dim index As Integer = fluidElements.IndexOf(child)

                ' Get the initial location of the child
                Dim pos As Point = layoutManager.GetInitialLocationOfChild(index)

                ' Initialize the appropriate Render Transform for the child
                child.RenderTransform = layoutManager.CreateTransform(pos.X, pos.Y, NORMAL_SCALE, NORMAL_SCALE)
            Next
        End Sub

        ''' <summary>
        ''' Iterates through all the fluid elements and animate their
        ''' movement to their new location.
        ''' </summary>
        Private Sub UpdateFluidLayout(Optional showEasing As Boolean = True)
            ' Iterate through all the fluid elements and animate their
            ' movement to their new location.
            For index As Integer = 0 To fluidElements.Count - 1
                Dim element As UIElement = fluidElements(index)
                If element Is Nothing Then
                    Continue For
                End If

                ' If an child is currently being dragged, then no need to animate it
                If dragElement IsNot Nothing AndAlso index = fluidElements.IndexOf(dragElement) Then
                    Continue For
                End If

                element.Arrange(New Rect(0, 0, element.DesiredSize.Width, element.DesiredSize.Height))

                ' Get the cell position of the current index
                Dim pos As Point = layoutManager.GetPointFromIndex(index)

                Dim transition As Storyboard
                ' Is the child being animated the same as the child which was last dragged?
                If element Is lastDragElement Then
                    If Not showEasing Then
                        ' Create the Storyboard for the transition
                        transition = layoutManager.CreateTransition(element, pos, FIRST_TIME_ANIMATION_DURATION, Nothing)
                    Else
                        ' Is easing function specified for the animation?
                        Dim duration As TimeSpan = If((DragEasing IsNot Nothing), DEFAULT_ANIMATION_TIME_WITH_EASING, DEFAULT_ANIMATION_TIME_WITHOUT_EASING)
                        ' Create the Storyboard for the transition
                        transition = layoutManager.CreateTransition(element, pos, duration, DragEasing)
                    End If

                    ' When the user releases the drag child, it's Z-Index is set to 1 so that 
                    ' during the animation it does not go below other elements.
                    ' After the animation has completed set its Z-Index to 0
                    AddHandler transition.Completed, Sub(s, e)
                                                         If lastDragElement IsNot Nothing Then
                                                             lastDragElement.SetValue(Canvas.ZIndexProperty, 0)
                                                             lastDragElement = Nothing
                                                         End If

                                                     End Sub
                Else
                    ' It is a non-dragElement
                    If Not showEasing Then
                        ' Create the Storyboard for the transition
                        transition = layoutManager.CreateTransition(element, pos, FIRST_TIME_ANIMATION_DURATION, Nothing)
                    Else
                        ' Is easing function specified for the animation?
                        Dim duration As TimeSpan = If((ElementEasing IsNot Nothing), DEFAULT_ANIMATION_TIME_WITH_EASING, DEFAULT_ANIMATION_TIME_WITHOUT_EASING)
                        ' Create the Storyboard for the transition
                        transition = layoutManager.CreateTransition(element, pos, duration, ElementEasing)
                    End If
                End If

                ' Start the animation
                transition.Begin()
            Next
        End Sub

        ''' <summary>
        ''' Moves the dragElement to the new Index
        ''' </summary>
        ''' <param name="newIndex">Index of the new location</param>
        ''' <returns>True-if dragElement was moved otherwise False</returns>
        Private Function UpdateDragElementIndex(newIndex As Integer) As Boolean
            ' Check if the dragElement is being moved to its current place
            ' If yes, then no need to proceed further. (Improves efficiency!)
            Dim dragCellIndex As Integer = fluidElements.IndexOf(dragElement)
            If dragCellIndex = newIndex Then
                Return False
            End If

            fluidElements.RemoveAt(dragCellIndex)
            fluidElements.Insert(newIndex, dragElement)


            'How to implement this

            'Throws error:
            'Children.RemoveAt(dragCellIndex)
            'Children.Insert(newIndex, dragElement)

            'Dim list As IList = New List(Of Object)
            'For Each item In ParentListBox.ItemsSource
            '    list.Add(item)
            'Next
            'Dim index As Integer = list.IndexOf(DirectCast(dragElement, ListViewItem).Content)
            'DirectCast(ParentListBox.ItemsSource, IList).RemoveAt(dragCellIndex)
            'DirectCast(ParentListBox.ItemsSource, IList).Insert(newIndex, list.Item(index))


            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            'fluidElements.RemoveAt(dragCellIndex)
            'fluidElements.Insert(newIndex, InternalChildren(newIndex))
            'dragElement = fluidElements.ElementAt(newIndex)
                Return True
        End Function

        ''' <summary>
        ''' Removes all the children from the FluidWrapPanel
        ''' </summary>
        Private Sub ClearItemsSource()
            fluidElements.Clear()
            Children.Clear()
        End Sub

#End Region

#Region "FluidDrag Event Handlers"

        ''' <summary>
        ''' Handler for the event when the user starts dragging the dragElement.
        ''' </summary>
        ''' <param name="child">UIElement being dragged</param>
        ''' <param name="position">Position in the child where the user clicked</param>
        Friend Sub BeginFluidDrag(child As UIElement, position As Point)
            If (child Is Nothing) OrElse (Not IsComposing) Then
                Return
            End If

            ' Call the event handler core on the Dispatcher. (Improves efficiency!)
            Dispatcher.BeginInvoke(New Action(Sub()
                                                  child.Opacity = DragOpacity
                                                  child.SetValue(Canvas.ZIndexProperty, Z_INDEX_DRAG)
                                                  ' Capture further mouse events
                                                  child.CaptureMouse()
                                                  dragElement = child
                                                  lastDragElement = Nothing

                                                  ' Since we are scaling the dragElement by DragScale, the clickPoint also shifts
                                                  dragStartPoint = New Point(position.X * DragScale, position.Y * DragScale)
                                                  oldIndex = fluidElements.IndexOf(child)
                                              End Sub))
        End Sub

        ''' <summary>
        ''' Handler for the event when the user drags the dragElement.
        ''' </summary>
        ''' <param name="child">UIElement being dragged</param>
        ''' <param name="position">Position where the user clicked w.r.t. the UIElement being dragged</param>
        ''' <param name="positionInParent">Position where the user clicked w.r.t. the FluidWrapPanel (the parentFWPanel of the UIElement being dragged</param>
        Friend Sub FluidDrag(child As UIElement, position As Point, positionInParent As Point)
            If (child Is Nothing) OrElse (Not IsComposing) Then
                Return
            End If

            ' Call the event handler core on the Dispatcher. (Improves efficiency!)
            Dispatcher.BeginInvoke(New Action(Sub()
                                                  If (dragElement IsNot Nothing) AndAlso (layoutManager IsNot Nothing) Then
                                                      dragElement.RenderTransform = layoutManager.CreateTransform(positionInParent.X - dragStartPoint.X, positionInParent.Y - dragStartPoint.Y, DragScale, DragScale)

                                                      ' Get the index in the fluidElements list corresponding to the current mouse location
                                                      Dim currentPt As Point = positionInParent
                                                      Dim index As Integer = layoutManager.GetIndexFromPoint(currentPt)

                                                      ' If no valid cell index is obtained, add the child to the end of the 
                                                      ' fluidElements list.
                                                      If (index = -1) OrElse (index >= fluidElements.Count) Then
                                                          index = fluidElements.Count - 1
                                                      End If

                                                      ' If the dragElement is moved to a new location, then only
                                                      ' call the updation of the layout.
                                                      If UpdateDragElementIndex(index) Then
                                                          UpdateFluidLayout()
                                                      End If
                                                  End If

                                              End Sub))
        End Sub

        ''' <summary>
        ''' Handler for the event when the user stops dragging the dragElement and releases it.
        ''' </summary>
        ''' <param name="child">UIElement being dragged</param>
        ''' <param name="position">Position where the user clicked w.r.t. the UIElement being dragged</param>
        ''' <param name="positionInParent">Position where the user clicked w.r.t. the FluidWrapPanel (the parentFWPanel of the UIElement being dragged</param>
        Friend Sub EndFluidDrag(child As UIElement, position As Point, positionInParent As Point)
            If (child Is Nothing) OrElse (Not IsComposing) Then
                Return
            End If

            ' Call the event handler core on the Dispatcher. (Improves efficiency!)
            Dispatcher.BeginInvoke(New Action(Sub()
                                                  If (dragElement IsNot Nothing) AndAlso (layoutManager IsNot Nothing) Then
                                                      dragElement.RenderTransform = layoutManager.CreateTransform(positionInParent.X - dragStartPoint.X, positionInParent.Y - dragStartPoint.Y, DragScale, DragScale)

                                                      child.Opacity = NORMAL_OPACITY
                                                      ' Z-Index is set to 1 so that during the animation it does not go below other elements.
                                                      child.SetValue(Canvas.ZIndexProperty, Z_INDEX_INTERMEDIATE)
                                                      ' Release the mouse capture
                                                      child.ReleaseMouseCapture()

                                                      ' Reference used to set the Z-Index to 0 during the UpdateFluidLayout
                                                      lastDragElement = dragElement

                                                      dragElement = Nothing
                                                  End If

                                                  UpdateFluidLayout()

                                              End Sub))

            'TODO: Fix this - Not working - Getting DisconnectedItem


            'Dim newindex As Integer = layoutManager.GetIndexFromPoint(positionInParent)
            'Dim dragCellIndex As Integer = fluidElements.IndexOf(dragElement)
            'Dim parent As ListBox = ParentListBox
            'Dim itemSource As IEnumerable = parent.ItemsSource
            'Dim typeelement As Type = dragElement.GetType()
            'Dim ls As New ObservableCollection(Of Object)(itemSource.Cast(Of Object))
            'Dim Content = ls(dragCellIndex)
            'If itemSource Is Nothing Then
            '    parent.Items.Insert(newIndex, Content)

            '    'Is the ItemsSource IList or IList? If so, insert the dragged item in the list.
            'ElseIf TypeOf itemSource Is IList Then
            '    If TypeOf dragElement Is ICloneable Then
            '        Dim copy As Object = DirectCast(dragElement, ICloneable).Clone
            '        DirectCast(itemSource, IList).RemoveAt(dragCellIndex)
            '        DirectCast(itemSource, IList).Insert(newIndex, copy)
            '    Else
            '        DirectCast(itemSource, IList).RemoveAt(dragCellIndex)
            '        DirectCast(itemSource, IList).Insert(newindex, Content)
            '    End If
            'Else
            '    Dim type As Type = itemSource.GetType()
            '    Dim genericIListType As Type = type.GetInterface("IList`1")
            '    If genericIListType IsNot Nothing Then
            '        type.GetMethod("Insert").Invoke(itemSource, New Object() {newIndex, Content})
            '    End If
            'End If

            Dim newindex As Integer = layoutManager.GetIndexFromPoint(positionInParent)

            ' If no valid cell index is obtained, add the child to the end of the 
            ' fluidElements list.
            If (newindex = -1) OrElse (newindex >= fluidElements.Count) Then
                newindex = fluidElements.Count - 1
            End If

            Dim Content = DirectCast(child, ListViewItem).Content
            Dim data As IList(Of Object) = fluidElements.Cast(Of ListViewItem)().Select(Function(p) p.Content).ToList
            RaiseEvent DataUpdated(data, Content, oldIndex, newindex)
        End Sub

        Private ReadOnly Property ParentListBox As ListBox
            Get
                If Me.Parent Is Nothing Then
                    Dim parent As DependencyObject = Me
                    While parent IsNot Nothing AndAlso Not (TypeOf parent Is ListBox)
                        parent = VisualTreeHelper.GetParent(parent)
                    End While
                    Me._parentlistbox = TryCast(parent, ListBox)
                End If
                Return Me._parentlistbox
            End Get
        End Property

        Public Event DataUpdated(data As IList(Of Object), changedItem As Object, oldindex As Integer, newIndex As Integer)

#End Region

    End Class
End Namespace
