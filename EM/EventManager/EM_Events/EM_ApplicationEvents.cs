using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using LdrText;
using Bubble;
using DimPL;

using Base_Tools45;
namespace EventManager {
    /// <summary>
    /// EM_ApplicationEvents.
    /// </summary>
    public class EM_ApplicationEvents {
        public EM_ApplicationEvents() {
            try
            {
                Application.SystemVariableChanged += callback_SystemVariableChanged;
            }
            catch (System.Exception ex){
                BaseObjs.writeDebug(ex.Message + " EM_ApplicationEvents.cs: line: 20");
            }
        }

        private void callback_SystemVariableChanged(Object sender, Autodesk.AutoCAD.ApplicationServices.SystemVariableChangedEventArgs e) {
            //WriteLine(String.Format("SystemVariableChanged - {0}", e.Name));
            if (e.Name == "CANNOSCALE") {
                int scale = (int)HostApplicationServices.WorkingDatabase.Cannoscale.DrawingUnits;
                if (scale != EM_Global.DrawingUnits) {
                    EM_Global.DrawingUnits = scale;
                    LdrText_Scale.scaleLdrText(scale);
                    BB_LnkBub_Scale.updateSymbols(scale); 
                    DimPL_Scale.scaleDimPL(scale);
                }
            }
        }
    }// end of class EM_ApplicationEvents
}
