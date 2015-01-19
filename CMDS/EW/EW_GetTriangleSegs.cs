using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Base_Tools45;
using Base_Tools45.C3D;
using EW.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace EW
{
    public class EdgeCoords {
        public double X1;
        public double Y1;
        public double Z1;
        public double X2;
        public double Y2;
        public double Z2;
    }

    public class EdgePnt{
        public Point3d p1;
        public Point3d p2;
    }

    public static class EW_GetTriangleSegs
    {
        static frmProgressBar fProgressBar;

        public static void getTriangleSegs(string strSurfaceName)
        {
            DateTime timeStart = DateTime.Now;
            string nameLayer = "";
            if (strSurfaceName == "CPNT-ON")
            {
                nameLayer = "CPNT";
            }

            nameLayer = string.Format("{0}-BRKLINE", nameLayer);
            Layer.manageLayers(nameLayer);

            TinSurface objSurface = Surf.getTinSurface(strSurfaceName);
          
            if (objSurface == null)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("CPNT-ON surface is not present - Needs data shortcut. Exiting......");
                return;
            }

            List<EdgePnt> ePnts = new List<EdgePnt>();

            TinSurfaceVertexCollection vertices = objSurface.Vertices;

            foreach (TinSurfaceVertex v in vertices)
            {
                TinSurfaceEdgeCollection edges = v.Edges;
                foreach (TinSurfaceEdge e in edges)
                {
                    EdgePnt ePnt = new EdgePnt { p1 = e.Vertex1.Location, p2 = e.Vertex2.Location };
                    ePnts.Add(ePnt);    
                }
            }
            
            fProgressBar = new frmProgressBar();
            fProgressBar.Show();

            int k = ePnts.Count;
            long lngOnePercent = (int)System.Math.Truncate( k / 100.0);
            string strStatus = null;
            List<int> lngMark = new List<int>();

            for (int i = 0; i < k - 1; i++)
            {
                EdgePnt ePnt1 = ePnts[i];
                
                for (int j = i + 1; j < k; j++)
                {
                    EdgePnt ePnt2 = ePnts[j];

                    if(ePnt1.p1.isEqual(ePnt2.p2, 0.1)){
                        if (ePnt1.p2.isEqual(ePnt2.p1, 0.1))
                        {
                            lngMark.Add(j);
                            break;                          
                        }
                    }
                }

                if (i % lngOnePercent == 0)
                {
                    strStatus = "Comparing " + i + " of " + k / 2 + " Objects";
                    fProgressBar.ProgressBar1.Increment(1);
                    fProgressBar.Text = strStatus;
                }
            }

            fProgressBar.ProgressBar1.Value = 0;

            k = lngMark.Count;
            lngOnePercent = (int)System.Math.Truncate(k / 100.0);

            List<Point3d> pnts3dList = new List<Point3d>();

            using (BaseObjs._acadDoc.LockDocument())
            {
                for (int i = 0; i < k; i++)
                {
                    pnts3dList = new List<Point3d>() { ePnts[lngMark[i]].p1, ePnts[lngMark[i]].p2 };

                    ObjectId id3dPoly = Draw.addPoly3d(pnts3dList);
                    id3dPoly.changeProp(nameLayer: nameLayer);

                    if (i % lngOnePercent == 0)
                    {
                        strStatus = "Importing " + i + " of " + k + " Breaklines";
                        fProgressBar.ProgressBar1.Increment(1);
                        fProgressBar.Text = strStatus;
                    }
                }
            }

            fProgressBar.Dispose();
            fProgressBar = null;

            DateTime timeEnd = DateTime.Now;

            string mess = string.Format("Elapsed Time: {0}", (timeEnd - timeStart).ToString());
            MessageBox.Show(mess);

        }
    }
}
