using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using System.Windows.Forms;
using Surface = Autodesk.Civil.DatabaseServices.Surface;

namespace Base_Tools45.C3D
{
    /// <summary>
    ///
    /// </summary>
    public static class Surf_Analysis
    {
        /// <summary>
        /// </summary>
        public static void doSurfaceAnalysis(string nameSurface, int steps, short[] colors, Polyline3d poly3d = null)
        {
            if (steps > colors.Length)
            {
                DialogResult dr =
                    MessageBox.Show("\nNumber of steps is greater than the number of colors. Proceed Y/N: ", "Alert",
                        MessageBoxButtons.YesNo);
                if (dr == DialogResult.No)
                    return;
                short[] colorsX = new short[steps];
                colors.CopyTo(colorsX, 0);
                int n = colors.Length;
                for (int i = n; i < steps; i++)
                {
                    colorsX[i] = colors[i - n];
                }

                colors = new short[steps];
                colorsX.CopyTo(colors, 0);
            }

            bool exists = false;
            ObjectId idsurface = Surf.getSurface(nameSurface, out exists);
            if (!exists)
            {
                return;
            }
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    TinSurface surface = (TinSurface)tr.GetObject(idsurface, OpenMode.ForWrite);
                    SurfaceAnalysisElevationData[] newData = CreateElevationRegions(surface, steps, colors);
                    surface.Analysis.SetElevationData(newData);

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} Surf_Analysis.cs: line: 49", ex.Message));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="steps"></param>
        /// <param name="colors"></param>
        /// <returns></returns>
        private static SurfaceAnalysisElevationData[]
        CreateElevationRegions(Surface surface, int steps, short[] colors)
        {
            GeneralSurfaceProperties props = surface.GetGeneralProperties();
            double minElevation = props.MinimumElevation;
            double maxElevation = props.MaximumElevation;
            double increment = (maxElevation - minElevation) / steps;

            SurfaceAnalysisElevationData[] newData = new SurfaceAnalysisElevationData[steps];

            for (int i = 0; i < steps; i++)
            {
                Color color = Color.FromColorIndex(ColorMethod.ByLayer, colors[i]);
                newData[i] = new SurfaceAnalysisElevationData(
                    minElevation + (increment * i),
                    minElevation + (increment * (i + 1)),
                    color);
            }
            return newData;
        }
    }
}