using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: ExtensionApplication(typeof(Grading.Grading_Startup))]

namespace Grading
{
    public class Grading_Startup : IExtensionApplication
    {
        void IExtensionApplication.Initialize()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("\nGrading.dll Loaded");
        }

        void IExtensionApplication.Terminate()
        {
        }
    }
}
