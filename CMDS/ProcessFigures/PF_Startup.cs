using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(ProcessFigures.PF_Startup))]

namespace ProcessFigures
{
    public class PF_Startup : IExtensionApplication
    {
        void IExtensionApplication.Initialize()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("\nProcessFigures.dll Loaded");
        }

        void IExtensionApplication.Terminate()
        {
        }
    }
}
