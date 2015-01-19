using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: ExtensionApplication(typeof(WSP.WSP_Startup))]

namespace WSP
{
    public class WSP_Startup : IExtensionApplication
    {
        void IExtensionApplication.Initialize()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("\nWSP.dll Loaded");
        }

        void IExtensionApplication.Terminate()
        {
        }

    }
}
