using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Grading.myForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Grading {

	public static class Grading_GetNestedCurbObjects {

		public static bool
		trimGutterEdgeToCurbX(ObjectIdCollection idsPoly3d, ObjectId idPoly3dFL, List<Handle> handles3) {
			bool exists;
			ObjectId idDict = Dict.getNamedDictionary("cmdBVxRefs", out exists);
			ObjectId idXRef = ObjectId.Null;
			string fileName = string.Empty;
			bool hasXRefs = false;

			try {
				if (!exists) {
					ObjectIdCollection ids = xRef.getXRefsContainingTargetLayer("CURB");
					if (ids == null)
						return hasXRefs;
					switch (ids.Count) {
						case 0:
							Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog("No CURB layer found in any of the xRefs ...... exiting");
							hasXRefs = false;
							break;
						case 1:
							fileName = xRef.getXRefFileName(ids[0]);
							Grading_Palette.gPalette.pGrading.BlockXRefName = fileName;                     //if only one xref has curb layer then use it and
							idXRef = ids[0];
							trimEdgeToCurb(idXRef, idsPoly3d, idPoly3dFL, handles3);                                 //store xref name in form for use again
							hasXRefs = true;
							break;
						default:
							hasXRefs = true;
							winXRefs fXRefs = winXRefs.frmXRefs;
							if (fXRefs.idXRef == ObjectId.Null) { //initiate user interaction to select which
								List<Grading_xRefID> listXRefs = new List<Grading_xRefID>();
								foreach (ObjectId id in ids) {
									fileName = xRef.getXRefFileName(id);
									listXRefs.Add(new Grading_xRefID(fileName, id));
									fXRefs.lstBox1.Items.Add(fileName);
								}
								fXRefs.listXRefs = listXRefs;
								fXRefs.ShowDialog();
							}
							idXRef = fXRefs.idXRef;
							trimEdgeToCurb(idXRef, idsPoly3d, idPoly3dFL, handles3);
							break;
					}
					if (fileName == string.Empty)
						Dict.deleteDictionary("cmdVBxRefs");
					else
						Dict.addXRec(idDict, "xRefName", new ResultBuffer(new TypedValue(1000, fileName)));
				}else {
					ResultBuffer rb = Dict.getXRec(idDict, "xRefName");
					if (rb != null) {
						TypedValue[] tvs = rb.AsArray();
						fileName = tvs[0].Value.ToString();
						BlockReference br = xRef.getXRefBlockReference(fileName);
						idXRef = br.ObjectId;
						trimEdgeToCurb(idXRef, idsPoly3d, idPoly3dFL, handles3);
						hasXRefs = true;
					}
				}
			}
			catch (System.Exception ex) {
				BaseObjs.writeDebug(ex.Message + " Grading_GetNestedCurbObjects.cs: line: 77");
			}
			return hasXRefs;
		}

		public static bool
		getCurbFromXref(ObjectIdCollection idsPoly3d, ObjectId idPoly3dFL, List<Handle> handles3) {
			bool escape = false;
			string xRefPath = null;
			try {
				xRef.getEntity("Select xref entity - curb: ", out escape, out xRefPath);
				if (escape)
					return false;
				string xRefName = Path.GetFileNameWithoutExtension(xRefPath);
				BlockReference br = xRef.getXRefBlockReference(xRefName);
				escape = trimEdgeToCurb(br.ObjectId, idsPoly3d, idPoly3dFL, handles3);
			}
			catch (System.Exception ex) {
				BaseObjs.writeDebug(ex.Message + " Grading_GetNestedCurbObjects.cs: line: 95");
			}
			return escape;
		}

		public static bool
		trimEdgeToCurb(ObjectId idBlockRef, ObjectIdCollection idsPoly3d, ObjectId idPoly3dRF, List<Handle> handles3) {
			bool ints = false;
			ObjectId idPolyLT = handles3[1].getObjectId();
			ObjectId idPolyRT = handles3[3].getObjectId();

			List<Point3d> pnt3dBox = new List<Point3d>{
				idPolyRT.getBegPnt(),
				idPolyRT.getEndPnt(), 
				idPolyLT.getEndPnt(), 
				idPolyLT.getBegPnt(), 
			};
			pnt3dBox.Add(pnt3dBox[0]);

			ObjectId idPolyBox = Draw.addPoly(pnt3dBox);
			Entity entPolyBox = idPolyBox.getEnt();

			string nameLayer = "CURB-TEMP";
			Layer.manageLayers(nameLayer);
			ResultBuffer rb = null;
			try {

				ObjectIdCollection idsEntTemp = xRef.copyXRefEnts(idBlockRef, "CURB", nameLayer);

				BaseObjs.updateGraphics();
				ObjectIdCollection idsEnts = new ObjectIdCollection();

				try {

					for (int i = idsEntTemp.Count - 1; i > -1; i--) {
						Debug.Print(i.ToString());
						ObjectId idEnt = idsEntTemp[i];
						Entity ent = idEnt.getEnt();
						if (ent == null)
							continue;

						Point3dCollection pnts3dInt0 = new Point3dCollection();

						switch (idEnt.getType()) {
							case "Arc":
							case "Line":
								idEnt.changeProp(LineWeight.LineWeight050, clr.mag);
								BaseObjs.updateGraphics();
	
								ent.BoundingBoxIntersectWith(entPolyBox, Intersect.OnBothOperands, pnts3dInt0, IntPtr.Zero, IntPtr.Zero);

								if (pnts3dInt0.Count > 0) {
									idsEnts.Add(idEnt);
									idEnt.changeProp(LineWeight.LineWeight100, clr.cyn);
									BaseObjs.updateGraphics();
								}else{
									idEnt.delete();
								}
								break;
							case "Polyline":

								List<ObjectId> idsPolyEnts = idEnt.poly_ArcsLines(nameLayer);
								idEnt.delete();
								idsEntTemp.Remove(idEnt);

								foreach (ObjectId idx in idsPolyEnts) {
									ent = idx.getEnt();
									idx.changeProp(LineWeight.LineWeight050, clr.mag);
									BaseObjs.updateGraphics();
	
									pnts3dInt0 = new Point3dCollection();
									
									ent.BoundingBoxIntersectWith(entPolyBox, Intersect.OnBothOperands, pnts3dInt0, IntPtr.Zero, IntPtr.Zero);	

									if (pnts3dInt0.Count > 0) {
										idsEnts.Add(idx);
										idsEntTemp.Add(idx);
										idx.changeProp(LineWeight.LineWeight100, clr.cyn);
										BaseObjs.updateGraphics();
									}else{
										idx.delete();
									}

								}
								break;
							case "Spline":
								string message = "\nWarning: Drawing containing the curb objects contains Splines.\nReplace the Splines with Lines, Arcs, or Polylines and reprocess.\nExiting...";
								Application.ShowAlertDialog(message);
								idsEntTemp.delete();
								return false;
	
							default:
								idEnt.delete();
								idsEntTemp.Remove(idEnt);
								break;
						}
					}
				}
				catch (System.Exception) {
				}
				finally{
					idPolyBox.delete();
				}

				if (idsEnts.Count == 0)
					return ints;

				CogoPoint cogoPntBEG;

				Handle[] hPolysIn = new Handle[4];
				handles3.CopyTo(hPolysIn);

				for (int n = 0; n < idsPoly3d.Count; n++) {
					bool delete = false;
					bool exists = false;

					List<Point3d> pnt3dIntL = new List<Point3d>();
					List<Point3d> pnt3dRevL = new List<Point3d>();
					List<Point3d> pnt3dFinalL = new List<Point3d>();

					ObjectId idPoly3dEDGE = idsPoly3d[n];
					Handle h = idPoly3dEDGE.getHandle();

					ObjectId idPolyEDGE = hPolysIn[n * 2 + 1].getObjectId();
					h = idPolyEDGE.getHandle();

					double lenEDGE = idPolyEDGE.getLength();    //2dPoly IS DERIVED FROM REFERENCE 3DPOLYLINE in cmdBV
					double deltaZ = 0;

					Polyline polyEDGE = (Polyline)idPolyEDGE.getEnt();

					ObjectIdCollection idsDelete = new ObjectIdCollection();
					try {
						foreach (ObjectId idEnt in idsEnts) {
							Entity ent = idEnt.getEnt();

							if (ent is Arc) {
								try {
									Arc arc = (Arc)ent;
									idEnt.changeProp(clr.red);
									BaseObjs.updateGraphics();

									Point3d pnt3dPolyBeg = polyEDGE.ObjectId.getBegPnt();
									Point3d pnt3dPolyEnd = polyEDGE.ObjectId.getEndPnt();
									List<Point3d> pnts3dPoly = new List<Point3d> { pnt3dPolyBeg, pnt3dPolyEnd };
									List<Point3d> pnt3dIntLx = new List<Point3d>();
									bool intersects = Geom.intersectsArc(pnts3dPoly, arc, out pnt3dIntLx, ref idsDelete);
									foreach (Point3d p3d in pnt3dIntLx) {
										pnt3dIntL.Add(p3d);
									}
								}
								catch (System.Exception) {
								}
							}else {
								try {
									idEnt.changeProp(clr.yel);
									BaseObjs.updateGraphics();
									Point3dCollection pnt3dIntC = new Point3dCollection();
									polyEDGE.IntersectWith(ent, 0, pnt3dIntC, IntPtr.Zero, IntPtr.Zero);
									foreach (Point3d pnt3d in pnt3dIntC) {
										pnt3dIntL.Add(pnt3d);
									}
								}
								catch (System.Exception) {
								}
							}
						}
						idsDelete.delete();
						BaseObjs.updateGraphics();
					}
					catch (Autodesk.AutoCAD.Runtime.Exception) {
					}
					finally {
					}
					if (pnt3dIntL.Count > 0) {
						delete = true;
						ints = true;
						rb = idPoly3dRF.getXData(apps.lnkBrks2);                    //poly3dEDGE stores FL/RF cogo points at each end
						if (rb == null)
							return false;
						List<Handle> handles2 = rb.rb_handles();                         //list of handles of points at each end of FL/RF
						List<CogoPoint> cogoPnts = handles2.getCogoPntsFromHandlesList();
						cogoPntBEG = cogoPnts[0];

						ObjectId idDictM = Dict.getNamedDictionary(apps.lnkBrks3, out exists);
						ObjectId idDictBEG = Dict.getSubEntry(idDictM, handles2[0].ToString());
						ObjectId idDictEND = Dict.getSubEntry(idDictM, handles2[1].ToString());
						ObjectId idDictX = ObjectId.Null;

						idDictX = Dict.getSubEntry(idDictBEG, idPoly3dEDGE.getHandle().ToString());
						rb = Dict.getXRec(idDictX, "Offset");
						if (rb == null)
							return false;
						TypedValue[] TVs = rb.AsArray();
						double offset = (double)TVs[0].Value;

						rb = Dict.getXRec(idDictX, "DeltaZ");
						TVs = rb.AsArray();
						deltaZ = (double)TVs[0].Value;

						Point3d pnt3dFLbeg = cogoPntBEG.Location;

						Point3d pnt3dEDGEbeg = idPoly3dEDGE.getBegPnt();
						Point3d pnt3dEDGEend = idPoly3dEDGE.getEndPnt();

						double slopeEdge = (pnt3dEDGEend.Z - pnt3dEDGEbeg.Z) / pnt3dEDGEbeg.getDistance(pnt3dEDGEend);

						pnt3dRevL.Add(new Point3d(pnt3dEDGEbeg.X, pnt3dEDGEbeg.Y, 0.0));    //start list with begin point of edge
						pnt3dRevL.Add(pnt3dIntL[0]);    //add first intersection

						double angle = pnt3dEDGEbeg.getDirection(pnt3dFLbeg);
						double distance = pnt3dEDGEbeg.getDistance(pnt3dFLbeg);
						double slope = pnt3dEDGEbeg.getSlope(pnt3dFLbeg);


						distance = distance * 1.1;

						for (int i = 1; i < pnt3dIntL.Count; i++) {
							if (pnt3dIntL[i - 1].isEqual(pnt3dIntL[i - 0], 0.1) == false) {
								pnt3dRevL.Add(pnt3dIntL[i]);
							}
						}

						pnt3dRevL.Add(new Point3d(pnt3dEDGEend.X, pnt3dEDGEend.Y, 0.0));

						List<PNT_LIST> pntList = new List<PNT_LIST>();
						short x = -1;
						foreach (Point3d pnt3d in pnt3dRevL) {
							pntList.Add(new PNT_LIST {
								index = ++x,
								length = pnt3dEDGEbeg.getDistance(pnt3d)
							});
						}

						var sortDist = from pntlst in pntList
									   orderby pntlst.length ascending
									   select pntlst;

						foreach (PNT_LIST pntLst in sortDist) {
							pnt3dFinalL.Add(pnt3dRevL[pntLst.index]);
						}

						for (int i = 1; i < pnt3dFinalL.Count; i++) {
							double distX = pnt3dFinalL[i - 1].getDistance(pnt3dFinalL[i]);
							if (distX == 0)
								pnt3dFinalL.Remove(pnt3dFinalL[i]);
						}


						if (pnt3dFinalL.Count > 1) {
							idsDelete = new ObjectIdCollection();
							for (int i = 1; i < pnt3dFinalL.Count; i++) {
								bool curb = false;
								Point3d pnt3dMID = pnt3dFinalL[i - 1].getMidPoint2d(pnt3dFinalL[i + 0]);
								Point3d pnt3dTAR = pnt3dMID.traverse(angle, distance);
								ObjectId idLine = Base_Tools45.Draw.addLine(pnt3dMID, pnt3dTAR);
								Line line = (Line)idLine.getEnt();
								BaseObjs.updateGraphics();

								Point3dCollection pnts3dInts = new Point3dCollection();
								for (int j = idsEnts.Count - 1; j > -1; j--) {
									ObjectId idEnt = idsEnts[j];
									Entity ent = idEnt.getEnt();
									if (ent is Arc) {
										Arc arc = (Arc)ent;

										List<Point3d> pnts3dLine = new List<Point3d> { pnt3dMID, pnt3dTAR };
										List<Point3d> pnt3dIntLx = new List<Point3d>();
										bool intersects = Geom.intersectsArc(pnts3dLine, arc, out pnt3dIntLx, ref idsDelete);
										foreach (Point3d p3d in pnt3dIntLx) {
											pnts3dInts.Add(p3d);
										}
									}else {
										Point3dCollection pnt3dIntC = new Point3dCollection();
										line.IntersectWith(ent, 0, pnt3dIntC, IntPtr.Zero, IntPtr.Zero);
										foreach (Point3d pnt3d in pnt3dIntC) {
											pnts3dInts.Add(pnt3d);
										}
									}
								}
								if (pnts3dInts.Count > 0) {
									curb = true;
								}

								if (!curb) { //'IF NO CURB THEN BUILD 3DPOLY SEGMENT
									Point3dCollection pnts3d = new Point3dCollection();
									double distBEG = pnt3dEDGEbeg.getDistance(pnt3dFinalL[i - 1]);
									pnts3d.Add(new Point3d(pnt3dFinalL[i - 1].X, pnt3dFinalL[i - 1].Y, pnt3dEDGEbeg.Z + distBEG * slopeEdge));

									double distEND = pnt3dEDGEbeg.getDistance(pnt3dFinalL[i + 0]);
									pnts3d.Add(new Point3d(pnt3dFinalL[i + 0].X, pnt3dFinalL[i + 0].Y, pnt3dEDGEbeg.Z + distEND * slopeEdge));

									ObjectId idPoly = Base_Tools45.Draw.addPoly(pnts3d, "GUTTER");
									handles3.Add(idPoly.getHandle());



									if (distBEG == 0 && System.Math.Round(distEND, 3) == System.Math.Round(lenEDGE)) {
										distEND = -1;
									}
								}
								line.ObjectId.delete();
							}
							if (delete) { //'REMOVE  ORGINAL BREAKLINE DATA FROM EXTENSION DICTIONARY
								handles3.Remove(idPolyEDGE.getHandle());

								Misc.deleteObj(idPolyEDGE);
							}
						}
					}else { // no intersection with curb
						idPolyEDGE.changeProp(nameLayer: "GUTTER");
					}
				}// end loop

				idsEntTemp.delete();

				idPoly3dRF.setXData(handles3, apps.lnkBrks3);
			}
			catch (System.Exception ex) {
				BaseObjs.writeDebug(ex.Message + " Grading_GetNestedCurbObjects.cs: line: 414");
			}
			return ints;
		}
	}
}
