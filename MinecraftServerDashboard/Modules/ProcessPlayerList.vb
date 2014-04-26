Public Module PlayerList

    Public Function ProcessPlayerList(input As String) As List(Of String)
        Dim PlayerList As New List(Of String)

        ' Remove [INFO] string if it exists
        ' e.g. 2013-07-09 16:46:13 [INFO] bearbear12345
        ' or:
        ' e.g. 2013-07-09 16:46:13 [INFO] [Minecraft-Server] bearbear12345
        Dim f As String
        If input.Contains("INFO]:") Then
            f = "INFO]: "
        ElseIf input.Contains("INFO] [Minecraft-Server] ") Then
            f = "INFO] [Minecraft-Server] "
        Else
            f = "INFO] "
        End If

        Dim i As Integer = input.IndexOf(f)
        Dim s As String = ""
        If input.Contains(f) Then
            If Not ((i + f.Length) = (input.Length - (i + f.Length))) Then
                s = input.Substring(i + f.Length, input.Length - (i + f.Length))
            Else
                s = ""
            End If
        Else
            s = input
        End If

        'If Not i + 8 = input.Length Then

        If Not s Is Nothing Then
            Dim words = s.Split({" "c}, StringSplitOptions.RemoveEmptyEntries)

            'Dim m As Integer
            For Each f In words
                'm += 1
                'Dim x As String = "17:19:53 [INFO] Connected players: "
                'Dim e As String = "2012-06-01 17:19:53 [INFO] Connected players: "
                If f.EndsWith(",") Then
                    PlayerList.Add(f.Substring(0, f.Length - 1))
                Else
                    PlayerList.Add(f)
                End If
            Next

        End If

        Return PlayerList
    End Function

End Module
