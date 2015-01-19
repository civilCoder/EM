using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using sMath = System.Math;

namespace Grading.Cmds
{
    public static class cmdBD
    {
        public static myForms.frmSG fSG = new myForms.frmSG();

        private static double PI = sMath.PI;

        private static Point3d pnt3dSegBeg { get; set; }

        private static Point3d pnt3dSegEnd { get; set; }

        private static Point3d pnt3dSegPnt { get; set; }

        public static void BD()
        {
            fSG = null;
            fSG = new myForms.frmSG();
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(fSG);
            fSG.nameCmd = "cmdBD";
        }

        public static Boolean
        BD(double B1Width, double B1Slope,
            double B2Width, double B2Slope,
            double gradeSlope, int interval,
            string surfTAR, string surfDES,
            int side = 0,
            double elev = 0.0,
        Polyline3d poly3d = null)
        {
            Boolean success = false;
            Boolean exists = false;

            Boolean doB1 = fSG.boolB1;
            Boolean doB2 = fSG.boolB1;

            List<Point3d> pnts3d = new List<Point3d>();
            List<Point3d> pnts3dB1 = new List<Point3d>();
            List<Point3d> pnts3dB2 = new List<Point3d>();
            List<Point3d> pnts3dDL = new List<Point3d>();
            List<Point3d> pnts3dRF = new List<Point3d>();

            Vector3d v3d = Vector3d.XAxis;

            Point3d pnt3dB;
            BaseObjs.acadActivate();

            ObjectId idPoly3dOrg = ObjectId.Null;
            ObjectId idPoly3d = ObjectId.Null;
            ObjectId idPoly = ObjectId.Null;

            if (poly3d == null)
            {
                idPoly3dOrg = Select.getBrkLine("\nSelect 3D Polyline Reference: ");
            }
            else
            {
                idPoly3dOrg = poly3d.ObjectId;
            }

            if (idPoly3dOrg == ObjectId.Null)
                return false;

            ResultBuffer rb = idPoly3dOrg.getXData(apps.lnkBrks);
            if (rb == null)
                return false;

            TypedValue[] tvs = rb.AsArray();
            List<ObjectId> idsCgPntRF = tvs.getObjectIdList();
            List<ObjectId> idsCgPntM = null;

            ObjectId idCgPnt = ObjectId.Null;

            bool isClosed = idPoly3dOrg.checkIfClosed3d(false);
            if (!isClosed)
            {
                Application.ShowAlertDialog("Design reference Breakline is not closed. Exiting...");
                return false;
            }
            Point3d pnt3dB1_Mb = Pub.pnt3dO, pnt3dB2_Mb = Pub.pnt3dO, pnt3dDL_Mb = Pub.pnt3dO, pnt3dRF_Mb = Pub.pnt3dO;
            Point3d pnt3dB1_Me = Pub.pnt3dO, pnt3dB2_Me = Pub.pnt3dO, pnt3dDL_Me = Pub.pnt3dO, pnt3dRF_Me = Pub.pnt3dO;
            Point3d pnt3dX = Pub.pnt3dO, pnt3dXA = Pub.pnt3dO, pnt3dXB = Pub.pnt3dO;

            uint pntNum = 0;
            int s = 0;
            double angle = 0.0, deflc = 0.0, delta = 0.0, slopeChk = 0.0;

            List<ANG_PT_PROP> angPtProps = new List<ANG_PT_PROP>();
            ANG_PT_PROP angPtProp = null;
            try
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    pnts3d = idPoly3dOrg.getCoordinates3dList();

                    if (side == 0)
                        side = Geom.getSide(pnts3d);

                    angPtProps = pnts3d.getPoly3dAngPtProps();

                    int k = angPtProps.Count;
                    for (int i = 0; i < k; i++)
                    {
                        angPtProp = angPtProps[i];

                        pnt3dB = angPtProp.BEG;     //last point = first point

                        deflc = angPtProp.ANG_DEFLC;

                        if (i == 0)
                            pnts3dRF.Add(pnt3dB);       //add begin point


                        angle = getMidDeltaDirection(side, deflc, angPtProp.DIR_SEG1, angPtProp.DIR_SEG2, out s);


                        if (s == 0)
                        {
                            if (surfTAR == "")
                            {
                                pnt3dDL_Mb = getTargetElevation(elev, pnt3dB, angle, gradeSlope);
                            }
                            else
                            {
                                v3d = new Vector3d(sMath.Cos(angle), sMath.Sin(angle), -gradeSlope);
                                pnt3dDL_Mb = getSlopeIntercept(surfTAR, pnt3dB, gradeSlope, v3d);
                            }

                            pnts3dDL.Add(pnt3dDL_Mb);

                            idCgPnt = pnt3dDL_Mb.setPoint(out pntNum);
                            BaseObjs.updateGraphics();

                            idsCgPntM = new List<ObjectId> { idsCgPntRF[i], idCgPnt };
                            idPoly3d = BrkLine.makeBreakline(apps.lnkSLP, "cmdBD", out idPoly, idsCgPntM);
                            BaseObjs.updateGraphics();
                        }
                        else if (s == -2 || s == 4)
                        {
                            getToeIntercept(side, angPtProp, surfTAR, elev, gradeSlope, doB1, B1Width, B1Slope, doB2, B2Width, B2Slope, ref pnt3dDL_Mb, ref pnt3dXA, ref pnt3dXB);

                            if (i == 0)
                            {
                                pnt3dDL_Me = pnt3dXB;
                            }
                            else
                            {
                                pnts3dRF.Add(pnt3dXB);
                                pnt3dXB.setPoint(out pntNum, "CPNT-ON");
                                BaseObjs.updateGraphics();
                            }

                            slopeChk = pnt3dDL_Mb.getSlope(pnt3dXB);
                            Debug.Print(slopeChk.ToString("0.0000"));

                            pnts3dRF.Add(pnt3dB);          //add RF Angle point

                            pnts3dRF.Add(pnt3dXA);
                            pnt3dXA.setPoint(out pntNum, "CPNT-ON");
                            BaseObjs.updateGraphics();

                            slopeChk = pnt3dDL_Mb.getSlope(pnt3dXA);
                            Debug.Print(slopeChk.ToString("0.0000"));

                            idCgPnt = pnt3dDL_Mb.setPoint(out pntNum, "CPNT-ON");
                            BaseObjs.updateGraphics();
                            idsCgPntM = new List<ObjectId> { idsCgPntRF[i], idCgPnt };
                            pnts3dDL.Add(pnt3dDL_Mb);

                            idPoly3d = BrkLine.makeBreakline(apps.lnkSLP, "cmdBD", out idPoly, idsCgPntM);
                            BaseObjs.updateGraphics();
                        }
                        else if (s == 2 || s == -4)
                        {
                            pnts3dRF.Add(pnt3dB);

                            double anglePerp1 = angPtProp.DIR_SEG1 + PI / 2 * -side;
                            double anglePerp2 = angPtProp.DIR_SEG2 + PI / 2 * -side;
                            Vector2d v2d1 = new Vector2d(sMath.Cos(anglePerp1), sMath.Sin(anglePerp1));
                            Vector2d v2d2 = new Vector2d(sMath.Cos(anglePerp2), sMath.Sin(anglePerp2));

                            delta = v2d1.GetAngleTo(v2d2);

                            List<double> angles = new List<double>();
                            angles.Add(anglePerp1);
                            double angleX = anglePerp1 + delta / 4 * side;
                            angles.Add(angleX);
                            angles.Add(angle);
                            angleX = angle + delta / 4 * side;
                            angles.Add(angleX);
                            angles.Add(anglePerp2);



                            foreach (double ang in angles)
                            {
                                angle = ang;
                                if (surfTAR != "")
                                {
                                    v3d = new Vector3d(sMath.Cos(angle), sMath.Sin(angle), -gradeSlope);
                                    pnt3dDL_Mb = getSlopeIntercept(surfTAR, pnt3dB, gradeSlope, v3d);
                                }
                                else
                                    pnt3dDL_Mb = getTargetElevation(elev, pnt3dB, angle, gradeSlope);

                                pnts3dDL.Add(pnt3dDL_Mb);
                                idCgPnt = pnt3dDL_Mb.setPoint(out pntNum, "CPNT-ON");
                                BaseObjs.updateGraphics();

                                idsCgPntM = new List<ObjectId> { idsCgPntRF[i], idCgPnt };

                                idPoly3d = BrkLine.makeBreakline(apps.lnkSLP, "cmdBD", out idPoly, idsCgPntM);
                                BaseObjs.updateGraphics();
                            }
                        }
                    }

                    pnts3dDL.Add(pnts3dDL[0]);  //complete point list with first point - for closure
                    if (pnt3dRF_Me != Pub.pnt3dO)
                    {
                        pnts3dRF.Add(pnt3dRF_Me);   //point on RF opposite DL back - first mid delta
                        pnt3dRF_Me.setPoint(out pntNum, "CPNT-ON");
                        BaseObjs.updateGraphics();
                    }

                    pnts3dRF.Add(pnts3dRF[0]);  //complete point list with first point - for closure
                }
            }
            catch (SystemException ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdBD.cs: line: 244");
                success = false;
            }
            ObjectId idPoly3dX = ObjectId.Null;
            ObjectIdCollection idsPoly3dX = new ObjectIdCollection();
            tvs = new TypedValue[13];
            tvs.SetValue(new TypedValue(1001, "lnkSLP"), 0);
            using (BaseObjs._acadDoc.LockDocument())
            {
                try
                {
                    idPoly3dOrg.handOverPoly3d2(pnts3dRF);

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
                    idPoly3dX = pnts3dDL.addPoly3d("CPNT-BRKLINE"); //Top of Slope/Daylight (if no upper bench)
                    idsPoly3dX.Add(idPoly3dX);

                    hDL = idPoly3dX.getHandle();
                    tvs.SetValue(new TypedValue(1000, gradeSlope), 7);
                    tvs.SetValue(new TypedValue(1070, side), 8);
                    tvs.SetValue(new TypedValue(1000, interval), 9);
                    tvs.SetValue(new TypedValue(1005, hDL), 10);
                    tvs.SetValue(new TypedValue(1000, surfTAR), 11);
                    tvs.SetValue(new TypedValue(1000, surfDES), 12);

                    idPoly3dOrg.setXData(tvs, apps.lnkSLP);
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(ex.Message + " cmdBD.cs: line: 307");
                }
            }

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

        private static void
        getToeIntercept(int side, ANG_PT_PROP angPtProp,
            string surfaceTAR, double elev, double gradeSlope,
            bool doB1, double B1Width, double B1Slope,
            bool doB2, double B2Width, double B2Slope,
            ref Point3d pnt3dDL_M, ref Point3d pnt3dXA, ref Point3d pnt3dXB)
        {
            Point3d pnt3dB = angPtProp.BEG;
            double anglePerpBack = angPtProp.DIR_SEG1 + PI / 2 * -side;
            double anglePerpAhead = angPtProp.DIR_SEG2 + PI / 2 * -side;

            Point3d pnt3dDL_B = Pub.pnt3dO;
            Point3d pnt3dDL_A = Pub.pnt3dO;

            bool stop = false;
            double incre = 0.10;
            double distA = 5.0, distB = 5.0;
            double angA = 0.0, angB = 0.0, distX = 0.0;

            ObjectId idPoly3dB = ObjectId.Null;
            ObjectId idPoly3dA = ObjectId.Null;
            Point3d pnt3dInt = Pub.pnt3dO;
            Point3d pnt3dDL = Pub.pnt3dO;

            Vector3d v3dA = Vector3d.XAxis;
            Vector3d v3dB = v3dA;
            Vector3d v3dM = v3dA;

            do
            {
                distB += incre;
                distA += incre;

                pnt3dXB = pnt3dB.traverse(angPtProp.DIR_SEG1 + PI, distB, -angPtProp.SLP_SEG1);
                pnt3dXA = pnt3dB.traverse(angPtProp.DIR_SEG2, distA, angPtProp.SLP_SEG2);

                if (doB1 == true)
                {
                    pnt3dXB = pnt3dXB.traverse(anglePerpBack, B1Width, B1Slope);
                    pnt3dXA = pnt3dXA.traverse(anglePerpAhead, B1Width, B1Slope);
                }
                if (doB2 == true)
                {
                    pnt3dXB = pnt3dXB.traverse(anglePerpBack, B2Width, B2Slope);
                    pnt3dXA = pnt3dXA.traverse(anglePerpAhead, B2Width, B2Slope);
                }

                pnt3dDL = pnt3dDL_A;
                if (surfaceTAR != "")
                {
                    Vector3d v3d = new Vector3d(sMath.Cos(anglePerpBack), sMath.Sin(anglePerpBack), -gradeSlope);
                    pnt3dDL_B = getSlopeIntercept(surfaceTAR, pnt3dXB, gradeSlope, v3d);

                    v3d = new Vector3d(sMath.Cos(anglePerpAhead), sMath.Sin(anglePerpAhead), -gradeSlope);
                    pnt3dDL_A = getSlopeIntercept(surfaceTAR, pnt3dXA, gradeSlope, v3d);
                }
                else
                {
                    try
                    {
                        pnt3dDL_B = getTargetElevation(elev, pnt3dXB, anglePerpBack, gradeSlope);
                        pnt3dDL_A = getTargetElevation(elev, pnt3dXA, anglePerpAhead, gradeSlope);
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(ex.Message + " cmdBD.cs: line: 408");
                    }
                }



                double lenA = pnt3dXA.getDistance(pnt3dDL_A);
                double lenB = pnt3dXB.getDistance(pnt3dDL_B);

                pnt3dInt = Geom.getPntInt(pnt3dXA, pnt3dDL_A, pnt3dXB, pnt3dDL_B, false, 0);
                if (pnt3dInt == Pub.pnt3dO)
                {
                    if (lenA > lenB)
                    {
                        pnt3dDL_M = pnt3dDL_A;

                        v3dM = (pnt3dDL_M - pnt3dB);
                        v3dB = new Vector3d(sMath.Cos(anglePerpBack), sMath.Sin(anglePerpBack), 0);
                        angB = Geom.getAngle2Vectors(v3dM, v3dB);
                        distX = pnt3dB.getDistance(pnt3dDL_M) * sMath.Sin(angB);
                        pnt3dXB = pnt3dB.traverse(angPtProp.DIR_SEG1 + PI, distX, -angPtProp.SLP_SEG1);
                    }
                    else
                    {
                        pnt3dDL_M = pnt3dDL_B;

                        v3dM = (pnt3dDL_M - pnt3dB);
                        v3dA = new Vector3d(sMath.Cos(anglePerpAhead), sMath.Sin(anglePerpAhead), 0);
                        angA = Geom.getAngle2Vectors(v3dM, v3dA);
                        distX = pnt3dB.getDistance(pnt3dDL_M) * sMath.Sin(angA);
                        pnt3dXA = pnt3dB.traverse(angPtProp.DIR_SEG2, distX, angPtProp.SLP_SEG2);
                    }
                    stop = true;
                }
            }
            while (!stop);
        }

        private static double
        doMidDelta(int side, ANG_PT_PROP angPtProp,
            bool doB1, double B1Width, double B1Slope,
            bool doB2, double B2Width, double B2Slope,
            string surfaceTAR, double elev, double gradeSlope,
            ref Point3d pnt3dB1_M, ref Point3d pnt3dB2_M, ref Point3d pnt3dDL_M, out int s)
        {
            double deflc = angPtProp.ANG_DEFLC;
            Point3d pnt3dB = angPtProp.BEG;
            double angleV = getMidDeltaDirection(side, deflc, angPtProp.DIR_SEG1, angPtProp.DIR_SEG2, out s);

            if (doB1 == true)
            {
                pnt3dB1_M = pnt3dB.traverse(angleV, B1Width / sMath.Cos(deflc / 2), B1Slope * sMath.Cos(deflc / 2));
            }
            if (doB2 == true)
            {
                pnt3dB2_M = pnt3dB.traverse(angleV, B2Width / sMath.Cos(deflc / 2), B2Slope * sMath.Cos(deflc / 2));  //temporary point
            }

            if (surfaceTAR != "")
            {
                Vector3d v3d = new Vector3d(sMath.Cos(angleV), sMath.Sin(angleV), -gradeSlope * sMath.Cos(deflc / 2));
                pnt3dDL_M = getSlopeIntercept(surfaceTAR, pnt3dB, gradeSlope * sMath.Cos(deflc / 2), v3d);
            }
            else
            {
                try
                {
                    pnt3dDL_M = getTargetElevation(elev, pnt3dB, angleV, gradeSlope * sMath.Cos(deflc / 2));
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(ex.Message + " cmdBD.cs: line: 479");
                }
            }

            if (doB2 == true)
            {
                pnt3dB2_M = pnt3dDL_M.traverse(angleV + PI, B2Width, -B2Slope);    //check
            }

            return angleV;
        }

        private static double
        getMidDeltaDirection(int side, double deflc, double dirSeg1, double dirSeg2, out int s)
        {
            int d = 0;  //delta factor
            s = 0;      //situation
            double angle = 0;

            if (sMath.Round(deflc, 4) == 0)
                d = 0;
            else if (deflc > 0)
                d = 2;
            else if (deflc < 0)
                d = 4;

            s = side * d;

            switch (s)
            {
                case 0:
                    angle = dirSeg1 + (PI / 2) * -side;
                    break;

                case -2:     //in
                case 4:     //in
                    angle = dirSeg1 + (PI - sMath.Abs(deflc)) / 2 * -side;
                    break;

                case 2:     //out
                case -4:     //out
                    angle = dirSeg1 + (PI - sMath.Abs(deflc)) / 2 * -side;
                    break;

                default:
                    break;
            }
            return angle;
        }

        private static Point3d
        getTargetElevation(double elev, Point3d pnt3dX, double dblAng, double slope)
        {
            Point3d pnt3d0 = Pub.pnt3dO;
            double elevDiff = elev - pnt3dX.Z;
            if (elevDiff > 0)
            {
                pnt3d0 = pnt3dX.traverse(dblAng, sMath.Abs(elevDiff) / slope, slope);
            }
            else
            {
                pnt3d0 = pnt3dX.traverse(dblAng, sMath.Abs(elevDiff) / slope, -slope);
            }
            return pnt3d0;
        }

        private static Point3d
        getSlopeIntercept(string surfaceTAR, Point3d pnt3dX, double slope, Vector3d v3d)
        {
            bool exists = false;
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
    }//end class slopeGrading
}
