using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using wd = Wall.Wall_Design;

namespace Wall {
	public static class Wall_DesignProfile {

		static Wall_Form.frmWall1 fWall1 = Wall_Forms.wForms.fWall1;

		public static ObjectId
		CreateProfileBySurface(string strName, ObjectId idAlign, double dblAlignOffset) {


			string strAlignName = Align.getAlignName(idAlign);

			//Dim idProfileStyle As ObjectId = Prof_Style.getProfileStyle("WALL")
			//Dim idStyleLabelSet As ObjectId = Prof_Style.getProfileLabelSetStyle("WALL")

			ObjectId idProfileStyle = Prof_Style.getProfileStyle("Standard");
			ObjectId idStyleLabelSet = Prof_Style.getProfileLabelSetStyle("Standard");

			string strLayerName = string.Format("PROFILE-{0}", strAlignName);

			Alignment objAlignOff = null;
			if (dblAlignOffset != 0)
			{
				ObjectId idPoly = Align.getAlignmentPoly(idAlign);
				ObjectId idPolyOff = Base_Tools45.Draw.addPolyOffset(idPoly, dblAlignOffset);

				strAlignName = string.Format("{0}_{1}_OFF", strAlignName, dblAlignOffset);
				objAlignOff = Align.addAlignmentFromPoly(strAlignName, strLayerName, idPolyOff, "Thienes_Proposed", "Thienes_Proposed", true);
			}

			string strSurface = "";
			if (strName == "CPNT")
			{
				strSurface = "CPNT-ON";
			}
			else
			{
				strSurface = strName;
			}

			TinSurface objSurface = null;
			try
			{
				bool exists = false;
				objSurface = Surf.getTinSurface(strSurface, out exists);
			}
			catch (Autodesk.AutoCAD.Runtime.Exception )
			{
				System.Windows.Forms.MessageBox.Show(strSurface + " is not available.");
			}

			Profile objProfile = null;


			if (dblAlignOffset != 0)
			{
				Prof.removeProfile(objAlignOff.ObjectId, strName);
			}
			else
			{
				Prof.removeProfile(idAlign, strName);
			}
			ObjectId idStyle = default(ObjectId);
			ObjectId idLabel = default(ObjectId);

			using (BaseObjs._acadDoc.LockDocument())
			{
				using (Transaction TR = BaseObjs.startTransactionDb())
				{
					ObjectId idSurf = objSurface.ObjectId;
					ObjectId idLayer = Layer.manageLayers(strLayerName);
					string mess = string.Format("WALL,{0},{1},{2},{3},{4}", idAlign, idSurf, idLayer, idStyle, idLabel);
					BaseObjs.write(mess);

					try
					{
						objProfile = Prof.addProfileBySurface(strName, idAlign, idSurf, idLayer, idProfileStyle, idStyleLabelSet);
						objProfile.Layer = strLayerName;

					}
					catch (Autodesk.AutoCAD.Runtime.Exception exp)
					{
						Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog(exp.Message);
					}

					TR.Commit();

				}
			}


			if (dblAlignOffset != 0)
			{
				ProfilePVI objPvi = null;
				List<PNT_DATA> varPnt_Data = new List<PNT_DATA>();
				PNT_DATA vPnt_Data = new PNT_DATA();

				for (int i = 0; i <= objProfile.PVIs.Count; i++)
				{
					objPvi = objProfile.PVIs[i];
					vPnt_Data.STA = objPvi.Station;
					vPnt_Data.z = objPvi.Elevation;
					vPnt_Data.OFFSET = dblAlignOffset;
					varPnt_Data.Add(vPnt_Data);
				}

				fWall1.PNTSDESIGN = varPnt_Data;
				return objAlignOff.ObjectId;

			}
			return idAlign;
		}

		public static void
		CreateProfileByDesign2c(Alignment objAlignPL, Alignment objAlignRF, List<PNT_DATA> varPNT_DATA) {


			short i = 0;

			Profile objProfilePL = default(Profile);
			Profile objProfileRF = default(Profile);

			double XT = 0;
			double X1 = 0;
			double Xo = 0;

			double X0 = Convert.ToDouble(fWall1.tbx2c_X0.Text);

			double B1 = Convert.ToDouble(fWall1.tbx2c_B1.Text);
			double B2 = Convert.ToDouble(fWall1.tbx2c_B2.Text);

			double S0 = Convert.ToDouble(fWall1.tbx2c_S0.Text);
			double S1 = Convert.ToDouble(fWall1.tbx2c_S1.Text);
			double S2 = Convert.ToDouble(fWall1.tbx2c_S2.Text);
			double SG = Convert.ToDouble(fWall1.tbx2c_SG.Text);

			double CF = Convert.ToDouble(fWall1.tbx2c_CF.Text);

			double dblOffsetPL = 0.0;

			double dblEasting = 0.0;
			double dblNorthing = 0.0;

			List<Point3d> pnts3dRF = new List<Point3d>();
			List<Point3d> pnts3dFL = new List<Point3d>();
			List<Point3d> pnts3dTC = new List<Point3d>();
			List<Point3d> pnts3dTOE = new List<Point3d>();
			List<Point3d> pnts3dTOP = new List<Point3d>();
			List<Point3d> pnts3dPL = new List<Point3d>();

			short intSign = 0;

			double dblElevRF = 0;
			double dblElevFL = 0;
			double dblElevTC = 0;
			double dblElevTOE = 0;
			double dblElevTOP = 0;
			double dblElevPL = 0;

			double dblElevDiff = 0;

			double dblStationRF = 0;
			double dblStationPL = 0;

			Point3d pnt3d = default(Point3d);
			ObjectId idPoly3d = ObjectId.Null;

			string strLayerName = null;

			strLayerName = "PROFILE-CPNT";
			Layer.manageLayers(strLayerName);

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
			}
			else
			{
				intSign = 1;
			}

			//----------------------------------------------------------------------------------------------------------------------------------------------------------------

			//Dim varPNT_DATA_FINAL() As PNT_DATA = getPNT_DATA_FINAL(objAlignPL, objAlignRF)
			List<double> dblStationsFinal = new List<double>();
			dblStationsFinal = wd.getPNT_DATA_FINAL(objAlignPL, objAlignRF);
			fWall1.Stations = dblStationsFinal;
			//----------------------------------------------------------------------------------------------------------------------------------------------------------------


			if ((fWall1.Stations == null))
			{
				return;


			}
			else
			{
				objProfileRF = Prof.getProfile(objAlignRF.ObjectId, "CPNT");
				objProfilePL = Prof.getProfile(objAlignPL.ObjectId, "EXIST");

			}

			//-------CREATE PROFILE FOR DESIGN SURFACE AT WALL/PL ALIGNMENT

			bool boolStart = false;
			bool boolDone = false;


			for (i = 0; i <= dblStationsFinal.Count - 1; i++)
			{
				dblStationRF = dblStationsFinal[i];
				//CURRENT STATION ON REF
				Debug.Print(dblStationRF.ToString());

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
					dblElevRF = objProfileRF.ElevationAt(dblStationRF);
					//elevation on REF at current REF station

					dblElevPL = objProfilePL.ElevationAt(dblStationPL);
					//elevation on PL at PL station corresponding to REF station

					//valid surface elevations for both REF and PL
					if (dblElevRF > 0 & dblElevPL > 0)
					{

						dblElevTOP = dblElevPL - (B2 * S2);
						//top of slope - always sloping away from PL

						objAlignRF.PointLocation(dblStationRF, 0.0, ref dblEasting, ref dblNorthing);
						pnt3d = new Point3d(dblEasting, dblNorthing, dblElevRF);
						pnts3dRF.Add(pnt3d);

						dblElevDiff = dblElevRF - dblElevPL;

						objAlignRF.PointLocation(dblStationRF, 0.0, ref dblEasting, ref dblNorthing);
						//point location at REF align

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

							Point3d pnt3dBeg = new Point3d(dblEasting, dblNorthing, 0.0);
							//point location at REF align

							dblStationPL = objAlignPL.EndingStation;
							objAlignPL.PointLocation(objAlignPL.EndingStation, 0, ref dblEasting, ref dblNorthing);

							Point3d pnt3dEnd = new Point3d(dblEasting, dblNorthing, 0.0);
							//point location at end PL align

							dblOffsetPL = pnt3dBeg.getDistance(pnt3dEnd);
							//distance from REF to PL

						}

						XT = System.Math.Abs(dblOffsetPL);

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

						X1 = ((dblElevPL - dblElevRF) - (XT * S0) - B1 * (S1 - S0) - B2 * (S2 - S0) - CF / 12) / (SLOPE - S0);

						if (X1 < 0)
						{
							SLOPE = SLOPE * -1;
							X1 = ((dblElevPL - dblElevRF) - (XT * S0) - B1 * (S1 - S0) - B2 * (S2 - S0) - CF / 12) / (SLOPE - S0);

						}

						Xo = XT - X1 - B1 - B2;


						if (X0 > Xo)
						{
						}

						dblElevFL = dblElevRF + X0 * S0;
						objAlignPL.PointLocation(dblStationPL, (XT - X0 + 0.15) * intSign, ref dblEasting, ref dblNorthing);
						//point perp to PL
						pnt3d = new Point3d(dblEasting, dblNorthing, dblElevFL);
						pnts3dFL.Add(pnt3d);

						dblElevTC = dblElevFL + CF / 12;
						objAlignPL.PointLocation(dblStationPL, (XT - X0) * intSign, ref dblEasting, ref dblNorthing);
						//point perp to PL
						pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTC);
						pnts3dTC.Add(pnt3d);

						dblElevTOE = (dblElevTC + B1 * S1);
						objAlignPL.PointLocation(dblStationPL, (XT - X0 - B1) * intSign, ref dblEasting, ref dblNorthing);
						//point perp to PL
						pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOE);
						pnts3dTOE.Add(pnt3d);

						dblElevTOP = dblElevTOE + X1 * SLOPE;
						objAlignPL.PointLocation(dblStationPL, (B2) * intSign, ref dblEasting, ref dblNorthing);
						pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOP);
						pnts3dTOP.Add(pnt3d);

						dblElevPL = dblElevTOP + B2 * S2;
						objAlignPL.PointLocation(dblStationPL, 0.0, ref dblEasting, ref dblNorthing);
						pnt3d = new Point3d(dblEasting, dblNorthing, dblElevPL);
						pnts3dPL.Add(pnt3d);

					}
				}
			}


			try
			{
				ObjectIdCollection objEntIDs = new ObjectIdCollection();

				TypedValue[] tvs = new TypedValue[2];
				tvs[0] = new TypedValue(1001, "PL");


				idPoly3d = Draw.addPoly3d(pnts3dRF, color: 6);
				tvs[1] = new TypedValue(1000, "REF");
				idPoly3d.setXData(tvs, "PL");
				objEntIDs.Add(idPoly3d);

				idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dFL, "CPNT-BRKLINE", 1);
				tvs[1] = new TypedValue(1000, "FL");
				idPoly3d.setXData(tvs, "PL");
				objEntIDs.Add(idPoly3d);

				idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dTC, "CPNT-BRKLINE", 2);
				tvs[1] = new TypedValue(1000, "TC");
				idPoly3d.setXData(tvs, "PL");
				objEntIDs.Add(idPoly3d);

				idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dTOE, "CPNT-BRKLINE", 3);
				tvs[1] = new TypedValue(1000, "TOE");
				idPoly3d.setXData(tvs, "PL");
				objEntIDs.Add(idPoly3d);

				idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dTOP, "CPNT-BRKLINE", 4);
				tvs[1] = new TypedValue(1000, "TOP");
				idPoly3d.setXData(tvs, "PL");
				objEntIDs.Add(idPoly3d);

				idPoly3d = Base_Tools45.Draw.addPoly3d(pnts3dPL, "CPNT-BRKLINE", 5);
				tvs[1] = new TypedValue(1000, "PL");
				idPoly3d.setXData(tvs, "PL");
				objEntIDs.Add(idPoly3d);

				string strLayer = string.Format("{0}-BRKLINE", objAlignPL.Name);
				Layer.manageLayers(strLayer);

				ObjectIdCollection objEntEndIDs = new ObjectIdCollection();

				objEntEndIDs = makeEndBrklinesORG(strLayer, objEntIDs, false);

				bool exists = false;
				TinSurface objSurfaceCPNT = Surf.getTinSurface("CPNT-ON", out exists);

				objSurfaceCPNT.BreaklinesDefinition.AddStandardBreaklines(objEntIDs, 0, 0, 0, 0);
				objSurfaceCPNT.BreaklinesDefinition.AddStandardBreaklines(objEntEndIDs, 0, 0, 0, 0);


			}
			catch (Autodesk.AutoCAD.Runtime.Exception )
			{
			}
		}

		public static void
		CreateProfileByDesign(string strName, Alignment objAlign) {


			List<PNT_DATA> varPNT_DATA = new List<PNT_DATA>();

			Profile objProfile = default(Profile);

			string strLayerName = null;

			short i = 0;

			Profile objProfileExist = Prof.getProfile(objAlign.ObjectId, "EXIST");

			strLayerName = "PROFILE-" + strName;
			ObjectId idLayer = Layer.manageLayers(strLayerName);

			varPNT_DATA = fWall1.PNTSDESIGN;

			Prof.removeProfile(objAlign.ObjectId, strName);
			//Dim idProfileStyle As ObjectId = Prof_Style.getProfileStyle("WALL")
			//Dim idStyleLabelSet As ObjectId = Prof_Style.getProfileLabelSetStyle("WALL")
			ObjectId idProfileStyle = Prof_Style.getProfileStyle("Standard");
			ObjectId idStyleLabelSet = Prof_Style.getProfileLabelSetStyle("Standard");

			using (BaseObjs._acadDoc.LockDocument())
			{
				using (Transaction TR = BaseObjs.startTransactionDb())
				{

					objProfile = Prof.addProfileByLayout(strName, objAlign.ObjectId, idLayer, idProfileStyle, idStyleLabelSet);

					TR.Commit();

				}
			}

			double dblBenchWidth1 = Convert.ToDouble(fWall1.tbx_B1.Text);
			double dblBenchWidth2 = Convert.ToDouble(fWall1.tbx_B2.Text);
			double dblCurbHeight = Convert.ToDouble(fWall1.tbx_CF.Text);

			double dblEasting = 0.0;
			double dblNorthing = 0.0;

			List<Point3d> pnt3dFL = new List<Point3d>();
			List<Point3d> pnt3dTC = new List<Point3d>();
			List<Point3d> pnt3dTOE = new List<Point3d>();
			List<Point3d> pnt3dTS = new List<Point3d>();
			List<Point3d> pnt3dTOP = new List<Point3d>();
			short intSign = 0;

			double dblElevFL = 0;
			double dblElevTC = 0;
			double dblElevTOE = 0;
			double dblElevTS = 0;
			double dblElevTOP = 0;

			double dblElevEXIST = 0;
			double dblElevDiff = 0;

			double dblOffset = 0;

			ObjectId idPoly3d = ObjectId.Null;

			double dblSlope = Convert.ToDouble(fWall1.tbx_SG.Text);
			Point3d pnt3d = default(Point3d);

			double X = 0;
			double X2 = 0;
			double X1 = 0;


			for (i = 0; i <= varPNT_DATA.Count - 1; i++)
			{
				dblElevFL = varPNT_DATA[i].z;
				dblElevEXIST = objProfileExist.ElevationAt(varPNT_DATA[i].STA);
				dblElevDiff = dblElevFL - dblElevEXIST;
				dblOffset = varPNT_DATA[i].OFFSET;

				if (dblOffset < 0)
				{
					intSign = -1;
					dblOffset = dblOffset * -1;
				}
				else
				{
					intSign = 1;
				}


				if (dblElevFL > 0 & dblElevEXIST > 0)
				{
					dblElevTS = dblElevEXIST - dblBenchWidth2 * 0.02;
					dblElevTC = dblElevFL + dblCurbHeight / 12;

					objAlign.PointLocation(varPNT_DATA[i].STA, dblOffset * intSign, ref dblEasting, ref dblNorthing);

					pnt3d = new Point3d(dblEasting, dblNorthing, dblElevFL);
					pnt3dFL.Add(pnt3d);

					objAlign.PointLocation(varPNT_DATA[i].STA, (dblOffset - 0.1) * intSign, ref dblEasting, ref dblNorthing);
					pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTC);
					pnt3dTC.Add(pnt3d);

					dblElevDiff = dblElevFL - dblElevEXIST;


					if(dblElevDiff < 0){
						X = dblOffset - dblBenchWidth2;
						X2 = (dblElevTS - dblElevTC - (0.02 * dblOffset)) / dblSlope;
						X1 = X - X2;


						if (X1 > dblBenchWidth1)
						{
							dblElevTOE = dblElevTC + X1 * 0.02;

							objAlign.PointLocation(varPNT_DATA[i].STA, (dblOffset - X1) * intSign, ref dblEasting, ref dblNorthing);
							pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOE);
							pnt3dTOE.Add(pnt3d);

							dblElevTS = dblElevTOE + dblSlope * X2;

							objAlign.PointLocation(varPNT_DATA[i].STA, (dblOffset - (X1 + X2)) * intSign, ref dblEasting, ref dblNorthing);
							pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTS);
							pnt3dTS.Add(pnt3d);

							dblElevTOP = dblElevTS + 0.02 * dblBenchWidth2;

							objAlign.PointLocation(varPNT_DATA[i].STA, (dblOffset - (X1 + X2 + dblBenchWidth2)) * intSign, ref dblEasting, ref dblNorthing);
							pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOP);
							pnt3dTOP.Add(pnt3d);

							objProfile.PVIs.AddPVI(varPNT_DATA[i].STA, dblElevTOP);


						}
						else
						{
							dblElevTOE = dblElevTC + dblBenchWidth1 * 0.02;

							objAlign.PointLocation(varPNT_DATA[i].STA, (dblOffset - dblBenchWidth1) * intSign, ref dblEasting, ref dblNorthing);
							pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOE);
							pnt3dTOE.Add(pnt3d);

							dblElevTS = dblElevTOE + dblSlope * dblOffset - (dblBenchWidth1 + dblBenchWidth2);

							objAlign.PointLocation(varPNT_DATA[i].STA, (dblOffset - (X1 + X2)) * intSign, ref dblEasting, ref dblNorthing);
							pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTS);
							pnt3dTS.Add(pnt3d);

							dblElevTOP = dblElevTS + 0.02 * dblBenchWidth2;

							objAlign.PointLocation(varPNT_DATA[i].STA, 0.0, ref dblEasting, ref dblNorthing);
							pnt3d = new Point3d(dblEasting, dblNorthing, dblElevTOP);
							pnt3dTOP.Add(pnt3d);

							objProfile.PVIs.AddPVI(varPNT_DATA[i].STA, dblElevTOP);

						}
						
					}else if(dblElevDiff > 0){
						dblElevTOP = dblElevTC + 0.02 * dblOffset;

						objAlign.PointLocation(varPNT_DATA[i].STA, 0.0, ref dblEasting, ref dblNorthing);
						pnt3dTOP.Add(new Point3d(dblEasting, dblNorthing, dblElevTOP));

						objProfile.PVIs.AddPVI(varPNT_DATA[i].STA, dblElevTOP);
						
					}
				}
			}


			try
			{
				ObjectIdCollection objEntIds = new ObjectIdCollection();

				idPoly3d = Base_Tools45.Draw.addPoly3d(pnt3dFL);
				objEntIds.Add(idPoly3d);

				idPoly3d = Base_Tools45.Draw.addPoly3d(pnt3dTC);
				objEntIds.Add(idPoly3d);

				idPoly3d = Base_Tools45.Draw.addPoly3d(pnt3dTOE);
				objEntIds.Add(idPoly3d);

				idPoly3d = Base_Tools45.Draw.addPoly3d(pnt3dTS);
				objEntIds.Add(idPoly3d);

				idPoly3d = Base_Tools45.Draw.addPoly3d(pnt3dTOP);
				objEntIds.Add(idPoly3d);

				bool exists = false;
				TinSurface objSurfaceCPNT = Surf.getTinSurface("CPNT-ON", out exists);

				objSurfaceCPNT.BreaklinesDefinition.AddStandardBreaklines(objEntIds, 0, 0, 0, 0);


			}
			catch (Autodesk.AutoCAD.Runtime.Exception )
			{
			}

			objProfile.Layer = strLayerName;

		}

		public static ObjectIdCollection
		makeEndBrklines(string strLayer, ObjectIdCollection idEnts) {
			ObjectIdCollection idsEntNew = new ObjectIdCollection();

			Polyline3d poly3d = null;

			short i = 0;

			List<Point3d> pnts3dBeg = new List<Point3d>();
			List<Point3d> pnts3dEnd = new List<Point3d>();


			for (i = 0; i <= idEnts.Count - 1; i++)
			{
				poly3d = (Polyline3d)idEnts[i].getEnt();

				pnts3dBeg.Add(poly3d.StartPoint);
				pnts3dEnd.Add(poly3d.EndPoint);
				poly3d.DowngradeOpen();

			}

			if (fWall1.PntsWallCutBeg.Count > 0)
			{

				for (i = 0; i <= fWall1.PntsWallCutBeg.Count - 1; i++)
				{
					pnts3dBeg.Add(fWall1.PntsWallCutBeg[i]);

				}

			}

			if (fWall1.PntsWallCutEnd.Count > 0)
			{

				for (i = 0; i <= fWall1.PntsWallCutEnd.Count - 1; i++)
				{
					pnts3dEnd.Add(fWall1.PntsWallCutEnd[i]);

				}

			}


			for (i = 3; i <= idEnts.Count - 1; i++)
			{
				poly3d = (Polyline3d)idEnts[i].getEnt();

				pnts3dBeg.Add(poly3d.StartPoint);
				pnts3dEnd.Add(poly3d.EndPoint);
				poly3d.DowngradeOpen();

			}

			Transaction TR = BaseObjs.startTransactionDb();
			using (TR)
			{
				poly3d = (Polyline3d)TR.GetObject(Base_Tools45.Draw.addPoly3d(pnts3dBeg), OpenMode.ForWrite);
				poly3d.Layer = strLayer;
				poly3d.Color = Color.FromColorIndex(ColorMethod.ByBlock, 140);
				poly3d.LineWeight = LineWeight.LineWeight050;
				idsEntNew.Add(poly3d.ObjectId);

				poly3d = (Polyline3d)TR.GetObject(Base_Tools45.Draw.addPoly3d(pnts3dEnd), OpenMode.ForWrite);
				poly3d.Layer = strLayer;
				poly3d.Color = Color.FromColorIndex(ColorMethod.ByBlock, 140);
				poly3d.LineWeight = LineWeight.LineWeight050;
				idsEntNew.Add(poly3d.ObjectId);

			}

			return idsEntNew;
		}

		public static ObjectIdCollection
		makeEndBrklinesWALL(string strLayer, ObjectIdCollection idEnts, bool boolClose) {

			ObjectIdCollection idsEntNew = new ObjectIdCollection();
			Polyline3d poly3d = null;

			short i = 0;

			List<Point3d> pnts3dBeg = new List<Point3d>();
			List<Point3d> pnts3dEnd = new List<Point3d>();
			using (BaseObjs._acadDoc.LockDocument())
			{

				Transaction TR = BaseObjs.startTransactionDb();
				using (TR)
				{


					for (i = 0; i <= idEnts.Count - 1; i++)
					{
						poly3d = (Polyline3d)TR.GetObject(idEnts[i], OpenMode.ForRead);

						pnts3dBeg.Add(poly3d.StartPoint);
						pnts3dEnd.Add(poly3d.EndPoint);

					}


					if (boolClose)
					{
						pnts3dBeg.Add(pnts3dBeg[0]);
						pnts3dEnd.Add(pnts3dEnd[0]);

					}

					poly3d = (Polyline3d)TR.GetObject(Base_Tools45.Draw.addPoly3d(pnts3dBeg), OpenMode.ForWrite);
					poly3d.Layer = strLayer;
					poly3d.Color = Color.FromColorIndex(ColorMethod.ByBlock, 120);
					poly3d.LineWeight = LineWeight.LineWeight050;
					idsEntNew.Add(poly3d.ObjectId);

					poly3d = (Polyline3d)TR.GetObject(Base_Tools45.Draw.addPoly3d(pnts3dEnd), OpenMode.ForWrite);
					poly3d.Layer = strLayer;
					poly3d.Color = Color.FromColorIndex(ColorMethod.ByBlock, 120);
					poly3d.LineWeight = LineWeight.LineWeight050;
					idsEntNew.Add(poly3d.ObjectId);

					TR.Commit();

				}
			}

			return idsEntNew;
		}

		public static ObjectIdCollection
		makeEndBrklinesORG(string strLayer, ObjectIdCollection idEnts, bool boolClose) {

			ObjectIdCollection idsEntNew = new ObjectIdCollection();
			Polyline3d poly3d = default(Polyline3d);

			List<Point3d> pnts3dBeg = new List<Point3d>();
			List<Point3d> pnts3dEnd = new List<Point3d>();

			using (BaseObjs._acadDoc.LockDocument())
			{
				Transaction TR = BaseObjs.startTransactionDb();
				using (TR)
				{


					for (int i = 0; i <= idEnts.Count - 1; i++)
					{
						poly3d = (Polyline3d)TR.GetObject(idEnts[i], OpenMode.ForRead);

						pnts3dBeg.Add(poly3d.StartPoint);
						pnts3dEnd.Add(poly3d.EndPoint);

					}


					if (boolClose)
					{
						pnts3dBeg.Add(pnts3dBeg[0]);
						pnts3dEnd.Add(pnts3dEnd[0]);

					}
					ObjectId idPoly3d = ObjectId.Null;
					idPoly3d = Draw.addPoly3d(pnts3dBeg, strLayer, 120);
					idsEntNew.Add(idPoly3d);

					idPoly3d = Draw.addPoly3d(pnts3dEnd, strLayer, 120);
					idsEntNew.Add(idPoly3d);

					TR.Commit();

				}
			}

			return idsEntNew;
		}

		public static ObjectId
		CreateProfileByLayout(string strName, Alignment objAlign, List<PNT_DATA> varPNT_DATA) {

			string strLayerName = string.Format("PROFILE-{0}", strName);
			ObjectId idLayer = Layer.manageLayers(strLayerName);

			Profile objProfile = null;
			ObjectId idProfileStyle = Prof_Style.getProfileStyle("WALL");
			ObjectId idStyleLabelSet = Prof_Style.getProfileLabelSetStyle("WALL");

			//ObjectId idProfileStyle = Prof_Style.getProfileStyle("Standard");
			//ObjectId idStyleLabelSet = Prof_Style.getProfileLabelSetStyle("Standard");

			using (BaseObjs._acadDoc.LockDocument())
			{
				using (Transaction TR = BaseObjs.startTransactionDb())
				{


					try
					{
						Prof.removeProfile(objAlign.ObjectId, strName);


					}
					catch (Autodesk.AutoCAD.Runtime.Exception )
					{
					}

					objProfile = Prof.addProfileByLayout(strName, objAlign.ObjectId, idLayer, idProfileStyle, idStyleLabelSet);

					double dblPntZ = 0;


					for (int i = 0; i < varPNT_DATA.Count; i++)
					{
						dblPntZ = varPNT_DATA[i].z;

						try
						{
							objProfile.PVIs.AddPVI(varPNT_DATA[i].STA, dblPntZ);

						}
						catch (Autodesk.AutoCAD.Runtime.Exception )
						{
						}

					}

					objProfile.Layer = strLayerName;

					TR.Commit();

				}
			}

			return objProfile.ObjectId;
		}

		public static ObjectId
		CreateProfileByLayout2(string strName, Alignment objAlign, List<PNT_DATA> varPNT_DATA) {
			ObjectId idAlign = objAlign.ObjectId;

			string strLayerName = string.Format("PROFILE-{0}", strName);
			ObjectId idLayer = Layer.manageLayers(strLayerName);
		   
			ObjectId idProfileStyle = default(ObjectId);
			try {
				using (BaseObjs._acadDoc.LockDocument()) {
					idProfileStyle = Prof_Style.getProfileStyle("WALL");
				}
				if (idProfileStyle.IsNull) {
					idProfileStyle = BaseObjs._civDoc.Styles.ProfileStyles[0];
				}
			}
			catch {
				Application.ShowAlertDialog("Profile Style not available.");
				return ObjectId.Null;
			}

			ObjectId idStyleLabelSet = default(ObjectId);
			try {
				using (BaseObjs._acadDoc.LockDocument()) {
					idStyleLabelSet = Prof_Style.getProfileLabelSetStyle("WALL");
				}
				if (idStyleLabelSet == ObjectId.Null) {
					idStyleLabelSet = BaseObjs._civDoc.Styles.LabelSetStyles.ProfileLabelSetStyles[0];
				}
			}
			catch {
				Application.ShowAlertDialog("Profile LabelSet Style not available.");
				return ObjectId.Null;
			}

			using (BaseObjs._acadDoc.LockDocument()) {
				using (Transaction TR = BaseObjs.startTransactionDb()) {
					try {
						Prof.removeProfile(objAlign.ObjectId, strName);
					}
					catch (Autodesk.AutoCAD.Runtime.Exception) {
					}

					Profile objProfile = Prof.addProfileByLayout(strName, objAlign.ObjectId, idLayer, idProfileStyle, idStyleLabelSet);

					double dblPntZ = 0;

					for (int i = 0; i < varPNT_DATA.Count; i++) {
						dblPntZ = varPNT_DATA[i].z;

						try {
							objProfile.PVIs.AddPVI(varPNT_DATA[i].STA, dblPntZ);
						}
						catch (Autodesk.Civil.CivilException) {
							try {
								objProfile.PVIs.AddPVI(varPNT_DATA[i].STA + 0.01, dblPntZ);
							}
							catch (Autodesk.Civil.CivilException) {
								try {
									objProfile.PVIs.AddPVI(varPNT_DATA[i].STA - 0.01, dblPntZ);
								}
								catch (Autodesk.Civil.CivilException) {
								}
							}
						}
					}

					objProfile.Layer = strLayerName;

					TR.Commit();
				}
			}
			return idAlign;
		}

		public static bool
		getDesignPoints(ObjectId idAlign, ref List<POI> varPOI, string strSide) {

			List<POI> varPOI_PNTs = new List<POI>();
			POI vPOI_PNT = default(POI);

			vPOI_PNT = new POI();
			vPOI_PNT.Desc0 = "NOTHING";
			varPOI_PNTs.Add(vPOI_PNT);

			double dblStation = 0;
			double dblOffset = 0;

			double dblToleranceLength = 0;

			short i = 0;
			short k = 0;

			bool boolAdd = false;

			CogoPointCollection objCogoPnts = BaseObjs._civDoc.CogoPoints;

			k = -1;

			foreach (ObjectId idCogoPnt in objCogoPnts) {
				CogoPoint objCogoPnt = (CogoPoint)idCogoPnt.getEnt();

				boolAdd = false;

				try {
					try {
						idAlign.getAlignStaOffset(objCogoPnt.Location, ref dblStation, ref dblOffset);
					}
					catch (Autodesk.Civil.PointNotOnEntityException ) {
						dblStation = 0.0;
					}

					if (strSide == "LEFT") {
						if (dblOffset > dblToleranceLength * -1 & dblOffset < 0) {
							boolAdd = true;
						}
					}else if (strSide == "RIGHT") {
						if (dblOffset > 0 & dblOffset < dblToleranceLength) {
							boolAdd = true;
						}
					}else if (strSide == "BOTH") {
						if (System.Math.Abs(dblOffset) < dblToleranceLength) {
							boolAdd = true;
						}
					}
				}
				catch (Autodesk.AutoCAD.Runtime.Exception ex) {
					boolAdd = false;
					Debug.Print(ex.ToString());
				}

				if (boolAdd == true) {
					vPOI_PNT = new POI();
					vPOI_PNT.Station = Base_Tools45.Math.roundDown3(dblStation);
					vPOI_PNT.PntNum = objCogoPnt.PointNumber.ToString();
					vPOI_PNT.Elevation = System.Math.Round(objCogoPnt.Elevation, 3);

					try {
						idAlign.getAlignStaOffset(objCogoPnt.Location, ref dblStation, ref dblOffset);
					}
					catch (Autodesk.Civil.PointNotOnEntityException ) {
						dblStation = 0.0;
					}

					vPOI_PNT.OFFSET = System.Math.Round(dblOffset, 2);
					vPOI_PNT.Desc0 = string.Format("{0} {1}", varPOI_PNTs[k].OFFSET, varPOI_PNTs[k].Elevation);
					vPOI_PNT.PntSource = "GCAL";

					varPOI_PNTs.Add(vPOI_PNT);
				}

				objCogoPnt.Dispose();
			}

			dynamic stations = from data in varPOI_PNTs
							   orderby data.Station
							   select data;
			List<POI> p = new List<POI>();
			foreach (var d in stations)
				p.Add(d);
			varPOI_PNTs = p;

			Misc.removeDuplicatePoints(ref varPOI_PNTs);

			if (varPOI_PNTs.Count > 0)
				for (i = 0; i < varPOI_PNTs.Count; i++) 
					varPOI.Add(varPOI_PNTs[i]);

			dynamic staPOI = from data in varPOI
							 orderby data.Station
							 select data;
			p = new List<POI>();
			foreach (var d in staPOI)
				p.Add(d);

			if (varPOI.Count > 0)
				return true;
			else 
				return false;		
		}
	}
}