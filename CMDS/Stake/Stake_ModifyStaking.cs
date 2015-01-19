using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using System.Collections.Generic;
using System.Linq;
using Table = Autodesk.AutoCAD.DatabaseServices.Table;

namespace Stake
{
    public static class Stake_ModifyStaking
    {
        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;

        public static List<POI>
        modifyStaking(List<POI> varpoi)
        {
            Alignment objAlign = fStake.ACTIVEALIGN;
            ObjectId idAlign = objAlign.ObjectId;

            List<POI> varPOIadd = new List<POI>();
            if (Stake_GetCardinals.userAddCrossingsFeatures(idAlign, ref varPOIadd))
            {
                switch (fStake.ClassObj)
                {
                    case "WTR":
                        Stake_GetSurfaceElev.getSurfaceElevations(idAlign, ref varPOIadd);

                        break;

                    case "ALIGN":
                    case "SEWER":

                        Stake_GetSurfaceElev.getProfileElevations(idAlign, "STAKE", ref varPOIadd);

                        break;
                }

                for (int i = 0; i <= varPOIadd.Count; i++)
                {
                    varpoi.Add(varPOIadd[i]);
                }

                var sortSta = from s in varpoi
                              orderby s.Station ascending
                              select s;

                List<POI> poitmp = new List<POI>();
                foreach (var s in sortSta)
                    poitmp.Add(s);

                varpoi = poitmp;

                Stake_AddProfile.makeProfile(idAlign, varpoi, "STAKE", "ByLayout", true);

                switch (fStake.ClassObj)
                {
                    case "ALIGN":
                    case "SEWER":

                        Stake_UpdateProfile.updateProfile(idAlign, varpoi, "STAKE", false, "STAKE");

                        break;
                }

                List<AlgnData> algnData = fStake.algnData;
                AlgnData aData = new AlgnData();
                Table objTable = null;
                for (int i = 1; i < algnData.Count; i++)
                {
                    aData = algnData[i];

                    if (aData.AlignHandle == objAlign.Handle)
                    {
                        objTable = (Table)aData.TableHandle.getEnt();
                        objTable.ObjectId.delete();
                        break;
                    }
                }

                ObjectId idTable = Stake_Table.makeTable(idAlign, varpoi, aData);
                Stake_Table.addTableData(idTable, varpoi);
            }
            return varpoi;
        }
    }
}