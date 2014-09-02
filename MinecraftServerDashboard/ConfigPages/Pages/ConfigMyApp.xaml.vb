Public Class ConfigMyApp

    Dim isUnsavedChanges As Boolean = False
    Sub New(m As SuperOverlay)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        superoverlay = m

        DoInit(ContentGrid) ' Add event handlers to each property control (see 'DoInit' sub code)

        Select Case MyUserSettings.settingsStore.App_SuppressMinimiseMessage ' Load settings
            Case "s"
                defaultexitaction.SelectedIndex = 1
            Case "m"
                defaultexitaction.SelectedIndex = 2
            Case Else
                defaultexitaction.SelectedIndex = 0
        End Select

        For Each i In MyUserSettings.settingsStore.ProfileDir_ExcludedDirectories
            dir2exclude.Text = dir2exclude.Text & i & vbLf
        Next

        notepadExec.Text = MyUserSettings.settingsStore.UserSettings_DefaultTextEditor
        isUnsavedChanges = False
    End Sub

    Dim superoverlay As SuperOverlay
    Public Sub isClosing()

        If isUnsavedChanges Then
            Select Case MessageBox.Show("You have made unsaved changes! Would you like to save these changes?", "Server Configuration", MessageBoxButton.YesNoCancel)
                Case MessageBoxResult.Yes
                    btnSave_Click()
                    superoverlay.Confirm_DoClose(Me)
                Case MessageBoxResult.No
                    superoverlay.Confirm_DoClose(Me)
                Case MessageBoxResult.Cancel
            End Select
        Else
            superoverlay.Confirm_DoClose(Me)
        End If
    End Sub

    Private Sub btnSave_Click() Handles btnSave.Click
        ' Save settings
        Select Case defaultexitaction.SelectedIndex
            Case 1
                MyUserSettings.settingsStore.App_SuppressMinimiseMessage = "s"
            Case 2
                MyUserSettings.settingsStore.App_SuppressMinimiseMessage = "m"
            Case Else
                MyUserSettings.settingsStore.App_SuppressMinimiseMessage = ""
        End Select

        MyUserSettings.settingsStore.ProfileDir_ExcludedDirectories.Clear()
        For Each i In dir2exclude.Text.Split(vbLf)
            MyUserSettings.settingsStore.ProfileDir_ExcludedDirectories.Add(i)
        Next

        MyUserSettings.settingsStore.UserSettings_DefaultTextEditor = notepadExec.Text

        ' Store settings to disk now since this activity is the main app configuration screen
        MyUserSettings.settingsStore.Save()

        isUnsavedChanges = False
    End Sub

#Region "This code adds events to each TextBox, CheckBox, etc. on this form to ensure that the user does not navigate away from this page with unsaved changes"

    Sub DoInit(e As Object)
        For Each o As Object In e.Children
            DoRecursion(o)
        Next
    End Sub

    Sub DoRecursion(o As Object)
        If TypeOf o Is StackPanel Then
            DoInit(o)
        ElseIf TypeOf o Is Grid Then
            DoInit(o)
        ElseIf TypeOf o Is Border Then
            DoRecursion(CType(o, Border).Child)
        ElseIf TypeOf o Is TextBox Then
            AddHandler CType(o, TextBox).TextChanged, AddressOf InvalidateisUnsavedChanges
        ElseIf TypeOf o Is CheckBox Then
            AddHandler CType(o, CheckBox).Checked, AddressOf InvalidateisUnsavedChanges
        ElseIf TypeOf o Is ComboBox Then
            AddHandler CType(o, ComboBox).SelectionChanged, AddressOf InvalidateisUnsavedChanges
        End If
    End Sub
#End Region

    Public Sub InvalidateisUnsavedChanges()
        isUnsavedChanges = True
    End Sub

    Private Sub btnReset_Click(sender As Object, e As RoutedEventArgs) Handles btnReset.Click
        ' Reset all app settings, then exit
        If MyServer.ServerIsOnline Then
            MessageBox.Show("Stop the server before proceeding")
            Exit Sub
        End If
        If MessageBox.Show("This will restart the application and reset all Dashboard settings. None of your game data will be deleted. Continue?", "App Reset", MessageBoxButton.YesNoCancel) = MessageBoxResult.Yes Then
            ' Force save a blank config
            My.Computer.FileSystem.DeleteFile(DEFAULT_CONFIG_FILENAME)
            MyUserSettings.Load()

            'Restart Dashboard
            superoverlay.Confirm_DoClose(Me)
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location)
            Application.Current.Shutdown()
        End If

    End Sub
End Class