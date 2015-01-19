using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

using Base_Tools45.C3D;

namespace EW.Forms
{
    /// <summary>
    /// Interaction logic for winSurfaces.xaml
    /// </summary>
    public partial class winSurfaces : Window
    {
        public winSurfaces()
        {
            InitializeComponent();
        }

        private void cmdOK_Click(object sender, RoutedEventArgs e)
        {
            string strBASE = this.lbxBase.SelectedItem.ToString();
            string strCOMPARE = this.lbxComp.SelectedItem.ToString();

            EW.EW_MakeSurfaceVol.makeVolSurface(strBASE, strCOMPARE, true);
        }

        private void wSurfaces_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            EW.EW_Forms.ewFrms.wEW.Show();
        }

        private void wSurfaces_Activated(object sender, EventArgs e)
        {
            List<string> surfaces = Surf.getSurfaces();
            foreach (string surface in surfaces)
            {

                this.lbxBase.Items.Add(surface);
                this.lbxComp.Items.Add(surface);

            }
        }
    }
}
