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
    End Enum

    Structure Task
        Property ID As Integer

        Property Action As TaskActionType
        Property Interval As Integer

        Property Commands As String()
    End Structure

    Public Function nextTaskID() As Integer
        Return TaskList.Count + 1
    End Function

    Public Sub startScheduler()
        Debug.Print("Starting scheduler")
        If (Not _schedulerRunning) Then
            Debug.Print("...ok (not already running)")
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
            Debug.Print("task fired")
            Debug.Assert(MyServer.ServerIsOnline) ' No more mum approach (COMP1917)

            CType(source, System.Timers.Timer).Enabled = False
            RemoveHandler CType(source, TaskStartTimer).Elapsed, AddressOf Me.OnTimer
            ' The task start timer is paused while we process the task events.

            Select Case _thisTask.Action
                Case TaskActionType.sendCommand
                    Debug.Assert(Not _thisTask.Commands Is Nothing)
                    For Each i In _thisTask.Commands
                        MyServer.SendCommand(i)
                    Next
                Case TaskActionType.doBackup
                    'TODO
            End Select

#If DEBUG Then
            AddHandler New TaskStartTimer(5000).Elapsed, AddressOf New TimerHandler(_thisTask).OnTimer

#Else
            AddHandler New TaskStartTimer(_thisTask.Interval).Elapsed, AddressOf New TimerHandler(_thisTask).OnTimer
#End If
            Debug.Print("task fired")
        End Sub

    End Class

    Public Shared ListOfTaskStartTimer As New List(Of TaskStartTimer)

    Public Class TaskStartTimer : Inherits System.Timers.Timer
        Sub New(ByVal intervalMilliseconds As Integer)
            Me.Interval = intervalMilliseconds
            Me.Enabled = True
            ListOfTaskStartTimer.Add(Me)
        End Sub
    End Class


End Class
