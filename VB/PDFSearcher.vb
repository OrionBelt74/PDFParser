Imports System.Text.RegularExpressions
Imports System.Threading
Imports Tesseract

Friend Class PDFSearcher
    Private TESS_DATA_FOLDER As String = "D:\Tesseract-OCR\tessdata"
    Private _PDFList As List(Of String)
    Private _KeyWords As List(Of String)
    Private _OutputDirectory As String
    Private _StopSearch As Boolean
    'Dim docList As New List(Of FileInfo)({New FileInfo(DocumentPath)})
    Private _Replacements As New List(Of KeyValuePair(Of String, String))

    Private engine As TesseractEngine
    Private Regexes As New List(Of KeyValuePair(Of String, Regex))
    Public Event PDFDone(res As SearchResult)
    Public Event Trace(msg As String)
    Property OutputDirectory As String
        Set(value As String)
            _OutputDirectory = value
        End Set
        Get
            Return _OutputDirectory
        End Get
    End Property

    Property PDFList As List(Of String)
        Set(value As List(Of String))
            _PDFList = value
        End Set
        Get
            Return _PDFList
        End Get
    End Property
    Property KeyWords As List(Of String)
        Set(value As List(Of String))
            _KeyWords = value
        End Set
        Get
            Return _KeyWords
        End Get
    End Property

    Sub New()
        engine = New TesseractEngine(TESS_DATA_FOLDER, "spa", EngineMode.Default)
        PDFList = New List(Of String)
        KeyWords = New List(Of String)

        _Replacements.Add(New KeyValuePair(Of String, String)("á", "a"))
        _Replacements.Add(New KeyValuePair(Of String, String)("é", "e"))
        _Replacements.Add(New KeyValuePair(Of String, String)("í", "i"))
        _Replacements.Add(New KeyValuePair(Of String, String)("ó", "o"))
        _Replacements.Add(New KeyValuePair(Of String, String)("ú", "u"))
        _Replacements.Add(New KeyValuePair(Of String, String)("-" & vbCrLf, ""))
        _Replacements.Add(New KeyValuePair(Of String, String)("-" & vbCr, ""))
    End Sub
    Public Sub LoadPDFsFromFolder(folder As String)
        Dim Files() As String = IO.Directory.GetFiles(folder, "*.pdf")
        PDFList.Clear()

        For Each file As String In Files
            PDFList.Add(file)
        Next
    End Sub
    Public Sub ProcessPDFList()
        _StopSearch = False
        Dim procThread As New Thread(New ThreadStart(AddressOf ProcessPDFs))
        procThread.Start()
    End Sub

    Public Sub StopProcess()
        _StopSearch = True
    End Sub
    Public Sub ProcessPDFs()
        Dim pdfDoc As PDFDocument
        Dim res As SearchResult

        'Process the keywords and convert to regex
        Dim r As Regex
        For Each keyword As String In KeyWords
            If InStr(keyword, " ") = 0 Then
                'The OCR processing may generate wrong characters, so we cannot trust having the pattern starting with spaces
                'the search must include ONLY the pattern, we may find wrong matches but it is better than to lose good ones
                'r = New Regex("(\s+|^)(" & keyword & ")(\s+|$)", RegexOptions.Multiline + RegexOptions.IgnoreCase)
                r = New Regex("(" & keyword & ")", RegexOptions.Multiline + RegexOptions.IgnoreCase)
            Else
                Dim comps() As String = keyword.Split(" ")
                Dim words As New List(Of String)
                Dim regexPatt As String = ""
                For Each cpm As String In comps
                    If cpm <> "" Then
                        regexPatt = regexPatt & "(" & cpm & ")(\s*)" 'should be \s+ but since the OCR is not perfect, we try without space as well
                    End If
                Next
                If comps.Length > 0 Then
                    regexPatt = Mid(regexPatt, 1, Len(regexPatt) - 1)
                End If

                'The OCR processing may generate wrong characters, so we cannot trust having the pattern starting with spaces
                'the search must include ONLY the pattern, we may find wrong matches but it is better than to lose good ones
                'regexPatt = "(\s+|^)" & regexPatt & "|$)"
                regexPatt = regexPatt & "|$)"

                r = New Regex(regexPatt, RegexOptions.Multiline + RegexOptions.IgnoreCase)

            End If
            Regexes.Add(New KeyValuePair(Of String, Regex)(keyword, r))
        Next



        Dim mcol As MatchCollection
        Dim PDFStringB As New System.Text.StringBuilder

        For Each pdfPath As String In PDFList


            pdfDoc = New PDFDocument
            pdfDoc.OutputDirectory = OutputDirectory
            pdfDoc.Load(pdfPath)
            RaiseEvent Trace("Procesando '" & pdfDoc.Name & "'")

            Dim imgCnt As Integer = 0

            PDFStringB.Clear()


            Dim srchRes As New SearchResult
            srchRes.PDFName = pdfPath
            Dim txtFile As String = pdfDoc.NameNoExt & ".txt"

            If Not IO.File.Exists(OutputDirectory & "\" & txtFile) Then
                For Each imagePath As String In pdfDoc.Images
                    RaiseEvent Trace("Procesando imagen " & imgCnt + 1 & " de " & pdfDoc.Images.Count)
                    srchRes.NumTotalImages += 1
                    If Not _StopSearch Then
                        Dim img As Tesseract.Pix = Pix.LoadFromFile(OutputDirectory & "\" & imagePath)
                        Dim page As Tesseract.Page = engine.Process(img, PageSegMode.Auto)
                        Dim text As String = page.GetText()



                        PDFStringB.AppendLine("")

                        If text IsNot Nothing And text <> "" Then
                            For Each kvp As KeyValuePair(Of String, String) In _Replacements
                                text = Replace(text, kvp.Key, kvp.Value)
                            Next

                            PDFStringB.AppendLine("___ IMAGEN " & imgCnt + 1 & "____________________________")
                            PDFStringB.Append(text)

                            Dim meanConfidence As Single = page.GetMeanConfidence()

                            For Each kvp As KeyValuePair(Of String, Regex) In Regexes
                                mcol = kvp.Value.Matches(text)
                                If mcol.Count > 0 Then
                                    If Not srchRes.ImageResults.Keywords.ContainsKey(kvp.Key) Then
                                        srchRes.ImageResults.Keywords.Add(kvp.Key, New List(Of Tuple(Of Integer, Integer, Single)))
                                    End If

                                    srchRes.ImageResults.Keywords(kvp.Key).Add(New Tuple(Of Integer, Integer, Single)(imgCnt + 1, mcol.Count, meanConfidence))
                                End If
                                'For Each m As Match In mcol
                                '    srchRes.ImageResults.Keywords(kvp.Key).Add(New Tuple(Of Integer, Integer, Single)(imgCnt + 1, mcol.Count, meanConfidence))
                                'Next
                            Next
                        End If
                        text = ""
                        img.Dispose()
                        page.Dispose()
                        imgCnt += 1

                    End If
                Next
                IO.File.WriteAllText(OutputDirectory & "\" & txtFile, PDFStringB.ToString)
            End If
            pdfDoc.Close
            RaiseEvent PDFDone(srchRes)
        Next
    End Sub
End Class

Public Class ImageResults
    Private _Keywords As Dictionary(Of String, List(Of Tuple(Of Integer, Integer, Single)))  'keywords, list of (page number, number of times the keyword was found), meanconfidence
    Property Keywords As Dictionary(Of String, List(Of Tuple(Of Integer, Integer, Single)))
        Set(value As Dictionary(Of String, List(Of Tuple(Of Integer, Integer, Single))))
            _Keywords = value
        End Set
        Get
            Return _Keywords
        End Get
    End Property
    Sub New()
        Keywords = New Dictionary(Of String, List(Of Tuple(Of Integer, Integer, Single)))
    End Sub
End Class
Friend Class SearchResult
    Private _PDFName As String
    Private _NumTotalImages As Integer
    Private _ImageResults As ImageResults
    Property NumTotalImages As Integer
        Set(value As Integer)
            _NumTotalImages = value
        End Set
        Get
            Return _NumTotalImages
        End Get
    End Property
    Property PDFName As String
        Set(value As String)
            _PDFName = value
        End Set
        Get
            Return _PDFName
        End Get
    End Property
    Property ImageResults As ImageResults
        Set(value As ImageResults)
            _ImageResults = value
        End Set
        Get
            Return _ImageResults
        End Get
    End Property

    Sub New()
        ImageResults = New ImageResults
        NumTotalImages = 0
    End Sub
End Class