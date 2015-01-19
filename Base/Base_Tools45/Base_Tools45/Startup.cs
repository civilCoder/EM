using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(Base_Tools45.Startup))]

namespace Base_Tools45
{
    public class Startup : IExtensionApplication
    {
        public void Initialize()
        {
            BaseObjs._editor.WriteMessage("\nBase_Tools45.dll Loaded\n");
        }

        public void Terminate()
        {
        }
    }
}
