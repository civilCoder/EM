using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using wdp = Wall.Wall_DesignProfile;

namespace Wall
{
    public static class Wall_LocateCurb
    {
        static Wall_Form.frmWall4 fWall4 = Wall_Forms.wForms.fWall4;

        public static bool
        LocateCurb(string strName, Alignment objAlignPL, Alignment objAlignRF){
            bool success = false;

            double B1 = double.Parse(fWall4.tbx_B1.Text);
            double B2 = double.Parse(fWall4.tbx_B2.Text);

            double S0 = double.Parse(fWall4.tbx_S0.Text);
            double S1 = double.Parse(fWall4.tbx_S1.Text);
            double S2 = double.Parse(fWall4.tbx_S2.Text);
            double SG = double.Parse(fWall4.tbx_SG.Text);

            double CF = double.Parse(fWall4.tbx_CF.Text);
            double CV = 0;
            double CH = 0;

            List<Point3d> pnts3dRF = new List<Point3d>();
            List<Point3d> pnts3dFL = new List<Point3d>();
            List<Point3d> pnts3dTC = new List<Point3d>();
            List<Point3d> pnts3dTOE = new List<Point3d>();
            List<Point3d> pnts3dTOP = new List<Point3d>();
            List<Point3d> pnts3dPL = new List<Point3d>();

            bool boolMakeTempSurface = false;
            Profile objProfileRF = null, objProfilePL = null;

            try
            {
                int intSign = fWall4.Side;
                List<double> dblStationsFinal = dblStationsFinal = fWall4.Stations;


                if ((dblStationsFinal.Count == 0))
                {
                    return false;


                }
                else
                {
                    objProfileRF = Prof.getProfile(objAlignRF.ObjectId, strName);
                    objProfilePL = Prof.getProfile(objAlignPL.ObjectId, "EXIST");

                }

                Point3d pnt3dBeg = default(Point3d);
                Point3d pnt3dEnd = default(Point3d);

                bool boolStart = false;
                bool boolDone = false;

                double dblEasting = 0, dblNorthing = 0, dblStationPL = 0, dblOffsetPL = 0;
                double dblElevRF = 0, dblElevPL = 0, dblElevTOP = 0, dblElevDiff = 0;

                for (int i = 0; i < dblStationsFinal.Count ; i++)
                {
                    double dblStationRF = dblStationsFinal[i];  //CURRENT STATION ON REF
                    
                    Debug.Print(dblStationRF.ToString());


                    if (System.Math.Abs(dblStationRF - objAlignRF.StartingStation) < 0.1)
                    {
                        dblStationRF = Base_Tools45.Math.roundUP2(objAlignRF.StartingStation);
                    }
                    if (System.Math.Abs(dblStationRF - objAlignRF.EndingStation) < 0.1)
                    {
                        dblStationRF = Base_Tools45.Math.roundDown2(objAlignRF.EndingStation);
                    }

                    try
                    {
                        objAlignRF.PointLocation(dblStationRF, 0.0, ref dblEasting, ref dblNorthing);
                    }
                    catch (Autodesk.Civil.PointNotOnEntityException )
                    {
                    }

                    try
                    {
                        objAlignPL.StationOffset(dblEasting, dblNorthing, ref dblStationPL, ref dblOffsetPL);
                        // CORRESPONDING STATION ON PL
                    }
                    catch (Autodesk.Civil.PointNotOnEntityException )
                    {
                        dblStationPL = 0.0;
                    }

                    //CHECK IF

                    if (System.Math.Round(dblStationRF, 1) >= System.Math.Round(objAlignRF.StartingStation, 1))
                    {
                        boolStart = true;

                    }


                    if (System.Math.Round(dblStationRF, 1) <= System.Math.Round(objAlignRF.EndingStation, 1))
                    {
                        boolDone = false;

                    }

                    if (boolStart == true & boolDone == false)
                    {
                        try
                        {
                            dblElevRF = objProfileRF.ElevationAt(dblStationRF);
                            //elevation on REF at current REF station
                        }
                        catch (Autodesk.AutoCAD.Runtime.Exception )
                        {
                        }

                        try
                        {
                            dblElevPL = objProfilePL.ElevationAt(Base_Tools45.Math.roundDown2(dblStationPL));
                            //elevation on PL at PL station corresponding to REF station
                        }
                        catch (Autodesk.AutoCAD.Runtime.Exception )
                        {
                            dblElevPL = 0;
                        }

                        //valid surface elevations for both RF and PL
                        if (dblElevRF > 0 & dblElevPL > 0)
                        {

                            dblElevTOP = dblElevPL - (B2 * S2);
                            //top of slope - always sloping away from PL

                            try
                            {
                                objAlignRF.PointLocation(dblStationRF, 0.0, ref dblEasting, ref dblNorthing);
                                //point location at RF align station corresponding to PL station
                            }
                            catch (Autodesk.AutoCAD.Runtime.Exception )
                            {
                            }

                            Point3d pnt3d = new Point3d(dblEasting, dblNorthing, dblElevRF);
                            pnts3dRF.Add(pnt3d);

                            dblElevDiff = dblElevRF - dblElevPL;

                            try
                            {
                                objAlignPL.StationOffset(dblEasting, dblNorthing, ref dblStationPL, ref dblOffsetPL);
                                //station and offset from PL to REF
                            }
                            catch (Autodesk.Civil.PointNotOnEntityException )
                            {
                                dblStationPL = 0.0;
                            }

                            //0 station means that the return station was outside limits
                            if (dblStationPL == 0)
                            {

                                pnt3dBeg = new Point3d(dblEasting, dblNorthing, 0.0);
                                //point location at REF align

                                dblStationPL = objAlignPL.EndingStation;
                                objAlignPL.PointLocation(objAlignPL.EndingStation, 0, ref dblEasting, ref dblNorthing);
                                pnt3dEnd = new Point3d(dblEasting, dblNorthing, 0.0);
                                //point location at end PL align

                                dblOffsetPL = pnt3dBeg.getDistance(pnt3dEnd);
                                //distance from REF to PL

                            }

                            double XT = System.Math.Abs(dblOffsetPL);

                            if (CF != 0)
                            {
                                CV = CF / 12 + 0.021;
                                CH = CF / 12 / 4 + 0.5;
                            }
                            else
                            {
                                CV = 0;
                                CH = 0;
                            }

                            double SLOPE = 0.0;
                            //PL lower than REF - slope down
                            if (dblElevDiff >= 0)
                            {
                                SLOPE = SG * -1;
                            }
                            else
                            {
                                SLOPE = SG;
                            }

                            double X1 = ((dblElevDiff) + CV + (XT - CH - B2) * SLOPE + (B2 * S2)) / (SLOPE - S1);
                            double X2 = 0;

                            if (X1 < 0)
                            {
                                boolMakeTempSurface = true;

                                double dblElevRFs = dblElevPL - (B2 * S2) - (XT - B2) * SLOPE;
                                double dblDY = dblElevRFs - dblElevRF;


                                if (CV == 0)
                                {
                                    if (SLOPE > 0)
                                    {
                                        if (S1 > 0)
                                        {
                                            X2 = XT - B2 + dblDY / (SLOPE - S1);

                                        }
                                        else
                                        {
                                        }


                                    }
                                    else
                                    {
                                    }

                                    double dblElevTOE = 0, dblElevBC = 0, dblElevFL = 0;

                                    //dblElevPL = dblElevPL;
                                    objAlignPL.PointLocation(dblStationPL, 0.0, ref dblEasting, ref dblNorthing);
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevPL);
                                    pnts3dPL.Add(pnt3d);

                                    dblElevTOP = dblElevPL - (B2 * S2);
                                    objAlignPL.PointLocation(dblStationPL, B2 * intSign, ref dblEasting, ref dblNorthing);
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOP);
                                    pnts3dTOP.Add(pnt3d);

                                    dblElevTOE = dblElevPL - (B2 * S2) - (X2 * SLOPE);
                                    objAlignPL.PointLocation(dblStationPL, (B2 + X2) * intSign, ref dblEasting, ref dblNorthing);
                                    //point perp to PL
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOE);
                                    pnts3dTOE.Add(pnt3d);

                                    dblElevBC = dblElevTOE;
                                    objAlignPL.PointLocation(dblStationPL, (B2 + X2) * intSign, ref dblEasting, ref dblNorthing);
                                    //point perp to PL
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevBC);
                                    pnts3dTC.Add(pnt3d);

                                    dblElevFL = dblElevRF + X1 * S0;
                                    objAlignPL.PointLocation(dblStationPL, (XT - X1) * intSign, ref dblEasting, ref dblNorthing);
                                    //point perp to PL
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevFL);
                                    pnts3dFL.Add(pnt3d);


                                }
                                else if (CV > 0)
                                {
                                    double dblDX = 0, dblElevTOE = 0, dblElevBC = 0, dblElevFL = 0;
                                    dblDX = (dblDY - (B1 * S1) - CV - (CH + B1) * S0) / (SLOPE - S0);

                                    //dblElevPL = dblElevPL;
                                    objAlignPL.PointLocation(dblStationPL, 0.0, ref dblEasting, ref dblNorthing);
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevPL);
                                    pnts3dPL.Add(pnt3d);

                                    dblElevTOP = dblElevPL - (B2 * S2);
                                    objAlignPL.PointLocation(dblStationPL, B2 * intSign, ref dblEasting, ref dblNorthing);
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOP);
                                    pnts3dTOP.Add(pnt3d);

                                    dblElevTOE = dblElevTOP - ((X2 + dblDX) * SLOPE);
                                    objAlignPL.PointLocation(dblStationPL, (B2 + X2 + dblDX) * intSign, ref dblEasting, ref dblNorthing);
                                    //point perp to PL
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOE);
                                    pnts3dTOE.Add(pnt3d);

                                    dblElevBC = dblElevTOE - (B1 * S1);
                                    objAlignPL.PointLocation(dblStationPL, (B2 + X2 + dblDX + B1) * intSign, ref dblEasting, ref dblNorthing);
                                    //point perp to PL
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevBC);
                                    pnts3dTC.Add(pnt3d);

                                    dblElevFL = dblElevBC - CV;
                                    objAlignPL.PointLocation(dblStationPL, (B2 + X2 + dblDX + B1 + CH) * intSign, ref dblEasting, ref dblNorthing);
                                    //point perp to PL
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevFL);
                                    pnts3dFL.Add(pnt3d);

                                }


                            }
                            else if (X1 >= 0)
                            {
                                double dblDX = 0, dblElevTOE = 0, dblElevBC = 0, dblElevFL = 0;

                                if (X1 > B1)
                                {
                                    if (S0 == S1)
                                    {
                                        X2 = XT - CH - X1 - B2;
                                        //all good we use B1
                                    }
                                    else if (S0 > S1)
                                    {
                                        dblDX = ((dblElevRF - dblElevPL) + CV + (B1 * S1) + (B2 * S2) + (XT - CH - B1 - B2) * SLOPE) / (SLOPE - S0);
                                        X2 = XT - dblDX - CH - B1 - B2;
                                    }
                                    else if (S0 < S1)
                                    {
                                        dblDX = ((dblElevRF - dblElevPL) + CV + (B1 * S1) + (B2 * S2) + (XT - CH - B1 - B2) * SLOPE) / (SLOPE - S0);
                                        X2 = XT - dblDX - CH - B1 - B2;
                                    }


                                }
                                else if (X1 < B1)
                                {
                                    if (S0 > S1)
                                    {
                                        dblDX = ((dblElevRF - dblElevPL) + CV + (B1 * S1) + (XT - CH - B1 - B2) * SLOPE + (B2 * S2)) / (S0 - SLOPE);
                                        X2 = XT - CH - B1 - B2 + dblDX;
                                    }
                                    else if (S0 == S1)
                                    {
                                        X2 = XT - CH - X1 - B2;
                                        //B1 is used no change necessary
                                    }
                                    else if (S0 < S1)
                                    {
                                        dblDX = ((dblElevRF - dblElevPL) + CV + (B1 * S1) + (XT - CH - B1 - B2) * SLOPE + (B2 * S2)) / (S0 - SLOPE);
                                        X2 = XT - CH - B1 - B2 + dblDX;
                                    }

                                }

                                //dblElevPL = dblElevPL;
                                objAlignPL.PointLocation(dblStationPL, 0.0, ref dblEasting, ref dblNorthing);
                                pnt3d = new Point3d(dblEasting, dblNorthing, dblElevPL);
                                pnts3dPL.Add(pnt3d);

                                dblElevTOP = dblElevPL - (B2 * S2);
                                objAlignPL.PointLocation(dblStationPL, B2 * intSign, ref dblEasting, ref dblNorthing);
                                pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOP);
                                pnts3dTOP.Add(pnt3d);

                                dblElevTOE = dblElevTOP - (X2 * SLOPE);
                                objAlignPL.PointLocation(dblStationPL, (B2 + X2) * intSign, ref dblEasting, ref dblNorthing);
                                //point perp to PL
                                pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOE);
                                pnts3dTOE.Add(pnt3d);

                                dblElevBC = dblElevTOE - (B1 * S1);
                                objAlignPL.PointLocation(dblStationPL, (B2 + X2 + B1) * intSign, ref dblEasting, ref dblNorthing);
                                //point perp to PL
                                pnt3d = new Point3d(dblEasting, dblNorthing, dblElevBC);
                                pnts3dTC.Add(pnt3d);

                                dblElevFL = dblElevBC - CV;
                                objAlignPL.PointLocation(dblStationPL, (B2 + X2 + B1 + CH) * intSign, ref dblEasting, ref dblNorthing);
                                //point perp to PL
                                pnt3d = new Point3d(dblEasting, dblNorthing, dblElevFL);
                                pnts3dFL.Add(pnt3d);

                            }

                        }
                    }
                }
                ObjectId idPoly3d = ObjectId.Null;

                using(BaseObjs._acadDoc.LockDocument())
                {
                    using (Transaction TR = BaseObjs.startTransactionDb())
                    {

                        try
                        {
                            ObjectIdCollection objEntIDs = new ObjectIdCollection();

                            //Dim intXDataType As List(Of int) = New List(Of int)(2)
                            //Dim varXDataVal As List(Of Object) = New List(Of Object)(2)

                            string strLayer = null;
                            if (boolMakeTempSurface)
                            {
                                strLayer = "CPNT-BRKLINE-TEMP";
                            }
                            else
                            {
                                strLayer = "CPNT-BRKLINE";
                            }

                            Layer.manageLayers(strLayer);

                            //intXDataType.Add(1001) : varXDataVal.Add("PL")

                            TypedValue[] tvs = new TypedValue[2];
                            tvs[0] = new TypedValue(1001, "PL");


                            if (boolMakeTempSurface == false)
                            {
                                idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dRF, strLayer, 6);
                                tvs[1] = new TypedValue(1000, "REF");
                                idPoly3d.setXData(tvs, "PL");
                                objEntIDs.Add(idPoly3d);

                                idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dFL, strLayer, 1);
                                tvs[1] = new TypedValue(1000, "FL");
                                idPoly3d.setXData(tvs, "PL");
                                objEntIDs.Add(idPoly3d);


                            }
                            else
                            {
                                idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dFL, strLayer, 1);
                                tvs[1] = new TypedValue(1000, "FL");
                                idPoly3d.setXData(tvs, "PL");
                                objEntIDs.Add(idPoly3d);

                            }

                            idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dTC, strLayer, 2);
                            tvs[1] = new TypedValue(1000, "TC");
                            idPoly3d.setXData(tvs, "PL");
                            objEntIDs.Add(idPoly3d);

                            idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dTOE, strLayer, 3);
                            tvs[1] = new TypedValue(1000, "TOE");
                            idPoly3d.setXData(tvs, "PL");
                            objEntIDs.Add(idPoly3d);

                            idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dTOP, strLayer, 4);
                            tvs[1] = new TypedValue(1000, "TOP");
                            idPoly3d.setXData(tvs, "PL");
                            objEntIDs.Add(idPoly3d);

                            idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dPL, strLayer, 5);
                            tvs[1] = new TypedValue(1000, "PL");
                            idPoly3d.setXData(tvs, "PL");
                            objEntIDs.Add(idPoly3d);

                            ObjectIdCollection objEntEndIDs = wdp.makeEndBrklinesORG(strLayer, objEntIDs, false);

                            bool exists = false;

                            if (boolMakeTempSurface == false)
                            {
                                TinSurface objSurfaceCPNT = Surf.getTinSurface("CPNT-ON", out exists);

                                objSurfaceCPNT.BreaklinesDefinition.AddStandardBreaklines(objEntIDs, 1, 0, 0, 0);
                                objSurfaceCPNT.BreaklinesDefinition.AddStandardBreaklines(objEntEndIDs, 1, 0, 0, 0);


                            }
                            else
                            {
                                TinSurface objSurfaceTemp = Surf.getTinSurface("TEMP", out exists);
                                objSurfaceTemp.BreaklinesDefinition.AddStandardBreaklines(objEntIDs, 1, 0, 0, 0);
                                objSurfaceTemp.BreaklinesDefinition.AddStandardBreaklines(objEntEndIDs, 1, 0, 0, 0);

                            }

                        }
                        catch (Autodesk.AutoCAD.Runtime.Exception ex)
                        {
                            Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog(ex.ToString());
                        }

                        TR.Commit();

                    }
                }

            }
            catch (Autodesk.AutoCAD.Runtime.Exception )
            {
                MessageBox.Show("LocateCurb");
                return false;
            }
            return success;
        }

        public static bool
        LocateCurb4(string strName, Alignment objAlignPL, Alignment objAlignRF)
        {
            bool success = false;


            Profile objProfilePL = default(Profile);
            Profile objProfileRF = default(Profile);

            double B1 = double.Parse(fWall4.tbx_B1.Text);
            double B2 = double.Parse(fWall4.tbx_B2.Text);

            double S0 = double.Parse(fWall4.tbx_S0.Text);
            double S1 = double.Parse(fWall4.tbx_S1.Text);
            double S2 = double.Parse(fWall4.tbx_S2.Text);
            double SG = double.Parse(fWall4.tbx_SG.Text);

            double CF = double.Parse(fWall4.tbx_CF.Text);
            double CV = 0;
            double CH = 0;


            List<Point3d> pnts3dRF = new List<Point3d>();
            List<Point3d> pnts3dFL = new List<Point3d>();
            List<Point3d> pnts3dTC = new List<Point3d>();
            List<Point3d> pnts3dTOE = new List<Point3d>();
            List<Point3d> pnts3dTOP = new List<Point3d>();
            List<Point3d> pnts3dPL = new List<Point3d>();

            double dblOffsetPL = 0.0, dblOffsetRF = 0.0, dblEasting = 0.0,dblNorthing = 0.0;
            double dblElevRF = 0, dblElevRFs = 0, dblElevFL = 0, dblElevBC = 0, dblElevTOE = 0, dblElevTOP = 0, dblElevPL = 0, dblElevDiff = 0;
            double dblStationRF = 0, dblStationPL = 0;
            double XT = 0, X0 = 0, X1 = 0, X2 = 0, dblDX = 0, dblDY = 0;

            bool boolMakeTempSurface = false;


            try
            {
                int intSign = fWall4.Side;

                List<double> dblStationsFinal = new List<double>();
                dblStationsFinal = fWall4.Stations;


                if ((dblStationsFinal.Count == 0))
                {
                    return false;


                }
                else
                {
                    objProfileRF = Prof.getProfile(objAlignRF.ObjectId, strName);
                    objProfilePL = Prof.getProfile(objAlignPL.ObjectId, "EXIST");

                }

                Point3d pnt3dBeg = default(Point3d);
                Point3d pnt3dEnd = default(Point3d);

                bool boolStart = false;
                bool boolDone = false;


                for (int i = 0; i < dblStationsFinal.Count; i++)
                {
                    dblStationRF = dblStationsFinal[i];
                    //CURRENT STATION ON REF
                    Debug.Print(dblStationRF.ToString());

                    dblEasting = 0;
                    dblNorthing = 0;

                    if (System.Math.Abs(dblStationRF - objAlignRF.StartingStation) < 0.1)
                    {
                        dblStationRF = Base_Tools45.Math.roundUP2(objAlignRF.StartingStation);
                    }
                    if (System.Math.Abs(dblStationRF - objAlignRF.EndingStation) < 0.1)
                    {
                        dblStationRF = Base_Tools45.Math.roundDown2(objAlignRF.EndingStation);
                    }

                    try
                    {
                        objAlignRF.PointLocation(dblStationRF, 0.0, ref dblEasting, ref dblNorthing);
                    }
                    catch (Autodesk.Civil.PointNotOnEntityException )
                    {
                    }

                    try
                    {
                        objAlignPL.StationOffset(dblEasting, dblNorthing, ref dblStationPL, ref dblOffsetPL);
                        // CORRESPONDING STATION ON PL
                    }
                    catch (Autodesk.Civil.PointNotOnEntityException )
                    {
                        dblStationPL = 0.0;
                    }

                    //CHECK IF

                    if (System.Math.Round(dblStationRF, 1) >= System.Math.Round(objAlignRF.StartingStation, 1))
                    {
                        boolStart = true;

                    }


                    if (System.Math.Round(dblStationRF, 1) <= System.Math.Round(objAlignRF.EndingStation, 1))
                    {
                        boolDone = false;

                    }


                    if (boolStart == true & boolDone == false)
                    {
                        try
                        {
                            dblElevRF = objProfileRF.ElevationAt(dblStationRF);
                            //elevation on REF at current REF station
                        }
                        catch (Autodesk.AutoCAD.Runtime.Exception )
                        {
                        }

                        try
                        {
                            dblElevPL = objProfilePL.ElevationAt(Base_Tools45.Math.roundDown2(dblStationPL));
                            //elevation on PL at PL station corresponding to REF station
                        }
                        catch (Autodesk.AutoCAD.Runtime.Exception )
                        {
                            dblElevPL = 0;
                        }

                        //valid surface elevations for both RF and PL
                        if (dblElevRF > 0 & dblElevPL > 0)
                        {

                            dblElevTOP = dblElevPL - (B2 * S2);
                            //top of slope - always sloping away from PL

                            try
                            {
                                objAlignRF.PointLocation(dblStationRF, 0.0, ref dblEasting, ref dblNorthing);
                                //point location at RF align station corresponding to PL station
                            }
                            catch (Autodesk.AutoCAD.Runtime.Exception )
                            {
                            }

                            Point3d pnt3d = new Point3d(dblEasting, dblNorthing, dblElevRF);
                            pnts3dRF.Add(pnt3d);

                            dblElevDiff = dblElevRF - dblElevPL;

                            try
                            {
                                objAlignPL.StationOffset(dblEasting, dblNorthing, ref dblStationPL, ref dblOffsetPL);
                                //station and offset from PL to REF
                            }
                            catch (Autodesk.Civil.PointNotOnEntityException )
                            {
                                dblStationPL = 0.0;
                            }

                            //0 station means that the return station was outside limits
                            if (dblStationPL == 0)
                            {

                                pnt3dBeg = new Point3d(dblEasting, dblNorthing, 0.0);
                                //point location at REF align

                                dblStationPL = objAlignPL.EndingStation;
                                objAlignPL.PointLocation(objAlignPL.EndingStation, 0, ref dblEasting, ref dblNorthing);
                                pnt3dEnd = new Point3d(dblEasting, dblNorthing, 0.0);
                                //point location at end PL align

                                dblOffsetPL = pnt3dBeg.getDistance(pnt3dEnd);
                                //distance from REF to PL

                            }

                            XT = System.Math.Abs(dblOffsetPL);

                            if (CF != 0)
                            {
                                CV = CF / 12 + 0.021;
                                CH = CF / 12 / 4 + 0.5;
                            }
                            else
                            {
                                CV = 0;
                                CH = 0;
                            }
                            double SLOPE = 0.0;
                            //PL lower than REF - slope down
                            if (dblElevDiff >= 0)
                            {
                                SLOPE = SG * -1;
                            }
                            else
                            {
                                SLOPE = SG;
                            }

                            X1 = ((dblElevRF - dblElevPL) + CV + (XT - CH - B2) * SLOPE + (B2 * S2)) / (SLOPE - S1);


                            if (X1 < 0)
                            {
                                boolMakeTempSurface = true;

                                dblElevRFs = dblElevPL - (B2 * S2) - (XT - B2) * SLOPE;
                                dblDY = dblElevRFs - dblElevRF;


                                if (CV == 0)
                                {
                                    if (SLOPE > 0)
                                    {
                                        if (S1 > 0)
                                        {
                                            X2 = XT - B2 + dblDY / (SLOPE - S1);

                                        }
                                        else
                                        {
                                        }


                                    }
                                    else
                                    {
                                    }

                                    //dblElevPL = dblElevPL;
                                    objAlignPL.PointLocation(dblStationPL, 0.0, ref dblEasting, ref dblNorthing);
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevPL);
                                    pnts3dPL.Add(pnt3d);

                                    dblElevTOP = dblElevPL - (B2 * S2);
                                    objAlignPL.PointLocation(dblStationPL, B2 * intSign, ref dblEasting, ref dblNorthing);
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOP);
                                    pnts3dTOP.Add(pnt3d);

                                    dblElevTOE = dblElevPL - (B2 * S2) - (X2 * SLOPE);
                                    objAlignPL.PointLocation(dblStationPL, (B2 + X2) * intSign, ref dblEasting, ref dblNorthing);
                                    //point perp to PL
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOE);
                                    pnts3dTOE.Add(pnt3d);

                                    dblElevBC = dblElevTOE;
                                    objAlignPL.PointLocation(dblStationPL, (B2 + X2) * intSign, ref dblEasting, ref dblNorthing);
                                    //point perp to PL
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevBC);
                                    pnts3dTC.Add(pnt3d);

                                    dblElevFL = dblElevRF + X1 * S0;
                                    objAlignPL.PointLocation(dblStationPL, (XT - X1) * intSign, ref dblEasting, ref dblNorthing);
                                    //point perp to PL
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevFL);
                                    pnts3dFL.Add(pnt3d);


                                }
                                else if (CV > 0)
                                {
                                    dblDX = (dblDY - (B1 * S1) - CV - (CH + B1) * S0) / (SLOPE - S0);

                                    //dblElevPL = dblElevPL;
                                    objAlignPL.PointLocation(dblStationPL, 0.0, ref dblEasting, ref dblNorthing);
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevPL);
                                    pnts3dPL.Add(pnt3d);

                                    dblElevTOP = dblElevPL - (B2 * S2);
                                    objAlignPL.PointLocation(dblStationPL, B2 * intSign, ref dblEasting, ref dblNorthing);
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOP);
                                    pnts3dTOP.Add(pnt3d);

                                    dblElevTOE = dblElevTOP - ((X2 + dblDX) * SLOPE);
                                    objAlignPL.PointLocation(dblStationPL, (B2 + X2 + dblDX) * intSign, ref dblEasting, ref dblNorthing);
                                    //point perp to PL
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOE);
                                    pnts3dTOE.Add(pnt3d);

                                    dblElevBC = dblElevTOE - (B1 * S1);
                                    objAlignPL.PointLocation(dblStationPL, (B2 + X2 + dblDX + B1) * intSign, ref dblEasting, ref dblNorthing);
                                    //point perp to PL
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevBC);
                                    pnts3dTC.Add(pnt3d);

                                    dblElevFL = dblElevBC - CV;
                                    objAlignPL.PointLocation(dblStationPL, (B2 + X2 + dblDX + B1 + CH) * intSign, ref dblEasting, ref dblNorthing);
                                    //point perp to PL
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevFL);
                                    pnts3dFL.Add(pnt3d);

                                }


                            }
                            else if (X1 >= 0)
                            {

                                if (X1 > B1)
                                {
                                    if (S0 == S1)
                                    {
                                        X2 = XT - CH - X1 - B2;
                                        //all good we use B1
                                    }
                                    else if (S0 > S1)
                                    {
                                        dblDX = ((dblElevRF - dblElevPL) + CV + (B1 * S1) + (B2 * S2) + (XT - CH - B1 - B2) * SLOPE) / (SLOPE - S0);
                                        X2 = XT - dblDX - CH - B1 - B2;
                                    }
                                    else if (S0 < S1)
                                    {
                                        dblDX = ((dblElevRF - dblElevPL) + CV + (B1 * S1) + (B2 * S2) + (XT - CH - B1 - B2) * SLOPE) / (SLOPE - S0);
                                        X2 = XT - dblDX - CH - B1 - B2;
                                    }


                                }
                                else if (X1 < B1)
                                {
                                    if (S0 > S1)
                                    {
                                        dblDX = ((dblElevRF - dblElevPL) + CV + (B1 * S1) + (XT - CH - B1 - B2) * SLOPE + (B2 * S2)) / (S0 - SLOPE);
                                        X2 = XT - CH - B1 - B2 + dblDX;
                                    }
                                    else if (S0 == S1)
                                    {
                                        X2 = XT - CH - X1 - B2;
                                        //B1 is used no change necessary
                                    }
                                    else if (S0 < S1)
                                    {
                                        dblDX = ((dblElevRF - dblElevPL) + CV + (B1 * S1) + (XT - CH - B1 - B2) * SLOPE + (B2 * S2)) / (S0 - SLOPE);
                                        X2 = XT - CH - B1 - B2 + dblDX;
                                    }

                                }

                                //dblElevPL = dblElevPL;
                                objAlignPL.PointLocation(dblStationPL, 0.0, ref dblEasting, ref dblNorthing);
                                pnt3d = new Point3d(dblEasting, dblNorthing, dblElevPL);
                                pnts3dPL.Add(pnt3d);

                                dblElevTOP = dblElevPL - (B2 * S2);
                                objAlignPL.PointLocation(dblStationPL, B2 * intSign, ref dblEasting, ref dblNorthing);
                                pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOP);
                                pnts3dTOP.Add(pnt3d);

                                dblElevTOE = dblElevTOP - (X2 * SLOPE);
                                objAlignPL.PointLocation(dblStationPL, (B2 + X2) * intSign, ref dblEasting, ref dblNorthing);
                                //point perp to PL
                                pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOE);
                                pnts3dTOE.Add(pnt3d);

                                dblElevBC = dblElevTOE - (B1 * S1);
                                objAlignPL.PointLocation(dblStationPL, (B2 + X2 + B1) * intSign, ref dblEasting, ref dblNorthing);
                                //point perp to PL
                                pnt3d = new Point3d(dblEasting, dblNorthing, dblElevBC);
                                pnts3dTC.Add(pnt3d);

                                dblElevFL = dblElevBC - CV;
                                objAlignPL.PointLocation(dblStationPL, (B2 + X2 + B1 + CH) * intSign, ref dblEasting, ref dblNorthing);
                                //point perp to PL
                                pnt3d = new Point3d(dblEasting, dblNorthing, dblElevFL);
                                pnts3dFL.Add(pnt3d);

                            }

                        }
                    }
                }
                ObjectId idPoly3d = ObjectId.Null;

                using (BaseObjs._acadDoc.LockDocument())
                {
                    using (Transaction TR = BaseObjs.startTransactionDb())
                    {

                        try
                        {
                            ObjectIdCollection objEntIDs = new ObjectIdCollection();

                            //Dim intXDataType As List(Of int) = New List(Of int)(2)
                            //Dim varXDataVal As List(Of Object) = New List(Of Object)(2)

                            string strLayer = null;
                            if (boolMakeTempSurface)
                            {
                                strLayer = "CPNT-BRKLINE-TEMP";
                            }
                            else
                            {
                                strLayer = "CPNT-BRKLINE";
                            }

                            Layer.manageLayers(strLayer);

                            //intXDataType.Add(1001) : varXDataVal.Add("PL")

                            TypedValue[] tvs = new TypedValue[2];
                            tvs[0] = new TypedValue(1001, "PL");


                            if (boolMakeTempSurface == false)
                            {
                                idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dRF, strLayer, 6);
                                tvs[1] = new TypedValue(1000, "REF");
                                idPoly3d.setXData(tvs, "PL");
                                objEntIDs.Add(idPoly3d);

                                idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dFL, strLayer, 1);
                                tvs[1] = new TypedValue(1000, "FL");
                                idPoly3d.setXData(tvs, "PL");
                                objEntIDs.Add(idPoly3d);


                            }
                            else
                            {
                                idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dFL, strLayer, 1);
                                tvs[1] = new TypedValue(1000, "FL");
                                idPoly3d.setXData(tvs, "PL");
                                objEntIDs.Add(idPoly3d);

                            }

                            idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dTC, strLayer, 2);
                            tvs[1] = new TypedValue(1000, "TC");
                            idPoly3d.setXData(tvs, "PL");
                            objEntIDs.Add(idPoly3d);

                            idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dTOE, strLayer, 3);
                            tvs[1] = new TypedValue(1000, "TOE");
                            idPoly3d.setXData(tvs, "PL");
                            objEntIDs.Add(idPoly3d);

                            idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dTOP, strLayer, 4);
                            tvs[1] = new TypedValue(1000, "TOP");
                            idPoly3d.setXData(tvs, "PL");
                            objEntIDs.Add(idPoly3d);

                            idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dPL, strLayer, 5);
                            tvs[1] = new TypedValue(1000, "PL");
                            idPoly3d.setXData(tvs, "PL");
                            objEntIDs.Add(idPoly3d);

                            ObjectIdCollection objEntEndIDs = wdp.makeEndBrklinesORG(strLayer, objEntIDs, false);

                            bool exists = false;

                            if (boolMakeTempSurface == false)
                            {
                                TinSurface objSurfaceCPNT = Surf.getTinSurface("CPNT-ON", out exists);

                                objSurfaceCPNT.BreaklinesDefinition.AddStandardBreaklines(objEntIDs, 1, 0, 0, 0);
                                objSurfaceCPNT.BreaklinesDefinition.AddStandardBreaklines(objEntEndIDs, 1, 0, 0, 0);


                            }
                            else
                            {
                                TinSurface objSurfaceTemp = Surf.getTinSurface("TEMP", out exists);
                                objSurfaceTemp.BreaklinesDefinition.AddStandardBreaklines(objEntIDs, 1, 0, 0, 0);
                                objSurfaceTemp.BreaklinesDefinition.AddStandardBreaklines(objEntEndIDs, 1, 0, 0, 0);

                            }

                        }
                        catch (Autodesk.AutoCAD.Runtime.Exception ex)
                        {
                            Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog(ex.ToString());
                        }

                        TR.Commit();

                    }
                }

            }
            catch (Autodesk.AutoCAD.Runtime.Exception )
            {
                MessageBox.Show("LocateCurb");
                return false;
            }
            return success;
        }

    }
}
