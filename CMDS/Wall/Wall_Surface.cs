using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using p = Wall.Wall_Public;
using wdp = Wall.Wall_DesignProfile;
using wu = Wall.Wall_Utility;

namespace Wall
{
    public static class Wall_Surface
    {
        public static void
        wallBrlkinesToSurface(Alignment objAlignPL, Alignment objAlignRF, Alignment objAlignWall)
        {

            FEATUREDATA fDataW0 = new FEATUREDATA();
            FEATUREDATA fDataW1 = new FEATUREDATA();
            FEATUREDATA fDataW2 = new FEATUREDATA();
            FEATUREDATA fDataW3 = new FEATUREDATA();
            FEATUREDATA fDataG0 = new FEATUREDATA();
            FEATUREDATA fDataGB = new FEATUREDATA();

            bool exists = false;
            try
            {
                TinSurface objSurfaceCPNT = Surf.getTinSurface("CPNT-ON", out exists);

                string strLayer = null;
                strLayer = string.Format("{0}-BRKLINE", objAlignPL.Name);
                Layer.manageLayers(strLayer);

                //-------------------------------------GUTTER BREAKLINES -------------------------------------------------------------------------------------------------------------------ADDED TO CPNT

                ObjectIdCollection objEntsGutter = new ObjectIdCollection();
                ObjectIdCollection objEntsEndsGutter = new ObjectIdCollection();


                for (int i = 0; i < p.dGUTs.Count - 1; i++)
                {

                    if (p.dGUTs[i]["W3"].Count > 1)
                    {
                        objEntsGutter.Add(wu.fData_3dPoly("W3", strLayer, p.dGUTs[i]["W3"], objAlignRF, objAlignPL));                    
                        objEntsGutter.Add(wu.fData_3dPoly("G0", strLayer, p.dGUTs[i]["G0"], objAlignRF, objAlignPL));
                        objEntsGutter.Add(wu.fData_3dPoly("GB", strLayer, p.dGUTs[i]["GB"], objAlignRF, objAlignPL));

                        objEntsEndsGutter = wdp.makeEndBrklinesWALL(strLayer, objEntsGutter, false);

                        objSurfaceCPNT.BreaklinesDefinition.AddStandardBreaklines(objEntsEndsGutter, 0, 0, 0, 0);

                    }

                }

                objSurfaceCPNT.BreaklinesDefinition.AddStandardBreaklines(objEntsGutter, 0, 0, 0, 0);

                //-------------------------------------WALL BREAKLINES------------------------------------------------------------------------------------------------------------------  NEW SURFACE - WALL

                ObjectIdCollection objEntsWall = new ObjectIdCollection();
                ObjectIdCollection objEntEndsWall = new ObjectIdCollection();
                ObjectIdCollection objEntsWallCPNT = new ObjectIdCollection();


                for (int i = 0; i < p.dWALLs.Count - 1; i++)
                {
                    //TEST IS THERE ARE AT LEAST TWO POINTS
                    if (p.dWALLs[i]["W0"].Count > 1)
                    {

                        objEntsWall.Add(wu.fData_3dPoly("W0", strLayer, p.dWALLs[i]["W0"], objAlignRF, objAlignPL));
                        objEntsWall.Add(wu.fData_3dPoly("W1", strLayer, p.dWALLs[i]["W1"], objAlignRF, objAlignPL));
                        objEntsWall.Add(wu.fData_3dPoly("W2", strLayer, p.dWALLs[i]["W2"], objAlignRF, objAlignPL));
                        objEntsWall.Add(wu.fData_3dPoly("W3", strLayer, p.dWALLs[i]["W3"], objAlignRF, objAlignPL));
                        objEntsWall.Add(wu.fData_3dPoly("W4", strLayer, p.dWALLs[i]["W4"], objAlignRF, objAlignPL));
                        objEntsWall.Add(wu.fData_3dPoly("W5", strLayer, p.dWALLs[i]["W5"], objAlignRF, objAlignPL));
                        objEntsWall.Add(wu.fData_3dPoly("W6", strLayer, p.dWALLs[i]["W6"], objAlignRF, objAlignPL));
                        objEntsWall.Add(wu.fData_3dPoly("W7", strLayer, p.dWALLs[i]["W7"], objAlignRF, objAlignPL));
                        objEntsWall.Add(wu.fData_3dPoly("W8", strLayer, p.dWALLs[i]["W8"], objAlignRF, objAlignPL));
                        objEntsWall.Add(wu.fData_3dPoly("W9", strLayer, p.dWALLs[i]["W9"], objAlignRF, objAlignPL));

                        objEntsWallCPNT.Add(objEntsWall[0]);
                        objEntsWallCPNT.Add(objEntsWall[1]);
                        objEntsWallCPNT.Add(objEntsWall[2]);
                        objEntsWallCPNT.Add(objEntsWall[3]);

                        TinSurface objSurfaceWallCut = Surf.getTinSurface(Align.getAlignName("WALL") + "-CUT", out exists);
                        objSurfaceWallCut.BreaklinesDefinition.AddStandardBreaklines(objEntsWall, 0, 0, 0, 0);

                        objEntEndsWall = wdp.makeEndBrklinesWALL(strLayer, objEntsWall, true);
                        objSurfaceWallCut.BreaklinesDefinition.AddStandardBreaklines(objEntEndsWall, 0, 0, 0, 0);
                        //ADD END BREAKLINES

                        objSurfaceCPNT.BreaklinesDefinition.AddStandardBreaklines(objEntsWallCPNT, 0, 0, 0, 0);

                    }

                }

                //-----------------------------------  WALL ENDS  -------------------------------------------------------------------------------------------------------------------

                double dblStaBeg = objAlignRF.StartingStation;
                double dblStaEnd = objAlignRF.EndingStation;
                double dblEasting = 0, dblNorthing = 0;

                if (p.dWALLs[0]["W0"].ContainsKey((float)dblStaBeg))
                {

                    List<Point3d> dblPntsBEG = new List<Point3d>();

                    fDataW0 = p.dWALLs[0]["W0"][(float)dblStaBeg];
                    fDataW1 = p.dWALLs[0]["W1"][(float)dblStaBeg];
                    fDataW2 = p.dWALLs[0]["W2"][(float)dblStaBeg];
                    fDataW3 = p.dWALLs[0]["W3"][(float)dblStaBeg];
                    fDataG0 = p.dGUTs[0]["G0"][(float)dblStaBeg];
                    fDataGB = p.dGUTs[0]["GB"][(float)dblStaBeg];

                    objAlignRF.PointLocation(fDataW0.Station, fDataW0.Offset, ref dblEasting, ref dblNorthing);
                    dblPntsBEG.Add(new Point3d(dblEasting, dblNorthing, fDataW0.Elev));

                    objAlignRF.PointLocation(fDataW1.Station, fDataW1.Offset, ref dblEasting, ref dblNorthing);
                    dblPntsBEG.Add(new Point3d(dblEasting, dblNorthing, fDataW1.Elev));

                    objAlignRF.PointLocation(fDataW2.Station, fDataW2.Offset, ref dblEasting, ref dblNorthing);
                    dblPntsBEG.Add(new Point3d(dblEasting, dblNorthing, fDataW2.Elev));

                    objAlignRF.PointLocation(fDataW3.Station, fDataW3.Offset, ref dblEasting, ref dblNorthing);
                    dblPntsBEG.Add(new Point3d(dblEasting, dblNorthing, fDataW3.Elev));

                    objAlignRF.PointLocation(fDataG0.Station, fDataG0.Offset, ref dblEasting, ref dblNorthing);
                    dblPntsBEG.Add(new Point3d(dblEasting, dblNorthing, fDataG0.Elev));

                    objAlignPL.PointLocation(fDataGB.Station, fDataGB.Offset, ref dblEasting, ref dblNorthing);
                    dblPntsBEG.Add(new Point3d(dblEasting, dblNorthing, fDataGB.Elev));

                    //Wall_Forms.fWall1.PntsWallCutBeg = dblPntsBEG;

                }

                int j = p.dWALLs.Count - 1;


                if (j > 1)
                {

                    if (p.dWALLs[j]["W0"].ContainsKey((float)dblStaEnd))
                    {
                        List<Point3d> dblPntsEND = new List<Point3d>();

                        objAlignRF.PointLocation(fDataW0.Station, fDataW0.Offset, ref dblEasting, ref dblNorthing);
                        dblPntsEND.Add(new Point3d(dblEasting, dblNorthing, fDataW0.Elev));

                        objAlignRF.PointLocation(fDataW1.Station, fDataW1.Offset, ref dblEasting, ref dblNorthing);
                        dblPntsEND.Add(new Point3d(dblEasting, dblNorthing, fDataW1.Elev));

                        objAlignRF.PointLocation(fDataW2.Station, fDataW2.Offset, ref dblEasting, ref dblNorthing);
                        dblPntsEND.Add(new Point3d(dblEasting, dblNorthing, fDataW2.Elev));

                        objAlignRF.PointLocation(fDataW3.Station, fDataW3.Offset, ref dblEasting, ref dblNorthing);
                        dblPntsEND.Add(new Point3d(dblEasting, dblNorthing, fDataW3.Elev));

                        objAlignRF.PointLocation(fDataG0.Station, fDataG0.Offset, ref dblEasting, ref dblNorthing);
                        dblPntsEND.Add(new Point3d(dblEasting, dblNorthing, fDataG0.Elev));

                        objAlignPL.PointLocation(fDataGB.Station, fDataGB.Offset, ref dblEasting, ref dblNorthing);
                        dblPntsEND.Add(new Point3d(dblEasting, dblNorthing, fDataGB.Elev));

                        //Wall_Forms.wForms.fWall1.PntsWallCutEnd = dblPntsEND;

                    }

                }

                //-------------------------------------REGULARS----------------------------------------------------------------------------------------------------------------------------------- ADD TO CPNT

                ObjectIdCollection objEntsNOM = new ObjectIdCollection();
                ObjectIdCollection objEntEnds = new ObjectIdCollection();

                objEntsNOM.Add(wu.fData_3dPoly("RF", strLayer, p.dNOM["RF"], objAlignRF, objAlignPL));
                objEntsNOM.Add(wu.fData_3dPoly("RF", strLayer, p.dNOM["FL"], objAlignRF, objAlignPL));
                objEntsNOM.Add(wu.fData_3dPoly("RF", strLayer, p.dNOM["TC"], objAlignRF, objAlignPL));
                objEntsNOM.Add(wu.fData_3dPoly("RF", strLayer, p.dNOM["TOE"], objAlignRF, objAlignPL));
                objEntsNOM.Add(wu.fData_3dPoly("RF", strLayer, p.dNOM["TOP"], objAlignRF, objAlignPL));
                objEntsNOM.Add(wu.fData_3dPoly("RF", strLayer, p.dNOM["PL"], objAlignRF, objAlignPL));

                objEntEnds = wdp.makeEndBrklines(strLayer, objEntsNOM);

                objSurfaceCPNT.BreaklinesDefinition.AddStandardBreaklines(objEntsNOM, 0, 0, 0, 0);
                objSurfaceCPNT.BreaklinesDefinition.AddStandardBreaklines(objEntEnds, 0, 0, 0, 0);


            }
            catch (Autodesk.AutoCAD.Runtime.Exception)
            {
            }
        }
    }
}
