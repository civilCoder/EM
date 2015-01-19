
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Base_Tools45;

[assembly: ExtensionApplication(typeof(Bubble.BB_Startup))]

namespace Bubble
{
    public class BB_Startup : IExtensionApplication
    {
        public static void
        initDrawing()
        {
            Layer.manageLayers("BUBBLE", 5);
        }

        void
        IExtensionApplication.Initialize()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            doc.Editor.WriteMessage("\nBubble.dll Loaded\n");
        }

        void
        IExtensionApplication.Terminate()
        {
        }
    }
}
