Public Class ConfigJava

    Dim isInit As Boolean = True

    Sub New(m As SuperOverlay)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        superoverlay = m

        ' Set the max of the slider's range to the total memory of system
        sliderMemory.Maximum = (ServerClass.GetTotalMemoryInBytes() / (1024 * 1024))

        'Dim savedMemory As Integer = My.Settings.Startup_Memory.ToLower.Replace("m", "")
        Dim savedMemory As Integer = MyAppSettings.UserSettings_SrvMaxMemoryInt

        If savedMemory < sliderMemory.Maximum Then
            maxmem.Text = savedMemory
        Else
            ' ??
            If MessageBox.Show("The current amount of memory you've allocated to the server appears to have exceeded the maximum range of this machine. Resetting the memory to the default (1GB / 1024MB)") Then
                maxmem.Text = 1024
            End If
            ' ??
        End If
        Try
            sliderMemory.Value = maxmem.Text
        Catch ex As Exception

        End Try

        minmem.Text = My.Settings.Startup_MemoryMin.ToLower.Replace("m", "")
        jarpath.Text = MyAppSettings.JavaExec
        isInit = False
    End Sub

    Dim superoverlay As SuperOverlay
    Public Sub isClosing()

        'NOT NEEDED since the value is updated on the textbox's value change event
        'Dim savedMemory As Integer = My.Settings.Startup_Memory.ToLower.Replace("m", "")
        'Debug.Print(maxmem.Text & "" & savedMemory)
        'If Not maxmem.Text = savedMemory Then
        '    If MessageBox.Show("Save new memory configuration of " & maxmem.Text & "MB ?", "Dashboard", MessageBoxButton.YesNo) Then
        '        MyAppSettings.UserSettings_SrvMaxMemory = maxmem.Text & "M"
        '    End If
        'End If

        superoverlay.Confirm_DoClose(Me)
    End Sub

    Public Sub HelpClicked()
        System.Diagnostics.Process.Start("http://www.minecraftwiki.net/wiki/Server/Requirements")
    End Sub

    ' Save settings on change
    Private Sub TextBox_TextChanged(sender As Object, e As TextChangedEventArgs)
        If Not isInit Then
            MyAppSettings.UserSettings_SrvMaxMemory = maxmem.Text & "M"
            'My.Settings.Startup_Memory = CType(sender, TextBox).Text & "M"
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

    Private Sub Hyperlink_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub sliderMemory_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of System.Double)) Handles sliderMemory.ValueChanged
        If Not isInit Then
            maxmem.Text = CType(sliderMemory.Value, Integer)
        End If
    End Sub
End Class