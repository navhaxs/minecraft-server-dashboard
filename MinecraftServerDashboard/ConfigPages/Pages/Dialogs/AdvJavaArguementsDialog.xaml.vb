Public Class AdvJavaArguementsDialog

    Private Sub btnDefault_Click(sender As Object, e As RoutedEventArgs) Handles btnDefault.Click
        txtJAVASpecificStartupParameters.Text = ""
    End Sub

    Private Sub btnSpigot_Click(sender As Object, e As RoutedEventArgs) Handles btnSpigot.Click
        txtJAVASpecificStartupParameters.Text = "-XX:MaxPermSize=128M -Djline.terminal=jline.UnsupportedTerminal"
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        MyUserSettings.settingsStore.Startup_JavaSpecificArgs = txtJAVASpecificStartupParameters.Text
        MyUserSettings.settingsStore.Save()
        Me.Close()
    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        txtJAVASpecificStartupParameters.Text = MyUserSettings.settingsStore.Startup_JavaSpecificArgs
    End Sub
End Class
