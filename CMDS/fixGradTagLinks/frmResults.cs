using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace fixGradeTagLinks
{
    public partial class frmResults : Form
    {
        private Editor ed;

        private int rem, itm, row, col, half, halfAdd, halfFix;
        private int btnH = 20, btnW = 50, buf = 2;

        private CogoPoint pnt = null;

        private ObjectId idX;
        private SelectionSet _ss;
        private bool proceed;

        private Button _btn;
        private int index;

        public frmResults()
        {
            InitializeComponent();
            lbxFix.Height = 220;
            lbxAdd.Height = 220;
            this.Height = 565;
            this.Width = 282;
            lbxFix.DragDrop += lbxFix_DragDrop;
            lbxFix.DragEnter += lbxFix_DragEnter;
        }

        public ObjectId id
        {
            get
            {
                return idX;
            }
            set
            {
                idX = value;
            }
        }

        public SelectionSet ss
        {
            get
            {
                return _ss;
            }
            set
            {
                _ss = value;
            }
        }

        public Button btn
        {
            get
            {
                return _btn;
            }
            set
            {
                _btn = value;
            }
        }

        private Document acadDoc
        {
            get
            {
                return Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            }
        }

        public void addControls(frmResults fResults, ListBox lbx, List<Handle> hLnk)
        {
            int n = hLnk.Count;
            if (n > 0)
            {
                System.Math.DivRem(n, 2, out rem);
                half = n / 2;
                if (rem == 1)
                    half++;

                if (half > 10)
                {
                    lbx.Height += (1 + half - 10) * (btnH + buf);
                    fResults.Height += (half - 10) * (btnH + buf);
                }
                else
                {
                    half = 10;
                }

                if (lbx.Name == "lbxAdd")
                    halfAdd = half;
                if (lbx.Name == "lbxFix")
                    halfFix = half;

                col = -1;
                int colX = 0;
                int rowY = 0;
                for (itm = 0; itm < n; itm += half)
                {
                    col++;
                    colX = buf + itm / half * btnW;
                    for (row = 0; row < half; row++)
                    {
                        if ((itm + row) == n)
                            break;
                        rowY = row * (btnH + buf);

                        string h = hLnk[col * half + row].ToString();
                        //Debug.Print(h.ToString());

                        Button btn = new Button();
                        btn.Name = h;
                        btn.Text = h;
                        btn.Size = new System.Drawing.Size(btnW, btnH);
                        btn.Location = new System.Drawing.Point(colX, rowY);

                        btn.MouseDown += btn_MouseDown;
                        lbx.Controls.Add(btn);
                    }
                }

                switch (lbx.Name)
                {
                    case "lbxAdd":
                        lblCountAdd.Text = n.ToString();
                        break;

                    case "lbxFix":
                        lblCountFix.Text = n.ToString();
                        break;
                }
            }
            return;
        }

        private void btn_MouseDown(object sender, MouseEventArgs e)
        {
            btn = (Button)sender;
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < lbxAdd.Controls.Count; i++)
                {
                    if (btn.Name == lbxAdd.Controls[i].Name)
                    {
                        index = i;
                        break;
                    }
                }
                DragDropEffects effects = btn.DoDragDrop(btn, DragDropEffects.Move);
                if (effects == DragDropEffects.Move)
                    updateLbxAdd(lbxAdd);
            }
            else if (e.Button == MouseButtons.Left)
            {
                ed = BaseObjs._editor;
                ed.SelectionAdded -= ed_SelectionAdded;

                try
                {
                    gotoPoint(btn);
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    MessageBox.Show(string.Format("{0} method", ex.Message));
                }
            }
        }

        private void gotoPoint(Button btn)
        {
            Entity entX = null;
            using (BaseObjs._acadDoc.LockDocument())
            {
                Handle h = btn.Text.stringToHandle();
                entX = h.getEnt();

                pnt = (CogoPoint)entX;
                Point3d pnt3dIns = pnt.Location;
                ViewTableRecord vtr = new ViewTableRecord();
                vtr.CenterPoint = new Point2d(pnt3dIns.X, pnt3dIns.Y);
                vtr.Height = 30;
                vtr.Width = 50;
                BaseObjs._editor.SetCurrentView(vtr);

                pnt.Highlight();
                BaseObjs.acadActivate();

                ObjectId[] idsPntArr = new ObjectId[] { pnt.ObjectId };

                List<ObjectId> idPnt = Base_Tools45.Select.getEntityatPoint(pnt3dIns, typeof(CogoPoint), "*");
                ed.SelectionAdded += ed_SelectionAdded;
                proceed = true;
            }
        }

        private void lbxFix_DragEnter(object sender, DragEventArgs e)
        {
            Debug.Print("DragEnter");
            if (e.Data.GetDataPresent(typeof(Button)))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void lbxFix_DragDrop(object sender, DragEventArgs e)
        {
            Debug.Print("DragDrop");
            if (e.Data.GetDataPresent(typeof(Button)))
            {
                Button btn = (Button)e.Data.GetData(typeof(Button));
                addBtnLbxFix(btn);
            }
        }

        private void btnProceed_Click(object sender, EventArgs e)
        {
            App.executeCLKS();
        }

        private void ed_SelectionAdded(object sender, SelectionAddedEventArgs e)
        {
            proceed = false;
            MessageBox.Show("Selection Set has changed. Canceling command...");
            ed.SelectionAdded -= ed_SelectionAdded;
        }

        private void btnSAL_Click(object sender, EventArgs e)
        {
            try
            {
                BaseObjs.runMacro("Module3.Show_All_Links");
            }
            catch
            {
                BaseObjs.loadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                BaseObjs.runMacro("Module3.Show_All_Links");
            }
        }

        private void btnUSAL_Click(object sender, EventArgs e)
        {
            try
            {
                BaseObjs.runMacro("Module3.Undo_Show_All_Links");
            }
            catch
            {
                BaseObjs.loadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                BaseObjs.runMacro("Module3.Undo_Show_All_Links");
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            if (proceed)
            {
                BaseObjs._acadDoc.SendStringToExecute("upDn\r", true, false, true);
                ed.SelectionAdded -= ed_SelectionAdded;
            }
        }

        private void btnPass_Click(object sender, EventArgs e)
        {
            try
            {
                if (removeBtn(lbxAdd))
                {
                    if (lbxAdd.Controls.Count > 0)
                    {
                        btn = (Button)lbxAdd.Controls[0];
                        gotoPoint(btn);
                    }
                    return;
                }

                if (removeBtn(lbxFix))
                {
                    if (lbxFix.Controls.Count > 0)
                    {
                        btn = (Button)lbxFix.Controls[0];
                        gotoPoint(btn);
                    }
                    return;
                }
            }
            catch
            {
            }
        }

        private bool removeBtn(ListBox lbx)
        {
            for (int i = 0; i < lbx.Controls.Count; i++)
            {
                if (lbx.Controls[i].Name == btn.Name)
                {
                    lbx.Controls.RemoveAt(i);
                    index = i;
                    lbx.Update();
                    updateLbxAdd(lbx);
                    switch (lbx.Name)
                    {
                        case "lbxAdd":
                            lblCountAdd.Text = lbx.Controls.Count.ToString();
                            break;

                        case "lbxFix":
                            lblCountFix.Text = lbx.Controls.Count.ToString();
                            break;
                    }
                    return true;
                }
            }
            return false;
        }

        private void updateLbxAdd(ListBox lbx)
        {
            int n = lbx.Controls.Count;
            if (n == 0 || index == n)
                return;

            for (int i = index; i < n; i++)
            {
                Button btn = (Button)lbx.Controls[i];
                System.Drawing.Point pnt = btn.Location;
                if (n > halfAdd)
                {
                    if (i != halfAdd - 1)
                    {
                        pnt = new System.Drawing.Point(pnt.X, pnt.Y - (btnH + buf));
                        btn.Location = pnt;
                    }
                    else
                    {
                        pnt = new System.Drawing.Point(pnt.X - (btnW + buf), i * (btnH + buf));
                        btn.Location = pnt;
                    }
                }
                else
                {
                    pnt = new System.Drawing.Point(pnt.X, pnt.Y - (btnH + buf));
                    btn.Location = pnt;
                }
            }
        }

        private void addBtnLbxFix(Button btn)
        {
            lbxFix.Controls.Add(btn);
            int y = lbxFix.Controls.Count;
            if (y < 11)
                btn.Location = new System.Drawing.Point(0, (y - 1) * 22);
            else
                btn.Location = new System.Drawing.Point(btnW + buf, (y - 1) * (btnH + buf));
        }

        private void frmResults_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}