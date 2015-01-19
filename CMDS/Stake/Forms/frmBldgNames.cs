using System;
using System.Windows.Forms;

namespace Stake.Forms
{
    public partial class frmBldgNames : Form
    {
        public frmBldgNames()
        {
            InitializeComponent();
        }

        private void cmdDone_Click(object sender, EventArgs e)
        {
            Stake_Grid.updateDictGridsBldgNum();
            this.Hide();
        }

        private void frmBldgNames_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}