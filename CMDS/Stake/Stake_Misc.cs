using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Base_Tools45;
using System;
using System.Collections.Generic;

namespace Stake
{
    public static class Stake_Misc
    {
        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;

        public static void
        set_fStakeProps(string strClass)
        {
            switch (strClass)
            {
                case "ALIGN":

                    fStake.cboHeight.Enabled = false;

                    fStake.cboInterval.Enabled = true;

                    fStake.cboInterval.Text = 25.ToString();
                    fStake.cboTolerance.Enabled = true;

                    fStake.cboTolerance.Text = 5.ToString();
                    fStake.cboOffset.Enabled = true;
                    fStake.cboOffset.Text = Convert.ToString(10);

                    fStake.optNo.Enabled = true;
                    fStake.optYes.Enabled = true;

                    fStake.optYes.Checked = true;

                    break;

                case "CURB":

                    fStake.cboHeight.Enabled = true;
                    fStake.cboInterval.Enabled = true;

                    fStake.cboInterval.Text = 25.ToString();
                    fStake.cboTolerance.Enabled = true;

                    fStake.cboTolerance.Text = 5.ToString();
                    fStake.cboOffset.Enabled = true;

                    fStake.cboOffset.Text = 3.ToString();

                    fStake.optNo.Enabled = true;
                    fStake.optYes.Enabled = true;

                    fStake.optNo.Checked = true;

                    fStake.cboDelta.Enabled = true;

                    fStake.cboDelta.Text = 4.ToString();

                    break;

                case "FL":

                    fStake.cboHeight.Enabled = false;
                    fStake.cboInterval.Enabled = true;

                    fStake.cboInterval.Text = 25.ToString();
                    fStake.cboTolerance.Enabled = true;

                    fStake.cboTolerance.Text = 5.ToString();
                    fStake.cboOffset.Enabled = true;

                    fStake.cboOffset.Text = 3.ToString();

                    fStake.optNo.Enabled = true;
                    fStake.optYes.Enabled = true;

                    fStake.optNo.Checked = true;

                    fStake.cboDelta.Enabled = false;

                    fStake.cboDelta.Text = 4.ToString();

                    break;

                case "FTG":

                    fStake.cboHeight.Enabled = false;
                    fStake.cboInterval.Enabled = false;
                    fStake.cboTolerance.Enabled = false;
                    fStake.cboOffset.Enabled = false;

                    fStake.optNo.Enabled = true;
                    fStake.optYes.Enabled = true;

                    fStake.optNo.Checked = true;

                    fStake.cboDelta.Enabled = false;

                    break;

                case "MISC":

                    fStake.cboHeight.Enabled = false;
                    fStake.cboInterval.Enabled = false;
                    fStake.cboTolerance.Enabled = false;
                    fStake.cboOffset.Enabled = true;
                    fStake.cboDelta.Enabled = false;

                    fStake.optNo.Enabled = false;
                    fStake.optYes.Enabled = false;

                    break;

                case "SEWER":
                case "SD":

                    fStake.cboHeight.Enabled = false;
                    fStake.cboInterval.Enabled = true;

                    fStake.cboInterval.Text = 25.ToString();
                    fStake.cboTolerance.Enabled = false;

                    fStake.cboTolerance.Text = 5.ToString();
                    fStake.cboOffset.Enabled = true;
                    fStake.cboOffset.Text = Convert.ToString(10);

                    fStake.optNo.Enabled = true;
                    fStake.optYes.Enabled = true;

                    fStake.optYes.Checked = true;

                    break;

                case "WALL":

                    fStake.cboHeight.Enabled = false;
                    fStake.cboInterval.Enabled = true;

                    fStake.cboInterval.Text = 50.ToString();
                    fStake.cboTolerance.Enabled = true;

                    fStake.cboTolerance.Text = 10.ToString();
                    fStake.cboOffset.Enabled = true;

                    fStake.cboOffset.Text = 10.ToString();

                    fStake.optNo.Enabled = true;
                    fStake.optYes.Enabled = true;

                    break;

                case "WTR":

                    fStake.cboHeight.Enabled = false;
                    fStake.cboInterval.Enabled = true;

                    fStake.cboInterval.Text = 50.ToString();
                    fStake.cboTolerance.Enabled = true;

                    fStake.cboTolerance.Text = 10.ToString();
                    fStake.cboOffset.Enabled = true;

                    fStake.cboOffset.Text = 10.ToString();

                    fStake.optNo.Enabled = false;
                    fStake.optYes.Enabled = false;

                    break;
            }
        }

        public static void
        deletePntFromProfile(string strLayer, int lngPntNum)
        {
            TypedValue[] tvs = new TypedValue[3];
            tvs.SetValue(new TypedValue((int)DxfCode.Start, "MTEXT"), 0);
            tvs.SetValue(new TypedValue((int)DxfCode.Text, lngPntNum.ToString()), 1);
            tvs.SetValue(new TypedValue((int)DxfCode.LayerName, strLayer), 2);

            SelectionSet ss = Select.buildSSet(tvs);
            ObjectId[] ids = ss.GetObjectIds();

            ResultBuffer rb = ids[0].getXData("LEADER");
            if (rb == null)
                return;
            tvs = rb.AsArray();

            ids[0].delete();

            ObjectId idLeader = tvs.getObjectId(1);
            idLeader.delete();
        }

        //UPDATE OBJECTIDs WHEN OPENING DRAWING

        public static void
        updateSTAKE_PNTS(ObjectId idDictSTAKE_PNTS)
        {
            List<uint> lngPntNums = new List<uint>();
            List<Handle> strAlignHandle = new List<Handle>();
            int k = 0;
            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                DBDictionary objDictSTAKE_PNTS = (DBDictionary)idDictSTAKE_PNTS.GetObject(OpenMode.ForRead);

                if (objDictSTAKE_PNTS.Count == 0)
                {
                    return;
                }

                k = objDictSTAKE_PNTS.Count;

                for (int i = 0; i < k; i++)
                {
                    List<DBDictionaryEntry> entries = Dict.getEntries(idDictSTAKE_PNTS);
                    foreach (DBDictionaryEntry entry in entries)
                    {
                        ResultBuffer rb = Dict.getXRec(idDictSTAKE_PNTS, entry.Key);
                        if (rb == null)
                            continue;
                        TypedValue[] tvs = rb.AsArray();
                        lngPntNums.Add(uint.Parse(tvs[0].Value.ToString()));
                        strAlignHandle.Add(tvs[1].Value.ToString().stringToHandle());
                    }
                }
                idDictSTAKE_PNTS.delete();
                tr.Commit();
            }

            bool exists = false;
            idDictSTAKE_PNTS = Dict.getNamedDictionary("STAKE_PNTS", out exists);  //returns id of new dictionary

            Autodesk.Civil.ApplicationServices.CivilDocument civDoc =
                (Autodesk.Civil.ApplicationServices.CivilDocument)Autodesk.Civil.ApplicationServices.CivilApplication.ActiveDocument;

            for (int i = 0; i < k; i++)
            {
                ObjectId idCgPnt = civDoc.CogoPoints.GetPointByPointNumber((uint)lngPntNums[i]);
                if (!idCgPnt.IsValid)
                    continue;

                TypedValue[] tv = new TypedValue[2];
                tv.SetValue(new TypedValue((int)DxfCode.Int32, lngPntNums[i]), 0);
                tv.SetValue(new TypedValue(1005, strAlignHandle[i]), 1);

                Dict.addXRec(idDictSTAKE_PNTS, idCgPnt.ToString(), tv);
            }
        }
    }
}