using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using System.Collections.Generic;

namespace Base_Tools45.C3D
{
    /// <summary>
    ///
    /// </summary>
    public static class CgPnt_Group
    {
        public static PointGroupCollection
        _pointGroups
        {
            get
            {
                return BaseObjs._civDoc.PointGroups;
            }
        }


        public static List<pntGroupParams> pntGroups = new List<pntGroupParams> {
            new pntGroupParams {
                name = "CP",
                key = "C",
                color = 1,
                nameLayerLabel = "CP-LABEL",
                layerOff = false,
                layerFrozen = true},
            new pntGroupParams {
                name = "CPNT-JOIN",
                key = "J",
                color = 32, nameLayerLabel = "CPNT-LABEL",
                layerOff = false,
                layerFrozen = true},
            new pntGroupParams {
                name = "CPNT-MISC",
                key = "M",
                color = 5,
                nameLayerLabel = "CPNT-LABEL",
                layerOff = false,
                layerFrozen = true},
            new pntGroupParams {
                name = "CPNT-ON",
                key = "O",
                color = 2,
                nameLayerLabel = "CPNT-LABEL",
                layerOff = false,
                layerFrozen = true},
            new pntGroupParams {
                name = "CPNT-ST",
                key = "S",
                color = 4,
                nameLayerLabel = "CPNT-LABEL",
                layerOff = false,
                layerFrozen = true},
            new pntGroupParams {
                name = "CPNT-TRANS",
                key = "T",
                color = 6,
                nameLayerLabel = "CPNT-LABEL",
                layerOff = false,
                layerFrozen = true},
            new pntGroupParams {
                name = "EXIST",
                key = "E",
                color = 1,
                nameLayerLabel = "EXIST-LABEL",
                layerOff = false,
                layerFrozen = true},
            new pntGroupParams {
                name = "SPNT",
                key = "SP",
                color = 1,
                nameLayerLabel = "SPNT-LABEL",
                layerOff = false,
                layerFrozen = false},
            new pntGroupParams {
                name = "UTL-FIRE",
                key = "FIRE",
                color = 11,
                nameLayerLabel = "UTL-LABEL",
                layerOff = false,
                layerFrozen = true},
            new pntGroupParams {
                name = "UTL-SD",
                key = "SD",
                color = 3,
                nameLayerLabel = "UTL-LABEL",
                layerOff = false,
                layerFrozen = true},
            new pntGroupParams {
                name = "UTL-SEW",
                key = "SE",
                color = 3,
                nameLayerLabel = "UTL-LABEL",
                layerOff = false,
                layerFrozen = true},
            new pntGroupParams {
                name = "UTL-MISC",
                key = "MI",
                color = 3,
                 nameLayerLabel = "UTL-LABEL",
                layerOff = false,
                layerFrozen = true},
            new pntGroupParams {
                name = "UTL-WAT",
                key = "W",
                color = 4,
                nameLayerLabel = "UTL-LABEL",
                layerOff = false,
                layerFrozen = true}
            };

        public static PointGroup
        addPntGroup(string nameGroup, out bool exists)
        {
            exists = false;
            PointGroup pntGrp = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDoc())
                {
                    try
                    {
                        if (_pointGroups.Contains(nameGroup))
                        {
                            ObjectId grpId = _pointGroups[nameGroup];
                            pntGrp = (PointGroup)tr.GetObject(grpId, OpenMode.ForRead);
                            exists = true;
                        }
                        else
                        {
                            exists = false;
                            ObjectId grpId = _pointGroups.Add(nameGroup);
                            pntGrp = (PointGroup)tr.GetObject(grpId, OpenMode.ForRead);
                        }
                        StandardPointGroupQuery query = new StandardPointGroupQuery();
                        query.IncludeRawDescriptions = string.Format("{0}*", nameGroup);
                        pntGrp.SetQuery(query);
                        pntGrp.Update();
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Pnt_Group.cs: line: 210", ex.Message));
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnt_Group.cs: line: 217", ex.Message));
            }
            return pntGrp;
        }

        public static void
        addPntRawDescToGroup(string namePnt, string nameGroup)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDoc())
                {
                    try
                    {
                        if (_pointGroups.Contains(nameGroup))
                        {
                            ObjectId grpId = _pointGroups[nameGroup];
                            PointGroup pntGrp = (PointGroup)tr.GetObject(grpId, OpenMode.ForWrite);
                            StandardPointGroupQuery query = new StandardPointGroupQuery();
                            query.IncludeRawDescriptions = string.Format("{0}*", nameGroup);
                            pntGrp.SetQuery(query);
                            pntGrp.Update();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Pnt_Group.cs: line: 243", ex.Message));
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnt_Group.cs: line: 250", ex.Message));
            }
        }

        /// <summary>
        /// update specified PointGroup
        /// </summary>
        /// <param name="strGroupName"></param>
        public static ObjectId
        checkPntGroup(string pntDesc)
        {
            ObjectId idPntGrp = ObjectId.Null;
            ObjectId idPntLblStyle = Pnt_Style.getPntLabelStyle(pntDesc);
            ObjectId idPntStyle = Pnt_Style.getPntStyle(pntDesc);
            PointGroup pntGrp = null;

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                try
                {
                    if (_pointGroups.Contains(pntDesc))
                    {
                        idPntGrp = _pointGroups[pntDesc];
                        pntGrp = (PointGroup)tr.GetObject(idPntGrp, OpenMode.ForWrite);
                    }
                    else
                    {
                        idPntGrp = _pointGroups.Add(pntDesc);
                        pntGrp = (PointGroup)tr.GetObject(idPntGrp, OpenMode.ForWrite);
                    }

                    if(pntGrp != null){
                        StandardPointGroupQuery query = new StandardPointGroupQuery();
                        query.IncludeRawDescriptions = string.Format("{0}", pntDesc);
                        pntGrp.SetQuery(query);
                        pntGrp.PointLabelStyleId = idPntLblStyle;
                        pntGrp.PointStyleId = idPntStyle;
                        pntGrp.Update();                            
                    }else{
                        return ObjectId.Null;
                    }
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(string.Format("{0} Pnt_Group.cs: line: 289", ex.Message));
                }
                tr.Commit();
            }
            return idPntGrp;
        }

        /// <summary>
        /// check if point group exists, if not, create point group
        /// </summary>
        /// <param name="nameGroup"></param>
        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        public static void
        deletePntGroup(string name)
        {
            try
            {
                BaseObjs._civDoc.PointGroups.Remove(name);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnt_Group.cs: line: 317", ex.Message));
            }
        }

        public static void
        updatePntGroup(string strGroupName)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDoc())
                {
                    try
                    {
                        if (_pointGroups.Contains(strGroupName))
                        {
                            ObjectId grpId = _pointGroups[strGroupName];
                            PointGroup pntGrp = (PointGroup)tr.GetObject(grpId, OpenMode.ForWrite);
                            pntGrp.Update();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Pnt_Group.cs: line: 339", ex.Message));
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnt_Group.cs: line: 346", ex.Message));
            }
        }

        public static void
        updatePntGroups()
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDoc())
                {
                    try
                    {
                        foreach (ObjectId id in _pointGroups)
                        {
                            PointGroup pg = (PointGroup)tr.GetObject(id, OpenMode.ForWrite);
                            pg.Update();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Pnt_Group.cs: line: 367", ex.Message));
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnt_Group.cs: line: 374", ex.Message));
            }
        }

        public struct pntGroupParams
        {
            public string name;
            public string key;
            public short color;
            public string nameLayerLabel;
            public bool layerOff;
            public bool layerFrozen;
        }

        public struct pntLabelLayer
        {
            public string nameLayerLabel;
            public bool layerOff;
            public bool layerFrozen;
        }
    }
}