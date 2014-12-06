Imports System.Text

Public Class JoinNewPlayers
    Sub New()
        ' Add any initialization after the InitializeComponent() call.
        InitializeComponent()

        Me.DataContext = MyExternalIPAddressViewModel
    End Sub

    Private Sub JoinNewPlayers_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        MyMainWindow.MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.None
    End Sub

    Private Sub JoinNewPlayers_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim APIreader As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)
        lblPortNumber.Text = ":" & APIreader.ReturnConfigValue("server-port")
        If lblPortNumber.Text = ":25565" Or lblPortNumber.Text = ":" Then
            ' If default port number, hide it (not needed/redundant)
            lblPortNumber.Text = ""
        End If
    End Sub

    Private Sub btnHelp_Click(sender As Object, e As RoutedEventArgs)
        System.Diagnostics.Process.Start("http://www.minecraftwiki.net/wiki/Tutorials/Setting_up_a_server#Connect_to_the_Minecraft_server")
    End Sub

    Private Sub Hyperlink_Click(sender As Object, e As RoutedEventArgs)
        Dim copyStr As New StringBuilder
        copyStr.Append(MyExternalIPAddressViewModel.ExternalIP)

        Dim APIreader As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)
        Dim port As String = APIreader.ReturnConfigValue("server-port")

        If Not ((port = "25565") Or (port = "")) Then
            copyStr.Append(":" & APIreader.ReturnConfigValue("server-port"))
        End If

        My.Computer.Clipboard.SetText(copyStr.ToString)
    End Sub
End Class