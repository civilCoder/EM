using System.Collections.Generic;
using System.Windows.Forms;

using DataSet = Base_Tools45.DataSet;

namespace Stake.Forms
{
    public partial class frmExport : Form
    {
        public frmExport()
        {
            InitializeComponent();
        }

        public List<DataSet> dataSet { get; set; }
        public List<DataSet> dataSum { get; set; }
        public List<CmdControls> cntls { get; set; }
        public string MODE { get; set; }

        private void cmdOnScreen_Click(object sender, System.EventArgs e)
        {
            this.Hide();
            SelectPoints.selectPntsOnScreen();
            this.Show();
        }

        private void cmdByRange_Click(object sender, System.EventArgs e)
        {
            this.Hide();
            SelectPoints.SelectPntsByRange();
            this.Show();
        }

        private void cmdByAlignment_Click(object sender, System.EventArgs e)
        {
            this.Hide();
            SelectPoints.SelectPntsByAlign();
            this.Show();
        }

        private void cmdLayer_Click(object sender, System.EventArgs e)
        {
            List<DataSet> varDataSet = dataSet;
            //Stake_ProcessPntList.sortGroup(ref varDataSet, "LAYER");
            Stake_ProcessPntList.addData(varDataSet, "SORT");

        }

        private void cmdObject_Click(object sender, System.EventArgs e)
        {
            List<DataSet> varDataSet = dataSet;
            //Stake_ProcessPntList.sortGroup(ref varDataSet, "TYPE");
            Stake_ProcessPntList.addData(varDataSet, "SORT");
        }

        private void cmdRange_Click(object sender, System.EventArgs e)
        {
            List<DataSet> varDataSet = dataSet;
            //Stake_ProcessPntList.sortGroup(ref varDataSet, "RANGE");
            Stake_ProcessPntList.addData(varDataSet, "SORT");

        }

        private void cmdCreateCutSheet_Click(object sender, System.EventArgs e)
        {
            Stake_CutSheet.processCutsheetData("EXPORTPOINTS");
        }

        private void frmExport_FormClosed(object sender, FormClosedEventArgs e)
        {
            Stake_Forms.sForms.fStake.Show();
        }

        private void frmExport_Load(object sender, System.EventArgs e)
        {
            if (!SelectPoints.SelectPntsByAlign())
            {
                SelectPoints.SelectPntsByRange();
            }
        }

        private void frmExport_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}