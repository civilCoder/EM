using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

using Autodesk.Civil.DatabaseServices;

using Base_Tools45;
using Base_Tools45.C3D;

namespace Stake
{
    public static class Stake_AddProfileView
    {
        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;

        public static void
        addProfileView(ObjectId idAlign, bool boolMod)
        {
            ObjectId idProfileViewStyle = ObjectId.Null;
            ObjectId idProfileViewLabelSetStyle = ObjectId.Null;
            ObjectId idProfileViewBandSetStyle = ObjectId.Null;

            try
            {
                idProfileViewStyle = Prof_Style.getProfileViewStyle("STAKE");
            }
            catch (System.Exception)
            {
                idProfileViewStyle = Prof_Style.getProfileViewStyle("Standard");
            }

            try
            {
                idProfileViewBandSetStyle = Prof_Style.getProfileViewBandSetStyle("STAKE");
            }
            catch (System.Exception)
            {
                idProfileViewBandSetStyle = Prof_Style.getProfileViewBandSetStyle("Standard");
            }

            Point3d varPntPick = Pub.pnt3dO;
            PromptStatus ps;

            if (boolMod)
            {
                varPntPick = fStake.InsertProfileView;
            }
            else
            {
                varPntPick = UserInput.getPoint("Select insertion point for Profile View", out ps, osMode: 0);
                if (ps == PromptStatus.Cancel)
                {
                    idAlign.delete();
                    return;
                }
                fStake.InsertProfileView = varPntPick;
            }

            string strLayerName = "PROFILES";
            ObjectId idLayer = Layer.manageLayers(strLayerName);

            ProfileView pv = Prof.addProfileView(idAlign, varPntPick);
            Alignment objAlign = (Alignment)idAlign.getEnt();

            pv.StationStart = objAlign.StartingStation;
            pv.StationEnd = objAlign.EndingStation;
            pv.Layer = strLayerName;

            fStake.HandleProfileView = pv.Handle;

            TypedValue[] tvs = new TypedValue[2] { new TypedValue(1001, "PROFILE"), new TypedValue(1000, pv.Handle) };
            idAlign.setXData(tvs, "PROFILE");
        }
    }
}