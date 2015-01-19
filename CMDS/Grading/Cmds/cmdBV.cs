using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Base_Tools45;
using System;
using System.Collections.Generic;

namespace Grading.Cmds
{
    public static class cmdBV
    {
        public static readonly double PI = System.Math.PI;
        private static Grading_Palette gPalette = Grading_Palette.gPalette;
        private static myForms.GradeSite fGrading = gPalette.pGrading;

        public static void
        BV()
        {
            int snapMode = SnapMode.getOSnap();
            try
            {
                ObjectId idPoly3dFL = ObjectId.Null;
                ObjectId idPoly3dOP = ObjectId.Null;
                ObjectId idPoly3dLT = ObjectId.Null;
                ObjectId idPoly3dRT = ObjectId.Null;

                List<Point3d> pnts3dFL = null;
                List<Point3d> pnts3dRF = null;
                List<Point3d> pnts3dOP = null;

                List<Handle> handles3 = null;

                ObjectIdCollection ids = null;
                Point3d pnt3dBEG = Pub.pnt3dO;

                double width = double.Parse(fGrading.cmdBV_GutterWidth);
                UserInput.getUserInput("\nGutter Width in Feet: ", out width, width);

                if (width == -99.99)
                    return;

                fGrading.cmdBV_GutterWidth = width.ToString();
                Dict.setCmdDefault("cmdBV", "GUTTERWIDTH", width.ToString());

                double deltaZ = double.Parse(fGrading.cmdBV_GutterDepth);
                UserInput.getUserInput("\nGutter Depth in Feet: ", out deltaZ, deltaZ);

                if (deltaZ == -99.99)
                    return;

                fGrading.cmdBV_GutterDepth = deltaZ.ToString();
                Dict.setCmdDefault("cmdBV", "GUTTERDEPTH", deltaZ.ToString());

                ObjectId idPoly3dRF = ObjectId.Null;
                ObjectId idPolyRF = ObjectId.Null;
                ObjectId idPolyFL = ObjectId.Null;

                switch (fGrading.cmdBV_Source)      //SOURCE
                {
                    case "BRKLINE":
                        Type type = typeof(Polyline3d);
                        TypedValue[] tvs = new TypedValue[2];
                        tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(type).DxfName), 0);
                        tvs.SetValue(new TypedValue((int)DxfCode.LayerName, "CPNT_BRKLINE"), 1);

                        SelectionSet ss = Select.buildSSet(tvs);
                        ss.moveToTop();

                        idPoly3dRF = Select.getBrkLine("Select 3d Breakline: ");
                        if (idPoly3dRF == ObjectId.Null)
                            return;

                        if(fGrading.optFL.IsChecked == true)
                            idPolyFL = idPoly3dRF.toPolyline("FL");

                        if(fGrading.optEDGE.IsChecked == true)
                            idPolyRF = idPoly3dRF.toPolyline("GUTTER");
                        
                        break;

                    case "POINTS":

                        if (fGrading.optFL.IsChecked == true)
                        {
                            try
                            {
                                idPoly3dFL = BrkLine.makeBreakline(apps.lnkBrks2, "cmdBV", out idPolyFL);        //make breakline and store endPoints on breakline, store breakline on endpoints

                                if (idPoly3dFL == ObjectId.Null)
                                    return;
                                pnts3dFL = idPoly3dFL.getCoordinates3dList();
                                idPoly3dRF = idPoly3dFL;
                                pnts3dRF = pnts3dFL;
                                pnt3dBEG = idPoly3dFL.getBegPnt();
                            }
                            catch (System.Exception ex)
                            {
                                BaseObjs.writeDebug(ex.Message + " cmdBV.cs: line: 99");
                            }
                        }
                        else if (fGrading.optEDGE.IsChecked == true)
                        {
                            try
                            {
                                idPoly3dRF = BrkLine.makeBreakline(apps.lnkBrks2, "cmdRF", out idPolyRF);
                                if (idPoly3dRF == ObjectId.Null)
                                    return;
                                pnts3dRF = idPoly3dRF.getCoordinates3dList();
                                pnt3dBEG = idPoly3dRF.getBegPnt();
                            }
                            catch (System.Exception ex)
                            {
                                BaseObjs.writeDebug(ex.Message + " cmdBV.cs: line: 114");
                            }
                        }
                        break;
                }

                int i = 0;

                List<SEG_PROP> segProps = pnts3dRF.getPoly3dSegProps();

                double offFL = width / 2;
                double xSlopeFL = deltaZ / offFL;
                double offOP = width;   //opposite side
                double xSlopeOP = 0.00;
                string nameLayer = "CPNT-BRKLINE";

                PromptStatus ps;

                switch (fGrading.cmdBV_Control)     //CONTROL
                {
                    case "EDGE":
                        pnts3dOP = new List<Point3d>();
                        bool escape = false;
                        int side = -1;
                        Point3d pnt3dPick = UserInput.getPoint("Pick point on FL side of Gutter Edge.", pnt3dBEG, out escape, out ps, osMode: 0);
                        if (escape || pnt3dPick == Pub.pnt3dO)
                        {
                            SnapMode.setOSnap(snapMode);
                            return;
                        }
                        for (i = 1; i < pnts3dRF.Count; i++)
                        {
                            double length = pnts3dRF[i - 1].getDistance(pnts3dRF[i + 0]);
                            double distance = pnt3dPick.getOrthoDist(pnts3dRF[i - 1], pnts3dRF[i + 0]);

                            if (distance > 0 && distance < length)
                            {
                                if (pnt3dPick.isRightSide(pnts3dRF[i - 1], pnts3dRF[i + 0]))
                                {
                                    side = 1; //otherwise default is -1
                                    break;
                                }
                            }
                        }

                        if (i == pnts3dRF.Count - 1)
                        {
                            Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog("You have selected a point in the blind spot.");
                            SnapMode.setOSnap(snapMode);
                            return;
                        }

                        for (i = 0; i < segProps.Count; i++)
                        {
                            switch (side)
                            {
                                case 1:

                                    if (i == 0)
                                    {
                                        pnts3dFL.Add(Geom.traverse_pnt3d(segProps[i].BEG, segProps[i].DIR_AHEAD + PI / 2 * side, -1 * offFL, xSlopeFL));
                                        pnts3dOP.Add(Geom.traverse_pnt3d(segProps[i].BEG, segProps[i].DIR_AHEAD + PI / 2 * side, -1 * offOP, xSlopeOP));
                                        continue;
                                    }

                                    if (segProps[i].DELTA == 0)
                                    {
                                        pnts3dFL.Add(Geom.traverse_pnt3d(segProps[i].BEG, segProps[i].DIR_AHEAD + PI / 2 * side, -1 * offFL, xSlopeFL));
                                        pnts3dOP.Add(Geom.traverse_pnt3d(segProps[i].BEG, segProps[i].DIR_AHEAD + PI / 2 * side, -1 * offOP, xSlopeOP));
                                    }
                                    else if (segProps[i].DELTA > 0)
                                    {
                                        pnts3dFL.Add(Geom.doAnglePointOUT(segProps[i].BEG, xSlopeFL, segProps[i].DELTA, segProps[i - 1].DIR_AHEAD, -1, offFL));
                                        pnts3dOP.Add(Geom.doAnglePointOUT(segProps[i].BEG, xSlopeOP, segProps[i].DELTA, segProps[i - 1].DIR_AHEAD, -1, offOP));
                                    }
                                    else if (segProps[i].DELTA < 0)
                                    {
                                        pnts3dFL.Add(Geom.doAnglePointIN(segProps[i].BEG, xSlopeFL, System.Math.Abs(segProps[i].DELTA), segProps[i].DIR_AHEAD, -1, offFL));
                                        pnts3dOP.Add(Geom.doAnglePointIN(segProps[i].BEG, xSlopeOP, System.Math.Abs(segProps[i].DELTA), segProps[i].DIR_AHEAD, -1, offOP));
                                    }

                                    if (i == segProps.Count - 1)
                                    {
                                        pnts3dFL.Add(Geom.traverse_pnt3d(segProps[i].END, segProps[i].DIR_AHEAD + PI / 2 * side, -1 * offFL, xSlopeFL));
                                        pnts3dOP.Add(Geom.traverse_pnt3d(segProps[i].END, segProps[i].DIR_AHEAD + PI / 2 * side, -1 * offOP, xSlopeOP));
                                    }

                                    break;

                                case -1:

                                    if (i == 0)
                                    {
                                        pnts3dFL.Add(Geom.traverse_pnt3d(segProps[i].BEG, segProps[i].DIR_AHEAD + PI / 2 * side, -1 * offFL, xSlopeFL));
                                        pnts3dOP.Add(Geom.traverse_pnt3d(segProps[i].BEG, segProps[i].DIR_AHEAD + PI / 2 * side, -1 * offOP, xSlopeOP));
                                        continue;
                                    }

                                    if (segProps[i].DELTA == 0)
                                    {
                                        pnts3dFL.Add(Geom.traverse_pnt3d(segProps[i].BEG, segProps[i].DIR_AHEAD + PI / 2 * side, -1 * offFL, xSlopeFL));
                                        pnts3dOP.Add(Geom.traverse_pnt3d(segProps[i].BEG, segProps[i].DIR_AHEAD + PI / 2 * side, -1 * offOP, xSlopeOP));
                                    }
                                    else if (segProps[i].DELTA > 0)
                                    {
                                        pnts3dFL.Add(Geom.doAnglePointIN(segProps[i].BEG, xSlopeFL, segProps[i].DELTA, segProps[i].DIR_AHEAD, 1, offFL));
                                        pnts3dOP.Add(Geom.doAnglePointIN(segProps[i].BEG, xSlopeOP, segProps[i].DELTA, segProps[i].DIR_AHEAD, 1, offOP));
                                    }
                                    else if (segProps[i].DELTA < 0)
                                    {
                                        pnts3dFL.Add(Geom.doAnglePointOUT(segProps[i].BEG, xSlopeFL, System.Math.Abs(segProps[i].DELTA), segProps[i - 1].DIR_AHEAD, 1, offFL));
                                        pnts3dOP.Add(Geom.doAnglePointOUT(segProps[i].BEG, xSlopeOP, System.Math.Abs(segProps[i].DELTA), segProps[i - 1].DIR_AHEAD, 1, offOP));
                                    }

                                    if (i == segProps.Count - 1)
                                    {
                                        pnts3dFL.Add(Geom.traverse_pnt3d(segProps[i].END, segProps[i].DIR_AHEAD + PI / 2 * side, -1 * offFL, xSlopeFL));
                                        pnts3dOP.Add(Geom.traverse_pnt3d(segProps[i].END, segProps[i].DIR_AHEAD + PI / 2 * side, -1 * offOP, xSlopeOP));
                                    }

                                    break;
                            }
                        }

                        idPoly3dFL = Draw.addPoly3d(pnts3dFL, nameLayer);
                        Grading_Dict.addBrksToPntXDict(idPoly3dRF, idPoly3dFL, offFL, deltaZ, 0.0, -1.0); //ADD EDGE HANDLE AND OFFSETS TO POINT EXTENSION DICTIONARY, ADD POINT HANDLES TO EDGE
                        idPolyFL = idPoly3dFL.toPolyline("FL");

                        idPoly3dOP = Draw.addPoly3d(pnts3dOP, nameLayer);
                        Grading_Dict.addBrksToPntXDict(idPoly3dRF, idPoly3dOP, offOP, 0.0, 0.0, -1.0);
                        ObjectId idPolyOP = idPoly3dOP.toPolyline("GUTTER");

                        handles3 = new List<Handle>();
                        handles3.Add(idPoly3dRF.getHandle());
                        handles3.Add(idPolyRF.getHandle());
                        handles3.Add(idPolyFL.getHandle());
                        handles3.Add(idPoly3dOP.getHandle());
                        handles3.Add(idPolyOP.getHandle());

                        ids = new ObjectIdCollection();
                        ids.Add(idPoly3dRF);
                        ids.Add(idPoly3dOP);

                        break;

                    case "FL":

                        Point3dCollection pnts3dLT = new Point3dCollection();
                        Point3dCollection pnts3dRT = new Point3dCollection();

                        int x = segProps.Count;

                        for (i = 0; i < x; i++)
                        {
                            if (i == 0)
                            {
                                pnts3dLT.Add(Geom.traverse_pnt3d(segProps[i].BEG, segProps[i].DIR_AHEAD + PI / 2, 1 * offFL, xSlopeFL));
                                pnts3dRT.Add(Geom.traverse_pnt3d(segProps[i].BEG, segProps[i].DIR_AHEAD - PI / 2, 1 * offFL, xSlopeFL));
                            }
                            if (i > 0 && i < x)
                            {
                                if (segProps[i].DELTA == 0)
                                {
                                    pnts3dLT.Add(Geom.traverse_pnt3d(segProps[i].BEG, segProps[i].DIR_AHEAD + PI / 2, 1 * offFL, xSlopeFL));
                                    pnts3dRT.Add(Geom.traverse_pnt3d(segProps[i].BEG, segProps[i].DIR_AHEAD - PI / 2, 1 * offFL, xSlopeFL));
                                }
                                else if (segProps[i].DELTA > 0)
                                {
                                    pnts3dLT.Add(Geom.doAnglePointIN(segProps[i].BEG, xSlopeFL, segProps[i - 1].DELTA, segProps[i - 1].DIR_AHEAD, -1, offFL));
                                    pnts3dRT.Add(Geom.doAnglePointOUT(segProps[i].BEG, xSlopeFL, segProps[i - 1].DELTA, segProps[i - 0].DIR_AHEAD, 1, offFL));
                                }
                                else if (segProps[i].DELTA < 0)
                                {
                                    pnts3dLT.Add(Geom.doAnglePointOUT(segProps[i].BEG, xSlopeFL, System.Math.Abs(segProps[i].DELTA), segProps[i - 0].DIR_AHEAD, -1, offFL));
                                    pnts3dRT.Add(Geom.doAnglePointIN(segProps[i].BEG, xSlopeFL, System.Math.Abs(segProps[i].DELTA), segProps[i - 1].DIR_AHEAD, 1, offOP));
                                }
                            }
                            if (i == x - 1)
                            {
                                pnts3dLT.Add(Geom.traverse_pnt3d(segProps[i].END, segProps[i].DIR_AHEAD + PI / 2, 1 * offFL, xSlopeFL));
                                pnts3dRT.Add(Geom.traverse_pnt3d(segProps[i].END, segProps[i].DIR_AHEAD - PI / 2, 1 * offFL, xSlopeFL));
                            }
                        }

                        ObjectId idPolyLT = Draw.addPoly(pnts3dLT, "2dPoly");
                        idPoly3dLT = Draw.addPoly3d(pnts3dLT, nameLayer);
                        Grading_Dict.addBrksToPntXDict(idPoly3dFL, idPoly3dLT, -offFL, deltaZ, 0.0, -1.0); //ADD EDGE HANDLE AND OFFSETS TO POINT EXTENSION DICTIONARY, ADD POINT HANDLES TO EDGE

                        ObjectId idPolyRT = Draw.addPoly(pnts3dRT, "2dPoly");
                        idPoly3dRT = Draw.addPoly3d(pnts3dRT, nameLayer);
                        Grading_Dict.addBrksToPntXDict(idPoly3dFL, idPoly3dRT, offFL, deltaZ, 0.0, -1.0);

                        handles3 = new List<Handle>();
                        handles3.Add(idPoly3dLT.getHandle());
                        handles3.Add(idPolyLT.getHandle());
                        handles3.Add(idPoly3dRT.getHandle());
                        handles3.Add(idPolyRT.getHandle());

                        ids = new ObjectIdCollection();
                        ids.Add(idPoly3dLT);
                        ids.Add(idPoly3dRT);
                        break;
                }

                Grading_GetNestedCurbObjects.getCurbFromXref(ids, idPoly3dFL, handles3);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdBV.cs: line: 322");
            }
            finally
            {
            }
        }
    }
}
