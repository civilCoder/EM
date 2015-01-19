using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(LdrText.LdrText_Startup))]

namespace LdrText
{
    public class LdrText_Startup : IExtensionApplication
    {
        void IExtensionApplication.Initialize()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("\nLdrText.dll Loaded");
        }

        void IExtensionApplication.Terminate()
        {
        }
    }
}
