using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using System;
using System.Collections.Generic;
using Math = System.Math;

namespace DimPL
{
    public static class DimPL_Update
    {
        public static void
        updateDimPL(ObjectId idMTxt, string nameFile)
        {
            TypedValue[] tvsTX = idMTxt.getXData(apps.lnkDimPL).AsArray();

            string xRefPath = tvsTX[14].Value.ToString();
            if (xRefPath != nameFile)
                return;

            int row, side, annoScale;
            double txtSize, sizeRow;

            int.TryParse(tvsTX[2].Value.ToString(), out row);
            int.TryParse(tvsTX[3].Value.ToString(), out side);
            int.TryParse(tvsTX[4].Value.ToString(), out annoScale);
            Double.TryParse(tvsTX[5].Value.ToString(), out txtSize);
            Double.TryParse(tvsTX[6].Value.ToString(), out sizeRow);

            Point3d pnt3d1a = new Point3d((double)tvsTX[7].Value, (double)tvsTX[8].Value, 0.0);     //endpoint of segment
            Point3d pnt3d2a = new Point3d((double)tvsTX[9].Value, (double)tvsTX[10].Value, 0.0);    //endpoint of segment

            Handle hPolyLdr1 = tvsTX[11].Value.ToString().stringToHandle();
            Handle hPolyLdr2 = tvsTX[12].Value.ToString().stringToHandle();

            Handle hEntX = tvsTX[13].Value.ToString().stringToHandle();                             //parent entity

            double station = (double)tvsTX[15].Value;                                               //location along parent entity from pnt3d1a

            Handle hLine1 = tvsTX[16].Value.ToString().stringToHandle();
            Handle hLine2 = tvsTX[17].Value.ToString().stringToHandle();

            List<Handle> handles = new List<Handle>();
            handles.Add(hEntX);
            handles.Add(hLine1);
            handles.Add(hLine2);

            List<Point3d> pnts3dX = xRef.getNestedEntityCoordinates2(hEntX, xRefPath);
            if (pnts3dX == null || pnts3dX.Count == 0)
            {
                cleanUp(idMTxt, hPolyLdr1, hPolyLdr2);
                return;
            }

            List<Point3d> pnts3dL1 = null;
            if (hLine1 != null)
            {
                pnts3dL1 = xRef.getNestedEntityCoordinates2(hLine1, xRefPath);
                if (pnts3dL1 == null || pnts3dL1.Count == 0)
                {
                    cleanUp(idMTxt, hPolyLdr1, hPolyLdr2);
                    return;
                }
            }

            List<Point3d> pnts3dL2 = null;
            if (hLine2 != null)
            {
                pnts3dL2 = xRef.getNestedEntityCoordinates2(hLine2, xRefPath);
                if (pnts3dL2 == null)
                {
                    cleanUp(idMTxt, hPolyLdr1, hPolyLdr2);
                    return;
                }
            }

            List<Point3d> pnts3d1a = pnts3dL1.intersectWith(pnts3dX, true, extend.both);
            if (pnts3d1a == null)
            {
                cleanUp(idMTxt, hPolyLdr1, hPolyLdr2);
                return;
            }

            Point3d pnt3d1aX = pnts3d1a[0];
            if (pnt3d1aX.X == -1)
            {
                pnts3d1a = pnts3dX.intersectWith(pnts3d1a, true, extend.source);
                pnt3d1aX = pnts3d1a[0];
                if (pnt3d1aX.X == -1)
                {
                    cleanUp(idMTxt, hPolyLdr1, hPolyLdr2);
                    return;
                }
            }

            List<Point3d> pnts3d2a = pnts3dL2.intersectWith(pnts3dX, true, extend.both);
            if (pnts3d2a == null)
            {
                //idMTxt.activateObj();
                idMTxt.delete();
                return;
            }

            Point3d pnt3d2aX = pnts3d2a[0];
            if (pnt3d2aX.X == -1)
            {
                pnts3d2a = pnts3dX.intersectWith(pnts3d2a, true, extend.source);
                pnt3d2aX = pnts3d2a[0];
                if (pnt3d2aX.X == -1)
                {
                    cleanUp(idMTxt, hPolyLdr1, hPolyLdr2);
                    return;
                }
            }

            if (pnt3d1a == pnt3d1aX && pnt3d2a == pnt3d2aX)
                return;

            double dir = pnt3d1aX.getDirection(pnt3d2aX);
            double distT = pnt3d1aX.getDistance(pnt3d2aX);

            int maxRows = 0;
            if (txtSize == 0.075)
                maxRows = DimPL_Global.maxRows075;
            else
                maxRows = DimPL_Global.maxRows090;

            double txtHeight = Math.Round(txtSize * annoScale, 3);

            Point3d pnt3dX = Pub.pnt3dO;

            using (BaseObjs._acadDoc.LockDocument())
            {
                cleanUp(idMTxt, hPolyLdr1, hPolyLdr2);
                DimPL_App.addOrMoveTX(pnt3dX, pnt3d1aX, pnt3d2aX, txtSize, annoScale, sizeRow, txtHeight, maxRows, dir, distT, handles, null, xRefPath, station, row, side);
            }
            DimPL_Global.countUpdates++;
        }

        private static void
        cleanUp(ObjectId idMTxt, Handle hPolyLdr1, Handle hPolyLdr2)
        {
            string h = idMTxt.getHandle().ToString();
            bool exists;
            ObjectId idDict = Dict.getNamedDictionary(apps.lnkDimPL, out exists);
            Dict.removeSubEntry(idDict, h);

            hPolyLdr1.getObjectId().delete();
            hPolyLdr2.getObjectId().delete();
            idMTxt.delete();
        }
    }
}