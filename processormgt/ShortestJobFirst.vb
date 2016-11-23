Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq

''' <summary>
''' Shortest-Job-First strategy.
''' </summary>
''' <remarks>
''' This is a priority strategy. The smaller the burst time a process has left to run, the higher
''' it's priority is.
''' </remarks>
Public NotInheritable Class ShortestJobFirst
    Implements IStrategy

    Private ReadOnly _pollTime As Integer

    ''' <summary>
    ''' Creates and intializes a new instance of the SJF strategy using a specified poll time.
    ''' </summary>
    ''' <param name="pollTime">Poll time.</param>
    ''' <exception cref="ArgumentOutOfRangeException"><strong>pollTime</strong> is less than <strong>1</strong>.</exception>
    Public Sub New(pollTime As Integer)
        If pollTime < 1 Then
            Throw New ArgumentOutOfRangeException(My.Resources.PollTimeGreaterThanZero)
        End If

        _pollTime = pollTime
    End Sub

    ''' <summary>
    ''' Executes the processes in <see cref="Runner.ProcessLoad"/> in shortest job first order.
    ''' </summary>
    ''' <param name="runner">Runner to use as execution enviroment.</param>
    Public Sub Execute(runner As Runner) Implements IStrategy.Execute
        Dim readyQueue As New List(Of Process)(runner.ProcessLoad)
        While readyQueue.Count > 0
            Try
                ' select the process that is eligible to run with the smallest remaining burst time
                Dim p As Process = readyQueue.Where(Function(x) x.ArrivalTime <= runner.Time).OrderBy(Function(x) x.BurstTime - x.Data.UtilizedCpuTime).First()
                runner.UtilizeCpu(p, _pollTime)
                If Process.IsComplete(p) Then
                    runner.LogProcessMetrics(p)
                    readyQueue.Remove(p)
                End If
            Catch generatedExceptionName As InvalidOperationException
                runner.SkipIdleCpuTime()
            End Try
        End While
    End Sub

    ''' <summary>
    ''' Returns a <see cref="String"/> that represents the current <see cref="Object"/>.
    ''' </summary>
    ''' <returns>String representation of object.</returns>
    Public Overrides Function ToString() As String Implements IStrategy.ToString
        Return String.Format(CultureInfo.InvariantCulture, "Shortest Job First (Poll Time = {0})", _pollTime)
    End Function

End Class

