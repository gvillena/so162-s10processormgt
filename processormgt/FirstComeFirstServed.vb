
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq

''' <summary>
''' First-Come-First-Served strategy.
''' </summary>
''' <remarks>
''' Processes are executed in the order in which they arrived. This version of FCFS does not use a secondary
''' clause when more than one process arrives at the same time.
''' </remarks>
Public NotInheritable Class FirstComeFirstServed
    Implements IStrategy

    ''' <summary>
    ''' Executes the processes in <see cref="Runner.ProcessLoad"/> in FCFS order.
    ''' </summary>
    ''' <param name="runner">Runner to use as execution enviroment.</param>
    Public Sub Execute(runner As Runner) Implements IStrategy.Execute
        Dim readyQueue As New Queue(Of Process)(runner.ProcessLoad.OrderBy(Function(x) x.ArrivalTime))

        While readyQueue.Count > 0
            Dim process__1 As Process = readyQueue.Dequeue()
            runner.SkipIdleCpuTime(process__1)

            While Not Process.IsComplete(process__1)
                runner.UtilizeCpu(process__1)
            End While

            runner.LogProcessMetrics(process__1)
        End While
    End Sub

    ''' <summary>
    ''' Returns a <see cref="String"/> that represents the current <see cref="Object"/>.
    ''' </summary>
    ''' <returns>String representation of object.</returns>
    Public Overrides Function ToString() As String Implements IStrategy.ToString
        Return String.Format(CultureInfo.InvariantCulture, "First Come First Served")
    End Function

End Class
