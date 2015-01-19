using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace Base_Tools45.Office
{
    public class Excel_ext
    {
        public Excel.Application excelAPP = null;
        public Excel.Workbook excelWB = null;
        public Excel.Worksheet excelWrkSht = null;

        public Excel.Sheets excelWrkShts
        {
            get
            {
                return excelWB.Worksheets;
            }
        }

        private static object _missing = true;
        private static object _false = false;
        private static object _true = true;

        private bool _app_visable = false;

        private object _filename;

        #region OPEN WORKBOOK VARIABLES

        private object _updateLInks = 0;
        private object _read_only = _false;
        private object _format = 1;
        private object _password = _missing;
        private object _write_res_password = _missing;
        private object _ignore_read_only_recommend = _true;
        private object _origin = _missing;
        private object _delimiter = _missing;
        private object _editable = _false;
        private object _notify = _false;
        private object _converter = _missing;
        private object _add_to_mru = _false;
        private object _local = _false;
        private object _corrupt_load = _false;

        #endregion OPEN WORKBOOK VARIABLES

        #region CLOSE WORKBOOK VARIABLES

        private object _save_changes = _false;
        private object _route_workbook = _false;

        #endregion CLOSE WORKBOOK VARIABLES

        public Excel_ext()
        {
            this.startExcel();
        }

        public Excel_ext(bool visable)
        {
            this._app_visable = visable;
            this.startExcel();
        }

        #region START EXCEL

        private void startExcel()
        {
            if (this.excelAPP == null)
            {
                this.excelAPP = new Excel.Application();
            }
            this.excelAPP.Visible = this._app_visable;
        }

        #endregion START EXCEL

        #region STOP EXCEL

        private void
        stopExcel()
        {
            if (this.excelAPP != null)
            {
                Process[] pProcess;
                pProcess = System.Diagnostics.Process.GetProcessesByName("Excel");
                pProcess[0].Kill();
            }
        }

        #endregion STOP EXCEL

        #region OPEN FILE FOR EXCEL

        public string
        OpenFile(string fileName, string password)
        {
            _filename = fileName;
            if (password != null)
                _password = password;
            if (File.Exists(fileName))
            {
                try
                {
                    this.excelWB = this.excelAPP.Workbooks.Open(fileName, _updateLInks, _read_only, _format, _password,
                        _ignore_read_only_recommend, _origin, _delimiter,
                        _editable, _notify, _converter, _add_to_mru,
                        _local, _corrupt_load);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(string.Format("{0} Excel_ext.cs: line: 99", ex.Message));
                    BaseObjs.writeDebug(ex.Message);
                }
            }
            else
            {
                this.excelWB = this.excelAPP.Workbooks.Add();
            }
            return "OK";
        }

        #endregion OPEN FILE FOR EXCEL

        public void
        CloseFile(bool saveChanges, string fileName, bool routeWorkbook)
        {
            excelWB.Close(saveChanges, fileName, routeWorkbook);
            stopExcel();
        }

        #region FIND EXCEL WORKSHEET

        public bool
        FindExcelWorksheet(string worksheetName)
        {
            bool SHEET_FOUND = false;
            if (this.excelWrkShts != null)
            {
                for (int i = 1; i < this.excelWrkShts.Count; i++)
                {
                    this.excelWrkSht = excelWrkShts.get_Item((object)i);
                    if (this.excelWrkSht.Name.Equals(worksheetName))
                    {
                        this.excelWrkSht.Activate();
                        SHEET_FOUND = true;
                        return SHEET_FOUND;
                    }
                }
            }
            return SHEET_FOUND;
        }

        #endregion FIND EXCEL WORKSHEET

        #region GET RANGE

        public string[]
        GetRange(string range)
        {
            Excel.Range workingRangeCells = excelWrkSht.get_Range(range, Type.Missing);
            System.Array array = workingRangeCells.Cells.Value2;
            string[] arrayS = this.ConvertToStringArray(array);
            return arrayS;
        }

        #endregion GET RANGE

        #region CONVERT TO STRING ARRAY

        private string[]
        ConvertToStringArray(System.Array values)
        {
            string[] newArray = new string[values.Length];
            int index = 0;
            for (int i = values.GetLowerBound(0); i < values.GetUpperBound(0); i++)
            {
                for (int j = values.GetLowerBound(1); j < values.GetUpperBound(1); j++)
                {
                    if (values.GetValue(i, j) == null)
                        newArray[index] = "";
                    else
                        newArray[index] = values.GetValue(i, j).ToString();
                    index++;
                }
            }
            return newArray;
        }

        #endregion CONVERT TO STRING ARRAY
    }
}