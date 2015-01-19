using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

using Base_Tools45;
using gp = Grading.Grading_Public;

using System.Diagnostics;

namespace Grading
{
	public static class Grading_CalcBasePnt
	{
		public static Point3d
		calcBasePnt2d(Point3d pnt3dTAR, Point3d pnt3dBEG, Point3d pnt3dEND)
		{
			Point3d pnt3dBASE = Pub.pnt3dO;

			double length = pnt3dBEG.getDistance(pnt3dEND);                 //2d length
			double distance = pnt3dTAR.getOrthoDist(pnt3dBEG, pnt3dEND);    //2d length
			Debug.Print(distance.ToString());
			double angle = Measure.getAzRadians(pnt3dBEG, pnt3dEND);        //angle in XY plane

			System.Windows.Forms.Keys mods = System.Windows.Forms.Control.ModifierKeys;
			gp.shift = (mods & System.Windows.Forms.Keys.Shift) > 0;
			bool control = (mods & System.Windows.Forms.Keys.Control) > 0;

			if (gp.shift)
			{
				pnt3dBASE = pnt3dBEG.traverse(angle, distance);                 //3dPoint with z = 0
			}
			else
			{
				if (distance > 0 && distance < length)
				{
					pnt3dBASE = pnt3dBEG.traverse(angle, distance);             //3dPoint with z = 0
					View.remCursorBadge();
					gp.inBounds = true;
				}else{
					gp.inBounds = false;
					View.addCursorBadge();
				}
			}
			return pnt3dBASE;
		}

		public static Point3d
		calcBasePnt3d(Point3d pnt3dTAR, Polyline3d poly3d, int vertexNo)
		{
			Point3d pnt3dBASE = Pub.pnt3dO;

			Point3dCollection pnts3d = poly3d.getCoordinates3d();
			Point3d pnt3dBEG = pnts3d[vertexNo + 0];
			Point3d pnt3dEND = pnts3d[vertexNo + 1];

			double length = pnt3dBEG.getDistance(pnt3dEND);
			double slope = (pnt3dEND.Z - pnt3dBEG.Z) / length;
			double distance = pnt3dTAR.getOrthoDist(pnt3dBEG, pnt3dEND);
			if (distance > 0 && distance < length)
			{
				double angle = Measure.getAzRadians(pnt3dBEG, pnt3dEND);
				pnt3dBASE = pnt3dBEG.traverse(angle, distance, slope);
			}
			return pnt3dBASE;
		}

		public static Point3d
		calcBasePnt3d(Point3d pnt3dTAR, Point3d pnt3dBEG, Point3d pnt3dEND)
		{
			Point3d pnt3dBASE = Pub.pnt3dO;

			double length = pnt3dBEG.getDistance(pnt3dEND);
			double slope = pnt3dBEG.getSlope(pnt3dEND);
			double distance = pnt3dTAR.getOrthoDist(pnt3dBEG, pnt3dEND);
			double angle = pnt3dBEG.getDirection(pnt3dEND);

			if (gp.shift){
				pnt3dBASE = pnt3dBEG.traverse(angle, distance, slope);
			}
			else{
			if (distance > 0 && distance < length)
				pnt3dBASE = pnt3dBEG.traverse(angle, distance, slope);
			}
			return pnt3dBASE;
		}

		public static Point3d
		calcBasePntAndTargetPntElev(Point3d pnt3dTAR, double grade, Polyline3d poly3d, int vertexNo, out double elevTAR, out double station)
		{
			station = 0.0;
			elevTAR = 0.0;
			gp.pnt3dX = Pub.pnt3dO;

			Point3dCollection pnts3d = poly3d.getCoordinates3d();
			Point3d pnt3dBEG = pnts3d[vertexNo + 0];
			Point3d pnt3dEND = pnts3d[vertexNo + 1];

			double length = pnt3dBEG.getDistance(pnt3dEND);                         //result is 2d distance
			double slope = (pnt3dEND.Z - pnt3dBEG.Z) / length;
			station = Math.roundDown4(pnt3dTAR.getOrthoDist(pnt3dBEG, pnt3dEND));   //result is 2d distance

			if (gp.shift){
				double angle = Measure.getAzRadians(pnt3dBEG, pnt3dEND);
				gp.pnt3dX = pnt3dBEG.traverse(angle, station, slope);
				double offset = gp.pnt3dX.getDistance(pnt3dTAR);                    //result is 2d distance
				elevTAR = gp.pnt3dX.Z + offset * grade;               
			}else{
				if (station > 0 && station < length)
				{
					double angle = Measure.getAzRadians(pnt3dBEG, pnt3dEND);
					gp.pnt3dX = pnt3dBEG.traverse(angle, station, slope);
					double offset = gp.pnt3dX.getDistance(pnt3dTAR);                //result is 2d distance
					elevTAR = gp.pnt3dX.Z + offset * grade;
				}
			}
			return gp.pnt3dX;
		}
	}
}
