using Autodesk.AutoCAD.DatabaseServices;
using Base_Tools45;
using System.Collections.Generic;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace Stake
{
    public static partial class Stake_Main
    {
        private static void
        processAlign(string nameLayer, ObjectId idPolyGuideline)
        {
            BlockTableRecord ms = null;
            List<POI> varPOI = new List<POI>();
            ObjectId idTable = ObjectId.Null;
            ObjectId idAlign = ObjectId.Null;
            string nameXRef = "";
            ResultBuffer rb;
            TypedValue[] tvs;

            switch (fStake.ClassObj)
            {
                case "BLDG":

                    Stake_GetNestedObjects.copyGRID((fStake.XRefDbModelSpace)); //XrefDbModelSpace is source of grid selected
                    idPolyGuideline.delete();
                    fStake.Hide();
                    Application.ShowModelessDialog(Application.MainWindow.Handle, fGrid, false);

                    break;

                case "CURB":
                    rb = xRef.getXRefsContainingTargetDwgName("GCAL");
                    if (rb == null)
                    {
                        Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("GCAL drawing not attached");
                        return;
                    }

                    tvs = rb.AsArray();
                    nameXRef = tvs[0].Value.ToString();
                    ms = xRef.getXRefBlockTableRecordMS(nameXRef);

                    processCurb(nameLayer, idPolyGuideline, ms, nameXRef, ref varPOI, out idAlign);

                    idTable = Stake_Table.makeTable(idAlign, varPOI);
                    Stake_Table.addTableData(idTable, varPOI);

                    break;

                case "FL":
                    rb = xRef.getXRefsContainingTargetDwgName("GCAL");
                    if (rb == null)
                    {
                        Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("GCAL drawing not attached");
                        return;
                    }

                    tvs = rb.AsArray();
                    nameXRef = tvs[0].Value.ToString();
                    ms = xRef.getXRefBlockTableRecordMS(nameXRef);

                    processFlowline(nameLayer, idPolyGuideline, ms, nameXRef, ref varPOI, out idAlign);

                    //        fStake.POI = varPOI

                    idTable = Stake_Table.makeTable(idAlign, varPOI);
                    Stake_Table.addTableData(idTable, varPOI);

                    break;

                case "SEWER":
                    rb = xRef.getXRefsContainingTargetDwgName("UTIL");
                    if (rb == null)
                    {
                        Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("UTIL drawing not attached");
                        return;
                    }

                    tvs = rb.AsArray();
                    nameXRef = tvs[0].Value.ToString();
                    ms = xRef.getXRefBlockTableRecordMS(nameXRef);

                    processSewer(nameLayer, idPolyGuideline, ms, nameXRef, ref varPOI, out idAlign);

                    //    Call makeProfileINVERT(objAlign, varPOI)

                    //        fStake.POI = varPOI

                    idTable = Stake_Table.makeTable(idAlign, varPOI);
                    Stake_Table.addTableData(idTable, varPOI);

                    break;

                case "WTR":

                    processWater(nameLayer, idPolyGuideline, ms, ref varPOI, out idAlign);

                    idTable = Stake_Table.makeTable(idAlign, varPOI);
                    Stake_Table.addTableData(idTable, varPOI);

                    break;
            }

            fStake.POI_CALC = varPOI;
            //#############################################################
        }
    }
}