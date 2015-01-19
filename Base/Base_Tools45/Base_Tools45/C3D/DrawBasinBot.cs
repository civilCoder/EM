using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Base_Tools45.C3D
{
    public static class DrawBasinBot
    {
        public static ObjectId
        build3dPolyBasinBot(this ObjectId idPolyLIM, Point3d pnt3dCEN, Point3d pnt3dTAR, double dblSlope, string nameLayer, string nameApp = "basinBot")
        {
            ObjectId idPoly3d = ObjectId.Null;

            Point3d pnt3dX = Pub.pnt3dO;
            Point3dCollection pnts3d = new Point3dCollection();
            Polyline polyLIM = (Polyline)idPolyLIM.getEnt();
            int k = polyLIM.NumberOfVertices;

            try
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    for (int i = 0; i < k; i++)
                    {
                        pnt3dX = polyLIM.GetPoint3dAt(i);
                        pnt3dX = new Point3d(pnt3dX.X, pnt3dX.Y, pnt3dTAR.Z + Geom.getCosineComponent(pnt3dCEN, pnt3dTAR, pnt3dX) * (dblSlope * -1));
                        pnts3d.Add(pnt3dX);
                    }
                    Layer.manageLayers(nameLayer);
                    idPoly3d = Draw.addPoly3d(pnts3d, nameLayer, 1);
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Draw3d.cs: line: 64", ex.Message));
            }
            return idPoly3d;
        }
    }
}
