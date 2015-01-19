using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;

using Base_Tools45;

namespace fixGradeTagLinks
{
    /// <summary>
    /// Interaction logic for winResults.xaml
    /// </summary>
    public partial class winResults : Window
    {
        public winResults()
        {
            InitializeComponent();
        }

        public ObjectId id { get; set; }
        public SelectionSet ss { get; set; }
        public Button btn { get; set; }

        private Editor ed;
        private CogoPoint pnt = null;

        private int rem, itm, row, col, half, halfAdd, halfFix;
        private int btnH = 20, btnW = 50, buf = 2;
        private int index;

        private bool proceed;
        
        private Document acadDoc
        {
            get
            {
                return BaseObjs._acadDoc;
            }
        }

        public void addControls(winResults wResults, ListBox lbx, List<Handle> hLnk)
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
                    wResults.Height += (half - 10) * (btnH + buf);
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
                        btn.Content = h;
                        btn.Height = btnH;
                        btn.Width = btnW;
                        //btn.Location = new System.Drawing.Point(colX, rowY);

                        btn.MouseDown += btn_MouseDown;
                        lbx.Items.Add(btn);
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
            if (e.RightButton == MouseButtonState.Pressed)
            {
                for (int i = 0; i < lbxAdd.Items.Count; i++)
                {
                    Button btnX = (Button)lbxAdd.Items[i];
                    if (btn.Name == btnX.Name)
                    {
                        DragDrop.DoDragDrop(btnX, btnX, DragDropEffects.Move);
                        updateLbxAdd(lbxAdd);
                        break;
                    }
                }                
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
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
                Handle h = btn.Content.ToString().stringToHandle();
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


        private void btnProceed_Click(object sender, RoutedEventArgs e)
        {
            App.executeCLKS();
        }

        private void btnSAL_Click(object sender, RoutedEventArgs e)
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

        private void btnUSAL_Click(object sender, RoutedEventArgs e)
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

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            if (proceed)
            {
                BaseObjs._acadDoc.SendStringToExecute("upDn\r", true, false, true);
                ed.SelectionAdded -= ed_SelectionAdded;
            }

        }

        private void btnPass_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (removeBtn(lbxAdd))
                {
                    if (lbxAdd.Items.Count > 0)
                    {
                        btn = (Button)lbxAdd.Items[0];
                        gotoPoint(btn);
                    }
                    return;
                }

                if (removeBtn(lbxFix))
                {
                    if (lbxFix.Items.Count > 0)
                    {
                        btn = (Button)lbxFix.Items[0];
                        gotoPoint(btn);
                    }
                    return;
                }
            }
            catch
            {
            }
        }

        private void lbxFix_Drop(object sender, DragEventArgs e)
        {
            Debug.Print("DragDrop");
            if (e.Data.GetDataPresent(typeof(Button)))
            {
                Button btn = (Button)e.Data.GetData(typeof(Button));
                addBtnLbxFix(btn);
            }

        }

        private void lbxFix_DragEnter(object sender, DragEventArgs e)
        {
            Debug.Print("DragEnter");
            if (e.Data.GetDataPresent(typeof(Button)))
                e.Effects = DragDropEffects.Move;
            else
                e.Effects = DragDropEffects.None;

        }

        private bool removeBtn(ListBox lbx)
        {
            for (int i = 0; i < lbx.Items.Count; i++)
            {
                if (((Button)lbx.Items[i]).Name == btn.Name)
                {
                    lbx.Items.RemoveAt(i);
                    index = i;
                    updateLbxAdd(lbx);
                    switch (lbx.Name)
                    {
                        case "lbxAdd":
                            lblCountAdd.Text = lbx.Items.Count.ToString();
                            break;

                        case "lbxFix":
                            lblCountFix.Text = lbx.Items.Count.ToString();
                            break;
                    }
                    return true;
                }
            }
            return false;
        }

        private void ed_SelectionAdded(object sender, SelectionAddedEventArgs e)
        {
            proceed = false;
            MessageBox.Show("Selection Set has changed. Canceling command...");
            ed.SelectionAdded -= ed_SelectionAdded;
        }

        private void updateLbxAdd(ListBox lbx)
        {
            int n = lbx.Items.Count;
            if (n == 0 || index == n)
                return;

            for (int i = index; i < n; i++)
            {
                Button btn = (Button)lbx.Items[i];
                System.Windows.Point pnt = btn.PointToScreen(new System.Windows.Point(0, 0));
                if (n > halfAdd)
                {
                    if (i != halfAdd - 1)
                    {
                        System.Windows.Point pntx = new System.Windows.Point(pnt.X, pnt.Y - (btnH + buf));
                        //btn. = pntx;
                    }
                    else
                    {
                        System.Windows.Point pntx  = new System.Windows.Point(pnt.X - (btnW + buf), i * (btnH + buf));
                        //btn.Location = pnt;
                    }
                }
                else
                {
                    pnt = new System.Windows.Point(pnt.X, pnt.Y - (btnH + buf));
                    //btn.Location = pnt;
                }
            }
        }

        private void addBtnLbxFix(Button btn)
        {
            lbxFix.Items.Add(btn);
            int y = lbxFix.Items.Count;
            //if (y < 11)
                //btn.Location = new System.Drawing.Point(0, (y - 1) * 22);
            //else
                //btn.Location = new System.Drawing.Point(btnW + buf, (y - 1) * (btnH + buf));
        }

    }
}
