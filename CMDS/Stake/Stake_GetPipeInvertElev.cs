using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

using Autodesk.Civil.DatabaseServices;

using Base_Tools45;
using Base_Tools45.C3D;

namespace Stake
{
    public static class Stake_GetPipeInvertElev
    {
        public static PIPE_DATA
        getPipeData(double dblStation, ObjectId idAlign)
        {
            //BEGIN MATCH UP NETWORK TO PIPE
            ObjectIdCollection idsNets = BaseObjs._civDoc.GetPipeNetworkIds();
            ObjectId idAlignRef = ObjectId.Null;
            string strRefAlignName = "";
            Network objNetWorkX = null;

            PIPE_DATA pipeData = new PIPE_DATA();
            for (int i = 0; i < idsNets.Count; i++)
            {
                Network objNetWork = (Network)idsNets[i].getEnt();

                try
                {
                    idAlignRef = objNetWork.ReferenceAlignmentId;

                    strRefAlignName = objNetWork.ReferenceAlignmentName;
                }
                catch (System.Exception)
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Error: Reference Alignment for Network" + objNetWork.Name + "not found - exiting");
                    return pipeData;
                }

                string nameAlign = Align.getAlignName(idAlign);
                if (strRefAlignName == nameAlign)
                {
                    objNetWorkX = objNetWork;
                    break;
                }
            }
            //END MATCH UP NETWORK TO PIPE

            if (objNetWorkX == null)
            {
                pipeData.Invert = -1;
                pipeData.Size = -1;
                return pipeData;
            }

            ObjectIdCollection objPipes = objNetWorkX.GetPipeIds();
            ObjectId idStructStart = ObjectId.Null, idStructEnd = ObjectId.Null;

            Structure objStructStart = null, objStructEnd = null;
            double dblStationX = 0, dblOffset = 0, dblStaStructStart = 0, dblStaStructEnd = 0;
            for (int i = 0; i < objPipes.Count; i++)
            {
                Pipe objPipe = (Pipe)objPipes[i].getEnt();

                try
                {
                    idStructStart = objPipe.StartStructureId;
                    objStructStart = (Structure)idStructStart.getEnt();
                    Point3d pnt3d = objStructStart.Location;
                    idAlign.getAlignStaOffset(pnt3d, ref dblStationX, ref dblOffset);
                    dblStaStructStart = dblStationX;
                }
                catch (System.Exception )
                {
                }

                try
                {
                    idStructEnd = objPipe.EndStructureId;
                    objStructEnd = (Structure)idStructEnd.getEnt();
                    Point3d pnt3d = objStructEnd.Location;
                    idAlign.getAlignStaOffset(pnt3d, ref dblStationX, ref dblOffset);
                    dblStaStructEnd = dblStationX;
                }
                catch (System.Exception )
                {
                }
                double dblStaPipeStart = 0;
                if (System.Math.Round(dblStation, 2) > System.Math.Round(dblStaStructStart, 2))
                {
                    if (System.Math.Round(dblStation, 2) < System.Math.Round(dblStaStructEnd, 2))
                    {
                        idAlign.getAlignStaOffset(objPipe.StartPoint, ref dblStaPipeStart, ref dblOffset);
                        double dblElevStart = objPipe.StartPoint.Z;
                        double dblElevEnd = objPipe.EndPoint.Z;
                        double dblSlope = (dblElevEnd - dblElevStart) / objPipe.Length2DCenterToCenter;
                        double dblLenX = dblStation - dblStaPipeStart;
                        double dblSize = objPipe.InnerDiameterOrWidth;
                        double dblVertComp = dblSize / System.Math.Cos(System.Math.Atan(dblSlope));
                        dblSize = System.Math.Round(dblSize * 12, 0);
                        double dblInvElev = (dblElevStart - 0.5 * dblVertComp) + dblSlope * dblLenX;

                        pipeData.Invert = System.Math.Round(dblInvElev, 3);
                        pipeData.Size = dblSize;

                        break;
                    }
                }
            }

            return pipeData;
        }
    }
}