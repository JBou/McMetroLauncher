Imports System.Windows

Namespace JBou.Controls
    Public Interface ILayoutStrategy
        ReadOnly Property ResultSize() As Size
        Sub Calculate(availableSize As Size, sizes As Size(), Optional ColumnCount As Integer = Integer.MaxValue)
        Function GetPosition(index As Integer) As Rect
        Function GetIndex(position As Point) As Integer
    End Interface
End Namespace