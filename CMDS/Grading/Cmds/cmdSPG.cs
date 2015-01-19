using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using Base_Tools45.C3D;
using gc = Grading.Grading_CalcBasePnt;
using gp = Grading.Grading_Public;
using gPnt = Grading.Grading_GetPointTracking;

namespace Grading.Cmds
{
    public class cmdSPG
    {
        public static void
        SPG(string nameCmd)
        {
            gp.pnt3d1 = Pub.pnt3dO;
            gp.pnt3d2 = Pub.pnt3dO;
            PromptStatus ps = PromptStatus.Cancel;
            string desc1 = "";
            string desc2 = "";
            bool escape;
            object mode = SnapMode.getOSnap();

            try
            {
                Point3d pnt3d1 = Pub.pnt3dO;
                ObjectId idCgPnt1 = CgPnt.selectCogoPointByNode("\nSelect FIRST point-node: ", ref pnt3d1, out escape, out ps, osMode: 8);
                if (ps == PromptStatus.Cancel)
                    return;
                desc1 = idCgPnt1.getCogoPntDesc();
                gp.pnt3d1 = pnt3d1;

                Point3d pnt3d2 = Pub.pnt3dO;
                ObjectId idCgPnt2 = CgPnt.selectCogoPointByNode("\nSelect SECOND point-node: ", ref pnt3d2, out escape, out ps, osMode: 8);
                if (ps == PromptStatus.Cancel)
                    return;
                desc2 = idCgPnt2.getCogoPntDesc();
                gp.pnt3d2 = pnt3d2;

                SnapMode.setOSnap(1);

                do
                {
                    gp.pnt3dX = Pub.pnt3dO;
                    gp.pnt3dT = Pub.pnt3dO;

                    gp.pnt3dT = gPnt.getPoint("\nSelect Target location or press Enter to exit: ", nameCmd);
                    if (gp.pnt3dT == Pub.pnt3dO)
                        return;

                    gp.pnt3dX = gc.calcBasePnt3d(gp.pnt3dT, gp.pnt3d1, gp.pnt3d2);
                    if (gp.pnt3dX == Pub.pnt3dO)
                        return;

                    uint pntNum;
                    CgPnt.setPoint(gp.pnt3dX, out pntNum, desc1);
                }
                while (1 < 2);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdSPG.cs: line: 63");
            }
            finally
            {
                SnapMode.setOSnap((int)mode);
            }
        }
    }
}
