using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(ConvertToXData.CXD_Startup))]

namespace ConvertToXData
{
    public class CXD_Startup : IExtensionApplication
    {
        void IExtensionApplication.Initialize()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("\nConvertToXData.dll Loaded\n");
        }

        void IExtensionApplication.Terminate()
        {
        }
    }
}
