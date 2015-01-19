using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

using Base_Tools45;

using System;
using System.Collections.Generic;

namespace Bubble
{
    public static class BB_Events
    {
        #region obj

        private static Point3d _ldrEndPoint3D;

        public static Point3d LdrEndPoint3D
        {
            get
            {
                return _ldrEndPoint3D;
            }
            set
            {
                _ldrEndPoint3D = value;
            }
        }

        public static void
        activateObj(this ObjectId id)
        {
            id.activate<DBObject>(
                obj =>
                {
                    obj.Modified += new EventHandler(obj_Modified);
                });
        }

        public static void
        deactivateObj(this ObjectId id)
        {
            id.activate<DBObject>(
                obj =>
                {
                    obj.Modified -= new EventHandler(obj_Modified);
                });
        }

        private static void
        obj_Modified(object sender, EventArgs e)
        {
            DBObject dbObj = (DBObject)sender;
            if (dbObj.IsErased)
                return;
            ResultBuffer rb = dbObj.GetXDataForApplication(null);
            TypedValue[] tvs = rb.AsArray();
            List<string> nameApps = null;
            List<TypedValue[]> lstTVs = xData.parseXData(tvs, out nameApps);

            foreach (TypedValue[] tvsRet in lstTVs)
            {
                string nameApp = tvsRet[0].Value.ToString();
                string nameObj = tvsRet[1].Value.ToString();

                switch (nameApp)
                {
                    case "lnkBubs":

                        switch (nameObj)
                        {
                            case "TARGET":
                                break;

                            case "TX":
                                modTXlnkBubs(dbObj.ObjectId, tvsRet);
                                break;

                            case "SM":
                                break;

                            case "LDR":
                                modLDRlnkBubs(dbObj.ObjectId, tvsRet);
                                break;
                        }
                        break;

                    case "lnkBubsLdrEndPnt":
                        break;
                }
            }
        }

        public static void deleteSmWoLdrs(TypedValue[] tvsTX)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            string nameApp = tvsTX[0].Value.ToString();
            switch (nameApp)
            {
                case apps.lnkBubs:
                    Handle hSM = tvsTX[2].Value.ToString().stringToHandle();
                    ObjectId idSM = hSM.getObjectId();
                    ResultBuffer rbSM = null;
                    TypedValue[] tvsSM = null;
                    if (idSM.IsValid)
                    {
                        rbSM = idSM.getXData(apps.lnkBubs);
                        tvsSM = rbSM.AsArray();
                        ids.Add(idSM);
                    }

                    Handle hWO = tvsSM[2].Value.ToString().stringToHandle();
                    ObjectId idWO = hWO.getObjectId();
                    if (idWO.IsValid)
                    {
                        ids.Add(idWO);
                    }

                    for (int i = 6; i < tvsSM.Length; i++)
                    {
                        ObjectId idLDR = tvsSM[i].getObjectId();
                        if (idLDR.IsValid)
                        {
                            ids.Add((idLDR));
                        }
                    }
                    break;
            }

            foreach (ObjectId id in ids)
            {
                try
                {
                    id.delete();
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(ex.Message + " BB_Events.cs: line: 138");
                }
            }
        }

        public static void
        modTXlnkBubs(ObjectId idObj, TypedValue[] tvsTX)
        {
            Point3d pnt3dIns = idObj.getMTextLocation();

            Handle hSM = tvsTX[2].Value.ToString().stringToHandle();
            ObjectId idSM = hSM.getObjectId();

            ResultBuffer rbSM = idSM.getXData(apps.lnkBubs);
            TypedValue[] tvsSM = rbSM.AsArray();
            tvsSM = rbSM.AsArray();

            Handle hWO = tvsSM[2].Value.ToString().stringToHandle();
            ObjectId idWO = hWO.getObjectId();

            int numSides;
            int.TryParse(tvsSM[5].Value.ToString(), out numSides);

            Point3d pnt3dCEN = hSM.getCenter(numSides);

            ObjectIdCollection ids = new ObjectIdCollection();
            ids.Add(idSM);
            ids.Add(idWO);

            ids.moveObjs(pnt3dCEN, pnt3dIns);

            BB_Ldr.moveLdrEndPoint(tvsSM, pnt3dCEN, pnt3dIns, idSM);
        }

        public static void
        modLDRlnkBubs(ObjectId idLdr, TypedValue[] tvsLDR)
        {
            Point3d pnt3dV0 = idLdr.getBegPnt();
            string layerTarget = tvsLDR[5].Value.ToString();

            ObjectId[] ids = pnt3dV0.getEntitysAtPoint(layerTarget);
            switch (ids.Length)
            {
                case 0:
                    Application.ShowAlertDialog("Attention Walmart Shoppers: Target Entity's layer does not match original.  Exiting...");
                    return;

                case 1:
                    break;

                default:
                    Application.ShowAlertDialog("Attention Walmart Shoppers: More than one Target Entity at picked location. Exiting...");
                    break;
            }
        }

        private static void
        modLDRlnkBubsEndPnts(DBObject dbObj, TypedValue[] tvsLDR)
        {
            Leader ldr = (Leader)dbObj;
            Point3d pnt3dVX = ldr.LastVertex;
            Handle hSM = tvsLDR[3].Value.ToString().stringToHandle();
            ObjectId idSM = hSM.getObjectId();

            ObjectId[] ids = pnt3dVX.getEntitysAtPoint("TEXT");
            if (ids == null)
            {
                Application.ShowAlertDialog("Attention Walmart Shoppers: Last Vertex of Leader is no longer connected to the Symbol.  Exiting...");
                return;
            }

            switch (ids.Length)
            {
                case 0:
                    Application.ShowAlertDialog("Attention Walmart Shoppers: Last Vertex of Leader is no longer connected to the Symbol.  Exiting...");
                    return;

                case 1:

                    Ldr.setLdrXData(pnt3dVX, ldr.ObjectId, idSM);

                    break;

                default:
                    Application.ShowAlertDialog("Attention Walmart Shoppers: More than one Target Entity at picked location. Exiting...");
                    break;
            }
        }

        #endregion obj
    }// class BB_Events
}
