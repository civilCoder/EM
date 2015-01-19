using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using Stake.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Math = Base_Tools45.Math;

namespace Stake
{
	public static class Stake_Calc
	{
		private static Forms.frmStake fStake = Stake_Forms.sForms.fStake;
		private static Forms.frmGrid fGrid = Forms.Stake_Forms.sForms.fGrid;

		private static double PI = System.Math.PI;

		public static List<POI>
		addDeltaPnts(int i, int n, List<POI> varpoi, int intDelta)
		{
			List<POI> varPOIadd = new List<POI>();
			double sta = 0;
			string strDesc = null;

			int j = 0;

			for (j = 0; j <= intDelta - 2; j++)
			{
				sta = varpoi[i].Station + (j + 1) * (varpoi[n].Station - varpoi[i].Station) / intDelta;
				strDesc = "\\U+0394" + "/" + intDelta;
				POI vPOI = new POI();

				vPOI.Station = Base_Tools45.Math.roundDown3(sta);
				vPOI.Desc0 = strDesc;
				varPOIadd.Add(vPOI);
			}
			return varPOIadd;
		}

		public static List<POI>
		addGradeBreaks(ref int i, int n, ObjectId idAlign, List<POI> varpoi, List<POI> varPOIadd, int intDelta)
		{
			//GBs between BC and EC
			for (int k = i + 1; k <= n - 1; k++)
			{
				varPOIadd.Add(varpoi[k]);
			}

			var sortPOI = from p in varPOIadd
						  orderby p.Station ascending
						  select p;

			List<POI> varPOIsorted = new List<POI>();
			foreach (var p in sortPOI)
				varPOIsorted.Add(p);
			varPOIadd = varPOIsorted;

			List<POI> varPOIadd2 = new List<POI>();
			POI vPOI = varPOIadd[0];
			vPOI.DescX = vPOI.Desc0;

			varPOIadd2.Add(vPOI);

			int j = 0;

			for (int k = 1; k <= varPOIadd.Count; k++)
			{
				if (System.Math.Round(varPOIadd[k - 1].Station, 2) != System.Math.Round(varPOIadd[k + 0].Station, 2))
				{
					j = j + 1;
					vPOI = varPOIadd[k];
					vPOI.DescX = vPOI.Desc0;
					varPOIadd2.Add(vPOI);
				}
				else
				{
					vPOI = varPOIadd[k - 1];
					string descX = string.Format("{0} {1}", varPOIadd[k + 0].Desc0, varPOIadd[k - 1].Desc0);
					vPOI.DescX = descX;
					varPOIadd2.Add(vPOI);
				}
			}

			i = n - 1;
			return varPOIadd2;
		}

		public static void
		doAnglePointIN(ObjectId idAlign, double dblElev, double dblStation, double dblAngDelta, double dblAngDir, string strDesc, string strName, int intSide, double dblOffset)
		{
			double dblEasting = 0;
			double dblNorthing = 0;

			string strOffset = string.Format("{0} x {1}", dblOffset.ToString(), dblOffset.ToString());

			idAlign.getAlignPointLoc(dblStation, 0.0, ref dblEasting, ref dblNorthing);

			Point3d pnt3d = new Point3d(dblEasting, dblNorthing, dblElev);

			Point3d pnt3dX = pnt3d.traverse(dblAngDir + dblAngDelta / 2 * intSide * -1, dblOffset / System.Math.Sin(dblAngDelta / 2));
			pnt3dX = pnt3dX.addElevation(dblElev);

			Debug.Print(dblStation + " doAnglePointIn");
			setOffsetPoint(pnt3dX, strOffset, strName, strDesc, idAlign, dblStation);
		}

		public static void
		doAnglePointOUT(ObjectId idAlign, double dblElev, double dblStation, double dblAngDelta, double dblAngDir, string strDesc, string strName, int intSide, double dblOffset)
		{
			double dblEasting = 0;
			double dblNorthing = 0;

			string strOffset = dblOffset.ToString();

			idAlign.getAlignPointLoc(dblStation, 0.0, ref dblEasting, ref dblNorthing);

			Point3d pnt3d = new Point3d(dblEasting, dblNorthing, dblElev);

			Point3d pnt3dX = pnt3d.traverse(dblAngDir + ((3 * PI / 2 - dblAngDelta) * intSide * -1), dblOffset);
			//point opposite begin point of next segment
			pnt3dX = pnt3dX.addElevation(dblElev);

			Debug.Print(dblStation + " doAnglePointOUT");
			setOffsetPoint(pnt3dX, strOffset, strName, strDesc, idAlign, dblStation);

			pnt3dX = pnt3d.traverse(dblAngDir + (PI / 2 * intSide * -1), dblOffset);
			//point opposite begin point of next segment
			pnt3dX = pnt3dX.addElevation(dblElev);

			Debug.Print(dblStation + " doAnglePointOUT");
			setOffsetPoint(pnt3dX, strOffset, strName, strDesc, idAlign, dblStation);
		}

		public static void
		doAnglePointOUT_RBC(ObjectId idAlign, double dblElev, double dblStation, double dblAngDelta, double dblAngDir, string strDesc, string strName, int intSide, double dblOffset)
		{
			double dblEasting = 0;
			double dblNorthing = 0;

			string strOffset = dblOffset.ToString();

			idAlign.getAlignPointLoc(dblStation, 0.0, ref dblEasting, ref dblNorthing);

			Point3d pnt3d = new Point3d(dblEasting, dblNorthing, dblElev);

			Point3d pnt3dX = pnt3d.traverse(dblAngDir + PI / 2 + ((dblAngDelta / 2) * intSide * -1), dblOffset / System.Math.Sin(dblAngDelta / 2));
			pnt3dX = pnt3dX.addElevation(dblElev);

			setOffsetPoint(pnt3dX, strOffset, strName, strDesc, idAlign, dblStation);
		}

		public static void
		doCurve(ref int i, ObjectId idAlign, Profile objProfile, List<POI> varpoi, int intSide, double dblOffset, string strName)
		{
			List<POI> varPOIadd = new List<POI>();

			double dblAngDelta = 0;
			double dblAngDir = 0;
			double dblAngChord = 0;
			double dblAngTan = 0;
			double dblRadius = 0;
			double dblStation = 0;

			double dblEasting = 0;
			double dblNorthing = 0;

			string strDesc = null;
			string strOffset = null;

			bool boolGB = false;
			bool isRightHand = false;

			dblRadius = varpoi[i].Radius;
			dblAngDelta = 4 * (System.Math.Atan(varpoi[i].Bulge));
			dblAngChord = varpoi[i].AngChord;
			dblAngTan = dblAngChord - dblAngDelta / 2;
			dblAngDir = varpoi[i].AngDir;
			dblStation = varpoi[i].Station;

			strDesc = varpoi[i].DescX;

			isRightHand = varpoi[i].isRightHand;

			int intDelta = int.Parse(fStake.cboDelta.Text);
			strOffset = dblOffset.ToString();

			int n = i;

			if (varpoi[i].Desc0 == "BC")
			{
				do
				{
					n = n + 1;
					if (varpoi[n].Desc0 != "EC" & varpoi[n].Desc0 != "BC")
					{
						boolGB = true;
					}
					else
					{
						break;
					}
				}
				while (true);
			}
			idAlign.getAlignPointLoc(varpoi[i].Station, dblOffset * intSide, ref dblEasting, ref dblNorthing);
			Point3d pnt3d = new Point3d(dblEasting, dblNorthing, objProfile.ElevationAt(varpoi[i].Station));

			switch (varpoi[i].Desc0)
			{
				case "BC":

					setRadiusPoint(idAlign, varpoi[i], dblRadius, strName);

					if (dblRadius <= 5.0)
					{
						setOffsetPoint(pnt3d, strOffset, strName, strDesc, idAlign, varpoi[i].Station);
						break;
						//Radius point and BC set - grade breaks if any are next before EC
					}
					else if (dblRadius <= 10.0)
					{
						intDelta = 2;

						setOffsetPoint(pnt3d, strOffset, strName, strDesc, idAlign, varpoi[i].Station);
						//set BC

						varPOIadd = addDeltaPnts(i, n, varpoi, intDelta);
						//return midpoint
						varPOIadd = addGradeBreaks(ref i, n, idAlign, varpoi, varPOIadd, intDelta);
						//get GBs

						//make sure midpoint is returned if there are no GBs
						for (int j = 0; j <= varPOIadd.Count; j++)
						{
							idAlign.getAlignPointLoc(varPOIadd[j].Station, dblOffset * intSide, ref dblEasting, ref dblNorthing);
							pnt3d = new Point3d(dblEasting, dblNorthing, objProfile.ElevationAt(varpoi[j].Station));

							setOffsetPoint(pnt3d, strOffset, strName, varPOIadd[j].DescX, idAlign, varPOIadd[j].Station);
						}
					}
					else if (dblRadius <= 50.0)
					{
						setOffsetPoint(pnt3d, strOffset, strName, strDesc, idAlign, varpoi[i].Station);
						//set BC

						varPOIadd = addDeltaPnts(i, n, varpoi, intDelta);
						//Delta frmPoints
						varPOIadd = addGradeBreaks(ref i, n, idAlign, varpoi, varPOIadd, intDelta);
						//get GBs

						for (int j = 0; j <= varPOIadd.Count; j++)
						{
							idAlign.getAlignPointLoc(varPOIadd[j].Station, dblOffset * intSide, ref dblEasting, ref dblNorthing);
							pnt3d = new Point3d(dblEasting, dblNorthing, objProfile.ElevationAt(varpoi[j].Station));

							setOffsetPoint(pnt3d, strOffset, strName, varPOIadd[j].DescX, idAlign, varPOIadd[j].Station);
						}
					}
					else
					{
						setOffsetPoint(pnt3d, strOffset, strName, strDesc, idAlign, varpoi[i].Station);
						//set BC

						intDelta = (int)System.Math.Truncate((varpoi[n].Station - varpoi[i].Station) / 12.5);

						if (intDelta == 1)
						{
							intDelta = 2;
						}

						varPOIadd = addDeltaPnts(i, n, varpoi, intDelta);
						//Delta frmPoints
						varPOIadd = addGradeBreaks(ref i, n, idAlign, varpoi, varPOIadd, intDelta);
						//get GBs

						for (int j = 0; j <= varPOIadd.Count; j++)
						{
							idAlign.getAlignPointLoc(varPOIadd[j].Station, dblOffset * intSide, ref dblEasting, ref dblNorthing);
							pnt3d = new Point3d(dblEasting, dblNorthing, objProfile.ElevationAt(varpoi[j].Station));

							setOffsetPoint(pnt3d, strOffset, strName, varPOIadd[j].DescX, idAlign, varPOIadd[j].Station);
						}
					}

					break;

				case "EC":
					idAlign.getAlignPointLoc(dblStation, dblOffset * intSide, ref dblEasting, ref dblNorthing);

					if (dblRadius <= 5.0)
					{
						pnt3d = new Point3d(dblEasting, dblNorthing, objProfile.ElevationAt(varpoi[i].Station));

						setOffsetPoint(pnt3d, strOffset, strName, strDesc, idAlign, varpoi[i].Station);
					}
					else
					{
						pnt3d = new Point3d(dblEasting, dblNorthing, objProfile.ElevationAt(dblStation));

						setOffsetPoint(pnt3d, strOffset, strName, strDesc, idAlign, varpoi[i].Station);
					}
					break;
			}
		}

		public static void
		set5x5(ObjectId idAlign, double dblElev, double dblStation, double dblOffset, int intSide, string strName, string strDesc)
		{
			double dblAngBack = 0;
			double dblAngAhead = 0;

			double dblEasting0 = 0;
			double dblNorthing0 = 0;
			Alignment objAlign = (Alignment)idAlign.getEnt();
			AlignmentEntityCollection alignEnts = objAlign.Entities;
			//Debug.Print objAlign.EndingStation

			AlignmentEntity alignEnt0 = (AlignmentEntity)alignEnts.EntityAtStation(dblStation);
			AlignmentLine alignLine0 = null;
			AlignmentSubEntityLine subEntLine0 = null;

			if (alignEnt0.EntityType == AlignmentEntityType.Line)
			{
				alignLine0 = (AlignmentLine)alignEnts.EntityAtStation(dblStation);
				subEntLine0 = (AlignmentSubEntityLine)alignLine0[0];
			}
			else
			{
				return;
			}

			AlignmentLine alignLineA = null;
			AlignmentLine alignLineB = null;
			AlignmentEntity alignEntX = null;

			Point2d pnt2dBeg = Point2d.Origin;
			Point2d pnt2dEnd = Point2d.Origin;

			//Point at Tangent Start Point
			if (Math.roundDown3(dblStation) == Math.roundDown3(alignLine0.StartStation))
			{
				alignLineA = alignLine0;

				pnt2dBeg = alignLineA.StartPoint;
				pnt2dEnd = alignLineA.EndPoint;
				dblAngAhead = pnt2dBeg.getDirection(pnt2dEnd);

				//First Tangent
				if (alignEnt0.EntityBefore != 0)
				{
					alignEntX = alignEnts[alignEnt0.EntityBefore - 1];

					if (alignEntX.EntityType == AlignmentEntityType.Line)
					{
						alignLineB = (AlignmentLine)alignEntX;
						pnt2dBeg = alignLineB.StartPoint;
						pnt2dEnd = alignLineB.EndPoint;
						dblAngBack = pnt2dBeg.getDirection(pnt2dEnd);
					}
				}
				else
				{
					dblAngBack = dblAngAhead;
					//     Debug.Print dblAngBack
				}
				//Point at Tangent End Point
			}
			else if (Math.roundDown3(dblStation) == Math.roundDown3(alignLine0.EndStation))
			{
				alignLineB = alignLine0;

				pnt2dBeg = alignLineB.StartPoint;
				pnt2dEnd = alignLineB.EndPoint;
				dblAngBack = pnt2dBeg.getDirection(pnt2dEnd);

				//Last Tangent
				if (alignEnt0.EntityAfter != 0)
				{
					alignEntX = alignEnts[alignLine0.EntityAfter - 1];

					if (alignEntX.EntityType == AlignmentEntityType.Line)
					{
						alignLineA = (AlignmentLine)alignEntX;

						pnt2dBeg = alignLineA.StartPoint;
						pnt2dEnd = alignLineA.EndPoint;
						dblAngAhead = pnt2dBeg.getDirection(pnt2dEnd);
					}
				}
				else
				{
					dblAngAhead = dblAngBack;
				}
			}
			else
			{
				pnt2dBeg = alignLine0.StartPoint;
				pnt2dEnd = alignLine0.EndPoint;
				dblAngAhead = pnt2dBeg.getDirection(pnt2dEnd);
			}

			idAlign.getAlignPointLoc(dblStation, 0.0, ref dblEasting0, ref dblNorthing0);
			//get point of intersection

			Point3d pnt3d = new Point3d(dblEasting0, dblNorthing0, 0.0);

			Point3d pnt3dX = pnt3d.traverse(dblAngAhead + (3 * PI / 4 * intSide * -1), dblOffset / System.Math.Sin(PI / 4));
			pnt3dX = pnt3dX.addElevation(dblElev);

			string strOffset = string.Format("{0} x {1}", dblOffset.ToString(), dblOffset.ToString());

			Debug.Print(dblStation + " set5x5");
			setOffsetPoint(pnt3dX, strOffset, strName, strDesc, idAlign, dblStation);

			pnt3dX = pnt3dX.traverse(dblAngAhead + (PI / 4 * intSide * -1), dblOffset / System.Math.Sin(PI / 4));
			pnt3dX = pnt3dX.addElevation(dblElev);

			strOffset = string.Format("{0} x {1}", dblOffset.ToString(), dblOffset.ToString());

			Debug.Print(dblStation + " set5x5");
			setOffsetPoint(pnt3dX, strOffset, strName, strDesc, idAlign, dblStation);
		}

		public static void
		setOffsetPoint(Point3d pnt3dX, string strOffset, string strName, string strDesc, ObjectId idAlign, double dblStation, object varXAlignDesc = null)
		{
			string nameAlign = Align.getAlignName(idAlign);
			Debug.Print(dblStation + " setOffsetPoint - Station In");

			double dblElevCPNT = 0;
			double dblElevSTAKE = 0;
			int intCurbHeight = 0;

			string strLayer = fStake.STAKE_LAYER;
			string descCgPnt = "";

			double offset = double.Parse(strOffset) * fStake.Side;

			TypedValue[] tvs = new TypedValue[4];
			tvs.SetValue(new TypedValue(1001, "STAKE"), 0);
			tvs.SetValue(new TypedValue(1005, idAlign.getHandle()), 1);
			tvs.SetValue(new TypedValue(1004, dblStation), 2);
			tvs.SetValue(new TypedValue(1040, offset), 3);

			pnt3dX = pnt3dX.addElevation(System.Math.Round(pnt3dX.Z, 3));

			//uint pntNum = 0;
			//ObjectId idCgPnt = pnt3dX.setPoint(out pntNum, "SPNT");
			//CogoPoint cgPnt = (CogoPoint)idCgPnt.getEnt();

			POI varPoi = new POI();
			varPoi.Station = dblStation;
			varPoi.Elevation = pnt3dX.Z;
			//varPoi.PntNum = cgPnt .PointNumber.ToString();

			if (varPoi.Elevation == 0)
			{
				if (fStake.ClassObj != "WALL")
				{
					varPoi.Elevation = System.Math.Round(Prof.getProfileElev(idAlign, "CPNT", dblStation), 3);
					varPoi.PntNum = varPoi.PntNum + " RP";
				}
			}

			//fStake.POI_STAKED.Add(varPoi);

			string strFS = "";

			switch (fStake.ClassObj)
			{
				case "CURB":

					strFS = " TC";
					dblElevCPNT = Prof.getProfileElev(idAlign, "FLOWLINE", dblStation);

					dblElevSTAKE = Prof.getProfileElev(idAlign, "STAKE", dblStation);
					intCurbHeight = Convert.ToInt32((Math.roundUP3(dblElevSTAKE - dblElevCPNT) * 12));
					ASCIIEncoding ascii = new ASCIIEncoding();
					strName = string.Format("{0}{1}{2}{3}", intCurbHeight, ascii.GetString((new Byte[] { 34 })), ascii.GetString((new Byte[] { 32 })), strName);

					break;

				case "WTR":

					strFS = " FS";

					break;

				case "ALIGN":

					switch (strDesc)
					{
						case "TG":
							strFS = "";
							break;

						case "HC":
						case "HE":
						case "H0":
							strDesc = "";
							strFS = " INV";
							break;

						default:
							strFS = " INV";
							break;
					}

					break;

				case "FL":

					strFS = " FS";

					break;

				case "BLDG":

					if (fGrid.optPBC.Checked)
					{
						strFS = " FF";
					}
					else if (fGrid.optRBC.Checked)
					{
						strFS = " PAD";
					}

					strLayer = string.Format("{0}-OS-{1}", fGrid.tbxOffsetH.Text, nameAlign);

					Alignment objAlign = (Alignment)idAlign.getEnt();

					double easting = 0, northing = 0;

					idAlign.getAlignPointLoc(dblStation, 0.0, ref easting, ref northing);
					Point3d pnt3d0 = new Point3d(easting, northing, 0);

					ObjectId idLine = Draw.addLine(pnt3d0, pnt3dX, strLayer, 9);

					break;
			}

			if ((varXAlignDesc == null))
			{
				if (strDesc.Substring(0, 2) == "RP")
				{
					descCgPnt = string.Format("{0} {1}", strName, strDesc);

					if (fStake.optYes.Checked)
					{
						descCgPnt = string.Format("{0} {1} {2}", strName, strDesc, string.Format("{0:###+00.00}", varPoi.Station));
						descCgPnt = descCgPnt.Replace("  ", " ");
					}
				}
				else
				{
					if (fStake.ClassObj == "BLDG")
					{
						descCgPnt = string.Format("{0}'O/S {1} {2}", strOffset, strName, string.Format("{0} {1}", strDesc, strFS).Trim());
						descCgPnt = descCgPnt.Replace("  ", " ");
					}
					else
					{
						descCgPnt = string.Format("{0}'O/S {1}@{2}", strOffset, strName, string.Format("{0} {1}", strDesc, strFS).Trim());
						descCgPnt = descCgPnt.Replace("  ", " ");

						if (fStake.optYes.Checked)
						{
							descCgPnt = string.Format("{0}'O/S {1}@{2} {3}", strOffset, strName, string.Format("{0} {1}", strDesc, strFS).Trim(), string.Format("{0:###+00.00}", varPoi.Station));
							descCgPnt = descCgPnt.Replace("  ", " ");
						}
					}
				}
			}
			else
			{
				descCgPnt = string.Format("{0}'O/S {1}@{2}", strOffset, strName, string.Format("{0} {1}", strDesc, varXAlignDesc).Trim());
				descCgPnt = descCgPnt.Replace("  ", " ");
			}

			uint pntNum = 0;
			ObjectId idCgPnt = pnt3dX.setPoint("SPNT");

			using (Transaction tr = BaseObjs.startTransactionDb())
			{
				CogoPoint CgPnt = (CogoPoint)tr.GetObject(idCgPnt, OpenMode.ForWrite);
				CgPnt.LabelStyleId = Pnt_Style.getPntLabelStyle("SPNT");
				CgPnt.RawDescription = descCgPnt;
				pntNum = fStake.NextPntNum;
				CgPnt.PointNumber = pntNum;
				fStake.NextPntNum++;
				CgPnt.PointName = string.Format("SPNT{0}", pntNum);

				tr.Commit();
			}

			varPoi.PntNum = pntNum.ToString();
			fStake.POI_STAKED.Add(varPoi);

			Debug.Print(varPoi.Station + " setOffsetPoint");

			idCgPnt.setXData(tvs, "STAKE");
		}

		public static void
		setOffsetPointMISC(Point3d dblPntX, string strLayer, string strDesc)
		{
			TypedValue[] tvs = new TypedValue[4] {
				new TypedValue(1001, "STAKE"),
				new TypedValue(1005, "0000"),
				new TypedValue(1070, -1),
				new TypedValue(1000, "MISC")
			};
			uint pntNum = 0;
			double elev = System.Math.Round(dblPntX.Z, 3);
			Point3d pnt3d = new Point3d(dblPntX.X, dblPntX.Y, elev);
			ObjectId idCgPnt = pnt3d.setPoint("SPNT");
			using (Transaction tr = BaseObjs.startTransactionDb())
			{
				CogoPoint CgPnt = (CogoPoint)tr.GetObject(idCgPnt, OpenMode.ForWrite);
				CgPnt.LabelStyleId = Pnt_Style.getPntLabelStyle("SPNT");
				pntNum = fStake.NextPntNum;
				CgPnt.PointName = string.Format("SPNT{0}", pntNum);
				CgPnt.RawDescription = strDesc;
				CgPnt.PointNumber = fStake.NextPntNum;
				fStake.NextPntNum++;

				tr.Commit();
			}

			idCgPnt.setXData(tvs, "STAKE");
		}

		public static void
		setRadiusPoint(ObjectId idAlign, POI varpoi, double dblRadius, string strName)
		{
			Point3d dblPnt = varpoi.CenterPnt;
			string strDescX = string.Format("RP R={0:F2}'", dblRadius);
			string strOffset = string.Format("{0:F2}", dblRadius);
			setOffsetPoint(dblPnt, strOffset, strName, strDescX, idAlign, varpoi.Station);
		}

		public static void
		stakePoints(object varStaBeg = null, object varStaEnd = null, object varSide = null, object varBegNum = null)
		{
			string strClass = fStake.ClassObj;
			Alignment objAlign = null;

			try
			{
				objAlign = fStake.ACTIVEALIGN;
			}
			catch (System.Exception )
			{
				if (Stake_Algn.selectAlignment(ObjectId.Null))
				{
					objAlign = fStake.ACTIVEALIGN;
				}
				else
				{
					return;
				}
			}

			ObjectId idAlign = objAlign.ObjectId;

			fStake.STAKE_LAYER = fStake.cboOffset.Text + "-OS-" + objAlign.Name;

			ObjectId idTable = Stake_Table.getTableId(idAlign);

			List<POI> varpoi = Stake_Table.resetPOI(idTable);

			fStake.POI_CALC = varpoi;

			fStake.objectID = objAlign.ObjectId;
			int intSide = 0;

			double dblStation = 0, dblOffset = 0;
			if ((varSide == null))
			{
				if (double.Parse(fStake.cboOffset.Text) != 0)
				{
					PromptStatus ps;
					Point3d varPntPick = Pub.pnt3dO;
					try
					{
						varPntPick = UserInput.getPoint("\nSelect side to place stake points: <ESC to Cancel>", out ps, osMode: 0);
					}
					catch (System.Exception )
					{
						return;
						;
					}

					idAlign.getAlignStaOffset(varPntPick, ref dblStation, ref dblOffset);
					if (System.Math.Abs(dblOffset) > 10.0)
					{
						string message = string.Format("Point selected is more than 10' from Alignment: {0} Continue", objAlign.Name);
						DialogResult varResponse = MessageBox.Show(message, "Confirm stakng target.", MessageBoxButtons.YesNo);

						if (varResponse == DialogResult.No)
						{
							return;
						}
						else
						{
							if (dblOffset < 0)
							{
								intSide = -1;
							}
							else
							{
								intSide = 1;
							}

							fStake.Side = intSide;
						}
					}
					else
					{
						if (dblOffset < 0)
						{
							intSide = -1;
						}
						else
						{
							intSide = 1;
						}

						fStake.Side = intSide;
					}
				}
				else
				{
					fStake.Side = 1;
				}
			}
			else
			{
				intSide = int.Parse(varSide.ToString());
			}

			dblOffset = double.Parse(fStake.cboOffset.Text);
			string strName = fStake.NameStakeObject;

			var sortSta = from p in varpoi                  //sort stations
						  orderby p.Station ascending
						  select p;

			List<POI> poiTmp = new List<POI>();
			foreach (var p in sortSta)
				poiTmp.Add(p);
			varpoi = poiTmp;

			double dblStaBeg = Math.roundDown3((objAlign.StartingStation));
			double dblStaEnd = Math.roundDown3((objAlign.EndingStation));

			List<POI> varPOI_TOF = null;
			List<POI> varPOI_TOW = null;

			Profile objProfile = null;

			switch (strClass)
			{
				case "WTR":

					objProfile = Prof.getProfile(idAlign, "CPNT");

					break;

				case "WALL":
					for (int i = 0; i < varpoi.Count; i++)
					{
						if (varpoi[i].Desc0 == "TOF")
						{
							varPOI_TOF.Add(varpoi[i]);
						}
						else if (varpoi[i].Desc0 == "TOW")
						{
							varPOI_TOW.Add(varpoi[i]);
						}
					}
					break;

				default:

					objProfile = Prof.getProfile(idAlign, "STAKE");

					break;
			}

			List<POI> varPOI_TMP = new List<POI>();

			if (strClass == "WALL")
			{
				Stake_GetStationing.getStationing(ref varPOI_TOF, dblStaBeg, dblStaEnd);
				Stake_GetStationing.getStationing(ref varPOI_TOW, dblStaBeg, dblStaEnd);
				for (int i = 0; i < varPOI_TOF.Count; i++)
				{
					varPOI_TMP.Add(varPOI_TOF[i]);
				}

				for (int j = 0; j < varPOI_TOW.Count; j++)
				{
					varPOI_TMP.Add(varPOI_TOW[j]);
				}

				varpoi = varPOI_TMP;
			}
			else
			{
				Stake_GetCardinals.checkBegAndEndDesc(idAlign, ref varpoi);
				Stake_GetStationing.getStationing(ref varpoi, dblStaBeg, dblStaEnd);
			}

			poiTmp = new List<POI>();
			foreach (var p in sortSta) //sort stations
				poiTmp.Add(p);
			varpoi = poiTmp;

			uint lngPntNumBeg = Stake_Util.getBegPntNum();

			fStake.POI_STAKED = new List<POI>();
			List<POI> varPOI_STAKE = new List<POI>();

			if (strClass == "BLDG")
			{
				varPOI_STAKE = fStake.POI_STAKE;
			}
			else
			{
				if ((varStaBeg == null))
				{
					varPOI_STAKE = varpoi;
				}
				else
				{
					for (int i = 0; i < varpoi.Count; i++)
					{
						if (varpoi[i].Station >= double.Parse(varStaBeg.ToString()) && varpoi[i].Station <= double.Parse(varStaEnd.ToString()))
						{
							varPOI_STAKE.Add(varpoi[i]);
						}
					}
				}
			}

			for (int i = 0; i < varPOI_STAKE.Count; i++)
			{
				Debug.Print(varPOI_STAKE[i].Station + " " + varPOI_STAKE[i].Desc0);
			}
			POI poi = new POI();
			double dblElev = 0, dblEasting = 0, dblNorthing = 0;
			string strDesc = "", strOffset = "";
			Point3d dblPnt = Pub.pnt3dO;

			for (int i = 0; i < varPOI_STAKE.Count; i++)
			{ //**********PROCESS varPOI_STAKE*********************
				if (varPOI_STAKE[i].DescX == "")
				{
					poi = varPOI_STAKE[i];
					poi.DescX = varPOI_STAKE[i].Desc0;
					varPOI_STAKE[i] = poi;
				}

				switch (varPOI_STAKE[i].ClassObj)
				{
					case "WALL":

						if (varPOI_STAKE[i].Desc0 == "TOW")
						{
							dblElev = varPOI_STAKE[i].ElevTOW;
						}
						else if (varPOI_STAKE[i].Desc0 == "TOF")
						{
							dblElev = varPOI_STAKE[i].ElevTOF;
						}

						switch (varPOI_STAKE[i].Desc0)
						{
							case "AP":

								if (varPOI_STAKE[i].DescX.Contains("END"))
								{
									switch (intSide)
									{
										case 1://right side

											if (varPOI_STAKE[i].isRightHand)
											{ //counterclockwise
												doAnglePointOUT(idAlign, dblElev, varPOI_STAKE[i].Station, varPOI_STAKE[i].AngDelta, varPOI_STAKE[i].AngDir, varPOI_STAKE[i].DescX, strName, intSide, dblOffset);
											}
											else
											{ //clockwise
												doAnglePointIN(idAlign, dblElev, varPOI_STAKE[i].Station, varPOI_STAKE[i].AngDelta, varPOI_STAKE[i].AngDir, varPOI_STAKE[i].DescX, strName, intSide, dblOffset);
											}

											break;

										case -1://left side

											if (varPOI_STAKE[i].isRightHand)
											{ //counterclockwise
												doAnglePointIN(idAlign, dblElev, varPOI_STAKE[i].Station, varPOI_STAKE[i].AngDelta, varPOI_STAKE[i].AngDir, varPOI_STAKE[i].DescX, strName, intSide, dblOffset);
											}
											else
											{ //clockwise
												doAnglePointOUT(idAlign, dblElev, varPOI_STAKE[i].Station, varPOI_STAKE[i].AngDelta, varPOI_STAKE[i].AngDir, varPOI_STAKE[i].DescX, strName, intSide, dblOffset);
											}

											break;
									}
								}

								break;

							case "BC":
							case "EC":

								doCurve(ref i, idAlign, objProfile, varPOI_STAKE, intSide, dblOffset, strName);

								break;

							default:

								strDesc = varPOI_STAKE[i].DescX;
								if (string.IsNullOrEmpty(strDesc))
								{
									strDesc = varPOI_STAKE[i].Desc0;
								}

								try
								{
									idAlign.getAlignPointLoc(varPOI_STAKE[i].Station, dblOffset * intSide, ref dblEasting, ref dblNorthing);
								}
								catch (System.Exception )
								{
									idAlign.getAlignPointLoc(varPOI_STAKE[i].Station - 0.01, dblOffset * intSide, ref dblEasting, ref dblNorthing);
								}

								dblPnt = new Point3d(dblEasting, dblNorthing, dblElev);

								strOffset = dblOffset.ToString();
								setOffsetPoint(dblPnt, strOffset, strName, strDesc, idAlign, varPOI_STAKE[i].Station);

								break;
						}

						break;

					case "WTR":

						dblElev = System.Math.Round(objProfile.ElevationAt(Math.roundDown3(varPOI_STAKE[i].Station)), 2);

						switch (varPOI_STAKE[i].Desc0.Substring(0, 2))
						{
							case "TE":

								if (varPOI_STAKE[i].Side == intSide)
								{
									set5x5(idAlign, dblElev, varPOI_STAKE[i].Station, dblOffset, intSide, strName, varPOI_STAKE[i].DescX);
								}
								else
								{
									idAlign.getAlignPointLoc(varPOI_STAKE[i].Station, dblOffset * intSide, ref dblEasting, ref dblNorthing);

									dblPnt = new Point3d(dblEasting, dblNorthing, dblElev);

									strOffset = dblOffset.ToString();

									setOffsetPoint(dblPnt, strOffset, strName, varPOI_STAKE[i].DescX, idAlign, varPOI_STAKE[i].Station);
								}

								break;

							case "AP":

								switch (intSide)
								{
									case 1:
										//right side

										//counterclockwise
										if (varPOI_STAKE[i].isRightHand)
										{
											doAnglePointOUT(idAlign, dblElev, varPOI_STAKE[i].Station, varPOI_STAKE[i].AngDelta, varPOI_STAKE[i].AngDir, varPOI_STAKE[i].DescX, strName, intSide, dblOffset);
											//clockwise
										}
										else
										{
											doAnglePointIN(idAlign, dblElev, varPOI_STAKE[i].Station, varPOI_STAKE[i].AngDelta, varPOI_STAKE[i].AngDir, varPOI_STAKE[i].DescX, strName, intSide, dblOffset);
										}

										break;

									case -1:
										//left side

										//counterclockwise
										if (varPOI_STAKE[i].isRightHand)
										{
											doAnglePointIN(idAlign, dblElev, varPOI_STAKE[i].Station, varPOI_STAKE[i].AngDelta, varPOI_STAKE[i].AngDir, varPOI_STAKE[i].DescX, strName, intSide, dblOffset);
											//clockwise
										}
										else
										{
											doAnglePointOUT(idAlign, dblElev, varPOI_STAKE[i].Station, varPOI_STAKE[i].AngDelta, varPOI_STAKE[i].AngDir, varPOI_STAKE[i].DescX, strName, intSide, dblOffset);
										}

										break;
								}

								break;

							default:

								idAlign.getAlignPointLoc(varPOI_STAKE[i].Station, dblOffset * intSide, ref dblEasting, ref dblNorthing);
								dblPnt = new Point3d(dblEasting, dblNorthing, dblElev);

								strOffset = dblOffset.ToString();

								double dblCrossingInv = 0;
								string strCrossingInv = "";
								if (varPOI_STAKE[i].CrossAlign != "")
								{
									if (varPOI_STAKE[i].CrossAlignInv != 0)
									{
										dblCrossingInv = System.Math.Round(varPOI_STAKE[i].CrossAlignInv, 2);
										strCrossingInv = string.Format("{0} INV.", dblCrossingInv);
									}
									else
									{
										strCrossingInv = "";
									}

									ASCIIEncoding ascii = new ASCIIEncoding();

									string strAlignDesc = string.Format("{0}{1}%%C PIPE {2}", varPOI_STAKE[i].CrossAlignSize, ascii.GetString(new Byte[] { (Byte)34 }), strCrossingInv);

									setOffsetPoint(dblPnt, strOffset, strName, strAlignDesc, idAlign, varPOI_STAKE[i].Station);
								}
								else
								{
									setOffsetPoint(dblPnt, strOffset, strName, varPOI_STAKE[i].DescX, idAlign, varPOI_STAKE[i].Station);

									if (varPOI_STAKE[i].Type != "")
									{
										setOffsetPoint(dblPnt, strOffset, strName, varPOI_STAKE[i].Type, idAlign, varPOI_STAKE[i].Station, " INV=");
									}
								}

								break;
						}

						break;

					case "CURB":
					case "FL":

						dblElev = System.Math.Round(objProfile.ElevationAt(Math.roundDown3(varPOI_STAKE[i].Station)), 2);

						switch (varPOI_STAKE[i].Desc0)
						{
							case "AP":

								if (varPOI_STAKE[i].DescX.Contains("END"))
								{
									switch (intSide)
									{
										case 1://right side

											if (varPOI_STAKE[i].isRightHand)
											{ //counterclockwise
												doAnglePointOUT(idAlign, dblElev, varPOI_STAKE[i].Station, varPOI_STAKE[i].AngDelta, varPOI_STAKE[i].AngDir, varPOI_STAKE[i].DescX, strName, intSide, dblOffset);
											}
											else
											{ //clockwise
												doAnglePointIN(idAlign, dblElev, varPOI_STAKE[i].Station, varPOI_STAKE[i].AngDelta, varPOI_STAKE[i].AngDir, varPOI_STAKE[i].DescX, strName, intSide, dblOffset);
											}

											break;

										case -1://left side

											if (varPOI_STAKE[i].isRightHand)
											{ //counterclockwise
												doAnglePointIN(idAlign, dblElev, varPOI_STAKE[i].Station, varPOI_STAKE[i].AngDelta, varPOI_STAKE[i].AngDir, varPOI_STAKE[i].DescX, strName, intSide, dblOffset);
											}
											else
											{ //clockwise
												doAnglePointOUT(idAlign, dblElev, varPOI_STAKE[i].Station, varPOI_STAKE[i].AngDelta, varPOI_STAKE[i].AngDir, varPOI_STAKE[i].DescX, strName, intSide, dblOffset);
											}

											break;
									}
								}

								break;

							case "BC":
							case "EC":

								doCurve(ref i, idAlign, objProfile, varPOI_STAKE, intSide, dblOffset, strName);

								break;

							default:

								strDesc = varPOI_STAKE[i].DescX;
								//          If strDesc = "" Then
								//            strDesc = varPOI_Stake[i].Desc0
								//          End If

								try
								{
									idAlign.getAlignPointLoc(varPOI_STAKE[i].Station, dblOffset * intSide, ref dblEasting, ref dblNorthing);
								}
								catch (System.Exception )
								{
									idAlign.getAlignPointLoc(varPOI_STAKE[i].Station - 0.01, dblOffset * intSide, ref dblEasting, ref dblNorthing);
								}
								dblPnt = new Point3d(dblEasting, dblNorthing, dblElev);

								strOffset = dblOffset.ToString();
								setOffsetPoint(dblPnt, strOffset, strName, strDesc, idAlign, varPOI_STAKE[i].Station);

								break;
						}

						break;

					case "ALIGN":
					case "SEWER":

						dblElev = System.Math.Round(objProfile.ElevationAt(Math.roundDown3(varPOI_STAKE[i].Station)), 2);

						if (varPOI_STAKE[i].Desc0 == "BC")
						{
							setRadiusPoint(idAlign, varPOI_STAKE[i], varPOI_STAKE[i].Radius, strName);
						}

						try
						{
							idAlign.getAlignPointLoc(varPOI_STAKE[i].Station, dblOffset * intSide, ref dblEasting, ref dblNorthing);
						}
						catch (System.Exception )
						{
						}

						dblPnt = new Point3d(dblEasting, dblNorthing, dblElev);

						strOffset = dblOffset.ToString();
						setOffsetPoint(dblPnt, strOffset, strName, varPOI_STAKE[i].Desc0, idAlign, varPOI_STAKE[i].Station);

						break;
				}
			}

			List<POI> varPOI_STAKED = fStake.POI_STAKED;
			int intUBnd = varPOI_STAKED.Count - 1;

			uint lngPntNumEnd = uint.Parse(varPOI_STAKED[intUBnd].PntNum);

			TypedValue[] tvs = new TypedValue[4] {
				new TypedValue(1001, "STAKE"),
				new TypedValue(1071, lngPntNumBeg),
				new TypedValue(1071, lngPntNumEnd),
				new TypedValue(1000, fStake.STAKE_LAYER)
			};

			idAlign.setXData(tvs, "STAKE");

			CgPnt_Group.updatePntGroup("SPNT");

			Stake_UpdateProfile.updateProfile(idAlign, (fStake.POI_STAKED), "STAKE", true, "STAKED");

			bool exists = false;
			ObjectId idDict = Dict.getNamedDictionary("STAKE_PNTS", out exists);

			List<Point3d> dblPnts = new List<Point3d>();
			Point3d pnt3d = Pub.pnt3dO;
			uint lngPntNum = 0;

			for (int p = 0; p < varPOI_STAKED.Count; p++)
			{
				string varPntNum = varPOI_STAKED[p].PntNum;

				int intPos = varPntNum.IndexOf(" ");

				if (intPos != 0)
				{
					lngPntNum = uint.Parse(varPntNum.Substring(0, intPos - 1));
				}
				else
				{
					lngPntNum = uint.Parse(varPntNum);
				}

				ObjectId idPnt = BaseObjs._civDoc.CogoPoints.GetPointByPointNumber(uint.Parse(varPOI_STAKED[p].PntNum));
				pnt3d = idPnt.getCogoPntCoordinates();
				dblPnts.Add(pnt3d);
				ResultBuffer rb = new ResultBuffer {
					new TypedValue(1000, idPnt.getCogoPntNumber().ToString()),
					new TypedValue(1005, idPnt.getHandle().ToString())
				};
				Dict.addXRec(idDict, idPnt.ToString(), rb);
			}

			dblPnts.Add(dblPnts[0]);

			Misc.logUsage("STAKE", (lngPntNumEnd - lngPntNumBeg + 1));
		}
	}
}