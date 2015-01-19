using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Base_Tools45;
using System;
using System.Collections.Generic;
using System.IO;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace DimPL
{
    public class DimPL_Startup{

        private static List<string> XRefTar = new List<string>();
        
        public static void
        initDrawing()
        {
            using (BaseObjs._acadDoc.LockDocument())
            {
                Layer.manageLayers("DIMENSION", 7);
                Layer.manageLayers("TEXT", 2);
            }
        }

        public static void
        checkXRefsDimPl()
        {
            DimPL_Global.countUpdates = 0;
            getDictXRefNames();
            if (XRefTar != null && XRefTar.Count > 0)
                foreach (string name in XRefTar)
                    checkIfCurrent(name);
        }


        private static void
        getDictXRefNames()
        {
            //Dict.deleteDictionary("XRefNames");
            bool exists = false;
            ObjectId idDict = Dict.getNamedDictionary("XRefNames", out exists);
            ResultBuffer rb;
            if (!exists)
            {
                rb = xRef.getXRefsContainingTargetDwgName("BNDY");
                if (rb != null)
                {
                    //XRefTar = new List<string>();
                    Dict.addXRec(idDict, "XRefNames", rb);
                    TypedValue[] tvs = rb.AsArray();
                    foreach (TypedValue tv in tvs)
                    {
                        if (tv.TypeCode == 1001)
                        {
                            string nameFile = tv.Value.ToString();
                            XRefTar.Add(nameFile);
                        }
                    }
                }
            }
            else
            {
                //XRefTar = new List<string>();
                try
                {
                    rb = Dict.getXRec(idDict, "XRefNames");
                    TypedValue[] tvs = rb.AsArray();
                    foreach (TypedValue tv in tvs)
                    {
                        if (tv.TypeCode == 1001)
                        {
                            string nameFile = tv.Value.ToString();
                            XRefTar.Add(nameFile);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(string.Format("{0} DimPL_Startup.cs: line: 69", ex.Message));
                }
            }
        }

        private static void
        checkIfCurrent(string nameFile)
        {
            bool isCurrent = false;
            int compare = 0;

            bool exists = false;
            ObjectId idDict = Dict.getNamedDictionary("XRefNames", out exists);
            ResultBuffer rb = Dict.getXRec(idDict, "XRefNames");
            TypedValue[] tvs = rb.AsArray();
            DateTime lastWrite = new DateTime();

            if (File.Exists(nameFile))
            {
                FileInfo fi = new FileInfo(nameFile);
                lastWrite = fi.LastWriteTime;
                string strLastWrite = lastWrite.ToString();
                lastWrite = System.Convert.ToDateTime(strLastWrite);
            }
            for (int i = 0; i < tvs.Length; i++)
            {
                if (tvs[i].Value.ToString() == nameFile)
                {
                    DateTime savedWrite = System.Convert.ToDateTime(tvs[i + 1].Value.ToString());
                    compare = lastWrite.CompareTo(savedWrite);
                    if (compare == 0)
                        isCurrent = true;
                    if (compare > 0)
                        isCurrent = false;
                    break;
                }
            }
            if (!isCurrent)
            {
                DimPL_Global.countUpdates = 0;
                Document workDoc = Application.DocumentManager.MdiActiveDocument;
                string nameUser = "";
                int dwgStatus = Base_Tools45.FileManager.getFileStatus(nameFile, out nameUser);
                switch (dwgStatus)
                {
                    case (int)filestatus.isOpenLocal:
                        Document doc = FileManager.getDoc(nameFile, true, false);
                        Application.DocumentManager.MdiActiveDocument = doc;
                        Object dbMod = Application.GetSystemVariable("DBMOD");
                        if (System.Convert.ToInt16(dbMod) != 0)
                        {
                            doc.Database.SaveAs(doc.Name, true, DwgVersion.Current, doc.Database.SecurityParameters);
                        }
                        Application.DocumentManager.MdiActiveDocument = workDoc;
                        updateDimPL(nameFile);
                        break;

                    case (int)filestatus.isOpenLocalReadOnly:
                        updateDimPL(nameFile);
                        break;

                    case (int)filestatus.isLocked:
                        updateDimPL(nameFile);
                        break;

                    case (int)filestatus.isAvailable:
                        updateDimPL(nameFile);
                        break;
                }
                if (DimPL_Global.countUpdates > 0)
                    Application.ShowAlertDialog(string.Format("{0} DimPL updates were performed.", DimPL_Global.countUpdates));
            }
        }

        private static void
        updateDimPL(string nameFile)
        {
            bool exists;
            ObjectId idDict = Dict.getNamedDictionary(apps.lnkDimPL, out exists);
            List<DBDictionaryEntry> dictEntries = idDict.getEntries();
            if (dictEntries.Count == 0)
                return;
            foreach (DBDictionaryEntry entry in dictEntries)
            {
                string h = entry.Key;
                ObjectId idTX = h.stringToHandle().getObjectId();
                if (!idTX.IsValid)
                {
                    Dict.removeSubEntry(idDict, h);
                    continue;
                }

                //idTX.deactivateObj();
                DimPL_Update.updateDimPL(idTX, nameFile);
                //idTX.activateObj();
            }
        }
    }
}