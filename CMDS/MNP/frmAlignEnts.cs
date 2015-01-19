using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Label = System.Windows.Forms.Label;

namespace MNP
{
    public sealed partial class frmAlignEnts : Form
    {
        private static frmMNP fMNP = MNP_Forms.fMNP;

        private ObjectId idAlign = ObjectId.Null;
        public Handle AlignHandle;

        public frmAlignEnts()
        {
            InitializeComponent();
        }

        public void
        updateForm(List<AlgnEntData> algnEntData)
        {
            ControlCollection cntrls = (ControlCollection)this.Controls;
            Control cntrl;
            Label lbl;
            TextBox txBox;
            int i;
            for (i = cntrls.Count - 1; i > -1; i--)
            {
                cntrl = cntrls[i];
                if (cntrl.Name.Contains("lbl"))
                    cntrls.Remove(cntrl);
            }

            int l0 = 6;
            int l1 = 36;
            int l2 = 70;
            int l3 = 150;
            int l4 = 210;
            int l5 = 260;
            int l6 = 330;

            lbl = new Label();
            lbl.Name = "lblIndex";
            lbl.Text = "#";
            cntrl = lbl;
            cntrl.Top = 0;
            cntrl.Left = l0;
            cntrl.Height = 18;
            cntrl.Width = 18;
            cntrls.Add(cntrl);

            lbl = new Label();
            lbl.Name = "lblId";
            lbl.Text = "ID";
            cntrl = lbl;
            cntrl.Top = 0;
            cntrl.Left = l1;
            cntrl.Height = 18;
            cntrl.Width = 18;
            cntrls.Add(cntrl);

            lbl = new Label();
            lbl.Name = "lblEntity";
            lbl.Text = "Entity";
            cntrl = lbl;
            cntrl.Top = 0;
            cntrl.Left = l2;
            cntrl.Height = 18;
            cntrl.Width = 48;
            cntrls.Add(cntrl);

            lbl = new Label();
            lbl.Name = "lblStaBeg";
            lbl.Text = "Beg Sta";
            cntrl = lbl;
            cntrl.Top = 0;
            cntrl.Left = l3;
            cntrl.Height = 18;
            cntrl.Width = 48;
            cntrls.Add(cntrl);

            lbl = new Label();
            lbl.Name = "lblStaEnd";
            lbl.Text = "End Sta";
            cntrl = lbl;
            cntrl.Top = 0;
            cntrl.Left = l4;
            cntrl.Height = 18;
            cntrl.Width = 48;
            cntrls.Add(cntrl);

            lbl = new Label();
            lbl.Name = "lblLength";
            lbl.Text = "Length";
            cntrl = lbl;
            cntrl.Top = 0;
            cntrl.Left = l5;
            cntrl.Height = 18;
            cntrl.Width = 48;
            cntrls.Add(cntrl);

            lbl = new Label();
            lbl.Name = "lblRadius";
            lbl.Text = "Radius";
            cntrl = lbl;
            cntrl.Top = 0;
            cntrl.Left = l6;
            cntrl.Height = 18;
            cntrl.Width = 48;
            cntrls.Add(cntrl);

            int k = algnEntData.Count;
            this.cmdUpdate.Top = k * 30 + 80;
            this.cmdAddCurve.Top = this.cmdUpdate.Top;
            this.Height = this.cmdUpdate.Top + 120;

            i = 0;
            for (int j = 0; j < k; j++)
            {
                int c = 0;

                lbl = new Label();
                lbl.Name = string.Format("lbl{0}_{1}", i, c);
                lbl.Text = j.ToString();
                cntrl = lbl;
                cntrl.Top = 36 + i * 20;
                cntrl.Left = l0;
                cntrl.Height = 18;
                cntrl.Width = 48;
                cntrls.Add(cntrl);

                lbl = new Label();
                lbl.Name = string.Format("lbl{0}_{1}", i, c += 1);
                lbl.Text = algnEntData[j].ID.ToString();
                cntrl = lbl;
                cntrl.Top = 36 + i * 20;
                cntrl.Left = l1;
                cntrl.Height = 18;
                cntrl.Width = 48;
                cntrls.Add(cntrl);

                lbl = new Label();
                lbl.Name = string.Format("lbl{0}_{1}", i, c += 1);
                lbl.Text = algnEntData[j].Type;
                cntrl = lbl;
                cntrl.Top = 36 + i * 20;
                cntrl.Left = l2;
                cntrl.Height = 18;
                cntrl.Width = 48;
                cntrls.Add(cntrl);

                lbl = new Label();
                lbl.Name = string.Format("lbl{0}_{1}", i, c += 1);
                lbl.Text = algnEntData[j].StaBeg.ToString();
                cntrl = lbl;
                cntrl.Top = 36 + i * 20;
                cntrl.Left = l3;
                cntrl.Height = 18;
                cntrl.Width = 48;
                cntrls.Add(cntrl);

                lbl = new Label();
                lbl.Name = string.Format("lbl{0}_{1}", i, c += 1);
                lbl.Text = algnEntData[j].StaEnd.ToString();
                cntrl = lbl;
                cntrl.Top = 36 + i * 20;
                cntrl.Left = l4;
                cntrl.Height = 18;
                cntrl.Width = 48;
                cntrls.Add(cntrl);

                lbl = new Label();
                lbl.Name = string.Format("lbl{0}_{1}", i, c += 1);
                lbl.Text = algnEntData[j].Length.ToString();
                cntrl = lbl;
                cntrl.Top = 36 + i * 20;
                cntrl.Left = l5;
                cntrl.Height = 18;
                cntrl.Width = 48;
                cntrls.Add(cntrl);

                txBox = new TextBox();
                txBox.Name = string.Format("lbl{0}_{1}", i, c += 1);
                txBox.Text = algnEntData[j].Radius.ToString();
                cntrl = txBox;
                cntrl.Top = 36 + i * 20;
                cntrl.Left = l6;
                cntrl.Height = 18;
                cntrl.Width = 48;
                cntrls.Add(cntrl);

                i += 1;
            }
        }

        private void cmdAddCurve_Click(object sender, EventArgs e)
        {
            MNP_Align.insertCurve(idAlign);
            this.Hide();
        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            Control cntrl;
            ControlCollection cntrls = (ControlCollection)this.Controls;
            List<AlgnEntData> algnData = new List<AlgnEntData>();
            int k = cntrls.Count;
            k = (k - 9) / 1 - 1;

            Object[,] varData = new object[k, 6];
            for (int i = 0; i < cntrls.Count; i++)
            {
                cntrl = cntrls[i];
                if (cntrl.Name.Contains("lbl"))
                {
                    int id = int.Parse(cntrl.Name.Substring(4, 1));
                    int idx = id + 2;
                    varData[id, idx] = cntrl.Text;
                }
            }
            AlgnEntData aData;
            for (int i = 0; i < k; i++)
            {
                aData = new AlgnEntData();
                aData.ID = int.Parse(varData[i, 1].ToString());
                aData.Type = varData[i, 2].ToString();
                aData.StaBeg = double.Parse(varData[i, 3].ToString());
                aData.StaEnd = double.Parse(varData[i, 4].ToString());
                aData.Length = double.Parse(varData[i, 5].ToString());
                aData.Radius = double.Parse(varData[i, 6].ToString());
                algnData.Add(aData);
            }

            Alignment align = (Alignment)idAlign.getEnt();
            AlignmentEntityCollection ents = align.Entities;
            bool changed = false;

            for (int i = 0; i < ents.Count; i++)
            {
                AlignmentEntity ent = ents[i];
                if (ent.EntityType == AlignmentEntityType.Arc)
                {
                    if (algnData[1].Radius == 0)
                    {
                        ents.Remove(ent);
                        changed = true;
                    }
                }
                else
                {
                    AlignmentArc arc = (AlignmentArc)ent;
                    if (!arc.Radius.Equals(algnData[i].Radius))
                    {
                        arc.Radius = algnData[i].Radius;
                        changed = true;
                    }
                }
                if (changed)
                {
                    this.Close();
                    MNP_Align.editAlign(align.ObjectId);
                }
            }
        }

        private void frmAlignEnts_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}