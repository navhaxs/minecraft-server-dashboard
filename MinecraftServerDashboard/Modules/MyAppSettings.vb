Namespace MyAppSettings

    Module MyAppSettings

#Region "Server set-up"
        ''' <summary>
        ''' (User setting) The max memory to allocate, in startup parameter format
        ''' </summary>
        Property UserSettings_SrvMaxMemory() As String
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
        ''' Return just the integer value of the Maximum memory allocation parameter
        ''' </summary>
        ReadOnly Property UserSettings_SrvMaxMemoryInt() As String
            Get
                If My.Settings.Startup_Memory.ToLower.EndsWith("g") Then ' is this parameter given in GB, as denoted by 'G' at end?
                    'Strip any 'M' and 'G' from the string, and also convert from GB to MB
                    Return My.Settings.Startup_Memory.ToLower.Replace("m", "").Replace("g", "") * 1024
                Else
                    'Strip any 'M' and 'G' from the string
                    Return My.Settings.Startup_Memory.ToLower.Replace("m", "").Replace("g", "")
                End If
            End Get
        End Property

        ''' <summary>
        ''' (User setting) The min memory to allocate, in startup parameter format
        ''' </summary>
        Property UserSettings_SrvMinMemory() As String
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
        ''' Return just the integer value of the Minimum memory allocation parameter
        ''' </summary>
        ReadOnly Property UserSettings_SrvMinMemoryInt() As String
            Get
                If My.Settings.Startup_MemoryMin.ToLower.EndsWith("g") Then ' is this parameter given in GB, as denoted by 'G' at end?
                    'Strip any 'M' and 'G' from the string, and also convert from GB to MB
                    Return My.Settings.Startup_MemoryMin.ToLower.Replace("m", "").Replace("g", "") * 1024
                Else
                    'Strip any 'M' and 'G' from the string
                    Return My.Settings.Startup_MemoryMin.ToLower.Replace("m", "").Replace("g", "")
                End If
            End Get
        End Property

        ''' <summary>
        ''' (User setting) Path to the Java executable. Defaults to "java" if unset.
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
        ''' (User setting) Path to the Java executable. Defaults to "java" if unset.
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
    End Module

End Namespace