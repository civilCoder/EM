using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

using System;
using System.Collections;
using System.Collections.Generic;

using Base_Tools45;
using btp = Base_Tools45.Pub;

namespace EventManager
{
    public class EM_EventsDocument
    {
        public static EM_EventsDatabase m_dbWatcher;

        private Document m_doc;
        private Hashtable m_docsTable;
        private bool m_bDone;

        public EM_EventsDocument()
        {
            m_bDone = false;
            m_docsTable = new Hashtable();
            collectAllDocs();
            Do();
            numDocEvents++;
            EM_Helper.StreamMessage(string.Format("DocEvent Added: {0}", numDocEvents));
            m_dbWatcher = new EM_EventsDatabase();

        }


        ~EM_EventsDocument()
        {
            numDocEvents--;
            EM_Helper.StreamMessage(string.Format("DocEvents Removed: {0}", numDocEvents));
        }

        public static int numDocEvents { get; protected set; }

        public void collectAllDocs()
        {
            try
            {
                DocumentCollection m_docs = Application.DocumentManager;
                IEnumerator docEnum = m_docs.GetEnumerator();
                while (docEnum.MoveNext())
                {
                    Document doc = (Document)docEnum.Current;
                    addDoc(ref doc);
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_EventsDocument.cs: line: 59");
            }
        }

        public void addDoc(ref Document doc)
        {
            if (!m_docsTable.ContainsKey(doc))
                m_docsTable.Add(doc, new EM_CBoolClass(false));
            if (m_dbWatcher != null)
            {
                Database db = doc.Database;
                m_dbWatcher.addDb(ref db);
            }

            List<string> docList = EM_DocList.emDockList.docList;
            if (!docList.Contains(doc.Name)){
                    docList.Add(doc.Name);
                bool exists = false;
                if(doc.Name.Contains("CGP") || doc.Name.Contains("GCAL")){                    
                    ObjectId idDict = Dict.getNamedDictionary("checkCOs", out exists);
                    if(!exists){
                        Misc.updateXDataOnCOs();
                    }
                    Dict.deleteDictionary("checkGSs");
                    idDict = Dict.getNamedDictionary("checkGSs", out exists);
                    if (!exists)
                    {
                        LdrText.LdrText_Misc.updateXDataOnGSs();
                    }                        
                      
                }
            }
        }

        public void removeDoc(ref Document doc)
        {
            if (m_docsTable.ContainsKey(doc))
            {
                UndoADoc(ref doc);
                m_docsTable.Remove(doc);
            }
        }

        public void Do()
        {
            if (m_bDone == false)
            {
                m_bDone = true;
            }
            else
            {
            }

            try
            {
                foreach (DictionaryEntry entry in m_docsTable)
                {
                    EM_CBoolClass c_bClassVar = (EM_CBoolClass)entry.Value;

                    if (c_bClassVar.ToString().ToLower() == "true")
                        continue;
                    m_doc = (Document)entry.Key;
                    m_doc.CommandWillStart += m_doc_CommandWillStart;

                    c_bClassVar.val = true;
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_EventsDocument.cs: line: 128");
            }
        }

        public void Undo()
        {
            if (m_bDone == false)
                return;
            else
                m_bDone = false;
            try
            {
                IDictionaryEnumerator docsEnumerator = m_docsTable.GetEnumerator();
                while (docsEnumerator.MoveNext())
                {
                    DictionaryEntry entry = (DictionaryEntry)docsEnumerator.Current;

                    EM_CBoolClass c_bClassVar = (EM_CBoolClass)entry.Value;

                    if (c_bClassVar.ToString().ToLower() == "false")
                        continue;

                    m_doc = (Document)entry.Key;
                    m_doc.CommandWillStart -= m_doc_CommandWillStart;

                    c_bClassVar.val = false;
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_EventsDocument.cs: line: 158");
            }
        }

        public void UndoADoc(ref Document doc)
        {
            try
            {
                if (!m_docsTable.ContainsKey(doc))
                    return;
                EM_CBoolClass c_bClassVar = (EM_CBoolClass)m_docsTable[doc];
                if (c_bClassVar.ToString().ToLower() == "false")
                    return;

                m_doc = doc;
                m_doc.CommandWillStart -= m_doc_CommandWillStart;

                c_bClassVar.val = false;
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_EventsDocument.cs: line: 179");
            }
        }

        private void m_doc_CommandWillStart(object sender, CommandEventArgs e)
        {
            EM_Helper.StreamMessage(String.Format("CommandWillStart - {0}", e.GlobalCommandName));
            Database db = m_doc.Database;

            EW_Storage.enData = new List<EM_EData>();
            if (e.GlobalCommandName == "ERASE" || e.GlobalCommandName == "E")
            {
                m_doc.CommandEnded += m_doc_CommandEnded;
                m_doc.CommandFailed += m_doc_CommandEnded;
                m_doc.CommandCancelled += m_doc_CommandEnded;
                m_dbWatcher.DoADb(ref db);
            }
            else if (e.GlobalCommandName == "GRIP_STRETCH" || e.GlobalCommandName == "MOVE")
            {
                m_doc.CommandEnded += m_doc_CommandEnded;
                m_doc.CommandFailed += m_doc_CommandEnded;
                m_doc.CommandCancelled += m_doc_CommandEnded;
                m_dbWatcher.DoADb(ref db);
            }
            else if (e.GlobalCommandName == "AECCEDITPIPEPROPERTIES" || e.GlobalCommandName == "AECCEDITPARENTPARTPROPERTIES")
            {
                m_doc.CommandEnded += m_doc_CommandEnded;
                m_doc.CommandFailed += m_doc_CommandEnded;
                m_doc.CommandCancelled += m_doc_CommandEnded;
                m_dbWatcher.DoADb(ref db);
            }
            else if (e.GlobalCommandName == "SAVEAS")
            {
                m_doc.CommandEnded += m_doc_CommandEnded;
            }
            else if (e.GlobalCommandName == "QUIT")
            {
            }
            else if (e.GlobalCommandName == "CLOSE")
            {
                m_doc.CommandEnded += m_doc_CommandEnded;
                Dict.deleteDictionary("cmdBC");
            }
            else if (e.GlobalCommandName == "OPEN")
            {
                m_doc.CommandEnded += m_doc_CommandEnded;
            }
        }

        private void m_doc_CommandEnded(object sender, CommandEventArgs e)
        {
            EM_Helper.StreamMessage(String.Format("CommandEnded - {0}", e.GlobalCommandName));
            Database db = m_doc.Database;

            System.Windows.Forms.Keys mods = System.Windows.Forms.Control.ModifierKeys;
            btp.shiftKey = (mods & System.Windows.Forms.Keys.Shift) > 0;
            btp.cntrlKey = (mods & System.Windows.Forms.Keys.Control) > 0;
            btp.altX = (mods & System.Windows.Forms.Keys.Alt) > 0;

            List<EM_EData> enData = EW_Storage.enData;

            if (e.GlobalCommandName == "ERASE" || e.GlobalCommandName == "E")
            {
                m_doc.CommandEnded -= m_doc_CommandEnded;
                m_doc.CommandFailed -= m_doc_CommandEnded;
                m_doc.CommandCancelled -= m_doc_CommandEnded;
                m_dbWatcher.UndoADb(ref db);
                if (enData != null)
                    EM_Delete.doDelete(enData);
            }
            else if (e.GlobalCommandName == "SAVEAS")
            {
                m_doc.CommandEnded -= m_doc_CommandEnded;
            }
            else if (e.GlobalCommandName == "GRIP_STRETCH" || e.GlobalCommandName == "MOVE")
            {
                m_doc.CommandEnded -= m_doc_CommandEnded;
                m_doc.CommandFailed -= m_doc_CommandEnded;
                m_doc.CommandCancelled -= m_doc_CommandEnded;
                m_dbWatcher.UndoADb(ref db);
                if (enData != null)
                    EM_StretchMove.doStretchMove(enData);
            }
            else if (e.GlobalCommandName == "AECCEDITPIPEPROPERTIES" || e.GlobalCommandName == "AECCEDITPARENTPARTPROPERTIES")
            {
                m_doc.CommandEnded -= m_doc_CommandEnded;
                m_doc.CommandFailed -= m_doc_CommandEnded;
                m_doc.CommandCancelled -= m_doc_CommandEnded;
                m_dbWatcher.UndoADb(ref db);
                if (enData != null)
                    EM_StretchMove.doStretchMove(enData);
            }
            else if (e.GlobalCommandName == "QUIT")
            {
            }
            else if (e.GlobalCommandName == "CLOSE")
            {
                m_doc.CommandEnded -= m_doc_CommandEnded;
                if (!m_doc.Name.Contains("Drawing1.dwg"))
                {
                    List<string> docList = EM_DocList.emDockList.docList;
                    try
                    {
                        docList.Remove(m_doc.Name);
                        ObjectId idDict = Dict.dictExists("cmdFL");
                        if (idDict != ObjectId.Null)
                            Dict.setCmdDefault("cmdFL", "resBot", "FL");
                        idDict = Dict.dictExists("cmdG");
                        if (idDict != ObjectId.Null)
                        {
                            Dict.setCmdDefault("cmdG", "resBot", "FL");
                            Dict.setCmdDefault("cmdG", "resTop", "TC");
                        }
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " EM_EventsDocument.cs: line: 295");
                    }
                }
            }
            else if (e.GlobalCommandName == "OPEN")
            {
                m_doc.CommandEnded -= m_doc_CommandEnded;
            }
            EW_Storage.enData = null;
        }
    }
}
