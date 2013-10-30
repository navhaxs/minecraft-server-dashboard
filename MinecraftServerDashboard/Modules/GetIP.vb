Imports System.Net
Imports System.IO

Module GetIP

    Function NetworkIsOnline()
        Dim myNetwork As New Microsoft.VisualBasic.Devices.Network
        Return myNetwork.IsAvailable
    End Function

    'Adapted from: http://stackoverflow.com/questions/1069103/how-to-get-my-own-ip-address-in-c
    'Does not function behind DET Proxy
    'http://bot.whatismyipaddress.com/ may work
    Public Function GetExternalIP() As String
        If NetworkIsOnline() Then
            Try
                Dim direction As [String] = ""
                Dim request As HttpWebRequest = HttpWebRequest.Create("http://checkip.dyndns.org/")

                Using response As HttpWebResponse = request.GetResponse()
                    'MessageBox.Show(response.StatusCode)=200
                    Using stream As New StreamReader(response.GetResponseStream())
                        direction = stream.ReadToEnd()
                    End Using
                End Using

                'Search for the IP in the html
                Dim first As Integer = direction.IndexOf("Address: ") + 9
                Dim last As Integer = direction.LastIndexOf("</body>")
                direction = direction.Substring(first, last - first)

                Return direction
            Catch ex As WebException

                Return Nothing
            Catch ex As Exception
                Debug.Print("Error obtaining Ext IP! - Is the machine is offline?")
                Debug.Print(ex.Message)
                Return Nothing
            End Try
        Else
            Debug.Print("The machine is offline")
            Return Nothing
        End If
    End Function

End Module