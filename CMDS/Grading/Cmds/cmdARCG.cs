using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace Grading.Cmds
{
    public static class cmdARCG
    {
        public static void
        ARCG()
        {
            Arc arc = null;

            Point3d pnt3dBeg = Pub.pnt3dO, pnt3dEnd = Pub.pnt3dO, pnt3dX = Pub.pnt3dO;

            ObjectId idCgPntEnd = ObjectId.Null;
            ObjectId idPoly = ObjectId.Null; 
            List<Point3d> pnts3d = null;

            PromptStatus ps;
            bool escape = true;
            Point3d pnt3dPick;
            string nameLayer;
            FullSubentityPath path = new FullSubentityPath();

            Entity ent = xRef.getNestedEntity("\nSelect ARC: ", out escape, out pnt3dPick, out nameLayer, out path);
            if (escape)
                return;

            if (ent.GetType().ToString() != "Autodesk.AutoCAD.DatabaseServices.Arc")
            {
                Application.ShowAlertDialog(string.Format("Selected object was type of: {0} - Exiting...", ent.GetType().ToString()));
                return;
            }
            else
            {
                arc = (Arc)ent;
            }

            string elev = "", prompt = "Select Cogo Point for begin elevation: ";

            ObjectId idCgPntBeg = cmdABC.getEndPoint(pnt3dBeg, out elev, prompt);
            if (elev == "")
                return;

            List<ObjectId> idsCgPnt = new List<ObjectId>();
            idsCgPnt.Add(idCgPntBeg);

            pnt3dBeg = idCgPntBeg.getCogoPntCoordinates();
            BaseObjs.write(string.Format("\nBegin Elevation = {0:F2}", elev));

            int n = (arc.Normal.Z > 0) ? 1 : -1;                            //right hand direction

            if (arc.EndPoint.isEqual(pnt3dBeg, 0.1))
            {
                arc.ReverseCurve();         //*****************************************************************************
            }

            string cmdOpts = "M";
            prompt = string.Format("\nDistance/End of curve/Single point/Multiple points <{0}> [D/E/S/M]:", cmdOpts);
            escape = UserInput.getUserInputKeyword(cmdOpts, out cmdOpts, prompt, "D E S M");
            if (escape)
                return;

            double dist = 0.0, elv = 0.0, deltaZ = 0.0, grade = 0.0;
            uint pntNum = 0;

            resultsRL resultsRL = new resultsRL();
            ObjectId idCgPntX = ObjectId.Null;

            switch (cmdOpts)
            {
                #region "D"

                case "D":
                    escape = UserInput.getUserInput("\nEnter arc distance from selected begin point: ", out dist);
                    if (escape)
                        return;

                    pnt3dX = arc.GetPointAtDist(dist);

                    cmdRL.getRLpromptresults(out resultsRL, out escape);
                    if (escape)
                        return;
                    switch (resultsRL.opt)
                    {
                        case "R":
                            grade = resultsRL.val;
                            elv = pnt3dBeg.Z + dist * grade;
                            break;

                        case "Z":
                            deltaZ = resultsRL.val;
                            elv = pnt3dBeg.Z + deltaZ;
                            break;
                    }
                    pnt3dX = new Point3d(pnt3dX.X, pnt3dX.Y, elv);
                    idCgPntX = pnt3dX.setPoint(out pntNum);                     //***********   new point   *******************
                    break;

                #endregion "D"

                #region "E"

                case "E":
                    cmdRL.getRLpromptresults(out resultsRL, out escape);
                    if (escape)
                        return;
                    switch (resultsRL.opt)
                    {
                        case "R":
                            grade = resultsRL.val;
                            elv = pnt3dBeg.Z + arc.Length * grade;
                            break;

                        case "Z":
                            deltaZ = resultsRL.val;
                            grade = deltaZ / arc.Length;
                            break;
                    }

                    pnts3d = arc.traverse(grade, pnt3dBeg);

                    idCgPntEnd = pnts3d[pnts3d.Count - 1].setPoint(out pntNum); //***********   new point  *******************
                    idsCgPnt.Add(idCgPntEnd);

                    BrkLine.makeBreakline(apps.lnkBrks, "cmdARCG", out idPoly, idsCgPnt, pnts3dL: pnts3d);

                    break;

                #endregion "E"

                #region "S"

                case "S":
                    pnt3dX = UserInput.getPoint("\nSelect target point on arc: ", out ps, osMode: 512);
                    if (pnt3dX == Pub.pnt3dO)
                        return;

                    dist = arc.GetDistAtPoint(pnt3dX);

                    resultsRL = new resultsRL();
                    cmdRL.getRLpromptresults(out resultsRL, out escape);
                    if (escape)
                        return;
                    switch (resultsRL.opt)
                    {
                        case "R":
                            grade = resultsRL.val;
                            elv = pnt3dBeg.Z + dist * grade;
                            break;

                        case "Z":
                            deltaZ = resultsRL.val;
                            elv = pnt3dBeg.Z + deltaZ;
                            break;
                    }
                    pnt3dX = new Point3d(pnt3dX.X, pnt3dX.Y, elv);
                    idCgPntX = pnt3dX.setPoint(out pntNum);                     //***********   new point  *******************
                    break;

                #endregion "S"

                #region "M"

                case "M":
                    string res = "N";
                    prompt = string.Format("\nSelect Method: <Interval distance/Number of points per segment> <{0}> [I N]: ", res);
                    escape = UserInput.getUserInputKeyword(res, out res, prompt, "I N");
                    int numPnts = 5;
                    switch (res)
                    {
                        case "I":
                            dist = 10;
                            escape = UserInput.getUserInput(string.Format("\nEnter distance between points <{0}>:", dist), out dist, dist, false);
                            if (escape)
                                return;
                            numPnts = (int)System.Math.Truncate(arc.Length / dist);
                            break;

                        case "N":
                            ps = UserInput.getUserInputInt(string.Format("\nNumber of Points <{0}>:", numPnts), false, true, false, false, true, numPnts, out numPnts);
                            if (ps != PromptStatus.OK)
                                return;
                            dist = arc.Length / numPnts;
                            break;
                    }

                    resultsRL = new resultsRL();
                    cmdRL.getRLpromptresults(out resultsRL, out escape);
                    if (escape)
                        return;
                    switch (resultsRL.opt)
                    {
                        case "R":
                            grade = resultsRL.val;
                            break;

                        case "Z":
                            deltaZ = resultsRL.val;
                            grade = deltaZ / arc.Length;
                            break;
                    }

                    pnt3dX = new Point3d(pnt3dX.X, pnt3dX.Y, elv);
                    idCgPntX = pnt3dX.setPoint(out pntNum);                     //***********   new point  *******************
                    double sta = 0.0;

                    for (int i = 1; i < numPnts; i++)
                    {
                        sta = i * dist;
                        if (sta <= (arc.Length - dist / 2))
                        {
                            pnt3dX = arc.GetPointAtDist(sta);
                            pnt3dX = new Point3d(pnt3dX.X, pnt3dX.Y, pnt3dBeg.Z + sta * grade);
                            idCgPntX = pnt3dX.setPoint(out pntNum);
                            idsCgPnt.Add(idCgPntX);
                        }
                    }

                    elv = pnt3dBeg.Z + arc.Length * grade;
                    pnt3dX = new Point3d(pnt3dX.X, pnt3dX.Y, elv);
                    idCgPntX = pnt3dX.setPoint(out pntNum);
                    idsCgPnt.Add(idCgPntX);                             //***********   new point - end point  *******************

                    break;

                #endregion "M"
            }
        }
    }
}
