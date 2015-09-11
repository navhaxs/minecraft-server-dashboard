Imports System.Net.NetworkInformation

Partial Public Class ConfigJarfileBackend
    Inherits ConfigPage

    Dim superoverlay As SuperOverlay

    Sub New(m As SuperOverlay)
        InitializeComponent() ' This call is required by the designer.
        ' Add any initialization after the InitializeComponent() call.

        superoverlay = m

        ' 
        AddHandler NetworkChange.NetworkAvailabilityChanged, New NetworkAvailabilityChangedEventHandler(AddressOf NetworkChange_NetworkAvailabilityChanged)

        If My.Computer.Network.IsAvailable Then
            FetchLatestVersionMeta_Runnable.RunWorkerAsync()
        End If

        UpdatePageContent()

    End Sub

    Dim _selectedJarfile As String

    Public Sub isClosing()
        If isSelectedJarfileValid() Then
            MyUserSettings.settingsStore.Jarfile = _selectedJarfile
            MyServer.ReloadStartupParameters()
        Else
            Dim n As New MessageWindow(MyMainWindow, "", "Most options will be unavailable until you've selected which jarfile to use." & vbNewLine & vbNewLine & "Return to this screen by clicking on the Configuration tab, and selecting 'Backend settings'", ":(", "large")
        End If
        superoverlay.Confirm_DoClose(Me)
    End Sub

    Function isSelectedJarfileValid()
        If My.Computer.FileSystem.FileExists(System.Environment.CurrentDirectory & "\" & _selectedJarfile) Then
            Return True
        Else : Return False
        End If
    End Function

    ''' <summary>
    ''' Scan the app directory for *.jar files
    ''' </summary>
    Friend Sub UpdatePageContent()
        jarList.Items.Clear()
        For Each file In My.Computer.FileSystem.GetDirectoryInfo(System.Environment.CurrentDirectory).GetFiles("*.jar", IO.SearchOption.TopDirectoryOnly)
            jarList.Items.Add(file.Name.ToString)
        Next
        On Error Resume Next
        jarList.SelectedValue = My.Computer.FileSystem.GetFileInfo(MyUserSettings.settingsStore.Jarfile).Name
        _selectedJarfile = jarList.SelectedValue
    End Sub

    Dim UpdaterProgressWindow As DownloadWindow
    Public WithEvents myDownloaderEngine As New JarDownloadEngine()

    Private Sub btnVn_Click(sender As Object, e As RoutedEventArgs)
        UpdaterProgressWindow = New DownloadWindow(myDownloaderEngine)

        Dim Ver As String = myDownloaderEngine.GetLatestVanillaVersion
        Dim url As String = "https://s3.amazonaws.com/Minecraft.Download/versions/" + Ver + "/minecraft_server." + Ver + ".jar"

        UpdaterProgressWindow.Owner = MyMainWindow
        UpdaterProgressWindow.ShowDialog()
        myDownloaderEngine.StartJarDownload(System.Environment.CurrentDirectory, UpdaterProgressWindow.UIProgressBar, url)
    End Sub

    Private Sub btnCb_Click(sender As Object, e As RoutedEventArgs)
        UpdaterProgressWindow = New DownloadWindow(myDownloaderEngine)

        UpdaterProgressWindow.Owner = MyMainWindow
        UpdaterProgressWindow.ShowDialog()
        myDownloaderEngine.StartJarDownload(System.Environment.CurrentDirectory, UpdaterProgressWindow.UIProgressBar, "http://cbukk.it/craftbukkit.jar")
    End Sub


#Region "WebEngine"

#Region "Fetch latest version in background"

    Dim WithEvents FetchLatestVersionMeta_Runnable As New System.ComponentModel.BackgroundWorker

    Class resultData
        Public cbVer As String
        Public vnVer As String
    End Class

    Private Sub WebService_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles FetchLatestVersionMeta_Runnable.DoWork
        Dim result As New resultData
        'result.cbVer = myDownloaderEngine.GetLatestCraftBukkitVersion(True)
        result.vnVer = myDownloaderEngine.GetLatestVanillaVersion()
        e.Result = result
    End Sub

    Private Sub WebService_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles FetchLatestVersionMeta_Runnable.RunWorkerCompleted
        Dispatcher.Invoke(
                            New Action(Function()
                                           If Not e.Cancelled Then
                                               'txtCraftBukkitVer.Text = "(" & e.Result.cbVer & ")"
                                               txtVanillaVer.Text = "(" & e.Result.vnVer & ")"
                                               btnVn.IsEnabled = True
                                           End If
                                           Return True
                                       End Function))
    End Sub

    Sub NetworkChange_NetworkAvailabilityChanged(ByVal sender As Object, ByVal e As NetworkAvailabilityEventArgs)
        If e.IsAvailable Then
            ' Try fetch version info again if the network state has changed to 'internet'
            On Error Resume Next
            FetchLatestVersionMeta_Runnable.RunWorkerAsync()
        End If
    End Sub


#End Region
#End Region

#Region "Download Dialog UI"

    Private Sub myDownloaderEngine_DownloadCompleted(e As System.ComponentModel.AsyncCompletedEventArgs, rootDir As String, filename As String) Handles myDownloaderEngine.DownloadCompleted
        If Not e.Cancelled Then
            MyMainWindow.Dispatcher.Invoke(
                    New Action(Function()

                                   UpdaterProgressWindow.Close()

                                   _selectedJarfile = filename

                                   jarList.SelectedValue = filename

                                   Dim n As New MessageWindow(MyMainWindow, "", "Dashboard has finished downloading the latest server release, and it has been selected." & vbNewLine & vbNewLine &
                                                              "Return to the Overview tab to launch the server", "Download complete!", "large")

                                   UpdatePageContent()

                                   Return True
                               End Function))
        End If
    End Sub

    Private Sub myDownloaderEngine_DownloadProgressChanged(percentage As String, receiveddata As String, totaldata As String, filename As String) Handles myDownloaderEngine.DownloadProgressChanged
        MyMainWindow.Dispatcher.Invoke(
            New Action(Function()
                           UpdaterProgressWindow.Label1.Text = filename
                           UpdaterProgressWindow.UIProgressText.Text = Decimal.Round(CInt(percentage), 0) & "% " & receiveddata & "/" & totaldata
                           UpdaterProgressWindow.UIProgressBar.Value = percentage
                           Return True
                       End Function))
    End Sub

#End Region

    Private Sub jarList_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        _selectedJarfile = jarList.SelectedValue
    End Sub
End Class

