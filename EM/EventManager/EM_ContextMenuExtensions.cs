using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Windows;
using Base_Tools45;
using System;

namespace EventManager
{
    public class EM_ContextMenuExtensions
    {
        private static ContextMenuExtension s_cme;

        public static void RemoveMe()
        {
            Application.RemoveDefaultContextMenuExtension(s_cme);
        }

        public static void AddMe()
        {
            try
            {
                s_cme = new ContextMenuExtension();
                s_cme.Title = "Watcher";

                MenuItem mi1 = new MenuItem("Events Output Window");
                mi1.Click += new EventHandler(callback_OnClick1);
                s_cme.MenuItems.Add(mi1);

                Application.AddDefaultContextMenuExtension(s_cme);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_ContextMenuExtensions.cs: line: 32");
            }
        }

        public static void showOutput()
        {
            try
            {
                if (EM_Startup.outForm != null && !EM_Startup.outForm.IsDisposed)
                {
                    EM_Startup.outForm.Close();
                }

                EM_Startup.outForm = new EM_Output();
                EM_Startup.outForm.Show();
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_ContextMenuExtensions.cs: line: 50");
            }
        }

        private static void callback_OnClick1(Object o, EventArgs e)
        {
            try
            {
                if (EM_Startup.outForm != null && !EM_Startup.outForm.IsDisposed)
                {
                    EM_Startup.outForm.Close();
                }

                EM_Startup.outForm = new EM_Output();
                EM_Startup.outForm.Show();
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_ContextMenuExtensions.cs: line: 68");
            }
        }
    }
}
