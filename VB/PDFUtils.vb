Imports System.Text.RegularExpressions

Module PDFUtils

    Public Const REGEX_PDF_NAME As String = "/[a-zA-Z0-9#_]+"

    Public IndirectReferenceRegex As New Regex("\d+\s\d+\sR")

    Public Const REGEX_PDF_ARRAY As String = "/[a-zA-Z0-9#_]*?"
    Public NameRegex As New Regex(REGEX_PDF_NAME)



    Public Const REGEXP_BALANCED_GROUP As String = "{[^{}]*" & "(((?'Open'{)[^{}]*)+((?'Close-Open'})[^{}]*)+" + ")*(?(Open)(?!))}"

    Public Const REGEXP_BALANCED_GROUP_PDFDICT As String = "<<[^<<>>]*" & "(((?'Open'<<)[^<<>>]*)+((?'Close-Open'>>)[^<<>>]*)+" + ")*(?(Open)(?!))>>"
    Public DictRegex As New Regex(REGEXP_BALANCED_GROUP_PDFDICT)
    Public Const REGEXP_BALANCED_GROUP_PDFARRAY As String = "\[[^\[\]]*" & "(((?'Open'\[)[^\[\]]*)+((?'Close-Open'\])[^\[\]]*)+" + ")*(?(Open)(?!))\]"

    Public Const REGEXP_BALANCED_GROUP_HEX_DATA As String = "<[^<>]*" & "(((?'Open'<)[^<>]*)+((?'Close-Open'>)[^<>]*)+" + ")*(?(Open)(?!))>"

    Public ArrayRegex As New Regex(REGEXP_BALANCED_GROUP_PDFARRAY)
    Public HexDataRegex As New Regex(REGEXP_BALANCED_GROUP_HEX_DATA)

    Public BooleanRegex As New Regex("(true|false)")

    Public NumericRegex = New Regex("(\-)*\d+(\.\d+)*")
End Module
