
using Autodesk.AutoCAD.DatabaseServices;

using Autodesk.Civil.DatabaseServices.Styles;
using Base_Tools45;

using s = Autodesk.Civil.DatabaseServices.Styles;

namespace EW
{
    public static class EW_CheckSectionViewStyle
    {
        public static void
        checkSectionViewStyle()
        {

            ObjectId idSectionViewStyle = BaseObjs._civDoc.Styles.SectionViewStyles.Add("EW");
            using(var tr = BaseObjs.startTransactionDb()){
                
                s.SectionViewStyle svs = (s.SectionViewStyle)tr.GetObject(idSectionViewStyle, OpenMode.ForWrite);
                
                //----------------------------------Center Axis-----------------------------------
                DisplayStyle ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.BottomAxis);
                ds.Color = clr.grn;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = true;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.BottomAxisAnnotationMajor);
                ds.Color = clr.grn;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = true;
                
                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.BottomAxisTicksMajor);
                ds.Color = clr.grn;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = false;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.BottomAxisAnnotationMinor);
                ds.Color = clr.yel;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = false;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.BottomAxisTicksMinor);
                ds.Color = clr.yel;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = false;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.BottomAxisTitle);
                ds.Color = clr.yel;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = true;

                //----------------------------------Center Axis-----------------------------------

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.CenterAxis);
                ds.Color = clr.red;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = false;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.CenterAxisAnnotationMajor);
                ds.Color = clr.grn;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = false;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.CenterAxisTicksMajor);
                ds.Color = clr.grn;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = false;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.CenterAxisAnnotationMinor);
                ds.Color = clr.yel;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = false;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.CenterAxisTicksMinor);
                ds.Color = clr.yel;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = false;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.CenterAxisTitle);
                ds.Color = clr.yel;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = false;

                //----------------------------------Graph Style-----------------------------------

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.GraphTitle);
                ds.Color = clr.cyn;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = true;

                //----------------------------------Grid Style-----------------------------------

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.GridHorizontalMajor);
                ds.Color = clr.c8;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = true;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.GridVerticalMajor);
                ds.Color = clr.c8;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = true;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.GridHorizontalMinor);
                ds.Color = clr.c9;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = true;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.GridVerticalMinor);
                ds.Color = clr.c9;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = true;

                //----------------------------------Left Axis-----------------------------------

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.LeftAxis);
                ds.Color = clr.grn;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = true;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.LeftAxisAnnotationMajor);
                ds.Color = clr.grn;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = true;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.LeftAxisTicksMajor);
                ds.Color = clr.grn;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = false;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.LeftAxisAnnotationMinor);
                ds.Color = clr.yel;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = true;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.LeftAxisTicksMinor);
                ds.Color = clr.yel;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = true;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.LeftAxisTitle);
                ds.Color = clr.yel;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = false;
                //----------------------------------Right Axis-----------------------------------

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.RightAxis);
                ds.Color = clr.grn;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = true;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.RightAxisAnnotationMajor);
                ds.Color = clr.grn;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = true;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.RightAxisTicksMajor);
                ds.Color = clr.grn;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = false;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.RightAxisAnnotationMinor);
                ds.Color = clr.yel;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = true;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.RightAxisTicksMinor);
                ds.Color = clr.yel;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = true;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.RightAxisTitle);
                ds.Color = clr.yel;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = false;

                //----------------------------------Top Axis-----------------------------------

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.TopAxis);
                ds.Color = clr.red;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = false;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.TopAxisAnnotationMajor);
                ds.Color = clr.grn;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = false;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.TopAxisTicksMajor);
                ds.Color = clr.grn;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = false;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.TopAxisAnnotationMinor);
                ds.Color = clr.yel;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = false;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.TopAxisTicksMinor);
                ds.Color = clr.yel;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = false;

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.TopAxisTitle);
                ds.Color = clr.yel;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = false;

                //----------------------------------Sample Line-----------------------------------

                ds = svs.GetDisplayStylePlan(SectionViewDisplayStyleType.GridAtSampleLineStations);
                ds.Color = clr.yel;
                ds.Layer = "SECTIONVIEW";
                ds.Visible = true;

                tr.Commit();
            }                       
        }
    }
}
