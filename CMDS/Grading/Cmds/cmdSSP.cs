using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using Base_Tools45.C3D;

namespace Grading.Cmds
{
    public static class cmdSSP
    {
        public static void
        SSP()
        {
            ObjectId idLine = ObjectId.Null;
            Point3d pnt3dInt = Pub.pnt3dO;
            PromptStatus ps = PromptStatus.Cancel;

            try
            {
                Point3d pnt3d1 = UserInput.getPoint("\nSelect FIRST point: ", out ps, osMode: 8);
                if (pnt3d1 == Pub.pnt3dO)
                    return;

                ObjectId idCgPnt = Select.selectCogoPntAtPoint3d(pnt3d1);
                string descPnt = idCgPnt.getCogoPntDesc();

                Point3d pnt3d2 = UserInput.getPoint("\nSelect SECOND point: ", out ps, osMode: 8);
                if (pnt3d2 == Pub.pnt3dO)
                    return;

                double distH = pnt3d1.getDistance(pnt3d2);
                double distV = pnt3d2.Z - pnt3d1.Z;

                double slp0 = distV / distH;

                idLine = Draw.addLine(pnt3d1, pnt3d2);

                BaseObjs.write(string.Format("Slope between points is: {0:P2}", slp0));

                double m1 = Pub.slp1;
                double m2 = Pub.slp2;

                UserInput.getUserInput("\nEnter slope from FIRST POINT: ", out m1, m1);
                if (m1 == -99.99)
                {
                    return;
                }
                UserInput.getUserInput("\nEnter slope from SECOND POINT: ", out m2, m2);
                if (m2 == -99.99)
                {
                    return;
                }

                Pub.slp1 = m1;
                Pub.slp2 = m2;

                m2 = -m2;

                idLine.delete();

                pnt3dInt = getSlopeIntercept(pnt3d1, pnt3d2, m1, m2);
                if (pnt3dInt == Pub.pnt3dO)
                {
                    return;
                }
                uint pntNum;
                pnt3dInt.setPoint(out pntNum, descPnt);
                Application.ShowAlertDialog(string.Format("The elevation of the slope\nintercept is {0:F2}", pnt3dInt.Z));
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdSSP.cs: line: 73");
            }
            finally
            {
                idLine.delete();
            }
        }

        public static Point3d
        getSlopeIntercept(Point3d pnt3d1, Point3d pnt3d2, double m1, double m2)
        {
            Point3d pnt3dInt = Pub.pnt3dO;

            double distH = pnt3d1.getDistance(pnt3d2);
            double distV = pnt3d2.Z - pnt3d1.Z;
            double distX = 0;
            double dir = pnt3d1.getDirection(pnt3d2);

            double algDiff = m2 - m1;

            if (m1 == -m2)
                if (pnt3d1.Z == pnt3d2.Z)
                    return pnt3d1.traverse(dir, distH / 2, m1);

            try
            {
                distX = ((pnt3d2.Z - pnt3d1.Z) - (distH * m2)) / (m1 - m2);
            }
            catch (System.DivideByZeroException)
            {
                return pnt3dInt;
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdSSP.cs: line: 107");
            }
            finally
            {
            }

            if (distX < 0 || distX > distH)
                Application.ShowAlertDialog("Slope inputs results are out of bounds...");
            else
                pnt3dInt = pnt3d1.traverse(dir, distX, m1);

            return pnt3dInt;
        }

        public static double
        getSlopeInterceptArc(double distH, double elev1, double elev2, double m1, double m2)
        {
            double distV = elev2 - elev1;
            double distX = 0;

            double algDiff = m2 - m1;

            if (m1 == -m2)
                if (elev1 == elev2)
                    return distH / 2;
            try
            {
                distX = ((distV) - (distH * m2)) / (m1 - m2);
            }
            catch (System.DivideByZeroException)
            {
                return distX;
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " cmdSSP.cs: line: 142");
                return distX;
            }

            if (distX < 0 || distX > distH)
                Application.ShowAlertDialog("Slope inputs results are out of bounds...");
            else
                return distX;

            return distX;
        }
    }
}
