using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using System;
using System.Collections.Generic;

namespace Base_Tools45.C3D
{
    public static class Align_Style
    {
        public static ObjectId
        createAlignStyle(string name)
        {
            ObjectId idStyle = ObjectId.Null;
            try
            {
                idStyle = BaseObjs._civDoc.Styles.AlignmentStyles.Add(name);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Align_Style.cs: line: 18", ex.Message));
            }
            AlignmentStyle style = null;
            if (idStyle != ObjectId.Null)
            {
                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        style = (AlignmentStyle)tr.GetObject(idStyle, OpenMode.ForWrite);
                        style.GetDisplayStyleModel(AlignmentDisplayStyleType.Arrow).Visible = false;
                        // Do not show direction arrows.  Styles do not govern
                        // the size of direction arrows - that is done in the
                        // ambient settings.
                        style.GetDisplayStyleModel(AlignmentDisplayStyleType.Arrow).Visible = false;
                        style.GetDisplayStylePlan(AlignmentDisplayStyleType.Arrow).Visible = false;
                        // Display curves using violet.
                        style.GetDisplayStyleModel(AlignmentDisplayStyleType.Curve).Color = Autodesk.AutoCAD.Colors.Color.FromRgb(191, 0, 255); // violet
                        style.GetDisplayStylePlan(AlignmentDisplayStyleType.Curve).Color = Autodesk.AutoCAD.Colors.Color.FromRgb(191, 0, 255);  // violet
                        style.GetDisplayStyleModel(AlignmentDisplayStyleType.Curve).Visible = true;
                        style.GetDisplayStylePlan(AlignmentDisplayStyleType.Curve).Visible = true;
                        // Display straight sections in blue.
                        style.GetDisplayStyleModel(AlignmentDisplayStyleType.Line).Color = Autodesk.AutoCAD.Colors.Color.FromRgb(0, 0, 255); // blue
                        style.GetDisplayStylePlan(AlignmentDisplayStyleType.Line).Color = Autodesk.AutoCAD.Colors.Color.FromRgb(0, 0, 255); // blue
                        style.GetDisplayStyleModel(AlignmentDisplayStyleType.Line).Visible = true;
                        style.GetDisplayStylePlan(AlignmentDisplayStyleType.Line).Visible = true;

                        style.EnableRadiusSnap = true;

                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(string.Format("{0} Align_Style.cs: line: 48", ex.Message));
                }
            }
            return idStyle;
        }

        public static ObjectId
        createAlignLabelSetStyle(string name)
        {
            AlignmentLabelSetStyle alignLabelSetStyle = null;
            ObjectId idStyle = ObjectId.Null;
            ObjectId idItem = ObjectId.Null;

            try
            {
                idStyle = BaseObjs._civDoc.Styles.LabelSetStyles.AlignmentLabelSetStyles[name];
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Align_Style.cs: line: 112", ex.Message));
            }
            if (idStyle == ObjectId.Null)
            {
                idStyle = BaseObjs._civDoc.Styles.LabelSetStyles.AlignmentLabelSetStyles.Add(name);
            }

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    alignLabelSetStyle = (AlignmentLabelSetStyle)tr.GetObject(idStyle, OpenMode.ForWrite);
                    alignLabelSetStyle.Description = "Program Generated";

                    idItem = BaseObjs._civDoc.Styles.LabelStyles.AlignmentLabelStyles.MajorStationLabelStyles.Add("Parallel with line Tick");
                    LabelStyle labelStyle = (LabelStyle)tr.GetObject(idItem, OpenMode.ForWrite);
                    try
                    {
                        labelStyle.RemoveComponent("New Tick");
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Align_Style.cs: line: 129", ex.Message));
                    }

                    try
                    {
                        ObjectId id = labelStyle.AddComponent("Tick", LabelStyleComponentType.Tick);
                        LabelStyleTickComponent cmpnnt = (LabelStyleTickComponent)tr.GetObject(id, OpenMode.ForWrite);
                        cmpnnt.Tick.Lineweight.Value = LineWeight.LineWeight060;
                        cmpnnt.Tick.AlignWithObject.Value = true;
                        cmpnnt.Tick.RotationAngle.Value = 0.0;
                        cmpnnt.General.Visible.Value = true;
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Align_Style.cs: line: 141", ex.Message));
                    }
                    try
                    {
                        labelStyle.RemoveComponent("New Block");
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Align_Style.cs: line: 147", ex.Message));
                    }

                    try
                    {
                        labelStyle.RemoveComponent("New Line");
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Align_Style.cs: line: 154", ex.Message));
                    }

                    try
                    {
                        ObjectId id = labelStyle.AddComponent("Line", LabelStyleComponentType.Line);
                        LabelStyleLineComponent cmpnnt = (LabelStyleLineComponent)tr.GetObject(id, OpenMode.ForWrite);
                        cmpnnt.Line.Color.Value = Autodesk.AutoCAD.Colors.Color.FromRgb(255, 127, 0);
                        cmpnnt.Line.Angle.Value = 2.094;
                        cmpnnt.Line.StartPointXOffset.Value = 0.005;
                        cmpnnt.Line.StartPointYOffset.Value = -0.005;
                        cmpnnt.General.Visible.Value = true;
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Align_Style.cs: line: 167", ex.Message));
                    }

                    try
                    {
                        labelStyle.RemoveComponent("New Text");
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Align_Style.cs: line: 174", ex.Message));
                    }

                    try
                    {
                        ObjectId id = labelStyle.AddComponent("Text", LabelStyleComponentType.Text);
                        LabelStyleTextComponent cmpnnt = (LabelStyleTextComponent)tr.GetObject(id, OpenMode.ForWrite);

                        cmpnnt.Text.XOffset.Value = 60;
                        cmpnnt.Text.YOffset.Value = -0.25 - cmpnnt.Text.Height.Value;
                        cmpnnt.Text.Color.Value = Autodesk.AutoCAD.Colors.Color.FromRgb(0, 255, 0);
                        cmpnnt.Text.Contents.Value = "STA=<[Station Value(Uft|FS|P2|RN|AP|Sn|TP|B2|EN|W0|OF)]>";
                        cmpnnt.General.Visible.Value = true;
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Align_Style.cs: line: 188", ex.Message));
                    }

                    alignLabelSetStyle.Add(labelStyle.ObjectId);

                    AlignmentLabelSetItem stationStyleMajor = alignLabelSetStyle[alignLabelSetStyle.Count - 1];
                    stationStyleMajor.Increment = 20;

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Align_Style.cs: line: 200", ex.Message));
            }
            return idStyle;
        }


        public static ObjectId
        getAlignmentLabelSetStyle(string name)
        {
            ObjectId idStyle = ObjectId.Null;
            AlignmentLabelSetStyleCollection styles = BaseObjs._civDoc.Styles.LabelSetStyles.AlignmentLabelSetStyles;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    foreach (ObjectId id in styles)
                    {
                        AlignmentLabelSetStyle style = (AlignmentLabelSetStyle)tr.GetObject(id, OpenMode.ForRead);
                        if (style.Name == name)
                            idStyle = style.ObjectId;
                        break;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Align_Style.cs: line: 70", ex.Message));
            }
            return idStyle;
        }

        public static List<string>
        getAlignmentLabelSetStyles()
        {
            List<string> styleSets = new List<string>();
            AlignmentLabelSetStyleCollection styles = BaseObjs._civDoc.Styles.LabelSetStyles.AlignmentLabelSetStyles;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    foreach (ObjectId id in styles)
                    {
                        AlignmentLabelSetStyle style = (AlignmentLabelSetStyle)tr.GetObject(id, OpenMode.ForRead);
                        styleSets.Add(style.Name);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Align_Style.cs: line: 70", ex.Message));
            }
            return styleSets;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ObjectId
        getAlignmentStyle(string name)
        {
            ObjectId idStyle = ObjectId.Null;
            AlignmentStyleCollection styles = BaseObjs._civDoc.Styles.AlignmentStyles;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    foreach (ObjectId id in styles)
                    {
                        AlignmentStyle style = (AlignmentStyle)tr.GetObject(id, OpenMode.ForRead);
                        if (style.Name == name)
                            idStyle = style.ObjectId;
                        break;
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Align_Style.cs: line: 96", ex.Message));
            }
            return idStyle;
        }

        public static List<String>
        getAlignStyles()
        {
            List<string> alignStyles = new List<string>();
            AlignmentStyleCollection styles = BaseObjs._civDoc.Styles.AlignmentStyles;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    foreach (ObjectId id in styles)
                    {
                        AlignmentStyle style = (AlignmentStyle)tr.GetObject(id, OpenMode.ForRead);
                        alignStyles.Add(style.Name);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Align_Style.cs: line: 96", ex.Message));
            }
            return alignStyles;
        }

        public static List<String>
        getAlignStyleLabels
            ()
        {
            List<string> alignStyleLabels = new List<string>();
            AlignmentLabelSetStyleCollection styleLabels = BaseObjs._civDoc.Styles.LabelSetStyles.AlignmentLabelSetStyles;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    foreach (ObjectId id in styleLabels)
                    {
                        AlignmentLabelSetStyle style = (AlignmentLabelSetStyle)tr.GetObject(id, OpenMode.ForRead);
                        alignStyleLabels.Add(style.Name);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Align_Style.cs: line: 96", ex.Message));
            }
            return alignStyleLabels;
        }

    }
}