using System.Windows.Forms;

namespace Stake.Forms
{
    public partial class frmAddPoint : Form
    {
        public frmAddPoint()
        {
            InitializeComponent();
        }

        private void frmAddPoint_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}