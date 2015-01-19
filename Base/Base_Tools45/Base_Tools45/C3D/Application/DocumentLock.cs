using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using acadAppSvcs = Autodesk.AutoCAD.ApplicationServices;

namespace Base_Tools.C3d.Application {

    public class DocumentLock : IDisposable
    {

        internal DocumentLock(Document parent)
        {
            m_ParentDocument = parent;
            m_TheDocumentLock = m_ParentDocument._acadDoc.LockDocument();
        }

        public void Dispose()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        private Document m_ParentDocument;
        private acadAppSvcs.DocumentLock m_TheDocumentLock;
    }
}
