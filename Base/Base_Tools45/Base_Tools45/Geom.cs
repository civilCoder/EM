using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using GeoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Base_Tools45 {
    public enum extend {
        none,
        source,
        both
    }

    /// <summary>
    ///
    /// </summary>
    public static class Geom {
        /// <summary>
        ///
        /// </summary>
        public const double PI = System.Math.PI;

        public static void
        checkIfClosed(ref List<Point3d> pnts3d) {
            int k = pnts3d.Count - 1;
            if (pnts3d[0].X != pnts3d[k].X && pnts3d[0].Y != pnts3d[k].Y) {
                pnts3d.Add(pnts3d[0]);
            }
        }

        /// <summary>
        /// test if Polyline end point = Polyline beg point
        /// if not, then add beg point to end of Polyline
        /// </summary>
        /// <param name="idPoly"></param>
        public static void
        checkIfClosed(ObjectId idPoly) {
            Point3d pnt3dBeg;

            try {
                using (Transaction tr = BaseObjs.startTransactionDb()) {
                    Polyline poly = (Polyline)tr.GetObject(idPoly, OpenMode.ForWrite);
                    pnt3dBeg = poly.StartPoint;

                    Point2d pnt2dBeg = new Point2d(pnt3dBeg.X, pnt3dBeg.Y);
                    Point2d pnt2dEnd = poly.GetPoint2dAt(poly.NumberOfVertices - 1);
                    if (pnt2dEnd.X != pnt2dBeg.X || pnt2dEnd.Y != pnt2dBeg.Y) {
                        if (System.Math.Abs(pnt2dBeg.X - pnt2dEnd.X) > 1.0 || System.Math.Abs(pnt2dBeg.Y - pnt2dEnd.Y) > 1.0) {
                            poly.AddVertexAt(poly.NumberOfVertices, pnt2dBeg, 0, 0, 0);
                        }else {
                            poly.RemoveVertexAt(poly.NumberOfVertices - 1);
                            poly.AddVertexAt(poly.NumberOfVertices, pnt2dBeg, 0, 0, 0);
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex) {
                BaseObjs.writeDebug(ex.Message + " Geom.cs: line: 63");
            }
        }

        /// <summary>
        /// test if Polyline3d end point = Polyline3d beg point
        /// if not, then add beg point to end of Polyline3d
        /// </summary>
        /// <param name="idPoly3d"></param>
        public static bool
        checkIfClosed3d(this ObjectId idPoly3d, bool forceClosed = true) {
            bool isClosed = false;
            Point3d pnt3dBeg = default(Point3d);
            Point3d pnt3dEnd = default(Point3d);

            try {
                using (Transaction tr = BaseObjs.startTransactionDb()) {
                    Polyline3d poly3d = (Polyline3d)tr.GetObject(idPoly3d, OpenMode.ForRead);
                    pnt3dBeg = poly3d.StartPoint;
                    pnt3dEnd = poly3d.EndPoint;

                    if (System.Math.Abs(pnt3dBeg.X - pnt3dEnd.X) > 0.1 | System.Math.Abs(pnt3dBeg.Y - pnt3dEnd.Y) > 0.1) {
                        PolylineVertex3d vTex3d = new PolylineVertex3d(pnt3dBeg);
                        if (forceClosed) {
                            poly3d.UpgradeOpen();
                            poly3d.AppendVertex(vTex3d);
                        }
                    }else {
                        isClosed = true;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex) {
                BaseObjs.writeDebug(ex.Message + " Geom.cs: line: 97");
            }
            return isClosed;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static Vector3d
        cross3D(Vector3d A, Vector3d B) {
            return new Vector3d(A.Y * B.Z - A.Z * B.Y, -(A.X * B.Z - A.Z * B.X), A.X * B.Y - A.Y * B.X);
        }

        /// <summary>
        /// convert double(2) to Point3d
        /// </summary>
        /// <param name="dblPnt"></param>
        /// <returns>Point3d</returns>
        public static Point3d
        dblPnt2pnt3d(double[] dblPnt) {
            Point3d pnt3d = new Point3d(dblPnt[0], dblPnt[1], dblPnt[2]);
            return pnt3d;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnt3d0"></param>
        /// <param name="slope"></param>
        /// <param name="delta"></param>
        /// <param name="direction"></param>
        /// <param name="rotation"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Point3d
        doAnglePointIN(this Point3d pnt3d0, double slope, double delta, double direction, int rotation, double offset) {
            double angleAdj = delta / 2 * rotation;
            slope = slope * System.Math.Cos(delta / 2);
            Point3d pnt3d = pnt3d0.traverse(direction + angleAdj, offset / System.Math.Cos(delta / 2), slope);  //double check cosine or sine
            return pnt3d;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnt3d0"></param>
        /// <param name="slope"></param>
        /// <param name="delta"></param>
        /// <param name="direction"></param>
        /// <param name="rotation"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Point3d
        doAnglePointOUT(this Point3d pnt3d0, double slope, double delta, double direction, int rotation, double offset) {
            double angleAdj = PI / 2 * rotation + (PI - delta) / 2 * rotation * -1;
            slope = slope * System.Math.Cos(delta / 2);
            Point3d pnt3d = pnt3d0.traverse(direction + angleAdj, offset / System.Math.Cos(delta / 2), slope);  //double check cosine or sine
            return pnt3d;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnt2d1"></param>
        /// <param name="pnt2d2"></param>
        /// <returns></returns>
        public static double
        get2dDistance(Point2d pnt2d1, Point2d pnt2d2) {
            return System.Math.Sqrt(System.Math.Pow(pnt2d2.X - pnt2d1.X, 2) + System.Math.Pow(pnt2d2.Y - pnt2d1.Y, 2));
        }

        /// <summary>
        /// calculate 2d distance between two Point3d
        /// </summary>
        /// <param name="pnt3d1"></param>
        /// <param name="pnt3d2"></param>
        /// <returns>double</returns>
        public static double
        get2Ddistance(Point3d pnt3d1, Point3d pnt3d2) {
            return System.Math.Sqrt(System.Math.Pow(pnt3d2.X - pnt3d1.X, 2) + (System.Math.Pow(pnt3d2.Y - pnt3d1.Y, 2)));
        }

        /// <summary>
        /// calculate 2d Dot Product
        /// </summary>
        /// <param name="pnt3dDxDyDz1"></param>
        /// <param name="pnt3dDxDyDz2"></param>
        /// <returns>double</returns>
        public static double
        get2dDotProduct(Point3d pnt3dDxDyDz1, Point3d pnt3dDxDyDz2) {
            return pnt3dDxDyDz1.X * pnt3dDxDyDz2.X + pnt3dDxDyDz1.Y * pnt3dDxDyDz2.Y;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnt3dDxDy1"></param>
        /// <param name="pnt3dDxDy2"></param>
        /// <returns></returns>
        public static double
        get2dDotProduct(Point2d pnt3dDxDy1, Point2d pnt3dDxDy2) {
            return pnt3dDxDy1.X * pnt3dDxDy2.X + pnt3dDxDy1.Y * pnt3dDxDy2.Y;
        }

        /// <summary>
        /// calculate acute angle between two vectors
        /// </summary>
        /// <param name="dxdydz1"></param>
        /// <param name="dxdydz2"></param>
        /// <param name="len1"></param>
        /// <param name="len2"></param>
        /// <returns></returns>
        public static double
        getAngle2Vectors(Point3d dxdydz1, Point3d dxdydz2, double len1, double len2) {
            double dbl2dDotProduct = 0;
            double dblNum = 0;

            dbl2dDotProduct = get2dDotProduct(dxdydz1, dxdydz2);
            if (dbl2dDotProduct == 0)
                return PI / 2;

            dblNum = dbl2dDotProduct / (len1 * len2);

            if (dblNum > -1 & dblNum < 1) {
                return System.Math.Acos(dblNum);
            }else {
                return 0;
            }
        }

        public static double
        getAngle2Vectors(this Vector2d v2d1, Vector2d v2d2)
        {
            double dbl2dDotProduct = 0;
            double dblNum = 0;

            Point3d pnt3d1 = new Point3d(v2d1.X, v2d1.Y, 0.0);
            Point3d pnt3d2 = new Point3d(v2d2.X, v2d2.Y, 0.0);

            double len1 = System.Math.Sqrt(v2d1.X * v2d1.X + v2d1.Y * v2d1.Y);
            double len2 = System.Math.Sqrt(v2d2.X * v2d2.X + v2d2.Y * v2d2.Y);

            dbl2dDotProduct = get2dDotProduct(pnt3d1, pnt3d2);
            if (dbl2dDotProduct == 0)
                return PI / 2;
            
            dblNum = dbl2dDotProduct / (len1 * len2);

            if (dblNum > -1 && dblNum < 1)
            {
                return System.Math.Acos(dblNum);
            }
            else
            {
                return 0;
            }
        }


        public static double
        getAngle2Vectors(this Vector3d v3d1, Vector3d v3d2) {
            double dbl2dDotProduct = 0;
            double dblNum = 0;

            Point3d pnt3d1 = new Point3d(v3d1.X, v3d1.Y, 0.0);
            Point3d pnt3d2 = new Point3d(v3d2.X, v3d2.Y, 0.0);

            dbl2dDotProduct = get2dDotProduct(pnt3d1, pnt3d2);
            if (dbl2dDotProduct == 0)
                return PI / 2;

            double len1 = System.Math.Sqrt(v3d1.X * v3d1.X + v3d1.Y * v3d1.Y);
            double len2 = System.Math.Sqrt(v3d2.X * v3d2.X + v3d2.Y * v3d2.Y);

            dblNum = dbl2dDotProduct / (len1 * len2);

            if (dblNum > -1 & dblNum < 1)
            {
                return System.Math.Acos(dblNum);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// calculate angle between 2 3d vectors defined
        /// by 3 Point3d pnt3dBeg is the angle point
        /// </summary>
        /// <param name="pnt3dBeg"></param>
        /// <param name="pnt3dEnd"></param>
        /// <param name="pnt3d3X"></param>
        /// <returns>double</returns>
        public static double
        getAngle3Points(Point3d pnt3dBeg, Point3d pnt3dMid, Point3d pnt3dEnd, bool do3d = false)
        {
            Vector3d v3d12 = pnt3dMid - pnt3dBeg;
            Vector3d v3d23 = pnt3dEnd - pnt3dMid;
            double dot = 0.0, len12 = 0.0, len23 = 0.0, delta = 0.0;

            if (do3d)
            {
                dot = v3d12.DotProduct(v3d23);

                len12 = pnt3dBeg.getDistance(pnt3dMid);
                len23 = pnt3dMid.getDistance(pnt3dEnd);
            }
            else
            {
                Plane plane = BaseObjs.xyPlane;

                Vector2d v2d12 = v3d12.Convert2d(plane);
                Vector2d v2d23 = v3d23.Convert2d(plane);

                dot = v2d12.DotProduct(v2d23);

                len12 = pnt3dBeg.getDistance(pnt3dMid);
                len23 = pnt3dMid.getDistance(pnt3dEnd);
            }

            delta = System.Math.Acos(dot / (len12 * len23));
            return delta;
        }

        /// <summary>
        /// pnt3d2 is the angle point
        /// </summary>
        /// <param name="pnt2d1"></param>
        /// <param name="pnt2d2"></param>
        /// <param name="pnt2d3"></param>
        /// <returns></returns>
        public static double
        getAngle3Points2d(Point2d pnt2d1, Point2d pnt2d2, Point2d pnt2d3)
        {
            Vector2d v2d12 = pnt2d2 - pnt2d1;
            Vector2d v2d23 = pnt2d3 - pnt2d2;

            double dot = v2d12.X * v2d23.X + v2d12.Y * v2d23.Y;

            double len12 = pnt2d1.getDistance(pnt2d2);
            double len23 = pnt2d2.getDistance(pnt2d3);

            double delta = System.Math.Acos(dot / (len12 * len23));
            return delta;
        }

        public static double
        getAngle3Points3d(Point3d pnt3dBeg, Point3d pnt3dMid, Point3d pnt3dEnd)
        {
            Vector3d v3d12 = pnt3dMid - pnt3dBeg;
            Vector3d v3d23 = pnt3dEnd - pnt3dMid;

            Plane plane = BaseObjs.xyPlane;

            Vector2d v2d12 = v3d12.Convert2d(plane);
            Vector2d v2d23 = v3d23.Convert2d(plane);

            double dot = v2d12.DotProduct(v2d23);

            double len12 = pnt3dBeg.getDistance(pnt3dMid);
            double len23 = pnt3dMid.getDistance(pnt3dEnd);

            double delta = System.Math.Acos(dot / (len12 * len23));
            return delta;
        }

        public static double
        getArea(this ObjectId idPoly){
            double area = 0;
            using(var tr = BaseObjs.startTransactionDb()){
                Polyline poly = (Polyline)tr.GetObject(idPoly, OpenMode.ForRead);
                area = poly.Area;
                tr.Commit();
            }
            return area;
        }

        public static Point3d
        getCenter(Point3d pnt3dBeg, Point3d pnt3dEnd, double bulge)
        {
            double dblDelta = getDelta(bulge);
            double radius = getRadius(pnt3dBeg, pnt3dEnd, bulge);
            double dblAzChord = Base_Tools45.Measure.getAzRadians(pnt3dBeg, pnt3dEnd);
            double dblAzTan = dblAzChord - dblDelta / 2;
            return Math.traverse(pnt3dBeg, (dblAzTan + PI / 2), radius);
        }

        /// <summary>
        /// calculate Centroid of area defined by Polyline
        /// </summary>
        /// <param name="poly"></param>
        /// <returns>Point 3d</returns>
        public static Point3d
        getCentroid(Polyline poly)
        {
            double a = 0;
            double cenX = 0;
            double cenY = 0;

            int z = 0;

            List<Point2d> pnts2d = new List<Point2d>();

            checkIfClosed(poly.ObjectId);

            Point3d pnt3d = default(Point3d);

            pnts2d = getPolyPoint2ds(poly);

            if (pnts2d.Count > 2)
            {
                a = 0;

                for (z = 0; z < pnts2d.Count - 1; z++)
                {
                    a = a + (pnts2d[z + 0].X * pnts2d[z + 1].Y) - (pnts2d[z + 1].X * pnts2d[z + 0].Y);
                }

                a = a * 0.5;

                cenX = 0;
                cenY = 0;

                for (z = 0; z < pnts2d.Count - 1; z++)
                {
                    cenX = cenX + (pnts2d[z + 0].X + pnts2d[z + 1].X) * ((pnts2d[z + 0].X * pnts2d[z + 1].Y) - (pnts2d[z + 1].X * pnts2d[z + 0].Y));
                }
                cenX = cenX / (6 * a);

                for (z = 0; z < pnts2d.Count - 1; z++)
                {
                    cenY = cenY + (pnts2d[z + 0].Y + pnts2d[z + 1].Y) * ((pnts2d[z + 0].X * pnts2d[z + 1]).Y - (pnts2d[z + 1].X * pnts2d[z + 0].Y));
                }
                cenY = cenY / (6 * a);

                pnt3d = new Point3d(cenX, cenY, 0);
            }
            return pnt3d;
        }

        /// <summary>
        /// calculate distance along defined axis of target point
        /// </summary>
        /// <param name="pnt3dRef"></param>
        /// <param name="pnt3dTar"></param>
        /// <param name="pnt3dX"></param>
        /// <returns>double</returns>
        public static double
        getCosineComponent(Point3d pnt3dRef, Point3d pnt3dTar, Point3d pnt3dX)
        {
            double dblLen0 = get2Ddistance(pnt3dRef, pnt3dTar);

            Point3d pnt3dDxDy0 = new Point3d(pnt3dRef.X - pnt3dTar.X, pnt3dRef.Y - pnt3dTar.Y, 0.0);
            Point3d pnt3dDxDyX = new Point3d(pnt3dX.X - pnt3dTar.X, pnt3dX.Y - pnt3dTar.Y, 0.0);

            double dblLenX = get2Ddistance(pnt3dX, pnt3dTar);

            double dblAng = getAngle2Vectors(pnt3dDxDy0, pnt3dDxDyX, dblLen0, dblLenX);

            return System.Math.Cos(dblAng) * dblLenX;
        }

        public static double
        getDeflectionAngle2Vectors(this Vector2d v2d1, Vector2d v2d2) {
            double angle = 0.0;
            double dblNum = 0;
            double dbl2dDotProduct = 0;

            double len1 = System.Math.Sqrt(v2d1.X * v2d1.X + v2d1.Y * v2d1.Y);
            double len2 = System.Math.Sqrt(v2d2.X * v2d2.X + v2d2.Y * v2d2.Y);

            Point3d pnt3d1 = new Point3d(v2d1.X, v2d1.Y, 0.0);
            Point3d pnt3d2 = new Point3d(v2d2.X, v2d2.Y, 0.0);

            dbl2dDotProduct = get2dDotProduct(pnt3d1, pnt3d2);
            if (dbl2dDotProduct == 0)
                return PI / 2;

            dblNum = dbl2dDotProduct / (len1 * len2);

            if (dblNum > -1 && dblNum < 0) {
                angle = System.Math.Acos(dblNum) * -1;
            }else if (dblNum > 0 && dblNum < 1) {
                angle = System.Math.Acos(dblNum);
            }else {
                angle = 0;
            }
            
            return angle;
        }
        public static double getDelta(double bulge)
        {
            return 4 * (System.Math.Atan(bulge));
        }

        public static double
        getDistanceToPointOnArc(this Arc arc, Point3d pnt3d) {
            double dist = 0.0;
            Point3d pnt3dClose = arc.GetClosestPointTo(pnt3d, false);
            double pntParam = arc.GetParameterAtPoint(pnt3dClose);
            double X = arc.GetDistanceAtParameter(pntParam);
            double O = arc.GetDistanceAtParameter(arc.StartParam);
            dist = X - O;
            return dist;
        }

        public static double
        getDistanceToPointOnCurve(this Curve curve, Point3d pnt3d) {
            double dist = 0.0;
            Point3d pnt3dClose = curve.GetClosestPointTo(pnt3d, false);
            double pntParam = curve.GetParameterAtPoint(pnt3dClose);
            double X = curve.GetDistanceAtParameter(pntParam);
            double O = curve.GetDistanceAtParameter(curve.StartParam);
            dist = X - O;
            return dist;
        }
        public static void
        getEastWestBaseLineDir(this ObjectId idPoly, ref double dblAngTar, ref int intMark) {
            double pi = System.Math.PI;
            double dblLenMax = 0;

            List<SEG_PROP> segProps;
            Polyline polyTemp = null;
            ObjectId idPolyTemp = ObjectId.Null;
            using (BaseObjs._acadDoc.LockDocument()) {
                try {
                    using (Transaction tr = BaseObjs.startTransactionDb()) {
                        idPolyTemp = idPoly.copy("TEMP");

                        idPolyTemp.checkIfClosed();
                        idPolyTemp.removeDuplicatVertex();
                        polyTemp = (Polyline)idPolyTemp.getEnt();
                        Point2dCollection pnts2dPolyTemp = Conv.poly_pnt2dColl(polyTemp);

                        for (int i = 1; i < pnts2dPolyTemp.Count - 1; i++) {
                            Point2d pnt2d1 = pnts2dPolyTemp[i - 1];
                            Point2d pnt2d2 = pnts2dPolyTemp[i + 0];
                            Point2d pnt2d3 = pnts2dPolyTemp[i + 1];

                            double dblAng3Pnts = System.Math.Round(Geom.getAngle3Points2d(pnt2d1, pnt2d2, pnt2d3), 3);

                            if (dblAng3Pnts == 0 || dblAng3Pnts == System.Math.Round(System.Math.PI, 3)) {
                                polyTemp.RemoveVertexAt(i);
                            }
                        }
                        tr.Commit();
                    }
                }
                catch (System.Exception ex) {
                BaseObjs.writeDebug(ex.Message + " Geom.cs: line: 550");
                }

                segProps = polyTemp.getSegProps();
                Misc.deleteObj(idPolyTemp);
            }

            for (int i = 0; i < segProps.Count; i++) {
                if (segProps[i].LENGTH >= dblLenMax) {
                    if (pi / 2 >= segProps[i].DIR_AHEAD & segProps[i].DIR_AHEAD >= 0) {
                        dblLenMax = segProps[i].LENGTH;
                        dblAngTar = segProps[i].DIR_AHEAD;
                        intMark = i;
                    }else if (3 * pi / 2 < segProps[i].DIR_AHEAD & segProps[i].DIR_AHEAD < 2 * pi) {
                        dblLenMax = segProps[i].LENGTH;
                        dblAngTar = segProps[i].DIR_AHEAD;
                        intMark = i;
                    }
                }
            }
        }

        /// <summary>
        /// calculate distance along vector defined by two points of a third point projected to the vector
        /// </summary>
        /// <param name="pnt3dA1"></param>
        /// <param name="pnt3dA2"></param>
        /// <param name="pnt3dB"></param>
        /// <returns>double</returns>
        public static double
        getPerpDistToLine(Point3d pnt3dA1, Point3d pnt3dA2, Point3d pnt3dB) {
            double dblDist = 0;

            Point3d pnt3dAdxdy = new Point3d(pnt3dA2.X - pnt3dA1.X, pnt3dA2.Y - pnt3dA1.Y, 0.0);
            Point3d pnt3dBdxdy = new Point3d(pnt3dB.X - pnt3dA1.X, pnt3dB.Y - pnt3dA1.Y, 0.0);

            double dblLenA = get2Ddistance(pnt3dA1, pnt3dA2);
            double dblDot = get2dDotProduct(pnt3dAdxdy, pnt3dBdxdy);

            if (dblLenA == 0) {
                dblDist = -1;
            }else {
                dblDist = dblDot / dblLenA;
            }

            return dblDist;
        }

        public static double
        getPerpDistToLine(Point2d pnt2dA1, Point2d pnt2dA2, Point3d pnt3dB) {
            double dblDist = 0;

            Point3d pnt3dAdxdy = new Point3d(pnt2dA2.X - pnt2dA1.X, pnt2dA2.Y - pnt2dA1.Y, 0.0);
            Point3d pnt3dBdxdy = new Point3d(pnt3dB.X - pnt2dA1.X, pnt3dB.Y - pnt2dA1.Y, 0.0);

            double dblLenA = pnt2dA1.GetDistanceTo(pnt2dA2);
            Point2d pnt2dB = new Point2d(pnt3dB.X, pnt3dB.Y);
            double dblDot = (pnt2dA2 - pnt2dA1).DotProduct(pnt2dB - pnt2dA1);

            if (dblLenA == 0) {
                dblDist = -1;
            }else {
                dblDist = dblDot / dblLenA;
            }

            return dblDist;
        }

        public static double
        getPerpDistToLine(Point3d pnt3dA1, Point3d pnt3dA2, Point2d pnt2dB) {
            double dblDist = 0;

            Point3d pnt3dAdxdy = new Point3d(pnt3dA2.X - pnt3dA1.X, pnt3dA2.Y - pnt3dA1.Y, 0.0);
            Point3d pnt3dBdxdy = new Point3d(pnt2dB.X - pnt3dA1.X, pnt2dB.Y - pnt3dA1.Y, 0.0);

            double dblLenA = get2Ddistance(pnt3dA1, pnt3dA2);
            double dblDot = get2dDotProduct(pnt3dAdxdy, pnt3dBdxdy);

            if (dblLenA == 0) {
                dblDist = -1;
            }else {
                dblDist = dblDot / dblLenA;
            }

            return dblDist;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnt3dA1"></param>
        /// <param name="pnt3dA2"></param>
        /// <param name="pnt3dB1"></param>
        /// <param name="pnt3dB2"></param>
        /// <param name="includeBothEndsBndrySeg"></param>
        /// <param name="extend"></param>
        /// <returns></returns>
        public static Point3d
        getPntInt(Point3d pnt3dA1, Point3d pnt3dA2, Point3d pnt3dB1, Point3d pnt3dB2, bool includeBothEndsBndrySeg, extend ext) {
            int extendOption = (int)ext;

            double dblNumerUa, dblNumerUb, dblDenomUaUb, Ua, Ub;
            Point3d pnt3dInt = Pub.pnt3dO;
            Point3d pnt3dIntX = Pub.pnt3dO;

            double X1 = pnt3dA1.X;
            double Y1 = pnt3dA1.Y;
            double X2 = pnt3dA2.X;
            double Y2 = pnt3dA2.Y;

            double X3 = pnt3dB1.X;
            double Y3 = pnt3dB1.Y;
            double X4 = pnt3dB2.X;
            double Y4 = pnt3dB2.Y;

            dblNumerUa = (X4 - X3) * (Y1 - Y3) - (Y4 - Y3) * (X1 - X3);
            dblNumerUb = (X2 - X1) * (Y1 - Y3) - (Y2 - Y1) * (X1 - X3);
            dblDenomUaUb = (Y4 - Y3) * (X2 - X1) - (X4 - X3) * (Y2 - Y1);

            if (dblDenomUaUb == 0) {
                if (dblNumerUa == 0 & dblNumerUb == 0) { 
                    pnt3dInt = new Point3d(-9, -9, -9); //Target and segment are coincident
                    return pnt3dInt;
                }else { 
                    pnt3dInt = Pub.pnt3dO;              //Target and segment are parallel
                    return pnt3dInt;
                }
            }else {
                Ua = System.Math.Round(dblNumerUa / dblDenomUaUb, 8);
                Ub = System.Math.Round(dblNumerUb / dblDenomUaUb, 8);
                
                if (Ua >= 0d & Ua <= 1d) { //include both/either end of breakline segment
                    if (includeBothEndsBndrySeg == true) {
                        //include end point of boundary segment and beg point (same as endpoint of previous segment)
                        if (Ub >= 0d & Ub <= 1d) {
                            pnt3dInt = new Point3d(X1 + Ua * (X2 - X1), Y1 + Ua * (Y2 - Y1), 0d);
                        }else {
                            pnt3dInt = Pub.pnt3dO;
                        }
                    }else {
                        //exclude beg point (same as endpoint of previous segment) and include end point of boundary segment
                        if (Ub > 0d & Ub <= 1d) {
                            pnt3dInt = new Point3d(X1 + Ua * (X2 - X1), Y1 + Ua * (Y2 - Y1), 0d);
                        }else {
                            pnt3dInt = Pub.pnt3dO;
                        }
                    }
                }else if (extendOption == 0) {
                    pnt3dInt = Pub.pnt3dO;
                }else if (extendOption == 1) {
                    pnt3dInt = new Point3d(X1 + Ua * (X2 - X1), Y1 + Ua * (Y2 - Y1), 0d);

                    double dirSegX = Base_Tools45.Math.roundDown3(pnt3dB1.getDirection(pnt3dB2));
                    double dirSeg = Base_Tools45.Math.roundDown3(pnt3dB1.getDirection(pnt3dInt));

                    double distSegX = Base_Tools45.Math.roundDown3(pnt3dB1.getDistance(pnt3dB2));
                    double distSeg = Base_Tools45.Math.roundDown3(pnt3dB1.getDistance(pnt3dInt));

                    if (dirSeg != dirSegX) {
                        pnt3dInt = Pub.pnt3dO;
                    }else {
                        if (distSeg > distSegX) {
                            pnt3dInt = Pub.pnt3dO;
                        }
                    }
                }else if (extendOption == 2) {
                    pnt3dInt = new Point3d(X1 + Ua * (X2 - X1), Y1 + Ua * (Y2 - Y1), 0d);
                    pnt3dIntX = new Point3d(X3 + Ub * (X4 - X3), Y3 + Ub * (Y4 - Y3), 0d);
                    double dX = Base_Tools45.Math.roundDown3(pnt3dInt.X - pnt3dIntX.X);
                    double dY = Base_Tools45.Math.roundDown3(pnt3dInt.Y - pnt3dIntX.Y);
                    if (System.Math.Abs(dX) != 0.0 || System.Math.Abs(dY) != 0.0) {
                        pnt3dInt = Pub.pnt3dO;
                    }
                }
            }
            return pnt3dInt;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnt3dA1"></param>
        /// <param name="pnt3dA2"></param>
        /// <param name="pnts3d"></param>
        /// <param name="includeBothEndsBndrySeg"></param>
        /// <param name="extend"></param>
        /// <returns></returns>
        public static List<Point3d>
        getPntInts(Point3d pnt3dA1, Point3d pnt3dA2, List<Point3d> pnts3d, bool includeBothEndsBndrySeg, extend ext) {
            List<Point3d> pnt3dInts = new List<Point3d>();

            Point3d pnt3dB1, pnt3dB2;
            Point3d pnt3dInt;

            bool boolHit = false, boolMiss = false, boolCoincident = false;

            for (int i = 0; i < pnts3d.Count - 1; i = i + 1) {
                pnt3dB1 = pnts3d[i + 0];
                pnt3dB2 = pnts3d[i + 1];
                pnt3dInt = getPntInt(pnt3dA1, pnt3dA2, pnt3dB1, pnt3dB2, includeBothEndsBndrySeg, ext);

                if (pnt3dInt.X == -1)
                    boolMiss = true;
                else if (pnt3dInt.X == -9)
                    boolCoincident = true;
                else {
                    boolHit = true;
                    pnt3dInts.Add(pnt3dInt);
                }
            }
            if (boolMiss = true && boolHit == false)
                pnt3dInts.Add(Pub.pnt3dO);
            if (pnt3dInts.Count == 2)
                if (pnt3dInts[0].X == pnt3dInts[1].X && pnt3dInts[0].Y == pnt3dInts[1].Y) {
                    pnt3dInts.Clear();
                    pnt3dInts.Add(Pub.pnt3dO);
                }
            if (boolCoincident == true) {
                pnt3dInts.Clear();
                pnt3dInts.Add(new Point3d(-9, -9, -9));
            }
            return pnt3dInts;
        }

        /// <summary>
        /// calculate point from base point using length and direction
        /// </summary>
        /// <param name="pnt3dBase"></param>
        /// <param name="dblAng"></param>
        /// <param name="dblLen"></param>
        /// <returns>Point3d</returns>
        public static Point3d
        getPolarPnt(Point3d pnt3dBase, double dblAng, double dblLen) {
            Point3d pnt3d = new Point3d(pnt3dBase.X + dblLen * System.Math.Cos(dblAng), pnt3dBase.Y + dblLen * System.Math.Sin(dblAng), 0.0);
            return pnt3d;
        }

        /// <summary>
        /// get list of Point3d from Polyline3d
        /// </summary>
        /// <param name="poly3d"></param>
        /// <returns>List of Point3d</returns>
        public static List<Point3d>
        getPoly3dVertexs(Polyline3d poly3d) {
            List<Point3d> pnts3d = new List<Point3d>();

            foreach (Point3d pnt3d in poly3d) {
                pnts3d.Add(pnt3d);
            }

            return pnts3d;
        }

        /// <summary>
        /// get list of Point2d from Polyline
        /// </summary>
        /// <param name="poly"></param>
        /// <returns>list of Point2d</returns>
        public static List<Point2d>
        getPolyPoint2ds(Polyline poly) {
            List<Point2d> pnts2d = new List<Point2d>();

            for (int i = 0; i < poly.NumberOfVertices; i++) {
                pnts2d.Add(poly.GetPoint2dAt(i));
            }

            return pnts2d;
        }

        public static double
        getRadius(Point3d pnt3dBeg, Point3d pnt3dEnd, double bulge) {
            double dblDelta = getDelta(bulge);
            double dblLenChord = Base_Tools45.Measure.getDistance2d(pnt3dEnd, pnt3dBeg);
            return System.Math.Abs((dblLenChord / 2) / System.Math.Sin(dblDelta / 2));
        }

        public static int getSide(List<Point3d> pnts3d) {
            int intSide = 0;
            PromptPointOptions PPO = new PromptPointOptions("Select a location near the begin point of reference line and on the side to grade to.");
            PromptPointResult PPR = null;

            try {
                PPR = BaseObjs._editor.GetPoint(PPO);
            }
            catch (Exception) {
                intSide = 0;
            }

            if (PPR.Status == PromptStatus.OK) {
                Point3d pnt3d = PPR.Value;

                if (pnt3d.isRightSide(pnts3d[0], pnts3d[1])) {
                    intSide = 1;
                }else {
                    intSide = -1;
                }
            }

            return intSide;
        }

        public static staOffElev
        getStaOff(Point3d pnt3dB, Point3d pnt3dE, Point3d pnt3dX) {

            staOffElev soe = new staOffElev();

            double dblLen0 = get2Ddistance(pnt3dB, pnt3dE);
            double dblLenX = get2Ddistance(pnt3dB, pnt3dX);

            Point3d v2d0 = new Point3d(pnt3dE.X - pnt3dB.X, pnt3dE.Y - pnt3dB.Y, 0.0);
            Point3d v2dX = new Point3d(pnt3dX.X - pnt3dB.X, pnt3dX.Y - pnt3dB.Y, 0.0);

            double dblAng = getAngle2Vectors(v2d0, v2dX, dblLen0, dblLenX);

            soe.staSeg = System.Math.Cos(dblAng) * dblLenX;
            soe.off =    System.Math.Sin(dblAng) * dblLenX;

            if(Geom.testRight(pnt3dB, pnt3dE, pnt3dX) > 0){
                soe.off *= -1;    
            }
            soe.elev = pnt3dX.Z;
            return soe;
        }

        public static staOffElev
        getStaOff(Point2d pnt2dB, Point2d pnt2dE, Point3d pnt3dX)
        {

            staOffElev soe = new staOffElev();

            double dblLen0 = pnt2dB.getDistance(pnt2dE);
            double dblLenX = pnt2dB.getDistance(pnt3dX);

            Point3d v2d0 = new Point3d(pnt2dE.X - pnt2dB.X, pnt2dE.Y - pnt2dB.Y, 0.0);
            Point3d v2dX = new Point3d(pnt3dX.X - pnt2dB.X, pnt3dX.Y - pnt2dB.Y, 0.0);

            double dblAng = getAngle2Vectors(v2d0, v2dX, dblLen0, dblLenX);

            soe.staSeg = System.Math.Cos(dblAng) * dblLenX;
            soe.off = System.Math.Sin(dblAng) * dblLenX;

            if(Geom.testRight(pnt2dB, pnt2dE, pnt3dX.Convert2d(BaseObjs.xyPlane)) > 0){     //testRight is positive when turning according to righthand rule
                soe.off *= -1;                                                              //if turning right from baseline, then target is on left side
            }

            soe.elev = pnt3dX.Z;
            return soe;
        }


        public static void
        getStaOff(List<ObjectId> idCgPnts, Point3d pnt3dIns, ref double distX, ref double distY)
        {
            Point3d pnt3d1 = idCgPnts[0].getCogoPntCoordinates();
            Point3d pnt3d2 = idCgPnts[1].getCogoPntCoordinates();

            double distH = pnt3d1.getDistance(pnt3dIns);
            Vector3d v3dBase = pnt3d2 - pnt3d1;
            Vector3d v3dTar = pnt3dIns - pnt3d1;

            double alpha = Geom.getAngle2Vectors(v3dTar, v3dBase);
            distX = distH * System.Math.Cos(alpha);
            distY = distH * System.Math.Sin(alpha);
        }

        public static void
        getStaOff(Point3d pnt3d1, Point3d pnt3d2, Point3d pnt3dIns, ref double distX, ref double distY)
        {
            double distH = pnt3d1.getDistance(pnt3dIns);

            Vector3d v3dBase = pnt3d2 - pnt3d1;
            Vector3d v3dTar = pnt3dIns - pnt3d1;

            double alpha = Geom.getAngle2Vectors(v3dTar, v3dBase);
            distX = distH * System.Math.Cos(alpha);
            distY = distH * System.Math.Sin(alpha);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="poly3d"></param>
        /// <param name="pnt3dPick"></param>
        /// <returns></returns>
        public static int
        getVertexNo(ObjectId id, Point3d pnt3dPick)
        {
            Point3dCollection pnts3d = null;
            Entity ent = id.getEnt();

            if (ent.GetType() == typeof(Polyline))
            {
                Polyline poly = (Polyline)ent;
                pnts3d = poly.getCoordinates3d();
            }
            else if (ent.GetType() == typeof(Polyline3d))
            {
                Polyline3d poly3d = (Polyline3d)ent;
                pnts3d = poly3d.getCoordinates3d();
            }
            int k = pnts3d.Count;

            if (k == 2)
                return 0;
            Point3d pnt3dBeg, pnt3dEnd;

            SortedDictionary<double, int> offSets = new SortedDictionary<double, int>();
            for (int i = 0; i < k - 1; i++)
            {
                pnt3dBeg = pnts3d[i];
                pnt3dEnd = pnts3d[i + 1];

                double length = pnt3dBeg.getDistance(pnt3dEnd);

                double dist = pnt3dPick.getOrthoDist(pnt3dBeg, pnt3dEnd);
                if (dist > 0 && dist < length)
                {
                    double dir = pnt3dBeg.getDirection(pnt3dEnd);
                    Point3d pnt3dBase = pnt3dBeg.traverse(dir, dist);
                    //Point3d pnt3dBase = new Point3d(pnt3dBeg.X + dist / length * (pnt3dBeg.X - pnt3dEnd.X),
                    //    pnt3dBeg.Y + dist / length * (pnt3dBeg.Y - pnt3dEnd.Y),
                    //    0);
                    offSets.Add(pnt3dBase.getDistance(pnt3dPick), i);
                }
            }

            KeyValuePair<double, int> data = offSets.ElementAt(0);
            return data.Value;
        }

        public static int
        getVertexNo(Point3dCollection pnts3d, Point3d pnt3dPick)
        {
            int k = pnts3d.Count;

            if (k == 2)
                return 0;
            Point3d pnt3dBeg, pnt3dEnd;

            SortedDictionary<double, int> offSets = new SortedDictionary<double, int>();
            for (int i = 0; i < k - 1; i++)
            {
                pnt3dBeg = pnts3d[i];
                pnt3dEnd = pnts3d[i + 1];

                double length = pnt3dBeg.getDistance(pnt3dEnd);

                double dist = pnt3dPick.getOrthoDist(pnt3dBeg, pnt3dEnd);
                if (dist > 0 && dist < length)
                {
                    Point3d pnt3dBase = new Point3d(pnt3dBeg.X + dist / length * (pnt3dBeg.X - pnt3dEnd.X),
                        pnt3dBeg.Y + dist / length * (pnt3dBeg.Y - pnt3dEnd.Y),
                        0);
                    offSets.Add(pnt3dBase.getDistance(pnt3dPick), i);
                }
            }

            KeyValuePair<double, int> data = offSets.ElementAt(0);
            return data.Value;
        }

        public static int
        getVertexNo(List<Point3d> pnts3d, Point3d pnt3dPick)
        {
            int k = pnts3d.Count;

            if (k == 2)
                return 0;

            Point3d pnt3dBeg, pnt3dEnd;

            SortedDictionary<double, int> offSets = new SortedDictionary<double, int>();
            for (int i = 0; i < k - 1; i++)
            {
                pnt3dBeg = pnts3d[i];
                pnt3dEnd = pnts3d[i + 1];

                double length = pnt3dBeg.getDistance(pnt3dEnd);

                double dist = pnt3dPick.getOrthoDist(pnt3dBeg, pnt3dEnd);
                if (dist > 0 && dist < length)
                {
                    Point3d pnt3dBase = new Point3d(pnt3dBeg.X + dist / length * (pnt3dEnd.X - pnt3dBeg.X),
                        pnt3dBeg.Y + dist / length * (pnt3dEnd.Y - pnt3dBeg.Y),
                        0);
                    offSets.Add(pnt3dBase.getDistance(pnt3dPick), i);
                }
            }

            KeyValuePair<double, int> data = offSets.ElementAt(0);
            return data.Value;
        }

        public static bool
        intersectsArc(this List<Point3d> pnts3d, Arc arc, out List<Point3d> pnts3dInt, ref ObjectIdCollection idsDelete)
        {
            pnts3dInt = new List<Point3d>();

            Point3d pnt3dCen = arc.Center;
            Point3d pnt3dBeg = arc.StartPoint;
            Point3d pnt3dEnd = arc.EndPoint;

            //double r = arc.Radius;
            double r = pnt3dCen.getDistance(pnt3dBeg);

            double Xc = pnt3dCen.X;
            double Yc = pnt3dCen.Y;

            double X1 = pnts3d[0].X;
            double Y1 = pnts3d[0].Y;

            double X2 = pnts3d[1].X;
            double Y2 = pnts3d[1].Y;

            double dx = X2 - X1;
            double dy = Y2 - Y1;

            double a = dx * dx + dy * dy;
            double b = 2 * (dx * (X1 - Xc) + dy * (Y1 - Yc));
            double c = Xc * Xc + Yc * Yc;
            c += X1 * X1 + Y1 * Y1;
            c -= 2 * (Xc * X1 + Yc * Y1);
            c -= r * r;

            bool bResult = false;

            Vector3d v3d = Vector3d.XAxis;
            Point3d pnt3dX = Pub.pnt3dO;        //point on line perp to arc centerpoint
            ObjectId idCircle = ObjectId.Null;
            double dist = 0.0;

            v3d = pnts3d[1] - pnts3d[0];
            double u = -b / (2 * a);

            try
            {
                if (u == 0)
                    return false;
                else if (u < 0)
                    pnt3dX = pnts3d[0];
                else if (u > 1)
                    pnt3dX = pnts3d[1];
                else
                    pnt3dX = pnts3d[0] + v3d * u;

                //idCircle = Draw.addCircle(pnt3dX, 1.0, "0", (short)1);
                //idsDelete.Add(idCircle);
                //BaseObjs.updateGraphics();

                dist = pnt3dCen.getDistance(pnt3dX);
                bool keep = false;

                try
                {
                    if (dist > r)
                        return false;
                    else
                    {
                        double d1 = b * b - 4 * a * c;
                        if (d1 < 0)
                            return false;
                        else if (d1 == 0)
                        {
                            double p1 = -b / (2 * a);
                            Point3d pnt3dInt = pnts3d[0] + p1 * v3d;
                            keep = pnt3dInt.isOnArc(arc, ref idsDelete);
                            if (keep)
                            {
                                pnts3dInt.Add(pnt3dInt);
                                //idCircle = Draw.addCircle(pnt3dInt, 1.0, "0", (short)2);
                                //idsDelete.Add(idCircle);
                                //BaseObjs.updateGraphics();
                                return true;
                            }
                            else
                                return false;
                        }
                        else
                        {
                            d1 = System.Math.Sqrt(d1);
                            double p1 = (-b + d1) / (2 * a);
                            double p2 = (-b - d1) / (2 * a);

                            if (p2 >= 0 && p2 <= 1)
                            {
                                Point3d pnt3dInt = pnts3d[0] + p2 * v3d;
                                keep = pnt3dInt.isOnArc(arc, ref idsDelete);
                                if (keep)
                                {
                                    pnts3dInt.Add(pnt3dInt);
                                    //idCircle = Draw.addCircle(pnt3dInt, 1.0, "0", (short)3);
                                    //idsDelete.Add(idCircle);
                                    //BaseObjs.updateGraphics();
                                    bResult = true;
                                }
                            }
                            if (p1 >= 0 && p1 <= 1)
                            {
                                Point3d pnt3dInt = pnts3d[0] + p1 * v3d;
                                keep = pnt3dInt.isOnArc(arc, ref idsDelete);
                                if (keep)
                                {
                                    pnts3dInt.Add(pnt3dInt);
                                    //idCircle = Draw.addCircle(pnt3dInt, 1.0, "0", (short)4);
                                    //idsDelete.Add(idCircle);
                                    //BaseObjs.updateGraphics();
                                    bResult = true;
                                }
                            }
                            return bResult;
                        }
                    }
                }
                catch (Autodesk.AutoCAD.Runtime.Exception )
                {
                }
            }
            catch (System.Exception )
            {
            }
            return bResult;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="entX"></param>
        /// <param name="includeBothEndsBndrySeg"></param>
        /// <param name="extendEnt"></param>
        /// <returns></returns>
        public static List<Point3d>
        intersectWith(this Entity ent, Entity entX, bool includeBothEndsBndrySeg, extend ext)
        {
            List<Point3d> pnts3d = new List<Point3d>();
            List<Point3d> pnts3dX = new List<Point3d>();
            List<Point3d> pnt3dInts = new List<Point3d>();

            if (entX is Line)
            {
                Line lineX = (Line)entX;
                pnts3dX.Add(lineX.StartPoint);
                pnts3dX.Add(lineX.EndPoint);
            }
            else if (entX is Polyline)
            {
                pnts3dX = entX.ObjectId.getCoordinates3dList();
            }
            else if (entX is Polyline3d)
            {
                pnts3dX = entX.ObjectId.getCoordinates3dList();
            }

            if (ent is Line)
            {
                Line line = (Line)ent;
                pnt3dInts = getPntInts(line.StartPoint, line.EndPoint, pnts3dX, includeBothEndsBndrySeg, ext);
            }
            else if (ent is Polyline)
            {
                pnts3d = ent.ObjectId.getCoordinates3dList();
                pnt3dInts = getPntInts(pnts3d[0], pnts3d[1], pnts3dX, includeBothEndsBndrySeg, ext);
            }
            return pnt3dInts;
        }

        public static List<Point3d>
        intersectWithArc(this List<Point3d> pnts3d, Arc arc)
        {
            List<Point3d> pnts3dInt = new List<Point3d>();
            C2DLine line = new C2DLine(pnts3d[0].X, pnts3d[0].Y, pnts3d[1].X, pnts3d[1].Y);
            C2DArc arc2 = new C2DArc();
            //incomplete
            return pnts3dInt;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnt3dX"></param>
        /// <param name="pnts3d"></param>
        /// <param name="includeBothEndsBndrySeg"></param>
        /// <returns></returns>
        public static bool
        isInside(this Point3d pnt3dX, List<Point3d> pnts3d, bool includeBothEndsBndrySeg = true)
        {
            Point3d pnt3dPolar;
            List<Point3d> pnts3dInt = new List<Point3d>();
            List<Point3d> pnts3dTmp = new List<Point3d>();
            Double angle;
            Double dX, dY;
            bool isInside = false;

            pnt3dPolar = traverse_pnt3d(pnt3dX, System.Math.PI / 4, 5000.0);

            pnts3dInt = getPntInts(pnt3dX, pnt3dPolar, pnts3d, includeBothEndsBndrySeg, 0);

            if (pnts3dInt[0].X != -1 && pnts3dInt[0].X != -9)
            {
                for (int i = 0; i < pnts3dInt.Count; i++)
                {
                    dX = System.Math.Round(pnts3dInt[i].X - pnt3dX.X, 3);
                    dY = System.Math.Round(pnts3dInt[i].Y - pnt3dX.Y, 3);
                    angle = System.Math.Round(Base_Tools45.Measure.getAzRadians(dX, dY), 3);
                    if (angle == System.Math.Round(System.Math.PI / 4, 3) || angle == 0)
                    {
                        pnts3dTmp.Add(pnts3dInt[i]);
                    }
                }
                if (pnts3dTmp.Count == 0)
                    isInside = false;
                else if (pnts3dTmp.Count % 2 != 0)
                    isInside = true;
                else
                    isInside = false;
            }
            else
                isInside = false;
            return isInside;
        }

        private static bool
        isOnArc(this Point3d pnt3dX, Arc arc, ref ObjectIdCollection idsDelete)
        {
            Vector3d v3dN = arc.Normal;
            double r = arc.Radius;

            double dirM = 0.0;

            if (v3dN.Z > 0)
                dirM = arc.StartAngle + arc.TotalAngle / 2;
            else
                dirM = arc.StartAngle - arc.TotalAngle / 2;

            Point3d pnt3dCen = arc.Center;
            Point3d pnt3dBeg = arc.StartPoint;
            Point3d pnt3dEnd = arc.EndPoint;

            Point3d pnt3dMid = pnt3dCen.traverse(dirM, r);
            //ObjectId idLine = Draw.addLine(pnt3dCen, pnt3dMid);
            //idsDelete.Add(idLine);
            //idLine.changeProp(color: (short)1);
            //BaseObjs.updateGraphics();

            Vector3d v3dBE = pnt3dEnd - pnt3dBeg;
            //idLine = Draw.addLine(pnt3dBeg, pnt3dEnd);
            //idsDelete.Add(idLine);
            //idLine.changeProp(color: (short)1);
            //BaseObjs.updateGraphics();

            Vector3d v3dBM = pnt3dMid - pnt3dBeg;
            //idLine = Draw.addLine(pnt3dBeg, pnt3dMid);
            //idsDelete.Add(idLine);
            //idLine.changeProp(color: (short)1);
            //BaseObjs.updateGraphics();

            Vector3d v3dBX = pnt3dX - pnt3dBeg;
            //idLine = Draw.addLine(pnt3dBeg, pnt3dX);
            //idsDelete.Add(idLine);
            //idLine.changeProp(color: (short)3);
            //BaseObjs.updateGraphics();

            Vector3d v3dBE_BM = v3dBE.CrossProduct(v3dBM);
            Vector3d v3dBE_BX = v3dBE.CrossProduct(v3dBX);

            if (v3dBE_BM.Z < 0 && v3dBE_BX.Z > 0)
                return false;
            else if (v3dBE_BM.Z < 0 && v3dBE_BX.Z > 0)
                return false;
            else
                return true;
        }


        /// <summary>
        /// test if target point is on line segment
        /// </summary>
        /// <param name="pnt3dX"></param>
        /// <param name="pnt3dBEG"></param>
        /// <param name="pnt3dEND"></param>
        /// <returns>bool</returns>
        public static bool
        isOn2dSegment(Point3d pnt3dX, Point3d pnt3dBEG, Point3d pnt3dEND)
        {
            double dblDX1 = pnt3dEND.X - pnt3dBEG.X;
            double dblDY1 = pnt3dEND.Y - pnt3dBEG.Y;
            double dblLen1 = System.Math.Sqrt(System.Math.Pow(dblDX1, 2) + System.Math.Pow(dblDY1, 2));

            double dblDX2 = pnt3dX.X - pnt3dBEG.X;
            double dblDY2 = pnt3dX.Y - pnt3dBEG.Y;
            double dblLen2 = System.Math.Sqrt(System.Math.Pow(dblDX2, 2) + System.Math.Pow(dblDY2, 2));

            double dblResult = dblDX1 * dblDY2 - dblDX2 * dblDY1;

            //test if point is on line with same direction as boundary segment

            if (System.Math.Round(dblResult, 3) == 0.0)
            {
                //test if point is on segment
                if (dblLen2 <= dblLen1)
                {
                    double dblAng1 = Base_Tools45.Measure.getAzRadians(pnt3dBEG, pnt3dEND);
                    double dblAng2 = Base_Tools45.Measure.getAzRadians(pnt3dBEG, pnt3dX);

                    if (System.Math.Round(dblAng2, 3) == System.Math.Round(dblAng1, 3))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// test if target Point3d is on Polyline, or Polyline3d
        /// ignores Z
        /// </summary>
        /// <param name="pnt3dX"></param>
        /// <param name="pnts3d"></param>
        /// <returns>int as segment where located</returns>
        public static int
        isOn2dSegment(this Point3d pnt3dX, List<Point3d> pnts3d)
        {
            int x = -1;

            for (int i = 1; i < pnts3d.Count; i++)
            {
                Point3d pnt3dSegBeg = pnts3d[i - 1];
                Point3d pnt3dSegEnd = pnts3d[i - 0];

                double dblDX1 = pnt3dSegEnd.X - pnt3dSegBeg.X;
                double dblDY1 = pnt3dSegEnd.Y - pnt3dSegBeg.Y;
                double dblLen1 = pnt3dSegBeg.getDistance(pnt3dSegEnd);

                double dblDX2 = pnt3dX.X - pnt3dSegBeg.X;
                double dblDY2 = pnt3dX.Y - pnt3dSegBeg.Y;
                double dblLen2 = pnt3dSegBeg.getDistance(pnt3dX);

                double dblResult = dblDX1 * dblDY2 - dblDX2 * dblDY1;

                if (System.Math.Round(dblResult, 3) == 0.0)
                {
                    if (dblLen2 <= dblLen1)
                    {
                        double dblAng1 = pnt3dSegBeg.getDirection(pnt3dSegEnd);
                        double dblAng2 = pnt3dSegBeg.getDirection(pnt3dX);

                        if (System.Math.Round(dblAng2, 3) == System.Math.Round(dblAng1, 3))
                        {
                            x = i;
                            break;
                        }
                        else
                        {
                            x = -1;
                        }
                    }
                    else
                    {
                        x = -1;
                    }
                }
            }
            return x;
        }

        /// <summary>
        /// determine if Polyline3d is drawing CCW or CC
        /// </summary>
        /// <param name="poly3d"></param>
        /// <returns>bool</returns>
        public static bool
        isRightHandPoly(Polyline3d poly3d)
        {
            double dblX1 = 0;
            double dblY1 = 0;
            double dblX2 = 0;
            double dblY2 = 0;

            double dblArea = 0;

            List<Point3d> pnts3d = getPoly3dVertexs(poly3d);

            for (int i = 0; i < pnts3d.Count - 1; i++)
            {
                dblX1 = pnts3d[i + 0].X;
                dblY1 = pnts3d[i + 0].Y;

                dblX2 = pnts3d[i + 1].X;
                dblY2 = pnts3d[i + 1].Y;

                dblArea = dblArea + (dblX1 * dblY2) - (dblX2 * dblY1);
            }

            if (dblArea > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idPoly"></param>
        /// <returns></returns>
        public static bool
        isRightHandPoly(ObjectId idPoly)
        {
            double dblX1 = 0;
            double dblY1 = 0;
            double dblX2 = 0;
            double dblY2 = 0;

            double dblArea = 0;
            Polyline poly = null;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    try
                    {
                        poly = (Polyline)tr.GetObject(idPoly, OpenMode.ForRead);
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Geom.cs: line: 1478");
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Geom.cs: line: 1484");
            }

            List<Point2d> pnts2d = getPolyPoint2ds(poly);

            for (int i = 0; i < pnts2d.Count - 1; i++)
            {
                dblX1 = pnts2d[i + 0].X;
                dblY1 = pnts2d[i + 0].Y;

                dblX2 = pnts2d[i + 1].X;
                dblY2 = pnts2d[i + 1].Y;

                dblArea = dblArea + (dblX1 * dblY2) - (dblX2 * dblY1);
            }

            if (dblArea > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// find midpoint between two points
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public static Point3d
        midpoint(Point3d pt1, Point3d pt2)
        {
            Point3d newPt = new Point3d(((pt1.X + pt2.X) / 2.0),
                ((pt1.Y + pt2.Y) / 2.0),
                ((pt1.Z + pt2.Z) / 2.0));

            return newPt;
        }

        /// <summary>
        /// convert Point3d to double(2)
        /// </summary>
        /// <param name="pnt3d"></param>
        /// <returns></returns>
        public static double[]
        pnt3dTo2dblPnt(Point3d pnt3d)
        {
            double[] dblPnt = new double[3];

            dblPnt[0] = pnt3d.X;
            dblPnt[1] = pnt3d.Y;
            dblPnt[2] = pnt3d.Z;

            return dblPnt;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnts3d"></param>
        /// <returns></returns>
        public static Point3dCollection
        reversePnt3dColl(Point3dCollection pnts3d)
        {
            int k = pnts3d.Count;

            Point3dCollection pnt3dsNew = new Point3dCollection();

            for (int i = 0; i < k; i++)
            {
                pnt3dsNew.Add(pnts3d[k - 1 - i]);
            }
            return pnt3dsNew;
        }

        /// <summary>
        /// reverse list of Point3d
        /// </summary>
        /// <param name="pnts3d"></param>
        /// <returns>list of point3d</returns>
        public static List<Point3d>
        reversePnt3dList(this List<Point3d> pnts3d)
        {
            int k = pnts3d.Count;

            List<Point3d> pnt3dsNew = new List<Point3d>();

            for (int i = 0; i < k; i++)
            {
                pnt3dsNew.Add(pnts3d[k - 1 - i]);
            }
            return pnt3dsNew;
        }

        /// <summary>
        /// determine if two 2d line segments are drawn rotating CCW or CC
        /// </summary>
        /// <param name="dx1"></param>
        /// <param name="dy1"></param>
        /// <param name="dx2"></param>
        /// <param name="dy2"></param>
        /// <returns>double</returns>
        public static double
        testRight(double dx1, double dy1, double dx2, double dy2)
        {
            return dx1 * dy2 - dx2 * dy1;
            //right hand rule
        }

        /// <summary>
        /// determine if pick point is on right side of segment
        /// </summary>
        /// <param name="pnt3dBeg"></param>
        /// <param name="pnt3dEnd"></param>
        /// <param name="pnt3dX"></param>
        /// <returns>double</returns>
        public static double
        testRight(Point3d pnt3dBeg, Point3d pnt3dEnd, Point3d pnt3dX)
        {
            double dblDX1 = 0;
            double dblDY1 = 0;
            double dblDX2 = 0;
            double dblDY2 = 0;

            dblDX1 = pnt3dEnd.X - pnt3dBeg.X;
            dblDY1 = pnt3dEnd.Y - pnt3dBeg.Y;

            dblDX2 = pnt3dX.X - pnt3dBeg.X;
            dblDY2 = pnt3dX.Y - pnt3dBeg.Y;

            return dblDX1 * dblDY2 - dblDX2 * dblDY1;
            //right hand rule
        }

        public static double
        testRight(Point2d pnt2dBeg, Point2d pnt2dEnd, Point2d pnt2dX)
        {
            double dblDX1 = 0;
            double dblDY1 = 0;
            double dblDX2 = 0;
            double dblDY2 = 0;

            dblDX1 = pnt2dEnd.X - pnt2dBeg.X;
            dblDY1 = pnt2dEnd.Y - pnt2dBeg.Y;

            dblDX2 = pnt2dX.X - pnt2dBeg.X;
            dblDY2 = pnt2dX.Y - pnt2dBeg.Y;

            return dblDX1 * dblDY2 - dblDX2 * dblDY1;
            //right hand rule
        }

        public static Point2d
        traverse_pnt2d(Point2d pnt2dBase, double dblAng, double dblLen)
        {
            return new Point2d(pnt2dBase.X + System.Math.Cos(dblAng) * dblLen, pnt2dBase.Y + System.Math.Sin(dblAng) * dblLen);
        }

        /// <summary>
        /// calculate point from base using length, direction and slope
        /// </summary>
        /// <param name="pnt3dBase"></param>
        /// <param name="dblAng"></param>
        /// <param name="dblLen"></param>
        /// <param name="dblSlope"></param>
        /// <returns>Point3d</returns>
        public static Point3d
        traverse_pnt3d(Point3d pnt3dBase, double dblAng, double dblLen, double dblSlope = 0)
        {
            return new Point3d(pnt3dBase.X + System.Math.Cos(dblAng) * dblLen, pnt3dBase.Y + System.Math.Sin(dblAng) * dblLen, pnt3dBase.Z + dblSlope * dblLen);
        }

        public static double
        vectorLength(this Vector2d v2d)
        {
            return System.Math.Sqrt(v2d.X * v2d.X + v2d.Y * v2d.Y);
        }

        public static double
        vectorLength(this Vector3d v3d)
        {
            return System.Math.Sqrt(v3d.X * v3d.X + v3d.Y * v3d.Y);
        }

    }//  Class Geom
}
