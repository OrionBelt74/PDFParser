#include "../inc/PDFObject.hpp"
#include "../inc/PDFDefinitions.hpp"
#include <regex>
#include <cassert>

PDFObject::PDFObject(PDFObjectType ObjecType)
{
    Type = ObjectType;
}

PDFObject::~PDFObject()
{


}




Property Type As PDFObjectType
Set(value As PDFObjectType)
_Type = value
End Set
Get
Return _Type
End Get
End Property
Property Value As String
Set(val As String)
_Value = val
ProcessValue()
End Set



void PDFObject::AddEntry(std::string key, std::string val)
{
    //key element MUST be a name
    bool isName = (key[0] == "/");

    assert(isName);

    PDFObject* valObj = new PDFObject();
    valObj->Value = val;
    valObj->ProcessValue();

    std::string newKey = Trim(key.substr(1, key.size()-1);
    dict[newKey] = valObj;

}


int PDFObject::GetDelimiterPosition(std::string& s)
{
    bool found = false;
    std::vector<int> positions;
    std::size_t ind;

    for (std::string delimiter : Delimiters)
    {
        ind = s.find(delimiter);
        found = (ind != std::string::npos);
        if (found)
            positions.push_back(ind);
    }


    for (std::string whiteSpace : WhiteSpaces)
    {
        ind = s.find(whiteSpace);
        found = (ind != std::string::npos);
        if (found)
            positions.push_back(ind);
    }


    if (positions.size() > 0)
    {
        int minVal = (int)s.size() + 1;
        for (std::size_t pos : positions)
        {
            minVal = (minVal < pos) ? minVal : pos;
        }
        return minVal;
    }
    else
        return -1;
}

//'    Private Static String decompress(Byte[] input)
//'{
//'    Byte[] cutinput = New Byte[input.Length - 2];
//'    Array.Copy(input, 2, cutinput, 0, cutinput.Length);
//
//'    var stream = New MemoryStream();
//
//'    Using (var compressStream = New MemoryStream(cutinput))
//'    Using (var decompressor = New DeflateStream(compressStream, CompressionMode.Decompress))
//'        decompressor.CopyTo(stream);
//
//'    Return Encoding.Default.GetString(stream.ToArray());
//'}


void PDFObject::ExtractKeyValuePairs(std::string& s, std::vector<std::pair<std::string, std::string>>& keyValuePairs)
{
    int delPos = GetDelimiterPosition(s);
    std::string sTemp = s;
    std::string key;
    std::string value;

        Dim m As Match
        Dim res As New List(Of KeyValuePair(Of String, String))
        While sTemp < > ""
        If NameRegex.IsMatch(sTemp, 0) Then
        m = NameRegex.Match(sTemp)
        key = m.Value
        Else
        MsgBox("ERROR: key must be a name!!!")
        Return Nothing
        End If
        sTemp = NameRegex.Replace(sTemp, "", 1).Trim()
        sTemp.Trim()


        If DictRegex.IsMatch(sTemp) AndAlso DictRegex.Match(sTemp).Value = Mid(sTemp, 1, Len(DictRegex.Match(sTemp).Value)) Then
        m = DictRegex.Match(sTemp)
        value = m.Value
        sTemp = DictRegex.Replace(sTemp, "", 1).Trim()

        ElseIf ArrayRegex.IsMatch(sTemp) AndAlso ArrayRegex.Match(sTemp).Value = Mid(sTemp, 1, Len(ArrayRegex.Match(sTemp).Value)) Then
        m = ArrayRegex.Match(sTemp)
        value = m.Value
        sTemp = ArrayRegex.Replace(sTemp, "", 1).Trim()

        ElseIf HexDataRegex.IsMatch(sTemp) AndAlso HexDataRegex.Match(sTemp).Value = Mid(sTemp, 1, Len(HexDataRegex.Match(sTemp).Value)) Then
        m = HexDataRegex.Match(sTemp)
        value = m.Value
        sTemp = HexDataRegex.Replace(sTemp, "", 1).Trim()

        ElseIf BooleanRegex.IsMatch(sTemp) AndAlso BooleanRegex.Match(sTemp).Value = Mid(sTemp, 1, Len(BooleanRegex.Match(sTemp).Value)) Then
        m = BooleanRegex.Match(sTemp)
        value = m.Value
        sTemp = BooleanRegex.Replace(sTemp, "", 1).Trim()

        ElseIf IndirectReferenceRegex.IsMatch(sTemp, 0) AndAlso IndirectReferenceRegex.Match(sTemp).Value = Mid(sTemp, 1, Len(IndirectReferenceRegex.Match(sTemp).Value)) Then
        m = IndirectReferenceRegex.Match(sTemp)
        value = m.Value
        sTemp = IndirectReferenceRegex.Replace(sTemp, "", 1).Trim()

        ElseIf NumericRegex.IsMatch(sTemp) AndAlso NumericRegex.Match(sTemp).Value = Mid(sTemp, 1, Len(NumericRegex.Match(sTemp).Value)) Then
        m = NumericRegex.Match(sTemp)
        value = m.Value
        sTemp = NumericRegex.Replace(sTemp, "", 1).Trim()

        ElseIf NameRegex.IsMatch(sTemp) AndAlso NameRegex.Match(sTemp).Value = Mid(sTemp, 1, Len(NameRegex.Match(sTemp).Value)) Then
        m = NameRegex.Match(sTemp)
        value = m.Value
        sTemp = NameRegex.Replace(sTemp, "", 1).Trim()
        Else
        Dim b = 1
        value = sTemp
        sTemp = ""
        End If

        res.Add(New KeyValuePair(Of String, String)(key, value))


        Dim a = 1

        End While


        Return res

}


Private Function ExtractArrayElements(s As String) As List(Of String)
Dim sTemp As String = s

Dim element As String
Dim res As New List(Of String)
Dim mcol As MatchCollection

While sTemp < > ""

If DictRegex.IsMatch(sTemp) AndAlso DictRegex.Match(sTemp).Value = Mid(sTemp, 1, Len(DictRegex.Match(sTemp).Value)) Then
mcol = DictRegex.Matches(sTemp)
For Each m As Match In mcol
element = m.Value
res.Add(element)
Next
sTemp = DictRegex.Replace(sTemp, "").Trim()

ElseIf ArrayRegex.IsMatch(sTemp) AndAlso ArrayRegex.Match(sTemp).Value = Mid(sTemp, 1, Len(ArrayRegex.Match(sTemp).Value)) Then
mcol = ArrayRegex.Matches(sTemp)
For Each m As Match In mcol
element = m.Value
res.Add(element)
Next
sTemp = ArrayRegex.Replace(sTemp, "").Trim()

ElseIf HexDataRegex.IsMatch(sTemp) AndAlso HexDataRegex.Match(sTemp).Value = Mid(sTemp, 1, Len(HexDataRegex.Match(sTemp).Value)) Then
mcol = HexDataRegex.Matches(sTemp)
For Each m As Match In mcol
element = m.Value
res.Add(element)
Next
sTemp = HexDataRegex.Replace(sTemp, "").Trim()

ElseIf BooleanRegex.IsMatch(sTemp) AndAlso BooleanRegex.Match(sTemp).Value = Mid(sTemp, 1, Len(BooleanRegex.Match(sTemp).Value)) Then
mcol = BooleanRegex.Matches(sTemp)
For Each m As Match In mcol
element = m.Value
res.Add(element)
Next
sTemp = BooleanRegex.Replace(sTemp, "").Trim()

ElseIf IndirectReferenceRegex.IsMatch(sTemp, 0) AndAlso IndirectReferenceRegex.Match(sTemp).Value = Mid(sTemp, 1, Len(IndirectReferenceRegex.Match(sTemp).Value)) Then
mcol = IndirectReferenceRegex.Matches(sTemp)
For Each m As Match In mcol
element = m.Value
res.Add(element)
Next
sTemp = IndirectReferenceRegex.Replace(sTemp, "").Trim()

ElseIf NumericRegex.IsMatch(sTemp) AndAlso NumericRegex.Match(sTemp).Value = Mid(sTemp, 1, Len(NumericRegex.Match(sTemp).Value)) Then
mcol = NumericRegex.Matches(sTemp)
For Each m As Match In mcol
element = m.Value
res.Add(element)
Next
sTemp = NumericRegex.Replace(sTemp, "").Trim()

ElseIf NameRegex.IsMatch(sTemp) AndAlso NameRegex.Match(sTemp).Value = Mid(sTemp, 1, Len(NameRegex.Match(sTemp).Value)) Then
mcol = NameRegex.Matches(sTemp)
For Each m As Match In mcol
element = m.Value
res.Add(element)
Next
sTemp = NameRegex.Replace(sTemp, "").Trim()
Else
Dim b = 1
End If



End While
Return res

End Function

Private Sub ProcessValue()

Dim tmpStr As String
Dim KeyValuePairs As List(Of KeyValuePair(Of String, String))

If Len(Value.Trim) >= 2 AndAlso(Mid(Value, 1, 2) = "<<" And Mid(Value, Len(Value) - 1, 2) = ">>") Then
Type = PDFObjectType.PDF_Dictionary
tmpStr = Mid(Value, 3, Len(Value) - 4).Trim

'Do not process based on lines! values can be also dictionaries which are split in different lines.
_dict.Clear()

KeyValuePairs = ExtractKeyValuePairs(tmpStr)
For Each kvp As KeyValuePair(Of String, String) In KeyValuePairs
AddEntry(kvp.Key, kvp.Value)
Next


ElseIf Mid(Value, 1, 1) = "[" And Mid(Value, Len(Value), 1) = "]" Then
Type = PDFObjectType.PDF_Array
tmpStr = Mid(Value, 2, Len(Value) - 2).Trim
ArrayElements.Clear()
Dim elements As List(Of String) = ExtractArrayElements(tmpStr)
For Each s As String In elements
Dim pdfObj As New PDFObject
pdfObj.Value = s
ArrayElements.Add(pdfObj)
Next


ElseIf Mid(Value, 1, 1) = "/" Then
Type = PDFObjectType.PDF_Name
_Value = Mid(Value, 2, Len(Value) - 1)


ElseIf IndirectReferenceRegex.IsMatch(Value.Trim) Then
Type = PDFObjectType.PDF_IndirectReference
Dim val As String = Value.Replace("R", "").Trim
Dim comps() As String = val.Split(" ")
IndirectReference = New Tuple(Of Integer, Integer)(comps(0).Trim, comps(1).Trim)

ElseIf NumericRegex.IsMatch(Value.Trim) Then
Type = PDFObjectType.PDF_Number
End If







End Sub
Public Function GetDictValue(key As String) As PDFObject
If Type = PDFObjectType.PDF_Dictionary Then
If _dict.ContainsKey(key) Then
Return _dict(key)
Else
Return Nothing
End If


ElseIf Type = PDFObjectType.PDF_Array Then
'Assume each element is a dictionary
Dim found As Boolean = False
Dim ind As Integer = 0
While Not found And ind < ArrayElements.Count
    Dim pdfD As PDFObject = ArrayElements(ind)
    found = pdfD.ContainsKey(key)
    If found Then
    Return pdfD.GetDictValue(key)
    End If
    ind += 1
    End While
    Else
    Return Nothing
    End If
    End Function
    Public Function GetAllDictValues() As List(Of PDFObject)
    Dim res As New List(Of PDFObject)
    If Type = PDFObjectType.PDF_Dictionary Then
    For Each v As PDFObject In _dict.Values
    res.Add(v)
    Next
    Return res
    Else
    Return Nothing
    End If
    End Function

    Public Function ContainsKey(key As String) As Boolean
    If Type = PDFObjectType.PDF_Dictionary Then
    Return _dict.ContainsKey(key)
    Else
    Return False
    End If
    End Function
    End Class