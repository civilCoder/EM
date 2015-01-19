using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using gc = Grading.Grading_CalcBasePnt;
using gp = Grading.Grading_Public;

namespace Grading.Cmds
{
    public class cmdRTx
    {
        public static void
        SetPointBySlopeFromRef3dPoly()
        {
            ObjectId idPoly = ObjectId.Null;
            Point3d pnt3dPick = Pub.pnt3dO;
            Dictionary<double, ObjectId> basePnts = new Dictionary<double, ObjectId>();

            double station = 0.0;

            try
            {
                gp.poly3dRF = (Polyline3d)Select.selectEntity(typeof(Polyline3d), "\nSelect Breakline (3dPolyline): ", "Breakline selection failed.  Exiting...", out pnt3dPick);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdRTx.cs: line: 28");
            }

            gp.vertex = pnt3dPick.getVertexNo(gp.poly3dRF.ObjectId);

            string ResultRTr = Grading_Palette.gPalette.pGrading.cmdRTr_Default;
            if (ResultRTr == string.Empty)
            {
                ResultRTr = "R";
            }

            double deltaZ = double.Parse(Dict.getCmdDefault("cmdRTr", "DeltaZ").ToString());
            double grade = Pub.Slope;

            object mode = SnapMode.getOSnap();
            SnapMode.setOSnap(513); //endpoint + nearest

            try
            {
                do
                {
                    List<ObjectId> idPnts = new List<ObjectId>();

                    bool escape = false;

                    gp.pnt3dT = Grading_GetPointTracking.getPoint("Select Target Location: ", "cmdRTR");
                    
                    
                    if (!gp.inBounds)
                        continue;

                    if (gp.pnt3dT != Pub.pnt3dO)
                    {
                        string prompt = string.Format("\nZ value difference / Rate of grade /Both <{0}>:[Z/R/B]", ResultRTr);
                        escape = UserInput.getUserInputKeyword(ResultRTr.ToUpper(), out ResultRTr, prompt, "Z R B");
                        if (escape)
                            break;

                        double elevTAR = 0;
                        switch (ResultRTr.ToUpper())
                        {
                            case "R":
                                escape = UserInput.getUserInput("\nRate of Grade: ", out grade, grade);
                                if (escape)
                                    break;
                                Pub.Slope = grade;
                                gp.pnt3dX = gc.calcBasePntAndTargetPntElev(gp.pnt3dT, grade, gp.poly3dRF, gp.vertex, out elevTAR, out station);
                                if (gp.pnt3dX == Pub.pnt3dO && !gp.shift)
                                {
                                    Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog("Point Selected is out of bounds of selected segment. Exiting...");
                                    continue;
                                }

                                gp.pnt3dT = new Point3d(gp.pnt3dT.X, gp.pnt3dT.Y, elevTAR);
                                break;

                            case "Z":
                                deltaZ = Pub.dZ;
                                escape = UserInput.getUserInput("Z Value Difference", out deltaZ, deltaZ);
                                if (escape)
                                    break;
                                Pub.dZ = deltaZ;
                                gp.pnt3dX = gc.calcBasePntAndTargetPntElev(gp.pnt3dT, 0.0, gp.poly3dRF, gp.vertex, out elevTAR, out station);
                                if (gp.pnt3dX == Pub.pnt3dO && ! gp.shift)
                                {
                                    Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog("Point Selected is out of bounds of selected segment.");
                                    break;
                                }

                                gp.pnt3dT = new Point3d(gp.pnt3dT.X, gp.pnt3dT.Y, elevTAR + deltaZ);
                                break;

                            case "B":
                                escape = UserInput.getUserInput("\nRate of Grade: ", out grade, grade);
                                if (escape)
                                    break;
                                Pub.Slope = grade;
                                escape = UserInput.getUserInput("Z Value Difference", out deltaZ, deltaZ);
                                if (escape)
                                    break;

                                gp.pnt3dX = gc.calcBasePntAndTargetPntElev(gp.pnt3dT, grade, gp.poly3dRF, gp.vertex, out elevTAR, out station);
                                if (gp.pnt3dX == Pub.pnt3dO && !gp.shift)
                                {
                                    Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog("Point Selected is out of bounds of selected segment.");
                                    break;
                                }

                                gp.pnt3dT = new Point3d(gp.pnt3dT.X, gp.pnt3dT.Y, elevTAR + deltaZ);
                                break;
                        }

                        if (!escape)
                        {
                            uint pntNum;
                            ObjectId idPntBase = ObjectId.Null;
                            if (basePnts.TryGetValue(station, out idPntBase))
                            {
                                idPnts.Add(idPntBase);
                            }
                            else
                            {
                                idPntBase = gp.pnt3dX.setPoint(out pntNum, "CPNT-ON");
                                basePnts.Add(station, idPntBase);
                                idPnts.Add(idPntBase);
                            }

                            idPnts.Add(gp.pnt3dT.setPoint(out pntNum, "CPNT-ON"));

                            if (ResultRTr != "B")
                            {
                                List<Handle> hPnts = new List<Handle>();
                                hPnts.Add(idPnts[0].getHandle());
                                hPnts.Add(idPnts[1].getHandle());
                                BrkLine.makeBreakline(apps.lnkBrks, "cmdRTR", out idPoly, idPnts);
                            }
                        }
                    }
                    else
                        break;
                }
                while (1 < 2);

                Grading_Palette.gPalette.pGrading.cmdRTr_Default = ResultRTr;

                Dict.setCmdDefault("cmdRTr", "cmdDefault", ResultRTr);
                Dict.setCmdDefault("cmdRTr", "Slope", grade.ToString());
                Dict.setCmdDefault("cmdRTr", "DeltaZ", deltaZ.ToString());
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdRTx.cs: line: 157");
            }
            finally
            {
                SnapMode.setOSnap((int)mode);
            }
        }
    }
}
