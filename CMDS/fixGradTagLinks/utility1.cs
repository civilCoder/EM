using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace fixGradeTagLinks
{
    public static class utility1
    {
        private static string topNum;
        private static string botNum;
        private static bool ex;
        private static bool skipBot;
        private static double elevTop;
        private static double elevBot;
        private static double elevDif;

        public static bool
        isValidObjInDictionary(ObjectId idBlkRef, List<DBDictionaryEntry> entries, string nameDict, ObjectId idDict, out bool fix, out Handle hPnt1)
        {
            bool isValid = false;
            fix = false;
            Point3d pnt3dPnt = Point3d.Origin;
            ObjectId idBlock = ObjectId.Null;
            ObjectId idLeader = ObjectId.Null;
            ObjectId idPnt1 = ObjectId.Null;
            hPnt1 = new Handle();
            ResultBuffer rb = null;
            TypedValue[] tvs = null;

            foreach (DBDictionaryEntry entry in entries)
            {
                try
                {
                    string shPnt1 = entry.Key;
                    hPnt1 = new Handle(Int64.Parse(shPnt1, NumberStyles.AllowHexSpecifier));

                    rb = Dict.getXRec(idDict, entry.Key);
                    if (rb == null)
                    {
                        Dict.deleteXRec(idDict, entry.Key);
                        continue;
                    }
                    tvs = rb.AsArray();
                    idBlock = tvs[0].Value.ToString().stringToHandle().getObjectId();
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex4)
                {
                    Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog(string.Format("{0} ex4", ex4.Message));
                }
                if (idBlock == idBlkRef)
                {
                    try
                    {
                        idLeader = tvs[1].Value.ToString().stringToHandle().getObjectId();

                        if (idLeader.IsErased || idLeader.IsEffectivelyErased)
                        {
                            Dict.deleteXRec(idDict, hPnt1.ToString());
                            fix = true;
                            return isValid = false;
                        }
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception ex0)
                    {
                        Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog(string.Format("{0} @ex0", ex0.Message));
                    }

                    try
                    {
                        idPnt1 = hPnt1.getObjectId();
                        pnt3dPnt = idPnt1.getCogoPntCoordinates();
                        Point3d pnt3dLdrBeg = idLeader.getBegPnt();
                        Vector3d v3d = new Vector3d(0, 0, 0);

                        Point3d pnt2dPnt = new Point3d(pnt3dPnt.X, pnt3dPnt.Y, 0);
                        Point3d pnt2dLdrBeg = new Point3d(pnt3dLdrBeg.X, pnt3dLdrBeg.Y, 0);

                        string hBlkRef = idBlkRef.getHandle().ToString();
                        if (pnt2dPnt - pnt2dLdrBeg != v3d)
                        {
                            fix = true;                                                                             //needs fixing
                            //Debug.Print(string.Format("{0}  {1}", hBlkRef, (pnt2dPnt - pnt3dLdrBeg).ToString()));
                        }
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception ex1)
                    {
                        Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog(string.Format("{0} @ex1", ex1.Message));
                    }//idLeader.moveLdrBegPoint(pnt3dLdrBeg, pnt3dPnt, pnt3dLdrBeg);

                    string desc = "";

                    try
                    {
                        using (BaseObjs._acadDoc.LockDocument())
                        {
                            switch (nameDict)
                            {
                                case "GradeTagDict":
                                    desc = Blocks.getBlockRefAttributeValue(idBlkRef, "TOPTXT");
                                    desc = desc.Replace(")", "");
                                    switch (desc)
                                    {
                                        case "FF":
                                            checkFF(idBlkRef, pnt3dPnt);
                                            return isValid = true;

                                        case "TC":
                                            checkTC(idBlkRef, pnt3dPnt);
                                            return isValid = true;
                                    }
                                    return true;

                                case "FlTagDict":
                                    checkFL(idBlkRef, pnt3dPnt);
                                    return isValid = true;
                            }
                        }
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception ex2)
                    {
                        Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog(string.Format("{0} @ex2", ex2.Message));
                    }
                    return isValid;
                }
                isValid = false;
            }
            return isValid;
        }

        public static void
        checkFF(ObjectId idBlkRef, Point3d pnt3dPnt)
        {
            topNum = Blocks.getBlockRefAttributeValue(idBlkRef, "TOPNUM");
            //Debug.Print(topNum);
            if (topNum.Contains("("))
                return;
            elevTop = Double.Parse(topNum, NumberStyles.Number);
            if (System.Math.Abs(elevTop - pnt3dPnt.Z) > 0.005)
            {
                botNum = Blocks.getBlockRefAttributeValue(idBlkRef, "BOTNUM");
                elevBot = double.Parse(botNum, NumberStyles.Number);
                elevDif = elevTop - elevBot;
                idBlkRef.updateAttribute("TOPNUM", string.Format("{0:F2}", pnt3dPnt.Z));
                elevBot = pnt3dPnt.Z - elevDif;
                idBlkRef.updateAttribute("BOTNUM", string.Format("{0:F2}", elevBot));
            }
        }

        public static void
        checkTC(ObjectId idBlkRef, Point3d pnt3dPnt)
        {
            skipBot = false;
            ex = false;
            botNum = Blocks.getBlockRefAttributeValue(idBlkRef, "BOTNUM");
            //Debug.Print(botNum);
            if (botNum.Contains("\""))
            {
                skipBot = true;
            }
            else if (botNum.Contains("("))
            {
                botNum = botNum.Replace("(", "");
                ex = true;
            }
            if (!skipBot)
            {
                elevBot = double.Parse(botNum, NumberStyles.Number);
                if (System.Math.Abs(elevBot - pnt3dPnt.Z) > 0.005)
                {
                    topNum = Blocks.getBlockRefAttributeValue(idBlkRef, "TOPNUM");
                    //Debug.Print(topNum);
                    if (topNum.Contains("("))
                    {
                        topNum = topNum.Replace("(", "");
                        ex = true;
                    }
                    elevTop = double.Parse(topNum, NumberStyles.Number);

                    if (!skipBot)
                    {
                        elevDif = elevTop - elevBot;
                        if (!ex)
                            idBlkRef.updateAttribute("BOTNUM", string.Format("{0:F2}", pnt3dPnt.Z));
                        else
                            idBlkRef.updateAttribute("BOTNUM", string.Format("({0:F2}", pnt3dPnt.Z));
                        elevTop = pnt3dPnt.Z + elevDif;
                        if (!ex)
                            idBlkRef.updateAttribute("TOPNUM", string.Format("{0:F2}", elevTop));
                        else
                            idBlkRef.updateAttribute("TOPNUM", string.Format("({0:F2}", elevTop));
                    }
                }
            }
        }

        public static void
        checkFL(ObjectId idBlkRef, Point3d pnt3dPnt)
        {
            ex = false;
            topNum = Blocks.getBlockRefAttributeValue(idBlkRef, "TOPNUM");
            Debug.Print(idBlkRef.getHandle().ToString());
            Debug.Print(topNum);
            if (topNum.Contains("("))
            {
                topNum = topNum.Replace("(", "");
                ex = true;
            }
            else
            {
                elevTop = double.Parse(topNum, NumberStyles.Any);
            }

            if (System.Math.Abs(elevTop - pnt3dPnt.Z) > 0.005)
            {
                if (ex)
                    idBlkRef.updateAttribute("TOPNUM", string.Format("({0:F2}", pnt3dPnt.Z));
                else
                    idBlkRef.updateAttribute("TOPNUM", string.Format("{0:F2}", pnt3dPnt.Z));
            }
        }
    }
}