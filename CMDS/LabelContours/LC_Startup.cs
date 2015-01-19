using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(LabelContours.LC_Startup))]

namespace LabelContours
{
    public class LC_Startup : IExtensionApplication
    {
        void IExtensionApplication.Initialize()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("\nLabelContours.dll Loaded");
        }

        void IExtensionApplication.Terminate()
        {
        }
    }
}
