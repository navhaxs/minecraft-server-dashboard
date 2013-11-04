Public Class rconquerysettings

    ' Save settings and close
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        SetBooleanValueOfProperty(EnableQuery, "enable-query")
        SetTextBoxStringOfProperty(queryPort, "query.port")

        SetBooleanValueOfProperty(EnableRcon, "enable-rcon")
        SetTextBoxStringOfProperty(rconPasswd, "rcon.password")
        SetTextBoxStringOfProperty(rconPort, "rcon.port")
        Close()
    End Sub

    ' Load settings on open
    Private Sub rconquerysettings_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim configreader As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)
        GetBooleanValueOfProperty(EnableQuery, "enable-query", configreader)
        GetTextBoxStringOfProperty(queryPort, "query.port", configreader)

        GetBooleanValueOfProperty(EnableRcon, "enable-rcon", configreader)
        GetTextBoxStringOfProperty(rconPasswd, "rcon.password", configreader)
        GetTextBoxStringOfProperty(rconPort, "rcon.port", configreader)
    End Sub
End Class