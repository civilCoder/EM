using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;

using Base_Tools45;

namespace Grading {
	public static class Grading_xData {
		public static void
		checkPntXData() {
		}

		public static ResultBuffer
		checkPointXData2Nodes(ObjectId idCogoPnt, ResultBuffer RBpnt, string nameApp) {
			try {
				List<Handle> handlesP3d = RBpnt.rb_handles();

				int i = 0;
				foreach (Handle hP3d in handlesP3d) {
					Polyline3d poly3d = (Polyline3d)Db.handleToObject(hP3d.ToString());

					if ((poly3d != null)) {
						if (poly3d.Length != 0) {
							ResultBuffer RBp3d = poly3d.ObjectId.getXData(nameApp);
							List<Handle> handlesPnts = RBp3d.rb_handles();
							Handle hPnt = idCogoPnt.getHandle();
							Point3dCollection pnts3d = poly3d.getCoordinates3d();
							Point3d pnt3dCogoPnt = hPnt.getCogoPntCoordinates();

							if (System.Math.Round(pnts3d[0].X, 3) == System.Math.Round(pnt3dCogoPnt.X, 3) &&
								System.Math.Round(pnts3d[0].Y, 3) == System.Math.Round(pnt3dCogoPnt.Y, 3)) {
								if (pnts3d[0].Z != pnt3dCogoPnt.Z) {
									poly3d.setBegPnt(pnt3dCogoPnt);
								}
								if (!handlesPnts.Contains(hPnt)) {
									handlesPnts.Add(hPnt);
									poly3d.ObjectId.setXData(handlesPnts, nameApp);
								}
							}else if (System.Math.Round(pnts3d[1].X, 3) == System.Math.Round(pnt3dCogoPnt.X, 3) &&
									  System.Math.Round(pnts3d[1].Y, 3) == System.Math.Round(pnt3dCogoPnt.Y, 3)) {
								if (pnts3d[1].Z != pnt3dCogoPnt.Z) {
									poly3d.setEndPnt(pnt3dCogoPnt);
								}
								if (!handlesPnts.Contains(hPnt)) {
									handlesPnts.Add(hPnt);
									poly3d.ObjectId.setXData(handlesPnts, nameApp);
								}
							}else {
								poly3d.ObjectId.delete();
								handlesP3d.RemoveAt(i);
								RBpnt = handlesP3d.handles_RB(nameApp);
								idCogoPnt.setXData(RBpnt.AsArray(), nameApp);
							}
						}else {
							poly3d.ObjectId.delete();
							handlesP3d.RemoveAt(i);
							RBpnt = handlesP3d.handles_RB(nameApp);
                            idCogoPnt.setXData(RBpnt.AsArray(), nameApp);
						}
					}else {
						handlesP3d.RemoveAt(i);
						RBpnt = handlesP3d.handles_RB(nameApp);
                        idCogoPnt.setXData(RBpnt.AsArray(), nameApp);
					}
					i++;
				}
			}
			catch (System.Exception ex){
	BaseObjs.writeDebug(ex.Message + " Grading_xData.cs: line: 70");
			}
			return RBpnt;
		}

		public static void
		checkPointXDataXNodes(CogoPoint cogoPnt, string nameApp) {
			try {
				ResultBuffer RB0 = cogoPnt.ObjectId.getXData(nameApp);
				if (RB0 != null) {
					List<Handle> handles = RB0.rb_handles();
					Handle h = "0".stringToHandle();
					ObjectId id = ObjectId.Null;
					int k = handles.Count;
					for (int i = k -1; i > -1; i-- ) {
						try
						{
							h = handles[i];
							id = h.getObjectId();
							if (!id.IsValid || id.IsErased || id.IsEffectivelyErased){
								handles.RemoveAt(i);
								continue;
							}
						}
						catch (System.Exception ex)
						{
							 handles.RemoveAt(i);                       	
						}
						Polyline3d poly3d = (Polyline3d)h.getEnt();
						if (poly3d == null)
							handles.RemoveAt(i);
					}
					cogoPnt.ObjectId.clearXData(nameApp);
					cogoPnt.ObjectId.setXData(handles.handles_RB(nameApp), nameApp);
				}
			}
			catch (System.Exception ex){
	            BaseObjs.writeDebug(ex.Message + " Grading_xData.cs: line: 107");
			}
		}

		public static void
		lnkPntsAndPoly3d(ObjectId idPoly3d, ObjectId idCogoPnt1, ObjectId idCogoPnt2, string nameApp) {
			try {
				Handle hPoly3d = idPoly3d.getHandle();
				TypedValue[] tvs = null;

				ResultBuffer rb = idCogoPnt1.getXData(nameApp); //if from MBL nameApp = lnkBrks2
				if (rb == null) {
					tvs = new TypedValue[2];
					tvs.SetValue(new TypedValue(1001, nameApp), 0);
					tvs.SetValue(new TypedValue(1005, hPoly3d), 1);
					rb = new ResultBuffer(tvs);
				}else {
					rb.Add(new TypedValue(1005, hPoly3d));
				}
				idCogoPnt1.setXData(rb, nameApp);               //add poly3d to point xdata

				rb = null;
				rb = idCogoPnt2.getXData(nameApp);
				if (rb == null) {
					tvs = new TypedValue[2];
					tvs.SetValue(new TypedValue(1001, nameApp), 0);
					tvs.SetValue(new TypedValue(1005, hPoly3d), 1);
					rb = new ResultBuffer(tvs);
				}else {
					rb.Add(new TypedValue(1005, hPoly3d));
				}
				idCogoPnt2.setXData(rb, nameApp);               //add poly3d to point xdata

				tvs = new TypedValue[3];
				tvs.SetValue(new TypedValue(1001, nameApp), 0);
				tvs.SetValue(new TypedValue(1005, idCogoPnt1.getHandle()), 1);
				tvs.SetValue(new TypedValue(1005, idCogoPnt2.getHandle()), 2);
				rb = new ResultBuffer(tvs);
				idPoly3d.setXData(rb, nameApp);                 //add cogo points to poly3d
			}
			catch (System.Exception ex){
	            BaseObjs.writeDebug(ex.Message + " Grading_xData.cs: line: 148");
			}
		}

		public static void
		replacePntXData(CogoPoint cogoPnt, CogoPoint cogoPntX, Polyline3d poly3d) {
			try {
				ResultBuffer RB0 = poly3d.ObjectId.getXData(apps.lnkBrks);
				List<Handle> handles = RB0.rb_handles();     //get list of unique non Zero handles

				int i = handles.IndexOf(cogoPnt.Handle);
				handles.RemoveAt(i);
				handles.Insert(i, cogoPntX.Handle);

				poly3d.ObjectId.clearXData(apps.lnkBrks);
				poly3d.ObjectId.setXData(handles.handles_RB(apps.lnkBrks), apps.lnkBrks3);
			}
			catch (System.Exception ex){
	            BaseObjs.writeDebug(ex.Message + " Grading_xData.cs: line: 166");
			}
		}

		public static void
		updatePntXData(CogoPoint cogoPnt, ObjectId idPoly3d) {
			try {
				checkPointXDataXNodes(cogoPnt, apps.lnkBrks);
				ResultBuffer RB0 = cogoPnt.ObjectId.getXData(apps.lnkBrks);
				Handle hPoly3d = idPoly3d.getHandle();

				if (RB0 != null) {
					List<Handle> handles = RB0.rb_handles();
					handles.Add(hPoly3d);
					cogoPnt.ObjectId.clearXData(apps.lnkBrks);
					cogoPnt.ObjectId.setXData(handles.handles_RB(apps.lnkBrks), apps.lnkBrks);
				}else {
					TypedValue[] tvs = new TypedValue[2];
					tvs.SetValue(new TypedValue(1001, apps.lnkBrks), 0);
					tvs.SetValue(new TypedValue(1005, hPoly3d), 1);

					ResultBuffer RBX = new ResultBuffer(tvs);
					cogoPnt.ObjectId.setXData(tvs, apps.lnkBrks);
				}
			}
			catch (System.Exception ex){
	BaseObjs.writeDebug(ex.Message + " Grading_xData.cs: line: 192");
			}
		}
	}
}
