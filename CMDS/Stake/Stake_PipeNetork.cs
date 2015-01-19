using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Stake
{
    public static class
    Stake_PipeNetwork
    {
        public static void
        addPointsFromPipeNetwork()
        {
            Point3d pnt3dPick = Pub.pnt3dO;
            Entity ent = Select.selectEntity(typeof(Alignment), "Select SEWER Alignment", "", out pnt3dPick);
            if (ent == null)
                return;

            Alignment align = (Alignment)ent;

            ObjectIdCollection idsNetWork = BaseObjs._civDoc.GetPipeNetworkIds();
            Network networkX = null;

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                for (int i = 0; i < idsNetWork.Count; i++)
                {
                    Network nw = (Network)tr.GetObject(idsNetWork[i], OpenMode.ForRead);
                    if (nw.ReferenceAlignmentName == align.Name)
                    {
                        networkX = nw;
                        break;
                    }
                }
                tr.Commit();
            }

            uint pntNum = 0;

            ObjectIdCollection idsPipe = networkX.GetPipeIds();

            using (Transaction tr1 = BaseObjs.startTransactionDb())
            {
                for (int i = 0; i <= idsPipe.Count; i++)
                {
                    Pipe pipe = (Pipe)tr1.GetObject(idsPipe[i], OpenMode.ForRead);

                    double dblElevStart = pipe.StartPoint.Z;
                    double dblElevEnd = pipe.EndPoint.Z;
                    double dblSlope = (dblElevEnd - dblElevStart) / pipe.Length2DCenterToCenter;
                    double dblSize = pipe.InnerDiameterOrWidth;
                    double dblVertComp = dblSize / System.Math.Cos(System.Math.Atan(dblSlope));
                    dblSize = System.Math.Round(dblSize * 12, 0);

                    Point3d pnt3dBeg = pipe.StartPoint;
                    Point3d pnt3dEnd = pipe.EndPoint;

                    pnt3dBeg = pnt3dBeg.addElevation(dblElevStart - 0.5 * dblVertComp);
                    ObjectId idPnt = pnt3dBeg.setPoint(out pntNum);

                    pnt3dEnd = pnt3dEnd.addElevation(dblElevEnd - 0.5 * dblVertComp);
                    idPnt = pnt3dEnd.setPoint(out pntNum);

                    //Add Callout

                    //Connect callout to point and point
                }
                tr1.Commit();
            }
        }
    }
}