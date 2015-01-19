using Autodesk.AECC.Interop.Land;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using DBObject = Autodesk.AutoCAD.DatabaseServices.DBObject;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Grading
{
    public static class Grading_Utility
    {
        public static double PI = System.Math.PI;

        public static void
        objPnt_Modified(ObjectId idCogoPnt, string nameApp, ref List<ObjectId> idsPoly3dSLP)
        {
            ObjectId idPoly3d = ObjectId.Null;
            ResultBuffer rb = idCogoPnt.getXData(nameApp);
            if (rb != null)
            {
                List<Handle> handles = rb.rb_handles();
                switch (nameApp)
                {
                    case apps.lnkBrks:

                        foreach (Handle handle in handles)
                        {
                            try
                            {
                                idPoly3d = update3dPoly_lnkBrks1_2(apps.lnkBrks, idCogoPnt, handle);
                            }
                            catch (System.Exception ex)
                            {
                                BaseObjs.writeDebug(ex.Message + " Grading_Utility.cs: line: 38");
                            }
                        }

                        ResultBuffer rbSLP = idPoly3d.getXData(apps.lnkSLP);
                        if (rbSLP != null)
                        {
                            if (!idsPoly3dSLP.Contains(idPoly3d))
                                idsPoly3dSLP.Add(idPoly3d);
                        }

                        break;

                    case apps.lnkBrks2:

                        foreach (Handle handle in handles)
                        {
                            try
                            {
                                idPoly3d = update3dPoly_lnkBrks1_2(apps.lnkBrks2, idCogoPnt, handle);
                            }
                            catch (System.Exception ex)
                            {
                                BaseObjs.writeDebug(ex.Message + " Grading_Utility.cs: line: 61");
                            }
                        }

                        bool exists = false;
                        ObjectId idDictM = Dict.getNamedDictionary(apps.lnkBrks3, out exists);
                        ObjectId idDictPnt = Dict.getSubEntry(idDictM, idCogoPnt.getHandle().ToString());
                        List<DBDictionaryEntry> entries = Dict.getEntries(idDictPnt);
                        foreach (DBDictionaryEntry entry in entries)
                        {
                            ObjectId idDictX = Dict.getSubEntry(idDictPnt, entry.Key);

                            ResultBuffer rb2 = Dict.getXRec(idDictX, "Offset");
                            TypedValue[] tvs = rb2.AsArray();
                            double offset = (double)tvs[0].Value;

                            rb2 = Dict.getXRec(idDictX, "DeltaZ");
                            tvs = rb2.AsArray();
                            double deltaZ = (double)tvs[0].Value;

                            rb2 = Dict.getXRec(idDictX, "HandleFL");
                            tvs = rb2.AsArray();
                            Handle handleFL = tvs[0].Value.ToString().stringToHandle();

                            rb2 = Dict.getXRec(idDictX, "Beg");
                            tvs = rb2.AsArray();
                            double beg = (double)tvs[0].Value;

                            rb2 = Dict.getXRec(idDictX, "End");
                            tvs = rb2.AsArray();
                            double end = (double)tvs[0].Value;

                            Handle h3dPoly = entry.Key.ToString().stringToHandle();
                            update3dPoly_lnkBrks3(h3dPoly, offset, deltaZ, handleFL, beg, end);
                        }
                        break;
                }
            }
        }

        public static void
        resetHandle(ObjectId idPoly3d)
        {
            ResultBuffer rb = idPoly3d.getXData(apps.lnkBrks);
            List<Handle> handles = rb.rb_handles();
            Point3dCollection pnts3d = idPoly3d.getCoordinates3d();

            if (handles.Count == pnts3d.Count)
            {
                for (int i = 0; i < handles.Count; i++)
                {
                    Point3d pnt3dCogoPnt = handles[i].getCogoPntCoordinates();
                    if (Measure.getDistance2d(pnts3d[i], pnt3dCogoPnt, 3) == 0.0 && pnts3d[i].Z == pnt3dCogoPnt.Z)
                    {
                    }
                    else
                    {
                        idPoly3d.updateVertex(i, pnt3dCogoPnt);
                    }
                }
            }
        }

        public static ObjectId
        update3dPoly_lnkBrks1_2(string nameApp, ObjectId idCogoPnt, Handle hEnt3d)
        {
            ObjectId idEnt3d = ObjectId.Null;
            Entity ent3d = hEnt3d.getEnt();

            if (ent3d == null)
                return idEnt3d;

            if (ent3d is Polyline3d)
            {
                Polyline3d poly3d = (Polyline3d)ent3d;
                ObjectId idPoly3d = poly3d.ObjectId;
                idEnt3d = idPoly3d;
                Point3d pnt3dBeg = idPoly3d.getBegPnt();
                Point3d pnt3dEnd = idPoly3d.getEndPnt();
                bool closed = false;
                if (pnt3dBeg.isEqual(pnt3dEnd, 0.01))
                    closed = true;

                ResultBuffer rb = idEnt3d.getXData(nameApp);
                if (rb == null)
                    return idEnt3d;
                List<Handle> handles = rb.rb_handles();
                int n = 0;
                for (int i = 0; i < handles.Count; i++)
                {
                    Point3d pnt3d = handles[i].getCogoPntCoordinates();
                    idPoly3d.updateVertex(i, pnt3d);
                    n = i;
                }
                if (closed)
                    idPoly3d.updateVertex(n + 1, handles[0].getCogoPntCoordinates());
            }
            if (ent3d is FeatureLine)
            {
                try
                {
                    DBObject dbObj = ent3d;
                    AeccLandFeatureLine fLine = (AeccLandFeatureLine)dbObj.AcadObject;
                    idEnt3d = ent3d.ObjectId;

                    var points = fLine.GetPoints(AeccLandFeatureLinePointType.aeccLandFeatureLinePointPI);
                    object varType = null;
                    object varVal = null;

                    fLine.GetXData(apps.lnkBrks, out varType, out varVal);
                    fLine = null;
                    List<Handle> handles = Base_VB.myUtility.comXDataToList(varVal);

                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        AeccLandFeatureLine oFL = (AeccLandFeatureLine)dbObj.AcadObject;
                        for (int i = 0; i < handles.Count; i++)
                        {
                            Point3d pnt3d = handles[i].getCogoPntCoordinates();
                            double[] varPoint = new double[3];
                            varPoint[0] = pnt3d.X;
                            varPoint[1] = pnt3d.Y;
                            varPoint[2] = pnt3d.Z;

                            oFL.SetPointElevation((object)varPoint);
                        }

                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(ex.Message + " Grading_Utility.cs: line: 193");
                }
            }

            return idEnt3d;
        }

        public static void
        update3dPoly_lnkBrks3(Handle handleX, double offset, double deltaZ, Handle handleRF, double beg, double end)
        {
            Point3d pnt3dRF_BEG = handleRF.getBegPnt();
            Point3d pnt3dRF_END = handleRF.getEndPnt();

            double dir = pnt3dRF_BEG.getDirection(pnt3dRF_END);
            double distance = pnt3dRF_BEG.getDistance(pnt3dRF_END);
            double slope = pnt3dRF_BEG.getSlope(pnt3dRF_END);
            double xSlope = System.Math.Abs(deltaZ / offset);

            Point3d pnt3dRF = pnt3dRF_BEG.traverse(dir, beg, slope);        //POINT ON RF (FL or EDGE) OPPOSITE BEG OF TARGET BREAKLINE

            double angleOFF = 0;

            if (offset < 0)
                angleOFF = dir + PI / 2;
            else
                angleOFF = dir - PI / 2;

            Point3d pnt3dBegX = pnt3dRF.traverse(angleOFF, System.Math.Abs(offset), xSlope);    //begin point on target (FL or EDGE)

            handleX.setBegPnt(pnt3dBegX);

            if (end != -1)
                pnt3dRF = pnt3dBegX.traverse(dir, end - beg, slope);            //POINT ON FL OPPOSITE BEG OF TARGET BREAKLINE
            else
                pnt3dRF = pnt3dBegX.traverse(dir, distance, slope);

            handleX.setEndPnt(pnt3dRF);
        }

        public static void
        updateGS(TypedValue[] tvsPnt, string nameCmd) {
            double pi = System.Math.PI;

            for (int i = 2; i < tvsPnt.Length; i++)
            {
                ObjectId idMTxt = tvsPnt[i].Value.ToString().stringToHandle().getObjectId();
                ResultBuffer rbMTxt = idMTxt.getXData(apps.lnkGS);
                if (rbMTxt == null)
                    continue;
                TypedValue[] tvsMTxt = rbMTxt.AsArray();

                string nameCmdGS = tvsMTxt[1].Value.ToString();
                ObjectId idLdr = tvsMTxt[3].Value.ToString().stringToHandle().getObjectId();
                double scale = double.Parse(tvsMTxt[4].Value.ToString());
                double station = double.Parse(tvsMTxt[5].Value.ToString());
                double offset = double.Parse(tvsMTxt[6].Value.ToString());

                ObjectId idPnt1 = tvsMTxt[9].Value.ToString().stringToHandle().getObjectId();
                ObjectId idPnt2 = tvsMTxt[10].Value.ToString().stringToHandle().getObjectId();

                ResultBuffer rbLdr = idLdr.getXData(apps.lnkGS);
                if (rbLdr == null)
                    return;
                TypedValue[] tvsLdr = rbLdr.AsArray();
                Point3d pnt3dM = new Point3d(double.Parse(tvsLdr[8].Value.ToString()), double.Parse(tvsLdr[9].Value.ToString()), 0.0);

                Point3d pnt3d1, pnt3d2;
                if (idPnt1.IsValid)
                    pnt3d1 = idPnt1.getCogoPntCoordinates();
                else
                    pnt3d1 = Mod.stringCoordinateListToPoint3d(tvsMTxt[11]);

                if (idPnt2.IsValid)
                    pnt3d2 = idPnt2.getCogoPntCoordinates();
                else
                    pnt3d2 = Mod.stringCoordinateListToPoint3d(tvsMTxt[11]);

                double angleSlope = 0.0;

                double angle = pnt3d1.getDirection(pnt3d2);     //angle from pnt1 to pnt2
                double dxy = pnt3d1.getDistance(pnt3d2);
                double dz = pnt3d2.Z - pnt3d1.Z;
                double slope = dz / dxy;

                if (dz > 0)                                     //nominal case is sloping downhill - arrow points to lower grade
                    angleSlope = angle + pi;
                else
                    angleSlope = angle;

                Point3d pnt3dLdr1 = Pub.pnt3dO;
                Point3d pnt3dLdr2 = Pub.pnt3dO;

                string res1 = string.Empty;
                slope = System.Math.Abs(slope);

                switch (nameCmdGS)
                {
                    case "cmdGS":
                    case "cmdGSE":
                        res1 = string.Format("R={0:F2}%", slope * 100);
                        break;

                    case "cmdGS0":
                        res1 = string.Format("{0:F0}%", Base_Tools45.Math.roundUP3(slope) * 100);
                        break;

                    case "cmdGS2":
                        res1 = string.Format("R={0:F2}%", slope * 100);
                        break;

                    case "cmdGS3":
                        res1 = string.Format("{0:F0}%", Base_Tools45.Math.roundUP3(slope) * 100);
                        break;

                    case "cmdGSS":
                        res1 = string.Format("{0:F1}%", slope * 100);
                        break;

                    case "cmdGSX":
                        res1 = string.Format("(R={0:F2}%)", slope * 100);
                        break;

                    case "cmdSS":
                        res1 = string.Format("S={0:F4}", slope);
                        break;
                }

                Point3dCollection pnts3d = idLdr.getCoordinates3d();
                Point3d pnt3dMx = Pub.pnt3dO;

                pnt3dLdr1 = pnts3d[0];
                pnt3dLdr2 = pnts3d[1];

                double angleLdr = pnt3dLdr2.getDirection(pnt3dLdr1);                //angle that leader is pointing
                double angleDiff = System.Math.Abs(System.Math.Round(angleLdr - angleSlope, 2));
                double angleTxt = 0.0;

                if (nameCmd == "cmdMCE")
                {
                    if (angleDiff == 3.14)
                        Mod.rotateEnt(idLdr, pnt3dM, System.Math.PI);               //just need to flip leader to match downhill slope
                }
                else
                { //nameCmd = "Grip_Stretch_Move"
                    angleDiff = angleSlope - angleLdr;                              //angle to rotate text and leader
                    pnt3dMx = pnt3d1.traverse(angle, station);
                    bool left_justify = Base_Tools45.Math.left_Justify(angleSlope);
                    if (left_justify)
                    {
                        pnt3dMx = pnt3dMx.traverse(angleSlope - pi, offset);
                        angleTxt = angle;
                    }
                    else
                    {
                        pnt3dMx = pnt3dMx.traverse(angleSlope + pi, offset);
                        angleTxt = angle + pi;
                    }

                    idMTxt.moveObj(pnt3dM, pnt3dMx);
                    idLdr.moveObj(pnt3dM, pnt3dMx);

                    Mod.rotateEnt(idMTxt, idMTxt.getMTextLocation(), angleDiff);
                    Mod.rotateEnt(idLdr, pnt3dMx, angleDiff);

                    pnt3dLdr1 = idLdr.getBegPnt();
                    pnt3dLdr2 = idLdr.getEndPnt();

                    tvsLdr[4] = new TypedValue(1040, pnt3dLdr1.X);
                    tvsLdr[5] = new TypedValue(1040, pnt3dLdr1.Y);
                    tvsLdr[6] = new TypedValue(1040, pnt3dLdr2.X);
                    tvsLdr[7] = new TypedValue(1040, pnt3dLdr2.Y);
                    tvsLdr[8] = new TypedValue(1040, pnt3dMx.X);
                    tvsLdr[9] = new TypedValue(1040, pnt3dMx.Y);

                    idLdr.setXData(tvsLdr, apps.lnkGS);

                    pnt3d1 = tvsMTxt.getObjectId(9).getCogoPntCoordinates();
                    pnt3d2 = tvsMTxt.getObjectId(10).getCogoPntCoordinates();

                    Geom.getStaOff(pnt3d1, pnt3d2, pnt3dMx, ref station, ref offset);
                    tvsMTxt[5] = new TypedValue(1040, station);
                    tvsMTxt[6] = new TypedValue(1040, offset);

                    Point3d pnt3dIns = idMTxt.getMTextLocation();
                    tvsMTxt[7] = new TypedValue(1040, pnt3dIns.X);
                    tvsMTxt[8] = new TypedValue(1040, pnt3dIns.Y);

                    idMTxt.setXData(tvsMTxt, apps.lnkGS);
                }

                idMTxt.updateMText(res1);
            }
        }
 

        public static void
        updatePntCalloutElev(double elev, TypedValue[] tvsPnt)
        {
            string txtBotText = string.Empty;
            string txtBotFormat = string.Empty;

            string txtBot2Text = string.Empty;

            string txtTopNew = string.Empty;
            string txtBotNew = string.Empty;

            string suffixTop;
            string suffixBot;

            string nameApp = tvsPnt[0].Value.ToString();

            string nameCmd = "";

            switch (nameApp)
            {
                case apps.lnkCO:

                    for (int i = 1; i < tvsPnt.Length; i = i + 3)
                    {
                        nameCmd = tvsPnt[i].Value.ToString();
                        ObjectId idMTxt = tvsPnt.getObjectId(i + 2);

                        ResultBuffer rbMTxt = idMTxt.getXData(nameApp);
                        if (rbMTxt == null)
                            continue;
                        TypedValue[] tvsMTxt = rbMTxt.AsArray();
                        ObjectId idMTxtBot = tvsMTxt.getObjectId(4);
                        ObjectId idMTxtBot2 = tvsMTxt.getObjectId(5);

                        string txtTopText = idMTxt.getMTextText();
                        bool existBot = false;
                        bool existBot2 = false;
                        if (idMTxtBot.IsValid)
                        {
                            txtBotText = idMTxtBot.getMTextText();
                            existBot = true;
                        }

                        if (idMTxtBot2.IsValid)
                        {
                            txtBot2Text = idMTxtBot2.getMTextText();
                            existBot2 = true;
                        }

                        switch (nameCmd)
                        {
                            case "cmdCF0":
                                txtTopNew = string.Format("{0:F2}TC", elev);
                                idMTxt.updateMText(txtTopNew);
                                break;

                            case "cmdFF":
                            case "cmdFFD":
                                txtTopNew = string.Format("{0:F2}FF", elev);
                                idMTxt.updateMText(txtTopNew);
                                if (existBot)
                                {
                                    double elevBotNew = elev - double.Parse(tvsMTxt[9].Value.ToString());
                                    txtBotNew = string.Format("{0:F2} PAD", elevBotNew);
                                    idMTxtBot.updateMText(txtBotNew);
                                }
                                break;

                            case "cmdFL":
                            case "cmdFLX":
                                switch (nameCmd)
                                {
                                    case "cmdFL":
                                        txtTopNew = string.Format("{0:F2}", elev);
                                        break;

                                    case "cmdFLX":
                                        txtTopNew = string.Format("({0:F2})", elev);
                                        break;
                                }
                                idMTxt.updateMText(txtTopNew);
                                break;

                            case "cmdG":
                            case "cmdGX":
                                suffixTop = Txt.getCalloutSuffix(txtTopText);
                                suffixBot = Txt.getCalloutSuffix(txtBotText);
                                double elevTopNew = elev + double.Parse(tvsMTxt[9].Value.ToString());
                                switch (nameCmd)
                                {
                                    case "cmdG":
                                        txtTopNew = string.Format("{0:F2}{1}", elevTopNew, suffixTop);
                                        txtBotNew = string.Format("{0:F2}{1}", elev, suffixBot);
                                        break;

                                    case "cmdGX":
                                        txtTopNew = string.Format("{0:F2}({1})", elevTopNew, suffixTop);
                                        txtBotNew = string.Format("{0:F2}({1})", elev, suffixBot);

                                        break;
                                }
                                idMTxt.updateMText(txtTopNew);
                                idMTxtBot.updateMText(txtBotNew);
                                break;

                            case "cmdSDE":
                            case "cmdSED":
                                suffixBot = Txt.getCalloutSuffix(txtBot2Text);
                                idMTxtBot2.updateMText(string.Format("{0:F2}{1}", elev, suffixBot));
                                break;
                        }
                    }
                    break;

                case apps.lnkDP:

                    ObjectId idCgPnt2 = tvsPnt.getObjectId(5);
                    double deltaZ = System.Math.Abs(elev - idCgPnt2.getCogoPntElevation());
                    txtBotNew = string.Format("{0}PANEL= {1}'", txtBotFormat, deltaZ.ToString("F2"));
                    break;

                case apps.lnkLD:
                    break;

                default:
                    break;
            }
        }

        public static void updateSurfaces()
        {
            try
            {
                Grading_Palette.gPalette.pGrading.BlockXRefName = String.Empty;
                Grading_Palette.gPalette.pGrading.Initialize_Form();

                myForms.GradeFloor pGF = Grading_Palette.gPalette.pGradeFloor;

                pGF.lstBox1.Items.Clear();

                List<string> surfaces = Surf.getSurfaces();
                if (surfaces != null)
                {
                    for (int i = 0; i < surfaces.Count; i++)
                    {
                        pGF.lstBox1.Items.Add(surfaces[i].ToUpper());
                        if (surfaces[i].ToUpper() == "EXIST")
                        {
                            pGF.lstBox1.SelectedIndex = i;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Grading_Utility.cs: line: 543");
            }
        }
    }
}
