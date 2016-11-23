
Imports System.Collections.Generic
Imports System.Linq

''' <summary>
''' Runner for CPU scheduling strategies.
''' </summary>
''' <remarks>
''' Runner emulates an enviroment for executing some process load using a defined scheduling strategy. Metrics 
''' are gathered throughout the simulation for further analysis.
''' </remarks>
Public NotInheritable Class Runner

    Private Shared _processId As Integer = 1
    Private ReadOnly _id As Guid
    Private ReadOnly _metrics As Dictionary(Of Integer, Process)
    Private ReadOnly _processLoad As ProcessLoad
    Private ReadOnly _strategy As IStrategy
    Private _simulationCompleted As Boolean
    Private _windowCompletions As Integer
    Private _throughputWindows As List(Of Integer)
    Private _windowTimeframe As Integer
    Private _windowUpperBound As Integer
    Private previousProcess As Process

    ''' <summary>
    ''' Creates a new instance of <see cref="Runner"/> with a specified <see cref="IStrategy"/>.
    ''' </summary>
    ''' <param name="strategy">Strategy to use when selecting the next ready process to utilize the CPU.</param>
    ''' <exception cref="ArgumentNullException"><strong>strategy</strong> is <strong>null</strong>.</exception>
    Private Sub New(strategy As IStrategy)
        If strategy Is Nothing Then
            Throw New ArgumentNullException("strategy", My.Resources.RunnerStrategyNull)
        End If

        _strategy = strategy
        _metrics = New Dictionary(Of Integer, Process)()
        _id = Guid.NewGuid()
    End Sub

    ''' <summary>
    ''' Creates a new instance of <see cref="Runner"/> using a defined <see cref="List{Process}"/> and <see cref="IStrategy"/>.
    ''' </summary>
    ''' <param name="processLoad">Processes to be used in the simulation.</param>
    ''' <param name="strategy">Strategy to use when selecting the next ready process to utilize the CPU.</param>
    ''' <exception cref="ArgumentNullException"><strong>processLoad</strong> or <strong>strategy</strong> is <strong>null</strong>.</exception>
    ''' <exception cref="ArgumentException"><strong>processLoad</strong> contains <strong>0</strong> <see cref="Process"/>'s.</exception>
    Public Sub New(processLoad As ProcessLoad, strategy As IStrategy)
        Me.New(strategy)
        If processLoad Is Nothing Then
            Throw New ArgumentNullException("processLoad")
        ElseIf processLoad.Count < 1 Then
            Throw New ArgumentException(My.Resources.ProcessLoadEmpty)
        End If

        _processLoad = processLoad
        WireProcessEvents()
        ThroughputSetup(_windowTimeframe)
    End Sub

    ''' <summary>
    ''' Creates a new instance of <see cref="Runner"/> using a defined <see cref="List{Process}"/>, <see cref="IStrategy"/> and throughput
    ''' timeframe.
    ''' </summary>
    ''' <param name="processLoad">Processes to be used in the simulation.</param>
    ''' <param name="strategy">Strategy to use when selecting the next ready process to utilize the CPU.</param>
    ''' <param name="windowTimeframe">Window timeframe to use for throughput.</param>
    ''' <exception cref="ArgumentOutOfRangeException"><strong>windowTimeframe</strong> is less than <strong>0</strong>.</exception>
    Public Sub New(processLoad As ProcessLoad, strategy As IStrategy, windowTimeframe As Integer)
        Me.New(processLoad, strategy)
        If windowTimeframe < 0 Then
            Throw New ArgumentOutOfRangeException(My.Resources.WindowTimeFrameLessThanZero)
        End If
        _windowTimeframe = windowTimeframe
        ThroughputSetup(_windowTimeframe)
    End Sub


    ''' <summary>
    ''' Creates a new instance of <see cref="Runner"/> using a defined number of small, medium, and large processes and
    ''' <see cref="IStrategy"/>.
    ''' </summary>
    ''' <remarks>
    ''' Burst times ranges are defined in <see cref="BurstTime"/>.
    ''' Each process has the following generated:  <see cref="Process.ArrivalTime"/>, <see cref="Process.Priority"/> and unique <see cref="Process.Id"/>.
    ''' </remarks>
    ''' <param name="small">Number of small processes.</param>
    ''' <param name="medium">Number of medium processes.</param>
    ''' <param name="large">Number of large processes.</param>
    ''' <param name="strategy">Strategy to use when selecting the next ready process to utilize the CPU.</param>
    ''' <exception cref="ArgumentException"><strong>0</strong> processes are specified for the simulation.</exception>
    ''' <exception cref="ArgumentNullException"><strong>strategy</strong> is <strong>null</strong>.</exception>
    ''' <exception cref="ArgumentOutOfRangeException"><strong>small</strong>, <strong>medium</strong>, or <strong>large</strong>
    ''' is less than <strong>zero</strong>.</exception>
    Public Sub New(small As Integer, medium As Integer, large As Integer, strategy As IStrategy)
        Me.New(strategy)
        If (small + medium + large) < 1 Then
            Throw New ArgumentException(My.Resources.NoProcessesToRun)
        ElseIf small < 0 Then
            Throw New ArgumentOutOfRangeException("small")
        ElseIf medium < 0 Then
            Throw New ArgumentOutOfRangeException("medium")
        ElseIf large < 0 Then
            Throw New ArgumentOutOfRangeException("large")
        End If

        _processLoad = New ProcessLoad()
        Dim random = New Random()
        Dim total As Integer = small + medium + large
        ' used as the upper limit when generating the ArrivalTime of a process
        ' generate the processes to satisfy the properties defined by the user.
        For i As Integer = 0 To small - 1
            _processLoad.Add(New Process(System.Math.Max(System.Threading.Interlocked.Increment(_processId), _processId - 1), random.[Next](CInt(BurstTime.SmallMin), CInt(BurstTime.SmallMax)), random.[Next](0, total)) With {
                    .Priority = DirectCast(random.[Next](0, 3), Priority)
            })
        Next
        For i As Integer = 0 To medium - 1
            _processLoad.Add(New Process(System.Math.Max(System.Threading.Interlocked.Increment(_processId), _processId - 1), random.[Next](CInt(BurstTime.MediumMin), CInt(BurstTime.MediumMax)), random.[Next](0, total)) With {
                    .Priority = DirectCast(random.[Next](0, 3), Priority)
            })
        Next
        For i As Integer = 0 To large - 1
            _processLoad.Add(New Process(System.Math.Max(System.Threading.Interlocked.Increment(_processId), _processId - 1), random.[Next](CInt(BurstTime.LargeMin), CInt(BurstTime.LargeMax)), random.[Next](0, total)) With {
                    .Priority = DirectCast(random.[Next](0, 3), Priority)
            })
        Next

        WireProcessEvents()
        ThroughputSetup(_windowTimeframe)
    End Sub

    ''' <summary>
    ''' Creates a new instance of <see cref="Runner"/> using a defined number of small, medium, and large processes and
    ''' <see cref="IStrategy"/> with a defined throughput timeframe.
    ''' </summary>
    ''' <param name="small">Number of small processes.</param>
    ''' <param name="medium">Number of medium processes.</param>
    ''' <param name="large">Number of large processes.</param>
    ''' <param name="strategy">Strategy to use when selecting the next ready process to utilize the CPU.</param>
    ''' <param name="windowTimeframe">Window timeframe to use for throughput.</param>
    ''' <exception cref="ArgumentOutOfRangeException"><strong>windowTimeframe</strong> is less than <strong>1</strong>.</exception>
    Public Sub New(small As Integer, medium As Integer, large As Integer, strategy As IStrategy, windowTimeframe As Integer)
        Me.New(small, medium, large, strategy)
        If windowTimeframe < 0 Then
            Throw New ArgumentOutOfRangeException(My.Resources.WindowTimeFrameLessThanZero)
        End If
        _windowTimeframe = windowTimeframe
        ThroughputSetup(_windowTimeframe)
    End Sub


    ''' <summary>
    ''' Gets the current time the simulation has been running for.
    ''' </summary>
    Public Property Time() As Integer
        Get
            Return m_Time
        End Get
        Private Set
            m_Time = Value
        End Set
    End Property
    Private m_Time As Integer

    ''' <summary>
    ''' Gets the total idle CPU time of the simulation.
    ''' </summary>
    Public Property IdleCpuTime() As Integer
        Get
            Return m_IdleCpuTime
        End Get
        Private Set
            m_IdleCpuTime = Value
        End Set
    End Property
    Private m_IdleCpuTime As Integer

    ''' <summary>
    ''' Gets the total busy CPU time of the simulation.
    ''' </summary>
    ''' 
    Public ReadOnly Property BusyCpuTime() As Integer
        Get
            Return Time - IdleCpuTime
        End Get
    End Property

    ''' <summary>
    ''' Gets the process load being used for the simulation.
    ''' </summary>
    Public ReadOnly Property ProcessLoad() As ProcessLoad
        Get
            Return _processLoad
        End Get
    End Property

    ''' <summary>
    ''' Gets the metrics for a simulation run.
    ''' </summary>
    Public ReadOnly Property Metrics() As Dictionary(Of Integer, Process)
        Get
            Return _metrics
        End Get
    End Property

    ''' <summary>
    ''' Gets the Id of the <see cref="Runner"/>.
    ''' </summary>
    Public ReadOnly Property Id() As Guid
        Get
            Return _id
        End Get
    End Property

    ''' <summary>
    ''' Gets the strategy being used by the <see cref="Runner"/>.
    ''' </summary>
    Public ReadOnly Property Strategy() As String
        Get
            Return _strategy.ToString()
        End Get
    End Property

    ''' <summary>
    ''' Gets the window time frame in ns used for the throughput window.
    ''' </summary>
    Public ReadOnly Property WindowTimeframe() As Integer
        Get
            Return _windowTimeframe
        End Get
    End Property

    ''' <summary>
    ''' Occurs when a <see cref="Process"/> in <see cref="Runner.ProcessLoad"/> utilizes the CPU for the first time.
    ''' </summary>
    Public Event ProcessStarted As EventHandler(Of ProcessStartedEventArgs)

    ''' <summary>
    ''' Occurs when a <see cref="Process"/> in <see cref="Runner.ProcessLoad"/> resumes utilization of the CPU.
    ''' </summary>
    Public Event ProcessResumed As EventHandler(Of ProcessResumedEventArgs)

    ''' <summary>
    ''' Occurs when a <see cref="Process"/> in <see cref="Runner.ProcessLoad"/> has utilized enough CPU time to voluntarily release the CPU.
    ''' </summary>
    Public Event ProcessCompleted As EventHandler(Of ProcessCompletedEventArgs)

    ''' <summary>
    ''' Occurs when a <see cref="Process"/> in <see cref="Runner.ProcessLoad"/> is preempted by another <see cref="Process"/>.
    ''' </summary>
    Public Event ProcessPreempted As EventHandler(Of ProcessPreemptedEventArgs)

    ''' <summary>
    ''' Occurs when the <see cref="Runner"/> starts.
    ''' </summary>
    Public Event Started As EventHandler(Of RunnerStartedEventArgs)

    ''' <summary>
    ''' Occurs when the <see cref="Runner"/> completes.
    ''' </summary>
    Public Event Completed As EventHandler(Of RunnerCompletedEventArgs)

    '''<summary>
    ''' Gets the % CPU Utilization of the simulation.
    '''</summary>
    '''<returns>% CPU utilization.</returns>
    '''<exception cref="InvalidOperationException">Attempt to get the % CPU utilizaton <strong>before</strong>
    ''' the simulation has completed.</exception>
    Public ReadOnly Property CpuUtilization() As Double
        Get
            If Not _simulationCompleted Then
                Throw New InvalidOperationException(My.Resources.CpuUtilizationBeforeSimulationCompleted)
            End If

            Return Math.Round((CDbl(BusyCpuTime) / Time) * 100, 2)
        End Get
    End Property

    ''' <summary>
    ''' Gets the windows of throughput, each value in the <see cref="List{T}"/> is the number of processes that completed in that window.
    ''' </summary>
    ''' <exception cref="InvalidOperationException">ThroughputWindows accessed <strong>before</strong> the simulation has completed.</exception>
    Public ReadOnly Property ThroughputWindows() As List(Of Integer)
        Get
            If Not _simulationCompleted Then
                Throw New InvalidOperationException(My.Resources.ThroughputWindowsBeforeCompletion)
            End If
            Return _throughputWindows
        End Get
    End Property

    ''' <summary>
    ''' Initializes the data structures required for gathering throughput metrics.
    ''' </summary>
    Private Sub ThroughputSetup(windowTimeframe As Integer)
        If windowTimeframe < 1 Then
            _windowTimeframe = 4 * CInt(Math.Floor(GetBurstTimeMean()))
        End If
        _windowUpperBound = Time + _windowTimeframe
        _throughputWindows = New List(Of Integer)()
        _windowCompletions = 0
    End Sub

    ''' <summary>
    ''' Wires the <see cref="Process.Started"/> and <see cref="Process.Completed"/> events for each <see cref="Process"/> in <see cref="ProcessLoad"/>
    ''' to the correct handler method.
    ''' </summary>
    Private Sub WireProcessEvents()
        For Each process As Process In _processLoad
            AddHandler process.Started, AddressOf Process_Started
            AddHandler process.Completed, AddressOf Process_Completed
        Next
    End Sub

    ''' <summary>
    ''' Raises <see cref="ProcessStarted"/> event.
    ''' </summary>
    ''' <param name="e">Data associated with a <see cref="Process"/> when started.</param>
    Private Sub OnProcessStarted(e As ProcessStartedEventArgs)
        RaiseEvent ProcessStarted(Me, e)
    End Sub

    ''' <summary>
    ''' Invoked when a process in the <see cref="ProcessLoad"/> utilizes the CPU for the first time.
    ''' </summary>
    ''' <param name="sender">Sender of event.</param>
    ''' <param name="e">Data associated with the <see cref="Process"/> when started.</param>
    Private Sub Process_Started(sender As Object, e As ProcessStartedEventArgs)
        OnProcessStarted(e)
    End Sub

    ''' <summary>
    ''' Raises <see cref="ProcessCompleted"/> event.
    ''' </summary>
    ''' <param name="e">Data associated with a <see cref="Process"/> when completed.</param>
    Private Sub OnProcessCompleted(e As ProcessCompletedEventArgs)
        RaiseEvent ProcessCompleted(Me, e)
        If e.CompletionTime <= _windowUpperBound Then
            _windowCompletions += 1
        End If
    End Sub

    ''' <summary>
    ''' Invoked when a process in the <see cref="ProcessLoad"/> has utilized enough CPU time to voluntarily release
    ''' the CPU.
    ''' </summary>
    ''' <param name="sender">Sender of event.</param>
    ''' <param name="e">Data associated with the <see cref="Process"/> when completed.</param>
    Private Sub Process_Completed(sender As Object, e As ProcessCompletedEventArgs)
        OnProcessCompleted(e)
    End Sub

    ''' <summary>
    ''' Raises <see cref="Started"/> event.
    ''' </summary>
    ''' <param name="e">Data associated with runner when started.</param>
    Private Sub OnStarted(e As RunnerStartedEventArgs)
        RaiseEvent Started(Me, e)
    End Sub

    ''' <summary>
    ''' Raises <see cref="Completed"/> event.
    ''' </summary>
    ''' <param name="e">Data associated with runner when completed.</param>
    Private Sub OnCompleted(e As RunnerCompletedEventArgs)
        RaiseEvent Completed(Me, e)
    End Sub

    ''' <summary>
    ''' Raises <see cref="ProcessResumed"/> event.
    ''' </summary>
    ''' <param name="e">Data associated with <see cref="Process"/> when it resumes utilization of the CPU.</param>
    Private Sub OnProcessResumed(e As ProcessResumedEventArgs)
        RaiseEvent ProcessResumed(Me, e)
    End Sub

    ''' <summary>
    ''' Raises <see cref="ProcessPreempted"/> event.
    ''' </summary>
    ''' <param name="e">Data associated with <see cref="Process"/> when preemption occurs.</param>
    Private Sub OnProcessPreempted(e As ProcessPreemptedEventArgs)
        RaiseEvent ProcessPreempted(Me, e)
    End Sub

    ''' <summary>
    ''' Invokes the simulation.
    ''' </summary>
    Public Sub Run()
        OnStarted(New RunnerStartedEventArgs(_processLoad.Count, _strategy))
        _strategy.Execute(Me)
        For Each process In _metrics
            process.Value.WaitTime = GetWaitTime(process.Value)
        Next
        OnCompleted(New RunnerCompletedEventArgs(_processLoad.Count, _strategy, Time))
        ' check to see if a process(s) finished in a time window that spanned past the total simulation time
        If _windowCompletions <> 0 Then
            _throughputWindows.Add(_windowCompletions)
        End If
        _simulationCompleted = True
    End Sub

    ''' <summary>
    ''' Logs the metrics of a <see cref="Process"/> to <see cref="Metrics"/> using the <see cref="Process.Id"/> as the key.
    ''' </summary>
    ''' <param name="process">Process to log the metrics of.</param>
    Public Sub LogProcessMetrics(process As Process)
        _metrics(process.Id) = process
    End Sub

    ''' <summary>
    ''' Utilizes the CPU for a single ns updating the executing <see cref="Process.Data"/> and <see cref="Runner"/> 
    ''' properties accordingly.
    ''' </summary>
    ''' <param name="process">Process to utilize the CPU.</param>
    Public Sub UtilizeCpu(process As Process)

        If process IsNot previousProcess Then

            process.CpuActivity.Add(New Pair(Of Integer, Integer)() With {.First = Time})
            If IsPreempted(previousProcess) Then
                OnProcessPreempted(New ProcessPreemptedEventArgs(previousProcess.Id, previousProcess.BurstTime, previousProcess.ArrivalTime, previousProcess.Priority))
            End If
            If IsResuming(process) Then
                OnProcessResumed(New ProcessResumedEventArgs(process.Id, process.BurstTime, process.ArrivalTime, process.Priority, process.StartTime))
            End If
            previousProcess = process
        End If

        process.CpuActivity(process.CpuActivity.Count - 1).Second = Time + 1
        Dim utilized As Integer = process.Data.UtilizedCpuTime + 1
        process.Data = New ProcessRunData(utilized, System.Threading.Interlocked.Increment(Time))

        ' add the number of processes to _throughputWindows if we are at the end of the current window bound
        If Time = _windowUpperBound Then
            _throughputWindows.Add(_windowCompletions)
            _windowCompletions = 0
            _windowUpperBound += _windowTimeframe
        End If
    End Sub

    ''' <summary>
    ''' Determines whether or not a process has been preempted by another process.
    ''' </summary>
    ''' <param name="process">Process to check if it has been preempted.</param>
    ''' <returns>True if the process has been preempted, false otherwise.</returns>
    Private Shared Function IsPreempted(ByRef process As Process) As Boolean
        Return process IsNot Nothing AndAlso process.Data.UtilizedCpuTime > 0 AndAlso process.Data.UtilizedCpuTime < process.BurstTime
    End Function

    ''' <summary>
    ''' Determines whether or not a process is resuming CPU utilization.
    ''' </summary>
    ''' <param name="process">Process to check if it is resuming CPU utilization.</param>
    ''' <returns>True if the process is resuming, false otherwise.</returns>
    Private Function IsResuming(ByRef process As Process) As Boolean
        Return process.Data.UtilizedCpuTime > 0 AndAlso (Time - process.CpuActivity(process.CpuActivity.Count - 1).Second) > 0
    End Function

    ''' <summary>
    ''' Utilizes the CPU for a defined duration updating the executing <see cref="Process.Data"/> and <see cref="Runner"/> 
    ''' properties accordingly.
    ''' </summary>
    ''' <param name="process">Process to utilize the CPU.</param>
    ''' <param name="duration">Duration to utilize CPU for.</param>
    Public Sub UtilizeCpu(process__1 As Process, duration As Integer)
        Dim elapsed As Integer = 0
        While elapsed < duration AndAlso Not Process.IsComplete(process__1)
            UtilizeCpu(process__1)
            elapsed += 1
        End While
    End Sub

    ''' <summary>
    ''' Skips a single nanosecond (ns) of idle CPU time.
    ''' </summary>
    Public Sub SkipIdleCpuTime()
        IdleCpuTime += 1
        Time += 1
    End Sub

    ''' <summary>
    ''' Skips all idle CPU time up until the arrival time of a given process.
    ''' </summary>
    ''' <param name="process">Process to skip idle time until.</param>
    Public Sub SkipIdleCpuTime(process As Process)
        While Time < process.ArrivalTime
            IdleCpuTime += 1
            Time += 1
        End While
    End Sub

    ''' <summary>
    ''' Gets the wait time for a process.
    ''' </summary>
    ''' <param name="process">Process to get wait time for.</param>
    ''' <returns>Wait time of process.</returns>
    Private Shared Function GetWaitTime(process As Process) As Integer
        Dim totalWaitTime As Integer = 0
        If process.CpuActivity.Count > 1 Then
            Dim i As Integer = process.CpuActivity.Count - 1
            While i > 0
                totalWaitTime += process.CpuActivity(i).First - process.CpuActivity(i - 1).Second
                i -= 1
            End While
        End If
        totalWaitTime += process.StartTime - process.ArrivalTime
        Return totalWaitTime
    End Function

    ''' <summary>
    ''' Gets the average wait time for all processes executed in the simulation.
    ''' </summary>
    ''' <returns>Average wait time of all processes (ns).</returns>
    ''' <exception cref="InvalidOperationException">Mean wait time calculated <strong>before</strong> the simulation has been invoked.</exception>
    Public Function GetWaitTimeMean() As Double
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetWaitTimeMeanInvokedBeforeSimulationCompleted)
        End If

        Return _processLoad.Select(Function(x) x.WaitTime).Mean()
    End Function

    ''' <summary>
    ''' Gets the average wait time for all the processes that satisfy the specified predicate function.
    ''' </summary>
    ''' <param name="predicate">Predicate function.</param>
    ''' <returns>Average wait time of all processes that satisfy the predicate function.</returns>
    ''' <exception cref="InvalidOperationException">Mean wait time calculated <strong>before</strong> the simulation has been invoked.</exception>
    ''' <exception cref="ArgumentNullException"><strong>predicate</strong> is <strong>null</strong>.</exception>
    Public Function GetWaitTimeMean(predicate As Func(Of Process, Boolean)) As Double
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetWaitTimeMeanInvokedBeforeSimulationCompleted)
        ElseIf predicate Is Nothing Then
            Throw New ArgumentNullException("predicate")
        End If

        Return _processLoad.Where(predicate).[Select](Function(x) x.WaitTime).Mean()
    End Function

    ''' <summary>
    ''' Gets the variance for the process wait times.
    ''' </summary>
    ''' <returns>Variance for process wait times.</returns>
    ''' <exception cref="InvalidOperationException">
    ''' Variance for process wait times calculated <strong>before</strong> the simulation has been 
    ''' invoked.
    ''' </exception>
    Public Function GetWaitTimeVariance() As Double
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetVarianceInvokedBeforeSimulationInvoked)
        End If

        Return _processLoad.[Select](Function(x) x.WaitTime).Variance()
    End Function

    ''' <summary>
    ''' Gets the variance for the process wait times that satisfy the provided predicate function.
    ''' </summary>
    ''' <param name="predicate">Predicate function.</param>
    ''' <returns>Variance for process wait times.</returns>
    ''' <exception cref="InvalidOperationException">Variance for process wait times calculated <strong>before</strong> the simulation has been 
    ''' invoked.</exception>
    ''' <exception cref="ArgumentNullException"><strong>predicate</strong> is <strong>null</strong>.</exception>
    Public Function GetWaitTimeVariance(predicate As Func(Of Process, Boolean)) As Double
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetVarianceInvokedBeforeSimulationInvoked)
        ElseIf predicate Is Nothing Then
            Throw New ArgumentNullException("predicate")
        End If

        Return _processLoad.Where(predicate).[Select](Function(x) x.WaitTime).Variance()
    End Function

    ''' <summary>
    ''' Gets standard deviation of process wait times.
    ''' </summary>
    ''' <returns>Standard deviation of process wait times.</returns>
    ''' <exception cref="InvalidOperationException">
    ''' Standard deviation for process wait times calculated <strong>before</strong> the simulation has been 
    ''' invoked.
    ''' </exception>
    Public Function GetWaitTimeStandardDeviation() As Double
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetWaitTimeStandardDeviationBeforeSimulationInvoked)
        End If

        Return Math.Sqrt(GetWaitTimeVariance())
    End Function

    ''' <summary>
    ''' Gets standard deviation of process wait times that satisfy the provided predicate function.
    ''' </summary>
    ''' <param name="predicate">Predicate function.</param>
    ''' <returns>Standard deviation of process wait times.</returns>
    ''' <exception cref="InvalidOperationException">
    ''' Standard deviation for process wait times calculated <strong>before</strong> the simulation has been 
    ''' invoked.
    ''' </exception>
    Public Function GetWaitTimeStandardDeviation(predicate As Func(Of Process, Boolean)) As Double
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetWaitTimeStandardDeviationBeforeSimulationInvoked)
        End If

        Return Math.Sqrt(GetWaitTimeVariance(predicate))
    End Function

    ''' <summary>
    ''' Gets the minimum wait time experienced by any process.
    ''' </summary>
    ''' <returns>Minimum wait time.</returns>
    ''' <exception cref="InvalidOperationException">Minimum wait time calculated <strong>before</strong> the simulation has been invoked.</exception>
    Public Function GetMinimumWaitTime() As Integer
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetMinimumWaitTimeBeforeSimulationInvoked)
        End If

        Dim min As Integer = Int32.MaxValue
        For Each process In _metrics
            If process.Value.WaitTime = 0 Then
                ' can't get any less than that
                Return 0
            ElseIf process.Value.WaitTime < min Then
                min = process.Value.WaitTime
            End If
        Next
        Return min
    End Function

    ''' <summary>
    ''' Gets the maximum wait time experienced by any process.
    ''' </summary>
    ''' <returns>Maximum wait time.</returns>
    ''' <exception cref="InvalidOperationException">Maximum wait time calculated <strong>before</strong> the simulation has been invoked.</exception>
    Public Function GetMaximumWaitTime() As Integer
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetMaximumWaitTimeBeforeSimulationInvoked)
        End If

        Dim max As Integer = 0
        For Each process In _metrics
            If process.Value.WaitTime > max Then
                max = process.Value.WaitTime
            End If
        Next
        Return max
    End Function

    ''' <summary>
    ''' Gets the mean turnaround time for all processes.
    ''' </summary>
    ''' <returns>Mean turnaround time.</returns>
    ''' <exception cref="InvalidOperationException">Turnaround time mean calculated <strong>before</strong> simulation has completed.</exception>
    Public Function GetTurnaroundTimeMean() As Double
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetTurnaroundTimeMeanBeforeSimulationComplete)
        End If

        Return _processLoad.[Select](Function(x) x.TurnaroundTime).Mean()
    End Function

    ''' <summary>
    ''' Gets the mean turnaround time for all processes that satisfy a predicate function.
    ''' </summary>
    ''' <param name="predicate">Predicate function.</param>
    ''' <returns>Mean turnaround time.</returns>
    ''' <exception cref="InvalidOperationException">Turnaround time mean calculated <strong>before</strong> simulation has completed.</exception>
    ''' <exception cref="ArgumentNullException"><strong>predicate</strong> is <strong>null</strong>.</exception>
    Public Function GetTurnaroundTimeMean(predicate As Func(Of Process, Boolean)) As Double
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetTurnaroundTimeMeanBeforeSimulationComplete)
        ElseIf predicate Is Nothing Then
            Throw New ArgumentNullException("predicate")
        End If

        Return _processLoad.Where(predicate).[Select](Function(x) x.TurnaroundTime).Mean()
    End Function

    ''' <summary>
    ''' Gets the mean response time for all processes.
    ''' </summary>
    ''' <returns>Mean response time.</returns>
    ''' <exception cref="InvalidOperationException">Response time mean calculated <strong>before</strong> simulation has completed.</exception>
    Public Function GetResponseTimeMean() As Double
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetResponseTimeMeanBeforeSimulationComplete)
        End If

        Return _processLoad.[Select](Function(x) x.ResponseTime).Mean()
    End Function

    ''' <summary>
    ''' Gets the mean response time for all processes that satisfy a predicate function.
    ''' </summary>
    ''' <param name="predicate">Predicate function.</param>
    ''' <returns>Mean response time.</returns>
    ''' <exception cref="InvalidOperationException">Response time mean calculated <strong>before</strong> simulation has completed.</exception>
    ''' <exception cref="ArgumentNullException"><strong>predicate</strong> is <strong>null</strong>.</exception>
    Public Function GetResponseTimeMean(predicate As Func(Of Process, Boolean)) As Double
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetResponseTimeMeanBeforeSimulationComplete)
        ElseIf predicate Is Nothing Then
            Throw New ArgumentNullException("predicate")
        End If

        Return _processLoad.Where(predicate).[Select](Function(x) x.ResponseTime).Mean()
    End Function

    ''' <summary>
    ''' Gets the mean of all the process burst times.
    ''' </summary>
    ''' <returns>Mean burst time.</returns>
    Public Function GetBurstTimeMean() As Double
        Return _processLoad.[Select](Function(x) x.BurstTime).Mean()
    End Function

    ''' <summary>
    ''' Gets the mean throughput for a time window <em>t</em>.
    ''' </summary>
    ''' <returns>Mean process throughput.</returns>
    ''' <exception cref="InvalidOperationException">Throughput mean calculated <strong>before</strong> simulation has completed.</exception>
    Public Function GetThroughputMean() As Double
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetThroughputMeanBeforeSimulationComplete)
        End If

        Return _throughputWindows.Mean()
    End Function

    ''' <summary>
    ''' Gets the response time variance for all processes.
    ''' </summary>
    ''' <returns>Response time variance.</returns>
    ''' <exception cref="InvalidOperationException">Response time variance is calculated <strong>before</strong> the simulation has completed.</exception>
    Public Function GetResponseTimeVariance() As Double
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetResponseTimeVarianceBeforeSimulationCompleted)
        End If

        Return _processLoad.[Select](Function(x) x.ResponseTime).Variance()
    End Function

    ''' <summary>
    ''' Gets the response time variance for all processes that satisfy a predicate function.
    ''' </summary>
    ''' <param name="predicate">Predicate function.</param>
    ''' <returns>Respone time variance.</returns>
    ''' <exception cref="InvalidOperationException">Response time variance is calculated <strong>before</strong> the simulation has completed.</exception>
    ''' <exception cref="ArgumentNullException"><strong>predicate</strong> is <strong>null</strong>.</exception>
    Public Function GetResponseTimeVariance(predicate As Func(Of Process, Boolean)) As Double
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetResponseTimeVarianceBeforeSimulationCompleted)
        ElseIf predicate Is Nothing Then
            Throw New ArgumentNullException("predicate")
        End If

        Return _processLoad.Where(predicate).[Select](Function(x) x.ResponseTime).Variance()
    End Function

    ''' <summary>
    ''' Gets the response time standard deviation for all processes.
    ''' </summary>
    ''' <returns>Response time standard deviation.</returns>
    ''' <exception cref="InvalidOperationException">Response time standard deviation is calculated <strong>before</strong> the simulation 
    ''' has completed.</exception>
    Public Function GetResponseTimeStandardDeviation() As Double
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetResponseTimeStandardDeviationBeforeSimulationCompleted)
        End If

        Return Math.Sqrt(GetResponseTimeVariance())
    End Function

    ''' <summary>
    ''' Gets the response time standard deviation for all processes that satisfy a predicate function.
    ''' </summary>
    ''' <param name="predicate">Predicate function.</param>
    ''' <returns>Response time standard deviation.</returns>
    ''' <exception cref="InvalidOperationException">Response time standard deviation is calculated <strong>before</strong> the simulation 
    ''' has completed.</exception>
    Public Function GetResponseTimeStandardDeviation(predicate As Func(Of Process, Boolean)) As Double
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetResponseTimeStandardDeviationBeforeSimulationCompleted)
        End If

        Return Math.Sqrt(GetResponseTimeVariance(predicate))
    End Function

    ''' <summary>
    ''' Gets the turnaround time variance for all processes.
    ''' </summary>
    ''' <returns>Turnaround time variance.</returns>
    ''' <exception cref="InvalidOperationException">Turnaround time variance is calculated <strong>before</strong> the simulation has completed.</exception>
    Public Function GetTurnaroundTimeVariance() As Double
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetTurnaroundTimeVarianceBeforeSimulationCompleted)
        End If

        Return _processLoad.[Select](Function(x) x.TurnaroundTime).Variance()
    End Function

    ''' <summary>
    ''' Gets the turnaround time variance for all processes that satisfy a predicate function.
    ''' </summary>
    ''' <param name="predicate">Predicate function.</param>
    ''' <exception cref="InvalidOperationException">Turnaround time variance is calculated <strong>before</strong> the simulation has completed.</exception>
    ''' <exception cref="ArgumentNullException"><strong>predicate</strong> is <strong>null</strong>.</exception>
    Public Function GetTurnaroundTimeVariance(predicate As Func(Of Process, Boolean)) As Double
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetTurnaroundTimeVarianceBeforeSimulationCompleted)
        ElseIf predicate Is Nothing Then
            Throw New ArgumentNullException("predicate")
        End If

        Return _processLoad.Where(predicate).[Select](Function(x) x.TurnaroundTime).Variance()
    End Function

    ''' <summary>
    ''' Gets the turnaround time standard deviation for all processes.
    ''' </summary>
    ''' <returns>Turnaround time standard deviation.</returns>
    ''' <exception cref="InvalidOperationException">Turnaround time standard deviation is calculated <strong>before</strong> the simulation 
    ''' has completed.</exception>
    Public Function GetTurnaroundTimeStandardDeviation() As Double
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetTurnaroundTimeStandardDeviationSimulationCompleted)
        End If

        Return Math.Sqrt(GetTurnaroundTimeVariance())
    End Function

    ''' <summary>
    ''' Gets the turnaround time standard deviation for all processes that satisfy a predicate function.
    ''' </summary>
    ''' <param name="predicate">Predicate function.</param>
    ''' <returns>Turnaround time standard deviation.</returns>
    ''' <exception cref="InvalidOperationException">Turnaround time standard deviation is calculated <strong>before</strong> the simulation 
    ''' has completed.</exception>
    Public Function GetTurnaroundTimeStandardDeviation(predicate As Func(Of Process, Boolean)) As Double
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetTurnaroundTimeStandardDeviationSimulationCompleted)
        End If

        Return Math.Sqrt(GetTurnaroundTimeVariance(predicate))
    End Function

    ''' <summary>
    ''' Gets the throughput variance for a time window <em>t</em>.
    ''' </summary>
    ''' <returns>Throughput variance.</returns>
    ''' <exception cref="InvalidOperationException">Throughput time variance is calculated <strong>before</strong> the simulation 
    ''' has completed.</exception>
    Public Function GetThroughputVariance() As Double
        If Not _simulationCompleted Then
            Throw New InvalidOperationException(My.Resources.GetThroughputVarianceBeforeSimulationComplete)
        End If

        Return _throughputWindows.Variance()
    End Function

    ''' <summary>
    ''' Gets the throughput standard deviation for a time window <em>t</em>.
    ''' </summary>
    ''' <returns>Throughput standard deviation.</returns>
    Public Function GetThroughputStandardDeviation() As Double
        If Not _simulationCompleted Then
            'Throw New InvalidOperationException(My.Resources.GetThroughputStandardDeviationBeforeSimulationComplete)
        End If

        Return Math.Sqrt(GetThroughputVariance())
    End Function

End Class


