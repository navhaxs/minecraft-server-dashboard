Public Class Welcome

    'Accessibility - let you use the spacebar to close the dialog
    Private Sub MessageWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dismiss.Focus()
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Me.Close()
    End Sub
End Class