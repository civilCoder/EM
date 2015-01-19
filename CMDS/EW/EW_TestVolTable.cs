
using Autodesk.AutoCAD.Colors;

using Autodesk.Civil.DatabaseServices;
using Base_Tools45.C3D;

namespace EW
{
    public static class EW_TestVolTable
    {

        public static void
        testVolTable(int numSteps = 0, double incre = 0, short clrStart = 100)
        {

            TinVolumeSurface objSurfaceVOL = Surf.getTinVolumeSurface("wse");

            double minElev = objSurfaceVOL.GetGeneralProperties().MinimumElevation;
            double maxElev = objSurfaceVOL.GetGeneralProperties().MaximumElevation;
            if (incre == 0)
                incre = (maxElev - minElev) / numSteps;
            if(numSteps == 0)
                numSteps = (int)((maxElev - minElev) / incre);

            SurfaceAnalysisElevationData[] objSurfaceAnalysisElevation = new SurfaceAnalysisElevationData[numSteps];

            for (int i = 0; i < numSteps; i++ ){
                Color clr = Color.FromColorIndex(ColorMethod.ByLayer, (short)(clrStart + (i * 2)));
                objSurfaceAnalysisElevation[i] = new SurfaceAnalysisElevationData(minElev + (incre * i), minElev + (incre * (i + 1)), clr);
            }

            objSurfaceVOL.Analysis.SetElevationData(objSurfaceAnalysisElevation);

        }
    }
}
