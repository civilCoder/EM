using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using SectionViewStyle = Autodesk.Civil.DatabaseServices.Styles.SectionViewStyle;

namespace Base_Tools45.C3D
{
    /// <summary>
    ///
    /// </summary>
    public static class Sect_Style
    {
        public static GroupPlotStyle
        getGroupPlotStyle(string name)
        {
            GroupPlotStyleCollection styles = BaseObjs._civDoc.Styles.GroupPlotStyles;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    foreach (ObjectId style in styles)
                    {
                        GroupPlotStyle gStyle = (GroupPlotStyle)tr.GetObject(style, OpenMode.ForRead);
                        if (gStyle.Name == name)
                        {
                            return gStyle;
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Sect_Style.cs: line: 27", ex.Message));
            }
            return null;
        }

        public static SectionLabelSetStyle
        getSampleLineLabelStyle(string name)
        {
            SectionLabelSetStyleCollection styles = BaseObjs._civDoc.Styles.LabelSetStyles.SectionLabelSetStyles;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    foreach (ObjectId style in styles)
                    {
                        SectionLabelSetStyle slStyle = (SectionLabelSetStyle)tr.GetObject(style, OpenMode.ForRead);
                        if (slStyle.Name == name)
                        {
                            return slStyle;
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Sect_Style.cs: line: 48", ex.Message));
            }
            return null;
        }

        public static SampleLineStyle
        getSampleLineStyle(string name)
        {
            SampleLineStyleCollection styles = BaseObjs._civDoc.Styles.SampleLineStyles;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    foreach (ObjectId style in styles)
                    {
                        SampleLineStyle slStyle = (SampleLineStyle)tr.GetObject(style, OpenMode.ForRead);
                        if (slStyle.Name == name)
                        {
                            return slStyle;
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Sect_Style.cs: line: 69", ex.Message));
            }
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ObjectId
        getSectionStyleId(string name)
        {
            SectionStyleCollection styles = BaseObjs._civDoc.Styles.SectionStyles;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    foreach (ObjectId style in styles)
                    {
                        SectionStyle sStyle = (SectionStyle)tr.GetObject(style, OpenMode.ForRead);
                        if (sStyle.Name == name)
                        {
                            return sStyle.ObjectId;
                        }
                    }

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Sect_Style.cs: line: 95", ex.Message));
            }
            return ObjectId.Null;
        }

        public static SectionViewBandSetStyle
        getSectionViewBandSetStyle(string name)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    SectionViewBandSetStyleCollection styles = BaseObjs._civDoc.Styles.SectionViewBandSetStyles;
                    foreach (ObjectId style in styles)
                    {
                        SectionViewBandSetStyle svbsStyle = (SectionViewBandSetStyle)tr.GetObject(style, OpenMode.ForRead);
                        if (svbsStyle.Name == name)
                        {
                            return svbsStyle;
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Sect_Style.cs: line: 115", ex.Message));
            }
            return null;
        }

        public static SectionViewStyle
        getSectionViewStyle(string name)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    SectionViewStyleCollection styles = BaseObjs._civDoc.Styles.SectionViewStyles;
                    foreach (ObjectId style in styles)
                    {
                        SectionViewStyle svStyle = (SectionViewStyle)tr.GetObject(style, OpenMode.ForRead);
                        if (svStyle.Name == name)
                        {
                            return svStyle;
                        }
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Sect_Style.cs: line: 135", ex.Message));
            }
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
    }
}