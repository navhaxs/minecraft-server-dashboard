Public Class MessageWindow

    Sub New(owner As Object, titleString As String, messageString As String, Optional headerString As String = "Oops", Optional size As String = "small")

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        If size = "large" Then
            Me.Height = 260
            Me.Width = 550
        End If

        With Me
            .Owner = owner
            .Title = titleString
            Message.Text = messageString
            Heading.Text = headerString
            .ShowDialog()
        End With
    End Sub

    Private Sub Dismiss_Click(sender As Object, e As RoutedEventArgs) Handles Dismiss.Click
        Me.Close()
    End Sub

    'Accessibility - let you use the spacebar to close the dialog
    Private Sub MessageWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dismiss.Focus()
    End Sub
End Class
