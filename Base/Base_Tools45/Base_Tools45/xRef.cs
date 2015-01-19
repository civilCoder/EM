using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using System;
using System.Collections.Generic;
using System.IO;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using DBObject = Autodesk.AutoCAD.DatabaseServices.DBObject;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static partial class xRef
    {
        public static bool
        checkIfLoaded(string name, out bool isXref)
        {
            bool isLoaded = false;
            isXref = false;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    SelectionSet ss = Base_Tools45.Select.buildSSet(typeof(BlockReference), true);
                    if (ss == null)
                    {
                        return false;
                    }

                    foreach (SelectedObject ssObj in ss)
                    {
                        BlockReference br = (BlockReference)ssObj.ObjectId.GetObject(OpenMode.ForRead);

                        if (br != null)
                        {
                            ObjectId idBtr = br.BlockTableRecord;
                            BlockTableRecord Btr = (BlockTableRecord)idBtr.GetObject(OpenMode.ForRead);
                            if (Btr != null)
                            {
                                if (Btr.IsFromExternalReference || Btr.IsFromOverlayReference)
                                {
                                    isXref = true;
                                    if (!Btr.IsUnloaded)
                                    {
                                        isLoaded = true;
                                    }
                                }
                            }
                        }
                    }

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef.cs: line: 63");
            }
            return isLoaded;
        }

        public static ObjectIdCollection
        copyXRefEnts(ObjectId idBlkRef, string nameLayer, string nameLayerTarget)
        {
            ObjectIdCollection idEnts = getXRefEntsByLayer(idBlkRef, nameLayer);
            ObjectIdCollection ids = new ObjectIdCollection();
            Layer.manageLayers(nameLayerTarget);
            Vector3d v3d = default(Vector3d);

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockReference br = (BlockReference)tr.GetObject(idBlkRef, OpenMode.ForRead);
                    foreach (ObjectId idEnt in idEnts)
                    {
                        Entity ent = (Entity)tr.GetObject(idEnt, OpenMode.ForWrite);
                        object o = ent.Clone();


                        Entity clone = (Entity)o;
                        if (clone != null)
                        {
                            clone.SetPropertiesFrom(br);
                            clone.Layer = nameLayerTarget;
                            BlockTableRecord space = (BlockTableRecord)BaseObjs._db.CurrentSpaceId.GetObject(OpenMode.ForWrite);
                            if (space == null)
                            {
                                clone.Dispose();
                                return null;
                            }
                            if(o is Arc){
                                Arc a = (Arc)o;
                                v3d = a.Normal;                            
                            }else if(o is Line){
                                Line l = (Line)o;
                                v3d = l.Normal;
                            }else if(o is Polyline){
                                Polyline p = (Polyline)o;
                                v3d = p.Normal;
                            }
                            // Translate the OCS to WCS                            
                            Matrix3d mWPlane = Matrix3d.WorldToPlane(v3d);
                            clone.TransformBy(mWPlane);

                            space.AppendEntity(clone);
                            tr.AddNewlyCreatedDBObject(clone, true);
                            ids.Add(clone.ObjectId);
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef.cs: line: 122");
            }
            return ids;
        }

        //public static ObjectIdCollection
        //copyXRefEnts(ObjectId idBlkRefSource, string nameLayer, string nameLayerTarget)
        //{
        //    ObjectIdCollection idEnts = getXRefEntsByLayer(idBlkRefSource, nameLayer);
        //    ObjectIdCollection ids = new ObjectIdCollection();
        //    Layer.manageLayers(nameLayerTarget);
        //    try
        //    {
        //        using (BaseObjs._acadDoc.LockDocument())
        //        {
        //            using (Transaction tr = BaseObjs.startTransactionDb())
        //            {
        //                BlockReference br = (BlockReference)tr.GetObject(idBlkRefSource, OpenMode.ForRead);
        //                Database dbSource = br.Database;

        //                BlockTable btTarget = (BlockTable)tr.GetObject(BaseObjs._db.BlockTableId, OpenMode.ForRead);
        //                BlockTableRecord msTarget = (BlockTableRecord)tr.GetObject(btTarget[BlockTableRecord.ModelSpace], OpenMode.ForRead);

        //                IdMapping idMapping = new IdMapping();

        //                dbSource.WblockCloneObjects(idEnts, msTarget.ObjectId, idMapping, DuplicateRecordCloning.Ignore, false);

        //                foreach(IdPair idp in idMapping){
        //                    ids.Add(idp.Value);
        //                }

        //                tr.Commit();
        //            }
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        BaseObjs.writeDebug(string.Format("{0} xRef.cs: line: 60", ex.Message));
        //    }
        //    return ids;
        //}


        public static ObjectIdCollection
        copyXRefEnts(ObjectId idBR, List<ObjectId> idEnts)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockReference br = (BlockReference)tr.GetObject(idBR, OpenMode.ForRead);
                    foreach (ObjectId id in idEnts)
                    {
                        Entity ent = id.getEnt();
                        object o = ent.Clone();
                        Entity clone = (Entity)o;
                        if (clone != null)
                        {
                            clone.SetPropertiesFrom(br);
                            BlockTableRecord space = (BlockTableRecord)BaseObjs._db.CurrentSpaceId.GetObject(OpenMode.ForWrite);
                            if (space == null)
                            {
                                clone.Dispose();
                                return null;
                            }
                            space.AppendEntity(clone);
                            tr.AddNewlyCreatedDBObject(clone, true);
                            ids.Add(clone.ObjectId);
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef.cs: line: 198");
            }
            return ids;
        }

        public static void doMXR()
        {
            string title = "Multiple XRef - MXR";
            string dir = Path.GetDirectoryName(BaseObjs.docFullName);
            string filter = "All Drawings (*.dwg)|*.dwg|Current Design Files1 (*.dwg)|????BNDY.dwg;????CGP.dwg;????CUP.dwg;????CNTL.dwg;????CONT.dwg;????DEMO.dwg;????GCAL.dwg;????SD.dwg;????TOPO.dwg;????T-SITE.dwg;????UTIL.dwg|Current Design Files2(*.dwg)|*BNDY.dwg;*CGP.dwg;*CUP.dwg;*CNTL.dwg;*CONT.dwg;*DEMO.dwg;*GCAL.dwg;*SD.dwg;*TOPO.dwg;*T-SITE.dwg;*UTIL.dwg|_JDH Files (*_JDH.dwg)|*_JDH.dwg";
            string defExt = ".dwg";

            string[] files = FileManager.getFiles(defExt, title, filter, dir);


            if (files == null || files.Length == 0)
                return;

            string jn = BaseObjs.jobNumber();

            object tMode = Application.GetSystemVariable("TILEMODE");
            if (tMode.ToString() != "1")
                BaseObjs._editor.Command("_tilemode", "1", "");

            BaseObjs._editor.Command("_ucs", "w", "");

            for (int i = 0; i < files.Length; i++)
            {

                string nameLayer = string.Format("_{0}", Path.GetFileNameWithoutExtension(files[i]));
                Layer.manageLayers(nameLayer);

                string nameXRef = nameLayer.Replace("_", "x");

                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ObjectId id = BaseObjs._db.AttachXref(files[i], nameXRef);

                    if (!id.IsNull)
                    {
                        using (BlockReference br = new BlockReference(Point3d.Origin, id))
                        {
                            BlockTable bt = (BlockTable)BaseObjs._db.BlockTableId.GetObject(OpenMode.ForRead);
                            BlockTableRecord ms = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                            ms.AppendEntity(br);
                            tr.AddNewlyCreatedDBObject(br, true);
                            br.Layer = nameLayer;
                        }
                    }
                    tr.Commit();
                }
            }
            Editor ed = BaseObjs._editor;
            ed.Command("_.LAYER", "C", "9", "*TOPO*", "");

        }


        /// <summary>
        ///
        /// </summary>
        public static void
        fixXrefs()
        {
            DBObject DbObj;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    SelectionSet SS = Base_Tools45.Select.buildSSet(typeof(BlockReference), true);
                    if (SS == null)
                        return;

                    foreach (SelectedObject ssObj in SS)
                    {
                        DbObj = ssObj.ObjectId.GetObject(OpenMode.ForRead);

                        if (DbObj is BlockReference)
                        {
                            BlockReference BR = (BlockReference)DbObj;
                            ObjectId id = BR.BlockTableRecord;
                            BlockTableRecord Btr = (BlockTableRecord)id.GetObject(OpenMode.ForRead);
                            string nameLayer;
                            if (Btr.XrefStatus == XrefStatus.Resolved)
                            {
                                if (Btr.Name.Substring(1, 1) != "x")
                                {
                                    nameLayer = string.Format("_{0}", Btr.Name);
                                    Btr.UpgradeOpen();
                                    Btr.Name = string.Format("x{0}", Btr.Name);
                                }
                                else
                                {
                                    nameLayer = String.Format("_{0}", Btr.Name.Substring(2));
                                }
                                if (BR.Layer != nameLayer)
                                {
                                    BR.UpgradeOpen();
                                    Layer.manageLayers(nameLayer);
                                    BR.Layer = nameLayer;
                                }
                            }
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef.cs: line: 307");
            }
        }

        public static ObjectId
        getEntity(string prompt, bool makeCopy, out string nameLayer, out Point3d pnt3dPick, out ObjectId idBlkRef)
        {
            ObjectId objId = ObjectId.Null;

            nameLayer = "";
            idBlkRef = ObjectId.Null;
            pnt3dPick = Pub.pnt3dO;
            Editor ed = BaseObjs._editor;
            PromptEntityOptions peo = new PromptEntityOptions(prompt);
            peo.AllowNone = true;
            PromptEntityResult per = ed.GetEntity(peo);
            if (per.Status != PromptStatus.OK)
                return objId;
            pnt3dPick = per.PickedPoint;

            objId = per.ObjectId;
            Entity entX = null;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    var ent0 = (Entity)objId.GetObject(OpenMode.ForRead);

                    if (ent0 is BlockReference)
                    {
                        var br = (BlockReference)objId.GetObject(OpenMode.ForRead);

                        if (br != null)
                        {
                            ObjectId idBtr = br.BlockTableRecord;
                            var btr = (BlockTableRecord)idBtr.GetObject(OpenMode.ForRead);
                            if (btr != null)
                            {
                                if (btr.IsFromExternalReference || btr.IsFromOverlayReference)
                                {
                                    entX = getNestedEntity(per, br, makeCopy, out nameLayer);
                                    objId = entX.ObjectId;
                                    idBlkRef = br.ObjectId;
                                }
                                else
                                {
                                    entX = (Entity)objId.GetObject(OpenMode.ForRead);
                                    objId = entX.ObjectId;
                                }
                            }
                        }
                    }
                    else
                    {
                        nameLayer = ent0.Layer;
                        entX = ent0;
                        objId = entX.ObjectId;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef.cs: line: 371");
            }
            return objId;
        }

        public static Entity
        getEntity(string prompt, out bool escape, out string xRefPath)
        {
            xRefPath = "";
            escape = true;
            Entity entX = null;
            Editor ED = BaseObjs._editor;
            PromptEntityOptions PEO = new PromptEntityOptions(prompt);
            PEO.AllowNone = true;
            PromptEntityResult PER = ED.GetEntity(PEO);

            switch (PER.Status)
            {
                case PromptStatus.OK:
                    escape = false;
                    break;

                case PromptStatus.None:
                    escape = false;
                    return entX;

                default:
                    return entX;
            }

            ObjectId objId = PER.ObjectId;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Entity ent0 = (Entity)objId.GetObject(OpenMode.ForRead);

                    if (ent0 is BlockReference)
                    {
                        BlockReference BR = (BlockReference)objId.GetObject(OpenMode.ForRead);

                        if (BR != null)
                        {
                            ObjectId idBtr = BR.BlockTableRecord;
                            BlockTableRecord Btr = (BlockTableRecord)idBtr.GetObject(OpenMode.ForRead);
                            if (Btr != null)
                            {
                                if (Btr.IsFromExternalReference || Btr.IsFromOverlayReference)
                                {
                                    entX = getNestedEntity(PER, BR);
                                    xRefPath = getXRefFileName(BR.ObjectId);
                                }
                                else
                                {
                                    entX = (Entity)objId.GetObject(OpenMode.ForRead);
                                }
                            }
                        }
                    }
                    else
                    {
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef.cs: line: 439");
            }
            return entX;
        }

        public static Entity
        getEntity(string prompt, out bool escape, out Object xRefPath, out PromptStatus ps)
        {
            xRefPath = null;
            escape = true;
            Entity entX = null;
            Editor ED = BaseObjs._editor;
            PromptEntityOptions PEO = new PromptEntityOptions(prompt);
            PEO.AllowNone = true;
            PromptEntityResult PER = ED.GetEntity(PEO);
            ps = PER.Status;
            switch (ps)
            {
                case PromptStatus.OK:
                    escape = false;
                    break;

                case PromptStatus.None:
                    escape = false;
                    return entX;

                default:
                    return entX;
            }

            ObjectId objId = PER.ObjectId;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Entity ent0 = (Entity)objId.GetObject(OpenMode.ForRead);

                    if (ent0 is BlockReference)
                    {
                        BlockReference BR = (BlockReference)objId.GetObject(OpenMode.ForRead);

                        if (BR != null)
                        {
                            ObjectId idBtr = BR.BlockTableRecord;
                            BlockTableRecord Btr = (BlockTableRecord)idBtr.GetObject(OpenMode.ForRead);
                            if (Btr != null)
                            {
                                if (Btr.IsFromExternalReference || Btr.IsFromOverlayReference)
                                {
                                    entX = getNestedEntity(PER, BR);
                                    xRefPath = getXRefFileName(BR.ObjectId);
                                }
                                else
                                {
                                    entX = (Entity)objId.GetObject(OpenMode.ForRead);
                                }
                            }
                        }
                    }
                    else
                    {
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef.cs: line: 507");
            }
            return entX;
        }

        public static Entity
        getNestedEntity(string prompt, out bool escape, out Object xRefPathX, out PromptStatus ps, out List<Point3d> pnts3dEntX, out FullSubentityPath path, out Point3d pnt3dPick, out bool isClosed)
        {
            xRefPathX = null;
            escape = true;
            pnt3dPick = Pub.pnt3dO;
            isClosed = false;
            Entity entX = null;
            Editor ED = BaseObjs._editor;
            PromptEntityOptions PEO = new PromptEntityOptions(prompt);
            PEO.AllowNone = true;
            PromptEntityResult PER = ED.GetEntity(PEO);
            ps = PER.Status;
            pnts3dEntX = new List<Point3d>();
            pnt3dPick = PER.PickedPoint;
            path = new FullSubentityPath();

            switch (ps)
            {
                case PromptStatus.OK:
                    escape = false;
                    break;

                case PromptStatus.None:
                    escape = false;
                    return entX;

                default:
                    return entX;
            }

            ObjectId objId = PER.ObjectId;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    isClosed = false;
                    Entity ent0 = (Entity)objId.GetObject(OpenMode.ForRead);
                    if (ent0 is BlockReference)
                    {
                        BlockReference BR = (BlockReference)objId.GetObject(OpenMode.ForRead);

                        if (BR != null)
                        {
                            ObjectId idBtr = BR.BlockTableRecord;
                            BlockTableRecord Btr = (BlockTableRecord)idBtr.GetObject(OpenMode.ForRead);
                            if (Btr != null)
                            {
                                if (Btr.IsFromExternalReference || Btr.IsFromOverlayReference)
                                {
                                    entX = getNestedEntityAndHighlight(PER.PickedPoint, BR, out path, out isClosed);
                                    xRefPathX = getXRefFileName(BR.ObjectId);
                                }
                                else
                                {
                                    entX = (Entity)objId.GetObject(OpenMode.ForRead);
                                }
                                if (entX is Line)
                                {
                                    Line line = (Line)entX;
                                    pnts3dEntX.Add(line.StartPoint);
                                    pnts3dEntX.Add(line.EndPoint);
                                }
                                else if (entX is Polyline)
                                {
                                    Polyline poly = (Polyline)entX;
                                    pnts3dEntX = Conv.polyX_listPnts3d(entX.ObjectId);
                                    isClosed = poly.Closed;
                                }
                                else if (entX is Polyline3d)
                                {
                                    Polyline3d poly3d = (Polyline3d)entX;
                                    pnts3dEntX = Conv.polyX_listPnts3d(entX.ObjectId);
                                    isClosed = poly3d.Closed;
                                }
                            }
                        }
                    }
                    else
                    {
                        Application.ShowAlertDialog("Selected Entity was not from a xref.  Exiting...");
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef.cs: line: 600");
            }
            return entX;
        }

        public static Entity
        getNestedEntity(PromptEntityResult per, BlockReference br)
        {
            Entity ent = null;
            Editor ed = BaseObjs._editor;
            PromptNestedEntityOptions pneo = new PromptNestedEntityOptions("");

            pneo.NonInteractivePickPoint = per.PickedPoint;
            pneo.UseNonInteractivePickPoint = true;

            PromptNestedEntityResult pner = ed.GetNestedEntity(pneo);

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    if (pner.Status == PromptStatus.OK)
                    {
                        ObjectId idSel = pner.ObjectId;
                        DBObject obj = idSel.GetObject(OpenMode.ForRead);

                        if (obj is PolylineVertex3d || obj is Vertex2d)
                            idSel = obj.OwnerId;

                        if (obj is MText || obj is DBText)
                            return null;

                        ent = (Entity)obj;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef.cs: line: 639");
            }
            return ent;
        }

        public static Entity
        getNestedEntity(PromptEntityResult per, BlockReference br, bool makeCopy, out string nameLayer)
        {
            nameLayer = "";
            Entity ent = null;
            Editor ed = BaseObjs._editor;
            Database db = BaseObjs._db;
            PromptNestedEntityOptions pneo = new PromptNestedEntityOptions("");
            pneo.NonInteractivePickPoint = per.PickedPoint;
            pneo.UseNonInteractivePickPoint = true;

            PromptNestedEntityResult pner = ed.GetNestedEntity(pneo);

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    if (pner.Status == PromptStatus.OK)
                    {
                        ObjectId idSel = pner.ObjectId;
                        DBObject OBJ = idSel.GetObject(OpenMode.ForRead);

                        if (OBJ is PolylineVertex3d || OBJ is Vertex2d)
                            idSel = OBJ.OwnerId;

                        if (OBJ is MText || OBJ is DBText || !(OBJ is Entity))
                            return null;

                        ent = (Entity)OBJ;
                        if (makeCopy == true)
                        {
                            object o = ent.Clone();
                            Entity clone = o as Entity;

                            if (clone != null)
                            {
                                clone.SetPropertiesFrom(br);

                                ObjectId[] conts = pner.GetContainers();
                                foreach (ObjectId idCont in conts)
                                {
                                    BlockReference cont = (BlockReference)idCont.GetObject(OpenMode.ForRead);
                                    if (cont != null)
                                        clone.TransformBy(cont.BlockTransform);
                                }

                                BlockTableRecord space = (BlockTableRecord)db.CurrentSpaceId.GetObject(OpenMode.ForWrite);
                                if (space == null)
                                {
                                    clone.Dispose();

                                    nameLayer = ent.Layer;
                                    return null;
                                }
                                space.AppendEntity(clone);
                                tr.AddNewlyCreatedDBObject(clone, true);
                                tr.Commit();

                                nameLayer = ent.Layer.ToString();
                                return clone;
                            }
                        }
                        else
                        {
                            return ent;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef.cs: line: 715");
            }
            return ent;
        }

        public static Entity
        getNestedEntity(string prompt, out bool escape, out Point3d pnt3dX, out string nameLayer, out FullSubentityPath path)
        {
            nameLayer = "";
            Entity ent = null;
            Database db = BaseObjs._db;
            path = new FullSubentityPath();

            Editor ed = BaseObjs._editor;

            PromptNestedEntityOptions pneo = new PromptNestedEntityOptions(prompt);
            PromptNestedEntityResult pner = ed.GetNestedEntity(pneo);

            if (pner.Status == PromptStatus.Cancel || pner.Status == PromptStatus.None)
            {
                pnt3dX = Pub.pnt3dO;
                escape = true;
                return null;
            }

            pnt3dX = pner.PickedPoint;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    if (pner.Status == PromptStatus.OK)
                    {
                        escape = false;
                        ObjectId[] idsContainers = pner.GetContainers();
                        ObjectId idSel = pner.ObjectId;
                        int len = idsContainers.Length;

                        ObjectId[] idsRev = new ObjectId[len + 1];
                        //Reverse the "containers" list
                        for (int i = 0; i < len; i++)
                        {
                            ObjectId id = (ObjectId)idsContainers.GetValue(len - 1 - i);
                            idsRev.SetValue(id, i);
                        }
                        //Now add the selected entity to the end of the array
                        idsRev.SetValue(idSel, len);

                        //Retrieve the sub-entity path for this entity
                        SubentityId idSubEnt = new SubentityId(SubentityType.Null, (IntPtr)0);
                        path = new FullSubentityPath(idsRev, idSubEnt);

                        ObjectId idX = (ObjectId)idsRev.GetValue(0);
                        ent = (Entity)tr.GetObject(idX, OpenMode.ForRead);

                        ent.Highlight(path, false);

                        DBObject obj = idSel.GetObject(OpenMode.ForRead);

                        ObjectId idOwner = ObjectId.Null;

                        if (obj is PolylineVertex3d || obj is Vertex2d)
                        {
                            idOwner = obj.OwnerId;
                            ent = (Entity)tr.GetObject(idOwner, OpenMode.ForRead);
                        }
                        else if (obj is MText || obj is DBText || !(obj is Entity))
                            return ent;
                        else
                            ent = (Entity)obj;

                        object o = ent.Clone();
                        Entity clone = (Entity)o;

                        if (clone != null)
                        {
                            //ObjectId[] conts = pner.GetContainers();
                            foreach (ObjectId idContainer in idsContainers)
                            {
                                BlockReference br = (BlockReference)idContainer.GetObject(OpenMode.ForRead);
                                if (br != null)
                                {
                                    clone.SetPropertiesFrom(br);
                                    clone.TransformBy(br.BlockTransform);
                                }
                            }

                            BlockTableRecord space = (BlockTableRecord)db.CurrentSpaceId.GetObject(OpenMode.ForWrite);
                            if (space == null)
                            {
                                clone.Dispose();
                                return ent;
                            }
                            else if (clone is Arc)
                            {
                                Arc arc = (Arc)clone;
                                pnt3dX = arc.GetClosestPointTo(pnt3dX, false);
                            }
                            else if (clone is Line)
                            {
                                Line line = (Line)clone;
                                pnt3dX = line.GetClosestPointTo(pnt3dX, false);
                            }
                            else if (clone is Polyline)
                            {
                                Polyline poly = (Polyline)clone;
                                pnt3dX = poly.GetClosestPointTo(pnt3dX, false);
                            }
                            else
                            {
                                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(string.Format("Selected item was of type: {0}.  Exiting...", clone.GetType().Name));
                            }
                            nameLayer = ent.Layer.ToString();

                            tr.Commit();

                            return ent;
                        }
                    }
                    else
                    {
                        escape = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef.cs: line: 842");
                escape = true;
            }
            return ent;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static BlockReference
        getXRefBlockReference(string name)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    SelectionSet SS = Base_Tools45.Select.buildSSet(typeof(BlockReference), true);
                    if (SS == null)
                        return null;

                    foreach (SelectedObject ssObj in SS)
                    {
                        BlockReference BR = (BlockReference)ssObj.ObjectId.GetObject(OpenMode.ForRead);

                        if (BR != null)
                        {
                            ObjectId idBtr = BR.BlockTableRecord;
                            BlockTableRecord Btr = (BlockTableRecord)idBtr.GetObject(OpenMode.ForRead);
                            if (Btr != null)
                            {
                                if (Btr.IsFromExternalReference || Btr.IsFromOverlayReference)
                                {
                                    if (Btr.PathName == name || Btr.Name.ToUpper().Contains(name))
                                        return BR;
                                }
                            }
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef.cs: line: 887");
            }
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static ObjectIdCollection
        getXRefBlockReferences()
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    SelectionSet SS = Base_Tools45.Select.buildSSet(typeof(BlockReference), true);
                    if (SS == null)
                        return null;

                    foreach (SelectedObject ssObj in SS)
                    {
                        DBObject dbObj = (DBObject)tr.GetObject(ssObj.ObjectId, OpenMode.ForRead);
                        if (dbObj is BlockReference)
                        {
                            BlockReference br = (BlockReference)dbObj;
                            if (br != null)
                            {
                                ObjectId idBtr = br.BlockTableRecord;
                                BlockTableRecord Btr = (BlockTableRecord)idBtr.GetObject(OpenMode.ForRead);
                                if (Btr != null)
                                {
                                    if (Btr.IsFromExternalReference || Btr.IsFromOverlayReference)
                                    {
                                        ids.Add(br.ObjectId);
                                    }
                                }
                            }
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef.cs: line: 933");
            }
            return ids;
        }

        public static ObjectIdCollection
        getXRefBlockTableRecords()
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb()){

                    ObjectIdCollection idsBR = getXRefBlockReferences();
                    foreach(ObjectId id in idsBR){
                        BlockReference br = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                        ids.Add(br.BlockTableRecord);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef.cs: line: 956");
            }
            return ids;
        }

        public static BlockTableRecord
        getXRefBlockTableRecordMS(string nameDwg)
        {
            BlockTableRecord ms;
            BlockReference br = getXRefBlockReference(nameDwg);
            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                BlockTable bt = (BlockTable)br.Database.BlockTableId.GetObject(OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)bt[BlockTableRecord.ModelSpace].GetObject(OpenMode.ForRead);
                ms = btr;
            }
            return ms;
        }

        public static Database
        getXRefDatabase(string name)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    SelectionSet SS = Base_Tools45.Select.buildSSet(typeof(BlockReference), true);
                    if (SS == null)
                        return null;

                    foreach (SelectedObject ssObj in SS)
                    {
                        BlockReference BR = (BlockReference)ssObj.ObjectId.GetObject(OpenMode.ForRead);

                        if (BR != null)
                        {
                            ObjectId idBtr = BR.BlockTableRecord;
                            BlockTableRecord Btr = (BlockTableRecord)idBtr.GetObject(OpenMode.ForRead);
                            if (Btr != null)
                            {
                                if (Btr.IsFromExternalReference || Btr.IsFromOverlayReference)
                                {
                                    var db = Btr.GetXrefDatabase(false);
                                    if (db.Filename == name)
                                        return db;
                                    //if (Btr.PathName == name || Btr.Name.Contains(name))
                                    //    return Btr;
                                }
                            }
                        }
                    }

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef.cs: line: 1013");
            }
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idBlkRef"></param>
        /// <param name="nameLayer"></param>
        /// <returns></returns>
        public static ObjectIdCollection
        getXRefEntsByLayer(ObjectId idBlkRef, string nameLayer)
        {
            ObjectIdCollection idEnts = new ObjectIdCollection();

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockReference br = (BlockReference)tr.GetObject(idBlkRef, OpenMode.ForRead);
                    ObjectId idbtr = br.BlockTableRecord;
                    BlockTableRecord btr = (BlockTableRecord)idbtr.GetObject(OpenMode.ForRead);
                    foreach (ObjectId idObj in btr)
                    {
                        Entity ent = (Entity)tr.GetObject(idObj, OpenMode.ForRead);
                        if (ent.Layer.Contains(nameLayer))
                        {
                            idEnts.Add(idObj);                                
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef.cs: line: 1049");
            }
            return idEnts;
        }

        /// <summary>
        /// Full Path & Filename
        /// </summary>
        /// <param name="id"></param>
        /// <returns>String</returns>
        public static string
        getXRefFileName(ObjectId id)
        {
            ObjectId idbtr = ObjectId.Null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockReference br = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                    idbtr = br.BlockTableRecord;
                    BlockTableRecord btr = (BlockTableRecord)idbtr.GetObject(OpenMode.ForRead);
                    tr.Commit();
                    return btr.PathName;
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef.cs: line: 1076");
            }
            return string.Empty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static BlockTableRecord
        getXRefMSpace(string name)
        {
            BlockTableRecord MS = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockReference BR = getXRefBlockReference(name);
                    BlockTable BT = (BlockTable)BR.Database.BlockTableId.GetObject(OpenMode.ForRead);
                    MS = (BlockTableRecord)BT[BlockTableRecord.ModelSpace].GetObject(OpenMode.ForRead);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef.cs: line: 1102");
            }
            return MS;
        }

        public static ResultBuffer
        getXRefsContainingTargetDwgName(string nameTarget)
        {
            ObjectIdCollection ids = getXRefBlockReferences();
            if (ids == null)
                return null;

            ResultBuffer rb = new ResultBuffer();
            foreach (ObjectId id in ids)
            {
                string nameFile = getXRefFileName(id);
                if (nameFile.Contains(nameTarget))
                {
                    if (File.Exists(nameFile))
                    {
                        FileInfo fi = new FileInfo(nameFile);
                        DateTime lastWrite = fi.LastWriteTime;
                        rb.Add(new TypedValue(1001, nameFile));
                        rb.Add(new TypedValue(1000, lastWrite.ToString()));
                    }
                }
            }
            return rb;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nameTarget"></param>
        /// <returns></returns>
        public static ObjectIdCollection
        getXRefsContainingTargetLayer(string nameTarget)
        {
            ObjectIdCollection ids = getXRefBlockReferences();
            if (ids == null)
                return null;

            ObjectIdCollection idsTarget = new ObjectIdCollection();
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    foreach (ObjectId id in ids)
                    {
                        BlockReference br = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                        ObjectId idbtr = br.BlockTableRecord;
                        BlockTableRecord btr = (BlockTableRecord)idbtr.GetObject(OpenMode.ForRead);
                        foreach (ObjectId idEnt in btr)
                        {
                            Entity ent = (Entity)tr.GetObject(idEnt, OpenMode.ForRead);
                            string nameLayer = ent.Layer;
                            int index = nameLayer.IndexOf("|");
                            nameLayer = nameLayer.Substring(index + 1);
                            if (nameLayer.Contains(nameTarget))
                            {
                                idsTarget.Add(id);
                                break;
                            }
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef.cs: line: 1172");
            }
            return idsTarget;
        }

        public static TinSurface
        getXRefTinSurface(ObjectId idbr, string nameSurf)
        {
            TinSurface s = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockReference br = (BlockReference)tr.GetObject(idbr, OpenMode.ForRead);
                    ObjectId idbtr = br.BlockTableRecord;
                    BlockTableRecord btr = (BlockTableRecord)idbtr.GetObject(OpenMode.ForRead);
                    foreach (ObjectId idObj in btr)
                    {
                        Entity ent = (Entity)tr.GetObject(idObj, OpenMode.ForRead);
                        if (ent is TinSurface)
                        {
                            s = (TinSurface)ent;
                            if (s.Name == nameSurf)
                                break;
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef.cs: line: 1203");
            }
            return s;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="Btr"></param>
        /// <returns></returns>
        public static void
        unHighlightNestedEntity(List<FullSubentityPath> paths)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    for (int i = 0; i < paths.Count; i++)
                    {
                        ObjectId[] ids = paths[i].GetObjectIds();
                        Entity ent = (Entity)ids[0].GetObject(OpenMode.ForRead);
                        if (ent != null)
                            ent.Unhighlight(paths[i], false);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef.cs: line: 1232");
            }
        }
    }
}
