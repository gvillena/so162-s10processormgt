
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq

''' <summary>
''' Executes the processes in the ready queue using shortest job first with large process priority elevevation.
''' </summary>
''' <remarks>
''' <para>
''' <em>L</em> = % of large processes.<br />
''' <em>t</em> = current time.<br />
''' <em>T</em> = % threshold.<br />
''' <em>N</em> = % of large processes.
''' </para>
''' <para>
''' iff <em>L<sub>t</sub> > T</em> then invoke the expert rule (execute <em>N<sub>t</sub></em> large processes).
''' </para>
''' <para>
''' By default <em>N</em> = 50%. The threshold can be modified.
''' </para>
''' </remarks>
Public Class ShortestJobFirstExpert
    Implements IStrategy

    Private ReadOnly _pollTime As Integer
    Private ReadOnly _threshold As Integer

    ''' <summary>
    ''' Initializes a new instance of <see cref="ShortestJobFirstExpert"/>.
    ''' </summary>
    ''' <param name="pollTime">Poll time.</param>
    ''' <param name="threshold">Threshold (%) that when crossed invokes the expert rule.</param>
    ''' <exception cref="ArgumentOutOfRangeException">
    ''' <para>
    ''' <strong>pollTime</strong> is less than <strong>1</strong>
    ''' </para>
    ''' <para>
    ''' -- or --
    ''' </para>
    ''' <para>
    ''' <strong>threshold</strong> is less than <strong>1</strong> or greater than <strong>100</strong>.
    ''' </para>
    ''' </exception>
    Public Sub New(pollTime As Integer, threshold As Integer)
        If pollTime < 1 Then
            Throw New ArgumentOutOfRangeException(My.Resources.PollTimeGreaterThanZero)
        ElseIf threshold < 1 OrElse threshold > 100 Then
            Throw New ArgumentOutOfRangeException(My.Resources.ThresholdOutsideBounds)
        End If

        _pollTime = pollTime
        _threshold = threshold
    End Sub

    ''' <summary>
    ''' Executes the processes in <see cref="Runner.ProcessLoad"/> using a modified <see cref="ShortestJobFirst"/> strategy that elevates the priority
    ''' of large processes based on a threshold being crossed (please see documentation for more information).
    ''' </summary>
    ''' <param name="runner">Runner to use as execution enviroment.</param>
    Public Sub Execute(runner As Runner) Implements IStrategy.Execute
        Dim readyQueue As New List(Of Process)(runner.ProcessLoad)
        While readyQueue.Count > 0
            Dim large As List(Of Process) = readyQueue.Where(Function(x) x.ArrivalTime <= runner.Time AndAlso x.BurstTime >= CInt(BurstTime.LargeMin)).ToList()
            If (large.Count / CDbl(readyQueue.Count)) * 100 > _threshold Then
                Dim n As Double = (CDbl(large.Count) / 100) * 50
                Dim count As Integer = 0
                While count < n
                    Dim p As Process = large(count)
                    While Not Process.IsComplete(p)
                        runner.UtilizeCpu(p)
                    End While
                    runner.LogProcessMetrics(p)
                    readyQueue.Remove(p)
                    count += 1
                End While
            Else
                Try
                    Dim p As Process = readyQueue.Where(Function(x) x.ArrivalTime <= runner.Time).OrderBy(Function(x) x.BurstTime - x.Data.UtilizedCpuTime).First()
                    runner.UtilizeCpu(p, _pollTime)
                    If Process.IsComplete(p) Then
                        runner.LogProcessMetrics(p)
                        readyQueue.Remove(p)
                    End If
                Catch generatedExceptionName As InvalidOperationException
                    runner.SkipIdleCpuTime()
                End Try
            End If
        End While
    End Sub

    ''' <summary>
    ''' Returns a <see cref="String"/> that represents the current <see cref="Object"/>.
    ''' </summary>
    ''' <returns>String representation of object.</returns>
    Public Overrides Function ToString() As String Implements IStrategy.ToString
        Return String.Format(CultureInfo.InvariantCulture, "Shortest Job First Expert (Poll Time = {0}, Threshold = {1}%)", _pollTime, _threshold)
    End Function

End Class
