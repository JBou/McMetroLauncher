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

Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Interactivity
Imports System.Windows.Media

Namespace JBou.Controls
    ''' <summary>
    ''' Defines the Drag Behavior in the FluidWrapPanel using the Mouse
    ''' </summary>
    Public Class FluidMouseDragBehavior
        Inherits Behavior(Of UIElement)
#Region "Fields"

        Private parentFWPanel As FluidWrapPanel = Nothing
        Private parentLBItem As ListBoxItem = Nothing

#End Region

#Region "Dependency Properties"

#Region "DragButton"

        ''' <summary>
        ''' DragButton Dependency Property
        ''' </summary>
        Public Shared ReadOnly DragButtonProperty As DependencyProperty = DependencyProperty.Register("DragButton", GetType(MouseButton), GetType(FluidMouseDragBehavior), New FrameworkPropertyMetadata(MouseButton.Left))

        ''' <summary>
        ''' Gets or sets the DragButton property. This dependency property 
        ''' indicates which Mouse button should participate in the drag interaction.
        ''' </summary>
        Public Property DragButton() As MouseButton
            Get
                Return DirectCast(GetValue(DragButtonProperty), MouseButton)
            End Get
            Set(value As MouseButton)
                SetValue(DragButtonProperty, value)
            End Set
        End Property

#End Region

#End Region

#Region "Overrides"

        ''' <summary>
        ''' 
        ''' </summary>
        Protected Overrides Sub OnAttached()
            ' Subscribe to the Loaded event
            AddHandler TryCast(Me.AssociatedObject, FrameworkElement).Loaded, New RoutedEventHandler(AddressOf OnAssociatedObjectLoaded)
        End Sub

        Private Sub OnAssociatedObjectLoaded(sender As Object, e As RoutedEventArgs)
            ' Get the parent FluidWrapPanel and check if the AssociatedObject is
            ' hosted inside a ListBoxItem (this scenario will occur if the FluidWrapPanel
            ' is the ItemsPanel for a ListBox).
            GetParentPanel()

            ' Subscribe to the Mouse down/move/up events
            If parentLBItem IsNot Nothing Then
                AddHandler parentLBItem.PreviewMouseDown, New MouseButtonEventHandler(AddressOf OnPreviewMouseDown)
                AddHandler parentLBItem.PreviewMouseMove, New MouseEventHandler(AddressOf OnPreviewMouseMove)
                AddHandler parentLBItem.PreviewMouseUp, New MouseButtonEventHandler(AddressOf OnPreviewMouseUp)
            Else
                AddHandler Me.AssociatedObject.PreviewMouseDown, New MouseButtonEventHandler(AddressOf OnPreviewMouseDown)
                AddHandler Me.AssociatedObject.PreviewMouseMove, New MouseEventHandler(AddressOf OnPreviewMouseMove)
                AddHandler Me.AssociatedObject.PreviewMouseUp, New MouseButtonEventHandler(AddressOf OnPreviewMouseUp)
            End If
        End Sub

        ''' <summary>
        ''' Get the parent FluidWrapPanel and check if the AssociatedObject is
        ''' hosted inside a ListBoxItem (this scenario will occur if the FluidWrapPanel
        ''' is the ItemsPanel for a ListBox).
        ''' </summary>
        ''' 
        Private Sub GetParentPanel()
            Dim ancestor As FrameworkElement = TryCast(Me.AssociatedObject, FrameworkElement)

            While ancestor IsNot Nothing
                If TypeOf ancestor Is ListBoxItem Then
                    parentLBItem = TryCast(ancestor, ListBoxItem)
                End If

                If TypeOf ancestor Is FluidWrapPanel Then
                    parentFWPanel = TryCast(ancestor, FluidWrapPanel)
                    ' No need to go further up
                    Return
                End If

                ' Find the visual ancestor of the current item
                ancestor = TryCast(VisualTreeHelper.GetParent(ancestor), FrameworkElement)
            End While
        End Sub

        Protected Overrides Sub OnDetaching()
            RemoveHandler TryCast(Me.AssociatedObject, FrameworkElement).Loaded, AddressOf OnAssociatedObjectLoaded
            If parentLBItem IsNot Nothing Then
                RemoveHandler parentLBItem.PreviewMouseDown, AddressOf OnPreviewMouseDown
                RemoveHandler parentLBItem.PreviewMouseMove, AddressOf OnPreviewMouseMove
                RemoveHandler parentLBItem.PreviewMouseUp, AddressOf OnPreviewMouseUp
            Else
                RemoveHandler Me.AssociatedObject.PreviewMouseDown, AddressOf OnPreviewMouseDown
                RemoveHandler Me.AssociatedObject.PreviewMouseMove, AddressOf OnPreviewMouseMove
                RemoveHandler Me.AssociatedObject.PreviewMouseUp, AddressOf OnPreviewMouseUp
            End If
        End Sub

#End Region

#Region "Event Handlers"

        Private Sub OnPreviewMouseDown(sender As Object, e As MouseButtonEventArgs)
            If e.ChangedButton = DragButton Then
                Dim position As Point = If(parentLBItem IsNot Nothing, e.GetPosition(parentLBItem), e.GetPosition(Me.AssociatedObject))

                Dim fElem As FrameworkElement = TryCast(Me.AssociatedObject, FrameworkElement)
                If (fElem IsNot Nothing) AndAlso (parentFWPanel IsNot Nothing) Then
                    If parentLBItem IsNot Nothing Then
                        parentFWPanel.BeginFluidDrag(parentLBItem, position)
                    Else
                        parentFWPanel.BeginFluidDrag(Me.AssociatedObject, position)
                    End If
                End If
            End If
        End Sub

        Private Sub OnPreviewMouseMove(sender As Object, e As MouseEventArgs)
            Dim isDragging As Boolean = False

            Select Case DragButton
                Case MouseButton.Left
                    If e.LeftButton = MouseButtonState.Pressed Then
                        isDragging = True
                    End If
                    Exit Select
                Case MouseButton.Middle
                    If e.MiddleButton = MouseButtonState.Pressed Then
                        isDragging = True
                    End If
                    Exit Select
                Case MouseButton.Right
                    If e.RightButton = MouseButtonState.Pressed Then
                        isDragging = True
                    End If
                    Exit Select
                Case MouseButton.XButton1
                    If e.XButton1 = MouseButtonState.Pressed Then
                        isDragging = True
                    End If
                    Exit Select
                Case MouseButton.XButton2
                    If e.XButton2 = MouseButtonState.Pressed Then
                        isDragging = True
                    End If
                    Exit Select
                Case Else
                    Exit Select
            End Select

            If isDragging Then
                Dim position As Point = If(parentLBItem IsNot Nothing, e.GetPosition(parentLBItem), e.GetPosition(Me.AssociatedObject))

                Dim fElem As FrameworkElement = TryCast(Me.AssociatedObject, FrameworkElement)
                If (fElem IsNot Nothing) AndAlso (parentFWPanel IsNot Nothing) Then
                    Dim positionInParent As Point = e.GetPosition(parentFWPanel)
                    If parentLBItem IsNot Nothing Then
                        parentFWPanel.FluidDrag(parentLBItem, position, positionInParent)
                    Else
                        parentFWPanel.FluidDrag(Me.AssociatedObject, position, positionInParent)
                    End If
                End If
            End If
        End Sub

        Private Sub OnPreviewMouseUp(sender As Object, e As MouseButtonEventArgs)
            If e.ChangedButton = DragButton Then
                Dim position As Point = If(parentLBItem IsNot Nothing, e.GetPosition(parentLBItem), e.GetPosition(Me.AssociatedObject))

                Dim fElem As FrameworkElement = TryCast(Me.AssociatedObject, FrameworkElement)
                If (fElem IsNot Nothing) AndAlso (parentFWPanel IsNot Nothing) Then
                    Dim positionInParent As Point = e.GetPosition(parentFWPanel)
                    If parentLBItem IsNot Nothing Then
                        parentFWPanel.EndFluidDrag(parentLBItem, position, positionInParent)
                    Else
                        parentFWPanel.EndFluidDrag(Me.AssociatedObject, position, positionInParent)
                    End If
                End If
            End If
        End Sub

#End Region
    End Class
End Namespace
