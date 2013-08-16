Public Class StartupParameters

    Property JavaParameters As String = " -client -Xmx" & MyAppSettings.UserSettings_SrvMaxMemory() & " -Xms" & MyAppSettings.UserSettings_SrvMinMemory()

    ' Set MyAppSettings.Jarfile to "DummyApp.exe" for testing.

    Property CB_ServerParameters As String = "--nojline nogui" '"nogui -d""yyyy-MM-dd HH:mm:ss"""

    ReadOnly Property FullParameters
        Get
            Return JavaParameters + " -jar " & Chr(34) & MyAppSettings.Jarfile & Chr(34) & " " & CB_ServerParameters
        End Get
    End Property

    ''' <summary>
    ''' Get the root folder of the Minecraft server
    ''' </summary>
    Public ReadOnly Property ServerPath As String
        Get
            If MyAppSettings.Jarfile = Nothing Then
                Return Nothing
            ElseIf Not My.Computer.FileSystem.FileExists(MyAppSettings.Jarfile) Then
                Return Nothing
            Else
                Return My.Computer.FileSystem.GetFileInfo(MyAppSettings.Jarfile).Directory.FullName
            End If
        End Get
    End Property

    ''' <summary>
    ''' Get full path to server.properties config file
    ''' </summary>
    Public ReadOnly Property ServerProperties As String
        Get
            If MyAppSettings.Jarfile = Nothing Then
                Return Nothing
            ElseIf Not My.Computer.FileSystem.FileExists(MyAppSettings.Jarfile) Then
                Return Nothing
            Else
                Return My.Computer.FileSystem.GetFileInfo(MyAppSettings.Jarfile).Directory.FullName & "\server.properties"
            End If
        End Get
    End Property
End Class