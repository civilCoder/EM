using System;
using System.Linq;

using Autodesk.AutoCAD.ApplicationServices;
using acadAppSvcs = Autodesk.AutoCAD.ApplicationServices;
using acadAppSvcsCore = Autodesk.AutoCAD.ApplicationServices.Core;

namespace Base_Tools.C3d.Application {

    /// <summary>
    /// This class manages and provides access to Document objects.
    /// </summary>
    public class DocumentManager
    {
        private static Document m_ActiveDocument = null;
        private static acadAppSvcs.DocumentCollection m_DocumentCollection = null;

        /// <summary>
        /// Returns the active document.
        /// </summary>
        public Document
            ActiveDocument
        {
            get
            {
                if (m_ActiveDocument == null)
                {
                    m_ActiveDocument = new Document(acadAppSvcsCore.Application.DocumentManager.MdiActiveDocument);
                }
                return m_ActiveDocument;
            }
        }

        /// <summary>
        /// Opens a document from a DWG file name. Opening a DWG creates a new Document object
        /// and activates it.
        /// </summary>
        /// <param name="fileName">DWG file to open.</param>
        /// <returns>Returns the created Document object.</returns>
        public Document
            OpenDocument(string fileName)
        {
            acadAppSvcs.Document acadDoc = acadAppSvcsCore.Application.DocumentManager.Open(fileName);
            return new Document(acadDoc);
        }

        /// <summary>
        /// Activates the specified Document.
        /// </summary>
        /// <param name="doc">Document to be activated.</param>
        internal static void
            _activateDocument(Document doc)
        {
            acadAppSvcsCore.Application.DocumentManager.MdiActiveDocument = doc._acadDoc;
            m_ActiveDocument = doc;
        }

        public DocumentCollection
        acadDocs
        {         
            get
            {
                m_DocumentCollection = acadAppSvcs.DocumentCollection();
                return m_DocumentCollection;
               
            }
        }
    }
}
