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
};











#endif