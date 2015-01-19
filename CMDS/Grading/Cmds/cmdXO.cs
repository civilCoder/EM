using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Grading.Cmds
{
    public static class cmdXO
    {
        public static void
        XO()
        {
            object objOffset = Application.GetSystemVariable("OFFSETDIST");
            Point3d pnt3dBeg = Pub.pnt3dO;
            Point3d pnt3dEnd = Pub.pnt3dO;
            Point3d pnt3dCen, pnt3dX;
            string defPrompt;
            if (objOffset.ToString() == "-1")
            {
                defPrompt = "Through";
            }
            else
            {
                defPrompt = objOffset.ToString();
            }

            double disX = 0.0, offset = 0.0, radius = 0.0;

            bool through = false;
            string result, prompt = "\nSpecify Offset distance or [Through]: ";
            UserInput.getUserInput(defPrompt, prompt, out result, false);
            if (result == string.Empty)
                return;

            Point3d pnt3dPick = Pub.pnt3dO;
            string nameLayer = string.Empty;
            ObjectId idBlkRef = ObjectId.Null;
            ObjectId idEnt = xRef.getEntity("\nSelect object to offset or <exit>: ", true, out nameLayer, out pnt3dPick, out idBlkRef);
            if (idEnt == ObjectId.Null)
                return;

            PromptStatus ps = PromptStatus.Cancel;
            if (result.ToUpper() == "THROUGH" || result.ToUpper() == "T")
            {
                through = true;
                pnt3dX = UserInput.getPoint("\nSpecify Offset point : ", out ps, osMode: 0);
            }
            else
            {
                offset = double.Parse(result);
                Application.SetSystemVariable("OFFSETDIST", offset);
                if (offset == 0)
                {
                    idEnt.copy("0");
                    return;
                }
                else
                {
                    pnt3dX = UserInput.getPoint("\nSpecify point on side to offset: ", out ps, osMode: 0);
                }
            }
            bool rh = true;

            string typeEnt = idEnt.getType();
            switch (typeEnt)
            {
                case "Polyline":
                    double bulge = 0.0;
                    try
                    {
                        using (Transaction tr = BaseObjs.startTransactionDb())
                        {
                            Polyline poly = (Polyline)tr.GetObject(idEnt, OpenMode.ForRead);
                            if (poly.Closed)
                                idEnt.checkIfClosed();
                            int vtx = Geom.getVertexNo(idEnt, pnt3dX);
                            pnt3dBeg = poly.GetPoint3dAt(vtx);
                            pnt3dEnd = poly.GetPoint3dAt(vtx + 1);
                            bulge = poly.GetBulgeAt(vtx);
                            tr.Commit();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(ex.Message + " cmdXO.cs: line: 86");
                    }
                    if (bulge == 0.0)
                    {
                        processLine(pnt3dBeg, pnt3dEnd, pnt3dX, through, disX, ref offset);
                    }
                    else
                    {
                        if (bulge < 0)
                            rh = false;
                        pnt3dCen = Geom.getCenter(pnt3dBeg, pnt3dEnd, bulge);

                        if (!processArc(pnt3dBeg, pnt3dEnd, pnt3dCen, pnt3dX, through, radius, rh, idEnt, disX, ref offset))
                        {
                            return;
                        }
                    }
                    break;

                case "Line":
                    Line line = (Line)idEnt.getEnt();
                    pnt3dBeg = line.StartPoint;
                    pnt3dEnd = line.EndPoint;
                    processLine(pnt3dBeg, pnt3dEnd, pnt3dX, through, disX, ref offset);
                    break;

                case "Arc":
                    Arc arc = (Arc)idEnt.getEnt();
                    pnt3dBeg = arc.StartPoint;
                    pnt3dEnd = arc.EndPoint;
                    pnt3dCen = arc.Center;
                    radius = arc.Radius;
                    Vector3d v3d = arc.Normal;
                    if (v3d.Z < 0)
                    {
                        rh = false;
                    }

                    if (!processArc(pnt3dBeg, pnt3dEnd, pnt3dCen, pnt3dX, through, radius, rh, idEnt, disX, ref offset))
                    {
                        return;
                    }
                    break;
            }
            idEnt.offset(offset);
            idEnt.delete();
        }

        private static bool
        processArc(Point3d pnt3dBeg, Point3d pnt3dEnd, Point3d pnt3dCen, Point3d pnt3dX,
            bool through, double radius, bool rh, ObjectId idEnt, double disX, ref double offset)
        {
            disX = pnt3dCen.getDistance(pnt3dX);

            double dirX = pnt3dCen.getDirection(pnt3dX);
            double dirB = pnt3dCen.getDirection(pnt3dBeg);
            double dirE = pnt3dCen.getDirection(pnt3dEnd);

            if (through)
            {
                offset = System.Math.Abs(disX - radius);
            }

            bool go = false;

            if (rh)
            {
                if (dirB > dirE)
                {
                    double adj = 2 * System.Math.PI - dirB;
                    if (dirX + adj >= 0 && dirX + adj <= dirE + adj)
                    {
                        go = true;
                        if (disX < radius)
                            offset = -offset;
                    }
                }
                else
                {
                    if (dirX >= dirB && dirX <= dirE)
                    {
                        go = true;
                        if (disX < radius)
                            offset = -offset;
                    }
                }
            }
            else if (!rh)
            {
                if (dirB < dirE)
                {
                    double adj = 2 * System.Math.PI - dirE;
                    if (dirX + adj <= dirB + adj && dirX + adj <= 0)
                    {
                        go = true;
                        if (disX > radius)
                            offset = -offset;
                    }
                }
                else
                {
                    if (dirX >= dirE && dirX <= dirB)
                    {
                        go = true;
                        if (disX > radius)
                            offset = -offset;
                    }
                }
            }
            if (!go)
            {
                idEnt.delete();
                Application.ShowAlertDialog("Point selected is outside limits of procedure.  Exiting...");
            }
            return go;
        }

        private static void
        processLine(Point3d pnt3dBeg, Point3d pnt3dEnd, Point3d pnt3dX, bool through, double disX, ref double offset)
        {
            double dirL = pnt3dBeg.getDirection(pnt3dEnd);
            bool rs = false;
            if (Geom.testRight(pnt3dBeg, pnt3dEnd, pnt3dX) < 0)
            {
                rs = true;
            }
            if (through)
            {
                disX = pnt3dBeg.getDistance(pnt3dX);
                double x = Geom.getCosineComponent(pnt3dEnd, pnt3dBeg, pnt3dX);
                offset = System.Math.Sqrt(System.Math.Pow(disX, 2) - System.Math.Pow(x, 2));
            }
            if (!rs)
                offset = offset * -1;
        }
    }
}
