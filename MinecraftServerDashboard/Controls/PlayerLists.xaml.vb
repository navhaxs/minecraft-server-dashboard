' This contol is used twice in the Players tab - both the left and right player lists
Public Class PlayerLists

    Public thislist As New PlayerListEditor()
    Dim thisfilename As String

    Event ComboBoxSelectionChanged(sender As PlayerLists, thisindex As Integer, thisvalue As String)

    ''' <summary>
    ''' Disable the selection of a particlar entry (where it has been selected in the other player list already)
    ''' Also enables the rest of the items if previously disabled.
    ''' </summary>
    Public Sub ToggleIndexFor(thisindex As Integer)

        For Each i As ComboBoxItem In ComboBox1.Items
            i.IsEnabled = True
        Next
        ComboBox1.Items(thisindex).IsEnabled = False
    End Sub

    ''' <summary>
    ''' Returns currently selected player list
    ''' </summary>
    Public Property ComboBoxSelection As String
        Get
            If ComboBox1.SelectedValue Is Nothing Then
                Return ""
            Else
                Return ComboBox1.SelectedItem.Content
            End If
        End Get
        Set(value As String)
            ComboBox1.SelectedValue = value
        End Set
    End Property

    ''' <summary>
    ''' Enumerate all PlayerItems in the list, then remove them
    ''' </summary>
    Sub UI_ClearAll_Items(a As ListBox)

        Dim m As New List(Of String)
        For Each thisItem As Object In a.Items
            m.Add(thisItem)
        Next

        For Each thisItem In m
            a.Items.Remove(thisItem)
        Next
    End Sub

    ''' <summary>
    ''' Refresh the player list
    ''' </summary>
    Public Sub Reload()
        ' Firstly reset UI
        UI_ClearAll_Items(PlayerListBox)
        isNoSelectionMessage.Visibility = Windows.Visibility.Collapsed
        EmptyMessage.Visibility = Windows.Visibility.Collapsed
        bannerRemindWhitelistIsDisabled.Visibility = Windows.Visibility.Collapsed
        Grid_canModify.IsEnabled = False
        EmptyMessage.Text = "This list is empty."

        ' If no list selected, stop.
        If ComboBox1.SelectedIndex = -1 Then
            isNoSelectionMessage.Visibility = Windows.Visibility.Visible
            Exit Sub
        End If

        ' Check to see which text file to load , depending on what list has been selected
        Select Case CType(ComboBox1.SelectedValue, ListBoxItem).Content
            Case "Online Players"
                ' Get the online players directly from the Dashboard tab's 'online players' control
                If Not MyServer.ServerIsOnline Then
                    thislist.thisPlayerList = PlayerListEditor.PlayerListType.OnlinePlayers
                    EmptyMessage.Visibility = Windows.Visibility.Visible
                    EmptyMessage.Text = "The server is offline."
                    Exit Sub
                Else
                    For Each i As Object In navpageDashboard.MyOnlinePlayerList.StackPanel1.Children
                        If TypeOf i Is PlayerTile Then
                            PlayerListBox.Items.Add(CType(i, PlayerTile).username)
                        End If
                    Next

                End If
                Grid_canModify.IsEnabled = False
            Case "Banned IP's"

                thisfilename = MyServer.MyStartupParameters.ServerPath & "\banned-ips.txt"

                For Each i In thislist.GetPlayersFrom(thisfilename)
                    PlayerListBox.Items.Add(i.Name)
                Next
                Grid_canModify.IsEnabled = True
            Case "Banned Players"

                thisfilename = MyServer.MyStartupParameters.ServerPath & "\banned-players.txt"
                For Each i In thislist.GetPlayersFrom(thisfilename)
                    PlayerListBox.Items.Add(i.Name)
                Next
                Grid_canModify.IsEnabled = True
            Case "Ops"

                thisfilename = MyServer.MyStartupParameters.ServerPath & "\ops.txt"
                For Each i In thislist.GetPlayersFrom(thisfilename)
                    PlayerListBox.Items.Add(i.Name)
                Next
                Grid_canModify.IsEnabled = True

            Case "Whitelist"
                thisfilename = MyServer.MyStartupParameters.ServerPath & "\white-list.txt"

                For Each i In thislist.GetPlayersFrom(thisfilename)
                    PlayerListBox.Items.Add(i.Name)
                Next
                Grid_canModify.IsEnabled = True

                ' Show a warning banner if the current list is 'white-list', where 'white-list' mode
                ' is switched off in the game server settings.
                Dim b As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)
                Dim isWhitelistEnabledConf As String = b.ReturnConfigValue("white-list")

                If isWhitelistEnabledConf Is Nothing Then
                    ' If server.properties does not exist yet, assume default state
                    ' (the first server start will initialize all default values)
                    bannerRemindWhitelistIsDisabled.Visibility = Windows.Visibility.Collapsed
                ElseIf isWhitelistEnabledConf.ToString.ToLower = "true" Then
                    bannerRemindWhitelistIsDisabled.Visibility = Windows.Visibility.Collapsed
                Else
                    bannerRemindWhitelistIsDisabled.Visibility = Windows.Visibility.Visible
                End If

            Case Nothing
                thislist.thisPlayerList = PlayerListEditor.PlayerListType.None
                isNoSelectionMessage.Visibility = Windows.Visibility.Visible
                Exit Sub
        End Select

        If PlayerListBox.Items.Count = 0 Then
            EmptyMessage.Visibility = Windows.Visibility.Visible
        End If
    End Sub

    Public Sub AddPlayerToThisList(l As String)
        ' Don't add the new entry if it already exists to prevent duplicates
        If Not PlayerListBox.Items.Contains(l) Then
            If thislist.AddPlayer(l) Then
                PlayerListBox.Items.Add(l)
            End If
        End If
        ' There must be at least one item in the list by now...
        EmptyMessage.Visibility = Windows.Visibility.Collapsed
    End Sub

    Public Sub RemovePlayerFromThisList(l As String)
        If thislist.RemovePlayer(l) Then
            PlayerListBox.Items.Remove(l)
        End If
    End Sub

    ''' <summary>
    ''' Enables the white-list mode in the game server settings
    ''' </summary>
    Private Sub EnableWhitelist_Click(sender As Object, e As RoutedEventArgs)
        bannerRemindWhitelistIsDisabled.Visibility = Windows.Visibility.Collapsed
        Dim b As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)
        b.WriteConfigLine("white-list", "true")
    End Sub

#Region "UI"

    Private Sub AddPlayer_Click(sender As Object, e As RoutedEventArgs)
        Dim x As New OverlayDialog
        With MyMainWindow
            .FormControls.Children.Add(x)
            .OverlayOpened()
            .MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.StandardDialog
        End With

        With New AddPlayerToList
            .Owner = MyMainWindow
            .WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner
            Dim l As String = ""
            Select Case CType(ComboBox1.SelectedValue, ListBoxItem).Content
                Case "Banned IP's"
                    l = .DoAddPlayerToList("Enter the IP Address you wish to block from joining your server", "IP Address:", False, True)
                Case "Banned Players"
                    l = .DoAddPlayerToList("Enter the username of the player you wish to block from joining your server", "Username:", True, False)
                Case "Ops"
                    l = .DoAddPlayerToList("Enter the username of the player you wish to give 'Operator' privileges on server", "Username:", True, False)
                Case "Whitelist"
                    l = .DoAddPlayerToList("Enter the username of the player you wish allow to join your server", "Username:", True, False)
                Case Nothing
                    isNoSelectionMessage.Visibility = Windows.Visibility.Visible
                    Exit Sub
            End Select
            If Not l Is Nothing Then
                AddPlayerToThisList(l)
            End If
        End With
        With MyMainWindow
            .FormControls.Children.Remove(x)
            .MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.None
            .OverlayClosed()
        End With
    End Sub
    Private Sub RemovePlayer_Click(sender As Object, e As RoutedEventArgs)
        Dim x As New List(Of Object)
        For Each i In PlayerListBox.SelectedItems
            x.Add(i)
        Next

        For Each i In x
            RemovePlayerFromThisList(i)
        Next
    End Sub
    Private Sub ComboBox1_onSelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles ComboBox1.SelectionChanged
        Reload()
        RaiseEvent ComboBoxSelectionChanged(Me, ComboBox1.SelectedIndex, ComboBox1.SelectedValue.ToString)
    End Sub
#End Region

End Class