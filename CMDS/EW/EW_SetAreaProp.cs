using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;

namespace EW {
    public static class EW_SetAreaProp {
        public static void
        setAreaProp() {
            Point3d pnt3dPicked = Pub.pnt3dO;
            Polyline poly = (Polyline)Select.selectEntity(typeof(Polyline), "Select Control Object in Legend", "Selection Failed - Try Again or not.", out pnt3dPicked);
            int c = 27;
            BaseObjs._acadDoc.SendStringToExecute(c.asciiToString(), true, false, false);

            string strLayerName = poly.Layer.Substring(3);

            ObjectId idLayer = Layer.manageLayers(strLayerName);

            SelectionSet ss = Select.buildSSet(typeof(Polyline), false, "Select Target Object(s): ");
            ObjectId[] ids = ss.GetObjectIds();
            for (int i = 0; i < ids.Length; i++) {
                ids[i].changeProp(clr.byl, strLayerName);
            }
        }
    }
}