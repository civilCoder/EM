//using Autodesk.Civil.DatabaseServices;
using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;
using DBObject = Autodesk.AutoCAD.DatabaseServices.DBObject;

namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static class Dict
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="idParent"></param>
        /// <param name="entrySub"></param>
        /// <returns></returns>
        public static void
        addDictEntry(ObjectId idParent, string nameSubEntry, ObjectId id)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    DBObject dbObj = tr.GetObject(id, OpenMode.ForWrite);
                    DBDictionary parent = (DBDictionary)idParent.GetObject(OpenMode.ForWrite);
                    parent.SetAt(nameSubEntry, dbObj);

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 35");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idParent"></param>
        /// <param name="entrySub"></param>
        /// <returns></returns>
        public static ObjectId
        addSubDict(ObjectId idParent, string nameSubDict)
        {
            ObjectId idDict = ObjectId.Null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    DBDictionary parent = (DBDictionary)idParent.GetObject(OpenMode.ForWrite);
                    DBDictionary subentry;
                    subentry = new DBDictionary();
                    parent.SetAt(nameSubDict, subentry);
                    tr.AddNewlyCreatedDBObject(subentry, true);
                    tr.Commit();
                    idDict = subentry.ObjectId;
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 64");
            }
            return idDict;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idDict"></param>
        /// <param name="nameApp"></param>
        /// <param name="rb"></param>
        public static void
        addXRec(ObjectId idDict, string nameApp, ResultBuffer rb)
        {
            try
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        DBDictionary dict = (DBDictionary)idDict.GetObject(OpenMode.ForWrite);

                        //if (dict.Contains(nameApp))
                        //    dict.Remove(nameApp);

                        Xrecord xrec = new Xrecord();
                        xrec.XlateReferences = true;
                        xrec.Data = rb;
                        dict.SetAt(nameApp, xrec);
                        tr.AddNewlyCreatedDBObject(xrec, true);
                        tr.Commit();
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 100");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idDict"></param>
        /// <param name="nameApp"></param>
        /// <param name="tvs"></param>
        public static void
        addXRec(ObjectId idDict, string nameApp, TypedValue[] TVs)
        {
            ResultBuffer RB = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    try
                    {
                        RB = new ResultBuffer(TVs);
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 124");
                    }
                    DBDictionary dict = (DBDictionary)idDict.GetObject(OpenMode.ForRead);
                    Xrecord xrec = new Xrecord();
                    xrec.XlateReferences = true;
                    xrec.Data = RB;
                    dict.UpgradeOpen();
                    dict.SetAt(nameApp, xrec);
                    dict.DowngradeOpen();
                    tr.AddNewlyCreatedDBObject(xrec, true);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 139");
            }
        }

        public static void
        deleteDictionary(string name)
        {
            Database db = BaseObjs._db;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    DBDictionary objDict = (DBDictionary)db.NamedObjectsDictionaryId.GetObject(OpenMode.ForWrite);
                    if (objDict.Contains(name))
                    {
                        try
                        {
                            objDict.Remove(name);
                        }
                        catch (System.Exception ex)
                        {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 160");
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 168");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idDict"></param>
        /// <param name="nameApp"></param>
        public static void
        deleteXRec(ObjectId idDict, string nameApp)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    DBDictionary dict = (DBDictionary)idDict.GetObject(OpenMode.ForRead);
                    dict.UpgradeOpen();
                    if (dict.Contains(nameApp))
                        dict.Remove(nameApp);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 193");
            }
        }

        public static void
        delSubDict(ObjectId idParent, string nameSubDict)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    DBDictionary parent = (DBDictionary)idParent.GetObject(OpenMode.ForWrite);
                    if (parent.Contains(nameSubDict))
                        parent.Remove(nameSubDict);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 212");
            }
        }

        public static ObjectId
        dictExists(string nameDict)
        {
            ObjectId idDict = ObjectId.Null;
            DBDictionary dictNOD = null;
            Database db = BaseObjs._db;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    try
                    {
                        dictNOD = (DBDictionary)db.NamedObjectsDictionaryId.GetObject(OpenMode.ForRead);
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 232");
                    }
                    try
                    {
                        if (dictNOD.Contains(nameDict))
                        {
                            idDict = dictNOD.GetAt(nameDict);
                        }
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 243");
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 250");
            }
            return idDict;
        }

        //public static string
        //getCmdDefault(string nameCmd, string nameApp) {
        //    string valueCmd = string.Empty;
        //    try {
        //        using (Transaction tr = BaseObjs.startTransactionDb()) {
        //            ResultBuffer rb = null;
        //            TypedValue[] tvs = null;
        //            bool exists = false;
        //            ObjectId idDict = getNamedDictionary("cmdDefault", out exists);
        //            if (exists) {
        //                ObjectId idDictX = getSubEntry(idDict, nameCmd);
        //                if (idDictX == ObjectId.Null)
        //                    idDictX = addSubDict(idDict, nameCmd);
        //                rb = getXRec(idDictX, nameApp);
        //                if (rb != null) {
        //                    tvs = rb.AsArray();
        //                    valueCmd = tvs[0].Value.ToString();
        //                    //setCmdDefault(nameCmd, nameApp, valueCmd);
        //                }else {
        //                    rb = new ResultBuffer(new TypedValue(1000, ""));
        //                    Dict.addXRec(idDictX, nameApp, rb);
        //                }
        //            }
        //            tr.Commit();
        //        }
        //    }
        //    catch (System.Exception ex){
        //        BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 282");
        //    }
        //    return valueCmd;
        //}
        public static string
        getCmdDefault(string nameCmd, string nameApp)
        {
            string valueCmd = string.Empty;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ResultBuffer rb = null;
                    TypedValue[] tvs = null;
                    bool exists = false;
                    ObjectId idDict = getNamedDictionary(nameCmd, out exists);
                    if (exists)
                    {
                        rb = getXRec(idDict, nameApp);
                        if (rb != null)
                        {
                            tvs = rb.AsArray();
                            valueCmd = tvs[0].Value.ToString();
                        }
                        //else
                        //{
                        //    rb = new ResultBuffer(new TypedValue(1000, ""));
                        //    Dict.addXRec(idDict, nameApp, rb);
                        //}
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 317");
            }
            return valueCmd;
        }

        public static DBDictionary
        getDictionary(ObjectId idDict)
        {
            DBDictionary dict = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    try
                    {
                        dict = (DBDictionary)tr.GetObject(idDict, OpenMode.ForRead);
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 336");
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 343");
            }
            return dict;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idDict"></param>
        /// <returns></returns>
        public static List<DBDictionaryEntry>
        getEntries(ObjectId idDict)
        {
            List<DBDictionaryEntry> entries = new List<DBDictionaryEntry>();
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    DBDictionary dict = (DBDictionary)tr.GetObject(idDict, OpenMode.ForRead);
                    try
                    {
                        foreach (DBDictionaryEntry entry in dict)
                        {
                            entries.Add(entry);
                        }
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 371");
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 378");
            }
            return entries;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="exists"></param>
        /// <returns></returns>
        public static ObjectId
        getNamedDictionary(string name, out bool exists)
        {
            exists = false;
            Database db = HostApplicationServices.WorkingDatabase;
            DBDictionary appDict = null;
            ObjectId idDict = ObjectId.Null;
            DBDictionary dictNOD = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    try
                    {
                        dictNOD = (DBDictionary)db.NamedObjectsDictionaryId.GetObject(OpenMode.ForRead);
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 407");
                    }
                    try
                    {
                        if (dictNOD.Contains(name))
                        {
                            idDict = dictNOD.GetAt(name);
                            exists = true;
                        }
                        else
                        {
                            dictNOD.UpgradeOpen();
                            appDict = new DBDictionary();
                            idDict = dictNOD.SetAt(name, appDict);
                            tr.AddNewlyCreatedDBObject(appDict, true);
                        }
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 426");
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 433");
            }
            return idDict;
        }

        public static ObjectId
        getSubDict(ObjectId idParent, string nameSubDict)
        {
            ObjectId idDict = ObjectId.Null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    DBDictionary parent = (DBDictionary)idParent.GetObject(OpenMode.ForRead);
                    if (parent.Contains(nameSubDict))
                    {
                        try
                        {
                            idDict = parent.GetAt(nameSubDict);
                        }
                        catch (System.Exception ex)
                        {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 455");
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 463");
            }
            return idDict;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idParent"></param>
        /// <param name="entrySub"></param>
        /// <returns></returns>
        public static ObjectId
        getSubEntry(ObjectId idParent, string entrySub)
        {
            ObjectId idDict = ObjectId.Null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    DBDictionary parent = (DBDictionary)idParent.GetObject(OpenMode.ForRead);
                    if (parent.Contains(entrySub))
                    {
                        try
                        {
                            idDict = parent.GetAt(entrySub);
                        }
                        catch (System.Exception ex)
                        {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 491");
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 499");
            }
            return idDict;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idDict"></param>
        /// <param name="nameApp"></param>
        /// <returns></returns>
        public static ResultBuffer
        getXRec(ObjectId idDict, string nameApp)
        {
            ResultBuffer RB = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    DBDictionary dict = (DBDictionary)idDict.GetObject(OpenMode.ForRead);
                    try
                    {
                        if (dict.Contains(nameApp))
                        {
                            ObjectId idDictX = dict.GetAt(nameApp);
                            DBObject dbObj = idDictX.GetObject(OpenMode.ForRead);
                            Xrecord xrec = (Xrecord)dbObj;
                            if (xrec != null)
                            {
                                RB = xrec.Data;
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 534");
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 541");
            }
            return RB;
        }

        public static TypedValue[]
        getXRec(ObjectId idXRec)
        {
            TypedValue[] tvs = new TypedValue[] {
            };
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    try
                    {
                        Xrecord xrec = (Xrecord)tr.GetObject(idXRec, OpenMode.ForRead);
                        tvs = xrec.Data.AsArray();
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 562");
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 569");
            }
            return tvs;
        }

        public static List<TypedValue[]>
        getXRecs(ObjectId idDict)
        {
            List<TypedValue[]> lsttvs = new List<TypedValue[]>();
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    DBDictionary dict = (DBDictionary)idDict.GetObject(OpenMode.ForRead);
                    try
                    {
                        foreach (DBDictionaryEntry entry in dict)
                        {
                            ObjectId id = dict.GetAt(entry.Key);
                            Xrecord xrec = (Xrecord)tr.GetObject(id, OpenMode.ForRead);
                            lsttvs.Add(xrec.Data.AsArray());
                        }
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 594");
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 601");
            }
            return lsttvs;
        }

        public static Xrecord
        getXRec(ObjectId idDict, DBDictionaryEntry entry)
        {
            Xrecord xrec = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    DBDictionary dict = (DBDictionary)idDict.GetObject(OpenMode.ForRead);
                    try
                    {
                        ObjectId id = dict.GetAt(entry.Key);
                        xrec = (Xrecord)tr.GetObject(id, OpenMode.ForRead);
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 622");
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 629");
            }
            return xrec;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        public static void
        removeNamedDictionary(string name)
        {
            Database db = BaseObjs._db;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    DBDictionary objDict = (DBDictionary)db.NamedObjectsDictionaryId.GetObject(OpenMode.ForRead);
                    try
                    {
                        ObjectId idDict = objDict.GetAt(name);
                        objDict.UpgradeOpen();
                        objDict.Remove(idDict);
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 655");
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 662");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idParent"></param>
        /// <param name="entrySub"></param>
        public static void
        removeSubEntry(ObjectId idParent, string entrySub)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    DBDictionary parent = (DBDictionary)idParent.GetObject(OpenMode.ForRead);
                    if (parent.Contains(entrySub))
                    {
                        parent.UpgradeOpen();
                        try
                        {
                            parent.Remove(entrySub);
                        }
                        catch (System.Exception ex)
                        {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 688");
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 696");
            }
        }

        public static void
        setCmdDefault(string nameCmd, string nameApp, string valueCmd)
        {
            TypedValue[] tvs = new TypedValue[1];
            tvs.SetValue(new TypedValue(1000, valueCmd), 0);
            ResultBuffer rb = new ResultBuffer(tvs);
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    bool exists = false;
                    ObjectId idDict = getNamedDictionary(nameCmd, out exists);
                    try
                    {
                        ResultBuffer rbX = getXRec(idDict, nameApp);
                        if (rbX != null)
                        {
                            deleteXRec(idDict, nameApp);
                        }
                        addXRec(idDict, nameApp, rb);
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 723");
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Dict.cs: line: 730");
            }
        }
    }
}
