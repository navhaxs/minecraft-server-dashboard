''' <summary>
''' The SuperOverlay is displayed as a control in the MainWindow form with a dark transparent background.
''' </summary>
''' <remarks></remarks>
Public Class SuperOverlay

    Public Event isClosingTrigger()

    ' Call the isClosingTrigger to prevent unsaved changes, if any.
    Private Sub BackButton1_Click(sender As Object, e As RoutedEventArgs) Handles BackButton1.Click
        RaiseEvent isClosingTrigger()
    End Sub

    ' This sub is called after the isClosingTrigger completes
    Public Sub Confirm_DoClose(s As Object)
        MyMainWindow.thisOverlayState = New MainWindow.MyOverlayState
        MyMainWindow.MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.None
        FrameConfigOverlay = New Frame ' Clear the screen content/controls
        Me.Visibility = Windows.Visibility.Hidden
        MyMainWindow.OverlayClosed()
    End Sub

    Private Sub btnHelp_Click(sender As Object, e As RoutedEventArgs)
        On Error Resume Next ' The particular page being shown may not have a help action
        FrameConfigOverlay.Content.HelpClicked()
    End Sub

End Class