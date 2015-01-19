using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using Stake.Forms;
using System;
using System.Collections.Generic;

namespace Stake
{
    public static class Stake_UpdateProfile
    {
        private static Forms.frmStake fStake = Stake_Forms.sForms.fStake;
        private static double PI = System.Math.PI;
        private static Color color;

        public static void
        updateProfile(ObjectId idAlign, List<POI> varPOI, string strProfileName, bool boolAdpnt3dNum, string strFunction)
        {
            int intColor = 0;
            Alignment objAlign = (Alignment)idAlign.getEnt();
            string nameLayer = objAlign.Layer;

            ProfileView objProfileView = null;
            TypedValue[] tvs;
            if (strFunction != "WALLDESIGN")
            {
                ResultBuffer rb = idAlign.getXData("PROFILE");
                if (rb == null)
                    return;
                tvs = rb.AsArray();
                ObjectId idProfileView = tvs.getObjectId(1);

                objProfileView = (ProfileView)idProfileView.getEnt();
            }
            else
            {
                ResultBuffer rb = idAlign.getXData("WALLDESIGN");
                if (rb == null)
                    return;
                tvs = rb.AsArray();
                ObjectId idProfileView = tvs.getObjectId(1);
                objProfileView = (ProfileView)idProfileView.getEnt();
            }

            int shtScale = Misc.getCurrAnnoScale();

            if (strProfileName == "STAKE")
            {
                intColor = 2;
            }
            else
            {
                intColor = 4;
            }

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

                string strDesc = "";
                if (strProfileName == "STAKE")
                {
                    strDesc = varPOI[i].DescX;
                    if (strDesc == "")
                    {
                        strDesc = varPOI[i].Desc0;
                    }
                }
                else if (strFunction == "WALLDESIGN")
                {
                    strDesc = varPOI[i].Desc0;
                }
                else
                {
                    strDesc = varPOI[i].Desc0;
                }

                if (strDesc.Contains("HE"))
                {
                    string note = string.Format("{0} {1:###+00.00}", "???", varPOI[i].Station);
                    addTriangle(dblCenX, dblCenY, nameLayer, note);
                }

                Point3d dblPntCen = new Point3d(dblCenX, dblCenY, 0);
                color = new Color();
                color = Color.FromColorIndex(ColorMethod.ByBlock, (short)intColor);

                if (strDesc.Contains("HC"))
                {
                    ObjectId idCircle = Draw.addCircle(dblPntCen, 1.0);
                    idCircle.changeProp(color, nameLayer);
                }

                if (strDesc.Contains("GB"))
                {
                    List<Vertex2d> dblPnts = new List<Vertex2d>{
                        new Vertex2d(new Point3d(dblCenX - 0.5, dblCenY - 0.5, 0), 0, 0, 0, 0),
                        new Vertex2d(new Point3d(dblCenX + 1,   dblCenY - 0.5, 0), 0, 0, 0, 0),
                        new Vertex2d(new Point3d(dblCenX + 1,   dblCenY + 0.5, 0), 0, 0, 0, 0),
                        new Vertex2d(new Point3d(dblCenX + 0,   dblCenY - 0.5, 0), 0, 0, 0, 0),
                        new Vertex2d(new Point3d(dblCenX + 0,   dblCenY - 0.0, 0), 0, 0, 0, 0),
                    };

                    ObjectId idBox = Draw.addPoly(dblPnts);
                    idBox.changeProp(color, nameLayer, LineWeight.LineWeight035);
                }

                if (strDesc.Contains("PNT"))
                {
                    ObjectId idCircle = Draw.addCircle(dblPntCen, 0.5);
                    idCircle.changeProp(color, nameLayer);
                }

                if (varPOI[i].CrossAlign != "")
                {
                    string note = string.Format("{0} {1:###+00.00}", varPOI[i].CrossAlign, varPOI[i].Station);
                    addTriangle(dblCenX, dblCenY, nameLayer, note);
                }
                else if (varPOI[i].CrossDesc != "")
                {
                    string note = string.Format("{0} {1:###+00.00}", varPOI[i].CrossDesc, varPOI[i].Station);
                    addTriangle(dblCenX, dblCenY, nameLayer, note);
                }
                else if (varPOI[i].Type != "")
                {
                    string note = string.Format("{0} {1:###+00.00}", varPOI[i].Desc0, varPOI[i].Station);
                    addTriangle(dblCenX, dblCenY, nameLayer, note);
                }

                if (boolAdpnt3dNum)
                {
                    if ((varPOI[i].PntNum != null))
                    {
                        if (!addLeader(dblPntCen, varPOI[i].PntNum, nameLayer, strProfileName, shtScale, dblScaleProfileView, varPOI[i].Desc0))
                        {
                            return;
                        }
                    }
                }
            }

            if (strFunction == "STAKED")
            {
                Dim.addDims(pnt3dMin, dblStaStart, dblElevMin, varPOI, nameLayer, dblScaleProfileView);
            }
        }

        private static void addTriangle(double dblCenX, double dblCenY, string nameLayer, string note)
        {
            List<Vertex2d> dblPnts = new List<Vertex2d> {
                new Vertex2d(new Point3d(dblCenX, dblCenY, 0), 0, 0, 0, 0),
                new Vertex2d(new Point3d(dblCenX + 4.0 * System.Math.Cos(PI / 3), dblCenY + 4.0 * System.Math.Sin(PI / 3), 0), 0, 0, 0, 0),
                new Vertex2d(new Point3d(dblCenX - 4.0 * System.Math.Cos(PI / 3), dblCenY + 4.0 * System.Math.Sin(PI / 3), 0), 0, 0, 0, 0),
                new Vertex2d(new Point3d(dblCenX, dblCenY, 0), 0, 0, 0, 0)
            };

            color = new Color();
            color = Color.FromColorIndex(ColorMethod.ByBlock, 5);

            ObjectId idTriangle = Draw.addPoly(dblPnts);
            idTriangle.changeProp(color, nameLayer, LineWeight.LineWeight035);

            Point3d pnt3dIns = new Point3d(dblCenX - 4.0 * System.Math.Cos(PI / 3), dblCenY - 4.0 * System.Math.Sin(PI / 3) - 1.5, 0);

            ObjectId idText = Txt.addText(note, pnt3dIns, 0, AttachmentPoint.TopLeft);
            idText.changeProp(color, nameLayer);
        }


        public static bool
        addLeader(Point3d varPntIns, string varPntNum, string strLayer, string strProfile, int shtScale, double dblScaleProfileView, object varDesc = null)
        {
            Point3dCollection pnts3dLdr = new Point3dCollection();
            Point3d pnt3d = Pub.pnt3dO;

            if (strProfile == "STAKE")
            {
                pnts3dLdr.Add(varPntIns);
                pnt3d = new Point3d(varPntIns.X + 3, varPntIns.Y + 3, 0);
                pnts3dLdr.Add(pnt3d);
            }
            else
            {
                pnts3dLdr.Add(varPntIns);
                pnt3d = new Point3d(varPntIns.X + 4, varPntIns.Y - 4, 0);
                pnts3dLdr.Add(pnt3d);
            }

            string strDesc = varPntNum;

            if ((varDesc != null))
            {
                strDesc = strDesc + " " + varDesc.ToString();
            }

            int intStrLen = strDesc.Length;
            ObjectId idTxtStyle = ObjectId.Null;

            try
            {
                idTxtStyle = Txt.getTextStyleId("Annotative");
            }
            catch (Exception )
            {
                idTxtStyle = Txt.getTextStyleId("Standard");
            }

            ObjectId idDimStyle = ObjectId.Null;

            try
            {
                idDimStyle = Dim.getDimStyleTableRecord("Annotative");
            }
            catch (Exception )
            {
                idDimStyle = Dim.getDimStyleTableRecord("Standard");
            }

            short intColor = 256;
            if (strProfile == "EXIST")
            {
                intColor = 1;
            }
            else if (strProfile == "CPNT")
            {
                intColor = 4;
            }
            else
            {
                intColor = 2;
            }
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

                    MText objLabel = fStake.LABEL;
                    Point3dCollection varPntsInt = new Point3dCollection();
                    mTxt.IntersectWith(objLabel, Intersect.OnBothOperands, varPntsInt, IntPtr.Zero, IntPtr.Zero);

                    if (varPntsInt.Count > 0)
                    {
                        Point3d pnt3dX = new Point3d(pnt3d.X - 0.35 * dblScaleProfileView, pnt3d.Y, 0);
                        mTxt.Location = pnt3dX;
                    }
                    fStake.LABEL = mTxt;

                    tr.Commit();
                }
            }
            catch (Exception )
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