#ifndef PDF_OBJECT
#define PDF_OBJECT

#include <vector>
#include <string>
#include <map>
#include <tuple>
#include <regex>

#include "./PDFDefinitions.hpp"

class PDFObject
{
public:
    PDFObject(PDFObjectType Objectype = PDFObjectType.PDF_NULL);
    ~PDFObject();

    PDFObjectType Type;
    std::string Value;
    std::map<std::string, PDFObject*> dict;
    std::vector<std::string> WhiteSpaces;
    std::vector<std::string> Delimiters;
    

    Private WhiteSpaces() As String = { Chr(0), Chr(9), Chr(10), Chr(12), Chr(13), Chr(32) }
        Private Delimiters() As String = { "[", "]", "{", "}", "<", ">", "%", "(", ")" }
        Public IndirectReference As Tuple(Of Integer, Integer)
        Public ArrayElements As New List(Of PDFObject)
        void AddEntry(std::string& key, std::string& val);
        int GetDelimiterPosition(std::string& s);
        void ExtractKeyValuePairs(std::string& s, std::vector<std::pair<std::string, std::string>>& keyValuePairs);
};

class PDFBody
{
    std::vector <PDFObject*> IndirectedObjects;
};

class PDFTrailer
{
public:
    int startXRef;
    PDFObject* dictionary;
    int Position;

};

#endif