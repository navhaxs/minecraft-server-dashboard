Public Class OverlayDialog

    ''' <summary>
    ''' Display a page as an embeded screen within the MainWindow
    ''' </summary>
    Public Function DisplayConfig(ContentToDisplay As Object)
        With MyMainWindow
            .FormControls.Children.Add(Me) ' Adds shaded background
            .OverlayOpened() ' Lock controls on MainWindow from being selected (e.g. by using tab on the keyboard)
            .MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.StandardDialog
        End With

        If Not ContentToDisplay Is Nothing Then
            With ContentToDisplay
                .Owner = MyMainWindow
                .WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner

                .ShowDialog()

                If MyMainWindow.MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.StandardDialog Then
                    MyMainWindow.FormControls.Children.Remove(Me)
                    ' Don't remove lock if another dialog/overlay is present
                    Return True
                End If

            End With
        End If

        With MyMainWindow
            .FormControls.Children.Remove(Me)
            .OverlayClosed() ' Remove lock on MainWindow controls
        End With
        Return True
    End Function

End Class