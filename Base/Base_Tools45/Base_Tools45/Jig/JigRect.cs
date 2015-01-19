using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;

namespace Base_Tools45.Jig
{
    public class JigRect : DrawJig
    {
        private Point3d pnt3dBase;
        private Point3d pnt3dRes;

        public JigRect(Point3d pnt3d)
        {
            pnt3dBase = pnt3d;
        }

        public Matrix3d UCS
        {
            get
            {
                return BaseObjs._editor.CurrentUserCoordinateSystem;
            }
        }

        public Point3dCollection corners
        {
            get
            {
                return new Point3dCollection { pnt3dBase, new Point3d(pnt3dRes.X, pnt3dBase.Y, 0), pnt3dRes, new Point3d(pnt3dBase.X, pnt3dRes.Y, 0), pnt3dBase };
            }
        }

        protected override bool WorldDraw(Autodesk.AutoCAD.GraphicsInterface.WorldDraw draw)
        {
            WorldGeometry wGeo = draw.Geometry;
            if (wGeo != null)
            {
                wGeo.PushModelTransform(UCS);
                wGeo.Polygon(corners);
                wGeo.PopModelTransform();
            }
            return true;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions prPntOpts = new JigPromptPointOptions("\nPick Corner 2: ");

            prPntOpts.UseBasePoint = false;

            PromptPointResult prResult = prompts.AcquirePoint(prPntOpts);

            if (prResult.Status == PromptStatus.Cancel || prResult.Status == PromptStatus.Error)
                return SamplerStatus.Cancel;

            Point3d pnt3dTmp = prResult.Value;
            pnt3dTmp = Db.wcsToUcs(pnt3dTmp);
            //.TransformBy(UCS.Inverse());
            //Point3d pnt3dTmp = prResult.Value;
            if (!pnt3dRes.IsEqualTo(pnt3dTmp, new Tolerance(1E-09, 1E-09)))
            {
                pnt3dRes = pnt3dTmp;
                return SamplerStatus.OK;
            }
            else
            {
                return SamplerStatus.NoChange;
            }
        }
    }
}