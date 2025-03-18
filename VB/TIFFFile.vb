Imports System.IO

Friend Class TIFFFile
    Public Header(7) As Byte
    Public Entries As List(Of TIFFIFDEntry)
    Private _ByteOrder As ByteOrder
    Private _OffsetFirstIFD As UInt32
    Private _PhotometricInterpretation As PhotometricInterpretation
    Private _Width As UInt32
    Private _Height As UInt32
    Private _Compression As Compression
    Private _BitsPerSample As UInt32
    Private _Size As UInt32
    Private _ValuesLongerThan32Bits
    Private _Folder As String
    Private _Name As String
    Private _FullName As String
    Private _Bytes() As Byte
    Private _ImageSize As Integer
    Private _ImageOffset As Integer

    Property Folder As String
        Set(value As String)
            _Folder = value
            FullName = value & "\" & Name
        End Set
        Get
            Return _Folder
        End Get
    End Property
    Property Name As String
        Set(value As String)
            _Name = value
            FullName = _Folder & "\" & value
        End Set
        Get
            Return _Name
        End Get
    End Property
    Property FullName As String
        Set(value As String)
            _FullName = value
        End Set
        Get
            Return _FullName
        End Get
    End Property
    Property ImageSize As UInt32
        Set(value As UInt32)
            _ImageSize = value
        End Set
        Get
            Return _ImageSize
        End Get
    End Property
    Property Compression As Compression
        Set(value As Compression)
            _Compression = value
        End Set
        Get
            Return _Compression
        End Get
    End Property
    Property Width As UInt32
        Set(value As UInt32)
            _Width = value
        End Set
        Get
            Return _Width
        End Get
    End Property
    Property Height As UInt32
        Set(value As UInt32)
            _Height = value
        End Set
        Get
            Return _Height
        End Get
    End Property
    Property BitsPerSample As UInt32
        Set(value As UInt32)
            _BitsPerSample = value
        End Set
        Get
            Return _BitsPerSample
        End Get
    End Property
    Property PhotometricInterpretation As PhotometricInterpretation
        Set(value As PhotometricInterpretation)
            _PhotometricInterpretation = value
        End Set
        Get
            Return _PhotometricInterpretation
        End Get
    End Property
    Property ByteOrder As ByteOrder
        Set(value As ByteOrder)
            _ByteOrder = value
            Select Case _ByteOrder
                Case ByteOrder.BitEndian
                    Header(0) = &H4D
                    Header(1) = &H4D
                    Header(2) = &H0
                    Header(3) = 42


                Case ByteOrder.LittleEndian
                    Header(0) = &H49
                    Header(1) = &H49
                    Header(2) = 42
                    Header(3) = 0
            End Select
        End Set
        Get
            Return _ByteOrder
        End Get
    End Property
    Property OffsetFirstIFD As UInt32
        Set(value As UInt32)
            _OffsetFirstIFD = value
            Select Case _ByteOrder
                Case ByteOrder.BitEndian 'MSB first
                    Header(4) = (OffsetFirstIFD And &HFF000000) >> 24
                    Header(5) = (OffsetFirstIFD And &HFF0000) >> 16
                    Header(6) = (OffsetFirstIFD And &HFF00) >> 8
                    Header(7) = (OffsetFirstIFD And &HFF)


                Case ByteOrder.LittleEndian 'LSB first
                    Header(4) = (OffsetFirstIFD And &HFF)
                    Header(5) = (OffsetFirstIFD And &HFF00) >> 8
                    Header(6) = (OffsetFirstIFD And &HFF0000) >> 16
                    Header(7) = (OffsetFirstIFD And &HFF000000) >> 24
            End Select
        End Set
        Get
            Return _OffsetFirstIFD
        End Get
    End Property
    Sub New()
        ByteOrder = ByteOrder.BitEndian
        OffsetFirstIFD = 8
        Entries = New List(Of TIFFIFDEntry)
    End Sub
    Sub AddEntry(newEntry As TIFFIFDEntry)
        newEntry.ByteOrder = ByteOrder
        Entries.Add(newEntry)
    End Sub
    Public Sub Update()
        Entries.Clear()

        Dim numBits As Integer

        Dim PhotometricInterpretationEntry As New TIFFIFDEntry
        PhotometricInterpretationEntry.Tag = 262
        PhotometricInterpretationEntry.Type = TIFFFieldType.TYPE_SHORT
        PhotometricInterpretationEntry.Value = PhotometricInterpretation

        Dim ImageWidthEntry As New TIFFIFDEntry
        ImageWidthEntry.Tag = 256
        numBits = Math.Ceiling(Math.Log(Width) / Math.Log(2))
        If numBits <= 16 Then ImageWidthEntry.Type = TIFFFieldType.TYPE_SHORT Else ImageWidthEntry.Type = TIFFFieldType.TYPE_LONG
        ImageWidthEntry.Value = Width

        Dim ImageHeightEntry As New TIFFIFDEntry
        ImageHeightEntry.Tag = 257
        numBits = Math.Ceiling(Math.Log(Height) / Math.Log(2))
        If numBits <= 16 Then ImageHeightEntry.Type = TIFFFieldType.TYPE_SHORT Else ImageHeightEntry.Type = TIFFFieldType.TYPE_LONG
        ImageHeightEntry.Value = Height

        Dim RowsPerStripEntry As New TIFFIFDEntry
        RowsPerStripEntry.Tag = 278
        numBits = Math.Ceiling(Math.Log(Height) / Math.Log(2))
        If numBits <= 16 Then RowsPerStripEntry.Type = TIFFFieldType.TYPE_SHORT Else RowsPerStripEntry.Type = TIFFFieldType.TYPE_LONG
        RowsPerStripEntry.Value = Height

        Dim StripByteCountsEntry As New TIFFIFDEntry
        StripByteCountsEntry.Tag = 279
        numBits = Math.Ceiling(Math.Log(ImageSize) / Math.Log(2))
        If numBits <= 16 Then StripByteCountsEntry.Type = TIFFFieldType.TYPE_SHORT Else StripByteCountsEntry.Type = TIFFFieldType.TYPE_LONG
        StripByteCountsEntry.Value = ImageSize

        Dim StripOffsetsEntry As New TIFFIFDEntry
        StripOffsetsEntry.Tag = 273
        'At this moment we do not know the position of the image, since it depends on the number of entries
        StripOffsetsEntry.Value = 0


        Dim CompressionEntry As New TIFFIFDEntry
        CompressionEntry.Tag = 259
        CompressionEntry.Type = TIFFFieldType.TYPE_SHORT
        CompressionEntry.Value = Compression


        AddEntry(PhotometricInterpretationEntry)
        AddEntry(ImageWidthEntry)
        AddEntry(ImageHeightEntry)
        AddEntry(CompressionEntry)
        AddEntry(RowsPerStripEntry)
        AddEntry(StripByteCountsEntry)
        AddEntry(StripOffsetsEntry)

        Entries.Sort(New IFDSorter)


        Dim HeaderSize As Integer = 8
        Dim IDFSize As Integer = 2 + Entries.Count * 12 + 4  'Count + Entries + NextIFD = 0x00000000
        Dim Offset As Integer = HeaderSize + IDFSize

        For Each entry As TIFFIFDEntry In Entries
            If Not entry.ValueFitsIn32Bits Then
                entry.ValueOffset = Offset
                Offset += entry.ValueSize
            Else
                entry.ValueOffset = entry.Value
            End If
        Next

        _ImageOffset = Offset

        'Update the position of start of the image
        numBits = Math.Ceiling(Math.Log(_ImageOffset) / Math.Log(2))
        If numBits <= 16 Then
            StripOffsetsEntry.Type = TIFFFieldType.TYPE_SHORT
        Else
            StripOffsetsEntry.Type = TIFFFieldType.TYPE_LONG
        End If
        StripOffsetsEntry.Value = _ImageOffset

        Array.Resize(_Bytes, HeaderSize + IDFSize + ImageSize)


    End Sub



    Public Sub ReadImage(fs As FileStream, buffLength As Integer)
        ImageSize = buffLength
        Update()
        Dim offset As Integer = 0


        'Header
        Array.Copy(Header, 0, _Bytes, offset, 8)
        offset = 8

        'IFD Count
        WriteToBuffer_UINT16(Entries.Count, offset, ByteOrder, _Bytes)

        'IFD Entries
        For Each entry As TIFFIFDEntry In Entries
            entry.Update()
            Dim entrySize As Integer = entry.ValueSize

            'copy tag, type and count
            Array.Copy(entry.Bytes, 0, _Bytes, offset, 8)
            offset += 8

            If entry.ValueFitsIn32Bits Then
                entry.CopyValue(_Bytes, offset) 'Offset is incremented by the actual length of the type
                offset += (4 - entrySize)
            Else
                entry.CopyValueOffset(_Bytes, offset)
            End If
        Next
        'Next IFD = 0x00000000
        WriteToBuffer_UINT32(0, offset, ByteOrder, _Bytes)

        'All Values Longer than 4 bytes
        For Each entry As TIFFIFDEntry In Entries
            If Not entry.ValueFitsIn32Bits Then
                entry.CopyValue(_Bytes, offset)
            End If
        Next

        'Image
        fs.Read(_Bytes, _ImageOffset, _ImageSize)

    End Sub

    Public Sub Save(FileFolder As String, FileName As String)

        Name = FileName
        Folder = FileFolder

        IO.File.WriteAllBytes(FullName, _Bytes)
    End Sub


End Class

Friend Class IFDSorter
    Implements IComparer(Of TIFFIFDEntry)

    Public Function Compare(x As TIFFIFDEntry, y As TIFFIFDEntry) As Integer Implements IComparer(Of TIFFIFDEntry).Compare

        If x.Tag = y.Tag Then
            Return 0
        ElseIf x.Tag > y.Tag Then
            Return 1
        Else
            Return -1
        End If
    End Function
End Class
