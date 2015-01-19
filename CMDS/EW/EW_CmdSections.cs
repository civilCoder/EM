using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using Base_Tools45;
using Base_Tools45.C3D;
using SectionViewStyle = Autodesk.Civil.DatabaseServices.Styles.SectionViewStyle;

namespace EW {
    public static class EW_CmdSections {
        public static void
        cmdSections(ObjectId idAlign, double dblOffR, double dblOffL) {

            ObjectId idSectionStyleEXIST = default(ObjectId);
            ObjectId idSectionStyleEXIST90 = default(ObjectId);
            ObjectId idSectionStyleCPNT = default(ObjectId);
            ObjectId idSectionStyleSG = default(ObjectId);
            ObjectId idSectionStyleOX = default(ObjectId);
            ObjectId idSectionStyleOXg = default(ObjectId);
            ObjectId idSectionStyleBOT = default(ObjectId);
            ObjectId idSectionStyleMID = default(ObjectId);

            ObjectIdCollection idsSurface = new ObjectIdCollection();

            bool exists = false;
            ObjectId idSurfaceEXIST = Surf.getSurface("EXIST", out exists);
            if (!exists) {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Surface EXIST is missing - exiting!");
                return;
            }else {
                idsSurface.Add(idSurfaceEXIST);
                idSectionStyleEXIST = Sect_Style.getSectionStyleId("EXIST");
            }

            ObjectId idSurfaceEXIST90 = Surf.getSurface("EXIST90", out exists);
            if (!exists) {
                //  MsgBox "Surface EXIST is missing - exiting!"
                //  Exit Sub
            }else {
                idsSurface.Add(idSurfaceEXIST90);
                idSectionStyleEXIST90 = Sect_Style.getSectionStyleId("EXIST90");
            }

            ObjectId idSurfaceCPNT = Surf.getSurface("CPNT-ON", out exists);
            if (!exists) {
                //  MsgBox "Surface CPNT-ON is missing - exiting!"
                //  Exit Sub
            }else {
                idsSurface.Add(idSurfaceCPNT);
                idSectionStyleCPNT = Sect_Style.getSectionStyleId("CPNT");
            }

            ObjectId idSurfaceSG = Surf.getSurface("SG", out exists);
            if (!exists) {
            }else {
                idsSurface.Add(idSurfaceSG);
                idSectionStyleSG = Sect_Style.getSectionStyleId("SG");
            }

            ObjectId idSurfaceOX = Surf.getSurface("OX", out exists);
            if (!exists) {
            }else {
                idsSurface.Add(idSurfaceOX);
                idSectionStyleOX = Sect_Style.getSectionStyleId("OX");
            }

            ObjectId idSurfaceOXg = Surf.getSurface("OXg", out exists);
            if (!exists) {
            }else {
                idsSurface.Add(idSurfaceOXg);
                idSectionStyleOXg = Sect_Style.getSectionStyleId("OXg");
            }

            ObjectId idSurfaceBOT = Surf.getSurface("BOT", out exists);

            if (!exists) {
            }else {
                idsSurface.Add(idSurfaceBOT);
                idSectionStyleBOT = Sect_Style.getSectionStyleId("BOT");
            }

            if (idSectionStyleBOT == ObjectId.Null) {
                EW_Utility1.copyEWstyles();
            }

            ObjectId idSurfaceMID = Surf.getSurface("MIDGRADE", out exists);
            if (!exists) {
            }else {
                idsSurface.Add(idSurfaceMID);
                idSectionStyleMID = Sect_Style.getSectionStyleId("EXIST90");
            }

            idAlign.removeSampleLineGroups();
            ObjectId idSLG = idAlign.addSampleLineGroupAndSampleLines("EW");

            idSLG.removeSampledSurfaces();

            idSLG.addSurfaceToSample(idsSurface);

            idAlign.addSections(idSLG, dblOffL, dblOffR);

            ObjectId idGroupPlotStyle = ObjectId.Null;
            GroupPlotStyleCollection grpPltStyles = BaseObjs._civDoc.Styles.GroupPlotStyles;
            if (!grpPltStyles.Contains("EW")) {
                EW_Utility1.copyEWstyles();
                idGroupPlotStyle = BaseObjs._civDoc.Styles.GroupPlotStyles["EW"];
            }

            SampleLineStyleCollection sampleLineStyles = BaseObjs._civDoc.Styles.SampleLineStyles;
            ObjectId idSampleLineStyle = sampleLineStyles["Standard"];

            SectionLabelSetStyleCollection sectionLabelSetStyles = BaseObjs._civDoc.Styles.LabelSetStyles.SectionLabelSetStyles;         
            
            double dblWidth = 0;
            double dblHeight = 0;
            Autodesk.AutoCAD.DatabaseServices.Table objTable = default(Autodesk.AutoCAD.DatabaseServices.Table);

            SelectionSet objSSet = default(SelectionSet);

            objSSet = EW_Utility1.buildSSetTable();
            objTable = (Autodesk.AutoCAD.DatabaseServices.Table)objSSet.GetObjectIds()[0].getEnt();

            Point3d varPntIns = objTable.Position;

            dblWidth = objTable.Width;
            dblHeight = objTable.Height;

            Point3d dblPntBase = new Point3d(varPntIns.X + dblWidth + 600, varPntIns.Y - dblHeight, 0);

            SectionViewStyle objSectionViewStyle = Sect_Style.getSectionViewStyle("EW");

            if (objSectionViewStyle == null) {
                EW_Utility1.copyEWstyles();
                objSectionViewStyle = objSectionViewStyle = Sect_Style.getSectionViewStyle("EW");
            }

            SectionViewBandSetStyle objSectionViewBandSetStyle = Sect_Style.getSectionViewBandSetStyle("Standard");

            double dblOffX = 0;
            double dblOffY = 0;

            bool boolFirstPass = false;
            boolFirstPass = true;

            dblOffX = 0;
            dblOffY = 0;

            Point3d dblPntIns = Pub.pnt3dO;

            int j = 0;
            ObjectIdCollection idsSampleLine = idSLG.getSampleLineIDs();
            using (var tr = BaseObjs.startTransactionDb()) {
                for (int i = 0; i < idsSampleLine.Count; i += 5) {
                    SampleLine sl = (SampleLine)tr.GetObject(idsSampleLine[i], OpenMode.ForRead);

                    dblPntIns = new Point3d(dblPntBase.X + i / 5 * dblOffX, dblPntBase.Y, 0);
                    int k = -1;

                    while (j < idsSampleLine.Count) {
                        k = k + 1;

                        ObjectId idSectionView = SectionView.Create(string.Format("SV-{0}", j), sl.ObjectId, dblPntIns);
                        SectionView objSectionView = (SectionView)tr.GetObject(idSectionView, OpenMode.ForWrite);
                        if (boolFirstPass) {
                            dblOffX = System.Math.Abs(objSectionView.OffsetLeft) + objSectionView.OffsetRight + 30;
                            boolFirstPass = false;
                        }

                        Extents3d ext3d = (Extents3d)objSectionView.Bounds;

                        dblOffY = ext3d.MaxPoint.Y - ext3d.MinPoint.Y + 30;

                        dblPntIns = new Point3d(dblPntIns.X, dblPntIns.Y + dblOffY, 0);//increment Y after placing first section in column
                        objSectionView = null;

                        j = j + 1;

                        if (k == 4) {
                            break;
                        }
                    }
                }

                tr.Commit();
            }

            BaseObjs.acadActivate();

            return;
        }
    }
}