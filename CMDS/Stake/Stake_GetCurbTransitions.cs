using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.Linq;
using Math = Base_Tools45.Math;

namespace Stake
{
    public static class Stake_GetCurbTransitions
    {
        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;

        public static void
        getCurbTransitions()
        {
            Alignment objAlign = fStake.ACTIVEALIGN;
            ObjectId idAlign = objAlign.ObjectId;

            if (idAlign == ObjectId.Null)
            {
                Point3d pnt3dPicked = Pub.pnt3dO;
                string nameAlign = "";
                idAlign = Align.selectAlign("\nSelect Curb Alignment: ", "\nAlignment selection failed.", out pnt3dPicked, out nameAlign);
                objAlign = (Alignment)idAlign.getEnt();
            }

            ObjectId idTable = Stake_Table.getTableId(idAlign);

            List<POI> varpoi = fStake.POI_CALC;
            string strResponse = "";
            bool escape = UserInput.getUserInput("\nEnter Curb Height in INCHES: ", out strResponse);
            double dblHeightCurb = Double.Parse(strResponse);
            Point3d varPntPick = Pub.pnt3dO;
            PromptStatus ps;
            double dblStation = 0, dblOffset = 0;
            bool boolNewPVI = false;

            do
            {
                try
                {
                    varPntPick = UserInput.getPoint("\nSelect Location to Adjust: ", out ps, osMode: 8);
                }
                catch (System.Exception)
                {
                    break;
                }

                objAlign.StationOffset(varPntPick.X, varPntPick.Y, ref dblStation, ref dblOffset);
                getPVI(idAlign, Math.roundDown3(dblStation), ref varpoi, dblHeightCurb, ref boolNewPVI);
            }
            while (true);

            fStake.POI_CALC = varpoi;

            if (boolNewPVI)
            {
                Stake_Table.updateTableData(idTable, varpoi);
            }
        }

        public static void
        getPVI(ObjectId idAlign, double dblStation, ref List<POI> varpoi, double dblHeightCurb, ref bool boolNewPVI)
        {
            bool boolFound = false;

            Profile objProfileFLOWLINE = Prof.getProfile(idAlign, "FLOWLINE");
            Profile objProfileSTAKE = Prof.getProfile(idAlign, "STAKE");
            ProfilePVICollection objProfilePVIs = objProfileSTAKE.PVIs;

            double dblProfileElev = System.Math.Round(objProfileFLOWLINE.ElevationAt(dblStation) + dblHeightCurb / 12, 3);

            for (int i = 0; i <= objProfilePVIs.Count - 1; i++)
            {
                ProfilePVI objProfilePVI = objProfilePVIs[i];

                if (Math.roundDown1((objProfilePVI.Station)) == Math.roundDown1(dblStation))
                {
                    objProfilePVI.Elevation = dblProfileElev;
                    boolFound = true;

                    break;
                }
            }

            if (boolFound)
            {
                for (int k = 0; k < varpoi.Count; k++)
                {
                    if (varpoi[k].Station == Math.roundDown3(dblStation))
                    {
                        check4GradeBreak(k, varpoi);
                    }
                }

                boolFound = false;
            }
            else
            {
                boolNewPVI = true;
                ProfilePVI objProfilePVI = objProfilePVIs.AddPVI(dblStation, dblProfileElev);

                varpoi.Add(new POI { Station = Math.roundDown3(dblStation), Elevation = dblProfileElev, ClassObj = fStake.ClassObj });

                var sortSta = from p in varpoi
                              orderby p.Station
                              select p;
                int j = 0;
                foreach (var p in sortSta)
                {
                    j += 1;
                    if (Math.roundDown3(dblStation) == p.Station)
                    {
                        check4GradeBreak(j, varpoi);
                        break;
                    }
                }
            }
        }

        public static void
        check4GradeBreak(int i, List<POI> varpoi)
        {
            double dblLenBack = 0;
            double dblLenAhead = 0;

            POI vpoi = varpoi[i];

            if (i > 0 && i < varpoi.Count)
            {
                dblLenBack = (varpoi[i - 0].Station - varpoi[i - 1].Station);
                dblLenAhead = (varpoi[i + 1].Station - varpoi[i + 0].Station);

                if (dblLenBack != 0)
                {
                    vpoi.SlopeBack = System.Math.Round((varpoi[i - 1].Elevation - varpoi[i - 0].Elevation) / dblLenBack, 5);
                }

                if (dblLenAhead != 0)
                {
                    vpoi.SlopeAhead = System.Math.Round((varpoi[i + 1].Elevation - varpoi[i + 0].Elevation) / dblLenAhead, 5);
                }

                if (System.Math.Round(varpoi[i].SlopeBack + varpoi[i].SlopeAhead, 2) != 0)
                {
                    vpoi.Desc0 = varpoi[i].Desc0 + " GB";
                    vpoi.DescX = varpoi[i].DescX + " GB";
                }
            }
            else if (i == 0)
            {
                dblLenAhead = (varpoi[i + 1].Station - varpoi[i + 0].Station);

                if (dblLenAhead != 0)
                {
                    vpoi.SlopeAhead = System.Math.Round((varpoi[i + 1].Elevation - varpoi[i + 0].Elevation) / dblLenAhead, 5);
                }

                if (System.Math.Round(varpoi[i + 1].SlopeAhead - varpoi[i + 0].SlopeAhead, 2) != 0)
                {
                    POI vpoiA = varpoi[i + 1];
                    vpoiA.Desc0 = varpoi[i + 1].Desc0 + " GB";
                    vpoiA.DescX = varpoi[i + 1].DescX + " GB";
                    varpoi[i + 1] = vpoiA;
                }
            }
            else if (i == varpoi.Count - 1)
            {
                dblLenBack = (varpoi[i - 0].Station - varpoi[i - 1].Station);

                if (dblLenBack != 0)
                {
                    vpoi.SlopeBack = System.Math.Round((varpoi[i - 1].Elevation - varpoi[i - 0].Elevation) / dblLenBack, 5);
                }

                if (System.Math.Round(varpoi[i + 0].SlopeBack - varpoi[i - 1].SlopeBack, 2) != 0)
                {
                    POI vpoiB = varpoi[i - 1];
                    vpoiB.Desc0 = varpoi[i - 1].Desc0 + " GB";
                    vpoiB.DescX = varpoi[i - 1].DescX + " GB";
                    varpoi[i - 1] = vpoiB;
                }
            }
            varpoi[i] = vpoi;
        }
    }
}