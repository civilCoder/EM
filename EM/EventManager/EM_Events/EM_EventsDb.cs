using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;

namespace EventManager {
    public static class dbCallback {

        private static List<string> types = new List<string> {
            "CogoPoint",
            "DBText",
            "Leader",
            "MText",
            "Polyline",
            "Polyline3d",
            "BlockReference"
        };

        internal static void m_db_ObjectOpenedForModify(object sender, ObjectEventArgs e) {
            EM_Helper.StreamMessage(String.Format("ObjectOpenedForModify - {0} {1}", e.DBObject.ToString(), e.DBObject.Id.ToString()));

            try {
                RXObject obj = e.DBObject;
                string type = obj.GetType().Name;

                if (!types.Contains(type))
                    return;

                List<EM_EData> enData = new List<EM_EData>();

                ResultBuffer rb = null;
                ObjectId idX = ObjectId.Null;
                string objTypeX = string.Empty;
                Handle hX = "0".stringToHandle();
                Point3d pnt3d = Public.pnt3dOrg;

                switch (type) {
                    case "CogoPoint":
                        CogoPoint ent = (CogoPoint)obj;
                        rb = ent.GetXDataForApplication(null);
                        idX = ent.ObjectId;
                        objTypeX = type;
                        hX = ent.Handle;
                        break;
                    case "DBText":
                    case "MText":
                    case "Leader":
                    case "Polyline":
                    case "Polyline3d":
                    case "BlockReference":
                        Entity entX = (Entity)obj;
                        rb = entX.GetXDataForApplication(null);
                        idX = entX.ObjectId;
                        objTypeX = type;
                        hX = entX.Handle;
                        break;
                }

                if (rb == null)
                    return;

                TypedValue[] tvsX = rb.AsArray();

                List<string> nameApps = null;
                List<TypedValue[]> lstTVs = xData.parseXData(tvsX, out nameApps);

                for (int i = 0; i < lstTVs.Count; i++) {
                    TypedValue[] tvs = lstTVs[i];
                    string nameApp = tvs[0].Value.ToString();

                    if (!Public.lstApps.Contains(nameApp))
                        continue;

                    if (type == "MText") {
                        MText mTxt = (MText)e.DBObject;
                        pnt3d = mTxt.Location;
                    }else if (type == "DBText") {
                        DBText Txt = (DBText)e.DBObject;
                        pnt3d = Txt.Position;
                    }else {
                        pnt3d = Public.pnt3dOrg;
                    }
                    EM_EData eData = new EM_EData(hX, idX, objTypeX, pnt3d, tvs);
                    enData.Add(eData);
                    EW_Storage.enData = enData;
                }
            }
            catch (System.Exception ex){
                BaseObjs.writeDebug(ex.Message + " EM_EventsDb.cs: line: 96");
            }
        }
    }

    public class EM_EventsDb {

        private Database m_db;
        private Hashtable m_dbsTable;
        private bool m_bDone;

        public EM_EventsDb() {
            m_bDone = false;
            m_dbsTable = new Hashtable();
            collectAllDbs();
            numDbEvents ++;
            EM_Helper.StreamMessage(string.Format("DbEvent Added: {0}", numDbEvents));            
        }

        ~EM_EventsDb() {
            numDbEvents --;
            EM_Helper.StreamMessage(string.Format("DbEvent Removed: {0}", numDbEvents));            
        }

        public static int numDbEvents {
            get; protected set;
        }

        public void collectAllDbs() {
            try {
                DocumentCollection m_docs = Application.DocumentManager;
                IEnumerator docEnum = m_docs.GetEnumerator();
                while (docEnum.MoveNext()) {
                    Document doc = (Document)docEnum.Current;
                    Database db = doc.Database;
                    addDb(ref db);
                }
            }
            catch (System.Exception ex){
                BaseObjs.writeDebug(ex.Message + " EM_EventsDb.cs: line: 135");
            }
        }

        public void addDb(ref Database db) {
            if (!m_dbsTable.ContainsKey(db))
                m_dbsTable.Add(db, new EM_CBoolClass(false));
        }
        
        public void removeDb(ref Database db) {
            if (m_dbsTable.ContainsKey(db)) {
                UndoADb(ref db);
                m_dbsTable.Remove(db);
            }
        }

        public void Do() {
            if (m_bDone == false) {
                m_bDone = true;
            }else {
            }

            try {
                foreach (DictionaryEntry entry in m_dbsTable) {
                    EM_CBoolClass c_bClassVar = (EM_CBoolClass)entry.Value;

                    if (c_bClassVar.ToString().ToLower() == "true")
                        continue;
                    m_db = (Database)entry.Key;
                    m_db.ObjectOpenedForModify += dbCallback.m_db_ObjectOpenedForModify;    

                    c_bClassVar.val = true;
                }
            }
            catch (System.Exception ex){
                BaseObjs.writeDebug(ex.Message + " EM_EventsDb.cs: line: 170");
            }
        }

        public void Undo() {
            if (m_bDone == false)
                return;
            else
                m_bDone = false;
            try {
                IDictionaryEnumerator dbsEnumerator = m_dbsTable.GetEnumerator();
                while (dbsEnumerator.MoveNext()) {
                    DictionaryEntry entry = (DictionaryEntry)dbsEnumerator.Current;

                    EM_CBoolClass c_bClassVar = (EM_CBoolClass)entry.Value;

                    if (c_bClassVar.ToString().ToLower() == "false")
                        continue;

                    m_db = (Database)entry.Key;
                    m_db.ObjectOpenedForModify -= dbCallback.m_db_ObjectOpenedForModify; 
                    c_bClassVar.val = false;
                }
            }
            catch (System.Exception ex){
                BaseObjs.writeDebug(ex.Message + " EM_EventsDb.cs: line: 195");
            }
        }

        public void DoADb(ref Database db) {
            try {
                if (!m_dbsTable.ContainsKey(db))
                    return;
                EM_CBoolClass c_bClassVar = (EM_CBoolClass)m_dbsTable[db];

                if (c_bClassVar.ToString().ToLower() == "true")
                    return;
                
                m_db = db;
                m_db.ObjectOpenedForModify += dbCallback.m_db_ObjectOpenedForModify;
                c_bClassVar.val = true;
            }
            catch (System.Exception ex){
                BaseObjs.writeDebug(ex.Message + " EM_EventsDb.cs: line: 213");
            }
        }

        public void UndoADb(ref Database db) {
            try {
                if (!m_dbsTable.ContainsKey(db))
                    return;
                EM_CBoolClass c_bClassVar = (EM_CBoolClass)m_dbsTable[db];
                if (c_bClassVar.ToString().ToLower() == "false")
                    return;

                m_db = db;
                m_db.ObjectOpenedForModify -= dbCallback.m_db_ObjectOpenedForModify; 
                c_bClassVar.val = false;
            }
            catch (System.Exception ex){
                BaseObjs.writeDebug(ex.Message + " EM_EventsDb.cs: line: 230");
            }
        }
    }
}
