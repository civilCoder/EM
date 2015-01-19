using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace Base_Tools45.C3D
{
	/// <summary>
	///
	/// </summary>
	public static class DrawFloorSlab {

		public static ObjectIdCollection
		build3dPolyFloorSlab(this ObjectId idPolyLIM, Point3d pnt3dCEN, Point3d pnt3dTAR, double dblSlope,
		string nameSurface, string nameLayer, string nameFunction) {

			ObjectIdCollection idsPoly3d = new ObjectIdCollection();
			Polyline polyLIM = (Polyline)idPolyLIM.getEnt();
			if (polyLIM == null)
				return null;

			int k = polyLIM.NumberOfVertices;
			Layer.manageLayers(nameLayer);

			List<Point3d> pnts3d = new List<Point3d>();
			try
			{
				using (BaseObjs._acadDoc.LockDocument())
				{
					if (dblSlope != 0)
					{
						for (int i = 0; i < k; i++)
						{
							Point3d pnt3dX = polyLIM.GetPoint3dAt(i);
							pnt3dX = new Point3d(pnt3dX.X, pnt3dX.Y, pnt3dTAR.Z + Geom.getCosineComponent(pnt3dCEN, pnt3dTAR, pnt3dX) * (dblSlope * -1));
							pnts3d.Add(pnt3dX);
						}
					}else{
						for (int i = 0; i < k; i++)
						{
							Point3d pnt3dX = polyLIM.GetPoint3dAt(i);
							pnt3dX = new Point3d(pnt3dX.X, pnt3dX.Y, pnt3dCEN.Z);
							pnts3d.Add(pnt3dX);
						}                      
					}

					pnts3d.addBrklineSegmentsFloor(out idsPoly3d, apps.lnkBrks);

					//idCgPntsX = new List<ObjectId>();
					//idCgPnt = pnts3d[n - 0].setPoint(out pntNum);
					//idCgPntsX.Add(idCgPnt);
					//idCgPntsX.Add(idCgPntsX[0]);

					//idPoly3d = BrkLine.makeBreakline(apps.lnkBrks, "cmdAVG", idCgPntsX);


					//idPoly3d = Draw.addPoly3d(pnts3d, nameLayer, 1);
					//TypedValue[] tvs = new TypedValue[k];
					//tvs.SetValue(new TypedValue(1001, "lnkBrks"), 0);
					//ObjectIdCollection idsCogoPnt = new ObjectIdCollection();
					//uint pntNum;
					//if (nameFunction == "AVG") {
					//    for (int i = 0; i < k - 1; i++) {
					//        ObjectId idCogoPnt = pnts3d[i].setPoint(out pntNum, "CPNT-ON");
					//        idsCogoPnt.Add(idCogoPnt);
					//        tvs.SetValue(new TypedValue(1005, idCogoPnt.getHandle()), i + 1);
					//    }
					//    idPoly3d.setXData(tvs, "lnkBrks");
					//    tvs = new TypedValue[2];
					//    tvs.SetValue(new TypedValue(1001, "lnkBrks"), 0);
					//    tvs.SetValue(new TypedValue(1005, idPoly3d.getHandle()), 1);
					//    for (int i = 0; i < idsCogoPnt.Count; i++) {
					//        idsCogoPnt[i].setXData(tvs, "lnkBrks");
					//    }
					//}
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(string.Format("{0} Draw3d.cs: line: 64", ex.Message));
			}
			return idsPoly3d;
		}

		public static List<ObjectId>
		addBrklineSegmentsFloor(this List<Point3d> pnts3d, out ObjectIdCollection idsPoly3d, string nameApp)
		{
			idsPoly3d = new ObjectIdCollection();
			uint pntNum = 0;

			List<ObjectId> idCgPntList = new List<ObjectId>();
			ObjectId idCgPnt0 = ObjectId.Null, idCgPnt1 = ObjectId.Null, idPoly3d = ObjectId.Null;

			int n = pnts3d.Count;
			for (int i = 1; i < n; i++)
			{
				if (i == 1)
				{ //first segment

					idCgPnt0 = pnts3d[0].setPoint(out pntNum);
					idCgPntList.Add(idCgPnt0);           //overall list

					idCgPnt1 = pnts3d[1].setPoint(out pntNum);
					idCgPntList.Add(idCgPnt1);

					idPoly3d = Draw.addPoly3d(pnts3d[0], pnts3d[1], "CPNT-BRKLINE");
					idsPoly3d.Add(idPoly3d);

					idPoly3d.lnkPntsAndPoly3d(idCgPnt0, idCgPnt1, nameApp);
				}
				else if (i == n - 1)
				{
					idPoly3d = Draw.addPoly3d(pnts3d[n - 2], pnts3d[n - 1], "CPNT-BRKLINE");
					idsPoly3d.Add(idPoly3d);

					idCgPnt0 = idCgPntList[i - 1];
					idCgPntList.Add(idCgPnt0);

					idCgPnt1 = idCgPntList[0];
					idPoly3d.lnkPntsAndPoly3d(idCgPnt0, idCgPnt1, nameApp);
				}
				else
				{
					idCgPnt0 = idCgPntList[i - 1];
					idCgPnt1 = pnts3d[i].setPoint(out pntNum);
					idCgPntList.Add(idCgPnt1);

					idPoly3d = Draw.addPoly3d(pnts3d[i - 1], pnts3d[i], "CPNT-BRKLINE");
					idsPoly3d.Add(idPoly3d);

					idPoly3d.lnkPntsAndPoly3d(idCgPnt0, idCgPnt1, nameApp);
				}
			}
			return idCgPntList;
		}


	}
}