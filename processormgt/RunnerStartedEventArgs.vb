
''' <summary>
''' Data associated with <see cref="Runner.Started"/>.
''' </summary>
Public Class RunnerStartedEventArgs
    Inherits EventArgs

    Private ReadOnly _processCount As Integer

    Private ReadOnly _strategy As IStrategy

        ''' <summary>
        ''' Initializes a new instance of <see cref="RunnerStartedEventArgs"/>.
        ''' </summary>
        ''' <param name="processCount">Number of processes in simulation.</param>
        ''' <param name="strategy">Strategy being used in simulation.</param>
        Public Sub New(processCount As Integer, strategy As IStrategy)
            _processCount = processCount
            _strategy = strategy
        End Sub

        ''' <summary>
        ''' Gets the total number of processes being executed in the simulation.
        ''' </summary>
        Public ReadOnly Property ProcessCount() As Integer
            Get
                Return _processCount
            End Get
        End Property

    ''' <summary>
    ''' Gets the <see cref="IStrategy"/> being used by the simulation.
    ''' </summary>
    Public ReadOnly Property Strategy() As IStrategy
        Get
            Return _strategy
        End Get
    End Property

End Class
