using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static class Events
    {
        #region declares

        ///// <summary>
        /////
        ///// </summary>
        //public static CogoPoint cogoPnt = null;
        /// <summary>
        ///
        /// </summary>
        public static MText mTxt = null;

        /// <summary>
        ///
        /// </summary>
        public static Leader ldr = null;

        /// <summary>
        ///
        /// </summary>
        public static Polyline poly = null;

        /// <summary>
        ///
        /// </summary>
        public static Polyline3d poly3d = null;

        /// <summary>
        ///
        /// </summary>
        public static List<ObjectId> cogoPnts = new List<ObjectId>();

        /// <summary>
        ///
        /// </summary>
        public static List<ObjectId> mTxts = new List<ObjectId>();

        /// <summary>
        ///
        /// </summary>
        public static List<ObjectId> ldrs = new List<ObjectId>();

        /// <summary>
        ///
        /// </summary>
        public static List<ObjectId> polys = new List<ObjectId>();

        /// <summary>
        ///
        /// </summary>
        public static List<ObjectId> poly3ds = new List<ObjectId>();

        #endregion declares

        #region "poly3d"

        /// <summary>
        ///
        /// </summary>
        /// <param name="nameLayer"></param>
        /// <param name="poly3d"></param>
        public static void activatePoly3ds(string nameLayer, Polyline3d poly3d = null)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    if (poly3d == null)
                    {
                        Type type = typeof(Polyline3d);
                        TypedValue[] TVs = new TypedValue[2];
                        TVs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(type).DxfName), 0);
                        TVs.SetValue(new TypedValue((int)DxfCode.LayerName, nameLayer), 1);

                        SelectionSet ss = Base_Tools45.Select.buildSSet(TVs);
                        foreach (ObjectId id in ss.GetObjectIds())
                        {
                            Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                            if (ent is Polyline3d)
                            {
                                poly3d = (Polyline3d)ent;
                                poly3d.Modified += new EventHandler(poly3d_Modified);
                                poly3d.Erased += new ObjectErasedEventHandler(poly3d_Erased);
                                poly3ds.Add(poly3d.ObjectId);
                                poly3ds.TrimExcess();
                            }
                        }
                    }
                    else
                    {
                        poly3d = (Polyline3d)poly3d.ObjectId.GetObject(OpenMode.ForWrite);
                        poly3d.Modified += new EventHandler(poly3d_Modified);
                        poly3d.Erased += new ObjectErasedEventHandler(poly3d_Erased);
                        poly3ds.Add(poly3d.ObjectId);
                        poly3ds.TrimExcess();
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Events.cs: line: 117");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nameLayer"></param>
        /// <param name="poly3d"></param>
        public static void deactivatePoly3ds(string nameLayer, Polyline3d poly3d = null)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    if (poly3d == null)
                    {
                        Type type = typeof(Polyline3d);
                        TypedValue[] TVs = new TypedValue[2];
                        TVs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(type).DxfName), 0);
                        TVs.SetValue(new TypedValue((int)DxfCode.LayerName, nameLayer), 1);

                        SelectionSet ss = Base_Tools45.Select.buildSSet(TVs);
                        foreach (ObjectId id in ss.GetObjectIds())
                        {
                            Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                            if (ent is Polyline3d)
                            {
                                poly3d = (Polyline3d)ent;
                                poly3d.Modified -= new EventHandler(poly3d_Modified);
                                poly3d.Erased -= new ObjectErasedEventHandler(poly3d_Erased);
                                poly3ds.Add(poly3d.ObjectId);
                                poly3ds.TrimExcess();
                            }
                        }
                    }
                    else
                    {
                        poly3d = (Polyline3d)poly3d.ObjectId.GetObject(OpenMode.ForWrite);
                        poly3d.Modified -= new EventHandler(poly3d_Modified);
                        poly3d.Erased -= new ObjectErasedEventHandler(poly3d_Erased);
                        poly3ds.Add(poly3d.ObjectId);
                        poly3ds.TrimExcess();
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Events.cs: line: 166");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="senderObj"></param>
        /// <param name="evtArgs"></param>
        public static void poly3d_Modified(object senderObj, EventArgs evtArgs)
        {
            MessageBox.Show(string.Format("The area of {0} is: {1}", poly.ToString(), poly3d.Area));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="senderObj"></param>
        /// <param name="evtArgs"></param>
        public static void poly3d_Erased(object senderObj, EventArgs evtArgs)
        {
            MessageBox.Show(poly3d.ObjectId.ToString());
        }

        #endregion "poly3d"

        #region "cogoPnts"

        /// <summary>
        ///
        /// </summary>
        /// <param name="cogoPnt"></param>
        public static void activateCogoPnts(CogoPoint cogoPnt = null)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    if (cogoPnt == null)
                    {
                        TypedValue[] TVs = new TypedValue[1];
                        TVs.SetValue(new TypedValue((int)DxfCode.Start, "AECC_COGO_POINT"), 0);
                        SelectionSet SS = Base_Tools45.Select.buildSSet(TVs);
                        foreach (ObjectId id in SS.GetObjectIds())
                        {
                            cogoPnt = (CogoPoint)tr.GetObject(id, OpenMode.ForWrite);

                            if (cogoPnt != null)
                            {
                                cogoPnt.Modified += new EventHandler(cogoPnt_Modified);
                                cogoPnt.Erased += new ObjectErasedEventHandler(cogoPnt_Erased);
                                cogoPnts.Add(cogoPnt.ObjectId);
                                cogoPnts.TrimExcess();
                            }
                        }// end foreach
                    }
                    else
                    {
                        cogoPnt = (CogoPoint)cogoPnt.ObjectId.GetObject(OpenMode.ForWrite);
                        cogoPnt.Modified += new EventHandler(cogoPnt_Modified);
                        cogoPnt.Erased += new ObjectErasedEventHandler(cogoPnt_Erased);
                        cogoPnts.Add(cogoPnt.ObjectId);
                        cogoPnts.TrimExcess();
                    }
                    tr.Commit();
                }// end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Events.cs: line: 235");
            }
        }// activateCogoPnts

        /// <summary>
        ///
        /// </summary>
        /// <param name="cogoPnt"></param>
        public static void deactivateCogoPnts(CogoPoint cogoPnt = null)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    if (cogoPnt == null)
                    {
                        TypedValue[] TVs = new TypedValue[1];
                        TVs.SetValue(new TypedValue((int)DxfCode.Start, "AECC_COGO_POINT"), 0);
                        SelectionSet SS = Base_Tools45.Select.buildSSet(TVs);
                        foreach (ObjectId id in SS.GetObjectIds())
                        {
                            cogoPnt = (CogoPoint)tr.GetObject(id, OpenMode.ForWrite);

                            if (cogoPnt != null)
                            {
                                cogoPnt.Modified -= new EventHandler(cogoPnt_Modified);
                                cogoPnt.Erased -= new ObjectErasedEventHandler(cogoPnt_Erased);
                                cogoPnts.Add(cogoPnt.ObjectId);
                                cogoPnts.TrimExcess();
                            }
                        }// end foreach
                    }
                    else
                    {
                        cogoPnt = (CogoPoint)cogoPnt.ObjectId.GetObject(OpenMode.ForWrite);
                        cogoPnt.Modified -= new EventHandler(cogoPnt_Modified);
                        cogoPnt.Erased -= new ObjectErasedEventHandler(cogoPnt_Erased);
                        cogoPnts.Add(cogoPnt.ObjectId);
                        cogoPnts.TrimExcess();
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Events.cs: line: 280");
            }
        }// deactivateCogoPnts

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void cogoPnt_Erased(object sender, EventArgs e)
        {
            MessageBox.Show(e.GetType().ToString());
        }// cogoPnt_Erased

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void cogoPnt_Modified(object sender, EventArgs e)
        {
            //ResultBuffer RB = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    try
                    {
                        //RB = cogoPnt.GetXDataForApplication("");
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Events.cs: line: 312");
                    }

                    tr.Commit();
                }// end using tr
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Events.cs: line: 320");
            }
        }// cogoPnt_Modified

        #endregion "cogoPnts"

        #region "mTxt"

        /// <summary>
        ///
        /// </summary>
        /// <param name="nameLayer"></param>
        /// <param name="mTxt"></param>
        public static void activateMText(string nameLayer, MText mTxt = null)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    if (mTxt == null)
                    {
                        TypedValue[] TVs = new TypedValue[2];
                        TVs.SetValue(new TypedValue((int)DxfCode.Start, "MTEXT"), 0);
                        TVs.SetValue(new TypedValue((int)DxfCode.LayerName, nameLayer), 1);

                        SelectionSet SS = Base_Tools45.Select.buildSSet(TVs);

                        foreach (SelectedObject OBJ in SS)
                        {
                            if (OBJ != null)
                            {
                                mTxt = (MText)tr.GetObject(OBJ.ObjectId, OpenMode.ForWrite);
                                mTxt.Modified += new EventHandler(mTxt_Modified);
                                mTxt.Erased += new ObjectErasedEventHandler(mTxt_Erased);
                                mTxts.Add(mTxt.ObjectId);
                                mTxts.TrimExcess();
                            }
                        }
                    }
                    else
                    {
                        mTxt = (MText)mTxt.ObjectId.GetObject(OpenMode.ForWrite);
                        mTxt.Modified += new EventHandler(mTxt_Modified);
                        mTxt.Erased += new ObjectErasedEventHandler(mTxt_Erased);
                        mTxts.Add(mTxt.ObjectId);
                        mTxts.TrimExcess();
                    }

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Events.cs: line: 373");
            }
        }// activateMText

        /// <summary>
        ///
        /// </summary>
        /// <param name="nameLayer"></param>
        /// <param name="mTxt"></param>
        public static void deactivateMText(string nameLayer, MText mTxt = null)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    if (mTxt == null)
                    {
                        TypedValue[] TVs = new TypedValue[2];
                        TVs.SetValue(new TypedValue((int)DxfCode.Start, "MTEXT"), 0);
                        TVs.SetValue(new TypedValue((int)DxfCode.LayerName, nameLayer), 1);
                        SelectionSet SS = Base_Tools45.Select.buildSSet(TVs);

                        foreach (SelectedObject OBJ in SS)
                        {
                            if (OBJ != null)
                            {
                                mTxt = (MText)tr.GetObject(OBJ.ObjectId, OpenMode.ForWrite);
                                mTxt.Modified -= new EventHandler(mTxt_Modified);
                                mTxt.Erased -= new ObjectErasedEventHandler(mTxt_Erased);
                                mTxts.Remove(mTxt.ObjectId);
                                mTxts.TrimExcess();
                            }
                        }
                    }
                    else
                    {
                        mTxt = (MText)mTxt.ObjectId.GetObject(OpenMode.ForWrite);
                        mTxt.Modified += new EventHandler(mTxt_Modified);
                        mTxt.Erased += new ObjectErasedEventHandler(mTxt_Erased);
                        mTxts.Remove(mTxt.ObjectId);
                        mTxts.TrimExcess();
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Events.cs: line: 419");
            }
        }// deactivateMText

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void mTxt_Erased(object sender, EventArgs e)
        {
            MessageBox.Show(e.GetType().ToString());
        }// mTxt_Erased

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void mTxt_Modified(object sender, EventArgs e)
        {
            //ResultBuffer RB = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    try
                    {
                        //RB = mTxt.GetXDataForApplication("FL");
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Events.cs: line: 451");
                    }

                    tr.Commit();
                }// end using tr
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Events.cs: line: 459");
            }
        }// mTxt_Modified

        #endregion "mTxt"

        #region "ldr"

        /// <summary>
        ///
        /// </summary>
        /// <param name="nameLayer"></param>
        /// <param name="ldr"></param>
        public static void activateLdr(string nameLayer, Leader ldr = null)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    if (ldr == null)
                    {
                        TypedValue[] TVs = new TypedValue[2];
                        TVs.SetValue(new TypedValue((int)DxfCode.Start, "LEADER"), 0);
                        TVs.SetValue(new TypedValue((int)DxfCode.LayerName, nameLayer), 1);
                        SelectionSet SS = Base_Tools45.Select.buildSSet(TVs);
                        foreach (SelectedObject ssObj in SS)
                        {
                            if (ssObj != null)
                            {
                                ldr = (Leader)tr.GetObject(ssObj.ObjectId, OpenMode.ForWrite);
                                ldr.Modified += new EventHandler(Ldr_Modified);
                                ldrs.Add(ldr.ObjectId);
                                ldrs.TrimExcess();
                            }
                        }
                    }
                    else
                    {
                        ldr = (Leader)ldr.ObjectId.GetObject(OpenMode.ForWrite);
                        ldr.Modified += new EventHandler(Ldr_Modified);
                        ldrs.Add(ldr.ObjectId);
                        ldrs.TrimExcess();
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Events.cs: line: 507");
            }
        }// activateLdr

        /// <summary>
        ///
        /// </summary>
        /// <param name="nameLayer"></param>
        /// <param name="ldr"></param>
        public static void deactivateLdr(string nameLayer, Leader ldr = null)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    if (ldr == null)
                    {
                        TypedValue[] TVs = new TypedValue[2];
                        TVs.SetValue(new TypedValue((int)DxfCode.Start, "LEADER"), 0);
                        TVs.SetValue(new TypedValue((int)DxfCode.LayerName, nameLayer), 1);

                        SelectionSet SS = Base_Tools45.Select.buildSSet(TVs);
                        foreach (SelectedObject OBJ in SS)
                        {
                            if (OBJ != null)
                            {
                                ldr = (Leader)tr.GetObject(OBJ.ObjectId, OpenMode.ForWrite);
                                ldr.Modified -= new EventHandler(Ldr_Modified);
                                ldrs.Remove(ldr.ObjectId);
                                ldrs.TrimExcess();
                            }
                        }
                    }
                    else
                    {
                        ldr = (Leader)ldr.ObjectId.GetObject(OpenMode.ForWrite);
                        ldr.Modified += new EventHandler(Ldr_Modified);
                        ldrs.Remove(ldr.ObjectId);
                        ldrs.TrimExcess();
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Events.cs: line: 552");
            }
        }// deactivateLdr

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Ldr_Modified(object sender, EventArgs e)
        {
            //ResultBuffer RB = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    try
                    {
                        //RB = ldr.GetXDataForApplication("FL");
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Events.cs: line: 574");
                    }

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Events.cs: line: 582");
            }
        }

        #endregion "ldr"
    }// class Events
}
