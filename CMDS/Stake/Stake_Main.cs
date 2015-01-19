using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Stake
{
    public static partial class Stake_Main
    {
        

        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;
        private static Forms.frmGrid fGrid = Forms.Stake_Forms.sForms.fGrid;

        private static bool escape = false;
        private static PromptStatus ps;

        public static bool
        testClass(string strName, string strLayer)
        {
            if (fStake.ClassObj != strName)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(string.Format("Layer of Object selected is - {0} - and does not agree with Option selected", strLayer));
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void
        Process(string varObjName = "")
        {
            Object xRefPath = null;
            Entity obj = xRef.getEntity("\nSelect feature to stake: ", out escape, out xRefPath, out ps);
            fStake.XRefDbModelSpace = xRef.getXRefBlockTableRecordMS(xRefPath.ToString());
            ObjectId idAlign = ObjectId.Null;

            if (escape)
                return;

            ObjectId idPolyGuideline = ObjectId.Null;

            string nameLayer = "";

            switch (obj.GetType().Name)
            {
                case "Arc":
                case "Line":
                case "Polyline":
                case "Polyline2d":
                case "Polyline3d":

                    idPolyGuideline = Stake_GetGuidelines.getGuidelines(obj);

                    if (varObjName == "")
                    {
                        nameLayer = obj.Layer;
                        int intPos = nameLayer.IndexOf("|");
                        nameLayer = nameLayer.Substring(intPos + 1);

                        fStake.NameStakeObject = nameLayer;
                    }
                    else
                    {
                        nameLayer = "SIDEWALK";
                        fStake.ClassObj = "SIDEWALK";
                        fStake.NameStakeObject = "SIDEWALK";
                    }

                    if (nameLayer.Contains("CURB"))
                    {
                        if (!testClass("CURB", nameLayer))
                        {
                            return;
                        }
                    }
                    else if (nameLayer.Contains("FL"))
                    {
                        if (!testClass("FL", nameLayer))
                        {
                            return;
                        }
                    }
                    else if (nameLayer.Contains("FTG") || nameLayer.Contains("FOOTING"))
                    {
                        if (!testClass("FTG", nameLayer))
                        {
                            return;
                        }
                    }
                    else if (nameLayer.Contains("SEWER"))
                    {
                        if (!testClass("SEWER", nameLayer))
                        {
                            return;
                        }
                    }
                    else if (nameLayer.Contains("SD"))
                    {
                        if (!testClass("SD", nameLayer))
                        {
                            return;
                        }
                    }
                    else if (nameLayer.Contains("WALL"))
                    {
                        if (!testClass("WALL", nameLayer))
                        {
                            return;
                        }
                    }
                    else if (nameLayer.Contains("WATER") || nameLayer.Contains("WAT") || nameLayer.Contains("WTR") || nameLayer.Contains("FIRE") ||
                             nameLayer.Contains("VERIZON") || nameLayer.Contains("ELEC") || nameLayer.Contains("CABLE") ||
                             nameLayer.Contains("PHONE") || nameLayer.Contains("TELE"))
                    {
                        if (!testClass("WTR", nameLayer))
                        {
                            return;
                        }
                    }

                    processAlign(nameLayer, idPolyGuideline);

                    break;

                case "DbAlignment":

                    List<POI> varPOI = new List<POI>();
                    Alignment objAlign = (Alignment)obj;
                    idAlign = objAlign.ObjectId;

                    fStake.ACTIVEALIGN = objAlign;
                    nameLayer = string.Format("STAKE-{0}", objAlign.Name);
                    Layer.manageLayers(nameLayer);
                    idAlign.changeProp(nameLayer: nameLayer);

                    fStake.HandleAlign = objAlign.Handle;
                    fStake.objectID = idAlign;

                    fStake.NameStakeObject = objAlign.Name;

                    if (objAlign.Name.Contains("WALL"))
                    {
                        fStake.ClassObj = "WALL";
                    }
                    else
                    {
                        fStake.ClassObj = "ALIGN";
                    }

                    switch (fStake.ClassObj)
                    {
                        case "ALIGN":

                            Stake_GetCardinals.getCardinals_Horizontal(idAlign, ref varPOI);

                            Stake_GetNestedObjects.getNestedPoints(idAlign, ref varPOI, fStake.XRefDbModelSpace, Path.GetFileName(xRefPath.ToString()));

                            Stake_GetCardinals.getCardinals_Vertical(idAlign, ref varPOI);

                            //        Call getNestedObjects(objAlign, varPOI)

                            Stake_GetCardinals.getCrossingAligns(idAlign, ref varPOI);

                            Stake_AddProfile.makeProfile(idAlign, varPOI, "STAKE", "ByLayout", false);

                            Stake_GetBC_EC.getBC_EC(idAlign, ref varPOI);

                            Stake_DuplicateStations.resolveDuplicateStations(ref varPOI);

                            Stake_UpdateProfile.updateProfile(idAlign, varPOI, "STAKE", false, "STAKE");
                            Stake_UpdateProfile.updateProfile(idAlign, (fStake.POI_PNTs), "STAKE", true, "PNTS");

                            ResultBuffer rb = xRef.getXRefsContainingTargetDwgName("GCAL");
                            if (rb == null)
                            {
                                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("GCAL drawing not attached");
                                return;
                            }

                            TypedValue[] tvs = rb.AsArray();
                            string nameXRef = tvs[0].Value.ToString();
                            BlockTableRecord ms = xRef.getXRefBlockTableRecordMS(nameXRef);

                            //        If getNestedPoints(objAlign, varPOI, objXRefDbModelSpace, "GCAL") = False Then
                            //          Exit Sub
                            //        End If

                            Debug.Print("After getNestedPoints");
                            for (int i = 0; i < varPOI.Count; i++)
                            {
                                Debug.Print(varPOI[i].Station + "  " + varPOI[i].Desc0);
                            }

                            //    Call makeProfileINVERT(objAlign, varPOI)

                            fStake.POI_CALC = varPOI;
                            //#############################################################

                            ObjectId idTable = Stake_Table.makeTable(idAlign, varPOI);
                            Stake_Table.addTableData(idTable, varPOI);

                            break;

                        case "WALL":

                            Stake_Wall.processWall(idAlign);

                            break;
                    }
                    break;

                default:
                    break;
            }

            if (fStake.ClassObj != "BLDG")
            {
                TypedValue[] tvs = new TypedValue[2] { new TypedValue(1001, "CLASS"), new TypedValue(1000, fStake.ClassObj) };
                idAlign.setXData(tvs, "CLASS");
            }
        }
    }
}