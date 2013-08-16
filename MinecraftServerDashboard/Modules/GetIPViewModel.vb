Imports System.ComponentModel

Public Class GetIPViewModel
    Implements INotifyPropertyChanged

    Private _externalIP As String
    Public Property ExternalIP As String
        Get
            Return _externalIP
        End Get
        Set(value As String)
            _externalIP = value
            OnPropertyChanged("ExternalIP")
        End Set
    End Property

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    ' Create the OnPropertyChanged method to raise the event
    Protected Sub OnPropertyChanged(ByVal name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class