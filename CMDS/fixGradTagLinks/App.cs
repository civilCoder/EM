using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;

using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace fixGradeTagLinks
{
    public static class App
    {
        public static ObjectIdCollection ids = null;

        private static List<Handle> hLnkAdds = null;
        private static List<Handle> hLnkFixs = null;

        private static Vector3d v3d0 = new Vector3d(0, 0, 0);

        public static void
        executeCLKS(SelectionSet ss0 = null)
        {
            hLnkAdds = new List<Handle>();
            hLnkFixs = new List<Handle>();
            winResults wResults = FormHandler.fHandler.wResults;
            Application.ShowModelessWindow(wResults);

            ListBox lbxAdd = wResults.lbxAdd;
            lbxAdd.Items.Clear();
            lbxAdd.Height = 220;

            ListBox lbxFix = wResults.lbxFix;
            lbxFix.Items.Clear();
            lbxFix.Height = 220;
            wResults.Height = 565;

            bool exists;
            string nameDict = "";
            ObjectId idDict = ObjectId.Null;
            ObjectId idPnt = ObjectId.Null;

            List<ObjectId> ids = getBlockRefs();
            List<ObjectId> idsPnt = getPoints();
            List<ObjectId> idsLdr = getLeaders();

            Handle hPntX = new Handle();

            if (ids != null && ids.Count > 0)
            {
                wResults.pBar1.Maximum = ids.Count;
                int i = 0;
                Point3d pnt3dIns;

                ObjectId idGradeTagDict = Dict.getNamedDictionary("GradeTagDict", out exists);
                List<DBDictionaryEntry> entriesGradeTagDict = idGradeTagDict.getEntries();

                ObjectId idFlTagDict = Dict.getNamedDictionary("FlTagDict", out exists);
                List<DBDictionaryEntry> entriesFlTagDict = idFlTagDict.getEntries();
                List<DBDictionaryEntry> entries = null;

                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    foreach (ObjectId idBlkRef in ids)
                    {
                        ++i;
                        Debug.Print(i.ToString());

                        string nameBlkRef = Blocks.getBlockRefName(idBlkRef).ToUpper();
                        switch (nameBlkRef)
                        {
                            case "GRADETAG":
                                entries = entriesGradeTagDict;
                                nameDict = "GradeTagDict";
                                idDict = idGradeTagDict;
                                break;

                            case "FLTAG":
                                entries = entriesFlTagDict;
                                nameDict = "FlTagDict";
                                idDict = idFlTagDict;
                                break;
                        }
                        bool fix;

                        bool isValid = utility1.isValidObjInDictionary(idBlkRef, entries, nameDict, idDict, out fix, out hPntX);

                        if (isValid)
                        {
                            if (fix)
                                hLnkFixs.Add(hPntX);
                        }
                        else
                        {
                            using (BaseObjs._acadDoc.LockDocument())
                            {
                                try
                                {
                                    BlockReference br = (BlockReference)tr.GetObject(idBlkRef, OpenMode.ForRead);
                                    pnt3dIns = br.Position;
                                    //Debug.Print(string.Format("{0:F3},{1:F3},{2:F3}", pnt3dIns.X, pnt3dIns.Y, pnt3dIns.Z));

                                    ObjectId idLdr = findLeader(pnt3dIns, idsLdr);
                                    if (idLdr == ObjectId.Null)
                                        continue;

                                    Point3d pnt3dLdrBeg = idLdr.getBegPnt();
                                    Point3d pnt3dPnt = Point3d.Origin;

                                    bool found = false;

                                    try
                                    {
                                        foreach (ObjectId id in idsPnt)
                                        {
                                            pnt3dPnt = id.getCogoPntCoordinates();

                                            if ((pnt3dPnt - pnt3dLdrBeg) == v3d0)
                                            {
                                                found = true;
                                                idPnt = id;
                                                break;
                                            }
                                            else if (System.Math.Round(pnt3dPnt.getDistance(pnt3dLdrBeg)) < .001)
                                            {
                                                found = true;
                                                idPnt = id;
                                                break;
                                            }
                                        }
                                    }
                                    catch (System.Exception ex)
                                    {
                                        Application.ShowAlertDialog(string.Format("{0} line 149", ex.Message));
                                    }

                                    if (!found)
                                        continue;

                                    string desc = string.Empty;

                                    try
                                    {
                                        switch (nameBlkRef)
                                        {
                                            case "GRADETAG":
                                                desc = Blocks.getBlockRefAttributeValue(idBlkRef, "TOPTXT");
                                                desc = desc.Replace(")", "");
                                                desc = desc.Replace("(", "");
                                                if (desc == "")
                                                {
                                                    idBlkRef.delete();
                                                    continue;
                                                }
                                                switch (desc)
                                                {
                                                    case "FF":
                                                        utility1.checkFF(idBlkRef, pnt3dPnt);
                                                        addOrphanToDicts("GradeTagDict", idPnt.getHandle(), idBlkRef.getHandle(), idLdr.getHandle());
                                                        //Debug.Print(idPnt.getHandle().ToString());
                                                        break;

                                                    case "TC":
                                                        utility1.checkTC(idBlkRef, pnt3dPnt);
                                                        addOrphanToDicts("GradeTagDict", idPnt.getHandle(), idBlkRef.getHandle(), idLdr.getHandle());
                                                        //Debug.Print(idPnt.getHandle().ToString());
                                                        break;
                                                }
                                                break;

                                            case "FLTAG":
                                                desc = Blocks.getBlockRefAttributeValue(idBlkRef, "BOTTXT");
                                                desc = desc.Replace(")", "");
                                                desc = desc.Replace("(", "");
                                                if (desc == "")
                                                {
                                                    idBlkRef.delete();
                                                    continue;
                                                }
                                                utility1.checkFL(idBlkRef, pnt3dPnt);
                                                addOrphanToDicts("FlTagDict", idPnt.getHandle(), idBlkRef.getHandle(), idLdr.getHandle());
                                                //Debug.Print(idPnt.getHandle().ToString());

                                                break;
                                        }
                                    }
                                    catch (System.Exception ex)
                                    {
                                        Application.ShowAlertDialog(string.Format("{0} line 198", ex.Message));
                                    }
                                }
                                catch (Autodesk.AutoCAD.Runtime.Exception ex5)
                                {
                                    Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog(string.Format("{0} line 204", ex5.Message));
                                }
                            }
                        }
                        wResults.pBar1.Value = i;
                        
                    }
                    tr.Commit();
                }
            }

            wResults.addControls(wResults, lbxAdd, hLnkAdds);
            wResults.addControls(wResults, lbxFix, hLnkFixs);

            using (BaseObjs._acadDoc.LockDocument())
            {
                Dict.getNamedDictionary("CLKS", out exists);
            }

            wResults.pBar1.Value = 0;
        }

        public static void
        addOrphanToDicts(string nameDict, Handle hPnt, Handle hBlkRef, Handle hLdr)
        {
            bool exists;
            ObjectId idDict = Dict.getNamedDictionary(nameDict, out exists);
            ResultBuffer rb = null;
            TypedValue[] tvs = new TypedValue[2];
            tvs.SetValue(new TypedValue(1005, hBlkRef), 0);
            tvs.SetValue(new TypedValue(1005, hLdr), 1);
            rb = new ResultBuffer(tvs);
            Dict.addXRec(idDict, hPnt.ToString(), rb);

            Point3d pnt3dPnt = hPnt.getCogoPntCoordinates();

            Entity ent = hBlkRef.getObjectId().getEnt();
            BlockReference br = (BlockReference)ent;
            Point3d pnt3dBlk = br.Position;

            idDict = Dict.getNamedDictionary("ObjectDict", out exists);
            tvs = new TypedValue[9];
            tvs.SetValue(new TypedValue(1000, hLdr.getObjectId().ToString()), 0);
            tvs.SetValue(new TypedValue(1000, nameDict), 1);
            tvs.SetValue(new TypedValue(1000, hLdr.ToString()), 2);
            tvs.SetValue(new TypedValue(1040, pnt3dPnt.X), 3);
            tvs.SetValue(new TypedValue(1040, pnt3dPnt.Y), 4);
            tvs.SetValue(new TypedValue(1040, 0.0), 5);
            tvs.SetValue(new TypedValue(1040, pnt3dBlk.X), 6);
            tvs.SetValue(new TypedValue(1040, pnt3dBlk.Y), 7);
            tvs.SetValue(new TypedValue(1040, 0.0), 8);

            rb = new ResultBuffer(tvs);
            string idBlkRef = hBlkRef.getObjectId().ToString();
            idBlkRef = idBlkRef.Replace("(", "");
            idBlkRef = idBlkRef.Replace(")", "");
            Dict.addXRec(idDict, idBlkRef, rb);

            tvs = new TypedValue[6];
            tvs.SetValue(new TypedValue(1000, hBlkRef.getObjectId().ToString()), 0);
            tvs.SetValue(new TypedValue(1000, nameDict), 1);
            tvs.SetValue(new TypedValue(1005, hPnt.ToString()), 2);
            tvs.SetValue(new TypedValue(1040, pnt3dPnt.X), 3);
            tvs.SetValue(new TypedValue(1040, pnt3dPnt.Y), 4);
            tvs.SetValue(new TypedValue(1040, 0.0), 5);

            rb = new ResultBuffer(tvs);
            string idLdr = hLdr.getObjectId().ToString();
            idLdr = idLdr.Replace("(", "");
            idLdr = idLdr.Replace(")", "");
            Dict.addXRec(idDict, idLdr, rb);

            idDict = Dict.getNamedDictionary("SearchDict", out exists);
            tvs = new TypedValue[1];
            tvs.SetValue(new TypedValue(1005, hPnt), 0);
            rb = new ResultBuffer(tvs);
            string idPnt = hPnt.getObjectId().ToString();
            idPnt = idPnt.Replace("(", "");
            idPnt = idPnt.Replace(")", "");

            Dict.addXRec(idDict, idPnt, rb);
            hLnkAdds.Add(hPnt);
        }

        public static List<ObjectId> getBlockRefs()
        {
            List<ObjectId> ids = new List<ObjectId>();
            using (var tr = BaseObjs.startTransactionDb())
            {
                var bt =
                    (BlockTable)tr.GetObject(
                        BaseObjs._db.BlockTableId, OpenMode.ForRead);
                var ms =
                    (BlockTableRecord)tr.GetObject(
                        bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                RXClass theClass = RXObject.GetClass(typeof(BlockReference));

                foreach (ObjectId id in ms)
                {
                    if (id.ObjectClass.IsDerivedFrom(theClass))
                    {
                        var br =
                            (BlockReference)tr.GetObject(
                                id, OpenMode.ForRead);

                        if (br.Name.ToUpper() == "GRADETAG" || br.Name.ToUpper() == "FLTAG")
                            ids.Add(br.ObjectId);
                    }
                }
            }
            return ids;
        }

        public static List<ObjectId> getPoints()
        {
            List<ObjectId> ids = new List<ObjectId>();
            using (var tr = BaseObjs.startTransactionDb())
            {
                var bt =
                    (BlockTable)tr.GetObject(
                        BaseObjs._db.BlockTableId, OpenMode.ForRead);
                var ms =
                    (BlockTableRecord)tr.GetObject(
                        bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                foreach (ObjectId id in ms)
                {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                    if (ent is CogoPoint)
                    {
                        ids.Add(id);
                    }
                }
                tr.Commit();
            }
            return ids;
        }

        public static List<ObjectId> getLeaders()
        {
            List<ObjectId> ids = new List<ObjectId>();
            using (var tr = BaseObjs.startTransactionDb())
            {
                var bt =
                    (BlockTable)tr.GetObject(
                        BaseObjs._db.BlockTableId, OpenMode.ForRead);
                var ms =
                    (BlockTableRecord)tr.GetObject(
                        bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                foreach (ObjectId id in ms)
                {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                    if (ent is Leader)
                    {
                        ids.Add(id);
                    }
                }
                tr.Commit();
            }
            return ids;
        }

        public static ObjectId
        findLeader(Point3d pnt3d, List<ObjectId> idsLdr)
        {
            ObjectId idLdr = ObjectId.Null;
            Point3d pnt3dBEG = Point3d.Origin;
            Point3d pnt3dEND = Point3d.Origin;

            Point3dCollection pnts3dColl = null;
            foreach (ObjectId id in idsLdr)
            {
                pnts3dColl = id.getCoordinates3d();
                if (pnts3dColl.Count == 3)
                {
                    pnt3dBEG = pnts3dColl[1];
                    pnt3dEND = pnts3dColl[2];
                }
                else if (pnts3dColl.Count == 2)
                {
                    pnt3dBEG = pnts3dColl[0];
                    pnt3dEND = pnts3dColl[1];
                }

                if (Geom.isOn2dSegment(pnt3d, pnt3dBEG, pnt3dEND))
                {
                    idLdr = id;
                    break;
                }
            }
            return idLdr;
        }
    }
}