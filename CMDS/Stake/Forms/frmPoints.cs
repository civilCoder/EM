using System;
using System.Windows.Forms;

namespace Stake.Forms
{
    public partial class frmPoints : Form
    {
        public frmPoints()
        {
            InitializeComponent();
        }

        private void cmdCreateCutsheet_Click(object sender, EventArgs e)
        {
            Stake_CutSheet.processCutsheetData("POINTLIST");
        }

        private void frmPoints_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}