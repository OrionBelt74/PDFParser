#include "../inc/PDFDocument.hpp"

#include <cstdint>
#include <vector>
#include <filesystem>
#include <fstream>
#include "../inc/StringUtils.hpp"

PDFDocument::PDFDocument()
{
        //CCITTFaxDecodeFilter = New CCITTFaxDecodeFilter
    Trailer = NULL;
}

PDFDocument::~PDFDocument()
{
    /*PDFStream.Close()
    Images.Clear()*/
}


int PDFDocument::SearchForEOF(uint8_t* Buff, int size)
{
    int res = -1;
    bool found = false;
    int index = size - 5;

    uint8_t charVal1 = '%';
    uint8_t charVal2 = 'E';
    uint8_t charVal3 = 'O';
    uint8_t charVal4 = 'F';

        while ((!found) && (index > -1))
        {
            found = (Buff[index] == charVal1) &&
                    (Buff[index+1] == charVal2) && 
                    (Buff[index + 2] == charVal3) && 
                (Buff[index + 3] == charVal4));
            index--;
        }
        if (found)
            res = index + 1;
       
        return res;
}


int PDFDocument::SearchForOBJ(uint8_t* Buff, int size, int  startIndex)
{
    int res = -1;
    bool found = false;
    int index = startIndex;

    uint8_t charVal1 = 'o';
    uint8_t charVal2 = 'b';
    uint8_t charVal3 = 'j';


    while (!found && (index < size - 3))
    {
        found = (Buff[index] == charVal1) &&
            (Buff[index + 1] == charVal2) &&
            (Buff[index + 2] == charVal3);
        index++;
    }

    if (found)
        res = index - 1;

    return res;
}

int PDFDocument::SearchForENDOBJ(uint8_t* Buff, int size, int startIndex = 0)
{
    int res = -1;
    bool found = false;
    int index = startIndex;

    uint8_t charVal1 = 'e';
    uint8_t charVal2 = 'n';
    uint8_t charVal3 = 'd';
    uint8_t charVal4 = 'o';
    uint8_t charVal5 = 'b';
    uint8_t charVal6 = 'j';

    while (!found && (index < size - 6))
    {
        found = (Buff[index] == charVal1) &&
            (Buff[index + 1] == charVal2) &&
            (Buff[index + 2] == charVal3) &&
            (Buff[index + 3] == charVal4) &&
            (Buff[index + 4] == charVal5) &&
            (Buff[index + 5] == charVal6);

        index++;
    }

    if (found)
        res = index - 1;

    return res;
}


    //Private Function IsEOLMarker(buffer() As Byte, startIndex As Integer) As Boolean

    //End Function
void PDFDocument::MoveToPreviousEOL(uint8_t* Buff, int* currentPos)
{
    int index = *currentPos;
    while (Buff[index] != 13)
        index--;
    *currentPos = index;
}


bool PDFDocument::SearchPattern(uint8_t* Buff, std::string Pattern, int* currentPos, SearchDirection direction)
{
    std::vector<int> patternAscii;
    int L = (int)Pattern.length();

    for (int n = 0; n < L; n++)
    {
        patternAscii.push_back(int((char)Pattern[n]));
    }

    bool found = false;
    if (direction == SearchDirection::BACKWARDS)
        (*currentPos) -= L;


    while (!found && (*currentPos > -1))
    {
        found = true;
        for (int n = 0; n < L; n++)
            found = found && (buffer[currentPos + n] == patternAscii[n]);

        if (direction == SearchDirection::BACKWARDS)
            (*currentPos)--;
        else
            (*currentPos)++;

        //move back one position to let 'currentPos' at the start of the pattern
        if (direction == SearchDirection::BACKWARDS)
            (*currentPos)++;
        else
            (*currentPos)--;
        return found;
    }
}

    bool PDFDocument::SearchPattern(std::string& Pattern, SearchDirection searchDirection)
    {
        std::vector<int> patternAscii;
        int L = (int)Pattern.size();
        uint8_t currentByte;

        for (int n = 0; n < L; n++)
            patternAscii.push_back(int((char)Pattern[n]));
        

        bool found = false;
        if (SearchDirection == SearchDirection::BACKWARDS)
        {
            PDFStream.Position -= L;

            while (!found && PDFStream.Position > -1)
            {
                found = true;
                for (int n = 0; n < L; n++)
                {
                    currentByte = PDFStream.ReadByte;
                    found = found && (currentByte == patternAscii[n]);
                }
                PDFStream.Position -= (L + 1);
            }
        }
        else
        {
            while (!found && PDFStream.Position < Size)
            {
                found = true;
                for (int n = 0; n < L; n++)
                {
                    currentByte = PDFStream.ReadByte;
                    found = found && (currentByte == patternAscii[n]);
                }
                PDFStream.Position -= (L - 1);
            }
            if (found)
                PDFStream.Position -= 1;  //align with the first character

        }
        return found;
}

    std::string PDFDocument::ReadDictionary(uint8_t* buffer, int startPos, int buffersize)
    {
        bool found = false;
        int pos = startPos;
        while (!found && pos < buffersize)
        {
            found = (buffer[pos] == int('<')) && (buffer[pos + 1] = int('<'));
            pos++;
        }


        if (found)
        {
            int markerCount = 1;
            int startdict = pos - 1;
            int enddict;
            found = false;
            while (!found && pos < buffersize)
            {
                if (buffer[pos] = int('<')) && (buffer[pos + 1] == int('<'))
                    markerCount++;
                if (buffer[pos] = int('>')) && (buffer[pos + 1] == int('>'))
                    markerCount--;
                found = (markerCount == 0);
                pos++;
            }

            if (found)
            {
                enddict = pos + 1;
                std::string Str = System.Text.Encoding.ASCII.GetString(buffer, startdict, enddict - startdict)
                    return Str;
            }
            else
                return "";
        }
        else
            return "";
    
    }



    std::string PDFDocument::ReadDictionary(int startPos)
    {
        bool found = false;
        PDFStream.Seek(startPos, SeekOrigin.Begin);
        uint8_t byte1 = 0;
        uint8_t byte2 = 0;
        while (!found && PDFStream.Position < Size)
        {
            byte1 = PDFStream.ReadByte;
            byte2 = PDFStream.ReadByte;
            found = (byte1 == int('<')) && (byte2 == int('<'));
            PDFStream.Position--;
        }
        if (found)
        {
            int markerCount = 1;
            int startdict = PDFStream.Position - 1; //Position the stream in the first ' < ' symbol
            int enddict;
            found = false;
            while (!found && PDFStream.Position < Size)
            {
                byte1 = PDFStream.ReadByte;
                byte2 = PDFStream.ReadByte;
                if ((byte1 == int('<')) && (byte2 == int('<')))
                {
                    markerCount++;
                }
                else if (byte1 == int('>')) && (byte2 == int('>'))
                {
                    markerCount--;
                }
                else
                    PDFStream.Position -= 1;     //if no pattern is found, step one back
            
                found = (markerCount == 0);
            }

            if (found)
            {
                int enddict = PDFStream.Position - 1;
                PDFStream.Position = startdict;
                int length = enddict - startdict + 1;

                uint8_t* Buffer = new uint8_t[length];
                PDFStream.Read(Buffer, 0, length);
                std::string Str = System.Text.Encoding.ASCII.GetString(Buffer, 0, length);
                delete[] Buffer;
                return Str;
            }
            else
                return "";
        }
        else
          return "";
    }



    void PDFDocument::ReadStream(int startPos, PDFObject* DictObj, int* StreamStartPos, int* StreamEndPos)
    {
        bool found = false;
        PDFStream.Seek(startPos, SeekOrigin.Begin);
        uint8_t byte1;
        uint8_t byte2;

        while (!found && PDFStream.Position < Size)
        {
            byte1 = PDFStream.ReadByte;
            byte2 = PDFStream.ReadByte;
            found = (byte1 == int('<')) && (byte2 == int('<'));
            PDFStream.Position--;
        }

        if (found)
        {
            int markerCount = 1;
            int startdict = PDFStream.Position - 1;
            int enddict;
            found = false;

            while (!found && PDFStream.Position < Size)
            {
                byte1 = PDFStream.ReadByte;
                byte2 = PDFStream.ReadByte;
                if (byte1 == int('<')) && (byte2 == int('<'))
                    markerCount++;
                if (byte1 == int('>')) && (byte2 == int('>'))
                    markerCount--;

                found = (markerCount == 0);
                PDFStream.Position--;
            }

            if (found)
            {
                enddict = PDFStream.Position + 1;
                PDFStream.Position = startdict;
                uint8_t Buffer* = new uint8_t[enddict - startdict];
                PDFStream.Read(Buffer, 0, enddict - startdict);
                std::string Str = System.Text.Encoding.ASCII.GetString(Buffer, 0, enddict - startdict);
                DictObj = New PDFObject;
                DictObj.Value = Str;

                //Move to the beginning of the stream
                SearchPattern(PDF_STREAM, SearchDirection.Forward);
                PDFStream.Position += Len(PDF_STREAM);
                //stream must be followed either by a LF or a CR LF --> NO CR alone!
                byte1 = PDFStream.ReadByte;
                if (byte1 == 10)
                    StreamStartPos = PDFStream.Position;
                else if (byte1 == 13)
                {
                    byte2 = PDFStream.ReadByte;
                    if (byte2 == 10)
                        StreamStartPos = PDFStream.Position;
                    else
                    {
                        //MsgBox("ERROR!!! stream not followed by either LF or CR LF")
                    }
                }

                SearchPattern(PDF_ENDSTREAM, SearchDirection.Forward);
                StreamEndPos = PDFStream.Position;
            }
            else
            {
                DictObj = Nothing;
                StreamStartPos = -1;
                StreamEndPos = -1;
            }
        }

        else
        {
            DictObj = Nothing;
            StreamStartPos = -1;
            StreamEndPos = -1;
        }
    }


    PDFCrossReferenceTable* PDFDocument::ReadReferenceTable(PDFTrailer* Trailer)
    {
        PDFCrossReferenceTable* res = NULL;

        uint8_t byte1 = PDFStream.ReadByte;
        uint8_t byte2 = PDFStream.ReadByte;
        uint8_t byte3 = PDFStream.ReadByte;
        uint8_t byte4 = PDFStream.ReadByte;

        bool isAligned = (byte1 == int('x')) && (byte2 == int('r')) && (byte3 == int('e')) && (byte4 == int('f'));
            //Dim regexSubsection As New Regex("\d+\s\d+(?!(\s(n|f)))")
        Dim regexSubsectionEntry As New Regex("\d{10}\s\d{5}\s(n|f)");

            if( isAligned)
            {

                std::string line;
                res = New PDFCrossReferenceTable;
                int sectionSize;
                int sectionStart = PDFStream.Position;
                int trailerStart;
            if (SearchPattern(PDF_TRAILER, SearchDirection.Forward))
            {
                trailerStart = PDFStream.Position;

                Trailer = New PDFTrailer;
                Trailer.Dictionary.Value = ReadDictionary(trailerStart);

                sectionSize = trailerStart - sectionStart;
                uint8_t* tmpBuff = new uint8_t[sectionSize];
                PDFStream.Position = sectionStart;
                PDFStream.Read(tmpBuff, 0, sectionSize);

                std::string Str = System.Text.Encoding.ASCII.GetString(tmpBuff, 0, sectionSize);
                Str = Replace(Str, vbCrLf, vbCr);
                Str = Replace(Str, vbLf, vbCr);
                Dim lines() As String = Str.Split(vbCr);
                Dim listLine As New List(Of String);
                For Each line In lines
                    If line < > "" And Mid(line, 1, 1) < > "%" Then listLine.Add(line)
                    Next

                    bool done = false;
                PDFCrossReferenceSection* currentsection = new PDFCrossReferenceSection;
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


    }



            int PDFDocument::GetObjectPosition(PDFObject & indirectRef)
            {
                int res = -1;
                for (int n = 0; n < CrossRefTables.Count; n++)
                {
                    res = CrossRefTables[n].GetObjectPos(indirectRef);
                    if (res > -1)
                        return res;

                }
                return res;
            }

            void PDFDocument::ExtractXObjectImage(StreamDict As PDFObject, int StreamStartPos, int StreamEndPos, bool IsMask)
            {
                std::string typeStr;
                std::string SubType;
                std::string widthStr;
                std::string HeightStr;

                //Validate the type (OPTIONAL)
                if (StreamDict.ContainsKey(PDF_TYPE))
                {
                    typeStr = StreamDict.GetDictValue(PDF_TYPE).Value;
                    if (typeStr != PDF_XOBJECT)
                    {
                        //MsgBox("ERROR: type for Image must be 'XObject'")
                        return;
                    }
                }



                if (StreamDict.ContainsKey(PDF_SUBTYPE))
                {
                    SubType = StreamDict.GetDictValue(PDF_SUBTYPE).Value;
                    if (SubType != PDF_IMAGE)
                    {
                        //MsgBox("ERROR: subtype for Image must be 'Image'")
                        return;
                    }
                }


                if (StreamDict.ContainsKey(PDF_WIDTH))
                {
                    widthStr = StreamDict.GetDictValue(PDF_WIDTH).Value;
                }
                else
                {
                    //MsgBox("ERROR: image XObject must include the Width entry")
                    return;
                }


                if (StreamDict.ContainsKey(PDF_HEIGHT))
                {
                    HeightStr = StreamDict.GetDictValue(PDF_HEIGHT).Value;
                }
                else
                {
                    //MsgBox("ERROR: image XObject must include the Height entry")
                    return;
                }

                std::string BitsPerComponent = StreamDict.GetDictValue(PDF_BITS_PER_COMPONENT).Value;


                    //Dim ColorSpace As String = StreamDict.GetDictValue(PDF_COLORSPACE).Value
                PDFObject filterObj = StreamDict.GetDictValue(PDF_FILTER);



                std::string filterName;
                if (filterObj.Type == PDFObjectType.PDF_Name)
                {
                    filterName = filterObj.Value;
                }

                else if (filterObj.Type == PDFObjectType.PDF_Array)
                {
                    filterName = filterObj.ArrayElements(0).Value;
                }

                if (filterObj.Type == PDFObjectType.PDF_Name)
                {
                    filterName = filterObj.Value;
                }
                else if (filterObj.Type == PDFObjectType.PDF_Array)
                {
                    int a = 1;
                }


                switch (filterName)
                {
                case PDF_ASCII_HEX_DECODE:
                case PDF_ASCII_HEX_DECODE_ABREV:
                case PDF_CCITT_85_DECODE:
                case PDF_CCITT_85_DECODE_ABREV:
                case PDF_LZW_DECODE:
                case PDF_LZW_DECODE_ABREV:
                case PDF_FLATE_DECODE:
                case PDF_FLATE_DECODE_ABREV:
                case PDF_RUN_LENGTH_DECODE:
                case PDF_RUN_LENGTH_DECODE_ABREV:
                case PDF_CCITT_FAX_DECODE:
                case PDF_CCITT_FAX_DECODE_ABREV:
                {
                    int L = StreamEndPos - StreamStartPos + 1;
                    uint8_t* buffer = new uint8_t[L];
                    PDFStream.Position = StreamStartPos;

                    if (StreamDict.ContainsKey(PDF_DECODEPARAMS))
                    {
                        PDFObject DecodeParamsObj = StreamDict.GetDictValue(PDF_DECODEPARAMS);

                        //DecodeParamsObj is a dictionary or an array of dictionaries

                        int K = DecodeParamsObj.GetDictValue("K").Value;

                        if (StreamDict.ContainsKey("ImageMask"))
                        {
                            std::string isMaskStr = StreamDict.GetDictValue("ImageMask").Value;
                        }




                        //CCITTFaxDecodeFilter.K = DecodeParamsObj.GetDictValue("K").Value
                        //CCITTFaxDecodeFilter.Columns = DecodeParamsObj.GetDictValue("Width").Value
                        //CCITTFaxDecodeFilter.Rows = DecodeParamsObj.GetDictValue("Height").Value
                        Dim tiffF As New TIFFFile
                            tiffF.ByteOrder = ByteOrder.BitEndian;

                        if (K < 0)
                        {
                            // K < 0 --- Pure two-dimensional encoding (Group 4)
                            tiffF.Compression = Compression.T6Encoding;
                        }
                        else if (K > 0)
                        {
                            // K > 0 --- Mixed one- and two-dimensional encoding (Group 3, 2-D)
                            tiffF.Compression = Compression.T4Encoding;
                        }
                        else
                        {
                            // K = 0 --- Pure one-dimensional encoding (Group 3, 1-D)
                            tiffF.Compression = Compression.T4Encoding;
                        }


                        tiffF.Height = HeightStr;
                        tiffF.Width = widthStr;
                        tiffF.ReadImage(PDFStream, L);
                        std::string filePath;

                        if (IsMask)
                            filePath = NameNoExt & Images.Count & "_mask.Tiff";
                        else
                            filePath = NameNoExt & Images.Count & ".Tiff";



                        tiffF.Save(OutputDirectory, filePath);
                        Images.Add(filePath);

                    }
                    break;
                }

                case PDF_DCT_DECODE:
                case PDF_DCT_DECODE_ABREV:
                {

                    std::string filePath;
                    if (IsMask)
                        filePath = NameNoExt + Images.Count + "_mask.jpg";
                    else
                        filePath = NameNoExt + Images.Count + ".jpg";

                    int L = StreamEndPos - StreamStartPos + 1;
                    uint8_t* buffer = new uint8_t[L];
                    PDFStream.Position = StreamStartPos;
                    PDFStream.Read(buffer, 0, L);
                    delete[] buffer;

                    if (StreamDict.ContainsKey("Mask"))
                    {
                        PDFObject maskObj = StreamDict.GetDictValue("Mask");

                        //reference to a stream dict containing the image
                        PDFObject maskDictObj;
                        int maskStreamStartPos; // As Long
                        int maskStreamEndPos; // As Long
                        ReadStream(GetObjectPosition(maskObj), maskDictObj, maskStreamStartPos, maskStreamEndPos);
                        ExtractXObjectImage(maskDictObj, maskStreamStartPos, maskStreamEndPos);
                        int a = 0;
                    }


                    IO.File.WriteAllBytes(OutputDirectory & "\" & filePath, buffer)
                        Images.Add(filePath);

                }



                }



                PDFObject* PDFDocument::ParseResourcesDict(PDFObject& resObj )
                {
                    PDFObject* resDictObj = NULL;
                            //The standard says this should be a dictionary, but some pdfs place an indirect reference!
                    if (resObj.Type == PDFObjectType.PDF_Dictionary)
                    {
                        resDictObj = *resObj;
                    }
                    else if (resObj.Type == PDFObjectType.PDF_IndirectReference)
                    {
                        std::string resDict = ReadDictionary(GetObjectPosition(resObj));
                        resDictObj = New PDFObject;
                        resDictObj.Value = resDict;
                    }

                            //Parse Procedure Set Names
                    if (resDictObj.ContainsKey("ProcSet"))
                    {
                        PDFObject ProcSetObj = resDictObj.GetDictValue("ProcSet");
                        if (ProcSetObj.Type == PDFObjectType.PDF_IndirectReference)
                        {
                        }
                    }



                            //Parse XObject names
                    if (resDictObj.ContainsKey(PDF_XOBJECT))
                    {
                        PDFObject* ref = new PDFObject;
                        ref.Value = resDictObj.GetDictValue(PDF_XOBJECT).Value;

                        //The XObject is a dictionary with the images
                        Dim images As List(Of PDFObject) = ref.GetAllDictValues


                            For Each image As PDFObject In images
                            Dim DictObj As PDFObject
                            Dim StreamStartPos As Integer = -1
                            Dim StreamEndPos As Integer = -1

                            ReadStream(GetObjectPosition(image), DictObj, StreamStartPos, StreamEndPos)
                            ExtractXObjectImage(DictObj, StreamStartPos, StreamEndPos)
                            Next
                    }

                return resDictObj;
            }


                PDFObject* PDFDocument::ReadPageContents(int Pos)
                {
                    PDFObject* pagecontentsObj = new PDFObject;
                    PDFStream.Position = Pos;
                    if (SearchPattern(PDF_OBJ, SearchDirection.Forward))
                    {
                        int p1 = PDFStream.Position;

                        SearchPattern(PDF_ENDOBJ, SearchDirection.Forward);
                        int p2 = PDFStream.Position;

                        int cnt = p2 - p1 - 3;
                        uint8_t arr = new uint8_t[cnt];
                        PDFStream.Position = p1 + 3;
                        PDFStream.Read(arr, 0, cnt);
                        delete[] arr;

                        std::string Str = System.Text.Encoding.ASCII.GetString(arr, 0, cnt);
                        Str = Replace(Str, vbCrLf, "");
                        Str = Replace(Str, vbCr, "");
                        Str = Replace(Str, vbLf, "");

                        pagecontentsObj.Value = Str;
                    }
                    return pagecontentsObj;
                }


        void PDFDocument::ReadPages(int NodePosition, bool isRoot)
        {
            std::string nodeDict = ReadDictionary(NodePosition);
            PDFObject* node = new PDFObject;
            node.Value = nodeDict;

            std::string NodeType = node.GetDictValue(PDF_TYPE).Value;
        switch(NodeType)
        {
        case PDF_PAGES:
        {
            //Read Kids Array
            std::string kids = node.GetDictValue(PDF_KIDS).Value;
            Dim mCol As MatchCollection
                mCol = IndirectReferenceRegex.Matches(kids)
                For Each m As Match In mCol
                Dim indRef As New PDFObject
                indRef.Value = m.Value
                Dim objPos As Integer = GetObjectPosition(indRef)
                ReadPages(objPos, False)
                Next
                break;
        }
        case PDF_PAGE:
        {
            std::string Contents = node.GetDictValue(PDF_CONTENTS).Value;
            PDFObject* indRef = new PDFObject;
            indRef.Value = Contents;
            int objPos;

            if (indRef.Type == PDFObjectType.PDF_Array)
            {
                objPos = GetObjectPosition(indRef.ArrayElements(0));
            }
            else if (indRef.Type == PDFObjectType.PDF_IndirectReference)
            {
                objPos = GetObjectPosition(indRef);
            }
            else
            {
                //MsgBox("Check!!!")
            }


            std::string Resources = node.GetDictValue(PDF_RESOURCES).Value;
            PDFObject* objDict = new PDFObject;
            objDict.Value = Resources;
            PDFObject ResourcesDict = ParseResourcesDict(objDict);

            CommandParser.Clear();
            CommandParser.SetPageResourceDict(ResourcesDict);
            PDFObject contentsObj = ReadPageContents(objPos);

            if (contentsObj.Type == PDFObjectType.PDF_Array)
            {
                PDFObject reft = contentsObj.ArrayElements(0);
                if (reft.Type == PDFObjectType.PDF_IndirectReference)
                {
                    int contentobjPos = GetObjectPosition(reft);

                    PDFObject contentStreamDict;
                    int StreamStartPos;
                    int StreamEndPos;
                    ReadStream(contentobjPos, contentStreamDict, StreamStartPos, StreamEndPos);

                    PDFObject lengthStr;
                    PDFObject filter;
                    if (contentStreamDict.ContainsKey(PDF_FILTER))
                    {
                        filter = New PDFObject;
                        filter.Value = contentStreamDict.GetDictValue(PDF_FILTER).Value;
                    }


                    if (contentStreamDict.ContainsKey(PDF_LENGTH))
                    {
                        lengthStr = New PDFObject;
                        lengthStr.Value = contentStreamDict.GetDictValue(PDF_LENGTH).Value;
                    }

                    int L = StreamEndPos - StreamStartPos + 1 - 2;
                    uint8_t* buffer = uint8_t[L];
                    PDFStream.Position = StreamStartPos + 2;
                    PDFStream.Read(buffer, 0, L);


                    if (filter != NULL)
                    {
                        Dim Stream As New MemoryStream();
                        Dim compressStream As New MemoryStream(buffer);

                        Dim decompressor As New DeflateStream(compressStream, CompressionMode.Decompress);
                        decompressor.CopyTo(Stream);

                        std::string str = Encoding.Default.GetString(Stream.ToArray());
                        CommandParser.Parse(str);
                    }
                }
                else
                {
                }

            }
        }


        void PDFDocument::Load(std::string FilePath)
        {
            std::ifstream file(FilePath);
            Lines.clear();
            p = FilePath;
            Name = p.replace_extension();
            NameNoExt = p.replace_extension();
            Images.Clear();

            std::string line;

            while (std::getline(file, line)
            {
                std::istringstream ss(line);
                Lines.push_back(line);
            }
            file.close();

            Trailer = New PDFTrailer;
            NumLines = (int)Lines.size();


            Dim offset As Integer = 0
                FileInfo = New FileInfo(FilePath)
                Size = FileInfo.Length

                //PDFStream = New FileStream(FilePath, FileMode.Open)
                unsigned char tmpBuffer[BUFF_SIZE];


                //Read Trailer first. Load 1024 bytes from the end
            file.seekg(0, ios_base::end);
            int length = file.tellg();

            file.seekg(length - BUFF_SIZE, ios_base::beg);
            file.read(tmpBuffer, 0, BUFF_SIZE);

            int eofIndex = SearchForEOF(tmpBuffer, BUFF_SIZE);
            if (eofIndex == -1)
            {
                MsgBox("ERROR:: Could not read the trailer")
            }
            int currentPos = eofIndex;
            bool patternOK = SearchPattern(tmpBuffer, PDF_START_XREF, currentPos, SearchDirection::BACKWARDS);
            std::string str = System.Text.Encoding.ASCII.GetString(tmpBuffer, currentPos, eofIndex - currentPos);
            str = Replace(str, PDF_START_XREF, "");
            //str = Replace(str, vbCrLf, "");
            //str = Replace(str, vbCr, "");
            str = Replace(str, "\n", "");

            Trailer->startXRef = std::stoi(str);
            int startxrefIndex = currentPos;
            patternOK = SearchPattern(tmpBuffer, PDF_TRAILER, currentPos, SearchDirection::BACKWARDS);
            int trailerStartPos = Size - BUFF_SIZE + currentPos;
            std::string trailerDict = ReadDictionary(tmpBuffer, currentPos, BUFF_SIZE);
            Trailer->Dictionary.Value = trailerDict;
            Trailer->Position = trailerStartPos;

            ReadCRTs(Trailer, true);


                //Check Trailer dict content
            int TotalNumCRTEntries = Trailer->Dictionary.GetDictValue("Size").Value;
            bool fileHasMoreThanOneCRT = Trailer->Dictionary.ContainsKey("Prev");
            if (Trailer->Dictionary.ContainsKey(PDF_ROOT))
            {
                PDFObject indRef = TrailerDictionaryr->GetDictValue(PDF_ROOT);
                int catalogPos = GetObjectPosition(indRef);
                if (catalogPos > -1)
                {
                    std::string catalogDict = ReadDictionary(catalogPos);
                    Catalog.Value = catalogDict;

                    //Validate Catalog
                    bool CatalogOK = Catalog.GetDictValue("Type").Value == "Catalog";
                    PDFObject* pageTreeNode = New PDFObject();
                    pageTreeNode->Value = Catalog.GetDictValue("Pages").Value;
                    int pageTreeNodePos = GetObjectPosition(pageTreeNode);
                    ReadPages(pageTreeNodePos, true);
                }
            }
            bool fileHasInfoDictionary = Trailer->Dictionary.ContainsKey("Info");
            bool fileIsEncryptedAs = Trailer->Dictionary.ContainsKey("Encrypt");


                //Check if file is linearized
                //PDFStream.Seek(0, SeekOrigin.Begin)
                //PDFStream.Read(tmpBuffer, 0, BUFF_SIZE)
                //Dim objPos As Integer = SearchForOBJ(tmpBuffer, BUFF_SIZE)
                //Dim endObjPos As Integer = SearchForENDOBJ(tmpBuffer, BUFF_SIZE, objPos)
                //str = System.Text.Encoding.ASCII.GetString(tmpBuffer, objPos, endObjPos - objPos - 2)
                //IsLinearized = InStr(str, "/Linearized") > 0
                //If IsLinearized Then
                //    LinearizationParameterDictionary = New PDFObject
                //    LinearizationParameterDictionary.Value = ReadDictionary(tmpBuffer, objPos, BUFF_SIZE)

                //    'Sanity check : read L
                //    Dim L As Integer = LinearizationParameterDictionary.GetDictValue("L").Value
                //    If L <> Size Then
                //        MsgBox("ERROR:: L and Size are not equal!!!!")
                //        Return
                //    End If
                //    Dim N As Integer = LinearizationParameterDictionary.GetDictValue("N").Value
                //End If
            file.close();

        }

        PDFObject* PDFDocument::ReadNextObject(int* offset)
        {
            int index = *offset;
            PDFObject* res = NULL;
            int objStartIndex = 0;
            int objStopIndex = 0;

            regex = New Regex("\d\s\d\s" & PDF_OBJ, RegexOptions.Singleline);
            bool Found = false;
            while (!Found && index < NumLines)
            {
                Found = regex.IsMatch(Lines(offset));
                index++;
            }

            if (Found)
            {
                objStartIndex = index;
                std::vector<std::string> nums;
                Split(Lines[index - 1], " ", nums);

                int objNumber = nums[0];
                int objGenNumber = nums[1];


                Found = false;
                regex = New Regex(PDF_ENDOBJ, RegexOptions.Singleline);
                while (!Found && (index < NumLines))
                {
                    Found = regex.IsMatch(Lines(offset));
                    index++;
                }

                if (Found)
                {
                    objStopIndex = offset - 2;
                    std::string ObjValue = "";
                    int cnt = 0;
                    for (int n = objStartIndex; n <= objStopIndex; n++)
                    {
                        ObjValue = ObjValue & Lines[n].Trim();
                        if (cnt < objStopIndex - objStartIndex)
                            ObjValue = ObjValue + "\n";
                        cnt++;
                    }

                    res = New PDFObject();
                    res->Value = ObjValue;
                }
            }

            *offset = index;
            return res;
        }
        
        bool CheckFirstPageTrailer(PDFTrailer* firstPageTrailer)
        {
            bool res = true;

            if (!firstPageTrailer->Dictionary.ContainsKey(PDF_SIZE))
                res = false;
            if (!firstPageTrailer->Dictionary.ContainsKey(PDF_ROOT))
                res = false;

            return res;
        }

        bool CheckIfLinearized(int* offset)
        {
            int dictStartIndex;
            int dictStopIndex;
            bool res = false;

            regex = New Regex("\d\s\d\s" & PDF_OBJ, RegexOptions.Singleline)
                bool Found = false;
            while (!Found && offset < NumLines)
            {
                Found = regex.IsMatch(Lines(offset));
                offset++;
            }

            if (Found)
            {
                dictStartIndex = offset;
                std::vector<string> nums;
                Split(Lines [[offset - 1]], " ", nums);
                //Dim nums() As String = Lines(offset - 1).Split(" ")
                int objNumber = std::stoi(nums[0]);
                int objGenNumber = std::stoi(nums[1]);

                LinearizationParameterDictionary = new PDFObject(PDFObjectType.PDF_Dictionary);
                found = false;
                regex = New Regex(PDF_ENDOBJ, RegexOptions.Singleline);

                while (!Found && offset < NumLines)
                {
                    found = regex.IsMatch(Lines(offset));
                    offset++;
                }

                /* 'If Found Then
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

                 'End If*/
                res = true;
            }

            return res;
        }




        bool PDFDocument::ProcessHeader(int* offset)
        {

            std::string line = Lines[0].Trim

                if (line[0] == '%')
                {
                    line = Mid(line, 2, Len(line) - 1);
                    Version = line;
                    //check next line, probably a comment with binary data
                    line = Lines[1];
                    if (line[0] == '%')
                        offset = 2;
                    else
                        offset = 1;


                    return true;
                    //Select Case line
                    //    Case PDF_VERSION_1_0
                    //        Version = line
                    //    Case PDF_VERSION_1_1
                    //        Version = line
                    //    Case PDF_VERSION_1_2
                    //        Version = line
                    //    Case PDF_VERSION_1_3
                    //        Version = line
                    //    Case PDF_VERSION_1_4
                    //        Version = line
                    //    Case Else
                    //        Return False
                    //End Select
                }
                else
                    return false;

           
            return true;
        }




    PDFCrossReferenceTable* ProcessCrossReferenceTable(int offset)
    {
        bool found = false;
        PDFCrossReferenceTable* res = new PDFCrossReferenceTable;

        while (!found && (offset < NumLines))
        {
            std::string line = Lines[offset];
            found = (InStr(line, PDF_XREF) > 0) && (line <> PDF_START_XREF);

            if (found)
            {
                PDFCrossReferenceSection* newSection = new PDFCrossReferenceSection();
                offset++;
                //Start subsections
                regex = New Regex("\d\s\d", RegexOptions.Singleline);
                bool FoundSubsections = regex.IsMatch(Lines(offset));
                while (FoundSubsections)
                {
                    Dim nums() As String = Lines(offset).Split(" ");
                    int objNum = nums(0);
                    int numObjects = nums(1);
                    PDFSubsection* newSubsection = new PDFSubsection;

                    offset++;
                    for (int n = 0; n < numObjects; n++)
                    {
                        std::string subSectionLine = Lines(offset + n);
                        int objNumber = objNum + n;

                        CrossReferenceEntry* entry = new CrossReferenceEntry;
                        entry.ObjectNumber = objNumber;

                        nums = subSectionLine.Split(" ");
                        if (nums.Length == 3)
                        {
                            entry.ByteOffset = nums(0);
                            entry.GenerationNumber = nums(1);
                            if (nums(2) == "n")
                            {
                                entry.Type = SubsectionEntryType.InUse;
                            }
                            else if (nums(2) == "f")
                            {
                                entry.Type = SubsectionEntryType.Free;
                            }
                        }
                        else
                        {
                            //MsgBox("ERROR:: Subsection format not recognized")
                            Exit Function;
                        }
                            newSubsection.Entries.Add(entry);
                    }

                    offset += numObjects;
                    FoundSubsections = regex.IsMatch(Lines(offset));
                    newSection.Subsections.Add(newSubsection);
                }

                res.Sections.Add(newSection);

            }

            offset++;
        }
        return res;
    }
