''' <summary>
''' Interface for all strategies.
''' </summary>
Public Interface IStrategy

    ''' <summary>
    ''' Executes the strategy using the provided <see cref="Runner"/>.
    ''' </summary>
    ''' <param name="runner">Runner to use as execution enviroment.</param>
    Sub Execute(runner As Runner)

    ''' <summary>
    ''' Returns a <see cref="String"/> that represents the current <see cref="Object"/>.
    ''' </summary>
    ''' <returns>String representation of object.</returns>
    Function ToString() As String

End Interface

