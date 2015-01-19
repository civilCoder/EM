using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Base_Tools45
{
    /// <summary>
    ///
    /// </summary>
    public static class VPort
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static ObjectId
        getViewportTable(Transaction tr, Database db)
        {
            ObjectId id = ObjectId.Null;
            try
            {
                using (tr)
                {
                    ViewportTable vpT = (ViewportTable)tr.GetObject(db.ViewportTableId, OpenMode.ForRead);
                    id = vpT.ObjectId;
                }
            }
            catch (System.Exception ex)
            {
                BaseObjs.writeDebug(ex.Message + " VPort.cs: line: 31");
            }
            return id;
        }

        public static void
        zoomWindow(Extents3d ex3d)
        {
            Point2d min = new Point2d(ex3d.MinPoint.X, ex3d.MinPoint.Y);
            Point2d max = new Point2d(ex3d.MaxPoint.X, ex3d.MaxPoint.Y);
            ViewTableRecord view = new ViewTableRecord();
            view.CenterPoint = min + (max - min) / 2;
            view.Height = max.Y - min.Y;
            view.Width = max.X - min.X;
            BaseObjs._editor.SetCurrentView(view);
        }

        public static void
        zoomWindow(Point2d min, Point2d max)
        {
            ViewTableRecord view = new ViewTableRecord();
            view.CenterPoint = min + (max - min) / 2;
            view.Height = max.Y - min.Y;
            view.Width = max.X - min.X;
            BaseObjs._editor.SetCurrentView(view);
        }

    }
}
