using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using System.Collections.Generic;
using System.Linq;

namespace Base_Tools45.C3D
{
    /// <summary>
    ///
    /// </summary>
    public static class CgPnts
    {
        /// <summary>
        ///
        /// </summary>
        public static PointGroupCollection
        _pointGroups
        {
            get
            {
                return BaseObjs._civDoc.PointGroups;
            }
        }

        public static PointGroup
        addPntGroup(string nameGroup)
        {
            PointGroup pntGrp = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    if (_pointGroups.Contains(nameGroup))
                    {
                        ObjectId grpId = _pointGroups[nameGroup];
                        pntGrp = (PointGroup)tr.GetObject(grpId, OpenMode.ForRead);
                    }
                    else
                    {
                        ObjectId grpId = _pointGroups.Add(nameGroup);
                        pntGrp = (PointGroup)tr.GetObject(grpId, OpenMode.ForRead);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnts.cs: line: 39", ex.Message));
            }
            finally
            {
            }
            return pntGrp;
        }

        public static void
        addPntNameToGroup(string namePnt, string nameGroup)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    if (_pointGroups.Contains(nameGroup))
                    {
                        ObjectId grpId = _pointGroups[nameGroup];
                        PointGroup pntGrp = (PointGroup)tr.GetObject(grpId, OpenMode.ForWrite);
                        StandardPointGroupQuery query = new StandardPointGroupQuery();
                        query.IncludeNames = string.Format("{0}*", namePnt);
                        pntGrp.SetQuery(query);
                        pntGrp.Update();
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnts.cs: line: 61", ex.Message));
            }
        }

        public static void
        checkPntGroup(string nameGroup)
        {
            ObjectId id = getPntLabelStyle(nameGroup);

            PointGroup pntGrp;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    LabelStyle LS = (LabelStyle)tr.GetObject(id, OpenMode.ForRead);
                    if (_pointGroups.Contains(nameGroup))
                    {
                        ObjectId grpId = _pointGroups[nameGroup];
                        pntGrp = (PointGroup)tr.GetObject(grpId, OpenMode.ForWrite);
                    }
                    else
                    {
                        pntGrp = addPntGroup(nameGroup);
                    }

                    StandardPointGroupQuery query = new StandardPointGroupQuery();
                    query.IncludeNames = "SPNT*";
                    pntGrp.SetQuery(query);
                    pntGrp.Update();
                    pntGrp.PointLabelStyleId = LS.ObjectId;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnts.cs: line: 90", ex.Message));
            }
        }

        public static ObjectId
        getPntLabelStyle(string name)
        {
            LabelStyle LS = null;
            ObjectId id = ObjectId.Null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    LabelStyleCollection labelStyles = BaseObjs._civDoc.Styles.LabelStyles.PointLabelStyles.LabelStyles;
                    if (labelStyles.Contains(name))
                    {
                        LS = (LabelStyle)tr.GetObject(labelStyles[name], OpenMode.ForRead);
                        return LS.ObjectId;
                    }
                    else
                    {
                        Base_Tools45.Layer.manageLayers(name);

                        TextStyleTableRecord TStr = Base_Tools45.Txt.getTextStyleTableRecord("L100");
                        if (TStr == null)
                        {
                            TStr = Base_Tools45.Txt.addTextStyleTableRecord("L100");
                        }

                        TStr.FileName = "ROMANS";
                        TStr.XScale = 0.8;

                        CivilDocument civDoc = CivilApplication.ActiveDocument;

                        id = civDoc.Styles.LabelStyles.PointLabelStyles.LabelStyles.Add(name);

                        LS = (LabelStyle)tr.GetObject(id, OpenMode.ForWrite);

                        LS.AddComponent("PointNumber", LabelStyleComponentType.Text);
                        ObjectIdCollection ids = LS.GetComponents(LabelStyleComponentType.Text);
                        LabelStyleTextComponent lstc1 = (LabelStyleTextComponent)tr.GetObject(ids[0], OpenMode.ForWrite);
                        lstc1.General.Visible.Value = true;

                        LS.Properties.DraggedStateComponents.DisplayType.Value = Autodesk.Civil.LabelContentDisplayType.StackedText;

                        tr.Commit();
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnts.cs: line: 133", ex.Message));
            }
            return id;
        }

        public static PointStyle
        getPntStyle(string namePntStyle)
        {
            PointStyle pntStyle = null;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    PointStyleCollection pntStyles = BaseObjs._civDoc.Styles.PointStyles;

                    foreach (ObjectId id in pntStyles)
                    {
                        pntStyle = (PointStyle)tr.GetObject(id, OpenMode.ForRead);
                        if (pntStyle.Name == namePntStyle)
                        {
                            return pntStyle;
                        }
                    }
                    ObjectId idx = pntStyles.Add(namePntStyle);
                    pntStyle = (PointStyle)tr.GetObject(idx, OpenMode.ForRead); //pntStyle not defined                    
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnts.cs: line: 159", ex.Message));
            }
            return pntStyle;
        }

        public static uint
        getNextPointNumber(uint lowLim, uint uppLim)
        {
            uint pntNum = 0;
            CogoPointCollection idsPnts = BaseObjs._civDoc.CogoPoints;
            List<uint> pntNums = new List<uint>();
            foreach (ObjectId id in idsPnts)
            {
                CogoPoint cgPnt = (CogoPoint)id.getEnt();
                pntNums.Add(cgPnt.PointNumber);
            }

            var sortNums = from n in pntNums
                           orderby n descending
                           select n;
            foreach (var n in sortNums)
            {
                pntNum = n;
                break;
            }

            return pntNum + 1;
        }

        public static double
        setPointElevation(this ObjectId id, string result)
        {
            double elev = 0.0;
            double diff = 0.0;
            string optor = string.Empty;
            result = result.Trim();
            string firstChar = result.Substring(0, 1);

            if (firstChar == "+" || firstChar == "-")
            {
                optor = result.Substring(0, 1);
                diff = double.Parse(result.Substring(1));
            }
            else
            {
                double.TryParse(result.Substring(0), out elev);
                optor = "=";
            }

            CogoPoint pnt = null;
            Document doc = Application.DocumentManager.MdiActiveDocument;
            try
            {
                using (Transaction tr = doc.TransactionManager.StartTransaction())
                {
                    pnt = (CogoPoint)tr.GetObject(id, OpenMode.ForWrite);
                    switch (optor)
                    {
                        case "=":
                            pnt.Elevation = elev;
                            break;

                        case "+":
                            pnt.Elevation = pnt.Elevation + diff;
                            break;

                        case "-":
                            pnt.Elevation = pnt.Elevation - diff;
                            break;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnts.cs: line: 200", ex.Message));
            }
            return pnt.Elevation;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="desc"></param>
        /// <returns></returns>
        public static string
        setup(string desc)
        {
            desc = desc.ToUpper();
            short color = 0;
            string namePntLabelStyle = string.Empty;

            switch (desc)
            {
                case "CPNT-ON":
                    color = 2;
                    namePntLabelStyle = "CPNT";
                    break;

                case "CPNT-trANS":
                    color = 6;
                    namePntLabelStyle = "CPNT";
                    break;

                case "CPNT-ST":
                    color = 4;
                    namePntLabelStyle = "CPNT";
                    break;

                case "CPNT-MISC":
                    color = 5;
                    namePntLabelStyle = "CPNT";
                    break;

                case "EXIST":
                    color = 1;
                    namePntLabelStyle = "EXIST";
                    break;

                case "SPNT":
                    color = 1;
                    namePntLabelStyle = "SPNT";
                    break;

                case "UTL-MISC":
                case "UTL-SEW":
                case "UTL-SD":
                case "UTL-WAT":
                    color = 3;
                    namePntLabelStyle = "UTL";
                    break;

                default:
                    if (desc.Substring(0, 2) == "CP")
                    {
                        namePntLabelStyle = "CP";
                    }
                    else
                    {
                        namePntLabelStyle = "SPNT";
                    }
                    break;
            }

            Layer.manageLayers(desc, color);
            Layer.manageLayers(namePntLabelStyle);

            //Pnt_Style.upgradePntLabelStyle(namePntLabelStyle);
            //if (namePntLabelStyle != "SPNT-LABEL" && namePntLabelStyle != "CP-LABEL")
            //{
            //    Layer.manageLayer(namePntLabelStyle, 0, false, false);
            //}

            return namePntLabelStyle;
        }

        public static void
        updatePntGroup(string strGroupName)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
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
                        BaseObjs.writeDebug(string.Format("{0} Pnts.cs: line: 281", ex.Message));
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnts.cs: line: 287", ex.Message));
            }
        }
    }
}