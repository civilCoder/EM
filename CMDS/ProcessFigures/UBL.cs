using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Base_Tools45;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ProcessFigures
{
    public static class UBL
    {
        private const string TemplateCONT = @"R:\TSet\Template\CIVIL3D2010\Thienes_CONT.dwt";
        private const string TemplateTOPO = @"R:\TSet\Template\CIVIL3D2010\Thienes_TOPO_20.dwt";

        public static void
        updateBrkLines()
        {
            bool exists = false;
            ObjectId idDictHist = ObjectId.Null;
            try
            {
                idDictHist = Dict.getNamedDictionary("HISTORY", out exists);
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} UBL.cs: line: 23", ex.Message));
            }
            if (!exists)
            {
                MessageBox.Show("Dictionary HISTORY not present - drawing not compatible with this command. Exiting....");
                return;
            }

            ResultBuffer rb = Dict.getXRec(idDictHist, "lastEnt");
            if (rb == null)
                return;

            TypedValue[] tvs = rb.AsArray();
            string handleLast0 = tvs[0].Value.ToString();

            tvs[0] = new TypedValue((int)DxfCode.Start, "POLYLINE");

            SelectionSet ss = Select.buildSSetBase(tvs, true);
            SelectedObject ssObj = ss[ss.Count - 1];
            if (ssObj.GetType() == typeof(Polyline3d))
            {
                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        Polyline3d poly3d = (Polyline3d)tr.GetObject(ssObj.ObjectId, OpenMode.ForRead);
                        string handleLastX = poly3d.Handle.ToString();

                        if (Int32.Parse(handleLastX, System.Globalization.NumberStyles.HexNumber) > Int32.Parse(handleLast0, System.Globalization.NumberStyles.HexNumber))
                        {
                            tvs[0] = new TypedValue(1005, poly3d.Handle);
                            Dict.addXRec(idDictHist, "lastENT", tvs);

                            var response = MessageBox.Show("Transfer new Breaklines to TOPO and CONT dwgs?", "trANSFER BREAKLINES?", MessageBoxButtons.YesNo);
                            if (response == DialogResult.Yes)
                            {
                                List<ObjectId> idPolys3d = new List<ObjectId>();
                                foreach (SelectedObject sObj in ss)
                                {
                                    if (sObj.GetType() == typeof(Polyline3d))
                                    {
                                        idPolys3d.Add(sObj.ObjectId);
                                    }
                                }

                                TransferObjs.transferObjects(idPolys3d, TemplateCONT, "CONT");
                                TransferObjs.transferObjects(idPolys3d, TemplateTOPO, "TOPO");
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(string.Format("{0} UBL.cs: line: 69", ex.Message));
                }
            }
        }
    }
}