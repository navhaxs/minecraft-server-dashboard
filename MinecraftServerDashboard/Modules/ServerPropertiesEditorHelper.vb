Module ServerPropertiesEditorHelper
    ''' <summary>
    ''' Return the config value as a Boolean type
    ''' </summary>
    ''' <param name="x">CheckBox object</param>
    ''' <param name="propertyname">Server.properties Property</param>
    ''' <remarks></remarks>
    Sub GetBooleanValueOfProperty(x As CheckBox, propertyname As String, thisServerPropertiesFile As ServerProperties)
        Dim result As String = thisServerPropertiesFile.ReturnConfigValue(propertyname)
        If result Is Nothing Then
            'Property non-existant in config
            x.IsThreeState = True

            ' Set an indeterminate state (the square-thingy checkbox state)
            ' This will prevent the unintentional overwritting of a blank choice.
            x.IsChecked = Nothing
        ElseIf result.ToLower = "true" Then
            x.IsThreeState = False
            x.IsChecked = True
        ElseIf result.ToLower = "false" Then
            x.IsThreeState = False
            x.IsChecked = False
        ElseIf result.ToLower = "" Then
            'Property's value is empty in config
            x.IsThreeState = True
            x.IsChecked = Nothing
        Else
            MessageBox.Show("The property """ + propertyname + """ returned an unexpected value of:""" & result & """" _
                            & vbNewLine & vbNewLine & "CraftBukkit will only accept the values ""true"" or ""false""", "Error reading server.properties")
            x.IsThreeState = True
            x.IsChecked = Nothing
        End If
    End Sub

    Public Sub SetBooleanValueOfProperty(ByVal x As CheckBox, ByVal propertyname As String)
        ' If indeterminate state, don't write anything (ignore)
        If Not x.IsChecked Is Nothing Then
            Try
                Dim APIreader As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)
                APIreader.WriteConfigLine(propertyname, x.IsChecked.ToString.ToLower)
            Catch ex As Exception
            End Try
        End If
    End Sub

    Public Function GetStringOfProperty(propertyname As String, thisServerPropertiesFile As ServerProperties)
        Dim result As String = thisServerPropertiesFile.ReturnConfigValue(propertyname)
        Return result
    End Function
    Public Sub SetStringOfProperty(ByVal newvalue As String, ByVal propertyname As String)
        Try
            Dim APIreader As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)
            APIreader.WriteConfigLine(propertyname, newvalue)
        Catch ex As Exception
        End Try
    End Sub
    Public Sub GetTextBoxStringOfProperty(x As TextBox, propertyname As String, thisServerPropertiesFile As ServerProperties)
        Dim result As String = thisServerPropertiesFile.ReturnConfigValue(propertyname)
        x.Text = result
    End Sub
    Public Sub SetTextBoxStringOfProperty(ByVal x As TextBox, ByVal propertyname As String)
        Try
            Dim APIreader As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)
            APIreader.WriteConfigLine(propertyname, x.Text)
        Catch ex As Exception
        End Try
    End Sub
End Module