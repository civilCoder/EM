using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using Base_Tools45;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Styles = Autodesk.Civil.DatabaseServices.Styles;
using Table = Autodesk.AutoCAD.DatabaseServices.Table;

namespace EW {
	public static class EW_Utility1 {
		public static SelectionSet 
		buildSSet0() {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, "_XX*");

			return Select.buildSSetBase(TVs);            
		}

		public static SelectionSet
		buildSSet_XX_OX() {
			TypedValue[] TVs = new TypedValue[5];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.Operator, "<OR");
			TVs[2] = new TypedValue((int)DxfCode.LayerName, "_XX");
			TVs[3] = new TypedValue((int)DxfCode.LayerName, "_OX");
			TVs[4] = new TypedValue((int)DxfCode.Operator, "OR>");

			return Select.buildSSetBase(TVs);
		}

		public static SelectionSet
		buildSSet1() {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, "_XX");

			return Select.buildSSetBase(TVs, false);
		}

		public static SelectionSet
		buildSSetCPNT_ON_SEGMENTS(string strName, List<Point3d> pnts3d) {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline3d)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, strName);

			Point3dCollection pnts3dC = new Point3dCollection();
			foreach (Point3d p in pnts3d)
				pnts3dC.Add(p);
			return Select.buildSSet(TVs, pnts3dC);
		}

		public static SelectionSet
		buildSSet2a(string strSurfaceName) {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline3d)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, strSurfaceName + "-BRKLINE");

			return Select.buildSSetBase(TVs, true);
		}

		public static SelectionSet
		buildSSet2b(string strSurfaceName) {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline3d)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, strSurfaceName + "-BRKLINE");

			return Select.buildSSetBase(TVs, false);
		}

		public static SelectionSet
		buildSSetTable() {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Table)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, "ZZ_ZZ-TABLE");

			return Select.buildSSetBase(TVs);
		}

		public static SelectionSet
		buildSSetGradingLim() {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, "GRADING LIMIT");

			return Select.buildSSetBase(TVs);
		}

		public static SelectionSet
		buildSSet7() {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Circle)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, "0");

			return Select.buildSSetBase(TVs);
		}

		public static SelectionSet
		buildSSet8() {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Ole2Frame)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, "ZZ_ZZ-SPREADSHEET");

			return Select.buildSSetBase(TVs);
		}

		public static SelectionSet
		buildSSet9()
		{
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, "_YY-BLDG LIM");

			return Select.buildSSetBase(TVs, true);
		}

		public static SelectionSet
		buildSSet11(Point3dCollection pnts3d = null) {
			TypedValue[] TVs = new TypedValue[1];

			if (pnts3d == null) {
				TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(CogoPoint)).DxfName);
				return Select.buildSSet(TVs);
			}else {
				TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(CogoPoint)).DxfName);
				return Select.buildSSet(TVs, pnts3d);
			}
		}

		public static SelectionSet
		buildSSet12() {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline3d)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, "OX-BRKLINE-AREA");

			return Select.buildSSetBase(TVs);
		}

		public static SelectionSet buildSSet13() {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline3d)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, "BOT-BRKLINE*");

			return Select.buildSSetBase(TVs);
		}

		public static SelectionSet
		buildSSet17(Point3dCollection pnts3d) {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, "_YY-K BRACE");

			return Select.buildSSet(TVs, pnts3d);
		}

		public static SelectionSet
		buildSSetFLOOR_SLAB() {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, "_XX-FLOOR SLAB*");

			return Select.buildSSet(TVs);
		}

		public static SelectionSet buildSSet19() {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, "OX-AREAS-2d");

			return Select.buildSSet(TVs);
		}

		public static SelectionSet
		buildSSet20() {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, "_XX-*");

			return Select.buildSSetBase(TVs, false);
		}

		public static SelectionSet
		buildSSet21(Point3dCollection pnts3d) {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, "OX-AREAS-2d");

			return Select.buildSSet(TVs, pnts3d);
		}

		public static SelectionSet
		buildSSet22() {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, "OX-LIM-OUTER");

			return Select.buildSSet(TVs);
		}

		public static SelectionSet
		buildSSetBLDG_LIM() {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, "_YY-BLDG LIM");

			return Select.buildSSet(TVs);
		}

		public static SelectionSet
		buildSSetLinesLayer0() {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Line)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, "0");

			return Select.buildSSet(TVs);
		}

		public static SelectionSet
		buildSSetOX2d() {
			SelectionSet objSSet1 = default(SelectionSet);
			SelectionSet objSSet2 = default(SelectionSet);
			SelectionSet objSSetX = default(SelectionSet);

			TypedValue[] TVs = new TypedValue[1];

			TVs[0] = new TypedValue((int)DxfCode.LayerName, "OX-AREAS-*");
			objSSet1 = Select.buildSSet(TVs);

			ObjectIdCollection ids = default(ObjectIdCollection);

			if (objSSet1.Count > 0) {
				ids = new ObjectIdCollection(objSSet1.GetObjectIds());
			}

			TVs[0] = new TypedValue((int)DxfCode.LayerName, "OX-LIM-OUTER");
			objSSet2 = Select.buildSSet(TVs);

			if (objSSet2.Count > 0) {
				foreach (ObjectId id in objSSet2.GetObjectIds()) {
					ids.Add(id);
				}
			}

			ObjectId[] idss = new ObjectId[ids.Count];

			for (int i = 0; i <= ids.Count - 1; i++) {
				idss[i] = ids[i];
			}

			objSSetX = SelectionSet.FromObjectIds(idss);

			return objSSetX;
		}

		public static void
		deleteByLayer(string nameLayer) {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.LayerName, "DEBUG-*");
			TVs[1] = new TypedValue((int)DxfCode.LayerName, nameLayer);

			Misc.deleteObjs(TVs);
		}

		public static void
		deleteCIRCLESonLayer0() {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Circle)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, "0");

			Misc.deleteObjs(TVs);
		}

		public static SelectionSet
		buildSSetOXg() {
			TypedValue[] TVs = new TypedValue[2];
			TVs[0] = new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline)).DxfName);
			TVs[1] = new TypedValue((int)DxfCode.LayerName, "_OXg-*");

			return Select.buildSSet(TVs);
		}

		public static bool
		pntsRsame(Point3d varPntX, Point3d varPnt0) {
			bool functionReturnValue = false;

			if (System.Math.Round(varPntX.X, 3) == System.Math.Round(varPnt0.X, 3) & System.Math.Round(varPntX.Y, 3) == System.Math.Round(varPnt0.Y, 3)) {
				functionReturnValue = true;
			}
			return functionReturnValue;
		}

		public static int 
		getSegNo(Point3d varPnt, List<Point3d> var2dCoords) {
			int numSeg = 0;

			for (int s = 1; s < var2dCoords.Count; s++) {
				Point3d pnt3dBeg = var2dCoords[s - 1];
				Point3d pnt3dEnd = var2dCoords[s - 0];

				double dblLen1 = pnt3dBeg.getDistance(pnt3dEnd);
				double dblLen2 = varPnt.getDistance(pnt3dBeg);

				Vector3d v3d1 = pnt3dEnd - pnt3dBeg;
				Vector3d v3d2 = varPnt - pnt3dBeg;

				Vector2d v2d1 = v3d1.Convert2d(BaseObjs.xyPlane);
				Vector2d v2d2 = v3d2.Convert2d(BaseObjs.xyPlane);

				double dblResult = v2d1.DotProduct(v2d2);

				//test if point is on line with same direction as boundary segment

				if (System.Math.Round(dblResult, 3) == 0.0) { //test if point is on segment
					if (dblLen2 <= dblLen1) {
						double dblAng1 = pnt3dBeg.getDirection(pnt3dEnd);
						double dblAng2 = pnt3dBeg.getDirection(varPnt);

						if (System.Math.Round(dblAng2, 3) == System.Math.Round(dblAng1, 3)) {
							numSeg = s - 1;
						}
					}
				}
			}
			return numSeg;
		}

		public static void
		copyEWstyles() {
			Document acDocTar = BaseObjs._acadDoc;                                                  // target - Active Document
			Database dbTar = acDocTar.Database;                                                     // target database

			Document acDocSrc = BaseObjs.openDwg("R:\\TSet\\Template\\CIVIL3D2010\\EW.dwt");         // source - Active Document
			Application.DocumentManager.MdiActiveDocument = acDocSrc;
			CivilDocument civDocSrc = BaseObjs._civDoc;                                         		
			
			SectionStyleCollection objSectionStyles = civDocSrc.Styles.SectionStyles;               //source Section Styles			
			SectionViewStyleCollection objSectionViewStyles = civDocSrc.Styles.SectionViewStyles;   //soubce Section View Styles   
			GroupPlotStyleCollection objGroupPlotStyles = civDocSrc.Styles.GroupPlotStyles;         //source GroupPlotStyles

			using (Transaction tr = BaseObjs.startTransactionDb()) {
				foreach (ObjectId idStyle in objSectionStyles) {
					SectionStyle style = (SectionStyle)tr.GetObject(idStyle, OpenMode.ForRead);
					if (style.Name != "Standard") {
						style.ExportTo(dbTar, Autodesk.Civil.StyleConflictResolverType.Override);               //export from source to target db					
					}
				}
		
				foreach (ObjectId idStyle in objSectionViewStyles) {
					Styles.SectionViewStyle objSectionViewStyle = (Styles.SectionViewStyle)tr.GetObject(idStyle, OpenMode.ForRead);

					if (objSectionViewStyle.Name != "Standard") {
						objSectionViewStyle.ExportTo(dbTar, Autodesk.Civil.StyleConflictResolverType.Override); //export from source to target db
					}
				}

				foreach (ObjectId idStyle in objGroupPlotStyles) {
					GroupPlotStyle objGroupPlotStyle = (GroupPlotStyle)tr.GetObject(idStyle, OpenMode.ForRead);

					if (objGroupPlotStyle.Name != "Standard") {
						objGroupPlotStyle.ExportTo(dbTar, Autodesk.Civil.StyleConflictResolverType.Override);   //export from source to target db
					}
				}
				tr.Commit();
			}

			Application.DocumentManager.MdiActiveDocument = acDocTar;
			acDocSrc.CloseAndDiscard();
		}

		public static void
		copySurfaceStyle() {
			Document acDocTar = BaseObjs._acadDoc;                                                  // target - Active Document
			Database dbTar = acDocTar.Database;                                                     // target database

			Document acDocSrc = BaseObjs.openDwg("R:\\TSet\\Template\\CIVIL3D2010\\EW.dwt");         // source - Active Document
			Application.DocumentManager.MdiActiveDocument = acDocSrc;
			CivilDocument civDocSrc = BaseObjs._civDoc;                                         		

			SurfaceStyleCollection objSurfaceStyles = civDocSrc.Styles.SurfaceStyles;

			using (Transaction tr = BaseObjs.startTransactionDb()) {
				foreach (ObjectId idStyle in objSurfaceStyles) {
					SurfaceStyle objSurfaceStyle = (SurfaceStyle)tr.GetObject(idStyle, OpenMode.ForRead);

					if (objSurfaceStyle.Name != "Standard") {
						objSurfaceStyle.ExportTo(dbTar, Autodesk.Civil.StyleConflictResolverType.Override);
					}
				}

				tr.Commit();
			}
			Application.DocumentManager.MdiActiveDocument = acDocTar;
			acDocSrc.CloseAndDiscard();
		}

		public static void
		copySectionViewStyle() {
			Document acDocTar = BaseObjs._acadDoc;                                                  // target - Active Document
			Database dbTar = acDocTar.Database;                                                     // target database

			Document acDocSrc = BaseObjs.openDwg("R:\\TSet\\Template\\CIVIL3D2010\\EW.dwt");        // source - Active Document
			Application.DocumentManager.MdiActiveDocument = acDocSrc;
			CivilDocument civDocSrc = BaseObjs._civDoc;

			SectionViewStyleCollection objSectionViewStyles = civDocSrc.Styles.SectionViewStyles;   //soubce Section View Styles   

			using (Transaction tr = BaseObjs.startTransactionDb()) {
				foreach (ObjectId idStyle in objSectionViewStyles) {
					Styles.SectionViewStyle objSectionViewStyle = (Styles.SectionViewStyle)tr.GetObject(idStyle, OpenMode.ForRead);

					if (objSectionViewStyle.Name != "Standard") {
						objSectionViewStyle.ExportTo(dbTar, Autodesk.Civil.StyleConflictResolverType.Override); //export from source to target db
					}
				}
				tr.Commit();
			}

			Application.DocumentManager.MdiActiveDocument = acDocTar;
			acDocSrc.CloseAndDiscard();
		}

		public static void
		copyGroupPlotStyle() {
			Document acDocTar = BaseObjs._acadDoc;                                                  // target - Active Document
			Database dbTar = acDocTar.Database;                                                     // target database

			Document acDocSrc = BaseObjs.openDwg("R:\\TSet\\Template\\CIVIL3D2010\\EW.dwt");         // source - Active Document
			Application.DocumentManager.MdiActiveDocument = acDocSrc;
			CivilDocument civDocSrc = BaseObjs._civDoc;

			GroupPlotStyleCollection objGroupPlotStyles = civDocSrc.Styles.GroupPlotStyles;         //source GroupPlotStyles

			using (Transaction tr = BaseObjs.startTransactionDb()) {
				foreach (ObjectId idStyle in objGroupPlotStyles) {
					GroupPlotStyle objGroupPlotStyle = (GroupPlotStyle)tr.GetObject(idStyle, OpenMode.ForRead);

					if (objGroupPlotStyle.Name != "Standard") {
						objGroupPlotStyle.ExportTo(dbTar, Autodesk.Civil.StyleConflictResolverType.Override);   //export from source to target db
					}
				}
				tr.Commit();
			}

			Application.DocumentManager.MdiActiveDocument = acDocTar;
			acDocSrc.CloseAndDiscard();
		}

		public static void
		copySectionStyle() {
			Document acDocTar = BaseObjs._acadDoc;                                                  // target - Active Document
			Database dbTar = acDocTar.Database;                                                     // target database

			Document acDocSrc = BaseObjs.openDwg("R:\\TSet\\Template\\CIVIL3D2010\\EW.dwt");        // source - Active Document
			Application.DocumentManager.MdiActiveDocument = acDocSrc;
			CivilDocument civDocSrc = BaseObjs._civDoc;

			SectionStyleCollection objSectionStyles = civDocSrc.Styles.SectionStyles;               //source Section Styles			

			using (Transaction tr = BaseObjs.startTransactionDb()) {
				foreach (ObjectId idStyle in objSectionStyles) {
					SectionStyle style = (SectionStyle)tr.GetObject(idStyle, OpenMode.ForRead);
					if (style.Name != "Standard") {
						style.ExportTo(dbTar, Autodesk.Civil.StyleConflictResolverType.Override);               //export from source to target db					
					}
				}
				tr.Commit();
			}

			Application.DocumentManager.MdiActiveDocument = acDocTar;
			acDocSrc.CloseAndDiscard();
		}

		public static void
		getTableData() {

			SelectionSet ss = buildSSetTable();
			if (ss == null || ss.Count == 0)
				return;

			ObjectId idTable = ss.GetObjectIds()[0];
			
			List<DEPTH> depths = new List<DEPTH>();
			Table table = (Table)idTable.getEnt();
			
			try {
				for (int i = 3; i <= 13; i++) {
					DEPTH depth = new DEPTH();
					depth.KEYWORD = table.Cells[i, 3].Value.ToString();
					depth.SG = double.Parse(table.Cells[i, 0].Value.ToString());
					depth.OX = double.Parse(table.Cells[i, 1].Value.ToString());
					depths.Add(depth);

					//Debug.Print "Keyword: " & varDepths(i - 3).KEYWORD & " SG: " & varDepths(i - 3).SG & " OX: " & varDepths(i - 3).OX
				}

				EW_Pub.depths = depths;

				EW_Pub.K_BRACE_V = double.Parse(table.Cells[15, 1].Value.ToString());
				EW_Pub.COLUMN_FOOTINGS_V = double.Parse(table.Cells[16, 1].Value.ToString());
				EW_Pub.OUTER_FOOTINGS_V = double.Parse(table.Cells[17, 1].Value.ToString());
				EW_Pub.REMOVE_REPLACE_V = double.Parse(table.Cells[18, 1].Value.ToString());
				EW_Pub.REMOVE_REPLACE_BLDG_V = double.Parse(table.Cells[19, 1].Value.ToString());
				EW_Pub.OX_LIMIT_H = double.Parse(table.Cells[20, 1].Value.ToString());
				EW_Pub.FOOTING_LIMIT_IN_H = double.Parse(table.Cells[21, 1].Value.ToString());
				EW_Pub.K_BRACE_H = double.Parse(table.Cells[22, 1].Value.ToString());
			}
			catch (System.Exception) {
				Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Update Table - create new drawing using EW.dwt and replace table after transferring values.  Rename FLOOR SLAB to FLOOR SLAB A");
			}
			//Debug.Print "K-Brace Vertica: " & K_BRACE_V
			//Debug.Print "Column Footings Depth: " & COLUMN_FOOTINGS_V
			//Debug.Print "Outer Footings Depth: " & OUTER_FOOTINGS_V
			//Debug.Print "Remove Replace Depth: " & REMOVE_REPLACE_V
			//Debug.Print "Remove Replace Building Depth: " & REMOVE_REPLACE_BLDG_V
			//Debug.Print "OX Limit: " & OX_LIMIT_H
			//Debug.Print "Footing Limit Inside: " & FOOTING_LIMIT_IN_H
			//Debug.Print "K-Brace Horizontal: " & K_BRACE_H
		}

		public static double
		getDepth(string strLayName, string strSurfaceName) {
			string strDescription = null;
			double dblDepth = 0;
			int i = 0;

			List<DEPTH> depths = EW_Pub.depths;

			for (i = 0; i <= 10; i++) {
				strDescription = depths[i].KEYWORD;

				if (strLayName.Substring(4) == strDescription) {
					switch (strSurfaceName) {
						case "SG":

							dblDepth = depths[i].SG;
							break;
						case "OX":

							dblDepth = depths[i].OX;
							break;
					}
				}
			}

			return dblDepth;
		}

		public static double
		getRemoveAndReplace() {
			SelectionSet objSSet = buildSSetTable();
			ObjectId[] ids = objSSet.GetObjectIds();

			Table objTable = (Table)ids[0].getEnt();

			int row = 16;
			int col = 1;

			return double.Parse(objTable.Cells[row, col].Value.ToString());
		}

		public static void
		removeDuplicateVertex(ref Polyline objPline) {
			ObjectId idPline = objPline.ObjectId;
			List<Point3d> varPntsPline = idPline.getCoordinates3dList();

			int intUBnd = varPntsPline.Count;            

			List<Point3d> dblPntsPline = new List<Point3d> { varPntsPline[0] };   //first vertex of polyline

			for (int i = 1; i < intUBnd; i++) {
				if (varPntsPline[i - 1].X != varPntsPline[i].X && varPntsPline[i - 1].Y != varPntsPline[i].Y)
					dblPntsPline.Add(varPntsPline[i]);
			}

			idPline.updatePoly3dCoordinates(dblPntsPline);
		}

		public static Vector
		getClosetAdjacentSegment(Point3d pnt3dCen, ObjectId idPoly) {
			List<Vector> varVectors = new List<Vector>();

			List<Point3d> pnts3d = idPoly.getCoordinates3dList();

			for (int i = 0; i < pnts3d.Count; i++) {
				double dblDist = Geom.getCosineComponent(pnt3dCen, pnts3d[i + 0], pnts3d[i + 1]);

				if (dblDist >= 0 & dblDist <= pnts3d[i + 0].getDistance(pnts3d[i + 1])) {
					double dblAng = pnts3d[i + 0].getDirection(pnts3d[i + 1]);

					Point3d pnt3dX = pnts3d[i + 0].traverse(dblAng, dblDist);

					Vector v = new Vector();
					v.dir = pnt3dCen.getDirection(pnt3dX);
					v.dist = pnt3dCen.getDistance(pnt3dX);
					varVectors.Add(v);
				}
			}

			var sortDist = from v in varVectors
						   orderby v.dist ascending
						   select v;
			List<Vector> vecs = new List<Vector>();
			foreach (var v in sortDist) {
				vecs.Add(v);
			}

			if (vecs[0].dist < 50) {
				Application.ShowAlertDialog("Need to move reference point");
			}

			return vecs[0];
		}
	}
}