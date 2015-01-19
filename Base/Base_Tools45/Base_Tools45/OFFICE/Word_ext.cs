using System;
using System.Diagnostics;
using System.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;

namespace Base_Tools45.Office
{
    public class Word_ext
    {
        //EXAMPLES FROM SCOTT HANSELMAN
        static void Main(string[] args)
        {
            GenerateChart(copyToWord: true);
        }

        static void GenerateChart(bool copyToWord = false)
        {
            var excel = new Excel.Application();
            excel.Visible = true;
            excel.Workbooks.Add();

            excel.get_Range("A1").Value2 = "Process Name";
            excel.get_Range("B1").Value2 = "Memory Usage";

            var processes = Process.GetProcesses()
                                   .OrderByDescending(p => p.WorkingSet64)
                                   .Take(10);
            int i = 2;
            foreach (var p in processes)
            {
                excel.get_Range("A" + i).Value2 = p.ProcessName;
                excel.get_Range("B" + i).Value2 = p.WorkingSet64;
                i++;
            }

            Excel.Range range = excel.get_Range("A1");
            Excel.Chart chart = (Excel.Chart)excel.ActiveWorkbook.Charts.Add(
                After: excel.ActiveSheet);

            chart.ChartWizard(Source: range.CurrentRegion,
                Title: "Memory Usage in " + Environment.MachineName);

            chart.ChartStyle = 45;
            chart.CopyPicture(Excel.XlPictureAppearance.xlScreen,
                Excel.XlCopyPictureFormat.xlBitmap,
                Excel.XlPictureAppearance.xlScreen);

            if (copyToWord)
            {
                var word = new Word.Application();
                word.Visible = true;
                word.Documents.Add();

                word.Selection.Paste();
            }
        }
    }
}