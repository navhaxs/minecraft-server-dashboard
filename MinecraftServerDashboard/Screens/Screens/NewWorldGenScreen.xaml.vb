Public Class NewWorldGenScreen

    Dim thisConfigServerProp As New ConfigServerProp(Nothing)
    Dim FullDirNameToDelete As String

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

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
        Close()
    End Sub

End Class