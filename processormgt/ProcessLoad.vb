Imports System.Collections.Generic

''' <summary>
''' Collection of <see cref="Process"/>.
''' </summary>
Public Class ProcessLoad 
    Inherits List(Of Process)

    ''' <summary>
    ''' Initializes a new instance of <see cref="ProcessLoad"/>.
    ''' </summary>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' Initializes a new instance of <see cref="ProcessLoad"/> with the items from an <see cref="IEnumerable{Process}"/>.
    ''' </summary>
    ''' <param name="processes">Processes to populate <see cref="ProcessLoad"/> with.</param>
    Public Sub New(processes As IEnumerable(Of Process))
        MyBase.New(processes)
    End Sub

End Class



