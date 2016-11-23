
''' <summary>
''' Event data associated with <see cref="Process.Started"/>.
''' </summary>
Public NotInheritable Class ProcessCompletedEventArgs
    Inherits ProcessStartedEventArgs
    Private ReadOnly _completionTime As Integer

    ''' <summary>
    ''' Initializes a new instance of <see cref="ProcessCompletedEventArgs"/>.
    ''' </summary>
    ''' <param name="id">Id of process (PID).</param>
    ''' <param name="burstTime">Burst time of process.</param>
    ''' <param name="arrivalTime">Arrival time of process.</param>
    ''' <param name="priority">Priority of process.</param>
    ''' <param name="startTime">Start time of process.</param>
    ''' <param name="completionTime">Completion time of process.</param>
    Public Sub New(id As Integer, burstTime As Integer, arrivalTime As Integer, priority As Priority, startTime As Integer, completionTime As Integer)
        MyBase.New(id, burstTime, arrivalTime, priority, startTime)
        _completionTime = completionTime
    End Sub

    ''' <summary>
    ''' Gets the completion time of the process.
    ''' </summary>
    Public ReadOnly Property CompletionTime() As Integer
        Get
            Return _completionTime
        End Get
    End Property
End Class

