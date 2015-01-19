using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Linq;

namespace Stake
{
    public static class SelectPoints
    {
        private static Forms.frmExport fExport = Forms.Stake_Forms.sForms.fExport;

        public static void
        selectPntsOnScreen()
        {
            SelectionSet ss = Select.buildSSet(typeof(CogoPoint), false, "Select Cogo Point");
            ObjectId[] ids = ss.GetObjectIds();
            List<CgPnt> cgPnts = new List<CgPnt>();

            for (int i = 0; i < ids.Length; i++)
            {
                ObjectId idCgPnt = ids[i];
                CogoPoint objPnt = (CogoPoint)idCgPnt.getEnt();

                if (objPnt.Layer.Contains("-OS-"))
                {
                    if (!objPnt.Layer.Contains("NOT-USED"))
                    {
                        ResultBuffer rb = idCgPnt.getXData("STAKE");
                        if (rb == null)
                            continue;
                        TypedValue[] tvs = rb.AsArray();

                        CgPnt cgPnt = new CgPnt
                        {
                            Num = objPnt.PointNumber,
                            Desc = conformPntDesc(objPnt.RawDescription),
                            nameLayer = objPnt.Layer,
                            hAlign = tvs[1].Value.ToString().stringToHandle()
                        };
                        cgPnts.Add(cgPnt);
                    }
                }
            }

            Stake_ProcessPntList.ProcPntList(cgPnts, "OnScreen");
        }

        public static void
        SelectPntsByRange(string strOption = "")
        {
            TypedValue[] tvs = new TypedValue[1];
            tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(CogoPoint)).DxfName), 0);
            SelectionSet ss = Select.buildSSet(tvs);

            ObjectId[] ids = ss.GetObjectIds();
            if (ids.Length == 0)
                return;

            List<CgPnt> cgPnts = new List<CgPnt>();

            for (int i = 0; i < ids.Length; i++)
            {
                ObjectId idCgPnt = ids[i];
                CogoPoint objPnt = (CogoPoint)idCgPnt.getEnt();

                if (objPnt.Layer.Contains("-OS-"))
                {
                    if (!objPnt.Layer.Contains("NOT-USED"))
                    {
                        ResultBuffer rb = idCgPnt.getXData("STAKE");
                        if (rb == null)
                            continue;
                        TypedValue[] tvs1 = rb.AsArray();

                        CgPnt cgPnt = new CgPnt
                        {
                            Num = objPnt.PointNumber,
                            Desc = conformPntDesc(objPnt.RawDescription),
                            nameLayer = objPnt.Layer,
                            hAlign = tvs1[1].Value.ToString().stringToHandle()
                        };
                        cgPnts.Add(cgPnt);
                    }
                }
            }
            if (strOption == "")
                strOption = "ByRange";

            Stake_ProcessPntList.ProcPntList(cgPnts, strOption);
        }

        public static bool
        SelectPntsByAlign()
        {
            bool byAlign = false;

            ObjectIdCollection idsAlign = Align.getAlignmentIDs();
            bool exists = false;
            ObjectId idDict = Dict.getNamedDictionary("STAKE_PNTS", out exists);
            if (!exists)
                return false;

            List<StakedPnt> stakedPnts = new List<StakedPnt>();
            List<DBDictionaryEntry> entries = Dict.getEntries(idDict);
            foreach (DBDictionaryEntry entry in entries)
            {
                if (entry.GetType() == typeof(Xrecord))
                {
                    Xrecord xRec = Dict.getXRec(idDict, entry);
                    ResultBuffer rb = xRec.Data;
                    if (rb == null)
                        continue;

                    TypedValue[] tvs = rb.AsArray();
                    StakedPnt stakedPnt = new StakedPnt
                    {
                        hAlign = tvs[0].Value.ToString().stringToHandle(),
                        Number = uint.Parse(tvs[1].Value.ToString())
                    };
                    stakedPnts.Add(stakedPnt);
                }
            }

            var sortPnts = from p in stakedPnts
                           orderby p.Number ascending
                           group p by p.hAlign
                               into grpAlign
                               orderby grpAlign.Key
                               select grpAlign;

            //List<StakePntSum> stakedPntSum = new List<StakePntSum>();   //align handle and list of points
            List<DataSet> dataSet = new List<DataSet>();

            foreach (var p in sortPnts)
            {
                Handle hAlign = p.Key;
                Alignment align = (Alignment)hAlign.getEnt();

                if (align != null)
                {
                    List<uint> nums = new List<uint>();
                    foreach (var n in p)
                        nums.Add(n.Number);

                    var sortNums = from a in nums
                                   orderby a ascending
                                   select a;

                    ResultBuffer rb = align.GetXDataForApplication("STAKE");
                    if (rb == null)
                        continue;
                    TypedValue[] tvs = rb.AsArray();

                    DataSet dSet = new DataSet
                    {
                        Layer = tvs[3].Value.ToString(),
                        ObjectName = align.Name,
                        Lower = (uint)sortNums.Min(),
                        Upper = (uint)sortNums.Max(),
                        COUNT = (int)sortNums.Count(),
                        Nums = nums
                    };
                    dataSet.Add(dSet);
                    byAlign = true;
                }
                else
                {
                    byAlign = false;
                }
            }
            return byAlign;
        }

        public static string
        conformPntDesc(string strDesc)
        {
            int intPos1 = strDesc.IndexOf("O/S");
            int intPos2 = 0;

            if (intPos1 >= 0)
            {
                intPos2 = strDesc.IndexOf("@", intPos1);
                return strDesc.Substring(intPos1 + 4, intPos2 - intPos1 - 4);
            }
            else
            {
                intPos1 = strDesc.IndexOf("-OS-");
                return strDesc.Substring(intPos1 + 4, intPos2 - intPos1 - 4);
            }
        }
    }
}