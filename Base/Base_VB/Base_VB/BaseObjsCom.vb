Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic

Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices

Imports Autodesk.AutoCAD.Interop
Imports Autodesk.AutoCAD.Interop.Common

Imports Autodesk.AECC.Interop

Imports Autodesk.AECC.Interop.Land
Imports Autodesk.AECC.Interop.UiLand

Imports Autodesk.AECC.Interop.Survey
Imports Autodesk.AECC.Interop.UiSurvey

Imports Autodesk.AEC.ApplicationServices
Imports Autodesk.AEC.Interop.Base

Public NotInheritable Class BaseObjsCom

    ''' <summary>
    '''
    ''' </summary>
    Public Shared ReadOnly Property acadApp() As AcadApplication
        Get
            Return Application.AcadApplication
        End Get
    End Property

    ''' <summary>
    '''
    ''' </summary>
    Public Shared ReadOnly Property acadDoc() As AcadDocument
        Get
            Return acadApp.ActiveDocument
        End Get
    End Property

    ''' <summary>
    '''
    ''' </summary>
    Public Shared ReadOnly Property aeccApp() As AeccApplication
        Get
            Return DirectCast(acadApp.GetInterfaceObject("AeccXUiLand.AeccApplication.10.4"), AeccApplication)
        End Get
    End Property

    ''' <summary>
    '''
    ''' </summary>
    Public Shared ReadOnly Property aeccDoc() As AeccDocument
        Get
            Return DirectCast(aeccApp.ActiveDocument, AeccDocument)
        End Get
    End Property

    Public Shared ReadOnly Property aeccDb() As AeccDatabase
        Get
            Return DirectCast(aeccApp.ActiveDocument.Database, AeccDatabase)
        End Get
    End Property

    Public Shared ReadOnly Property aeccSurvApp() As AeccApplication
        Get
            Return DirectCast(acadApp.GetInterfaceObject("AeccXUiSurvey.AeccSurveyApplication.10.4"), AeccApplication)
        End Get
    End Property

    Public Shared ReadOnly Property aeccSurvDoc() As AeccSurveyDocument
        Get
            Return DirectCast(aeccSurvApp.ActiveDocument, AeccSurveyDocument)
        End Get
    End Property

    Public Shared ReadOnly Property aeccSurvDb() As AeccSurveyDatabase
        Get
            Return DirectCast(aeccSurvApp.ActiveDocument.Database, AeccSurveyDatabase)
        End Get
    End Property

End Class
