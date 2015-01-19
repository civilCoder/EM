using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

using Base_Tools45;
using btp = Base_Tools45.Pub;

namespace LdrText
{
    public static class LdrText_Move
    {
        const double PI = System.Math.PI;

        public static void
        moveLdrGS(ObjectId idMTxt, TypedValue[] tvsText, Point3d pnt3dInsOrg)
        {
            Point3d pnt3dInsCurr = idMTxt.getMTextLocation();

            Vector3d v3d = new Vector3d(pnt3dInsCurr.X - pnt3dInsOrg.X, pnt3dInsCurr.Y - pnt3dInsOrg.Y, 0);

            ObjectId idLdr = tvsText.getObjectId(3);
            ResultBuffer rbLdr = idLdr.getXData(apps.lnkGS);
            if (rbLdr == null)
                return;
            TypedValue[] tvsLdr = rbLdr.AsArray();
            Point3d pnt3dM = new Point3d(double.Parse(tvsLdr[8].Value.ToString()), double.Parse(tvsLdr[9].Value.ToString()), 0.0);  //stored point on leader opposite text insertion point
            
            Point3d pnt3dMx = Pub.pnt3dO;
            if(!btp.shiftKey){
                 pnt3dMx = pnt3dM.TransformBy(Matrix3d.Displacement(v3d));       //calculated mid point of leader new location                
            }else{
                double dir = pnt3dM.getDirection(pnt3dInsOrg);
                double dist = pnt3dM.getDistance(pnt3dInsOrg);

                pnt3dMx = pnt3dInsCurr.traverse(dir, dist);
                idMTxt.changeGStxtAttachmentPoint();
            }

            idLdr.moveObj(pnt3dM, pnt3dMx);  //text moves -> ldr goes with text

            Point3d pnt3dLdr1 = idLdr.getBegPnt();
            Point3d pnt3dLdr2 = idLdr.getEndPnt();

            tvsLdr[4] = new TypedValue(1040, pnt3dLdr1.X);
            tvsLdr[5] = new TypedValue(1040, pnt3dLdr1.Y);
            tvsLdr[6] = new TypedValue(1040, pnt3dLdr2.X);
            tvsLdr[7] = new TypedValue(1040, pnt3dLdr2.Y);
            tvsLdr[8] = new TypedValue(1040, pnt3dMx.X);        //point on leader line opposite text insertion point
            tvsLdr[9] = new TypedValue(1040, pnt3dMx.Y);

            idLdr.setXData(tvsLdr, apps.lnkGS);

            ResultBuffer rbMTxt = idMTxt.getXData(apps.lnkGS);
            TypedValue[] tvsMTxt = rbMTxt.AsArray();

            string s1 = tvsMTxt[9].Value.ToString();
            string s2 = tvsMTxt[10].Value.ToString();
            string s11 = tvsMTxt[11].Value.ToString();

            string [] xyz = s11.Split(new char[] {' '});
            
            Point3d pnt3d1 = Pub.pnt3dO;
            Point3d pnt3d2 = Pub.pnt3dO;

            if(int.Parse(s1) != 0){
                Handle h1 = s1.stringToHandle();                
                pnt3d1 = h1.getCogoPntCoordinates();
            }else{
                pnt3d1 = new Point3d(double.Parse(xyz[0]), double.Parse(xyz[1]), double.Parse(xyz[2]));
            }

            if(int.Parse(s2) != 0){
                Handle h2 = s2.ToString().stringToHandle();                
                pnt3d2 = h2.getCogoPntCoordinates();
            }else{
                pnt3d2 = new Point3d(double.Parse(xyz[0]), double.Parse(xyz[1]), double.Parse(xyz[2]));
            }
                       
            double station = 0.0, offset = 0.0;
            Geom.getStaOff(pnt3d1, pnt3d2, pnt3dMx, ref station, ref offset);

            tvsMTxt[5] = new TypedValue(1040, station);
            tvsMTxt[6] = new TypedValue(1040, offset);
            tvsMTxt[7] = new TypedValue(1040, pnt3dInsCurr.X);
            tvsMTxt[8] = new TypedValue(1040, pnt3dInsCurr.Y);
            tvsMTxt[9] = new TypedValue(1040, pnt3dMx.X);
            tvsMTxt[10] = new TypedValue(1040, pnt3dMx.Y);

            idMTxt.setXData(tvsMTxt, apps.lnkGS);
        }

        public static void
        moveTX(ObjectId idTxTop, TypedValue[] tvsLdrText, Point3d pnt3dInsOrg)
        {
            Point3d pnt3dInsCurr = idTxTop.getMTextLocation();
            Point3d pnt3dNew;

            Matrix3d matrx3d = BaseObjs._editor.CurrentUserCoordinateSystem;
            CoordinateSystem3d coordSys = matrx3d.CoordinateSystem3d;
            Vector3d v3d = new Vector3d(pnt3dInsCurr.X - pnt3dInsOrg.X, pnt3dInsCurr.Y - pnt3dInsOrg.Y, 0);

            ObjectId idTxBot = tvsLdrText.getObjectId(4);
            ObjectId idTxBot2 = tvsLdrText.getObjectId(5);

            if (idTxBot != ObjectId.Null)
            {
                Point3d pnt3dIns = idTxBot.getMTextLocation();
                pnt3dNew = pnt3dIns.TransformBy(Matrix3d.Displacement(v3d));
                idTxBot.moveObj(pnt3dIns, pnt3dNew);
            }

            if (idTxBot2 != ObjectId.Null)
            {
                Point3d pnt3dIns = idTxBot2.getMTextLocation();
                pnt3dNew = pnt3dIns.TransformBy(Matrix3d.Displacement(v3d));
                idTxBot2.moveObj(pnt3dIns, pnt3dNew);
            }

            ObjectId idLdr = tvsLdrText.getObjectId(3);
            Point3dCollection pnts3dLdr = idLdr.getCoordinates3d();
            if (pnts3dLdr.Count == 2)
            {
                Application.ShowAlertDialog("Method not implemented for single segment Leaders.");
                return;
            }
            int n = pnts3dLdr.Count;
            Point3d pnt3dEnd = idLdr.getLastVertex();
            Point3d pnt3dNextToLast = idLdr.getNextToLastVertex();

            pnt3dNew = pnt3dEnd.TransformBy(Matrix3d.Displacement(v3d));
            idLdr.updateVertex(n - 1, pnt3dNew);

            pnt3dNew = pnt3dNextToLast.TransformBy(Matrix3d.Displacement(v3d));
            idLdr.updateVertex(n - 2, pnt3dNew);

            pnts3dLdr = idLdr.getCoordinates3d();

            ResultBuffer rb = idLdr.getXData(apps.lnkLD);
            if (rb != null)
            {
                TypedValue[] tvsLdr = rb.AsArray();

                for (int i = 0; i < pnts3dLdr.Count; i++)
                {
                    tvsLdr[i * 2 + 4] = new TypedValue(1040, pnts3dLdr[i].X);
                    tvsLdr[i * 2 + 5] = new TypedValue(1040, pnts3dLdr[i].Y);
                }

                idLdr.setXData(tvsLdr, apps.lnkLD);
            }

            rb = idTxTop.getXData(apps.lnkLD);
            if (rb != null)
            {
                TypedValue[] tvsTxTop = idTxTop.getXData(apps.lnkLD).AsArray();

                tvsTxTop[6] = new TypedValue(1040, pnt3dInsCurr.X);
                tvsTxTop[7] = new TypedValue(1040, pnt3dInsCurr.Y);

                idTxTop.setXData(tvsTxTop, apps.lnkLD);
            }
        }

        public static void
        moveTxtGS(ObjectId idLdr, TypedValue[] tvsLdr)
        {
            ObjectId idMTxt = tvsLdr.getObjectId(3);
            Point3d pnt3dIns = idMTxt.getMTextLocation();

            Point3d pnt3dLdrBegOrg = new Point3d(double.Parse(tvsLdr[4].Value.ToString()), double.Parse(tvsLdr[5].Value.ToString()), 0.0);
            Point3d pnt3dLdrEndOrg = new Point3d(double.Parse(tvsLdr[6].Value.ToString()), double.Parse(tvsLdr[7].Value.ToString()), 0.0);
            Point3d pnt3dLdrMidOrg = new Point3d(double.Parse(tvsLdr[8].Value.ToString()), double.Parse(tvsLdr[9].Value.ToString()), 0.0);

            Point3dCollection pnts3dLdr = idLdr.getCoordinates3d();
            int n = pnts3dLdr.Count;

            Matrix3d matrx3d = BaseObjs._editor.CurrentUserCoordinateSystem;
            CoordinateSystem3d coordSys = matrx3d.CoordinateSystem3d;

            Point3d pnt3dLdrBegCur = pnts3dLdr[0];
            Point3d pnt3dLdrEndCur = pnts3dLdr[1];

            Point3d pnt3dNew = Pub.pnt3dO;
            Point3d pnt3dInsNew = Pub.pnt3dO;
            Point3d pnt3dLdrMidNew = Pub.pnt3dO;

            Vector3d v3d;
            if (!pnt3dLdrBegOrg.isEqual(pnt3dLdrBegCur, 0.01) && !pnt3dLdrEndOrg.isEqual(pnt3dLdrEndCur, 0.01))
            { //both end points moved -> grip_stretch midpoint or moved
                v3d = new Vector3d(pnt3dLdrBegCur.X - pnt3dLdrBegOrg.X, pnt3dLdrBegCur.Y - pnt3dLdrBegOrg.Y, 0);
            }
            else if (!pnt3dLdrBegOrg.isEqual(pnt3dLdrBegCur, 0.01))
            { //one moved check beg point
                v3d = new Vector3d(pnt3dLdrBegCur.X - pnt3dLdrBegOrg.X, pnt3dLdrBegCur.Y - pnt3dLdrBegOrg.Y, 0);
                pnt3dNew = pnt3dLdrEndOrg.TransformBy(Matrix3d.Displacement(v3d));
                idLdr.updateVertex(n - 1, pnt3dNew);
            }
            else
            { //check end point
                v3d = new Vector3d(pnt3dLdrEndCur.X - pnt3dLdrEndOrg.X, pnt3dLdrEndCur.Y - pnt3dLdrEndOrg.Y, 0);
                pnt3dNew = pnt3dLdrBegOrg.TransformBy(Matrix3d.Displacement(v3d));
                idLdr.updateVertex(n - 2, pnt3dNew);
            }
            pnt3dInsNew = pnt3dIns.TransformBy(Matrix3d.Displacement(v3d));
            idMTxt.setMTextLocation(pnt3dInsNew);
            pnt3dLdrMidNew = pnt3dLdrMidOrg.TransformBy(Matrix3d.Displacement(v3d));

            pnts3dLdr = idLdr.getCoordinates3d();

            ResultBuffer rbMTxt = idMTxt.getXData(apps.lnkGS);
            TypedValue[] tvsMTxt = rbMTxt.AsArray();

            Point3d pnt3d1 = tvsMTxt.getObjectId(9).getCogoPntCoordinates();
            Point3d pnt3d2 = tvsMTxt.getObjectId(10).getCogoPntCoordinates();

            double station = 0.0, offset = 0.0;
            Geom.getStaOff(pnt3d1, pnt3d2, pnt3dLdrMidNew, ref station, ref offset);

            tvsMTxt[5] = new TypedValue(1040, station);
            tvsMTxt[6] = new TypedValue(1040, offset);
            tvsMTxt[7] = new TypedValue(1040, pnt3dInsNew.X);
            tvsMTxt[8] = new TypedValue(1040, pnt3dInsNew.Y);

            idMTxt.setXData(tvsMTxt, apps.lnkGS);

            tvsLdr[4] = new TypedValue(1040, pnts3dLdr[0].X);
            tvsLdr[5] = new TypedValue(1040, pnts3dLdr[0].Y);
            tvsLdr[6] = new TypedValue(1040, pnts3dLdr[1].X);
            tvsLdr[7] = new TypedValue(1040, pnts3dLdr[1].Y);
            tvsLdr[8] = new TypedValue(1040, pnt3dLdrMidNew.X);
            tvsLdr[9] = new TypedValue(1040, pnt3dLdrMidNew.Y);

            idLdr.setXData(tvsLdr, apps.lnkGS);
        }
    }
}
