Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq
Imports System.Windows

Namespace JBou.Controls
    Public Class TableLayoutStrategy
        Implements ILayoutStrategy
        Private _columnCount As Integer
        Private _colWidths As Double()
        Private ReadOnly _rowHeights As New List(Of Double)()
        Private _elementCount As Integer

        Public ReadOnly Property ResultSize() As Size Implements ILayoutStrategy.ResultSize
            Get
                Return If(_colWidths IsNot Nothing AndAlso _rowHeights.Any(), New Size(_colWidths.Sum(), _rowHeights.Sum()), New Size(0, 0))
            End Get
        End Property

        Public Sub Calculate(availableSize As Size, measures As Size(), Optional ColumnCount As Integer = Integer.MaxValue) Implements ILayoutStrategy.Calculate
            If ColumnCount = Integer.MaxValue Then
                BaseCalculation(availableSize, measures)
            Else
                BaseCalculation(availableSize, measures, ColumnCount)
            End If
            AdjustEmptySpace(availableSize)
        End Sub

        Private Sub BaseCalculation(availableSize As Size, measures As Size(), Optional ColumnCount As Integer = Integer.MaxValue)
            _elementCount = measures.Length
            If ColumnCount = Integer.MaxValue Then
                _columnCount = GetColumnCount(availableSize, measures)
            Else
                _columnCount = ColumnCount
            End If
            If _colWidths Is Nothing OrElse _colWidths.Length < _columnCount Then
                _colWidths = New Double(_columnCount - 1) {}
            End If
            Dim calculating = True
            While calculating
                calculating = False
                ResetSizes()
                Dim row As Integer
                row = 0
                While row * _columnCount < measures.Length
                    Dim rowHeight = 0.0
                    Dim col As Integer
                    For col = 0 To _columnCount - 1
                        Dim i As Integer = row * _columnCount + col
                        If i >= measures.Length Then
                            Exit For
                        End If
                        _colWidths(col) = Math.Max(_colWidths(col), measures(i).Width)
                        rowHeight = Math.Max(rowHeight, measures(i).Height)
                    Next

                    If _columnCount > 1 AndAlso _colWidths.Sum() > availableSize.Width Then
                        _columnCount -= 1
                        calculating = True
                        Exit While
                    End If
                    _rowHeights.Add(rowHeight)
                    row += 1
                End While
            End While
        End Sub

        Public Function GetPosition(index As Integer) As Rect Implements ILayoutStrategy.GetPosition
            Dim columnIndex = index Mod _columnCount
            Dim rowIndex = index \ _columnCount
            Dim x = 0.0
            For i As Integer = 0 To columnIndex - 1
                x += _colWidths(i)
            Next
            Dim y = 0.0
            For i As Integer = 0 To rowIndex - 1
                y += _rowHeights(i)
            Next
            Return New Rect(New Point(x, y), New Size(_colWidths(columnIndex), _rowHeights(rowIndex)))
        End Function

        Public Function GetIndex(position As Point) As Integer Implements ILayoutStrategy.GetIndex
            Dim col = 0
            Dim x = 0.0
            While x < position.X AndAlso _columnCount > col
                x += _colWidths(col)
                col += 1
            End While
            col -= 1
            Dim row = 0
            Dim y = 0.0
            While y < position.Y AndAlso _rowHeights.Count > row
                y += _rowHeights(row)
                row += 1
            End While
            row -= 1
            If row < 0 Then
                row = 0
            End If
            If col < 0 Then
                col = 0
            End If
            If col >= _columnCount Then
                col = _columnCount - 1
            End If
            Dim result = row * _columnCount + col
            If result > _elementCount Then
                result = _elementCount - 1
            End If
            Return result
        End Function

        Private Sub AdjustEmptySpace(availableSize As Size)
            Dim width = _colWidths.Sum()
            If Not Double.IsNaN(availableSize.Width) AndAlso availableSize.Width > width Then
                Dim dif = (availableSize.Width - width) / _columnCount

                For i = 0 To _columnCount - 1
                    _colWidths(i) += dif
                Next
            End If
        End Sub

        Private Sub ResetSizes()
            _rowHeights.Clear()
            For j = 0 To _colWidths.Length - 1
                _colWidths(j) = 0
            Next
        End Sub

        Private Shared Function GetColumnCount(availableSize As Size, measures As Size()) As Integer
            Dim width As Double = 0
            For colCnt As Integer = 0 To measures.Length - 1
                Dim nwidth = width + measures(colCnt).Width
                If nwidth > availableSize.Width Then
                    Return Math.Max(1, colCnt)
                End If
                width = nwidth
            Next
            Return measures.Length
        End Function
    End Class
End Namespace