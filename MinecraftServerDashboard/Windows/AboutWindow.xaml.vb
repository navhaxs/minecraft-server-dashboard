Public Class AboutWindow

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Close()
    End Sub

    Private Sub AboutWindow_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        MyMainWindow.MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.None
    End Sub

    Private Sub Hyperlink_Click(sender As Object, e As RoutedEventArgs)
        System.Diagnostics.Process.Start("http://navhaxs.tk/minecraft-dashboard/")
    End Sub

    Private Sub Hyperlink_Go_WWW(sender As Object, e As RequestNavigateEventArgs)
        System.Diagnostics.Process.Start(e.Uri.ToString())
    End Sub
End Class