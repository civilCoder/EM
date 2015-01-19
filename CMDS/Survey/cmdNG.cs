using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;

namespace Survey
{
    public static class cmdNG
    {
        public static void
        NG()
        {
            Layer.manageLayers("EXIST-SPOT");
            Layer.manageLayers("SPOT-LABEL");

            SelectionSet ss = Select.buildSSet(typeof(CogoPoint), selectAll: false);
            if (ss == null || ss.Count == 0)
                return;
            string elev = "";
            int length = 0;

            ObjectId[] ids = ss.GetObjectIds();
            ObjectId idMTxt = ObjectId.Null;
            Vector3d v3d = new Vector3d();

            for (int i = 0; i < ids.Length; i++)
            {
                Point3d pnt3dX = ids[i].getCogoPntCoordinates() + v3d;
                elev = string.Format("({0:F2})", ids[i].getCogoPntElevation());
                length = elev.Length;
                switch (length)
                {
                    case 6:         //      (X.XX)
                        v3d = new Vector3d(.030, .03, 0) * Misc.getCurrAnnoScale();
                        break;

                    case 7:         //     (XX.XX)
                        v3d = new Vector3d(-.000, .03, 0) * Misc.getCurrAnnoScale();
                        break;

                    case 8:         //    (XXX.XX)
                        v3d = new Vector3d(-.030, .03, 0) * Misc.getCurrAnnoScale();
                        break;

                    case 9:         //   (XXXX.XX)
                        v3d = new Vector3d(-.060, .03, 0) * Misc.getCurrAnnoScale();
                        break;

                    case 10:        //  (XXXXX.XX)
                        v3d = new Vector3d(-.090, .03, 0) * Misc.getCurrAnnoScale();
                        break;

                    case 11:        // (XXXXXX.XX)
                        v3d = new Vector3d(-.120, .03, 0) * Misc.getCurrAnnoScale();
                        break;

                    case 12:        //(XXXXXXX.XX)
                        v3d = new Vector3d(-.150, .03, 0) * Misc.getCurrAnnoScale();
                        break;
                }

                idMTxt = Txt.addMText(elev, pnt3dX + v3d,
                    0.0, 0.09, nameLayer: "SPOT-LABEL", justify: Pub.JUSTIFYCENTER);
            }
        }
    }
}