using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

using Base_Tools45;
namespace EventManager {
    /// <summary>
    /// EM_DocManEvents.
    /// </summary>
    public class EM_DocManEvents {

        static readonly EM_DocManEvents m_docManWatcher = new EM_DocManEvents();

        public EM_DocManEvents() {
            m_docMan = Application.DocumentManager;
            Do();
        }

        private EM_DocumentEvents m_docWatcher;

        public EM_DocumentEvents docWatcher {
            get {
                return m_docWatcher;
            }
            set {
                m_docWatcher = value;
            }
        }
		
        private DocumentCollection m_docMan;

        public void Do() {
            try {
                m_docMan.DocumentCreated += callback_DocumentCreated;
                m_docMan.DocumentActivated += callback_DocumentActivated;
                m_docMan.DocumentBecameCurrent += callback_DocumentBecameCurrent;
            }
            catch (System.Exception ex){
                BaseObjs.writeDebug(ex.Message + " EM_DocManEvents.cs: line: 39");
            }
        }

        private void callback_DocumentCreated(Object sender, DocumentCollectionEventArgs e) {
            DocumentCollection docs = (DocumentCollection)sender;
            Document doc = docs.MdiActiveDocument;
            docWatcher = new EM_DocumentEvents();
            docWatcher.addDoc(ref doc);
        }
		
        private void callback_DocumentBecameCurrent(Object sender, DocumentCollectionEventArgs e) {
            DocumentCollection docs = (DocumentCollection)sender;
            Document doc = docs.MdiActiveDocument;
            docWatcher = new EM_DocumentEvents();
            docWatcher.addDoc(ref doc);
            EM_Global.DrawingUnits = Base_Tools45.Misc.getCurrAnnoScale();
        }
		
        private void callback_DocumentActivated(Object sender, DocumentCollectionEventArgs e) {
            DocumentCollection docs = (DocumentCollection)sender;
            Document doc = docs.MdiActiveDocument;
            docWatcher = new EM_DocumentEvents();
            docWatcher.addDoc(ref doc);
            EM_Global.DrawingUnits = Base_Tools45.Misc.getCurrAnnoScale();
        }
    }// end of class EM_DocManEvents
}
