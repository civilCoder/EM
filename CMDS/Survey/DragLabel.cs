using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;

using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;

using Base_Tools45;
[assembly: CommandClass(typeof(DragLabels.DragLabel))]

namespace DragLabels
{
    public class DragLabel
    {
       [CommandMethod("DRG",CommandFlags.Session)]
        public void dragLabel()
       {
           Document acadDoc = Application.DocumentManager.MdiActiveDocument;
           CivilDocument civDoc = CivilApplication.ActiveDocument;

            SelectionSet ss = selectPoints("\nSelect Points on Screen.");
            ObjectId[] ids = ss.GetObjectIds();

           using (Transaction TR = acadDoc.Database.TransactionManager.StartTransaction())
           {
                foreach (ObjectId id in ids)
                {
                    CogoPoint pnt = (CogoPoint)TR.GetObject(id, OpenMode.ForWrite);                    
                }         
           }
       }

       public SelectionSet selectPoints(string message)
       {
           Document DOC = Application.DocumentManager.MdiActiveDocument;
           Database DB = DOC.Database;
           Editor ED = Application.DocumentManager.MdiActiveDocument.Editor;

           TypedValue[] TVs = new TypedValue[1];
           TVs.SetValue(new TypedValue((int)DxfCode.Start, "AECC_COGO_POINT"), 0);

           SelectionFilter FILTER = new SelectionFilter(TVs);
           PromptSelectionOptions PSO = new PromptSelectionOptions();
           PSO.MessageForAdding = message;
           
           PromptSelectionResult PSR = null;

           using (Transaction TR = DB.TransactionManager.StartTransaction())
           {
               try
               {
                   PSR = ED.GetSelection(PSO,FILTER);
               }
               catch (System.Exception ex){
                   Application.ShowAlertDialog(ex.Message + " DragLabel.cs: line: 73");
                   BaseObjs.writeDebug(ex.Message + " DragLabel.cs: line: 73");
               }
               if (PSR.Status == PromptStatus.OK)
               {
                   return PSR.Value;
               }
           }
           return null;
       } // selectPoints
    }
}
