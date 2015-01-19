using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Base_Tools45;
using Base_VB;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;


namespace Grading.myForms
{
    /// <summary>
    /// Interaction logic for ucMain.xaml
    /// </summary>
    public partial class GradeSite : UserControl
    {
        #region "Initialize"

        public GradeSite()
        {
            InitializeComponent();
        }

        public void
        Initialize_Form()
        {
            ResultBuffer rb;
            Document doc = Application.DocumentManager.MdiActiveDocument;

            using (doc.LockDocument())
            {
                Layer.manageLayers("BRKLINE ERRORS");
                Layer.manageLayers("CPNT-BRKLINE");
                Layer.manageLayers("CPNT-ON");
                Layer.manageLayers("CURB-TEMP");
                Layer.manageLayers("FL");
                Layer.manageLayers("GB");
                Layer.manageLayers("GUTTER");
            }

            Application.SetSystemVariable("PICKFIRST", 1);
            gintCmdStatus = 1;

            TARGETDWGNAME = "";
            TARGETDWGPATH = "";

            using (BaseObjs._acadDoc.LockDocument())
            {
                bool exists = false;
                //try
                //{
                //    Dict.getNamedDictionary(lnkBrks3, out exists);
                //}
                //catch
                //{
                //}

                ObjectId idDict = ObjectId.Null;
                try
                {
                    idDict = Dict.getNamedDictionary("LASTBRKLINE", out exists);
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(string.Format("{0} ucGrading.xaml.cs: line: 57", ex.Message));
                }
                if (exists)
                {
                    string sHandle = myUtility.getXrec0("LASTBRKLINE");
                    if (!string.IsNullOrEmpty(sHandle))
                    {
                        this.HandleLastBrkLine = Db.stringToHandle(sHandle);                        
                    }else{
                        this.HandleLastBrkLine = "0000".stringToHandle();
                    }

                }
                else
                {
                    rb = null;
                    ObjectIdCollection ids = BaseObjs._acadDoc.getBrkLines();
                    Handle handle;
                    if (ids != null && ids.Count > 0)
                    {
                        try
                        {
                            handle = mySurfaces.cleanUpBreaklines().stringToHandle();
                            rb = new ResultBuffer(new TypedValue((int)DxfCode.Handle, handle));
                            Dict.addXRec(idDict, "LASTBRKLINE", rb);
                            this.HandleLastBrkLine = handle;
                            Grading.Cmds.cmdCPXD.checkPntXData();
                        }
                        catch (System.Exception ex)
                        {
                            BaseObjs.writeDebug(string.Format("{0} ucGrading.xaml.cs: line: 75", ex.Message));
                        }
                    }
                }

                TypedValue[] TVs = new TypedValue[5];
                TVs.SetValue(new TypedValue((int)DxfCode.Operator, "<OR"), 0);
                TVs.SetValue(new TypedValue((int)DxfCode.Start, "LINE"), 1);
                TVs.SetValue(new TypedValue((int)DxfCode.Start, "POLYLINE"), 2);
                TVs.SetValue(new TypedValue((int)DxfCode.Operator, "OR>"), 3);
                TVs.SetValue(new TypedValue((int)DxfCode.LayerName, "CPNT-BRKLINE*"), 4);

                SelectionSet ss = Select.buildSSet(TVs);

                if (ss != null)
                {
                    try
                    {
                        using (Transaction tr = BaseObjs.startTransactionDb())
                        {
                            ObjectId[] ids = ss.GetObjectIds();
                            foreach (ObjectId id in ids)
                            {
                                if (id.getType() == "Line")
                                    id.delete();
                                else if (id.getType() == "Polyline3d")
                                {
                                    Polyline3d poly3d = (Polyline3d)tr.GetObject(id, OpenMode.ForRead);
                                    if (poly3d.Length == 0)
                                    {
                                        id.delete();
                                    }
                                }
                            }
                            tr.Commit();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} ucGrading.xaml.cs: line: 107", ex.Message));
                    }
                }

                //------------------------------------------------------------------

                optFL.IsChecked = true;
                optPNTS.IsChecked = true;

                if (System.Environment.UserName.ToUpper() == "JOHN")
                    cmdPD.IsEnabled = true;
                else
                    cmdPD.IsEnabled = false;               

                Pub.Slope = Double.Parse(cmdRL_GRADE);
            }
        }

        #endregion "Initialize"

        # region Properties

        public bool isOpen                  { get; set; }
        public bool BrkLines2Dwg            { get; set; }
        public bool BrkLines2Surface        { get; set; }
        public Nullable<bool> dialogResult  { get; set; }        
        
        public object XRefDbModelSpace      { get; set; }
        public object XRefObject            { get; set; }
        
        public int gintCmdStatus            { get; set; }
        
        public string cmdFL_resBot          { get; set; }
        public string cmdG_resBot           { get; set; }
        public string cmdG_resTop           { get; set; }
        public string cmdG_resCurb          { get; set; }
        public string cmdLD_resBot          { get; set; }
        public string cmdLD_resTop          { get; set; }
        public string cmdG_Curb             { get; set; }
        public string cmdRTr_Default        { get; set; }
        public string cmdRTr_Slope          { get; set; }
        public string cmdRTr_DeltaZ         { get; set; }
        public string cmdRTd_Default        { get; set; }
        public string cmdRTd_Distance       { get; set; }
        public string cmdRTd_Elevation      { get; set; }
        public string cmdRTd_Slope          { get; set; }
        public string cmdRL_Default         { get; set; }
        public string cmdRL_GRADE           { get; set; }
        public string cmdRL_DELTAZ          { get; set; }
        public string BlockXRefName         { get; set; }
        public string cmdBV_GutterWidth     { get; set; }
        public string cmdBV_GutterDepth     { get; set; }
        public string cmdBV_Source          { get; set; }
        public string cmdBV_Control         { get; set; }
        public string TARGETDWGPATH         { get; set; }
        public string TARGETDWGNAME         { get; set; }
        public List<string> LayerList       { get; set; }

        public Handle HandleLastBrkLine     { get; set; }
        public Database XRefDb              { get; set; }
        public ObjectId idBlockRef          { get; set; }
        
        #endregion

        #region "Controls"

        private void optPNTS_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (optPNTS.IsChecked == true)
            {
                cmdBV_Source = "POINTS";
                Dict.setCmdDefault("cmdBV", "SOURCE", "POINTS");
            }
        }

        private void optBRKLINE_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (optBRKLINE.IsChecked == true)
            {
                cmdBV_Source = "BRKLINE";
                Dict.setCmdDefault("cmdBV", "SOURCE", "BRKLINE");
            }
        }

        private void optFL_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (optFL.IsChecked == true)
            {
                cmdBV_Control = "FL";
                Dict.setCmdDefault("cmdBV", "CONTROL", "FL");
            }
        }

        private void optEDGE_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (optEDGE.IsChecked == true)
            {
                cmdBV_Control = "EDGE";
                Dict.setCmdDefault("cmdBV", "CONTROL", "EDGE");
            }
        }

        #endregion

        #region "Commands"

        private void cmdRT_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            BaseObjs._acadDoc.SendStringToExecute("RT\r", true, false, false);
        }

        private void cmdRL_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (BaseObjs._acadDoc.LockDocument())
            {
                BaseObjs.acadActivate();
                BaseObjs._acadDoc.SendStringToExecute("RL\r", true, false, false);
            }
        }

        private void cmdRTr_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            BaseObjs._acadDoc.SendStringToExecute("RTr\r", true, false, false);
        }

        private void cmdRTd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            BaseObjs._acadDoc.SendStringToExecute("RTd\r", true, false, false);
        }

        private void cmdMB_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            BaseObjs._acadDoc.SendStringToExecute("MB\r", true, false, false);
        }

        private void cmdBL_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            BaseObjs._acadDoc.SendStringToExecute("BL\r", true, false, false);
        }

        private void cmdGB_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            BaseObjs._acadDoc.SendStringToExecute("GB\r", true, false, false);
        }

        private void cmdMBL_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            BaseObjs._acadDoc.SendStringToExecute("MBL\r", true, false, false);
        }

        private void cmdBV_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            BaseObjs._acadDoc.SendStringToExecute("BV\r", true, false, false);
        }

        private void cndBG_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            BaseObjs._acadDoc.SendStringToExecute("BG\r", true, false, false);
        }

        private void cmdBC_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            BaseObjs._acadDoc.SendStringToExecute("BC\r", true, false, false);
        }

        private void cmdUC_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            BaseObjs._acadDoc.SendStringToExecute("UC\r", true, false, false);
        }

        private void cmdBFL_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            BaseObjs._acadDoc.SendStringToExecute("BFL\r", true, false, false);
        }

        private void cmdRB_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            BaseObjs._acadDoc.SendStringToExecute("RB\r", true, false, false);
        }

        private void cmdRB0_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            BaseObjs._acadDoc.SendStringToExecute("RB0\r", true, false, false);
        }

        private void cmdUBP_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            BaseObjs._acadDoc.SendStringToExecute("UBP\r", true, false, false);
        }

        private void cmdRBL_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            BaseObjs._acadDoc.SendStringToExecute("RBL\r", true, false, false);
        }

        private void cmdDBL_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            BaseObjs._acadDoc.SendStringToExecute("DBL\r", true, false, false);
        }

        private void cmdPD_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            Grading_Dict.printPntDictionary();
        }

        private void cmdGBM_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            BaseObjs._acadDoc.SendStringToExecute("GBM\r", true, false, false);
        }

        private void cmdABC_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            BaseObjs._acadDoc.SendStringToExecute("ABC\r", true, false, false);
        }

        private void cndABG_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BaseObjs.acadActivate();
            BaseObjs._acadDoc.SendStringToExecute("ABG\r", true, false, false);
        }
        #endregion
    }
}