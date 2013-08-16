Public Class ExitWarning

    Private Sub btn_Minimize()
        ' Close this dialog, minimise MainWindow
        MyMainWindow.isExitWindowOverlay = False
        MyMainWindow.FormControls.Children.Remove(Me)
        MyMainWindow.MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.None
        MyMainWindow.OverlayClosed()
        MyMainWindow.WindowState = WindowState.Minimized

        If surpress.IsChecked = True Then
            My.Settings.SurpressMinimiseMessage = "m" ' Set to always minimize
        End If
    End Sub

    Private Sub ExitWarning_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Select Case My.Settings.SurpressMinimiseMessage
            Case "m"
                btn_Minimize() ' Always minimize
            Case "s"
                btn_StopSrv() ' Always stop server and exit
        End Select
    End Sub

    Sub DoCancel() Handles btnCancel.Click
        MyMainWindow.TryQuitOnExit = False
        MyMainWindow.isExitWindowOverlay = False
        MyMainWindow.FormControls.Children.Remove(Me)
        MyMainWindow.MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.None
        MyMainWindow.OverlayClosed()
    End Sub

    Private Sub btn_StopSrv() Handles btnStopSrv.Click
        If surpress.IsChecked = True Then
            My.Settings.SurpressMinimiseMessage = "s" ' Set to always minimize
        End If
        If MyServer.ServerIsOnline Then
            MyServer.StopServer()
            btnStopSrv.IsEnabled = False
            btnStopSrv.Content = "Stopping server"
            MyMainWindow.TryQuitOnExit = True
        Else
            MyMainWindow.Close()
        End If
    End Sub
End Class