using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;
using Point3d = Autodesk.AutoCAD.Geometry.Point3d;

namespace Grading.Cmds
{
    public static class cmdMCG
    {
        public static void
        MCG()
        {
            object dynMode = null;
            object dynPrompt = null;
            object osMode = null;
            try
            {
                dynMode = Application.GetSystemVariable("DYNMODE");
                dynPrompt = Application.GetSystemVariable("DYNPROMPT");

                Application.SetSystemVariable("DYNMODE", 3);
                Application.SetSystemVariable("DYNPROMPT", 1);

                osMode = SnapMode.getOSnap();
                SnapMode.setOSnap((int)osModes.NOD);

                string descVal = "", descKey = "";
                Point3d pnt3dPick;
                PromptStatus ps;
                CogoPoint cgPnt;
                Entity ent = Select.selectEntity(typeof(CogoPoint), "\nSelect a point on the desired group or <CR> for Menu: ", "", out pnt3dPick, out ps);
                short color = 256;
                string prompt = "";
                TypedValue[] tvs;

                switch (ps)
                {
                    case PromptStatus.OK:
                        cgPnt = (CogoPoint)ent;
                        descVal = cgPnt.RawDescription;
                        break;

                    case PromptStatus.Cancel:
                        return;

                    case PromptStatus.None:
                        bool exists;
                        ObjectId idDict = Dict.getNamedDictionary("cmdMCG", out exists);
                        if (exists)
                        {
                            ResultBuffer rb = Dict.getXRec(idDict, "defaultCmd");
                            if (rb == null)
                                descVal = "CPNT-ON";
                            else
                            {
                                tvs = rb.AsArray();
                                descVal = tvs[0].Value.ToString();
                            }
                        }
                        else
                        {
                            descVal = "CPNT-ON";
                        }
                        descKey = "O";
                        foreach (CgPnt_Group.pntGroupParams pntGrp in CgPnt_Group.pntGroups)
                        {
                            if (pntGrp.name == descVal)
                            {
                                descKey = pntGrp.key;
                                break;
                            }
                        }

                        bool escape;
                        prompt = string.Format("\nSelect [cpntJoin/cpntOn/cpntSt/cpntTrans/cpntMisc/utlSEw/utlSD/utlWat/utlMIsc/Cp/SPnt/Exist/eXit] <{0}> [J/O/S/T/M/SE/SD/W/MI/C/SP/E/X]", descVal);
                        escape = UserInput.getUserInputKeyword(descKey, out descKey, prompt, "J O S T M SE SD W MI C SP E X");
                        if (escape)
                            return;

                        if (descKey.ToUpper() == "X")
                            return;

                        exists = false;
                        foreach (CgPnt_Group.pntGroupParams pntGrp in CgPnt_Group.pntGroups)
                        {
                            if (pntGrp.key == descKey)
                            {
                                descVal = pntGrp.name;
                                color = pntGrp.color;
                                exists = true;
                                break;
                            }
                        }

                        if (!exists)
                        {
                            Application.ShowAlertDialog("Value entered is out of range. Exiting...");
                            return;
                        }

                        break;
                }
                Layer.manageLayers(descVal);
                ObjectId idPntLblStyle = Pnt_Style.getPntLabelStyle(descVal);
                ObjectId idPntStyle = Pnt_Style.getPntStyle(descVal);

                Layer.manageLayer(descVal, color);

                prompt = string.Format("\nSelect Point(s) to be moved to group {0}\r", descVal);

                Editor ed = BaseObjs._editor;

                tvs = new TypedValue[1];
                Type type = typeof(CogoPoint);
                tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(type).DxfName), 0);

                SelectionFilter filter = new SelectionFilter(tvs);
                PromptSelectionOptions pso = new PromptSelectionOptions();
                pso.MessageForAdding = prompt;
                pso.MessageForRemoval = "\nRemove Items";
                pso.RejectObjectsOnLockedLayers = true;

                PromptSelectionResult psr = null;
                BaseObjs.acadActivate();

                psr = BaseObjs._editor.GetSelection(pso, filter);
                SelectionSet ss = psr.Value;

                if (ss != null && ss.Count > 0)
                {
                    ObjectId[] ids = ss.GetObjectIds();
                    try
                    {
                        using (Transaction tr = BaseObjs.startTransactionDb())
                        {
                            foreach (ObjectId id in ids)
                            {
                                CogoPoint cogoPnt = (CogoPoint)tr.GetObject(id, OpenMode.ForWrite);
                                cogoPnt.RawDescription = descVal;
                                cogoPnt.Layer = descVal;
                                cogoPnt.LabelStyleId = idPntLblStyle;
                                cogoPnt.StyleId = idPntStyle;
                            }
                            tr.Commit();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(ex.Message + " cmdMCG.cs: line: 155");
                    }
                    CgPnt_Group.updatePntGroups();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdMCG.cs: line: 162");
            }
            finally
            {
                SnapMode.setOSnap((int)osMode);
                Application.SetSystemVariable("DYNMODE", (Int16)dynMode);
                Application.SetSystemVariable("DYNPROMPT", (Int16)dynPrompt);
            }
        }
    }
}
