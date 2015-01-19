using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EW.Forms
{
    public partial class frmSurfaces : Form
    {
        public frmSurfaces()
        {
            InitializeComponent();
            List<string> surfaces = Surf.getSurfaces();
            foreach (string surface in surfaces)
            {

                this.lboxBASE.Items.Add(surface);
                this.lboxCOMP.Items.Add(surface);

            }
        }
        private void cmdOK_Click(System.Object eventSender, System.EventArgs eventArgs)
        {

            string strBASE = this.lboxBASE.SelectedItem.ToString();
            string strCOMPARE = this.lboxCOMP.SelectedItem.ToString();

            this.Hide();

            EW.EW_MakeSurfaceVol.makeVolSurface(strBASE, strCOMPARE, true);

        }

        private void frmSurfaces_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
