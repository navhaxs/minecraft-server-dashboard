Public Class ConfigJava

    Dim isInit As Boolean = True

    Sub New(m As SuperOverlay)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        superoverlay = m

        ' Set the max of the slider's range to the total memory of system        
        maxmem_Validate()

        Try
            sliderMemory.Value = maxmem.Text
        Catch ex As Exception

        End Try

        javainstalltype.Text = DetectJava.FindJavaInstallType

        minmem.Text = My.Settings.Startup_MemoryMin.ToLower.Replace("m", "")
        jarpath.Text = MyUserSettings.JavaExec
        parameters_right.Text = My.Settings.JarLaunchArguments

        If MyUserSettings.JavaExec Is "" Then
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
    Private Sub maxmem_TextChanged(sender As Object, e As TextChangedEventArgs)
        If Not isInit Then
            If CType(maxmem.Text, Integer) > 0 Then
                'todo ignore if null2
                MyUserSettings.MaximumMemory = maxmem.Text & "M"
            End If
            'My.Settings.Startup_Memory = CType(sender, TextBox).Text & "M"
        End If
    End Sub

    Private Sub TextBox_TextChanged_1(sender As Object, e As TextChangedEventArgs)
        If Not isInit Then
            MyUserSettings.MinimumMemory = CType(sender, TextBox).Text & "M"
        End If
    End Sub

    Private Sub TextBox_TextChanged_3(sender As Object, e As TextChangedEventArgs)
        'parameters RIGHT
        If Not isInit Then
            My.Settings.JarLaunchArguments = CType(sender, TextBox).Text
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
        MyUserSettings.JavaExec = ""
    End Sub

    Private Sub TextBox_TextChanged_JREpath(sender As Object, e As TextChangedEventArgs)
        If Not isInit Then
            MyUserSettings.JavaExec = CType(sender, TextBox).Text
        End If
    End Sub

    Private Sub Hyperlink_ExploreJavaPath(sender As Object, e As RoutedEventArgs)
        Dim path As String = DetectJava.FindPath
        If Not My.Computer.FileSystem.DirectoryExists(path) Then
            path = "http://java.com/en/download/"
        End If
        System.Diagnostics.Process.Start("explorer.exe", path)
    End Sub

#End Region

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim o As Microsoft.Win32.OpenFileDialog = New Microsoft.Win32.OpenFileDialog()
        o.DefaultExt = "java.exe"
        o.Filter = "Java Executable|java.exe"
        Dim result As Boolean = o.ShowDialog()
        If result = True Then
            jarpath.Text = o.FileName
        End If
    End Sub

    Private Sub maxmem_LostFocus(sender As Object, e As RoutedEventArgs) Handles maxmem.LostFocus
        maxmem_Validate()
    End Sub

    Private Sub maxmem_Validate()
        Dim TotalMachineMemory As Integer = (ServerClass.GetTotalMemoryInBytes() / (1024 * 1024)) * 0.95 ' in MiB 
        'Times by 0.95 to prevent allocating a ridiculous maximum memory value

        sliderMemory.Maximum = TotalMachineMemory

        Dim savedMemory As Integer = MyUserSettings.JVM_Xm_paramter_AsInteger(MyUserSettings.MaximumMemory)

        If savedMemory <= TotalMachineMemory Then
            maxmem.Text = savedMemory
        Else
            If TotalMachineMemory > 1024 Then
                Dim n As New MessageWindow(MyMainWindow, "", "You've set the server more memory than you have available on this machine! Resetting the memory to the default (1GB)")
                maxmem.Text = 1024
            Else
                Dim n As New MessageWindow(MyMainWindow, "", "You've set the server more memory than you have available on this machine! Please set another value for the memory")
                maxmem.Text = 32
            End If
        End If
    End Sub

    Private Sub btnSetJavaSpecificArgs_Click(sender As Object, e As RoutedEventArgs) Handles btnSetJavaSpecificArgs.Click
        Dim m As New AdvJavaArguementsDialog
        m.Owner = MyMainWindow
        m.ShowDialog()
    End Sub
End Class