using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Bubble;
using DimPL;
using Grading;
using LdrText;
using System.Collections.Generic;

namespace EventManager
{
    public static class EM_StretchMove
    {
        public static void
        doStretchMove(List<EM_EData> enData)
        {
            foreach (EM_EData eData in enData)
            {
                List<ObjectId> ids = null;
                ObjectId idP3d, idLdr;
                List<Handle> handles = null;
                ObjectId idCgPnt, idMTxt;
                bool exists;

                TypedValue[] tvs = eData.tvs;
                string nameApp = tvs[0].Value.ToString();

                switch (nameApp)
                {
                    case apps.lnkGS:
                        switch (eData.objType)
                        {
                            case "CogoPoint":
                                idCgPnt = eData.id;
                                Grading_Utility.updateGS(tvs, "Grip_Stretch_Move");
                                break;

                            case "MText":
                                idMTxt = eData.id;
                                LdrText_Move.moveLdrGS(idMTxt, tvs, eData.pnt3d);
                                break;

                            case "Leader":
                                idLdr = eData.id;
                                LdrText_Move.moveTxtGS(idLdr, tvs);
                                break;
                        }
                        break;

                    case apps.lnkBrks:
                        ids = tvs.getObjectIdList();
                        handles = tvs.getHandleList();

                        foreach (Handle handle in handles)
                        {
                            idP3d = Grading_Utility.update3dPoly_lnkBrks1_2(apps.lnkBrks, eData.id, handle);
                            if (idP3d == ObjectId.Null)
                                handles.Remove(handle);
                        }
                        break;

                    case apps.lnkBrks2:
                        ids = tvs.getObjectIdList();
                        handles = tvs.getHandleList();
                        foreach (Handle handle in handles)
                        {
                            idP3d = Grading_Utility.update3dPoly_lnkBrks1_2(apps.lnkBrks2, eData.id, handle);
                            if (idP3d == ObjectId.Null)
                            {
                                handles.Remove(handle);
                            }
                        }
                        ObjectId idDictM = Dict.getNamedDictionary(apps.lnkBrks3, out exists);
                        ObjectId idDictPnt = Dict.getSubEntry(idDictM, eData.id.getHandle().ToString());

                        List<DBDictionaryEntry> entries = Dict.getEntries(idDictPnt);
                        foreach (DBDictionaryEntry entry in entries)
                        {
                            ObjectId idDictX = Dict.getSubEntry(idDictPnt, entry.Key);

                            ResultBuffer rb2 = Dict.getXRec(idDictX, "Offset");
                            tvs = rb2.AsArray();
                            double offset = (double)tvs[0].Value;

                            rb2 = Dict.getXRec(idDictX, "DeltaZ");
                            tvs = rb2.AsArray();
                            double deltaZ = (double)tvs[0].Value;

                            rb2 = Dict.getXRec(idDictX, "HandleFL");
                            tvs = rb2.AsArray();
                            Handle handleFL = (Handle)tvs[0].Value;

                            rb2 = Dict.getXRec(idDictX, "Beg");
                            tvs = rb2.AsArray();
                            double beg = (double)tvs[0].Value;

                            rb2 = Dict.getXRec(idDictX, "End");
                            tvs = rb2.AsArray();
                            double end = (double)tvs[0].Value;

                            Grading_Utility.update3dPoly_lnkBrks3(idDictX.getHandle(), offset, deltaZ, handleFL, beg, end);
                        }
                        break;

                    case apps.lnkBubs:
                        switch (eData.objType)
                        {
                            case "MText":
                                BB_Events.modTXlnkBubs(eData.id, tvs);
                                break;

                            case "Leader":
                                BB_Events.modLDRlnkBubs(eData.id, tvs);
                                break;
                        }
                        break;

                    case apps.lnkCO:
                    case apps.lnkLD:
                        switch (eData.objType)
                        {
                            case "CogoPoint":
                                idCgPnt = eData.id;
                                idMTxt = tvs.getObjectId(3);
                                ResultBuffer rbMTxt = idMTxt.getXData(nameApp);
                                TypedValue[] tvsMTxt = rbMTxt.AsArray();

                                idLdr = tvsMTxt.getObjectId(3);
                                Point3d pnt3dCgPoint = idCgPnt.getCogoPntCoordinates();
                                Point3d pnt3dLdr = idLdr.getBegPnt();
                                if (!pnt3dCgPoint.X.Equals(pnt3dLdr.X) || !pnt3dCgPoint.Y.Equals(pnt3dLdr.Y))
                                {
                                    idLdr.adjLdrBegPnt(pnt3dCgPoint);
                                }
                                Grading_Utility.updatePntCalloutElev(pnt3dCgPoint.Z, tvs);
                                break;

                            case "MText":
                                idMTxt = eData.id;
                                try
                                {
                                    LdrText_Move.moveTX(idMTxt, tvs, eData.pnt3d);
                                }
                                catch (System.Exception ex)
                                {
                BaseObjs.writeDebug(ex.Message + " EM_StretchMove.cs: line: 147");
                                }
                                break;

                            case "Leader":
                                idLdr = eData.id;
                                LdrText_Rotate.adjTxt(idLdr, nameApp);
                                break;
                        }
                        break;


                    //case apps.lnkLD:
                    //    switch (eData.objType)
                    //    {
                    //        case "CogoPoint":
                    //            break;

                    //        case "MText":
                    //            idMTxt = eData.id;
                    //            try
                    //            {
                    //                LdrText_Move.moveTX(idMTxt, tvs, eData.pnt3d);
                    //            }
                    //            catch (System.Exception ex)
                    //            {
                    //                BaseObjs.writeDebug(string.Format("{0} EM_StretchMove.cs: line: 138", ex.Message));
                    //            }
                    //            break;

                    //        case "Leader":
                    //            idLdr = eData.id;
                    //            LdrText_Rotate.adjTxt(idLdr, nameApp);
                    //            break;
                    //    }
                    //    break;

                    case apps.lnkDimPL:
                        if (eData.objType == "MText")
                        {
                            idMTxt = eData.id;
                            DimPL_Move.modTXlnkDimPl(idMTxt, tvs, idMTxt.getMTextLocation());
                        }
                        break;

                    case apps.lnkMNP:
                        ObjectId id = eData.id;
                        switch (eData.objType)
                        {
                            case "Pipe":
                                using (Transaction tr = BaseObjs.startTransactionDb())
                                {
                                    Pipe pipe = (Pipe)tr.GetObject(id, OpenMode.ForWrite);
                                    idCgPnt = tvs.getObjectId(1);
                                    CogoPoint cgPNt = (CogoPoint)tr.GetObject(idCgPnt, OpenMode.ForWrite);

                                    double dZ = pipe.InnerDiameterOrWidth / (2 * System.Math.Cos(System.Math.Atan(System.Math.Abs(pipe.Slope))));
                                    double inv = pipe.StartPoint.Z - dZ;
                                    cgPNt.Elevation = inv;

                                    ResultBuffer rb = idCgPnt.getXData(apps.lnkCO);
                                    if (rb != null)
                                    {
                                        TypedValue[] tvsCgPNt = rb.AsArray();

                                        Grading.Grading_Utility.updatePntCalloutElev(inv, tvsCgPNt);
                                        BaseObjs.updateGraphics();
                                    }

                                    idCgPnt = tvs.getObjectId(2);
                                    cgPNt = (CogoPoint)tr.GetObject(idCgPnt, OpenMode.ForWrite);
                                    inv = pipe.EndPoint.Z - dZ;
                                    cgPNt.Elevation = inv;

                                    rb = idCgPnt.getXData(apps.lnkCO);
                                    if (rb != null)
                                    {
                                        TypedValue[] tvsCgPNt = rb.AsArray();

                                        Grading.Grading_Utility.updatePntCalloutElev(inv, tvsCgPNt);
                                        BaseObjs.updateGraphics();
                                    }
                                    tr.Commit();
                                }

                                break;

                            case "Structure":
                                Structure s = (Structure)id.getEnt();
                                break;
                        }
                        break;

                }
            }
        }
    }
}
