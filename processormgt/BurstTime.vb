''' <summary>
''' Constants used to generalize process burst times(ns).
''' </summary>
''' <remarks>
''' <para>
''' These burst times are used when generating automated process loads with <see cref="Runner"/>.
''' </para>
''' <para>
''' <em>Note: these are the advised burst time bounds. Strategy designers are encouraged to use these bounds, if you
''' change them you may inadvertently affect other strategies that are based on these values.</em>
''' </para>
''' </remarks>
Public Enum BurstTime
    ''' <summary>
    ''' Minimum burst time(ns) a small sized process will have.
    ''' </summary>
    SmallMin = 1
    ''' <summary>
    ''' Maximum burst time(ns) a small sized process will have.
    ''' </summary>
    SmallMax = 99
    ''' <summary>
    ''' Minimum burst time(ns) a medium sized process will have.
    ''' </summary>
    MediumMin = 100
    ''' <summary>
    ''' Maximum burst time(ns) a medium sized process will have.
    ''' </summary>
    MediumMax = 499
    ''' <summary>
    ''' Minimum burst time(ns) a large sized process will have.
    ''' </summary>
    LargeMin = 500
    ''' <summary>
    ''' Maximum burst time(ns) a large sized process will have.
    ''' </summary>
    LargeMax = 1000
End Enum
