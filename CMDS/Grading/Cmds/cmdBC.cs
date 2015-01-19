using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Grading.Cmds
{
    public static class cmdBC
    {
        public static void
        BC(string nameCmd)
        {
            List<double> userInput = null;
            Polyline3d poly3dFL = null;
            ObjectId idPoly3dFL = ObjectId.Null;

            Point3d pnt3dBEG = Pub.pnt3dO;
            Point3d pnt3dEND = Pub.pnt3dO;
            Point3d pnt3dX = Pub.pnt3dO;

            int side;

            bool escape = false;
            try
            {
                userInput = getUserInput(nameCmd);
                if (userInput == null)
                    return;

                BaseObjs.acadActivate();

                int snapMode = SnapMode.getOSnap();
                SnapMode.setOSnap(8);

                if (Grading_Palette.gPalette.pGrading.optBRKLINE.IsChecked == true)
                {
                    Point3d pnt3dPick;
                    Entity ent = Select.selectEntity(typeof(Polyline3d), "Select Brkline:", "Brkline selection failed.", out pnt3dPick);
                    poly3dFL = (Polyline3d)ent;                                     //this 3dPoly is from points and is linked to the points with "apps.lnkBrks" !!!!!!!!!!!!!!!!!!!!!!!!!!
                    idPoly3dFL = poly3dFL.ObjectId;
                    ResultBuffer rb = idPoly3dFL.getXData(null);
                    if (rb == null)
                        return;
                }
                else if (Grading_Palette.gPalette.pGrading.optPNTS.IsChecked == true)
                {
                    ObjectId idPoly = ObjectId.Null;
                    try
                    {
                        idPoly3dFL = BrkLine.makeBreakline(apps.lnkBrks2, "BC", out idPoly);         //this 3dPoly is from points and is linked to the points with "lnkBrks2"    OK.
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(ex.Message + " cmdBC.cs: line: 58");
                    }
                }

                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        poly3dFL = (Polyline3d)tr.GetObject(idPoly3dFL, OpenMode.ForRead);
                        pnt3dBEG = poly3dFL.StartPoint;
                        pnt3dEND = poly3dFL.EndPoint;
                        PromptStatus ps;
                        pnt3dX = UserInput.getPoint("Pick point Back of Curb adjacent to segment", pnt3dBEG, out escape, out ps, osMode: 8);
                        if (pnt3dX == Pub.pnt3dO || escape)
                        {
                            SnapMode.setOSnap(snapMode);
                            return;
                        }
                        if (Geom.testRight(pnt3dBEG, pnt3dEND, pnt3dX) > 0)
                            side = -1;
                        else
                            side = 1;

                        double offH1 = 0.5 * 1.25 * side;
                        double offV1 = userInput[0] + 0.0208;
                        double offH2 = userInput[1] * side;
                        double offV2 = userInput[2] * userInput[1];

                        List<Handle> handles = new List<Handle>();

                        ObjectId idPoly3dTC = buildTCandBench(offH1, offV1, idPoly3dFL, idPoly3dFL, false, ref handles);        // collect handles from TC, BB, CgPnt, GT
                        Grading_Dict.addBrksToPntXDict(idPoly3dFL, idPoly3dTC, offH1, offV1, 0.0, -1.0);

                        ObjectId idPoly3dBB = buildTCandBench(offH2, offV2, idPoly3dFL, idPoly3dTC, true, ref handles);
                        Grading_Dict.addBrksToPntXDict(idPoly3dFL, idPoly3dBB, offH1 + offH2, offV1 + offV2, 0.0, -1.0);

                        ObjectId idPolyBB = idPoly3dBB.addPoly("GB");
                        idPoly3dBB.moveBottom();
                        handles.Add(idPolyBB.getHandle());
                        idPoly3dBB.moveToTop();

                        if (nameCmd == "cmdBG")
                        {
                            double offH3 = userInput[3] * side * -1;
                            double offV3 = userInput[4];

                            ObjectId idPoly3dLP = buildTCandBench(offH3, offV3, idPoly3dFL, idPoly3dFL, true, ref handles);
                            Grading_Dict.addBrksToPntXDict(poly3dFL.ObjectId, idPoly3dLP, offH3, offV3, 0.0, -1.0);

                            ObjectId idPolyGT = idPoly3dLP.addPoly("Gutter");
                            idPolyGT.moveBottom();
                            handles.Add(idPolyGT.getHandle());

                            idPoly3dLP.moveToTop();
                        }

                        idPoly3dFL.setXData(handles, apps.lnkBrks3);

                        SnapMode.setOSnap(snapMode);

                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(ex.Message + " cmdBC.cs: line: 123");
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdBC.cs: line: 128");
            }
        }

        public static ObjectId
        buildTCandBench(double offH, double offV, ObjectId idPoly3dFL, ObjectId idPoly3dRF, bool addPoints, ref List<Handle> handles)
        {
            ObjectId idPoly3dNew = ObjectId.Null;
            try
            {
                ObjectId idPoly = idPoly3dRF.addPoly();
                ObjectId idPolyOff = idPoly.offset(offH);

                Point3dCollection pnts3d = idPoly3dRF.getCoordinates3d();
                Point3dCollection pnts3dOff = idPolyOff.getCoordinates3d();
                Point3dCollection pnts3dNew = new Point3dCollection();

                List<ObjectId> idsCgPnts = new List<ObjectId>();

                idPoly.delete();
                idPolyOff.delete();

                if (pnts3d.Count == pnts3dOff.Count)
                {
                    uint pntNum;
                    for (int i = 0; i < pnts3dOff.Count; i++)
                    {
                        Point3d pnt3d = new Point3d(pnts3dOff[i].X, pnts3dOff[i].Y, pnts3d[i].Z + offV);
                        pnts3dNew.Add(pnt3d);
                        if (addPoints)
                        {
                            ObjectId idCogoPnt = pnt3d.setPoint(out pntNum, "CPNT-ON");
                            handles.Add(idCogoPnt.getHandle());
                            idsCgPnts.Add(idCogoPnt);
                        }
                    }
                    if (addPoints)
                    {
                        idPoly3dNew = BrkLine.makeBreakline(apps.lnkBrks, "cmdBC", out idPoly, idsCgPnts);
                        foreach (ObjectId id in idsCgPnts)
                        {
                            id.setXData(idPoly3dNew.getHandle(), apps.lnkBrks3);
                        }
                    }
                    else
                    {
                        idPoly3dNew = Draw.addPoly3d(pnts3dNew, "CPNT-BRKLINE");
                    }
                    idPoly3dNew.setXData(idPoly3dFL.Handle, apps.lnkBrks3);
                    handles.Add(idPoly3dNew.Handle);
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdBC.cs: line: 182");
            }
            return idPoly3dNew;
        }

        public static List<double>
        getUserInput(string nameCmd)
        {
            bool escape = false;
            List<double> userInput = new List<double>();
            try
            {
                TypedValue[] TVs = null;
                ResultBuffer RB = null;
                bool exists = false;
                if (nameCmd == "cmdABG")
                    nameCmd = "cmdBG";

                if (nameCmd == "cmdABC")
                    nameCmd = "cmdBC";

                ObjectId idDict = Dict.getNamedDictionary(nameCmd, out exists);
                if (!exists)
                {
                    userInput.Add(0.5);
                    userInput.Add(1.5);
                    userInput.Add(0.02);

                    userInput.Add(1.5);
                    userInput.Add(0.1875);

                    TVs = new TypedValue[userInput.Count];
                    for (int i = 0; i < userInput.Count; i++)
                    {
                        TVs.SetValue(new TypedValue(1040, userInput[i]), i);
                    }
                    RB = new ResultBuffer(TVs);
                    Dict.addXRec(idDict, "cmdDefault", RB);
                }
                else
                {
                    RB = Dict.getXRec(idDict, "cmdDefault");
                    TVs = RB.AsArray();
                    for (int i = 0; i < TVs.Length; i++)
                    {
                        if (TVs[i].Value.ToString() != "")
                            userInput.Add(Convert.ToDouble(TVs[i].Value));
                        else
                        {
                            switch (i)
                            {
                                case 0:
                                case 1:
                                case 2:
                                    break;

                                case 3:
                                    userInput.Add(1.5);
                                    break;

                                case 4:
                                    userInput.Add(0.1875);
                                    break;
                            }
                        }
                    }
                }

                if (RB != null)
                {
                    string message1 = "Curb Height: ";
                    string message2 = "\nBench Width: ";
                    string message3 = "\nBench Slope: ";

                    string message = string.Format("{0}{1}{2}{3}{4}{5}", message1.PadRight(50), userInput[0].ToString(), message2.PadRight(49), userInput[1].ToString(), message3.PadRight(50), userInput[2].ToString());
                    if (nameCmd == "cmdBG")
                    {
                        string message4 = "\nGutter Width: ";
                        string message5 = "\nGutter Vertical Difference: ";

                        message = string.Format("{0}{1}{2}{3}{4}", message, message4.PadRight(50), userInput[3].ToString(), message5.PadRight(42), userInput[4].ToString());
                    }

                    var response = MessageBox.Show(message, "Accept Current Settings?", MessageBoxButtons.YesNoCancel);
                    if (response == DialogResult.Yes)
                        return userInput;
                    if (response == DialogResult.Cancel)
                        return null;
                }
                double value = 0;
                escape = UserInput.getUserInput("\nEnter Curb Height in Feet: ", out value, Convert.ToDouble(userInput[0]));
                if (escape)
                    return null;
                userInput[0] = value;
                escape = UserInput.getUserInput("\nEnter Bench Width in feet (from Back of Curb to Grade Break): ", out value, Convert.ToDouble(userInput[1]));
                if (escape)
                    return null;
                userInput[1] = value;
                escape = UserInput.getUserInput("\nEnter Bench Slope: ", out value, Convert.ToDouble(userInput[2]));
                if (escape)
                    return null;
                userInput[2] = value;

                if (nameCmd == "cmdBG")
                {
                    escape = UserInput.getUserInput("\nEnter Gutter Width in feet (distance from Curb Line to Edge of Pavement: ", out value, Convert.ToDouble(userInput[3]));
                    if (escape)
                        return null;
                    userInput[3] = value;
                    escape = UserInput.getUserInput("\nEnter Vert. Diff (FL to Lip of Gutter/Edge of Pavement: ", out value, Convert.ToDouble(userInput[4]));
                    if (escape)
                        return null;
                    userInput[4] = value;
                }

                Dict.deleteXRec(idDict, "cmdDefault");

                TVs = new TypedValue[userInput.Count];

                for (int i = 0; i < userInput.Count; i++)
                {
                    TVs.SetValue(new TypedValue(1040, userInput[i]), i);
                }
                RB = new ResultBuffer(TVs);
                Dict.addXRec(idDict, "cmdDefault", RB);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdBC.cs: line: 310");
            }

            return userInput;
        }
    }
}
