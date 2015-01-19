using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(Wall.Wall_Startup))]

namespace Wall
{
    public class Wall_Startup : IExtensionApplication
    {
        void IExtensionApplication.Initialize()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("\nWall.dll Loaded");
        }

        void IExtensionApplication.Terminate()
        {
        }
    }
}
