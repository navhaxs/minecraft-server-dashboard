' Used for data binding with UI
' Inverts boolean
' e.g. If server is NOT running (False), enable 'Start Server' menu item (True)
'      Where False ---> True
'
' References: http://wpftutorial.net/ValueConverters.html
'             http://stackoverflow.com/questions/1039636/how-to-bind-inverse-boolean-properties-in-wpf
<ValueConversion(GetType(Boolean), GetType(Boolean))> _
Public Class InverseBooleanConverter
    Implements IValueConverter
#Region "IValueConverter Members"

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If targetType <> GetType(Boolean) Then
            Throw New InvalidOperationException("The target must be a boolean")
        End If

        Return Not CBool(value)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotSupportedException()
    End Function

#End Region
End Class

<ValueConversion(GetType(Boolean), GetType(System.Windows.Visibility))> _
Public Class BooleanToVisibilityConverter
    Implements IValueConverter
#Region "IValueConverter Members"

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If targetType <> GetType(System.Windows.Visibility) Then
            Throw New InvalidOperationException("The target must be a System.Windows.Visibility")
        End If

        Select Case CBool(value)
            Case True
                Return System.Windows.Visibility.Visible
            Case False
                Return System.Windows.Visibility.Collapsed
            Case Else
                Throw New InvalidOperationException("The target must be a boolean")
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotSupportedException()
    End Function

#End Region
End Class

<ValueConversion(GetType(Boolean), GetType(System.Windows.Visibility))> _
Public Class InverseBooleanToVisibilityConverter
    Implements IValueConverter
#Region "IValueConverter Members"

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If targetType <> GetType(System.Windows.Visibility) Then
            Throw New InvalidOperationException("The target must be a System.Windows.Visibility")
        End If

        Select Case CBool(value)
            Case True
                Return System.Windows.Visibility.Collapsed
            Case False
                Return System.Windows.Visibility.Visible
            Case Else
                Throw New InvalidOperationException("The target must be a boolean")
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotSupportedException()
    End Function

#End Region
End Class