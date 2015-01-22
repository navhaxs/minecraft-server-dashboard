Imports Newtonsoft.Json.Linq
Imports System.IO
Imports Newtonsoft.Json

''' <summary>
''' Edit the banned-ips, banned-players, ops and white-list
''' in either JSON (if existing), or otherwise *.txt to remain backwards compatible
''' </summary>
Public Class PlayerListEditor

    Public Enum PlayerListType
        OnlinePlayers = -2
        None = -1
        BannedIP = 0
        BannedPlayers = 1
        Ops = 2
        Whitelist = 3
    End Enum
    ' The associated filenames (and key value if JSON) for PlayerListType
    Const TXT_INDEX As Integer = 0
    Const JSON_INDEX As Integer = 1
    Const KEY_INDEX As Integer = 2
    Dim filename(,) As String = New String(,) {
            {"\banned-ips.txt", "\banned-ips.json", "ip"},
            {"\banned-players.txt", "\banned-players.json", "name"},
            {"\ops.txt", "\ops.json", "name"},
            {"\white-list.txt", "\whitelist.json", "name"}
        }


    'Public Class PlayerData

    '    Public Sub New(_name As String, Optional _ban_date As String = "", Optional _banned_by As String = "", Optional _banned_until As String = "", Optional _banned_reason As String = "")
    '        Name = _name
    '        ban_date = _ban_date
    '        banned_by = _banned_by
    '        banned_until = _banned_until                  
    '        banned_reason = _banned_reason
    '    End Sub

    '    Public Name As String = ""
    '    Public ban_date As String = ""
    '    Public banned_by As String = ""
    '    Public banned_until As String = ""
    '    Public banned_reason As String = ""

    'End Class

    Private thisFilename As String
    Private thisPlayerList As PlayerListType = PlayerListType.None
    Private thisJSONList As Object

    ''' <summary>
    ''' Load a player list file for manipulation
    ''' </summary>
    Public Sub New(type As PlayerListType)
        ' Get the file path of the requested config file from 'filename'

        thisPlayerList = type

        If (type < 0) Then
            ' Do nothing for 'virtual' player lists that aren't actual files on disk

        ElseIf My.Computer.FileSystem.FileExists(MyServer.MyStartupParameters.ServerPath & filename(type, JSON_INDEX)) Then
            thisFilename = MyServer.MyStartupParameters.ServerPath & filename(type, JSON_INDEX)

            ' Parse the JSON file
            Using r As StreamReader = New StreamReader(thisFilename)
                Dim json As String = r.ReadToEnd()
                thisJSONList = JArray.Parse(json)
            End Using
        Else
            thisFilename = MyServer.MyStartupParameters.ServerPath & filename(type, TXT_INDEX)
            If Not My.Computer.FileSystem.FileExists(MyServer.MyStartupParameters.ServerPath & filename(type, TXT_INDEX)) Then
                ' Create a new blank file
                System.IO.File.WriteAllText(My.Computer.FileSystem.CurrentDirectory & "\" & thisFilename, "")
            End If
        End If

    End Sub

    ''' <summary>
    '''Returns a list of all the key values
    ''' </summary>
    Public Function getListItems() As List(Of String)
        Dim items As New List(Of String)

        If (thisPlayerList < 0) Then
            ' Do nothing f

        ElseIf Not thisJSONList Is Nothing Then

            Dim key As String = filename(thisPlayerList, KEY_INDEX)

            For Each i In thisJSONList
                If i.Type = JTokenType.Object Then
                    If Not i(key) Is Nothing Then
                        items.Add(i(key))
                    End If
                End If
            Next

        ElseIf (My.Computer.FileSystem.FileExists(My.Computer.FileSystem.CurrentDirectory & "\" & thisFilename)) Then

            '============== Sample player list *.txt file produced by the server ==============
            '
            '# Updated 20/05/13 5:50 PM by Minecraft 1.5.1
            '# victim name | ban date | banned by | banned until | reason
            '

            Dim objReader As New System.IO.StreamReader(My.Computer.FileSystem.CurrentDirectory & "\" & thisFilename)

            Do Until objReader.EndOfStream
                ' Parse line by line
                Dim line As String = objReader.ReadLine

                ' Skip empty lines and comment
                If (Not line.StartsWith("#")) And (Not (line.Replace(" ", "").Length = 0)) Then

                    If line.IndexOf(" ") = -1 Then
                        ' If just one 'field'
                        items.Add(line)
                    Else
                        ' Strip of the rest and store just the first 'field'
                        items.Add(line.Substring(0, line.IndexOf("|")))
                    End If

                End If

            Loop
            objReader.Close()
        Else
            ' Return an empty list for a blank file
        End If

        Return items
    End Function

    ''' <summary>
    ''' Add a player to the list, and save changes
    ''' </summary>
    Public Function addPlayer(player As String) As Boolean
        'Server is running, simply send appropriate commands and the server do the rest
        If MyServer.ServerIsOnline Then
            Select Case thisPlayerList
                Case PlayerListType.BannedIP
                    MyServer.SendCommand("ban-ip " & player)
                Case PlayerListType.BannedPlayers
                    MyServer.SendCommand("ban " & player)
                Case PlayerListType.Ops
                    MyServer.SendCommand("op " & player)
                Case PlayerListType.Whitelist
                    MyServer.SendCommand("whitelist add " & player)
                Case Else
                    Throw New Exception("Cannot add item to a readonly playerlist")
            End Select

            'Reload player list
            navpagePlayers.ReloadPlayerLists(True)
        Else
            'Server is not running, must manually write to text file
            Try

                If Not thisJSONList Is Nothing Then
                    ' Process the json file
                    Dim m As New JObject
                    m.Add(filename(thisPlayerList, KEY_INDEX), player)

                    thisJSONList.Add(m)
                    saveJSONFile()

                Else
                    ' Process the txt file

                    '' Check if playername appears already in the file
                    'Using objReader As New System.IO.StreamReader(thisFilename)
                    '    Do Until objReader.EndOfStream
                    '        Dim line As String = objReader.ReadLine

                    '        'Skip line when it is either empty, or is a comment
                    '        If (Not line.StartsWith("#")) And (Not line.Replace(" ", "") = "") Then
                    '            If line.IndexOf(" ") = -1 Then
                    '                If line = player Then
                    '                    Return True ' Found match, don't need to write it again!
                    '                End If
                    '            Else
                    '                If line.Substring(0, line.IndexOf("|")) = player Then
                    '                    Return True ' Found match, don't need to write it again!
                    '                End If
                    '            End If

                    '        End If
                    '    Loop

                    '    objReader.Close()
                    'End Using

                    ' Append the new item
                    Using writer As System.IO.StreamWriter = System.IO.File.AppendText(thisFilename)
                        writer.WriteLine(player)
                        writer.Close()
                    End Using

                End If
            Catch ex As Exception
                MessageBox.Show("Error accessing " & thisFilename & vbNewLine & ex.Message)
                Return False
            End Try
        End If
        Return True
    End Function

    ''' <summary>
    ''' Removes all instance of the requested item from the list
    ''' </summary>
    Public Function removePlayer(player As String)
        If MyServer.ServerIsOnline Then
            'Server is running, simply send appropriate commands
            Select Case thisPlayerList
                Case PlayerListType.BannedIP
                    MyServer.SendCommand("pardon-ip " & player)
                Case PlayerListType.BannedPlayers
                    MyServer.SendCommand("pardon " & player)
                Case PlayerListType.Ops
                    MyServer.SendCommand("deop " & player)
                Case PlayerListType.Whitelist
                    MyServer.SendCommand("whitelist remove " & player)
            End Select

            'Reload player list
            navpagePlayers.ReloadPlayerLists(True)
        Else
            'Server is not running, must manually modify files
            Try

                If Not thisJSONList Is Nothing Then
                    Dim key As String = filename(thisPlayerList, KEY_INDEX)

                    Dim toDelete As New List(Of JObject)

                    For Each item In thisJSONList
                        If (item.Type = JTokenType.Object) And (Not item(key) Is Nothing) Then

                            If item(key) = player Then
                                toDelete.Add(item)
                            End If
                        End If
                    Next

                    For Each item In toDelete
                        item.Remove()
                    Next

                    saveJSONFile()
                Else



                    ' Search for item in file, and delete
                    Dim objReader As New System.IO.StreamReader(thisFilename)
                    Dim thisFileData As New List(Of String)
                    '                    StringBuilder sb = new StringBuilder();
                    'sb.Append("Some Stuff");
                    'sb.Append(var1);
                    'sb.Append(" more stuff");
                    'sb.Append(var2);
                    'sb.Append("other stuff");

                    Dim i As Integer = 0
                    Dim linetoremove As Integer = -1

                    Do Until objReader.EndOfStream
                        Dim line As String = objReader.ReadLine

                        thisFileData.Add(line)

                        If linetoremove = -1 Then
                            'Find and get the index of the entry with the variable containing 's'
                            If (Not line.StartsWith("#")) And (Not line.Replace(" ", "") = "") Then
                                If line.IndexOf(" ") = -1 Then
                                    If line = player Then
                                        ' Found match, store the current line position
                                        linetoremove = i
                                    End If
                                Else
                                    If line.Substring(0, line.IndexOf("|")) = player Then
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

                End If
            Catch ex As Exception
                Return False
            End Try
        End If
        Return True
    End Function

    Public ReadOnly Property isReadOnly As Boolean ' Some player lists are not writable files. Set isReadOnly flag.
        Get
            Select Case thisPlayerList
                Case PlayerListType.BannedIP
                    Return True ' Really??
                Case PlayerListType.None
                    Return True
                Case PlayerListType.OnlinePlayers
                    Return True
                Case Else
                    Return False
            End Select
        End Get
    End Property

    Public ReadOnly Property getPlayerlistType As PlayerListType
        Get
            Return thisPlayerList
        End Get
    End Property

    ' Write to disk
    Private Sub saveJSONFile()
        Dim json As String = JsonConvert.SerializeObject(thisJSONList)
        System.IO.File.WriteAllText(thisFilename, json)
    End Sub


End Class