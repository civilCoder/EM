using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Base_Tools45;
using System;

namespace Grading
{
    public class Grading_JigDraw : DrawJig
    {
        public static void
        drawLine(Point3d pnt3d, Polyline3d poly3d, int vertex)
        {
            Editor ed = BaseObjs._editor;

            Grading_JigDraw jig = new Grading_JigDraw();
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            throw new NotImplementedException();
        }

        protected override bool WorldDraw(WorldDraw draw)
        {
            throw new NotImplementedException();
        }
    }
}
