Imports System.ComponentModel

''' <summary>
''' This class generates the data bindings between the UI and code for 'MainWindow'
''' </summary>
Public Class MainWindowViewModel
    Implements INotifyPropertyChanged

#Region "Navigation title, e.g. 'overview', 'players', 'world'"

    ' Updated whenever the use changes the selected tab
    Private _navTitle As String
    Public Property navTitle As String
        Get
            Return _navTitle
        End Get
        Set(value As String)
            _navTitle = value
            OnPropertyChanged("navTitle")
        End Set
    End Property
#End Region

#Region "Whenever another window is being displayed, disable the controls of the MainWindow UI."

    Public ReadOnly Property MainWindowOverlay_MainWindowIsEnabled As Boolean
        Get
            Select Case _MainWindowOverlay
                ' If a dialog is being displayed, disable all controls on MainWindow form
                Case OverlayShownType.StandardDialog
                    'IsEnabled = False
                    Return False
                Case OverlayShownType.None
                    'IsEnabled = True
                    Return True
                Case Else
                    Return False
            End Select
        End Get
    End Property

    Dim _MainWindowOverlay As OverlayShownType = OverlayShownType.None
    Public Property MainWindowOverlay As OverlayShownType
        Get
            Return _MainWindowOverlay
        End Get
        Set(value As OverlayShownType)
            _MainWindowOverlay = value
            ' Trigger the UI once this value has changes, forces UI to update
            OnPropertyChanged("MainWindowOverlay")
            OnPropertyChanged("MainWindowOverlay_MainWindowIsEnabled")
        End Set
    End Property

    Public Enum OverlayShownType As Integer
        None = -1
        StandardDialog = 0
    End Enum

#End Region

#Region "Required data binding code"
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    ' Create the OnPropertyChanged method to raise the event
    Protected Sub OnPropertyChanged(ByVal name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
#End Region
End Class