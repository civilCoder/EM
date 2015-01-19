using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Windows.Forms;
using App = Autodesk.AutoCAD.ApplicationServices.Application;

namespace EW
{
    public static class EW_MakeSurfaceBot
    {
        static bool exists = false;

        public static void
        setPointLabelStyleBOT()
        {
            ObjectId idPntLabelStyle = Pnt_Style.getPntLabelStyle("BOT");
            Layer.manageLayers("BOT-POINT-LABEL");

            using(var tr = BaseObjs.startTransactionDb()){
                LabelStyle labelStyle = (LabelStyle)tr.GetObject(idPntLabelStyle, OpenMode.ForWrite);
                labelStyle.Properties.Label.Layer.Value = "BOT-POINT-LABEL";
                labelStyle.Properties.Label.Visibility.Value = false;
                tr.Commit();
            }
        }


        public static void
        setPointStyleBOT()
        {
            ObjectId idPntStyle = BaseObjs._civDoc.Styles.PointStyles["BOT"];
            PointStyle objPntStyle = default(PointStyle);

            if ((idPntStyle.IsNull))
            {
                objPntStyle = CgPnts.getPntStyle("BOT");
            }

            objPntStyle.MarkerType = PointMarkerDisplayType.UsePointForMarker;
            objPntStyle.GetMarkerDisplayStyleModel().Color = clr.byl;
            objPntStyle.GetMarkerDisplayStyleModel().Visible = true;
            objPntStyle.GetMarkerDisplayStylePlan().Color = clr.byl;
            objPntStyle.GetMarkerDisplayStylePlan().Visible = true;
            objPntStyle.GetLabelDisplayStyleModel().Color = clr.byl;
            objPntStyle.GetLabelDisplayStyleModel().Visible = true;
            objPntStyle.GetLabelDisplayStylePlan().Color = clr.byl;
            objPntStyle.GetLabelDisplayStylePlan().Visible = true;
        }

        public static object[]
        makeBotSurfaceGrid(int intInterval, int i1)
        {
            App.SetSystemVariable("PDSISE", 0.01);

            setPointStyleBOT();
            setPointLabelStyleBOT();
            bool exists;
            TinSurface objSurfaceCPNT_ON = Surf.getTinSurface("CPNT-ON", out exists);

            Object[] varLimits = EW_Utility2.getLimits(intInterval);
            
            double dblPntXmin = (double)varLimits[0];
            double dblPntYmin = (double)varLimits[1];

            int iMax = (int)varLimits[2];
            int jMax = (int)varLimits[3];


            for (int j = 0; j <= jMax; j++)
            {
                double dblY = dblPntYmin + (j * intInterval);


                for (int i = 0; i <= iMax; i++)
                {
                    double dblX = dblPntXmin + (i * intInterval);

                    double dblZ_OX = objSurfaceCPNT_ON.FindElevationAtXY(dblX, dblY);

                    
                    if (dblZ_OX > 0)    //point is inside OX boundary
                    {
                        Point3d dblPnt = new Point3d(dblX, dblY, dblZ_OX);
                        ObjectId idCivilPnt = dblPnt.setPoint("GRID-POINTS");
                    }

                }

            }

            TypedValue[] tvs = new TypedValue[4]{
                new TypedValue(1001, "BOT"),
                new TypedValue(1040, varLimits[0]),
                new TypedValue(1040, varLimits[1]),
                new TypedValue(1070, intInterval)
            };
            ObjectId idDictBOT = Dict.getNamedDictionary("BOT", out exists);
            idDictBOT.setXData(tvs, "BOT");

            return varLimits;

        }

        public static void makeSurfaceBOT()
        {
            string strSurfaceName = "BOT";
            EW_CheckSurfaceStyles.checkSurfaceStyles(strSurfaceName);

            Surf.removeSurface("BOT");
            TinSurface objSurfaceBOT = Surf.addTinSurface("BOT", out exists);

            ObjectId idCivilPntGrp = CgPnt_Group.checkPntGroup("BOT");
            objSurfaceBOT.PointGroupsDefinition.AddPointGroup(idCivilPntGrp);

            SelectionSet objSSet = EW_Utility1.buildSSet13();
            ObjectId[] idsBrksArray = objSSet.GetObjectIds();
            ObjectIdCollection idsBrks = new ObjectIdCollection();
            foreach (ObjectId id in idsBrksArray)
                idsBrks.Add(id);

            objSurfaceBOT.BreaklinesDefinition.AddStandardBreaklines(idsBrks, 1.0, 0, 0, 0);

            ObjectId idLWPline = EW_Utility1.buildSSetGradingLim().GetObjectIds()[0];

            if (idLWPline != ObjectId.Null)
            {
                ObjectIdCollection ids = new ObjectIdCollection();
                ids.Add(idLWPline);
                objSurfaceBOT.BoundariesDefinition.AddBoundaries(ids, 1, Autodesk.Civil.SurfaceBoundaryType.Outer, true);
                objSurfaceBOT.Rebuild();
            }
            else
            {
                MessageBox.Show("GRADING LIMIT not found - OUTER BOUNDARY not added.");
            }

        }

    }
}
