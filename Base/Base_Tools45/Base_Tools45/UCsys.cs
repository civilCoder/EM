using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static class UCsys
    {
        public static Matrix3d
        addUCS(Point3d pnt3dOrigin, Point3d pnt3dXaxis, string nameUCS)
        {
            Matrix3d newMatrix = new Matrix3d();
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    // Open the UCS table for read
                    UcsTable ucsT = tr.GetObject(BaseObjs._db.UcsTableId, OpenMode.ForRead) as UcsTable;
                    UcsTableRecord ucstr;

                    Vector3d v3dXaxis = new Vector3d(pnt3dXaxis.X - pnt3dOrigin.X, pnt3dXaxis.Y - pnt3dOrigin.Y, pnt3dXaxis.Z - pnt3dOrigin.Z);
                    Vector3d v3dYaxis = v3dXaxis.GetPerpendicularVector();
                    Vector3d v3dZ = v3dXaxis.CrossProduct(v3dYaxis);
                    Vector3d v3dY = v3dZ.CrossProduct(v3dXaxis);

                    Vector3d v3dPerpY = new Vector3d(v3dY.X + pnt3dOrigin.X, v3dY.Y + pnt3dOrigin.Y, v3dY.Z + pnt3dOrigin.Z);

                    // Check to see if the nameUCS UCS table record exists
                    if (ucsT.Has(nameUCS) == false)
                    {
                        ucstr = new UcsTableRecord();
                        ucstr.Name = nameUCS;

                        // Open the UCSTable for write
                        ucsT.UpgradeOpen();

                        // Add the new UCS table record
                        ucsT.Add(ucstr);
                        tr.AddNewlyCreatedDBObject(ucstr, true);
                    }
                    else
                    {
                        ucstr = (UcsTableRecord)tr.GetObject(ucsT[nameUCS], OpenMode.ForWrite);
                    }

                    ucstr.Origin = pnt3dOrigin;
                    ucstr.XAxis = v3dXaxis;
                    //ucstr.YAxis = v3dPerpY;
                    ucstr.YAxis = v3dYaxis;

                    // Open the active viewport
                    ViewportTableRecord vptr;
                    vptr = (ViewportTableRecord)tr.GetObject(BaseObjs._editor.ActiveViewportId, OpenMode.ForWrite);

                    // Display the UCS Icon at the origin of the current viewport
                    vptr.IconAtOrigin = true;
                    vptr.IconEnabled = true;

                    // Set the UCS current
                    vptr.SetUcs(ucstr.ObjectId);
                    BaseObjs._editor.UpdateTiledViewportsFromDatabase();

                    newMatrix = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis,
                        vptr.Ucs.Origin, vptr.Ucs.Xaxis, vptr.Ucs.Yaxis, vptr.Ucs.Zaxis);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " UCsys.cs: line: 75");
            }
            return newMatrix;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnt3dOrigin"></param>
        /// <param name="pnt3dXaxis"></param>
        /// <param name="pnt3dYaxis"></param>
        /// <param name="nameUCS"></param>
        /// <returns></returns>
        public static Matrix3d
        addUCS(Point3d pnt3dOrigin, Point3d pnt3dXaxis, Point3d pnt3dYaxis, string nameUCS)
        {
            Matrix3d newMatrix = new Matrix3d();
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    // Open the UCS table for read
                    UcsTable ucsT = tr.GetObject(BaseObjs._db.UcsTableId, OpenMode.ForRead) as UcsTable;
                    UcsTableRecord ucstr;

                    Vector3d vector3dXaxis = new Vector3d(pnt3dXaxis.X - pnt3dOrigin.X, pnt3dXaxis.Y - pnt3dOrigin.Y, pnt3dXaxis.Z - pnt3dOrigin.Z);
                    Vector3d vector3dYaxis = new Vector3d(pnt3dYaxis.X - pnt3dOrigin.X, pnt3dYaxis.Y - pnt3dOrigin.Y, pnt3dYaxis.Z - pnt3dOrigin.Z);

                    Vector3d vector3dZ = vector3dXaxis.CrossProduct(vector3dYaxis);
                    Vector3d vector3dY = vector3dZ.CrossProduct(vector3dXaxis);

                    Vector3d vector3dPerpY = new Vector3d(vector3dY.X + pnt3dOrigin.X, vector3dY.Y + pnt3dOrigin.Y, vector3dY.Z + pnt3dOrigin.Z);

                    // Check to see if the nameUCS UCS table record exists
                    if (ucsT.Has(nameUCS) == false)
                    {
                        ucstr = new UcsTableRecord();
                        ucstr.Name = nameUCS;

                        // Open the UCSTable for write
                        ucsT.UpgradeOpen();

                        // Add the new UCS table record
                        ucsT.Add(ucstr);
                        tr.AddNewlyCreatedDBObject(ucstr, true);
                    }
                    else
                    {
                        ucstr = (UcsTableRecord)tr.GetObject(ucsT[nameUCS], OpenMode.ForWrite);
                    }

                    ucstr.Origin = pnt3dOrigin;
                    ucstr.XAxis = vector3dXaxis;
                    ucstr.YAxis = vector3dPerpY;

                    // Open the active viewport
                    ViewportTableRecord vptr;
                    vptr = (ViewportTableRecord)tr.GetObject(BaseObjs._editor.ActiveViewportId, OpenMode.ForWrite);

                    // Display the UCS Icon at the origin of the current viewport
                    vptr.IconAtOrigin = true;
                    vptr.IconEnabled = true;

                    // Set the UCS current
                    vptr.SetUcs(ucstr.ObjectId);
                    BaseObjs._editor.UpdateTiledViewportsFromDatabase();

                    newMatrix = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis,
                        vptr.Ucs.Origin, vptr.Ucs.Xaxis, vptr.Ucs.Yaxis, vptr.Ucs.Zaxis);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " UCsys.cs: line: 149");
            }
            return newMatrix;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ObjectId
        addUcsTableRecord(string name)
        {
            ObjectId id = ObjectId.Null;
            UcsTableRecord Ucstr = new UcsTableRecord();
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    UcsTable UcsT = getUcsTable();
                    try
                    {
                        Ucstr.Name = name;
                        UcsT.Add(Ucstr);
                        tr.AddNewlyCreatedDBObject(Ucstr, true);
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " UCsys.cs: line: 177");
                    }
                    tr.Commit();
                    id = Ucstr.ObjectId;
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " UCsys.cs: line: 185");
            }
            return id;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnts3d"></param>
        /// <returns></returns>
        public static List<double>
        getObjectExtents(Point3dCollection pnts3d)
        {
            List<double> dblPntsX = new List<double>();
            List<double> dblPntsY = new List<double>();

            for (int i = 0; i < pnts3d.Count; i++)
            {
                dblPntsX.Add(pnts3d[i].X);
                dblPntsY.Add(pnts3d[i].Y);
            }

            double dblPntXmin = dblPntsX.Min();
            double dblPntYmin = dblPntsY.Min();

            double dblPntXmax = dblPntsX.Max();
            double dblPntYmax = dblPntsY.Max();

            double dblDeltaX = dblPntXmax - dblPntXmin;
            double dblDeltaY = dblPntYmax - dblPntYmin;

            List<double> delta = new List<double>();

            delta.Add(dblPntXmin);
            delta.Add(dblPntYmin);
            delta.Add(dblDeltaX);
            delta.Add(dblDeltaY);
            return delta;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static UcsTable
        getUcsTable()
        {
            UcsTable ucsTbl = null;
            Database DB = BaseObjs._db;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ucsTbl = (UcsTable)tr.GetObject(DB.UcsTableId, OpenMode.ForRead);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " UCsys.cs: line: 244");
            }
            return ucsTbl;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static UcsTableRecord
        getUcsTableRecord(string name)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    UcsTable UcsT = getUcsTable();
                    if (UcsT.Has(name) == true)
                    {
                        UcsTableRecord Ucstr = (UcsTableRecord)tr.GetObject(UcsT[name], OpenMode.ForRead);
                        return Ucstr;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " UCsys.cs: line: 272");
            }
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        public static void
        setUCS2Object(Point3d pnt3dBase0, Point3d pnt3dBaseX, Point3d pnt3dBaseY)
        {
            Vector3d vec3dXaxis = new Vector3d(pnt3dBaseX.X - pnt3dBase0.X, pnt3dBaseX.Y - pnt3dBase0.Y, pnt3dBaseX.Z - pnt3dBase0.Z);
            Vector3d vec3dYaxis = new Vector3d(pnt3dBaseY.X - pnt3dBase0.X, pnt3dBaseY.Y - pnt3dBase0.Y, pnt3dBaseY.Z - pnt3dBase0.Z);

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    // Open the UCS table for read
                    UcsTable acUCSTbl = getUcsTable();
                    UcsTableRecord acUCSTblRec;

                    // Check to see if the "New_UCS" UCS table record exists
                    if (acUCSTbl.Has("Temp") == false)
                    {
                        acUCSTblRec = new UcsTableRecord();
                        acUCSTblRec.Name = "Temp";

                        // Open the UCSTable for write
                        acUCSTbl.UpgradeOpen();

                        // Add the new UCS table record
                        acUCSTbl.Add(acUCSTblRec);
                        tr.AddNewlyCreatedDBObject(acUCSTblRec, true);
                    }
                    else
                    {
                        acUCSTblRec = tr.GetObject(acUCSTbl["Temp"], OpenMode.ForWrite) as UcsTableRecord;
                    }

                    acUCSTblRec.Origin = pnt3dBase0;
                    acUCSTblRec.XAxis = vec3dXaxis;
                    acUCSTblRec.YAxis = vec3dYaxis;

                    // Open the active viewport
                    ViewportTableRecord acVportTblRec;
                    acVportTblRec = tr.GetObject(BaseObjs._editor.ActiveViewportId, OpenMode.ForWrite) as ViewportTableRecord;
                    // Set the UCS current
                    acVportTblRec.SetUcs(acUCSTblRec.ObjectId);
                    BaseObjs._editor.UpdateTiledViewportsFromDatabase();

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " UCsys.cs: line: 328");
            }
        }

        public static void
        setUCS2World()
        {
            try
            {
                Editor ed = BaseObjs._editor;
                ed.CurrentUserCoordinateSystem = new Matrix3d(new double[16] {
                    1.0, 0.0, 0.0, 0.0,
                    0.0, 1.0, 0.0, 0.0,
                    0.0, 0.0, 1.0, 0.0,
                    0.0, 0.0, 0.0, 1.0
                });
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " UCsys.cs: line: 347");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static Point3dCollection
        TranslateCoordinates(Point3dCollection pnts3d, Point3d pnt3dBase, double angle)
        {
            Point3dCollection pnts3dTrans = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    pnts3dTrans = new Point3dCollection();

                    // Translate the OCS to WCS
                    Matrix3d matrx3d = BaseObjs._editor.CurrentUserCoordinateSystem;
                    CoordinateSystem3d coordSys = matrx3d.CoordinateSystem3d;

                    foreach (Point3d pnt3d in pnts3d)
                    {
                        pnts3dTrans.Add(pnt3d.TransformBy(Matrix3d.Rotation(angle, coordSys.Zaxis, pnt3dBase)));
                    }

                    // Save the new objects to the database
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " UCsys.cs: line: 379");
            }
            return pnts3dTrans;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnt3dOrigin"></param>
        /// <param name="pnt3dXaxis"></param>
        /// <param name="pnt3dYaxis"></param>
        /// <returns></returns>
        public static Matrix3d
        UCS2WCS(Point3d pnt3dOrigin, Point3d pnt3dXaxis, Point3d pnt3dYaxis)
        {
            Matrix3d newMatrix = new Matrix3d();
            Vector3d vector3dXaxis = new Vector3d(pnt3dXaxis.X - pnt3dOrigin.X, pnt3dXaxis.Y - pnt3dOrigin.Y, pnt3dXaxis.Z - pnt3dOrigin.Z);
            Vector3d vector3dYaxis = new Vector3d(pnt3dYaxis.X - pnt3dOrigin.X, pnt3dYaxis.Y - pnt3dOrigin.Y, pnt3dYaxis.Z - pnt3dOrigin.Z);

            Vector3d vector3dZ = vector3dXaxis.CrossProduct(vector3dYaxis);
            Vector3d vector3dY = vector3dZ.CrossProduct(vector3dXaxis);

            Vector3d vector3dPerpY = new Vector3d(vector3dY.X + pnt3dOrigin.X, vector3dY.Y + pnt3dOrigin.Y, vector3dY.Z + pnt3dOrigin.Z);

            newMatrix = Matrix3d.AlignCoordinateSystem(pnt3dOrigin, vector3dXaxis, vector3dPerpY, Vector3d.ZAxis,
                Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis);
            return newMatrix;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pnt3dOrigin"></param>
        /// <param name="pnt3dXaxis"></param>
        /// <param name="pnt3dYaxis"></param>
        /// <returns></returns>
        public static Matrix3d
        WCS2UCS(Point3d pnt3dOrigin, Point3d pnt3dXaxis, Point3d pnt3dYaxis)
        {
            Matrix3d newMatrix = new Matrix3d();
            Vector3d vector3dXaxis = new Vector3d(pnt3dXaxis.X - pnt3dOrigin.X, pnt3dXaxis.Y - pnt3dOrigin.Y, pnt3dXaxis.Z - pnt3dOrigin.Z);
            Vector3d vector3dYaxis = new Vector3d(pnt3dYaxis.X - pnt3dOrigin.X, pnt3dYaxis.Y - pnt3dOrigin.Y, pnt3dYaxis.Z - pnt3dOrigin.Z);

            Vector3d vector3dZ = vector3dXaxis.CrossProduct(vector3dYaxis);
            Vector3d vector3dY = vector3dZ.CrossProduct(vector3dXaxis);

            Vector3d vector3dPerpY = new Vector3d(vector3dY.X + pnt3dOrigin.X, vector3dY.Y + pnt3dOrigin.Y, vector3dY.Z + pnt3dOrigin.Z);

            newMatrix = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis,
                pnt3dOrigin, vector3dXaxis, vector3dPerpY, Vector3d.ZAxis);
            return newMatrix;
        }
    }
}
