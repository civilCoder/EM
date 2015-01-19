using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Linq;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace MNP
{
    public static class MNP_Align
    {
        private static frmAlignEnts fAlignEnts = MNP_Forms.fAlignEnts;
        private static frmMNP fMNP = MNP_Forms.fMNP;

        public static void
        labelAlign()
        {       // not done
            ObjectIdCollection idsAlign = Align.getAlignmentIDs();
            ProfilePVICollection pvis;
            foreach (ObjectId id in idsAlign)
            {
                Alignment align = (Alignment)id.getEnt();
                foreach (ObjectId idP in align.GetProfileIds())
                {
                    Profile p = (Profile)idP.getEnt();
                    if (p.Name == "CPNT-ON")
                    {
                        pvis = p.PVIs;
                        break;
                    }
                }
            }
        }

        public static string
        makeAlign(string nameCmd, string nameLayer, string nameStyle, string nameStyleLabel, bool addCurves = false)
        {
            string nameAlign = "";
            using (BaseObjs._acadDoc.LockDocument())
            {
                List<Point3d> pnts3dList = new List<Point3d>();
                bool success = Misc.getPline3dList(out pnts3dList, "\nPick alignment start point: ", "\nSelect next point");
                if (!success)
                    return "";

                ObjectId idPoly = Draw.addPoly(pnts3dList);
                Alignment align = Align.addAlignmentFromPoly(nameLayer, nameLayer, idPoly, nameStyle, nameStyleLabel, addCurves);
                nameAlign = align.Name;
                fMNP.idAlign = align.ObjectId;
            }

            return nameAlign;
        }

        public static void
        editAlign(ObjectId idAlign)
        {
            Point3d pnt3dPick = Pub.pnt3dO;
            if (idAlign == ObjectId.Null)
            {
                Entity ent = Select.selectEntity(typeof(Alignment), "Select Alignment: ", "Selected object was not an alignment. Try again: ", out pnt3dPick);
                idAlign = ent.ObjectId;
            }

            Alignment align = (Alignment)idAlign.getEnt();

            List<AlgnEntData> algnData = sortAlignEnts(align);

            fAlignEnts.updateForm(algnData);
            fAlignEnts.AlignHandle = align.Handle;
            Application.ShowModelessDialog(Application.MainWindow.Handle, fAlignEnts, false);
        }

        private static List<AlgnEntData>
        getAlignEntData(Alignment align)
        {
            List<AlgnEntData> algnData = new List<AlgnEntData>();
            AlignmentEntityCollection ents = align.Entities;
            foreach (AlignmentEntity ent in ents)
            {
                AlgnEntData aData = new AlgnEntData();
                if (ent.EntityType == AlignmentEntityType.Arc)
                {
                    AlignmentArc arc = (AlignmentArc)ent;
                    aData.ID = arc.EntityId;
                    aData.Type = "Arc";
                    aData.StaBeg = arc.StartStation;
                    aData.StaEnd = arc.EndStation;
                    aData.Length = arc.Length;
                    aData.Radius = arc.Radius;
                    try
                    {
                        aData.EntBefore = arc.EntityBefore;
                    }
                    catch
                    {
                        aData.EntBefore = 0;
                    }
                }
                else if (ent.EntityType == AlignmentEntityType.Line)
                {
                    AlignmentLine line = (AlignmentLine)ent;
                    aData.ID = line.EntityId;
                    aData.Type = "Line";
                    aData.StaBeg = line.StartStation;
                    aData.StaEnd = line.EndStation;
                    aData.Length = line.Length;
                    aData.Radius = 0;
                    try
                    {
                        aData.EntBefore = line.EntityBefore;
                    }
                    catch
                    {
                        aData.EntBefore = 0;
                    }
                }
                algnData.Add(aData);
            }
            return algnData;
        }

        public static List<AlgnEntData>
        sortAlignEnts(Alignment align)
        {
            List<AlgnEntData> algnEntData = getAlignEntData(align);
            List<AlgnEntData> algnEntDataSorted = new List<AlgnEntData>();

            var sortEnt = from e in algnEntData
                          orderby e.EntBefore ascending
                          select e;

            foreach (var e in sortEnt)
                algnEntDataSorted.Add(e);

            return algnEntData;
        }

        public static void
        insertCurve(ObjectId idAlign)
        {
            Alignment align = (Alignment)idAlign.getEnt();
            AlignmentEntityCollection ents = align.Entities;
            PromptStatus ps;
            Point3d pnt3dPick = UserInput.getPoint("Select Curve PI", out ps, osMode: 1);

            AlignmentEntity lineBefore = null;
            AlignmentEntity lineAfter = null;

            for (int i = 0; i < ents.Count; i++)
            {
                AlignmentEntity ent = ents[i];
                if (ent.EntityType == AlignmentEntityType.Line)
                {
                    AlignmentLine line = (AlignmentLine)ent;
                    if (line.EndPoint.IsEqualTo(pnt3dPick.Convert2d(BaseObjs.xyPlane), new Tolerance(.01, .01)))
                    {
                        lineBefore = line;
                    }
                    if (line.StartPoint.IsEqualTo(pnt3dPick.Convert2d(BaseObjs.xyPlane), new Tolerance(.01, .01)))
                    {
                        lineAfter = line;
                    }
                }
            }

            AlignmentArc arc = ents.AddFreeCurve(lineBefore.EntityId, lineAfter.EntityId, 200.0, CurveParamType.Radius, false, CurveType.Compound);
            List<AlgnEntData> algnEntData = sortAlignEnts(align);

            fAlignEnts.updateForm(algnEntData);
            fAlignEnts.AlignHandle = align.Handle;
            Application.ShowModelessDialog(Application.MainWindow.Handle, fAlignEnts, false);
        }
    }
}