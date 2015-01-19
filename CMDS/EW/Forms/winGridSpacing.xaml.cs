using System.Windows;

namespace EW.Forms
{
    /// <summary>
    /// Interaction logic for winGridSpacing.xaml
    /// </summary>
    public partial class winGridSpacing : Window
    {
        short intGridSpacing;
        object[] varGridData;

        public winGridSpacing()
        {
            InitializeComponent();
        }

        private void wGridSpacing_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            EW.EW_Forms.ewFrms.wEW.Show();

        }

        private void cmdOK_Click(object sender, RoutedEventArgs e)
        {
            intGridSpacing = System.Convert.ToInt16(EW.EW_Forms.ewFrms.fGridSpacing.txtGridSpacing.Text);

            varGridData = EW.EW_MakeSurfaceBot.makeBotSurfaceGrid(intGridSpacing, 4);
            EW.EW_Forms.ewFrms.fEarthwork.GRIDDATA = varGridData;

            EW.EW_Forms.ewFrms.fEarthwork.GRID = true;

            this.Hide();
        }
    }
}
