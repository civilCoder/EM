//using System.Windows.Media;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;
using Color = Autodesk.AutoCAD.Colors.Color;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

//using Autodesk.Civil.DatabaseServices;
namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static class Layer
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="pntDes"></param>
        /// <returns></returns>
        public static string
        conformLayerName(string pntDes)
        {
            string strBuf = null;
            string strLayerName = null;
            int pos = 0;

            strBuf = pntDes;

            pos = strBuf.IndexOf("/");
            if (pos != 0)
            {
                strBuf = strBuf.Substring(1, pos - 1);
            }

            switch (strBuf)
            {
                case "ACC":
                    strLayerName = "PVMT-AC";
                    break;

                case "EOC":
                    strLayerName = "EC";
                    break;

                case "FP":
                    strLayerName = "PVMT";
                    break;

                case "LP":
                case "LIP":
                    strLayerName = "LIP";
                    break;

                case "SP":
                    strLayerName = "SPOT";
                    break;

                case "TC":
                    strLayerName = "CURB";
                    break;

                case "trANSPAD":
                    strLayerName = "PAD-trANS";
                    break;

                case "TWBL":
                    strLayerName = "WALL-BRICK-TOP";
                    break;

                case "TWC":
                    strLayerName = "WALL-CONC-TOP";
                    break;

                case "UTL-VLT":
                    strLayerName = "UTIL-VAULT-ELEC";
                    break;

                default:
                    strLayerName = strBuf;
                    break;
            }
            return strLayerName;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string
        getLayer(ObjectId id)
        {
            string nameLayer = "";
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForRead);
                    tr.Commit();
                    nameLayer = ent.Layer;
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Layer.cs: line: 108");
            }
            return nameLayer;
        }

        public static ObjectId
        getLayer(string nameLayer)
        {
            ObjectId idLayer = ObjectId.Null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    LayerTableRecord ltr = getLayerTableRecord(nameLayer);
                    idLayer = ltr.ObjectId;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Layer.cs: line: 128");
            }
            return idLayer;
        }

        public static LayerTable
        getLayerTable()
        {
            LayerTable LT = null;
            Database DB = BaseObjs._db;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    LT = (LayerTable)tr.GetObject(DB.LayerTableId, OpenMode.ForRead);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Layer.cs: line: 148");
            }
            return LT;
        }

        public static LayerTableRecord
        getLayerTableRecord(string name)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    LayerTable LT = getLayerTable();
                    if (LT.Has(name) == true)
                    {
                        LayerTableRecord Ltr = (LayerTableRecord)tr.GetObject(LT[name], OpenMode.ForRead);
                        return Ltr;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Layer.cs: line: 171");
            }
            return null;
        }

        /// <summary>
        /// get LayerTable
        /// </summary>
        /// <returns>LayerTable</returns>
        /// <summary>
        /// get LayerTableRecord
        /// </summary>
        /// <param name="name"></param>
        /// <returns>LayerTableRecord</returns>
        public static List<string>
        layersInDwg()
        {
            List<string> layers = new List<string>();
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDoc())
                {
                    LayerTable lt = getLayerTable();
                    foreach (ObjectId id in lt)
                    {
                        LayerTableRecord ltr = (LayerTableRecord)tr.GetObject(id, OpenMode.ForRead);
                        layers.Add(ltr.Name);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Layer.cs: line: 204");
            }
            return layers;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="strLayerName"></param>
        /// <param name="color"></param>
        /// <param name="layerOff"></param>
        /// <param name="layerFrozen"></param>
        public static void
        manageLayer(string strLayerName, short color = 256, bool layerOff = false, bool layerFrozen = false)
        {
            manageLayers(strLayerName);
            Database DB = BaseObjs._db;
            try
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        LayerTable LT = (LayerTable)tr.GetObject(DB.LayerTableId, OpenMode.ForRead);
                        LayerTableRecord Ltr = tr.GetObject(LT[strLayerName], OpenMode.ForWrite) as LayerTableRecord;
                        Ltr.Color = Color.FromColorIndex(ColorMethod.ByLayer, color);
                        Ltr.IsOff = layerOff;
                        Ltr.IsFrozen = layerFrozen;
                        tr.Commit();
                    }//end using
                }//end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Layer.cs: line: 238");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="strLayerName"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static ObjectId
        manageLayers(string strLayerName, short color = 256)
        {
            ObjectId idLayer = ObjectId.Null;
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database DB = doc.Database;
            bool thawed;
            try
            {
                using (doc.LockDocument())
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        LayerTable LT = (LayerTable)tr.GetObject(DB.LayerTableId, OpenMode.ForRead);
                        LayerTableRecord Ltr;
                        if (LT.Has(strLayerName) == false)
                        {
                            Ltr = new LayerTableRecord();
                            Ltr.Name = strLayerName;
                            if (color != 0)
                            {
                                Ltr.Color = Color.FromColorIndex(ColorMethod.ByLayer, color);
                            }
                            LT.UpgradeOpen();
                            LT.Add(Ltr);
                            tr.AddNewlyCreatedDBObject(Ltr, true);
                            tr.Commit();
                            idLayer = Ltr.ObjectId;
                        }
                        else
                        {
                            Ltr = tr.GetObject(LT[strLayerName], OpenMode.ForWrite) as LayerTableRecord;
                            Ltr.Color = Color.FromColorIndex(ColorMethod.ByLayer, color);
                            idLayer = Ltr.ObjectId;

                            if (Ltr.IsFrozen)
                            {
                                LT.UpgradeOpen();
                                Ltr.IsFrozen = false;
                                thawed = true;
                            }
                            else
                            {
                                thawed = false;
                            }
                            if (Ltr.IsOff)
                            {
                                if (thawed == false)
                                {
                                    LT.UpgradeOpen();
                                }
                                Ltr.IsOff = false;
                            }
                            tr.Commit();
                        }
                    }//end using
                }//end using
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Layer.cs: line: 307");
            }
            return idLayer;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idLayer"></param>
        /// <param name="colorIndex"></param>
        /// <param name="weight"></param>
        /// <param name="nameLineType"></param>
        public static void
        modifyLayer(ObjectId idLayer, short colorIndex, LineWeight weight, string nameLineType = null)
        {
            try
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        LayerTableRecord Ltr = (LayerTableRecord)tr.GetObject(idLayer, OpenMode.ForWrite);
                        Ltr.Color = Color.FromColorIndex(ColorMethod.ByBlock, colorIndex);
                        Ltr.LineWeight = weight;
                        if (nameLineType != null)
                        {
                            LinetypeTable LTT = Base_Tools45.LineType.getLineTypeTable();
                            if (LTT.Has(nameLineType) == false)
                            {
                                BaseObjs._db.LoadLineTypeFile(nameLineType, "acad.lin");
                            }
                            Ltr.LinetypeObjectId = LTT[nameLineType];
                        }
                        tr.Commit();
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Layer.cs: line: 346");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nameLayer"></param>
        public static void
        setLayer(ObjectId id, string nameLayer)
        {
            Layer.manageLayers(nameLayer);
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                    ent.Layer = nameLayer;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Layer.cs: line: 370");
            }
        }

        // manageLayers
        // manageLayers
    }
}
