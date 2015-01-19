Option Strict Off
Option Explicit On

Public Class myFileDialog

    'Notice: Don't forget to set the OwnerHwnd property to the handle of the calling window in order to bind the dialog
    'to the calling window.

    '//The Win32 API Functions///
    Private Declare Function GetSaveFileName Lib "comdlg32.dll" Alias "GetSaveFileNameA" (ByRef pOpenfilename As OPENFILENAME) As Integer
    Private Declare Function GetOpenFileName Lib "comdlg32.dll" Alias "GetOpenFileNameA" (ByRef pOpenfilename As OPENFILENAME) As Integer

    '//A few of the available Flags///
    Private Const OFN_HIDEREADONLY As Integer = &H4
    Private Const OFN_ALLOWMULTISELECT As Integer = &H200
    Private Const OFN_EXPLORER As Integer = &H80000

    '//The Structure

    Private Structure OPENFILENAME
        Dim lStructSize As Integer
        Dim hwndOwner As Integer
        Dim hInstance As Integer
        Dim lpstrFilter As String
        Dim lpstrCustomFilter As String
        Dim nMaxCustFilter As Integer
        Dim nFilterIndex As Integer
        Dim lpstrFile As String
        Dim nMaxFile As Integer
        Dim lpstrFileTitle As String
        Dim nMaxFileTitle As Integer
        Dim lpstrInitialDir As String
        Dim lpstrTitle As String
        Dim Flags As Integer
        Dim nFileOffset As Short
        Dim nFileExtension As Short
        Dim lpstrDefExt As String
        Dim lCustData As Integer
        Dim lpfnHook As Integer
        Dim lpTemplateName As String
    End Structure

    Private lngHwnd As Integer
    Private strFilter As String
    Private strTitle As String
    Private strDir As String
    Private strFile As String 'elj
    Private lngSelectedFilter As Integer
    Private blnHideReadOnly As Boolean

    Private Sub FileDialog_Initialize()
        'Set default values when class is first created
        strDir = CurDir()
        strTitle = ""
        strFile = ""
        strFilter = "All Files" & Chr(0) & "*.*" & Chr(0)
        lngSelectedFilter = 0
        lngHwnd = &O0S 'Desktop
    End Sub

    Public Sub New()
        MyBase.New()
        FileDialog_Initialize()
    End Sub

    Public Property OwnerHwnd() As Integer
        Get
            OwnerHwnd = lngHwnd
        End Get
        Set(ByVal Value As Integer)
            '//FOR YOU TO DO//
            'Use the API to validate this handle
            lngHwnd = Value
        End Set
    End Property

    Public Property SelectedFilter() As Integer
        Get
            SelectedFilter = lngSelectedFilter
        End Get
        Set(ByVal Value As Integer)
            lngSelectedFilter = Value
        End Set
    End Property

    Public Property StartFile() As String
        Get
            StartFile = strFile
        End Get
        Set(ByVal Value As String)
            If Not Value = vbNullString Then
                strFile = Value
            End If
        End Set
    End Property

    Public Property StartInDir() As String
        Get
            StartInDir = strDir
        End Get
        Set(ByVal Value As String)
            If Not Value = vbNullString Then
                strDir = Value
            End If
        End Set
    End Property

    Public Property Title() As String
        Get
            Title = strTitle
        End Get
        Set(ByVal Value As String)
            If Not Value = vbNullString Then
                strTitle = Value
            End If
        End Set
    End Property

    Public Property Filter() As String
        Get
            'Here we reverse the process and return the Filter in the same format that it was entered
            Dim intPos As Short
            Dim strTemp As String
            strTemp = strFilter
            Do While InStr(strTemp, Chr(0)) > 0
                intPos = InStr(strTemp, Chr(0))
                If intPos > 0 Then
                    strTemp = Left(strTemp, intPos - 1) & "|" & Right(strTemp, Len(strTemp) - intPos)
                End If
            Loop
            If Right(strTemp, 1) = "|" Then
                strTemp = Left(strTemp, Len(strTemp) - 1)
            End If
            Filter = strTemp
        End Get
        Set(ByVal Value As String)
            'Filters change the type of files that are displayed in the dialog. I have designed this
            'validation to use the same filter format the Common dialog OCX uses: "All Files (*.*) | *.*"
            Dim intPos As Short
            Do While InStr(Value, "|") > 0
                intPos = InStr(Value, "|")
                If intPos > 0 Then
                    Value = Left(Value, intPos - 1) & Chr(0) & Right(Value, Len(Value) - intPos)
                End If
            Loop
            If Right(Value, 2) <> Chr(0) & Chr(0) Then
                Value = Value & Chr(0)
            End If
            strFilter = Value
        End Set
    End Property

    Public Property HideReadOnly() As Boolean
        Get
            HideReadOnly = blnHideReadOnly
        End Get
        Set(ByVal Value As Boolean)
            blnHideReadOnly = Value
        End Set
    End Property

    '@~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~@
    ' Display and use the File Save dialog
    '@~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~@
    Public Function ShowSave() As String
        Dim strTemp As String = ""
        Dim udtStruct As OPENFILENAME = New OPENFILENAME()
        udtStruct.lStructSize = Len(udtStruct)
        udtStruct.hwndOwner = lngHwnd
        udtStruct.lpstrFilter = strFilter
        If Not strFile = vbNullString Then
            udtStruct.lpstrFile = strFile & Space(254 - Len(strFile))
        Else
            udtStruct.lpstrFile = Space(254)
        End If
        udtStruct.nMaxFile = 255
        udtStruct.lpstrFileTitle = Space(254)
        udtStruct.nMaxFileTitle = 255
        udtStruct.lpstrInitialDir = strDir
        udtStruct.lpstrTitle = strTitle
        If blnHideReadOnly Then
            udtStruct.Flags = OFN_HIDEREADONLY
        Else
            udtStruct.Flags = 0
        End If
        If GetSaveFileName(udtStruct) Then
            strTemp = (Trim(udtStruct.lpstrFile))
            ShowSave = Mid(strTemp, 1, Len(strTemp) - 1)
        Else
            ShowSave = Nothing
        End If

    End Function

    '@~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~@
    ' Display and use the File open dialog
    '@~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~@
    Public Function ShowOpen() As Object
        Dim strTemp As String = ""
        Dim udtStruct As OPENFILENAME = New OPENFILENAME()
        Dim lngFlags As Integer = 0
        Dim intFileOffset As Short = 0

        udtStruct.lStructSize = Len(udtStruct)
        udtStruct.hwndOwner = lngHwnd
        udtStruct.lpstrFilter = strFilter
        udtStruct.nFilterIndex = lngSelectedFilter

        If Not strFile = vbNullString Then
            udtStruct.lpstrFile = strFile & Space(254 - Len(strFile))
        Else
            udtStruct.lpstrFile = Space(254)
        End If

        udtStruct.nMaxFile = 255
        udtStruct.lpstrFileTitle = Space(254)
        udtStruct.nMaxFileTitle = 255
        udtStruct.lpstrInitialDir = strDir
        udtStruct.lpstrTitle = strTitle
        If blnHideReadOnly Then
            lngFlags = OFN_HIDEREADONLY
        Else
            lngFlags = 0
        End If
        lngFlags = lngFlags + OFN_ALLOWMULTISELECT + OFN_EXPLORER
        udtStruct.Flags = lngFlags

        If GetOpenFileName(udtStruct) Then

            strTemp = (Trim(udtStruct.lpstrFile))
            'strTemp = Mid(strTemp, 1, Len(strTemp) - 1)
            intFileOffset = udtStruct.nFileOffset
            strTemp = Left(strTemp, intFileOffset - 1) & Chr(0) & Right(strTemp, Len(strTemp) - intFileOffset)

            ShowOpen = str2var(strTemp)

        Else

            ShowOpen = Nothing

        End If

    End Function

    Private Function str2var(ByVal strTemp As String) As Object
        Dim intPos As Short
        Dim i As Short
        Dim strRetval As String
        Dim strArray() As String

        strRetval = strTemp
        i = 0
        Do
            intPos = InStr(strRetval, Chr(0)) 'position of target
            Debug.Print(intPos)
            If intPos > 0 Then
                ReDim Preserve strArray(i)
                strArray(i) = Left(strRetval, intPos - 1)
                strRetval = Right(strRetval, Len(strRetval) - intPos)
            Else
                ReDim Preserve strArray(i)
                strArray(i) = strRetval
                Exit Do
            End If
            i = i + 1
        Loop

        str2var = strArray

    End Function

End Class