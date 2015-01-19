using Autodesk.AutoCAD.ApplicationServices;

namespace EventManager
{
    public static class EM_DocActivated
    {
        private static Document m_active = null;

        static EM_DocActivated()
        {
            Application.DocumentManager.DocumentActivated += documentActivated;
        }

        public static event DocumentCollectionEventHandler DocumentActivated = null;

        private static void documentActivated(object sender, DocumentCollectionEventArgs e)
        {
            if (m_active != e.Document)
            {
                if (e.Document != null && DocumentActivated != null)
                    DocumentActivated(sender, e);

                m_active = e.Document;
            }
        }
    }
}
