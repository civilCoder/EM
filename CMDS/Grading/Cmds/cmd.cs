using System;
using System.Linq;
using System.Windows.Forms;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Base_Tools45;

[assembly: CommandClass(typeof(Grading.Commands.cmd))]
namespace Grading.Commands {
    public class cmd {
        
        [CommandMethod("GRADE", CommandFlags.Session)]
        public void grade() {
            Grading_Palette.grdPalette.Grade();
            Grading_Palette.grdPalette.fGrading.Initialize_Form();            
        }

        [CommandMethod("BC")]
        public void BC() {
            try {
                cmdBC.buildBench(false);
            }
            catch (System.Exception ex){
                BaseObjs.writeDebug(ex.Message + " cmd.cs: line: 27");
            }
            finally {
            }
        }

        [CommandMethod("BG")]
        public void BG() {
                try {
                    cmdBC.buildBench(true);
                }
                catch (System.Exception ex){
                    BaseObjs.writeDebug(ex.Message + " cmd.cs: line: 39");
                }
                finally {
                }
        }

        [CommandMethod("BL")]
        public void BL() {
                try {
                    cmdMB.makeBreakline("lnkBrks", "BL");
                }
                catch (System.Exception ex){
                    BaseObjs.writeDebug(ex.Message + " cmd.cs: line: 51");
                }
                finally {
                }
        }

        [CommandMethod("BV")]
        public void BV() {
                try {
                    cmdBV.valleyGutter();
                }
                catch (System.Exception ex){
                    BaseObjs.writeDebug(ex.Message + " cmd.cs: line: 63");
                }
                finally {
                }
        }

        [CommandMethod("MB")]
        public void MB() {
                try {
                    cmdMB.makeBreakline("lnkBrks", "MB");
                }
                catch (System.Exception ex){
                    BaseObjs.writeDebug(ex.Message + " cmd.cs: line: 75");
                }
                finally {
                }
        }

        [CommandMethod("MBL")]
        public void MBL() {
                try {
                    cmdMB.makeBreakline("lnkBrks", "MBL");
                }
                catch (System.Exception ex){
                    BaseObjs.writeDebug(ex.Message + " cmd.cs: line: 87");
                }
                finally {
                }
        }

        [CommandMethod("RL")]
        public void RL() {
                try {
                    cmdBL.SetPointBySlopeWithBrkline("lnkBrks", "RL");
                }
                catch (System.Exception ex){
                    BaseObjs.writeDebug(ex.Message + " cmd.cs: line: 99");
                }
                finally {
                }
        }

        [CommandMethod("RTr")]
        public void RTr() {
                try {
                    cmdRTx.SetPointBySlopeFromRef3dPoly();
                }
                catch (System.Exception ex){
                    BaseObjs.writeDebug(ex.Message + " cmd.cs: line: 111");
                }
                finally {
                }
        }

        [CommandMethod("RTd")]
        public void RTd() {
                try {
                    cmdBL.SetPointBySlopeWithBrkline("lnkBrks", "RTd");
                }
                catch (System.Exception ex){
                    BaseObjs.writeDebug(ex.Message + " cmd.cs: line: 123");
                }
                finally {
                }
        }

        [CommandMethod("GB")]
        public void GB() {
                try {
                    cmdMB.makeBreakline("lnkBrks", "GB");
                }
                catch (System.Exception ex){
                    BaseObjs.writeDebug(ex.Message + " cmd.cs: line: 135");
                }
                finally {
                }
        }

        [CommandMethod("BFL")]
        public void BFL() {
                try {
                    cmdMB.makeBreakline("lnkBrks", "FL");
                }
                catch (System.Exception ex){
                    BaseObjs.writeDebug(ex.Message + " cmd.cs: line: 147");
                }
                finally {
                }
        }

        [CommandMethod("UBP")]
        public void UBP() {
                try {
                    cmdUBP.updateBrkLines_Pnts();
                }
                catch (System.Exception ex){
                    BaseObjs.writeDebug(ex.Message + " cmd.cs: line: 159");
                }
                finally {
                }
        }

        [CommandMethod("RB")]
        public void RB() {
                try {
                    cmdRB.rebuildCPNT_ON();
                }
                catch (System.Exception ex){
                    BaseObjs.writeDebug(ex.Message + " cmd.cs: line: 171");
                }
                finally {
                }
        }

        [CommandMethod("RB0")]
        public void RB0() {
                try {
                    cmdABL.removeBreaklinewWith0vertices();
                }
                catch (System.Exception ex){
                    BaseObjs.writeDebug(ex.Message + " cmd.cs: line: 183");
                }
                finally {
                }
        }

        [CommandMethod("RBL")]
        public void CBL() {
                try {
                    cmdABL.resetBreaklines();
                }
                catch (System.Exception ex){
                    BaseObjs.writeDebug(ex.Message + " cmd.cs: line: 195");
                }
                finally {
                }
        }

        [CommandMethod("UC")]
        public void UC() {
                try {
                    cmdUC.updateControl();
                }
                catch (System.Exception ex){
                    BaseObjs.writeDebug(ex.Message + " cmd.cs: line: 207");
                }
                finally {
                }
        }

        [CommandMethod("CPXD")]
        public void CPXD() {
                try {
                    cmdCPXD.checkPntXData();
                }
                catch (System.Exception ex){
                    BaseObjs.writeDebug(ex.Message + " cmd.cs: line: 219");
                }
                finally {
                }
        }

        [CommandMethod("DBL")]
        public void DBL() {
            var varResponse = MessageBox.Show("Warning:\r\r" + "Selecting YES will delete all 3dBreaklines on layer CPNT-BRKLINES\r" +
                                              " and remove all Breaklines from surface CPNT-ON.\r\r" +
                                              "                                        CONTINUE?", "DELETE BREAKLINES: SURFACE & DRAWING", MessageBoxButtons.OKCancel);
            if (varResponse == DialogResult.Yes) { 
                try {
                    using (BaseObjs._acadDoc.LockDocument()) {
                        cmdDBL.deleteBreaklinesInSurface("CPNT-ON");
                        cmdDBL.deleteBreaklinesInDwg();
                    }
                }
                catch (System.Exception ex){
                    BaseObjs.writeDebug(ex.Message + " cmd.cs: line: 238");
                }
                finally {
                }
            }else {
                return;
            }
        }
    }
}
