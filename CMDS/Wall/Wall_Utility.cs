using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using System.Collections.Generic;

namespace Wall {
    public static class Wall_Utility {
        static PromptStatus ps;

        public static ObjectId
        fData_3dPoly(string strID, string strLayer, Dictionary<float, FEATUREDATA> fData, Alignment objAlignRF, Alignment objAlignPL) {
            ObjectId idPoly3d = ObjectId.Null;

            List<Point3d> pnts3d = fData_pnts3d(fData, objAlignRF, objAlignPL);

            idPoly3d = Draw.addPoly3d(pnts3d, strLayer, 11);

            TypedValue[] TVs = new TypedValue[2];
            TVs[0] = new TypedValue(1001, "WALL");
            TVs[1] = new TypedValue(1000, strID);

            idPoly3d.setXData(TVs, "WALL");

            return idPoly3d;
        }

        public static List<Point3d>
        fData_pnts3d(Dictionary<float, FEATUREDATA> fData, Alignment objAlignRF, Alignment objAlignPL) {
            List<Point3d> pnts3d = new List<Point3d>();

            FEATUREDATA fd = default(FEATUREDATA);
            double fk = 0;

            double dblEasting = 0;
            double dblNorthing = 0;

            int i = -1;
            int k = fData.Count - 1;

            foreach (KeyValuePair<float, FEATUREDATA> kvp in fData) {
                fd = kvp.Value;
                fk = kvp.Key;
                i = i + 1;
                switch (fd.AlignName) {
                    case "RF":

                        objAlignRF.PointLocation(fk, fd.Offset, ref dblEasting, ref dblNorthing);

                        pnts3d.Add(new Point3d(dblEasting, dblNorthing, fd.Elev));

                        break;
                    case "PL":

                        objAlignPL.PointLocation(fk, fd.Offset, ref dblEasting, ref dblNorthing);

                        pnts3d.Add(new Point3d(dblEasting, dblNorthing, fd.Elev));

                        break;
                }
            }

            return pnts3d;
        }

        public static double
        getStationAlign(Alignment objAlign, string strPrompt) {
            double sta = 0.0;
            double off = 0.0;

            Point3d pnt3d = UserInput.getPoint(strPrompt, out ps, osMode: 0);
            objAlign.StationOffset(pnt3d.X, pnt3d.Y, ref sta, ref off);

            return sta;
        }

        public static double
        getStationProfile(ProfileView objProfileView, string strPrompt) {
            double sta = 0.0;
            double elev = 0.0;

            Point3d pnt3d = UserInput.getPoint(strPrompt, out ps, osMode: 0);
            objProfileView.FindStationAndElevationAtXY(pnt3d.X, pnt3d.Y, ref sta, ref elev);

            return sta;
        }
    }
}