using System.Windows.Forms;

namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static class Dialog
    {
        public static string
        OpenFileDialog(string defExtension, string title, string filterString, string directory)
        {
            string fileName = string.Empty;
            System.Windows.Forms.OpenFileDialog dbox = new System.Windows.Forms.OpenFileDialog();
            dbox.AddExtension = true;
            dbox.DefaultExt = defExtension;
            dbox.Filter = filterString;
            dbox.Title = title;
            dbox.CheckFileExists = true;
            dbox.Multiselect = false;
            dbox.InitialDirectory = directory;
            if (dbox.ShowDialog() == DialogResult.OK)
            {
                fileName = dbox.FileName;
            }
            return fileName;
        }
    }
}
