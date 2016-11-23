''' <summary>
''' Data associated with <see cref="Runner.Completed"/>.
''' </summary>
Public NotInheritable Class RunnerCompletedEventArgs
    Inherits RunnerStartedEventArgs

    Private ReadOnly _totalTime As Integer

    ''' <summary>
    ''' Initializes a new instance of <see cref="RunnerCompletedEventArgs"/>.
    ''' </summary>
    ''' <param name="processCount">Number of processes in simulation.</param>
    ''' <param name="strategy">Strategy being used in simulation.</param>
    ''' <param name="totalTime">Total time of simulation.</param>
    Public Sub New(processCount As Integer, strategy As IStrategy, totalTime As Integer)
        MyBase.New(processCount, strategy)
        _totalTime = totalTime
    End Sub

    ''' <summary>
    ''' Gets the total time of simulation.
    ''' </summary>
    Public ReadOnly Property TotalTime() As Integer
        Get
            Return _totalTime
        End Get
    End Property

End Class
