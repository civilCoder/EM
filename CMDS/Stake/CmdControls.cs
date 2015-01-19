using Base_Tools45;
using System.Collections.Generic;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;


namespace Stake
{
    public class CmdControls
    {
        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;
        private static Forms.frmPoints fPoints = Forms.Stake_Forms.sForms.fPoints;
        private static Forms.frmExport fExport = Forms.Stake_Forms.sForms.fExport;

        public CheckBox chkBox1;
        public System.Windows.Forms.Label lblObject;
        public System.Windows.Forms.Label lblNums;
        public System.Windows.Forms.Label lblLowerA;
        public System.Windows.Forms.Label lblUpperA;
        public System.Windows.Forms.Label lblToA;
        public System.Windows.Forms.CheckBox chkBox2;
        public System.Windows.Forms.TextBox tBxLowerB;
        public System.Windows.Forms.TextBox tBxUpperB;
        public System.Windows.Forms.Label lblToB;
        public System.Windows.Forms.Button cmdBtn;

        private void cmdBtn_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            System.Windows.Forms.ListBox lbxPnts = new ListBox();

            List<uint> lngPnts = null;

            lbxPnts = fPoints.lbxPoints;

            List<DataSet> vDataSet;
            List<DataSet> vDataSum;

            int intIndex = int.Parse(cmdBtn.Text.Substring(15));

            switch (cmdBtn.Text.Substring(11, 3))
            {
                case "SRT":

                    vDataSet = fExport.dataSet;
                    lngPnts = vDataSet[intIndex].Nums;

                    break;

                case "SUM":

                    vDataSum = fExport.dataSum;
                    lngPnts = vDataSum[intIndex].Nums;

                    break;
            }

            int intUBnd = lngPnts.Count;

            for (int i = 0; i < intUBnd; i++)
            {
                lbxPnts.Items.Add(lngPnts[i]);
            }

            if (intUBnd == 0)
            {
                lbxPnts.Height = 2 * 18;
                fPoints.Height = 2 * 18 + 72;
            }

            if (intUBnd < 30)
            {
                lbxPnts.Height = (intUBnd + 1) * 18;
                fPoints.Height = (intUBnd + 1) * 18 + 72;
            }

            if (intUBnd > 30)
            {
                var _with1 = lbxPnts;
                _with1.IntegralHeight = false;
                _with1.Height = 30 * 18;
                _with1.IntegralHeight = true;
                _with1.SelectionMode = SelectionMode.MultiExtended;

                fPoints.Height = 30 * 18 + 72;
            }

            fPoints.cmdCreateCutsheet.Top = fPoints.Height - 60;
            Application.ShowModelessDialog(Application.MainWindow.Handle, fPoints, false);
        }
    }
}