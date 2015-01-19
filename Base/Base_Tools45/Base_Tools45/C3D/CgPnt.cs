using Autodesk.Civil.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

using System.Collections.Generic;

namespace Base_Tools45.C3D
{
    /// <summary>
    ///
    /// </summary>
    public static class CgPnt
    {
        /// <summary>
        ///
        /// </summary>
        public static void
        deletePnts()
        {
            foreach (ObjectId idPnt in BaseObjs._civDoc.CogoPoints)
            {
                BaseObjs._civDoc.CogoPoints.Remove(idPnt);
            }
        }

        public static ObjectId
        getPointByNumber(this uint pntNum)
        {
            ObjectId idCgPnt = ObjectId.Null;
            CivilDocument civDoc = CivilApplication.ActiveDocument;
            CogoPointCollection cgPnts = civDoc.CogoPoints;
            idCgPnt = cgPnts.GetPointByPointNumber(pntNum);
            return idCgPnt;
        }

        public static CogoPoint
        selectPoint(string cmd, int osMode)
        {
            object mode = SnapMode.getOSnap();
            SnapMode.setOSnap(osMode);
            CogoPoint cogoPnt = null;

            Database db = BaseObjs._db;
            Editor ed = BaseObjs._editor;

            Autodesk.AutoCAD.DatabaseServices.Entity ent = null;

            TypedValue[] tvs = new TypedValue[1];
            tvs.SetValue(new TypedValue((int)DxfCode.Start, "AECC_COGO_POINT"), 0);

            SelectionFilter filter = new SelectionFilter(tvs);
            PromptSelectionResult psr = null;

            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = string.Format("\nSelect {0} Point:", cmd);

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    try
                    {
                        psr = ed.GetSelection(filter);
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Pnt.cs: line: 72", ex.Message));
                    }
                    if (psr.Status == PromptStatus.OK)
                    {
                        SelectionSet SS = psr.Value;

                        foreach (SelectedObject OBJ in SS)
                        {
                            if (OBJ != null)
                            {
                                ent = (Autodesk.AutoCAD.DatabaseServices.Entity)tr.GetObject(OBJ.ObjectId, OpenMode.ForRead);
                            }
                            if (ent != null)
                            {
                                if (ent.GetType().ToString() == "Autodesk.Civil.DatabaseServices.CogoPoint")
                                {
                                    cogoPnt = (CogoPoint)ent;
                                }
                            }
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnt.cs: line: 92", ex.Message));
            }
            finally
            {
                SnapMode.setOSnap((int)mode);
            }
            return cogoPnt;
        }

        public static ObjectId
        selectPointEntity(string cmd, int osMode)
        {
            object mode = SnapMode.getOSnap();
            SnapMode.setOSnap(osMode);

            ObjectId idPnt = ObjectId.Null;

            Database db = BaseObjs._db;
            Editor ed = BaseObjs._editor;

            PromptEntityOptions peo = new PromptEntityOptions(string.Format("\nSelect {0} Point:", cmd));
            peo.AllowNone = false;
            peo.SetRejectMessage("\nSelected Entity is not a Cogo Point.");
            peo.AddAllowedClass(typeof(CogoPoint), true);
            try
            {
                idPnt = ed.GetEntity(peo).ObjectId;
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnt.cs: line: 118", ex.Message));
            }
            finally
            {
                SnapMode.setOSnap((int)mode);
            }

            return idPnt;
        }

        public static ObjectId
        selectCogoPointByNode(string prompt, ref Point3d pnt3dBase, out bool escape, out PromptStatus ps, int osMode)
        {
            ObjectId id = ObjectId.Null;

            pnt3dBase = UserInput.getPoint(prompt, pnt3dBase, out escape, out ps, osMode);
            Point3d pnt3dTar = pnt3dBase;
            BaseObjs._db.forEachMS<CogoPoint>(cg =>
            {
                if (cg.Location == pnt3dTar)
                {
                    id = cg.ObjectId;
                }
            });
            return id;
        }

        public static ObjectId
        selectCogoPointByNode(string prompt, int osMode)
        {
            ObjectId id = ObjectId.Null;
            PromptStatus ps = PromptStatus.Cancel;
            Point3d pnt3dBase = UserInput.getPoint(prompt, out ps, osMode);
            BaseObjs._db.forEachMS<CogoPoint>(cg =>
            {
                if (cg.Location == pnt3dBase)
                {
                    id = cg.ObjectId;
                }
            });
            return id;
        }

        public static List<ObjectId>
        checkForCogoPointsByNode(Point3d pnt3dBase)
        {
            
            ObjectId id = ObjectId.Null;
            List<ObjectId> ids = new List<ObjectId>();

            BaseObjs._db.forEachMS<CogoPoint>(cg =>
            {
                if (cg.Location == pnt3dBase)
                {
                    id = cg.ObjectId;
                    ids.Add(id);
                }
            });
            return ids;
        }


        public static SelectionSet
        selectPoints()
        {
            SelectionSet ss = null;

            Database db = BaseObjs._db;
            Editor ed = BaseObjs._editor;

            TypedValue[] tvs = new TypedValue[1];
            tvs.SetValue(new TypedValue((int)DxfCode.Start, "AECC_COGO_POINT"), 0);

            SelectionFilter filter = new SelectionFilter(tvs);
            PromptSelectionResult psr = null;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    try
                    {
                        psr = ed.SelectAll(filter);
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Pnt.cs: line: 173", ex.Message));
                    }
                    if (psr.Status == PromptStatus.OK)
                    {
                        ss = psr.Value;
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnt.cs: line: 181", ex.Message));
            }

            return ss;
        }

        public static ObjectId
        setPoint(this Point3d pnt3d, string nameLayer = "CPNT-ON")
        {
            ObjectId id = ObjectId.Null;
            Layer.manageLayers(nameLayer);
            CivilDocument civDOC = CivilApplication.ActiveDocument;
            id = civDOC.CogoPoints.Add(pnt3d, true);
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    CogoPoint cogoPnt = (CogoPoint)id.GetObject(OpenMode.ForWrite);
                    cogoPnt.PointName = cogoPnt.PointNumber.ToString();
                    if (nameLayer != null)
                    {
                        try
                        {
                            cogoPnt.Layer = nameLayer;
                        }
                        catch (System.Exception ex)
                        {
                            BaseObjs.writeDebug(string.Format("{0} Pnt.cs: line: 203", ex.Message));
                        }
                        cogoPnt.RawDescription = nameLayer;
                        cogoPnt.LabelStyleId = Pnt_Style.getPntLabelStyle(nameLayer);
                        cogoPnt.PointName = nameLayer + cogoPnt.PointNumber;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnt.cs: line: 214", ex.Message));
            }
            return id;
        }

        public static ObjectId
        setPoint(this Point3d pnt3d, out uint pntNum, string pntDesc = "CPNT-ON")
        {
            ObjectId idPnt = ObjectId.Null;
            pntNum = 0;
            CivilDocument civDoc = CivilApplication.ActiveDocument;
            idPnt = civDoc.CogoPoints.Add(pnt3d, false);

            Layer.manageLayers(pntDesc);
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    CogoPoint pnt = (CogoPoint)tr.GetObject(idPnt, OpenMode.ForWrite);
                    pnt.PointName = pnt.PointNumber.ToString();
                    pnt.LabelStyleId = Pnt_Style.getPntLabelStyle(pntDesc);
                    pnt.StyleId = Pnt_Style.getPntStyle(pntDesc);
                    try
                    {
                        pnt.Layer = pntDesc;
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Pnt.cs: line: 237", ex.Message));
                    }
                    pnt.RawDescription = pntDesc;
                    pntNum = pnt.PointNumber;
                    CgPnt_Group.updatePntGroup(pntDesc);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnt.cs: line: 246", ex.Message));
            }
            return idPnt;
        }
    }
}