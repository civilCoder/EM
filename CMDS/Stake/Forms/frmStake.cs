using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using sFrms = Stake.Forms.Stake_Forms;

namespace Stake.Forms
{
    public partial class frmStake : Form
    {
        private static Forms.frmMisc fMisc;
        private static Forms.frmStaRange fStaRange;
        private static Forms.frmAddPoint fAddPoint;

        public frmStake()
        {
            InitializeComponent();
            optBLDG.Checked = false;
        }

        #region "Property"

        public uint NextPntNum { get; set; }

        public uint LastPntNumCURB { get; set; }

        public uint LastPntNumUTIL { get; set; }

        public uint AlignNum { get; set; }

        public int Side { get; set; }

        public MText LABEL { get; set; }

        public string CNTL_LAYER { get; set; }

        public string GCAL_LAYER { get; set; }

        public string STAKE_LAYER { get; set; }

        public string ClassObj { get; set; }

        public string XRefObjectName { get; set; }

        public string ActiveSurface { get; set; }

        public string NameStakeObject { get; set; }

        public ObjectId objectID { get; set; }

        public List<POI> POI_CALC { get; set; }

        public List<POI> POI_PNTs { get; set; }

        public List<POI> POI_ORG { get; set; }

        public List<POI> POI_STAKED { get; set; }

        public List<POI> POI_STAKE { get; set; }

        public List<PntRange> PntRanges { get; set; }

        public List<AlgnData> algnData { get; set; }

        public BlockTableRecord XRefDbModelSpace { get; set; }

        public BlockReference XRefObject { get; set; }

        public Database XRefDb { get; set; }

        public TinSurface SurfaceCPNT { get; set; }

        public Handle HandleAlign { get; set; }

        public Handle HandleProfileView { get; set; }

        public Point3d InsertProfileView { get; set; }

        private Alignment oActiveAlign;

        public Alignment ACTIVEALIGN
        {
            get
            {
                return oActiveAlign;
            }
            set
            {
                oActiveAlign = value;
                this.Text = "STAKE - Active Alignment is: " + value.Name;
            }
        }

        #endregion "Property"

        #region "Events"

        private void optBLDG_Click(object sender, EventArgs e)
        {
            if (optBLDG.Checked)
            {
                ClassObj = "BLDG";
                this.Hide();
                Application.ShowModelessDialog(Application.MainWindow.Handle, Stake_Forms.sForms.fGrid, false);
            }
        }

        private void optCURB_Click(object sender, EventArgs e)
        {
            if (optCURB.Checked)
            {
                ClassObj = "CURB";
                Stake_Misc.set_fStakeProps("CURB");
            }
        }

        private void optFL_Click(object sender, EventArgs e)
        {
            if (optFL.Checked)
            {
                ClassObj = "FL";
                Stake_Misc.set_fStakeProps("FL");
            }
        }

        private void optMISC_Click(object sender, EventArgs e)
        {
            if (optMISC.Checked)
            {
                this.ClassObj = "MISC";
                Stake_Misc.set_fStakeProps("MISC");
                this.Hide();
                fMisc = Forms.Stake_Forms.sForms.fMisc;
                Application.ShowModelessDialog(Application.MainWindow.Handle, fMisc, false);
            }
        }

        private void optSEWER_Click(object sender, EventArgs e)
        {
            if (optSEWER.Checked)
            {
                ClassObj = "SEWER";
                Stake_Misc.set_fStakeProps("SEWER");
            }
        }

        private void optSD_Click(object sender, EventArgs e)
        {
            if (optSD.Checked)
            {
                ClassObj = "SD";
                Stake_Misc.set_fStakeProps("SD");
            }
        }

        private void optWALL_Click(object sender, EventArgs e)
        {
            if (optWALL.Checked)
            {
                ClassObj = "WALL";
                Stake_Misc.set_fStakeProps("WALL");
            }
        }

        private void optUG_Click(object sender, EventArgs e)
        {
            if (optUG.Checked)
            {
                ClassObj = "WTR";
                Stake_Misc.set_fStakeProps("WTR");
            }
        }

        #endregion "Events"

        #region "Commands"

        private void cmdGetEnt_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (optSEWER.Checked || optSD.Checked)
            {
                Layer.manageLayers("0");
                Layer.manageLayer(CNTL_LAYER, layerFrozen: true);
            }

            Stake_Main.Process();

            if (ClassObj != "BLDG")
            {
                this.Show();
            }
        }

        private void cmdActivateAlign_Click(object sender, EventArgs e)
        {
            this.Hide();
            Point3d pnt3dPicked = Pub.pnt3dO;
            string nameAlign = "";
            try
            {
                ObjectId idAlign = Align.selectAlign("\nSelect Alignment: ", "\nAlignment Selection failed.", out pnt3dPicked, out nameAlign);
                ACTIVEALIGN = (Alignment)idAlign.getEnt();
            }
            catch (System.Exception)
            {
                return;
            }
            this.Show();
        }

        private void cmdAlignReverse_Click(object sender, EventArgs e)
        {
            this.Hide();
            Point3d pnt3dPicked = Pub.pnt3dO;
            string nameAlign = "";

            ObjectId idAlign = Align.selectAlign("\nSelect Alignment: ", "\nAlignment Selection failed.", out pnt3dPicked, out nameAlign);
            ACTIVEALIGN = (Alignment)idAlign.getEnt();
            Stake_Algn.reverseAlign(idAlign);

            this.Show();
        }

        private void cmdCopyStyles_Click(object sender, EventArgs e)
        {
            Stake_Styles.copyStakeStyles();
        }

        private void cmdCurbTrans_Click(object sender, EventArgs e)
        {
            this.Hide();
            Stake_GetCurbTransitions.getCurbTransitions();
            this.Show();
        }

        private void cmdAddCrossings_Click(object sender, EventArgs e)
        {
            this.Hide();
            POI_CALC = Stake_ModifyStaking.modifyStaking(POI_CALC);
            this.Show();
        }

        private void cmdStakeAll_Click(object sender, EventArgs e)
        {
            this.Hide();
            Stake_Calc.stakePoints();
            this.Show();
        }

        private void cmdStakeSta_Click(object sender, EventArgs e)
        {
            this.Hide();

            fStaRange = Forms.Stake_Forms.sForms.fStaRange;
            Application.ShowModelessDialog(Application.MainWindow.Handle, fStaRange, false);
        }

        private void cmdStakeSingle_Click(object sender, EventArgs e)
        {
        }

        private void cmdFlipSideMove_Click(object sender, EventArgs e)
        {
            this.Hide();

            Stake_FlipPoints.flipPointsMove(false);

            this.Show();
        }

        private void cmdFlipSideAdd_Click(object sender, EventArgs e)
        {
            this.Hide();

            Stake_FlipPoints.flipPointsMove(true);

            this.Show();
        }

        private void cmdAddPointsToAlign_Click(object sender, EventArgs e)
        {
            this.Hide();
            fAddPoint = Forms.Stake_Forms.sForms.fAddPoint;
            fAddPoint.Show();
        }

        private void cmdExportPoints_ClicK(object sender, EventArgs e)
        {
            this.Hide();
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(null, sFrms.sForms.fExport, false);
        }

        private void cmdPrintCollection_Click(object sender, EventArgs e)
        {
            int i = 0;
            foreach (AlgnData aData in algnData)
            {
                System.Diagnostics.Debug.Print(string.Format("{0}: {1} {2} {3} {4} {5}", i, aData.AlignName, aData.AlignLayer, aData.AlignHandle, aData.AlignID, aData.TableHandle));
                i++;
            }
        }

        private void cmdChangeAlignStartPoint_Click(object sender, EventArgs e)
        {
            this.Hide();
            Stake_Algn.changeAlignStartPoint();
            this.Show();
        }

        private void cmdPrintDictionary_Click(object sender, EventArgs e)
        {
            Stake_Dict.printDictionary();
        }

        #endregion "Commands"

        #region "Form"

        public void frmStake_Load()
        {
            bool exists = false;
            ObjectId idDict = Dict.getNamedDictionary("STAKE_PNTS", out exists);
            this.Location = new System.Drawing.Point(0, 0);
            if (exists)
            {
                Stake_Misc.updateSTAKE_PNTS(idDict);
            }

            xRef.fixXrefs();

            Stake_XRefStatus.reloadXRefs();
            Stake_GetSurfaceCPNT.getSurfaceFromXRef("CPNT-ON", "GCAL");
            BlockReference br = xRef.getXRefBlockReference("CNTL");
            if(br == null){
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("CNTL drawing not found.  Exiting...");
                return;
            }


            CNTL_LAYER = br.Layer;

            Stake_Dict.updateAlignData();

            optBLDG.Checked = false;

            cboOffset.Items.Add(0);
            cboOffset.Items.Add(3);
            cboOffset.Items.Add(5);
            cboOffset.Items.Add(10);
            cboOffset.Items.Add(15);

            cboOffset.SelectedIndex = 0;

            cboInterval.Items.Add(10);
            cboInterval.Items.Add(20);
            cboInterval.Items.Add(25);
            cboInterval.Items.Add(50);
            cboInterval.Items.Add(100);

            cboInterval.SelectedIndex = 3;

            cboHeight.Items.Add(0);
            cboHeight.Items.Add(6);
            cboHeight.Items.Add(8);

            cboHeight.SelectedIndex = 1;

            cboTolerance.Items.Add(0);
            cboTolerance.Items.Add(5);

            cboTolerance.SelectedIndex = 1;

            cboDelta.Items.Add(2);
            cboDelta.Items.Add(3);
            cboDelta.Items.Add(4);
            cboDelta.Items.Add(8);

            cboDelta.SelectedIndex = 2;

            this.Text = "STAKE - Active Alignment is: NONE";

            try
            {
                if (Environment.UserName == "john")
                {
                    this.Height = 550;
                }
                else
                {
                    this.Height = 500;
                }
            }
            catch (System.Exception)
            {
            }
            //CgPnt_Group.checkPntGroup("SPNT");
            //Layer.manageLayers("SPNT-LABEL");
        }

        #endregion "Form"

        private void frmStake_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}