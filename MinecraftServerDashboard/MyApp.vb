''' <summary>
''' This module holds global objects
''' </summary>
Module MyApp

    ''' <summary>
    '''  The current instance of the MainWindow, set when MainWindow is initialized
    ''' </summary>
    Public MyMainWindow As MainWindow

    ''' <summary>
    '''  The current instance of the server process, set when ServerClass is initialized
    ''' </summary>
    Public WithEvents MyServer As ServerManager

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

                                                        navpageScheduler.labelServerIsRunning.Visibility = Visibility.Visible
                                                        For Each i In navpageScheduler.UITasksList.Children
                                                            i.isEnabled = False
                                                        Next

                                                    End Sub)
                navpageScheduler.Dispatcher.BeginInvoke(Sub()
                                                            ' Clear any previous tasks
                                                            MyApp.taskScheduler.clearTasks()

                                                            ' Pass the tasks to the scheduler
                                                            For Each task In navpageScheduler.ListOfSchedulerTaskItem
                                                                MyApp.taskScheduler.addTask(task.Task)
                                                            Next

                                                            taskScheduler.startScheduler()

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
                MyMainWindow.Dispatcher.BeginInvoke(Sub()
                                                        MyMainWindow.lblserverstatus.Content = "stopping"

                                                        taskScheduler.stopScheduler() ' Stop scheduler now for a user-initiated server stop
                                                    End Sub)
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

                                               navpageScheduler.labelServerIsRunning.Visibility = Visibility.Hidden
                                               For Each i In navpageScheduler.UITasksList.Children
                                                   i.isEnabled = True
                                               Next

                                               taskScheduler.stopScheduler()

                                           End Sub))
        End Select

        'Update the home tab (dashboard) labels and icons, etc. to reflect the changed state of the server
        MyMainWindow.Dispatcher.BeginInvoke(Sub() navpageDashboard.UpdateDashboardIndicators())
    End Sub

    ''' <summary>
    ''' Prompt the user to sign Mojang's EULA.txt file if required
    ''' </summary>
    Private Sub MyServer_PromptEULAUserAction() Handles MyServer.PromptEULAUserAction
        MyMainWindow.Dispatcher.BeginInvoke(Sub()
                                                Dim n As New MessageWindow(MyMainWindow, "", "Mojang requires you to agree to their End User Licence Agreement." & vbNewLine & vbNewLine & "Dashboard will now open a text editor where you can agree to the EULA.", "EULA", "large")
                                                Try
                                                    System.Diagnostics.Process.Start(MyUserSettings.UserSettings_DefaultTextEditor, MyServer.MyStartupParameters.ServerPath & "\eula.txt")
                                                Catch ex As Exception
                                                    System.Diagnostics.Process.Start("explorer.exe", MyServer.MyStartupParameters.ServerPath)
                                                End Try
                                            End Sub)
    End Sub


    '''
    Public Const DEFAULT_CONFIG_FILENAME As String = "dashboard.jsn"

    'TabPages:
    Public navpageDashboard As PageDashboard

    Public navpagePlayers As PagePlayers

    Public navpageWorld As pageWorld

    Public navpageConsole As PageConsole

    Public navpageScheduler As PageScheduler

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

#Region "task scheduler"
    Public taskScheduler As New TaskScheduler
#End Region
End Module