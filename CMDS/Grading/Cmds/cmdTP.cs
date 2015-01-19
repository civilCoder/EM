using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using Base_Tools45.Jig;
using System.Collections.Generic;
using gc = Grading.Grading_CalcBasePnt;
using gp = Grading.Grading_Public;
using gPnt = Grading.Grading_GetPointTracking;
using sMath = System.Math;

namespace Grading.Cmds
{
    public static class cmdTP
    {
        private static double pi = sMath.PI;
        private static uint pntNum = 0;

        static ObjectId idCgPnt1 = ObjectId.Null, idCgPnt2 = ObjectId.Null, idCgPnt3 = ObjectId.Null;
        static List<ObjectId> idsCgPnt = new List<ObjectId>();
        static string elev = "";
        static string slope = "";
        static string xSlope = "";
        static string width = "";

        static bool escaped = true;
        static PromptStatus ps;

        public static void
        TP1()
        {
            elev = UserInput.getCogoPoint("\nPick Cogo Point 1: ", out idCgPnt1, ObjectId.Null, osMode: 8);
            if (elev == "")
                return;
            idsCgPnt.Add(idCgPnt1);

            elev = UserInput.getCogoPoint("\nPick Cogo Point 2: ", out idCgPnt2, idCgPnt1, osMode: 8);
            if (elev == "")
                return;
            idsCgPnt.Add(idCgPnt2);

            escaped = UserInput.getUserInputDoubleAsString("\nEnter Cross Slope (+ or -): ", out xSlope, xSlope);
            if (escaped)
                return;

            escaped = UserInput.getUserInputDoubleAsString("\nEnter Width: ", out width, width);
            if (escaped)
                return;

            Point3d pnt3d1 = idCgPnt1.getCogoPntCoordinates();
            Point3d pnt3d2 = idCgPnt2.getCogoPntCoordinates();
            List<Point3d> pnts3d = new List<Point3d> { pnt3d1, pnt3d2 };

            int side = Geom.getSide(pnts3d);

            double dir = pnt3d1.getDirection(pnt3d2);
            double len = pnt3d1.getDistance(pnt3d2);

            Point3d pnt3d3 = pnt3d2.traverse(dir + pi / 2 * -side, double.Parse(width), double.Parse(xSlope));
            ObjectId idCgPnt3 = pnt3d3.setPoint(out pntNum);
            idsCgPnt.Add(idCgPnt3);
            BaseObjs.updateGraphics();

            Point3d pnt3d4 = pnt3d1.traverse(dir + pi / 2 * -side, double.Parse(width), double.Parse(xSlope));
            ObjectId idCgPnt4 = pnt3d4.setPoint(out pntNum);
            idsCgPnt.Add(idCgPnt4);
            BaseObjs.updateGraphics();

            idsCgPnt.Add(idCgPnt1);

            ObjectId idPoly = ObjectId.Null;

            List<ObjectId> idCgPnts = new List<ObjectId> { idCgPnt1, idCgPnt2 };
            BrkLine.makeBreakline(apps.lnkBrks, "cmdTP4", out idPoly, idCgPnts);

            idCgPnts = new List<ObjectId> { idCgPnt2, idCgPnt3 };
            BrkLine.makeBreakline(apps.lnkBrks, "cmdTP4", out idPoly, idCgPnts);

            idCgPnts = new List<ObjectId> { idCgPnt3, idCgPnt4 };
            BrkLine.makeBreakline(apps.lnkBrks, "cmdTP4", out idPoly, idCgPnts);

            idCgPnts = new List<ObjectId> { idCgPnt4, idCgPnt1 };
            BrkLine.makeBreakline(apps.lnkBrks, "cmdTP4", out idPoly, idCgPnts);

        }

        public static void
        TP2()
        {
            Point3d pnt3d1 = UserInput.getPoint("\nPick Corner 1: ", Pub.pnt3dO, out escaped, out ps, osMode: 0);
            if (pnt3d1 == Pub.pnt3dO)
                return;

            Point3d pnt3d2 = UserInput.getPoint("\nPick Baseline direction: ", pnt3d1, out escaped, out ps, osMode: 0);
            if (pnt3d1 == Pub.pnt3dO)
                return;

            Vector2d v2dX = pnt3d2.Convert2d(BaseObjs.xyPlane) - pnt3d1.Convert2d(BaseObjs.xyPlane);
            Vector2d v2dY = v2dX.RotateBy(System.Math.PI / 2);

            Vector3d v3dY = new Vector3d(v2dY.X, v2dY.Y, 0);

            Point3d pnt3dY = pnt3d1 + v3dY;

            Matrix3d m3d = UCsys.addUCS(pnt3d1, pnt3d2, "temp");

            Point3d pnt3d0 = Db.wcsToUcs(pnt3d1);
            Polyline poly = jigPolylineArea(pnt3d0);

            UCsys.setUCS2World();

            poly.ObjectId.transformToWcs(BaseObjs._db);

            escaped = UserInput.getUserInputDoubleAsString("\nEnter Elevation: ", out elev, elev);
            if (escaped)
                return;

            escaped = UserInput.getUserInputDoubleAsString("\nEnter Slope: ", out slope, slope);
            if (escaped)
                return;

            double dblSlope = double.Parse(slope);
            Point3d pnt3dCEN = poly.getCentroid();

            Point3d pnt3dTAR = UserInput.getPoint("\nPick edge of polygon in the Direction of Increasing Slope: ", pnt3dCEN, out escaped, out ps, osMode: 641);
            if (pnt3dTAR == Pub.pnt3dO)
                return;

            ObjectIdCollection idspoly3d = new ObjectIdCollection();

            double dblDist = 0.0;
            ObjectId idPoly3d = ObjectId.Null;

            pnt3dCEN = pnt3dCEN.addElevation(double.Parse(elev));

            if (pnt3dTAR != Pub.pnt3dO)
            {
                dblDist = pnt3dCEN.getDistance(pnt3dTAR);
            }
            string nameLayer = string.Format("{0}-BORDER", "BASIN");
            Layer.manageLayers(nameLayer);

            int numObj = 0;
            bool exists = false;
            ObjectId idDict = Dict.getNamedDictionary(apps.lnkBrks, out exists);
            if (exists)
                numObj = idDict.getDictEntries().Count;

            string nameSurf = string.Format("{0}{1}", "BASIN", numObj.ToString("00"));

            ObjectId idDictObj = Dict.addSubDict(idDict, nameSurf);

            using (BaseObjs._acadDoc.LockDocument())
            {
                if (pnt3dTAR != Pub.pnt3dO)
                {
                    pnt3dTAR = new Point3d(pnt3dTAR.X, pnt3dTAR.Y, pnt3dCEN.Z + dblDist * dblSlope);
                    idPoly3d = Base_Tools45.C3D.DrawBasinBot.build3dPolyBasinBot(poly.ObjectId, pnt3dCEN, pnt3dTAR, dblSlope, nameLayer, apps.lnkBrks);
                }
                else
                {
                    idPoly3d = Conv.poly_Poly3d(poly.ObjectId, double.Parse(elev), nameLayer);
                }
                idspoly3d.Add(idPoly3d);

                TinSurface surf = Surf.addTinSurface(nameSurf, out exists);
                if (exists)
                {
                    Application.ShowAlertDialog(string.Format("Surface Name conflict - surface \"{0}\" already exists.  Exiting...", nameSurf));
                    return;
                }

                surf.BreaklinesDefinition.AddStandardBreaklines(idspoly3d, 1.0, 0.0, 0.0, 0.0);
                surf.Rebuild();
            }

            using (BaseObjs._acadDoc.LockDocument())
            {
                BaseObjs.regen();
            }
        }

        public static void
        TP3()
        {
            elev = UserInput.getCogoPoint("\nPick Cogo Point 1: ", out idCgPnt1, ObjectId.Null, osMode: 8);
            if (elev == "")
                return;

            idsCgPnt.Add(idCgPnt1);

            elev = UserInput.getCogoPoint("\nPick Cogo Point 2: ", out idCgPnt2, idCgPnt1, osMode: 8);
            if (elev == "")
                return;

            idsCgPnt.Add(idCgPnt3);

            elev = UserInput.getCogoPoint("\nPick Cogo Point 3: ", out idCgPnt3, idCgPnt2, osMode: 8);
            if (elev == "")
                return;

            idsCgPnt.Add(idCgPnt3);

            Point3d pnt3d1 = idCgPnt1.getCogoPntCoordinates();
            Point3d pnt3d2 = idCgPnt2.getCogoPntCoordinates();
            Point3d pnt3d3 = idCgPnt3.getCogoPntCoordinates();

            Vector2d v2d12 = (pnt3d2 - pnt3d1).Convert2d(BaseObjs.xyPlane);
            Vector2d v2d21 = (pnt3d1 - pnt3d2).Convert2d(BaseObjs.xyPlane);
            Vector2d v2d23 = (pnt3d3 - pnt3d2).Convert2d(BaseObjs.xyPlane);
            Vector2d v2d32 = (pnt3d2 - pnt3d3).Convert2d(BaseObjs.xyPlane);

            ObjectId idCgPnt4 = ObjectId.Null;
            Point3d pnt3d4 = Pub.pnt3dO;

            double dir = 0.0;
            double len = 0.0;
            double slp = 0.0;
            double delta = pi - v2d12.GetAngleTo(v2d23);
            double diff = System.Math.Abs(delta - pi / 2);
            if (diff < 0.01)
            {
                dir = v2d21.Angle;
                len = pnt3d2.getDistance(pnt3d1);
                slp = pnt3d2.getSlope(pnt3d1);
                pnt3d4 = pnt3d3.traverse(dir, len, slp);
                idCgPnt4 = pnt3d4.setPoint(out pntNum);
                idsCgPnt.Add(idCgPnt4);
            }
            else
            {
                dir = v2d23.Angle;
                len = pnt3d2.getDistance(pnt3d3);
                len = len * System.Math.Cos(delta);
                slp = pnt3d2.getSlope(pnt3d3);
                slp = slp * System.Math.Cos(delta);
                double testRight = Geom.testRight(pnt3d1, pnt3d2, pnt3d3);
                dir = v2d12.Angle;
                len = pnt3d1.getDistance(pnt3d2);
                pnt3d4 = pnt3d3.traverse(dir, len, slp);
                idCgPnt4 = pnt3d4.setPoint(out pntNum);
                idsCgPnt.Insert(2, idCgPnt4);
            }
            idsCgPnt.Add(idsCgPnt[0]);
            ObjectId idPoly = ObjectId.Null;
            for (int n = 0; n < idsCgPnt.Count - 1; n++)
            {
                List<ObjectId> idCgPnts = new List<ObjectId> { idsCgPnt[n], idsCgPnt[n + 1] };
                BrkLine.makeBreakline(apps.lnkBrks, "cmdTP3", out idPoly, idCgPnts);
            }
        }

        public static void
        TP4()
        {

            gp.pnt3d1 = Pub.pnt3dO;
            gp.pnt3d2 = Pub.pnt3dO;
            ps = PromptStatus.Cancel;
            object mode = 0;

            BaseObjs.acadActivate();
            Vector3d v3d = Vector3d.XAxis;
            try
            {
                elev = UserInput.getCogoPoint("\nPick Cogo Point 1: ", out idCgPnt1, ObjectId.Null, osMode: 8);
                if (elev == "")
                    return;
                else
                {
                    gp.pnt3d1 = idCgPnt1.getCogoPntCoordinates();
                }

                elev = UserInput.getCogoPoint("\nPick Cogo Point 2: ", out idCgPnt2, ObjectId.Null, osMode: 8);
                if (elev == "")
                    return;
                else
                    gp.pnt3d2 = idCgPnt2.getCogoPntCoordinates();

                mode = SnapMode.getOSnap();
                SnapMode.setOSnap(8);

                gp.pnt3dX = Pub.pnt3dO;
                gp.pnt3dT = Pub.pnt3dO;

                System.Windows.Forms.Keys mods = System.Windows.Forms.Control.ModifierKeys;
                bool shift = (mods & System.Windows.Forms.Keys.Shift) > 0;
                bool control = (mods & System.Windows.Forms.Keys.Control) > 0;
                gp.shift = shift;

                gp.pnt3dT = gPnt.getPoint("\nSelect Target location (CogoPoint for xSlope and distance / pick side to enter xSlope and distance: ", "cmdTP");

                if (gp.pnt3dT == Pub.pnt3dO)
                    return;
                else if (gp.pnt3dT.Z == 0)
                {
                    gp.pnt3dX = gc.calcBasePnt3d(gp.pnt3dT, gp.pnt3d1, gp.pnt3d2);
                    if (gp.pnt3dX == Pub.pnt3dO)
                        return;

                    escaped = UserInput.getUserInputDoubleAsString("\nEnter Cross Slope (+ or -): ", out xSlope, xSlope);
                    if (escaped)
                        return;

                    escaped = UserInput.getUserInputDoubleAsString("\nEnter Width: ", out width, width);
                    if (escaped)
                        return;
                    double dist = gp.pnt3d1.getDistance(gp.pnt3d2);
                    double distX = gp.pnt3d1.getDistance(gp.pnt3dX);

                    Point2d pnt2dX = gp.pnt3dX.Convert2d(BaseObjs.xyPlane);
                    Point2d pnt2dT = gp.pnt3dT.Convert2d(BaseObjs.xyPlane);
                    double distT = pnt2dX.GetDistanceTo(pnt2dT);

                    gp.pnt3dT = new Point3d(gp.pnt3dT.X, gp.pnt3dT.Y, gp.pnt3dX.Z + distT * double.Parse(xSlope));

                    v3d = gp.pnt3dT - gp.pnt3dX;
                    v3d *= double.Parse(width) / distT;
                }
                else
                {
                    gp.pnt3dX = gc.calcBasePnt3d(gp.pnt3dT, gp.pnt3d1, gp.pnt3d2);
                    if (gp.pnt3dX == Pub.pnt3dO)
                        return;
                   
                    CgPnt.setPoint(gp.pnt3dX, out pntNum, "CPNT-ON");

                    v3d = gp.pnt3dT - gp.pnt3dX;
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdTP.cs: line: 336");
            }
            finally
            {
                SnapMode.setOSnap((int)mode);
            }

            ObjectId idPoly = ObjectId.Null;
            Point3d pnt3d3 = idCgPnt2.getCogoPntCoordinates() + v3d;
            ObjectId idCgPnt3 = pnt3d3.setPoint(out pntNum);
            Point3d pnt3d4 = idCgPnt1.getCogoPntCoordinates() + v3d;
            ObjectId idCgPnt4 = pnt3d4.setPoint(out pntNum);

            List<ObjectId> idCgPnts = new List<ObjectId> { idCgPnt1, idCgPnt2 };
            BrkLine.makeBreakline(apps.lnkBrks, "cmdTP4", out idPoly, idCgPnts);

            idCgPnts = new List<ObjectId> { idCgPnt2, idCgPnt3 };
            BrkLine.makeBreakline(apps.lnkBrks, "cmdTP4", out idPoly, idCgPnts);

            idCgPnts = new List<ObjectId> { idCgPnt3, idCgPnt4 };
            BrkLine.makeBreakline(apps.lnkBrks, "cmdTP4", out idPoly, idCgPnts);

            idCgPnts = new List<ObjectId> { idCgPnt4, idCgPnt1 };
            BrkLine.makeBreakline(apps.lnkBrks, "cmdTP4", out idPoly, idCgPnts);
        }

        static private Polyline
        jigPolylineArea(Point3d pnt3dBase)
        {
            Database db = BaseObjs._db;
            Editor ed = BaseObjs._editor;

            Polyline poly = null;
            JigRect jgRect = new JigRect(pnt3dBase);
            ed.Drag(jgRect);

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                poly = new Polyline();
                poly.SetDatabaseDefaults();
                for (int i = 0; i < jgRect.corners.Count - 1; i++)
                {
                    Point3d pnt3d = jgRect.corners[i];
                    Point2d pnt2d = new Point2d(pnt3d.X, pnt3d.Y);
                    poly.AddVertexAt(i, pnt2d, 0, db.Plinewid, db.Plinewid);
                }
                poly.Closed = true;
                poly.TransformBy(jgRect.UCS);
                btr.AppendEntity(poly);
                tr.AddNewlyCreatedDBObject(poly, true);
                tr.Commit();
            }
            return poly;
        }

    }
}
