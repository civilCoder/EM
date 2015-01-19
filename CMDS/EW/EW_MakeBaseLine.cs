using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace EW {
    public static class EW_MakeBaseLine {
        static double pi = System.Math.PI;

        public static void
        makeBaseline() {
            Layer.manageLayers("GRADING LIMIT");
            Layer.manageLayers("SECTIONS");

            SelectionSet objSSet = EW_Utility1.buildSSetGradingLim();
            Entity ent = objSSet.GetObjectIds()[0].getEnt();
            ObjectId idLim = ent.ObjectId;

            double dblStationBase = 0;
            bool boolUseBase;
            Alignment objAlignBASE = null;
            Alignment objAlign = Align.getAlignment("BASE");

            if (objAlign != null) {
                DialogResult varResponse = MessageBox.Show("Use existing BASE alignment for sections?", "", MessageBoxButtons.YesNo);

                if (varResponse == DialogResult.Yes) {
                    objAlignBASE = objAlign;
                    dblStationBase = objAlignBASE.StartingStation;

                    objAlign = null;
                    boolUseBase = true;
                }else {
                    boolUseBase = false;
                    objAlign.Name = "BASE_old";
                }
            }else {
                boolUseBase = false;
            }
            List<Point3d> varPntInts = new List<Point3d>();
            ObjectId idLWPline = ObjectId.Null, idLine = ObjectId.Null;
            double dblAng = 0;
            bool exists;
            Point3d pnt3dCEN = Pub.pnt3dO, pnt3dTAR = Pub.pnt3dO, dPntBeg = Pub.pnt3dO, dPntEnd = Pub.pnt3dO;
            if (!boolUseBase) {
                ObjectId idDictGRADEDOCK = Dict.getNamedDictionary("GRADEDOCK", out exists);

                if (!exists) {
                    Application.ShowAlertDialog("GRADEDOCK Dictionary missing. Create alignment BASE and try again.");
                    return;
                }else {
                    ResultBuffer rb = Dict.getXRec(idDictGRADEDOCK, "CENTROID");
                    TypedValue[] tvs = rb.AsArray();

                    pnt3dCEN = new Point3d(double.Parse(tvs[0].Value.ToString()),
                        double.Parse(tvs[1].Value.ToString()),
                        double.Parse(tvs[2].Value.ToString()));

                    rb = Dict.getXRec(idDictGRADEDOCK, "TARGET");
                    tvs = rb.AsArray();

                    pnt3dTAR = new Point3d(double.Parse(tvs[0].Value.ToString()),
                        double.Parse(tvs[1].Value.ToString()),
                        double.Parse(tvs[2].Value.ToString()));
                }

                idLine = Draw.addLine(pnt3dCEN, pnt3dTAR);

                varPntInts = idLine.intersectWith(idLim, extend.source);
                idLine.delete();

                dPntBeg = varPntInts[0];
                dPntEnd = varPntInts[1];

                dblAng = dPntBeg.getDirection(dPntEnd);

                if (dblAng > pi / 2 & dblAng <= 3 * pi / 2) {
                    varPntInts.Reverse();
                    dPntBeg = varPntInts[0];
                    dPntEnd = varPntInts[1];

                    dblAng = Measure.getAzRadians(dPntBeg, dPntEnd);
                }

                idLWPline = Draw.addPoly(varPntInts);

                dblStationBase = 0;
            }

            Point3d pnt3dB = dPntBeg.traverse(dblAng, 10);
            Point3d pnt3dE = dPntEnd.traverse(dblAng, 10);
            Point3d pnt3dM = pnt3dB.getMidPoint3d(pnt3dE);

            if ((objAlignBASE == null)) {
                objAlignBASE = Align.addAlignmentFromPoly("BASE", "SECTIONS", idLWPline, "Standard", "Standard", true, true);
            }

            AlignmentEntityCollection ents = objAlignBASE.Entities;

            AlignmentEntity objAlignEnt = ents[0];
            if (objAlignEnt.EntityType == AlignmentEntityType.Line) {
                AlignmentLine objAlignEntTan = (AlignmentLine)objAlignEnt;
                Point2d pnt2dBeg = objAlignEntTan.StartPoint;
                Point2d pnt2dEnd = objAlignEntTan.EndPoint;

                dblAng = pnt2dBeg.getDirection(pnt2dEnd);
            }

            int j = -1;
            double easting = 0, northing = 0;
            double dblOffR = 0, dblOffL = 0;
            while (dblStationBase < objAlignBASE.Length) {
                j = j + 1;
                dblStationBase = dblStationBase + j * 50;
                objAlignBASE.PointLocation(dblStationBase, 0.0, ref easting, ref northing);
                Point3d pnt3dX = new Point3d(easting, northing, 0);
                Point3d varPntPolar = pnt3dX.traverse(dblAng + pi / 2, 100.0);
                idLine = Draw.addLine(pnt3dX, varPntPolar);
                varPntInts = idLine.intersectWith(idLim, extend.source);
                idLine.delete();

                StaOff sOff = new StaOff();
                sOff.Sta = dblStationBase;
                double dblStation = 0, dblOffset = 0;

                for (int k = 0; k < varPntInts.Count; k++) {
                    objAlignBASE.StationOffset(varPntInts[k].X, varPntInts[k].Y, ref dblStation, ref dblOffset);

                    if (dblOffset >= 0) {
                        sOff.R = dblOffset;
                    }else {
                        sOff.L = System.Math.Abs(dblOffset);
                    }

                    if (sOff.R > dblOffR) {
                        dblOffR = sOff.R;
                    }

                    if (sOff.L > dblOffL) {
                        dblOffL = sOff.L;
                    }

                    if (k == 3)
                        break; 
                }
            }

            double dblRem = dblOffL % 10;

            if (dblRem < 5) {
                dblOffL = System.Math.Round(dblOffL / 10, 0) * 10 + 10;
            }else {
                dblOffL = System.Math.Round(dblOffL / 10, 0) * 10;
            }

            dblRem = dblOffR % 10;

            if (dblRem < 5) {
                dblOffR = System.Math.Round(dblOffR / 10, 0) * 10 + 10;
            }else {
                dblOffR = System.Math.Round(dblOffR / 10, 0) * 10;
            }

            EW_CmdSections.cmdSections(objAlignBASE.ObjectId, dblOffR, dblOffL);
        }
        
    }
}