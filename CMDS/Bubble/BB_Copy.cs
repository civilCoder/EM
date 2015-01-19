using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;

namespace Bubble
{
    public static class BB_Copy
    {
        public static void
        copyBub()
        {
            Editor ed = BaseObjs._editor;
            TypedValue[] tvs = new TypedValue[2];
            tvs.SetValue(new TypedValue((int)DxfCode.Start, "MTEXT"), 0);
            tvs.SetValue(new TypedValue((int)DxfCode.LayerName, "BUBBLE"), 1);
            SelectionFilter filter = new SelectionFilter(tvs);
            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.SingleOnly = true;
            pso.MessageForAdding = "\nSelect Bubble MTEXT: ";
            PromptSelectionResult psr = ed.GetSelection(pso, filter);
            if (psr.Status != PromptStatus.OK)
                return;

            SelectionSet ss = psr.Value;
            ObjectId[] ids = ss.GetObjectIds();

            ObjectId idTX = ids[0];
            if (idTX.getType() != "MText")
            {
                Application.ShowAlertDialog("\nSelected Object is not a MText Entity.  Exiting...");
                return;
            }
            bool escape = true;
            PromptStatus ps = PromptStatus.Cancel;

            Point3d pnt3d = UserInput.getPoint("\nPick Start Point for new leader: ", Pub.pnt3dO, out escape, out ps, osMode: 512);
            if (escape || ps == PromptStatus.None)
            {
                return;
            }

            Point3d pnt3dMTxtIns = idTX.getMTextLocation();

            ResultBuffer rb = idTX.getXData(apps.lnkBubs);
            TypedValue[] tvsTX = rb.AsArray();

            Handle hSM = tvsTX[2].Value.ToString().stringToHandle();
            int scale;
            int.TryParse(tvsTX[3].Value.ToString(), out scale);

            ObjectId idSM = hSM.getObjectId();
            rb = idSM.getXData(apps.lnkBubs);
            TypedValue[] tvsSM = rb.AsArray();

            ObjectIdCollection idsLDR = new ObjectIdCollection();
            ResultBuffer rbLDR0 = new ResultBuffer();

            if (tvsSM.Length > 6)
            {
                for (int i = 6; i < tvsSM.Length; i++)
                {
                    idsLDR.Add(tvsSM[i].Value.ToString().stringToHandle().getObjectId());
                }
                rbLDR0 = idsLDR[0].getXData(apps.lnkBubs);
            }
            else
            {
                return;
            }

            Point3d pnt3dLdr0Beg = idsLDR[0].getBegPnt();
            TypedValue[] tvsLDR0 = rbLDR0.AsArray();
            Handle hTarget = tvsLDR0[3].Value.ToString().stringToHandle();
            string layerTarget = tvsLDR0[4].Value.ToString();

            double angleView = -(double)Application.GetSystemVariable("VIEWTWIST");

            int numSides;
            int.TryParse(tvsSM[5].Value.ToString(), out numSides);


            Vector3d v3d = new Vector3d(pnt3d.X - pnt3dLdr0Beg.X, pnt3d.Y - pnt3dLdr0Beg.Y, pnt3d.Z - pnt3dLdr0Beg.Z);
            ObjectIdCollection idLdrsNew = new ObjectIdCollection();

            foreach (ObjectId id in idsLDR)
            {
                Point3dCollection pnts3dLdr = id.getCoordinates3d();
                Point3dCollection pnts3dLdrNew = new Point3dCollection();

                foreach (Point3d pnt3dx in pnts3dLdr)
                {
                    Point3d pnt3dLdrNew = pnt3dx.TransformBy(Matrix3d.Displacement(v3d));
                    pnts3dLdrNew.Add(pnt3dLdrNew);
                }
                ObjectId idLdr = Draw.addLdr(pnts3dLdrNew, true, null, "BUBBLE", 7);
                
                idLdrsNew.Add(idLdr);
            }

            ObjectId idWO = ObjectId.Null;

            pnt3d = pnt3dMTxtIns.TransformBy(Matrix3d.Displacement(v3d));
            idSM = Draw.addSymbolAndWipeout(pnt3d, angleView, out idWO, Pub.radius, numSides, true);
            idSM.moveToTop();
            idSM.moveBelow(new ObjectIdCollection {
                idWO
            });

            Color color = new Color();
            color = Color.FromColorIndex(ColorMethod.ByLayer, 256);
            string textValue = idTX.getMTextText();
            idTX = Txt.addMText(textValue, pnt3d, angleView, 0.0, 0.09, AttachmentPoint.MiddleCenter, "Annotative", "BUBBLE", color, Pub.JUSTIFYCENTER);

            Draw.addXData(idSM, scale, idTX, idLdrsNew, idWO, numSides, hTarget, layerTarget);
        }
    }
}
