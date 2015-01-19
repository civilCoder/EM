using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices.Styles;
using System;
using System.Collections.Generic;

namespace Base_Tools45.C3D
{
    /// <summary>
    ///
    /// </summary>
    public static class Pnt_Style
    {
        private const string _description   = "<[Full Description(CP)]>";
        private const string _easting       = "<[Easting(Uft|P4|RN|AP|GC|UN|Sn|OF)>";
        private const string _elevation     = "<[Point Elevation(Uft|P2|RN|AP|Sn|OF)]>";
        private const string _northing      = "<[Northing(Uft|P4|RN|AP|GC|UN|Sn|OF)]>";
        private const string _pointNumber   = "<[Point Number(Sn)]>";

        private static LabelStyleCollection _pointLabelStyles
        {
            get
            {
                return BaseObjs._civDoc.Styles.LabelStyles.PointLabelStyles.LabelStyles;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static void
        checkPntLabelStyles()
        {
            try
            {
                using (Transaction tr0 = BaseObjs.startTransactionDb())
                {
                    LabelStyleCollection ids = _pointLabelStyles;
                    Dictionary<string, ObjectId> stylesWith = new Dictionary<string, ObjectId>();
                    Dictionary<string, ObjectId> stylesWithout = new Dictionary<string, ObjectId>();

                    int k = ids.Count;
                    using (Transaction tr1 = BaseObjs.startTransactionDb())
                    {
                        for (int i = 0; i < k; i++)
                        {
                            LabelStyle ls = (LabelStyle)tr1.GetObject(ids[i], OpenMode.ForRead);
                            if (ls.Name.ToUpper().Contains("-LABEL"))
                            {
                                stylesWith.Add(ls.Name, ls.ObjectId);
                            }
                            else
                            {
                                stylesWithout.Add(ls.Name, ls.ObjectId);
                            }
                        }
                        tr1.Commit();
                    }

                    k = ids.Count;
                    for (int i = k - 1; i > -1; i--)
                    {
                        LabelStyle ls = (LabelStyle)tr0.GetObject(ids[i], OpenMode.ForWrite);
                        if (ls.Name.ToUpper().Contains("-LABEL"))
                        {
                            string name = ls.Name.Replace("-LABEL", "");

                            if (!stylesWithout.ContainsKey(name))
                            { //avoid duplicate naming
                                ls.Name = name;
                            }
                            else
                            {
                                if (!ls.IsUsed)
                                {
                                    ids.Remove(ls.Name);
                                }
                                else
                                {
                                    ids.Remove(name);
                                }
                            }
                        }
                        if (ls.CreateBy.Contains("Autodesk"))
                        {
                            try
                            {
                                if (!ls.IsUsed)
                                    ids.Remove(i);
                            }
                            catch (System.Exception ex)
                            {
                                BaseObjs.writeDebug(string.Format("{0} Pnt_Style.cs: line: 85", ex.Message));
                            }
                        }
                    }
                    tr0.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnt_Style.cs: line: 94", ex.Message));
            }
        }

        /// <summary>
        /// Return point label style if it exists, if not, create point label style
        /// </summary>
        /// <param name="name"></param>
        public static ObjectId
        getPntLabelStyle(string name)
        {
            switch (name)
            { 
                case "BOT":
                    name = "BOT";
                    break;
                case "CP":
                    name = "CP";
                    break;

                case "CPNT-JOIN":
                case "CPNT-MISC":
                case "CPNT-ON":
                case "CPNT-ST":
                case "CPNT-TRANS":
                    name = "CPNT";
                    break;

                case "SPNT":
                    name = "SPNT";
                    break;

                case "UTL-MISC":
                case "UTL-SD":
                case "UTL-SEW":
                case "UTL-WAT":
                    name = "UTL";
                    break;

                case "EXIST":
                    name = "EXIST";
                    break;
            }
            LabelStyle ls = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    Layer.manageLayers(name);
                    Layer.manageLayers(string.Format("{0}-LABEL", name));
                    if (name == "SPNT")
                        Layer.manageLayer(string.Format("{0}-LABEL", name), layerOff: false, layerFrozen: false);
                    else
                        Layer.manageLayer(string.Format("{0}-LABEL", name), layerOff: false, layerFrozen: true);

                    LabelStyleCollection labelStyles = BaseObjs._civDoc.Styles.LabelStyles.PointLabelStyles.LabelStyles;
                    if (labelStyles.Contains(name))
                    {
                        ls = (LabelStyle)tr.GetObject(labelStyles[name], OpenMode.ForRead);
                        if (!ls.Properties.Label.Layer.Value.Contains("-LABEL"))
                        {
                            ls.UpgradeOpen();
                            ls.Properties.Label.Layer.Value = string.Format("{0}-LABEL", name);
                        }
                    }
                    else
                    {
                        TextStyleTableRecord tstr = Txt.getTextStyleTableRecord("L100");
                        if (tstr == null)
                        {
                            tstr = Txt.addTextStyleTableRecord("L100");
                        }
                        else
                        {
                            tstr.UpgradeOpen();
                            tstr.FileName = "ROMANS";
                            tstr.XScale = 0.8;
                        }

                        CivilDocument civDoc = BaseObjs._civDoc;
                        ObjectId id = civDoc.Styles.LabelStyles.PointLabelStyles.LabelStyles.Add(name);
                        ls = (LabelStyle)tr.GetObject(id, OpenMode.ForWrite);
                        ls.Properties.DraggedStateComponents.DisplayType.Value = Autodesk.Civil.LabelContentDisplayType.StackedText;
                        ls.Properties.Label.Layer.Value = string.Format("{0}-LABEL", name);

                        removeAllComponents(id);

                        addLocationZcomponent(ls);
                        addDescriptionComponent(ls);
                        addPointNumberComponent(ls);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnt_Style.cs: line: 172", ex.Message));
            }
            return ls.ObjectId;
        }

        /// <summary>
        /// Check if point style exists, if not, create style
        /// </summary>
        /// <param name="namePntStyle"></param>
        /// <returns>Point Style</returns>
        public static ObjectId
        getPntStyle(string namePntStyle)
        {
            PointStyle pntStyle = null;
            ObjectId idPntStyle = ObjectId.Null;

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
                            idPntStyle = pntStyle.ObjectId;
                            break;
                        }
                    }
                    if (idPntStyle == ObjectId.Null)
                    {
                        ObjectId idx = pntStyles.Add(namePntStyle);
                        pntStyle = (PointStyle)tr.GetObject(idx, OpenMode.ForWrite);
                        pntStyle.MarkerType = PointMarkerDisplayType.UsePointForMarker;
                        idPntStyle = pntStyle.ObjectId;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnt_Style.cs: line: 207", ex.Message));
            }
            return idPntStyle;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameFontFile"></param>
        /// <param name="xscale"></param>
        /// <returns></returns>
        public static ObjectId
        upgradePntLabelStyle(string nameLabelStyle, string nameFontFile = "Romans.shx", double xscale = 0.8)
        {
            LabelStyle ls = null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    LabelStyleCollection labelStyles = BaseObjs._civDoc.Styles.LabelStyles.PointLabelStyles.LabelStyles;
                    if (labelStyles.Contains(nameLabelStyle))
                    {
                        labelStyles.Remove(nameLabelStyle);
                    }

                    //Layer.manageLayers(name);

                    TextStyleTableRecord TStr = Txt.getTextStyleTableRecord("L100");
                    if (TStr == null)
                    {
                        TStr = Txt.addTextStyleTableRecord("L100");
                    }

                    TStr.FileName = nameFontFile;
                    TStr.XScale = xscale;

                    CivilDocument civDoc = BaseObjs._civDoc;

                    ObjectId id = civDoc.Styles.LabelStyles.PointLabelStyles.LabelStyles.Add(nameLabelStyle);

                    ls = (LabelStyle)tr.GetObject(id, OpenMode.ForWrite);
                    ls.Properties.DraggedStateComponents.DisplayType.Value = Autodesk.Civil.LabelContentDisplayType.StackedText;

                    addLocationZcomponent(ls);
                    addPointNumberComponent(ls);
                    addDescriptionComponent(ls);

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnt_Style.cs: line: 254", ex.Message));
            }
            return ls.ObjectId;
        }

        private static void
        addDescriptionComponent(LabelStyle style)
        {
            ObjectId id = style.AddComponent("DESC", LabelStyleComponentType.Text);
            LabelStyleTextComponent textComp = (LabelStyleTextComponent)id.GetObject(OpenMode.ForWrite);
            textComp.Text.Attachment.Value = LabelTextAttachmentType.TopLeft;
            textComp.Text.Contents.Value = _description;
            textComp.General.AnchorComponent.Value = "Z";
            textComp.General.AnchorLocation.Value = AnchorPointType.BottomLeft;
        }

        private static void
        addLabelStyleComponents(ObjectId idStyle)
        {
            LabelStyle style = (LabelStyle)idStyle.GetObject(OpenMode.ForWrite);
            addLeaderComponent(style);
            addDescriptionComponent(style);
            addPointNumberComponent(style);
            addLocationZcomponent(style);
            //addLocationNEcomponent(style);
            //addLocationXYcomponent(style);
        }

        private static void
        addLeaderComponent(LabelStyle style)
        {
            ObjectId id = style.AddComponent("LDR", LabelStyleComponentType.Line);
            LabelStyleLineComponent lineComp = (LabelStyleLineComponent)id.GetObject(OpenMode.ForWrite);
            lineComp.General.StartAnchorPoint.Value = AnchorPointType.MiddleCenter;
        }

        private static void
        addLocationNEcomponent(LabelStyle style)
        {
            ObjectId id = style.AddComponent("NE", LabelStyleComponentType.Text);
            LabelStyleTextComponent textComp = (LabelStyleTextComponent)id.GetObject(OpenMode.ForWrite);
            textComp.Text.Attachment.Value = LabelTextAttachmentType.TopLeft;
            string value = String.Format("({0}, {1},{2})", _northing, _easting, _elevation);
            textComp.Text.Contents.Value = value;
            textComp.General.AnchorComponent.Value = "PN";
            textComp.General.AnchorLocation.Value = AnchorPointType.BottomLeft;
        }

        private static void
        addLocationXYcomponent(LabelStyle style)
        {
            ObjectId id = style.AddComponent("XY", LabelStyleComponentType.Text);
            LabelStyleTextComponent textComp = (LabelStyleTextComponent)id.GetObject(OpenMode.ForWrite);
            textComp.Text.Attachment.Value = LabelTextAttachmentType.TopLeft;
            string value = string.Format("(X={0}, Y={1}, Z={2})", _easting, _northing, _elevation);
            textComp.Text.Contents.Value = value;
            textComp.General.AnchorComponent.Value = "DESC";
            textComp.General.AnchorLocation.Value = AnchorPointType.BottomLeft;
        }

        private static void
        addLocationZcomponent(LabelStyle style)
        {
            ObjectId id = style.AddComponent("Z", LabelStyleComponentType.Text);
            LabelStyleTextComponent textComp = (LabelStyleTextComponent)id.GetObject(OpenMode.ForWrite);
            textComp.Text.Attachment.Value = LabelTextAttachmentType.MiddleLeft;
            textComp.Text.Contents.Value = _elevation;
            //textComp.General.AnchorComponent.Value =                              ******defaults to <Feature> when value is unassigned*************
            textComp.General.AnchorLocation.Value = AnchorPointType.MiddleRight;
        }

        private static void
        addPointNumberComponent(LabelStyle style)
        {
            ObjectId id = style.AddComponent("PN", LabelStyleComponentType.Text);
            LabelStyleTextComponent textComp = (LabelStyleTextComponent)id.GetObject(OpenMode.ForWrite);
            textComp.Text.Attachment.Value = LabelTextAttachmentType.BottomLeft;
            textComp.Text.Contents.Value = _pointNumber;
            textComp.General.AnchorComponent.Value = "Z";
            textComp.General.AnchorLocation.Value = AnchorPointType.TopLeft;
        }

        private static void
        customizeStyle(ObjectId idStyle)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    addLabelStyleComponents(idStyle);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnt_Style.cs: line: 330", ex.Message));
            }
        }

        private static IEnumerable<string>
        getTextComponentNames(ObjectId idStyle)
        {
            List<string> names = new List<string>();
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    LabelStyle style = (LabelStyle)idStyle.GetObject(OpenMode.ForRead);
                    foreach (ObjectId id in style.GetComponents(LabelStyleComponentType.Text))
                    {
                        LabelStyleComponent lsc = (LabelStyleComponent)id.GetObject(OpenMode.ForRead);
                        names.Add(lsc.Name);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnt_Style.cs: line: 347", ex.Message));
            }
            return names;
        }

        private static void
        removeAllComponents(ObjectId idStyle)
        {
            IEnumerable<string> names = getTextComponentNames(idStyle);
            removeComponents(idStyle, names);
        }

        private static void
        removeComponents(ObjectId idStyle, IEnumerable<string> nameComponents)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    LabelStyle style = (LabelStyle)idStyle.GetObject(OpenMode.ForWrite);
                    foreach (string name in nameComponents)
                    {
                        style.RemoveComponent(name);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Pnt_Style.cs: line: 368", ex.Message));
            }
        }
    }
}