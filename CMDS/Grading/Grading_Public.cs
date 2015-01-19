using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Grading
{
    public static class Grading_Public
    {
        public static Point3d pnt3d1 { get; set; }
        public static Point3d pnt3d2 { get; set; }
        public static Point3d pnt3dT { get; set; }
        public static Point3d pnt3dX { get; set; }

        public static Polyline3d poly3dRF { get; set; }

        public static int vertex { get; set; }

        public static ObjectId idLine { get; set; }

        public static bool shift { get; set; }
        public static bool inBounds { get; set; }
    }
}
