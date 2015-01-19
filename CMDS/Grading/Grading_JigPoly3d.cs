using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using gc = Grading.Grading_CalcBasePnt;

namespace Grading
{
    public class Grading_JigPoly3d : EntityJig
    {
        private Point3d _mTempPoint;

        private Polyline3d p3d;
        private int vtx;

        public Grading_JigPoly3d(Polyline3d ent, Polyline3d poly3d, int vertex)
            : base(ent)
        {
            p3d = poly3d;
            vtx = vertex;
            _mTempPoint = ent.StartPoint;
        }

        public static ObjectId
        jigLine(Point3d pnt3d, Polyline3d poly3d, int vertex)
        {
            ObjectId idPoly3d = ObjectId.Null;
            Editor ed = BaseObjs._editor;
            Point3dCollection pnts3d = new Point3dCollection { pnt3d, pnt3d };
            Polyline3d ent = new Polyline3d(Poly3dType.SimplePoly, pnts3d, false);
            ent.TransformBy(ed.CurrentUserCoordinateSystem);

            Grading_JigPoly3d jig = new Grading_JigPoly3d(ent, poly3d, vertex);
            PromptResult res = ed.Drag(jig);

            if (res.Status == PromptStatus.OK)
            {
                Transaction tr = BaseObjs.startTransactionDb();
                using (tr)
                {
                    BlockTableRecord ms = Blocks.getBlockTableRecordMS();
                    idPoly3d = ms.AppendEntity(jig.Entity);
                    tr.AddNewlyCreatedDBObject(jig.Entity, true);
                    tr.Commit();
                }
            }
            return idPoly3d;
        }

        protected override bool Update()
        {
            try
            {
                (Entity as Polyline3d).EndPoint = _mTempPoint;
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " Grading_JigPoly3d.cs: line: 58");
            }
            return true;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions opts = new JigPromptPointOptions("\nTarget Point Location (XY): (ESC to exit) ");
            Point3d pnt3dStartPoint = gc.calcBasePnt3d(_mTempPoint, p3d, vtx);

            opts.BasePoint = pnt3dStartPoint;
            opts.UseBasePoint = true;
            opts.UserInputControls = UserInputControls.Accept3dCoordinates | UserInputControls.AnyBlankTerminatesInput |
                                     UserInputControls.GovernedByOrthoMode | UserInputControls.GovernedByUCSDetect |
                                     UserInputControls.UseBasePointElevation | UserInputControls.InitialBlankTerminatesInput |
                                     UserInputControls.NullResponseAccepted;

            PromptPointResult res = prompts.AcquirePoint(opts);

            if (res.Status == PromptStatus.Cancel)
                return SamplerStatus.Cancel;

            if (res.Value.Equals(_mTempPoint))
            {
                return SamplerStatus.NoChange;
            }
            else
            {
                _mTempPoint = res.Value;
                return SamplerStatus.OK;
            }
        }
    }
}
