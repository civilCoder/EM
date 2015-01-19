using Autodesk.AutoCAD.Runtime;

namespace Base_Tools45
{
    public class thisAPP : IExtensionApplication
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            Base_Tools45.BaseObjs._editor.WriteMessage("\nBase_Tools2010 Loaded");
        }

        /// <summary>
        /// Terminates this instance.
        /// </summary>
        public void Terminate()
        {
        }
    }
}
