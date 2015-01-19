using System;
using System.Windows.Forms;

namespace Stake.Forms
{
    public partial class frmPickXref : Form
    {
        public string nameXRef { get; set; }

        public frmPickXref()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            nameXRef = this.lboxSelectXref.SelectedItem.ToString();
            this.Hide();
        }

        private void frmPickXref_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}