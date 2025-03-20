#ifndef PDF_CROSS_REFERENCE_ENTRY_HPP__
#define PDF_CROSS_REFERENCE_ENTRY_HPP__

#include "./PDFDefinitions.hpp"

class CrossReferenceEntry
{
public:
    int ObjectNumber;
    int ByteOffset;
    int GenerationNumber;
    PDFSubsectionEntryType Type;
};


#endif