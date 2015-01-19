using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Windows.Forms;
using Section = Autodesk.Civil.DatabaseServices.Section;

namespace Wall {
    public static class Wall_Sections {

        static Wall_Form.frmWall1 fWall1 = Wall_Forms.wForms.fWall1;

        public static void
        drawSections() {
            Alignment objAlign = default(Alignment);

            try {
                objAlign = fWall1.ACTIVEALIGN;

                if ((objAlign == null)) {
                    MessageBox.Show("Select Alignment");
                    return;
                }
            }
            catch (System.Exception ) {
                MessageBox.Show("Select Alignment");
                return;
            }
            bool exists = false;

            TinSurface objSurfaceEXIST = Surf.getTinSurface("EXIST", out exists);
            TinSurface objSurfaceCPNT = Surf.getTinSurface("CPNT-ON", out exists);

            //ObjectId objSectionStyleEXISTId = Sect.getSectionStyleId("EXIST");
            //ObjectId objSectionStyleCPNTId = Sect.getSectionStyleId("CPNT");
            //GroupPlotStyle objGroupPlotStyle = Sect.getGroupPlotStyle("WALL");
            //SampleLineStyle objSampleLineStyle = Sect.getSampleLineStyle("Standard");
            //SectionLabelSetStyle objSampleLabelSetStyle = Sect.getSampleLineLabelStyle("Standard");

            double dblLen = objAlign.Length;
            Sect.removeSampleLineGroups(objAlign.ObjectId);

            string strLayer = string.Format("{0}-SEC", objAlign.Name);
            Layer.manageLayers(strLayer);

            ObjectId idSLG = Sect.addSampleLineGroupAndSampleLines(objAlign.ObjectId, "SLG-1");
            idSLG.removeSampledSurfaces();

            ObjectIdCollection idSurfaces = new ObjectIdCollection();
            idSurfaces.Add(objSurfaceCPNT.ObjectId);
            idSurfaces.Add(objSurfaceEXIST.ObjectId);

            idSLG.addSurfaceToSample(idSurfaces);

            Sect.addSections(objAlign.ObjectId, idSLG, 20.0, 20.0);

            SampleLine objSampleLine = default(SampleLine);
            Autodesk.Civil.DatabaseServices.Section objSection = default(Autodesk.Civil.DatabaseServices.Section);

            ObjectIdCollection idsSampleLine = idSLG.getSampleLineIDs();
            using (var tr = BaseObjs.startTransactionDb()) {
                foreach (ObjectId idSampleLine in idsSampleLine) {
                    objSampleLine = (SampleLine)tr.GetObject(idSampleLine, OpenMode.ForRead);
                    foreach (ObjectId idSection in objSampleLine.GetSectionIds()) {
                        objSection = (Section)tr.GetObject(idSection, OpenMode.ForWrite);
                        objSection.Layer = strLayer;
                    }
                }
                tr.Commit();
            }

            Autodesk.Civil.DatabaseServices.Styles.SectionViewStyle objSectionViewStyle = default(Autodesk.Civil.DatabaseServices.Styles.SectionViewStyle);

            try {
                objSectionViewStyle = Sect_Style.getSectionViewStyle("WALL");
            }
            catch (System.Exception ) {
                objSectionViewStyle = Sect_Style.getSectionViewStyle("Standard");
            }

            SectionViewBandSetStyle objSectionViewBandStyleSet = Sect_Style.getSectionViewBandSetStyle("Standard");

            double dblOffX = 0;
            double dblOffY = 0;

            bool boolFirstPass = true;

            //Dim objStationRange As New StationRange
            //With objStationRange
            //    .UseSampleIncrements = True
            //    .SampleAtHighLowPoints = False
            //    .SampleAtHorizontalGeometryPoints = False
            //    .SampleAtSuperelevationCriticalStations = False
            //    .SampleAtRangeEnd = True
            //    .SampleAtRangeStart = True
            //    .StartRangeAtAlignmentStart = True
            //    .EndRangeAtAlignmentEnd = True
            //    .StartRange = 0.0
            //    .EndRange = dblLen - 1
            //    .IncrementTangent = 50.0
            //    .SwathWidthLeft = 20.0
            //    .SwathWidthRight = 20.0
            //    .SampleLineDefaultDirection = DirectionFromType.DirectionFromBaseAlignment
            //End With

            //objStationRange.SampleLineStyle = objSampleLineStyle

            //objSLG.SampleLines.AddByStationRange("SL-1", SampleLineDuplicateActionType.SampleLineDuplicateActionOverwrite, objStationRange)
            
            PromptStatus ps = default(PromptStatus);
            Point3d pnt3dBase = default(Point3d);
            try {
                BaseObjs.acadActivate();
                pnt3dBase = UserInput.getPoint("Select Insertion Point for Sections", out ps, osMode: 0);
            }
            catch (System.Exception ) {
                return;
            }

            SectionView objSectionView = null;
            Point3d pnt3dIns = default(Point3d);

            using (var tr1 = BaseObjs.startTransactionDb()) {
                int j = 0;
                for (int i = 0; i < idsSampleLine.Count; i += 5) {
                    objSampleLine = (SampleLine)tr1.GetObject(idsSampleLine[i], OpenMode.ForRead);

                    //dblPntIns(0) = pnt3dBase.X + i / 5 * dblOffX

                    int k = -1;

                    while (j < idsSampleLine.Count) {
                        k ++;

                        pnt3dIns = new Point3d(pnt3dBase.X + i / 5 * dblOffX, pnt3dBase.Y + k * dblOffY, 0.0);

                        try {
                            ObjectId idSectionView = SectionView.Create("SV" + Convert.ToString(j), idsSampleLine[i], pnt3dIns);
                            objSectionView = (SectionView)tr1.GetObject(idSectionView, OpenMode.ForWrite);
                            objSectionView.OffsetLeft = 20.0;
                            objSectionView.OffsetRight = 20.0;
                            objSectionView.StyleId = objSectionViewStyle.ObjectId;
                        }
                        catch (Autodesk.AutoCAD.Runtime.Exception ) {
                        }

                        if (boolFirstPass) {
                            dblOffX = System.Math.Abs(objSectionView.OffsetLeft) + objSectionView.OffsetRight + 30;
                            dblOffY = (objSectionView.ElevationMax - objSectionView.ElevationMin) + 20;
                            boolFirstPass = false;
                        }

                        j++;

                        if (k == 4) {
                            break;
                        }
                    }
                }

                tr1.Commit();
            }
        }
    }
}