using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using System.Collections.Generic;
using System.IO;

namespace Grading.Cmds
{
    public static partial class cmdUC1
    {
        public static void
        updateControl()
        {
            myForms.updateCNTL fmUpdateCNTL = Grading_Palette.gPalette.pUpdateCNTL;

            string path = Path.GetDirectoryName(BaseObjs.docFullName);
            string name = BaseObjs.docName;
            string file = "";

            if (fmUpdateCNTL.TARGETDWGNAME == "" || fmUpdateCNTL.TARGETDWGNAME == null)
            {
                try
                {
                    file = FileManager.getFilesWithEditor("Select Control Drawing for Transfer.", "GETFILE", "Drawing (*.dwg)|*.dwg", path, "Message");
                    if (file == null)
                        return;
                }
                catch (System.Exception ex)
                {
                    BaseObjs.writeDebug(ex.Message + " cmdUC1.cs: line: 30");
                }
                path = Path.GetDirectoryName(file);
                name = Path.GetFileName(file);
            }
            else
            {
                path = fmUpdateCNTL.TARGETDWGPATH;
                name = fmUpdateCNTL.TARGETDWGNAME;
            }

            Grading_Palette.gPalette.showPalettes(vis: true, index: 5);
            Grading_Palette.gPalette.pGrading.Initialize_Form();

            int fileStatus = FileManager.getFileStatus(string.Format("{0}{1}{2}", path, @"\", name));
            switch (fileStatus)
            {
                case (int)filestatus.isOpenLocalReadOnly:
                    return;

                case (int)filestatus.isOpenLocal:  //set file active
                    fmUpdateCNTL.isOpen = true;
                    break;

                case (int)filestatus.isLocked:
                    return;

                case (int)filestatus.isAvailable:  //open the file
                    fmUpdateCNTL.TARGETDWGPATH = path;
                    fmUpdateCNTL.TARGETDWGNAME = name;
                    try
                    {
                        Grading_Palette.setfocus("Update CNTL");
                        fmUpdateCNTL.Focusable = true;
                        fmUpdateCNTL.IsEnabled = true;
                        fmUpdateCNTL.Focus();
                        fmUpdateCNTL.lblTarDwg.Content = name;
                    }
                    catch (System.Exception ex)
                    {
                        BaseObjs.writeDebug(ex.Message + " cmdUC1.cs: line: 70");
                    }
                    Grading_Palette.gPalette.pGrading.Focus();
                    break;
            }
        }

        public static string
        addEntToList()
        {
            BaseObjs.acadActivate();
            string layer = "";
            Point3d pnt3dPicked = Pub.pnt3dO;
            Entity ent = Base_Tools45.Select.selectEntity(typeof(Entity), "Select Entity (to get layer)", "No Entity Selected. Exiting... ", out pnt3dPicked);
            if (ent != null)
                layer = ent.Layer;
            return layer;
        }

        public static List<string>
        getLayers()
        {
            List<string> layers = new List<string> {
                "CURB",
                "FL",
                "FS",
                "GB",
                "GUTTER",
                "RAMP",
                "RISER",
                "SLOPE",
                "VG",
                "WALK",
                "WALL"
            };

            List<string> layersInDwg = Layer.layersInDwg();
            for (int i = 0; i < layersInDwg.Count; i++)
            {
                if (layersInDwg[i].Contains("|"))
                    layersInDwg.RemoveAt(i);
            }
            for (int i = 0; i < layers.Count; i++)
            {
                if (!layersInDwg.Contains(layers[i]))
                    layers.RemoveAt(i);
            }
            return layers;
        }
    }
}
