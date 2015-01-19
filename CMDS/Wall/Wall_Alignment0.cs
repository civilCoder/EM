using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using wdpv = Wall.Wall_DesignProfileView;

namespace Wall
{
    public static class Wall_Alignment0
    {
        public static ObjectId
        makeAlignWall(Alignment objAlign, ProfileView objProfileView, double dblStaBeg, double dblStaEnd){
            ObjectId idAlign = ObjectId.Null;


            string strAlignName = Align.getAlignName("WALL");

            double dblX = 0;
            double dblY = 0;

            Point2dCollection pnts2d = new Point2dCollection();

            objAlign.PointLocation(dblStaBeg, 0.0, ref dblX, ref dblY);
            Point2d pnt2dBeg = new Point2d(dblX, dblY);
            pnts2d.Add(pnt2dBeg);

            objAlign.PointLocation(dblStaEnd, 0.0, ref dblX, ref dblY);
            Point2d pnt2dEnd = new Point2d(dblX, dblY);
            pnts2d.Add(pnt2dEnd);

            //omitting anything other than single tangent

            ObjectId idPolyWall = Base_Tools45.Draw.addPoly(pnts2d);

            string strLayer = strAlignName + "-Align";
            ObjectId idLayer = Layer.manageLayers(strLayer);

            Alignment objAlignWall = Align.addAlignmentFromPoly(strAlignName, strLayer, idPolyWall, "Standard", "Standard", true);

            objAlignWall.ReferencePointStation = 1000.0;

            double dblStaOff = dblStaBeg - objAlignWall.StartingStation;

            Profile objProfileEX = Prof.getProfile(objAlign.Name, "EXIST");
            Profile objProfileDE = Prof.getProfile(objAlign.Name, "CPNT");

            List<PNT_XY> varStaOff = new List<PNT_XY>();
            PNT_XY vStaOff = new PNT_XY();

            vStaOff.x = (float)objAlignWall.StartingStation;
            vStaOff.y = (float)objProfileEX.ElevationAt(dblStaBeg);
            varStaOff.Add(vStaOff);

            int i = 0;
            int k = 0;

            foreach (ProfilePVI objPVIex in objProfileEX.PVIs)
            {
                if (objPVIex.Station > dblStaBeg & objPVIex.Station < dblStaEnd)
                {
                    vStaOff.x = (float)(objPVIex.Station - dblStaOff);
                    vStaOff.y = (float)objProfileEX.ElevationAt(objPVIex.Station);
                    varStaOff.Add(vStaOff);
                }

            }

            vStaOff.x = (float)objAlignWall.EndingStation;
            vStaOff.y = (float)objProfileEX.ElevationAt(dblStaEnd);
            varStaOff.Add(vStaOff);

            ObjectId idProfileStyle = Prof_Style.getProfileStyle("WALL");
            ObjectId idStyleLabelSet = Prof_Style.getProfileLabelSetStyle("WALL");
            using (BaseObjs._acadDoc.LockDocument())
            {
                using (Transaction TR = BaseObjs.startTransactionDb())
                {

                    Profile profileEX = Prof.addProfileByLayout("EXIST", objAlignWall.ObjectId, idLayer, idProfileStyle, idStyleLabelSet);

                    for (i = 0; i <= varStaOff.Count - 1; i++)
                    {
                        objProfileEX.PVIs.AddPVI(varStaOff[i].x, varStaOff[i].y);
                        profileEX.PVIs.AddPVI(varStaOff[i].x, varStaOff[i].y);
                    }

                    varStaOff = new List<PNT_XY>();

                    vStaOff.x = (float)objAlignWall.StartingStation;
                    vStaOff.y = (float)objProfileDE.ElevationAt(dblStaBeg);
                    varStaOff.Add(vStaOff);


                    foreach (ProfilePVI objPviDE in objProfileDE.PVIs)
                    {
                        if (objPviDE.Station > dblStaBeg & objPviDE.Station < dblStaEnd)
                        {
                            vStaOff.x = (float)(objPviDE.Station - dblStaOff);
                            vStaOff.y = (float)objProfileDE.ElevationAt(objPviDE.Station);
                            varStaOff.Add(vStaOff);
                        }
                    }

                    if (objAlignWall.EndingStation - varStaOff[k].x > 0.1)
                    {
                        vStaOff.x = (float)System.Math.Round(objAlignWall.EndingStation, 2);
                        vStaOff.y = (float)objProfileDE.ElevationAt(dblStaEnd);
                        varStaOff.Add(vStaOff);
                    }

                    objProfileDE = Prof.addProfileByLayout("CPNT", objAlign.ObjectId, idLayer, idProfileStyle, idStyleLabelSet);
                    for (i = 0; i <= varStaOff.Count - 1; i++)
                    {
                        if (varStaOff[i].x <= objAlignWall.EndingStation)
                        {
                            objProfileDE.PVIs.AddPVI(varStaOff[i].x, varStaOff[i].y);
                        }
                    }

                    wdpv.CreateProfileViewPrelim(objAlignWall);

                    TR.Commit();

                }
            }

            return idAlign;
        }
    }
}
