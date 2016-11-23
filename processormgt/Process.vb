
''' <summary>
''' Process.
''' </summary>
Public NotInheritable Class Process

    Private ReadOnly _arrivalTime As Integer
    Private ReadOnly _burstTime As Integer
    Private ReadOnly _cpuActivity As Pairs(Of Integer, Integer)
    Private ReadOnly _id As Integer
    Private _data As ProcessRunData

    ''' <summary>
    ''' Initializes a new instance of <see cref="Process"/>.
    ''' </summary>
    ''' <param name="id">Id of process.</param>
    ''' <param name="burstTime">Burst time of the process.</param>
    ''' <param name="arrivalTime">Arrival time of the process.</param>
    ''' <exception cref="ArgumentException"><strong>id</strong> is less than <strong>0</strong>.</exception>
    ''' <exception cref="ArgumentException"><strong>burstTime</strong> is less than <strong>1</strong>.</exception>
    ''' <exception cref="ArgumentException"><strong>arrivalTime</strong> is less than <strong>0</strong>.</exception>
    Public Sub New(id As Integer, burstTime As Integer, arrivalTime As Integer)
        If id < 0 Then
            Throw New ArgumentException(My.Resources.ProcessIdLessThanZero)
        ElseIf burstTime < 1 Then
            Throw New ArgumentException(My.Resources.ProcessBurstTimeLessThanOne)
        ElseIf arrivalTime < 0 Then
            Throw New ArgumentException(My.Resources.ArrivalTimeLessThanZero)
        End If

        _id = id
        _burstTime = burstTime
        _arrivalTime = arrivalTime
        _cpuActivity = New Pairs(Of Integer, Integer)()
        Priority = Priority.Medium
    End Sub

    ''' <summary>
    ''' Gets or sets the data associated with the process during simulation. 
    ''' </summary>
    Public Property Data() As ProcessRunData
        Get
            Return _data
        End Get
        Friend Set
            If Value.UtilizedCpuTime <> _data.UtilizedCpuTime Then
                CheckState(Value)
            End If
            _data = Value
        End Set
    End Property

    ''' <summary>
    ''' Gets the Id of the process (PID).
    ''' </summary>
    Public ReadOnly Property Id() As Integer
        Get
            Return _id
        End Get
    End Property

    ''' <summary>
    ''' Gets the burst time of the process in nanoseconds (ns).
    ''' </summary>
    Public ReadOnly Property BurstTime() As Integer
        Get
            Return _burstTime
        End Get
    End Property

    ''' <summary>
    ''' Gets the arrival time of the process in nanoseconds (ns).
    ''' </summary>
    Public ReadOnly Property ArrivalTime() As Integer
        Get
            Return _arrivalTime
        End Get
    End Property

    ''' <summary>
    ''' Gets the CPU activity of the process.
    ''' </summary>
    Public ReadOnly Property CpuActivity() As Pairs(Of Integer, Integer)
        Get
            Return _cpuActivity
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets the start time of the process in nanoseconds (ns).
    ''' </summary>
    Public Property StartTime() As Integer
        Get
            Return m_StartTime
        End Get
        Set
            m_StartTime = Value
        End Set
    End Property
    Private m_StartTime As Integer

    ''' <summary>
    ''' Gets or sets the completion time of the process in nanoseconds (ns).
    ''' </summary>
    Public Property CompletionTime() As Integer
        Get
            Return m_CompletionTime
        End Get
        Set
            m_CompletionTime = Value
        End Set
    End Property
    Private m_CompletionTime As Integer

    ''' <summary>
    ''' Gets or sets the <see cref="Priority"/> of the process.
    ''' </summary>
    Public Property Priority() As Priority
        Get
            Return m_Priority
        End Get
        Set
            m_Priority = Value
        End Set
    End Property
    Private m_Priority As Priority

    ''' <summary>
    ''' Gets the wait time of the <see cref="Process"/>.
    ''' </summary>
    Public Property WaitTime() As Integer
        Get
            Return m_WaitTime
        End Get
        Set
            m_WaitTime = Value
        End Set
    End Property
    Private m_WaitTime As Integer

    ''' <summary>
    ''' Gets or sets the turnaround time of the process.
    ''' </summary>
    Public Property TurnaroundTime() As Integer
        Get
            Return m_TurnaroundTime
        End Get
        Set
            m_TurnaroundTime = Value
        End Set
    End Property
    Private m_TurnaroundTime As Integer

    ''' <summary>
    ''' Gets or sets the resposen time of the process.
    ''' </summary>
    Public Property ResponseTime() As Integer
        Get
            Return m_ResponseTime
        End Get
        Set
            m_ResponseTime = Value
        End Set
    End Property
    Private m_ResponseTime As Integer

    ''' <summary>
    ''' Occurs when the process utilizes the CPU for the first time.
    ''' </summary>
    Public Event Started As EventHandler(Of ProcessStartedEventArgs)

    ''' <summary>
    ''' Occurs when the process has utilized enough CPU time to satisfy it's burst time.
    ''' </summary>
    Public Event Completed As EventHandler(Of ProcessCompletedEventArgs)

    ''' <summary>
    ''' Raises the <see cref="Started"/> event.
    ''' </summary>
    ''' <param name="e">Data associated with the process when it has started.</param>
    Private Sub OnStarted(e As ProcessStartedEventArgs)
        'Dim temp As EventHandler(Of ProcessStartedEventArgs) = Started
        'RaiseEvent temp(Me, e)
    End Sub

    ''' <summary>
    ''' Raises the <see cref="Completed"/> event.
    ''' </summary>
    ''' <param name="e">Data associated with the process upon its completion.</param>
    Private Sub OnCompleted(e As ProcessCompletedEventArgs)
        'Dim temp As EventHandler(Of ProcessCompletedEventArgs) = Completed
        'RaiseEvent temp(Me, e)
    End Sub

    ''' <summary>
    ''' Check's the state of the process, e.g. if the process has just started or utilized enough CPU time to 
    ''' satisfy its burst time.
    ''' </summary>
    ''' <param name="data">Data associated with this process during the simulation.</param>
    Private Sub CheckState(data As ProcessRunData)
        ' check to see if the process hasn't utilized the CPU before
        If _data.UtilizedCpuTime = 0 Then
            StartTime = data.RunnerTime - 1
            OnStarted(New ProcessStartedEventArgs(Id, BurstTime, ArrivalTime, Priority, StartTime))
            ResponseTime = StartTime - _arrivalTime
        End If
        ' check to see if the process has utilized enough CPU time to voluntarily release the CPU
        If data.UtilizedCpuTime = BurstTime Then
            CompletionTime = data.RunnerTime
            OnCompleted(New ProcessCompletedEventArgs(Id, BurstTime, ArrivalTime, Priority, StartTime, CompletionTime))
            TurnaroundTime = CompletionTime - _arrivalTime
        End If
    End Sub

    ''' <summary>
    ''' Checks whether a given process has utilized enough CPU time to satisfy it's overall burst time.
    ''' </summary>
    ''' <param name="process">Process to check is complete.</param>
    ''' <returns>True if the process has completed; otherwise false.</returns>
    Public Shared Function IsComplete(process As Process) As Boolean
        Return If(process.BurstTime - process.Data.UtilizedCpuTime = 0, True, False)
    End Function
End Class
