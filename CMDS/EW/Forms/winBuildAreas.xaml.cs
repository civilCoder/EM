using System.Windows;

using Autodesk.AutoCAD.EditorInput;

using Base_Tools45;

namespace EW.Forms
{
    /// <summary>
    /// Interaction logic for winBuildAreas.xaml
    /// </summary>
    public partial class winBuildAreas : Window
    {
        public winBuildAreas()
        {
            InitializeComponent();
        }

        private void cmdTransfer_Click(object sender, RoutedEventArgs e)
        {
            EW_Transfer.transferObjs("AREAS");
        }

        private void cmdIdLayer_Click(object sender, RoutedEventArgs e)
        {
            BaseObjs._acadDoc.SendStringToExecute("_xlist\r", true, false, false);
        }

        private void cmdSelectLayers2Freeze_Click(object sender, RoutedEventArgs e)
        {
            BaseObjs._acadDoc.SendStringToExecute("_layfrz\r", true, false, false);
        }

        private void cmdBoundary_Click(object sender, RoutedEventArgs e)
        {
            BaseObjs._acadDoc.SendStringToExecute("BOUNDARY\r", true, false, false);
        }

        private void cmdMakePolyline_Click(object sender, RoutedEventArgs e)
        {
            BaseObjs._acadDoc.SendStringToExecute("PL\r", true, false, false);
        }

        private void cmdSAP_Click(object sender, RoutedEventArgs e)
        {
            EW.EW_SetAreaProp.setAreaProp();
        }

        private void cmdCheckAreaLimits_Click(object sender, RoutedEventArgs e)
        {
            SelectionSet objSSet = EW_Utility1.buildSSet0();
            EW.EW_CheckAreaLimits.checkAreaLimits(objSSet);

            objSSet = EW_Utility1.buildSSet_XX_OX();
            EW.EW_CheckAreaLimits.testAreaOffset(objSSet);
        }

        private void wBuildAreas_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            EW_Forms.ewFrms.wEW.Show();
        }   
    }
}
