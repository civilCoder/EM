using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

using System;

[assembly: ExtensionApplication(typeof(EW.EW_Startup))]

namespace EW
{
    public class EW_Startup : IExtensionApplication
    {
        public void Initialize(){
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("\nEW.dll Loaded");
           
        }

        public void Terminate() {
        }

    }
}
