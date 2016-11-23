
''' <summary>
''' Event data associated with <see cref="Process.Started"/>.
''' </summary>
Public Class ProcessStartedEventArgs
    Inherits ProcessEventArgs
    Private ReadOnly _startTime As Integer

    ''' <summary>
    ''' Initializes a new instance of <see cref="ProcessStartedEventArgs"/>.
    ''' </summary>
    ''' <param name="id">PID.</param>
    ''' <param name="burstTime">Burst time of process.</param>
    ''' <param name="arrivalTime">Arrival time of process.</param>
    ''' <param name="priority">Priority of process.</param>
    ''' <param name="startTime">Start time of process.</param>
    Public Sub New(id As Integer, burstTime As Integer, arrivalTime As Integer, priority As Priority, startTime As Integer)
        MyBase.New(id, burstTime, arrivalTime, priority)
        _startTime = startTime
    End Sub

    ''' <summary>
    ''' Gets time when the process utilized the CPU for the first time.
    ''' </summary>
    Public ReadOnly Property StartTime() As Integer
        Get
            Return _startTime
        End Get
    End Property
End Class
