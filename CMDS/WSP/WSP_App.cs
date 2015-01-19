using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using Base_Tools45;
using Base_Tools45.C3D;
using System.Collections.Generic;
using System.IO;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

namespace WSP
{
    public static class WSP_App
    {
        public static void
        addWaterSurfaceProfile()
        {
            SelectionSet ss = Select.buildSSet(typeof(ProfileView), false, "Select Profile View");
            if (ss == null || ss.Count == 0)
                return;
            ObjectId[] ids = ss.GetObjectIds();

            ProfileView view = (ProfileView)ids[0].getEnt();
            ObjectId idAlign = view.AlignmentId;

            Entity ent = idAlign.getEnt();

            Prof.removeProfile(idAlign, "WSelev");
            ObjectId idLayer = Layer.manageLayers("PF-HGL");
            ObjectId idStyle = Prof_Style.getProfileStyle("WS");
            try
            {
                using (Transaction tr = BaseObjs.startTransactionDb())
                {
                    ProfileStyle style = (ProfileStyle)tr.GetObject(idStyle, OpenMode.ForWrite);
                    style.GetDisplayStyleProfile(ProfileDisplayStyleProfileType.Arrow).Visible = false;
                    style.GetDisplayStyleProfile(ProfileDisplayStyleProfileType.Line).Visible = true;
                    style.GetDisplayStyleProfile(ProfileDisplayStyleProfileType.Line).Layer = "PF-HGL";
                    style.GetDisplayStyleProfile(ProfileDisplayStyleProfileType.Line).LinetypeScale = 0.5;
                    style.GetDisplayStyleProfile(ProfileDisplayStyleProfileType.LineExtension).Visible = false;
                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} WSP_App.cs: line: 44", ex.Message));
            }

            ObjectId idStyleLabelSet = Prof_Style.getProfileLabelSetStyle("None");
            Profile prof = Prof.addProfileByLayout("WSelev", idAlign, idLayer, idStyle, idStyleLabelSet);

            string nameDwg = BaseObjs.docName;

            string defExt = "OUT";
            string title = "Select WSPG output file";

            string filter = "Text files (*.out)|*.out";
            //filter="Text files (*.txt)|*.txt|All files (*.*)|*.*"
            string defPath = string.Format("{0}{1}", @"\\Brianw-civil3d\xdrive\", nameDwg.Substring(0, 4));
            string nameFile = Dialog.OpenFileDialog(defExt, title, filter, defPath);

            StreamReader sr = new StreamReader(nameFile);

            bool start = false;
            string heading01 = "", heading02 = "";
            while (!sr.EndOfStream)
            {
                string buf = sr.ReadLine();
                if (buf.Contains("HEADING LINE NO 1"))
                {
                    sr.ReadLine();
                    buf = sr.ReadLine();
                    heading01 = buf.Trim();
                }
                if (buf.Contains("HEADING LINE NO 2"))
                {
                    sr.ReadLine();
                    buf = sr.ReadLine();
                    heading02 = buf.Trim();
                }
            }

            sr.Close();

            List<double> pnts = new List<double>();
            sr = new StreamReader(nameFile);
            double staPrior = 0.0;
            int k = -1;
            double resElev = 0;

            while (!sr.EndOfStream)
            {
                string buf = sr.ReadLine();
                if (buf.Length >= 6)
                    if (buf.Substring(0, 5) == "*****")
                    {
                        start = true;
                        buf = sr.ReadLine();
                    }
                if (buf.Length >= 10)
                    if (buf.Substring(0, 9) == "LICENSEE:")
                    {
                        start = false;
                    }

                if (buf.Contains(heading01))
                {
                    start = false;
                }

                if (start)
                {
                    if (buf.Length >= 38){

                        string elev = buf.Substring(27, 10);
                        if (double.TryParse(elev, out resElev))
                        {
                            k += 2;
                            pnts.Add(double.Parse(buf.Substring(0, 10).Trim()));
                            pnts.Add(double.Parse(buf.Substring(27, 10).Trim()));
                            if (pnts[k - 1] == staPrior)
                            {
                                pnts[k - 1] += 0.05;
                            }
                            staPrior = pnts[k - 1];
                        }                        
                    }
                }
            }
            sr.Close();

            try
            {
                using (Transaction t = BaseObjs.startTransactionDb())
                {
                    for (int i = 0; i < pnts.Count; i += 2)
                    {
                        prof.PVIs.AddPVI(pnts[i], pnts[i + 1]);
                    }
                    t.Commit();
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(string.Format("{0} WSP_App.cs: line: 125", ex.Message));
            }
        }
    }
}