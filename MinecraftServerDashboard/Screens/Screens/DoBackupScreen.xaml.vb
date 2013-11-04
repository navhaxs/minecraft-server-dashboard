Imports System.IO
Public Class DoBackupScreen

    Sub New(HideLinkToBackupManager As Boolean)
        InitializeComponent()

        If HideLinkToBackupManager Then
            DoBackupScreen.Visibility = Windows.Visibility.Collapsed
        End If

    End Sub

    Dim b As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)
    Dim TargetZipPath As String
    Dim thislevelname As String

    Sub DoClose() Handles btnCancel.Click
        Close() ' Simply close the dialog if there are no operations in progress
    End Sub

    Dim isWaitingForResonse As Boolean = False
    Private Sub Button_Click_1()

        If thislevelname Is Nothing Then
            MessageBox.Show("No world has been created yet! Nothing found to backup")
            Exit Sub
        End If

        ' Create backup path if not already exists
        If Not My.Computer.FileSystem.DirectoryExists(MyServer.MyStartupParameters.ServerPath & "\world-backups\") Then
            My.Computer.FileSystem.CreateDirectory(MyServer.MyStartupParameters.ServerPath & "\world-backups\")
        End If

        TargetZipPath = MyServer.MyStartupParameters.ServerPath & "\world-backups\" & txtFilename.Text

        If My.Computer.FileSystem.FileExists(TargetZipPath & ".zip") Then
            Select Case MessageBox.Show("The file """ & txtFilename.Text & """ already exists. Overwrite?", "", MessageBoxButton.YesNoCancel)
                Case MessageBoxResult.Yes
                    'Continue
                Case MessageBoxResult.No
                    Dim NewTargetZipPath As String = TargetZipPath
                    Try
                        ' Append a (2) to end of filename if exists
                        Dim i As Integer = 1
                        While My.Computer.FileSystem.FileExists(NewTargetZipPath & ".zip")
                            i += 1
                            NewTargetZipPath = TargetZipPath & " (" & i & ")"
                        End While
                        TargetZipPath = NewTargetZipPath
                    Catch ex As Exception
                        MessageBox.Show("Could not write to file """ & txtFilename.Text & """! Try another filename.")
                        Exit Sub
                    End Try

                Case MessageBoxResult.Cancel
                    Exit Sub
            End Select
        End If

        TargetZipPath = TargetZipPath & ".zip"

        'Provide user feedback
        Me.IsBusy = True
        Me.FormControls.IsEnabled = False
        Me.Cursor = System.Windows.Input.Cursors.Wait

        If MyServer.ServerIsOnline Then
            'Ensure that the server is NOT still writing to the files to be backed up (for a consistent backup)
            'Make all saves...
            MyServer.SendCommand("save-all")
            '...and right after, turn of saving until backup is completed...
            MyServer.SendCommand("save-off")

            ' DELAY BACKUP
            'Wait for "saved world" response before continuing
            isWaitingForResonse = True ' Don't allow dialog to close whilst waiting
            MyServer.isWaitingForWorldSavedActivity = True
            AddHandler MyServer.Detected_WorldSavedCompleted, AddressOf CompleteWorldBckup
        Else

            ' DO BACKUP NOW
            CompleteWorldBckup()
        End If
    End Sub

    Private Sub DoBackupScreen_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        If isWaitingForResonse Then
            If MessageBox.Show("Abort world backup?", "World Backup", MessageBoxButton.YesNo) = MessageBoxResult.Yes Then
                'Abort operation and close
                MyServer.SendCommand("save-on") ' Turn back on World saving
                MyMainWindow.MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.None
                'Continue closing this dialog
            Else
                e.Cancel = True
            End If
        Else
            MyMainWindow.MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.None
            'Continue closing this dialog
        End If
    End Sub

    Sub CompleteWorldBckup()
        MyMainWindow.Dispatcher.BeginInvoke( _
                                            New Action(Sub()
                                                           isWaitingForResonse = False
                                                           If MyServer.ServerIsOnline Then
                                                               RemoveHandler MyServer.Detected_WorldSavedCompleted, AddressOf CompleteWorldBckup
                                                           End If

                                                           Compress(thislevelname, My.Computer.FileSystem.GetFileInfo(TargetZipPath).Name, TargetZipPath)

                                                           If MyServer.ServerIsOnline Then
                                                               MyServer.SendCommand("save-on") ' Turn back on World saving
                                                           End If

                                                           Me.IsBusy = False
                                                           Me.Cursor = System.Windows.Input.Cursors.Arrow

                                                           MessageBox.Show("Saving world """ & TargetZipPath & """ is complete")

                                                           ' Close this dialog box once completed
                                                           DoClose()

                                                       End Sub))
    End Sub

    Private Sub NewWorldGenScreen_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        thislevelname = b.ReturnConfigValue("level-name")
    End Sub

    Private Sub txtFilename_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtFilename.TextChanged
        ' Check for valid filename
        btnGo.IsEnabled = (txtFilename.Text.Length > 0) And FilenameIsOK(txtFilename.Text)
    End Sub

    Public Shared Function FilenameIsOK(ByVal fileName As String) As Boolean
        'List of invalid characters, taken from
        'http://msdn.microsoft.com/en-us/library/windows/desktop/aa365247%28v=vs.85%29.aspx
        '       < (less than)
        '       > (greater than)
        '       : (colon)
        '       " (double quote)
        '       / (forward slash)
        '       \ (backslash)
        '       | (vertical bar or pipe)
        '       ? (question mark)
        '       * (asterisk)
        Dim m() As String = {"<", ">", ":", """", "/", "\", "|", "?", "*"}
        For Each i In m
            If fileName.Contains(i) Then
                Return False
            End If
        Next
        Return True
    End Function

    Private Sub btnOpenBackups_Click(sender As Object, e As RoutedEventArgs) Handles btnOpenBackups.Click
        Close()
        MyMainWindow.MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.StandardDialog
        navpageWorld.btnOpenBackups_Click()
    End Sub
End Class