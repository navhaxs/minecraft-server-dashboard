Partial Public Class ConfigServerProp
    Inherits ConfigPage

    Dim isUnsavedChanges As Boolean = False

    Dim superoverlay As SuperOverlay
    Sub New(m As SuperOverlay)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        superoverlay = m
        DoInit(ContentGrid)
    End Sub

#Region "Set up handlers"
    ''' This code adds events to each TextBox/CheckBox/ComboBox on this form
    ''' to ensure that the user does not navigate away from this page if
    ''' there are unsaved changes
    Sub DoInit(e As Object)
        For Each o As Object In e.Children
            DoRecursion(o)
        Next
    End Sub

    Sub DoRecursion(o As Object)
        If TypeOf o Is StackPanel Then ' Repeat operation on each container, as some controls contain controls themselves
            DoInit(o)
        ElseIf TypeOf o Is Grid Then
            DoInit(o)
        ElseIf TypeOf o Is Border Then
            DoRecursion(CType(o, Border).Child)
        ElseIf TypeOf o Is TextBox Then ' Set handler
            AddHandler CType(o, TextBox).TextChanged, AddressOf InvalidateisUnsavedChanges
        ElseIf TypeOf o Is CheckBox Then
            AddHandler CType(o, CheckBox).Checked, AddressOf InvalidateisUnsavedChanges
            AddHandler CType(o, CheckBox).Unchecked, AddressOf InvalidateisUnsavedChanges
        ElseIf TypeOf o Is ComboBox Then
            AddHandler CType(o, ComboBox).SelectionChanged, AddressOf InvalidateisUnsavedChanges
        End If
    End Sub
#End Region

    Sub Load()
        RefreshData()
    End Sub

    Public Sub isClosing()
        _DoClose()
    End Sub

    Private Function _DoClose() As Boolean
        If isUnsavedChanges Then
            Select Case MessageBox.Show("There are unsaved changes" & vbLf & "Would you like to save these changes?", "Server Configuration", MessageBoxButton.YesNoCancel)
                Case MessageBoxResult.Yes
                    btnSave_Click()
                    superoverlay.Confirm_DoClose(Me)
                Case MessageBoxResult.No
                    superoverlay.Confirm_DoClose(Me)
                Case MessageBoxResult.Cancel
                    Return False
            End Select
        Else
            superoverlay.Confirm_DoClose(Me)
        End If
        Return True
    End Function

    ''' <summary>
    ''' Reload settings from the file "server.properties"
    ''' </summary>
    Friend Sub RefreshData() Handles btnRefreshData.Click
        On Error Resume Next
        Dim configreader As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)

        GetTextBoxStringOfProperty(prop_motd, "motd", configreader)

        gamemode.SelectedIndex = CType(GetStringOfProperty("gamemode", configreader), Integer)
        difficulty.SelectedIndex = CType(GetStringOfProperty("difficulty", configreader), Integer)

        Dim s As String = GetStringOfProperty("op-permission-level", configreader)
        Select Case s
            Case Nothing
                oppermissionlevel.SelectedValue = Nothing
            Case Else
                oppermissionlevel.SelectedIndex = s - 1
        End Select

        GetTextBoxStringOfProperty(spawnprotection, "spawn-protection", configreader)

        GetBooleanValueOfProperty(hardcore, "hardcore", configreader)
        GetBooleanValueOfProperty(snooperenabled, "snooper-enabled", configreader)

        GetBooleanValueOfProperty(CheckBox1, "allow-flight", configreader)
        GetBooleanValueOfProperty(CheckBox2, "allow-nether", configreader)

        GetBooleanValueOfProperty(CheckBox5, "generate-structures", configreader)
        GetBooleanValueOfProperty(CheckBox6, "online-mode", configreader)
        GetBooleanValueOfProperty(CheckBox7, "pvp", configreader)
        GetBooleanValueOfProperty(CheckBox8, "spawn-animals", configreader)
        GetBooleanValueOfProperty(CheckBox9, "spawn-monsters", configreader)
        GetBooleanValueOfProperty(CheckBox10, "spawn-npcs", configreader)
        GetBooleanValueOfProperty(CheckBox11, "white-list", configreader)
        isUnsavedChanges = False
        btnSave.IsEnabled = False

        ScrollViewer1.ScrollToHome()
    End Sub

    ''' <summary>
    ''' Write all settings
    ''' </summary>
    Private Sub btnSave_Click() Handles btnSave.Click
        Try
            SetTextBoxStringOfProperty(prop_motd, "motd")

            SetStringOfProperty(gamemode.SelectedIndex, "gamemode")
            SetStringOfProperty(difficulty.SelectedIndex, "difficulty")

            Select Case oppermissionlevel.SelectedIndex
                Case -1
                Case Else
                    Dim m As Integer = oppermissionlevel.SelectedIndex + 1
                    SetStringOfProperty(m.ToString, "op-permission-level")
            End Select

            SetTextBoxStringOfProperty(spawnprotection, "spawn-protection")

            SetBooleanValueOfProperty(hardcore, "hardcore")
            SetBooleanValueOfProperty(snooperenabled, "snooper-enabled")

            SetBooleanValueOfProperty(CheckBox1, "allow-flight")
            SetBooleanValueOfProperty(CheckBox2, "allow-nether")

            SetBooleanValueOfProperty(CheckBox5, "generate-structures")
            SetBooleanValueOfProperty(CheckBox6, "online-mode")
            SetBooleanValueOfProperty(CheckBox7, "pvp")
            SetBooleanValueOfProperty(CheckBox8, "spawn-animals")
            SetBooleanValueOfProperty(CheckBox9, "spawn-monsters")
            SetBooleanValueOfProperty(CheckBox10, "spawn-npcs")
            SetBooleanValueOfProperty(CheckBox11, "white-list")
            isUnsavedChanges = False
        Catch ex As Exception
            Exit Sub
        End Try

        MessageBox.Show("Your settings have been saved.")

        navPageCBconfig.Data.Is_infotip_RestartRequired = System.Windows.Visibility.Visible

    End Sub

    Sub Do_btn_advancedproperties() Handles btn_advancedproperties.Click
        MyMainWindow.DoManualSrvPropertiesEdit()
    End Sub

    Public Sub InvalidateisUnsavedChanges()
        isUnsavedChanges = True
        btnSave.IsEnabled = True
    End Sub

    ' Delete config file for server to re-create on next start
    Private Sub btn_resetprop_Click(sender As Object, e As RoutedEventArgs) Handles btn_resetprop.Click
        If My.Computer.FileSystem.FileExists(MyServer.MyStartupParameters.ServerProperties) Then
            If MessageBox.Show("Are you sure you wish to reset all settings? You will loose all customisations, and this cannot be undone", "Reset settings", MessageBoxButton.YesNoCancel) = MessageBoxResult.Yes Then
                My.Computer.FileSystem.DeleteFile(MyServer.MyStartupParameters.ServerProperties, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently, FileIO.UICancelOption.DoNothing)
                MessageBox.Show("Success! (Re)Start of the server now to recreate the default settings", "Reset settings", MessageBoxButton.OK)
                isUnsavedChanges = False
                isClosing()
            End If
        Else
            MessageBox.Show("No server.properties file exists yet! This file is automatically generated on the first start of the server.")
        End If
    End Sub

    Private Sub Hyperlink_Click(sender As Object, e As RoutedEventArgs)
        If _DoClose() Then
            navpageWorld.GenerateNewWorld()
        End If
    End Sub

    Private Sub Hyperlink_Click_1(sender As Object, e As RoutedEventArgs)
        If _DoClose() Then
            MyMainWindow.navDashboard.SelectedIndex = 1
        End If
    End Sub

    Private Sub Hyperlink_Click_2(sender As Object, e As RoutedEventArgs)
        Dim m As New rconquerysettings()
        m.Owner = MyMainWindow
        m.ShowDialog()
    End Sub

    Private Sub Hyperlink_Click_3(sender As Object, e As RoutedEventArgs)
        If _DoClose() Then
            MyMainWindow.navDashboard.SelectedIndex = 2
        End If
    End Sub


#Region "ConfigPage"

    Public Overrides Function isHelpAvailable()
        Return True
    End Function

    Public Overrides Sub HelpClicked()
        System.Diagnostics.Process.Start("http://www.minecraftwiki.net/wiki/Server.properties")
    End Sub
#End Region

End Class