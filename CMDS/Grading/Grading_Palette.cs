using Autodesk.AutoCAD.Windows;
using Base_Tools45;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Grading
{
    public sealed class Grading_Palette
    {
        private static readonly Grading_Palette gradePalette = new Grading_Palette();

        private static PaletteSet _ps = null;


        private Grading_Palette()
        {
            pGradeFloor = new myForms.GradeFloor();
            pGradePlane = new myForms.GradeTiltedPlane();
            pGrading = new myForms.GradeSite();
            pGradeSlope = new myForms.GradeSlope();
            pUpdateCNTL = new myForms.updateCNTL();
        }

        public static Grading_Palette gPalette
        {
            get
            {
                return gradePalette;
            }
        }

        public static System.Windows.Controls.ListBox lstBox1X
        {
            get
            {
                return gPalette.pGradeFloor.lstBox1;
            }
        }

        public myForms.GradeFloor pGradeFloor { get; set; }
        public myForms.GradeTiltedPlane pGradePlane  { get; set; }
        public myForms.GradeSite pGrading { get; set; }
        public myForms.GradeSlope pGradeSlope { get; set; }
        public myForms.updateCNTL pUpdateCNTL { get; set; }

        public static void
        setfocus(string name)
        {
            PaletteSet p = _ps;
            for (int i = 0; i < p.Count; i++)
            {
                if (p[i].Name == name)
                {
                    p.Activate(i);
                    break;
                }
            }
        }

        public void showPalettes(bool vis, int index = 0)
        {
            bool autoSize = false;
            try
            {
                if (_ps == null)
                {
                    try
                    {
                        _ps = new PaletteSet("TEI Engineering Tools", new Guid("{C30FE6B0-7A39-40DD-97E1-E31697DD26A3}"));
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(ex.Message + " Grading_Palette.cs: line: 75");
                    }
                    _ps.DockEnabled = (DockSides)DockingBehavior.Never;
                    _ps.Dock = (DockSides)DockStyle.None;
                    _ps.WindowState = Window.State.Minimized;

                    ElementHost host;

                    host = new ElementHost();
                    host.AutoSize = autoSize;
                    host.Dock = DockStyle.None;
                    host.Child = pGradeFloor;
                    _ps.Add("Grade Finish Floor", host);



                    host = new ElementHost();
                    host.AutoSize = autoSize;
                    host.Dock = DockStyle.None;
                    host.Child = pGradeSlope;
                    _ps.Add("Grade Slope", host);


                    host = new ElementHost();
                    host.AutoSize = autoSize;
                    host.Dock = DockStyle.None;
                    host.Child = pGradePlane;
                    _ps.Add("Grade Tilted Plane", host);

                    host = new ElementHost();
                    host.AutoSize = autoSize;
                    host.Dock = DockStyle.None;
                    host.Child = pGrading;
                    _ps.Add("Site Grading Tools", host);

                    host = new ElementHost();
                    host.AutoSize = autoSize;
                    host.Dock = DockStyle.None;
                    host.Child = pUpdateCNTL;
                    _ps.Add("Update CNTL", host);

                    _ps.PaletteActivated += _ps_PaletteActivated;
                }

                _ps.InitializeFloatingPosition(new System.Windows.Rect(new System.Windows.Point(200, 200),
                    new System.Windows.Size(400, 400)));
                _ps.FloatControl(new System.Windows.Point(200, 200));
                _ps.KeepFocus = true;
                _ps.Visible = vis;
                _ps.Activate(index);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Grading_Palette.cs: line: 128");
            }
        }

        private static void _ps_PaletteActivated(object sender, PaletteActivatedEventArgs e)
        {
            int width = 400;
            int height = 400;
            Palette p = e.Activated;

            if (p.Name == "Grade Finish Floor")
                _ps.Size = new Size(width, height);
            if (p.Name == "Grade Slope")
                _ps.Size = new Size(width, height);
            if (p.Name == "Grade Tilted Plane")
                _ps.Size = new Size(width, height);
            if (p.Name == "Site Grading Tools")
                _ps.Size = new Size(width, height);
            if (p.Name == "Update CNTL")
                _ps.Size = new Size(width, height);
        }
    }
}
