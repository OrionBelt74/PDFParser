Friend Class CCITTFaxDecodeFilter
    Private _K As Integer
    Private _EndOfLine As Boolean
    Private _EncodedByteAlign As Boolean
    Private _Columns As Integer
    Private _Rows As Integer
    Private _EndOfBlock As Boolean
    Private _BlackIs1 As Boolean
    Private _DamagedRowsBeforeError As Integer
    Private _DecMode As DecodeMode
    Private _Buff() As Byte
    Private _BuffSize As Integer
    Private _BuffByteIndex As Integer
    Private _BuffBitOffset As Integer
    Public Enum DecodeMode
        Pure2D = 0
        Pure1D
        Mixed
    End Enum
    Public Enum CodingMode
        Pass = 0
        Horizontal
        Vertical
    End Enum

    Property K As Integer
        Set(value As Integer)
            _K = value
            If K < 0 Then
                DecMode = DecodeMode.Pure2D
            ElseIf K > 0 Then
                DecMode = DecodeMode.Mixed
            Else
                DecMode = DecodeMode.Pure1D
            End If
        End Set
        Get
            Return _K
        End Get
    End Property
    Property DecMode As DecodeMode
        Set(value As DecodeMode)
            _DecMode = value
        End Set
        Get
            Return _DecMode
        End Get
    End Property

    Property EndOfLine As Boolean
        Set(value As Boolean)
            _EndOfLine = value
        End Set
        Get
            Return _EndOfLine
        End Get
    End Property
    Property EncodedByteAlign As Boolean
        Set(value As Boolean)
            _EncodedByteAlign = value
        End Set
        Get
            Return _EncodedByteAlign
        End Get
    End Property
    Property EndOfBlock As Boolean
        Set(value As Boolean)
            _EndOfBlock = value
        End Set
        Get
            Return _EndOfBlock
        End Get
    End Property
    Property BlackIs1 As Boolean
        Set(value As Boolean)
            _BlackIs1 = value
        End Set
        Get
            Return _BlackIs1
        End Get

    End Property
    Property Columns As Integer
        Set(value As Integer)
            _Columns = value
        End Set
        Get
            Return _Columns
        End Get

    End Property
    Property Rows As Integer
        Set(value As Integer)
            _Rows = value
        End Set
        Get
            Return _Rows
        End Get

    End Property
    Property DamagedRowsBeforeError As Integer
        Set(value As Integer)
            _DamagedRowsBeforeError = value
        End Set
        Get
            Return _DamagedRowsBeforeError
        End Get

    End Property
    Sub New()
        K = 0
        EndOfLine = False
        EncodedByteAlign = False
        Columns = 1728
        Rows = 0
        EndOfBlock = True
        BlackIs1 = False
        DamagedRowsBeforeError = 0

        InitLUTs()
    End Sub
    Private Sub InitLUTs()
        Dim codesWhite As New List(Of Integer)
        Dim codesBlack As New List(Of Integer)

        For n = 0 To 255
            TERMINATING_CODES_WHITERUN(n) = -1
            TERMINATING_CODES_BLACKRUN(n) = -1
        Next

        For n = 0 To 63
            If codesWhite.Contains(TERMINATING_CODES_WHITERUN_CTRL(n)) Or TERMINATING_CODES_WHITERUN_CTRL(n) = 1 Then
                MsgBox("ERROR!!!!")
            Else
                codesWhite.Add(TERMINATING_CODES_WHITERUN_CTRL(n))
            End If

            If codesBlack.Contains(TERMINATING_CODES_BLACKRUN_CTRL(n)) Or TERMINATING_CODES_WHITERUN_CTRL(n) = 1 Then
                MsgBox("ERROR!!!!")
            Else
                codesBlack.Add(TERMINATING_CODES_BLACKRUN_CTRL(n))
            End If

            TERMINATING_CODES_WHITERUN(TERMINATING_CODES_WHITERUN_CTRL(n)) = n
            TERMINATING_CODES_BLACKRUN(TERMINATING_CODES_BLACKRUN_CTRL(n)) = n
        Next

    End Sub
    'Private Function ReadFromBuffer(numBits As Integer, ByRef Bits As UInt32) As Boolean

    '    Dim numBytes As Integer = Math.Ceiling((numBits + _BuffBitOffset) / 8)
    '    Dim tmpRes As UInt32
    '    Dim byteMask As Byte
    '    Dim readByteOffset As Byte
    '    Dim tmpValue As UInt32
    '    Dim nb As Integer
    '    Dim accBits As Integer = 0
    '    Dim res As Boolean = (_BuffByteIndex + numBytes <= _BuffSize)

    '    tmpRes = 0
    '    For n = 0 To numBytes - 1

    '        If n = 0 Then
    '            nb = Math.Min(numBits, 8 - _BuffBitOffset)
    '            readByteOffset = (8 - _BuffBitOffset) - nb
    '            byteMask = (Math.Pow(2, nb) - 1) << readByteOffset
    '            tmpValue = (_Buff(_BuffByteIndex + n) And byteMask) >> readByteOffset
    '            accBits += nb
    '            tmpRes = tmpValue

    '            'update values for next iteration
    '            If n = numBytes - 1 Then
    '                If nb = 8 Then
    '                    _BuffBitOffset = 0
    '                    _BuffByteIndex += numBytes
    '                Else
    '                    _BuffBitOffset = nb
    '                    _BuffByteIndex += numBytes - 1
    '                End If
    '            End If
    '        ElseIf n = numBytes - 1 Then
    '            'Last
    '            nb = numBits - accBits
    '            readByteOffset = 8 - nb
    '            byteMask = (Math.Pow(2, nb) - 1) << readByteOffset
    '            tmpValue = (_Buff(_BuffByteIndex + n) And byteMask) >> readByteOffset
    '            tmpRes = tmpRes << nb
    '            tmpRes += tmpValue

    '            'update values for next iteration
    '            If nb = 8 Then
    '                _BuffBitOffset = 0
    '                _BuffByteIndex += numBytes
    '            Else
    '                _BuffBitOffset = nb
    '                _BuffByteIndex += numBytes - 1
    '            End If

    '        Else
    '            'Intermediate bytes
    '            nb = 8
    '            tmpValue = _Buff(_BuffByteIndex + n)
    '            tmpRes = tmpRes << 8
    '            tmpRes += tmpValue
    '            accBits += 8
    '        End If
    '    Next
    '    Bits = tmpRes
    '    Return res

    'End Function
    Public Sub DecodeBuffer(Buffer() As Byte, buffLength As Integer)
        _Buff = Buffer
        _BuffSize = buffLength


        ' K < 0 --- Pure two-dimensional encoding (Group 4)
        ' K = 0 --- Pure one-dimensional encoding (Group 3, 1-D)
        ' K > 0 --- Mixed one- and two-dimensional encoding (Group 3, 2-D)






        'Return


        'Dim ReadWhiteLUT As Boolean
        'Dim numBitsReading As Integer = 1
        'Dim currWord As UInt16
        'Dim currWordWhite As UInt16
        'Dim currWordBlack As UInt16
        'Dim currMaskWhite As UInt16
        'Dim currMaskBlack As UInt16
        'Dim nextByteIndex As Integer = 0
        'Dim lutByteWhite As Byte
        'Dim lutByteBlack As Byte
        'Dim currCpdeWord As Byte
        'Dim Done As Boolean = False
        'Dim CodeWordFound As Boolean = False
        'Dim WhiteRunLength As Integer
        'Dim BlackRunLength As Integer
        'currWord = Buffer(0)
        'currWord = (currWord <<8)
        'currWord += Buffer(1)
        'nextByteIndex = 2

        'Dim minNumBitsWhite As Integer = 4
        'Dim minNumBitsBlack As Integer = 2
        'ReadWhiteLUT = True
        'While Not Done
        '    CodeWordFound = False
        '    If ReadWhiteLUT Then numBitsReading = minNumBitsWhite Else numBitsReading = minNumBitsBlack

        '    While Not CodeWordFound

        '        If ReadWhiteLUT Then

        '            If numBitsReading < 8 Then
        '                'extend the number with 0s on the left
        '                currMaskWhite = (Math.Pow(2, numBitsReading) - 1) << (16 - numBitsReading)
        '                currWordWhite = currWord And currMaskWhite
        '                currWordWhite = (currWordWhite >> 8)
        '                lutByteWhite = currWordWhite

        '                WhiteRunLength = TERMINATING_CODES_WHITERUN(lutByteWhite)
        '                CodeWordFound = WhiteRunLength <> -1
        '                If CodeWordFound Then

        '                Else
        '                    numBitsReading += 1
        '                End If
        '            End If

        '        Else


        '        End If
        '    End While
        'End While

    End Sub

    'Private Sub Decode2D()
    '    Dim v As UInt32 = 0
    '    _BuffByteIndex = 0
    '    _BuffBitOffset = 0

    '    Dim PassModeMask As UInt32 = 1 << 28
    '    Dim PassModeVal As UInt32 = 1 << 28
    '    Dim HModeMask As UInt32 = 1 << 29
    '    Dim HModeVal As UInt32 = 1 << 29
    '    Dim VMode_0 As UInt32 = 1UI << 31
    '    Dim VMode_R1_L1 As UInt32 = 7UI << 29
    '    Dim VMode_R2_L2 As UInt32 = 3UI << 26
    '    Dim VMode_R3_L3 As UInt32 = 3UI << 25
    '    Dim VMode_R1 As UInt32 = 3UI << 29
    '    Dim VMode_L1 As UInt32 = 2UI << 29
    '    Dim VMode_R2 As UInt32 = 3UI << 29
    '    Dim VMode_L2 As UInt32 = 2UI << 29
    '    Dim VMode_R3 As UInt32 = 1UI << 31
    '    Dim VMode_L3 As UInt32 = 1UI << 31
    '    Dim EOFBMask As UInt32 = &H1001 << 8

    '    Dim UsedBytes As UInt32 = 0
    '    ReadFromBuffer(32, v)
    '    UsedBytes = UsedBytes Or v

    '    Dim done As Boolean = False

    '    While Not done
    '        If ((UsedBytes And VMode_0) = VMode_0) Then
    '            'V0
    '            ReadFromBuffer(1, v)
    '            UsedBytes = UsedBytes << 1
    '            UsedBytes = UsedBytes Or v

    '        ElseIf ((v And VMode_R1_L1) = VMode_R1) Then
    '            'VR1
    '            ReadFromBuffer(3, v)
    '            UsedBytes = UsedBytes << 3
    '            UsedBytes = UsedBytes Or v

    '        ElseIf ((v And VMode_R1_L1) = VMode_L1) Then
    '            'VL1
    '            ReadFromBuffer(3, v)
    '            UsedBytes = UsedBytes << 3
    '            UsedBytes = UsedBytes Or v

    '        ElseIf ((v And VMode_R2_L2) = VMode_R2) Then
    '            'VR2
    '            ReadFromBuffer(6, v)
    '            UsedBytes = UsedBytes << 6
    '            UsedBytes = UsedBytes Or v

    '        ElseIf ((v And VMode_R2_L2) = VMode_L2) Then
    '            'VL2
    '            ReadFromBuffer(6, v)
    '            UsedBytes = UsedBytes << 6
    '            UsedBytes = UsedBytes Or v

    '        ElseIf ((v And VMode_R3_L3) = VMode_R3) Then
    '            'VR3
    '            ReadFromBuffer(7, v)
    '            UsedBytes = UsedBytes << 7
    '            UsedBytes = UsedBytes Or v
    '        ElseIf ((v And VMode_R3_L3) = VMode_L3) Then
    '            'VL3
    '            ReadFromBuffer(7, v)
    '            UsedBytes = UsedBytes << 7
    '            UsedBytes = UsedBytes Or v

    '        ElseIf ((v And PassModeMask) = PassModeVal) Then
    '            'P
    '            ReadFromBuffer(4, v)
    '            UsedBytes = UsedBytes << 4
    '            UsedBytes = UsedBytes Or v

    '        ElseIf ((v And HModeMask) = HModeVal) Then
    '            'H
    '            Dim a = 1

    '        ElseIf ((v And EOFBMask) = EOFBMask) Then
    '            done = True
    '        End If

    '    End While
    'End Sub


End Class









