using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;


namespace Bubble
{
    public static class BB_Arrow
    {
        public static void
        cmdA(){
            PromptStatus ps;
            Point3d pnt3dBeg = UserInput.getPoint("\nSelect First Point: ", out ps, osMode: 0);
            if (ps != PromptStatus.OK)
                return;
            Color color = Misc.getColorByLayer();
            
            ObjectId idLdr = BB_JigSplineArrow.jigSplineArrow(pnt3dBeg, 0.09, "ARROW", color);


        }
    }
}
