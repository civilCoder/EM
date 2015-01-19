using Base_Tools45;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Label = System.Windows.Forms.Label;

namespace Stake
{
    public static class Stake_ProcessPntList
    {
        private static Forms.frmExport fExport = Forms.Stake_Forms.sForms.fExport;

        public static void
        ProcPntList(List<CgPnt> vPnts, string strOpt)
        {
            List<uint> pnts = null;
            var query = vPnts.GroupBy(pd => pd.nameLayer)
               .Select(group => new
               {
                   Layer = @group.Key,
                   pntGrp = @group.OrderBy(pn => pn.Num)
               });

            List<string> pntLayer = new List<string>();
            List<DataSet> dataSets = new List<DataSet>();

            foreach (var group in query)
            {
                DataSet dSet = new DataSet();
                foreach (var p in group.pntGrp)
                {
                    pnts.Add(p.Num);
                }

                dSet.COUNT = pnts.Count;
                dSet.Layer = group.Layer;
                dSet.Lower = (uint)pnts[0];
                dSet.Upper = (uint)pnts[pnts.Count - 1];
                dSet.Nums = pnts;
                dSet.Missing = (pnts.Count == (dSet.Upper - dSet.Lower)) ? false : true;
                dSet.Name = "";

                int pos = dSet.Layer.IndexOf("-");
                pos = dSet.Layer.IndexOf("-", pos + 1);
                dSet.ObjectName = dSet.Layer.Substring(pos + 1);

                dSet.Stations = null;
                dataSets.Add(dSet);
            }

            var sortGrp = dataSets.GroupBy(g => g.ObjectName)
                .Select(grp => new
                {
                    Name = @grp.Key,
                    pntGrp = @grp.OrderBy(pg => pg.Lower
                    )
                });

            List<DataSet> dataSum = new List<DataSet>();
            foreach (var ds in sortGrp)
            {
                DataSet dSum = new DataSet();
                dSum.Name = ds.Name;
                dSum.Nums = pnts;

                dataSum.Add(dSum);
            }

            fExport.dataSet = dataSets;
            fExport.dataSum = dataSum;

            if (strOpt != "STAKE")
            {
                addData(dataSets, strOpt);
            }

            return;
        }

        public static void
        addData(List<DataSet> varDataSet, string strOpt)
        {
            string strSource = "";

            switch (strOpt)
            {
                case "OnScreen":
                    fExport.cmdObject.Text = "OBJECT";
                    strSource = "SRT";

                    break;

                case "ByRange":
                    fExport.cmdObject.Text = "OBJECT";
                    strSource = "SRT";

                    break;

                case "ByAlign":
                    fExport.cmdObject.Text = "ALIGNMENT";
                    strSource = "SRT";
                    break;

                case "SORT":
                    switch (fExport.MODE)
                    {
                        case "OnScreen":
                        case "ByRange":
                            fExport.cmdObject.Text = "OBJECT";
                            break;

                        case "ByAlign":
                            fExport.cmdObject.Text = "ALIGNMENT";
                            break;
                    }
                    strSource = "SRT";
                    break;

                case "SUM":
                    switch (fExport.MODE)
                    {
                        case "OnScreen":
                        case "ByRange":
                            fExport.cmdObject.Text = "OBJECT";
                            break;

                        case "ByAlign":
                            fExport.cmdObject.Text = "ALIGNMENT";
                            break;
                    }
                    strSource = "SUM";
                    break;
            }

            Form.ControlCollection cntrls = (Form.ControlCollection)fExport.fraPntDesc.Controls;
            int iCount = cntrls.Count;
            //initial count of controls on form

            Control cntrl = null;
            CheckBox chkBox = new CheckBox();
            Label lbl = new Label();
            TextBox tBox = new TextBox();
            Button cmdBtn = new Button();

            for (int i = iCount - 1; i > -1; i--)
            {
                cntrl = cntrls[i];
                cntrls.Remove(cntrl);
            }

            for (int i = 0; i < varDataSet.Count; i++)
            {
                //1_________________________________________________________
                chkBox = new CheckBox();
                chkBox.Name = string.Format("chkBoxA{0}", i);
                if (strOpt == "SUM")
                {
                    chkBox.Text = "VARIOUS";
                }
                else
                {
                    chkBox.Text = varDataSet[i].Layer;
                }
                cntrl = chkBox;
                cntrl.Top = 5 + i * 20;
                cntrl.Height = 18;
                cntrl.Left = 5;

                cntrls.Add(cntrl);

                //2_________________________________________________________
                lbl = new Label();
                lbl.Name = string.Format("lblObject{0}", i);
                if (strOpt == "SUM")
                {
                    lbl.Text = varDataSet[i].Name;
                }
                else
                {
                    lbl.Text = varDataSet[i].ObjectName;
                }
                lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                cntrl = lbl;
                cntrl.Top = 10 + i * 20;
                cntrl.Height = 18;
                cntrl.Left = 132;

                cntrls.Add(lbl);

                //3_________________________________________________________
                lbl = new Label();
                lbl.Name = string.Format("lblNumPnts{0}", i);
                lbl.Text = varDataSet[i].COUNT.ToString();
                lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                cntrl = lbl;
                cntrl.Top = 10 + i * 20;
                cntrl.Height = 18;
                cntrl.Width = 48;
                cntrl.Left = 222;

                cntrls.Add(lbl);

                //4_________________________________________________________
                lbl = new Label();
                lbl.Name = string.Format("lblLowerA{0}", i);
                lbl.Text = varDataSet[i].Lower.ToString();
                lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                cntrl = lbl;
                cntrl.Top = 10 + i * 20;
                cntrl.Height = 18;
                cntrl.Width = 36;
                cntrl.Left = 264;

                cntrls.Add(lbl);

                //5_________________________________________________________
                lbl = new Label();
                lbl.Name = string.Format("lblToA{0}", i);
                lbl.Text = "TO";
                lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                cntrl = lbl;
                cntrl.Top = 10 + i * 20;
                cntrl.Height = 18;
                cntrl.Width = 18;
                cntrl.Left = 288;

                cntrls.Add(lbl);

                //6_________________________________________________________
                lbl = new Label();
                lbl.Name = string.Format("lblUpperA{0}", i);
                lbl.Text = varDataSet[i].Upper.ToString();
                lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                cntrl = lbl;
                cntrl.Top = 10 + i * 20;
                cntrl.Height = 18;
                cntrl.Width = 36;
                cntrl.Left = 306;

                cntrls.Add(lbl);

                //7_________________________________________________________
                cmdBtn = new Button();
                cmdBtn.Name = string.Format("cmdBox{0}", i);
                cmdBtn.Text = string.Format("LIST              {0} {1}", strSource, i);
                if (varDataSet[i].Missing)
                    cmdBtn.BackColor = System.Drawing.Color.Red;
                cntrl = cmdBtn;
                cntrl.Top = 6 + i * 20;
                cntrl.Height = 18;
                cntrl.Width = 24;
                cntrl.Left = 342;

                cntrls.Add(lbl);

                //8_________________________________________________________
                chkBox = new CheckBox();
                chkBox.Name = string.Format("chkBoxB{0}", i);
                cntrl = chkBox;
                cntrl.Top = 5 + i * 20;
                cntrl.Height = 18;
                cntrl.Left = 390;

                cntrls.Add(cntrl);

                //9_________________________________________________________
                tBox = new TextBox();
                tBox.Name = string.Format("tbxLowerB{0}", i);
                cntrl = tBox;
                cntrl.Top = 5 + i * 20;
                cntrl.Height = 36;
                cntrl.Left = 412;

                cntrls.Add(cntrl);

                //10_________________________________________________________
                lbl = new Label();
                lbl.Name = string.Format("lblToB{0}", i);
                lbl.Text = "TO";
                lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                cntrl = lbl;
                cntrl.Top = 10 + i * 20;
                cntrl.Height = 18;
                cntrl.Width = 18;
                cntrl.Left = 456;

                cntrls.Add(lbl);

                //11_________________________________________________________
                tBox = new TextBox();
                tBox.Name = string.Format("tbxUpperB{0}", i);
                cntrl = tBox;
                cntrl.Top = 5 + i * 20;
                cntrl.Height = 36;
                cntrl.Left = 480;

                cntrls.Add(cntrl);
            }
        }
    }
}