Imports System.Net
Imports System.IO

Public Class PlayerTile
    Public Property thisplayer As Player

    Sub New(_thisplayer As Player)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        thisplayer = _thisplayer
        Label1.Content = thisplayer.DisplayName

        ' Load the player's AVARTAR (skin) from the internet
        Dim UserTileImage As New BitmapImage()

        If My.Computer.Network.IsAvailable Then
            Try
                ' Check to see if http://minotar.net is up
                If Not My.Computer.Network.Ping("minotar.net") = True Then
                    Dim myuri As Uri = New Uri("../Images/Blank.png", UriKind.Relative)
                    UserTileImage = New BitmapImage(myuri)

                    ' Check for legitimate 'paid' accounts
                    'Else
                    'Dim client As New WebClient()
                    'Dim data As Stream = client.OpenRead("http://www.minecraft.net/haspaid.jsp?user=" & _username)
                    'Dim reader As New StreamReader(data)
                    'Dim str As String = ""
                    'str = reader.ReadLine()
                    'data.Close()
                    'If str.ToLower = "false" Then
                    'Dim myuri As Uri = New Uri("../Images/BlankBW.png", UriKind.Relative)
                    'UserTileImage = New BitmapImage(myuri)
                Else
                    UserTileImage.BeginInit()
                    UserTileImage.UriSource = New Uri("http://minotar.net/avatar/" & thisplayer.Username & "/" & Image1.Height & ".png", UriKind.Absolute)
                    UserTileImage.CacheOption = BitmapCacheOption.OnDemand
                    UserTileImage.EndInit()

                    Image1.Stretch = Stretch.Uniform
                    Image1.UpdateLayout()
                    'End If
                End If

            Catch
                Dim myuri As Uri = New Uri("../Images/Blank.png", UriKind.Relative)
                UserTileImage = New BitmapImage(myuri)
            End Try
        Else
            Dim myuri As Uri = New Uri("../../Images/Blank.png", UriKind.Relative)
            UserTileImage = New BitmapImage(myuri)
        End If

        Image1.Source = UserTileImage
    End Sub

    Private Sub PlayerItemContextmenu_Opened(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles PlayerItemContextmenu.Opened
        'Dim bc As New BrushConverter
        'Me.Background = bc.ConvertFrom("#FF2A90C1")
    End Sub

    Private Sub PlayerItemContextmenu_Closed(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles PlayerItemContextmenu.Closed
        'Dim bc As New BrushConverter
        'Me.Background = bc.ConvertFrom("#FF1A6D93")
    End Sub

    Private Sub UserControl_MouseEnter(sender As System.Object, e As System.Windows.Input.MouseEventArgs) Handles MyBase.MouseEnter
        'Dim bc As New BrushConverter
        'Me.Background = bc.ConvertFrom("#FF2A90C1")
    End Sub

    Private Sub UserControl_MouseLeave(sender As System.Object, e As System.Windows.Input.MouseEventArgs) Handles MyBase.MouseLeave
        'Dim bc As New BrushConverter
        'Me.Background = bc.ConvertFrom("#FF1A6D93")
    End Sub

    Private Sub btnKick_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnKick.Click
        MyServer.SendCommand("kick " + thisplayer.Username)
    End Sub

    Private Sub btnBan_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnBan.Click
        MyServer.SendCommand("ban " + thisplayer.Username)
    End Sub

    Private Sub btnOp_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnOp.Click
        MyServer.SendCommand("op " + thisplayer.Username)
    End Sub

    Private Sub btnDeOp_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles btnDeOp.Click
        MyServer.SendCommand("deop " + thisplayer.Username)
    End Sub
End Class