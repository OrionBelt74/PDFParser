#ifndef PDF_CROSS_REFERENCE_SECTION_HPP__
#define PDF_CROSS_REFERENCE_SECTION_HPP__

#include <vector>
#include "./PDFSubsection.hpp"

class PDFCrossReferenceSection
{
    std::vector <PDFSubsection*> Subsections;
};


#endif