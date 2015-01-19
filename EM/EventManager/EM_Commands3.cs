using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.Jig;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

//[assembly: CommandClass(typeof(EventManager.EM_Commands))]

namespace EventManager
{
    public partial class EM_Commands
    {

        [CommandMethod("//")]
        public void twoForwardSlash_x()
        {
            try
            {
                acedPostCommand("Ddvpoint\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 29");
            }
        }

        [CommandMethod(@"\")]
        public void oneBackslash_x()
        {
            try
            {
                acedPostCommand("(Ddchprop_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 42");
            }
        }

        [CommandMethod(@"\\")]
        public void twoBackslash_x()
        {
            try
            {
                acedPostCommand("dsettings\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 55");
            }
        }

        [CommandMethod("_arc")]
        public void _arc_x()
        {
            try
            {
                acedPostCommand("(_arc_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 68");
            }
        }

        [CommandMethod("1")]
        public void one_x()
        {
            try
            {
                acedPostCommand("line\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 81");
            }
        }

        [CommandMethod("2")]
        public void two_x()
        {
            try
            {
                acedPostCommand("rtpan\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 94");
            }
        }

        [CommandMethod("3")]
        public void three_x()
        {
            try
            {
                acedPostCommand("move\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 107");
            }
        }

        [CommandMethod("3D")]
        public void threeD_x()
        {
            try
            {
                acedPostCommand("line\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 120");
            }
        }

        [CommandMethod("A")]
        public void A_x()
        {
            try
            {
                //acedPostCommand("(c-Arrow_x)\r");
                Bubble.BB_Arrow.cmdA();
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 134");
            }
        }

        [CommandMethod("aa")]
        public void aa_x()
        {
            try
            {
                acedPostCommand("arc\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 147");
            }
        }

        [CommandMethod("ac")]
        public void ac_x()
        {
            try
            {
                acadApp.RunMacro("modCommands.PntData2CO");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/lblPntsMisc2015.dvb");
                acadApp.RunMacro("modCommands.PntData2CO");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 143", ex.Message));
            }
        }

        [CommandMethod("acds")]
        public void acds_x()
        {
            try
            {
                acadApp.RunMacro("m_InterpolatePoints.addCodes");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/interpPnts2015.dvb");
                acadApp.RunMacro("m_InterpolatePoints.addCodes");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 155", ex.Message));
            }
        }

        [CommandMethod("achk")]
        public void achk_x()
        {
            try
            {
                acedPostCommand("(achk_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 190");
            }
        }

        [CommandMethod("ae")]
        public void ae_x()
        {
            try
            {
                acedPostCommand(".ddatte\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 203");
            }
        }

        [CommandMethod("aelb")]
        public void aelb_x()
        {
            try
            {
                acedPostCommand("(aelb_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 216");
            }
        }

        [CommandMethod("aetx")]
        public void aetx_x()
        {
            try
            {
                acedPostCommand("(aetx_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 229");
            }
        }

        [CommandMethod("ags")]
        public void ags_x()
        {
            try
            {
                acedPostCommand("(ags_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 242");
            }
        }

        [CommandMethod("Ape")]
        public void Ape_x()
        {
            try
            {
                acedPostCommand("Aperture\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 255");
            }
        }

        [CommandMethod("Ar")]
        public void Ar_x()
        {
            try
            {
                acedPostCommand("Arc\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 268");
            }
        }

        [CommandMethod("arcgsd")]
        public void arcgsd_x()
        {
            try
            {
                acedPostCommand("(arcgsd_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 281");
            }
        }

        [CommandMethod("arcgse")]
        public void arcgse_x()
        {
            try
            {
                acedPostCommand("(arcgse_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 294");
            }
        }

        [CommandMethod("arcinfo")]
        public void arcinfo_x()
        {
            try
            {
                acedPostCommand("(arcinfo_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 307");
            }
        }

        [CommandMethod("arclen")]
        public void arclen_x()
        {
            try
            {
                acedPostCommand("(arclen_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 320");
            }
        }

        [CommandMethod("arctxt")]
        public void arctxt_x()
        {
            try
            {
                acedPostCommand("(arctxt_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 333");
            }
        }

        [CommandMethod("as")]
        public void as_x()
        {
            try
            {
                acedPostCommand("(asctext_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 346");
            }
        }

        [CommandMethod("astp")]
        public void astp_x()
        {
            try
            {
                acedPostCommand("(astp_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 359");
            }
        }

        [CommandMethod("At")]
        public void At_x()
        {
            try
            {
                acedPostCommand("Attdef\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 372");
            }
        }

        [CommandMethod("-At")]
        public void _At_x()
        {
            try
            {
                acedPostCommand("Attdef\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 385");
            }
        }

        [CommandMethod("Ate")]
        public void Ate_x()
        {
            try
            {
                acedPostCommand("Ddatte\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 398");
            }
        }

        [CommandMethod("-Ate")]
        public void _Ate_x()
        {
            try
            {
                acedPostCommand("Attedit\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 411");
            }
        }

        [CommandMethod("Att")]
        public void Att_x()
        {
            try
            {
                acedPostCommand("Attedit\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 424");
            }
        }

        [CommandMethod("avp")]
        public void avp_x()
        {
            try
            {
                acedPostCommand("(avp_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 437");
            }
        }

        [CommandMethod("az")]
        public void az_x()
        {
            try
            {
                acedPostCommand("(az_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 450");
            }
        }

        [CommandMethod("B")]
        public void B_x()
        {
            try
            {
                acedPostCommand("Break\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 463");
            }
        }

        [CommandMethod("Bf")]
        public void Bf_x()
        {
            try
            {
                acedPostCommand("(bf_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 476");
            }
        }

        [CommandMethod("bl0")]
        public void bl0_x()
        {
            try
            {
                acedPostCommand("(bl0_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 489");
            }
        }

        [CommandMethod("blc")]
        public void blc_x()
        {
            try
            {
                acadApp.RunMacro("m_main.checkBrkLineLinks");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/checkBreaklineLinks2015.dvb");
                acadApp.RunMacro("m_main.checkBrkLineLinks");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 417", ex.Message));
            }
        }

        [CommandMethod("bln")]
        public void bln_x()
        {
            try
            {
                acedPostCommand("(bln_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 517");
            }
        }

        [CommandMethod("BLO")]
        public void blo_x()
        {
            //try {
            //    acadApp.RunMacro("Module3.blo");
            //}
            //catch (System.Exception ex) {
            //    acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
            //    acadApp.RunMacro("Module3.blo");
            //    BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 439");
            //}
            Grading.Cmds.cmdBLO.BLO();
        }

        [CommandMethod("blog")]
        public void blog_x()
        {
            try
            {
                acadApp.RunMacro("Module3.blog");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module3.blog");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 451", ex.Message));
            }
        }

        [CommandMethod("Bm")]
        public void Bm_x()
        {
            try
            {
                acedPostCommand("Blipmode\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 559");
            }
        }

        [CommandMethod("brg")]
        public void brg_x()
        {
            try
            {
                acedPostCommand("(brg_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 572");
            }
        }

        [CommandMethod("Bt")]
        public void Bt_x()
        {
            try
            {
                acedPostCommand("btrim\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 585");
            }
        }

        [CommandMethod("bvc")]
        public void bvc_x()
        {
            try
            {
                acadApp.RunMacro("jdhtest.bv");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("jdhtest.bv");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 493", ex.Message));
            }
        }

        [CommandMethod("c2pl")]
        public void c2pl_x()
        {
            try
            {
                acedPostCommand("(c2pl_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 613");
            }
        }

        [CommandMethod("Ca")]
        public void Ca_x()
        {
            try
            {
                acedPostCommand("Ddatte\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 626");
            }
        }

        [CommandMethod("calcdiff")]
        public void calcdiff_x()
        {
            try
            {
                acedPostCommand("(calcdiff_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 639");
            }
        }

        [CommandMethod("cbp")]
        public void cbp_x()
        {
            try
            {
                acedPostCommand("(cbp_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 652");
            }
        }

        [CommandMethod("cbretn")]
        public void cbretn_x()
        {
            try
            {
                acedPostCommand("(cbretn_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 665");
            }
        }

        [CommandMethod("Cc")]
        public void Cc_x()
        {
            try
            {
                acedPostCommand("Change\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 678");
            }
        }

        [CommandMethod("cdz")]
        public void cdz_x()
        {
            try
            {
                acedPostCommand("(cdz_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 691");
            }
        }

        [CommandMethod("CE")]
        public void CE_x()
        {
            try
            {
                acedPostCommand("(chgelev_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 704");
            }
        }

        [CommandMethod("Cf")]
        public void Cf_x()
        {
            try
            {
                acedPostCommand("Chamfer\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 717");
            }
        }

        [CommandMethod("Cg")]
        public void Cg_x()
        {
            try
            {
                acedPostCommand("Change\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 730");
            }
        }

        [CommandMethod("cgps")]
        public void cgps_x()
        {
            try
            {
                acedPostCommand("(cgps_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 743");
            }
        }

        [CommandMethod("cgr")]
        public void cgr_x()
        {
            try
            {
                acedPostCommand("(cgr_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 756");
            }
        }

        [CommandMethod("CH")]
        public void CH_x()
        {
            try
            {
                acedPostCommand("(ddchprop_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 769");
            }
        }

        [CommandMethod("CHc")]
        public void CHc_x()
        {
            try
            {
                acedPostCommand("(chgcase_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 782");
            }
        }

        [CommandMethod("chgtext")]
        public void chgtext_x()
        {
            try
            {
                acedPostCommand("(chgtext_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 795");
            }
        }

        [CommandMethod("cht")]
        public void cht_x()
        {
            try
            {
                acedPostCommand("(cht_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 808");
            }
        }

        [CommandMethod("Ci")]
        public void Ci_x()
        {
            try
            {
                acedPostCommand("Circle\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 821");
            }
        }

        [CommandMethod("cl")]
        public void cl_x()
        {
            try
            {
                acedPostCommand("(cl_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 834");
            }
        }

        [CommandMethod("clean")]
        public void clean_x()
        {
            try
            {
                acedPostCommand("(clean_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 847");
            }
        }

        [CommandMethod("clo")]
        public void clo_x()
        {
            try
            {
                acedPostCommand("(clo_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 860");
            }
        }

        [CommandMethod("cmp")]
        public void cmp_x()
        {
            try
            {
                acedPostCommand("(cmp_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 873");
            }
        }

        [CommandMethod("conkml")]
        public void conkml_x()
        {
            try
            {
                acedPostCommand("(conkml_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 886");
            }
        }

        [CommandMethod("conusf")]
        public void conusf_x()
        {
            try
            {
                acedPostCommand("(conusf_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 899");
            }
        }

        [CommandMethod("conwgs")]
        public void conwgs_x()
        {
            try
            {
                acedPostCommand("(conwgs_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 912");
            }
        }

        [CommandMethod("conx")]
        public void conx_x()
        {
            try
            {
                acadApp.RunMacro("Module3.conx");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module3.conx");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 745", ex.Message));
            }
        }

        [CommandMethod("cpd")]
        public void cpd_x()
        {
            try
            {
                acadApp.RunMacro("ThisDrawing.doProjectDirectory");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/createProjDirectory2015.dvb");
                acadApp.RunMacro("ThisDrawing.doProjectDirectory");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 769", ex.Message));
            }
        }

        [CommandMethod("cpl")]
        public void cpl_x()
        {
            try
            {
                acedPostCommand("(cpl_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 955");
            }
        }

        [CommandMethod("CPT")]
        public void CPT_x()
        {
            try
            {
                acadApp.RunMacro("Module2.cpt");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module2.cpt");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 791", ex.Message));
            }
        }

        [CommandMethod("Cr")]
        public void Cr_x()
        {
            try
            {
                acedPostCommand("Color\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 983");
            }
        }

        [CommandMethod("cr2d")]
        public void cr2d_x()
        {
            try
            {
                acedPostCommand("(cr2d_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 996");
            }
        }

        [CommandMethod("crp")]
        public void crp_x()
        {
            try
            {
                acadApp.RunMacro("Module5.crp");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module5.crp");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 823", ex.Message));
            }
        }

        [CommandMethod("crss")]
        public void crss_x()
        {
            try
            {
                acedPostCommand("(crss_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1024");
            }
        }

        [CommandMethod("csht")]
        public void csht_x()
        {
            try
            {
                acedPostCommand("(csht_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1037");
            }
        }

        [CommandMethod("Ct")]
        public void Ct_x()
        {
            try
            {
                acedPostCommand("Ddedit\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1050");
            }
        }

        [CommandMethod("ctof")]
        public void ctof_x()
        {
            try
            {
                acedPostCommand("(ctof_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1063");
            }
        }

        [CommandMethod("cvloc")]
        public void cvloc_x()
        {
            try
            {
                acedPostCommand("(cvloc_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1076");
            }
        }

        [CommandMethod("D")]
        public void D_x()
        {
            try
            {
                acedPostCommand("Dist\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1089");
            }
        }

        [CommandMethod("da")]
        public void da_x()
        {
            try
            {
                acedPostCommand("(dimpl_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1102");
            }
        }

        [CommandMethod("db")]
        public void db_x()
        {
            try
            {
                acadApp.RunMacro("Module6.doBoundary");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module6.doBoundary");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 905", ex.Message));
            }
        }

        [CommandMethod("dd")]
        public void dd_x()
        {
            try
            {
                acedPostCommand("(ddmodify_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1130");
            }
        }

        [CommandMethod("ddc")]
        public void ddc_x()
        {
            try
            {
                acedPostCommand("(ddchprop_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1143");
            }
        }

        [CommandMethod("Ddd")]
        public void Ddd_x()
        {
            try
            {
                acedPostCommand("Ddim\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1156");
            }
        }

        [CommandMethod("Ddi")]
        public void Ddi_x()
        {
            try
            {
                acedPostCommand("Dim Diameter\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1169");
            }
        }

        [CommandMethod("Ddl")]
        public void Ddl_x()
        {
            try
            {
                acedPostCommand("Layer\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1182");
            }
        }

        [CommandMethod("ddm")]
        public void ddm_x()
        {
            try
            {
                acedPostCommand("(ddmodify_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1195");
            }
        }

        [CommandMethod("Ddo")]
        public void Ddo_x()
        {
            try
            {
                acedPostCommand("dsettings\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1208");
            }
        }

        [CommandMethod("Ddp")]
        public void Ddp_x()
        {
            try
            {
                acedPostCommand("Ddptype\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1221");
            }
        }

        [CommandMethod("Ddr")]
        public void Ddr_x()
        {
            try
            {
                acedPostCommand("rename\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1234");
            }
        }

        [CommandMethod("Dds")]
        public void Dds_x()
        {
            try
            {
                acedPostCommand("dsettings\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1247");
            }
        }

        [CommandMethod("Ddu")]
        public void Ddu_x()
        {
            try
            {
                acedPostCommand("Ddunits\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1260");
            }
        }

        [CommandMethod("Ddv")]
        public void Ddv_x()
        {
            try
            {
                acedPostCommand("view\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1273");
            }
        }

        [CommandMethod("-Ddv")]
        public void _Ddv_x()
        {
            try
            {
                acedPostCommand("View\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1286");
            }
        }

        [CommandMethod("Ddvp")]
        public void Ddvp_x()
        {
            try
            {
                acedPostCommand("Ddvpoint\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1299");
            }
        }

        [CommandMethod("deln")]
        public void deln_x()
        {
            try
            {
                acedPostCommand("(deln_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1312");
            }
        }

        [CommandMethod("depz")]
        public void depz_x()
        {
            try
            {
                acedPostCommand("(depz_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1325");
            }
        }

        [CommandMethod("desf")]
        public void desf_x()
        {
            try
            {
                acedPostCommand("(desf_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1338");
            }
        }

        [CommandMethod("detf")]
        public void detf_x()
        {
            try
            {
                acedPostCommand("(detf_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1351");
            }
        }

        [CommandMethod("df")]
        public void df_x()
        {
            try
            {
                acedPostCommand("dim leader\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1364");
            }
        }

        [CommandMethod("DGL")]
        public void DGL_x()
        {
            try
            {
                acadApp.RunMacro("Module4.DGL");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module4.DGL");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 1107", ex.Message));
            }
        }

        [CommandMethod("dh")]
        public void dh_x()
        {
            try
            {
                acedPostCommand("dim hor\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1392");
            }
        }

        [CommandMethod("draw_tof_pline")]
        public void draw_tof_pline_x()
        {
            try
            {
                acedPostCommand("(draw_tof_pline_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1405");
            }
        }

        [CommandMethod("draw_tow_pline")]
        public void draw_tow_pline_x()
        {
            try
            {
                acedPostCommand("(draw_tow_pline_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1418");
            }
        }

        [CommandMethod("drv")]
        public void drv_x()
        {
            try
            {
                acedPostCommand("(drv_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1431");
            }
        }

        [CommandMethod("ds")]
        public void ds_x()
        {
            try
            {
                acedPostCommand("(ds_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1444");
            }
        }

        [CommandMethod("dtmp")]
        public void dtmp_x()
        {
            try
            {
                acedPostCommand("(dtmp_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1457");
            }
        }

        [CommandMethod("dvt")]
        public void dvt_x()
        {
            try
            {
                acedPostCommand("(dvt_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1470");
            }
        }

        [CommandMethod("dvtw")]
        public void dvtw_x()
        {
            try
            {
                acedPostCommand("(dviewtw_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1483");
            }
        }

        [CommandMethod("Dw")]
        public void Dw_x()
        {
            try
            {
                acedPostCommand("Dwgname\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1496");
            }
        }

        [CommandMethod("e3p")]
        public void e3p_x()
        {
            try
            {
                acedPostCommand("(e3p_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1509");
            }
        }

        [CommandMethod("ec")]
        public void ec_x()
        {
            try
            {
                acedPostCommand("erase c\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1522");
            }
        }

        [CommandMethod("ecc")]
        public void ecc_x()
        {
            try
            {
                acedPostCommand("(ecc_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1535");
            }
        }

        [CommandMethod("edc")]
        public void edc_x()
        {
            try
            {
                acadApp.RunMacro("modCommands.lblEdit1");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/lblPntsMisc2015.dvb");
                acadApp.RunMacro("modCommands.lblEdit1");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 1239", ex.Message));
            }
        }

        [CommandMethod("eet")]
        public void eet_x()
        {
            try
            {
                acedPostCommand("(eet_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1563");
            }
        }

        [CommandMethod("eff")]
        public void eff_x()
        {
            try
            {
                acadApp.RunMacro("m_Commands.doTiltedPlane");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/interpPnts2015.dvb");
                acadApp.RunMacro("m_Commands.doTiltedPlane");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 1261", ex.Message));
            }
        }

        [CommandMethod("elp")]
        public void elp_x()
        {
            try
            {
                acedPostCommand("(elp_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1591");
            }
        }

        [CommandMethod("et")]
        public void et_x()
        {
            try
            {
                acedPostCommand("(et_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1604");
            }
        }

        [CommandMethod("evc")]
        public void evc_x()
        {
            try
            {
                acedPostCommand("(evc_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1617");
            }
        }

        [CommandMethod("ew")]
        public void ew_x()
        {
            try
            {
                acadApp.RunMacro("ThisDrawing.doEarthwork");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/earthwork2015.dvb");
                acadApp.RunMacro("ThisDrawing.doEarthwork");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 1303", ex.Message));
            }
        }

        [CommandMethod("EWX")]
        public static void cmdEWX()
        {
            try
            {
                //EW.EW_Main.runEW();
                EW.Forms.winEW wEW = EW.EW_Forms.ewFrms.wEW;
                Application.ShowModelessWindow(wEW);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1647");
            }
        }


        [CommandMethod("Ex")]
        public void Ex_x()
        {
            try
            {
                acedPostCommand("Explode\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1661");
            }
        }

        [CommandMethod("f0")]
        public void f0_x()
        {
            try
            {
                acedPostCommand("fillet r 0 fillet\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1674");
            }
        }

        [CommandMethod("fdl")]
        public void fdl_x()
        {
            try
            {
                acedPostCommand("(fdl_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1687");
            }
        }

        [CommandMethod("fdll")]
        public void fdll_x()
        {
            try
            {
                acedPostCommand("(fdll_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1700");
            }
        }

        [CommandMethod("ff2")]
        public void ff2_x()
        {
            try
            {
                acadApp.RunMacro("Module2.ff2");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module2.ff2");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 1355", ex.Message));
            }
        }

        [CommandMethod("fftag")]
        public void fftag_x()
        {
            try
            {
                acedPostCommand("(fftag_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1728");
            }
        }

        [CommandMethod("Fi")]
        public void Fi_x()
        {
            try
            {
                acedPostCommand("Filter\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1741");
            }
        }

        [CommandMethod("fl2")]
        public void fl2_x()
        {
            try
            {
                acadApp.RunMacro("Module2.fl2");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module2.fl2");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 1387", ex.Message));
            }
        }

        [CommandMethod("fln")]
        public void fln_x()
        {
            try
            {
                acedPostCommand("(fln_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1769");
            }
        }

        [CommandMethod("fp")]
        public void fp_x()
        {
            try
            {
                acedPostCommand("(fp_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1782");
            }
        }

        [CommandMethod("g2")]
        public void g2_x()
        {
            try
            {
                acadApp.RunMacro("Module2.g2");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module2.g2");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 1419", ex.Message));
            }
        }

        [CommandMethod("gc")]
        public void gc_x()
        {
            try
            {
                acadApp.RunMacro("ThisDrawing.modXRefProp");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/modXRefProps2015.dvb");
                acadApp.RunMacro("ThisDrawing.modXRefProp");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 1431", ex.Message));
            }
        }

        [CommandMethod("ge")]
        public void ge_x()
        {
            try
            {
                acedPostCommand("(gedit_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1825");
            }
        }

        [CommandMethod("Gp")]
        public void Gp_x()
        {
            try
            {
                acedPostCommand("Group\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1838");
            }
        }

        [CommandMethod("-Gp")]
        public void _Gp_x()
        {
            try
            {
                acedPostCommand("Group\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1851");
            }
        }

        [CommandMethod("gpt")]
        public void gpt_x()
        {
            try
            {
                acedPostCommand("(gpt_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1864");
            }
        }

        [CommandMethod("grsym")]
        public void grsym_x()
        {
            try
            {
                acedPostCommand("(grsym_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1877");
            }
        }

        [CommandMethod("gt")]
        public void gt_x()
        {
            try
            {
                acadApp.RunMacro("Module2.gt");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module2.gt");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 1493", ex.Message));
            }
        }

        [CommandMethod("gtag")]
        public void gtag_x()
        {
            try
            {
                acedPostCommand("(gtag_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1905");
            }
        }

        [CommandMethod("gx")]
        public void gx_x()
        {
            try
            {
                acadApp.RunMacro("Module2.gx");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module2.gx");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 1515", ex.Message));
            }
        }

        [CommandMethod("h")]
        public void h_x()
        {
            try
            {
                acedPostCommand("bhatch\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1933");
            }
        }

        [CommandMethod("HC")]
        public void HC_x()
        {
            try
            {
                acedPostCommand("(hcloud_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1946");
            }
        }

        [CommandMethod("Ht")]
        public void Ht_x()
        {
            try
            {
                acedPostCommand("Highlight\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1959");
            }
        }

        [CommandMethod("Ia")]
        public void Ia_x()
        {
            try
            {
                acedPostCommand("Astag\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1972");
            }
        }

        [CommandMethod("Ib")]
        public void Ib_x()
        {
            try
            {
                acedPostCommand("Insbase\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1985");
            }
        }

        [CommandMethod("idpt")]
        public void idpt_x()
        {
            try
            {
                acedPostCommand("(idpt_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 1998");
            }
        }

        [CommandMethod("inv-c-arrow")]
        public void inv_c_arrow_x()
        {
            try
            {
                acedPostCommand("(inv-c-arrow_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2011");
            }
        }

        [CommandMethod("ip")]
        public void ip_x()
        {
            try
            {
                acadApp.RunMacro("m_InterpolatePoints.interpolatePoints");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/interpPnts2015.dvb");
                acadApp.RunMacro("m_InterpolatePoints.interpolatePoints");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 1597", ex.Message));
            }
        }

        [CommandMethod("ipnts")]
        public void ipnts_x()
        {
            try
            {
                acedPostCommand("(ipnts_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2039");
            }
        }

        [CommandMethod("lc")]
        public void lc_x()
        {
            try
            {
                acadApp.RunMacro("modCommands.lblCopyCO");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/lblPntsMisc2015.dvb");
                acadApp.RunMacro("modCommands.lblCopyCO");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 1619", ex.Message));
            }
        }

        [CommandMethod("lcl")]
        public void lcl_x()
        {
            try
            {
                acedPostCommand("(lcl_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2067");
            }
        }

        [CommandMethod("ldsp")]
        public void ldsp_x()
        {
            try
            {
                acedPostCommand("(ldsp_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2080");
            }
        }

        [CommandMethod("ldtxt")]
        public void ldtxt_x()
        {
            try
            {
                acedPostCommand("(ldtxt_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2093");
            }
        }

        [CommandMethod("ldtxt2")]
        public void ldtxt2_x()
        {
            try
            {
                acedPostCommand("(ldtxt2_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2106");
            }
        }

        [CommandMethod("led")]
        public void led_x()
        {
            try
            {
                acadApp.RunMacro("modLabelElevDiff.labelElevDiff");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/lblElevDif2015.dvb");
                acadApp.RunMacro("modLabelElevDiff.labelElevDiff");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 1671", ex.Message));
            }
        }

        [CommandMethod("lf")]
        public void lf_x()
        {
            try
            {
                acedPostCommand("_layfrz\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2134");
            }
        }

        [CommandMethod("lfa")]
        public void lfa_x()
        {
            try
            {
                acedPostCommand("-layer f *\r\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2147");
            }
        }

        [CommandMethod("lg")]
        public void lg_x()
        {
            try
            {
                acedPostCommand("(latag_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2160");
            }
        }

        [CommandMethod("lgr")]
        public void lgr_x()
        {
            try
            {
                acedPostCommand("(lgr_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2173");
            }
        }

        [CommandMethod("LGT")]
        public void lgt_x()
        {
            try
            {
                acedPostCommand("(length_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2186");
            }
        }

        [CommandMethod("Ll")]
        public void Ll_x()
        {
            try
            {
                acedPostCommand("Layer\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2199");
            }
        }

        [CommandMethod("LLA")]
        public void cmdLLA()
        {
            try
            {
                LdrText.LdrText_ProcessCmds.ProcessCmds("cmdLLA");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2212");
            }
        }


        [CommandMethod("LLG")]
        public void cmdLLG()
        {
            try
            {
                LdrText.LdrText_ProcessCmds.ProcessCmds("cmdLLG");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2226");
            }
        }

        [CommandMethod("llm")]
        public void llm_x()
        {
            try
            {
                acedPostCommand("-layer m\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2239");
            }
        }

        [CommandMethod("lln")]
        public void lln_x()
        {
            try
            {
                acedPostCommand("-layer n\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2252");
            }
        }

        [CommandMethod("llt")]
        public void llt_x()
        {
            try
            {
                acedPostCommand("-layer t\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2265");
            }
        }

        [CommandMethod("Lm")]
        public void Lm_x()
        {
            try
            {
                acedPostCommand("Limits\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2278");
            }
        }

        [CommandMethod("ln")]
        public void ln_x()
        {
            try
            {
                acedPostCommand("(ln_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2291");
            }
        }

        [CommandMethod("lnk")]
        public void lnk_x()
        {
            try
            {
                acadApp.RunMacro("Module3.lnk");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module3.lnk");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 1803", ex.Message));
            }
        }

        [CommandMethod("Lo")]
        public void Lo_x()
        {
            try
            {
                acedPostCommand("Layon\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2319");
            }
        }

        [CommandMethod("lp")]
        public void lp_x()
        {
            try
            {
                acadApp.RunMacro("modCommands.lblMisc");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/lblPntsMisc2015.dvb");
                acadApp.RunMacro("modCommands.lblMisc");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 1825", ex.Message));
            }
        }

        [CommandMethod("lta")]
        public void lta_x()
        {
            try
            {
                acedPostCommand("-layer t *\r\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2347");
            }
        }

        [CommandMethod("ltg")]
        public void ltg_x()
        {
            try
            {
                acedPostCommand("(ltg_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2360");
            }
        }

        [CommandMethod("mal")]
        public void mal_x()
        {
            try
            {
                acedPostCommand("(mal_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2373");
            }
        }

        [CommandMethod("mbrk")]
        public void mbrk_x()
        {
            try
            {
                acadApp.RunMacro("ThisDrawing.make3DPolys");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/make3dPoly2015.dvb");
                acadApp.RunMacro("ThisDrawing.make3DPolys");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 1867", ex.Message));
            }
        }

        [CommandMethod("mc")]
        public void mc_x()
        {
            try
            {
                acedPostCommand("move c\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2401");
            }
        }

        [CommandMethod("mex")]
        public void mex_x()
        {
            try
            {
                acedPostCommand("(mex_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2414");
            }
        }

        [CommandMethod("mgb")]
        public void mgb_x()
        {
            try
            {
                acedPostCommand("(mgb_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2427");
            }
        }

        [CommandMethod("mj")]
        public void mj_x()
        {
            try
            {
                acadApp.RunMacro("main.move2JunkLayer");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/acadUtil2015.dvb");
                acadApp.RunMacro("main.move2JunkLayer");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 1909", ex.Message));
            }
        }

        [CommandMethod("mj0")]
        public void mj0_x()
        {
            try
            {
                acedPostCommand("(mj0_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2455");
            }
        }

        [CommandMethod("mj1")]
        public void mj1_x()
        {
            try
            {
                acedPostCommand("(mj1_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2468");
            }
        }

        [CommandMethod("mj11")]
        public void mj11_x()
        {
            try
            {
                acedPostCommand("(mj11_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2481");
            }
        }

        [CommandMethod("mj12")]
        public void mj12_x()
        {
            try
            {
                acedPostCommand("(mj12_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2494");
            }
        }

        [CommandMethod("mj13")]
        public void mj13_x()
        {
            try
            {
                acedPostCommand("(mj13_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2507");
            }
        }

        [CommandMethod("mj14")]
        public void mj14_x()
        {
            try
            {
                acedPostCommand("(mj14_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2520");
            }
        }

        [CommandMethod("mj15")]
        public void mj15_x()
        {
            try
            {
                acedPostCommand("(mj15_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2533");
            }
        }

        [CommandMethod("mj16")]
        public void mj16_x()
        {
            try
            {
                acedPostCommand("(mj16_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2546");
            }
        }

        [CommandMethod("mj17")]
        public void mj17_x()
        {
            try
            {
                acedPostCommand("(mj17_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2559");
            }
        }

        [CommandMethod("mj18")]
        public void mj18_x()
        {
            try
            {
                acedPostCommand("(mj18_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2572");
            }
        }

        [CommandMethod("mj19")]
        public void mj19_x()
        {
            try
            {
                acedPostCommand("(mj19_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2585");
            }
        }

        [CommandMethod("mj2")]
        public void mj2_x()
        {
            try
            {
                acedPostCommand("(mj2_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2598");
            }
        }

        [CommandMethod("mj3")]
        public void mj3_x()
        {
            try
            {
                acedPostCommand("(mj3_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2611");
            }
        }

        [CommandMethod("mj4")]
        public void mj4_x()
        {
            try
            {
                acedPostCommand("(mj4_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2624");
            }
        }

        [CommandMethod("mj5")]
        public void mj5_x()
        {
            try
            {
                acedPostCommand("(mj5_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2637");
            }
        }

        [CommandMethod("mj6")]
        public void mj6_x()
        {
            try
            {
                acedPostCommand("(mj6_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2650");
            }
        }

        [CommandMethod("mj7")]
        public void mj7_x()
        {
            try
            {
                acedPostCommand("(mj7_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2663");
            }
        }

        [CommandMethod("mj8")]
        public void mj8_x()
        {
            try
            {
                acedPostCommand("(mj8_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2676");
            }
        }

        [CommandMethod("mj9")]
        public void mj9_x()
        {
            try
            {
                acedPostCommand("(mj9_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2689");
            }
        }

        [CommandMethod("Mm")]
        public void Mm_x()
        {
            try
            {
                acedPostCommand("Mirror\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2702");
            }
        }

        [CommandMethod("Mn")]
        public void Mn_x()
        {
            try
            {
                acedPostCommand("Menu\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2715");
            }
        }

        [CommandMethod("Mnc")]
        public void Mnc_x()
        {
            try
            {
                acedPostCommand("Menuctl\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2728");
            }
        }

        [CommandMethod("mnp")]
        public void mnp_x()
        {
            try
            {
                acedPostCommand("(mnp_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2741");
            }
        }

        [CommandMethod("MNPX")]
        public void cmdMNPX()
        {
            MNP.frmMNP fMNP = MNP.MNP_Forms.fMNP;
            Application.ShowModelessDialog(fMNP);
        }

        [CommandMethod("MNP2")]
        public void cmdMNP()
        {
            try
            {
                Application.ShowModelessDialog(MNP.MNP_Forms.fMNP);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2761");
            }
        }

        [CommandMethod("mo")]
        public void mo_x()
        {
            try
            {
                acedPostCommand("(ddmodify_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2774");
            }
        }

        [CommandMethod("mot")]
        public void mot_x()
        {
            try
            {
                acedPostCommand("(modtxt_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2787");
            }
        }

        [CommandMethod("mp")]
        public void mp_x()
        {
            try
            {
                acedPostCommand("move p\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2800");
            }
        }

        [CommandMethod("mps")]
        public void mps_x()
        {
            try
            {
                acedPostCommand("(mps_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2813");
            }
        }

        [CommandMethod("Mrt")]
        public void Mrt_x()
        {
            try
            {
                acedPostCommand("mirrtext\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2826");
            }
        }

        [CommandMethod("mrw")]
        public void mrw_x()
        {
            try
            {
                acedPostCommand("(mrw_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2839");
            }
        }

        [CommandMethod("mrwc")]
        public void mrwc_x()
        {
            try
            {
                acedPostCommand("(mrwc_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2852");
            }
        }

        [CommandMethod("mse")]
        public void mse_x()
        {
            try
            {
                acedPostCommand("(mse_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2865");
            }
        }

        [CommandMethod("Mss")]
        public void Mss_x()
        {
            try
            {
                acedPostCommand("Measure\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2878");
            }
        }

        [CommandMethod("mst")]
        public void mst_x()
        {
            try
            {
                acedPostCommand("(mst_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2891");
            }
        }

        [CommandMethod("mtr")]
        public void mtr_x()
        {
            try
            {
                acedPostCommand("(mtr_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2904");
            }
        }

        [CommandMethod("mtre")]
        public void mtre_x()
        {
            try
            {
                acedPostCommand("(mtr_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2917");
            }
        }

        [CommandMethod("MUE")]
        public void MUE_x()
        {
            try
            {
                acadApp.RunMacro("Module5.MUE");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module5.MUE");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 2271", ex.Message));
            }
        }

        [CommandMethod("mw")]
        public void mw_x()
        {
            try
            {
                acedPostCommand("move w\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2945");
            }
        }

        [CommandMethod("MXR")]
        public void doMXR()
        {
            try
            {
                xRef.doMXR();
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2960");
            }
        }

        [CommandMethod("mz")]
        public void mz_x()
        {
            try
            {
                acedPostCommand("(mz_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2973");
            }
        }

        [CommandMethod("mz0")]
        public void mz0_x()
        {
            try
            {
                acedPostCommand("(mz0_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2986");
            }
        }

        [CommandMethod("N")]
        public void N_x()
        {
            try
            {
                acedPostCommand("Move\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 2999");
            }
        }

        [CommandMethod("ng")]
        public void ng_x()
        {
            try
            {
                //acadApp.RunMacro("m_ChangePntLayerAndStyle.changePntLayerAndStyle");
                Survey.cmdNG.NG();
            }
            catch (System.Exception ex)
            {
                //acadApp.LoadDVB("C:/TSet/VBA2015/interpPnts2015.dvb");
                //acadApp.RunMacro("m_ChangePntLayerAndStyle.changePntLayerAndStyle");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 2335", ex.Message));
            }
        }

        [CommandMethod("NSTG")]
        public void NSTG_x()
        {
            try
            {
                acadApp.RunMacro("Module4.null_structure_toggle");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module4.null_structure_toggle");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 2347", ex.Message));
            }
        }

        [CommandMethod("ol", CommandFlags.UsePickSet)]
        public void Ol_x()
        {
            try
            {
                List<string> layers = new List<string>();
                PromptSelectionResult psr = acadEditor.SelectImplied();
                SelectionSet ss;
                if (psr.Status == PromptStatus.OK)
                {
                    using (Transaction tr = acadDoc.Database.TransactionManager.StartTransaction())
                    {
                        ss = psr.Value;
                        foreach (SelectedObject sObj in ss)
                        {
                            Entity ent = (Entity)tr.GetObject(sObj.ObjectId, OpenMode.ForRead);
                            if (!layers.Contains(ent.Layer))
                            {
                                layers.Add(ent.Layer);
                            }
                        }

                        LayerTable lt = (LayerTable)tr.GetObject(acadDoc.Database.LayerTableId, OpenMode.ForRead);
                        foreach (ObjectId idLtr in lt)
                        {
                            LayerTableRecord ltr = (LayerTableRecord)tr.GetObject(idLtr, OpenMode.ForWrite);
                            if (layers.Contains(ltr.Name))
                            {
                                ltr.IsOff = false;
                            }
                            else
                            {
                                ltr.IsOff = true;
                            }
                        }
                        tr.Commit();
                    }
                }
                else
                {
                    acedPostCommand("Layiso S O\r\r");
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3079");
            }
        }

        [CommandMethod("oo")]
        public void oo_x()
        {
            try
            {
                acedPostCommand("open\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3092");
            }
        }

        [CommandMethod("op")]
        public void op_x()
        {
            try
            {
                acedPostCommand("open\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3105");
            }
        }

        [CommandMethod("opt")]
        public void opt_x()
        {
            try
            {
                acedPostCommand("OPTIONS\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3118");
            }
        }

        [CommandMethod("OPTS")]
        public void OPTS_x()
        {
            try
            {
                acedPostCommand("OPTIONS\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3131");
            }
        }

        [CommandMethod("ot")]
        public void ot_x()
        {
            try
            {
                acedPostCommand("(otla_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3144");
            }
        }

        [CommandMethod("otc")]
        public void otc_x()
        {
            try
            {
                acedPostCommand("(otc_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3157");
            }
        }

        [CommandMethod("otl")]
        public void otl_x()
        {
            try
            {
                acedPostCommand("(otl_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3170");
            }
        }

        [CommandMethod("otm")]
        public void otm_x()
        {
            try
            {
                acedPostCommand("(otladvd_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3183");
            }
        }

        [CommandMethod("P2P")]
        public void P2P_x()
        {
            try
            {
                acadApp.RunMacro("Module4.PipeToInsideEdge");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module4.PipeToInsideEdge");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 2475", ex.Message));
            }
        }

        [CommandMethod("padelev")]
        public void padelev_x()
        {
            try
            {
                acedPostCommand("(padelev_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3211");
            }
        }

        [CommandMethod("Pb")]
        public void Pb_x()
        {
            try
            {
                acedPostCommand("Pickbox\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3224");
            }
        }

        [CommandMethod("pc")]
        public void pc_x()
        {
            try
            {
                acadApp.RunMacro("ThisDrawing.Main");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/lblElevDiff_PadCert2015.dvb");
                acadApp.RunMacro("ThisDrawing.Main");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 2507", ex.Message));
            }
        }

        [CommandMethod("pd")]
        public void pd_x()
        {
            try
            {
                acedPostCommand("(paddiff_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3252");
            }
        }

        [CommandMethod("PDF")]
        public void PDF_x()
        {
            try
            {
                acadApp.RunMacro("Module1.pdf");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module1.pdf");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 2529", ex.Message));
            }
        }

        [CommandMethod("pfdms")]
        public void pfdms_x()
        {
            try
            {
                acedPostCommand("(pfdms_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3280");
            }
        }

        [CommandMethod("PFSED")]
        public void PFSED_x()
        {
            try
            {
                acadApp.RunMacro("Module4.pfsed");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module4.pfsed");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 2551", ex.Message));
            }
        }

        [CommandMethod("pfssdim")]
        public void pfssdim_x()
        {
            try
            {
                acedPostCommand("(pfssdim_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3308");
            }
        }

        [CommandMethod("PFSSDM")]
        public void PFSSDM_x()
        {
            try
            {
                acadApp.RunMacro("Module4.pfssdm");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module4.pfssdm");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 2573", ex.Message));
            }
        }

        [CommandMethod("PFSSDP")]
        public void PFSSDP_x()
        {
            try
            {
                acadApp.RunMacro("Module4.pfssdp");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module4.pfssdp");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 2585", ex.Message));
            }
        }

        [CommandMethod("PIP")]
        public void PIP_x()
        {
            try
            {
                acadApp.RunMacro("Module4.set_pip");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module4.set_pip");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 2597", ex.Message));
            }
        }

        [CommandMethod("plcd")]
        public void plcd_x()
        {
            try
            {
                acedPostCommand("(plcd_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3366");
            }
        }

        [CommandMethod("pld")]
        public void pld_x()
        {
            try
            {
                acedPostCommand("(pld_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3379");
            }
        }

        [CommandMethod("pllzd")]
        public void pllzd_x()
        {
            try
            {
                acedPostCommand("(pllzd_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3392");
            }
        }

        [CommandMethod("plm")]
        public void plm_x()
        {
            try
            {
                acedPostCommand("(plm_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3405");
            }
        }

        [CommandMethod("pln")]
        public void pln_x()
        {
            try
            {
                acedPostCommand("(pln_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3418");
            }
        }

        [CommandMethod("pln0")]
        public void pln0_x()
        {
            try
            {
                acedPostCommand("(pln0_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3431");
            }
        }

        [CommandMethod("plsb")]
        public void plsb_x()
        {
            try
            {
                acedPostCommand("(plssbeg_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3444");
            }
        }

        [CommandMethod("plsbd")]
        public void plsbd_x()
        {
            try
            {
                acedPostCommand("(plssbd_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3457");
            }
        }

        [CommandMethod("plse")]
        public void plse_x()
        {
            try
            {
                acedPostCommand("(plssend_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3470");
            }
        }

        [CommandMethod("plstart")]
        public void plstart_x()
        {
            try
            {
                acedPostCommand("(plstart_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3483");
            }
        }

        [CommandMethod("Plt")]
        public void Plt_x()
        {
            try
            {
                acedPostCommand("Psltscale\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3496");
            }
        }

        [CommandMethod("Po")]
        public void Po_x()
        {
            try
            {
                acedPostCommand(".Point\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3509");
            }
        }

        [CommandMethod("Pp")]
        public void Pp_x()
        {
            try
            {
                acedPostCommand("_.Plot\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3522");
            }
        }

        [CommandMethod("PPF")]
        public void PPF_x()
        {
            try
            {
                acadApp.RunMacro("Module5.PPF");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module5.PPF");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 2739", ex.Message));
            }
        }

        [CommandMethod("Pr")]
        public void Pr_x()
        {
            try
            {
                acedPostCommand("Preferences\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3550");
            }
        }

        [CommandMethod("prof")]
        public void prof_x()
        {
            try
            {
                acedPostCommand("(prof_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3563");
            }
        }

        [CommandMethod("prof2")]
        public void prof2_x()
        {
            try
            {
                acedPostCommand("(prof2_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3576");
            }
        }

        [CommandMethod("pt")]
        public void pt_x()
        {
            try
            {
                acadApp.RunMacro("modCommands.lblAtPL");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/lblPntsMisc2015.dvb");
                acadApp.RunMacro("modCommands.lblAtPL");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 2781", ex.Message));
            }
        }

        [CommandMethod("ptsym")]
        public void ptsym_x()
        {
            try
            {
                acedPostCommand("(ptsym_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3604");
            }
        }

        [CommandMethod("ptsym2")]
        public void ptsym2_x()
        {
            try
            {
                acedPostCommand("(ptsym2_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3617");
            }
        }

        [CommandMethod("Q")]
        public void Q_x()
        {
            try
            {
                acedPostCommand("Quit\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3630");
            }
        }

        [CommandMethod("qav")]
        public void qav_x()
        {
            try
            {
                acedPostCommand("(qav_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3643");
            }
        }

        [CommandMethod("qbd")]
        public void qbd_x()
        {
            try
            {
                acedPostCommand("(qbd_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3656");
            }
        }

        [CommandMethod("qf")]
        public void qf_x()
        {
            try
            {
                acedPostCommand("(qf_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3669");
            }
        }

        [CommandMethod("QL")]
        public void QL_x()
        {
            try
            {
                acedPostCommand("qleader\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3682");
            }
        }

        [CommandMethod("Qq")]
        public void Qq_x()
        {
            try
            {
                acedPostCommand("Dimaligned\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3695");
            }
        }

        [CommandMethod("Qs")]
        public void Qs_x()
        {
            try
            {
                acedPostCommand("Qsave\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3708");
            }
        }

        [CommandMethod("Qt")]
        public void Qt_x()
        {
            try
            {
                acedPostCommand("Qtext\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3721");
            }
        }

        [CommandMethod("R")]
        public void R_x()
        {
            try
            {
                acedPostCommand("Rotate\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3734");
            }
        }

        [CommandMethod("Ra")]
        public void Ra_x()
        {
            try
            {
                acedPostCommand("Regenauto\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3747");
            }
        }

        [CommandMethod("rc")]
        public void rc_x()
        {
            try
            {
                acedPostCommand("\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3760");
            }
        }

        [CommandMethod("Rd")]
        public void Rd_x()
        {
            try
            {
                acedPostCommand("Redraw\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3773");
            }
        }

        [CommandMethod("rdc")]
        public void rdc_x()
        {
            try
            {
                acadApp.RunMacro("modCommands.remDupCOs");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/lblPntsMisc2015.dvb");
                acadApp.RunMacro("modCommands.remDupCOs");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 2933", ex.Message));
            }
        }

        [CommandMethod("RDP")]
        public void RDP_x()
        {
            try
            {
                //acadApp.RunMacro("Module3.Rem_Dup_Pts");
                Grading.Cmds.cmdRDP.RDP();
            }
            catch (System.Exception ex)
            {
                //acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                //acadApp.RunMacro("Module3.Rem_Dup_Pts");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 2945", ex.Message));
            }
        }

        [CommandMethod("resetelv")]
        public void resetelv_x()
        {
            try
            {
                acedPostCommand("(resetelv_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3817");
            }
        }

        [CommandMethod("Rg")]
        public void Rg_x()
        {
            try
            {
                acedPostCommand("Regen\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3830");
            }
        }

        [CommandMethod("Rga")]
        public void Rga_x()
        {
            try
            {
                acedPostCommand("Regenall\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3843");
            }
        }

        //[CommandMethod("riser")]
        //public void riser_x() {
        //    try {
        //        //acedPostCommand("(riser_x)\r");
        //    }
        //    catch (System.Exception ex) {
        //          BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3853");
        //    }
        //}
        [CommandMethod("Rm")]
        public void Rm_x()
        {
            try
            {
                acedPostCommand("dsettings\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3865");
            }
        }

        [CommandMethod("RP")]
        public void RP_x()
        {
            try
            {
                acadApp.RunMacro("Module3.rp");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module3.rp");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3007", ex.Message));
            }
        }

        [CommandMethod("rpc")]
        public void rpc_x()
        {
            try
            {
                acadApp.RunMacro("ThisDrawing.rpc");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/surveyRPS2015.dvb");
                acadApp.RunMacro("ThisDrawing.rpc");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3019", ex.Message));
            }
        }

        [CommandMethod("rpd")]
        public void rpd_x()
        {
            try
            {
                acedPostCommand("(rpd_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 3908");
            }
        }

        [CommandMethod("RPFL")]
        public void RPFL_x()
        {
            try
            {
                acadApp.RunMacro("Module2.rpfl");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module2.rpfl");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3041", ex.Message));
            }
        }

        [CommandMethod("RPG")]
        public void RPG_x()
        {
            try
            {
                acadApp.RunMacro("Module2.rpg");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module2.rpg");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3053", ex.Message));
            }
        }

        [CommandMethod("RPI")]
        public void RPI_x()
        {
            try
            {
                acadApp.RunMacro("Module2.ResetPointIndex");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module2.ResetPointIndex");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3065", ex.Message));
            }
        }

        [CommandMethod("rpp")]
        public void rpp_x()
        {
            try
            {
                acadApp.RunMacro("ThisDrawing.rpp");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/surveyRPS2015.dvb");
                acadApp.RunMacro("ThisDrawing.rpp");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3077", ex.Message));
            }
        }

        [CommandMethod("RPS")]
        public void RPS_x()
        {
            try
            {
                acadApp.RunMacro("Module3.rps");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module3.rps");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3089", ex.Message));
            }
        }

        [CommandMethod("RPX")]
        public void RPX_x()
        {
            try
            {
                acadApp.RunMacro("Module3.rpx");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module3.rpx");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3101", ex.Message));
            }
        }

        [CommandMethod("rs")]
        public void rs_x()
        {
            try
            {
                acadApp.RunMacro("modCommands.lblRescale");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/lblPntsMisc2015.dvb");
                acadApp.RunMacro("modCommands.lblRescale");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3113", ex.Message));
            }
        }

        [CommandMethod("rts")]
        public void rts_x()
        {
            try
            {
                acadApp.RunMacro("Module2.rts");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module2.rts");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3125", ex.Message));
            }
        }

        [CommandMethod("rtsd")]
        public void rtsd_x()
        {
            try
            {
                acadApp.RunMacro("Module2.rtsd");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module2.rtsd");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3137", ex.Message));
            }
        }

        [CommandMethod("rtse")]
        public void rtse_x()
        {
            try
            {
                acadApp.RunMacro("Module2.rtse");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module2.rtse");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3149", ex.Message));
            }
        }

        [CommandMethod("rtst")]
        public void rtst_x()
        {
            try
            {
                acadApp.RunMacro("Module2.rtst");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module2.rtst");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3161", ex.Message));
            }
        }

        [CommandMethod("rtx")]
        public void rtx_x()
        {
            try
            {
                acadApp.RunMacro("Module2.rtx");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module2.rtx");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3173", ex.Message));
            }
        }

        [CommandMethod("run_lcl")]
        public void run_lcl_x()
        {
            try
            {
                acedPostCommand("(run_lcl_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4101");
            }
        }

        [CommandMethod("Rw")]
        public void Rw_x()
        {
            try
            {
                acedPostCommand("Redraw\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4114");
            }
        }

        [CommandMethod("rwl")]
        public void rwl_x()
        {
            try
            {
                acadApp.RunMacro("TESTWALL.rwl");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("TESTWALL.rwl");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3205", ex.Message));
            }
        }

        [CommandMethod("rwp")]
        public void rwp_x()
        {
            try
            {
                acadApp.RunMacro("TESTWALL.rwp_main");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("TESTWALL.rwp_main");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3217", ex.Message));
            }
        }

        [CommandMethod("rwpl")]
        public void rwpl_x()
        {
            try
            {
                acadApp.RunMacro("TESTWALL.rwpl");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("TESTWALL.rwpl");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3229", ex.Message));
            }
        }

        [CommandMethod("s0")]
        public void s0_x()
        {
            try
            {
                acedPostCommand("(s0_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4172");
            }
        }

        [CommandMethod("s30")]
        public void s30_x()
        {
            try
            {
                acedPostCommand("(s30_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4185");
            }
        }

        [CommandMethod("s3p")]
        public void s3p_x()
        {
            try
            {
                acedPostCommand("(s3p_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4198");
            }
        }

        [CommandMethod("s45")]
        public void s45_x()
        {
            try
            {
                acedPostCommand("(s45_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4211");
            }
        }

        [CommandMethod("s60")]
        public void s60_x()
        {
            try
            {
                acedPostCommand("(s60_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4224");
            }
        }

        [CommandMethod("Sag")]
        public void Sag_x()
        {
            try
            {
                acedPostCommand("Snapang\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4237");
            }
        }

        [CommandMethod("SAL")]
        public void SAL_x()
        {
            try
            {
                acadApp.RunMacro("Module3.Show_All_Links");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module3.Show_All_Links");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3301", ex.Message));
            }
        }

        [CommandMethod("sap")]
        public void sap_x()
        {
            try
            {
                acadApp.RunMacro("ThisDrawing.doSAP");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/earthwork2015.dvb");
                acadApp.RunMacro("ThisDrawing.doSAP");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3313", ex.Message));
            }
        }

        [CommandMethod("sbl")]
        public void sbl_x()
        {
            try
            {
                acedPostCommand("(sbl_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4280");
            }
        }

        [CommandMethod("sbpl")]
        public void sbpl_x()
        {
            try
            {
                acedPostCommand("(sbpl_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4293");
            }
        }

        [CommandMethod("scgt")]
        public void scgt_x()
        {
            try
            {
                acedPostCommand("(scgt_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4306");
            }
        }

        [CommandMethod("sctp")]
        public void sctp_x()
        {
            try
            {
                acedPostCommand("(sctp_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4319");
            }
        }

        [CommandMethod("Sd")]
        public void Sd_x()
        {
            try
            {
                acedPostCommand("Solid\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4332");
            }
        }

        [CommandMethod("SDES")]
        public void SDES_x()
        {
            try
            {
                acadApp.RunMacro("Module5.SDE2");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module5.SDE2");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3375", ex.Message));
            }
        }

        [CommandMethod("SEDS")]
        public void SEDS_x()
        {
            try
            {
                acadApp.RunMacro("Module5.SED2");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module5.SED2");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3387", ex.Message));
            }
        }

        [CommandMethod("Serial")]
        public void Serial_x()
        {
            try
            {
                acedPostCommand("_Pkser\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4375");
            }
        }

        [CommandMethod("setBr")]
        public void setBr_x()
        {
            try
            {
                acadApp.RunMacro("Module2.setInterval");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module2.setInterval");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3409", ex.Message));
            }
        }

        [CommandMethod("setdesc")]
        public void setdesc_x()
        {
            try
            {
                acedPostCommand("(setdesc_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4403");
            }
        }

        [CommandMethod("setptcurv")]
        public void setptcurv_x()
        {
            try
            {
                acedPostCommand("(setptcurv_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4416");
            }
        }

        [CommandMethod("setrpt")]
        public void setrpt_x()
        {
            try
            {
                acedPostCommand("(setrpt_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4429");
            }
        }

        [CommandMethod("sf1")]
        public void sf1_x()
        {
            try
            {
                acedPostCommand("(sf1_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4442");
            }
        }

        [CommandMethod("sf2")]
        public void sf2_x()
        {
            try
            {
                acedPostCommand("(sf2_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4455");
            }
        }

        [CommandMethod("sgc")]
        public void sgc_x()
        {
            try
            {
                acedPostCommand("(sgc_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4468");
            }
        }

        [CommandMethod("sge")]
        public void sge_x()
        {
            try
            {
                acedPostCommand("(sge_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4481");
            }
        }

        [CommandMethod("sgu")]
        public void sgu_x()
        {
            try
            {
                acedPostCommand("(sgu_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4494");
            }
        }

        [CommandMethod("sl")]
        public void sl_x()
        {
            try
            {
                LdrText.LdrText_ProcessCmds.ProcessCmds("cmdSL");
                //acadApp.RunMacro("Module2.gs4ss");
            }
            catch (System.Exception ex)
            {
                //acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                //acadApp.RunMacro("Module2.gs4ss");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3501", ex.Message));
            }
        }

        [CommandMethod("slg")]
        public void slg_x()
        {
            try
            {
                acedPostCommand("(slg_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4523");
            }
        }

        [CommandMethod("Sli")]
        public void Sli_x()
        {
            try
            {
                acedPostCommand("Slice\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4536");
            }
        }

        [CommandMethod("slp")]
        public void slp_x()
        {
            try
            {
                acedPostCommand("(slp_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4549");
            }
        }

        [CommandMethod("slsym")]
        public void slsym_x()
        {
            try
            {
                acedPostCommand("(slsym_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4562");
            }
        }

        [CommandMethod("snp")]
        public void snp_x()
        {
            try
            {
                acadApp.RunMacro("ThisDrawing.setNextPointNum");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/setNextPoint2015.dvb");
                acadApp.RunMacro("ThisDrawing.setNextPointNum");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3553", ex.Message));
            }
        }

        //[CommandMethod("So")]
        //public void So_x() {
        //    try {
        //        acedPostCommand("Solid\r");
        //    }
        //    catch (System.Exception ex) {
        //           BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4587");
        //    }
        //}

        [CommandMethod("sp")]
        public void sp_x()
        {
            try
            {
                acedPostCommand("(sp_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4600");
            }
        }

        [CommandMethod("spc83")]
        public void spc83_x()
        {
            try
            {
                acedPostCommand("(spc83_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4613");
            }
        }

        [CommandMethod("spcc")]
        public void spcc_x()
        {
            try
            {
                acedPostCommand("(spcc_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4626");
            }
        }

        [CommandMethod("SPG")]
        public void cmdSPG()
        {
            try
            {
                Grading.Cmds.cmdSPG.SPG("cmdSPG");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4639");
            }
        }

        [CommandMethod("SPGS")]
        public void cmdSPGS()
        {
            try
            {
                Grading.Cmds.cmdSPG.SPG("cmdSPGS");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4652");
            }
        }

        [CommandMethod("spgx")]
        public void spgx_x()
        {
            try
            {
                acedPostCommand("(spgx_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4665");
            }
        }

        [CommandMethod("spi")]
        public void spi_x()
        {
            try
            {
                acadApp.RunMacro("Module2.SetPointIndex");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module2.SetPointIndex");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3625", ex.Message));
            }
        }

        [CommandMethod("spt")]
        public void spt_x()
        {
            try
            {
                acedPostCommand("(spt_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4693");
            }
        }

        [CommandMethod("sr")]
        public void sr_x()
        {
            try
            {
                acedPostCommand("(setrad_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4706");
            }
        }

        [CommandMethod("ss1")]
        public void ss1_x()
        {
            try
            {
                acedPostCommand("(ss1_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4719");
            }
        }

        [CommandMethod("ss2")]
        public void ss2_x()
        {
            try
            {
                acedPostCommand("(ss2_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4732");
            }
        }

        [CommandMethod("SSP")]
        public void cmdSSP()
        {
            try
            {
                Grading.Cmds.cmdSSP.SSP();
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4745");
            }
        }

        [CommandMethod("ssp2")]
        public void ssp2_x()
        {
            try
            {
                acedPostCommand("(ssp2_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4758");
            }
        }

        [CommandMethod("sspsd")]
        public void sspsd_x()
        {
            try
            {
                acedPostCommand("(sspsd_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4771");
            }
        }

        [CommandMethod("st")]
        public void st_x()
        {
            try
            {
                acedPostCommand("stretch c\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4784");
            }
        }

        [CommandMethod("stake")]
        public void stake_x()
        {
            try
            {
                acadApp.RunMacro("thisdrawing.Stake");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/Stake2015.dvb");
                acadApp.RunMacro("thisdrawing.Stake");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3717", ex.Message));
            }
        }

        [CommandMethod("stakeX")]
        public void cmdStakeX()
        {
            try
            {
                Application.ShowModelessDialog(Stake.Forms.Stake_Forms.sForms.fStake);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4812");
            }
        }

        [CommandMethod("STG")]
        public void STG_x()
        {
            try
            {
                acadApp.RunMacro("Module4.structure_toggle");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module4.structure_toggle");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3729", ex.Message));
            }
        }

        [CommandMethod("Stl")]
        public void Stl_x()
        {
            try
            {
                acedPostCommand("Style\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4840");
            }
        }

        [CommandMethod("Str")]
        public void Str_x()
        {
            try
            {
                acedPostCommand("Stretch\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4853");
            }
        }

        [CommandMethod("Sty")]
        public void Sty_x()
        {
            try
            {
                acedPostCommand("Style\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4866");
            }
        }

        [CommandMethod("Sv")]
        public void Sv_x()
        {
            try
            {
                acedPostCommand("Setvar\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4879");
            }
        }

        [CommandMethod("svol")]
        public void svol_x()
        {
            try
            {
                acedPostCommand("(svol_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4892");
            }
        }

        [CommandMethod("SW")]
        public void sw_x()
        {
            try
            {
                acadApp.RunMacro("Module6.surfaceDisplay");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module6.surfaceDisplay");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3791", ex.Message));
            }
        }

        [CommandMethod("T")]
        public void T_x()
        {
            try
            {
                acedPostCommand("Trim\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4920");
            }
        }

        [CommandMethod("tb3")]
        public void tb3_x()
        {
            try
            {
                acedPostCommand("(tb3_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4933");
            }
        }

        [CommandMethod("tc")]
        public void tc_x()
        {
            try
            {
                acadApp.RunMacro("modCommands.lblCFL");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/lblPntsMisc2015.dvb");
                acadApp.RunMacro("modCommands.lblCFL");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3823", ex.Message));
            }
        }

        [CommandMethod("tctag")]
        public void tctag_x()
        {
            try
            {
                acedPostCommand("(tctag_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4961");
            }
        }

        [CommandMethod("TE", CommandFlags.UsePickSet)]
        public void cmdTE()
        {
            try
            {
                BaseObjs.sendCommand("TextEdit\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4974");
            }
        }

        [CommandMethod("Ti")]
        public void Ti_x()
        {
            try
            {
                acedPostCommand("Time\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 4987");
            }
        }

        [CommandMethod("tl")]
        public void tl_x()
        {
            try
            {
                acedPostCommand("(tl_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5000");
            }
        }

        [CommandMethod("Tt")]
        public void Tt_x()
        {
            try
            {
                acedPostCommand("textfit\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5013");
            }
        }

        [CommandMethod("tx")]
        public void tx_x()
        {
            try
            {
                acedPostCommand("(tx_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5026");
            }
        }

        [CommandMethod("uas")]
        public void uas_x()
        {
            try
            {
                acedPostCommand("(uas_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5039");
            }
        }

        [CommandMethod("ucon")]
        public void ucon_x()
        {
            try
            {
                acedPostCommand("(ucon_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5052");
            }
        }

        [CommandMethod("Ucp")]
        public void Ucp_x()
        {
            try
            {
                acedPostCommand("Dducsp\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5065");
            }
        }

        [CommandMethod("Ui")]
        public void Ui_x()
        {
            try
            {
                acedPostCommand("Ucsicon\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5078");
            }
        }

        [CommandMethod("um")]
        public void um_x()
        {
            try
            {
                acedPostCommand("(um_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5091");
            }
        }

        [CommandMethod("uo")]
        public void uo_x()
        {
            try
            {
                acedPostCommand("(uo_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5104");
            }
        }

        [CommandMethod("Uol")]
        public void Uol_x()
        {
            try
            {
                acedPostCommand("layuniso\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5117");
            }
        }

        [CommandMethod("USAL")]
        public void USAL_x()
        {
            try
            {
                acadApp.RunMacro("Module3.Undo_Show_All_Links");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module3.Undo_Show_All_Links");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 3965", ex.Message));
            }
        }

        [CommandMethod("uts")]
        public void uts_x()
        {
            try
            {
                acedPostCommand("(utw_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5145");
            }
        }

        [CommandMethod("V")]
        public void V_x()
        {
            try
            {
                acedPostCommand("viewres\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5158");
            }
        }

        [CommandMethod("vba")]
        public void vba_x()
        {
            try
            {
                acedPostCommand("(vba_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5171");
            }
        }

        [CommandMethod("vga")]
        public void vga_x()
        {
            try
            {
                acedPostCommand("(vga_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5184");
            }
        }

        [CommandMethod("vgr")]
        public void vgr_x()
        {
            try
            {
                acedPostCommand("(vgr_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5197");
            }
        }

        [CommandMethod("vmm")]
        public void vmm_x()
        {
            try
            {
                acadApp.RunMacro("TESTWALL.vmaxmin");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("TESTWALL.vmaxmin");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 4027", ex.Message));
            }
        }

        [CommandMethod("vr")]
        public void vr_x()
        {
            try
            {
                acedPostCommand("view r\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5225");
            }
        }

        [CommandMethod("Vrt")]
        public void Vrt_x()
        {
            try
            {
                acedPostCommand("Visretain\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5238");
            }
        }

        [CommandMethod("vs")]
        public void vs_x()
        {
            try
            {
                acedPostCommand("view s\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5251");
            }
        }

        [CommandMethod("Vv")]
        public void Vv_x()
        {
            try
            {
                acedPostCommand("Ddvpoint\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5264");
            }
        }

        [CommandMethod("vw")]
        public void vw_x()
        {
            try
            {
                acedPostCommand("view w\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5277");
            }
        }

        [CommandMethod("w")]
        public void w_x()
        {
            try
            {
                Mod.whip();
                //acedPostCommand("(w_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5291");
            }
        }

        [CommandMethod("wa")]
        public void wa_x()
        {
            try
            {
                acedPostCommand("(wall_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5304");
            }
        }

        [CommandMethod("wac")]
        public void wac_x()
        {
            try
            {
                acedPostCommand("(wac_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5317");
            }
        }

        [CommandMethod("wgs83")]
        public void wgs83_x()
        {
            try
            {
                acedPostCommand("(wgs83_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5330");
            }
        }

        [CommandMethod("wgs83c")]
        public void wgs83c_x()
        {
            try
            {
                acedPostCommand("(wgs83c_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5343");
            }
        }

        [CommandMethod("wgs83d")]
        public void wgs83d_x()
        {
            try
            {
                acedPostCommand("(wgs83d_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5356");
            }
        }

        [CommandMethod("wgs83dc")]
        public void wgs83dc_x()
        {
            try
            {
                acedPostCommand("(wgs83dc_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5369");
            }
        }

        [CommandMethod("wgsc")]
        public void wgsc_x()
        {
            try
            {
                acedPostCommand("(wgsc_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5382");
            }
        }

        [CommandMethod("wgsd")]
        public void wgsd_x()
        {
            try
            {
                acedPostCommand("(wgsd_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5395");
            }
        }

        [CommandMethod("wgsi")]
        public void wgsi_x()
        {
            try
            {
                acedPostCommand("(wgsi_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5408");
            }
        }

        [CommandMethod("wid")]
        public void wid_x()
        {
            try
            {
                acedPostCommand("(wid_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5421");
            }
        }

        [CommandMethod("wid2")]
        public void wid2_x()
        {
            try
            {
                acedPostCommand("(wid2_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5434");
            }
        }

        [CommandMethod("wo")]
        public void wo_x()
        {
            try
            {
                acedPostCommand("(wo_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5447");
            }
        }

        [CommandMethod("wof")]
        public void wof_x()
        {
            try
            {
                acedPostCommand("(wof_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5460");
            }
        }

        [CommandMethod("wq0")]
        public void wq0_x()
        {
            try
            {
                acedPostCommand("(wq0_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5473");
            }
        }

        [CommandMethod("Ww")]
        public void Ww_x()
        {
            try
            {
                acedPostCommand("Wblock\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5486");
            }
        }

        [CommandMethod("X")]
        public void X_x()
        {
            try
            {
                acedPostCommand("Extend\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5499");
            }
        }

        [CommandMethod("XD")]
        public void XD_x()
        {
            try
            {
                acedPostCommand("_xdlist\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5512");
            }
        }

        [CommandMethod("XDC")]
        public void XDC_x()
        {
            try
            {
                Base_Tools45.xData.clearAllXdata();
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5525");
            }
        }

        [CommandMethod("FE")]
        public void cmdFE()
        {
            try
            {
                Base_Tools45.Misc.gotoEnt();
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5538");
            }
        }

        [CommandMethod("SSZ")]
        public void cmdSSZ()
        {
            try
            {
                LdrText.LdrText_ProcessCmds.ProcessCmds("cmdSSZ");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5551");
            }
        }

        [CommandMethod("GH")]
        public void cmdGH()
        {
            try
            {
                Base_Tools45.Misc.showHandle();
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5564");
            }
        }

        [CommandMethod("XF")]
        public void XF_x()
        {
            try
            {
                acedPostCommand("_layfrz\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5577");
            }
        }

        [CommandMethod("xfl")]
        public void xfl_x()
        {
            try
            {
                acadApp.RunMacro("Module2.XrFlowlineTag");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module2.XrFlowlineTag");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 4269", ex.Message));
            }
        }

        [CommandMethod("xg")]
        public void xg_x()
        {
            try
            {
                acadApp.RunMacro("Module2.XRGradeTag");
            }
            catch (System.Exception ex)
            {
                acadApp.LoadDVB("C:/TSet/VBA2015/CIVIL3D2015.DVB");
                acadApp.RunMacro("Module2.XRGradeTag");
                BaseObjs.writeDebug(string.Format("{0} EM_Commands3.cs: line: 4281", ex.Message));
            }
        }

        [CommandMethod("xhr")]
        public void xhr_x()
        {
            try
            {
                acedPostCommand("(xhr_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5620");
            }
        }

        [CommandMethod("XL")]
        public void XL_x()
        {
            try
            {
                acedPostCommand("_layoff\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5633");
            }
        }

        [CommandMethod("XLO")]
        public void XLO_x()
        {
            try
            {
                acedPostCommand("_layoff\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5646");
            }
        }

        [CommandMethod("xpnt")]
        public void xpnt_x()
        {
            try
            {
                acedPostCommand("(xpnt_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5659");
            }
        }

        [CommandMethod("xpt")]
        public void xpt_x()
        {
            try
            {
                acedPostCommand("(xpt_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5672");
            }
        }

        [CommandMethod("XR2B")]
        public void xr2b_x()
        {
            try
            {
                acedPostCommand("(xr2b_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5685");
            }
        }

        [CommandMethod("xsc")]
        public void xsc_x()
        {
            try
            {
                acedPostCommand("(xsc_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5698");
            }
        }

        [CommandMethod("xsl")]
        public void xsl_x()
        {
            try
            {
                acedPostCommand("(xsl_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5711");
            }
        }

        [CommandMethod("XX")]
        public void XX_x()
        {
            try
            {
                acedPostCommand("_externalreferencesclose\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5724");
            }
        }

        [CommandMethod("y")]
        public void y_x()
        {
            try
            {
                acedPostCommand("(ysym_x)\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5737");
            }
        }

        [CommandMethod("Za")]
        public void Za_x()
        {
            try
            {
                acedPostCommand("zoom 1.5X\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5750");
            }
        }

        [CommandMethod("Zx")]
        public void Zx_x()
        {
            try
            {
                acedPostCommand("Zoom e\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5763");
            }
        }

        [CommandMethod("Zz")]
        public void Zz_x()
        {
            try
            {
                acedPostCommand("zoom .5X\r");
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5776");
            }
        }

        [CommandMethod("so")]
        public void cmdSO()
        {
            PromptStatus ps;
            Point3d pnt3dB = UserInput.getPoint("Select Begin Point", out ps, osMode: 8);
            Point3d pnt3dE = UserInput.getPoint("Select End Point", out ps, osMode: 8);
            Point3d pnt3dX = UserInput.getPoint("Select Target Point", out ps, osMode: 8);

            staOffElev so = Geom.getStaOff(pnt3dB, pnt3dE, pnt3dX);

            Application.ShowAlertDialog(string.Format("Sta: {0:F3}\nOff: {1:F3}", so.staSeg, so.off));
        }

        [CommandMethod("testWheel")]
        public void cmdTestWheel()
        {
            try
            {
                Editor ed = BaseObjs._editor;
                Database db = BaseObjs._db;
                PromptEntityResult per = ed.GetEntity("Select Point to edit: ");
                if (per.Status == PromptStatus.OK)
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        CogoPoint cgPnt = (CogoPoint)tr.GetObject(per.ObjectId, OpenMode.ForWrite);
                        if (cgPnt != null)
                        {
                            if (ElevationAdjuster.Jig(cgPnt))
                                tr.Commit();
                            else
                                tr.Abort();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " EM_Commands3.cs: line: 5818");
            }
        }

        [CommandMethod("msite")]
        public void cmdMSITE()
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            bool escape = Select.getSelSetFromUser(out ids);
            if (escape)
                return;
            PromptStatus ps;
            Point3d pnt3dFrom = UserInput.getPoint("Select Base Location: ", out ps, 9);
            if (ps != PromptStatus.OK)
                return;
            Point3d pnt3dTo = UserInput.getPoint("Select Target Location: ", pnt3dFrom, out escape, out ps, 9);
            if (ps != PromptStatus.OK)
                return;

            ids.moveSite(pnt3dFrom, pnt3dTo);
        }

        [DllImport("accore.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl, EntryPoint = "?acedPostCommand@@YAHPEB_W@Z")]
        extern static private int acedPostCommand(string strExpr);
    }
}
