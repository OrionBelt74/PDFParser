Module TIFFDefs
    Public Enum ByteOrder
        LittleEndian = 0
        BitEndian = 1

    End Enum
    Public Enum TIFFFieldType
        TYPE_BYTE = 1
        TYPE_ASCII = 2
        TYPE_SHORT = 3
        TYPE_LONG = 4
        TYPE_RATIONAL = 5
        TYPE_SBYTE = 6
        TYPE_UNDEFINED = 7
        TYPE_SSHORT = 8
        TYPE_SLONG = 9
        TYPE_SRATIONAL = 10
        TYPE_FLOAT = 11
        TYPE_DOUBLE = 12
    End Enum

    Public Enum PhotometricInterpretation
        WhiteIsZero = 0
        BlackIsZero = 1
    End Enum
    Public Enum Compression
        NoCompression = 1
        CCITTGroup3 = 2
        T4Encoding = 3
        T6Encoding = 4
        PackBits = 32773
    End Enum

    Public Sub WriteToBuffer_UINT32(num As UInt32, ByRef offset As Integer, _ByteOrder As ByteOrder, _Bytes() As Byte)
        Select Case _ByteOrder
            Case ByteOrder.BitEndian 'MSB first
                _Bytes(offset) = (num And &HFF000000) >> 24
                _Bytes(offset + 1) = (num And &HFF0000) >> 16
                _Bytes(offset + 2) = (num And &HFF00) >> 8
                _Bytes(offset + 3) = (num And &HFF)


            Case ByteOrder.LittleEndian 'LSB first
                _Bytes(offset + 3) = (num And &HFF)
                _Bytes(offset + 2) = (num And &HFF00) >> 8
                _Bytes(offset + 1) = (num And &HFF0000) >> 16
                _Bytes(offset) = (num And &HFF000000) >> 24
        End Select
        offset += 4
    End Sub

    Public Sub WriteToBuffer_INT32(num As Int32, ByRef offset As Integer, _ByteOrder As ByteOrder, _Bytes() As Byte)
        Select Case _ByteOrder
            Case ByteOrder.BitEndian 'MSB first
                _Bytes(offset) = (num And &HFF000000) >> 24
                _Bytes(offset + 1) = (num And &HFF0000) >> 16
                _Bytes(offset + 2) = (num And &HFF00) >> 8
                _Bytes(offset + 3) = (num And &HFF)


            Case ByteOrder.LittleEndian 'LSB first
                _Bytes(offset + 3) = (num And &HFF)
                _Bytes(offset + 2) = (num And &HFF00) >> 8
                _Bytes(offset + 1) = (num And &HFF0000) >> 16
                _Bytes(offset) = (num And &HFF000000) >> 24
        End Select
        offset += 4
    End Sub

    Public Sub WriteToBuffer_INT16(num As Int16, ByRef offset As Integer, _ByteOrder As ByteOrder, _Bytes() As Byte)
        Select Case _ByteOrder
            Case ByteOrder.BitEndian 'MSB first
                _Bytes(offset) = (num And &HFF00) >> 8
                _Bytes(offset + 1) = (num And &HFF)

            Case ByteOrder.LittleEndian 'LSB first
                _Bytes(offset + 1) = (num And &HFF00) >> 8
                _Bytes(offset) = (num And &HFF)
        End Select
        offset += 2
    End Sub
    Public Sub WriteToBuffer_UINT16(num As UInt16, ByRef offset As Integer, _ByteOrder As ByteOrder, _Bytes() As Byte)
        Select Case _ByteOrder
            Case ByteOrder.BitEndian 'MSB first
                _Bytes(offset) = (num And &HFF00) >> 8
                _Bytes(offset + 1) = (num And &HFF)

            Case ByteOrder.LittleEndian 'LSB first
                _Bytes(offset + 1) = (num And &HFF00) >> 8
                _Bytes(offset) = (num And &HFF)
        End Select
        offset += 2
    End Sub

    Public Sub WriteToBuffer_Double(num As Int64, ByRef offset As Integer, _ByteOrder As ByteOrder, _Bytes() As Byte)
        Select Case _ByteOrder
            Case ByteOrder.BitEndian 'MSB first
                _Bytes(offset) = (num And &HFF00000000000000) >> 56
                _Bytes(offset + 1) = (num And &HFF000000000000) >> 48
                _Bytes(offset + 2) = (num And &HFF0000000000) >> 40
                _Bytes(offset + 3) = (num And &HFF00000000) >> 32
                _Bytes(offset + 4) = (num And &HFF000000) >> 24
                _Bytes(offset + 5) = (num And &HFF0000) >> 16
                _Bytes(offset + 6) = (num And &HFF00) >> 8
                _Bytes(offset + 7) = (num And &HFF)

            Case ByteOrder.LittleEndian 'LSB first
                _Bytes(offset + 7) = (num And &HFF00000000000000) >> 56
                _Bytes(offset + 6) = (num And &HFF000000000000) >> 48
                _Bytes(offset + 5) = (num And &HFF0000000000) >> 40
                _Bytes(offset + 4) = (num And &HFF00000000) >> 32
                _Bytes(offset + 3) = (num And &HFF000000) >> 24
                _Bytes(offset + 2) = (num And &HFF0000) >> 16
                _Bytes(offset + 1) = (num And &HFF00) >> 8
                _Bytes(offset) = (num And &HFF)
        End Select
        offset += 8
    End Sub
End Module
