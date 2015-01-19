using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(Base_Tools45.Jig.JigPline2.PlineJig))]

namespace Base_Tools45.Jig
{
    public class JigLeader3 : EntityJig
    {
        private Point3dCollection pnts3d;
        private Point3d pnt3dTmp;
        private Plane plane;

        public JigLeader3(Matrix3d ucs)
            : base(new Leader())
        {
            pnts3d = new Point3dCollection();
            Point3d pnt3dOrigin = Pub.pnt3dO;

            Vector3d v3d = new Vector3d(0, 0, 1);
            v3d = v3d.TransformBy(ucs);
            plane = new Plane(pnt3dOrigin, v3d);

            Leader ldr = Entity as Leader;
            ldr.SetDatabaseDefaults();
            ldr.IsSplined = false;

            ldr.AppendVertex(pnt3dOrigin);
        }

        public Entity GetEntity()
        {
            return Entity;
        }

        public void AddLatestVertex()
        {
            pnts3d.Add(pnt3dTmp);
            Leader ldr = Entity as Leader;
            ldr.AppendVertex(pnt3dTmp);
        }

        public void RemoveLastVertex()
        {
            Leader ldr = Entity as Leader;
            ldr.RemoveLastVertex();
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <returns></returns>
        protected override bool Update()
        {
            Leader ldr = Entity as Leader;
            ldr.AppendVertex(pnt3dTmp);
            return true;
        }

        /// <summary>
        /// Samplers the specified prompts.
        /// </summary>
        /// <param name="prompts">The prompts.</param>
        /// <returns></returns>
        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions jigOpts = new JigPromptPointOptions();
            jigOpts.UserInputControls = (UserInputControls.Accept3dCoordinates |
                                         UserInputControls.NullResponseAccepted |
                                         UserInputControls.NoNegativeResponseAccepted);

            if (pnts3d.Count == 0)
            {
                jigOpts.Message = "\nPick start point of leader: ";
            }
            else if (pnts3d.Count > 0)
            {
                jigOpts.BasePoint = pnts3d.lastPoint();
                jigOpts.UseBasePoint = true;
                jigOpts.Message = "\nPick next vertex: ";
            }
            else
                return SamplerStatus.Cancel;

            PromptPointResult res = prompts.AcquirePoint(jigOpts);
            if (pnt3dTmp == res.Value)
                return SamplerStatus.NoChange;
            else if (res.Status == PromptStatus.OK)
            {
                pnt3dTmp = res.Value;
                return SamplerStatus.OK;
            }
            else
                return SamplerStatus.Cancel;
        }
    }
}