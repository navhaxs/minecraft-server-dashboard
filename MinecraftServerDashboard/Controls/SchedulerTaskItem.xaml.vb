Public Class SchedulerTaskItem
    Event taskIsEnabledStateChanged(e As SchedulerTaskItem)
    Event taskRemoved(e As SchedulerTaskItem)

    ReadOnly Property Task As TaskScheduler.Task
        Get
            ' Convert the minutes to milliseconds
            If UITimeInterval.Text > 0 Then
                _Task.Interval = UITimeInterval.Text * 60000
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
                _Task.Action = TaskScheduler.TaskActionType.sendCommand
                Dim input As String = sayThisParamtersText.Text
                Dim list As String() = input.Split(New [Char]() {ControlChars.Lf, ControlChars.Cr}, StringSplitOptions.RemoveEmptyEntries)
                _Task.Commands = list
                For Each i In _Task.Commands
                    i = "say " & i
                Next
                Debug.Print("doCommand (say)")
            ElseIf thisActionMode_ComboBox.SelectedItem Is doBackup Then
                _Task.Action = TaskScheduler.TaskActionType.doBackup
                Debug.Print("doBackup")
            End If

            Return _Task
        End Get
    End Property

    Private _Task As New TaskScheduler.Task

    Sub New(taskID As Integer)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _Task.ID = taskID
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

    Private Sub ComboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
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
