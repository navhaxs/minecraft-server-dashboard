Imports System.Diagnostics
Imports System.Text.RegularExpressions
Imports System.ComponentModel
Imports System.Threading

Public Class ServerManager
    Implements INotifyPropertyChanged

    ''' <summary>
    ''' The Minecraft Java server process
    ''' </summary>
    Private WithEvents ServerProc As New Process

    ''' <summary>
    ''' The startup Minecraft server parameters
    ''' </summary>
    Public Property MyStartupParameters As New StartupParameters

    ''' <summary>
    ''' Fired when the server's running state is changed
    ''' </summary>
    ''' <param name="newstate">The current state that the Minecraft server is now in</param>
    Public Event ServerStateChanged(ByVal newstate As ServerState)

    ''' <summary>
    ''' Fired when the server process has exited
    ''' </summary>
    Public Event ServerStopped()

    ''' <summary>
    ''' Fired when the server requires the EULA signed
    ''' </summary>
    Public Event PromptEULAUserAction()

    ''' <summary>
    ''' Is the ServerManager currently attempting to stop the server
    ''' </summary>
    ''' <remarks>Hopefully prevents deadlock</remarks>
    Private isExiting As Boolean = False

    Public ReadOnly Property getIsExiting As Boolean
        Get
            Return isExiting
        End Get
    End Property

#Region "INotifyPropertyChanged - WPF UI Binding"
    'This code is used to bind between the server's console data and the frontend textbox UI

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
    ' Create the OnPropertyChanged method to raise the event
    Protected Sub OnPropertyChanged(ByVal name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
#End Region

#Region "My Server Settings"
    ''' <summary>
    ''' Sets up the parameters in which to the server is to be launched with
    ''' </summary>
    Function ReloadStartupParameters()

        Dim javaexe As String
        If MyUserSettings.settingsStore.Startup_JavaExec.Length = 0 Then 'Set the default if blank
            If My.Computer.FileSystem.FileExists(DetectJava.FindPath & "\bin\java.exe") Then
                javaexe = DetectJava.FindPath & "\bin\java.exe"
            Else
                javaexe = "java"
            End If
        Else
            javaexe = MyUserSettings.settingsStore.Startup_JavaExec
        End If

        'DEBUGGING javaexe = System.Environment.CurrentDirectory & "\java.exe"

        ServerProc = New Process With {
            .StartInfo = New ProcessStartInfo() With {
                .FileName = javaexe,
                .WorkingDirectory = MyStartupParameters.ServerPath,
                .Arguments = MyStartupParameters.FullParameters,
                .RedirectStandardInput = True,
                .RedirectStandardError = True,
                .RedirectStandardOutput = True,
                .UseShellExecute = False,
                .CreateNoWindow = True
            }
        }

        ' Redirect Java's stdio to Dashboard
        ServerProc.EnableRaisingEvents = True

        Return True
    End Function
#End Region

#Region "Server console stream parsing"

    Private _consolestream As String ' This variable holds the entire stdio of the server

    ''' <summary>
    ''' The server process' output stream
    ''' </summary>
    Public Property ConsoleStream As String
        Get
            Return _consolestream
        End Get
        Set(value As String)
            _consolestream = value
            OnPropertyChanged("ConsoleStream") ' Trigger UI update
        End Set
    End Property

    ''' <summary>
    ''' Send a command to the Minecraft server
    ''' </summary>
    ''' <param name="command">The command to pass to the Minecraft server</param>
    Public Function SendCommand(command As String) As Boolean
        MyServer.ConsoleStream += ">" & command & vbLf
        If ServerIsOnline = True Then 'Only send commands if the server process is running
            ServerProc.StandardInput.Write(command & vbLf) ' Write the command into the process, then press 'enter'
            Return True
        Else
            'Do nothing if the server is NOT running
            ConsoleStream += "[Dashboard] Start the server first before running commands! Failed to execute command: " & command & vbLf
            Return False
        End If
    End Function

    ''' <summary>
    ''' Called when the Minecraft server process exists
    ''' </summary>
    Private Sub ServerProc_Exited(sender As Object, e As EventArgs) Handles ServerProc.Exited
        On Error Resume Next ' Occasionally the .NET framework incorrectly triggers the Exited event
        If ServerProc.HasExited Then

            isGettingPlayerListActivity = isGettingPlayerListActivity_STATE.Null
            CurrentServerState = ServerState.NotRunning
            RaiseEvent ServerStopped()

            Dim exitmssg As String
            If ServerProc.ExitCode = 0 Then
                exitmssg = "The server stopped successfully."
            Else
                exitmssg = "The server stopped with an error code of " & ServerProc.ExitCode
            End If
            ConsoleStream += "[Dashboard] " & exitmssg & vbLf
        End If

    End Sub

    '/* SAMPLE CONSOLE OUTPUT */
    '[INFO] Starting minecraft server version 1.5.1
    '[INFO] Loading properties
    '[INFO] Default game type: SURVIVAL
    '[INFO] Generating keypair
    '[INFO] Starting Minecraft server on *:25565
    '[WARNING] **** SERVER IS RUNNING IN OFFLINE/INSECURE MODE!
    '[WARNING] The server will make no attempt to authenticate usernames. Beware.
    '[WARNING] While this makes the game possible to play without internet access, it also opens up the ability for hackers to connect with any username they choose.
    '[WARNING] To change this, set "online-mode" to "true" in the server.properties file.
    '[INFO] Preparing level "world"
    '[INFO] Preparing start region for level 0
    '[INFO] Preparing spawn area: 77%
    '[INFO] Done (1.349s)! For help, type "help" or "?"

#Region "Logic to enable detection of the specific console output when retrieving the names of online players"

    ' Whether any recieved console entries should be checked for output from the "list" command
    Private _isGettingPlayerList As isGettingPlayerListActivity_STATE = isGettingPlayerListActivity_STATE.Null

    'Private _isGettingPlayerList_linecount As Integer = 0
    Public Enum isGettingPlayerListActivity_STATE As Integer
        ''' <summary>
        ''' Don't/stop detecting for online players in the console output
        ''' </summary>
        Null = -1

        ''' <summary>
        ''' Begin retrieving the list of online players
        ''' </summary>
        LookingForMatch = 0

        ''' <summary>
        ''' The server console responded to the request to list the online players
        ''' </summary>
        FoundMatch = 1
    End Enum

    ''' <summary>
    ''' Current state of the 'getting list of online players' activity
    ''' </summary>
    Public Property isGettingPlayerListActivity As isGettingPlayerListActivity_STATE
        Get
            Return _isGettingPlayerList
        End Get
        Set(value As isGettingPlayerListActivity_STATE)
            _isGettingPlayerList = value

            ' Initiate player list update
            If value = isGettingPlayerListActivity_STATE.LookingForMatch Then
                ' Send the "list" command
                SendCommand("list")
            End If

        End Set
    End Property
#End Region

#Region "Logic to enable detection of the completion of a 'world saved' event "

    'VANILLA/FORGE Minecraft sample output:
    '[INFO] Saving...
    '[INFO] Saved the world
    '[INFO] Turned off world auto-saving

    'CraftBukkit Minecraft sample output:
    '22:28:25 [INFO] CONSOLE: Forcing save..
    '22:28:25 [INFO] CONSOLE: Save complete.
    '22:29:01 [INFO] CONSOLE: Disabled level saving..

    Event Detected_WorldSavedCompleted()

    ''' <summary>
    ''' Current state of the 'world saved' detection activity
    ''' </summary>
    Friend isWaitingForWorldSavedActivity As Boolean = False

#End Region

    'TODO: REGEX
    Private Sub ServerProc_ErrorDataReceived(sender As Object, e As DataReceivedEventArgs) Handles ServerProc.OutputDataReceived, ServerProc.ErrorDataReceived
        If Not (e.Data = Nothing) Then

            If isWaitingForWorldSavedActivity Then
                ' Two different strings to detect depending on Minecraft server type (VANILLA (and Forge) servers // CraftBukkit servers)
                ' VANILLA 1.7.2
                '    [...timestamp...] [Server thread/INFO]: Saved the world
                If e.Data.EndsWith("INFO] CONSOLE: Save complete.") Or e.Data.EndsWith("INFO]: CONSOLE: Save complete.") Or e.Data.EndsWith("INFO] Saved the world") Or e.Data.EndsWith("[Server thread/INFO]: Saved the world") Or e.Data.EndsWith("INFO] [Minecraft-Server] Saved the world") Then
                    isWaitingForWorldSavedActivity = False
                    RaiseEvent Detected_WorldSavedCompleted()
                End If
            End If

            'Check for a new player joining in the log entry, e.g.
            '[INFO] bearbear12345[/127.0.0.1:64087] logged in with entity id 42 at (1220.6917402868266, 99.0, 318.87037902873044)
            '2013-06-22 11:19:18 [INFO] Jeremy[/127.0.0.1:52315] logged in with entity id 346 at (254.5, 73.0, 241.5)
            '2013-06-24 23:12:25 [INFO] Notch lost connection: disconnect.quitting
            '[INFO] bearbear12345 lost connection: disconnect.quitting
            '2013-06-24 23:28:27 [INFO] username lost connection: disconnect.genericReason
            If e.Data.Contains("INFO] ") Or e.Data.Contains("INFO]: ") Then
                If e.Data.EndsWith(")") Then
                    If e.Data.Contains(" logged in with entity id ") And e.Data.Contains(" at (") Then
                        isGettingPlayerListActivity = isGettingPlayerListActivity_STATE.LookingForMatch 'Trigger the event to refresh the online player list
                    End If
                ElseIf e.Data.Contains(" lost connection: disconnect.") Then
                    isGettingPlayerListActivity = isGettingPlayerListActivity_STATE.LookingForMatch 'Trigger the event to refresh the online player list
                ElseIf e.Data.Contains(" Kicked ") Then
                    '2013-07-09 16:43:59 [INFO] Kicked bearbear12345 from the game
                    '[INFO] CONSOLE: Kicked player bearbear12345. With reason:
                    isGettingPlayerListActivity = isGettingPlayerListActivity_STATE.LookingForMatch 'Trigger the event to refresh the online player list

                End If

            End If

            If isGettingPlayerListActivity = isGettingPlayerListActivity_STATE.FoundMatch Then

                'Get the list of online players from the console output line
                Dim PlayerList As List(Of Player) = ProcessPlayerList(e.Data)

                'Update UI
                navpageDashboard.Dispatcher.BeginInvoke(
                                    New Action(Sub()

                                                   navpageDashboard.MyOnlinePlayerList.StackPanel.Children.Clear()
                                                   For Each p In PlayerList
                                                       navpageDashboard.MyOnlinePlayerList.StackPanel.Children.Add(New PlayerTile(p))
                                                   Next
                                                   Dim b As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)
                                                   navpageDashboard.MyOnlinePlayerList.lblPlayerCounter.Content = PlayerList.Count & "/" & b.ReturnConfigValue("max-players")
                                               End Sub))

                'Clear the search flag
                isGettingPlayerListActivity = isGettingPlayerListActivity_STATE.Null
            End If

            If isGettingPlayerListActivity = isGettingPlayerListActivity_STATE.LookingForMatch Then
                If e.Data.Contains("INFO] There are ") _
                    Or e.Data.Contains("INFO] [Minecraft-Server] There are ") _
                    Or e.Data.Contains("INFO]: There are ") Then

                    If e.Data.Contains("INFO] There are 0/") Or e.Data.Contains("INFO]: There are 0/") Then
                        'If there are no players, simply clear player list UI now
                        navpageDashboard.Dispatcher.BeginInvoke(
                                    New Action(Sub()

                                                   navpageDashboard.MyOnlinePlayerList.StackPanel.Children.Clear()
                                                   Dim b As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)
                                                   navpageDashboard.MyOnlinePlayerList.lblPlayerCounter.Content = "0/" & b.ReturnConfigValue("max-players")
                                               End Sub))
                        isGettingPlayerListActivity = isGettingPlayerListActivity_STATE.Null
                    Else
                        'Else set the flag to catch the next line of output
                        isGettingPlayerListActivity = isGettingPlayerListActivity_STATE.FoundMatch
                    End If
                End If
            End If

            'Check if the server has completed initializing, e.g. line matching the string:
            '2013-05-20 12:34:07 [INFO] Done (1.701s)! For help, type "help" or "?"
            If CurrentServerState = ServerState.WarmUp Then
                If (e.Data.Contains("INFO] Done (") Or e.Data.Contains("INFO]: Done (") Or e.Data.Contains("INFO] [Minecraft-Server] Done (")) And e.Data.EndsWith("s)! For help, type ""help"" or ""?""") Then
                    CurrentServerState = ServerState.Running
                ElseIf (e.Data.Contains(" [Server thread/INFO]: You need to agree to the EULA in order to run the server. Go to eula.txt for more info.")) Then
                    RaiseEvent PromptEULAUserAction()
                End If
            End If

            'Writeout lines to the applications's UI (console tab)
            ConsoleStream += e.Data & vbLf
        End If
    End Sub

#Region "Console UI"
    'This flag is used to toggle-off the 'server not started' message in the UI's Console tab
    Dim _HasColdBootFlag As System.Windows.Visibility = Visibility.Collapsed
    Public ReadOnly Property ServerColdBoot As System.Windows.Visibility
        Get
            Return _HasColdBootFlag
        End Get
    End Property
#End Region

#End Region

#Region "Server Start/Stop actions"
    Public Sub StartServer()
        If Not ServerIsOnline Then
            Try
                ' Update flags
                CurrentServerState = ServerState.WarmUp

                ' Initialize startup parameters

                If Not ReloadStartupParameters() Then
                    navPageCBconfig.Go_BackendSetup()
                    Exit Sub
                End If

                ' Prepare the RAM monitor background thread
                Dim RAMmonitorThread As New System.Threading.Thread(
                    New System.Threading.ParameterizedThreadStart(
                        AddressOf ProcessStatMonitor)) With {
                            .IsBackground = True
                                }

                ' Start the server process
                With ServerProc
                    ConsoleStream += "[Dashboard] Startup parameters: " & .StartInfo.Arguments & vbLf
                    ConsoleStream += "[Dashboard] Java executable: " & ServerProc.StartInfo.FileName & vbLf
                    ConsoleStream += "[Dashboard] Starting server..."
                    .Start()
                    ConsoleStream += " OK! [" & ServerProc.StartTime & "]" & vbLf
                    ' Begin reading from stdio (and stderr)
                    .BeginErrorReadLine()
                    .BeginOutputReadLine()
                    isExiting = False
                End With

                ' Start RAM monitor thread
                RAMmonitorThread.Start()

                ' Update UI
                _HasColdBootFlag = Visibility.Visible
                OnPropertyChanged("ServerColdBoot")

            Catch ex As Exception
                MessageBox.Show("An error occured whilst trying to start the server!" & vbNewLine & "Exception error message: " & ex.Message, "Failed to launch server")
            End Try
        End If
    End Sub

    Sub StopServer()
        On Error Resume Next
        If ServerIsOnline Then
            isExiting = True
            CurrentServerState = ServerState.Stopping
            ConsoleStream += "[Dashboard] Asking server to stop... " & vbLf
            ServerProc.StandardInput.WriteLine("stop")
        End If
    End Sub

    ''' <summary>
    ''' Kill any running instance of the Minecraft server
    ''' </summary>
    Public Sub KillServer()
        On Error Resume Next ' crash handling done right in .net
        If Not MyServer.ServerProc.HasExited Then
            MyServer.ServerProc.Kill()
        End If
    End Sub

#End Region

#Region "Server restart helper"
    ''' <summary>
    ''' Used for background restarts:
    ''' If the server is being restarted, ignore any subsequent requests to restart the server.
    ''' </summary>
    Dim RestartSrvThread As New System.Threading.Thread(
                  New System.Threading.ParameterizedThreadStart(
                      AddressOf DoBackgroundRestartServer)) With {
                          .IsBackground = True
                              }


    ''' <summary>
    ''' Restart the server. Waits for the server to stop, then start it again.
    ''' </summary>
    ''' <param name="background">Set True to restart in the background, i.e. non-blocking code.</param>
    Public Function RestartServer(background As Boolean)
        With MyServer
            If Not background Then
                Try
                    .StopServer()
                    .ServerProc.WaitForExit()
                    .StartServer()
                    Return True
                Catch ex As Exception
                    Return False
                End Try
            Else

                ' Only start a new restart job if there isn't an existing job
                If Not RestartSrvThread.IsAlive Then
                    ' Create a new thread to run the restart job in (i.e. a background thread)
                    RestartSrvThread = New System.Threading.Thread(
                      New System.Threading.ParameterizedThreadStart(
                          AddressOf DoBackgroundRestartServer)) With {
                              .IsBackground = True
                                  }
                    RestartSrvThread.Start()
                End If

                Return True

            End If
        End With
    End Function

    Private Sub DoBackgroundRestartServer()
        Dim workFunc = Function()
                           With MyServer
                               If .ServerIsOnline Then
                                   ' Stop server
                                   .CurrentServerState = ServerState.Stopping
                                   Try
                                       .StopServer()

                                       ' Wait for server exit
                                       .ServerProc.WaitForExit()

                                       Dim i As Integer = 0
                                       While .ServerIsOnline
                                           System.Threading.Thread.Sleep(1500)
                                           i += 1

                                           ' Two minute timeout
                                           ' XXX Causes problems if timeout is shorter than intervals between the "restart" scheduled task
                                           If i = 120 / 1.5 Then
                                               MessageBox.Show("An attempt to restart the server failed.")
                                               Return False
                                           End If
                                       End While

                                       ' Start server again
                                       .StartServer()
                                   Catch
                                   End Try
                               End If
                           End With
                           Return True
                       End Function

        workFunc()
    End Sub
#End Region

#Region "Data types"

    ''' <summary>
    ''' Is the server process running?
    ''' </summary>
    Public ReadOnly Property ServerIsOnline As Boolean
        Get
            If CurrentServerState = ServerState.NotRunning Then
                Return False
            Else
                Return True
            End If
        End Get
    End Property

    ''' <summary>
    ''' The current running state of the server
    ''' </summary>
    Public Property CurrentServerState() As ServerState
        Get
            Return _ServerState
        End Get
        Set(ByVal value As ServerState)
            _ServerState = value
            RaiseEvent ServerStateChanged(value)
            OnPropertyChanged("ServerIsOnline")
        End Set
    End Property

    Private _ServerState As ServerState = ServerState.NotRunning ' Encapsulated variable

#End Region

#Region "Stats monitor"

    Private serverprocess_RAMusage As ULong
    Private serverprocess_PreviousCPUUsage As CounterSample
    Private serverprocess_CurrentCPUUsage As CounterSample
    Private serverprocess_ComputedCPUUsage As Single
    Private serverProcess_CPUCounter As PerformanceCounter

    Public Shared Function GetTotalMemoryInBytes() As ULong
        ' (Total memory of PC used to calculate % of total used by server)
        Return New Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory
    End Function

    Public Sub Execute_UpdateServerStats_graph()
        ' Begin updating the RAM usage info
        MyMainWindow.Dispatcher.Invoke(New Action(AddressOf UpdateServerStats_graph))
    End Sub

    Private Sub UpdateServerStats_graph()
        ' Convert from bytes to MegaBytes
        Dim serverprocess_memory_usage As ULong = (serverprocess_RAMusage / (1024 * 1024))
        Dim total_machine_memory As ULong = (GetTotalMemoryInBytes() / (1024 * 1024))

        ' Get total_allocated_memory, as stored in startup parameters
        Dim total_allocated_memory As ULong = MyUserSettings.MaxMemoryInMB

        ' Calculate the server's memory usage against machines' total memory
        Dim ratio As Single = CSng(serverprocess_memory_usage) / CSng(total_machine_memory)
        navpageDashboard.progbarRAM.Value = ratio * 100
        navpageDashboard.txtRAM.Text = String.Format("{0:0.00}%", navpageDashboard.progbarRAM.Value)

        ' Calculate the usage of the server's own memory allocation
        Dim ratio2 As Single = CSng(serverprocess_memory_usage) / CSng(total_allocated_memory)
        navpageDashboard.progbarRAMalloc.Value = ratio2 * 100
        navpageDashboard.txtRAMAlloc.Text = String.Format("{0:0.00}MiB", serverprocess_memory_usage)

        navpageDashboard.progbarCPUUsage.Value = serverprocess_ComputedCPUUsage
        navpageDashboard.txtCPUUsage.Text = String.Format("{0:0.00}%", serverprocess_ComputedCPUUsage)
    End Sub

    Private Sub ProcessStatMonitor()
        On Error Resume Next

        ' Define variables to track the peak
        ' memory usage of the process.
        Dim peakPagedMem As Long = 0, peakWorkingSet As Long = 0, peakVirtualMem As Long = 0

        ' Display the process statistics until
        ' the user closes the program.
        ' Polls every 1 sec.
        Do
            If Not MyServer.ServerProc.HasExited Then
                If IsNothing(serverProcess_CPUCounter) Then
                    serverProcess_CPUCounter = GetPerfCounterForProcessId(MyServer.ServerProc.Id)
                    serverprocess_CurrentCPUUsage = serverProcess_CPUCounter.NextSample()
                    serverprocess_PreviousCPUUsage = serverprocess_CurrentCPUUsage ' Just to have a non Nothing value at start
                End If

                ' Refresh the current process property values.
                MyServer.ServerProc.Refresh()

                ' Display current process statistics.
                serverprocess_RAMusage = CULng(MyServer.ServerProc.WorkingSet64)
                Execute_UpdateServerStats_graph()

                ' Update the values for the overall peak memory statistics.
                peakPagedMem = MyServer.ServerProc.PeakPagedMemorySize64
                peakVirtualMem = MyServer.ServerProc.PeakVirtualMemorySize64
                peakWorkingSet = MyServer.ServerProc.PeakWorkingSet64

                'Update values for current CPU usage %
                serverprocess_PreviousCPUUsage = serverprocess_CurrentCPUUsage
                serverprocess_CurrentCPUUsage = serverProcess_CPUCounter.NextSample()
                serverprocess_ComputedCPUUsage = CounterSampleCalculator.ComputeCounterValue(serverprocess_PreviousCPUUsage, serverprocess_CurrentCPUUsage) / Environment.ProcessorCount
            End If
        Loop While (isExiting = False) And (Not MyServer.ServerProc.WaitForExit(1000))

        'On process exit, clear info & update UI
        serverprocess_RAMusage = 0
        serverprocess_ComputedCPUUsage = 0
        If Not IsNothing(serverProcess_CPUCounter) Then
            serverProcess_CPUCounter.Dispose()
            serverProcess_CPUCounter = Nothing
        End If

        Execute_UpdateServerStats_graph()
    End Sub

    Private Function GetPerfCounterForProcessId(processId As Integer, Optional processCounterName As String = "% Processor Time") As PerformanceCounter
        Dim instance As String
        instance = GetInstanceNameForProcessId(processId)
        If String.IsNullOrEmpty(instance) Then
            Return Nothing
        End If

        Return New PerformanceCounter("Process", processCounterName, instance)
    End Function

    Private Function GetInstanceNameForProcessId(processId As Integer) As String
        Dim process As Process
        Dim processName As String
        Dim cat As PerformanceCounterCategory
        Dim instances As String()

        process = Process.GetProcessById(processId)
        processName = IO.Path.GetFileNameWithoutExtension(process.ProcessName)
        cat = New PerformanceCounterCategory("Process")

        instances = (From inst In cat.GetInstanceNames() Where inst.StartsWith(processName)).ToArray()

        For Each instance In instances
            Using cnt As PerformanceCounter = New PerformanceCounter("Process", "ID Process", instance, True)
                Dim val As Integer = CInt(cnt.RawValue)
                If val = processId Then
                    Return instance
                End If
            End Using
        Next

        Return Nothing
    End Function

#End Region

End Class