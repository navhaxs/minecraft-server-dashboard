Class PagePlayers

    ' See 'ToggleIndexFor' annotations
    Private Sub LeftPlayerList_ComboBoxSelectionChanged(sender As PlayerLists, thisindex As Integer, thisvalue As String)
        RightPlayerList.ToggleIndexFor(thisindex)
        DoCheck()
    End Sub

    Private Sub RightPlayerList_ComboBoxSelectionChanged(sender As PlayerLists, thisindex As Integer, thisvalue As String)
        LeftPlayerList.ToggleIndexFor(thisindex)
        DoCheck()
    End Sub

    ''' <summary>
    ''' Update the state of the move left / right arrow buttons
    ''' </summary>
    Sub DoCheck()

        ' Don't allow the moving of items into the list if it is readonly
        btnMoveLeft.IsEnabled = Not LeftPlayerList.thislist.isReadOnly
        btnMoveRight.IsEnabled = Not RightPlayerList.thislist.isReadOnly

        ' Don't allow the copying of items from the following lists
        ' since IP addresses cannot be used in any other type of list (the rest are lists of usernames)
        If RightPlayerList.thislist.getPlayerlistType = PlayerListEditor.PlayerListType.None Or LeftPlayerList.thislist.getPlayerlistType = PlayerListEditor.PlayerListType.None Then
            btnMoveLeft.IsEnabled = False
            btnMoveRight.IsEnabled = False
        End If

        If RightPlayerList.thislist.getPlayerlistType = PlayerListEditor.PlayerListType.BannedIP Then
            btnMoveLeft.IsEnabled = False
        End If

        If LeftPlayerList.thislist.getPlayerlistType = PlayerListEditor.PlayerListType.BannedIP Then
            btnMoveRight.IsEnabled = False
        End If

    End Sub

    Private Sub btnMoveLeft_Click(sender As Object, e As RoutedEventArgs) Handles btnMoveLeft.Click
        LeftPlayerList.AddPlayerToThisList(RightPlayerList.PlayerListBox.SelectedItem)
        If Not My.Computer.Keyboard.ShiftKeyDown Then ' Only copy if SHIFT is held down, else MOVE
            RightPlayerList.RemovePlayerFromThisList(RightPlayerList.PlayerListBox.SelectedItem)
        End If

        CheckForEmptyList()
    End Sub

    Private Sub btnMoveRight_Click(sender As Object, e As RoutedEventArgs) Handles btnMoveRight.Click
        RightPlayerList.AddPlayerToThisList(LeftPlayerList.PlayerListBox.SelectedItem)
        If Not My.Computer.Keyboard.ShiftKeyDown Then ' Only copy if SHIFT is held down, else MOVE
            LeftPlayerList.RemovePlayerFromThisList(LeftPlayerList.PlayerListBox.SelectedItem)
        End If

        CheckForEmptyList()
    End Sub

    ' Update UI for empty lists (shows 'this list is empty' text)
    Sub CheckForEmptyList()
        If RightPlayerList.PlayerListBox.Items.Count = 0 Then
            RightPlayerList.EmptyMessage.Visibility = Windows.Visibility.Visible
        Else
            RightPlayerList.EmptyMessage.Visibility = Windows.Visibility.Collapsed
        End If
        If LeftPlayerList.PlayerListBox.Items.Count = 0 Then
            LeftPlayerList.EmptyMessage.Visibility = Windows.Visibility.Visible
        Else
            LeftPlayerList.EmptyMessage.Visibility = Windows.Visibility.Collapsed
        End If
    End Sub

    Sub RefreshBtn_Click() Handles Button1.Click
        ReloadPlayerLists()
    End Sub

    Sub ReloadPlayerLists(Optional ByVal delay As Boolean = False)
        ' Enable only if there are player lists to be edited
        ContentFrame.IsEnabled = My.Computer.FileSystem.DirectoryExists(MyServer.MyStartupParameters.ServerPath)

        ' Add a non-blocking delay in the case of waiting for the server process to write its' changes.
        If delay Then
            ' Intitialise new thread
            Dim RestartSrvThread As New System.Threading.Thread( _
             New System.Threading.ParameterizedThreadStart( _
                 AddressOf _DoReloadPlayerLists)) With { _
                     .IsBackground = True _
                         }
            RestartSrvThread.Start()
        Else
            LeftPlayerList.Reload()
            RightPlayerList.Reload()
            DoCheck()
        End If
    End Sub

    Private Sub _DoReloadPlayerLists()
        ' Give the server time to process the prior commands
        System.Threading.Thread.Sleep(1500)

        MyMainWindow.Dispatcher.BeginInvoke(New Action(Sub()
                                                           LeftPlayerList.Reload()
                                                           RightPlayerList.Reload()
                                                           DoCheck()
                                                       End Sub))
    End Sub

End Class