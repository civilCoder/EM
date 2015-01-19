using System.Collections.Generic;

namespace Base_Tools45
{
    /// <summary>
    /// Helper class including some static helper functions.
    /// </summary>
    public class Helper
    {
        private static List<string> msgs;

        public Helper()
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
                BaseObjs.writeDebug(ex.Message + " Helper.cs: line: 26");
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
                BaseObjs.writeDebug(ex.Message + " Helper.cs: line: 42");
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
                BaseObjs.writeDebug(ex.Message + " Helper.cs: line: 58");
            }
        }

        public static void StreamToRichTextControl(string str)
        {
            try
            {
                if (BaseObjs.outForm == null || BaseObjs.outForm.richTextBox1 == null)
                    return;

                msgs.Insert(0, str);

                BaseObjs.outForm.richTextBox1.Clear();

                for (int i = 0; i < msgs.Count; i++)
                {
                    BaseObjs.outForm.richTextBox1.AppendText(msgs[i]);
                    if (i == 50)
                        break;
                }
                BaseObjs.outForm.richTextBox1.SelectionStart = 0;
                BaseObjs.outForm.richTextBox1.ScrollToCaret();
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Helper.cs: line: 84");
            }
        }
    }
}
