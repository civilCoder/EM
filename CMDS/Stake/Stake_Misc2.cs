using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using System;
using System.Collections.Generic;
using System.Linq;
using DBObject = Autodesk.AutoCAD.DatabaseServices.DBObject;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Stake
{
    public static class Stake_Misc2
    {
        private static double PI = System.Math.PI;
        private static Forms.frmStake fStake = Forms.Stake_Forms.sForms.fStake;
        private static Forms.frmMisc fMisc = Forms.Stake_Forms.sForms.fMisc;

        public static void
        stakeMISC(string strCommand)
        {
            TinSurface objSurface = fStake.SurfaceCPNT;

            string strLayer = fMisc.tbxLayer.Text;
            string strDesc = fMisc.tbxDesc0.Text + fMisc.tbxDescription.Text;
            PromptStatus ps;
            bool escape;
            switch (strCommand)
            {
                case "InLine_Center":

                    BaseObjs.acadActivate();

                    Point3d pnt3dTar = UserInput.getPoint("\nSelect Target Point for Staking: ", out ps, osMode: 8);

                    Point3d pnt3dPick = UserInput.getPoint("\nSelect Directional Point(any XY location): ", pnt3dTar, out escape, out ps, osMode: 8);

                    double dblAngDir1 = pnt3dTar.getDirection(pnt3dPick);
                    Point3d pnt3dPolar = pnt3dTar.traverse(dblAngDir1, double.Parse(fMisc.cbxOffset.Text));

                    double dblElev = objSurface.FindElevationAtXY(pnt3dTar.X, pnt3dTar.Y);
                    Point3d pnt3d = new Point3d(pnt3dPolar.X, pnt3dPolar.Y, dblElev);

                    Stake_Calc.setOffsetPointMISC(pnt3d, strLayer, strDesc);

                    pnt3dPolar = pnt3dTar.traverse(dblAngDir1 + PI, double.Parse(fMisc.cbxOffset.Text));

                    pnt3dPolar = pnt3dTar.traverse(dblAngDir1, double.Parse(fMisc.cbxOffset.Text));

                    Stake_Calc.setOffsetPointMISC(pnt3d, strLayer, strDesc);

                    break;

                case "InLine_Direction":

                    BaseObjs.acadActivate();

                    pnt3dTar = UserInput.getPoint("\nSelect Target Point for Staking: ", out ps, osMode: 8);

                    pnt3dPick = UserInput.getPoint("\nSelect Directional Point(any XY location): ", pnt3dTar, out escape, out ps, osMode: 8);

                    dblAngDir1 = pnt3dTar.getDirection(pnt3dPick);

                    pnt3dPolar = pnt3dTar.traverse(dblAngDir1, double.Parse(fMisc.cbxOffset.Text));

                    dblElev = objSurface.FindElevationAtXY(pnt3dTar.X, pnt3dTar.Y);
                    pnt3d = new Point3d(pnt3dPolar.X, pnt3dPolar.Y, dblElev);

                    Stake_Calc.setOffsetPointMISC(pnt3d, strLayer, strDesc);

                    if (double.Parse(fMisc.cbxOffsetDir.Text) == 0)
                    {
                        Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Direction Offset is 0 - revise....exiting");
                        return;
                    }

                    pnt3dPolar = pnt3dTar.traverse(dblAngDir1, double.Parse(fMisc.cbxOffset.Text) + double.Parse(fMisc.cbxOffsetDir.Text));

                    pnt3d = new Point3d(pnt3dPolar.X, pnt3dPolar.Y, dblElev);

                    strDesc = string.Format("{0}' O/S {1} @", double.Parse(fMisc.cbxOffset.Text) +
                                                              double.Parse(fMisc.cbxOffsetDir.Text), fMisc.cbxObjType.Text);

                    Stake_Calc.setOffsetPointMISC(pnt3d, strLayer, strDesc);

                    break;

                case "Proj2Curb":

                    BaseObjs.acadActivate();

                    pnt3dTar = UserInput.getPoint("\nSelect Target Point for Staking: ", out ps, osMode: 8);
                    string xRefPath = "";
                    Entity ent = xRef.getEntity("\nSelect Adjacent Curb:", out escape, out xRefPath);

                    Point3d pnt3dInt0 = getRadial_Perpendicular(pnt3dTar, ent);

                    ent.ObjectId.delete();

                    dblAngDir1 = pnt3dTar.getDirection(pnt3dInt0);

                    pnt3dPolar = pnt3dInt0.traverse(dblAngDir1 + PI / 2, double.Parse(fMisc.cbxOffset.Text));

                    dblElev = objSurface.FindElevationAtXY(pnt3dInt0.X, pnt3dInt0.Y);
                    dblElev = dblElev + double.Parse(fMisc.cbxCurbHeight.Text) / 12;
                    pnt3d = new Point3d(pnt3dPolar.X, pnt3dPolar.Y, dblElev);

                    Stake_Calc.setOffsetPointMISC(pnt3d, strLayer, strDesc);

                    pnt3dPolar = pnt3dInt0.traverse(dblAngDir1 - PI / 2, double.Parse(fMisc.cbxOffset.Text));

                    pnt3d = new Point3d(pnt3dPolar.X, pnt3dPolar.Y, dblElev);

                    Stake_Calc.setOffsetPointMISC(pnt3d, strLayer, strDesc);

                    break;

                case "Proj2Bldg":

                    BaseObjs.acadActivate();

                    pnt3dTar = UserInput.getPoint("\nSelect Target Point for Staking: ", out ps, osMode: 8);
                    Point3d pnt3dRef1 = UserInput.getPoint("Pick Point Perpendicular to Building: ", pnt3dTar, out escape, out ps, osMode: 8);
                    dblAngDir1 = pnt3dTar.getDirection(pnt3dRef1);

                    pnt3dPolar = pnt3dRef1.traverse(dblAngDir1 + PI / 2, double.Parse(fMisc.cbxOffset.Text));

                    dblElev = objSurface.FindElevationAtXY(pnt3dRef1.X, pnt3dRef1.Y);
                    pnt3d = new Point3d(pnt3dPolar.X, pnt3dPolar.Y, dblElev);
                    Stake_Calc.setOffsetPointMISC(pnt3d, strLayer, strDesc);

                    pnt3dPolar = pnt3dRef1.traverse(dblAngDir1 - PI / 2, double.Parse(fMisc.cbxOffset.Text));

                    pnt3d = new Point3d(pnt3dPolar.X, pnt3dPolar.Y, dblElev);
                    Stake_Calc.setOffsetPointMISC(pnt3d, strLayer, strDesc);

                    break;

                case "AP":

                    BaseObjs.acadActivate();

                    pnt3dTar = UserInput.getPoint("\nSelect Target Point for Staking: ", out ps, osMode: 8);

                    pnt3dRef1 = UserInput.getPoint("\nSelect Adjacent Angle Point1: ", out ps, osMode: 1);
                    Point3d pnt3dRef2 = UserInput.getPoint("\nSelect Adjacent Angle Point2: ", out ps, osMode: 1);
                    Point3d varPntPick = UserInput.getPoint("\nSelect Side: ", out ps, osMode: 0);

                    double dblAngDelta = Geom.getAngle3Points(pnt3dRef1, pnt3dTar, pnt3dRef2);

                    dblAngDir1 = pnt3dRef1.getDirection(pnt3dTar);
                    double dblAngDir2 = pnt3dTar.getDirection(pnt3dRef2);

                    bool isRightHand = (pnt3dTar.isRightSide(pnt3dRef1, pnt3dRef2)) ? true : false;

                    pnt3dPolar = varPntPick.traverse(dblAngDir1 + PI / 2, 10);

                    List<Point3d> pnts3d1 = new List<Point3d> { pnt3dRef1, pnt3dTar };
                    List<Point3d> pnts3d2 = new List<Point3d> { varPntPick, pnt3dPolar };

                    List<Point3d> pnts3dInt = pnts3d1.intersectWith(pnts3d2, false, extend.both);

                    if (pnts3dInt.Count == 0)
                        return;
                    bool boolHit = false, boolIN = false;
                    int intSide = 0;

                    //PICK POINT ADJACENT TO FIRST SEGMENT
                    if (pnt3dTar.isRightSide(pnt3dRef1, varPntPick))
                    {
                        //CCW
                        if (isRightHand)
                        {
                            boolIN = true;
                            intSide = -1;
                            //CW
                        }
                        else
                        {
                            boolIN = false;
                            intSide = -1;
                        }
                        boolHit = true;
                    }
                    else
                    { //RIGHT SIDE OF FIRST SEGMENT
                        //CCW
                        if (isRightHand)
                        {
                            boolIN = false;
                            intSide = 1;
                        }
                        else
                        {
                            boolIN = true;
                            //CW
                            intSide = 1;
                        }
                        boolHit = true;
                    }

                    if (!boolHit)
                    {
                        pnt3dPolar = varPntPick.traverse(dblAngDir2 + PI / 2, 10);
                        pnts3d1 = new List<Point3d> { pnt3dRef2, pnt3dTar };
                        pnts3d2 = new List<Point3d> { varPntPick, pnt3dPolar };

                        List<Point3d> pnts3dInt2 = pnts3d1.intersectWith(pnts3d2, false, extend.both);

                        //PICK POINT ADJACENT TO SECOND SEGMENT

                        //LEFT SIDE OF SECOND SEGMENT
                        if (pnt3dRef2.isRightSide(pnt3dTar, varPntPick))
                        {
                            //CCW
                            if (isRightHand)
                            {
                                boolIN = true;
                                intSide = -1;
                                //CW
                            }
                            else
                            {
                                boolIN = false;
                                intSide = -1;
                            }
                            //RIGHT SIDE OF SECOND SEGMENT
                        }
                        else
                        {
                            //CCW
                            if (isRightHand)
                            {
                                boolIN = false;
                                intSide = 1;
                                //CW
                            }
                            else
                            {
                                boolIN = true;
                                intSide = 1;
                            }
                        }
                    }

                    if (boolIN)
                    {
                        pnt3dPolar = pnt3dTar.traverse(dblAngDir2 + dblAngDelta / 2 * intSide * -1, Convert.ToDouble(fMisc.cbxOffset.Text) / System.Math.Sin(dblAngDelta / 2));
                        dblElev = objSurface.FindElevationAtXY(pnt3dTar.X, pnt3dTar.Y);
                        pnt3d = new Point3d(pnt3dPolar.X, pnt3dPolar.Y, dblElev);
                        Stake_Calc.setOffsetPointMISC(pnt3d, strLayer, strDesc);
                    }
                    else
                    {
                        pnt3dPolar = pnt3dTar.traverse(dblAngDir2 + ((3 * PI / 2 - dblAngDelta) * intSide * -1), Convert.ToDouble((fMisc.cbxOffset).Text));
                        //point opposite begin point of next segment
                        dblElev = objSurface.FindElevationAtXY(pnt3dTar.X, pnt3dTar.Y);

                        pnt3d = new Point3d(pnt3dPolar.X, pnt3dPolar.Y, dblElev);
                        Stake_Calc.setOffsetPointMISC(pnt3d, strLayer, strDesc);

                        pnt3dPolar = pnt3dTar.traverse(dblAngDir2 + (PI / 2 * intSide * -1), Convert.ToDouble((fMisc.cbxOffset).Text));
                        //point opposite begin point of next segment

                        pnt3d = new Point3d(pnt3dPolar.X, pnt3dPolar.Y, dblElev);
                        Stake_Calc.setOffsetPointMISC(pnt3d, strLayer, strDesc);
                    }

                    break;
            }
        }

        public static Point3d
        getRadial_Perpendicular(Point3d pnt3dTar, Entity obj)
        {
            double dblAng = 0, dblLC = 0, dblM = 0;
            Point3d pnt3dPolar = Pub.pnt3dO, pnt3dInt = Pub.pnt3dO;
            Point3dCollection varPntInt = new Point3dCollection();
            Line objLineDir = null;
            ObjectId idLineDir = ObjectId.Null;
            List<ObjectId> idList = new List<ObjectId>();
            switch (obj.GetType().ToString())
            {
                case "Arc":

                    Arc objArc = (Arc)obj;

                    Point3d pnt3dCen = objArc.Center;

                    if (System.Math.Round(pnt3dCen.getDistance(pnt3dTar), 1) != 0)
                    {
                        idLineDir = Draw.addLine(pnt3dCen, pnt3dTar);
                        objLineDir = (Line)idLineDir.getEnt();
                        objLineDir.IntersectWith(objArc, Intersect.ExtendThis, varPntInt, IntPtr.Zero, IntPtr.Zero);
                        idLineDir.delete();

                        pnt3dInt = varPntInt[0];
                    }
                    else
                    {
                        Point3d pnt3dBeg = objArc.StartPoint;
                        Point3d pnt3dEnd = objArc.EndPoint;
                        dblAng = pnt3dBeg.getDirection(pnt3dEnd);
                        dblLC = pnt3dBeg.getDistance(pnt3dEnd);
                        pnt3dPolar = pnt3dBeg.traverse(dblAng, dblLC / 2);
                        dblM = objArc.Radius * (1 - System.Math.Cos(objArc.TotalAngle / 2));

                        pnt3dInt = pnt3dPolar.traverse(dblAng - PI / 2, dblM);
                    }
                    break;

                case "Line":

                    Line objline = (Line)obj;
                    dblAng = objline.Angle + PI / 2;

                    pnt3dPolar = pnt3dTar.traverse(dblAng, 1);
                    idLineDir = Draw.addLine(pnt3dTar, pnt3dPolar);

                    objLineDir.IntersectWith(objline, Intersect.ExtendThis, varPntInt, IntPtr.Zero, IntPtr.Zero);
                    idLineDir.delete();

                    pnt3dInt = varPntInt[0];
                    break;

                case "Polyline2d":

                    Polyline2d objPoly2d = (Polyline2d)obj;

                    break;

                case "Polyline3d":

                    Polyline3d obj3dPoly = (Polyline3d)obj;

                    break;

                case "Polyline":

                    Polyline objLWPoly = (Polyline)obj;
                    ObjectId idPoly = objLWPoly.ObjectId;
                    idPoly.changeProp(nameLayer: "STAKE-TEMP");

                    DBObjectCollection varEnts = new DBObjectCollection();
                    objLWPoly.Explode(varEnts);
                    objLWPoly.ObjectId.delete();

                    Layer.manageLayer(fStake.CNTL_LAYER, layerFrozen: true);

                    Point3d varPntPik = fMisc.PICK_PNT;

                    TypedValue[] tvs = new TypedValue[5];
                    tvs.SetValue(new TypedValue((int)DxfCode.Operator, "<OR"), 0);
                    tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Arc)).DxfName), 1);
                    tvs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(Line)).DxfName), 2);
                    tvs.SetValue(new TypedValue((int)DxfCode.Operator, "OR>"), 3);
                    tvs.SetValue(new TypedValue((int)DxfCode.LayerName, "STAKE-TEMP"), 4);

                    Point3dCollection pnts3dBox = new Point3dCollection {
                        new Point3d(varPntPik.X - 1, varPntPik.Y - 1, 0),
                        new Point3d(varPntPik.X + 1, varPntPik.Y - 1, 0),
                        new Point3d(varPntPik.X + 1, varPntPik.Y + 1, 0),
                        new Point3d(varPntPik.X - 1, varPntPik.Y + 1, 0)
                    };

                    SelectionSet ss = Select.buildSSet(tvs, pnts3dBox);
                    ObjectId[] ids = ss.GetObjectIds();
                    for (int i = 0; i < ids.Length; i++)
                        idList.Add(ids[i]);

                    Layer.manageLayer(fStake.CNTL_LAYER, layerFrozen: false);

                    if (ids.Count() == 1)
                    {
                        obj = ids[0].getEnt();
                    }

                    switch (obj.GetType().ToString())
                    {
                        case "Arc":

                            objArc = (Arc)obj;

                            pnt3dCen = objArc.Center;

                            if (System.Math.Round(pnt3dCen.getDistance(pnt3dTar), 1) != 0)
                            {
                                idLineDir = Draw.addLine(pnt3dCen, pnt3dTar);
                                objLineDir = (Line)idLineDir.getEnt();
                                objLineDir.IntersectWith(objArc, Intersect.ExtendThis, varPntInt, IntPtr.Zero, IntPtr.Zero);
                                idLineDir.delete();

                                pnt3dInt = varPntInt[0];
                            }
                            else
                            {
                                Point3d pnt3dBeg = objArc.StartPoint;
                                Point3d pnt3dEnd = objArc.EndPoint;
                                dblAng = pnt3dBeg.getDirection(pnt3dEnd);
                                dblLC = pnt3dBeg.getDistance(pnt3dEnd);
                                pnt3dPolar = pnt3dBeg.traverse(dblAng, dblLC / 2);
                                dblM = objArc.Radius * (1 - System.Math.Cos(objArc.TotalAngle / 2));

                                pnt3dInt = pnt3dPolar.traverse(dblAng - PI / 2, dblM);
                            }
                            break;

                        case "Line":

                            objline = (Line)obj;
                            dblAng = objline.Angle + PI / 2;

                            pnt3dPolar = pnt3dTar.traverse(dblAng, 1);
                            idLineDir = Draw.addLine(pnt3dTar, pnt3dPolar);
                            objLineDir = (Line)idLineDir.getEnt();
                            objLineDir.IntersectWith(objline, Intersect.ExtendThis, varPntInt, IntPtr.Zero, IntPtr.Zero);
                            idLineDir.delete();

                            pnt3dInt = varPntInt[0];

                            break;
                    }

                    foreach (DBObject dbObj in varEnts)
                        idList.Add(dbObj.ObjectId);

                    foreach (ObjectId id in idList)
                        id.delete();

                    break;
            }

            BaseObjs.regen();

            return pnt3dInt;
        }
    }
}