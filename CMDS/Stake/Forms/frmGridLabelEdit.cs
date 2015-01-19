using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace Stake.Forms
{
    public partial class frmGridLabelEdit : Form
    {
        public frmGridLabelEdit()
        {
            InitializeComponent();
        }

        private static ObjectId idLine;
        public static Line GRIDLINE { get; set;}

        public TypedValue[] tv { get; set; }

        private void cmdDone_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            TypedValue[] tvs = new TypedValue[4];
            tvs[0] = tv[0];
            tvs.SetValue(new TypedValue(tv[1].TypeCode, txtGridLabelEdit.Text), 1);
            tvs[2] = tv[2];
            tvs[3] = tv[3];

            idLine.setXData(tvs, "GRID");

            this.Hide();

            Stake_Forms.sForms.fGrid.Show();
        }

        private void txtGridLabelEdit_TextChanged(System.Object eventSender, System.EventArgs eventArgs)
        {
            txtGridLabelEdit.Text = txtGridLabelEdit.Text.ToUpper();
        }

        private void frmGridLabelEdit_Load(object sender, System.EventArgs e)
        {
            Point3d pnt3dPick = Pub.pnt3dO;
            try
            {
                Entity ent = Base_Tools45.Select.selectEntity(typeof(Line), "Select Grid Line for Label Edit:", "", out pnt3dPick);
                if (ent == null)
                    return;
                idLine = ent.ObjectId;
            }
            catch
            {
                Application.ShowAlertDialog("Retry and make sure to elect the Grid Line not the text");
                return;
            }

            ResultBuffer rb = idLine.getXData("GRID");
            TypedValue[] tvs = rb.AsArray();
            tv = tvs;

            this.lblGridLabel.Text = tvs[1].Value.ToString();
        }

        private void frmGridLabelEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}