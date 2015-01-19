using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace Base_Tools45
{
    public static partial class Txt
    {
        private const double pi = System.Math.PI;

        private static List<string> cmdsC0 = new List<string> {
            "cmdCF0",
            "cmdFF",
            "cmdFFD",
            "cmdFL",
            "cmdFLX",
            "cmdG",
            "cmdGX",
            "cmdLD",
            "cmdPT",
            "cmdSDE",
            "cmdSDS",
            "cmdSED",
            "cmdMH",
            "cmdVB"
        };

        private static List<string> cmdsGS = new List<string> {
            "cmdGS",
            "cmdGS0",
            "cmdGS2",
            "cmdGS3",
            "cmdGSE",
            "cmdGSS",
            "cmdGSX",
            "cmdSS",
            "cmdSL"
        };

        public static void
        addLdrText(string nameCmd, string nameApp, ObjectId idLdr, List<ObjectId> idsCgPnts, string top = "", string bot = "", string bot2 = "",
            double deltaZ = 0.0, double xFactor = 0.8, bool useBackgroundFill = false, List<Point3d> pnts3dX = null)
        {
            string resultTop = top.ToUpper();
            string resultBot = bot.ToUpper();
            string resultBot2 = bot2.ToUpper();

            Point3d pnt3dTop = Pub.pnt3dO;
            Point3d pnt3dBot = Pub.pnt3dO;

            Point3d pnt3dLdrMid = Pub.pnt3dO;
            Point3d pnt3dLdrIns = Pub.pnt3dO;       //text insertion point on Ldr before offset
            Point3d pnt3dLdrEnd = Pub.pnt3dO;

            Point3d pnt3d1 = Pub.pnt3dO;
            Point3d pnt3d2 = Pub.pnt3dO;

            ObjectId idTxTop = ObjectId.Null;  //text
            ObjectId idTxBot = ObjectId.Null;
            ObjectId idTxBot2 = ObjectId.Null;

            double angle = 0.0;
            double scale = Misc.getCurrAnnoScale();
            double gapTop = 0.09;
            double gapBot = 0.09;
            List<Point3d> pnts3dLdr = new List<Point3d>();

            int n = 0;
            //Application.ShowAlertDialog(idLDR.IsErased.ToString() + idLDR.IsEffectivelyErased.ToString() + idLDR.ToString());
            PromptStatus ps = PromptStatus.Cancel;
            if (nameCmd == "cmdFFD")
            {
                pnt3d1 = UserInput.getPoint("Pick Insertion Point for Label: ", out ps, osMode: 0);
                //Line line = LdrText_JigLine.jigLine(pnt3d1);
                //angle = line.Angle;
                //line.ObjectId.delete();
                angle = (double)Application.GetSystemVariable("VIEWTWIST");
            }
            else
            {
                if (idLdr.IsEffectivelyErased || idLdr.IsErased)
                    return;

                pnts3dLdr = idLdr.getCoordinates3dList();
                n = pnts3dLdr.Count;
                angle = pnts3dLdr[n - 2].getDirection(pnts3dLdr[n - 1]);  //angle opposite direction arrow is pointing for GS or direction of 2nd segment of leader if 3 points
                pnt3dLdrEnd = pnts3dLdr[n - 1];
            }

            double angleTx = 0;

            AttachmentPoint apTop;
            AttachmentPoint apBot;
            string justifyTop = string.Empty;
            string justifyBot = string.Empty;

            bool left_justify = Base_Tools45.Math.left_Justify(angle);

            if (left_justify)
            {
                apTop = AttachmentPoint.BottomLeft;
                apBot = AttachmentPoint.TopLeft;
                angleTx = angle + pi;
                justifyTop = Pub.JUSTIFYLEFT;
                justifyBot = Pub.JUSTIFYLEFT;
            }
            else
            {
                apTop = AttachmentPoint.BottomRight;
                apBot = AttachmentPoint.TopRight;
                angleTx = angle;
                justifyTop = Pub.JUSTIFYRIGHT;
                justifyBot = Pub.JUSTIFYRIGHT;
            }

            if (cmdsGS.Contains(nameCmd))
            { // overrides for GS commands
                apTop = AttachmentPoint.BottomCenter;
                justifyTop = Pub.JUSTIFYCENTER;
            }

            if (nameCmd == "cmdFFD")
            {
                apTop = AttachmentPoint.MiddleCenter;
                justifyTop = Pub.JUSTIFYCENTER;
            }

            if (nameCmd == "cmdDEP")
                justifyTop = Pub.JUSTIFYCENTER;
            if (bot.Length < 4)
                justifyBot = Pub.JUSTIFYCENTER;

            double widthTop = 0;
            double widthBot = 0;
            double station = 0.0;
            double offset = 0.0;

            Txt.setAnnoStyle();

            Color color = Misc.getColorByLayer();
            switch (nameCmd)
            {
                case "cmdFFD":
                    string txtTop = string.Format("{0}{1}{2}", resultTop, @"\P", resultBot);
                    idTxTop = Txt.addMText(txtTop, pnt3d1, angleTx, 0.0, 0.50, apTop, "Annotative", "GRADES", color, justifyTop, AnnotativeStates.True, 0.8, true, backgroundFill: useBackgroundFill);
                    station = 0.0;
                    offset = 0.0;
                    return;

                case "cmdGS3":
                    pnt3dLdrMid = idLdr.getBegPnt().getMidPoint2d(idLdr.getEndPnt());
                    pnt3dLdrIns = pnt3dLdrMid.traverse(angle, (0.09 * scale / 2.4));
                    if (left_justify)
                    {
                        pnt3dTop = pnt3dLdrIns.traverse(angle - pi / 2, (gapTop * scale / 5));
                    }
                    else
                    {
                        pnt3dLdrIns = pnt3dLdrMid.traverse(angle, (0.09 * scale / 2.4));
                        pnt3dTop = pnt3dLdrIns.traverse(angle + pi / 2, (gapTop * scale / 5));
                    }
                    idTxTop = Txt.addMText(top, pnt3dTop, angleTx, 0.0, 0.085, apTop, "Annotative", "GRADES", color, justifyTop, AnnotativeStates.True, 0.7, backgroundFill: useBackgroundFill);
                    Geom.getStaOff(idsCgPnts, pnt3dLdrIns, ref station, ref offset);
                    pnts3dLdr = new List<Point3d> {
                        idLdr.getBegPnt(),
                        idLdr.getEndPnt(),
                        pnt3dLdrIns
                    };
                    break;

                case "cmdGS":
                case "cmdGS0":
                case "cmdGSE":
                case "cmdGSS":
                case "cmdGSX":
                case "cmdSL":
                case "cmdSS":
                    if (nameCmd == "cmdGSX")
                        gapTop = 0.01;
                    else
                        gapTop = 0.09;
                    pnt3dLdrMid = idLdr.getBegPnt().getMidPoint2d(idLdr.getEndPnt());
                    pnt3dLdrIns = pnt3dLdrMid.traverse(angle, (0.09 * scale / 2.4));
                    if (left_justify)
                    {
                        pnt3dTop = pnt3dLdrIns.traverse(angle - pi / 2, (gapTop * scale / 4));
                    }
                    else
                    {
                        pnt3dTop = pnt3dLdrIns.traverse(angle + pi / 2, (gapTop * scale / 4));
                    }
                    idTxTop = Txt.addMText(top, pnt3dTop, angleTx, 0.0, 0.09, apTop, "Annotative", "GRADES", color, justifyTop, backgroundFill: useBackgroundFill);

                    Geom.getStaOff(idsCgPnts, pnt3dLdrIns, ref station, ref offset);
                    pnts3dLdr = new List<Point3d> {
                        idLdr.getBegPnt(),
                        idLdr.getEndPnt(),
                        pnt3dLdrIns                //pnt3dM - on ldr opposite text insertion point
                    };
                    break;

                default:
                    idTxTop = Txt.addMText(resultTop, pnt3dLdrEnd, angleTx, 0.0, 0.09, apTop, "Annotative", "GRADES", color, justifyTop, backgroundFill: useBackgroundFill);
                    widthTop = idTxTop.getMTextWidth();
                    if (resultBot != string.Empty)
                        idTxBot = Txt.addMText(resultBot, pnt3dLdrEnd, angleTx, 0.0, 0.09, apBot, "Annotative", "GRADES", color, justifyBot, backgroundFill: useBackgroundFill);
                    if (resultBot2 != string.Empty)
                    {
                        pnt3dBot = pnt3dLdrEnd.traverse(angleTx - pi / 2, (.14 * scale));
                        idTxBot2 = Txt.addMText(resultBot2, pnt3dBot, angleTx, 0.0, 0.09, apBot, "Annotative", "GRADES", color, justifyBot, backgroundFill: useBackgroundFill);
                    }
                    station = 0.0;
                    offset = 0.0;
                    break;
            }
            if (resultBot == string.Empty || nameCmd == "cmdFFD")
                widthBot = 0;
            else
                widthBot = idTxBot.getMTextWidth();

            double width = 0;
            if (widthBot > widthTop)
                width = widthBot;
            else
                width = widthTop;

            //Adjust callout leader length
            if (!cmdsGS.Contains(nameCmd))
            {
                if (nameCmd == "cmdFLX" || nameCmd == "cmdGX")
                {
                    gapTop = 0.01;
                    gapBot = 0.08;
                }
                else
                {
                    gapTop = 0.09;
                    gapBot = 0.09;
                }

                Point3d pnt3dLdrEndAdj = pnts3dLdr[n - 2].traverse(angle, width);  //reseting leader endpoint based on text width

                idLdr.adjLdrEndPnt(pnt3dLdrEndAdj);
                pnts3dLdr = idLdr.getCoordinates3dList();

                if (left_justify)
                {
                    pnt3dTop = pnt3dLdrEndAdj.traverse(angle - pi / 2, gapTop * scale / 4);
                    pnt3dBot = pnt3dLdrEndAdj.traverse(angle + pi / 2, gapBot * scale / 4);
                }
                else
                {
                    pnt3dTop = pnt3dLdrEndAdj.traverse(angle + pi / 2, gapTop * scale / 4);
                    pnt3dBot = pnt3dLdrEndAdj.traverse(angle - pi / 2, gapBot * scale / 4);
                }

                idTxTop.adjMTextXYandAngle(pnt3dTop, angleTx, width);
                if (idTxBot != ObjectId.Null)
                    idTxBot.adjMTextXYandAngle(pnt3dBot, angleTx, width);
                if (idTxBot2 != ObjectId.Null)
                {
                    if (left_justify)
                    {
                        pnt3dBot = pnt3dLdrEndAdj.traverse(angle + pi / 2, 0.14 * scale);
                    }
                    else
                    {
                        pnt3dBot = pnt3dLdrEndAdj.traverse(angle - pi / 2, 0.14 * scale);
                    }

                    idTxBot2.adjMTextXYandAngle(pnt3dBot, angleTx, width);
                }
            }

            idLdr.moveToTop();

            //Add XData
            if (nameCmd != "cmdRDR" && nameCmd != "cmdSW")
            {
                if (idTxTop.IsValid)
                {
                    addXDataLnks(nameApp, nameCmd, idLdr, idTxTop, idTxBot, idTxBot2, pnt3dTop, scale, deltaZ, pnts3dLdr, idsCgPnts, station, offset, pnts3dX);
                }
                else
                    return;
                //addMTextIdToAppDict(nameApp, idTxTop);
            }
        }

        private static void
        addXDataLnks(string nameApp, string nameCmd, ObjectId idLDR, ObjectId idTxTop, ObjectId idTxBot, ObjectId idTxBot2, Point3d pnt3dTop, double scale,
            double deltaZ, List<Point3d> pnts3dLdr, List<ObjectId> idsCgPnts, double station, double offset, List<Point3d> pnts3dX = null)
        {
            ResultBuffer rb = null;
            Point3d pnt3d = Pub.pnt3dO;
            bool exists = false;
            ObjectId idDict = Dict.getNamedDictionary("checkCOs", out exists);

            #region MText

            //MText
            if (nameApp != apps.lnkGS)
            {
                TypedValue[] tvsTxTop = new TypedValue[11];
                tvsTxTop.SetValue(new TypedValue(1001, nameApp), 0);
                tvsTxTop.SetValue(new TypedValue(1000, nameCmd), 1);
                tvsTxTop.SetValue(new TypedValue(1000, "TX"), 2);
                if (nameCmd == "cmdFFD")
                    tvsTxTop.SetValue(new TypedValue(1005, "0".stringToHandle()), 3);
                else
                    tvsTxTop.SetValue(new TypedValue(1005, idLDR.getHandle()), 3);

                if (idTxBot.IsValid)
                {
                    tvsTxTop.SetValue(new TypedValue(1005, idTxBot.getHandle()), 4);
                }
                else
                {
                    tvsTxTop.SetValue(new TypedValue(1005, "0".stringToHandle()), 4);
                }

                if (idTxBot2.IsValid)
                {
                    tvsTxTop.SetValue(new TypedValue(1005, idTxBot2.getHandle()), 5);
                }
                else
                {
                    tvsTxTop.SetValue(new TypedValue(1005, "0".stringToHandle()), 5);
                }

                tvsTxTop.SetValue(new TypedValue(1070, scale), 6);
                tvsTxTop.SetValue(new TypedValue(1040, pnt3dTop.X), 7);
                tvsTxTop.SetValue(new TypedValue(1040, pnt3dTop.Y), 8);
                tvsTxTop.SetValue(new TypedValue(1040, deltaZ), 9);                 //thickness of floor slab, etc.

                if (idsCgPnts != null && idsCgPnts.Count > 0)
                    tvsTxTop.SetValue(new TypedValue(1005, idsCgPnts[0].getHandle()), 10);
                else
                    tvsTxTop.SetValue(new TypedValue(1005, "0".stringToHandle()), 10);

                idTxTop.setXData(tvsTxTop, nameApp);
            }
            else if (nameApp == apps.lnkGS)
            {
                TypedValue[] tvsTxTop = new TypedValue[12];
                tvsTxTop.SetValue(new TypedValue(1001, nameApp), 0);
                tvsTxTop.SetValue(new TypedValue(1000, nameCmd), 1);
                tvsTxTop.SetValue(new TypedValue(1000, "TX"), 2);
                tvsTxTop.SetValue(new TypedValue(1005, idLDR.getHandle()), 3);
                tvsTxTop.SetValue(new TypedValue(1070, scale), 4);
                tvsTxTop.SetValue(new TypedValue(1040, station), 5);
                tvsTxTop.SetValue(new TypedValue(1040, offset), 6);
                tvsTxTop.SetValue(new TypedValue(1040, pnt3dTop.X), 7);
                tvsTxTop.SetValue(new TypedValue(1040, pnt3dTop.Y), 8);
                if (idsCgPnts[0].IsValid)
                    tvsTxTop.SetValue(new TypedValue(1005, idsCgPnts[0].getHandle()), 9);       //to allow picking any object i.e. endpoint, nearest
                else
                {
                    tvsTxTop.SetValue(new TypedValue(1005, "0".stringToHandle()), 9);
                    pnt3d = pnts3dX[0];
                }
                if (idsCgPnts[1].IsValid)
                    tvsTxTop.SetValue(new TypedValue(1005, idsCgPnts[1].getHandle()), 10);
                else
                {
                    tvsTxTop.SetValue(new TypedValue(1005, "0".stringToHandle()), 10);
                    pnt3d = pnts3dX[1];
                }

                string xyz = string.Format("{0} {1} {2}", pnt3d.X, pnt3d.Y, pnt3d.Z);
                tvsTxTop.SetValue(new TypedValue(1000, xyz), 11);

                idTxTop.setXData(tvsTxTop, nameApp);
            }

            #endregion MText

            #region Leader

            //Leader
            TypedValue[] tvsLdr = null;
            if (nameCmd != "cmdFFD")
            { //cmdFFD does not have a leader
                if (nameApp == apps.lnkCO || nameApp == apps.lnkDP || nameApp == apps.lnkLD)
                {
                    tvsLdr = new TypedValue[6 + pnts3dLdr.Count * 2];

                    tvsLdr.SetValue(new TypedValue(1001, nameApp), 0);
                    tvsLdr.SetValue(new TypedValue(1000, nameCmd), 1);
                    tvsLdr.SetValue(new TypedValue(1000, "LDR"), 2);
                    tvsLdr.SetValue(new TypedValue(1005, idTxTop.getHandle()), 3);

                    if (idTxBot.IsValid)
                    {
                        tvsLdr.SetValue(new TypedValue(1005, idTxBot.getHandle()), 4);
                    }
                    else
                    {
                        tvsLdr.SetValue(new TypedValue(1005, "0".stringToHandle()), 4);
                    }

                    if (idTxBot2.IsValid)
                    {
                        tvsLdr.SetValue(new TypedValue(1005, idTxBot2.getHandle()), 5);
                    }
                    else
                    {
                        tvsLdr.SetValue(new TypedValue(1005, "0".stringToHandle()), 5);
                    }

                    for (int i = 0; i < pnts3dLdr.Count; i++)
                    {
                        tvsLdr.SetValue(new TypedValue(1040, pnts3dLdr[i].X), i * 2 + 6);
                        tvsLdr.SetValue(new TypedValue(1040, pnts3dLdr[i].Y), i * 2 + 7);
                    }
                    idLDR.setXData(tvsLdr, nameApp);
                }
                else if (nameApp == apps.lnkGS)
                {
                    tvsLdr = new TypedValue[11];
                    tvsLdr.SetValue(new TypedValue(1001, nameApp), 0);
                    tvsLdr.SetValue(new TypedValue(1000, nameCmd), 1);
                    tvsLdr.SetValue(new TypedValue(1000, "LDR"), 2);
                    tvsLdr.SetValue(new TypedValue(1005, idTxTop.getHandle()), 3);
                    tvsLdr.SetValue(new TypedValue(1040, pnts3dLdr[0].X), 4);
                    tvsLdr.SetValue(new TypedValue(1040, pnts3dLdr[0].Y), 5);
                    tvsLdr.SetValue(new TypedValue(1040, pnts3dLdr[1].X), 6);
                    tvsLdr.SetValue(new TypedValue(1040, pnts3dLdr[1].Y), 7);
                    tvsLdr.SetValue(new TypedValue(1040, pnts3dLdr[2].X), 8);      //case cmdsGS -> pnt3dM
                    tvsLdr.SetValue(new TypedValue(1040, pnts3dLdr[2].Y), 9);
                    tvsLdr.SetValue(new TypedValue(1040, pnts3dLdr[0].getDirection(pnts3dLdr[1])), 10);

                    idLDR.setXData(tvsLdr, nameApp);
                }
            }

            #endregion Leader

            #region CogoPoint

            //CogoPoint
            if (idsCgPnts != null && idsCgPnts.Count > 0)
            {
                ResultBuffer rbPNT = null;
                TypedValue[] tvsPNT = null;
                TypedValue[] tvsTemp = null;
                switch (nameApp)
                {
                    case apps.lnkCO:
                        rbPNT = idsCgPnts[0].getXData(nameApp);
                        if (rbPNT != null)
                        {
                            int k = 0;
                            tvsTemp = rbPNT.AsArray();
                            for (int i = 1; i < tvsTemp.Length; i++)
                            {
                                if (nameCmd == tvsTemp[i].Value.ToString())
                                { //delete existing callout
                                    ObjectId idTx = tvsTemp[i + 2].getObjectId();
                                    ResultBuffer rbTx = idTx.getXData(nameApp);
                                    if (rb == null)
                                        return;
                                    TypedValue[] tvsTx = rbTx.AsArray();
                                    ObjectId idLdr = tvsTx.getObjectId(3);
                                    idTxBot = tvsTx.getObjectId(4);
                                    idTxBot2 = tvsTx.getObjectId(5);
                                    idTx.delete();
                                    idLdr.delete();
                                    idTxBot.delete();
                                    idTxBot2.delete();
                                    k = i;
                                    break;
                                }
                            }
                            if (k != 0)
                            { //k != 0 -> existing callout deleted
                                tvsPNT = new TypedValue[tvsTemp.Length];
                                tvsTemp.CopyTo(tvsPNT, 0);
                                tvsPNT.SetValue(new TypedValue(1005, idTxTop.getHandle()), k + 2);      //replace old handle with new one
                            }
                            else
                            {
                                tvsPNT = new TypedValue[tvsTemp.Length + 3];
                                tvsTemp.CopyTo(tvsPNT, 0);
                                tvsPNT.SetValue(new TypedValue(1000, nameCmd), tvsPNT.Length - 3);
                                tvsPNT.SetValue(new TypedValue(1000, "CogoPoint"), tvsPNT.Length - 2);
                                tvsPNT.SetValue(new TypedValue(1005, idTxTop.getHandle()), tvsPNT.Length - 1);
                            }
                        }
                        else
                        {
                            tvsPNT = new TypedValue[4];
                            tvsPNT.SetValue(new TypedValue(1001, nameApp), 0);
                            tvsPNT.SetValue(new TypedValue(1000, nameCmd), 1);
                            tvsPNT.SetValue(new TypedValue(1000, "CogoPoint"), 2);
                            tvsPNT.SetValue(new TypedValue(1005, idTxTop.getHandle()), 3);
                        }

                        idsCgPnts[0].setXData(tvsPNT, nameApp);
                        break;

                    case apps.lnkDP:
                        // if xdata already exists delete callout and replace
                        tvsPNT = new TypedValue[6];
                        tvsPNT.SetValue(new TypedValue(1001, nameApp), 0);
                        tvsPNT.SetValue(new TypedValue(1000, nameCmd), 1);
                        tvsPNT.SetValue(new TypedValue(1000, "Primary"), 2);
                        tvsPNT.SetValue(new TypedValue(1005, idTxTop.getHandle()), 3);
                        tvsPNT.SetValue(new TypedValue(1005, idLDR.getHandle()), 4);
                        tvsPNT.SetValue(new TypedValue(1005, idsCgPnts[1].getHandle()), 5);

                        idsCgPnts[0].setXData(tvsPNT, nameApp);

                        tvsPNT = new TypedValue[4];
                        tvsPNT.SetValue(new TypedValue(1001, nameApp), 0);
                        tvsPNT.SetValue(new TypedValue(1000, nameCmd), 1);
                        tvsPNT.SetValue(new TypedValue(1000, "Secondary"), 2);
                        tvsPNT.SetValue(new TypedValue(1005, idsCgPnts[0].getHandle()), 3);

                        idsCgPnts[1].setXData(tvsPNT, nameApp);
                        break;

                    case apps.lnkGS:
                        //get all xdata for each point
                        foreach (ObjectId idCgPnt in idsCgPnts)
                        {
                            if (!idCgPnt.IsValid)
                                continue;
                            rbPNT = idCgPnt.getXData(nameApp);
                            if (rbPNT != null)
                            {
                                tvsTemp = rbPNT.AsArray();
                                tvsPNT = new TypedValue[tvsTemp.Length + 1];
                                tvsTemp.CopyTo(tvsPNT, 0);
                                tvsPNT.SetValue(new TypedValue(1005, idTxTop.getHandle()), tvsTemp.Length);
                            }
                            else
                            {
                                tvsPNT = new TypedValue[3];
                                tvsPNT.SetValue(new TypedValue(1001, nameApp), 0);
                                tvsPNT.SetValue(new TypedValue(1000, nameCmd), 1);
                                tvsPNT.SetValue(new TypedValue(1005, idTxTop.getHandle()), 2);
                            }
                            idCgPnt.setXData(tvsPNT, nameApp);
                        }
                        break;

                    case apps.lnkLD:
                        break;
                }
            }

            #endregion CogoPoint
        }
    }
}
