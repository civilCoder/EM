using System.Windows.Forms;

namespace EW.Forms
{
    public partial class frmGridSpacing : Form
    {
        short intGridSpacing;
        object[] varGridData;

        public bool boolGrid;

        public frmGridSpacing()
        {
            InitializeComponent();
        }

        private void cmdOK_Click(System.Object eventSender, System.EventArgs eventArgs)
        {

            intGridSpacing = System.Convert.ToInt16(EW.EW_Forms.ewFrms.fGridSpacing.txtGridSpacing.Text);

            varGridData = EW.EW_MakeSurfaceBot.makeBotSurfaceGrid(intGridSpacing, 4);
            EW.EW_Forms.ewFrms.fEarthwork.GRIDDATA = varGridData;

            EW.EW_Forms.ewFrms.fEarthwork.GRID = true;

            this.Hide();

            EW.EW_Forms.ewFrms.fEarthwork.Show();

        }
        private void frmGridSpacing_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
