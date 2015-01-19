using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Base_Tools45;
using Bubble;
using DimPL;
using LdrText;
using System;
using System.Collections.Generic;

[assembly: ExtensionApplication(typeof(EventManager.EM_Startup))]

namespace EventManager
{
    /// <summary>
    /// EM_Startup.
    /// </summary>
    public sealed class EM_Startup : IExtensionApplication
    {
        //Base_Tools45.Startup            b0 = new Base_Tools45.Startup();
        //Base_VB.Startup                 b1 = new Base_VB.Startup();
        //Bubble.BB_Startup               b2 = new BB_Startup();
        //ConvertToXData.CXD_Startup      b3 = new ConvertToXData.CXD_Startup();
        //Cout.CO_Startup                 b4 = new Cout.CO_Startup();
        //DimPL.DimPL_Startup             b5 = new DimPL_Startup();
        //EW.EW_Startup                   b6 = new EW.EW_Startup();
        //fixGradeTagLinks.Startup        b7 = new fixGradeTagLinks.Startup();
        //Grading.Grading_Startup         b8 = new Grading.Grading_Startup();
        //LabelContours.LC_Startup        b9 = new LabelContours.LC_Startup();
        //LdrText.LdrText_Startup         b10 = new LdrText_Startup();
        //MNP.MNP_Startup                 b11 = new MNP.MNP_Startup();
        //PadCerts.PC_Startup             b12 = new PadCerts.PC_Startup();
        //ProcessFigures.PF_Startup       b13 = new ProcessFigures.PF_Startup();
        //ProcessPointFile.PPF_Startup    b14 = new ProcessPointFile.PPF_Startup();
        //SDrain.SD_Startup               b15 = new SDrain.SD_Startup();
        //slopeGrading.SG_Startup         b16 = new slopeGrading.SG_Startup();
        //Stake.Stake_Startup             b17 = new Stake.Stake_Startup();
        //Survey.Survey_Startup           b18 = new Survey.Survey_Startup();
        //Wall.Wall_Startup               b19 = new Wall.Wall_Startup();
        //WSP.WSP_Startup                 b20 = new WSP.WSP_Startup();




        public static EM_Output outForm;

        private Grading.Grading_Palette gPalette = Grading.Grading_Palette.gPalette;

        private static Grading.myForms.GradeSite fGrading;
        private static Grading.myForms.GradeFloor fGradeFloor;

        private DocumentCollection m_docMan = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager; //executes first
        private Document activeDoc = null;

        private List<string> log = new List<string>();

        public EM_Startup()
        {
            EM_Helper emHelper = new EM_Helper();
            EM_ContextMenu0.replaceTextEditContextMenu();

            Layer.manageLayers("BUBBLE", 5);
        }

        public void Terminate()
        {
            EM_ContextMenuExtensions.RemoveMe();
        }

        public void Initialize()
        {
            try
            {
                log.Add("Initialize");
                activeDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                activeDoc.Editor.WriteMessage("\nEventManager.dll Loaded");

                Autodesk.AutoCAD.ApplicationServices.Core.Application.SystemVariableChanged += callback_SystemVariableChanged;

                //EM_DocActivated.DocumentActivated += callBack_DocumentActivated;

                EM_ContextMenuExtensions.AddMe();
                if (BaseObjs.userName.ToUpper() == "JOHN")
                {
                    EM_ContextMenuExtensions.showOutput();
                }

                EM_EventsWatcher eventWatcher = new EM_EventsWatcher();

                log.Add("EM_Startup");

                gPalette.showPalettes(false);

                try
                {
                    Grading.Grading_CommandDefaults.initializeCommandDefaults();
                }
                catch (System.Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message + "EM_Startup");
                }

                fGrading = gPalette.pGrading;
                fGrading.Initialize_Form();

                fGradeFloor = gPalette.pGradeFloor;
                fGradeFloor.Initialize_Form();
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Startup.cs: line: 77");
            }
        }

        private void callback_SystemVariableChanged(Object sender, Autodesk.AutoCAD.ApplicationServices.SystemVariableChangedEventArgs e)
        {
            if (e.Name == "CANNOSCALE")
            {
                int scale = (int)HostApplicationServices.WorkingDatabase.Cannoscale.DrawingUnits;
                if (scale != Pub.DrawingUnits)
                {
                    Pub.DrawingUnits = scale;
                    LdrText_Scale.scaleLdrText(scale);
                    BB_LnkBub_Scale.updateSymbols(scale);
                    DimPL_Scale.scaleDimPL(scale);
                }
            }
        }
    }
}
