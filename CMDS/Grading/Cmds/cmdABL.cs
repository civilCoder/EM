using Autodesk.AECC.Interop.Land;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_VB;
using System.Collections.Generic;
using System.Linq;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Grading.Cmds
{
    public static class cmdABL
    {
        public static void
        addBrkLine()
        {
            addBrkLines2Surface();
        }

        public static void
        addBrkLines2Surface(ObjectIdCollection ids = null, string nameSurface = null)
        {
            TinSurface surf = null;
            bool exists = false;

            try
            {
                if (ids == null)
                {
                    ids = Select.buildSSetIDs(typeof(Polyline3d), true);
                }

                if (nameSurface == null)
                {
                    string nameDwg = BaseObjs.docName;
                    if (nameDwg.Contains("CGP") || nameDwg.Contains("GCAL") || nameDwg.Contains("MASS"))
                        surf = Base_Tools45.C3D.Surf.getTinSurface("CPNT-ON", out exists);
                    if (nameDwg.Contains("CONT"))
                        surf = Base_Tools45.C3D.Surf.getTinSurface("EXIST", out exists);
                }
                else
                {
                    surf = Base_Tools45.C3D.Surf.getTinSurface(nameSurface, out exists);
                }

                if (ids.Count > 0)
                    surf.BreaklinesDefinition.AddStandardBreaklines(ids, 1.0, 0, 0, 0);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdABL.cs: line: 53");
            }
        }

        public static Handle
        cleanUpBreaklines()
        {
            bool exists = false;
            Handle handleBrkLine = "0000".stringToHandle();
            try
            {
                AeccTinSurface surf = mySurfaces.getAeccTinSurface("CPNT-ON", ref exists);
                if (exists && surf.Breaklines.Count > 0)
                {
                    myUtility.deleteAeccBreaklines(surf);

                    ObjectIdCollection ids = Select.buildSSetIDs(typeof(Polyline3d), true);
                    if (ids.Count > 0)
                    {
                        myUtility.addAeccBreaklines(surf, ids, "BRK-00");
                        surf.Rebuild();
                        handleBrkLine = ids[ids.Count - 1].getHandle();
                    }
                    Grading_Palette.gPalette.pGrading.HandleLastBrkLine = handleBrkLine;
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdABL.cs: line: 81");
            }
            return handleBrkLine;
        }

        public static void
        deleteBreaklinesInSurface()
        {
            bool exists = false;
            AeccTinSurface surf = mySurfaces.getAeccTinSurface("CPNT-ON", ref exists);

            myUtility.deleteAeccBreaklines(surf);
        }

        public static void
        removeBreaklinewWith0vertices()
        {
            bool exists = false;
            AeccTinSurface surf = mySurfaces.getAeccTinSurface("CPNT-ON", ref exists);
            mySurfaces.removeBreaklineswith0vertices();
        }

        public static string
        getBrkLineName(AeccTinSurface aeccSurf)
        {
            string nameBrkLine = string.Empty;
            List<string> listDesc = myUtility.getBrklineDescriptions(aeccSurf);

            switch (listDesc.Count)
            {
                case 0:
                    nameBrkLine = "BRK-00";
                    return nameBrkLine;

                case 1:
                    string temp = listDesc[0];
                    if (temp.Contains("BRK"))
                    {
                        nameBrkLine = temp.getStringIncrement();
                        return nameBrkLine;
                    }
                    else
                    {
                        nameBrkLine = "BRK-00";
                        return nameBrkLine;
                    }

                default:

                    if (listDesc.Contains("BRK"))
                    {
                        var nameQuery = from name in listDesc
                                        where name.Contains("BRK")
                                        orderby name descending
                                        select name;
                        List<string> descs = new List<string>();
                        foreach (string desc in nameQuery)
                        {
                            descs.Add(desc);
                        }

                        nameBrkLine = descs[0].getStringIncrement();
                        return nameBrkLine;
                    }
                    else
                    {
                        nameBrkLine = "BRK-00";
                        return nameBrkLine;
                    }
            }
        }

        public static string
        initializeBrklines()
        {
            string handleBrkLine = string.Empty;
            bool exists = false;
            try
            {
                AeccTinSurface surf = mySurfaces.getAeccTinSurface("CPNT-ON", ref exists);
                if (!exists)
                {
                    handleBrkLine = "0000";
                    return handleBrkLine;
                }
                if (surf.Breaklines.Count == 0)
                {
                    handleBrkLine = "0000";
                    return handleBrkLine;
                }
                myUtility.deleteAeccBreaklines(surf);
                TypedValue[] TVs = new TypedValue[5];
                TVs.SetValue(new TypedValue((int)DxfCode.Operator, "<OR"), 0);
                TVs.SetValue(new TypedValue((int)DxfCode.Start, "LINE"), 1);
                TVs.SetValue(new TypedValue((int)DxfCode.Start, "POLYLINE"), 2);
                TVs.SetValue(new TypedValue((int)DxfCode.Operator, "OR>"), 3);
                TVs.SetValue(new TypedValue((int)DxfCode.LayerName, "CPNT-BRKLINE"), 4);

                ObjectIdCollection idsObj = new ObjectIdCollection();
                SelectionSet ss = Select.buildSSet(TVs);
                ObjectId[] ids = ss.GetObjectIds();

                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        foreach (ObjectId id in ids)
                        {
                            Entity ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                            if (ent.GetType() != typeof(Polyline3d))
                            {
                                Misc.deleteObj(ent.ObjectId);
                            }
                            else
                            {
                                idsObj.Add(id);
                            }
                        }
                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(ex.Message + " cmdABL.cs: line: 211");
                }

               myUtility.addAeccBreaklines(surf, idsObj, "BRK-00");

                handleBrkLine = Db.idObjToHandle(idsObj[idsObj.Count - 1]).ToString();
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdABL.cs: line: 220");
            }
            return handleBrkLine;
        }
    }
}
