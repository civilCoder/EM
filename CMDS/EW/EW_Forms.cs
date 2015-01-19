using EW.Forms;

namespace EW
{
    public sealed class EW_Forms
    {
        private static readonly EW_Forms ewForms = new EW_Forms();

        public static EW_Forms ewFrms{
            get{
                return ewForms;
            }
        }

        private EW_Forms(){
            fBalanceSite = new frmBalanceSite();
            fBuildAreas = new frmBuildAreas();
            fEarthwork = new frmEarthwork();
            fGridSpacing = new frmGridSpacing();
            fProgressBar = new frmProgressBar();
            fSurfaces = new frmSurfaces();

            wBalanceSite = new winBalanceSite();
            wBuildAreas = new winBuildAreas();
            wEW = new winEW();
            wGridSpacing = new winGridSpacing();
            wInputs = new winInputs();
            wProgressBar = new winProgressBar();
            wSurfaces = new winSurfaces();
        }

        public frmBalanceSite fBalanceSite { get; set; }
        public frmBuildAreas fBuildAreas { get; set; }
        public frmEarthwork fEarthwork { get; set; }
        public frmGridSpacing fGridSpacing { get; set; }
        public frmProgressBar fProgressBar { get; set; }
        public frmSurfaces fSurfaces { get; set; }

        public winBalanceSite wBalanceSite { get; set; }
        public winBuildAreas wBuildAreas { get; set; }
        public winEW wEW { get; set; }
        public winGridSpacing wGridSpacing { get; set; }
        public winInputs wInputs { get; set; }
        public winProgressBar wProgressBar { get; set; }
        public winSurfaces wSurfaces { get; set; }
    }
}
