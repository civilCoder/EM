using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace Base_Tools45.Jig
{
    public class LeaderJig : EntityJig
    {
        private int mCurrentJigIndex = 0;
        private Point3d mLastVertex;

        private bool bNewPoint = true;

        public LeaderJig(Leader ent)
            : base(ent)
        {
            Entity.SetDatabaseDefaults();
            Entity.TransformBy(ucs);
            mLastVertex = ent.LastVertex;
        }

        public new Leader Entity
        {
            get
            {
                return base.Entity as Leader;
            }
        }

        private static Editor ed
        {
            get
            {
                return BaseObjs._editor;
            }
        }

        private Matrix3d ucs
        {
            get
            {
                return ed.CurrentUserCoordinateSystem;
            }
        }

        public static bool Jig(Leader ent)
        {
            bool test = false;
            LeaderJig jLdr = null;
            try
            {
                jLdr = new LeaderJig(ent);
                PromptResult pr;
                int index = 0;

                do
                {
                    pr = ed.Drag(jLdr);
                    if (pr.Status == PromptStatus.Keyword)
                    {
                    }
                    if (index++ < 1)
                    {
                        jLdr.Entity.RemoveLastVertex();
                        jLdr.mCurrentJigIndex--;
                    }
                    jLdr.bNewPoint = true;
                }
                while (pr.Status != PromptStatus.Cancel && pr.Status != PromptStatus.Error && pr.Status != PromptStatus.None);
                if (pr.Status == PromptStatus.Cancel || pr.Status == PromptStatus.Error)
                    test = false;
                else
                {
                    jLdr.Entity.RemoveLastVertex();
                    test = true;
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} JigLeader.cs: line: 66", ex.Message));
            }
            return test;
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <returns></returns>
        protected override bool Update()
        {
            try
            {
                if (bNewPoint)
                {
                    Entity.AppendVertex(mLastVertex);
                    bNewPoint = false;
                }
                else
                {
                    Entity.RemoveLastVertex();
                    Entity.AppendVertex(mLastVertex);
                }
                bNewPoint = true;
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} JigLeader.cs: line: 87", ex.Message));
            }
            return bNewPoint;
        }

        /// <summary>
        /// Samplers the specified prompts.
        /// </summary>
        /// <param name="prompts">The prompts.</param>
        /// <returns></returns>
        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions jigOpts = new JigPromptPointOptions("\nSelect next point: ");
            jigOpts.UserInputControls = UserInputControls.Accept3dCoordinates |
                                        UserInputControls.NullResponseAccepted |
                                        UserInputControls.GovernedByUCSDetect;

            PromptPointResult res = prompts.AcquirePoint(jigOpts);

            if (res.Status == PromptStatus.Cancel && res.Status == PromptStatus.Error)
                return SamplerStatus.Cancel;
            else if (res.Value.DistanceTo(mLastVertex) < 1)
                return SamplerStatus.NoChange;
            else
            {
                mLastVertex = res.Value;
                return SamplerStatus.OK;
            }
        }
    }
}