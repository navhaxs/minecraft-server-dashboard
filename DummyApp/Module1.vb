''' <summary>
''' This application emulates the server console output, used for testing.
''' </summary>
''' <remarks></remarks>
Module Module1

    Sub Main()
        DoRandomErrorTextRightNow("StartupText.txt")
        Console.WriteLine(TimeNow() & " [INFO] Done (4.510s)! For help, type ""help"" or ""?""")
        Console.WriteLine(TimeNow() & " [WARNING] **** FAILED TO BIND TO PORT!")
        DoRandomErrorTextRightNow("ErrorText.txt")
        ExitTimer.Interval = 5000
        ExitTimer.Start()
        While True
            Select Case Console.ReadLine()
                Case "stop"
                    End
                Case "doerror"
                    DoRandomErrorTextRightNow("ErrorText.txt")
                Case "list"
                    Console.WriteLine(TimeNow() & " [INFO] Players...")
                    Console.WriteLine(TimeNow() & " [INFO] Players...")
            End Select
            System.Threading.Thread.Sleep("1000")
        End While
    End Sub

    Function TimeNow()
        Return Microsoft.VisualBasic.Left(Date.Now.TimeOfDay.ToString, 8)
    End Function

    Sub DoRandomErrorTextRightNow(filename As String)
        Dim objReader As New System.IO.StreamReader(filename)

        Do Until objReader.EndOfStream
            Dim line As String = objReader.ReadLine
            System.Threading.Thread.Sleep(CInt(Math.Ceiling(Rnd() * 10)))
            Console.WriteLine(TimeNow() & " " & line)
        Loop
    End Sub

    Dim WithEvents ExitTimer As New Timers.Timer

    Private Sub ExitTimer_Elapsed(sender As Object, e As Timers.ElapsedEventArgs) Handles ExitTimer.Elapsed
        End
    End Sub
End Module