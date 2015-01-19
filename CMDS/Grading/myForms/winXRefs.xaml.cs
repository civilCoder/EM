using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Grading.myForms
{
    /// <summary>
    /// Interaction logic for winXRefs.xaml
    /// </summary>
    public sealed partial class winXRefs : Window
    {
        private static readonly winXRefs fXRefs = new winXRefs();

        private ObjectIdCollection _idBrkLineEdges;

        private ObjectId _idRF;

        private string _XRefName;

        private List<Grading_xRefID> _listXRefs;

        private winXRefs()
        {
            InitializeComponent();
            idXRef = ObjectId.Null;
        }

        public static winXRefs frmXRefs
        {
            get
            {
                return fXRefs;
            }
        }

        public ObjectIdCollection idBrkLineEdges
        {
            get
            {
                return _idBrkLineEdges;
            }
            set
            {
                _idBrkLineEdges = value;
            }
        }

        public ObjectId idRF
        {
            get
            {
                return _idRF;
            }
            set
            {
                _idRF = value;
            }
        }

        public string XRefName
        {
            get
            {
                return _XRefName;
            }
            set
            {
                _XRefName = value;
            }
        }

        public List<Grading_xRefID> listXRefs
        {
            get
            {
                return _listXRefs;
            }
            set
            {
                _listXRefs = value;
            }
        }

        public ObjectId idXRef { get; set; }

        private void cmdOK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string nameBlkRef = this.lstBox1.SelectedItem.ToString();
            foreach (Grading_xRefID item in listXRefs)
            {
                if (item.name == nameBlkRef)
                {
                    idXRef = item.id;
                    break;
                }
            }

            this.Close();
        }

        private void lstBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstBox1.SelectedItems.Count > 1)
            {
                lstBox1.Items.Clear();
                Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog("Only one item can be selected");
            }
            else
            {
                Grading_Palette.gPalette.pGrading.BlockXRefName = lstBox1.SelectedItem.ToString();
            }
        }
    }
}