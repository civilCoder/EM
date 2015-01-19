using Base_Tools45;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ProcessPointFile
{
    public sealed partial class PPF_PntDesc : Form
    {
        private static readonly PPF_PntDesc fPntDesc = new PPF_PntDesc();

        private PPF_PntDesc()
        {
            InitializeComponent();
        }

        public static PPF_PntDesc frmPntDesc
        {
            get
            {
                return fPntDesc;
            }
        }

        public static void
        loadForm()
        {
            List<string> pntDescs = PPF_APP.DESCLIST;
            pntDescs.Sort();

            int n = pntDescs.Count;
            const int rows = 30;
            int cols = 1;

            for (int i = 1; i < 10; i++)
            {
                if (n <= i * rows)
                {
                    cols = i;
                    break;
                }
            }

            GroupBox gBox = fPntDesc.groupBox1;
            gBox.Controls.Clear();

            int k = -1;
            try
            {
                for (int j = 0; j < cols; j++)
                {
                    for (int i = 0; i < rows; i++)
                    {
                        ++k;
                        if (k < n)
                        {
                            CheckBox chkBox = new CheckBox();
                            int i1 = i;
                            int j1 = j;
                            int k1 = k;
                            chkBox.With(cb =>
                            {
                                cb.Name = string.Format("chkBox{0}", k1.ToString("00"));
                                cb.Text = pntDescs[k1];
                                cb.Top = i1 * 20 + 5;
                                cb.Left = j1 * 220 + 5;
                                cb.Width = 200;
                            });
                            if (PPF_PostProcess.testDesc(pntDescs[k]))
                                chkBox.CheckState = CheckState.Checked;
                            gBox.Controls.Add(chkBox);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " PPF_PntDesc.cs: line: 70");
            }

            gBox.Height = rows * 20 + 30;
            if (cols > 3)
            {
                gBox.Width = cols * 220 + 12;
                fPntDesc.Width = gBox.Width + 48;
                fPntDesc.cmdOK.Left = gBox.Left + gBox.Width - 112;
                fPntDesc.cmdCancel.Left = gBox.Left + gBox.Width - 234;
                fPntDesc.cmdPrint.Left = gBox.Left;
            }

            fPntDesc.Height = rows * 20 + 130;
            fPntDesc.cmdOK.Top = rows * 20 + 50;
            fPntDesc.cmdCancel.Top = rows * 20 + 50;
            fPntDesc.cmdPrint.Top = rows * 20 + 50;
        }

        private void cmdPrint_Click(object sender, EventArgs e)
        {
            Control cntrl;
            int length = PPF_APP.TXTFILE.Length;
            string nameFileOut = string.Format("{0}_des.txt", PPF_APP.TXTFILE.Substring(0, length - 4));
            StreamWriter sw = new StreamWriter(nameFileOut, false);
            for (int i = 0; i < this.groupBox1.Controls.Count; i++)
            {
                cntrl = groupBox1.Controls[i];
                sw.WriteLine(cntrl.Text);
            }
            sw.Close();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            List<string> dList = (from object obj in groupBox1.Controls
                                  where obj.GetType() == typeof(CheckBox)
                                  select (CheckBox)obj
                                      into cbBox
                                      where cbBox.Checked
                                      select cbBox.Text).ToList();

            int length = PPF_APP.TXTFILE.Length;
            string nameFileOut = string.Format("{0}_mod.txt", PPF_APP.TXTFILE.Substring(0, length - 4));
            StreamWriter sw = new StreamWriter(nameFileOut, false);

            List<PPF_PntData> pds = PPF_APP.PNTDATA;

            foreach (PPF_PntData pd in pds)
            {
                if (dList.Contains(pd.Desc))
                {
                    string buf = string.Format("{0},{1},{2},{3},{4},{5} {6} {7}", pd.PntNum, pd.Y, pd.X, pd.Z, pd.Desc, pd.Code1, pd.Code2, pd.Code3);
                    buf = buf.TrimEnd();
                    sw.WriteLine(buf);
                }
            }

            sw.Close();

            PPF_APP.getDrawing(PPF_APP.DWGPATH);
        }

        private void PPF_PntDesc_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}