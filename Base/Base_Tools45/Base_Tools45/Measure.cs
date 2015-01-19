using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static class Measure
    {
        /// <summary>
        ///
        /// </summary>
        public static double PI = System.Math.PI;

        /// <summary>
        /// calculate direction in radians
        /// of line segment defined by 2 Point3d
        /// </summary>
        /// <param name="pnt3dBEG"></param>
        /// <param name="pnt3dEND"></param>
        /// <returns>double</returns>
        public static double
        getAzRadians(Point3d pnt3dBEG, Point3d pnt3dEND)
        {
            double dx1 = pnt3dEND.X - pnt3dBEG.X;
            double dy1 = pnt3dEND.Y - pnt3dBEG.Y;

            double result = 0.0;

            if (dx1 < 0.0)
            {
                if (dy1 < 0.0)
                {
                    result = PI + System.Math.Atan(dy1 / dx1);
                }
                if (dy1 == 0.0)
                {
                    result = PI;
                }
                if (dy1 > 0.0)
                {
                    result = PI + System.Math.Atan(dy1 / dx1);
                }
            }
            if (dx1 == 0.0)
            {
                if (dy1 < 0.0)
                {
                    result = 3 * PI / 2;
                }
                if (dy1 == 0.0)
                {
                    result = 0.0;
                }
                if (dy1 > 0.0)
                {
                    result = PI / 2;
                }
            }
            if (dx1 > 0.0)
            {
                if (dy1 < 0.0)
                {
                    result = 2 * PI + System.Math.Atan(dy1 / dx1);
                }
                if (dy1 == 0.0)
                {
                    result = 0.0;
                }
                if (dy1 > 0.0)
                {
                    result = System.Math.Atan(dy1 / dx1);
                }
            }
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnt2d0"></param>
        /// <param name="pnt2d1"></param>
        /// <returns></returns>
        public static double
        getAzRadians(Point2d pnt2d0, Point2d pnt2d1)
        {
            double dx1 = pnt2d1.X - pnt2d0.X;
            double dy1 = pnt2d1.Y - pnt2d0.Y;

            double result = 0.0;

            if (dx1 < 0.0)
            {
                if (dy1 < 0.0)
                {
                    result = PI + System.Math.Atan(dy1 / dx1);
                }
                if (dy1 == 0.0)
                {
                    result = PI;
                }
                if (dy1 > 0.0)
                {
                    result = PI + System.Math.Atan(dy1 / dx1);
                }
            }
            if (dx1 == 0.0)
            {
                if (dy1 < 0.0)
                {
                    result = 3 * PI / 2;
                }
                if (dy1 == 0.0)
                {
                    result = 0.0;
                }
                if (dy1 > 0.0)
                {
                    result = PI / 2;
                }
            }
            if (dx1 > 0.0)
            {
                if (dy1 < 0.0)
                {
                    result = 2 * PI + System.Math.Atan(dy1 / dx1);
                }
                if (dy1 == 0.0)
                {
                    result = 0.0;
                }
                if (dy1 > 0.0)
                {
                    result = System.Math.Atan(dy1 / dx1);
                }
            }
            return result;
        }

        /// <summary>
        /// calculate direction in radians
        /// of line segment defined by dx and dy
        /// </summary>
        /// <param name="dx1"></param>
        /// <param name="dy1"></param>
        /// <returns>double</returns>
        public static double
        getAzRadians(double dx1, double dy1)
        {
            double result = 0.0;

            if (dx1 < 0.0)
            {
                if (dy1 < 0.0)
                {
                    result = PI + System.Math.Atan(dy1 / dx1);
                }
                if (dy1 == 0.0)
                {
                    result = PI;
                }
                if (dy1 > 0.0)
                {
                    result = PI + System.Math.Atan(dy1 / dx1);
                }
            }
            if (dx1 == 0.0)
            {
                if (dy1 < 0.0)
                {
                    result = 3 * PI / 2;
                }
                if (dy1 == 0.0)
                {
                    result = 0.0;
                }
                if (dy1 > 0.0)
                {
                    result = PI / 2;
                }
            }
            if (dx1 > 0.0)
            {
                if (dy1 < 0.0)
                {
                    result = 2 * PI + System.Math.Atan(dy1 / dx1);
                }
                if (dy1 == 0.0)
                {
                    result = 0.0;
                }
                if (dy1 > 0.0)
                {
                    result = System.Math.Atan(dy1 / dx1);
                }
            }
            return result;
        }

        /// <summary>
        /// calculate distance between line
        /// segment defined by 2 Point3d
        /// </summary>
        /// <param name="pnt3d0"></param>
        /// <param name="pnt3d1"></param>
        /// <param name="decPlaces"></param>
        /// <returns>double</returns>
        public static double
        getDistance2d(Point3d pnt3d0, Point3d pnt3d1, int decPlaces = -1)
        {
            double dx1 = pnt3d1.X - pnt3d0.X;
            double dy1 = pnt3d1.Y - pnt3d0.Y;

            double result = System.Math.Sqrt(dx1 * dx1 + dy1 * dy1);
            if (decPlaces != -1)
                return (System.Math.Round(result, decPlaces));
            else
                return result;
        }

        public static double
        getDistance2d(Point2d pnt2d0, Point2d pnt2d1, int decPlaces = -1)
        {
            double dx1 = pnt2d1.X - pnt2d0.X;
            double dy1 = pnt2d1.Y - pnt2d0.Y;

            double result = System.Math.Sqrt(dx1 * dx1 + dy1 * dy1);
            if (decPlaces != -1)
                return (System.Math.Round(result, decPlaces));
            else
                return result;
        }

        /// <summary>
        /// calculate slope between two Point3d
        /// </summary>
        /// <param name="pnt3d1"></param>
        /// <param name="pnt3d2"></param>
        /// <returns>double</returns>
        public static double
        getSlope(Point3d pnt3d1, Point3d pnt3d2)
        {
            return (pnt3d2.Z - pnt3d1.Z) / getDistance2d(pnt3d1, pnt3d2);
        }

        public static double
        getSlope(this ObjectId idCgPnt1, ObjectId idCgPnt2)
        {
            Point3d pnt3d1 = idCgPnt1.getCogoPntCoordinates();
            Point3d pnt3d2 = idCgPnt2.getCogoPntCoordinates();

            return (pnt3d2.Z - pnt3d1.Z) / getDistance2d(pnt3d1, pnt3d2);
        }
    }
}
