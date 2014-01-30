Imports Microsoft.Win32

Namespace DetectJava

    Module DetectJava

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
                Return "Java not found or installed"
            End If

            'Append arch of Java
            If Not System.Environment.Is64BitOperatingSystem Then 'If x86 system, Java must obviously be x86 too
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

            Dim baseKey As Microsoft.Win32.RegistryKey
            Dim result As String

            ' Check for a 64 bit installation if Windows is 64 bit
            If System.Environment.Is64BitOperatingSystem Then
                baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(javaKey)
                    result = ScanForJre(baseKey)
                    If Not result Is "" Then
                        Return result
                    End If
            End If

            ' Otherwise, continue to check for a 32 bit installation
            baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(javaKey)
            result = ScanForJre(baseKey)
            If Not result Is "" Then
                Return result
            End If

            ' Lastly, return nothing if nothing was found
            Return ""
        End Function

        Private Function ScanForJre(baseKey As Microsoft.Win32.RegistryKey) As String
            If baseKey Is Nothing Then
                Return ""
            End If
            Dim currentVersion As [String] = baseKey.GetValue("CurrentVersion", "").ToString()
            If currentVersion Is "" Then
                Return ""
            End If
            Using homeKey = baseKey.OpenSubKey(currentVersion)
                Return homeKey.GetValue("JavaHome", "").ToString()
            End Using
        End Function

    End Module


End Namespace
