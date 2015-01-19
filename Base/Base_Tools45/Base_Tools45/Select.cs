using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
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
    public static class Select
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static SelectionSet
        buildSSet(Type type, bool selectAll = true, string prompt = "")
        {
            TypedValue[] TVs = new TypedValue[1];
            TVs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(type).DxfName), 0);
            return buildSSetBase(TVs, selectAll, prompt);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="TVs"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static SelectionSet
        buildSSet(TypedValue[] TVs, bool selectAll = true)
        {
            return buildSSetBase(TVs, selectAll);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="TVs"></param>
        /// <returns></returns>
        public static SelectionSet
        buildSSet(TypedValue[] TVs)
        {
            SelectionFilter filter = new SelectionFilter(TVs);
            PromptSelectionResult PSR = BaseObjs._editor.SelectAll(filter);
            if (PSR.Status == PromptStatus.OK)
                return PSR.Value;
            else
                return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pnt3dLower"></param>
        /// <param name="pnt3dUpper"></param>
        /// <returns></returns>
        public static SelectionSet
        buildSSet(Type type, Point3d pnt3dLower, Point3d pnt3dUpper)
        {
            TypedValue[] TVs = new TypedValue[1];
            TVs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(type).DxfName), 0);
            SelectionFilter filter = new SelectionFilter(TVs);
            PromptSelectionResult PSR = BaseObjs._editor.SelectCrossingWindow(pnt3dLower, pnt3dUpper, filter);
            if (PSR.Status == PromptStatus.OK)
                return PSR.Value;
            else
                return null;
        }

        public static SelectionSet
        buildSSet(TypedValue[] tvs, Point3d pnt3dLower, Point3d pnt3dUpper)
        {
            SelectionFilter filter = new SelectionFilter(tvs);
            PromptSelectionResult PSR = BaseObjs._editor.SelectCrossingWindow(pnt3dLower, pnt3dUpper, filter);
            if (PSR.Status == PromptStatus.OK)
                return PSR.Value;
            else
                return null;
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="tvs"></param>
        /// <param name="pnts3d"></param>
        /// <returns></returns>
        public static SelectionSet
        buildSSet(TypedValue[] tvs, Point3dCollection pnts3d)
        {
            SelectionFilter filter = new SelectionFilter(tvs);
            PromptSelectionResult psr = BaseObjs._editor.SelectCrossingPolygon(pnts3d, filter);
            return psr.Value;
        }

        public static SelectionSet
        buildSSetBase(TypedValue[] tvs, bool selectAll = true, string prompt = "")
        {
            SelectionFilter filter = new SelectionFilter(tvs);
            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = prompt;
            pso.MessageForRemoval = "";

            PromptSelectionResult psr = null;

            if (selectAll)
            {
                psr = BaseObjs._editor.SelectAll(filter);
            }
            else
            {
                psr = BaseObjs._editor.GetSelection(pso, filter);
            }

            if (psr.Status == PromptStatus.OK)
            {
                return psr.Value;
            }
            else
            {
                return null;
            }
        }

        public static ObjectIdCollection
        buildSSetIDs(Type type, bool selectAll = true)
        {
            TypedValue[] TVs = new TypedValue[1];
            TVs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(type).DxfName), 0);
            ObjectIdCollection ids = new ObjectIdCollection();
            foreach (ObjectId id in buildSSetBase(TVs, selectAll).GetObjectIds())
            {
                ids.Add(id);
            }
            return ids;
        }

        public static ObjectId[]
        checkForEntityatPoint(Point3d pnt3d1, Point3d pnt3d2, Type type)
        {
            ObjectId[] ids = null;
            TypedValue[] TVs = new TypedValue[1];
            TVs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(type).DxfName), 0);
            SelectionFilter FILTER = new SelectionFilter(TVs);

            PromptSelectionResult PSR = BaseObjs._editor.SelectCrossingWindow(pnt3d1, pnt3d2, FILTER);
            if (PSR.Status == PromptStatus.OK)
            {
                ids = PSR.Value.GetObjectIds();
            }
            return ids;
        }

        public static bool
        checkForEntityatPoint(Point3d pnt3d, ObjectId idObj, Type type)
        {
            bool match = false;
            TypedValue[] TVs = new TypedValue[1];
            TVs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(type).DxfName), 0);
            SelectionFilter FILTER = new SelectionFilter(TVs);

            PromptSelectionResult PSR = BaseObjs._editor.SelectCrossingWindow(pnt3d, pnt3d, FILTER);
            if (PSR.Status == PromptStatus.OK)
            {
                ObjectId[] ids = PSR.Value.GetObjectIds();

                foreach (ObjectId id in ids)
                {
                    if (id == idObj)
                    {
                        match = true;
                        break;
                    }
                }
            }
            return match;
        }

        public static ObjectId
        getBrkLine(string prompt)
        {
            ObjectId idBrkLine = ObjectId.Null;

            TypedValue[] tvs = new TypedValue[2];
            tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline3d)).DxfName), 0);
            tvs.SetValue(new TypedValue((int)DxfCode.LayerName, "CPNT-BRKLINE"), 1);

            SelectionFilter filter = new SelectionFilter(tvs);

            SelectionSet ss = null;

            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = prompt;
            pso.AllowDuplicates = false;
            pso.SingleOnly = true;

            PromptSelectionResult psr = null;

            psr = BaseObjs._editor.GetSelection(pso, filter);

            ss = psr.Value;

            if (ss == null)
                return idBrkLine;

            switch (ss.Count)
            {
                case 0:
                    MessageBox.Show("3dPolyline selection failed. Exiting........");
                    break;

                case 1:
                    try
                    {
                        using (Transaction tr = BaseObjs.startTransactionDb())
                        {
                            ObjectId[] ids = ss.GetObjectIds();
                            ObjectId id = ids[0];
                            Entity ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                            if (ent is Polyline3d)
                                idBrkLine = ent.ObjectId;
                            tr.Commit();
                        }
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Select.cs: line: 237");
                    }
                    break;

                default:
                    MessageBox.Show("Multiple 3dPolylines selected. Exiting....");
                    break;
            }
            return idBrkLine;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static ObjectIdCollection
        getBrkLines()
        {
            ObjectIdCollection idsBrkLine = new ObjectIdCollection();

            TypedValue[] tvs = new TypedValue[2];
            tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Polyline3d)).DxfName), 0);
            tvs.SetValue(new TypedValue((int)DxfCode.LayerName, "CPNT-BRKLINE"), 1);

            SelectionSet ss = Select.buildSSet(tvs);
            if (ss == null)
                return idsBrkLine;

            ObjectId[] ids = ss.GetObjectIds();
            for (int i = 0; i < ids.Length; i++)
                idsBrkLine.Add(ids[i]);

            return idsBrkLine;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="selSet"></param>
        /// <returns></returns>
        public static ObjectId[]
        getEntitysAtPoint(Point3d pnt3d, string nameLayer)
        {
            ObjectId[] ids = null;
            TypedValue[] tvs = new TypedValue[1];
            tvs.SetValue(new TypedValue((int)DxfCode.LayerName, nameLayer), 0);
            SelectionFilter filter = new SelectionFilter(tvs);
            PromptSelectionResult psr = BaseObjs._editor.SelectCrossingWindow(pnt3d, pnt3d, filter);
            if (psr.Status == PromptStatus.OK)
                ids = psr.Value.GetObjectIds();

            return ids;
        }

        public static List<ObjectId>
        getEntityatPoint(Point3d pnt3d, string nameLayer = "*")
        {
            List<ObjectId> ids = null;
            TypedValue[] TVs = new TypedValue[6];
            TVs.SetValue(new TypedValue((int)DxfCode.Operator, "<OR"), 0);
            TVs.SetValue(new TypedValue((int)DxfCode.Start, "LINE"), 1);
            TVs.SetValue(new TypedValue((int)DxfCode.Start, "ARC"), 2);
            TVs.SetValue(new TypedValue((int)DxfCode.Start, "CIRCLE"), 3);
            TVs.SetValue(new TypedValue((int)DxfCode.Start, "POLYLINE"), 4);
            TVs.SetValue(new TypedValue((int)DxfCode.Operator, "OR>"), 5);
            //TVs.SetValue(new TypedValue((int)DxfCode.LayerName, nameLayer), 6);

            SelectionFilter FILTER = new SelectionFilter(TVs);
            PromptSelectionResult PSR = BaseObjs._editor.SelectCrossingWindow(pnt3d, pnt3d, FILTER);
            if (PSR.Status == PromptStatus.OK)
            {
                ObjectId[] idss = PSR.Value.GetObjectIds();
                foreach (ObjectId id in idss)
                    ids.Add(id);
            }
            return ids;
        }

        public static List<ObjectId>
        getEntityatPoint(Point3d pnt3d, Type type, string layer, double offset = 0.0)
        {
            List<ObjectId> ids = new List<ObjectId>();
            TypedValue[] tvs = new TypedValue[2];

            tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(type).DxfName), 0);
            tvs.SetValue(new TypedValue((int)DxfCode.LayerName, layer), 1);

            SelectionFilter filter = new SelectionFilter(tvs);
            Point3d pnt3d1 = new Point3d(pnt3d.X - offset, pnt3d.Y - offset, 0.0);
            Point3d pnt3d2 = new Point3d(pnt3d.X + offset, pnt3d.Y + offset, 0.0);

            //Draw.addLine(pnt3d1, pnt3d2);

            PromptSelectionResult psr = BaseObjs._editor.SelectCrossingWindow(pnt3d2, pnt3d1, filter);
            if (psr.Status == PromptStatus.OK)
            {
                ObjectId[] idss = psr.Value.GetObjectIds();
                foreach (ObjectId id in idss)
                    ids.Add(id);
            }
            return ids;
        }

        public static ObjectId[]
        getEntityatPoint(Point3d pnt3d, List<Type> types, string layer, double offset = 0.0)
        {
            int k = types.Count;

            TypedValue[] TVs = new TypedValue[k + 3];

            TVs.SetValue(new TypedValue((int)DxfCode.Operator, "<OR"), 0);

            for (int i = 0; i < types.Count; i++)
                TVs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(types[i]).DxfName), i + 1);

            TVs.SetValue(new TypedValue((int)DxfCode.Operator, "OR>"), k + 1);
            TVs.SetValue(new TypedValue((int)DxfCode.LayerName, layer), k + 2);

            SelectionFilter FILTER = new SelectionFilter(TVs);

            Point3d pnt3d1 = new Point3d(pnt3d.X - offset, pnt3d.Y - offset, 0.0);
            Point3d pnt3d2 = new Point3d(pnt3d.X + offset, pnt3d.Y + offset, 0.0);

            PromptSelectionResult PSR = BaseObjs._editor.SelectCrossingWindow(pnt3d1, pnt3d2, FILTER);
            if (PSR.Status == PromptStatus.OK)
            {
                return PSR.Value.GetObjectIds();
            }
            return null;
        }

        public static bool
        getSelSetFromUser(out ObjectIdCollection ids)
        {
            bool escape = true;
            ids = null;
            Editor ed = BaseObjs._editor;

            PromptSelectionResult res = ed.GetSelection();

            if (res.Status != PromptStatus.OK)
                return escape;
            else
                escape = false;

            ids = new ObjectIdCollection(res.Value.GetObjectIds());

            return escape;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="strBlkName"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static SelectionSet
        selectBlkRefs(string strBlkName, bool selectAll = true)
        {
            TypedValue[] TVs = new TypedValue[2];
            TVs.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 0);
            TVs.SetValue(new TypedValue((int)DxfCode.BlockName, strBlkName), 1);

            return buildSSetBase(TVs, selectAll);
        }// selectBlkRefs

        /// <summary>
        /// select Cogo Point at point location
        /// </summary>
        /// <param name="pnt3d"></param>
        /// <returns></returns>
        public static ObjectId
        selectCogoPntAtPoint3d(Point3d pnt3d)
        {
            Autodesk.AutoCAD.DatabaseServices.Entity ENT = null;
            Point3d pnt3d0 = new Point3d(pnt3d.X, pnt3d.Y, 0);
            CogoPoint cogoPnt = null;
            ObjectId idCgPnt = ObjectId.Null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    TypedValue[] TVs = new TypedValue[1];
                    TVs.SetValue(new TypedValue((int)DxfCode.Start, "AECC_COGO_POINT"), 0);
                    SelectionFilter FILTER = new SelectionFilter(TVs);

                    PromptSelectionResult PSR = BaseObjs._editor.SelectCrossingWindow(pnt3d0, pnt3d0, FILTER);

                    if (PSR.Status == PromptStatus.OK)
                    {
                        SelectionSet SS = PSR.Value;

                        if (SS.Count > 1)
                        {
                            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(string.Format("{0} Points Found at Selected Location - Exiting....", SS.Count.ToString()));
                            tr.Commit();
                            return idCgPnt;
                        }
                        SelectedObject OBJ = SS[0];
                        if (OBJ != null)
                        {
                            ENT = tr.GetObject(OBJ.ObjectId, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Entity;
                        }
                        if (ENT != null)
                        {
                            if (ENT is CogoPoint)
                            {
                                cogoPnt = (CogoPoint)ENT;
                                idCgPnt = cogoPnt.ObjectId;
                                tr.Commit();
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Select.cs: line: 454");
            }
            return idCgPnt;
        }

        public static Entity
        selectEntity(string message, out bool escape)
        {
            escape = false;
            Entity ent = null;
            BaseObjs.acadActivate();
            PromptEntityOptions peos = new PromptEntityOptions(message);
            peos.AllowNone = false;

            PromptEntityResult per = BaseObjs._editor.GetEntity(peos);

            if (per.Status == PromptStatus.OK)
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ent = (Entity)tr.GetObject(per.ObjectId, OpenMode.ForRead);
                    tr.Commit();
                }
            }
            else
            {
                escape = true;
            }
            return ent;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="message"></param>
        /// <param name="reject"></param>
        /// <param name="pnt3dPicked"></param>
        /// <returns></returns>
        public static Entity
        selectEntity(System.Type classType, string message, string reject, out Point3d pnt3dPicked)
        {
            Entity ENT = null;
            BaseObjs.acadActivate();
            PromptEntityOptions PEOS = new PromptEntityOptions(message);
            PEOS.SetRejectMessage(reject);
            PEOS.AddAllowedClass(classType, false);
            PEOS.AllowNone = false;

            PromptEntityResult PER = BaseObjs._editor.GetEntity(PEOS);
            pnt3dPicked = PER.PickedPoint;

            switch (PER.Status)
            {
                case PromptStatus.Cancel:
                case PromptStatus.Error:
                    break;

                case PromptStatus.OK:

                    try
                    {
                        using (Transaction tr = BaseObjs.startTransactionDb())
                        {
                            ENT = (Entity)tr.GetObject(PER.ObjectId, OpenMode.ForRead);
                            tr.Commit();
                        }
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Select.cs: line: 524");
                    }
                    break;
            }
            return ENT;
        }

        public static Entity
        selectEntity(List<Type> tps, string message, string reject, out Point3d pnt3dPicked)
        {
            Entity ENT = null;
            BaseObjs.acadActivate();
            PromptEntityOptions PEOS = new PromptEntityOptions(message);
            PEOS.SetRejectMessage(reject);
            PEOS.AllowNone = false;
            foreach (Type tp in tps)
            {
                PEOS.AddAllowedClass(tp, true);
            }

            PromptEntityResult PER = BaseObjs._editor.GetEntity(PEOS);
            pnt3dPicked = PER.PickedPoint;

            switch (PER.Status)
            {
                case PromptStatus.Cancel:
                case PromptStatus.Error:
                    break;

                case PromptStatus.OK:

                    try
                    {
                        using (Transaction tr = BaseObjs.startTransactionDb())
                        {
                            ENT = (Entity)tr.GetObject(PER.ObjectId, OpenMode.ForRead);
                            tr.Commit();
                        }
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Select.cs: line: 565");
                    }
                    break;
            }
            return ENT;
        }

        public static Entity
        selectEntity(System.Type classType, string message, string reject, out Point3d pnt3dPicked, out PromptStatus ps)
        {
            Entity ent = null;
            BaseObjs.acadActivate();
            PromptEntityOptions peos = new PromptEntityOptions(message);
            peos.SetRejectMessage(reject);
            peos.AddAllowedClass(classType, true);
            peos.AllowNone = true;

            PromptEntityResult per = BaseObjs._editor.GetEntity(peos);
            pnt3dPicked = per.PickedPoint;
            ps = per.Status;

            switch (ps)
            {
                case PromptStatus.OK:
                    try
                    {
                        using (Transaction tr = BaseObjs.startTransactionDb())
                        {
                            ent = (Entity)tr.GetObject(per.ObjectId, OpenMode.ForRead);
                            tr.Commit();
                        }
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Select.cs: line: 599");
                    }
                    break;
            }
            return ent;
        }

        public static ObjectId
        selectEntity(string message, string reject, out bool escape)
        {
            escape = true;
            ObjectId idEnt = ObjectId.Null;
            BaseObjs.acadActivate();
            PromptEntityOptions PEOS = new PromptEntityOptions(message);
            PEOS.SetRejectMessage(reject);
            PEOS.AddAllowedClass(typeof(Line), true);
            PEOS.AddAllowedClass(typeof(Arc), true);
            PEOS.AddAllowedClass(typeof(Polyline), true);
            PEOS.AllowNone = false;

            PromptEntityResult PER = BaseObjs._editor.GetEntity(PEOS);

            if (PER.Status == PromptStatus.OK)
            {
                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        Entity ENT = (Entity)tr.GetObject(PER.ObjectId, OpenMode.ForRead);
                        idEnt = ENT.ObjectId;
                        escape = false;
                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " Select.cs: line: 635");
                }
            }
            return idEnt;
        }

        public static ObjectId
        selectMTextForMoving(string layer, out bool escape)
        {
            escape = true;
            ObjectId id = ObjectId.Null;
            TypedValue[] TVs = new TypedValue[2];

            TVs.SetValue(new TypedValue((int)DxfCode.Start, "MTEXT"), 0);
            TVs.SetValue(new TypedValue((int)DxfCode.LayerName, layer), 1);

            SelectionFilter filter = new SelectionFilter(TVs);

            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = "Select MText to Move";

            PromptSelectionResult PSR = BaseObjs._editor.GetSelection(pso, filter);

            if (PSR.Status == PromptStatus.OK)
            {
                id = PSR.Value.GetObjectIds()[0];
                escape = false;
            }
            return id;
        }
    }// Class Select
}// end TEI.BASE
