using Autodesk.AutoCAD.DatabaseServices;

using Base_Tools45;
using Bubble;

using System.Collections.Generic;

namespace EventManager
{
    public static class EM_Delete
    {
        public static void
        doDelete(List<EM_EData> enData)
        {
            List<Handle> hDeletes = new List<Handle>();
            foreach(var edata in enData)
                hDeletes.Add(edata.h);

            bool pass = false;
            foreach (EM_EData eData in enData)
            {
                if (pass)
                    return;
                List<ObjectId> ids = null;
                ObjectId idEnt, idDict, idTxTop, idTxBot, idTxBot2, idLdr, idLdr1, idLdr2, idCgPnt1, idCgPnt2;
                List<Handle> handles = null;

                TypedValue[] tvs = eData.tvs;

                string nameApp = tvs[0].Value.ToString();
                string nameObj, nameCmd;
                ObjectId id = ObjectId.Null;
                bool exists;
                TypedValue[] tvsTx;

                try
                {
                    switch (nameApp)
                    {
                        case "AVG":
                            ObjectId idDictGRADEDOCK = Dict.getNamedDictionary("GRADEDOCK", out exists);
                            if (!exists)
                                continue;
                            Dict.delSubDict(idDictGRADEDOCK, tvs[2].Value.ToString());
                            break;

                        case apps.lnkBubs:
                            nameObj = tvs[1].Value.ToString();
                            if (nameObj == "TX")
                            {
                                BB_Events.deleteSmWoLdrs(tvs);
                            }
                            break;

                        case apps.lnkCO:
                            try
                            {
                                nameCmd = tvs[1].Value.ToString();
                                nameObj = tvs[2].Value.ToString();
                                switch (nameObj)
                                {
                                    case "TX":
                                        idLdr = tvs.getObjectId(3);
                                        idTxBot = tvs.getObjectId(4);
                                        idTxBot2 = tvs.getObjectId(5);
                                        idCgPnt1 = tvs.getObjectId(10);
                                        xData.removeCmdXDataFromCogoPoint(idCgPnt1, nameApp, nameCmd);           //remove xdata from point for this entry
                                        idLdr.delete();
                                        idTxBot.delete();
                                        idTxBot2.delete();
                                        break;

                                    case "LDR":
                                        idTxTop = tvs.getObjectId(3);
                                        idTxBot = tvs.getObjectId(4);
                                        idTxBot2 = tvs.getObjectId(5);
                                        tvsTx = idTxTop.getXData(nameApp).AsArray();
                                        idCgPnt1 = tvsTx.getObjectId(10);
                                        xData.removeCmdXDataFromCogoPoint(idCgPnt1, nameApp, nameCmd);           //remove xdata from point for this entry
                                        idTxTop.delete();
                                        idTxBot.delete();
                                        idTxBot2.delete();
                                        break;

                                    case "CogoPoint":
                                        foreach (TypedValue tv in tvs)
                                        {
                                            if (tv.TypeCode == 1005)
                                            {
                                                idTxTop = tv.Value.ToString().stringToHandle().getObjectId();
                                                ResultBuffer rb = idTxTop.getXData(nameApp);
                                                TypedValue[] tvsTxTop = rb.AsArray();
                                                tvsTxTop.deleteLinkedEnts();
                                                idTxTop.delete();
                                            }
                                        }
                                        break;
                                }
                                if (eData.objType == "MText")
                                {
                                    idDict = Dict.getNamedDictionary(nameApp, out exists);
                                    Dict.delSubDict(idDict, eData.h.ToString());
                                }
                            }
                            catch (System.Exception ex)
                            {
                BaseObjs.writeDebug(ex.Message + " EM_Delete.cs: line: 107");
                            }
                            break;

                        case apps.lnkDimPL:
                            idLdr1 = tvs.getObjectId(11);
                            idLdr2 = tvs.getObjectId(12);
                            idLdr1.delete();
                            idLdr2.delete();
                            idDict = Dict.getNamedDictionary(nameApp, out exists);
                            Dict.delSubDict(idDict, eData.h.ToString());
                            break;

                        case apps.lnkGS:
                            if (eData.objType == "CogoPoint")
                            {
                                for (int i = 2; i < tvs.Length; i++)
                                {
                                    idTxTop = tvs.getObjectId(i);
                                    ResultBuffer rbTxt = idTxTop.getXData(apps.lnkGS);
                                    idTxTop.delete();
                                    if (rbTxt == null)
                                        continue;
                                    TypedValue[] tvsTxt = rbTxt.AsArray();
                                    idLdr = tvsTxt[3].getObjectId();
                                    idLdr.delete();
                                    handles = new List<Handle>();
                                    handles.Add(tvsTxt[9].getObjectId().getHandle());
                                    handles.Add(tvsTxt[10].getObjectId().getHandle());
                                    handles.Remove(eData.h);
                                    idCgPnt1 = handles[0].getObjectId();
                                    idCgPnt1.removeHandleFromXDataGS(apps.lnkGS, eData.h);
                                }
                            }
                            if (eData.objType == "MText")
                            {
                                idLdr = tvs.getObjectId(3);
                                idLdr.delete();
                                idCgPnt1 = tvs.getObjectId(9);
                                idCgPnt1.removeHandleFromXDataGS(apps.lnkGS, eData.h);
                                idCgPnt2 = tvs.getObjectId(10);
                                idCgPnt2.removeHandleFromXDataGS(apps.lnkGS, eData.h);
                            }

                            break;

                        case apps.lnkDP:
                            break;

                        case apps.lnkLD:
                            try
                            {
                                if (tvs[2].Value.ToString() == "TX")
                                {
                                    idLdr = tvs.getObjectId(3);
                                    idTxBot = tvs.getObjectId(4);
                                    if (!idLdr.IsErased && idLdr != ObjectId.Null)
                                        idLdr.delete();
                                    if (!idTxBot.IsErased && idTxBot != ObjectId.Null)
                                        idTxBot.delete();
                                    idDict = Dict.getNamedDictionary(nameApp, out exists);
                                    Dict.delSubDict(idDict, eData.h.ToString());
                                }
                                if (tvs[2].Value.ToString() == "LDR")
                                {
                                    idTxTop = tvs.getObjectId(3);
                                    if (!idTxTop.IsErased && idTxTop != ObjectId.Null)
                                    {
                                        Handle h = idTxTop.getHandle();
                                        idTxTop.delete();
                                        idDict = Dict.getNamedDictionary(nameApp, out exists);
                                        Dict.delSubDict(idDict, h.ToString());
                                    }
                                    idTxBot = tvs.getObjectId(4);
                                    if (!idTxBot.IsErased && idTxBot != ObjectId.Null)
                                    {
                                        idTxBot.delete();
                                    }
                                }
                            }
                            catch (System.Exception ex)
                            {
                BaseObjs.writeDebug(ex.Message + " EM_Delete.cs: line: 189");
                            }
                            break;

                        case apps.lblPnts:
                        case apps.lblPntsPT:
                            try
                            {
                                switch (eData.objType)
                                {
                                    case "BlockReference":
                                        tvs.deleteLinkedEnts();
                                        //**************need to remove BlockReference handle from point******************
                                        break;

                                    case "Leader":
                                        tvs.deleteLinkedEnts();
                                        break;

                                    case "CogoPoint":
                                        ObjectId idBr = tvs.getObjectId(1);
                                        if (idBr == ObjectId.Null)
                                            return;
                                        ResultBuffer rbBr = idBr.getXData(apps.lblPnts);      //= Point XData -> Point links BlockReference / BlockReference links Leader
                                        TypedValue[] tvsBr = rbBr.AsArray();
                                        idLdr = tvsBr.getObjectId(1);          //linked handle is in position 1 for lblPnts and lblPntsPT
                                        idBr.delete();
                                        idLdr.delete();
                                        break;
                                }
                            }
                            catch (System.Exception ex)
                            {
                BaseObjs.writeDebug(ex.Message + " EM_Delete.cs: line: 222");
                            }
                            break;

                        case apps.lnkBrks:
                            try
                            {
                                ids = tvs.getObjectIdList();
                                if (ids.Count == 0)
                                    continue;

                                //MessageBox.Show(ids[0].getType());
                                if (eData.objType == "CogoPoint") //Deleted ent was a CogoPoint
                                { 
                                    foreach (ObjectId idX in ids) //idX is poly3d(s) linked to CogoPoint
                                    { 
                                        if (!idX.IsValid || idX.IsErased)
                                            continue;
                                        foreach (EM_EData eDataX in enData)
                                        {
                                            if (eDataX.tvs[0].Value.ToString() == apps.lnkBrks3)
                                            {
                                                ObjectId idPoly3d = eDataX.tvs[1].getObjectId();
                                                List<Handle> hBrks = idPoly3d.getXData(apps.lnkBrks).rb_handles();   //CogoPoints on each end of BB or GT
                                                List<Handle> hBrks3 = idPoly3d.getXData(apps.lnkBrks3).rb_handles(); //FL handle

                                                Handle hFL = hBrks3[0];

                                                Handle hCgPnt1 = hBrks[0];
                                                Handle hCgPnt2 = hBrks[1];

                                                ObjectId idFL = hFL.getObjectId();

                                                idFL.removeHandleFromXdata(apps.lnkBrks3, hCgPnt1);                   //remove CogoPoint from FL.lnkBrks3
                                                idFL.removeHandleFromXdata(apps.lnkBrks3, hCgPnt2);                   //remove CogoPoint from FL.lnkBrks3
                                                idFL.removeHandleFromXdata(apps.lnkBrks3, idPoly3d.getHandle());      //remove poly3d from FL.lnkBrks3

                                                hBrks.Remove(eData.h);                      //remove deleted CogoPoint from list leaving CogoPoint at opposite end of poly3d
                                                hBrks[0].getObjectId().delete();           //delete CogoPoint at opposite end

                                                ResultBuffer rbSLP = idPoly3d.getXData(apps.lnkSLP);
                                                if (rbSLP != null)
                                                {
                                                    TypedValue[] tvsSLP = rbSLP.AsArray();
                                                    List<ObjectId> idsSLP = tvsSLP.getObjectIdList();
                                                    foreach (ObjectId idSLP in idsSLP)
                                                        idSLP.delete();
                                                }

                                                idPoly3d.delete();                          //delete poly3d
                                                pass = true;
                                            }
                                        }
                                        if (!pass)
                                        {
                                            Handle hP3d = idX.getHandle();                          //poly3d handle attached to CogoPoint
                                            ResultBuffer rbX = idX.getXData(apps.lnkBrks);          //path to other points linked to poly3d which is going to be deleted
                                            List<Handle> hPnts = rbX.rb_handles();                  //handles of points linked to poly3d including point to be deleted
                                            foreach (Handle h in hPnts)
                                            {
                                                if (h == eData.h)
                                                    continue;
                                                ObjectId idT = h.getObjectId();                     //id of other point linked to poly3d
                                                if (!idT.IsValid || idT.IsEffectivelyErased || idT.IsErased)
                                                    continue;

                                                if (hDeletes.Contains(h))
                                                    continue;
                                                
                                                ResultBuffer rbT = idT.getXData(apps.lnkBrks);
                                                if (rbT == null)
                                                    continue;
                                                List<Handle> hPolys = rbT.rb_handles();            //list of poly3d linked to other point
                                                if (hPolys.Contains(hP3d))
                                                    hPolys.Remove(hP3d);                           //remove poly3d handle from other point list
                                                if (hPolys.Count > 0)
                                                    idT.setXData(hPolys, apps.lnkBrks);
                                                else
                                                    idT.clearXData(apps.lnkBrks);
                                            }
                                            idX.delete();
                                        }
                                    }
                                }
                                else if (eData.objType == "Polyline3d" || eData.objType == "FeatureLine")
                                { //Deleted ent was a Polyline3d or Featureline
                                    ids = tvs.getObjectIdList();                                              //points linked to poly3d or feature line
                                    foreach (ObjectId idP in ids)
                                    {
                                        if (idP.IsValid && !idP.IsEffectivelyErased && !idP.IsErased)
                                        {
                                            ResultBuffer rbP = idP.getXData(apps.lnkBrks);
                                            if (rbP == null)
                                                continue;
                                            List<Handle> hPnts = rbP.rb_handles();
                                            if (hPnts.Contains(eData.h))
                                                hPnts.Remove(eData.h);
                                            if (hPnts.Count > 0)
                                                idP.setXData(hPnts, apps.lnkBrks2);
                                            else
                                                idP.clearXData(apps.lnkBrks);
                                        }
                                    }
                                }
                            }
                            catch (System.Exception ex)
                            {
                BaseObjs.writeDebug(ex.Message + " EM_Delete.cs: line: 329");
                            }

                            break;

                        case apps.lnkBrks2:
                            try
                            {
                                ids = tvs.getObjectIdList();                            //FL stores end Cogo Points / Cogo Points store FL
                                if (ids.Count == 0)
                                    continue;
                                idEnt = ids[0];                                         //idEnt is either a CogoPoint or a poly3d
                                //COGOPOINT DELETED
                                if (eData.objType == "CogoPoint")
                                { //Deleted ent was a CogoPoint - get lnkBrks3 from poly3d (FL) and delete all entities listed
                                    ResultBuffer rbFL2 = idEnt.getXData(apps.lnkBrks2);
                                    if (rbFL2 == null)
                                        continue;
                                    List<Handle> hCgPnts = rbFL2.rb_handles();
                                    if (hCgPnts.Contains(eData.h))
                                        hCgPnts.Remove(eData.h);                        //remove deleted point leaving point at other end
                                    if (hCgPnts.Count == 1)
                                    {
                                        ObjectId idCgPntX = hCgPnts[0].getObjectId();
                                        updateCogoPointDict3(idCgPntX, idEnt.getHandle());
                                        idCgPntX.removeHandleFromXdata(apps.lnkBrks2, idEnt.Handle);
                                    }

                                    ResultBuffer rbP3d3 = idEnt.getXData(apps.lnkBrks3);
                                    if (rbP3d3 == null)
                                        continue;

                                    TypedValue[] tvsP3D3 = rbP3d3.AsArray();
                                    List<ObjectId> idsEdge = tvsP3D3.getObjectIdList();
                                    foreach (ObjectId idEdge in idsEdge)
                                        idEdge.delete();
                                    idEnt.delete();                                         //delete FL
                                    //POLYLINE3D DELETED
                                }
                                else if (eData.objType == "Polyline3d")
                                { //Deleted ent was a Polyline3d   !!!!!!!!!!!!!!!!!!!!!!! SAME AS lnkBrks !!!!!!!!!!!!!!!!!!!!!!!!
                                    foreach (ObjectId idP3d in ids)
                                    {
                                        ResultBuffer rbP = idP3d.getXData(apps.lnkBrks2);
                                        if (rbP == null)
                                            continue;
                                        handles = rbP.rb_handles();
                                        if (handles.Contains(eData.h))                      // clean up xdata on points; lnkBrks3 will take care of edges
                                            handles.Remove(eData.h);
                                        if (handles.Count > 0)
                                            idP3d.setXData(handles, apps.lnkBrks2);
                                        else
                                            idP3d.clearXData(apps.lnkBrks2);
                                        updateCogoPointDict3(idP3d, eData.h);
                                    }
                                }
                            }
                            catch (System.Exception ex)
                            {
                BaseObjs.writeDebug(ex.Message + " EM_Delete.cs: line: 388");
                            }
                            break;

                        case apps.lnkBrks3:
                            try
                            {
                                ids = tvs.getObjectIdList();
                                switch (ids.Count)
                                {
                                    case 0:
                                        break;

                                    case 1:
                                        ids[0].removeHandleFromXdata(apps.lnkBrks3, eData.h);
                                        break;

                                    default:
                                        foreach (ObjectId idP in ids)
                                        {
                                            if (idP.getType() == "CogoPoint")
                                            {
                                                ResultBuffer rbX = null;
                                                rbX = idP.getXData(apps.lnkBrks);
                                                if (rbX != null)
                                                {
                                                    TypedValue[] tvsX = rbX.AsArray();
                                                    List<ObjectId> idsX = tvsX.getObjectIdList();
                                                    foreach (ObjectId idX in idsX)
                                                        idX.delete();
                                                }
                                            }
                                            idP.delete();
                                        }
                                        break;
                                }
                            }
                            catch (System.Exception ex)
                            {
                BaseObjs.writeDebug(ex.Message + " EM_Delete.cs: line: 427");
                            }
                            break;

                        case apps.lnkSLP:
                            ids = tvs.getObjectIdList();
                            foreach (ObjectId idSLP in ids)
                                idSLP.delete();
                            break;
                    }
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " EM_Delete.cs: line: 440");
                }
            }
        }

        private static bool updateCogoPointDict3(ObjectId idPnt, Handle hFL)
        {
            bool exists = false;
            ObjectId idDictM = Dict.getNamedDictionary(apps.lnkBrks3, out exists);        //dictionary holds info for edge parameters, offset, deltaZ, beg, end
            if (!exists)
                return false;
            ObjectId idDictPnt = Dict.getSubEntry(idDictM, idPnt.getHandle().ToString());
            List<DBDictionaryEntry> entries = idDictPnt.getDictEntries();
            Dict.removeSubEntry(idDictM, hFL.ToString());
            return false;
        }
    }
}
