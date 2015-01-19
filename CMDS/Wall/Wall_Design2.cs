
namespace Wall
{
    public static class Wall_Design2
    {
        //List<WALLPOI> wpf = new List<WALLPOI>();
        //for (int i = 0; i < wallPoi.Count - 1; i++) {
        //    if (wallPoi[i].Sta == 0) {
        //        switch [i] {
        //            case 0:
        //                if (wallPoi[i + 1].Sta != 0) {
        //                    wpf.Add(wallPoi[i]);
        //                }

        //                break;
        //            case wallPoi.Count - 1:
        //                if (wallPoi[i - 1].Sta != 0) {
        //                    wpf.Add(wallPoi[i]);
        //                    break;
        //                }

        //                break;
        //            default:
        //                if (wallPoi[i - 1].Sta == 0 & wallPoi[i + 1].Sta == 0) {
        //                    wpf.Add(wallPoi[i]);
        //                } else if (wallPoi[i - 1].Sta != 0 & wallPoi[i + 1].Sta != 0) {
        //                    wPOI = wpf[i];
        //                    wPOI.Sta = -999.99;
        //                    wpf.Add(wPOI);
        //                } else {
        //                    int j = 0;
        //                    foreach (void algnEnt_loopVariable in algnEnts) {
        //                        algnEnt = algnEnt_loopVariable;
        //                        so1 = Geom.getStaOff(algnEnt.pnt2dBeg, algnEnt.pnt2dEnd, wallPoi[i].pnt3d);
        //                        so1.alignSegNum = j;
        //                        j = j + 1;
        //                        sOff.Add(so1);
        //                    }

        //                    dynamic soSort = from s in sOfforderby s.off ascendings;

        //                    so1 = soSort.ElementAt(0);

        //                    wPOI = wpf[i];
        //                    wPOI.Sta = so1.sta;
        //                    wPOI.AlignSegBefore = so1.alignSegNum - 1;
        //                    wPOI.AlignSegAfter = j;
        //                    wpf.Add(wPOI);
        //                }

        //                break;
        //        }
        //    } else {
        //        wpf.Add(wallPoi[i]);
        //    }
        //}

        //AlignEntData algnEntP = default(AlignEntData);
        //AlignEntData algnEntN = default(AlignEntData);

        //for (i = 0; i <= wpf.Count - 1; i++) {
        //    wPOI = wpf[i];
        //    if (wPOI.Sta == 0) {
        //        switch [i] {
        //            case 0:
        //                break;
        //            case wpf.Count - 1:
        //                break;
        //            default:
        //                for (j = 1; j <= algnEnts.Count - 1; j++) {
        //                    algnEntP = algnEnts(j - 1);
        //                    algnEntN = algnEnts(j);
        //                    distE = Geom.getPerpDistToLine(algnEntP.pnt2dBeg, algnEntP.pnt2dEnd, wPOI.pnt3d);
        //                    distB = Geom.getPerpDistToLine(algnEntN.pnt2dBeg, algnEntN.pnt2dEnd, wPOI.pnt3d);
        //                    if (distE > algnEntP.Length & distB < 0) {
        //                        wPOI.AlignSegBefore = j - 1;
        //                        wPOI.AlignSegAfter = j;
        //                        wpf[i] = wPOI;
        //                        break; 
        //                    }
        //                }

        //                break;
        //        }
        //    }
        //}

        //j = wpf.Count;
        //List<WALLPOI> wPOI_Add = new List<WALLPOI>();
        //double slope = 0;

        //for (i = 0; i <= j - 1; i++) {
        //    Point3d pnt3dCurr = wpf[i].pnt3d;
        //    Point3d pnt3dNext = Pub.pnt3dO;
        //    Point3d pnt3dPrev = Pub.pnt3dO;

        //    wPOI = wpf[i];

        //    if (wpf[i].Sta == 0) {
        //        switch [i] {

        //            case 0:
        //                //point before first point on alignment
        //                pnt3dNext = wpf[i + 1].pnt3d;
        //                dblDist2Pnts = pnt3dCurr.getDistance(pnt3dNext);
        //                slope = pnt3dCurr.getSlope(pnt3dNext);
        //                double dist0 = dblDist2Pnts - (wpf(1).Sta - objAlign.StartingStation);
        //                wPOI.Elev = pnt3dCurr.Z + (dist0) * slope;
        //                wPOI.Sta = objAlign.StartingStation;
        //                wpf[i] = wPOI;

        //                break;
        //            case wpf.Count - 1:
        //                //point after last point on alignment
        //                pnt3dPrev = wpf[i - 1].pnt3d;
        //                dblDist2Pnts = pnt3dCurr.getDistance(pnt3dPrev);
        //                slope = pnt3dPrev.getSlope(pnt3dCurr);
        //                wPOI.Elev = pnt3dPrev.Z + (objAlign.EndingStation - wpf[i - 1].Sta) * slope;
        //                wPOI.Sta = objAlign.EndingStation;
        //                wpf[i] = wPOI;

        //                break;
        //            default:
        //                pnt3dPrev = wpf[i - 1].pnt3d;
        //                pnt3dNext = wpf[i + 1].pnt3d;
        //                slope = pnt3dPrev.getSlope(pnt3dCurr);
        //                double distX = Geom.getPerpDistToLine(pnt3dPrev, pnt3dCurr, algnEnts(wPOI.AlignSegBefore).pnt2dEnd);
        //                wPOI.Sta = algnEnts(wPOI.AlignSegBefore).StaEnd;
        //                wPOI.Elev = pnt3dPrev.Z + distX * slope;
        //                wpf[i] = wPOI;

        //                wPOI = new WALLPOI();
        //                wPOI = wpf[i];
        //                wPOI.Sta = algnEnts(wPOI.AlignSegAfter).StaBeg;
        //                slope = pnt3dCurr.getSlope(pnt3dNext);
        //                distX = Geom.getPerpDistToLine(pnt3dCurr, pnt3dNext, algnEnts(wPOI.AlignSegAfter).pnt2dBeg);
        //                wPOI.Elev = pnt3dCurr.Z + distX * slope;
        //                wPOI.Sta = wPOI.Sta + 0.01;
        //                wPOI_Add.Add(wPOI);

        //                break;
        //        }
        //    }
        //}

        //foreach (WALLPOI wPOIadd in wPOI_Add) {
        //    wpf.Add(wPOIadd);
        //}

        //dynamic wPOIsort = from p in wpforderby p.Sta ascendingp;

        //List<PNT_DATA> varPNT_DATA = new List<PNT_DATA>();
        //PNT_DATA vPNT_DATA = new PNT_DATA();

        //foreach (WALLPOI s in wPOIsort) {
        //    vPNT_DATA = new PNT_DATA();
        //    vPNT_DATA.z = s.Elev;
        //    vPNT_DATA.STA = s.Sta;
        //    varPNT_DATA.Add(vPNT_DATA);
        //}

        //switch (strName) {

        //    case "DESIGN":
        //    case "BRKRIGHT":

        //        fWall.PNTSDESIGN = varPNT_DATA;

        //        break;
        //    case "EXIST":
        //    case "BRKLEFT":

        //        fWall.PNTSEXIST = varPNT_DATA;

        //        break;
        //}
    }
}
