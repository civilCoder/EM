using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;

namespace Grading.Cmds
{
    public static class cmdBC1
    {    //extended version of BC implementing multiple segments
        public static void
        BC1(string nameCmd)
        {
            List<double> userInput = null;
            Polyline3d poly3dFL = null;
            ObjectId idPoly3dFL = ObjectId.Null;

            int side;

            Point3d pnt3dBEG = Pub.pnt3dO;
            Point3d pnt3dEND = Pub.pnt3dO;
            Point3d pnt3dX = Pub.pnt3dO;

            List<ObjectId> idsCgPnt = new List<ObjectId>();
            ObjectId idPoly = ObjectId.Null;
            bool escape = false;
            try
            {
                userInput = cmdBC.getUserInput(nameCmd);
                if (userInput == null)
                    return;

                BaseObjs.acadActivate();

                if (Grading_Palette.gPalette.pGrading.optBRKLINE.IsChecked == true)
                {
                    Point3d pnt3dPick;
                    Entity ent = Select.selectEntity(typeof(Polyline3d), "Select Brkline:", "Brkline selection failed.", out pnt3dPick);
                    poly3dFL = (Polyline3d)ent;                                     //this 3dPoly is from points and is linked to the points with "lnkBrks" !!!!!!!!!!!!!!!!!!!!!!!!!!
                    idPoly3dFL = poly3dFL.ObjectId;
                    ResultBuffer rb = idPoly3dFL.getXData(null);
                    if (rb == null)
                        return;
                    TypedValue[] tvsAll = rb.AsArray();
                    List<string> nameApps;
                    List<TypedValue[]> tvsLst = tvsAll.parseXData(out nameApps);

                    if (nameApps.Contains(apps.lnkBrks) || nameApps.Contains(apps.lnkBrks2))
                    {
                        foreach (TypedValue[] tvs in tvsLst)
                        {
                            if (tvs[0].Value.ToString() == apps.lnkBrks)
                            {
                                idPoly3dFL.clearXData(apps.lnkBrks);
                                tvs.SetValue(new TypedValue(1001, apps.lnkBrks2), 0);     //link end points to FL upgraded from brkline
                                idPoly3dFL.setXData(tvs, apps.lnkBrks2);
                            }
                            else if (tvs[0].Value.ToString() == apps.lnkBrks2)
                            {
                                idPoly3dFL.clearXData(apps.lnkBrks3);                     //clear old link data if exists - endpoints should be same
                            }
                        }
                    }
                }
                else if (Grading_Palette.gPalette.pGrading.optPNTS.IsChecked == true)
                {
                    try
                    {
                        string prompt1 = "\nSelect first point (Esc to quit): ";
                        string prompt2 = "\nSelect next point (Enter to exit/Esc to quit: ";

                        idsCgPnt = getPoints(prompt1, prompt2);

                        idPoly3dFL = BrkLine.makeBreakline(apps.lnkBrks2, "BC", out idPoly, idsCgPnt);         //this 3dPoly is from points and is linked to the points with "lnkBrks2"    OK.
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(ex.Message + " cmdBC1.cs: line: 79");
                    }
                }

                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        poly3dFL = (Polyline3d)tr.GetObject(idPoly3dFL, OpenMode.ForRead);

                        pnt3dBEG = idsCgPnt[0].getCogoPntCoordinates();
                        pnt3dEND = idsCgPnt[1].getCogoPntCoordinates();

                        PromptStatus ps;
                        string prompt = "\nPick point Back of Curb adjacent to first segment";
                        pnt3dX = UserInput.getPoint(prompt, pnt3dBEG, out escape, out ps, osMode: 0);
                        if (pnt3dX == Pub.pnt3dO || escape)
                        {
                            return;
                        }
                        if (pnt3dX.isRightSide(pnt3dBEG, pnt3dEND))
                            side = 1;
                        else
                            side = -1;

                        double offH1 = 0.5 * side;
                        double offV1 = userInput[0] + 0.0208;
                        double offH2 = userInput[1] * side;
                        double offV2 = userInput[2] * userInput[1];

                        List<Handle> handles = new List<Handle>();


                        ObjectId idPoly3dTC = buildCurbFeature(offH1, offV1, idPoly3dFL, idPoly3dFL, false, ref handles, side);        // collect handles from TC, BB, CgPnt, GT
                        Grading_Dict.addBrksToPntXDict(idPoly3dFL, idPoly3dTC, offH1, offV1, 0.0, -1.0);

                        ObjectId idPoly3dBB = buildCurbFeature(offH2, offV2, idPoly3dFL, idPoly3dTC, true, ref handles, side);
                        Grading_Dict.addBrksToPntXDict(idPoly3dFL, idPoly3dBB, offH1 + offH2, offV1 + offV2, 0.0, -1.0);

                        ObjectId idPolyBB = idPoly3dBB.addPoly("GB");
                        handles.Add(idPolyBB.getHandle());

                        if (nameCmd == "cmdBG")
                        {
                            double offH3 = userInput[3] * side * -1;
                            double offV3 = userInput[4];

                            ObjectId idPoly3dLP = buildCurbFeature(offH3, offV3, idPoly3dFL, idPoly3dFL, true, ref handles, side);
                            Grading_Dict.addBrksToPntXDict(poly3dFL.ObjectId, idPoly3dLP, offH3, offV3, 0.0, -1.0);

                            ObjectId idPolyGT = idPoly3dLP.addPoly("Gutter");
                            handles.Add(idPolyGT.getHandle());
                        }

                        idPoly3dFL.setXData(handles, apps.lnkBrks3);

                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(ex.Message + " cmdBC1.cs: line: 140");
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdBC1.cs: line: 145");
            }
        }

        public static ObjectId
        buildCurbFeature(double offH, double offV, ObjectId idPoly3dFL, ObjectId idPoly3dRF, bool addPoints, ref List<Handle> handles, int side)
        {
            ObjectId idPoly3dOff = ObjectId.Null;

            try
            {
                List<Point3d> pnts3d = idPoly3dRF.getCoordinates3dList();
                List<Point3d> pnts3dNew = new List<Point3d>();

                List<ObjectId> idsCgPnts = new List<ObjectId>();

                idPoly3dOff = idPoly3dRF.offset3dPoly(offH, offV, side, "CPNT-BRKLINE");
                Handle hPoly3dOff = idPoly3dOff.getHandle();

                List<Point3d> pnts3dOff = idPoly3dOff.getCoordinates3dList();

                uint pntNum;
                foreach (Point3d pnt3d in pnts3dOff)
                {
                    ObjectId idCogoPnt = pnt3d.setPoint(out pntNum, "CPNT-ON");
                    handles.Add(idCogoPnt.getHandle());
                    idsCgPnts.Add(idCogoPnt);
                }
                foreach (ObjectId id in idsCgPnts)
                {
                    id.setXData(hPoly3dOff, apps.lnkBrks3);
                }

                idPoly3dOff.setXData(idPoly3dFL.Handle, apps.lnkBrks3);

                handles.Add(idPoly3dOff.Handle);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdBC1.cs: line: 184");
            }
            return idPoly3dOff;
        }

        public static List<ObjectId>
        getPoints(string prompt1, string prompt2)
        {
            List<ObjectId> idCgPnts = new List<ObjectId>();

            try
            {
                int n = 0;
                PromptStatus ps;
                bool done = false;
                Point3d pnt3d;
                bool escape = true;
                ObjectId idCgPnt = ObjectId.Null;
                do
                {
                    switch (n)
                    {
                        case 0:
                            idCgPnt = CgPnt.selectCogoPointByNode(prompt1, ref Pub.pnt3dO, out escape, out ps, osMode: 8);
                            n++;
                            break;

                        default:
                            pnt3d = idCgPnt.getCogoPntCoordinates();
                            idCgPnt = CgPnt.selectCogoPointByNode(prompt2, ref pnt3d, out escape, out ps, osMode: 8);

                            break;
                    }

                    switch (ps)
                    {
                        case PromptStatus.Cancel:
                            return idCgPnts;

                        case PromptStatus.None:
                            if (idCgPnts.Count < 2)
                            {
                                return idCgPnts;
                            }
                            done = true;
                            break;

                        case PromptStatus.OK:
                            idCgPnts.Add(idCgPnt);
                            break;
                    }
                }
                while (!done);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdBC1.cs: line: 240");
            }
            finally
            {
            }
            return idCgPnts;
        }
    }
}
