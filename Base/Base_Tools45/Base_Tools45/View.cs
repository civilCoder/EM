using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Internal;
using Autodesk.AutoCAD.Runtime;

using System.Drawing;
using System;

namespace Base_Tools45
{
    public static class View
    {
        private static ImageBGRA32 _img = null;

        static public void ZoomExtents()
        {
            // Zoom to the extents of the current space
            Zoom(new Point3d(), new Point3d(), new Point3d(), 1.01);
        }

        static public void
        Zoom(Point3d pnt3dMin, Point3d pnt3dMax, Point3d pnt3dCen, double scaleFactor){
            Document doc = BaseObjs._acadDoc;
            Database db = doc.Database;

            int nCurVport = System.Convert.ToInt32(Application.GetSystemVariable("CVPORT"));
            if(db.TileMode == true){
                if(pnt3dMin.Equals(new Point3d()) && pnt3dMax.Equals(new Point3d())){
                    pnt3dMin = db.Extmin;
                    pnt3dMax = db.Extmax;
                }                
            }else{
                if(nCurVport == 1){
                    if(pnt3dMin.Equals(new Point3d()) && pnt3dMax.Equals(new Point3d())){
                        pnt3dMin = db.Pextmin;
                        pnt3dMax = db.Pextmax;
                    }                
                   
                }else{
                    if(pnt3dMin.Equals(new Point3d()) && pnt3dMax.Equals(new Point3d())){
                        pnt3dMin = db.Extmin;
                        pnt3dMax = db.Extmax;
                    }                                    
                }
            }

            using(var tr = BaseObjs.startTransactionDb()){
                using( var acView = doc.Editor.GetCurrentView()){
                    Extents3d ext3d;
                    Matrix3d mtxWCS2DCS;
                    mtxWCS2DCS = Matrix3d.PlaneToWorld(acView.ViewDirection);
                    mtxWCS2DCS = Matrix3d.Displacement(acView.Target - Point3d.Origin) * mtxWCS2DCS;
                    mtxWCS2DCS = Matrix3d.Rotation(-acView.ViewTwist, acView.ViewDirection, acView.Target) * mtxWCS2DCS;

                    if(pnt3dCen.DistanceTo(Point3d.Origin) != 0){
                        pnt3dMin = new Point3d(pnt3dCen.X - (acView.Width / 2),
                                               pnt3dCen.Y - (acView.Height / 2),
                                               0);
                        pnt3dMax = new Point3d(pnt3dCen.X + (acView.Width / 2),
                                               pnt3dCen.Y + (acView.Height / 2),
                                               0);                    
                    }

                    using(Line line = new Line(pnt3dMin, pnt3dMax)){
                        ext3d = new Extents3d(line.Bounds.Value.MinPoint,
                                              line.Bounds.Value.MaxPoint);
                    }

                    double viewRatio = (acView.Width / acView.Height);
                    mtxWCS2DCS = mtxWCS2DCS.Inverse();
                    ext3d.TransformBy(mtxWCS2DCS);

                    double width, height;
                    Point2d pnt2dCenNew;

                    if(pnt3dCen.DistanceTo(Point3d.Origin) != 0){
                        width = acView.Width;
                        height = acView.Height;

                        if(scaleFactor == 0){
                            pnt3dCen = pnt3dCen.TransformBy(mtxWCS2DCS);
                        }

                        pnt2dCenNew = new Point2d(pnt3dCen.X, pnt3dCen.Y);
                    }else{
                        width = ext3d.MaxPoint.X - ext3d.MinPoint.X;
                        height = ext3d.MaxPoint.Y - ext3d.MinPoint.Y;

                        pnt2dCenNew = new Point2d(((ext3d.MaxPoint.X + ext3d.MinPoint.X) / 2),
                                                  ((ext3d.MaxPoint.Y + ext3d.MinPoint.Y) / 2));
                    }

                    if(width > (height * viewRatio)) height = width / viewRatio;

                    if(scaleFactor != 0){
                        acView.Height = height * scaleFactor;
                        acView.Width = width * scaleFactor;
                    }

                    acView.CenterPoint = pnt2dCenNew;

                    doc.Editor.SetCurrentView(acView);

                }
                tr.Commit();
            }


        }

        public static void
        highlightNestedEntity()
        {
            Document doc =
                Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            PromptNestedEntityResult rs =
                ed.GetNestedEntity("\nSelect nested entity: ");

            if (rs.Status == PromptStatus.OK)
            {
                ObjectId[] objIds = rs.GetContainers();
                ObjectId ensel = rs.ObjectId;
                int len = objIds.Length;

                // Reverse the "containers" list
                ObjectId[] revIds = new ObjectId[len + 1];
                for (int i = 0; i < len; i++)
                {
                    ObjectId id =
                        (ObjectId)objIds.GetValue(len - i - 1);
                    revIds.SetValue(id, i);
                }
                // Now add the selected entity to the end
                revIds.SetValue(ensel, len);

                // Retrieve the sub-entity path for this entity
                SubentityId subEnt =
                    new SubentityId(SubentityType.Null, (IntPtr)0);
                FullSubentityPath path =
                    new FullSubentityPath(revIds, subEnt);

                try
                {
                    using (Transaction tr = BaseObjs.startTransactionDb())
                    {
                        // Open the outermost container...
                        ObjectId id = (ObjectId)revIds.GetValue(0);
                        Entity ent =
                            (Entity)tr.GetObject(id, OpenMode.ForRead);
                        // ... and highlight the nested entity
                        ent.Highlight(path, false);
                        tr.Commit();
                    }
                }
                catch (System.Exception ex)
                {
                BaseObjs.writeDebug(ex.Message + " View.cs: line: 162");
                }
            }
        }

        public static void
        addCursorBadge(){

            Point[] pnts = new Point[17]{                
                new Point(0,0),
                new Point(1,0),
                new Point(15,15),
                new Point(29,0),
                new Point(30,0),
                new Point(30,1),
                new Point(16,15),
                new Point(30,29),
                new Point(30,30),
                new Point(29, 30),
                new Point(15,16),
                new Point(1,30),
                new Point(0,30),
                new Point(0,29),
                new Point(14,15),
                new Point(0,1),
                new Point(0,0)
            };

            if(_img == null){
                using(var bmp = new Bitmap(30,30)){
                    using(var g = Graphics.FromImage(bmp)){
                        g.Clear(Color.Magenta);
                        g.FillPolygon(Brushes.Red, pnts);
                    }
                    _img = Utils.ConvertBitmapToAcGiImageBGRA32(bmp);
                }
            }
            var cbu = new CursorBadgeUtilities();
            cbu.AddSupplementalCursorImage(_img, 3);
        }

        public static void
        remCursorBadge(){
            var cbu = new CursorBadgeUtilities();
            if (cbu.HasSupplementalCursorImage() && _img != null)
                cbu.RemoveSupplementalCursorImage(_img);
        }
    }
}
