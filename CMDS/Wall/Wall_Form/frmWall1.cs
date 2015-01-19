using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Pen = System.Drawing.Pen;
using wa0 = Wall.Wall_Alignment0;
using wa1 = Wall.Wall_Alignment1;
using wa2 = Wall.Wall_Alignment2;
using wa2d = Wall.Wall_Alignment2d;
using wd = Wall.Wall_Design;
using wdl = Wall.Wall_DesignLimits;
using wdp = Wall.Wall_DesignProfile;
using wdpv = Wall.Wall_DesignProfileView;
using wlc = Wall.Wall_LocateCurb;
using wne = Wall.Wall_NestedEnts;
using ws = Wall.Wall_Sections;
using wsa = Wall.Wall_SectionAnalysis;
using wsu = Wall.Wall_SectionUpdate;
using wu = Wall.Wall_Utility;

namespace Wall.Wall_Form {
    /// <summary>
    /// Interaction logic for frmWall1.xaml
    /// </summary>
    public partial class frmWall1 : System.Windows.Forms.Form {
        public frmWall1() {
            InitializeComponent();
            ErrorProvider1 = new ErrorProvider();
            ErrorProvider1.BlinkStyle = ErrorBlinkStyle.BlinkIfDifferentError;
            ErrorProvider1.BlinkRate = 500;
        }

        //****************************       Declares          ***********************************

        #region "Declares"

        private ErrorProvider ErrorProvider1;

        private System.Drawing.Font font2a = new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 8);
        private Pen penBlack2 = new Pen(System.Drawing.Color.Black, 2);
        private Pen penPL4 = new Pen(System.Drawing.Color.Black, 2);
        private float[] sngDashValues = {
            40,
            2,
            4,
            2,
            4,
            2
        };
        private Pen penDashed2 = new Pen(System.Drawing.Color.Black, 2);

        private Pen penDashed1 = new Pen(System.Drawing.Color.Gray, 1);

        private Autodesk.AutoCAD.Windows.Window win = Autodesk.AutoCAD.ApplicationServices.Core.Application.MainWindow;
        
        #endregion
        
        #region "Properties"
        
        private TinSurface oSurfaceEXIST;
        
        public TinSurface SurfaceEXIST {
            get {
                try {
                    bool exists = false;
                    oSurfaceEXIST = Surf.getTinSurface("EXIST", out exists);
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex) {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }
                return oSurfaceEXIST;
            }
        }
        
        private Alignment oAlign;
        
        public Alignment ACTIVEALIGN {
            get {
                return oAlign;
            }
            set {
                oAlign = value;
                ToolStripStatusLabel1.Text = "ACTIVE ALIGNMENT: " + oAlign.Name.ToString();
            }
        }
        
        public string SECTIONVIEWMODE { get; set; }
        
        public SECTIONDATASET SDS { get; set; }
        
        public int VIEWSCALE { get; set; }
        
        public int WALLNO { get; set; }
        
        public int SIDE { get; set; }
        
        public Dictionary<string, Dictionary<double, FEATUREDATA>> NOMDATA { get; set; }
        
        public Dictionary<string, Dictionary<double, FEATUREDATA>> WALLDATA { get; set; }
        
        public Dictionary<string, Dictionary<double, FEATUREDATA>> GUTDATA { get; set; }
        
        public bool WallStart { get; set; }
        
        public int CurrentStationIndex { get; set; }
        
        public Alignment AlignPL { get; set; }
        
        public Alignment AlignRF { get; set; }
        
        public double Skew { get; set; }
        
        public List<double> Stations { get; set; }
        
        public List<Point3d> PntsWallCutBeg { get; set; }
        
        public List<Point3d> PntsWallCutEnd { get; set; }
        
        public List<Point3d> PntsWallFillBeg { get; set; }
        
        public List<Point3d> PntsWallFillEnd { get; set; }
        
        public ProfileView ACTIVEPROFILEVIEW { get; set; }
        
        public string BRKLINE_DESIGN_HANDLE { get; set; }
        
        public string BRKLINE_EXIST_HANDLE { get; set; }
        
        public List<PNT_DATA> PNTSDESIGN { get; set; }
        
        public List<PNT_DATA> PNTSEXIST { get; set; }
        
        public MText LABEL { get; set; }
        
        public Polyline3d BrkLEFT { get; set; }
        
        public Polyline3d BrkRIGHT { get; set; }
        
        #endregion
        
        #region "1"
        
        private void
        btn1_SelectAlign_Click(System.Object eventSender, System.EventArgs eventArgs) {
            Alignment objAlign = default(Alignment);
            
            string strPrompt = null;
            
            if (opt1_PL.Checked) {
                strPrompt = "Select Boundary Alignment: ";
            }else {
                strPrompt = "Select On-Site Wall Alignment: ";
            }
            
            try {
                BaseObjs.acadActivate();
                Point3d pnt3d = Point3d.Origin;
                objAlign = (Alignment)Base_Tools45.Select.selectEntity(typeof(Alignment), strPrompt, "Alignment Selection failed", out pnt3d);
                if ((objAlign == null)) {
                    System.Windows.Forms.MessageBox.Show("Alignment Selecton Failed");
                }else {
                    ACTIVEALIGN = objAlign;
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception) {
                System.Windows.Forms.MessageBox.Show("Alignment Selection Failed");
                return;
            }
            
            if (objAlign.StartingStation == 0) {
                System.Windows.Forms.MessageBox.Show("Starting Station of Boundary Alignment needs to be greater than 0: recommend using 1+00 or 10+00 as starting station.");
            }
        }
        
        private void
        opt1_PL_CheckedChanged_1(System.Object sender, System.EventArgs e) {
            if (opt1_PL.Checked == true) {
                btn1_SelectAlign.Text = "SELECT BOUNDARY ALIGNMENT";
                
                opt1_SurfaceExist.Checked = true;
                opt1_3dPolyDesign.Checked = true;
                
                //btn1_CreateWallProfileView.Visible = False
                
                gbx1_Source1.Text = "EXIST SURFACE PROFILE BUILD OPTIONS";
                this.opt1_SurfaceExist.Text = "USE SURFACE 'EXIST'";
                
                gbx1_Source2.Text = "DESIGN SURFACE PROFILE BUILD OPTIONS";
                opt1_SurfaceDesign.Text = "USE SURFACE 'CPNT-ON'";
            }
        }
        
        private void
        opt1_SITE_CheckedChanged_1(System.Object sender, System.EventArgs e) {
            if (opt1_SITE.Checked == true) {
                btn1_SelectAlign.Text = "SELECT WALL ALIGNMENT";
                
                opt1_3dPolyExist.Checked = true;
                opt1_3dPolyDesign.Checked = true;
                
                //btn1_CreateWallProfileView.Visible = True
                
                gbx1_Source1.Text = "DESIGN SURFACE PROFILE BUILD OPTIONS - LEFT";
                opt1_SurfaceExist.Text = "USE SURFACE 'CPNT-ON'";
                
                gbx1_Source2.Visible = true;
                gbx1_Source2.Text = "DESIGN SURFACE PROFILE BUILD OPTIONS - RIGHT";
                opt1_SurfaceDesign.Text = "USE SURFACE 'CPNT-ON'";
            }
        }
        
        private void
        opt1_PointsExist_CheckedChanged(System.Object sender, System.EventArgs e) {
            if (opt1_3dPolyExist.Checked == true) {
                btn1_ExistSelect.Text = "SELECT POINTS";
            }
        }
        
        private void
        opt1_3dPolyExist_CheckedChanged(System.Object sender, System.EventArgs e) {
            if (opt1_3dPolyExist.Checked == true) {
                btn1_ExistSelect.Text = "SELECT 3D POLYLINE";
            }
        }
        
        private void
        btn1_ExistSelect_Click(object sender, System.EventArgs e) {
            BaseObjs.acadActivate();
            
            switch (btn1_ExistSelect.Text) {
                case "SELECT POINTS":
                    
                    wne.selectNestedPoints("EXIST");
                    
                    break;
                case "SELECT 3D POLYLINE":
                    
                    ObjectId idPoly3d = ObjectId.Null;
                    
                    string strPrompt = null;
                    
                    if (opt1_PL.Checked) {
                        strPrompt = "Select 3dPolyline Exist Reference:";
                    }else {
                        strPrompt = "Select Design 3dPolyline - Left Side:";
                    }
                    
                    try {
                        Point3d pnt3d = Point3d.Origin;
                        Polyline3d objPoly3d = (Polyline3d)Base_Tools45.Select.selectEntity(typeof(Polyline3d), strPrompt, "Breakline Selection failed.", out pnt3d);
                        BRKLINE_EXIST_HANDLE = objPoly3d.Handle.ToString();
                        
                        if (opt1_PL.Checked) {
                            List<AlgnEntData> aEntData = wd.getAlgnEntData(ACTIVEALIGN); 						
                            List<staOffElev> sOffs = wd.convert3dPolyToPnt_Data(ACTIVEALIGN, aEntData, idPoly3d, "EXIST");
                        }else if (opt1_SITE.Checked) {
                            BrkLEFT = objPoly3d;
                        }
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception) {
                        return;
                    }
                    
                    break;
                default:
                    
                    return;                   
            }
        }
        
        private void
        opt1_PointsDesign_CheckedChanged(System.Object sender, System.EventArgs e) {
            if (opt1_PointsDesign.Checked == true) {
                btn1_DesignSelect.Text = "SELECT POINTS";
            }
        }
        
        private void            
        opt1_3dPolyDesign_CheckedChanged(System.Object sender, System.EventArgs e) {
            if (opt1_3dPolyDesign.Checked == true) {
                btn1_DesignSelect.Text = "SELECT 3D POLYLINE";
            }
        }
        
        private void
        btn1_DesignSelect_Click(object sender, System.EventArgs e) {
            Point3d pnt3d = Point3d.Origin;
            
            win.Focus();
            
            switch (btn1_DesignSelect.Text) {
                case "SELECT POINTS":
                    
                    wd.getPoints("DESIGN");
                    
                    break;
                case "SELECT 3D POLYLINE":
                    
                    ObjectId idPoly3d = ObjectId.Null;
                    
                    string strPrompt = null;
                    
                    if (opt1_PL.Checked) {
                        strPrompt = "Select 3dPolyline Design Reference:";
                    }else {
                        strPrompt = "Select Design 3dPolyline - Right Side:";
                    }
                    
                    //Try
                    
                    //    idPoly3d = Base_Tools45.Select.selectEntity(GetType(Polyline3d), strPrompt, "Breakline Selection failed.", pnt3d)
                    
                    //Catch ex As Autodesk.AutoCAD.Runtime.Exception
                    
                    //    MsgBox("Selection of nested object failed. Exiting......")
                    //    Exit Sub
                    
                    //End Try
                    
                    //If (idPoly3d Is Nothing) Then
                    
                    //    MsgBox("Selection of nested object failed. Exiting......")
                    //    Exit Sub
                    
                    //End If
                    
                    try {
                        try {
                            idPoly3d = Base_Tools45.Select.selectEntity(typeof(Polyline3d), strPrompt, "Breakline Selection failed.", out pnt3d).ObjectId;
                            
                            if (opt1_PL.Checked) {
                                BRKLINE_DESIGN_HANDLE = idPoly3d.Handle.ToString();
                            }else if (opt1_SITE.Checked) {
                                BrkRIGHT = (Polyline3d)idPoly3d.getEnt();
                            }
                        }
                        catch (Autodesk.AutoCAD.Runtime.Exception ) {
                            System.Windows.Forms.MessageBox.Show("Object selected was not a 3d Polyline");
                            return;
                        }
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception ) {
                        System.Windows.Forms.MessageBox.Show("Object selected was not a 3d Polyline");
                    }
                    
                    break;
                default:
                    
                    return;
            }
        }
        
        //Private Sub btn2b_CreateWallProfileView_Click(sender As System.Object, e As System.EventArgs) Handles btn2b_CreateProfileView.Click
        
        //    If Not (BrkLEFT Is Nothing) Then
        //        If Not (BrkRIGHT Is Nothing) Then
        
        //            win.focus()
        //            ACTIVEPROFILEVIEW = CreateWallProfileView(ACTIVEALIGN, BrkLEFT, BrkRIGHT, "WDX", Me)
        
        //        Else
        //            MsgBox("Right Breakline not Selected. Exiting....")
        //            Exit Sub
        //        End If
        //    Else
        //        MsgBox("Left Breakline not Selected. Exiting....")
        //        Exit Sub
        //    End If
        
        //End Sub
        
        #endregion
        
        #region "2a"
        
        private void
        btn2a_LocateCurb_Click(object sender, System.EventArgs e) {
            Alignment objAlignPL = default(Alignment);
            Alignment objAlignREF = default(Alignment);
            ObjectId idAlignPL = ObjectId.Null;
            ObjectId idAlignRF = ObjectId.Null;
            //EXIST SURFACE PROFILE
            
            if (opt1_SurfaceExist.Checked == true) {
                idAlignPL = wdp.CreateProfileBySurface("EXIST", ACTIVEALIGN.ObjectId, 0.0);
            }else {
                idAlignPL = wdp.CreateProfileByLayout("EXIST", ACTIVEALIGN, PNTSEXIST);
            }
            
            //DESIGN SURFACE PROFILE
            
            if (opt1_SurfaceDesign.Checked == true) {
                idAlignRF = wdp.CreateProfileBySurface("CPNT", ACTIVEALIGN.ObjectId, Convert.ToDouble(tbx1_Offset.Text));
                wa2.CreateProfileByDesign2b(ACTIVEALIGN, objAlignREF);
            }else if (opt1_PointsDesign.Checked == true) {
                wdp.CreateProfileByDesign("CPNT", ACTIVEALIGN);
            }else if (opt1_3dPolyDesign.Checked == true) {
                ObjectId idPoly3d = BRKLINE_DESIGN_HANDLE.stringToHandle().getObjectId();
                
                string strNameAlign = string.Format("{0}-REF", ACTIVEALIGN.Name);
                string strLayer = strNameAlign;
                
                ObjectId idLayer = Layer.manageLayers(strLayer);
                Layer.manageLayers(strNameAlign);
                
                try {
                    idAlignRF = wa1.Create_Align_Profile_By3dPoly2a(ACTIVEALIGN, "CPNT", strNameAlign, idLayer, idPoly3d);
                    wlc.LocateCurb("CPNT", objAlignPL, objAlignREF);
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ) {
                    System.Windows.Forms.MessageBox.Show("Error in LocateCurb");
                    return;
                }
            }
        }
        
        private void
        UpdateSection_2a() {
            penDashed2.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            penPL4.DashPattern = sngDashValues;
            
            double dXscale = 1;
            double dYscale = 1;
            
            PointF pnt0 = default(PointF);
            PointF pnt1 = default(PointF);
            PointF pnt2 = default(PointF);
            PointF pnt3 = default(PointF);
            PointF pnt4 = default(PointF);
            PointF pnt5 = default(PointF);
            double p0x = 0;
            double p0y = 0;
            double p1x = 0;
            double p1y = 0;
            double p2x = 0;
            double p2y = 0;
            double p3x = 0;
            double p3y = 0;
            double p4x = 0;
            double p4y = 0;
            double p5x = 0;
            double p5y = 0;
            double dx1 = 0;
            double dy1 = 0;
            double dx2 = 0;
            double dy2 = 0;
            double dx3 = 0;
            double dy3 = 0;
            double dx4 = 0;
            double dy4 = 0;
            double dx5 = 0;
            double dy5 = 0;
            
            double dblDX = 0;
            double dblDY = 0;
            
            p0x = 15;
            p0y = 510;
            
            dx1 = Convert.ToDouble(tbx_X0.Text) * dXscale;
            dy1 = dx1 * Convert.ToDouble(tbx_S0.Text) * dYscale;
            
            p1x = p0x + dx1;
            p1y = p0y - dy1;
            
            dx2 = 0;
            dy2 = (Convert.ToDouble(tbx_CF.Text.ToString()) / 12) * dYscale;
            
            p2x = p1x + dx2;
            p2y = p1y - dy2;
            
            dx3 = Convert.ToDouble(tbx_B1.Text) * dXscale;
            dy3 = dx3 * Convert.ToDouble(tbx_S1.Text) * dYscale;
            
            p3x = p2x + dx3;
            p3y = p2y - dy3;
            
            dx4 = 20 * dXscale;
            dy4 = dx4 * Convert.ToDouble(tbx_SG.Text) * dYscale;
            
            p4x = p3x + dx4;
            p4y = p3y - dy4;
            
            dx5 = Convert.ToDouble(tbx_B2.Text) * dXscale;
            dy5 = dx5 * Convert.ToDouble(tbx_S2.Text) * dYscale;
            
            p5x = p4x + dx5;
            p5y = p4y - dy5;
            
            dblDX = p5x - p0x;
            dblDY = p5y - p0y;
            
            if (dblDY > 0) {
                p0y = 20;
            }else {
                dblDY = dblDY * -1;
            }
            
            if (dblDX / 500 > dblDY / 500) {
                //DX controls
                dXscale = (int)(500 / dblDX);
                dYscale = dXscale;
                p0y = 250;
            }else {
                //DY controls
                dYscale = (int)(500 / dblDY);
                dXscale = dYscale;
            }
            
            p1x = p0x + dx1 * dXscale;
            p1y = p0y - dy1 * dYscale;
            p2x = p1x + dx2 * dXscale;
            p2y = p1y - dy2 * dYscale;
            p3x = p2x + dx3 * dXscale;
            p3y = p2y - dy3 * dYscale;
            //UPSLOPE
            p4x = p3x + dx4 * dXscale;
            p4y = p3y - dy4 * dYscale;
            p5x = p4x + dx5 * dXscale;
            p5y = p4y - dy5 * dYscale;
            
            pnt0.X = (float)p0x;
            pnt0.Y = (float)p0y;
            pnt1.X = (float)p1x;
            pnt1.Y = (float)p1y;
            pnt2.X = (float)p2x;
            pnt2.Y = (float)p2y;
            pnt3.X = (float)p3x;
            pnt3.Y = (float)p3y;
            pnt4.X = (float)p4x;
            pnt4.Y = (float)p4y;
            pnt5.X = (float)p5x;
            pnt5.Y = (float)p5y;
            
            PointF[] ptsA = new PointF[6] {
                pnt0,
                pnt1,
                pnt2,
                pnt3,
                pnt4,
                pnt5
            };
            
            Graphics g = PictureBox2d.CreateGraphics();
            PictureBox2d.Refresh();
            
            g.DrawLines(penBlack2, ptsA);
            
            //---------------------------------------------------------------------------------------------------------------------------
            
            wsu.labelX0S0("B2", "S2", g, font2a, pnt4, pnt5);
            
            PointF pntCF = default(PointF);
            PointF pntPL = default(PointF);
            PointF pntDR = default(PointF);
            
            wsu.labelSLOPE("S", g, font2a, pnt3, pnt4, true);
            
            //DOWN SLOPE
            p4x = p3x + dx4 * dXscale;
            p4y = p3y + dy4 * dYscale;
            p5x = p4x + dx5 * dXscale;
            p5y = p4y - dy5 * dYscale;
            
            pnt4.X = (float)p4x;
            pnt4.Y = (float)p4y;
            pnt5.X = (float)p5x;
            pnt5.Y = (float)p5y;
            
            PointF[] pntsB = new PointF[3] {
                pnt3,
                pnt4,
                pnt5
            };
            g.DrawLines(penDashed2, pntsB);
            
            //DESIGN REF
            
            PointF pnt6 = default(PointF);
            PointF pnt7 = default(PointF);
            pnt6.X = pnt0.X;
            pnt6.Y = 50;
            pnt7.X = pnt0.X;
            pnt7.Y = 450;
            
            PointF[] pntsC = new PointF[2] {
                pnt6,
                pnt7
            };
            g.DrawLines(penDashed2, pntsC);
            
            //PROPERTY LINE
            
            pnt6.X = pnt5.X;
            pnt6.Y = 50;
            pnt7.X = pnt5.X;
            pnt7.Y = 450;
            pntsC = new PointF[2] {
                pnt6,
                pnt7
            };
            g.DrawLines(penPL4, pntsC);
            
            //LABELS
            
            if (dx1 != 0) {
                wsu.labelX0S0("XO", "S0", g, font2a, pnt0, pnt1);
            }
            
            wsu.labelX0S0("B1", "S1", g, font2a, pnt2, pnt3);
            wsu.labelX0S0("B2", "S2", g, font2a, pnt4, pnt5);
            
            //CF
            
            pntCF.X = pnt1.X + (pnt2.X - pnt1.X) / 2 - 15;
            pntCF.Y = pnt1.Y - (pnt2.Y - pnt1.Y) / 2 - 15;
            g.DrawString("CF", font2a, System.Drawing.Brushes.Red, pntCF);
            
            //S
            wsu.labelSLOPE("S", g, font2a, pnt3, pnt4, false);
            
            //PL
            pntPL.X = pnt5.X - 5;
            pntPL.Y = 35;
            g.DrawString("P", font2a, System.Drawing.Brushes.Red, pntPL);
            pntPL.X = pnt5.X - 3;
            pntPL.Y = 38;
            g.DrawString("L", font2a, System.Drawing.Brushes.Red, pntPL);
            
            //DESIGN REF
            pntDR.X = pnt0.X + 5;
            pntDR.Y = pntPL.Y;
            g.DrawString("REF", font2a, System.Drawing.Brushes.Red, pntDR);
            
            g.Dispose();
        }
        
        private void
        tbx_LostFocus(object sender, System.EventArgs e) {
            UpdateSection_2a();
        }
        
        #endregion
        
        #region "2c"
        
        private void btn2c_CreateProfileView_Click(object sender, System.EventArgs e) {
            Alignment objAlignPL = default(Alignment);
            ObjectId idAlignPL = ObjectId.Null;
            
            if (opt1_SurfaceExist.Checked == true) {
                idAlignPL = wdp.CreateProfileBySurface("EXIST", ACTIVEALIGN.ObjectId, 0.0);
            }else {
                wdp.CreateProfileByLayout("EXIST", ACTIVEALIGN, PNTSDESIGN);
            }
            
            if (opt1_SurfaceDesign.Checked == true) {
                idAlignPL = wdp.CreateProfileBySurface("CPNT", ACTIVEALIGN.ObjectId, Convert.ToDouble(tbx1_Offset.Text));
                wdp.CreateProfileByDesign2c(ACTIVEALIGN, objAlignPL, PNTSDESIGN);
            }else if (opt1_PointsDesign.Checked == true) {
                wdp.CreateProfileByDesign2c(ACTIVEALIGN, objAlignPL, PNTSDESIGN);
            }else if (opt1_3dPolyDesign.Checked == true) {
                ObjectId idPoly3d = BRKLINE_DESIGN_HANDLE.stringToHandle().getObjectId();
                
                string strNameAlign = string.Format("{0}-REF", ACTIVEALIGN.Name);
                string strLayer = strNameAlign;
                
                ObjectId idLayer = Layer.manageLayers(strLayer);
                Layer.manageLayers(strNameAlign);
                
                try {
                    idAlignPL = wa2.Create_Align_Profile_By3dPoly2b2c(ACTIVEALIGN, "CPNT", strNameAlign, idLayer, idPoly3d);
                    wdp.CreateProfileByDesign2c(ACTIVEALIGN, objAlignPL, PNTSDESIGN);
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ) {
                    System.Windows.Forms.MessageBox.Show("LWPoly to alignment not implemented YET.");
                    return;
                }
            }
            
            wdpv.CreateProfileViewPrelim(ACTIVEALIGN);
        }
        
        private void btn2c_DisplaySections_Click(object sender, System.EventArgs e) {
            ws.drawSections();
        }
        
        private void UpdateSection_2c() {
            penDashed2.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            penPL4.DashPattern = sngDashValues;
            
            double dXscale = 1;
            double dYscale = 1;
            
            PointF pnt0 = default(PointF);
            PointF pnt1 = default(PointF);
            PointF pnt2 = default(PointF);
            PointF pnt3 = default(PointF);
            PointF pnt4 = default(PointF);
            PointF pnt5 = default(PointF);
            PointF pnt6 = default(PointF);
            PointF pnt7 = default(PointF);
            PointF pnt8 = default(PointF);
            PointF pnt9 = default(PointF);
            PointF pnt10 = default(PointF);
            PointF pnt11 = default(PointF);
            double p0x = 0;
            double p0y = 0;
            double p1x = 0;
            double p1y = 0;
            double p2x = 0;
            double p2y = 0;
            double p3x = 0;
            double p3y = 0;
            double p4x = 0;
            double p4y = 0;
            double p5x = 0;
            double p5y = 0;
            double p6x = 0;
            double p6y = 0;
            double p7x = 0;
            double p7y = 0;
            double p8x = 0;
            double p8y = 0;
            double p9x = 0;
            double p9y = 0;
            double p10x = 0;
            double p10y = 0;
            double p11x = 0;
            double p11y = 0;
            double dx1 = 0;
            double dy1 = 0;
            double dx2 = 0;
            double dy2 = 0;
            double dx3 = 0;
            double dy3 = 0;
            double dx4 = 0;
            double dy4 = 0;
            double dx5 = 0;
            double dy5 = 0;
            double dx6 = 0;
            double dy6 = 0;
            double dx7 = 0;
            double dy7 = 0;
            double dx8 = 0;
            double dy8 = 0;
            double dx9 = 0;
            double dy9 = 0;
            double dx10 = 0;
            double dy10 = 0;
            double dx11 = 0;
            double dy11 = 0;
            
            double dYh = 4.0;
            
            double dblDX = 0;
            double dblDY = 0;
            
            p0x = 15;
            p0y = 510;
            
            dx1 = Convert.ToDouble(tbx2c_X0.Text) * dXscale;
            dy1 = dx1 * Convert.ToDouble(tbx2c_S0.Text) * dYscale;
            
            p1x = p0x + dx1;
            p1y = p0y - dy1;
            
            dx2 = 0.0;
            dy2 = (Convert.ToDouble(tbx2c_CF.Text.ToString()) / 12) * dYscale;
            
            p2x = p1x + dx2;
            p2y = p1y - dy2;
            
            dx3 = Convert.ToDouble(tbx2c_B1.Text) * dXscale;
            dy3 = dx3 * Convert.ToDouble(tbx2c_S1.Text) * dYscale;
            
            p3x = p2x + dx3;
            p3y = p2y - dy3;
            
            dx4 = 0.0;
            dy4 = dYh * dYscale;
            
            p4x = p3x + dx4;
            p4y = p3y - dy4;
            
            dx5 = Convert.ToDouble(tbx2c_WT.Text) / 12 * dXscale;
            dy5 = 0.0;
            
            p5x = p4x + dx5;
            p5y = p4y - dy5;
            
            dx6 = 0;
            dy6 = -1 * Convert.ToDouble(tbx2c_FB.Text) / 12 * dYscale;
            
            p6x = p5x + dx6;
            p6y = p5y - dy6;
            
            dx7 = Convert.ToDouble(tbx2c_GW.Text) / 2 * dXscale;
            dy7 = -2 / 12 * dYscale;
            
            p7x = p6x + dx7;
            p7y = p6y - dy7;
            
            dx8 = Convert.ToDouble(tbx2c_GW.Text) / 2 * dXscale;
            dy8 = 2 / 12 * dYscale;
            
            p8x = p7x + dx8;
            p8y = p7y - dy8;
            
            dx9 = Convert.ToDouble(tbx2c_B2.Text) * dXscale;
            dy9 = dx9 * Convert.ToDouble(tbx2c_S2.Text) * dYscale;
            
            p9x = p8x + dx9;
            p9y = p8y - dy9;
            
            dx10 = 10.0 * dXscale;
            dy10 = dx10 * Convert.ToDouble(tbx2c_SG.Text) * dYscale;
            
            p10x = p9x + dx10;
            p10y = p9y - dy10;
            
            dx11 = Convert.ToDouble(tbx2c_B3.Text) * dXscale;
            dy11 = dx11 * Convert.ToDouble(tbx2c_S3.Text) * dYscale;
            
            p11x = p10x + dx11;
            p11y = p9y - dy11;
            
            dblDX = p11x - p0x;
            dblDY = p11y - p0y;
            
            if (dblDY > 0) {
                p0y = 20;
            }else {
                dblDY = dblDY * -1;
            }
            
            if (dblDX / 500 > dblDY / 500) {
                //DX controls
                dXscale = (int)(500 / dblDX);
                dYscale = dXscale;
                p0y = 250;
            }else {
                //DY controls
                dYscale = (int)(500 / dblDY);
                dXscale = dYscale;
            }
            
            Graphics g = PictureBox2d.CreateGraphics();
            PictureBox2d.Refresh();
            
            p1x = p0x + dx1 * dXscale;
            p1y = p0y - dy1 * dYscale;
            p2x = p1x + dx2 * dXscale;
            p2y = p1y - dy2 * dYscale;
            p3x = p2x + dx3 * dXscale;
            p3y = p2y - dy3 * dYscale;
            p4x = p3x + dx4 * dXscale;
            p4y = p3y - dy4 * dYscale;
            p5x = p4x + dx5 * dXscale;
            p5y = p4y - dy5 * dYscale;
            p6x = p5x + dx6 * dXscale;
            p6y = p5y - dy6 * dYscale;
            p7x = p6x + dx7 * dXscale;
            p7y = p6y - dy7 * dYscale;
            p8x = p7x + dx8 * dXscale;
            p8y = p7y - dy8 * dYscale;
            p9x = p8x + dx9 * dXscale;
            p9y = p8y - dy9 * dYscale;
            p10x = p9x + dx10 * dXscale;
            p10y = p9y - dy10 * dYscale;
            p11x = p10x + dx11 * dXscale;
            p11y = p10y - dy11 * dYscale;
            
            //DRAW UP TO WALL FACE
            
            pnt0.X = (float)p0x;
            pnt0.Y = (float)p0y;
            pnt1.X = (float)p1x;
            pnt1.Y = (float)p1y;
            pnt2.X = (float)p2x;
            pnt2.Y = (float)p2y;
            pnt3.X = (float)p3x;
            pnt3.Y = (float)p3y;
            
            PointF[] ptsA = new PointF[4] {
                pnt0,
                pnt1,
                pnt2,
                pnt3
            };
            g.DrawLines(penBlack2, ptsA);
            
            pnt4.X = (float)p4x;
            pnt4.Y = (float)p4y;
            pnt5.X = (float)p5x;
            pnt5.Y = (float)p5y;
            
            //DRAW FROM BACK OF WALL TO PL
            
            pnt6.X = (float)p6x;
            pnt6.Y = (float)p6y;
            pnt7.X = (float)p7x;
            pnt7.Y = (float)p7y;
            pnt8.X = (float)p8x;
            pnt8.Y = (float)p8y;
            pnt9.X = (float)p9x;
            pnt9.Y = (float)p9y;
            pnt10.X = (float)p10x;
            pnt10.Y = (float)p10y;
            pnt11.X = (float)p11x;
            pnt11.Y = (float)p11y;
            
            PointF[] ptsC = new PointF[6] {
                pnt6,
                pnt7,
                pnt8,
                pnt9,
                pnt10,
                pnt11
            };
            g.DrawLines(penBlack2, ptsC);
            
            //DRAW WALL
            
            PointF pntW7 = default(PointF);
            PointF pntW8 = default(PointF);
            PointF pntW9 = default(PointF);
            PointF pntW10 = default(PointF);
            PointF pntW11 = default(PointF);
            PointF pntW12 = default(PointF);
            
            pntW7.X = pnt6.X;
            pntW7.Y = pnt6.Y + (float)((dYh + dy6 + 1) * dYscale);
            pntW8.X = pntW7.X + (float)(2 * dXscale);
            pntW8.Y = pntW7.Y;
            pntW9.X = pntW8.X;
            pntW9.Y = pntW8.Y + (float)(1.33 * dYscale);
            pntW10.X = pntW9.X - (float)(5 * dXscale);
            pntW10.Y = pntW9.Y;
            pntW11.X = pntW10.X;
            pntW11.Y = pntW7.Y;
            pntW12.X = pnt3.X;
            pntW12.Y = pntW7.Y;
            
            PointF[] ptsW = new PointF[11] {
                pnt3,
                pnt4,
                pnt5,
                pnt6,
                pntW7,
                pntW8,
                pntW9,
                pntW10,
                pntW11,
                pntW12,
                pnt3
            };
            g.FillPolygon(System.Drawing.Brushes.Gray, ptsW);
            
            //---------------------------------------------------------------------------------------------------------------------------
            
            PointF pntCF = default(PointF);
            PointF pntPL = default(PointF);
            PointF pntDR = default(PointF);
            
            //DESIGN REF
            
            PointF pntR1 = default(PointF);
            PointF pntR2 = default(PointF);
            pntR1.X = pnt0.X;
            pntR1.Y = 50;
            pntR2.X = pnt0.X;
            pntR2.Y = 450;
            
            PointF[] pntsC = new PointF[2] {
                pntR1,
                pntR2
            };
            g.DrawLines(penDashed2, pntsC);
            
            //PROPERTY LINE
            
            pntR1.X = pnt11.X;
            pntR1.Y = 50;
            pntR2.X = pnt11.X;
            pntR2.Y = 450;
            pntsC = new PointF[2] {
                pntR1,
                pntR2
            };
            g.DrawLines(penPL4, pntsC);
            
            //LABELS
            
            if (dx1 != 0) {
                wsu.labelX0S0("XO", "S0", g, font2a, pnt0, pnt1);
            }
            
            wsu.labelX0S0("B1", "S1", g, font2a, pnt2, pnt3);
            wsu.labelX0S0("B2", "S2", g, font2a, pnt8, pnt9);
            wsu.labelX0S0("B3", "S3", g, font2a, pnt10, pnt11);
            
            //CF
            
            pntCF.X = pnt1.X + (pnt2.X - pnt1.X) / 2 - 15;
            pntCF.Y = pnt1.Y - (pnt2.Y - pnt1.Y) / 2 - 15;
            g.DrawString("CF", font2a, System.Drawing.Brushes.Red, pntCF);
            
            wsu.labelMISC("WT", g, font2a, pnt4, pnt5, -10, -25);
            wsu.labelMISC("GW", g, font2a, pnt6, pnt8, -10, -5);
            
            //S
            wsu.labelSLOPE("S", g, font2a, pnt9, pnt10, true);
            
            //PL
            pntPL.X = pnt11.X - 5;
            pntPL.Y = 35;
            g.DrawString("P", font2a, System.Drawing.Brushes.Red, pntPL);
            pntPL.X = pnt11.X - 3;
            pntPL.Y = 38;
            g.DrawString("L", font2a, System.Drawing.Brushes.Red, pntPL);
            
            //DESIGN REF
            pntDR.X = pnt0.X + 5;
            pntDR.Y = pntPL.Y;
            g.DrawString("REF", font2a, System.Drawing.Brushes.Red, pntDR);
            
            g.Dispose();
        }
        
        private void tbx2c_LostFocus(object sender, System.EventArgs e) {
            UpdateSection_2c();
        }
        
        #endregion
        
        #region "2b"
        
        private void btn2b_CreateProfileView_Click(object sender, System.EventArgs e) {
            Alignment objAlign = default(Alignment);
            ObjectId idAlign = ObjectId.Null;
            
            if (opt1_SurfaceExist.Checked == true) {
                idAlign = wdp.CreateProfileBySurface("EXIST", ACTIVEALIGN.ObjectId, 0.0);
            }else {
                wdp.CreateProfileByLayout("EXIST", ACTIVEALIGN, PNTSDESIGN);
            }
            
            if (opt1_SurfaceDesign.Checked == true) {
                idAlign = wdp.CreateProfileBySurface("CPNT", ACTIVEALIGN.ObjectId, Convert.ToDouble(tbx1_Offset.Text));
                wa2.CreateProfileByDesign2b(ACTIVEALIGN, objAlign);
            }else if (opt1_PointsDesign.Checked == true) {
                wdp.CreateProfileByDesign("CPNT", ACTIVEALIGN);
            }else if (opt1_3dPolyDesign.Checked == true) {
                ObjectId idPoly3d = BRKLINE_DESIGN_HANDLE.stringToHandle().getObjectId();
                
                string strNameAlign = string.Format("{0}-REF", ACTIVEALIGN.Name);
                string strLayer = strNameAlign;
                
                ObjectId idLayer = Layer.manageLayers(strLayer);
                Layer.manageLayers(strNameAlign);
                
                try {
                    idAlign = wa2.Create_Align_Profile_By3dPoly2b2c(ACTIVEALIGN, "CPNT", strNameAlign, idLayer, idPoly3d);
                    wa2.CreateProfileByDesign2b(ACTIVEALIGN, objAlign);
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ) {
                    System.Windows.Forms.MessageBox.Show("Error in Region 2b");
                    return;
                }
            }
            
            wdpv.CreateProfileViewPrelim(ACTIVEALIGN);
        }
        
        private void btn2b_DisplaySections_Click(object sender, System.EventArgs e) {
            ws.drawSections();
        }
        
        private void UpdateSection_2b() {
            penDashed2.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            penPL4.DashPattern = sngDashValues;
            
            double dXscale = 1;
            double dYscale = 1;
            
            PointF pnt0 = default(PointF);
            PointF pnt1 = default(PointF);
            PointF pnt2 = default(PointF);
            PointF pnt3 = default(PointF);
            PointF pnt4 = default(PointF);
            PointF pnt5 = default(PointF);
            double p0x = 0;
            double p0y = 0;
            double p1x = 0;
            double p1y = 0;
            double p2x = 0;
            double p2y = 0;
            double p3x = 0;
            double p3y = 0;
            double p4x = 0;
            double p4y = 0;
            double p5x = 0;
            double p5y = 0;
            double dx1 = 0;
            double dy1 = 0;
            double dx2 = 0;
            double dy2 = 0;
            double dx3 = 0;
            double dy3 = 0;
            double dx4 = 0;
            double dy4 = 0;
            double dx5 = 0;
            double dy5 = 0;
            
            double dblDX = 0;
            double dblDY = 0;
            
            p0x = 15;
            p0y = 510;
            
            dx1 = Convert.ToDouble(tbx2b_X0.Text) * dXscale;
            dy1 = dx1 * Convert.ToDouble(tbx2b_S0.Text) * dYscale;
            
            p1x = p0x + dx1;
            p1y = p0y - dy1;
            
            dx2 = 0;
            dy2 = (Convert.ToDouble(tbx2b_CF.Text.ToString()) / 12) * dYscale;
            
            p2x = p1x + dx2;
            p2y = p1y - dy2;
            
            dx3 = Convert.ToDouble(tbx2b_B1.Text) * dXscale;
            dy3 = dx3 * Convert.ToDouble(tbx2b_S1.Text) * dYscale;
            
            p3x = p2x + dx3;
            p3y = p2y - dy3;
            
            dx4 = 10 * dXscale;
            dy4 = dx4 * Convert.ToDouble(tbx2b_SG.Text) * dYscale;
            
            p4x = p3x + dx4;
            p4y = p3y - dy4;
            
            dx5 = Convert.ToDouble(tbx2b_B2.Text) * dXscale;
            dy5 = dx5 * Convert.ToDouble(tbx2b_S2.Text) * dYscale;
            
            p5x = p4x + dx5;
            p5y = p4y - dy5;
            
            dblDX = p5x - p0x;
            dblDY = p5y - p0y;
            
            dXscale = 800 / dblDX;
            dYscale = 600 / dblDY;
            
            if (dXscale > dYscale) {
                //DX controls
                dXscale = (int)(dXscale * 0.8);
                dYscale = dXscale;
            }else {
                //DY controls
                dYscale = (int)(dYscale * 0.8);
                dXscale = dYscale;
            }
            
            if (dblDY > 0) {
                p0y = 350;
            }else {
                p0y = 250;
            }
            
            p1x = p0x + dx1 * dXscale;
            p1y = p0y - dy1 * dYscale;
            p2x = p1x + dx2 * dXscale;
            p2y = p1y - dy2 * dYscale;
            p3x = p2x + dx3 * dXscale;
            p3y = p2y - dy3 * dYscale;
            //UPSLOPE
            p4x = p3x + dx4 * dXscale;
            p4y = p3y - dy4 * dYscale;
            p5x = p4x + dx5 * dXscale;
            p5y = p4y - dy5 * dYscale;
            
            pnt0.X = (float)p0x;
            pnt0.Y = (float)p0y;
            pnt1.X = (float)p1x;
            pnt1.Y = (float)p1y;
            pnt2.X = (float)p2x;
            pnt2.Y = (float)p2y;
            pnt3.X = (float)p3x;
            pnt3.Y = (float)p3y;
            pnt4.X = (float)p4x;
            pnt4.Y = (float)p4y;
            pnt5.X = (float)p5x;
            pnt5.Y = (float)p5y;
            
            PointF[] ptsA = new PointF[6] {
                pnt0,
                pnt1,
                pnt2,
                pnt3,
                pnt4,
                pnt5
            };
            
            Graphics g = PictureBox2d.CreateGraphics();
            
            Pen penBlack = new Pen(System.Drawing.Color.Black, 2);
            PictureBox2d.Refresh();
            
            g.DrawLines(penBlack, ptsA);
            
            //---------------------------------------------------------------------------------------------------------------------------
            
            wsu.labelX0S0("B2", "S2", g, font2a, pnt4, pnt5);
            
            PointF pntCF = default(PointF);
            PointF pntPL = default(PointF);
            PointF pntDR = default(PointF);
            
            //LABEL UPSLOPE
            wsu.labelSLOPE("S", g, font2a, pnt3, pnt4, true);
            
            //DOWN SLOPE
            p4x = p3x + dx4 * dXscale;
            p4y = p3y + dy4 * dYscale;
            p5x = p4x + dx5 * dXscale;
            p5y = p4y - dy5 * dYscale;
            
            pnt4.X = (float)p4x;
            pnt4.Y = (float)p4y;
            pnt5.X = (float)p5x;
            pnt5.Y = (float)p5y;
            
            PointF[] pntsB = new PointF[3] {
                pnt3,
                pnt4,
                pnt5
            };
            g.DrawLines(penDashed2, pntsB);
            
            //DESIGN REF
            
            PointF pnt6 = default(PointF);
            PointF pnt7 = default(PointF);
            pnt6.X = pnt0.X;
            pnt6.Y = 50;
            pnt7.X = pnt0.X;
            pnt7.Y = 450;
            
            PointF[] pntsC = new PointF[2] {
                pnt6,
                pnt7
            };
            g.DrawLines(penDashed2, pntsC);
            
            //PROPERTY LINE
            
            pnt6.X = pnt5.X;
            pnt6.Y = 50;
            pnt7.X = pnt5.X;
            pnt7.Y = 450;
            pntsC = new PointF[2] {
                pnt6,
                pnt7
            };
            g.DrawLines(penPL4, pntsC);
            
            //LABELS
            
            if (dx1 != 0) {
                wsu.labelX0S0("XO", "S0", g, font2a, pnt0, pnt1);
            }
            
            wsu.labelX0S0("B1", "S1", g, font2a, pnt2, pnt3);
            wsu.labelX0S0("B2", "S2", g, font2a, pnt4, pnt5);
            
            //CF
            
            pntCF.X = pnt1.X + (pnt2.X - pnt1.X) / 2 - 15;
            pntCF.Y = pnt1.Y - (pnt2.Y - pnt1.Y) / 2 - 15;
            g.DrawString("CF", font2a, System.Drawing.Brushes.Red, pntCF);
            
            //S
            wsu.labelSLOPE("S", g, font2a, pnt3, pnt4, false);
            
            //PL
            pntPL.X = pnt5.X - 5;
            pntPL.Y = 35;
            g.DrawString("P", font2a, System.Drawing.Brushes.Red, pntPL);
            pntPL.X = pnt5.X - 3;
            pntPL.Y = 38;
            g.DrawString("L", font2a, System.Drawing.Brushes.Red, pntPL);
            
            //DESIGN REF
            pntDR.X = pnt0.X + 5;
            pntDR.Y = pntPL.Y;
            g.DrawString("REF", font2a, System.Drawing.Brushes.Red, pntDR);
            
            g.Dispose();
        }
        
        private void tbx2b_LostFocus(object sender, System.EventArgs e) {
            UpdateSection_2b();
        }
        
        #endregion
        
        #region "2d"
        
        private void
        btn2d_CreateProfileView_Click(object sender, System.EventArgs e) {
            Alignment objAlignPL = default(Alignment);
            Alignment objAlignRF = default(Alignment);
            
            ObjectId idAlignPL = ObjectId.Null;
            ObjectId idAlignRF = ObjectId.Null;
            
            if (opt1_SurfaceExist.Checked == true) {
                idAlignPL = wdp.CreateProfileBySurface("EXIST", ACTIVEALIGN.ObjectId, 0.0); //Wall_DesignProfile
            }else {
                wdp.CreateProfileByLayout("EXIST", ACTIVEALIGN, PNTSEXIST);                    //Wall_DesignProfile
            }
            
            if (opt1_SurfaceDesign.Checked == true) {
                idAlignRF = wdp.CreateProfileBySurface("CPNT", ACTIVEALIGN.ObjectId, double.Parse(tbx1_Offset.Text));
                //m_WallDesignProfile
                wdp.CreateProfileByDesign2c(ACTIVEALIGN, objAlignRF, PNTSDESIGN);
                //m_WallDesignProfile
            }else if (opt1_PointsDesign.Checked == true) {
                wdp.CreateProfileByDesign2c(ACTIVEALIGN, objAlignPL, PNTSDESIGN);
                //m_WallDesignProfile
            }else if (opt1_3dPolyDesign.Checked == true) {
                ObjectId idPoly3d = BRKLINE_DESIGN_HANDLE.stringToHandle().getObjectId();
                
                string strNameAlign = string.Format("{0}-REF", ACTIVEALIGN.Name);
                string strLayer = strNameAlign;
                
                ObjectId idLayer = Layer.manageLayers(strLayer);
                Layer.manageLayers(strNameAlign);
                
                try {
                    idAlignRF = wa2.Create_Align_Profile_By3dPoly2b2c(ACTIVEALIGN, "CPNT", strNameAlign, idLayer, idPoly3d);
                    //m_WallAlignment2
                    AlignRF = objAlignRF;
                    AlignPL = ACTIVEALIGN;
                    wa2d.getStationsToSample(ACTIVEALIGN.ObjectId, idAlignRF);
                    //m_WallAlignment2d
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ) {
                    System.Windows.Forms.MessageBox.Show("Error in 2d");
                    return;
                }
            }
            
            WALLNO = -1;
            
            wdpv.CreateProfileViewPrelim(ACTIVEALIGN);
            //m_WallDesignProfileView
        }
        
        private void
        btn2d_DisplaySections_Click(object sender, System.EventArgs e) {
            SECTIONVIEWMODE = "SECTIONS";
            Panel1.Visible = true;
            
            wsu.buildProfileForAlignRFandAlignPL();
            wsu.initializeViewSections();
            
            double dblStationRF = Stations[0];
            
            SECTIONDATASET vSDS = wsa.wallSectionAnalysis(dblStationRF);             //Wall_SectionAnalysis
            
            wsu.UpdateSection_2d(vSDS);
        }
        
        private void
        NumUpDownSection_Click(object sender, System.EventArgs e) {
            SECTIONDATASET vSDS = default(SECTIONDATASET);
            
            if (this.NumUpDownSection.Value < this.NumUpDownSection.Minimum)
                return;
            if (this.NumUpDownSection.Value > this.NumUpDownSection.Maximum)
                return;
            
            tbxStation.Text = (string.Format("{0:###+##.##}", Stations[(int)NumUpDownSection.Value - 1]));
            CurrentStationIndex = (int)NumUpDownSection.Value - 1;
            double dblStationRF = Stations[(int)CurrentStationIndex];
            
            try {
                vSDS = wsa.wallSectionAnalysis(dblStationRF);
                //m_WallSectionAnalysis
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ) {
                System.Windows.Forms.MessageBox.Show("Error in NumUpDownSection_Click.wallSectionAnalysis");
                return;
            }
            
            try {
                wsu.UpdateSection_2d(vSDS, (float)numUpDownScale.Value);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ) {
                System.Windows.Forms.MessageBox.Show("Error in NumUpDownSection_Click.UpdateSection_2d");
                return;
            }
        }
        
        private void
        NumUpDownSection_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e) {
            double dblStationRF = 0;
            SECTIONDATASET vSDS = default(SECTIONDATASET);
            
            if (this.NumUpDownSection.Value < this.NumUpDownSection.Minimum)
                return;
            if (this.NumUpDownSection.Value > this.NumUpDownSection.Maximum)
                return;
            
            if (e.Delta > 0) {
                if (CurrentStationIndex + 1 > Stations.Count - 1) {
                    return;
                }
            }else {
                if (CurrentStationIndex - 1 < 0) {
                    return;
                }
            }
            
            try {
                this.tbxStation.Text = string.Format("{0:###+##.##}", Stations[(int)NumUpDownSection.Value - 1]);
                CurrentStationIndex = (int)NumUpDownSection.Value - 1;
                dblStationRF = Stations[CurrentStationIndex];
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex) {
                System.Windows.Forms.MessageBox.Show("MouseWheele " + ex.ToString());
                return;
            }
            
            try {
                vSDS = wsa.wallSectionAnalysis(dblStationRF);
                //m_WallSectionAnalysis
                wsu.UpdateSection_2d(vSDS, (int)numUpDownScale.Value);
                
                int intLinesScroll = e.Delta * SystemInformation.MouseWheelScrollLines / 120;
                
                if (e.Delta > 0) {
                    NumUpDownSection.Value = NumUpDownSection.Value - intLinesScroll + 1;
                }else {
                    NumUpDownSection.Value = NumUpDownSection.Value - intLinesScroll - 1;
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex) {
                System.Windows.Forms.MessageBox.Show("MouseWheele " + ex.ToString());
                return;
            }
        }
        
        private void
        btnGoToStation_Click(object sender, System.EventArgs e) {
            double dblStationRF = 0;
            SECTIONDATASET vSDS = default(SECTIONDATASET);
            
            dblStationRF = float.Parse(tbxStation.Text);
            if (dblStationRF < Stations[(int)NumUpDownSection.Minimum - 1] | dblStationRF > Stations[(int)NumUpDownSection.Maximum - 1]) {
                return;
            }
            
            vSDS = wsa.wallSectionAnalysis(dblStationRF);
            //m_WallSectionAnalysis
            wsu.UpdateSection_2d(vSDS, (float)numUpDownScale.Value);
        }
        
        private void
        NumUpDownInterval_Click(object sender, System.EventArgs e) {
            if (NumUpDownInterval.Value < NumUpDownInterval.Minimum)
                return;
            if (NumUpDownInterval.Value > NumUpDownInterval.Maximum)
                return;
            
            List<double> dblStations = wd.getPNT_DATA_FINAL(AlignPL, AlignRF);
            SECTIONDATASET vSDS = default(SECTIONDATASET);
            
            NumUpDownSection.Maximum = dblStations.Count;
            
            this.lblTotal.Text = dblStations.Count.ToString();
            
            this.tbxStation.Text = string.Format("{0:###+##.##}", dblStations[(int)NumUpDownInterval.Value - 1]);
            NumUpDownSection.Value = 1;
            
            CurrentStationIndex = (int)NumUpDownSection.Value;
            
            vSDS = wsa.wallSectionAnalysis(Stations[CurrentStationIndex]);  //Wall_SectionAnalysis
            
            wsu.UpdateSection_2d(vSDS, (int)numUpDownScale.Value);
        }
        
        private void
        btn2d_ViewTypicalSection_Click(System.Object sender, System.EventArgs e) {
            SECTIONVIEWMODE = "TYPICAL";
            
            wsu.UpdateSectionTypical_2D();
        }
        
        private void tbx2d_LostFocus(object sender, System.EventArgs e) {
            switch (SECTIONVIEWMODE) {
                case "TYPICAL":
                    wsu.UpdateSectionTypical_2D();
                    break;
                case "SECTIONS":
                    SECTIONDATASET vSDS = wsa.wallSectionAnalysis(Stations[CurrentStationIndex]);
                    wsu.UpdateSection_2d(vSDS, (int)numUpDownScale.Value);
                    break;
            }
        }
        
        private void
        numUpDownScale_Click(object sender, System.EventArgs e) {
        }
        
        #endregion
        
        #region "3"
        
        private void btn3_AlignMake_Click(System.Object sender, System.EventArgs e) {
            Alignment objAlign = ACTIVEALIGN;
            ProfileView objProfileView = default(ProfileView);
            
            try {
                BaseObjs.acadActivate();
                Point3d pnt3d = Point3d.Origin;
                objProfileView = (ProfileView)Base_Tools45.Select.selectEntity(typeof(ProfileView), "Select Boundary Alignment Profile View: ", "Boundary Alignment Profile View Selection failed.", out pnt3d);
                
                ObjectId id = objProfileView.AlignmentId;
                ACTIVEALIGN = Align.getAlignment(id);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ) {
                return;
            }
            
            double dblStaBeg = wu.getStationProfile(objProfileView, "Select starting point for new Wall Alignment");
            double dblStaEnd = wu.getStationProfile(objProfileView, "Select ending point for new Wall Alignment");
            
            try {
                wa0.makeAlignWall(objAlign, objProfileView, dblStaBeg, System.Math.Round(dblStaEnd, 2));
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex) {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(ex.ToString());
            }
        }
        
        private void opt3_Block_Checked(object sender, System.EventArgs e) {
            if (opt3_BLOCK.Checked) {
                tbx3_SpaceV.Enabled = true;
                tbx3_StepH.Enabled = true;
                tbx3_StepV.Enabled = true;
                
                lblSpaceUnitsH.Text = "(IN.)";
                lblSpaceUnitsV.Text = "(IN.)";
                lblStepUnitsH.Text = "(FT.)";
                lblStepUnitsV.Text = "(IN.)";
                
                tbx3_SpaceH.Text = Convert.ToString(16);
                tbx3_SpaceV.Text = Convert.ToString(8);
                tbx3_StepH.Text = Convert.ToString(8);
                tbx3_StepV.Text = tbx3_SpaceV.Text;
            }
        }
        
        private void opt3_PANEL_Checked(object sender, System.EventArgs e) {
            if (opt3_PANEL.Checked) {
                tbx3_SpaceV.Enabled = false;
                
                lblSpaceUnitsH.Text = "(FT.)";
                lblStepUnitsV.Text = "(IN.)";
                
                tbx3_SpaceH.Text = Convert.ToString(25);
                tbx3_StepH.Text = Convert.ToString(25);
                tbx3_StepV.Text = Convert.ToString(6);
            }
        }
        
        private void cbx3_Screen_Checked(System.Object eventSender, System.EventArgs eventArgs) {
            if (cbx3_Screen.Checked == true) {
                this.lblFreeBoardHeight.Text = "SCREEN WALL HEIGHT";
                this.lblFreeBoardTolerance.Text = "SCREEN WALL HEIGHT TOLERANCE +/-";
            }else {
                this.lblFreeBoardHeight.Text = "TOP OF WALL FREEBOARD";
                this.lblFreeBoardTolerance.Text = "FREEBOARD TOLERANCE +/-";
            }
        }
        
        private void btn3_BuildWallProfiles_Click(System.Object eventSender, System.EventArgs eventArgs) {
            BaseObjs.acadActivate();
            wdl.wallDesignLimits(ACTIVEPROFILEVIEW, "WDX");
        }
        
        private void btn3_UPDATE_Click(System.Object sender, System.EventArgs e) {
            Alignment objAlign = default(Alignment);
            ProfileView objProfileView = default(ProfileView);
            
            ObjectId idPoly3dRIGHT = ObjectId.Null;
            ObjectId idPoly3dLEFT = ObjectId.Null;
            
            string strHandleRIGHT = null;
            string strHandleLEFT = null;
            
            try {
                BaseObjs.acadActivate();
                Point3d pnt3d = Point3d.Origin;
                objProfileView = (ProfileView)Base_Tools45.Select.selectEntity(typeof(ProfileView), "Select Profile View: ", "Profile View Selection failed.", out pnt3d);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ) {
                return;
            }
            
            try {
                ResultBuffer rb = objProfileView.GetXDataForApplication("BRKRIGHT");
                if (rb == null)
                    strHandleRIGHT = string.Empty;
                else
                    strHandleRIGHT = rb.AsArray()[1].Value.ToString();
                
                rb = objProfileView.GetXDataForApplication("BRKLEFT");
                if (rb == null)
                    strHandleLEFT = string.Empty;
                else
                    strHandleLEFT = rb.AsArray()[1].Value.ToString();
            }
            catch (Autodesk.AutoCAD.Runtime.Exception) {
                System.Windows.Forms.MessageBox.Show("Profile View contains NO data link to breakline. Exiting......");
                return;
            }
            
            BRKLINE_DESIGN_HANDLE = strHandleRIGHT;
            BRKLINE_EXIST_HANDLE = strHandleLEFT;
            
            try {
                if (strHandleRIGHT != string.Empty) {
                    idPoly3dRIGHT = strHandleRIGHT.stringToHandle().getObjectId();
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ) {
                System.Windows.Forms.MessageBox.Show("Connection to Reference BREAKLINE broken. Exiting......");
                return;
            }
            
            try {
                if (strHandleLEFT != string.Empty) {
                    idPoly3dLEFT = strHandleLEFT.stringToHandle().getObjectId();
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ) {
                System.Windows.Forms.MessageBox.Show("Connection to Reference BREAKLINE broken. Exiting......");
                return;
            }
            
            try {
                ObjectId objAlignId = objProfileView.AlignmentId;
                objAlign = Align.getAlignment(objAlignId);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ) {
                System.Windows.Forms.MessageBox.Show("Original Alignment is not available. Exiting.....");
                return;
            }
            
            List<AlgnEntData> aEntData = wd.getAlgnEntData(objAlign);
            List<staOffElev> sOffs = wd.convert3dPolyToPnt_Data(objAlign, aEntData, idPoly3dRIGHT, "DESIGN");
            PNTSDESIGN = wd.sOffsToPNT_DATA(sOffs);
            wdp.CreateProfileByLayout2("BRKRIGHT", objAlign, PNTSDESIGN);
            
            sOffs = wd.convert3dPolyToPnt_Data(objAlign, aEntData, idPoly3dLEFT, "EXIST");
            PNTSEXIST = wd.sOffsToPNT_DATA(sOffs);
            wdp.CreateProfileByLayout2("BRKLEFT", objAlign, PNTSEXIST);
            
            wdl.wallDesignLimits(objProfileView, "WDX");
            //Call CreateProfileByDesign("CPNT", ACTIVEALIGN)
        }
        
        #endregion
        
        #region "Form"
        
        private void
        tbx_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
            string errorMsg = null;
            TextBox tbx = (TextBox)sender;
            if (!validNumber(tbx.Text, ref errorMsg)) {
                e.Cancel = true;
                tbx.Select(0, tbx.Text.Length);
                this.ErrorProvider1.SetError(tbx, errorMsg);
            }
        }
        
        private void tbx_Validated(object sender, System.EventArgs e) {
            TextBox tbx = (TextBox)sender;
            
            ErrorProvider1.SetError(tbx, "");
            ErrorProvider1.Clear();
            
            switch (tbx.Name.Substring(4, 1)) {
                case "a":
                    UpdateSection_2a();
                    break;
                case "b":
                    UpdateSection_2a();
                    break;
                case "c":
                    UpdateSection_2a();
                    break;
            }
        }
        
        private bool
        validNumber(string strValue, ref string errorMessage) {
            decimal result = 0;
            if (strValue.isDecimal(out result)) {
                errorMessage = "Entry is not a valid number";
                return false;
            }else {
                return true;
            }
        }
        
        private void
        frmWall_Load(object sender, System.EventArgs e) {
            int i = 0;
            TextBox tbx = default(TextBox);
            Control cntl = default(Control);
            
            this.Width = 570;
            
            for (i = 0; i <= gbx2a_Design.Controls.Count - 1; i++) {
                cntl = gbx2a_Design.Controls[i];
                if (cntl.GetType().ToString() == "System.Windows.Forms.TextBox") {
                    tbx = (TextBox)cntl;
                    tbx.Validating += tbx_Validating;
                    tbx.Validated += tbx_Validated;
                }
            }
            
            for (i = 0; i <= gbx2b_Design.Controls.Count - 1; i++) {
                cntl = gbx2b_Design.Controls[i];
                if (cntl.GetType().ToString() == "System.Windows.Forms.TextBox") {
                    tbx = (TextBox)cntl;
                    tbx.Validating += tbx_Validating;
                    tbx.Validated += tbx_Validated;
                }
            }
            
            for (i = 0; i <= gbx2c_Design.Controls.Count - 1; i++) {
                cntl = gbx2c_Design.Controls[i];
                if (cntl.GetType().ToString() == "System.Windows.Forms.TextBox") {
                    tbx = (TextBox)cntl;
                    tbx.Validating += tbx_Validating;
                    tbx.Validated += tbx_Validated;
                }
            }
            
            for (i = 0; i <= gbx2d_Design.Controls.Count - 1; i++) {
                cntl = gbx2d_Design.Controls[i];
                if (cntl.GetType().ToString() == "System.Windows.Forms.TextBox") {
                    tbx = (TextBox)cntl;
                    tbx.Validating += tbx_Validating;
                    tbx.Validated += tbx_Validated;
                }
            }
            
            opt1_SurfaceExist.Checked = true;
            //opt1_SurfaceDesign.Checked = True
            opt1_3dPolyDesign.Checked = true;
            
            tbx3_Cover.Text = Convert.ToString(1.25);
            tbx3_Freeboard.Text = Convert.ToString(1.0);
            tbx3_StepV.Text = Convert.ToString(0.5);
            tbx3_FreeboardTolerance.Text = Convert.ToString(0.1);
            
            opt1_PL.Checked = true;
            opt1_SurfaceExist.Checked = true;
            
            opt3_BLOCK.Checked = true;
            
            Panel1.Visible = false;
            //CONTROL PANEL ABOVE PICTURE BOX
            
            NumUpDownInterval.Minimum = 1;
            NumUpDownInterval.Maximum = 100;
        }
        
        private void
        TabControl1_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e) {
            Graphics g = e.Graphics;
            System.Drawing.Brush _TextBrush = default(System.Drawing.Brush);
            
            // Get the item from the collection.
            TabPage _TabPage = TabControl1.TabPages[e.Index];
            
            // Get the real bounds for the tab rectangle.
            System.Drawing.Rectangle _TabBounds = TabControl1.GetTabRect(e.Index);
            
            if ((e.State == DrawItemState.Selected)) {
                // Draw a different background color, and don't paint a focus rectangle.
                _TextBrush = new SolidBrush(Color.Red);
                g.FillRectangle(Brushes.AntiqueWhite, e.Bounds);
            }else {
                _TextBrush = new System.Drawing.SolidBrush(e.ForeColor);
                e.DrawBackground();
            }
            
            // Use our own font.
            System.Drawing.Font _TabFont = new System.Drawing.Font("Tahoma", 10.0f, FontStyle.Bold, GraphicsUnit.Pixel);
            // Draw string. Center the text.
            StringFormat _StringFlags = new StringFormat();
            _StringFlags.Alignment = StringAlignment.Near;
            _StringFlags.LineAlignment = StringAlignment.Center;
            g.DrawString(_TabPage.Text, _TabFont, _TextBrush, _TabBounds, new StringFormat(_StringFlags));
            
            g.Dispose();
        }
        
        private void
        TabControl1_Selected(object sender, System.Windows.Forms.TabControlEventArgs e) {
            switch (e.TabPage.Name) {
                case "tabCNTL":
                    this.Width = 576;
                    break;
                case "tabDaylight":
                    this.Width = 1386;
                    PictureBox1.Visible = true;
                    PictureBox1.BringToFront();
                    PictureBox2d.Visible = false;
                    UpdateSection_2a();
                    break;
                case "tabWallCheck":
                    this.Width = 1386;
                    PictureBox1.Visible = true;
                    PictureBox1.BringToFront();
                    PictureBox2d.Visible = false;
                    UpdateSection_2b();
                    break;
                case "tabWallEP":
                    this.Width = 1386;
                    PictureBox1.Visible = true;
                    PictureBox1.BringToFront();
                    PictureBox2d.Visible = false;
                    UpdateSection_2c();
                    break;
                case "tabOldGuysRuleTool":
                    this.Width = 1386;
                    PictureBox2d.Visible = true;
                    PictureBox2d.BringToFront();
                    PictureBox1.SendToBack();
                    PictureBox1.Visible = false;
                    VIEWSCALE = -99;
                    numUpDownScale.Minimum = 1;
                    numUpDownScale.Maximum = 100;
                    numUpDownScale.Increment = 1;
                    numUpDownScale.Value = 20;
                    numUpDownScale.BringToFront();
                    Label74.BringToFront();
                    SECTIONVIEWMODE = "TYPICAL";
                    wsu.UpdateSectionTypical_2D();
                    break;
                case "tabWallDesign":
                    this.Width = 576;
                    
                    break;
            }
        }
    
        #endregion

        private void frmWall1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}