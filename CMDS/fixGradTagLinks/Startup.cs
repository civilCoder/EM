using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(fixGradeTagLinks.Startup))]

namespace fixGradeTagLinks
{
    public class Startup : IExtensionApplication
    {
        private DocumentCollection m_docMan = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager;
        private Document activeDoc = null;

        public void Initialize()
        {
            activeDoc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            activeDoc.Editor.WriteMessage("\nfixGradeTagLinks.dll Loaded\n");

            DocReactor.DocumentActivated += DocReactor_DocumentActivated;
        }

        public void Terminate()
        {
        }

        private void DocReactor_DocumentActivated(object sender, DocumentCollectionEventArgs e)
        {
            //activeDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            //if (activeDoc.Name.Contains("CGP") || activeDoc.Name.Contains("GCAL"))
            //{
            //    bool exists;
            //    ObjectId idDict = Dict.getNamedDictionary("CLKS", out exists);
            //    if (!exists)
            //    {
            //        DialogResult res = MessageBox.Show("Application to check link data is ready to execute.  Proceed?", "CHECK GRADETAG LINKS", MessageBoxButtons.YesNo);
            //        if (res == DialogResult.Yes)
            //            App.loadForm();
            //        else
            //            idDict.delete();
            //    }
            //}
        }
    }
}