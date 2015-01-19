using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace Base_Tools45
{
    public enum filestatus : int
    {
        isLocked            = 0, 
        isAvailable         = 1, 
        isOpenLocal         = 2, 
        isOpenLocalReadOnly = 3
    }

    public enum osModes : int
    {
        NON     = 0,
        END     = 1,
        MID     = 2,
        CEN     = 4,
        NOD     = 8,
        QUAD    = 16,
        INT     = 32,
        INS     = 64,
        PER     = 128,
        TAN     = 256,
        NEAR    = 512,
        QUI     = 1024,
        APPINT  = 2048,
        EXT     = 4096,
        PAR     = 8192
    }

    public struct arcInfo
    {
        public double distFromBeg;
        public double distToNext;
        public double elev;
        public double grd;
        public ObjectId idCgPnt;
        public List<Vector3d> v3ds;
    }

    public struct GRID_DATA
    {
        public string Name;
        public List<POI> POIs;
    }

    public struct PIPE_DATA
    {
        public double Size;
        public double Invert;
    }

    public struct DataSet
    {
        public string Name;
        public string Layer;
        public int COUNT;
        public uint Lower;
        public uint Upper;
        public string ObjectName;
        public List<uint> Nums;
        public List<double> Stations;
        public bool Missing;
    }

    public static class clr
    {
        public static Color byl = Misc.getColorByLayer();
        public static Color red = Misc.getColorByBlock(1);
        public static Color yel = Misc.getColorByBlock(2);
        public static Color grn = Misc.getColorByBlock(3);
        public static Color cyn = Misc.getColorByBlock(4);
        public static Color blu = Misc.getColorByBlock(5);
        public static Color mag = Misc.getColorByBlock(6);
        public static Color wht = Misc.getColorByBlock(7);
        public static Color c8 = Misc.getColorByBlock(8);
        public static Color c9 = Misc.getColorByBlock(9);
        public static Color c10 = Misc.getColorByBlock(10);
        public static Color c11 = Misc.getColorByBlock(11);
        public static Color c70 = Misc.getColorByBlock(70);
        public static Color c90 = Misc.getColorByBlock(90);
        public static Color c120 = Misc.getColorByBlock(120);
        public static Color c140 = Misc.getColorByBlock(140);
        public static Color c150 = Misc.getColorByBlock(150);
        public static Color c160 = Misc.getColorByBlock(160);
        public static Color c170 = Misc.getColorByBlock(170);
        public static Color c180 = Misc.getColorByBlock(180);

    }


    public struct CogoPnt
    {
        public long Num;
        public string Desc;
        public string Layer;
        public Handle hAlign;
    }

    public struct FieldData
    {
        public string PntNum;
        public double ElevStake;
        public double ElevDesign;
    }

    public struct ProjectData
    {
        public string Client;
        public string Number;
        public string Name;
        public string Location;
        public string Coordinator;
    }

    public struct PntRange
    {
        public long BegNum;
        public long EndNum;
    }

    public struct AlgnEntData
    {
        public int      ID;
        public string   Name;
        public string   Type;
        public double   StaBeg;
        public double   StaEnd;
        public double   Length;
        public double   Radius;
        public double   Direction;
        public double   AngDeflection;      //deflection from prior segment direction
        public bool     ClockWise;
        public int      EntBefore;
        public Point2d  pnt2dBeg;
        public Point2d  pnt2dEnd;
    }

    public struct StakedPnt
    {
        public Handle hAlign;
        public Handle hCgPnt;
        public double Station;
        public double Offset;
        public double Elev;
        public string Desc;
        public uint Number;
    }

    public struct StakePntSum
    {
        public string AlignHandle;
        public List<long> PntsNums;
    }

    public struct GRIDPROP
    {
        public Line GRIDMEM;
        public double OFFSET;
        public double DELTA;
    }

    public struct DELTA
    {
        public double delta;
        public int count;
    }

    public struct PNT_XY
    {
        public double x;
        public double y;
    }

    public struct staOffElev
    {
        public int pntIndex;
        public int segIndex;
        public double staAlign;
        public double staSeg;
        public double off;
        public double elev;
        public bool angPt;
    }

    public static class Pub
    {
        public static string JUSTIFYLEFT = string.Format("{0}A1{1}{2}pxql{3}", char.ConvertFromUtf32(92), char.ConvertFromUtf32(59), char.ConvertFromUtf32(92), char.ConvertFromUtf32(59));
        public static string JUSTIFYCENTER = string.Format("{0}A1{1}{2}pxqc{3}", char.ConvertFromUtf32(92), char.ConvertFromUtf32(59), char.ConvertFromUtf32(92), char.ConvertFromUtf32(59));
        public static string JUSTIFYRIGHT = string.Format("{0}A1{1}{2}pxqr{3}", char.ConvertFromUtf32(92), char.ConvertFromUtf32(59), char.ConvertFromUtf32(92), char.ConvertFromUtf32(59));

        public static Point3d pnt3dO = new Point3d(-1, -1, -1);

        public const double radius = 0.10625;   //based on inspection per BLT

        public const double PI = System.Math.PI;

        public static int DrawingUnits { get; set; }

        public static double Elev { get; set; }

        public static double Slope { get; set; }

        public static double Dist { get; set; }

        public static double dZ { get; set; }

        public static double offH { get; set; }

        public static double offV { get; set; }

        public static double slp1 { get; set; }

        public static double slp2 { get; set; }

        public static bool shiftKey { get; set; }
        public static bool cntrlKey { get; set; }
        public static bool altX   { get; set; }
           
    }

    public static class apps
    {
        public static List<string> lstApps = new List<string> {
            "lblPnts",              //Survey point labels
            "lblPntsPT",            //Survey point labels at PL
            "lnkBrks",              //Grading Breaklines
            "lnkBrks2",             //Grading Breaklines - BV, BG, BC, etc.
            "lnkBrks3",             //Grading Breaklines - offset info for BV, BG, BC, etc.
            "lnkBubs",              //Bubble callout
            "lnkBubsLdrEndPnt",     //Bubble callout leader info
            "lnkCO",                //Callout
            "lnkDimPL",             //Mapping PL dimension labeling
            "lnkDP",                //Deepen panel
            "lnkGS",                //Grading - slope labels
            "lnkLD",                //Leader
            "lnkMNP",               //Pipe Network
            "lnkRiser",             //Riser at Building Apron
            "lnkSLP"                //Grading Breaklines for slope
        };

        public const string lblPnts = "lblPnts";
        public const string lblPntsPT = "lblPntsPT";
        public const string lnkBrks = "lnkBrks";
        public const string lnkBrks2 = "lnkBrks2";
        public const string lnkBrks3 = "lnkBrks3";
        public const string lnkBubs = "lnkBubs";
        public const string lnkBubsLdrEndPnt = "lnkBubsLdrEndPnt";
        public const string lnkCO = "lnkCO";
        public const string lnkDimPL = "lnkDimPL";
        public const string lnkDP = "lnkDP";
        public const string lnkGS = "lnkGS";
        public const string lnkLD = "lnkLD";
        public const string lnkMNP = "lnkMNP";
        public const string lnkRiser = "lnkRiser";
        public const string lnkSLP = "lnkSLP";
    }

    public class Node3d
    {
        public double X, Y, Z, Bulge;

        public Node3d(double x, double y, double z, double bulge)
        {
            X = x;
            Y = y;
            Z = z;
            Bulge = bulge;
        }
    }

    public class POI
    {
        public string ClassObj = "";           //1
        public string Desc0 = "";           //2
        public string DescX = "";           //3
        public double Station = 0;            //4
        public double OFFSET = 0;            //5
        public double Elevation = 0;            //6
        public double ElevTOW = 0;            //7
        public double ElevTOF = 0;            //8
        public double Invert = 0;            //9
        public double Size = 0;            //10
        public double AngDelta = 0;            //11
        public double AngDir = 0;            //12
        public double AngChord = 0;            //13
        public bool isRightHand = false;        //14
        public bool isClosed = false;        //15
        public bool isStep = false;        //16
        public string CrossDesc = "";           //17
        public string CrossAlign = "";           //18
        public double CrossAlignInv = 0;            //19
        public double CrossAlignSta = 0;            //20
        public double CrossAlignSize = 0;            //21
        public string PntNum = "";           //22
        public string PntSource = "";           //23
        public string Type = "";           //24
        public double Bulge = 0;            //25
        public double Radius = 0;            //26
        public int Side = -1;           //27
        public double SlopeAhead = 0;            //28
        public double SlopeBack = 0;            //29
        public double SlopeH2H = 0;            //30
        public Point3d CenterPnt = Pub.pnt3dO;   //31
    }

    public class PNT_DATA
    {
        public string   ALIGN;
        public string   DESC;
        public long     NUM;
        public double   OFFSET;
        public double   STA;
        public double   SLOPEAHEAD;
        public double   SLOPEBACK;
        public double   x;
        public double   y;
        public double   z;
    }

    public class SEG_PROP
    {
        public double LENGTH = 0;

        public double DIR_AHEAD = 0;
        public double DIR_BACK = 0;
        public double DELTA = 0;

        public double SLOPE_AHEAD = 0;
        public double SLOPE_BACK = 0;

        public Point3d PRV = Pub.pnt3dO;
        public Point3d BEG = Pub.pnt3dO;
        public Point3d END = Pub.pnt3dO;
    }

    public class ANG_PT_PROP
    {
        public double LEN_SEG1 = 0;
        public double LEN_SEG2 = 0;

        public double DIR_SEG1 = 0;
        public double DIR_SEG2 = 0;
        public double ANG_DEFLC = 0;
        public double DELTA = 0;

        public double SLP_SEG1 = 0;
        public double SLP_SEG2 = 0;

        public Point3d PRV = Pub.pnt3dO;
        public Point3d BEG = Pub.pnt3dO;
        public Point3d END = Pub.pnt3dO;
    }

    public class PNT_LIST
    {
        public short index = 0;
        public double length = 0;
    }

    public class WALL_STEP
    {
        public double BEGSTA { get; set; }
        public double ENDSTA { get; set; }
        public double ELEV { get; set; }
    }

    public class WALL_STEPs
    {
        public List<WALL_STEP> TOF { get; set; }
        public List<WALL_STEP> TOW { get; set; }
    }

    public class WALL_DATA
    {
        public string Name { get; set; }
        public Point3d PntLower { get; set; }
        public Point3d PntUpper { get; set; }
        public double StaStart { get; set; }
        public double ElevStart { get; set; }
        public List<Point3d> TOWcoords { get; set; }
        public List<Point3d> TOFcoords { get; set; }
        public double ScaleV { get; set; }
    }

}
