using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;

using Base_Tools45;
using Base_Tools45.C3D;

namespace PacCerts
{
    public static class PC_LblElevDiff
    {
        public static void 
        labelElevDiff(double dblElevPnt, double dblElevSurface, List<Point3d> pnts3dLdr, Point3d pnt3dInsTxt, double dblDir)
        {
            string nameLayer = "GRADES";
            ObjectId idLayer = Layer.manageLayers(nameLayer);
            double dblElevDiff = dblElevSurface - dblElevPnt;

            string strText = "";
            string strText1 = "";
            string strText2 = "";
            string strText3 = "";

            strText1 = string.Format("{0:#0.00}-DESIGN", dblElevSurface);
            strText2 = string.Format("\\P{0:#0.00}-EXIST", dblElevPnt);
            if (dblElevDiff < 0)
            {
                strText3 = string.Format("\\P{0:##.00} C", dblElevDiff);
            }
            else if (dblElevDiff >= 0)
            {

                strText3 = string.Format("\\P{0:##.00} F", dblElevDiff);
            }

            strText = strText1 + strText2 + strText3;

            int intDrawUnits = Misc.getCurrAnnoScale();
            double dblCurrHeight = 0.09 * intDrawUnits;

            ObjectId idMTxt = Txt.addMText(strText, pnt3dInsTxt, dblDir, 0, attachPnt: AttachmentPoint.MiddleLeft, nameLayer: nameLayer);

            Point3dCollection pnts3dLdrCol = new Point3dCollection();
            foreach (Point3d pnt3d in pnts3dLdr)
                pnts3dLdrCol.Add(pnt3d);

            ObjectId idLdr = Ldr.addLdr(pnts3dLdrCol, idLayer, sizeArrow: 0.1, sizeGap: 0.1, color: clr.wht, idMTxt: idMTxt);

            //List<Point3d> pnts3d = idLdr.getCoordinates3dList();



        }

    }
}
