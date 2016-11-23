
''' <summary>
''' Tuple (int * int) of data associated with each process as it is running.
''' </summary>
Public Structure ProcessRunData
    Private ReadOnly _runnerTime As Integer
    Private ReadOnly _utilizedCpuTime As Integer

    ''' <summary>
    ''' Intializes a new instance of <see cref="ProcessRunData"/>.
    ''' </summary>
    ''' <param name="utilizedCpuTime">Total time the process has utilized of the CPU.</param>
    ''' <param name="runnerTime">Time of the runner.</param>
    Friend Sub New(utilizedCpuTime As Integer, runnerTime As Integer)
        _utilizedCpuTime = utilizedCpuTime
        _runnerTime = runnerTime
    End Sub

    ''' <summary>
    ''' Gets the number of nanoseconds (ns) the process has utilized the CPU for.
    ''' </summary>
    Public ReadOnly Property UtilizedCpuTime() As Integer
        Get
            Return _utilizedCpuTime
        End Get
    End Property

    ''' <summary>
    ''' Gets the current time of the <see cref="Runner"/>.
    ''' </summary>
    Public ReadOnly Property RunnerTime() As Integer
        Get
            Return _runnerTime
        End Get
    End Property
End Structure
