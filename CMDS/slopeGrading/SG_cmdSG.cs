using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

[assembly: CommandClass(typeof(slopeGrading.SG_cmdSG))]

namespace slopeGrading
{
    public class SG_cmdSG
    {
        private static double PI = System.Math.PI;

        public static frmMain fmain = new frmMain();

        //[CommandMethod("GRADING", "SG", CommandFlags.UsePickSet)]
        //public void slopeGrade()
        //{
        //    slopeGrading.SG_cmdSG.cmdSG();
        //}

        public static void cmdSG()
        {
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(fmain);
        }

        public static Boolean SG(double slope, string surfTAR, string surfDES)
        {
            Boolean boolDoFirst = true;
            Boolean success = false;

            List<Point3d> pnts3d = new List<Point3d>();
            List<Point3d> pnts3dB1 = new List<Point3d>();
            List<Point3d> pnts3dB2 = new List<Point3d>();
            List<Point3d> pnts3dD = new List<Point3d>();
            List<Point3d> pnts3dR = new List<Point3d>();

            Point3d pnt3dA, pnt3dB, pnt3dC;
            BaseObjs.acadActivate();

            ObjectId idPoly3d = Select.getBrkLine("\nSelect 3D Polyline Reference: ");
            try
            {
                if (idPoly3d != ObjectId.Null)
                {
                    pnts3d = idPoly3d.getCoordinates3dList();
                    int intSide = SG_Utility.getSide(pnts3d);

                    if (pnts3d.Count > 2)
                    {
                        for (int i = 0; i < pnts3d.Count - 2; i++)
                        {
                            pnt3dA = pnts3d[i + 0];
                            pnt3dB = pnts3d[i + 1];
                            pnt3dC = pnts3d[i + 2];

                            success = setupSlope(intSide, pnt3dA, pnt3dB, pnt3dC, slope, ref boolDoFirst, surfTAR, surfDES, ref pnts3dB1, ref pnts3dB2, ref pnts3dD, ref pnts3dR);
                        }

                        pnt3dB = pnts3d[pnts3d.Count - 2];
                        pnt3dC = pnts3d[pnts3d.Count - 1];

                        success = setupSlope(intSide, pnt3dB, pnt3dC, slope, ref boolDoFirst, surfTAR, surfDES, ref pnts3dB1, ref pnts3dB2, ref pnts3dD, ref pnts3dR);
                    }
                    else
                    {
                        pnt3dA = pnts3d[0];
                        pnt3dB = pnts3d[1];
                        success = setupSlope(intSide, pnt3dA, pnt3dB, slope, ref boolDoFirst, surfTAR, surfDES, ref pnts3dB1, ref pnts3dB2, ref pnts3dD, ref pnts3dR);
                    }
                }
            }
            catch (SystemException)
            {
                success = false;
            }
            ObjectId idPoly3dX = ObjectId.Null;
            ObjectIdCollection idsPoly3dX = new ObjectIdCollection();
            using (BaseObjs._acadDoc.LockDocument())
            {
                try
                {
                    //idPoly3dX = pnts3dR.addPoly3d("CPNT-BRKLINE");
                    //idsPoly3dX.Add(idPoly3dX);

                    idPoly3dX = pnts3dB1.addPoly3d("CPNT-BRKLINE");
                    idsPoly3dX.Add(idPoly3dX);

                    idPoly3dX = pnts3dB2.addPoly3d("CPNT-BRKLINE");
                    idsPoly3dX.Add(idPoly3dX);

                    idPoly3dX = pnts3dD.addPoly3d("CPNT-BRKLINE");
                    idsPoly3dX.Add(idPoly3dX);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            } bool exists = false;
            if (idPoly3dX != ObjectId.Null)
            {
                DialogResult result;
                result = MessageBox.Show("Add Slope Breaklines to Design Surface? \nYes to add to surface: " + surfDES
                                         + "\nNo to keep Breaklines, but not add to surface."
                                         + "\nCancel to dispose of Breaklines.",
                                         "Slope Breaklines",

                                         System.Windows.Forms.MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.Yes:
                        TinSurface surf = (TinSurface)Surf.getTinSurface(surfDES, out exists);
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
        setupSlope(int intSide,
                    Point3d pnt3dA,
                    Point3d pnt3dB,
                    double slope,
                    ref Boolean boolDoFirst,
                    string strSurfaceTAR,
                    string strSurfaceDES,
                    ref List<Point3d> pnts3dB1,
                    ref List<Point3d> pnts3dB2,
                    ref List<Point3d> pnts3dD,
                    ref List<Point3d> pnts3dR)
        {
            double dblB1Width = Convert.ToDouble(fmain.strB1Width.ToString());
            double dblB2Width = Convert.ToDouble(fmain.strB2Width.ToString());
            double dblB1Slope = Convert.ToDouble(fmain.strB1Slope.ToString());
            double dblB2Slope = Convert.ToDouble(fmain.strB2Slope.ToString());

            Boolean boolB1 = fmain.boolB1;
            Boolean boolB2 = fmain.boolB1;

            Boolean boolSuccess = false;

            double dblAngAB = pnt3dA.getDirection(pnt3dB);
            //double[] dblPnts = new double[] { };

            Point3d pnt3dX;
            bool exists = false;

            List<Point3d> pnts3dT = new List<Point3d>();

            double dblAng = dblAngAB + PI / 2 * intSide;
            pnts3dT = getIntermediates(pnt3dA, pnt3dB, dblAngAB);
            for (int i = 0; i < pnts3dT.Count; i++)
            {
                pnts3dR.Add(pnts3dT.ElementAt(i));
            }

            for (int i = 0; i < pnts3dT.Count; i++)
            {
                pnt3dX = pnts3dT.ElementAt(i);
                if (boolB1 == true)
                {
                    pnt3dX = pnt3dX.traverse(dblAng, dblB1Width, dblB1Slope);
                    pnts3dB1.Add(pnt3dX);
                }
                if (boolB2 == true)
                {
                    pnt3dX = pnt3dX.traverse(dblAng, dblB2Width, dblB2Slope);
                }

                ObjectId idSurf = Surf.getSurface(strSurfaceTAR, out exists);
                TinSurface surf = idSurf.getEnt() as TinSurface;

                Vector3d v3d = new Vector3d(System.Math.Cos(dblAng), System.Math.Sin(dblAng), -slope);
                Point3d pnt3d0 = Pub.pnt3dO;
                try
                {
                    pnt3d0 = surf.GetIntersectionPoint(pnt3dX, v3d);
                }
                catch (System.Exception )
                {
                    v3d = new Vector3d(v3d.X, v3d.Y, slope);
                    pnt3d0 = surf.GetIntersectionPoint(pnt3dX, v3d);
                }

                pnts3dD.Add(pnt3d0);

                if (boolB2 == true)
                {
                    pnt3dX = pnt3d0.traverse(dblAng + PI, dblB2Width, -dblB2Slope);
                    pnts3dB2.Add(pnt3dX);
                }
            }

            return boolSuccess;
        } // end setupSlope

        public static Boolean
        setupSlope(int intSide,
                    Point3d pnt3dA,
                    Point3d pnt3dB,
                    Point3d pnt3dC,
                    double slope,
                    ref Boolean doFirst,
                    string surfTar,
                    string surfDES,
                    ref List<Point3d> pnts3dB1,
                    ref List<Point3d> pnts3dB2,
                    ref List<Point3d> pnts3dD,
                    ref List<Point3d> pnts3dR)
        {
            double dblB1Width = Convert.ToDouble(fmain.strB1Width.ToString());
            double dblB2Width = Convert.ToDouble(fmain.strB2Width.ToString());
            double dblB1Slope = Convert.ToDouble(fmain.strB1Slope.ToString());
            double dblB2Slope = Convert.ToDouble(fmain.strB2Slope.ToString());

            Boolean boolB1 = fmain.boolB1;
            Boolean boolB2 = fmain.boolB1;

            Boolean success = false;

            double dblAngAB = pnt3dA.getDirection(pnt3dB);
            double dblAngBC = pnt3dB.getDirection(pnt3dC);
            double dblAngABC = Geom.getAngle3Points(pnt3dA, pnt3dB, pnt3dC);

            Point3d pnt3dX;
            bool exists = false;

            List<Point3d> pnts3dT = new List<Point3d>();
            ObjectId idSurf = Surf.getSurface(surfTar, out exists);
            TinSurface surf = idSurf.getEnt() as TinSurface;

            double dblAng = dblAngAB + PI / 2 * intSide;
            Vector3d v3d = new Vector3d(System.Math.Cos(dblAng), System.Math.Sin(dblAng), -slope);

            pnts3dT = getIntermediates(pnt3dA, pnt3dB, dblAngAB);

            for (int i = 0; i < pnts3dT.Count; i++)
            {
                pnts3dR.Add(pnts3dT.ElementAt(i));
            }

            if (doFirst == true)
            {
                pnt3dX = pnts3dT.ElementAt(0);
                if (boolB1 == true)
                {
                    pnt3dX = pnt3dX.traverse(dblAng, dblB1Width, dblB1Slope);
                    pnts3dB1.Add(pnt3dX);
                }
                if (boolB2 == true)
                {
                    pnt3dX = pnt3dX.traverse(dblAng, dblB2Width, dblB2Slope);
                }

                Point3d pnt3d00 = surf.GetIntersectionPoint(pnt3dX, v3d);

                pnts3dD.Add(pnt3d00);

                if (boolB2 == true)
                {
                    pnt3dX = pnt3d00.traverse(dblAng + PI, dblB2Width, -dblB2Slope);
                    pnts3dB2.Add(pnt3dX);
                }
            }

            for (int i = 1; i < pnts3dT.Count; i++)
            {
                pnt3dX = pnts3dT.ElementAt(i);
                if (boolB1 == true)
                {
                    pnt3dX = pnt3dX.traverse(dblAng, dblB1Width, dblB1Slope);
                    pnts3dB1.Add(pnt3dX);
                }
                if (boolB2 == true)
                {
                    pnt3dX = pnt3dX.traverse(dblAng, dblB2Width, dblB2Slope);
                }

                Point3d pnt3d01 = surf.GetIntersectionPoint(pnt3dX, v3d);
                pnts3dD.Add(pnt3d01);

                if (boolB2 == true)
                {
                    pnt3dX = pnt3d01.traverse(dblAng + PI, dblB2Width, -dblB2Slope);
                    pnts3dB2.Add(pnt3dX);
                }
            }

            //do mid delta
            double dblAngV = intSide * dblAngBC + (2 * PI - dblAngABC) / 2;
            pnt3dX = pnts3dT.ElementAt(pnts3dT.Count - 1);
            if (boolB1 == true)
            {
                pnt3dX = pnt3dX.traverse(dblAngV, dblB1Width, dblB1Slope);
                pnts3dB1.Add(pnt3dX);
            }
            if (boolB2 == true)
            {
                pnt3dX = pnt3dX.traverse(dblAngV, dblB2Width, dblB2Slope);
            }

            Point3d pnt3d02 = surf.GetIntersectionPoint(pnt3dX, v3d);
            pnts3dD.Add(pnt3d02);

            if (boolB2 == true)
            {
                pnt3dX = pnt3d02.traverse(dblAngV + PI, dblB2Width, -dblB2Slope);
                pnts3dB2.Add(pnt3dX);
            }

            return success;
        } // end setupSlope

        private static List<Point3d> getIntermediates(Point3d pnt3dA, Point3d pnt3dB, double angAB)
        {
            int intInterval = Convert.ToInt32(fmain.strInterval.ToString());

            List<Point3d> pnts3dX = new List<Point3d>();
            double len = pnt3dA.getDistance(pnt3dB);
            int num = (int)System.Math.Truncate(len / intInterval);
            double slope = (pnt3dB.Z - pnt3dA.Z) / len;

            for (int i = 0; i < num; i++)
            {
                pnts3dX.Add(pnt3dA.traverse(angAB, i * intInterval, slope));
            }

            pnts3dX.Add(pnt3dB);

            return pnts3dX;
        } // end  getIntermediates
    } //end class slopeGrading
}