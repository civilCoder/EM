using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

//using Autodesk.Civil.DatabaseServices;
namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static class Conv
    {
        static Point3d pnt3dPicked = Pub.pnt3dO;

        private static double pi = System.Math.PI;

        public static List<POI>
        sortPOIbyStation(this List<POI> varPOI)
        {
            List<POI> poiOUT = new List<POI>();
            var sortSta = from s in varPOI
                          orderby s.Station ascending
                          select s;
            foreach (var s in sortSta)
                poiOUT.Add(s);
            return poiOUT;
        }

        public static Point3d
        objPoint3dToPoint3d(this Object obj)
        {
            Point3d pnt3d = Pub.pnt3dO;

            string[] xyz = obj.ToString().splitFields(' ');
            double x, y, z;
            double.TryParse(xyz[0], out x);
            double.TryParse(xyz[1], out y);
            double.TryParse(xyz[2], out z);

            pnt3d = new Point3d(x, y, z);

            return pnt3d;
        }

        public static ObjectId
        convertArcToPolyline(this Arc arc, string nameLayer)
        {
            Polyline poly = new Polyline();
            Point2d p2dBeg = arc.StartPoint.Convert2d(BaseObjs.xyPlane);
            Point2d p2dEnd = arc.EndPoint.Convert2d(BaseObjs.xyPlane);
            int n = (arc.Normal.Z > 0) ? 1 : -1;
            double bulge = System.Math.Tan(arc.TotalAngle / 4) * n;
            ObjectId idPoly = ObjectId.Null;
            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                BlockTableRecord ms = Blocks.getBlockTableRecordMS();
                poly.SetDatabaseDefaults();
                poly.Layer = nameLayer;
                poly.AddVertexAt(0, p2dBeg, bulge, 0, 0);
                poly.AddVertexAt(1, p2dEnd, 0, 0, 0);
                idPoly = ms.AppendEntity(poly);
                tr.AddNewlyCreatedDBObject(poly, true);
                tr.Commit();
            }
            return idPoly;
        }

        public static ObjectId
        convertLineToPolyline(this Line line, string nameLayer)
        {
            Point2d p2dBeg = line.StartPoint.Convert2d(BaseObjs.xyPlane);
            Point2d p2dEnd = line.EndPoint.Convert2d(BaseObjs.xyPlane);
            ObjectId idPoly = ObjectId.Null;
            Polyline poly = new Polyline();
            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                BlockTableRecord ms = Blocks.getBlockTableRecordMS();
                poly.SetDatabaseDefaults();
                poly.Layer = nameLayer;
                poly.AddVertexAt(0, p2dBeg, 0, 0, 0);
                poly.AddVertexAt(1, p2dEnd, 0, 0, 0);
                idPoly = ms.AppendEntity(poly);
                tr.AddNewlyCreatedDBObject(poly, true);
                tr.Commit();
            }
            return idPoly;
        }

        public static Polyline
        convertLineToPolyline(this ObjectId idLine, string nameLayer)
        {
            ObjectId idPoly = ObjectId.Null;
            Polyline poly = new Polyline();
            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                Line line = (Line)tr.GetObject(idLine, OpenMode.ForWrite);
                Point2d p2dBeg = line.StartPoint.Convert2d(BaseObjs.xyPlane);
                Point2d p2dEnd = line.EndPoint.Convert2d(BaseObjs.xyPlane);
                line.Erase();

                BlockTableRecord ms = Blocks.getBlockTableRecordMS();
                poly.SetDatabaseDefaults();
                poly.Layer = nameLayer;
                poly.AddVertexAt(0, p2dBeg, 0, 0, 0);
                poly.AddVertexAt(1, p2dEnd, 0, 0, 0);
                idPoly = ms.AppendEntity(poly);
                tr.AddNewlyCreatedDBObject(poly, true);
                tr.Commit();
            }
            return poly;
        }


        public static Point3dCollection
        coords3d_Coll(ObjectId idPoly3d)
        {
            Point3dCollection pnts3ds = new Point3dCollection();
            Point3d pnt3d;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Polyline3d poly3d = (Polyline3d)idPoly3d.GetObject(OpenMode.ForRead);
                    foreach (ObjectId objID in poly3d)
                    {
                        PolylineVertex3d v3d = (PolylineVertex3d)objID.GetObject(OpenMode.ForRead);
                        pnt3d = v3d.Position;
                        pnts3ds.Add(pnt3d);
                    }

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Conv.cs: line: 139");
            }

            return pnts3ds;
        }

        /// <summary>
        /// Convert DBPoint ObjectId array to Point3d Array using delegate
        /// Example from theswamp.org
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static Point3d[]
        dbPointArray_Point3dArray(ObjectId[] ids)
        {
            Point3d[] pts = Array.ConvertAll<ObjectId, Point3d>(ids, x => ((DBPoint)x.GetObject(OpenMode.ForRead)).Position);
            return pts;
        }

        public static string
        decimalToFraction(this double x, int accuracy = 16)
        {
            string result = string.Empty;

            double rem = x.mantissa();
            double rem1000 = rem * 10000;
            int w = int.Parse(Math.truncate(x).ToString());

            if (rem1000.mod(5000) == 0)
            {
                result = string.Format("{0} 1/2\"", w);
            }
            else if (rem1000.mod(2500) == 0)
            {
                int n = int.Parse(Math.truncate(rem1000 / 2500).ToString());
                result = string.Format("{0}-{1}/4\"", w, n);
            }
            else if (rem1000.mod(1250) == 0)
            {
                int n = int.Parse(Math.truncate(rem1000 / 1250).ToString());
                result = string.Format("{0}-{1}/8\"", w, n);
            }
            else
            { //if (rem1000.mod(0625) == 0) {
                int n = int.Parse(Math.truncate(rem1000 / 625).ToString());
                if (n % 2 == 0)
                {
                    n = n / 2;
                    result = string.Format("{0}-{1}/8\"", w, n);
                }
                else
                    result = string.Format("{0}-{1}/16\"", w, n);
            }//else{
            //int n = int.Parse(Math.truncate(rem1000 / 312.5).ToString());
            //result = string.Format("{0}-{1}/32\"", w, n);
            //}

            return result;
        }

        public static string
        toStringFraction(this double dec)
        {
            string num = dec.ToString();
            string[] nums = num.Split('.');
            string x = nums[nums.Length - 1];
            string y = "1";

            for (int i = 0; i < x.Length; i++)
            {
                y += "0";
            }

            Int64 gcf = GCF(Int64.Parse(x), Int64.Parse(y));
            if (Int64.Parse(nums[0]) != 0)
                return string.Format("{0} {1}/{2}", nums[0], Int64.Parse(x) / gcf, Int64.Parse(y) / gcf);
            else
                return string.Format("{0}/{1}", Int64.Parse(x) / gcf, Int64.Parse(y) / gcf);
        }

        public static Int64
        GCF(Int64 x, Int64 y)
        {
            x = System.Math.Abs(x);
            y = System.Math.Abs(y);
            Int64 z;
            do
            {
                z = x % y;
                if (z == 0)
                    return y;
                x = y;
                y = z;
            }
            while (true);
        }

        public static string
        DDtoDMS(double degrees, int decPlaces)
        {
            // Get d/m/s components
            double d = System.Math.Floor(degrees);
            degrees -= d;
            degrees *= 60;
            double m = System.Math.Floor(degrees);
            degrees -= m;
            degrees *= 60;
            double s = System.Math.Round(degrees, decPlaces);

            // Create padding character
            char pad;
            char.TryParse("0", out pad);

            // Create d/m/s strings
            string dd = d.ToString();
            string mm = m.ToString().PadLeft(2, pad);
            string ss = s.ToString().PadRight(decPlaces + 3, pad);

            // Append d/m/s
            string dms = string.Format("{0}°{1}'{2}\"", dd, mm, ss);

            // Return formated string
            return dms;
        }

        public static string
        RADtoBearing(double rads)
        {
            double degrees = 0;
            string prefix;
            string suffix;

            if (rads >= 0 && rads <= pi / 2)
            {
                degrees = Ge.RadiansToDegrees(pi / 2 - rads);
                prefix = "N";
                suffix = "E";
            }
            else if (rads > pi / 2 && rads <= pi)
            {
                degrees = Ge.RadiansToDegrees(rads - pi / 2);
                prefix = "N";
                suffix = "W";
            }
            else if (rads > pi && rads <= 3 * pi / 2)
            {
                degrees = Ge.RadiansToDegrees(3 * pi / 2 - rads);
                prefix = "S";
                suffix = "W";
            }
            else
            {
                degrees = Ge.RadiansToDegrees(rads - 3 * pi / 2);
                prefix = "S";
                suffix = "E";
            }

            // Get d/m/s components
            double d = System.Math.Floor(degrees);
            degrees -= d;
            degrees *= 60;
            double m = System.Math.Floor(degrees);
            degrees -= m;
            degrees *= 60;
            double s = System.Math.Round(degrees);

            // Create padding character
            char pad;
            char.TryParse("0", out pad);

            // Create d/m/s strings
            string dd = d.ToString();
            string mm = m.ToString().PadLeft(2, pad);
            string ss = s.ToString().PadLeft(2, pad);

            // Append d/m/s
            string dms = string.Format("{0}°{1}'{2}\"", dd, mm, ss);

            dms = string.Format("{0}{1}{2}", prefix, dms, suffix);

            // Return formated string
            return dms;
        }

        /// <summary>
        /// convert Point3d to 2d version of 3d point
        /// </summary>
        /// <param name="pnt3d"></param>
        /// <returns>Point3d</returns>
        public static Point3d
        pnt3d_pnt2d(Point3d pnt3d)
        {
            Point3d pnt2d = new Point3d(pnt3d.X, pnt3d.Y, 0.0);

            return pnt2d;
        }

        public static double[]
        poly_dblPnts(Polyline poly)
        {
            List<double> pnts = new List<double>();
            int numVer = poly.NumberOfVertices;
            for (int i = 0; i < numVer; i++)
            {
                Point2d pnt2d = new Point2d();
                pnt2d = poly.GetPoint2dAt(i);
                pnts.Add(pnt2d.X);
                pnts.Add(pnt2d.Y);
            }
            return pnts.ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="poly3d"></param>
        /// <returns></returns>
        public static Point2dCollection
        poly_pnt2dColl(Polyline poly)
        {
            Point2dCollection pnts2d = new Point2dCollection();
            Point3d pnt3d;
            Point2d pnt2d;
            for (int i = 0; i < poly.NumberOfVertices; i++)
            {
                pnt3d = poly.GetPoint3dAt(i);
                pnt2d = new Point2d(pnt3d.X, pnt3d.Y);
                pnts2d.Add(pnt2d);
            }
            return pnts2d;
        }

        public static List<Vertex2d>
        poly_Vertex2dList(this ObjectId idPoly)
        {
            List<Vertex2d> v2ds = new List<Vertex2d>();
            Polyline poly = (Polyline)idPoly.getEnt();
            for (int i = 0; i < poly.NumberOfVertices; i++)
            {
                Point3d pnt3d = poly.GetPoint3dAt(i);
                double bulge = poly.GetBulgeAt(i);
                Vertex2d v2d = new Vertex2d(pnt3d, bulge, 0, 0, 0);
                v2ds.Add(v2d);
            }
            return v2ds;
        }

        public static Point3dCollection
        poly_pnt3dColl(this ObjectId idPoly)
        {
            Point3dCollection pnts3d = new Point3dCollection();
            Point3d pnt3d;
            using(var tr = BaseObjs.startTransactionDb()){
                Polyline poly = (Polyline)tr.GetObject(idPoly, OpenMode.ForRead);
                for (int i = 0; i < poly.NumberOfVertices; i++)
                {
                    pnt3d = poly.GetPoint3dAt(i);
                    pnts3d.Add(pnt3d);
                }
                tr.Commit();               
            }
            return pnts3d;
        }

        public static Point3dCollection
        poly_pnt3dColl(Polyline poly)
        {
            Point3dCollection pnts3d = new Point3dCollection();
            Point3d pnt3d;
            for (int i = 0; i < poly.NumberOfVertices; i++)
            {
                pnt3d = poly.GetPoint3dAt(i);
                pnts3d.Add(pnt3d);
            }
            return pnts3d;
        }

        public static List<Point3d>
        poly_pnt3dList(Polyline poly)
        {
            List<Point3d> pnts3d = new List<Point3d>();
            Point3d pnt3d;
            for (int i = 0; i < poly.NumberOfVertices; i++)
            {
                pnt3d = poly.GetPoint3dAt(i);
                pnts3d.Add(pnt3d);
            }
            return pnts3d;
        }

        public static List<ObjectId>
        poly_ArcsLines(this ObjectId idPoly, string nameLayer){
            List<ObjectId> ids = new List<ObjectId>();
            using(var tr = BaseObjs.startTransactionDb()){
                Polyline poly = (Polyline)tr.GetObject(idPoly, OpenMode.ForWrite);
                for (int i = 1; i < poly.NumberOfVertices; i++ ){
                    double bulge = poly.GetBulgeAt(i - 1);                    
                    if(bulge == 0){
                        ObjectId id = Draw.addLine(poly.GetPoint3dAt(i-1), poly.GetPoint3dAt(i), nameLayer);
                        id.changeProp(clr.red);
                        BaseObjs.updateGraphics();
                        ids.Add(id);
                    }else{
                        ObjectId id = Draw.addArc(poly.GetPoint3dAt(i - 1), poly.GetPoint3dAt(i), bulge, nameLayer);
                        id.changeProp(clr.red);
                        BaseObjs.updateGraphics();                        
                        ids.Add(id);
                    }
                }
                tr.Commit();
            }
            return ids;
        }

        public static double[]
        poly2d_dblPnts(Polyline2d poly2d)
        {
            List<double> pnts = new List<double>();
            foreach (Point2d pnt2d in poly2d)
            {
                pnts.Add(pnt2d.X);
                pnts.Add(pnt2d.Y);
            }

            return pnts.ToArray();
        }

        public static ObjectId
        poly2dToPoly(Polyline2d poly2d)
        {
            ObjectId idPoly = ObjectId.Null;
            List<Vertex2d> vtxs2d = new List<Vertex2d>();

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                foreach (ObjectId id in poly2d)
                {
                    Vertex2d vtx2d = (Vertex2d)tr.GetObject(id, OpenMode.ForRead);
                    vtxs2d.Add(vtx2d);
                }
                tr.Commit();
            }

            idPoly = Draw.addPoly(vtxs2d);
            poly2d.ObjectId.delete();
            return idPoly;
        }

        public static Point3dCollection
        poly2d_pnt3dColl(Polyline2d poly2d)
        {
            Point3dCollection pnts3d = new Point3dCollection();
            using (Polyline poly = new Polyline())
            {
                poly.ConvertFrom(poly2d, true);
                pnts3d = poly_pnt3dColl(poly);
            }

            return pnts3d;
        }

        public static Point3dCollection
        poly3d_pnt3dColl(Polyline3d poly3d)
        {
            Point3dCollection pnts3d = new Point3dCollection();
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    foreach (ObjectId objID in poly3d)
                    {
                        PolylineVertex3d v3d = (PolylineVertex3d)tr.GetObject(objID, OpenMode.ForRead);
                        Point3d pnt3d = new Point3d();
                        pnt3d = v3d.Position;

                        pnts3d.Add(pnt3d);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Conv.cs: line: 521");
            }
            return pnts3d;
        }

        public static Point3d[]
        poly3d_pnts3d(Polyline3d poly3d)
        {
            Point3dCollection pnts3dColl = poly3d_pnt3dColl(poly3d);
            Point3d[] pnts3d = new Point3d[pnts3dColl.Count];
            for (int i = 0; i < pnts3dColl.Count; i++)
            {
                pnts3d[i] = pnts3dColl[i];
            }

            return pnts3d;
        }

        public static ObjectId
        poly_Poly3d(ObjectId idPoly, double elev, string nameLayer)
        {
            ObjectId idPoly3d = ObjectId.Null;
            Point3dCollection pntsPoly = idPoly.getCoordinates3d();
            Point3dCollection pnts3d = new Point3dCollection();
            foreach (Point3d pnt3d in pntsPoly)
            {
                Point3d pnt3dNew = new Point3d(pnt3d.X, pnt3d.Y, elev);
                pnts3d.Add(pnt3dNew);
            }

            return Draw.addPoly3d(pnts3d, nameLayer);
        }

        public static ObjectId
        poly3d_Poly(ObjectId idPoly3d, string nameLayer)
        {
            ObjectId id = ObjectId.Null;
            try
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        Polyline3d poly3d = (Polyline3d)tr.GetObject(idPoly3d, OpenMode.ForRead);
                        Point3dCollection pnts3d = new Point3dCollection();
                        foreach (ObjectId vId in poly3d)
                        {
                            PolylineVertex3d v3d = (PolylineVertex3d)vId.GetObject(OpenMode.ForRead);
                            Point3d pnt3d = new Point3d();
                            pnt3d = v3d.Position;
                            try
                            {
                                pnts3d.Add(pnt3d);
                            }
                            catch (System.Exception ex)
                            {
                BaseObjs.writeDebug(ex.Message + " Conv.cs: line: 577");
                            }
                        }
                        tr.Commit();
                        id = Draw.addPoly(pnts3d, nameLayer);
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Conv.cs: line: 587");
            }
            return id;
        }

        public static List<Point3d>
        polyX_listPnts3d(ObjectId id)
        {
            List<Point3d> pnts3d = new List<Point3d>();
            pnts3d.TrimExcess();
            Point3d pnt3d;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForRead);

                    if (ent is Polyline3d)
                    {
                        Polyline3d poly3d = (Polyline3d)ent;
                        foreach (ObjectId objID in poly3d)
                        {
                            PolylineVertex3d v3d = (PolylineVertex3d)objID.GetObject(OpenMode.ForRead);
                            pnt3d = v3d.Position;
                            pnts3d.Add(pnt3d);
                        }
                    }
                    else if (ent is Polyline)
                    {
                        Polyline poly = (Polyline)ent;
                        for (int i = 0; i < poly.NumberOfVertices; i++)
                        {
                            pnt3d = poly.GetPoint3dAt(i);
                            pnts3d.Add(pnt3d);
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Conv.cs: line: 628");
            }

            return pnts3d;
        }

        public static ObjectId
        processBndry(ObjectId idPolyBndry)
        {
            ObjectId idPolyX = ObjectId.Null;

            if (idPolyBndry.IsNull)
            {
                idPolyBndry = Select.selectEntity(typeof(Polyline), "Select Boundary Polyline", "Boundary Polyline Selection Failed.", out pnt3dPicked).ObjectId;
            }

            idPolyBndry.checkIfClosed();

            if (idPolyBndry.isRightHand() == false)
            {
                idPolyBndry.reversePolyX();
            }

            List<Point3d> pnts3d = new List<Point3d>();

            List<Vertex2d> v2ds = idPolyBndry.poly_Vertex2dList();

            int intDivideDelta = 4;

            for (int i = 1; i < v2ds.Count; i++)
            {
                Vertex2d v2d = v2ds[i];

                double dblBulge = v2ds[i - 1].Bulge;

                if (dblBulge == 0)
                {
                    pnts3d.Add(v2ds[i - 1].Position);
                }
                else
                {
                    pnts3d.Add(v2d.Position);

                    Point3d pnt2dBeg = v2ds[i - 1].Position;
                    Point3d pnt2dEnd = v2ds[i - 0].Position;

                    double dblDelta = 4 * (System.Math.Atan(dblBulge));

                    double dblLenChord = pnt2dBeg.getDistance(pnt2dEnd);
                    double dblRadius = System.Math.Abs((dblLenChord / 2) / System.Math.Sin(dblDelta / 2));

                    double dblAzChord = pnt2dBeg.getDirection(pnt2dEnd);
                    double dblAzTan = dblAzChord - dblDelta / 2;

                    if (dblBulge <= 10.0)
                        intDivideDelta = 4;
                    else if (dblBulge <= 20.0)
                        intDivideDelta = 8;
                    else if (dblBulge <= 30.0)
                        intDivideDelta = 12;
                    else
                        intDivideDelta = 16;

                    double dblIncrDelta = dblDelta / intDivideDelta;

                    for (int z = 1; z < intDivideDelta; z++)
                    {
                        double dblIncrChordAz = dblAzTan + z * dblIncrDelta / 2;
                        double dblIncrChordLen = (2 * dblRadius * System.Math.Sin(System.Math.Abs(dblIncrDelta * z / 2)));

                        Point3d pnt3d = pnt2dBeg.traverse(dblIncrChordAz, dblIncrChordLen);
                        pnts3d.Add(pnt3d);
                    }
                }
            }

            pnts3d.Add(pnts3d[0]);  //end point same as beg point  -> poly was forced closed

            idPolyX = Draw.addPoly(pnts3d);

            Polyline objPoly = (Polyline)idPolyBndry.getEnt();
            idPolyX.changeProp(objPoly.LineWeight, objPoly.Color);

            idPolyBndry.delete();

            return idPolyX;
        }

    }// class Conv
}// namespace myAcadUtil
