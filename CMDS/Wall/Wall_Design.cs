using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Linq;
using wne = Wall.Wall_NestedEnts;

namespace Wall {
	public static class Wall_Design {
		static double PI = System.Math.PI;

		static Wall_Form.frmWall1 fWall1 = Wall_Forms.wForms.fWall1;

		public static List<PNT_DATA>
		sOffsToPNT_DATA(List<staOffElev> sOffs){
			List<PNT_DATA> pData = new List<PNT_DATA>();
			foreach(staOffElev s in sOffs){
				PNT_DATA p = new PNT_DATA() { STA = s.staAlign, z = s.elev };
				pData.Add(p);
			}
			return pData;
		}

		public static List<staOffElev>
		convert3dPolyToPnt_Data(Alignment objAlign, List<AlgnEntData> algnEntDataList, ObjectId idPoly3d, string strName) {
			Vector2d v2d0;
			Vector2d v2dX;
			AlgnEntData algnEntData;

			//-----------------------------------------------Check orientation of Breakline-----------------------------------------------

			List<Point3d> pnts3d = idPoly3d.getCoordinates3dList();

			Point2d pnt2dBrkBeg = new Point2d(pnts3d[0].X, pnts3d[0].Y);
			Point2d pnt2dBrkEnd = new Point2d(pnts3d[1].X, pnts3d[1].Y);
			v2d0 = pnt2dBrkEnd - pnt2dBrkBeg;
			
			Point2d pnt2dSegBeg = algnEntDataList[0].pnt2dBeg;
			Point2d pnt2dSegEnd = algnEntDataList[0].pnt2dEnd;
			v2dX = pnt2dSegEnd - pnt2dSegBeg;

			double angFirstAlignSegAndFirstTwoPoints = Geom.getAngle2Vectors(v2d0, v2dX);
			if(angFirstAlignSegAndFirstTwoPoints > PI/30){
				pnts3d.Reverse();
				pnt2dBrkBeg = new Point2d(pnts3d[0].X, pnts3d[0].Y);
				pnt2dBrkEnd = new Point2d(pnts3d[1].X, pnts3d[1].Y);
				v2d0 = pnt2dBrkEnd - pnt2dBrkBeg;
				angFirstAlignSegAndFirstTwoPoints = Geom.getAngle2Vectors(v2d0, v2dX);
				if (angFirstAlignSegAndFirstTwoPoints > PI / 30)
				{
					Application.ShowAlertDialog("The angle between the first alignment segment and the first two points of the breakline is exceeds 6 degrees.  Revise and retry...");
				}
			}else if(angFirstAlignSegAndFirstTwoPoints == 0){
                double dirAlignSeg = pnt2dSegBeg.getDirection(pnt2dSegEnd);
                double dirBrkLine = pnt2dBrkBeg.getDirection(pnt2dBrkEnd);
                if (dirAlignSeg != dirBrkLine)
                    pnts3d.Reverse();
			}

			//-----------------------------------------------Check orientation of Breakline-----------------------------------------------

			//-------------------------------------Analyze point locations relative to Alignment Segments---------------------------------

			staOffElev so = default(staOffElev);

			List<staOffElev> sOffs = new List<staOffElev>();

			int m = 0;
			double angLim1 = 0, angLim2 = 0, slope = 0, distX = 0, elev = 0;
			for (int i = 0; i < algnEntDataList.Count; i++) {
				algnEntData = algnEntDataList[i];
				double staAlignSegBeg = algnEntData.StaBeg;

				for (int j = m; j < pnts3d.Count; j++) { 
					Point3d pnt3dX = pnts3d[j];
					//defer finding begin point - point before first positive sta point if any
					//this exercise is for determining angle points

					so = Geom.getStaOff(algnEntData.pnt2dBeg, algnEntData.pnt2dEnd, pnt3dX);
					so.segIndex = i;

					//if (so.staSeg <= algnEntData.Length) {
					//    so.staAlign = staAlignSegBeg + so.staSeg;
					//    sOffs.Add(so);                        
					//}else if (so.staSeg > algnEntData.Length) {
						if (i < algnEntDataList.Count - 1) {
							Point3d pnt3dEnd = new Point3d(algnEntData.pnt2dEnd.X, algnEntData.pnt2dEnd.Y, 0.0);
							double angPntToEndOfSeg = pnt3dEnd.getDirection(pnt3dX);

							if (so.off > 0) { //right side
								if (algnEntData.AngDeflection > 0) { //LH Outside angle point Case 1
									//angLim1 = algnEntData.Direction - PI / 2;
									//angLim2 = angLim1 + algnEntData.AngDeflection;
									if (so.staSeg <= algnEntData.Length)
									{
										so.staAlign = staAlignSegBeg + so.staSeg;
										sOffs.Add(so);
									}
									else if (so.staSeg > algnEntData.Length)
									{
										slope = pnt3dX.getSlope(pnts3d[j - 1]);
										distX = so.staSeg - algnEntData.Length;
										elev = pnt3dX.Z + slope * distX;
										so = new staOffElev() { staAlign = algnEntData.StaEnd - 0.01, elev = elev, angPt = true, segIndex = i };
										sOffs.Add(so);

										//compute and add staOffElev to next segment data
										algnEntData = algnEntDataList[i + 1];
										so = Geom.getStaOff(algnEntData.pnt2dBeg, algnEntData.pnt2dEnd, pnt3dX);
										slope = pnt3dX.getSlope(pnts3d[j + 1]);
										elev = pnt3dX.Z - so.staSeg * slope;
										so = new staOffElev() { staAlign = algnEntData.StaBeg + 0.01, elev = elev, angPt = true, segIndex = i + 1 };
										sOffs.Add(so);
										m = j + 1;
										break;
									}
								}else { //RH Inside angle point Case 2
									angLim1 = algnEntData.Direction + algnEntData.AngDeflection - PI / 2;
									angLim2 = angLim1 + algnEntData.AngDeflection;
									if(so.staSeg < algnEntData.Length){
										
										if (angPntToEndOfSeg > angLim1 && angPntToEndOfSeg < angLim2) {
											so.staAlign = algnEntData.StaEnd;
											sOffs.Add(so);
											m = j + 1;
											break;
										}else{
											so.staAlign = staAlignSegBeg + so.staSeg;
											sOffs.Add(so);
											m = j + 1;
											break;
										}                                        
									}

								}
							}else if (so.off < 0) { //left side  
				
								if (algnEntData.AngDeflection > 0) { //LH Inside angle point Case 3

									angLim1 = algnEntData.Direction + PI / 2;
									angLim2 = angLim1 + algnEntData.AngDeflection;
									if(so.staSeg < algnEntData.Length){
										if (angPntToEndOfSeg > angLim1 && angPntToEndOfSeg < angLim2) {
											so.staAlign = algnEntData.StaEnd;
											sOffs.Add(so);
											m = j + 1;
											break;
										}else{
											so.staAlign = staAlignSegBeg + so.staSeg;
											sOffs.Add(so);
											m = j + 1;
											break;
										}                                        
									}


								}else { //RH Outside angle point Case 4
									//angLim1 = algnEntData.Direction + PI / 2 + algnEntData.AngDeflection;
									//angLim2 = angLim1 - algnEntData.AngDeflection;

									if (so.staSeg <= algnEntData.Length) {
										so.staAlign = staAlignSegBeg + so.staSeg;
										sOffs.Add(so);
									}
									else if (so.staSeg > algnEntData.Length)
									{
										slope = pnt3dX.getSlope(pnts3d[j - 1]);
										distX = so.staSeg - algnEntData.Length;
										elev = pnt3dX.Z + slope * distX;
										so = new staOffElev() { staAlign = algnEntData.StaEnd - 0.01, elev = elev, angPt = true, segIndex = i };
										sOffs.Add(so);

										//compute and add staOffElev to next segment data
										algnEntData = algnEntDataList[i + 1];
										so = Geom.getStaOff(algnEntData.pnt2dBeg, algnEntData.pnt2dEnd, pnt3dX);
										slope = pnt3dX.getSlope(pnts3d[j + 1]);
										elev = pnt3dX.Z - so.staSeg * slope;
										so = new staOffElev() { staAlign = algnEntData.StaBeg + 0.01, elev = elev, angPt = true, segIndex = i + 1 };
										sOffs.Add(so);
										m = j + 1;
										break;
									}
								}
							}else {
								so.staAlign = staAlignSegBeg + so.staSeg;
								sOffs.Add(so);
							}							
						}else {
							so.staAlign = staAlignSegBeg + so.staSeg;
							sOffs.Add(so);
						}
					//}
				}
			}		
			return sOffs;
		}
 
		public static List<AlgnEntData>
		getAlgnEntData(Alignment objAlign) {
			//-----------------------------------------------Collect alignment segment data-----------------------------------------------
			AlgnEntData algnEntData = default(AlgnEntData);
			List<AlgnEntData> algnEntDataList = new List<AlgnEntData>(); //data for alignment entities in order occurance in alignment not by ID

				AlignmentEntityCollection alignEnts = objAlign.Entities;
				int k = alignEnts.Count;

				foreach (AlignmentEntity alignEnt in alignEnts) {
					algnEntData = new AlgnEntData();
					if (alignEnt.EntityType == AlignmentEntityType.Arc) {
						AlignmentArc alignEntArc = alignEnt as AlignmentArc;
						try {
							algnEntData.EntBefore = alignEntArc.EntityBefore;
						}
						catch (Autodesk.Civil.EntityNotFoundException) {
							algnEntData.EntBefore = 0;
						}
						algnEntData.ID = alignEntArc.EntityId;
						algnEntData.Length = alignEntArc.Length;
						algnEntData.Radius = alignEntArc.Radius;
						algnEntData.Direction = 0;
						algnEntData.ClockWise = alignEntArc.Clockwise;
						algnEntData.StaBeg = alignEntArc.StartStation;
						algnEntData.StaEnd = alignEntArc.EndStation;
						algnEntData.Type = "Arc";
						algnEntData.pnt2dBeg = alignEntArc.StartPoint;
						algnEntData.pnt2dEnd = alignEntArc.EndPoint;
					}else if (alignEnt.EntityType == AlignmentEntityType.Line) {
						AlignmentLine alignEntLine = alignEnt as AlignmentLine;
						try {
							algnEntData.EntBefore = alignEntLine.EntityBefore;
						}
						catch (Autodesk.Civil.EntityNotFoundException) {
							algnEntData.EntBefore = 0;
						}
						algnEntData.ID = alignEntLine.EntityId;
						algnEntData.Length = alignEntLine.Length;
						algnEntData.Radius = 0;
						algnEntData.Direction = alignEntLine.Direction;
						algnEntData.StaBeg = alignEntLine.StartStation;
						algnEntData.StaEnd = alignEntLine.EndStation;
						algnEntData.Type = "Line";
						algnEntData.pnt2dBeg = alignEntLine.StartPoint;
						algnEntData.pnt2dEnd = alignEntLine.EndPoint;
					}
					algnEntDataList.Add(algnEntData);
				}

				for (int i = 0; i < algnEntDataList.Count - 1; i++) {
					AlgnEntData algnEnt0 = algnEntDataList[i];
					AlgnEntData algnEntX = algnEntDataList[i + 1];

					Vector2d v2d0 = algnEnt0.pnt2dEnd - algnEnt0.pnt2dBeg;
					Vector2d v2dX = algnEntX.pnt2dEnd - algnEntX.pnt2dBeg;

					double ang3pnts = Geom.getAngle3Points2d(algnEnt0.pnt2dBeg, algnEnt0.pnt2dEnd, algnEntX.pnt2dEnd);
					algnEnt0.AngDeflection = v2d0.getDeflectionAngle2Vectors(v2dX);
					if (Geom.testRight(algnEnt0.pnt2dBeg, algnEnt0.pnt2dEnd, algnEntX.pnt2dEnd) < 0) {
						algnEnt0.AngDeflection *= -1;
					}

					algnEntDataList[i] = algnEnt0;
				}
				return algnEntDataList;
		}

		public static void
		getPoints(string strFunction) {

			ObjectId idPnt = default(ObjectId);
			CogoPoint objPnt = default(CogoPoint);

			double dblStation = 0;
			double dblOffset = 0;

			List<PNT_DATA> varPNT_DATA = new List<PNT_DATA>();
			PNT_DATA vPNT_DATA = new PNT_DATA();

			Alignment objAlign = fWall1.ACTIVEALIGN;

			BaseObjs.write(string.Format("\nSelect {0} Points adjacent to Alignment: ", strFunction));

			do {
				try {
					idPnt = wne.getNestedPoint("Select Point <ESC to exit>: \n");
				} catch (Autodesk.AutoCAD.Runtime.Exception ) {
					break;
				}

				if ((idPnt == ObjectId.Null)) {
					return;
				}

				objPnt = (CogoPoint)idPnt.getEnt();
				try {
					objAlign.StationOffset(objPnt.Easting, objPnt.Northing, ref dblStation, ref dblOffset);
				} catch (Autodesk.Civil.PointNotOnEntityException ) {
					dblStation = 0.0;
				}

				vPNT_DATA.DESC = objPnt.RawDescription;
				vPNT_DATA.NUM = objPnt.PointNumber;
				vPNT_DATA.x = objPnt.Easting;
				vPNT_DATA.y = objPnt.Northing;
				vPNT_DATA.z = System.Math.Round(objPnt.Elevation, 3);
				vPNT_DATA.ALIGN = objAlign.Name;
				vPNT_DATA.STA = System.Math.Round(dblStation, 2);
				vPNT_DATA.OFFSET = System.Math.Round(dblOffset, 2);

				varPNT_DATA.Add(vPNT_DATA);

			} while (true);

			dynamic staPNT_DATA = from data in varPNT_DATA
								  orderby data.STA
								  select data;

			List<PNT_DATA> p = new List<PNT_DATA>();
			foreach (var d in staPNT_DATA)
				p.Add(d);

			if (strFunction == "EXIST") {
				fWall1.PNTSEXIST = varPNT_DATA;
			} else if (strFunction == "DESIGN") {
				fWall1.PNTSDESIGN = varPNT_DATA;
			}
		}

		public static List<double>
		getPNT_DATA_FINAL(Alignment objAlignPL, Alignment objAlignRF) {
			List<double> dblStationsFinal = new List<double>();
			List<PNT_DATA> Pnt_Data_PL = new List<PNT_DATA>();
			PNT_DATA Pnt_Data = new PNT_DATA();

			double dblStationRF = 0;

			int i = 0;
			int j = 0;
			int k = 0;
			int n = 0;
			int x = 0;

			//GET PL AND RF PROFILES

			Profile objProfilePL = null;
			try {
				objProfilePL = Prof.getProfile(objAlignPL.Name, "EXIST");
			}
			catch (Autodesk.AutoCAD.Runtime.Exception) {
			}

			if ((objProfilePL == null)) {
				Application.ShowAlertDialog("EXIST profile for PL missing.  Exiting .....");
				return null;
			}

			Profile objProfileRF = null;

			try {
				objProfileRF = Prof.getProfile(objAlignRF.Name, "CPNT");
			}
			catch (Autodesk.AutoCAD.Runtime.Exception) {
			}

			if ((objProfileRF == null)) {
				Application.ShowAlertDialog("CPNT profile for REF Alignment missing.  Exiting .....");
				return null;
			}

			foreach (ProfilePVI objPVI_PL in objProfilePL.PVIs) {
				if (objPVI_PL.Station >= objAlignRF.StartingStation & objPVI_PL.Station <= objAlignRF.EndingStation) {
					Pnt_Data = new PNT_DATA();
					Pnt_Data.STA = System.Math.Round(objPVI_PL.Station, 2);
					Pnt_Data_PL.Add(Pnt_Data);
				}
			}

			// Existing profile PVIs within objAlignRF limits
			double dblLenBack = 0;
			double dblLenAhead = 0;

			for (i = 1; i < Pnt_Data_PL.Count; i++) {
				dblLenBack = (Pnt_Data_PL[i - 0].STA - Pnt_Data_PL[i - 1].STA);
				dblLenAhead = (Pnt_Data_PL[i + 1].STA - Pnt_Data_PL[i + 0].STA);

				if (dblLenBack != 0) {
					Pnt_Data_PL[i].SLOPEBACK = System.Math.Round((objProfilePL.ElevationAt(Pnt_Data_PL[i - 1].STA) - objProfilePL.ElevationAt(Pnt_Data_PL[i - 0].STA)) / dblLenBack, 5);
				}

				if (dblLenAhead != 0) {
					Pnt_Data_PL[i].SLOPEAHEAD = System.Math.Round((objProfilePL.ElevationAt(Pnt_Data_PL[i + 1].STA) - objProfilePL.ElevationAt(Pnt_Data_PL[i + 0].STA)) / dblLenAhead, 5);
				}
			}

			// CPNT profile pvis
			List<PNT_DATA> Pnt_Data_DE = new List<PNT_DATA>();
			Wall_Form.frmWall2 fWall2 = Wall_Forms.wForms.fWall2; {
				Pnt_Data_DE = fWall2.PNTSDESIGN;

				if ((Pnt_Data_DE == null)) {
					Pnt_Data_DE = new List<PNT_DATA>();

					int z = objProfileRF.PVIs.Count;

					foreach (ProfilePVI objPVI_RF in objProfileRF.PVIs) {
						dblStationRF = objPVI_RF.Station;

						Pnt_Data = new PNT_DATA();
						if (i == 0) {
							Pnt_Data.STA = System.Math.Round(dblStationRF + 0.01, 2);
						}else if (i == z - 1) {
							Pnt_Data.STA = System.Math.Round(dblStationRF - 0.01, 2);
						}else {
							Pnt_Data.STA = System.Math.Round(dblStationRF, 2);
						}

						Pnt_Data.z = objPVI_RF.Elevation;

						Pnt_Data_DE.Add(Pnt_Data);
					}
				}else {
					k = Pnt_Data_DE.Count - 1;

					for (i = 1; i <= k - 1; i++) {
						dblLenBack = (Pnt_Data_DE[i - 0].STA - Pnt_Data_DE[i - 1].STA);
						dblLenAhead = (Pnt_Data_DE[i + 1].STA - Pnt_Data_DE[i + 0].STA);

						if (dblLenBack != 0) {
							Pnt_Data_DE[i].SLOPEBACK = System.Math.Round((objProfileRF.ElevationAt(Pnt_Data_DE[i - 1].STA) - objProfileRF.ElevationAt(Pnt_Data_DE[i - 0].STA)) / dblLenBack, 5);
						}

						if (dblLenAhead != 0) {
							Pnt_Data_DE[i].SLOPEAHEAD = System.Math.Round((objProfileRF.ElevationAt(Pnt_Data_DE[i + 1].STA) - objProfileRF.ElevationAt(Pnt_Data_DE[i + 0].STA)) / dblLenAhead, 5);
						}
					}
				}
			}

			double dblSlopDiff = 0;
			List<PNT_DATA> Pnt_Data_Tmp = new List<PNT_DATA>();
			if (Pnt_Data_DE.Count > 0) {
				Pnt_Data_Tmp.Add(Pnt_Data_DE[0]);
			}

			j = 0;

			for (i = 0; i <= Pnt_Data_PL.Count - 1; i++) {
				dblSlopDiff = Pnt_Data_PL[i].SLOPEAHEAD - Pnt_Data_PL[i].SLOPEBACK;

				if (System.Math.Abs(System.Math.Round(dblSlopDiff, 2)) > 0.005) {
					Pnt_Data_Tmp.Add(Pnt_Data_PL[i]);
				}
			}

			for (i = 1; i <= Pnt_Data_DE.Count - 1; i++) {
				dblSlopDiff = Pnt_Data_DE[i].SLOPEAHEAD + Pnt_Data_DE[i].SLOPEBACK;

				if (System.Math.Round(dblSlopDiff, 2) > 0.001) {
					Pnt_Data_Tmp.Add(Pnt_Data_DE[i]);
				}
			}

			if (Pnt_Data_DE.Count > 1) {
				Pnt_Data_Tmp.Add(Pnt_Data_DE[Pnt_Data_DE.Count - 1]);
			}

			dynamic staPNT_DATA = from data in Pnt_Data_Tmp
								  orderby data.STA
								  select data;

			i = -1;
			foreach (PNT_DATA vPNT_DATA in staPNT_DATA) {
				i = i + 1;
				Pnt_Data_Tmp[i] = vPNT_DATA;
			}

			List<PNT_DATA> Pnt_Data_Final = new List<PNT_DATA>();
			Pnt_Data_Final.Add(Pnt_Data_Tmp[0]);

			k = Pnt_Data_Tmp.Count - 1;
			j = 0;
			//GET GRADEBREAKS
			for (i = 1; i <= k; i++) {
				dblLenBack = System.Math.Round(Pnt_Data_Tmp[i - 0].STA - Pnt_Data_Tmp[i - 1].STA, 1);

				if (Pnt_Data_Tmp[i].SLOPEBACK < 0) {
					if (Pnt_Data_Tmp[i].SLOPEAHEAD < 0) {
						Pnt_Data_Final.Add(Pnt_Data_Tmp[i]);
					}
				}else if (Pnt_Data_Tmp[i].SLOPEBACK > 0) {
					if (Pnt_Data_Tmp[i].SLOPEAHEAD > 0) {
						Pnt_Data_Final.Add(Pnt_Data_Tmp[i]);
					}
				}
			}

			int intInterval = 10;

			//ADD DATAPOINTS EVERY intInterval  FEET
			for (i = 1; i < Pnt_Data_Tmp.Count; i++) {
				dblLenBack = System.Math.Round(Pnt_Data_Tmp[i - 0].STA - Pnt_Data_Tmp[i - 1].STA, 1);

				if (dblLenBack > intInterval) {
					n = (int)System.Math.Truncate(dblLenBack / intInterval);

					if (i == 1) {
						for (x = 1; x <= n; x++) {
							Pnt_Data = new PNT_DATA();
							Pnt_Data.STA = Pnt_Data_Tmp[i - 1].STA + x * intInterval;
							Pnt_Data_Final.Add(Pnt_Data);
						}
					}else {
						if (dblLenBack / intInterval - n < 0.2) {
							Pnt_Data = new PNT_DATA();
							Pnt_Data_Final.Add(Pnt_Data_Tmp[i - 1]);

							Pnt_Data = new PNT_DATA();
							Pnt_Data.STA = Pnt_Data_Tmp[i - 1].STA + dblLenBack / (n + 1);
							Pnt_Data_Final.Add(Pnt_Data);
						}else {
							for (x = 0; x <= n; x++) {
								Pnt_Data = new PNT_DATA();
								Pnt_Data.STA = Pnt_Data_Tmp[i - 1].STA + x * intInterval;
								Pnt_Data_Final.Add(Pnt_Data);
							}
						}
					}
				}else if (dblLenBack > 5) {
					Pnt_Data_Final.Add(Pnt_Data_Tmp[i]);
				}
			}

			j = Pnt_Data_Final.Count - 1;

			if (objAlignRF.EndingStation - Pnt_Data_Final[j].STA > 5) {
				Pnt_Data_Final.Add(Pnt_Data_Tmp[k]);
			}else {
				Pnt_Data_Final[j] = Pnt_Data_Tmp[k];
			}

			for (i = 0; i <= Pnt_Data_Final.Count - 1; i++) {
				if (Pnt_Data_Final[i].STA < Pnt_Data_PL[0].STA) {
					Pnt_Data_Final[i] = Pnt_Data_PL[0];
				}else {
					break; // TODO: might not be correct. Was : Exit For
				}
			}

			if (Pnt_Data_DE.Count > 1) {
				Pnt_Data_Final.Add(Pnt_Data_DE[Pnt_Data_DE.Count - 1]);
			}

			staPNT_DATA = from data in Pnt_Data_Final
						  orderby data.STA
						  select data;

			Pnt_Data_Tmp = new List<PNT_DATA>();

			foreach (PNT_DATA vPNT_DATA in staPNT_DATA) {
				vPNT_DATA.STA = System.Math.Round(vPNT_DATA.STA, 1);
				Pnt_Data_Tmp.Add(vPNT_DATA);
			}

			Pnt_Data_Final = new List<PNT_DATA>();

			Pnt_Data_Final = Misc.removeDuplicateStations(Pnt_Data_Tmp);

			dblStationsFinal = new List<double>();

			for (i = 0; i < Pnt_Data_Final.Count; i++) {
				dblStationsFinal.Add(Pnt_Data_Final[i].STA);
			}

			return dblStationsFinal;
		}

		public static EXGROUND
		getExGround(TinSurface objSurfaceEXIST, Alignment objAlignRF, double dblStationRF, double[] dblOffsets) {
			EXGROUND vExGround = null;

			double dblEasting = 0;
			double dblNorthing = 0;
			List<double> dblElev = null;

			for (int i = 0; i < dblOffsets.Length; i++) {
				objAlignRF.PointLocation(dblStationRF, dblOffsets[i], ref dblEasting, ref dblNorthing);
				dblElev[i] = objSurfaceEXIST.FindElevationAtXY(dblEasting, dblNorthing);
			}

			vExGround.RF.Offset = (float)dblOffsets[0];
			vExGround.RF.Elev = (float)dblElev[0];

			vExGround.TC.Offset = (float)dblOffsets[1];
			vExGround.TC.Elev = (float)dblElev[1];

			vExGround.TOE.Offset = (float)dblOffsets[2];
			vExGround.TOE.Elev = (float)dblElev[2];

			vExGround.TOP.Offset = (float)dblOffsets[3];
			vExGround.TOP.Elev = (float)dblElev[3];

			vExGround.PL.Offset = (float)dblOffsets[4];
			vExGround.PL.Elev = (float)dblElev[4];

			vExGround.Off5.Offset = (float)dblOffsets[5];
			vExGround.Off5.Elev = (float)dblElev[5];

			return vExGround;
		}
	}
}