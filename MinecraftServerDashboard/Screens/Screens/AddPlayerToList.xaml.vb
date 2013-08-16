Imports System.IO
Public Class AddPlayerToList

    Public isRequireUsernameValidation As Boolean = False
    Public isRequireIPaddressValidation As Boolean = False

    Function DoAddPlayerToList(Optional descriptiontext As String = Nothing, Optional labeltext As String = Nothing, Optional requireUsernameChecking As Boolean = False, Optional RequireIPaddressValidation As Boolean = False)
        ' Set which rule to apply when checking textbox field
        isRequireUsernameValidation = requireUsernameChecking

        txtLabel.Text = labeltext

        If Not descriptiontext Is Nothing Then
            lbldescription.Text = descriptiontext
        End If
        Me.ShowDialog()
        If result Then
            Return txtUsername.Text
        Else
            Return Nothing
        End If
    End Function

    Dim result As Boolean = False
    Sub DoClose() Handles btnCancel.Click
        Close()
    End Sub

    Private Sub Button_Click_1()
        result = True
        DoClose()
    End Sub

    Private Sub txtFilename_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtUsername.TextChanged
        btnGo.IsEnabled = (txtUsername.Text.Length > 0) And UsernameIsOK(txtUsername.Text)
    End Sub

    Public Function UsernameIsOK(ByVal notch As String) As Boolean 'Alphanumeric string and underscore only
        If isRequireUsernameValidation Then
            Dim valid As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_"
            Dim validchars As New List(Of String)
            For Each i In valid.ToCharArray()
                validchars.Add(i)
            Next

            For Each x In notch.ToCharArray
                If Not validchars.Contains(x) Then

                    Return False
                End If
            Next
        ElseIf isRequireIPaddressValidation Then 'Alphanumeric string (e.g. localhost) and period only
            Dim valid As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789."
            Dim validchars As New List(Of String)

            For Each i In valid.ToCharArray()
                validchars.Add(i)
            Next

            For Each x In notch.ToCharArray
                If Not validchars.Contains(x) Then

                    Return False
                End If
            Next

        End If
        Return True
    End Function

    Private Sub AddPlayerToList_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        txtUsername.Focus()
    End Sub
End Class