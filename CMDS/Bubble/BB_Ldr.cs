using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using Base_Tools45.Jig;
using System;
using System.Collections.Generic;

namespace Bubble
{
    public static class BB_Ldr
    {
        public static void
        addLdr()
        {
            Point3d pnt3d = Pub.pnt3dO;
            bool canLdr = false;
            Handle hTarget;
            ObjectId idSM = ObjectId.Null;
            string layerTarget = "";
            FullSubentityPath path = new FullSubentityPath();
            List<FullSubentityPath> paths = new List<FullSubentityPath>();

            Entity ent = Ldr.getFirstLdrPoint(out pnt3d, out canLdr, out hTarget, out layerTarget, out path);
            if (canLdr)
                return;
            ent.PushHighlight(path, Autodesk.AutoCAD.GraphicsInterface.HighlightStyle.Glow);
            Color color = Misc.getColorByLayer();
            ObjectId idLDR = JigSplineLeader_BB.jigSplineLeader(pnt3d, 0.09, "BUBBLE", color);
            if (idLDR == ObjectId.Null)
                return;

            Point3d pnt3dEnd = idLDR.getLastVertex();
            List<Type> types = new List<Type> {
                typeof(Polyline),
                typeof(Circle)
            };
            ObjectId[] ids = Select.getEntityatPoint(pnt3dEnd, types, "BUBBLE");

            if (ids == null)
                return;

            foreach (ObjectId id in ids)
            {
                ResultBuffer rb = id.getXData(apps.lnkBubs);
                if (rb != null)
                {
                    idSM = id;
                    Ldr.setLdrXData(pnt3dEnd, idLDR, idSM);
                    if (!updateSMxData(idSM, idLDR))
                        Application.ShowAlertDialog("Leader linkage to Symbol failed.");
                    break;
                }
            }
            paths.Add(path);
            xRef.unHighlightNestedEntity(paths);
        }

        public static void
        adjustLdrEndPoint(Point3d pnt3dX, Point3d pnt3dCEN, ObjectId idSM)
        {
        }

        public static void
        moveLdrEndPoint(TypedValue[] tvsSM, Point3d pnt3dCEN, Point3d pnt3dIns, ObjectId idSM)
        {
            for (int i = 6; i < tvsSM.Length; i++)
            {
                ObjectId idLDR = tvsSM.getObjectId(i);
                Point3d pnt3dBase = idLDR.getLastVertex();
                Point3d pnt3dEnd = idLDR.moveLdrEndPoint(pnt3dCEN, pnt3dIns, pnt3dBase);

                Ldr.setLdrXData(pnt3dEnd, idLDR, idSM);
            }
        }

        public static void
        scaleLDRs(Point3d pnt3dCEN, double scaleFactor, ObjectId idSM)
        {
            TypedValue[] tvsSM = idSM.getTVsAsArray(apps.lnkBubs);
            for (int i = 6; i < tvsSM.Length; i++)
            {
                ObjectId idLDR = tvsSM.getObjectId(i);
                ResultBuffer rbLDR = idLDR.getXData(apps.lnkBubsLdrEndPnt);
                TypedValue[] tvsLDR = rbLDR.AsArray();

                Point3d pnt3dOrg = new Point3d((double)tvsLDR[1].Value, (double)tvsLDR[2].Value, 0.0);
                Point3d pnt3dEnd = idLDR.adjLdrEndPnt(pnt3dCEN, Pub.pnt3dO, scaleFactor);

                Ldr.setLdrXData(pnt3dEnd, idLDR, idSM);
            }
        }

        public static bool
        updateSMxData(ObjectId idSM, ObjectId idLDR)
        {
            bool success = true;
            TypedValue[] tvsSM = idSM.getXData(apps.lnkBubs).AsArray();
            int k = tvsSM.Length;

            TypedValue[] tvsSMnew = new TypedValue[k + 1];
            tvsSM.CopyTo(tvsSMnew, 0);
            tvsSMnew.SetValue(new TypedValue(1005, idLDR.getHandle()), k);

            try
            {
                idSM.setXData(tvsSMnew, apps.lnkBubs);
                success = true;
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " BB_Ldr.cs: line: 113");
            }
            return success;
        }
    }
}
