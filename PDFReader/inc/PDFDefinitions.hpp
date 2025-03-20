#ifndef PDF_DEFINITIONS_HPP__
#define PDF_DEFINITIONS_HPP__

#include <string>
#include <regex>

enum class PDFObjectType
{
    PDF_BOOLEAN = 0,
    PDF_NUMBER,
    PDF_STRING,
    PDF_NAME,
    PDF_ARRAY,
    PDF_DICTIONARY,
    PDF_STREAM,
    PDF_NULL,
    PDF_INDIRECT_REFERENCE
};


enum class PDFSubsectionEntryType
{
    IN_USE = 0,
    FREE
};

const std::string PDF_VERSION_1_0 = "PDF-1.0";
const std::string PDF_VERSION_1_1 = "PDF-1.1";
const std::string PDF_VERSION_1_2 = "PDF-1.2";
const std::string PDF_VERSION_1_3 = "PDF-1.3";
const std::string PDF_VERSION_1_4 = "PDF-1.4";

const std::string PDF_TRAILER = "trailer";
const std::string PDF_START_XREF = "startxref";
const std::string PDF_EOF = "%%EOF";

const std::string PDF_TRUE = "true";
const std::string PDF_FALSE = "false";
const std::string PDF_XREF = "xref";
const std::string PDF_OBJ = "obj";
const std::string PDF_ENDOBJ = "endobj"

const std::string PDF_LINEARIZED = "Linearized";

const std::string PDF_SIZE = "Size";
const std::string PDF_ROOT = "Root";
const std::string PDF_INFO = "Info";
const std::string PDF_PREV = "Prev";
const std::string PDF_ID = "ID";

const std::string PDF_TYPE = "Type";
const std::string PDF_PAGES = "Pages";
const std::string PDF_PAGE = "Page";
const std::string PDF_KIDS = "Kids";
const std::string PDF_PARENT = "Parent";
const std::string PDF_COUNT = "Count";


const std::string PDF_CONTENTS = "Contents";
const std::string PDF_STREAM  = "stream";
const std::string PDF_ENDSTREAM  = "endstream";
const std::string PDF_RESOURCES  = "Resources";
const std::string PDF_MEDIABOX  = "MediaBox";
const std::string PDF_LENGTH  = "Length";
const std::string PDF_FILTER  = "Filter";

const std::string PDF_EXTGSTATE  = "ExtGState";
const std::string PDF_COLORSPACE  = "ColorSpace";
const std::string PDF_PATTERN  = "Pattern";
const std::string PDF_SCHADING  = "ShadIng";
const std::string PDF_XOBJECT  = "XObject";
const std::string PDF_FONT  = "Font";
const std::string PDF_PROCSET  = "ProcSet";
const std::string PDF_PROPERTIES  = "Properties";

//Definitions for Procsets
const std::string PDF_PDF  = "PDF";
const std::string PDF_TEXT  = "Text";
const std::string PDF_IMAGEB  = "ImageB";
const std::string PDF_IMAGEC  = "ImageC";
const std::string PDF_IMAGEI  = "ImageI";
const std::string PDF_IMAGE  = "Image";

const std::string PDF_SUBTYPE  = "Subtype";
const std::string PDF_WIDTH  = "Width";
const std::string PDF_HEIGHT  = "Height";
const std::string PDF_BITS_PER_COMPONENT  = "BitsPerComponent";
const std::string PDF_INTENT  = "Intent";
const std::string PDF_IMAGEMASK  = "ImageMask";
const std::string PDF_MASK  = "Mask";
const std::string PDF_SMASK  = "SMask";
const std::string PDF_DECODE  = "Decode";
const std::string PDF_INTERPOLATE  = "Interpolate";
const std::string PDF_ALTERNATES  = "Alternates";
const std::string PDF_STRUCTPARENT  = "StructParent";
const std::string PDF_OPI  = "OPI";
const std::string PDF_METADATA  = "Metadata";
const std::string PDF_DECODEPARAMS  = "DecodeParms";


const std::string PDF_ASCII_HEX_DECODE  = "ASCIIHexDecode";
const std::string PDF_CCITT_85_DECODE  = "ASCII85Decode";
const std::string PDF_LZW_DECODE  = "LZWDecode";
const std::string PDF_FLATE_DECODE  = "FlateDecode";
const std::string PDF_RUN_LENGTH_DECODE  = "RunLengthDecode";
const std::string PDF_CCITT_FAX_DECODE  = "CCITTFaxDecode";
const std::string PDF_DCT_DECODE  = "DCTDecode";

const std::string PDF_ASCII_HEX_DECODE_ABREV  = "AHx";
const std::string PDF_CCITT_85_DECODE_ABREV  = "A85";
const std::string PDF_LZW_DECODE_ABREV  = "LZW";
const std::string PDF_FLATE_DECODE_ABREV  = "Fl";
const std::string PDF_RUN_LENGTH_DECODE_ABREV  = "RL";
const std::string PDF_CCITT_FAX_DECODE_ABREV  = "CCF";
const std::string PDF_DCT_DECODE_ABREV  = "DCT";

const std::string REGEX_PDF_NAME = "/[a-zA-Z0-9#_]+";

const std::regex IndirectReferenceRegex("\\d+\\s\\d+\\sR");

const std::string REGEX_PDF_ARRAY As String = "/[a-zA-Z0-9#_]*?";
const std::regex NameRegex(REGEX_PDF_NAME);
#endif