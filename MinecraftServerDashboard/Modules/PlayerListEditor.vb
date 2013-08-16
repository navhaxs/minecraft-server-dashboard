Public Class PlayerListEditor

    Dim thisFilename As String
    Public thisPlayerList As PlayerListType = PlayerListType.None
    Public ReadOnly Property isReadOnly As Boolean ' Some player lists are not writable files. Set isReadOnly flag.
        Get
            Select Case thisPlayerList
                Case PlayerListType.BannedIP
                    Return True
                Case PlayerListType.None
                    Return True
                Case PlayerListType.OnlinePlayers
                    Return True
                Case Else
                    Return False
            End Select
        End Get
    End Property

    '============== Sample player list *.txt file produced by the server ==============
    '
    '# Updated 20/05/13 5:50 PM by Minecraft 1.5.1
    '# victim name | ban date | banned by | banned until | reason
    '

    Function GetPlayersFrom(filename As String) As List(Of PlayerData)
        thisFilename = filename
        Dim playerlist As New List(Of PlayerData)

        If Not My.Computer.FileSystem.FileExists(thisFilename) Then
            Return playerlist
        End If

        If thisFilename.ToLower.EndsWith("banned-ips.txt") Then
            thisPlayerList = PlayerListType.BannedIP
        ElseIf thisFilename.ToLower.EndsWith("banned-players.txt") Then
            thisPlayerList = PlayerListType.BannedPlayers
        ElseIf thisFilename.ToLower.EndsWith("ops.txt") Then
            thisPlayerList = PlayerListType.Ops
        ElseIf thisFilename.ToLower.EndsWith("white-list.txt") Then
            thisPlayerList = PlayerListType.Whitelist
        End If

        Try
            Dim objReader As New System.IO.StreamReader(thisFilename)

            Do Until objReader.EndOfStream
                Dim line As String = objReader.ReadLine

                'Skip line when it is either empty, or is a comment
                If (Not line.StartsWith("#")) And (Not (line.Replace(" ", "").Length = 0)) Then

                    If line.IndexOf(" ") = -1 Then
                        playerlist.Add(New PlayerData(line))
                    Else
                        playerlist.Add(New PlayerData(line.Substring(0, line.IndexOf("|"))))
                    End If

                End If

            Loop
            objReader.Close()
        Catch ex As Exception
            MessageBox.Show("")
        End Try
        Return playerlist
    End Function

    Function AddPlayer(s As String)

        If MyServer.ServerIsOnline Then
            'Server is running, simply send appropriate commands
            Select Case thisPlayerList
                Case PlayerListType.BannedIP
                    MyServer.SendCommand("ban-ip " & s)
                Case PlayerListType.BannedPlayers
                    MyServer.SendCommand("ban " & s)
                Case PlayerListType.Ops
                    MyServer.SendCommand("op " & s)
                Case PlayerListType.Whitelist
                    MyServer.SendCommand("whitelist add " & s)
            End Select

            'Reload player list
            navpagePlayers.ReloadPlayerLists(True)
        Else
            'Server is not running, must manually write to text file
            Try
                ' Check if playername appears already in the file
                Dim objReader As New System.IO.StreamReader(thisFilename)

                Do Until objReader.EndOfStream
                    Dim line As String = objReader.ReadLine

                    'Skip line when it is either empty, or is a comment
                    If (Not line.StartsWith("#")) And (Not line.Replace(" ", "") = "") Then
                        If line.IndexOf(" ") = -1 Then
                            If line = s Then
                                Return True ' Found match, don't need to write it again!
                            End If
                        Else
                            If line.Substring(0, line.IndexOf("|")) = s Then
                                Return True ' Found match, don't need to write it again!
                            End If
                        End If

                    End If
                Loop

                objReader.Close()
                'objReader.Dispose()

                Dim objWriter As System.IO.StreamWriter = System.IO.File.AppendText(thisFilename)

                objWriter.WriteLine(s)

                objWriter.Close()
                'objWriter.Dispose()
            Catch ex As Exception
                MessageBox.Show("Error reading from " & thisFilename & vbNewLine & ex.Message)
                Return False
            End Try
        End If
        Return True
    End Function

    Function RemovePlayer(s As String)
        If MyServer.ServerIsOnline Then
            'Server is running, simply send appropriate commands
            Select Case thisPlayerList
                Case PlayerListType.BannedIP
                    MyServer.SendCommand("pardon-ip " & s)
                Case PlayerListType.BannedPlayers
                    MyServer.SendCommand("pardon " & s)
                Case PlayerListType.Ops
                    MyServer.SendCommand("deop " & s)
                Case PlayerListType.Whitelist
                    MyServer.SendCommand("whitelist remove " & s)
            End Select

            'Reload player list
            navpagePlayers.ReloadPlayerLists(True)
        Else
            'Server is not running, must manually modify text file
            Try
                ' Look for where 's' appears in this file, and read this file into memory
                Dim objReader As New System.IO.StreamReader(thisFilename)
                Dim thisFileData As New List(Of String)

                Dim i As Integer = 0
                Dim linetoremove As Integer = -1

                Do Until objReader.EndOfStream
                    Dim line As String = objReader.ReadLine

                    thisFileData.Add(line)

                    If linetoremove = -1 Then
                        'Find and get the index of the entry with the variable containing 's'
                        If (Not line.StartsWith("#")) And (Not line.Replace(" ", "") = "") Then
                            If line.IndexOf(" ") = -1 Then
                                If line = s Then
                                    ' Found match, store the current line position
                                    linetoremove = i
                                End If
                            Else
                                If line.Substring(0, line.IndexOf("|")) = s Then
                                    ' Found match, store the current line position
                                    linetoremove = i
                                End If
                            End If
                        End If
                    End If

                    i = i + 1
                Loop

                objReader.Close()
                'objReader.Dispose()

                'Begine writing new file; skipping line to remove
                Dim objWriter As New System.IO.StreamWriter(thisFilename)

                i = 0
                For Each thisline In thisFileData
                    If Not i = linetoremove Then
                        objWriter.WriteLine(thisline)
                    End If
                    i += 1
                Next

                objWriter.Close()
                'objWriter.Dispose()
            Catch ex As Exception
                Return False
            End Try
        End If
        Return True
    End Function

    Enum PlayerListType As Integer

        None = -1

        OnlinePlayers = 0

        BannedIP = 1

        BannedPlayers = 2

        Ops = 3

        Whitelist = 4

    End Enum

    Public Class PlayerData

        Public Sub New(_name As String, Optional _ban_date As String = "", Optional _banned_by As String = "", Optional _banned_until As String = "", Optional _banned_reason As String = "")
            Name = _name
            ban_date = _ban_date
            banned_by = _banned_by
            banned_until = _banned_until
            banned_reason = _banned_reason
        End Sub

        Public Name As String = ""
        Public ban_date As String = ""
        Public banned_by As String = ""
        Public banned_until As String = ""
        Public banned_reason As String = ""

    End Class
End Class