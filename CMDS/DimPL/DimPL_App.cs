using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using System;
using System.Collections.Generic;

namespace DimPL
{
    public static class DimPL_App
    {
        private const double pi = System.Math.PI;

        public static void
        addOrMoveTX(Point3d pnt3dX, Point3d pnt3d1a, Point3d pnt3d2a, double txtSize, int annoScale,
            double sizeRow, double txtHeight, int maxRows, double dir, double distT, List<Handle> handles,
            MText mTxt = null, Object xRefPath = null, double station = 0, int row = 0, int side = -1)
        {
            Handle hEntX = handles[0];

            if (pnt3dX == Pub.pnt3dO)
            {
                pnt3dX = pnt3d1a.traverse(dir, distT * station);
                pnt3dX = pnt3dX.traverse(dir + pi / 2 * side, row * txtHeight);
            }
            else if (pnt3dX.isRightSide(pnt3d1a, pnt3d2a))
            {
                side = 1;
            }

            ObjectId idTX = ObjectId.Null;   //text
            double distH = pnt3d1a.getDistance(pnt3dX);
            double alpha = Geom.getAngle3Points(pnt3d1a, pnt3d2a, pnt3dX);
            double distX = distH * System.Math.Cos(alpha);

            double distY = System.Math.Round(distH * System.Math.Sin(alpha), 3);

            double spaceRow = sizeRow * txtHeight;
            double buffer = spaceRow * 0.499;

            if (distY < spaceRow - buffer)
            {
                row = 1;
            }
            else
            {
                for (int i = 1; i < maxRows + 1; i++)
                {
                    if (distY > (spaceRow * i) - buffer && distY < (spaceRow * i) + buffer)
                    {
                        row = i;
                        break;
                    }
                }
            }
            if (row == 0)
                row = maxRows;

            double offset = (sizeRow * txtHeight * row);
            Point3d pnt3d3 = pnt3d1a.traverse(dir, distX);
            Point3d pnt3d4 = pnt3d3.traverse(dir - pi / 2 * side, offset);
            station = distX / distT;

            if (mTxt == null)
            {
                double dirTxt = dir;
                if (dir > pi / 2 && dir < 3 * pi / 2)
                    dirTxt = dir + pi;

                string txt = dir.radianToDegreeString();
                txt = string.Format("{0}  {1}\'", txt, distT.ToString("F2"));
                Color color = new Color();
                color = Color.FromColorIndex(ColorMethod.ByLayer, 256);
                using (BaseObjs._acadDoc.LockDocument())
                {
                    idTX = Txt.addMText(txt, pnt3d4, dirTxt, 0.0, txtSize, AttachmentPoint.MiddleCenter, "Annotative", "TEXT", color, Pub.JUSTIFYCENTER);
                }
                string h = idTX.getHandle().ToString();

                bool exists = false;
                ObjectId idDictDimPL = Dict.getNamedDictionary(apps.lnkDimPL, out exists);

                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        DBDictionary dictDimPL = (DBDictionary)tr.GetObject(idDictDimPL, OpenMode.ForWrite);
                        DBDictionary dictH = new DBDictionary();
                        dictDimPL.SetAt(h, dictH);
                        tr.AddNewlyCreatedDBObject(dictH, true);
                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(string.Format("{0} DimPL_App.cs: line: 80", ex.Message));
                }
            }
            else
            {
                idTX = mTxt.ObjectId;
                idTX.moveObj(mTxt.Location, pnt3d4);
            }

            DimPL_Scale.addPolyLdrs(pnt3d1a, pnt3d2a, dir, side, row, idTX, txtSize, annoScale, sizeRow, handles, xRefPath, station);
            //idTX.activateObj();
        }

        public static void
        dimpl(double txtSize, int maxRows, double sizeRow)
        {
            PromptStatus ps;
            Point3d pnt3d1a, pnt3d2a, pntPick;
            List<Point3d> pnts3dEntX, pnts3dEnt1, pnts3dEnt2, pnts3d1a, pnts3d2a;
            Object xRefPath1, xRefPath2, xRefPathX = null;
            Line lineX, line1, line2;

            Entity ent1, ent2;
            List<Handle> handles = new List<Handle>();
            int annoScale = Misc.getCurrAnnoScale();
            double txtHeight = System.Math.Round(txtSize * annoScale, 3);

            Object osModeOrg = SnapMode.getOSnap();
            SnapMode.setOSnap(1);

            bool escape = false;
            bool isClosed;
            FullSubentityPath path;
            List<FullSubentityPath> paths = new List<FullSubentityPath>();

            Entity entX = xRef.getNestedEntity("\nSelect Property Line: ", out escape, out xRefPathX, out ps, out pnts3dEntX, out path, out pntPick, out isClosed);
            if (escape || ps == PromptStatus.None)
                return;
            paths.Add(path);

            lineX = null;
            if (entX is Line)
            {
                lineX = (Line)entX;
            }
            else if (entX is Polyline)
            {
            }
            entHandles.hX = lineX.Handle;

            ent1 = xRef.getNestedEntity("\nSelect SubDivision Line: ", out escape, out xRefPath1, out ps, out pnts3dEnt1, out path, out pntPick, out isClosed);  //check if entX and line1 are in same drawing.
            if (escape || ps == PromptStatus.None)
                return;
            paths.Add(path);

            line1 = null;
            if (ent1 is Line)
            {
                line1 = (Line)ent1;
            }
            else if (ent1 is Polyline)
            {
            }

            pnts3d1a = pnts3dEnt1.intersectWith(pnts3dEntX, true, extend.both);
            if (pnts3d1a.Count > 1)
                return;
            pnt3d1a = pnts3d1a[0];
            if (pnt3d1a.X == -1)
            {
                pnts3d1a = pnts3dEntX.intersectWith(pnts3dEnt1, true, extend.both);
                pnt3d1a = pnts3d1a[0];
            }

            entHandles.h1 = line1.Handle;

            escape = false;

            ent2 = xRef.getNestedEntity("\nSelect SubDivision Line: ", out escape, out xRefPath2, out ps, out pnts3dEnt2, out path, out pntPick, out isClosed);  //To do: check if entX and line1 are in same drawing.
            if (escape || ps == PromptStatus.None)
                return;

            paths.Add(path);

            line2 = null;
            if (ent2 is Line)
            {
                line2 = (Line)ent2;
            }
            else if (ent2 is Polyline)
            {
            }

            pnts3d2a = pnts3dEnt2.intersectWith(pnts3dEntX, false, extend.both);
            if (pnts3d2a.Count > 1)
                return;
            pnt3d2a = pnts3d2a[0];
            if (pnt3d2a.X == -1)
            {
                pnts3d2a = pnts3dEntX.intersectWith(pnts3dEnt2, true, extend.both);
                pnt3d2a = pnts3d2a[0];
            }

            entHandles.h2 = line2.Handle;

            handles.Add(entHandles.hX);
            handles.Add(entHandles.h1);
            handles.Add(entHandles.h2);

            SnapMode.setOSnap(0);
            getTextLocation(pnt3d1a, pnt3d2a, txtSize, annoScale, sizeRow, txtHeight, maxRows, handles, xRefPathX);
            int osM;
            int.TryParse(osModeOrg.ToString(), out osM);
            SnapMode.setOSnap(osM);

            xRef.unHighlightNestedEntity(paths);
        }

        private static void
        getTextLocation(Point3d pnt3d1a, Point3d pnt3d2a, double txtSize, int annoScale, double sizeRow, double txtHeight, int maxRows, List<Handle> handles, Object xRefPath)
        {
            bool escape = false;
            PromptStatus ps;

            Point3d pnt3dX = UserInput.getPoint("\nSelect Text Location: ", Pub.pnt3dO, out escape, out ps, osMode: 0);
            if (escape || pnt3dX == Pub.pnt3dO)
                return;

            double distT = pnt3d1a.getDistance(pnt3d2a);
            double dir = pnt3d1a.getDirection(pnt3d2a);

            addOrMoveTX(pnt3dX, pnt3d1a, pnt3d2a, txtSize, annoScale, sizeRow, txtHeight, maxRows, dir, distT, handles, null, xRefPath);
        }

        public struct entHandles
        {
            public static Handle h1;
            public static Handle h2;
            public static Handle hX;
        }
    }
}