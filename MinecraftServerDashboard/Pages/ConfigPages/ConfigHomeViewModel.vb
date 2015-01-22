Imports System.ComponentModel

''' <summary>
''' Data bindings with UI
''' </summary>
Public Class PageConfigViewModel
    Implements INotifyPropertyChanged

    Private _is_infotip_RestartRequired As Windows.Visibility = Visibility.Collapsed
    Public Property Is_infotip_RestartRequired As Windows.Visibility
        Get
            Return _is_infotip_RestartRequired
        End Get
        Set(value As Windows.Visibility)
            If MyServer.ServerIsOnline Then
                ' Only show the "restart required" banner if the server is actually currently running
                _is_infotip_RestartRequired = value
                OnPropertyChanged("Is_infotip_RestartRequired")
            End If
        End Set
    End Property

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    ' Create the OnPropertyChanged method to raise the event
    Protected Sub OnPropertyChanged(ByVal name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class