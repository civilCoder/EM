using Autodesk.AutoCAD.ApplicationServices;

namespace fixGradeTagLinks
{
    public static class DocReactor
    {
        private static Document m_active = null;

        static DocReactor()
        {
            Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.DocumentActivated += documentActivated;
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