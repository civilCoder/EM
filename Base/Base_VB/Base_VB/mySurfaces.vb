Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic

Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry

Imports Autodesk.AutoCAD.Interop.Common
Imports Autodesk.AECC.Interop.Land

Public Class mySurfaces

    Public Shared Function getAeccBoundaries(strName As String) As AcadEntity
        Dim ent As AcadEntity = Nothing
        Dim aeccSurfs As AeccSurfaces = BaseObjsCom.aeccDoc.Surfaces
        Dim boundaries As AeccSurfaceBoundaries = Nothing
        For Each aeccTinSurf As AeccTinSurface In aeccSurfs
            If aeccTinSurf.Name = strName Then
                boundaries = aeccTinSurf.Boundaries
                For Each boundary As AeccSurfaceBoundary In boundaries
                    If boundary.Type = AeccBoundaryType.aeccBoundaryOuter Then
                        ent = boundary.BoundaryEntity
                    End If
                Next
                Exit For
            End If
        Next
        Return ent
    End Function

    Public Shared Function getOuterBoundary(strName As String) As Point3dCollection
        Dim pnts3d As New Point3dCollection()
        Dim ent As AcadEntity = Nothing
        Dim aeccSurfs As AeccSurfaces = BaseObjsCom.aeccDoc.Surfaces
        Dim boundaries As AeccSurfaceBoundaries = Nothing
        For Each aeccTinSurf As AeccTinSurface In aeccSurfs
            If aeccTinSurf.Name = strName Then
                Try
                    boundaries = aeccTinSurf.Boundaries
                    If boundaries IsNot Nothing Then
                        For i As Integer = 0 To boundaries.Count - 1
                            Dim boundary As AeccSurfaceBoundary = boundaries.Item(i)
                            If boundary.Type = AeccBoundaryType.aeccBoundaryOuter Then
                                ent = boundary.BoundaryEntity
                                Select Case ent.ObjectName
                                    Case "AcDbPolyline"
                                        Dim lwPoly As AcadLWPolyline = DirectCast(ent, AcadLWPolyline)
                                        pnts3d = lwPoly.lwPolyToPoint3dCollection()
                                        boundaries.Remove(i, AeccDeleteDefinitionFromType.aeccDeleteFromDrawingAndSurface)
                                        Exit Select

                                    Case "AcDb3dPolyline"

                                        Exit Select
                                End Select
                            End If
                        Next
                    End If
                Catch ex As System.Exception
                End Try
            End If
        Next
        Return pnts3d
    End Function


    Public Shared Function getAeccTinSurface(strName As String, ByRef exists As Boolean) As AeccTinSurface
        exists = False
        Dim surf As AeccTinSurface = Nothing
        Dim aeccSurfs As AeccSurfaces = BaseObjsCom.aeccDoc.Surfaces
        For Each aeccTinSurf As AeccTinSurface In aeccSurfs
            If aeccTinSurf.Name = strName Then
                exists = True
                surf = aeccTinSurf
            End If
        Next
        Return surf
    End Function

    Public Shared Sub addBreakLines2Surface(Optional ByRef acad3dPolys As Object = Nothing, Optional ByRef nameSurface As Object = Nothing)

        Dim objSurface As Autodesk.AECC.Interop.Land.AeccTinSurface = Nothing

        Dim objSurfaceBreaklines As AeccSurfaceBreaklines = Nothing

        Dim objEntityArray() As AcadEntity = Nothing
        Dim obj3dPoly As Acad3DPolyline = Nothing

        Dim strDwgType As String = ""
        Dim strDwgName As String = ""

        Dim i As Object
        Dim k As Short

        If IsNothing(acad3dPolys) Then 'select 3dPolys to add to surface

            objEntityArray = myUtility.getBrklines

            If objEntityArray.Count = 0 Then
                Exit Sub
            End If

        Else 'add 3dPolys passed to surface

            k = UBound(acad3dPolys)

            For i = 0 To k
                obj3dPoly = acad3dPolys(i)
                ReDim Preserve objEntityArray(i)

                objEntityArray(i) = obj3dPoly
            Next i

        End If

        Dim exists As Boolean = False
        If IsNothing(nameSurface) Then

            strDwgName = myDrawing.acadDoc.Name
            If InStr(5, strDwgName, "CGP", CompareMethod.Text) Then
                strDwgType = "CGP"
            ElseIf InStr(5, strDwgName, "GCAL", CompareMethod.Text) Then
                strDwgType = "GCAL"
            ElseIf InStr(5, strDwgName, "MASS", CompareMethod.Text) Then
                strDwgType = "MASS"
            ElseIf InStr(5, strDwgName, "CONT", CompareMethod.Text) Then
                strDwgType = "CONT"
            End If

            Select Case strDwgType
                Case "CGP", "GCAL", "MASS"
                    objSurface = mySurfaces.getAeccTinSurface("CPNT-ON", exists)
                Case "CONT"
                    objSurface = mySurfaces.getAeccTinSurface("EXIST", exists)
            End Select

        Else

            objSurface = mySurfaces.getAeccTinSurface(nameSurface, exists)

        End If
        objSurfaceBreaklines = objSurface.Breaklines

        Dim strBrkLineName As String
        strBrkLineName = getBrkLineName(objSurfaceBreaklines)

        objSurfaceBreaklines.AddStandardBreakline(objEntityArray, strBrkLineName, 1.0#)

    End Sub

    Public Shared Function cleanUpBreaklines() As String       'PENDING UPGRADE TO 2014 - SURFACES NOT INCLUDED IN 2010 MANAGED API

        Dim objSurface As AeccTinSurface = Nothing

        Dim objSurfaceBreaklines As AeccSurfaceBreaklines

        Dim objAcadEnts() As AcadEntity = Nothing

        Dim strDesc As String = String.Empty
        Dim strHandleBrkLine As String = String.Empty

        Dim k As Short
        Dim exists As Boolean = False

        Try
            objSurface = mySurfaces.getAeccTinSurface("CPNT-ON", exists)
        Catch ex As Exception
        End Try

        If (objSurface Is Nothing) Then
            strHandleBrkLine = "0000"
            Return strHandleBrkLine
        End If

        objSurfaceBreaklines = objSurface.Breaklines

        k = objSurfaceBreaklines.Count

        If k = 0 Then
            strHandleBrkLine = "0000"
            Return strHandleBrkLine
        Else
            Call deleteBreaklinesInSurface()
        End If

        objAcadEnts = myUtility.getBrklines

        If objAcadEnts.Count > 0 Then

            strDesc = "BRK-000"
            objSurfaceBreaklines.AddStandardBreakline(objAcadEnts, strDesc, 1.0#)

            objSurface.Rebuild()

        End If

        Return myUtility.lastBrkline

    End Function

    Public Shared Sub deleteBreaklinesInSurface()

        Dim objSurface As AeccTinSurface

        Dim objSurfaceBreaklines As AeccSurfaceBreaklines

        Dim i As Short

        Dim boolFound As Boolean = False

        Dim exists As Boolean
        objSurface = mySurfaces.getAeccTinSurface("CPNT-ON", exists)

        Do

            objSurfaceBreaklines = Nothing
            objSurfaceBreaklines = objSurface.Breaklines
            boolFound = False

            For i = 0 To objSurfaceBreaklines.Count - 1

                objSurfaceBreaklines.Remove((i))
                boolFound = True
                Exit For

            Next i

        Loop While boolFound = True

    End Sub

    Public Shared Sub removeBreaklineswith0vertices()

        Dim objSurface As AeccTinSurface

        Dim objSurfaceBreaklines As AeccSurfaceBreaklines
        Dim objSurfaceBreakline As AeccSurfaceBreakline

        Dim obj3dPoly As Acad3DPolyline

        Dim varBreaklineEnts As Object

        Dim i As Short
        Dim j As Short

        Dim boolFound As Boolean
        Dim exists As Boolean

        objSurface = mySurfaces.getAeccTinSurface("CPNT-ON", exists)

        Do

            objSurfaceBreaklines = Nothing
            objSurfaceBreaklines = objSurface.Breaklines

            boolFound = False

            For i = 0 To objSurfaceBreaklines.Count - 1                     'BREAKLINE COUNT BASED ON UNIQUE NAME OF BREAKLINES

                objSurfaceBreakline = objSurfaceBreaklines.Item(i)

                varBreaklineEnts = objSurfaceBreakline.BreaklineEntities

                For j = 0 To UBound(varBreaklineEnts)

                    obj3dPoly = varBreaklineEnts(j)

                    If UBound(obj3dPoly.Coordinates) = 0 Then
                        obj3dPoly.Delete()
                        '        boolFound = True
                    End If

                Next j

                '      Exit For

            Next i

        Loop While boolFound = True

    End Sub

    Public Shared Function getBrkLineName(ByRef objSurfaceBreaklines As AeccSurfaceBreaklines) As String

        Dim objSurfaceBreakline As AeccSurfaceBreakline

        Dim strBrkLineNums() As String = Nothing
        Dim strBrkLineNum As String = String.Empty
        Dim intBrkLineNum As Integer = 0

        Dim i, j, k As Short

        Select Case objSurfaceBreaklines.Count

            Case 0

                getBrkLineName = "BRK-0"

            Case 1

                objSurfaceBreakline = objSurfaceBreaklines.Item(0)

                j = InStr(1, objSurfaceBreakline.Description, "BRK-", CompareMethod.Text)

                If j = 0 Then

                    getBrkLineName = "BRK-0"
                    Exit Function

                Else

                    strBrkLineNum = Mid(objSurfaceBreakline.Description, 5)
                    intBrkLineNum = System.Convert.ToInt16(strBrkLineNum) + 1
                    If intBrkLineNum < 10 Then
                        strBrkLineNum = "0" & System.Convert.ToString(intBrkLineNum)
                    Else
                        strBrkLineNum = System.Convert.ToString(intBrkLineNum)
                    End If
                    getBrkLineName = "BRK-" & strBrkLineNum
                    Exit Function

                End If

            Case Else

                k = -1
                For i = 0 To objSurfaceBreaklines.Count - 1

                    objSurfaceBreakline = objSurfaceBreaklines.Item(i)

                    j = InStr(1, objSurfaceBreakline.Description, "BRK-", CompareMethod.Text)

                    If j <> 0 Then

                        k = k + 1
                        ReDim Preserve strBrkLineNums(k)
                        strBrkLineNums(k) = Mid(objSurfaceBreakline.Description, 5)

                    End If

                Next i

                If k = -1 Then

                    getBrkLineName = "BRK-0"

                Else

                    strBrkLineNum = strBrkLineNums(k)
                    intBrkLineNum = System.Convert.ToInt16(strBrkLineNum) + 1
                    If intBrkLineNum < 10 Then
                        strBrkLineNum = "0" & System.Convert.ToString(intBrkLineNum)
                    Else
                        strBrkLineNum = System.Convert.ToString(intBrkLineNum)
                    End If
                    getBrkLineName = "BRK-" & strBrkLineNum

                End If

        End Select

    End Function

    Public Shared Function initializeBrklines() As Handle

        Dim objSurface As AeccTinSurface = Nothing
        Dim objSurfaceBreaklines As AeccSurfaceBreaklines

        Dim objAcadEnts() As AcadEntity = Nothing

        Dim strDescription As String = String.Empty
        Dim exists As Boolean = False

        objSurface = mySurfaces.getAeccTinSurface("CPNT-ON", exists)
        objSurfaceBreaklines = objSurface.Breaklines

        objAcadEnts = myUtility.getBrklines

        Dim k As Integer = objAcadEnts.Count - 1

        Dim strHandles As List(Of String) = New List(Of String)()

        For i As Integer = 0 To k
            strHandles.Add(objAcadEnts(i).Handle)
        Next i

        strDescription = "BRK-0"
        objSurfaceBreaklines.AddStandardBreakline(objAcadEnts, strDescription, 1.0#)

        Dim lngHandles(k) As Handle

        For i = 0 To k
            lngHandles(i) = myUtility.stringToHandle(strHandles(i))
        Next i
        Dim kvpair As KeyValuePair(Of Integer, Long) = myUtility.getLastHandle(strHandles)

        Dim ln As Long = kvpair.Value

        Return New Handle(ln)

    End Function

End Class