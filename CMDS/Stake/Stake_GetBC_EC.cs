using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using Math = Base_Tools45.Math;

namespace Stake
{
    public static class Stake_GetBC_EC
    {
        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;

        public static bool
        getBC_EC(ObjectId idAlign, ref List<POI> varpoi)
        {
            bool boolExists = false;
            Profile objProfile = null;
            Alignment objAlign = (Alignment)idAlign.getEnt();

            try
            {
                objProfile = Prof.getProfile(idAlign, "FLOWLINE");
            }
            catch (System.Exception)
            {
                objProfile = Prof.getProfile(idAlign, "STAKE");
            }

            string strClass = fStake.ClassObj;

            AlignmentEntityCollection objAlignEnts = objAlign.Entities;
            bool isRightHand = false;

            if (objAlignEnts.Count > 1)
            {
                for (int i = 0; i < objAlignEnts.Count; i++)
                {
                    AlignmentEntity objAlignEnt = objAlignEnts[i];

                    if (objAlignEnt.EntityType == AlignmentEntityType.Arc)
                    {
                        boolExists = false;

                        AlignmentArc objAlignEntArc = (AlignmentArc)objAlignEnt;

                        isRightHand = !(objAlignEntArc.Clockwise);
                        double dblStationBC = System.Math.Round(objAlignEntArc.StartStation, 3);

                        int x = 0;
                        for (int j = 0; j < varpoi.Count; j++)
                        {
                            if (System.Math.Abs(varpoi[j].Station - dblStationBC) < 0.05)
                            {
                                boolExists = true;
                                x = j;
                                break;
                            }
                        }

                        double dblBulge = 0;
                        double dblAngDirCen = objAlignEntArc.StartPoint.getDirection(objAlignEntArc.CenterPoint);//direction to center of arc
                        double dblAngDelta = objAlignEntArc.Delta;
                        double dblRadius = System.Math.Round(objAlignEntArc.Radius, 2);
                        Point2d dblPntCen = objAlignEntArc.CenterPoint;

                        if (boolExists)
                        {
                            dblBulge = System.Math.Tan(objAlignEntArc.Delta / 4);

                            if (objAlignEntArc.Clockwise)
                            {
                                dblBulge = dblBulge * -1.0;
                            }

                            POI vpoi = varpoi[x];

                            vpoi.Station = Math.roundDown3(dblStationBC);
                            if (vpoi.Desc0 == "EC")
                            {
                                vpoi.Desc0 = "PCC";
                            }
                            else
                            {
                                vpoi.Desc0 = "BC";
                            }
                            vpoi.AngDir = dblAngDirCen;
                            vpoi.isRightHand = isRightHand;
                            vpoi.Bulge = dblBulge;
                            vpoi.Radius = dblRadius;
                            vpoi.CenterPnt = new Point3d(dblPntCen.X, dblPntCen.Y, 0);

                            varpoi[x] = vpoi;
                        }
                        else
                        {
                            string strDesc = "BC";
                            double dblElev = 0;

                            try
                            {
                                dblElev = objProfile.ElevationAt(dblStationBC);
                            }
                            catch (System.Exception)
                            {
                                try
                                {
                                    dblElev = objProfile.ElevationAt(dblStationBC + 0.05);
                                }
                                catch (System.Exception)
                                {
                                    idAlign.delete();
                                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(string.Format("Design Point not found at beginning of Arc @ {0:###+00.00} - revise and retry - exiting", dblStationBC));
                                    return false;
                                }
                            }

                            if (dblElev != 0)
                            {
                                dblBulge = System.Math.Tan(objAlignEntArc.Delta / 4);
                                if (objAlignEntArc.Clockwise)
                                {
                                    dblBulge = dblBulge * -1.0;
                                }
                                dblRadius = System.Math.Round(objAlignEntArc.Radius, 2);

                                POI vpoi = new POI();
                                vpoi.Station = dblStationBC;
                                vpoi.Desc0 = strDesc;
                                vpoi.ClassObj = strClass;
                                vpoi.AngDelta = dblAngDelta;
                                vpoi.AngDir = dblAngDirCen;
                                vpoi.isRightHand = isRightHand;
                                vpoi.Elevation = dblElev;
                                vpoi.Bulge = dblBulge;
                                vpoi.Radius = dblRadius;
                                vpoi.CenterPnt = new Point3d(dblPntCen.X, dblPntCen.Y, 0);

                                varpoi.Add(vpoi);
                                varpoi = varpoi.sortPOIbyStation();
                            }
                        }

                        boolExists = false;

                        double dblStationEC = System.Math.Round(objAlignEntArc.EndStation, 3);

                        x = 0;
                        for (int j = 0; j < varpoi.Count; j++)
                        {
                            if (System.Math.Abs(varpoi[j].Station - dblStationEC) < 0.05)
                            {
                                boolExists = true;
                                x = j;
                                break;
                            }
                        }

                        dblAngDirCen = objAlignEntArc.EndPoint.getDirection(objAlignEntArc.CenterPoint);

                        if (boolExists)
                        {
                            POI vpoi = varpoi[x];
                            vpoi.Station = Math.roundDown3(dblStationEC);
                            vpoi.Desc0 = "EC";
                            vpoi.AngDir = dblAngDirCen;
                            vpoi.isRightHand = isRightHand;
                            vpoi.Bulge = dblBulge;
                            vpoi.Radius = dblRadius;
                            varpoi[x] = vpoi;
                        }
                        else
                        {
                            string strDesc = "EC";
                            double dblElev = 0;

                            try
                            {
                                dblElev = objProfile.ElevationAt(dblStationEC);
                            }
                            catch (System.Exception)
                            {
                                dblElev = objProfile.ElevationAt(dblStationBC - 0.05);
                            }

                            POI vpoi = new POI();
                            vpoi.Station = dblStationEC;
                            vpoi.Desc0 = strDesc;
                            vpoi.ClassObj = strClass;
                            vpoi.AngDelta = dblAngDelta;
                            vpoi.AngDir = dblAngDirCen;
                            vpoi.isRightHand = isRightHand;
                            vpoi.Elevation = dblElev;
                            vpoi.Bulge = dblBulge;
                            vpoi.Radius = dblRadius;

                            varpoi.Add(vpoi);

                            varpoi = varpoi.sortPOIbyStation();
                        }
                    }
                }
            }

            return true;
        }
    }
}