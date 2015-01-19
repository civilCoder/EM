using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using acWindows = Autodesk.AutoCAD.Windows;

namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static class FileManager
    {
        public static class BrowseFolder{

            private const int BIF_RETURNONLYFSDIRS = 0x1;
            private const int BIF_DONTGOBELOWDOMAIN = 0x2;
            private const int BIF_RETURNFSANCESTORS = 0x8;
            private const int BIF_BROWSEFORCOMPUTER = 0x1000;
            private const int BIF_BROWSEFORPRINTER = 0x2000;
            private const int BIF_BROWSEINCLUDEFILES = 0x4000;
            private const int MAX_PATH = 260;

            public struct BrowseInfo
            {
                public int hOwner;
                public int pidlRoot;
                public string pszDisplayName;
                public string lpszINSTRUCTIONS;
                public int ulFlags;
                public int lpfn;
                public int lParam;
                public int iImage;
            }

            public struct SHFILEOPSTRUCT
            {
                public int hwnd;
                public int wFunc;
                public string pFrom;
                public string pTo;
                public short fFlags;
                public bool fAnyOperationsAborted;
                public int hNameMappings;
                public string lpszProgressTitle;
            }

            [DllImport("shell32.dll", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
            public static extern int SHGetPathFromIDListA(int pidl, string pszBuffer);

            [DllImport("shell32.dll", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
            public static extern int SHBrowseForFolderA(BrowseInfo lpBrowseInfo);

            static public string
            getFolder(string Caption = "")
            {
                string returnValue = "";

                BrowseInfo structBrowseInfo = new BrowseInfo();

                string FolderName = "";
                int ID = 0;
                int Res = 0;

                structBrowseInfo.hOwner = 0;
                structBrowseInfo.pidlRoot = 0;
                structBrowseInfo.pszDisplayName = new string(Convert.ToChar(0x0), MAX_PATH);
                structBrowseInfo.lpszINSTRUCTIONS = Caption;
                structBrowseInfo.ulFlags = BIF_RETURNONLYFSDIRS;
                structBrowseInfo.lpfn = 0;

                FolderName = new string(Convert.ToChar(0x0), MAX_PATH);
                ID = System.Convert.ToInt32(SHBrowseForFolderA(structBrowseInfo));
                if (ID != 0)
                {
                    Res = System.Convert.ToInt32(SHGetPathFromIDListA(ID, FolderName));
                    if (Res != 0)
                    {
                        returnValue = FolderName.Substring(0, FolderName.IndexOf(Convert.ToChar(0x0)) + 0);
                    }
                }

                return returnValue;

            }
        }


        public static string[]
        getFiles(string defExt, string title, string filter, string dir, bool multi = true){

            System.Windows.Forms.OpenFileDialog ofd = new OpenFileDialog();

            ofd.DefaultExt = defExt;
            ofd.Filter = filter;
            ofd.InitialDirectory = dir;
            ofd.Multiselect = multi;
            ofd.RestoreDirectory = true;
            ofd.FilterIndex = 2;
            ofd.Title = title;

            DialogResult dr = ofd.ShowDialog();
            string[] resFiles = null;

            if (dr == DialogResult.OK)
                resFiles = ofd.FileNames;

            return resFiles;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="title"></param>
        /// <param name="nameDefault"></param>
        /// <param name="extension"></param>
        /// <param name="nameDialog"></param>
        /// <param name="selectMultiple"></param>
        /// <returns></returns>
        public static List<string>
        getFile(string title, string nameDefault, string extension, string nameDialog, bool selectMultiple = false)
        {
            List<string> files = new List<string>();

            acWindows.OpenFileDialog.OpenFileDialogFlags flags;
            //if (selectMultiple)
            //{
            //    flags = acWindows.OpenFileDialog.OpenFileDialogFlags.AllowMultiple;
            //}
            //else
            //{
            //    flags = acWindows.OpenFileDialog.OpenFileDialogFlags.DefaultIsFolder;
            //}

            flags = acWindows.OpenFileDialog.OpenFileDialogFlags.AllowMultiple | 
                    acWindows.OpenFileDialog.OpenFileDialogFlags.DefaultIsFolder | 
                    acWindows.OpenFileDialog.OpenFileDialogFlags.ForceDefaultFolder;

            string[] target;
            acWindows.OpenFileDialog ofd = new acWindows.OpenFileDialog(title, 
                                                                        nameDefault, 
                                                                        extension, 
                                                                        nameDialog, 
                                                                        flags);
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                target = ofd.GetFilenames();
                foreach (string file in target)
                {
                    files.Add(file);
                }
                return files;
            }
            else
                return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="name"></param>
        /// <param name="filter"></param>
        /// <param name="directory"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string
        getFilesWithEditor(string caption, string name, string filter, string directory, string message)
        {
            Editor ed = BaseObjs._editor;
            PromptOpenFileOptions pofo = null;
            try
            {
                pofo = new PromptOpenFileOptions("Select a file using Editor. GetFileNameForOpen()");
                pofo.DialogCaption = caption;
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " FileManager.cs: line: 183");
            }
            pofo.DialogName = name;
            pofo.Filter = filter;
            pofo.InitialDirectory = directory;
            pofo.Message = message;
            
            PromptFileNameResult pfnr = ed.GetFileNameForOpen(pofo);
            if (pfnr.Status == PromptStatus.OK)
            {
                return pfnr.StringResult;
            }
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int
        getFileStatus(string name)
        {
            DocumentCollection acadDocs = BaseObjs._acadDocs;

            foreach (Document acadDoc in acadDocs)
            {
                if (acadDoc.Name == name)
                {
                    FileInfo fInfo = new FileInfo(acadDoc.Name);
                    if (fInfo.IsReadOnly)
                    {
                        MessageBox.Show("Target file is already open on this machine as READONLY - close file and retry.  Exiting....");
                        return (int)filestatus.isOpenLocalReadOnly;
                    }
                    return (int)filestatus.isOpenLocal;
                }
            }
            Autodesk.AutoCAD.ApplicationServices.WhoHasInfo whoHasInfo = Autodesk.AutoCAD.ApplicationServices.Application.GetWhoHasInfo(name);
            if (whoHasInfo.IsFileLocked)
            {
                MessageBox.Show(string.Format("User: {0} has target file: {1} open. Exiting...", whoHasInfo.UserName, name));
                return (int)filestatus.isLocked;
            }
            return (int)filestatus.isAvailable;
        }

        public static int
        getFileStatus(string name, out string nameUser)
        {
            nameUser = "";
            DocumentCollection acadDocs = BaseObjs._acadDocs;
            foreach (Document acadDoc in acadDocs)
            {
                if (acadDoc.Name == name)
                {
                    FileInfo fInfo = new FileInfo(acadDoc.Name);
                    if (fInfo.IsReadOnly)
                    {
                        return (int)filestatus.isOpenLocalReadOnly;
                    }
                    return (int)filestatus.isOpenLocal;
                }
            }
            Autodesk.AutoCAD.ApplicationServices.WhoHasInfo whoHasInfo = Autodesk.AutoCAD.ApplicationServices.Application.GetWhoHasInfo(name);
            if (whoHasInfo.IsFileLocked)
            {
                nameUser = whoHasInfo.UserName;
                return (int)filestatus.isLocked;
            }
            return (int)filestatus.isAvailable;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nameFull"></param>
        /// <param name="isOpen"></param>
        /// <returns></returns>
        public static Document
        getDoc(string nameFull, bool isOpen = false, bool activate = false)
        {
            Document acDoc = null;
            DocumentCollection acadDocs = BaseObjs._acadDocs;
            if(isOpen)
            {
                foreach (Document doc in BaseObjs._acadDocs)
                {
                    if (doc.Name == nameFull)
                    {
                        acDoc = doc;
                        if (activate)
                            acadDocs.MdiActiveDocument = acDoc;
                        break;
                    }
                }
            }
            else
            {
                try
                {
                    acDoc = acadDocs.Open(nameFull, false);
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " FileManager.cs: line: 288");
                }
            }
            return acDoc;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="dbCURR"></param>
        /// <param name="dbTAR"></param>
        /// <returns></returns>
        public static bool
        transferObjects(ObjectId[] idss, Database dbCURR, Database dbTAR)
        {
            bool success = false;
            ObjectIdCollection ids = new ObjectIdCollection();
            foreach (ObjectId id in idss)
                ids.Add(id);
            try
            {
                try
                {
                    using (BaseObjs._acadDoc.LockDocument())
                    {
                        using (Transaction tr = dbTAR.TransactionManager.StartTransaction())
                        {
                            BlockTable bt = (BlockTable)tr.GetObject(dbTAR.BlockTableId, OpenMode.ForRead);
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                            IdMapping acIdMap = new IdMapping();
                            dbCURR.WblockCloneObjects(ids, btr.ObjectId, acIdMap, DuplicateRecordCloning.Ignore, false);

                            tr.Commit();
                            MessageBox.Show(string.Format("{0} Objects transfered to {1}", ids.Count.ToString(), dbTAR.Filename));
                            success = true;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " FileManager.cs: line: 329");
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " FileManager.cs: line: 334");
            }
            return success;
        }
    }
}
