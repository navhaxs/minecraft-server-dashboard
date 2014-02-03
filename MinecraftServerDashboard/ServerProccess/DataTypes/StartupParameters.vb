Public Class StartupParameters

    ''' <summary>
    ''' Returns the complete command line parameters to launch Java with from Dashboard's configuration
    ''' </summary>
    Public ReadOnly Property FullParameters
        Get
            ' ====== Java launch parameters ======
            '       -client             http://stackoverflow.com/questions/198577/real-differences-between-java-server-and-java-client
            '                           Note: The Server JVM is only included in the JDK download, NOT JRE
            '       --nojline           Required to redirect console output into Dashboard on CraftBukkit
            '       nogui               Required to redirect console output into Dashboard on Vanilla
            Dim JavaParameters As String = " -client -Xmx" & MyUserSettings.MaximumMemory() & " -Xms" & MyUserSettings.MinimumMemory()
            Return JavaParameters + " -jar " & Chr(34) & MyUserSettings.Jarfile & Chr(34) & " " & My.Settings.JarLaunchArguments & " --nojline nogui"
        End Get
    End Property

    ''' <summary>
    ''' Get the root folder of the Minecraft server
    ''' </summary>
    Public ReadOnly Property ServerPath As String
        Get
            If MyUserSettings.Jarfile = Nothing Then
                Return Nothing
            ElseIf Not My.Computer.FileSystem.FileExists(MyUserSettings.Jarfile) Then
                Return Nothing
            Else
                Return My.Computer.FileSystem.GetFileInfo(MyUserSettings.Jarfile).Directory.FullName
            End If
        End Get
    End Property

    ''' <summary>
    ''' Get full path to server.properties config file
    ''' </summary>
    Public ReadOnly Property ServerProperties As String
        Get
            If MyUserSettings.Jarfile = Nothing Then
                Return Nothing
            ElseIf Not My.Computer.FileSystem.FileExists(MyUserSettings.Jarfile) Then
                Return Nothing
            Else
                Return My.Computer.FileSystem.GetFileInfo(MyUserSettings.Jarfile).Directory.FullName & "\server.properties"
            End If
        End Get
    End Property
End Class