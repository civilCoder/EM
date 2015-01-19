using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using Color = Autodesk.AutoCAD.Colors.Color;
using DBObject = Autodesk.AutoCAD.DatabaseServices.DBObject;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static class ExtensionMethods
    {
        private static double PI = System.Math.PI;

        /// <summary>
        ///
        /// </summary>
        /// <param name="surf"></param>
        /// <param name="ids"></param>
        public static void
        addBreaklines(this ObjectId idTinSurf, ObjectIdCollection ids)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    TinSurface tinSurf = (TinSurface)tr.GetObject(idTinSurf, OpenMode.ForWrite);
                    if (tinSurf == null)
                        return;
                    tinSurf.BreaklinesDefinition.AddStandardBreaklines(ids, 1.0, 1.0, 0.0, 0.0);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 46");
            }
        }

        public static void
        addDictEntry(this ObjectId idDict, string nameSubEntry, ObjectId idObj)
        {
            Dict.addDictEntry(idDict, nameSubEntry, idObj);
        }

        public static void
        addPointNode(this Point2d pnt2d, int pdMode, double pdSize)
        {
            Database db = BaseObjs._db;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord ms = Blocks.getBlockTableRecordMS();
                    DBPoint pnt = new DBPoint(new Point3d(pnt2d.X, pnt2d.Y, 0));
                    pnt.SetDatabaseDefaults();
                    ms.AppendEntity(pnt);
                    tr.AddNewlyCreatedDBObject(pnt, true);

                    db.Pdmode = pdMode;
                    db.Pdsize = pdSize;

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 78");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idPoly3d"></param>
        /// <param name="nameLayer"></param>
        /// <returns></returns>
        public static ObjectId
        addPoly(this ObjectId idPoly3d, string nameLayer = "0")
        {
            return Draw.addPolyFromPoly3d(idPoly3d, nameLayer);
        }

        public static ObjectId
        addPoly(this List<Vertex2d> vtxList, string nameLayer)
        {
            return Draw.addPoly(vtxList, nameLayer);
        }

        public static ObjectId
        addPoly(this Point3dCollection pnts3d, string nameLayer = "0", short color = 256)
        {
            return Draw.addPoly(pnts3d, nameLayer);
        }

        /// </summary>
        /// <param name="cogoPnts"></param>
        /// <param name="nameLayer"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static ObjectId
        addPoly(this List<ObjectId> cogoPnts, string nameLayer = "0", short color = 256)
        {
            ObjectId idPoly = ObjectId.Null;
            Point3dCollection pnts3d = new Point3dCollection();
            Layer.manageLayers(nameLayer);
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    foreach (ObjectId idCogoPnt in cogoPnts)
                    {
                        CogoPoint cogoPnt = (CogoPoint)tr.GetObject(idCogoPnt, OpenMode.ForRead);
                        pnts3d.Add(new Point3d(cogoPnt.Easting, cogoPnt.Northing, cogoPnt.Elevation));
                        idPoly = Draw.addPoly(pnts3d, nameLayer);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 132");
            }
            return idPoly;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnts3d"></param>
        /// <param name="nameLayer"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static ObjectId
        addPoly3d(this Point3dCollection pnts3d, string nameLayer = "0", short color = 256)
        {
            return Draw.addPoly3d(pnts3d, nameLayer, color);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cogoPnts"></param>
        /// <param name="nameLayer"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static ObjectId
        addPoly3d(this List<ObjectId> cogoPnts, string nameLayer = "0", short color = 256)
        {
            ObjectId idPoly3d = ObjectId.Null;
            Point3dCollection pnts3d = new Point3dCollection();
            Layer.manageLayers(nameLayer);
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    foreach (ObjectId idCogoPnt in cogoPnts)
                    {
                        CogoPoint cogoPnt = (CogoPoint)tr.GetObject(idCogoPnt, OpenMode.ForRead);
                        pnts3d.Add(new Point3d(cogoPnt.Easting, cogoPnt.Northing, cogoPnt.Elevation));
                    }
                    idPoly3d = Draw.addPoly3d(pnts3d, nameLayer);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 178");
            }
            return idPoly3d;
        }

        public static void
        adjLdrBegPnt(this ObjectId idLdr, Point3d pnt3dX)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Leader ldr = (Leader)tr.GetObject(idLdr, OpenMode.ForWrite);
                    ldr.SetVertexAt(0, pnt3dX);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 197");
            }
        }

        public static Point3d
        adjLdrEndPnt(this ObjectId idLdr, Point3d pnt3dCen, Point3d pnt3dOrg, double scaleFactor)
        {
            return Mod.adjLdrEndpoint(idLdr, pnt3dCen, pnt3dOrg, scaleFactor);
        }

        public static void
        adjLdrEndPnt(this ObjectId idLdr, Point3d pnt3dEnd)
        {
            Mod.adjLdrEndpoint(idLdr, pnt3dEnd);
        }

        public static void
        adjMTextXYandAngle(this ObjectId id, Point3d pnt3dEnd, double angle, double width)
        {
            Mod.adjMTextXYandRotation(id, pnt3dEnd, angle, width);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nameLayer"></param>
        /// <param name="lineWt"></param>
        /// <param name="color"></param>
        public static void
        changeProp(this ObjectId id, Color color = null, string nameLayer = "0", LineWeight lineWt = LineWeight.ByLayer)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                    try
                    {
                        Layer.manageLayers(nameLayer);
                        ent.Layer = nameLayer;
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 241");
                    }

                    if (color == null)
                        color = Color.FromColorIndex(ColorMethod.ByLayer, 256);

                    ent.LineWeight = lineWt;
                    ent.Color = color;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 254");
            }
        }

        public static void
        changeProp(this ObjectId id, LineWeight lineWt, Color color)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                    ent.LineWeight = lineWt;
                    ent.Color = color;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 273");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="poly"></param>
        public static void
        checkIfClosed(this Polyline poly)
        {
            Geom.checkIfClosed(poly.ObjectId);
        }

        public static void
        checkIfClosed(this ObjectId idPoly)
        {
            Geom.checkIfClosed(idPoly);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="poly3d"></param>
        public static void
        checkIfClosed3d(this Polyline3d poly3d)
        {
            Geom.checkIfClosed(poly3d.ObjectId);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nameApp"></param>
        public static void
        clearXData(this ObjectId id, string nameApp)
        {
            xData.removeAppXData(id, nameApp);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="h"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static bool
        compareHandle(this Handle h, Object x)
        {
            bool check = h == x.ToString().stringToHandle();
            return check;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nameLayer"></param>
        /// <returns></returns>
        public static ObjectId
        copy(this ObjectId id, string nameLayer)
        {
            ObjectId idCopy = ObjectId.Null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                    Layer.manageLayers(nameLayer);
                    idCopy = Blocks.copy(ent, nameLayer);
                    Entity copy = (Entity)tr.GetObject(idCopy, OpenMode.ForRead);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 350");
            }
            Entity entX = idCopy.getEnt();
            if (entX is PolylineVertex3d || entX is Vertex2d)
                idCopy = entX.OwnerId;

            return idCopy;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="nameLayer"></param>
        /// <returns></returns>
        public static ObjectId
        copy(this Entity ent, string nameLayer = "0")
        {
            return Blocks.copy(ent, nameLayer);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ids"></param>
        public static void
        delete(this ObjectIdCollection ids)
        {
            foreach (ObjectId id in ids)
                id.delete();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        public static void
        delete(this ObjectId id)
        {
            Misc.deleteObj(id);
        }

        public static void
        delete(this Entity ent)
        {
            ObjectId idEnt = ent.ObjectId;
            Misc.deleteObj(idEnt);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="surf"></param>
        public static void
        deleteBreaklines(this TinSurface surf)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    for (int i = surf.BreaklinesDefinition.Count - 1; i > -1; i--)
                    {
                        surf.BreaklinesDefinition.RemoveAt(i);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 415");
            }
        }

        public static void
        deleteLinkedEnts(this TypedValue[] tvs)
        {
            foreach (TypedValue tv in tvs)
            {
                if (tv.TypeCode == 1005)
                    if (tv.Value.ToString() != "0")
                    {
                        ObjectId id = tv.Value.ToString().stringToHandle().getObjectId();
                        id.delete();
                    }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ids"></param>
        public static void
        deleteObjs(this ObjectId[] ids)
        {
            Misc.deleteObjs(ids);
        }

        public static void
        deleteVertices(this Polyline3d poly3d, List<ObjectId> idsCgPnt)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Polyline3d p3d = (Polyline3d)tr.GetObject(poly3d.ObjectId, OpenMode.ForWrite);
                    ObjectId[] ids = p3d.Cast<ObjectId>().ToArray();
                    for (int i = ids.Length - 2; i > 0; i--)
                    {
                        PolylineVertex3d v3d = (PolylineVertex3d)tr.GetObject(ids[i], OpenMode.ForWrite);
                        foreach (ObjectId idCgPnt in idsCgPnt)
                        {
                            Point3d pnt3dCgPnt = idCgPnt.getCogoPntCoordinates();
                            Point3d pnt3dV3d = v3d.Position;
                            if (pnt3dV3d != pnt3dCgPnt)
                                v3d.Erase();
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 468");
            }
        }

        public static bool
        dictExists(this string name)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            try
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    DBDictionary dictNOD = (DBDictionary)db.NamedObjectsDictionaryId.GetObject(OpenMode.ForRead);
                    return (dictNOD.Contains(name));
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 486");
            }
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ss"></param>
        public static void
        eraseSelectedItems(this SelectionSet ss)
        {
            foreach (ObjectId id in ss)
            {
                Misc.deleteObj(id);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database"></param>
        /// <param name="action"></param>
        public static void
        forEachBR<T>(this Database database, Action<T> action) where T : BlockReference
        {
            try
            {
                using (var tr = BaseObjs.startTransactionDb())
                {
                    SelectionSet ss = Select.buildSSet(typeof(BlockReference), true);
                    RXClass theClass = RXObject.GetClass(typeof(T));
                    foreach (ObjectId objectId in ss.GetObjectIds())
                    {
                        if (objectId.ObjectClass.IsDerivedFrom(theClass))
                        {
                            var entity = (T)tr.GetObject(objectId, OpenMode.ForRead);
                            action(entity);
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 532");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="action"></param>
        public static void
        forEachMS<T>(this Database db, Action<T> action) where T : Autodesk.AutoCAD.DatabaseServices.Entity
        {
            Document acadDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            try
            {
                using (var tr = BaseObjs.startTransactionDb())
                {
                    var bt = (BlockTable)tr.GetObject(acadDoc.Database.BlockTableId, OpenMode.ForRead);
                    var ms = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                    RXClass theClass = RXObject.GetClass(typeof(T));
                    foreach (ObjectId ed in ms)
                    {
                        if (ed.ObjectClass.IsDerivedFrom(theClass))
                        {
                            var entity = (T)tr.GetObject(ed, OpenMode.ForRead);
                            action(entity);
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 566");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="poly3d"></param>
        /// <returns></returns>
        public static Point3d
        getBegPnt(this Polyline3d poly3d)
        {
            Point3d pnt3d = Pub.pnt3dO;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Polyline3d p3d = (Polyline3d)tr.GetObject(poly3d.ObjectId, OpenMode.ForRead);
                    tr.Commit();
                    pnt3d = p3d.StartPoint;
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 590");
            }
            return pnt3d;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        public static Point3d
        getBegPnt(this Handle h)
        {
            Point3d pnt3d = Pub.pnt3dO;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ObjectId id = h.getObjectId();
                    Polyline3d p3d = (Polyline3d)tr.GetObject(id, OpenMode.ForRead);
                    tr.Commit();
                    pnt3d = p3d.StartPoint;
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 616");
            }
            return pnt3d;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idPoly3d"></param>
        /// <returns></returns>
        public static Point3d
        getBegPnt(this ObjectId id)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                    if (ent is Polyline)
                    {
                        Polyline poly = (Polyline)ent;
                        return poly.StartPoint;
                    }
                    if (ent is Polyline3d)
                    {
                        Polyline3d poly3d = (Polyline3d)ent;
                        return poly3d.StartPoint;
                    }
                    if (ent is Leader)
                    {
                        Leader ldr = (Leader)ent;
                        return ldr.FirstVertex;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 654");
            }
            return Pub.pnt3dO;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static ObjectIdCollection
        getBrkLines(this Document doc)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            try
            {
                Type type = typeof(Polyline3d);
                TypedValue[] TVs = new TypedValue[2];
                TVs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(type).DxfName), 0);
                TVs.SetValue(new TypedValue((int)DxfCode.LayerName, "*-BRKLINE"), 1);
                SelectionSet ss = Select.buildSSet(TVs);

                if (ss != null && ss.Count > 0)
                {
                    ObjectId[] idArray = ss.GetObjectIds();
                    foreach (ObjectId id in idArray)
                        ids.Add(id);
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 685");
            }
            return ids;
        }

        public static Point3d
        getCenter(this Handle h, int sides)
        {
            Point3d pnt3dCEN = Pub.pnt3dO;
            Point3dCollection pnts3d = new Point3dCollection();
            double x = 0;
            double y = 0;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Entity ent = (Entity)tr.GetObject(h.getObjectId(), OpenMode.ForRead);
                    if (ent is Circle)
                    {
                        Circle c = (Circle)ent;
                        pnt3dCEN = c.Center;
                    }
                    if (ent is Polyline)
                    {
                        Polyline poly = (Polyline)ent;
                        pnts3d = poly.getCoordinates3d();

                        for (int i = 0; i < sides; i++)
                        {
                            x = x + pnts3d[i].X;
                            y = y + pnts3d[i].Y;
                        }

                        x = x / sides;
                        y = y / sides;
                        pnt3dCEN = new Point3d(x, y, 0);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 727");
            }
            return pnt3dCEN;
        }

        public static Point3d
        getCentroid(this Handle h)
        {
            Point3d pnt3d = Pub.pnt3dO;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Polyline poly = (Polyline)tr.GetObject(h.getObjectId(), OpenMode.ForRead);
                    tr.Commit();
                    pnt3d = poly.getCentroid();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 747");
            }
            return pnt3d;
        }

        public static Point3d
        getCentroid(this ObjectId id)
        {
            Point3d pnt3d = Pub.pnt3dO;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Polyline poly = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                    tr.Commit();
                    pnt3d = poly.getCentroid();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 767");
            }
            return pnt3d;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="poly"></param>
        /// <returns></returns>
        public static Point3d
        getCentroid(this Polyline poly)
        {
            return Geom.getCentroid(poly);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        public static Point3d
        getCogoPntCoordinates(this Handle h)
        {
            Point3d pnt3d = Pub.pnt3dO;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ObjectId id = Db.handleToObjectId(HostApplicationServices.WorkingDatabase, h);
                    Entity ent = (Entity)id.GetObject(OpenMode.ForRead);
                    if (ent is CogoPoint)
                    {
                        CogoPoint cogoPnt = (CogoPoint)ent;
                        pnt3d = cogoPnt.Location;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 808");
            }
            return pnt3d;
        }

        public static Point3d
        getCogoPntCoordinates(this ObjectId id)
        {
            Point3d pnt3d = Pub.pnt3dO;
            if (id == ObjectId.Null)
                return pnt3d;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Entity ent = (Entity)id.GetObject(OpenMode.ForRead);
                    if (ent is CogoPoint)
                    {
                        CogoPoint cogoPnt = (CogoPoint)ent;
                        pnt3d = cogoPnt.Location;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 834");
            }
            return pnt3d;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string
        getCogoPntDesc(this ObjectId id)
        {
            string desc = string.Empty;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    DBObject dbObj = tr.GetObject(id, OpenMode.ForRead);
                    CogoPoint cogoPnt = (CogoPoint)dbObj;
                    if (cogoPnt != null)
                    {
                        desc = cogoPnt.RawDescription;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 863");
            }
            return desc;
        }

        public static double
        getCogoPntElevation(this ObjectId idCgPnt)
        {
            double elev = 0.0;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDoc())
                {
                    CogoPoint cgPnt = (CogoPoint)tr.GetObject(idCgPnt, OpenMode.ForRead);
                    elev = cgPnt.Elevation;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 883");
            }
            return elev;
        }

        public static object
        getCogoPntNumber(this ObjectId id)
        {
            object pntNum = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    DBObject dbObj = tr.GetObject(id, OpenMode.ForRead);
                    CogoPoint cogoPnt = (CogoPoint)dbObj;
                    if (cogoPnt != null)
                    {
                        pntNum = cogoPnt.PointNumber;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 907");
            }
            return pntNum;
        }

        public static List<CogoPoint>
        getCogoPntsFromHandlesList(this List<Handle> handles)
        {
            Database db = BaseObjs._db;
            List<CogoPoint> cogoPnts = new List<CogoPoint>();
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    foreach (Handle handle in handles)
                    {
                        CogoPoint cogoPnt = (CogoPoint)handle.getEnt();
                        cogoPnts.Add(cogoPnt);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 931");
            }
            return cogoPnts;
        }

        public static Point2dCollection
        getCoordinates2d(this Polyline poly)
        {
            return Conv.poly_pnt2dColl(poly);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Point3dCollection
        getCoordinates3d(this ObjectId id)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                    if (ent is Polyline)
                    {
                        Polyline poly = (Polyline)ent;
                        return poly.getCoordinates3d();
                    }
                    if (ent is Polyline2d)
                    {
                        Polyline2d poly2d = (Polyline2d)ent;
                        return poly2d.getCoordinates3d();
                    }
                    if (ent is Polyline3d)
                    {
                        Polyline3d poly3d = (Polyline3d)ent;
                        return poly3d.getCoordinates3d();
                    }
                    if (ent is Leader)
                    {
                        Leader ldr = (Leader)ent;
                        Point3dCollection pnts3d = new Point3dCollection();
                        for (int i = 0; i < ldr.NumVertices; i++)
                        {
                            pnts3d.Add(ldr.VertexAt(i));
                        }
                        return pnts3d;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 985");
            }
            return null;
        }

        public static Point3dCollection
        getCoordinates3d(this Polyline poly)
        {
            return Conv.poly_pnt3dColl(poly);
        }

        public static Point3dCollection
        getCoordinates3d(this Polyline2d poly2d)
        {
            return Conv.poly2d_pnt3dColl(poly2d);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="poly3d"></param>
        /// <returns></returns>
        public static Point3dCollection
        getCoordinates3d(this Polyline3d poly3d)
        {
            return Conv.poly3d_pnt3dColl(poly3d);
        }

        public static List<Point3d>
        getCoordinates3dList(this ObjectId id)
        {
            if (id.IsNull)
                return null;

            List<Point3d> pnts3d = new List<Point3d>();
            string type = id.getType();
            Entity ent = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 1029");
            }

            Point3dCollection pnts = null;
            switch (type)
            {
                case "Polyline":
                    Polyline poly = (Polyline)ent;
                    pnts = poly.getCoordinates3d();
                    break;

                case "Polyline2d":
                    Polyline2d poly2d = (Polyline2d)ent;
                    pnts = new Point3dCollection();
                    foreach (ObjectId idVtx in poly2d)
                    {
                        Vertex2d vtx2d = (Vertex2d)idVtx.getEnt();
                        pnts.Add(vtx2d.Position);
                    }
                    break;

                case "Polyline3d":
                    Polyline3d poly3d = (Polyline3d)ent;
                    pnts = poly3d.getCoordinates3d();
                    break;

                case "Leader":
                    Leader ldr = (Leader)ent;
                    pnts = new Point3dCollection();
                    for (int i = 0; i < ldr.NumVertices; i++)
                    {
                        pnts.Add(ldr.VertexAt(i));
                    }
                    break;
            }

            foreach (Point3d pnt in pnts)
                pnts3d.Add(pnt);

            return pnts3d;
        }

        public static double
        getCosineComponent(this Point3d pnt3dX, Point3d pnt3dCen, Point3d pnt3dTar)
        {
            return Geom.getCosineComponent(pnt3dCen, pnt3dTar, pnt3dX);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<DBDictionaryEntry>
        getDictEntries(this ObjectId id)
        {
            return Dict.getEntries(id);
        }

        public static double
        getDirection(this Point2d pnt2d0, Point2d pnt2dX)
        {
            return Measure.getAzRadians(pnt2d0, pnt2dX);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnt3d0"></param>
        /// <param name="pnt3dX"></param>
        /// <returns></returns>
        public static double
        getDirection(this Point3d pnt3d0, Point3d pnt3dX)
        {
            return Measure.getAzRadians(pnt3d0, pnt3dX);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnt3dBASE"></param>
        /// <param name="pnt3dTAR"></param>
        /// <returns></returns>
        public static double
        getDistance(this Point3d pnt3dBASE, Point3d pnt3dTAR)
        {
            return Measure.getDistance2d(pnt3dBASE, pnt3dTAR);
        }

        public static double
        getDistance(this Point2d pnt2dBASE, Point2d pnt2dTAR)
        {
            return Measure.getDistance2d(pnt2dBASE, pnt2dTAR);
        }

        public static double
        getDistance(this Point2d pnt2dBASE, Point3d pnt3dTAR)
        {
            Point2d pnt2dTAR = new Point2d(pnt3dTAR.X, pnt3dTAR.Y);
            return Measure.getDistance2d(pnt2dBASE, pnt2dTAR);
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="idPoly3d"></param>
        /// <returns></returns>
        public static Point3d
        getEndPnt(this ObjectId id)
        {
            Point3d pnt3d = Pub.pnt3dO;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                    if (ent is Polyline)
                    {
                        Polyline poly = (Polyline)ent;
                        pnt3d = poly.EndPoint;
                    }
                    if (ent is Polyline3d)
                    {
                        Polyline3d poly3d = (Polyline3d)ent;
                        pnt3d = poly3d.EndPoint;
                    }
                    if (ent is Leader)
                    {
                        Leader ldr = (Leader)ent;
                        pnt3d = ldr.LastVertex;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 1166");
            }
            return pnt3d;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="poly3d"></param>
        /// <returns></returns>
        public static Point3d
        getEndPnt(this Polyline3d poly3d)
        {
            Point3d pnt3d = Pub.pnt3dO;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Polyline3d p3d = (Polyline3d)tr.GetObject(poly3d.ObjectId, OpenMode.ForRead);
                    tr.Commit();
                    pnt3d = p3d.EndPoint;
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 1191");
            }
            return pnt3d;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        public static Point3d
        getEndPnt(this Handle h)
        {
            Point3d pnt3d = Pub.pnt3dO;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ObjectId id = h.getObjectId();
                    Polyline3d p3d = (Polyline3d)tr.GetObject(id, OpenMode.ForRead);
                    tr.Commit();
                    pnt3d = p3d.EndPoint;
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 1217");
            }
            return pnt3d;
        }

        public static DBObject
        getDbObject(this ObjectId id)
        {
            DBObject dbObj = null;
            using(var tr = BaseObjs.startTransactionDb()){
                dbObj = tr.GetObject(id, OpenMode.ForRead);                
            }
            return dbObj;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Entity
        getEnt(this ObjectId id)
        {
            Entity ent = null;
            ent = Misc.getEntityByObjectID(id);
            return ent;
        }

        public static Entity
        getEnt(this ObjectId id, string nameDwg)
        {
            Database db = xRef.getXRefBlockReference(nameDwg).Database;
            Entity ent = null;
            try
            {
                using (Transaction tr = db.TransactionManager.StartOpenCloseTransaction())
                {
                    ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 1259");
            }
            return ent;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static Entity
        getEnt(this Handle handle)
        {
            return Db.handleToObject(handle.ToString());
        }

        public static ObjectId[]
        getEntitysAtPoint(this Point3d pnt3d, string nameLayer = "*")
        {
            return Select.getEntitysAtPoint(pnt3d, nameLayer);
        }

        public static List<DBDictionaryEntry>
        getEntries(this ObjectId id)
        {
            return Dict.getEntries(id);
        }

        public static ObjectIdCollection
        getFeatureLines(this Document doc)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            try
            {
                Type type = typeof(FeatureLine);
                TypedValue[] TVs = new TypedValue[2];
                TVs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(type).DxfName), 0);
                TVs.SetValue(new TypedValue((int)DxfCode.LayerName, "CPNT-BRKLINE"), 1);
                SelectionSet ss = Select.buildSSet(TVs);

                if (ss != null && ss.Count > 0)
                {
                    ObjectId[] idArray = ss.GetObjectIds();
                    foreach (ObjectId id in idArray)
                        ids.Add(id);
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 1308");
            }
            return ids;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Handle
        getHandle(this ObjectId id)
        {
            return Db.idObjToHandle(id);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nameApp"></param>
        /// <returns></returns>
        public static List<Handle>
        getHandleList(this ObjectId id, string nameApp)
        {
            ResultBuffer rb = id.getXData(nameApp);
            if (rb == null)
                return null;
            return rb.rb_handles();
        }

        public static List<Handle>
        getHandleList(this TypedValue[] tvs)
        {
            List<Handle> handles = new List<Handle>();
            for (int i = 1; i < tvs.Length; i++)
                handles.Add(tvs[i].Value.ToString().stringToHandle());
            return handles;
        }

        public static KeyValuePair<int, long>
        getLastHandle(this List<string> sHandles)
        {
            SortedDictionary<int, long> lnH = new SortedDictionary<int, long>();
            for (int i = 0; i < sHandles.Count; i++)
            {
                lnH.Add(i, System.Convert.ToInt64(sHandles[i], 16));
            }

            return lnH.Last();
        }

        public static Point3d
        getLastVertex(this ObjectId id)
        {
            Point3d pnt3d = Pub.pnt3dO;
            Leader ldr = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ldr = (Leader)tr.GetObject(id, OpenMode.ForRead);
                    pnt3d = ldr.LastVertex;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 1376");
            }
            return pnt3d;
        }

        public static string
        getLayer(this ObjectId id)
        {
            return Layer.getLayer(id);
        }

        public static double
        getLength(this ObjectId id)
        {
            double length = 0;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                    if (ent is Line)
                    {
                        Line line = (Line)ent;
                        length = line.Length;
                    }
                    else if (ent is Polyline)
                    {
                        Polyline poly = (Polyline)ent;
                        length = poly.Length;
                    }
                    else if (ent is Polyline3d)
                    {
                        Polyline3d poly3d = (Polyline3d)ent;
                        length = poly3d.Length;
                    }
                    else if (ent is Arc)
                    {
                        Arc arc = (Arc)ent;
                        length = arc.Length;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 1421");
            }
            return length;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnt3d0"></param>
        /// <param name="pnt3dX"></param>
        /// <returns></returns>
        public static Point3d
        getMidPoint2d(this Point3d pnt3d0, Point3d pnt3dX)
        {
            double x = (pnt3dX.X + pnt3d0.X) / 2;
            double y = (pnt3dX.Y + pnt3d0.Y) / 2;
            double z = 0.0;
            return new Point3d(x, y, z);
        }

        public static Point3d
        getMidPoint3d(this Point3d pnt3d0, Point3d pnt3dX)
        {
            double x = (pnt3dX.X + pnt3d0.X) / 2;
            double y = (pnt3dX.Y + pnt3d0.Y) / 2;
            double z = (pnt3dX.Z + pnt3d0.Z) / 2;
            return new Point3d(x, y, z);
        }

        public static ObjectId
        getNestedObjectID(this Handle h, string nameDwg)
        {
            ObjectId id = ObjectId.Null;
            BlockReference br = xRef.getXRefBlockReference(nameDwg);
            ObjectId idBtr = br.BlockTableRecord;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(idBtr, OpenMode.ForRead);
                    Database db = btr.Database;
                    id = Db.handleToObjectId(db, h);
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 1467");
            }
            return id;
        }

        public static Point3d
        getNextToLastVertex(this ObjectId id)
        {
            Point3d pnt3d = Pub.pnt3dO;
            int n = 0;
            Leader ldr = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ldr = (Leader)tr.GetObject(id, OpenMode.ForRead);
                    n = ldr.NumVertices;
                    pnt3d = ldr.VertexAt(n - 2);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 1490");
            }
            return pnt3d;
        }

        public static ObjectId
        getObjectId(this Handle h)
        {
            ObjectId id = ObjectId.Null;
            Database db = BaseObjs._db;
            if (db.TryGetObjectId(h, out id))
                return id;
            else
                return id;
        }

        public static ObjectId
        getObjectId(this TypedValue tv)
        {
            ObjectId id = ObjectId.Null;
            Handle h = tv.Value.ToString().stringToHandle();
            id = h.getObjectId();
            return id;
        }

        public static ObjectId
        getObjectId(this TypedValue[] tvs, int pos)
        {
            ObjectId id = ObjectId.Null;
            if (tvs[pos].TypeCode == 1005)
            {
                try
                {
                    id = tvs[pos].Value.ToString().stringToHandle().getObjectId();
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 1527");
                }
            }
            return id;
        }

        public static List<ObjectId>                //when list is from CogoPoint and it is being deleted then OK not to fix xData on Point
        getObjectIdList(this TypedValue[] tvs)
        {
            List<ObjectId> ids = new List<ObjectId>();
            ObjectId id = ObjectId.Null;
            for (int i = 1; i < tvs.Length; i++)
            {
                Handle h = tvs[i].Value.ToString().stringToHandle();
                id = h.getObjectId();
                if (id != ObjectId.Null)
                    ids.Add(id);
            }
            return ids;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnt3dPick"></param>
        /// <param name="pnt3d1"></param>
        /// <param name="pnt3d2"></param>
        /// <returns></returns>
        public static double
        getOrthoDist(this Point3d pnt3dPick, Point3d pnt3d1, Point3d pnt3d2)
        {
            return Geom.getPerpDistToLine(pnt3d1, pnt3d2, pnt3dPick);
        }

        public static List<ANG_PT_PROP>
        getPoly3dAngPtProps(this List<Point3d> pnts3dRF)
        {
            List<ANG_PT_PROP> angPtProps = new List<ANG_PT_PROP>();
            ANG_PT_PROP angPtProp;

            int k = pnts3dRF.Count;

            Point3d pnt3dPrv = pnts3dRF[k - 2];        //next to last point
            Point3d pnt3dEnd = pnts3dRF[k - 1];        //last point
            Point3d pnt3dBeg = pnts3dRF[0];            //first point

            bool isClosed = false;
            if (pnt3dBeg.isEqual(pnt3dEnd, 0.1))
            {
                isClosed = true;
            }

            for (int i = 1; i < k; i++)
            {
                angPtProp = new ANG_PT_PROP();

                angPtProp.SLP_SEG2 = pnts3dRF[i - 1].getSlope(pnts3dRF[i + 0]);
                angPtProp.DIR_SEG2 = pnts3dRF[i - 1].getDirection(pnts3dRF[i + 0]);
                angPtProp.LEN_SEG2 = pnts3dRF[i - 1].getDistance(pnts3dRF[i + 0]);
                angPtProp.BEG = pnts3dRF[i - 1];
                angPtProp.END = pnts3dRF[i + 0];

                if (i > 1)
                {
                    double deflc = Geom.getAngle3Points(pnts3dRF[i - 2], pnts3dRF[i - 1], pnts3dRF[i + 0]);
                    double testRight = Geom.testRight(pnts3dRF[i - 2], pnts3dRF[i - 1], pnts3dRF[i + 0]);
                    if (testRight < 0)
                    { // clockwise
                        angPtProp.ANG_DEFLC = -deflc;
                    }
                    else
                    { //counterclockwise
                        angPtProp.ANG_DEFLC = deflc;
                    }
                    angPtProp.PRV = pnts3dRF[i - 2];
                    angPtProp.SLP_SEG1 = pnts3dRF[i - 2].getSlope(pnts3dRF[i - 1]);
                    angPtProp.DIR_SEG1 = pnts3dRF[i - 2].getDirection(pnts3dRF[i - 1]);
                    angPtProp.LEN_SEG1 = pnts3dRF[i - 2].getDistance(pnts3dRF[i - 1]);
                }
                angPtProps.Add(angPtProp);
            }

            if (isClosed)
            {
                angPtProps[0].ANG_DEFLC = Geom.getAngle3Points(pnt3dPrv, pnt3dBeg, pnts3dRF[1]);
                if (Geom.testRight(pnt3dPrv, pnt3dBeg, pnts3dRF[1]) < 0)
                {
                    angPtProps[0].ANG_DEFLC = angPtProps[0].ANG_DEFLC * -1;
                }
                angPtProps[0].PRV = pnt3dPrv;
                angPtProps[0].SLP_SEG1 = pnt3dPrv.getSlope(pnt3dBeg);
                angPtProps[0].DIR_SEG1 = pnt3dPrv.getDirection(pnt3dBeg);
                angPtProps[0].LEN_SEG1 = pnt3dPrv.getDistance(pnt3dBeg);
            }
            return angPtProps;
        }

        public static List<SEG_PROP>
        getPoly3dSegProps(this List<Point3d> pnts3dRF)
        {
            List<SEG_PROP> segProps = new List<SEG_PROP>();
            SEG_PROP segProp;

            int k = pnts3dRF.Count;

            Point3d pnt3dPrv = pnts3dRF[k - 2];        //next to last point
            Point3d pnt3dEnd = pnts3dRF[k - 1];        //last point
            Point3d pnt3dBeg = pnts3dRF[0];            //first point

            bool isClosed = false;
            if (pnt3dBeg.isEqual(pnt3dEnd, 0.1))
            {
                isClosed = true;
            }

            for (int i = 1; i < k; i++)
            {
                segProp = new SEG_PROP();

                segProp.SLOPE_AHEAD = pnts3dRF[i - 1].getSlope(pnts3dRF[i + 0]);
                segProp.DIR_AHEAD = pnts3dRF[i - 1].getDirection(pnts3dRF[i + 0]);
                segProp.LENGTH = pnts3dRF[i - 1].getDistance(pnts3dRF[i + 0]);
                segProp.BEG = pnts3dRF[i - 1];
                segProp.END = pnts3dRF[i + 0];

                if (i > 1)
                {
                    segProp.DELTA = Geom.getAngle3Points(pnts3dRF[i - 2], pnts3dRF[i - 1], pnts3dRF[i + 0]);
                    if (Geom.testRight(pnts3dRF[i - 2], pnts3dRF[i - 1], pnts3dRF[i + 0]) < 0)
                    {
                        segProp.DELTA = segProp.DELTA * -1;
                    }
                    segProp.PRV = pnts3dRF[i - 2];
                    segProp.DIR_BACK = pnts3dRF[i - 1].getDirection(pnts3dRF[i - 2]);
                    segProp.SLOPE_BACK = pnts3dRF[i - 1].getSlope(pnts3dRF[i - 2]);
                }
                segProps.Add(segProp);
            }

            if (isClosed)
            {
                segProps[0].DELTA = Geom.getAngle3Points(pnt3dPrv, pnt3dBeg, pnts3dRF[1]);
                if (Geom.testRight(pnt3dPrv, pnt3dBeg, pnts3dRF[1]) < 0)
                {
                    segProps[0].DELTA = segProps[0].DELTA * -1;
                }
                segProps[0].PRV = pnt3dPrv;
                segProps[0].DIR_BACK = pnt3dBeg.getDirection(pnt3dPrv);
                segProps[0].SLOPE_BACK = pnt3dBeg.getSlope(pnt3dPrv);
            }
            return segProps;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="poly"></param>
        /// <returns></returns>
        public static List<SEG_PROP>
        getSegProps(this Polyline poly)
        {
            return Misc.getSegProps(poly.ObjectId);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnt3d0"></param>
        /// <param name="pnt3dX"></param>
        /// <returns></returns>
        public static double
        getSlope(this Point3d pnt3d0, Point3d pnt3dX)
        {
            return (pnt3dX.Z - pnt3d0.Z) / pnt3d0.getDistance(pnt3dX);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string
        getStringIncrement(this string name)
        {
            int j = 0;
            bool result = false;
            int num;
            string nameRet = string.Empty;

            for (int i = name.Length - 1; i > -1; i--)
            {
                result = Int32.TryParse(name.Substring(i), out num);
                if (!result)
                {
                    if (i == name.Length - 1)
                    {
                        j = 1;
                        nameRet = string.Format("{0}{1}", name, j.ToString());
                        return nameRet;
                    }
                    else
                    {
                        j = i;
                        break;
                    }
                }
            }

            string strNum = name.Substring(j + 1);
            result = Int32.TryParse(strNum, out num);
            if (result)
            {
                int intNum = num + 1;
                nameRet = string.Format("{0}{1}", name, intNum.ToString());
            }
            return nameRet;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ObjectId
        getStyle(this IEnumerable<ObjectId> ids, string name)
        {
            foreach (ObjectId id in ids)
            {
                StyleBase style = (StyleBase)id.GetObject(OpenMode.ForRead);
                if (style.Name == name)
                {
                    return id;
                }
            }
            return ObjectId.Null;
        }

        public static ObjectId
        getSubDict(this ObjectId id, string nameDictX)
        {
            return Dict.getSubEntry(id, nameDictX);
        }

        public static TypedValue[]
        getTVsAsArray(this ObjectId id, string nameApp)
        {
            return id.getXData(nameApp).AsArray();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string
        getType(this ObjectId id)
        {
            return Db.idObjToType(id);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnt3dPick"></param>
        /// <param name="poly3d"></param>
        /// <returns></returns>
        public static int
        getVertexNo(this Point3d pnt3dPick, ObjectId id)
        {
            int vertexNo = Geom.getVertexNo(id, pnt3dPick);
            return vertexNo;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="h"></param>
        /// <param name="nameApp"></param>
        /// <returns></returns>
        public static ResultBuffer
        getXData(this Handle h, string nameApp = "")
        {
            return xData.getXdata(h.getObjectId(), nameApp);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nameApp"></param>
        /// <returns></returns>
        public static ResultBuffer
        getXData(this ObjectId id, string nameApp = "")
        {
            return xData.getXdata(id, nameApp);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="poly3d"></param>
        /// <param name="nameApp"></param>
        /// <returns></returns>
        public static ResultBuffer
        getXData(this Polyline3d poly3d, string nameApp)
        {
            Entity ent = poly3d;
            return xData.getXdata(ent, nameApp);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cogoPnt"></param>
        /// <param name="nameApp"></param>
        /// <returns></returns>
        public static ResultBuffer
        getXData(this CogoPoint cogoPnt, string nameApp)
        {
            Entity ent = cogoPnt;
            return xData.getXdata(ent, nameApp);
        }

        public static long
        handle_long10(this string strHandle)
        {
            return System.Convert.ToInt64(strHandle, 10);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="nameApp"></param>
        /// <returns></returns>
        public static ResultBuffer
        handles_RB(this List<Handle> handles, string nameApp)
        {
            TypedValue[] TVs = new TypedValue[handles.Count + 1];
            TVs.SetValue(new TypedValue(1001, nameApp), 0);
            for (int i = 0; i < handles.Count; i++)
            {
                TVs.SetValue(new TypedValue(1005, handles[i]), i + 1);
                BaseObjs.write(handles[i].ToString());
            }
            return new ResultBuffer(TVs);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="entX"></param>
        /// <param name="includeBegAndEndPoints"></param>
        /// <param name="extendEnt"></param>
        /// <returns></returns>
        public static List<Point3d>
        intersectWith(this ObjectId id, Entity entX, bool includeBegAndEndPoints, extend ext)
        {
            Entity ent = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 1898");
            }
            return Geom.intersectWith(ent, entX, includeBegAndEndPoints, ext);
        }

        public static List<Point3d>
        intersectWith(this ObjectId id, ObjectId idEntX, extend ext)
        {
            List<Point3d> pnt3dList = new List<Point3d>();
            Entity ent = null;
            Entity entX = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                    entX = (Entity)tr.GetObject(idEntX, OpenMode.ForRead);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 1920");
            }

            return Geom.intersectWith(ent, entX, true, ext);
        }

        public static List<Point3d>
        intersectWith(this List<Point3d> pnts3d, List<Point3d> pnts3dX, bool includeBothEndsBndrySeg, extend ext)
        {

            return Geom.getPntInts(pnts3d[0], pnts3d[1], pnts3dX, includeBothEndsBndrySeg, ext);
        }

        /// <summary>
        /// Test if distance between two points is less than tolerance
        /// </summary>
        /// <param name="pnt3d0"></param>
        /// <param name="pnt3dX"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool
        isEqual(this Point3d pnt3d0, Point3d pnt3dX, double tolerance)
        {
            double dist = Measure.getDistance2d(pnt3d0, pnt3dX);
            if (dist <= tolerance)
            {
                return true;
            }
            return false;
        }

        public static bool
        isEqual(this Point2d pnt2d0, Point2d pnt2dX, double tolerance)
        {
            double dist = Measure.getDistance2d(pnt2d0, pnt2dX);
            if (dist <= tolerance)
            {
                return true;
            }
            return false;
        }

        public static bool
        isInteger(this string stringToTest, out int result)
        {
            if (int.TryParse(stringToTest, out result))
                return true;
            return false;
        }

        public static bool
        isDecimal(this string stringToTest, out decimal result)
        {
            if (decimal.TryParse(stringToTest, out result))
                return true;
            return false;
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="poly"></param>
        /// <returns></returns>
        public static bool
        isRightHand(this Polyline poly)
        {
            return Geom.isRightHandPoly(poly.ObjectId);
        }

        public static bool
        isRightHand(this ObjectId idPoly)
        {
            return Geom.isRightHandPoly(idPoly);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnt3dPick"></param>
        /// <param name="pnt3dBEG"></param>
        /// <param name="pnt3dEND"></param>
        /// <returns></returns>
        public static bool
        isRightSide(this Point3d pnt3dPick, Point3d pnt3dBEG, Point3d pnt3dEND)
        {
            bool isRight = false;
            double val = Geom.testRight(pnt3dBEG, pnt3dEND, pnt3dPick);
            if (val < 0)
                isRight = true;
            return isRight;
        }

        public static Point3d
        lastPoint(this Point3dCollection pnts3d)
        {
            if (pnts3d.Count > 0)
                return pnts3d[pnts3d.Count - 1];
            else
                return Pub.pnt3dO;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public static bool
        listContainsHandle(this List<Handle> handles, Handle h)
        {
            bool check = false;
            if (handles.Contains(h))
                check = true;
            return check;
        }

        public static void
        moveBelow(this ObjectId idTarget, List<ObjectId> ids)
        {
            ObjectIdCollection idsColl = new ObjectIdCollection();
            foreach (ObjectId id in ids)
                idsColl.Add(id);
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord ms = Blocks.getBlockTableRecordMS();
                    DrawOrderTable dot = (DrawOrderTable)tr.GetObject(ms.DrawOrderTableId, OpenMode.ForWrite);
                    dot.MoveBelow(idsColl, idTarget);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 2055");
            }
        }

        public static void
        moveBelow(this ObjectId idTarget, ObjectIdCollection ids)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord ms = Blocks.getBlockTableRecordMS();
                    DrawOrderTable dot = (DrawOrderTable)tr.GetObject(ms.DrawOrderTableId, OpenMode.ForWrite);
                    dot.MoveBelow(ids, idTarget);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 2074");
            }
        }

        public static void
        moveBottom(this ObjectId idTarget)
        {
            ObjectIdCollection ids = new ObjectIdCollection { idTarget };
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord ms = Blocks.getBlockTableRecordMS();
                    DrawOrderTable dot = (DrawOrderTable)tr.GetObject(ms.DrawOrderTableId, OpenMode.ForWrite);
                    dot.MoveToBottom(ids);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 2094");
            }
        }

        public static Point3d
        moveLdrEndPoint(this ObjectId id, Point3d pnt3dFrom, Point3d pnt3dTo, Point3d pnt3dOrg)
        {
            return Mod.moveLdrEndpoint(id, pnt3dFrom, pnt3dTo, pnt3dOrg);
        }

        public static void
        moveObj(this ObjectId id, Point3d pnt3dFrom, Point3d pnt3dTo)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Vector3d v3d = new Vector3d(pnt3dTo.X - pnt3dFrom.X, pnt3dTo.Y - pnt3dFrom.Y, 0);
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                    ent.TransformBy(Matrix3d.Displacement(v3d));
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 2119");
            }
        }

        public static void
        moveObjs(this ObjectIdCollection ids, Point3d pnt3dFrom, Point3d pnt3dTo)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Vector3d v3d = new Vector3d(pnt3dTo.X - pnt3dFrom.X, pnt3dTo.Y - pnt3dFrom.Y, 0);
                    foreach (ObjectId id in ids)
                    {
                        Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                        ent.TransformBy(Matrix3d.Displacement(v3d));
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 2141");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ss"></param>
        public static void
        moveToTop(this SelectionSet ss)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            ObjectId[] idArray = ss.GetObjectIds();

            foreach (ObjectId id in idArray)
                ids.Add(id);

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord ms = Blocks.getBlockTableRecordMS();
                    DrawOrderTable dot = (DrawOrderTable)tr.GetObject(ms.DrawOrderTableId, OpenMode.ForWrite);
                    dot.MoveToTop(ids);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 2170");
            }
        }

        public static void
        moveToTop(this ObjectId id)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            ids.Add(id);

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord ms = Blocks.getBlockTableRecordMS();
                    DrawOrderTable dot = (DrawOrderTable)tr.GetObject(ms.DrawOrderTableId, OpenMode.ForWrite);
                    dot.MoveToTop(ids);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 2192");
            }
        }

        public static TypedValue[]
        objectIdsToHandles(ObjectIdCollection ids, string nameApp)
        {
            TypedValue[] tvs = new TypedValue[ids.Count + 1];
            tvs.SetValue(new TypedValue((int)DxfCode.Start, nameApp), 0);
            for (int i = 0; i < ids.Count; i++)
            {
                tvs.SetValue(new TypedValue((int)DxfCode.Handle, ids[i].getHandle()), i + 1);
            }
            return tvs;
        }

        public static ObjectId
        offset(this ObjectId idX, double dist, string nameLayer = "0")
        {
            ObjectId idEnt = ObjectId.Null;
            string typeEnt = idX.getType();
            DBObjectCollection dbObjColl = null;
            Layer.manageLayers(nameLayer);
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    switch (typeEnt)
                    {
                        case "Line":
                            idEnt = Draw.addLineOffset(idX, dist);
                            break;

                        case "Polyline":
                            Polyline poly = (Polyline)tr.GetObject(idX, OpenMode.ForWrite);
                            dbObjColl = poly.GetOffsetCurves(dist);
                            break;

                        case "Arc":
                            Arc arc = (Arc)tr.GetObject(idX, OpenMode.ForWrite);
                            dbObjColl = arc.GetOffsetCurves(dist);
                            break;

                        default:
                            Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog(string.Format("{0} not supported for this command.", typeEnt));
                            break;
                    }
                    if (dbObjColl != null && dbObjColl.Count > 0)
                    {
                        BlockTableRecord btrMS = Base_Tools45.Blocks.getBlockTableRecordMS();
                        foreach (Entity ent in dbObjColl)
                        {
                            ent.Layer = nameLayer;
                            idEnt = btrMS.AppendEntity(ent);
                            tr.AddNewlyCreatedDBObject(ent, true);
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 2254");
            }
            return idEnt;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="poly"></param>
        /// <param name="offH"></param>
        /// <param name="offV"></param>
        /// <returns></returns>
        public static Polyline
        offset(this Polyline poly, double offH, double offV)
        {
            Polyline polyOff = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord MS = Blocks.getBlockTableRecordMS();
                    DBObjectCollection dbColl = poly.GetOffsetCurves(offH);
                    foreach (Entity ent in dbColl)
                    {
                        MS.AppendEntity(ent);
                        tr.AddNewlyCreatedDBObject(ent, true);
                        polyOff = (Polyline)ent;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 2287");
            }
            return polyOff;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="poly"></param>
        /// <param name="dist"></param>
        /// <returns></returns>
        public static ObjectId
        offset(this Polyline poly, double dist)
        {
            return Draw.addPolyOffset(poly.ObjectId, dist);
        }

        public static ObjectId
        offset3dPoly(this ObjectId idPoly3dRF, double offH, double offV, int side, string nameLayer)
        {
            ObjectId idPoly3dNew = ObjectId.Null;
            List<Point3d> pnts3dRF = idPoly3dRF.getCoordinates3dList();
            List<SEG_PROP> segProps = pnts3dRF.getPoly3dSegProps();

            int k = segProps.Count;
            double xSlope = offV / offH;

            double dist = 0.0;
            double slope0 = 0.0;
            double slopeX = 0.0;
            double elev0 = 0.0;
            double elevX = 0.0;
            Point3d pnt3d1, pnt3d2, pnt3dX;

            for (int i = 0; i < k; i++)
            {
                slope0 = segProps[i].SLOPE_AHEAD;
                if (i < k - 1)
                    slopeX = segProps[i + 1].SLOPE_AHEAD;

                switch (side)
                {
                    case 1: //right side

                        if (k == 1)
                        {
                            pnt3d1 = segProps[i].BEG.traverse(segProps[i].DIR_AHEAD + PI / 2 * side, offH, xSlope);
                            pnt3d2 = segProps[i].END.traverse(segProps[i].DIR_AHEAD + PI / 2 * side, offH, xSlope);
                            pnts3dRF.Add(pnt3d1);
                            pnts3dRF.Add(pnt3d2);
                            continue;
                        }

                        if (segProps[i].DELTA == 0)
                        {
                            pnt3d1 = segProps[i].BEG.traverse(segProps[i].DIR_AHEAD + PI / 2 * side, offH, xSlope);     //do beg pnt only; next segment will provide end / beg pnt
                            pnts3dRF.Add(pnt3d1);
                        }
                        else if (segProps[i].DELTA > 0)
                        {
                            pnt3d1 = segProps[i].BEG.traverse(segProps[i].DIR_AHEAD + PI / 2 * side, offH, xSlope); //point opposite begin pnt
                            pnt3d2 = segProps[i].END.traverse(segProps[i].DIR_AHEAD + PI / 2 * side, offH, xSlope); //point opposite end pnt
                            pnts3dRF.Add(pnt3d1);
                            pnts3dRF.Add(pnt3d2);

                            pnt3dX = Geom.doAnglePointOUT(segProps[i].BEG, xSlope, segProps[i].DELTA, segProps[i - 1].DIR_AHEAD, -1, offH);

                            dist = pnt3d2.getDistance(pnt3dX);
                            elev0 = pnt3d2.Z + dist * slope0;
                            elevX = segProps[i].END.Z + offV - dist * slopeX;

                            pnt3dX = new Point3d(pnt3dX.X, pnt3dX.Y, (elev0 + elevX) / 2);
                            pnts3dRF.Add(pnt3dX);
                        }
                        else if (segProps[i].DELTA < 0)
                        {
                            pnt3dX = Geom.doAnglePointIN(segProps[i].BEG, xSlope, System.Math.Abs(segProps[i].DELTA), segProps[i].DIR_AHEAD, -side, offH);
                            dist = offH * System.Math.Tan(segProps[i].DELTA);
                            elev0 = segProps[i].END.Z - dist * slope0 + offV;
                            elevX = segProps[i].END.Z + dist * slopeX + offV;
                            pnt3dX = new Point3d(pnt3dX.X, pnt3dX.Y, (elev0 + elevX) / 2);

                            pnt3d1 = segProps[i].END.traverse(segProps[i].DIR_AHEAD - PI, 4 + dist, -slope0);
                            pnt3d1 = pnt3d1.traverse(segProps[i].DIR_AHEAD + PI / 2 * side, offH, xSlope);

                            pnt3d2 = segProps[i].END.traverse(segProps[i + 1].DIR_AHEAD, 4 + dist, slopeX);
                            pnt3d2 = pnt3d2.traverse(segProps[i].DIR_AHEAD + PI / 2 * side, offH, xSlope);

                            pnts3dRF.Add(pnt3d1);
                            pnts3dRF.Add(pnt3dX);
                            pnts3dRF.Add(pnt3d2);
                        }

                        if (i == segProps.Count - 1)
                        {
                            if (segProps[i - 1].DELTA > 0)
                            { //need point opposite beg pnt
                                pnt3d1 = segProps[i].BEG.traverse(segProps[i].DIR_AHEAD + PI / 2 * side, offH, xSlope);
                                pnts3dRF.Add(pnt3d1);
                            }

                            pnt3d2 = segProps[i].END.traverse(segProps[i].DIR_AHEAD + PI / 2 * side, offH, xSlope);
                            pnts3dRF.Add(pnt3d2);
                        }

                        break;

                    case -1: //left side

                        if (k == 1)
                        {
                            pnt3d1 = segProps[i].BEG.traverse(segProps[i].DIR_AHEAD + PI / 2 * side, offH, xSlope);
                            pnt3d2 = segProps[i].END.traverse(segProps[i].DIR_AHEAD + PI / 2 * side, offH, xSlope);
                            pnts3dRF.Add(pnt3d1);
                            pnts3dRF.Add(pnt3d2);
                            continue;
                        }

                        if (segProps[i].DELTA == 0)
                        {
                            pnt3d1 = segProps[i].BEG.traverse(segProps[i].DIR_AHEAD + PI / 2 * side, offH, xSlope);     //do beg pnt only; next segment will provide end / beg pnt
                            pnts3dRF.Add(pnt3d1);
                        }
                        else if (segProps[i].DELTA > 0)
                        {
                            pnt3dX = Geom.doAnglePointIN(segProps[i].BEG, xSlope, segProps[i].DELTA, segProps[i].DIR_AHEAD, side, offH);
                            dist = offH * System.Math.Tan(segProps[i].DELTA);
                            elev0 = segProps[i].END.Z - dist * slope0 + offV;
                            elevX = segProps[i].END.Z + dist * slopeX + offV;
                            pnt3dX = new Point3d(pnt3dX.X, pnt3dX.Y, (elev0 + elevX) / 2);

                            pnt3d1 = segProps[i].END.traverse(segProps[i].DIR_AHEAD - PI, 4 + dist, -slope0);
                            pnt3d1 = pnt3d1.traverse(segProps[i].DIR_AHEAD + PI / 2 * side, offH, xSlope);

                            pnt3d2 = segProps[i].END.traverse(segProps[i + 1].DIR_AHEAD, 4 + dist, slopeX);
                            pnt3d2 = pnt3d2.traverse(segProps[i].DIR_AHEAD + PI / 2 * side, offH, xSlope);

                            pnts3dRF.Add(pnt3d1);
                            pnts3dRF.Add(pnt3dX);
                            pnts3dRF.Add(pnt3d2);
                        }
                        else if (segProps[i].DELTA < 0)
                        {
                            pnt3d1 = segProps[i].BEG.traverse(segProps[i].DIR_AHEAD + PI / 2 * side, offH, xSlope); //point opposite begin pnt
                            pnt3d2 = segProps[i].END.traverse(segProps[i].DIR_AHEAD + PI / 2 * side, offH, xSlope); //point opposite end pnt
                            pnts3dRF.Add(pnt3d1);
                            pnts3dRF.Add(pnt3d2);

                            pnt3dX = Geom.doAnglePointOUT(segProps[i].BEG, xSlope, System.Math.Abs(segProps[i].DELTA), segProps[i - 1].DIR_AHEAD, -side, offH);

                            dist = pnt3d2.getDistance(pnt3dX);
                            elev0 = pnt3d2.Z + dist * slope0;
                            elevX = segProps[i].END.Z + offV - dist * slopeX;

                            pnt3dX = new Point3d(pnt3dX.X, pnt3dX.Y, (elev0 + elevX) / 2);
                            pnts3dRF.Add(pnt3dX);
                        }

                        if (i == segProps.Count - 1)
                        {
                            if (segProps[i - 1].DELTA < 0)
                            { //need point opposite beg pnt
                                pnt3d1 = segProps[i].BEG.traverse(segProps[i].DIR_AHEAD + PI / 2 * side, offH, xSlope);
                                pnts3dRF.Add(pnt3d1);
                            }

                            pnt3d2 = segProps[i].END.traverse(segProps[i].DIR_AHEAD + PI / 2 * side, offH, xSlope);
                            pnts3dRF.Add(pnt3d2);
                        }

                        break;
                }
            }

            idPoly3dNew = pnts3dRF.addPoly3d(nameLayer);

            return idPoly3dNew;
        }

        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="tr"></param>
        /// <param name="openMode"></param>
        /// <returns></returns>
        public static IEnumerable<T>
        ofType<T>(this IEnumerable<ObjectId> enumerable, Transaction tr, OpenMode openMode) where T : Autodesk.AutoCAD.DatabaseServices.DBObject
        {
            RXClass rxclass = RXObject.GetClass(typeof(T));
            return from ObjectId objectId in enumerable
                   where objectId.ObjectClass.IsDerivedFrom(rxclass)
                   select (T)tr.GetObject(objectId, openMode);
        }

        public static void
        putOnTop(this ObjectId idSYM, ObjectId idTX, Object idWO = null)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord ms = Blocks.getBlockTableRecordMS();
                    DrawOrderTable dot = (DrawOrderTable)tr.GetObject(ms.DrawOrderTableId, OpenMode.ForWrite);
                    ObjectIdCollection ids = new ObjectIdCollection();
                    if (idWO != null)
                    {
                        ids.Add((ObjectId)idWO);
                        dot.MoveToTop(ids);
                        ids = new ObjectIdCollection();
                    }

                    ids.Add(idSYM);
                    ids.Add(idTX);
                    dot.MoveToTop(ids);

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 2507");
            }
        }

        public static string
        radianToDegreeString(this double rads, int dec = 2)
        {
            return Conv.RADtoBearing(rads);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="RB"></param>
        /// <returns></returns>
        public static List<Handle>
        rb_handles(this ResultBuffer RB)
        {
            TypedValue[] TVs = RB.AsArray();
            List<Object> objs = new List<Object>();
            List<Handle> handles = new List<Handle>();

            foreach (TypedValue TV in TVs)
            {
                if (TV.TypeCode == 1005)
                    if (TV.Value.ToString() != "0")
                        objs.Add(TV.Value.ToString());
            }
            foreach (Object obj in objs)
            {
                if (!handles.Contains(obj.ToString().stringToHandle()))
                    handles.Add(obj.ToString().stringToHandle());
            }
            return handles;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="poly"></param>
        public static void
        removeDuplicatVertex(this ObjectId idPoly)
        {
            Polyline poly = (Polyline)idPoly.getEnt();
            Misc.removeDuplicateVertex(poly);
        }

        public static void
        reversePolyX(this ObjectId id)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                    if (ent is Polyline)
                    {
                        Polyline poly = (Polyline)tr.GetObject(id, OpenMode.ForWrite);
                        poly.ReverseCurve();
                    }
                    else if (ent is Polyline3d)
                    {
                        Polyline3d poly3d = (Polyline3d)tr.GetObject(id, OpenMode.ForWrite);
                        poly3d.ReverseCurve();
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 2577");
            }
        }

        public static void
        scaleObj(this ObjectId id, double scaleFactor, Point3d pnt3dRef)
        {
            Mod.scaleObj(id, scaleFactor, pnt3dRef);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="poly3d"></param>
        /// <param name="pnt3d"></param>
        public static void
        setBegPnt(this Polyline3d poly3d, Point3d pnt3d)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Polyline3d p3d = (Polyline3d)tr.GetObject(poly3d.ObjectId, OpenMode.ForWrite);
                    ObjectId[] ids = p3d.Cast<ObjectId>().ToArray();
                    PolylineVertex3d v3d = (PolylineVertex3d)tr.GetObject(ids[0], OpenMode.ForWrite);
                    v3d.Position = pnt3d;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 2608");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="h"></param>
        /// <param name="pnt3d"></param>
        public static void
        setBegPnt(this Handle h, Point3d pnt3d)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ObjectId id = Db.handleToObjectId(HostApplicationServices.WorkingDatabase, h);
                    Polyline3d p3d = (Polyline3d)tr.GetObject(id, OpenMode.ForWrite);
                    ObjectId[] ids = p3d.Cast<ObjectId>().ToArray();

                    PolylineVertex3d v3d = (PolylineVertex3d)tr.GetObject(ids[0], OpenMode.ForWrite);
                    v3d.Position = pnt3d;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 2635");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="poly3d"></param>
        /// <param name="pnt3d"></param>
        public static void
        setEndPnt(this Polyline3d poly3d, Point3d pnt3d)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Polyline3d p3d = (Polyline3d)tr.GetObject(poly3d.ObjectId, OpenMode.ForWrite);
                    ObjectId[] ids = p3d.Cast<ObjectId>().ToArray();
                    PolylineVertex3d v3d = (PolylineVertex3d)tr.GetObject(ids[ids.Length - 1], OpenMode.ForWrite);
                    v3d.Position = pnt3d;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 2660");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="h"></param>
        /// <param name="pnt3d"></param>
        public static void
        setEndPnt(this Handle h, Point3d pnt3d)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ObjectId id = Db.handleToObjectId(HostApplicationServices.WorkingDatabase, h);
                    Polyline3d p3d = (Polyline3d)tr.GetObject(id, OpenMode.ForWrite);
                    ObjectId[] ids = p3d.Cast<ObjectId>().ToArray();
                    PolylineVertex3d v3d = (PolylineVertex3d)tr.GetObject(ids[ids.Length - 1], OpenMode.ForWrite);
                    v3d.Position = pnt3d;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 2686");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="nameLayer"></param>
        public static void
        setLayer(this ObjectId id, string nameLayer)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                    try
                    {
                        Layer.manageLayers(nameLayer);
                        ent.Layer = nameLayer;
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 2710");
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 2717");
            }
        }

        public static double
        setMTextWidth(this ObjectId id, bool left_justify, out Point3d pnt3dLoc)
        {
            return Txt.setMTextWidth(id, left_justify, out pnt3dLoc);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nameApp"></param>
        /// <param name="type"></param>
        /// <param name="val"></param>
        public static void
        setXData(this ObjectId id, List<short> type, List<Object> val, string nameApp)
        {
            xData.addXData(id, nameApp, type, val);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nameApp"></param>
        /// <param name="RB"></param>
        public static void
        setXData(this ObjectId id, TypedValue[] tvs, string nameApp)
        {
            xData.addXData(id, tvs, nameApp);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="handles"></param>
        /// <param name="nameApp"></param>
        public static void
        setXData(this ObjectId id, List<Handle> handles, string nameApp)
        {
            id.clearXData(nameApp);
            TypedValue[] tvs = new TypedValue[handles.Count + 1];
            tvs.SetValue(new TypedValue(1001, nameApp), 0);
            for (int i = 0; i < handles.Count; i++)
            {
                tvs.SetValue(new TypedValue(1005, handles[i]), i + 1);
            }
            id.setXData(tvs, nameApp);
        }

        public static void
        setXData(this ObjectId id, Handle handle, string nameApp)
        {
            id.clearXData(nameApp);
            TypedValue[] tvs = new TypedValue[2];
            tvs.SetValue(new TypedValue(1001, nameApp), 0);
            tvs.SetValue(new TypedValue(1005, handle), 1);
            id.setXData(tvs, nameApp);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="poly3d"></param>
        /// <param name="nameLayer"></param>
        /// <returns></returns>
        public static ObjectId
        toPolyline(this ObjectId idPoly3d, string nameLayer)
        {
            return Conv.poly3d_Poly(idPoly3d, nameLayer);
        }

        public static Point2d
        traverse(this Point2d pnt2dBASE, double angle, double length)
        {
            Point2d pnt3dTAR = Geom.traverse_pnt2d(pnt2dBASE, angle, length);
            return pnt3dTAR;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnt3dBASE"></param>
        /// <param name="angle"></param>
        /// <param name="length"></param>
        /// <param name="slope"></param>
        /// <returns></returns>
        public static Point3d
        traverse(this Point3d pnt3dBASE, double angle, double length, double slope = 0)
        {
            Point3d pnt3dTAR = Geom.traverse_pnt3d(pnt3dBASE, angle, length, slope);
            return pnt3dTAR;
        }

        public static List<ObjectId>
        tvs_ObjectIDs(this TypedValue[] tvs)
        {
            List<ObjectId> ids = new List<ObjectId>();
            foreach (TypedValue tv in tvs)
            {
                if (tv.TypeCode == 1005)
                    if (tv.Value.ToString() != "0")
                    {
                        ObjectId id = tv.Value.ToString().stringToHandle().getObjectId();
                        ids.Add(id);
                    }
            }
            return ids;
        }

        public static void
        updateAttribute(this ObjectId id, string tag, string value)
        {
            AttributeCollection attColl = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockReference br = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)br.BlockTableRecord.GetObject(OpenMode.ForRead);
                    using (btr)
                    {
                        attColl = br.AttributeCollection;
                        foreach (AttributeDefinition ad in attColl)
                        {
                            if (ad.Tag == tag)
                            {
                                btr.UpgradeOpen();
                                ad.TextString = value;
                            }
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 2869");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        public static void
        updateGraphics(this Document doc)
        {
            BaseObjs._transactionManagerDoc.EnableGraphicsFlush(true);
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDoc())
                {
                    BlockTableRecord ms = Blocks.getBlockTableRecordMS();
                    foreach (ObjectId id in ms)
                    {
                        Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                        BaseObjs._transactionManagerDoc.QueueForGraphicsFlush();
                        BaseObjs._transactionManagerDoc.FlushGraphics();
                        BaseObjs._editor.UpdateScreen();
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 2898");
            }
        }

        /// From Swamp Rat
        /// <summary>
        /// A specialization that opens an Entity for write,
        /// and calls its RecordGrahicsModified() method.
        /// </summary>
        /// <param name="id"></param>
        public static void
        updateGraphics(this ObjectId id)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    DBObject dbObj = tr.GetObject(id, OpenMode.ForWrite);
                    ((Entity)dbObj).RecordGraphicsModified(true);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 2922");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="index"></param>
        /// <param name="pnt3d"></param>
        public static void
        updateVertex(this ObjectId id, int index, Point3d pnt3d)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                    if (ent is Polyline3d)
                    {
                        Polyline3d poly3d = (Polyline3d)ent;
                        poly3d.UpgradeOpen();
                        int i = 0;
                        foreach (ObjectId idV in poly3d)
                        {
                            PolylineVertex3d v3d = (PolylineVertex3d)tr.GetObject(idV, OpenMode.ForRead);
                            if (i == index)
                            {
                                v3d.UpgradeOpen();
                                v3d.Position = pnt3d;
                                break;
                            }
                            i++;
                        }
                    }
                    else if (ent is Leader)
                    {
                        Leader ldr = (Leader)ent;
                        ldr.UpgradeOpen();
                        ldr.SetVertexAt(index, pnt3d);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " ExtensionMethods.cs: line: 2968");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int
        wordCount(this String str)
        {
            return str.Split(new char[] {
                ' ',
                '.',
                '?'
            },
                StringSplitOptions.RemoveEmptyEntries).Length;
        }
    }
}
