using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

using System;
using System.Collections.Generic;

using DBObject = Autodesk.AutoCAD.DatabaseServices.DBObject;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Base_Tools45
{
    public static partial class xRef
    {
        public static Entity
        getNestedEntityAndHighlight(Point3d pnt3d, BlockReference br, out FullSubentityPath path, out bool isClosed)
        {
            Entity ent = null;
            isClosed = false;
            path = new FullSubentityPath();

            Editor ed = BaseObjs._editor;

            PromptNestedEntityOptions pneo = new PromptNestedEntityOptions("");
            pneo.NonInteractivePickPoint = pnt3d;
            pneo.UseNonInteractivePickPoint = true;
            PromptNestedEntityResult pner = ed.GetNestedEntity(pneo);

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    if (pner.Status == PromptStatus.OK)
                    {
                        ObjectId[] idsContainers = pner.GetContainers();
                        ObjectId idSel = pner.ObjectId;
                        int len = idsContainers.Length;

                        ObjectId[] idsRev = new ObjectId[len + 1];
                        //Reverse the "containers" list
                        for (int i = 0; i < len; i++)
                        {
                            ObjectId id = (ObjectId)idsContainers.GetValue(len - i - 1);
                            idsRev.SetValue(id, i);
                        }
                        //Now add the selected entity to the end of the array
                        idsRev.SetValue(idSel, len);

                        //Retrieve the sub-entity path for this entity
                        SubentityId idSubEnt = new SubentityId(SubentityType.Null, (IntPtr)0);
                        path = new FullSubentityPath(idsRev, idSubEnt);

                        ObjectId idX = (ObjectId)idsRev.GetValue(0);
                        ent = (Entity)tr.GetObject(idX, OpenMode.ForRead);

                        DBObject obj = idSel.GetObject(OpenMode.ForRead);

                        ObjectId idOwner = ObjectId.Null;
                        DBObject objParent = null;

                        if (obj is PolylineVertex3d)
                        {
                            idOwner = obj.OwnerId;
                            objParent = (Polyline3d)idOwner.GetObject(OpenMode.ForRead);
                            Polyline3d poly3d = (Polyline3d)obj;
                            poly3d.Highlight(path, false);
                            isClosed = poly3d.Closed;
                        }
                        else if (obj is Vertex2d)
                        {
                            idOwner = obj.OwnerId;
                            objParent = (Polyline)idOwner.GetObject(OpenMode.ForRead);
                            Polyline poly = (Polyline)obj;
                            poly.Highlight(path, false);
                            isClosed = poly.Closed;
                        }
                        else if (obj is Polyline3d)
                        {
                            objParent = obj;
                            Polyline3d poly3d = (Polyline3d)obj;
                            poly3d.Highlight(path, false);
                            isClosed = poly3d.Closed;
                        }
                        else if (obj is Polyline)
                        {
                            objParent = obj;
                            Polyline poly = (Polyline)obj;
                            poly.Highlight(path, false);
                            isClosed = poly.Closed;
                        }
                        else if (obj is Arc)
                        {
                            objParent = obj;
                            Arc arc = (Arc)obj;
                            arc.Highlight(path, false);
                            isClosed = false;
                        }
                        else if (obj is MText || obj is DBText)
                            return null;

                        ent = (Entity)objParent;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef2.cs: line: 108");
            }
            return ent;
        }

        public static List<Point3d>
        getNestedEntityCoordinates(Handle h, string nameFile)
        {
            Entity ent = null;

            Database xdb = new Database(false, true);
            xdb.ReadDwgFile(nameFile, FileOpenMode.OpenForReadAndReadShare, true, null);
            List<Point3d> pnts3d = new List<Point3d>();

            //BlockReference br = getXRefBlock(nameFile);
            //ObjectId idbtr = br.BlockTableRecord;

            try
            {
                using (Transaction tr = xdb.TransactionManager.StartOpenCloseTransaction())
                {
                    //BlockTableRecord btr = (BlockTableRecord)tr.GetObject(idbtr, OpenMode.ForRead);
                    //Database dbXRef = btr.Database;
                    ObjectId id = Db.handleToObjectId(xdb, h);
                    if (!id.IsValid)
                        return pnts3d;

                    ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                    if (ent is Line)
                    {
                        Line line = (Line)ent;
                        pnts3d.Add(line.StartPoint);
                        pnts3d.Add(line.EndPoint);
                    }
                    else if (ent is Polyline || ent is Polyline3d)
                    {
                        pnts3d = Conv.polyX_listPnts3d(id);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef2.cs: line: 151");
            }
            finally
            {
                xdb.Dispose();
                xdb = null;
            }
            return pnts3d;
        }

        public static List<Point3d>
        getNestedEntityCoordinates2(Handle h, string nameFile)
        {
            List<Point3d> pnts3d = new List<Point3d>();
            try
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        Database db = getXRefDatabase(nameFile);
                        BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                        BlockTableRecord ms = (BlockTableRecord)bt[BlockTableRecord.ModelSpace].GetObject(OpenMode.ForRead);
                        foreach (ObjectId id in ms)
                        {
                            Entity ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                            if (ent.Handle == h)
                            {
                                if (ent is Line)
                                {
                                    Line line = (Line)ent;
                                    pnts3d.Add(line.StartPoint);
                                    pnts3d.Add(line.EndPoint);
                                    break;
                                }
                                else if (ent is Polyline || ent is Polyline3d)
                                {
                                    pnts3d = Conv.polyX_listPnts3d(id);
                                }
                            }
                        }
                        tr.Commit();
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xRef2.cs: line: 198");
            }
            return pnts3d;
        }
    }
}
