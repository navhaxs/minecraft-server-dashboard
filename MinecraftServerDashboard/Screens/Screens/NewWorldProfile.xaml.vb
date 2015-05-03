Imports System.IO
Public Class NewWorldProfile

    Sub DoClose() Handles btnCancel.Click
        Close() ' Simply close the dialog if there are no operations in progress
    End Sub

    Private Sub GoBtn_Click()
        Try
            If My.Computer.FileSystem.DirectoryExists(MyServer.MyStartupParameters.ServerPath & "\" & txtFilename.Text) Then
                Dim n As New MessageWindow(MyMainWindow, "", "A directory of the same name already exists. Pick another name.", "Already exists", "large")
            Else
                My.Computer.FileSystem.CreateDirectory(MyServer.MyStartupParameters.ServerPath & "\" & txtFilename.Text)
                Close()
            End If
        Catch ex As Exception
            MessageBox.Show("Could not create new world profile:" & vbNewLine & ex.Message)
        End Try
    End Sub

    Private Sub NewWorldProfile_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        navpageWorld.RefreshPageData()
    End Sub

    Private Sub NewWorldProfile_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        txtFilename.Focus()
    End Sub

    Private Sub txtFilename_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtFilename.TextChanged
        ' Must have filename entered before proceeding
        btnGo.IsEnabled = txtFilename.Text.Length > 0
    End Sub
End Class