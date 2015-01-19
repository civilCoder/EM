using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

using Autodesk.Civil.DatabaseServices;

using Base_Tools45;
using Base_Tools45.C3D;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace Wall {
	public static class Wall_DesignLimits {
		static double PI = System.Math.PI;

		static Wall_Form.frmWall2 fWall2 = Wall_Forms.wForms.fWall2;

		public static bool
		wallDesignLimits(ProfileView objProfileViewWall, string strAPP) {
			bool success = false;

			try {
				double dblUnitWidth = double.Parse(fWall2.tbx_SpaceH.Text);
				double dblStepMinV = double.Parse(fWall2.tbx_StepV.Text);

				double dblStepMinH = 0, dblUnitHeight = 0;
				if (fWall2.opt_BLOCK.Checked) {
					dblStepMinH = double.Parse(fWall2.tbx_StepH.Text);
					dblUnitHeight = double.Parse(fWall2.tbx_SpaceV.Text);
				}else {
					dblStepMinH = dblUnitWidth;
					dblUnitHeight = -1;
				}

				double dblCover = double.Parse(fWall2.tbx_Cover.Text);
				double dblFreeboard = double.Parse(fWall2.tbx_Freeboard.Text);
				double dblFreeboardTolerance = double.Parse(fWall2.tbx_FreeboardTolerance.Text);
				Point3d pnt3d = Point3d.Origin;

				if ((objProfileViewWall == null)) {
					try {
						objProfileViewWall = (ProfileView)Base_Tools45.Select.selectEntity(typeof(ProfileView), "\nSelect ProfileView: ", "\nProfile View Selection Failed.", out pnt3d);
					}
					catch (Autodesk.AutoCAD.Runtime.Exception ) {
						Application.ShowAlertDialog("ProfileView selection failed...try again");
						return false;
					}
				}

				Alignment objAlignWall = Align.getAlignment(objProfileViewWall.AlignmentId);
				
				string strLayer = string.Format("PROFILE-{0}-TOW", objAlignWall.Name.ToString());


				Layer.manageLayers(strLayer);

				strLayer = string.Format("PROFILE-{0}-TOF", objAlignWall.Name.ToString());
				Layer.manageLayers(strLayer);

				strLayer = string.Format("PROFILE-{0}-TO*", objAlignWall.Name.ToString());
				double dblScale = 0;
				TypedValue[] tvs = null;
				using (BaseObjs._acadDoc.LockDocument()) {
					using (Transaction TR = BaseObjs.startTransactionDb()) {
						List<Point3d> pnts3d = Prof.getProfileViewOriginAndScale(objProfileViewWall.ObjectId, out dblScale);

						tvs = new TypedValue[9] {
							new TypedValue((int)DxfCode.Operator, "<OR"),
							new TypedValue((int)DxfCode.Start, "LWPOLYLINE"),
							new TypedValue((int)DxfCode.Start, "LEADER"),
							new TypedValue((int)DxfCode.Start, "TEXT"),
							new TypedValue((int)DxfCode.Start, "MTEXT"),
							new TypedValue((int)DxfCode.Start, "CIRCLE"),
							new TypedValue((int)DxfCode.Start, "DIMENSION"),
							new TypedValue((int)DxfCode.Operator, "OR>"),
							new TypedValue((int)DxfCode.LayerName, strLayer)
						};

						Misc.deleteObjs(tvs, pnts3d[0], pnts3d[1]);
						TR.Commit();
					}
				}

				Profile objProfileEx = null;
				try {
					objProfileEx = Prof.getProfile(objAlignWall.Name, "EXIST");
					if ((objProfileEx == null)) {
						objProfileEx = Prof.getProfile(objAlignWall.Name, "BRKLEFT");
					}
				}
				catch (Autodesk.AutoCAD.Runtime.Exception ) {
				}

				Profile objProfileDe = null;
				try {
					objProfileDe = Prof.getProfile(objAlignWall.Name, "CPNT");
					if ((objProfileDe == null)) {
						objProfileDe = Prof.getProfile(objAlignWall.Name, "BRKRIGHT");
					}
				}
				catch (Autodesk.AutoCAD.Runtime.Exception ) {
				}

				double dblStaStart = System.Math.Truncate(objAlignWall.StartingStation);
				double dblStaEnd = System.Math.Truncate(objAlignWall.EndingStation);

				if (fWall2.opt_BLOCK.Checked) {
					dblUnitWidth = System.Math.Round(dblUnitWidth / 12, 5);
				}

				List<object> varXDataType = new List<object>();
				List<object> varXDataVal = new List<object>();

				double dblStaStartLeft = 0;
				double dblStaEndLeft = 0;
				double dblStaStartRight = 0;
				double dblStaEndRight = 0;

				ResultBuffer rb = objProfileViewWall.GetXDataForApplication("BRKLEFT");
				if (rb == null)
					return false;
				tvs = rb.AsArray();

				dblStaStartLeft = double.Parse(tvs[2].Value.ToString());
				dblStaEndLeft = double.Parse(tvs[3].Value.ToString());

				rb = objProfileViewWall.GetXDataForApplication("BRKRIGHT");
				if (rb == null)
					return false;
				tvs = rb.AsArray();                
				dblStaStartRight = double.Parse(tvs[2].Value.ToString());
				dblStaEndRight = double.Parse(tvs[3].Value.ToString());                

				if (dblStaStartRight > dblStaStart) {
					dblStaStart = dblStaStartRight;
				}

				if (dblStaStartLeft > dblStaStartRight) {
					dblStaStart = dblStaStartLeft;
				}

				dblStaStart = Base_Tools45.Math.roundUP0(dblStaStart);

				if (dblStaEndRight < dblStaEnd) {
					dblStaEnd = dblStaEndRight;
				}

				if (dblStaEndLeft < dblStaEndRight) {
					dblStaEnd = dblStaEndLeft;
				}

				dblStaEnd = Base_Tools45.Math.roundDown2(dblStaEnd);

				double dblEleEX = System.Math.Round(objProfileEx.ElevationAt(dblStaStart), 2);
				double dblEleDE = System.Math.Round(objProfileDe.ElevationAt(dblStaStart), 2);

				WALL_STEP vTOW_STEP = new WALL_STEP();
				vTOW_STEP.BEGSTA = dblStaStart;
				vTOW_STEP.ENDSTA = dblStaStart + dblStepMinH;
				if (vTOW_STEP.ENDSTA > dblStaEnd) {
					vTOW_STEP.ENDSTA = dblStaEnd;
				}

				WALL_STEP vTOF_STEP = new WALL_STEP();
				vTOF_STEP.BEGSTA = dblStaStart;
				vTOF_STEP.ENDSTA = dblStaStart + dblStepMinH;
				if (vTOF_STEP.ENDSTA > dblStaEnd) {
					vTOF_STEP.ENDSTA = dblStaEnd;
				}

				List<WALL_STEP> varTOW_STEP = new List<WALL_STEP>();
				List<WALL_STEP> varTOF_STEP = new List<WALL_STEP>();

				varTOW_STEP.Add(vTOW_STEP);
				varTOF_STEP.Add(vTOF_STEP);

				int p = 0;

				if (dblEleEX > dblEleDE) {
					varTOF_STEP[0].ELEV = dblEleDE - dblCover;

					if (fWall2.opt_PANEL.Checked) {
						if (fWall2.cbx_HeightAboveFooting.Checked == true) {
							varTOW_STEP[0].ELEV = dblEleDE + dblFreeboard - dblCover;
						}else {
							varTOW_STEP[0].ELEV = dblEleEX + dblFreeboard;
						}
					}

					if (fWall2.opt_BLOCK.Checked) {
						if (fWall2.cbx_HeightAboveFooting.Checked == true) {
							if (dblFreeboard * 12 % System.Math.Round(dblStepMinV * 12, 2) != 0) {
								p = (int)System.Math.Truncate(dblFreeboard / dblStepMinV);
								dblFreeboard = (p + 1) * dblStepMinV;
							}

							varTOW_STEP[0].ELEV = varTOF_STEP[0].ELEV + dblFreeboard;
						}else {
							p = (int)System.Math.Truncate((dblEleEX + dblFreeboard - varTOF_STEP[0].ELEV) / dblStepMinV);
							varTOW_STEP[0].ELEV = varTOF_STEP[0].ELEV + (p + 1) * dblStepMinV;
						}
					}
				}else if (dblEleDE >= dblEleEX) {
					varTOF_STEP[0].ELEV = dblEleEX - dblCover;

					if (fWall2.opt_PANEL.Checked) {
						if (fWall2.cbx_HeightAboveFooting.Checked == true) {
							varTOW_STEP[0].ELEV = dblEleEX + dblFreeboard - dblCover;
						}else {
							varTOW_STEP[0].ELEV = dblEleDE + dblFreeboard;
						}
					}

					if (fWall2.opt_BLOCK.Checked) {
						if (fWall2.cbx_HeightAboveFooting.Checked == true) {
							if (dblFreeboard * 12 % System.Math.Round(dblStepMinV * 12, 2) != 0) {
								p = (int)System.Math.Truncate(dblFreeboard / dblStepMinV);
								dblFreeboard = (p + 1) * dblStepMinV;
							}

							varTOW_STEP[0].ELEV = varTOF_STEP[0].ELEV + dblFreeboard;
						}else {
							p = (int)System.Math.Truncate((dblEleDE + dblFreeboard - varTOF_STEP[0].ELEV) / dblStepMinV);
							varTOW_STEP[0].ELEV = varTOF_STEP[0].ELEV + (p + 1) * dblStepMinV;
						}
					}
				}

				//Call drawWallStep(objProfileView, varTOW_STEP[0])
				//Call drawWallStep(objProfileView, varTOF_STEP[0])

				p = 0;
				int i = 0, j = 0, k = 0, n = 0, z = 0;
				//i = station for top of wall
				//j = counter for top of wall steps
				//k = counter for top of footing steps
				//n = station for top of footing
				//p = counter for vertical step increments
				//x = counter for horizontal  increments per step - TOP OF WALL
				//z = counter for horizontal increments per step - TOP OF FOOTING
				double lenAlign = objAlignWall.Length;
				lenAlign = dblStaEnd - dblStaStart;

				Debug.Print("wallDesignLimits - Line 246");
				if ((lenAlign < dblStepMinH)) {
					i = (int)(dblStaEnd);
					n = (int)(dblStaEnd);
				}else {
					i = (int)(dblStaStart + dblStepMinH);
					n = (int)(dblStaStart + dblStepMinH);
				}

				//Do While i < objAlignWall.EndingStation - dblStepMinH
				//Do While i < objProfileDE.EndingStation - dblStepMinH

				double dblEleTOP = 0, dblEleBOT = 0;
				double dblEleTOW_REV = 0, dblEleTOF_REV;

				while (i < dblStaEnd - dblStepMinH) {
					j = j + 1;
					vTOW_STEP = new WALL_STEP();
					vTOW_STEP.BEGSTA = varTOW_STEP[j - 1].ENDSTA;
					vTOW_STEP.ELEV = varTOW_STEP[j - 1].ELEV;
					//elevation is same as previous step to start with
					vTOW_STEP.ENDSTA = i + dblStepMinH;
					varTOW_STEP.Add(vTOW_STEP);

					k = k + 1;
					vTOF_STEP = new WALL_STEP();
					vTOF_STEP.BEGSTA = varTOF_STEP[k - 1].ENDSTA;
					vTOF_STEP.ELEV = varTOF_STEP[k - 1].ELEV;
					//elevation is same as previous step to start with
					vTOF_STEP.ENDSTA = n + dblStepMinH;
					varTOF_STEP.Add(vTOF_STEP);

					dblEleEX = objProfileEx.ElevationAt(i);
					dblEleDE = objProfileDe.ElevationAt(i);

					if (dblEleEX > dblEleDE) {
						dblEleTOP = dblEleEX;
						dblEleBOT = dblEleDE;
					}else {
						dblEleTOP = dblEleDE;
						dblEleBOT = dblEleEX;
					}

					if (fWall2.cbx_HeightAboveFooting.Checked == false) {
						double dblClearTOP = Base_Tools45.Math.roundDown2(varTOW_STEP[j].ELEV - dblEleTOP); //TOP OF WALL                      
						double dblDiffTOP = Base_Tools45.Math.roundDown2(dblClearTOP - dblFreeboard);       //target for DiffTop is 0 to MinStepV

						if (dblClearTOP >= 0) {
							if (dblDiffTOP > 0 & dblDiffTOP < dblStepMinV) {
								//do nothing
								//Call drawWallStep(objProfileView, varTOW_STEP[j])
								//excess clear at end of current step  -->> lower current step
							}else if (dblClearTOP > dblFreeboard + dblStepMinV) {
								//no change to previous step
								varTOW_STEP[j].ELEV = varTOW_STEP[j - 1].ELEV - dblStepMinV * (int)((dblClearTOP - dblFreeboard) / dblStepMinV);
								//adjust down to reduce dblClear

								p = 0;
								do {
									p = p + 1;
									dblEleTOW_REV = varTOW_STEP[j].ELEV - dblStepMinV * p;
									//test if step can be lowered more;  if clear would be less than required freeboard then quit
									if (dblEleTOW_REV - dblEleTOP < dblFreeboard) {
										break; // TODO: might not be correct. Was : Exit Do
									}else {
										varTOW_STEP[j].ELEV = varTOW_STEP[j].ELEV - dblStepMinV * p;
										//else lower step and keep track of adjustments with p
									}
								}
								while (true);
								//Call drawWallStep(objProfileView, varTOW_STEP[j])
								//insufficient clear so raise top of previous wall step and set current step to same elevation
							}else if (dblClearTOP < dblFreeboard) {
								p = 0;
								do {
									p = p + 1;
									dblEleTOW_REV = varTOW_STEP[j - 1].ELEV + dblStepMinV * p;
									//check if additional adjustment is needed
									//if adjustment is needed
									if (dblEleTOW_REV - dblEleTOP > dblFreeboard) {
										varTOW_STEP[j - 1].ELEV = dblEleTOW_REV;
										break; // TODO: might not be correct. Was : Exit Do
									}
								}
								while (true);
								varTOW_STEP[j].ELEV = varTOW_STEP[j - 1].ELEV;
								//Call drawWallStep(objProfileView, varTOW_STEP[j])
							}
						}else if (dblClearTOP < 0) {
							p = 0;
							do {
								p = p + 1;
								dblEleTOW_REV = varTOW_STEP[j - 1].ELEV + dblStepMinV * p;
								//check if additional adjustment is needed
								if (dblEleTOW_REV - dblEleTOP > dblFreeboard) {
									varTOW_STEP[j - 1].ELEV = dblEleTOW_REV;
									break; // TODO: might not be correct. Was : Exit Do
								}
							}
							while (true);

							varTOW_STEP[j].ELEV = varTOW_STEP[j - 1].ELEV;
						}

						double dblClearBOT = Base_Tools45.Math.roundDown2(dblEleBOT - varTOF_STEP[k].ELEV);//TOP OF FOOTING                       
						double dblDiffBOT = dblClearBOT - dblCover;

						if (dblClearBOT >= 0) {
							if (dblDiffBOT > 0 & dblDiffBOT < dblStepMinV) {
								//do nothing
								//Call drawWallStep(objProfileView, varTOF_STEP[j])
								//excess cover raise current step. leave prior step alone
							}else if (dblClearBOT > dblCover + dblStepMinV) {
								p = 0;
								do {
									p = p + 1;
									dblEleTOF_REV = varTOF_STEP[k].ELEV + dblStepMinV * p;
									if (dblEleBOT - dblEleTOF_REV < (dblCover + dblStepMinV)) {
										varTOF_STEP[k].ELEV = dblEleTOF_REV;
										break; // TODO: might not be correct. Was : Exit Do
									}
								}
								while (true);
								//Call drawWallStep(objProfileView, varTOF_STEP[k])
							}else if (dblClearBOT < dblCover) {
								p = 0;
								do {
									p = p + 1;
									dblEleTOF_REV = varTOF_STEP[k - 1].ELEV - dblStepMinV * p;

									if (dblEleBOT - dblEleTOF_REV > dblCover) {
										varTOF_STEP[k - 1].ELEV = dblEleTOF_REV;
										break; // TODO: might not be correct. Was : Exit Do
									}
								}
								while (true);
								varTOF_STEP[k].ELEV = dblEleTOF_REV;
								//Call drawWallStep(objProfileView, varTOF_STEP[k])
							}
						}else if (dblClearBOT < 0) {
							p = 0;
							do {
								p = p + 1;
								dblEleTOF_REV = varTOF_STEP[k - 1].ELEV - dblStepMinV * p;

								if (dblEleBOT - dblEleTOF_REV > dblCover) {
									varTOF_STEP[k - 1].ELEV = dblEleTOF_REV;
									break; // TODO: might not be correct. Was : Exit Do
								}
							}
							while (true);

							varTOF_STEP[k].ELEV = dblEleTOF_REV;
						}
						//Call drawWallStep(objProfileView, varTOF_STEP[k])
					}

					i = i + (int)dblStepMinH;
					n = n + (int)dblStepMinH;

					Debug.Print(i.ToString());
				}

				double dblEleDE_BEG = 0;
				double dblEleEX_BEG = 0;
				double dblElevTOPBeg = 0, dblElevTOPEnd = 0, dblElevBOTBeg, dblElevBOTEnd = 0;

				double dblEleDE_END = objProfileDe.ElevationAt(dblStaEnd);
				double dblEleEX_END = objProfileEx.ElevationAt(dblStaEnd);

				if (dblEleEX_END > dblEleDE_END) {
					dblElevTOPEnd = dblEleEX_END;
					dblElevBOTEnd = dblEleDE_END;
				}else {
					dblElevTOPEnd = dblEleDE_END;
					dblElevBOTEnd = dblEleEX_END;
				}

				dblEleDE_BEG = objProfileDe.ElevationAt(varTOW_STEP[j].ENDSTA);
				dblEleEX_BEG = objProfileEx.ElevationAt(varTOW_STEP[j].ENDSTA);

				if (dblEleEX_BEG <= dblEleDE_BEG) {
					dblElevTOPBeg = dblEleDE_BEG;
					dblElevBOTBeg = dblEleEX_BEG;
				}else {
					dblElevTOPBeg = dblEleEX_BEG;
					dblElevBOTBeg = dblEleDE_BEG;
				}

				//grade is sloping up
				if (dblElevTOPEnd > dblElevTOPBeg) {
					if (varTOW_STEP[j].ELEV < dblElevTOPEnd + dblFreeboard) {
						vTOW_STEP = new WALL_STEP();
						p = 0;
						do {
							p = p + 1;
							dblEleTOW_REV = varTOW_STEP[j].ELEV + dblStepMinV * p;
							//check if additional adjustment is needed
							//if adjustment is needed
							if (dblEleTOW_REV - dblElevTOPEnd > dblFreeboard) {
								vTOW_STEP.ELEV = dblEleTOW_REV;
								break; // TODO: might not be correct. Was : Exit Do
							}
						}
						while (true);
						vTOW_STEP.BEGSTA = varTOW_STEP[j].ENDSTA;
						vTOW_STEP.ENDSTA = dblStaEnd;
						varTOW_STEP.Add(vTOW_STEP);
					}else {
						varTOW_STEP[j].ENDSTA = dblStaEnd;
					}
					
				}else {
					if (varTOW_STEP[j].ELEV > dblElevTOPEnd + dblFreeboard + dblStepMinV) { // grade is sloping down
						vTOW_STEP = new WALL_STEP();
						p = 0;
						do {
							p = p + 1;
							dblEleTOW_REV = varTOW_STEP[j].ELEV - dblStepMinV * p;
							//test if step can be lowered more;  if clear would be less than required freeboard then quit
							if (dblEleTOW_REV - dblElevTOPEnd < dblFreeboard) {
								break; // TODO: might not be correct. Was : Exit Do
							}else {
								vTOW_STEP.ELEV = varTOW_STEP[j].ELEV - dblStepMinV * (p - 1);
								//else lower step and keep track of adjustments with p
							}
						}
						while (true);
						vTOW_STEP.BEGSTA = varTOW_STEP[j].ENDSTA;
						vTOW_STEP.ENDSTA = dblStaEnd;
						varTOW_STEP.Add(vTOW_STEP);
					}else {
						varTOW_STEP[j].ENDSTA = dblStaEnd;
					}
				}
				//dblEleDE_BEG = objProfileDE.ElevationAt(varTOF_STEP[k].ENDSTA)
				//dblEleEX_BEG = objProfileEX.ElevationAt(varTOF_STEP[k].ENDSTA)

				//If dblEleEX_BEG <= dblEleDE_BEG Then
				//    dblElevTOPBeg = dblEleDE_BEG
				//    dblElevBOTBeg = dblEleEX_BEG
				//Else
				//    dblElevTOPBeg = dblEleEX_BEG
				//    dblElevBOTBeg = dblEleDE_BEG
				//End If

				//grade is sloping up
				if (dblElevBOTEnd > dblElevBOTBeg) {
					if (dblElevBOTEnd - varTOF_STEP[k].ELEV > dblCover + dblStepMinV) {
						vTOF_STEP = new WALL_STEP();
						p = 0;
						do {
							p = p + 1;
							dblEleTOF_REV = varTOF_STEP[k].ELEV + dblStepMinV * p;
							//check if additional adjustment is needed
							//if adjustment is needed
							if (dblElevBOTEnd - dblEleTOF_REV > dblCover) {
								vTOF_STEP.ELEV = dblEleTOF_REV;
								break; // TODO: might not be correct. Was : Exit Do
							}
						}
						while (true);
						vTOF_STEP.BEGSTA = varTOF_STEP[k].ENDSTA;
						vTOF_STEP.ENDSTA = dblStaEnd;
						varTOF_STEP.Add(vTOF_STEP);
					}else {
						varTOF_STEP[j].ENDSTA = dblStaEnd;
					}
					// grade is sloping down
				}else {
					if (dblElevBOTEnd - varTOF_STEP[k].ELEV < dblCover) {
						vTOF_STEP = new WALL_STEP();
						p = 0;
						do {
							p = p + 1;
							dblEleTOF_REV = varTOF_STEP[k].ELEV - dblStepMinV * p;
							//test if step needs to be lowered more;
							if (dblElevBOTEnd - dblEleTOF_REV > dblCover) {
								vTOF_STEP.ELEV = dblEleTOF_REV;
								break; // TODO: might not be correct. Was : Exit Do
							}
						}
						while (true);
						vTOF_STEP.BEGSTA = varTOF_STEP[k].ENDSTA;
						vTOF_STEP.ENDSTA = dblStaEnd;
						varTOF_STEP.Add(vTOF_STEP);
					}else {
						varTOF_STEP[j].ENDSTA = dblStaEnd;
					}
				}

				if (fWall2.cbx_HeightAboveFooting.Checked == true) {
					for (i = 0; i <= k; i++) {
						varTOW_STEP[i].ELEV = varTOF_STEP[i].ELEV + dblFreeboard;
					}
				}

				Debug.Print("wallDesignLimits - Line 545");

				List<WALL_STEP> rawTOW_STEP = varTOW_STEP;
				List<WALL_STEP> rawTOF_STEP = varTOF_STEP;

				if (varTOW_STEP.Count > 1) {
					cleanWallStep(ref varTOW_STEP);
				}
				if (varTOF_STEP.Count > 1) {
					cleanWallStep(ref varTOF_STEP);
				}

				strLayer = "PROFILE-" + objAlignWall.Name.ToString() + "-TOW";

				ObjectId idLayer = Layer.manageLayers(strLayer);
				Layer.modifyLayer(idLayer, 2, LineWeight.LineWeight000);

				ObjectId idLWPline = default(ObjectId);
				List<double> dblElevs = new List<double>();
				double  dblProfileViewScale = 0;
				List<Point3d> pnt3dOrigin = new List<Point3d>();

				using (BaseObjs._acadDoc.LockDocument()) {
					using (Transaction TR = BaseObjs.startTransactionDb()) {
						//Set cWallLim = New clsWallLim
						idLWPline = drawWallLimits(objProfileViewWall, varTOW_STEP, strLayer, "TOW", dblElevs);
						//Set cWallLim.LIM = objLWPline
						//colWallLim.Add cWallLim

						TR.Commit();
					}
				}

				using (BaseObjs._acadDoc.LockDocument()) {
					using (Transaction TR = BaseObjs.startTransactionDb()) {
						pnt3dOrigin = Prof.getProfileViewOriginAndScale(objProfileViewWall.ObjectId, out dblProfileViewScale);
						TR.Commit();
					}
				}

				using (BaseObjs._acadDoc.LockDocument()) {
					using (Transaction TR = BaseObjs.startTransactionDb()) {
						labelWallStepsTE(objProfileViewWall, "TOW", strLayer, dblProfileViewScale, dblElevs, varTOW_STEP);

						TR.Commit();
					}
				}

				using (BaseObjs._acadDoc.LockDocument()) {
					using (Transaction TR = BaseObjs.startTransactionDb()) {
						strLayer = "PROFILE-" + objAlignWall.Name.ToString() + "-TOF";
						idLayer = Layer.manageLayers(strLayer);
						Layer.modifyLayer(idLayer, 2, LineWeight.LineWeight000);

						//Set cWallLim = New clsWallLim
						idLWPline = drawWallLimits(objProfileViewWall, varTOF_STEP, strLayer, "TOF", dblElevs);
						//Set cWallLim.LIM = objLWPline
						//colWallLim.Add cWallLim

						TR.Commit();
					}
				}

				using (BaseObjs._acadDoc.LockDocument()) {
					using (Transaction TR = BaseObjs.startTransactionDb()) {
						labelWallStepsTE(objProfileViewWall, "TOF", strLayer, dblProfileViewScale, dblElevs, varTOF_STEP);
						//not translated from VB
						TR.Commit();
					}
				}

				using (BaseObjs._acadDoc.LockDocument()) {
					using (Transaction TR = BaseObjs.startTransactionDb()) {
						double dblElevStart = 0;
						List<POI> varPOI = new List<POI>();
						POI vPOI = default(POI);

						dblElevStart = objProfileViewWall.ElevationMin;
						dblStaStart = objProfileViewWall.StationStart;

						z = varTOW_STEP.Count - 1;
						vPOI = new POI();
						vPOI.Station = varTOW_STEP[0].BEGSTA;
						vPOI.Elevation = varTOW_STEP[0].ELEV;
						varPOI.Add(vPOI);

						vPOI = new POI();
						vPOI.Station = varTOW_STEP[z].ENDSTA;
						vPOI.Elevation = varTOW_STEP[z].ELEV;
						varPOI.Add(vPOI);

						double min = 0, max = 0, area = 0;
						getWallMinMaXHeightAndArea(rawTOF_STEP, rawTOW_STEP, dblStepMinH, out min, out max, out area);

						string strLine2 = string.Format("\\PAREA: {0:###,#00} SF -  HEIGHT VARIES: {1:F2} TO {2:F}", area, min, max);
						string strLine1 = string.Format("CONST, {0} L.F. RET. WALL", lenAlign);
						Dim.addDims(pnt3dOrigin[0], dblStaStart, dblElevStart, varPOI, strLayer, dblProfileViewScale, strLine1 + strLine2);
						//OVERALL LEADER LINE

						TR.Commit();
					}
				}

				success = true;
			}
			catch {
				return success;
			}

			return success;
		}

		public static void
		getWallMinMaXHeightAndArea(List<WALL_STEP> varWallStep_TOF, List<WALL_STEP> varWallStep_TOW, double stepH, out double min, out double max, out double area) {

			List<double> wallSegElevs = new List<double>();
			area = 0;

			int uF = varWallStep_TOF.Count - 1;
			int uW = varWallStep_TOW.Count - 1;

			int u = (uF < uW) ? uF : uW;


			double elevF = 0, elevW = 0, h = 0;

			try
			{
				for (int i = 0; i < u; i++ ){
					elevF = varWallStep_TOF[i].ELEV;
					elevW = varWallStep_TOW[i].ELEV;
					h = elevW - elevF;
					area += h * stepH;
					wallSegElevs.Add(h);
				}
			}
			catch (System.Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message);
			}

			List<double> y = new List<double>();
			foreach(double p in wallSegElevs.OrderBy(p => p)){
				y.Add(p);
			}
						

			min = y[0];
			max = y[y.Count - 1];
		}
	  

		public static void
		cleanWallStep(ref List<WALL_STEP> varWallStep) {
			int u = varWallStep.Count - 1;
			List<WALL_STEP> vWallStep = new List<WALL_STEP>();

			vWallStep.Add(varWallStep[0]);

			int k = 0;

			for (int i = 1; i < varWallStep.Count; i++) {
				if (varWallStep[i - 1].ELEV != varWallStep[i - 0].ELEV) {
					vWallStep[k].ENDSTA = varWallStep[i - 0].BEGSTA;

					k++;
					vWallStep.Add(varWallStep[i - 0]);
				}else {
					vWallStep[k].ENDSTA = varWallStep[i - 0].ENDSTA; //if first two are same elevation hold first elevation and second end station                    
				}
			}

			if (varWallStep[u - 1].ELEV != varWallStep[u - 0].ELEV) {
				k = k + 1;
				vWallStep.Add(varWallStep[u - 0]);
			}else {
				vWallStep[k].ENDSTA = varWallStep[u - 0].ENDSTA;
			}

			varWallStep = vWallStep;
		}

		public static ObjectId
		drawWallLimits(ProfileView objProfileView, List<WALL_STEP> wallStep, string strLayer, string strName, List<double> dblElevs) {
			ObjectId idPoly = ObjectId.Null;

			dblElevs = new List<double>();

			double dblProfileViewStartSta = objProfileView.StationStart;
			double dblElevStartProfileView = objProfileView.ElevationMin;
			double dblProfileViewScale = 0;

			List<Point3d> pnt3dOrigin = Prof.getProfileViewOriginAndScale(objProfileView.ObjectId, out dblProfileViewScale);
			List<Point3d> pnts3d = new List<Point3d>();
			for (int i = 0; i < wallStep.Count; i++) {
				Point3d pnt3d = new Point3d(pnt3dOrigin[0].X + (wallStep[i].BEGSTA - dblProfileViewStartSta), pnt3dOrigin[0].Y + (wallStep[i].ELEV - dblElevStartProfileView) * dblProfileViewScale, 0.0);
				pnts3d.Add(pnt3d);

				dblElevs.Add(wallStep[i].ELEV);

				pnt3d = new Point3d(pnt3dOrigin[0].X + (wallStep[i].ENDSTA - dblProfileViewStartSta), pnt3dOrigin[0].Y + (wallStep[i].ELEV - dblElevStartProfileView) * dblProfileViewScale, 0.0);
				pnts3d.Add(pnt3d);

				dblElevs.Add(wallStep[i].ELEV);
			}

			idPoly = Base_Tools45.Draw.addPoly(pnts3d, strLayer);

			return idPoly;
		}

		public static void
		labelWallSteps(ProfileView objProfileViewWall, string strName, string strLayer, double dblScaleProfileView, List<double>dblElevs, List<WALL_STEP> varWall_Step) {
		}

		public static void
		labelWallStepsTE(ProfileView objProfileViewWall, string strName, string strLayer, double dblScaleProfileView, List<double>dblElevs, List<WALL_STEP> varWall_Step) {
			string strDescBot = "", strDescTop = "";
			if (fWall2.opt_BLOCK.Checked) {
				strDescBot = " TF";
				strDescTop = " TW";
			}else {
				strDescBot = " BP";
				strDescTop = " TP";
			}

			List<string> strText = new List<string>();

			double dblSta = 0, dblElev = 0, dblDir = 0, dblDirTxt = 0;
			AttachmentPoint attachPnt = 0;
			
			switch (varWall_Step.Count) {
				case 1:

					dblSta = varWall_Step[0].BEGSTA;
					dblElev = varWall_Step[0].ELEV;

					if (strName == "TOF") {
						dblDir = 3 * PI / 2;
						dblDirTxt = dblDir - PI;
						attachPnt = AttachmentPoint.MiddleRight;
					}else if (strName == "TOW") {
						dblDir = PI / 2;
						dblDirTxt = dblDir;
						attachPnt = AttachmentPoint.MiddleLeft;
					}

					strText.Add(string.Format("{0:##+##.00}", dblSta));

					if (strName == "TOF") {
						strText.Add(string.Format("\\P{0:###.00}{1}", dblElev, strDescBot));
					}else {
						strText.Add(string.Format("\\P{0:###.00}{1}", dblElev, strDescTop));
					}

					addLabelWall(objProfileViewWall, dblSta, dblElev, dblDirTxt, strText, attachPnt, strLayer, dblScaleProfileView, strName);

					dblSta = varWall_Step[0].ENDSTA;
					dblElev = varWall_Step[0].ELEV;

					strText = new List<string>();
					strText.Add(string.Format("{0:##+##.00}", dblSta));

					if (strName == "TOF") {
						strText.Add(string.Format("\\P{0:###.00}{1}", dblElev, strDescBot));
					}else {
						strText.Add(string.Format("\\P{0:###.00}{1}", dblElev, strDescTop));
					}

					addLabelWall(objProfileViewWall, dblSta, dblElev, dblDirTxt, strText, attachPnt, strLayer, dblScaleProfileView, strName);

					break;
				default:
					
					double dblDYNext = 0, dblDYPrev = 0;
					
					bool boolDoBEG = false, boolDoEND = false;

					for (int i = 0; i < varWall_Step.Count; i++) {
						strText = new List<string>();

						if (i == 0) {
							dblDYNext = varWall_Step[i + 1].ELEV - varWall_Step[i + 0].ELEV;
							//next ELEV minus current ELEV             + step up   - step down

							boolDoBEG = true;

							if (strName == "TOF") {
								dblDir = 3 * PI / 2;
								dblDirTxt = dblDir - PI;
								attachPnt = AttachmentPoint.MiddleRight;
								if (dblDYNext < 0) {
									boolDoEND = false;
								}else {
									boolDoEND = true;
								}
							}else if (strName == "TOW") {
								dblDir = PI / 2;
								dblDirTxt = dblDir;
								attachPnt = AttachmentPoint.MiddleLeft;
								if (dblDYNext < 0) {
									boolDoEND = true;
								}else {
									boolDoEND = false;
								}
							}
						}else if (i == varWall_Step.Count - 1) {
							dblDYPrev = varWall_Step[i - 0].ELEV - varWall_Step[i - 1].ELEV;    //current ELEV minus previous ELEV       + step up   - step down
								
							boolDoEND = true;

							if (strName == "TOF") {
								dblDir = 3 * PI / 2;
								dblDirTxt = dblDir - PI;
								attachPnt = AttachmentPoint.MiddleRight;
								if (dblDYPrev < 0) {
									boolDoBEG = true;
								}else {
									boolDoBEG = false;
								}
							}else {
								dblDir = PI / 2;
								dblDirTxt = dblDir;
								attachPnt = AttachmentPoint.MiddleLeft;
								if (dblDYPrev < 0) {
									boolDoBEG = false;
								}else {
									boolDoBEG = true;
								}
							}
						}else {
							dblDYPrev = varWall_Step[i - 0].ELEV - varWall_Step[i - 1].ELEV;    //current ELEV minus previous ELEV       + step up   - step down
								
							dblDYNext = varWall_Step[i + 1].ELEV - varWall_Step[i + 0].ELEV;    //next ELEV minus current ELEV           + step up   - step down

							if (dblDYPrev > 0) {
								if (strName == "TOF") {
									dblDir = 3 * PI / 2;
									dblDirTxt = dblDir - PI;
									attachPnt = AttachmentPoint.MiddleRight;
								}else if (strName == "TOW") {
									dblDir = PI / 2;
									dblDirTxt = dblDir;
									attachPnt = AttachmentPoint.MiddleLeft;
								}

								if (strName == "TOF") {
									if (dblDYNext < 0) {
										boolDoBEG = false;
										boolDoEND = false;
									}else {
										boolDoBEG = false;
										boolDoEND = true;
									}
								}else if (strName == "TOW") {
									if (dblDYNext < 0) {
										boolDoBEG = true;
										boolDoEND = true;
									}else {
										boolDoBEG = true;
										boolDoEND = false;
									}
								}
							}else if (dblDYPrev < 0) {
								if (strName == "TOF") {
									if (dblDYNext > 0) {
										boolDoBEG = true;
										boolDoEND = true;
									}else {
										boolDoBEG = true;
										boolDoEND = false;
									}
								}else if (strName == "TOW") {
									if (dblDYNext > 0) {
										boolDoBEG = false;
										boolDoEND = false;
									}else {
										boolDoBEG = false;
										boolDoEND = true;
									}
								}
							}
						}

						if (boolDoBEG) {
							dblSta = varWall_Step[i].BEGSTA;
							dblElev = varWall_Step[i].ELEV;

							strText = new List<string>();
							strText.Add(string.Format("{0:##+##.00}", dblSta));

							if (strName == "TOF") {
								strText.Add(string.Format("\\P{0:###.00}{1}", dblElev, strDescBot));
							}else {
								strText.Add(string.Format("\\P{0:###.00}{1}", dblElev, strDescTop));
							}

							addLabelWall(objProfileViewWall, dblSta, dblElev, dblDirTxt, strText, attachPnt, strLayer, dblScaleProfileView, strName);
						}

						if (boolDoEND) {
							dblSta = varWall_Step[i].ENDSTA;
							dblElev = varWall_Step[i].ELEV;

							strText = new List<string>();
							strText.Add(string.Format("{0:##+##.00}", dblSta));

							if (strName == "TOF") {
								strText.Add(string.Format("\\P{0:###.00}{1}", dblElev, strDescBot));
							}else {
								strText.Add(string.Format("\\P{0:###.00}{1}", dblElev, strDescTop));
							}

							addLabelWall(objProfileViewWall, dblSta, dblElev, dblDirTxt, strText, attachPnt, strLayer, dblScaleProfileView, strName);
						}
					}

					break;
			}
		}

		public static void
		addLabelWall(ProfileView objProfileViewWall, double dblSta, double dblElev, double dblDirTxt, List<string> strText,
			AttachmentPoint attachPnt, string strLayer, double dblScaleProfileView, string strName) {
			Database DB = HostApplicationServices.WorkingDatabase;
			double intDrawUnits = DB.Cannoscale.DrawingUnits;
			int intStrLen = 0;

			if (strText[0].Length > strText[1].Length - 2) {
				intStrLen = strText[0].Length;
			}else {
				intStrLen = strText[1].Length - 2;           //\P
			}

			double dblStrLen = intStrLen * intDrawUnits * 0.09;

			ObjectId idTxtStyle = Txt.getTextStyleTableRecord("ANNO").ObjectId;
			double dblTxtHeight = 0.09;
			string strMText = strText[0] + strText[1];

			double dblX = 0;
			double dblY = 0;
			using (BaseObjs._acadDoc.LockDocument()) {
				using (Transaction TR = BaseObjs.startTransactionDb()) {
					objProfileViewWall.FindXYAtStationAndElevation(dblSta, dblElev, ref dblX, ref dblY);
				}
			}

			Point3d pnt3dTar = new Point3d(dblX, dblY, 0.0);

			double dX = 3 * System.Math.Cos(dblDirTxt);
			double dY = 0;
			if (strName == "TOF") {
				dY = 3 * System.Math.Sin(dblDirTxt + PI);
			}else {
				dY = 3 * System.Math.Sin(dblDirTxt);
			}

			Point3d pnt3dIns = new Point3d(pnt3dTar.X + dX, pnt3dTar.Y + dY, 0.0);
			MText objMText = null;
			using (BaseObjs._acadDoc.LockDocument()) {
				ObjectId idMText = Txt.addMText(strMText, pnt3dIns, dblDirTxt, dblStrLen, dblTxtHeight, attachPnt, nameLayer: strLayer);

				try {
					MText objLabel = Wall_Public.LABEL;
					if (objLabel != null){
						ObjectId idLabel = objLabel.ObjectId;

						List<Point3d> pnts3dInt = new List<Point3d>();
						if ((objLabel != null)) {
							if ((!objLabel.IsErased)) {
								idMText.intersectWith(idLabel, extend.none);

								if (pnts3dInt.Count > 0) {
									if (strLayer.Substring(9, 3) == "TOF") {
										pnt3dIns = new Point3d(pnt3dIns.X - 0.35 * dblScaleProfileView, pnt3dIns.Y, 0.0);
										idMText.setMTextLocation(pnt3dIns);
									}else {
										pnt3dIns = new Point3d(pnt3dIns.X + 0.35 * dblScaleProfileView, pnt3dIns.Y, 0.0);
										idMText.setMTextLocation(pnt3dIns);
									}
								}
							}
						}                        
					}                        
				}
				catch (Autodesk.AutoCAD.Runtime.Exception ) {
				}
			}

			ObjectId idLayer = Layer.manageLayers(strLayer + "-DIM");

			Wall_Public.LABEL = objMText;
			Point3d pnt3dEnd = default(Point3d);
			if (strName == "TOW") {
				pnt3dEnd = new Point3d(pnt3dTar.X, pnt3dTar.Y + dblStrLen, 0.0);
			}else if (strName == "TOF") {
				pnt3dEnd = new Point3d(pnt3dTar.X, pnt3dTar.Y - dblStrLen, 0.0);
			}

			Point3dCollection pnts3dLdr = new Point3dCollection();
			pnts3dLdr.Add(pnt3dTar);
			pnts3dLdr.Add(pnt3dEnd);

			using (BaseObjs._acadDoc.LockDocument()) {
				Ldr.addLdr(pnts3dLdr, idLayer, dblTxtHeight, 0.2, clr.byl, ObjectId.Null);
			}
		}

		public static void
		adjustLeaderInsPnt() {
			Database DB = HostApplicationServices.WorkingDatabase;
			double intDrawUnits = DB.Cannoscale.DrawingUnits;
			double dblScale = 0;

			Point3d pnt3d = Pub.pnt3dO;
			ProfileView objProfileView = (ProfileView)Base_Tools45.Select.selectEntity(typeof(ProfileView), "Select Profile View: ", "Profile View Selection failed.", out pnt3d);
			List<Point3d> pnts3d = Prof.getProfileViewOriginAndScale(objProfileView.ObjectId, out dblScale);

			SelectionSet objSSet = Base_Tools45.Select.buildSSet(typeof(Leader), pnts3d[0], pnts3d[1]);
			ObjectId[] ids = objSSet.GetObjectIds();

			for (int i = 0; i < ids.Length; i++) {
				ObjectId idLeader = ids[i];
				ObjectId idMtext = Ldr.getLdrAnnotationObject(idLeader);

				string nameLayer = idMtext.getLayer();
			   
				string strName = nameLayer.Substring(nameLayer.Length - 2); //last two characters

				string strMText = idMtext.getMTextText();

				string[] strText = strMText.Split(new char[] { '\'' });

				double intStrLen = 0;

				if (strText[0].Length > strText[1].Length - 1) {
					intStrLen = strText[0].Length;
				}else {
					intStrLen = strText[1].Length - 1;  //\P
				}

				intStrLen = intStrLen * intDrawUnits * 0.09;

				Point3d pnt3dMTextEnd = default(Point3d);
				Point3d pnt3dIns = idMtext.getMTextLocation();
				if (strName == "TOW") {
					pnt3dMTextEnd = new Point3d(pnt3dIns.X, pnt3dIns.Y + intStrLen / 2, 0.0);
				}else {
					pnt3dMTextEnd = new Point3d(pnt3dIns.X, pnt3dIns.Y - intStrLen / 2, 0.0);
				}

				if (Ldr.getLdrVerticeCount(idLeader) == 3) {
					Ldr.setLdrVertex(idLeader, 1, idMtext.getMTextLocation());
					Ldr.setLdrVertex(idLeader, 2, pnt3dMTextEnd);
				}
			}
		}
	}
}