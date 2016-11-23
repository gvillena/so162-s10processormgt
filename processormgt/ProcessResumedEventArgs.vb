''' <summary>
''' Event data asscoiated with <see cref="Runner.ProcessResumed"/>
''' </summary>
Public Class ProcessResumedEventArgs
    Inherits ProcessStartedEventArgs

    ''' <summary>
    ''' Creates and initializes a new instance of <see cref="ProcessResumedEventArgs"/>.
    ''' </summary>
    ''' <param name="id">PID.</param>
    ''' <param name="burstTime">Burst time of process.</param>
    ''' <param name="arrivalTime">Arrival time of process.</param>
    ''' <param name="priority">Priority of process.</param>
    ''' <param name="startTime">Start time of process.</param>
    Public Sub New(id As Integer, burstTime As Integer, arrivalTime As Integer, priority As Priority, startTime As Integer)
        MyBase.New(id, burstTime, arrivalTime, priority, startTime)
    End Sub

End Class