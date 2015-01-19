using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(PadCerts.PC_Startup))]

namespace PadCerts
{
    public class PC_Startup : IExtensionApplication
    {
        void IExtensionApplication.Initialize()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("\nPadCerts.dll Loaded");
        }

        void IExtensionApplication.Terminate()
        {
        }
    }
}
