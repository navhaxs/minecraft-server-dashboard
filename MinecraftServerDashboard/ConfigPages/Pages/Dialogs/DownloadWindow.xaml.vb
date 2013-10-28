Public Class DownloadWindow

    Sub New(UpdaterEngine As UpdateEngine)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        m_UpdaterEngine = UpdaterEngine
    End Sub

    Public m_UpdaterEngine As UpdateEngine

    Private Sub Button1_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles Button1.Click
        CancelDownload()
    End Sub

    Private Sub CancelDownload()
        Me.Close()
        m_UpdaterEngine.client.CancelAsync()
        Dim n As New MessageWindow(MyMainWindow, "", "The download has been cancelled.", "Cancelled")
    End Sub
End Class
