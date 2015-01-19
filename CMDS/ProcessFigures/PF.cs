using Autodesk.AECC.Interop.Survey;
using Autodesk.AECC.Interop.UiSurvey;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

using Base_Tools45;
using Base_Tools45.C3D;
using Base_VB;

using System.Collections.Generic;
using System.IO;

namespace ProcessFigures
{
    public static class PF
    {
        private const string TemplateCONT = @"R:\TSet\Template\CIVIL3D2010\Thienes_CONT.dwt";
        private const string TemplateTOPO = @"R:\TSet\Template\CIVIL3D2010\Thienes_TOPO_20.dwt";

        public static ObjectId
        buildPoly3dFromNodes(List<Node3d> nodes3d)
        {
            Point3dCollection pnts3d = new Point3dCollection();
            Point3d pnt3dPrev = Pub.pnt3dO;

            for (int i = 0; i < nodes3d.Count; i++)
            {
                if (nodes3d[i].Bulge == 0)
                {
                    Point3d pnt3d = new Point3d(nodes3d[i].X, nodes3d[i].Y, nodes3d[i].Z);
                    pnts3d.Add(pnt3d);
                    pnt3dPrev = pnt3d;
                }
                else
                {
                    Point3d pnt3dBeg = new Point3d(nodes3d[i + 0].X, nodes3d[i + 0].Y, nodes3d[i + 0].Z);
                    Point3d pnt3dEnd = new Point3d(nodes3d[i + 1].X, nodes3d[i + 1].Y, nodes3d[i + 1].Z);

                    pnts3d.Add(pnt3dBeg);
                    pnt3dPrev = pnt3dBeg;

                    double delta = 4 * System.Math.Atan(nodes3d[i + 0].Bulge);
                    double lenChord = pnt3dBeg.getDistance(pnt3dEnd);
                    double radius = System.Math.Abs((lenChord / 2) / System.Math.Sin(delta / 2));
                    double azChord = Measure.getAzRadians(pnt3dBeg, pnt3dEnd);
                    double azTan = azChord - delta / 2;
                    double lenArc = System.Math.Abs(radius * delta);

                    int intervalCrv = 0;
                    if (lenArc <= 5.0)
                    {
                        intervalCrv = (int)System.Math.Truncate(lenArc) * 2 + 1;
                    }
                    else if (lenArc <= 10.0)
                    {
                        intervalCrv = (int)System.Math.Truncate(lenArc) + 1;
                    }
                    else if (lenArc <= 20.0)
                    {
                        intervalCrv = (int)System.Math.Truncate(lenArc / 2) + 1;
                    }
                    else if (lenArc <= 50.0)
                    {
                        intervalCrv = (int)System.Math.Truncate(lenArc / 3) + 1;
                    }
                    else if (lenArc <= 100.0)
                    {
                        intervalCrv = (int)System.Math.Truncate(lenArc / 4) + 1;
                    }
                    else if (lenArc <= 200.0)
                    {
                        intervalCrv = (int)System.Math.Truncate(lenArc / 5) + 1;
                    }
                    else
                    {
                        intervalCrv = (int)System.Math.Truncate(lenArc / 6) + 1;
                    }

                    double incrDelta = delta / intervalCrv;
                    double slope = (pnt3dEnd.Z - pnt3dBeg.Z) / lenArc;

                    for (int y = 0; y < intervalCrv; y++)
                    {
                        double incrChordAz = azTan + (y * incrDelta / 2);
                        double incrChordLen = (2 * radius * System.Math.Sin(System.Math.Abs(incrDelta * y / 2)));
                        Point3d pnt3dPolar = Geom.traverse_pnt3d(pnt3dBeg, incrChordAz, incrChordLen);
                        double elev = pnt3dPrev.Z + slope * (intervalCrv * y) / lenArc;
                        pnt3dPrev = new Point3d(pnt3dPolar.X, pnt3dPolar.Y, elev);          //current point
                        pnts3d.Add(pnt3dPrev);
                    }
                }
            }

            return pnts3d.addPoly3d();
        }

        public static void
        doProcessFigures()
        {
            bool exists = false;
            ObjectId idDictHist = Dict.getNamedDictionary("HISTORY", out exists);
            if (exists)
            {
                bool def = true;
                bool answer = false;
                PromptStatus ps = UserInput.getUserInputYesNo("Command PF has been executed on this drawing - Continue?", def, out answer);
                if (ps == PromptStatus.OK)
                {
                    if (answer == false)
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            if (BaseObjs._acadDocs.Count > 4)
            {
                bool def = false;
                bool answer = false;
                PromptStatus ps = UserInput.getUserInputYesNo(string.Format("There are {0} drawings open on this machine.  Do you wish to continue?", BaseObjs._acadDocs.Count.ToString()), def, out answer);
                if (ps == PromptStatus.OK)
                {
                    if (answer == false)
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            string JN = string.Empty;
            string TOP = string.Empty;
            string nameFile = string.Empty;
            string nameFileMod = string.Empty;
            ResultBuffer rb = null;

            ObjectId idDictPPF2 = Dict.getNamedDictionary("PPF2", out exists);
            if (!exists)
            {
                Application.ShowAlertDialog("Dictionary PPF2 is missing - Breaklines will be named \'TEMP\'");

                string nameFull = BaseObjs.docFullName;
                JN = BaseObjs.docName.Substring(0, 4);
                TOP = "TEMP";
            }
            else
            {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    rb = Dict.getXRec(idDictPPF2, "strTOP");
                }

                TypedValue[] tvs = rb.AsArray();

                if (tvs.Length > 0)
                {
                    JN = tvs[0].Value.ToString();
                    TOP = tvs[1].Value.ToString();
                    nameFile = tvs[2].Value.ToString();
                    nameFileMod = tvs[3].Value.ToString();
                }
            }

            AeccSurveyDocument survDoc = BaseObjsCom.aeccSurvDoc;
            AeccSurveyProjects survProjs = survDoc.Projects;
            AeccSurveyProject survProj = null;

            try
            {
                foreach (AeccSurveyProject sProj in survProjs)
                    if (sProj.Name == JN)
                    {
                        survProj = sProj;
                        survProj.Open();
                        break;
                    }
            }
            catch (System.Exception ex)
            {
                //Application.ShowAlertDialog(ex.Message + " PF.cs: line: 146");
                BaseObjs.writeDebug(ex.Message + " PF.cs: line: 146");
            }

            AeccSurveyFigures survFigures = survProj.Figures;
            List<ObjectId> idPolys3d = new List<ObjectId>();
            List<Node3d> nodes3d = new List<Node3d>();
            foreach (AeccSurveyFigure figure in survFigures)
            {
                AeccSurveyFigureNodes nodes = figure.FigureNodes;

                foreach (AeccSurveyFigureNode node in nodes)
                {
                    Node3d node3d = new Node3d(node.X, node.Y, node.Z, node.Bulge);
                    nodes3d.Add(node3d);
                }

                ObjectId idPoly3d = buildPoly3dFromNodes(nodes3d);
                Layer.setLayer(idPoly3d, figure.Layer);
                idPolys3d.Add(idPoly3d);
            }

            survProj.Close();

            TransferObjs.transferObjects(idPolys3d, TemplateCONT, nameFileMod);
            TransferObjs.transferObjects(idPolys3d, TemplateTOPO, nameFile);

            string handleLastEnt = string.Empty;
            try
            {
                handleLastEnt = idPolys3d[idPolys3d.Count - 1].getHandle().ToString();
            }
            catch (System.Exception ex)
            {
                //Application.ShowAlertDialog(ex.Message + " PF.cs: line: 176");
                BaseObjs.writeDebug(ex.Message + " PF.cs: line: 176");
            }

            CgPnt_Group.deletePntGroup(Path.GetFileName(nameFileMod));

            rb = new ResultBuffer(new TypedValue(1005, handleLastEnt));

            Dict.addXRec(idDictHist, "lastENT", rb);
        }
    }
}