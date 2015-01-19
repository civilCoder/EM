using System.Windows;

namespace EW.Forms
{
    /// <summary>
    /// Interaction logic for winProgressBar.xaml
    /// </summary>
    public partial class winProgressBar : Window
    {
        public winProgressBar()
        {
            InitializeComponent();
        }

        private void wProgressBar_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();

        }
    }
}
