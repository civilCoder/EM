using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;

namespace ProcessFigures
{
    internal class Startup : IExtensionApplication
    {
        public void Initialize()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            doc.Editor.WriteMessage("\nProcessFigures.dll Loaded");
        }

        public void Terminate()
        {
        }
    }
}