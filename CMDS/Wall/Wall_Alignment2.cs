using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using Math = Base_Tools45.Math;
using wd = Wall.Wall_Design;
using wdp = Wall.Wall_DesignProfile;
namespace Wall
{
    public static class Wall_Alignment2
    {
        static Wall_Form.frmWall1 fWall1 = Wall_Forms.wForms.fWall1;
        const double PI = System.Math.PI;

        public static ObjectId
        Create_Align_Profile_By3dPoly2b2c(Alignment objAlignPL, string strName, string strNameAlign, ObjectId idLayer, ObjectId idPoly3dRF){

            bool boolBeg = false;
            bool boolEnd = false;

            ObjectId idProfileStyle = Prof_Style.getProfileStyle("WALL");
            ObjectId idStyleLabelSet = Prof_Style.getProfileLabelSetStyle("WALL");
            //ObjectId idProfileStyle = Prof_Style.getProfileStyle("Standard");
            //ObjectId idStyleLabelSet = Prof_Style.getProfileLabelSetStyle("Standard");

            List<Point3d> pnt3dsPoly3d = idPoly3dRF.getCoordinates3dList();

            ObjectId idPoly2dRF = idPoly3dRF.toPolyline(idPoly3dRF.getLayer());
            Point3d pnt3dBegRF = idPoly2dRF.getBegPnt();
            Point3d pnt3dEndRF = idPoly2dRF.getEndPnt();

            double easting = 0, northing = 0;

            objAlignPL.PointLocation(objAlignPL.StartingStation, 0.0, ref easting, ref northing);
            Point3d pnt3dBegWALL = new Point3d(easting, northing, 0);

            objAlignPL.PointLocation(Math.roundDown2(objAlignPL.EndingStation), 0.0, ref easting, ref northing);
            Point3d pnt3dEndWALL = new Point3d(easting, northing, 0);

            List<Point3d> pnts3d = new List<Point3d>{pnt3dBegWALL, pnt3dEndWALL};

            double dblAngle = 0;
            dblAngle = pnt3dBegWALL.getDirection(pnt3dEndWALL);

            double dblDistBeg = 0;
            dblDistBeg = Geom.getPerpDistToLine(pnt3dBegWALL, pnt3dEndWALL, pnt3dBegRF);

            double dblDistEnd = 0;
            dblDistEnd = Geom.getPerpDistToLine(pnt3dBegWALL, pnt3dEndWALL, pnt3dEndRF);
            double dblStaBegRef = 0, dblOffBegRef = 0;

            if (dblDistBeg > dblDistEnd)
            {
                idPoly2dRF.reversePolyX();

                pnt3dBegRF = idPoly2dRF.getBegPnt();
                pnt3dEndRF = idPoly2dRF.getEndPnt();

                dblDistBeg = Geom.getPerpDistToLine(pnt3dBegWALL, pnt3dEndWALL, pnt3dBegRF);
                dblDistEnd = Geom.getPerpDistToLine(pnt3dBegWALL, pnt3dEndWALL, pnt3dEndRF);

            }


            if (dblDistBeg < 0)
            {
                boolBeg = true;
                pnt3dBegWALL = Math.traverse(pnt3dBegWALL, dblAngle - PI, dblDistBeg * -1 + 10);
                pnts3d.Insert(0, pnt3dBegWALL);
            }


            if (dblDistEnd > objAlignPL.Length + 10)
            {
                boolEnd = true;
                pnt3dEndWALL = Math.traverse(pnt3dEndWALL, dblAngle, objAlignPL.Length - dblDistEnd + 10);
                pnts3d.Add(pnt3dEndWALL);
            }

            Profile objProfile = null;

            string strAlignName = objAlignPL.Name;
            string strLayer = objAlignPL.Layer;
            idLayer = Layer.manageLayers(strLayer);


            if (boolBeg || boolEnd)
            {
                Align.removeAlignment(strAlignName);
                ObjectId idPoly2dWALL = Draw.addPoly(pnts3d, strLayer);

                objAlignPL = Align.addAlignmentFromPoly(strAlignName, strLayer, idPoly2dWALL, "Standard", "Standard", true);

                try
                {
                    objAlignPL.StationOffset(pnt3dBegRF.X, pnt3dBegRF.Y, ref dblStaBegRef, ref dblOffBegRef);
                }
                catch (Autodesk.Civil.PointNotOnEntityException )
                {
                    dblStaBegRef = 0.0;
                }

                objAlignPL.PointLocation(dblStaBegRef, 0.0, ref easting, ref northing);

                Point2d pnt2dRef = new Point2d(easting, northing);
                objAlignPL.ReferencePoint = pnt2dRef;

                fWall1.ACTIVEALIGN = objAlignPL;

                bool exists = false;
                TinSurface objSurfaceEXIST = Surf.getTinSurface("EXIST", out exists);

                ObjectId idAlignStyle = Align_Style.getAlignmentStyle("Standard");
                ObjectId idAlignLabelSetStyle = Align_Style.getAlignmentLabelSetStyle("Standard");

                using (BaseObjs._acadDoc.LockDocument())
                {
                    using (Transaction TR = BaseObjs.startTransactionDb())
                    {

                        objProfile = Prof.addProfileBySurface("EXIST", objAlignPL.ObjectId, objSurfaceEXIST.ObjectId, idLayer, idProfileStyle, idStyleLabelSet);

                        TR.Commit();

                    }
                }

            }

            Alignment objAlignRF = Align.addAlignmentFromPoly(strNameAlign, strLayer, idPoly2dRF, "Standard", "Standard", true);

            objAlignRF.ReferencePointStation = 1000.0;

            double dblStation = 0;
            double dblOffset = 0;
            ObjectId idAlignRF = ObjectId.Null;
            try
            {
                idAlignRF.getAlignStaOffset(pnt3dsPoly3d[0], ref dblStation, ref dblOffset);
            }
            catch (Autodesk.Civil.PointNotOnEntityException )
            {
                dblStation = 0.0;
            }


            if (dblStation != objAlignRF.StartingStation)
            {
                idPoly3dRF.reversePolyX();
                pnt3dsPoly3d = idPoly3dRF.getCoordinates3dList();

            }

            using (BaseObjs._acadDoc.LockDocument())
            {
                using (Transaction TR = BaseObjs.startTransactionDb())
                {

                    objProfile = Prof.addProfileByLayout(strName, objAlignRF.ObjectId, idLayer, idProfileStyle, idStyleLabelSet);

                    double dblElev = 0;


                    for (short i = 0; i <= pnt3dsPoly3d.Count - 1; i++)
                    {
                        try
                        {
                            idAlignRF.getAlignStaOffset(pnt3dsPoly3d[i], ref dblStation, ref dblOffset);
                        }
                        catch (Autodesk.Civil.PointNotOnEntityException )
                        {
                            dblStation = 0.0;
                        }
                        dblElev = pnt3dsPoly3d[i].Z;

                        objProfile.PVIs.AddPVI(dblStation, dblElev);

                    }

                    TR.Commit();

                }
            }

            return idAlignRF;

        }

        public static void
        CreateProfileByDesign2b(Alignment objAlignPL, Alignment objAlignRF){

            Profile objProfileDES = null;

            Profile objprofilePL = Prof.getProfile(objAlignPL.Name, "EXIST");
            Profile objProfileRF = Prof.getProfile(objAlignRF.Name, "CPNT");

            double XT = 0;
            double X1 = 0;
            double X2 = 0;

            double X0 = Convert.ToDouble(fWall1.tbx_X0.Text);
            double S0 = Convert.ToDouble(fWall1.tbx_S0.Text);

            double CF = Convert.ToDouble(fWall1.tbx_CF.Text);

            double B1 = Convert.ToDouble(fWall1.tbx_B1.Text);
            double S1 = Convert.ToDouble(fWall1.tbx_S1.Text);

            double SG = Convert.ToDouble(fWall1.tbx_SG.Text);

            double B2 = Convert.ToDouble(fWall1.tbx_B2.Text);
            double S2 = Convert.ToDouble(fWall1.tbx_S2.Text);

            double dblOffsetPL = 0;

            double dblEasting = 0;
            double dblNorthing = 0;

            List<Point3d> pnts3dX0 = new List<Point3d>();
            List<Point3d> pnts3dFL = new List<Point3d>();
            List<Point3d> pnts3dTC = new List<Point3d>();
            List<Point3d> pnts3dTOE = new List<Point3d>();
            List<Point3d> pnts3dTS = new List<Point3d>();
            List<Point3d> pnts3dTOP = new List<Point3d>();

            List<Point3d> pnts3dWT0 = new List<Point3d>();
            List<Point3d> pnts3dWB0 = new List<Point3d>();
            List<Point3d> pnts3dWTB = new List<Point3d>();
            List<Point3d> pnts3dWBB = new List<Point3d>();

            short intSign = 0;

            double dblElevRF = 0;
            double dblElevFL = 0;
            double dblElevTC = 0;
            double dblElevTOE = 0;
            double dblElevTS = 0;
            double dblElevTOP = 0;

            double dblElevWT0 = 0;
            double dblElevWB0 = 0;
            double dblElevWTB = 0;
            double dblElevWBB = 0;
            double dblElevPL = 0;

            double dblWallWidth = 10 / 12;

            double dblStationRF = 0;
            double dblStationPL = 0;

            ObjectId idPoly3d = ObjectId.Null;

            Point3d pnt3d = default(Point3d);

            string strLayerName = null;

            bool boolDesLow = false;
            bool boolDesHigh = false;

            strLayerName = "PROFILE-CPNT";
            ObjectId idLayer = Layer.manageLayers(strLayerName);

            objAlignRF.PointLocation(objAlignRF.StartingStation, 0, ref dblEasting, ref dblNorthing);

            try
            {
                objAlignPL.StationOffset(dblEasting, dblNorthing, ref dblStationPL, ref dblOffsetPL);
            }
            catch (Autodesk.Civil.PointNotOnEntityException )
            {
                dblStationPL = 0.0;
            }

            if (dblOffsetPL < 0)
            {
                intSign = -1;
                dblOffsetPL = dblOffsetPL * -1;
            }
            else
            {
                intSign = 1;
            }

            //----------------------------------------------------------------------------------------------------------------------------------------------------------------

            //Dim varPNT_DATA_FINAL() As PNT_DATA = getPNT_DATA_FINAL(objAlignPL, objAlignRF)
            List<double> dblStationsFinal = wd.getPNT_DATA_FINAL(objAlignPL, objAlignRF);
            fWall1.Stations = dblStationsFinal;

            //----------------------------------------------------------------------------------------------------------------------------------------------------------------


            if ((fWall1.Stations == null))
            {
                return;


            }
            else
            {
                objProfileRF = Prof.getProfile(objAlignRF.Name, "CPNT");
                objprofilePL = Prof.getProfile(objAlignPL.Name, "EXIST");

            }

            //-------CREATE PROFILE FOR DESIGN SURFACE AT WALL/PL ALIGNMENT

            try
            {
                Prof.removeProfile(objAlignPL.ObjectId, "CPNT");
                //Dim idProfileStyle As ObjectId = Prof_Style.getProfileStyle("WALL")
                //Dim idStyleLabelSet As ObjectId = Prof_Style.getProfileLabelSetStyle("WALL")
                ObjectId idProfileStyle = Prof_Style.getProfileStyle("Standard");
                ObjectId idStyleLabelSet = Prof_Style.getProfileLabelSetStyle("Standard");

                using (BaseObjs._acadDoc.LockDocument())
                {
                    using (Transaction TR = BaseObjs.startTransactionDb())
                    {

                        objProfileDES = Prof.addProfileByLayout("CPNT", objAlignPL.ObjectId, idLayer, idProfileStyle, idStyleLabelSet);

                        TR.Commit();

                    }
                }


            }
            catch (Autodesk.AutoCAD.Runtime.Exception )
            {
            }

            double dblAngOFF = 0;
            double dblAngWALL = 0;
            double dblSkew = 0;

            Point3d pnt3dBeg = default(Point3d);
            Point3d pnt3dEnd = default(Point3d);

            pnt3dBeg = objAlignRF.StartPoint;
            pnt3dEnd = objAlignRF.EndPoint;
            dblAngOFF = Measure.getAzRadians(pnt3dBeg, pnt3dEnd);

            pnt3dBeg = objAlignPL.StartPoint;
            pnt3dEnd = objAlignPL.EndPoint;
            dblAngWALL = Measure.getAzRadians(pnt3dBeg, pnt3dEnd);

            dblSkew = dblAngWALL - dblAngOFF;

            bool boolStart = false;
            bool boolDone = false;


            for (int i = 0; i < dblStationsFinal.Count; i++)
            {
                dblStationRF = Math.roundDown1(dblStationsFinal[i]);
                //CURRENT STATION ON RF

                objAlignRF.PointLocation(dblStationRF, 0.0, ref dblEasting, ref dblNorthing);

                try
                {
                    objAlignPL.StationOffset(dblEasting, dblNorthing, ref dblStationPL, ref dblOffsetPL);
                    // CORRESPONDING STATION ON PL
                }
                catch (Autodesk.Civil.PointNotOnEntityException )
                {
                    dblStationPL = 0.0;
                }


                if (System.Math.Round(dblStationPL, 1) >= System.Math.Round(objAlignPL.StartingStation, 1))
                {
                    boolStart = true;

                }


                if (System.Math.Round(dblStationPL, 1) <= System.Math.Round(objAlignPL.EndingStation, 1))
                {
                    boolDone = false;

                }


                if (boolStart == true & boolDone == false)
                {
                    dblElevRF = objProfileRF.ElevationAt(dblStationRF);
                    //elevation on RF at current RF station
                    dblElevFL = dblElevRF + (X0 - 0.15) * S0;
                    dblElevTC = dblElevFL + CF / 12;

                    dblElevPL = objprofilePL.ElevationAt(dblStationPL);


                    if (dblElevRF > 0 & dblElevPL > 0)
                    {
                        dblElevTS = dblElevPL - (B2 * S2);

                        objAlignRF.PointLocation(dblStationRF, X0 - 0.15, ref dblEasting, ref dblNorthing);
                        pnt3d = new Point3d(dblEasting, dblNorthing, dblElevFL);
                        pnts3dFL.Add(pnt3d);

                        objAlignRF.PointLocation(dblStationRF, X0, ref dblEasting, ref dblNorthing);
                        pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTC);
                        pnts3dTC.Add(pnt3d);

                        if (dblElevTS - dblElevTC > 0)
                        {
                            boolDesLow = true;
                            boolDesHigh = false;
                        }
                        else if (dblElevTC - dblElevTS > 0)
                        {
                            boolDesHigh = true;
                            boolDesLow = false;
                        }


                        if (boolDesLow)
                        {
                            objAlignRF.PointLocation(dblStationRF, B1, ref dblEasting, ref dblNorthing);
                            //point location at back of B1

                            try
                            {
                                objAlignPL.StationOffset(dblEasting, dblNorthing, ref dblStationPL, ref dblOffsetPL);
                            }
                            catch (Autodesk.Civil.PointNotOnEntityException )
                            {
                                dblStationPL = 0.0;
                            }


                            if (dblStationPL == 0)
                            {
                                pnt3dBeg = new Point3d(dblEasting, dblNorthing, 0.0);
                                pnt3dEnd = objAlignPL.EndPoint;

                                dblOffsetPL = pnt3dBeg.getDistance(pnt3dEnd);

                            }

                            XT = System.Math.Abs(dblOffsetPL) - X0;
                            X2 = ((dblElevTC - dblElevPL) + (XT - B2) * S1 + (B2 * S2)) / (S1 - SG);
                            //slope width


                            if (X2 >= 0)
                            {
                                X1 = XT - X2;


                                if (X1 >= 0)
                                {
                                    dblElevTOE = dblElevTC + (S1 * B1) + (X1 * S1);

                                    objAlignPL.PointLocation(dblStationPL, (X2 + B2) * intSign, ref dblEasting, ref dblNorthing);
                                    //point perp to PL
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOE);
                                    pnts3dTOE.Add(pnt3d);

                                    dblElevTS = dblElevTOE + X2 * SG;

                                    objAlignPL.PointLocation(dblStationPL, (B2) * intSign, ref dblEasting, ref dblNorthing);
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTS);
                                    pnts3dTS.Add(pnt3d);

                                    dblElevTOP = dblElevTS + B2 * S2;

                                    objAlignPL.PointLocation(dblStationPL, 0.0, ref dblEasting, ref dblNorthing);
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOP);
                                    pnts3dTOP.Add(pnt3d);


                                    try
                                    {
                                        objProfileDES.PVIs.AddPVI(dblStationPL, dblElevTOP);


                                    }
                                    catch (Autodesk.AutoCAD.Runtime.Exception )
                                    {
                                    }


                                }
                                else
                                {
                                    //here is where we put the wall at the limit of pavement or at back of B1

                                    X1 = B1;
                                    X2 = XT - X1;

                                    dblElevTOE = dblElevTC + X1 * S1;

                                    objAlignRF.PointLocation(dblStationRF, X1 + 0.1, ref dblEasting, ref dblNorthing);
                                    //point perp to RF
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOE);
                                    pnts3dTOE.Add(pnt3d);

                                    dblElevTS = dblElevTOE + X2 * SG;

                                    objAlignPL.PointLocation(dblStationPL, B2 * intSign, ref dblEasting, ref dblNorthing);
                                    //point perp to PL
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTS);
                                    pnts3dTS.Add(pnt3d);

                                    dblElevTOP = dblElevTS + B2 * S2;

                                    objAlignPL.PointLocation(dblStationPL, 0.0, ref dblEasting, ref dblNorthing);
                                    //point perp to PL
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOP);
                                    pnts3dTOP.Add(pnt3d);

                                    //wall location

                                    dblElevWT0 = dblElevPL + 1.0;

                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevWT0);
                                    pnts3dWT0.Add(pnt3d);

                                    dblElevWB0 = dblElevTOP - 1.0;

                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevWB0);
                                    pnts3dWB0.Add(pnt3d);

                                    dblElevWTB = dblElevWT0;

                                    objAlignPL.PointLocation(dblStationRF, dblWallWidth * intSign * -1, ref dblEasting, ref dblNorthing);
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevWT0);
                                    pnts3dWTB.Add(pnt3d);

                                    dblElevWBB = dblElevWB0;
                                    pnt3d = new Point3d(dblEasting, dblNorthing, dblElevWBB);
                                    pnts3dWBB.Add(pnt3d);


                                    try
                                    {
                                        objProfileDES.PVIs.AddPVI(dblStationRF, dblElevTOP);


                                    }
                                    catch (Autodesk.AutoCAD.Runtime.Exception )
                                    {
                                    }

                                }
                            }

                            //wall at pl
                        }
                        else if (boolDesHigh)
                        {

                            X1 = B1;
                            X2 = XT - X1;

                            dblElevTOE = dblElevTC + X1 * S1;

                            objAlignRF.PointLocation(dblStationRF, X1 + 0.1, ref dblEasting, ref dblNorthing);
                            //point perp to OFF at back of bench
                            pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOE);
                            pnts3dTOE.Add(pnt3d);

                            dblElevTS = dblElevTOE + X2 * S1;

                            objAlignPL.PointLocation(dblStationPL, B2 * intSign, ref dblEasting, ref dblNorthing);
                            pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTS);
                            pnts3dTS.Add(pnt3d);

                            dblElevTOP = dblElevTS + B2 * S2;

                            objAlignPL.PointLocation(dblStationRF, 0, ref dblEasting, ref dblNorthing);
                            pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOP);
                            pnts3dTOP.Add(pnt3d);

                            //WALL Limits

                            dblElevWT0 = dblElevTOP + 1.0;
                            //wall top 0 offset
                            objAlignPL.PointLocation(dblStationRF, 0, ref dblEasting, ref dblNorthing);
                            pnt3d = new Point3d(dblEasting, dblNorthing, dblElevWT0);
                            pnts3dWT0.Add(pnt3d);

                            dblElevWB0 = dblElevPL - 1.0;
                            //wall bottom 0.1 offset

                            objAlignPL.PointLocation(dblStationRF, 0.1 * intSign, ref dblEasting, ref dblNorthing);
                            pnt3d = new Point3d(dblEasting, dblNorthing, dblElevWB0);
                            pnts3dWB0.Add(pnt3d);

                            dblElevWTB = dblElevWT0;
                            //wall top WallWidth offset

                            objAlignPL.PointLocation(dblStationRF, dblWallWidth * intSign * -1, ref dblEasting, ref dblNorthing);
                            pnt3d = new Point3d(dblEasting, dblNorthing, dblElevWT0);
                            pnts3dWTB.Add(pnt3d);

                            dblElevWBB = dblElevWB0;
                            //Wall bottom WallWidth + 0.1 offset

                            objAlignPL.PointLocation(dblStationRF, (dblWallWidth + 0.1) * intSign * -1, ref dblEasting, ref dblNorthing);
                            pnt3d = new Point3d(dblEasting, dblNorthing, dblElevWBB);
                            pnts3dWBB.Add(pnt3d);


                            try
                            {
                                objProfileDES.PVIs.AddPVI(dblStationRF, dblElevTOP);


                            }
                            catch (Autodesk.AutoCAD.Runtime.Exception )
                            {
                            }

                        }
                    }
                }
            }


            try
            {
                ObjectIdCollection objEntIDs = new ObjectIdCollection();
                ObjectIdCollection objEntWallIDs = new ObjectIdCollection();

                TypedValue[] tvs = new TypedValue[2];
                tvs[0] = new TypedValue(1001, "WALL");

                idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dFL, "CPNT-BRKLINE", 1);
                tvs[1] = new TypedValue(1000, "FL");
                idPoly3d.setXData(tvs, "WALL");
                objEntIDs.Add(idPoly3d);


                idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dTC, "CPNT-BRKLINE", 2);
                tvs[1] = new TypedValue(1000, "TC");
                idPoly3d.setXData(tvs, "WALL");
                objEntIDs.Add(idPoly3d);

                idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dTOE, "CPNT-BRKLINE", 3);
                tvs[1] = new TypedValue(1000, "TOE");
                idPoly3d.setXData(tvs, "WALL");
                objEntIDs.Add(idPoly3d);

                idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dTS, "CPNT-BRKLINE", 4);
                tvs[1] = new TypedValue(1000, "TS");
                idPoly3d.setXData(tvs, "WALL");
                objEntIDs.Add(idPoly3d);

                idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dTOP, "CPNT-BRKLINE", 5);
                tvs[1] = new TypedValue(1000, "TOP");
                idPoly3d.setXData(tvs, "WALL");
                objEntIDs.Add(idPoly3d);

                string strLayer = string.Format("{0}-BRKLINE", objAlignPL.Name);
                Layer.manageLayers(strLayer);

                idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dWB0, strLayer, 11);
                tvs[1] = new TypedValue(1000, "WB0");
                idPoly3d.setXData(tvs, "WALL");
                objEntWallIDs.Add(idPoly3d);

                idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dWT0, strLayer, 170);
                tvs[1] = new TypedValue(1000, "WT0");
                idPoly3d.setXData(tvs, "WALL");
                objEntWallIDs.Add(idPoly3d);

                idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dWTB, strLayer, 170);
                tvs[1] = new TypedValue(1000, "WTB");
                idPoly3d.setXData(tvs, "WALL");
                objEntWallIDs.Add(idPoly3d);

                idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dWBB, strLayer, 11);
                tvs[1] = new TypedValue(1000, "WBB");
                idPoly3d.setXData(tvs, "WALL");
                objEntWallIDs.Add(idPoly3d);

                ObjectIdCollection objEntEndIDs = default(ObjectIdCollection);
                objEntEndIDs = wdp.makeEndBrklinesWALL(strLayer, objEntIDs, false);

                bool exists = false;
                TinSurface objSurfaceCPNT = Surf.getTinSurface("CPNT-ON", out exists);

                objSurfaceCPNT.BreaklinesDefinition.AddStandardBreaklines(objEntIDs, 0, 0, 0, 0);
                objSurfaceCPNT.BreaklinesDefinition.AddStandardBreaklines(objEntEndIDs, 0, 0, 0, 0);

                TinSurface objSurfaceWall = Surf.getTinSurface(objAlignPL.Name, out exists);
                objSurfaceWall.BreaklinesDefinition.AddStandardBreaklines(objEntWallIDs, 0, 0, 0, 0);

                objEntEndIDs = wdp.makeEndBrklinesWALL(strLayer, objEntWallIDs, true);
                objSurfaceWall.BreaklinesDefinition.AddStandardBreaklines(objEntEndIDs, 0, 0, 0, 0);


            }
            catch (Autodesk.AutoCAD.Runtime.Exception )
            {
            }

            objProfileDES.Layer = strLayerName;

        }

        //public static Alignment
        //makeAlignWall(string strAlignName, Polyline objLWPline)
        //{
        //    Layer.manageLayers(strAlignName);

        //    Align.removeAlignment(strAlignName);

        //    Alignment align = Align.addAlignmentFromPoly(strAlignName, strAlignName, objLWPline.ObjectId, "Standard", "Standard", true);
        //    align.ReferencePointStation = fWall1.ACTIVEALIGN.ReferencePointStation;

        //    if (align == null)
        //    {
        //        Application.ShowAlertDialog("Offset Alignment failed");
        //    }

        //    return align;
        //}

    }
}
