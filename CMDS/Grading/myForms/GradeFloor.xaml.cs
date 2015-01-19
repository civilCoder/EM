using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using Base_Tools45.C3D;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Grading.myForms
{
	/// <summary>
	/// Interaction logic for Grading_Floor.xaml
	/// </summary>
	public partial class GradeFloor : UserControl
	{
		public bool boolSetPointAtCenter = false;
		public bool boolSetPoint = false;

		public GradeFloor()
		{
			InitializeComponent();
			Initialize_Form();
		}

		public double AverageElevation { get; set; }
		public Point3d Centroid { get; set; }
		public ObjectId idBldgLim { get; set; }

		public void
		Initialize_Form()
		{
			optSetElevByDiff.IsChecked = false;
			int index = 0;
			List<string> surfaces = Surf.getSurfaces();
			lstBox1.Items.Clear();
			if (surfaces != null)
			{
				for (int i = 0; i < surfaces.Count; i++)
				{
					string surface = surfaces[i];
					if (surface.Length < 15)
					{
						lstBox1.Items.Add(surface);
						if (surface.ToUpper() == "EXIST")
						{
							index = i;
						}
					}
				}
				lstBox1.SelectedIndex = index;
			}

			cmbSlope.Items.Clear();
			cmbSlope.Items.Add(0.000);
			cmbSlope.Items.Add(0.001);
			cmbSlope.Items.Add(0.002);
			cmbSlope.Items.Add(0.003);
			cmbSlope.Items.Add(0.004);
			cmbSlope.Items.Add(0.005);
			cmbSlope.SelectedValue = "0.000";
			cmbSlope.SelectedIndex = 0;
			
		}

		private void cmdSetPoint_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			boolSetPointAtCenter = false;
			bool escape = true;

			Point3d pnt3dTAR = Pub.pnt3dO;
			BaseObjs.acadActivate();
			Object osMode = SnapMode.getOSnap();
			SnapMode.setOSnap((int)osModes.PER);

			double slope = Convert.ToDouble(cmbSlope.SelectedValue);
			if (slope != 0.0)
				pnt3dTAR = Grading_Floor.getPoint("Select Building Edge in Direction of Increasing Slope", Centroid, out escape);

			Grading_Floor.AVG(idBldgLim, Centroid, slope, Convert.ToDouble(txtPadElev.Text), boolSetPointAtCenter, pnt3dTAR);
			idBldgLim.delete();
			SnapMode.setOSnap((int)osMode);
		}

		private void cmdSetPointAtCenter_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Point3d pnt3dTAR = Pub.pnt3dO;
			boolSetPointAtCenter = true;
			Grading_Floor.AVG(idBldgLim, Centroid, Convert.ToDouble(cmbSlope.SelectedValue), Convert.ToDouble(txtPadElev.Text), boolSetPointAtCenter, pnt3dTAR);
		}

		private void cmdSelectBldgLimits_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			BaseObjs.acadActivate();
			Polyline poly = null;
			string nameLayer = "";
			ObjectId idBlkRef = ObjectId.Null;

			using (BaseObjs._acadDoc.LockDocument())
			{
				try
				{
					idBldgLim = Grading_GetNestedObject.getBldgLimit(out nameLayer, out idBlkRef);
					if (idBldgLim == ObjectId.Null)
						return;
				}
				catch (System.Exception ex)
				{
					BaseObjs.writeDebug(string.Format("{0} GradeFloor.xaml.cs: line: 117", ex.Message));
				}

				Entity ent = idBldgLim.getEnt();

				if (ent is PolylineVertex3d)
				{
					PolylineVertex3d v = (PolylineVertex3d)idBldgLim.getEnt();
					ObjectId idPoly3d = v.OwnerId;
					poly = (Polyline)Conv.poly3d_Poly(idPoly3d, "0").getEnt();
				}
				else if (ent is Polyline3d)
				{
					poly = (Polyline)Conv.poly3d_Poly(idBldgLim, "0").getEnt();
					idBldgLim.delete();
					Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog("Selected Object was a 3d Polyline and has been deleted!!");
				}
				else if (ent is Polyline)
					poly = (Polyline)idBldgLim.getEnt();
				else if (ent is Polyline2d)
				{
					Polyline2d poly2d = (Polyline2d)ent;
					ObjectId idPoly = Conv.poly2dToPoly(poly2d);
					poly = (Polyline)idPoly.getEnt();
				}else if(ent is Line){
					SelectionSet ss = Select.buildSSet(new TypedValue[] { new TypedValue(8, nameLayer) });
					if(ss != null && ss.Count > 0){
						
					}else{
						ObjectIdCollection ids = xRef.getXRefEntsByLayer(idBlkRef, nameLayer);
						List<ObjectId> idsLines = new List<ObjectId>();
						foreach (ObjectId id in ids)
							idsLines.Add(id);
						ObjectId idPoly = Misc.rebuildLWPoly(idsLines);
						if(idPoly == ObjectId.Null){
							Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Failed to build Building Inner Boundary from xRef segments.\nUser needs to build polyline.");
							return;
						}else{
							poly = (Polyline)idPoly.getEnt();
						}
					}
				}

				if (poly == null)
					return;

				Centroid = poly.getCentroid();
				idBldgLim = poly.ObjectId;
			}

			try
			{
				AverageElevation = Grading_Floor.getAverageElev(idBldgLim, chkShowPoints.IsChecked.Value, lstBox1.SelectedValue.ToString());
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(string.Format("{0} GradeFloor.xaml.cs: line: 146", ex.Message));
			}

			lblAvePadElev.Content = AverageElevation.ToString("#,###.00");
			double dblPadElev = AverageElevation + Convert.ToDouble(txtElevDiff.Text);
			lblPadElev.Content = dblPadElev.ToString("#,###.00");
			txtPadElev.Text = string.Format("{0:#,###.00}", lblPadElev.Content);
		}

		private void cmbSlope_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cmbSlope.SelectedValue != null)
			{
				cmbSlope.Text = cmbSlope.SelectedValue.ToString();
			}
			//else{
			//    cmbSlope.Text = "0.000";
			//}

			if (optEnterPadElev.IsChecked == false)
			{
				optSetElevByDiff.IsChecked = true;
				if (Convert.ToDouble(cmbSlope.SelectedValue) == 0.0)
				{
					txtElevDiff.Text = (2.5).ToString("#.00");
				}
				else
				{
					txtElevDiff.Text = (2.0).ToString("#.00");
				}
			}

			double dblPadElev = AverageElevation + Convert.ToDouble(txtElevDiff.Text);
			lblPadElev.Content = dblPadElev.ToString("#,###.00");
			txtPadElev.Text = string.Format("{0:#,###.00}", lblPadElev.Content);
		}

		private void optSetElevByDiff_Checked(object sender, System.Windows.RoutedEventArgs e)
		{
			if (optSetElevByDiff.IsChecked == true)
			{
				if (Convert.ToDouble(cmbSlope.SelectedValue) == 0.0)
				{
					txtElevDiff.Text = (2.5).ToString("#.00");
				}
				else
				{
					txtElevDiff.Text = (2.0).ToString("#.00");
				}
			}
		}

		private void txtElevDiff_TextChanged(object sender, TextChangedEventArgs e)
		{
			string elevDiff = txtElevDiff.Text;
			txtElevDiff.Text = string.Format("{0:#,###.00}", elevDiff);
			double padElev = AverageElevation + Convert.ToDouble(elevDiff);
			lblPadElev.Content = padElev.ToString("#.###.00");
			txtPadElev.Text = lblPadElev.Content.ToString();
		}

		private void txtPadElev_TextChanged(object sender, TextChangedEventArgs e)
		{
			string padElev = txtPadElev.Text;
			txtPadElev.Text = string.Format("{0:#,###.00}", padElev);
			lblPadElev.Content = txtPadElev.Text;

			double elevDiff = Convert.ToDouble(padElev) - AverageElevation;
			txtElevDiff.Text = elevDiff.ToString("#,###.00");
		}
	}
}