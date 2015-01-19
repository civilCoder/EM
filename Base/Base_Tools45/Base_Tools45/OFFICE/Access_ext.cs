using System.Data.OleDb;
using System.Windows.Forms;

namespace Base_Tools45.Office
{
    public static class Access_ext
    {
        public static int
        Main()
        {
            int done = 0;
            frmInput fInput = new frmInput();
            fInput.ShowDialog();
            done = 1;
            return done;
        }

        public static void
        readData(string dataSource)
        {
            string strAccessSelect = "SELECT * FROM Jobs";

            using (OleDbConnection dbCon = new OleDbConnection(string.Format("Provider=Microsoft.Ace.OLEDB.12.0; Data Source={0}", dataSource)))
            {
                OleDbCommand command = new OleDbCommand(strAccessSelect, dbCon);
                dbCon.Open();
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    MessageBox.Show(reader[0].ToString());
                }
                reader.Close();
            }
        }
    }
}