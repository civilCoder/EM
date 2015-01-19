
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

using Base_Tools45;

namespace EW
{
    public static class EW_Dict
    {
        public static void
        updateDictGRADEDOCK(Handle h2d, Handle h3d, double slope, Point3d pnt3dBase0, Point3d pnt3dBase1){
            bool exists;
            ObjectId idDict = Dict.getNamedDictionary("GRADEDOCK", out exists);
            
            Dict.deleteXRec(idDict, "HANDLE2D");
            TypedValue[] tvs = new TypedValue[1] { new TypedValue(1005, h2d) };
            Dict.addXRec(idDict, "HANDLE2d", new ResultBuffer(tvs));

            Dict.deleteXRec(idDict, "HANDLE3D");
            tvs = new TypedValue[1] { new TypedValue(1005, h3d) };
            Dict.addXRec(idDict, "HANDLE3d", new ResultBuffer(tvs));

            Dict.deleteXRec(idDict, "SLOPE");
            tvs = new TypedValue[1] { new TypedValue(1040, slope) };
            Dict.addXRec(idDict, "SLOPE", new ResultBuffer(tvs));

            Dict.deleteXRec(idDict, "CENTROID");
            tvs = new TypedValue[1] { new TypedValue(1000, pnt3dBase0.ToString())};
            Dict.addXRec(idDict, "CENTROID", new ResultBuffer(tvs));

            Dict.deleteXRec(idDict, "TARGET");
            tvs = new TypedValue[1] { new TypedValue(1000, pnt3dBase1.ToString())};
            Dict.addXRec(idDict, "TARGET", new ResultBuffer(tvs));           
        }

        public static void
        retrieveDictGRADEDOCK(out Handle h2d, out Handle h3d, out double slope, out Point3d pnt3dBase0, out Point3d pnt3dBase1)
        {
            bool exists;
            ObjectId idDict = Dict.getNamedDictionary("GRADEDOCK", out exists);

            ResultBuffer rb = Dict.getXRec(idDict, "HANDLE2d");
            h2d = rb.AsArray()[0].Value.ToString().stringToHandle();

            rb = Dict.getXRec(idDict, "HANDLE3d");
            h3d = rb.AsArray()[0].Value.ToString().stringToHandle();

            rb = Dict.getXRec(idDict, "SLOPE");
            slope = double.Parse(rb.AsArray()[0].Value.ToString());

            rb = Dict.getXRec(idDict, "CENTROID");
            pnt3dBase0 = rb.AsArray()[0].Value.objPoint3dToPoint3d();


            rb = Dict.getXRec(idDict, "TARGET");
            pnt3dBase1 = rb.AsArray()[0].Value.objPoint3dToPoint3d();
        }

    }
}
