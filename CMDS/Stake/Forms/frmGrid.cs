using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Color = Autodesk.AutoCAD.Colors.Color;

namespace Stake.Forms
{
    public partial class frmGrid : Form
    {
        public frmGrid()
        {
            InitializeComponent();
        }

        public List<Handle> GRIDALPHA { get; set; }

        public List<Handle> GRIDNUMERIC { get; set; }

        public List<POI> POI_ORG { get; set; }

        public int BLDG_COUNT { get; set; }

        private List<CmdStake> CmdStake = new List<CmdStake>();
        private List<CmdCheck> CmdCheck = new List<CmdCheck>();

        private void cmdDeleteAll_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            Stake_Algn.deleteAllGridAligns();
        }

        private void cmdEditBuildNames_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            Stake_Grid.editBldgNum();
        }

        private void cmdIdSecondary_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            Stake_Grid.idSecondaryGrids();
        }

        private void cmdIdGridSets_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            Stake_Grid.idGridSets();
        }

        private void cboxUseI_CheckStateChanged(System.Object eventSender, System.EventArgs eventArgs)
        {
            ASCIIEncoding ascii = new ASCIIEncoding();
            if (cboxUseI.Checked)
            {
                cbxAlpha.Items.Insert(8, ascii.GetString(new Byte[] { (Byte)73 }));
            }
            else
            {
                if (cbxAlpha.Items[8].ToString() == ascii.GetString(new Byte[] { (Byte)73 }))
                {
                    cbxAlpha.Items.RemoveAt((8));
                }
            }
        }

        private void cmdAlignCreate_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            Stake_Grid.processGrid();
        }

        private void cmdAlignDelete_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            Stake_Algn.deleteGridAlign();
        }

        private void cmdGetAlpha_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            BaseObjs.acadActivate();
            Entity ent = null;
            Point3d pnt3dPicked = Pub.pnt3dO;
            try
            {
                string msg = string.Format("\nSelect the {0}A{1} grid line:\n", 34.asciiToString(), 34.asciiToString());
                ent = Base_Tools45.Select.selectEntity(typeof(Line), msg, "", out pnt3dPicked);
                if (ent == null)
                    return;
            }
            catch (System.Exception)
            {
                return;
            }

            Line objGridAlphaA = (Line)ent;
            Color color = new Color();
            color = Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByBlock, 6);
            objGridAlphaA.ObjectId.changeProp(LineWeight.ByLayer, color);

            GRIDALPHA.Add(objGridAlphaA.Handle);

            int i = objGridAlphaA.Layer.IndexOf("GRID");

            string strName = objGridAlphaA.Layer.Substring(i);

            Stake_Grid.setupGrid(objGridAlphaA.ObjectId, strName, "ALPHA");

            Stake_Dict.updateDictGRIDsWithBldgName(GRIDALPHA, strName, "ALPHA");
        }

        private void cmdGetNumeric_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            BaseObjs.acadActivate();
            Entity ent = null;
            Point3d pnt3dPicked = Pub.pnt3dO;
            try
            {
                string msg = string.Format("\nSelect the {0}1{1} grid line:\n", 34.asciiToString(), 34.asciiToString());
                ent = Base_Tools45.Select.selectEntity(typeof(Line), msg, "", out pnt3dPicked);
                if (ent == null)
                    return;
            }
            catch (System.Exception)
            {
                return;
            }

            Line objGridNumeric1 = (Line)ent;
            Color color = new Color();
            color = Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByBlock, 3);
            objGridNumeric1.ObjectId.changeProp(LineWeight.ByLayer, color);

            GRIDNUMERIC.Add(objGridNumeric1.Handle);
            int i = objGridNumeric1.Layer.IndexOf("GRID");

            string strName = objGridNumeric1.Layer.Substring(i);

            Stake_Grid.setupGrid(objGridNumeric1.ObjectId, strName, "NUMERIC");

            Stake_Dict.updateDictGRIDsWithBldgName(GRIDNUMERIC, strName, "NUMERIC");
        }

        private void cmdGridAdd_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            BaseObjs.acadActivate();

            ObjectId idLineX = Stake_Grid.addToGroup();
            string nameLayer = idLineX.getLayer();

            int i = nameLayer.IndexOf("GRID");
            string name = nameLayer.Substring(i);

            ResultBuffer rb = idLineX.getXData("GRID");
            if (rb == null)
                return;
            TypedValue[] tvs = rb.AsArray();
            int result = 0;
            string typ = "";
            if (tvs[1].Value.ToString().isInteger(out result))
            {
                GRIDNUMERIC.Add(idLineX.getHandle());
                typ = "NUMERIC";
            }
            else
            {
                GRIDALPHA.Add(idLineX.getHandle());
                typ = "ALPHA";
            }

            Stake_Dict.updateDictGRIDs(name, typ, "ADD", idLineX, idLineX.getHandle().ToString());
        }

        private void cmdGridDelete_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            BaseObjs.acadActivate();

            Point3d pnt3d = Pub.pnt3dO;
            Entity ent = Base_Tools45.Select.selectEntity(typeof(Line), "Select Grid LINE to delete:", "", out pnt3d);
            if (ent == null)
                return;

            Line objLineX = (Line)ent;

            int n = objLineX.Layer.IndexOf("GRID");
            string strName = objLineX.Layer.Substring(n);

            ResultBuffer rb = objLineX.ObjectId.getXData("GRID");
            TypedValue[] tvs = rb.AsArray();

            string strType = "";
            string strVal = tvs[1].Value.ToString();
            int res = 0;
            if (strVal.isInteger(out res))
            {
                strType = "NUMERIC";

                for (int i = 1; i <= GRIDNUMERIC.Count; i++)
                {
                    if (objLineX.Handle == GRIDNUMERIC[i])
                    {
                        GRIDNUMERIC.RemoveAt(i);
                    }
                }
            }
            else
            {
                strType = "ALPHA";

                for (int i = 1; i <= GRIDALPHA.Count; i++)
                {
                    if (objLineX.Handle == GRIDALPHA[i])
                    {
                        GRIDALPHA.RemoveAt(i);
                    }
                }
            }

            Stake_Dict.updateDictGRIDs(strName, strType, "DELETE", objLineX.ObjectId, objLineX.Handle.ToString());

            objLineX.ObjectId.delete();
        }

        private void cmdGridEdit_Click(System.Object eventSender, System.EventArgs eventArgs)
        {

            Application.ShowModelessDialog(Application.MainWindow.Handle, Stake_Forms.sForms.fGridLabelEdit, false);
        }

        private void cmdGridLabel_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            Stake_Grid.labelGrids();
        }

        private void cmdSelectGrid_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            BaseObjs.acadActivate();
            bool escape;
            string xRefPath = "";
            Entity obj = xRef.getEntity("Select feature to stake:", out escape, out xRefPath);

            ObjectId idGuideline = ObjectId.Null;

            if (obj.GetType().Name != "Line")
            {
                Application.ShowAlertDialog("Grid Lines need to be simple Acad Lines, not Polylines, etc. - exiting...");
                return;
            }
            else
            {
                idGuideline = Stake_GetGuidelines.getGuidelines(obj);

                string strLayerName = idGuideline.getLayer();

                int intPos = strLayerName.IndexOf("|");
                strLayerName = strLayerName.Substring(intPos + 1);

                if (!Stake_Main.testClass("BLDG", strLayerName))
                {
                    return;
                }

                Stake_Forms.sForms.fStake.NameStakeObject = strLayerName;
            }

            Stake_GetNestedObjects.copyGRID((Stake_Forms.sForms.fStake.XRefDbModelSpace));//XrefDbModelSpace is source of grid selected

            idGuideline.delete();

            Stake_Forms.sForms.fStake.Hide();
            Application.ShowModelessDialog(Application.MainWindow.Handle, Stake_Forms.sForms.fGrid, false);
        }

        private void optPBC_CheckStateChanged(System.Object eventSender, System.EventArgs eventArgs)
        {
            if (optPBC.Checked)
            {
                tbxOffsetV.Enabled = false;
            }
        }

        private void optRBC_CheckStateChanged(System.Object eventSender, System.EventArgs eventArgs)
        {
            if (optRBC.Checked)
            {
                tbxOffsetV.Enabled = true;
            }
        }

        public void setupControls(int intNum)
        {
            Control.ControlCollection objCntrls = this.Frame05.Controls;
            Control cntrl;

            RadioButton objOpt = null;
            for (int i = objCntrls.Count - 1; i > -1; i--)
            {
                cntrl = objCntrls[i];
                if (cntrl.Name.Contains("optBTN"))
                {
                    objCntrls.RemoveAt(i);
                }
            }

            for (int i = 1; i < intNum + 1; i++)
            {
                try
                {
                    objOpt = new RadioButton();
                    objOpt.Name = "optBTN" + i;

                    cntrl = objOpt;
                    cntrl.Left = 3 + (i - 1) * 51;
                    cntrl.Top = 6;
                    cntrl.Width = 54;
                    cntrl.Height = 24;
                    cntrl.Text = "BLDG " + intNum;

                    objCntrls.Add(cntrl);
                }
                catch (System.Exception )
                {
                }
            }

            objCntrls = this.Frame08.Controls;

            for (int i = objCntrls.Count - 1; i >= 0; i--)
            {
                objCntrls.RemoveAt(i);
            }

            Button objCMD = null;

            for (int i = 1; i <= intNum; i++)
            {
                objCMD = new Button();
                objCMD.Name = "GRID" + i;
                objCMD.Image = System.Drawing.Image.FromFile("R:\\TSET\\VBA\\Q.bmp");
                objCMD.Text = "BLDG " + intNum;

                cntrl = objCMD;
                cntrl.Left = 6 + (i - 1) * 48;
                cntrl.Top = 9;
                cntrl.Height = 42;
                cntrl.Width = 42;
                objCntrls.Add(cntrl);
            }

            objCntrls = this.Frame10.Controls;

            for (int i = objCntrls.Count - 1; i >= 0; i += -1)
            {
                objCntrls.RemoveAt(i);
            }

            for (int i = 1; i <= intNum; i++)
            {
                objCMD = new Button();
                objCMD.Name = "cmd" + i;

                cntrl = objCMD;
                cntrl.Text = "BLDG " + intNum;
                cntrl.Enabled = false;

                cntrl.Left = 12 + (i - 1) * 48;
                cntrl.Top = 6;
                cntrl.Height = 24;
                cntrl.Width = 42;
                objCntrls.Add(cntrl);
            }
        }

        private void frmGrid_Load(object sender, System.EventArgs e)
        {
            this.Frame02.ForeColor = System.Drawing.Color.Yellow;
            for (int i = 4; i < 41; i++)
                cbxNumeric.Items.Add(i.ToString());

            cbxNumeric.SelectedIndex = 0;

            for (int i = 65; i <= 72; i++)
            {
                cbxAlpha.Items.Add(i.asciiToString());
            }
            for (int i = 74; i <= 90; i++)
            {
                cbxAlpha.Items.Add(i.asciiToString());
            }
            cbxAlpha.SelectedIndex = 0;

            tbxOffsetV.Text = "-0.50";

            optPBC.Checked = true;
            optOUT.Checked = true;
            optALT.Checked = true;
            ObjectId idDictGRIDS = ObjectId.Null;
            bool exists = false;
            try
            {
                idDictGRIDS = Dict.getNamedDictionary("GRIDS", out exists);
            }
            catch (System.Exception )
            {
                return;
            }

            List<DBDictionaryEntry> entries = Dict.getEntries(idDictGRIDS);
            if (entries.Count > 0)
            {
                setupControls(entries.Count);
                BLDG_COUNT = entries.Count;
            }

            if (BLDG_COUNT > 0)
            {
            }

            Stake_Dict.resetObjectIdsInDict();
        }

        private void frmGrid_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            //Application.ShowModelessDialog(Application.MainWindow.Handle, Stake_Forms.sForms.fStake, false);
            Stake_Forms.sForms.fStake.Show();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
            Application.ShowModelessDialog(Application.MainWindow.Handle, Stake_Forms.sForms.fStake, false);
        }

        private void cmdDone_Click(object sender, EventArgs e)
        {
            this.Hide();
            Application.ShowModelessDialog(Application.MainWindow.Handle, Stake_Forms.sForms.fStake, false);
        }

        private void cmdDeleteAll_Click_1(object sender, EventArgs e)
        {
            Stake_Algn.deleteAllGridAligns();
        }
    }
}