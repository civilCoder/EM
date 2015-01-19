using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

using Autodesk.Civil.DatabaseServices;

using Base_Tools45;
using Base_Tools45.C3D;

using System.Collections.Generic;

namespace Grading.Cmds
{
    public class cmdRDP
    {
        public static void RDP()
        {

            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;



            TypedValue[] tvs = new TypedValue[2] { new TypedValue(8, "*"), new TypedValue(0, "AECC_COGO_POINT") };

            SelectionFilter filter = new SelectionFilter(tvs);

            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = "\nSelect Points to Filter:";

            PromptSelectionResult psr = ed.GetSelection(pso, filter);
            if (psr.Status == PromptStatus.Cancel)
                return;

            SelectionSet ss = psr.Value;
            ObjectId[] ids = ss.GetObjectIds();
            List<ObjectId> idsPicked = new List<ObjectId>();
            for (int i = 0; i < ids.Length; i++)
                idsPicked.Add(ids[i]);

            List<ObjectId> idsCgPntsFound = new List<ObjectId>();

            int k = 0;
            foreach (ObjectId idPick in idsPicked)
            {
                if (!idPick.IsValid || idPick.IsEffectivelyErased || idPick.IsErased)
                    continue;

                Point3d pnt3dP = idPick.getCogoPntCoordinates();

                idsCgPntsFound = CgPnt.checkForCogoPointsByNode(pnt3dP);

                if (idsCgPntsFound.Count == 1)      //no duplicate - done with pick
                    continue;

                foreach (ObjectId idFound in idsCgPntsFound) //duplicates found
                {
                    if (idFound == idPick)      //we found all cogo points at pick xy including pick
                        continue;

                    ResultBuffer rbF = idFound.getXData(null);      
                    ResultBuffer rbP = idPick.getXData(null);

                    if (rbF == null)
                    {
                        idFound.delete();
                        k++;
                    }
                    else if (rbP == null)
                    {
                        idPick.delete();
                        k++;
                    }
                }
            }

            Application.ShowAlertDialog(string.Format("{0} Duplicated Point(s) Deleted", k));

        }

        public static List<ObjectId>
        getPointsAtXYLocation(Point3d pnt3d, SelectionFilter filter)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            List<ObjectId> ids = new List<ObjectId>();
            PromptSelectionResult psr = ed.SelectCrossingWindow(pnt3d, pnt3d, filter);
            if (psr.Status == PromptStatus.OK)
            {
                ObjectId[] idsArray = psr.Value.GetObjectIds();
                for (int i = 0; i < idsArray.Length; i++)
                    ids.Add(idsArray[i]);
            }
            return ids;
        }
    }
}
