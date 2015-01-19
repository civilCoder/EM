using Base_Tools45;
using System.Collections.Generic;
using System.Linq;
using Math = Base_Tools45.Math;

namespace Stake
{
    public static class Stake_GetStationing
    {
        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;

        public static bool
        isInTolerance(List<POI> varPoi, double sta, double tolerance)
        {
            for (int i = 0; i < varPoi.Count; i++)
            {
                if (System.Math.Abs(sta - varPoi[i].Station) < tolerance)
                {
                    return true;
                }
            }
            return false;
        }

        public static void
        getStationing(ref List<POI> varpoi, double dblStaBeg, double dblStaEnd)
        {
            POI vPoi;
            bool boolBC = false;
            bool boolEC = false;
            bool boolSta = true;

            double dblInterval0 = double.Parse(fStake.cboInterval.Text);
            double dblTolerance = double.Parse(fStake.cboTolerance.Text);

            //BEGIN ON-STATION
            if (fStake.optYes.Checked)
            {
                double dblSta = getNominalStation(dblStaBeg, dblInterval0);

                //get nominal stations
                POI vPoiCur = new POI();
                do
                {
                    dblSta += dblInterval0;
                    boolSta = false;
                    for (int x = 0; x < varpoi.Count; x++)
                    {
                        vPoiCur = varpoi[x];
                        if (System.Math.Abs(varpoi[x].Station - dblSta) < double.Parse(fStake.cboTolerance.Text))
                        {
                            dblSta = varpoi[x].Station;
                            boolSta = true;
                            break;
                        }
                    }

                    if (boolSta == false && dblSta < dblStaEnd)
                    {
                        vPoi = new POI();
                        vPoi.Station = Math.roundDown3(dblSta);
                        vPoi.ClassObj = fStake.ClassObj;

                        if (fStake.ClassObj == "WALL")
                        {
                            vPoi.Desc0 = vPoiCur.Desc0;
                            vPoi.ElevTOF = vPoiCur.ElevTOF;
                            vPoi.ElevTOW = vPoiCur.ElevTOW;
                        }
                        varpoi.Add(vPoi);
                    }
                } while (dblSta < dblStaEnd);

                var sortPOI = from p in varpoi
                              orderby p.Station ascending
                              select p;

                varpoi = varpoi.sortPOIbyStation();

                int n = 0;
                List<POI> varPoiTmp = new List<POI>();

                for (int j = 0; j < varpoi.Count; j++)
                {
                    if (varpoi[j].Desc0 == "BC")
                    {
                        n = j;

                        do
                        {
                            n += 1;
                            if (varpoi[n].Desc0 != "EC" & varpoi[n].Desc0 != "BC")
                            {
                                boolEC = false;
                            }
                            else
                            {
                                break;
                            }
                        } while (true);
                        //----------------------------------------------------------------------------------
                        int p = 0;
                        for (p = j; p < n; p++)
                        {
                            varPoiTmp.Add(varpoi[j]);
                            double dblStaBeg0 = getNominalStation(varpoi[p].Station, dblInterval0 / 2);

                            do
                            {
                                if (dblStaBeg0 - varpoi[p].Station > dblTolerance && varpoi[p + 1].Station - dblStaBeg0 > dblTolerance)
                                {
                                    varPoiTmp.Add(new POI { Station = dblStaBeg0, ClassObj = fStake.ClassObj });
                                }
                                else if (varpoi[p + 1].Station - dblStaBeg0 <= dblTolerance)
                                {
                                    varPoiTmp.Add(varpoi[p + 1]);
                                    break;
                                }

                                dblStaBeg0 += dblInterval0 / 2;
                            } while (true);
                        }
                        j = p;
                    }
                    else
                    {
                        varPoiTmp.Add(varpoi[j]);
                    }
                }

                var sortTmp = from p in varPoiTmp
                              orderby p.Station ascending
                              select p;

                varpoi = varPoiTmp.sortPOIbyStation();

                if (varpoi[varpoi.Count - 1].Station > dblStaEnd)
                {
                    varpoi.RemoveAt(varpoi.Count - 1);
                }

                //END GET NOMINAL STATIONS

                //BEGIN GET EVEN INTERVALS
            }
            else//not fStake.optYes
            {
                boolBC = false;
                boolEC = true;

                if (varpoi[0].Desc0 == "BC")
                {
                    boolBC = true;
                    boolEC = false;
                }

                for (int i = 1; i < varpoi.Count; i++)
                {
                    switch (varpoi[i].Desc0)
                    {
                        case "BC":
                        case "PCC":

                            boolBC = true;

                            if (boolEC)
                            {
                                getStationsOnTangent(ref i, varpoi, dblInterval0, dblTolerance);
                            }

                            boolEC = false;

                            break;

                        case "EC":

                            boolBC = false;
                            boolEC = true;

                            break;

                        default:

                            if (!boolBC & boolEC)
                            {
                                getStationsOnTangent(ref i, varpoi, dblInterval0, dblTolerance);
                            }

                            break;
                    }
                } //END GET EVEN INTERVALS
            } //END ON-STATION
        }

        public static void
        getStationsOnTangent(ref int i, List<POI> varpoi, double dblInterval0, double dblTolerance)
        {
            bool boolAddRemain = false;
            double dblDistance = (varpoi[i + 0].Station - varpoi[i - 1].Station);

            int intSEGs = (int)System.Math.Truncate(dblDistance / dblInterval0);

            if (dblDistance % dblInterval0 != 0)
            {
                if (dblDistance - (intSEGs * dblInterval0) < dblTolerance)
                {
                    boolAddRemain = true;
                }
                else
                {
                    boolAddRemain = false;
                }
            }

            switch (intSEGs)
            {
                case 0:
                    break;               //do nothing
                case 1:
                    if (!boolAddRemain)
                    {
                        varpoi.Add(new POI { Station = Math.roundDown3(varpoi[i - 1].Station + dblInterval0), Desc0 = "", ClassObj = fStake.ClassObj });
                    }
                    break;

                default:
                    varpoi.Add(new POI { Station = Math.roundDown3(varpoi[i - 1].Station + dblInterval0), Desc0 = "", ClassObj = fStake.ClassObj });

                    for (int j = 2; j < intSEGs; j++)
                    {
                        varpoi.Add(new POI { Station = Math.roundDown3(varpoi[varpoi.Count - 1].Station + dblInterval0), Desc0 = "", ClassObj = fStake.ClassObj });
                    }

                    if (!boolAddRemain)//if booAddRemain is true then skip adding last point
                    {
                        varpoi.Add(new POI { Station = Math.roundDown3(varpoi[varpoi.Count - 1].Station + dblInterval0), Desc0 = "", ClassObj = fStake.ClassObj });
                    }

                    break;
            }
        }

        public static void
        getStationsOnTangentEVENS(ref int i, List<POI> varpoi, double dblInterval0, double dblTolerance)
        {
            double dblDistance = (varpoi[i + 0].Station - varpoi[i - 1].Station);

            int j = 1;
            int intervalX = 0;

            do
            {
                if (dblDistance / j < dblInterval0 + dblTolerance)
                {
                    intervalX = j;
                    break;
                }
                else
                {
                    j += 1;
                }
            } while (true);

            switch (intervalX)
            {
                case 0:
                case 1:
                    break;                //do nothing
                default:
                    varpoi.Add(new POI { Station = Math.roundDown3(varpoi[i - 1].Station + dblDistance / intervalX), Desc0 = "", ClassObj = fStake.ClassObj });

                    for (j = 2; j <= intervalX - 1; j++)
                    {
                        varpoi.Add(new POI { Station = Math.roundDown3(varpoi[varpoi.Count - 1].Station + dblDistance / intervalX), Desc0 = "", ClassObj = fStake.ClassObj });
                    }
                    break;
            }
        }

        public static double
        getNominalStation(double dblStaBeg, double dblInterval0)
        {
            if (dblStaBeg % dblInterval0 != 0)
            {
                double dblRemainder = System.Math.Truncate(dblStaBeg / dblInterval0) + 1 - (dblStaBeg / dblInterval0);

                return dblStaBeg + dblRemainder * dblInterval0;
            }
            else
            {
                return dblStaBeg;
            }
        }
    }
}