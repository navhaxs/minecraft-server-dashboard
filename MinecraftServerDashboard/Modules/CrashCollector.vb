Module CrashCollector

    Friend Sub CrashHelpMe(ByRef sender As Object, ByRef e As Windows.Threading.DispatcherUnhandledExceptionEventArgs)
        Dim filename As String = "Dashboard_ErrorReport_" & Date.Today.Day & Date.Today.Month & Date.Today.Year & "_" & Date.Now.Hour & Date.Now.Minute & Date.Now.Second & ".txt"

        Dim report As String

        report = "*** Please include this report when reporting your issue ***" & vbNewLine

        'Build the error report
        report = report & "TimeDate: " & Date.Now & vbNewLine

        report = report & "OSVersion: " & My.Computer.Info.OSVersion & vbNewLine

        report = report & "OSPlatform: " & My.Computer.Info.OSPlatform & vbNewLine

        report = report & "OSFullName: " & My.Computer.Info.OSFullName & vbNewLine

        report = report & "InstalledUICulture: " & My.Computer.Info.InstalledUICulture.ToString & vbNewLine

        report = report & "AvailablePhysicalMemory: " & My.Computer.Info.AvailablePhysicalMemory & vbNewLine

        report = report & "TotalPhysicalMemory: " & My.Computer.Info.TotalPhysicalMemory & vbNewLine

        report = report & "e.Exception.Message" & e.Exception.ToString & vbNewLine

        My.Computer.FileSystem.WriteAllText(System.Environment.CurrentDirectory & "\" & filename, report, False)
        MessageBox.Show("An error report was generated: " & filename)

        'Dim m As New CrashReportUI
        'm.txtReport.Text = report
        'm.Show()
        'e.Handled = True
    End Sub

End Module
