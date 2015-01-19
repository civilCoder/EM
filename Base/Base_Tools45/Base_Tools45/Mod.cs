using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using DBObject = Autodesk.AutoCAD.DatabaseServices.DBObject;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Base_Tools45
{
	/// <summary>
	///
	/// </summary>
	public static class Mod
	{
		public static ObjectId
		getOwnerOfVertex2d3d(this ObjectId idV3d)
		{
			ObjectId idPoly2d3d = ObjectId.Null;
			using (var tr = BaseObjs.startTransactionDb())
			{
				DBObject obj = idV3d.GetObject(OpenMode.ForRead);
				if (obj is PolylineVertex3d || obj is Vertex2d)
					idPoly2d3d = obj.OwnerId;
			}
			return idPoly2d3d;
		}

		/// <summary>
		///
		/// </summary>
		public const double PI = System.Math.PI;

		public static void
		addVertexToPoly3d(this ObjectId idPoly3dOrg, Point3d pnt3d, int pos, Handle hCgPNt)
		{
			List<Point3d> pnts3d = idPoly3dOrg.getCoordinates3dList();
			ObjectId idPoly3dNew = ObjectId.Null;
			Polyline3d poly3dNew = new Polyline3d();

			using (Transaction tr = BaseObjs.startTransactionDb())
			{
				BlockTableRecord ms = Blocks.getBlockTableRecordMS();
				idPoly3dNew = ms.AppendEntity(poly3dNew);

				poly3dNew.SetDatabaseDefaults();
				poly3dNew.Layer = idPoly3dOrg.getLayer();
				pnts3d.Insert(pos + 1, pnt3d);
				foreach (Point3d pnt3dX in pnts3d)
				{
					PolylineVertex3d v3d = new PolylineVertex3d(pnt3dX);
					poly3dNew.AppendVertex(v3d);
					tr.AddNewlyCreatedDBObject(v3d, true);
				}
				tr.AddNewlyCreatedDBObject(poly3dNew, true);
				tr.Commit();
			}

			using (Transaction tr1 = BaseObjs.startTransactionDb())
			{
				DBObject dbObjOrg = tr1.GetObject(idPoly3dOrg, OpenMode.ForRead);
				DBObject dbObjNew = tr1.GetObject(idPoly3dNew, OpenMode.ForRead);

				dbObjNew.UpgradeOpen();
				dbObjNew.SwapIdWith(dbObjOrg.ObjectId, true, true);

				idPoly3dNew.delete();
				tr1.Commit();
			}

			ResultBuffer rb = idPoly3dOrg.getXData(apps.lnkBrks);
			TypedValue[] tvs = rb.AsArray();

			TypedValue[] tvsNew = new TypedValue[tvs.Length + 1];
			for(int i = 0; i < pos + 2; i++){
				tvsNew[i] = tvs[i];
			}
			tvsNew.SetValue(new TypedValue(1005, hCgPNt), pos + 2);

			for(int i = pos + 2; i < tvs.Length; i++){
				tvsNew[i + 1] = tvs[i];
			}

			idPoly3dOrg.clearXData(apps.lnkBrks);
			idPoly3dOrg.setXData(tvsNew, apps.lnkBrks);

		}

		public static Point3d
		adjLdrEndpoint(ObjectId idLDR, Point3d pnt3dCen, Point3d pnt3d, double scaleFactor)
		{
			Point3d pnt3dEnd = Pub.pnt3dO;
			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					Leader ldr = (Leader)tr.GetObject(idLDR, OpenMode.ForWrite);
					double direction = pnt3dCen.getDirection(pnt3d);
					pnt3dEnd = pnt3dCen.traverse(direction, pnt3dCen.getDistance(pnt3d) * scaleFactor);
					int x = ldr.NumVertices;
					ldr.SetVertexAt(x - 1, pnt3dEnd);
					tr.Commit();
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 96");
			}
			return pnt3dEnd;
		}

		public static void
		adjLdrEndpoint(ObjectId idLDR, Point3d pnt3dEnd)
		{
			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					Leader ldr = (Leader)tr.GetObject(idLDR, OpenMode.ForWrite);
					int x = ldr.NumVertices;
					ldr.SetVertexAt(x - 1, pnt3dEnd);
					tr.Commit();
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 116");
			}
		}

		public static void
		adjMTextXYandRotation(ObjectId idMTxt, Point3d pnt3dEnd, double angle, double width)
		{
			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					MText mTxt = (MText)tr.GetObject(idMTxt, OpenMode.ForWrite);
					mTxt.Location = pnt3dEnd;
					mTxt.Rotation = angle;
					mTxt.Width = width;

					tr.Commit();
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 137");
			}
		}

		public static Point3d
		stringCoordinateListToPoint3d(TypedValue tv)
		{
			string[] coords = tv.Value.ToString().splitFields(' ');
			double x, y, z;
			double.TryParse(coords[0], out x);
			double.TryParse(coords[1], out y);
			double.TryParse(coords[2], out z);
			Point3d pnt3d = new Point3d(x, y, z);
			return pnt3d;
		}

		/// <summary>
		/// copy text contents from source to target text obj
		/// </summary>
		/// <param name="SS0"></param>
		/// <param name="SSX"></param>
		public static void
		copyTextContentsToTextEnt(SelectionSet SS0, SelectionSet SSX)
		{
			string strContents = string.Empty;

			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					ObjectId[] objIDs = null;
					ObjectId objID0 = ObjectId.Null;
					ObjectId objIDX = ObjectId.Null;

					objIDs = SS0.GetObjectIds();
					if (objIDs.Length == 1)
					{
						objID0 = objIDs[0];
						Autodesk.AutoCAD.DatabaseServices.DBObject dbObj0 = tr.GetObject(objID0, OpenMode.ForRead);
						try
						{
							DBText TXT = (DBText)dbObj0;
							if (TXT != null)
							{
								strContents = TXT.TextString.ToString();
							}
							else
							{
								MText mTXT = (MText)dbObj0;
								if (mTXT != null)
									strContents = mTXT.Text.ToString();
							}
						}
						catch (System.Exception ex)
						{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 192");
						}
					}

					objIDs = SSX.GetObjectIds();
					if (objIDs.Length == 1)
					{
						objIDX = objIDs[0];
						Autodesk.AutoCAD.DatabaseServices.DBObject dbObjX = tr.GetObject(objIDX, OpenMode.ForWrite);
						try
						{
							DBText TXT = (DBText)dbObjX;
							if (TXT != null)
							{
								TXT.TextString = strContents;
							}
							else
							{
								MText mTXT = (MText)dbObjX;
								if (mTXT != null)
								{
									mTXT.Contents = strContents;
								}
							}
						}
						catch (System.Exception ex)
						{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 219");
						}
					}
					else if (objIDs.Length > 1)
					{
						foreach (ObjectId objID in objIDs)
						{
							Autodesk.AutoCAD.DatabaseServices.DBObject dbObjX = tr.GetObject(objID, OpenMode.ForWrite);
							try
							{
								DBText TXT = (DBText)dbObjX;
								if (TXT != null)
								{
									TXT.TextString = strContents;
								}
								else
								{
									MText mTXT = (MText)dbObjX;
									if (mTXT != null)
									{
										mTXT.Contents = strContents;
									}
								}
							}
							catch (System.Exception ex)
							{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 245");
							}
						}
					}

					tr.Commit();
				}//end using
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 255");
			}
		}

		/// <summary>
		/// get point in selectionset: get xdata, get blockreference,
		/// update elevation based on datum adjustment
		/// </summary>
		/// <param name="SS"></param>
		/// <param name="dblAdj"></param>
		public static void
		editExistCalloutsByDatumAdj(SelectionSet SS, double dblAdj)
		{
			BlockReference BR = null;

			string strElev = string.Empty;
			double dblElev = 0.0;

			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					ObjectId[] objIDs = SS.GetObjectIds();
					foreach (ObjectId objID in objIDs)
					{
						BR = (BlockReference)tr.GetObject(objID, OpenMode.ForWrite);

						if (BR != null)
						{
							AttributeCollection AC = BR.AttributeCollection;
							foreach (ObjectId arID in AC)
							{
								AttributeReference AR = (AttributeReference)tr.GetObject(arID, OpenMode.ForWrite);
								string strAttVal = AR.TextString.ToString();
								if (strAttVal != string.Empty)
								{
									if (strAttVal.StartsWith("("))
									{
										if (strAttVal.Contains(" ") == true)
										{
											string[] strFields = Txt.splitFields(strAttVal, ' ');
											if (strFields[0] != string.Empty)
											{
												string strVal = strFields[0];

												strElev = strVal.Substring(1, strVal.Length - 1);

												Boolean boolDbl = double.TryParse(strElev, out dblElev);
												if (boolDbl == true)
												{
													dblElev = dblElev + dblAdj;
													strElev = dblElev.ToString();
													strAttVal = string.Format("({0} {1}", strElev, strFields[1]);
													AR.TextString = strAttVal;
													BR.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(255, 200, 200);
												}
											}
										}
										else
										{
											strElev = strAttVal.Substring(1, strAttVal.Length - 2);
											Boolean boolDbl = double.TryParse(strElev, out dblElev);
											if (boolDbl == true)
											{
												dblElev = dblElev + dblAdj;
												strElev = dblElev.ToString();
												strAttVal = string.Format("({0})", strElev);
												AR.TextString = strAttVal;
												BR.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(255, 200, 200);
											}
										}
									}
								}
							}
						}
					}

					tr.Commit();
				}//end using tr
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 337");
			}
		}

		public static ObjectIdCollection
		explode(this ObjectId id)
		{
			ObjectIdCollection ids = new ObjectIdCollection();

			using (Transaction tr = BaseObjs.startTransactionDb())
			{
				Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
				DBObjectCollection dbObjs = new DBObjectCollection();
				ent.Explode(dbObjs);
				ent.Erase();
				BlockTable bt = (BlockTable)tr.GetObject(BaseObjs._db.BlockTableId, OpenMode.ForRead);
				BlockTableRecord ms = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
				foreach (DBObject dbObj in dbObjs)
				{
					Entity entX = (Entity)dbObj;
					ids.Add(ms.AppendEntity(entX));
					tr.AddNewlyCreatedDBObject(entX, true);
				}
				tr.Commit();
			}
			return ids;
		}

		public static ObjectIdCollection
		explode(this ObjectId[] ids)
		{
			ObjectIdCollection idsEx = new ObjectIdCollection();

			using (Transaction tr = BaseObjs.startTransactionDb())
			{
				for (int i = 0; i < ids.Length; i++ )
				{
					try
					{
						Entity ent = (Entity)tr.GetObject(ids[i], OpenMode.ForWrite);
						DBObjectCollection dbObjs = new DBObjectCollection();
						ent.Explode(dbObjs);
						ent.Erase();
						BlockTableRecord ms = (BlockTableRecord)tr.GetObject(BaseObjs._db.CurrentSpaceId, OpenMode.ForWrite);
						foreach (DBObject dbObj in dbObjs)
						{
							Entity entX = (Entity)dbObj;
							idsEx.Add(ms.AppendEntity(entX));
							tr.AddNewlyCreatedDBObject(entX, true);
						}
					}
					catch (System.Exception ex)
					{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 390");
					}
				}

				tr.Commit();
			}
			return idsEx;
		}

		public static void  //NOT COMPLETE
		filletPoly(ObjectId idPoly, int seg, double radius)
		{
			try
			{
				using (BaseObjs._acadDoc.LockDocument())
				{
					using (Transaction tr = BaseObjs.startTransactionDb())
					{
						Polyline pl = (Polyline)tr.GetObject(idPoly, OpenMode.ForWrite);
						tr.Commit();
					}
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 415");
			}
		}

		public static void
		handOverPoly3d(this ObjectId idPoly3dOrg, List<Point3d> pnts3d)
		{
			Polyline3d poly3dOrg = (Polyline3d)idPoly3dOrg.Open(OpenMode.ForRead);

			Point3dCollection pnts3dColl = new Point3dCollection();
			foreach (Point3d pnt3d in pnts3d)
				pnts3dColl.Add(pnt3d);

			Polyline3d poly3dNew = new Polyline3d(Poly3dType.SimplePoly, pnts3dColl, false);

			poly3dNew.SetDatabaseDefaults();
			poly3dOrg.UpgradeOpen();
			poly3dOrg.HandOverTo(poly3dNew, true, true);
			poly3dNew.Close();
			poly3dOrg.Dispose();
		}

		public static void
		handOverPoly3d2(this ObjectId idPoly3dOrg, List<Point3d> pnts3d)
		{
			Database db = BaseObjs._db;
			Point3dCollection pnts3dColl = new Point3dCollection();
			foreach (Point3d pnt3d in pnts3d)
				pnts3dColl.Add(pnt3d);

			using (OpenCloseTransaction tr = db.TransactionManager.StartOpenCloseTransaction())
			{
				Polyline3d poly3dOrg = (Polyline3d)tr.GetObject(idPoly3dOrg, OpenMode.ForRead);
				Debug.Print(poly3dOrg.Handle.ToString());

				Polyline3d poly3dNew = new Polyline3d(Poly3dType.SimplePoly, pnts3dColl, false);
				poly3dNew.SetDatabaseDefaults();
				poly3dNew.Layer = poly3dOrg.Layer;

				poly3dOrg.UpgradeOpen();
				poly3dOrg.HandOverTo(poly3dNew, true, true);
				Debug.Print(poly3dOrg.Handle.ToString());
				Debug.Print(poly3dNew.Handle.ToString());
				bool x = poly3dOrg.IsDisposed;
				poly3dOrg.Dispose();

				tr.AddNewlyCreatedDBObject(poly3dNew, true);
				tr.Commit();
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="pnts3d"></param>
		/// <param name="pnts3dRev"></param>
		/// <returns></returns>
		public static bool
		hasDuplicateVertex(Point3dCollection pnts3d, out Point3dCollection pnts3dRev)
		{
			Point3dCollection pnts3dRevised = new Point3dCollection();
			pnts3dRevised.Add(pnts3d[0]);
			bool hasDup = false;
			for (int i = 0; i < pnts3d.Count - 1; i++)
			{
				if (pnts3d[i + 1].X != pnts3d[i + 0].X && pnts3d[i + 1].Y != pnts3d[i + 0].Y)
				{
					pnts3dRevised.Add(pnts3d[1 + 1]);
				}
				else
				{
					hasDup = true;
				}
			}
			pnts3dRev = pnts3dRevised;
			return hasDup;
		}

		/// <summary>
		/// for translation of leader endpoints when callout is moved
		/// </summary>
		/// <param name="idLDR"></param>
		/// <param name="pnt3dFrom"></param>
		/// <param name="pnt3dTo"></param>
		/// <param name="Pub.pnt3dOrg"></param>
		/// <returns></returns>
		public static Point3d
		moveLdrEndpoint(ObjectId idLDR, Point3d pnt3dFrom, Point3d pnt3dTo, Point3d pnt3dBase)
		{
			Point3d pnt3dNew = Pub.pnt3dO;
			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					Leader ldr = (Leader)tr.GetObject(idLDR, OpenMode.ForWrite);
					Vector3d v3d = new Vector3d(pnt3dTo.X - pnt3dFrom.X, pnt3dTo.Y - pnt3dFrom.Y, 0);
					pnt3dNew = pnt3dBase.TransformBy(Matrix3d.Displacement(v3d));

					try
					{
						int x = ldr.NumVertices;
						ldr.SetVertexAt(x - 1, pnt3dNew);
					}
					catch (System.Exception ex)
					{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 520");
					}
					tr.Commit();
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 527");
			}
			return pnt3dNew;
		}

		public static void
		moveCogoPoint(this ObjectId idCgPnt, Point3d pnt3dTo)
		{
			Point3d pnt3dNew = Pub.pnt3dO;
			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					CogoPoint cgPnt = (CogoPoint)tr.GetObject(idCgPnt, OpenMode.ForWrite);

					Matrix3d matrx3d = BaseObjs._editor.CurrentUserCoordinateSystem;
					CoordinateSystem3d coordSys = matrx3d.CoordinateSystem3d;
					Point3d pnt3dFrom = idCgPnt.getCogoPntCoordinates();
					Vector3d v3d = new Vector3d(pnt3dTo.X - pnt3dFrom.X, pnt3dTo.Y - pnt3dFrom.Y, 0);

					pnt3dNew = pnt3dFrom.TransformBy(Matrix3d.Displacement(v3d));
					cgPnt.Easting = pnt3dNew.X;
					cgPnt.Northing = pnt3dNew.Y;
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 554");
			}
		}

		public static void
		moveSite(this ObjectIdCollection ids, Point3d pnt3dFrom, Point3d pnt3dTo)
		{
			Vector3d v3d = pnt3dFrom.GetVectorTo(pnt3dTo);
			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					foreach (ObjectId id in ids)
					{
						Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
						if (ent is CogoPoint)
						{
							CogoPoint cgPnt = (CogoPoint)ent;
							Point3d cgPntLoc = id.getCogoPntCoordinates();
							Point3d cgPntNew = cgPntLoc.TransformBy(Matrix3d.Displacement(v3d));
							cgPnt.Easting = cgPntNew.X;
							cgPnt.Northing = cgPntNew.Y;
						}
						else
						{
							ent.TransformBy(Matrix3d.Displacement(v3d));
						}
					}
					tr.Commit();
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 587");
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="nodes3d"></param>
		public static void
		removeDuplicateNodes(ref List<Node3d> nodes3d)
		{
			List<Node3d> nodes3dRev = new List<Node3d>();
			nodes3dRev.Add(nodes3d[0]);
			for (int i = 0; i < nodes3d.Count - 1; i++)
			{
				if (nodes3d[i + 1].X != nodes3d[1 + 0].X && nodes3d[1 + 1].Y != nodes3d[1 + 0].Y)
				{
					nodes3dRev.Add(nodes3d[i + 1]);
				}
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="id"></param>
		public static void
		removeDuplicateVertex(ObjectId id)
		{
			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					DBObject dbObj = tr.GetObject(id, OpenMode.ForWrite);
					if (dbObj is Polyline)
					{
						Polyline poly = (Polyline)tr.GetObject(id, OpenMode.ForRead);
						Point3dCollection pnts3d = poly.getCoordinates3d();
						Point3dCollection pnts3dRev;
						if (hasDuplicateVertex(pnts3d, out pnts3dRev))
						{
							string layer = poly.Layer;
							Misc.deleteObj(id);
							Draw.addPoly(pnts3d, layer);
						}
					}
					if (dbObj is Polyline3d)
					{
						Polyline3d poly3d = (Polyline3d)tr.GetObject(id, OpenMode.ForRead);
						Point3dCollection pnts3d = poly3d.getCoordinates3d();
						Point3dCollection pnts3dRev;
						if (hasDuplicateVertex(pnts3d, out pnts3dRev))
						{
							string layer = poly3d.Layer;
							Misc.deleteObj(id);
							Draw.addPoly3d(pnts3d, layer);
						}
					}
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 649");
			}
		}

		public static void
		reverse(this ObjectId idLine)
		{
			if (idLine.getType() == "Line")
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					Line line = (Line)tr.GetObject(idLine, OpenMode.ForWrite);
					Point3d pnt3d = line.StartPoint;
					line.StartPoint = line.EndPoint;
					line.EndPoint = pnt3d;
					tr.Commit();
				}
			}
		}

		public static void
		rotateEnt(this ObjectId id, Point3d pnt3d, double delta)
		{
			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
					Matrix3d curUCSMatrix = BaseObjs._editor.CurrentUserCoordinateSystem;
					CoordinateSystem3d curUCS = curUCSMatrix.CoordinateSystem3d;
					ent.TransformBy(Matrix3d.Rotation(delta, curUCS.Zaxis, pnt3d));
					tr.Commit();
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 685");
			}
		}

		public static void
		scaleObj(ObjectId id, double scaleFactor, Point3d pnt3dRef)
		{
			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					DBObject dbObj = tr.GetObject(id, OpenMode.ForWrite);
					Entity ent = (Entity)dbObj;
					if (ent != null)
					{
						ent.TransformBy(Matrix3d.Scaling(scaleFactor, pnt3dRef));
					}
					tr.Commit();
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 707");
			}
		}

		public static void
		updateBrkLine(this ObjectId idPoly3dOrg, List<Point3d> pnts3d)
		{
			ObjectId idPoly3dNew = ObjectId.Null;
			Polyline3d poly3dNew = new Polyline3d();

			using (Transaction tr = BaseObjs.startTransactionDb())
			{
				BlockTableRecord ms = Blocks.getBlockTableRecordMS();
				idPoly3dNew = ms.AppendEntity(poly3dNew);

				poly3dNew.SetDatabaseDefaults();
				poly3dNew.Layer = idPoly3dOrg.getLayer();

				foreach (Point3d pnt3dX in pnts3d)
				{
					PolylineVertex3d v3d = new PolylineVertex3d(pnt3dX);
					poly3dNew.AppendVertex(v3d);
					tr.AddNewlyCreatedDBObject(v3d, true);
				}
				tr.AddNewlyCreatedDBObject(poly3dNew, true);
				tr.Commit();
			}

			using (Transaction tr1 = BaseObjs.startTransactionDb())
			{
				DBObject dbObjOrg = tr1.GetObject(idPoly3dOrg, OpenMode.ForRead);
				DBObject dbObjNew = tr1.GetObject(idPoly3dNew, OpenMode.ForRead);

				dbObjNew.UpgradeOpen();
				dbObjNew.SwapIdWith(dbObjOrg.ObjectId, true, true);
				//dbObjOrg.HandOverTo(dbObjNew, true, true);
				dbObjOrg.Erase();

				tr1.Commit();
			}
		}

		/// <summary>
		/// not done - check annobase - get point xdata and update linked breaklines
		/// </summary>
		/// <param name="cogoPnt"></param>
		public static void
		updateBrkLine(CogoPoint cogoPnt)
		{
			Database DB = BaseObjs._db;

			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					ResultBuffer RB = cogoPnt.GetXDataForApplication("lblBrks");
					foreach (TypedValue TV in RB)
					{
						if (TV.TypeCode == 1005)
						{
							string strHandle = TV.Value.ToString();
							long LN = Convert.ToInt64(strHandle, 16);
							Handle HN = new Handle(LN);
							ObjectId objID = DB.GetObjectId(false, HN, 0);

							Autodesk.AutoCAD.DatabaseServices.DBObject dbObj = tr.GetObject(objID, OpenMode.ForRead);
							BaseObjs._editor.WriteMessage(dbObj.Handle.ToString());
						}
					}
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 780");
			}
		}//end updateBrkline

		/// <summary>
		/// not done - check annobase - get linked leader deactivate leader then update leader
		/// </summary>
		/// <param name="mTxt"></param>
		/// <param name="strApp"></param>
		public static void
		updateDesignCallout(MText mTxt, string strApp)
		{
			ResultBuffer RB = null;
			Point3dCollection pnt3ds = new Point3dCollection();
			Leader Ldr = null;

			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					RB = mTxt.GetXDataForApplication(strApp);
					foreach (TypedValue TV in RB)
					{
						if (TV.TypeCode.ToString() == "1005")
						{
							string strHandle = TV.Value.ToString();
							ObjectId objID = Misc.getObjectIdFromHandle(strHandle);
							Ldr = (Leader)tr.GetObject(objID, OpenMode.ForRead);
							Events.deactivateLdr("*", Ldr);
							Ldr.UpgradeOpen();

							double dblWidth = mTxt.ActualWidth;

							for (int i = 0; i < Ldr.NumVertices; i++)
							{
								pnt3ds.Add(Ldr.VertexAt(i));
							}

							double dblAngle = Base_Tools45.Measure.getAzRadians(pnt3ds[1], pnt3ds[2]);
							Boolean Left_Justify = Base_Tools45.Math.left_Justify(dblAngle);

							Point3d pnt3d = Math.traverse(pnt3ds[1], dblAngle, dblWidth / 2 + mTxt.ActualHeight * 0.1);

							pnt3d = Math.traverse(pnt3ds[1], dblAngle, dblWidth * 1.20);
						}
					}

					tr.Commit();
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 832");
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="Ldr"></param>
		/// <param name="strApp"></param>
		public static void
		updateDesignCallout(Leader Ldr, string strApp)
		{
			Editor ED = BaseObjs._editor;

			ResultBuffer RB = null;
			Point3dCollection pnt3ds = new Point3dCollection();
			MText mTxt = null;

			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					RB = Ldr.GetXDataForApplication(strApp);
					foreach (TypedValue TV in RB)
					{
						if (TV.TypeCode.ToString() == "1005")
						{
							string strHandle = TV.Value.ToString();
							ObjectId objID = Misc.getObjectIdFromHandle(strHandle);
							mTxt = (MText)tr.GetObject(objID, OpenMode.ForRead);
							Events.deactivateMText("*", mTxt);
							mTxt.UpgradeOpen();

							double dblWidth = mTxt.ActualWidth;

							for (int i = 0; i < Ldr.NumVertices; i++)
							{
								pnt3ds.Add(Ldr.VertexAt(i));
							}

							Point3d pnt3d1 = pnt3ds[1];
							Point3d pnt3d2 = pnt3ds[2];
							Point3d pnt3d3 = mTxt.Location;

							double dblAngle = Base_Tools45.Measure.getAzRadians(pnt3d1, pnt3d2);

							double dblRotate = Geom.getAngle3Points(pnt3d3, pnt3d1, pnt3d2);
							Boolean isRightHand = Base_Tools45.Math.isRightHand(pnt3d3, pnt3d1, pnt3d2);
							if (isRightHand != true)
							{
								dblRotate = -dblRotate;
							}

							Boolean Left_Justify = Base_Tools45.Math.left_Justify(dblAngle);

							Point3d pnt3d = Base_Tools45.Math.traverse(pnt3ds[1], dblAngle, dblWidth / 2 + mTxt.ActualHeight * 0.1);

							if (Left_Justify != true)
							{
								dblRotate = dblRotate + PI;
							}

							Matrix3d curUCSMatrix = ED.CurrentUserCoordinateSystem;
							CoordinateSystem3d curUCS = curUCSMatrix.CoordinateSystem3d;

							mTxt.TransformBy(Matrix3d.Rotation(dblRotate, curUCS.Zaxis, pnt3ds[1]));

							mTxt.Location = mTxt.Location.TransformBy(Matrix3d.Displacement(mTxt.Location.GetVectorTo(pnt3d)));

							Events.activateMText("*", mTxt);
							pnt3d = Base_Tools45.Math.traverse(pnt3ds[1], dblAngle, dblWidth * 1.20);
						}
					}

					tr.Commit();
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 911");
			}
		}

		/// <summary>
		/// get cogo point xdata, get MText, deactivate MText, update MText
		/// activate MText, get MText xdata, get leader, deactivate leader,
		/// update leader, activate leader
		/// </summary>
		/// <param name="cogoPnt"></param>
		/// <param name="strApp"></param>
		public static void
		updateDesignCallout(CogoPoint cogoPnt, string strApp)
		{
			ResultBuffer RB = null;

			MText mText = null;
			string[] strLines;
			string[] strFieldsTop;
			string[] strFieldsBot;
			string strCalloutX = "";

			string strElev = cogoPnt.Location.Z.ToString("#,###.00");

			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					RB = cogoPnt.GetXDataForApplication(strApp);

					string strCallout0 = "";
					foreach (TypedValue TV in RB)
					{
						if (TV.TypeCode.ToString() == "1005")
						{
							string strHandle = TV.Value.ToString();
							ObjectId objID = Misc.getObjectIdFromHandle(strHandle);

							Autodesk.AutoCAD.DatabaseServices.DBObject dbObj = tr.GetObject(objID, OpenMode.ForRead);

							mText = (MText)dbObj;
							strCallout0 = mText.Contents;
							if (strApp == "FL")
							{
								strLines = Txt.splitLines(strCallout0);
								strCalloutX = string.Format("{0}\\P{1}", strElev, strLines[1]);
							}
							if (strApp == "FF")
							{
								strLines = Txt.splitLines(strCallout0);
								strFieldsTop = Txt.splitFields(strLines[0], ' ');
								strFieldsBot = Txt.splitFields(strLines[1], ' ');
							}

							if (strApp == "G")
							{
								strLines = Txt.splitLines(strCallout0);
								strFieldsTop = Txt.splitFields(strLines[0], ' ');
								strFieldsBot = Txt.splitFields(strLines[1], ' ');
							}

							Events.deactivateMText("*", mText);
							mText.UpgradeOpen();
							mText.Contents = strCalloutX;
							mText.DowngradeOpen();
							Events.activateMText("*", mText);
						}
					}

					RB = mText.GetXDataForApplication(strApp);
					foreach (TypedValue TV in RB)
					{
						if (TV.TypeCode.ToString() == "1005")
						{
							string strHandle = TV.Value.ToString();
							ObjectId objID = Misc.getObjectIdFromHandle(strHandle);

							Autodesk.AutoCAD.DatabaseServices.DBObject dbObj = tr.GetObject(objID, OpenMode.ForRead);

							Leader LDR = (Leader)dbObj;
							Events.deactivateLdr("*", LDR);
							LDR.UpgradeOpen();
							LDR.SetVertexAt(0, new Point3d(cogoPnt.Location.X, cogoPnt.Location.Y, 0.0));
							LDR.DowngradeOpen();
							Events.activateLdr("*", LDR);
						}
					}

					tr.Commit();
				}//end using tr
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 1004");
			}
		}//end updateDesignCallout

		/// <summary>
		/// for each point in selectionset: get xdata, get blockreference
		/// update blockreference attribute fields
		/// </summary>
		/// <param name="SS"></param>
		public static void
		updateExistCallouts(SelectionSet SS)
		{
			ResultBuffer RB = null;

			BlockReference BR = null;
			CogoPoint cogoPnt = null;

			string strAppName = string.Empty;
			string strElev = string.Empty;

			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					ObjectId[] objIDs = SS.GetObjectIds();
					foreach (ObjectId objID in objIDs)
					{
						cogoPnt = (CogoPoint)tr.GetObject(objID, OpenMode.ForRead);
						strElev = cogoPnt.Location.Z.ToString("#,###.00");

						RB = cogoPnt.GetXDataForApplication("lblPnts");
						if (RB != null)
						{
							strAppName = "lblPnts";
						}
						else
						{
							RB = cogoPnt.GetXDataForApplication("lblPntsPT");
							strAppName = "lblPntsPT";
						}
						if (RB != null)
						{
							try
							{
								foreach (TypedValue TV in RB)
								{
									if (TV.TypeCode.ToString() == "1005")
									{
										try
										{
											string strHandle = TV.Value.ToString();
											ObjectId brID = Misc.getObjectIdFromHandle(strHandle);

											Autodesk.AutoCAD.DatabaseServices.DBObject dbObj = tr.GetObject(brID, OpenMode.ForRead);

											BR = (BlockReference)dbObj;
											if (BR != null)
											{
												AttributeCollection AC = BR.AttributeCollection;
												foreach (ObjectId arID in AC)
												{
													AttributeReference AR = (AttributeReference)tr.GetObject(arID, OpenMode.ForWrite);
													string strAttVal = AR.TextString.ToString();
													if (strAttVal != string.Empty)
													{
														string[] strFields = Txt.splitFields(strAttVal, ' ');
														if (strFields[0] != string.Empty)
														{
															string strVal = strFields[0];
															if (strVal.StartsWith("("))
															{
																string strChr = strVal[1].ToString();
																int num;
																Boolean isNum = strChr.isInteger(out num);
																if (isNum == true)
																{
																	strAttVal = string.Format("({0} {1}", strElev, strFields[1]);
																	AR.TextString = strAttVal;
																}
															}
														}
													}
												}
											}
										}
										catch (System.Exception ex)
										{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 1091");
										}
									}
								}
							}
							catch (System.Exception ex)
							{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 1098");
							}
						}
					}

					tr.Commit();
				}//end using tr
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Mod.cs: line: 1108");
			}
		}//end updateDesignCallout

		public static void
		updatePoly3dCoordinates(this ObjectId idPoly3dOrg, List<Point3d> pnts3d){
			
			ObjectId idPoly3dNew = ObjectId.Null;
			Polyline3d poly3dNew = new Polyline3d();

			using (Transaction tr = BaseObjs.startTransactionDb())
			{
				BlockTableRecord ms = Blocks.getBlockTableRecordMS();
				idPoly3dNew = ms.AppendEntity(poly3dNew);

				poly3dNew.SetDatabaseDefaults();
				poly3dNew.Layer = idPoly3dOrg.getLayer();
				foreach (Point3d pnt3dX in pnts3d)
				{
					PolylineVertex3d v3d = new PolylineVertex3d(pnt3dX);
					poly3dNew.AppendVertex(v3d);
					tr.AddNewlyCreatedDBObject(v3d, true);
				}
				tr.AddNewlyCreatedDBObject(poly3dNew, true);
				tr.Commit();
			}

			using (Transaction tr1 = BaseObjs.startTransactionDb())
			{
				DBObject dbObjOrg = tr1.GetObject(idPoly3dOrg, OpenMode.ForRead);
				DBObject dbObjNew = tr1.GetObject(idPoly3dNew, OpenMode.ForRead);

				dbObjNew.UpgradeOpen();
				dbObjNew.SwapIdWith(dbObjOrg.ObjectId, true, true);

				idPoly3dNew.delete();
				tr1.Commit();
			}
		}

		public static void
		whip()
		{
			bool escape;
			Entity ent = Select.selectEntity("Select the text: ", out escape);
			if (escape)
				return;

			ResultBuffer rb = null;
			List<string> appList = new List<string>();

			if (ent is DBText)
			{
				DBText txt = (DBText)ent;
				rb = txt.GetXDataForApplication(null);
				if (rb == null)
				{
					Txt.rotateText180(ent.ObjectId);
				}
				else
				{
					TypedValue[] tvs = rb.AsArray();
					List<TypedValue[]> tvsList = tvs.parseXData(out appList);
					if (appList.Contains(apps.lnkGS))
					{

					}
				}

			}
			else if (ent is MText)
			{
				MText mTxt = (MText)ent;
				ObjectId idMTxt = mTxt.ObjectId;

				rb = mTxt.GetXDataForApplication(null);
				if (rb == null)
				{
					Txt.rotateText180(ent.ObjectId);
				}
				else
				{
					TypedValue[] tvsMTxt = rb.AsArray();
					List<TypedValue[]> tvsList = tvsMTxt.parseXData(out appList);

					if (appList.Contains(apps.lnkGS))
					{
						rb = mTxt.GetXDataForApplication(apps.lnkGS);
						TypedValue[] tvs = rb.AsArray();

						ObjectId idPnt1 = tvs[9].Value.ToString().stringToHandle().getObjectId();
						ObjectId idPnt2 = tvs[10].Value.ToString().stringToHandle().getObjectId();

						Point3d pnt3d1, pnt3d2;
						if (idPnt1.IsValid)
							pnt3d1 = idPnt1.getCogoPntCoordinates();
						else
							pnt3d1 = Mod.stringCoordinateListToPoint3d(tvsMTxt[11]);

						if (idPnt2.IsValid)
							pnt3d2 = idPnt2.getCogoPntCoordinates();
						else
							pnt3d2 = Mod.stringCoordinateListToPoint3d(tvsMTxt[11]);

						double station = double.Parse(tvs[5].Value.ToString());
						double offset = double.Parse(tvs[6].Value.ToString());

						Point3d pnt3dBase = pnt3d1.traverse(pnt3d1.getDirection(pnt3d2), station);
						double angle = pnt3dBase.getDirection(mTxt.Location);
						pnt3dBase = pnt3dBase.traverse(angle, offset);

						Mod.rotateEnt(ent.ObjectId, pnt3dBase, PI);
					}
					else
					{
						if (appList.Contains(apps.lnkCO))
						{                           
							rb = mTxt.GetXDataForApplication(apps.lnkCO);
							TypedValue[] tvs = rb.AsArray();
							ObjectId idldr = tvs.getObjectId(3);

							Point3d pnt3dBase = idldr.getEndPnt();
                            idMTxt.flipMTxt(pnt3dBase);
                            ObjectId id = tvs.getObjectId(4);
                            if (id.IsNull)
                                return;

						}
						else if (appList.Contains(apps.lnkLD))
						{


						}
					}
				}
			}
		}

	}
}
