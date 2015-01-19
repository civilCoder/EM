using Autodesk.AutoCAD.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MNP
{
    public sealed partial class frmMNP : Form
    {
        public frmMNP()
        {
            InitializeComponent();
            initialize_Form();
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

            optSD.Checked = true;
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

        #region "Events"

        private void optSD_Click(object sender, EventArgs e)
        {
            if (optSD.Checked)
            {
                cmd = "cmdSD";
                tbxType.Text = "SD";
                pntDesc = "UTL-SD";
            }
        }

        private void optSS_Click(Object sender, EventArgs e)
        {
            if (optSS.Checked)
            {
                cmd = "cmdSS";
                tbxType.Text = "SS";
                pntDesc = "UTL-SEW";
            }
        }

        private void optWTR_Click(Object sender, EventArgs e)
        {
            if (optWTR.Checked)
            {
                cmd = "cmdWTR";
                tbxType.Text = "WTR";
                pntDesc = "UTL-WTR";
            }
        }

        private void optFIRE_Click(object sender, EventArgs e)
        {
            if (optFIRE.Checked)
            {
                cmd = "cmdFIRE";
                tbxType.Text = "FIRE";
                pntDesc = "UTL-FIRE";
            }
        }

        private void optAlpha_Click(object sender, EventArgs e)
        {
            if (optAlpha.Checked)
            {
                if (tbxType.Text != "")
                {
                    string index = Align.getAlignIndex(string.Format("{0}-{1}", tbxType.Text, cbxLineLat.SelectedItem.ToString()));
                    tbxIndex.Text = index;
                }
            }
        }

        private void optNumeric_Click(object sender, EventArgs e)
        {
            if (optNumeric.Checked)
            {
                if (tbxType.Text != "")
                {
                    string index = Align.getAlignIndex(string.Format("{0}-{1}", tbxType.Text, cbxLineLat.SelectedItem.ToString()));
                    tbxIndex.Text = index;
                }
            }
        }

        private void cbxAlignStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            string nameAlignStyle = cbxAlignStyle.SelectedItem.ToString();
        }

        private void cbxLineLat_SelectedIndexChanged(object sender, EventArgs e)
        {
            string lineLat = cbxLineLat.SelectedItem.ToString();
        }

        private void cbxAlignStyleLabel_SelectedIndexChanged(object sender, EventArgs e)
        {
            string nameAlignStyleLabel = cbxAlignLabelSetStyles.SelectedItem.ToString();
        }

        private void cbxProfileStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            string nameProfileStyle = cbxProfileStyleDE.SelectedItem.ToString();
        }

        private void cbxProfileViewStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbxProfileViewStyle.SelectedItem = cbxProfileViewStyle.SelectedIndex;
            string nameProfileViewStyle = cbxProfileViewStyle.SelectedItem.ToString();
        }

        #endregion "Events"

        #region "Commands"

        private void cmdMakeAlign_Click(object sender, EventArgs e)
        {
            BaseObjs.acadActivate();
            bool addCurves = false;
            if (cbxAddCurves.Checked)
                addCurves = true;

            nameLayer = string.Format("{0}-{1}-{2}", tbxType.Text, cbxLineLat.SelectedItem.ToString().ToUpper(), tbxIndex.Text);
            idLayer = Layer.manageLayers(nameLayer);

            nameStyle = cbxAlignStyle.SelectedItem.ToString();
            nameStyleLabel = cbxAlignLabelSetStyles.SelectedItem.ToString();

            string nameAlign = MNP_Align.makeAlign(nameLayer, nameLayer, nameStyle, nameStyleLabel, addCurves);
            ToolStripStatusLabel1.Text = string.Format("ACTIVE ALIGNMENT: {0}", nameAlign);
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

        private void cmdEdit_Click(object sender, EventArgs e)
        {
            MNP_Align.editAlign(idAlign);
        }

        private void cmdProfileView_Click(object sender, EventArgs e)
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

        private void cmdMakePipeNetwork_Click(object sender, EventArgs e)
        {
            using (BaseObjs._acadDoc.LockDocument())
            {
                MNP_Network.makePipeNetwork(idAlign, "N01", "Concrete");
            }
        }

        private void cmdEditNetwork_Click(object sender, EventArgs e)
        {
        }

        private void cmdAddNetworkToProfileView_Click(object sender, EventArgs e)
        {
        }

        private void cmdExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion "Commands"

        private void frmMNP_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}