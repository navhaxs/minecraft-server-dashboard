''' Schedule tasks to be run at set intervals
''' e.g. do a backup, or send a command
'''
''' Attributions -
''' http://forums.devx.com/showthread.php?56988-how-to-pass-a-variable-to-a-timer-s-handler
Public Class TaskScheduler

    Private TaskList As New List(Of Task)

    Private _schedulerRunning As Boolean = False
    ReadOnly Property schedulerRunning As Boolean
        Get
            Return _schedulerRunning
        End Get
    End Property

    Public Enum TaskActionType As Integer
        sendCommand = 0
        doBackup = 1
        sayThis = 2
        restartServer = 3
    End Enum

    ''' <summary>
    ''' An automated job for Dashboard to execute
    ''' </summary>
    Class Task
        ''' <summary>
        ''' Task index
        ''' </summary>
        Property ID As Integer

        ''' <summary>
        ''' The enabled state of this task
        ''' </summary>
        Property Enabled As Boolean = True

        ''' <summary>
        ''' The action of this task
        ''' </summary>
        Property Action As TaskActionType = TaskActionType.doBackup

        ''' <summary>
        ''' The time interval of when to execute this task, in milliseconds
        ''' </summary>
        Property Interval As Double = 10 * 60000

        ''' <summary>
        ''' An array of commands to execute for sayThis or doCommand
        ''' </summary>
        Property Commands As String() = {""}
    End Class

    Public Function nextTaskID() As Integer
        Return TaskList.Count + 1
    End Function

    Public Sub startScheduler()
        Debug.Print("Starting scheduler loop")
        If (Not _schedulerRunning) Then
            ListOfTaskStartTimer = New List(Of TaskStartTimer)
            For Each task In TaskList
#If DEBUG Then
                Debug.Print("Added task ID #" & task.ID)
                AddHandler New TaskStartTimer(5000).Elapsed, AddressOf New TimerHandler(task).OnTimer
#Else
                AddHandler New TaskStartTimer(task.Interval).Elapsed, AddressOf New TimerHandler(task).OnTimer
#End If
            Next

            _schedulerRunning = True
        Else
            Debug.Print("XXX Scheduler loop already running")
        End If
    End Sub

    Public Sub stopScheduler()
        If schedulerRunning Then
            For Each i In ListOfTaskStartTimer
                i.Stop()
            Next
            _schedulerRunning = False
        End If
    End Sub

    Public Sub clearTasks()
        TaskList = New List(Of Task)
    End Sub

    Public Sub addTask(task As Task)
        TaskList.Add(task)
        Debug.Print(TaskList.Count)
    End Sub

    Public Sub removeTask(task As Task)
        TaskList.Remove(task)
    End Sub

    Public Function getTaskFromIndex(idToFind As Integer) As Task
        For Each i In TaskList
            If i.ID = idToFind Then
                Return i
            End If
        Next
        Return Nothing ' Null in vb.net
    End Function

    Public Class TimerHandler
        Private _thisTask As Task
        Sub New(ByVal thisTask As Task)
            _thisTask = thisTask
        End Sub

        Public Sub OnTimer(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)
            Debug.Print("task " & _thisTask.ID & " fired")
            Debug.Assert(MyServer.ServerIsOnline)

            CType(source, System.Timers.Timer).Enabled = False
            RemoveHandler CType(source, TaskStartTimer).Elapsed, AddressOf Me.OnTimer
            ' The task start timer is paused while we process the task events.

            Select Case _thisTask.Action
                Case TaskActionType.sendCommand
                    Debug.Assert(Not _thisTask.Commands Is Nothing)
                    For Each i In _thisTask.Commands
                        MyServer.SendCommand(i)
                    Next
                Case TaskActionType.sayThis
                    Debug.Assert(Not _thisTask.Commands Is Nothing)
                    For Each i In _thisTask.Commands
                        MyServer.SendCommand("say " & i)
                    Next
                Case TaskActionType.doBackup
                    Dim backupUtil As New WorldDataBackupUtil
                    Dim b As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)
                    backupUtil.inWorldDirectory = b.ReturnConfigValue("level-name")
                    backupUtil.outZipArchiveFile = MyServer.MyStartupParameters.ServerPath & "\world-backups\" & backupUtil.inWorldDirectory & "_" & DateTime.Now.ToString("yyyyMMddHHmmssfff") & ".zip"

                    backupUtil.startBackup()

                    ' Don't wait for backup completion
                Case TaskActionType.restartServer
                    MyServer.RestartServer(True)
            End Select

#If DEBUG Then
            AddHandler New TaskStartTimer(5000).Elapsed, AddressOf New TimerHandler(_thisTask).OnTimer

#Else
            AddHandler New TaskStartTimer(_thisTask.Interval).Elapsed, AddressOf New TimerHandler(_thisTask).OnTimer
#End If
        End Sub

    End Class

    Public Shared ListOfTaskStartTimer As New List(Of TaskStartTimer)

    Public Class TaskStartTimer : Inherits System.Timers.Timer
        Sub New(ByVal intervalMilliseconds As Integer)
            If intervalMilliseconds < 0 Then
                MessageBox.Show("Dashboard could not start one of your scheduled task jobs because its time interval was not set.")
                Exit Sub
            End If
            Me.Interval = intervalMilliseconds
            Me.Enabled = True
            ListOfTaskStartTimer.Add(Me)
        End Sub
    End Class


End Class
