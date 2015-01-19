using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Stake
{
    public static partial class Stake_Main
    {
        private static void processWater(string nameLayer, ObjectId idPolyGuideline, BlockTableRecord ms, ref List<POI> varPOI, out ObjectId idAlign)
        {
            string nameAlign = Align.getAlignName(nameLayer);
            Alignment align = Align.addAlignmentFromPoly(nameAlign, nameLayer, idPolyGuideline, "Standard", "Standard", true);

            fStake.ACTIVEALIGN = align;
            idAlign = align.ObjectId;
            fStake.objectID = idAlign;

            Stake_GetCardinals.getCardinals_Horizontal(idAlign, ref varPOI);

            Stake_GetNestedObjects.getNestedObjects(idAlign, ref varPOI, "CNTL");

            Stake_GetCardinals.getCrossingAligns(idAlign, ref varPOI);

            varPOI = Stake_ModifyStaking.modifyStaking(varPOI);
            fStake.POI_ORG = varPOI;

            Stake_GetCardinals.checkBegAndEndStations(idAlign, ref varPOI);

            Stake_AddProfile.makeProfile(idAlign, varPOI, "STAKE", "ByLayout", false);

            Stake_GetAnglePoints.getAnglePoints(idAlign, ref varPOI);//IDENTIFY ANGLE POINTS - CHECK IF CLOSED

            var sortSta = from s in varPOI
                          orderby s.Station ascending
                          select s;

            List<POI> poiTmp = new List<POI>();
            foreach (var s in sortSta)
                poiTmp.Add(s);
            varPOI = poiTmp;

            Stake_DuplicateStations.resolveDuplicateStations(ref varPOI);

            Stake_GetSurfaceElev.getSurfaceElevations(idAlign, ref varPOI);//ALL ELEVATIONS COME FROM SURFACE

            Stake_AddProfile.makeProfile(idAlign, varPOI, "STAKE", "ByLayout", true);

            if (varPOI[0].isClosed)
            {
                Stake_GetAnglePoints.addEndElev(idAlign, ref varPOI, "CPNT");
            }

            Stake_UpdateProfile.updateProfile(idAlign, varPOI, "STAKE", true, "STAKE");
            fStake.POI_CALC = varPOI;
        }

        private static void
        processSewer(string nameLayer, ObjectId idPolyGuideline, BlockTableRecord ms, string nameXRef, ref List<POI> varPOI, out ObjectId idAlign)
        {
            List<Point3d> pnts3d = idPolyGuideline.getCoordinates3dList();
            Alignment align = null;
            string nameAlign = "";
            idAlign = ObjectId.Null;

            if (pnts3d.Count == 1)
            {
                DialogResult varResponse = System.Windows.Forms.MessageBox.Show("Single segment selected!!\n\n" + "Proceed with single segment?\n", "SINGLE SEGMENT SELECTED", MessageBoxButtons.YesNo);

                switch (varResponse)
                {
                    case DialogResult.Yes:
                        nameAlign = Align.getAlignName(nameLayer);
                        align = Align.addAlignmentFromPoly(nameAlign, nameLayer, idPolyGuideline, "Standard", "Standard", true);
                        fStake.ACTIVEALIGN = align;
                        idAlign = align.ObjectId;
                        fStake.objectID = idAlign;
                        break;

                    case DialogResult.No:
                        varResponse = System.Windows.Forms.MessageBox.Show("Create Alignment?\n", "CREATE ALIGNMENT", MessageBoxButtons.YesNo);

                        switch (varResponse)
                        {
                            case DialogResult.Yes:
                                Stake_Algn.createNewAlign(nameLayer);
                                idPolyGuideline.delete();
                                break;

                            case DialogResult.No:
                                idPolyGuideline.delete();
                                break;
                        }
                        break;
                }
            }
            else
            {
                nameAlign = Align.getAlignName(nameLayer);
                align = Align.addAlignmentFromPoly(nameAlign, nameLayer, idPolyGuideline, "Standard", "Standard", true);
                fStake.ACTIVEALIGN = align;
                idAlign = align.ObjectId;
                fStake.objectID = idAlign;
            }

            fStake.HandleAlign = idAlign.getHandle();
            fStake.NameStakeObject = nameAlign;

            Stake_GetCardinals.getCardinals_Horizontal(idAlign, ref varPOI);

            Stake_GetNestedObjects.getNestedPoints(idAlign, ref varPOI, ms, nameXRef);

            Stake_GetCardinals.getCrossingAligns(idAlign, ref varPOI);

            fStake.POI_ORG = varPOI;

            Stake_GetCardinals.getCardinals_Vertical(idAlign, ref varPOI);

            Stake_AddProfile.makeProfile(idAlign, varPOI, "STAKE", "ByLayout", false);

            Stake_GetBC_EC.getBC_EC(idAlign, ref varPOI);

            Stake_DuplicateStations.resolveDuplicateStations(ref varPOI);

            Stake_UpdateProfile.updateProfile(idAlign, varPOI, "STAKE", false, "STAKE");

            Stake_UpdateProfile.updateProfile(idAlign, fStake.POI_PNTs, "STAKE", true, "PNTS");
        }
    }
}