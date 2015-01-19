using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_VB;
using Surface = Autodesk.Civil.DatabaseServices.Surface;

namespace Grading.Cmds
{
    public static class cmdRB
    {
        public static void
        RB()
        {
            bool exists = false;
            ObjectIdCollection ids = BaseObjs._acadDoc.getBrkLines();
            ObjectIdCollection idsF = BaseObjs._acadDoc.getFeatureLines();

            foreach (ObjectId id in idsF)
                ids.Add(id);

            ObjectId idSurface = Base_Tools45.C3D.Surf.getSurface("CPNT-ON", out exists);
            Point3dCollection pnts3d = mySurfaces.getOuterBoundary("CPNT-ON");
            Layer.manageLayers("CPNT-BNDRY");
            ObjectId idPoly = pnts3d.addPoly("CPNT-BNDRY");
            ObjectIdCollection idsBndry = new ObjectIdCollection { idPoly };
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    TinSurface tinSurface = (TinSurface)tr.GetObject(idSurface, OpenMode.ForWrite);
                    tinSurface.deleteBreaklines();
                    tinSurface.BreaklinesDefinition.AddStandardBreaklines(ids, 1.0, 0.0, 0.0, 0.0);
                    tinSurface.BoundariesDefinition.AddBoundaries(idsBndry, 1.0, Autodesk.Civil.SurfaceBoundaryType.Outer, true);
                    tinSurface.Rebuild();
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdRB.cs: line: 41");
            }
        }

        public static void
        RBA()
        {
            Surface surface = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDoc())
                {
                    ObjectIdCollection ids = BaseObjs._civDoc.GetSurfaceIds();
                    if (ids.Count == 0)
                        return;
                    foreach (ObjectId id in ids)
                    {
                        try
                        {
                            surface = (Surface)tr.GetObject(id, OpenMode.ForRead);
                            if (surface is TinSurface)
                            {
                                TinSurface tinSurface = (TinSurface)surface;
                                tinSurface.UpgradeOpen();
                                tinSurface.Rebuild();
                                tinSurface.DowngradeOpen();
                            }
                        }
                        catch (System.Exception ex)
                        {
                            BaseObjs.writeDebug(ex.Message + " cmdRB.cs: line: 71");
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdRB.cs: line: 79");
            }
        }
    }
}
