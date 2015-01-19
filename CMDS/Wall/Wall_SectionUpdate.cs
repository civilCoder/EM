using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using p = Wall.Wall_Public;
using wa2 = Wall.Wall_Alignment2;
using wa2d = Wall.Wall_Alignment2d;
using wdp = Wall.Wall_DesignProfile;


namespace Wall
{
	public static class Wall_SectionUpdate
	{
		static Wall_Form.frmWall1 fWall1 = Wall_Forms.wForms.fWall1;


		private static System.Drawing.Font font2a = new System.Drawing.Font(FontFamily.GenericSansSerif, 8);

		private static Pen penBlack2 = new Pen(Color.Black,2);
		private static Pen penPL4 = new Pen(Color.Black, 2); private static float[] sngDashValuesPL = {	40,	2,	4,	2,	4,	2 };
		private static Pen pen1 = new Pen(Color.Red, 2);
		private static Pen pen2 = new Pen(Color.Yellow, 2);
		private static Pen pen3 = new Pen(Color.Green, 2);
		private static Pen pen4 = new Pen(Color.Cyan, 2);
		private static Pen pen5 = new Pen(Color.Blue, 2);
		private static Pen pen6 = new Pen(Color.Magenta, 2);
		private static Pen pen7 = new Pen(Color.Black, 2);
		private static Pen pen8 = new Pen(Color.Gray, 2);
		private static Pen penDashedRF = new Pen(Color.Black, 2);
		private static Pen penDashedEX = new Pen(Color.Gray, 1); private static float[] sngDashedValuesEX = { 4, 4 };

		public static void
		UpdateSection_2d(SECTIONDATASET vSDS, float varScale = -99){
 
			Graphics g2d = fWall1.PictureBox2d.CreateGraphics();
			fWall1.PictureBox2d.Refresh();

			penDashedRF.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
			penDashedEX.DashPattern = sngDashedValuesEX;
			penPL4.DashPattern = sngDashValuesPL;

			PointF pnt1 = default(PointF);
			PointF pnt2 = default(PointF);
			PointF[] pnts = new PointF[2];
			PointF[] pntsEX = new PointF[6];

			float dXscale = 1;
			float dYscale = 1;

			float dblDX = 0;
			float dblDY = 0;

			string strInfo = null;
			PointF pntCF = default(PointF);
			PointF pntPL = default(PointF);
			PointF pntDR = default(PointF);

			PointF RF = default(PointF);

			RF.X = 15;
			RF.Y = 200; //RF
			


			try {

				if (vSDS.RESOLVED == false) {
					//DESIGN REF & STATION
					pntDR.X = RF.X + 5;
					pntDR.Y = 100;
					strInfo = string.Format("REF : {0:##=##.00}",vSDS.STA);
					if (vSDS.RESOLVED == true) {
						strInfo = strInfo + " -RESOLVED";
					} else {
						strInfo = strInfo + " - NOT RESOLVED";
					}
					g2d.DrawString(strInfo, font2a, Brushes.Red, pntDR);


				} else {
					float XTot = vSDS.X;

					EXGROUND vExGround = vSDS.EX;

					dblDX = (float)vSDS.X;
					dblDY = (float)(vSDS.PL.ELEV - vSDS.RF.ELEV);

					if (vSDS.dYh > 0) {
						dblDY = dblDY + 2;
					}


					if (varScale == -99) {
						dXscale = 800 / dblDX;
						dYscale = 600 / dblDY;


						if (dXscale < dYscale) {
							//DX controls
							dXscale = (int)(dXscale * 0.8);
							dYscale = dXscale;


						} else {
							//DY controls
							dYscale = (int)(dYscale * 0.8);
							dXscale = dYscale;

						}

						fWall1.VIEWSCALE = (int)dXscale;
						fWall1.numUpDownScale.Value = (decimal)dXscale;


					} else {
						dXscale = (float)varScale;
						dYscale = (float)varScale;

					}

					float dxFL = 0;
					float dyFL = 0;
					float dxTC = 0;
					float dyTC = 0;
					float dxBB1 = 0;
					float dyBB1 = 0;
					float dxW0 = 0;
					float dyW0 = 0;
					float dxW1 = 0;
					float dyW1 = 0;
					float dxW2 = 0;
					float dyW2 = 0;
					float dxW3 = 0;
					float dyW3 = 0;
					float dxG0 = 0;
					float dyG0 = 0;
					float dxGB = 0;
					float dyGB = 0;
					float dxTOE = 0;
					float dyTOE = 0;
					float dxTOP = 0;
					float dyTOP = 0;
					float dxPL = 0;
					float dyPL = 0;
					float dX = 0;
					float dY = 0;

					PointF FL = default(PointF);
					PointF TC = default(PointF);
					PointF BB1 = default(PointF);
					PointF W0 = default(PointF);
					PointF W1 = default(PointF);
					PointF W2 = default(PointF);
					PointF W3 = default(PointF);
					PointF W4 = default(PointF);
					PointF W5 = default(PointF);
					PointF W6 = default(PointF);
					PointF W7 = default(PointF);
					PointF W8 = default(PointF);
					PointF W9 = default(PointF);
					PointF G0 = default(PointF);
					PointF GB = default(PointF);
					PointF TOE = default(PointF);
					PointF TOP = default(PointF);
					PointF PL = default(PointF);

					dxFL = vSDS.FL.OFFSET - vSDS.RF.OFFSET;
					dyFL = vSDS.FL.ELEV - vSDS.RF.ELEV;

					FL.X = RF.X + dxFL * dXscale;
					FL.Y = RF.Y - dyFL * dYscale;
					//FL

					dxTC = vSDS.TC.OFFSET - vSDS.FL.OFFSET;
					dyTC = vSDS.TC.ELEV - vSDS.FL.ELEV;

					TC.X = FL.X + dxTC * dXscale;
					TC.Y = FL.Y - dyTC * dYscale;
					//TC

					//WALL HEIGHT
					//NO WALL
					if (vSDS.dYh == 0) {

						//UPSLOPE
						if (vSDS.SG > 0) {

							dxBB1 = vSDS.BB1.OFFSET - vSDS.TC.OFFSET;
							dyBB1 = vSDS.BB1.ELEV - vSDS.TC.ELEV;

							BB1.X = TC.X + dxBB1 * dXscale;
							BB1.Y = TC.Y + dyBB1 * dYscale;

							dxTOE = vSDS.TOE.OFFSET - vSDS.BB1.OFFSET;
							dyTOE = vSDS.TOE.ELEV - vSDS.BB1.ELEV;

							TOE.X = BB1.X + dxTOE * dXscale;
							TOE.Y = BB1.Y - dyTOE * dYscale;

							dxTOP = vSDS.TOP.OFFSET - vSDS.TOE.OFFSET;
							dyTOP = vSDS.TOP.ELEV - vSDS.TOE.ELEV;

							TOP.X = TOE.X + dxTOP * dXscale;
							TOP.Y = TOE.Y - dyTOP * dYscale;
							//TOP

							dxPL = vSDS.PL.OFFSET - vSDS.TOP.OFFSET;
							dyPL = vSDS.PL.ELEV - vSDS.TOP.ELEV;

							PL.X = TOP.X + dxPL * dXscale;
							PL.Y = TOP.Y - dyPL * dYscale;
							//PL

						//DOWNSLOPE
						} else if (vSDS.SG < 0) {

							dxBB1 = vSDS.BB1.OFFSET - vSDS.TC.OFFSET;
							dyBB1 = vSDS.BB1.ELEV - vSDS.TC.ELEV;

							BB1.X = TC.X + dxBB1 * dXscale;
							BB1.Y = TC.Y + dyBB1 * dYscale;

							dxTOP = vSDS.TOP.OFFSET - vSDS.BB1.OFFSET;
							dyTOP = vSDS.TOP.ELEV - vSDS.BB1.ELEV;

							TOP.X = BB1.X + dxTOP * dXscale;
							TOP.Y = BB1.Y - dyTOP * dYscale;
							//TOP

							dxTOE = vSDS.TOE.OFFSET - vSDS.TOP.OFFSET;
							dyTOE = vSDS.TOE.ELEV - vSDS.TOP.ELEV;

							TOE.X = TOP.X + dxTOE * dXscale;
							TOE.Y = TOP.Y - dyTOE * dYscale;
							//TOE

							dxPL = vSDS.PL.OFFSET - vSDS.TOE.OFFSET;
							dyPL = vSDS.PL.ELEV - vSDS.TOE.ELEV;

							PL.X = TOE.X + dxPL * dXscale;
							PL.Y = TOE.Y - dyPL * dYscale;
							//PL

						}

					//WALL AT B1
					} else if (vSDS.dYh > 0) {

						dxW0 = vSDS.W0.OFFSET - vSDS.TC.OFFSET;
						dyW0 = vSDS.W0.ELEV - vSDS.TC.ELEV;

						W0.X = TC.X + dxW0 * dXscale;
						W0.Y = TC.Y - dyW0 * dYscale;
						//W0

						dxW1 = vSDS.W1.OFFSET - vSDS.W0.OFFSET;
						dyW1 = vSDS.W1.ELEV - vSDS.W0.ELEV;

						W1.X = W0.X + dxW1 * dXscale;
						W1.Y = W0.Y - dyW1 * dYscale;
						//W1

						dxW2 = vSDS.W2.OFFSET - vSDS.W1.OFFSET;
						dyW2 = vSDS.W2.ELEV - vSDS.W1.ELEV;

						W2.X = W1.X + dxW2 * dXscale;
						W2.Y = W1.Y - dyW2 * dYscale;
						//W2

						dxW3 = vSDS.W3.OFFSET - vSDS.W2.OFFSET;
						dyW3 = vSDS.W3.ELEV - vSDS.W2.ELEV;

						W3.X = W2.X + dxW3 * dXscale;
						W3.Y = W2.Y - dyW3 * dYscale;
						//W3

						dxG0 = vSDS.G0.OFFSET - vSDS.W3.OFFSET;
						dyG0 = vSDS.G0.ELEV - vSDS.W3.ELEV;

						G0.X = W3.X + dxG0 * dXscale;
						G0.Y = W3.Y - dyG0 * dYscale;
						//G0

						dxGB = vSDS.GB.OFFSET - vSDS.W3.OFFSET;
						dyGB = vSDS.GB.ELEV - vSDS.W3.ELEV;

						GB.X = W3.X + dxGB * dXscale;
						GB.Y = W3.Y;
						//GB

						dxTOE = vSDS.TOE.OFFSET - vSDS.GB.OFFSET;
						dyTOE = vSDS.TOE.ELEV - vSDS.GB.ELEV;

						TOE.X = GB.X + dxTOE * dXscale;
						TOE.Y = GB.Y - dyTOE * dYscale;
						//TOE

						dxTOP = vSDS.TOP.OFFSET - vSDS.TOE.OFFSET;
						dyTOP = vSDS.TOP.ELEV - vSDS.TOE.ELEV;

						TOP.X = TOE.X + dxTOP * dXscale;
						TOP.Y = TOE.Y - dyTOP * dYscale;
						//TOP

						dxPL = vSDS.PL.OFFSET - vSDS.TOP.OFFSET;
						dyPL = vSDS.PL.ELEV - vSDS.TOP.ELEV;

						PL.X = TOP.X + dxPL * dXscale;
						PL.Y = TOP.Y - dyPL * dYscale;
						//PL

					//WALL AT PL
					} else if (vSDS.dYh < 0) {

						dxTOP = vSDS.TOP.OFFSET - vSDS.TC.OFFSET;
						dyTOP = vSDS.TOP.ELEV - vSDS.TC.ELEV;

						TOP.X = TC.X + dxTOP * dXscale;
						TOP.Y = TC.Y - dyTOP * dYscale;
						//TOP

						dxTOE = vSDS.TOE.OFFSET - vSDS.TOP.OFFSET;
						dyTOE = vSDS.TOE.ELEV - vSDS.TOP.ELEV;

						TOE.X = TOP.X + dxTOE * dXscale;
						TOE.Y = TOP.Y - dyTOE * dYscale;
						//TOE

						dxGB = vSDS.GB.OFFSET - vSDS.TOP.OFFSET;
						dyGB = vSDS.GB.ELEV - vSDS.TOP.ELEV;

						GB.X = TOE.X + dxGB * dXscale;
						GB.Y = TOE.Y - dyGB * dYscale;
						//GB

						dxG0 = vSDS.G0.OFFSET - vSDS.GB.OFFSET;
						dyG0 = vSDS.G0.ELEV - vSDS.GB.ELEV;

						G0.X = GB.X + dxG0 * dXscale;
						G0.Y = GB.Y - dyG0 * dYscale;
						//G0

						dxW0 = vSDS.W0.OFFSET - vSDS.G0.OFFSET;
						dyW0 = vSDS.W0.ELEV - vSDS.G0.ELEV;

						W0.X = G0.X + dxW0 * dXscale;
						W0.Y = G0.Y - dyW0 * dYscale;
						//W0

						dxW1 = vSDS.W1.OFFSET - vSDS.W0.OFFSET;
						dyW1 = vSDS.W1.ELEV - vSDS.W0.ELEV;

						W1.X = W0.X + dxW1 * dXscale;
						W1.Y = W0.Y - dyW1 * dYscale;
						//W1

						dxW2 = vSDS.W2.OFFSET - vSDS.W1.OFFSET;
						dyW2 = vSDS.W2.ELEV - vSDS.W1.ELEV;

						W2.X = W1.X + dxW2 * dXscale;
						W2.Y = W1.Y - dyW2 * dYscale;
						//W2

						dxW3 = vSDS.W3.OFFSET - vSDS.W2.OFFSET;
						dyW3 = vSDS.W3.ELEV - vSDS.W2.ELEV;

						W3.X = W2.X + dxW3 * dXscale;
						W3.Y = W2.Y - dyW3 * dYscale;
						//W3

						dxPL = vSDS.PL.OFFSET - vSDS.W3.OFFSET;
						dyPL = vSDS.PL.ELEV - vSDS.W3.ELEV;

						PL.X = W3.X + dxPL * dXscale;
						PL.Y = W3.Y - dyPL * dYscale;
						//PL

					}

					float X0 = 0;
					float X1 = 0;
					float X2 = 0;
					X0 = vSDS.X0;
					X1 = (float)System.Math.Round(vSDS.X1, 3);
					X2 = (float)System.Math.Round(vSDS.X2, 3);

					if(vSDS.dYh == 0){
							if (vSDS.SG > 0) {
								PointF[] ptsA = new PointF[6] {
									RF,
									FL,
									TC,
									TOE,
									TOP,
									PL
								};
								g2d.DrawLines(penBlack2, ptsA);


							} else if (vSDS.SG < 0) {
								PointF[] ptsA = new PointF[6] {
									RF,
									FL,
									TC,
									TOP,
									TOE,
									PL
								};
								g2d.DrawLines(penBlack2, ptsA);

							}

						
					}else if(vSDS.dYh > 0){
							PointF[] ptsB = new PointF[12] {
								RF,
								FL,
								TC,
								W0,
								W1,
								W2,
								W3,
								G0,
								GB,
								TOE,
								TOP,
								PL
							};
							g2d.DrawLines(penBlack2, ptsB);
							labelX0S0("B2", "S2", g2d, font2a, GB, TOE);
						
					}else if(vSDS.dYh < 0){
							PointF[] ptsB = new PointF[12] {
								RF,
								FL,
								TC,
								TOP,
								TOE,
								GB,
								G0,
								W0,
								W1,
								W2,
								W3,
								PL
							};
							g2d.DrawLines(penBlack2, ptsB);
							labelX0S0("B2", "S2", g2d, font2a, TOE, GB);
						
					}

					drawMarker(g2d, pen2, RF, GB, vSDS.GB.OFFSET, -10);

					//DRAW WALL

					dX = vSDS.W4.OFFSET - vSDS.W3.OFFSET;
					dY = vSDS.W4.ELEV - vSDS.W3.ELEV;
					W4.X = W3.X + dX * dXscale;
					W4.Y = W3.Y - dY * dYscale;

					dX = vSDS.W5.OFFSET - vSDS.W4.OFFSET;
					dY = vSDS.W5.ELEV - vSDS.W4.ELEV;
					W5.X = W4.X + dX * dXscale;
					W5.Y = W4.Y - dY * dYscale;

					dX = vSDS.W6.OFFSET - vSDS.W5.OFFSET;
					dY = vSDS.W6.ELEV - vSDS.W5.ELEV;
					W6.X = W5.X + dX * dXscale;
					W6.Y = W5.Y - dY * dYscale;

					dX = vSDS.W7.OFFSET - vSDS.W6.OFFSET;
					dY = vSDS.W7.ELEV - vSDS.W6.ELEV;
					W7.X = W6.X + dX * dXscale;
					W7.Y = W6.Y - dY * dYscale;

					dX = vSDS.W8.OFFSET - vSDS.W7.OFFSET;
					dY = vSDS.W8.ELEV - vSDS.W7.ELEV;
					W8.X = W7.X + dX * dXscale;
					W8.Y = W7.Y - dY * dYscale;

					dX = vSDS.W9.OFFSET - vSDS.W8.OFFSET;
					dY = vSDS.W9.ELEV - vSDS.W8.ELEV;
					W9.X = W8.X + dX * dXscale;
					W9.Y = W8.Y - dY * dYscale;

					PointF[] ptsW = new PointF[11] {
						W0,
						W1,
						W2,
						W3,
						W4,
						W5,
						W6,
						W7,
						W8,
						W9,
						W0
					};
					g2d.FillPolygon(Brushes.Gray, ptsW);

					//---------------------------------------------------------------------------------------------------------------------------

					//DESIGN REF
					pnt1.X = RF.X;
					pnt1.Y = 10;
					pnt2.X = RF.X;
					pnt2.Y = 350;
					pnts = new PointF[2] {
						pnt1,
						pnt2
					};
					g2d.DrawLines(penDashedRF, pnts);

					//PROPERTY LINE
					pnt1.X = PL.X;
					pnt1.Y = 50;
					pnt2.X = PL.X;
					pnt2.Y = 350;
					pnts = new PointF[2] {
						pnt1,
						pnt2
					};
					g2d.DrawLines(penPL4, pnts);

					//LABELS

					if (dxFL != 0) {
						labelX0S0("XO", "S0", g2d, font2a, RF, FL);
					}

				   
					if (vSDS.dYh == 0) {

						if (vSDS.SG >= 0) {
							//SG
							if (X2 > 0) 
								labelSLOPE("S", g2d, font2a, TOE, TOP, true);
							

							labelX0S0("B1", "S1", g2d, font2a, TC, BB1);
							labelX0S0("B3", "S3", g2d, font2a, TOP, PL);


						} else { //SG
							
							if (X2 > 0) {
								labelSLOPE("S", g2d, font2a, TOP, TOE, false);
							}

							labelX0S0("B1", "S1", g2d, font2a, TC, BB1);
							labelX0S0("B3", "S3", g2d, font2a, TOE, PL);

						}


					} else {

						if (vSDS.SG >= 0) {
							//SG
							labelSLOPE("S", g2d, font2a, TOE, TOP, true);

							labelX0S0("B1", "S1", g2d, font2a, TC, W0);
							labelX0S0("B2", "S2", g2d, font2a, GB, TOE);
							labelX0S0("B3", "S3", g2d, font2a, TOP, PL);


						} else {
							//SG
							labelSLOPE("S", g2d, font2a, TOP, TOE, false);

							labelX0S0("B1", "S1", g2d, font2a, TC, TOP);
							labelX0S0("B2", "S2", g2d, font2a, TOE, GB);
							labelX0S0("B3", "S3", g2d, font2a, W3, PL);

						}

						labelMISC("WT", g2d, font2a, W1, W2, -10, -25);
						labelMISC("GW", g2d, font2a, W3, GB, -10, -5);

					}

					//CF
					pntCF.X = FL.X + (TC.X - FL.X) / 2 - 40;
					pntCF.Y = FL.Y + (TC.Y - FL.Y) / 2 - 10;
					strInfo = "CF=" + System.Math.Round((FL.Y - TC.Y) * 12 / dYscale, 0).ToString();
					g2d.DrawString(strInfo, font2a, Brushes.Red, pntCF);

					//PL
					pntPL.X = PL.X - 5;
					pntPL.Y = 30;
					g2d.DrawString("P", font2a, Brushes.Red, pntPL);
					pntPL.X = PL.X - 3;
					pntPL.Y = 33;
					g2d.DrawString("L", font2a, Brushes.Red, pntPL);

					//DESIGN REF & STATION
					pntDR.X = RF.X + 5;
					pntDR.Y = 15;
					strInfo = string.Format("REF : {0:##+##.00}", vSDS.STA);
					if (vSDS.RESOLVED == true) {
						strInfo = strInfo + " -RESOLVED";
					} else {
						strInfo = strInfo + " - NOT RESOLVED";
					}
					g2d.DrawString(strInfo, font2a, Brushes.Red, pntDR);

					//X1
					pntDR.X = RF.X + 5;
					pntDR.Y = 220;
					strInfo = "X1=" + (System.Math.Round(vSDS.X1, 3)).ToString();
					g2d.DrawString(strInfo, font2a, Brushes.Red, pntDR);

					//X2
					pntDR.X = RF.X + 5;
					pntDR.Y = 235;
					strInfo = "X2=" + (System.Math.Round(vSDS.X2, 3)).ToString();
					g2d.DrawString(strInfo, font2a, Brushes.Red, pntDR);

					//SG
					pntDR.X = RF.X + 5;
					pntDR.Y = 250;
					strInfo = "SG=" + (System.Math.Round(vSDS.SG, 3)).ToString();
					g2d.DrawString(strInfo, font2a, Brushes.Red, pntDR);

					//XT
					pntDR.X = RF.X + 5;
					pntDR.Y = 265;
					strInfo = "XT=" + (System.Math.Round(vSDS.XT, 3)).ToString();
					g2d.DrawString(strInfo, font2a, Brushes.Red, pntDR);

					//dYh
					pntDR.X = RF.X + 5;
					pntDR.Y = 280;
					strInfo = "dYh=" + (System.Math.Round(vSDS.dYh, 3)).ToString();
					g2d.DrawString(strInfo, font2a, Brushes.Red, pntDR);

					//TC
					pntDR.X = RF.X + 5;
					pntDR.Y = 295;
					strInfo = "TC=" + (System.Math.Round(vSDS.TC.ELEV, 3)).ToString();
					g2d.DrawString(strInfo, font2a, Brushes.Red, pntDR);

					//PL
					pntDR.X = RF.X + 5;
					pntDR.Y = 310;
					strInfo = "PL=" + (System.Math.Round(vSDS.PL.ELEV, 3)).ToString();
					g2d.DrawString(strInfo, font2a, Brushes.Red, pntDR);

					//---------------------------------------------------------------
					//RF - PL
					labelDistance(g2d, pen8, font2a, RF, PL, XTot, 340f);

					//---------------------------------------------------------------

					//DRAW MARKERS
					drawMarker(g2d, pen1, RF, TC, vSDS.TC.OFFSET, 0);
					drawMarker(g2d, pen2, RF, BB1, vSDS.BB1.OFFSET, 0);
					drawMarker(g2d, pen4, RF, TOE, vSDS.TOE.OFFSET, 0);
					drawMarker(g2d, pen6, RF, TOP, vSDS.TOP.OFFSET, 0);

					//--------------------------------------------------------------------------------------------------------------

					//EXIST GROUND
					pntsEX[0].X = RF.X + vExGround.RF.Offset * dXscale;
					pntsEX[0].Y = RF.Y - (vExGround.RF.Elev - vSDS.RF.ELEV) * dYscale;

					pntsEX[1].X = RF.X + vExGround.TC.Offset * dXscale;
					pntsEX[1].Y = RF.Y - (vExGround.TC.Elev - vSDS.RF.ELEV) * dYscale;

					pntsEX[2].X = RF.X + vExGround.TOE.Offset * dXscale;
					pntsEX[2].Y = RF.Y - (vExGround.TOE.Elev - vSDS.RF.ELEV) * dYscale;

					pntsEX[3].X = RF.X + vExGround.TOP.Offset * dXscale;
					pntsEX[3].Y = RF.Y - (vExGround.TOP.Elev - vSDS.RF.ELEV) * dYscale;

					pntsEX[4].X = RF.X + vExGround.PL.Offset * dXscale;
					pntsEX[4].Y = RF.Y - (vExGround.PL.Elev - vSDS.RF.ELEV) * dYscale;

					pntsEX[5].X = RF.X + vExGround.Off5.Offset * dXscale;
					pntsEX[5].Y = RF.Y - (vExGround.Off5.Elev - vSDS.RF.ELEV) * dYscale;

					g2d.DrawLines(penDashedEX, pntsEX);

					g2d.Dispose();

				}


			} catch (Autodesk.AutoCAD.Runtime.Exception ) {
				MessageBox.Show("Error in UpdataSection_2d");

			}
		   
		}

		public static void
		labelDistance(Graphics g, Pen pen, System.Drawing.Font fontx, PointF pnt1, PointF pnt2, float dist, float y){

			try {
				PointF pntB = new PointF(pnt1.X, y);
				PointF pntE = new PointF((pnt2.X - pnt1.X) / 2 - 20, y);

				PointF[] Pnts = new PointF[2]{
					pntB,
					pntE
				};

				g.DrawLines(pen, Pnts);

				PointF pntX = new PointF(pntE.X + 5, y - 5);
				g.DrawString(string.Format("{0:F2}",dist), fontx, Brushes.Red, pntX);

				pntB = new PointF(pntE.X + 35, y);
				pntE = new PointF(pnt2.X, y);

				Pnts = new PointF[2]{
					pntB,
					pntE
				};
				g.DrawLines(pen, Pnts);


			} catch (Autodesk.AutoCAD.Runtime.Exception ) {
			}
		}

		public static void
		drawMarker(Graphics g2D, Pen penX, PointF pnt0, PointF pntX, float dblOffset, float dblOffsetV){

			//DRAW VERTICAL MARKER
			PointF pnt1 = new PointF(pntX.X, pntX.Y - 5);
			PointF pnt2 = new PointF(pntX.X, pnt1.Y - 50);
			PointF[] Pnts = new PointF[2]{
				pnt1,
				pnt2
			};
			g2D.DrawLines(penX, Pnts);	//RF - TOP
			labelDistance(g2D, pen8, font2a, pnt0, pntX, dblOffset, pnt1.Y - 30 + dblOffsetV);
		}

		public static void
		labelX0S0(string strX, string strS, Graphics g, System.Drawing.Font fontX, PointF pnt1, PointF pnt2){

			PointF pntX = new PointF(pnt1.X + (pnt2.X - pnt1.X) / 2 - 5, pnt1.Y + (pnt2.Y - pnt1.Y) / 2 - 12);
			PointF pntS = new PointF(pntX.X, pntX.Y + 12);

			g.DrawString(strX, fontX, Brushes.Red, pntX);
			g.DrawString(strS, fontX, Brushes.Red, pntS);

		}

		public static void
		labelMISC(string strX, Graphics g, System.Drawing.Font fontX, PointF pnt1, PointF pnt2, float dx, float dy){

			PointF pntX = new PointF(pnt1.X + (pnt2.X - pnt1.X) / 2 + dx, pnt1.Y + (pnt2.Y - pnt1.Y) / 2 + 10 + dy);
			g.DrawString(strX, fontX, Brushes.Red, pntX);

		}

		public static void
		labelSLOPE(string strX, Graphics g, System.Drawing.Font fontX, PointF pnt1, PointF pnt2, bool boolUP){

			PointF pntM = new PointF(pnt1.X + (pnt2.X - pnt1.X) / 2, pnt1.Y + (pnt2.Y - pnt1.Y) / 2);

			if (boolUP)
			{
				PointF pntX = new PointF(pntM.X + 15, pntM.Y + 10);
				g.DrawString(strX, fontX, Brushes.Red, pntX);

				pntX = new PointF(pntM.X - 5, pntM.Y + 30);
				g.DrawString("1", fontX, Brushes.Black, pntX);

				PointF pntX1 = new PointF(pntM.X - 10, pntM.Y + 25);
				PointF pntX2 = new PointF(pntM.X + 10, pntM.Y + 25);
				PointF pntX3 = new PointF(pntM.X + 10, pntM.Y + 5);

				PointF[] pts = new PointF[3] {
					pntX1,
					pntX2,
					pntX3
				};
				g.DrawLines(penBlack2, pts);
			}
			else
			{
				PointF pntX = new PointF(pntM.X - 25, pntM.Y + 10);
				g.DrawString(strX, fontX, Brushes.Red, pntX);

				pntX = new PointF(pntM.X - 5, pntM.Y + 30);
				g.DrawString("1", fontX, Brushes.Black, pntX);

				PointF pntX1 = new PointF(pntM.X - 10, pntM.Y + 5);
				PointF pntX2 = new PointF(pntM.X - 10, pntM.Y + 25);
				PointF pntX3 = new PointF(pntM.X + 10, pntM.Y + 25);

				PointF[] pts = new PointF[3] {
					pntX1,
					pntX2,
					pntX3
				};
				g.DrawLines(penBlack2, pts);

			}
		}
		public static void
		UpdateSectionTypical_2D(){

			EXGROUND vExGrnd = default(EXGROUND);
			vExGrnd = new EXGROUND();

			vExGrnd.RF.Offset = 0.0f;
			vExGrnd.RF.Elev = 998.0f;
			vExGrnd.TC.Offset = float.Parse(fWall1.tbx2d_X0.Text);
			vExGrnd.TC.Elev = 1004.0f;
			vExGrnd.TOE.Offset = vExGrnd.TC.Offset + float.Parse(fWall1.tbx2d_B1.Text) + float.Parse(fWall1.tbx2d_WT.Text) / 12 + float.Parse(fWall1.tbx2d_GW.Text) + float.Parse(fWall1.tbx2d_B2.Text);
			vExGrnd.TOE.Elev = 1007.0f;
			vExGrnd.TOP.Offset = vExGrnd.TOE.Offset + 4;
			//wall dx
			vExGrnd.TOP.Elev = 1008.0f;
			vExGrnd.PL.Offset = vExGrnd.TOP.Offset + float.Parse(fWall1.tbx2d_B3.Text);
			vExGrnd.PL.Elev = 1004.48f;
			vExGrnd.Off5.Offset = vExGrnd.PL.Offset + 5;
			//off5 dx
			vExGrnd.Off5.Elev = 1008.0f;

			SECTIONDATASET vSDS = default(SECTIONDATASET);
			vSDS = new SECTIONDATASET();

			vSDS.dYh = 2.0f;
			vSDS.SG = float.Parse(fWall1.tbx2d_SG.Text);

			vSDS.RF.OFFSET = 0f;
			vSDS.RF.ELEV = 1000.0f;

			vSDS.FL.OFFSET = float.Parse(fWall1.tbx2d_X0.Text) - 0.12f;
			vSDS.FL.ELEV = vSDS.RF.ELEV + vSDS.FL.OFFSET * float.Parse(fWall1.tbx2d_S0.Text);

			vSDS.TC.OFFSET = float.Parse(fWall1.tbx2d_X0.Text);
			vSDS.TC.ELEV = vSDS.FL.ELEV + float.Parse(fWall1.tbx2d_CF.Text) / 12;

			vSDS.W0.OFFSET = vSDS.TC.OFFSET + float.Parse(fWall1.tbx2d_B1.Text);
			vSDS.W0.ELEV = vSDS.TC.ELEV + float.Parse(fWall1.tbx2d_B1.Text) * float.Parse(fWall1.tbx2d_S1.Text);

			vSDS.W1.OFFSET = vSDS.W0.OFFSET;
			vSDS.W1.ELEV = vSDS.W0.ELEV + vSDS.dYh;

			vSDS.W2.OFFSET = vSDS.W1.OFFSET + float.Parse(fWall1.tbx2d_WT.Text) / 12;
			vSDS.W2.ELEV = vSDS.W1.ELEV;

			vSDS.W3.OFFSET = vSDS.W2.OFFSET;
			vSDS.W3.ELEV = vSDS.W2.ELEV - float.Parse(fWall1.tbx2d_FB.Text) / 12;

			vSDS.G0.OFFSET = vSDS.W3.OFFSET + float.Parse(fWall1.tbx2d_GW.Text) / 2;
			vSDS.G0.ELEV = vSDS.W3.ELEV - 0.167f;

			vSDS.GB.OFFSET = vSDS.W3.OFFSET + float.Parse(fWall1.tbx2d_GW.Text);
			vSDS.GB.ELEV = vSDS.W3.ELEV;

			vSDS.TOE.OFFSET = vSDS.GB.OFFSET + float.Parse(fWall1.tbx2d_B2.Text);
			vSDS.TOE.ELEV = vSDS.W3.ELEV + float.Parse(fWall1.tbx2d_B2.Text) * float.Parse(fWall1.tbx2d_S2.Text);

			vSDS.TOP.OFFSET = vSDS.TOE.OFFSET + vSDS.dYh / float.Parse(fWall1.tbx2d_SG.Text);
			vSDS.TOP.ELEV = vSDS.TOE.ELEV + vSDS.dYh;

			vSDS.PL.OFFSET = vSDS.TOP.OFFSET + float.Parse(fWall1.tbx2d_B3.Text);
			vSDS.PL.ELEV = vSDS.TOP.ELEV + float.Parse(fWall1.tbx2d_B3.Text) * float.Parse(fWall1.tbx2d_S3.Text);

			vSDS.W4.OFFSET = vSDS.W3.OFFSET;
			vSDS.W4.ELEV = vSDS.W0.ELEV - 1;

			vSDS.W5.OFFSET = vSDS.W4.OFFSET + 1;
			vSDS.W5.ELEV = vSDS.W4.ELEV;

			vSDS.W6.OFFSET = vSDS.W5.OFFSET;
			vSDS.W6.ELEV = vSDS.W5.ELEV - 1;

			vSDS.W7.OFFSET = vSDS.W0.OFFSET - 3;
			vSDS.W7.ELEV = vSDS.W6.ELEV;

			vSDS.W8.OFFSET = vSDS.W7.OFFSET;
			vSDS.W8.ELEV = vSDS.W7.ELEV + 1;

			vSDS.W9.OFFSET = vSDS.W0.OFFSET;
			vSDS.W9.ELEV = vSDS.W4.ELEV;

			vSDS.X = vSDS.PL.OFFSET - vSDS.RF.OFFSET;

			vSDS.EX = vExGrnd;
			vSDS.RESOLVED = true;

			fWall1.SDS = vSDS;
			UpdateSection_2d(vSDS);

		}

		public static void
		initializeViewSections(){

			p.dNOM.Add("RF", new Dictionary<float, FEATUREDATA>());
			p.dNOM.Add("FL", new Dictionary<float, FEATUREDATA>());
			p.dNOM.Add("TC", new Dictionary<float, FEATUREDATA>());
			p.dNOM.Add("BB1", new Dictionary<float, FEATUREDATA>());
			p.dNOM.Add("TOE", new Dictionary<float, FEATUREDATA>());
			p.dNOM.Add("TOP", new Dictionary<float, FEATUREDATA>());
			p.dNOM.Add("PL", new Dictionary<float, FEATUREDATA>());
			
			p.dWALL.Add("W0", new Dictionary<float, FEATUREDATA>());
			p.dWALL.Add("W1", new Dictionary<float, FEATUREDATA>());
			p.dWALL.Add("W2", new Dictionary<float, FEATUREDATA>());
			p.dWALL.Add("W3", new Dictionary<float, FEATUREDATA>());
			p.dWALL.Add("W4", new Dictionary<float, FEATUREDATA>());
			p.dWALL.Add("W5", new Dictionary<float, FEATUREDATA>());
			p.dWALL.Add("W6", new Dictionary<float, FEATUREDATA>());
			p.dWALL.Add("W7", new Dictionary<float, FEATUREDATA>());
			p.dWALL.Add("W8", new Dictionary<float, FEATUREDATA>());
			p.dWALL.Add("W9", new Dictionary<float, FEATUREDATA>());
			
			p.dGUT.Add("GB", new Dictionary<float, FEATUREDATA>());
			p.dGUT.Add("G0", new Dictionary<float, FEATUREDATA>());
			p.dGUT.Add("W3", new Dictionary<float, FEATUREDATA>());

			fWall1.WALLNO = -1;

			fWall1.NumUpDownSection.Minimum = 1;
			fWall1.NumUpDownSection.Maximum = fWall1.Stations.Count;
			//+1 offset with Stations()
			fWall1.NumUpDownSection.Increment = 1;

			fWall1.NumUpDownInterval.Minimum = 1;
			fWall1.NumUpDownInterval.Maximum = 100;
			fWall1.NumUpDownInterval.Increment = 1;

			fWall1.lblTotal.Text = fWall1.Stations.Count.ToString();

			fWall1.tbxStation.Text = string.Format("{0:###+##.##}", fWall1.Stations[(int)fWall1.NumUpDownSection.Value - 1]);
			fWall1.CurrentStationIndex = (int)fWall1.NumUpDownSection.Value - 1;
		}

		public static void
		buildProfileForAlignRFandAlignPL(){

			Alignment objAlignPL = null;
			Alignment objAlignRF = null;
			
			ObjectId idAlignPL = ObjectId.Null;
			ObjectId idAlignRF = ObjectId.Null;

			if (fWall1.opt1_SurfaceExist.Checked == true)
			{
				idAlignPL = wdp.CreateProfileBySurface("EXIST", fWall1.ACTIVEALIGN.ObjectId, 0.0);
				//m_WallDesignProfile


			}
			else
			{
				wdp.CreateProfileByLayout("EXIST", fWall1.ACTIVEALIGN, fWall1.PNTSEXIST);
				//m_WallDesignProfile

			}


			if (fWall1.opt1_SurfaceDesign.Checked == true)
			{
				idAlignRF = wdp.CreateProfileBySurface("CPNT", fWall1.ACTIVEALIGN.ObjectId, float.Parse(fWall1.tbx1_Offset.Text));
				//m_WallDesignProfile
				wdp.CreateProfileByDesign2c(fWall1.ACTIVEALIGN, objAlignRF, fWall1.PNTSDESIGN);
				//m_WallDesignProfile


			}
			else if (fWall1.opt1_PointsDesign.Checked == true)
			{
				wdp.CreateProfileByDesign2c(fWall1.ACTIVEALIGN, objAlignPL, fWall1.PNTSDESIGN);
				//m_WallDesignProfile

			}
			else if (fWall1.opt1_3dPolyDesign.Checked == true)
			{
				string handle = fWall1.BRKLINE_DESIGN_HANDLE;
				ObjectId idPoly3d = handle.stringToHandle().getObjectId();

				string strNameAlign = string.Format("{0}-REF", fWall1.ACTIVEALIGN.Name);
				string strLayer = strNameAlign;

				ObjectId idLayer = Layer.manageLayers(strLayer);

				try
				{
					idAlignRF = wa2.Create_Align_Profile_By3dPoly2b2c(fWall1.ACTIVEALIGN, "CPNT", strNameAlign, idLayer, idPoly3d); //m_WallAlignment2
					
					fWall1.AlignRF = (Alignment)idAlignRF.getEnt();
					fWall1.AlignPL = fWall1.ACTIVEALIGN;
					wa2d.getStationsToSample(idAlignPL, idAlignRF);
					//m_WallAlignment2


				}
				catch (Autodesk.AutoCAD.Runtime.Exception )
				{
					MessageBox.Show("Error in 2d");
					return;

				}
			}
		}
	}
}
