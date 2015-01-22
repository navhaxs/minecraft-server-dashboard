Imports System.IO
Imports System.Web.Script.Serialization

Namespace MyUserSettings

    Module MyUserSettings

        Public settingsStore As MySettingsConfig
        Public tasksStore As MyTasksConfig

        Public Sub Load()
            settingsStore = MySettingsConfig.Load()
            tasksStore = MyTasksConfig.Load("tasks.jsn")
        End Sub

        Public Sub Save()
            ' Save the current list of scheduled tasks
            tasksStore.TaskList.Clear()
            For Each task In navpageScheduler.ListOfSchedulerTaskItem
                tasksStore.TaskList.Add(task.Task)
            Next
            Try
                settingsStore.Save()
                tasksStore.Save("tasks.jsn")
            Catch ex As Exception
                MessageBox.Show("Could not write the Dashboard configuration to disk. Make sure you extracted Dashboard!")
            End Try
            
        End Sub

#Region "Dashboard customisation"

        ''' <summary>
        ''' Default text editor
        ''' </summary>
        Property UserSettings_DefaultTextEditor() As String
            Get
                Dim s As String = MyUserSettings.settingsStore.UserSettings_DefaultTextEditor
                If s.Length = 0 Then
                    'Set the default if blank
                    MyUserSettings.settingsStore.UserSettings_DefaultTextEditor = "notepad"
                    Return "notepad"
                Else
                    Return s
                End If

            End Get
            Set(value As String)
                MyUserSettings.settingsStore.UserSettings_DefaultTextEditor = value
            End Set
        End Property

#End Region

        Public Function MaxMemoryInMB() As String
            Dim MaxMemInMB As String
            If MyUserSettings.settingsStore.Startup_Memory.ToUpper.EndsWith("G") Then
                MaxMemInMB = MyUserSettings.settingsStore.Startup_Memory.Substring(0, MyUserSettings.settingsStore.Startup_Memory.Length - 1) * 1024
            ElseIf MyUserSettings.settingsStore.Startup_Memory.ToUpper.EndsWith("M") Then
                MaxMemInMB = MyUserSettings.settingsStore.Startup_Memory.Substring(0, MyUserSettings.settingsStore.Startup_Memory.Length - 1)
            Else
                MaxMemInMB = MyUserSettings.settingsStore.Startup_Memory.Substring(0, MyUserSettings.settingsStore.Startup_Memory.Length)
            End If
            Return MaxMemInMB
        End Function

        ' dashboard.jsn
        Public Class MySettingsConfig
            Inherits AppSettings(Of MySettingsConfig)
            Public App_SuppressMinimiseMessage As String = ""
            Public Startup_Memory As String = "1G" ' TODO: Change to MB (integer)
            Public Startup_MemoryMin As String = "512M" ' TODO: Change to MB (integer)
            Public Startup_JavaExec As String = ""
            Public UserSettings_DefaultTextEditor As String = ""
            Public Jarfile As String = ""
            Public LaunchArgu_JAVA As String = ""
            Public JarLaunchArguments As String = ""
            Public Startup_JavaSpecificArgs As String = ""
            Public ProfileDir_ExcludedDirectories As New List(Of String)() From {"world-backups",
                                                                 "plugins",
                                                                 "crash-reports",
                                                                 "logs",
                                                                 "config",
                                                                 "libraries",
                                                                 "mods"}
        End Class

        ' tasks.jsn
        Public Class MyTasksConfig
            Inherits AppSettings(Of MyTasksConfig)

            Public schemaVersion As Integer = 1

            Public TaskList As New List(Of TaskScheduler.Task)

        End Class

    End Module

End Namespace

' Store the app's settings in a local file, instead of the previous My.Settings implementation
' To allow saving the scheduled tasks, and simplifies the storage for multiple instances of settings
' http://stackoverflow.com/questions/8688724/how-to-store-a-list-of-objects-in-application-settings
Public Class AppSettings(Of T As New)

    'Serialize app settings to JSON format
    Public Sub Save(Optional fileName As String = DEFAULT_CONFIG_FILENAME)
        File.WriteAllText(fileName, (New JavaScriptSerializer()).Serialize(Me))
    End Sub

    Public Shared Sub Save(pSettings As T, Optional fileName As String = DEFAULT_CONFIG_FILENAME)
        File.WriteAllText(fileName, (New JavaScriptSerializer()).Serialize(pSettings))
    End Sub

    Public Shared Function Load(Optional fileName As String = DEFAULT_CONFIG_FILENAME) As T
        Dim t As New T()
        If File.Exists(fileName) Then
            t = (New JavaScriptSerializer()).Deserialize(Of T)(File.ReadAllText(fileName))
        End If
        Return t
    End Function
End Class