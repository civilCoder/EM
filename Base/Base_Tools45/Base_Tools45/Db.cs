using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Diagnostics;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using DBObject = Autodesk.AutoCAD.DatabaseServices.DBObject;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Base_Tools45
{
    /// <summary>
    /// Summary description for Db.
    /// </summary>
    public static class Db
    {
        public static void
        cloneAndXformObjects(Database db, ObjectIdCollection entsToClone,
            ObjectId ownerBlockId, Matrix3d xformMat)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    IdMapping idMap = new IdMapping();
                    db.DeepCloneObjects(entsToClone, ownerBlockId, idMap, false);

                    // walk through all the cloned objects and Xform any of the entities
                    foreach (IdPair idpair in idMap)
                    {
                        if (idpair.IsCloned)
                        {
                            DBObject clonedObj = tr.GetObject(idpair.Value, OpenMode.ForWrite);
                            Entity clonedEnt = clonedObj as Entity;
                            if (clonedEnt != null)
                                clonedEnt.TransformBy(xformMat);
                        }
                    }

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Db.cs: line: 46");
            }
        }

        public static string
        dbObjToRxClassAndHandleStr(DBObject dbObj)
        {
            Debug.Assert(dbObj != null);

            string str1 = objToRxClassStr(dbObj);
            return string.Format("< {0,-25} {1,4} >", str1, dbObj.Handle.ToString());
        }

        /// <summary>
        /// shortcut for getting the current DWG's database
        /// </summary>
        /// <returns>Database for the current drawing</returns>
        public static Database
        getDB()
        {
            Database db = BaseObjs._db;
            return db;
        }

        /// <summary>
        /// Is Paper Space active in the given database?
        /// </summary>
        /// <param name="db">Specific database to use</param>
        /// <returns></returns>
        public static Matrix3d
        getEcsToWcsMatrix(Point3d origin, Vector3d zAxis)
        {
            const double kArbBound = 0.015625;         //  1/64th

            // short circuit if in WCS already
            if (zAxis == Ge.kZAxis)
            {
                return Matrix3d.Identity;
            }

            Vector3d xAxis, yAxis;

            Debug.Assert(zAxis.IsUnitLength(Tolerance.Global));

            if ((System.Math.Abs(zAxis.X) < kArbBound) && (System.Math.Abs(zAxis.Y) < kArbBound))
                xAxis = Ge.kYAxis.CrossProduct(zAxis);
            else
                xAxis = Ge.kZAxis.CrossProduct(zAxis);

            xAxis = xAxis.GetNormal();
            yAxis = zAxis.CrossProduct(xAxis).GetNormal();

            return Matrix3d.AlignCoordinateSystem(Ge.kOrigin, Ge.kXAxis, Ge.kYAxis, Ge.kZAxis,
                origin, xAxis, yAxis, zAxis);
        }

        public static Vector3d
        getEcsXAxis(Vector3d ecsZAxis)
        {
            Matrix3d arbMat = getEcsToWcsMatrix(Ge.kOrigin, ecsZAxis);

            return arbMat.CoordinateSystem3d.Xaxis;
        }

        public static Matrix3d
        getUcsMatrix(Database db)
        {
            Debug.Assert(db != null);

            Point3d origin;
            Vector3d xAxis, yAxis, zAxis;

            if (isPaperSpace(db))
            {
                origin = db.Pucsorg;
                xAxis = db.Pucsxdir;
                yAxis = db.Pucsydir;
            }
            else
            {
                origin = db.Ucsorg;
                xAxis = db.Ucsxdir;
                yAxis = db.Ucsydir;
            }

            zAxis = xAxis.CrossProduct(yAxis);

            return Matrix3d.AlignCoordinateSystem(Ge.kOrigin, Ge.kXAxis, Ge.kYAxis, Ge.kZAxis, origin, xAxis, yAxis, zAxis);
        }

        public static Plane
        getUcsPlane(Database db)
        {
            Matrix3d m = getUcsMatrix(db);
            CoordinateSystem3d coordSys = m.CoordinateSystem3d;
            return new Plane(coordSys.Origin, coordSys.Xaxis, coordSys.Yaxis);
        }

        public static Matrix3d
        getUcsToWcsOriginMatrix(Point3d wcsBasePt, Database db)
        {
            Matrix3d m = getUcsMatrix(db);

            Point3d origin = m.CoordinateSystem3d.Origin;
            origin += wcsBasePt.GetAsVector();

            m = Matrix3d.AlignCoordinateSystem(origin,
                m.CoordinateSystem3d.Xaxis,
                m.CoordinateSystem3d.Yaxis,
                m.CoordinateSystem3d.Zaxis,
                Ge.kOrigin, Ge.kXAxis, Ge.kYAxis, Ge.kZAxis);

            return m;
        }

        public static Vector3d
        getUcsXAxis(Database db)
        {
            Matrix3d m = getUcsMatrix(db);

            return m.CoordinateSystem3d.Xaxis;
        }

        public static Vector3d
        getUcsYAxis(Database db)
        {
            Matrix3d m = getUcsMatrix(db);

            return m.CoordinateSystem3d.Yaxis;
        }

        public static Vector3d
        getUcsZAxis(Database db)
        {
            Matrix3d m = getUcsMatrix(db);

            return m.CoordinateSystem3d.Zaxis;
        }

        public static Entity
        handleToObject(String strHandle)
        {
            Entity ent = null;
            Database db = getDB();
            ObjectId id = ObjectId.Null;
            Handle h = stringToHandle(strHandle);

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    try
                    {
                        id = handleToObjectId(db, h);
                        try
                        {
                            ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                        }
                        catch (System.Exception ex)
                        {
                BaseObjs.writeDebug(ex.Message + " Db.cs: line: 206");
                        }
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " Db.cs: line: 211");
                    }

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Db.cs: line: 219");
            }
            return ent;
        }

        public static ObjectId
        handleToObjectId(Database db, Handle h)
        {
            ObjectId id = ObjectId.Null;
            try
            {
                id = db.GetObjectId(false, h, 0);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Db.cs: line: 234");
            }
            return id;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Handle
        idObjToHandle(ObjectId id)
        {
            Handle h = "0000".stringToHandle();
            if (id == ObjectId.Null)
                return h;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    if (id.IsValid && (!id.IsEffectivelyErased || !id.IsErased))
                    {
                        DBObject dbObj = tr.GetObject(id, OpenMode.ForRead);
                        h = dbObj.Handle;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Db.cs: line: 265");
            }
            return h;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Polyline3d
        idObjToPoly3d(ObjectId id)
        {
            Polyline3d poly3d = null;
            try
            {
                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        poly3d = (Polyline3d)tr.GetObject(id, OpenMode.ForRead);
                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " Db.cs: line: 291");
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Db.cs: line: 296");
            }
            return poly3d;
        }

        public static string
        idObjToRxClassAndHandleStr(ObjectId objId)
        {
            string str = "";

            if (objId.IsNull)
                str = "(null)";
            else
            {
                // open up even if erased
                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        DBObject tmpObj = tr.GetObject(objId, OpenMode.ForRead, true);
                        str = dbObjToRxClassAndHandleStr(tmpObj);
                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " Db.cs: line: 322");
                }
            }

            return str;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        public static string
        idObjToType(ObjectId objId)
        {
            string type = string.Empty;
            {
                // open up even if erased
                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        DBObject tmpObj = tr.GetObject(objId, OpenMode.ForRead, true);
                        type = tmpObj.GetType().Name;
                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " Db.cs: line: 351");
                }
            }
            return type;
        }

        public static string
        idObjToTypeAndHandleStr(DBObject dbObj)
        {
            Debug.Assert(dbObj != null);

            string str1 = dbObj.GetType().Name;
            return string.Format("< {0,-20} {1,4} >", str1, dbObj.Handle.ToString());
        }

        public static string
        idObjToTypeAndHandleStr(ObjectId objId)
        {
            string str = "";

            if (objId.IsNull)
                str = "(null)";
            else
            {
                // open up even if erased
                Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = objId.Database.TransactionManager;
                try
                {
                    using (Autodesk.AutoCAD.DatabaseServices.Transaction tr = tm.StartTransaction())
                    {
                        DBObject tmpObj = tr.GetObject(objId, OpenMode.ForRead, true);
                        str = idObjToTypeAndHandleStr(tmpObj);
                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " Db.cs: line: 388");
                }
            }

            return str;
        }

        public static bool
        isPaperSpace(Database db)
        {
            Debug.Assert(db != null);

            if (db.TileMode)
                return false;

            Editor ed = BaseObjs._editor;
            if (db.PaperSpaceVportId == ed.CurrentViewportObjectId)
                return true;

            return false;
        }

        public static void
        makePointEnt(Point3d pt, int colorIndex, Database db)
        {
            int mode = (int)AcadApp.GetSystemVariable("pdmode");
            if (mode == 0)
                AcadApp.SetSystemVariable("pdmode", 99);

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    using (DBPoint dbPt = new DBPoint(pt))
                    {
                        dbPt.ColorIndex = colorIndex;
                        SymTbl.AddToCurrentSpace(dbPt, db);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Db.cs: line: 431");
            }
        }

        public static void
        makeRayEnt(Point3d basePt, Vector3d unitDir, int colorIndex, Database db)
        {
            if (unitDir.IsZeroLength())
            {
                Editor ed = BaseObjs._editor;
                ed.WriteMessage("\nSkipping zero length vector (colorIndex = {0:d})", colorIndex);
                return;
            }

            try
            {
                using (Ray ray = new Ray())
                {
                    ray.ColorIndex = colorIndex;
                    ray.BasePoint = basePt;
                    ray.UnitDir = unitDir;
                    SymTbl.AddToCurrentSpace(ray, db);
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Db.cs: line: 457");
            }
        }

        public static string
        objToRxClassStr(RXObject obj)
        {
            Debug.Assert(obj != null);

            RXClass rxClass = obj.GetRXClass();
            if (rxClass == null)
            {
                Debug.Assert(false);
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("AcRxObject class has not called rxInit()!");
                return "*Unknown*";
            }
            else
                return rxClass.Name;
        }

        public static Handle
        stringToHandle(this String strHandle)
        {
            Handle handle = new Handle();

            try
            {
                Int64 nHandle = Convert.ToInt64(strHandle, 16);
                handle = new Handle(nHandle);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Db.cs: line: 489");
            }
            return handle;
        }

        public static void
        transformToWcs(this ObjectId id, Database db)
        {
            if (id == ObjectId.Null)
                return;
            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                Matrix3d m = getUcsMatrix(db);
                ent.TransformBy(m);
                tr.Commit();
            }
        }

        public static void
        transformToWcs(DBObjectCollection ents, Database db)
        {
            Debug.Assert(ents != null);
            Debug.Assert(db != null);

            Matrix3d m = getUcsMatrix(db);

            foreach (Entity tmpEnt in ents)
            {
                Debug.Assert(tmpEnt.IsWriteEnabled);
                tmpEnt.TransformBy(m);
            }
        }

        public static Point3d
        ucsToWcs(Point3d pt)
        {
            Matrix3d m = getUcsMatrix(getDB());

            return pt.TransformBy(m);
        }

        /// <summary>
        /// Get a transformed copy of a vector from UCS to WCS
        /// </summary>
        /// <param name="vec">Vector to transform</param>
        /// <returns>Transformed copy of vector</returns>
        public static Vector3d
        ucsToWcs(Vector3d vec)
        {
            Matrix3d m = getUcsMatrix(getDB());

            return vec.TransformBy(m);
        }

        /// <summary>
        /// Get a transformed copy of a point from WCS to UCS
        /// </summary>
        /// <param name="pt">Point to transform</param>
        /// <returns>Transformed copy of point</returns>
        public static Point3d
        wcsToUcs(Point3d pt)
        {
            Matrix3d m = getUcsMatrix(getDB());

            return pt.TransformBy(m.Inverse());
        }
    }
}
