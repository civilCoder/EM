using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Grading.myForms
{
    public partial class frmSG : Form
    {
        private string _nameCmd;

        public frmSG()
        {
            InitializeComponent();

            cbxB2.Checked = true;
            cbxB1.Checked = true;

            tbxB2Width.Text = Convert.ToString(2);
            tbxB1Width.Text = Convert.ToString(2);
            tbxB2Slope.Text = Convert.ToString(.02);
            tbxB1Slope.Text = Convert.ToString(.02);
            tbxSlopeH.Text = Convert.ToString(2);
            tbxInterval.Text = Convert.ToString(10);
            updateSurfaceTarList();
        }

        public string strInterval
        {
            get
            {
                return tbxInterval.Text.ToString();
            }
        }

        public string strB1Width
        {
            get
            {
                return tbxB1Width.Text.ToString();
            }
        }

        public string strB2Width
        {
            get
            {
                return tbxB2Width.Text.ToString();
            }
        }

        public string strB1Slope
        {
            get
            {
                return tbxB1Slope.Text.ToString();
            }
        }

        public string strB2Slope
        {
            get
            {
                return tbxB2Slope.Text.ToString();
            }
        }

        public double slope
        {
            get
            {
                return double.Parse(tbxSlopeV.Text.ToString()) / double.Parse(tbxSlopeH.Text.ToString());
            }
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

        public string nameCmd
        {
            get
            {
                return _nameCmd;
            }
            set
            {
                _nameCmd = value;
            }
        }

        private void
        updateSurfaceTarList()
        {
            try
            {
                cbxSurfaceDES.Items.Clear();
                cbxSurfaceTAR.Items.Clear();
                List<string> Surfaces = Surf.getSurfaces();
                foreach (string Name in Surfaces)
                {
                    cbxSurfaceDES.Items.Add(Name);
                    cbxSurfaceTAR.Items.Add(Name);
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

            if (rdoTargetSurface.Checked)
            {
                try
                {
                    surfTAR = cbxSurfaceTAR.SelectedItem.ToString();
                }
                catch (Exception)
                {
                    BaseObjs.write("Select Target Surface (surface to daylight)");
                    return;
                }
            }

            double slope = double.Parse(tbxSlopeV.Text) / double.Parse(tbxSlopeH.Text);

            try
            {
                surfDES = cbxSurfaceDES.SelectedItem.ToString();
            }
            catch (Exception)
            {
                BaseObjs.write("Select Design Surface (where the results are to be placed)");
                return;
            }
            double elev = 0.0;
            if (rdoTargetElev.Checked)
            {
                surfTAR = "";
                elev = double.Parse(tbxElev.Text);
            }
            double B1Width = double.Parse(strB1Width);
            double B2Width = double.Parse(strB2Width);
            double B1Slope = double.Parse(strB1Slope);
            double B2Slope = double.Parse(strB2Slope);

            int interval = int.Parse(strInterval);
            switch (nameCmd)
            {
                case "cmdBD":
                    boolOK = Grading.Cmds.cmdBD.BD(B1Width, B1Slope, B2Width, B2Slope, slope, interval, surfTAR, surfDES, elev: elev);
                    break;

                case "cmdSG":
                    boolOK = Grading.Cmds.cmdSG.SG(B1Width, B1Slope, B2Width, B2Slope, slope, interval, surfTAR, surfDES, elev: elev);
                    break;
            }
        }

        private void rdoTargetElev_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoTargetElev.Checked)
            {
                string text = label6.Text;
                text = text.Replace("SURFACE", "ELEVATION");
                label6.Text = text;
                cbxSurfaceTAR.Visible = false;
                tbxElev.Visible = true;
            }
        }

        private void rdoTargetSurface_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoTargetSurface.Checked)
            {
                string text = label6.Text;
                text = text.Replace("ELEVATION", "SURFACE");
                label6.Text = text;
                cbxSurfaceTAR.Visible = true;
                tbxElev.Visible = false;
            }
        }

        private void cbxSurfaceTAR_Click(object sender, EventArgs e)
        {
            updateSurfaceTarList();
        }

        private void frmSG_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}