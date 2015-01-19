using System.Collections.Generic;
using System.Windows.Controls;
using System;

using Base_Tools45;
using Base_Tools45.C3D;

namespace Grading.myForms
{
    /// <summary>
    /// Interaction logic for SlopeGrade.xaml
    /// </summary>
    public partial class GradeSlope : UserControl
    {
        private string _nameCmd;

        public GradeSlope()
        {
            InitializeComponent();
            cbxB2.IsChecked = true;
            cbxB1.IsChecked = true;

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
                if (cbxB1.IsChecked == true)
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
            get {
                return (cbxB2.IsChecked == true) ? true : false;
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
                cbxSurfaceDes.Items.Clear();
                cbxSurfaceTar.Items.Clear();
                List<string> Surfaces = Surf.getSurfaces();
                foreach (string Name in Surfaces)
                {
                    cbxSurfaceDes.Items.Add(Name);
                    cbxSurfaceTar.Items.Add(Name);
                }
            }
            catch (SystemException)
            {
            }
        }

        private void cbxSurfaceTar_Click(object sender, SelectionChangedEventArgs e)
        {
            updateSurfaceTarList();
        }

        private void cbxSurfaceDes_Click(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnBuildSlope_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Boolean boolOK = false;
            string surfTAR = string.Empty;
            string surfDES = string.Empty;
            int intInterval = Convert.ToInt32(tbxInterval.Text.ToString());

            if (optSurface.IsChecked == true)
            {
                try
                {
                    surfTAR = cbxSurfaceTar.SelectedItem.ToString();
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
                surfDES = cbxSurfaceDes.SelectedItem.ToString();
            }
            catch (Exception)
            {
                BaseObjs.write("Select Design Surface (where the results are to be placed)");
                return;
            }
            double elev = 0.0;
            if (optElevation.IsChecked == true)
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
    }
}