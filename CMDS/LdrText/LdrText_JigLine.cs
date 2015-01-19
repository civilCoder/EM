using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Base_Tools45;


namespace LdrText
{
    public class LdrText_JigLine : EntityJig
    {
        private Point3d _mTempPoint = new Point3d();

        public LdrText_JigLine(Line ent)
            : base(ent)
        {
        }

        public static Line
        jigLine(Point3d pnt3d)
        {
            ObjectId idLine = ObjectId.Null;
            Editor ed = BaseObjs._editor;

            Line ent = new Line(pnt3d, pnt3d);
            ent.TransformBy(ed.CurrentUserCoordinateSystem);

            LdrText_JigLine jig = new LdrText_JigLine(ent);
            PromptResult res = ed.Drag(jig);

            if (res.Status == PromptStatus.OK)
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    BlockTableRecord ms = Blocks.getBlockTableRecordMS();
                    idLine = ms.AppendEntity(jig.Entity);
                    tr.AddNewlyCreatedDBObject(jig.Entity, true);
                    tr.Commit();
                }
            }
            return (Line)jig.Entity;
        }

        protected override bool Update()
        {
            try
            {
                (Entity as Line).EndPoint = _mTempPoint;
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " LdrText_JigLine.cs: line: 52");
            }
            return true;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions opts = new JigPromptPointOptions("\nNext point: ");
            opts.BasePoint = (Entity as Line).StartPoint;
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
