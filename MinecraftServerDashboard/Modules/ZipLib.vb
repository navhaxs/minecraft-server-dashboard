Imports Ionic.Zip

Module MyZipLib

    'Using DotNetZip library http://DotNetZip.codeplex.com
    Sub Extract(ZipToUnpack As String, TargetDir As String)
        Try
            Using zip1 As ZipFile = ZipFile.Read(ZipToUnpack)
                Dim e As ZipEntry
                ' Extract every entry for this ZIP file
                Dim ThisRoot As String = "*" ' Delete directories to overwrite, where necessary
                For Each e In zip1
                    Debug.Print(TargetDir & "\" & e.FileName)
                    If e.IsDirectory And (Not e.FileName.Contains(ThisRoot)) Then ' Only delete root directories, as subdirectories would have been cleared already.
                        If My.Computer.FileSystem.DirectoryExists(TargetDir & "\" & e.FileName) Then
                            My.Computer.FileSystem.DeleteDirectory(TargetDir & "\" & e.FileName, FileIO.DeleteDirectoryOption.DeleteAllContents)
                            ThisRoot = e.FileName
                        End If
                    End If
                    e.Extract(TargetDir, ExtractExistingFileAction.OverwriteSilently)
                Next
                MessageBox.Show("The world has been restored from the backup!")
            End Using
        Catch ex As Exception
            MessageBox.Show("Failed to restore backup!" & vbNewLine & ex.Message)
        End Try
    End Sub

    Dim myprogresswnd As MyZipProgress
    Sub Compress(DirToPack As String, TargetZip As String, TargetZipPath As String)
        Using m As New ZipFile()
            With m
                .ParallelDeflateThreshold = -1

                .AddDirectory(DirToPack, DirToPack) ' Zip the directory in a folder within the ZIP archieve named "DirToPack"
                If My.Computer.FileSystem.DirectoryExists(DirToPack & "_nether") Then
                    .AddDirectory(DirToPack & "_nether", DirToPack & "_nether")
                End If
                If My.Computer.FileSystem.DirectoryExists(DirToPack & "_the_end") Then
                    .AddDirectory(DirToPack & "_the_end", DirToPack & "_the_end")
                End If
                
                .Save(TargetZipPath)
            End With
        End Using

        SetAttr(TargetZipPath, FileAttribute.Archive + FileAttribute.ReadOnly)
    End Sub

    Private Sub MyCompressProgress(sender As Object, e As SaveProgressEventArgs)
        ' Display simple progress window
        myprogresswnd.Text = "Backing up... (" & e.EntriesSaved & "/" & e.EntriesTotal & ")"
        If e.EventType = ZipProgressEventType.Saving_Completed Then
            myprogresswnd.Close()
        End If
    End Sub

End Module