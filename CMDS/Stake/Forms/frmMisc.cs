using Autodesk.AutoCAD.Geometry;
using System;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace Stake.Forms
{
    public partial class frmMisc : Form
    {
        private static Forms.frmStake fStake;

        public frmMisc()
        {
            InitializeComponent();
        }

        public Point3d PICK_PNT { get; set; }

        private void cbxObjType_TextChanged(System.Object eventSender, System.EventArgs eventArgs)
        {
            cbxObjType.Text = cbxObjType.Text.ToUpper();
            tbxLayer.Text = cbxOffset.Text + "-OS-" + cbxObjType.Text;
            tbxDesc0.Text = cbxOffset.Text + "' O/S ";
            tbxDescription.Text = cbxObjType.Text + "@";
        }

        private void cbxOffset_TextChanged(System.Object eventSender, System.EventArgs eventArgs)
        {
            tbxLayer.Text = cbxOffset.Text + "-OS-" + cbxObjType.Text;
            tbxDesc0.Text = cbxOffset.Text + "' O/S ";
            tbxDescription.Text = cbxObjType.Text + "@";
        }

        private void cmdAP_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            Stake_Misc2.stakeMISC("AP");
        }

        private void cmdInLine_Center_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            Stake_Misc2.stakeMISC("InLine_Center");
        }

        private void cmdInLine_Direction_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            Stake_Misc2.stakeMISC("InLine_Direction");
        }

        private void cmdProj2Curb_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            Stake_Misc2.stakeMISC("Proj2Curb");
        }

        private void cmdProj2Bldg_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            Stake_Misc2.stakeMISC("Proj2Bldg");
        }

        private void tbxDescription_TextChanged(System.Object eventSender, System.EventArgs eventArgs)
        {
            tbxDescription.Text = tbxDescription.Text.ToUpper();
        }

        private void tbxLayer_TextChanged(System.Object eventSender, System.EventArgs eventArgs)
        {
            tbxLayer.Text = tbxLayer.Text.ToUpper();
        }

        private void frmMisc_Load(object sender, System.EventArgs e)
        {
            cbxOffset.Items.Add(Convert.ToString(0));
            cbxOffset.Items.Add(Convert.ToString(3));
            cbxOffset.Items.Add(Convert.ToString(5));
            cbxOffset.Items.Add(Convert.ToString(10));
            cbxOffset.Items.Add(Convert.ToString(15));

            cbxOffset.SelectedIndex = 1;

            cbxOffsetDir.Items.Add(Convert.ToString(3));
            cbxOffsetDir.Items.Add(Convert.ToString(5));
            cbxOffsetDir.Items.Add(Convert.ToString(10));
            cbxOffsetDir.Items.Add(Convert.ToString(15));

            cbxOffsetDir.SelectedIndex = 2;

            cbxObjType.Items.Add("Select Object Type");
            cbxObjType.Items.Add("CURB");
            cbxObjType.Items.Add("FH");
            cbxObjType.Items.Add("LOT LIGHT");
            cbxObjType.Items.Add("VAULT");

            cbxObjType.SelectedIndex = 0;

            cbxCurbHeight.Items.Add(Convert.ToString(0));
            cbxCurbHeight.Items.Add(Convert.ToString(6));
            cbxCurbHeight.Items.Add(Convert.ToString(8));
            cbxCurbHeight.Items.Add(Convert.ToString(12));
            cbxCurbHeight.Items.Add(Convert.ToString(18));

            cbxCurbHeight.SelectedIndex = 1;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            fStake = Forms.Stake_Forms.sForms.fStake;
            Application.ShowModelessDialog(Application.MainWindow.Handle, fStake, false);
            fStake.optMISC.Checked = false;
            this.Hide();
        }

        private void cmdDone_Click(object sender, EventArgs e)
        {
            fStake = Forms.Stake_Forms.sForms.fStake;
            fStake.optMISC.Checked = false;
            Application.ShowModelessDialog(Application.MainWindow.Handle, fStake, false);
            this.Hide();
        }

        private void frmMisc_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}