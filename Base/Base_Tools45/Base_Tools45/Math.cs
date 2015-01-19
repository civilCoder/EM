// (C) Copyright 2013 by JOHN HERMAN

using Autodesk.AutoCAD.Geometry;

//using Autodesk.Civil.DatabaseServices;
//using Tei.Base.Application;
namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static class Math
    {
        /// <summary>
        ///
        /// </summary>
        public const double PI = System.Math.PI;

        /// <summary>
        /// determine if 2 segments are drawn CCW or CC
        /// </summary>
        /// <param name="pnt3dA"></param>
        /// <param name="pnt3d0"></param>
        /// <param name="pnt3dB"></param>
        /// <returns>bool</returns>
        ///
        public static bool
        isRightHand(Point3d pnt3dA, Point3d pnt3d0, Point3d pnt3dB)
        {
            double dxA = pnt3dA.X - pnt3d0.X;
            double dyA = pnt3dA.Y - pnt3d0.Y;

            double dxB = pnt3dB.X - pnt3d0.X;
            double dyB = pnt3dB.Y - pnt3d0.Y;

            if ((dxA * dyB - dxB * dyA) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// determine if target point is on right or left side of line segment
        /// </summary>
        /// <param name="pnt3d0"></param>
        /// <param name="pnt3d1"></param>
        /// <param name="pnt3dX"></param>
        /// <returns>bool</returns>
        public static bool
        isRightSide(Point3d pnt3d0, Point3d pnt3d1, Point3d pnt3dX)
        {
            double dx1 = pnt3d1.X - pnt3d0.X;
            double dy1 = pnt3d1.Y - pnt3d0.Y;

            double dx2 = pnt3dX.X - pnt3d0.X;
            double dy2 = pnt3dX.Y - pnt3d0.Y;

            if ((dx1 * dy2 - dx2 * dy1) > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// calculate point from base point, length and direction
        /// </summary>
        /// <param name="pnt3dBase"></param>
        /// <param name="dblAng"></param>
        /// <param name="dblLen"></param>
        /// <returns>Point3d</returns>
        public static bool
        left_Justify(double dblAngle)
        {
            double dblAngViewTwist = (double)Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("VIEWTWIST");
            double dblAngViewNorth;
            double dblAngViewSouth;

            if (dblAngle > 2 * PI)
            {
                dblAngle = dblAngle - 2 * PI;
            }

            dblAngle = System.Math.Round(dblAngle, 6);

            if (dblAngViewTwist == 0)
            {
                dblAngViewNorth = PI / 2;
            }
            else
            {
                dblAngViewNorth = 5 * PI / 2 - dblAngViewTwist;
            }

            if (dblAngViewNorth > 2 * PI)
            {
                dblAngViewNorth = dblAngViewNorth - 2 * PI;
            }

            dblAngViewSouth = dblAngViewNorth + PI;

            dblAngViewNorth = System.Math.Round(dblAngViewNorth, 6);
            dblAngViewSouth = System.Math.Round(dblAngViewSouth, 6);

            if (dblAngle > dblAngViewNorth && dblAngle <= dblAngViewSouth)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static double
        mantissa(this double x)
        {
            return x - System.Math.Truncate(x);
        }

        public static double
        mod(this double x, double y)
        {
            return (x / y) - System.Math.Truncate(x / y);
        }

        public static double
        roundDown1(double dblNum)
        {
            return (System.Math.Truncate(dblNum * 10)) / 10;
        }

        public static double
        roundDown2(double dblNum)
        {
            return (System.Math.Truncate(dblNum * 100)) / 100;
        }

        public static double
        roundDown3(double dblNum)
        {
            return (System.Math.Truncate(dblNum * 1000)) / 1000;
        }

        public static double
        roundDown4(double dblNum)
        {
            return (System.Math.Truncate(dblNum * 10000)) / 10000;
        }

        public static double
        roundDown5(double dblNum)
        {
            return (System.Math.Truncate(dblNum * 100000)) / 100000;
        }

        public static double
        roundUP0(double dblNum)
        {
            return (System.Math.Truncate(dblNum)) + 1;
        }

        public static double
        roundUP2(double dblNum)
        {
            return (System.Math.Truncate(dblNum * 100 + 1)) / 100;
        }

        public static double
        roundUP3(double dblNum)
        {
            return (System.Math.Truncate(dblNum * 1000 + 1)) / 1000;
        }

        public static Point3d
        traverse(Point3d pnt3dBase, double dblAng, double dblLen)
        {
            Point3d pnt3dTar = new Point3d(pnt3dBase.X + System.Math.Cos(dblAng) * dblLen,
                pnt3dBase.Y + System.Math.Sin(dblAng) * dblLen,
                0.0);
            return pnt3dTar;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnt2dBase"></param>
        /// <param name="dblAng"></param>
        /// <param name="dblLen"></param>
        /// <returns></returns>
        public static Point2d
        traverse(Point2d pnt2dBase, double dblAng, double dblLen)
        {
            Point2d pnt2dTar = new Point2d(pnt2dBase.X + System.Math.Cos(dblAng) * dblLen,
                pnt2dBase.Y + System.Math.Sin(dblAng) * dblLen);
            return pnt2dTar;
        }

        /// <summary>
        /// calculate point from base point, length, direction and slope
        /// </summary>
        /// <param name="pnt3dBase"></param>
        /// <param name="dblAng"></param>
        /// <param name="dblLen"></param>
        /// <param name="dblSlope"></param>
        /// <returns>Point3d</returns>
        public static Point3d
        traverse(Point3d pnt3dBase, double dblAng, double dblLen, double dblSlope)
        {
            Point3d pnt3dTar = new Point3d(pnt3dBase.X + System.Math.Cos(dblAng) * dblLen,
                pnt3dBase.Y + System.Math.Sin(dblAng) * dblLen,
                pnt3dBase.Z + dblSlope * dblLen);
            return pnt3dTar;
        }

        public static int
        truncate(this double x)
        {
            return int.Parse(System.Math.Truncate(x).ToString());
        }

        public static double
        sin(double a)
        {
            return System.Math.Sin(a);
        }

        public static double
        cos(double a)
        {
            return System.Math.Cos(a);
        }

        public static double
        tan(double a)
        {
            return System.Math.Tan(a);
        }

        // Custom ArcTangent method, as the Math.Atan
        // doesn't handle specific cases

        public static double atan(double y, double x)
        {
            if (x > 0)
                return System.Math.Atan(y / x);

            else if (x < 0)
                return System.Math.Atan(y / x) - PI;
            else  // x == 0
            {
                if (y > 0)
                    return PI;
                else if (y < 0)
                    return -PI;
                else // if (y == 0) theta is undefined
                    return 0.0;
            }
        }
    }// Class Math
}// namespace myAcadUtil
