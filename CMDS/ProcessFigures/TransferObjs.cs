using Autodesk.AECC.Interop.Land;
using Autodesk.AECC.Interop.UiSurvey;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

using Base_Tools45;
using Base_VB;

using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ProcessFigures
{
    public static class TransferObjs
    {
        private const string TempPntFile = @"M:\SURVEY\CIVIL3D\tempPnts.txt";

        public static void
        transferObjects(List<ObjectId> idPolys3d, string dwgTemplate, string type, string namePntFile = null)
        {
            List<polyInfo> pInfo = new List<polyInfo>();
            List<string> layers = new List<string>();

            foreach (ObjectId idPoly3d in idPolys3d)
            {
                string nameLayer = idPoly3d.getLayer();
                polyInfo p = new polyInfo(p.pnts3d = idPoly3d.getCoordinates3d(), p.layer = nameLayer);
                layers.Add(nameLayer);
                if (!layers.Contains(nameLayer))
                {
                    Layer.manageLayers(nameLayer);
                }
            }

            Document acadDocX = BaseObjs._acadDoc;
            Document acadDocT = null;

            string pathX = Path.GetDirectoryName(acadDocX.Database.Filename);
            string nameX = Path.GetFileName(acadDocX.Database.Filename);

            string nameT = string.Format("{0}{1}{2}{3}.dwg", pathX, @"\", nameX.Substring(0, 4), type);

            if (File.Exists(nameT))
            {
                List<string> names = new List<string>();           //check to see if user has it open

                foreach (Document doc in BaseObjs._acadDocs)
                {
                    names.Add(Path.GetFileName(doc.Name));
                }

                if (names.Contains(Path.GetFileName(nameT)))
                {
                    foreach (Document doc in BaseObjs._acadDocs)
                    {
                        if (doc.Name == Path.GetFileName(nameT))
                        {
                            acadDocT = doc;
                            BaseObjs._acadDocs.MdiActiveDocument = acadDocT;
                            break;
                        }
                    }
                }
                else
                {
                    string nameDWL = string.Format("{0}{1}{2}.dwl", pathX, @"\", Path.GetFileNameWithoutExtension(nameX));
                    if (File.Exists(nameDWL))
                    {
                        StreamReader sr = new StreamReader(nameDWL);
                        string user = sr.ReadLine();
                        sr.Close();
                        string message = string.Format("{0} has {1}open. Select OK to Proceed when the file is available", user, nameT);
                        var result = MessageBox.Show(message, string.Format("{0} is open.", nameT), MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            acadDocT = DocumentCollectionExtension.Open(BaseObjs._acadDocs, nameT, false);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        acadDocT = DocumentCollectionExtension.Open(BaseObjs._acadDocs, nameT, false);
                    }
                }
            }
            else
            {
                acadDocT = DocumentCollectionExtension.Add(BaseObjs._acadDocs, dwgTemplate);
                acadDocT.save();

                //BaseObjs.sendCommand(System.Convert.ToString(27));
                //BaseObjs.sendCommand("filedia\r0\r");
                //BaseObjs.sendCommand(string.Format("saveas\r2013\r{0}\r", nameT));
                //BaseObjs.sendCommand("filedia\r1\r");
            }

            if (type == "TOPO")
            {
                if (idPolys3d.Count > 0)
                {
                    foreach (polyInfo p in pInfo)
                    {
                        p.pnts3d.addPoly3d(p.layer);
                    }
                }

                if (namePntFile != null)
                {
                    transferPoints("Import", namePntFile, BaseObjsCom.aeccSurvDoc);
                }
            }
            else if (type == "CONT")
            {
                if (idPolys3d.Count > 0)
                {
                    foreach (polyInfo p in pInfo)
                    {
                        p.pnts3d.addPoly3d(p.layer);
                    }
                }

                if (namePntFile != null)
                {
                    transferPoints("Import", namePntFile, BaseObjsCom.aeccSurvDoc);
                }
            }
        }

        public static void
        transferPoints(string direction, string namePntFile, AeccSurveyDocument civDoc)
        {
            if (direction == "Export")
            {
                if (File.Exists(TempPntFile))
                {
                    File.Delete(TempPntFile);
                }

                AeccPointExportOptions pntExportOpts = new AeccPointExportOptions();
                pntExportOpts.AdjustElevation = false;
                pntExportOpts.ExpandCoordinateData = false;
                pntExportOpts.TransformCoordinate = false;
                pntExportOpts.UsePointGroup = false;

                AeccPoints aeccPnts = civDoc.Points;
                aeccPnts.ExportPoints(TempPntFile, "PNEZD (comma delimited)", pntExportOpts);
            }
            else if (direction == "Import")
            {
                AeccPointImportOptions pntImportOpts = new AeccPointImportOptions();
                pntImportOpts.AdjustElevation = false;
                pntImportOpts.ExpandCoordinateData = false;
                pntImportOpts.PointDuplicateResolution = AeccPointDuplicateResolutionType.aeccPointDuplicateOverwrite;
                pntImportOpts.TransformCoordinate = false;
                pntImportOpts.UsePointGroup = true;
                pntImportOpts.PointGroup = "EXIST";

                Layer.manageLayers("EXIST");

                civDoc.Points.ImportPoints(namePntFile, "PNEZD (comma delimited)", pntImportOpts);
            }
        }

        public struct polyInfo
        {
            public string layer;
            public Point3dCollection pnts3d;

            public polyInfo(Point3dCollection pnts3D, string Layer)
            {
                pnts3d = pnts3D;
                layer = Layer;
            }
        }
    }
}