using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using System.Collections.Generic;

namespace Base_Tools45.C3D
{
    public static class Prof_Style
    {
        public static ObjectId
        CreateProfileLabelSetStyle(string name)
        {
            CivilDocument civDoc = CivilApplication.ActiveDocument;
            ObjectIdCollection ids = null;
            ObjectId id = ObjectId.Null;
            ProfileLabelSetStyle oProfileLabelSetStyle = null;

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ObjectIdCollection idStyles = new ObjectIdCollection();

                    id = civDoc.Styles.LabelSetStyles.ProfileLabelSetStyles.Add(name);
                    oProfileLabelSetStyle = (ProfileLabelSetStyle)tr.GetObject(id, OpenMode.ForWrite);

                    try
                    {
                        ObjectId idGradeBreakLabelStyle = civDoc.Styles.LabelStyles.ProfileLabelStyles.GradeBreakLabelStyles.Add(name);
                        idStyles.Add(idGradeBreakLabelStyle);
                        LabelStyle oGradeBreakLabelStyle = (LabelStyle)tr.GetObject(idGradeBreakLabelStyle, OpenMode.ForWrite);

                        ids = oGradeBreakLabelStyle.GetComponents(LabelStyleComponentType.Line);
                        if (ids.Count != 0)
                        {
                            foreach (ObjectId idLS in ids)
                            {
                                var lc = (LabelStyleLineComponent)tr.GetObject(idLS, OpenMode.ForWrite);
                                lc.General.Visible.Value = false;
                            }
                        }
                        else
                        {
                            id = oGradeBreakLabelStyle.AddComponent("Line", LabelStyleComponentType.Line);
                            var lc = (LabelStyleLineComponent)tr.GetObject(id, OpenMode.ForWrite);
                            lc.General.Visible.Value = false;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Prof_Style.cs: line: 45", ex.Message));
                    }

                    try
                    {
                        ObjectId idCurveLabelStyle = civDoc.Styles.LabelStyles.ProfileLabelStyles.CurveLabelStyles.Add(name);
                        //idStyles.Add(idCurveLabelStyle);
                        LabelStyle oCurveLabelStyle = (LabelStyle)tr.GetObject(idCurveLabelStyle, OpenMode.ForWrite);

                        ids = oCurveLabelStyle.GetComponents(LabelStyleComponentType.Line);
                        if (ids.Count != 0)
                        {
                            foreach (ObjectId idLS in ids)
                            {
                                var lc = (LabelStyleLineComponent)tr.GetObject(idLS, OpenMode.ForWrite);
                                lc.General.Visible.Value = false;
                            }
                        }
                        else
                        {
                            id = oCurveLabelStyle.AddComponent("Line", LabelStyleComponentType.Line);
                            var lc = (LabelStyleLineComponent)tr.GetObject(id, OpenMode.ForWrite);
                            lc.General.Visible.Value = false;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Prof_Style.cs: line: 66", ex.Message));
                    }

                    try
                    {
                        ObjectId idHorizontalGeometryPointLabelStyle = civDoc.Styles.LabelStyles.ProfileLabelStyles.HorizontalGeometryPointLabelStyles.Add(name);
                        idStyles.Add(idHorizontalGeometryPointLabelStyle);
                        LabelStyle oHorizontalGeometryPointLabelStyle = (LabelStyle)tr.GetObject(idHorizontalGeometryPointLabelStyle, OpenMode.ForWrite);
                        ids = oHorizontalGeometryPointLabelStyle.GetComponents(LabelStyleComponentType.Line);
                        if (ids.Count != 0)
                        {
                            foreach (ObjectId idLS in ids)
                            {
                                var lc = (LabelStyleLineComponent)tr.GetObject(idLS, OpenMode.ForWrite);
                                lc.General.Visible.Value = false;
                            }
                        }
                        else
                        {
                            id = oHorizontalGeometryPointLabelStyle.AddComponent("Line", LabelStyleComponentType.Line);
                            var lc = (LabelStyleLineComponent)tr.GetObject(id, OpenMode.ForWrite);
                            lc.General.Visible.Value = false;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Prof_Style.cs: line: 86", ex.Message));
                    }

                    try
                    {
                        ObjectId idLineLabelStyle = civDoc.Styles.LabelStyles.ProfileLabelStyles.LineLabelStyles.Add(name);
                        idStyles.Add(idLineLabelStyle);
                        LabelStyle oLineLabelStyle = (LabelStyle)tr.GetObject(idLineLabelStyle, OpenMode.ForWrite);
                        ids = oLineLabelStyle.GetComponents(LabelStyleComponentType.Line);
                        if (ids.Count != 0)
                        {
                            foreach (ObjectId idLS in ids)
                            {
                                var lc = (LabelStyleLineComponent)tr.GetObject(idLS, OpenMode.ForWrite);
                                lc.General.Visible.Value = false;
                            }
                        }
                        else
                        {
                            id = oLineLabelStyle.AddComponent("Line", LabelStyleComponentType.Line);
                            var lc = (LabelStyleLineComponent)tr.GetObject(id, OpenMode.ForWrite);
                            lc.General.Visible.Value = false;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Prof_Style.cs: line: 106", ex.Message));
                    }

                    try
                    {
                        ObjectId idMinorStationLabelStyle = civDoc.Styles.LabelStyles.ProfileLabelStyles.MinorStationLabelStyles.Add(name);
                        //idStyles.Add(idMinorStationLabelStyle);
                        LabelStyle oMinorStationLabelStyle = (LabelStyle)tr.GetObject(idMinorStationLabelStyle, OpenMode.ForWrite);
                        ids = oMinorStationLabelStyle.GetComponents(LabelStyleComponentType.Line);
                        if (ids.Count != 0)
                        {
                            foreach (ObjectId idLS in ids)
                            {
                                var lc = (LabelStyleLineComponent)tr.GetObject(idLS, OpenMode.ForWrite);
                                lc.General.Visible.Value = false;
                            }
                        }
                        else
                        {
                            id = oMinorStationLabelStyle.AddComponent("Line", LabelStyleComponentType.Line);
                            var lc = (LabelStyleLineComponent)tr.GetObject(id, OpenMode.ForWrite);
                            lc.General.Visible.Value = false;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Prof_Style.cs: line: 126", ex.Message));
                    }

                    try
                    {
                        ObjectId idMajorStationLabelStyle = civDoc.Styles.LabelStyles.ProfileLabelStyles.MajorStationLabelStyles.Add(name);
                        idStyles.Add(idMajorStationLabelStyle);
                        LabelStyle oMajorStationLabelStyle = (LabelStyle)tr.GetObject(idMajorStationLabelStyle, OpenMode.ForWrite);
                        ids = oMajorStationLabelStyle.GetComponents(LabelStyleComponentType.Line);
                        if (ids.Count != 0)
                        {
                            foreach (ObjectId idLS in ids)
                            {
                                var lc = (LabelStyleLineComponent)tr.GetObject(idLS, OpenMode.ForWrite);
                                lc.General.Visible.Value = false;
                            }
                        }
                        else
                        {
                            id = oMajorStationLabelStyle.AddComponent("Line", LabelStyleComponentType.Line);
                            var lc = (LabelStyleLineComponent)tr.GetObject(id, OpenMode.ForWrite);
                            lc.General.Visible.Value = false;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(string.Format("{0} Prof_Style.cs: line: 146", ex.Message));
                    }

                    foreach (ObjectId idStyle in idStyles)
                        oProfileLabelSetStyle.Add(idStyle);

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof_Style.cs: line: 156", ex.Message));
            }
            return oProfileLabelSetStyle.ObjectId;
        }

        public static ObjectId
        CreateProfileStyle(string name)
        {
            CivilDocument civDoc = CivilApplication.ActiveDocument;
            ProfileStyleCollection profileStyles = civDoc.Styles.ProfileStyles;

            ObjectId idProfileStyle = ObjectId.Null;
            try
            {
                idProfileStyle = profileStyles.Add(name);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof_Style.cs: line: 172", ex.Message));
            }

            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ProfileStyle oProfileStyle = (ProfileStyle)tr.GetObject(idProfileStyle, OpenMode.ForWrite);

                    oProfileStyle.GetDisplayStyleProfile(ProfileDisplayStyleProfileType.Arrow).Visible = false;

                    // set to yellow:
                    oProfileStyle.GetDisplayStyleProfile(ProfileDisplayStyleProfileType.Line).Color = Color.FromColorIndex(ColorMethod.ByColor, 50);
                    oProfileStyle.GetDisplayStyleProfile(ProfileDisplayStyleProfileType.Line).Visible = true;

                    // grey
                    oProfileStyle.GetDisplayStyleProfile(ProfileDisplayStyleProfileType.LineExtension).Color = Color.FromColorIndex(ColorMethod.ByColor, 251);
                    oProfileStyle.GetDisplayStyleProfile(ProfileDisplayStyleProfileType.LineExtension).Visible = true;

                    // green
                    oProfileStyle.GetDisplayStyleProfile(ProfileDisplayStyleProfileType.Curve).Color = Color.FromColorIndex(ColorMethod.ByColor, 80);
                    oProfileStyle.GetDisplayStyleProfile(ProfileDisplayStyleProfileType.Curve).Visible = true;

                    // grey
                    oProfileStyle.GetDisplayStyleProfile(ProfileDisplayStyleProfileType.ParabolicCurveExtension).Color = Color.FromColorIndex(ColorMethod.ByColor, 251);
                    oProfileStyle.GetDisplayStyleProfile(ProfileDisplayStyleProfileType.ParabolicCurveExtension).Visible = true;

                    // green
                    oProfileStyle.GetDisplayStyleProfile(ProfileDisplayStyleProfileType.SymmetricalParabola).Color = Color.FromColorIndex(ColorMethod.ByColor, 81);
                    oProfileStyle.GetDisplayStyleProfile(ProfileDisplayStyleProfileType.SymmetricalParabola).Visible = true;

                    // green
                    oProfileStyle.GetDisplayStyleProfile(ProfileDisplayStyleProfileType.AsymmetricalParabola).Color = Color.FromColorIndex(ColorMethod.ByColor, 83);
                    oProfileStyle.GetDisplayStyleProfile(ProfileDisplayStyleProfileType.AsymmetricalParabola).Visible = true;                    

                    // properties for 3D should also be set

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof_Style.cs: line: 210", ex.Message));
            }

            return idProfileStyle;
        }

        public static ObjectId
        CreateProfileViewStyle(string name){
            ObjectId idPVStyle = ObjectId.Null;

            ProfileViewStyleCollection pvStyleCol = BaseObjs._civDoc.Styles.ProfileViewStyles;
            try{
                idPVStyle = pvStyleCol[name];
            }
            catch{}

            if (!idPVStyle.IsNull)
                return idPVStyle;

            idPVStyle = pvStyleCol.Add(name);
            using(var tr = BaseObjs.startTransactionDb()){
                ProfileViewStyle pvStyle = (ProfileViewStyle)tr.GetObject(idPVStyle, OpenMode.ForWrite);
                
                AxisStyle axisStyle = pvStyle.BottomAxis;                
                axisStyle.MajorTickStyle.Size = 0.01;
                axisStyle.MajorTickStyle.Interval = 50;
                axisStyle.TitleStyle.Text = "Stations";
                
                axisStyle = pvStyle.TopAxis;
                axisStyle.TitleStyle.Text = "Feet";

                tr.Commit();
            }

            return idPVStyle;
        }

        public static ObjectId
        CreateProfileViewBandSetStyle(string name){

            string PROFILEVIEW_BANDSET_STYLE_NAME = name;
            string PROFILEVIEW_PROFILE_BAND_STYLE_NAME = name;

            ObjectId idPVBandSetStyle = ObjectId.Null;

            ProfileViewBandSetStyleCollection pvBandSetStyleCol = BaseObjs._civDoc.Styles.ProfileViewBandSetStyles;
            try{
                idPVBandSetStyle = pvBandSetStyleCol[PROFILEVIEW_BANDSET_STYLE_NAME];                
            }
            catch{}

            if (!idPVBandSetStyle.IsNull)
                return idPVBandSetStyle;

            idPVBandSetStyle = BaseObjs._civDoc.Styles.ProfileViewBandSetStyles.Add(PROFILEVIEW_BANDSET_STYLE_NAME);

            using(var tr = BaseObjs.startTransactionDb()){
                ProfileViewBandSetStyle pvBandSetStyle = (ProfileViewBandSetStyle)tr.GetObject(idPVBandSetStyle, OpenMode.ForWrite);
                ObjectId id = ObjectId.Null;

                try{
                    id = BaseObjs._civDoc.Styles.BandStyles.ProfileViewProfileDataBandStyles[PROFILEVIEW_PROFILE_BAND_STYLE_NAME];
                }
                catch{}

                if(!id.IsNull)
                    return idPVBandSetStyle;

                id = BaseObjs._civDoc.Styles.BandStyles.ProfileViewProfileDataBandStyles.Add(PROFILEVIEW_PROFILE_BAND_STYLE_NAME);
                ProfileDataBandStyle pDataBandStyle = (ProfileDataBandStyle)tr.GetObject(id, OpenMode.ForWrite);

                DisplayStyle displayStyle = pDataBandStyle.GetDisplayStylePlan(ProfileDataDisplayStyleType.LabelsAtVGP);
                displayStyle.Visible = true;

                displayStyle = pDataBandStyle.GetDisplayStylePlan(ProfileDataDisplayStyleType.TicksAtHGP);
                displayStyle.Color = clr.red;
                displayStyle.Visible = true;

                id = pDataBandStyle.HGPLabelStyleId;
                LabelStyle labelStyle = (LabelStyle)tr.GetObject(id, OpenMode.ForWrite);
                
                ObjectIdCollection ids = labelStyle.GetComponents(LabelStyleComponentType.Text);
                id = ids[0];

                LabelStyleTextComponent labelStyleText = (LabelStyleTextComponent)tr.GetObject(id, OpenMode.ForWrite);
                labelStyleText.Text.Contents.Value = "<[Station Value(Uft|FS|P0|RN|AP|Sn|TP|B2|EN|W0|OF)]>";
                labelStyleText.Text.Color.Value = clr.c11;

                displayStyle = pDataBandStyle.GetDisplayStylePlan(ProfileDataDisplayStyleType.TitleBox);
                displayStyle.Color = clr.red;
                displayStyle.Linetype = "ByBlock";
                displayStyle.Visible = true;

                displayStyle = pDataBandStyle.GetDisplayStylePlan(ProfileDataDisplayStyleType.TitleBoxText);
                displayStyle.Color = clr.grn;
                displayStyle.Visible = true;

                id = pDataBandStyle.TitleTextLabelStyleId;
                labelStyle = (LabelStyle)tr.GetObject(id, OpenMode.ForWrite);
                
                ids = labelStyle.GetComponents(LabelStyleComponentType.Text);             
                id = ids[0];
                labelStyleText = (LabelStyleTextComponent)tr.GetObject(id, OpenMode.ForWrite);
                labelStyleText.Text.Contents.Value = "Profile Data: XX";
                labelStyleText.Text.Height.Value = 0.09;

                pDataBandStyle.GetDisplayStylePlan(ProfileDataDisplayStyleType.MajorStationLabel).Visible = false;
                pDataBandStyle.GetDisplayStylePlan(ProfileDataDisplayStyleType.MajorTicks).Visible = false;
                pDataBandStyle.GetDisplayStylePlan(ProfileDataDisplayStyleType.MinorStationLabel).Visible = false;
                pDataBandStyle.GetDisplayStylePlan(ProfileDataDisplayStyleType.MinorTicks).Visible = false;
                pDataBandStyle.GetDisplayStylePlan(ProfileDataDisplayStyleType.TicksAtVGP).Visible = false;
                pDataBandStyle.GetDisplayStylePlan(ProfileDataDisplayStyleType.TicksAtStationEquations).Visible = false;
                pDataBandStyle.GetDisplayStylePlan(ProfileDataDisplayStyleType.LabelsAtVGP).Visible = false;
                pDataBandStyle.GetDisplayStylePlan(ProfileDataDisplayStyleType.LabelsAtStationEquations).Visible = false;

                pvBandSetStyle.GetBottomBandSetItems().Add(BaseObjs._db, Autodesk.Civil.BandType.ProfileData, PROFILEVIEW_PROFILE_BAND_STYLE_NAME);
            }



            return idPVBandSetStyle;
        }

        public static ObjectId
        getProfileLabelSetStyle(string name)
        {
            ObjectId idStyle = ObjectId.Null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ProfileLabelSetStyleCollection styles = BaseObjs._civDoc.Styles.LabelSetStyles.ProfileLabelSetStyles;
                    if (styles.Contains(name))
                        idStyle = styles[name];
                    else
                        idStyle = CreateProfileLabelSetStyle(name);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof_Style.cs: line: 230", ex.Message));
            }
            return idStyle;
        }

        public static List<string>
        getProfileLabelSetStyles()
        {
            List<string> s = new List<string>();
            ObjectId idStyle = ObjectId.Null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ProfileLabelSetStyleCollection styles = BaseObjs._civDoc.Styles.LabelSetStyles.ProfileLabelSetStyles;
                    foreach (ObjectId id in styles)
                    {
                        ProfileLabelSetStyle style = (ProfileLabelSetStyle)tr.GetObject(id, OpenMode.ForRead);
                        s.Add(style.Name);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof_Style.cs: line: 230", ex.Message));
            }
            return s;
        }

        public static ObjectId
        getProfileStyle(string name)
        {
            ObjectId idStyle = ObjectId.Null;
            CivilDocument civDoc = CivilApplication.ActiveDocument;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ProfileStyleCollection styles = BaseObjs._civDoc.Styles.ProfileStyles;
                    if (styles.Contains(name))
                        idStyle = styles[name];
                    else
                        idStyle = CreateProfileStyle(name);
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof_Style.cs: line: 250", ex.Message));
            }
            return idStyle;
        }

        public static List<string>
        getProfileStyles()
        {
            List<string> styles = new List<string>();
            CivilDocument civDoc = CivilApplication.ActiveDocument;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ProfileStyleCollection profileStyles = BaseObjs._civDoc.Styles.ProfileStyles;
                    foreach (ObjectId idProfileStyle in profileStyles)
                    {
                        ProfileStyle profileStyle = (ProfileStyle)tr.GetObject(idProfileStyle, OpenMode.ForRead);
                        styles.Add(profileStyle.Name);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof_Style.cs: line: 250", ex.Message));
            }
            return styles;
        }

        public static ObjectId
        getProfileViewStyle(string name)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ProfileViewStyleCollection styles = BaseObjs._civDoc.Styles.ProfileViewStyles;
                    if (styles.Contains(name))
                        return styles[name];

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof_Style.cs: line: 267", ex.Message));
            }
            return ObjectId.Null;
        }

        public static List<string>
        getProfileViewStyles()
        {
            List<string> profileViewStyles = new List<string>();
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ProfileViewStyleCollection styles = BaseObjs._civDoc.Styles.ProfileViewStyles;
                    foreach (ObjectId id in styles)
                    {
                        ProfileViewStyle profileViewStyle = (ProfileViewStyle)tr.GetObject(id, OpenMode.ForRead);
                        profileViewStyles.Add(profileViewStyle.Name);
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof_Style.cs: line: 267", ex.Message));
            }
            return profileViewStyles;
        }

        public static void
        ModProfileViewStyle(string name)
        {
            CivilDocument civDoc = CivilApplication.ActiveDocument;
            ObjectId idPViewStyle;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    if (civDoc.Styles.ProfileViewStyles.Contains(name))
                        idPViewStyle = civDoc.Styles.ProfileViewStyles[name];
                    else
                        return;

                    ProfileViewStyle oProfileViewStyle = (ProfileViewStyle)tr.GetObject(idPViewStyle, OpenMode.ForRead);

                    // Adjust the top axis.  Put station information here, with the title at the far left.
                    oProfileViewStyle.GetDisplayStylePlan(ProfileViewDisplayStyleType.TopAxis).Visible = true;
                    oProfileViewStyle.TopAxis.MajorTickStyle.LabelText = "<[Station Value(Um|FD|P1)]> m";
                    oProfileViewStyle.TopAxis.MajorTickStyle.Interval = 164.041995;
                    oProfileViewStyle.TopAxis.TitleStyle.OffsetX = 0.13;
                    oProfileViewStyle.TopAxis.TitleStyle.OffsetY = 0.0;
                    oProfileViewStyle.TopAxis.TitleStyle.Text = "Meters";
                    oProfileViewStyle.TopAxis.TitleStyle.Location = Autodesk.Civil.DatabaseServices.Styles.AxisTitleLocationType.TopOrLeft;

                    // Adjust the title to show the alignment name
                    oProfileViewStyle.GraphStyle.TitleStyle.Text = "Profile View of:<[Parent Alignment(CP)]>";

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof_Style.cs: line: 301", ex.Message));
            }
        }

        public static void
        removeProfileLabelGroup(ObjectId pViewID, ObjectId profileID)
        {
            ObjectIdCollection ids = ProfileLabelGroup.GetAvailableLabelGroupIds(RXClass.GetClass(typeof(ProfileLabelGroup)), pViewID, profileID, false);
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDoc())
                {
                    foreach (ObjectId id in ids)
                    {
                        ProfileLabelGroup group = (ProfileLabelGroup)tr.GetObject(id, OpenMode.ForWrite);
                        group.Erase();
                    }
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof_Style.cs: line: 318", ex.Message));
            }
        }

        public static ObjectId
        getProfileViewBandSetStyle(string name)
        {
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ProfileViewBandSetStyleCollection styles = BaseObjs._civDoc.Styles.ProfileViewBandSetStyles;
                    if (styles.Contains(name))
                        return styles[name];
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof_Style.cs: line: 333", ex.Message));
            }
            return ObjectId.Null;
        }

        public static ObjectId
        getProfileViewBandSetStyle()
        {
            ObjectId idBandStyle = ObjectId.Null;
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ProfileViewBandSetStyleCollection styles = BaseObjs._civDoc.Styles.ProfileViewBandSetStyles;
                    idBandStyle = styles[0];
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Prof_Style.cs: line: 333", ex.Message));
            }
            return idBandStyle;
        }
    }
}