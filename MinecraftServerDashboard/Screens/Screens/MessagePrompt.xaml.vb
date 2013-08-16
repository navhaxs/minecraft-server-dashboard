Public Class MessagePrompt

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        MyMainWindow.FormControls.Children.Remove(Me)
        MyMainWindow.MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.None
        MyMainWindow.OverlayClosed()
    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        btnOK.Focus()
    End Sub
End Class