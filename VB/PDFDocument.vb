Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports System.Text.RegularExpressions

Friend Class PDFDocument
    Public Name As String
    Public NameNoExt As String
    Public OutputDirectory As String

    Private Lines() As String
    Private Version As String
    Private Trailer As PDFTrailer
    Public Body As PDFBody
    Private CrossRefTables As New List(Of PDFCrossReferenceTable)
    Private regex As Regex
    Private NumLines As Integer
    Private LinearizationParameterDictionary As PDFObject
    Private PDFStream As FileStream
    Private FileInfo As FileInfo
    Private Size As Integer
    Private Const BUFF_SIZE As Integer = 1024
    Public IsLinearized As Boolean
    Private Catalog As New PDFObject
    Private ASCII_PERCENTAGE As Integer = Asc("%")
    Private CCITTFaxDecodeFilter As CCITTFaxDecodeFilter = Nothing
    Public Images As New List(Of String)
    Public CommandParser As New PDFCommandParser
    Private Enum SearchDirection
        Forward = 0
        Backward
    End Enum
    Private Function SearchForEOF(buffer() As Byte, size As Integer) As Integer
        Dim res As Integer = -1
        Dim found As Boolean = False
        Dim index As Integer = size - 5

        Dim charVal1 As Integer = Asc("%")
        Dim charVal2 As Integer = Asc("E")
        Dim charVal3 As Integer = Asc("O")
        Dim charVal4 As Integer = Asc("F")


        While Not found And index > -1
            found = (buffer(index) = charVal1) And (buffer(index + 1) = charVal1) And (buffer(index + 2) = charVal2) And (buffer(index + 3) = charVal3) And (buffer(index + 4) = charVal4)
            index -= 1
        End While
        If found Then
            res = index + 1
        End If

        Return res
    End Function
    Private Function SearchForOBJ(buffer() As Byte, size As Integer, Optional startIndex As Integer = 0) As Integer
        Dim res As Integer = -1
        Dim found As Boolean = False
        Dim index As Integer = startIndex

        Dim charVal1 As Integer = Asc("o")
        Dim charVal2 As Integer = Asc("b")
        Dim charVal3 As Integer = Asc("j")


        While Not found And index < size - 3
            found = (buffer(index) = charVal1) And (buffer(index + 1) = charVal2) And (buffer(index + 2) = charVal3)
            index += 1
        End While
        If found Then
            res = index - 1
        End If

        Return res
    End Function
    Private Function SearchForENDOBJ(buffer() As Byte, size As Integer, Optional startIndex As Integer = 0) As Integer
        Dim res As Integer = -1
        Dim found As Boolean = False
        Dim index As Integer = startIndex

        Dim charVal1 As Integer = Asc("e")
        Dim charVal2 As Integer = Asc("n")
        Dim charVal3 As Integer = Asc("d")
        Dim charVal4 As Integer = Asc("o")
        Dim charVal5 As Integer = Asc("b")
        Dim charVal6 As Integer = Asc("j")

        While Not found And index < size - 6
            found = (buffer(index) = charVal1) And (buffer(index + 1) = charVal2) And (buffer(index + 2) = charVal3) And
                    (buffer(index + 3) = charVal4) And (buffer(index + 4) = charVal5) And (buffer(index + 5) = charVal6)
            index += 1
        End While
        If found Then
            res = index - 1
        End If

        Return res
    End Function
    Private Function IsEOLMarker(buffer() As Byte, startIndex As Integer) As Boolean

    End Function
    Private Sub MoveToPreviousEOL(buffer() As Byte, ByRef currentPos As Integer)
        While (buffer(currentPos) <> 13)
            currentPos -= 1
        End While
    End Sub
    Private Function SearchPattern(buffer() As Byte, Pattern As String, ByRef currentPos As Integer, direction As SearchDirection) As Boolean
        Dim patternAscii As New List(Of Integer)
        Dim L As Integer = Len(Pattern)

        For n = 1 To L
            patternAscii.Add(Asc(Mid(Pattern, n, 1)))
        Next

        Dim found As Boolean = False
        If direction = SearchDirection.Backward Then
            currentPos -= L
        End If

        While Not found And currentPos > -1
            found = True
            For n = 0 To L - 1
                found = found And (buffer(currentPos + n) = patternAscii(n))
            Next
            If direction = SearchDirection.Backward Then currentPos -= 1 Else currentPos += 1
        End While

        'move back one position to let 'currentPos' at the start of the pattern
        If direction = SearchDirection.Backward Then currentPos += 1 Else currentPos -= 1
        Return found
    End Function
    Private Function SearchPattern(Pattern As String, SearchDirection As SearchDirection) As Boolean
        Dim patternAscii As New List(Of Integer)
        Dim L As Long = Len(Pattern)
        Dim currentByte As Byte

        For n = 1 To L
            patternAscii.Add(Asc(Mid(Pattern, n, 1)))
        Next

        Dim found As Boolean = False
        If SearchDirection = SearchDirection.Backward Then
            PDFStream.Position -= L

            While Not found And PDFStream.Position > -1
                found = True
                For n = 0 To L - 1
                    currentByte = PDFStream.ReadByte
                    found = found And (currentByte = patternAscii(n))
                Next
                PDFStream.Position -= L + 1
            End While

        Else
            While Not found And PDFStream.Position < Size
                found = True
                For n = 0 To L - 1
                    currentByte = PDFStream.ReadByte
                    found = found And (currentByte = patternAscii(n))
                Next
                PDFStream.Position -= L - 1
            End While
            If found Then PDFStream.Position -= 1   'align with the first character
        End If
        Return found

    End Function
    Private Function ReadDictionary(buffer() As Byte, startPos As Integer, buffersize As Integer) As String
        Dim found As Boolean = False
        Dim pos As Integer = startPos
        While (Not found And pos < buffersize)
            found = (buffer(pos) = Asc("<")) And (buffer(pos + 1) = Asc("<"))
            pos += 1
        End While
        If found Then
            Dim markerCount As Integer = 1
            Dim startdict As Integer = pos - 1
            Dim enddict As Integer
            found = False
            While (Not found And pos < buffersize)
                If (buffer(pos) = Asc("<")) And (buffer(pos + 1) = Asc("<")) Then markerCount += 1
                If (buffer(pos) = Asc(">")) And (buffer(pos + 1) = Asc(">")) Then markerCount -= 1
                found = (markerCount = 0)
                pos += 1
            End While
            If found Then
                enddict = pos + 1
                Dim Str As String = System.Text.Encoding.ASCII.GetString(buffer, startdict, enddict - startdict)
                Return Str
            Else
                Return ""
            End If

        Else
            Return ""
        End If

    End Function
    Private Function ReadDictionary(startPos As Integer) As String
        Dim found As Boolean = False
        PDFStream.Seek(startPos, SeekOrigin.Begin)
        Dim byte1 As Byte
        Dim byte2 As Byte
        While (Not found And PDFStream.Position < Size)
            byte1 = PDFStream.ReadByte
            byte2 = PDFStream.ReadByte
            found = (byte1 = Asc("<")) And (byte2 = Asc("<"))
            PDFStream.Position -= 1
        End While
        If found Then
            Dim markerCount As Integer = 1
            Dim startdict As Integer = PDFStream.Position - 1  'Position the stream in the first '<' symbol
            Dim enddict As Integer
            found = False
            While (Not found And PDFStream.Position < Size)
                byte1 = PDFStream.ReadByte
                byte2 = PDFStream.ReadByte
                If (byte1 = Asc("<")) And (byte2 = Asc("<")) Then
                    markerCount += 1
                ElseIf (byte1 = Asc(">")) And (byte2 = Asc(">")) Then
                    markerCount -= 1
                Else
                    PDFStream.Position -= 1     'if no pattern is found, step one back
                End If
                found = (markerCount = 0)
            End While
            If found Then

                enddict = PDFStream.Position - 1
                PDFStream.Position = startdict
                Dim length As Integer = enddict - startdict + 1
                Dim Buffer(length - 1) As Byte
                PDFStream.Read(Buffer, 0, length)
                Dim Str As String = System.Text.Encoding.ASCII.GetString(Buffer, 0, length)
                Return Str
            Else
                Return ""
            End If

        Else
            Return ""
        End If

    End Function
    Private Sub ReadStream(startPos As Integer, ByRef DictObj As PDFObject, ByRef StreamStartPos As Integer, ByRef StreamEndPos As Integer)
        Dim found As Boolean = False
        PDFStream.Seek(startPos, SeekOrigin.Begin)
        Dim byte1 As Byte
        Dim byte2 As Byte
        While (Not found And PDFStream.Position < Size)
            byte1 = PDFStream.ReadByte
            byte2 = PDFStream.ReadByte
            found = (byte1 = Asc("<")) And (byte2 = Asc("<"))
            PDFStream.Position -= 1
        End While
        If found Then
            Dim markerCount As Integer = 1
            Dim startdict As Integer = PDFStream.Position - 1
            Dim enddict As Integer
            found = False
            While (Not found And PDFStream.Position < Size)
                byte1 = PDFStream.ReadByte
                byte2 = PDFStream.ReadByte
                If (byte1 = Asc("<")) And (byte2 = Asc("<")) Then markerCount += 1
                If (byte1 = Asc(">")) And (byte2 = Asc(">")) Then markerCount -= 1
                found = (markerCount = 0)
                PDFStream.Position -= 1
            End While
            If found Then
                enddict = PDFStream.Position + 1
                PDFStream.Position = startdict
                Dim Buffer(enddict - startdict) As Byte
                PDFStream.Read(Buffer, 0, enddict - startdict)
                Dim Str As String = System.Text.Encoding.ASCII.GetString(Buffer, 0, enddict - startdict)
                DictObj = New PDFObject
                DictObj.Value = Str

                'Move to the beginning of the stream
                SearchPattern(PDF_STREAM, SearchDirection.Forward)
                PDFStream.Position += Len(PDF_STREAM)
                'stream must be followed either by a LF or a CR LF --> NO CR alone!
                byte1 = PDFStream.ReadByte
                If byte1 = 10 Then
                    StreamStartPos = PDFStream.Position
                ElseIf byte1 = 13 Then
                    byte2 = PDFStream.ReadByte
                    If byte2 = 10 Then
                        StreamStartPos = PDFStream.Position
                    Else
                        MsgBox("ERROR!!! stream not followed by either LF or CR LF")
                    End If
                End If
                SearchPattern(PDF_ENDSTREAM, SearchDirection.Forward)
                StreamEndPos = PDFStream.Position
            Else
                DictObj = Nothing
                StreamStartPos = -1
                StreamEndPos = -1
            End If

        Else
            DictObj = Nothing
            StreamStartPos = -1
            StreamEndPos = -1
        End If

    End Sub
    Private Function ReadReferenceTable(ByRef Trailer As PDFTrailer) As PDFCrossReferenceTable
        Dim res As PDFCrossReferenceTable = Nothing

        Dim byte1 As Byte = PDFStream.ReadByte
        Dim byte2 As Byte = PDFStream.ReadByte
        Dim byte3 As Byte = PDFStream.ReadByte
        Dim byte4 As Byte = PDFStream.ReadByte

        Dim isAligned As Boolean = (byte1 = Asc("x")) And (byte2 = Asc("r")) And (byte3 = Asc("e")) And (byte4 = Asc("f"))
        'Dim regexSubsection As New Regex("\d+\s\d+(?!(\s(n|f)))")
        Dim regexSubsectionEntry As New Regex("\d{10}\s\d{5}\s(n|f)")

        If isAligned Then

            Dim line As String
            res = New PDFCrossReferenceTable
            Dim sectionSize As Integer
            Dim sectionStart As Integer = PDFStream.Position
            Dim trailerStart As Integer
            If SearchPattern(PDF_TRAILER, SearchDirection.Forward) Then
                trailerStart = PDFStream.Position

                Trailer = New PDFTrailer
                Trailer.Dictionary.Value = ReadDictionary(trailerStart)

                sectionSize = trailerStart - sectionStart
                Dim tmpBuff(sectionSize) As Byte
                PDFStream.Position = sectionStart
                PDFStream.Read(tmpBuff, 0, sectionSize)

                Dim Str As String = System.Text.Encoding.ASCII.GetString(tmpBuff, 0, sectionSize)
                Str = Replace(Str, vbCrLf, vbCr)
                Str = Replace(Str, vbLf, vbCr)
                Dim lines() As String = Str.Split(vbCr)
                Dim listLine As New List(Of String)
                For Each line In lines
                    If line <> "" And Mid(line, 1, 1) <> "%" Then listLine.Add(line)
                Next

                Dim done As Boolean = False
                Dim currentsection As New PDFCrossReferenceSection
                Dim currentsubsection As PDFSubsection
                Dim objInitNum As Integer
                Dim numObjs As Integer
                While Not done
                    line = listLine(0)


                    If regexSubsectionEntry.IsMatch(line) Then
                        Dim crefEntry As New CrossReferenceEntry
                        crefEntry.ObjectNumber = objInitNum
                        Dim s As String = line
                        Dim offsetStr As String = Mid(s, 1, 10)
                        s = Mid(line, 11, Len(s) - 10).Trim
                        Dim genNumStr As String = Mid(s, 1, 5)
                        s = Mid(s, 6, Len(s) - 5).Trim
                        crefEntry.ByteOffset = offsetStr
                        crefEntry.GenerationNumber = genNumStr

                        Dim state As String = Mid(s, 1, 1)
                        If state = "n" Then crefEntry.Type = SubsectionEntryType.InUse Else crefEntry.Type = SubsectionEntryType.Free
                        objInitNum += 1
                        numObjs -= 1
                        listLine.RemoveAt(0)
                        currentsubsection.Entries.Add(crefEntry)
                        If numObjs = 0 Then
                            currentsection.Subsections.Add(currentsubsection)
                        End If
                    Else
                        Dim nums() As String = line.Split(" ")
                        objInitNum = nums(0)
                        numObjs = nums(1)
                        currentsubsection = New PDFSubsection
                        listLine.RemoveAt(0)
                    End If
                    done = (listLine.Count = 0)
                End While
                res.Sections.Add(currentsection)
            End If
        End If
        Return res
    End Function
    Private Sub ReadCRTs(Trailer As PDFTrailer, isFirst As Boolean)

        Dim nextTrailer As PDFTrailer
        Dim PrevOffset As Integer

        If isFirst Then
            PrevOffset = Trailer.startXRef
        Else
            If Trailer.Dictionary.ContainsKey("Prev") Then
                PrevOffset = Trailer.Dictionary.GetDictValue("Prev").Value
            Else
                'no more CRT sections, exit
                Return
            End If

        End If

        PDFStream.Seek(PrevOffset, SeekOrigin.Begin)
        CrossRefTables.Add(ReadReferenceTable(nextTrailer))
        ReadCRTs(nextTrailer, False)


    End Sub
    Private Function GetObjectPosition(indirectRef As PDFObject) As Integer
        Dim res As Integer = -1
        For n = 0 To CrossRefTables.Count - 1
            res = CrossRefTables(n).GetObjectPos(indirectRef)
            If res > -1 Then
                Return res
            End If
        Next
        Return res
    End Function

    Private Sub ExtractXObjectImage(StreamDict As PDFObject, StreamStartPos As Integer, StreamEndPos As Integer, Optional IsMask As Boolean = False)


        Dim typeStr As String
        Dim SubType As String
        Dim widthStr As String
        Dim HeightStr As String

        'Validate the type (OPTIONAL)
        If StreamDict.ContainsKey(PDF_TYPE) Then
            typeStr = StreamDict.GetDictValue(PDF_TYPE).Value
            If typeStr <> PDF_XOBJECT Then
                MsgBox("ERROR: type for Image must be 'XObject'")
                Return
            End If
        End If

        If StreamDict.ContainsKey(PDF_SUBTYPE) Then
            SubType = StreamDict.GetDictValue(PDF_SUBTYPE).Value
            If SubType <> PDF_IMAGE Then
                MsgBox("ERROR: subtype for Image must be 'Image'")
                Return
            End If
        End If

        If StreamDict.ContainsKey(PDF_WIDTH) Then
            widthStr = StreamDict.GetDictValue(PDF_WIDTH).Value
        Else
            MsgBox("ERROR: image XObject must include the Width entry")
            Return
        End If

        If StreamDict.ContainsKey(PDF_HEIGHT) Then
            HeightStr = StreamDict.GetDictValue(PDF_HEIGHT).Value
        Else
            MsgBox("ERROR: image XObject must include the Height entry")
            Return
        End If

        Dim BitsPerComponent As String = StreamDict.GetDictValue(PDF_BITS_PER_COMPONENT).Value


        'Dim ColorSpace As String = StreamDict.GetDictValue(PDF_COLORSPACE).Value
        Dim filterObj As PDFObject = StreamDict.GetDictValue(PDF_FILTER)



        Dim filterName As String
        If filterObj.Type = PDFObjectType.PDF_Name Then
            filterName = filterObj.Value

        ElseIf filterObj.Type = PDFObjectType.PDF_Array Then
            filterName = filterObj.ArrayElements(0).Value

        End If




        If filterObj.Type = PDFObjectType.PDF_Name Then
            filterName = filterObj.Value
        ElseIf filterObj.Type = PDFObjectType.PDF_Array Then
            Dim a = 1
        End If


        Select Case filterName
            Case PDF_ASCII_HEX_DECODE, PDF_ASCII_HEX_DECODE_ABREV
            Case PDF_CCITT_85_DECODE, PDF_CCITT_85_DECODE_ABREV
            Case PDF_LZW_DECODE, PDF_LZW_DECODE_ABREV
            Case PDF_FLATE_DECODE, PDF_FLATE_DECODE_ABREV
            Case PDF_RUN_LENGTH_DECODE, PDF_RUN_LENGTH_DECODE_ABREV
            Case PDF_CCITT_FAX_DECODE, PDF_CCITT_FAX_DECODE_ABREV
                Dim L As Integer = StreamEndPos - StreamStartPos + 1
                Dim buffer(L - 1) As Byte
                PDFStream.Position = StreamStartPos

                If StreamDict.ContainsKey(PDF_DECODEPARAMS) Then
                    Dim DecodeParamsObj As PDFObject = StreamDict.GetDictValue(PDF_DECODEPARAMS)

                    'DecodeParamsObj is a dictionary or an array of dictionaries

                    Dim K As Integer = DecodeParamsObj.GetDictValue("K").Value

                    If StreamDict.ContainsKey("ImageMask") Then
                        Dim isMaskStr As String = StreamDict.GetDictValue("ImageMask").Value



                    End If




                    'CCITTFaxDecodeFilter.K = DecodeParamsObj.GetDictValue("K").Value
                    'CCITTFaxDecodeFilter.Columns = DecodeParamsObj.GetDictValue("Width").Value
                    'CCITTFaxDecodeFilter.Rows = DecodeParamsObj.GetDictValue("Height").Value
                    Dim tiffF As New TIFFFile
                    tiffF.ByteOrder = ByteOrder.BitEndian

                    If K < 0 Then
                        ' K < 0 --- Pure two-dimensional encoding (Group 4)
                        tiffF.Compression = Compression.T6Encoding

                    ElseIf K > 0 Then
                        ' K > 0 --- Mixed one- and two-dimensional encoding (Group 3, 2-D)
                        tiffF.Compression = Compression.T4Encoding

                    Else
                        ' K = 0 --- Pure one-dimensional encoding (Group 3, 1-D)
                        tiffF.Compression = Compression.T4Encoding
                    End If

                    tiffF.Height = HeightStr
                    tiffF.Width = widthStr
                    tiffF.ReadImage(PDFStream, L)
                    Dim filePath As String

                    If IsMask Then
                        filePath = NameNoExt & Images.Count & "_mask.Tiff"
                    Else
                        filePath = NameNoExt & Images.Count & ".Tiff"
                    End If


                    tiffF.Save(OutputDirectory, filePath)
                    Images.Add(filePath)
                End If

            Case PDF_DCT_DECODE, PDF_DCT_DECODE_ABREV

                Dim filePath As String
                If IsMask Then
                    filePath = NameNoExt & Images.Count & "_mask.jpg"
                Else
                    filePath = NameNoExt & Images.Count & ".jpg"
                End If


                Dim L As Integer = StreamEndPos - StreamStartPos + 1
                Dim buffer(L - 1) As Byte
                PDFStream.Position = StreamStartPos
                PDFStream.Read(buffer, 0, L)

                If StreamDict.ContainsKey("Mask") Then
                    Dim maskObj As PDFObject = StreamDict.GetDictValue("Mask")

                    'reference to a stream dict containing the image
                    Dim maskDictObj As PDFObject
                    Dim maskStreamStartPos As Long
                    Dim maskStreamEndPos As Long
                    ReadStream(GetObjectPosition(maskObj), maskDictObj, maskStreamStartPos, maskStreamEndPos)
                    ExtractXObjectImage(maskDictObj, maskStreamStartPos, maskStreamEndPos)
                    Dim a As Integer = 0
                End If

                IO.File.WriteAllBytes(OutputDirectory & "\" & filePath, buffer)
                Images.Add(filePath)

        End Select



    End Sub
    Private Function ParseResourcesDict(resObj As PDFObject) As PDFObject
        Dim resDictObj As PDFObject
        'The standard says this should be a dictionary, but some pdfs place an indirect reference!
        If resObj.Type = PDFObjectType.PDF_Dictionary Then
            resDictObj = resObj

        ElseIf resObj.Type = PDFObjectType.PDF_IndirectReference Then
            Dim resDict As String = ReadDictionary(GetObjectPosition(resObj))
            resDictObj = New PDFObject
            resDictObj.Value = resDict
        End If

        'Parse Procedure Set Names
        If resDictObj.ContainsKey("ProcSet") Then
            Dim ProcSetObj As PDFObject = resDictObj.GetDictValue("ProcSet")
            If ProcSetObj.Type = PDFObjectType.PDF_IndirectReference Then

            End If
        End If



        'Parse XObject names
        If resDictObj.ContainsKey(PDF_XOBJECT) Then
            Dim ref As New PDFObject
            ref.Value = resDictObj.GetDictValue(PDF_XOBJECT).Value

            'The XObject is a dictionary with the images
            Dim images As List(Of PDFObject) = ref.GetAllDictValues


            For Each image As PDFObject In images
                Dim DictObj As PDFObject
                Dim StreamStartPos As Integer = -1
                Dim StreamEndPos As Integer = -1

                ReadStream(GetObjectPosition(image), DictObj, StreamStartPos, StreamEndPos)
                ExtractXObjectImage(DictObj, StreamStartPos, StreamEndPos)
            Next
        End If

        Return resDictObj
    End Function
    Private Function ReadPageContents(Pos As Integer) As PDFObject
        Dim pagecontentsObj As New PDFObject
        PDFStream.Position = Pos
        If SearchPattern(PDF_OBJ, SearchDirection.Forward) Then
            Dim p1 As Integer = PDFStream.Position

            SearchPattern(PDF_ENDOBJ, SearchDirection.Forward)
            Dim p2 As Integer = PDFStream.Position

            Dim cnt As Integer = p2 - p1 - 3
            Dim arr(cnt) As Byte
            PDFStream.Position = p1 + 3
            PDFStream.Read(arr, 0, cnt)

            Dim Str As String = System.Text.Encoding.ASCII.GetString(arr, 0, cnt)
            Str = Replace(Str, vbCrLf, "")
            Str = Replace(Str, vbCr, "")
            Str = Replace(Str, vbLf, "")

            pagecontentsObj.Value = Str
        End If




        Return pagecontentsObj
    End Function
    Private Sub ReadPages(NodePosition As Integer, isRoot As Boolean)
        Dim nodeDict As String = ReadDictionary(NodePosition)
        Dim node As New PDFObject
        node.Value = nodeDict

        Dim NodeType As String = node.GetDictValue(PDF_TYPE).Value
        Select Case NodeType
            Case PDF_PAGES
                'Read Kids Array
                Dim kids As String = node.GetDictValue(PDF_KIDS).Value
                Dim mCol As MatchCollection
                mCol = IndirectReferenceRegex.Matches(kids)
                For Each m As Match In mCol
                    Dim indRef As New PDFObject
                    indRef.Value = m.Value
                    Dim objPos As Integer = GetObjectPosition(indRef)
                    ReadPages(objPos, False)
                Next

            Case PDF_PAGE
                Dim Contents As String = node.GetDictValue(PDF_CONTENTS).Value
                Dim indRef As New PDFObject
                indRef.Value = Contents
                Dim objPos As Integer

                If indRef.Type = PDFObjectType.PDF_Array Then
                    objPos = GetObjectPosition(indRef.ArrayElements(0))

                ElseIf indRef.Type = PDFObjectType.PDF_IndirectReference Then
                    objPos = GetObjectPosition(indRef)
                Else
                    MsgBox("Check!!!")
                End If

                Dim Resources As String = node.GetDictValue(PDF_RESOURCES).Value
                Dim objDict As New PDFObject
                objDict.Value = Resources
                Dim ResourcesDict As PDFObject = ParseResourcesDict(objDict)

                CommandParser.Clear()
                CommandParser.SetPageResourceDict(ResourcesDict)
                Dim contentsObj As PDFObject = ReadPageContents(objPos)

                If contentsObj.Type = PDFObjectType.PDF_Array Then
                    Dim reft As PDFObject = contentsObj.ArrayElements(0)
                    If reft.Type = PDFObjectType.PDF_IndirectReference Then
                        Dim contentobjPos As Integer = GetObjectPosition(reft)

                        Dim contentStreamDict As PDFObject
                        Dim StreamStartPos As Integer
                        Dim StreamEndPos As Integer
                        ReadStream(contentobjPos, contentStreamDict, StreamStartPos, StreamEndPos)

                        Dim lengthStr As PDFObject
                        Dim filter As PDFObject
                        If contentStreamDict.ContainsKey(PDF_FILTER) Then
                            filter = New PDFObject
                            filter.Value = contentStreamDict.GetDictValue(PDF_FILTER).Value
                        End If

                        If contentStreamDict.ContainsKey(PDF_LENGTH) Then
                            lengthStr = New PDFObject
                            lengthStr.Value = contentStreamDict.GetDictValue(PDF_LENGTH).Value
                        End If

                        Dim L As Integer = StreamEndPos - StreamStartPos + 1 - 2
                        Dim buffer(L - 1) As Byte
                        PDFStream.Position = StreamStartPos + 2
                        PDFStream.Read(buffer, 0, L)


                        If filter IsNot Nothing Then
                            Dim Stream As New MemoryStream()
                            Dim compressStream As New MemoryStream(buffer)

                            Dim decompressor As New DeflateStream(compressStream, CompressionMode.Decompress)
                            decompressor.CopyTo(Stream)

                            Dim str As String = Encoding.Default.GetString(Stream.ToArray())
                            CommandParser.Parse(str)
                        End If
                    End If
                Else

                End If



















        End Select

    End Sub
    Public Sub Load(FilePath As String)

        Name = IO.Path.GetFileName(FilePath)
        NameNoExt = IO.Path.GetFileNameWithoutExtension(FilePath)
        Images.Clear()

        Lines = IO.File.ReadAllLines(FilePath)
        Trailer = New PDFTrailer
        NumLines = Lines.Length


        Dim offset As Integer = 0
        FileInfo = New FileInfo(FilePath)
        Size = FileInfo.Length

        PDFStream = New FileStream(FilePath, FileMode.Open)
        Dim tmpBuffer(BUFF_SIZE - 1) As Byte


        'Read Trailer first. Load 1024 bytes from the end
        PDFStream.Seek(-BUFF_SIZE, SeekOrigin.End)
        PDFStream.Read(tmpBuffer, 0, BUFF_SIZE)
        Dim eofIndex As Integer = SearchForEOF(tmpBuffer, BUFF_SIZE)
        If eofIndex = -1 Then
            MsgBox("ERROR:: Could not read the trailer")
        End If
        Dim currentPos As Integer = eofIndex
        Dim patternOK As Boolean = SearchPattern(tmpBuffer, PDF_START_XREF, currentPos, SearchDirection.Backward)
        Dim str As String = System.Text.Encoding.ASCII.GetString(tmpBuffer, currentPos, eofIndex - currentPos)
        str = Replace(str, PDF_START_XREF, "")
        str = Replace(str, vbCrLf, "")
        str = Replace(str, vbCr, "")
        str = Replace(str, vbLf, "")
        Trailer.startXRef = Integer.Parse(str)
        Dim startxrefIndex As Integer = currentPos
        patternOK = SearchPattern(tmpBuffer, PDF_TRAILER, currentPos, SearchDirection.Backward)
        Dim trailerStartPos As Integer = Size - BUFF_SIZE + currentPos
        Dim trailerDict As String = ReadDictionary(tmpBuffer, currentPos, BUFF_SIZE)
        Trailer.Dictionary.Value = trailerDict
        Trailer.Position = trailerStartPos

        ReadCRTs(Trailer, True)


        'Check Trailer dict content
        Dim TotalNumCRTEntries As Integer = Trailer.Dictionary.GetDictValue("Size").Value
        Dim fileHasMoreThanOneCRT As Boolean = Trailer.Dictionary.ContainsKey("Prev")
        If Trailer.Dictionary.ContainsKey(PDF_ROOT) Then
            Dim indRef As PDFObject = Trailer.Dictionary.GetDictValue(PDF_ROOT)
            Dim catalogPos As Integer = GetObjectPosition(indRef)
            If catalogPos > -1 Then
                Dim catalogDict As String = ReadDictionary(catalogPos)
                Catalog.Value = catalogDict
                'Validate Catalog
                Dim CatalogOK As Boolean = Catalog.GetDictValue("Type").Value = "Catalog"
                Dim pageTreeNode As New PDFObject
                pageTreeNode.Value = Catalog.GetDictValue("Pages").Value
                Dim pageTreeNodePos As Integer = GetObjectPosition(pageTreeNode)
                ReadPages(pageTreeNodePos, True)
            End If
        End If
        Dim fileHasInfoDictionary As Boolean = Trailer.Dictionary.ContainsKey("Info")
        Dim fileIsEncryptedAs As Boolean = Trailer.Dictionary.ContainsKey("Encrypt")


        ''Check if file is linearized
        'PDFStream.Seek(0, SeekOrigin.Begin)
        'PDFStream.Read(tmpBuffer, 0, BUFF_SIZE)
        'Dim objPos As Integer = SearchForOBJ(tmpBuffer, BUFF_SIZE)
        'Dim endObjPos As Integer = SearchForENDOBJ(tmpBuffer, BUFF_SIZE, objPos)
        'str = System.Text.Encoding.ASCII.GetString(tmpBuffer, objPos, endObjPos - objPos - 2)
        'IsLinearized = InStr(str, "/Linearized") > 0
        'If IsLinearized Then
        '    LinearizationParameterDictionary = New PDFObject
        '    LinearizationParameterDictionary.Value = ReadDictionary(tmpBuffer, objPos, BUFF_SIZE)

        '    'Sanity check: read L
        '    Dim L As Integer = LinearizationParameterDictionary.GetDictValue("L").Value
        '    If L <> Size Then
        '        MsgBox("ERROR:: L and Size are not equal!!!!")
        '        Return
        '    End If
        '    Dim N As Integer = LinearizationParameterDictionary.GetDictValue("N").Value
        'End If
        PDFStream.Close()

    End Sub
    Private Function ReadNextObject(ByRef offset As Integer) As PDFObject
        Dim res As PDFObject = Nothing
        Dim objStartIndex As Integer
        Dim objStopIndex As Integer

        regex = New Regex("\d\s\d\s" & PDF_OBJ, RegexOptions.Singleline)
        Dim Found As Boolean = False
        While Not Found And offset < NumLines
            Found = regex.IsMatch(Lines(offset))
            offset += 1
        End While

        If Found Then
            objStartIndex = offset

            Dim nums() As String = Lines(offset - 1).Split(" ")
            Dim objNumber As Integer = nums(0)
            Dim objGenNumber As Integer = nums(1)


            Found = False
            regex = New Regex(PDF_ENDOBJ, RegexOptions.Singleline)
            While Not Found And offset < NumLines
                Found = regex.IsMatch(Lines(offset))
                offset += 1
            End While

            If Found Then
                objStopIndex = offset - 2
                Dim ObjValue As String = ""
                Dim cnt As Integer = 0
                For n = objStartIndex To objStopIndex
                    ObjValue = ObjValue & Lines(n).Trim
                    If cnt < objStopIndex - objStartIndex Then ObjValue = ObjValue & vbCrLf
                    cnt += 1
                Next

                res = New PDFObject
                res.Value = ObjValue
            End If


        End If

        Return res
    End Function
    Private Function CheckFirstPageTrailer(ByRef firstPageTrailer As PDFTrailer) As Boolean
        Dim res As Boolean

        If Not firstPageTrailer.Dictionary.ContainsKey(PDF_SIZE) Then Return False
        If Not firstPageTrailer.Dictionary.ContainsKey(PDF_ROOT) Then Return False





        Return res
    End Function
    Private Function CheckIfLinearized(ByRef offset As Integer) As Boolean
        Dim dictStartIndex As Integer
        Dim dictStopIndex As Integer
        Dim res As Boolean = False

        regex = New Regex("\d\s\d\s" & PDF_OBJ, RegexOptions.Singleline)
        Dim Found As Boolean = False
        While Not Found And offset < NumLines
            Found = regex.IsMatch(Lines(offset))
            offset += 1
        End While
        If Found Then
            dictStartIndex = offset

            Dim nums() As String = Lines(offset - 1).Split(" ")
            Dim objNumber As Integer = nums(0)
            Dim objGenNumber As Integer = nums(1)

            LinearizationParameterDictionary = New PDFObject(PDFObjectType.PDF_Dictionary)
            Found = False
            regex = New Regex(PDF_ENDOBJ, RegexOptions.Singleline)
            While Not Found And offset < NumLines
                Found = regex.IsMatch(Lines(offset))
                offset += 1
            End While

            'If Found Then
            '    dictStopIndex = offset - 2

            '    For n = dictStartIndex To dictStopIndex
            '        Dim dictLine As String = Lines(n).Trim
            '        If dictLine <> "" Then dictLine = Replace(dictLine, "<<", "").Trim
            '        If dictLine <> "" Then dictLine = Replace(dictLine, ">>", "").Trim
            '        If dictLine <> "" Then LinearizationParameterDictionary.AddEntry(dictLine)
            '    Next

            '    'Check that all applicable fields are present
            '    If Not LinearizationParameterDictionary.ContainsKey("/" & PDF_LINEARIZED) Then
            '        LinearizationParameterDictionary = Nothing
            '        Return False
            '    End If
            '    If Not LinearizationParameterDictionary.ContainsKey("/L") Then
            '        MsgBox("ERROR:: 'Length' entry not present in the LinearizationParameterDictionary")
            '        LinearizationParameterDictionary = Nothing
            '        Return False
            '    End If
            '    If Not LinearizationParameterDictionary.ContainsKey("/H") Then
            '        MsgBox("ERROR:: 'H' entry not present in the LinearizationParameterDictionary")
            '        LinearizationParameterDictionary = Nothing
            '        Return False
            '    End If
            '    If Not LinearizationParameterDictionary.ContainsKey("/O") Then
            '        MsgBox("ERROR:: 'O' entry not present in the LinearizationParameterDictionary")
            '        LinearizationParameterDictionary = Nothing
            '        Return False
            '    End If
            '    If Not LinearizationParameterDictionary.ContainsKey("/E") Then
            '        MsgBox("ERROR:: 'E' entry not present in the LinearizationParameterDictionary")
            '        LinearizationParameterDictionary = Nothing
            '        Return False
            '    End If
            '    If Not LinearizationParameterDictionary.ContainsKey("/N") Then
            '        MsgBox("ERROR:: 'N' entry not present in the LinearizationParameterDictionary")
            '        LinearizationParameterDictionary = Nothing
            '        Return False
            '    End If
            '    If Not LinearizationParameterDictionary.ContainsKey("/T") Then
            '        MsgBox("ERROR:: 'T' entry not present in the LinearizationParameterDictionary")
            '        LinearizationParameterDictionary = Nothing
            '        Return False
            '    End If

            'End If
            res = True
        End If
        Return res
    End Function





    Private Function ProcessHeader(ByRef offset As Integer) As Boolean
        Dim line As String = Lines(0).Trim

        If Mid(line, 1, 1) = "%" Then
            line = Mid(line, 2, Len(line) - 1)
            Version = line
            'check next line, probably a comment with binary data
            line = Lines(1)
            If Mid(line, 1, 1) = "%" Then
                offset = 2
            Else
                offset = 1
            End If

            Return True
            'Select Case line
            '    Case PDF_VERSION_1_0
            '        Version = line
            '    Case PDF_VERSION_1_1
            '        Version = line
            '    Case PDF_VERSION_1_2
            '        Version = line
            '    Case PDF_VERSION_1_3
            '        Version = line
            '    Case PDF_VERSION_1_4
            '        Version = line
            '    Case Else
            '        Return False
            'End Select
        Else
            Return False

        End If

        Return True
    End Function




    Private Function ProcessCrossReferenceTable(offset As Integer) As PDFCrossReferenceTable
        Dim found As Boolean = False
        Dim res As New PDFCrossReferenceTable

        While Not found And offset < NumLines
            Dim line As String = Lines(offset)
            found = (InStr(line, PDF_XREF) > 0) And (line <> PDF_START_XREF)

            If found Then
                Dim newSection As New PDFCrossReferenceSection
                offset += 1
                'Start subsections
                regex = New Regex("\d\s\d", RegexOptions.Singleline)
                Dim FoundSubsections As Boolean = regex.IsMatch(Lines(offset))
                While FoundSubsections
                    Dim nums() As String = Lines(offset).Split(" ")
                    Dim objNum As Integer = nums(0)
                    Dim numObjects As Integer = nums(1)
                    Dim newSubsection As New PDFSubsection

                    offset += 1
                    For n = 0 To numObjects - 1

                        Dim subSectionLine As String = Lines(offset + n)
                        Dim objNumber As Integer = objNum + n

                        Dim entry As New CrossReferenceEntry
                        entry.ObjectNumber = objNumber

                        nums = subSectionLine.Split(" ")
                        If nums.Length = 3 Then
                            entry.ByteOffset = nums(0)
                            entry.GenerationNumber = nums(1)
                            If nums(2) = "n" Then
                                entry.Type = SubsectionEntryType.InUse
                            ElseIf nums(2) = "f" Then
                                entry.Type = SubsectionEntryType.Free
                            End If

                        Else
                            MsgBox("ERROR:: Subsection format not recognized")
                            Exit Function
                        End If
                        newSubsection.Entries.Add(entry)
                    Next

                    offset += numObjects
                    FoundSubsections = regex.IsMatch(Lines(offset))
                    newSection.Subsections.Add(newSubsection)
                End While

                res.Sections.Add(newSection)

            End If

            offset += 1
        End While
        Return res
    End Function
    Sub New()
        CCITTFaxDecodeFilter = New CCITTFaxDecodeFilter
    End Sub
    Sub Close()
        PDFStream.Close()
        Images.Clear()

    End Sub
End Class



