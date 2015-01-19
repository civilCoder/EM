using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;
using Point3d = Autodesk.AutoCAD.Geometry.Point3d;

namespace Grading.Cmds
{
    public static class cmdGD_old
    {
        public static string bldgNum { get; set; }

        public static void
        GD()
        {
            ObjectId idPoly = ObjectId.Null;
            const string nameLayer = "CPNT-BRKLINE";

            double dblAngDock = 0;
            double dblLenDock = 0;

            Point3d pnt3dA = Pub.pnt3dO;           //point AHEAD
            Point3d pnt3dB;                             //point BACK

            int intSlopeSign = 0;

            const double pi = System.Math.PI;

            bool exists = false;
            ObjectId idDictGRADEDOCK = Dict.getNamedDictionary("GRADEDOCK", out exists);

            if (!exists)
            {
                Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog("Run AVG - exiting .....");
            }

            List<Point3d> pnts3dLim = getDockLimits();   //use 0 as dock number i.e. do one dock at a time

            bool escape = false;

            double width = 60;
            escape = UserInput.getUserInput(string.Format("Enter dock width: <{0}>:", width), out width, width);
            if (escape)
                return;

            double height = 4.04;
            escape = UserInput.getUserInput(string.Format("Enter dock height: <{0}>:", height), out height, height);
            if (escape)
                return;

            ObjectId idDictBLDG = Dict.getSubDict(idDictGRADEDOCK, bldgNum);        //bldgNum obtained @ getDockLimits
            ObjectId idDictDOCKS = ObjectId.Null;

            TypedValue[] tvs;
            using (BaseObjs._acadDoc.LockDocument())
            {
                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        ResultBuffer rb = Dict.getXRec(idDictBLDG, "HANDLE3D");
                        if (rb == null)
                            return;
                        tvs = rb.AsArray();

                        rb = Dict.getXRec(idDictBLDG, "SLOPE");
                        if (rb == null)
                            return;
                        tvs = rb.AsArray();
                        double dblSlope = (double)tvs[0].Value;

                        rb = Dict.getXRec(idDictBLDG, "CENTROID");
                        if (rb == null)
                            return;
                        tvs = rb.AsArray();
                        Point3d pnt3dCEN = new Point3d((double)tvs[0].Value, (double)tvs[1].Value, (double)tvs[2].Value);

                        rb = Dict.getXRec(idDictBLDG, "TARGET");
                        if (rb == null)
                            return;
                        tvs = rb.AsArray();

                        Point3d pnt3dTAR = new Point3d((double)tvs[0].Value, (double)tvs[1].Value, (double)tvs[2].Value);

                        double dblAngBase = pnt3dCEN.getDirection(pnt3dTAR);

                        ObjectIdCollection idsPoly3dX = new ObjectIdCollection();
                        List<Point3d> pnts3d = new List<Point3d>();
                        List<ObjectId> idsCgPnts = new List<ObjectId>();

                        try
                        {
                            Point3d pnt3dBEG = pnts3dLim[0];
                            Point3d pnt3dEND = pnts3dLim[1];

                            dblAngDock = Measure.getAzRadians(pnt3dBEG, pnt3dEND);
                            dblLenDock = pnt3dBEG.getDistance(pnt3dEND);
                            pnt3dB = pnt3dBEG;

                            if (pnt3dTAR != Pub.pnt3dO)
                                pnt3dA = new Point3d(pnt3dBEG.X, pnt3dBEG.Y, pnt3dTAR.Z + Geom.getCosineComponent(pnt3dCEN, pnt3dTAR, pnt3dBEG) * (dblSlope * -1) - height);
                            else
                                pnt3dA = new Point3d(pnt3dBEG.X, pnt3dBEG.Y, pnt3dCEN.Z - height);      //assuming flat floor if pnt3dTAR is -1,-1,-1

                            pnts3d.Add(pnt3dA);           //CgPnt 1
                        }
                        catch (System.Exception ex)
                        {
                            BaseObjs.writeDebug(ex.Message + " cmdGD_old.cs: line: 113");
                        }

                        ObjectId idPoly3d = ObjectId.Null;

                        if (dblSlope != 0)
                        {
                            double angDock2 = System.Math.Round(dblAngDock, 2);
                            double angBase2 = System.Math.Round(dblAngBase, 2);

                            double modAngles = 0;
                            double angDiff = System.Math.Round(angBase2 - angDock2, 2);

                            if (angDiff == 0)
                            {
                                intSlopeSign = 1;
                            }
                            else
                            {
                                if (angBase2 > angDock2)
                                    modAngles = angBase2.mod(angDock2);
                                else
                                    modAngles = angDock2.mod(angBase2);

                                if (modAngles == 0)
                                {
                                    if (angDiff > 0)
                                        intSlopeSign = 1;
                                    else if (angDiff < 0)
                                        intSlopeSign = -1;
                                }

                                if (System.Math.Abs(angDiff) == System.Math.Round(pi / 2, 2))
                                    intSlopeSign = 0;
                            }
                        }
                        if (intSlopeSign != 0)
                        { // sloped floor
                            dblAngDock = dblAngDock - pi / 2;
                            pnt3dA = pnt3dA.traverse(dblAngDock, width, -0.01);
                            pnts3d.Add(pnt3dA);           //CgPnt 2

                            dblAngDock = dblAngDock + pi / 2;
                            pnt3dA = pnt3dA.traverse(dblAngDock, dblLenDock, dblSlope * intSlopeSign * 1);
                            pnts3d.Add(pnt3dA);           //Pnt3

                            dblAngDock = dblAngDock + pi / 2;
                            pnt3dA = pnt3dA.traverse(dblAngDock, width, 0.01);
                            pnts3d.Add(pnt3dA);           //CgPnt 4

                            dblAngDock = dblAngDock + pi / 2;
                            pnt3dA = pnt3dA.traverse(dblAngDock, dblLenDock, dblSlope * intSlopeSign * -1);
                            pnts3d.Add(pnt3dA);           //Pnt5

                            idPoly3d = pnts3d.build3dPolyDockApron("CPNT-ON", nameLayer, "GD", out idsCgPnts);
                            idsPoly3dX.Add(idPoly3d);
                        }
                        else
                        {
                            dblAngDock = dblAngDock - pi / 2;
                            pnt3dA = pnt3dA.traverse(dblAngDock, width, -0.005);

                            pnts3d.Add(pnt3dA);     //Pnt2

                            int intDivide = (int)System.Math.Truncate(dblLenDock / 84) + 1;

                            if (intDivide % 2 != 0)
                            {
                                intDivide = intDivide + 1;
                            }

                            int x = 0;
                            dblAngDock = dblAngDock + pi / 2;
                            double seg = dblLenDock / intDivide;
                            int updown = 1;
                            for (x = 0; x <= intDivide - 1; x++)
                            {
                                updown = -updown;
                                pnt3dA = pnt3dA.traverse(dblAngDock, seg, 0.005 * updown);
                                pnts3d.Add(pnt3dA);
                            }

                            dblAngDock = dblAngDock + pi / 2;
                            pnt3dA = pnt3dA.traverse(dblAngDock, width, 0.005);
                            pnts3d.Add(pnt3dA);

                            dblAngDock = dblAngDock + pi / 2;

                            for (x = 0; x <= intDivide - 1; x++)
                            {
                                pnt3dA = pnt3dA.traverse(dblAngDock, dblLenDock / intDivide, 0.0);
                                pnts3d.Add(pnt3dA);
                            }

                            idPoly3d = pnts3d.build3dPolyDockApron("CPNT-ON", nameLayer, "GD", out idsCgPnts);
                            idsPoly3dX.Add(idPoly3d);

                            List<ObjectId> idCgPntsX = new List<ObjectId>();

                            idCgPntsX.Add(idsCgPnts[1]);     //CgPnt 2 at dock limit away from building

                            int intUBnd = idsCgPnts.Count;
                            x = -1;
                            int n = 1;
                            int k = intUBnd / 2;
                            for (int j = 1; j <= k - 1; j++)
                            {
                                x = -x;
                                n = n + (intUBnd - 2 * j) * x;
                                System.Diagnostics.Debug.Print(string.Format("{0},{1}", j, n));
                                idCgPntsX.Add(idsCgPnts[n]);
                            }
                            for (int i = 1; i < idCgPntsX.Count; i++)
                            {
                                List<ObjectId> idsCogoPnts = new List<ObjectId> { idCgPntsX[i - 1], idCgPntsX[i - 0] };
                                idPoly3d = BrkLine.makeBreakline(apps.lnkBrks, "GD", out idPoly, idsCogoPnts);
                                idsPoly3dX.Add(idPoly3d);
                            }
                        }
                        Grading_Floor.modSurface("CPNT-ON", "Finish Surface", idsPoly3dX, false);
                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(ex.Message + " cmdGD_old.cs: line: 238");
                }
            }
        }

        public static List<Point3d>
        getDockLimits()
        {
            List<Point3d> pnts3d = new List<Point3d>();

            Point3d pnt3dPick;

            Entity ent = Select.selectEntity(typeof(Polyline3d), "\nSelect Finish Floor 3d Polyline: ",
                "3d Polyline selection failed.  Exiting...", out pnt3dPick);

            Polyline3d poly3d = (Polyline3d)ent;

            ResultBuffer rb = poly3d.ObjectId.getXData("AVG");
            if (rb == null)
                return pnts3d;
            TypedValue[] tvs = rb.AsArray();
            bldgNum = tvs[2].Value.ToString();

            using (BaseObjs._acadDoc.LockDocument())
            {
                Object selectCycling = Application.GetSystemVariable("SELECTIONCYCLING");
                Application.SetSystemVariable("SELECTIONCYCLING", 0);
                Object osMode = SnapMode.getOSnap();
                SnapMode.setOSnap((int)osModes.END);
                PromptStatus ps;

                try
                {
                    bool escape = false;
                    Point3d pnt3dRet = UserInput.getPoint("\nSelect Dock Begin point (CCW orientation)", Pub.pnt3dO, out escape, out ps, osMode: 1);
                    if (escape)
                        return pnts3d;
                    pnts3d.Add(pnt3dRet);

                    pnt3dRet = UserInput.getPoint("\nSelect Dock End point (CCW orientation)", pnt3dRet, out escape, out ps, osMode: 1);
                    if (escape)
                        return pnts3d;
                    pnts3d.Add(pnt3dRet);
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(ex.Message + " cmdGD_old.cs: line: 284");
                }
                finally
                {
                    Application.SetSystemVariable("SELECTIONCYCLING", selectCycling);
                    SnapMode.setOSnap((int)osMode);
                }
            }
            return pnts3d;
        }

        public static void
        resetDockLimits()
        {
            bool exists = false;
            try
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    try
                    {
                        ObjectId idDictGRADEDOCK = Dict.getNamedDictionary("GRADEDOCK", out exists);
                        Dict.removeSubEntry(idDictGRADEDOCK, "DOCKS");
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(ex.Message + " cmdGD_old.cs: line: 310");
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdGD_old.cs: line: 316");
            }
        }
    }
}
