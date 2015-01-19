using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using System.Collections.Generic;
using wd = Wall.Wall_Design;

namespace Wall
{
    public static class Wall_Alignment2d
    {
        static Wall_Form.frmWall1 fWall1 = Wall_Forms.wForms.fWall1;

        public static void
        getStationsToSample(ObjectId idAlignPL, ObjectId idAlignRF){

            //CHECK IF ALIGNMENT DIRECTIONS ARE OPPOSING - CORRECT IF NECESSARY

            Alignment objAlignRF = (Alignment)idAlignRF.getEnt();
            Alignment objAlignPL = (Alignment)idAlignPL.getEnt();
            double dblEasting = 0, dblNorthing = 0;
            double dblStationPL = 0, dblOffsetPL = 0;
            int intSign = 0;

            {
                objAlignRF.PointLocation(objAlignRF.StartingStation, 0, ref dblEasting, ref dblNorthing);
                try
                {
                    objAlignPL.StationOffset(dblEasting, dblNorthing, ref dblStationPL, ref dblOffsetPL);
                }
                catch (Autodesk.Civil.PointNotOnEntityException )
                {
                    dblStationPL = 0.0;
                }

                if (dblOffsetPL < 0)
                {
                    intSign = -1;
                    dblOffsetPL = dblOffsetPL * -1;
                }
                else
                {
                    intSign = 1;
                }

                fWall1.SIDE = intSign;

                //GET LIST OF STATIONS TO SAMPLE

                List<double> dblStationsFinal = new List<double>();
                dblStationsFinal = wd.getPNT_DATA_FINAL(objAlignPL, objAlignRF);
                fWall1.Stations = dblStationsFinal;
            }
        }
    }
}
