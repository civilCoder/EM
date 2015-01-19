using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace slopeGrading
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            cbxB2.Checked = true;
            cbxB1.Checked = true;

            tbxB2Width.Text = Convert.ToString(2);
            tbxB1Width.Text = Convert.ToString(2);
            tbxB2Slope.Text = Convert.ToString(.02);
            tbxB1Slope.Text = Convert.ToString(.02);
            tbxSlope.Text = Convert.ToString(2);
            tbxInterval.Text = Convert.ToString(10);

            try
            {
                List<string> Surfaces = Surf.getSurfaces();
                foreach (string Name in Surfaces)
                {
                    lbxSurfaceDES.Items.Add(Name);
                    lbxSurfaceTAR.Items.Add(Name);
                }
            }
            catch (SystemException)
            {
            }
        }

        private void btnBuildSlope_Click(object sender, EventArgs e)
        {
            Boolean boolOK = false;
            string surfTAR = string.Empty;
            string surfDES = string.Empty;
            int intInterval = Convert.ToInt32(tbxInterval.Text.ToString());

            try
            {
                surfTAR = lbxSurfaceTAR.SelectedItem.ToString();
            }
            catch (Exception)
            {
                SG_Utility.Write("Select Target Surface (surface to daylight)");
                return;
            }

            double slope = Convert.ToDouble(tbxSlope.Text);

            try
            {
                surfDES = lbxSurfaceDES.SelectedItem.ToString();
            }
            catch (Exception)
            {
                SG_Utility.Write("Select Design Surface (where the results are to be placed)");
                return;
            }

            boolOK = SG_cmdSG.SG(slope, surfTAR, surfDES);
        }

        public string strInterval
        {
            get { return tbxInterval.Text.ToString(); }
        }

        public string strB1Width
        {
            get { return tbxB1Width.Text.ToString(); }
        }

        public string strB2Width
        {
            get { return tbxB2Width.Text.ToString(); }
        }

        public string strB1Slope
        {
            get { return tbxB1Slope.Text.ToString(); }
        }

        public string strB2Slope
        {
            get { return tbxB2Slope.Text.ToString(); }
        }

        public Boolean boolB1
        {
            get
            {
                if (cbxB1.Checked == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public Boolean boolB2
        {
            get
            {
                if (cbxB2.Checked == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}