Module MyUserSettings

#Region "JVM -Xms -Xmx"

    ''' <summary>
    ''' The user defined -Xmx value
    ''' </summary>
    ''' 
    Property MaximumMemory() As String
        Get
            'Should end with M (MB) or G (GB)
            Return My.Settings.Startup_Memory
        End Get
        Set(value As String)
            My.Settings.Startup_Memory = value
            My.Settings.Save()
        End Set
    End Property

    ''' <summary>
    ''' The user defined -Xms value
    ''' </summary>
    Property MinimumMemory() As String
        Get
            'Should end with M (MB) or G (GB)
            Return My.Settings.Startup_MemoryMin
        End Get
        Set(value As String)
            My.Settings.Startup_MemoryMin = value
            My.Settings.Save()
        End Set
    End Property

    ''' <summary>
    ''' Returns the user defined -Xms or -Xmx value as an integer, converted to MB
    ''' </summary>
    Function JVM_Xm_paramter_AsInteger(Xms_or_Xmx As String) As Integer
        If Xms_or_Xmx.ToLower.EndsWith("g") Then ' is this parameter given in GB, as denoted by 'G' at end?
            'Strip any 'M' and 'G' from the string, and also convert from GB to MB
            Return Xms_or_Xmx.ToLower.Replace("g", "") * 1024
        Else
            'Strip any 'M' and 'G' from the string
            Return Xms_or_Xmx.ToLower.Replace("m", "")
        End If
    End Function

#End Region

#Region "JVM path + jarfile"

    ''' <summary>
    ''' Specifed path to the Java.exe executable. Dashboard defaults to "java" if empty.
    ''' </summary>
    Property JavaExec() As String
        Get
            Dim s As String = My.Settings.Startup_JavaExec
            Return s
        End Get
        Set(value As String)
            My.Settings.Startup_JavaExec = value
            My.Settings.Save()
        End Set
    End Property

    ''' <summary>
    ''' Stores the path to the jarfile, e.g. C:\Minecraft_server.jar
    ''' </summary>
    Property Jarfile() As String
        Get
            Return My.Settings.Jarfile
        End Get
        Set(value As String)
            My.Settings.Jarfile = value
            My.Settings.Save()
        End Set
    End Property

#End Region

#Region "Dashboard customisation"

    ''' <summary>
    ''' Default text editor
    ''' </summary>
    Property UserSettings_DefaultTextEditor() As String
        Get
            Dim s As String = My.Settings.UserSettings_DefaultTextEditor
            If s.Length = 0 Then
                'Set the default if blank
                My.Settings.UserSettings_DefaultTextEditor = "notepad"
                My.Settings.Save()
                Return "notepad"
            Else
                Return s
            End If

        End Get
        Set(value As String)
            My.Settings.UserSettings_DefaultTextEditor = value
            My.Settings.Save()
        End Set
    End Property

#End Region


    'TODO: Scheduler


End Module