using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using gp = Grading.Grading_Public;

namespace Grading.Cmds
{
    public class cmdBLO
    {
        public static void
        BLO()
        {
            bool escape;
            PromptStatus ps;

            double deltaZ = Pub.offV;
            if (deltaZ == 0)
                deltaZ = 0.50;
            escape = UserInput.getUserInput(string.Format("\nElevation Offset <{0}>", deltaZ), out deltaZ, deltaZ);
            if (escape)
                return;

            double deltaXY = Pub.offH;
            if (deltaXY == 0)
                deltaXY = 2.0;
            escape = UserInput.getUserInput(string.Format("\nHorizontal Offset <{0}>", deltaXY), out deltaXY, deltaXY);
            if (escape)
                return;

            Pub.offV = deltaZ;
            Pub.offH = deltaXY;


            ObjectId id = ObjectId.Null;
            string prompt = "";

            Point3d pnt3dBase = Pub.pnt3dO;
            Point3d pnt3dLast = Pub.pnt3dO;

            prompt = "\nSelect First Point: ";
            string resElev1 = UserInput.getCogoPoint(prompt, out id, Pub.pnt3dO, osMode: 8);
            if (resElev1 == string.Empty)
                return;
            Point3d pnt3d1 = id.getCogoPntCoordinates();
            string desc1 = id.getCogoPntDesc();

            prompt = "\nSelect Second Point: ";
            string resElev2 = UserInput.getCogoPoint(prompt, out id, pnt3d1, osMode: 8);
            if (resElev2 == string.Empty)
                return;
            Point3d pnt3d2 = id.getCogoPntCoordinates();
            string desc2 = id.getCogoPntDesc();

            Point3d pnt3dX = UserInput.getPoint("Pick a point on side to offset to: ", out ps, osMode: 0);

            double dist = pnt3d1.getDistance(pnt3d2);
            double slope = (gp.pnt3d2.Z - gp.pnt3d1.Z) / dist;
            double dir = pnt3d1.getDirection(pnt3d2);

            if (pnt3dX.isRightSide(pnt3d1, pnt3d2))
            {
                dir = dir - System.Math.PI / 2;
            }
            else
            {
                dir = dir + System.Math.PI / 2;
            }

            Point3d pnt3d1X = pnt3d1.traverse(dir, deltaXY);
            Point3d pnt3d2X = pnt3d2.traverse(dir, deltaXY);

            pnt3d1X = new Point3d(pnt3d1X.X, pnt3d1X.Y, pnt3d1X.Z + deltaZ);
            pnt3d2X = new Point3d(pnt3d2X.X, pnt3d2X.Y, pnt3d2X.Z + deltaZ);

            uint pntNum = 0;
            ObjectId idCgPnt1 = pnt3d1X.setPoint(out pntNum, desc1);
            ObjectId idCgPnt2 = pnt3d2X.setPoint(out pntNum, desc2);
            List<ObjectId> ids = new List<ObjectId> { idCgPnt1, idCgPnt2 };
            
            ObjectId idPoly = ObjectId.Null;
            BrkLine.makeBreakline(apps.lnkBrks, "BLO", out idPoly, ids);
        }
    }
}
