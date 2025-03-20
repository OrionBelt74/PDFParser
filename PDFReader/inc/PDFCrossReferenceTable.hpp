#ifndef PDF_CROSS_REFERENCE_TABLE_HPP__
#define PDF_CROSS_REFERENCE_TABLE_HPP__

class PDFCrossReferenceTable
{


    Public Sections As List(Of PDFCrossReferenceSection)

    Sub New()
        Sections = New List(Of PDFCrossReferenceSection)
    End Sub

    Public Function GetObjectPos(indRef As PDFObject) As Integer
        Dim res As Integer = -1
        Dim objNum As Integer = indRef.IndirectReference.Item1
        Dim genNum As Integer = indRef.IndirectReference.Item2

        For Each s As PDFCrossReferenceSection In Sections
            For Each ss As PDFSubsection In s.Subsections
                For Each e As CrossReferenceEntry In ss.Entries
                    If e.ObjectNumber = objNum And e.GenerationNumber = genNum And e.Type = SubsectionEntryType.InUse Then
                        Return e.ByteOffset
                    End If
                Next
            Next
        Next

        Return res
    End Function

};
#endif