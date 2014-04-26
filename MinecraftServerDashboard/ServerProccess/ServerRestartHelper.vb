Module ServerRestartHelper

    ''' <summary>
    ''' Waits for the server to stop, then start it again
    ''' </summary>
    ''' <param name="background">Set False to prevent blocking code (ie. do a restart in the background)</param>
    Function RestartServer(background As Boolean)
        With MyServer
            If Not background Then
                Try
                    .StopServer()
                    .ServerProc.WaitForExit()
                    .StartServer()
                    Return True
                Catch ex As Exception
                    Return False
                End Try
            Else

                ' Initialise the restart job to run in the background
                Dim RestartSrvThread As New System.Threading.Thread( _
                  New System.Threading.ParameterizedThreadStart( _
                      AddressOf DoBackgroundRestartServer)) With { _
                          .IsBackground = True _
                              }
                RestartSrvThread.Start()
                Return True

            End If
        End With
    End Function

    Private Sub DoBackgroundRestartServer()
        With MyServer
            If .ServerIsOnline Then
                ' Stop server
                .CurrentServerState = ServerState.Stopping
                Try
                    .StopServer()

                    ' Wait for server exit
                    .ServerProc.WaitForExit()

                    Dim i As Integer = 0
                    While .ServerIsOnline
                        System.Threading.Thread.Sleep(1500)
                        i += 1

                        ' Two minute timeout
                        If i = 120 / 1.5 Then
                            MessageBox.Show("An attempt to restart the server failed.")
                            Exit Sub
                        End If
                    End While

                    ' Start server again
                    .StartServer()
                Catch
                End Try
            End If
        End With
    End Sub

End Module