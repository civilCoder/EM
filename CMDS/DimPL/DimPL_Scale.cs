using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using System;
using System.Collections.Generic;

namespace DimPL
{
    public class DimPL_Scale
    {
        private const double pi = System.Math.PI;

        public static void
        addPolyLdrs(Point3d pnt3d1a, Point3d pnt3d2a, double dir, int side, int row,
            ObjectId idTX, double txtSize, int annoScale, double sizeRow, List<Handle> handles, Object xRefPath, double station)
        {
            double txtHeight = txtSize * annoScale;
            double delta = 0;

            int maxRow = 1;

            if (txtSize == 0.075)
                maxRow = 15;
            else
                maxRow = 4;

            double R = txtHeight * (2.0 - row / maxRow);

            if (row < 5)
                delta = System.Math.Atan((double)row / (double)maxRow);     //maxrow
            else
                delta = System.Math.Atan(1 + (double)row / (double)maxRow);

            double L = (sizeRow * txtHeight * row) / System.Math.Sin(delta);
            double T = R * System.Math.Tan(delta / 2.0);
            double L1 = L - T;
            double C = 2 * R * System.Math.Sin(delta / 2.0);
            double Bulge = System.Math.Tan(delta / 4.0);
            txtHeight = System.Math.Round(txtHeight, 3);

            double dirX = dir - delta * side;
            Point3d pnt3d1b = pnt3d1a.traverse(dirX, txtHeight);
            Point3d pnt3d1c = pnt3d1a.traverse(dirX, L1);  //need bulge attached here
            Point3d pnt3d1d = pnt3d1c.traverse(dirX + delta / 2 * side, C);
            Point3d pnt3d1e = pnt3d1d.traverse(dir, row * 0.5 * txtHeight);

            dirX = dir + (pi + delta) * side;
            Point3d pnt3d2b = pnt3d2a.traverse(dirX, txtHeight);
            Point3d pnt3d2c = pnt3d2a.traverse(dirX, L1);  //need bulge attached here
            Point3d pnt3d2d = pnt3d2c.traverse(dirX - delta / 2 * side, C);
            Point3d pnt3d2e = pnt3d2d.traverse(dir - pi, row * 0.5 * txtHeight);

            //draw leaders

            List<Vertex2d> vertex2Ds = new List<Vertex2d>();
            vertex2Ds.Add(new Vertex2d(pnt3d1a, 0.0, 0.0, txtHeight / 6, 0.0));
            vertex2Ds.Add(new Vertex2d(pnt3d1b, 0.0, 0.0, 0.0, 0.0));
            vertex2Ds.Add(new Vertex2d(pnt3d1c, Bulge * side, 0.0, 0.0, 0.0));
            vertex2Ds.Add(new Vertex2d(pnt3d1d, 0.0, 0.0, 0.0, 0.0));
            vertex2Ds.Add(new Vertex2d(pnt3d1e, 0.0, 0.0, 0.0, 0.0));

            ObjectId idPolyLdr1 = vertex2Ds.addPoly("DIMENSION");
            //ObjectId idPolyLdr1 = Blocks.addBlockRefPolyLdr(vertex2Ds, "DIMENSION");

            vertex2Ds = new List<Vertex2d>();
            vertex2Ds.Add(new Vertex2d(pnt3d2a, 0.0, 0.0, txtHeight / 6, 0.0));
            vertex2Ds.Add(new Vertex2d(pnt3d2b, 0.0, 0.0, 0.0, 0.0));
            vertex2Ds.Add(new Vertex2d(pnt3d2c, Bulge * side * -1, 0.0, 0.0, 0.0));
            vertex2Ds.Add(new Vertex2d(pnt3d2d, 0.0, 0.0, 0.0, 0.0));
            vertex2Ds.Add(new Vertex2d(pnt3d2e, 0.0, 0.0, 0.0, 0.0));

            ObjectId idPolyLdr2 = vertex2Ds.addPoly("DIMENSION");
            //ObjectId idPolyLdr2 = Blocks.addBlockRefPolyLdr(vertex2Ds, "DIMENSION");

            TypedValue[] tvTX = new TypedValue[18];
            tvTX.SetValue(new TypedValue(1001, apps.lnkDimPL), 0);
            tvTX.SetValue(new TypedValue(1000, "TX"), 1);
            tvTX.SetValue(new TypedValue(1070, row), 2);
            tvTX.SetValue(new TypedValue(1070, side), 3);
            tvTX.SetValue(new TypedValue(1070, annoScale), 4);
            tvTX.SetValue(new TypedValue(1040, txtSize), 5);
            tvTX.SetValue(new TypedValue(1040, sizeRow), 6);
            tvTX.SetValue(new TypedValue(1040, pnt3d1a.X), 7);
            tvTX.SetValue(new TypedValue(1040, pnt3d1a.Y), 8);
            tvTX.SetValue(new TypedValue(1040, pnt3d2a.X), 9);
            tvTX.SetValue(new TypedValue(1040, pnt3d2a.Y), 10);
            tvTX.SetValue(new TypedValue(1005, idPolyLdr1.getHandle()), 11);
            tvTX.SetValue(new TypedValue(1005, idPolyLdr2.getHandle()), 12);
            tvTX.SetValue(new TypedValue(1005, handles[0]), 13);
            tvTX.SetValue(new TypedValue(1000, xRefPath.ToString()), 14);
            tvTX.SetValue(new TypedValue(1040, station), 15);
            tvTX.SetValue(new TypedValue(1005, handles[1]), 16);
            tvTX.SetValue(new TypedValue(1005, handles[2]), 17);

            idTX.setXData(tvTX, apps.lnkDimPL);
        }

        public static void
        scaleDimPL(int scaleCurr)
        {
            //"lnkDimPL" dictionary stores list of DimPL_App text handles
            bool exists = false;
            ObjectId idDictDimPL = Dict.getNamedDictionary(apps.lnkDimPL, out exists);
            List<DBDictionaryEntry> entries = idDictDimPL.getDictEntries();
            if (entries.Count == 0)
                return;
            try
            {
                foreach (DBDictionaryEntry entry in entries)
                {
                    string h = entry.Key;
                    ObjectId id = h.stringToHandle().getObjectId();
                    if (!id.IsValid)
                        return;

                    ResultBuffer rb = id.getXData(apps.lnkDimPL);
                    TypedValue[] tvsTX = rb.AsArray();

                    ObjectId idTX = h.stringToHandle().getObjectId();

                    int row, side, scalePrior;
                    double txtSize, sizeRow;

                    int.TryParse(tvsTX[2].Value.ToString(), out row);
                    int.TryParse(tvsTX[3].Value.ToString(), out side);
                    int.TryParse(tvsTX[4].Value.ToString(), out scalePrior);
                    double.TryParse(tvsTX[5].Value.ToString(), out txtSize);
                    double.TryParse(tvsTX[6].Value.ToString(), out sizeRow);
                    Point3d pnt3d1a = new Point3d((double)tvsTX[7].Value, (double)tvsTX[8].Value, 0.0);
                    Point3d pnt3d2a = new Point3d((double)tvsTX[9].Value, (double)tvsTX[10].Value, 0.0);
                    Handle hPolyLdr1 = tvsTX[11].Value.ToString().stringToHandle();
                    Handle hPolyLdr2 = tvsTX[12].Value.ToString().stringToHandle();
                    Handle hEntX = tvsTX[13].Value.ToString().stringToHandle();
                    string xRefPath = tvsTX[14].Value.ToString();
                    double station = (double)tvsTX[15].Value;
                    Handle hLine1 = tvsTX[16].Value.ToString().stringToHandle();
                    Handle hLine2 = tvsTX[17].Value.ToString().stringToHandle();

                    hPolyLdr1.getObjectId().delete();
                    hPolyLdr2.getObjectId().delete();

                    double dir = pnt3d1a.getDirection(pnt3d2a);

                    Point3d pnt3dIns = idTX.getMTextLocation();

                    double txtHeight = System.Math.Round(txtSize * scaleCurr, 3);
                    double offset = (sizeRow * txtHeight * row);
                    double distT = pnt3d1a.getDistance(pnt3d2a);

                    double distH = pnt3d1a.getDistance(pnt3dIns);
                    double alpha = Geom.getAngle3Points(pnt3d1a, pnt3d2a, pnt3dIns);
                    double distX = distH * System.Math.Cos(alpha);

                    Point3d pnt3d3 = pnt3d1a.traverse(dir, distX);
                    Point3d pnt3d4 = pnt3d3.traverse(dir - pi / 2 * side, offset);

                    idTX.moveObj(pnt3dIns, pnt3d4);

                    List<Handle> handles = new List<Handle>();
                    handles.Add(hEntX);
                    handles.Add(hLine1);
                    handles.Add(hLine2);

                    addPolyLdrs(pnt3d1a, pnt3d2a, dir, side, row, idTX, txtSize, scaleCurr, sizeRow, handles, xRefPath, station);
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} DimPL_Scale.cs: line: 163", ex.Message));
            }
        }
    }
}