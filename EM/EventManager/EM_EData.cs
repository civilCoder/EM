using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace EventManager
{
    public class EM_EData
    {
        public Handle h;
        public ObjectId id;
        public string objType;
        public Point3d pnt3d;
        public TypedValue[] tvs;

        public EM_EData(Handle han, ObjectId idObj, string strType, Point3d pnt3dLoc, TypedValue[] tv)
        {
            h = han;
            id = idObj;
            objType = strType;
            pnt3d = pnt3dLoc;
            tvs = tv;
        }
    }
}
