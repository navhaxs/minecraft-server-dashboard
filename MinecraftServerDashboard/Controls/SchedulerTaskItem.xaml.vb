Public Class SchedulerTaskItem
    Event taskIsEnabledStateChanged(e As SchedulerTaskItem)
    Event taskRemoved(e As SchedulerTaskItem)

    Private Const SECOND_TO_MS As Integer = 60000

    ReadOnly Property Task As TaskScheduler.Task
        Get
            ' Convert the minutes to milliseconds
            If UITimeInterval.Text > 0 Then
                _Task.Interval = UITimeInterval.Text * SECOND_TO_MS
                Debug.Print("Interval (min): " & UITimeInterval.Text)
                Debug.Print("Interval (ms): " & _Task.Interval)
            End If

            If thisActionMode_ComboBox.SelectedItem Is doCommand Then
                _Task.Action = TaskScheduler.TaskActionType.sendCommand
                ' Copy the users' commands to automate to the Task object
                Dim input As String = doCommandParamtersText.Text
                Dim list As String() = input.Split(New [Char]() {ControlChars.Lf, ControlChars.Cr}, StringSplitOptions.RemoveEmptyEntries)
                _Task.Commands = list
                Debug.Print("doCommand")
            ElseIf thisActionMode_ComboBox.SelectedItem Is sayThis Then
                _Task.Action = TaskScheduler.TaskActionType.sayThis
                Dim input As String = sayThisParamtersText.Text
                Dim list As String() = input.Split(New [Char]() {ControlChars.Lf, ControlChars.Cr}, StringSplitOptions.RemoveEmptyEntries)
                _Task.Commands = list
                Debug.Print("sayThis")
            ElseIf thisActionMode_ComboBox.SelectedItem Is restartServer Then
                _Task.Action = TaskScheduler.TaskActionType.restartServer
                Debug.Print("restartServer")
            ElseIf thisActionMode_ComboBox.SelectedItem Is doBackup Then
                _Task.Action = TaskScheduler.TaskActionType.doBackup
                Debug.Print("doBackup")
            End If

            _Task.Enabled = chkboxIsTaskEnabled.IsChecked

            Return _Task
        End Get
    End Property

    Private _Task As New TaskScheduler.Task

    Sub New(taskToLoad As TaskScheduler.Task)

        ' This call is required by the designer.
        InitializeComponent()

        _Task = taskToLoad
        ' Load the task to the UI
        UITimeInterval.Text = taskToLoad.Interval / SECOND_TO_MS
        chkboxIsTaskEnabled.IsChecked = taskToLoad.Enabled
        Select Case taskToLoad.Action
            Case TaskScheduler.TaskActionType.doBackup
                thisActionMode_ComboBox.SelectedItem = doBackup
            Case TaskScheduler.TaskActionType.sendCommand
                thisActionMode_ComboBox.SelectedItem = doCommand
                If Not taskToLoad.Commands(0) = "" Then ' Clear the textbox if there is user-set data
                    doCommandParamtersText.Text = ""
                End If
                For Each line In taskToLoad.Commands
                    doCommandParamtersText.Text = doCommandParamtersText.Text & line & vbNewLine
                Next
            Case TaskScheduler.TaskActionType.sayThis
                thisActionMode_ComboBox.SelectedItem = sayThis
                If Not taskToLoad.Commands(0) = "" Then ' Clear the textbox if there is user-set data
                    sayThisParamtersText.Text = ""
                End If
                For Each line In taskToLoad.Commands
                    sayThisParamtersText.Text = sayThisParamtersText.Text & line & vbNewLine
                Next
        End Select
        ComboBox_SelectionChanged()
    End Sub

    Private Sub btnRemoveTask_Click(sender As Object, e As RoutedEventArgs)
        RaiseEvent taskRemoved(Me)
    End Sub

    Private Sub TimeIntervalComboBox_TextChanged(sender As Object, e As RoutedEventArgs)
        'data validation here
        

    End Sub

    Private Sub chkboxIsTaskEnabled_Click(sender As Object, e As RoutedEventArgs)
        RaiseEvent taskIsEnabledStateChanged(Me)
    End Sub

    Private Sub ComboBox_SelectionChanged()
        'WPF fires events even before all the controls have been initialised yet
        'http://stackoverflow.com/questions/2518231/wpf-getting-control-null-reference-during-initializecomponent
        If (sayThisParameters Is Nothing) Or (doCommandParamters Is Nothing) Then
            Exit Sub
        End If

        If thisActionMode_ComboBox.SelectedItem Is doCommand Then
            UImoreParameters.Visibility = Windows.Visibility.Visible
            doCommandParamters.IsSelected = True
        ElseIf thisActionMode_ComboBox.SelectedItem Is sayThis Then
            UImoreParameters.Visibility = Windows.Visibility.Visible
            sayThisParameters.IsSelected = True
        Else
            UImoreParameters.Visibility = Windows.Visibility.Hidden
        End If
    End Sub

End Class
