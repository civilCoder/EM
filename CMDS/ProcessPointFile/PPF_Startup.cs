using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(ProcessPointFile.PPF_Startup))]

namespace ProcessPointFile
{
    public class PPF_Startup : IExtensionApplication
    {
        void IExtensionApplication.Initialize()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("\nProcessPointFile.dll Loaded");
        }

        void IExtensionApplication.Terminate()
        {
        }
    }
}
