using Autodesk.AutoCAD.DatabaseServices;
using Base_Tools45.C3D;

namespace EW {
    public static class EW_CheckSurfaceStyles {

        public static void
        checkSurfaceStyles(string varStyle = "") {

            ObjectId idSurfaceStyle = ObjectId.Null;
            if ((varStyle == "")) {
                Surf_Styles.getSurfaceStyle("EXIST");
                Surf_Styles.getSurfaceStyle("CPNT");
                Surf_Styles.getSurfaceStyle("SG");
                Surf_Styles.getSurfaceStyle("OX");
                Surf_Styles.getSurfaceStyle("BOT");
                Surf_Styles.getSurfaceStyle("VOL");
            }else {
                Surf_Styles.getSurfaceStyle(varStyle);
            }
        }
    }
}