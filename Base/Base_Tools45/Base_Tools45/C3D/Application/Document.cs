using System;
using System.Runtime.InteropServices;

// AutoCAD References
using acadAppSvcsCore = Autodesk.AutoCAD.ApplicationServices.Core;
using acadAppSvcs = Autodesk.AutoCAD.ApplicationServices;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

using Tei.Base.Application;

// Civil 3D References
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;

namespace Base_Tools.C3d.Application {

    /// <summary>
    ///
    /// </summary>
    public class Document
    {
        private acadAppSvcs.Document m_AcadDocument;
        private CivilDocument m_CivilDocument;
        private Transaction m_ActiveTransaction;
        private acadAppSvcs.DocumentLock m_DocumentLock;

        #region Documents and Database Access
        
        internal Document(acadAppSvcs.Document acadDoc)
        {
            m_AcadDocument = acadDoc;
            m_CivilDocument = CivilDocument.GetCivilDocument(m_AcadDocument.Database);
            m_DocumentLock = null;
            m_ActiveTransaction = null;
        }

        public string Name
        {
            get
            {
                return m_AcadDocument.Name;
            }
        }

        public ObjectNodeProvider NodeProvider
        {
            get
            {
                return new ObjectNodeProvider(this);
            }
        }

        public void Activate()
        {
            DocumentManager._activateDocument(this);
        }

        public DocumentLock lockDock()
        {
            if(m_DocumentLock == null)
            {
                m_DocumentLock = new DocumentLock(this);
            }
            return m_DocumentLock;
        }

        public Transaction startTransaction()
        {
            if(m_ActiveTransaction == null)
            {
                m_ActiveTransaction = new Transaction(this);
            }
            return m_ActiveTransaction;
        }
        
        internal acadAppSvcs.Document _acadDoc
        {
            get
            {
                return m_AcadDocument;
            }
        }

        internal CivilDocument _civDoc
        {
            get
            {
                return m_CivilDocument;
            }
        }

        internal void _closeTransaction()
        {
            m_ActiveTransaction = null;
        }
       
//----------------------------------------------------------------------------------------------------        
        /// <summary>
        /// get Editor
        /// </summary>
        public Editor
        ed
        {
            get
            {
                return _acadDoc.Editor;
            }
        }
        
        #endregion Documents and Database Access
        
        #region Civil 3D Objects Access
        
        /// <summary>
        /// get CogoPointCollection
        /// </summary>
        
        public CogoPointCollection
        _cogopoints
        {
            get
            {
                return _civDoc.CogoPoints;
            }
        }
        
        #endregion Civil 3D Objects Access
                
        /// <summary>
        /// write message to command line
        /// </summary>
        /// <param name="msg"></param>
       
        public void
            write(string msg)
        {
            _acadDoc.Editor.WriteMessage(msg);
            
        }
                              
        /// <summary>
        /// 
        /// </summary>
        public ObjectId
            activeViewport
        {
            get
            {
                return ed.ActiveViewportId;
            }           
        }

        /// <summary>
        /// 
        /// </summary>
        public void
            acadActivate()
        {
            SetFocus(
              Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle);
        }

        [DllImport("user32.dll")]
        private static extern
            IntPtr SetFocus(IntPtr hWnd);
       
    }
}