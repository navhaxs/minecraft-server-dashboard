Public Class NewWorldGenScreen

    Dim thisConfigServerProp As New ConfigServerProp(Nothing)
    Dim FullDirNameToDelete As String

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Load any previously saved world generation settings
        Dim configreader As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)
        GetTextBoxStringOfProperty(levelseed, "level-seed", configreader)
        Try
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
        Catch ex As Exception
            ' Assuming no conf file exists yet
            ' TODO: FIXME Dashboard cannot save world gen fields to non-existent conf file!!
        End Try
    End Sub

    Private Sub btn_OK()
        ' Save world generation settings
        SetTextBoxStringOfProperty(levelseed, "level-seed")
        SetStringOfProperty(leveltype.SelectedValue.Content.ToString.ToUpper, "level-type")
        SetTextBoxStringOfProperty(generatorsettings, "generator-settings")
        On Error Resume Next
        Close()
    End Sub

End Class