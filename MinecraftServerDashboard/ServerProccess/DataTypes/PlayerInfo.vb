Public Class Player

    Public Sub New(dname As String, uname As String)
        DisplayName = dname
        Username = uname
    End Sub

    Public Property DisplayName As String
    Public Property Username As String

    Public Overrides Function ToString() As String
        Return Username
    End Function

End Class
