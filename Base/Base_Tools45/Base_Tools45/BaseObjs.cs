using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static class BaseObjs
    {
        public static Output outForm;

        static BaseObjs()
        {
            if (System.Environment.UserName.ToUpper() == "JOHN")
            {
                Helper helper = new Helper();
                if (outForm != null && !outForm.IsDisposed)
                {
                    outForm.Close();
                }
                outForm = new Output();
                outForm.Show();
                Helper.StreamMessage("From BaseObjs Constructor");
            }
        }

        /// <summary>
        /// Returns the document object from where the command was
        /// launched.
        /// </summary>
        public static Document
        _acadDoc
        {
            get
            {
                return Application.DocumentManager.MdiActiveDocument;
            }
        }

        /// <summary>
        /// get DocumentCollection
        /// </summary>
        public static DocumentCollection
        _acadDocs
        {
            get
            {
                return Application.DocumentManager;
            }
        }

        public static List<string>
        _acadDocsList
        {
            get
            {
                List<string> names = new List<string>();
                foreach (Document doc in _acadDocs)
                    names.Add(doc.Name);
                return names;
            }
        }

        /// <summary>
        /// Return the Civil 3D Document instance.
        /// </summary>
        public static CivilDocument
        _civDoc
        {
            get
            {
                return CivilApplication.ActiveDocument;
            }
        }

        public static CogoPointCollection
        _cogopoints
        {
            get
            {
                return _civDoc.CogoPoints;
            }
        }

        /// <summary>
        /// Returns the current database from where the command
        /// was launched.
        /// </summary>
        public static Database
        _db
        {
            get
            {
                return _acadDoc.Database;
            }
        }

        /// <summary>
        /// Returns the Editor instance for the current document.
        /// </summary>
        public static Editor
        _editor
        {
            get
            {
                return _acadDoc.Editor;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static Autodesk.AutoCAD.DatabaseServices.TransactionManager
        _transactionManagerDb
        {
            get
            {
                return _db.TransactionManager;
            }
        }

        public static Extents3d
        _getExtents
        {
            get{
                Point3d pnt3dMin = BaseObjs._db.Extmin;
                Point3d pnt3dMax = BaseObjs._db.Extmax;

                Extents3d ext3d = new Extents3d(pnt3dMin, pnt3dMax);
                return ext3d;              
            }
        }


        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static Autodesk.AutoCAD.ApplicationServices.TransactionManager
        _transactionManagerDoc
        {
            get
            {
                return _acadDoc.TransactionManager;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static ObjectId
        activeViewport
        {
            get
            {
                return _editor.ActiveViewportId;
            }
        }

        public static Plane
        xyPlane
        {
            get
            {
                return Db.getUcsPlane(_db);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static string
        docFullName
        {
            get
            {
                return _acadDoc.Name;
            }
        }

        public static string
        docName
        {
            get
            {
                return Path.GetFileName(_acadDoc.Name);
            }
        }

        public static string userName
        {
            get
            {
                return Environment.UserName;
            }
        }

        public static void
        _closeDoc(Document acadDoc, bool save)
        {
            if (save)
                acadDoc.CloseAndSave(acadDoc.Name);
            else
                acadDoc.CloseAndDiscard();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="acadDoc"></param>
        public static void
        _saveDoc(Document acadDoc)
        {
            acadDoc.Database.Save();
        }

        public static void
        save(this Document doc)
        {
            object acadDoc = doc.GetAcadDocument();
            acadDoc.GetType().InvokeMember("Save", BindingFlags.InvokeMethod, null, acadDoc, new object[0]);
        }

        public static void
        saveas(this Document doc, string name)
        {
            using (doc.LockDocument())
            {
                doc.Database.UpdateThumbnail = 15;
                doc.Database.SaveAs(name, true, DwgVersion.Current, doc.Database.SecurityParameters);
                doc.Database.CloseInput(false);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static void
        acadActivate()
        {
            SetFocus(
                Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle);
        }

        //public static void
        //Activate(this Document doc){
        //    dynamic acadDoc = doc.GetAcadDocument();
        //    acadDoc.Activate();
        //}

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static string
        jobNumber()
        {
            int jn = 0;
            int res = 0;
            string nameFile = Path.GetFileNameWithoutExtension(docName);
            bool isNumeric = false;
            int n = 0;
            do
            {
                n++;
                string buf = nameFile.Substring(0, n);
                isNumeric = buf.isInteger(out res);
                if (isNumeric)
                    jn = res;
            } while (isNumeric);

            return jn.ToString();
        }

        public static string
        jobNumber(this string nameDwg)
        {
            int jn = 0;
            int res = 0;
            string nameFile = Path.GetFileNameWithoutExtension(nameDwg);
            bool isNumeric = false;
            int n = 0;
            do
            {
                n++;
                string buf = nameFile.Substring(0, n);
                isNumeric = buf.isInteger(out res);
                if (isNumeric)
                    jn = res;
            } while (isNumeric);

            return jn.ToString();
        }

        public static Document
        openDwg(string strName)
        {
            return Application.DocumentManager.Open(strName);
        }

        public static void
        reloadXrefs(ObjectIdCollection ids)
        {
            _db.ReloadXrefs(ids);
        }

        //subroutine methos of calling command
        //---------------------------------------------------------------------
        //  ed.Command("_.INSERT", "BLOCKNAME", "0, 0", 1, 1, 0);
        //
        //
        //
        //---------------------------------------------------------------------

        //coroutine methos of calling command
        //---------------------------------------------------------------------
        // await ed.CommandAsync("_.INSERT", "BLOCKNAME", Editor.PauseToken, 1, 1, 0);
        //
        //
        //
        //---------------------------------------------------------------------
        
        
        public static void
        sendCommand(string command)
        {
            acedPostCommand(command);
        }

        /// <summary>
        /// Starts a new transaction in the current database.
        /// </summary>
        /// <returns></returns>
        public static Transaction
        startTransactionDb()
        {
            return HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static Transaction
        startTransactionDoc()
        {
            return _acadDoc.TransactionManager.StartTransaction();
        }

        /// <summary>
        ///
        /// </summary>
        public static void
        updateGraphics()
        {
            _acadDoc.updateGraphics();
        }

        public static void
        regen()
        {
            _editor.Regen();
        }

        /// <summary>
        /// Writes the specified message to the Editor output window.
        /// </summary>
        /// <param name="message"></param>
        public static void
        write(string message)
        {
            _editor.WriteMessage(message);
        }

        public static void
        writeDebug(string msg)
        {
            string nameFile = @"M:\John\Debug\debug.log";
            string user = Environment.UserName;
            msg = string.Format("{0} {1} {2}", user, DateTime.Now, msg);
            if (!File.Exists(nameFile))
            {
                try
                {
                    using (StreamWriter sw = File.CreateText(nameFile))
                    {
                        sw.WriteLine(msg);
                    }
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " BaseObjs.cs: line: 406");
                }
            }
            else
            {
                try
                {
                    using (StreamWriter sw = File.AppendText(nameFile))
                    {
                        sw.WriteLine(msg);
                    }
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " BaseObjs.cs: line: 420");
                }
            }

            if (user.ToUpper() == "JOHN")
                Helper.StreamMessage(string.Format("\n{0}", msg));
        }

        public static string getDiagnosticsInformationForCurrentFrame()
        {
            string msg = string.Empty;
            StackTrace st = new StackTrace(new StackFrame(true));
            StackFrame sf = st.GetFrame(1);
            msg = string.Format("File: {0}, Method: {1}, Line Number: {2}", sf.GetFileName(), sf.GetMethod(), sf.GetFileLineNumber());
            return msg;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr
        SetFocus(IntPtr hWnd);

        [DllImport("accore.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "?acedPostCommand@@YAHPEB_W@Z")]
        extern static private int acedPostCommand(string strExpr);

        [DllImport("accore.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "acedCmd")]
        private static extern int acedCmd(System.IntPtr vlist);

        public static void runMacro(string name)
        {
            ResultBuffer rb = new ResultBuffer(){
                new TypedValue(5005, "_.vbarun"),
                new TypedValue(5005, name)
            };
            acedCmd(rb.UnmanagedObject);
        }

        public static void loadDVB(string name)
        {
            ResultBuffer rb = new ResultBuffer(){
                new TypedValue(5005, "_.vbaload"),
                new TypedValue(5005, name)
            };
            acedCmd(rb.UnmanagedObject);
        }

    }
}
