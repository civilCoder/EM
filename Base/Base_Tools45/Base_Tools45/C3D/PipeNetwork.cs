using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;

namespace Base_Tools45.C3D
{
    public static class PipeNetwork
    {
        public static void
        updatePipes(double dZ, double inv, ObjectId idCgPNt)
        {
            ResultBuffer rb = idCgPNt.getXData(apps.lnkMNP);
            if (rb == null)
                return;
            TypedValue[] tvs = rb.AsArray();
            Point3d pnt3dBeg, pnt3dEnd, pnt3dX;
            double diam = 0, slope = 0, invX = 0;

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                if (tvs[1].Value.ToString() != "0")
                {
                    ObjectId idPipe = tvs[1].Value.ToString().stringToHandle().getObjectId();
                    Pipe pipe0 = (Pipe)tr.GetObject(idPipe, OpenMode.ForWrite);

                    diam = pipe0.InnerDiameterOrWidth;

                    pnt3dBeg = pipe0.StartPoint;
                    pnt3dEnd = pipe0.EndPoint;

                    pnt3dX = new Point3d(pnt3dEnd.X, pnt3dEnd.Y, pnt3dEnd.Z + dZ);
                    pipe0.EndPoint = pnt3dX;

                    slope = (pnt3dBeg.Z - pnt3dX.Z) / pipe0.Length2DCenterToCenter;

                    invX = pnt3dX.Z - diam / (2 * System.Math.Cos(System.Math.Atan(System.Math.Abs(slope))));

                    if (System.Math.Abs(inv - invX) > .01)
                    {
                        Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Calc'd invert based on revised springline of pipe is greater than 0.01' different.");
                    }
                }

                if (tvs[2].Value.ToString() != "0")
                {
                    ObjectId idPipe = tvs[2].Value.ToString().stringToHandle().getObjectId();
                    Pipe pipe0 = (Pipe)tr.GetObject(idPipe, OpenMode.ForWrite);

                    diam = pipe0.InnerDiameterOrWidth;

                    pnt3dBeg = pipe0.StartPoint;
                    pnt3dEnd = pipe0.EndPoint;

                    pnt3dX = new Point3d(pnt3dBeg.X, pnt3dBeg.Y, pnt3dBeg.Z + dZ);
                    pipe0.StartPoint = pnt3dX;

                    slope = (pnt3dX.Z - pnt3dEnd.Z) / pipe0.Length2DCenterToCenter;

                    invX = pnt3dX.Z - diam / (2 * System.Math.Cos(System.Math.Atan(System.Math.Abs(slope))));

                    if (System.Math.Abs(inv - invX) > .01)
                    {
                        Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Calc'd invert based on revised springline of pipe is greater than 0.01' different.");
                    }
                }
                tr.Commit();
            }
        }
    }
}