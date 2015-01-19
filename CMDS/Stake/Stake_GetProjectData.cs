using Base_Tools45;
using System.Data.OleDb;

namespace Stake
{
    public static class Stake_GetProjectData
    {
        public static ProjectData
        getProjectData(string strJN)
        {
            string dataSource = @"E:\Spinn\ADMIN2014\timedata.accdb";
            ProjectData projData = new ProjectData();
            using (OleDbConnection dbCon = new OleDbConnection(string.Format("Provider=Microsoft.Ace.OLEDB.12.0; Data Source={0}", dataSource)))
            {
                string clientNumber = "";
                string empNumber = "";

                string strAccessSelect = "SELECT * FROM Jobs";
                OleDbCommand command = new OleDbCommand(strAccessSelect, dbCon);
                dbCon.Open();
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int colNum = reader.GetOrdinal("Job Number");
                    if (reader.GetValue(colNum).ToString() == strJN)
                    {
                        projData.Number = strJN;

                        colNum = reader.GetOrdinal("Job Name");
                        projData.Name = reader.GetValue(colNum).ToString();

                        colNum = reader.GetOrdinal("Project Location");
                        projData.Location = reader.GetValue(colNum).ToString();

                        colNum = reader.GetOrdinal("Client Number");
                        clientNumber = reader.GetValue(colNum).ToString();

                        colNum = reader.GetOrdinal("Project Coordinator");
                        empNumber = reader.GetValue(colNum).ToString();
                        break;
                    }
                }
                reader.Close();
                dbCon.Close();

                strAccessSelect = "SELECT * FROM Clients";
                command = new OleDbCommand(strAccessSelect, dbCon);
                dbCon.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int colNum = reader.GetOrdinal("ID");
                    if (reader.GetValue(colNum).ToString() == clientNumber)
                    {
                        colNum = reader.GetOrdinal("Company");
                        projData.Client = reader.GetValue(colNum).ToString();
                        break;
                    }
                }
                reader.Close();
                dbCon.Close();

                strAccessSelect = "SELECT * FROM Employee";
                command = new OleDbCommand(strAccessSelect, dbCon);
                dbCon.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int colNum = reader.GetOrdinal("EmployeeID");
                    if (reader.GetValue(colNum).ToString() == empNumber)
                    {
                        colNum = reader.GetOrdinal("Employee Name");
                        projData.Coordinator = reader.GetValue(colNum).ToString();
                        break;
                    }
                }
                reader.Close();
                dbCon.Close();
            }
            return projData;
        }
    }
}