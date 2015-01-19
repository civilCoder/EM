using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

// (C) Copyright 2014 by
//
using Autodesk.AutoCAD.Runtime;
using Base_Tools45;

[assembly: CommandClass(typeof(fixGradeTagLinks.MyCommands))]

namespace fixGradeTagLinks
{
    public class MyCommands
    {
        //public MyCommands(){
        //    FormHandler fHandler = new FormHandler();
        //    fHandler.ed = BaseObjs._editor;
        //}
        //[CommandMethod("CLKS")]
        //public void cmdCLKS() {
        //    App.loadForm();
        //}

        [CommandMethod("CLKSS")]
        public void cmdCLKSS()
        {
            Editor ed = BaseObjs._editor;

            TypedValue[] tvs = new TypedValue[5];
            SelectionSet ss = null;

            tvs.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 0);
            tvs.SetValue(new TypedValue((int)DxfCode.Operator, "<OR"), 1);
            tvs.SetValue(new TypedValue((int)DxfCode.BlockName, "GradeTag"), 2);
            tvs.SetValue(new TypedValue((int)DxfCode.BlockName, "FlTag"), 3);
            tvs.SetValue(new TypedValue((int)DxfCode.Operator, "OR>"), 4);

            SelectionFilter sf = new SelectionFilter(tvs);
            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = "Select callout blocks to link: ";

            PromptSelectionResult psr = ed.GetSelection(pso, sf);
            if (psr.Status == PromptStatus.OK)
                ss = psr.Value;
            else
                return;

            App.executeCLKS(ss);
        }
    }
}