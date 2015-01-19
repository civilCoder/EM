using AutoCAD;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Base_Tools45;
using Base_Tools45.C3D;
using Grading;

// This line is not mandatory, but improves loading performances
[assembly: CommandClass(typeof(EventManager.EM_Commands))]

namespace EventManager
{
    // This class is instantiated by AutoCAD for each document when
    // a command is called by the user the first time in the context
    // of a given document. In other words, non static data in this class
    // is per-document by default!
    public partial class EM_Commands
    {
        public EM_Commands()
        {
            Grading_CommandDefaults.initializeCommandDefaults();

            Pnt_Style.checkPntLabelStyles();
            foreach (CgPnt_Group.pntGroupParams pntGrpParams in CgPnt_Group.pntGroups)
            {
                if (pntGrpParams.name != "SPNT")
                    CgPnt_Group.checkPntGroup(pntGrpParams.name);

                Layer.manageLayer(pntGrpParams.name, pntGrpParams.color);
                Layer.manageLayer(pntGrpParams.nameLayerLabel, 7, pntGrpParams.layerOff, pntGrpParams.layerFrozen);
            }

            using (BaseObjs._acadDoc.LockDocument())
            {
                Layer.manageLayers("ARROW", 7);
                Layer.manageLayers("GRADES", 2);
            }

            Pub.Slope = 0.005;
        }

        private AcadApplication acadApp
        {
            get
            {
                return (AcadApplication)Autodesk.AutoCAD.ApplicationServices.Application.AcadApplication;
            }
        }

        private Document acadDoc
        {
            get
            {
                return Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            }
        }

        private Editor acadEditor
        {
            get
            {
                return acadDoc.Editor;
            }
        }

    }
}