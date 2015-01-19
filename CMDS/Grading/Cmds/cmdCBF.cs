using Autodesk.AECC.Interop.Land;
using Autodesk.AutoCAD.DatabaseServices;
using Base_Tools45;
using Base_VB;
using System;
using System.Collections.Generic;

namespace Grading.Cmds
{
    public static class cmdCBF
    {
        [STAThread]
        public static void
        CBF()
        {
            try
            {
                AeccDatabase db = BaseObjsCom.aeccDb;
               

                ObjectId idPoly3d = Select.getBrkLine("Select 3d Breakline");
                Handle hPoly3d = idPoly3d.getHandle();

                ResultBuffer rbPoly3d = idPoly3d.getXData(apps.lnkBrks);
                if (rbPoly3d == null)
                    return;
                List<Handle> hCgPnts = rbPoly3d.rb_handles();
                TypedValue[] tvsX = rbPoly3d.AsArray();
                List<string> nameApps = null;
                List<TypedValue[]> lstTVs = xData.parseXData(tvsX, out nameApps);

                AeccSites sites = db.Sites;
                AeccSite site = null;
                bool exists = false;

                foreach (AeccSite s in sites)
                {
                    if (s.Name == "Site1")
                    {
                        site = s;
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                    site = sites.Add("Site1");

                AeccLandFeatureLines features = site.FeatureLines;
                AeccFeatureLineStyles styles = db.FeatureLineStyles;
                AeccFeatureLineStyle style = styles[0];

                long idOldPoly3d = (long)idPoly3d.OldIdPtr;

                AeccLandFeatureLine feature = features.AddFromPolyline(idOldPoly3d, style);
                feature.Layer = idPoly3d.getLayer();



                short[] varType = null;
                object[] varVal = null;

                for (int i = 0; i < lstTVs.Count; i++)
                {
                    TypedValue[] tvs = lstTVs[i];
                    string nameApp = tvs[0].Value.ToString();
                    if (nameApp == apps.lnkBrks)
                    {
                        myUtility.tvsToList(tvs, ref varType, ref varVal);
                        feature.SetXData(varType, varVal);
                        break;
                    }
                }



                Polyline3d poly3d = (Polyline3d)idPoly3d.getEnt();

                foreach (Handle hCgPnt in hCgPnts)
                {
                    ObjectId idCgPnt = hCgPnt.getObjectId();
                    idCgPnt.replaceHandleInXdata(apps.lnkBrks, feature.Handle.stringToHandle(), poly3d.Handle);
                }

                rbPoly3d = idPoly3d.getXData(apps.lnkBrks3);
                if (rbPoly3d == null)
                    return;

                TypedValue[] tvsPoly3d = rbPoly3d.AsArray();
                ObjectId idPoly3dFL = tvsPoly3d[1].getObjectId();

                idPoly3dFL.replaceHandleInXdata(apps.lnkBrks3, feature.Handle.stringToHandle(), poly3d.Handle);

                varType = new short[2];
                varVal = new object[2];

                varType[0] = 1001;
                varVal[0] = apps.lnkBrks3;
                varType[1] = 1005;
                varVal[1] = idPoly3dFL.getHandle().ToString();

                feature.SetXData(varType, varVal);

                idPoly3d.delete();
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdCBF.cs: line: 107");
            }
        }
    }
}
