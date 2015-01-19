Imports System
Imports System.Runtime.InteropServices


Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices

Imports Autodesk.AutoCAD.Interop
Imports Autodesk.AutoCAD.Interop.Common


Public Class myEvents
    <Flags()> _
    Friend Enum EventStatus As Integer
        CommandWillStart = 1
        CommandCancelled = 2
        CommandFailed = 4
        CommandEnded = 8
        CommandFinish = CommandCancelled + CommandFailed + CommandEnded
        SelectionAdded = 16
        SelectionRemoved = 32
        SelectionEvents = SelectionAdded + SelectionRemoved
        ObjectErased = 64
        ObjectOpenedForModify = 128
        ObjectModified = 256
        ObjectChangedEvents = ObjectErased + ObjectOpenedForModify + ObjectModified
        BeginDocumentClose = 512
        SaveBegin = 1024
        SaveComplete = 2048
        SaveAborted = 4096
        SaveEndEvents = SaveComplete + SaveAborted
        SaveEvents = SaveBegin + SaveComplete + SaveAborted
        ImpliedSelectionChanged = 8192
    End Enum

    Friend Enum ForceEvent
        ActivateOnly = 1
        DeactivateOnly = 2
        SetTo = 3
    End Enum

    Friend Overloads Sub ActivateImpliedSelectionChangedEvent(Optional ByVal ShowStatus As Boolean = False)
        If CBool(eCurrentEventStatus And EventStatus.ImpliedSelectionChanged) = False Then
            AddHandler _Document.ImpliedSelectionChanged, AddressOf H_ImpliedSelectionChanged
            eCurrentEventStatus += EventStatus.ImpliedSelectionChanged
            If ShowStatus Then Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(vbLf + "Activated Object ImpliedSelectionChanged")
        Else
            If ShowStatus Then Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(vbLf + "ImpliedSelectionChange Was Already Active")
        End If
    End Sub
    Friend Overloads Sub DeactivateImpliedSelectionChangedEvent(Optional ByVal ShowStatus As Boolean = False)
        If CBool(eCurrentEventStatus And EventStatus.ImpliedSelectionChanged) Then
            Try : RemoveHandler _Document.ImpliedSelectionChanged, AddressOf H_ImpliedSelectionChanged : Catch ex As System.Exception : End Try
            eCurrentEventStatus -= EventStatus.ImpliedSelectionChanged
            If ShowStatus Then Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(vbLf + "Deactivated Object ImpliedSelectionChanged")
        Else
            If ShowStatus Then Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(vbLf + "ImpliedSelectionChange Was Not Active")
        End If
    End Sub

    Friend Function SetEvents(Optional ByVal ForceOption As ForceEvent = ForceEvent.ActivateOnly, Optional ByVal ForceStatus As EventStatus = 0) As EventStatus
        Dim eStatusToSet As EventStatus = 0
        Dim eWas As EventStatus = eCurrentEventStatus
        If CInt(ForceStatus) > -1 Then eStatusToSet = ForceStatus Else eStatusToSet = eLastEventStatus
        Select Case ForceOption
            Case ForceEvent.ActivateOnly
                If (eStatusToSet And EventStatus.BeginDocumentClose) Then ActivateDocBeginCloseEvent()
                If (eStatusToSet And EventStatus.ImpliedSelectionChanged) Then ActivateImpliedSelectionChangedEvent()
                If (eStatusToSet And EventStatus.ObjectErased) Then ActivateObjectErasedEvent()
                If (eStatusToSet And EventStatus.ObjectModified) Then ActivateObjectModified()
                If (eStatusToSet And EventStatus.ObjectOpenedForModify) Then ActivateObjectOpenedForModify()
                If (eStatusToSet And EventStatus.CommandWillStart) Then ActivateCmdWillStartEvent()
                If (eStatusToSet And EventStatus.CommandFinish) Then ActivateCmdEvents()
            Case ForceEvent.DeactivateOnly
                If (eStatusToSet And EventStatus.BeginDocumentClose) = False Then DeactivateDocBeginCloseEvent()
                If (eStatusToSet And EventStatus.ImpliedSelectionChanged) = False Then DeactivateImpliedSelectionChangedEvent()
                If (eStatusToSet And EventStatus.ObjectErased) = False Then DeactivateObjectErasedEvent()
                If (eStatusToSet And EventStatus.ObjectModified) = False Then DeactivateObjectModified()
                If (eStatusToSet And EventStatus.ObjectOpenedForModify) = False Then DeactivateObjectOpenedForModify()
                If (eStatusToSet And EventStatus.CommandWillStart) = False Then DeactivateCmdWillStartEvent()
                If (eStatusToSet And EventStatus.CommandFinish) = False Then DeactivateCmdEvents()
            Case ForceEvent.SetTo
                If (eStatusToSet And EventStatus.BeginDocumentClose) Then ActivateDocBeginCloseEvent() Else DeactivateDocBeginCloseEvent()
                If (eStatusToSet And EventStatus.ImpliedSelectionChanged) Then ActivateImpliedSelectionChangedEvent() Else DeactivateImpliedSelectionChangedEvent()
                If (eStatusToSet And EventStatus.ObjectErased) Then ActivateObjectErasedEvent() Else DeactivateObjectErasedEvent()
                If (eStatusToSet And EventStatus.ObjectModified) Then ActivateObjectModified() Else DeactivateObjectModified()
                If (eStatusToSet And EventStatus.ObjectOpenedForModify) Then ActivateObjectOpenedForModify() Else DeactivateObjectOpenedForModify()
                If (eStatusToSet And EventStatus.CommandWillStart) Then ActivateCmdWillStartEvent() Else DeactivateCmdWillStartEvent()
                If (eStatusToSet And EventStatus.CommandFinish) Then ActivateCmdEvents() Else DeactivateCmdEvents()
        End Select
        Return eWas
    End Function

    Private eCurrentEventStatus As Boolean
    Private eLastEventStatus As Boolean

End Class
