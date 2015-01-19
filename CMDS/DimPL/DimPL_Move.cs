using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using System;
using System.Collections.Generic;
using Math = System.Math;

namespace DimPL
{
    public class DimPL_Move
    {
        public static void
        modTXlnkDimPl(ObjectId idObj, TypedValue[] tvsTX, Point3d pnt3dX)
        {
            double txtSize, sizeRow;

            MText mTxt = (MText)idObj.getEnt();
            int annoScale = Misc.getCurrAnnoScale();

            Double.TryParse(tvsTX[5].Value.ToString(), out txtSize);
            Double.TryParse(tvsTX[6].Value.ToString(), out sizeRow);
            Point3d pnt3d1a = new Point3d((double)tvsTX[7].Value, (double)tvsTX[8].Value, 0.0);
            Point3d pnt3d2a = new Point3d((double)tvsTX[9].Value, (double)tvsTX[10].Value, 0.0);
            Handle hPoly1 = tvsTX[11].Value.ToString().stringToHandle();
            Handle hPoly2 = tvsTX[12].Value.ToString().stringToHandle();
            Handle hEntX = tvsTX[13].Value.ToString().stringToHandle();
            Object xRefPath = tvsTX[14].Value;

            Handle hLine1 = tvsTX[16].Value.ToString().stringToHandle();
            Handle hLine2 = tvsTX[17].Value.ToString().stringToHandle();

            List<Handle> handles = new List<Handle>();
            handles.Add(hEntX);
            handles.Add(hLine1);
            handles.Add(hLine2);

            using (BaseObjs._acadDoc.LockDocument())
            {
                hPoly1.getObjectId().delete();
                hPoly2.getObjectId().delete();

                double dir = pnt3d1a.getDirection(pnt3d2a);
                double distT = pnt3d1a.getDistance(pnt3d2a);

                int maxRows = 0;
                if (txtSize == 0.075)
                    maxRows = DimPL_Global.maxRows075;
                else
                    maxRows = DimPL_Global.maxRows090;

                double txtHeight = Math.Round(txtSize * annoScale, 3);

                DimPL_App.addOrMoveTX(pnt3dX, pnt3d1a, pnt3d2a, txtSize, annoScale, sizeRow, txtHeight, maxRows, dir, distT, handles, mTxt, xRefPath);
            }
        }

        public static void
        moveDimPL(MText mText = null)
        {
            Object osMode = Base_Tools45.SnapMode.getOSnap();
            Base_Tools45.SnapMode.setOSnap(0);

            ObjectId idMtxt = ObjectId.Null;
            TypedValue[] tvsTX = null;

            Editor ed = BaseObjs._editor;
            PromptSelectionResult psr = ed.SelectImplied();
            if (psr.Value == null)
            {
                bool escape = true;
                idMtxt = Select.selectMTextForMoving("TEXT", out escape);
                if (escape)
                    return;
                tvsTX = idMtxt.getXData(apps.lnkDimPL).AsArray();
            }
            else
            {
                ObjectId[] ids = psr.Value.GetObjectIds();
                if (ids[0] is MText)
                {
                    tvsTX = ids[0].getXData(apps.lnkDimPL).AsArray();
                    if (tvsTX == null)
                        return;
                    idMtxt = ids[0];
                }
            }
            PromptStatus ps = PromptStatus.Cancel;
            Point3d pnt3dX = UserInput.getPoint("Pick desired location for text: ", out ps, osMode: 0);

            modTXlnkDimPl(idMtxt, tvsTX, pnt3dX);
            int osM;
            int.TryParse(osMode.ToString(), out osM);
            Base_Tools45.SnapMode.setOSnap(osM);
        }

        public static void
        moveDimPL2(MText mText = null)
        {
            ObjectId idMtxt = ObjectId.Null;
            TypedValue[] tvsTX = null;

            if (mText != null)
            {
                idMtxt = mText.ObjectId;
                tvsTX = idMtxt.getXData(apps.lnkDimPL).AsArray();
            }
            else
            {
                Editor ed = BaseObjs._editor;
                PromptEntityOptions ppo = new PromptEntityOptions("Select Text to Move: ");
                PromptEntityResult per = ed.GetEntity(ppo);

                ObjectId id = per.ObjectId;
                if (id is MText)
                {
                    tvsTX = id.getXData(apps.lnkDimPL).AsArray();
                    if (tvsTX == null)
                        return;
                    idMtxt = id;
                }
            }
            PromptStatus ps = PromptStatus.Cancel;
            Point3d pnt3dX = UserInput.getPoint("Pick desired location for text: ", out ps, osMode: 0);

            modTXlnkDimPl(idMtxt, tvsTX, pnt3dX);
        }
    }
}