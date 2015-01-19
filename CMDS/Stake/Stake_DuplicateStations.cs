using Base_Tools45;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Math = Base_Tools45.Math;

namespace Stake
{
    public static class Stake_DuplicateStations
    {
        public static void
        resolveDuplicateStations(ref List<POI> varpoi)
        {
            var sortSta = from p in varpoi
                          orderby p.Station ascending
                          select p;

            List<POI> poiTmp = new List<POI>();
            foreach (var p in sortSta)
                poiTmp.Add(p);

            varpoi = poiTmp;

            //BEGIN RESOLVE DUPLICATE STATIONS
            Debug.Print("BEGIN RESOLVE DUPLICATE STATIONS");

            int j = -1;
            List<POI> varPOI_Temp = new List<POI>();
            varPOI_Temp.Add(varpoi[0]);
            j++;

            Debug.Print("0" + " " + varpoi[0].Station + " " + varpoi[0].Elevation + " " + varpoi[0].Desc0 + "   " + varpoi[0].DescX);
            string strDesc = ""; int intLen = 0;

            for (int i = 1; i < varpoi.Count; i++)
            {
                if (System.Math.Abs(Math.roundDown2(varpoi[i - 1].Station) - Math.roundDown2(varpoi[i - 0].Station)) > 0.1)
                {
                    if (varpoi[i - 0].Desc0.Substring(0, 2) != "HE")
                    {
                        string descx = string.Format("{0} {1}", varpoi[i - 0].Desc0, varpoi[i - 0].DescX.Replace("HC", "").Trim());
                        varpoi[i - 0].DescX = descx;
                    }

                    varPOI_Temp.Add(varpoi[i - 0]);
                    j++;
                }
                else
                {
                    switch (varpoi[i - 1].Desc0.Substring(0, 2))//(i-1) PREVIOUS
                    {
                        case "AP": //(i-1) PREVIOUS
                            switch (varpoi[i - 0].Desc0)
                            {
                                case "TEE"://(i-0) CURRENT
                                    varpoi[i - 1].DescX = "AP/TEE";
                                    varPOI_Temp[j] = varpoi[i - 1];
                                    break;

                                case "BEG":
                                case "END"://(i-0) CURRENT
                                    varpoi[i - 0].DescX = varpoi[i - 0].Desc0;
                                    varPOI_Temp[j] = varpoi[i - 0];
                                    break;

                                case "GB":
                                case "GB LP":
                                case "GB HP"://(i-0) CURRENT

                                    //CHECK NEXT
                                    varpoi[i - 0].DescX = varpoi[i - 1].Desc0 + " " + varpoi[i - 0].Desc0;
                                    varpoi[i - 0].Desc0 = varpoi[i - 1].Desc0;
                                    varpoi[i - 0].AngDelta = varpoi[i - 1].AngDelta;
                                    varpoi[i - 0].AngDir = varpoi[i - 1].AngDir;
                                    varpoi[i - 0].isRightHand = varpoi[i - 1].isRightHand;

                                    varPOI_Temp[j] = varpoi[i - 0];

                                    break;
                            }

                            break;

                        case "BE"://BEG         '(i-1) PREVIOUS

                            switch (varpoi[i - 0].Desc0)
                            {
                                case "TEE"://(i-0) CURRENT

                                    varpoi[i - 0].DescX = "BEG/TEE";
                                    varpoi[i - 0].isClosed = varpoi[i - 1].isClosed;
                                    varPOI_Temp[j] = varpoi[i - 0];

                                    break;

                                case "GB":
                                case "GB LP":
                                case "GB HP"://(i-0) CURRENT

                                    varpoi[i - 0].DescX = varpoi[i - 1].Desc0 + " " + varpoi[i - 0].Desc0;
                                    varpoi[i - 0].isClosed = varpoi[i - 1].isClosed;
                                    varPOI_Temp[j] = varpoi[i - 0];

                                    break;
                            }

                            break;

                        case "BC"://(i-1) PREVIOUS

                            switch (varpoi[i - 0].Desc0)
                            {
                                case "GB":
                                case "GB LP":
                                case "GB HP"://(i-0) CURRENT

                                    varpoi[i - 0].DescX = varpoi[i - 1].Desc0 + " " + varpoi[i - 0].Desc0;
                                    varpoi[i - 0].Desc0 = varpoi[i - 1].Desc0;
                                    varpoi[i - 0].Radius = varpoi[i - 1].Radius;
                                    varpoi[i - 0].CenterPnt = varpoi[i - 1].CenterPnt;
                                    varPOI_Temp[j] = varpoi[i - 0];
                                    break;
                            }

                            break;

                        case "EC"://(i-1) PREVIOUS

                            switch (varpoi[i - 0].Desc0)
                            {
                                case "GB":
                                case "GB LP":
                                case "GB HP"://(i-0) CURRENT

                                    varpoi[i - 0].DescX = varpoi[i - 1].Desc0 + " " + varpoi[i - 0].Desc0;
                                    varpoi[i - 0].Desc0 = varpoi[i - 1].Desc0;

                                    varPOI_Temp[j] = varpoi[i - 0];

                                    break;

                                case "BC"://(i-0) CURRENT

                                    switch (varpoi[i - 1].isRightHand)
                                    {
                                        case true:

                                            switch (varpoi[i - 0].isRightHand)
                                            {
                                                case true:

                                                    strDesc = varpoi[i - 1].DescX;
                                                    intLen = strDesc.Length;
                                                    varpoi[i - 0].DescX = strDesc.Substring(0, intLen - 2) + "/PCC";

                                                    break;

                                                case false:

                                                    strDesc = varpoi[i - 1].DescX;
                                                    intLen = strDesc.Length;
                                                    varpoi[i - 0].DescX = strDesc.Substring(0, intLen - 2) + "/PRC";

                                                    break;
                                            }

                                            break;

                                        case false:

                                            switch (varpoi[i - 0].isRightHand)
                                            {
                                                case true:

                                                    strDesc = varpoi[i - 1].DescX;
                                                    intLen = strDesc.Length;
                                                    varpoi[i - 0].DescX = strDesc.Substring(0, intLen - 2) + "/PRC";

                                                    break;

                                                case false:

                                                    strDesc = varpoi[i - 1].DescX;
                                                    intLen = strDesc.Length;
                                                    varpoi[i - 0].DescX = strDesc.Substring(0, intLen - 2) + " PCC";

                                                    break;
                                            }

                                            break;
                                    }

                                    varPOI_Temp[j] = varpoi[i - 0];

                                    break;
                            }

                            break;

                        case "GB"://(i-1) PREVIOUS     "GB LP", "GB HP" is covered

                            switch (varpoi[i - 0].Desc0)
                            {
                                case "BC":
                                    varpoi[i - 1].DescX = varpoi[i - 0].Desc0 + " " + varpoi[i - 1].Desc0;
                                    varpoi[i - 1].Desc0 = varpoi[i - 0].Desc0;
                                    varpoi[i - 1].Radius = varpoi[i - 0].Radius;
                                    varpoi[i - 1].isRightHand = varpoi[i - 0].isRightHand;
                                    varpoi[i - 1].CenterPnt = varpoi[i - 0].CenterPnt;
                                    varPOI_Temp[j] = varpoi[i - 1];

                                    break;

                                case "EC"://(i-0) CURRENT
                                    varpoi[i - 1].DescX = varpoi[i - 0].Desc0 + " " + varpoi[i - 1].Desc0;
                                    varpoi[i - 1].Desc0 = varpoi[i - 0].Desc0;
                                    varPOI_Temp[j] = varpoi[i - 1];
                                    break;

                                case "AP"://(i-0) CURRENT

                                    varpoi[i - 0].DescX = varpoi[i - 0].Desc0 + " " + varpoi[i - 1].Desc0;
                                    varPOI_Temp[j] = varpoi[i - 0];
                                    break;
                            }
                            break;

                        case "HC"://(i-1) PREVIOUS HORIZONTAL CONTROL

                            switch (varpoi[i - 0].Desc0)
                            {
                                case "AP"://(i-0) CURRENT
                                    string descX = string.Format("{0} {1}", varpoi[i - 0].Desc0, varpoi[i - 1].DescX.Replace("HC", "").Trim());
                                    varpoi[i - 0].DescX = descX;
                                    varPOI_Temp[j] = varpoi[i - 0];

                                    break;

                                case "BEG":
                                case "END"://(i-0) CURRENT

                                    varpoi[i - 0].DescX = varpoi[i - 0].Desc0;
                                    varpoi[i - 0].isClosed = varpoi[i - 1].isClosed;
                                    varPOI_Temp[j] = varpoi[i - 0];

                                    break;

                                case "BC":
                                case "EC"://(i-0) CURRENT

                                    varpoi[i - 0].DescX = varpoi[i - 0].Desc0;
                                    varpoi[i - 0].isClosed = varpoi[i - 1].isClosed;
                                    varPOI_Temp[j] = varpoi[i - 0];

                                    break;

                                case "GB":
                                case "GB LP":
                                case "GB HP"://(i-0) CURRENT

                                    varpoi[i - 0].DescX = varpoi[i - 0].Desc0;
                                    varpoi[i - 0].isClosed = varpoi[i - 1].isClosed;
                                    varPOI_Temp[j] = varpoi[i - 0];

                                    break;
                            }

                            break;

                        case "HE":
                        case "H0"://(i-1) PREVIOUS   HORIZONTAL CONTROL WITH INTERPOLATED ELEVATION

                            switch (varpoi[i - 0].Desc0)
                            {
                                case "AP"://(i-0) CURRENT

                                    varpoi[i - 0].DescX = varpoi[i - 0].Desc0;

                                    varPOI_Temp[j] = varpoi[i - 0];

                                    break;

                                case "BEG":
                                case "END"://(i-0) CURRENT

                                    varpoi[i - 0].DescX = varpoi[i - 0].Desc0;
                                    varpoi[i - 0].isClosed = varpoi[i - 1].isClosed;

                                    varPOI_Temp[j] = varpoi[i - 0];

                                    break;

                                case "BC":
                                case "EC"://(i-0) CURRENT

                                    varpoi[i - 0].DescX = varpoi[i - 0].Desc0 + " " + varpoi[i - 1].DescX.Replace("HE", "").Trim();
                                    varpoi[i - 0].isClosed = varpoi[i - 1].isClosed;

                                    varPOI_Temp[j] = varpoi[i - 0];

                                    break;

                                case "GB":
                                case "GB LP":
                                case "GB HP"://(i-0) CURRENT

                                    varpoi[i - 0].DescX = varpoi[i - 0].Desc0;
                                    varpoi[i - 0].isClosed = varpoi[i - 1].isClosed;

                                    varPOI_Temp[j] = varpoi[i - 0];

                                    break;
                            }

                            break;

                        case "TE":
                            //(i-1) PREVIOUS   TEE

                            switch (varpoi[i - 0].Desc0)
                            {
                                case "AP":
                                    //(i-0) CURRENT

                                    varpoi[i - 0].DescX = "AP/TEE";

                                    varPOI_Temp[j] = varpoi[i - 0];

                                    break;

                                case "BEG":
                                case "END":
                                    //(i-0) CURRENT

                                    varpoi[i - 0].DescX = varpoi[i - 0].Desc0 + "/TEE";

                                    varPOI_Temp[j] = varpoi[i - 0];

                                    break;
                            }

                            break;

                        default:

                            varpoi[i - 0].DescX = varpoi[i - 0].Desc0;

                            varPOI_Temp[j] = varpoi[i - 0];

                            break;
                    }
                }

                Debug.Print(j + " " + varPOI_Temp[j].Station + " " + varPOI_Temp[j].Elevation + " " + varPOI_Temp[j].Desc0 + "   " + varPOI_Temp[j].DescX);
            }

            varpoi = varPOI_Temp;

            Debug.Print("END RESOLVE DUPLICATE STATIONS");
            //END RESOLVE DUPLICATE STATIONS
        }

        public static void
        resolveDuplicateStationsWall(ref List<POI> varpoi)
        {
            List<POI> varPOI_IN = null;

            //BEGIN RESOLVE DUPLICATE STATIONS WALL

            int j = -1;
            List<POI> varPOI_Temp = null;
            varPOI_Temp.Add(varpoi[0]);
            j++;

            int k = -1;

            Debug.Print("BEGIN RESOLVE DUPLICATE STATIONS WALL");
            Debug.Print("0" + " " + varpoi[0].Station + " " + varpoi[0].Elevation + " " + varpoi[0].Desc0 + "   " + varpoi[0].DescX);

            for (int i = 1; i < varpoi.Count; i++)
            {
                if (System.Math.Abs(Math.roundDown2(varpoi[i - 1].Station) - Math.roundDown2(varpoi[i - 0].Station)) > 0.1)
                {
                    varPOI_Temp.Add(varpoi[i - 0]);
                    j++;
                }
                else
                {
                    string strDescPrev = varpoi[i - 1].Desc0.Substring(0, 2);
                    string strDescCurr = varpoi[i - 0].Desc0.Substring(0, 2);

                    switch (strDescPrev)
                    {
                        case "HC":
                            switch (strDescCurr)
                            {
                                case "BE":
                                case "EN":

                                    varpoi[i - 0].DescX = varpoi[i - 0].Desc0;
                                    varPOI_Temp[j] = varpoi[i - 0];

                                    break;

                                case "AP":

                                    varpoi[i - 0].DescX = varpoi[i - 1].DescX;
                                    varPOI_Temp[j] = varpoi[i - 0];

                                    break;

                                default:

                                    varpoi[i - 0].DescX = varpoi[i - 0].Desc0;
                                    varPOI_Temp[j] = varpoi[i - 0];

                                    break;
                            }

                            break;

                        case "AP":

                            switch (strDescCurr)
                            {
                                case "TOF":

                                    varpoi[i - 1].DescX = varpoi[i - 1].Desc0 + " " + varpoi[i - 0].Desc0;
                                    varpoi[i - 1].Elevation = varpoi[i - 0].Elevation;
                                    varPOI_Temp[j] = varpoi[i - 1];

                                    break;

                                case "TOW":

                                    varpoi[i - 1].DescX = varpoi[i - 0].DescX;
                                    varPOI_Temp[j] = varpoi[i - 1];

                                    break;
                            }

                            break;

                        case "TOF":

                            switch (strDescCurr)
                            {
                                case "AP":

                                    varpoi[i - 1].DescX = varpoi[i - 0].DescX;
                                    varPOI_Temp[j] = varpoi[i - 1];

                                    break;

                                case "TOW":

                                    varpoi[i - 1].DescX = varpoi[i - 0].DescX;
                                    varPOI_Temp[j] = varpoi[i - 1];

                                    break;
                            }

                            break;

                        case "TOW":

                            switch (strDescCurr)
                            {
                                case "AP":

                                    varpoi[i - 1].DescX = varpoi[i - 0].DescX;
                                    varPOI_Temp[j] = varpoi[i - 1];

                                    break;

                                case "TOF":

                                    varpoi[i - 1].DescX = varpoi[i - 0].DescX;
                                    varPOI_Temp[j] = varpoi[i - 1];

                                    break;
                            }

                            break;
                    }

                    Debug.Print(j + " " + varPOI_Temp[j].Station + " " + varPOI_Temp[j].Elevation + " " + varPOI_Temp[j].Desc0 + "   " + varPOI_Temp[j].DescX);
                }
            }

            varPOI_Temp[j].DescX = varPOI_Temp[0].DescX;

            varpoi = new List<POI>();

            for (int i = 0; i <= j; i++)
            {
                varpoi.Add(varPOI_Temp[i]);
            }

            for (int i = j + 1; i <= j + k + 1; i++)
            {
                varpoi.Add(varPOI_IN[i - j - 1]);
            }

            Debug.Print("END RESOLVE DUPLICATE STATIONS WALL");
            //END RESOLVE DUPLICATE STATIONS WALL
        }

        public static void
        resolveDuplicateStationsGrid(List<POI> varpoi)
        {
            //BEGIN RESOLVE DUPLICATE STATIONS GRID

            List<POI> varPOI_LIM = new List<POI>();
            List<POI> varPOI_IN = new List<POI>();
            int k = -1;
            for (int i = 0; i < varpoi.Count; i++)
            {
                if (varpoi[i].OFFSET == 0)
                {
                    varPOI_LIM.Add(varpoi[i]);
                }
                else
                {
                    varPOI_IN.Add(varpoi[i]);
                    k++;
                }
            }

            Debug.Print("BEGIN RESOLVE DUPLICATE STATIONS GRID");

            int j = -1;
            List<POI> varPOI_Temp = new List<POI>();
            varPOI_Temp.Add(varPOI_LIM[0]);
            j++;

            Debug.Print("0" + " " + varPOI_Temp[0].Station + " " + varPOI_Temp[0].Elevation + " " + varPOI_Temp[0].Desc0 + "   " + varPOI_Temp[0].DescX);

            for (int i = 1; i < varPOI_LIM.Count; i++)
            {
                if (System.Math.Abs(Math.roundDown2(varPOI_LIM[i - 1].Station) - Math.roundDown2(varPOI_LIM[i - 0].Station)) > 0.1)
                {
                    varPOI_Temp.Add(varPOI_LIM[i - 0]);
                    j++;
                }
                else if (varPOI_LIM[i - 1].Desc0.Substring(0, 2) == "AP")
                {
                    varPOI_LIM[i - 1].DescX = varPOI_LIM[i - 0].DescX;
                    varPOI_Temp[j] = varPOI_LIM[i - 1];
                }
                else if (varPOI_LIM[i - 0].Desc0.Substring(0, 2) == "AP")
                {
                    varPOI_LIM[i - 0].DescX = varPOI_LIM[i - 1].DescX;
                    varPOI_Temp[j] = varPOI_LIM[i - 0];
                }

                Debug.Print(j + " " + varPOI_Temp[j].Station + " " + varPOI_Temp[j].Elevation + " " + varPOI_Temp[j].Desc0 + "   " + varPOI_Temp[j].DescX);
            }

            varPOI_Temp[j].DescX = varPOI_Temp[0].DescX;

            varpoi = new List<POI>();

            for (int i = 0; i <= j; i++)
            {
                varpoi.Add(varPOI_Temp[i]);
            }

            for (int i = j + 1; i <= j + k + 1; i++)
            {
                varpoi[i] = varPOI_IN[i - j - 1];
            }

            Debug.Print("END RESOLVE DUPLICATE STATIONS GRID");
            //END RESOLVE DUPLICATE STATIONS GRID
        }
    }
}