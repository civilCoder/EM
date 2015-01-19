namespace EventManager
{
    public class EM_EventsWatcher
    {
        public static EM_EventsDocument m_docWatcher;
        public static EM_DocManager m_docManager;

        //public static EM_EventsDatabase             m_dbWatcher;
        public EM_EventsWatcher()
        {
            m_docManager = new EM_DocManager();
            m_docWatcher = new EM_EventsDocument();
            //m_dbWatcher  = new EM_EventsDatabase();
        }

        public static void documentActivated(ref Autodesk.AutoCAD.ApplicationServices.Document doc)
        {
            if (m_docWatcher != null)
            {
                m_docWatcher.addDoc(ref doc);
                m_docWatcher.Do();
            }
            //if (m_dbWatcher != null){
            //    Database db = doc.Database;
            //    m_dbWatcher.addDb(ref db);
            //}
        }

        public static void documentToBeDeactivated(ref Autodesk.AutoCAD.ApplicationServices.Document doc)
        {
            if (m_docWatcher != null)
            {
                m_docWatcher.removeDoc(ref doc);
            }
        }
    }
}
