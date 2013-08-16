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
        GetBooleanValueOfProperty(EnableQuery, "enable-query")
        GetTextBoxStringOfProperty(queryPort, "query.port")

        GetBooleanValueOfProperty(EnableRcon, "enable-rcon")
        GetTextBoxStringOfProperty(rconPasswd, "rcon.password")
        GetTextBoxStringOfProperty(rconPort, "rcon.port")
    End Sub
End Class