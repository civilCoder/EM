using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Autodesk.AutoCAD.DatabaseServices;

using Base_Tools45;
using Base_Tools45.C3D;

namespace MNP
{
    /// <summary>
    /// Interaction logic for winMNP.xaml
    /// </summary>
    public partial class winMNP : Window
    {
        public winMNP()
        {
            InitializeComponent();
        }

        public void initialize_Form()
        {
            List<string> styles = Align_Style.getAlignStyles();
            foreach (string style in styles)
                cbxAlignStyle.Items.Add(style);

            styles = Align_Style.getAlignmentLabelSetStyles();
            foreach (string style in styles)
                cbxAlignLabelSetStyles.Items.Add(style);

            styles = Prof_Style.getProfileStyles();
            foreach (string style in styles)
            {
                cbxProfileStyleDE.Items.Add(style);
                cbxProfileStyleEX.Items.Add(style);
            }

            styles = Prof_Style.getProfileViewStyles();
            foreach (string style in styles)
                cbxProfileViewStyle.Items.Add(style);

            styles = Prof_Style.getProfileLabelSetStyles();
            foreach (string style in styles)
            {
                cbxProfileLabelSetDE.Items.Add(style);
                cbxProfileLabelSetEX.Items.Add(style);
                if (style.ToUpper() == "NONE" || style.ToUpper() == "_NO LABEL")
                {
                    cbxProfileLabelSetDE.SelectedItem = style;
                    cbxProfileLabelSetEX.SelectedItem = style;
                }
            }

            cbxLineLat.SelectedIndex = 0;

            optSD.IsChecked = true;
            cmd = "cmdSD";
            tbxType.Text = "SD";

            Height = 620;
        }

        private string cmd { get; set; }

        public string nameLayer { get; set; }

        public string nameStyle { get; set; }

        public string nameStyleLabel { get; set; }

        public string nameProfile { get; set; }

        public string pntDesc { get; set; }

        public ObjectId idLayer { get; set; }

        public ObjectId idAlign { get; set; }

        public ObjectId idAlignStyl { get; set; }

        public ObjectId idAlignStyleLabel { get; set; }

        public ObjectId idProfile { get; set; }

        public ObjectId idProfileStyleDE { get; set; }

        public ObjectId idProfileStyleEX { get; set; }

        public ObjectId idProfileStyleLabel { get; set; }

        public ObjectId idProfileLabelSetDE { get; set; }

        public ObjectId idProfileLabelSetEX { get; set; }

        public ObjectId idProfileView { get; set; }

        public ObjectId idProfileViewStyle { get; set; }

        public ObjectId idProfileBandSetStyle { get; set; }
        private void optSD_Click(object sender, RoutedEventArgs e)
        {
            if (optSD.IsChecked.Value == true)
            {
                cmd = "cmdSD";
                tbxType.Text = "SD";
                pntDesc = "UTL-SD";
            }
        }

        private void optSS_Click(object sender, RoutedEventArgs e)
        {
            if (optSS.IsChecked.Value == true)
            {
                cmd = "cmdSS";
                tbxType.Text = "SS";
                pntDesc = "UTL-SEW";
            }
        }

        private void optWTR_Click(object sender, RoutedEventArgs e)
        {
            if (optWTR.IsChecked.Value == true)
            {
                cmd = "cmdWTR";
                tbxType.Text = "WTR";
                pntDesc = "UTL-WTR";
            }
        }

        private void optFIRE_Click(object sender, RoutedEventArgs e)
        {
            if (optFIRE.IsChecked.Value == true)
            {
                cmd = "cmdFIRE";
                tbxType.Text = "FIRE";
                pntDesc = "UTL-FIRE";
            }
        }

        private void cmdAlignMake_Click(object sender, RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            bool addCurves = false;
            if (cbxAddCurves.IsChecked.Value == true)
                addCurves = true;

            nameLayer = string.Format("{0}-{1}-{2}", tbxType.Text, cbxLineLat.SelectedItem.ToString().ToUpper(), tbxIndex.Text);
            idLayer = Layer.manageLayers(nameLayer);

            nameStyle = cbxAlignStyle.SelectedItem.ToString();
            nameStyleLabel = cbxAlignLabelSetStyles.SelectedItem.ToString();

            string nameAlign = MNP_Align.makeAlign(nameLayer, nameLayer, nameStyle, nameStyleLabel, addCurves);
            tbxActiveAlign.Text = string.Format("ACTIVE ALIGNMENT: {0}", nameAlign);
            int idx = 0;
            string index = tbxIndex.Text;
            if (index.isInteger(out idx))
            {
                idx += 1;
                index = idx.ToString();
            }
            else
            {
                index.getStringIncrement();
            }

            tbxIndex.Text = index;
        }

        private void cmdAlignActivate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmdProfileView_Click(object sender, RoutedEventArgs e)
        {
            BaseObjs.acadActivate();

            idProfileBandSetStyle = Prof_Style.getProfileViewBandSetStyle();

            if (cbxProfileStyleDE.SelectedIndex == -1)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Please select Profile Style: ");
                return;
            }

            if (cbxProfileViewStyle.SelectedIndex == -1)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Please select Profile View Style: ");
                return;
            }

            idProfileStyleDE = Prof_Style.getProfileStyle(cbxProfileStyleDE.SelectedItem.ToString());
            idProfileStyleEX = Prof_Style.getProfileStyle(cbxProfileStyleEX.SelectedItem.ToString());
            idProfileLabelSetDE = Prof_Style.getProfileLabelSetStyle(cbxProfileLabelSetDE.SelectedItem.ToString());
            idProfileLabelSetEX = Prof_Style.getProfileLabelSetStyle(cbxProfileLabelSetEX.SelectedItem.ToString());
            idProfileViewStyle = Prof_Style.getProfileViewStyle(cbxProfileViewStyle.SelectedItem.ToString());

            idProfileView = MNP_Profile.makeProfile(idAlign);
        }

        private void cmdEdit_Click(object sender, RoutedEventArgs e)
        {
            MNP_Align.editAlign(idAlign);
        }

        private void cmdMakePipeNetwork_Click(object sender, RoutedEventArgs e)
        {
            using (BaseObjs._acadDoc.LockDocument())
            {
                MNP_Network.makePipeNetwork(idAlign, "N01", "Concrete");
            }
        }

        private void cmdAddNetworkToProfileView_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cbxAddCurves_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void optAlpha_Click(object sender, RoutedEventArgs e)
        {
            if (optAlpha.IsChecked.Value == true)
            {
                if (tbxType.Text != "")
                {
                    string index = Align.getAlignIndex(string.Format("{0}-{1}", tbxType.Text, cbxLineLat.SelectedItem.ToString()));
                    tbxIndex.Text = index;
                }
            }
        }

        private void optNumeric_Checked(object sender, RoutedEventArgs e)
        {
            if (optNumeric.IsChecked.Value == true)
            {
                if (tbxType.Text != "")
                {
                    string index = Align.getAlignIndex(string.Format("{0}-{1}", tbxType.Text, cbxLineLat.SelectedItem.ToString()));
                    tbxIndex.Text = index;
                }
            }
        }

        private void cbxLineLat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string lineLat = cbxLineLat.SelectedItem.ToString();
        }

        private void cbxAlignStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string nameAlignStyle = cbxAlignStyle.SelectedItem.ToString();
        }

        private void cbxAlignLabelSetStyles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string nameAlignStyleLabel = cbxAlignLabelSetStyles.SelectedItem.ToString();
        }

        private void cbxProfileStyleDE_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string nameProfileStyle = cbxProfileStyleDE.SelectedItem.ToString();
        }

        private void cbxProfileStyleEX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbxProfileLabelSetDE_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbxProfileLabelSetEX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbxProfileViewStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbxProfileViewStyle.SelectedItem = cbxProfileViewStyle.SelectedIndex;
            string nameProfileViewStyle = cbxProfileViewStyle.SelectedItem.ToString();

        }

        private void wMNP_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
