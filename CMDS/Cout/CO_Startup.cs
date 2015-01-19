using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(Cout.CO_Startup))]

namespace Cout
{
    public class CO_Startup : IExtensionApplication
    {
        void IExtensionApplication.Initialize()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("\nCadOUT.dll Loaded\n");
        }

        void IExtensionApplication.Terminate()
        {
        }
    }
}
