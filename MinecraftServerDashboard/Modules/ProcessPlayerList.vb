Imports System.Text.RegularExpressions

Public Module PlayerList
    Const HEADERCHARS As String = "[:.$]" 'Strip off the list header - e.g. "Developers:" (Essentials plugin)
    Const VALIDCHARS As String = "[^abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_.$]"

    Public Function ProcessPlayerList(consoleOutput As String) As List(Of Player)
        Dim playerList As New List(Of Player)

        Dim s As String = ""

        'First, strip of any prefixing "[INFO]" tag strings if required
        '           e.g. 2013-07-09 16:46:13 [INFO] bearbear12345
        '                or
        '           e.g. 2013-07-09 16:46:13 [INFO] [Minecraft-Server] bearbear12345
        Dim prefix As String
        If consoleOutput.Contains("INFO]:") Then
            prefix = "INFO]: "
        ElseIf consoleOutput.Contains("INFO] [Minecraft-Server] ") Then
            prefix = "INFO] [Minecraft-Server] "
        Else
            prefix = "INFO] "
        End If

        Dim i As Integer = consoleOutput.IndexOf(prefix)

        If consoleOutput.Contains(prefix) Then
            If Not ((i + prefix.Length) = (consoleOutput.Length - (i + prefix.Length))) Then
                s = consoleOutput.Substring(i + prefix.Length, consoleOutput.Length - (i + prefix.Length))
            Else
                s = ""
            End If
        Else
            s = consoleOutput
        End If

        'Break up the string into its separate words
        Dim wordList As New List(Of String)
        If Not s Is Nothing Then
            Dim words = s.Split({" "c}, StringSplitOptions.RemoveEmptyEntries)

            For Each prefix In words
                'Remove any trailing comma
                If prefix.EndsWith(",") Then
                    wordList.Add(prefix.Substring(0, prefix.Length - 1))
                Else
                    wordList.Add(prefix)
                End If
            Next

        End If

        'Filter through words with invalid characters, until the next proper username is found.
        'Combine this proper username with the current set of invalid words ('tags'), and add this. Repeat.
        Dim r As Regex = New Regex(VALIDCHARS)
        Dim h As Regex = New Regex(HEADERCHARS)
        Dim curr As String = ""

        For Each item In wordList
            If (r.IsMatch(item)) Then
                ' validation failed
                If Not h.IsMatch(item) Then ' Ignore "headers"
                    curr &= item & " "
                End If
            Else
                playerList.Add(New Player(curr & item, item))
                curr = ""
            End If

        Next

        Return playerList
    End Function

End Module
