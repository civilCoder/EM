using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;

using Autodesk.Civil.DatabaseServices;

using Base_Tools45;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Grading.Cmds
{
    public static class cmdUBP
    {
        private static Color color;

        public static void
        updateBrkLines_Pnts()
        {
            int count1 = 0;
            int count2 = 0;
            int count3 = 0;
            int count4 = 0;
            int n = 0;
            try
            {
                ObjectIdCollection ids = BaseObjs._acadDoc.getBrkLines();
                color = new Color();
                color = Color.FromColorIndex(ColorMethod.ByBlock, 100);
                foreach (ObjectId idBrkLine in ids)
                {
                    Debug.Print(idBrkLine.getHandle().ToString());
                    List<TypedValue[]> lstTVs = null;
                    try
                    {
                        ResultBuffer RB_BrkLine = idBrkLine.getXData(null);
                        TypedValue[] tvsX = null;
                        if (RB_BrkLine != null)
                        {
                            tvsX = RB_BrkLine.AsArray();
                            List<string> nameApps = null;
                            lstTVs = xData.parseXData(tvsX, out nameApps);

                            var result = apps.lstApps.Intersect(nameApps);

                            if (result.Count<string>() == 0)
                            {
                                idBrkLine.changeProp(color, "BRKLINE-ERROR", LineWeight.LineWeight200);
                                count1++;
                                continue;
                            }
                        }
                        else
                        {
                            idBrkLine.changeProp(color, "BRKLINE-ERROR", LineWeight.LineWeight200);
                            count1++;
                            continue;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(ex.Message + " cmdUBP.cs: line: 66");
                    }

                    Color color7 = new Color();
                    color7 = Color.FromColorIndex(ColorMethod.ByBlock, 7);

                    Color color21 = new Color();
                    color21 = Color.FromColorIndex(ColorMethod.ByBlock, 21);

                    foreach (TypedValue[] tvs in lstTVs)
                    {
                        if (tvs[0].Value.ToString() != apps.lnkBrks)
                            continue;

                        Point3dCollection pnts3d = idBrkLine.getCoordinates3d();
                        if (pnts3d != null)
                        {
                            List<Handle> handlesBRk = tvs.getHandleList();
                            for (int i = 0; i < handlesBRk.Count; i++)
                            {
                                Handle handle = handlesBRk[i];
                                if (handle.ToString() != "0")
                                {
                                    CogoPoint cogoPnt = (CogoPoint)handle.getEnt();
                                    if (cogoPnt != null)
                                    {
                                        Point3d pnt3d = pnts3d[i];
                                        if (System.Math.Round(pnt3d.X, 3) == System.Math.Round(cogoPnt.Easting, 3) &&
                                            System.Math.Round(pnt3d.Y, 3) == System.Math.Round(cogoPnt.Northing, 3))
                                        {
                                            if (System.Math.Round(pnt3d.Z, 3) != System.Math.Round(cogoPnt.Elevation, 3))
                                            {
                                                count2++;
                                                pnt3d = new Point3d(pnt3d.X, pnt3d.Y, cogoPnt.Elevation);
                                                idBrkLine.updateVertex(i, pnt3d);
                                            }
                                            try
                                            {
                                                ResultBuffer rb = cogoPnt.ObjectId.getXData(apps.lnkBrks);
                                                if (rb != null)
                                                {
                                                    List<Handle> handlesPNT = rb.rb_handles();
                                                    if (handlesPNT.Contains(idBrkLine.getHandle()) == false)
                                                    {
                                                        TypedValue[] tvsX = new TypedValue[1]{new TypedValue(1005, idBrkLine.getHandle())};
                                                        cogoPnt.ObjectId.setXData(tvsX, apps.lnkBrks);
                                                    }
                                                }
                                                else
                                                {
                                                    TypedValue[] TVs = new TypedValue[2];
                                                    TVs.SetValue(new TypedValue(1001, apps.lnkBrks), 0);
                                                    TVs.SetValue(new TypedValue(1005, idBrkLine.getHandle()), 1);
                                                    cogoPnt.ObjectId.setXData(TVs, apps.lnkBrks);
                                                }
                                            }
                                            catch (System.Exception ex)
                                            {
                                                BaseObjs.writeDebug(ex.Message + " cmdUBP.cs: line: 124");
                                            }
                                        }
                                        else
                                        {
                                            count3++;
                                            idBrkLine.changeProp(color7, "BRKLINE-ERROR", LineWeight.LineWeight200);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    count4++;
                                    idBrkLine.changeProp(color21, "BRKLINE-ERROR", LineWeight.LineWeight200);
                                    break;
                                }
                            }
                        }
                    }
                }

                string mess = string.Format("Results: \nRB Missing: {0}\nHandle = 0: {1}\nXY Error: {2}\nZ Error: {3}", count1, count4, count3, count2);
                Application.ShowAlertDialog(mess);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdUBP.cs: line: 151");
                Debug.Print(n.ToString());
            }

            try
            {
                SelectionSet ss = Select.buildSSet(typeof(MText), true);
                ObjectId[] idsMText = ss.GetObjectIds();
                IEnumerator<ObjectId> iEn = (IEnumerator<ObjectId>)idsMText.GetEnumerator();
                ResultBuffer rbMTxt = null;
                foreach (ObjectId id in idsMText)
                {
                    rbMTxt = id.getXData(apps.lnkGS);
                    if (rbMTxt == null)
                        continue;
                    TypedValue[] tvs = rbMTxt.AsArray();
                    List<Handle> handles = rbMTxt.rb_handles();
                    foreach (Handle h in handles)
                    {
                        ObjectId idObj = h.getObjectId();
                        if (!idObj.IsValid)
                        {
                            xData.removeHandleFromXDataGS(id, apps.lnkGS, h);
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}
