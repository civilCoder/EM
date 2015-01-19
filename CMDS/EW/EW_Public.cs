using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System.Collections.Generic;

namespace EW
{
	public static class EW_Pub
	{

		public static double K_BRACE_V;
		public static double K_BRACE_H;
		public static double COLUMN_FOOTINGS_V;
		public static double OUTER_FOOTINGS_V;
		public static double REMOVE_REPLACE_V;
		public static double REMOVE_REPLACE_BLDG_V;
		public static double OX_LIMIT_H;

		public static double FOOTING_LIMIT_IN_H;

		public const double pi = 3.14159265359;

		public const string SURFACE = "CPNT-ON";
		public static bool boolDebug;
		public static List<DEPTH> depths { get; set; }

		public static bool debug = false;
	}

	public class EW_Data{
		public string ITERATION;
		public string DATE;
		public string USER;
		public double AREA_SITE;
		public double SG_MEAN_ELEV;
		public double VOL_CUT_TOT;
		public double VOL_CUT_ADJ_NET;
		public double VOL_CUT_SHRINK;
		public double VOL_CUT_NET;
		public double VOL_FILL_TOT;
		public double VOL_FILL_ADJ_NET;
		public double VOL_FILL_NET;
		public double VOL_NET;
		public double SITE_ADJUST;      
	}

	public class TABLEDATA{
		public double SG;
		public double OX;
		public string KEY;
		public string DESC;
	}

	public class DEPTH
	{
		public string KEYWORD;
		public double SG;
		public double OX;
	}

	public class SEG
	{
		public int Index;
		public double Length;
		public double Direction;
		public Point3d Beg;
		public Point3d End;
	}

	public class ADDPNT
	{
		public double X;
		public double Y;
		public int   Va;
		public int   Vb;
	}

	public class StaOff
	{
		public double Sta;
		public double L;
		public double R;
	}

	public class Vector
	{
		public double dir;
		public double dist;
	}
}