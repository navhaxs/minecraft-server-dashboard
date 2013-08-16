''' <summary>
''' Interact with the server.properties config file
''' </summary>
Public Class ServerProperties

    ''' <summary>
    ''' Retrive the value of the specified property
    ''' DO NOT INCLUDE THE = sign at the end
    ''' </summary>
    Public Function ReturnConfigValue(ByVal PropertyName As String) As Object
        Try

            Dim objReader As New System.IO.StreamReader(conffile)

            ' Read through the entire file line by line
            Do Until objReader.EndOfStream
                Dim line As String = objReader.ReadLine

                If line.StartsWith(PropertyName) Then
                    ' If this line is the required one, stop search and return result
                    Dim endstring As New Integer
                    endstring = line.IndexOf("=")

                    objReader.Close()
                    Return Microsoft.VisualBasic.Right(line, line.Length - (endstring + 1))
                    'End function at return statement.
                End If
            Loop
            objReader.Close()

            'Not found
            Return Nothing
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Update a property in server.properties with a new value
    ''' </summary>
    ''' <param name="propertyToMatch">Property to write to</param>
    ''' <param name="value">New value to write</param>
    Sub WriteConfigLine(ByVal propertyToMatch As String, ByVal value As String)

        Debug.Print(conffile)

        Dim wasmatchFound = False

        Debug.Print("BEGIN WriteConfigLine for property """ & propertyToMatch & """")
        Debug.Print("      OriginalValues.Count:""" & OriginalValues.Count & """")

        Dim fileWriter As New System.IO.StreamWriter(conffile)

        Dim linecount As Integer = 0

        ' Write out each line from stored memory
        Do While Not linecount = (OriginalValues.Count)
            Dim mystring As String = OriginalValues(linecount)
            Debug.Print("    Lineout: " & linecount)

            Dim endstring As New Integer
            endstring = mystring.IndexOf("=")

            Dim match As Boolean = False
            ' Check if this particular line is the line to change
            If endstring = -1 Then ' This line does not contain "=", but still matches
                If value = mystring Then
                    Debug.Print("             This line matches!" & linecount)
                    match = True
                End If
            ElseIf Microsoft.VisualBasic.Left(mystring, endstring) = propertyToMatch Then ' This line contains "=", and the suffix before the "=" matches
                Debug.Print("             This line matches!" & linecount)
                match = True
            End If

            If match Then
                ' Modify this line with the new value to update file with.
                wasmatchFound = True
                If endstring = -1 Then
                    fileWriter.WriteLine(value)
                Else
                    fileWriter.WriteLine(Microsoft.VisualBasic.Left(mystring, endstring) + "=" + value)
                End If
            Else
                fileWriter.WriteLine(mystring)
            End If
            linecount += 1
        Loop

        fileWriter.Close()

        ' If this property did not already exist, append it to end of file
        If wasmatchFound = False Then
            Debug.Print("WRITE OUT: " & propertyToMatch + "=" + value)

            Dim x As IO.StreamWriter = System.IO.File.AppendText(conffile)
            x.WriteLine(propertyToMatch + "=" + value)
            x.Close()
        End If
    End Sub

    Sub New(e As String)
        conffile = e
        OriginalValues.Clear()

        ' Check if the config file is non-existent
        If Not My.Computer.FileSystem.FileExists(conffile) Then
            If Not MyServer.MyStartupParameters.ServerPath Is Nothing Then
                Try
                    'Create a blank one for now (placeholder)
                    Dim NewBlankFile As New System.IO.StreamWriter(conffile)
                    NewBlankFile.Write("")
                    NewBlankFile.Flush()
                    NewBlankFile.Close()
                Catch ex As Exception

                End Try
            Else
                'Do nothing - user has not set up server yet!
                Exit Sub
            End If
        End If

        ' Store the file's initial contents in memory
        Try
            Dim objReader As New System.IO.StreamReader(conffile)
            Dim count As Integer = 0
            Do While objReader.Peek() <> -1
                Dim M As String = objReader.ReadLine()

                OriginalValues.Add(M)

                count += 1
            Loop
            objReader.Close()
        Catch ex As Exception

        End Try

    End Sub

    Dim conffile As String = ""
    Dim SelectedIndex As Integer
    Dim OriginalValues As New List(Of String)

    Dim ListBox1 As New ListBox

End Class