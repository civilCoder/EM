using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;

namespace Grading
{
    public static class Grading_GetNestedObject
    {
        public static ObjectId
        getBldgLimit(out string nameLayer, out ObjectId idBlkRef)
        {
            nameLayer = string.Empty;
            ObjectId id = ObjectId.Null;
            Point3d pnt3dPick = Pub.pnt3dO;
            idBlkRef = ObjectId.Null;
            try
            {
                id = xRef.getEntity("Select Inner Limit of Building Perimeter Wall:", true, out nameLayer, out pnt3dPick, out idBlkRef);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Grading_GetNestedObject.cs: line: 22");
            }
            return id;
        }
    }
}
