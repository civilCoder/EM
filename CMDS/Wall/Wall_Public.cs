using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using System.Collections.Generic;

namespace Wall
{
    public static class Wall_Public{

        public static MText LABEL { get; set; }

        public static Dictionary<float, FEATUREDATA> W0  { get; set; }
        public static Dictionary<float, FEATUREDATA> W1  { get; set; }
        public static Dictionary<float, FEATUREDATA> W2  { get; set; }
        public static Dictionary<float, FEATUREDATA> W3  { get; set; }
        public static Dictionary<float, FEATUREDATA> W4  { get; set; }
        public static Dictionary<float, FEATUREDATA> W5  { get; set; }
        public static Dictionary<float, FEATUREDATA> W6  { get; set; }
        public static Dictionary<float, FEATUREDATA> W7  { get; set; }
        public static Dictionary<float, FEATUREDATA> W8  { get; set; }
        public static Dictionary<float, FEATUREDATA> W9  { get; set; }

        public static Dictionary<float, FEATUREDATA> GB  { get; set; }
        public static Dictionary<float, FEATUREDATA> G0  { get; set; }

        public static Dictionary<float, FEATUREDATA> RF  { get; set; }
        public static Dictionary<float, FEATUREDATA> FL  { get; set; }
        public static Dictionary<float, FEATUREDATA> TC  { get; set; }
        public static Dictionary<float, FEATUREDATA> BB1 { get; set; } // back of bench
        
        public static Dictionary<float, FEATUREDATA> TOE { get; set; }
        public static Dictionary<float, FEATUREDATA> TOP { get; set; }
        public static Dictionary<float, FEATUREDATA> PL  { get; set; }
        
        public static Dictionary<string, Dictionary<float, FEATUREDATA>> dNOM  { get; set; }        
        public static Dictionary<string, Dictionary<float, FEATUREDATA>> dWALL { get; set; }
        public static Dictionary<string, Dictionary<float, FEATUREDATA>> dGUT  { get; set; }

        public static Dictionary<int, Dictionary<string, Dictionary<float, FEATUREDATA>>> dWALLs { get; set; }        
        public static Dictionary<int, Dictionary<string, Dictionary<float, FEATUREDATA>>> dGUTs  { get; set; }
    }

    public class WALLPOI
    {
        public float Sta;
        public float Elev;
        public Point3d pnt3d;
        public string AlignName;
        public int AlignSegNum;
        public string Desc;
    }

    public class WALL_POIs{
        public List<POI> POI_TOW { get; set; }
        public List<POI> POI_TOF { get; set; }
    }

    public class FEATUREDATA{
        public string AlignName { get; set; }
        public float Station   { get; set; }
        public float Offset    { get; set; }
        public float Elev      { get; set; }
    }

    public class EXGROUND{
        public FEATUREDATA RF   {get; set; }
        public FEATUREDATA TC   { get; set; }
        public FEATUREDATA TOE  { get; set; }
        public FEATUREDATA TOP  { get; set; }
        public FEATUREDATA PL   { get; set; }
        public FEATUREDATA Off5 { get; set; }
    }

    public class WALLDATA{
        public float XT { get; set; }
        //distance form TC perp to PL
        public float X0 { get; set; }
        public float X1 { get; set; }
        public float X2 { get; set; }

        public float B1 { get; set; }
        public float B2 { get; set; }
        public float B3 { get; set; }

        public float RFy { get; set; }
        public float PLy { get; set; }

        public float TCy { get; set; }
        public float dYh { get; set; }

        public float SG { get; set; }
        public float S0 { get; set; }
        public float S1 { get; set; }
        public float S2 { get; set; }
        public float S3 { get; set; }

        public EXGROUND EX { get; set; }
    }

    public class SECTIONDATA{
        public float OFFSET { get; set; }
        public float ELEV   { get; set; }
    }

    public class SECTIONDATASET{
        public SECTIONDATA RF  { get; set; }
        public SECTIONDATA FL  { get; set; }
        public SECTIONDATA TC  { get; set; }
        public SECTIONDATA BB1 { get; set; }
        public SECTIONDATA BB2 { get; set; }
        public SECTIONDATA BB3 { get; set; }
        public SECTIONDATA TOE { get; set; }
        public SECTIONDATA TOP { get; set; }
        public SECTIONDATA PL  { get; set; }

        public SECTIONDATA W0 { get; set; }
        public SECTIONDATA W1 { get; set; }
        public SECTIONDATA W2 { get; set; }
        public SECTIONDATA W3 { get; set; }
        public SECTIONDATA W4 { get; set; }
        public SECTIONDATA W5 { get; set; }
        public SECTIONDATA W6 { get; set; }
        public SECTIONDATA W7 { get; set; }
        public SECTIONDATA W8 { get; set; }
        public SECTIONDATA W9 { get; set; }

        public SECTIONDATA GB { get; set; }
        public SECTIONDATA G0 { get; set; }

        public float X  { get; set; }
        public float X0 { get; set; }
        public float X1 { get; set; }
        public float X2 { get; set; }
        public float XT { get; set; }

        public float SG  { get; set; }
        public float dYh { get; set; }

        public EXGROUND EX   { get; set; }
        public float STA    { get; set; }
        public bool RESOLVED { get; set; }
    }

    public class PNT_XY{
        public float x { get; set; }
        public float y { get; set; }
    }

    public class STA_ELEV{
        public float ELEV;
        public float STA;
        public float CLEAR;
    }
}
