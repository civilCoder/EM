using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using System;
using System.Collections.Generic;
using DBObject = Autodesk.AutoCAD.DatabaseServices.DBObject;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static class xData
    {
        public static void
        addXData(Entity ent, string nameApp, List<short> type, List<object> val)
        {
            TypedValue[] TVs = new TypedValue[type.Count];
            for (int i = 0; i < type.Count; i++)
            {
                TVs.SetValue(new TypedValue(type[i], val[i]), i);
            }
            addXData(ent.ObjectId, TVs, nameApp);
        }

        public static void
        addXData(ObjectId id, TypedValue[] tvs, string nameApp)
        {
            AddRegAppTableRecord(nameApp);
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    try
                    {
                        ResultBuffer RB = new ResultBuffer(tvs);
                        DBObject dbObj = tr.GetObject(id, OpenMode.ForWrite);
                        dbObj.XData = RB;
                        RB.Dispose();
                        tr.Commit();
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " xData.cs: line: 45");
                    }
                }//end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xData.cs: line: 51");
            }
        }

        public static void
        addXData(ObjectId id, string nameApp, List<short> type, List<object> val)
        {
            TypedValue[] TVs = new TypedValue[type.Count];
            for (int i = 0; i < type.Count; i++)
            {
                TVs.SetValue(new TypedValue(type[i], val[i]), i);
            }
            addXData(id, TVs, nameApp);
        }

        public static ResultBuffer
        checkPointXData2Nodes(ObjectId idCogoPnt, ResultBuffer RBpnt, string nameApp)
        {
            try
            {
                List<Handle> handlesP3d = RBpnt.rb_handles();

                int i = 0;
                //Working with a list of unique non-zero handles
                foreach (Handle hP3d in handlesP3d)
                {
                    //GET BREAKLINE PER STORED HANDLES
                    Polyline3d poly3d = (Polyline3d)Db.handleToObject(hP3d.ToString());

                    if ((poly3d != null))
                    {
                        if (poly3d.Length != 0)
                        {
                            ResultBuffer RBp3d = poly3d.ObjectId.getXData(nameApp);
                            List<Handle> handlesPnts = RBp3d.rb_handles();
                            Handle hPnt = idCogoPnt.getHandle();
                            // if pnt handle is in poly3d handle list then check coordinates otherwise
                            Point3dCollection pnts3d = poly3d.getCoordinates3d();
                            Point3d pnt3dCogoPnt = hPnt.getCogoPntCoordinates();

                            if (System.Math.Round(pnts3d[0].X, 3) == System.Math.Round(pnt3dCogoPnt.X, 3) &&
                                System.Math.Round(pnts3d[0].Y, 3) == System.Math.Round(pnt3dCogoPnt.Y, 3))
                            {
                                if (pnts3d[0].Z != pnt3dCogoPnt.Z)
                                {
                                    poly3d.setBegPnt(pnt3dCogoPnt);
                                }
                                if (!handlesPnts.Contains(hPnt))
                                {
                                    handlesPnts.Add(hPnt);
                                    poly3d.ObjectId.setXData(handlesPnts, nameApp);
                                }
                            }
                            else if (System.Math.Round(pnts3d[1].X, 3) == System.Math.Round(pnt3dCogoPnt.X, 3) &&
                                     System.Math.Round(pnts3d[1].Y, 3) == System.Math.Round(pnt3dCogoPnt.Y, 3))
                            {
                                if (pnts3d[1].Z != pnt3dCogoPnt.Z)
                                {
                                    poly3d.setEndPnt(pnt3dCogoPnt);
                                }
                                if (!handlesPnts.Contains(hPnt))
                                {
                                    handlesPnts.Add(hPnt);
                                    poly3d.ObjectId.setXData(handlesPnts, nameApp);
                                }
                            }
                            else
                            {
                                poly3d.ObjectId.delete();
                                handlesP3d.RemoveAt(i);
                                RBpnt = handlesP3d.handles_RB(nameApp);
                                idCogoPnt.setXData(handlesP3d, nameApp);
                            }
                        }
                        else
                        {
                            poly3d.ObjectId.delete();
                            handlesP3d.RemoveAt(i);
                            RBpnt = handlesP3d.handles_RB(nameApp);
                            idCogoPnt.setXData(handlesP3d, nameApp);
                        }
                    }
                    else
                    {
                        //remove bad handle from list
                        handlesP3d.RemoveAt(i);
                        RBpnt = handlesP3d.handles_RB(nameApp);
                        idCogoPnt.setXData(handlesP3d, nameApp);
                    }
                    i++;
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xData.cs: line: 145");
            }
            return RBpnt;
        }

        public static void
        checkPointXDataXNodes(ObjectId idCgPnt, string nameApp)
        {
            try
            {
                ResultBuffer rbo = idCgPnt.getXData(nameApp);
                if (rbo != null)
                {
                    //CLEAR XDATA
                    List<Handle> handles = rbo.rb_handles();
                    Handle h = "0".stringToHandle();
                    ObjectId id = ObjectId.Null;
                    //Working with a list of unique non-zero handles
                    int k = handles.Count;
                    for (int i = k - 1; i > -1; i--)
                    {
                        //GET BREAKLINE PER STORED HANDLES
                        try
                        {
                            h = handles[i];
                            id = h.getObjectId();
                            if (!id.IsValid || id.IsErased || id.IsEffectivelyErased)
                            {
                                handles.RemoveAt(i);
                                continue;
                            }
                        }
                        catch (System.Exception)
                        {
                            handles.RemoveAt(i);
                        }
                        Polyline3d poly3d = (Polyline3d)h.getEnt();
                        if (poly3d == null)
                            handles.RemoveAt(i);
                    }
                    idCgPnt.clearXData(nameApp);
                    idCgPnt.setXData(handles, nameApp);
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xData.cs: line: 191");
            }
        }

        public static void
        clearAllXdata()
        {
            bool escape = true;
            Entity ent = Select.selectEntity("Select ent:", out escape);
            if (escape)
                return;

            ResultBuffer rb = ent.ObjectId.getXData(null);
            if (rb == null)
                return;
            TypedValue[] tvs = rb.AsArray();
            List<string> apps = new List<string>();
            List<TypedValue[]> tvsList = tvs.parseXData(out apps);
            foreach (string n in apps)
                ent.ObjectId.clearXData(n);
        }

        public static void
        clearAllXdata(this ObjectId id)
        {
            ResultBuffer rb = id.getXData(null);
            if (rb == null)
                return;
            TypedValue[] tvs = rb.AsArray();
            List<string> apps = new List<string>();
            List<TypedValue[]> tvsList = tvs.parseXData(out apps);
            foreach (string n in apps)
                id.clearXData(n);
        }

        public static RegAppTable
        getRegAppTable()
        {
            RegAppTable RAT = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    RAT = (RegAppTable)BaseObjs._db.RegAppTableId.GetObject(OpenMode.ForRead);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xData.cs: line: 240");
            }
            return RAT;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="nameApp"></param>
        /// <param name="typeOut"></param>
        /// <param name="valOut"></param>
        /// <returns></returns>
        public static bool
        getXdata(Entity ent, string nameApp, out List<Object> typeOut, out List<Object> valOut)
        {
            bool isEmpty = true;
            List<Object> type = new List<Object>();
            List<Object> val = new List<Object>();

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ResultBuffer rb = ent.GetXDataForApplication(nameApp);
                    if (rb != null)
                    {
                        foreach (TypedValue tv in rb)
                        {
                            type.Add(tv.TypeCode);
                            val.Add(tv.Value);
                        }
                        isEmpty = false;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xData.cs: line: 279");
            }
            typeOut = type;
            valOut = val;
            return isEmpty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nameApp"></param>
        /// <returns></returns>
        public static ResultBuffer
        getXdata(ObjectId id, string nameApp)
        {
            ResultBuffer rb = null;
            if (id == ObjectId.Null || id.IsEffectivelyErased || id.IsErased)
                return null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDoc())
                {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                    rb = ent.GetXDataForApplication(nameApp);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xData.cs: line: 309");
            }
            return rb;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="nameApp"></param>
        /// <returns></returns>
        public static ResultBuffer
        getXdata(Entity ent, string nameApp)
        {
            ResultBuffer rb = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    rb = ent.GetXDataForApplication(nameApp);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xData.cs: line: 334");
            }
            return rb;
        }

        public static void
        lnkPntsAndPoly3d(this ObjectId idPoly3d, ObjectId idCogoPnt1, ObjectId idCogoPnt2, string nameApp)
        {
            try
            {
                Handle hPoly3d = idPoly3d.getHandle();
                TypedValue[] TVs = null;

                ResultBuffer RB = idCogoPnt1.getXData(nameApp); //if from MBL nameApp = lnkBrks2
                if (RB == null)
                {
                    TVs = new TypedValue[2];
                    TVs.SetValue(new TypedValue(1001, nameApp), 0);
                    TVs.SetValue(new TypedValue(1005, hPoly3d), 1);
                    idCogoPnt1.setXData(TVs, nameApp);
                }
                else
                {
                    idCogoPnt1.updatePntXData(idPoly3d, nameApp);               //add poly3d to point xdata
                }

                RB = null;
                RB = idCogoPnt2.getXData(nameApp);
                if (RB == null)
                {
                    TVs = new TypedValue[2];
                    TVs.SetValue(new TypedValue(1001, nameApp), 0);
                    TVs.SetValue(new TypedValue(1005, hPoly3d), 1);
                    idCogoPnt2.setXData(TVs, nameApp);
                }
                else
                {
                    idCogoPnt2.updatePntXData(idPoly3d, nameApp); 
                }

                TVs = new TypedValue[3];
                TVs.SetValue(new TypedValue(1001, nameApp), 0);
                TVs.SetValue(new TypedValue(1005, idCogoPnt1.getHandle()), 1);
                TVs.SetValue(new TypedValue(1005, idCogoPnt2.getHandle()), 2);
                
                idPoly3d.setXData(TVs, nameApp);                 //add cogo points to poly3d
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xData.cs: line: 383");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tvs"></param>
        /// <returns></returns>
        public static List<TypedValue[]>
        parseXData(this TypedValue[] tvs, out List<string> nameApps)
        {
            List<List<TypedValue>> tvsALL = new List<List<TypedValue>>();
            List<TypedValue> tvsApp = null;

            nameApps = new List<string>();
            foreach (TypedValue tv in tvs)
            {
                switch (tv.TypeCode)
                {
                    case 1001:
                        if (!tv.Value.ToString().Contains("Acad"))
                            nameApps.Add(tv.Value.ToString());

                        tvsApp = new List<TypedValue>();
                        tvsApp.Add(tv);
                        tvsALL.Add(tvsApp);
                        break;

                    default:
                        tvsApp.Add(tv);
                        break;
                }
            }
            List<TypedValue[]> tvRet = new List<TypedValue[]>();
            foreach (List<TypedValue> tvApp in tvsALL)
            {
                TypedValue[] tvArray = tvApp.ToArray();
                if (tvArray[0].TypeCode == 1001 && !tvArray[0].Value.ToString().Contains("Acad"))
                    tvRet.Add(tvArray);
            }
            return tvRet;
        }

        /// <summary>
        /// remove xdata from obj
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nameApp"></param>
        public static void
        removeAppXData(ObjectId id, string nameApp)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                    try
                    {
                        ent.XData = new ResultBuffer(new TypedValue(1001, nameApp));
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " xData.cs: line: 446");
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xData.cs: line: 453");
            }
        }

        public static void
        removeCmdXDataFromCogoPoint(ObjectId id, string nameApp, string nameCmd)
        {
            TypedValue[] tvsNew = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    if (id.getType() != "CogoPoint")
                        return;
                    ResultBuffer rb = id.getXData(nameApp);
                    if (rb == null)
                        return;

                    TypedValue[] tvs = rb.AsArray();
                    int x = 0;
                    for (int i = 1; i < tvs.Length; i++)
                    {
                        if (nameCmd == tvs[i].Value.ToString())
                        {
                            x = i;
                            break;
                        }
                    }
                    if (tvs.Length > 4)
                    {
                        int n = -1;
                        tvsNew = new TypedValue[tvs.Length - 3];
                        for (int j = 0; j < tvs.Length; j++)
                        {
                            if (j < x || j >= x + 3)
                            {
                                tvsNew[++n] = tvs[j];
                            }
                        }

                        removeAppXData(id, nameApp);
                        id.setXData(tvsNew, nameApp);
                    }
                    else
                    {
                        removeAppXData(id, nameApp);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xData.cs: line: 505");
            }
        }

        public static void
        removeHandleFromXdata(this ObjectId id, string nameApp, Handle h)
        {
            TypedValue[] tvsNew = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ResultBuffer rb = id.getXData(nameApp);
                    if (rb == null)
                        return;
                    List<Handle> handles = rb.rb_handles();
                    handles.Remove(h);
                    tvsNew = new TypedValue[handles.Count + 1];
                    tvsNew.SetValue(new TypedValue(1001, nameApp), 0);
                    for (int i = 0; i < handles.Count; i++)
                    {
                        tvsNew.SetValue(new TypedValue(1005, handles[i]), i + 1);
                    }
                    id.clearXData(nameApp);
                    id.setXData(tvsNew, nameApp);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xData.cs: line: 535");
            }
        }

        // removeXData
        public static void
        removeHandleFromXDataGS(this ObjectId id, string nameApp, Handle h)
        {
            ResultBuffer rb = id.getXData(nameApp);
            if (rb == null)
                return;
            TypedValue[] tvs = rb.AsArray();
            List<Handle> handles = rb.rb_handles();
            switch (handles.Count)
            {
                case 0:
                    id.clearXData(nameApp);
                    break;

                case 1:
                    if (h.ToString() == handles[0].ToString())
                        id.clearXData(nameApp);
                    break;

                default:
                    if (handles.Contains(h))
                        handles.Remove(h);
                    TypedValue[] tvsNew = new TypedValue[2 + handles.Count];
                    tvsNew.SetValue(tvs[0], 0);
                    tvsNew.SetValue(tvs[1], 1);
                    for (int i = 0; i < handles.Count; i++)
                    {
                        tvsNew.SetValue(new TypedValue(1005, handles[i]), i + 2);
                    }
                    id.clearXData(nameApp);
                    id.setXData(tvsNew, nameApp);
                    break;
            }
        }

        public static void
        replaceHandleInXdata(this ObjectId id, string nameApp, Handle hAdd, Handle hDel)
        {
            TypedValue[] tvsNew = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ResultBuffer rb = id.getXData(nameApp);
                    if (rb == null)
                        return;
                    List<Handle> handles = rb.rb_handles();
                    int x = handles.IndexOf(hDel);
                    handles.Insert(x, hAdd);
                    handles.Remove(hDel);
                    tvsNew = new TypedValue[handles.Count + 1];
                    tvsNew.SetValue(new TypedValue(1001, nameApp), 0);
                    for (int i = 0; i < handles.Count; i++)
                    {
                        tvsNew.SetValue(new TypedValue(1005, handles[i]), i + 1);
                    }
                    id.clearXData(nameApp);
                    id.setXData(tvsNew, nameApp);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xData.cs: line: 603");
            }
        }

        public static void
        replacePntXData(CogoPoint cogoPnt, CogoPoint cogoPntX, Polyline3d poly3d)
        {
            try
            {
                ResultBuffer RB0 = poly3d.ObjectId.getXData(apps.lnkBrks);
                List<Handle> handles = RB0.rb_handles();     //get list of unique non Zero handles

                int i = handles.IndexOf(cogoPnt.Handle);
                handles.RemoveAt(i);
                handles.Insert(i, cogoPntX.Handle);

                poly3d.ObjectId.clearXData(apps.lnkBrks);
                poly3d.ObjectId.setXData(handles, apps.lnkBrks3);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xData.cs: line: 624");
            }
        }

        public static void
        toList(this TypedValue[] tvs, out short[] typeOut, out object[] valOut)
        {
            List<short> type = new List<short>();
            List<Object> val = new List<Object>();
            ResultBuffer rb = new ResultBuffer(tvs);
            foreach (TypedValue tv in rb)
            {
                type.Add(tv.TypeCode);
                val.Add(tv.Value);
            }
            typeOut = new short[type.Count];
            valOut = new object[val.Count];

            for (int i = 0; i < type.Count; i++)
            {
                typeOut[i] = type[1];
                valOut[i] = val[i];
            }
        }

        public static void
        updatePntXData(this ObjectId idCgPnt, ObjectId idPoly3d, string nameApp)
        {
            try
            {
                checkPointXDataXNodes(idCgPnt, nameApp);
                Handle hPoly3d = idPoly3d.getHandle();
                
                ResultBuffer rbo = idCgPnt.getXData(nameApp);

                if (rbo != null)
                {
                    List<Handle> handles = rbo.rb_handles();
                    handles.Add(hPoly3d);
                    idCgPnt.clearXData(nameApp);
                    idCgPnt.setXData(handles, nameApp);
                }
                else
                {
                    TypedValue[] TVs = new TypedValue[2];
                    TVs.SetValue(new TypedValue(1001, nameApp), 0);
                    TVs.SetValue(new TypedValue(1005, hPoly3d), 1);

                    idCgPnt.setXData(TVs, nameApp);
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xData.cs: line: 677");
            }
        }

        /// <summary>
        /// get RegAppTable
        /// </summary>
        /// <returns>RegAppTable</returns>
        /// <summary>
        /// AddRegAppTableRecord
        /// </summary>
        /// <param name="nameApp"></param>
        private static void
        AddRegAppTableRecord(string nameApp)
        {
            Database DB = BaseObjs._db;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    RegAppTable RAT = (RegAppTable)tr.GetObject(DB.RegAppTableId, OpenMode.ForRead);
                    if (!RAT.Has(nameApp))
                    {
                        RAT.UpgradeOpen();
                        RegAppTableRecord RAtr = new RegAppTableRecord();
                        RAtr.Name = nameApp;
                        RAT.Add(RAtr);
                        tr.AddNewlyCreatedDBObject(RAtr, true);
                    }
                    tr.Commit();
                }//end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " xData.cs: line: 711");
            }
        }
    }
}
