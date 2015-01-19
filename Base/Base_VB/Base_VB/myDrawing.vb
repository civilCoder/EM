Option Strict Off
Option Explicit On

Imports Autodesk.AutoCAD.Interop
Imports Autodesk.AutoCAD.ApplicationServices

Public Class myDrawing

    Public Shared acadApp As AcadApplication = Application.AcadApplication

    Public Shared ReadOnly Property acadDoc() As AcadDocument
        Get
            Return acadApp.ActiveDocument
        End Get
    End Property

End Class