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
    Private Sub StartDownloadBtn_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles Button4.Click
        If combobox1.SelectedValue Is Nothing Then
            Exit Sub
        End If

        UpdaterModule = New UpdatePage(myUpdaterEngine)

        Me.NavigationService.Content = UpdaterModule

        Select Case combobox1.SelectedValue.Content.ToString
            Case "Official Mojang Server (Vanilla)"
                UpdaterModule.Label1.Content = "Downloading latest Minecraft server release from Mojang"
                myUpdaterEngine.AutoNewUpdate(System.Environment.CurrentDirectory, UpdaterModule, "https://s3.amazonaws.com/MinecraftDownload/launcher/minecraft_server.jar")
            Case "CraftBukkit"
                UpdaterModule.Label1.Content = UpdaterModule.Label1.Content.ToString & ": " & LatestVerLabel.Text
                myUpdaterEngine.AutoNewUpdate(System.Environment.CurrentDirectory, UpdaterModule, "http://cbukk.it/craftbukkit.jar")
        End Select

    End Sub

    Dim WithEvents webservice As New System.ComponentModel.BackgroundWorker

    Private Sub WebService_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles webservice.DoWork
        e.Result = myUpdaterEngine.GetLatestVersionID_Online()
    End Sub
    Private Sub WebService_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles webservice.RunWorkerCompleted
        Dispatcher.Invoke( _
                            New Action(Function()
                                           If Not e.Cancelled Then
                                               combobox1.IsEnabled = True
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
            If combobox1.SelectedValue.Content.ToString = "CraftBukkit" And (Not webservice.IsBusy) Then
                webservice.RunWorkerAsync()
            End If
        End If
    End Sub

#End Region

#Region "UI"
    Private Sub ComboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

        Select Case CType(sender, System.Windows.Controls.ComboBox).SelectedItem.Content.ToString
            Case "Official Mojang Server (Vanilla)"
                LatestVerLabel.Text = "Click ""Download"" to begin downloading the latest Minecraft server release from Mojang"
                hyperlinkWebText.Text = ""
                dwnldlink.Visibility = Windows.Visibility.Visible
                If webservice.IsBusy Then
                    webservice.CancelAsync()
                End If

                'https://minecraft.net/download
            Case "CraftBukkit"
                LatestVerLabel.Text = "Contacting server..."
                hyperlinkWebText.Text = ""
                dwnldlink.Visibility = Windows.Visibility.Collapsed
                Try
                    webservice.RunWorkerAsync()
                    combobox1.IsEnabled = False
                Catch ex As Exception

                End Try

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
        Select Case combobox1.SelectedItem.Content.ToString
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