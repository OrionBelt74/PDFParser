#ifndef PDF_SUBSECTION_HPP__
#define PDF_SUBSECTION_HPP__

#include <vector>
#include "./CrossReferenceEntry.hpp"

class PDFSubsection
{
    std::vector<CrossReferenceEntry*> Entries;
};

#endif