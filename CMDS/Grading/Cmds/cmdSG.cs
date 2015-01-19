using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Grading.Cmds
{
    public static class cmdSG
    {
        public static myForms.frmSG fSG = new myForms.frmSG();

        private static double PI = System.Math.PI;

        private static Point3d pnt3dSegBeg { get; set; }

        private static Point3d pnt3dSegEnd { get; set; }

        private static Point3d pnt3dSegPnt { get; set; }

        public static void SG()
        {
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(fSG);
            fSG.nameCmd = "cmdSG";
        }

        public static Boolean
        SG(double B1Width, double B1Slope,
            double B2Width, double B2Slope,
            double slope, int interval,
            string surfTAR, string surfDES,
            int side = 0,
            double elev = 0.0,
            Polyline3d poly3d = null)
        {
            Boolean success = false;
            Boolean doFirst = true;

            Boolean doB1 = fSG.boolB1;
            Boolean doB2 = fSG.boolB1;

            List<Point3d> pnts3d = new List<Point3d>();
            List<Point3d> pnts3dB1 = new List<Point3d>();
            List<Point3d> pnts3dB2 = new List<Point3d>();
            List<Point3d> pnts3dDL = new List<Point3d>();
            List<Point3d> pnts3dRF = new List<Point3d>();

            Point3d pnt3dA, pnt3dB, pnt3dC;
            BaseObjs.acadActivate();
            ObjectId idPoly3d = ObjectId.Null;

            if (poly3d == null)
            {
                idPoly3d = Select.getBrkLine("\nSelect 3D Polyline Reference: ");
            }
            else
            {
                idPoly3d = poly3d.ObjectId;
            }
            List<SEG_PROP> segProps = new List<SEG_PROP>();

            bool isClosed = idPoly3d.checkIfClosed3d(false);
            Point3d pnt3dB1_Mb = Pub.pnt3dO, pnt3dB2_Mb = Pub.pnt3dO, pnt3dDL_Mb = Pub.pnt3dO, pnt3dRF_Mb = Pub.pnt3dO;

            try
            {
                if (idPoly3d != ObjectId.Null)
                {
                    pnts3d = idPoly3d.getCoordinates3dList();
                    segProps = pnts3d.getPoly3dSegProps();

                    if (side == 0)
                        side = Geom.getSide(pnts3d);
                    int k = pnts3d.Count;
                    switch (k)
                    {
                        case 2:
                            pnt3dA = pnts3d[0];
                            pnt3dB = pnts3d[1];
                            success = setupSlope(side, pnt3dA, pnt3dB, slope, ref doFirst, surfTAR, surfDES,
                                ref pnts3dB1, ref pnts3dB2, ref pnts3dDL, ref pnts3dRF,
                                B1Width, B2Width, B1Slope, B2Slope, doB1, doB2, interval, elev);
                            break;

                        default:
                            for (int i = 0; i < k - 1; i++)
                            {
                                if (i == 0)
                                {
                                    pnt3dA = pnts3d[k - 2]; //second from last point
                                    pnt3dB = pnts3d[k - 1]; //last point = first point
                                    pnt3dC = pnts3d[1];     //second point

                                    doMidDelta(side, pnt3dA, pnt3dB, pnt3dC, doB1, B1Width, B1Slope, doB2, B2Width, B2Slope,
                                        surfTAR, elev, slope, ref pnt3dB1_Mb, ref pnt3dB2_Mb, ref pnt3dDL_Mb);

                                    pnt3dA = pnts3d[i + 0];
                                    pnt3dB = pnts3d[i + 1];
                                    pnt3dC = pnts3d[i + 2];

                                    doMidDelta(side, pnt3dA, pnt3dB, pnt3dC, doB1, B1Width, B1Slope, doB2, B2Width, B2Slope,
                                        surfTAR, elev, slope, ref pnt3dB1_Mb, ref pnt3dB2_Mb, ref pnt3dDL_Mb);
                                }
                                else if (i < k - 2)
                                {
                                    pnt3dA = pnts3d[i + 0];
                                    pnt3dB = pnts3d[i + 1];
                                    pnt3dC = pnts3d[i + 2];

                                    doMidDelta(side, pnt3dA, pnt3dB, pnt3dC, doB1, B1Width, B1Slope, doB2, B2Width, B2Slope,
                                        surfTAR, elev, slope, ref pnt3dB1_Mb, ref pnt3dB2_Mb, ref pnt3dDL_Mb);
                                }
                                else
                                {
                                    pnt3dA = pnts3d[i + 0];
                                    pnt3dB = pnts3d[i + 1];
                                    pnt3dC = pnts3d[i + 2];
                                }
                            }
                            break;
                    }
                }
            }
            catch (SystemException ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdSG.cs: line: 129");
                success = false;
            }
            ObjectId idPoly3dX = ObjectId.Null;
            ObjectIdCollection idsPoly3dX = new ObjectIdCollection();
            TypedValue[] tvs = new TypedValue[13];
            tvs.SetValue(new TypedValue(1001, "lnkSLP"), 0);
            using (BaseObjs._acadDoc.LockDocument())
            {
                try
                {
                    idPoly3d.updateBrkLine(pnts3dRF);                   //Grading reference line

                    Handle hB1;
                    if (pnts3dB1.Count > 0)
                    {
                        idPoly3dX = pnts3dB1.addPoly3d("CPNT-BRKLINE"); //Lower Bench
                        idsPoly3dX.Add(idPoly3dX);
                        hB1 = idPoly3dX.getHandle();
                    }
                    else
                    {
                        B1Width = 0.0;
                        B1Slope = 0.0;
                        hB1 = "0".stringToHandle();
                    }
                    tvs.SetValue(new TypedValue(1000, B1Width), 1);
                    tvs.SetValue(new TypedValue(1000, B1Slope), 2);
                    tvs.SetValue(new TypedValue(1005, hB1), 3);

                    Handle hB2;
                    if (pnts3dB2.Count > 0)
                    {
                        idPoly3dX = pnts3dB2.addPoly3d("CPNT-BRKLINE"); //Top of Slope/Upper Bench
                        idsPoly3dX.Add(idPoly3dX);
                        hB2 = idPoly3dX.getHandle();
                    }
                    else
                    {
                        B2Width = 0.0;
                        B2Slope = 0.0;
                        hB2 = "0".stringToHandle();
                    }
                    tvs.SetValue(new TypedValue(1000, B2Width), 4);
                    tvs.SetValue(new TypedValue(1000, B2Slope), 5);
                    tvs.SetValue(new TypedValue(1005, hB2), 6);

                    Handle hDL;
                    idPoly3dX = pnts3dDL.addPoly3d("CPNT-BRKLINE"); //Top of Slope/Daylight (in no upper bench)
                    idsPoly3dX.Add(idPoly3dX);

                    hDL = idPoly3dX.getHandle();
                    tvs.SetValue(new TypedValue(1000, slope), 7);
                    tvs.SetValue(new TypedValue(1070, side), 8);
                    tvs.SetValue(new TypedValue(1000, interval), 9);
                    tvs.SetValue(new TypedValue(1005, hDL), 10);
                    tvs.SetValue(new TypedValue(1000, surfTAR), 11);
                    tvs.SetValue(new TypedValue(1000, surfDES), 12);

                    idPoly3d.setXData(tvs, apps.lnkSLP);
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(ex.Message + " cmdSG.cs: line: 192");
                }
            }
            bool exists = false;
            if (idPoly3dX != ObjectId.Null)
            {
                DialogResult result;
                result = MessageBox.Show(string.Format("Add Slope Breaklines to Design Surface? \nYes to add to surface: {0}\nNo to keep Breaklines, but not add to surface.\nCancel to dispose of Breaklines.", surfDES),
                    "Slope Breaklines",
                    System.Windows.Forms.MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.Yes:
                        TinSurface surf = Surf.getTinSurface(surfDES, out exists);
                        surf.BreaklinesDefinition.AddStandardBreaklines(idsPoly3dX, 1.0, 1.0, 0.0, 0.0);
                        success = true;
                        break;

                    case DialogResult.No:
                        success = true;
                        break;

                    case DialogResult.Cancel:
                        success = false;
                        break;
                }
            }
            else
            {
                success = false;
            }

            return success;
        }


        public static Boolean
        setupSlope(int intSide, Point3d pnt3dA, Point3d pnt3dB, double slope, ref Boolean doFirst, string surfaceTAR, string surfaceDES,
            ref List<Point3d> pnts3dB1, ref List<Point3d> pnts3dB2, ref List<Point3d> pnts3dD, ref List<Point3d> pnts3dR, double B1Width,
            double B2Width, double B1Slope, double B2Slope, bool doB1, bool doB2, int interval, double elev = 0.0)
        {
            Boolean boolSuccess = false;

            double dblAngAB = pnt3dA.getDirection(pnt3dB);

            Point3d pnt3dX;
            bool exists = false;

            List<Point3d> pnts3dT = new List<Point3d>();

            double dblAng = dblAngAB + PI / 2 * intSide;
            pnts3dT = getIntermediates(pnt3dA, pnt3dB, dblAngAB, interval);
            for (int i = 0; i < pnts3dT.Count; i++)
            {
                pnts3dR.Add(pnts3dT[i]);
            }

            for (int i = 0; i < pnts3dT.Count; i++)
            {
                pnt3dX = pnts3dT[i];
                if (doB1)
                {
                    pnt3dX = pnt3dX.traverse(dblAng, B1Width, B1Slope);
                    pnts3dB1.Add(pnt3dX);
                }
                if (doB2)
                {
                    pnt3dX = pnt3dX.traverse(dblAng, B2Width, B2Slope);
                }

                Vector3d v3d = new Vector3d(System.Math.Cos(dblAng), System.Math.Sin(dblAng), slope);
                Point3d pnt3d0 = Pub.pnt3dO;

                if (surfaceTAR != "")
                {
                    pnt3d0 = getSlopeIntercept(surfaceTAR, pnt3dX, slope, exists, v3d);
                }
                else
                {
                    try
                    {
                        pnt3d0 = getTargetElevation(elev, pnt3dX, dblAng, slope);
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(ex.Message + " cmdSG.cs: line: 277");
                    }
                }

                pnts3dD.Add(pnt3d0);

                if (doB2)
                {
                    pnt3dX = pnt3d0.traverse(dblAng + PI, B2Width, -B2Slope);
                    pnts3dB2.Add(pnt3dX);
                }
            }

            return boolSuccess;
        }

        private static void
        doMidDelta(int intSide, Point3d pnt3dA, Point3d pnt3dB, Point3d pnt3dC,
            bool doB1, double B1Width, double B1Slope, bool doB2, double B2Width, double B2Slope,
            string surfaceTAR, double elev, double slope,
            ref Point3d pnt3dB1_M, ref Point3d pnt3dB2_M, ref Point3d pnt3dDL_M)
        {
            using (BaseObjs._acadDoc.LockDocument())
            {
                double dblAngBC = pnt3dB.getDirection(pnt3dC);
                Vector3d v3d1 = pnt3dB - pnt3dA;
                Vector3d v3d2 = pnt3dC - pnt3dB;
                double dblAngABC = v3d1.getAngle2Vectors(v3d2);

                uint pntNum = 0;
                double dblAngV = dblAngBC + intSide * dblAngABC / 2;
                if (doB1 == true)
                {
                    pnt3dB1_M = pnt3dB.traverse(dblAngV, B1Width, B1Slope);
                    pnt3dB1_M.setPoint(out pntNum, "CPNT-ON");
                    BaseObjs.updateGraphics();
                }
                if (doB2 == true)
                {
                    pnt3dB2_M = pnt3dB.traverse(dblAngV, B2Width, B2Slope);  //temporary point
                    pnt3dB2_M.setPoint(out pntNum, "CPNT-ON");
                    BaseObjs.updateGraphics();
                }

                if (surfaceTAR != "")
                {
                    pnt3dDL_M = getTargetElevation(elev, pnt3dB, dblAngV, slope);
                    pnt3dDL_M.setPoint(out pntNum, "CPNT-ON");
                    BaseObjs.updateGraphics();
                }
                else
                {
                    try
                    {
                        pnt3dDL_M = getTargetElevation(elev, pnt3dB, dblAngV, slope);
                        pnt3dDL_M.setPoint(out pntNum, "CPNT-ON");
                        BaseObjs.updateGraphics();
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(ex.Message + " cmdSG.cs: line: 337");
                    }
                }

                if (doB2 == true)
                {
                    pnt3dB2_M = pnt3dDL_M.traverse(dblAngV + PI, B2Width, -B2Slope);
                    pnt3dB2_M.setPoint(out pntNum, "CPNT-ON");
                    BaseObjs.updateGraphics();
                }
            }
        }

        private static Point3d getTargetElevation(double elev, Point3d pnt3dX, double dblAng, double slope)
        {
            Point3d pnt3d0 = Pub.pnt3dO;
            double elevDiff = elev - pnt3dX.Z;
            if (elevDiff > 0)
            {
                pnt3d0 = pnt3dX.traverse(dblAng, elevDiff / slope, slope);
            }
            else
            {
                pnt3d0 = pnt3dX.traverse(dblAng, System.Math.Abs(elevDiff) / slope, -slope);
            }
            return pnt3d0;
        }

        private static Point3d getSlopeIntercept(string surfaceTAR, Point3d pnt3dX, double slope, bool exists, Vector3d v3d)
        {
            Point3d pnt3d0 = Pub.pnt3dO;
            ObjectId idSurf = Surf.getSurface(surfaceTAR, out exists);
            TinSurface surf = idSurf.getEnt() as TinSurface;
            try
            {
                pnt3d0 = surf.GetIntersectionPoint(pnt3dX, v3d);
            }
            catch (System.Exception)
            {
                v3d = new Vector3d(v3d.X, v3d.Y, -slope);
                pnt3d0 = surf.GetIntersectionPoint(pnt3dX, v3d);
            }
            return pnt3d0;
        }

        private static List<Point3d> getIntermediates(Point3d pnt3dA, Point3d pnt3dB, double angAB, int interval)
        {
            List<Point3d> pnts3dX = new List<Point3d>();
            double len = pnt3dA.getDistance(pnt3dB);
            int num = (int)System.Math.Truncate(len / interval);
            double slope = (pnt3dB.Z - pnt3dA.Z) / len;

            for (int i = 0; i < num; i++)
            {
                pnts3dX.Add(pnt3dA.traverse(angAB, i * interval, slope));
            }

            pnts3dX.Add(pnt3dB);

            return pnts3dX;
        }// end  getIntermediates
    }//end class slopeGrading
}
