Class PageConsole
    Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.DataContext = MyServer
    End Sub

    ''' <summary>
    ''' On 'Enter' keydown, pass the entered command to the server process, and keep history of the past entered commands,
    ''' then clear the textbox.
    ''' </summary>
    Private Sub CommandTextBox_KeyDown(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Enter Then
            If CommandTextBox.Text IsNot Nothing OrElse CommandTextBox.Text <> "" Then

                'Get the last command in the history
                History_selectedIndex = hsCount - 1
                Dim lastvalue As String = getCommandHistory(CommandHistoryNavigate.Up)

                'Set the index counter back to the MAXIMUM (because the item is added to the end)
                History_selectedIndex = hsCount - 1 ' Zero-based index

                ' Don't add this value to the history if it was the same as the last one
                If Not lastvalue = CommandTextBox.Text Then
                    commandHistory.Add(CommandTextBox.Text)
                End If

                'Only send commands if the server is running...
                If MyServer.ServerIsOnline = True Then
                    MyServer.SendCommand(CommandTextBox.Text)

                    CommandTextBox.Text = "" ' Clear the textbox
                End If

                History_selectedIndex = hsCount - 1 'Reset counter back to MAX
            End If
            ConsoleTextBlock.ScrollToEnd()
        End If
    End Sub

    Private Sub ConsoleTextBlock_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs) Handles ConsoleTextBlock.TextChanged
        ConsoleTextBlock.ScrollToEnd()
    End Sub

#Region "Command history"
    Private commandHistory As New List(Of String)
    Private History_selectedIndex As Integer = -1
    Private playerCommands As New List(Of String)
    ReadOnly Property hsCount As Integer
        Get
            Return commandHistory.Count
        End Get
    End Property
    Public Enum CommandHistoryNavigate
        ''' <summary>
        ''' Cycle through the command history back one
        ''' </summary>
        Up = 1
        ''' <summary>
        ''' Cycle through the command history forward one
        ''' </summary>
        Down = 0
    End Enum

    ''' <summary>
    '''
    ''' </summary>
    Private Function getCommandHistory(UpOrDown As CommandHistoryNavigate) As String
        If hsCount = 0 Then ' If history is empty
            Return "" ' Don't do anything
        End If

        'Starts at 0

        If UpOrDown = CommandHistoryNavigate.Up Then
            ' Go upwards (backwards)
            History_selectedIndex -= 1

            If History_selectedIndex < 0 Then
                ' At the 'top' (start) of the list, cannot go backwards any further
                History_selectedIndex = 0
                Return commandHistory.ElementAt(0)
            Else
                Return commandHistory.ElementAt(History_selectedIndex + 1)
            End If

        ElseIf UpOrDown = CommandHistoryNavigate.Down Then
            ' Go downwards (forwards) in history list

            If History_selectedIndex + 2 > hsCount Then
                ' At the 'bottom' (end) of the list, cannot go forwards any further.
                Return ""
            Else
                History_selectedIndex += 1
                Return commandHistory.ElementAt(History_selectedIndex)
            End If
        End If
        Return ""
    End Function

    ''' <summary>
    ''' Allows the cycling through the command history in the textbox
    ''' </summary>
    Private Sub CommandTextBox_KeyUp(sender As Object, e As KeyEventArgs) Handles CommandTextBox.KeyUp
        Select Case e.Key
            Case Key.Up
                CommandTextBox.Text = getCommandHistory(CommandHistoryNavigate.Up)
                Exit Select
            Case Key.Down
                CommandTextBox.Text = getCommandHistory(CommandHistoryNavigate.Down)
                Exit Select
            Case Else
                Exit Select
        End Select
    End Sub

#End Region
End Class