Module PDFConstants
    Public Const PDF_VERSION_1_0 As String = "PDF-1.0"
    Public Const PDF_VERSION_1_1 As String = "PDF-1.1"
    Public Const PDF_VERSION_1_2 As String = "PDF-1.2"
    Public Const PDF_VERSION_1_3 As String = "PDF-1.3"
    Public Const PDF_VERSION_1_4 As String = "PDF-1.4"

    Public Const PDF_TRAILER As String = "trailer"
    Public Const PDF_START_XREF As String = "startxref"
    Public Const PDF_EOF As String = "%%EOF"

    Public Const PDF_TRUE As String = "true"
    Public Const PDF_FALSE As String = "false"
    Public Const PDF_XREF As String = "xref"
    Public Const PDF_OBJ As String = "obj"
    Public Const PDF_ENDOBJ As String = "endobj"

    Public Const PDF_LINEARIZED As String = "Linearized"

    Public Const PDF_SIZE As String = "Size"
    Public Const PDF_ROOT As String = "Root"
    Public Const PDF_INFO As String = "Info"
    Public Const PDF_PREV As String = "Prev"
    Public Const PDF_ID As String = "ID"

    Public Const PDF_TYPE As String = "Type"
    Public Const PDF_PAGES As String = "Pages"
    Public Const PDF_PAGE As String = "Page"
    Public Const PDF_KIDS As String = "Kids"
    Public Const PDF_PARENT As String = "Parent"
    Public Const PDF_COUNT As String = "Count"


    Public Const PDF_CONTENTS As String = "Contents"
    Public Const PDF_STREAM As String = "stream"
    Public Const PDF_ENDSTREAM As String = "endstream"
    Public Const PDF_RESOURCES As String = "Resources"
    Public Const PDF_MEDIABOX As String = "MediaBox"
    Public Const PDF_LENGTH As String = "Length"
    Public Const PDF_FILTER As String = "Filter"

    Public Const PDF_EXTGSTATE As String = "ExtGState"
    Public Const PDF_COLORSPACE As String = "ColorSpace"
    Public Const PDF_PATTERN As String = "Pattern"
    Public Const PDF_SCHADING As String = "ShadIng"
    Public Const PDF_XOBJECT As String = "XObject"
    Public Const PDF_FONT As String = "Font"
    Public Const PDF_PROCSET As String = "ProcSet"
    Public Const PDF_PROPERTIES As String = "Properties"

    'Definitions for Procsets
    Public Const PDF_PDF As String = "PDF"
    Public Const PDF_TEXT As String = "Text"
    Public Const PDF_IMAGEB As String = "ImageB"
    Public Const PDF_IMAGEC As String = "ImageC"
    Public Const PDF_IMAGEI As String = "ImageI"
    Public Const PDF_IMAGE As String = "Image"

    Public Const PDF_SUBTYPE As String = "Subtype"
    Public Const PDF_WIDTH As String = "Width"
    Public Const PDF_HEIGHT As String = "Height"
    Public Const PDF_BITS_PER_COMPONENT As String = "BitsPerComponent"
    Public Const PDF_INTENT As String = "Intent"
    Public Const PDF_IMAGEMASK As String = "ImageMask"
    Public Const PDF_MASK As String = "Mask"
    Public Const PDF_SMASK As String = "SMask"
    Public Const PDF_DECODE As String = "Decode"
    Public Const PDF_INTERPOLATE As String = "Interpolate"
    Public Const PDF_ALTERNATES As String = "Alternates"
    Public Const PDF_STRUCTPARENT As String = "StructParent"
    Public Const PDF_OPI As String = "OPI"
    Public Const PDF_METADATA As String = "Metadata"
    Public Const PDF_DECODEPARAMS As String = "DecodeParms"


    Public Const PDF_ASCII_HEX_DECODE As String = "ASCIIHexDecode"
    Public Const PDF_CCITT_85_DECODE As String = "ASCII85Decode"
    Public Const PDF_LZW_DECODE As String = "LZWDecode"
    Public Const PDF_FLATE_DECODE As String = "FlateDecode"
    Public Const PDF_RUN_LENGTH_DECODE As String = "RunLengthDecode"
    Public Const PDF_CCITT_FAX_DECODE As String = "CCITTFaxDecode"
    Public Const PDF_DCT_DECODE As String = "DCTDecode"

    Public Const PDF_ASCII_HEX_DECODE_ABREV As String = "AHx"
    Public Const PDF_CCITT_85_DECODE_ABREV As String = "A85"
    Public Const PDF_LZW_DECODE_ABREV As String = "LZW"
    Public Const PDF_FLATE_DECODE_ABREV As String = "Fl"
    Public Const PDF_RUN_LENGTH_DECODE_ABREV As String = "RL"
    Public Const PDF_CCITT_FAX_DECODE_ABREV As String = "CCF"
    Public Const PDF_DCT_DECODE_ABREV As String = "DCT"




    Public Enum PDFObjectType
        PDF_Boolean = 0
        PDF_Number
        PDF_String
        PDF_Name
        PDF_Array
        PDF_Dictionary
        PDF_Stream
        PDF_NULL
        PDF_IndirectReference
    End Enum


    Public Enum SubsectionEntryType
        InUse = 0
        Free
    End Enum
End Module
