Imports System.Windows.Media.Animation

Public Class OnlinePlayerList

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        If Not System.ComponentModel.DesignerProperties.GetIsInDesignMode(Me) Then

            ' Add any initialization after the InitializeComponent() call.
            Me.DataContext = MyServer

            ' Intial value of online player counter
            Dim b As New ServerProperties(MyServer.MyStartupParameters.ServerProperties)

            Dim s As String = b.ReturnConfigValue("max-players")
            If s = Nothing Then
                lblPlayerCounter.Content = ""
            Else
                lblPlayerCounter.Content = "0/" & b.ReturnConfigValue("max-players")
            End If

        End If
    End Sub

    Public Sub RefreshOnlinePlayers() Handles Button2.Click
        MyServer.isGettingPlayerListActivity = ServerManager.isGettingPlayerListActivity_STATE.LookingForMatch
    End Sub

End Class