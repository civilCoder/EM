using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

using Autodesk.Civil.DatabaseServices;

using Base_Tools45;
using Base_Tools45.C3D;

using System;
using System.Collections.Generic;

namespace Grading.Cmds
{
    public static class cmdMCE
    {
        public static void
        MCE(List<ObjectId> idsPnts = null, string result = "")
        {
            double elev = 0.0;

            if (idsPnts == null)
            {
                idsPnts = new List<ObjectId>();
                Type type;
                TypedValue[] tvs = new TypedValue[5];
                tvs.SetValue(new TypedValue((int)DxfCode.Operator, "<OR"), 0);

                type = typeof(Polyline3d);
                tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(type).DxfName), 1);

                type = typeof(CogoPoint);
                tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(type).DxfName), 2);

                type = typeof(Polyline);
                tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(type).DxfName), 3);

                tvs.SetValue(new TypedValue((int)DxfCode.Operator, "OR>"), 4);

                SelectionSet ss = Select.buildSSet(tvs, false);
                if (ss == null)
                    return;
                ObjectId[] ids = ss.GetObjectIds();

                List<string> descPnts = new List<string>();
                string descPnt = "";
                try
                {
                    if (ss.Count == 1)
                    {
                        string t = ids[0].getType().ToUpper();
                        if (t == "POLYLINE" || t == "POLYLINE2D" || t == "POLYLINE3D")
                        {
                            Point3dCollection pnts3d = ids[0].getCoordinates3d();
                            tvs = new TypedValue[1];
                            tvs.SetValue(new TypedValue((int)DxfCode.Start, "AECC_COGO_POINT"), 0);
                            SelectionFilter filter = new SelectionFilter(tvs);
                            PromptSelectionResult psr = BaseObjs._editor.SelectCrossingPolygon(pnts3d, filter);
                            if (psr.Status == PromptStatus.OK)
                            {
                                ids = psr.Value.GetObjectIds();
                                foreach (ObjectId id in ids)
                                {
                                    idsPnts.Add(id);
                                    descPnt = id.getCogoPntDesc();
                                    if (!descPnts.Contains(descPnt))
                                        descPnts.Add(descPnt);
                                }
                            }
                            else
                                return;
                        }
                        else{
                            idsPnts.Add(ids[0]);
                            string message = string.Format("\nElev = {0:F2}", ids[0].getCogoPntElevation());
                            BaseObjs._editor.WriteMessage(message);                            
                        }

                    }
                    else if (ss.Count > 1)
                    {
                        foreach (ObjectId id in ids)
                        {
                            if (id.getType().ToUpper() == "COGOPOINT")
                            {
                                idsPnts.Add(id);
                                descPnt = id.getCogoPntDesc();
                                if (!descPnts.Contains(descPnt))
                                    descPnts.Add(descPnt);
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(ex.Message + " cmdMCE.cs: line: 98");
                }

                int len = descPnts.Count;
                bool escape = true;

                if (len > 1)
                {
                    string prompt = "";
                    string keyWords1 = "";      // not used - couldn't
                    string keyWords2 = "";
                    string desWords = "";
                    string desWords2 = "";

                    try
                    {
                        List<string> nameGrp = new List<string>();
                        List<CgPnt_Group.pntGroupParams> pntGroups = new List<CgPnt_Group.pntGroupParams>();
                        CgPnt_Group.pntGroupParams pntGrpSymCol;
                        for (int i = 0; i < len; i++)
                        {
                            pntGroups.Add(CgPnt_Group.pntGroups.Find(s => s.name == descPnts[i]));
                        }

                        foreach (CgPnt_Group.pntGroupParams p in pntGroups)
                        {
                            keyWords1 = string.Format("{0}{1} ", keyWords1, p.key);
                            desWords = string.Format("{0}{1} ", desWords, p.name);
                        }

                        keyWords1 = keyWords1.Trim();
                        keyWords2 = keyWords1.Replace(" ", "/");
                        desWords2 = desWords.Trim();

                        keyWords2 = string.Format("[{0}]", keyWords2);
                        desWords = string.Format("[{0}]", desWords2);

                        prompt = string.Format("\n** WARNING ** Multiple Point Groups in Selection: {0} Found!", desWords);
                        BaseObjs.write(prompt);

                        desWords = desWords.Replace(" ", "/");

                        string resDesc = pntGroups[0].name;
                        string resKey = pntGroups[0].key;

                        pntGrpSymCol = CgPnt_Group.pntGroups.Find(s => s.name == resDesc);

                        prompt = string.Format("\nSelect Target Point Group from List: {0} <{1}> {2}", desWords, pntGrpSymCol.name, desWords);

                        escape = UserInput.getUserInputKeyword(resDesc, out resDesc, prompt, desWords2);

                        pntGrpSymCol = CgPnt_Group.pntGroups.Find(s => s.name == resDesc);

                        List<ObjectId> idsT = new List<ObjectId>();
                        foreach (ObjectId id in idsPnts)
                        {
                            if (id.getCogoPntDesc() == pntGrpSymCol.name)
                                idsT.Add(id);
                        }
                        idsPnts = idsT;
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(ex.Message + " cmdMCE.cs: line: 161");
                    }
                }

                escape = UserInput.getUserInput("\nEnter new elevation (or +/- change) for points: ", out result, false);

                if (!escape)
                {
                    if (!double.TryParse(result.Substring(0, 1), out elev))
                    {
                        if (result.Substring(0, 1) != "+" && result.Substring(0, 1) != "-")
                        {
                            Application.ShowAlertDialog("Error in input.  Please try again...");
                            return;
                        }
                    }
                }
            }


            List<ObjectId> idsPoly3dSLP = new List<ObjectId>();
            try
            {
                foreach (ObjectId id in idsPnts)
                {
                    double elev0 = id.getCogoPntElevation();
                    elev = id.setPointElevation(result);
                    ResultBuffer rb = id.getXData(null);
                    if (rb != null)
                    {
                        TypedValue[] tvsAll = rb.AsArray();
                        List<string> nameApps = null;
                        List<TypedValue[]> lstTVs = xData.parseXData(tvsAll, out nameApps);

                        foreach (TypedValue[] tvsPnt in lstTVs)
                        {
                            string nameApp = tvsPnt[0].Value.ToString();
                            switch (nameApp)
                            {
                                case apps.lnkBrks:
                                case apps.lnkBrks2:
                                    Grading_Utility.objPnt_Modified(id, nameApp, ref idsPoly3dSLP);               //update breaklines
                                    break;

                                case apps.lnkCO:
                                    Grading_Utility.updatePntCalloutElev(elev, tvsPnt);          //update callouts
                                    break;

                                case apps.lnkDP:
                                    if (tvsPnt[2].Value.ToString() == "Primary")
                                    {
                                        Grading_Utility.updatePntCalloutElev(elev, tvsPnt);     //update callouts
                                    }
                                    break;

                                case apps.lnkGS:
                                    System.Diagnostics.Debug.Print(id.getHandle().ToString());
                                    Grading_Utility.updateGS(tvsPnt, "cmdMCE");                  //update GS0
                                    break;

                                case apps.lnkMNP:
                                    double dZ = elev - elev0;
                                    PipeNetwork.updatePipes(dZ, elev0, id);
                                   
                                    break;
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdMCE.cs: line: 233");
            }

            if (idsPoly3dSLP.Count > 0)
            {
                foreach (ObjectId idPoly3dSLP in idsPoly3dSLP)
                    Grading_UpdateSG.updateSG(idPoly3dSLP);
            }
        }
    }
}
