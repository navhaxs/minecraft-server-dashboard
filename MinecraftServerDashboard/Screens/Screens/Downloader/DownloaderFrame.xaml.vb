Public Class DownloaderFrame

    Function isSuccessful() As Boolean
        Me.ShowDialog()
        Return result
    End Function

    Dim result As Boolean = False

    Dim WithEvents DownloaderFrame_UpdaterEngine As New JarDownloadEngine
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        tabcontrol.SelectedIndex += 1

        Select Case btnVanilla.IsChecked
            Case True ' Official Mojang Server (Vanilla)
                lblStatus.Text = "Now fetching the latest server files from the internet..."
            Case False 'CraftBukkit
                lblStatus.Text = "Now fetching the CraftBukkit server files from the internet..."
        End Select

        latestverfetchservice.RunWorkerAsync(btnVanilla.IsChecked)
    End Sub

    Dim WithEvents latestverfetchservice As New System.ComponentModel.BackgroundWorker

    Private Sub WebService_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles latestverfetchservice.DoWork
        If Not My.Computer.Network.IsAvailable Then
            Dim n As New MessageWindow(MyMainWindow, "", "Connect to the internet first and try again.", "Offline =(")
        Else
            Select Case e.Argument
                Case True ' Official Mojang Server (Vanilla)

                    Dim Ver As String = DownloaderFrame_UpdaterEngine.GetLatestVanillaVersion
                    Dim url As String = "https://s3.amazonaws.com/Minecraft.Download/versions/" + Ver + "/minecraft_server." + Ver + ".jar"

                    DownloaderFrame_UpdaterEngine.StartJarDownload(System.Environment.CurrentDirectory, thisProgressBar, url)
                Case False 'CraftBukkit

                    DownloaderFrame_UpdaterEngine.StartJarDownload(System.Environment.CurrentDirectory, thisProgressBar, "http://cbukk.it/craftbukkit.jar")
            End Select
        End If
    End Sub

    Private Sub myUpdaterEngine_DownloadCompleted(e As ComponentModel.AsyncCompletedEventArgs, rootDir As String, filename As String) Handles DownloaderFrame_UpdaterEngine.DownloadCompleted
        MyMainWindow.Dispatcher.Invoke( _
                    New Action(Function()
                                   If Not e.Cancelled Then
                                       result = True
                                       If Not isClosed Then ' Used to determine if this window has already been closed (to prevent Me.Close() from firing, if this is the case)
                                           Me.Close()
                                       End If
                                   End If

                                   Return True
                               End Function))
    End Sub

    Private Sub myUpdaterEngine_DownloadProgressChanged(percentage As String, receiveddata As String, totaldata As String) Handles DownloaderFrame_UpdaterEngine.DownloadProgressChanged
        MyMainWindow.Dispatcher.Invoke( _
                    New Action(Function()
                                   thisProgressBar.State = Elysium.Controls.ProgressState.Normal
                                   thisProgressBar.Value = percentage
                                   txtProgess.Text = percentage
                                   Return True
                               End Function))
    End Sub

    Private Sub tabcontrol_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles tabcontrol.SelectionChanged
        BackButton.IsEnabled = (Not tabcontrol.SelectedIndex = 0)
        If (Not tabcontrol.SelectedIndex = 0) Then
            tabcontrol.Margin = New Thickness(80, 0, 0, 0)
        Else
            tabcontrol.Margin = New Thickness(-5, 0, 0, 0)
        End If
    End Sub

    Private Sub BackButton_Click(sender As Object, e As RoutedEventArgs) Handles BackButton.Click
        If tabcontrol.SelectedIndex = 1 Then ' If current screen is the download screen,
            DownloaderFrame_UpdaterEngine.client.CancelAsync() ' Then cancel download first before changing screen
        End If
        tabcontrol.SelectedIndex -= 1
    End Sub

    Dim isClosed As Boolean = False
    Private Sub DownloaderFrame_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        If tabcontrol.SelectedIndex = 1 Then ' If current screen is the download screen,
            If DownloaderFrame_UpdaterEngine.client.IsBusy Then
                DownloaderFrame_UpdaterEngine.client.CancelAsync() ' Then cancel download first before changing screen
                isClosed = True
            End If
        End If
    End Sub
End Class
