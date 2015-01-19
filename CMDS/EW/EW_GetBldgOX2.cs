using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Linq;

namespace EW
{
	public static class EW_GetBldgOX2
	{
		static double pi = System.Math.PI;

		public static ObjectId
		getBldgOX2(ObjectId idLWPline_LIM, int intBldgNo, string strOption, string strLayer)
		{
			bool exists;
			TinSurface surf = Surf.getTinSurface("CPNT-ON", out exists);

			TypedValue[] tvs = new TypedValue[3]{
				new TypedValue(1001, "OX"),
				new TypedValue(1000, intBldgNo),
				new TypedValue(1000, "OX0")};

			Layer.manageLayers("OX-LIM-OUTER");
			Layer.manageLayers("OX-AREAS-BLDG");
			Layer.manageLayers("OX-AREAS-2d");

			if (!idLWPline_LIM.isRightHand())
				idLWPline_LIM.reversePolyX();


			//BLDGOX0
			Color color = Misc.getColorByBlock(7);
			ObjectId idBldgOX0 = ObjectId.Null;
			if (strOption == "MAKE")
			{
				Polyline objBldgOX0 = (Polyline)Misc.getBldgLimitsEW(idLWPline_LIM); //returns BldgOX0 2dPolyline
				idBldgOX0 = objBldgOX0.ObjectId;
				//outer limit of building footprint
				idBldgOX0.changeProp(color, "OX-AREAS-BLDG", LineWeight.LineWeight020);

				idBldgOX0.checkIfClosed();

				if (!idBldgOX0.isRightHand())
				{
					idBldgOX0.reversePolyX();
				}
				idBldgOX0.setXData(tvs, "OX");
			}
			else if (strOption == "MADE")
			{
				idBldgOX0 = idLWPline_LIM.copy("OX-AREAS-BLDG"); //limit of building footprint already made

				idBldgOX0.changeProp(color, "OX-AREAS-BLDG", LineWeight.LineWeight020);

				idBldgOX0.checkIfClosed();

				if (!idBldgOX0.isRightHand())
				{
					idBldgOX0.reversePolyX();
				}
				idBldgOX0.setXData(tvs, "OX");
			}
			//BLDGOX1
			color = Misc.getColorByBlock(1);
			ObjectId idBldgOX1 = idBldgOX0.offset(EW_Pub.OX_LIMIT_H);   //BldgOX1 2dPolyline
			idBldgOX1.changeProp(color, "OX-AREAS-BLDG", LineWeight.LineWeight020);

			//BLDGOX2
			color = Misc.getColorByBlock(2);
			ObjectId idBldgOX2 = idBldgOX0.offset(EW_Pub.OX_LIMIT_H + 2);//BldgOX2 2dPolyline
			idBldgOX2.changeProp(color, "OX-LIM-OUTER",LineWeight.LineWeight020);

			SelectionSet objSSet = EW_Utility1.buildSSetGradingLim();   //GRADING LIMIT
			ObjectId idGradingLim = objSSet.GetObjectIds()[0];
			int intSide = 0;
			if (idBldgOX1.isOutsideGradingLimit(idGradingLim))
			{
				if (!idGradingLim.isRightHand())
				{
					intSide = 1;
				}
				else
				{
					intSide = -1;
				}

				ObjectId idGradingLimOffset = idGradingLim.offset(0.2 * intSide);
				color = Misc.getColorByBlock(4);
				idGradingLimOffset.changeProp(LineWeight.LineWeight020, color);

				idBldgOX1 = EW_ModAdjacentAreas.modAdjacentOX_Intersect(idBldgOX1, idGradingLimOffset);
				//modified OX1 - portion was outside GRADING LIMIT

				color = Misc.getColorByBlock(1);
				idBldgOX1.changeProp(color, "OX-AREAS-BLDG", LineWeight.LineWeight020);

				tvs[2] = new TypedValue(1000, "OX1");
				idBldgOX1.setXData(tvs, "OX");

				idGradingLimOffset = idGradingLim.offset(0.1 * intSide);
				color = Misc.getColorByBlock(5);
				idGradingLimOffset.changeProp(LineWeight.LineWeight020, color);

				idBldgOX2 = EW_ModAdjacentAreas.modAdjacentOX_Intersect(idBldgOX2, idGradingLimOffset);
				//modified OX2 - portion was outside GRADING LIMIT
				color = Misc.getColorByBlock(2);
				idBldgOX2.changeProp(color, "OX-LIM-OUTER", LineWeight.LineWeight020);

				tvs[2] = new TypedValue(1000, "OX2");
				idBldgOX2.setXData(tvs, "OX");
			}
			else if (idBldgOX2.isOutsideGradingLimit(idGradingLim))
			{
				if (!idGradingLim.isRightHand())
				{
					intSide = 1;
				}
				else
				{
					intSide = -1;
				}

				ObjectId idGradingLimOffset = idGradingLim.offset(0.1 * intSide);
				color = Misc.getColorByBlock(4);
				idGradingLimOffset.changeProp(LineWeight.LineWeight020, color);

				idBldgOX2 = EW_ModAdjacentAreas.modAdjacentOX_Intersect(idBldgOX2, idGradingLimOffset);
				//modified OX2 - portion was outside GRADING LIMIT
				color = Misc.getColorByBlock(1);
				idBldgOX2.changeProp(color, "OX-AREAS-BLDG", LineWeight.LineWeight020);

				tvs[2] = new TypedValue(1000, "OX1");
				idBldgOX1.setXData(tvs, "OX");

				tvs[2] = new TypedValue(1000, "OX2");
				idBldgOX2.setXData(tvs, "OX");
			}
			else
			{
				tvs[2] = new TypedValue(1000, "OX1");
				idBldgOX1.setXData(tvs, "OX");

				tvs[2] = new TypedValue(1000, "OX2");
				idBldgOX2.setXData(tvs, "OX");
			}

			//BLDGOX3
			ObjectId idBldgOX3 = ObjectId.Null;
			if (strOption == "MAKE")
			{
				ObjectId idBldgOXT = getOX_PadLim(idLWPline_LIM, idBldgOX0);
				//building footprint and outer limit of building footprint
				idBldgOX3 = idBldgOXT.offset(-EW_Pub.FOOTING_LIMIT_IN_H);
				color = Misc.getColorByBlock(3);
				idBldgOX3.changeProp(color, "OX-AREAS-BLDG", LineWeight.LineWeight020);
				idBldgOXT.delete();

				tvs[2] = new TypedValue(1000, "OX3");
				idBldgOX3.setXData(tvs, "OX");


			}
			else if (strOption == "MADE")
			{
				idBldgOX3 = idBldgOX0.offset(-EW_Pub.FOOTING_LIMIT_IN_H);
				color = Misc.getColorByBlock(3);
				idBldgOX3.changeProp(color, "OX-AREAS-BLDG", LineWeight.LineWeight020);

				tvs[2] = new TypedValue(1000, "OX3");
				idBldgOX3.setXData(tvs, "OX");


			}

			//BLDGOX4
			ObjectId idBldgOX4 = idBldgOX3.offset(-2);
			//BldgOX4 2dPolyline
			color = Misc.getColorByBlock(4);
			idBldgOX4.changeProp(color, "OX-AREAS-BLDG", LineWeight.LineWeight020);

			tvs[2] = new TypedValue(1000, "OX4");
			idBldgOX4.setXData(tvs, "OX");
			ObjectId idLine = ObjectId.Null;

			Handle h2d, h3d;
			double dblSlope, dblAngBase, dblAngTest;
			Point3d pnt3dCEN = Pub.pnt3dO, pnt3dTAR = Pub.pnt3dO, pnt3dX = Pub.pnt3dO;

			ObjectId idDict = Dict.getNamedDictionary("GRADEDOCK", out exists);
			if (!exists)
			{
				Point3d varPntCen = idLWPline_LIM.getCentroid();
				//  objLWPline_LIM.getXdata "BldgSlab", varXDataType, varXData
				//
				//  Dim objBldgSlab As Polyline
				//  Set objBldgSlab = clsdwg.thisdrawing.HandleToObject(varXData(1))
				//  Dim varPntsLWPline As Variant
				//  varPntsLWPline = objBldgSlab.getCoordinates3d
				//  Dim varPnts3dPoly As Variant
				//  varPnts3dPoly = convert2dCoordsTo3dCoords(varPntsLWPline)
				//
				//
				//  Dim varVector As Vector
				//  varVector = getClosetAdjacentSegment(dPntCen, objBldgSlab)
				//  MsgBox ("Direction to nearest segment is: " & varVector.Dir & _
				//'           vbCr & _
				//'           "Distance to nearest segment is: " & varVector.Dist)
				//
				//  If isInside(varPntCen, varPnts3dPoly) = False Then
				//    MsgBox "Calculated centroid location is outside limits of Building footprint"
				//    Exit Function
				//  End If

				double elevCen = surf.FindElevationAtXY(varPntCen.X, varPntCen.Y);
				//SURFACE = "CPNT-ON"
				double dblAngTar = 0; int intMark = 0;

				Geom.getEastWestBaseLineDir(idLWPline_LIM, ref dblAngTar, ref intMark);

				Point3d dPntTAR = varPntCen.traverse(dblAngTar, 20);
				double elevTar = surf.FindElevationAtXY(dPntTAR.X, dPntTAR.Y);
				dblSlope = System.Math.Round((elevTar - varPntCen.Z) / 20, 4);

				if (dblSlope == 0){
					dPntTAR = varPntCen.traverse(dblAngTar + pi / 2, 20);
					elevTar = surf.FindElevationAtXY(dPntTAR.X, dPntTAR.Y);
					dblSlope = System.Math.Round((elevTar - varPntCen.Z) / 20, 4);


					if (System.Math.Round(dblSlope, 3) == 0)
					{
						dPntTAR = varPntCen.traverse(dblAngTar - pi / 2, 20); //back to original orientation					
						elevTar = surf.FindElevationAtXY(dPntTAR.X, dPntTAR.Y);

					}
 
				}else if(dblSlope < 0)
					dblAngTar = dblAngTar + pi;

				dblAngBase = varPntCen.getDirection(dPntTAR);

				idLine = Draw.addLine(varPntCen, dPntTAR);

				EW_Dict.updateDictGRADEDOCK(idLWPline_LIM.getHandle(), "0".stringToHandle(), dblSlope, varPntCen, dPntTAR); //obj3dPoly.handle not pertinent at earthwork stage				
			}
			else
			{

				EW_Dict.retrieveDictGRADEDOCK(out h2d, out h3d, out dblSlope, out pnt3dCEN, out pnt3dTAR);

				dblAngBase = pnt3dCEN.getDirection(pnt3dTAR);
				double dblLenBase = pnt3dCEN.getDistance(pnt3dTAR);

				Point3d varPntPolar = pnt3dCEN.traverse(dblAngBase, dblLenBase);
				idLine = Draw.addLine(pnt3dCEN, varPntPolar);

			}

			List<Point3d> varPntInt = idLine.intersectWith(idBldgOX0, extend.source);          

			if (varPntInt.Count > 0)
			{

				for (int i = 0; i < varPntInt.Count; i++)
				{
					pnt3dX = varPntInt[i];
					dblAngTest = pnt3dCEN.getDirection(pnt3dX);


					if (System.Math.Round(dblAngTest, 4) == System.Math.Round(dblAngBase, 4))
					{
						idLine = Draw.addLine(pnt3dCEN, pnt3dX);
						pnt3dTAR = new Point3d(pnt3dX.X, pnt3dX.Y, pnt3dCEN.Z + pnt3dCEN.getDistance(pnt3dTAR) * dblSlope);
					}
				}
			}

			double elev = surf.FindElevationAtXY(pnt3dCEN.X, pnt3dCEN.Y);
			pnt3dCEN = new Point3d(pnt3dCEN.X, pnt3dCEN.Y, elev);

			double dblOffOX = EW_Pub.OUTER_FOOTINGS_V + EW_Utility1.getDepth(strLayer, "OX");
			color = Misc.getColorByLayer();
			//OX1
			ObjectId id3dPoly = EW_Build3dPoly.build3dPoly(idBldgOX1, pnt3dCEN, pnt3dTAR, dblSlope, "OX", "OX-BRKLINE", "OX1", dblOffOX, 20);
			id3dPoly.changeProp(LineWeight.LineWeight050, color);

			varPntInt = idLine.intersectWith(idBldgOX2, extend.source);
			idLine.delete();


			if (varPntInt.Count > 2)
			{
				for (int i = 0; i < varPntInt.Count; i++)
				{
					pnt3dX = varPntInt[i];
					dblAngTest = pnt3dCEN.getDirection(pnt3dX);

					if (System.Math.Round(dblAngTest, 4) == System.Math.Round(dblAngBase, 4))
					{
						idLine = Draw.addLine(pnt3dCEN, pnt3dX);

						pnt3dTAR = pnt3dTAR.addElevation(pnt3dCEN.Z + pnt3dCEN.getDistance(pnt3dX) * dblSlope);
						idLine.delete();
						break; 
					}
				}
			}

			dblOffOX = EW_Pub.OUTER_FOOTINGS_V + EW_Utility1.getDepth(strLayer, "OX");

			//OX3
			id3dPoly = EW_Build3dPoly.build3dPoly(idBldgOX3, pnt3dCEN, pnt3dTAR, dblSlope, "OX", "OX-BRKLINE", "OX3", dblOffOX);
			id3dPoly.changeProp(LineWeight.LineWeight050, color);

			dblOffOX = EW_Pub.COLUMN_FOOTINGS_V + EW_Utility1.getDepth(strLayer, "OX");

			//OX4
			id3dPoly = EW_Build3dPoly.build3dPoly(idBldgOX4, pnt3dCEN, pnt3dTAR, dblSlope, "OX", "OX-BRKLINE", "OX4", dblOffOX);
			id3dPoly.changeProp(LineWeight.LineWeight050, color);

			Point3dCollection pnts3d = idBldgOX0.poly_pnt3dColl();
			objSSet = EW_Utility1.buildSSet17(pnts3d); //get K BRACE areas

			ObjectId[] ids = objSSet.GetObjectIds();
			
			color = Misc.getColorByBlock(7);
			//BEGIN K BRACE
			for (int i = 0; i < ids.Length; i++)
			{
				ObjectId idBrace = ids[i];
				idBrace = idBrace.offset(5.0);
				idBrace.changeProp(LineWeight.LineWeight030, color);

				dblOffOX = EW_Pub.K_BRACE_V + EW_Utility1.getDepth(strLayer, "OX");

				id3dPoly = EW_Build3dPoly.build3dPoly(idBrace, pnt3dCEN, pnt3dTAR, dblSlope, "OX", "OX-BRKLINE", "OTHER", dblOffOX);
				id3dPoly.changeProp(color, "OX-BRKLINE");
				//bottom of K-Brace overexcavation

				dblOffOX = EW_Pub.COLUMN_FOOTINGS_V + EW_Utility1.getDepth(strLayer, "OX");
				color = Misc.getColorByBlock(5);
				idBrace = idBrace.offset(0.2);
				idBrace.changeProp(color);

				List<Point3d> pnts3dBrace = idBrace.getCoordinates3dList();
				List<Point3d> pnts3dNew = new List<Point3d>();

				for (int j = 0; j < pnts3dBrace.Count; j++)
				{
					Point3d pnt3dBEG = pnts3dBrace[i + 0];
					Point3d pnt3dEND = pnts3dBrace[i + 1];

					double dblAng = pnt3dBEG.getDirection(pnt3dEND);
					//angle of each segment

					double angDiff = System.Math.Round(dblAngBase - dblAng, 2);
					
					if(angDiff == 0){
						pnt3dX = pnt3dBEG.traverse(dblAng - pi / 2, (EW_Pub.K_BRACE_V - dblOffOX) * 5.0);
						pnts3dNew.Add(pnt3dX);
						
					}else if(angDiff == System.Math.Round(pi / 2, 2)        || angDiff == System.Math.Round(3 * pi / 2, 2)){
						pnt3dX = pnt3dBEG.traverse(dblAng + pi, (EW_Pub.K_BRACE_V - dblOffOX) * 5.0);
						pnts3dNew.Add(pnt3dX);
					}else if(angDiff == System.Math.Round(-1 * pi / 2, 2)   || angDiff == System.Math.Round(-3 * pi / 2, 2)){
						pnt3dX = pnt3dBEG.traverse(dblAng - pi, (EW_Pub.K_BRACE_V - dblOffOX) * 5.0);
						pnts3dNew.Add(pnt3dX);
					}
					else if (angDiff == System.Math.Round(1 * pi, 2) || angDiff == System.Math.Round(3 * pi, 2)){
						pnt3dX = pnt3dBEG.traverse(dblAng + pi / 2, (EW_Pub.K_BRACE_V - dblOffOX) * 5.0);
						pnts3dNew.Add(pnt3dX);
					}
					else if (angDiff == System.Math.Round(-1 * pi, 2) || angDiff == System.Math.Round(-3 * pi, 2)){
						pnt3dX = pnt3dBEG.traverse(dblAng - pi / 2, (EW_Pub.K_BRACE_V - dblOffOX) * 5.0);
						pnts3dNew.Add(pnt3dX);
					}

					//Call addCircle(dPntPolar, 2, 1)

				}
				
				pnts3dNew.Add(pnts3dNew[0]);
				ObjectId idPoly = Draw.addPoly(pnts3dNew);
				idPoly.checkIfClosed3d();

				id3dPoly = EW_Build3dPoly.build3dPoly(id3dPoly, pnt3dCEN, pnt3dTAR, dblSlope, "OX", "OX-BRKLINE", "OTHER", dblOffOX);
				id3dPoly.changeProp(color, "OX-BRKLINE");

				idPoly.delete();

			}

			//END K BRACE

			return idBldgOX2;
		}


		public static ObjectId 
		getOX_PadLim(ObjectId idPolyBldg, ObjectId idPolyBndry){

			//BEGIN GET SEGMENT DATA
			List<Point3d> varPntsBldg = idPolyBldg.getCoordinates3dList();
			List<SEG> segs = new List<SEG>();
			Point3d pnt3d1 = Pub.pnt3dO, pnt3d2 = Pub.pnt3dO;
			for (int i = 0; i < varPntsBldg.Count; i++)
			{
				pnt3d1 = varPntsBldg[i + 0];
				pnt3d2 = varPntsBldg[i + 1];
				SEG seg = new SEG();
				seg.Index = i;
				seg.Length = pnt3d1.getDistance(pnt3d2);
				seg.Direction = pnt3d1.getDirection(pnt3d2);
				seg.Beg = pnt3d1;
				seg.End = pnt3d2;
				segs.Add(seg);

			}
			//END GET SEGMENT DATA

            //var sortLen = from s in segs
            //              orderby s.Length descending
            //              select s;

            //List<SEG> tmp = new List<SEG>();
            //foreach (var s in sortLen)
            //    tmp.Add(s);
            //segs = tmp;

            var sortLen2 = from s in segs
                          orderby s.Length descending
                          select new SEG 
                          {
                              Beg = s.Beg,
                              Direction = s.Direction,
                              End = s.End,
                              Index = s.Index,
                              Length = s.Length
                          };
            segs = sortLen2.ToList();

			//BEGIN GET LONGEST SEGMENT
			Point3d pnt3dBeg = segs[0].Beg;
			Point3d pnt3dEnd = segs[0].End;
			double dblLen1 = segs[0].Length;
			double dblAng1 = segs[0].Direction;//orientation of longest segment
			
			Color color = Misc.getColorByBlock(10);

			ObjectId idLineOX = Draw.addLine(pnt3dBeg, pnt3dEnd);
			idLineOX.changeProp(LineWeight.LineWeight200, color);

			//intersect longest segment with outer limit of bldg footprint
			List<Point3d> varPntRet = idLineOX.intersectWith(idPolyBndry, extend.source);
			idLineOX.delete();

			ObjectId idLine1 = ObjectId.Null, idLine2 = ObjectId.Null;

			if(varPntRet.Count == 2){
				pnt3d1 = varPntRet[0];
				pnt3d2 = varPntRet[1];
			}

			idLine1 = Draw.addLine(pnt3d1, pnt3d2);
			Vector3d v3d1 = pnt3d2 - pnt3d1;

			double dblLenX = pnt3d1.getDistance(pnt3d2);
			double dblAngX = pnt3d1.getDirection(pnt3d2);
			Point3d pnt3dTemp;
			List<Point3d> pnts3dOX = new List<Point3d>();
			//check if same orientation as longest segment
			if (System.Math.Round(dblAngX, 4) != System.Math.Round(dblAng1, 4))
			{
				pnts3dOX.Add(pnt3d2);
				pnts3dOX.Add(pnt3d1);

				pnt3dTemp = pnt3d2;
				pnt3d2 = pnt3d1;
				pnt3d1 = pnt3dTemp;
				dblAngX = dblAngX + pi;
			}
			else
			{
				pnts3dOX.Add(pnt3d1);
				pnts3dOX.Add(pnt3d2);
			}

			//END GET LONGEST SEGMENT

			//BEGIN TEST FOR LONGEST PERPENDICULAR SEGMENT
			//segs[1] is second longest segment
			for (int i = 1 ; i < segs.Count; i++)
			{
				pnt3d1 = segs[i].Beg;
				pnt3d2 = segs[i].End;

				double dblLen2 = segs[i].Length;
				double dblAng2 = segs[i].Direction;
				Vector3d v3d2 = pnt3d2 - pnt3d1;


				double dblAngDelta = v3d1.getAngle2Vectors(v3d2);
				
				if (dblAngDelta != 0)
				{

					if (System.Math.Abs(System.Math.Round(dblAngDelta, 3)) == System.Math.Round(pi / 2, 3))
					{
						idLineOX = Draw.addLine(pnt3d1, pnt3d2);
						idLineOX.changeProp(LineWeight.LineWeight200, color);

						varPntRet = idLineOX.intersectWith(idPolyBndry, extend.source);
						idLineOX.delete();

						pnt3d1 = varPntRet[0];
						pnt3d2 = varPntRet[1];

						idLine2 = Draw.addLine(pnt3d1, pnt3d2);

						dblAng1 = pnt3d1.getDirection(pnt3d2);


						if (System.Math.Round(dblAng2, 4) != System.Math.Round(dblAng1, 4))
						{
							pnt3dTemp = pnt3d2;
							pnt3d2 = pnt3d1;
							pnt3d1 = pnt3dTemp;

						}

						break;

					}

				}

			}

			Point3d pnt3dBase, pnt3dX;
			if (System.Math.Round(pnts3dOX[1].X, 3) == System.Math.Round(pnt3d1.X, 3))
			{
				if (System.Math.Round(pnts3dOX[1].Y, 3) == System.Math.Round(pnt3d1.Y, 3))
				{
					pnt3dBase = pnt3d2;
					pnt3dX = pnt3dBase.traverse(dblAngX + pi, dblLenX);

					pnts3dOX.Add(pnt3dBase);
					pnts3dOX.Add(pnt3dX);
				}


			}
			else if (System.Math.Round(pnts3dOX[0].X, 3) == System.Math.Round(pnt3d2.X, 3))
			{

				if (System.Math.Round(pnts3dOX[0].Y, 3) == System.Math.Round(pnt3d2.Y, 3))
				{
					pnt3dBase = pnt3d1;

					pnt3dX = pnt3dBase.traverse(dblAngX, dblLenX);

					pnts3dOX.Add(pnt3dX);
					pnts3dOX.Add(pnt3dBase);
				}


			}
			else
			{
				varPntRet = idLine2.intersectWith(idLine1, extend.source);

			}

			pnts3dOX.Add(varPntRet[0]);

			ObjectId idPoly = Draw.addPoly(pnts3dOX, "OX-AREAS-2d");

			idPoly.reversePolyX();

			return idPoly;

		} 
	}
}
