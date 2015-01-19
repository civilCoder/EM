using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;

using Base_Tools45;
using Base_Tools45.C3D;

namespace PadCerts
{
	static public class PC_App
	{
		public static void processPoints(SelectionSet objSSet, List<Point3d> varPntsLeader)
		{
			winPadCerts wPadCerts = PC_Forms.pcForm.wPadCerts;

			int k = varPntsLeader.Count;
			List<double> dx = new List<double>();
			List<double> dy = new List<double>();

			for(int i = 1; i < k; i++){
				dx.Add(varPntsLeader[i].X - varPntsLeader[i - 1].X);
				dy.Add(varPntsLeader[i].Y - varPntsLeader[i - 1].Y);
			}

			double dblDir = varPntsLeader[k - 2].getDirection(varPntsLeader[k - 1]);

			string strName = System.Convert.ToString(wPadCerts.lbxSurfaceName.SelectedItem);
	
			TinSurface objSurface = Surf.getTinSurface(strName);
			ObjectId[] ids = objSSet.GetObjectIds();

			for (int i = 0; i < ids.Length; i++)
			{

				List<Point3d> pnts3dLdr = new List<Point3d>();
				Point3d pnt3d = ids[i].getCogoPntCoordinates();

				double dblElevPnt = pnt3d.Z;
				pnts3dLdr.Add(new Point3d(pnt3d.X, pnt3d.Y, 0));

				for (int j = 0; j < dx.Count; j++)
				{
					pnt3d = new Point3d(pnt3d.X + dx[j], pnt3d.Y + dy[j], 0);
					pnts3dLdr.Add(pnt3d);
				}

				Point3d pnt3dInsTxt = pnt3d;

				double dblElevSurface = objSurface.FindElevationAtXY(pnts3dLdr[0].X, pnts3dLdr[0].Y);
		
				if (wPadCerts.optBase.IsChecked == true)
				{
					dblElevSurface = dblElevSurface + (System.Convert.ToDouble(wPadCerts.tbxPvmtThickness.Text)) / 12;
				}
				else if (wPadCerts.optSG.IsChecked == true)
				{
					dblElevSurface = dblElevSurface + (System.Convert.ToDouble(wPadCerts.tbxTotalSection.Text)) / 12;
				}

                PacCerts.PC_LblElevDiff.labelElevDiff(dblElevPnt, dblElevSurface, pnts3dLdr, pnt3dInsTxt, dblDir);		
			}
	
		}

	}
}
