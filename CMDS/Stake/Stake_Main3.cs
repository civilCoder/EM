using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Diagnostics;

namespace Stake
{
    public static partial class Stake_Main
    {
        private static void
        processCurb(string nameLayer, ObjectId idPolyGuideline, BlockTableRecord ms, string nameXRef, ref List<POI> varPOI, out ObjectId idAlign)
        {
            string nameAlign = Align.getAlignName(nameLayer);
            Alignment align = Align.addAlignmentFromPoly(nameAlign, nameLayer, idPolyGuideline, "Standard", "Standard", true);

            fStake.ACTIVEALIGN = align;
            idAlign = align.ObjectId;
            fStake.objectID = idAlign;

            Stake_GetCardinals.getCardinals_Horizontal(idAlign, ref varPOI);

            Stake_GetNestedObjects.getNestedPoints(idAlign, ref varPOI, ms, nameXRef);

            fStake.POI_ORG = varPOI;

            Stake_GetCardinals.getCardinals_Vertical(idAlign, ref varPOI);

            Stake_AddProfile.makeProfile(idAlign, varPOI, "CURB", "ByLayout", false); //VERTICAL CONTROL IS SET FROM POINTS

            Stake_GetAnglePoints.getAnglePoints(idAlign, ref varPOI);//IDENTIFY ANGLE POINTS - CHECK IF CLOSED

            Stake_GetCardinals.checkBegAndEndStations(idAlign, ref varPOI);

            if (varPOI[0].isClosed)
            {
                Stake_GetAnglePoints.addEndElev(idAlign, ref varPOI, "FLOWLINE");
            }

            Stake_GetBC_EC.getBC_EC(idAlign, ref varPOI);

            Debug.Print("BEGIN POI AFTER getBC_EC");

            for (int i = 0; i < varPOI.Count; i++)
            {
                Debug.Print(i + " " + varPOI[i].Station + " " + varPOI[i].Elevation + " " + varPOI[i].Desc0 + "   " + varPOI[i].DescX);
            }

            Debug.Print("END POI AFTER getBC_EC");

            Stake_DuplicateStations.resolveDuplicateStations(ref varPOI);

            Stake_AddProfile.makeProfile(idAlign, varPOI, "CURB", "ByLayout", true);

            Stake_UpdateProfile.updateProfile(idAlign, fStake.POI_ORG, "FLOWLINE", true, "ORG");

            Stake_UpdateProfile.updateProfile(idAlign, varPOI, "STAKE", false, "STAKE");
        }
    }
}