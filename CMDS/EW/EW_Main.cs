using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;
using Color = Autodesk.AutoCAD.Colors.Color;

using Autodesk.AutoCAD.Windows;

using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices;

using Base_Tools45;
using Base_Tools45.C3D;

using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System;

using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

using pub = EW.EW_Pub;

namespace EW {

	public static class EW_Main {
	
		static DateTime timeEnd;
		static DateTime timeBeg;
		static bool exists = false;
		static bool escape = false;
		static Color color = null;
		static SelectionSet ss = null;

		static PaletteSet _ps = null;
		static Forms.winEW _winEW = null;

		public static void
		runEW(){
			
			if(_ps == null){
				_ps = new PaletteSet(
					"EW", 
					new Guid("{8E2AE1E0-D31D-4A8E-B7AD-E91D92CEF06F}"));
			}

			_ps.Size = new Size(280, 480);
			_ps.DockEnabled = (DockSides)((int)DockSides.Left + (int)DockSides.Right);

			_winEW = new Forms.winEW();
			_ps.AddVisual("EARTHWORK", _winEW);
			//PaletteSetStylesCollection styles = new PaletteSetStyles();
 
			_ps.SizeChanged += 
				delegate(object sender, PaletteSetSizeEventArgs e){
				_winEW.Width = e.Width;
				_winEW.Height = e.Height;
				};

			_ps.KeepFocus = true;
			_ps.Visible = true;
		}

		public static void viewResults(string strName, bool boolMake) {
			BaseObjs._acadDoc.SendStringToExecute("-UNITS\r2\r3\r1\r5\r0.0\rN\r", true, false, false);
			BaseObjs._acadDoc.SendStringToExecute("-VPOINT\rR\r300\r10\r", true, false, false);
			BaseObjs.updateGraphics();

			string strMessage = null;

			if (boolMake) {
				TimeSpan timeSpan = timeEnd - timeBeg;

				strMessage = string.Format("Elapsed Time: {0} \n Select OK to return to PLAN view and create surface {1} or CANCEL to keep current view and return to AutoCAD", timeSpan, strName);
			}else {
				strMessage = "Select OK to return to PLAN view or CANCEL to keep current view";
			}

			timeEnd = DateTime.Now;

			DialogResult varResponse = MessageBox.Show(strMessage, "CHECK FOR SPIKES", MessageBoxButtons.OKCancel);

			switch (varResponse) {
				case DialogResult.OK:

					BaseObjs._acadDoc.SendStringToExecute("PLAN\rW\r", true, false, false);
					if (boolMake) {
						switch (strName) {
							case "OX":
							case "OXg":
							case "SG":
								EW_MakeSurfaceSG_OX.makeSurface(strName);
								break;
						}
					}
					return;
			}
		}

		public static void makeSurfaceSG() {

			//Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.Command("_ucs", "w", "");
			BaseObjs.sendCommand("_ucs\rw\r");
			TinSurface objSurfaceCPNT = Surf.getTinSurface("CPNT-ON", out exists);
			
			SelectionSet objSSet = null;
			if (pub.boolDebug) {
				objSSet = EW_Utility1.buildSSet1(); //SelectOnScreen _XX-*               
			}else {
				objSSet = EW_Utility1.buildSSet0(); //get _XX-*               
			}
			if (objSSet == null || objSSet.Count == 0)
				return;

			ObjectId[] ids = objSSet.GetObjectIds();

			timeBeg = DateTime.Now;

			ObjectId idPolyT = ObjectId.Null;
			for (int i = 0; i < ids.Length; i++) {
				Polyline objLWPline = (Polyline)ids[i].getEnt();

				ObjectId idLWPlineT = objLWPline.copy();
				if (idLWPlineT.IsNull){
					MessageBox.Show("Null Id @ line 131 EW_Main");
					continue;
				}

				idLWPlineT.changeProp(LineWeight.LineWeight050, clr.yel);
				
				if(objLWPline.HasBulges){
					idPolyT = Conv.processBndry(idLWPlineT);    //boundary with arcs converted to line segments
					idLWPlineT.delete();
				}else{
					Entity ent = idLWPlineT.getEnt();
					idPolyT = ent.ObjectId;
				}

				//Entity entx = idPolyT.getEnt();
				//MessageBox.Show(entx.GetType().ToString());

				switch (objLWPline.Layer) {
					case "_XX-FLOOR SLAB_A":
					case "_XX-FLOOR SLAB_B":
					case "_XX-FLOOR SLAB_C":
					case "_XX-DOCK APRON":
						color = clr.yel;
						break;
					default:
						color = clr.grn;
						break;
				}

				ObjectId idLWPlineX = ObjectId.Null;
				if(!objLWPline.Layer.Contains("_XX-FLOOR")){
					idLWPlineX = idPolyT.offset(-0.05);                    
					idPolyT.delete();
				}else{
					idLWPlineX = idPolyT;
				}

				if (idLWPlineX.IsNull)
				{
					MessageBox.Show("Null Id @ line 162 EW_Main" + idPolyT.getHandle().ToString());
					continue;
				}

				idLWPlineX.changeProp(LineWeight.LineWeight060, color);


				if (!offsetSegments("CPNT-ON", "SG", idLWPlineX, objLWPline.Layer)) {
					return;
				}
			}

			viewResults("SG", true);
		}

		public static void makeSurfaceOX() {
			bool boolFirstPass = false;

			timeBeg = DateTime.Now;

			Layer.manageLayers("SG-BRKLINE");
			Layer.manageLayers("_YY-BLDG LIM");

			SelectionSet objSSetBLDG_LIM = EW_Utility1.buildSSetBLDG_LIM(); //get _YY-BLDG LIM(s)
			
			SelectionSet objSSetFLOOR_SLAB = EW_Utility1.buildSSetFLOOR_SLAB(); //get _XX-FLOOR SLAB(s)
			
			if (objSSetBLDG_LIM.Count == 0) {
				//  For i = 0 To objSSetFLOOR_SLAB.Count - 1
				//
				//    Set objFLOORSLAB = objSSetFLOOR_SLAB.Item(i)
				//    strLayer = objFLOORSLAB.Layer
				//
				//    objFLOORSLAB.Closed = False
				//    Call forceClosed(objFLOORSLAB)
				//
				//    varXDataVal = Empty
				//    objFLOORSLAB.getXdata "OX", varXDataType, varXDataVal
				//
				//    If Not IsEmpty(varXDataVal) Then
				//      intBldgNo = varXDataVal(1)
				//    Else
				//      intBldgNo = i
				//    End If
				//
				//    Set objBldgOX2 = getBldgOX2(objFLOORSLAB, intBldgNo, "MAKE", strLayer)               'get BLDG OX limits
				//    objBldgOX2.color = acMagenta
				//    objBldgOX2.Lineweight = acLnWt100
				//
				//    If i = 0 Then
				//      boolFirstPass = True
				//    Else
				//      boolFirstPass = False
				//    End If
				//
				//    Call modAdjacentAreas_Subtract(objBldgOX2, boolFirstPass)                        'get modified areas adjacent to BLDG OX limits
				//
				//  Next i
				Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("There are Zero BLDG LIMs - there needs to be one BLDG LIM for each Building.  Exiting...");
				return;
				//BLDG LIM exists
			}else {
				color = Misc.getColorByBlock(6);
				ObjectId[] ids = objSSetBLDG_LIM.GetObjectIds();

				for (int i = 0; i < ids.Length; i++) {
					string strLayer = EW_Build3dPoly.getBldgLayer(ids[i]);
					//FOR EACH BLDG LIM GET CORRESPONDING FLOOR SLAB
					//as is will return last bldg area inside BLDG LIM if more than one FLOOR SLAB

					ObjectId idBldgOX2 = EW_GetBldgOX2.getBldgOX2(ids[i], 1, "MADE", strLayer);//get BLDG OX limits
					
					if ((idBldgOX2 == ObjectId.Null))
						return;

					idBldgOX2.changeProp(LineWeight.LineWeight100, color);

					if (i == 0) {
						boolFirstPass = true; //FOR MULTIPLE BUILDINGS                        
					}else {
						boolFirstPass = false;
					}
					EW_ModAdjacentAreas.modAdjacentAreas_Subtract(idBldgOX2, boolFirstPass); //build modified areas adjacent to BLDG OX limits                    
				}
				//  If objSSetBLDG_LIM.Count <> objSSetFLOOR_SLAB.Count Then
				//    MsgBox "Number of Building Limit not equal to Number of FLOOR SLAB"
				//  End If
			}

			SelectionSet ss = EW_Utility1.buildSSet22();  //OX-LIM-OUTER
			ObjectId[] idsOX_LIM_OUTER = ss.GetObjectIds();

			for (int i = 0; i < idsOX_LIM_OUTER.Length; i++) {
				ObjectId idBldgOX2 = idsOX_LIM_OUTER[i];

				Point3dCollection varPnts3d = idBldgOX2.getCoordinates3d();

				ss = EW_Utility1.buildSSet21(varPnts3d);    //get OX-AREAS-2d - inside OX limit for removal
				ObjectId[] idsAreas = ss.GetObjectIds();
				for (int j = 0; j < idsAreas.Length; j++) {
					idsAreas[j].changeProp(LineWeight.LineWeight050, clr.red);
					idsAreas[j].delete(); //delete areas inside OX-LIM-OUTER                  
				}
			}

			ss = EW_Utility1.buildSSet19(); //get OX-AREAS-2d - select all
			ObjectId[] idsOX_AREAS_2d = ss.GetObjectIds();

			for (int i = 0; i < idsOX_AREAS_2d.Length; i++) {
				ObjectId idLWPlineT = idsOX_AREAS_2d[i];
				idLWPlineT.changeProp(LineWeight.LineWeight000, clr.wht);

				ResultBuffer rb = idLWPlineT.getXData("OX-Layer");
				if (rb == null)
					continue;

				string strLayer = rb.AsArray()[1].Value.ToString();

				color = Misc.getColorByBlock(140);
				ObjectId idLWPlineX = idLWPlineT.offset(-0.05);
				if (idLWPlineX == ObjectId.Null) {
					idLWPlineT.changeProp(LineWeight.LineWeight200, color);
					continue;                
				}else {
					idLWPlineX.changeProp(LineWeight.LineWeight100, clr.mag);
					idLWPlineT.delete();

					if (!offsetSegments("SG", "OX", idLWPlineX, strLayer)) {
						return;
					}
				}
			}

			viewResults("OX", true);

			if (!pub.boolDebug) {
				ss = EW_Utility1.buildSSetOX2d();
				ss.eraseSelectedItems();
			}

			ss = EW_Utility1.buildSSetLinesLayer0();
			ss.eraseSelectedItems();
		}

		public static void makeSurfaceOXg() {
			Layer.manageLayers("OXg-BRKLINE");
			Layer.manageLayers("OXg-BRKLINE-AREA");
			Layer.manageLayers("OXg-SURFACE");

			SelectionSet ss = EW_Utility1.buildSSetOXg();
			ObjectId[] ids = ss.GetObjectIds();

			for (int i = 0; i < ids.Length; i++) {
				ObjectId idOXg = ids[i];
				string strLayer = idOXg.getLayer();

				ObjectId idLWPlineX = idOXg.offset(-0.05);

				if (idLWPlineX == ObjectId.Null) { 
					idOXg.changeProp(LineWeight.LineWeight200, Misc.getColorByBlock(206));
				}else {
					idLWPlineX.changeProp(LineWeight.LineWeight100, clr.mag);

					if (!offsetSegments("SG", "OXg", idLWPlineX, strLayer)) {
						return;
					}
				}
			}

			if (ss.Count > 0) {
				viewResults("OXg", true);
			}
		}

		public static bool offsetSegments(string strSurfaceName, string strSurfaceNameX, ObjectId idLWPlineX, string varLayerName = "") {
			double dblZOff = 0;
		   
			int intCase1 = 0, intCase2_1 = 0, intCase2_2 = 0, intCase2_3 = 0, intCase2_4 = 0, intCase2_5 = 0, intCase2_6 = 0, intCase2_7 = 0;
			int intCase2_8 = 0, intCase5_1 = 0, intCase5_2 = 0, intCase5_3 = 0, intCase5_4 = 0, intCase5_5 = 0, intCase8_5 = 0, intCase11 = 0;

			TinSurface objSurfaceSG = null;

			if (strSurfaceName == "SG") {
				objSurfaceSG = Surf.getTinSurface("SG", out exists);

				if ((objSurfaceSG == null)) {
					Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Surface SG missing - exiting routine");
					return false;
				}
			}

			ObjectId id3dBrkP = ObjectId.Null, idTextB = ObjectId.Null, idTextE = ObjectId.Null;
			string strLayerName = varLayerName;
			ObjectId id3dBndry = EW_Build3dPoly.buildArea3dLimit(idLWPlineX, strSurfaceName, strSurfaceNameX, strLayerName);//returns 3D poly with surface elevations, endPnt = begPnt
			
			if (id3dBndry == ObjectId.Null) {
				return false;
			}

			string strHandle = idLWPlineX.getHandle().ToString();
			string strLayer = idLWPlineX.getLayer();

			if (varLayerName == "") {
				dblZOff = EW_Utility1.getDepth(strLayer, strSurfaceNameX);  //get depth for area               
			}else {
				if (!varLayerName.Contains("_OX-AREA-")) {
					dblZOff = EW_Utility1.getDepth(varLayerName, strSurfaceNameX);
				}else {
					dblZOff = double.Parse(varLayerName.Substring(10));
				}
			}

			Point3d pnt3d0 = new Point3d(0, 0, 0);
			Point3d pnt3dX = new Point3d(0, 0, -dblZOff);
			id3dBndry.moveObj(pnt3d0, pnt3dX);

			if (idLWPlineX == ObjectId.Null)
			{
				MessageBox.Show("idLWPlineX is null");
			}

			List<Point3d> varPnts3dBndry0z = idLWPlineX.getCoordinates3dList();
			idLWPlineX.delete();

			List<Point3d> varPnts3dBndry0zForInt = varPnts3dBndry0z;
			varPnts3dBndry0zForInt.RemoveAt(varPnts3dBndry0z.Count - 1);    //remove endPnt ==>> endPnt <> begPnt

			if(strSurfaceName == "CPNT-ON"){
				strLayerName = "CPNT-BRKLINE";
			}else{
				strLayerName = string.Format("{0}-BRKLINE", strSurfaceName);
			}

			ObjectIdCollection ids = null;
			if (pub.boolDebug) {
				escape = Select.getSelSetFromUser(out ids);
			}else {
				ss = EW_Utility1.buildSSetCPNT_ON_SEGMENTS(strLayerName, varPnts3dBndry0z);  //select triangle segments (3dPolylines) by polygon crossing
			}

			if (ss == null)
				return false;

			ObjectId[] idsSegments = ss.GetObjectIds();

			TypedValue[] tvs = new TypedValue[3] {
				new TypedValue(1001, "makeBOT"),
				new TypedValue(1000, varLayerName),
				new TypedValue(1000, "BRKLINES")
			};

			for (int j = 0; j < idsSegments.Length; j++) {

				strLayer = string.Format("{0}-BRKLINE", strSurfaceNameX);
				ObjectId id = idsSegments[j];
				ObjectId idBrk = ObjectId.Null;
				Entity ent = id.getEnt();

				if (ent.GetType() == typeof(PolylineVertex3d))
					idBrk = id.getOwnerOfVertex2d3d();
				else if (ent.GetType() == typeof(Polyline3d))
					idBrk = id;

				//ent = idBrk.getEnt();
				//MessageBox.Show(ent.GetType().ToString());
								  
				ObjectId id3dBrkX = idBrk.copy(strLayer);

				//ent = id3dBrkX.getEnt();
				//MessageBox.Show(ent.GetType().ToString());


				id3dBrkX.moveObj(pnt3d0, pnt3dX);  //move copy of triangle edge based on vertical offset               
				id3dBrkX.changeProp(clr.byl, strLayer);

				//ent = id3dBrkX.getEnt();
				//MessageBox.Show(ent.GetType().ToString());

				List<Point3d> varPnts3d = id3dBrkX.getCoordinates3dList();

				Point3d dblPntBeg = varPnts3d[0];
				Point3d dblPntEnd = varPnts3d[1];   //end point of triangle edge (3dPoly segment)
				if (pub.boolDebug) {
					idTextB = Txt.addText("B", dblPntBeg, 0, AttachmentPoint.BottomLeft);
					idTextB.changeProp(color, "DEBUG-0", LineWeight.LineWeight020);
				}

				if (pub.boolDebug) {
					idTextE = Txt.addText("E", dblPntBeg, 0, AttachmentPoint.BottomLeft);
					idTextE.changeProp(color, "DEBUG-0", LineWeight.LineWeight020);
				}

				//------------------------------------------------------------------test for intersection
				List<Point3d> dblPntInts = Geom.getPntInts(dblPntBeg, dblPntEnd, varPnts3dBndry0zForInt, false, extend.none);
				//------------------------------------------------------------------test for intersection
				bool boolAddEnt = false;
				bool boolBegOnSeg = false;
				bool boolEndOnSeg = false;
				bool boolBegIn = false;
				bool boolEndIn = false;

				int intUBnd = 0;
				switch ((int)(dblPntInts[0].X)) {
					case -1:

						intUBnd = -1;

						boolBegOnSeg = (dblPntBeg.isOn2dSegment(varPnts3dBndry0zForInt) == - 1) ? false : true;
						boolEndOnSeg = (dblPntEnd.isOn2dSegment(varPnts3dBndry0zForInt) == - 1) ? false : true;

						break;
					case -9:

						intUBnd = -9;
						boolBegOnSeg = true;
						boolEndOnSeg = true;

						break;
					default:

						intUBnd = dblPntInts.Count;

						boolBegOnSeg = (dblPntBeg.isOn2dSegment(varPnts3dBndry0zForInt) == - 1) ? false : true;
						boolEndOnSeg = (dblPntEnd.isOn2dSegment(varPnts3dBndry0zForInt) == - 1) ? false : true;

						break;
				}

				if (boolBegOnSeg == false) {
					boolBegIn = dblPntBeg.isInside(varPnts3dBndry0zForInt);
				}else {
					boolBegIn = true;
				}

				if (boolEndOnSeg == false) {
					boolEndIn = dblPntEnd.isInside(varPnts3dBndry0zForInt);
				}else {
					boolEndIn = true;
				}

				switch (intUBnd) {
					case -1:
						//Case -1

						if (boolBegOnSeg & !boolEndIn) {
							boolAddEnt = false;
						}else if (boolEndOnSeg & !boolBegIn) {
							boolAddEnt = false;
						}else {
							boolAddEnt = true;
							intCase1 = intCase1 + 1;
						}

						break;
					case 1: //one intersection - one point inside and one outside

						//Case 2-1
						if (boolBegOnSeg == true & boolEndOnSeg == true) {
							//both ends on the line
							//test midPoint to see if segment is inside or outside
							Point3d pnt3dMid = dblPntBeg.getMidPoint3d(dblPntEnd);

							//boolAddEnt = (pnt3dMid.isOn2dSegment(varPnts3dBndry0zForInt) == -1) ? true : false;
							boolAddEnt = pnt3dMid.isInside(varPnts3dBndry0zForInt);

							if (boolAddEnt)
								intCase5_1 = intCase2_1 + 1;
							//Case 2-2
						}else if (boolBegOnSeg == true & boolEndIn == true) {
							boolAddEnt = true;
							intCase2_2 = intCase2_2 + 1;
							//Case 2-3 ???????????????????????????????????
						}else if (boolBegOnSeg == true & boolEndIn == false) {
							if (dblPntInts[0].isOn2dSegment(varPnts3dBndry0zForInt) != -1) {
								dblPntEnd = dblPntInts[0];
								double elev = EW_Utility2.getElev(dblPntEnd, id3dBndry);
								if (elev == 0 && strSurfaceName == "SG") {
									elev = objSurfaceSG.FindElevationAtXY(dblPntEnd.X, dblPntEnd.Y);
								}
								dblPntEnd = dblPntEnd.addElevation(elev);

								boolAddEnt = true;
							}else {
								boolAddEnt = false;
								intCase2_3 = intCase2_3 + 1;
							}
							//Case 2-4
						}else if (boolEndOnSeg == true & boolBegIn == true) {
							boolAddEnt = true;
							intCase2_4 = intCase2_4 + 1;
							//Case 2-5
						}else if (boolEndOnSeg == true & boolBegIn == false) {
							if (dblPntInts[0].isOn2dSegment(varPnts3dBndry0zForInt) != -1) {
								dblPntBeg = dblPntInts[0];
								double elev = EW_Utility2.getElev(dblPntBeg, id3dBndry);
								if (elev == 0 && strSurfaceName == "SG") {
									elev = objSurfaceSG.FindElevationAtXY(dblPntBeg.X, dblPntBeg.Y);
								}
								dblPntBeg = dblPntBeg.addElevation(elev);
								boolAddEnt = true;
							}else {
								boolAddEnt = false;
								intCase2_5 = intCase2_5 + 1;
							}
							//Case 2-6
						}else if (boolBegIn == true & boolEndIn == false) {
							dblPntEnd = dblPntInts[0];
							double elev = EW_Utility2.getElev(dblPntEnd, id3dBndry);
							if (elev == 0 && strSurfaceName == "SG") {
								elev = objSurfaceSG.FindElevationAtXY(dblPntEnd.X, dblPntEnd.Y);
							}
							dblPntEnd = dblPntEnd.addElevation(elev);

							boolAddEnt = true;
							intCase2_6 = intCase2_6 + 1;
							//Case 2-7
						}else if (boolBegIn == false & boolEndIn == true) {
							dblPntBeg = dblPntInts[0];
							double elev = EW_Utility2.getElev(dblPntBeg, id3dBndry);
							if (elev == 0 && strSurfaceName == "SG") {
								elev = objSurfaceSG.FindElevationAtXY(dblPntBeg.X, dblPntBeg.Y);
							}
							dblPntBeg = dblPntBeg.addElevation(elev);

							boolAddEnt = true;
							intCase2_7 = intCase2_7 + 1;
							//Case2-8
						}else if (boolBegIn == true & boolEndIn == true) {
							boolAddEnt = true;
							intCase2_8 = intCase2_8 + 1;
						}

						break;
					case 2:

						//Case 5-1
						if (boolBegOnSeg == true & boolEndOnSeg == true) {
							//both ends on the line
							//test miPoint3d to see if segment is inside or outside
							Point3d pnt3dMid = dblPntBeg.getMidPoint3d(dblPntEnd);
							boolAddEnt = pnt3dMid.isInside(varPnts3dBndry0zForInt);

							if (boolAddEnt)
								intCase5_1 = intCase5_1 + 1;
							//Case 5-2
						}else if (boolBegIn == false & boolEndIn == false) {
							dblPntBeg = dblPntInts[0];
							double elev = EW_Utility2.getElev(dblPntBeg, id3dBndry);

							if (elev == 0 && strSurfaceName == "SG") {
								elev = objSurfaceSG.FindElevationAtXY(dblPntBeg.X, dblPntBeg.Y);
							}
							dblPntBeg = dblPntBeg.addElevation(elev);

							dblPntEnd = dblPntInts[1];
							elev = EW_Utility2.getElev(dblPntEnd, id3dBndry);
							if (elev == 0 && strSurfaceName == "SG") {
								elev = objSurfaceSG.FindElevationAtXY(dblPntEnd.X, dblPntEnd.Y);
							}
							dblPntEnd = dblPntEnd.addElevation(elev);

							boolAddEnt = true;
							intCase5_2 = intCase5_2 + 1;
							//Case 5-3
						}else if (boolBegOnSeg == true & boolEndIn == false) {
							dblPntBeg = dblPntInts[0];
							double elev = EW_Utility2.getElev(dblPntBeg, id3dBndry);

							if (elev == 0 && strSurfaceName == "SG") {
								elev = objSurfaceSG.FindElevationAtXY(dblPntBeg.X, dblPntBeg.Y);
							}
							dblPntBeg = dblPntBeg.addElevation(elev);

							dblPntEnd = dblPntInts[1];
							elev = EW_Utility2.getElev(dblPntEnd, id3dBndry);
							if (elev == 0 && strSurfaceName == "SG") {
								elev = objSurfaceSG.FindElevationAtXY(dblPntEnd.X, dblPntEnd.Y);
							}
							dblPntEnd = dblPntEnd.addElevation(elev);

							boolAddEnt = true;
							intCase5_3 = intCase5_3 + 1;
							//Case 5-4
						}else if (boolEndOnSeg == true & boolBegIn == false) {
							dblPntBeg = dblPntInts[0];
							double elev = EW_Utility2.getElev(dblPntBeg, id3dBndry);

							if (elev == 0 && strSurfaceName == "SG") {
								elev = objSurfaceSG.FindElevationAtXY(dblPntBeg.X, dblPntBeg.Y);
							}
							dblPntBeg = dblPntBeg.addElevation(elev);

							dblPntEnd = dblPntInts[1];
							elev = EW_Utility2.getElev(dblPntEnd, id3dBndry);
							if (elev == 0 && strSurfaceName == "SG") {
								elev = objSurfaceSG.FindElevationAtXY(dblPntEnd.X, dblPntEnd.Y);
							}
							dblPntEnd = dblPntEnd.addElevation(elev);
							boolAddEnt = true;
							intCase5_4 = intCase5_4 + 1;
							//Case 5-5
						}else if (boolBegIn == true & boolEndIn == true) {
							boolAddEnt = true;
							intCase5_5 = intCase5_5 + 1;
						}

						break;
					case 3:
						//Case 8
						List<Point3d> dblPntInts_New = new List<Point3d>();
						for (int s = dblPntInts.Count; s > 0; s--) {
							if (System.Math.Round(dblPntBeg.X, 3) == System.Math.Round(dblPntInts[s - 1].X, 3) &&
								System.Math.Round(dblPntBeg.Y, 3) == System.Math.Round(dblPntInts[s - 1].Y, 3)) {
								dblPntInts.RemoveAt(s - 1);
							}
						}

						for (int s = dblPntInts.Count; s > 0; s--) {
							if (System.Math.Round(dblPntEnd.X, 3) == System.Math.Round(dblPntInts[s - 1].X, 3) &&
								System.Math.Round(dblPntEnd.Y, 3) == System.Math.Round(dblPntInts[s - 1].Y, 3)) {
								dblPntInts.RemoveAt(s - 1);
							}
						}

						switch (dblPntInts.Count) {
							case 2:

								double dblDist1 = dblPntBeg.getDistance(dblPntInts[0]);
								double dblDist2 = dblPntBeg.getDistance(dblPntInts[1]);

								if (dblDist1 > dblDist2) {
									Point3d pnt3dTmp = dblPntInts[0];
									dblPntInts[0] = dblPntInts[1];
									dblPntInts[1] = pnt3dTmp;
								}

								Point3d dblPntMid = dblPntBeg + (dblPntInts[0] - dblPntBeg) / 2;

								if (dblPntMid.isInside(varPnts3dBndry0zForInt)) {
									Point3d dblPntInt = dblPntInts[0];
									List<Point3d> dblPnts3dBrk = new List<Point3d>();
									dblPnts3dBrk.Add(dblPntBeg);
									double elev = EW_Utility2.getElev(dblPntInt, id3dBndry);

									if (elev == 0.0 && strSurfaceName == "SG") {
										elev = objSurfaceSG.FindElevationAtXY(dblPntInt.X, dblPntInt.Y);
									}
									dblPntInt.addElevation(elev);
									dblPnts3dBrk.Add(dblPntInt);

									ObjectId id3dBrkXNew = Draw.addPoly3d(dblPnts3dBrk, strSurfaceNameX + "-BRKLINE");
									id3dBrkXNew.changeProp(LineWeight.ByLayer, Color.FromColorIndex(ColorMethod.ByAci, 0));
									id3dBrkXNew.setXData(tvs, "makeBOT");

									intCase5_1 = intCase5_1 + 1;

									id3dBrkX.delete();
								}

								dblPntMid = dblPntInts[1] + (dblPntEnd - dblPntInts[1]) / 2;
								if (dblPntMid.isInside(varPnts3dBndry0zForInt)) {
									Point3d dblPntInt = dblPntInts[1];
									List<Point3d> dblPnts3dBrk = new List<Point3d>();
									double elev = EW_Utility2.getElev(dblPntInt, id3dBndry);

									if (elev == 0.0 && strSurfaceName == "SG") {
										elev = objSurfaceSG.FindElevationAtXY(dblPntInt.X, dblPntInt.Y);
									}
									dblPntInt.addElevation(elev);
									dblPnts3dBrk.Add(dblPntInt);
									dblPnts3dBrk.Add(dblPntEnd);

									ObjectId id3dBrkXNew = Draw.addPoly3d(dblPnts3dBrk, strSurfaceNameX + "-BRKLINE");
									id3dBrkXNew.changeProp(LineWeight.ByLayer, Color.FromColorIndex(ColorMethod.ByAci, 0));
									id3dBrkXNew.setXData(tvs, "makeBOT");

									intCase8_5 = intCase8_5 + 1;

									id3dBrkX.delete();
								}

								break;
							case 3:

								break;
						}

						break;
					case 4:
						//Case 11

						intCase11 = intCase11 + 1;

						break;
					case -9:

						boolAddEnt = false;

						break;
				}

				if (boolAddEnt == true) {
					id3dBrkX.delete();
					id3dBrkP = Draw.addPoly3d(dblPntBeg, dblPntEnd, strLayer);
					id3dBrkP.setXData(tvs, "makeBOT");

					if (pub.boolDebug) {
						id3dBrkX.changeProp(LineWeight.LineWeight200, clr.byl);
						id3dBrkP = id3dBrkX;
					}
				}else {
					//      obj3dBrkX.Delete
				}

				if (pub.boolDebug) {
					idTextB.delete();
					idTextE.delete();

					ss = EW_Utility1.buildSSet7();
					ss.eraseSelectedItems();
				}
			}

			//Debug.Print "Case -1 " & intCase1
			//Debug.Print "Case 2-1 " & intCase2_1
			//Debug.Print "Case 2-2 " & intCase2_2
			//Debug.Print "Case 2-3 " & intCase2_3
			//Debug.Print "Case 2-4 " & intCase2_4
			//Debug.Print "Case 2-5 " & intCase2_5
			//Debug.Print "Case 2-6 " & intCase2_6
			//Debug.Print "Case 2-7 " & intCase2_7
			//Debug.Print "Case 2-8 " & intCase2_8
			//Debug.Print "Case 5-1 " & intCase5_1
			//Debug.Print "Case 5-2 " & intCase5_2
			//Debug.Print "Case 5-3 " & intCase5_3
			//Debug.Print "Case 5-4 " & intCase5_4
			//Debug.Print "Case 5-5 " & intCase5_5
			//Debug.Print "Case 8-5 " & intCase8_5
			//Debug.Print "Case 8-8 " & intCase8_8
			//Debug.Print "Case 11 " & intCase11
			//Debug.Print "Total=" & intCase1 + intCase2_1 + intCase2_2 + intCase2_3 + intCase2_4 + intCase2_5 + intCase2_6 + _
			//'            intCase2_7 + intCase2_8 + intCase5_1 + intCase5_2 + intCase5_3 + intCase5_4 + intCase5_5 + _
			//'            intCase8_5 + intCase8_8 + intCase11

			return true;
		}
	}
}