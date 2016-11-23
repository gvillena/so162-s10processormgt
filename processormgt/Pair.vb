
''' <summary>
''' Pair.
''' </summary>
Public Class Pair(Of TFirst, TSecond)
    ''' <summary>
    ''' Gets or sets the first <see cref="System.Int32"/> in the pair.
    ''' </summary>
    Public Property First() As TFirst
        Get
            Return m_First
        End Get
        Set
            m_First = Value
        End Set
    End Property
    Private m_First As TFirst

    ''' <summary>
    ''' Gets or sets the second <see cref="System.Int32"/> in the pair.
    ''' </summary>
    Public Property Second() As TSecond
        Get
            Return m_Second
        End Get
        Set
            m_Second = Value
        End Set
    End Property
    Private m_Second As TSecond
End Class

