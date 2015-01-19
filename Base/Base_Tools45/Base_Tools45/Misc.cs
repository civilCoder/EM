using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
//using Autodesk.Civil.DatabaseServices;
using Base_Tools45.Jig;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Base_Tools45
{
	/// <summary>
	///
	/// </summary>
	public static class Misc
	{
		private static ObjectId idLast { get; set; }

		public static Point3dCollection
		getBldgLimitsAVG(ObjectId idPoly, int intInterval)
		{
			double dblAngTar = 0; int intMark = 0;
		
			Point3d pnt3dBase0;

			Point3dCollection pnts3dPoly = idPoly.getCoordinates3d();
			Point3dCollection pnts3dRot;
			using (BaseObjs._acadDoc.LockDocument())
			{
				idPoly.getEastWestBaseLineDir(ref dblAngTar, ref intMark);
				pnt3dBase0 = pnts3dPoly[intMark];
				pnts3dRot = UCsys.TranslateCoordinates(pnts3dPoly, pnt3dBase0, dblAngTar * -1);
			}

			List<double> deltas = UCsys.getObjectExtents(pnts3dRot);

			double dblPntXmin = deltas[0];
			double dblPntYmin = deltas[1];

			int iMax = 1 + (int)System.Math.Truncate(deltas[2] / intInterval);
			int jMax = 1 + (int)System.Math.Truncate(deltas[3] / intInterval);

			Point3d pnt3dBase = default(Point3d);
			Point3d pnt3dPolar = default(Point3d);

			double dblDY = intInterval;

			Point3dCollection pnts3dRotGrid = new Point3dCollection();
			Point3dCollection pnts3dGrid = new Point3dCollection();

			for (int j = 0; j <= jMax; j++)
			{
				double dblX = dblPntXmin - (iMax * intInterval - deltas[2]) / 2;
				double dblY = dblPntYmin + (j * dblDY) - (jMax * intInterval - deltas[3]) / 2;

				pnt3dBase = new Point3d(dblX, dblY, 0.0);

				for (int i = 0; i <= iMax; i++)
				{
					pnt3dPolar = Base_Tools45.Math.traverse(pnt3dBase, 0.0, i * intInterval);
					pnts3dRotGrid.Add(pnt3dPolar);
				}
			}

			pnts3dGrid = UCsys.TranslateCoordinates(pnts3dRotGrid, pnt3dBase0, dblAngTar);

			return pnts3dGrid;
		}

		public static object
		getBldgLimitsEW(ObjectId idPoly){
			Object obj = null;
			double dblAngTar = 0; int intMark = 0;

			Point3d pnt3dBase0;

			Point3dCollection pnts3dPoly = idPoly.getCoordinates3d();
			Point3dCollection pnts3dRot;
			using (BaseObjs._acadDoc.LockDocument())
			{
				idPoly.getEastWestBaseLineDir(ref dblAngTar, ref intMark);
				pnt3dBase0 = pnts3dPoly[intMark];
				pnts3dRot = UCsys.TranslateCoordinates(pnts3dPoly, pnt3dBase0, dblAngTar * -1);
			}
			return obj;
		}

		/// <summary>
		///
		/// </summary>
		public static string
		ColorIndexToStr(int color, bool convertStandard)
		{
			string str;

			color = System.Math.Abs(color);    // in case used from Layer table DXF

			Debug.Assert((color >= 0) && (color <= 256));

			if (color == 0)
				str = "ByBlock";
			else if (color == 256)
				str = "ByLayer";
			else if (convertStandard)
			{
				if (color == 1)
					str = "1-Red";
				else if (color == 2)
					str = "2-Yellow";
				else if (color == 3)
					str = "3-Green";
				else if (color == 4)
					str = "4-Cyan";
				else if (color == 5)
					str = "5-Blue";
				else if (color == 6)
					str = "6-Magenta";
				else if (color == 7)
					str = "7-White";
				else
					str = color.ToString();
			}
			else
				str = color.ToString();

			return str;
		}

		public static ObjectId
		rebuildPoly(this DBObjectCollection dbObjs){
			List<ObjectId> ids = new List<ObjectId>();
			foreach (DBObject dbObj in dbObjs)
				ids.Add(dbObj.ObjectId);

			return rebuildLWPoly(ids);
		}

		public static ObjectId
		rebuildLWPoly(List<ObjectId> idEnts)
		{
			int intRnd = 2;

			List<Point3d> pntList = new List<Point3d>();
			List<List<Point3d>> pntGrps = new List<List<Point3d>>();

			Polyline poly = null;
			ObjectId idPoly = ObjectId.Null;

			foreach (ObjectId id in idEnts)
			{
				Entity ent = id.getEnt();
				if (ent is Line)
				{
					Line line = (Line)ent;
					pntList = new List<Point3d> { line.StartPoint, line.EndPoint };
					pntGrps.Add(pntList);
				}
				else if (ent is Polyline)
				{
					poly = (Polyline)ent;
					pntList = poly.ObjectId.getCoordinates3dList();
					pntGrps.Add(pntList);
				}
			}

			bool boolHit = false;

			List<Point3d> pntList0 = pntGrps[0];        //control list - first group in master list
			pntGrps.RemoveAt(0);                        //remove from master list
			
			List<Point3d> pntListX;

			do
			{
				bool boolFirstPass = true;
				bool boolClosed = false;

				if (pntGrps.Count == 0)
				{
					break;
				}

				int j = pntList0.Count;
				Point3d p0Beg = pntList0[0];            //beg point of current control list
				Point3d p0End = pntList0[j - 1];        //end point of current control list

				while (boolClosed == false)             //test if done
				{
					if (boolFirstPass == false)         //first pass with control List0
					{
						if (boolHit == false)           //made a hit -> loop again for another hit
						{
							break;
						}
					}

					if (pntGrps.Count == 0)
					{
						return idPoly;                  
					}

					int i = 0;
					while (i < pntGrps.Count)
					{
						boolHit = false;
						pntListX = pntGrps[i];      //test list
						int n = pntListX.Count;

						Point3d pXBeg = pntListX[0];
						Point3d pXEnd = pntListX[n - 1];

						//E-B - check end point of control with begin point of test 
						if (System.Math.Round(p0End.X, intRnd) == System.Math.Round(pXBeg.X, intRnd) &&
							System.Math.Round(p0End.Y, intRnd) == System.Math.Round(pXBeg.Y, intRnd))
						{
							boolHit = true;

							for (int k = 1; k < n; k++) //skip first point - same as control end point
							{
								pntList0.Add(pntListX[k]);
							}
							pntGrps.RemoveAt(i);
							i = 0;
							p0End = pXEnd; //end point of control 

						}//E-E - check end point of control with end point of test 
						else if (System.Math.Round(p0End.X, intRnd) == System.Math.Round(pXEnd.X, intRnd) &&
								 System.Math.Round(p0End.Y, intRnd) == System.Math.Round(pXEnd.Y, intRnd))
						{
							boolHit = true;

							pntListX.Reverse();

							for (int k = 1; k < n; k++) //skip first point - same as control end point
							{
								pntList0.Add(pntListX[k]);
							}
							pntGrps.RemoveAt(i);
							i = 0;
							p0End = pXBeg; //end point of control 

						}//B-B - check begin point of control with begin point of test
						else if (System.Math.Round(p0Beg.X, intRnd) == System.Math.Round(pXBeg.X, intRnd) &&
								 System.Math.Round(p0Beg.Y, intRnd) == System.Math.Round(pXBeg.Y, intRnd))
						{
							boolHit = true;

							for (int k = 1; k < n; k++)
							{
								pntList0.Insert(0, pntListX[k]);    //insert all but first point 
							}
							pntGrps.RemoveAt(i);
							i = 0;
							p0Beg = pXEnd; //end point of control 

						}//B-E - check begin point of control with end point of test 
						else if (System.Math.Round(p0Beg.X, intRnd) == System.Math.Round(pXEnd.X, intRnd) &&
								 System.Math.Round(p0Beg.Y, intRnd) == System.Math.Round(pXEnd.Y, intRnd))
						{
							boolHit = true;

							for (int k = 0; k < n - 1; k++)
							{
								pntList0.Insert(0, pntListX[k]);    //insert all but last point 
							}
							pntGrps.RemoveAt(i);
							i = 0;
							p0Beg = pXBeg; //end point of control 
						}
						else
							i++;

						if (System.Math.Round(pntList0[0].X, intRnd) == System.Math.Round(pntList0[pntList0.Count - 1].X, intRnd) &&
							System.Math.Round(pntList0[0].Y, intRnd) == System.Math.Round(pntList0[pntList0.Count - 1].Y, intRnd))
						{
							idPoly = Draw.addPoly(pntList0);
							return idPoly;
						}

						boolFirstPass = false;
					}
				}
			} while (true);

			return idPoly;
		}


		public static Color
		getColorByBlock(this short numC){
			Color c = new Color();
			c = Color.FromColorIndex(ColorMethod.ByBlock, numC);
			return c;
		}

		public static Color
		getColorByLayer()
		{
			Color c = new Color();
			c = Color.FromColorIndex(ColorMethod.ByLayer, 256);
			return c;
		}

		public static Color
		getColorByColor(this short numC)
		{
			Color c = new Color();
			c = Color.FromColorIndex(ColorMethod.ByColor, numC);
			return c;
		}


		public static double
		getCalloutElevation(this string callout){
			double elev = 0;

			int res;
			bool isNumeric = false;
			int n = callout.Length;
			do
			{
				n--;
				isNumeric = callout.Substring(n, 1).isInteger(out res);
				if (isNumeric){
					elev = double.Parse(callout.Substring(0, n + 1));
					break;                    
				}
			} while (!isNumeric);

			return elev;
		}


		public static Point3d
		addElevation(this Point3d pnt3d, double elev)
		{
			return new Point3d(pnt3d.X, pnt3d.Y, elev);
		}

		public static void
		deleteLine(ObjectId idLine)
		{
			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					Line ln = (Line)tr.GetObject(idLine, OpenMode.ForWrite);
					ln.Erase();
					tr.Commit();
				}// end using
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Misc.cs: line: 361");
			}
		}

		public static void
		deleteObj(ObjectId objID)
		{
			Entity ent = null;
			try
			{
				using (BaseObjs._acadDoc.LockDocument())
				{
					using (Transaction tr = BaseObjs.startTransactionDb())
					{
						if (objID.IsValid && !objID.IsEffectivelyErased && !objID.IsErased)
						{
							try
							{
								DBObject dbObj = (DBObject)tr.GetObject(objID, OpenMode.ForWrite);
								if (dbObj is Entity)
								{
									ent = (Entity)dbObj;
									if (ent != null)
										ent.Erase(true);
								}
								else
								{
									dbObj.Erase();
								}
							}
							catch (System.Exception ex)
							{
				BaseObjs.writeDebug(ex.Message + " Misc.cs: line: 393");
							}
						}
						else
							return;
						tr.Commit();
					}// end using
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Misc.cs: line: 404");
			}
		}

		public static void
		deleteObjs(TypedValue[] TVs, Point3d pnt3dLL, Point3d pnt3dUR)
		{
			ObjectId[] ids;
			SelectionFilter filter = new SelectionFilter(TVs);
			PromptSelectionResult PSR = BaseObjs._editor.SelectCrossingWindow(pnt3dLL, pnt3dUR, filter);
			if (PSR.Status == PromptStatus.OK)
				ids = PSR.Value.GetObjectIds();
			else
				return;

			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					foreach (ObjectId idObj in ids)
					{
						Entity ENT = (Entity)idObj.GetObject(OpenMode.ForWrite, true);
						ENT.Erase();
					}
					tr.Commit();
				}// end using
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Misc.cs: line: 433");
			}
		}

		public static void
		deleteObjs(TypedValue[] TVs)
		{
			SelectionSet ss = Select.buildSSetBase(TVs, true);
			if (ss == null)
				return;

			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					foreach (ObjectId idObj in ss.GetObjectIds())
					{
						Entity ENT = (Entity)idObj.GetObject(OpenMode.ForWrite, true);
						ENT.Erase();
					}
					tr.Commit();
				}// end using
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Misc.cs: line: 458");
			}
		}

		public static void
		deleteObjs(List<ObjectId> ids)
		{
			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					foreach (ObjectId idObj in ids)
					{
						Entity ENT = (Entity)idObj.GetObject(OpenMode.ForWrite, true);
						ENT.Erase();
					}
					tr.Commit();
				}// end using
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Misc.cs: line: 479");
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="ids"></param>
		public static void
		deleteObjs(ObjectId[] ids)
		{
			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					foreach (ObjectId idObj in ids)
					{
						try
						{
							Entity ENT = (Entity)idObj.GetObject(OpenMode.ForWrite);
							ENT.Erase();
						}
						catch (System.Exception ex)
						{
				BaseObjs.writeDebug(ex.Message + " Misc.cs: line: 503");
						}
					}
					tr.Commit();
				}// end using
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Misc.cs: line: 511");
			}
		}
	   
		public static string[]
		getCommandList()
		{
			string[] cmds = null;
			string cmd = (string)Application.GetSystemVariable("CMDNAMES");
			if (cmd.Length > 0)
			{
				int cmdNum = cmd.Split(new char[] {
					'\''
				}).Length;
				cmds = cmd.Split(new char[] {
					'\''
				});
			}
			return cmds;
		}

		public static Boolean
		getCorners(out Point3d corner1, out Point3d corner2)
		{
			Editor ed = BaseObjs._editor;

			PromptPointResult prRes = ed.GetPoint("\nSelect one corner");
			if (prRes.Status != PromptStatus.OK)
			{
				corner1 = prRes.Value;
				corner2 = prRes.Value;
				return false;
			}

			corner1 = prRes.Value;
			PromptCornerOptions prOpts = new PromptCornerOptions(string.Format("\n{0}", "Select another corner"), corner1);
			prOpts.AllowNone = true;
			prOpts.UseDashedLine = true;

			prRes = ed.GetCorner(prOpts);
			if (prRes.Status != PromptStatus.OK)
			{
				corner2 = prRes.Value;
				return false;
			}
			corner2 = prRes.Value;

			return true;
		}

		public static int
		getCurrAnnoScale()
		{
			Database DB = HostApplicationServices.WorkingDatabase;
			return (int)DB.Cannoscale.DrawingUnits;
		}

		public static Entity
		getEntityByObjectID(ObjectId id)
		{
			Entity ent = null;
			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					DBObject dbObj = tr.GetObject(id, OpenMode.ForRead);
					if (dbObj is Entity)
						ent = (Entity)dbObj;
					tr.Commit();
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Misc.cs: line: 584");
			}
			return ent;
		}

		public static DBObject
		getDbObjectByObjectID(ObjectId id)
		{
			DBObject dbObj = null;
			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					dbObj = tr.GetObject(id, OpenMode.ForRead);
					tr.Commit();
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Misc.cs: line: 603");
			}
			return dbObj;
		}

		public static ObjectId
		getObjectIdFromHandle(string strHandle)
		{
			ObjectId ID = ObjectId.Null;
			Database DB = BaseObjs._db;

			// Convert hexadecimal string to 64-bit integer
			long LN = Convert.ToInt64(strHandle, 16);

			// Not create a Handle from the long integer
			Handle HN = new Handle(LN);

			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					// And attempt to get an ObjectId for the Handle
					ID = DB.GetObjectId(false, HN, 0);
					tr.Dispose();
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Misc.cs: line: 631");
			}
			return ID;
		}

		public static Boolean
		getPline3dList(out List<Point3d> pnts3dList, string prompt = "", string prompt2 = "")
		{
			pnts3dList = new List<Point3d>();
			if (prompt == "")
				prompt = "\nSelect Point";

			Editor ed = BaseObjs._editor;
			PromptPointOptions options = new PromptPointOptions(prompt);
			options.AllowNone = true;

			PromptPointResult ptResult = ed.GetPoint(options);

			// Get the initial point
			Point3d basePt = new Point3d();
			if (ptResult.Status == PromptStatus.OK)
			{
				basePt = ptResult.Value;
			}

			// loop till you break out
			while (true)
			{
				pnts3dList.Add(basePt);
				int count = pnts3dList.Count;
				if (count > 1)
				{
					//
					// NOTE: DrawVector wraps acedGrDraw, which provides a jig for free.
					// However here we have to set up our own pline jig
					//
					ed.DrawVector(pnts3dList[count - 2], pnts3dList[count - 1], -1, false);
					options.BasePoint = basePt;
					options.Message = prompt2;
				}

				// drag with the help of a pline jig
				JigPline jig = new JigPline(basePt);
				PromptResult result = ed.Drag(jig);
				if (result.Status != PromptStatus.OK)
				{
					break;
				}
				basePt = jig.EndPoint;
			}

			if (pnts3dList.Count < 2)
			{
				PrintToCmdLine("There must be at least 2 points.");
				return false;
			}
			return true;
		}
		public static ProjectData
		getProjectData(string strJN)
		{
			string dataSource = @"E:\Spinn\ADMIN2014\timedata.accdb";
			ProjectData projData = new ProjectData();
			using (OleDbConnection dbCon = new OleDbConnection(string.Format("Provider=Microsoft.Ace.OLEDB.12.0; Data Source={0}", dataSource)))
			{
				string clientNumber = "";
				string empNumber = "";

				string strAccessSelect = "SELECT * FROM Jobs";
				OleDbCommand command = new OleDbCommand(strAccessSelect, dbCon);
				dbCon.Open();
				OleDbDataReader reader = command.ExecuteReader();
				while (reader.Read())
				{
					int colNum = reader.GetOrdinal("Job Number");
					if (reader.GetValue(colNum).ToString() == strJN)
					{
						projData.Number = strJN;

						colNum = reader.GetOrdinal("Job Name");
						projData.Name = reader.GetValue(colNum).ToString();

						colNum = reader.GetOrdinal("Project Location");
						projData.Location = reader.GetValue(colNum).ToString();

						colNum = reader.GetOrdinal("Client Number");
						clientNumber = reader.GetValue(colNum).ToString();

						colNum = reader.GetOrdinal("Project Coordinator");
						empNumber = reader.GetValue(colNum).ToString();
						break;
					}
				}
				reader.Close();
				dbCon.Close();

				strAccessSelect = "SELECT * FROM Clients";
				command = new OleDbCommand(strAccessSelect, dbCon);
				dbCon.Open();
				reader = command.ExecuteReader();
				while (reader.Read())
				{
					int colNum = reader.GetOrdinal("ID");
					if (reader.GetValue(colNum).ToString() == clientNumber)
					{
						colNum = reader.GetOrdinal("Company");
						projData.Client = reader.GetValue(colNum).ToString();
						break;
					}
				}
				reader.Close();
				dbCon.Close();

				strAccessSelect = "SELECT * FROM Employee";
				command = new OleDbCommand(strAccessSelect, dbCon);
				dbCon.Open();
				reader = command.ExecuteReader();
				while (reader.Read())
				{
					int colNum = reader.GetOrdinal("EmployeeID");
					if (reader.GetValue(colNum).ToString() == empNumber)
					{
						colNum = reader.GetOrdinal("Employee Name");
						projData.Coordinator = reader.GetValue(colNum).ToString();
						break;
					}
				}
				reader.Close();
				dbCon.Close();
			}
			return projData;
		}

		public static string
		getProjNum()
		{
			int intPos = 0;

			string strPath = strPath = BaseObjs._acadDoc.Name;

			for (int i = strPath.Length - 1; i > -1; i--)
			{
				if (strPath.Substring(i, 1) == @"\")
				{
					intPos = i + 1;
					break;
				}
			}
			return strPath.Substring(intPos);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="idPoly"></param>
		/// <returns></returns>
		public static List<SEG_PROP>
		getSegProps(ObjectId idPoly)
		{
			List<SEG_PROP> props = new List<SEG_PROP>();

			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					Polyline poly = (Polyline)tr.GetObject(idPoly, OpenMode.ForRead);
					for (int i = 1; i < poly.NumberOfVertices; i++)
					{
						Point2d pnt2d0 = poly.GetPoint2dAt(i - 1);
						Point2d pnt2d1 = poly.GetPoint2dAt(i - 0);

						SEG_PROP prop = new SEG_PROP();

						prop.LENGTH = Geom.get2dDistance(pnt2d0, pnt2d1);
						prop.DIR_AHEAD = Measure.getAzRadians(pnt2d0, pnt2d1);

						props.Add(prop);
					}

					tr.Commit();
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Misc.cs: line: 815");
			}
			return props;
		}

		public static void
		gotoEnt()
		{
			string result = "";
			bool escape = UserInput.getUserInput("Enter handle", out result);
			if (escape)
				return;
			Entity ent = null;
			try
			{
				Handle h = result.stringToHandle();
				ent = h.getEnt();
				if(ent == null){
					Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Entity not found.");
				}
			}
			catch
			{
				return;
			}

			Extents3d ext3d = (Extents3d)ent.Bounds;

			Point3d pnt3dMin = ext3d.MinPoint;
			Point3d pnt3dMax = ext3d.MaxPoint;

			ViewTableRecord vtr = new ViewTableRecord();

			vtr.CenterPoint = new Point2d((pnt3dMin.X + pnt3dMax.X) / 2, (pnt3dMin.Y + pnt3dMax.Y) / 2);
			vtr.Height = 50;
			vtr.Width = 70;
			BaseObjs._editor.SetCurrentView(vtr);
		}

		public static void
		showHandle()
		{
			bool escape = true;
			Entity ent = Select.selectEntity("Select ent:", out escape);
			if (escape)
				return;
			BaseObjs.write(string.Format("\nHandle: {0}", ent.Handle.ToString()));
		}

		public static void
		logUsage(string strCmdName, object lngUsage = null)
		{
			string strFileName = null;
			string strUserData = null;
			string strUserName = null;

			if ((lngUsage == null))
			{
				lngUsage = -1;
			}

			strUserName = System.Environment.UserName;

			strFileName = string.Format("M:\\John\\UserData\\2015\\{0}.txt", strUserName);

			DateTime today = DateTime.Now;

			strUserData = string.Format("{0}  {1} - {2} - {3}", today.ToShortDateString(), today.ToShortTimeString(), strCmdName, lngUsage);

			try
			{
				using (StreamWriter SW = new StreamWriter(strFileName, true))
				{
					SW.WriteLine(strUserData);
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Misc.cs: line: 893");
			}
		}

		public static void
		printDebug2File(string[] strText)
		{
			string strFileName = null;
			string strPath = null;

			int i = 0;

			strPath = "C:\\Users\\john\\Desktop";
			strFileName = string.Format("{0}\\Debug.txt", strPath);

			try
			{
				using (StreamWriter SW = new StreamWriter(strFileName, true))
				{
					for (i = 0; i < strText.Length; i++)
					{
						SW.WriteLine(strText[i]);
					}
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Misc.cs: line: 920");
			}
		}

		public static void
		PrintToCmdLine(string str)
		{
			Editor ed = BaseObjs._editor;
			ed.WriteMessage(str);
		}

		public static void
		processLayername(string nameFileLayer, ref string nameLayer, ref string nameFile)
		{
			string[] nameFile_Layer = new string[2];
			string[] delim = new string[1];
			delim[0] = "|";
			nameFile_Layer = nameFileLayer.Split(delim, StringSplitOptions.RemoveEmptyEntries);
			if (nameFile_Layer.Length == 1)
			{
				nameLayer = nameFile_Layer[0];
				nameFile = nameFile = string.Empty;
			}
			else
			{
				nameFile = nameFile_Layer[0];
				nameLayer = nameFile_Layer[1];
			}
		}

		public static string
		PtToStr(Point3d pt, DistanceUnitFormat unitType, int prec)
		{
			string x = Converter.DistanceToString(pt.X, unitType, prec);
			string y = Converter.DistanceToString(pt.Y, unitType, prec);
			string z = Converter.DistanceToString(pt.Z, unitType, prec);

			return string.Format("({0}, {1}, {2})", x, y, z);
		}

		public static string
		PtToStr(Point3d pt)
		{
			return PtToStr(pt, DistanceUnitFormat.Current, -1);
		}

		public static string
		PtToStr(Point2d pt, DistanceUnitFormat unitType, int prec)
		{
			string x = Converter.DistanceToString(pt.X, unitType, prec);
			string y = Converter.DistanceToString(pt.Y, unitType, prec);

			return string.Format("({0}, {1})", x, y);
		}

		public static string
		PtToStr(Point2d pt)
		{
			return PtToStr(pt, DistanceUnitFormat.Current, -1);
		}

		public static void
		removeDuplicatePoints(ref List<POI> varpoi)
		{
			short i = 0;

			List<POI> varPOItemp = new List<POI>();
			POI vPOI;

			vPOI = varpoi[0];
			varPOItemp.Add(vPOI);

			for (i = 1; i <= varpoi.Count - 1; i++)
			{
				//stations are different -> keep point
				if (varpoi[i - 1].Station != varpoi[i + 0].Station)
				{
					varPOItemp.Add(varpoi[i]); // stations are equal
				}
				else
				{
					if (varpoi[i - 1].Elevation != varpoi[i - 0].Elevation)
					{
						//different elevation -> keep closest point
						if (varpoi[i - 1].OFFSET < varpoi[i - 0].OFFSET)
						{
							varPOItemp.Add(varpoi[i - 1]);
						}
						else
						{
							varPOItemp.Add(varpoi[i - 0]);
						}
					}
				}
			}

			varpoi.Clear();
			varpoi = varPOItemp;
		}

		public static List<PNT_DATA>
		removeDuplicateStations(List<PNT_DATA> PNT_DATA)
		{
			List<PNT_DATA> tmpPNT_DATA = new List<PNT_DATA>();

			tmpPNT_DATA.Add(PNT_DATA[0]);

			for (int i = 1; i < PNT_DATA.Count; i++)
			{
				//stations are different -> keep point
				if (System.Math.Abs(System.Math.Round(PNT_DATA[i - 1].STA - PNT_DATA[i + 0].STA, 1)) > 0.5)
				{
					tmpPNT_DATA.Add(PNT_DATA[i]);
				}
			}
			return tmpPNT_DATA;
		}

		public static void
		removeDuplicateVertex(Polyline poly)
		{
			Point2d pnt2d0;
			Point2d pnt2dX;

			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					for (int i = 1; i < poly.NumberOfVertices; i++)
					{
						pnt2d0 = poly.GetPoint2dAt(i - 1);
						pnt2dX = poly.GetPoint2dAt(i - 0);

						if (pnt2d0.X == pnt2dX.X && pnt2d0.Y == pnt2dX.Y)
							poly.RemoveVertexAt(i);
					}
					tr.Commit();
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Misc.cs: line: 1061");
			}
		}

		public static string
		resolvePath(string JN)
		{
			int jn = int.Parse(JN.ToString(CultureInfo.InvariantCulture));

			string path = string.Empty;

			if (jn <= 1649)
				path = @"E:\";
			else if (jn <= 1749)
				path = @"F:\";
			else if (jn <= 1849)
				path = @"G:\";
			else if (jn <= 1949)
				path = @"H:\";
			else if (jn <= 2049)
				path = @"J:\";
			else if (jn <= 2149)
				path = @"K:\";
			else if (jn <= 2499)
				path = @"L:\";
			else if (jn <= 2599)
				path = @"N:\";
			else if (jn <= 2699)
				path = @"N:\2600-2699\";
			else if (jn <= 2799)
				path = @"O:\2700-2699\";
			else if (jn <= 2899)
				path = @"O:\2800-2899\";
			else if (jn <= 2999)
				path = @"O:\2900-2999\";
			else if (jn <= 3099)
				path = @"O:\3000-3099\";
			else if (jn <= 3199)
				path = @"O:\3100-3199\";
			else if (jn <= 3299)
				path = @"O:\3200-3299\";
			else if (jn <= 3399)
				path = @"O:\3300-3399\";
			else if (jn <= 3499)
				path = @"O:\3400-3499\";
			else
			{
				Application.ShowAlertDialog("Drawing File location not found");
				path = "";
			}
			return path;
		}

		public static string
		ScaleToStr(Scale2d sc, DistanceUnitFormat unitType, int prec)
		{
			string x = Converter.DistanceToString(sc.X, unitType, prec);
			string y = Converter.DistanceToString(sc.Y, unitType, prec);

			return string.Format("({0}, {1})", x, y);
		}

		public static string
		ScaleToStr(Scale2d sc)
		{
			return ScaleToStr(sc, DistanceUnitFormat.Current, -1);
		}

		public static string
		ScaleToStr(Scale3d sc, DistanceUnitFormat unitType, int prec)
		{
			string x = Converter.DistanceToString(sc.X, unitType, prec);
			string y = Converter.DistanceToString(sc.Y, unitType, prec);
			string z = Converter.DistanceToString(sc.Z, unitType, prec);

			return string.Format("({0}, {1}, {2})", x, y, z);
		}

		public static string
		ScaleToStr(Scale3d sc)
		{
			return ScaleToStr(sc, DistanceUnitFormat.Current, -1);
		}

		public static bool
		selectObject(out ObjectId objId)
		{
			objId = ObjectId.Null;
			Editor ed = BaseObjs._editor;

			PromptStringOptions prOpts = new PromptStringOptions(string.Format("\nHandle of Db Object"));
			prOpts.AllowSpaces = true;

			PromptResult prRes = ed.GetString(prOpts);

			if (prRes.Status != PromptStatus.OK)
				return false;

			Handle handle = Db.stringToHandle(prRes.StringResult);
			objId = Db.handleToObjectId(BaseObjs._db, handle);
			return true;
		}

		public static string
		VecToStr(Vector3d v, DistanceUnitFormat unitType, int prec)
		{
			string x = Converter.DistanceToString(v.X, unitType, prec);
			string y = Converter.DistanceToString(v.Y, unitType, prec);
			string z = Converter.DistanceToString(v.Z, unitType, prec);

			return string.Format("({0}, {1}, {2})", x, y, z);
		}

		/// <summary>
		/// format a vector in AutoCAD terminology and style using the default unit
		/// and precision.
		/// </summary>
		/// <param name="v">value to format</param>
		public static string
		VecToStr(Vector3d v)
		{
			return VecToStr(v, DistanceUnitFormat.Current, -1);
		}

		/// <summary>
		/// format a vector in AutoCAD terminology and style using a specified unit
		/// and precision.
		/// </summary>
		/// <param name="v">value to format</param>
		/// <param name="unitType">display unit to format as</param>
		/// <param name="prec">number of decimal places for display precision</param>
		public static string
		VecToStr(Vector2d v, DistanceUnitFormat unitType, int prec)
		{
			string x = Converter.DistanceToString(v.X, unitType, prec);
			string y = Converter.DistanceToString(v.Y, unitType, prec);

			return string.Format("({0}, {1})", x, y);
		}

		/// <summary>
		/// format a vector in AutoCAD terminology and style using the default unit
		/// and precision.
		/// </summary>
		/// <param name="v">value to format</param>
		public static string
		VecToStr(Vector2d v)
		{
			return VecToStr(v, DistanceUnitFormat.Current, -1);
		}

		public static ObjectId
		drawBounds2(this Entity ent)
		{
			ObjectId id = ObjectId.Null;
			Extents3d ext3d = (Extents3d)ent.Bounds;
			Point3dCollection pnts3d = new Point3dCollection(){
				new Point3d(ext3d.MinPoint.X, ext3d.MinPoint.Y, 0),
				new Point3d(ext3d.MaxPoint.X, ext3d.MinPoint.Y, 0),
				new Point3d(ext3d.MaxPoint.X, ext3d.MaxPoint.Y, 0),
				new Point3d(ext3d.MinPoint.X, ext3d.MaxPoint.Y, 0),
				new Point3d(ext3d.MinPoint.X, ext3d.MinPoint.Y, 0)
			};
			return id;
		}


		public static void
		drawBounds(this Entity ent)
		{
			idLast.changeProp(clr.mag);
			BaseObjs.updateGraphics();

			Extents3d xtnts3d = (Extents3d)ent.Bounds;
			Point3d pnt3dLL = xtnts3d.MinPoint;
			Point3d pnt3dUR = xtnts3d.MaxPoint;

			Point3d pnt3dLR = new Point3d(pnt3dUR.X, pnt3dLL.Y, 0);
			Point3d pnt3dUL = new Point3d(pnt3dLL.X, pnt3dUR.Y, 0);
			List<Point3d> pnts3d = new List<Point3d> { pnt3dLL, pnt3dLR, pnt3dUR, pnt3dUL, pnt3dLL };
			ObjectId idPoly = pnts3d.addPoly3d();
			idPoly.changeProp(clr.yel);
			BaseObjs.updateGraphics();

			idLast = idPoly;
		}

		public static void
		updateXDataOnCOs(){
			ForEach<MText>(updateXData);   
		}


		public static void
		updateXData(MText mTxt){

			ObjectId idMTxt = mTxt.ObjectId;

			ResultBuffer rb = idMTxt.getXData(apps.lnkCO);
			if (rb == null)
				return;
			TypedValue[] tvs = rb.AsArray();
			string cmd = tvs[1].Value.ToString();

			if(cmd == "cmdG" || cmd == "cmdGX"){

				using(var tr = BaseObjs.startTransactionDb()){
					double elevTop, elevBot, deltaZ;
					string topTxt = mTxt.Text;
					topTxt = topTxt.Replace("(", "");
					topTxt = topTxt.Replace(")", "");

					string botTxt = tvs.getObjectId(4).getMTextText();
					botTxt = botTxt.Replace("(", "");
					botTxt = botTxt.Replace(")", "");

					elevTop = getCalloutElevation(topTxt);
					elevBot = getCalloutElevation(botTxt);

					deltaZ = elevTop - elevBot;
					tvs[9] = new TypedValue(1040, deltaZ);                        

					idMTxt.clearXData(apps.lnkCO);
					idMTxt.setXData(tvs, apps.lnkCO);

					tr.Commit();                    
				}
			}
		}

		private static void
		ForEach<T>(Action<T> action) where T : Entity
		{
			try
			{
				using (var tr = BaseObjs.startTransactionDb())
				{
					var blockTable = (BlockTable)tr.GetObject(BaseObjs._db.BlockTableId, OpenMode.ForRead);
					var modelSpace = (BlockTableRecord)tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);
					RXClass theClass = RXObject.GetClass(typeof(T));
					foreach (ObjectId id in modelSpace)
					{
						if (id.ObjectClass.IsDerivedFrom(theClass))
						{
							try
							{
								var ent = (T)tr.GetObject(id, OpenMode.ForRead);
								action(ent);
							}
							catch (System.Exception ex)
							{
				BaseObjs.writeDebug(ex.Message + " Misc.cs: line: 1312");
							}
						}
					}
					tr.Commit();
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Misc.cs: line: 1321");
			}
		}
	}// class Misc
}
