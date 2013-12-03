Public Class DownloadWindow

    Dim _myDownloaderEngine As JarDownloadEngine
    Sub New(ByRef thisDownloaderEngine As JarDownloadEngine)
        InitializeComponent()
        _myDownloaderEngine = thisDownloaderEngine
    End Sub
    Private Sub Button1_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles Button1.Click
        CancelDownload()
    End Sub

    Private Sub CancelDownload()
        Me.Close()
        _myDownloaderEngine.client.CancelAsync()
        Dim n As New MessageWindow(MyMainWindow, "", "The download has been cancelled.", "Cancelled")
    End Sub
End Class