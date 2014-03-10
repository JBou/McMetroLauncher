Public Class BindableRichTextBox
    Inherits RichTextBox
    'disable the control from ever being able to gain focus:




    Public Shared DocumentProperty As DependencyProperty = DependencyProperty.Register("Document", GetType(FlowDocument),
                      GetType(BindableRichTextBox), New FrameworkPropertyMetadata(Nothing,
                        New PropertyChangedCallback(AddressOf OnDocumentChanged)))

    Sub New()
        MyBase.New()
        InitializeComponent()

        Me.IsReadOnly = False
        Me.IsDocumentEnabled = True

    End Sub

    Public Shadows Property Document() As FlowDocument
        Get
            Return DirectCast(Me.GetValue(DocumentProperty), FlowDocument)
        End Get

        Set(value As FlowDocument)
            Me.SetValue(DocumentProperty, value)
        End Set
    End Property

    Public Shared Sub OnDocumentChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim rtb As RichTextBox = DirectCast(obj, RichTextBox)
        rtb.Document = DirectCast(args.NewValue, FlowDocument)
    End Sub
End Class