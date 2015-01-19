using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using Math = Base_Tools45.Math;

namespace Stake
{
    public static class Stake_GetAnglePoints
    {
        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;

        public static void
        getAnglePoints(ObjectId idAlign, ref List<POI> varpoi)
        {
            POI vPoi = new POI();
            bool boolClosed = false;

            string strClass = fStake.ClassObj;
            Profile objProfile = null;
            switch (strClass)
            {
                case "CURB":
                    objProfile = Prof.getProfile(idAlign, "FLOWLINE");
                    break;

                case "FL":
                    objProfile = Prof.getProfile(idAlign, "STAKE");
                    break;

                case "WTR":
                    objProfile = Prof.getProfile(idAlign, "CPNT");
                    break;

                case "BLDG":
                    vPoi = new POI();
                    break;
            }

            Alignment objAlign = (Alignment)idAlign.getEnt();
            AlignmentEntityCollection objAlignEnts = objAlign.Entities;
            double dblElev = 0;
            bool isRightHand = false;
            AlignmentEntity objAlignEnt0 = null;
            AlignmentEntity objAlignEntX = null;
            AlignmentLine objAlignTan0 = null;
            AlignmentLine objAlignTanX = null;
            AlignmentArc objAlignArc0 = null;
            AlignmentArc objAlignArcX = null;

            if (objAlignEnts.Count > 1)
            {
                for (int i = 0; i < objAlignEnts.Count - 1; i++)
                {
                    objAlignEnt0 = objAlignEnts[i + 0];
                    objAlignEntX = objAlignEnts[i + 1];

                    if (objAlignEnt0.EntityType == AlignmentEntityType.Line)
                    {
                        objAlignTan0 = (AlignmentLine)objAlignEnt0;

                        if (objAlignEntX.EntityType == AlignmentEntityType.Line)
                        {
                            objAlignTanX = (AlignmentLine)objAlignEntX;

                            double dblAng0 = objAlignTan0.StartPoint.getDirection(objAlignTan0.EndPoint);
                            double dblAngX = objAlignTanX.StartPoint.getDirection(objAlignTanX.EndPoint);

                            if (System.Math.Round(dblAng0, 3) != System.Math.Round(dblAngX, 3))
                            {
                                Point3d pnt3d1 = new Point3d(objAlignTan0.StartPoint.X, objAlignTan0.StartPoint.Y, 0);
                                Point3d pnt3d2 = new Point3d(objAlignTan0.EndPoint.X, objAlignTan0.EndPoint.Y, 0);
                                Point3d pnt3d3 = new Point3d(objAlignTanX.EndPoint.X, objAlignTanX.EndPoint.Y, 0);

                                double dblStation = System.Math.Round(objAlignTanX.StartStation, 3);
                                //          Debug.Print i

                                double dblAngDelta = Geom.getAngle3Points(pnt3d1, pnt3d2, pnt3d3);
                                double dblAngDir = dblAngX;

                                Vector3d v3d1 = pnt3d2 - pnt3d1;
                                Vector3d v3d2 = pnt3d3 - pnt3d2;

                                Vector3d v3dC = v3d1.CrossProduct(v3d2);

                                if (v3dC.Z > 0)
                                {
                                    isRightHand = true;
                                }
                                else
                                {
                                    isRightHand = false;
                                }

                                switch (strClass)
                                {
                                    case "CURB":
                                    case "FL":
                                    case "WTR":

                                        try
                                        {
                                            dblElev = System.Math.Round(objProfile.ElevationAt(dblStation), 3);
                                        }
                                        catch (System.Exception)
                                        {
                                        }

                                        break;
                                }

                                POI vpoi = new POI();

                                vpoi.Station = dblStation;
                                vpoi.Desc0 = "AP";
                                vpoi.ClassObj = strClass;
                                vpoi.AngDelta = dblAngDelta;
                                vpoi.AngDir = dblAngDir;
                                vpoi.isRightHand = isRightHand;
                                vpoi.Elevation = dblElev;

                                varpoi.Add(vpoi);
                                varpoi = varpoi.sortPOIbyStation();
                            }
                        }
                    }
                }
            }

            objAlignEnt0 = objAlignEnts[objAlignEnts.FirstEntity - 1];
            objAlignEntX = objAlignEnts[objAlignEnts.LastEntity - 1];

            bool boolArc0 = false;
            bool boolArcX = false;
            bool boolTan0 = false;
            bool boolTanX = false;

            Point2d pnt2dBeg = Point2d.Origin, pnt2dEnd = Point2d.Origin;
            if (objAlignEnt0.EntityType == AlignmentEntityType.Arc)
            {
                objAlignArc0 = (AlignmentArc)objAlignEnt0;
                pnt2dBeg = objAlignArc0.StartPoint;
                boolArc0 = true;
            }
            else
            {
                objAlignTan0 = (AlignmentLine)objAlignEnt0;
                pnt2dBeg = objAlignTan0.StartPoint;
                boolTan0 = true;
            }

            if (objAlignEntX.EntityType == AlignmentEntityType.Arc)
            {
                objAlignArcX = (AlignmentArc)objAlignEntX;
                pnt2dEnd = objAlignArcX.StartPoint;
                boolArcX = true;
            }
            else
            {
                objAlignTanX = (AlignmentLine)objAlignEntX;
                pnt2dEnd = objAlignTanX.StartPoint;
                boolTanX = true;
            }

            if (System.Math.Round(pnt2dEnd.X, 2) == System.Math.Round(pnt2dBeg.X, 2))
            {
                if (System.Math.Round(pnt2dEnd.Y, 2) == System.Math.Round(pnt2dBeg.Y, 2))
                {
                    boolClosed = true;
                    varpoi[0].isClosed = true;

                    if (boolArc0 & boolArcX)
                    {
                        varpoi[0].Desc0 = "PCC";
                    }
                    else if (boolArc0 & boolTanX)
                    {
                        //do nothing
                    }
                    else if (boolTan0 & boolArcX)
                    {
                        varpoi[0].Desc0 = "EC";
                    }
                    else if (boolTan0 & boolTanX)
                    {
                        Point3d pnt3d1 = new Point3d(objAlignTanX.StartPoint.X, objAlignTanX.StartPoint.Y, 0);
                        Point3d pnt3d2 = new Point3d(objAlignTanX.EndPoint.X, objAlignTanX.EndPoint.Y, 0);
                        Point3d pnt3d3 = new Point3d(objAlignTan0.EndPoint.X, objAlignTan0.EndPoint.Y, 0);

                        double dblAngDelta = Geom.getAngle3Points(pnt3d1, pnt3d2, pnt3d3);

                        double dblAng0 = objAlignTan0.StartPoint.getDirection(objAlignTan0.EndPoint);
                        double dblAngDir = dblAng0;

                        Vector3d v3d1 = pnt3d2 - pnt3d1;
                        Vector3d v3d2 = pnt3d3 - pnt3d2;

                        Vector3d v3dC = v3d1.CrossProduct(v3d2);

                        if (v3dC.Z > 0)
                        {
                            isRightHand = true;
                        }
                        else
                        {
                            isRightHand = false;
                        }

                        if (strClass == "BLDG")
                        {
                            varpoi[0].Station = objAlignTan0.StartStation;
                            varpoi[0].Desc0 = "AP";
                            varpoi[0].DescX = "BEG AP";
                            varpoi[0].ClassObj = strClass;
                            varpoi[0].AngDelta = dblAngDelta;
                            varpoi[0].AngDir = dblAngDir;
                        }
                        else
                        {
                            varpoi[0].Desc0 = "AP";
                            varpoi[0].DescX = "BEG AP";
                            varpoi[0].AngDelta = dblAngDelta;
                            varpoi[0].AngDir = dblAngDir;
                        }
                    }
                }
            }

            if (boolClosed)
            {
                int j = varpoi.Count - 1;

                if (varpoi[j].Station != Math.roundDown3((objAlign.EndingStation)))
                {
                    POI vpoi = new POI();
                    vpoi.Station = Math.roundDown3((objAlign.EndingStation));
                    vpoi.Elevation = varpoi[0].Elevation;
                    if (strClass == "BLDG")
                    {
                        vpoi.DescX = varpoi[0].DescX;
                    }
                    else
                    {
                        vpoi.DescX = "END AP";
                    }
                    vpoi.Desc0 = "AP";
                    vpoi.ClassObj = strClass;
                    vpoi.isClosed = true;

                    varpoi.Add(vpoi);
                }
                else
                {
                    POI vpoi = varpoi[j];

                    vpoi.Elevation = varpoi[0].Elevation;
                    vpoi.Desc0 = "AP";
                    if (strClass == "BLDG")
                    {
                        vpoi.DescX = varpoi[0].DescX;
                    }
                    vpoi.DescX = "END AP";
                    vpoi.ClassObj = strClass;
                    vpoi.isClosed = true;
                    varpoi[j] = vpoi;
                }
            }
        }

        public static void
        addEndElev(ObjectId idAlign, ref List<POI> varpoi, string strBaseProfileName)
        {
            Profile objProfile = default(Profile);
            ProfilePVI objProfilePVI = default(ProfilePVI);
            int j = varpoi.Count - 1;

            switch (strBaseProfileName)
            {
                case "FLOWLINE":

                    objProfile = Prof.getProfile(idAlign, "STAKE");

                    try
                    {
                        objProfilePVI = objProfile.PVIs.AddPVI(varpoi[j].Station, varpoi[j].Elevation + double.Parse(fStake.cboHeight.Text) / 12);
                    }
                    catch (System.Exception)
                    {
                    }

                    break;

                case "CPNT":

                    objProfile = Prof.getProfile(idAlign, "CPNT");
                    objProfilePVI = objProfile.PVIs.AddPVI(varpoi[j].Station, varpoi[j].Elevation);

                    break;

                default:

                    objProfilePVI = null;
                    objProfile = Prof.getProfile(idAlign, strBaseProfileName);

                    try
                    {
                        objProfilePVI = objProfile.PVIs.AddPVI(varpoi[j].Station, varpoi[j].Elevation);
                    }
                    catch (System.Exception)
                    {
                    }

                    break;
            }
        }
    }
}