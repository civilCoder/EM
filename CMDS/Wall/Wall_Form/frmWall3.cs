using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using wa1 = Wall.Wall_Alignment1;
using wd = Wall.Wall_Design;
using wdp = Wall.Wall_DesignProfile;
using wlc = Wall.Wall_LocateCurb;

namespace Wall.Wall_Form
{
    public partial class frmWall3 : Form
    {
        public frmWall3()
        {
            InitializeComponent();
            btnSelectBrklineEx.Enabled = false;
            btn_UpdateCurb.Enabled = false;

        }

        private Alignment oAlign;
        public Alignment ACTIVEALIGN
        {
            get { return oAlign; }
            set
            {
                oAlign = value;
                ToolStripStatusLabel1.Text = "ACTIVE ALIGNMENT: " + oAlign.Name.ToString();
            }
        }
        public int Side { get; set; }
        public Alignment AlignPL  { get; set; }
        public Alignment AlignRF { get; set; }
        public string BRKLINE_DESIGN_HANDLE { get; set; }
        public string BRKLINE_EXIST_HANDLE { get; set; }
        public List<PNT_DATA> PNTSDESIGN { get; set; }
        public List<PNT_DATA> PNTSEXIST { get; set; }
        public List<double> Stations { get; set; }
        public List<AlgnEntData> ALGNENTDATA { get; set; }

        private void btn_LocateCurb_Click(object sender, System.EventArgs e)
        {
            Alignment objAlignPL = default(Alignment);
            Alignment objAlignRF = default(Alignment);
            ObjectId idAlignPL = ObjectId.Null;
            ObjectId idAlignRF = ObjectId.Null;
            //EXIST SURFACE PROFILE

            if (chkUseExistSurface.Checked == true)
            {
                idAlignPL = wdp.CreateProfileBySurface("EXIST", ACTIVEALIGN.ObjectId, 0.0);


            }
            else
            {
                idAlignPL = wdp.CreateProfileByLayout2("EXIST", ACTIVEALIGN, PNTSEXIST);

            }

            AlignPL = objAlignPL;

            //DESIGN SURFACE PROFILE

            ObjectId idPoly3d = BRKLINE_DESIGN_HANDLE.stringToHandle().getObjectId();

            string strNameAlign = ACTIVEALIGN.Name.ToString() + "-REF";
            string strLayer = ACTIVEALIGN.Name.ToString() + "-REF";

            ObjectId idLayer = Layer.manageLayers(strLayer);
            Layer.manageLayers(strNameAlign);


            try
            {
                idAlignRF = wa1.Create_Align_Profile_By3dPoly2a(ACTIVEALIGN, "CPNT", strNameAlign, idLayer, idPoly3d);
                AlignRF = objAlignRF;

                Stations = wd.getPNT_DATA_FINAL(objAlignPL, objAlignRF);
                Side = wa1.getSide(objAlignPL, objAlignRF);

                if (wlc.LocateCurb("CPNT", AlignPL, AlignRF) == true)
                {
                    btn_UpdateCurb.Enabled = true;
                }


            }
            catch (Autodesk.AutoCAD.Runtime.Exception)
            {
                MessageBox.Show("btn_LocateCurb");
                return;

            }

        }


        private void btn_UpdateCurb_Click(object sender, EventArgs e)
        {
            using (BaseObjs._acadDoc.LockDocument())
            {
                using (Transaction TR = BaseObjs.startTransactionDb())
                {

                    Surf.removeSurface("TEMP");
                    TypedValue[] TVs = new TypedValue[2];

                    TVs[0] = new TypedValue((int)DxfCode.Start, "POLYLINE");
                    TVs[1] = new TypedValue((int)DxfCode.LayerName, "CPNT-BRKLINE-TEMP");

                    Misc.deleteObjs(TVs);

                    TR.Commit();

                }
            }

            wlc.LocateCurb("CPNT", AlignPL, AlignRF);

        }


        private void btnSelectWallAlign_Click(System.Object sender, System.EventArgs e)
        {
            Alignment objAlign = default(Alignment);

            string strPrompt = null;
            strPrompt = "Select Boundary Alignment: ";

            try
            {
                Point3d pnt3d = Point3d.Origin;
                objAlign = (Alignment)Base_Tools45.Select.selectEntity(typeof(Alignment), strPrompt, "Alignment Selection failed.", out pnt3d);
                if ((objAlign == null))
                {
                    MessageBox.Show("Alignment Selection Failed");
                    return;
                }

                ACTIVEALIGN = objAlign;


            }
            catch (Autodesk.AutoCAD.Runtime.Exception )
            {
                MessageBox.Show("Alignment Selection Failed");
                return;

            }

            if (objAlign.StartingStation == 0)
            {
                MessageBox.Show("Starting Station of Boundary Alignment needs to be greater than 0: recommend using 1+00 or 10+00 as starting station.");
            }

            this.ToolStripStatusLabel1.Text = "ACTIVE ALIGN: " + objAlign.Name;

        }


        private void btnSelectBrklineDes_Click(System.Object sender, System.EventArgs e)
        {
            Polyline3d idPoly3d = default(Polyline3d);

            string strPrompt = null;

            strPrompt = "Select 3dPolyline Design Reference:";

            try
            {
                Point3d pnt3d = Point3d.Origin;
                idPoly3d = (Polyline3d)Base_Tools45.Select.selectEntity(typeof(Polyline3d), strPrompt, "3dPolyline Selection failed.", out pnt3d);


            }
            catch (Autodesk.AutoCAD.Runtime.Exception )
            {
                MessageBox.Show("Selection of Design Breakline failed. Exiting......");
                return;

            }

            BRKLINE_DESIGN_HANDLE = idPoly3d.Handle.ToString();

        }


        private void btnSelectBrklineEx_Click(System.Object sender, System.EventArgs e)
        {
            BaseObjs.acadActivate();

            ObjectId idPoly3d = ObjectId.Null;
            string strPrompt = null;

            strPrompt = "Select 3dPolyline Exist Reference:";

            try
            {
                Point3d pnt3d = Point3d.Origin;
                Polyline3d poly3d = (Polyline3d)Base_Tools45.Select.selectEntity(typeof(Polyline3d), strPrompt, "3dPolyline Selection failed.", out pnt3d);
                idPoly3d = poly3d.ObjectId;
            }
            catch (Autodesk.AutoCAD.Runtime.Exception )
            {
                return;

            }

            BRKLINE_EXIST_HANDLE = idPoly3d.Handle.ToString();
            ALGNENTDATA = wd.getAlgnEntData(ACTIVEALIGN);
            List<staOffElev> sOffs = wd.convert3dPolyToPnt_Data(ACTIVEALIGN, ALGNENTDATA, idPoly3d, "EXIST");

        }

        private void chkUseExistSurface_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (chkUseExistSurface.Checked)
            {
                btnSelectBrklineEx.Enabled = false;
            }
            else
            {
                btnSelectBrklineEx.Enabled = true;
            }
        }

        private void chkUseExistSurface_Click(object sender, EventArgs e)
        {

        }

        private void frmWall3_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
