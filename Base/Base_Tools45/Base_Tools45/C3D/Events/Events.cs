using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using System;
using System.Collections.Generic;

namespace Base_Tools45.C3D.Events
{
    /// <summary>
    ///
    /// </summary>
    public static class Events
    {
        /// <summary>
        ///
        /// </summary>
        public static CogoPoint cogoPnt = null;

        /// <summary>
        ///
        /// </summary>
        public static MText mTxt = null;

        /// <summary>
        ///
        /// </summary>
        public static Leader Ldr = null;

        /// <summary>
        ///
        /// </summary>
        public static Polyline POLY = null;

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
        public static List<ObjectId> Ldrs = new List<ObjectId>();

        /// <summary>
        /// add eventhandler to all cogo points in drawing or a single point
        /// </summary>
        /// <param name="cogoPnt1"></param>
        public static void
        activatecogoPnts(CogoPoint cogoPnt1 = null)
        {
            Editor ED = BaseObjs._editor;

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                if (cogoPnt1 == null)
                {
                    TypedValue[] TVs = new TypedValue[1];
                    TVs.SetValue(new TypedValue((int)DxfCode.Start, "AECC_COGO_POINT"), 0);
                    SelectionFilter SF = new SelectionFilter(TVs);

                    PromptSelectionResult PSR = ED.SelectAll(SF);
                    if (PSR.Status == PromptStatus.OK)
                    {
                        SelectionSet SS = PSR.Value;

                        foreach (SelectedObject OBJ in SS)
                        {
                            if (OBJ != null)
                            {
                                cogoPnt = (CogoPoint)tr.GetObject(OBJ.ObjectId, OpenMode.ForWrite);
                                cogoPnt.Modified += new EventHandler(cogoPnt_Modified);
                                cogoPnt.Erased += new ObjectErasedEventHandler(cogoPnt_Erased);
                                cogoPnts.Add(cogoPnt.ObjectId);
                                cogoPnts.TrimExcess();
                            }
                        }// end foreach
                    }// end if
                }
                else
                {
                    cogoPnt = (CogoPoint)tr.GetObject(cogoPnt1.ObjectId, OpenMode.ForWrite);
                    cogoPnt.Modified += new EventHandler(cogoPnt_Modified);
                    cogoPnt.Erased += new ObjectErasedEventHandler(cogoPnt_Erased);
                }

                tr.Commit();
            }// end using
        }// activatecogoPnts

        /// <summary>
        /// remove event handler from cogo point
        /// </summary>
        public static void
        deactivatecogoPnts()
        {
            if (cogoPnt != null)
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    // Open the polyline for read
                    cogoPnt = (CogoPoint)tr.GetObject(cogoPnt.ObjectId, OpenMode.ForRead);

                    if (cogoPnt.IsWriteEnabled == false)
                    {
                        cogoPnt.UpgradeOpen();
                    }

                    cogoPnt.Modified -= new EventHandler(cogoPnt_Modified);
                    cogoPnts.Remove(cogoPnt.ObjectId);
                    cogoPnt = null;

                    tr.Commit();
                }
            }
        }// deactivateMText

        /// <summary>
        /// event cogo point erased
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void
        cogoPnt_Erased(object sender, EventArgs e)
        {
            Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog(e.GetType().ToString());
        }// cogoPnt_Erased

        /// <summary>
        /// event cogo point modified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void
        cogoPnt_Modified(object sender, EventArgs e)
        {
            ResultBuffer RB = null;

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                try
                {
                    RB = cogoPnt.GetXDataForApplication("lbkBrks");
                    if (RB != null)
                    {
                        Mod.updateBrkLine(cogoPnt);
                    }
                }
                catch
                {
                    tr.Abort();
                    return;
                }

                tr.Commit();
            }// end using tr
        }// cogoPnt_Modified

        /// <summary>
        /// add event handler for all mtext or single mtext
        /// </summary>
        /// <param name="mTxt1"></param>
        public static void
        activateMText(MText mTxt1 = null)
        {
            Editor ED = BaseObjs._editor;

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                if (mTxt1 == null)
                {
                    TypedValue[] TVs = new TypedValue[1];
                    TVs.SetValue(new TypedValue((int)DxfCode.Start, "MTEXT"), 0);
                    SelectionFilter SF = new SelectionFilter(TVs);

                    PromptSelectionResult PSR = ED.SelectAll(SF);
                    if (PSR.Status == PromptStatus.OK)
                    {
                        SelectionSet SS = PSR.Value;

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
                }
                else
                {
                    mTxt = (MText)tr.GetObject(mTxt1.ObjectId, OpenMode.ForWrite);
                    mTxt.Modified += new EventHandler(mTxt_Modified);
                    mTxt.Erased += new ObjectErasedEventHandler(mTxt_Erased);
                    mTxts.Add(mTxt.ObjectId);
                }

                tr.Commit();
            }
        }// activateMText

        /// <summary>
        /// remove event handler for mtext
        /// </summary>
        /// <param name="mTxt"></param>
        public static void
        deactivateMText(MText mTxt)
        {
            if (mTxt != null)
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    // Open the polyline for read
                    mTxt = (MText)tr.GetObject(mTxt.ObjectId, OpenMode.ForRead);

                    if (mTxt.IsWriteEnabled == false)
                    {
                        mTxt.UpgradeOpen();
                    }

                    mTxt.Modified -= new EventHandler(mTxt_Modified);
                    mTxt.Erased -= new ObjectErasedEventHandler(mTxt_Erased);
                    mTxts.Remove(mTxt.ObjectId);
                    mTxt = null;

                    tr.Commit();
                }
            }
        }// deactivateMText

        /// <summary>
        /// event mtext erased
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void
        mTxt_Erased(object sender, EventArgs e)
        {
            Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog(e.GetType().ToString());
        }// mTxt_Erased

        /// <summary>
        /// event mtext modified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void
        mTxt_Modified(object sender, EventArgs e)
        {
            ResultBuffer RB = null;

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                try
                {
                    RB = mTxt.GetXDataForApplication("FL");
                    if (RB != null)
                    {
                        Mod.updateDesignCallout(mTxt, "FL");
                    }
                }
                catch
                {
                    tr.Abort();
                    return;
                }

                tr.Commit();
            }// end using tr
        }// mTxt_Modified

        /// <summary>
        /// add event handler for all leaders or single leader
        /// </summary>
        /// <param name="Ldr1"></param>
        public static void
        activateLdr(Leader Ldr1 = null)
        {
            Editor ED = BaseObjs._editor;

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                if (Ldr1 == null)
                {
                    TypedValue[] TVs = new TypedValue[1];
                    TVs.SetValue(new TypedValue((int)DxfCode.Start, "LEADER"), 0);
                    SelectionFilter SF = new SelectionFilter(TVs);

                    PromptSelectionResult PSR = ED.SelectAll(SF);
                    if (PSR.Status == PromptStatus.OK)
                    {
                        SelectionSet SS = PSR.Value;

                        foreach (SelectedObject OBJ in SS)
                        {
                            if (OBJ != null)
                            {
                                Ldr = (Leader)tr.GetObject(OBJ.ObjectId, OpenMode.ForWrite);
                                Ldr.Modified += new EventHandler(Ldr_Modified);
                                Ldrs.Add(Ldr.ObjectId);
                                Ldrs.TrimExcess();
                            }
                        }
                    }
                }
                else
                {
                    Ldr = (Leader)tr.GetObject(Ldr1.ObjectId, OpenMode.ForWrite);
                    Ldr.Modified += new EventHandler(Ldr_Modified);
                }

                tr.Commit();
            }
        }// activateLdr

        /// <summary>
        ///
        /// </summary>
        /// <param name="Ldr"></param>
        public static void
        deactivateLdr(Leader Ldr)
        {
            if (Ldr != null)
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    // Open the polyline for read
                    Ldr = (Leader)tr.GetObject(Ldr.ObjectId, OpenMode.ForRead);

                    if (Ldr.IsWriteEnabled == false)
                    {
                        Ldr.UpgradeOpen();
                    }

                    Ldr.Modified -= new EventHandler(Ldr_Modified);
                    Ldrs.Remove(Ldr.ObjectId);
                    Ldr = null;
                }
            }
        }// deactivateLdr

        /// <summary>
        /// example of adding event handler to polyline
        /// </summary>
        public static void
        AddPLObjEvent()
        {
            Database DB = BaseObjs._db;

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                BlockTable BT = (BlockTable)tr.GetObject(DB.BlockTableId, OpenMode.ForRead);
                BlockTableRecord MS = (BlockTableRecord)tr.GetObject(BT[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                // Create a closed polyline
                POLY = new Polyline();
                POLY.AddVertexAt(0, new Point2d(1, 1), 0, 0, 0);
                POLY.AddVertexAt(1, new Point2d(1, 2), 0, 0, 0);
                POLY.AddVertexAt(2, new Point2d(2, 2), 0, 0, 0);
                POLY.AddVertexAt(3, new Point2d(3, 3), 0, 0, 0);
                POLY.AddVertexAt(4, new Point2d(3, 2), 0, 0, 0);
                POLY.AddVertexAt(5, new Point2d(1, 1), 0, 0, 0);
                POLY.Closed = true;

                MS.AppendEntity(POLY);
                tr.AddNewlyCreatedDBObject(POLY, true);

                POLY.Modified += new EventHandler(POLY_Modified);
                POLY.Erased += new ObjectErasedEventHandler(POLY_Erased);

                tr.Commit();
            }
        }

        /// <summary>
        /// example of removing event handler from polyline
        /// </summary>
        public static void
        RemovePlObjEvent()
        {
            if (POLY != null)
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    // Open the polyline for read
                    POLY = tr.GetObject(POLY.ObjectId,
                        OpenMode.ForRead) as Polyline;

                    if (POLY.IsWriteEnabled == false)
                    {
                        POLY.UpgradeOpen();
                    }

                    POLY.Modified -= new EventHandler(POLY_Modified);
                    POLY = null;

                    tr.Commit();
                }
            }
        }

        /// <summary>
        /// example polyine modified
        /// </summary>
        /// <param name="senderObj"></param>
        /// <param name="evtArgs"></param>
        public static void
        POLY_Modified(object senderObj, EventArgs evtArgs)
        {
            Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog(string.Format("The area of {0} is: {1}", POLY.ToString(), POLY.Area));
        }

        /// <summary>
        /// example polyline deleted
        /// </summary>
        /// <param name="senderObj"></param>
        /// <param name="evtArgs"></param>
        public static void
        POLY_Erased(object senderObj, EventArgs evtArgs)
        {
            Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog(POLY.ObjectId.ToString());
        }

        /// <summary>
        /// event leader modified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void
        Ldr_Modified(object sender, EventArgs e)
        {
            ResultBuffer RB = null;

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                try
                {
                    RB = Ldr.GetXDataForApplication("FL");
                    if (RB != null)
                    {
                        Mod.updateDesignCallout(Ldr, "FL");
                    }
                }
                catch
                {
                    tr.Abort();
                    return;
                }

                tr.Commit();
            }
        }
    }// class Events
}