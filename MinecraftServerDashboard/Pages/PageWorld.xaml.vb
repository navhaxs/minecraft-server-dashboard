Imports System.Windows.Forms

Class pageWorld

    ' Keep a list of directories, including information about last Write Time (the last time the world data was saved)
    Dim MyProfileDirs As New List(Of ProfileDir)

    ' Each directory is represented by a ProfileDir
    Class ProfileDir

        Sub New(thisDirName As String)
            DirName = thisDirName
        End Sub

        Private _DirName As String 'Full path to directory
        Private _DisplayName As String 'Display name does not include the full path
        Public Property DirName As String
            Get
                Return _DirName
            End Get
            Set(value As String)
                _DirName = value
                Dim s As String = My.Computer.FileSystem.GetDirectoryInfo(value).Name

                If My.Computer.FileSystem.GetDirectoryInfo(value).GetFiles.Length = 0 Then
                    s = s
                Else

                    s = s + " (Last saved " + My.Computer.FileSystem.GetDirectoryInfo(value).LastWriteTime + ")"
                End If

                _DisplayName = s
            End Set
        End Property

        ' Different methods of presenting the directory's name:

        Public ReadOnly Property DisplayNameWithDate As String
            Get
                Return _DisplayName
            End Get
        End Property

        Public ReadOnly Property DisplayNameOnly As String
            Get
                Return My.Computer.FileSystem.GetDirectoryInfo(_DirName).Name
            End Get
        End Property

        Public ReadOnly Property FullDirName As String
            Get
                Return _DirName
            End Get
        End Property

        Public ReadOnly Property DisplayDate As String
            Get
                Return My.Computer.FileSystem.GetDirectoryInfo(_DirName).CreationTime
            End Get
        End Property

    End Class

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.DataContext = MyServer
    End Sub

    Sub RefreshPageData()
        'Get current world name
        Dim b As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)
        Dim s As String = b.ReturnConfigValue("level-name")

        If Not s Is Nothing Then
            If s = "" Then s = "world"
            txtCurrentWorld.Text = s

            Dim p As String = MyServer.MyStartupParameters.ServerPath & "\" & s

            txtCurrentWorldLocation.Text = My.Computer.FileSystem.GetDirectoryInfo(p).FullName

            'Get world data size
            If My.Computer.FileSystem.DirectoryExists(p) Then
                Dim size As Decimal
                For Each file In My.Computer.FileSystem.GetDirectoryInfo(p).GetFiles("*", IO.SearchOption.AllDirectories)
                    size += file.Length
                Next
                size = size / (1024 ^ 2)

                txtCurrentWorldSize.Text = Decimal.Round(size, 2) & " MB"
            Else
                txtCurrentWorldSize.Text = "Empty"
            End If

        End If

        'worldsnapshots
        MyProfileDirs.Clear()
        If My.Computer.FileSystem.DirectoryExists(MyServer.MyStartupParameters.ServerPath) Then
            For Each i In My.Computer.FileSystem.GetDirectories(MyServer.MyStartupParameters.ServerPath)
                ' ie. Include all three folders:
                '               world
                '               world_nether
                '               world_the_end
                ' and include them as a set, where only the 'primary' folder is shown (ie. just show "world" only)
                If (Not MyUserSettings.settingsStore.ProfileDir_ExcludedDirectories.Contains(My.Computer.FileSystem.GetDirectoryInfo(i).Name)) _
                    And (Not My.Computer.FileSystem.GetDirectoryInfo(i).Name.EndsWith("_nether")) _
                    And (Not My.Computer.FileSystem.GetDirectoryInfo(i).Name.EndsWith("_the_end")) Then
                    MyProfileDirs.Add(New ProfileDir(i))
                End If
            Next
            ListBox_MyWorldProfiles.IsEnabled = True
        Else
            ListBox_MyWorldProfiles.IsEnabled = False
        End If

        ' Update UI with the profiles
        ListBox_MyWorldProfiles.Items.Clear()
        Dim index As Integer = 0
        Dim selindex As Integer = -1
        For Each i As ProfileDir In MyProfileDirs
            If i.DisplayNameOnly = s Then
                ' Append (Active) to the world currently selected in the game settings
                ListBox_MyWorldProfiles.Items.Add(i.DisplayNameWithDate & " (Active)")
                selindex = index
            Else
                ListBox_MyWorldProfiles.Items.Add(i.DisplayNameWithDate)
            End If
            index += 1
        Next

        On Error Resume Next
        ListBox_MyWorldProfiles.SelectedIndex = selindex

        ListBox_MyWorldProfiles_SelectionChanged()
    End Sub

    Friend Sub ListBox_MyWorldProfiles_SelectionChanged() Handles ListBox_MyWorldProfiles.SelectionChanged
        ' Toggle whether buttons can be clicked depending on the selection

        If Not ListBox_MyWorldProfiles.SelectedIndex = -1 Then

            btnDeleteProfile.IsEnabled = True

            'Don't allow "Set as active" for the already active world
            btnSwitchTo.IsEnabled = Not ListBox_MyWorldProfiles.SelectedItem.ToString.EndsWith("(Active)")
            
            'Don't allow the deletion of the currently active (running) world
            If ListBox_MyWorldProfiles.SelectedItem.ToString.EndsWith("(Active)") Then
                btnDeleteProfile.IsEnabled = Not MyServer.ServerIsOnline
            End If

        Else
            'Nothing selected
            btnSwitchTo.IsEnabled = False
            btnDeleteProfile.IsEnabled = False
        End If
    End Sub

#Region "UI Button actions"

    ''' <summary>
    ''' Initiate a restart of the game server
    ''' </summary>
    Private Sub RestartButton_Click(sender As Object, e As RoutedEventArgs)
        sender.Visibility = Windows.Visibility.Collapsed
        MyServer.RestartServer(True)
    End Sub

    ''' <summary>
    ''' Display new profile creation dialog
    ''' </summary>
    Public Sub NewProfile()
        Using l As New NewWorldProfile
            Dim x As New OverlayDialog
            x.DisplayConfig(l)
            MyMainWindow.MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.None
        End Using
    End Sub

    ''' <summary>
    ''' Mark the selected world as 'active' in the game settings
    ''' </summary>
    Private Sub SetAsActive()
        Dim b As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)
        b.WriteConfigLine("level-name", MyProfileDirs(ListBox_MyWorldProfiles.SelectedIndex).DisplayNameOnly)
        If MyServer.ServerIsOnline Then
            BannerRequireRestart.Visibility = Windows.Visibility.Visible
        End If
        RefreshPageData()
    End Sub

    ''' <summary>
    ''' Display new world re-create dialog for the active profile
    ''' </summary>
    Public Sub GenerateNewWorld()
        Using l As New NewWorldGenScreen
            Dim x As New OverlayDialog
            x.DisplayConfig(l)
            MyMainWindow.MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.None
        End Using
    End Sub

    ''' <summary>
    ''' Display new world backup dialog
    ''' </summary>
    Public Sub DoBackupScreen()
        Dim b As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)
        Dim thislevelname As String = b.ReturnConfigValue("level-name")

        'Check if there is anything to backup
        If Not My.Computer.FileSystem.DirectoryExists(MyServer.MyStartupParameters.ServerPath & "\" & thislevelname) Then
            MessageBox.Show("Sorry, there is nothing to backup - the current world data is either empty, or was not found. The world data will be automatically generated once the server is started for the first time")
        Else

            Using e As New DoBackupScreen(False)
                Dim M As New OverlayDialog
                M.DisplayConfig(e)
                e.txtFilename.Focus()
            End Using

        End If
        RefreshPageData()

    End Sub

    ''' <summary>
    ''' Display the world backup manager
    ''' </summary>
    Sub btnOpenBackups_Click() Handles btnOpenBackups.Click
        Dim s As New SuperOverlay
        Dim m As New ConfigMyWorldBackups(s)
        m.RefreshPageData()
        navPageCBconfig.ShowConfigPage(m, s)
    End Sub

    ''' <summary>
    ''' Delete the selected profile
    ''' </summary>
    Private Sub DeleteWorld_Click(sender As Object, e As RoutedEventArgs)
        Dim s As Integer = ListBox_MyWorldProfiles.SelectedIndex

        Dim _m As New ConfirmWorldDeletion
        _m.Owner = MyMainWindow
        If Not _m.ShowMessage(MyProfileDirs(s).DirName) Then Exit Sub

        Try
            My.Computer.FileSystem.DeleteDirectory(MyProfileDirs(s).DirName, FileIO.UIOption.AllDialogs, FileIO.RecycleOption.SendToRecycleBin)
            ListBox_MyWorldProfiles.Items.RemoveAt(s)
            MyProfileDirs.RemoveAt(s)
        Catch ex As Exception
            ' The delete operation was cancelled by the user from Windows Explorer
        End Try
        RefreshPageData()
    End Sub

    '' <summary>
    '' Backup selected profile (Not implemented)
    '' </summary>
    'Private Sub Button_Click_2(sender As Object, e As RoutedEventArgs)
    '    Dim m As New SaveFileDialog
    '    With m
    '        .DefaultExt = ".zip"
    '        .AddExtension = True
    '        .InitialDirectory = My.Computer.FileSystem.SpecialDirectories.Desktop

    '    End With

    '    If m.ShowDialog = DialogResult.OK Then
    '        Compress(MyProfileDirs(ListBox_MyWorldProfiles.SelectedIndex).DirName, MyProfileDirs(ListBox_MyWorldProfiles.SelectedIndex).DisplayNameOnly, m.FileName)
    '    End If
    'End Sub

    Private Sub btnOpenProfiles_Click(sender As Object, e As RoutedEventArgs) Handles btnOpenProfiles.Click
        navPageCBconfig.Go_ExploreSreverDirectory_inWindowsExplorer()
    End Sub

    Private Sub Hyperlink_Click(sender As Object, e As RoutedEventArgs)
        RefreshPageData()
    End Sub

#End Region


End Class