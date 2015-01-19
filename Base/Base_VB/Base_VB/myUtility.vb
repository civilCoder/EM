' (C) Copyright 2014 by
Imports System
Imports System.Linq

Imports Ent = Autodesk.AutoCAD.DatabaseServices.Entity

Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.Runtime

Imports Autodesk.AutoCAD.Interop.Common
Imports Autodesk.AECC.Interop.Land

Imports Autodesk.Civil.DatabaseServices
Imports Autodesk.Civil.DatabaseServices.Styles

Imports Autodesk.AECC.Interop.Survey

Public Class BrkLineData
    Private m_id As ObjectId
    Public Property id() As ObjectId
        Get
            Return m_id
        End Get
        Set(value As ObjectId)
            m_id = Value
        End Set
    End Property

    Private m_Desc As String
    Public Property Desc() As String
        Get
            Return m_Desc
        End Get
        Set(value As String)
            m_Desc = Value
        End Set
    End Property

    Private m_Handle As Handle
    Public Property Handle() As Handle
        Get
            Return m_Handle
        End Get
        Set(value As Handle)
            m_Handle = value
        End Set
    End Property
End Class


Public Class myUtility

    Public Shared Function stringToHandle(strHandle As [String]) As Handle
        Dim handle As New Handle()

        Try
            Dim nHandle As Int64 = Convert.ToInt64(strHandle, 16)
            handle = New Handle(nHandle)
        Catch ex As System.Exception
        End Try

        Return handle
    End Function

    Public Shared Function getLastHandle(sHandles As List(Of String)) As KeyValuePair(Of Integer, Long)
        Dim lnH As New SortedDictionary(Of Integer, Long)()
        For i As Integer = 0 To sHandles.Count - 1
            lnH.Add(i, System.Convert.ToInt64(sHandles(i), 16))
        Next

        Return lnH.Last()
    End Function

    Public Shared Sub tvsToList(tvs() As TypedValue, ByRef type() As Short, ByRef val() As Object)
        Dim intXDataType(tvs.Count - 1) As Short
        Dim varXDataIn(tvs.Count - 1) As Object
        For i As Integer = 0 To tvs.Count - 1
            intXDataType(i) = tvs(i).TypeCode
            varXDataIn(i) = tvs(i).Value
        Next

        type = intXDataType
        val = varXDataIn

    End Sub

    Public Shared Function comXDataToList(varVal As Object) As List(Of Handle)
        Dim h As List(Of Handle) = New List(Of Handle)

        Dim h1 As Handle = stringToHandle(varVal(1).ToString())
        Dim h2 As Handle = stringToHandle(varVal(2).ToString())
        h.Add(h1)
        h.Add(h2)

        Return h
    End Function

    Public Shared Function getXrec0(nameDict As String) As String

        Dim strHandle As String = String.Empty

        Try
            Dim objDict As AcadDictionary = myDrawing.acadDoc.Dictionaries.Item(nameDict)
            If objDict.Count > 0 Then
                Dim objXrec As AcadXRecord = objDict.Item(0)
                strHandle = objXrec.Name
            End If
        Catch ex As Autodesk.AutoCAD.Runtime.Exception

        End Try

        Return strHandle

    End Function

    Public Shared Function getBrklines() As AcadEntity()

        Dim objAcadEnts() As AcadEntity = Nothing

        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor
        Dim tvs(1) As TypedValue
        tvs(0) = New TypedValue(DxfCode.Start, "POLYLINE")
        tvs(1) = New TypedValue(DxfCode.LayerName, "CPNT-BRKLINE")

        Dim filter As New SelectionFilter(tvs)
        Dim psr As PromptSelectionResult = ed.SelectAll(filter)
        Dim ss As SelectionSet = Nothing
        If psr.Status = PromptStatus.OK Then
            ss = psr.Value
        End If

        Dim j As Integer = -1
        For i As Integer = 0 To ss.Count - 1
            j = j + 1
            ReDim Preserve objAcadEnts(j)
            objAcadEnts(j) = ss.Item(i)
        Next i

        Return objAcadEnts

    End Function

    Public Shared Function lastBrkline() As String

        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor
        Dim tvs(1) As TypedValue
        tvs(0) = New TypedValue(DxfCode.Start, "POLYLINE")
        tvs(1) = New TypedValue(DxfCode.LayerName, "CPNT-BRKLINE")

        Dim filter As New SelectionFilter(tvs)
        Dim psr As PromptSelectionResult = ed.SelectAll(filter)
        Dim ss As SelectionSet = Nothing
        If psr.Status = PromptStatus.OK Then
            ss = psr.Value
        End If

        Dim id As ObjectId = ss.Item(0).ObjectId
        Dim dbObj As Autodesk.AutoCAD.DatabaseServices.DBObject = Nothing
        Using tr As Transaction = HostApplicationServices.WorkingDatabase.TransactionManager.StartOpenCloseTransaction()
            dbObj = tr.GetObject(id, OpenMode.ForRead)
        End Using
        If dbObj Is Nothing Then
            Return ""
        Else
            Return dbObj.Handle.ToString()
        End If


    End Function

    Public Function CreatePipeNetwork() As Boolean
        Dim tr As Transaction = HostApplicationServices.WorkingDatabase.TransactionManager.StartOpenCloseTransaction()
        Dim oPipeNetworkIds As ObjectIdCollection
        Dim oNetworkId As ObjectId
        Dim oNetwork As Network
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor
        Dim civDoc As Autodesk.Civil.ApplicationServices.CivilDocument = Autodesk.Civil.ApplicationServices.CivilApplication.ActiveDocument
        oNetworkId = Network.Create(civDoc, "test")
        ' get the network
        Try
            oNetwork = tr.GetObject(oNetworkId, OpenMode.ForWrite)
        Catch
            CreatePipeNetwork = False
            Exit Function
        End Try
        '
        'Add pipe and Structure
        ' Get the Networks collections
        oPipeNetworkIds = civDoc.GetPipeNetworkIds()
        If (oPipeNetworkIds Is Nothing) Then
            MsgBox("There is no PipeNetwork Collection." + Convert.ToChar(10))
            ed.WriteMessage("There is no PipeNetwork Collection." + Convert.ToChar(10))
            CreatePipeNetwork = False
            Exit Function
        End If

        Dim oPartsListId As ObjectId = civDoc.Styles.PartsListSet("Standard") 'Standard PartsList
        Dim oPartsList As PartsList = tr.GetObject(oPartsListId, OpenMode.ForWrite)

        Dim oidPipe As ObjectId = oPartsList("Concrete Pipe SI")
        Dim opfPipe As PartFamily = tr.GetObject(oidPipe, OpenMode.ForWrite)

        Dim psizePipe As ObjectId = opfPipe(0)

        Dim line As LineSegment3d = New LineSegment3d(New Point3d(30, 9, 0), New Point3d(33, 7, 0))

        Dim oidNewPipe As ObjectId = ObjectId.Null
        oNetwork.AddLinePipe(oidPipe, psizePipe, line, oidNewPipe, True)

        Dim oidStructure As ObjectId = oPartsList("CMP Rectangular End Section SI")
        Dim opfStructure As PartFamily = tr.GetObject(oidStructure, OpenMode.ForWrite)

        Dim psizeStructure As ObjectId = opfStructure(0)

        Dim startPoint As Point3d = New Point3d(30, 9, 0)
        Dim endPoint As Point3d = New Point3d(33, 7, 0)

        Dim oidNewStructure As ObjectId = ObjectId.Null

        oNetwork.AddStructure(oidStructure, psizeStructure, startPoint, 0, oidNewStructure, True)
        oNetwork.AddStructure(oidStructure, psizeStructure, endPoint, 0, oidNewStructure, True)

        ed.WriteMessage("PipeNetwork created" + Convert.ToChar(10))
        tr.Commit()
        CreatePipeNetwork = True
    End Function ' CreatePipeNetwork

    Public Shared Sub addAeccBreaklines(aeccSurf As AeccTinSurface, ids As ObjectIdCollection, desc As String)
        Try
            Using tr As Transaction = HostApplicationServices.WorkingDatabase.TransactionManager.StartOpenCloseTransaction()
                Dim i As Integer = -1
                Dim ents As Ent() = New Ent(ids.Count - 1) {}
                For Each id As ObjectId In ids
                    Dim ent As Ent = DirectCast(tr.GetObject(id, OpenMode.ForRead), Ent)
                    i += 1
                    ents(i) = ent
                Next
                aeccSurf.Breaklines.AddStandardBreakline(ents, desc, 1.0)
                tr.Commit()
            End Using
        Catch ex As System.Exception
        End Try
    End Sub


    Public Shared Sub addAeccBreaklines(aeccSurf As AeccTinSurface, ents As AcadEntity(), desc As String)
        Try
            Using tr As Transaction = HostApplicationServices.WorkingDatabase.TransactionManager.StartOpenCloseTransaction()
                aeccSurf.Breaklines.AddStandardBreakline(ents, desc, 1.0)
                tr.Commit()
            End Using
        Catch ex As System.Exception
        End Try
    End Sub

    Public Shared Sub deleteAeccBreaklines(surf As AeccTinSurface)
        Try
            Using tr As Transaction = HostApplicationServices.WorkingDatabase.TransactionManager.StartOpenCloseTransaction()
                For i As Integer = 0 To surf.Breaklines.Count - 1
                    surf.Breaklines.Remove(i)
                Next
                tr.Commit()
            End Using
        Catch ex As System.Exception
        End Try
    End Sub

    Public Shared Function getBreaklines(aeccSurf As AeccTinSurface) As List(Of Polyline3d)
        Dim brkLines As New List(Of Polyline3d)()
        Try
            Using tr As Transaction = HostApplicationServices.WorkingDatabase.TransactionManager.StartOpenCloseTransaction()
                For Each brkLine As AeccSurfaceBreakline In aeccSurf.Breaklines
                    Dim brkLineEnts As Ent() = brkLine.BreaklineEntities

                    For i As Integer = 0 To brkLineEnts.Length - 1
                        Dim ent As Ent = brkLineEnts(i)
                        If ent IsNot Nothing Then
                            If TypeOf ent Is Polyline3d Then
                                Dim poly3d As Polyline3d = DirectCast(ent, Polyline3d)
                                brkLines.Add(poly3d)
                            End If
                        End If
                    Next
                Next
                tr.Commit()
            End Using
        Catch ex As System.Exception
        End Try

        Return brkLines
    End Function

    Public Shared Sub removeBreaklinesWith0Vertices(aeccSurf As AeccTinSurface)
        Dim acad3dPoly As Acad3DPolyline = Nothing
        Try
            Using tr As Transaction = HostApplicationServices.WorkingDatabase.TransactionManager.StartOpenCloseTransaction()
                Dim aeccBrkLines As AeccSurfaceBreaklines = aeccSurf.Breaklines
                For i As Integer = 0 To aeccBrkLines.Count - 1
                    Dim brkLine As AeccSurfaceBreakline = aeccBrkLines.Item(i)
                    Dim brkLineEnts As [Object] = brkLine.BreaklineEntities
                    Dim objs As [Object]() = DirectCast(brkLineEnts, [Object]())

                    If objs.Length = 0 Then
                        aeccBrkLines.Remove(i)
                        Continue For
                    End If

                    For Each obj As [Object] In objs
                        Dim acadObj As AcadObject = DirectCast(obj, AcadObject)
                        If acadObj.ObjectName.ToString() = "AcDb3dPolyline" Then
                            acad3dPoly = DirectCast(acadObj, Acad3DPolyline)
                        End If
                        If acad3dPoly IsNot Nothing Then
                            Dim varPnts As [Object] = Nothing
                            varPnts = acad3dPoly.Coordinates
                            If varPnts Is Nothing Then
                                acad3dPoly.Delete()
                            End If
                        End If
                    Next
                Next
                tr.Commit()
            End Using
        Catch ex As System.Exception
        End Try
    End Sub

    Public Shared Function lwPolyToPoint3dCollection(lwPoly As AcadLWPolyline) As Point3dCollection
        Dim pnts3d As New Point3dCollection()
        Dim pnt3d As Point3d = Point3d.Origin
        Dim pnts As Double() = lwPoly.Coordinates
        For i As Integer = 0 To pnts.Length - 1 Step 2
            pnt3d = New Point3d(pnts(i), pnts(i + 1), 0.0)
            pnts3d.Add(pnt3d)
        Next

        Return pnts3d
    End Function

    Public Shared Sub CreateSurveyProject(JN As String)
        Dim survProjs As AeccSurveyProjects = BaseObjsCom.aeccSurvDoc.Projects
        For Each survProj As AeccSurveyProject In survProjs
            If survProj.Name = JN Then
                survProj.Open()
                Exit Sub
            End If
        Next
        Try
            survProjs.Create(JN)
        Catch ex As System.Exception
            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(String.Format("Failed to create Survey Project: {0}", JN))
        End Try
    End Sub

    Public Shared Function resetAeccBrklines(aeccSurf As AeccTinSurface) As Handle
        Dim handleBrkline As Handle = stringToHandle("0000")
        Dim brkLineSum As New List(Of BrkLineData)()
        Dim acad3dPoly As Acad3DPolyline = Nothing
        Try
            Using tr As Transaction = HostApplicationServices.WorkingDatabase.TransactionManager.StartOpenCloseTransaction()
                Dim aeccBrklines As AeccSurfaceBreaklines = aeccSurf.Breaklines

                For i As Integer = 0 To aeccBrklines.Count - 1
                    Dim brkLine As AeccSurfaceBreakline = aeccBrklines.Item(i)
                    Dim brkLineEnts As [Object] = brkLine.BreaklineEntities
                    Dim objs As [Object]() = DirectCast(brkLineEnts, [Object]())
                    For Each obj As [Object] In objs
                        Dim acadObj As AcadObject = DirectCast(obj, AcadObject)
                        If acadObj.ObjectName.ToString() = "AcDb3dPolyline" Then
                            acad3dPoly = DirectCast(acadObj, Acad3DPolyline)
                        End If

                        If acad3dPoly IsNot Nothing Then
                            Dim h As String = acad3dPoly.Handle
                            Dim ln As Long = Convert.ToInt64(h, 16)
                            Dim hn As New Handle(ln)

                            Dim brkLineData As New BrkLineData()

                            brkLineData.id = BaseObjsCom.acadDoc.Database.GetObjectId(False, hn, 0)
                            brkLineData.Desc = brkLine.Description
                            brkLineData.Handle = hn
                            brkLineSum.Add(brkLineData)
                        End If
                    Next
                Next

                aeccSurf.deleteAeccBreaklines()


                Dim byDesc As Object = From n In brkLineSum
                             Group n By n.Desc
                             Into Group
                             Order By Desc
                             Select Group


                Dim ids As New ObjectIdCollection()
                For Each brkData As Object In byDesc
                    Dim desc As String = brkData.Key
                    For Each brklinData As Object In brkData
                        ids.Add(brklinData.id)
                    Next
                    aeccSurf.addAeccBreaklines(ids, desc)
                Next
                tr.Commit()
            End Using
        Catch ex As System.Exception
            Application.ShowAlertDialog(String.Format("{0} ExtensionMethods.cs: line: 251", ex.Message))
        End Try
        Dim ss As SelectionSet = Nothing

        Dim ed As Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
        Dim tvs() As TypedValue = Nothing

        tvs.SetValue(New TypedValue(DxfCode.Start, RXClass.GetClass(GetType(Polyline3d))), 0)
        Dim filter = New SelectionFilter(tvs)

        Dim psr As PromptSelectionResult = ed.GetSelection(filter)

        ss = psr.Value

        If ss IsNot Nothing Then
            Dim ids() As ObjectId = ss.GetObjectIds()
            Using tr As Transaction = HostApplicationServices.WorkingDatabase.TransactionManager.StartOpenCloseTransaction()
                Dim dbObj As Autodesk.AutoCAD.DatabaseServices.DBObject = tr.GetObject(ids(ids.Count - 1), OpenMode.ForRead)
                handleBrkline = dbObj.Handle
            End Using

        Else
            handleBrkline = stringToHandle("0000")
        End If

        Return handleBrkline
    End Function

    Public Shared Function getBrklineDescriptions(aeccSurf As AeccTinSurface) As List(Of String)
        Dim brkLineSum As New List(Of BrkLineData)()
        Dim descs As New List(Of String)()
        Dim poly3d As Polyline3d = Nothing
        Try
            Using tr As Transaction = HostApplicationServices.WorkingDatabase.TransactionManager.StartOpenCloseTransaction()
                For i As Integer = 0 To aeccSurf.Breaklines.Count - 1
                    Dim brkLine As AeccSurfaceBreakline = aeccSurf.Breaklines.Item(i)
                    Dim brkLineEnts As [Object] = brkLine.BreaklineEntities
                    Dim ents As Ent() = DirectCast(brkLineEnts, Ent())
                    For Each ent As Ent In ents
                        If TypeOf ent Is Polyline3d Then
                            poly3d = DirectCast(ent, Polyline3d)
                        End If

                        If poly3d IsNot Nothing Then
                            Dim brkLineData As New BrkLineData()
                            brkLineData.Desc = brkLine.Description
                            brkLineData.id = poly3d.ObjectId
                            brkLineData.Handle = poly3d.Handle
                            brkLineSum.Add(brkLineData)
                        End If
                    Next
                Next

                Dim byDesc As Object = From n In brkLineSum
                                       Group n By n.Desc
                                       Into Group
                                       Order By Desc
                                       Select Group

                For Each brkData As Object In byDesc
                    Dim desc As String = brkData.Key
                    descs.Add(desc)
                Next
                tr.Commit()
            End Using
        Catch ex As System.Exception
            Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(String.Format("{0} ExtensionMethods.cs: line: 147", ex.Message))
        End Try
        Return descs
    End Function

End Class