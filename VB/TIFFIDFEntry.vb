Friend Class TIFFIFDEntry
    Public Bytes(11) As Byte
    Private _Tag As UInt16
    Private _Count As UInt32
    Private _ByteOrder As ByteOrder
    Private _Type As TIFFFieldType
    Private _ValueOffset As UInt32
    Private _Value As Object
    Private _ValueFitsIn32Bits As Boolean
    Private _ValueSize As Integer
    Property ValueSize As Integer
        Set(value As Integer)
            _ValueSize = value
        End Set
        Get
            Return _ValueSize
        End Get
    End Property
    Property Tag As UInt16
        Set(value As UInt16)
            _Tag = value
        End Set
        Get
            Return _Tag
        End Get
    End Property
    Property ValueFitsIn32Bits As Boolean
        Set(value As Boolean)
            _ValueFitsIn32Bits = value
        End Set
        Get
            Return _ValueFitsIn32Bits
        End Get
    End Property
    Property Type As TIFFFieldType
        Set(value As TIFFFieldType)
            _Type = value
            EvalValueFits32Bits()
        End Set
        Get
            Return _Type
        End Get
    End Property
    Property ByteOrder As ByteOrder
        Set(value As ByteOrder)
            _ByteOrder = value
        End Set
        Get
            Return _ByteOrder
        End Get
    End Property
    Property Count As UInt32
        Set(value As UInt32)
            _Count = value
            EvalValueFits32Bits()
        End Set
        Get
            Return _Count
        End Get
    End Property
    Property ValueOffset As UInt32
        Set(value As UInt32)
            _ValueOffset = value
        End Set
        Get
            Return _ValueOffset
        End Get
    End Property
    Property Value As Object
        Set(val As Object)
            _Value = val
        End Set
        Get
            Return _Value
        End Get
    End Property
    Sub New()
        ByteOrder = ByteOrder.BitEndian
        Value = 0
        Count = 1
    End Sub
    Sub EvalValueFits32Bits()
        Select Case Type
            Case TIFFFieldType.TYPE_ASCII
                ValueFitsIn32Bits = (Count <= 4)
                ValueSize = Count

            Case TIFFFieldType.TYPE_BYTE
                ValueFitsIn32Bits = (Count <= 4)
                ValueSize = Count

            Case TIFFFieldType.TYPE_DOUBLE
                ValueFitsIn32Bits = False
                ValueSize = Count * 8

            Case TIFFFieldType.TYPE_FLOAT
                ValueFitsIn32Bits = (Count <= 1)
                ValueSize = Count * 4

            Case TIFFFieldType.TYPE_LONG
                ValueFitsIn32Bits = (Count <= 1)
                ValueSize = Count * 4

            Case TIFFFieldType.TYPE_RATIONAL
                ValueFitsIn32Bits = False
                ValueSize = Count * 8

            Case TIFFFieldType.TYPE_SBYTE
                ValueFitsIn32Bits = (Count <= 4)
                ValueSize = Count

            Case TIFFFieldType.TYPE_SHORT
                ValueFitsIn32Bits = (Count <= 2)
                ValueSize = Count * 2

            Case TIFFFieldType.TYPE_SLONG
                ValueFitsIn32Bits = (Count <= 1)
                ValueSize = Count * 4

            Case TIFFFieldType.TYPE_SRATIONAL
                ValueFitsIn32Bits = False
                ValueSize = Count * 8

            Case TIFFFieldType.TYPE_SSHORT
                ValueFitsIn32Bits = (Count <= 2)
                ValueSize = Count * 2

            Case TIFFFieldType.TYPE_UNDEFINED
                ValueFitsIn32Bits = (Count <= 4)
                ValueSize = Count
        End Select
    End Sub
    Sub Update()


        Select Case _ByteOrder
            Case ByteOrder.BitEndian
                'Tag
                Bytes(0) = (Tag And &HFF00) >> 8
                Bytes(1) = (Tag And &HFF)

                'Type
                Bytes(2) = (Type And &HFF00) >> 8
                Bytes(3) = (Type And &HFF)

                'Count
                Bytes(4) = (Count And &HFF000000) >> 24
                Bytes(5) = (Count And &HFF0000) >> 16
                Bytes(6) = (Count And &HFF00) >> 8
                Bytes(7) = (Count And &HFF)

                'Value Offset/Value
                Bytes(8) = (ValueOffset And &HFF000000) >> 24
                Bytes(9) = (ValueOffset And &HFF0000) >> 16
                Bytes(10) = (ValueOffset And &HFF00) >> 8
                Bytes(11) = (ValueOffset And &HFF)

            Case ByteOrder.LittleEndian
                'Tag
                Bytes(1) = (Tag And &HFF00) >> 8
                Bytes(0) = (Tag And &HFF)

                'Type
                Bytes(3) = (Type And &HFF00) >> 8
                Bytes(2) = (Type And &HFF)

                'Count
                Bytes(7) = (Count And &HFF000000) >> 24
                Bytes(6) = (Count And &HFF0000) >> 16
                Bytes(5) = (Count And &HFF00) >> 8
                Bytes(4) = (Count And &HFF)

                'Value Offset
                Bytes(11) = (ValueOffset And &HFF000000) >> 24
                Bytes(10) = (ValueOffset And &HFF0000) >> 16
                Bytes(9) = (ValueOffset And &HFF00) >> 8
                Bytes(8) = (ValueOffset And &HFF)





        End Select
    End Sub



    Public Sub CopyValueOffset(ByRef buffer() As Byte, ByRef offset As Integer)
        Select Case _ByteOrder
            Case ByteOrder.BitEndian
                buffer(offset) = (ValueOffset And &HFF000000) >> 24
                buffer(offset + 1) = (ValueOffset And &HFF0000) >> 16
                buffer(offset + 2) = (ValueOffset And &HFF00) >> 8
                buffer(offset + 3) = (ValueOffset And &HFF)

            Case ByteOrder.LittleEndian
                buffer(offset + 3) = (ValueOffset And &HFF000000) >> 24
                buffer(offset + 2) = (ValueOffset And &HFF0000) >> 16
                buffer(offset + 1) = (ValueOffset And &HFF00) >> 8
                buffer(offset) = (ValueOffset And &HFF)
        End Select
        offset += 4
    End Sub
    Public Sub CopyValue(ByRef buffer() As Byte, ByRef offset As Integer)

        Dim V As UInt32
        If ValueFitsIn32Bits Then V = Value Else V = ValueOffset
        Select Case Type
            Case TIFFFieldType.TYPE_ASCII
                For n = 0 To Count - 1
                    buffer(offset + n) = 0
                Next
                offset += Count

            Case TIFFFieldType.TYPE_BYTE
                For n = 0 To Count - 1
                    buffer(offset + n) = 0
                Next
                offset += Count

            Case TIFFFieldType.TYPE_DOUBLE
                For n = 0 To Count - 1
                    WriteToBuffer_Double(Value, offset, ByteOrder, buffer)
                Next
            Case TIFFFieldType.TYPE_FLOAT
                For n = 0 To Count - 1
                    WriteToBuffer_UINT32(Value, offset, ByteOrder, buffer)
                Next

            Case TIFFFieldType.TYPE_LONG
                For n = 0 To Count - 1
                    WriteToBuffer_UINT32(Value, offset, ByteOrder, buffer)
                Next

            Case TIFFFieldType.TYPE_RATIONAL
                For n = 0 To Count - 1
                    WriteToBuffer_UINT32(Value, offset, ByteOrder, buffer)
                Next

            Case TIFFFieldType.TYPE_SBYTE
                For n = 0 To Count - 1
                    buffer(offset + n) = 0
                Next
                offset += Count

            Case TIFFFieldType.TYPE_SHORT
                For n = 0 To Count - 1
                    WriteToBuffer_UINT16(Value, offset, ByteOrder, buffer)
                Next

            Case TIFFFieldType.TYPE_SLONG
                For n = 0 To Count - 1
                    WriteToBuffer_INT32(Value, offset, ByteOrder, buffer)
                Next

            Case TIFFFieldType.TYPE_SRATIONAL
                For n = 0 To Count - 1
                    WriteToBuffer_UINT32(Value, offset, ByteOrder, buffer)
                Next

            Case TIFFFieldType.TYPE_SSHORT
                For n = 0 To Count - 1
                    WriteToBuffer_INT16(Value, offset, ByteOrder, buffer)
                Next

            Case TIFFFieldType.TYPE_UNDEFINED
                For n = 0 To Count - 1
                    buffer(offset + n) = 0
                Next
                offset += Count
        End Select
    End Sub
End Class
