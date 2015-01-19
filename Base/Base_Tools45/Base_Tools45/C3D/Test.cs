using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(Base_Tools45.Test))]

namespace Base_Tools45.C3D
{
    /// <summary>
    ///
    /// </summary>
    public class Test
    {
        /// <summary>
        ///
        /// </summary>
        [CommandMethod("testStyles")]
        public void testStyles()
        {
            Prof_Style.CreateProfileLabelSetStyle("WALL");
        }
    }
}