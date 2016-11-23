Imports System.Collections.Generic

''' <summary>
''' Math utility extension methods.
''' </summary>
Public Module MathUtil

    Sub New()
    End Sub
    ''' <summary>
    ''' Calculates variance for a given series.
    ''' </summary>
    ''' <param name="series">Series of values.</param>
    ''' <returns>Variance of series.</returns>
    ''' <exception cref="ArgumentNullException"><strong>series</strong> is <strong>null</strong>.</exception>
    <System.Runtime.CompilerServices.Extension>
    Public Function Variance(series As IEnumerable(Of Integer)) As Double
        If series Is Nothing Then
            Throw New ArgumentNullException("series")
        End If

        Dim mean As Double = series.Mean()
        Dim total As Double = 0
        Dim count As Integer = 0
        For Each d As Double In series
            Dim temp As Double = d - mean
            total += (temp) * (temp)
            count += 1
        Next
        Return If(count = 0, 0, total / count)
    End Function

    ''' <summary>
    ''' Calculates variance for a given series.
    ''' </summary>
    ''' <param name="series">Series of values.</param>
    ''' <returns>Variance of series.</returns>
    ''' <exception cref="ArgumentNullException"><strong>series</strong> is <strong>null</strong>.</exception>
    <System.Runtime.CompilerServices.Extension>
    Public Function Variance(series As IEnumerable(Of Double)) As Double
        If series Is Nothing Then
            Throw New ArgumentNullException("series")
        End If

        Dim mean As Double = series.Mean()
        Dim total As Double = 0
        Dim count As Integer = 0
        For Each d As Double In series
            Dim temp As Double = d - mean
            total += (temp) * (temp)
            count += 1
        Next
        Return If(count = 0, 0, total / count)
    End Function

    ''' <summary>
    ''' Calculates the mean of a series.
    ''' </summary>
    ''' <param name="series">Series of values.</param>
    ''' <returns>Mean of series.</returns>
    ''' <exception cref="ArgumentNullException"><strong>series</strong> is <strong>null</strong>.</exception>
    <System.Runtime.CompilerServices.Extension>
    Public Function Mean(series As IEnumerable(Of Integer)) As Double
        If series Is Nothing Then
            Throw New ArgumentNullException("series")
        End If

        Dim count As Integer = 0
        Dim total As Double = 0
        For Each d As Double In series
            total += d
            count += 1
        Next
        Return If(count = 0, 0, total / count)
    End Function

    ''' <summary>
    ''' Calculates the mean of a series.
    ''' </summary>
    ''' <param name="series">Series of values.</param>
    ''' <returns>Mean of series.</returns>
    ''' <exception cref="ArgumentNullException"><strong>series</strong> is <strong>null</strong>.</exception>
    <System.Runtime.CompilerServices.Extension>
    Public Function Mean(series As IEnumerable(Of Double)) As Double
        If series Is Nothing Then
            Throw New ArgumentNullException("series")
        End If

        Dim count As Integer = 0
        Dim total As Double = 0
        For Each d As Double In series
            total += d
            count += 1
        Next
        Return If(count = 0, 0, total / count)
    End Function

End Module

