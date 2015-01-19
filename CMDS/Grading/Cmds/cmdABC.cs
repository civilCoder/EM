using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.Linq;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace Grading.Cmds
{
    public static class cmdABC
    {
        private static double pi = System.Math.PI;

        public static void
        ABC(string nameCmd)
        {
            Arc arc0 = null;

            Point3d pnt3dBeg = Pub.pnt3dO, pnt3dEnd = Pub.pnt3dO, pnt3dX = Pub.pnt3dO;

            ObjectId idCgPntEnd = ObjectId.Null;
            ObjectId idPoly = ObjectId.Null;

            List<Point3d> pnts3d = null, pnts3dEntX = null;

            bool escape = true;
            Object xRefPathX = null;
            PromptStatus ps;
            FullSubentityPath path;
            Point3d pnt3dPick;
            bool isClosed = false;

            double grd = 0.0;
            Entity ent = xRef.getNestedEntity("\nSelect ARC: ", out escape, out xRefPathX, out ps, out pnts3dEntX, out path, out pnt3dPick, out isClosed);
            if (ent.GetType().ToString() != "Autodesk.AutoCAD.DatabaseServices.Arc")
            {
                Application.ShowAlertDialog(string.Format("Selected object was type of: {0} - Exiting...", ent.GetType().ToString()));
                return;
            }
            else
            {
                arc0 = (Arc)ent;
            }

            pnt3dX = UserInput.getPoint("\nPick point Back of Curb adjacent to arc segment", arc0.StartPoint, out escape, out ps, osMode: 8);

            double distX = arc0.Center.getDistance(pnt3dX);
            if (distX == arc0.Radius)
            {
                Application.ShowAlertDialog("Selected point is on arc.  Select point away from street side: ");
                pnt3dX = UserInput.getPoint("\nPick point Back of Curb adjacent to arc segment", pnt3dBeg, out escape, out ps, osMode: 8);
                distX = arc0.Center.getDistance(pnt3dX);
                if (distX == arc0.Radius)
                    return;
            }

            int side = -1;
            int n = (arc0.Normal.Z > 0) ? 1 : -1;
            switch (n)
            {
                case 1:                 //right hand direction
                    if (distX > arc0.Radius)
                        side = 1;
                    break;

                case -1:                //left hand direction
                    if (distX < arc0.Radius)
                        side = 1;
                    break;
            }

            string elev = "", prompt = "\nSelect Cogo Point for begin elevation at beginning of curve (as indicated by guideline): ";

            ObjectId idCgPntBeg = getEndPoint(pnt3dBeg, out elev, prompt);
            if (elev == "")
                return;

            List<ObjectId> idsCgPnt = new List<ObjectId>();
            idsCgPnt.Add(idCgPntBeg);

            pnt3dBeg = idCgPntBeg.getCogoPntCoordinates();
            BaseObjs.write(string.Format("\nBegin Elevation = {0:F2}", elev));

            if (arc0.EndPoint.isEqual(pnt3dBeg, 0.1))
            {
                Curve curve = arc0;
                curve.ReverseCurve();                                     //*****************************************************************************
            }
            else if (arc0.StartPoint.isEqual(pnt3dBeg, 0.1))
            {
            }
            else
            {
                Application.ShowAlertDialog("Selected Begin point is not at either end of the arc.  Revise and retry...");
                return;
            }

            uint pntNum = 0;

            ObjectId idCgPntX = ObjectId.Null;

            List<double> userInput = null;
            userInput = cmdBC.getUserInput(nameCmd);
            if (userInput == null)
                return;

            double offH1 = 0.5 * 1.25 * side;
            double offV1 = userInput[0] + 0.0208;
            double offH2 = userInput[1] * side;
            double offV2 = userInput[2] * userInput[1];
            double offH3 = 0;
            double offV3 = 0;

            List<double> offHs = new List<double> { 0.0, offH1, offH2 };
            List<double> offVs = new List<double> { 0.0, offV1, offV2 };

            List<Arc> arcs = new List<Arc>();
            arcs.Add(arc0);

            DBObjectCollection dbObjs = arc0.GetOffsetCurves(offH1);
            Arc arc1 = (Arc)dbObjs[0];
            arcs.Add(arc1);
            dbObjs = arc1.GetOffsetCurves(offH2);
            Arc arc2 = (Arc)dbObjs[0];
            arcs.Add(arc2);
            Arc arc3 = null;
            if (nameCmd == "cmdABG")
            {
                offH3 = (userInput[3] * side * -1) - offH1 - offH2;  //offset from bench
                offV3 = userInput[4] - offV1 - offV2;                //offset from bench
                offHs.Add(offH3);
                offVs.Add(offV3);

                dbObjs = arc0.GetOffsetCurves(userInput[3] * side * -1);
                arc3 = (Arc)dbObjs[0];
                arcs.Add(arc3);
            }

            string cmdDefault = "R";
            prompt = string.Format("\nStraight grade(two or more points)/slope Intercept/Vertical curve/Plane method/Modified plane method/Reinhard plane method: S/I/V/P/M/R <{0}> [S/I/V/P/M/R]:", cmdDefault);
            escape = UserInput.getUserInputKeyword(cmdDefault, out cmdDefault, prompt, "S I V P M R");
            if (escape)
                return;

            Vector3d v3dBeg = new Vector3d(0, 0, 0);
            Vector3d v3dEnd = new Vector3d(0, 0, 0);

            List<arcInfo> arcInfos = new List<arcInfo>();

            arcInfo aInfo = getArcInfo(pnt3dBeg, arc0, nameCmd, offHs, offVs, idCgPntBeg);
            arcInfos.Add(aInfo);


            Point3d pnt3d = pnt3dBeg;

            switch (cmdDefault)
            {
                #region "S"

                case "S":                                                               // Assuming points are set at both ends, with optional points set at intermediate locations on arc
                    bool Done = false;
                    do
                    {
                        prompt = "\nSelect Cogo Point on Arc (Arc endpoint or intermediate point) ENTER when done: ";
                        idCgPntEnd = getEndPoint(pnt3d, out elev, prompt);
                        if (elev == "")
                            Done = true;
                        else
                        {
                            pnt3d = idCgPntEnd.getCogoPntCoordinates();
                            idsCgPnt.Add(idCgPntEnd);

                            aInfo = getArcInfo(pnt3d, arc0, nameCmd, offHs, offVs, idCgPntEnd);
                            arcInfos.Add(aInfo);
                        }
                    }
                    while (!Done);

                    var sortByLen = from a in arcInfos
                                    orderby a.distFromBeg ascending
                                    select a;
                    int i = -1;
                    foreach (var b in sortByLen)
                        arcInfos[++i] = b;

                    processArcInfo(ref arcInfos);

                    switch (idsCgPnt.Count)
                    {
                        case 1:
                            return;

                        default:
                            processArcs(arcs, arcInfos, arc0, idsCgPnt, nameCmd, offHs, offVs);
                            break;
                    }

                    break;

                #endregion "S"

                #region "I"

                case "I":

                    double m1 = Pub.slp1;
                    double m2 = Pub.slp2;

                    prompt = "\nSelect Cogo Point for end elevation: ";
                    idCgPntEnd = getEndPoint(pnt3dBeg, out elev, prompt);
                    if (elev == "")
                        return;
                    pnt3dEnd = idCgPntEnd.getCogoPntCoordinates();
                    idsCgPnt.Add(idCgPntEnd);

                    grd = (pnt3dEnd.Z - pnt3dBeg.Z) / arc0.Length;
                    BaseObjs.write(string.Format("\nSlope along curve is: {0:P2}", grd));

                    m1 = Pub.slp1;
                    m2 = Pub.slp2;

                    UserInput.getUserInput("\nEnter slope from FIRST POINT: ", out m1, m1);
                    if (m1 == -99.99)
                    {
                        return;
                    }
                    UserInput.getUserInput("\nEnter slope from SECOND POINT: ", out m2, m2);
                    if (m2 == -99.99)
                    {
                        return;
                    }

                    Pub.slp1 = m1;
                    Pub.slp2 = m2;

                    double elev1 = pnt3dBeg.Z;
                    double elev2 = pnt3dEnd.Z;
                    distX = cmdSSP.getSlopeInterceptArc(arc0.Length, elev1, elev2, m1, -m2);
                    if (distX == 0)
                    {
                        return;
                    }

                    Point3d pnt3dInt = arc0.GetPointAtDist(distX);
                    pnt3dInt = new Point3d(pnt3dInt.X, pnt3dInt.Y, elev1 + distX * m1);
                    ObjectId idCgPntInt = pnt3dInt.setPoint(out pntNum);
                    idsCgPnt.Add(idCgPntInt);

                    Application.ShowAlertDialog(string.Format("The elevation of the slope\nintercept is {0:F2}", pnt3dInt.Z));


                    aInfo = getArcInfo(pnt3dInt, arc0, nameCmd, offHs, offVs, idCgPntInt);
                    arcInfos.Add(aInfo);


                    aInfo = getArcInfo(pnt3dEnd, arc0, nameCmd, offHs, offVs, idCgPntEnd);
                    arcInfos.Add(aInfo);


                    processArcInfo(ref arcInfos);

                    processArcs(arcs, arcInfos, arc0, idsCgPnt, nameCmd, offHs, offVs);

                    break;

                #endregion "I"

                #region "V"

                case "V":
                    prompt = "\nSelect Cogo Point for end elevation: ";
                    idCgPntEnd = getEndPoint(pnt3dBeg, out elev, prompt);
                    if (elev == "")
                        return;
                    pnt3dEnd = idCgPntEnd.getCogoPntCoordinates();
                    idsCgPnt.Add(idCgPntEnd);

                    grd = (pnt3dEnd.Z - pnt3dBeg.Z) / arc0.Length;
                    BaseObjs.write(string.Format("\nSlope along curve is: {0:P2}", grd));

                    m1 = Pub.slp1;
                    m2 = Pub.slp2;

                    UserInput.getUserInput("\nEnter slope from FIRST POINT: ", out m1, m1);
                    if (m1 == -99.99)
                    {
                        return;
                    }
                    UserInput.getUserInput("\nEnter slope from SECOND POINT: ", out m2, m2);
                    if (m2 == -99.99)
                    {
                        return;
                    }
                    string lenVC = "20";
                    escape = UserInput.getUserInputDoubleAsString("\nEnter length of vertical curve: ", out lenVC, lenVC);
                    if (escape)
                        return;

                    Pub.slp1 = m1;
                    Pub.slp2 = m2;

                    elev1 = pnt3dBeg.Z;
                    elev2 = pnt3dEnd.Z;
                    distX = cmdSSP.getSlopeInterceptArc(arc0.Length, elev1, elev2, m1, -m2);
                    if (distX == 0)
                    {
                        return;
                    }

                    pnt3dInt = arc0.GetPointAtDist(distX);
                    pnt3dInt = new Point3d(pnt3dInt.X, pnt3dInt.Y, elev1 + distX * m1);

                    Application.ShowAlertDialog(string.Format("The elevation of the slope\nintercept is {0:F2}", pnt3dInt.Z));

                    double lVC = double.Parse(lenVC);
                    if ((distX - lVC / 2) < 0 || (distX + lVC / 2) > arc0.Length)
                    {
                        double lenMax = distX * 2;
                        if ((arc0.Length - distX) < lenMax / 2)
                        {
                            lenMax = (arc0.Length - distX) * 2;
                            Application.ShowAlertDialog(string.Format("Maximum length of Vertical Curve length with current inputs is {0:F2}", lenMax));
                        }
                    }

                    pnts3d = arc0.traverse(m1, -m2, distX, lVC, pnt3dBeg);

                    BrkLine.makeBreakline(apps.lnkBrks, nameCmd, out idPoly, idsCgPnt, pnts3dL: pnts3d);
                    break;

                #endregion "V"

                #region "M"

                case "M":
                    prompt = "\nSelect Begin Curve backsight Cogo Point for grade and direction: ";
                    Point3d pnt3dBegBS = UserInput.getPoint(prompt, pnt3dBeg, out escape, out ps, osMode: 8);
                    if (pnt3dBegBS == Pub.pnt3dO)
                        return;

                    prompt = "\nSelect Cogo Point for end elevation: ";
                    idCgPntEnd = getEndPoint(pnt3dBeg, out elev, prompt);
                    if (elev == "")
                        return;
                    pnt3dEnd = idCgPntEnd.getCogoPntCoordinates();

                    grd = (pnt3dEnd.Z - pnt3dBeg.Z) / arc0.Length;
                    BaseObjs.write(string.Format("\nSlope along curve is: {0:P2}", grd));

                    prompt = "\nSelect End Curve backsight Cogo Point for grade and direction: ";
                    Point3d pnt3dEndBS = UserInput.getPoint(prompt, pnt3dEnd, out escape, out ps, osMode: 8);
                    if (pnt3dEndBS == Pub.pnt3dO)
                        return;

                    Point3d pnt3dPI = Geom.getPntInt(pnt3dBegBS, pnt3dBeg, pnt3dEndBS, pnt3dEnd, true, extend.both);

                    double g1 = pnt3dBegBS.getSlope(pnt3dBeg);
                    double g2 = pnt3dEnd.getSlope(pnt3dEndBS);

                    double el1 = pnt3dBeg.Z + g1 * pnt3dBeg.getDistance(pnt3dPI);
                    double el2 = pnt3dEnd.Z - g2 * pnt3dEnd.getDistance(pnt3dPI);

                    double ratio = 0;

                    double aDiff = g1 - g2;
                    double el = 0;

                    if (g1 < 0 && g2 < 0)
                    {
                        ratio = g1 / (g1 + g2);
                        if (g1 <= g2)
                        { //g1 controls
                            if (el1 < el2)
                            {
                                el = el1 + (el2 - el1) * ratio;
                            }
                            else if (el1 > el2)
                            {
                                el = el1 - (el1 - el2) * ratio;
                            }
                        }
                        else if (g2 < g1)
                        { //g2 controls
                            if (el2 < el1)
                            {
                                el = el2 + (el1 - el2) * (1 - ratio);
                            }
                            else if (el2 > el1)
                            {
                                el = el2 - (el2 - el1) * (1 - ratio);
                            }
                        }
                    }
                    else if (g1 > 0 && g2 > 0)
                    {
                        ratio = g1 / (g1 + g2);
                        if (g1 >= g2)
                        { //g1 controls
                            if (el1 < el2)
                            {
                                el = el1 + (el2 - el1) * ratio;
                            }
                            else if (el1 > el2)
                            {
                                el = el1 - (el1 - el2) * ratio;
                            }
                        }
                        else if (g2 > g1)
                        { //g2 controls
                            if (el2 < el1)
                            {
                                el = el2 + (el1 - el2) * (1 - ratio);
                            }
                            else if (el2 > el1)
                            {
                                el = el2 - (el2 - el1) * (1 - ratio);
                            }
                        }
                    }
                    else
                    {
                        ratio = g1 / aDiff;
                        if (ratio >= (1 - ratio))
                        { //g1 controls
                            if (el1 < el2)
                            {
                                el = el1 + (el2 - el1) * ratio;
                            }
                            else if (el1 > el2)
                            {
                                el = el1 - (el1 - el2) * ratio;
                            }
                        }
                        else if ((1 - ratio) > ratio)
                        { //g2 controls
                            if (el2 < el1)
                            {
                                el = el2 + (el1 - el2) * (1 - ratio);
                            }
                            else if (el2 > el1)
                            {
                                el = el2 - (el2 - el1) * (1 - ratio);
                            }
                        }
                    }

                    pnt3dPI = new Point3d(pnt3dPI.X, pnt3dPI.Y, el);
                    pnt3dPI.setPoint(out pntNum);
                    BaseObjs.updateGraphics();

                    double g1Mod = pnt3dBeg.getSlope(pnt3dPI);
                    double g2Mod = pnt3dPI.getSlope(pnt3dEnd);

                    g1 = (g1 + g1Mod) / 2;
                    g2 = (g2 + g2Mod) / 2;

                    Point3d pnt3dChordMid = pnt3dBeg.getMidPoint3d(pnt3dEnd);

                    Vector3d v3dM = pnt3dPI - pnt3dChordMid;

                    pnts3d = arc0.planeMethodModified(pnt3dBeg, pnt3dEnd, pnt3dPI, v3dM, g1, g2, ref idsCgPnt);
                    idsCgPnt.Add(idCgPntEnd);

                    arcInfos = new List<arcInfo>();
                    for (int s = 0; s < pnts3d.Count; s++)
                    {
                        pnt3dX = pnts3d[s];
                        aInfo = getArcInfo(pnt3dX, arc0, nameCmd, offHs, offVs, idsCgPnt[s]);
                        arcInfos.Add(aInfo);
                    }

                    processArcInfo(ref arcInfos);
                    processArcs(arcs, arcInfos, arc0, idsCgPnt, nameCmd, offHs, offVs);

                    break;

                #endregion "M"

                #region "R"

                case "R":
                    prompt = "\nSelect Begin Curve backsight Cogo Point for grade and direction: ";
                    pnt3dBegBS = UserInput.getPoint(prompt, pnt3dBeg, out escape, out ps, osMode: 8);
                    if (pnt3dBegBS == Pub.pnt3dO)
                        return;

                    prompt = "\nSelect Cogo Point for end elevation: ";
                    idCgPntEnd = getEndPoint(pnt3dBeg, out elev, prompt);
                    if (elev == "")
                        return;
                    pnt3dEnd = idCgPntEnd.getCogoPntCoordinates();

                    grd = (pnt3dEnd.Z - pnt3dBeg.Z) / arc0.Length;
                    BaseObjs.write(string.Format("\nSlope along curve is: {0:P2}", grd));

                    prompt = "\nSelect End Curve backsight Cogo Point for grade and direction: ";
                    pnt3dEndBS = UserInput.getPoint(prompt, pnt3dEnd, out escape, out ps, osMode: 8);
                    if (pnt3dEndBS == Pub.pnt3dO)
                        return;

                    pnt3dPI = Geom.getPntInt(pnt3dBegBS, pnt3dBeg, pnt3dEndBS, pnt3dEnd, true, extend.both);

                    g1 = pnt3dBegBS.getSlope(pnt3dBeg);
                    g2 = pnt3dEnd.getSlope(pnt3dEndBS);
                    string resOut = "";

                    prompt = string.Format("\nUse g1={0:P2} or g2={1:P2}?: [1/2]", g1, g2);
                    escape = UserInput.getUserInputKeyword("1", out resOut, prompt, "1 2");
                    if (escape)
                        return;

                    el = 0;
                    switch (resOut)
                    {
                        case "1":
                            el = pnt3dBeg.Z + g1 * pnt3dBeg.getDistance(pnt3dPI);
                            break;

                        case "2":
                            el = pnt3dEnd.Z - g2 * pnt3dEnd.getDistance(pnt3dPI);
                            break;
                    }

                    pnt3dPI = new Point3d(pnt3dPI.X, pnt3dPI.Y, el);
                    pnt3dPI.setPoint(out pntNum);
                    BaseObjs.updateGraphics();

                    pnts3d = arc0.planeMethodReinhard(pnt3dBeg, pnt3dEnd, pnt3dPI, ref idsCgPnt);
                    idsCgPnt.Add(idCgPntEnd);

                    arcInfos = new List<arcInfo>();
                    for (int s = 0; s < pnts3d.Count; s++)
                    {
                        pnt3dX = pnts3d[s];
                        aInfo = getArcInfo(pnt3dX, arc0, nameCmd, offHs, offVs, idsCgPnt[s]);
                        arcInfos.Add(aInfo);
                    }

                    processArcInfo(ref arcInfos);
                    processArcs(arcs, arcInfos, arc0, idsCgPnt, nameCmd, offHs, offVs);

                    break;

                #endregion "R"

                #region "P"

                case "P":
                    prompt = "S\nelect Begin Curve backsight Cogo Point for grade and direction: ";
                    pnt3dBegBS = UserInput.getPoint(prompt, pnt3dBeg, out escape, out ps, osMode: 8);
                    if (pnt3dBegBS == Pub.pnt3dO)
                        return;

                    prompt = "\nSelect Cogo Point for end elevation: ";
                    idCgPntEnd = getEndPoint(pnt3dBeg, out elev, prompt);
                    if (elev == "")
                        return;
                    pnt3dEnd = idCgPntEnd.getCogoPntCoordinates();

                    grd = (pnt3dEnd.Z - pnt3dBeg.Z) / arc0.Length;
                    BaseObjs.write(string.Format("\nSlope along curve is: {0:P2}", grd));

                    prompt = "\nSelect End Curve backsight Cogo Point for grade and direction: ";
                    pnt3dEndBS = UserInput.getPoint(prompt, pnt3dEnd, out escape, out ps, osMode: 8);
                    if (pnt3dEndBS == Pub.pnt3dO)
                        return;

                    pnt3dPI = Geom.getPntInt(pnt3dBegBS, pnt3dBeg, pnt3dEndBS, pnt3dEnd, true, extend.both);

                    g1 = pnt3dBegBS.getSlope(pnt3dBeg);
                    g2 = pnt3dEndBS.getSlope(pnt3dEnd);

                    el1 = pnt3dBeg.Z + g1 * pnt3dBeg.getDistance(pnt3dPI);
                    el2 = pnt3dEnd.Z + g2 * pnt3dEnd.getDistance(pnt3dPI);

                    el = (el1 + el2) / 2;

                    pnt3dPI = new Point3d(pnt3dPI.X, pnt3dPI.Y, el);

                    pnt3dChordMid = pnt3dBeg.getMidPoint3d(pnt3dEnd);

                    v3dM = pnt3dPI - pnt3dChordMid;

                    pnts3d = arc0.planeMethod(pnt3dBeg, pnt3dEnd, pnt3dPI, v3dM, ref idsCgPnt, nameCmd);
                    idsCgPnt.Add(idCgPntEnd);

                    arcInfos = new List<arcInfo>();
                    for (int s = 0; s < pnts3d.Count; s++)
                    {
                        pnt3dX = pnts3d[s];
                        aInfo = getArcInfo(pnt3dX, arc0, nameCmd, offHs, offVs, idsCgPnt[s]);
                        arcInfos.Add(aInfo);
                    }

                    processArcInfo(ref arcInfos);
                    processArcs(arcs, arcInfos, arc0, idsCgPnt, nameCmd, offHs, offVs);

                    break;

                #endregion "P"
            }
        }

        private static void
        processArcs(List<Arc> arcs, List<arcInfo> arcInfos, Arc arc0, List<ObjectId> idsCgPnt, string nameCmd, List<double> offHs, List<double> offVs)
        {
            ObjectId idPoly = ObjectId.Null;
            List<Handle> handles = new List<Handle>();
            ObjectId idFL = ObjectId.Null;
            for (int y = 0; y < arcs.Count; y++)
            {
                Arc arc = arcs[y];
                List<Point3d> pnts3d = new List<Point3d>();

                for (int p = 0; p < arcInfos.Count - 1; p++)
                {
                    Vector3d v3dBeg = new Vector3d(0, 0, 0);
                    Vector3d v3dEnd = new Vector3d(0, 0, 0);
                    for (int a = 0; a < y + 1; a++)
                    {
                        v3dBeg += arcInfos[p].v3ds[a];
                        v3dEnd += arcInfos[p + 1].v3ds[a];
                    }
                    Point3d pnt3dBeg = arcInfos[p].idCgPnt.getCogoPntCoordinates();
                    pnt3dBeg += v3dBeg;

                    Point3d pnt3dEnd = arcInfos[p + 1].idCgPnt.getCogoPntCoordinates();
                    pnt3dEnd += v3dEnd;

                    double dist = arcInfos[p].distToNext * arcs[y].Length / arc0.Length;

                    double grd = (pnt3dEnd.Z - pnt3dBeg.Z) / dist;

                    double staBeg = arcInfos[p].distFromBeg * arcs[y].Length / arc0.Length;
                    arc.traverse(pnt3dBeg, grd, dist, staBeg, ref pnts3d);
                }

                ObjectId idBrk = ObjectId.Null;

                if (y == 0)
                {
                    idFL = BrkLine.makeBreakline(apps.lnkBrks3, nameCmd, out idPoly, idsCgPnt, pnts3dL: pnts3d);
                    foreach (ObjectId id in idsCgPnt)
                    {
                        id.updatePntXData(idFL, apps.lnkBrks2);
                    }
                }
                else
                {
                    idBrk = BrkLine.makeBreakline(apps.lnkBrks, nameCmd, out idPoly, idsCgPnt, pnts3dL: pnts3d);
                    Grading_Dict.addBrksToPntXDict(idFL, idBrk, offHs[y], offVs[y], 0.0, -1);
                    if (y == 2)
                    {
                        using (Transaction tr = BaseObjs.startTransactionDb())
                        {
                            BlockTableRecord ms = Blocks.getBlockTableRecordMS();
                            idPoly = arcs[2].convertArcToPolyline("GB");
                            tr.Commit();
                        }
                        idPoly.moveBottom();
                        idBrk.moveToTop();
                    }
                    handles.Add(idBrk.getHandle());
                }

                BaseObjs.updateGraphics();
            }

            idFL.setXData(handles, apps.lnkBrks3);
        }

        private static void
        processArcInfo(ref List<arcInfo> arcInfos)
        {
            for (int a = 1; a < arcInfos.Count; a++)
            {
                arcInfo aInf = new arcInfo();
                aInf = arcInfos[a - 1];
                aInf.distToNext = arcInfos[a].distFromBeg - arcInfos[a - 1].distFromBeg;
                aInf.grd = (arcInfos[a].elev - arcInfos[a - 1].elev) / aInf.distToNext;
                arcInfos[a - 1] = aInf;
            }
        }

        private static arcInfo
        getArcInfo(Point3d pnt3d, Arc arc0, string nameCMD, List<double> offH, List<double> offV, ObjectId idCgPntBeg)
        {
            double dir = arc0.Center.getDirection(pnt3d);
            double dist = arc0.getDistanceToPointOnArc(pnt3d);

            List<Vector3d> v3ds = new List<Vector3d> {
                new Vector3d(0, 0, 0),
                new Vector3d(System.Math.Cos(dir) * offH[1], System.Math.Sin(dir) * offH[1], offV[1]),
                new Vector3d(System.Math.Cos(dir) * offH[2], System.Math.Sin(dir) * offH[2], offV[2])
            };

            if (nameCMD == "cmdABG")
            {
                v3ds.Add(new Vector3d(System.Math.Cos(dir) * offH[3], System.Math.Sin(dir) * offH[3], offV[3]));
            }

            arcInfo aInfo = new arcInfo { distFromBeg = dist, idCgPnt = idCgPntBeg, elev = pnt3d.Z, v3ds = v3ds };
            return aInfo;
        }

        public static List<Point3d>
        planeMethodReinhard(this Arc arc, Point3d pnt3dBeg, Point3d pnt3dEnd, Point3d pnt3dPI, ref List<ObjectId> idsCgPnt)
        {
            Point3d pnt3dM1 = arc.GetPointAtDist(arc.Length * 1 / 4);   //1st quarter delta on arc
            Point3d pnt3dM2 = arc.GetPointAtDist(arc.Length * 2 / 4);   //2nd quarter delta on arc
            Point3d pnt3dM3 = arc.GetPointAtDist(arc.Length * 3 / 4);   //3rd quarter delta on arc

            double elMOC = (2 * pnt3dPI.Z + pnt3dBeg.Z + pnt3dEnd.Z) / 4;
            double elM1 = (2 * pnt3dBeg.Z + elMOC + pnt3dPI.Z) / 4;
            double elM3 = (2 * pnt3dEnd.Z + elMOC + pnt3dPI.Z) / 4;

            pnt3dM1 = pnt3dM1.addElevation(elM1);
            pnt3dM2 = pnt3dM2.addElevation(elMOC);
            pnt3dM3 = pnt3dM3.addElevation(elM3);

            uint pntNum = 0;
            ObjectId idCgPntM1 = pnt3dM1.setPoint(out pntNum);
            ObjectId idCgPntM2 = pnt3dM2.setPoint(out pntNum);
            ObjectId idCgPntM3 = pnt3dM3.setPoint(out pntNum);

            idsCgPnt.Add(idCgPntM1);
            idsCgPnt.Add(idCgPntM2);
            idsCgPnt.Add(idCgPntM3);

            List<Point3d> pnts3d = new List<Point3d> { pnt3dBeg, pnt3dM1, pnt3dM2, pnt3dM3, pnt3dEnd };

            return pnts3d;
        }

        public static List<Point3d>
        planeMethodModified(this Arc arc, Point3d pnt3dBeg, Point3d pnt3dEnd, Point3d pnt3dPI, Vector3d v3dM, double g1, double g2, ref List<ObjectId> idsCgPnt)
        {
            double delta = arc.TotalAngle;
            double increDelta = delta / 4;

            double increX = increDelta * arc.Radius;

            Point3d pnt3dM1 = arc.GetPointAtDist(arc.Length * 1 / 4);   //1st quarter delta on arc
            Point3d pnt3dM2 = arc.GetPointAtDist(arc.Length * 2 / 4);   //2nd quarter delta on arc
            Point3d pnt3dM3 = arc.GetPointAtDist(arc.Length * 3 / 4);   //3rd quarter delta on arc

            Vector3d v3dPI = pnt3dPI - pnt3dBeg;        //vector from beg to PI

            Vector3d v3dM1 = pnt3dM1 - pnt3dBeg;        //vector from beg to 1st quarter delta on arc
            Vector3d v3dM2 = pnt3dM2 - pnt3dBeg;        //vector from beg to 2nd quarter delta on arc
            Vector3d v3dM3 = pnt3dM3 - pnt3dBeg;        //vector from beg to 3rd quarter delta on arc

            Vector3d v3dC = pnt3dEnd - pnt3dBeg;        //vector from beg to end of arc - long chord

            double delta0 = v3dPI.getAngle2Vectors(v3dC);       //delta / 2
            double deltaM1 = v3dM1.getAngle2Vectors(v3dC);      //delta from long chord to 1st quarter point
            double deltaM2 = v3dM2.getAngle2Vectors(v3dC);      //delta from long chord to 2nd quarter point
            double deltaM3 = v3dM3.getAngle2Vectors(v3dC);      //delta from long chord to 3rd quarter point

            double lenC0 = pnt3dBeg.getDistance(pnt3dEnd);      //length of long chord
            double lenC1 = pnt3dBeg.getDistance(pnt3dM1) * System.Math.Cos(deltaM1);    //length of chord from beg to 1st quarter delta
            double lenC2 = pnt3dBeg.getDistance(pnt3dM2) * System.Math.Cos(deltaM2);    //length of chord from beg to 2nd quarter delta
            double lenC3 = pnt3dBeg.getDistance(pnt3dM3) * System.Math.Cos(deltaM3);    //length of chord from beg to 3rd quarter delta

            Point3d pnt3dC1 = pnt3dBeg + v3dC * lenC1 / lenC0;      //point on long chord based on ratio of quarter chords vs long chord
            Point3d pnt3dC2 = pnt3dBeg + v3dC * lenC2 / lenC0;
            Point3d pnt3dC3 = pnt3dBeg + v3dC * lenC3 / lenC0;

            double lenPI = pnt3dC2.getDistance(pnt3dPI);      //middle ordinate + external distance

            uint pntNum;
            ObjectId idcgPnt = pnt3dC1.setPoint(out pntNum);    //points on long chord
            idcgPnt = pnt3dC2.setPoint(out pntNum);             //points on long chord
            idcgPnt = pnt3dC3.setPoint(out pntNum);             //points on long chord
            BaseObjs.updateGraphics();

            Point3d pnt3dT1 = pnt3dC1.traverse(pnt3dC2.getDirection(pnt3dPI), pnt3dBeg.getDistance(pnt3dC1) * System.Math.Tan(v3dPI.getAngle2Vectors(pnt3dEnd - pnt3dBeg)));
            Point3d pnt3dT3 = pnt3dC3.traverse(pnt3dC2.getDirection(pnt3dPI), pnt3dEnd.getDistance(pnt3dC3) * System.Math.Tan((pnt3dPI - pnt3dEnd).getAngle2Vectors(pnt3dBeg - pnt3dEnd)));
            BaseObjs.updateGraphics();

            pnt3dT1 = new Point3d(pnt3dT1.X, pnt3dT1.Y, pnt3dBeg.Z + pnt3dBeg.getDistance(pnt3dT1) * g1);
            pnt3dT1.setPoint(out pntNum);
            Vector3d v3dT1 = pnt3dT1 - pnt3dC1;

            pnt3dT3 = new Point3d(pnt3dT3.X, pnt3dT3.Y, pnt3dEnd.Z + pnt3dEnd.getDistance(pnt3dT3) * g2);
            pnt3dT3.setPoint(out pntNum);
            Vector3d v3dT3 = pnt3dT3 - pnt3dC3;

            pnt3dM1 = pnt3dC1 + v3dT1 * pnt3dC1.getDistance(pnt3dM1) / pnt3dC1.getDistance(pnt3dT1);
            pnt3dM2 = pnt3dC2 + v3dM * pnt3dBeg.getDistance(pnt3dM2) * System.Math.Sin(deltaM2) / lenPI;
            pnt3dM3 = pnt3dC3 + v3dT3 * pnt3dC3.getDistance(pnt3dM3) / pnt3dC3.getDistance(pnt3dT3);

            ObjectId idCgPntM1 = pnt3dM1.setPoint(out pntNum);
            ObjectId idCgPntM2 = pnt3dM2.setPoint(out pntNum);
            ObjectId idCgPntM3 = pnt3dM3.setPoint(out pntNum);
            BaseObjs.updateGraphics();

            idsCgPnt.Add(idCgPntM1);
            idsCgPnt.Add(idCgPntM2);
            idsCgPnt.Add(idCgPntM3);

            List<Point3d> pnts3d = new List<Point3d> { pnt3dBeg, pnt3dM1, pnt3dM2, pnt3dM3, pnt3dEnd };

            return pnts3d;
        }

        public static List<Point3d>
        planeMethod(this Arc arc, Point3d pnt3dBeg, Point3d pnt3dEnd, Point3d pnt3dPI, Vector3d v3dM, ref List<ObjectId> idsCgPnt, string nameCmd)
        {
            List<Point3d> pnts3d = new List<Point3d>();

            double delta = arc.TotalAngle;
            double increDelta = delta / 4;

            double increX = increDelta * arc.Radius;

            Point3d pnt3dM1 = arc.GetPointAtDist(arc.Length * 1 / 4);   //1st quarter delta on arc
            Point3d pnt3dM2 = arc.GetPointAtDist(arc.Length * 2 / 4);   //2nd quarter delta on arc
            Point3d pnt3dM3 = arc.GetPointAtDist(arc.Length * 3 / 4);   //3rd quarter delta on arc

            Vector3d v3dPI = pnt3dPI - pnt3dBeg;        //vector from beg to PI

            Vector3d v3dM1 = pnt3dM1 - pnt3dBeg;        //vector from beg to 1st quarter delta on arc
            Vector3d v3dM2 = pnt3dM2 - pnt3dBeg;        //vector from beg to 2nd quarter delta on arc
            Vector3d v3dM3 = pnt3dM3 - pnt3dBeg;        //vector from beg to 3rd quarter delta on arc

            Vector3d v3dC = pnt3dEnd - pnt3dBeg;        //vector from beg to end of arc - long chord

            double delta0 = v3dPI.getAngle2Vectors(v3dC);       //delta / 2
            double deltaM1 = v3dM1.getAngle2Vectors(v3dC);      //delta from long chord to 1st quarter point
            double deltaM2 = v3dM2.getAngle2Vectors(v3dC);      //delta from long chord to 2nd quarter point
            double deltaM3 = v3dM3.getAngle2Vectors(v3dC);      //delta from long chord to 3rd quarter point

            double lenC0 = pnt3dBeg.getDistance(pnt3dEnd);      //length of long chord
            double lenC1 = pnt3dBeg.getDistance(pnt3dM1) * System.Math.Cos(deltaM1);    //length of chord from beg to 1st quarter delta
            double lenC2 = pnt3dBeg.getDistance(pnt3dM2) * System.Math.Cos(deltaM2);    //length of chord from beg to 2nd quarter delta
            double lenC3 = pnt3dBeg.getDistance(pnt3dM3) * System.Math.Cos(deltaM3);    //length of chord from beg to 3rd quarter delta

            Point3d pnt3dC1 = pnt3dBeg + v3dC * lenC1 / lenC0;      //point on long chord based on ratio of quarter chords vs long chord
            Point3d pnt3dC2 = pnt3dBeg + v3dC * lenC2 / lenC0;
            Point3d pnt3dC3 = pnt3dBeg + v3dC * lenC3 / lenC0;

            double lenPI = pnt3dC2.getDistance(pnt3dPI);      //middle ordinate + external distance

            uint pntNum;
            ObjectId idcgPnt = pnt3dC1.setPoint(out pntNum);
            idcgPnt = pnt3dC2.setPoint(out pntNum);
            idcgPnt = pnt3dC3.setPoint(out pntNum);
            BaseObjs.updateGraphics();

            pnt3dM1 = pnt3dC1 + v3dM * pnt3dBeg.getDistance(pnt3dM1) * System.Math.Sin(deltaM1) / lenPI;
            pnt3dM2 = pnt3dC2 + v3dM * pnt3dBeg.getDistance(pnt3dM2) * System.Math.Sin(deltaM2) / lenPI;
            pnt3dM3 = pnt3dC3 + v3dM * pnt3dBeg.getDistance(pnt3dM3) * System.Math.Sin(deltaM3) / lenPI;

            ObjectId idCgPntM1 = pnt3dM1.setPoint(out pntNum);
            ObjectId idCgPntM2 = pnt3dM2.setPoint(out pntNum);
            ObjectId idCgPntM3 = pnt3dM3.setPoint(out pntNum);
            BaseObjs.updateGraphics();

            idsCgPnt.Add(idCgPntM1);
            idsCgPnt.Add(idCgPntM2);
            idsCgPnt.Add(idCgPntM3);

            Point3d pnt3d = pnt3dBeg;
            pnts3d.Add(pnt3d);

            double m1 = (pnt3dM1.Z - pnt3dBeg.Z) / (arc.Length / 4);
            double m2 = (pnt3dM2.Z - pnt3dM1.Z) / (arc.Length / 4);
            double m3 = (pnt3dM3.Z - pnt3dM2.Z) / (arc.Length / 4);
            double m4 = (pnt3dEnd.Z - pnt3dM3.Z) / (arc.Length / 4);

            double elev = 0.0;
            double sta = 0.0;

            for (int i = 1; i < 5; i++)
            {
                sta += increX;
                pnt3d = arc.GetPointAtDist(sta);

                switch (i)
                {
                    case 1:
                        elev = pnt3dBeg.Z + sta * m1;
                        break;

                    case 2:
                        elev = pnt3dM1.Z + increX * m2;
                        break;

                    case 3:
                        elev = pnt3dM2.Z + increX * m3;
                        break;

                    case 4:
                        elev = pnt3dM3.Z + increX * m4;
                        break;
                }

                pnt3d = new Point3d(pnt3d.X, pnt3d.Y, elev);
                pnts3d.Add(pnt3d);
            }

            return pnts3d;
        }

        public static List<Point3d>
        traverse(this Arc arc, double m1, double m2, double distVPI, double lenVC, Point3d pnt3dBeg)
        {
            List<Point3d> pnts3d = new List<Point3d>();

            double delta = arc.TotalAngle;
            double increDelta = 1 / (4 * pi);
            int numIncre = (int)System.Math.Truncate(delta / increDelta) + 1;
            increDelta = delta / numIncre;

            double increX = increDelta * arc.Radius;

            if (arc.EndPoint.isEqual(pnt3dBeg, 0.1))
            {
                arc.ReverseCurve();
            }

            Point3d pnt3d = pnt3dBeg;
            pnts3d.Add(pnt3d);

            double staBVC = distVPI - lenVC / 2;
            double staEVC = staBVC + lenVC;

            double a = (m2 - m1) / (2 * lenVC);

            double elev0 = pnt3dBeg.Z;
            double elevVPI = elev0 + distVPI * m1;
            double elevBVC = elev0 + staBVC * m1;

            double elev = 0.0;
            double sta = 0.0;

            for (int i = 1; i < numIncre + 1; i++)
            {
                sta += increX;
                pnt3d = arc.GetPointAtDist(sta);

                if (sta <= staBVC)
                {
                    elev = elev0 + sta * m1;
                }
                else if (staBVC < sta && sta <= staEVC)
                {
                    elev = elevBVC + m1 * (sta - staBVC) + a * (sta - staBVC) * (sta - staBVC);
                }
                else
                {
                    elev = elevVPI + (sta - distVPI) * m2;
                }

                pnt3d = new Point3d(pnt3d.X, pnt3d.Y, elev);
                pnts3d.Add(pnt3d);
            }

            return pnts3d;
        }

        public static void
        traverse(this Arc arc, Point3d pnt3dBeg, double grade, double dist, double staBeg, ref List<Point3d> pnts3d)
        {
            double delta = arc.TotalAngle * dist / arc.Length;

            double increDelta = 1 / (4 * pi);
            int numIncre = (int)System.Math.Truncate(delta / increDelta) + 1;
            increDelta = delta / numIncre;

            double increX = increDelta * arc.Radius;

            Point3d pnt3d = pnt3dBeg;
            if (pnts3d.Count == 0)
                pnts3d.Add(pnt3d);

            double elev = 0.0;
            double sta = staBeg;

            for (int i = 1; i < numIncre + 1; i++)
            {
                sta += increX;
                pnt3d = arc.GetPointAtDist(sta);
                elev = pnt3dBeg.Z + (sta - staBeg) * grade;
                pnt3d = new Point3d(pnt3d.X, pnt3d.Y, elev);
                pnts3d.Add(pnt3d);
            }
        }

        public static List<Point3d>
        traverse(this Arc arc, double grade, Point3d pnt3dBeg)
        {
            List<Point3d> pnts3d = new List<Point3d>();
            double delta = arc.TotalAngle;

            double increDelta = 1 / (4 * pi);
            int numIncre = (int)System.Math.Truncate(delta / increDelta) + 1;
            increDelta = delta / numIncre;

            double increX = increDelta * arc.Radius;

            Point3d pnt3d = pnt3dBeg;
            pnts3d.Add(pnt3d);

            double elev = 0.0;
            double sta = 0.0;

            for (int i = 1; i < numIncre + 1; i++)
            {
                sta += increX;
                pnt3d = arc.GetPointAtDist(sta);
                elev = pnt3dBeg.Z + sta * grade;
                pnt3d = new Point3d(pnt3d.X, pnt3d.Y, elev);
                pnts3d.Add(pnt3d);
            }

            return pnts3d;
        }

        public static ObjectId
        getEndPoint(Point3d pnt3d, out string elev, string prompt)
        {
            ObjectId idCgPntEnd = ObjectId.Null;
            elev = UserInput.getCogoPoint(prompt, out idCgPntEnd, pnt3d, osMode: 8);
            return idCgPntEnd;
        }
    }
}
