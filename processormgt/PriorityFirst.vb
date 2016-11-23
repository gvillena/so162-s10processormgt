
Imports System.Globalization
Imports System.Linq

''' <summary>
''' Priority First strategy.
''' </summary>
''' <remarks>
''' The processes in the ready queue with the higher priorities are given preference when selecting a process
''' to utilize the CPU.
''' </remarks>
Public Class PriorityFirst
    Implements IStrategy

    Private ReadOnly _pollTime As Integer

    ''' <summary>
    ''' Creates and initializes a new instance of the PF strategy.
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
    ''' Executes the processes in <see cref="Runner.ProcessLoad"/> in priority first order.
    ''' </summary>
    ''' <param name="runner">Runner to use as execution enviroment.</param>
    Public Sub Execute(runner As Runner) Implements IStrategy.Execute
        Dim readyQueue As New ProcessLoad(runner.ProcessLoad)

        While readyQueue.Count > 0
            Try
                ' select the (first) process in readyQueue with the highest priority
                Dim p As Process = readyQueue.Where(Function(x) x.ArrivalTime <= runner.Time).OrderByDescending(Function(x) x.Priority).First()
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
        Return String.Format(CultureInfo.InvariantCulture, "Priority First (Poll Time = {0})", _pollTime)
    End Function

End Class
