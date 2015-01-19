using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

using Autodesk.Civil.DatabaseServices;

using Base_Tools45;

namespace PadCerts
{
    /// <summary>
    /// Interaction logic for winPacCerts.xaml
    /// </summary>
    public partial class winPadCerts : Window{

        public List<Point3d> PntsLeader { get; set; }

        public winPadCerts()
        {
            InitializeComponent();
        }

        private void cmdSelectPoints_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            SelectionSet ss = Select.buildSSet(typeof(CogoPoint), false, "Select Survey Points for Analysis:");
            PC_App.processPoints(ss, PntsLeader);

        }

        private void cmdLeaderLayout_Click(object sender, RoutedEventArgs e)
        {
            ObjectId idLdr = Ldr.drawLdr(Point3d.Origin, 0.09, "ARROW", 7);
            PntsLeader = idLdr.getCoordinates3dList();
        }
    }
}
