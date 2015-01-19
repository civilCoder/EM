using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Base_Tools45;
using System.Collections.Generic;
using System.Timers;

namespace fixGradeTagLinks
{
    public class Flash
    {
        private static System.Timers.Timer t;

        private static int cnt;
        private Point3d pC, pUR, pUL, pLL, pLR;
        private List<Vector3d> v3ds = new List<Vector3d>();
        private Plane pln = new Plane();
        private ObjectIdCollection ids = null;

        public Flash(Point3d pnt3dC, double width, double height)
        {
            pC = pnt3dC;
            pUR = pC.traverse(0.70, 50);
            pUL = pC.traverse(2.20, 50);
            pLL = pC.traverse(3.80, 50);
            pLR = pC.traverse(5.30, 50);

            Vector3d v3d = pUR - pC;
            v3ds.Add(v3d);
            v3d = pUL - pC;
            v3ds.Add(v3d);
            v3d = pLL - pC;
            v3ds.Add(v3d);
            v3d = pLR - pC;
            v3ds.Add(v3d);
            cnt = 0;

            ids = new ObjectIdCollection();
        }

        private static int i
        {
            get
            {
                return cnt;
            }
            set
            {
                cnt = value;
            }
        }

        public void
        activateFlash()
        {
            t = new Timer();
            t.Interval = 1000;
            t.Enabled = true;
            t.Start();
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
        }

        public void
        deactivateFlash()
        {
            App.ids = ids;

            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                foreach (ObjectId id in App.ids)
                    id.delete();
                tr.TransactionManager.QueueForGraphicsFlush();
                tr.Commit();
            }
        }

        private void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            DBText txt = null;
            using (Transaction tr = BaseObjs.startTransactionDb())
            {
                BlockTableRecord ms = Blocks.getBlockTableRecordMS();
                cnt++;

                foreach (Vector3d v3d in v3ds)
                {
                    txt = new DBText();
                    Point3d pX = pC.traverse(v3d.AngleOnPlane(pln), v3d.Length * (1 - 0.1 * i));
                    txt.SetDatabaseDefaults();
                    txt.TextString = "*";
                    txt.Position = pX;
                    ids.Add(txt.ObjectId);
                    ms.AppendEntity(txt);
                    tr.AddNewlyCreatedDBObject(txt, true);
                    ids.Add(txt.ObjectId);
                }
                tr.TransactionManager.QueueForGraphicsFlush();
                tr.Commit();
            }
        }
    }
}