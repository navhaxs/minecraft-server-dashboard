Imports System.Net.NetworkInformation
Imports System.Windows.Forms

Public Class ConfigJarfileBackend

    Sub New(m As SuperOverlay)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        AddHandler NetworkChange.NetworkAvailabilityChanged, New NetworkAvailabilityChangedEventHandler(AddressOf NetworkChange_NetworkAvailabilityChanged)

        superoverlay = m
        webservice.WorkerSupportsCancellation = True
        UpdatePageContent()
    End Sub

    Dim superoverlay As SuperOverlay
    Public Sub isClosing()
        If My.Computer.FileSystem.FileExists(System.Environment.CurrentDirectory & "\" & thisJar.SelectedValue) Then
            MyAppSettings.Jarfile = thisJar.SelectedValue
            MyServer.ReloadStartupParameters()
        Else
            System.Windows.MessageBox.Show("You havn't selected a jarfile yet! Dashboard can't start the server or make configuration changes until you've selected which jarfile to use." & vbNewLine & vbNewLine & "Return to this screen by clicking on the Configuration tab, and selecting 'Backend settings'", "Dashboard", MessageBoxButton.OK)
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
        thisJar.SelectedValue = MyAppSettings.Jarfile
    End Sub

#Region "WebEngine"
    ' Gathers latest version info from the internet ('CraftBukkit' only, the rest will open a hyperlink to their respective project websites)
    ' The Mojang Minecraft server is downloaded from a static web address

    Dim myUpdaterEngine As New UpdateEngine(Me)
    Private Sub StartDownload_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles Button4.Click
        If download_combobox.SelectedValue Is Nothing Then Exit Sub


        UpdaterModule = New UpdatePage(myUpdaterEngine)

        Me.NavigationService.Content = UpdaterModule

        Select Case download_combobox.SelectedValue.Content.ToString
            Case "Official Mojang Server (Vanilla)"
                UpdaterModule.Label1.Content = "Downloading latest Minecraft server release from Mojang: " & LatestVerLabel.Text

                Dim Ver As String = myUpdateEngine.GetLatestVersion_Vanilla
                Dim url As String = "https://s3.amazonaws.com/Minecraft.Download/versions/" + Ver + "/minecraft_server." + Ver + ".jar"

                myUpdaterEngine.AutoNewUpdate(System.Environment.CurrentDirectory, UpdaterModule, url)
            Case "CraftBukkit"
                UpdaterModule.Label1.Content = UpdaterModule.Label1.Content.ToString & ": " & LatestVerLabel.Text
                myUpdaterEngine.AutoNewUpdate(System.Environment.CurrentDirectory, UpdaterModule, "http://cbukk.it/craftbukkit.jar")
        End Select

    End Sub

    Dim WithEvents webservice As New System.ComponentModel.BackgroundWorker

    Private Sub WebService_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles webservice.DoWork
        If Not My.Computer.Network.IsAvailable Then
            e.Result = "Offline =("
        Else
            Select Case e.Argument
                Case "vanilla"
                    Dim fetch As String = myUpdaterEngine.GetLatestVersion_Vanilla()
                    If Not fetch Is Nothing Then
                        e.Result = "The latest version is " & fetch
                    Else
                        e.Result = "Could not retreive latest version number. Try downloading from http://minecraft.net/ manually."
                    End If
                Case "craftbukkit"
                    e.Result = myUpdaterEngine.GetLatestVersion_CraftBukkit()
            End Select
        End If
    End Sub
    Private Sub WebService_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles webservice.RunWorkerCompleted
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
            If download_combobox.SelectedValue.Content.ToString = "CraftBukkit" And (Not webservice.IsBusy) Then
                webservice.RunWorkerAsync()
            End If
        End If
    End Sub

#End Region

#Region "UI"
    Private Sub ComboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

        Select Case CType(sender, System.Windows.Controls.ComboBox).SelectedItem.Content.ToString
            Case "Official Mojang Server (Vanilla)"
                LatestVerLabel.Text = "Contacting server..."
                'LatestVerLabel.Text = "Click ""Download"" to begin downloading the latest Minecraft server release from Mojang"
                hyperlinkWebText.Text = ""
                dwnldlink.Visibility = Windows.Visibility.Visible
                If webservice.IsBusy Then
                    Try
                        webservice.CancelAsync()
                        webservice.RunWorkerAsync("vanilla")
                        download_combobox.IsEnabled = False
                    Catch ex As Exception

                    End Try
                Else
                    webservice.RunWorkerAsync("vanilla")
                    download_combobox.IsEnabled = False
                End If


                'https://minecraft.net/download
            Case "CraftBukkit"
                LatestVerLabel.Text = "Contacting server..."
                hyperlinkWebText.Text = ""
                dwnldlink.Visibility = Windows.Visibility.Collapsed
                If webservice.IsBusy Then
                    Try
                        webservice.CancelAsync()
                        webservice.RunWorkerAsync("craftbukkit")
                        download_combobox.IsEnabled = False
                    Catch ex As Exception

                    End Try
                Else
                    webservice.RunWorkerAsync("craftbukkit")
                    download_combobox.IsEnabled = False
                End If

            Case "Tekkit"
                LatestVerLabel.Text = "Get the latest Tekkit Minecraft server from "
                hyperlinkWebText.Text = "http://www.technicpack.net/tekkit/"
                dwnldlink.Visibility = Windows.Visibility.Collapsed
                If webservice.IsBusy Then
                    webservice.CancelAsync()
                End If
            Case "Forge"
                LatestVerLabel.Text = "Get the latest Forge Minecraft server from "
                hyperlinkWebText.Text = "http://www.minecraftforge.net/wiki/Installation/Universal‎"
                dwnldlink.Visibility = Windows.Visibility.Collapsed
                If webservice.IsBusy Then
                    webservice.CancelAsync()
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
            MyAppSettings.Jarfile = thisJar.SelectedValue
            If System.Windows.MessageBox.Show(thisJar.SelectedValue & " has been sucessfully selected as your server backend. Please ensure you start the server at least once to create the configuration file defaults." & vbNewLine & vbNewLine & "Return to the Dashboard home screen?", "Dashboard", MessageBoxButton.YesNo) = MessageBoxResult.Yes Then
                superoverlay.Confirm_DoClose(Me)
            End If
        End If
    End Sub

#End Region
End Class