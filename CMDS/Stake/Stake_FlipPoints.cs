using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;

namespace Stake
{
    public static class Stake_FlipPoints
    {
        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;

        public static void
        flipPointsMove(bool add)
        {
            double dblEasting = 0;
            double dblNorthing = 0;
            List<StakedPnt> stakedPnts = getStakedPnts();
            try
            {
                for (int i = 0; i < stakedPnts.Count; i++)
                {
                    Alignment objAlign = (Alignment)stakedPnts[i].hAlign.getEnt();

                    objAlign.PointLocation(stakedPnts[i].Station, stakedPnts[i].Offset * -1, ref dblEasting, ref dblNorthing);

                    Point3d pnt3d = new Point3d(dblEasting, dblNorthing, stakedPnts[i].Elev);
                    ObjectId idCgPnt = stakedPnts[i].hCgPnt.getObjectId();
                    ResultBuffer rb = idCgPnt.getXData("STAKE");
                    if (rb == null)
                        continue;
                    TypedValue[] tvs = rb.AsArray();
                    double offset = double.Parse(tvs[3].Value.ToString()) * -1;
                    tvs[3] = new TypedValue(1040, offset);

                    if (add)
                    {
                        uint pntNum = 0;
                        idCgPnt = pnt3d.setPoint(out pntNum, "SPNT");
                        idCgPnt.setXData(tvs, "STAKE");
                    }
                    else
                    {
                        idCgPnt.moveCogoPoint(pnt3d);
                        idCgPnt.setXData(tvs, "STAKE");
                    }
                }
            }
            catch (System.Exception)
            {
            }
        }

        public static List<StakedPnt>
        getStakedPnts()
        {
            SelectionSet ss = Select.buildSSet(typeof(CogoPoint), false, "Select Cogo Point");
            ObjectId[] ids = ss.GetObjectIds();
            List<StakedPnt> stakedPnts = new List<StakedPnt>();

            for (int i = 0; i <= ids.Length; i++)
            {
                ObjectId idCgPnt = ids[i];
                ResultBuffer rb = idCgPnt.getXData("STAKE");
                if (rb == null)
                    continue;
                TypedValue[] tvs = rb.AsArray();
                StakedPnt stakedPnt = new StakedPnt
                {
                    hCgPnt = idCgPnt.getHandle(),
                    hAlign = tvs[1].Value.ToString().stringToHandle(),
                    Station = double.Parse(tvs[2].Value.ToString()),
                    Offset = double.Parse(tvs[3].Value.ToString()),
                    Elev = idCgPnt.getCogoPntElevation(),
                    Desc = idCgPnt.getCogoPntDesc(),
                    Number = (uint)idCgPnt.getCogoPntNumber()
                };
                stakedPnts.Add(stakedPnt);
            }
            return stakedPnts;
        }
    }
}