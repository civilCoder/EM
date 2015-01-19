using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;
using sMath = System.Math;

namespace Grading.Cmds
{
    public static class cmdRDR
    {
        public static double pi = System.Math.PI;

        public static void
        RDR()
        {
            string prompt, result;
            bool escape = false, exists = false, done = false, done2 = false;

            double trianglationFactor = 0.001;      //applied to treads to avoid vertical faces0

            double lenTread, lenLand, grdLandU, wthRiser;

            List<riserSum> rSum = new List<riserSum>();

            ResultBuffer rb;
            TypedValue[] tvs;
            Dict.deleteDictionary("RDR");
            ObjectId idDict = Dict.getNamedDictionary("RDR", out exists);
            if (exists)
            {
                rb = Dict.getXRec(idDict, "Defaults");
                tvs = rb.AsArray();
                lenTread = double.Parse(tvs[0].Value.ToString());
                lenLand = double.Parse(tvs[1].Value.ToString());
                grdLandU = double.Parse(tvs[2].Value.ToString());
                wthRiser = double.Parse(tvs[4].Value.ToString());
            }
            else
            {
                lenTread = 11.0;
                lenLand = 5.5;
                grdLandU = -0.01;
                wthRiser = 5.5;
            }

            do
            {
                prompt = string.Format("\nLength riser tread(LR): {0}; Width of Riser(W): {1}; Length upper landing(LL): {2}; Grade upper Landing(G): {3}; No Change <OK> [LR/W/LL/G/OK]", lenTread, wthRiser, lenLand, grdLandU);
                escape = UserInput.getUserInputKeyword("OK", out result, prompt, "LR W LL G OK");
                if (escape)
                    return;
                switch (result)
                {
                    case "LR":
                        UserInput.getUserInput("Enter Riser trEAD LENGTH in Inches ", out lenTread, lenTread);
                        break;

                    case "LL":
                        UserInput.getUserInput("Enter LENGTH of UPPER LANDING", out lenLand, lenLand);
                        break;

                    case "G":
                        UserInput.getUserInput("Enter SLOPE of UPPER Landing from HIGH POINT towards LOW POINT: ", out grdLandU, grdLandU);
                        break;

                    case "W":
                        UserInput.getUserInput("Enter RISER WIDTH: ", out wthRiser, wthRiser);
                        break;

                    case "OK":
                        done = true;
                        break;
                }
            }
            while (!done);

            tvs = new TypedValue[5];
            tvs.SetValue(new TypedValue(1000, lenTread), 0);       //length of riser tread in inches
            tvs.SetValue(new TypedValue(1000, lenLand), 1);       //length of upper landing - perp to bldg
            tvs.SetValue(new TypedValue(1000, grdLandU), 2);       //grade of upper landing
            tvs.SetValue(new TypedValue(1000, wthRiser), 4);       //width of riser tread in inches
            Dict.addXRec(idDict, "Defaults", new ResultBuffer(tvs));

            object osMode = SnapMode.getOSnap();
            SnapMode.setOSnap((int)osModes.NON);
            TinSurface surf = Surf.getTinSurface("CPNT-ON", out exists);
            Point3d pnt3dPick;
            PromptStatus ps;
            Entity ent = Select.selectEntity(typeof(Polyline3d), "\nSelect Building Floor Slab Breakline", "Selected entity was not a 3d Breakline", out pnt3dPick, out ps);
            if (ps != PromptStatus.OK)
                return;

            Polyline3d poly3dFF = (Polyline3d)ent;      //Finish Floor breakline

            ent = Select.selectEntity(typeof(Polyline3d), "\nSelect Dock Apron Breakline adjacent to Building", "Selected entity was not a 3d Breakline", out pnt3dPick, out ps);
            if (ps != PromptStatus.OK)
                return;

            Polyline3d poly3dDK = (Polyline3d)ent;      //Dock brkline

            List<Point3d> pnts3dDK = poly3dDK.ObjectId.getCoordinates3dList();

            SnapMode.setOSnap((int)osModes.END);

            Point3d pnt3dR0 = UserInput.getPnt3d(Pub.pnt3dO, "\nSelect Riser connect point: upper RH corner of upper Landing: ", osMode: 1);
            Point3d pnt3dL0 = UserInput.getPnt3d(Pub.pnt3dO, "\nSelect Riser connect point: upper LH corner of upper Landing: ", osMode: 1);
            Point3d pnt3dR, pnt3dL;

            List<Point3d> pnts3dFF = poly3dFF.ObjectId.getCoordinates3dList();
            List<Point3d> pnts3dSeg = new List<Point3d>();

            int x = 0;
            if (pnts3dFF.Count > 2)
            {
                x = Geom.getVertexNo(pnts3dFF, pnt3dR0);
                pnts3dSeg.Add(pnts3dFF[x]);
                pnts3dSeg.Add(pnts3dFF[x + 1]);
            }
            else
            {
                pnts3dSeg = pnts3dFF;
            }
            double dirSegFF = pnts3dSeg[0].getDirection(pnts3dSeg[1]);
            double dirRiser = pnt3dR0.getDirection(pnt3dL0);
            double dirPick = 0.0;

            double testRight = Geom.testRight(pnts3dSeg[0], pnts3dSeg[1], pnt3dR0);
            double angle0 = 0.0;
            Point3d pnt3dTar = Pub.pnt3dO;
            if (testRight > 0)
            {
                pnt3dTar = pnt3dR0.traverse(dirSegFF - pi / 2, 10.0);
                angle0 = dirSegFF + pi / 2;
            }
            else
            {
                pnt3dTar = pnt3dR0.traverse(dirSegFF + pi / 2, 10.0);
                angle0 = dirSegFF - pi / 2;
            }

            List<Point3d> pnts3dRay = new List<Point3d> {
                pnt3dR0,
                pnt3dTar
            };
            List<Point3d> pnts3dInt = pnts3dRay.intersectWith(pnts3dSeg, false, 0);
            Point3d pnt3dRef = Pub.pnt3dO;
            if (pnts3dInt.Count > 0)
                pnt3dRef = pnts3dInt[0];

            double slope = (pnts3dSeg[1].Z - pnts3dSeg[0].Z) / pnts3dSeg[0].getDistance(pnts3dSeg[1]);
            double dirDiff = System.Math.Round(dirSegFF - dirRiser, 3);
            double slopeRiser = 0.0;
            if (dirDiff == 0)
                slopeRiser = slope;
            else
                slopeRiser = -slope;

            double dist0 = pnts3dSeg[0].getDistance(pnt3dRef);
            double distTest = Geom.getPerpDistToLine(pnts3dSeg[0], pnts3dSeg[1], pnt3dR0);

            Vector3d v3d = new Vector3d(sMath.Cos(dirSegFF), sMath.Sin(dirSegFF), slope);

            Point3d pnt3dRBase = pnt3dR0;
            Point3d pnt3dRff, pnt3dLff;
            ObjectId idCgPnt = ObjectId.Null;
            ObjectId idPoly3d = ObjectId.Null;
            uint pntNum = 0;
            int numRisers = 1;
            double distBetween = 0.0;

            bool single = false;

            prompt = "\nMode <Single/Multiple> [S/M]";
            result = string.Empty;
            escape = UserInput.getUserInputKeyword("S", out result, prompt, "S M");
            if (escape)
                return;
            switch (result)
            {
                case "S":
                    numRisers = 1;
                    single = true;
                    break;

                case "M":
                    prompt = "\nUnEven or Even Spacing [U/E]";
                    escape = UserInput.getUserInputKeyword("U", out result, prompt, "U E");
                    if (escape)
                        return;
                    switch (result)
                    {
                        case "E":
                            escape = UserInput.getUserInput("Enter # of risers to calculate: ", out result);
                            if (escape)
                                return;
                            numRisers = int.Parse(result.ToString());
                            escape = UserInput.getUserInput("Enter Spacing: ", out result);
                            if (escape)
                                return;
                            distBetween = double.Parse(result.ToString());
                            Point3d pnt3dEnd = UserInput.getPoint("\nSelect upper RH corner of landing at next riser in direction of target risers:", out ps, osMode: 1);
                            if (pnt3dEnd == Pub.pnt3dO)
                                return;
                            double distX = pnt3dR0.getDistance(pnt3dEnd);

                            double diffX = sMath.Round(sMath.Abs(distX - distBetween), 3);
                            if (diffX > 0)
                            {
                                Application.ShowAlertDialog("Distance between picked points and Spacing does not agree.  Exiting...");
                                return;
                            }
                            dirPick = pnt3dR0.getDirection(pnt3dEnd);
                            break;

                        case "U":
                            numRisers = 1;
                            distBetween = 0.0;
                            dirPick = 0.0;
                            break;
                    }
                    break;
            }

            int k = 0;
            do
            {
                done2 = single;

                for (int p = 0; p < numRisers; p++)
                {
                    double dist0X = 0;

                    if (distBetween != 0)
                    { //multiple risers  -  even spacing
                        pnt3dR0 = pnt3dRBase + v3d * p * distBetween;
                        dist0X = dist0 + (p * distBetween);
                        pnt3dRff = pnts3dSeg[0] + v3d * dist0X;
                        if (p == numRisers - 1)
                            done2 = true;
                    }
                    else if (!single)
                    { //multiple risers  -  uneven spacing
                        if (k == 0)
                        { //
                            pnt3dRff = pnts3dSeg[0].traverse(dirSegFF, dist0, slope);
                            dist0X = dist0;
                            k++;
                        }
                        else
                        {
                            pnt3dR0 = UserInput.getPoint("Select upper RH corner of landing at next riser: ", out ps, osMode: 8);
                            if (pnt3dR0 == Pub.pnt3dO)
                            {
                                done2 = true;
                                break;
                            }
                            dist0X = Geom.getPerpDistToLine(pnts3dSeg[0], pnts3dSeg[1], pnt3dR0);
                            pnt3dRff = pnts3dSeg[0].traverse(dirSegFF, dist0X, slope);
                        }
                    }
                    else
                    {
                        pnt3dRff = pnts3dSeg[0].traverse(dirSegFF, dist0, slope);     //single riser
                        dist0X = dist0;
                    }

                    idCgPnt = pnt3dRff.setPoint(out pntNum, "CPNT-ON");

                    pnt3dLff = pnt3dRff.traverse(dirRiser, wthRiser, slopeRiser);
                    idCgPnt = pnt3dLff.setPoint(out pntNum, "CPNT-ON");

                    double elev0 = pnts3dSeg[0].Z + slope * dist0X;

                    pnt3dR0 = new Point3d(pnt3dR0.X, pnt3dR0.Y, elev0 - 0.04);              //point at upper right side of upper landing
                    idPoly3d = Draw.addPoly3d(pnt3dRff, pnt3dR0, "CPNT-BRKLINE");

                    pnt3dL0 = pnt3dR0.traverse(dirRiser, wthRiser, slopeRiser);              //point at upper left side of upper landing
                    idPoly3d = Draw.addPoly3d(pnt3dLff, pnt3dL0, "CPNT-BRKLINE");
                    Point3dCollection pnts3dR = new Point3dCollection();
                    pnts3dR.Add(pnt3dR0);
                    idCgPnt = pnt3dR0.setPoint(out pntNum, "CPNT-ON");
                    addRiserCallout(idCgPnt, angle0 + pi, -1);

                    Point3dCollection pnts3dL = new Point3dCollection();
                    pnts3dL.Add(pnt3dL0);
                    idCgPnt = pnt3dL0.setPoint(out pntNum, "CPNT-ON");
                    addRiserCallout(idCgPnt, angle0 + pi, 1);

                    pnt3dR = pnt3dR0.traverse(angle0, lenLand - trianglationFactor, grdLandU);     //point at lower right side of upper landing
                    pnts3dR.Add(pnt3dR);
                    idCgPnt = pnt3dR.setPoint(out pntNum, "CPNT-ON");
                    addRiserCallout(idCgPnt, angle0, 1);

                    Point3d pnt3dBase = pnt3dR.traverse(angle0, trianglationFactor);

                    pnt3dL = pnt3dR.traverse(dirRiser, wthRiser, slopeRiser);                       //point at lower left side of upper landing
                    pnts3dL.Add(pnt3dL);
                    idCgPnt = pnt3dL.setPoint(out pntNum, "CPNT-ON");
                    addRiserCallout(idCgPnt, angle0, -1);

                    idPoly3d = Draw.addPoly3d(pnt3dR, pnt3dL, "CPNT-BRKLINE");

                    Dictionary<double, double> res = new Dictionary<double, double>();
                    done = false;
                    double dZ = 0.0, adj = 0.0, distX = 0.0, steps = 0.0, rem = 0.0, hgtRiser = 0.0;
                    int stepX = 0;
                    double elevSurface = 0.0;
                    double i = 7.0;
                    do
                    {
                        pnt3dR = new Point3d(pnt3dBase.X, pnt3dBase.Y, pnt3dBase.Z - i / 12.0);    //bottom of first step for height - Base was adjusted for triangulationFactor
                        elevSurface = surf.FindElevationAtXY(pnt3dR.X, pnt3dR.Y);
                        dZ = pnt3dR.Z - elevSurface;
                        if (dZ < i / 12)
                            break;

                        Vector3d v3dx = new Vector3d(System.Math.Cos(angle0), System.Math.Sin(angle0), -i / lenTread);
                        Point3d pnt3dX = surf.GetIntersectionPoint(pnt3dR, v3dx);

                        distX = pnt3dR.getDistance(pnt3dX);
                        steps = distX / (lenTread / 12);
                        rem = System.Math.Truncate(steps) + 1 - steps;
                        res.Add(i, rem);
                        if (rem < 0.05)
                        {
                            stepX = (int)System.Math.Truncate(steps) + 1;
                            adj = rem * i / 12;
                            hgtRiser = i;
                            if (rem * i < stepX * 0.0625)
                            {
                                hgtRiser = i - 0.0625;
                                adj = adj - stepX * 0.0625 / 12;
                            }
                            break;
                        }
                        if (i == 4)
                            done = true;
                        i = i - 0.0625;
                    }
                    while (!done);


                    string dimV = Conv.decimalToFraction(hgtRiser);
                    string topTxt = string.Format("({0}) {1}", stepX, dimV);
                    rSum.Add(new riserSum { idPoly3d = idPoly3d, angle = angle0, lenLanding = lenLand, topText = topTxt });

                    pnts3dR.Add(pnt3dR);
                    pnt3dL = pnt3dR.traverse(dirRiser, wthRiser, slopeRiser);
                    pnts3dL.Add(pnt3dL);

                    for (int n = 1; n < steps; n++)
                    {
                        pnt3dR = pnt3dR.traverse(angle0, lenTread / 12 - trianglationFactor);       //adjusted to avoid vertical faces
                        pnts3dR.Add(pnt3dR);

                        pnt3dL = pnt3dR.traverse(dirRiser, wthRiser, slopeRiser);
                        pnts3dL.Add(pnt3dL);

                        idPoly3d = Draw.addPoly3d(pnt3dR, pnt3dL, "CPNT-BRKLINE");

                        pnt3dR = pnt3dR.traverse(angle0, trianglationFactor);                       //get back to correct location
                        pnt3dR = new Point3d(pnt3dR.X, pnt3dR.Y, pnt3dR.Z - hgtRiser / 12);
                        pnts3dR.Add(pnt3dR);

                        pnt3dL = pnt3dR.traverse(dirRiser, wthRiser, slopeRiser);
                        pnts3dL.Add(pnt3dL);

                        idPoly3d = Draw.addPoly3d(pnt3dR, pnt3dL, "CPNT-BRKLINE");
                    }

                    pnt3dR = pnt3dR.traverse(angle0, lenTread / 12 - 0.005);  //shorten width of last tread to avoid vertical faces
                    pnts3dR.Add(pnt3dR);

                    pnt3dL = pnt3dR.traverse(dirRiser, wthRiser, slopeRiser);
                    pnts3dL.Add(pnt3dL);

                    idPoly3d = Draw.addPoly3d(pnt3dR, pnt3dL, "CPNT-BRKLINE");

                    pnt3dR = pnt3dR.traverse(angle0, 0.005);                //makeup for reduced width of tread
                    pnt3dR = new Point3d(pnt3dR.X, pnt3dR.Y, pnt3dR.Z - (hgtRiser / 12 - adj));
                    pnts3dR.Add(pnt3dR);
                    idCgPnt = pnt3dR.setPoint(out pntNum, "CPNT-ON");
                    addRiserCallout(idCgPnt, angle0, 1);

                    pnt3dL = pnt3dR.traverse(dirRiser, wthRiser, slopeRiser);
                    pnts3dL.Add(pnt3dL);
                    idCgPnt = pnt3dL.setPoint(out pntNum, "CPNT-ON");
                    addRiserCallout(idCgPnt, angle0, -1);


                    pnt3dR = pnt3dR.traverse(angle0 + pi / 2, 0.5, slope);           //point at toe of stairs / outside face of sidewall
                    idCgPnt = pnt3dR.setPoint(out pntNum, "CPNT-ON");

                    pnt3dL = pnt3dL.traverse(angle0 - pi / 2, 0.5, slopeRiser);         //point at toe of stairs / outside face of sidewall
                    idCgPnt = pnt3dL.setPoint(out pntNum, "CPNT-ON");

                    List<Point3d> pnts3dSegDK = new List<Point3d>();

                    if (pnts3dDK.Count > 2)
                    {
                        x = Geom.getVertexNo(pnts3dDK, pnt3dR);
                        pnts3dSegDK.Add(pnts3dDK[x]);
                        pnts3dSegDK.Add(pnts3dDK[x + 1]);
                    }
                    else
                    {
                        pnts3dSegDK = pnts3dDK;
                    }

                    double dirSegDK = pnts3dSegDK[0].getDirection(pnts3dSegDK[1]);
                    double slopeDK = pnts3dSegDK[0].getSlope(pnts3dSegDK[1]);
                    double lenR = Geom.getPerpDistToLine(pnts3dSegDK[0], pnts3dSegDK[1], pnt3dR);

                    Point3d pnt3dRint = pnts3dSegDK[0].traverse(dirSegDK, lenR, slopeDK);
                    idCgPnt = pnt3dRint.setPoint(out pntNum, "CPNT-ON");
                    addRiserCallout(idCgPnt, angle0, 1);

                    double lenL = Geom.getPerpDistToLine(pnts3dSegDK[0], pnts3dSegDK[1], pnt3dL);
                    Point3d pnt3dLint = pnts3dSegDK[0].traverse(dirSegDK, lenL, slopeDK);
                    idCgPnt = pnt3dLint.setPoint(out pntNum, "CPNT-ON");
                    addRiserCallout(idCgPnt, angle0, -1);

                    if (lenL > lenR)
                    {
                        pnts3dDK.Insert(x + 1, pnt3dRint);
                        pnts3dDK.Insert(x + 2, pnt3dR);
                        pnts3dDK.Insert(x + 3, pnt3dL);
                        pnts3dDK.Insert(x + 4, pnt3dLint);
                    }
                    else
                    {
                        pnts3dDK.Insert(x + 1, pnt3dLint);
                        pnts3dDK.Insert(x + 2, pnt3dL);
                        pnts3dDK.Insert(x + 3, pnt3dR);
                        pnts3dDK.Insert(x + 4, pnt3dRint);
                    }

                    ObjectId idPoly3dR = Draw.addPoly3d(pnts3dR, "CPNT-BRKLINE");
                    ObjectId idPoly3dL = Draw.addPoly3d(pnts3dL, "CPNT-BRKLINE");

                    tvs = new TypedValue[4];
                    tvs.SetValue(new TypedValue(1001, apps.lnkRiser), 0);
                    tvs.SetValue(new TypedValue(1000, "cmdRDR"), 1);
                    tvs.SetValue(new TypedValue(1000, "R"), 2);
                    tvs.SetValue(new TypedValue(1005, idPoly3dL.getHandle()), 3);
                    idPoly3dR.setXData(tvs, apps.lnkRiser);

                    tvs = new TypedValue[4];
                    tvs.SetValue(new TypedValue(1001, apps.lnkRiser), 0);
                    tvs.SetValue(new TypedValue(1000, "cmdRDR"), 1);
                    tvs.SetValue(new TypedValue(1000, "L"), 2);
                    tvs.SetValue(new TypedValue(1005, idPoly3dR.getHandle()), 3);
                    idPoly3dL.setXData(tvs, apps.lnkRiser);
                }
            }
            while (!done2);

            foreach (riserSum r in rSum)
            {
                Point3d pnt3dMid = r.idPoly3d.getBegPnt().getMidPoint2d(r.idPoly3d.getEndPnt());
                addRiserSummaryCallout(pnt3dMid, r.angle, r.lenLanding, r.topText);
            }

            Point3dCollection pnts3dDKc = new Point3dCollection();
            foreach (Point3d pnt3d in pnts3dDK)
                pnts3dDKc.Add(pnt3d);

            ObjectId idPoly3dDK = Draw.addPoly3d(pnts3dDKc, "CPNT-BRKLINE");
            poly3dDK.ObjectId.delete();

            SnapMode.setOSnap((int)osMode);
        }

        public static void
        addRiserCallout(ObjectId idCgPnt, double angle0, int side)
        {
            List<ObjectId> idsCgPnts = new List<ObjectId> {
                idCgPnt
            };

            Point3d pnt3dV0 = idCgPnt.getCogoPntCoordinates();
            Point3d pnt3dV1 = pnt3dV0.traverse(angle0 + pi / 4 * side, 4);
            Point3d pnt3dV2 = pnt3dV1.traverse(angle0 + pi / 2 * side, 10);

            Point3dCollection pnts3dLdr = new Point3dCollection {
                pnt3dV0,
                pnt3dV1,
                pnt3dV2
            };
            ObjectId idLayer = Layer.manageLayers("ARROW");
            double scaleAnno = Misc.getCurrAnnoScale();

            ObjectId idLDR = Ldr.addLdr(pnts3dLdr, idLayer, 0.09, 0.0, clr.byl, ObjectId.Null);

            string resBot = "FS";
            string resTop = string.Format(pnt3dV0.Z.ToString("F2"));
            Txt.addLdrText("cmdFL", apps.lnkCO, idLDR, idsCgPnts, resTop, resBot, useBackgroundFill: true);
        }

        public static void
        addRiserSummaryCallout(Point3d pnt3dV0, double angle0, double lenLand, string topTxt, string botTxt = "RISERS")
        {
            Point3d pnt3dV1 = pnt3dV0.traverse(angle0 + pi, lenLand + 2);
            Point3d pnt3dV2 = pnt3dV1.traverse(angle0 + pi, 10);

            Point3dCollection pnts3dLdr = new Point3dCollection {
                pnt3dV0,
                pnt3dV1,
                pnt3dV2
            };

            ObjectId idLayer = Layer.manageLayers("ARROW");
            double scaleAnno = Misc.getCurrAnnoScale();
            double angleView = double.Parse(Application.GetSystemVariable("VIEWTWIST").ToString());

            ObjectId idLDR = Ldr.addLdr(pnts3dLdr, idLayer, 0.09, 0.0, clr.byl, ObjectId.Null);

            ObjectId idWO = ObjectId.Null;

            Txt.addLdrText("cmdRDR", apps.lnkCO, idLDR, null, topTxt, botTxt, useBackgroundFill: true);

            Point3d pnt3dIns = idLDR.getEndPnt();

            pnt3dIns = pnt3dIns.traverse(angle0 + pi, Pub.radius * scaleAnno);

            ObjectId idSym = Draw.addSymbolAndWipeout(pnt3dIns, angleView, out idWO, Pub.radius, 1024, true);
            Color color = new Color();
            color = Color.FromColorIndex(ColorMethod.ByLayer, 256);
            ObjectId idMTxt = Txt.addMText("XX", pnt3dIns, angleView, 0.0, 0.09, AttachmentPoint.MiddleCenter, "Annotative", "BUBBLE", color, Pub.JUSTIFYCENTER);
            idSym.putOnTop(idMTxt, idWO);
        }

        public static void
        MR()
        {
        }

        private struct riserSum
        {
            public ObjectId idPoly3d;
            public double angle;
            public double lenLanding;
            public string topText;
        }
    }
}
