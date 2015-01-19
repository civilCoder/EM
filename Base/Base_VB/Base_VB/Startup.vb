Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.ApplicationServices

<Assembly: ExtensionApplication(GetType(Base_VB.Startup))> 

Public Class Startup
    Implements IExtensionApplication


    Public Sub Initialize() Implements IExtensionApplication.Initialize
        Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(vbLf & "Base_VB.dll Loaded" & vbLf)
    End Sub

    Public Sub Terminate() Implements IExtensionApplication.Terminate

    End Sub
End Class
