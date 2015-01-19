using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using System.Collections.Generic;

namespace EW
{
    public static class EW_CheckAreaLimits
    {
        static double pi = System.Math.PI;

        public static void
        checkAreaLimits(SelectionSet objSSetAreas)
        {
            SelectionSet objSSet = EW_Utility1.buildSSetGradingLim();
            if (objSSet.Count == 0)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("GRADING LIMIT missing - exiting..........");
                return;
            }
            ObjectId idGradingLim = objSSet.GetObjectIds()[0];

            objSSet = EW_Utility1.buildSSetBLDG_LIM();
            if (objSSet.Count == 0)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("BUILDING LIMIT missing - exiting.......");
                return;
            }

            if (checkAreaLimit(objSSetAreas, idGradingLim))
            {
                return;
            }

            List<Point3d> varPnts3dBndry0z = getGradingLimitCoords();

            limitIsOutside(objSSetAreas, varPnts3dBndry0z);

        }


        public static void
        testAreaOffset(SelectionSet objSSet_XX_OX)
        {
            Color color = new Color();
            color = Color.FromColorIndex(ColorMethod.ByBlock, 200);
            ObjectId[] ids = objSSet_XX_OX.GetObjectIds();
            for (int i = 0; i < ids.Length; i++)
            {
                Polyline objLWPlineT = (Polyline)ids[i].getEnt();

                ObjectId idLWPlineX = objLWPlineT.offset(-0.1);

                if (idLWPlineX == ObjectId.Null)
                {
                    ids[i].changeProp(LineWeight.LineWeight200, color);
                }
                else
                {
                    idLWPlineX.delete();
                }
            }
        }

        public static bool
        checkAreaLimit(SelectionSet objSSetAreas, ObjectId idGradingLim)
        {
            Polyline objGradingLim = (Polyline)idGradingLim.getEnt();

            bool boolPass = true;
            Color color = null;
            double dblAreaSum = 0;
            int k = 0;
            if (objSSetAreas == null)
                return false;
            ObjectId[] ids = objSSetAreas.GetObjectIds();

            for (int i = 0; i < ids.Length; i++)
            {

                Polyline objLWPoly = (Polyline)ids[i].getEnt();


                if (objLWPoly.Area == 0)
                {
                    ids[i].delete();


                }
                else
                {
                    EW_Utility1.removeDuplicateVertex(ref objLWPoly);

                    dblAreaSum = dblAreaSum + objLWPoly.Area;

                    List<Point3d> pnts3d = objLWPoly.ObjectId.getCoordinates3dList();

                    for (int j = 1; j < pnts3d.Count - 1; j++)
                    {
                        Point3d pnt3dBack   = pnts3d[j - 1];
                        Point3d pnt3dX      = pnts3d[j - 0];
                        Point3d pnt3dAhed   = pnts3d[j + 1];

                        double dblAngBack = pnt3dX.getDirection(pnt3dBack);
                        double dblAngAhead = pnt3dX.getDirection(pnt3dAhed);


                        if (System.Math.Abs(System.Math.Round(dblAngAhead - dblAngBack, 3)) == System.Math.Round(pi, 3))
                        {

                            ObjectId idCircle = Draw.addCircle(pnt3dX, 2);
                            k ++;
                            color = new Color();
                            color = Color.FromColorIndex(ColorMethod.ByBlock, (short)k);
                            string mess = string.Format("Unnecessary vertex at:  X={0}, Y={1}", pnts3d[j].X, pnts3d[j].Y);
                            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(mess);

                            idCircle.changeProp(LineWeight.LineWeight100, color);
                            boolPass = false;

                        }
                    }
                }
            }

            double dblAreaLim = objGradingLim.Area;
            string msg = "";

            if (dblAreaLim - dblAreaSum > dblAreaLim * 0.05)
            {
                msg = string.Format("Grading Limit area: {0} is greater than the sum of the areas: {1}", dblAreaLim, dblAreaSum);
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(msg);
                boolPass = false;
            }

            if (dblAreaSum - dblAreaLim > dblAreaLim * 0.05)
            {

                msg = string.Format("The sum of the areas: {0} is greater than the Grading Limit area: {1}", dblAreaSum, dblAreaLim);
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(msg);
                boolPass = false;
            }

            if (boolPass)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Area Limits Passed Inspection.");
                return false;
            }
            else
            {
                return true;
            }

        }

        public static bool
        limitIsOutside(SelectionSet objSSetAreas, List<Point3d> pnts3dBndry0z)
        {
            ObjectId[] ids = objSSetAreas.GetObjectIds();
            List<Point3d> pnts3d = new List<Point3d>();

            Color color = new Color();
            color = Color.FromColorIndex(ColorMethod.ByBlock, 2);

            for (int i = 0; i <ids.Length; i++)
            {
                pnts3d = ids[i].getCoordinates3dList();

                foreach (Point3d pnt3d in pnts3d)
                {
                    if (!pnt3d.isInside(pnts3dBndry0z))
                    {
                        if (Geom.isOn2dSegment(pnt3d, pnts3dBndry0z) == -1)
                        {
                            ObjectId idCircle = Draw.addCircle(pnt3d, 2);
                            idCircle.changeProp(color, "DEBUG-0", LineWeight.LineWeight035);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static List<Point3d>
        getGradingLimitCoords()
        {
            SelectionSet objSSet = EW_Utility1.buildSSetGradingLim();
            if (objSSet.Count == 0)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("GRADING LIMIT missing - exiting..........");
                return null;
            }
            ObjectId[] ids = objSSet.GetObjectIds();
            List<Point3d> pnts3dBndry0z = ids[0].getCoordinates3dList();
            for (int i = 0; i < pnts3dBndry0z.Count; i++)
            {
                pnts3dBndry0z[i] = new Point3d(pnts3dBndry0z[i].X, pnts3dBndry0z[i].Y, 0.0);
            }

            return pnts3dBndry0z;
        }
    }
}
