using System.Windows.Controls;

namespace Grading.myForms
{
    /// <summary>
    /// Interaction logic for Grading_BuildingAdjacentParking.xaml
    /// </summary>
    public partial class GradeBuildingAdjacentParking : UserControl
    {
        public GradeBuildingAdjacentParking()
        {
            InitializeComponent();
            for (int i = 0; i < 11; i = i + 2)
            {
                if (i == 6)
                {
                    cbxDist2Curb.Items.Add(5);
                }
                cbxDist2Curb.Items.Add(i);
            }
            cbxDist2Curb.SelectedIndex = 1;

            cbxCurbHeight.Items.Add(6);
            cbxCurbHeight.Items.Add(8);
            cbxCurbHeight.Items.Add(12);
            cbxCurbHeight.SelectedIndex = 0;

            cbxGutterWidth.Items.Add(2);
            cbxGutterWidth.Items.Add(3);
            cbxGutterWidth.Items.Add(4);
            cbxGutterWidth.Items.Add(5);
            cbxGutterWidth.Items.Add(6);
            cbxGutterWidth.SelectedIndex = 1;

            cbxGutterDepth.Items.Add(1);
            cbxGutterDepth.Items.Add(2);
            cbxGutterDepth.Items.Add(3);
            cbxGutterDepth.SelectedIndex = 1;

            cbxDist2FL.Items.Add(17);
            cbxDist2FL.Items.Add(19);
            cbxDist2FL.SelectedIndex = 0;
        }

        private void cbxDist2Curb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void cbxCurbHeight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void cmdGradeParking_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void cmdLimParking_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void chkXGutter_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
        }
    }
}