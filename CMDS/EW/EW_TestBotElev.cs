using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Windows.Forms;
using p = EW.EW_Pub;

namespace EW {
	public static class EW_TestBotElev {
		static bool exists = false;

		public static void reTest() {
			try {
				double dblDepth = EW_Utility1.getRemoveAndReplace();

				object[] dblGridData = new object[3];

				ObjectId idDictBOT = Dict.getNamedDictionary("BOT", out exists);
				ResultBuffer rb = idDictBOT.getXData("BOT");
				if (rb == null)
					return;
				TypedValue[] tvs = rb.AsArray();

				dblGridData[0] = tvs[1].Value.ToString();
				dblGridData[1] = tvs[2].Value.ToString();
				dblGridData[2] = tvs[3].Value.ToString();

				SelectionSet objSSet = EW_Utility1.buildSSet13();
				objSSet.eraseSelectedItems();

				testBotElev(dblGridData, true);
			}
			catch {
				MessageBox.Show("  - reTest");
			}
		}

		public static void testBotElev(object[] varGridData, bool boolReTest) { 
			bool boolProceed = false;
			bool isOX = false;
			bool isOXg = false;
	
			double dblPntXmin = (double)(varGridData[0]);	
			double dblPntYmin = (double)(varGridData[1]);
			int intInterval = (int)varGridData[2];

			Layer.manageLayers("BOT-POINTS");
		
			TinSurface objSurfaceSG = Surf.getTinSurface("SG");
			if (objSurfaceSG == null) {
				MessageBox.Show("Surface SG is missing.  Exiting ......");
				return;
			}
	
			TinSurface objSurfaceOX = Surf.getTinSurface("OX");
			if (objSurfaceOX == null) {
				MessageBox.Show("Surface OX is missing.  Exiting ......");
			}
	
			TinSurface objSurfaceOXg = Surf.getTinSurface("OXg");
			if (objSurfaceOXg != null) {
				isOXg = true;
			}
	
			EW_Utility1.getTableData();
			TinSurface objSurfaceEXIST = null;
			if (p.REMOVE_REPLACE_BLDG_V == -1) {
				objSurfaceEXIST = Surf.getTinSurface("NATIVE");
				if (objSurfaceEXIST == null) {
					MessageBox.Show("NATIVE surface missing while Remove and Replace values are 0.  Exiting routine..........");
					return;
				}
			}else {
				objSurfaceEXIST = Surf.getTinSurface("EXIST");
				if (objSurfaceEXIST == null) {
					objSurfaceEXIST = Surf.getTinSurface("exist");
					if (objSurfaceEXIST == null) {
						MessageBox.Show("Surface EXIST missing.  Exiting routine......");
						return;
					}
				}
			}
		
			SurfaceDefinitionBoundaries bndrys = objSurfaceEXIST.BoundariesDefinition;

			for (int i = 0; i < bndrys.Count; i++) {
				if (bndrys[i].BoundaryType == Autodesk.Civil.SurfaceBoundaryType.Hide) { 
					MessageBox.Show("Surface EXIST is using HIDE Boundaries - exiting program");
					return;			
				}
			}
	
			try {
				SelectionSet objSSet = null;
				if (p.boolDebug) {
					objSSet = EW_Utility1.buildSSet2b("OX"); //GET "OX-BRKLINE* by select
				}else {
					if (isOX)
						objSSet = EW_Utility1.buildSSet2a("OX"); //GET "OX-BRKLINE*
					else
						objSSet = EW_Utility1.buildSSet2a("SG"); //GET "SG-BRKLINE*
				}
			
				//BEGIN BOUNDARY EDIT - ADD X GRID POINTS
				
				ObjectId[] ids = objSSet.GetObjectIds();
				for (int i = 0; i < ids.Length; i++) {
					ObjectId id3dBndry_Org = ids[i];
				
					if (p.boolDebug) {
						id3dBndry_Org.changeProp(LineWeight.LineWeight020, clr.c70);
					}
								
					ResultBuffer rb = id3dBndry_Org.getXData("makeBOT");
					if (rb == null)
						continue;
					TypedValue[] tvs = rb.AsArray();
					string strType = "";
					try {
						strType = tvs[2].Value.ToString();
					}
					catch (System.Exception ) {
						strType = "K-BRACE";
					}
				
					rb = id3dBndry_Org.getXData("OX");
					if (rb == null)
						continue;
					tvs = rb.AsArray();
								
					try {
						string strSource = tvs[1].Value.ToString();
						switch(strSource) {
							case "OX1":
								strType = "OX1";
								break;
							case "OX2":
								strType = "OX2";
								break;
							case "OX3":
								strType = "OX3";
								break;
							case "OX4":
								strType = "OX4";
								break;
						}
					}
					catch (System.Exception) {
					}
				
					List<Point3d> varPnts3dBndry = id3dBndry_Org.getCoordinates3dList();
				
					int k = varPnts3dBndry.Count;
				
					List<Point3d> dblPnts3dBndry = new List<Point3d>();
					Point3d pnt3d = new Point3d(varPnts3dBndry[0].X, varPnts3dBndry[0].Y, 0.0);
					dblPnts3dBndry.Add(pnt3d);

					int intXCounter = 0, intYCounter = 0;

					for (int j = 1; j < k; j++) { 
						Point3d pnt3dBeg = varPnts3dBndry[j - 1];
						Point3d pnt3dEnd = varPnts3dBndry[j - 0];

						double dblLEN = pnt3dBeg.getDistance(pnt3dEnd);
						if (dblLEN > 5) {
							double dblAngle = pnt3dBeg.getDirection(pnt3dEnd);
						
							int intXbeg = (int)((pnt3dBeg.X - dblPntXmin) / intInterval);						
							int intXend = (int)((pnt3dEnd.X - dblPntXmin) / intInterval);
							if (intXend - intXbeg > 0) {
								intXCounter = 1;
								intXbeg++;
								boolProceed = true;
							}else if (intXend - intXbeg < 0) {
								intXCounter = -1;
								intXend++;
								boolProceed = true;
							}else if (intXend - intXbeg == 0) {
								intXCounter = -1;
								boolProceed = false;
							}
						
							if (boolProceed == true) {
								for (int n = intXbeg; n <= intXend; n += intXCounter) {
									double dblX = dblPntXmin + n * intInterval;
									double dblY = pnt3dBeg.Y + (dblX - pnt3dBeg.X) * System.Math.Tan(dblAngle);
									pnt3d = new Point3d(dblX, dblY, 0);
								
									if (p.boolDebug) { 
										Draw.addCircle(pnt3d, 1, "DEBUG-0", (short)3);									
									}
								
									dblPnts3dBndry.Add(pnt3d);								
								}
							}

							pnt3d = new Point3d(pnt3dEnd.X, pnt3dEnd.Y, 0.0);
							dblPnts3dBndry.Add(pnt3d);

							if (p.boolDebug) {
								Draw.addCircle(pnt3d, 1, "DEBUG-0", (short)3);																
							}
						}else {
							pnt3d = new Point3d(pnt3dEnd.X, pnt3dEnd.Y, 0.0);
							dblPnts3dBndry.Add(pnt3d);
						
							if (p.boolDebug) {
								Draw.addCircle(pnt3d, 1, "DEBUG-0", (short)2);																							
							}
						}
					}
				
					//END BOUNDARY EDIT - ADD X GRID POINTS
				
					varPnts3dBndry = dblPnts3dBndry;
					k = varPnts3dBndry.Count;
				
					//BEGIN BOUNDARY EDIT - ADD Y GRID POINTS
					dblPnts3dBndry = new List<Point3d>();
					dblPnts3dBndry.Add(varPnts3dBndry[0]); //keep first point
				
					if (p.boolDebug) {
						Draw.addCircle(dblPnts3dBndry[0], 1, "DEBUG-0", (short)1);						
					}
				
					for (int x = 0; x < k; x++) { 
						Point3d pnt3dBeg = varPnts3dBndry[x - 1];
						Point3d pnt3dEnd = varPnts3dBndry[x - 0];

						double dblLEN = pnt3dBeg.getDistance(pnt3dEnd);

						if (dblLEN > 5) {
							double dblAngle = pnt3dBeg.getDirection(pnt3dEnd);
						
							int intYbeg = (int)((pnt3dBeg.Y - dblPntYmin) / intInterval);						
							int intYend = (int)((pnt3dEnd.Y - dblPntYmin) / intInterval);
						
							if (intYend - intYbeg > 0) {
								intYCounter = (short)1;
								intYbeg++;
								boolProceed = true;
							}else if (intYend - intYbeg < 0) {
								intYCounter = (short)(-1);
								intYend++;
								boolProceed = true;
							}else if (intYend - intYbeg == ((short)0)) {
								intYCounter = (short)(-1);
								boolProceed = false;
							}
						
							if (boolProceed == true) {
								for (k = intYbeg; k <= intYend; k += intYCounter) {
									double dblY = dblPntYmin + k * intInterval;
									double dblX = pnt3dBeg.X + (dblY - pnt3dBeg.Y) / System.Math.Tan(dblAngle);
									pnt3d = new Point3d(dblX, dblY, 0.0);
								
									if (p.boolDebug) {
										Draw.addCircle(pnt3d, 1, "DEBUG-0", (short)2);																																
									}
									dblPnts3dBndry.Add(pnt3d);
								}
							}

							dblPnts3dBndry.Add(pnt3dEnd);
						
							if (p.boolDebug) {
								Draw.addCircle(pnt3dEnd, 1, "DEBUG-0", (short)3);							
							}
						}else {
							dblPnts3dBndry.Add(pnt3dEnd);
						
							if (p.boolDebug) {
								Draw.addCircle(pnt3dEnd, 1, "DEBUG-0", (short)2);							
							}
						}
					}
				
					//END BOUNDARY EDIT - ADD Y GRID POINTS
				
					//BEGIN REMOVE DUPLICATE POINTS

					List<Point3d> dblPnts3dBndryRev = new List<Point3d>();
					dblPnts3dBndryRev.Add(dblPnts3dBndry[0]);
				
					for (int j = 1; j < dblPnts3dBndry.Count; j++) {
						if (System.Math.Round(dblPnts3dBndry[j - 1].X, 3) == System.Math.Round(dblPnts3dBndry[j - 0].X, 3) &&
							System.Math.Round(dblPnts3dBndry[j - 1].Y, 3) == System.Math.Round(dblPnts3dBndry[j - 0].Y, 3)) {
							//do nothing
						}else {
							dblPnts3dBndryRev.Add(dblPnts3dBndry[j]);
						}
					}
				
					//END REMOVE DUPLICATE POINTS
				
					//BEGIN TEST ELEVATION ON BOUNDARY
				
					k = dblPnts3dBndryRev.Count;
				
					if (k > 3) {
						for (int j = 0; j < k; j++) {
							pnt3d = dblPnts3dBndryRev[j];
						
							double dblZ_EXIST = EW_Utility2.getSurfaceElev(pnt3d.X, pnt3d.Y, objSurfaceEXIST);
							double dblZ_R_R = 0;
							if (isOX) {
								dblZ_R_R = dblZ_EXIST - p.REMOVE_REPLACE_BLDG_V;
							}else {
								dblZ_R_R = dblZ_EXIST - p.REMOVE_REPLACE_V;
							}
						
							double dblZ_SG = EW_Utility2.getSurfaceElev(pnt3d.X, pnt3d.Y, objSurfaceSG);
							double dblZ_OX = EW_Utility2.getSurfaceElev(pnt3d.X, pnt3d.Y, objSurfaceOX);
							double dblZ_OXg = 0;
							if (isOXg) {
								dblZ_OXg = EW_Utility2.getSurfaceElev(pnt3d.X, pnt3d.Y, objSurfaceOXg);
							}
						
							double dblZ_MIN = dblZ_R_R;
						
							if (dblZ_SG < dblZ_MIN) {
								dblZ_MIN = dblZ_SG;
							}
						
							if (dblZ_OX < dblZ_MIN) {
								dblZ_MIN = dblZ_OX;
							}
						
							if (isOXg) {
								if (dblZ_OXg < dblZ_MIN) {
									dblZ_MIN = dblZ_OXg;
								}
							}
						
							if (dblZ_MIN <= 0) {
								Draw.addCircle(pnt3d, 1, "DEBUG-0", (short)1);	
								pnt3d = pnt3d.addElevation(dblZ_R_R);						
								dblPnts3dBndryRev.Add(pnt3d);
							}else {
								pnt3d = pnt3d.addElevation(dblZ_MIN);						
								dblPnts3dBndryRev.Add(pnt3d);
							}
						}
					
						ObjectId id3dBndry = Draw.addPoly3d(dblPnts3dBndryRev);
					
						if (strType == "LIM" || strType == "K-BRACE") {
							id3dBndry.changeProp(clr.byl, "BOT-BRKLINE-AREA");
						}else if (strType == "BRKLINES") {
							id3dBndry.changeProp(clr.byl, "BOT-BRKLINE");
						}
					}
				
					//END TEST ELEVATION ON BOUNDARY
				
					//BEGIN TEST ELEVATION ON GRID POINTS
					List<string> excludes = new List<string>() { "BRKLINES", "OX2", "OX3", "OX4" };
					if (excludes.Contains(strType))
						continue;
					else {
						k = dblPnts3dBndryRev.Count;
						dblPnts3dBndryRev.RemoveAt(k - 1);
						
						Point3dCollection varPnts3dBndryCol = new Point3dCollection();
						foreach (Point3d pnt in dblPnts3dBndryRev) {
							pnt.addElevation(0.0);
							varPnts3dBndryCol.Add(pnt);
						}
					
						varPnts3dBndry = dblPnts3dBndryRev;
						
						SelectionSet objSSetX = EW_Utility1.buildSSet11(varPnts3dBndryCol); //select points by polygon crossing
						ids = objSSetX.GetObjectIds();

						if (strType == "_XX-BUILDING ADJACENT LANDSCAPING") {
							//do nothing i.e. do not assign point description "BOT"
						}else {
							for (int j = 0; j < ids.Length; j++) {
								pnt3d = ids[j].getCogoPntCoordinates();

								if (pnt3d.isInside(dblPnts3dBndryRev)) {
									double dblZ_EXIST = EW_Utility2.getSurfaceElev(pnt3d.X, pnt3d.Y, objSurfaceEXIST);
									double dblZ_R_R = 0;

									if (isOX) {
										dblZ_R_R = dblZ_EXIST - p.REMOVE_REPLACE_BLDG_V;
									}else {
										dblZ_R_R = dblZ_EXIST - p.REMOVE_REPLACE_V;
									}

									double dblZ_SG = EW_Utility2.getSurfaceElev(pnt3d.X, pnt3d.Y, objSurfaceSG);
									double dblZ_OX = EW_Utility2.getSurfaceElev(pnt3d.X, pnt3d.Y, objSurfaceOX);
									double dblZ_OXg = 0;
									if (isOXg) {
										dblZ_OXg = EW_Utility2.getSurfaceElev(pnt3d.X, pnt3d.Y, objSurfaceOXg);
									}
								
									if (dblZ_OX < 0) {
										dblZ_OX = dblZ_SG;
									}
								
									double dblZ_MIN = dblZ_R_R;
								
									if (dblZ_SG < dblZ_MIN) {
										dblZ_MIN = dblZ_SG;
									}
								
									if (dblZ_OX < dblZ_MIN) {
										dblZ_MIN = dblZ_OX;
									}
								
									if (isOXg) {
										if (dblZ_OXg < dblZ_MIN) {
											dblZ_MIN = dblZ_OXg;
										}
									}
								
									using (var tr = BaseObjs.startTransactionDb()) {
										CogoPoint cgPnt = (CogoPoint)tr.GetObject(ids[j], OpenMode.ForRead);
										cgPnt.Elevation = dblZ_MIN;
										cgPnt.RawDescription = "BOT";
										cgPnt.Layer = "BOT-POINTS";
										cgPnt.StyleId = CgPnts.getPntStyle("BOT").ObjectId;
										cgPnt.LabelStyleId = CgPnts.getPntLabelStyle("BOT");
									}
								}
							}
						}
					}
					//END TEST ELEVATION ON GRID POINTS+``````````
				}
			
				if (boolReTest == false)				
					EW_Main.viewResults("BOT", false);
			}
			catch {
			}
			return;
		}
	}
}