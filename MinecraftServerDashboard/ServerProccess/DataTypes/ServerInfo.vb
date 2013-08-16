Imports System.ComponentModel

''' <summary>
''' The state of the server process
''' </summary>
Public Enum ServerState
    ''' <summary>
    ''' Running and accepting connections
    ''' </summary>
    <Description("Running")> _
    Running = 1
    ''' <summary>
    ''' Craftbukkit is not running
    ''' </summary>
    <Description("NotRunning")> _
    NotRunning = 0
    ''' <summary>
    ''' Starting up
    ''' </summary>
    <Description("Startup")> _
    WarmUp = -1
    ''' <summary>
    ''' Stopping
    ''' </summary>
    <Description("Stopping")> _
    Stopping = -2
    ''' <summary>
    ''' Server is reloading
    ''' </summary>
    <Description("Reloading")> _
    Reloading = 3
    ''' <summary>
    ''' Server has deteced a critical error
    ''' </summary>
    <Description("Critical Error")> _
    BindCritical = -9
End Enum