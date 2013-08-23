Class PageConfig

    Public Data As New PageConfigViewModel

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.DataContext = Data
        IsServerIsOnline.DataContext = MyServer
    End Sub

#Region "Button actions"

    Private Sub Go_JavaSettings(sender As Object, e As RoutedEventArgs) Handles Button4.Click
        Dim s As New SuperOverlay
        s.btnHelp.Visibility = Windows.Visibility.Visible
        ShowConfigPage(New ConfigJava(s), s)
    End Sub

    Public Sub Go_ServerPropertiesSettings() Handles Button6.Click
        Dim s As New SuperOverlay
        Dim m As New ConfigServerProp(s)
        m.Load()
        s.btnHelp.Visibility = Windows.Visibility.Visible
        ShowConfigPage(m, s)
    End Sub

    Public Sub Go_BackendSetup() Handles Button1.Click
        Dim s As New SuperOverlay
        ShowConfigPage(New ConfigJarfileBackend(s), s)
    End Sub

    Private Sub Go_DashboardSettings(sender As Object, e As RoutedEventArgs) Handles Button2.Click
        Dim s As New SuperOverlay
        ShowConfigPage(New ConfigMyApp(s), s)
    End Sub

    Private Sub RestartBanner_Click(sender As Object, e As RoutedEventArgs)
        sender.Visibility = Windows.Visibility.Collapsed
        RestartServer(True)
    End Sub

    Private Sub StopSeverBanner_Click(sender As Object, e As RoutedEventArgs)
        MyServer.StopServer()
    End Sub

    Public Sub Go_OpenBackups_inWindowsExplorer() Handles btnOpenBackups.Click
        If My.Computer.FileSystem.DirectoryExists(MyServer.MyStartupParameters.ServerPath & "\world-backups") Then
            System.Diagnostics.Process.Start("explorer.exe", MyServer.MyStartupParameters.ServerPath & "\world-backups")
        Else
            MessageBox.Show("No previous backups have been found, or the server backend has not yet been set up yet.")
        End If
    End Sub

    Public Sub Go_ExploreSreverDirectory_inWindowsExplorer() Handles Button5.Click
        If My.Computer.FileSystem.DirectoryExists(MyServer.MyStartupParameters.ServerPath) Then
            System.Diagnostics.Process.Start("explorer.exe", MyServer.MyStartupParameters.ServerPath)
        Else
            MessageBox.Show("Please configure the server backend first")
            Go_BackendSetup()
        End If
    End Sub

#End Region

    ''' <summary>
    ''' Prepare the UI to show a settings page
    ''' </summary>
    Sub ShowConfigPage(newpage As Object, s As SuperOverlay)
        MyMainWindow.MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.StandardDialog
        MyMainWindow.FormControls.Children.Add(s)
        s.lblHeader.Content = newpage.Title

        With s
            .VerticalAlignment = Windows.VerticalAlignment.Stretch
            .HorizontalAlignment = Windows.HorizontalAlignment.Stretch

            ' Set the content to the settings page
            .FrameConfigOverlay.Content = newpage

            .Visibility = Windows.Visibility.Visible
            .FrameConfigOverlay.Focus()
        End With

        Try
            ' Add event handlers for the closing action
            AddHandler s.isClosingTrigger, AddressOf newpage.isClosing
            MyMainWindow.thisOverlayState = New MainWindow.MyOverlayState
            AddHandler MyMainWindow.thisOverlayState.isOverlay_CloseWindowEvent, AddressOf newpage.isClosing
        Catch ex As Exception

        End Try

    End Sub

End Class