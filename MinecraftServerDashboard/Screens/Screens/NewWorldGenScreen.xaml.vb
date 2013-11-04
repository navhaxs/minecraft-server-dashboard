Public Class NewWorldGenScreen

    Dim thisConfigServerProp As New ConfigServerProp(Nothing)
    Dim FullDirNameToDelete As String

    Sub New(Optional world_level_to_clear As String = Nothing)

        ' This call is required by the designer.
        InitializeComponent()

        ' Determine which folder is to be deleted
        If Not world_level_to_clear Is Nothing Then
            ' Load default world directory as the folder to clear
            FullDirNameToDelete = world_level_to_clear

            world_level_to_clear = My.Computer.FileSystem.GetDirectoryInfo(world_level_to_clear).Name

            lbloptional.Text = "for the profile """ & world_level_to_clear & """"
        Else
            Dim b As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)
            Dim s As String = b.ReturnConfigValue("level-name")
            If s = "" Then s = "word"
            FullDirNameToDelete = MyServer.MyStartupParameters.ServerPath & "\" & s
            world_level_to_clear = s
        End If

        ' Load any previously saved world generation settings
        Dim configreader As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)
        GetTextBoxStringOfProperty(levelseed, "level-seed", configreader)
        Select Case GetStringOfProperty("level-type", configreader).ToString.ToLower
            Case "default"
                leveltype.SelectedIndex = 0
            Case "flat"
                leveltype.SelectedIndex = 1
            Case "largebiomes"
                leveltype.SelectedIndex = 2
            Case Else
                leveltype.SelectedIndex = 0
        End Select
        GetTextBoxStringOfProperty(generatorsettings, "generator-settings", configreader)
    End Sub

    Private Sub Button_Click_1()
        ' Save world generation settings
        SetTextBoxStringOfProperty(levelseed, "level-seed")
        SetStringOfProperty(leveltype.SelectedValue.Content.ToString.ToUpper, "level-type")
        SetTextBoxStringOfProperty(generatorsettings, "generator-settings")
        On Error Resume Next
        ' Attempt to clear current world
        My.Computer.FileSystem.DeleteDirectory(FullDirNameToDelete, FileIO.DeleteDirectoryOption.DeleteAllContents)
        My.Computer.FileSystem.CreateDirectory(FullDirNameToDelete)
        MessageBox.Show("Success! This world will be automatically regenerated on the next start of the server.", "Reset world", MessageBoxButton.OK)
        Close()
    End Sub

    Sub Close() Handles btnCancel.Click
        MyMainWindow.FormControls.Children.Remove(Me)
        MyMainWindow.MyMainWindowProperties.MainWindowOverlay = MainWindowViewModel.OverlayShownType.None
        MyMainWindow.OverlayClosed()
        navpageWorld.RefreshPageData()
    End Sub
End Class