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
    PDFObject(PDFObjectType Objectype = PDFObjectType::PDF_NULL);
    ~PDFObject();

    PDFObjectType Type;
    std::string Value;
    std::map<std::string, PDFObject*> dict;
    std::vector<char> WhiteSpaces;
    std::vector<char> Delimiters;
    std::pair<int, int> IndirectReference;
    std::vector<PDFObject*> ArrayElements;
    
        void AddEntry(std::string key, std::string val);
        int GetDelimiterPosition(std::string& s);
        void ExtractKeyValuePairs(std::string& s, std::vector<std::pair<std::string, std::string>>& keyValuePairs);
        PDFObject* GetDictValue(std::string key);
        void GetAllDictValues(std::vector<PDFObject*>& result);
        bool ContainsKey(std::string key);
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