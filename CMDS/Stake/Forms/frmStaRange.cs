using System;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
namespace Stake.Forms
{
    public partial class frmStaRange : Form
    {
        //private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;

        public frmStaRange()
        {
            InitializeComponent();
        }

        private void cmdDone_Click(object sender, EventArgs e)
        {
            double staBeg = double.Parse(tbxStaStart.Text);
            double staEnd = double.Parse(tbxStaEnd.Text);
            this.Hide();

            Stake_Calc.stakePoints(staBeg, staEnd);
            Application.ShowModelessDialog(Application.MainWindow.Handle, Forms.Stake_Forms.sForms.fStake, false);
        }

        private void frmStaRange_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}