Public Class btnHelp
    Inherits Button

    Event Clicked()

    Private Sub thisGrid_MouseDown(sender As Object, e As MouseButtonEventArgs)
        RaiseEvent Clicked()
    End Sub
End Class