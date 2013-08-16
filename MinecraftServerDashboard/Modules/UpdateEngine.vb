Imports System.Net
Imports System.IO

Module AppShared_UpdateEngine
    Friend myUpdateEngine As UpdateEngine
    Public UpdaterModule As UpdatePage
End Module

''' <summary>
''' Used to download the server files from the internet
''' </summary>
Public Class UpdateEngine

    Dim thisOwner As ConfigJarfileBackend
    Sub New(e As ConfigJarfileBackend)
        thisOwner = e
    End Sub
    Event UUComplete(e As ComponentModel.AsyncCompletedEventArgs, _rootDir As String, _FileName As String)

    Function GetLatestVersionID_Online() As String
        Try
            Dim url As String = "http://cbukk.it/craftbukkit.jar"
            Dim fileName As String
            Using client As New MyWebClient()
                If Not My.Computer.Network.IsAvailable Then
                    Return "Offline =("
                    Exit Function
                End If
                Using rawStream As Stream = client.OpenRead(url)

                    ' Get the filename, which will include version info
                    fileName = client.ResponseUri.AbsolutePath.Substring(client.ResponseUri.AbsolutePath.LastIndexOf("/") + 1)
                    Using reader As New StreamReader(rawStream)
                        Dim firstpos As String = fileName.Substring("craftbukkit-".Length)
                        Dim endpos As Integer = fileName.IndexOf("-", firstpos.Length)
                        Return "Latest recommended release: " & fileName.Substring(firstpos.Length, fileName.Length - endpos - 1)
                        reader.Close()
                    End Using
                    rawStream.Close()
                End Using
            End Using
        Catch ex As Exception
            Return "Could not retrieve latest version number. Try visiting http://dl.bukkit.org/ manually."
        End Try
    End Function

    Dim _rootDir As String
    Dim _FileName As String

    Private m_UpdatePage As UpdatePage

    ''' <summary>
    ''' Downloads a file from the internet
    ''' </summary>
    ''' <param name="rootDir">Directory to save file in</param>
    ''' <param name="UpdateModule">Instance of UpdatePage (to update UI)</param>
    ''' <param name="url">Download URL</param>
    Function AutoNewUpdate(rootDir As String, UpdateModule As UpdatePage, url As String) As Boolean
        m_UpdatePage = UpdateModule

        _rootDir = rootDir
        If Not My.Computer.FileSystem.DirectoryExists(rootDir) Then
            Try
                My.Computer.FileSystem.CreateDirectory(rootDir)
            Catch ex As Exception
            End Try
        End If
        Debug.Print(rootDir)
        Dim fileName As String
        Debug.Print("Initiating download manager")

        AddHandler client.DownloadProgressChanged, AddressOf WWWclient_DownloadProgressChanged
        AddHandler client.DownloadFileCompleted, AddressOf WWWclient_DownloadFileCompleted

        Try
            Using rawStream As Stream = client.OpenRead(url)
                fileName = client.ResponseUri.AbsolutePath.Substring(client.ResponseUri.AbsolutePath.LastIndexOf("/") + 1)
                Using reader As New StreamReader(rawStream)
                    If MessageBox.Show("The server files will be downloaded to:" & vbNewLine & rootDir & "\" & fileName & vbNewLine & "Ready to start downloading?", "Minecraft Server Dashboard", MessageBoxButton.YesNo) = MessageBoxResult.Yes Then
                        Debug.Print("Now saving " & fileName & " to " & rootDir & "\" & fileName & "...")
                        _FileName = fileName
                        ' Download file in separate thread (prevent UI from freezing)
                        client.DownloadFileAsync(New Uri(url), rootDir & "\" & fileName)

                        Debug.Print("File download started in background")
                    Else
                        m_UpdatePage.NavigationService.GoBack()
                        m_UpdatePage.NavigationService.RemoveBackEntry()
                        Debug.Print("File download ABORTED")
                    End If
                    reader.Close()
                End Using
                rawStream.Close()
            End Using
        Catch e As WebException
            MessageBox.Show("Error downloading file" & vbNewLine & e.Message)
            Return False
        End Try
        Return True
    End Function

    ''' <summary>
    ''' Update the UI with download progress
    ''' </summary>
    Private Sub WWWclient_DownloadProgressChanged(sender As Object, e As System.Net.DownloadProgressChangedEventArgs)
        Debug.Print(e.ProgressPercentage)
        Debug.Print(">>" & e.BytesReceived / e.TotalBytesToReceive * 100)

        m_UpdatePage.Dispatcher.Invoke( _
                    New Action(Function()
                                   m_UpdatePage.ProgressBar1.Value = e.ProgressPercentage
                                   m_UpdatePage.Label2.Content = Decimal.Round(e.ProgressPercentage, 0) & "% " & Decimal.Round(CType(e.BytesReceived / 1024, Decimal), 2) & "KB/" & Decimal.Round(CType((e.TotalBytesToReceive / 1024) / 1024, Decimal), 2) & "MB"
                                   Return True
                               End Function))
    End Sub

    Private Sub WWWclient_DownloadFileCompleted(sender As Object, e As ComponentModel.AsyncCompletedEventArgs)
        If Not e.Cancelled Then
            m_UpdatePage.Dispatcher.Invoke( _
                    New Action(Function()
                                   ' Update UI
                                   MyAppSettings.Jarfile = _rootDir & "\" & _FileName

                                   MyServer.ReloadStartupParameters()

                                   If Not thisOwner.thisJar.Items.Contains(_FileName) Then
                                       thisOwner.thisJar.Items.Add(_FileName)
                                   End If

                                   thisOwner.thisJar.SelectedValue = _FileName

                                   thisOwner.UpdatePageContent()

                                   UpdaterModule.Label1.Content = "Download completed."
                                   MessageBox.Show("Download completed! Make sure you select """ & _FileName & """ as the Jarfile location")

                                   '_MainWindow.Frame1.Content = pageDashboard
                                   m_UpdatePage.NavigationService.GoBack()
                                   m_UpdatePage.NavigationService.RemoveBackEntry()

                                   If Not thisOwner Is Nothing Then
                                       thisOwner.UpdatePageContent()
                                   End If

                                   Return True
                               End Function))
            'MessageBox.Show("Download successful, CraftBukkit is now ready to start.")
        Else
            m_UpdatePage.Dispatcher.Invoke( _
                    New Action(Function()
                                   UpdaterModule.Label1.Content = "Download cancelled."
                                   'If _MainWindow.Frame1.CanGoBack Then
                                   '    _MainWindow.Frame1.GoBack()
                                   'End If

                                   'thisOwner.UpdatePageContent()
                                   Return True
                               End Function))
        End If
        RaiseEvent UUComplete(e, _rootDir, _FileName)
    End Sub

    Friend client As New MyWebClient()

    Class MyWebClient
        Inherits WebClient
        Private _responseUri As Uri

        Public ReadOnly Property ResponseUri() As Uri
            Get
                Return _responseUri
            End Get
        End Property

        Protected Overrides Function GetWebResponse(request As WebRequest) As WebResponse
            Try
                Dim response As WebResponse = MyBase.GetWebResponse(request)
                _responseUri = response.ResponseUri
                Return response
            Catch ex As Exception
                Return Nothing
            End Try
        End Function
    End Class

End Class