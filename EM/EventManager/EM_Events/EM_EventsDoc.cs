using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;

namespace EventManager {
    public class EM_EventsDoc {

        public static EM_EventsDb m_dbWatcher;

        public EM_EventsDoc() {
            m_bDone = false;
            m_docsTable = new Hashtable();
            collectAllDocs();
            Do();
            numDocEvents ++;
            EM_Helper.StreamMessage(string.Format("DocEvent Added: {0}", numDocEvents));
            m_dbWatcher = new EM_EventsDb();        
        }

        ~EM_EventsDoc() {
            numDocEvents --;
            EM_Helper.StreamMessage(string.Format("DocEvents Removed: {0}", numDocEvents));
        }

        public static int numDocEvents {
            get; protected set;
        }

        public void collectAllDocs() {
            try {
                DocumentCollection m_docs = Application.DocumentManager;
                IEnumerator docEnum = m_docs.GetEnumerator();
                while (docEnum.MoveNext()) {
                    Document doc = (Document)docEnum.Current;
                    addDoc(ref doc);
                }
            }
            catch (System.Exception ex){
                BaseObjs.writeDebug(ex.Message + " EM_EventsDoc.cs: line: 45");
            }
        }

        public void addDoc(ref Document doc) {
            if (!m_docsTable.ContainsKey(doc))
                m_docsTable.Add(doc, new EM_CBoolClass(false));
            if (m_dbWatcher != null) {
                Database db = doc.Database;
                m_dbWatcher.addDb(ref db);
            }
        }
        
        public void removeDoc(ref Document doc) {
            if (m_docsTable.ContainsKey(doc)) {
                UndoADoc(ref doc);
                m_docsTable.Remove(doc);
            }
        }

        private Document m_doc;
        private Hashtable m_docsTable;
        private bool m_bDone;

        public void Do() {
            if (m_bDone == false) {
                m_bDone = true;
            }else {
            }

            try {
                foreach (DictionaryEntry entry in m_docsTable) {
                    EM_CBoolClass c_bClassVar = (EM_CBoolClass)entry.Value;

                    if (c_bClassVar.ToString().ToLower() == "true")
                        continue;
                    m_doc = (Document)entry.Key;
                    m_doc.CommandWillStart += m_doc_CommandWillStart;    
                    
                    c_bClassVar.val = true;
                }
            }
            catch (System.Exception ex){
                BaseObjs.writeDebug(ex.Message + " EM_EventsDoc.cs: line: 88");
            }
        }

        public void Undo() {
            if (m_bDone == false)
                return;
            else
                m_bDone = false;
            try {
                IDictionaryEnumerator docsEnumerator = m_docsTable.GetEnumerator();
                while (docsEnumerator.MoveNext()) {
                    DictionaryEntry entry = (DictionaryEntry)docsEnumerator.Current;

                    EM_CBoolClass c_bClassVar = (EM_CBoolClass)entry.Value;

                    if (c_bClassVar.ToString().ToLower() == "false")
                        continue;

                    m_doc = (Document)entry.Key;
                    m_doc.CommandWillStart -= m_doc_CommandWillStart;   
 
                    c_bClassVar.val = false;
                }
            }
            catch (System.Exception ex){
                BaseObjs.writeDebug(ex.Message + " EM_EventsDoc.cs: line: 114");
            }
        }

        public void UndoADoc(ref Document doc) {
            try {
                if (!m_docsTable.ContainsKey(doc))
                    return;
                EM_CBoolClass c_bClassVar = (EM_CBoolClass)m_docsTable[doc];
                if (c_bClassVar.ToString().ToLower() == "false")
                    return;

                m_doc = doc;
                m_doc.CommandWillStart -= m_doc_CommandWillStart;

                c_bClassVar.val = false;
            }
            catch (System.Exception ex){
                BaseObjs.writeDebug(ex.Message + " EM_EventsDoc.cs: line: 132");
            }
        }

        void m_doc_CommandWillStart(object sender, CommandEventArgs e) {
            EM_Helper.StreamMessage(String.Format("CommandWillStart - {0}", e.GlobalCommandName));
            Database db = m_doc.Database;

            if (e.GlobalCommandName == "ERASE" || e.GlobalCommandName == "E") {
                m_doc.CommandEnded += m_doc_CommandEnded;
                m_doc.CommandFailed += m_doc_CommandEnded;
                m_doc.CommandCancelled += m_doc_CommandEnded;
                m_dbWatcher.DoADb(ref db);
                //m_doc.Database.ObjectOpenedForModify += dbCallback.m_db_ObjectOpenedForModify;
            }else if (e.GlobalCommandName == "GRIP_StrETCH" || e.GlobalCommandName == "MOVE") {
                m_doc.CommandEnded += m_doc_CommandEnded;
                m_doc.CommandFailed += m_doc_CommandEnded;
                m_doc.CommandCancelled += m_doc_CommandEnded;
                m_dbWatcher.DoADb(ref db);
                //m_doc.Database.ObjectOpenedForModify += dbCallback.m_db_ObjectOpenedForModify;
            }else if (e.GlobalCommandName == "SAVEAS") {
                m_doc.CommandEnded += m_doc_CommandEnded;
            }else if (e.GlobalCommandName == "QUIT") {
            }else if (e.GlobalCommandName == "CLOSE") {
                m_doc.CommandEnded += m_doc_CommandEnded;
            }else if (e.GlobalCommandName == "OPEN") {
                m_doc.CommandEnded += m_doc_CommandEnded;
            }
        }

        void m_doc_CommandEnded(object sender, CommandEventArgs e) {
            EM_Helper.StreamMessage(String.Format("CommandEnded - {0}", e.GlobalCommandName));
            Database db = m_doc.Database;

            //Document doc = (Document)sender;
                
            List<EM_EData> enData = EW_Storage.enData;

            if (e.GlobalCommandName == "ERASE" || e.GlobalCommandName == "E") {
                m_doc.CommandEnded -= m_doc_CommandEnded;
                m_doc.CommandFailed -= m_doc_CommandEnded;
                m_doc.CommandCancelled -= m_doc_CommandEnded;
                m_dbWatcher.UndoADb(ref db);
                //m_doc.Database.ObjectOpenedForModify -= dbCallback.m_db_ObjectOpenedForModify;
                if (enData != null)
                    EM_Delete.doDelete(enData);                                        
            }
            
            else if (e.GlobalCommandName == "SAVEAS") {
                m_doc.CommandEnded -= m_doc_CommandEnded;
            }
            
            else if (e.GlobalCommandName == "GRIP_StrETCH" || e.GlobalCommandName == "MOVE") {
                m_doc.CommandEnded -= m_doc_CommandEnded;
                m_doc.CommandFailed -= m_doc_CommandEnded;
                m_doc.CommandCancelled -= m_doc_CommandEnded;
                m_dbWatcher.UndoADb(ref db);
                //m_doc.Database.ObjectOpenedForModify -= dbCallback.m_db_ObjectOpenedForModify;
                if (enData != null)
                    EM_StretchMove.doStretchMove(enData);
            }else if (e.GlobalCommandName == "QUIT") {
            }else if (e.GlobalCommandName == "CLOSE") {
                m_doc.CommandEnded -= m_doc_CommandEnded;
                if (!m_doc.Name.Contains("Drawing1.dwg")) {
                    List<string> docList = EM_DocList.emDockList.docList;
                    try {
                        docList.Remove(m_doc.Name);
                    }
                    catch (System.Exception ex){
                        BaseObjs.writeDebug(ex.Message + " EM_EventsDoc.cs: line: 201");
                    }
                }
            }else if (e.GlobalCommandName == "OPEN") {
                m_doc.CommandEnded -= m_doc_CommandEnded;
            }
        }
    }
}
