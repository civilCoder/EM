using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Point3d = Autodesk.AutoCAD.Geometry.Point3d;

namespace Base_Tools45
{
    public static class BrkLine
    {
        public static ObjectId
        makeBreakline(string nameApp, string nameCmd, out ObjectId idPoly, 
                      List<ObjectId> idCogoPnts = null, string pntDesc = "CPNT-ON", List<Point3d> pnts3dL = null)
        {
            idPoly = ObjectId.Null;

            Point3d pnt3dBase = Pub.pnt3dO;
            Point3d pnt3dTemp = Pub.pnt3dO;

            Point3dCollection pnts3d = new Point3dCollection();

            ObjectId idPoly3d = ObjectId.Null;
            ObjectId idCgPnt = ObjectId.Null;

            string prompt = string.Empty;
            string nameLayer = string.Empty;

            List<ObjectId> idLines = new List<ObjectId>();
            bool escape = false;
            PromptStatus ps;

            try
            {
                if (idCogoPnts == null)
                {
                    idCogoPnts = new List<ObjectId>();

                    bool exitLoop = false;
                    do
                    {
                        int pntCount = idCogoPnts.Count;
                        switch (pntCount)
                        {
                            case 0:
                                prompt = "\nSelect begin point:";
                                break;

                            default:
                                prompt = "\nSelect point:";
                                pnt3dTemp = pnt3dBase;
                                break;
                        }

                        if (nameCmd == "cmdBV")
                        {
                            string resElev = "";
                            resElev = UserInput.getCogoPoint("\nSelect Begin Point: ", out idCgPnt, Pub.pnt3dO, osMode: 8);
                            if (idCgPnt == ObjectId.Null)
                                return ObjectId.Null;

                            idCogoPnts.Add(idCgPnt);
                            Point3d pnt3d1 = idCgPnt.getCogoPntCoordinates();

                            resElev = UserInput.getCogoPoint("\nSelect End Point: ", out idCgPnt, pnt3d1, osMode: 8);
                            if (idCgPnt == ObjectId.Null)
                                return ObjectId.Null;

                            idCogoPnts.Add(idCgPnt);
                            pntDesc = cleanPntDesc(idCgPnt.getCogoPntDesc());
                            exitLoop = true;
                        }
                        else
                        {
                            pnt3dBase = UserInput.getPoint(prompt, pnt3dTemp, out escape, out ps, osMode: 8);

                            if (!escape && pnt3dBase != Pub.pnt3dO)
                            {
                                pnts3d.Add(pnt3dBase);
                                BaseObjs._db.forEachMS<CogoPoint>(cg =>
                                {
                                    if (cg.Location == pnt3dBase)
                                    {
                                        idCgPnt = cg.ObjectId;
                                    }
                                });

                                if (idCgPnt == ObjectId.Null)
                                {
                                    if (pntCount < 2)
                                    {
                                        return ObjectId.Null;
                                    }
                                    else
                                    {
                                        exitLoop = true;
                                    }
                                }
                                else
                                {
                                    idCogoPnts.Add(idCgPnt);
                                    pntCount = idCogoPnts.Count;
                                    pntDesc = cleanPntDesc(idCgPnt.getCogoPntDesc());
                                    if (pntCount > 1)
                                    {
                                        if (nameCmd == "cmdMBL" || nameCmd == "cmdGBM")
                                        {
                                            ObjectId idLine = Base_Tools45.Draw.addLine(pnt3dTemp, pnt3dBase);
                                            idLines.Add(idLine);
                                        }
                                    }

                                    if (nameCmd != "cmdMBL" && nameCmd != "cmdGBM")
                                    {
                                        if (pntCount == 2)
                                        {
                                            exitLoop = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (pntCount < 2)
                                {
                                    return ObjectId.Null;
                                }
                                else
                                {
                                    exitLoop = true;
                                    if (nameCmd == "cmdMBL" || nameCmd == "cmdGBM")
                                    {
                                        Misc.deleteObjs(idLines);
                                    }
                                }
                            }
                        }
                    }
                    while (exitLoop == false);
                }
                else//----------CogoPoints passed to method - need to determine Description based on first point-----------------
                {
                    var pg = from data in CgPnt_Group.pntGroups       //Linq
                             where data.name == pntDesc
                             select data;

                    if (pg != null)
                        pntDesc = cleanPntDesc(idCogoPnts[0].getCogoPntDesc());
                }

                //------------------------------------determine layer name-------------------------------------------------

                if (pntDesc == "CPNT-ON")
                {
                    nameLayer = "CPNT-BRKLINE";
                }
                else
                {
                    var pg = from data in CgPnt_Group.pntGroups
                             where data.name == pntDesc
                             select data;

                    if (pg != null)
                        pntDesc = cleanPntDesc(pntDesc);
                    nameLayer = string.Format("{0}-BRKLINE", pntDesc);
                }

                //-------------------------------------------------------------------------------------------------------------

                Layer.manageLayers(nameLayer);

                //-------------------------------------------------------------------------------------------------------------

                if (nameCmd != "cmdMBL" && nameCmd != "cmdABC" && nameCmd != "cmdABG")
                {
                    switch (nameCmd)
                    {
                        case "cmdBV":
                        case "cmdFL":
                            idPoly = idCogoPnts.addPoly("FL");
                            break;

                        case "cmdGB":
                            idPoly = idCogoPnts.addPoly("GB");
                            break;
                        case "cmdGBM":
                            for (int i = 1; i < idCogoPnts.Count; i++ ){
                                List<ObjectId> idCgPnts = new List<ObjectId>() { idCogoPnts[i - 1], idCogoPnts[i] };
                                idPoly = idCgPnts.addPoly("GB");
                            }
                            break;
                        case "cmdRF":
                            idPoly = idCogoPnts.addPoly("GUTTER");  //cmdBV edge of gutter as reference
                            break;
                    }

                    using (BaseObjs._acadDoc.LockDocument())
                    {
                        idPoly3d = idCogoPnts.addPoly3d(nameLayer);
                        Layer.manageLayer(nameLayer, 171, false, false);
                    }

                    xData.lnkPntsAndPoly3d(idPoly3d, idCogoPnts[0], idCogoPnts[1], nameApp);        //adds poly3d to each cogo point and both points to poly3d
                }
                else if (nameCmd == "cmdMBL" || nameCmd == "cmdGBM")
                {
                    idPoly3d = pnts3d.addPoly3d(nameLayer);
                    Layer.manageLayer(nameLayer, 171, false, false);

                    bool isClosed = idPoly3d.checkIfClosed3d(false);
                    if (isClosed)
                        idCogoPnts.RemoveAt(idCogoPnts.Count - 1);

                    for (int i = 0; i < idCogoPnts.Count; i++)
                    {
                        idCogoPnts[i].updatePntXData(idPoly3d, nameApp);
                    }

                    TypedValue[] TVs = new TypedValue[idCogoPnts.Count + 1];
                    TVs.SetValue(new TypedValue(1001, apps.lnkBrks), 0);
                    for (int i = 0; i < idCogoPnts.Count; i++)
                    {
                        TVs.SetValue(new TypedValue(1005, idCogoPnts[i].getHandle()), i + 1);
                    }
                    idPoly3d.setXData(TVs, apps.lnkBrks);
                }
                else if (nameCmd == "cmdABC" || nameCmd == "cmdABG")
                {
                    idPoly3d = pnts3dL.addPoly3d(nameLayer);
                    Layer.manageLayer(nameLayer, 171, false, false);

                    if (nameApp == apps.lnkBrks2)
                    {
                        for (int i = 0; i < idCogoPnts.Count; i++)
                        {
                            idCogoPnts[i].updatePntXData(idPoly3d, nameApp);
                        }
                    }

                    //TypedValue[] TVs = new TypedValue[idCogoPnts.Count + 1];
                    //TVs.SetValue(new TypedValue(1001, nameApp), 0);
                    //for (int i = 0; i < idCogoPnts.Count; i++)
                    //{
                    //    TVs.SetValue(new TypedValue(1005, idCogoPnts[i].getHandle()), i + 1);
                    //}
                    //idPoly3d.setXData(TVs, nameApp);
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " BrkLine.cs: line: 254");
            }
            return idPoly3d;
        }

        public static string
        cleanPntDesc(string strPntDesc)
        {
            string strBuf = strPntDesc;
            for (int i = 0; i < strPntDesc.Length; i++)
            {
                string strBufChar = strPntDesc.Substring(i, 1);
                byte[] asciiBytes = Encoding.ASCII.GetBytes(strBufChar);

                if (asciiBytes[0] > 47 & asciiBytes[0] < 58 || asciiBytes[0] == 32)
                {
                    strBuf = strPntDesc.Substring(1, i - 1);
                }
            }
            return strBuf;
        }
    }
}
