Class WorldDataBackupUtil

    Enum BackupStage As Integer
        Idle = -1
        WaitingForServerResponse = 0
        NowArchiving = 1
    End Enum

    Enum BackupResult As Integer
        FAILURE_UNKOWN = -1
        FAILURE_NODATAFOUND = 0
        SUCCESS = 1
    End Enum

    Event BackupStageChanged(m As BackupStage)
    Event BackupCompleted(m As BackupResult)

    ''' <summary>
    ''' The directory name of the world to backup
    ''' </summary>
    Public inWorldDirectory As String

    ''' <summary>
    ''' The filename of the backup to save, including the zip file extension
    ''' </summary>
    Public outZipArchiveFile As String

    Private _currentState As BackupStage = BackupStage.Idle
    Public ReadOnly Property currentState As BackupStage
        Get
            Return _currentState
        End Get
    End Property

    Public Sub startBackup()

        ' Create backup directoy if not already exists
        If Not My.Computer.FileSystem.DirectoryExists(MyServer.MyStartupParameters.ServerPath & "\world-backups\") Then
            My.Computer.FileSystem.CreateDirectory(MyServer.MyStartupParameters.ServerPath & "\world-backups\")
        End If

        If MyServer.ServerIsOnline Then
            'Ensure that the server is NOT still writing to the files to be backed up (for a consistent backup)

            ' Ask the server to write saves...
            MyServer.SendCommand("save-all")
            ' ...and turn off saving until backup is completed.
            MyServer.SendCommand("save-off")

            ' Wait for "saved world" response before continuing
            MyServer.isWaitingForWorldSavedActivity = True
            AddHandler MyServer.Detected_WorldSavedCompleted, AddressOf doBackup

            _currentState = BackupStage.WaitingForServerResponse
            RaiseEvent BackupStageChanged(currentState)
        Else
            ' Don't block code execution (do work in separate thread)
            MyMainWindow.Dispatcher.BeginInvoke( _
                                   New Action(Sub()
                                                  doBackup()
                                              End Sub))
        End If
    End Sub

    Private Sub doBackup()
        If MyServer.ServerIsOnline Then
            RemoveHandler MyServer.Detected_WorldSavedCompleted, AddressOf doBackup
        End If

        _currentState = BackupStage.NowArchiving
        RaiseEvent BackupStageChanged(currentState)

        Compress(inWorldDirectory, My.Computer.FileSystem.GetFileInfo(outZipArchiveFile).Name, outZipArchiveFile)

        If MyServer.ServerIsOnline Then
            MyServer.SendCommand("save-on") ' Ask the server to turn saving back on
        End If

        _currentState = BackupStage.Idle
        RaiseEvent BackupCompleted(BackupResult.SUCCESS)
    End Sub

End Class
