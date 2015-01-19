using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using System.Collections.Generic;
using System.Linq;

namespace Stake
{
    public static class Stake_Util
    {
        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;

        public static void
        removeDuplicatePoints(ref List<POI> poi)
        {
            for (int i = 1; i < poi.Count; i++)
            {
                if (poi[i - 1].Station == poi[i].Station)
                {
                    if (poi[i - 1].Elevation != poi[i].Elevation)
                    {
                        if (poi[i - 1].OFFSET < poi[i].OFFSET)
                        {
                            poi.RemoveAt(i);
                        }
                        else
                        {
                            poi.RemoveAt(i - 1);
                        }
                    }
                }
            }
        }

        public static uint
        getBegPntNum()
        {
            long pntNumBeg = 0;
            List<long> pntNums = null;
            List<long> pntNumsSorted = new List<long>();

            CogoPointCollection idPnts = CivilApplication.ActiveDocument.CogoPoints;

            foreach (ObjectId idPnt in idPnts)
            {
                pntNums.Add((long)idPnt.getCogoPntNumber());
            }

            if (pntNums.Count == 0)
            {
                pntNumBeg = 30000;
            }
            else
            {
                var sortPntNums = from p in pntNums
                                  orderby p ascending
                                  select p;

                foreach (var p in sortPntNums)
                    pntNumsSorted.Add(p);

                int k = pntNumsSorted.Count;

                if (pntNumsSorted[k - 1] < 30000)
                {
                    pntNumBeg = 30000;
                }
                else if (pntNumsSorted[0] > 30000)
                {
                    pntNumBeg = pntNumsSorted[k - 1];
                }
            }
            return (uint)pntNumBeg;
        }

        public static void
        getPntRanges(List<long> lngPnts)
        {
            List<PntRange> pntRanges = null;
            PntRange pntRange;

            List<long> pnts = new List<long>();

            var sortedPnt = from p in lngPnts
                            orderby p ascending
                            select p;
            foreach (var p in sortedPnt)
                pnts.Add(p);

            int Xb = 0;
            int Xe = 0;
            int k = pnts.Count;
            for (int i = 1; i <= k; i++)
            {
                if (pnts[i - 1] + 1 != pnts[i])
                {
                    Xe = i;
                    pntRange = new PntRange();
                    pntRange.BegNum = pnts[Xb];
                    pntRange.EndNum = pnts[Xe - 1];
                    pntRanges.Add(pntRange);

                    Xb = i;
                }
            }

            if (pnts[k - 2] + 1 != pnts[k - 1])
            {
                pntRange = new PntRange();
                pntRange.BegNum = pnts[k - 1];
                pntRange.EndNum = pnts[k - 1];
                pntRanges.Add(pntRange);
            }

            fStake.PntRanges = pntRanges;
        }
    }
}