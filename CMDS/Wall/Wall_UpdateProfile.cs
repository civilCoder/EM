using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;


namespace Wall
{
    public static class Wall_UpdateProfile
    {
        private static Color color;

        public static void
        updateProfile(ObjectId idAlign, List<POI> varPOI, string strProfileName, bool boolAdpnt3dNum, string strFunction)
        {
            Alignment objAlign = (Alignment)idAlign.getEnt();
            string nameLayer = objAlign.Layer;

            ResultBuffer rb = idAlign.getXData("WALLDESIGN");
            if (rb == null)
                return;
            TypedValue[] tvs = rb.AsArray();

            ObjectId idProfileView = tvs.getObjectId(1);
            ProfileView objProfileView = (ProfileView)idProfileView.getEnt();

            int shtScale = Misc.getCurrAnnoScale();

            Extents3d ext3d = (Extents3d)objProfileView.Bounds;
            Point3d pnt3dMin = ext3d.MinPoint;
            Point3d pnt3dMax = ext3d.MaxPoint;

            double dblStaStart = objProfileView.StationStart;
            double dblElevMin = objProfileView.ElevationMin;
            double dblElevMax = objProfileView.ElevationMax;
            double dblScaleProfileView = (pnt3dMax.Y - pnt3dMin.Y) / (dblElevMax - dblElevMin);
            double staX = 0;

            Profile objProfile = Prof.getProfile(idAlign, strProfileName);

            for (int i = 0; i < varPOI.Count; i++)
            {
                POI vPOI = varPOI[i];

                if (varPOI[i].Elevation == 0)
                {
                    try
                    {
                        vPOI.Elevation = objProfile.ElevationAt(varPOI[i].Station);
                    }
                    catch (Exception)
                    {
                        if (i == 0)
                            staX = vPOI.Station + 0.01;
                        if (i == varPOI.Count - 1)
                            staX = vPOI.Station - 0.01;

                        vPOI.Elevation = objProfile.ElevationAt(staX);
                    }
                }

                varPOI[i] = vPOI;

                double dblCenX = pnt3dMin.X + (varPOI[i].Station - dblStaStart);
                double dblCenY = pnt3dMax.Y + (varPOI[i].Elevation - dblElevMin) * dblScaleProfileView;

                string strDesc = varPOI[i].Desc0;

                Point3d dblPntCen = new Point3d(dblCenX, dblCenY, 0);
                color = new Color();
                color = Color.FromColorIndex(ColorMethod.ByBlock, 4);

                if (strDesc.Contains("PNT"))
                {
                    ObjectId idCircle = Draw.addCircle(dblPntCen, 0.5);
                    idCircle.changeProp(color, nameLayer);
                }

                if (boolAdpnt3dNum)
                {
                    if ((varPOI[i].PntNum != null))
                    {
                        if (!addLeader(dblPntCen, varPOI[i].PntNum, nameLayer, strProfileName, shtScale, dblScaleProfileView,  varPOI[i].Desc0))
                        {
                            return;
                        }
                    }
                }
            }
        }

        public static bool
        addLeader(Point3d varPntIns, string varPntNum, string strLayer, string strProfile, int shtScale, double dblScaleProfileView, object varDesc = null)
        {
            Point3dCollection pnts3dLdr = new Point3dCollection();
            Point3d pnt3d = Pub.pnt3dO;

            pnts3dLdr.Add(varPntIns);
            pnt3d = new Point3d(varPntIns.X + 4, varPntIns.Y - 4, 0);
            pnts3dLdr.Add(pnt3d);

            string strDesc = varPntNum;

            int intStrLen = strDesc.Length;
            ObjectId idTxtStyle = ObjectId.Null;

            try
            {
                idTxtStyle = Txt.getTextStyleId("Annotative");
            }
            catch (Exception)
            {
                idTxtStyle = Txt.getTextStyleId("Standard");
            }

            ObjectId idDimStyle = ObjectId.Null;

            try
            {
                idDimStyle = Dim.getDimStyleTableRecord("Annotative");
            }
            catch (Exception)
            {
                idDimStyle = Dim.getDimStyleTableRecord("Standard");
            }

            short intColor = 256;
            if (strProfile == "EXIST")
            {
                intColor = 1;
            }
            else{
                intColor = 4;
            }
            Wall_Form.frmWall2 fWall2 = Wall_Forms.wForms.fWall2;
            MText mTxt = null;
            ObjectId idMtext = ObjectId.Null;
            try
            {
                idMtext = Txt.addMText(strDesc, pnt3d, 0, 0);

                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    mTxt = (MText)tr.GetObject(idMtext, OpenMode.ForWrite);
                    mTxt.Attachment = AttachmentPoint.BottomLeft;
                    mTxt.Layer = strLayer;
                    mTxt.Color = Color.FromColorIndex(ColorMethod.ByBlock, (short)intColor);
                    mTxt.TextStyleId = idTxtStyle;

                    MText objLabel = fWall2.LABEL;
                    List<Point3d> varPntsInt = mTxt.intersectWith(objLabel, false, extend.none);

                    if (varPntsInt.Count > 0)
                    {
                        Point3d pnt3dX = new Point3d(pnt3d.X - 0.35 * dblScaleProfileView, pnt3d.Y, 0);
                        mTxt.Location = pnt3dX;
                    }
                    fWall2.LABEL = mTxt;

                    tr.Commit();
                }
            }
            catch (Exception)
            {
            }

            ObjectId idLayer = Layer.manageLayers(strLayer);
            ObjectId idLeader = Ldr.addLdr(pnts3dLdr, idLayer, 0.09, 0, clr.byl, idMtext);
            Leader ldr = null;
            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                ldr = (Leader)tr.GetObject(idLeader, OpenMode.ForWrite);
                ldr.Dimasz = 0.09;
                ldr.HasArrowHead = true;
                ldr.Dimgap = 0.01;
                ldr.DimensionStyle = idDimStyle;

                tr.Commit();
            }

            TypedValue[] tvs = new TypedValue[] { new TypedValue(1001, "LEADER"), new TypedValue(1005, ldr.Handle) };
            idMtext.setXData(tvs, "LEADER");
            return true;
        }
        
    }
}
