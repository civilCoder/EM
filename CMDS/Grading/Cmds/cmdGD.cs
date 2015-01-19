using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.Diagnostics;
using Point3d = Autodesk.AutoCAD.Geometry.Point3d;

namespace Grading.Cmds
{
	public static class cmdGD
	{
		const double pi = System.Math.PI;

		public static void
		GD()
		{
			object snapMode = SnapMode.getOSnap();
			try
			{
				ObjectId idPoly = ObjectId.Null;

                double dblAngDock = 0, dblAng = 0; ;
				double dblLenDock = 0;
	
				Point3d pnt3dA = Pub.pnt3dO;           //point AHEAD
	
				List<Point3d> pnts3dLim = new List<Point3d>();

				ObjectId idCgPnt0 = ObjectId.Null, idCgPnt1 = ObjectId.Null, idCgPntBeg = ObjectId.Null, idCgPntEnd = ObjectId.Null;
				string elev = UserInput.getCogoPoint("\nSelect Dock Begin Point: ", out idCgPntBeg, ObjectId.Null, osMode: 8);
				if (idCgPntBeg == ObjectId.Null)
					return;
				Debug.Print(idCgPntBeg.getHandle().ToString());

				Point3d pnt3dBeg = idCgPntBeg.getCogoPntCoordinates();
				pnts3dLim.Add(pnt3dBeg);
				
				
				elev = UserInput.getCogoPoint("\nSelect Dock End Point", out idCgPntEnd, idCgPntBeg, osMode: 8);
				if (idCgPntEnd == ObjectId.Null)
					return;
				Debug.Print(idCgPntEnd.getHandle().ToString());

				Point3d pnt3dEnd = idCgPntEnd.getCogoPntCoordinates();
				pnts3dLim.Add(pnt3dEnd);

				bool escape = false;
				PromptStatus ps;
				Point3d pnt3dX = UserInput.getPoint("\nSpecify point on side to grade dock: ", pnt3dBeg, out escape, out ps, osMode: 0);
				if (pnt3dX == Pub.pnt3dO || escape)
				{
					return;
				}
				int side = 0;
				if (Geom.testRight(pnt3dBeg, pnt3dEnd, pnt3dX) > 0)
					side = -1;
				else
					side = 1;	
			
				double width = 60;
				escape = UserInput.getUserInput(string.Format("\nEnter dock width: <{0}>:", width), out width, width);
				if (escape)
					return;
	
				using (BaseObjs._acadDoc.LockDocument())
				{
					try
					{
						using (Transaction tr = BaseObjs.startTransactionDb())
						{
		
							dblAngDock = pnt3dBeg.getDirection(pnt3dEnd);
							dblLenDock = pnt3dBeg.getDistance(pnt3dEnd);

							double dblSlope = System.Math.Round(pnt3dBeg.getSlope(pnt3dEnd), 3);
		
							dblAng = dblAngDock - pi / 2 * side;

                            ObjectIdCollection idsPoly3dX = new ObjectIdCollection();
							List<ObjectId> idsCgPnts = new List<ObjectId>();
	
							List<Point3d> pnts3d = new List<Point3d>();
							pnts3d.Add(pnt3dBeg);               //CgPntBeg

                            //Slope is not 0
							if (dblSlope != 0)
							{
								pnt3dA = pnt3dBeg.traverse(dblAng, width, -0.01);
								pnts3d.Add(pnt3dA);             //Pnt 2
	
								dblAng = dblAng + pi / 2 * side;
								pnt3dA = pnt3dA.traverse(dblAng, dblLenDock, dblSlope);
								pnts3d.Add(pnt3dA);             //Pnt3
	
								pnts3d.Add(pnt3dEnd);           //Pnt4 - CgPntEnd	
								pnts3d.Add(pnt3dBeg);           //Pnt5 - CgPntBeg
	
								idsCgPnts = pnts3d.addBrklineSegmentsDock(out idsPoly3dX, apps.lnkBrks, side, idCgPntBeg, idCgPntEnd);
							
							}else{
                                int k = 1;

								int intDivide = (int)System.Math.Truncate(dblLenDock / 84) + 1;
	
								if (intDivide % 2 != 0)
								{
									intDivide = intDivide + 1;
								}
	
								pnt3dA = pnt3dBeg.traverse(dblAng, width, -0.005);	//Pnt2
								pnts3d.Add(pnt3dA);
                                k++;
	
								int x = 0;
								dblAng = dblAng + pi / 2 * side;

								double seg = dblLenDock / intDivide;
								int updown = 1;

								for (x = 0; x <= intDivide - 1; x++)
								{
									updown = -updown;
									pnt3dA = pnt3dA.traverse(dblAng, seg, 0.005 * updown);
									pnts3d.Add(pnt3dA);             //points on outer edge of Dock from Pnt2 through Pnt3 
                                    k++;
								}
	
								pnts3d.Add(pnt3dEnd);   //Pnt4 = CgPntEnd
                                int indexPntEnd = k++;

								pnt3dA = pnt3dEnd;
	
								dblAng = dblAng + pi * side;        //+pi because skipped turning towards bldg because we have point already
	
								for (x = 0; x <= intDivide - 1; x++)
								{
									pnt3dA = pnt3dA.traverse(dblAng, dblLenDock / intDivide, 0.0);
									pnts3d.Add(pnt3dA);            //points on inner edge of Dock from pntBeg to pntEnd
								}

                                // don't need to add pnt3dBeg to end of list because it is calc'd in loop previous

								idsCgPnts = pnts3d.addBrklineSegmentsDock(out idsPoly3dX, apps.lnkBrks, side, idCgPntBeg, idCgPntEnd, indexPntEnd);
	
								List<ObjectId> idCgPntsX = new List<ObjectId>();
								idCgPntsX.Add(idsCgPnts[1]);     //CgPnt 2 at dock limit away from building
	
								int intUBnd = idsCgPnts.Count;
								x = -1;
								int n = 1;
								k = intUBnd / 2;
								for (int j = 1; j <= k - 1; j++)
								{
									x = -x;
									n = n + (intUBnd - 2 * j) * x;
									System.Diagnostics.Debug.Print(string.Format("{0},{1}", j, n));
									idCgPntsX.Add(idsCgPnts[n]);
								}
                                
                                ObjectId idPoly3d = ObjectId.Null;
								for (int i = 1; i < idCgPntsX.Count; i++)
								{
									idCgPnt0 = idCgPntsX[i - 1];
									idCgPnt1 = idCgPntsX[i];

									idPoly3d = Draw.addPoly3d(idCgPnt0.getCogoPntCoordinates(), idCgPnt1.getCogoPntCoordinates(), "CPNT-BRKLINE");

									idPoly3d.lnkPntsAndPoly3d(idCgPnt0, idCgPnt1, apps.lnkBrks);

									idsPoly3dX.Add(idPoly3d);
								}
							}
							Grading_Floor.modSurface("CPNT-ON", "Finish Surface", idsPoly3dX, false);
							//}
							tr.Commit();
						}
					}
					catch (System.Exception ex)
					{
						BaseObjs.writeDebug(string.Format("{0} cmdGD.cs: line: 181", ex.Message));
					}
				}
			}
			catch (System.Exception )
			{
				
			}
			finally{
				SnapMode.setOSnap((int)snapMode);
			}
		}

        public static List<ObjectId>
        addBrklineSegmentsDock(this List<Point3d> pnts3d, out ObjectIdCollection idsPoly3d, string nameApp, int side,
                               ObjectId idCgPntBeg, ObjectId idCgPntEnd, int indexPntEnd = 0)
        {
            idsPoly3d = new ObjectIdCollection();
            uint pntNum = 0;

            List<ObjectId> idCgPntList = new List<ObjectId>();
            ObjectId idCgPnt0 = ObjectId.Null, idCgPnt1 = ObjectId.Null, idPoly3d = ObjectId.Null;

            int n = pnts3d.Count;
            for (int i = 1; i < n; i++)
            {
                if (i == 1)
                { //first segment

                    idCgPnt0 = idCgPntBeg;
                    idCgPntList.Add(idCgPnt0);           //overall list

                    idCgPnt1 = pnts3d[1].setPoint(out pntNum);
                    idCgPntList.Add(idCgPnt1);

                    idPoly3d = Draw.addPoly3d(idCgPnt0, idCgPnt1, nameApp, "CPNT-BRKLINE");
                    idsPoly3d.Add(idPoly3d);
                    BaseObjs.updateGraphics();
                }
                else if (i == n - 1)        //last vertex
                {
                    if (indexPntEnd == 0){
                        idCgPnt0 = idCgPntEnd;
                        idCgPntList.Add(idCgPnt0);                        
                    }
                    else
                        idCgPnt0 = idCgPntList[i - 1];

                    idCgPnt1 = idCgPntBeg;

                    idPoly3d = Draw.addPoly3d(idCgPnt0, idCgPnt1, nameApp, "CPNT-BRKLINE");
                    idsPoly3d.Add(idPoly3d);
                    BaseObjs.updateGraphics();
                }
                else
                {
                    if (i == indexPntEnd || i == indexPntEnd + 1){
                        if(i == indexPntEnd)                    {
                            idCgPnt0 = idCgPntList[i - 1];

                            idCgPnt1 = idCgPntEnd;
                            idCgPntList.Add(idCgPnt1);

                        }else if (i == indexPntEnd + 1){

                            idCgPnt0 = idCgPntEnd;
                            idCgPntList.Add(idCgPnt0);

                            idCgPnt1 = pnts3d[i].setPoint(out pntNum);
                            idCgPntList.Add(idCgPnt1);
                        }                                               
                    }else{

                        idCgPnt0 = idCgPntList[i - 1];
                    
                        idCgPnt1 = pnts3d[i].setPoint(out pntNum);                   
                        idCgPntList.Add(idCgPnt1);                                            
                    }

                    idPoly3d = Draw.addPoly3d(idCgPnt0, idCgPnt1, nameApp, "CPNT-BRKLINE");
                    idsPoly3d.Add(idPoly3d);
                    BaseObjs.updateGraphics();
                }
            }
            return idCgPntList;
        }

	}
}