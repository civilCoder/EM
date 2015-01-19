using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Linq;

namespace Stake
{
    public static class Stake_Wall
    {
        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;

        public static void
        processWall(ObjectId idAlign)
        {
            List<POI> varpoi = new List<POI>();

            Stake_GetCardinals.getCardinals_Horizontal(idAlign, ref varpoi);
            Stake_GetCardinals.checkBegAndEndStations(idAlign, ref varpoi);
            Stake_DuplicateStations.resolveDuplicateStations(ref varpoi);

            WALL_DATA wall_Data = getWallData(idAlign);

            object varPntUpper = wall_Data.PntUpper;

            List<WALL_STEP> varPOI_TOW;
            List<WALL_STEP> varPOI_TOF;

            convertWallData(wall_Data, out varPOI_TOW, out varPOI_TOF);

            List<POI> varPOI_WALL = getWallPOI(varPOI_TOW, varPOI_TOF);

            fStake.POI_STAKE = varPOI_WALL;

            ObjectId idTable = Stake_Table.makeTable(idAlign, varPOI_WALL, varPntUpper: varPntUpper);
            Stake_Table.addTableData(idTable, varPOI_WALL);
        }

        public static List<POI>
        getWallPOI(List<WALL_STEP> colTOW_STEP, List<WALL_STEP> colTOF_STEP)
        {
            List<POI> varPOI_WALL = null;

            for (int i = 0; i <= colTOW_STEP.Count; i++)
            {
                double dblTOWelev = colTOW_STEP[i].ELEV;
                double dblTOWbeg = colTOW_STEP[i].BEGSTA;
                double dblTOWend = colTOW_STEP[i].ENDSTA;

                POI vPOI_WALL = new POI();

                vPOI_WALL.ElevTOW = dblTOWelev;
                vPOI_WALL.ElevTOF = getElevAtWallSta(dblTOWbeg, colTOF_STEP);
                vPOI_WALL.Station = dblTOWbeg;
                vPOI_WALL.Desc0 = "TOW";
                vPOI_WALL.ClassObj = "WALL";

                vPOI_WALL.ElevTOW = dblTOWelev;
                vPOI_WALL.ElevTOF = getElevAtWallSta(dblTOWend, colTOF_STEP);
                vPOI_WALL.Station = dblTOWend;
                vPOI_WALL.Desc0 = "TOW";
                vPOI_WALL.ClassObj = "WALL";
                varPOI_WALL.Add(vPOI_WALL);
            }

            for (int i = 0; i < colTOF_STEP.Count; i++)
            {
                double dblTOFelev = colTOF_STEP[i].ELEV;
                double dblTOFbeg = colTOF_STEP[i].BEGSTA;
                double dblTOFend = colTOF_STEP[i].ENDSTA;

                POI vPOI_WALL = new POI();

                vPOI_WALL.ElevTOF = dblTOFelev;
                vPOI_WALL.ElevTOW = getElevAtWallSta(dblTOFbeg, colTOW_STEP);
                vPOI_WALL.Station = dblTOFbeg;
                vPOI_WALL.Desc0 = "TOF";
                vPOI_WALL.ClassObj = "WALL";

                vPOI_WALL.ElevTOF = dblTOFelev;
                vPOI_WALL.ElevTOW = getElevAtWallSta(dblTOFend, colTOW_STEP);
                vPOI_WALL.Station = dblTOFend;
                vPOI_WALL.Desc0 = "TOF";
                vPOI_WALL.ClassObj = "WALL";

                varPOI_WALL.Add(vPOI_WALL);
            }

            var sortSta = from p in varPOI_WALL
                          orderby p.Station ascending
                          select p;

            List<POI> poiTmp = new List<POI>();
            foreach (var p in sortSta)
                poiTmp.Add(p);

            return poiTmp;
        }

        public static double
        getElevAtWallSta(double dblSta, List<WALL_STEP> colWALL_STEP)
        {
            for (int j = 0; j < colWALL_STEP.Count; j++)
            {
                double dblElev = colWALL_STEP[j].ELEV;
                double dblBeg = colWALL_STEP[j].BEGSTA;
                double dblEnd = colWALL_STEP[j].ENDSTA;

                if (dblBeg <= dblSta & dblSta <= dblEnd)
                {
                    return dblElev;
                }
            }
            return -1;
        }

        public static void
        convertWallData(WALL_DATA varWall_Data, out List<WALL_STEP> POI_TOW_Step, out List<WALL_STEP> POI_TOF_Step)
        {
            double dblBegX = varWall_Data.PntLower.X;
            double dblBegY = varWall_Data.PntLower.Y;

            double dblStaStart = varWall_Data.StaStart;
            double dblElevStart = varWall_Data.ElevStart;
            double dblScaleV = varWall_Data.ScaleV;
            List<Point3d> varTOWcoords = varWall_Data.TOWcoords;
            List<Point3d> varTOFcoords = varWall_Data.TOFcoords;

            POI vPOI_TOW = new POI();
            POI vPOI_TOF = new POI();

            List<POI> POI_TOW = new List<POI>();
            List<POI> POI_TOF = new List<POI>();

            vPOI_TOW.ClassObj = "WALL";
            vPOI_TOW.Station = System.Math.Round(varTOWcoords[0].X - dblBegX + dblStaStart, 2);
            vPOI_TOW.ElevTOW = System.Math.Round(dblElevStart + (varTOWcoords[0].Y - dblBegY) / dblScaleV, 2);
            vPOI_TOW.Desc0 = "TOW";
            POI_TOW.Add(vPOI_TOW);

            for (int i = 1; i < varTOWcoords.Count; i++)
            {
                vPOI_TOW = new POI();
                vPOI_TOW.ClassObj = "WALL";
                vPOI_TOW.Station = System.Math.Round(varTOWcoords[i].X - dblBegX + dblStaStart, 2);
                vPOI_TOW.ElevTOW = System.Math.Round(dblElevStart + (varTOWcoords[i + 1].Y - dblBegY) / dblScaleV, 2);
                vPOI_TOW.Desc0 = "TOW";
                if (vPOI_TOW.Station == POI_TOW[i - 1].Station)
                {
                    vPOI_TOW.isStep = true;
                }
                POI_TOW.Add(vPOI_TOW);
            }

            vPOI_TOF.ClassObj = "WALL";
            vPOI_TOF.Station = System.Math.Round(varTOFcoords[0].X - dblBegX + dblStaStart, 2);
            vPOI_TOF.ElevTOF = System.Math.Round(dblElevStart + (varTOFcoords[0].Y - dblBegY) / dblScaleV, 2);
            vPOI_TOF.Desc0 = "TOF";
            POI_TOF.Add(vPOI_TOF);

            for (int i = 1; i < varTOFcoords.Count; i++)
            {
                vPOI_TOF = new POI();
                vPOI_TOF.ClassObj = "WALL";
                vPOI_TOF.Station = System.Math.Round(varTOFcoords[i].X - dblBegX + dblStaStart, 2);
                vPOI_TOF.ElevTOF = System.Math.Round(dblElevStart + (varTOFcoords[i + 1].Y - dblBegY) / dblScaleV, 2);
                vPOI_TOF.Desc0 = "TOF";
                if (vPOI_TOF.Station == POI_TOF[i - 1].Station)
                {
                    vPOI_TOF.isStep = true;
                }
                POI_TOF.Add(vPOI_TOF);
            }

            reduceWallData(POI_TOW, POI_TOF, out POI_TOW_Step, out POI_TOF_Step);
        }

        public static void
        reduceWallData(List<POI> varPOI_TOW, List<POI> varPOI_TOF, out List<WALL_STEP> varTOW_STEP, out List<WALL_STEP> varTOF_STEP)
        {
            //BEGIN REDUCE WALL DATA
            WALL_STEP vTOW_STEP = new WALL_STEP();
            WALL_STEP vTOF_STEP = new WALL_STEP();

            varTOW_STEP = new List<WALL_STEP>();
            varTOF_STEP = new List<WALL_STEP>();

            int i = 0;
            while (i < varPOI_TOW.Count)
            {
                i = i + 1;
                vTOW_STEP = new WALL_STEP();
                vTOW_STEP.BEGSTA = varPOI_TOW[0].Station;
                do
                {
                    i = i + 1;
                    if (varPOI_TOW[i].isStep == true)
                    {
                        vTOW_STEP.ENDSTA = varPOI_TOW[i - 1].Station;
                        vTOW_STEP.ELEV = varPOI_TOW[i - 1].ElevTOW;
                        varTOW_STEP.Add(vTOW_STEP);
                        break;
                    }
                }
                while (true);
            }

            vTOW_STEP = new WALL_STEP();
            int k = varPOI_TOW.Count;
            vTOW_STEP.BEGSTA = varPOI_TOW[k - 1].Station;
            vTOW_STEP.ENDSTA = varPOI_TOW[k].Station;
            vTOW_STEP.ELEV = varPOI_TOW[k].ElevTOW;
            varTOW_STEP.Add(vTOW_STEP);

            i = 0;
            while (i < varPOI_TOF.Count)
            {
                i = i + 1;
                vTOF_STEP = new WALL_STEP();
                vTOF_STEP.BEGSTA = varPOI_TOF[0].Station;
                do
                {
                    i = i + 1;
                    if (varPOI_TOF[i].isStep == true)
                    {
                        vTOF_STEP.ENDSTA = varPOI_TOF[i - 1].Station;
                        vTOF_STEP.ELEV = varPOI_TOF[i - 1].ElevTOF;
                        varTOF_STEP.Add(vTOF_STEP);
                        break;
                    }
                }
                while (true);
            }

            k = varPOI_TOF.Count;
            vTOF_STEP = new WALL_STEP();
            vTOF_STEP.BEGSTA = varPOI_TOF[k - 1].Station;
            vTOF_STEP.ENDSTA = varPOI_TOF[k].Station;
            vTOF_STEP.ELEV = varPOI_TOF[k].ElevTOF;
            varTOF_STEP.Add(vTOF_STEP);
        }

        public static WALL_DATA
        getWallData(ObjectId idAlign)
        {
            BlockTableRecord objXRefDbModelSpace = fStake.XRefDbModelSpace;
            Database objXRefDb = fStake.XRefDb;

            WALL_DATA varWall_Data = new WALL_DATA();

            Point3d varPntLower, varPntUpper;
            string nameAlign = Align.getAlignName(idAlign);
            foreach (ObjectId id in objXRefDbModelSpace)
            {
                Autodesk.AutoCAD.DatabaseServices.Entity objXRefObj = id.getEnt();

                if (objXRefObj is ProfileView)
                {
                    ProfileView objProfileView = (ProfileView)objXRefObj;
                    if (objProfileView.Name == nameAlign)
                    {
                        Extents3d ext3d = (Extents3d)objProfileView.Bounds;
                        varPntLower = ext3d.MinPoint;
                        varPntUpper = ext3d.MaxPoint;

                        double dblStaStart = objProfileView.StationStart;
                        double dblElevStart = objProfileView.ElevationMin;
                        varWall_Data.Name = nameAlign;

                        varWall_Data.PntLower = varPntLower;

                        varWall_Data.PntUpper = varPntUpper;
                        varWall_Data.StaStart = dblStaStart;
                        varWall_Data.ElevStart = dblElevStart;
                        double scale =

                        varWall_Data.ScaleV = Prof.getProfileViewVerticalScale(objProfileView);

                        break;
                    }
                }
            }

            foreach (ObjectId id in objXRefDbModelSpace)
            {
                bool boolTOF = false, boolTOW = false;
                Autodesk.AutoCAD.DatabaseServices.Entity objXRefObj = id.getEnt();
                if (objXRefObj is Polyline)
                {
                    int intPos = objXRefObj.Layer.IndexOf("|");
                    string strLayer = objXRefObj.Layer.Substring(intPos + 1);
                    if (strLayer == "PROFILE-TOF-" + nameAlign)
                    {
                        Polyline objLWPoly = (Polyline)objXRefObj;
                        List<Point3d> varCoords = objLWPoly.ObjectId.getCoordinates3dList();
                        int intUBnd = varCoords.Count - 1;
                        if (varCoords[0].X > varWall_Data.PntLower.X & varCoords[intUBnd].X < varWall_Data.PntUpper.X)
                        {
                            varWall_Data.TOFcoords = varCoords;
                            boolTOF = true;
                        }
                    }

                    if (strLayer == "PROFILE-TOW-" + nameAlign)
                    {
                        Polyline objLWPoly = (Polyline)objXRefObj;
                        List<Point3d> varCoords = objLWPoly.ObjectId.getCoordinates3dList();
                        int intUBnd = varCoords.Count - 1;
                        if (varCoords[0].Y > varWall_Data.PntLower.Y & varCoords[intUBnd].Y < varWall_Data.PntUpper.Y)
                        {
                            varWall_Data.TOWcoords = varCoords;
                            boolTOW = true;
                        }
                    }
                    if (boolTOF & boolTOW)
                    {
                        break;
                    }
                    break;
                }
            }
            return varWall_Data;
        }
    }
}