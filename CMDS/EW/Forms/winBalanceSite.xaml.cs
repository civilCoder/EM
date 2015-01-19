
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

using Autodesk.Civil.DatabaseServices;

using Base_Tools45;
using Base_Tools45.C3D;
using Base_Tools45.Office;

using Microsoft.VisualBasic.FileIO;

using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System;

using ewmsv = EW.EW_MakeSurfaceVol;
using ewtbe = EW.EW_TestBotElev;
using Label = System.Windows.Forms.Label;
using p = EW.EW_Pub;

using Table = Autodesk.AutoCAD.DatabaseServices.Table;

namespace EW.Forms
{
    /// <summary>
    /// Interaction logic for winBalanceSite.xaml
    /// </summary>
    public partial class winBalanceSite : Window
    {
        public winBalanceSite()
        {
            InitializeComponent();
            this.Title = string.Format("EARTHWORK BALANCE: {0}  {1}", BaseObjs.docName, DateTime.Today);
        }

        public List<EW_Data> ewData { get; set; }

        string strDATE = null;
        string strUSER = null;

        double dblVOL_EXIST_BOT_CUT = 0;
        double dblVOL_EXIST_BOT_FILL = 0;

        double dblVOL_BOT_SG_CUT = 0;
        double dblVOL_BOT_SG_FILL = 0;

        double dblVOL_CUT_TOT = 0;
        double dblVOL_FILL_TOT = 0;

        double dblVOL_CUT_ADJ_NET = 0;
        double dblVOL_FILL_ADJ_NET = 0;

        double dblSG_MEAN_ELEV0 = 0;
        double dblSG_MEAN_ELEV = 0;

        double dblAREA_PAD = 0;
        double dblAREA_SITE = 0;

        double dblSHRINKAGE_FACTOR = 0;

        double dblVOL_CUT_SHRINK = 0;
        double dblSITE_ADJUST = 0;

        double dblVOL_NET = 0;
        double dblVOL_CUT_NET = 0;
        double dblVOL_FILL_NET = 0;

        EW_Data varDataCurrent = new EW_Data();

        bool exists;

        private void
        cmdClearHistory_Click(object sender, EventArgs e)
        {
            EW_Data vData = new EW_Data();
            List<EW_Data> varData = new List<EW_Data>();

            vData.ITERATION = "";

            Dict.removeNamedDictionary("EARTHWORK");
            ObjectId idDict = Dict.getNamedDictionary("EARTHWORK", out exists);

            ewData.Add(vData);
            this.DG1.DataContext = ewData;
        }

        private void
        cmdDone_Click(object sender, EventArgs e)
        {
            this.Hide();
            EW_Forms.ewFrms.fEarthwork.Show();
        }

        private void
        cmdOK_Click(object sender, EventArgs e)
        {
            this.Hide();
            balanceSite();
            List<EW_Data> ewData = getDictData();
            ewData.Add(varDataCurrent);
            this.DG1.DataContext = ewData;
            this.Show();
        }

        public bool
        getExcelData()
        {
            ObjectId[] ids = default(ObjectId[]);
            Polyline poly = default(Polyline);

            try
            {
                SelectionSet objSSet = EW_Utility1.buildSSetGradingLim();
                switch (objSSet.Count)
                {
                    case 0:
                        MessageBox.Show("0 Items found on layer GRADING LIMIT - Exiting...");
                        break;
                    case 1:
                        poly = (Polyline)objSSet.GetObjectIds()[0].getEnt();
                        dblAREA_SITE = poly.Area;
                        break;
                    default:
                        ids = objSSet.GetObjectIds();
                        for (int i = 0; i < ids.Length; i++)
                        {
                            poly = (Polyline)ids[i].getEnt();
                            dblAREA_SITE += poly.Area;
                        }
                        break;
                }

                objSSet = EW_Utility1.buildSSetTable();
                Autodesk.AutoCAD.DatabaseServices.Table objTable = (Table)objSSet.GetObjectIds()[0].getEnt();
                if (p.OX_LIMIT_H == 0)
                {
                    p.OX_LIMIT_H = double.Parse(objTable.Cells[20, 1].Value.ToString());
                }

                objSSet = EW_Utility1.buildSSet9();
                ids = objSSet.GetObjectIds();
                for (int i = 0; i < ids.Length; i++)
                {
                    ObjectId idOff = ids[i].offset(p.OX_LIMIT_H);

                    dblAREA_PAD = dblAREA_PAD + idOff.getArea();
                    idOff.delete();

                }
                string strPath = BaseObjs.docFullName;
                string strJN = BaseObjs.jobNumber();
                string strFN = string.Format("{0}EW.xlsx", strJN);
                string strFullPath = strPath + "\\" + strFN;

                Excel_ext excl = new Excel_ext();
                Microsoft.Office.Interop.Excel.Workbook objWB;
                Microsoft.Office.Interop.Excel.Worksheet objWS;

                if (!FileSystem.FileExists(strFullPath))
                {
                    FileSystem.CopyFile("R:\\TSet\\Template\\EARTHWORK\\0000EW.xlsx", strFullPath);
                    string mess = string.Format("{0} not found.  A copy of the template has been created in {1} Exiting...", strFN, strPath);
                    MessageBox.Show(mess);
                }

                excl.OpenFile(strFullPath, "");
                objWB = excl.excelWB;

                objWS = objWB.Worksheets["SUMMARY"];
                objWS.Activate();

                //objWS.Range("SUMMARY!AREA_SITE").Value = dblAREA_SITE

                //Try
                //    objRange = objWS.Range("SUMMARY!AREA_PAD")
                //Catch ex As Exception
                //End Try

                //If (objRange Is Nothing) Then
                //    objRange = objWS.Range("C7")
                //    objRange.Name = "AREA_PAD"
                //End If

                //objRange.Value = Format(dblAREA_PAD, "###,###.00")

                dblSHRINKAGE_FACTOR = objWS.Range["SUMMARY!shrinkageFactor"].Text;

                double lngVolCut = objWS.Range["SUMMARY!volCUT"].Value;
                double lngVolFill = objWS.Range["SUMMARY!volFILL"].Value;

                double lngVolCutAdj = objWS.Range["SUMMARY!adjCUT"].Value;

                double dblVolCutShrink = lngVolCut * dblSHRINKAGE_FACTOR;

                dblVOL_CUT_ADJ_NET = lngVolCutAdj - (lngVolCut - dblVolCutShrink);//Net adjustment

                dblVOL_FILL_ADJ_NET = 0;

                excl.CloseFile(true, strFullPath, false);

            }
            catch (Exception)
            {
                MessageBox.Show("Error in getExcelData");
                return false;
            }
            return true;
        }


        public void
        balanceSite()
        {
            double dblSiteAdjust = 0;

            Point3d pnt3dMove0 = Point3d.Origin;

            TinSurface objSurfaceSG = Surf.getTinSurface("SG", out exists);
            TinSurface objSurfaceOX = Surf.getTinSurface("OX", out exists);

            dblSiteAdjust = double.Parse(this.tboxAdjSurface.Text);

            varDataCurrent.SITE_ADJUST = dblSiteAdjust;
            updateDictionary(varDataCurrent);

            Point3d pnt3dMoveX = new Point3d(0, 0, dblSiteAdjust);

            Matrix3d mtx3d = Matrix3d.Displacement(pnt3dMoveX - pnt3dMove0);

            objSurfaceSG.TransformBy(mtx3d);
            objSurfaceOX.TransformBy(mtx3d);

            ewtbe.reTest();
            ewmsv.updateVolSurface("EXIST", "BOT");
            ewmsv.updateVolSurface("BOT", "SG");

            // ERROR: Not supported in C#: OnErrorStatement


            ObjectId idSurfaceCUT = Surf.getSurface("VOL_EXIST_BOT", out exists);
            if (idSurfaceCUT.IsNull)
            {
                ewmsv.updateVolSurface("EXIST", "BOT");
            }

            ObjectId idSurfaceFILL = Surf.getSurface("VOL_BOT_SG", out exists);
            if (idSurfaceFILL.IsNull)
            {
                ewmsv.updateVolSurface("BOT", "SG");
            }

            TinVolumeSurface objSurfaceCUT = (TinVolumeSurface)idSurfaceCUT.getEnt();
            dblVOL_EXIST_BOT_CUT = objSurfaceCUT.GetVolumeProperties().UnadjustedCutVolume / 27;
            dblVOL_EXIST_BOT_FILL = objSurfaceCUT.GetVolumeProperties().UnadjustedFillVolume / 27;

            TinVolumeSurface objSurfaceFILL = (TinVolumeSurface)idSurfaceFILL.getEnt();
            dblVOL_BOT_SG_CUT = objSurfaceFILL.GetVolumeProperties().UnadjustedCutVolume / 27;
            dblVOL_BOT_SG_FILL = objSurfaceFILL.GetVolumeProperties().UnadjustedFillVolume / 27;

            dblVOL_CUT_TOT = dblVOL_EXIST_BOT_CUT + dblVOL_BOT_SG_CUT;
            //Gross Cut put in spreadsheet
            dblVOL_FILL_TOT = dblVOL_EXIST_BOT_FILL + dblVOL_BOT_SG_FILL;
            //Gross Fill put in spreadsheet

            dblVOL_CUT_SHRINK = (dblVOL_CUT_TOT * dblSHRINKAGE_FACTOR * -1);

            dblVOL_CUT_NET = dblVOL_CUT_ADJ_NET + dblVOL_CUT_TOT + dblVOL_CUT_SHRINK;
            dblVOL_FILL_NET = dblVOL_FILL_ADJ_NET + dblVOL_FILL_TOT;

            dblSG_MEAN_ELEV = objSurfaceSG.GetGeneralProperties().MeanElevation;

            dblVOL_NET = (dblVOL_CUT_NET - dblVOL_FILL_NET);

            dblSITE_ADJUST = dblVOL_NET * 27 / dblAREA_SITE;

            this.AdjustSurfaceSG.Text = string.Format("{0:#0.00", dblSG_MEAN_ELEV - dblSG_MEAN_ELEV0);

            varDataCurrent.ITERATION = "C";
            varDataCurrent.DATE = strDATE;
            varDataCurrent.USER = strUSER;
            varDataCurrent.AREA_SITE = dblAREA_SITE;
            varDataCurrent.SG_MEAN_ELEV = dblSG_MEAN_ELEV;
            varDataCurrent.VOL_CUT_TOT = dblVOL_CUT_TOT;
            varDataCurrent.VOL_CUT_ADJ_NET = dblVOL_CUT_ADJ_NET;
            varDataCurrent.VOL_CUT_SHRINK = dblVOL_CUT_SHRINK;
            varDataCurrent.VOL_CUT_NET = dblVOL_CUT_NET;
            varDataCurrent.VOL_FILL_TOT = dblVOL_FILL_TOT;
            varDataCurrent.VOL_FILL_ADJ_NET = dblVOL_FILL_ADJ_NET;
            varDataCurrent.VOL_FILL_NET = dblVOL_FILL_NET;
            varDataCurrent.VOL_NET = dblVOL_NET;
            varDataCurrent.SITE_ADJUST = dblSITE_ADJUST;

            ewData = getDictData();
            ewData.Add(varDataCurrent);
            this.DG1.DataContext = ewData;

            return;
        }

        public void
        initForm()
        {
            TinVolumeSurface objSurfaceTIN = Surf.getTinVolumeSurface("VOL_EXIST_BOT");
            dblVOL_EXIST_BOT_CUT = objSurfaceTIN.GetVolumeProperties().UnadjustedCutVolume / 27;
            dblVOL_EXIST_BOT_FILL = objSurfaceTIN.GetVolumeProperties().UnadjustedFillVolume / 27;

            objSurfaceTIN = Surf.getTinVolumeSurface("VOL_BOT_SG");
            dblVOL_BOT_SG_CUT = objSurfaceTIN.GetVolumeProperties().UnadjustedCutVolume / 27;
            dblVOL_BOT_SG_FILL = objSurfaceTIN.GetVolumeProperties().UnadjustedFillVolume / 27;

            TinSurface objSurfaceCPNT = Surf.getTinSurface("CPNT-ON");
            TinSurface objSurfaceSG = Surf.getTinSurface("SG");

            dblSG_MEAN_ELEV = System.Math.Round(objSurfaceSG.GetGeneralProperties().MeanElevation, 2);
            dblVOL_CUT_TOT = dblVOL_EXIST_BOT_CUT + dblVOL_BOT_SG_CUT;
            dblVOL_FILL_TOT = dblVOL_EXIST_BOT_FILL + dblVOL_BOT_SG_FILL;

            dblVOL_CUT_SHRINK = dblVOL_CUT_TOT * dblSHRINKAGE_FACTOR * -1;
            dblVOL_CUT_NET = dblVOL_CUT_TOT + dblVOL_CUT_ADJ_NET + dblVOL_CUT_SHRINK;

            dblVOL_FILL_NET = dblVOL_FILL_TOT + dblVOL_FILL_ADJ_NET;
            dblVOL_NET = dblVOL_CUT_NET - dblVOL_FILL_NET;

            dblSITE_ADJUST = dblVOL_NET * 27 / dblAREA_SITE;

            strDATE = DateTime.Today.ToShortDateString();
            strUSER = System.Environment.UserName;

            EW_Data varDataCurrent = new EW_Data();
            varDataCurrent.ITERATION = "C";
            varDataCurrent.DATE = strDATE;
            varDataCurrent.USER = strUSER;
            varDataCurrent.AREA_SITE = dblAREA_SITE;
            varDataCurrent.SG_MEAN_ELEV = dblSG_MEAN_ELEV;
            varDataCurrent.VOL_CUT_TOT = dblVOL_CUT_TOT;
            varDataCurrent.VOL_FILL_ADJ_NET = dblVOL_CUT_ADJ_NET;
            varDataCurrent.VOL_CUT_SHRINK = dblVOL_CUT_SHRINK;
            varDataCurrent.VOL_CUT_NET = dblVOL_CUT_NET;
            varDataCurrent.VOL_FILL_TOT = dblVOL_FILL_TOT;
            varDataCurrent.VOL_FILL_ADJ_NET = dblVOL_FILL_ADJ_NET;
            varDataCurrent.VOL_FILL_NET = dblVOL_FILL_NET;
            varDataCurrent.VOL_NET = dblVOL_NET;
            varDataCurrent.SITE_ADJUST = dblSITE_ADJUST;

            //ObjectId idDictEW = Dict.getNamedDictionary("EARTHWORK", out exists);
            //idDict.delete();                                               !!!!!!!!!!!!!!!!!!!!!!!****************************!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            ObjectId idDictEW = Dict.getNamedDictionary("EARTHWORK", out exists);

            if (!exists)
            {
                this.Title = "NEW EARTHWORK - COOL!!";
                dblSG_MEAN_ELEV0 = dblSG_MEAN_ELEV;
                this.AdjustSurfaceSG.Text = string.Format("{0:#0.00}", dblSG_MEAN_ELEV - dblSG_MEAN_ELEV0);
                ewData.Add(varDataCurrent);
                this.DG1.DataContext = ewData;
            }
            else
            {
                ewData = getDictData();
                if (ewData.Count != 0)
                {
                    dblSG_MEAN_ELEV0 = ewData[0].SG_MEAN_ELEV;

                    if (dblSG_MEAN_ELEV0 == 0)
                    {
                        dblSG_MEAN_ELEV0 = dblSG_MEAN_ELEV;
                    }
                    this.AdjustSurfaceSG.Text = string.Format("{0:#0.00}", dblSG_MEAN_ELEV - dblSG_MEAN_ELEV0);
                }
                else
                {
                    dblSG_MEAN_ELEV0 = dblSG_MEAN_ELEV;

                }

                this.DG1.DataContext = ewData;
            }
        }


        public void
        updateDictionary(EW_Data vData)
        {
            //Add new record for iteration

            ObjectId idDictEW = Dict.getNamedDictionary("EARTHWORK", out exists);
            DBDictionary objDictEW = null;
            using (var tr = BaseObjs.startTransactionDb())
            {
                objDictEW = (DBDictionary)tr.GetObject(idDictEW, OpenMode.ForWrite);
                int i = objDictEW.Count;

                ObjectId idDictX = Dict.addSubDict(idDictEW, i.ToString());

                TypedValue[] tvs = new TypedValue[1] { new TypedValue(1000, vData.DATE) };
                Dict.addXRec(idDictX, "DATE", tvs);

                tvs = new TypedValue[1] { new TypedValue(1000, vData.USER) };
                Dict.addXRec(idDictX, "USER", tvs);

                tvs = new TypedValue[1] { new TypedValue(1040, vData.AREA_SITE) };
                Dict.addXRec(idDictX, "AREA", tvs);

                tvs = new TypedValue[1] { new TypedValue(1040, vData.SG_MEAN_ELEV) };
                Dict.addXRec(idDictX, "SG_MEAN_ELEV", tvs);

                tvs = new TypedValue[1] { new TypedValue(1040, vData.VOL_CUT_TOT) };
                Dict.addXRec(idDictX, "VOL_CUT_TOT", tvs);

                tvs = new TypedValue[1] { new TypedValue(1040, vData.VOL_CUT_ADJ_NET) };
                Dict.addXRec(idDictX, "VOL_CUT_ADJ_NET", tvs);

                tvs = new TypedValue[1] { new TypedValue(1040, vData.VOL_CUT_SHRINK) };
                Dict.addXRec(idDictX, "VOL_CUT_SHRINK", tvs);

                tvs = new TypedValue[1] { new TypedValue(1040, vData.VOL_CUT_NET) };
                Dict.addXRec(idDictX, "VOL_CUT_NET", tvs);

                tvs = new TypedValue[1] { new TypedValue(1040, vData.VOL_FILL_TOT) };
                Dict.addXRec(idDictX, "VOL_FILL_TOT", tvs);

                tvs = new TypedValue[1] { new TypedValue(1040, vData.VOL_FILL_ADJ_NET) };
                Dict.addXRec(idDictX, "VOL_FILL_ADJ_NET", tvs);

                tvs = new TypedValue[1] { new TypedValue(1040, vData.VOL_FILL_NET) };
                Dict.addXRec(idDictX, "VOL_FILL_NET", tvs);

                tvs = new TypedValue[1] { new TypedValue(1040, vData.VOL_NET) };
                Dict.addXRec(idDictX, "VOL_NET", tvs);

                tvs = new TypedValue[1] { new TypedValue(1040, vData.SITE_ADJUST) };
                Dict.addXRec(idDictX, "SITE_ADJUST", tvs);

                tr.Commit();
            }

        }

        public List<EW_Data>
        getDictData()
        {
            ewData = new List<EW_Data>();
            ObjectId idDictEW = Dict.getNamedDictionary("EARTHWORK", out exists);
            List<DBDictionaryEntry> entries = Dict.getEntries(idDictEW);

            int k = entries.Count;

            for (int i = 0; i < k; i++)
            {
                DBDictionaryEntry entry = entries[i];
                ObjectId idDictX = Dict.getSubDict(idDictEW, entry.Key.ToString());
                EW_Data vData = new EW_Data();

                vData.ITERATION = i.ToString();

                ResultBuffer rb = Dict.getXRec(idDictX, "DATE");
                vData.DATE = rb.AsArray()[0].Value.ToString();

                rb = Dict.getXRec(idDictX, "USER");
                vData.USER = rb.AsArray()[0].Value.ToString();

                rb = Dict.getXRec(idDictX, "AREA");
                vData.AREA_SITE = double.Parse(rb.AsArray()[0].Value.ToString());

                rb = Dict.getXRec(idDictX, "SG_MEAN_ELEV");
                vData.SG_MEAN_ELEV = double.Parse(rb.AsArray()[0].Value.ToString());

                rb = Dict.getXRec(idDictX, "VOL_CUT_TOT");
                vData.VOL_CUT_TOT = double.Parse(rb.AsArray()[0].Value.ToString());

                rb = Dict.getXRec(idDictX, "VOL_CUT_ADJ_NET");
                vData.VOL_CUT_ADJ_NET = double.Parse(rb.AsArray()[0].Value.ToString());

                rb = Dict.getXRec(idDictX, "VOL_CUT_SHRINK");
                vData.VOL_CUT_SHRINK = double.Parse(rb.AsArray()[0].Value.ToString());

                rb = Dict.getXRec(idDictX, "VOL_CUT_NET");
                vData.VOL_CUT_NET = double.Parse(rb.AsArray()[0].Value.ToString());

                rb = Dict.getXRec(idDictX, "VOL_FILL_TOT");
                vData.VOL_FILL_TOT = double.Parse(rb.AsArray()[0].Value.ToString());

                rb = Dict.getXRec(idDictX, "VOL_FILL_ADJ_NET");
                vData.VOL_FILL_ADJ_NET = double.Parse(rb.AsArray()[0].Value.ToString());

                rb = Dict.getXRec(idDictX, "VOL_FILL_NET");
                vData.VOL_FILL_NET = double.Parse(rb.AsArray()[0].Value.ToString());

                rb = Dict.getXRec(idDictX, "VOL_NET");
                vData.VOL_NET = double.Parse(rb.AsArray()[0].Value.ToString());

                rb = Dict.getXRec(idDictX, "SITE_ADJUST");
                vData.SITE_ADJUST = double.Parse(rb.AsArray()[0].Value.ToString());

                ewData.Add(vData);

            }

            return ewData;

        }

        private void
        cmdOpenWS_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            string strPath = BaseObjs.docFullName;
            string strJN = BaseObjs.docName.Substring(1, 4);
            string strFN = strJN + "EW" + ".xlsx";
            string strFullPath = strPath + "\\" + strFN;

            Microsoft.Office.Interop.Excel.Application objExcelApp = new Microsoft.Office.Interop.Excel.Application();
            objExcelApp.Visible = true;

            Microsoft.Office.Interop.Excel.Workbook objWB = objExcelApp.Workbooks.Open(strFullPath);
            Microsoft.Office.Interop.Excel.Worksheet objWS = objExcelApp.Worksheets["SUMMARY"];
            objWS.Activate();

        }

        private void wBalanceSite_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }


    }
}
