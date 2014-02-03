Imports System.Net.NetworkInformation
Imports System.Windows.Forms

Public Class ConfigJarfileBackend

    Sub New(m As SuperOverlay)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        AddHandler NetworkChange.NetworkAvailabilityChanged, New NetworkAvailabilityChangedEventHandler(AddressOf NetworkChange_NetworkAvailabilityChanged)

        superoverlay = m
        latestverfetchservice.WorkerSupportsCancellation = True
        UpdatePageContent()
    End Sub

    Dim superoverlay As SuperOverlay
    Public Sub isClosing()
        If My.Computer.FileSystem.FileExists(System.Environment.CurrentDirectory & "\" & thisJar.SelectedValue) Then
            MyUserSettings.Jarfile = thisJar.SelectedValue
            MyServer.ReloadStartupParameters()
        Else
            Dim n As New MessageWindow(MyMainWindow, "", "You havn't selected a jarfile yet! Dashboard can't start the server or make configuration changes until you've selected which jarfile to use." & vbNewLine & vbNewLine & "Return to this screen by clicking on the Configuration tab, and selecting 'Backend settings'", "Oops", "large")
        End If
        superoverlay.Confirm_DoClose(Me)
    End Sub

    ''' <summary>
    ''' Scan the app directory for *.jar files
    ''' </summary>
    Friend Sub UpdatePageContent()
        thisJar.Items.Clear()
        For Each file In My.Computer.FileSystem.GetDirectoryInfo(System.Environment.CurrentDirectory).GetFiles("*.jar", IO.SearchOption.TopDirectoryOnly)
            thisJar.Items.Add(file.Name.ToString)
        Next
        thisJar.SelectedValue = MyUserSettings.Jarfile
    End Sub

#Region "WebEngine"
    ' Gathers latest version info from the internet ('CraftBukkit' only, the rest will open a hyperlink to their respective project websites)
    ' The Mojang Minecraft server is downloaded from a static web address

    Dim UpdaterProgressWindow As DownloadWindow

    Public WithEvents myDownloaderEngine As New JarDownloadEngine()
    Private Sub StartDownload_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles Button4.Click
        If download_combobox.SelectedValue Is Nothing Then Exit Sub

        ' UI
        UpdaterProgressWindow = New DownloadWindow(myDownloaderEngine)
        UpdaterProgressWindow.Show()


        Select Case download_combobox.SelectedValue.Content.ToString
            Case "Official Mojang Server (Vanilla)"

                UpdaterProgressWindow.Label1.Content = "Now fetching the latest server files from the internet..."

                Dim Ver As String = myDownloaderEngine.GetLatestVanillaVersion
                Dim url As String = "https://s3.amazonaws.com/Minecraft.Download/versions/" + Ver + "/minecraft_server." + Ver + ".jar"

                myDownloaderEngine.StartJarDownload(System.Environment.CurrentDirectory, UpdaterProgressWindow.UIProgressBar, url)
            Case "CraftBukkit"
                UpdaterProgressWindow.Label1.Content = "Now fetching the CraftBukkit server files from the internet..."
                myDownloaderEngine.StartJarDownload(System.Environment.CurrentDirectory, UpdaterProgressWindow.UIProgressBar, "http://cbukk.it/craftbukkit.jar")
        End Select

    End Sub


#Region "Fetch latest version in background"

    Dim WithEvents latestverfetchservice As New System.ComponentModel.BackgroundWorker

    Private Sub WebService_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles latestverfetchservice.DoWork
        If Not My.Computer.Network.IsAvailable Then
            e.Result = "Offline =("
        Else
            Select Case e.Argument
                Case "vanilla"
                    Dim fetch As String = myDownloaderEngine.GetLatestVanillaVersion()
                    If Not fetch Is Nothing Then
                        e.Result = "The latest version is " & fetch
                    Else
                        e.Result = "Could not retreive latest version number. Try downloading from http://minecraft.net/ manually."
                    End If
                Case "craftbukkit"
                    e.Result = myDownloaderEngine.GetLatestCraftBukkitVersion()
            End Select
        End If
    End Sub

    Private Sub WebService_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles latestverfetchservice.RunWorkerCompleted
        Dispatcher.Invoke( _
                            New Action(Function()
                                           If Not e.Cancelled Then
                                               download_combobox.IsEnabled = True
                                               LatestVerLabel.Text = e.Result
                                               hyperlinkWebText.Text = ""
                                               dwnldlink.Visibility = Windows.Visibility.Visible
                                           End If

                                           Return True
                                       End Function))
    End Sub

    Sub NetworkChange_NetworkAvailabilityChanged(ByVal sender As Object, ByVal e As NetworkAvailabilityEventArgs)
        If e.IsAvailable Then
            ' Try fetch version info again if the network state has changed to 'internet'
            On Error Resume Next
            If download_combobox.SelectedValue.Content.ToString = "CraftBukkit" And (Not latestverfetchservice.IsBusy) Then
                latestverfetchservice.RunWorkerAsync()
            End If
        End If
    End Sub


#End Region
#End Region

#Region "UI"
    Private Sub ComboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

        Select Case CType(sender, System.Windows.Controls.ComboBox).SelectedItem.Content.ToString
            Case "Official Mojang Server (Vanilla)"
                LatestVerLabel.Text = "Contacting server..."
                'LatestVerLabel.Text = "Click ""Download"" to begin downloading the latest Minecraft server release from Mojang"
                hyperlinkWebText.Text = ""
                dwnldlink.Visibility = Windows.Visibility.Visible
                If latestverfetchservice.IsBusy Then
                    Try
                        latestverfetchservice.CancelAsync()
                        latestverfetchservice.RunWorkerAsync("vanilla")
                        download_combobox.IsEnabled = False
                    Catch ex As Exception

                    End Try
                Else
                    latestverfetchservice.RunWorkerAsync("vanilla")
                    download_combobox.IsEnabled = False
                End If


                'https://minecraft.net/download
            Case "CraftBukkit"
                LatestVerLabel.Text = "Contacting server..."
                hyperlinkWebText.Text = ""
                dwnldlink.Visibility = Windows.Visibility.Collapsed
                If latestverfetchservice.IsBusy Then
                    Try
                        latestverfetchservice.CancelAsync()
                        latestverfetchservice.RunWorkerAsync("craftbukkit")
                        download_combobox.IsEnabled = False
                    Catch ex As Exception

                    End Try
                Else
                    latestverfetchservice.RunWorkerAsync("craftbukkit")
                    download_combobox.IsEnabled = False
                End If

            Case "Tekkit"
                LatestVerLabel.Text = "Get the latest Tekkit Minecraft server from "
                hyperlinkWebText.Text = "http://www.technicpack.net/tekkit/"
                dwnldlink.Visibility = Windows.Visibility.Collapsed
                If latestverfetchservice.IsBusy Then
                    latestverfetchservice.CancelAsync()
                End If
            Case "Forge"
                LatestVerLabel.Text = "Get the latest Forge Minecraft server from "
                hyperlinkWebText.Text = "http://www.minecraftforge.net/wiki/Installation/Universal‎"
                dwnldlink.Visibility = Windows.Visibility.Collapsed
                If latestverfetchservice.IsBusy Then
                    latestverfetchservice.CancelAsync()
                End If
        End Select
    End Sub

    Sub hyperlinkWeb_Clicked() Handles hyperlinkWeb.Click
        Select Case download_combobox.SelectedItem.Content.ToString
            Case "Tekkit"
                System.Diagnostics.Process.Start("http://www.technicpack.net/tekkit/")
            Case "Forge"
                System.Diagnostics.Process.Start("http://www.minecraftforge.net/wiki/Installation/Universal‎")
        End Select
    End Sub

    Private Sub refreshbtn_Click()
        UpdatePageContent()
    End Sub

    Private Sub thisJar_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles thisJar.SelectionChanged

        If My.Computer.FileSystem.FileExists(System.Environment.CurrentDirectory & "\" & thisJar.SelectedValue) Then
            MyUserSettings.Jarfile = thisJar.SelectedValue
            If System.Windows.MessageBox.Show(thisJar.SelectedValue & " has been sucessfully selected as your server backend. Please ensure you start the server at least once to create the configuration file defaults." & vbNewLine & vbNewLine & "Return to the Dashboard home screen?", "Dashboard", MessageBoxButton.YesNo) = MessageBoxResult.Yes Then
                superoverlay.Confirm_DoClose(Me)
            End If
        End If
    End Sub

#End Region

    Private Sub myDownloaderEngine_DownloadCompleted(e As System.ComponentModel.AsyncCompletedEventArgs, rootDir As String, filename As String) Handles myDownloaderEngine.DownloadCompleted
        If Not e.Cancelled Then
            MyMainWindow.Dispatcher.Invoke( _
                    New Action(Function()
                                   ' Update UI
                                   
                                   If Not thisJar.Items.Contains(filename) Then
                                       thisJar.Items.Add(filename)
                                   End If

                                   thisJar.SelectedValue = filename

                                   UpdatePageContent()

                                   Return True
                               End Function))
            'MessageBox.Show("Download successful, CraftBukkit is now ready to start.")
            'Else
            '    MyMainWindow.Dispatcher.Invoke( _
            '            New Action(Function()

            '                           Return True
            '                       End Function))
        End If
    End Sub

    Private Sub myDownloaderEngine_DownloadProgressChanged(percentage As String, receiveddata As String, totaldata As String) Handles myDownloaderEngine.DownloadProgressChanged
        MyMainWindow.Dispatcher.Invoke( _
            New Action(Function()
                           UpdaterProgressWindow.UIProgressText.Content = Decimal.Round(CInt(percentage), 0) & "% " & receiveddata & "/" & totaldata
                           UpdaterProgressWindow.UIProgressBar.Value = percentage
                           Return True
                       End Function))
    End Sub
End Class