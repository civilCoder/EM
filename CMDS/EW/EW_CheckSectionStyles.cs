
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices.Styles;
using Base_Tools45;
using Base_Tools45.C3D;

namespace EW
{
    public static class EW_CheckSectionStyles
    {
        public static void
        checkSectionStyles()
        {
            makeSectionStyle("EXIST");
            makeSectionStyle("CPNT");
            makeSectionStyle("SG");
            makeSectionStyle("OX");
            makeSectionStyle("BOT");
        }

        public static void
        makeSectionStyle(string strName)
        {
            string nameLayer = string.Format("{0}-SURFACE-SEC", strName);
            ObjectId idLayer = Layer.manageLayers(nameLayer);
            Color c = clr.byl;
            switch (strName)
            {
                case "CPNT":
                    Layer.modifyLayer(idLayer, 4,  LineWeight.ByLayer);
                    c = clr.cyn;
                    break;
                case "EXIST":
                    Layer.modifyLayer(idLayer, 8, LineWeight.ByLayer, "DASHED");
                    c = clr.c8;
                    break;
                case "SG":
                    Layer.modifyLayer(idLayer, 1, LineWeight.ByLayer);
                    c = clr.red;
                    break;
                case "OX":
                    Layer.modifyLayer(idLayer, 3, LineWeight.ByLayer);
                    c = clr.grn;
                    break;
                case "BOT":
                    Layer.modifyLayer(idLayer, 5, LineWeight.ByLayer);
                    c = clr.blu;
                    break;
            }

            ObjectId idStyle = Sect_Style.getSectionStyleId(strName);
            using(var tr = BaseObjs.startTransactionDb()){
                SectionStyle objSectionStyle = (SectionStyle)tr.GetObject(idStyle, OpenMode.ForWrite);
                objSectionStyle.CreateBy = "EarthWork";
                DisplayStyle ds = objSectionStyle.GetDisplayStyleSection(SectionDisplayStyleSectionType.Segments);
                ds.Visible = true;
                ds.Layer = nameLayer;
                ds.Color = c;
                ds.Linetype = "ByLayer";
                tr.Commit();
            }         
        }

        public static void
        makeSectionLabelStyles()
        {
            CivilDocument civDoc = BaseObjs._civDoc;

            LabelStyle objLabelStyle = null;
            using(var tr = BaseObjs.startTransactionDb()){

                 ObjectId idLabelStyle = civDoc.Styles.LabelStyles.SectionLabelStyles.MajorOffsetLabelStyles.Add("EW");
                 objLabelStyle = (LabelStyle)tr.GetObject(idLabelStyle, OpenMode.ForWrite);                               

                //Set objMajorStationSetItem = objSectionLabelStyleSet.MajorStationLabelSet.Add(objLabelStyle, 100#)

                idLabelStyle = civDoc.Styles.LabelStyles.SectionLabelStyles.MinorOffsetLabelStyles.Add("EW");
                objLabelStyle = (LabelStyle)tr.GetObject(idLabelStyle, OpenMode.ForWrite);                               
                objLabelStyle.Properties.Label.Visibility.Value = false;

                //Set objMinorStationSetItem = objSectionLabelStyleSet.MinorStationLabelSet.Add(objLabelStyle, 50#, objMajorStationSetItem)

                idLabelStyle = civDoc.Styles.LabelStyles.SectionLabelStyles.GradeBreakLabelStyles.Add("EW");
                objLabelStyle = (LabelStyle)tr.GetObject(idLabelStyle, OpenMode.ForWrite);
                objLabelStyle.Properties.Label.Visibility.Value = false;


                //Set objGradeBreakSetItem = objSectionLabelStyleSet.GradeBreaksLabelSet.Add(objLabelStyle)

                idLabelStyle = civDoc.Styles.LabelStyles.SectionLabelStyles.SegmentLabelStyles.Add("EW");
                objLabelStyle = (LabelStyle)tr.GetObject(idLabelStyle, OpenMode.ForWrite);
                objLabelStyle.Properties.Label.Visibility.Value = false;


                //Set objSegmentSetItem = objSectionLabelStyleSet.SegmentLabelSet.Add(objLabelStyle)
            
                tr.Commit();
            }

        }

        
        public static void      //NOT PRACTICAL - use style in template
        makeSectionVeiwStyle()
        {
            ObjectId idSectionViewStyle = BaseObjs._civDoc.Styles.SectionViewStyles.Add("EW");
        }
    }
}
