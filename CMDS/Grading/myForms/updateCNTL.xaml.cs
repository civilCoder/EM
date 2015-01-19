using Base_Tools45;
using System;
using System.Collections.Generic;

namespace Grading.myForms
{
    /// <summary>
    /// Interaction logic for updateCNTL.xaml
    /// </summary>
    public partial class updateCNTL : System.Windows.Controls.UserControl
    {
        private string snameXRef;

        private string _TARGETDWGNAME;

        private string _TARGETDWGPATH;

        private bool _isOpen;

        public updateCNTL()
        {
            InitializeComponent();
            UserControl_Initialize();
        }

        public string nameXRef
        {
            get
            {
                return snameXRef;
            }
            set
            {
                snameXRef = value;
            }
        }

        public string TARGETDWGNAME
        {
            get
            {
                return _TARGETDWGNAME;
            }
            set
            {
                _TARGETDWGNAME = value;
            }
        }

        public string TARGETDWGPATH
        {
            get
            {
                return _TARGETDWGPATH;
            }
            set
            {
                _TARGETDWGPATH = value;
            }
        }

        public bool isOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                _isOpen = value;
            }
        }

        public void
        UserControl_Initialize()
        {
            List<string> layers = Cmds.cmdUC1.getLayers();
            foreach (string layer in layers)
            {
                lstBox.Items.Add(layer);
            }
            for (int i = 0; i < lstBox.Items.Count; i++)
            {
                lstBox.SelectedIndex = i;
            }
            this.Width = 310;
            this.Height = 350;
        }

        private void cmdADD_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                string layer = Cmds.cmdUC1.addEntToList();
                if (!lstBox.Items.Contains(layer))
                    lstBox.Items.Add(layer);
                lstBox.SelectedIndex = lstBox.Items.Count - 1;
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} updateCNTL.xaml.cs: line: 82", ex.Message));
            }
        }

        private void cmdOK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                System.Collections.IList col = lstBox.SelectedItems;
                List<string> layers = new List<string>();

                foreach (Object obj in col)
                    layers.Add(obj.ToString());

                Cmds.cmdUC1.executeCmdUC(layers);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} updateCNTL.xaml.cs: line: 97", ex.Message));
            }
        }
    }
}