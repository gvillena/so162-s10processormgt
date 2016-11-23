Imports System

''' <summary>
''' Base class for all events fired by <see cref="Process"/>.
''' </summary>
Public MustInherit Class ProcessEventArgs
    Inherits EventArgs

    Private ReadOnly _arrivalTime As Integer
    Private ReadOnly _burstTime As Integer
    Private ReadOnly _id As Integer
    Private ReadOnly _priority As Priority

    ''' <summary>
    ''' Initializes a new instance of <see cref="ProcessEventArgs"/>.
    ''' </summary>
    ''' <param name="id">Id of process.</param>
    ''' <param name="burstTime">Burst time of process.</param>
    ''' <param name="arrivalTime">Arrival time of process.</param>
    ''' <param name="priority">Priority of process.</param>
    Protected Sub New(id As Integer, burstTime As Integer, arrivalTime As Integer, priority As Priority)
        _id = id
        _burstTime = burstTime
        _arrivalTime = arrivalTime
        _priority = priority
    End Sub

    ''' <summary>
    ''' Gets the Id of the process.
    ''' </summary>
    Public ReadOnly Property Id() As Integer
        Get
            Return _id
        End Get
    End Property

    ''' <summary>
    ''' Gets the burst time of the process.
    ''' </summary>
    Public ReadOnly Property BurstTime() As Integer
        Get
            Return _burstTime
        End Get
    End Property

    ''' <summary>
    ''' Gets the arrival time of the process.
    ''' </summary>
    Public ReadOnly Property ArrivalTime() As Integer
        Get
            Return _arrivalTime
        End Get
    End Property

    ''' <summary>
    ''' Gets the <see cref="Priority"/> of the process.
    ''' </summary>
    Public ReadOnly Property Priority() As Priority
        Get
            Return _priority
        End Get
    End Property

End Class
