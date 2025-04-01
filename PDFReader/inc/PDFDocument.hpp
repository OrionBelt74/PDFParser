#ifndef PDF_DOCUMENT
#define PDF_DOCUMENT

#include <string>

enum class SearchDirection
{
    FORWARD = 0,
    BACKWARDS
};

class PDFDocument
{
public:
    PDFDocument();
    ~PDFDocument();

    std::string Name;
    std::string NameNoExt;

    int SearchForEOF(uint8_t* Buff, int size);
    int SearchForOBJ(uint8_t* Buff, int size, int startIndex = 0);
    int SearchForENDOBJ(uint8_t* Buff, int size, int startIndex = 0);
    void MoveToPreviousEOL(uint8_t* Buff, int* currentPos);
    bool SearchPattern(uint8_t* Buff, std::string Pattern, int* currentPos, SearchDirection direction);
    bool SearchPattern(std::string& Pattern, SearchDirection searchDirection);
    bool ProcessHeader(int* offset);
    std::string ReadDictionary(uint8_t* buffer, int startPos, int buffersize);
    std::string ReadDictionary(int startPos);
    void ReadStream(int startPos, PDFObject* DictObj, int* StreamStartPos, int* StreamEndPos);
    PDFCrossReferenceTable* ReadReferenceTable(PDFTrailer* Trailer);

    PDFCrossReferenceTable* ProcessCrossReferenceTable(int offset);
    int GetObjectPosition(PDFObject& indirectRef);
    void ExtractXObjectImage(PDFObject& StreamDict, int StreamStartPos, int StreamEndPos, bool IsMask = false);
    PDFObject* ParseResourcesDict(PDFObject& resObj );
    PDFObject* ReadPageContents(int Pos);
    void ReadPages(int NodePosition, bool isRoot);
};











#endif