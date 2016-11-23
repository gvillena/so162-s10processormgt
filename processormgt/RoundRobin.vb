
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq

''' <summary>
''' Round-Robin strategy.
''' </summary>
''' <remarks>
''' <para>
''' Executes each process for at most the defined time quantum. If the <see cref="Process"/> has not voluntarily
''' released the CPU within the time quantum then it is added to the back of the ready queue.
''' </para>
''' <para>
''' 80% of the CPU bursts should be less than the selected time quantum. If you select a quantum of 1 then you emulate processor
''' sharing, in this case n processors will appear to have their own processor running at 1/n the speed of the real processor.
''' </para>
''' <para>
''' This framework is not a physical implementation of course, but when analysing the metrics gathered you should take into account the
''' quantum selected as it can adversly affect the strategies performance.
''' </para>
''' </remarks>
Public Class RoundRobin
    Implements IStrategy

    Private ReadOnly _quantum As Integer
    Private _processLoad As ProcessLoad
    Private _readyQueue As Queue(Of Process)

    ''' <summary>
    ''' Creates and initializes a new instance of <see cref="RoundRobin"/> with a defined time quantum.
    ''' </summary>
    ''' <param name="quantum">Time quantum.</param>
    Public Sub New(quantum As Integer)
        If quantum < 1 Then
            Throw New ArgumentOutOfRangeException(My.Resources.TimeQuantumLessThanOne)
        End If
        _quantum = quantum
        _readyQueue = New Queue(Of Process)()
    End Sub

    ''' <summary>
    ''' Executes the processes in <see cref="Runner.ProcessLoad"/> in Round-Robin order.
    ''' </summary>
    ''' <param name="runner">Runner to use as execution enviroment.</param>
    Public Sub Execute(runner As Runner) Implements IStrategy.Execute
        _processLoad = New ProcessLoad(runner.ProcessLoad)
        While _processLoad.Count > 0
            Dim validProcesses As List(Of Process) = _processLoad.Where(Function(x) x.ArrivalTime <= runner.Time).ToList()
            If validProcesses.Count > 0 Then
                For Each p As Process In validProcesses
                    _readyQueue.Enqueue(p)
                    _processLoad.Remove(p)
                Next
                While _readyQueue.Count > 0
                    Dim p As Process = _readyQueue.Dequeue()
                    Dim utilized As Integer = 0
                    While utilized < _quantum
                        runner.UtilizeCpu(p)
                        utilized += 1
                        For Each proc As Process In _processLoad.Where(Function(x) x.ArrivalTime <= runner.Time).ToList()
                            _readyQueue.Enqueue(proc)
                            _processLoad.Remove(proc)
                        Next
                        If Process.IsComplete(p) Then
                            Exit While
                        End If
                    End While
                    If Process.IsComplete(p) Then
                        runner.LogProcessMetrics(p)
                    Else
                        _readyQueue.Enqueue(p)
                    End If
                End While
            Else
                runner.SkipIdleCpuTime()
            End If
        End While
    End Sub

    ''' <summary>
    ''' Returns a <see cref="String"/> that represents the current <see cref="Object"/>.
    ''' </summary>
    ''' <returns>String representation of object.</returns>
    Public Overrides Function ToString() As String Implements IStrategy.ToString
        Return String.Format(CultureInfo.InvariantCulture, "Round Robin (Time Quantum = {0})", _quantum)
    End Function

End Class
