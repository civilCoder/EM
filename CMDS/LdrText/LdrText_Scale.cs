using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using System.Collections.Generic;

namespace LdrText
{
    public class LdrText_Scale
    {
        private const double pi = System.Math.PI;

        public static void
        scaleLdrText(int scaleCurr)
        {
            bool exists = false;
            ObjectId idDict = Dict.getNamedDictionary(apps.lnkLD, out exists);
            List<DBDictionaryEntry> entries = idDict.getDictEntries();
            if (entries.Count == 0)
                return;
            try
            {
                foreach (DBDictionaryEntry entry in entries)
                {
                    string h = entry.Key;
                    ObjectId idTxTop = h.stringToHandle().getObjectId();
                    if (!idTxTop.IsValid)
                        continue;

                    ResultBuffer rb = idTxTop.getXData(apps.lnkLD);
                    TypedValue[] tvsTxTop = rb.AsArray();

                    int scalePrior;

                    Handle hLdr = tvsTxTop[2].Value.ToString().stringToHandle();
                    int.TryParse(tvsTxTop[3].Value.ToString(), out scalePrior);
                    Handle hTxBot = tvsTxTop[4].Value.ToString().stringToHandle();
                    double txtX, txtY;
                    double.TryParse(tvsTxTop[5].Value.ToString(), out txtX);
                    double.TryParse(tvsTxTop[6].Value.ToString(), out txtY);
                    double mTxtDir = idTxTop.getMTextRotation();

                    ObjectId idTxBot = ObjectId.Null;
                    try
                    {
                        idTxBot = hTxBot.getObjectId();
                    }
                    catch (System.Exception ex)
                    {
                BaseObjs.writeDebug(ex.Message + " LdrText_Scale.cs: line: 49");
                    }

                    ObjectId idLdr = hLdr.getObjectId();
                    TypedValue[] tvsLdr = idLdr.getXData(apps.lnkLD).AsArray();

                    double X, Y;
                    Point3dCollection pnts3dLdrPrior = new Point3dCollection();
                    for (int i = 4; i < tvsLdr.Length; i = i + 2)
                    {
                        double.TryParse(tvsLdr[i + 0].Value.ToString(), out X);
                        double.TryParse(tvsLdr[i + 1].Value.ToString(), out Y);
                        pnts3dLdrPrior.Add(new Point3d(X, Y, 0));
                    }
                    int n = pnts3dLdrPrior.Count;

                    List<double> dirs = new List<double>();
                    List<double> lens = new List<double>();

                    double dir, len;
                    for (int j = 1; j < n; j++)
                    {
                        dir = pnts3dLdrPrior[j - 1].getDirection(pnts3dLdrPrior[j]);
                        len = pnts3dLdrPrior[j - 1].getDistance(pnts3dLdrPrior[j]);
                        dirs.Add(dir);
                        lens.Add(len);
                    }

                    mTxtDir = dirs[dirs.Count - 1];
                    bool left_justify = Base_Tools45.Math.left_Justify(mTxtDir);

                    AttachmentPoint apTop;
                    AttachmentPoint apBot;
                    string justify = string.Empty;

                    if (left_justify)
                    {
                        apTop = AttachmentPoint.BottomLeft;
                        apBot = AttachmentPoint.TopLeft;
                        mTxtDir = mTxtDir + pi;
                        justify = Pub.JUSTIFYLEFT;
                    }
                    else
                    {
                        apTop = AttachmentPoint.BottomRight;
                        apBot = AttachmentPoint.TopRight;
                        justify = Pub.JUSTIFYRIGHT;
                    }

                    double scale = Misc.getCurrAnnoScale();
                    double scaleFactor = scale / scalePrior;

                    Point3dCollection pnts3dLdr = new Point3dCollection();
                    Point3d pnt3d = pnts3dLdrPrior[0];
                    pnts3dLdr.Add(pnt3d);
                    for (int i = 0; i < dirs.Count; i++)
                    {
                        pnt3d = pnt3d.traverse(dirs[i], lens[i] * scaleFactor);
                        pnts3dLdr.Add(pnt3d);
                    }

                    for (int i = 1; i < pnts3dLdr.Count; i++)
                    {
                        idLdr.updateVertex(i, pnts3dLdr[i]);
                    }

                    Point3d pnt3dEndOrg = pnts3dLdrPrior[n - 1];
                    Point3d pnt3dEnd = idLdr.getLastVertex();

                    Point3d pnt3dInsTop = new Point3d(txtX, txtY, 0);
                    dir = pnt3dEndOrg.getDirection(pnt3dInsTop);
                    double offset = pnt3dEndOrg.getDistance(pnt3dInsTop);
                    Point3d pnt3dInsNew = pnt3dEnd.traverse(dir, offset * scaleFactor);
                    idTxTop.adjMTextPositionAndDirection(pnt3dInsNew, mTxtDir);
                    idTxTop.setMTextAttachPointAndJustify(apTop, justify);

                    tvsTxTop[3] = new TypedValue(1070, scale);
                    tvsTxTop[5] = new TypedValue(1040, pnt3dInsNew.X);
                    tvsTxTop[6] = new TypedValue(1040, pnt3dInsNew.Y);

                    idTxTop.setXData(tvsTxTop, apps.lnkLD);

                    if (idTxBot != null)
                    {
                        pnt3dInsNew = pnt3dEnd.traverse(dir + pi, offset * scaleFactor);
                        idTxBot.adjMTextPositionAndDirection(pnt3dInsNew, mTxtDir);
                        idTxBot.setMTextAttachPointAndJustify(apBot, justify);
                    }

                    for (int i = 0; i < pnts3dLdr.Count; i++)
                    {
                        tvsLdr[i * 2 + 4] = new TypedValue(1040, pnts3dLdr[i].X);
                        tvsLdr[i * 2 + 5] = new TypedValue(1040, pnts3dLdr[i].Y);
                    }

                    idLdr.setXData(tvsLdr, apps.lnkLD);
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " LdrText_Scale.cs: line: 149");
            }
        }
    }
}
