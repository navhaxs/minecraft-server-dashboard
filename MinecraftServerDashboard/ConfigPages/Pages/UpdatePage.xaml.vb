Class UpdatePage

    Sub New(UpdaterEngine As UpdateEngine)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        m_UpdaterEngine = UpdaterEngine
    End Sub

    Dim m_UpdaterEngine As UpdateEngine

    Private Sub Button1_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles Button1.Click
        m_UpdaterEngine.client.CancelAsync()
        Try
            ' Hide this control permanently
            Me.NavigationService.GoBack()
            Me.NavigationService.RemoveBackEntry()
        Catch ex As Exception

        End Try
        MessageBox.Show("The download has been cancelled")
    End Sub
End Class