Public Class ConfirmWorldDeletion

    Private result As Boolean = False

    Public Function ShowMessage(worldname As String)
        txtWorldName.Text = worldname
        Me.ShowDialog()
        Return result
    End Function

    Private Sub Yes_Click(sender As Object, e As RoutedEventArgs)
        result = True
        Me.Close()
    End Sub

    Private Sub Cancel_Click(sender As Object, e As RoutedEventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

End Class