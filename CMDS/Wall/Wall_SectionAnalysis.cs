using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Diagnostics;
using p = Wall.Wall_Public;
using wd = Wall.Wall_Design;

namespace Wall
{
    public static class Wall_SectionAnalysis
    {
        static Wall_Form.frmWall1 fWall1 = Wall_Forms.wForms.fWall1;

        public static SECTIONDATASET
        wallSectionAnalysis(double dblStationRF){
            
            Alignment objAlignPL = fWall1.AlignPL;
            Alignment objAlignRF = fWall1.AlignRF;

            EXGROUND vEXGROUND = new EXGROUND();
            SECTIONDATASET vSDS = new SECTIONDATASET();

            Profile objProfilePL = Prof.getProfile(objAlignPL.ObjectId, "EXIST");
            Profile objProfileRF = Prof.getProfile(objAlignRF.ObjectId, "CPNT");

            Point3d pnt3dBeg = default(Point3d);
            Point3d pnt3dEnd = default(Point3d);

            int intSign = fWall1.SIDE;

            int n = 0;

            double dblElevRF = 0;
            double dblElevFL = 0;
            double dblElevTC = 0;
            double dblElevB1 = 0;
            double dblElevTOE = 0;
            double dblElevTOP = 0;
            double dblElevPL = 0;
            double dblElevW0 = 0;
            double dblElevW1 = 0;
            double dblElevW2 = 0;
            double dblElevW3 = 0;
            double dblElevW4 = 0;
            double dblElevW5 = 0;
            double dblElevW6 = 0;
            double dblElevW7 = 0;
            double dblElevW8 = 0;
            double dblElevW9 = 0;
            double dblElevG0 = 0;
            double dblElevGB = 0;

            double dblElevTC_REVISED = 0;
            double dblDeltaTC = 0;
            double dblWallHeight = 0;

            double dblElevPL_REVISED = 0;
            double dblDeltaPL = 0;

            FEATUREDATA fDataRF = default(FEATUREDATA);
            FEATUREDATA fDataFL = default(FEATUREDATA);
            FEATUREDATA fDataTC = default(FEATUREDATA);
            FEATUREDATA fDataB1 = default(FEATUREDATA);
            FEATUREDATA fDataTOE = default(FEATUREDATA);
            FEATUREDATA fDataTOP = default(FEATUREDATA);
            FEATUREDATA fDataPL = default(FEATUREDATA);
            FEATUREDATA fDataW0 = default(FEATUREDATA);
            FEATUREDATA fDataW1 = default(FEATUREDATA);
            FEATUREDATA fDataW2 = default(FEATUREDATA);
            FEATUREDATA fDataW3 = default(FEATUREDATA);
            FEATUREDATA fDataW4 = default(FEATUREDATA);
            FEATUREDATA fDataW5 = default(FEATUREDATA);
            FEATUREDATA fDataW6 = default(FEATUREDATA);
            FEATUREDATA fDataW7 = default(FEATUREDATA);
            FEATUREDATA fDataW8 = default(FEATUREDATA);
            FEATUREDATA fDataW9 = default(FEATUREDATA);
            FEATUREDATA fDataG0 = default(FEATUREDATA);
            FEATUREDATA fDataGB = default(FEATUREDATA);

            fDataRF = new FEATUREDATA();
            fDataFL = new FEATUREDATA();
            fDataTC = new FEATUREDATA();
            fDataB1 = new FEATUREDATA();
            fDataTOE = new FEATUREDATA();
            fDataTOP = new FEATUREDATA();
            fDataPL = new FEATUREDATA();
            fDataW0 = new FEATUREDATA();
            fDataW1 = new FEATUREDATA();
            fDataW2 = new FEATUREDATA();
            fDataW3 = new FEATUREDATA();
            fDataW4 = new FEATUREDATA();
            fDataW5 = new FEATUREDATA();
            fDataW6 = new FEATUREDATA();
            fDataW7 = new FEATUREDATA();
            fDataW8 = new FEATUREDATA();
            fDataW9 = new FEATUREDATA();
            fDataG0 = new FEATUREDATA();
            fDataGB = new FEATUREDATA();

            double Xo = 0;

            double X = 0;
            double XT = 0;
            double X1 = 0;
            double X2 = 0;
            double SGx = 0;

            double X0 = float.Parse(fWall1.tbx2d_X0.Text);
            double S0 = float.Parse(fWall1.tbx2d_S0.Text);

            double CF = float.Parse(fWall1.tbx2d_CF.Text);

            double B1 = float.Parse(fWall1.tbx2d_B1.Text);
            double S1 = float.Parse(fWall1.tbx2d_S1.Text);

            double WT = float.Parse(fWall1.tbx2d_WT.Text);
            double FB = float.Parse(fWall1.tbx2d_FB.Text);
            double GW = float.Parse(fWall1.tbx2d_GW.Text);

            double B2 = float.Parse(fWall1.tbx2d_B2.Text);
            double S2 = float.Parse(fWall1.tbx2d_S2.Text);

            double SG = float.Parse(fWall1.tbx2d_SG.Text);

            double B3 = float.Parse(fWall1.tbx2d_B3.Text);
            double S3 = float.Parse(fWall1.tbx2d_S3.Text);

            double dblOffsetPL = 0.0;
            double dblOffsetRF = 0.0;

            double[] dblOffsetsEX = new double[6];

            double dblEasting = 0.0;
            double dblNorthing = 0.0;
            double dblStationPL = 0.0;


            try
            {
                vSDS.STA = (float)dblStationRF;
                objAlignRF.DistanceToAlignment(dblStationRF, objAlignPL, ref dblOffsetRF, ref dblStationPL);
                if (dblStationPL == 0)
                {
                    //do something
                }

                //Dim dblStationPL As Double = getStationTargetAlign(dblStationRF, dblOffsetRF, objAlignRF, objAlignPL)

                X = (dblOffsetRF * intSign);
                vSDS.X = (float)X;

                XT = X - X0;
                //distance from TC perp to PL
                if (XT < 0)
                {
                    XT = XT * -1;
                    X0 = System.Math.Truncate(X - XT);
                    //recalc X0 to fit
                    XT = X - X0;
                }

                try
                {
                    dblElevPL = objProfilePL.ElevationAt(dblStationPL);
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }

                dblElevRF = objProfileRF.ElevationAt(dblStationRF);
                //elevation on RF at current RF station
                if (dblElevRF <= 0 & dblElevPL <= 0)
                {
                    vSDS.RESOLVED = false;
                    return vSDS;
                }

                //-------------------------------------------------------------------------------------------------------------------
                //RF, FL,TC

                fDataRF.Station = (float)dblStationRF;
                fDataRF.Offset = 0.0f;
                fDataRF.Elev = (float)dblElevRF;
                fDataRF.AlignName = "RF";
                if (p.RF.ContainsKey((float)dblStationRF))
                {
                    p.RF.Remove((float)dblStationRF);
                }
                p.RF.Add((float)dblStationRF, fDataRF);
                vSDS.RF.OFFSET = fDataRF.Offset;
                vSDS.RF.ELEV = fDataRF.Elev;

                dblElevFL = dblElevRF + X0 * S0;

                fDataFL.Station = (float)dblStationRF;
                fDataFL.Offset = (float)X0 - 0.15f;
                fDataFL.Elev = (float)dblElevFL;
                fDataFL.AlignName = "RF";
                if (p.FL.ContainsKey((float)dblStationRF))
                {
                    p.FL.Remove((float)dblStationRF);
                }
                p.FL.Add((float)dblStationRF, fDataFL);
                vSDS.FL.OFFSET = fDataFL.Offset;
                vSDS.FL.ELEV = fDataFL.Elev;

                dblElevTC = dblElevFL + CF / 12;

                fDataTC.Station = (float)dblStationRF;
                fDataTC.Offset = (float)X0;
                fDataTC.Elev = (float)dblElevTC;
                fDataTC.AlignName = "RF";
                if (p.TC.ContainsKey((float)dblStationRF))
                {
                    p.TC.Remove((float)dblStationRF);
                }
                p.TC.Add((float)dblStationRF, fDataTC);
                vSDS.TC.OFFSET = fDataTC.Offset;
                vSDS.TC.ELEV = fDataTC.Elev;

                objAlignRF.PointLocation(dblStationRF, 0, ref dblEasting, ref dblNorthing); //valid result                

                try
                {
                    objAlignPL.StationOffset(dblEasting, dblNorthing, ref dblStationPL, ref dblOffsetPL);
                    //chance of invalid result if RF goes beyond PL in either direction
                }
                catch (Autodesk.Civil.PointNotOnEntityException )
                {
                    dblStationPL = 0.0;
                }

                //***************************       ???????????????????????????????????              *********************************
                if (dblStationPL == 0)
                {

                    //dblStationPL = objAlignPL.EndingStation
                    //pnt3dBeg.X = dblEasting : pnt3dBeg.Y = dblNorthing : pnt3dBeg.Z = 0.0

                    pnt3dBeg = new Point3d(dblEasting, dblNorthing, 0.0);
                    pnt3dEnd = objAlignPL.EndPoint;

                    dblOffsetPL = pnt3dBeg.getDistance(pnt3dEnd);

                }


                if (dblElevTC + XT * S1 < dblElevPL)
                {
                    SG = System.Math.Abs(SG);
                    X1 = ((dblElevTC - dblElevPL) + (XT - B3) * SG + (B3 * S3)) / (SG - S1);
                    //distance from TC to TOE


                }
                else if (dblElevTC + XT * S1 > dblElevPL)
                {
                    SG = System.Math.Abs(SG);
                    SG = SG * -1;
                    X1 = ((dblElevTC - dblElevPL) + (XT - B3) * SG + (B3 * S3)) / (SG - S1);
                    //distance from TC to TOE


                }
                else if (dblElevTC + XT * S1 == dblElevPL)
                {
                    X1 = XT - B1 - B3;
                    X2 = 0;

                }

                if (X1 >= 0 & X1 < B1)
                {
                    Debug.Print(dblStationRF.ToString());
                    //X1 = B1

                    //TC IS HIGHER SLOPING DOWN TO PL
                    if (SG < 0)
                    {

                        X2 = XT - B1 - 8 / 12 - B3;
                        //X2 FOR  GRAVITY WALL AT PL UP TO 2.5 FEET RETAINED


                        if (X2 > 0)
                        {
                            dblElevPL_REVISED = dblElevTC + (B1 * S1) + (X2 * SG) + B3 * S3;
                            dblDeltaPL = dblElevPL_REVISED - dblElevPL;


                            if (dblDeltaPL <= 2.5)
                            {
                                dblWallHeight = dblDeltaPL * -1;

                                if (XT >= B1 + 8 / 12 + B3)
                                {
                                    B2 = 0;
                                    GW = 0;
                                }
                                else
                                {
                                    vSDS.dYh = 2.18f;
                                    vSDS.RESOLVED = false;
                                    vSDS.XT = (float)XT;
                                    return vSDS;
                                }

                            }


                        }
                        else
                        {
                            vSDS.RESOLVED = false;
                            vSDS.XT = (float)XT;
                            return vSDS;

                        }

                        // TC IS LOWER - CHECK IF TALLER CURB WORKS - CASE B OR IF GRAVITY WALL WORKS
                    }
                    else if (SG > 0)
                    {

                        X2 = XT - B1 - B3;

                        if (X2 > 0)
                        {
                            dblElevTC_REVISED = dblElevPL - (B3 * S3) - (X2 * SG) - (X1 * S1);
                            //INCLUDE SLOPE AND BOTH BENCHES TO TEST FOR TALLER CURB
                            dblDeltaTC = dblElevTC_REVISED - dblElevTC;


                            if (dblDeltaTC > 0)
                            {
                                if(dblDeltaTC < 0.17){
                                         dblElevTC = dblElevTC + 0.17;
                                        vSDS.TC.ELEV = (float)dblElevTC;
                                        X2 = XT - B1 - B3;
                                        B2 = 0;
                                        GW = 0;
                                   
                                }else if (dblDeltaTC < 0.34){
                                        dblElevTC = dblElevTC + 0.34;
                                        vSDS.TC.ELEV = (float)dblElevTC;
                                        X2 = XT - B1 - B3;
                                        B2 = 0;
                                        GW = 0;
                                    
                                }else if(dblDeltaTC < 0.50){
                                        dblElevTC = dblElevTC + 0.5;
                                        vSDS.TC.ELEV = (float)dblElevTC;
                                        X2 = XT - B1 - B3;
                                        B2 = 0;
                                        GW = 0;
                                    
                                }else{
                                        //CHECK IF GRAVITY WALL WILL WORK WITH REVISED X2

                                        X2 = System.Math.Round(XT - B1 - 8 / 12 - B3, 3);

                                        if (X2 > 0)
                                        {
                                            dblElevTC_REVISED = dblElevPL - (B3 * S3) - (X2 * SG) - (X1 * S1);
                                            //INCLUDE SLOPE AND BOTH BENCHES TO TEST FOR TALLER CURB
                                            dblDeltaTC = dblElevTC_REVISED - dblElevTC;


                                            if (dblDeltaTC <= 2.5)
                                            {
                                                dblWallHeight = dblDeltaTC;


                                                if (System.Math.Round(XT, 3) >= System.Math.Round(B1 + 8 / 12 + X2 + B3, 3) - 0.01)
                                                {
                                                    B2 = 0;
                                                    GW = 0;


                                                }
                                                else
                                                {
                                                    vSDS.dYh = 2.18f;
                                                    vSDS.RESOLVED = false;
                                                    vSDS.XT = (float)XT;
                                                    return vSDS;

                                                }

                                            }


                                        }
                                        else
                                        {
                                            vSDS.dYh = 2.18f;
                                            vSDS.RESOLVED = false;
                                            vSDS.XT = (float)XT;
                                            return vSDS;

                                        }

                                    
                                }
                            }
                            else if (dblDeltaTC < 0)
                            {
                                X2 = 0;
                                X1 = XT - B3;
                                B2 = 0;
                                GW = 0;
                                S1 = (dblElevPL - dblElevTC) / (X - X0);
                                S3 = S1;

                            }

                        }

                    }

                }

                //SLOPE FITS WITHOUT ENCROACHINT INTO B1
                if (X1 >= 0 & dblWallHeight == 0)
                {

                    if (X1 > B1)
                    {
                        X2 = XT - X1 - B3;
                    }
                    else
                    {
                        X2 = XT - B1 - B3;
                    }

                    fWall1.WallStart = false;

                    dblElevB1 = dblElevTC + (B1 * S1);

                    fDataB1.Station = (float)dblStationRF;
                    fDataB1.Offset = (float)(X0 + B1);
                    fDataB1.Elev = (float)dblElevB1;
                    fDataB1.AlignName = "RF";
                    if (p.BB1.ContainsKey((float)dblStationRF))
                    {
                        p.BB1.Remove((float)dblStationRF);
                    }
                    p.BB1.Add((float)dblStationRF, fDataB1);
                    vSDS.BB1.OFFSET = fDataB1.Offset;
                    vSDS.BB1.ELEV = fDataB1.Elev;


                    if (SG > 0)
                    {
                        if (X1 > B1)
                        {
                            dblElevTOE = dblElevTC + (B1 * S1) + ((X1 - B1) * S1);
                        }
                        else
                        {
                            dblElevTOE = dblElevTC + (B1 * S1);
                        }

                        fDataTOE.Station = (float)dblStationPL;
                        fDataTOE.Offset = (float)(X2 + B3) * intSign;
                        fDataTOE.Elev = (float)dblElevTOE;
                        fDataTOE.AlignName = "PL";
                        if (p.TOE.ContainsKey((float)dblStationPL))
                        {
                            p.TOE.Remove((float)dblStationPL);
                        }
                        p.TOE.Add((float)dblStationPL, fDataTOE);
                        vSDS.TOE.OFFSET = (float)(X - fDataTOE.Offset) * intSign;
                        vSDS.TOE.ELEV = fDataTOE.Elev;

                        dblElevTOP = dblElevTOE + (X2 * SG);

                        fDataTOP.Station = (float)dblStationPL;
                        fDataTOP.Offset = (float)B3 * intSign;
                        fDataTOP.Elev = (float)dblElevTOP;
                        fDataTOP.AlignName = "PL";
                        if (p.TOP.ContainsKey((float)dblStationPL))
                        {
                            p.TOP.Remove((float)dblStationPL);
                        }
                        p.TOP.Add((float)dblStationPL, fDataTOP);
                        vSDS.TOP.OFFSET = (float)(X - fDataTOP.Offset) * intSign;
                        vSDS.TOP.ELEV = fDataTOP.Elev;

                        //dblElevPL = dblElevPL;

                        fDataPL.Station = (float)dblStationPL;
                        fDataPL.Offset = 0.0f * intSign;
                        fDataPL.Elev = (float)dblElevPL;
                        fDataPL.AlignName = "PL";
                        if (p.PL.ContainsKey((float)dblStationPL))
                        {
                            p.PL.Remove((float)dblStationPL);
                        }
                        p.PL.Add((float)dblStationPL, fDataPL);
                        vSDS.PL.OFFSET = (float)(X - fDataPL.Offset) * intSign;
                        vSDS.PL.ELEV = fDataPL.Elev;

                        dblOffsetsEX[0]  = 0.0;
                        //RF
                        dblOffsetsEX[1] = dblOffsetsEX[0] + X0;
                        //TC
                        dblOffsetsEX[2] = dblOffsetsEX[1] + X1;
                        //TOE
                        dblOffsetsEX[3] = dblOffsetsEX[2] + X2;
                        //TOP
                        dblOffsetsEX[4] = dblOffsetsEX[3] + B3;
                        //PL
                        dblOffsetsEX[5] = dblOffsetsEX[4] + 5;
                        //Off5


                    }
                    else if (SG < 0)
                    {
                        dblElevTOP = dblElevB1 + (X1 - B1) * S1;

                        fDataTOP.Station = (float)dblStationRF;
                        fDataTOP.Offset = (float)(X0 + B1 + (X1 - B1));
                        fDataTOP.Elev = (float)dblElevTOP;
                        fDataTOP.AlignName = "RF";
                        if (p.TOP.ContainsKey((float)dblStationRF))
                        {
                            p.TOP.Remove((float)dblStationRF);
                        }
                        p.TOP.Add((float)dblStationRF, fDataTOP);
                        vSDS.TOP.OFFSET = fDataTOP.Offset;
                        vSDS.TOP.ELEV = fDataTOP.Elev;

                        dblElevTOE = dblElevTOP + (X2 * SG);

                        fDataTOE.Station = (float)dblStationRF;
                        fDataTOE.Offset = (float)(X0 + X1 + X2);
                        fDataTOE.Elev = (float)dblElevTOE;
                        fDataTOE.AlignName = "RF";
                        if (p.TOE.ContainsKey((float)dblStationRF))
                        {
                            p.TOE.Remove((float)dblStationRF);
                        }
                        p.TOE.Add((float)dblStationRF, fDataTOE);
                        vSDS.TOE.OFFSET = fDataTOE.Offset;
                        vSDS.TOE.ELEV = fDataTOE.Elev;

                        //dblElevPL = dblElevPL;

                        fDataPL.Station = (float)dblStationPL;
                        fDataPL.Offset = 0.0f * intSign;
                        fDataPL.Elev = (float)dblElevPL;
                        fDataPL.AlignName = "PL";
                        if (p.PL.ContainsKey((float)dblStationPL))
                        {
                            p.PL.Remove((float)dblStationPL);
                        }
                        p.PL.Add((float)dblStationPL, fDataPL);
                        vSDS.PL.OFFSET = (float)(X - fDataPL.Offset) * intSign;
                        vSDS.PL.ELEV = fDataPL.Elev;

                        dblOffsetsEX[0] = 0.0;
                        //RF
                        dblOffsetsEX[1] = dblOffsetsEX[0] + X0;
                        //TC
                        dblOffsetsEX[2] = dblOffsetsEX[1] + X1;
                        //TOP
                        dblOffsetsEX[3] = dblOffsetsEX[2] + X2;
                        //TOE
                        dblOffsetsEX[4] = dblOffsetsEX[3] + B3;
                        //PL
                        dblOffsetsEX[5] = dblOffsetsEX[4] + 5;
                        //Off5

                    }

                    vEXGROUND = wd.getExGround(fWall1.SurfaceEXIST, objAlignRF, dblStationRF, dblOffsetsEX);

                    vSDS.dYh = 0;
                    vSDS.EX = vEXGROUND;
                    vSDS.SG = (float)SG;
                    vSDS.X = (float)X;
                    vSDS.X0 = (float)X0;
                    vSDS.X1 = (float)X1;
                    vSDS.X2 = (float)X2;
                    vSDS.XT = (float)XT;
                    vSDS.STA = (float)System.Math.Round(dblStationRF, 3);
                    vSDS.RESOLVED = true;

                    fWall1.SDS = vSDS;

                    return vSDS;

                    //SLOPE DOESN'T WORK
                }
                else
                {

                    X1 = B1;

                    //X1 ENCROACHES INTO B1 AND SLOPING UP  --> NEED WALL AT TC
                    if (SG > 0)
                    {

                        if (fWall1.WallStart == false)
                        {
                            n = fWall1.WALLNO + 1;
                            fWall1.WALLNO = n;

                            //dWALLs.Add(n, dWALL)
                            //dGUTs.Add(n, dGUT)
                            fWall1.WallStart = true;

                        }

                        if (dblWallHeight > 0)
                        {
                            X2 = XT - (B1 + 8 / 12 + B3);
                        }
                        else
                        {
                            if (XT >= B1 + WT / 12 + GW + B2 + B3)
                            {
                                X2 = XT - (B1 + WT / 12 + GW + B2 + B3);
                                //slope width (run)
                            }
                            else
                            {
                                vSDS.dYh = 2.18f;
                                vSDS.RESOLVED = false;
                                return vSDS;
                            }
                        }

                        dblElevW0 = dblElevTC + (B1 * S1);

                        fDataW0.Station = (float)dblStationRF;
                        fDataW0.Offset = (float)(X0 + B1);
                        fDataW0.Elev = (float)dblElevW0;
                        fDataW0.AlignName = "RF";
                        if (p.W0.ContainsKey((float)dblStationRF))
                        {
                            p.W0.Remove((float)dblStationRF);
                        }
                        p.W0.Add((float)dblStationRF, fDataW0);
                        //starts and stops with wall
                        vSDS.W0.OFFSET = fDataW0.Offset;
                        vSDS.W0.ELEV = fDataW0.Elev;

                        //START AT TOP AND WORK DOWN

                        //dblElevPL = dblElevPL;
                        //PL

                        fDataPL.Station = (float)dblStationPL;
                        fDataPL.Offset = 0.0f * intSign;
                        fDataPL.Elev = (float)dblElevPL;
                        fDataPL.AlignName = "PL";
                        if (p.PL.ContainsKey((float)dblStationPL))
                        {
                            p.PL.Remove((float)dblStationPL);
                        }
                        p.PL.Add((float)dblStationPL, fDataPL);
                        vSDS.PL.OFFSET = (float)(X - fDataPL.Offset) * intSign;
                        vSDS.PL.ELEV = fDataPL.Elev;

                        dblElevTOP = dblElevPL - (B3 * S3);
                        //TOP

                        fDataTOP.Station = (float)dblStationPL;
                        fDataTOP.Offset = (float)B3 * intSign;
                        fDataTOP.Elev = (float)dblElevTOP;
                        fDataTOP.AlignName = "PL";
                        if (p.TOP.ContainsKey((float)dblStationPL))
                        {
                            p.TOP.Remove((float)dblStationPL);
                        }
                        p.TOP.Add((float)dblStationPL, fDataTOP);
                        vSDS.TOP.OFFSET = (float)(X - fDataTOP.Offset) * intSign;
                        vSDS.TOP.ELEV = fDataTOP.Elev;

                        dblElevTOE = dblElevTOP - (X2 * SG);
                        //SG > 0                     'TOE

                        fDataTOE.Station = (float)dblStationPL;
                        fDataTOE.Offset = (float)(B3 + X2) * intSign;
                        fDataTOE.Elev = (float)dblElevTOE;
                        fDataTOE.AlignName = "PL";
                        if (p.TOE.ContainsKey((float)dblStationPL))
                        {
                            p.TOE.Remove((float)dblStationPL);
                        }
                        p.TOE.Add((float)dblStationPL, fDataTOE);
                        vSDS.TOE.OFFSET = (float)(X - fDataTOE.Offset) * intSign;
                        vSDS.TOE.ELEV = fDataTOE.Elev;

                        dblElevGB = dblElevTOE - (B2 * S2);
                        //GB

                        fDataGB.Station = (float)dblStationPL;
                        fDataGB.Offset = (float)(B3 + X2 + B2) * intSign;
                        fDataGB.Elev = (float)dblElevGB;
                        fDataGB.AlignName = "PL";
                        if (p.GB.ContainsKey((float)dblStationPL))
                        {
                            p.GB.Remove((float)dblStationPL);
                        }
                        p.GB.Add((float)dblStationPL, fDataGB);
                        //starts and stops with wall
                        vSDS.GB.OFFSET = (float)(X - fDataGB.Offset) * intSign;
                        vSDS.GB.ELEV = fDataGB.Elev;

                        //G0
                        if (dblWallHeight > 0)
                        {

                            dblElevG0 = dblElevGB;
                            fDataG0.Offset = (float)(X0 + B1 + 8 / 12);
                            //BACK OF 8" THICK BLOCK WALL


                        }
                        else
                        {
                            dblElevG0 = dblElevGB - (GW / 2 * 0.5);
                            fDataG0.Offset = (float)(X0 + B1 + WT / 12 + GW / 2);

                        }

                        fDataG0.Station = (float)dblStationRF;
                        fDataG0.Elev = (float)dblElevG0;
                        fDataG0.AlignName = "RF";
                        if (p.G0.ContainsKey((float)dblStationRF))
                        {
                            p.G0.Remove((float)dblStationRF);
                        }
                        p.G0.Add((float)dblStationRF, fDataG0);
                        //starts and stops with wall
                        vSDS.G0.OFFSET = fDataG0.Offset;
                        vSDS.G0.ELEV = fDataG0.Elev;

                        //W3
                        if (dblWallHeight > 0)
                        {

                            fDataW3.Offset = (float)(X0 + B1 + 8 / 12);


                        }
                        else
                        {
                            fDataW3.Offset = (float)(X0 + B1 + WT / 12);

                        }

                        dblElevW3 = dblElevGB;

                        fDataW3.Station = (float)dblStationRF;
                        fDataW3.Elev = (float)dblElevW3;
                        fDataW3.AlignName = "RF";
                        if (p.W3.ContainsKey((float)dblStationRF))
                        {
                            p.W3.Remove((float)dblStationRF);
                        }
                        p.W3.Add((float)dblStationRF, fDataW3);
                        //starts and stops with wall
                        vSDS.W3.OFFSET = fDataW3.Offset;
                        vSDS.W3.ELEV = fDataW3.Elev;

                        dblElevW2 = dblElevW3 + FB / 12;
                        //W2

                        fDataW2.Station = (float)dblStationRF;
                        fDataW2.Offset = (float)(X0 + B1 + WT / 12 - 0.001);
                        fDataW2.Elev = (float)dblElevW2;
                        fDataW2.AlignName = "RF";
                        if (p.W2.ContainsKey((float)dblStationRF))
                        {
                            p.W2.Remove((float)dblStationRF);
                        }
                        p.W2.Add((float)dblStationRF, fDataW2);
                        //starts and stops with wall
                        vSDS.W2.OFFSET = fDataW2.Offset;
                        vSDS.W2.ELEV = fDataW2.Elev;

                        dblElevW1 = dblElevW2;
                        //W1

                        fDataW1.Station = (float)dblStationRF;
                        fDataW1.Offset = (float)(X0 + B1 + 0.001);
                        fDataW1.Elev = (float)dblElevW1;
                        fDataW1.AlignName = "RF";
                        if (p.W1.ContainsKey((float)dblStationRF))
                        {
                            p.W1.Remove((float)dblStationRF);
                        }
                        p.W1.Add((float)dblStationRF, fDataW1);
                        vSDS.W1.OFFSET = fDataW1.Offset;
                        vSDS.W1.ELEV = fDataW1.Elev;

                        //WALL FOOTING

                        dblElevW9 = dblElevW0 - 1.33;
                        //W9

                        fDataW9.Station = (float)dblStationRF;
                        fDataW9.Offset = (float)(X0 + B1 - 0.001);
                        fDataW9.Elev = (float)dblElevW9;
                        fDataW9.AlignName = "RF";
                        if (p.W9.ContainsKey((float)dblStationRF))
                        {
                            p.W9.Remove((float)dblStationRF);
                        }
                        p.W9.Add((float)dblStationRF, fDataW9);
                        //starts and stops with wall
                        vSDS.W9.OFFSET = fDataW9.Offset;
                        vSDS.W9.ELEV = fDataW9.Elev;

                        //W8
                        if (dblWallHeight > 0)
                        {
                            fDataW8.Offset = fDataW9.Offset;
                        }
                        else
                        {
                            fDataW8.Offset = (float)(X0 + B1 - 3);
                        }

                        dblElevW8 = dblElevW9;

                        fDataW8.Station = (float)dblStationRF;
                        fDataW8.Elev = (float)dblElevW8;
                        fDataW8.AlignName = "RF";
                        if (p.W8.ContainsKey((float)dblStationRF))
                        {
                            p.W8.Remove((float)dblStationRF);
                        }
                        p.W8.Add((float)dblStationRF, fDataW8);
                        //starts and stops with wall
                        vSDS.W8.OFFSET = fDataW8.Offset;
                        vSDS.W8.ELEV = fDataW8.Elev;

                        //W7
                        if (dblWallHeight > 0)
                        {
                            dblElevW7 = dblElevW8;
                            fDataW7.Offset = fDataW8.Offset;
                        }
                        else
                        {
                            dblElevW7 = dblElevW8 - 1.33;
                            fDataW7.Offset = (float)(X0 + B1 - 3.001);
                        }

                        fDataW7.Station = (float)dblStationRF;
                        fDataW7.Elev = (float)dblElevW7;
                        fDataW7.AlignName = "RF";
                        if (p.W7.ContainsKey((float)dblStationRF))
                        {
                            p.W7.Remove((float)dblStationRF);
                        }
                        p.W7.Add((float)dblStationRF, fDataW7);
                        //starts and stops with wall
                        vSDS.W7.OFFSET = fDataW7.Offset;
                        vSDS.W7.ELEV = fDataW7.Elev;

                        //W6
                        if (dblWallHeight > 0)
                        {
                            fDataW6.Offset = (float)(X0 + B1 + 8 / 12 + 0.001);
                        }
                        else
                        {
                            fDataW6.Offset = (float)(X0 + B1 + WT / 12 + 2.001);
                        }

                        dblElevW6 = dblElevW7;

                        fDataW6.Station = (float)dblStationRF;
                        fDataW6.Elev = (float)dblElevW6;
                        fDataW6.AlignName = "RF";
                        if (p.W6.ContainsKey((float)dblStationRF))
                        {
                            p.W6.Remove((float)dblStationRF);
                        }
                        p.W6.Add((float)dblStationRF, fDataW6);
                        //starts and stops with wall
                        vSDS.W6.OFFSET = fDataW6.Offset;
                        vSDS.W6.ELEV = fDataW6.Elev;

                        //W5
                        if (dblWallHeight > 0)
                        {
                            dblElevW5 = dblElevW6;
                            fDataW5.Offset = fDataW5.Offset;
                        }
                        else
                        {
                            dblElevW5 = dblElevW6 + 1.33;
                            fDataW5.Offset = (float)(X0 + B1 + WT / 12 + 2);
                        }

                        fDataW5.Station = (float)dblStationRF;
                        fDataW5.Elev = (float)dblElevW5;
                        fDataW5.AlignName = "RF";
                        if (p.W5.ContainsKey((float)dblStationRF))
                        {
                            p.W5.Remove((float)dblStationRF);
                        }
                        p.W5.Add((float)dblStationRF, fDataW5);
                        //starts and stops with wall
                        vSDS.W5.OFFSET = fDataW5.Offset;
                        vSDS.W5.ELEV = fDataW5.Elev;

                        //W4
                        if (dblWallHeight > 0)
                        {
                            fDataW4.Offset = (float)(X0 + B1 + 8 / 12 + 0.002);
                        }
                        else
                        {
                            fDataW4.Offset = (float)(X0 + B1 + WT / 12 + 0.002);
                        }

                        dblElevW4 = dblElevW5;

                        fDataW4.Station = (float)dblStationRF;
                        fDataW4.Elev = (float)dblElevW4;
                        fDataW4.AlignName = "RF";
                        if (p.W4.ContainsKey((float)dblStationRF))
                        {
                            p.W4.Remove((float)dblStationRF);
                        }
                        p.W4.Add((float)dblStationRF, fDataW4);
                        //starts and stops with wall
                        vSDS.W4.OFFSET = fDataW4.Offset;
                        vSDS.W4.ELEV = fDataW4.Elev;

                        dblOffsetsEX[0] = 0.0;
                        //RF
                        dblOffsetsEX[1] = dblOffsetsEX[0] + X0;
                        //TC
                        dblOffsetsEX[2] = dblOffsetsEX[1] + X1;
                        //TOE
                        dblOffsetsEX[3] = dblOffsetsEX[2] + X2;
                        //TOP
                        dblOffsetsEX[4] = dblOffsetsEX[3] + B3;
                        //PL
                        dblOffsetsEX[5] = dblOffsetsEX[4] + 5;
                        //Off5

                        vEXGROUND = wd.getExGround(fWall1.SurfaceEXIST, objAlignRF, dblStationRF, dblOffsetsEX);

                        vSDS.dYh = (float)(dblElevW3 - dblElevW0);
                        vSDS.EX = vEXGROUND;
                        vSDS.SG = (float)SG;
                        vSDS.X = (float)X;
                        vSDS.X0 = (float)X0;
                        vSDS.X1 = (float)X1;
                        vSDS.X2 = (float)X2;
                        vSDS.XT = (float)XT;
                        vSDS.STA = (float)System.Math.Round(dblStationRF, 3);
                        vSDS.RESOLVED = true;

                        fWall1.SDS = vSDS;

                        return vSDS;

                        //X1 ENCROACHES INTO B1 AND SLOPING DOWN  --> NEED WALL AT PL
                    }
                    else if (SG < 0)
                    {

                        if (fWall1.WallStart == false)
                        {
                            n = fWall1.WALLNO + 1;
                            fWall1.WALLNO = n;

                            p.dWALLs.Add(n, p.dWALL);       //****************************??????????????????????????*************************
                            p.dGUTs.Add(n, p.dGUT);         //****************************??????????????????????????*************************
                            fWall1.WallStart = true;
                        }


                        if (dblWallHeight < 0)
                        {
                            X2 = XT - (B1 + 8 / 12 + B3);
                            //XT already tested above for minimum offset


                        }
                        else
                        {

                            if (XT >= B1 + B2 + GW + WT / 12 + B3)
                            {
                                X2 = XT - (B1 + B2 + GW + WT / 12 + B3);
                                //slope width (run)
                                X1 = B1;


                            }
                            else
                            {
                                vSDS.dYh = 2.18f;
                                vSDS.RESOLVED = false;
                                return vSDS;

                            }

                        }

                        dblElevTOP = dblElevTC + B1 * S1;

                        fDataTOP.Station = (float)dblStationRF;
                        fDataTOP.Offset = (float)(X0 + B1);
                        fDataTOP.Elev = (float)dblElevTOP;
                        fDataTOP.AlignName = "RF";
                        if (p.TOP.ContainsKey((float)dblStationRF))
                        {
                            p.TOP.Remove((float)dblStationRF);
                        }
                        p.TOP.Add((float)dblStationRF, fDataTOP);
                        vSDS.TOP.OFFSET = fDataTOP.Offset;
                        vSDS.TOP.ELEV = fDataTOP.Elev;

                        dblElevTOE = dblElevTOP + X2 * SG;

                        fDataTOE.Station = (float)dblStationRF;
                        fDataTOE.Offset = (float)(X0 + B1 + X2);
                        fDataTOE.Elev = (float)dblElevTOE;
                        fDataTOE.AlignName = "RF";
                        if (p.TOE.ContainsKey((float)dblStationRF))
                        {
                            p.TOE.Remove((float)dblStationRF);
                        }
                        p.TOE.Add((float)dblStationRF, fDataTOE);
                        vSDS.TOE.OFFSET = fDataTOE.Offset;
                        vSDS.TOE.ELEV = fDataTOE.Elev;

                        if (dblWallHeight < 0)
                        {
                            dblElevGB = dblElevTOE;
                            fDataGB.Offset = fDataTOE.Offset;
                        }
                        else
                        {
                            dblElevGB = dblElevTOE + B2 * S2 * -1;
                            fDataGB.Offset = (float)(X0 + B1 + X2 + B2);
                        }

                        fDataGB.Station = (float)dblStationRF;
                        fDataGB.Elev = (float)dblElevGB;
                        fDataGB.AlignName = "RF";
                        if (p.GB.ContainsKey((float)dblStationRF))
                        {
                            p.GB.Remove((float)dblStationRF);
                        }
                        p.GB.Add((float)dblStationRF, fDataGB);
                        //starts and stops with wall
                        vSDS.GB.OFFSET = fDataGB.Offset;
                        vSDS.GB.ELEV = fDataGB.Elev;

                        if (dblWallHeight < 0)
                        {
                            dblElevG0 = dblElevGB;
                            fDataG0.Offset = fDataGB.Offset;
                        }
                        else
                        {
                            dblElevG0 = dblElevGB - (GW / 2 * 0.5);
                            fDataG0.Offset = (float)(X0 + B1 + X2 + B2 + GW / 2);
                        }

                        fDataG0.Station = (float)dblStationRF;
                        fDataG0.Elev = (float)dblElevG0;
                        fDataG0.AlignName = "RF";
                        if (p.G0.ContainsKey((float)dblStationRF))
                        {
                            p.G0.Remove((float)dblStationRF);
                        }
                        p.G0.Add((float)dblStationRF, fDataG0);
                        //starts and stops with wall
                        vSDS.G0.OFFSET = fDataG0.Offset;
                        vSDS.G0.ELEV = fDataG0.Elev;

                        dblElevW0 = dblElevGB;


                        if (dblWallHeight < 0)
                        {
                            fDataW0.Offset = (float)(B3 + 8 / 12 + 0.001) * intSign;


                        }
                        else
                        {
                            fDataW0.Offset = (float)(B3 + WT / 12 + 0.001) * intSign;

                        }

                        fDataW0.Station = (float)dblStationPL;
                        fDataW0.Elev = (float)dblElevW0;
                        fDataW0.AlignName = "PL";
                        if (p.W0.ContainsKey((float)dblStationPL))
                        {
                            p.W0.Remove((float)dblStationPL);
                        }
                        p.W0.Add((float)dblStationPL, fDataW0);
                        //starts and stops with wall
                        vSDS.W0.OFFSET = (float)(X - fDataW0.Offset) * intSign;
                        vSDS.W0.ELEV = fDataW0.Elev;

                        dblElevW1 = dblElevW0 + FB / 12;


                        if (dblWallHeight < 0)
                        {
                            fDataW1.Offset = (float)(B3 + 8 / 12) * intSign;


                        }
                        else
                        {
                            fDataW1.Offset = (float)(B3 + WT / 12) * intSign;

                        }

                        fDataW1.Station = (float)dblStationPL;
                        fDataW1.Elev = (float)dblElevW1;
                        fDataW1.AlignName = "PL";
                        if (p.W1.ContainsKey((float)dblStationPL))
                        {
                            p.W1.Remove((float)dblStationPL);
                        }
                        p.W1.Add((float)dblStationPL, fDataW1);
                        //starts and stops with wall
                        vSDS.W1.OFFSET = (float)(X - fDataW1.Offset) * intSign;
                        vSDS.W1.ELEV = fDataW1.Elev;

                        dblElevW2 = dblElevW1;

                        fDataW2.Station = (float)dblStationPL;
                        fDataW2.Offset = (float)B3 * intSign;
                        fDataW2.Elev = (float)dblElevW2;
                        fDataW2.AlignName = "PL";
                        if (p.W2.ContainsKey((float)dblStationPL))
                        {
                            p.W2.Remove((float)dblStationPL);
                        }
                        p.W2.Add((float)dblStationPL, fDataW2);
                        //starts and stops with wall
                        vSDS.W2.OFFSET = (float)(X - fDataW2.Offset) * intSign;
                        vSDS.W2.ELEV = fDataW2.Elev;

                        dblElevW3 = dblElevPL - (B3 * S3);
                        //dblElevW3 = dblElevW2 - FB / 12 + dblWallHeight

                        fDataW3.Station = (float)dblStationPL;
                        fDataW3.Offset = (float)(B3 - 0.001) * intSign;
                        fDataW3.Elev = (float)dblElevW3;
                        fDataW3.AlignName = "PL";
                        if (p.W3.ContainsKey((float)dblStationPL))
                        {
                            p.W3.Remove((float)dblStationPL);
                        }
                        p.W3.Add((float)dblStationPL, fDataW3);
                        //starts and stops with wall
                        vSDS.W3.OFFSET = (float)(X - fDataW3.Offset) * intSign;
                        vSDS.W3.ELEV = fDataW3.Elev;

                        //dblElevPL = dblElevPL;

                        fDataPL.Station = (float)dblStationPL;
                        fDataPL.Offset = 0.0f * intSign;
                        fDataPL.Elev = (float)dblElevPL;
                        fDataPL.AlignName = "PL";
                        if (p.PL.ContainsKey((float)dblStationPL))
                        {
                            p.PL.Remove((float)dblStationPL);
                        }
                        p.PL.Add((float)dblStationPL, fDataPL);
                        //starts and stops with wall
                        vSDS.PL.OFFSET = (float)(X - fDataPL.Offset) * intSign;
                        vSDS.PL.ELEV = fDataPL.Elev;

                        //WALL FOOTING

                        if (dblWallHeight < 0)
                        {
                            dblElevW4 = dblElevW3 - 0.025;
                            //TOP OF SPREAD FOOTING / BOTTOM OF TRENCH FOOTING
                        }
                        else
                        {
                            dblElevW4 = dblElevW3 - 1.0;
                        }

                        fDataW4.Station = (float)dblStationPL;
                        fDataW4.Offset = (float)(B3 - 0.001) * intSign;
                        fDataW4.Elev = (float)dblElevW4;
                        fDataW4.AlignName = "PL";
                        if (p.W4.ContainsKey((float)dblStationPL))
                        {
                            p.W4.Remove((float)dblStationPL);
                        }
                        p.W4.Add((float)dblStationPL, fDataW4);
                        //starts and stops with wall
                        vSDS.W4.OFFSET = (float)(X - fDataW4.Offset) * intSign;
                        vSDS.W4.ELEV = fDataW4.Elev;

                        if (dblWallHeight < 0)
                        {
                            fDataW5.Offset = (float)(fDataW4.Offset + 0.17);
                        }
                        else
                        {
                            fDataW5.Offset = (float)(fDataW4.Offset + 1.0);
                        }

                        dblElevW5 = dblElevW4;

                        fDataW5.Station = (float)dblStationPL;
                        fDataW5.Elev = (float)dblElevW5;
                        fDataW5.AlignName = "RF";
                        if (p.W5.ContainsKey((float)dblStationPL))
                        {
                            p.W5.Remove((float)dblStationPL);
                        }
                        p.W5.Add((float)dblStationPL, fDataW5);
                        //starts and stops with wall
                        vSDS.W5.OFFSET = (float)(X - fDataW5.Offset) * intSign;
                        vSDS.W5.ELEV = fDataW5.Elev;

                        if (dblWallHeight < 0)
                        {
                            dblElevW6 = dblElevW5 - 1.0;
                            fDataW6.Offset = (float)(fDataW5.Offset - 0.001);
                        }
                        else
                        {
                            dblElevW6 = dblElevW5 - 1.0;
                            fDataW6.Offset = (float)(fDataW5.Offset + 0.001);
                        }

                        fDataW6.Station = (float)dblStationPL;
                        fDataW6.Elev = (float)dblElevW6;
                        fDataW6.AlignName = "PL";
                        if (p.W6.ContainsKey((float)dblStationPL))
                        {
                            p.W6.Remove((float)dblStationPL);
                        }
                        p.W6.Add((float)dblStationPL, fDataW6);
                        //starts and stops with wall
                        vSDS.W6.OFFSET = (float)(X - fDataW6.Offset) * intSign;
                        vSDS.W6.ELEV = fDataW6.Elev;

                        if (dblWallHeight < 0)
                        {
                            fDataW7.Offset = (float)(fDataW6.Offset - 1.001);
                        }
                        else
                        {
                            fDataW7.Offset = (float)(fDataW6.Offset - 5.001);
                        }

                        dblElevW7 = dblElevW6;

                        fDataW7.Station = (float)dblStationPL;
                        fDataW7.Elev = (float)dblElevW7;
                        fDataW7.AlignName = "PL";
                        if (p.W7.ContainsKey((float)dblStationPL))
                        {
                            p.W7.Remove((float)dblStationPL);
                        }
                        p.W7.Add((float)dblStationPL, fDataW7);
                        //starts and stops with wall
                        vSDS.W7.OFFSET = (float)(X - fDataW7.Offset) * intSign;
                        vSDS.W7.ELEV = fDataW7.Elev;

                        if (dblWallHeight < 0)
                        {
                            fDataW8.Offset = (float)(fDataW7.Offset - 0.001);
                        }
                        else
                        {
                            fDataW8.Offset = (float)(fDataW7.Offset + 0.001);
                        }

                        dblElevW8 = dblElevW4;

                        fDataW8.Station = (float)dblStationPL;
                        fDataW8.Elev = (float)dblElevW8;
                        fDataW8.AlignName = "PL";
                        if (p.W8.ContainsKey((float)dblStationPL))
                        {
                            p.W8.Remove((float)dblStationPL);
                        }
                        p.W8.Add((float)dblStationPL, fDataW8);
                        //starts and stops with wall
                        vSDS.W8.OFFSET = (float)(X - fDataW8.Offset) * intSign;
                        vSDS.W8.ELEV = fDataW8.Elev;

                        if (dblWallHeight < 0)
                        {
                            fDataW9.Offset = (float)(fDataW8.Offset + 0.17);
                        }
                        else
                        {
                            fDataW9.Offset = (float)(fDataW0.Offset + 0.001);
                        }

                        dblElevW9 = dblElevW8;

                        fDataW9.Station = (float)dblStationPL;
                        fDataW9.Elev = (float)dblElevW9;
                        fDataW9.AlignName = "PL";
                        if (p.W9.ContainsKey((float)dblStationPL))
                        {
                            p.W9.Remove((float)dblStationPL);
                        }
                        p.W9.Add((float)dblStationPL, fDataW9);
                        //starts and stops with wall
                        vSDS.W9.OFFSET = (float)(X - fDataW9.Offset) * intSign;
                        vSDS.W9.ELEV = fDataW9.Elev;

                        dblOffsetsEX[0] = 0.0;
                        //RF
                        dblOffsetsEX[1] = dblOffsetsEX[0] + X0;
                        //TC
                        dblOffsetsEX[2] = dblOffsetsEX[1] + X1;
                        //TOE
                        dblOffsetsEX[3] = dblOffsetsEX[2] + X2;
                        //TOP
                        dblOffsetsEX[4] = dblOffsetsEX[3] + B3;
                        //PL
                        dblOffsetsEX[5] = dblOffsetsEX[4] + 5;
                        //Off5

                        vEXGROUND = wd.getExGround(fWall1.SurfaceEXIST, objAlignRF, dblStationRF, dblOffsetsEX);

                        vSDS.dYh = (float)(dblElevW3 - dblElevW2);
                        vSDS.EX = vEXGROUND;
                        vSDS.SG = (float)SG;
                        vSDS.X = (float)X;
                        vSDS.X0 = (float)X0;
                        vSDS.X1 = (float)X1;
                        vSDS.X2 = (float)X2;
                        vSDS.XT = (float)XT;
                        vSDS.STA = (float)System.Math.Round(dblStationRF, 3);
                        vSDS.RESOLVED = true;

                        fWall1.SDS = vSDS;

                        return vSDS;

                    }

                }


            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error in wallSectionAnalysis " + ex.ToString());
                return null;
            }

            return vSDS;
        }
    }
}
