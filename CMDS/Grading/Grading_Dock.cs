using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Globalization;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Base_Tools45
{
    public static class Grading_Dock
    {
        public static string bldgNum { get; set; }

        public static void
        getDockLimits(int intNumDocks)
        {
            bool exists = false;
            ObjectId idDictGRADEDOCK = Dict.getNamedDictionary("GRADEDOCK", out exists);
            ObjectId idDictX = default(ObjectId);

            if ((idDictGRADEDOCK == ObjectId.Null))
            {
                Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog("GRADEDOCK dictionary missing - run AVG");
            }
            Point3d pnt3dPick;

            Entity ent = Select.selectEntity(typeof(Polyline3d), "\nSelect Finish Floor 3d Polyline: ",
                "3d Polyline selection failed.  Exiting...", out pnt3dPick);

            Polyline3d poly3d = (Polyline3d)ent;

            ResultBuffer rb = poly3d.ObjectId.getXData("AVG");
            if (rb == null)
                return;
            TypedValue[] tvs = rb.AsArray();

            bldgNum = tvs[2].Value.ToString();

            ObjectId idDictBLDG = Dict.getSubDict(idDictGRADEDOCK, bldgNum);

            ObjectId idDictDOCKS = Dict.getSubEntry(idDictBLDG, "DOCKS");
            if (idDictDOCKS == ObjectId.Null)
            {
                idDictDOCKS = Dict.addSubDict(idDictBLDG, "DOCKS");
            }

            using (BaseObjs._acadDoc.LockDocument())
            {
                Object selectCycling = Application.GetSystemVariable("SELECTIONCYCLING");
                Application.SetSystemVariable("SELECTIONCYCLING", 0);
                PromptStatus ps;
                try
                {
                    for (int i = 1; i < intNumDocks + 1; i++)
                    {
                        idDictX = Dict.addSubDict(idDictDOCKS, i.ToString(CultureInfo.InvariantCulture));

                        bool escape = false;
                        Point3d pnt3dRet = UserInput.getPoint(string.Format("\nSelect Dock {0} : Begin point (CCW orientation)", i), Pub.pnt3dO, out escape, out ps, osMode: 0);
                        BaseObjs.write("\n");

                        TypedValue[] TVs = new TypedValue[] {
                            new TypedValue(1040, pnt3dRet.X),
                            new TypedValue(1040, pnt3dRet.Y),
                            new TypedValue(1040, pnt3dRet.Z)
                        };

                        Dict.addXRec(idDictX, "BEG", TVs);

                        pnt3dRet = UserInput.getPoint(string.Format("\nSelect Dock {0} : End point (CCW orientation)", i), pnt3dRet, out escape, out ps, osMode: 0);
                        BaseObjs.write("\n");

                        TVs = new TypedValue[] {
                            new TypedValue(1040, pnt3dRet.X),
                            new TypedValue(1040, pnt3dRet.Y),
                            new TypedValue(1040, pnt3dRet.Z)
                        };

                        Dict.addXRec(idDictX, "END", TVs);
                    }
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(ex.Message + " Grading_Dock.cs: line: 85");
                }
                finally
                {
                    Application.SetSystemVariable("SELECTIONCYCLING", selectCycling);
                }
            }
        }

        public static void
        gradeDock(double dblWidth, double dblWallThickness)
        {
            const string nameLayer = "CPNT-BRKLINE";

            double dblAng = 0;
            double dblLEN = 0;

            Point3d pnt3dA = default(Point3d);          //point AHEAD
            Point3d pnt3dB;                             //point BACK

            int intSlopeSign = 0;

            const double pi = System.Math.PI;

            bool exists = false;
            ObjectId idDictGRADEDOCK = Dict.getNamedDictionary("GRADEDOCK", out exists);
            ObjectId idDictDOCKS = default(ObjectId);

            if ((idDictGRADEDOCK == ObjectId.Null))
            {
                Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog("Dock limits not yet defined  - exiting .....");
            }

            ObjectId idDictBLDG = Dict.getSubDict(idDictGRADEDOCK, bldgNum);

            TypedValue[] TVs = new TypedValue[3];
            using (BaseObjs._acadDoc.LockDocument())
            {
                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        ResultBuffer RB = Dict.getXRec(idDictBLDG, "HANDLE3D");
                        TVs = RB.AsArray();

                        RB = Dict.getXRec(idDictBLDG, "SLOPE");
                        TVs = RB.AsArray();
                        double dblSlope = (double)TVs[0].Value;

                        RB = Dict.getXRec(idDictBLDG, "CENtrOID");
                        TVs = RB.AsArray();
                        Point3d pnt3dCEN = new Point3d((double)TVs[0].Value, (double)TVs[1].Value, (double)TVs[2].Value);

                        RB = Dict.getXRec(idDictBLDG, "TARGET");
                        TVs = RB.AsArray();
                        Point3d pnt3dTAR = new Point3d((double)TVs[0].Value, (double)TVs[1].Value, (double)TVs[2].Value);

                        double dblAngBase = Measure.getAzRadians(pnt3dCEN, pnt3dTAR);

                        idDictDOCKS = Dict.getSubEntry(idDictBLDG, "DOCKS");

                        if ((idDictDOCKS == ObjectId.Null))
                        {
                            Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog("Dock limits not yet defined  - exiting .....");
                            return;
                        }

                        DBDictionary objDictDOCKS = (DBDictionary)tr.GetObject(idDictDOCKS, OpenMode.ForRead);

                        for (int i = 1; i < objDictDOCKS.Count + 1; i++)
                        {
                            ObjectId idDictX = Dict.getSubEntry(idDictDOCKS, i.ToString());
                            ObjectIdCollection idsPoly3dX = new ObjectIdCollection();
                            Point3dCollection pnts3d = new Point3dCollection();

                            try
                            {
                                RB = Dict.getXRec(idDictX, "BEG");
                                TVs = RB.AsArray();

                                Point3d pnt3dBEG = new Point3d((double)TVs[0].Value, (double)TVs[1].Value, (double)TVs[2].Value);

                                RB = Dict.getXRec(idDictX, "END");
                                TVs = RB.AsArray();
                                Point3d pnt3dEND = new Point3d((double)TVs[0].Value, (double)TVs[1].Value, (double)TVs[2].Value);

                                dblAng = Measure.getAzRadians(pnt3dBEG, pnt3dEND);
                                dblLEN = pnt3dBEG.getDistance(pnt3dEND) - (2 * dblWallThickness / 12.0);
                                pnt3dB = pnt3dBEG;

                                pnt3dA = Geom.traverse_pnt3d(pnt3dBEG, dblAng - pi / 2, dblWallThickness / 12.0);
                                pnt3dA = Geom.traverse_pnt3d(pnt3dA, dblAng, dblWallThickness / 12.0);
                                if (pnt3dTAR != Pub.pnt3dO)
                                    pnt3dA = new Point3d(pnt3dA.X, pnt3dA.Y, pnt3dTAR.Z + Geom.getCosineComponent(pnt3dCEN, pnt3dTAR, pnt3dA) * (dblSlope * -1) - 4.04);
                                else
                                    pnt3dA = new Point3d(pnt3dA.X, pnt3dA.Y, pnt3dCEN.Z - 4.04);

                                pnts3d.Add(pnt3dA);           //CgPnt 1
                            }
                            catch (System.Exception ex)
                            {
                                BaseObjs.writeDebug(ex.Message + " Grading_Dock.cs: line: 186");
                            }

                            ObjectId idPoly3d = ObjectId.Null;

                            if (dblSlope != 0)
                            {
                                if (System.Math.Round(dblAng, 2) == System.Math.Round(dblAngBase, 2))
                                    intSlopeSign = 1;
                                if (System.Math.Round(dblAng, 2) == System.Math.Round(dblAngBase - pi, 2))
                                    intSlopeSign = -1;
                                if (System.Math.Round(dblAng, 2) == System.Math.Round(dblAngBase + pi, 2))
                                    intSlopeSign = -1;
                                if (System.Math.Round(dblAng, 2) == System.Math.Round(dblAngBase - 2 * pi, 2))
                                    intSlopeSign = 1;

                                dblAng = dblAng - pi / 2;
                                pnt3dB = pnt3dA;
                                pnt3dA = Geom.traverse_pnt3d(pnt3dB, dblAng, dblWidth);
                                pnt3dA = new Point3d(pnt3dA.X, pnt3dA.Y, pnt3dB.Z + dblWidth * -0.01);

                                pnts3d.Add(pnt3dA);           //CgPnt 2

                                dblAng = dblAng + pi / 2;
                                pnt3dB = pnt3dA;
                                pnt3dA = Geom.traverse_pnt3d(pnt3dB, dblAng, dblLEN);
                                pnt3dA = new Point3d(pnt3dA.X, pnt3dA.Y, pnt3dB.Z + dblLEN * dblSlope * intSlopeSign * 1);

                                pnts3d.Add(pnt3dA);           //Pnt3

                                dblAng = dblAng + pi / 2;
                                pnt3dB = pnt3dA;
                                pnt3dA = Geom.traverse_pnt3d(pnt3dB, dblAng, dblWidth);
                                pnt3dA = new Point3d(pnt3dA.X, pnt3dA.Y, pnt3dB.Z + dblWidth * 0.01);

                                pnts3d.Add(pnt3dA);           //CgPnt 4

                                dblAng = dblAng + pi / 2;
                                pnt3dB = pnt3dA;
                                pnt3dA = Geom.traverse_pnt3d(pnt3dB, dblAng, dblLEN);
                                pnt3dA = new Point3d(pnt3dA.X, pnt3dA.Y, pnt3dB.Z + dblLEN * dblSlope * intSlopeSign * -1);

                                pnts3d.Add(pnt3dA);           //Pnt5

                                idPoly3d = Draw.addPoly3d(pnts3d, nameLayer);
                                idsPoly3dX.Add(idPoly3d);
                            }
                            else
                            {
                                dblAng = dblAng - pi / 2;
                                pnt3dB = pnt3dA;
                                pnt3dA = Geom.traverse_pnt3d(pnt3dB, dblAng, dblWidth);
                                pnt3dA = new Point3d(pnt3dA.X, pnt3dA.Y, pnt3dB.Z + dblWidth * -0.005);

                                pnts3d.Add(pnt3dA);

                                int intDivide = (int)System.Math.Truncate(dblLEN / 84) + 1;

                                if (intDivide % 2 != 0)
                                {
                                    intDivide = intDivide + 1;
                                }

                                int x = 0;
                                for (x = 0; x <= intDivide - 1; x++)
                                {
                                    int intMultiplier = 0;
                                    switch (x % 2)
                                    {
                                        case 0:
                                            intMultiplier = -1;
                                            break;

                                        default:
                                            intMultiplier = 1;
                                            break;
                                    }

                                    if (x == 0)
                                    {
                                        dblAng = dblAng + pi / 2;
                                    }

                                    pnt3dB = pnt3dA;
                                    pnt3dA = Geom.traverse_pnt3d(pnt3dB, dblAng, dblLEN / intDivide);
                                    pnt3dA = new Point3d(pnt3dA.X, pnt3dA.Y, pnt3dB.Z + dblLEN / intDivide * 0.005 * intMultiplier);

                                    pnts3d.Add(pnt3dA);
                                }

                                dblAng = dblAng + pi / 2;
                                pnt3dB = pnt3dA;
                                pnt3dA = Geom.traverse_pnt3d(pnt3dB, dblAng, dblWidth);
                                pnt3dA = new Point3d(pnt3dA.X, pnt3dA.Y, pnt3dB.Z + dblWidth * 0.005);

                                pnts3d.Add(pnt3dA);

                                for (x = 0; x <= intDivide - 1; x++)
                                {
                                    if (x == 0)
                                    {
                                        dblAng = dblAng + pi / 2;
                                    }

                                    pnt3dB = pnt3dA;
                                    pnt3dA = Geom.traverse_pnt3d(pnt3dB, dblAng, dblLEN / intDivide);
                                    pnt3dA = new Point3d(pnt3dA.X, pnt3dA.Y, pnt3dB.Z);

                                    pnts3d.Add(pnt3dA);
                                }

                                idPoly3d = Draw.addPoly3d(pnts3d, nameLayer);
                                idsPoly3dX.Add(idPoly3d);

                                Point3dCollection pnts3dX = new Point3dCollection();

                                pnts3dX.Add(pnts3d[0]);

                                int intUBnd = pnts3d.Count - 1;
                                x = -1;
                                int n = 1;

                                for (int j = 1; j <= intUBnd / 2 - 1; j++)
                                {
                                    x = x * -1;
                                    n = n + (intUBnd - 2 * j) * x;

                                    pnts3dX.Add(pnts3d[n]);
                                }

                                idPoly3d = Draw.addPoly3d(pnts3d, nameLayer);
                                idsPoly3dX.Add(idPoly3d);
                            }
                            Grading_Floor.modSurface("CPNT-ON", "Finish Surface", idsPoly3dX, false);
                        }
                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(ex.Message + " Grading_Dock.cs: line: 326");
                }
            }
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
                        BaseObjs.writeDebug(ex.Message + " Grading_Dock.cs: line: 346");
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Grading_Dock.cs: line: 352");
            }
        }
    }
}
