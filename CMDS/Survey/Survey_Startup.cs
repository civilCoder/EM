using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(Survey.Survey_Startup))]

namespace Survey
{
    public class Survey_Startup : IExtensionApplication
    {
        void IExtensionApplication.Initialize()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("\nSurvey.dll Loaded");
        }

        void IExtensionApplication.Terminate()
        {
        }

    }
}
