Imports System.Reflection

Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

#Region "http://social.msdn.microsoft.com/Forums/vstudio/en-US/ae6a3cc1-367d-4863-9976-a581a51d7d02/help-needed-with-reflectionembedded-reso"

    Private Sub Application_DispatcherUnhandledException(sender As Object, e As Windows.Threading.DispatcherUnhandledExceptionEventArgs) Handles Me.DispatcherUnhandledException
        CrashHelpMe(sender, e)
    End Sub

    '' !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    '' NOTE: The following code was taken from http://social.msdn.microsoft.com/Forums/vstudio/en-US/ae6a3cc1-367d-4863-9976-a581a51d7d02/help-needed-with-reflectionembedded-reso
    '' Allows all DLLs to be combined within the app's single EXE
    '' !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles MyBase.Startup

        Dim assemblies = New Dictionary(Of String, Assembly)()
        Dim executingAssembly = Assembly.GetExecutingAssembly()
        Dim resources = executingAssembly.GetManifestResourceNames().Where(Function(n) n.EndsWith(".dll"))

        For Each resource As String In resources
            Using stream = executingAssembly.GetManifestResourceStream(resource)
                If stream Is Nothing Then
                    Continue For
                End If

                Dim bytes = New Byte(stream.Length - 1) {}
                stream.Read(bytes, 0, bytes.Length)
                Try
                    assemblies.Add(resource, Assembly.Load(bytes))
                Catch ex As Exception
                    System.Diagnostics.Debug.Print(String.Format("Failed to load: {0}, Exception: {1}", resource, ex.Message))
                End Try
            End Using
        Next

        AddHandler AppDomain.CurrentDomain.AssemblyResolve,
         Function(sx As Object, args As System.ResolveEventArgs) As System.Reflection.Assembly

             Dim parentAssembly As Assembly = Assembly.GetExecutingAssembly()

             Dim name = args.Name.Substring(0, args.Name.IndexOf(","c)) & ".dll"
             Dim resourceName = parentAssembly.GetManifestResourceNames().First(Function(s) s.EndsWith(name))

             Using stream As IO.Stream = parentAssembly.GetManifestResourceStream(resourceName)
                 Dim block As Byte() = New Byte(stream.Length - 1) {}
                 stream.Read(block, 0, block.Length)
                 Return Assembly.Load(block)
             End Using
         End Function

    End Sub
#End Region
End Class