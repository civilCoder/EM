using System;
using System.Windows.Forms;

namespace Base_Tools45.Office
{
    public partial class frmInput : Form
    {
        public frmInput()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Access_ext.readData(@"E:\Spinn\ADMIN2014\timedata.accdb");
            Access_ext.readData(@"C:\Users\john\Desktop\timedata.mdb");
        }
    }
}