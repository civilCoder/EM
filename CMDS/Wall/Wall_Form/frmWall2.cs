using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using wd = Wall.Wall_Design;
using wdl = Wall.Wall_DesignLimits;
using wdp = Wall.Wall_DesignProfile;
using wpv = Wall.Wall_ProfileView;

namespace Wall.Wall_Form
{
    public partial class frmWall2 : Form
    {
        private PromptStatus ps;
        private Point3d pnt3dPicked;
        private ErrorProvider errorProvider1;

        private Alignment oAlign;

        public frmWall2()
        {
            InitializeComponent();
            frmWall2_Initialize();
            errorProvider1 = new ErrorProvider();
            errorProvider1.BlinkStyle = ErrorBlinkStyle.BlinkIfDifferentError;
            errorProvider1.BlinkRate = 500;

        }
        #region "Properties"

        public Alignment ACTIVEALIGN
        {
            get
            {
                return oAlign;
            }
            set
            {
                oAlign = value;
                if ((value == null))
                {
                    ToolStripStatusLabel1.Text = string.Format("ACTIVE ALIGNMENT: {0}", "Nothing");
                }
                else
                {
                    ToolStripStatusLabel1.Text = string.Format("ACTIVE ALIGNMENT: {0}", oAlign.Name);
                }
            }
        }

        public ProfileView ACTIVEPROFILEVIEW { get; set; }

        public string BRKLINE_DESIGN_HANDLE { get; set; }

        public string BRKLINE_EXIST_HANDLE { get; set; }

        public List<PNT_DATA> PNTSDESIGN { get; set; }

        public List<PNT_DATA> PNTSEXIST { get; set; }

        public MText LABEL { get; set; }

        public Polyline3d BrkLEFT { get; set; }

        public Polyline3d BrkRIGHT { get; set; }

        public List<AlgnEntData> ALGNENTDATA { get; set; }

        #endregion

        #region "Commands"

        private void btnSelectWallAlign_Click(object sender, EventArgs e)
        {
            Alignment objAlign = null;
            try
            {
                Point3d pnt3d = Point3d.Origin;
                objAlign = (Alignment)Base_Tools45.Select.selectEntity(typeof(Alignment), "Select Wall Alignment", "", out pnt3dPicked, out ps);

                if ((objAlign == null))
                {
                    ACTIVEALIGN = null;
                    return;
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception)
            {
            }
            //-----------------------------------------------Collect alignment segment data-----------------------------------------------

            ALGNENTDATA = Wall_Design.getAlgnEntData(objAlign);

            //-----------------------------------------------Collect alignment segment data-----------------------------------------------
            
            ACTIVEALIGN = objAlign;

        }

        private void btnSelectBrkline1_Click(object sender, EventArgs e)
        {
            Point3d pnt3d = Point3d.Origin;
            BrkLEFT = (Polyline3d)Base_Tools45.Select.selectEntity(typeof(Polyline3d), "Select Left Breakline:", "Left Breakline Selection failed.", out pnt3d, out ps);
            if (ps != PromptStatus.OK)
                return;
        }

        private void btnSelectBrkline2_Click(object sender, EventArgs e)
        {
            Point3d pnt3d = Point3d.Origin;
            BrkRIGHT = (Polyline3d)Base_Tools45.Select.selectEntity(typeof(Polyline3d), "Select Right Breakline:", "Right Breakline Selection failed.", out pnt3d, out ps);
            if (ps != PromptStatus.OK)
                return;
        }

        private void btnCreateWallProfileView_Click(object sender, EventArgs e)
        {
            BaseObjs.acadActivate();

            this.ACTIVEPROFILEVIEW = wpv.CreateWallProfileView(ACTIVEALIGN, ALGNENTDATA, BrkLEFT, BrkRIGHT, "WDE");
        }

        private void btnUpdateLeaders_Click(object sender, EventArgs e)
        {
            wdl.adjustLeaderInsPnt();
        }

        private void btn_BuildWallProfiles_Click(object sender, EventArgs e)
        {
            BaseObjs.acadActivate();
            wdl.wallDesignLimits(ACTIVEPROFILEVIEW, "WDE");
        }

        private string selectBrkLine(string strSide, ProfileView objProfileView)
        {
            Polyline3d poly3d = null;

            string strHandle = "";

            List<int> intXDataType = new List<int>(1);
            List<object> varXDataValIn = new List<object>(1);

            try
            {
                Point3d pnt3d = Point3d.Origin;
                poly3d = (Polyline3d)Base_Tools45.Select.selectEntity(typeof(Polyline3d), string.Format("Select Breakline on {0} Side of Wall Alignment: ", strSide), "Breakline Selection Failed.", out pnt3d, out ps);

                if ((poly3d != null))
                {
                    TypedValue[] tvs = new TypedValue[2] {
                        new TypedValue(1001, string.Format("BRK{0}", strSide)),
                        new TypedValue(1005, poly3d.Handle),
                    };
                    poly3d.ObjectId.setXData(tvs, string.Format("BRK{0}", strSide));
                    strHandle = poly3d.Handle.ToString();
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception)
            {
                strHandle = "";
            }

            return strHandle;
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {
            ProfileView objProfileView = null;
            TypedValue[] tvs;
            try
            {
                BaseObjs.acadActivate();
                Point3d pnt3d = Point3d.Origin;
                objProfileView = (ProfileView)Base_Tools45.Select.selectEntity(typeof(ProfileView), "Select Profile View: ", "Profile View Selection failed", out pnt3d, out ps);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception)
            {
                return;
            }

            string strHandleRIGHT = "";
            try
            {
                ResultBuffer rb = objProfileView.ObjectId.getXData("BRKRIGHT");
                tvs = rb.AsArray();
                strHandleRIGHT = tvs[1].Value.ToString();
            }
            catch (Autodesk.AutoCAD.Runtime.Exception)
            {
            }

            string strHandleLEFT = "";
            try
            {
                ResultBuffer rb = objProfileView.ObjectId.getXData("BRKLEFT");
                tvs = rb.AsArray();
                strHandleLEFT = tvs[1].Value.ToString();
            }
            catch (Autodesk.AutoCAD.Runtime.Exception)
            {
            }

            Polyline3d Poly3dRIGHT = null;
            bool boolUpdateRIGHT = false;
            try
            {
                Poly3dRIGHT = (Polyline3d)Base_Tools45.Db.handleToObject(strHandleRIGHT);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception)
            {
                if ((Poly3dRIGHT == null))
                {
                    DialogResult dr = MessageBox.Show("The Breakline for the RIGHT side has been deleted.\n Would you like to select a new Design Reference Breakline for the RIGHT Side?",
                        "",
                        MessageBoxButtons.YesNo);
                    switch (dr)
                    {
                        case DialogResult.Yes:

                            try
                            {
                                strHandleRIGHT = selectBrkLine("RIGHT", objProfileView);

                                if (strHandleRIGHT != string.Empty)
                                {
                                    Poly3dRIGHT = (Polyline3d)Base_Tools45.Db.handleToObject(strHandleRIGHT);
                                    boolUpdateRIGHT = true;
                                }
                                else
                                {
                                    DialogResult dr2 = MessageBox.Show("Selection Failed. Try again?", "", MessageBoxButtons.YesNo);

                                    switch (dr2)
                                    {
                                        case DialogResult.Yes:

                                            strHandleRIGHT = selectBrkLine("RIGHT", objProfileView);
                                            if (strHandleRIGHT != string.Empty)
                                            {
                                                Poly3dRIGHT = (Polyline3d)Base_Tools45.Db.handleToObject(strHandleRIGHT);
                                                boolUpdateRIGHT = true;
                                            }
                                            else
                                            {
                                                return;
                                            }

                                            break;
                                        case DialogResult.No:

                                            return;
                                    }
                                }
                            }
                            catch (Autodesk.AutoCAD.Runtime.Exception)
                            {
                            }

                            break;
                        case DialogResult.No:

                            return;
                    }
                }
            }

            Polyline3d Poly3dLEFT = null;
            bool boolUpdateLEFT = false;
            try
            {
                Poly3dLEFT = (Polyline3d)Base_Tools45.Db.handleToObject(strHandleLEFT);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception)
            {
                if (Poly3dLEFT == null)
                {
                    DialogResult dr = MessageBox.Show("The Breakline for the LEFT side has been deleted.\n Would you like to select a new Design Reference Breakline for the LEFT Side?",
                        "",
                        MessageBoxButtons.YesNo);

                    switch (dr)
                    {
                        case DialogResult.Yes:

                            try
                            {
                                strHandleLEFT = selectBrkLine("LEFT", objProfileView);

                                if (strHandleLEFT != string.Empty)
                                {
                                    Poly3dLEFT = (Polyline3d)Base_Tools45.Db.handleToObject(strHandleLEFT);
                                    boolUpdateLEFT = true;
                                }
                                else
                                {
                                    DialogResult dr2 = MessageBox.Show("Selection Failed. Try again?", "", MessageBoxButtons.YesNo);

                                    switch (dr2)
                                    {
                                        case DialogResult.Yes:

                                            strHandleRIGHT = selectBrkLine("RIGHT", objProfileView);
                                            if (strHandleRIGHT != string.Empty)
                                            {
                                                Poly3dLEFT = (Polyline3d)Base_Tools45.Db.handleToObject(strHandleLEFT);
                                                boolUpdateLEFT = true;
                                            }
                                            else
                                            {
                                                return;
                                            }

                                            break;
                                        case DialogResult.No:

                                            break;
                                    }
                                }
                            }
                            catch (Autodesk.AutoCAD.Runtime.Exception)
                            {
                            }

                            break;
                        case DialogResult.No:

                            return;
                    }
                }
            }

            BRKLINE_DESIGN_HANDLE = strHandleRIGHT;
            BRKLINE_EXIST_HANDLE = strHandleLEFT;

            Alignment objAlign = null;
            using (Transaction TR = BaseObjs.startTransactionDb())
            {
                try
                {
                    objAlign = (Alignment)TR.GetObject(objProfileView.AlignmentId, OpenMode.ForRead);
                    ACTIVEALIGN = objAlign;
                }
                catch (Autodesk.AutoCAD.Runtime.Exception)
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Original Alignment is not available. Exiting.....");
                    return;
                }
            }

            List<staOffElev> sOffs = wd.convert3dPolyToPnt_Data(objAlign, ALGNENTDATA, Poly3dRIGHT.ObjectId, "DESIGN");
            PNTSDESIGN = new List<PNT_DATA>();
            
            foreach(staOffElev s in sOffs){                                                      //convert data to work with existing methods.
                PNT_DATA p = new PNT_DATA() { STA = s.staAlign, z = s.elev };
                PNTSDESIGN.Add(p);
            }
            wdp.CreateProfileByLayout2("BRKRIGHT", objAlign, PNTSDESIGN);
            if (boolUpdateRIGHT)
            {
                tvs = new TypedValue[4] {
                    new TypedValue(1001, "BRKRIGHT"),
                    new TypedValue(1005, Poly3dRIGHT.Handle),
                    new TypedValue(1040, PNTSDESIGN[0].STA),
                    new TypedValue(1040, PNTSDESIGN[PNTSDESIGN.Count - 1].STA)
                };

                objProfileView.ObjectId.setXData(tvs, "BRKRIGHT");
            }

            sOffs = wd.convert3dPolyToPnt_Data(ACTIVEALIGN, ALGNENTDATA, Poly3dLEFT.ObjectId, "EXIST");
            PNTSEXIST = new List<PNT_DATA>();
            foreach(staOffElev s in sOffs){
                PNT_DATA p = new PNT_DATA(){STA = s.staAlign, z = s.elev};
                PNTSEXIST.Add(p);
            }

            wdp.CreateProfileByLayout2("BRKLEFT", objAlign, PNTSEXIST);
            if (boolUpdateLEFT)
            {
                tvs = new TypedValue[4] {
                    new TypedValue(1001, "BRKLEFT"),
                    new TypedValue(1005, Poly3dLEFT.Handle),
                    new TypedValue(1040, PNTSEXIST[0].STA),
                    new TypedValue(1040, PNTSEXIST[PNTSEXIST.Count - 1].STA)
                };

                objProfileView.ObjectId.setXData(tvs, "BRKLEFT");
            }

            wdl.wallDesignLimits(objProfileView, "WDE");
        }

        private void cbx_HeightAboveGround_Click(object sender, EventArgs e)
        {
            if (cbx_HeightAboveGround.Checked == true)
            {
                this.lblFreeBoardHeight.Text = "SCREEN WALL HEIGHT ABOVE GROUND";
                this.lblFreeBoardTolerance.Text = "SCREEN WALL HEIGHT TOLERANCE +/-";
                this.cbx_HeightAboveFooting.Checked = false;
            }

            if (this.cbx_HeightAboveFooting.Checked == false & this.cbx_HeightAboveGround.Checked == false)
            {
                this.lblFreeBoardHeight.Text = "TOP OF WALL FREEBOARD";
                this.lblFreeBoardTolerance.Text = "FREEBOARD TOLERANCE +/-";
            }
        }

        private void cbx_HeightAboveFooting_Click(object sender, EventArgs e)
        {
            if (cbx_HeightAboveFooting.Checked == true)
            {
                this.lblFreeBoardHeight.Text = "SCREEN WALL HEIGHT ABOVE FOOTING";
                this.lblFreeBoardTolerance.Text = "N/A";
                this.cbx_HeightAboveGround.Checked = false;
            }

            if (this.cbx_HeightAboveFooting.Checked == false & this.cbx_HeightAboveGround.Checked == false)
            {
                this.lblFreeBoardHeight.Text = "TOP OF WALL FREEBOARD";
                this.lblFreeBoardTolerance.Text = "FREEBOARD TOLERANCE +/-";
            }
        }

        private void opt_PANEL_Click(object sender, EventArgs e)
        {
            if (opt_PANEL.Checked)
            {
                tbx_SpaceV.Enabled = false;

                lblSpaceUnitsH.Text = "(FT.)";
                lblStepUnitsV.Text = "(IN.)";

                tbx_SpaceH.Text = Convert.ToString(25);
                tbx_StepH.Text = Convert.ToString(25);
                tbx_StepV.Text = Convert.ToString(6);
            }
        }

        private void opt_BLOCK_Click(object sender, EventArgs e)
        {
            if (opt_BLOCK.Checked)
            {
                tbx_SpaceV.Enabled = true;
                tbx_StepH.Enabled = true;
                tbx_StepV.Enabled = true;

                lblSpaceUnitsH.Text = "(IN.)";
                lblSpaceUnitsV.Text = "(IN.)";
                lblStepUnitsH.Text = "(FT.)";
                lblStepUnitsV.Text = "(IN.)";

                tbx_SpaceH.Text = Convert.ToString(16);
                tbx_SpaceV.Text = Convert.ToString(8);
                tbx_StepH.Text = Convert.ToString(8);
                tbx_StepV.Text = tbx_SpaceV.Text;
            }
        }

        #endregion

        #region "Form"

        private void tbx_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string errorMsg = "";
            TextBox tbx = (TextBox)sender;
            if (!validNumber(tbx.Text, out errorMsg))
            {
                e.Cancel = true;
                tbx.Select(0, tbx.Text.Length);

                this.errorProvider1.SetError(tbx, errorMsg);
            }
        }

        private void tbx_Validated(object sender, System.EventArgs e)
        {
            errorProvider1.SetError((TextBox)sender, "");
            errorProvider1.Clear();
        }

        private bool validNumber(string strValue, out string errorMessage)
        {
            decimal result;
            errorMessage = "";
            if (!strValue.isDecimal(out result))
            {
                errorMessage = "Entry is not a valid number";
                return false;
            }
            else
            {
                return true;
            }
        }

        private void frmWall2_Load(object sender, System.EventArgs e)
        {
        }

        private void
        frmWall2_Initialize(){
            TextBox tbx = default(TextBox);
            Control cntl = default(Control);

            this.Width = 600;
            //Me.Width = 210

            for (int i = 0; i < gbx_WallMaterial.Controls.Count; i++)
            {
                cntl = gbx_WallMaterial.Controls[i];
                if (cntl.GetType().ToString() == "System.Windows.Forms.TextBox")
                {
                    tbx = (TextBox)cntl;
                    tbx.Validating += tbx_Validating;
                    tbx.Validated += tbx_Validated;
                }
            }

            for (int i = 0; i < gbx_MinCover_Freeboard.Controls.Count; i++)
            {
                cntl = gbx_MinCover_Freeboard.Controls[i];
                if (cntl.GetType().ToString() == "System.Windows.Forms.TextBox")
                {
                    tbx = (TextBox)cntl;
                    tbx.Validating += tbx_Validating;
                    tbx.Validated += tbx_Validated;
                }
            }

            tbx_SpaceH.Text = Convert.ToString(16);
            tbx_SpaceV.Text = Convert.ToString(8);
            tbx_StepH.Text = Convert.ToString(8);
            tbx_StepV.Text = Convert.ToString(0.5);

            tbx_Cover.Text = Convert.ToString(1.25);
            tbx_Freeboard.Text = Convert.ToString(1.0);
            tbx_FreeboardTolerance.Text = Convert.ToString(0.1);

            opt_BLOCK.Checked = true;

            ObjectId idStyle = Prof_Style.getProfileLabelSetStyle("WALL");          
        }

        #endregion

        private void frmWall2_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }


    }
}
