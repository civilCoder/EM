using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

using Base_Tools45;
using Base_Tools45.C3D;

using System.Collections.Generic;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace LdrText
{
	public static class LdrText_ProcessCmds
	{
		public static void
		ProcessCmds(string nameCmd)
		{
			ObjectId idCgPnt1 = ObjectId.Null;
			ObjectId idCgPnt2 = ObjectId.Null;
			ObjectId idLDR = ObjectId.Null;

			List<ObjectId> idsCgPnts = new List<ObjectId>();

			Point3d pnt3d, pnt3d1, pnt3d2, pnt3dIns, pnt3dLdr1, pnt3dLdr2;
			string prompt;
			const string prompt1 = "Pick the Arrow Starting Point: ";

			string resElev = string.Empty, resElev1, resElev2, resTop = string.Empty, resBot = string.Empty, resBot2 = string.Empty,
			resSlab, resCurb, resPrefix, resSuffix, resDesc, resElevSuf = string.Empty;

			double deltaZ = 0.0;
			double pi = System.Math.PI;
			bool escape = true;
			PromptStatus ps = PromptStatus.Cancel;

			switch (nameCmd)
			{
				case "cmdCF0":
					resElev1 = UserInput.getCogoPoint(prompt1, out idCgPnt1, Pub.pnt3dO, osMode: 8);
					resTop = string.Format("{0} TC", resElev1);
					resBot = "0\" CF";
					pnt3d = idCgPnt1.getCogoPntCoordinates();
					//idLDR = LdrText_JigLeader0.jigLeader0(pnt3d, 0.09, "ARROW", 7);
					idLDR = Ldr.drawLdr(pnt3d, 0.09, "ARROW", 7);
					if (idLDR == ObjectId.Null)
						return;
					idsCgPnts.Add(idCgPnt1);
					Txt.addLdrText(nameCmd, apps.lnkCO, idLDR, idsCgPnts, resTop, resBot);
					break;

				case "cmdDEP":
				case "cmdGQ":
				case "cmdWQ":
					idCgPnt1 = CgPnt.selectCogoPointByNode("\nSelect Low Point : ", osMode: 8);
					if (idCgPnt1 == null)
						return;
					pnt3d1 = idCgPnt1.getCogoPntCoordinates();

					idCgPnt2 = CgPnt.selectCogoPointByNode("\nSelect High Point : ", osMode: 8);
					if (idCgPnt2 == null)
						return;
					pnt3d2 = idCgPnt2.getCogoPntCoordinates();
					if (pnt3d2 == Pub.pnt3dO)
						return;
					deltaZ = System.Math.Abs(pnt3d1.Z - pnt3d2.Z);
					switch (nameCmd)
					{
						case "cmdDEP":
							resTop = "DEEPEN";
							resBot = string.Format("PANEL= {0:F2}'", deltaZ);
							break;
						case "cmdGQ":
							resTop = "CONSTRUCT GRAVITY WALL";
							string resDelta = "0.50";
							escape = UserInput.getUserInputDoubleAsString("\nEnter Additional Height in feet: ", out resDelta, resDelta);
							if (escape)
								return;
							resBot = string.Format("H= {0:F2}'", deltaZ + double.Parse(resDelta));
							break;
						case "cmdWQ":
							resTop = "CONSTRUCT RETAINING WALL";
							resBot = string.Format("H= {0:F2}'", deltaZ + 1.5);
							break;
					}
					//idLDR = LdrText_JigLeader0.jigLeader0(pnt3d2, 0.09, "ARROW", 7);
					idLDR = Ldr.drawLdr(pnt3d2, 0.09, "ARROW", 7);

					if (idLDR == ObjectId.Null)
						return;
					idsCgPnts = new List<ObjectId> {
						idCgPnt1,
						idCgPnt2
					};

					Txt.addLdrText(nameCmd, apps.lnkDP, idLDR, idsCgPnts, resTop, resBot);
					break;

				case "cmdFF":
				case "cmdFFD":
					resElev1 = UserInput.getCogoPoint(prompt1, out idCgPnt1, Pub.pnt3dO, osMode: 8);
					pnt3d = idCgPnt1.getCogoPntCoordinates();
					resTop = string.Format("{0} FF", resElev1);
					if (nameCmd == "cmdFFD")
					{
						idLDR = ObjectId.Null;
					}
					else if (nameCmd == "cmdFF")
					{
						//idLDR = LdrText_JigLeader0.jigLeader0(pnt3d, 0.09, "ARROW", 7);
						idLDR = Ldr.drawLdr(pnt3d, 0.09, "ARROW", 7);
						if (idLDR == ObjectId.Null)
							return;
					}
					idsCgPnts.Add(idCgPnt1);
					//resSlab = Dict.getCmdDefault(nameCmd, "resSlab");
					resSlab = "0.50";                                                                       //per M. Roberts 6/21/2015
					escape = UserInput.getUserInputDoubleAsString("\nEnter Slab Thickness:", out resSlab, resSlab);
					if (escape){
						if(idLDR != ObjectId.Null){
							idLDR.delete();                            
						}
						return;
					}
					deltaZ = double.Parse(resSlab);
					resBot = string.Format("{0} PAD", (double.Parse(resElev1) - deltaZ).ToString("F2"));
					Txt.addLdrText(nameCmd, apps.lnkCO, idLDR, idsCgPnts, resTop, resBot, "", deltaZ);
					break;

				case "cmdFL":
				case "cmdFLX":
					resElev1 = UserInput.getCogoPoint(prompt1, out idCgPnt1, Pub.pnt3dO, osMode: 8);
					if (resElev1 == string.Empty)
						return;
					pnt3d = idCgPnt1.getCogoPntCoordinates();
					//idLDR = LdrText_JigLeader0.jigLeader0(pnt3d, 0.09, "ARROW", 7);
					idLDR = Ldr.drawLdr(pnt3d, 0.09, "ARROW", 7);
					if (idLDR == ObjectId.Null)
						return;
					idsCgPnts.Add(idCgPnt1);

					resBot = Dict.getCmdDefault(nameCmd, "resBot");
					if (resBot == string.Empty)
						resBot = "FL";

					prompt = string.Format("\nEnter Bottom Text: FL/FS/Other <{0}> [FL/FS]:", resBot);
					escape = UserInput.getUserInputKeyword(resBot, out resBot, prompt, "FL FS");
					if (escape){
						idLDR.delete();
						return;
					}

					if (nameCmd == "cmdFLX")
						resTop = string.Format("({0})", resElev1);
					else
						resTop = resElev1;

					Txt.addLdrText(nameCmd, apps.lnkCO, idLDR, idsCgPnts, resTop, resBot);
					Dict.setCmdDefault(nameCmd, "resBot", resBot);

					break;

				case "cmdG":
				case "cmdGX":
					resElev1 = UserInput.getCogoPoint(prompt1, out idCgPnt1, Pub.pnt3dO, osMode: 8);
					if (resElev1 == string.Empty)
						return;
					pnt3d = idCgPnt1.getCogoPntCoordinates();
					//idLDR = LdrText_JigLeader0.jigLeader0(pnt3d, 0.09, "ARROW", 7);
					idLDR = Ldr.drawLdr(pnt3d, 0.09, "ARROW", 7);
					if (idLDR == ObjectId.Null)
						return;
					idsCgPnts.Add(idCgPnt1);

					resBot = Dict.getCmdDefault(nameCmd, "resBot");
					if (resBot == string.Empty)
						resBot = "FL";

					prompt = string.Format("\nBottom Label is: FL/FS/Other <{0}>: [FL/FS]: ", resBot);
					escape = UserInput.getUserInputKeyword(resBot, out resBot, prompt, "FL FS");
					if (escape){
						idLDR.delete();
						return;
					}

					resTop = Dict.getCmdDefault(nameCmd, "resTop");
					if (resTop == "")
						resTop = "TC";
					prompt = string.Format("\nTop Label is: TC/Other <{0}>: [TC]: ", resTop);
					escape = UserInput.getUserInputKeyword(resTop, out resTop, prompt, "TC");
					if (escape){
						idLDR.delete();
						return;
					}

					resCurb = Dict.getCmdDefault(nameCmd, "resCurb");
					if (resCurb == "")
						resCurb = "0.50";
					escape = UserInput.getUserInputDoubleAsString(string.Format("\nEnter Offset for {0}", resTop), out resCurb, resCurb);
					if (escape){
						idLDR.delete();
						return;
					}

					resElev2 = string.Format("{0:F2}", double.Parse(resElev1) + double.Parse(resCurb));
					string resBotMod = string.Format("{0}{1}", resElev1, resBot);
					string resTopMod = string.Format("{0}{1}", resElev2, resTop);
					if (nameCmd == "cmdGX")
					{
						resBotMod = string.Format("({0})", resBotMod);
						resTopMod = string.Format("({0})", resTopMod);
					}

					Txt.addLdrText(nameCmd, apps.lnkCO, idLDR, idsCgPnts, resTopMod, resBotMod, deltaZ: double.Parse(resCurb));
					Dict.setCmdDefault(nameCmd, "resTop", resTop);
					Dict.setCmdDefault(nameCmd, "resBot", resBot);
					Dict.setCmdDefault(nameCmd, "resCurb", resCurb);
					break;

				case "cmdGS":
				case "cmdGS0":
				case "cmdGS2":
				case "cmdGS3":
				case "cmdGSE":
				case "cmdGSS":
				case "cmdGSX":
				case "cmdSL":
				case "cmdSS":
					try
					{
						if (nameCmd == "cmdGSE")
						{
							pnt3d1 = UserInput.getPoint("\nSelect First Endpoint: ", out ps, osMode: 1);
							if (pnt3d1 == Pub.pnt3dO)
								return;
							pnt3d2 = UserInput.getPoint("\nSelect Second Endpoint: ", out ps, osMode: 1);
							if (pnt3d2 == Pub.pnt3dO)
								return;
						}
						else if (nameCmd == "cmdGSX")
						{
							resElev1 = UserInput.getPoint("\nSelect First Point: ", out idCgPnt1, out pnt3d, Pub.pnt3dO, osMode: 8);
							if (resElev1 == string.Empty)
								return;
							if (idCgPnt1 == ObjectId.Null)
								pnt3d1 = pnt3d;
							else
								pnt3d1 = idCgPnt1.getCogoPntCoordinates();

							resElev2 = UserInput.getPoint("\nSelect Second Point: ", out idCgPnt2, out pnt3d, pnt3d1, osMode: 8);
							if (resElev2 == string.Empty)
								return;
							if (idCgPnt2 == ObjectId.Null)
								pnt3d2 = pnt3d;
							else
								pnt3d2 = idCgPnt2.getCogoPntCoordinates();
						}
						else
						{                              //cmd GS, GS0, GS2, GS3, GSE, GSS, GSX, SS, SL
							resElev1 = UserInput.getCogoPoint("\nSelect First Point: ", out idCgPnt1, Pub.pnt3dO, osMode: 8);
							if (resElev1 == string.Empty)
								return;
							pnt3d1 = idCgPnt1.getCogoPntCoordinates();

							resElev2 = UserInput.getCogoPoint("\nSelect Second Point: ", out idCgPnt2, pnt3d1, osMode: 8);
							if (resElev2 == string.Empty)
								return;
							pnt3d2 = idCgPnt2.getCogoPntCoordinates();
						}

						double scale = Misc.getCurrAnnoScale();
						double angle = pnt3d1.getDirection(pnt3d2);
						double dxy = pnt3d1.getDistance(pnt3d2);
						double dz = pnt3d2.Z - pnt3d1.Z;
						double slope = dz / dxy;

						Pub.Slope = slope;
						Pub.dZ = dz;

						if (pnt3d2.Z > pnt3d1.Z)            //nominal case is sloping downhill - arrow points to lower grade
							angle = angle + System.Math.PI;

						string message = string.Format("\nHorz = {0:F4} : Vert = {1:F4} : R = {2:F2}%\r", dxy, dz, slope * 100);
						BaseObjs._editor.WriteMessage(message);

						ps = PromptStatus.Cancel;
						pnt3dIns = UserInput.getPoint("\nPick Insertion Point for Slope: ", out ps, osMode: 0);
						if (ps == PromptStatus.Cancel)
							return;
						if (ps == PromptStatus.None)
							pnt3dIns = pnt3d1.getMidPoint2d(pnt3d2);

						double factor = 0.0;
						slope = System.Math.Abs(slope);
						switch (nameCmd)
						{
							case "cmdGS":
								resElev = string.Format("R={0:F2}%", slope * 100);
								factor = 3.8;
								break;

							case "cmdGSE":
								resElev = string.Format("R={0:F4}%", slope * 100);
								factor = 3.8;
								break;

							case "cmdGS0":
								resElev = string.Format("{0:F0}%", Base_Tools45.Math.roundUP3(slope) * 100);
								factor = 1.5;
								break;

							case "cmdGS2":
								resElev = string.Format("R={0:F2}%", slope * 100);
								factor = 3.0;
								break;

							case "cmdGS3":
								resElev = string.Format("{0:F0}%", Base_Tools45.Math.roundUP3(slope) * 100);
								factor = 1.2;
								break;

							case "cmdGSS":
								resElev = string.Format("{0:F1}%", slope * 100);
								factor = 2.0;
								break;

							case "cmdGSX":
								resElev = string.Format("(R={0:F2}%)", slope * 100);
								factor = 3.8;
								break;

							case "cmdSL":
								resElev = string.Format("S={0:F4}", slope);
								factor = 3.0;
								break;
						}
						pnt3dLdr1 = pnt3dIns.traverse(angle, 0.09 * scale * factor);
						pnt3dLdr2 = pnt3dIns.traverse(angle + pi, 0.09 * scale * factor);

						Point3dCollection pnts3dLdr = new Point3dCollection {
							pnt3dLdr1,
							pnt3dLdr2
						};
						ObjectId idLayer = Layer.manageLayers("ARROW");
						idLDR = Ldr.addLdr(pnts3dLdr, idLayer, 0.09, 0.0, clr.byl, ObjectId.Null);

						idsCgPnts = new List<ObjectId> {
							idCgPnt1,
							idCgPnt2
						};

						List<Point3d> pnts3d = new List<Point3d> { pnt3d1, pnt3d2 };

						if (nameCmd == "cmdGS3")
						{
							Txt.addLdrText(nameCmd, apps.lnkGS, idLDR, idsCgPnts, resElev, "", "", 0.0, 0.7, pnts3dX: pnts3d);
						}
						else
						{
							Txt.addLdrText(nameCmd, apps.lnkGS, idLDR, idsCgPnts, resElev, "", "", 0.0, pnts3dX: pnts3d);          //pnts3d added to allow using something other than a CogoPoint
						}
					}
					catch (System.Exception ex)
					{
						BaseObjs.writeDebug(ex.Message + " LdrText_ProcessCmds.cs: line: 313");
					}
					break;

				case "cmdLD":
					//resElev1 = UserInput.getCogoPoint(prompt1, out idCgPnt1, Pub.pnt3dOrg, osMode: 8);
					//pnt3d = idCgPnt1.getCogoPntCoordinates();
					//idLDR = LdrText_JigLeader0.jigLeader0(pnt3d, 0.09, "ARROW", 7);
					pnt3d = UserInput.getPoint(prompt1, out ps, 521);

					idLDR = Ldr.drawLdr(pnt3d, 0.09, "ARROW", 7);
					if (idLDR == ObjectId.Null)
						return;
					resTop = Dict.getCmdDefault(nameCmd, "resTop");
					bool cancel = true;
					cancel = UserInput.getUserInput(resTop, "\nEnter Top Text: ", out resTop, true);
					if (cancel || resTop == string.Empty)
					{
						idLDR.delete();
						return;
					}
					resBot = Dict.getCmdDefault(nameCmd, "resBot");
					cancel = true;
					cancel = UserInput.getUserInput(resBot, "\nEnter Bottom Text: ", out resBot, true);

					Txt.addLdrText(nameCmd, apps.lnkLD, idLDR, null, resTop, resBot);

					Dict.setCmdDefault(nameCmd, "resTop", resTop);  //OK
					Dict.setCmdDefault(nameCmd, "resBot", resBot);  //OK

					break;

				case "cmdLLA":
					bool canLdr;
					Handle hEntX = "0000".stringToHandle();
					string nameLayer = "";
					FullSubentityPath path = new FullSubentityPath();
					Entity ent = Ldr.getFirstLdrPoint(out pnt3d, out canLdr, out hEntX, out nameLayer, out path);
					if (ent == null)
						return;
					idLDR = Ldr.drawLdr(pnt3d, 0.09, "ARROW", 7);
					if (idLDR == ObjectId.Null)
						return;
					resTop = nameLayer;
					resBot = "";
					Txt.addLdrText(nameCmd, apps.lnkLD, idLDR, null, resTop, resBot);

					break;

				case "cmdLLG":
					string zone = "405";
					prompt = string.Format("\nEnter the California SPCS Zone: <{0}>: [401/402/403/404/405/406]: ", zone);
					escape = UserInput.getUserInputKeyword(zone, out zone, prompt, "401 402 403 404 405 406");
					if (escape)
						return;
					pnt3d = UserInput.getPoint("\nSelect a GRID point: ", out ps, osMode: 8);
					if (pnt3d == Pub.pnt3dO)
						return;
					//idLDR = LdrText_JigLeader0.jigLeader0(pnt3d, 0.09, "ARROW", 7);
					idLDR = Ldr.drawLdr(pnt3d, 0.09, "ARROW", 7);
					if (idLDR == ObjectId.Null)
						return;
					LdrText_cmdLLG.cmdLLG(zone, pnt3d.Y, pnt3d.X, out resTop, out resBot);
					Txt.addLdrText(nameCmd, apps.lnkLD, idLDR, null, resTop, resBot);
					break;

				case "cmdRQ":
				case "cmdRiser":
					resElev1 = UserInput.getPoint("\nSelect Low Point : ", out idCgPnt1, out pnt3d, Pub.pnt3dO, osMode: 8);
					if (resElev1 == string.Empty)
						return;
					if (idCgPnt1 == ObjectId.Null)
						pnt3d1 = pnt3d;
					else
						pnt3d1 = idCgPnt1.getCogoPntCoordinates();

					resElev2 = UserInput.getPoint("\nSelect High Point: ", out idCgPnt2, out pnt3d, pnt3d1, osMode: 8);
					if (resElev2 == string.Empty)
						return;
					if (idCgPnt2 == ObjectId.Null)
						pnt3d2 = pnt3d;
					else
						pnt3d2 = idCgPnt2.getCogoPntCoordinates();

					idLDR = Ldr.drawLdr(pnt3d2, 0.09, "ARROW", 7);
					if (idLDR == ObjectId.Null)
						return;

					if (pnt3d1.Z > pnt3d2.Z)
					{
						resElev = resElev1;
						resElev1 = resElev2;
						resElev2 = resElev;
						pnt3d = pnt3d1;
						pnt3d1 = pnt3d2;
						pnt3d2 = pnt3d;
					}

					double nomRiser = 0.583;
					double aveRiser = 0;
					int numRiser = 1;

					deltaZ = pnt3d2.Z - pnt3d1.Z;

					if (deltaZ < 0.333) //4"
						Application.ShowAlertDialog("\nElevation difference is less than 4 in. - revise landing elevation");
					else if (deltaZ <= nomRiser)
					{   //7"
						aveRiser = deltaZ;
					}
					else if (deltaZ <= 0.667) //8"
						Application.ShowAlertDialog("\nElevation difference is greater than 7 in.  and less than 8 in. - therefore landing elevation needs to be adjusted");
					else
					{ // > 8"
						if (deltaZ.mod(nomRiser) == 0)
						{
							numRiser = (int)System.Math.Truncate(deltaZ / nomRiser);
						}
						else
						{
							numRiser = (int)System.Math.Truncate(deltaZ / nomRiser);
							numRiser++;
						}
						aveRiser = deltaZ / numRiser;

						if (aveRiser < 0.333)
						{
							numRiser--;
							aveRiser = deltaZ / numRiser;
						}
					}

					string minRiser = (aveRiser * 12.0).decimalToFraction();
					if (nameCmd == "cmdRiser")
					{
						resTop = string.Format("({0}) {1} ", numRiser, minRiser);
						resBot = "RISERS";
					}
					else if (nameCmd == "cmdRQ")
					{
						double lenRamp = deltaZ / 0.0833;
						resTop = string.Format("CONSTRUCT {0:F2} FT HC RAMP", lenRamp);
						resBot = string.Format("({0}) RISERS {1}", numRiser, minRiser);
					}
					Txt.addLdrText(nameCmd, apps.lnkLD, idLDR, null, resTop, resBot);

					break;

				case "cmdSDE":
				case "cmdSDS":
				case "cmdSED":
					Point3d pnt3dPicked = Pub.pnt3dO;
					string nameAlign = string.Empty;
					ObjectId idAlign = Align.selectAlign("Select Alignment:\n", "Alignment not found! Retry\n", out pnt3dPicked, out nameAlign);
					if (idAlign == ObjectId.Null)
						return;
					bool go = true;
					do{					    
						BaseObjs.write(string.Format("Active Alignment is: {0}\n", nameAlign));
						pnt3d = UserInput.getPoint("Pick Arrow Start Point:\n", out ps, osMode: 8);
						if (pnt3d == Pub.pnt3dO)
							return;
						pnt3d1 = pnt3d; //store elevation from start point

						idCgPnt1 = Select.selectCogoPntAtPoint3d(pnt3d1);
						idsCgPnts.Add(idCgPnt1);

						//idLDR = LdrText_JigLeader0.jigLeader0(pnt3dTar, 0.09, "ARROW", 7);
						idLDR = Ldr.drawLdr(pnt3d, 0.09, "ARROW", 7);
						if (idLDR == ObjectId.Null)
							return;
						escape = false;
						resPrefix = Dict.getCmdDefault(nameCmd, "resPrefix");
						if (resPrefix == "")
							resPrefix = "STA";
						prompt = string.Format("\nEnter Station Prefix: STA/Other/. for None <{0}>: [STA/.]: ", resPrefix);
						escape = UserInput.getUserInputKeyword(resPrefix, out resPrefix, prompt, "STA .");
						if (escape){
							idLDR.delete();
							return;
						}

						escape = false;
						resSuffix = Dict.getCmdDefault(nameCmd, "resSuffix");
						if (resSuffix == "")
							resSuffix = ".";
						prompt = string.Format("\nEnter Station Suffix: BC/EC/Other/. for None <{0}>: [BC/EC/.]: ", resSuffix);
						escape = UserInput.getUserInputKeyword(resSuffix, out resSuffix, prompt, "BC EC .");
						if (escape){
							idLDR.delete();
							return;
						}

						escape = false;
						resDesc = Dict.getCmdDefault(nameCmd, "resDesc");
						if (resDesc == "")
							resDesc = nameAlign;
						prompt = string.Format("\nEnter Station Desc/. for None <{0}>: [.]: ", resDesc);
						escape = UserInput.getUserInputKeyword(resDesc, out resDesc, prompt, ".");
						if (escape){
							idLDR.delete();
							return;
						}

						if (nameCmd == "cmdSDE" || nameCmd == "cmdSED")
						{
							escape = false;
							resElevSuf = Dict.getCmdDefault(nameCmd, "resElevSuf");
							if (resElevSuf == "")
								resElevSuf = ".";
							prompt = string.Format("\nEnter Elevation Suffix: INV/TC/FS/Other/. for None <{0}>: [.]: ", resElevSuf);
							escape = UserInput.getUserInputKeyword(resElevSuf, out resElevSuf, prompt, ".");
							if (escape){
								idLDR.delete();
								return;
							}
						}

						double station = 0.0, offset = 0.0;
						pnt3d = idLDR.getBegPnt();
						Align.getAlignStaOffset(idAlign, pnt3d, ref station, ref offset);
						if (resPrefix == ".")
							resPrefix = "";
						if (resSuffix == ".")
							resSuffix = "";

						resTop = string.Format("{0} {1} {2}", resPrefix, station.ToString("#####0+00.00"), resSuffix);
						if (resDesc == ".")
							resDesc = "";
						resBot = resDesc;

						if (nameCmd == "cmdSDE" || nameCmd == "cmdSED")
						{
							resBot2 = string.Format("{0:F2} {1}", pnt3d1.Z, resElevSuf);
						}

						Txt.addLdrText(nameCmd, apps.lnkCO, idLDR, idsCgPnts, resTop, resBot, resBot2);
					} while (go);

					Dict.setCmdDefault(nameCmd, "resPrefix", resPrefix);
					Dict.setCmdDefault(nameCmd, "resSuffix", resSuffix);
					Dict.setCmdDefault(nameCmd, "resDesc", resDesc);
					Dict.setCmdDefault(nameCmd, "resElevSuf", resElevSuf);
					break;

				//case "cmdSF1":
				//    break;

				//case "cmdSF2":
				//    break;

				//case "cmdSLG":
				//    break;

				//case "cmdSS1":
				//    break;

				//case "cmdSS2":
				//    break;
				case "cmdPFA":
					break;

				case "cmdPFBEG":
					break;

				case "cmdPFDIM":
					break;

				case "cmdPFDIML":
					break;

				case "cmdPFDIMS":
					break;

				case "cmdPFDM":
					break;

				case "cmdPFDP":
					break;

				case "cmdPFED":
					break;

				case "cmdPFEND":
					break;

				case "cmdPFES":
					break;

				case "cmdPFI":
					break;

				case "cmdPFIS":
					break;

				case "cmdPFS":
					break;

				case "cmdPFSDE":
					break;

				case "cmdPFSIE":
					break;

				case "cmdPFXI":
					break;

				case "cmdPLA":
					break;

				case "cmdPLBD":
					break;

				case "cmdPLBDS":
					break;

				case "cmdPLCD":
					break;

				case "cmdPLEND":
					break;

				case "cmdPLBEG":
					break;
				//case "cmdPLMH":
				//    osMode = SnapMode.getOSnap();
				//    SnapMode.setOSnap(0);
				//    Point3d pnt3dPicked = Pub.pnt3dOrg;
				//    Algn align = (Algn)Base_Tools45.Select.selectEntity(typeof(Algn), "Select Algn", "Algn Selection Failed", out pnt3dPicked);

				//    SnapMode.setOSnap((int)osMode);
				//    break;
				case "cmdPLX":
					break;

				case "cmdVB":
					break;

				case "cmdVG":
					break;

				case "cmdPFSDM":
					break;

				case "cmdPFSDP":
					break;

				case "cmdPFSI":
					break;

				case "cmdPLSCO":
					break;

				case "cmdPLSMH":
					break;

				case "cmdSBD":
					break;

				case "cmdSE":
					break;

				case "cmdSSZ":
					pnt3d = UserInput.getPoint("\nSelect Node to label: ", out ps, osMode: 8);
					if (ps != PromptStatus.OK)
						return;
					//idLDR = LdrText_JigLeader0.jigLeader0(pnt3d, 0.09, "ARROW", 7);
					idLDR = Ldr.drawLdr(pnt3d, 0.09, "ARROW", 7);
					if (idLDR == ObjectId.Null)
						return;
					if (resBot == string.Empty)
						resBot = "TOP";

					prompt = string.Format("\nEnter Bottom Text: TOP/TOE/Other <{0}> [TOP/TOE]:", resBot);
					escape = UserInput.getUserInputKeyword(resBot, out resBot, prompt, "TOP TOE");
					if (escape){
						idLDR.delete();
						return;
					}

					resTop = string.Format("{0:F2}", pnt3d.Z);
					idsCgPnts = null;

					Txt.addLdrText(nameCmd, apps.lnkCO, idLDR, idsCgPnts, resTop, resBot);
					Dict.setCmdDefault(nameCmd, "resBot", resBot);

					break;
			}
		}
	}
}