Public Class ConfigMyWorldBackups

    Dim MyBackupFiles As New List(Of BackupFileArchive)
    Class BackupFileArchive

        Sub New(thisFileName As String)
            FileName = thisFileName
        End Sub

        Private _FileName As String 'Encapsulated varibles begin with an underscore to differentiate with public properties
        Private _DisplayName As String
        Public Property FileName As String
            Get
                Return _FileName
            End Get
            Set(value As String)
                _FileName = value

                ' Strip off the file extension from the display name
                Dim s As String = My.Computer.FileSystem.GetFileInfo(value).Name
                If s.ToLower.EndsWith(".zip") Then
                    s = s.Substring(0, s.Length - 4)
                End If
                s = s + " (" + My.Computer.FileSystem.GetFileInfo(value).CreationTime + ")"

                _DisplayName = s
            End Set
        End Property

        Public ReadOnly Property DisplayNameWithDate As String
            Get
                Return _DisplayName
            End Get
        End Property

        Public ReadOnly Property DisplayNameOnly As String
            Get
                Dim s As String = My.Computer.FileSystem.GetFileInfo(_FileName).Name
                If s.ToLower.EndsWith(".zip") Then
                    s = s.Substring(0, s.Length - 4)
                End If
                Return s
            End Get
        End Property

        Public ReadOnly Property DisplayDate As String
            Get
                Return My.Computer.FileSystem.GetFileInfo(_FileName).CreationTime
            End Get
        End Property

    End Class

    Sub New(m As SuperOverlay)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        superoverlay = m
    End Sub

    Dim superoverlay As SuperOverlay
    Public Sub isClosing()
        superoverlay.Confirm_DoClose(Me)
    End Sub

    ' Reload the *.ZIP world backup files
    Sub RefreshPageData()
        MyBackupFiles.Clear()
        If My.Computer.FileSystem.DirectoryExists(MyServer.MyStartupParameters.ServerPath & "\world-backups\") Then
            For Each i In My.Computer.FileSystem.GetFiles(MyServer.MyStartupParameters.ServerPath & "\world-backups\")
                If i.ToLower.EndsWith(".zip") Then
                    ' Maintains a list of backup files
                    MyBackupFiles.Add(New BackupFileArchive(i))
                End If
            Next
            ListBox_MyWorldBackups.IsEnabled = True
        Else
            ListBox_MyWorldBackups.IsEnabled = False
        End If

        ' Display list of backup files in UI
        ListBox_MyWorldBackups.Items.Clear()
        For Each i As BackupFileArchive In MyBackupFiles
            Dim s() As String = {i.DisplayNameOnly, i.DisplayDate}

            Dim m As New ListViewItem()
            m.Content = s

            ListBox_MyWorldBackups.Items.Add(m)
        Next

        ' Refresh UI
        ListBox_MyWorldBackups_SelectionChanged()
    End Sub

    ' Begin backup restore
    Private Sub Button_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Warn user if world to restore is server is online.
        If MyServer.ServerIsOnline Then
            If Not MessageBox.Show("WARNING! This server is currently running. If you are restoring a world that is currently in use, it is recommended that you stop the server first. Continue?", "Restore World Backup", MessageBoxButton.YesNoCancel) = MessageBoxResult.Yes Then
                Exit Sub
            End If
        End If

        Dim s As BackupFileArchive = MyBackupFiles(ListBox_MyWorldBackups.SelectedIndex)

        Dim b As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)

        ' Warn user if world to restore is currently active.
        If MyServer.ServerIsOnline And (s.DisplayNameOnly = b.ReturnConfigValue("level-name")) Then
            Dim dlg As New MessagePrompt
            With MyMainWindow
                .FormControls.Children.Add(dlg)

                .MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.StandardDialog
                .OverlayOpened()
            End With
        Else
            ' Final warning
            If MessageBox.Show("WARNING! This will revert the world to the backup. All changes since the backup will be permanently earased. Continue?", "Restore World Backup", MessageBoxButton.YesNoCancel) = MessageBoxResult.Yes Then
                ' Overwrite any existing data, unzips the backup file
                MyZipLib.Extract(s.FileName, MyServer.MyStartupParameters.ServerPath)
            End If
        End If
    End Sub

#Region "UI Events"

    Private Sub GenerateNewWorld_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim M As New NewWorldGenScreen
        With MyMainWindow
            .FormControls.Children.Add(M)
            .OverlayOpened()
            .MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.StandardDialog
        End With
    End Sub

    Public Sub DoBackupScreen()
        Using e As New DoBackupScreen(True)
            Dim M As New OverlayDialog
            M.DisplayConfig(e)
            e.txtFilename.Focus()
        End Using

        RefreshPageData()
    End Sub

    Private Sub btnDelete_Click_1(sender As Object, e As RoutedEventArgs)
        ' Enumerate all items to delete
        Dim to_delete_list As New List(Of Integer)
        For Each thisListItem As Object In ListBox_MyWorldBackups.SelectedItems
            If TypeOf thisListItem Is ListBoxItem Then
                to_delete_list.Add(ListBox_MyWorldBackups.Items.IndexOf(thisListItem))
            End If
        Next

        ' Delete each selected file
        For Each i In to_delete_list
            Try
                My.Computer.FileSystem.DeleteFile(MyBackupFiles(i).FileName, FileIO.UIOption.AllDialogs, FileIO.RecycleOption.SendToRecycleBin)
                ListBox_MyWorldBackups.Items.RemoveAt(i)
                MyBackupFiles.RemoveAt(i)
            Catch ex As Exception
                ' The delete operation was cancelled by the user from Windows Explorer
            End Try
        Next
    End Sub

    Private Sub ListBox_MyWorldBackups_SelectionChanged() Handles ListBox_MyWorldBackups.SelectionChanged
        ' Show buttons only when item(s) selected
        gridSelectedButtons.IsEnabled = Not CBool(ListBox_MyWorldBackups.SelectedIndex = -1)
    End Sub

    Sub btnOpenBackups_Click() Handles btnOpenBackups.Click
        navPageCBconfig.Go_OpenBackups_inWindowsExplorer()
    End Sub

    Private Sub Button_Click_2(sender As Object, e As RoutedEventArgs)
        DoBackupScreen()
    End Sub

#End Region
End Class