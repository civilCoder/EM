using Autodesk.AutoCAD.EditorInput;
using Base_Tools45;
using System;
using System.Windows.Forms;
using App = Autodesk.AutoCAD.ApplicationServices.Application;


namespace EW.Forms
{
    public partial class frmBuildAreas : Form
    {
        public frmBuildAreas()
        {
            InitializeComponent();
        }

        private void cmdIdLayer_Click(System.Object eventSender, System.EventArgs eventArgs)
        {

            BaseObjs._acadDoc.SendStringToExecute("_xlist\r", true, false, false);

        }

        private void cmdSelectLayers2Freeze_Click(System.Object eventSender, System.EventArgs eventArgs)
        {

            BaseObjs._acadDoc.SendStringToExecute("_layfrz\r", true, false, false);

        }

        private void cmdBoundary_Click(System.Object eventSender, System.EventArgs eventArgs)
        {

            BaseObjs._acadDoc.SendStringToExecute("BOUNDARY\r", true, false, false);

        }

        private void cmdMakePolyline_Click(System.Object eventSender, System.EventArgs eventArgs)
        {

            BaseObjs._acadDoc.SendStringToExecute("PL\r", true, false, false);

        }

        private void cmdSAP_Click(System.Object eventSender, System.EventArgs eventArgs)
        {

            EW.EW_SetAreaProp.setAreaProp();

        }

        private void cmdCheckAreaLimits_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            SelectionSet objSSet = EW_Utility1.buildSSet0();
            EW.EW_CheckAreaLimits.checkAreaLimits(objSSet);

            objSSet = EW_Utility1.buildSSet_XX_OX();
            EW.EW_CheckAreaLimits.testAreaOffset(objSSet);

        }

        private void UserForm_Terminate()
        {
            App.ShowModelessDialog(EW.EW_Forms.ewFrms.fEarthwork);
        }

        private void frmBuildAreas_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            App.ShowModelessDialog(EW.EW_Forms.ewFrms.fEarthwork);
        }

        private void cmdTransfer_Click(object sender, EventArgs e)
        {
            EW_Transfer.transferObjs("AREAS");
        }
    }
}
