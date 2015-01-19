using Base_Tools45;

namespace Grading
{
    public static class Grading_CommandDefaults
    {
        private static Grading_Palette gPalette = Grading_Palette.gPalette;
        private static myForms.GradeSite fGrading = gPalette.pGrading;

        public static void
        initializeCommandDefaults()
        {
            fGrading.cmdRTr_Default = "R";
            Dict.setCmdDefault("cmdRTr", "cmdDefault", "R");

            fGrading.cmdRTr_Slope = "0.015";
            Dict.setCmdDefault("cmdRTr", "Slope", "0.015");

            fGrading.cmdRTr_DeltaZ = "-4.04";
            Dict.setCmdDefault("cmdRTr", "DeltaZ", "-4.04");


            fGrading.cmdRTd_Default = "D";
            Dict.setCmdDefault("cmdRTd", "cmdDefault", "D");

            fGrading.cmdRTd_Distance = "0";
            Dict.setCmdDefault("cmdRTd", "Distance", "0");

            fGrading.cmdRTd_Elevation = "0";
            Dict.setCmdDefault("cmdRTd", "Elevation", "0");

            fGrading.cmdRTd_Slope = "0.015";
            Dict.setCmdDefault("cmdRTd", "Slope", "0.015");


            fGrading.cmdRL_Default = "R";
            Dict.setCmdDefault("cmdRL", "cmdDefault", "R");

            fGrading.cmdRL_GRADE = "0.005";
            Dict.setCmdDefault("cmdRL", "GRADE", "0.005");

            fGrading.cmdRL_DELTAZ = "-4.04";
            Dict.setCmdDefault("cmdRL", "DELTAZ", "-4.04");


            fGrading.cmdBV_Control = "FL";
            Dict.setCmdDefault("cmdBV", "CONTROL", "FL");

            fGrading.cmdBV_Source = "POINTS";
            Dict.setCmdDefault("cmdBV", "SOURCE", "POINTS");

            fGrading.cmdBV_GutterDepth = "0.17";
            Dict.setCmdDefault("cmdBV", "GUTTERDEPTH", "0.17");

            fGrading.cmdBV_GutterWidth = "3.0";
            Dict.setCmdDefault("cmdBV", "GUTTERWIDTH", "3.0");


            fGrading.cmdFL_resBot = "FL";
            Dict.setCmdDefault("cmdFL", "resBot", "FL");

            fGrading.cmdG_resBot = "FL";
            Dict.setCmdDefault("cmdFL", "resTop", "TC");


            fGrading.cmdG_resBot = "FL";
            Dict.setCmdDefault("cmdG", "resBot", "FL");
            fGrading.cmdG_resTop = "TC";
            Dict.setCmdDefault("cmdG", "resTop", "TC");
            fGrading.cmdG_resCurb = "0.50";
            Dict.setCmdDefault("cmdG", "resCurb", "0.50");


            fGrading.cmdLD_resBot = "";
            Dict.setCmdDefault("cmdLD", "resBot", "");
            fGrading.cmdLD_resTop = "";
            Dict.setCmdDefault("cmdLD", "resTop", "");


            Dict.setCmdDefault("cmdSDS", "resPrefix", "FL");
            Dict.setCmdDefault("cmdSDS", "resSuffix", "TC");
            Dict.setCmdDefault("cmdSDS", "resDesc", "0.50");


            Dict.setCmdDefault("cmdSDE", "resPrefix", "STA");
            Dict.setCmdDefault("cmdSDE", "resSuffix", "BC");
            Dict.setCmdDefault("cmdSDE", "resDesc", "");
            Dict.setCmdDefault("cmdSDE", "resElevSuf", "INV");


            Dict.setCmdDefault("cmdSED", "resPrefix", "STA");
            Dict.setCmdDefault("cmdSED", "resSuffix", "BC");
            Dict.setCmdDefault("cmdSED", "resDesc", "");
            Dict.setCmdDefault("cmdSED", "resElevSuf", "INV");
        }
    }
}
