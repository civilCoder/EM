using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45.Jig;
using System;
using System.Collections.Generic;

namespace Base_Tools45
{
	public static partial class Draw
	{
		public static string bubTxt { get; set; }

		public static void
		bubble(int numSides)
		{
			ObjectId idWO = ObjectId.Null;  //wipeout
			ObjectId idSM = ObjectId.Null;  //symbol
			ObjectId idTX = ObjectId.Null;  //text
			ObjectId idLDR = ObjectId.Null; //leader

			Object osMode = SnapMode.getOSnap();
			SnapMode.setOSnap(0);

			double angleView = -(double)Application.GetSystemVariable("VIEWTWIST");
			Point3d pnt3d = Pub.pnt3dO;
			Point3d pnt3dEnd = Pub.pnt3dO;

			Handle hTarget = "0000".stringToHandle();
			string layerTarget = "";
			ObjectIdCollection idsLDR = new ObjectIdCollection();
			FullSubentityPath path = new FullSubentityPath();
			List<FullSubentityPath> paths = new List<FullSubentityPath>();
			int scale = Misc.getCurrAnnoScale();

			bool canLdr = false;
			string result = bubTxt;

			try
			{
				do
				{
					Entity ent = Ldr.getFirstLdrPoint(out pnt3d, out canLdr, out hTarget, out layerTarget, out path);
					if (ent == null)
						break;
					Color color = Misc.getColorByLayer();
					idLDR = JigSplineLeader_BB.jigSplineLeader(pnt3d, 0.09, "BUBBLE", color);

					if (idLDR == ObjectId.Null)
						break;

					paths.Add(path);
					pnt3dEnd = idLDR.getLastVertex();

					Ldr.setLdrXData(pnt3dEnd, idLDR, idSM);

					idsLDR.Add(idLDR);

					bool cancel = false;

					if (idsLDR.Count == 1)
					{
						if (numSides == 0)
						{
							cancel = UserInput.getUserInput(result, "\nEnter Callout Number: ", out result);
							if (cancel || result == string.Empty)
								return;
							bubTxt = result;
							idTX = Txt.addMText(result, pnt3dEnd, angleView, 0.8, 0.09, AttachmentPoint.MiddleCenter, "Annotative", "BUBBLE", color, Pub.JUSTIFYCENTER);
						}
						else
						{
							cancel = UserInput.getUserInput(result, "\nEnter Callout Number", out result);
							if (cancel || result == string.Empty)
								return;
							bubTxt = result;
							idSM = Draw.addSymbolAndWipeout(pnt3dEnd, angleView, out idWO, Pub.radius, numSides, true);
							idSM.moveToTop();
							idSM.moveBelow(new ObjectIdCollection {
								idWO
							});
							idTX = Txt.addMText(result, pnt3dEnd, angleView, 0.0, 0.09, AttachmentPoint.MiddleCenter, "Annotative", "BUBBLE", color, Pub.JUSTIFYCENTER);
						}
					}
				}
				while (!canLdr);
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Draw2.cs: line: 91");
			}
			finally
			{
				Application.SetSystemVariable("OSMODE", osMode);
				xRef.unHighlightNestedEntity(paths);
			}

			if (idTX.IsValid)
			{
				addXData(idSM, scale, idTX, idsLDR, idWO, numSides, hTarget, layerTarget);
			}
		}

		public static void
		addXData(ObjectId idSM, int scale, ObjectId idTX, ObjectIdCollection idsLDR, ObjectId idWO, int numSides, Handle hTarget, string layerTarget)
		{
			//link symbol to text
			TypedValue[] tvs = new TypedValue[4];
			tvs.SetValue(new TypedValue(1001, apps.lnkBubs), 0);
			tvs.SetValue(new TypedValue(1000, "TX"), 1);
			tvs.SetValue(new TypedValue(1005, idSM.getHandle()), 2);
			tvs.SetValue(new TypedValue(1070, scale), 3);
			idTX.setXData(tvs, apps.lnkBubs);

			//link wipeout, text, construction note, type of symbol, and leaders to symbol
			tvs = new TypedValue[idsLDR.Count + 6];
			tvs.SetValue(new TypedValue(1001, apps.lnkBubs), 0);
			tvs.SetValue(new TypedValue(1000, "SM"), 1);
			tvs.SetValue(new TypedValue(1005, idWO.getHandle()), 2);
			tvs.SetValue(new TypedValue(1005, idTX.getHandle()), 3);
			tvs.SetValue(new TypedValue(1005, "0000".stringToHandle()), 4); //reserved for construction note
			tvs.SetValue(new TypedValue(1070, numSides), 5); //type of callout symbol

			for (int i = 0; i < idsLDR.Count; i++)
				tvs.SetValue(new TypedValue(1005, idsLDR[i].getHandle()), i + 6);

			idSM.setXData(tvs, apps.lnkBubs);

			//link symbol, text, and target to leader
			tvs = new TypedValue[5];
			tvs.SetValue(new TypedValue(1001, apps.lnkBubs), 0);
			tvs.SetValue(new TypedValue(1000, "LDR"), 1);
			tvs.SetValue(new TypedValue(1005, idTX.getHandle()), 2);
			tvs.SetValue(new TypedValue(1005, hTarget), 3);
			tvs.SetValue(new TypedValue(1000, layerTarget), 4);

			for (int i = 0; i < idsLDR.Count; i++)
				idsLDR[i].setXData(tvs, apps.lnkBubs);

			string h = idTX.getHandle().ToString();

			//add new callout text handle to "lnkBubs" dictionary
			bool exists = false;
			ObjectId idDictBubs = Dict.getNamedDictionary(apps.lnkBubs, out exists);

			try
			{
				using (Transaction tr = BaseObjs.startTransactionDb())
				{
					DBDictionary dictBubs = (DBDictionary)tr.GetObject(idDictBubs, OpenMode.ForWrite);
					DBDictionary dictH = new DBDictionary();
					dictBubs.SetAt(h, dictH);
					tr.AddNewlyCreatedDBObject(dictH, true);
					tr.Commit();
				}
			}
			catch (System.Exception ex)
			{
				BaseObjs.writeDebug(ex.Message + " Draw2.cs: line: 160");
			}
			//idTX.activateObj();
		}
	}
}
