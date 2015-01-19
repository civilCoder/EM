using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using System.Collections.Generic;

namespace Stake
{
    public static class Stake_AddPntIntersects
    {
        private static Point3d pnt3dOrg = Base_Tools45.Pub.pnt3dO;

        public static void
        addPnt3dIntersects(Polyline poly, List<Point3d> pnts3dInt)
        {
            List<int> SegNo = null;
            int OnSeg = 0;

            List<double> bulges = new List<double>();
            List<Point3d> pnts3d = poly.ObjectId.getCoordinates3dList();

            for (int i = 0; i < pnts3d.Count; i++)
            {
                double bulge = poly.GetBulgeAt(i);
                bulges.Add(bulge);
            }

            for (int i = 0; i <= pnts3dInt.Count; i++)
            {
                Point3d pnt3d = pnts3dInt[i];

                OnSeg = Geom.isOn2dSegment(pnt3d, pnts3d);

                Point2d pnt2d = Point2d.Origin;

                if (OnSeg != -1)
                {
                    pnt2d = new Point2d(pnt3d.X, pnt3d.Y);

                    //    Set objCircle = clsdwg.ThisDrawing.ModelSpace.AddCircle(dblPnt, 2)

                    poly.AddVertexAt(OnSeg + 1, pnt2d, 0, 0, 0);       //*************** inserted vertex need value for bulge if segment was a curve ****************

                    pnts3d = poly.ObjectId.getCoordinates3dList();

                    SegNo.Add(OnSeg);
                }
            }
        }
    }
}