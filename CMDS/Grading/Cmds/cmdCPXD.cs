using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

using Autodesk.Civil.DatabaseServices;

using Base_Tools45;
using System.Collections.Generic;

namespace Grading.Cmds
{
    public static class cmdCPXD
    {
        public static void
        checkPntXData()
        {
            using (BaseObjs._acadDoc.LockDocument())
            {
                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        TypedValue[] TVs = new TypedValue[1];
                        TVs.SetValue(new TypedValue((int)DxfCode.Start, RXClass.GetClass(typeof(CogoPoint)).DxfName), 0);
                        SelectionSet ss = Select.buildSSet(TVs);
                        if (ss != null)
                        {
                            foreach (ObjectId id in ss.GetObjectIds())
                            {
                                ResultBuffer rb = null;

                                if (id.IsValid)
                                {
                                    rb = id.getXData(apps.lnkBrks);
                                    if (rb != null)
                                    {
                                        List<Handle> handles = rb.rb_handles();
                                        Handle hZero = "0".stringToHandle();
                                        if (handles.Contains(hZero))
                                            handles.Remove(hZero);
                                    }
                                }
                            }
                        }
                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(ex.Message + " cmdCPXD.cs: line: 50");
                }
            }
        }
    }
}
