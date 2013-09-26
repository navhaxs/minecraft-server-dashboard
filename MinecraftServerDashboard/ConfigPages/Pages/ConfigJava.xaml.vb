Public Class ConfigJava

    Dim isInit As Boolean = True

    Sub New(m As SuperOverlay)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        superoverlay = m
        maxmem.Text = My.Settings.Startup_Memory.ToLower.Replace("m", "")
        minmem.Text = My.Settings.Startup_MemoryMin.ToLower.Replace("m", "")
        jarpath.Text = MyAppSettings.JavaExec
        isInit = False
    End Sub

    Dim superoverlay As SuperOverlay
    Public Sub isClosing()
        superoverlay.Confirm_DoClose(Me)
    End Sub

    Public Sub HelpClicked()
        System.Diagnostics.Process.Start("http://www.minecraftwiki.net/wiki/Server/Requirements")
    End Sub

    ' Save settings on change
    Private Sub TextBox_TextChanged(sender As Object, e As TextChangedEventArgs)
        If Not isInit Then
            MyAppSettings.UserSettings_SrvMaxMemory = CType(sender, TextBox).Text & "M"
        End If
    End Sub

    Private Sub TextBox_TextChanged_1(sender As Object, e As TextChangedEventArgs)
        If Not isInit Then
            MyAppSettings.UserSettings_SrvMinMemory = CType(sender, TextBox).Text & "M"
        End If
    End Sub

    Private Sub TextBox_TextChanged_2(sender As Object, e As TextChangedEventArgs)
        If Not isInit Then
            MyAppSettings.JavaExec = CType(sender, TextBox).Text
        End If
    End Sub

    Private Sub TextBox_TextChanged_3(sender As Object, e As TextChangedEventArgs)
        If Not isInit Then
            My.Settings.LaunchArgu_JAVA = CType(sender, TextBox).Text
        End If
    End Sub

    Private Sub TextBox_TextChanged_4(sender As Object, e As TextChangedEventArgs)
        If Not isInit Then
            My.Settings.LaunchArgu_SRV = CType(sender, TextBox).Text
        End If
    End Sub
End Class