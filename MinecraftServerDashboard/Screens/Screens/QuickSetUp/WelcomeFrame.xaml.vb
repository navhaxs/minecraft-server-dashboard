Public Class Welcome

    Dim result As Boolean = False

    Private Sub Welcome_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        If result Then
            Dim n As New MessageWindow(MyMainWindow, "", "Click 'Start server' to launch the server", "Ready Set Go!", "large")
        End If
    End Sub
    Private Sub Welcome_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        If isFreshInstall Then
            result = True
        ElseIf My.Computer.FileSystem.FileExists(System.Environment.CurrentDirectory & "\" & thisJar.SelectedValue) Then
            MyUserSettings.Jarfile = thisJar.SelectedValue
            MyServer.ReloadStartupParameters()
            result = True
        Else
            Dim n As New MessageWindow(MyMainWindow, "", "You havn't selected a jarfile yet! Dashboard can't start the server or make configuration changes until you've selected which jarfile to use." & vbNewLine & vbNewLine & "Return to this screen by clicking on the Configuration tab, and selecting 'Backend settings'", "Oops", "large")
        End If
        MyMainWindow.thisOverlayState = New MainWindow.MyOverlayState
        MyMainWindow.MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.None
        MyMainWindow.OverlayClosed()
    End Sub

    'Accessibility - let you use the spacebar to close the dialog
    Private Sub MessageWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'myFrame.Focus()
    End Sub


    Private Sub tabcontrol_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles tabcontrol.SelectionChanged
        BackButton.IsEnabled = (Not tabcontrol.SelectedIndex = 0)
        If (Not tabcontrol.SelectedIndex = 0) Then
            tabcontrol.Margin = New Thickness(80, 0, 0, 0)
        Else
            tabcontrol.Margin = New Thickness(-5, 0, 0, 0)
        End If
    End Sub

    Private Sub BackButton_Click(sender As Object, e As RoutedEventArgs) Handles BackButton.Click
        tabcontrol.SelectedIndex -= 1
    End Sub

#Region "Existing JAR"

    Private Sub btnSelectExisting_Click(sender As Object, e As RoutedEventArgs) Handles btnSelectExisting.Click
        tabcontrol.SelectedIndex += 1

        isFreshInstall = False

        thisJar.Items.Clear()
        For Each file In My.Computer.FileSystem.GetDirectoryInfo(System.Environment.CurrentDirectory).GetFiles("*.jar", IO.SearchOption.TopDirectoryOnly)
            thisJar.Items.Add(file.Name.ToString)
        Next
        thisJar.SelectedValue = MyUserSettings.Jarfile

    End Sub

    'Next btn once jar is selected
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)

        MyUserSettings.Jarfile = thisJar.SelectedValue

        Me.Close()
    End Sub

#End Region

    Dim isFreshInstall = False
    Private Sub btnGetNew_Click(sender As Object, e As RoutedEventArgs) Handles btnGetNew.Click
        isFreshInstall = True
        Dim m As New DownloaderFrame
        m.Owner = Me
        If m.isSuccessfull() Then
            Me.Close()
        End If
    End Sub
End Class