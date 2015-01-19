using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;
using wa1 = Wall.Wall_Alignment1;
using wd = Wall.Wall_Design;
using wdp = Wall.Wall_DesignProfile;
using wlc = Wall.Wall_LocateCurb;

namespace Wall.Wall_Form
{
    public partial class frmWall4 : Form
    {
        public frmWall4()
        {
            InitializeComponent();
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
        public Alignment AlignPL { get; set; }
        public Alignment AlignRF { get; set; }
        public string BRKLINE_DESIGN_HANDLE { get; set; }
        public string BRKLINE_EXIST_HANDLE { get; set; }
        public List<PNT_DATA> PNTSDESIGN { get; set; }
        public List<PNT_DATA> PNTSEXIST { get; set; }
        public List<double> Stations { get; set; }
        public bool is3d { get; set; }
        public List<AlgnEntData> ALGNENTDATA { get; set; }

        private void btnSelectBoundaryRef_Click(object sender, EventArgs e)
        {
            string strPrompt = "Select Boundary Reference (2D or 3D): ";
            Alignment objAlignPL = null;
            ObjectId idAlignPL = ObjectId.Null;

            try
            {
                Point3d pnt3d = Point3d.Origin;

                List<Type> tps = new List<Type>();
                tps.Add(typeof(Line));
                tps.Add(typeof(Polyline));
                tps.Add(typeof(Polyline3d));

                Entity ent = Base_Tools45.Select.selectEntity(tps, strPrompt, "Reference Object Selection failed - try again.", out pnt3d);

                if ((ent == null))
                {
                    MessageBox.Show("Reference Object Selection Failed");
                    return;
                }

                if ((ent) is Line)
                {
                    Line lin = ent as Line;
                    List<Point3d> pnts3d = new List<Point3d>();
                    pnts3d.Add(lin.StartPoint);
                    pnts3d.Add(lin.EndPoint);

                    ObjectId idPoly = Draw.addPoly(pnts3d);
                    objAlignPL = Align.addAlignmentFromPoly("WALL-PL", "WALL", idPoly, "Thienes_Proposed", "Thienes_Proposed", true);
                    if (objAlignPL == null)
                    {
                        MessageBox.Show("Alignment not created");
                        return;
                    }

                    idAlignPL = wdp.CreateProfileBySurface("EXIST", idAlignPL, 0.0);

                }
                else if ((ent) is Polyline)
                {
                    Polyline poly = ent as Polyline;

                    objAlignPL = Align.addAlignmentFromPoly("WALL-PL", "WALL", poly.ObjectId, "Thienes_Proposed", "Thienes_Proposed", false);
                    if (idAlignPL.IsNull)
                    {
                        MessageBox.Show("Alignment not created");
                        return;
                    }
                    idAlignPL = wdp.CreateProfileBySurface("EXIST", idAlignPL, 0.0);

                }
                else if (ent is Polyline3d)
                {
                    Polyline3d poly3d = (Polyline3d)ent;
                    ObjectId idPoly = Conv.poly3d_Poly(poly3d.ObjectId, "0");
                    objAlignPL = Align.addAlignmentFromPoly("WALL-PL", "WALL", idPoly, "Thienes_Proposed", "Thienes_Proposed", false);
                    if (objAlignPL == null)
                    {
                        MessageBox.Show("Alignment not created");
                        return;
                    }

                    BRKLINE_EXIST_HANDLE = poly3d.Handle.ToString();
                    ALGNENTDATA = wd.getAlgnEntData(objAlignPL);

                    List<staOffElev> sOffs = wd.convert3dPolyToPnt_Data(objAlignPL, ALGNENTDATA, poly3d.ObjectId, "EXIST");
                    PNTSEXIST = wd.sOffsToPNT_DATA(sOffs);
                    idAlignPL = wdp.CreateProfileByLayout("EXIST", objAlignPL, PNTSEXIST);

                }

                ACTIVEALIGN = objAlignPL;
                AlignPL = objAlignPL;


            }
            catch (Autodesk.AutoCAD.Runtime.Exception )
            {
                return;

            }

            this.ToolStripStatusLabel1.Text = "ACTIVE ALIGN: " + objAlignPL.Name;
        }

        private void btnSelectBrklineDes_Click(object sender, EventArgs e)
        {
            ObjectId idAlignRF = ObjectId.Null;
            Polyline3d poly3d = default(Polyline3d);

            string strPrompt = "Select 3dPolyline Design Reference:";

            try
            {
                Point3d pnt3d = Point3d.Origin;
                poly3d = (Polyline3d)Base_Tools45.Select.selectEntity(typeof(Polyline3d), strPrompt, "3dPolyline Selection failed.", out pnt3d);


            }
            catch (Autodesk.AutoCAD.Runtime.Exception )
            {
                MessageBox.Show("Selection of Design Breakline failed. Exiting......");
                return;

            }

            BRKLINE_DESIGN_HANDLE = poly3d.Handle.ToString();

            string strNameAlign = ACTIVEALIGN.Name.ToString() + "-REF";
            ObjectId idLayer = Layer.manageLayers(strNameAlign);

            idAlignRF = wa1.Create_Align_Profile_By3dPoly2a(ACTIVEALIGN, "CPNT", strNameAlign, idLayer, poly3d.ObjectId);
            AlignRF = (Alignment)idAlignRF.getEnt();
        }

        private void btn_LocateCurb_Click(object sender, EventArgs e)
        {
            Alignment objAlignPL = AlignPL;
            Alignment objAlignRF = AlignRF;
            try
            {
                Stations = wd.getPNT_DATA_FINAL(objAlignPL, objAlignRF);
                Side = wa1.getSide(objAlignPL, objAlignRF);

                if (wlc.LocateCurb4("CPNT", AlignPL, AlignRF) == true)
                {
                    btn_UpdateCurb.Enabled = true;
                }


            }
            catch (Autodesk.AutoCAD.Runtime.Exception )
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

        private void frmWall4_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
