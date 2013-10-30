Public Class ConfigJava

    Dim isInit As Boolean = True

    Sub New(m As SuperOverlay)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        superoverlay = m

        ' Set the max of the slider's range to the total memory of system
        Dim maxMemory As Integer = (ServerClass.GetTotalMemoryInBytes() / (1024 * 1024)) * 0.95 ' in MiB 
        'Times by 0.95 to prevent allocating a ridiculous maximum memory value

        sliderMemory.Maximum = maxMemory

        Dim savedMemory As Integer = MyAppSettings.UserSettings_SrvMaxMemoryInt

#If DEBUG Then
        If savedMemory < maxMemory Then
#Else
        If savedMemory <= maxMemory Then
#End If

            maxmem.Text = savedMemory
        Else
            If maxMemory > 1024 Then
                Dim n As New MessageWindow(MyMainWindow, "", "You've set the server more memory than you have available on this machine! Resetting the memory to the default (1GB)")
                maxmem.Text = 1024
            Else
                Dim n As New MessageWindow(MyMainWindow, "", "You've set the server more memory than you have available on this machine! Please set another value for the memory")
                maxmem.Text = 32
            End If
        End If
        Try
            sliderMemory.Value = maxmem.Text
        Catch ex As Exception

        End Try

        javainstalltype.Text = DetectJava.FindJavaInstallType

        minmem.Text = My.Settings.Startup_MemoryMin.ToLower.Replace("m", "")
        jarpath.Text = MyAppSettings.JavaExec

        If MyAppSettings.JavaExec Is "" Then
            jreauto.IsChecked = True
        Else
            jremanual.IsChecked = True
        End If

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

    Private Sub TextBox_TextChanged_3(sender As Object, e As TextChangedEventArgs)
        'parameters RIGHT
        If Not isInit Then
            My.Settings.LaunchArgu_SRV = CType(sender, TextBox).Text
        End If
    End Sub

    'Private Sub TextBox_TextChanged_4(sender As Object, e As TextChangedEventArgs)
    '    If Not isInit Then
    '        My.Settings.LaunchArgu_SRV = CType(sender, TextBox).Text
    '    End If
    'End Sub

    Private Sub Hyperlink_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub sliderMemory_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of System.Double)) Handles sliderMemory.ValueChanged
        If Not isInit Then
            maxmem.Text = CType(sliderMemory.Value, Integer)
        End If
    End Sub

#Region "'Combined' startup parameters textbox"

    'Allow the user to jump from the left side of the textbox to the right, and vice versa
    'Must use PreviewKeyDown, NOT KeyDown!! since the Left/Right key DON'T trigger KeyDown
    'Private Sub parameters_right_KeyDown(sender As Object, e As KeyEventArgs) Handles parameters_right.PreviewKeyDown
    '    If e.Key = Key.Left Then
    '        parameters_left.Focus()
    '    'End If
    'End Sub
    'Private Sub parameters_left_KeyDown(sender As Object, e As KeyEventArgs) Handles parameters_left.PreviewKeyDown
    '    If e.Key = Key.Right Then
    '        parameters_right.Focus()
    '    End If
    'End Sub

    Private Sub DockPanel_GotFocus(sender As Object, e As RoutedEventArgs)
        parameters_right.Focus()
    End Sub

#End Region


#Region "Java selection"

    Private Sub jremanual_Checked(sender As Object, e As RoutedEventArgs) Handles jremanual.Unchecked
        MyAppSettings.JavaExec = ""
    End Sub

    Private Sub TextBox_TextChanged_JREpath(sender As Object, e As TextChangedEventArgs)
        If Not isInit Then
            MyAppSettings.JavaExec = CType(sender, TextBox).Text
        End If
    End Sub

    Private Sub Hyperlink_ExploreJavaPath(sender As Object, e As RoutedEventArgs)
        System.Diagnostics.Process.Start("explorer.exe", DetectJava.FindPath)
    End Sub

#End Region

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim o As Microsoft.Win32.OpenFileDialog = New Microsoft.Win32.OpenFileDialog()
        o.DefaultExt = "java.exe"
        o.Filter = "Java Executable (java.exe)"
        Dim result As Boolean = o.ShowDialog()
        If result = True Then
            jarpath.Text = o.FileName
        End If
    End Sub
End Class