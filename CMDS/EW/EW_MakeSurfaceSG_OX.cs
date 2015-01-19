using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Windows.Forms;

namespace EW
{
    public static class EW_MakeSurfaceSG_OX
    {
        static bool exists = false;

        public static void
        makeSurface(string strSurfaceName)
        {
            SelectionSet objSSet = EW_Utility1.buildSSet2a(strSurfaceName);

            if (objSSet == null || objSSet.Count == 0)
                return;

            ObjectId[] ids = objSSet.GetObjectIds();
            ObjectIdCollection idsBrks = new ObjectIdCollection();

            foreach (ObjectId id in ids)
                idsBrks.Add(id);

            EW_CheckSurfaceStyles.checkSurfaceStyles(strSurfaceName);

            TinSurface objSurface = Surf.getTinSurface(strSurfaceName);
            if (objSurface == null)
                return;
            objSurface.ObjectId.delete();

            objSurface = Surf.addTinSurface(strSurfaceName, out exists);

            objSurface.BreaklinesDefinition.AddStandardBreaklines(idsBrks, 1.0, 0, 0, 0);
            objSurface.BuildOptions.CrossingBreaklinesElevationOption = Autodesk.Civil.CrossingBreaklinesElevationType.UseAverage;

            SelectionSet objSSetLim = EW_Utility1.buildSSetGradingLim();
            
            if (objSSetLim.Count == 0){
                MessageBox.Show("GRADING LIMIT not found - OUTER BOUNDARY not added.");
                return;                
            }
            

            ObjectId idLWPline = objSSetLim.GetObjectIds()[0];
            Conv.processBndry(idLWPline);

            ObjectId[] id3dPoly = {Conv.poly_Poly3d(idLWPline, 0, "0")};

            objSurface.BoundariesDefinition.AddBoundaries(new ObjectIdCollection(id3dPoly), 1.0, Autodesk.Civil.SurfaceBoundaryType.Outer, true);
            objSurface.Rebuild();
            id3dPoly[0].delete();
        }
    }
}
