Imports System.Text.RegularExpressions

Public Class PDFCommandParser
    Dim cmdRegex As New Regex("(.*?)(w|J|j|M|d|ri|i|gs|q|Q|cm|m|l|c|v|y|h|re|S|s|f|F|f*|B|B*|b|b*|n|W|W*|BT|ET|Tc|Tw|z|TL|Tf|Tr|Ts|Do)(\s*)$", RegexOptions.Multiline)
    Dim commands As New List(Of String)
    Dim PageResourcesDict As PDFObject
    Dim cmdDoRegex As New Regex("(.*?)(Do)$", RegexOptions.Multiline)
    Public Sub Clear()

    End Sub

    Public Sub Parse(str As String)
        Dim mcol As MatchCollection
        mcol = cmdRegex.Matches(str)

        commands.Clear()

        For Each m As Match In mcol
            Dim cmdLine As String = Replace(m.Value, vbCrLf, "")
            cmdLine = Replace(cmdLine, vbCr, "")
            cmdLine = Replace(cmdLine, vbLf, "")
            If cmdLine <> "" Then
                commands.Add(cmdLine)
            End If

        Next

        Execute

    End Sub
    Private Sub Execute()
        For Each cmd As String In commands
            If cmdDoRegex.IsMatch(cmd) Then
                Dim cmdLine As String = cmd
                cmdLine = Replace(cmdLine, " Do", "").Trim
                cmdLine = Replace(cmdLine, "/", "")
                Dim ImageName = cmdLine

                'Search for the image name in the resource dictionary
                If PageResourcesDict.ContainsKey("XObject") Then
                    Dim xobjectObj As PDFObject = PageResourcesDict.GetDictValue("XObject")

                    If xobjectObj.ContainsKey(ImageName) Then
                        Dim imgObj As PDFObject = xobjectObj.GetDictValue(ImageName)

                    End If
                End If


            End If
        Next
    End Sub
    Friend Sub SetPageResourceDict(ResourcesDict As PDFObject)
        PageResourcesDict = ResourcesDict
    End Sub

End Class
