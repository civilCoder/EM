using Autodesk.AutoCAD.EditorInput;
using Autodesk.Civil.DatabaseServices;
using System;
using System.Windows.Forms;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace Base_Tools45.Jig
{
    public class EM_MouseWheelMsgFilter : IMessageFilter
    {
        public static Int32 HighWord(Int32 word)
        {
            return word >> 16;
        }

        public bool PreFilterMessage(ref Message m)
        {
            bool result = false;
            if (m.Msg == 0x20a && m.WParam != IntPtr.Zero)
            { //WM_MOUSEWHEEL
                Int32 highword = HighWord((Int32)m.WParam);
                if (highword > 0)
                {
                    ElevationAdjuster.jigger.mElevAdj += ElevationAdjuster.elevAdjPerScroll;
                }
                else
                {
                    ElevationAdjuster.jigger.mElevAdj -= ElevationAdjuster.elevAdjPerScroll;
                }
                ElevationAdjuster.jigger.UpdateElevation();
            }
            return result;
        }
    }

    public class ElevationAdjuster : EntityJig
    {
        public static ElevationAdjuster jigger;

        public const double elevAdjPerScroll = 0.01;

        public int mCurJigFactorNumber = 1;
        public double mElevAdj = 0.0;
        private double mElevOffset;

        private EM_MouseWheelMsgFilter mMsgFilter;

        public ElevationAdjuster(Entity ent)
            : base(ent)
        {
            mMsgFilter = new EM_MouseWheelMsgFilter();

            System.Windows.Forms.Application.AddMessageFilter(mMsgFilter);
            mElevOffset = (ent as CogoPoint).Elevation;
        }

        ~ElevationAdjuster()
        {
            System.Windows.Forms.Application.RemoveMessageFilter(mMsgFilter);
        }

        public new CogoPoint Entity
        {
            get
            {
                return (base.Entity as CogoPoint);
            }
        }

        public static bool Jig(CogoPoint ent)
        {
            try
            {
                Editor ed = BaseObjs._editor;
                jigger = new ElevationAdjuster(ent);
                PromptResult pr;
                do
                {
                    pr = ed.Drag(jigger);

                    jigger.mCurJigFactorNumber++;
                }
                while (pr.Status != PromptStatus.Cancel &&
                       pr.Status != PromptStatus.Error &&
                       pr.Status != PromptStatus.Keyword &&
                       jigger.mCurJigFactorNumber <= 1);
                return pr.Status == PromptStatus.OK;
            }
            catch
            {
                return false;
            }
        }

        public void UpdateElevation()
        {
        }

        protected override bool Update()
        {
            switch (mCurJigFactorNumber)
            {
                case 1:
                    Entity.Elevation = mElevOffset + mElevAdj;
                    break;

                default:
                    return false;
            }
            return true;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            switch (mCurJigFactorNumber)
            {
                case 1:
                    JigPromptDistanceOptions opts = new JigPromptDistanceOptions("\nAdjust Elevation");
                    opts.BasePoint = Entity.Location;
                    opts.UseBasePoint = true;
                    PromptDoubleResult pdr = prompts.AcquireDistance(opts);
                    if (pdr.Status == PromptStatus.Cancel)
                        return SamplerStatus.Cancel;
                    if (pdr.Value.Equals(mElevAdj))
                        return SamplerStatus.NoChange;
                    else
                    {
                        mElevAdj = pdr.Value;
                        return SamplerStatus.OK;
                    }
                default:
                    break;
            }
            return SamplerStatus.OK;
        }
    }
}