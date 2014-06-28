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
Imports System.Windows.Media
Imports System.Windows.Media.Animation

Namespace JBou.Controls
    ''' <summary>
    ''' Class which helps in the layout management for the FluidWrapPanel
    ''' </summary>
    Friend NotInheritable Class FluidLayoutManager
#Region "Fields"

        Private panelSize As Size
        Private cellSize As Size
        Private panelOrientation As Orientation
        Private cellsPerLine As Int32

#End Region

#Region "APIs"

        ''' <summary>
        ''' Calculates the initial location of the child in the FluidWrapPanel
        ''' when the child is added.
        ''' </summary>
        ''' <param name="index">Index of the child in the FluidWrapPanel</param>
        ''' <returns></returns>
        Friend Function GetInitialLocationOfChild(index As Integer) As Point
            Dim result As New Point()

            Dim row As Integer, column As Integer

            GetCellFromIndex(index, row, column)

            Dim maxRows As Integer = CType(Math.Truncate(Math.Floor(panelSize.Height / cellSize.Height)), Int32)
            Dim maxCols As Integer = CType(Math.Truncate(Math.Floor(panelSize.Width / cellSize.Width)), Int32)

            Dim isLeft As Boolean = True
            Dim isTop As Boolean = True
            Dim isCenterHeight As Boolean = False
            Dim isCenterWidth As Boolean = False

            Dim halfRows As Integer = 0
            Dim halfCols As Integer = 0

            halfRows = CInt(Math.Truncate(CDbl(maxRows) / CDbl(2)))

            ' Even number of rows
            If (maxRows Mod 2) = 0 Then
                isTop = row < halfRows
            Else
                ' Odd number of rows
                If row = halfRows Then
                    isCenterHeight = True
                    isTop = False
                Else
                    isTop = row < halfRows
                End If
            End If

            halfCols = CInt(Math.Truncate(CDbl(maxCols) / CDbl(2)))

            ' Even number of columns
            If (maxCols Mod 2) = 0 Then
                isLeft = column < halfCols
            Else
                ' Odd number of columns
                If column = halfCols Then
                    isCenterWidth = True
                    isLeft = False
                Else
                    isLeft = column < halfCols
                End If
            End If

            If isCenterHeight AndAlso isCenterWidth Then
                Dim posX As Double = (halfCols) * cellSize.Width
                Dim posY As Double = (halfRows + 2) * cellSize.Height

                Return New Point(posX, posY)
            End If

            If isCenterHeight Then
                If isLeft Then
                    Dim posX As Double = ((halfCols - column) + 1) * cellSize.Width
                    Dim posY As Double = (halfRows) * cellSize.Height

                    result = New Point(-posX, posY)
                Else
                    Dim posX As Double = ((column - halfCols) + 1) * cellSize.Width
                    Dim posY As Double = (halfRows) * cellSize.Height

                    result = New Point(panelSize.Width + posX, posY)
                End If

                Return result
            End If

            If isCenterWidth Then
                If isTop Then
                    Dim posX As Double = (halfCols) * cellSize.Width
                    Dim posY As Double = ((halfRows - row) + 1) * cellSize.Height

                    result = New Point(posX, -posY)
                Else
                    Dim posX As Double = (halfCols) * cellSize.Width
                    Dim posY As Double = ((row - halfRows) + 1) * cellSize.Height

                    result = New Point(posX, panelSize.Height + posY)
                End If

                Return result
            End If

            If isTop Then
                If isLeft Then
                    Dim posX As Double = ((halfCols - column) + 1) * cellSize.Width
                    Dim posY As Double = ((halfRows - row) + 1) * cellSize.Height

                    result = New Point(-posX, -posY)
                Else
                    Dim posX As Double = ((column - halfCols) + 1) * cellSize.Width
                    Dim posY As Double = ((halfRows - row) + 1) * cellSize.Height

                    result = New Point(posX + panelSize.Width, -posY)
                End If
            Else
                If isLeft Then
                    Dim posX As Double = ((halfCols - column) + 1) * cellSize.Width
                    Dim posY As Double = ((row - halfRows) + 1) * cellSize.Height

                    result = New Point(-posX, panelSize.Height + posY)
                Else
                    Dim posX As Double = ((column - halfCols) + 1) * cellSize.Width
                    Dim posY As Double = ((row - halfRows) + 1) * cellSize.Height

                    result = New Point(posX + panelSize.Width, panelSize.Height + posY)
                End If
            End If

            Return result
        End Function

        ''' <summary>
        ''' Initializes the FluidLayoutManager
        ''' </summary>
        ''' <param name="panelWidth">Width of the FluidWrapPanel</param>
        ''' <param name="panelHeight">Height of the FluidWrapPanel</param>
        ''' <param name="cellWidth">Width of each child in the FluidWrapPanel</param>
        ''' <param name="cellHeight">Height of each child in the FluidWrapPanel</param>
        ''' <param name="orientation">Orientation of the panel - Horizontal or Vertical</param>
        Friend Sub Initialize(panelWidth As Double, panelHeight As Double, cellWidth As Double, cellHeight As Double, orientation As Orientation, Optional cellsperline__1 As Integer = 0)
            If panelWidth <= 0.0 Then
                panelWidth = cellWidth
            End If
            If panelHeight <= 0.0 Then
                panelHeight = cellHeight
            End If
            If (cellWidth <= 0.0) OrElse (cellHeight <= 0.0) Then
                cellsPerLine = 0
                Return
            End If

            If (panelSize.Width <> panelWidth) OrElse (panelSize.Height <> panelHeight) OrElse (cellSize.Width <> cellWidth) OrElse (cellSize.Height <> cellHeight) Then
                panelSize = New Size(panelWidth, panelHeight)
                cellSize = New Size(cellWidth, cellHeight)
                panelOrientation = orientation


                If cellsperline__1 = 0 Then
                    ' Calculate the number of cells that can be fit in a line
                    CalculateCellsPerLine()
                Else
                    'itemheight / width
                    cellsPerLine = cellsperline__1
                End If
            End If
        End Sub

        ''' <summary>
        ''' Provides the index of the child (in the FluidWrapPanel's children) from the given row and column
        ''' </summary>
        ''' <param name="row">Row</param>
        ''' <param name="column">Column</param>
        ''' <returns>Index</returns>
        Friend Function GetIndexFromCell(row As Integer, column As Integer) As Integer
            Dim result As Integer = -1

            If (row >= 0) AndAlso (column >= 0) Then
                Select Case panelOrientation
                    Case Orientation.Horizontal
                        result = (cellsPerLine * row) + column
                        Exit Select
                    Case Orientation.Vertical
                        result = (cellsPerLine * column) + row
                        Exit Select
                    Case Else
                        Exit Select
                End Select
            End If

            Return result
        End Function

        ''' <summary>
        ''' Provides the index of the child (in the FluidWrapPanel's children) from the given point
        ''' </summary>
        ''' <param name="p"></param>
        ''' <returns></returns>
        Friend Function GetIndexFromPoint(p As Point) As Integer
            Dim result As Integer = -1
            If (p.X > 0.0) AndAlso (p.X < panelSize.Width) AndAlso (p.Y > 0.0) AndAlso (p.Y < panelSize.Height) Then
                Dim row As Integer
                Dim column As Integer

                GetCellFromPoint(p, row, column)
                result = GetIndexFromCell(row, column)
            End If

            Return result
        End Function

        ''' <summary>
        ''' Provides the row and column of the child based on its index in the FluidWrapPanel.Children
        ''' </summary>
        ''' <param name="index">Index</param>
        ''' <param name="row">Row</param>
        ''' <param name="column">Column</param>
        Friend Sub GetCellFromIndex(index As Integer, ByRef row As Integer, ByRef column As Integer)
            row = InlineAssignHelper(column, -1)

            If index >= 0 Then
                Select Case panelOrientation
                    Case Orientation.Horizontal
                        row = CInt(Math.Truncate(index / CDbl(cellsPerLine)))
                        column = CInt(Math.Truncate(index Mod CDbl(cellsPerLine)))
                        Exit Select
                    Case Orientation.Vertical
                        column = CInt(Math.Truncate(index / CDbl(cellsPerLine)))
                        row = CInt(Math.Truncate(index Mod CDbl(cellsPerLine)))
                        Exit Select
                    Case Else
                        Exit Select
                End Select
            End If
        End Sub

        ''' <summary>
        ''' Provides the row and column of the child based on its location in the FluidWrapPanel
        ''' </summary>
        ''' <param name="p">Location of the child in the parent</param>
        ''' <param name="row">Row</param>
        ''' <param name="column">Column</param>
        Friend Sub GetCellFromPoint(p As Point, ByRef row As Integer, ByRef column As Integer)
            row = InlineAssignHelper(column, -1)
            Dim width = cellSize.Width * cellsPerLine
            If (p.X < 0.0) OrElse (p.X > panelSize.Width) OrElse (p.Y < 0.0) OrElse (p.Y > panelSize.Height) OrElse (p.X > width) Then
                Return
            End If

            row = CInt(Math.Floor(p.Y / cellSize.Height))
            column = CInt(Math.Floor(p.X / cellSize.Width))
        End Sub

        ''' <summary>
        ''' Provides the location of the child in the FluidWrapPanel based on the given row and column
        ''' </summary>
        ''' <param name="row">Row</param>
        ''' <param name="column">Column</param>
        ''' <returns>Location of the child in the panel</returns>
        Friend Function GetPointFromCell(row As Integer, column As Integer) As Point
            Dim result As New Point()

            If (row >= 0) AndAlso (column >= 0) Then
                result = New Point(cellSize.Width * column, cellSize.Height * row)
            End If

            Return result
        End Function

        ''' <summary>
        ''' Provides the location of the child in the FluidWrapPanel based on the given row and column
        ''' </summary>
        ''' <param name="index">Index</param>
        ''' <returns>Location of the child in the panel</returns>
        Friend Function GetPointFromIndex(index As Integer) As Point
            Dim result As New Point()

            If index >= 0 Then
                Dim row As Integer
                Dim column As Integer

                GetCellFromIndex(index, row, column)
                result = GetPointFromCell(row, column)
            End If

            Return result
        End Function

        ''' <summary>
        ''' Creates a TransformGroup based on the given Translation, Scale and Rotation
        ''' </summary>
        ''' <param name="transX">Translation in the X-axis</param>
        ''' <param name="transY">Translation in the Y-axis</param>
        ''' <param name="scaleX">Scale factor in the X-axis</param>
        ''' <param name="scaleY">Scale factor in the Y-axis</param>
        ''' <param name="rotAngle">Rotation</param>
        ''' <returns>TransformGroup</returns>
        Friend Function CreateTransform(transX As Double, transY As Double, scaleX As Double, scaleY As Double, Optional rotAngle As Double = 0.0) As TransformGroup
            Dim translation As New TranslateTransform()
            translation.X = transX
            translation.Y = transY

            Dim scale As New ScaleTransform()
            scale.ScaleX = scaleX
            scale.ScaleY = scaleY

            'RotateTransform rotation = new RotateTransform();
            'rotation.Angle = rotAngle;

            Dim transform As New TransformGroup()
            ' THE ORDER OF TRANSFORM IS IMPORTANT
            ' First, scale, then rotate and finally translate
            transform.Children.Add(scale)
            'transform.Children.Add(rotation);
            transform.Children.Add(translation)

            Return transform
        End Function

        ''' <summary>
        ''' Creates the storyboard for animating a child from its old location to the new location.
        ''' The Translation and Scale properties are animated.
        ''' </summary>
        ''' <param name="element">UIElement for which the storyboard has to be created</param>
        ''' <param name="newLocation">New location of the UIElement</param>
        ''' <param name="period">Duration of animation</param>
        ''' <param name="easing">Easing function</param>
        ''' <returns>Storyboard</returns>
        Friend Function CreateTransition(element As UIElement, newLocation As Point, period As TimeSpan, easing As EasingFunctionBase) As Storyboard
            Dim duration As New Duration(period)

            ' Animate X
            Dim translateAnimationX As New DoubleAnimation()
            translateAnimationX.[To] = newLocation.X
            translateAnimationX.Duration = duration
            If easing IsNot Nothing Then
                translateAnimationX.EasingFunction = easing
            End If

            Storyboard.SetTarget(translateAnimationX, element)
            Storyboard.SetTargetProperty(translateAnimationX, New PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)"))

            ' Animate Y
            Dim translateAnimationY As New DoubleAnimation()
            translateAnimationY.[To] = newLocation.Y
            translateAnimationY.Duration = duration
            If easing IsNot Nothing Then
                translateAnimationY.EasingFunction = easing
            End If

            Storyboard.SetTarget(translateAnimationY, element)
            Storyboard.SetTargetProperty(translateAnimationY, New PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.Y)"))

            ' Animate ScaleX
            Dim scaleAnimationX As New DoubleAnimation()
            scaleAnimationX.[To] = 1.0
            scaleAnimationX.Duration = duration
            If easing IsNot Nothing Then
                scaleAnimationX.EasingFunction = easing
            End If

            Storyboard.SetTarget(scaleAnimationX, element)
            Storyboard.SetTargetProperty(scaleAnimationX, New PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"))

            ' Animate ScaleY
            Dim scaleAnimationY As New DoubleAnimation()
            scaleAnimationY.[To] = 1.0
            scaleAnimationY.Duration = duration
            If easing IsNot Nothing Then
                scaleAnimationY.EasingFunction = easing
            End If

            Storyboard.SetTarget(scaleAnimationY, element)
            Storyboard.SetTargetProperty(scaleAnimationY, New PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"))

            Dim sb As New Storyboard()
            sb.Duration = duration
            sb.Children.Add(translateAnimationX)
            sb.Children.Add(translateAnimationY)
            sb.Children.Add(scaleAnimationX)
            sb.Children.Add(scaleAnimationY)

            Return sb
        End Function

        ''' <summary>
        ''' Gets the total size taken up by the children after the Arrange Layout Phase
        ''' </summary>
        ''' <param name="childrenCount">Number of children</param>
        ''' <param name="finalSize">Available size provided by the FluidWrapPanel</param>
        ''' <returns>Total size</returns>
        Friend Function GetArrangedSize(childrenCount As Integer, finalSize As Size) As Size
            If (cellsPerLine = 0.0) OrElse (childrenCount = 0) Then
                Return finalSize
            End If

            Dim numLines As Integer = CType(Math.Truncate(childrenCount / CDbl(cellsPerLine)), Int32)
            Dim modLines As Integer = childrenCount Mod cellsPerLine
            If modLines > 0 Then
                numLines += 1
            End If

            If panelOrientation = Orientation.Horizontal Then
                Return New Size(cellsPerLine * cellSize.Width, numLines * cellSize.Height)
            End If

            Return New Size(numLines * cellSize.Width, cellsPerLine * cellSize.Height)
        End Function

#End Region

#Region "Helpers"

        ''' <summary>
        ''' Calculates the number of child items that can be accommodated in a single line
        ''' </summary>
        Private Sub CalculateCellsPerLine()
            Dim count As Double = If((panelOrientation = Orientation.Horizontal), panelSize.Width / cellSize.Width, panelSize.Height / cellSize.Height)
            cellsPerLine = CType(Math.Truncate(Math.Floor(count)), Int32)
            If (1.0 + cellsPerLine - count) < [Double].Epsilon Then
                cellsPerLine += 1
            End If
        End Sub
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function

#End Region
    End Class
End Namespace
