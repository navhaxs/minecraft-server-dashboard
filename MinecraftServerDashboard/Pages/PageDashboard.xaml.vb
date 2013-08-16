Class PageDashboard

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        lblIPaddress.DataContext = MyExternalIPAddressViewModel
    End Sub

    ''' <summary>
    ''' Changes the graphic beneath the rocket ship to suit server state.
    ''' </summary>
    Sub UpdateDashboardIndicators()
        Select Case MyServer.CurrentServerState
            Case ServerState.Running
                lblServerStatus.Content = "Online"
                SetImage("StatusAnnotations_Play_32xLG_color.png")
                lblTipServerOnline.Visibility = Windows.Visibility.Visible
            Case ServerState.WarmUp
                lblServerStatus.Content = "Starting up..."
                SetImage("StatusAnnotations_Play_32xLG_color.png")
                btnStartStop.Content = "Stop server"
                lblTipServerOffline.Visibility = Windows.Visibility.Hidden
            Case ServerState.NotRunning
                lblServerStatus.Content = "Offline"
                SetImage("StatusAnnotations_Critical_32xLG_color.png")
                btnStartStop.Content = "Start server"
                lblTipServerOnline.Visibility = Windows.Visibility.Hidden
                lblTipServerOffline.Visibility = Windows.Visibility.Visible
            Case ServerState.BindCritical
                lblServerStatus.Content = "Error"
                SetImage("StatusAnnotations_Warning_32xLG_color.png")
            Case ServerState.Reloading
                lblServerStatus.Content = "Reloading..."
                SetImage("StatusAnnotations_Alert_32xLG_color.png")
            Case ServerState.Stopping
                lblServerStatus.Content = "Shutting down..."
                SetImage("StatusAnnotations_Alert_32xLG_color.png")
                lblTipServerOnline.Visibility = Windows.Visibility.Hidden
        End Select
    End Sub
    Function SetImage(url As String) ' Sets the new graphic by reading the stored image in the application's resources
        On Error Resume Next

        Dim ImageIcon As New BitmapImage
        ImageIcon.BeginInit()
        ImageIcon.CacheOption = BitmapCacheOption.OnLoad

        ' WPF-specific image location within the application's EXE
        ImageIcon.UriSource = New Uri("/MinecraftServerDashboard;component/Images/State/" & url, UriKind.Relative)
        imgServerState.Source = ImageIcon

        ImageIcon.EndInit()
        Return True
    End Function

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        If Not MyServer.ServerIsOnline Then
            MyServer.StartServer()
        Else
            MyServer.StopServer()
        End If
    End Sub

    Private Sub btnDoBackupNow_Click(sender As Object, e As RoutedEventArgs) Handles btnDoBackupNow.Click
        navpageWorld.DoBackupScreen()
    End Sub
End Class