Imports Microsoft.Win32

Namespace DetectJava

    Module DetectJava

        'http://stackoverflow.com/questions/3639129/c-sharp-how-do-you-get-the-operating-system-architecture-x86-or-x64
        Dim is64bit As Boolean = Not String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))

        ''' <summary>
        ''' Attempt to find the install path of whatever java version is installed
        ''' This method should work for 99% of people using this software :)
        ''' </summary>
        Function FindJavaInstallType()
            Dim e As String = FindPath()
            Dim r As String = ""

            Dim match As Boolean = False
            If e.Contains("\Java\jre7") Then
                r = "JRE 7"
                match = True
            ElseIf e.Contains("\Java\jre6") Then
                r = "JRE 6"
                match = True
            End If
            If Not match Then
                Return "none"
            End If

            'Append arch of Java
            If Not is64bit Then 'If x86 system, Java must obviously be x86 too
                r += " x86"
            ElseIf e.Contains("\Program Files (x86)\") Then
                r += " x86"
            ElseIf e.Contains("\Program Files\") Then
                r += " x64"
            Else
                Return "unknown"
            End If
            Return r
        End Function

        Function FindPath()
            'http://stackoverflow.com/questions/17821960/best-way-to-find-java-path-in-c-sharp
            Dim javaKey As [String] = "SOFTWARE\JavaSoft\Java Runtime Environment"
            Using baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(javaKey)
                Dim currentVersion As [String] = baseKey.GetValue("CurrentVersion").ToString()
                Using homeKey = baseKey.OpenSubKey(currentVersion)
                    Return homeKey.GetValue("JavaHome").ToString()
                End Using
            End Using
        End Function

    End Module


End Namespace
