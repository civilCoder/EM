using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;
using Math = Base_Tools45.Math;

namespace Stake
{
    public static class Stake_GetCardinals
    {
        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;

        public static void
        getCardinals_Horizontal(ObjectId idAlign, ref List<POI> varpoi)
        {
            Alignment objAlign = (Alignment)idAlign.getEnt();
            //BEGIN HORIZONTAL CARDINALS
            AlignmentEntityCollection objAlignEnts = objAlign.Entities;
            AlignmentEntity objAlignEnt = objAlignEnts[0];

            //GET BEG AND END OF FIRST SEGMENT - THEN GET END OF FOLLOWING SEGMENTS
            switch (objAlignEnt.EntityType)
            {
                case AlignmentEntityType.Arc:
                    AlignmentArc objAlignEntArc = (AlignmentArc)objAlignEnt;
                    POI poi = new POI();
                    poi.Station = Math.roundDown3((objAlignEntArc.StartStation));
                    poi.Desc0 = "HC";
                    poi.ClassObj = fStake.ClassObj;
                    varpoi.Add(poi);

                    poi = new POI();
                    poi.Station = Math.roundDown3((objAlignEntArc.EndStation));
                    poi.Desc0 = "HC";
                    poi.ClassObj = fStake.ClassObj;
                    varpoi.Add(poi);
                    break;

                case AlignmentEntityType.Line:
                    AlignmentLine objAlignEntTan = (AlignmentLine)objAlignEnt;
                    poi = new POI();
                    poi.Station = Math.roundDown3((objAlignEntTan.StartStation));
                    poi.Desc0 = "HC";
                    poi.ClassObj = fStake.ClassObj;
                    varpoi.Add(poi);

                    poi = new POI();
                    poi.Station = Math.roundDown3((objAlignEntTan.EndStation));
                    poi.Desc0 = "HC";
                    poi.ClassObj = fStake.ClassObj;
                    varpoi.Add(poi);
                    break;
            }

            POI vpoi = new POI();
            if (objAlignEnts.Count > 1)
            {
                for (int i = 1; i <= objAlignEnts.Count - 1; i++)
                {
                    objAlignEnt = objAlignEnts[i];

                    vpoi = new POI();
                    switch (objAlignEnt.EntityType)
                    {
                        case AlignmentEntityType.Arc:
                            AlignmentArc objAlignEntArc = (AlignmentArc)objAlignEnt;
                            vpoi.Station = Math.roundDown3((objAlignEntArc.EndStation));
                            vpoi.Desc0 = "HC";
                            vpoi.ClassObj = fStake.ClassObj;
                            break;

                        case AlignmentEntityType.Line:
                            AlignmentLine objAlignEntTan = (AlignmentLine)objAlignEnt;
                            vpoi.Station = Math.roundDown3((objAlignEntTan.EndStation));
                            vpoi.Desc0 = "HC";
                            vpoi.ClassObj = fStake.ClassObj;

                            break;
                    }
                    varpoi.Add(vpoi);
                }
            }
            //END HORIZONTAL CARDINALS
        }

        public static List<POI>
        testVerticals(List<POI> varpoi)
        {
            Debug.Print("Begin testVerticals");

            //BEGIN VERTICAL CARDINALS
            List<POI> varPOI_VC = new List<POI>();
            POI vpoi;

            for (int i = 1; i < varpoi.Count; i++)
            {
                double dblElevB4 = System.Math.Round(varpoi[i - 1].Elevation, 2);
                double dblElevX = System.Math.Round(varpoi[i - 0].Elevation, 2);

                double dblStaB4 = System.Math.Round(varpoi[i - 1].Station, 2);
                double dblStaX = System.Math.Round(varpoi[i - 0].Station, 2);
                bool boolPrevWasSame = false;

                //check preceeding point
                if (dblStaB4 == dblStaX)
                {
                    vpoi = new POI();

                    if (varpoi[i - 1].Desc0 == "PNT")
                    {
                        vpoi = varpoi[i - 1];
                        if (varpoi[i - 0].Desc0 == "HC")
                        {
                            vpoi.Desc0 = "HC";
                        }
                    }
                    else if (varpoi[i - 0].Desc0 == "PNT")
                    {
                        vpoi = varpoi[i - 0];
                        if (varpoi[i - 1].Desc0 == "HC")
                        {
                            vpoi.Desc0 = "HC";
                            vpoi.Station = varpoi[i - 1].Station;
                        }
                    }
                    else if (varpoi[i - 0].Desc0 == "HC")
                    {
                        vpoi = varpoi[i - 0];
                        if (varpoi[i - 1].Desc0 == "HC")
                        {
                            vpoi.Desc0 = "HC";
                            if (varpoi[i - 1].Elevation < varpoi[i - 0].Elevation)
                            {
                                vpoi.Elevation = varpoi[i - 1].Elevation;
                            }
                            else
                            {
                                vpoi.Elevation = varpoi[i - 0].Elevation;
                            }
                        }
                    }

                    boolPrevWasSame = true;

                    if (vpoi.Desc0 != "PNT")
                    {
                        Debug.Print(i + " " + vpoi.Station + " " + vpoi.Elevation + " " + vpoi.Desc0);
                    }
                    else
                    {
                        Debug.Print(i + " " + vpoi.Station + " " + vpoi.Elevation + " " + "PNT " + vpoi.PntNum);
                    }

                    varPOI_VC.Add(vpoi);
                }
                else if (dblElevB4 == dblElevX & dblElevB4 != 0 & dblElevX != 0)
                {
                    if (System.Math.Abs(dblStaB4 - dblStaX) > 0.5)
                    {
                        vpoi = new POI();

                        if (varpoi[i - 1].Desc0 == "PNT" & varpoi[i - 0].Desc0 == "PNT")
                        {
                            vpoi = varpoi[i - 1];
                        }
                        else if (varpoi[i - 1].Desc0 == "PNT")
                        {
                            vpoi = varpoi[i - 1];
                            if (varpoi[i - 0].Desc0 == "HC")
                            {
                                vpoi.Desc0 = "HC";
                            }
                        }
                        else if (varpoi[i - 0].Desc0 == "PNT")
                        {
                            vpoi = varpoi[i - 0];
                            if (varpoi[i - 1].Desc0 == "HC")
                            {
                                vpoi.Desc0 = "HC";
                            }
                        }
                        else if (varpoi[i - 0].Desc0 == "HC")
                        {
                            vpoi = varpoi[i - 0];
                            if (varpoi[i - 1].Desc0 == "HC")
                            {
                                vpoi.Desc0 = "HC";
                                if (varpoi[i - 1].Elevation < varpoi[i - 0].Elevation)
                                {
                                    vpoi.Elevation = varpoi[i - 1].Elevation;
                                }
                                else
                                {
                                    vpoi.Elevation = varpoi[i - 0].Elevation;
                                }
                            }
                        }

                        if (vpoi.Desc0 != "PNT")
                        {
                            Debug.Print(i + " " + vpoi.Station + " " + vpoi.Elevation + " " + vpoi.Desc0);
                        }
                        else
                        {
                            Debug.Print(i + " " + vpoi.Station + " " + vpoi.Elevation + " " + "PNT " + vpoi.PntNum);
                        }
                        varPOI_VC.Add(vpoi);
                    }
                }
                else
                {
                    if (boolPrevWasSame)
                    {
                        boolPrevWasSame = false;
                    }
                    else
                    {
                        vpoi = varpoi[i - 1]; //add preceeding point

                        if (vpoi.Desc0 != "PNT")
                        {
                            Debug.Print(i + " " + vpoi.Station + " " + vpoi.Elevation + " " + vpoi.Desc0 + " " + vpoi.PntNum);
                        }
                        else
                        {
                            Debug.Print(i + " " + vpoi.Station + " " + vpoi.Elevation + " " + vpoi.Desc0 + " " + vpoi.PntNum);
                        }

                        varPOI_VC.Add(vpoi);
                    }
                }
            }

            Debug.Print("End testVerticals");

            //add last POI if unique
            int n = varpoi.Count - 1;
            if (System.Math.Round(varpoi[n - 1].Station, 1) != System.Math.Round(varpoi[n - 0].Station, 1))
            {
                varPOI_VC.Add(varpoi[n]);
            }
            else
            {
                vpoi = varPOI_VC[varPOI_VC.Count - 1];
                vpoi.Desc0 = varpoi[n].Desc0;
                varPOI_VC[varPOI_VC.Count - 1] = vpoi;
            }
            //END VERTICAL CARDINALS

            return varPOI_VC;
        }

        public static bool
        getCardinals_Vertical(ObjectId idAlign, ref List<POI> varpoi)
        {
            Alignment objAlign = (Alignment)idAlign.getEnt();
            StationEquationCollection objStationEQs = objAlign.StationEquations;
            double dblSlope = 0, dblElevBase = 0, dblSlopeAhead = 0, dblLen = 0, dblElev = 0, dblLenAhead = 0, dblLenBack = 0;

            Debug.Print("Begin raw HC and Points");

            for (int i = 0; i < varpoi.Count; i++)
            {
                Debug.Print(i + " " + varpoi[i].Station + " " + varpoi[i].Elevation + " " + varpoi[i].Desc0 + " " + varpoi[i].PntNum);
            }
            Debug.Print("End raw HC and Points");

            List<POI> varPOI_VC = testVerticals(varpoi);
            //varPOI_VC = testVerticals(varPOI_VC)    'second pass to take care of triplicate points

            //BEGIN PREPARE LIST OF HC WITH ELEVATIONS
            POI vpoi = new POI();
            List<double> dblStaHC = new List<double>();
            List<POI> varPOI_HC = new List<POI>();

            for (int i = 0; i < varPOI_VC.Count; i++)
            {
                if (varPOI_VC[i].Desc0 == "HC")
                {
                    dblStaHC.Add(varPOI_VC[i].Station);//list of HC locations by station

                    if (varPOI_VC[i].Elevation != 0)
                    {
                        varPOI_HC.Add(varPOI_VC[i]);//list of HCs with elevation
                    }
                    else
                    {
                        vpoi = varPOI_VC[i];
                        vpoi.Desc0 = "H0";
                        varPOI_VC[i] = vpoi;
                    }
                }
            }
            //END PREPARE LIST OF HC WITH ELEVATIONS

            //BEGIN SLOPE CALC BETWEEN HC WITH ELEVATION

            if (objStationEQs.Count == 0)
            {
                for (int i = 0; i < varPOI_HC.Count; i++)
                {
                    dblLenAhead = varPOI_HC[i + 1].Station - varPOI_HC[i + 0].Station;
                    if (dblLenAhead != 0)
                    {
                        varPOI_HC[i].SlopeH2H = System.Math.Round((varPOI_HC[i + 1].Elevation - varPOI_HC[i + 0].Elevation) / dblLenAhead, 5);
                    }
                }
            }
            else
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Alignment has Station Equation - See Developer");
                return false;
            }
            //END SLOPE CALC BETWEEN HC WITH ELEVATION

            //BEGIN SET ELEVATION ON MISSING HC
            //BEGIN CHECK ON FIRST POINT

            if (varPOI_HC.Count > 0)
            {
                //k = UBound(varPOI_HC)                     'start with existing list of HCs with elevation
                if (varPOI_VC[0].Desc0 == "H0")
                {
                    int x = 0;
                    for (int i = 1; i < varPOI_VC.Count; i++)
                    {
                        dblElev = varPOI_VC[i].Elevation;
                        if (dblElev != 0)
                        {
                            x = i;
                            break;
                        }
                    }

                    dblLen = varPOI_HC[0].Station - varPOI_VC[x].Station;

                    if (dblLen == 0)
                    {
                        varPOI_VC[0].Elevation = varPOI_HC[0].Elevation + -varPOI_HC[0].SlopeH2H * (varPOI_HC[0].Station - varPOI_VC[0].Station);
                    }
                    else
                    {
                        dblSlopeAhead = (varPOI_HC[0].Elevation - varPOI_VC[x].Elevation) / dblLen;
                        varPOI_VC[0].Elevation = dblElev + -dblSlopeAhead * (varPOI_VC[x].Station - varPOI_VC[0].Station);
                    }

                    varPOI_VC[0].Desc0 = "HE";

                    varPOI_HC.Add(varPOI_VC[0]);

                    var sortSta = from s in varPOI_HC
                                  orderby s.Station ascending
                                  select s;
                    List<POI> poiTmp = new List<POI>();
                    foreach (var s in sortSta)
                        poiTmp.Add(s);

                    varPOI_HC = poiTmp;

                    //recompute slope between all HCs instead of finding matching station
                    for (int i = 0; i < varPOI_HC.Count; i++)
                    {
                        varPOI_HC[i].SlopeH2H = System.Math.Round((varPOI_HC[i + 1].Elevation - varPOI_HC[i + 0].Elevation) / (varPOI_HC[i + 1].Station - varPOI_HC[i + 0].Station), 5);
                    }
                }
                else
                {
                    var sortSta = from s in varPOI_HC
                                  orderby s.Station ascending
                                  select s;
                    List<POI> poiTmp = new List<POI>();
                    foreach (var s in sortSta)
                        poiTmp.Add(s);

                    varPOI_HC = poiTmp;
                }
                //END CHECK ON FIRST POINT

                //BEGIN CHECK ON REMAINING POINTS
                //start with existing list of HCs with elevation

                for (int i = 1; i < varPOI_VC.Count; i++)
                {
                    if (varPOI_VC[i].Desc0 == "H0")
                    {
                        for (int j = 1; j < varPOI_VC.Count; j++)
                        {
                            if (varPOI_VC[i - 1].Station < varPOI_VC[i].Station & varPOI_VC[i].Station < varPOI_VC[i + 1].Station)
                            {
                                if (varPOI_VC[i + 1].Elevation != 0 & varPOI_VC[i - 1].Elevation != 0)
                                {
                                    dblLen = varPOI_VC[i + 1].Station - varPOI_VC[i - 1].Station;
                                    dblElevBase = varPOI_VC[i - 1].Elevation;
                                    dblSlope = (varPOI_VC[i + 1].Elevation - dblElevBase) / dblLen;
                                    dblLen = varPOI_VC[i + 0].Station - varPOI_VC[i - 1].Station;
                                }
                                else if (varPOI_VC[i + 1].Elevation != 0 & varPOI_VC[i - 1].Elevation == 0)
                                {
                                    dblLen = varPOI_VC[i + 1].Station - varPOI_VC[i - 2].Station;
                                    dblElevBase = varPOI_VC[i - 2].Elevation;
                                    dblSlope = (varPOI_VC[i + 1].Elevation - dblElevBase) / dblLen;
                                    dblLen = varPOI_VC[i + 0].Station - varPOI_VC[i - 2].Station;
                                }
                                else if (varPOI_VC[i + 1].Elevation == 0 & varPOI_VC[i - 1].Elevation != 0)
                                {
                                    if (i < varPOI_VC.Count - 2)
                                    {
                                        dblLen = varPOI_VC[i + 2].Station - varPOI_VC[i - 1].Station;
                                        dblElevBase = varPOI_VC[i - 1].Elevation;
                                        dblSlope = (varPOI_VC[i + 2].Elevation - dblElevBase) / dblLen;
                                        dblLen = varPOI_VC[i + 0].Station - varPOI_VC[i - 1].Station;
                                    }
                                    else
                                    {
                                    }
                                }
                                else if (i > 1 && i < varPOI_VC.Count - 3)
                                {
                                    if (varPOI_VC[i + 1].Elevation == 0 & varPOI_VC[i - 1].Elevation == 0)
                                    {
                                        if (i < varPOI_VC.Count - 2)
                                        {
                                            dblLen = varPOI_VC[i + 2].Station - varPOI_VC[i - 2].Station;
                                            dblElevBase = varPOI_VC[i - 2].Elevation;
                                            dblSlope = (varPOI_VC[i + 2].Elevation - dblElevBase) / dblLen;
                                            dblLen = varPOI_VC[i + 0].Station - varPOI_VC[i - 2].Station;
                                        }
                                        else
                                        {
                                        }
                                    }
                                }

                                POI poiTmp = varPOI_VC[i];

                                poiTmp.Elevation = System.Math.Round(dblElevBase + dblSlope * dblLen, 3);
                                poiTmp.Desc0 = "HE";
                                varPOI_VC[i] = poiTmp;

                                varPOI_HC.Add(varPOI_VC[i]);
                                break;
                            }
                        }
                    }
                }
            }

            int a = varPOI_VC.Count - 1;

            if (varPOI_VC[a].Desc0 == "H0")
            {
                dblLen = varPOI_VC[a - 1].Station - varPOI_VC[a - 2].Station;
                dblElevBase = varPOI_VC[a - 1].Elevation;
                dblSlope = (dblElevBase - varPOI_VC[a - 2].Elevation) / dblLen;
                dblLen = varPOI_VC[a].Station - varPOI_VC[a - 1].Station;

                POI poiTemp = varPOI_VC[a];
                poiTemp.Elevation = System.Math.Round(dblElevBase + dblSlope * dblLen, 3);
                poiTemp.Desc0 = "HE";
                varPOI_VC[a] = poiTemp;

                varPOI_HC.Add(varPOI_VC[a]);
            }

            Debug.Print("Begin SET ELEVATION ON MISSING HC");

            for (int i = 0; i < varPOI_HC.Count; i++)
            {
                Debug.Print(i + " " + varPOI_HC[i].Station + " " + varPOI_HC[i].Elevation + " " + varPOI_HC[i].Desc0 + " " + varPOI_HC[i].PntNum);
            }
            Debug.Print("End SET ELEVATION ON MISSING HC");

            //END CHECK ON REMAINING POINTS
            //END SET ELEVATION ON MISSING HC

            varPOI_HC = varPOI_HC.sortPOIbyStation();

            //BEGIN SLOPE CALC FOR EACH POINT

            dblLenAhead = (varPOI_VC[1].Station - varPOI_VC[0].Station);

            if (dblLenAhead != 0)
            {
                varPOI_VC[0].SlopeAhead = System.Math.Round((varPOI_VC[1].Elevation - varPOI_VC[0].Elevation) / dblLenAhead, 5);
            }

            int u = varPOI_VC.Count - 1;
            for (int i = 1; i < u - 1; i++)
            {
                dblLenBack = (varPOI_VC[i - 0].Station - varPOI_VC[i - 1].Station);
                dblLenAhead = (varPOI_VC[i + 1].Station - varPOI_VC[i + 0].Station);

                vpoi = varPOI_VC[i];

                if (dblLenBack != 0)
                {
                    vpoi.SlopeBack = System.Math.Round((varPOI_VC[i - 1].Elevation - varPOI_VC[i - 0].Elevation) / dblLenBack, 5);
                }

                if (dblLenAhead != 0)
                {
                    vpoi.SlopeAhead = System.Math.Round((varPOI_VC[i + 1].Elevation - varPOI_VC[i + 0].Elevation) / dblLenAhead, 5);
                }
                varPOI_VC[i] = vpoi;
            }

            dblLenBack = (varPOI_VC[u - 0].Station - varPOI_VC[u - 1].Station);
            varPOI_VC[u].SlopeBack = System.Math.Round((varPOI_VC[u - 1].Elevation - varPOI_VC[u - 0].Elevation) / dblLenBack, 5);
            //END SLOPE CALC FOR EACH POINT

            List<int> intIndexHC = new List<int>();
            for (int i = 0; i < varPOI_VC.Count; i++)
            {
                switch (varPOI_VC[i].Desc0.Substring(0, 2))
                {
                    case "HC":
                    case "HE":
                    case "H0":
                        intIndexHC.Add(i);
                        break;
                }
            }

            Debug.Print("Begin POI_VC");

            for (int i = 0; i < varPOI_VC.Count; i++)
            {
                Debug.Print(i + " " + varPOI_VC[i].Station + " " + varPOI_VC[i].Elevation + " " + varPOI_VC[i].SlopeAhead + " " + varPOI_VC[i].SlopeBack + " " + varPOI_VC[i].Desc0 + ": " + varPOI_VC[i].PntNum);
            }
            Debug.Print("End POI_VC");

            //BEGIN CHECK FOR POINTS ON GRADE
            int intSlopeO = 0, intSlopeX = 0;
            double dblSlopeTotal = 0, dblSlopeAvg = 0, dblSlopeDiff = 0, dblSlopeDiffH2H = 0;
            bool boolSlopeSame = false;

            if (intIndexHC.Count > 0)
            {
                for (int i = 0; i < intIndexHC.Count - 1; i++)
                {
                    if (varPOI_VC[i].SlopeAhead >= 0)
                    {
                        intSlopeO = 1;
                    }
                    else
                    {
                        intSlopeO = -1;
                    }

                    int k = 0;
                    dblSlopeTotal = 0.0;

                    for (int j = intIndexHC[i + 1]; j < intIndexHC[i + 1] - 1; j++)
                    {
                        if (varPOI_VC[j].SlopeAhead >= 0)
                        {
                            intSlopeX = 1;
                        }
                        else
                        {
                            intSlopeX = -1;
                        }

                        if (intSlopeX == intSlopeO)
                        {
                            boolSlopeSame = true;

                            k++;

                            dblSlopeTotal = dblSlopeTotal + varPOI_VC[j].SlopeAhead;
                        }
                        else
                        {
                            boolSlopeSame = false;
                            break;
                        }
                    }

                    if (boolSlopeSame)
                    {
                        dblSlopeAvg = dblSlopeTotal / k;
                        dblSlopeDiff = System.Math.Abs(System.Math.Round(varPOI_HC[i].SlopeH2H - dblSlopeAvg, 5));

                        if (dblSlopeDiff < 0.0005)
                        {
                            Debug.Print(varPOI_HC[i].Station + " SlopeH2H: " + varPOI_HC[i].SlopeH2H);
                            Debug.Print(varPOI_HC[i + 1].Station + " SlopeH2H: " + varPOI_HC[i + 1].SlopeH2H);
                        }
                        else
                        {
                            for (int j = intIndexHC[i] + 1; j <= intIndexHC[i + 1] - 1; j++)
                            {
                                dblSlopeDiffH2H = System.Math.Abs(System.Math.Round(varPOI_VC[j].SlopeAhead - varPOI_HC[i].SlopeH2H, 4));
                                dblSlopeDiff = System.Math.Abs(System.Math.Round(varPOI_VC[j].SlopeAhead + varPOI_VC[j].SlopeBack, 4));

                                if (dblSlopeDiff <= 0.0005)
                                {
                                    Debug.Print(varPOI_VC[j].Station + " " + "dblSlopeDiff <= 0.0005");
                                }
                                else if (dblSlopeDiff > 0.0005 | dblSlopeDiffH2H > 0.0005)
                                {
                                    varPOI_VC[j].Desc0 = "GB";
                                    Debug.Print(varPOI_VC[j].Station + " " + dblSlopeDiff);
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int j = intIndexHC[i] + 1; j < intIndexHC[i + 1] + 1; j++)
                        {
                            dblSlopeDiff = System.Math.Abs(System.Math.Round(varPOI_VC[j].SlopeAhead + varPOI_VC[j].SlopeBack, 4));

                            if (dblSlopeDiff <= 0.0005)
                            {
                                Debug.Print(varPOI_VC[j].Station + " " + "dblSlopeDiff <= 0.0005");
                            }
                            else
                            {
                                varPOI_VC[j].Desc0 = "GB";
                                Debug.Print(varPOI_VC[j].Station + " " + dblSlopeDiff);
                            }
                        }
                    }
                }
            }
            else
            {
                if (varPOI_VC[0].SlopeAhead >= 0)
                {
                    intSlopeO = 1;
                }
                else
                {
                    intSlopeO = -1;
                }

                int k = 0;
                dblSlopeTotal = 0.0;

                //compute average slope between changes in sign of slope
                for (int j = 1; j < varPOI_VC.Count; j++)
                {
                    if (varPOI_VC[j].SlopeAhead >= 0)
                    {
                        intSlopeX = 1;
                    }
                    else
                    {
                        intSlopeX = -1;
                    }

                    if (intSlopeX == intSlopeO)
                    {
                        boolSlopeSame = true;

                        k = k + 1;

                        dblSlopeTotal = dblSlopeTotal + varPOI_VC[j].SlopeAhead;
                    }
                    else
                    {
                        boolSlopeSame = false;
                        break;
                    }
                }

                if (boolSlopeSame)
                {
                    dblSlopeAvg = dblSlopeTotal / k;
                    dblSlopeDiff = System.Math.Abs(System.Math.Round(varPOI_HC[0].SlopeH2H - dblSlopeAvg, 5));

                    if (dblSlopeDiff < 0.0005)
                    {
                        Debug.Print(varPOI_HC[0].Station + " SlopeH2H: " + varPOI_HC[0].SlopeH2H);
                    }
                    else
                    {
                        //check points between HC
                        for (int j = 1; j < varPOI_VC.Count; j++)
                        {
                            dblSlopeDiffH2H = System.Math.Abs(System.Math.Round(varPOI_VC[j].SlopeAhead - varPOI_HC[0].SlopeH2H, 4));
                            dblSlopeDiff = System.Math.Abs(System.Math.Round(varPOI_VC[j].SlopeAhead + varPOI_VC[j].SlopeBack, 4));

                            if (dblSlopeDiff <= 0.0005)
                            {
                                Debug.Print(varPOI_VC[j].Station + " " + "dblSlopeDiff <= 0.0005");
                            }
                            else if (dblSlopeDiff > 0.0005 | dblSlopeDiffH2H > 0.0005)
                            {
                                varPOI_VC[j].Desc0 = "GB";
                                Debug.Print(varPOI_VC[j].Station + " " + dblSlopeDiff);
                            }
                        }
                    }
                }
                else
                {
                    //check points between HC
                    for (int j = 1; j < varPOI_VC.Count; j++)
                    {
                        dblSlopeDiff = System.Math.Abs(System.Math.Round(varPOI_VC[j].SlopeAhead + varPOI_VC[j].SlopeBack, 4));

                        if (dblSlopeDiff <= 0.0005)
                        {
                            Debug.Print(varPOI_VC[j].Station + " " + "dblSlopeDiff <= 0.0005");
                        }
                        else
                        {
                            varPOI_VC[j].Desc0 = "GB";
                            Debug.Print(varPOI_VC[j].Station + " " + dblSlopeDiff);
                        }
                    }
                }
            }

            //END CHECK FOR POINTS ON GRADE

            Debug.Print("Begin Mod POI_VC");

            for (int i = 0; i < varPOI_VC.Count; i++)
            {
                Debug.Print(i + " " + varPOI_VC[i].Station + " " + varPOI_VC[i].Elevation + " " + varPOI_VC[i].SlopeAhead + " " + varPOI_VC[i].SlopeBack + " " + varPOI_VC[i].Desc0 + ": " + varPOI_VC[i].PntNum);
            }
            Debug.Print("End Mod POI_VC");

            //BEGIN LIST OF CARDINALS

            for (int i = 0; i < varPOI_VC.Count; i++)
            {
                switch (varPOI_VC[i].Desc0)
                {
                    case "HC":
                    case "HE":

                        if (varPOI_VC[i].SlopeBack + varPOI_VC[i].SlopeAhead != 0)
                        {
                            varPOI_VC[i].DescX = "GB " + varPOI_VC[i].Desc0;
                            //either HC or HE

                            varPOI_HC.Add(varPOI_VC[i]);
                        }
                        else if (i == 0 || i == varPOI_VC.Count - 1)
                        {
                            varPOI_HC.Add(varPOI_VC[i]);
                        }

                        break;

                    case "GB":

                        varPOI_HC.Add(varPOI_VC[i]);

                        break;
                }
            }
            //END LIST OF CARDINALS

            Debug.Print("Begin Cardinals HC");

            for (int i = 0; i < varPOI_HC.Count; i++)
            {
                Debug.Print(i + " " + varPOI_HC[i].Station + " " + varPOI_HC[i].Elevation + " " + varPOI_HC[i].SlopeAhead + " " + varPOI_HC[i].Desc0 + " " + varPOI_HC[i].DescX);
            }
            Debug.Print("End Cardinals HC");

            varpoi = new List<POI>();
            varpoi = varPOI_HC;

            //first and last entries are cardinals
            for (int i = 1; i < varpoi.Count - 1; i++)
            {
                vpoi = varpoi[i];

                if (varpoi[i].SlopeBack > 0 && varpoi[i].SlopeAhead > 0)
                {
                    vpoi.Desc0 = "GB LP";
                }
                else if (varpoi[i].SlopeBack < 0 & varpoi[i].SlopeAhead < 0)
                {
                    vpoi.Desc0 = "GB HP";
                }

                varpoi[i] = vpoi;
            }
            //END VERTICAL CARDINALS

            Debug.Print("Begin Final Cardinals");

            for (int i = 0; i < varpoi.Count; i++)
            {
                Debug.Print(i + " " + varpoi[i].Station + " " + varpoi[i].Elevation + " " + varpoi[i].SlopeAhead + " " + varpoi[i].SlopeBack + " " + varpoi[i].Desc0 + " " + varpoi[i].DescX);
            }
            Debug.Print("End Final Cardinals");
            return true;
        }

        public static bool
        isCardinal(double dblStation, List<double> dblStations)
        {
            for (int i = 0; i < dblStations.Count; i++)
            {
                if (System.Math.Round(dblStation, 3) == System.Math.Round(dblStations[i], 3))
                {
                    return true;
                }
            }
            return false;
        }

        public static void
        checkBegAndEndStations(ObjectId idAlign, ref List<POI> varpoi)
        {
            Alignment objAlign = (Alignment)idAlign.getEnt();

            double dblStaBeg = Math.roundDown3((objAlign.StartingStation));
            double dblStaEnd = Math.roundDown3((objAlign.EndingStation));

            if (varpoi.Count == 0)
                return;
            double dblEasting = 0, dblNorthing = 0;

            varpoi = varpoi.sortPOIbyStation();
            int j = varpoi.Count - 1;
            POI vpoi = varpoi[j];
            if (vpoi.Station != dblStaEnd)
            {
                idAlign.getAlignPointLoc(dblStaEnd - 0.001, 0.0, ref dblEasting, ref dblNorthing);

                vpoi.Station = Math.roundDown3(dblStaEnd);
                vpoi.Desc0 = "END";
                vpoi.ClassObj = fStake.ClassObj;

                varpoi.Add(vpoi);
            }
            else
            {
                vpoi.DescX = "END";
                varpoi[j] = vpoi;
            }

            if (System.Math.Round(varpoi[0].Station, 3) != dblStaBeg)
            {
                idAlign.getAlignPointLoc(dblStaBeg, 0.0, ref dblEasting, ref dblNorthing);

                vpoi = new POI();
                vpoi.Station = dblStaBeg;
                vpoi.Desc0 = "BEG";
                vpoi.ClassObj = fStake.ClassObj;

                varpoi.Add(vpoi);
            }
            else
            {
                vpoi = varpoi[0];
                vpoi.DescX = "BEG";
                varpoi[0] = vpoi;
            }

            varpoi = varpoi.sortPOIbyStation();
        }

        public static void
        checkBegAndEndDesc(ObjectId idAlign, ref List<POI> varpoi)
        {
            Alignment objAlign = (Alignment)idAlign.getEnt();

            double dblStaBeg = Math.roundDown3((objAlign.StartingStation));
            double dblStaEnd = Math.roundDown3((objAlign.EndingStation));

            POI vpoi = varpoi[0];

            if (vpoi.Station == dblStaBeg)
            {
                vpoi.Desc0 = vpoi.Desc0.Replace("AP", "BEG");
                vpoi.Desc0 = vpoi.Desc0.Replace("HC", "BEG");
                vpoi.Desc0 = vpoi.Desc0.Replace("HE", "BEG");
                vpoi.DescX = vpoi.Desc0;
                varpoi[0] = vpoi;
            }

            int j = varpoi.Count - 1;
            vpoi = varpoi[j];

            if (vpoi.Station == dblStaEnd)
            {
                vpoi.Desc0 = vpoi.Desc0.Replace("AP", "END");
                vpoi.Desc0 = vpoi.Desc0.Replace("HC", "END");
                vpoi.Desc0 = vpoi.Desc0.Replace("HE", "END");
                vpoi.DescX = vpoi.Desc0;
                varpoi[j] = vpoi;
            }
        }

        public static bool
        userAddCrossingsFeatures(ObjectId idAlign, ref List<POI> varpoi)
        {
            Alignment objAlign = (Alignment)idAlign.getEnt();
            bool escape;
            string xRefPath = "";
            //BEGIN GET CROSSING FEATURES
            List<string> strLayName = new List<string>();
            List<string> strHandle = new List<string>();
            List<Entity> ents = new List<Entity>();

            do
            {
                Entity obj = xRef.getEntity("Select crossing entity <ESC to exit>: ", out escape, out xRefPath);

                if (obj == null || escape)
                {
                    break;
                }

                strLayName.Add(obj.Layer);
                strHandle.Add(obj.Handle.ToString());
                ents.Add(obj);
            }
            while (true);

            double dblStation = 0, dblOffset = 0;
            foreach (Entity ent in ents)
            {
                Point3dCollection varPntInt = new Point3dCollection();
                objAlign.IntersectWith(ent, Intersect.OnBothOperands, varPntInt, IntPtr.Zero, IntPtr.Zero);
                //EXTEND NONE

                if (varPntInt.Count == 0)
                    return false;

                int k = varPntInt.Count;

                for (int n = 0; n < k; n++)
                {
                    Point3d dblPnt = varPntInt[n];

                    objAlign.StationOffset(dblPnt.X, dblPnt.Y, ref dblStation, ref dblOffset);

                    POI vpoi = new POI();

                    if (ent.Layer.Contains(fStake.NameStakeObject))
                    {
                        vpoi.Desc0 = "TEE";
                    }
                    else
                    {
                        int intPos = ent.Layer.IndexOf("|");
                        vpoi.Desc0 = ent.Layer.Substring(intPos + 1);
                        vpoi.CrossDesc = ent.Layer.Substring(intPos + 1);
                    }

                    vpoi.Station = Math.roundDown3(dblStation);
                    vpoi.ClassObj = fStake.ClassObj;
                    varpoi.Add(vpoi);
                }
            }

            varpoi = varpoi.sortPOIbyStation();
            return true;
        }

        public static void
        getCrossingAligns(ObjectId idAlign, ref List<POI> varpoi)
        {
            Alignment objAlign = (Alignment)idAlign.getEnt();
            bool boolAdd = false;
            double dblStation = 0, dblStationX = 0, dblOffset = 0;
            ObjectIdCollection idAligns = BaseObjs._civDoc.GetAlignmentIds();
            for (int i = 0; i < idAligns.Count; i++)
            {
                if (Align.getAlignName(idAligns[i]) != Align.getAlignName(idAlign))
                {
                    Alignment objAlignX = (Alignment)idAligns[i].getEnt();
                    objAlignX.Highlight();

                    if (objAlignX.IsReferenceObject)
                    {
                        boolAdd = true;

                        Point3dCollection varPntInt = new Point3dCollection();
                        objAlign.IntersectWith(objAlignX, Intersect.OnBothOperands, varPntInt, IntPtr.Zero, IntPtr.Zero);

                        for (int k = 0; k < varPntInt.Count; k++)
                        {
                            Point3d pnt3dInt = varPntInt[k];
                            idAlign.getAlignStaOffset(pnt3dInt, ref dblStation, ref dblOffset);

                            POI vpoi = new POI();

                            vpoi.Station = Math.roundDown3(dblStation);
                            vpoi.Desc0 = fStake.NameStakeObject;
                            vpoi.ClassObj = fStake.ClassObj;
                            vpoi.CrossAlign = objAlignX.Name;

                            //GET PIPE INVERT ELEVATION

                            PIPE_DATA varPipeData = Stake_GetPipeInvertElev.getPipeData(dblStation, idAlign);

                            vpoi.Size = varPipeData.Size;
                            vpoi.Invert = varPipeData.Invert;

                            idAligns[i].getAlignStaOffset(pnt3dInt, ref dblStationX, ref dblOffset);

                            vpoi.CrossAlignSta = System.Math.Round(dblStation, 3);

                            //GET PIPE INVERT ELEVATION

                            varPipeData = Stake_GetPipeInvertElev.getPipeData(dblStationX, idAligns[i]);

                            vpoi.CrossAlignSize = varPipeData.Size;
                            vpoi.CrossAlignInv = varPipeData.Invert;

                            varpoi.Add(vpoi);
                        }
                    }
                }
            }

            if (boolAdd)
            {
                varpoi = varpoi.sortPOIbyStation();
            }
        }
    }
}