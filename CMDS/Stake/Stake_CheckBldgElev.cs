using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Math = Base_Tools45.Math;

namespace Stake
{
	public static class Stake_CheckBldgElev
	{
		private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;
		private static Forms.frmGrid fGrid = Forms.Stake_Forms.sForms.fGrid;

		private static double PI = System.Math.PI;

		public static void
		setBldgElevRef(Alignment objAlign)
		{
			TinSurface objSurface = null;
			bool exists = false;
			try
			{
				objSurface = Surf.getTinSurface("CPNT-ON", out exists);
				if (!exists)
				{
					Stake_GetSurfaceCPNT.getSurfaceFromXRef("STAKE", "STAKE");
					objSurface = Surf.getTinSurface("CPNT-ON", out exists);
				}
			}
			catch (System.Exception)
			{
			}
			ObjectId idBldg = objAlign.GetPolyline();
			Point3d pnt3d = idBldg.getCentroid();

			Point3d pnt3dCen = new Point3d(pnt3d.X, pnt3d.Y, objSurface.FindElevationAtXY(pnt3d.X, pnt3d.Y));

			if (pnt3dCen.Z == -99999.99)
			{
				Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Invalid Elevation at Reference Point - Exiting.......");
				return;
			}

			//Set objCircle = addcircle(pnt3dCEN, 3)
			//objCircle.color = acRed
			//objCircle.Update

			double dblAngTar = 0;
			int intMark = 0;
			idBldg.getEastWestBaseLineDir(ref dblAngTar, ref intMark);

			Point3d pnt3dTar = new Point3d(pnt3d.X, pnt3d.Y, objSurface.FindElevationAtXY(pnt3d.X, pnt3d.Y));

			double dblSlope = System.Math.Round((pnt3dTar.Z - pnt3dCen.Z) / 100, 4);

			//Set objCircle = addcircle(pnt3dTAR, 3)
			//objCircle.color = acYellow
			//objCircle.Update

			if (dblSlope == 0)
			{
				dblAngTar = dblAngTar + PI / 2;

				if (dblAngTar > 2 * PI)
				{
					dblAngTar = dblAngTar - 2 * PI;
				}

				pnt3d = pnt3dCen.traverse(dblAngTar, 100);
				pnt3dTar = new Point3d(pnt3d.X, pnt3d.Y, objSurface.FindElevationAtXY(pnt3d.X, pnt3d.Y));

				//   Set objCircle = addcircle(pnt3dTAR, 3)
				//   objCircle.color = acGreen
				//   objCircle.Update

				if (System.Math.Round(dblSlope, 3) == 0)
				{
					pnt3d = pnt3dCen.traverse(dblAngTar - PI / 2, 100);
					//back to original orientation
					pnt3dTar = new Point3d(pnt3d.X, pnt3d.Y, objSurface.FindElevationAtXY(pnt3d.X, pnt3d.Y));

					//     Set objCircle = addcircle(pnt3dTAR, 3)
					//     objCircle.color = acCyan
					//     objCircle.Update
				}
			}
			else if (dblSlope < 0)
			{
				dblAngTar = dblAngTar + PI;

				if (dblAngTar > 2 * PI)
				{
					dblAngTar = dblAngTar - 2 * PI;
				}

				pnt3dTar = new Point3d(pnt3d.X, pnt3d.Y, objSurface.FindElevationAtXY(pnt3d.X, pnt3d.Y));

				//   Set objCircle = addcircle(pnt3dTAR, 3)
				//   objCircle.color = acBlue
				//   objCircle.Update

				dblSlope = -dblSlope;
			}

			BlockReference objBlkRef = null;
			using (Transaction tr = BaseObjs.startTransactionDb())
			{
				try
				{
					List<string> attVals = new List<string> { dblSlope.ToString() };
					objBlkRef = Blocks.addBlockRef("G:\\TOOLBOX\\Slparrow_STAKE.dwg", pnt3dCen, dblAngTar, attVals);
					objBlkRef.Layer = string.Format("STAKE-BLDG-{0}-LABEL", objAlign.Name);
				}
				catch (System.Exception)
				{
				}
				tr.Commit();
			}

			idBldg.delete();

			TypedValue[] tvs = new TypedValue[6]{
				new TypedValue(1001, "BLDG"),
				new TypedValue(1040, pnt3dCen.X),
				new TypedValue(1040, pnt3dCen.Y),
				new TypedValue(1040, pnt3dCen.Z),
				new TypedValue(1040, dblAngTar),
				new TypedValue(1040, dblSlope)
			};

			objAlign.ObjectId.setXData(tvs, "STAKE");
		}

		public static void
		getBldgElev(ObjectId idAlign, ref List<POI> varpoi)
		{
			ResultBuffer rb = idAlign.getXData("BLDG");
			if (rb == null)
				return;
			TypedValue[] tvs = rb.AsArray();

			Point3d pnt3dCen = new Point3d(double.Parse(tvs[1].Value.ToString()),
										   double.Parse(tvs[2].Value.ToString()),
										   double.Parse(tvs[3].Value.ToString()));

			double dblAngTar = double.Parse(tvs[4].Value.ToString());
			double dblSlope = double.Parse(tvs[5].Value.ToString());

			Point3d pnt3dTar = pnt3dCen.traverse(dblAngTar, 100, dblSlope);

			Debug.Print("BEGIN GET BLDG ELEV");
			double dblEasting = 0, dblNorthing = 0;
			Alignment objAlign = (Alignment)idAlign.getEnt();
			for (int i = 0; i < varpoi.Count; i++)
			{
				objAlign.PointLocation(varpoi[i].Station, varpoi[i].OFFSET, ref dblEasting, ref dblNorthing);
				Point3d pnt3dX = new Point3d(dblEasting, dblNorthing, 0);
				double elev = pnt3dTar.Z + Geom.getCosineComponent(pnt3dCen, pnt3dTar, pnt3dX) * (dblSlope * -1);
				pnt3dX = pnt3dX.addElevation(elev);
				POI vpoi = varpoi[i];
				vpoi.Elevation = Math.roundDown2(pnt3dX.Z);
				varpoi[i] = vpoi;

				Debug.Print(varpoi[i].Station + " " + varpoi[i].Elevation + " " + varpoi[i].Desc0);
			}

			Debug.Print("END GET BLDG ELEV");
		}

		public static void
		checkBldgElev(string strAlignName)
		{
			TinSurface objSurface = null;
			bool exists = false;

			try
			{
				ObjectId idLayer = Layer.manageLayers(fStake.GCAL_LAYER);
				BaseObjs.regen();
			}
			catch (System.Exception)
			{
				Stake_GetSurfaceCPNT.getSurfaceFromXRef("STAKE", "STAKE");
				objSurface = Surf.getTinSurface("CPNT-ON", out exists);
			}

			Alignment objAlign = Align.getAlignment(strAlignName);

			string.Format("\"Test Building No. {0} :\n", strAlignName);

			Form.ControlCollection objCntrls = default(Form.ControlCollection);
			Control objCntrl = default(Control);

			if (checkBldgElevs(objAlign.ObjectId))
			{
				objCntrls = (Form.ControlCollection)fGrid.Frame08.Controls;

				objCntrl = objCntrls[strAlignName];
				objCntrl.BackgroundImage = System.Drawing.Image.FromFile("R:\\TSET\\VBA\\CHECK.bmp");

				objCntrls = (Form.ControlCollection)fGrid.Frame10.Controls;

				objCntrl = objCntrls[string.Format("cmd{0}", strAlignName.Substring(5))];
				objCntrl.Enabled = true;
			}
			else
			{
				objCntrls = (Form.ControlCollection)fGrid.Frame08.Controls;

				objCntrl = objCntrls[strAlignName];
				objCntrl.BackgroundImage = System.Drawing.Image.FromFile("R:\\TSET\\VBA\\X.bmp");

				objCntrls = (Form.ControlCollection)fGrid.Frame10.Controls;

				objCntrl = objCntrls[string.Format("cmd{0}", strAlignName.Substring(5))];
				objCntrl.Enabled = false;
			}
		}

		public static bool
		checkBldgElevs(ObjectId idAlign)
		{
			ResultBuffer rb = idAlign.getXData("BLDG");
			if (rb == null)
				return false;

			TypedValue[] tvs = rb.AsArray();

			Point3d pnt3dCen = new Point3d(double.Parse(tvs[1].Value.ToString()),
										   double.Parse(tvs[2].Value.ToString()),
										   double.Parse(tvs[3].Value.ToString()));

			if (pnt3dCen.Z == -99999.99)
			{
				Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Invalid Elevation at Reference Point - Exiting.......");
				return false;
			}

			double dblAngTar = double.Parse(tvs[4].Value.ToString());
			double dblSlope = double.Parse(tvs[5].Value.ToString());

			Point3d pnt3dTar = pnt3dCen.traverse(dblAngTar, 100, dblSlope);

			List<Point3d> pnts3d = new List<Point3d>();
			Point3d pnt3d = Pub.pnt3dO;
			PromptStatus ps;
			do
			{
				try
				{
					pnt3d = UserInput.getPoint("Select point location to verify FF elevation:", out ps, osMode: 8);
				}
				catch (System.Exception)
				{
					break;
				}
				if (ps == PromptStatus.Cancel || ps == PromptStatus.None)
					break;

				pnts3d.Add(pnt3d);
			} while (true);

			double dblElev = 0, dblDiff = 0;
			string strElevs = "";
			if (pnts3d.Count == 0){
				Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("No Points Selected for Testing....exiting.");
				return false;                
			}

			for (int i = 0; i < pnts3d.Count; i++)
			{
				dblElev = pnt3dTar.Z + Geom.getCosineComponent(pnt3dCen, pnt3dTar, pnts3d[i]) * (dblSlope * -1);
				dblDiff = dblDiff + System.Math.Round(pnts3d[i].Z - dblElev, 2);

				strElevs = strElevs + "\n" + "Pnt Elev: " + pnts3d[i].Z + "\n" + "Surface Elev: " + dblElev + "\n";
			}

			if (dblDiff != 0)
			{
				Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Error found  - Surface correction necessary...." + "\n" + strElevs);
				return false;
			}
			else
			{
				Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Surface Check Successful!!!!...." + "\n" + strElevs);
				return true;
			}
		}
	}
}