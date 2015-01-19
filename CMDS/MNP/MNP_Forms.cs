using Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: CommandClass(typeof(MNP.MNP_Forms))]

namespace MNP
{
    public class MNP_Forms
    {
        //[CommandMethod("MNPX")]
        //public void cmdMNPX()
        //{
        //    Application.ShowModelessDialog(fMNP);
        //}

        private static readonly MNP_Forms mnpForms = new MNP_Forms();

        private static readonly frmAlignEnts fmAlignEnts;
        private static readonly frmMNP fmMNP;

        static MNP_Forms()
        {
            fmAlignEnts = new frmAlignEnts();
            fmMNP = new frmMNP();
        }

        public static MNP_Forms mnp_Frms
        {
            get
            {
                return mnpForms;
            }
        }

        public static frmAlignEnts fAlignEnts
        {
            get
            {
                return fmAlignEnts;
            }
        }

        public static frmMNP fMNP
        {
            get
            {
                return fmMNP;
            }
        }

        //public static void
        //show_fMNP(){
        //    fMNP = null;
        //    Application.ShowModalDialog(fMNP);
        //}

        //public static void
        //show_fAlignEnts(){
        //    fAlignEnts = null;
        //    Application.ShowModalDialog(fAlignEnts);
        //}
    }
}