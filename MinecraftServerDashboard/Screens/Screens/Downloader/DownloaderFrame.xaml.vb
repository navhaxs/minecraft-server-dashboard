Public Class DownloaderFrame

    'Dim myUpdaterEngine As New UpdateEngine()
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        tabcontrol.SelectedIndex += 1

        'UpdaterModule = New DownloadWindow(myUpdaterEngine)

        UpdaterModule.Show()

        Select Case btnVanilla.IsChecked
            Case True ' Official Mojang Server (Vanilla)
                UpdaterModule.Label1.Content = "Now fetching the latest server files from the internet..."


                Dim Ver As String = UpdaterModule.m_UpdaterEngine.GetLatestVersion_Vanilla
                Dim url As String = "https://s3.amazonaws.com/Minecraft.Download/versions/" + Ver + "/minecraft_server." + Ver + ".jar"

                'myUpdaterEngine.AutoNewUpdate(System.Environment.CurrentDirectory, UpdaterModule, url)
            Case False 'CraftBukkit
                UpdaterModule.Label1.Content = "Now fetching the CraftBukkit server files from the internet..."
                'myUpdaterEngine.AutoNewUpdate(System.Environment.CurrentDirectory, UpdaterModule, "http://cbukk.it/craftbukkit.jar")
        End Select


    End Sub


End Class
