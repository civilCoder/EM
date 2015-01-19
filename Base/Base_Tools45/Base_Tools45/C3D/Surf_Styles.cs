using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using System;
using System.Collections.Generic;

namespace Base_Tools45.C3D
{
    /// <summary>
    ///
    /// </summary>
    public static class Surf_Styles
    {
        /// <summary>
        ///
        /// </summary>
        public static void
        addSurfaceStyleSettings(string nameStyle)
        {
            ObjectId styleId = findSurfaceStyle(nameStyle);
            if (styleId == ObjectId.Null)
            {
                styleId = BaseObjs._civDoc.Styles.SurfaceStyles.Add(nameStyle);
            }

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    SurfaceStyle style = (SurfaceStyle)styleId.GetObject(OpenMode.ForWrite);
                    SurfaceDisplayStyleType[] settings = {
                        SurfaceDisplayStyleType.MajorContour,
                        SurfaceDisplayStyleType.MinorContour,
                        SurfaceDisplayStyleType.Points,
                        SurfaceDisplayStyleType.Triangles
                    };

                    applySettings(style, settings);

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Surf_Styles.cs: line: 40", ex.Message));
            }
        }

        public static ObjectId
        findSurfaceStyle(string name)
        {
            ObjectId idStyle = ObjectId.Null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    foreach (ObjectId id in BaseObjs._civDoc.Styles.SurfaceStyles)
                    {
                        SurfaceStyle style = (SurfaceStyle)id.GetObject(OpenMode.ForRead);
                        if (style.Name == name)
                        {
                            idStyle = id;
                            break;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Surf_Styles.cs: line: 59", ex.Message));
            }
            return idStyle;
        }

        public static ObjectId
        getSurfaceContourLabelStyleId(string name)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    if (BaseObjs._civDoc.Styles.LabelStyles.SurfaceLabelStyles.ContourLabelStyles.Contains(name))
                    {
                        return BaseObjs._civDoc.Styles.LabelStyles.SurfaceLabelStyles.ContourLabelStyles[name];
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Surf_Styles.cs: line: 75", ex.Message));
            }
            return ObjectId.Null;
        }

        public static SurfaceStyle
        getSurfaceStyle(ObjectId idStyle){
            SurfaceStyle style = null;

            using(Transaction tr = BaseObjs.startTransactionDb()){
                style = (SurfaceStyle)tr.GetObject(idStyle, OpenMode.ForRead);
            }

            return style;
        }

        public static ObjectId
        getSurfaceStyle(string nameStyle)
        {
            ObjectId idStyle = ObjectId.Null;
            idStyle = findSurfaceStyle(nameStyle);
            if (idStyle == ObjectId.Null)
            {
                idStyle = BaseObjs._civDoc.Styles.SurfaceStyles.Add(nameStyle);
                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        SurfaceStyle style = (SurfaceStyle)idStyle.GetObject(OpenMode.ForWrite);
                        style.BoundaryStyle.DisplayMode = SurfaceDisplay3dType.UseSurfaceElevation;
                        style.BoundaryStyle.DisplayExteriorBoundaries = true;
                        style.BoundaryStyle.DisplayInteriorBoundaries = true;
                        style.BoundaryStyle.UseDatum = false;

                        style.ContourStyle.GroupRangeValuesBy = SurfaceGroupValuesByType.Quantile;
                        style.ContourStyle.NumberOfRanges = 1;
                        style.ContourStyle.RangePrecision = PrecisionRangeType.Precision000001;
                        style.ContourStyle.UseColorScheme = false;
                        style.ContourStyle.DisplayMode = SurfaceDisplay3dType.UseSurfaceElevation;
                        style.ContourStyle.LegendStyleName = "Standard";
                        style.ContourStyle.BaseElevationInterval = 0.000;
                        style.ContourStyle.MinorContourInterval = 0.200;
                        style.ContourStyle.MajorContourInterval = 1.000;
                        style.ContourStyle.DisplayDepressions = false;
                        style.ContourStyle.SmoothContours = false;

                        style.GridStyle.DisplayMode = SurfaceDisplay3dType.UseSurfaceElevation;
                        style.GridStyle.UsePrimaryGrid = true;
                        style.GridStyle.PrimaryGridInterval = 25.00;
                        style.GridStyle.PrimaryGridOrientation = System.Math.PI / 2;
                        style.GridStyle.UseSecondaryGrid = true;
                        style.GridStyle.SecondaryGridInterval = 25.00;
                        style.GridStyle.SecondaryGridOrientation = 0.0;

                        style.PointStyle.DisplayMode = SurfaceDisplay3dType.UseSurfaceElevation;
                        style.PointStyle.ScalingMethodType = ScaleType.SizeInAbsoluteUnits;
                        style.PointStyle.Units = 3.000;
                        style.PointStyle.DataPointsSymbol = PointSymbolType.K2;
                        style.PointStyle.DataPointsColor = Color.FromColorIndex(ColorMethod.ByLayer, 1);
                        style.PointStyle.DerivedPointsSymbol = PointSymbolType.K34;
                        style.PointStyle.DerivedPointsColor = Color.FromColorIndex(ColorMethod.ByLayer, 3);
                        style.PointStyle.NondestructivePointsSymbol = PointSymbolType.K66;
                        style.PointStyle.NondestructivePointsColor = Color.FromColorIndex(ColorMethod.ByLayer, 11);

                        style.TriangleStyle.DisplayMode = SurfaceDisplay3dType.UseSurfaceElevation;

                        style.DirectionStyle.ColorScheme = new ColorSchemeType();
                        style.DirectionStyle.GroupValuesBy = SurfaceGroupValuesByType.Quantile;
                        style.DirectionStyle.NumberOfRanges = 8;
                        style.DirectionStyle.RangePrecision = PrecisionRangeType.Precision000001;
                        style.DirectionStyle.DisplayEntityMode = SurfaceDisplayType.Solid2d;
                        style.DirectionStyle.LegendStyleName = "Standard";
                        style.DirectionStyle.DisplayMode = SurfaceDisplay3dType.UseSurfaceElevation;

                        style.ElevationStyle.ColorScheme = ColorSchemeType.Blues;
                        style.ElevationStyle.GroupValuesBy = SurfaceGroupValuesByType.Quantile;
                        style.ElevationStyle.NumberOfRanges = 8;
                        style.ElevationStyle.RangePrecision = PrecisionRangeType.Precision000001;
                        style.ElevationStyle.DisplayEntityMode = SurfaceDisplayType.Solid2d;
                        style.ElevationStyle.LegendStyleName = "Standard";
                        style.ElevationStyle.DisplayMode = SurfaceDisplay3dType.UseSurfaceElevation;

                        style.SlopeStyle.ColorScheme = ColorSchemeType.Reds;
                        style.SlopeStyle.GroupValuesBy = SurfaceGroupValuesByType.Quantile;
                        style.SlopeStyle.NumberOfRanges = 10;
                        style.SlopeStyle.RangePrecision = PrecisionRangeType.Precision000001;
                        style.SlopeStyle.LegendStyleName = "Standard";
                        style.SlopeStyle.DisplayMode = SurfaceDisplay3dType.UseSurfaceElevation;

                        style.SlopeArrowStyle.ColorScheme = ColorSchemeType.Rainbow;
                        style.SlopeArrowStyle.GroupValuesBy = SurfaceGroupValuesByType.Quantile;
                        style.SlopeArrowStyle.NumberOfRanges = 8;
                        style.SlopeArrowStyle.RangePrecision = PrecisionRangeType.Precision000001;
                        style.SlopeArrowStyle.ArrowType = SlopeArrowType.Filled;
                        style.SlopeArrowStyle.ArrowScale = 5;
                        style.SlopeArrowStyle.LegendStyleName = "Standard";
                        style.SlopeArrowStyle.DisplayMode = SurfaceDisplay3dType.UseSurfaceElevation;

                        Layer.manageLayers("SURF_POINTS");
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.Points).Layer = "SURF_POINTS";
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.Points).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 1);

                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.Points).Layer = "SURF_POINTS";
                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.Points).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 1);

                        Layer.manageLayers("SURF_trIANGLES");
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.Triangles).Layer = "SURF_trIANGLES";
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.Triangles).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 3);

                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.Triangles).Layer = "SURF_trIANGLES";
                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.Triangles).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 3);

                        Layer.manageLayers("SURF_BORDER");
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.Boundary).Layer = "SURF_BORDER";
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.Boundary).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 2);

                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.Boundary).Layer = "SURF_BORDER";
                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.Boundary).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 2);

                        Layer.manageLayers("SURF_CMAJOR");
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.MajorContour).Layer = "SURF_CMAJOR";
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.MajorContour).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 4);
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.MajorContour).Visible = true;

                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.MajorContour).Layer = "SURF_CMAJOR";
                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.MajorContour).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 4);
                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.MajorContour).Visible = true;

                        Layer.manageLayers("SURF_CMINOR");
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.MinorContour).Layer = "SURF_CMINOR";
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.MinorContour).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 9);
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.MinorContour).Visible = true;

                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.MinorContour).Layer = "SURF_CMINOR";
                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.MinorContour).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 9);
                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.MinorContour).Visible = true;

                        Layer.manageLayers("SURF_USERCONTOURS");
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.UserContours).Layer = "SURF_USERCONTOURS";
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.UserContours).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 6);

                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.UserContours).Layer = "SURF_USERCONTOURS";
                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.UserContours).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 6);

                        Layer.manageLayers("SURF_GRIDDED");
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.Gridded).Layer = "SURF_GRIDDED";
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.Gridded).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 5);

                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.Gridded).Layer = "SURF_GRIDDED";
                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.Gridded).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 5);

                        Layer.manageLayers("SURF_DIRECTIONS");
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.Directions).Layer = "SURF_DIRECTIONS";
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.Directions).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 2);

                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.Directions).Layer = "SURF_DIRECTIONS";
                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.Directions).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 2);

                        Layer.manageLayers("SURF_ELEVATIONS");
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.Elevations).Layer = "SURF_ELEVATIONS";
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.Elevations).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 2);

                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.Elevations).Layer = "SURF_ELEVATIONS";
                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.Elevations).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 2);

                        Layer.manageLayers("SURF_SLOPES");
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.Slopes).Layer = "SURF_SLOPES";
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.Slopes).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 2);

                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.Slopes).Layer = "SURF_SLOPES";
                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.Slopes).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 2);

                        Layer.manageLayers("SURF_SLOPE ARROWS");
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.SlopeArrows).Layer = "SURF_SLOPE ARROWS";
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.SlopeArrows).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 2);

                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.SlopeArrows).Layer = "SURF_SLOPE ARROWS";
                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.SlopeArrows).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 2);

                        Layer.manageLayers("SURF_WATERSHED");
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.Watersheds).Layer = "SURF_WATERSHED";
                        style.GetDisplayStyleModel(SurfaceDisplayStyleType.Watersheds).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 2);

                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.Watersheds).Layer = "SURF_WATERSHED";
                        style.GetDisplayStylePlan(SurfaceDisplayStyleType.Watersheds).Color =
                            Color.FromColorIndex(ColorMethod.ByLayer, 2);

                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(string.Format("{0} Surf_Styles.cs: line: 274", ex.Message));
                }
            }
            return idStyle;
        }

        public static T
        With<T>(this T item, Action<T> action)
        {
            action(item);
            return item;
        }

        private static void
        applySettings(SurfaceStyle style, IList<SurfaceDisplayStyleType> settings)
        {
            IEnumerable<SurfaceDisplayStyleType> displayTypes =
                (IEnumerable<SurfaceDisplayStyleType>)Enum.GetValues(typeof(SurfaceDisplayStyleType));

            foreach (SurfaceDisplayStyleType displayType in displayTypes)
            {
                bool state = settings.Contains(displayType);                //if type is in list -> true then visible -> true;
                style.GetDisplayStylePlan(displayType).Visible = state;
                style.GetDisplayStyleModel(displayType).Visible = state;
            }
        }
    }
}