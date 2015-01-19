using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;


namespace Stake
{
    public class Stake_Startup : IExtensionApplication
    {      
        public void Initialize()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("\nStake.dll Loaded");
        }

        public void Terminate()
        {
        }
    }
}