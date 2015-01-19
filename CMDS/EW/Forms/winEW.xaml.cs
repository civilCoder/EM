using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using App = Autodesk.AutoCAD.ApplicationServices.Application;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;
using Table = Autodesk.AutoCAD.DatabaseServices.Table;
using View = Base_Tools45.View;

namespace EW.Forms {
    /// <summary>
    /// Interaction logic for winEW.xaml
    /// </summary>
    public partial class winEW : Window {
        public winEW() {
            InitializeComponent();

            Layer.manageLayers("CPNT-BRKLINE",      41);
            Layer.manageLayers("SG-BRKLINE",        50);
            Layer.manageLayers("SG-BRKLINE-AREA",   60);
            Layer.manageLayers("OX-BRKLINE",        120);
            Layer.manageLayers("OX-BRKLINE-AREA",   130);
            Layer.manageLayers("BOT-BRKLINE",       140);
            Layer.manageLayers("BOT-BRKLINE-AREA",  150);
        }

        DateTime varTimeBeg;
        DateTime varTimeEnd;

        static bool exists = false;

        public bool GRID { get; set; }

        public Object[] GRIDDATA { get; set; }

        private void cmdSetup_Click(object sender, RoutedEventArgs e){
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Browse to Project Folder";
            fbd.ShowDialog();

            string strPath = fbd.SelectedPath;

            int intPosX = 1, intPos = 0;

            if (strPath == "") {
                System.Windows.MessageBox.Show("No Folder Selected");
                return;
            }else {
                while (intPosX != 0) {
                    intPos = intPosX + 1;
                    intPosX = (short)(strPath.IndexOf("\\", intPos - 1) + 1);
                }
            }

            string strJN = strPath.Substring(intPos - 1);

            strPath = string.Format("{0}\\{1}EW.dwg", strPath, strJN);

            if (FileSystem.FileExists(strPath)) {
                MessageBoxResult varResponse = System.Windows.MessageBox.Show(string.Format("File {0} exists! Make Backup??", strPath), "File Exists!", MessageBoxButton.YesNoCancel);

                if (varResponse == MessageBoxResult.Yes) {
                    FileInfo fileInfo;

                    fileInfo = FileSystem.GetFileInfo(strPath);

                    string strDateCreated = (fileInfo.LastAccessTime).ToString();
                    intPos = (short)(strDateCreated.IndexOf(" ") + 1);
                    strDateCreated = strDateCreated.Substring(0, intPos - 1);
                    strDateCreated = strDateCreated.Replace("/", "_");
                    intPos = strPath.IndexOf(".");

                    FileSystem.CopyFile(strPath, string.Format("{0}_{1}.dwg", strPath.Substring(0, intPos - 1), strDateCreated));
                    FileSystem.DeleteFile(strPath);
                }else if (varResponse == MessageBoxResult.No) {
                    FileSystem.DeleteFile(strPath);
                }else if (varResponse == MessageBoxResult.Cancel) {
                    return;
                }
            }

            using (BaseObjs._acadDoc.LockDocument()) {
                Document objDwgEW = DocumentCollectionExtension.Add(BaseObjs._acadDocs, @"R:\TSET\Template\CIVIL3D2015\EW.dwt");
                BaseObjs._acadDoc.Database.SaveAs(strPath, true, DwgVersion.Current, BaseObjs._acadDoc.Database.SecurityParameters);
            }

            using (BaseObjs._acadDoc.LockDocument()) {
                xRef.doMXR();
            }
            Point3d pnt3dUR = BaseObjs._acadDoc.Database.Extmax;

            SelectionSet objSSet = EW_Utility1.buildSSetTable();
            ObjectId[] ids = objSSet.GetObjectIds();
            if (ids == null || ids.Length == 0)
                return;

            Table tbl = null;

            using (BaseObjs._acadDoc.LockDocument()) {
                ObjectId id = ids[0];
                try {
                    using (Transaction tr = BaseObjs.startTransactionDb()) {
                        tbl = (Table)tr.GetObject(id, OpenMode.ForWrite);
                        Extents3d ext3d = (Extents3d)tbl.Bounds;
                        tbl.TransformBy(Matrix3d.Displacement(new Vector3d(pnt3dUR.X, pnt3dUR.Y, 0)));

                        objSSet = Base_Tools45.Select.buildSSet(typeof(Polyline), ext3d.MinPoint, ext3d.MaxPoint);
                        ids = objSSet.GetObjectIds();
                        ObjectIdCollection idsm = new ObjectIdCollection();
                        foreach (ObjectId idm in ids)
                            idsm.Add(idm);
                        idsm.moveObjs(Point3d.Origin, pnt3dUR);

                        tr.Commit();
                    }
                }
                catch (System.Exception ex) {
                    BaseObjs.writeDebug(string.Format("{0} frmEarthwork.cs: line: 137", ex.Message));
                }

                Layer.manageLayers("0");

                string strDwgName = BaseObjs.docName;
                intPos = strDwgName.IndexOf(".") + 1;
                strDwgName = strDwgName.Substring(0, intPos - 1);
                BlockReference blkRef;

                blkRef = xRef.getXRefBlockReference("CNTL");
                if (blkRef != null) {
                    string strBlkName = blkRef.Name;

                    BaseObjs._acadDoc.SendStringToExecute(string.Format("-LA\rF\r{0}*\r\r", strBlkName), true, false, false);

                    BaseObjs._acadDoc.SendStringToExecute(string.Format("-LA\rT\r{0}|CURB\r\r", strBlkName), true, false, false);
                    BaseObjs._acadDoc.SendStringToExecute(string.Format("-LA\rT\r{0}|GB\r\r", strBlkName), true, false, false);
                    BaseObjs._acadDoc.SendStringToExecute(string.Format("-LA\rT\r{0}|BLDG\r\r", strBlkName), true, false, false);
                    BaseObjs._acadDoc.SendStringToExecute(string.Format("-LA\rT\r{0}|FL\r\r", strBlkName), true, false, false);
                    BaseObjs._acadDoc.SendStringToExecute(string.Format("-LA\rT\r{0}|FS\r\r", strBlkName), true, false, false);
                    BaseObjs._acadDoc.SendStringToExecute(string.Format("-LA\rT\r{0}|GUTTER\r\r", strBlkName), true, false, false);
                    BaseObjs._acadDoc.SendStringToExecute(string.Format("-LA\rT\r{0}|WALL\r\r", strBlkName), true, false, false);
                }

                blkRef = xRef.getXRefBlockReference("BNDRY");
                if (blkRef != null) {
                    string strBlkName = blkRef.Name;

                    BaseObjs._acadDoc.SendStringToExecute(string.Format("-LA\rF\r{0}*\r\r", strBlkName), true, false, false);
                    BaseObjs._acadDoc.SendStringToExecute(string.Format("-LA\rT\r{0}|PL\r\r", strBlkName), true, false, false);
                }
               
                View.ZoomExtents();
            }
            System.Windows.Forms.MessageBox.Show("User needs to create data shortcut for Surfaces: CPNT-ON and EXIST");
        }

        private void cmdBuildAreas_Click(object sender, RoutedEventArgs e) {
            this.Hide();
            App.ShowModelessWindow(EW_Forms.ewFrms.wBuildAreas);
        }

        private void cmdGetSpreadsheet_Click(object sender, RoutedEventArgs e) {
            EW_Spreadsheet.addExcelWorkbook();
        }

        private void cmdGetSegs_Click(object sender, RoutedEventArgs e) {
            BaseObjs._acadDoc.SendStringToExecute("UNDO\rMARK\r", true, false, false);

            EW_Utility1.deleteByLayer("PPNT-ON-BRKLINE");
            resetSG();
            EW_GetTriangleSegs.getTriangleSegs("CPNT-ON");
            this.cmdMakeSurfaceSG.IsEnabled = true;
        }

        private void cmdMakeSurfaceSG_Click(object sender, RoutedEventArgs e) {
            //using (BaseObjs._acadDoc.LockDocument())
            //{
            //    BaseObjs._editor.Command("_ucs", "w", "");
            //    BaseObjs._editor.Command("_undo", "m", "");
            //}
            using (BaseObjs._acadDoc.LockDocument()) {
                BaseObjs._acadDoc.SendStringToExecute("UNDO\rMARK\r", true, false, false);
                EW_Utility1.getTableData();
                EW.EW_Main.makeSurfaceSG();
            }
            this.cmdMakeSurfaceSG.IsEnabled = true;
        }

        private void cmdMakeSurfaceOX_Click(object sender, RoutedEventArgs e) {
            BaseObjs._acadDoc.SendStringToExecute("UNDO\rMARK\r", true, false, false);

            EW_Utility1.getTableData();
            EW.EW_Main.makeSurfaceOX();
            EW.EW_Main.makeSurfaceOXg();
            this.cmdGrid.IsEnabled = true;
        }

        private void cmdResetSG_Click(object sender, RoutedEventArgs e) {
            using (BaseObjs._acadDoc.LockDocument())
            {
                resetSG();
                BaseObjs.updateGraphics();
            }
        }

        private void cmdResetOX_Click(object sender, RoutedEventArgs e) {
            using (BaseObjs._acadDoc.LockDocument())
            {
                resetOX();
                BaseObjs.updateGraphics();
            }
        }

        private void cmdGrid_Click(object sender, RoutedEventArgs e) {
            BaseObjs._acadDoc.SendStringToExecute("UNDO\rMARK\r", true, false, false);

            EW.EW_Forms.ewFrms.fGridSpacing.Show();
        }

        private void cmdTest_Click(object sender, RoutedEventArgs e) {
            BaseObjs._acadDoc.SendStringToExecute("UNDO\rMARK\r", true, false, false);

            EW_Utility1.getTableData();

            double[] dblGridData = new double[2];

            if (GRID == false) {
                ObjectId idDictBOT = Dict.getNamedDictionary("BOT", out exists);
                ResultBuffer rb = idDictBOT.getXData("BOT");
                if (rb == null)
                    return;

                dblGridData[0] = double.Parse(rb.AsArray()[1].Value.ToString());
                dblGridData[1] = double.Parse(rb.AsArray()[2].Value.ToString());
                GRIDDATA = new Object[3] { dblGridData[0], dblGridData[1], int.Parse(rb.AsArray()[2].Value.ToString()) };
            }

            EW_TestBotElev.testBotElev(GRIDDATA, false);
        }

        private void cmdMakeSurfaceBOT_Click(object sender, RoutedEventArgs e) {
            BaseObjs._acadDoc.SendStringToExecute("UNDO\rMARK\r", true, false, false);

            EW.EW_MakeSurfaceBot.makeSurfaceBOT();
        }

        private void cmdMakeSurfaceVolEXIST_BOT_Click(object sender, RoutedEventArgs e) {
            BaseObjs._acadDoc.SendStringToExecute("UNDO\rMARK\r", true, false, false);
            EW.EW_MakeSurfaceVol.makeVolSurface("EXIST", "BOT", true);
        }

        private void cmdMakeSurfaceVolBOT_SG_Click(object sender, RoutedEventArgs e) {
            BaseObjs._acadDoc.SendStringToExecute("UNDO\rMARK\r", true, false, false);

            EW.EW_MakeSurfaceVol.makeVolSurface("BOT", "SG", true);
        }

        private void cmdBalanceSite_Click(object sender, RoutedEventArgs e) {
            App.ShowModelessDialog(EW.EW_Forms.ewFrms.fBalanceSite);

            if (EW.EW_Forms.ewFrms.fBalanceSite.getExcelData()) {
                EW.EW_Forms.ewFrms.fBalanceSite.initForm();
            }
        }

        private void cmdUpdateWS_Click(object sender, RoutedEventArgs e) {
            EW.EW_Spreadsheet.setupSpreadSheetMS();
        }

        private void cmdDisplaySections_Click(object sender, RoutedEventArgs e) {
            BaseObjs._acadDoc.SendStringToExecute("UNDO\rMARK\r", true, false, false);

            EW.EW_MakeBaseLine.makeBaseline();
        }

        private static void resetSG() {
            TypedValue[] tvs = new TypedValue[9] {
                (new TypedValue((int)DxfCode.Operator, "<OR")),
                (new TypedValue((int)DxfCode.LayerName, "SG-*")),
                (new TypedValue((int)DxfCode.LayerName, "OX-*")),
                (new TypedValue((int)DxfCode.LayerName, "OXg-*")),
                (new TypedValue((int)DxfCode.LayerName, "BOT-*")),
                (new TypedValue((int)DxfCode.LayerName, "VOL-*")),
                (new TypedValue((int)DxfCode.LayerName, "GRID-POINTS")),
                (new TypedValue((int)DxfCode.LayerName, "DEBUG-*")),
                (new TypedValue((int)DxfCode.Operator, "OR>"))
            };
            Misc.deleteObjs(tvs);
        }

        private static void resetOX() {
            TypedValue[] tvs = new TypedValue[8] {
                (new TypedValue((int)DxfCode.Operator, "<OR")),
                (new TypedValue((int)DxfCode.LayerName, "OX-*")),
                (new TypedValue((int)DxfCode.LayerName, "OXg-*")),
                (new TypedValue((int)DxfCode.LayerName, "BOT-*")),
                (new TypedValue((int)DxfCode.LayerName, "VOL-*")),
                (new TypedValue((int)DxfCode.LayerName, "GRID-POINTS")),
                (new TypedValue((int)DxfCode.LayerName, "DEBUG-*")),
                (new TypedValue((int)DxfCode.Operator, "OR>"))
            };
            Misc.deleteObjs(tvs);
        }

        private void winEW_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            e.Cancel = true;
            this.Hide();
        }

        private void cmdTableImport_Click(object sender, RoutedEventArgs e) {
            MessageBoxResult varResponse = System.Windows.MessageBox.Show("Import Table Data?", "TABLE DATA IMPORT", MessageBoxButton.YesNo);

            if (varResponse == MessageBoxResult.Yes) {
                EW.EW_Transfer.transferObjs("TABLE");
            }else if (varResponse == MessageBoxResult.No) {
                return;
            }
        }

        private void cmdTableEdit_Click(object sender, RoutedEventArgs e) {
            EW.EW_Transfer.editTableData();
        }
    }
}