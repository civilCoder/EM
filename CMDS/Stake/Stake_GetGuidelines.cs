using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using System.Collections.Generic;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Stake
{
    public static class Stake_GetGuidelines
    {
        public static ObjectId
        getGuidelines(Entity obj0)
        {
            List<Vertex2d> vtx2dList = new List<Vertex2d>();
            List<Point3d> dblPnts = new List<Point3d>();
            ObjectId idEnt = ObjectId.Null;
            string nameLayer = obj0.Layer;

            switch (obj0.GetType().Name)
            {
                case "Arc":

                    Arc objArc = (Arc)obj0;

                    dblPnts.Add(objArc.StartPoint);
                    dblPnts.Add(objArc.EndPoint);

                    double dblDelta = objArc.TotalAngle;
                    int intRightHand = (objArc.Normal.Z > 0) ? 1 : -1;
                    double dblBulge = System.Math.Tan(dblDelta / 4) * intRightHand;

                    vtx2dList = new List<Vertex2d> { new Vertex2d(objArc.StartPoint, dblBulge, 0, 0, 0), new Vertex2d(objArc.EndPoint, 0, 0, 0, 0) };
                    idEnt = Draw.addPoly(vtx2dList, nameLayer);
                    objArc.ObjectId.delete();

                    break;

                case "Line":

                    Line objLine = (Line)obj0;

                    vtx2dList = new List<Vertex2d> { new Vertex2d(objLine.StartPoint, 0, 0, 0, 0), new Vertex2d(objLine.EndPoint, 0, 0, 0, 0) };
                    idEnt = Draw.addPoly(vtx2dList, nameLayer);

                    objLine.ObjectId.delete();

                    break;

                case "Polyline":

                    idEnt = obj0.ObjectId;

                    break;

                case "Polyline2d":

                    Polyline2d obj2dPoly = (Polyline2d)obj0;
                    idEnt = Conv.poly2dToPoly(obj2dPoly);

                    break;

                case "Polyline3d":

                    Polyline3d objPoly3d = (Polyline3d)obj0;
                    idEnt = Conv.poly3d_Poly(objPoly3d.ObjectId, nameLayer);
                    break;

                default:
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(obj0.GetType().ToString() + " type are not supported.");

                    break;
            }

            return idEnt;
        }
    }
}