using Base_Tools45;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Grading.myForms
{
    /// <summary>
    /// Interaction logic for Grading_Dock.xaml
    /// </summary>
    public partial class GradeDock : UserControl
    {
        public GradeDock()
        {
            InitializeComponent();

            List<string> Thicknesses = new List<string>();
            for (int i = 6; i < 13; i++)
            {
                Thicknesses.Add(i.ToString());
            }
            lbxWallThickness.ItemsSource = Thicknesses;
            lbxWallThickness.SelectedIndex = 1;

            for (int i = 1; i < 5; i++)
            {
                lbxDockCount.Items.Add(i);
            }
            lbxDockCount.SelectedIndex = 1;
        }

        private void cmdGetDockLimits_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            try
            {
                Base_Tools45.Grading_Dock.getDockLimits(int.Parse(lbxDockCount.SelectedValue.ToString()));
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} GradeDock.xaml.cs: line: 41", ex.Message));
            }
        }

        private void cmdGradeDock_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            try
            {
                Base_Tools45.Grading_Dock.gradeDock(Convert.ToDouble(txtDockWidth.Text), Convert.ToDouble(lbxWallThickness.SelectedValue));
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} GradeDock.xaml.cs: line: 54", ex.Message));
            }
        }

        private void cmdResetDockLimits_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                Base_Tools45.Grading_Dock.resetDockLimits();
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} GradeDock.xaml.cs: line: 67", ex.Message));
            }
        }
    }
}