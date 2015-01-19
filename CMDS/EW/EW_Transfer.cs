using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Base_Tools45;
using System.IO;
using System.Collections.ObjectModel;

namespace EW {
    static public class EW_Transfer {

        static public void transferObjs(string option) {

            string defExt = ".dwg";
            string title = string.Format("Transfer {0}", option);
            string filter = "All Drawings (*.dwg)|*.dwg";
            string dir = Path.GetDirectoryName(BaseObjs.docFullName);

            string[] files = FileManager.getFiles(defExt, title, filter, dir);

            if (files == null || files.Length == 0)
                return;

            Document docTar = BaseObjs._acadDoc;
            Database dbTar = docTar.Database;

            Document docSrc = null;
            Database dbSrc = null;

            string nameFile = files[0];
            string nameUser = "";
            int status = FileManager.getFileStatus(nameFile, out nameUser);
            switch (status) {
                case (int)filestatus.isOpenLocalReadOnly:
                case (int)filestatus.isOpenLocal:
                    //connect to db and make transfer
                    foreach (Document doc in BaseObjs._acadDocs) {
                        if (doc.Name == nameFile) {
                            docSrc = doc;
                            dbSrc = docSrc.Database;
                        }
                    }
                    break;
                case (int)filestatus.isAvailable:
                    //open and make transfer
                    docSrc = DocumentCollectionExtension.Open(BaseObjs._acadDocs, nameFile, true);
                    dbSrc = docSrc.Database;
                    break;
                case (int)filestatus.isLocked:
                    //open readonly
                    docSrc = DocumentCollectionExtension.Open(BaseObjs._acadDocs, nameFile, true);
                    dbSrc = docSrc.Database;
                    break;
            }

            Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument = docSrc;
            
            TypedValue[] tvs = null;
            switch(option) {
                case "AREAS":
                    tvs = new TypedValue[5] {
                        new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline)).DxfName),
                        new TypedValue((int)DxfCode.Operator, "<OR"),
                        new TypedValue((int)DxfCode.LayerName, "_XX-*"),
                        new TypedValue((int)DxfCode.LayerName, "YY-*"),
                        new TypedValue((int)DxfCode.Operator, "OR>")
                    };
                    SelectionSet ss = Select.buildSSet(tvs);
                    ObjectId[] ids = ss.GetObjectIds();

                    FileManager.transferObjects(ids, dbSrc, dbTar);
                    break;
                case "TABLE":
                    transferTableData(docSrc, docTar, dbSrc, dbTar);
                    break;
            }

            Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument = docTar;
         
            switch (status) {
                case (int)filestatus.isAvailable:
                case (int)filestatus.isLocked:
                    BaseObjs._closeDoc(docSrc, false);
                    break;
                default:
                    break;
            }
        }

        static private void
        transferTableData(Document docSrc, Document docTar, Database dbSrc, Database dbTar) {
            TypedValue[] tvs = new TypedValue[2] {
                new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Table)).DxfName),
                new TypedValue((int)DxfCode.LayerName, "ZZ_ZZ-TABLE")
            };

            SelectionFilter filter = new SelectionFilter(tvs);
            using (var tr = dbTar.TransactionManager.StartOpenCloseTransaction()) {
                PromptSelectionResult psr = BaseObjs._editor.SelectAll(filter);
                SelectionSet ss = psr.Value;
                if (ss == null)
                    return;
                ObjectId[] ids = ss.GetObjectIds();
                ObjectId idTableSrc = ids[0];
                Table tblSrc = (Table)tr.GetObject(idTableSrc, OpenMode.ForRead);

                Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument = docTar;
                psr = BaseObjs._editor.SelectAll(filter);
                ss = psr.Value;
                ids = ss.GetObjectIds();
                ObjectId idTableTar = ids[0];
                Table tblTar = (Table)tr.GetObject(idTableTar, OpenMode.ForWrite);

                for (int i = 3; i < 14; i++) {
                    for (int j = 0; j < 2; j++) {
                        string val = tblSrc.Cells[i, j].Value.ToString();
                        if(val != string.Empty)
                            tblTar.Cells[i, j].Value = val;
                    }
                }
                for (int i = 3; i < 14; i++) {
                    for (int j = 3; j < tblSrc.Columns.Count; j++) {
                        string val = tblSrc.Cells[i, j].Value.ToString();
                        if (val != string.Empty)
                            tblTar.Cells[i, j].Value = val;
                    }
                }

                for (int i = 15; i < 23; i++)
                {
                    string val = tblSrc.Cells[i, 1].Value.ToString();
                    if (val != string.Empty)
                        tblTar.Cells[i, 1].Value = val;
                }
                tr.Commit();
            }
        }

        public static void
        editTableData(){

            EW.Forms.winInputs wInputs = EW.EW_Forms.ewFrms.wInputs;
            Application.ShowModelessWindow(wInputs);

            TypedValue[] tvs = new TypedValue[2] {
                new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Table)).DxfName),
                new TypedValue((int)DxfCode.LayerName, "ZZ_ZZ-TABLE")
            };

            SelectionFilter filter = new SelectionFilter(tvs);
            PromptSelectionResult psr = BaseObjs._editor.SelectAll(filter);
            SelectionSet ss = psr.Value;
            if (ss == null)
                return;
            ObjectId[] ids = ss.GetObjectIds();
            ObjectId idTableSrc = ids[0];

            using(var tr = BaseObjs.startTransactionDb()){
                Table tblSrc = (Table)tr.GetObject(idTableSrc, OpenMode.ForRead);
                for (int i = 3; i < 14; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        string val = tblSrc.Cells[i, j].Value.ToString();
                        //if (val != string.Empty)
                            //wInputs.Cells[i, j].Value = val;
                    }
                }
                for (int i = 3; i < 14; i++)
                {
                    for (int j = 3; j < tblSrc.Columns.Count; j++)
                    {
                        string val = tblSrc.Cells[i, j].Value.ToString();
                        //if (val != string.Empty)
                            //wInputs.Cells[i, j].Value = val;
                    }
                }

                for (int i = 15; i < 23; i++)
                {
                    string val = tblSrc.Cells[i, 1].Value.ToString();
                    //if (val != string.Empty)
                        //wInputs.Cells[i, 1].Value = val;
                }                
            }

        }
    }
}