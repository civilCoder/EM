using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using wdp = Wall.Wall_DesignProfile;
using wne = Wall.Wall_NestedEnts;
using wup = Wall.Wall_UpdateProfile;

namespace Wall
{
	public static class Wall_DesignProfileView
	{
		static Wall_Form.frmWall1 fWall1 = Wall_Forms.wForms.fWall1;

		public static ObjectId
		CreateProfileView(ObjectId idAlign){

			Debug.Print("CreateProfileView - Line 28");

			ObjectId idPViewStyle = ObjectId.Null;
			ObjectId idPViewBandSetStyle = ObjectId.Null;
			try {
				idPViewStyle = Prof_Style.getProfileViewStyle("WALL");
				idPViewBandSetStyle = Prof_Style.getProfileViewBandSetStyle("WALL");


			} catch (Autodesk.AutoCAD.Runtime.Exception ) {
				idPViewStyle = Prof_Style.getProfileViewStyle("Standard");
				idPViewBandSetStyle = Prof_Style.getProfileViewBandSetStyle("Standard");

			}

			PromptStatus ps = default(PromptStatus);
			Point3d pnt3dIns = default(Point3d);
			try {
				pnt3dIns = UserInput.getPoint("Select insertion point for Profile View", out ps, osMode: 0);

			} catch (Autodesk.AutoCAD.Runtime.Exception ) {
				idAlign.delete();
				return ObjectId.Null;
			}

			string strLayer = string.Format("{0}-PROFILEVIEW", Align.getAlignName(idAlign));
			Layer.manageLayers(strLayer);

			Debug.Print("CreateProfileView - Line 74");

			ProfileView objProfileView = null;
			using (BaseObjs._acadDoc.LockDocument()) {
				using (Transaction TR = BaseObjs.startTransactionDb()) {

					objProfileView = Prof.addProfileView(idAlign, pnt3dIns, idPViewBandSetStyle, idPViewStyle);
					TR.Commit();
				}
			}

			TypedValue[] tvs = new TypedValue[2]{
				new TypedValue(1001, "WALLDESIGN"),
				new TypedValue(1005, objProfileView.Handle)
			};

			objProfileView.ObjectId.setXData(tvs, "WALLDESIGN");

			Profile objProfile = Prof.getProfile(idAlign, "EXIST");

			Prof_Style.removeProfileLabelGroup(objProfileView.ObjectId, objProfile.ObjectId);

			Debug.Print("CreateProfileView - Line 113");
			
			List<POI> varPOI = new List<POI>();

			BlockTableRecord btrMS = null;
			if (fWall1.opt1_PL.Checked == true) {

				try {
					btrMS = xRef.getXRefMSpace("TOPO");
					if ((btrMS == null)) {
						System.Windows.Forms.MessageBox.Show("TOPO drawing not attached");
						return ObjectId.Null;
					}


				} catch (Autodesk.AutoCAD.Runtime.Exception ) {
					System.Windows.Forms.MessageBox.Show("TOPO drawing not attached");
					return ObjectId.Null;

				}

				if (fWall1.opt1_SurfaceExist.Checked == true) {

					if (wne.getNestedPoints(idAlign, ref varPOI, btrMS, "WALLDESIGN")) {
						dynamic sta = from poi in varPOI
									  orderby poi.Station
									  select poi;
						
						List<POI> p = new List<POI>();
						foreach (var s in sta) 
							p.Add(s);
						
						varPOI = p;
						wup.updateProfile(idAlign, varPOI, "EXIST", true, "WALLDESIGN");

					}

				}

				Debug.Print("CreateProfileView - Line 145");


				if (fWall1.opt1_SurfaceDesign.Checked == true) {

					if (BaseObjs._acadDoc.Name.Contains("GCAL")){

						if (wdp.getDesignPoints(idAlign, ref varPOI, "BOTH")) {
							dynamic sta = from poi in varPOI
										  orderby poi.Station
										  select poi;
							List<POI> t = new List<POI>();
							foreach (var s in sta)
                                t.Add(s);
                            varPOI = t;
							wup.updateProfile(idAlign, varPOI, "CPNT", true, "WALLDESIGN");

						}


					} else {

						try {
							btrMS = xRef.getXRefMSpace("GCAL");
							if ((btrMS == null)) {
								System.Windows.Forms.MessageBox.Show("TOPO drawing not attached");
								return ObjectId.Null;
							}


						} catch (Autodesk.AutoCAD.Runtime.Exception ) {
							System.Windows.Forms.MessageBox.Show("TOPO drawing not attached");
							return ObjectId.Null;

						}


						if (wne.getNestedPoints(idAlign, ref varPOI, btrMS, "WALLDESIGN")) {
							dynamic sta = from poi in varPOI 
										  orderby poi.Station
										  select poi;
							List<POI> t = new List<POI>();
							foreach (var s in sta)
                                t.Add(s);
                            varPOI = t;

							wup.updateProfile(idAlign, varPOI, "CPNT", true, "WALLDESIGN");

						}

					}
				}


			} else if (fWall1.opt1_SITE.Checked == true) {

				if (BaseObjs._acadDoc.Name.Contains("GCAL")) {
                    varPOI = new List<POI>();


				if (wdp.getDesignPoints(idAlign, ref varPOI, "LEFT")) {
					dynamic sta = from poi in varPOI
								  orderby poi.Station
								  select poi;
							List<POI> t = new List<POI>();
							foreach (var s in sta)
                                t.Add(s);
                            varPOI = t;

					wup.updateProfile(idAlign, varPOI, "CPNT", true, "WALLDESIGN");

				}


				if (wdp.getDesignPoints(idAlign, ref varPOI, "RIGHT")) {
					dynamic sta = from poi in varPOI
								  orderby poi.Station
								  select poi;
							List<POI> t = new List<POI>();
							foreach (var s in sta)
                                t.Add(s);
                            varPOI = t;

					wup.updateProfile(idAlign, varPOI, "CPNT", true, "WALLDESIGN");

				}


				} else {
					try {
						btrMS = xRef.getXRefMSpace("TOPO");
						if ((btrMS == null)) {
							System.Windows.Forms.MessageBox.Show("TOPO drawing not attached");
							return ObjectId.Null;
						}

					} catch (Autodesk.AutoCAD.Runtime.Exception ) {
						System.Windows.Forms.MessageBox.Show("TOPO drawing not attached");
						return ObjectId.Null;
					}

                    varPOI = new List<POI>();
					if (wne.getNestedPoints(idAlign, ref varPOI, btrMS, "WALLDESIGN")) {
						dynamic sta = from poi in varPOI
									  orderby poi.Station
									  select poi;
                        List<POI> t = new List<POI>();
                        foreach (var s in sta)
                            t.Add(s);
                        varPOI = t;

						wup.updateProfile(idAlign, varPOI, "CPNT", true, "WALLDESIGN");

					}

				}

			}

			return objProfileView.ObjectId;
		}

		public static ObjectId
		CreateProfileViewPrelim(Alignment objAlign)
		{
			ObjectId id = ObjectId.Null;

			ProfileView objProfileView = null;
			ObjectId idPViewStyle = ObjectId.Null;
			ObjectId idPViewBandSetStyle = ObjectId.Null;

			Debug.Print("CreateProfileViewPrelim - Line 28");


			try {
				idPViewStyle = Prof_Style.getProfileViewStyle("WALL");
				idPViewBandSetStyle = Prof_Style.getProfileViewBandSetStyle("WALL");


			} catch (Autodesk.AutoCAD.Runtime.Exception ) {
				idPViewStyle = Prof_Style.getProfileViewStyle("Standard");
				idPViewBandSetStyle = Prof_Style.getProfileViewBandSetStyle("Standard");

			}

			PromptStatus ps = default(PromptStatus);
			Point3d pnt3dIns = default(Point3d);

			try {
				BaseObjs.acadActivate();
				pnt3dIns = UserInput.getPoint("Select insertion point for Profile View", out ps, osMode: 0);


			} catch (Autodesk.AutoCAD.Runtime.Exception ) {
				objAlign.Erase();
				return ObjectId.Null;

			}

			string strLayer = string.Format("{0}-PROFILEVIEW", objAlign.Name);

			Layer.manageLayers(strLayer);

			Debug.Print("CreateProfileViewPrelim - Line 74");


			try {
				objProfileView = Prof.addProfileView(objAlign.ObjectId, pnt3dIns, idPViewBandSetStyle, idPViewStyle);


			} catch (Autodesk.AutoCAD.Runtime.Exception ex) {
				Application.ShowAlertDialog(ex.ToString());

			}

            TypedValue[] tvs = new TypedValue[2]{
                new TypedValue(1001, "WALLDESIGN"),
                new TypedValue(1005, objProfileView.Handle)
                    
                };

            objProfileView.ObjectId.setXData(tvs, "WALLDESIGN");

            tvs = new TypedValue[2]{
                new TypedValue(1001, "BRKLINE"),
                new TypedValue(1005, 0)                    
                };

            objProfileView.ObjectId.setXData(tvs, "BRKLINE");

            return objProfileView.ObjectId;

		}

	}
}
