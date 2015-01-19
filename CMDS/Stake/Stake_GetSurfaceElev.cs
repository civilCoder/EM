using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;

namespace Stake
{
    public static class Stake_GetSurfaceElev
    {
        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;

        public static double
        getSurfaceElevation(ObjectId idAlign, double dblStation)
        {
            double dblEasting = 0, dblNorthing = 0;
            TinSurface objSurface = fStake.SurfaceCPNT;
            idAlign.getAlignPointLoc(dblStation, 0.0, ref dblEasting, ref dblNorthing);
            return objSurface.FindElevationAtXY(dblEasting, dblNorthing);
        }

        public static void
        getSurfaceElevations(ObjectId idAlign, ref List<POI> varpoi)
        {
            double dblEasting = 0, dblNorthing = 0;
            TinSurface objSurface = fStake.SurfaceCPNT;
            POI vpoi;

            for (int i = 0; i < varpoi.Count; i++)
            {
                vpoi = varpoi[i];
                idAlign.getAlignPointLoc(varpoi[i].Station, 0.0, ref dblEasting, ref dblNorthing);
                vpoi.Elevation = objSurface.FindElevationAtXY(dblEasting, dblNorthing);
                varpoi[i] = vpoi;
            }
        }

        public static void
        getProfileElevations(ObjectId idAlign, string strProfileName, ref List<POI> varpoi)
        {
            Profile objProfile = Prof.getProfile(idAlign, strProfileName);
            POI vpoi;
            for (int i = 0; i < varpoi.Count; i++)
            {
                vpoi = varpoi[i];
                vpoi.Elevation = objProfile.ElevationAt(varpoi[i].Station);
                varpoi[i] = vpoi;
            }
        }
    }
}