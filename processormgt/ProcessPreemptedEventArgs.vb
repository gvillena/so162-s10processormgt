''' <summary>
''' Event data associated with <see cref="Runner.ProcessPreempted"/>.
''' </summary>
Public Class ProcessPreemptedEventArgs
    Inherits ProcessEventArgs

    ''' <summary>
    ''' Initializes a new instance of <see cref="ProcessPreemptedEventArgs"/>.
    ''' </summary>
    ''' <param name="id">PID.</param>
    ''' <param name="burstTime">Burst time of process.</param>
    ''' <param name="arrivalTime">Arrival time of process.</param>
    ''' <param name="priority">Priority of process.</param>
    Public Sub New(id As Integer, burstTime As Integer, arrivalTime As Integer, priority As Priority)
        MyBase.New(id, burstTime, arrivalTime, priority)
    End Sub

End Class