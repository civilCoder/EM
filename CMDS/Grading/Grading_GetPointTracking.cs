using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using gc = Grading.Grading_CalcBasePnt;
using gp = Grading.Grading_Public;

namespace Grading
{
    internal class Grading_GetPointTracking
    {
        public static Point3d
        getPoint(string prompt, string nameCmd)
        {
            PromptPointResult ppr = null;
            try
            {
                gp.pnt3dT = Pub.pnt3dO;

                Editor ed = BaseObjs._editor;
                ed.TurnForcedPickOn();

                switch (nameCmd)
                {
                    case "cmdSPG":
                    case "cmdSPGS":
                        ed.PointMonitor += ed_PointMonitorSPG;
                        ppr = ed.GetPoint(prompt);
                        ed.PointMonitor -= ed_PointMonitorSPG;
                        break;

                    case "cmdTP":
                        ed.PointMonitor += ed_PointMonitorTP;
                        ppr = ed.GetPoint(prompt);
                        ed.PointMonitor -= ed_PointMonitorTP;
                        break;

                    case "cmdRTR":
                        ed.PointMonitor += ed_PointMonitorRTR;
                        ppr = ed.GetPoint(prompt);
                        ed.PointMonitor -= ed_PointMonitorRTR;
                        break;
                }

                if (ppr.Status == PromptStatus.OK)
                    gp.pnt3dT = ppr.Value;
                else
                    View.remCursorBadge();

                if (gp.idLine != ObjectId.Null)
                    gp.idLine.delete();
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Grading_GetPointTracking.cs: line: 55");
            }

            return gp.pnt3dT;
        }

        private static void
        ed_PointMonitorRTR(object sender, PointMonitorEventArgs e)
        {
            Editor ed = BaseObjs._editor;
            if (!e.Context.PointComputed)
                return;
            if (gp.idLine != ObjectId.Null)
                gp.idLine.delete();

            Point3dCollection pnts3d = gp.poly3dRF.getCoordinates3d();

            Point3d pnt3dBEG = pnts3d[gp.vertex + 0];
            Point3d pnt3dEND = pnts3d[gp.vertex + 1];

            pnt3dBEG = new Point3d(pnt3dBEG.X, pnt3dBEG.Y, 0);
            pnt3dEND = new Point3d(pnt3dEND.X, pnt3dEND.Y, 0);

            Point3d pnt3dPick = e.Context.ComputedPoint;

            gp.pnt3dX = gc.calcBasePnt2d(pnt3dPick, pnt3dBEG, pnt3dEND);
            if (gp.pnt3dX == Pub.pnt3dO)
            {
                gp.inBounds = false;
                return;
            }
            else
                gp.inBounds = true;

            gp.idLine = Draw.addLine(gp.pnt3dX, pnt3dPick);
        }

        private static void
        ed_PointMonitorSPG(object sender, PointMonitorEventArgs e)
        {
            Editor ed = BaseObjs._editor;
            if (!e.Context.PointComputed)
                return;
            if (gp.idLine != ObjectId.Null)
                gp.idLine.delete();

            Point3d pnt3dBEG = gp.pnt3d1;
            Point3d pnt3dEND = gp.pnt3d2;

            pnt3dBEG = new Point3d(pnt3dBEG.X, pnt3dBEG.Y, 0);
            pnt3dEND = new Point3d(pnt3dEND.X, pnt3dEND.Y, 0);

            Point3d pnt3dPick = e.Context.ComputedPoint;

            gp.pnt3dX = gc.calcBasePnt2d(pnt3dPick, pnt3dBEG, pnt3dEND);
            if (gp.pnt3dX == Pub.pnt3dO)
                return;

            gp.idLine = Draw.addLine(gp.pnt3dX, pnt3dPick);
        }

        private static void
        ed_PointMonitorTP(object sender, PointMonitorEventArgs e)
        {
            Editor ed = BaseObjs._editor;
            if (!e.Context.PointComputed)
                return;
            if (gp.idLine != ObjectId.Null)
                gp.idLine.delete();

            Point3d pnt3dBEG = gp.pnt3d1;
            Point3d pnt3dEND = gp.pnt3d2;

            pnt3dBEG = new Point3d(pnt3dBEG.X, pnt3dBEG.Y, 0);
            pnt3dEND = new Point3d(pnt3dEND.X, pnt3dEND.Y, 0);

            Point3d pnt3dPick = e.Context.ComputedPoint;

            gp.pnt3dX = gc.calcBasePnt3d(pnt3dPick, pnt3dBEG, pnt3dEND);
            if (gp.pnt3dX == Pub.pnt3dO)
                return;

            gp.idLine = Draw.addLine(gp.pnt3dX, pnt3dPick);
        }

        private static void
        getPointRTR()
        {
            Point3dCollection pnts3d = gp.poly3dRF.getCoordinates3d();

            Point3d pnt3dBEG = pnts3d[gp.vertex + 0];
            Point3d pnt3dEND = pnts3d[gp.vertex + 1];

            pnt3dBEG = new Point3d(pnt3dBEG.X, pnt3dBEG.Y, 0);
            pnt3dEND = new Point3d(pnt3dEND.X, pnt3dEND.Y, 0);
            PromptStatus ps;

            Point3d pnt3dPick = UserInput.getPoint("\nSelect Target Location: ", out ps, osMode: 513);

            gp.pnt3dX = gc.calcBasePnt2d(pnt3dPick, pnt3dBEG, pnt3dEND);
            if (gp.pnt3dX == Pub.pnt3dO)
            {
                gp.inBounds = false;
                return;
            }
            else
                gp.inBounds = true;
        }
    }
}
