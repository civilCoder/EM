using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Base_Tools45;
using System.Collections.Generic;

namespace Grading.Cmds
{
    public static partial class cmdUC1
    {
        public static void
        executeCmdUC(List<string> layers)
        {
            myForms.updateCNTL fmUpdateCNTL = Grading_Palette.gPalette.pUpdateCNTL;

            string nameTAR = fmUpdateCNTL.TARGETDWGNAME;
            string pathTAR = fmUpdateCNTL.TARGETDWGPATH;
            string path_nameTAR = string.Format("{0}{1}{2}", pathTAR, @"\", nameTAR);

            int k = layers.Count;

            TypedValue[] TVs = new TypedValue[k + 2];
            TVs.SetValue(new TypedValue((int)DxfCode.Operator, "<OR"), 0);
            for (int i = 0; i < k; i++)
            {
                TVs.SetValue(new TypedValue((int)DxfCode.LayerName, layers[i]), i + 1);
            }
            TVs.SetValue(new TypedValue((int)DxfCode.Operator, "OR>"), k + 1);
            SelectionSet ss = Base_Tools45.Select.buildSSet(TVs, true);
            if (ss == null)
            {
                Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog("There are no targeted objects as this time.  Exiting...");
                return;
            }
            ObjectId[] idsSS = ss.GetObjectIds();


            Document acDoc = BaseObjs._acadDoc;
            Database acDbCUR = acDoc.Database;

            Document acDocTAR = FileManager.getDoc(path_nameTAR, Grading_Palette.gPalette.pGrading.isOpen);
            if (acDocTAR == null)
                return;
            Database acDbTAR = acDocTAR.Database;

            bool success = FileManager.transferObjects(idsSS, acDbCUR, acDbTAR);
            if (success)
            {
                acDocTAR.CloseAndSave(path_nameTAR);
                Application.DocumentManager.MdiActiveDocument = acDoc;
                using (BaseObjs._acadDoc.LockDocument())
                {
                    foreach (ObjectId id in idsSS)
                        try
                        {
                            id.delete();
                        }
                        catch (System.Exception ex)
                        {
                            BaseObjs.writeDebug(ex.Message + " cmdUC2.cs: line: 60");
                        }
                }
            }
            Grading_Palette.setfocus("Grading Tools");
        }
    }
}
