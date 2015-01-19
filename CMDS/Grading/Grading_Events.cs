using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Base_Tools45;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;
using Autodesk.Civil.DatabaseServices;

namespace Grading {
    public static class Grading_Events {
        #region declares
        
        public static Entity ent = null;
        public static CogoPoint cogoPnt = null;
        public static MText mTxt = null;
        public static Leader ldr = null;
        public static Polyline poly = null;
        public static Polyline3d poly3d = null;
        public static List<ObjectId> ents = new List<ObjectId>();
        public static List<ObjectId> cogoPnts = new List<ObjectId>();
        public static List<ObjectId> mTxts = new List<ObjectId>();
        public static List<ObjectId> ldrs = new List<ObjectId>();
        public static List<ObjectId> polys = new List<ObjectId>();
        public static List<ObjectId> poly3ds = new List<ObjectId>();
        
        #endregion
        
        public static void
        activateEnts(List<ObjectId> ids) {
            using (Transaction tr = BaseObjs.startTransactionDb()) {
                foreach (ObjectId id in ids) {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                    ent.Modified += new EventHandler(ent_Modified);
                    ent.Erased += new ObjectErasedEventHandler(ent_Erased);
                    ents.Add(id); ents.TrimExcess();
                }
                tr.Commit();
            }
        }
        
        public static void
        deactivateEnts(List<ObjectId> ids) {
            using (Transaction tr = BaseObjs.startTransactionDb()) {
                foreach (ObjectId id in ids) {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                    ent.Modified -= new EventHandler(ent_Modified);
                    ent.Erased -= new ObjectErasedEventHandler(ent_Erased);
                    ents.Add(id); ents.TrimExcess();
                }
                tr.Commit();
            }
        }
            
        public static void ent_Modified(object senderObj, EventArgs evtArgs) {
        }
            
        public static void ent_Erased(object senderObj, EventArgs evtArgs) {
        }
        
        #region "poly3d"
        
        public static void activatePoly3ds(string nameLayer = "", Object idPoly3d = null) {
            using (BaseObjs._acadDoc.LockDocument()) {
                using (Transaction TR = BaseObjs.startTransactionDb()) {
                    if (idPoly3d == null) {
                        Type type = typeof(Polyline3d);
                        TypedValue[] TVs = new TypedValue[2];
                        TVs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(type).DxfName), 0);
                        TVs.SetValue(new TypedValue((int)DxfCode.LayerName, nameLayer), 1);
                        SelectionSet ss = Select.buildSSet(TVs);
                                
                        if (ss != null) {
                            foreach (ObjectId id in ss.GetObjectIds()) {
                                Entity ent = (Entity)TR.GetObject(id, OpenMode.ForWrite);
                                if (ent.GetType() == typeof(Polyline3d)) {
                                    poly3d = (Polyline3d)ent;
                                    poly3d.Modified += new EventHandler(poly3d_Modified);
                                    poly3d.Erased += new ObjectErasedEventHandler(poly3d_Erased);
                                    poly3ds.Add(poly3d.ObjectId); poly3ds.TrimExcess();
                                }
                            }
                        }
                    }else {
                        poly3d = (Polyline3d)TR.GetObject((ObjectId)idPoly3d, OpenMode.ForWrite);
                        poly3d.Modified += new EventHandler(poly3d_Modified);
                        poly3d.Erased += new ObjectErasedEventHandler(poly3d_Erased);
                        poly3ds.Add((ObjectId)idPoly3d); poly3ds.TrimExcess();
                    }
                    TR.Commit();
                }
            }
        }
        
        public static void deactivatePoly3ds(string nameLayer = "", Polyline3d poly3d = null) {
            using (BaseObjs._acadDoc.LockDocument()) {
                using (Transaction TR = BaseObjs.startTransactionDb()) {
                    if (poly3d == null) {
                        Type type = typeof(Polyline3d);
                        TypedValue[] TVs = new TypedValue[2];
                        TVs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(type).DxfName), 0);
                        TVs.SetValue(new TypedValue((int)DxfCode.LayerName, nameLayer), 1);
                            
                        SelectionSet ss = Select.buildSSet(TVs);
                        if (ss != null) {
                            foreach (ObjectId id in ss.GetObjectIds()) {
                                Entity ent = (Entity)TR.GetObject(id, OpenMode.ForWrite);
                                if (ent.GetType() == typeof(Polyline3d)) {
                                    poly3d = (Polyline3d)ent;
                                    poly3d.Modified -= new EventHandler(poly3d_Modified);
                                    poly3d.Erased -= new ObjectErasedEventHandler(poly3d_Erased);
                                    poly3ds.Remove(poly3d.ObjectId); poly3ds.TrimExcess();
                                }
                            }
                        }
                    }else {
                        poly3d = (Polyline3d)poly3d.ObjectId.GetObject(OpenMode.ForWrite);
                        poly3d.Modified -= new EventHandler(poly3d_Modified);
                        poly3d.Erased -= new ObjectErasedEventHandler(poly3d_Erased);
                        poly3ds.Remove(poly3d.ObjectId); poly3ds.TrimExcess();
                    }
                    TR.Commit();
                }
            }
        }
        
        public static void poly3d_Modified(object senderObj, EventArgs evtArgs) {
        }
        
        public static void poly3d_Erased(object senderObj, EventArgs evtArgs) {
        }
        
        #endregion
        
        #region "cogoPnts"
        
        public static void activateCogoPnts(CogoPoint cogoPnt = null) {
            using (BaseObjs._acadDoc.LockDocument()) {
                using (Transaction TR = BaseObjs.startTransactionDb()) {
                    if (cogoPnt == null) {
                        TypedValue[] TVs = new TypedValue[1];
                        TVs.SetValue(new TypedValue((int)DxfCode.Start, "AECC_COGO_POINT"), 0);
                        SelectionSet ss = Select.buildSSet(TVs);
                        if (ss != null) {
                            foreach (ObjectId id in ss.GetObjectIds()) {
                                cogoPnt = (CogoPoint)TR.GetObject(id, OpenMode.ForWrite);
                                    
                                if (cogoPnt != null) {
                                    cogoPnt.Modified += new EventHandler(cogoPnt_Modified);
                                    cogoPnt.Erased += new ObjectErasedEventHandler(cogoPnt_Erased);
                                    cogoPnts.Add(cogoPnt.ObjectId); cogoPnts.TrimExcess();
                                }
                            }// end foreach                       
                        }
                    }else {
                        cogoPnt = (CogoPoint)cogoPnt.ObjectId.GetObject(OpenMode.ForWrite);
                        cogoPnt.Modified += new EventHandler(cogoPnt_Modified);
                        cogoPnt.Erased += new ObjectErasedEventHandler(cogoPnt_Erased);
                        cogoPnts.Add(cogoPnt.ObjectId); cogoPnts.TrimExcess();
                    }
                    TR.Commit();
                }// end using
            }
        }// activateCogoPnts
        
        public static void deactivateCogoPnts(CogoPoint cogoPnt = null) {
            using (BaseObjs._acadDoc.LockDocument()) {
                using (Transaction TR = BaseObjs.startTransactionDb()) {
                    if (cogoPnt == null) {
                        TypedValue[] TVs = new TypedValue[1];
                        TVs.SetValue(new TypedValue((int)DxfCode.Start, "AECC_COGO_POINT"), 0);
                        SelectionSet ss = Select.buildSSet(TVs);
                        if (ss != null) {
                            foreach (ObjectId id in ss.GetObjectIds()) {
                                cogoPnt = (CogoPoint)TR.GetObject(id, OpenMode.ForWrite);
                                    
                                if (cogoPnt != null) {
                                    cogoPnt.Modified -= new EventHandler(cogoPnt_Modified);
                                    cogoPnt.Erased -= new ObjectErasedEventHandler(cogoPnt_Erased);
                                    cogoPnts.Remove(cogoPnt.ObjectId); cogoPnts.TrimExcess();
                                }
                            }// end foreach                        
                        }
                    }else {
                        cogoPnt = (CogoPoint)cogoPnt.ObjectId.GetObject(OpenMode.ForWrite);
                        cogoPnt.Modified -= new EventHandler(cogoPnt_Modified);
                        cogoPnt.Erased -= new ObjectErasedEventHandler(cogoPnt_Erased);
                        cogoPnts.Remove(cogoPnt.ObjectId); cogoPnts.TrimExcess();
                    }
                    TR.Commit();
                }
            }
        }// deactivateCogoPnts
        
        public static void cogoPnt_Erased(object senderObj, EventArgs e) {
        }// cogoPnt_Erased
        
        public static void cogoPnt_Modified(object senderObj, EventArgs e) {
            CogoPoint cogoPnt = (CogoPoint)senderObj;
            Grading_Utility.objPnt_Modified(cogoPnt.ObjectId);
        }
        
        #endregion
        
        #region "mTxt"
        
        public static void activateMText(string nameLayer, MText mTxt = null) {
            using (BaseObjs._acadDoc.LockDocument()) {
                using (Transaction TR = BaseObjs.startTransactionDb()) {
                    if (mTxt == null) {
                        TypedValue[] TVs = new TypedValue[2];
                        TVs.SetValue(new TypedValue((int)DxfCode.Start, "MTEXT"), 0);
                        TVs.SetValue(new TypedValue((int)DxfCode.LayerName, nameLayer), 1);
                                
                        SelectionSet SS = Select.buildSSet(TVs);
                                
                        foreach (SelectedObject OBJ in SS) {
                            if (OBJ != null) {
                                mTxt = (MText)TR.GetObject(OBJ.ObjectId, OpenMode.ForWrite);
                                mTxt.Modified += new EventHandler(mTxt_Modified);
                                mTxt.Erased += new ObjectErasedEventHandler(mTxt_Erased);
                                mTxts.Add(mTxt.ObjectId); mTxts.TrimExcess();
                            }
                        }
                    }else {
                        mTxt = (MText)mTxt.ObjectId.GetObject(OpenMode.ForWrite);
                        mTxt.Modified += new EventHandler(mTxt_Modified);
                        mTxt.Erased += new ObjectErasedEventHandler(mTxt_Erased);
                        mTxts.Add(mTxt.ObjectId); mTxts.TrimExcess();
                    }

                    TR.Commit();
                }
            }
        }// activateMText
        
        public static void deactivateMText(string nameLayer, MText mTxt = null) {
            using (BaseObjs._acadDoc.LockDocument()) {
                using (Transaction TR = BaseObjs.startTransactionDb()) {
                    if (mTxt == null) {
                        TypedValue[] TVs = new TypedValue[2];
                        TVs.SetValue(new TypedValue((int)DxfCode.Start, "MTEXT"), 0);
                        TVs.SetValue(new TypedValue((int)DxfCode.LayerName, nameLayer), 1);
                        SelectionSet SS = Select.buildSSet(TVs);
                                
                        foreach (SelectedObject OBJ in SS) {
                            if (OBJ != null) {
                                mTxt = (MText)TR.GetObject(OBJ.ObjectId, OpenMode.ForWrite);
                                mTxt.Modified -= new EventHandler(mTxt_Modified);
                                mTxt.Erased -= new ObjectErasedEventHandler(mTxt_Erased);
                                mTxts.Remove(mTxt.ObjectId); mTxts.TrimExcess();
                            }
                        }
                    }else {
                        mTxt = (MText)mTxt.ObjectId.GetObject(OpenMode.ForWrite);
                        mTxt.Modified += new EventHandler(mTxt_Modified);
                        mTxt.Erased += new ObjectErasedEventHandler(mTxt_Erased);
                        mTxts.Remove(mTxt.ObjectId); mTxts.TrimExcess();
                    }
                }
            }
        }// deactivateMText
        
        public static void mTxt_Erased(object sender, EventArgs e) {
        }// mTxt_Erased
        
        public static void mTxt_Modified(object sender, EventArgs e) {
            using (BaseObjs._acadDoc.LockDocument()) {
                using (Transaction TR = BaseObjs.startTransactionDb()) {
                    try {
                    }
                    catch (System.Exception ex){
                        BaseObjs.writeDebug(ex.Message + " Grading_Events.cs: line: 276");
                    }

                    TR.Commit();
                }// end using TR 
            }
        }// mTxt_Modified
        
        #endregion
        
        #region "ldr"
        
        public static void activateLdr(string nameLayer, Leader ldr = null) {
            using (BaseObjs._acadDoc.LockDocument()) {
                using (Transaction TR = BaseObjs.startTransactionDb()) {
                    if (ldr == null) {
                        TypedValue[] TVs = new TypedValue[2];
                        TVs.SetValue(new TypedValue((int)DxfCode.Start, "LEADER"), 0);
                        TVs.SetValue(new TypedValue((int)DxfCode.LayerName, nameLayer), 1);
                        SelectionSet SS = Select.buildSSet(TVs);
                        foreach (SelectedObject ssObj in SS) {
                            if (ssObj != null) {
                                ldr = (Leader)TR.GetObject(ssObj.ObjectId, OpenMode.ForWrite);
                                ldr.Modified += new EventHandler(Ldr_Modified);
                                ldrs.Add(ldr.ObjectId); ldrs.TrimExcess();
                            }
                        }
                    }else {
                        ldr = (Leader)ldr.ObjectId.GetObject(OpenMode.ForWrite);
                        ldr.Modified += new EventHandler(Ldr_Modified);
                        ldrs.Add(ldr.ObjectId); ldrs.TrimExcess();
                    }
                    TR.Commit();
                }
            }
        }// activateLdr
        
        public static void deactivateLdr(string nameLayer, Leader ldr = null) {
            using (BaseObjs._acadDoc.LockDocument()) {
                using (Transaction TR = BaseObjs.startTransactionDb()) {
                    if (ldr == null) {
                        TypedValue[] TVs = new TypedValue[2];
                        TVs.SetValue(new TypedValue((int)DxfCode.Start, "LEADER"), 0);
                        TVs.SetValue(new TypedValue((int)DxfCode.LayerName, nameLayer), 1);
                                
                        SelectionSet SS = Select.buildSSet(TVs);
                        foreach (SelectedObject OBJ in SS) {
                            if (OBJ != null) {
                                ldr = (Leader)TR.GetObject(OBJ.ObjectId, OpenMode.ForWrite);
                                ldr.Modified -= new EventHandler(Ldr_Modified);
                                ldrs.Remove(ldr.ObjectId); ldrs.TrimExcess();
                            }
                        }
                    }else {
                        ldr = (Leader)ldr.ObjectId.GetObject(OpenMode.ForWrite);
                        ldr.Modified += new EventHandler(Ldr_Modified);
                        ldrs.Remove(ldr.ObjectId); ldrs.TrimExcess();
                    }
                    TR.Commit();
                }
            }
        }// deactivateLdr
        
        private static void Ldr_Modified(object sender, EventArgs e) {
            using (BaseObjs._acadDoc.LockDocument()) {
                using (Transaction TR = BaseObjs.startTransactionDb()) {
                    try {
                    }
                    catch (System.Exception ex){
                        BaseObjs.writeDebug(ex.Message + " Grading_Events.cs: line: 345");
                    }
    
                    TR.Commit();
                }
            }
        }

        #endregion
    }// class Grading_Events
}
