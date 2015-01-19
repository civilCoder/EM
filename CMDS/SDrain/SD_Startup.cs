using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(SDrain.SD_Startup))]

namespace SDrain
{
    public class SD_Startup : IExtensionApplication
    {
        void IExtensionApplication.Initialize()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("\nSDrain.dll Loaded");
        }

        void IExtensionApplication.Terminate()
        {
        }
    }
}
