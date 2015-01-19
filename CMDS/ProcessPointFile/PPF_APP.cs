using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Base_Tools45;
using Base_VB;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using ObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;
using OpenFileDialog = Autodesk.AutoCAD.Windows.OpenFileDialog;

namespace ProcessPointFile
{
    public static class PPF_APP
    {
        public static PPF_PntDesc fPntDesc;
        private const string TemplateTOP = @"R:\TSet\Template\CIVIL3D2014\TOPO_20.dwt";

        public static bool CANCEL { get; set; }

        public static List<string> DESCLIST { get; set; }

        public static string DWGNAME { get; set; }

        public static string DWGPATH { get; set; }

        public static string FILEOUT { get; set; }

        public static string JN { get; set; }

        public static string PATH { get; set; }

        public static List<PPF_PntData> PNTDATA { get; set; }

        public static string TOP { get; set; }

        public static string TXTFILE { get; set; }

        public static void
        getDrawing(string pathDwg)
        {
            Document acadDoc;
            bool fileExists = FileSystem.FileExists(pathDwg);
            DialogResult pr;

            var names = BaseObjs._acadDocsList;

            if (fileExists)
            {
                if (names.Contains(Path.GetFileName(pathDwg)))
                {
                    foreach (Document doc in BaseObjs._acadDocs)
                    {
                        if (doc.Name == Path.GetFileName(pathDwg))
                        {
                            BaseObjs._acadDocs.MdiActiveDocument = doc;
                            break;
                        }
                    }
                }
                else
                {
                    pr = MessageBox.Show(string.Format("{0} {1} .dwg exists - Open Y/N?: ", JN, TOP), "", MessageBoxButtons.YesNo);
                    if (pr == DialogResult.Yes)
                    {
                        acadDoc = DocumentCollectionExtension.Open(BaseObjs._acadDocs, pathDwg, false);
                    }
                    else
                        return;
                }
            }
            else
            {
                acadDoc = DocumentCollectionExtension.Add(BaseObjs._acadDocs, TemplateTOP);
                acadDoc.saveAsDoc(pathDwg);
                Application.DocumentManager.MdiActiveDocument = acadDoc;
            }

            myUtility.CreateSurveyProject(JN);            

            bool exists;
            using (BaseObjs._acadDoc.LockDocument())
            {
                ObjectId idDictPPF2 = Dict.getNamedDictionary("PPF2", out exists);

                if (exists)
                    Dict.deleteXRec(idDictPPF2, "strTOP");

                TypedValue[] tvs = new TypedValue[4];
                tvs.SetValue(new TypedValue(1000, JN), 0);
                tvs.SetValue(new TypedValue(1000, TOP), 1);
                tvs.SetValue(new TypedValue(1000, TXTFILE), 2);
                tvs.SetValue(new TypedValue(1000, string.Format("{0}{1}{2}_mod.txt", Path.GetDirectoryName(TXTFILE), @"\", Path.GetFileNameWithoutExtension(TXTFILE))), 3);
                ResultBuffer rb = new ResultBuffer(tvs);

                Dict.addXRec(idDictPPF2, "strTOP", rb);
            }
        }

        public static void
        getFileToProcess()
        {
            OpenFileDialog ofd = new OpenFileDialog("SELECT POINT FILE", "default", "txt", "Select Point File to Import",
                OpenFileDialog.OpenFileDialogFlags.DefaultIsFolder);

            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
                TXTFILE = ofd.Filename;
            else
            {
                return;
            }
            DWGNAME = Path.GetFileNameWithoutExtension(TXTFILE);
            if (DWGNAME != "")
            {
                JN = DWGNAME.Substring(0, 4);
                TOP = DWGNAME.Substring(4);
                PATH = Misc.resolvePath(JN);
                DWGNAME = string.Format("{0}.dwg", DWGNAME);
                DWGPATH = string.Format("{0}{1}{2}{3}", PATH, JN, @"\", DWGNAME);
            }
        }

        public static void
        runApp()
        {
            PPF_APP.getFileToProcess();
            if ((TXTFILE == null))
                return;

            //FILEOUT = PPF_Process.procPntFile(TXTFILE);
            PPF_Process.procPntFile(TXTFILE);
            PPF_PostProcess.postProcPntFile();

            fPntDesc = null;

            fPntDesc = PPF_PntDesc.frmPntDesc;
            PPF_PntDesc.loadForm();
            Application.ShowModelessDialog(fPntDesc);
            //fPntDesc.ShowDialog();
            //Point Description form
            //Application.ShowModalDialog(fPntDesc);
        }

        public static T
        With<T>(this T item, Action<T> action)
        {
            action(item);
            return item;
        }
    }
}