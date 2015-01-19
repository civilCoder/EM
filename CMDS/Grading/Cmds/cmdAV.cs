using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;

namespace Grading.Cmds
{
    public static class cmdAV
    {
        public static void
        AV()
        {
            ObjectId idPoly3d = Select.getBrkLine("\nSelect target breakline: ");
            CogoPoint cgPnt = CgPnt.selectPoint("\nSelect point to add to breakline: ", osMode: 8);
            cgPnt.ObjectId.updatePntXData(idPoly3d, apps.lnkBrks);
            Point3d pnt3d = cgPnt.ObjectId.getCogoPntCoordinates();
            int numVertex = Geom.getVertexNo(idPoly3d, pnt3d);
            idPoly3d.addVertexToPoly3d(pnt3d, numVertex, cgPnt.Handle);
        }
    }
}
