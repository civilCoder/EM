using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(slopeGrading.SG_Startup))]

namespace slopeGrading
{
    public class SG_Startup : IExtensionApplication
    {
        void IExtensionApplication.Initialize()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("\nslopeGrading.dll Loaded");
        }

        void IExtensionApplication.Terminate()
        {
        }
    }
}
