using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Math = Base_Tools45.Math;

namespace Stake
{
    public static class Stake_Grid2
    {
        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;
        private static Forms.frmGrid fGrid = Forms.Stake_Forms.sForms.fGrid;

        public static void
        stakeGridPoints(ObjectId idAlign, List<POI> varPOI_STAKE, double staBeg = 0, double staEnd = 0, int side = 0, long begNum = 30000)
        {
            Alignment objAlign = (Alignment)idAlign.getEnt();
            double dblStation = 0, dblOffset = 0;

            if ((side == 0))
            {
                if (double.Parse(fGrid.tbxOffsetH.Text) != 0)
                {
                    PromptStatus ps;
                    bool escape = true;

                    Point3d varPntPick = UserInput.getPoint("Select side to place stake points: <ESC to Cancel>", Pub.pnt3dO, out escape, out ps, osMode: 8);
                    if (escape)
                        return;

                    objAlign.StationOffset(varPntPick.X, varPntPick.Y, ref dblStation, ref dblOffset);

                    if (System.Math.Abs(dblOffset) > 20.0)
                    {
                        DialogResult varResponse = MessageBox.Show(string.Format("\nPoint selected is more than 20' from Alignment: {0} \nContinue?", objAlign.Name, MessageBoxButtons.YesNo));

                        if (varResponse == DialogResult.No)
                            return;
                    }  
                    if (dblOffset < 0)
                    {
                        side = -1;
                    }
                    else
                    {
                        side = 1;
                    }
                    fStake.Side = side;
                }
                else
                {
                    fStake.Side = 1;
                }
            }
            else
            {
                fStake.Side = side;
            }

            fStake.STAKE_LAYER = fGrid.tbxOffsetH.Text + "-OS-" + objAlign.Name;

            dblOffset = double.Parse(fGrid.tbxOffsetH.Text);
            string strName = fStake.NameStakeObject;

            double dblStaBeg = Math.roundDown3((objAlign.StartingStation));
            double dblStaEnd = Math.roundDown3((objAlign.EndingStation));

            uint lngPntNumBeg = Stake_Util.getBegPntNum();
            fStake.NextPntNum = lngPntNumBeg;

            CogoPointCollection cgPnts = CivilApplication.ActiveDocument.CogoPoints;

            fStake.POI_STAKED = new List<POI>();

            //**********PROCESS varPOI_STAKE*********************
            for (int i = 0; i <= varPOI_STAKE.Count; i++)
            {
                if (varPOI_STAKE[i].DescX == "")
                {
                    POI vPOI_STAKE = varPOI_STAKE[i];
                    vPOI_STAKE.DescX = vPOI_STAKE.Desc0;
                    varPOI_STAKE[i] = vPOI_STAKE;
                }

                if (i == varPOI_STAKE.Count - 1)
                    break;

                double dblElev = varPOI_STAKE[i].Elevation;

                switch (varPOI_STAKE[i].Desc0)
                {
                    case "AP":

                        switch (side)
                        {
                            case 1:
                                //right side

                                //counterclockwise
                                if (varPOI_STAKE[i].isRightHand)
                                {
                                    if (fGrid.optPBC.Checked)
                                    {
                                        Stake_Calc.doAnglePointOUT(idAlign, dblElev, varPOI_STAKE[i].Station, varPOI_STAKE[i].AngDelta, varPOI_STAKE[i].AngDir, varPOI_STAKE[i].DescX, strName, side, dblOffset);
                                    }
                                    else if (fGrid.optRBC.Checked)
                                    {
                                        Stake_Calc.doAnglePointOUT_RBC(idAlign, dblElev, varPOI_STAKE[i].Station, varPOI_STAKE[i].AngDelta, varPOI_STAKE[i].AngDir, varPOI_STAKE[i].DescX, strName, side, dblOffset);
                                    }
                                    //clockwise
                                }
                                else
                                {
                                    Stake_Calc.doAnglePointIN(idAlign, dblElev, varPOI_STAKE[i].Station, varPOI_STAKE[i].AngDelta, varPOI_STAKE[i].AngDir, varPOI_STAKE[i].DescX, strName, side, dblOffset);
                                }

                                break;

                            case -1:
                                //left side

                                //counterclockwise
                                if (varPOI_STAKE[i].isRightHand)
                                {
                                    Stake_Calc.doAnglePointIN(idAlign, dblElev, varPOI_STAKE[i].Station, varPOI_STAKE[i].AngDelta, varPOI_STAKE[i].AngDir, varPOI_STAKE[i].DescX, strName, side, dblOffset);
                                    //clockwise
                                }
                                else
                                {
                                    if (fGrid.optPBC.Checked)
                                    {
                                        Stake_Calc.doAnglePointOUT(idAlign, dblElev, varPOI_STAKE[i].Station, varPOI_STAKE[i].AngDelta, varPOI_STAKE[i].AngDir, varPOI_STAKE[i].DescX, strName, side, dblOffset);
                                    }
                                    else if (fGrid.optRBC.Checked)
                                    {
                                        Stake_Calc.doAnglePointOUT_RBC(idAlign, dblElev, varPOI_STAKE[i].Station, varPOI_STAKE[i].AngDelta, varPOI_STAKE[i].AngDir, varPOI_STAKE[i].DescX, strName, side, dblOffset);
                                    }
                                }

                                break;
                        }

                        break;

                    default:

                        string strDesc = varPOI_STAKE[i].DescX;
                        double dblEasting = 0, dblNorthing = 0;
                        try
                        {
                            objAlign.PointLocation(varPOI_STAKE[i].Station, dblOffset * side, ref dblEasting, ref dblNorthing);
                        }
                        catch (IndexOutOfRangeException)
                        {
                            try
                            {
                                objAlign.PointLocation(varPOI_STAKE[i].Station - 0.01, dblOffset * side, ref dblEasting, ref dblNorthing);
                            }
                            catch (IndexOutOfRangeException)
                            {
                                objAlign.PointLocation(varPOI_STAKE[i].Station + 0.01, dblOffset * side, ref dblEasting, ref dblNorthing);
                            }
                        }

                        Point3d dblPnt = new Point3d(dblEasting, dblNorthing, varPOI_STAKE[i].Elevation);

                        string strOffset = dblOffset.ToString();
                        Stake_Calc.setOffsetPoint(dblPnt, strOffset, strName, strDesc, idAlign, varPOI_STAKE[i].Station);

                        break;
                }
            }

            List<POI> varPOI_STAKED = fStake.POI_STAKED;
            int k = varPOI_STAKED.Count - 1;

            uint lngPntNumEnd = uint.Parse(varPOI_STAKED[k].PntNum);

            TypedValue[] tvs = new TypedValue[4] {
                new TypedValue(1001, "STAKE"),
                new TypedValue(1071, lngPntNumBeg),
                new TypedValue(1071, lngPntNumEnd),
                new TypedValue(1000, fStake.STAKE_LAYER)
            };

            idAlign.setXData(tvs, "STAKE");

            CgPnt_Group.updatePntGroup("SPNT");

            Stake_UpdateProfile.updateProfile(idAlign, (fStake.POI_STAKED), "STAKE", true, "STAKED");

            bool exists = false;
            ObjectId idDict = Dict.getNamedDictionary("STAKE_PNTS", out exists);

            List<Point3d> dblPnts = new List<Point3d>();
            Point3d pnt3d = Pub.pnt3dO;

            for (int p = 0; p < varPOI_STAKED.Count; p++)
            {
                ObjectId idPnt = BaseObjs._civDoc.CogoPoints.GetPointByPointNumber(uint.Parse(varPOI_STAKED[p].PntNum));
                pnt3d = idPnt.getCogoPntCoordinates();
                dblPnts.Add(pnt3d);
                ResultBuffer rb = new ResultBuffer {
                    new TypedValue(1000, idPnt.getCogoPntNumber().ToString()),
                    new TypedValue(1005, idPnt.getHandle().ToString())
                };
                Dict.addXRec(idDict, idPnt.ToString(), rb);
            }

            dblPnts.Add(dblPnts[0]);
            Draw.addPoly(dblPnts, fStake.STAKE_LAYER, 9);

            Misc.logUsage("STAKE", (lngPntNumEnd - lngPntNumBeg + 1));
        }
    }
}