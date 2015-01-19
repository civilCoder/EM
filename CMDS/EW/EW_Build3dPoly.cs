using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;

namespace EW {

    public static class EW_Build3dPoly {

        public static string getBldgLayer(ObjectId idPolyBLDGLIM) {
            List<Point3d> pnts3dBldgLim = idPolyBLDGLIM.getCoordinates3dList();
            SelectionSet objSSetFLOORSLAB = EW_Utility1.buildSSetFLOOR_SLAB();
            ObjectId[] ids = objSSetFLOORSLAB.GetObjectIds();

            string strLayer = "";
            for (int i = 0; i < ids.Length; i++) {
                Polyline poly = (Polyline)ids[i].getEnt();
                Point3dCollection pnts3dPline = poly.getCoordinates3d();
                Point3d pnt3dControl = Geom.getCentroid(poly);

                if (Geom.isInside(pnt3dControl, pnts3dBldgLim, false)) {
                    strLayer = poly.Layer;
                    TypedValue[] tvs = new TypedValue[2] {
                        new TypedValue(1001, "FLOORSLAB"),
                        new TypedValue(1005, poly.Handle)
                    };
                                        
                    idPolyBLDGLIM.setXData(tvs, "FLOORSLAB");
                    break;
                }
            }

            return strLayer;            //as is will return last bldg area inside BLDG LIM if more than one FLOOR SLAB
        }

        public static ObjectId
        build3dPoly(ObjectId id2dPline, Point3d dPntCen, Point3d dPntTAR, double dblSlope,
            string strSurfaceName, string strLayerName, string strFunctionName, double dblOffOX = 0, int varInterval = 0) {
            Point3dCollection pnts3d = id2dPline.getCoordinates3d();

            for (int i = 0; i < pnts3d.Count; i++) {
                double elev = dPntTAR.Z + Geom.getCosineComponent(dPntCen, dPntTAR, pnts3d[i]) * (dblSlope * -1);
                Point3d pnt3d = new Point3d(pnts3d[i].X, pnts3d[i].Y, elev - dblOffOX);
                pnts3d[i] = pnt3d;
            }

            ObjectId id3dPoly = Draw.addPoly3d(pnts3d);

            Layer.manageLayers(strLayerName);

            id3dPoly.changeProp(null, strLayerName);

            if ((varInterval != 0)) {
                addVertexTo3dPoly(id3dPoly, varInterval);
            }

            if (strFunctionName == "OX") {
                TypedValue[] tvs = new TypedValue[2] {
                    new TypedValue(1001, "OX"),
                    new TypedValue(1000, strFunctionName)
                };
                id3dPoly.setXData(tvs, "OX");                          
            }

            return id3dPoly;
        }

        public static void
        addVertexTo3dPoly(ObjectId idPoly3d, double lenSeg) {
            List<Point3d> pnts3d = idPoly3d.getCoordinates3dList();
            List<Point3d> pnts3dNew = new List<Point3d>();
            pnts3dNew.Add(pnts3d[0]);

            using (Transaction tr = BaseObjs.startTransactionDb()) {
                Polyline3d poly3d = (Polyline3d)tr.GetObject(idPoly3d, OpenMode.ForWrite);

                for (int i = 0; i < pnts3d.Count - 1; i++) {
                    Point3d pnt3dBeg = pnts3d[i];
                    Point3d pnt3dEnd = pnts3d[i + 1];

                    double dblLEN = pnt3dBeg.getDistance(pnt3dEnd);
                    double dblSlope = pnt3dBeg.getSlope(pnt3dEnd);
                    double dblAng = pnt3dBeg.getDirection(pnt3dEnd);

                    //debug.Print dblDX & " " & dblDY & " " & dblDZ & " " & dblLEN & " " & dblSlope

                    int intInterval = (int)System.Math.Truncate(dblLEN / lenSeg);

                    for (int j = 1; j < intInterval; j++) {
                        Point3d pnt3d = pnts3d[i].traverse(dblAng, dblLEN / intInterval * j, dblSlope);
                        pnts3dNew.Add(pnt3d);
                    }
                    pnts3dNew.Add(pnt3dEnd);
                }
                tr.Commit();
            }

            idPoly3d.updatePoly3dCoordinates(pnts3dNew);
        }

        public static ObjectId
        buildArea3dLimit(ObjectId idPoly, string nameSurface, string nameSurfaceX, string nameLayer = "") {
            //3d Boundary
            bool exists;

            idPoly.checkIfClosed();

            List<Point3d> pnts3dPoly = idPoly.getCoordinates3dList();

            string strLayName = idPoly.getLayer();

            List<Point3d> pnts3dBNDY = new List<Point3d>();

            Color color = new Color();
            color = Color.FromColorIndex(ColorMethod.ByBlock, 5);

            //each polyline segment
            TinSurface tinSurf = Surf.getTinSurface(nameSurface, out exists);
            for (int j = 1; j < pnts3dPoly.Count; j++) {
                Point3d pnt3dBEG = pnts3dPoly[j -1];
                Point3d pnt3dEND = pnts3dPoly[j];

                ObjectId idLine = Draw.addLine(pnt3dBEG, pnt3dEND);

                Point3dCollection pnts3d = tinSurf.SampleElevations(idLine);

                if (pnts3d.Count > 0) {
                    for (int k = 0; k < pnts3d.Count; k++) {
                        if (pnts3d[k].Z > 0) {
                            pnts3dBNDY.Add(pnts3d[k]);
                        }else {
                            string mess = string.Format("Area Limit outside surface at X={0}, Y={1}\n EXITING PROGRAM", pnts3d[k].X, pnts3d[k].Z);
                            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(mess);
                        }
                    }

                    idLine.delete();
                }else {
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("intUBnd not > 0 at: " + j + " -See Blue lines where boundary is outside surface limit");
                    idLine.changeProp(color, "DEBUG-0", LineWeight.LineWeight211);
                }
            }

            ObjectId id3dPline = pnts3dBNDY.addPoly3d();
            id3dPline.changeProp(color, nameSurfaceX + "-BRKLINE-AREA");

            TypedValue[] tvs = new TypedValue[3] {
                new TypedValue(1001, "makeBOT"),
                new TypedValue(1000, nameLayer),
                new TypedValue(1000, "LIM")
            };

            id3dPline.setXData(tvs, "makeBOT");

            return id3dPline;
        }
    }
}