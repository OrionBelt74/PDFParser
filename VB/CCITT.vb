Module CCITT
    Public TERMINATING_CODES_WHITERUN_CTRL() As Byte = {
        &H35,
        &H1C,
        &H70,
        &H80,
        &HB0,
        &HC0,
        &HE0,
        &HF0,
        &H98, '8
        &HA0, '9
        &HC8, '10
        &H38, '11
        &H20,
        &HC, '13
        &HD0, '14
        &HD4, '15
        &HA8,
        &HAC,
        &H4E, '18
        &H18,
        &H10, '20
        &H2E, '21
        &H6,
        &H8, '23
        &H50, '24
        &H56, '25
        &H26, '26
        &H48, '27
        &H30, '28
        &H2, '29
        &H3, '30
        &H1A,
        &H1B,
        &H12,
        &H13,
        &H14, '35
        &H15,
        &H16,
        &H17,
        &H28,
        &H29, '40
        &H2A,
        &H2B,
        &H2C,
        &H2D,
        &H4, '45
        &H5,
        &HA,
        &HB,
        &H52,
        &H53, '50
        &H54,
        &H55,
        &H24,
        &H25,
        &H58, '55
        &H59,
        &H5A,
        &H5B,
        &H4A,
        &H4B, '60
        &H32,
        &H33,
        &H34}


    Public TERMINATING_CODES_BLACKRUN_CTRL() As Byte = {
        &HDF, '0 --> remove the 4 leading zeroes and extend with ones on the right
        &H5F,
        &HFF,
        &HBF,
        &H7F,
        &H3F, '5
        &H2F,
        &H1F,
        &H17,
        &H13,
        &H9, '10 
        &HB,
        &HF,
        &H4,
        &H7,
        &HC0, '15 --> From here, remove the 4 leading zeroes and extend with ones on the right
        &H5C,
        &H60,
        &H20,
        &HCE,
        &HD0, '20
        &HD8,
        &H6E,
        &H50,
        &H2E,
        &H30, '25
        &HCA,
        &HCB,
        &HCC,
        &HCD,
        &H68, '30
        &H69,
        &H6A,
        &H6B,
        &HD2,
        &HD3, '35
        &HD4,
        &HD5,
        &HD6,
        &HD7,
        &H6C, '40
        &H6D,
        &HDA,
        &HDB,
        &H54,
        &H55, '45
        &H56,
        &H57,
        &H64,
        &H65,
        &H52, '50
        &H53,
        &H24,
        &H37,
        &H38,
        &H27, '55
        &H28,
        &H58,
        &H59,
        &H2B,
        &H2C, '60
        &H5A,
        &H66,
        &H67}



    Public TERMINATING_CODES_WHITERUN(255) As Integer
    Public TERMINATING_CODES_BLACKRUN(255) As Integer
End Module
