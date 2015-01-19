using Base_Tools45;

//
//////////////////////////////////////////////////////////////////////////////
//
//  Copyright 2014 Autodesk, Inc.  All rights reserved.
//
//  Use of this software is subject to the terms of the Autodesk license
//  agreement provided at the time of installation or download, or which
//  otherwise accompanies this software in either electronic or hard copy form.
//
//////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;

namespace EventManager
{
    /// <summary>
    /// EM_Helper class including some static helper functions.
    /// </summary>
    public class EM_Helper
    {
        private static List<string> msgs;

        public EM_Helper()
        {
            msgs = new List<string>();
        }

        public static void StreamMessage(string str)
        {
            try
            {
                //InfoMessageBox(str);
                StreamToRichTextControl(str);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Helper.cs: line: 39");
            }
        }

        public static void Message(System.Exception ex0)
        {
            try
            {
                System.Windows.Forms.MessageBox.Show(
                    ex0.ToString(),
                    "Error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Helper.cs: line: 55");
            }
        }

        public static void InfoMessageBox(string str)
        {
            try
            {
                System.Windows.Forms.MessageBox.Show(
                    str,
                    "Events Watcher",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Information);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Helper.cs: line: 71");
            }
        }

        public static void StreamToRichTextControl(string str)
        {
            try
            {
                if (EM_Startup.outForm == null || EM_Startup.outForm.richTextBox1 == null)
                    return;

                str = string.Format("{0} {1} {2}\n", DateTime.Now, Environment.UserName, str);
                msgs.Insert(0, str);

                EM_Startup.outForm.richTextBox1.Clear();

                for (int i = 0; i < msgs.Count; i++)
                {
                    EM_Startup.outForm.richTextBox1.AppendText(msgs[i]);
                    if (i == 50)
                        break;
                }
                EM_Startup.outForm.richTextBox1.SelectionStart = 0;
                EM_Startup.outForm.richTextBox1.ScrollToCaret();
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Helper.cs: line: 98");
            }
        }
    }
}
