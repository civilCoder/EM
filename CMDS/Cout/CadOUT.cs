using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Interop;


using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

using Base_Tools45;
using Base_Tools45.C3D;

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;
using Surface = Autodesk.Civil.DatabaseServices.Surface;

namespace Cout
{
	public static class CadOUT
	{
		private static AcadApplication acadApp {
			get{
				return (AcadApplication)Autodesk.AutoCAD.ApplicationServices.Application.AcadApplication;
			}
		}

		public static void
		doCadOUT(string cmdName) {
			Object pickf = null;

			try
			{
				Document doc = BaseObjs._acadDoc;
				doc.save();
				pickf = Autodesk.AutoCAD.ApplicationServices.Core.Application.GetSystemVariable("PICKFIRST");

				Autodesk.AutoCAD.ApplicationServices.Core.Application.SetSystemVariable("PICKFIRST", 1);
				Autodesk.AutoCAD.ApplicationServices.Core.Application.SetSystemVariable("FILEDIA", 0);

				BaseObjs.sendCommand("plan w\r");

				DateTime dt = DateTime.Now;

				string path0 = BaseObjs.docFullName;
				string nameDwg = Path.GetFileName(path0);

				if (nameDwg.Substring(0, 1) == "$")
				{
					DialogResult dr = MessageBox.Show("Temporary file is active.  Proceed?", "CADOUT", MessageBoxButtons.YesNo);
					if (dr == DialogResult.No)
						return;
				}
				string nameDir = Path.GetDirectoryName(path0);

				int numJob = int.Parse(BaseObjs.jobNumber(nameDwg));

				string path1 = "", path2 = "", path3 = "";

				if (nameDir.Substring(0, 1) == "N")
				{
					if (numJob > 2599 && numJob < 2700)
					{
						path1 = string.Format("{0}\\EMAILOUT", path0.Substring(0, 17));
						path2 = string.Format("{0}\\EMAIL-OUT", path1.Substring(0, 17));
						path3 = string.Format("{0}\\EMAIL OUT", path1.Substring(0, 17));
					}
					else
					{
						path1 = string.Format("{0}\\EMAILOUT", path0.Substring(0, 7));
						path2 = string.Format("{0}\\EMAIL-OUT", path1.Substring(0, 7));
						path3 = string.Format("{0}\\EMAIL OUT", path1.Substring(0, 7));
					}
				}
				else if (path0.Substring(0, 1) == "O")
				{
					path1 = string.Format("{0}\\EMAILOUT", path0.Substring(0, 17));
					path2 = string.Format("{0}\\EMAIL-OUT", path1.Substring(0, 17));
					path3 = string.Format("{0}\\EMAIL OUT", path1.Substring(0, 17));
				}
				else
				{
					path1 = string.Format("{0}\\EMAILOUT", path0.Substring(0, 7));
					path2 = string.Format("{0}\\EMAIL-OUT", path1.Substring(0, 7));
					path3 = string.Format("{0}\\EMAIL OUT", path1.Substring(0, 7));
				}

				bool fldr1 = Directory.Exists(path1);
				bool fldr2 = Directory.Exists(path2);
				bool fldr3 = Directory.Exists(path3);

				string nameTmp = "";
				string nameOut = "";
				string date = string.Format("{0}-{1:00}-{2:00}", dt.Year, dt.Month, dt.Day);

				if (fldr1)
				{
					if (!Directory.Exists(path1 + "\\" + date))
					{
						Directory.CreateDirectory(path1 + "\\" + date);
					}

					nameTmp = path1 + "\\" + date + "\\$" + nameDwg;
					nameOut = path1 + "\\" + date + "\\" + nameDwg.Substring(0, nameDwg.Length - 4) + "_out.dwg";
				}
				else if (fldr2)
				{
					if (!Directory.Exists(path2 + "\\" + date))
					{
						Directory.CreateDirectory(path2 + "\\" + date);
					}
					nameTmp = path2 + "\\" + date + "\\$" + nameDwg;
					nameOut = path2 + "\\" + date + "\\" + nameDwg.Substring(0, nameDwg.Length - 4) + "_out.dwg";
				}
				else if (fldr3)
				{
					if (!Directory.Exists(path3 + "\\" + date))
					{
						Directory.CreateDirectory(path3 + "\\" + date);
					}
					nameTmp = path3 + "\\" + date + "\\$" + nameDwg;
					nameOut = path3 + "\\" + date + "\\" + nameDwg.Substring(0, nameDwg.Length - 4) + "_out.dwg";
				}
				else
				{
					Directory.CreateDirectory(path1 + "\\" + date);
					nameTmp = path1 + "\\" + date + "\\$" + nameDwg;
					nameOut = path1 + "\\" + date + "\\" + nameDwg.Substring(0, nameDwg.Length - 4) + "_out.dwg";
				}

				bool fileExists = File.Exists(nameTmp);

				if (fileExists) //file exists - so delete and saveas
				{
					File.Delete(nameTmp);
					doc.saveas(nameTmp);
				}
				else //folder exists; file doesn't exist
				{
					doc.saveas(nameTmp);
				}

				//-------------------------------------------------------------------------------------------

				Dict.deleteDictionary("SearchDict");
				Dict.deleteDictionary("ObjectDict");

				if(cmdName == "cmdCOUT"){
					doCleanup1();                    
				}
				if(cmdName == "cmdCOUTX"){
					doCleanup2();                   
				}

				fileExists = File.Exists(nameOut);

				if (fileExists) //file exists - so delete and saveas
				{
					File.Delete(nameOut);
					doc.saveas(nameOut);
				}
				else //folder exists; file doesn't exist
				{
					doc.saveas(nameOut);
				}

				File.Delete(nameTmp);

				MessageBox.Show(string.Format("File created at: {0}", nameOut), "FILE CREATED", MessageBoxButtons.OK);
			}
			catch (System.Exception ex)	{
				BaseObjs.writeDebug(string.Format("{0} CadOUT.cs: line: 180", ex.Message));
			}
			finally {
				Autodesk.AutoCAD.ApplicationServices.Core.Application.SetSystemVariable("PICKFIRST", pickf);
				Autodesk.AutoCAD.ApplicationServices.Core.Application.SetSystemVariable("FILEDIA", 1);
			}
		}
 

		private static void
		doCleanup1(){

			//unload xrefs
			ObjectIdCollection xrefBTRs = xRef.getXRefBlockTableRecords();
			BaseObjs._db.UnloadXrefs(xrefBTRs);

			//unload CIVIL3D2015.dvb
			try{
				acadApp.UnloadDVB(@"C:\TSet\VBA2015\CIVIL3D2015.dvb");                
			}
			catch{}

			Extents3d ext3d = BaseObjs._getExtents;

			//change view to all
			VPort.zoomWindow(ext3d);

			TypedValue[] tvs = new TypedValue[4]{
					new TypedValue((int)DxfCode.Operator, "<OR"),
					new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(MText)).DxfName),
					new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Leader)).DxfName),
					new TypedValue((int)DxfCode.Operator, "OR>")
					};

			SelectionSet ss = Select.buildSSet(tvs);
			ObjectId[] ids = null;
			if (ss != null) { 
				ids = ss.GetObjectIds();
				for (int i = 0; i < ids.Length; i++)
				{
					ids[i].clearAllXdata();
				}                
			}

			//explode

			tvs = new TypedValue[17]{
				new TypedValue((int)DxfCode.Operator, "<NOT"),
				new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline3d)).DxfName),
				new TypedValue((int)DxfCode.Operator, "NOT>"),
				new TypedValue((int)DxfCode.Operator, "<NOT"),
				new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(CogoPoint)).DxfName),
				new TypedValue((int)DxfCode.Operator, "NOT>"),
				new TypedValue((int)DxfCode.Operator, "<NOT"),
				new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(MText)).DxfName),
				new TypedValue((int)DxfCode.Operator, "NOT>"),
				new TypedValue((int)DxfCode.Operator, "<NOT"),
				new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Leader)).DxfName),
				new TypedValue((int)DxfCode.Operator, "NOT>"),
				new TypedValue((int)DxfCode.Operator, "<NOT"),
				new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Wipeout)).DxfName),
				new TypedValue((int)DxfCode.Operator, "NOT>"),
				new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(BlockReference)).DxfName),
				new TypedValue((int)DxfCode.Operator, "NOT>")
			};

			ss = Select.buildSSet(tvs, ext3d.MinPoint, ext3d.MaxPoint);
			if (ss != null) { 
				ids = ss.GetObjectIds();
				ObjectIdCollection idsExp = ids.explode();                
			}

			//delete breaklines and points
			//DELETE BREAKLINES
			tvs = new TypedValue[2] { 
					new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline3d)).DxfName),
					new TypedValue((int)DxfCode.LayerName, "CPNT-BRKLINE")
				};

			ss = Select.buildSSet(tvs);
			if (ss != null){ 
				ids = ss.GetObjectIds();
				ids.deleteObjs();            
			}

			//DELETE COGO POINTS
			tvs = new TypedValue[1] { 
					new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(CogoPoint)).DxfName),
				};

			ss = Select.buildSSet(tvs);
			if (ss != null) { 
				ids = ss.GetObjectIds();
				ids.deleteObjs();            
			}

			//load CIVIL3D2015.dvb
			try
			{
				acadApp.LoadDVB(@"C:\TSet\VBA2015\CIVIL3D2015.dvb");
			}
			catch { }

		}

		private static void
		doCleanup2() {

			//unload xrefs
			ObjectIdCollection xrefBTRs = xRef.getXRefBlockTableRecords();
			BaseObjs._db.UnloadXrefs(xrefBTRs);

			//unload CIVIL3D2015.dvb
			try
			{
				acadApp.UnloadDVB(@"C:\TSet\VBA2015\CIVIL3D2015.dvb");
			}
			catch { }

			//clear XData on MText and Leaders
			TypedValue[] tvs = new TypedValue[4] {
				new TypedValue((int)DxfCode.Operator, "<OR"),
				new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(MText)).DxfName),
				new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Leader)).DxfName),
				new TypedValue((int)DxfCode.Operator, "OR>")
			};

			SelectionSet ss = Select.buildSSet(tvs);
			ObjectId[] ids = ss.GetObjectIds();
			for (int i = 0; i < ids.Length; i++) {
				ids[i].clearAllXdata();
			}

			//idList of MText and Leaders
			List<ObjectId> idsList = new List<ObjectId>();
			for (int i = 0; i < ids.Length; i++)
				idsList.Add(ids[i]);


			//get all objects
			PromptSelectionResult psr = BaseObjs._editor.SelectAll();
			SelectionSet ssAll = psr.Value;
			ObjectId[] idsAll = ssAll.GetObjectIds();

			//idList of all objects
			List<ObjectId> idsAllList = new List<ObjectId>();       //all objects
			for (int i = 0; i < idsAll.Length; i++)
				idsAllList.Add(idsAll[i]);

			//remove MText and Leaders
			foreach (ObjectId id in idsList)
				idsAllList.Remove(id);                              //remove MText and Leaders


			tvs = new TypedValue[2] {
				new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline3d)).DxfName),
				new TypedValue((int)DxfCode.LayerName, "CPNT-BRKLINE")
			};

			ss = Select.buildSSet(tvs);
			ids = ss.GetObjectIds();

			idsList = new List<ObjectId>();
			for (int i = 0; i < ids.Length; i++)
				idsList.Add(ids[i]);

			//remove Brklines
			foreach (ObjectId id in idsList)
				idsAllList.Remove(id);                              //remove breaklines

			tvs = new TypedValue[1] {
				new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(CogoPoint)).DxfName),
			};

			ss = Select.buildSSet(tvs);
			ids = ss.GetObjectIds();

			idsList = new List<ObjectId>();
			for (int i = 0; i < ids.Length; i++)
				idsList.Add(ids[i]);

			//remove CogoPoints
			foreach (ObjectId id in idsList)
				idsAllList.Remove(id);                              //remove CogoPoints


			//explode remaining entities in list
			foreach (ObjectId id in idsAllList)
				id.explode();                                       //explode

			//Layer.manageLayer("CPNT-BRKLINE", layerFrozen: true);
			//Layer.manageLayer("CPNT-ON", layerFrozen: true);             

			//ForEach<Surface>(extractContours);

			//ForEach<Label>(explodeEnt);
			//ForEach<LabelGroup>(explodeEnt);
			//ForEach<Surface>(explodeEnt);
			//ForEach<Surface>(deleteSurface);

			//ObjectIdCollection idsPV = Prof.getProfileViews();
			//foreach (ObjectId idPV in idsPV)
			//    idPV.explode();

			//ObjectIdCollection idsAlign = BaseObjs._civDoc.GetAlignmentIds();
			//foreach (ObjectId idAlign in idsAlign)
			//    idAlign.delete();

			//DELETE BREAKLINES
			tvs = new TypedValue[2] {
				new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline3d)).DxfName),
				new TypedValue((int)DxfCode.LayerName, "CPNT-BRKLINE")
			};

			ss = Select.buildSSet(tvs);                
			ids = ss.GetObjectIds();
			ids.deleteObjs();

			//DELETE COGO POINTS
			tvs = new TypedValue[1] {
				new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(CogoPoint)).DxfName),
			};

			ss = Select.buildSSet(tvs);
			ids = ss.GetObjectIds();
			ids.deleteObjs();

			//load CIVIL3D2015.dvb
			try
			{
				acadApp.LoadDVB(@"C:\TSet\VBA2015\CIVIL3D2015.dvb");
			}
			catch { }

		}

		private static void deleteSurface(Surface surf)
		{
			Surf.removeSurface(surf.Name);
		}


		private static void extractContours(Surface surf) {
			
			if (surf is TinSurface){
				if (surf.IsReferenceObject){
					Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(string.Format("Surface {0} is from {1}", surf.Name, surf.OwnerId.ToString()));
					return;
				}
				TinSurface tinSurf = (TinSurface)surf;
				string nameSurf = tinSurf.Name;
				ObjectId idStyle = tinSurf.StyleId;
				SurfaceStyle style = Surf_Styles.getSurfaceStyle(idStyle);

				double majorInt = style.ContourStyle.MajorContourInterval;
				double minorInt = style.ContourStyle.MinorContourInterval;

				DisplayStyle planDispStyle = style.GetDisplayStylePlan(SurfaceDisplayStyleType.MajorContour);
				string majorLay = planDispStyle.Layer;

				planDispStyle = style.GetDisplayStylePlan(SurfaceDisplayStyleType.MinorContour);
				string minorLay = planDispStyle.Layer;

				ObjectIdCollection idsContours = tinSurf.ExtractContours(minorInt);

				Color color = new Color();
				color = Color.FromColorIndex(ColorMethod.ByLayer, 256);

				using (Transaction tr = BaseObjs.startTransactionDb()) {
					foreach (ObjectId id in idsContours) {
						Polyline poly = (Polyline)tr.GetObject(id, OpenMode.ForRead);
						double elev = System.Math.Round(poly.Elevation, 2);
						if ((elev / majorInt).mantissa() == 0) {
							id.changeProp(color, majorLay, LineWeight.LineWeight030);                            
						}else {
							id.changeProp(color, minorLay, LineWeight.LineWeight020);
						}
					}
					tr.Commit();
				}
			}                
		}

		private static void
		explodeEnt(Entity ent)
		{
			DBObjectCollection dbObjs = new DBObjectCollection();
			ent.ObjectId.explode();
		}

		private static void
		ForEach<T>(Action<T> action) where T : Entity
		{
			try
			{
				using (var tr = BaseObjs.startTransactionDb())
				{
					var blockTable = (BlockTable)tr.GetObject(BaseObjs._db.BlockTableId, OpenMode.ForRead);
					var modelSpace = (BlockTableRecord)tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);
					RXClass theClass = RXObject.GetClass(typeof(T));
					foreach (ObjectId id in modelSpace)
					{
						if (id.ObjectClass.IsDerivedFrom(theClass))
						{
							try
							{
								var ent = (T)tr.GetObject(id, OpenMode.ForRead);
								action(ent);
							}
							catch (System.Exception ex)
							{
								BaseObjs.writeDebug(string.Format("{0} CadOUT.cs: line: 49", ex.Message));
							}
						}
					}
					tr.Commit();
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(string.Format("{0} CadOUT.cs: line: 58", ex.Message));
			}
		}
	}
}

//tvs = new TypedValue[52]{
//    new TypedValue((int)DxfCode.Operator, "<OR"),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.AlignmentCantLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.AlignmentCurveLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.AlignmentLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.AlignmentStationEquationLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.AlignmentStationLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.AlignmentTangentLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.AlignmentVerticalGeometryPointLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.CatchmentLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.FeatureLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.FlowSegmentLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.GeneralSegmentLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.MatchLineLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.NoteLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.ParcelAreaLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.PartProfileLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.PartSectionLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.PipeLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.PipeNetworkBandLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.PipeProfileLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.PipeSectionLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.ProfileBandLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.ProfileCrestCurveLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.ProfileDataBandLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.ProfileLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.ProfileLineLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.ProfileMinorStationLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.ProfileProjectionLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.ProfilePVILabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.ProfileSagCurveLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.ProfileStationLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.SampleLineLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.SectionalDataBandLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.SectionBandLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.SectionDataBandLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.SectionGradeBreakLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.SectionLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.SectionMinorOffsetLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.SectionOffsetLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.SectionSegmentBandLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.SectionSegmentLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.SectionViewDepthLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.SectionViewOffsetElevationLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.StationElevationLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.StationOffsetLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.StructureLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.StructureProfileLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.StructureSectionLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.SurfaceContourLabelGroup)).DxfName),                    					
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.SurfaceElevationLabel)).DxfName),
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Autodesk.Civil.DatabaseServices.VerticalGeometryBandLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.Operator, "<OR"),

//};

//ss = Select.buildSSet(tvs);
//ids = ss.GetObjectIds();
//ObjectIdCollection idsExploded = ids.explode();

//tvs = new TypedValue[2]{
//    new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(SurfaceContourLabelGroup)).DxfName),
//    new TypedValue((int)DxfCode.LayerName, "CANT")
//};

//ss = Select.buildSSet(tvs);
//ids = ss.GetObjectIds();
//ObjectIdCollection idsExploded = ids.explode();
