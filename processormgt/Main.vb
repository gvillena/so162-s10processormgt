Module Main

    Sub Main()

        Dim cpuUtilizations As List(Of Double)
        Dim id As Guid
        Dim large As Integer
        Dim medium As Integer
        Dim processCount As Integer
        Dim repeat As Integer
        Dim responseTimeStandardDeviations As List(Of Double)
        Dim small As Integer
        Dim strategy As IStrategy
        Dim turnaroundTimeStandardDeviations As List(Of Double)
        Dim waitTimeStandardDeviations As List(Of Double)
        Dim throughputs As Pairs(Of Integer, Double)
        Dim timeframe As Integer

        id = Guid.NewGuid()
        small = 2
        medium = 0
        large = 2
        strategy = New FirstComeFirstServed()
        repeat = 1
        processCount = small + medium + large
        waitTimeStandardDeviations = New List(Of Double)()
        cpuUtilizations = New List(Of Double)()
        turnaroundTimeStandardDeviations = New List(Of Double)()
        responseTimeStandardDeviations = New List(Of Double)()
        throughputs = New Pairs(Of Integer, Double)()
        timeframe = 100

        Dim runner As Runner = Nothing
        Dim processLoad As ProcessLoad = Nothing

        Dim p1 As New Process(1, 15, 1) With {.Priority = Priority.Medium}
        Dim p2 As New Process(2, 2, 1) With {.Priority = Priority.Medium}
        Dim p3 As New Process(3, 1, 1) With {.Priority = Priority.Medium}

        processLoad = New ProcessLoad()
        processLoad.Add(p3)
        processLoad.Add(p2)
        processLoad.Add(p1)

        'runner = New Runner(small, medium, large, strategy, timeframe)

        runner = New Runner(processLoad, strategy, timeframe)
        runner.Run()

        'waitTimeStandardDeviations.Add(runner.GetWaitTimeStandardDeviation())
        'cpuUtilizations.Add(runner.CpuUtilization)
        'turnaroundTimeStandardDeviations.Add(runner.GetTurnaroundTimeStandardDeviation())
        'responseTimeStandardDeviations.Add(runner.GetResponseTimeStandardDeviation())
        'throughputs.Add(New Pair(Of Integer, Double)() With {.First = runner.WindowTimeframe, .Second = runner.GetThroughputMean()})

        For Each p As Process In runner.ProcessLoad

            Console.WriteLine(p.Id)
            'Console.WriteLine(p.ArrivalTime)
            'Console.WriteLine(p.BurstTime)
            'Console.WriteLine(p.CompletionTime)
            Console.WriteLine(p.TurnaroundTime)
            Console.WriteLine()

        Next
        Console.WriteLine()
        Console.WriteLine(runner.GetTurnaroundTimeMean())
        Console.ReadKey()

    End Sub

End Module
