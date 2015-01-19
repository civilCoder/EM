using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using System.Collections.Generic;

namespace EW
{
    public static class EW_GradingLim
    {


        public static void
        test_isOutsideGradingLimit()
        {          
            Point3d varPntPick = Pub.pnt3dO;

            Polyline objBldgOX = (Polyline)Select.selectEntity(typeof(Polyline), "Select BldgOX", "", out varPntPick);
            Polyline objGradingLim = (Polyline)Select.selectEntity(typeof(Polyline), "Select Grading Limit", "",out varPntPick);

            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(isOutsideGradingLimit(objBldgOX.ObjectId, objGradingLim.ObjectId).ToString());
        }

        public static bool
        isOutsideGradingLimit(this ObjectId idBldgOX1, ObjectId idGradingLim)
        {
            bool boolOut = false;

            List<Point3d> varPntsGradingLim = idGradingLim.getCoordinates3dList();
            List<Point3d> varPntsGradingLim3d0z = new List<Point3d>();
            foreach(Point3d pnt3d in varPntsGradingLim)
                varPntsGradingLim3d0z.Add(new Point3d(pnt3d.X, pnt3d.Y, 0.0));
            
            List<Point3d> varPnts = idBldgOX1.getCoordinates3dList();

            for (int i = 0; i < varPnts.Count; i++)
            {
                Point3d pnt3d = varPnts[i];
                bool boolIsInside = pnt3d.isInside(varPntsGradingLim3d0z);
                if (!boolIsInside)
                {
                    boolOut = true;
                }
            }
            return boolOut;
        }
    }
}
