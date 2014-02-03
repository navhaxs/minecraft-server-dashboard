Imports System.Net
Imports System.IO
Imports Newtonsoft.Json

''' <summary>
''' Used to download the server files from the internet
''' </summary>
Public Class JarDownloadEngine

    Public Event DownloadProgressChanged(percentage As String, receiveddata As String, totaldata As String)

    Public Event DownloadCompleted(e As ComponentModel.AsyncCompletedEventArgs, rootDir As String, filename As String)

#Region "Retrieve latest version numbers from the internet"

    'We use the latest vanilla version number to determine the download url when requested
    ReadOnly Property GetLatestVanillaVersion() As String
        Get
            If Not m_GetLatestVanillaVersion Is Nothing Then
                Return m_GetLatestVanillaVersion
            End If
            Try
                Using wc As New MyWebClient()
                    Dim json = wc.DownloadString("https://s3.amazonaws.com/Minecraft.Download/versions/versions.json")
                    Dim data = JsonConvert.DeserializeObject(Of MojangVersionJSON)(json)
                    m_GetLatestVanillaVersion = data.Latest.release
                    Return m_GetLatestVanillaVersion
                End Using
            Catch ex As Exception
                Return Nothing
            End Try
        End Get
    End Property

    Private m_GetLatestVanillaVersion As String

    Private Class MojangVersionJSON
        <JsonProperty("latest")> _
        Public Property Latest() As VersionData
            Get
                Return m_VersionData
            End Get
            Set(value As VersionData)
                m_VersionData = value
            End Set
        End Property
        Private m_VersionData As VersionData

    End Class

    Private Class VersionData
        <JsonProperty("release")> _
        Public release As String
        <JsonProperty("snapshot")> _
        Public snapshot As String
    End Class

    Function GetLatestCraftBukkitVersion() As String
        Try
            Dim url As String = "http://cbukk.it/craftbukkit.jar"
            Dim fileName As String
            Using wc As New MyWebClient()
                If Not My.Computer.Network.IsAvailable Then
                    Return "Offline =("
                    Exit Function
                End If
                Using rawStream As Stream = wc.OpenRead(url)

                    ' Get the filename, which will include version info
                    fileName = wc.ResponseUri.AbsolutePath.Substring(wc.ResponseUri.AbsolutePath.LastIndexOf("/") + 1)
                    Using reader As New StreamReader(rawStream)
                        'Dim firstpos As String = fileName.Substring("craftbukkit-".Length)
                        'Dim endpos As Integer = fileName.IndexOf("-", firstpos.Length)

                        fileName = fileName.Replace(".jar", "").Replace("craftbukkit-", "")
                        Return "Latest recommended release: " & fileName '.Substring(firstpos.Length, fileName.Length - endpos - 1)
                        reader.Close()
                    End Using
                    rawStream.Close()
                End Using
            End Using
        Catch ex As Exception
            Return "Could not retrieve latest version number. Try visiting http://dl.bukkit.org/ manually."
        End Try
    End Function

#End Region


    Dim _rootDir As String  ' Rootdir should always be the current directory for simplicity of the app
    Dim _FileName As String

    'Private m_UpdatePage As DownloadWindow
    Private m_UpdateProgressBarUI As Elysium.Controls.ProgressBar
    Private m_PercentageCompleteUI As Run

    ''' <summary>
    ''' Downloads a file from the internet
    ''' </summary>
    ''' <param name="rootDir">Directory to save file in</param>
    ''' <param name="UpdateProgressBarUI">Instance of ProgressBar UI for feedback</param>
    ''' <param name="url">Download URL</param>
    Function StartJarDownload(rootDir As String, UpdateProgressBarUI As Elysium.Controls.ProgressBar, url As String) As Boolean
        m_UpdateProgressBarUI = UpdateProgressBarUI

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
                    'If MessageBox.Show("The server files will be downloaded to:" & vbNewLine & rootDir & "\" & fileName & vbNewLine & "Ready to start downloading?", "Minecraft Server Dashboard", MessageBoxButton.YesNo) = MessageBoxResult.Yes Then
                    Debug.Print("Now saving " & fileName & " to " & rootDir & "\" & fileName & "...")
                    _FileName = fileName
                    ' Download file in separate thread (prevent UI from freezing)
                    client.DownloadFileAsync(New Uri(url), rootDir & "\" & fileName)

                    Debug.Print("File download started in background")
                    'Else

                    'Debug.Print("File download ABORTED")
                    'End If
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
        'Debug.Print(e.ProgressPercentage)
        'Debug.Print(">>" & e.BytesReceived / e.TotalBytesToReceive * 100)

        Dim percentage As String = e.ProgressPercentage

        Dim received As String = Decimal.Round(CType(e.BytesReceived / 1024, Decimal), 2)
        If received > 1024 Then
            received = Decimal.Round(CType(received, Decimal) / 1024, 2) & "MB"
        Else
            received = received & "KB"
        End If

        Dim total As String = Decimal.Round(CType((e.TotalBytesToReceive / 1024) / 1024, Decimal), 2) & "MB"

        RaiseEvent DownloadProgressChanged(percentage, received, total)
    End Sub

    Private Sub WWWclient_DownloadFileCompleted(sender As Object, e As ComponentModel.AsyncCompletedEventArgs)
        If Not e.Cancelled Then
            MyUserSettings.Jarfile = _rootDir & "\" & _FileName
            MyServer.ReloadStartupParameters()
        End If
        RaiseEvent DownloadCompleted(e, _rootDir, _FileName)
    End Sub

#Region "Custom webclient"

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

#End Region

End Class