using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;

namespace Base_Tools45.Jig
{
    public class JigSpline : EntityJig
    {
        private Int32 mCurrentJigIndex = 3;
        private Autodesk.AutoCAD.Geometry.Point3d mLastVertex;

        private bool mNewPoint = true;

        public JigSpline(Spline ent)
            : base(ent)
        {
            Entity.SetDatabaseDefaults();
            Entity.TransformBy(ucs);
            mLastVertex = ent.GetFitPointAt(2);
        }

        public new Spline Entity
        {
            get
            {
                return base.Entity as Spline;
            }
        }

        private static Editor editor
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
                return editor.CurrentUserCoordinateSystem;
            }
        }

        public static bool Jig(Spline ent)
        {
            bool test = false;
            JigSpline sJig = null;
            try
            {
                sJig = new JigSpline(ent);
                PromptResult pr;
                int dummyIndex = 0;
                do
                {
                    pr = editor.Drag(sJig);
                    if (pr.Status == PromptStatus.Keyword)
                    {
                    }
                    if (dummyIndex++ < 2)
                    {
                        sJig.Entity.RemoveFitPointAt(0);
                        sJig.mCurrentJigIndex--;
                    }
                    sJig.mNewPoint = true;
                }
                while (pr.Status != PromptStatus.Cancel && pr.Status != PromptStatus.Error && pr.Status != PromptStatus.None);
                if (pr.Status == PromptStatus.Cancel || pr.Status == PromptStatus.Error)
                    test = false;
                else
                {
                    sJig.Entity.RemoveFitPointAt(sJig.mCurrentJigIndex - 1);
                    test = true;
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} JigSpline.cs: line: 65", ex.Message));
            }
            return test;
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <returns></returns>
        protected override bool Update()
        {
            bool test = false;
            try
            {
                if (mNewPoint)
                {
                    Entity.InsertFitPointAt(mCurrentJigIndex++, mLastVertex);
                    mNewPoint = false;
                }
                else
                {
                    Entity.InsertFitPointAt(mCurrentJigIndex, mLastVertex);
                    if (Entity.NumFitPoints > mCurrentJigIndex)
                        Entity.RemoveFitPointAt(mCurrentJigIndex - 1);
                }
                test = true;
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} JigSpline.cs: line: 88", ex.Message));
            }
            return test;
        }

        /// <summary>
        /// Samplers the specified prompts.
        /// </summary>
        /// <param name="prompts">The prompts.</param>
        /// <returns></returns>
        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions prOptions1 = new JigPromptPointOptions("\nFit point (Enter to finish): ");
            prOptions1.UserInputControls = UserInputControls.Accept3dCoordinates |
                                           UserInputControls.GovernedByUCSDetect |
                                           UserInputControls.NullResponseAccepted;

            PromptPointResult prResult1 = prompts.AcquirePoint(prOptions1);

            if (prResult1.Status == PromptStatus.Cancel && prResult1.Status == PromptStatus.Error)
                return SamplerStatus.Cancel;
            if (prResult1.Value.DistanceTo(mLastVertex) < 1)
                return SamplerStatus.NoChange;
            else
            {
                mLastVertex = prResult1.Value;
                return SamplerStatus.OK;
            }
        }
    }
}