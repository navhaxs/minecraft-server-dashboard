''' <summary>
''' This module holds shared variables throughout this project
''' </summary>
Module MyApp

    ''' <summary>
    '''  The current instance of the MainWindow, set when MainWindow is initialized
    ''' </summary>
    Public MyMainWindow As MainWindow

    ''' <summary>
    '''  The current instance of the server process, set when ServerClass is initialized
    ''' </summary>
    Public WithEvents MyServer As ServerClass

    ''' <summary>
    ''' This object holds the results of the IP Address retrieval
    ''' This is data binded with the UI
    ''' </summary>
    Friend MyExternalIPAddressViewModel As New GetIPViewModel

    ''' <summary>
    ''' Updates the dashboard UI whenever a change in the server process' state is detected
    ''' </summary>
    Private Sub MyServer_ServerStateChanged(newstate As ServerState) Handles MyServer.ServerStateChanged
        'Debug.Print("Server State Changed! --> " & newstate.ToString)
        Select Case newstate
            Case ServerState.Running
                MyMainWindow.Dispatcher.BeginInvoke(Sub()
                                                        MyMainWindow.lblserverstatus.Content = "online"
                                                    End Sub)
            Case ServerState.WarmUp
                MyMainWindow.Dispatcher.BeginInvoke( _
                                                New Action(Sub()
                                                               navpageWorld.ListBox_MyWorldProfiles_SelectionChanged()
                                                               With MyMainWindow
                                                                   .lblserverstatus.Content = "startup..."
                                                                   .EllipseOffline.Visibility = Visibility.Hidden
                                                               End With
                                                           End Sub))
            Case ServerState.Stopping
                MyMainWindow.Dispatcher.BeginInvoke(Sub() MyMainWindow.lblserverstatus.Content = "stopping")
            Case ServerState.NotRunning
                MyMainWindow.Dispatcher.BeginInvoke( _
                                New Action(Sub()
                                               navpageWorld.ListBox_MyWorldProfiles_SelectionChanged()
                                               navPageCBconfig.btnRestart.Visibility = Visibility.Collapsed
                                               navPageCBconfig.Data.Is_infotip_RestartRequired = System.Windows.Visibility.Collapsed
                                               With MyMainWindow
                                                   .lblserverstatus.Content = "offline"
                                                   .EllipseOffline.Visibility = Visibility.Visible
                                               End With
                                           End Sub))
        End Select

        'Update the home tab (dashboard) labels and icons, etc. to reflect the changed state of the server
        MyMainWindow.Dispatcher.BeginInvoke(Sub() navpageDashboard.UpdateDashboardIndicators())
    End Sub

    'TabPages:
    Public navpageDashboard As PageDashboard

    Public navpagePlayers As PagePlayers

    Public navpageWorld As pageWorld

    Public navpageConsole As PageConsole

    Public navPageCBconfig As PageConfig

    ' Exit the app upon the exit of the server process, where elected by the user. (See the form: Screens -> Screens -> ExitWarning.xaml)
    Private Sub MyServer_ServerStopped() Handles MyServer.ServerStopped
        If MyMainWindow.TryQuitOnExit Then
            MyMainWindow.Dispatcher.BeginInvoke( _
                                            New Action(Sub()
                                                           MyMainWindow.Close()
                                                       End Sub))

        End If
    End Sub
End Module