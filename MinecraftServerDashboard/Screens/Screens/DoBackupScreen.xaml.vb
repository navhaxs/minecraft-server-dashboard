Imports System.IO
Public Class DoBackupScreen

    Sub New(HideLinkToBackupManager As Boolean)
        InitializeComponent()

        If HideLinkToBackupManager Then
            DoBackupScreen.Visibility = Windows.Visibility.Collapsed
        End If
    End Sub

    Private b As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)
    Private WithEvents backupUtil As New WorldDataBackupUtil

    Sub closeDialog() Handles btnCancel.Click
        Close()
    End Sub

    Private Sub Button_StartBackup_Click()
        If backupUtil.inWorldDirectory Is Nothing Then
            MessageBox.Show("No world has been created yet! Nothing found to backup")
            Exit Sub
        End If

        'Provide user feedback
        Me.IsBusy = True
        Me.FormControls.IsEnabled = False
        Me.Cursor = System.Windows.Input.Cursors.Wait

        backupUtil.outZipArchiveFile = MyServer.MyStartupParameters.ServerPath & "\world-backups\" & txtFilename.Text

        If My.Computer.FileSystem.FileExists(backupUtil.outZipArchiveFile & ".zip") Then
            Select Case MessageBox.Show("The file """ & txtFilename.Text & """ already exists. Overwrite?", "", MessageBoxButton.YesNoCancel)
                Case MessageBoxResult.Yes
                    'Continue
                Case MessageBoxResult.No
                    Dim newTargetZipPath As String = backupUtil.outZipArchiveFile
                    Try
                        ' Append a (2) to end of filename if exists
                        Dim i As Integer = 1
                        While My.Computer.FileSystem.FileExists(newTargetZipPath & ".zip")
                            i += 1
                            newTargetZipPath = backupUtil.outZipArchiveFile & " (" & i & ")"
                        End While
                        backupUtil.outZipArchiveFile = newTargetZipPath
                    Catch ex As Exception
                        MessageBox.Show("Could not write to file """ & txtFilename.Text & """! Try another filename.")
                        Exit Sub
                    End Try

                Case MessageBoxResult.Cancel
                    Exit Sub
            End Select
        End If

        backupUtil.outZipArchiveFile = backupUtil.outZipArchiveFile & ".zip"

        backupUtil.startBackup()

    End Sub

    Private Sub DoBackupScreen_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        If backupUtil.currentState = WorldDataBackupUtil.BackupStage.WaitingForServerResponse Then
            'Abort operation and close
            MyServer.SendCommand("save-on") ' Turn back on World saving
            MyMainWindow.MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.None

            MessageBox.Show("The world backup was aborted.")

        Else
            MyMainWindow.MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.None
            ' (Continue closing this dialog)
        End If
    End Sub

    Private Sub backupUtil_BackupCompleted(m As WorldDataBackupUtil.BackupResult) Handles backupUtil.BackupCompleted
        MyMainWindow.Dispatcher.BeginInvoke( _
                                           New Action(Sub()

                                                          Me.IsBusy = False
                                                          Me.Cursor = System.Windows.Input.Cursors.Arrow

                                                          Select Case m
                                                              Case WorldDataBackupUtil.BackupResult.SUCCESS
                                                                  MessageBox.Show("Saving world """ & backupUtil.outZipArchiveFile & """ is complete")
                                                              Case Else
                                                                  MessageBox.Show("Dashboard failed to save world """ & backupUtil.outZipArchiveFile, "Dashboard", MessageBoxButton.OK, MessageBoxImage.Error)
                                                          End Select

                                                          closeDialog()

                                                      End Sub))
    End Sub

#Region "UI"

    Private Sub NewWorldGenScreen_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        backupUtil.inWorldDirectory = b.ReturnConfigValue("level-name")
    End Sub

    Private Sub txtFilename_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtFilename.TextChanged
        ' Check for valid filename
        btnGo.IsEnabled = (txtFilename.Text.Length > 0) And validateFilename(txtFilename.Text)
    End Sub

    Public Shared Function validateFilename(ByVal fileName As String) As Boolean
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

#End Region

End Class