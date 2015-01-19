using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using Stake.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using Table = Autodesk.AutoCAD.DatabaseServices.Table;

namespace Stake
{
    public static class Stake_Dict
    {
        private static frmStake fStake = Stake_Forms.sForms.fStake;

        public static void
        printDictionary()
        {
            bool exists = false;
            ObjectId idDict = Dict.getNamedDictionary("STAKE", out exists);
            if (idDict == ObjectId.Null)
                return;
            int i = 0;
            List<DBDictionaryEntry> entries = Dict.getEntries(idDict);
            foreach (DBDictionaryEntry entry in entries)
            {
                if (entry.GetType() == typeof(DBDictionary))
                {
                    ObjectId idDictX = Dict.getSubEntry(idDict, entry.Key);
                    List<TypedValue[]> tvs = Dict.getXRecs(idDictX);
                    foreach (TypedValue[] tv in tvs)
                    {
                        Debug.Print(string.Format("{0} {1} {2} {3} {4}", i, tv[0].Value.ToString(), tv[1].Value.ToString(), tv[2].Value.ToString(), tv[3].Value.ToString()));
                    }
                }
            }
        }

        public static void
        updateDictGRIDs(string strName, string strType, string strFunction, ObjectId idObj, string strHandle = "", int intIndex = -1)
        {
            bool exists;
            ObjectId idDictGRIDS = Dict.getNamedDictionary("GRIDS", out exists);

            ObjectId idDictG = Dict.getSubDict(idDictGRIDS, strName);
            if (idDictG == ObjectId.Null)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(string.Format("Dictionary {0} is missing - exiting...", strName));
                return;
            }

            ObjectId idDictT = Dict.addSubDict(idDictG, strType);

            if (strFunction == "ADD")
            {
                ObjectId idDictX = Dict.getSubDict(idDictT, idObj.ToString());
                Dict.addXRec(idDictX, idObj.ToString(), new ResultBuffer(new TypedValue(1005, strHandle)));
            }
            else if (strFunction == "DELETE")
            {
                Dict.delSubDict(idDictT, idObj.ToString());
            }

            printDictGRID();
        }

        public static void
        printDictGRID()
        {
            bool exists;
            ObjectId idDictGRIDs = Dict.getNamedDictionary("GRIDS", out exists);
            List<DBDictionaryEntry> entriesGRID = Dict.getEntries(idDictGRIDs);
            Debug.Print("Begin Dictionary Output");

            foreach (DBDictionaryEntry entryGRID in entriesGRID)
            {
                Debug.Print(entryGRID.Key.ToString());
                ObjectId idDictG = Dict.getSubDict(idDictGRIDs, entryGRID.Key.ToString());
                List<DBDictionaryEntry> entriesG = Dict.getEntries(idDictG);

                foreach (DBDictionaryEntry entryG in entriesG)
                {
                    Debug.Print(entryG.Key.ToString());
                    ObjectId idDictT = Dict.getSubDict(idDictG, entryG.Key.ToString());
                    List<DBDictionaryEntry> entriesT = Dict.getEntries(idDictT);
                    foreach (DBDictionaryEntry entryT in entriesT)
                    {
                        ObjectId idDictX = Dict.getSubDict(idDictT, entryT.Key.ToString());
                        List<DBDictionaryEntry> entriesX = Dict.getEntries(idDictX);
                        ResultBuffer rb = Dict.getXRec(idDictX, entriesX[0].Key.ToString());
                        Debug.Print(string.Format("{0} {1}", entryT.Key.ToString(), entriesX[0].Key.ToString()));
                    }
                }
            }
        }

        public static void
        updateDictSTAKE()
        {
            Dict.deleteDictionary("STAKE");
            bool exists = false;
            ObjectId idDictSTAKE = Dict.getNamedDictionary("STAKE", out exists);

            List<AlgnData> algnData = fStake.algnData;
            for (int i = 0; i < algnData.Count; i++)
            {
                ObjectId idAlign = algnData[i].AlignID;
                if (idAlign.IsValid && !idAlign.IsErased)
                {
                    Alignment align = Align.getAlignment(idAlign);
                    ObjectId idDictX = Dict.getNamedDictionary(i.ToString(), out exists);

                    TypedValue[] tvs = new TypedValue[1];
                    tvs.SetValue(new TypedValue(1000, align.Name), 0);
                    Dict.addXRec(idDictX, "STAKE", new ResultBuffer(tvs));

                    tvs = new TypedValue[1];
                    tvs.SetValue(new TypedValue(1005, align.Handle), 0);
                    Dict.addXRec(idDictX, "STAKE", new ResultBuffer(tvs));

                    tvs = new TypedValue[1];
                    tvs.SetValue(new TypedValue(1000, align.ObjectId), 0);
                    Dict.addXRec(idDictX, "STAKE", new ResultBuffer(tvs));

                    tvs = new TypedValue[1];
                    tvs.SetValue(new TypedValue(1000, align.Layer), 0);
                    Dict.addXRec(idDictX, "STAKE", new ResultBuffer(tvs));

                    tvs = new TypedValue[1];
                    tvs.SetValue(new TypedValue(1000, algnData[i].TableHandle), 0);
                    Dict.addXRec(idDictX, "STAKE", new ResultBuffer(tvs));
                }
            }

            Stake_Dict.printDictionary();
        }

        public static void
        updateDictGRIDsWithBldgName(List<Handle> colGrid, string strName, string strType)
        {
            bool exists;
            ObjectId idDictGRIDS = Dict.getNamedDictionary("GRIDS", out exists);

            ObjectId idDictG = Dict.getSubDict(idDictGRIDS, strName);
            if (idDictG == ObjectId.Null)
                idDictG = Dict.addSubDict(idDictGRIDS, strName);

            Dict.delSubDict(idDictG, strType);
            ObjectId idDictT = Dict.addSubDict(idDictG, strType);
            for (int i = 0; i < colGrid.Count; i++)
            {
                ObjectId idDictX = Dict.addSubDict(idDictT, colGrid[i].getObjectId().ToString());
                Dict.addXRec(idDictX, colGrid[i].getObjectId().ToString(), new ResultBuffer(new TypedValue(1005, colGrid[i].ToString())));
            }
            ObjectId idDict1 = Dict.addSubDict(idDictT, "INDEX");
            Dict.addXRec(idDict1, "INDEX", new ResultBuffer(new TypedValue(1005, colGrid[0].ToString())));
            printDictionary();
        }

        //update AlgnData from Dictionary
        public static void
        updateAlignData()
        {
            ObjectIdCollection idsAlign = Align.getAlignmentIDs();

            List<AlgnData> alignData = new List<AlgnData>();

            foreach (ObjectId id in idsAlign)
            {
                Alignment objAlign = (Alignment)id.getEnt();
                ResultBuffer rb = id.getXData("CLASS");

                if (rb != null)
                {
                    rb = id.getXData("TABLE");

                    if (rb != null)
                    {
                        TypedValue[] tvs = rb.AsArray();
                        AlgnData aData = new AlgnData();

                        aData.TableHandle = tvs[1].Value.ToString().stringToHandle();
                        aData.AlignLayer = objAlign.Layer;
                        aData.AlignName = objAlign.Name;
                        aData.AlignHandle = objAlign.Handle;
                        aData.AlignID = objAlign.ObjectId;

                        alignData.Add(aData);
                    }
                    else
                    {
                        SelectionSet objSSet = Select.buildSSet(typeof(Table));

                        if (objSSet.Count > 0)
                        {
                            ObjectId[] ids = objSSet.GetObjectIds();

                            foreach (ObjectId idTable in ids)
                            {
                                Table table = (Table)idTable.getEnt();

                                if (table.Cells[1, 1].Contents[0].ToString() == objAlign.Name)
                                {
                                    TypedValue[] tvs = new TypedValue[2];
                                    tvs.SetValue(new TypedValue(1001, "TABLE"), 0);
                                    tvs.SetValue(new TypedValue(1005, table.Handle), 0);
                                    id.setXData(tvs, "TABLE");

                                    AlgnData aData = new AlgnData();

                                    aData.TableHandle = table.Handle;
                                    aData.AlignLayer = objAlign.Layer;
                                    aData.AlignName = objAlign.Name;
                                    aData.AlignHandle = objAlign.Handle;
                                    aData.AlignID = objAlign.ObjectId;

                                    alignData.Add(aData);
                                }
                            }
                        }
                        else
                        {
                            string message = string.Format("Table for Align: {0} is missing -> redo alignment!!", objAlign.Name);
                            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(message);
                        }
                    }
                }
            }
        }

        public static void
        resetObjectIdsInDict()
        {
            bool exists;
            ObjectId idDictGRIDs = Dict.getNamedDictionary("GRIDS", out exists);

            List<DBDictionaryEntry> entriesGRID = Dict.getEntries(idDictGRIDs);

            foreach (DBDictionaryEntry entryGRID in entriesGRID)
            {
                ObjectId idDictG = Dict.getSubDict(idDictGRIDs, entryGRID.Key.ToString());
                ObjectId idDictT = Dict.getSubDict(idDictG, "ALPHA");
                List<DBDictionaryEntry> entriesT = Dict.getEntries(idDictT);
                foreach (DBDictionaryEntry entryT in entriesT)
                {
                    if (entryT.Key.ToString() != "INDEX")
                    {
                        ObjectId idDictX = Dict.getSubDict(idDictT, entryT.Key.ToString());
                        List<DBDictionaryEntry> entriesX = Dict.getEntries(idDictX);
                        Xrecord xrec = Dict.getXRec(idDictX, entriesX[0]);
                        ResultBuffer rb = xrec.Data;
                        TypedValue[] tvs = rb.AsArray();
                        Handle hLine = tvs[0].Value.ToString().stringToHandle();
                        Line line = (Line)hLine.getEnt();
                        if (line == null)
                            continue;
                        Dict.delSubDict(idDictT, entryT.Key.ToString());
                        Dict.addSubDict(idDictT, line.ObjectId.ToString());
                    }
                }

                idDictT = Dict.getSubDict(idDictG, "NUMERIC");
                entriesT = Dict.getEntries(idDictT);
                foreach (DBDictionaryEntry entryT in entriesT)
                {
                    if (entryT.Key.ToString() != "INDEX")
                    {
                        ObjectId idDictX = Dict.getSubDict(idDictT, entryT.Key.ToString());
                        List<DBDictionaryEntry> entriesX = Dict.getEntries(idDictX);
                        Xrecord xrec = Dict.getXRec(idDictX, entriesX[0]);
                        ResultBuffer rb = xrec.Data;
                        TypedValue[] tvs = rb.AsArray();
                        Handle hLine = tvs[0].Value.ToString().stringToHandle();
                        Line line = (Line)hLine.getEnt();
                        if (line == null)
                            continue;
                        Dict.delSubDict(idDictT, entryT.Key.ToString());
                        Dict.addSubDict(idDictT, line.ObjectId.ToString());
                    }
                }
            }
        }
    }
}