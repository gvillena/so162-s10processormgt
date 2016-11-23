
''' <summary>
''' Priority of a <see cref="Process"/>.
''' </summary>
''' <remarks>
''' The priority of a <see cref="Process"/> should be set with caution as it can lead to significant side effects
''' during simulations, only assign an explicit priority to a <see cref="Process"/> with good reason.
''' </remarks>
Public Enum Priority
    ''' <summary>
    ''' Low priority will result in the process being least favourable when using priority based scheduling strategies.
    ''' </summary>
    Low
    ''' <summary>
    ''' Medium priority (default for all <see cref="Process"/>).
    ''' </summary>
    Medium
    ''' <summary>
    ''' High priority will result in the process being most favourable when using priority based scheduling strategies.
    ''' </summary>
    High
End Enum
