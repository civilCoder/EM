using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.IO;

namespace Stake
{
    public static class Stake_Algn2
    {
        public struct XALGNS
        {
            public string Name;
            public double XSTA;
        }

        public static void
        getAlignInts()
        {
            Alignment objAlignCL = Align.getAlignment("RIVER_CL");
            List<XALGNS> xAlgns = new List<XALGNS>();
            XALGNS xAlgn = new XALGNS();
            ObjectIdCollection ids = Align.getAlignmentIDs();

            for (int i = 0; i < ids.Count; i++)
            {
                Alignment objAlign = (Alignment)ids[i].getEnt();

                Point3dCollection varPntInt = new Point3dCollection();
                objAlignCL.IntersectWith(objAlign, Intersect.OnBothOperands, varPntInt, IntPtr.Zero, IntPtr.Zero);

                if (varPntInt.Count == 0)
                    return;
                for (int j = 0; j < varPntInt.Count; j++)
                {
                    Point3d pnt3d = varPntInt[j];
                    double dblStation = 0, dblOffset = 0;
                    objAlign.StationOffset(pnt3d.X, pnt3d.Y, ref dblStation, ref dblOffset);
                    xAlgn = new XALGNS { Name = objAlign.Name, XSTA = System.Math.Round(dblStation, 3) };

                    ProfileView objProfileView = Prof.getProfileView(objAlign.Name, objAlign.Name);
                    Extents3d ext3d = (Extents3d)objProfileView.Bounds;
                    Point3d pnt3dMin = ext3d.MinPoint;
                    Point3d pnt3dMax = ext3d.MaxPoint;

                    double dblStaStart = objProfileView.StationStart;
                    double dblElevStart = objProfileView.ElevationMin;

                    Profile objProfile = Prof.getProfile(objAlign.ObjectId, objAlign.Name);
                    double dblElev = objProfile.ElevationAt(dblStation);

                    Point3d pnt3dIns = new Point3d(pnt3dMin.X + dblStation - dblStaStart,
                        pnt3dMin.Y + (dblElev - dblElevStart) * 10,
                        0);
                    Draw.addCircle(pnt3dIns, 2);
                }
            }
        }

        public static void
        importHecRasCenterline()
        {
            string strFileName = "O:\\3100-3199\\3158\\HEC RAS\\CenterlineDataIn.csv";
            List<Point3d> pnts3d = new List<Point3d>();
            using (StreamReader SR = new StreamReader(strFileName))
            {
                while (!SR.EndOfStream)
                {
                    int intPos1 = 1;

                    string strData = SR.ReadLine();

                    intPos1 = strData.IndexOf(",", intPos1);

                    string strAlignName = strData.Substring(0, intPos1 - 1);
                    Alignment objAlign = Align.getAlignment(strAlignName);

                    int intPos2 = intPos1;
                    intPos1 = strData.IndexOf(",", intPos1 + 1);
                    double dblStaL = double.Parse(strData.Substring(intPos2 + 1, intPos1 - intPos2));

                    double dblElev = double.Parse(strData.Substring(intPos1 + 1));

                    double easting = 0, northing = 0;
                    objAlign.PointLocation(dblStaL, 0.0, ref easting, ref northing);
                    Point3d pnt3d = new Point3d(easting, northing, dblElev);

                    pnts3d.Add(pnt3d);
                }
            }

            Draw.addPoly(pnts3d);
        }

        public static void
        importWaterSurfaceData()
        {
            string strFileName = "O:\\3100-3199\\3158\\HEC RAS\\SectionDataIn.csv";
            List<Point3d> pnts3dL = new List<Point3d>();
            List<Point3d> pnts3dR = new List<Point3d>();

            using (StreamReader SR = new StreamReader(strFileName))
            {
                while (!SR.EndOfStream)
                {
                    int intPos1 = 1;

                    string strData = SR.ReadLine();

                    intPos1 = strData.IndexOf(",", intPos1);
                    string strAlignName = strData.Substring(0, intPos1 - 1);
                    Alignment objAlign = Align.getAlignment(strAlignName);

                    int intPos2 = intPos1;
                    intPos1 = strData.IndexOf(",", intPos1 + 1);
                    double dblStaL = double.Parse(strData.Substring(intPos2 + 1, intPos1 - intPos2));

                    intPos2 = intPos1;
                    intPos1 = strData.IndexOf(",", intPos1 + 1);
                    double dblStaR = double.Parse(strData.Substring(intPos2 + 1, intPos1 - intPos2));

                    double dblElev = double.Parse(strData.Substring(intPos1 + 1));
                    double easting = 0, northing = 0;
                    objAlign.PointLocation(dblStaL, 0.0, ref easting, ref northing);

                    Point3d pnt3d = new Point3d(easting, northing, dblElev);

                    pnts3dL.Add(pnt3d);

                    objAlign.PointLocation(dblStaR, 0.0, ref easting, ref northing);
                    pnt3d = new Point3d(easting, northing, dblElev);

                    pnts3dR.Add(pnt3d);
                }
            }
            Draw.addPoly(pnts3dL);
            Draw.addPoly(pnts3dR);
        }
    }
}