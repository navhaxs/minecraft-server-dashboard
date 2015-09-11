Class PageConfig

    Public Data As PageConfigViewModel

    Friend viewSuperOverlay As SuperOverlay

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Data = New PageConfigViewModel
        Me.DataContext = Data
        IsServerIsOnline.DataContext = MyServer
    End Sub

#Region "Button actions"

    Private Sub Go_JavaSettings(sender As Object, e As RoutedEventArgs) Handles Button4.Click
        viewSuperOverlay = New SuperOverlay
        ShowConfigPage(New ConfigJava(viewSuperOverlay), viewSuperOverlay)
    End Sub

    Public Sub Go_ServerPropertiesSettings() Handles Button6.Click
        viewSuperOverlay = New SuperOverlay
        Dim m As New ConfigServerProp(viewSuperOverlay)
        m.Load()
        ShowConfigPage(m, viewSuperOverlay)
    End Sub

    Public Sub Go_BackendSetup() Handles Button1.Click
        viewSuperOverlay = New SuperOverlay
        ShowConfigPage(New ConfigJarfileBackend(viewSuperOverlay), viewSuperOverlay)
    End Sub

    Private Sub Go_DashboardSettings(sender As Object, e As RoutedEventArgs) Handles Button2.Click
        viewSuperOverlay = New SuperOverlay
        ShowConfigPage(New ConfigMyApp(viewSuperOverlay), viewSuperOverlay)
    End Sub

    Private Sub RestartBanner_Click(sender As Object, e As RoutedEventArgs)
        sender.Visibility = Windows.Visibility.Collapsed
        MyServer.RestartServer(True)
    End Sub

    Private Sub StopSeverBanner_Click(sender As Object, e As RoutedEventArgs)
        MyServer.StopServer()
    End Sub

    Public Sub Go_OpenBackups_inWindowsExplorer() Handles btnOpenBackups.Click
        If My.Computer.FileSystem.DirectoryExists(MyServer.MyStartupParameters.ServerPath & "\world-backups") Then
            System.Diagnostics.Process.Start("explorer.exe", MyServer.MyStartupParameters.ServerPath & "\world-backups")
        Else
            Dim n As New MessageWindow(MyMainWindow, "", "Either you haven't made any backups, or the server backend has not been set up.", "Nothing found")
        End If
    End Sub

    Public Sub Go_ExploreSreverDirectory_inWindowsExplorer() Handles Button5.Click
        If Not MyMainWindow.ExploreServerDir Then Go_BackendSetup()
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