Public Class ExitWarning

    Private Sub btn_Minimize()
        If always_suppress.IsChecked = True Then
            My.Settings.SuppressMinimiseMessage = "m" ' Set to always minimize
        End If
        ' Close this dialog, minimise MainWindow
        MyMainWindow.isExitWindowOverlay = False
        MyMainWindow.FormControls.Children.Remove(Me)
        MyMainWindow.MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.None
        MyMainWindow.OverlayClosed()
        MyMainWindow.WindowState = WindowState.Minimized
    End Sub

    Private Sub ExitWarning_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Select Case My.Settings.SuppressMinimiseMessage
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
        If always_suppress.IsChecked = True Then
            My.Settings.SuppressMinimiseMessage = "s" ' Set to always minimize
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