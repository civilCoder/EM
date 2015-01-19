using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Windows.Forms;


namespace Wall
{
	public static class Wall_Alignment1
	{
		const double PI = System.Math.PI;
		static Wall_Form.frmWall4 fWall4 = Wall_Forms.wForms.fWall4;

		public static ObjectId
		Create_Align_Profile_By3dPoly2a(Alignment objAlignPL, string strName, string strNameAlign, ObjectId idLayer, ObjectId idPoly3dRF){
			ObjectId idAlign = ObjectId.Null;

			ObjectId idProfileStyle = Prof_Style.getProfileStyle("WALL");
			ObjectId idStyleLabelSet = Prof_Style.getProfileLabelSetStyle("WALL");

			//ObjectId idProfileStyle = Prof_Style.getProfileStyle("Standard");
			//ObjectId idStyleLabelSet = Prof_Style.getProfileLabelSetStyle("Standard");

			bool boolBeg = false;
			bool boolEnd = false;

			ObjectId idPolyRF = ObjectId.Null;
			Point3d pnt3dBegREF = Pub.pnt3dO, pnt3dEndREF = Pub.pnt3dO, pnt3dBegPL = Pub.pnt3dO, pnt3dEndPL = Pub.pnt3dO;
			double dblStaBegRef = 0, dblOffBegRef = 0;

			if (objAlignPL.Entities.Count > 1) {
				MessageBox.Show("Program is set up for ONE simple tangent alignment. Exiting.....");
				return idAlign;


			} else {
				List<Point3d> pnt3dsPoly3d = idPoly3dRF.getCoordinates3dList();

				idPolyRF = idPoly3dRF.toPolyline(idPoly3dRF.getLayer());

				pnt3dBegREF = idPolyRF.getBegPnt();
				pnt3dEndREF = idPolyRF.getEndPnt();

				pnt3dBegPL = objAlignPL.StartPoint;
				pnt3dEndPL = objAlignPL.EndPoint;

				double dblAngle = Measure.getAzRadians(pnt3dBegPL, pnt3dEndPL);
				double dblDistBeg = Geom.getPerpDistToLine(pnt3dBegPL, pnt3dEndPL, pnt3dBegREF);
				double dblDistEnd = Geom.getPerpDistToLine(pnt3dBegPL, pnt3dEndPL, pnt3dEndREF);


				if (dblDistBeg > dblDistEnd) {
					using (BaseObjs._acadDoc.LockDocument()) {
						idPolyRF.reversePolyX();
					}

					pnt3dBegREF = idPolyRF.getBegPnt();
					pnt3dEndREF = idPolyRF.getEndPnt();

					dblDistBeg = Geom.getPerpDistToLine(pnt3dBegPL, pnt3dEndPL, pnt3dBegREF);
					dblDistEnd = Geom.getPerpDistToLine(pnt3dBegPL, pnt3dEndPL, pnt3dEndREF);

				}


				if (dblDistBeg < 0) {
					boolBeg = true;
					pnt3dBegPL = Geom.traverse_pnt3d(pnt3dBegPL, dblAngle - PI, dblDistBeg * -1 + 10);

				}


				if (dblDistEnd > objAlignPL.Length + 10) {
					boolEnd = true;
					pnt3dEndPL = Geom.traverse_pnt3d(pnt3dEndPL, dblAngle, objAlignPL.Length - dblDistEnd + 10);

				}

				string strlayer = objAlignPL.Layer;
				ObjectId idPolyPL = ObjectId.Null;

				if (boolBeg | boolEnd) {
					string strAlignName = objAlignPL.Name.ToString();
					idLayer = Layer.manageLayers(strlayer);

					using (BaseObjs._acadDoc.LockDocument()) {
						using (Transaction TR = BaseObjs.startTransactionDb()) {

							Align.removeAlignment(strAlignName);

							objAlignPL = null;

							List<Point3d> pnt3dColl = new List<Point3d>();
							pnt3dColl.Add(pnt3dBegPL);
							pnt3dColl.Add(pnt3dEndPL);

							idPolyPL = Base_Tools45.Draw.addPoly(pnt3dColl, strlayer);


							try {
								objAlignPL = Align.addAlignmentFromPoly(strAlignName, strlayer, idPolyPL, "Thienes_Proposed", "Thienes_Proposed", true);


							} catch (Autodesk.AutoCAD.Runtime.Exception ) {
								objAlignPL = Align.addAlignmentFromPoly(strAlignName, strlayer, idPolyPL, "Standard", "Standard", true);

							}

							objAlignPL.ReferencePointStation = 1000.0;

							try {
								objAlignPL.StationOffset(pnt3dBegREF.X, pnt3dBegREF.Y, ref dblStaBegRef, ref dblOffBegRef);
							} catch (Autodesk.Civil.PointNotOnEntityException ) {
								dblStaBegRef = 0.0;
							}
							double easting =  0, northing = 0;
							objAlignPL.PointLocation(dblStaBegRef, 0.0, ref easting, ref northing);

							Point2d pnt2dRef = new Point2d(easting, northing);

							objAlignPL.ReferencePoint = pnt2dRef;

							fWall4.AlignPL = objAlignPL;

							bool exists = false;
							TinSurface objSurfaceEXIST = Surf.getTinSurface("EXIST", out exists);

							ObjectId idAlignStyle = Align_Style.getAlignmentStyle("Standard");
							ObjectId idAlignStyleSet = Align_Style.getAlignmentLabelSetStyle("Standard");


							try {
								Prof.removeProfile(objAlignPL.ObjectId, "EXIST");


							} catch (Autodesk.AutoCAD.Runtime.Exception ) {
							}

							idAlign = objAlignPL.ObjectId;
							ObjectId idSurf = objSurfaceEXIST.ObjectId;

							try {
								Prof.addProfileBySurface("EXIST", idAlign, idSurf, idLayer, idProfileStyle, idStyleLabelSet);

							} catch (Autodesk.AutoCAD.Runtime.Exception exp) {
								Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog(exp.ToString());
							}

							TR.Commit();

						}
					}

				}

				Alignment objAlignRF = null;
				try {
					objAlignRF = Align.addAlignmentFromPoly(strNameAlign, strlayer, idPolyRF, "Thienes_Proposed", "Thienes_Proposed", true);


				} catch (Autodesk.AutoCAD.Runtime.Exception ) {
					objAlignRF = Align.addAlignmentFromPoly(strNameAlign, strlayer, idPolyRF, "Standard", "Standard", true);

				}

				double dblStation = 0;
				double dblOffset = 0;

				try {
					objAlignRF.StationOffset(pnt3dsPoly3d[0].X, pnt3dsPoly3d[0].Y, ref dblStation, ref dblOffset);
				} catch (Autodesk.Civil.PointNotOnEntityException ) {
					dblStation = 0.0;
				}

				if (dblStation != objAlignRF.StartingStation) {
					using (BaseObjs._acadDoc.LockDocument()) {
						idPoly3dRF.reversePolyX();
					}
					pnt3dsPoly3d = idPoly3dRF.getCoordinates3dList();

				}

				try {
					objAlignPL.StationOffset(pnt3dBegREF.X, pnt3dBegREF.Y, ref dblStaBegRef, ref dblOffBegRef);
				} catch (Autodesk.Civil.PointNotOnEntityException ) {
					dblStaBegRef = 0.0;
				}
				objAlignRF.ReferencePointStation = dblStaBegRef;
				Profile objProfile = null;

				using (BaseObjs._acadDoc.LockDocument()) {
					using (Transaction TR = BaseObjs.startTransactionDb()) {

						idAlign = objAlignRF.ObjectId;


						try {
							objProfile = Prof.addProfileByLayout(strName, idAlign, idLayer, idProfileStyle, idStyleLabelSet);

						} catch (Autodesk.AutoCAD.Runtime.Exception exp) {
							Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog(exp.ToString());
						}

						double dblElev = 0;


						for (int i = 0; i <= pnt3dsPoly3d.Count - 1; i++) {
							try {
								objAlignRF.StationOffset(pnt3dsPoly3d[i].X, pnt3dsPoly3d[i].Y, ref dblStation, ref dblOffset);
							} catch (Autodesk.Civil.PointNotOnEntityException ) {
								dblStation = 0.0;
							}
							dblElev = pnt3dsPoly3d[i].Z;

							if (System.Math.Abs(dblStation - objAlignRF.StartingStation) < 1.0) {
								dblStation = Base_Tools45.Math.roundUP2(dblStation);
							}
							if (System.Math.Abs(dblStation - objAlignRF.EndingStation) < 1.0) {
								dblStation = Base_Tools45.Math.roundDown2(dblStation);
							}

							objProfile.PVIs.AddPVI(dblStation, dblElev);
						}

						TR.Commit();

					}
				}

			}
			return idAlign;
		}

		public static int
		getSide(Alignment objAlignPl, Alignment objAlignRF){
			int side = 0;
			double dblEasting = 0, dblNorthing = 0, dblStationPL = 0, dblOffsetPL = 0;
			double midStation = objAlignRF.StartingStation + (objAlignRF.EndingStation - objAlignRF.StartingStation) / 2;

			try {
				objAlignRF.PointLocation(midStation, 0, ref dblEasting, ref dblNorthing);
			}
			catch (Autodesk.AutoCAD.Runtime.Exception ) {
			}

			try {
				objAlignPl.StationOffset(dblEasting, dblNorthing, ref dblStationPL, ref dblOffsetPL);
			}
			catch (Autodesk.Civil.PointNotOnEntityException ) {
				dblStationPL = 0.0;
			}

			if (dblOffsetPL < 0) {
				side = -1;
			}else {
				side = 1;
			}

			return side;
		}
	}
}
