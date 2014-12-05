Imports System.Net
Imports System.IO

'Adapted from: http://stackoverflow.com/questions/1069103/how-to-get-my-own-ip-address-in-c
Module GetIP

    Const URL As String = "http://checkip.dyndns.org/"

    Public Function GetExternalIP() As String
        If Debugger.IsAttached Then
            ' Dont fetch this when debugging to reduce number of requests
            Return Nothing
        End If
        If NetworkIsOnline() Then
            Try
                Dim direction As [String] = ""
                Dim request As HttpWebRequest = HttpWebRequest.Create(URL)
                Using response As HttpWebResponse = request.GetResponse()
                    Using stream As New StreamReader(response.GetResponseStream())
                        direction = stream.ReadToEnd()
                    End Using
                End Using

                'Extract the IP address from the html
                Dim first As Integer = direction.IndexOf("Address: ") + 9
                Dim last As Integer = direction.LastIndexOf("</body>")
                direction = direction.Substring(first, last - first)

                Return direction
            Catch ex As WebException
                Debug.Print(ex.Message)
                Return Nothing
            Catch ex As Exception
                Debug.Print(ex.Message)
                Return Nothing
            End Try
        Else
            Debug.Print("[GetIP] The network is unavailable.")
            Return Nothing
        End If
    End Function

    Function NetworkIsOnline()
        Dim myNetwork As New Microsoft.VisualBasic.Devices.Network
        Return myNetwork.IsAvailable
    End Function
End Module