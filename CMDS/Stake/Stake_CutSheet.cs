using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word.Application;

namespace Stake
{
    public static class Stake_CutSheet
    {
        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;
        private static Forms.frmExport fExport = Forms.Stake_Forms.sForms.fExport;
        private static Forms.frmPoints fPoints = Forms.Stake_Forms.sForms.fPoints;

        public static void
        processCutsheetData(string strSource)
        {
            List<CmdControls> cCntls = fExport.cntls;
            List<DataSet> varDataSet = fExport.dataSet;
            List<DataSet> varDataSum = fExport.dataSum;

            System.Windows.Forms.Button cmdBtn = default(System.Windows.Forms.Button);

            List<ObjectId> objCutSheetPnts = new List<ObjectId>();
            List<uint> lngPntNums = new List<uint>();

            uint lngPntNum = 0;

            CogoPointCollection objPnts = Autodesk.Civil.ApplicationServices.CivilApplication.ActiveDocument.CogoPoints;
            ObjectId objPnt = ObjectId.Null;

            List<string> strLayers = new List<string>();
            switch (strSource)
            {
                case "EXPORTPOINTS":

                    for (int i = 0; i < cCntls.Count; i++)
                    {
                        if (cCntls[i].chkBox1.Checked)
                        {
                            strLayers.Add(cCntls[i].chkBox1.Text);

                            cmdBtn = cCntls[i].cmdBtn;

                            switch (cmdBtn.Text.Substring(11, 3))
                            {
                                case "SRT":
                                    varDataSet = fExport.dataSet;
                                    lngPntNums = varDataSet[i].Nums;
                                    break;

                                case "SUM":
                                    varDataSum = fExport.dataSum;
                                    lngPntNums = varDataSum[i].Nums;
                                    break;
                            }

                            //do partial
                            if (cCntls[i].chkBox2.Checked)
                            {
                                for (int j = int.Parse(cCntls[i].tBxLowerB.Text); j <= int.Parse(cCntls[i].tBxUpperB.Text); j++)
                                {
                                    for (int k = 0; k < lngPntNums.Count; k++)
                                    {
                                        lngPntNum = lngPntNums[k];
                                        if ((uint)j == lngPntNum)
                                        {
                                            try
                                            {
                                                objPnt = objPnts.GetPointByPointNumber(lngPntNum);
                                                objCutSheetPnts.Add(objPnt);
                                            }
                                            catch (System.Exception)
                                            {
                                            }

                                            break;
                                        }
                                    }
                                }

                                //do full list
                            }
                            else
                            {
                                for (int k = 0; k < lngPntNums.Count; k++)
                                {
                                    lngPntNum = lngPntNums[k];
                                    try
                                    {
                                        objPnt = objPnts.GetPointByPointNumber(lngPntNum);
                                        objCutSheetPnts.Add(objPnt);
                                    }
                                    catch (System.Exception )
                                    {
                                    }
                                }
                            }
                        }
                    }

                    break;

                case "POINTLIST":

                    for (int j = 0; j < fPoints.lbxPoints.CheckedItems.Count; j++)
                    {
                        lngPntNum = (uint)fPoints.lbxPoints.Items[j];
                        try
                        {
                            objPnt = objPnts.GetPointByPointNumber(lngPntNum);
                            objCutSheetPnts.Add(objPnt);
                        }
                        catch (System.Exception )
                        {
                        }
                    }
                    fPoints.Hide();
                    break;
            }

            Microsoft.Office.Interop.Word.Document objCutSheet = setupCutSheet(strLayers);
            Microsoft.Office.Interop.Word.Table objCutSheetTable = objCutSheet.Tables[2];

            if (addCutSheetData(objCutSheetTable, objCutSheetPnts))
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Cutsheet created at: " + objCutSheet.Path + "\\" + objCutSheet.Name);
            }
            else
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Cutsheet operation failed - check if points were selected.....");
            }
        }

        public static Microsoft.Office.Interop.Word.Document
        setupCutSheet(List<string> strLayers)
        {
            string strProjNum = BaseObjs.jobNumber();

            string strDate = String.Format("{0}", DateTime.Today);

            string strFileName = string.Format("{0}_CutSheet{1}_{2}", strProjNum, strLayers, strDate);

            Microsoft.Office.Interop.Word.Document objCutSheet = makeCutSheet(strFileName);
            Microsoft.Office.Interop.Word.Table objWordTable1 = objCutSheet.Tables[1];

            Microsoft.Office.Interop.Word.Cell objCell = objWordTable1.Cell(6, 4);
            objCell.Range.InsertBefore(Convert.ToString(DateTime.Today));

            objCell = objWordTable1.Cell(7, 4);

            objCell.Range.InsertBefore(strProjNum);

            ProjectData varProjectData = Stake_GetProjectData.getProjectData(strProjNum);

            objCell = objWordTable1.Cell(8, 2);
            objCell.Range.InsertBefore(varProjectData.Name);

            objCell = objWordTable1.Cell(9, 2);
            objCell.Range.InsertBefore(varProjectData.Location);

            objCell = objWordTable1.Cell(10, 2);
            objCell.Range.InsertBefore(varProjectData.Coordinator);

            string strUserName = "";

            string strUser = Environment.UserName;
            switch (strUser)
            {
                case "john":
                    strUserName = "John Herman";
                    break;

                case "luis":
                    strUserName = "Luis Arballo";
                    break;

                case "brianp":
                    strUserName = "Brian Peterson";
                    break;

                case "michael":
                    strUserName = "Michael Roberts";
                    break;

                case "briant":
                    strUserName = "Brian Thienes";
                    break;

                default:
                    strUserName = strUser;
                    break;
            }

            objCell = objWordTable1.Cell(9, 4);
            objCell.Range.InsertBefore(strUserName);

            return objCutSheet;
        }

        public static Microsoft.Office.Interop.Word.Document
        makeCutSheet(string strDocName)
        {
            string strPath = Path.GetDirectoryName(BaseObjs.docFullName);
            string strPathX = string.Format("{0}\\Survey\\Cutsheets\\Staking\\", strPath);
            string strFileName = strPathX + strDocName;

            if (!File.Exists(strPathX))
            {
                try
                {
                    Directory.CreateDirectory(strPath);
                }
                catch (System.Exception )
                {
                    if (!Directory.Exists(strPath + "\\Survey"))
                    {
                        Directory.CreateDirectory(strPath + "\\Survey");
                    }
                    if (!Directory.Exists(strPath + "\\Survey\\Cutsheets"))
                    {
                        Directory.CreateDirectory(strPath + "\\Survey\\Cutsheets");
                    }
                    Directory.CreateDirectory(strPath + "\\Survey\\Cutsheets\\Staking");
                }
            }

            if (File.Exists(strFileName + ".docx") == true)
            {
                DialogResult response = System.Windows.Forms.MessageBox.Show(string.Format("File {0}.docx exists! Make Backup??", strFileName), "FILE EXISTS", MessageBoxButtons.YesNo);

                switch (response)
                {
                    case DialogResult.Yes:

                        File.Copy(strFileName + ".docx", strFileName + "_bak" + ".docx", true);

                        break;

                    case DialogResult.No:

                        File.Delete(strFileName + ".docx");

                        break;
                }
            }
            Microsoft.Office.Interop.Word.Application objWordApp = new Microsoft.Office.Interop.Word.Application();

            objWordApp.Visible = true;
            Microsoft.Office.Interop.Word.Document objWordDoc = objWordApp.Documents.Add("R:\\TSet\\Template\\SURVEY\\Staking\\CutSheet.dotx");
            objWordDoc.Activate();
            objWordDoc.SaveAs(strFileName);

            return objWordDoc;
        }

        public static bool
        addCutSheetData(Microsoft.Office.Interop.Word.Table objCutSheetTable, List<ObjectId> objPnts)
        {
            bool functionReturnValue = false;

            if (objPnts.Count == 0)
            {
                return false;
            }
            int k = 0;

            for (int i = 0; i < objPnts.Count; i++)
            {
                ObjectId objPnt = objPnts[i];

                ResultBuffer rb = objPnt.getXData("STAKE");
                if (rb == null)
                    return false;
                TypedValue[] tvs = rb.AsArray();

                string strStation = String.Format("##+##.00", tvs[2].Value.ToString());
                string strPntDesc = objPnt.getCogoPntDesc();

                int intPos = 0;
                string strOffset = "";

                if (strPntDesc.Contains("O/S"))
                {
                    int pos = strPntDesc.IndexOf("O/S");
                    strOffset = strPntDesc.Substring(0, intPos - 1);
                    strPntDesc = strPntDesc.Substring(intPos + 4);
                }

                Microsoft.Office.Interop.Word.Rows objRows = objCutSheetTable.Rows;
                Microsoft.Office.Interop.Word.Row objRowNew;

                objRowNew = objCutSheetTable.Rows.Add();
                objRowNew = objCutSheetTable.Rows[objRows.Count - 1];

                objRowNew.Range.Font.Bold = 0;

                Microsoft.Office.Interop.Word.Cell objCell = objRowNew.Cells[8];
                objCell.WordWrap = true;

                objCell = objRowNew.Cells[1];
                objCell.Range.InsertBefore(strStation);

                objCell = objRowNew.Cells[2];
                objCell.Range.InsertBefore(Convert.ToString(objPnt.getCogoPntNumber().ToString()));
                objCell = objRowNew.Cells[3];
                objCell.Range.InsertBefore(String.Format("{0:F2}", objPnt.getCogoPntElevation()));
                objCell = objRowNew.Cells[7];

                if (intPos == 0)
                {
                    strOffset = "-";
                }

                objCell.Range.InsertBefore(strOffset);

                objCell = objRowNew.Cells[8];
                string strDesc = "";

                Microsoft.Office.Interop.Word.Selection objSelection;
                if (strPntDesc.Contains("\\U+0394"))
                {
                    int pos = strPntDesc.IndexOf("\\U+0394");
                    //Delta /\

                    objCell.Range.Select();
                    objSelection = objCutSheetTable.Application.Selection;

                    objSelection.StartOf(Microsoft.Office.Interop.Word.WdUnits.wdCell);
                    //****************!!!!!!!!!!!!!!!!!!!****************
                    objSelection.Delete();

                    objSelection.InsertSymbol(Font: "Arial", CharacterNumber: 916, Unicode: true);

                    strDesc = strPntDesc.Substring(0, intPos - 1);
                    //preceeding part of string
                    objCell.Range.InsertBefore(Convert.ToString(strDesc));
                    strDesc = strPntDesc.Substring(intPos + 7);
                    //following part of string
                    objCell.Range.InsertAfter(Convert.ToString(strDesc));
                }
                else if (strPntDesc.Contains("%%C"))
                {
                    intPos = strPntDesc.IndexOf("%%C");
                    //Diameter

                    objCell.Range.Select();
                    objSelection = objCutSheetTable.Application.Selection;

                    objSelection.StartOf(Microsoft.Office.Interop.Word.WdUnits.wdCell);
                    //****************!!!!!!!!!!!!!!!!!!!****************
                    objSelection.Delete();
                    objSelection.StartOf(Microsoft.Office.Interop.Word.WdUnits.wdCell);
                    objSelection.InsertSymbol(Font: "Arial", CharacterNumber: 934, Unicode: true);

                    strDesc = strPntDesc.Substring(0, intPos - 1);
                    objCell.Range.InsertBefore(Convert.ToString(strDesc));
                    strDesc = strPntDesc.Substring(intPos + 3);
                    objCell.Range.InsertAfter(Convert.ToString(strDesc));
                }
                else
                {
                    objCell.Range.InsertBefore(Convert.ToString(strPntDesc));
                }

                Point3d pnt3d = objPnt.getCogoPntCoordinates();
                objCell = objRowNew.Cells[9];
                objCell.Range.InsertBefore(String.Format("{0:F2}", pnt3d.Y));
                objCell = objRowNew.Cells[10];
                objCell.Range.InsertBefore(String.Format("{0}", pnt3d.X));

                k += 1;
            }

            Misc.logUsage("STAKE_EXPORT", k);

            functionReturnValue = true;
            return functionReturnValue;
        }

        public static void
        updateCutSheet()
        {
            string strPath = Path.GetDirectoryName(BaseObjs.docFullName);
            string strPathX = string.Format("{0}\\Survey\\Cutsheets\\Staking\\", strPath);

            string nameFile = FileManager.getFilesWithEditor("SELECT FIELD DATA FILE", "GETFILE", "Text (*.txt)|*.txt", strPathX, "Select file for update: ");
            List<FieldData> varFD = new List<FieldData>();
            using (StreamReader sr = File.OpenText(nameFile))
            {
                while (!sr.EndOfStream)
                {
                    string strBuf = sr.ReadLine();

                    if (strBuf.Substring(1, 1) != "#")
                    {
                        FieldData fData = parseTextLine(strBuf);
                        varFD.Add(fData);
                    }
                }
            }

            addResultsToCutSheet(varFD, strPathX);
        }

        public static void
        addResultsToCutSheet(List<FieldData> fd, string strPath)
        {
            string strTxtTarget = FileManager.getFilesWithEditor("SELECT CUTSHEET", "UPDATE CUTSHEET", "Word Document (*.docx)|*.docx", strPath, "Select CutSheet to update: ");

            Microsoft.Office.Interop.Word.Application objWordApp = new Word();
            Microsoft.Office.Interop.Word.Document objWordDoc = objWordApp.Documents.Open(strTxtTarget);

            objWordApp.Visible = true;

            Microsoft.Office.Interop.Word.Table objWordTable = objWordDoc.Tables[2];

            for (int i = 0; i < fd.Count; i++)
            {
                for (int j = 2; j <= objWordTable.Rows.Count; j++)
                {
                    Microsoft.Office.Interop.Word.Cell objWordCell = objWordTable.Cell(j, 2);
                    string strCellText = objWordCell.Range.Text;

                    string strPntNum = "";
                    int k = 1;
                    do
                    {
                        byte[] asciiBytes = Encoding.ASCII.GetBytes(strCellText.Substring(k, 1));
                        if (asciiBytes[0] == 13)
                            break;
                        strPntNum = strPntNum + strCellText.Substring(k, 1);
                        k = k + 1;
                    } while (true);

                    string strGrade = "";
                    if (fd[i].PntNum == strPntNum)
                    {
                        objWordCell = objWordTable.Cell(j, 3);
                        strCellText = objWordCell.Range.Text;

                        k = 1;
                        do
                        {
                            byte[] asciiBytes = Encoding.ASCII.GetBytes(strCellText.Substring(k, 1));
                            if (asciiBytes[0] == 13)
                                break;
                            strGrade = strGrade + strCellText.Substring(k, 1);
                            k = k + 1;
                        } while (true);

                        double dblGrade = double.Parse(strGrade);
                        double dblStake = System.Math.Round(fd[i].ElevStake, 4);

                        objWordCell = objWordTable.Cell(j, 4);
                        objWordCell.Range.InsertBefore(Convert.ToString(dblStake));

                        double dblDiff = System.Math.Round(dblGrade - dblStake, 2);

                        if (dblDiff < 0)
                        {
                            objWordCell = objWordTable.Cell(j, 5);
                            objWordCell.Range.InsertBefore(Convert.ToString(dblDiff * -1));
                        }
                        else
                        {
                            objWordCell = objWordTable.Cell(j, 6);
                            objWordCell.Range.InsertBefore(Convert.ToString(dblDiff));
                        }
                        break;
                    }
                }
            }
        }

        public static FieldData
        parseTextLine(string strBuf)
        {
            FieldData fldData = new FieldData();
            string[] strData = strBuf.splitFields(',');

            fldData.PntNum = strData[0];
            fldData.ElevDesign = double.Parse(strData[1]);
            fldData.ElevStake = double.Parse(strData[4]);

            return fldData;
        }
    }
}