using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Diagnostics;

namespace Stake
{
    public static class Stake_AddProfile
    {
        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;

        public static ObjectId
        makeProfile(ObjectId idAlign, List<POI> varpoi, string nameProfile, string profileMode, bool boolMod)
        {
            string strLayerName = string.Format("PROFILE-{0}", nameProfile);
            ObjectId idLayer = Layer.manageLayers(strLayerName);

            ObjectId idProfileStyle = ObjectId.Null;
            ObjectId idProfileLabelSetStyle = ObjectId.Null;

            try
            {
                idProfileStyle = Prof_Style.getProfileStyle(nameProfile);
                if (idProfileStyle == ObjectId.Null)
                    idProfileStyle = Prof_Style.getProfileStyle("Standard");
            }
            catch (System.Exception)
            {
            }

            try
            {
                idProfileLabelSetStyle = Prof_Style.getProfileLabelSetStyle(nameProfile);
                if (idProfileLabelSetStyle == ObjectId.Null)
                    idProfileLabelSetStyle = Prof_Style.getProfileLabelSetStyle("Standard");
            }
            catch (System.Exception)
            {
            }

            Profile objProfile = null;
            ObjectId idProfile = ObjectId.Null;

            objProfile = Prof.getProfile(idAlign, nameProfile);
            if (objProfile != null)
                objProfile.ObjectId.delete();

            string nameAlign = Align.getAlignName(idAlign);

            switch (profileMode)
            {
                case "ByLayout":
                    objProfile = Prof.addProfileByLayout(nameProfile, nameAlign, strLayerName, "Standard", "Standard");
                    //objProfile = Prof.addProfileByLayout(nameProfile, idAlign, idLayer, idProfileStyle, idProfileLabelSetStyle);
                    if (objProfile == null)
                    {
                        Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(string.Format("Failed to make {0} Profile. Exiting...", nameProfile));
                        return idProfile;
                    }
                    break;

                case "BySurface":
                    TinSurface objSurface = fStake.SurfaceCPNT;
                    if ((objSurface == null))
                    {
                        Stake_GetSurfaceCPNT.getSurfaceFromXRef("CPNT-ON", "GCAL");
                        objSurface = fStake.SurfaceCPNT;
                    }

                    objProfile = Prof.addProfileBySurface(nameProfile, idAlign, objSurface.ObjectId, idLayer, idProfileStyle, idProfileLabelSetStyle);
                    if (objProfile == null)
                        Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(string.Format("Failed to make {0} Profile. Exiting...", nameProfile));
                    break;
            }

            TypedValue[] tvs = new TypedValue[2] { new TypedValue(1001, "STAKE"), new TypedValue(1000, fStake.ClassObj) };

            idProfile.setXData(tvs, "STAKE");

            Debug.Print("BEGIN POI IN makeProfile");

            for (int i = 0; i < varpoi.Count; i++)
            {
                Debug.Print(varpoi[i].Station + " " + varpoi[i].Elevation);
            }
            Debug.Print("END POI IN makeProfile");

            ProfilePVI objProfilePVI;

            double dblHeight = 0;
            if (nameProfile == "CURB")
            {
                dblHeight = System.Math.Round(double.Parse(fStake.cboHeight.Text) / 12, 3);
            }

            for (int i = 0; i < varpoi.Count; i++)
            {
                double dblStation = varpoi[i].Station;

                try
                {
                    objProfilePVI = objProfile.PVIs.AddPVI(dblStation, varpoi[i].Elevation + dblHeight);
                }
                catch (System.Exception)
                {
                    try
                    {
                        objProfilePVI = objProfile.PVIs.AddPVI(dblStation + 0.01, varpoi[i].Elevation + dblHeight);
                    }
                    catch
                    {
                        objProfilePVI = objProfile.PVIs.AddPVI(dblStation - 0.01, varpoi[i].Elevation + dblHeight);
                    }
                }
            }

            return idProfile;
        }
    }
}