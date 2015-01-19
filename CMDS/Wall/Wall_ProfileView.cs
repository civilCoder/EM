using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Diagnostics;
using wd = Wall.Wall_Design;
using wdp = Wall.Wall_DesignProfile;

namespace Wall {
    public static class Wall_ProfileView {
        public static ProfileView
        CreateWallProfileView(Alignment objAlign, List<AlgnEntData> algnEntDataList, Polyline3d objBrkLEFT, Polyline3d objBrkRIGHT, string strAPP) {
            ObjectId idAlign = objAlign.ObjectId;

            ObjectId idPViewStyle = default(ObjectId);
            ObjectId idPViewBandSetStyle = default(ObjectId);

            ProfileView objProfileView = default(ProfileView);

            string strLayer = null;

            List<staOffElev> sOffs = wd.convert3dPolyToPnt_Data(objAlign, algnEntDataList, objBrkLEFT.ObjectId, "BRKLEFT");
            List<PNT_DATA> PntsBrkLEFT = new List<PNT_DATA>();
            foreach (staOffElev s in sOffs) {
                PNT_DATA p = new PNT_DATA() { STA = s.staAlign, z = s.elev };
                PntsBrkLEFT.Add(p);
            }
            wdp.CreateProfileByLayout2("BRKLEFT", objAlign, PntsBrkLEFT);
            sOffs = new List<staOffElev>();
            sOffs = wd.convert3dPolyToPnt_Data(objAlign, algnEntDataList, objBrkRIGHT.ObjectId, "BRKRIGHT");
            List<PNT_DATA> PntsBrkRIGHT = new List<PNT_DATA>();
            foreach (staOffElev s in sOffs) {
                PNT_DATA p = new PNT_DATA() { STA = s.staAlign, z = s.elev };
                PntsBrkRIGHT.Add(p);
            }
            wdp.CreateProfileByLayout2("BRKRIGHT", objAlign, PntsBrkRIGHT);

            Debug.Print("CreateProfileView - Line 45");

            try {
                using (BaseObjs._acadDoc.LockDocument())
                {
                    idPViewStyle = Prof_Style.getProfileViewStyle("WALL");
                    if (idPViewStyle == ObjectId.Null)
                    {
                        idPViewStyle = Prof_Style.CreateProfileViewStyle("WALL");
                        if (idPViewStyle.IsNull)
                            idPViewStyle = BaseObjs._civDoc.Styles.ProfileViewStyles[0];
                    }
                }
                using (BaseObjs._acadDoc.LockDocument())
                {
                    idPViewBandSetStyle = Prof_Style.getProfileViewBandSetStyle("WALL");
                    if (idPViewBandSetStyle == ObjectId.Null)
                    {
                        idPViewBandSetStyle = Prof_Style.CreateProfileViewBandSetStyle("WALL");
                        if (idPViewBandSetStyle.IsNull)
                            idPViewBandSetStyle = BaseObjs._civDoc.Styles.ProfileViewBandSetStyles[0];
                    }
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex) {
                Application.ShowAlertDialog(ex.ToString());
                return null;
            }

            PromptStatus ps = default(PromptStatus);
            Point3d pnt3dIns = default(Point3d);
            try {
                pnt3dIns = UserInput.getPoint("Select insertion point for Profile View", out ps, osMode: 0);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception) {
                objAlign.Erase();
                return null;                
            }

            strLayer = string.Format("{0}-PROFILEVIEW", objAlign.Name);

            Layer.manageLayers(strLayer);

            Debug.Print("CreateProfileView - Line 75");

            using (BaseObjs._acadDoc.LockDocument()) {
                using (Transaction TR = BaseObjs.startTransactionDb()) {
                    try {
                        Prof.removeProfileViews(objAlign);
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception ex) {
                        Application.ShowAlertDialog(ex.ToString());
                    }

                    objProfileView = Prof.addProfileView(objAlign.ObjectId, pnt3dIns, idPViewBandSetStyle, idPViewStyle);

                    TypedValue[] tvs = new TypedValue[2] {
                        new TypedValue(1001, "WALLDESIGN"),
                        new TypedValue(1005, objProfileView.Handle)
                    };

                    idAlign.setXData(tvs, "WALLDESIGN");

                    tvs = new TypedValue[4] {
                        new TypedValue(1001, "BRKRIGHT"),
                        new TypedValue(1005, objBrkRIGHT.Handle),
                        new TypedValue(1040, PntsBrkRIGHT[0].STA),
                        new TypedValue(1040, PntsBrkRIGHT[PntsBrkRIGHT.Count - 1].STA)
                    };

                    objProfileView.ObjectId.setXData(tvs, "BRKRIGHT");

                    tvs = new TypedValue[4] {
                        new TypedValue(1001, "BRKLEFT"),
                        new TypedValue(1005, objBrkLEFT.Handle),
                        new TypedValue(1040, PntsBrkLEFT[0].STA),
                        new TypedValue(1040, PntsBrkLEFT[PntsBrkLEFT.Count - 1].STA)
                    };

                    objProfileView.ObjectId.setXData(tvs, "BRKLEFT");

                    Profile objProfile1 = Prof.getProfile(objAlign.ObjectId, "BRKLEFT");
                    //Prof.removeProfileLabelGroup(objProfileView.ObjectId, objProfile1.ObjectId)

                    Profile objProfile2 = Prof.getProfile(objAlign.ObjectId, "BRKRIGHT");
                    //Prof.removeProfileLabelGroup(objProfileView.ObjectId, objProfile2.ObjectId)

                    TR.Commit();
                }
            }
            Debug.Print("CreateProfileView - Line 145");

            return objProfileView;
        }
    }
}