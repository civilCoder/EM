using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using System.Collections.Generic;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Math = Base_Tools45.Math;

namespace EW {
    public static class EW_Utility2
    {
        public static double
        getSurfaceElev(double dblX, double dblY, TinSurface objSurface)
        {
            double dblElev = 0;
            double dblXX = 0;
            double dblYY = 0;

            dblElev = objSurface.FindElevationAtXY(dblX, dblY);

            if (dblElev > 0)
            {
                dblXX = Math.roundDown2(dblX);
                dblYY = Math.roundDown2(dblY);
                dblElev = objSurface.FindElevationAtXY(dblXX, dblYY);
            }

            if (dblElev < 0)
            {
                dblXX = Math.roundUP2(dblX);
                dblYY = Math.roundUP2(dblY);
                dblElev = objSurface.FindElevationAtXY(dblXX, dblYY);
            }

            return dblElev;
        }

        public static double
        getElev(Point3d pnt3dInt, ObjectId id3dBoundary)
        {

            double dblElev = 0;
            List<Point3d> pnts3d = id3dBoundary.getCoordinates3dList();

            for (int i = 0; i < pnts3d.Count; i++)
            {
                if (Math.roundDown2(pnt3dInt.X) == Math.roundDown2(pnts3d[i].X) &&
                    Math.roundDown2(pnt3dInt.Y) == Math.roundDown2(pnts3d[i].Y))
                {
                    dblElev = pnts3d[i].Z;
                    break;
                }
            }
            return dblElev;
        }

        public static object[]
        getLimits(int intInterval)
        {

            SelectionSet objSSet = EW_Utility1.buildSSetGradingLim();
            if (objSSet.Count == 0)
            {
                Application.ShowAlertDialog("Grading Limit not found - should be on layer GRADING LIMIT - exiting....");
                return null;
            }
            else if (objSSet.Count > 1)
            {
                Application.ShowAlertDialog("More than one Project Boundary found - exiting....");
                return null;
            }

            ObjectId[] ids = objSSet.GetObjectIds();

            Polyline objPlineBndry = (Polyline)ids[0].getEnt();

            Extents3d ext3d = (Extents3d)objPlineBndry.Bounds;
            Point3d pnt3dMin = new Point3d(ext3d.MinPoint.X - 100, ext3d.MinPoint.Y - 100, 0.0);
            Point3d pnt3dMax = new Point3d(ext3d.MaxPoint.X + 100, ext3d.MaxPoint.Y + 100, 0.0);

            double dblDeltaX = pnt3dMax.X - pnt3dMin.X;
            double dblDeltaY = pnt3dMax.Y - pnt3dMin.Y;


            int iMax = (int)System.Math.Truncate(dblDeltaX / intInterval);
            int jMax = (int)System.Math.Truncate(dblDeltaY / intInterval);

            object[] varLimits = new object[4];
            varLimits[0] = pnt3dMin.X;
            varLimits[1] = pnt3dMin.Y;
            varLimits[2] = iMax;
            varLimits[3] = jMax;

            return varLimits;
        }
    }
}