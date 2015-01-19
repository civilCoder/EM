using Base_Tools45;
using System.Windows.Controls;

namespace Grading.myForms
{
    /// <summary>
    /// Interaction logic for Grading_TiltedPlane.xaml
    /// </summary>
    public partial class GradeTiltedPlane : UserControl
    {
        public GradeTiltedPlane()
        {
            InitializeComponent();
        }

        private void btnSelPnts_Click(object sender, System.Windows.RoutedEventArgs e)
        {            
            if ((bool)tp1.IsChecked)
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    BaseObjs.acadActivate();
                    BaseObjs._acadDoc.SendStringToExecute("TP1\r", true, false, false);
                }
                //-----------------------------------------------------------------------------------------------------------
            }
            else if ((bool)tp2.IsChecked)
            {
                using (BaseObjs._acadDoc.LockDocument())
                {

                    BaseObjs.acadActivate();
                    BaseObjs._acadDoc.SendStringToExecute("TP2\r", true, false, false);
                }
                //-----------------------------------------------------------------------------------------------------------
            }
            else if ((bool)tp3.IsChecked)
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    BaseObjs.acadActivate();
                    BaseObjs._acadDoc.SendStringToExecute("TP3\r", true, false, false);
                }
            }
            else if ((bool)tp4.IsChecked)
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    BaseObjs.acadActivate();
                    BaseObjs._acadDoc.SendStringToExecute("TP4\r", true, false, false);
                }
            }
        }
    }
}