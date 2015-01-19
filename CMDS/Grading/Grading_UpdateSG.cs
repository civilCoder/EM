using Autodesk.AutoCAD.DatabaseServices;
using Base_Tools45;
using System.Collections.Generic;

namespace Grading
{
    public static class Grading_UpdateSG
    {
        public static void
        updateSG(ObjectId idPoly3dSLP)
        {
            Polyline3d poly3d = (Polyline3d)idPoly3dSLP.getEnt();

            ResultBuffer rbLnkBrks = idPoly3dSLP.getXData(apps.lnkBrks);
            if (rbLnkBrks == null)
                return;
            TypedValue[] tvs = rbLnkBrks.AsArray();

            List<ObjectId> idsCgPNts = tvs.getObjectIdList();
            poly3d.setBegPnt(idsCgPNts[0].getCogoPntCoordinates());
            poly3d.setEndPnt(idsCgPNts[1].getCogoPntCoordinates());

            poly3d.deleteVertices(idsCgPNts);

            ResultBuffer rbSLP = idPoly3dSLP.getXData(apps.lnkSLP);
            if (rbSLP == null)
                return;
            tvs = rbSLP.AsArray();

            tvs.deleteLinkedEnts();

            double B1Width = double.Parse(tvs[1].Value.ToString());
            double B1Slope = double.Parse(tvs[2].Value.ToString());
            double B2Width = double.Parse(tvs[4].Value.ToString());
            double B2Slope = double.Parse(tvs[5].Value.ToString());
            double slope = double.Parse(tvs[7].Value.ToString());
            int side = int.Parse(tvs[8].Value.ToString());
            int interval = int.Parse(tvs[9].Value.ToString());
            string surfTAR = tvs[11].Value.ToString();
            string surfDES = tvs[12].Value.ToString();

            Grading.Cmds.cmdSG.SG(B1Width, B1Slope, B2Width, B2Slope, slope, interval, surfTAR, surfDES, side, poly3d: poly3d);
        }
    }
}
