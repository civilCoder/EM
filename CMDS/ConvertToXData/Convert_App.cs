using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;



namespace ConvertToXData
{
    public static class Convert_App
    {
        public static void
        convertDictionaryLinksToXData()
        {
            try
            {
                BaseObjs.sendCommand("VBAUNLOAD\rC:\\TSET\\VBA2015\\CIVIL3D2015.DVB\r");
                //BaseObjs.sendCommand("(unloadCivil3d2015)\r");
            }
            catch (System.Exception ex)
            {
                Application.ShowAlertDialog(string.Format("{0} Convert_App.cs: line: 31", ex.Message));
                BaseObjs.writeDebug(string.Format("{0} Convert_App.cs: line: 31", ex.Message));
            }
            int k = 0;

            bool exists = false;

            ObjectId idBlock = ObjectId.Null;
            ObjectId idLeader = ObjectId.Null;
            ObjectId idText = ObjectId.Null;
            ObjectId idPnt1 = ObjectId.Null;
            ObjectId idPnt2 = ObjectId.Null;
            ObjectId idAlign = ObjectId.Null;
            ObjectId idSurface = ObjectId.Null;
            ObjectId idPoly = ObjectId.Null;
            ObjectId idBubble = ObjectId.Null;
            ObjectId idCwpRef = ObjectId.Null;

            string topNum = string.Empty;
            string topTxt = string.Empty;
            string botNum = string.Empty;
            string botTxt = string.Empty;
            string nameCmd = string.Empty;
            string nameApp = string.Empty;
            string top = string.Empty;
            string bot = string.Empty;

            List<string> dicts = new List<string> {
                "GradeTagDict",
                "PtTagDict",
                "FlTagDict",
                "SEDTagDict",
                "MHTagDict",
                "XrGradeTagDict",
                "XrFlTagDict",
                "VBTagDict"
            };

            List<ObjectId> idsCgPnts = null;

            ResultBuffer rb = null;
            TypedValue[] tvs = null;

            List<Object> errorCallouts = new List<object>();

            foreach (string dict in dicts)
            {
                ObjectId idDict = Dict.getNamedDictionary(dict, out exists);
                List<DBDictionaryEntry> entries = idDict.getEntries();

                foreach (DBDictionaryEntry entry in entries)
                {
                    string shPnt1 = entry.Key;
                    Handle hPnt1 = new Handle(Int64.Parse(shPnt1, NumberStyles.AllowHexSpecifier));
                    idPnt1 = hPnt1.getObjectId();

                    rb = Dict.getXRec(idDict, hPnt1.ToString());
                    if (rb == null)
                    {
                        continue;
                    }
                    tvs = rb.AsArray();
                    idBlock = tvs[0].Value.ToString().stringToHandle().getObjectId();
                    checkGroup(idBlock);

                    idLeader = tvs[1].Value.ToString().stringToHandle().getObjectId();

                    //if (idBlock.IsEffectivelyErased || idBlock.IsErased)
                    //{
                    if (idLeader.IsErased || idLeader.IsEffectivelyErased)
                    {
                        Dict.deleteXRec(idDict, hPnt1.ToString());
                        errorCallouts.Add(idPnt1.getCogoPntNumber());
                        continue;
                    }
                    //}

                    idsCgPnts = new List<ObjectId> {
                        idPnt1
                    };

                    Leader ldr = null;
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        ldr = (Leader)tr.GetObject(idLeader, OpenMode.ForWrite);
                        ldr.Annotative = AnnotativeStates.True;
                        tr.Commit();
                    }

                    idLeader.moveToTop();

                    topNum = Blocks.getBlockRefAttributeValue(idBlock, "TOPNUM");
                    topTxt = Blocks.getBlockRefAttributeValue(idBlock, "TOPTXT");
                    botNum = Blocks.getBlockRefAttributeValue(idBlock, "BOTNUM");
                    botTxt = Blocks.getBlockRefAttributeValue(idBlock, "BOTTXT");

                    switch (dict)
                    {
                        case "GradeTagDict":
                            nameCmd = "cmdG";
                            nameApp = "lnkC0";
                            break;

                        case "PtTagDict":
                            nameCmd = "cmdPT";
                            nameApp = "lnkC0";
                            break;

                        case "FlTagDict":
                            nameCmd = "cmdFL";
                            nameApp = "lnkC0";
                            break;

                        case "SEDTagDict":
                            nameCmd = "cmdSDE";
                            nameApp = "lnkC0";
                            idAlign = tvs[2].Value.ToString().stringToHandle().getObjectId();
                            break;

                        case "MHTagDict":
                            nameCmd = "cmdMH";
                            nameApp = "lnkC0";
                            idAlign = tvs[2].Value.ToString().stringToHandle().getObjectId();
                            idSurface = tvs[3].Value.ToString().stringToHandle().getObjectId();
                            break;

                        case "XrGradeTagDict":
                            nameCmd = "cmdSDE";
                            nameApp = "lnkC0";
                            idAlign = tvs[2].Value.ToString().stringToHandle().getObjectId();
                            idSurface = tvs[3].Value.ToString().stringToHandle().getObjectId();
                            break;

                        case "XrFlTagDict":
                            nameCmd = "cmdSDE";
                            nameApp = "lnkC0";
                            idAlign = tvs[2].Value.ToString().stringToHandle().getObjectId();
                            idSurface = tvs[3].Value.ToString().stringToHandle().getObjectId();
                            break;

                        case "VBTagDict":
                            nameCmd = "cmdVB";
                            nameApp = "lnkC0";
                            idPoly = tvs[1].Value.ToString().stringToHandle().getObjectId();
                            idSurface = tvs[2].Value.ToString().stringToHandle().getObjectId();
                            idBubble = tvs[3].Value.ToString().stringToHandle().getObjectId();
                            idCwpRef = tvs[4].Value.ToString().stringToHandle().getObjectId();
                            break;
                    }

                    top = string.Format("{0}{1}", topNum, topTxt);
                    bot = string.Format("{0}{1}", botNum, botTxt);
                    idBlock.delete();
                    Dict.deleteXRec(idDict, hPnt1.ToString());
                    if (topNum.Contains("(") && topNum.Contains(")"))
                        nameCmd = string.Format("{0}X", nameCmd);
                    double dZ = 0.0;
                    if (botNum != "")
                    {
                        if (botNum == "0\"")
                        {
                            nameCmd = "cmdCF0";
                            dZ = 0;
                        }
                        else
                        {
                            dZ = double.Parse(topNum.ToString()) - double.Parse(botNum.ToString());
                        }
                    }

                    Txt.addLdrText(nameCmd, nameApp, ldr.ObjectId, idsCgPnts, top, bot, "", dZ, useBackgroundFill: true);
                    ++k;
                }
            }

            ObjectId idDictGS = Dict.getNamedDictionary("GSDict", out exists);
            List<DBDictionaryEntry> entriesGS = idDictGS.getEntries();
            ResultBuffer rbGS = null;
            TypedValue[] tvsGS = null;

            foreach (DBDictionaryEntry entryGS in entriesGS)
            {
                ObjectId idEntryGS = ObjectId.Null;
                string shText = entryGS.Key;
                Handle hText = new Handle(Int64.Parse(shText, NumberStyles.AllowHexSpecifier));
                idText = hText.getObjectId();
                rbGS = Dict.getXRec(idDictGS, hText.ToString());
                if (rbGS == null)
                    continue;
                tvsGS = rbGS.AsArray();
                idLeader = tvsGS[0].getObjectId();
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    DBText text = (DBText)tr.GetObject(idText, OpenMode.ForRead);
                    top = text.TextString;
                    bot = string.Empty;
                    Leader ldr = (Leader)tr.GetObject(idLeader, OpenMode.ForWrite);
                    ldr.Annotative = AnnotativeStates.True;
                    tr.Commit();
                }
                switch (top.Substring(0, 1))
                {
                    case "(":
                        nameCmd = "cmdGSX";
                        break;

                    case "R":
                        nameCmd = "cmdGS";
                        break;

                    case "S":
                        nameCmd = "cmdSS";
                        break;

                    default:
                        if (top.Contains("."))
                            nameCmd = "cmdGSS";
                        else
                            nameCmd = "cmdGS0";
                        break;
                }

                idsCgPnts = new List<ObjectId> {
                    tvsGS[1].getObjectId(),
                    tvsGS[2].getObjectId()
                };
                nameApp = apps.lnkGS;
                Dict.deleteXRec(idDictGS, hText.ToString());
                idText.delete();

                Txt.addLdrText(nameCmd, nameApp, idLeader, idsCgPnts, top, bot, useBackgroundFill: true);
                ++k;
                Debug.Print(string.Format("k:{0}", k));
            }

            tvs = new TypedValue[11];
            tvs.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 0);
            tvs.SetValue(new TypedValue((int)DxfCode.Operator, "<OR"), 1);
            tvs.SetValue(new TypedValue((int)DxfCode.BlockName, "GradeTag"), 2);
            tvs.SetValue(new TypedValue((int)DxfCode.BlockName, "XrGradeTag"), 3);
            tvs.SetValue(new TypedValue((int)DxfCode.BlockName, "FlTag"), 4);
            tvs.SetValue(new TypedValue((int)DxfCode.BlockName, "XrFlTag"), 5);
            tvs.SetValue(new TypedValue((int)DxfCode.BlockName, "PlxTag"), 6);
            tvs.SetValue(new TypedValue((int)DxfCode.BlockName, "TcTag"), 7);
            tvs.SetValue(new TypedValue((int)DxfCode.BlockName, "PtTag"), 8);
            tvs.SetValue(new TypedValue((int)DxfCode.BlockName, "LabelTag"), 9);
            tvs.SetValue(new TypedValue((int)DxfCode.Operator, "OR>"), 10);

            SelectionSet ss = Select.buildSSet(tvs);
            if (ss != null && ss.Count > 0)
            {
                ObjectId[] ids = ss.GetObjectIds();
                Point3d pnt3dIns;

                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    foreach (ObjectId id in ids)
                    {
                        BlockReference br = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                        pnt3dIns = br.Position;
                        List<ObjectId> idLdrs = Select.getEntityatPoint(pnt3dIns, typeof(Leader), "ARROW", 1.0);
                        if (idLdrs == null || idLdrs.Count == 0)
                        {
                            Debug.Print(string.Format("No Leader: {0},{1}", pnt3dIns.X, pnt3dIns.Y));
                            continue;
                        }
                        Point3d pnt3dLdrBeg = idLdrs[0].getBegPnt();

                        List<ObjectId> idCgPnts = Select.getEntityatPoint(pnt3dLdrBeg, typeof(CogoPoint), "CPNT-ON");
                        if (idCgPnts == null || idCgPnts.Count == 0)
                            Debug.Print(string.Format("No CogoPnt: {0},{1}", pnt3dLdrBeg.X, pnt3dLdrBeg.Y));

                        idLeader = idLdrs[0];
                        Leader ldr = (Leader)tr.GetObject(idLeader, OpenMode.ForWrite);
                        ldr.Annotative = AnnotativeStates.True;

                        idLeader.moveToTop();

                        string nameBlock = br.Name.ToUpper();

                        switch (nameBlock)
                        {
                            case "GRADETAG":
                            case "XRGRADETAG":
                                topNum = Blocks.getBlockRefAttributeValue(id, "TOPNUM");
                                topTxt = Blocks.getBlockRefAttributeValue(id, "TOPTXT");
                                botNum = Blocks.getBlockRefAttributeValue(id, "BOTNUM");
                                botTxt = Blocks.getBlockRefAttributeValue(id, "BOTTXT");
                                top = string.Format("{0}{1}", topNum, topTxt);
                                bot = string.Format("{0}{1}", botNum, botTxt);
                                nameCmd = "cmdG";
                                break;

                            case "FLTAG":
                            case "XRFLTAG":
                            case "PLXTAG":
                                top = Blocks.getBlockRefAttributeValue(id, "TOPNUM");
                                bot = Blocks.getBlockRefAttributeValue(id, "BOTTXT");
                                nameCmd = "cmdFL";
                                break;

                            case "TCTAG":
                            case "PTTAG":
                            case "LABELTAG":
                                top = Blocks.getBlockRefAttributeValue(id, "TOPLABEL");
                                bot = Blocks.getBlockRefAttributeValue(id, "BOTLABEL");
                                if (top == "DEEPEN")
                                    nameCmd = "cmdDEP";
                                if (bot == "RISERS")
                                    nameCmd = "cmdRISER";
                                break;
                        }

                        id.delete();

                        nameApp = apps.lnkCO;

                        if (top.Contains("(") && top.Contains(")"))
                            nameCmd = string.Format("{0}X", nameCmd);
                        double dZ = 0.0;
                        if (botNum != "")
                        {
                            if (botNum == "0\"")
                            {
                                nameCmd = "cmdCF0";
                                dZ = 0;
                            }
                            else
                            {
                                dZ = double.Parse(topNum.ToString()) - double.Parse(botNum.ToString());
                            }
                        }

                        Txt.addLdrText(nameCmd, nameApp, idLeader, idsCgPnts, top, bot, "", dZ, useBackgroundFill: true);
                        ++k;
                    }
                    tr.Commit();
                }
            }

            Application.ShowAlertDialog(string.Format("{0} Callouts converted.  {1} Points with broken links", k.ToString(), errorCallouts.Count.ToString()));
            foreach (object pntNum in errorCallouts)
                Debug.Print(pntNum.ToString());

            BaseObjs.sendCommand("-VBALOAD\rC:\\TSET\\VBA2015\\CIVIL3D2015.DVB\r");
        }

        public static void
        convertGSItem(ObjectId idText)
        {
            Handle hText = idText.getHandle();
            bool exists = false;
            ObjectId idDictGS = Dict.getNamedDictionary("GSDict", out exists);
            ResultBuffer rbGS = Dict.getXRec(idDictGS, hText.ToString());
            if (rbGS == null)
                return;
            TypedValue[] tvsGS = rbGS.AsArray();
            ObjectId idLeader = tvsGS[0].getObjectId();

            string top, bot, nameCmd;
            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                DBText text = (DBText)tr.GetObject(idText, OpenMode.ForRead);
                top = text.TextString;
                bot = string.Empty;
                Leader ldr = (Leader)tr.GetObject(idLeader, OpenMode.ForWrite);
                ldr.Annotative = AnnotativeStates.True;
                tr.Commit();
            }
            switch (top.Substring(0, 1))
            {
                case "(":
                    nameCmd = "cmdGSX";
                    break;

                case "R":
                    nameCmd = "cmdGS";
                    break;

                case "S":
                    nameCmd = "cmdSS";
                    break;

                default:
                    if (top.Contains("."))
                        nameCmd = "cmdGSS";
                    else
                        nameCmd = "cmdGS0";
                    break;
            }

            List<ObjectId> idsCgPnts = new List<ObjectId> {
                tvsGS[1].getObjectId(),
                tvsGS[2].getObjectId()
            };
            string nameApp = apps.lnkGS;
            Dict.deleteXRec(idDictGS, hText.ToString());
            idText.delete();

            Txt.addLdrText(nameCmd, nameApp, idLeader, idsCgPnts, top, bot, useBackgroundFill: true);
        }

        public static void
        convertBubble()
        {
            ObjectId idWO = ObjectId.Null;  //wipeout
            ObjectId idSM = ObjectId.Null;  //symbol
            ObjectId idTX = ObjectId.Null;  //text
            ObjectId idLDR = ObjectId.Null; //leader

            Point3d pnt3d = Pub.pnt3dO;
            Object osMode = SnapMode.getOSnap();

            int scale = Misc.getCurrAnnoScale();

            TypedValue[] tvs = new TypedValue[2];
            tvs.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 0);
            tvs.SetValue(new TypedValue((int)DxfCode.LayerName, "BUBBLE"), 1);

            SelectionSet ss = Select.buildSSet(tvs);
            ObjectId[] idsBR = ss.GetObjectIds();

            ObjectId idLayer = Layer.manageLayers("BUBBLE");
            double angleView = -(double)Application.GetSystemVariable("VIEWTWIST");

            Database db = HostApplicationServices.WorkingDatabase;

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                DBDictionary groups = (DBDictionary)db.GroupDictionaryId.GetObject(OpenMode.ForRead);
                BlockTableRecord ms = Blocks.getBlockTableRecordMS();

                foreach (ObjectId idBR in idsBR)
                {
                    BlockReference br = (BlockReference)tr.GetObject(idBR, OpenMode.ForWrite);

                    if (br.Name == "CWP")
                    {
                        br.ObjectId.delete();
                        continue;
                    }

                    Scale3d scaleFactor = br.ScaleFactors;
                    Matrix3d m3d = scaleFactor.GetMatrix();
                    double scaleBR = m3d.GetScale();

                    double rotation = br.Rotation;
                    Point3d pnt3dPos = br.Position;
                    string name = br.Name;
                    string value = string.Empty;
                    Entity ent = null;
                    AttributeCollection atts = br.AttributeCollection;
                    foreach (ObjectId id in atts)
                    {
                        ent = id.getEnt();
                        AttributeReference attDef = (AttributeReference)ent;
                        value = attDef.TextString;
                    }

                    Vector3d v3d = new Vector3d(scaleBR * 1.35, scaleBR * 1.35, 0);
                    v3d = v3d * 1.1;
                    Point3d pnt3dLL = pnt3dPos - v3d;
                    Point3d pnt3dUR = pnt3dPos + v3d;
                    Point3d pnt3dLR = new Point3d(pnt3dUR.X, pnt3dLL.Y, 0.0);
                    Point3d pnt3dUL = new Point3d(pnt3dLL.X, pnt3dUR.Y, 0.0);

                    Point3dCollection pnts3d = new Point3dCollection {
                        pnt3dLL,
                        pnt3dLR,
                        pnt3dUR,
                        pnt3dUL,
                        pnt3dLL
                    };

                    tvs = new TypedValue[2];
                    tvs.SetValue(new TypedValue((int)DxfCode.Start, "LWPOLYLINE"), 0);
                    tvs.SetValue(new TypedValue((int)DxfCode.LayerName, "BUBBLE"), 1);

                    Point3d pnt3dBeg = Pub.pnt3dO;
                    Point3d pnt3dEnd = Pub.pnt3dO;
                    Point3d pnt3dMid = Pub.pnt3dO;

                    double bulge = 0.0, delta0 = 0.0;
                    double dirChord, dirTarget, disChord, disTarget;
                    Polyline poly;
                    ObjectIdCollection idsLdr = new ObjectIdCollection();
                    ss = Select.buildSSet(tvs, pnts3d);
                    if (ss != null && ss.Count > 0)
                    {
                        ObjectId[] ids = ss.GetObjectIds();
                        foreach (ObjectId id in ids)
                        {
                            poly = (Polyline)id.getEnt();
                            if (poly != null)
                            {
                                pnt3dBeg = poly.StartPoint;
                                pnt3dMid = poly.GetPoint3dAt(1);
                                pnt3dEnd = poly.EndPoint;

                                bulge = poly.GetBulgeAt(1);
                                delta0 = Geom.getDelta(bulge);
                                disChord = pnt3dMid.getDistance(pnt3dEnd);
                                disTarget = (0.5 * disChord) / System.Math.Cos(delta0 / 4);
                                dirChord = pnt3dMid.getDirection(pnt3dEnd);
                                dirTarget = dirChord - delta0 / 4;
                                pnt3dMid = pnt3dMid.traverse(dirTarget, disTarget);

                                //dirTarget = pnt3dEnd.getDirection(pnt3dPos);    //direction towards center of circle
                                //disTarget = pnt3dEnd.getDistance(pnt3dPos);     //distance from original end point to center
                                //disTarget = disTarget - Pub.radius * scale * 0.85;    //0.85 per addSymbolAndWipeout  - factors to adjust symbols for best fit.
                                //pnt3dEnd = pnt3dEnd.traverse(dirTarget, disTarget);

                                pnt3dEnd = pnt3dPos;

                                pnts3d = new Point3dCollection {
                                    pnt3dBeg,
                                    pnt3dMid,
                                    pnt3dEnd
                                };
                                ObjectId idLdr = Ldr.addLdr(pnts3d, idLayer, 0.09 * scale, 0, clr.byl, ObjectId.Null, spline: true);
                                idsLdr.Add(idLdr);
                                poly.ObjectId.delete();
                            }
                        }
                    }

                    idSM = Base_Tools45.Draw.addSymbolAndWipeout(pnt3dPos, angleView, out idWO, Pub.radius, 1024, true);
                    idSM.moveToTop();
                    idSM.moveBelow(new ObjectIdCollection {
                        idWO
                    });
                    if (idsLdr.Count > 0)
                        idWO.moveBelow(idsLdr);
                    Color color = new Color();
                    color = Color.FromColorIndex(ColorMethod.ByLayer, 256);
                    idTX = Txt.addMText(value, pnt3dPos, angleView, 0.8, 0.09, AttachmentPoint.MiddleCenter, "Annotative", "BUBBLE", color, Pub.JUSTIFYCENTER);
                    br.ObjectId.delete();

                    Draw.addXData(idSM, scale, idTX, idsLdr, idWO, 1024, "0".stringToHandle(), "");
                    //DBObjectCollection dbObjs = new DBObjectCollection();
                    //ent = (Entity)br;
                    //ent.Explode(dbObjs);
                    //foreach(Autodesk.AutoCAD.DatabaseServices.DBObject dbObj in dbObjs){
                    //    Autodesk.AutoCAD.DatabaseServices.Entity e = (Autodesk.AutoCAD.DatabaseServices.Entity)dbObj;
                    //    ms.AppendEntity(e);
                    //    tr.AddNewlyCreatedDBObject(e, true);
                    //    if(e.GetType() == typeof(Circle)){
                    //        foreach (DBDictionaryEntry entry in groups)
                    //        {
                    //            ObjectId idGrp = entry.Value;
                    //            Group group = (Group)tr.GetObject(idGrp, (OpenMode.ForRead));
                    //            ObjectId[] ids = group.GetAllEntityIds();
                    //            List<ObjectId> lstIDs = new List<ObjectId>();
                    //            foreach (ObjectId id in ids)
                    //                lstIDs.Add(id);
                    //            if (lstIDs.Contains(idBR))
                    //            {
                    //                group.UpgradeOpen();
                    //                foreach (ObjectId idEnt in lstIDs)
                    //                {
                    //                    group.Remove(idEnt);
                    //                    idEnt.delete();
                    //                }
                    //                group.Erase();
                    //                break;
                    //            }
                    //        }
                    //    }
                    //    e.ObjectId.delete();
                    //}
                }
                tr.Commit();
            }

            SnapMode.setOSnap((int)osMode);
        }

        private static void
        checkGroup(ObjectId idBlock)
        {
            Database db = HostApplicationServices.WorkingDatabase;

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                DBDictionary groups = (DBDictionary)db.GroupDictionaryId.GetObject(OpenMode.ForRead);

                foreach (DBDictionaryEntry entry in groups)
                {
                    ObjectId idGrp = entry.Value;
                    Group group = (Group)tr.GetObject(idGrp, (OpenMode.ForRead));
                    ObjectId[] ids = group.GetAllEntityIds();
                    List<ObjectId> lstIDs = new List<ObjectId>();

                    foreach (ObjectId id in ids)
                        lstIDs.Add(id);

                    if (lstIDs.Contains(idBlock))
                    {
                        group.UpgradeOpen();
                        foreach (ObjectId idObj in lstIDs)
                        {
                            group.Remove(idObj);

                            if (idObj.GetType() == typeof(Wipeout))
                                idObj.delete();
                        }
                        group.Erase();
                        break;
                    }
                }
                tr.Commit();
            }
        }
    }
}