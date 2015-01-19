using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45.Jig;
using System;
using System.Collections.Generic;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

//using Autodesk.Civil.DatabaseServices;
namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static class Blocks
    {
        public static void
        addAttributesToBlock(string strName)
        {
            Database DB = BaseObjs._db;
            TextStyleTableRecord tstr = Base_Tools45.Txt.getTextStyleTableRecord("ANNO");

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTable BT = (BlockTable)DB.BlockTableId.GetObject(OpenMode.ForWrite);
                    BlockTableRecord Btr = (BlockTableRecord)BT[strName].GetObject(OpenMode.ForWrite);

                    Point3d pntTop = new Point3d(0, 0.01, 0);
                    Point3d pntMid = new Point3d(0, -0.10, 0);
                    Point3d pntBot = new Point3d(0, -0.20, 0);

                    var AD = new AttributeDefinition();
                    AD.SetDatabaseDefaults();
                    AD.Tag = "TOPTXT";
                    AD.Prompt = "\nEnter Grade and Desc";
                    AD.Position = pntTop;

                    //AD.AlignmentPoint = pntTop;
                    AD.Justify = AttachmentPoint.BottomCenter;

                    AD.TextStyleId = tstr.ObjectId;
                    AD.Layer = "0";

                    Btr.AppendEntity(AD);
                    tr.AddNewlyCreatedDBObject(AD, true);

                    AD = new AttributeDefinition();
                    AD.SetDatabaseDefaults();
                    AD.Tag = "MIDTXT";
                    AD.Prompt = "\nEnter Grade and Desc";
                    AD.Position = pntMid;

                    //AD.AlignmentPoint = pntMid;
                    AD.Justify = AttachmentPoint.TopCenter;

                    AD.TextStyleId = tstr.ObjectId;
                    AD.Layer = "0";

                    Btr.AppendEntity(AD);
                    tr.AddNewlyCreatedDBObject(AD, true);

                    AD = new AttributeDefinition();
                    AD.SetDatabaseDefaults();
                    AD.Tag = "BOTTXT";
                    AD.Prompt = "\nEnter Grade and Desc";
                    AD.Position = pntBot;

                    // AD.AlignmentPoint = pntBtm;
                    AD.Justify = AttachmentPoint.TopCenter;

                    AD.TextStyleId = tstr.ObjectId;
                    AD.Layer = "0";

                    Btr.AppendEntity(AD);
                    tr.AddNewlyCreatedDBObject(AD, true);

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Blocks.cs: line: 85");
            }
        }

        public static BlockReference
        addBlockRef(string strName, Point3d pnt3dIns, double dblRotation, List<string> attValues)
        {
            Database db = BaseObjs._db;
            Editor ed = BaseObjs._editor;

            BlockTableRecord btrx = null;
            BlockReference br = null;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTable bt = (BlockTable)db.BlockTableId.GetObject(OpenMode.ForRead);
                    if (!bt.Has(strName))
                    {
                        btrx = addBtr(strName);
                    }
                    else
                        btrx = (BlockTableRecord)bt[strName].GetObject(OpenMode.ForRead);

                    //---> debug only
                    foreach (ObjectId idObj in btrx)
                    {
                        Entity ent = (Entity)idObj.GetObject(OpenMode.ForRead);
                        AttributeDefinition ad = ent as AttributeDefinition;

                        if (ad != null)
                        {
                            ed.WriteMessage(string.Format("\n{0}", ad.Tag));
                        }
                    }//<--- debug only

                    //BlockTableRecord Btr = (BlockTableRecord)DB.CurrentSpaceId.GetObject(OpenMode.ForWrite);

                    btrx.UpgradeOpen();

                    using (btrx)
                    {
                        br = new BlockReference(pnt3dIns, btrx.ObjectId);
                        using (br)
                        {
                            Matrix3d ucsMatrix = ed.CurrentUserCoordinateSystem;
                            CoordinateSystem3d ucs = ucsMatrix.CoordinateSystem3d;

                            Matrix3d mat3d = new Matrix3d();
                            mat3d = Matrix3d.Rotation(dblRotation, ucs.Zaxis, pnt3dIns);

                            br.TransformBy(mat3d);
                            br.ScaleFactors = new Scale3d(1, 1, 1);
                            btrx.AppendEntity(br);
                            tr.AddNewlyCreatedDBObject(br, true);

                            BlockTableRecord btratt = (BlockTableRecord)br.BlockTableRecord.GetObject(OpenMode.ForRead);
                            using (btratt)
                            {
                                Autodesk.AutoCAD.DatabaseServices.AttributeCollection ATTcol = br.AttributeCollection;

                                foreach (ObjectId subid in btratt)
                                {
                                    Entity ent = (Entity)subid.GetObject(OpenMode.ForRead);
                                    AttributeDefinition ad = ent as AttributeDefinition;

                                    if (ad != null)
                                    {
                                        AttributeReference ar = new AttributeReference();
                                        ar.SetPropertiesFrom(ad);
                                        ar.SetAttributeFromBlock(ad, br.BlockTransform);
                                        ar.Visible = ad.Visible;

                                        ar.HorizontalMode = ad.HorizontalMode;
                                        ar.VerticalMode = ad.VerticalMode;
                                        ar.Rotation = ad.Rotation;
                                        ar.TextStyleId = ad.TextStyleId;
                                        ar.Position = ad.Position + pnt3dIns.GetAsVector();
                                        ar.Tag = ad.Tag;
                                        ar.FieldLength = ad.FieldLength;
                                        ar.AdjustAlignment(db);

                                        //if (ar.Tag == "TOPTXT")
                                        //    ar.TextString = strTop;

                                        //if (ar.Tag == "MIDTXT")
                                        //    ar.TextString = strMid;

                                        //if (ar.Tag == "BOTTXT")
                                        //    ar.TextString = strBot;

                                        ar.Position = ad.Position.TransformBy(br.BlockTransform);

                                        ATTcol.AppendAttribute(ar);
                                        tr.AddNewlyCreatedDBObject(ar, true);
                                    }
                                }
                            }
                            br.DowngradeOpen();
                        }

                        btrx.DowngradeOpen();
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Blocks.cs: line: 194");
            }
            return br;
        }

        public static BlockReference
        addBlockRef(string strName, string strTop, string strMid, string strBot, Point3d pnt3dIns, double dblRotation)
        {
            Database DB = BaseObjs._db;
            Editor ED = BaseObjs._editor;

            BlockTableRecord Btrx = null;
            BlockReference BR = null;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTable BT = (BlockTable)DB.BlockTableId.GetObject(OpenMode.ForRead);
                    if (!BT.Has(strName))
                    {
                        Btrx = addBtr(strName);
                        Blocks.addAttributesToBlock(strName);
                    }
                    else
                        Btrx = (BlockTableRecord)BT[strName].GetObject(OpenMode.ForRead);

                    //---> debug only
                    foreach (ObjectId objID in Btrx)
                    {
                        Entity ENT = (Entity)objID.GetObject(OpenMode.ForRead);
                        AttributeDefinition AD = ENT as AttributeDefinition;

                        if (AD != null)
                        {
                            ED.WriteMessage(string.Format("\n{0}", AD.Tag));
                        }
                    }//<--- debug only

                    //BlockTableRecord Btr = (BlockTableRecord)DB.CurrentSpaceId.GetObject(OpenMode.ForWrite);

                    Btrx.UpgradeOpen();

                    using (Btrx)
                    {
                        BR = new BlockReference(pnt3dIns, Btrx.ObjectId);
                        using (BR)
                        {
                            Matrix3d UCSMatrix = ED.CurrentUserCoordinateSystem;
                            CoordinateSystem3d UCS = UCSMatrix.CoordinateSystem3d;

                            Matrix3d MAT3d = new Matrix3d();
                            MAT3d = Matrix3d.Rotation(dblRotation, UCS.Zaxis, pnt3dIns);

                            BR.TransformBy(MAT3d);
                            BR.ScaleFactors = new Scale3d(1, 1, 1);
                            Btrx.AppendEntity(BR);
                            tr.AddNewlyCreatedDBObject(BR, true);

                            BlockTableRecord Btratt = (BlockTableRecord)BR.BlockTableRecord.GetObject(OpenMode.ForRead);
                            using (Btratt)
                            {
                                Autodesk.AutoCAD.DatabaseServices.AttributeCollection ATTcol = BR.AttributeCollection;

                                foreach (ObjectId subID in Btratt)
                                {
                                    Entity ENT = (Entity)subID.GetObject(OpenMode.ForRead);
                                    AttributeDefinition AD = ENT as AttributeDefinition;

                                    if (AD != null)
                                    {
                                        AttributeReference AR = new AttributeReference();
                                        AR.SetPropertiesFrom(AD);
                                        AR.SetAttributeFromBlock(AD, BR.BlockTransform);
                                        AR.Visible = AD.Visible;

                                        AR.HorizontalMode = AD.HorizontalMode;
                                        AR.VerticalMode = AD.VerticalMode;
                                        AR.Rotation = AD.Rotation;
                                        AR.TextStyleId = AD.TextStyleId;
                                        AR.Position = AD.Position + pnt3dIns.GetAsVector();
                                        AR.Tag = AD.Tag;
                                        AR.FieldLength = AD.FieldLength;
                                        AR.AdjustAlignment(DB);

                                        if (AR.Tag == "TOPTXT")
                                            AR.TextString = strTop;

                                        if (AR.Tag == "MIDTXT")
                                            AR.TextString = strMid;

                                        if (AR.Tag == "BOTTXT")
                                            AR.TextString = strBot;

                                        AR.Position = AD.Position.TransformBy(BR.BlockTransform);

                                        ATTcol.AppendAttribute(AR);
                                        tr.AddNewlyCreatedDBObject(AR, true);
                                    }
                                }
                            }
                            BR.DowngradeOpen();
                        }

                        Btrx.DowngradeOpen();
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Blocks.cs: line: 305");
            }
            return BR;
        }

        public static Boolean
        addBlockRef(string strName, string strTopNum, string strTopTxt, string strBotNum, string strBotTxt, Point3d pnt3dIns, double dblRotation)
        {
            Database DB = BaseObjs._db;
            Editor ED = BaseObjs._editor;

            ObjectId blkID = ObjectId.Null;
            BlockTableRecord Btrx = null;

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                BlockTable BT = (BlockTable)DB.BlockTableId.GetObject(OpenMode.ForRead);
                if (!BT.Has(strName))
                {
                    blkID = insBlockRef(@"R:\Tset\Block\", "GradeTag.dwg");
                    Btrx = (BlockTableRecord)blkID.GetObject(OpenMode.ForRead);
                    Btrx.UpgradeOpen();
                    Btrx.Name = "GradeTag";
                    Btrx.DowngradeOpen();
                }
                else
                {
                    Btrx = (BlockTableRecord)BT[strName].GetObject(OpenMode.ForRead);
                }

                //---> debug only
                foreach (ObjectId objID in Btrx)
                {
                    Entity ENT = (Entity)objID.GetObject(OpenMode.ForRead);
                    AttributeDefinition AD = ENT as AttributeDefinition;

                    if (AD != null)
                    {
                        ED.WriteMessage(string.Format("\n{0}", AD.Tag));
                    }
                }//<--- debug only

                BlockTableRecord Btr = (BlockTableRecord)DB.CurrentSpaceId.GetObject(OpenMode.ForWrite);

                using (Btr)
                {
                    BlockReference BR = new BlockReference(pnt3dIns, Btrx.ObjectId);
                    using (BR)
                    {
                        Matrix3d UCSMatrix = ED.CurrentUserCoordinateSystem;
                        CoordinateSystem3d UCS = UCSMatrix.CoordinateSystem3d;

                        Matrix3d MAT3d = new Matrix3d();
                        MAT3d = Matrix3d.Rotation(dblRotation, UCS.Zaxis, pnt3dIns);

                        BR.TransformBy(MAT3d);
                        BR.ScaleFactors = new Scale3d(1, 1, 1);
                        Btr.AppendEntity(BR);
                        tr.AddNewlyCreatedDBObject(BR, true);

                        BlockTableRecord Btratt = (BlockTableRecord)BR.BlockTableRecord.GetObject(OpenMode.ForRead);
                        using (Btratt)
                        {
                            Autodesk.AutoCAD.DatabaseServices.AttributeCollection ATTcol = BR.AttributeCollection;

                            foreach (ObjectId subID in Btratt)
                            {
                                Entity ENT = (Entity)subID.GetObject(OpenMode.ForRead);
                                AttributeDefinition AD = ENT as AttributeDefinition;

                                if (AD != null)
                                {
                                    AttributeReference AR = new AttributeReference();
                                    AR.SetPropertiesFrom(AD);
                                    AR.SetAttributeFromBlock(AD, BR.BlockTransform);
                                    AR.Visible = AD.Visible;

                                    AR.HorizontalMode = AD.HorizontalMode;
                                    AR.VerticalMode = AD.VerticalMode;
                                    AR.Rotation = AD.Rotation;
                                    AR.TextStyleId = AD.TextStyleId;
                                    AR.Position = AD.Position + pnt3dIns.GetAsVector();
                                    AR.Tag = AD.Tag;
                                    AR.FieldLength = AD.FieldLength;
                                    AR.AdjustAlignment(DB);

                                    if (AR.Tag == "TOPNUM")
                                        AR.TextString = strTopNum;

                                    if (AR.Tag == "TOPTXT")
                                        AR.TextString = strTopTxt;

                                    if (AR.Tag == "BOTNUM")
                                        AR.TextString = strBotNum;

                                    if (AR.Tag == "BOTTXT")
                                        AR.TextString = strBotTxt;

                                    AR.Position = AD.Position.TransformBy(BR.BlockTransform);

                                    ATTcol.AppendAttribute(AR);
                                    tr.AddNewlyCreatedDBObject(AR, true);
                                }// end if
                            }//end foreach
                        }
                        BR.DowngradeOpen();
                    }

                    Btr.DowngradeOpen();
                }

                // BT.DowngradeOpen ();
                tr.Commit();
            }
            return true;
        }

        public static ObjectId
        addBlockRefPolyLdr(List<Vertex2d> vtxs2d, string nameLayer = "0", short color = 256)
        {
            ObjectId id = ObjectId.Null;
            Database db = BaseObjs._db;
            Layer.manageLayers(nameLayer);
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Polyline poly = new Polyline();
                    int i = -1;
                    poly.SetDatabaseDefaults();
                    foreach (Vertex2d vtx2d in vtxs2d)
                    {
                        i = ++i;
                        Point2d pnt2d = new Point2d(vtx2d.Position.X, vtx2d.Position.Y);
                        poly.AddVertexAt(i, pnt2d, vtx2d.Bulge, vtx2d.StartWidth, vtx2d.EndWidth);
                    }
                    poly.Layer = nameLayer;
                    poly.Color = Color.FromColorIndex(ColorMethod.ByBlock, color);

                    BlockTable bt = (BlockTable)db.BlockTableId.GetObject(OpenMode.ForWrite);
                    BlockTableRecord btr = new BlockTableRecord();
                    btr.Name = "*U";
                    btr.Explodable = false;
                    btr.Origin = poly.StartPoint;
                    btr.AppendEntity(poly);
                    btr.Annotative = AnnotativeStates.True;
                    ObjectId idbtr = bt.Add(btr);
                    tr.AddNewlyCreatedDBObject(btr, true);

                    BlockTableRecord ms = (BlockTableRecord)bt[BlockTableRecord.ModelSpace].GetObject(OpenMode.ForWrite);
                    BlockReference br = new BlockReference(poly.StartPoint, idbtr);
                    //BlockTableRecord btrPoly = (BlockTableRecord)tr.GetObject(br.ObjectId, OpenMode.ForWrite);
                    //btrPoly.Name = "*U";
                    //btrPoly.Annotative = AnnotativeStates.True;

                    id = ms.AppendEntity(br);
                    tr.AddNewlyCreatedDBObject(br, true);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Blocks.cs: line: 467");
            }
            return id;
        }

        public static BlockTableRecord
        addBtr(string strName)
        {
            Database DB = BaseObjs._db;
            BlockTableRecord Btr = null;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTable BT = (BlockTable)DB.BlockTableId.GetObject(OpenMode.ForWrite);

                    Btr = new BlockTableRecord();

                    Btr.Name = strName;
                    Btr.Origin = Pub.pnt3dO;

                    BT.Add(Btr);
                    tr.AddNewlyCreatedDBObject(Btr, true);

                    tr.Commit();
                }// end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Blocks.cs: line: 497");
            }
            return Btr;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nameBlock"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ObjectId
        cloneBlock(string nameBlock, Database source, Database target)
        {
            ObjectId id = ObjectId.Null;
            try
            {
                using (Transaction trTAR = BaseObjs.startTransactionDb())
                {
                    BlockTable btTarget = (BlockTable)target.BlockTableId.GetObject(OpenMode.ForWrite);
                    if (btTarget.Has(nameBlock))
                        return ObjectId.Null;

                    using (Transaction trSource = BaseObjs.startTransactionDb())
                    {
                        BlockTable btSource = (BlockTable)source.BlockTableId.GetObject(OpenMode.ForRead);
                        if (btSource.Has(nameBlock) == false)
                            return id;

                        BlockTableRecord btrSource = (BlockTableRecord)btSource[nameBlock].GetObject(OpenMode.ForRead);
                        IdMapping idMap = new IdMapping();
                        ObjectIdCollection idsSource = new ObjectIdCollection();
                        idsSource.Add(btrSource.ObjectId);
                        target.WblockCloneObjects(idsSource, btTarget.ObjectId, idMap, Autodesk.AutoCAD.DatabaseServices.DuplicateRecordCloning.Replace, false);
                        trSource.Commit();
                        trTAR.Commit();
                        id = idMap[btrSource.ObjectId].Value;
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Blocks.cs: line: 540");
            }
            return id;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="nameLayer"></param>
        /// <returns></returns>
        public static ObjectId
        copy(Entity ent, string nameLayer)
        {
            ObjectId idEnt = ObjectId.Null;

            Entity entCopy = null;
            Database db = BaseObjs._db;
            ObjectIdCollection ids = new ObjectIdCollection();
            ids.Add(ent.ObjectId);
            Layer.manageLayers(nameLayer);
            try
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        IdMapping idMap = new IdMapping();
                        db.DeepCloneObjects(ids, db.CurrentSpaceId, idMap, false);
                        foreach (IdPair idP in idMap)
                        {
                            ObjectId id = idP.Value;
                            entCopy = (Entity)id.GetObject(OpenMode.ForWrite);
                            entCopy.Layer = nameLayer;
                        }
                        db.Dispose();
                        tr.Commit();
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Blocks.cs: line: 588");
            }
            if (entCopy is PolylineVertex3d || entCopy is Vertex2d)
                idEnt = entCopy.OwnerId;

            return entCopy.ObjectId;
        }

        public static void
        deepClone(ObjectIdCollection ids, Point3d pnt3dBase)
        {
            Entity dbObj = null;
            Database db = BaseObjs._db;
            ObjectIdCollection idsNew = new ObjectIdCollection();

            try
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    try
                    {
                        using (Transaction tr = BaseObjs.startTransactionDb())
                        {
                            IdMapping idMap = new IdMapping();
                            db.DeepCloneObjects(ids, db.CurrentSpaceId, idMap, false);
                            foreach (IdPair idP in idMap)
                            {
                                ObjectId id = idP.Value;
                                dbObj = (Entity)id.GetObject(OpenMode.ForWrite);
                                idsNew.Add(id);
                            }

                            JigDraw jDraw = new JigDraw(pnt3dBase);
                            jDraw.AddEntity(idsNew);

                            PromptResult jRes = BaseObjs._editor.Drag(jDraw);
                            if (jRes.Status == PromptStatus.OK)
                            {
                                jDraw.TransformEntities();
                            }
                            else
                                return;

                            db.Dispose();
                            tr.Commit();
                        }
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Blocks.cs: line: 634");
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Blocks.cs: line: 640");
            }
        }

        public static void
        clone(ObjectIdCollection ids, Point3d pnt3dBase)
        {
            Database db = BaseObjs._db;
            ObjectIdCollection idsNew = new ObjectIdCollection();

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord ms = Blocks.getBlockTableRecordMS();

                    foreach (ObjectId id in ids)
                    {
                        Entity ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                        Entity entCopy = (Entity)ent.Clone();
                        ms.AppendEntity(entCopy);
                        tr.AddNewlyCreatedDBObject(entCopy, true);
                        idsNew.Add(entCopy.ObjectId);
                    }

                    db.Dispose();
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Blocks.cs: line: 671");
            }

            JigDraw jDraw = new JigDraw(pnt3dBase);
            jDraw.AddEntity(idsNew);

            PromptResult jRes = BaseObjs._editor.Drag(jDraw);
            if (jRes.Status == PromptStatus.OK)
            {
                jDraw.TransformEntities();
            }
            else
                return;
        }

        public static string
        getBlockRefAttributeValue(ObjectId idBR, string nameAttribute)
        {
            string value = string.Empty;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockReference br = (BlockReference)tr.GetObject(idBR, OpenMode.ForRead);
                    foreach (ObjectId idAR in br.AttributeCollection)
                    {
                        Autodesk.AutoCAD.DatabaseServices.DBObject obj = tr.GetObject(idAR, OpenMode.ForRead);
                        AttributeReference ar = (AttributeReference)obj;
                        if (ar != null)
                        {
                            if (ar.Tag.ToUpper() == nameAttribute.ToUpper())
                            {
                                value = ar.TextString;
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Blocks.cs: line: 711");
            }
            return value;
        }

        public static string
        getBlockRefName(ObjectId idBR)
        {
            string nameBlkRef;
            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                BlockReference br = (BlockReference)tr.GetObject(idBR, OpenMode.ForRead);
                nameBlkRef = br.Name;
            }
            return nameBlkRef;
        }

        /// <summary>
        /// get BlockTable
        /// </summary>
        /// <returns>BlockTable</returns>
        public static BlockTable
        getBlockTable()
        {
            BlockTable BT = null;
            Database DB = BaseObjs._db;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BT = (BlockTable)DB.BlockTableId.GetObject(OpenMode.ForRead);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Blocks.cs: line: 747");
            }
            return BT;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static BlockTableRecord
        getBlockTableRecordMS()
        {
            BlockTableRecord Btr = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTable BT = getBlockTable();
                    Btr = (BlockTableRecord)BT[BlockTableRecord.ModelSpace].GetObject(OpenMode.ForWrite);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Blocks.cs: line: 771");
            }
            return Btr;
        }

        public static ObjectId
        insBlockRef(string strPath, string strName)
        {
            Database DB = BaseObjs._db;

            ObjectId blkID = ObjectId.Null;

            string strNameDWG = string.Format("{0}{1}", strPath, strName);

            try
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    Database blkDB = new Database(false, false);

                    using (blkDB)
                    {
                        blkDB.ReadDwgFile(strNameDWG, System.IO.FileShare.Read, true, "");
                        blkID = DB.Insert(strNameDWG, blkDB, true);
                    }// end using blkDB
                }// end using DOCLOC
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Blocks.cs: line: 800");
            }

            return blkID;
        }// insBlockRef
    }
}
