using Autodesk.AutoCAD.DatabaseServices;
using Base_Tools45;
using System.Collections.Generic;

namespace Stake
{
    public static class Public
    {
        public static bool boolMsgShown;
        public static bool boolSTAKE;

        public static DBDictionary objDictGRIDS;

        public static bool gByPass;

        public static List<GRID_DATA> varGRID_DATA;
    }

    public class CgPnt
    {
        public uint Num;
        public string Desc;
        public string nameLayer;
        public Handle hAlign;
    }
}