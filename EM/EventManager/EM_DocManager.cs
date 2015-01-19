using Autodesk.AutoCAD.ApplicationServices;
using Base_Tools45;

namespace EventManager
{
    public class EM_DocManager
    {
        private Grading.Grading_Palette gPalette = Grading.Grading_Palette.gPalette;

        private static Grading.myForms.GradeSite fGrading;
        private static Grading.myForms.GradeFloor fGradeFloor;

        public EM_DocManager()
        {
            Do();
        }

        public void Do()
        {
            try
            {               
                DocumentCollection m_docs = Application.DocumentManager;
                //m_docs.DocumentActivated += m_docs_DocumentActivated;
                EM_DocActivated.DocumentActivated += m_docs_DocumentActivated;
                m_docs.DocumentToBeDeactivated += m_docs_DocumentToBeDeactivated;
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_DocManager.cs: line: 29");
            }
        }

        private void m_docs_DocumentToBeDeactivated(object sender, DocumentCollectionEventArgs e)
        {
            try
            {
                Document doc = e.Document;
                EM_EventsWatcher.documentToBeDeactivated(ref doc);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_DocManager.cs: line: 42");
            }
        }

        private void m_docs_DocumentActivated(object sender, DocumentCollectionEventArgs e)
        {
            Document doc = e.Document;
            try
            {
                EM_EventsWatcher.documentActivated(ref doc);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_DocManager.cs: line: 55");
            }
            try
            {
                if (gPalette != null)
                {
	                fGradeFloor = gPalette.pGradeFloor;
                }else{
                    BaseObjs.writeDebug("gPalette is null");

                }
                if (fGradeFloor == null)
                {
                    BaseObjs.writeDebug("fGradeFloor is null");
                }
                else
                {
                    fGradeFloor.Initialize_Form();

                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_DocManager.cs: line: 78");
            }
            try
            {
                fGrading = gPalette.pGrading;
                fGrading.Initialize_Form();
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_DocManager.cs: line: 87");
            }


        }
    }
}
