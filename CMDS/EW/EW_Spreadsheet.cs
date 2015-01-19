using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using Base_Tools45.Office;
using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;

using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Table = Autodesk.AutoCAD.DatabaseServices.Table;

namespace EW
{
	public static class EW_Spreadsheet
	{
		public static void
		addExcelWorkbook()
		{
			string strPath = BaseObjs.docFullName;
			string strJN = BaseObjs.jobNumber();
			string strFN = string.Format("{0}EW.xlsx", strJN);
			string strFullPath = string.Format("{0}\\{1}", Path.GetDirectoryName(strPath), strFN);
			string mess = string.Format("{0} exists. Overwrite existing file? Y/N", strFN);

			if (FileSystem.FileExists(strFullPath)) {
				DialogResult varResponse = MessageBox.Show(mess, "File exists!", MessageBoxButtons.YesNo);

				switch (varResponse) {

					case DialogResult.Yes:

						try {
							FileSystem.DeleteFile(strFullPath);
						} catch (Exception) {
							MessageBox.Show("Error deleting existing file - good luck");
							return;
						}

						FileSystem.CopyFile("R:\\TSet\\Template\\EARTHWORK\\0000EW.xlsx", strFullPath);
						mess = string.Format("{0} created in {1} folder.", strFN, strPath);
						MessageBox.Show(mess);

						break;
					case DialogResult.No:

						break;//do nothing					
				}


			} else {
				FileSystem.CopyFile("R:\\TSet\\Template\\EARTHWORK\\0000EW.xlsx", strFullPath);
				mess = string.Format("{0} created in {1} folder.", strFN, strPath);
				MessageBox.Show(mess);
			}

			Excel_ext excl = new Excel_ext(true);
			excl.OpenFile(strFullPath, "");
			excl.FindExcelWorksheet("SUMMARY");            
			Excel._Worksheet objWS = excl.excelWrkSht;
			
			ProjectData varProjectData = Misc.getProjectData(strJN);
			if (varProjectData.Name.ToString() != string.Empty) {                
				objWS.Range["SUMMARY!SITE_NAME"].Value = varProjectData.Name;
				objWS.Range["SUMMARY!LOCATION"].Value = varProjectData.Location;
				objWS.Range["SUMMARY!CLIENT"].Value = varProjectData.Client;

				objWS.Range["SUMMARY!JOB_NUMBER"].Value = varProjectData.Number;
				objWS.Range["SUMMARY!COORDINATOR"].Value = varProjectData.Coordinator;
				objWS.Range["SUMMARY!SOURCE"].Value = BaseObjs.docName;

			}

			double dblAreaPad = 0;
			int intCount = 0;

			SelectionSet objSSet = EW_Utility1.buildSSetTable();
			Table objTable = (Table)objSSet.GetObjectIds()[0].getEnt();
			if (EW_Pub.OX_LIMIT_H == 0) {
				EW_Pub.OX_LIMIT_H = double.Parse(objTable.Cells[20, 1].Value.ToString());              
			}

			objSSet = EW_Utility1.buildSSetBLDG_LIM();

			foreach (ObjectId idBldgLim in objSSet.GetObjectIds()) {
				ObjectId idBldgLimOffset = idBldgLim.offset(EW_Pub.OX_LIMIT_H);

				if ((idBldgLimOffset.IsNull)) {
					intCount++;
				} else {
					dblAreaPad = dblAreaPad + idBldgLimOffset.getArea();
					idBldgLimOffset.delete();

				}

			}

			if (intCount > 0) {
				MessageBox.Show(string.Format("{0} Building Limit Area(s) failed.  Requires manual calculation.", intCount));
			}

			objWS.Range["SUMMARY!AREA_PAD"].Value = (int)dblAreaPad;

			double dblAreaSite = 0;
			Polyline objGradingLim = default(Polyline);
			objSSet = EW_Utility1.buildSSetGradingLim();
			objGradingLim = (Polyline)objSSet.GetObjectIds()[0].getEnt();
			dblAreaSite = objGradingLim.Area;

			objWS.Range["SUMMARY!AREA_SITE"].Value = (int)dblAreaSite;

			excl.CloseFile(true, strFullPath, false);

		}

		public static void
		updateSpreadsheet(ref double dblVolFill, ref double dblVolCut)
		{
			string strPath = BaseObjs.docFullName;
			string strJN = BaseObjs.jobNumber();
			string strFN = string.Format("{0}EW.xlsx", strJN);
			string strFullPath = strPath + "\\" + strFN;

			SelectionSet objSSet = EW_Utility1.buildSSet22();

			if ((dblVolFill == 0)) {

				bool exists;
				ObjectId idSurfaceCUT = Surf.getSurface("VOL_EXIST_BOT", out exists);
				ObjectId idSurfaceFILL = Surf.getSurface("VOL_BOT_SG", out exists);

				TinVolumeSurface objSurfaceFILL = (TinVolumeSurface)idSurfaceFILL.getEnt();
				TinVolumeSurface objSurfaceCUT = (TinVolumeSurface)idSurfaceCUT.getEnt();

				dblVolCut = objSurfaceFILL.GetVolumeProperties().UnadjustedCutVolume / 27;
				dblVolFill = objSurfaceFILL.GetVolumeProperties().UnadjustedFillVolume / 27;
				dblVolCut = dblVolCut + objSurfaceCUT.GetVolumeProperties().UnadjustedCutVolume / 27;
				dblVolFill = dblVolFill + objSurfaceCUT.GetVolumeProperties().UnadjustedFillVolume / 27;
			}

			if (spreadSheetExists(strFullPath, dblVolFill, dblVolCut)) {

				Excel_ext excl = new Excel_ext(true);
				excl.OpenFile(strFullPath, "");
				excl.FindExcelWorksheet("SUMMARY");

				Excel._Worksheet objWS = excl.excelWrkSht;

				objWS.Range["SUMMARY!volCUT"].Value = dblVolCut;
				objWS.Range["SUMMARY!volFILL"].Value = dblVolFill;

				//objWS.Range["SUMMARY!AREA_BLDG"].Value = 0;

				excl.CloseFile(true, strFullPath, false);
				excl.excelAPP.Quit();

			} else {
				MessageBox.Show("Error copying EW spreadsheet");

			}

			objSSet = EW_Utility1.buildSSet8();
			if(objSSet != null && objSSet.Count > 0){
				ObjectId[] ids = objSSet.GetObjectIds();
				for (int i = 0; i < ids.Length; i++) {
					ids[i].delete();
				}                
			}

			setupSpreadSheetMS(dblVolFill, dblVolCut);

		}

		public static bool
		spreadSheetExists(string strFullPath, double lngVolFill, double lngVolCut)
		{
			if (FileSystem.FileExists(strFullPath))
			{
				return true;
			}
			else
			{
				FileSystem.CopyFile("R:\\TSet\\Template\\EARTHWORK\\0000EW.xlsx", strFullPath);
				setupSpreadSheetMS(lngVolFill, lngVolCut);

				return FileSystem.FileExists(strFullPath);
			}
		}

		public static void
		setupSpreadSheetMS(double dblVolCut = 0, double dblVolFill = 0)
		{
			bool boolIsOpen = false;

			if (dblVolFill == 0) {

				bool exists;
				ObjectId idSurfaceCUT = Surf.getSurface("VOL_EXIST_BOT", out exists);
				ObjectId idSurfaceFILL = Surf.getSurface("VOL_BOT_SG", out exists);

				TinVolumeSurface objSurfaceFILL = (TinVolumeSurface)idSurfaceFILL.getEnt();
				TinVolumeSurface objSurfaceCUT = (TinVolumeSurface)idSurfaceCUT.getEnt();

				dblVolCut = objSurfaceFILL.GetVolumeProperties().UnadjustedCutVolume / 27;
				dblVolFill = objSurfaceFILL.GetVolumeProperties().UnadjustedFillVolume / 27;
				dblVolCut = dblVolCut + objSurfaceCUT.GetVolumeProperties().UnadjustedCutVolume / 27;
				dblVolFill = dblVolFill + objSurfaceCUT.GetVolumeProperties().UnadjustedFillVolume / 27;

			}

			string strPath = BaseObjs.docFullName;
			string strJN = BaseObjs.jobNumber();
			string strFN = string.Format("{0}EW.xlsx", strJN);
			string strFullPath = strPath + "\\" + strFN;

			SelectionSet objSSet = EW_Utility1.buildSSet8();
			objSSet.eraseSelectedItems();

			objSSet = EW_Utility1.buildSSetTable();
			Table objTable = (Table)objSSet.GetObjectIds()[0].getEnt();

			Point3d varPntIns = objTable.Position;

			double dblWidth = objTable.Width;
			double dblHeight = objTable.Height;

			double dblPntX = varPntIns.X + dblWidth + 10;
			double dblPntY = varPntIns.Y - 8;

			string strPntX = System.Math.Round(dblPntX, 2).ToString();
			string strPntY = System.Math.Round(dblPntY, 2).ToString();

			string strPntIns = string.Format("{0},{1}", strPntX, strPntY);

			Excel._Application objExcelApp = (Excel._Application)Microsoft.VisualBasic.Interaction.GetObject(null, "Excel.Application");
			Excel.Workbook objWB = null;
			Excel_ext excl = null;
			if (objExcelApp == null) {
				excl = new Excel_ext();
				objExcelApp = excl.excelAPP;
			} else {
				
				for (int i = 1; i < objExcelApp.Workbooks.Count; i++) {
					objWB = objExcelApp.Workbooks[i];
					if (objWB.Name == strFN) {
						boolIsOpen = true;
						break;
					}
				}
			}

			objExcelApp.Visible = true;


			if (!boolIsOpen) {

				if (FileSystem.FileExists(strFullPath)) {
					objWB = objExcelApp.Workbooks.Open(strFullPath);


				} else {
					FileSystem.CopyFile("R:\\TSet\\Template\\EARTHWORK\\0000EW.xlsx", strFullPath);
					objWB = objExcelApp.Workbooks.Open(strFullPath);
					string mess = string.Format("{0} not found.  A copy of the template has been created in {1}\nExiting...", strFN, strPath);
					MessageBox.Show(mess);
				}

			}

			Excel.Worksheet objWS = objWB.Worksheets["SUMMARY"];
			objWS.Activate();
			objWS.Visible = Microsoft.Office.Interop.Excel.XlSheetVisibility.xlSheetVisible;

			objWS.Range["SUMMARY!volCUT"].Value = dblVolCut;
			objWS.Range["SUMMARY!volFILL"].Value = dblVolFill;
			objWS.Range["SUMMARY!JOB_NUMBER"].Value = BaseObjs.docName.Substring(1, 4);
			objWS.Range["EW_SUMMARY"].Copy();
			objExcelApp.Visible = true;

			BaseObjs._editor.Regen();

			BaseObjs._acadDoc.SendStringToExecute(string.Format("_pasteclip {0}\r", strPntIns), true, false, false);

			objSSet = EW_Utility1.buildSSet8();

			Autodesk.AutoCAD.DatabaseServices.Ole2Frame objAcadOle = null;
			if (objSSet == null || objSSet.Count == 0) {
				SelectionSet ss = BaseObjs._editor.SelectAll().Value;
				int k = ss.Count;
				ObjectId id = ss.GetObjectIds()[k - 1];
				using (var tr = BaseObjs.startTransactionDb())
				{
					objAcadOle = (Ole2Frame)tr.GetObject(id, OpenMode.ForWrite);
					objAcadOle.LockAspect = false;
					objAcadOle.WcsHeight = dblHeight;
					objAcadOle.WcsWidth = dblHeight / 2;
					objAcadOle.Layer = "ZZ_ZZ-SPREADSHEET";
					tr.Commit();
				}
			}
			else {
				using(var tr = BaseObjs.startTransactionDb()){
					objAcadOle = (Ole2Frame)tr.GetObject(objSSet.GetObjectIds()[0],OpenMode.ForWrite);
					objAcadOle.LockAspect = false;
					objAcadOle.WcsHeight = dblHeight;
					objAcadOle.WcsWidth = dblHeight / 2;
					objAcadOle.Layer = "ZZ_ZZ-SPREADSHEET";                    
				}
			}

			BaseObjs.acadActivate();
		}

		//setup spreadsheet in paperspace

		public static void
		setupSpreadSheetPS(string strFullPath, double dblHeight)
		{
			string strPath = BaseObjs.docFullName;
			string strJN = BaseObjs.jobNumber();
			string strFN = string.Format("{0}EW.xlsx", strJN);

			string strPntX = "24";
			string strPntY = "6";

			string strPntIns = string.Format("{0},{1}", strPntX, strPntY);

			Excel_ext excl = new Excel_ext(true);
			excl.OpenFile(strFullPath, "");
			excl.FindExcelWorksheet("SUMMARY");
			Excel._Worksheet objWS = excl.excelWrkSht;

			objWS.Range["EW_SUMMARY"].Copy();
			excl.excelAPP.Visible = false;

			BaseObjs._acadDoc.SendStringToExecute(string.Format("_pasteclip {0}\r", strPntIns), true, false, false);
			SelectionSet ss = BaseObjs._editor.SelectAll().Value;
			int k = ss.Count;
			ObjectId id = ss.GetObjectIds()[k - 1];
			using(var tr = BaseObjs.startTransactionDb()){
				Autodesk.AutoCAD.DatabaseServices.Ole2Frame objAcadOle = (Ole2Frame)tr.GetObject(id, OpenMode.ForWrite);
				objAcadOle.LockAspect = false;
				objAcadOle.WcsHeight = dblHeight;
				objAcadOle.WcsWidth = dblHeight / 2;
				tr.Commit();                
			}
		}
	}
}
